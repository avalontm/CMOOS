using MOOS.FS;
using MOOS.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using static MOOS.PIC;
using static MOOS.SATA;

namespace MOOS.Driver
{
    public unsafe static class IDE
    {
        public static List<IDEDevice> Ports;

        public static void Initialize()
        {
            Ports = new ();
            ScanPorts(Channels.Primary);
            ScanPorts(Channels.Secondary);
            if (Ports.Count != 0)
            Console.WriteLine("[IDE] IDE controller Initialized");
        }

        public enum Channels
        {
            Primary,
            Secondary
        }

        public static void ScanPorts(Channels index)
        {
            ushort LBALowPort;
            ushort LBAMidPort;
            ushort LBAHighPort;
            ushort DeviceHeadPort;
            ushort StatusPort;
            ushort CommandPort;
            ushort AltStatusPort;
            ushort DataPort;
            ushort FeaturePort;
            ushort ErrorPort;
            ushort SectorCountPort;

            ushort BasePort = 0, ControlPort = 0;

            bool Available()
            {
                Native.Out8(LBALowPort, 0x88);
                return Native.In8(LBALowPort) == 0x88;
            }

            bool WaitForReadyStatus()
            {
                byte status;
                do
                {
                    status = Native.In8(StatusPort);
                }
                while ((status & IDEDevice.Busy) == IDEDevice.Busy);

                return true;
            }

            bool WaitForIdentifyData()
            {
                byte status;
                do
                {
                    status = Native.In8(StatusPort);
                }
                while ((status & IDEDevice.DataRequest) != IDEDevice.DataRequest && (status & IDEDevice.Error) != IDEDevice.Error);
                return ((status & IDEDevice.Error) != IDEDevice.Error);
            }

            switch (index)
            {
                case Channels.Primary:
                    BasePort = 0x1F0;
                    ControlPort = 0x3F6;
                    break;
                case Channels.Secondary:
                    BasePort = 0x170;
                    ControlPort = 0x376;
                    break;
            }

            DataPort = (ushort)(BasePort + 0);
            ErrorPort = (ushort)(BasePort + 1);
            FeaturePort = (ushort)(BasePort + 1);
            SectorCountPort = (ushort)(BasePort + 2);
            LBALowPort = (ushort)(BasePort + 3);
            LBAMidPort = (ushort)(BasePort + 4);
            LBAHighPort = (ushort)(BasePort + 5);
            DeviceHeadPort = (ushort)(BasePort + 6);
            CommandPort = (ushort)(BasePort + 7);
            StatusPort = (ushort)(BasePort + 7);
            AltStatusPort = (ushort)(ControlPort + 6);

            if (!Available()) return;

            // Disable IRQ
            Native.Out8(ControlPort, 0x2);

            for (byte port = 0; port < 2; port++)
            {
                Native.Out8(DeviceHeadPort, (byte)((port == 0) ? 0xA0 : 0xB0));
                Native.Out8(SectorCountPort, 0);
                Native.Out8(LBALowPort, 0);
                Native.Out8(LBAMidPort, 0);
                Native.Out8(LBAHighPort, 0);
                Native.Out8(CommandPort, IDEDevice.IdentifyDrive);

                if (
                    Native.In8(StatusPort) == 0 ||
                    !WaitForReadyStatus() ||
                    Native.In8(LBAMidPort) != 0 && Native.In8(LBAHighPort) != 0 || //ATAPI
                    !WaitForIdentifyData()
                    )
                {
                    continue;
                }

                var DriveInfo = new ushort[256];
                IOPort io = new IOPort(DataPort);
                io.Read16(DriveInfo);

                var mModel = DriveInfo.GetString(27, 40);
                var mSerialNo = DriveInfo.GetString(10, 20);
                var mSize = (DriveInfo.ToUInt32(60) + IDEDevice.MaxLBA28) * IDEDevice.SectorSize;
              
                Console.WriteLine($"[IDE] {mModel}");
                Console.WriteLine($"Model: {mSerialNo}");
                Console.WriteLine($"Size: {(mSize / (1024 * 1024))} Mb");

                DriveInfo.Dispose();

                var drv = new IDEDevice();

                drv.LBALowPort = LBALowPort;
                drv.LBAMidPort = LBAMidPort;
                drv.LBAHighPort = LBAHighPort;
                drv.DeviceHeadPort= DeviceHeadPort;
                drv.StatusPort= StatusPort;
                drv.CommandPort= CommandPort;
                drv.FeaturePort = FeaturePort;

                drv.DataPort = DataPort;
                drv.SectorCountPort = SectorCountPort;

                drv.Drive = port;

                drv.Size = mSize;

                Ports.Add(drv);
            }
        }
    }

    internal static class Misc
    {
        internal static ushort ToUInt16(this ushort[] xBuff, int loc)
        {
            // Combinar dos bytes consecutivos en un valor UInt16
            ushort result = (ushort)((xBuff[loc + 1] << 8) | xBuff[loc]);

            return result;
        }

