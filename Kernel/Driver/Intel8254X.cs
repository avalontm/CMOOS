//Reference: https://www.intel.com/content/dam/doc/manual/pci-pci-x-family-gbe-controllers-software-dev-manual.pdf

using MOOS.IO;
using MOOS.Memory;
using MOOS.Misc;
using MOOS.NET;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static MOOS.Misc.MMIO;


namespace MOOS.Driver
{
    public unsafe class Intel8254X : NetworkDevice
    {
        public uint RXDescs;
        public uint TXDescs;

        public int IRQ;

        protected PCIDevice pciCard;
        protected MACAddress mac;
        protected bool mInitDone;
        protected ManagedMemoryBlock rxBuffer;
        protected int rxBufferOffset;
        protected ushort capr;
        protected Queue<byte[]> mRecvBuffer;
        protected Queue<byte[]> mTransmitBuffer;
        private int mNextTXDesc;
        const ushort RxBufferSize = 32768;

        static Intel8254X Instance;

        private uint Base;

        public bool FullDuplex
        {
            get
            {
                return (rxBuffer.Read32(8) & (1 << 0)) != 0;
            }
        }
        public int Speed
        {
            get
            {
                if ((rxBuffer.Read32(8) & (3 << 6)) == 0)
                {
                    return 10;
                }
                if ((rxBuffer.Read32(8) & (2 << 6)) != 0)
                {
                    return 1000;
                }
                if ((rxBuffer.Read32(8) & (1 << 6)) != 0)
                {
                    return 100;
                }
                return 0;
            }
        }

        public Intel8254X()
        {
            Init();
        }