        internal static ushort ToUInt16(this byte[] xBuff, int loc)
        {
            // Combinar dos bytes consecutivos en un valor UInt16
            ushort result = (ushort)((xBuff[loc + 1] << 8) | xBuff[loc]);

            return result;
        }


        internal static byte ToUInt8(this ushort[] xBuff, int loc)
        {
            return (byte)(xBuff[loc] & 0xFF);
        }

        internal static UInt32 ToUInt32(this ushort[] xBuff, int loc)
        {
            return (UInt32)(xBuff[loc + 1] << 16 | xBuff[loc]);
        }

        internal static UInt64 ToUInt64(this ushort[] xBuff, int loc)
        {
            return (UInt64)(xBuff[loc + 3] << 48 | xBuff[loc + 2] << 32 | xBuff[loc + 1] << 16 | xBuff[loc]);
        }

        internal static UInt64 ToUInt48(this ushort[] xBuff, int loc)
        {
            return (UInt64)(xBuff[loc + 2] << 32 | xBuff[loc + 1] << 16 | xBuff[loc]);
        }

        internal static string GetString(this ushort[] xBuff, int loc, int length)
        {
            char[] xResult = new char[length];
            for (int k = 0; k < (length / 2); k++)
            {
                xResult[k * 2] = (char)((xBuff[loc + k] >> 8) & 0xFF);
                xResult[k * 2 + 1] = (char)(xBuff[loc + k] & 0xFF);
            }
            return new String(xResult);
        }

        internal static string GetString(this byte[] xBuff, int loc, int length)
        {
            char[] xResult = new char[length];
            for (int k = 0; k < (length / 2); k++)
            {
                xResult[k * 2] = (char)((xBuff[loc + k] >> 8) & 0xFF);
                xResult[k * 2 + 1] = (char)(xBuff[loc + k] & 0xFF);
            }
            return new String(xResult);
        }
    }

    public unsafe class IDEDevice : Disk
    {
        public ushort LBALowPort;
        public ushort LBAMidPort;
        public ushort LBAHighPort;
        public ushort DeviceHeadPort;
        public ushort StatusPort;
        public ushort CommandPort;

        public const byte ReadSectorsWithRetry = 0x20;
        public const byte WriteSectorsWithRetry = 0x30;
        public const byte IdentifyDrive = 0xEC;
        public const byte CacheFlush = 0xE7;

        public const byte Busy = 1 << 7;
        public const byte DataRequest = 1 << 3;
        public const byte Error = 1 << 0;

        public const uint ModelNumber = 0x1B * 2;
        public const uint MaxLBA28 = 60 * 2;

        public const uint DrivesPerConroller = 2;

        public ushort DataPort;
        public ushort SectorCountPort;

        public const uint SectorSize = 512;

        public uint Drive;

        public ulong Size;

        public ushort FeaturePort;

        public bool ReadOrWrite(uint sector, byte* data, bool write)
        {
            Native.Out8(DeviceHeadPort, (byte)(0xE0 | (Drive << 4) | ((sector >> 24) & 0x0F)));
           // Native.Out8(FeaturePort, 0);
            Native.Out8(SectorCountPort, 1);
            Native.Out8(LBAHighPort, (byte)((sector >> 16) & 0xFF));
            Native.Out8(LBAMidPort, (byte)((sector >> 8) & 0xFF));
            Native.Out8(LBALowPort, (byte)(sector & 0xFF));

            Native.Out8(CommandPort, (write) ? WriteSectorsWithRetry : ReadSectorsWithRetry);

            if (!WaitForReadyStatus())
                return false;

            while ((Native.In8(StatusPort) & 0x80) != 0) ;

            if (write)
            {
                Native.Outsw(DataPort, (ushort*)data, SectorSize / 2);

                Native.Out8(CommandPort, CacheFlush);

                WaitForReadyStatus();
            }
            else
            {
                Native.Insw(DataPort, (ushort*)data, SectorSize / 2);
            }

            if ((Native.In8(StatusPort) & 0x1) != 0)
            {
                Console.WriteLine($"IDE bad status");
                return false;
            }

            return true;
        }

        private bool WaitForReadyStatus()
        {
            byte status;
            do
            {
                status = Native.In8(StatusPort);
            }
            while ((status & Busy) == Busy);

            return true;
        }


        public override bool Read(ulong sector, uint count, byte* p)
        {
            for (ulong i = 0; i < count; i++)
            {
                bool b = ReadOrWrite((uint)(sector + i), p + (i * SectorSize), false);
                if (!b) return false;
            }
            return true;
        }

        public override bool Write(ulong sector, uint count, byte* p)
        {
            for (ulong i = 0; i < count; i++)
            {
                bool b = ReadOrWrite((uint)(sector + i), p + (i * SectorSize), true);
                if (!b) return false;
            }
            return true;
        }
    }
}