        public void Init()
        {
            PCIDevice device =null;

            for (int i = 0; i < PCI.Devices.Count; i++)
            {
                if (PCI.Devices[i] != null && PCI.Devices[i].VendorID == 0x8086 && 
                    (
                        PCI.Devices[i].DeviceID == 0x1000 || //Intel82542
                        PCI.Devices[i].DeviceID == 0x1001 || //Intel82543GC
                        PCI.Devices[i].DeviceID == 0x1004 || //Intel82543GC
                        PCI.Devices[i].DeviceID == 0x1008 || //Intel82544EI
                        PCI.Devices[i].DeviceID == 0x1009 || //Intel82544EI
                        PCI.Devices[i].DeviceID == 0x100C || //Intel82543EI
                        PCI.Devices[i].DeviceID == 0x100D || //Intel82544GC
                        PCI.Devices[i].DeviceID == 0x100E || //Intel82540EM
                        PCI.Devices[i].DeviceID == 0x100F || //Intel82545EM
                        PCI.Devices[i].DeviceID == 0x1010 || //Intel82546EB
                        PCI.Devices[i].DeviceID == 0x1011 || //Intel82545EM
                        PCI.Devices[i].DeviceID == 0x1012 || //Intel82546EB
                        PCI.Devices[i].DeviceID == 0x1013 || //Intel82541EI
                        PCI.Devices[i].DeviceID == 0x1014 || //Intel82541ER
                        PCI.Devices[i].DeviceID == 0x1015 || //Intel82540EM
                        PCI.Devices[i].DeviceID == 0x1016 || //Intel82540EP
                        PCI.Devices[i].DeviceID == 0x1017 || //Intel82540EP
                        PCI.Devices[i].DeviceID == 0x1018 || //Intel82541EI
                        PCI.Devices[i].DeviceID == 0x1019 || //Intel82547EI
                        PCI.Devices[i].DeviceID == 0x101A || //Intel82547EI
                        PCI.Devices[i].DeviceID == 0x101D || //Intel82546EB
                        PCI.Devices[i].DeviceID == 0x101E || //Intel82540EP
                        PCI.Devices[i].DeviceID == 0x1026 || //Intel82545GM
                        PCI.Devices[i].DeviceID == 0x1027 || //Intel82545GM
                        PCI.Devices[i].DeviceID == 0x1028 || //Intel82545GM
                        PCI.Devices[i].DeviceID == 0x1049 || //Intel82566MM_ICH8
                        PCI.Devices[i].DeviceID == 0x104A || //Intel82566DM_ICH8
                        PCI.Devices[i].DeviceID == 0x104B || //Intel82566DC_ICH8
                        PCI.Devices[i].DeviceID == 0x104C || //Intel82562V_ICH8
                        PCI.Devices[i].DeviceID == 0x104D || //Intel82566MC_ICH8
                        PCI.Devices[i].DeviceID == 0x105E || //Intel82571EB
                        PCI.Devices[i].DeviceID == 0x105F || //Intel82571EB
                        PCI.Devices[i].DeviceID == 0x1060 || //Intel82571EB
                        PCI.Devices[i].DeviceID == 0x1075 || //Intel82547EI
                        PCI.Devices[i].DeviceID == 0x1076 || //Intel82541GI
                        PCI.Devices[i].DeviceID == 0x1077 || //Intel82547EI
                        PCI.Devices[i].DeviceID == 0x1078 || //Intel82541ER
                        PCI.Devices[i].DeviceID == 0x1079 || //Intel82546EB
                        PCI.Devices[i].DeviceID == 0x107A || //Intel82546EB
                        PCI.Devices[i].DeviceID == 0x107B || //Intel82546EB
                        PCI.Devices[i].DeviceID == 0x107C || //Intel82541PI
                        PCI.Devices[i].DeviceID == 0x107D || //Intel82572EI
                        PCI.Devices[i].DeviceID == 0x107E || //Intel82572EI
                        PCI.Devices[i].DeviceID == 0x107F || //Intel82572EI
                        PCI.Devices[i].DeviceID == 0x108A || //Intel82546GB
                        PCI.Devices[i].DeviceID == 0x108B || //Intel82573E
                        PCI.Devices[i].DeviceID == 0x108C || //Intel82573E
                        PCI.Devices[i].DeviceID == 0x1096 || //Intel80003ES2LAN
                        PCI.Devices[i].DeviceID == 0x1098 || //Intel80003ES2LAN
                        PCI.Devices[i].DeviceID == 0x1099 || //Intel82546GB
                        PCI.Devices[i].DeviceID == 0x109A || //Intel82573L
                        PCI.Devices[i].DeviceID == 0x10A4 || //Intel82571EB
                        PCI.Devices[i].DeviceID == 0x10A7 || //Intel82575
                        PCI.Devices[i].DeviceID == 0x10A9 || //Intel82575_serdes
                        PCI.Devices[i].DeviceID == 0x10B5 || //Intel82546GB
                        PCI.Devices[i].DeviceID == 0x10B9 || //Intel82572EI
                        PCI.Devices[i].DeviceID == 0x10BA || //Intel80003ES2LAN
                        PCI.Devices[i].DeviceID == 0x10BB || //Intel80003ES2LAN
                        PCI.Devices[i].DeviceID == 0x10BC || //Intel82571EB
                        PCI.Devices[i].DeviceID == 0x10BD || //Intel82566DM_ICH9
                        PCI.Devices[i].DeviceID == 0x10C4 || //Intel82562GT_ICH8
                        PCI.Devices[i].DeviceID == 0x10C5 || //Intel82562G_ICH8
                        PCI.Devices[i].DeviceID == 0x10C9 || //Intel82576
                        PCI.Devices[i].DeviceID == 0x10D3 || //Intel82574L
                        PCI.Devices[i].DeviceID == 0x10A9 || //Intel82575_quadcopper
                        PCI.Devices[i].DeviceID == 0x10CB || //Intel82567V_ICH9
                        PCI.Devices[i].DeviceID == 0x10E5 || //Intel82567LM_4_ICH9
                        PCI.Devices[i].DeviceID == 0x10EA || //Intel82577LM
                        PCI.Devices[i].DeviceID == 0x10EB || //Intel82577LC
                        PCI.Devices[i].DeviceID == 0x10EF || //Intel82578DM
                        PCI.Devices[i].DeviceID == 0x10F0 || //Intel82578DC
                        PCI.Devices[i].DeviceID == 0x10F5 || //Intel82567LM_ICH9_egDellE6400Notebook
                        PCI.Devices[i].DeviceID == 0x1502 || //Intel82579LM
                        PCI.Devices[i].DeviceID == 0x1503 || //Intel82579V
                        PCI.Devices[i].DeviceID == 0x150A || //Intel82576NS
                        PCI.Devices[i].DeviceID == 0x150E || //Intel82580
                        PCI.Devices[i].DeviceID == 0x1521 || //IntelI350
                        PCI.Devices[i].DeviceID == 0x1533 || //IntelI210
                        PCI.Devices[i].DeviceID == 0x157B || //IntelI210
                        PCI.Devices[i].DeviceID == 0x153A || //IntelI217LM
                        PCI.Devices[i].DeviceID == 0x153B || //IntelI217VA
                        PCI.Devices[i].DeviceID == 0x1559 || //IntelI218V
                        PCI.Devices[i].DeviceID == 0x155A || //IntelI218LM
                        PCI.Devices[i].DeviceID == 0x15A0 || //IntelI218LM2
                        PCI.Devices[i].DeviceID == 0x15A1 || //IntelI218V
                        PCI.Devices[i].DeviceID == 0x15A2 || //IntelI218LM3
                        PCI.Devices[i].DeviceID == 0x15A3 || //IntelI218V3
                        PCI.Devices[i].DeviceID == 0x156F || //IntelI219LM
                        PCI.Devices[i].DeviceID == 0x1570 || //IntelI219V
                        PCI.Devices[i].DeviceID == 0x15B7 || //IntelI219LM2
                        PCI.Devices[i].DeviceID == 0x15B8 || //IntelI219V2
                        PCI.Devices[i].DeviceID == 0x15BB || //IntelI219LM3
                        PCI.Devices[i].DeviceID == 0x15D7 || //IntelI219LM
                        PCI.Devices[i].DeviceID == 0x15E3    //IntelI219LM
                        )
                    )
                {
                    device = PCI.Devices[i];
                }
            }

            if (device == null)
            {
                //throw new ArgumentException("PCI Device is null. Unable to get Realtek 8139 card");
                Console.WriteLine("PCI Device is null. Unable to get Intel 8254X card");
                return;
            }

            Console.WriteLine("[Intel8254X] Intel 8254X Series Gigabit Ethernet Controller Found");
            rxBuffer.Write32(0x04, 0x04 | 0x02 | 0x01);

            Base = device.BaseAddressBar[0].BaseAddress;
            Console.Write("[Intel8254X] BAR0: 0x");
            Console.WriteLine(((ulong)Base).ToStringHex());

            // Get a receive buffer and assign it to the card
            rxBuffer = new ManagedMemoryBlock(RxBufferSize + 2048 + 16, 4);
            RBStartRegister = (uint)rxBuffer.Offset;
            // Setup receive Configuration
            RecvConfigRegister = 0xF381;
            // Setup Transmit Configuration
            TransmitConfigRegister = 0x3000300;
            // Setup Interrupts
            IntMaskRegister = 0x7F;
            IntStatusRegister = 0xFFFF;
            // Setup our Receive and Transmit Queues
            mRecvBuffer = new Queue<byte[]>();
            mTransmitBuffer = new Queue<byte[]>();

            //Do a software reset
            SoftwareReset();

            rxBuffer.Write32(0x14, 0x1);
            bool HasEEPROM = false;
            for (int i = 0; i < 1024; i++)
            {
                if ((rxBuffer.Read32(0x14) & 0x10) != 0)
                {
                    HasEEPROM = true;
                    break;
                }
            }

            //Must be set
            if (!HasEEPROM)
            {
                mac = new MACAddress( new byte[] {
                    In8((byte*)(Base + 0x5400)),
                    In8((byte*)(Base + 0x5401)),
                    In8((byte*)(Base + 0x5402)),
                    In8((byte*)(Base + 0x5403)),
                    In8((byte*)(Base + 0x5404)),
                    In8((byte*)(Base + 0x5405))
                });
                Console.WriteLine("[Intel8254X] This controller has no EEPROM");
            }
            else
            {
                mac = new MACAddress(new byte[] {
                    (byte)(ReadROM(0) & 0xFF),
                    (byte)(ReadROM(0) >> 8),
                    (byte)(ReadROM(1) & 0xFF),
                    (byte)(ReadROM(1) >> 8),
                    (byte)(ReadROM(2) & 0xFF),
                    (byte)(ReadROM(2) >> 8)
                });
                Console.WriteLine("[Intel8254X] EEPROM on this controller");
            }

            Console.WriteLine($"[Intel8254X] MAC: {mac}");

            Linkup();
            for (int i = 0; i < 0x80; i++)
                rxBuffer.Write32((ushort)(0x5200 + i * 4), 0);

            Console.Write("[Intel8254X] IRQ: ");
            Console.WriteLine(((ulong)device.IRQ).ToString("x2"));

            RXInitialize();
            TXInitialize();

            rxBuffer.Write32(0x00D0, 0x1F6DC);
            rxBuffer.Read32(0xC0);

            Console.Write("[Intel8254X] Speed: ");
            Console.Write(((ulong)Speed).ToString());
            Console.Write(' ');
            Console.Write("FullDuplex: ");
            Console.WriteLine(FullDuplex?"Yes":"No");
            Console.WriteLine("[Intel8254X] Configuration Done");

            Interrupts.EnableInterrupt(device.IRQ, &OnInterrupt);
            IRQ = device.IRQ;

            Instance = this;

        }

        void Linkup()
        {
            rxBuffer.Write32(0, rxBuffer.Read32(0) | 0x40);

            Console.Write("[Intel8254X] Waiting for network connection ");
            Console.Wait((uint*)(Base + 0x08), 1);
            Console.WriteLine();
        }

        void TXInitialize()
        {
            TXDescs = (uint)Allocator.Allocate(8 * 16);

            for (int i = 0; i < 8; i++)
            {
                TXDesc* desc = (TXDesc*)(TXDescs + (i * 16));
                desc->addr = (ulong)Allocator.Allocate(65536);
                desc->cmd = 0;
            }

            rxBuffer.Write32(0x3800, TXDescs);
            rxBuffer.Write32(0x3804, 0);
            rxBuffer.Write32(0x3808, 8 * 16);
            rxBuffer.Write32(0x3810, 0);
            rxBuffer.Write32(0x3818, 0);

            rxBuffer.Write32(0x0400, (1 << 1) | (1 << 3));
        }

        public static uint RXCurr = 0;
        public static uint TXCurr = 0;

        void RXInitialize()
        {
            RXDescs = (uint)Allocator.Allocate(32 * 16);

            for (uint i = 0; i < 32; i++)
            {
                RXDesc* desc = (RXDesc*)(RXDescs + (i * 16));
                desc->addr = (ulong)(void*)Allocator.Allocate(2048 + 16);
                desc->status = 0;
            }

            rxBuffer.Write32(0x2800, RXDescs);
            rxBuffer.Write32(0x2804, 0);

            rxBuffer.Write32(0x2808, 32 * 16);
            rxBuffer.Write32(0x2810, 0);
            rxBuffer.Write32(0x2818, 32 - 1);

            rxBuffer.Write32(0x0100,
                     (1 << 1) |
                     (1 << 2) |
                     (1 << 3) |
                     (1 << 4) |
                     (0 << 6) |
                     (0 << 8) |
                    (1 << 15) |
                    (1 << 26) |
                    (0 << 16)
                );
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RXDesc
        {
            public ulong addr;
            public ushort length;
            public ushort checksum;
            public byte status;
            public byte errors;
            public ushort special;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TXDesc
        {
            public ulong addr;
            public ushort length;
            public byte cso;
            public byte cmd;
            public byte status;
            public byte css;
            public ushort special;
        }

        ushort ReadROM(uint Addr)
        {
            uint Temp;
            rxBuffer.Write32(0x14, 1 | (Addr << 8));
            while (((Temp = rxBuffer.Read32(0x14)) & 0x10) == 0) ;
            return ((ushort)((Temp >> 16) & 0xFFFF));
        }

        public static void OnInterrupt()
        {
            uint Status = Instance.rxBuffer.Read32(0xC0);

            if ((Status & 0x04) != 0)
            {
                //Console.WriteLine("[Intel8254X] Linking Up");
                //Linkup();
            }
            if ((Status & 0x10) != 0)
            {
                //Console.WriteLine("[Intel8254X] Good Threshold");
            }

            if ((Status & 0x80) != 0)
            {
                //Console.WriteLine("[Intel8254X] Packet Received");
                uint _RXCurr = RXCurr;
                RXDesc* desc = (RXDesc*)(Instance.RXDescs + (RXCurr * 16));
                while ((desc->status & 0x1) != 0)
                {
                    //Ethernet.HandlePacket((byte*)desc->addr, desc->length);
                    //desc->addr;
                    desc->status = 0;
                    RXCurr = (RXCurr + 1) % 32;
                    Instance.rxBuffer.Write32(0x2818, _RXCurr);
                }
            }
        }

        private static byte Inb(uint port)
        {
            return new IOPort((ushort)port).Byte;
        }
        private static void OutB(uint port, byte val)
        {
            new IOPort((ushort)port).Byte = val;
        }

        private static ushort Inb16(uint port)
        {
            return new IOPort((ushort)port).Word;
        }
        private static void Out16(uint port, ushort val)
        {
            new IOPort((ushort)port).Word = val;
        }

        private static uint Inb32(uint port)
        {
            return new IOPort((ushort)port).DWord;
        }
        private static void Out32(uint port, uint val)
        {
            new IOPort((ushort)port).DWord = val;
        }

        /*
        public static List<RTL8139> FindAll()
        {
            Console.WriteLine("Scanning for Intel 8254X cards...");

            List<Intel8254X> cards = new List<Intel8254X>();

            for (int i = 0; i < PCI.Devices.Count; i++)
            {
                PCIDevice xDevice = PCI.Devices[i];
                if ((xDevice.VendorID == 0x10EC) && (xDevice.DeviceID == 0x8139) && (xDevice.Claimed == false))
                {
                    Intel8254X nic = new Intel8254X(xDevice);
                    cards.Add(nic);
                }
            }
            return cards;
        }
        */
        #region Register Access
        protected uint RBStartRegister
        {
            get { return Inb32(Base + 0x30); }
            set { Out32(Base + 0x30, value); }
        }
        internal uint RecvConfigRegister
        {
            get { return Inb32(Base + 0x44); }
            set { Out32(Base + 0x44, value); }
        }
        internal ushort CurAddressPointerRegister
        {
            get { return Inb16(Base + 0x38); }
            set { Out16(Base + 0x38, value); }
        }
        internal ushort CurBufferAddressRegister
        {
            get { return Inb16(Base + 0x3A); }
            set { Out16(Base + 0x3A, value); }
        }
        internal ushort IntMaskRegister
        {
            get { return Inb16(Base + 0x3C); }
            set { Out16(Base + 0x3C, value); }
        }
        internal ushort IntStatusRegister
        {
            get { return Inb16(Base + 0x3E); }
            set { Out16(Base + 0x3E, value); }
        }
        internal byte CommandRegister
        {
            get { return Inb(Base + 0x37); }
            set { OutB(Base + 0x37, value); }
        }
        protected byte MediaStatusRegister
        {
            get { return Inb(Base + 0x58); }
            set { OutB(Base + 0x58, value); }
        }
        protected byte Config1Register
        {
            get { return Inb(Base + 0x52); }
            set { OutB(Base + 0x52, value); }
        }
        internal uint TransmitConfigRegister
        {
            get { return Inb32(Base + 0x40); }
            set { Out32(Base + 0x40, value); }
        }
        internal uint TransmitAddress1Register
        {
            get { return Inb32(Base + 0x20); }
            set { Out32(Base + 0x20, value); }
        }
        internal uint TransmitAddress2Register
        {
            get { return Inb32(Base + 0x24); }
            set { Out32(Base + 0x24, value); }
        }
        internal uint TransmitAddress3Register
        {
            get { return Inb32(Base + 0x28); }
            set { Out32(Base + 0x28, value); }
        }
        internal uint TransmitAddress4Register
        {
            get { return Inb32(Base + 0x2C); }
            set { Out32(Base + 0x2C, value); }
        }
        internal uint TransmitDescriptor1Register
        {
            get { return Inb32(Base + 0x10); }
            set { Out32(Base + 0x10, value); }
        }
        internal uint TransmitDescriptor2Register
        {
            get { return Inb32(Base + 0x14); }
            set { Out32(Base + 0x14, value); }
        }
        internal uint TransmitDescriptor3Register
        {
            get { return Inb32(Base + 0x18); }
            set { Out32(Base + 0x18, value); }
        }
        internal uint TransmitDescriptor4Register
        {
            get { return Inb32(Base + 0x1C); }
            set { Out32(Base + 0x1C, value); }
        }
        #endregion
        protected bool CmdBufferEmpty
        {
            get { return ((CommandRegister & 0x01) == 0x01); }
        }

        #region Network Device Implementation
        public override MACAddress MACAddress
        {
            get { return this.mac; }
        }

        public override bool Enable()
        {
            // Enable Receiving and Transmitting of data
            CommandRegister = 0x0C;
            while (this.Ready == false)
            { }
            return true;
        }
        public override bool Ready
        {
            get { return ((Config1Register & 0x20) == 0); }
        }
        public override bool QueueBytes(byte[] buffer, int offset, int length)
        {
            byte[] data = new byte[length];
            for (int b = 0; b < length; b++)
            {
                data[b] = buffer[b + offset];
            }
            //Console.WriteLine("Try sending");
            if (SendBytes(ref data) == false)
            {
                // Console.WriteLine("Queuing");
                mTransmitBuffer.Enqueue(data);
            }
            return true;
        }
        public override bool ReceiveBytes(byte[] buffer, int offset, int max)
        {
            //throw new NotImplementedException();
            Console.WriteLine("NotImplementedException");
            return false;
        }
        public override byte[] ReceivePacket()
        {
            if (mRecvBuffer.Count < 1)
            {
                return null;
            }
            byte[] data = mRecvBuffer.Dequeue();
            return data;
        }
        public override int BytesAvailable()
        {
            if (mRecvBuffer.Count < 1)
            {
                return 0;
            }
            return mRecvBuffer.Peek().Length;
        }
        public override bool IsSendBufferFull()
        {
            return false;
        }
        public override bool IsReceiveBufferFull()
        {
            return false;
        }
        public override string Name
        {
            get { return "Realtek 8139 Chipset NIC"; }
        }

        public override CardType CardType => CardType.Ethernet;

        #endregion
        #region Helper Functions
        private void ReadRawData(ushort packetLen)
        {
            int recv_size = packetLen - 4;
            byte[] recv_data = new byte[recv_size];
            for (uint b = 0; b < recv_size; b++)
            {
                recv_data[b] = rxBuffer[(uint)(capr + 4 + b)];
            }
            if (DataReceived != null)
            {
                DataReceived(recv_data);
            }
            else
            {
                if (mRecvBuffer == null)
                {
                }
                mRecvBuffer.Enqueue(recv_data);
            }
            capr += (ushort)((packetLen + 4 + 3) & 0xFFFFFFFC);
            if (capr > RxBufferSize)
            {
                capr -= RxBufferSize;
            }
        }

        protected void SoftwareReset()
        {
            Console.Write("[Intel8254X] Reseting controller...");

            rxBuffer.Write32(0, 1 << 26);
            while (BitHelpers.IsBitSet(rxBuffer.Read32(0), 26)) ;
        }

        protected bool SendBytes(ref byte[] aData)
        {
            int txd = mNextTXDesc++;
            if (mNextTXDesc >= 4)
            {
                mNextTXDesc = 0;
            }
            ManagedMemoryBlock txBuffer;
            if (aData.Length < 64)
            {
                txBuffer = new ManagedMemoryBlock(64);
                for (uint b = 0; b < aData.Length; b++)
                {
                    txBuffer[b] = aData[b];
                }
            }
            else
            {
                txBuffer = new ManagedMemoryBlock((uint)aData.Length);
                for (uint i = 0; i < aData.Length; i++)
                {
                    txBuffer[i] = aData[i];
                }
            }
            switch (txd)
            {
                case 0:
                    TransmitAddress1Register = (uint)txBuffer.Offset;
                    TransmitDescriptor1Register = txBuffer.Size;
                    break;
                case 1:
                    TransmitAddress2Register = (uint)txBuffer.Offset;
                    TransmitDescriptor2Register = txBuffer.Size;
                    break;
                case 2:
                    TransmitAddress3Register = (uint)txBuffer.Offset;
                    TransmitDescriptor3Register = txBuffer.Size;
                    break;
                case 3:
                    TransmitAddress4Register = (uint)txBuffer.Offset;
                    TransmitDescriptor4Register = txBuffer.Size;
                    break;
                default:
                    return false;
            }
            return true;
        }
        #endregion
    }
}