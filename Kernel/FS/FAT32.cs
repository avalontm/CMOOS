using MOOS.Driver;
using MOOS.IO;
using MOOS.Memory;
using System;
using System.Collections.Generic;
using System.Common.Extentions;
using System.Runtime.InteropServices;
using System.Text;

namespace MOOS.FS
{
    public enum FatType : byte
    {
        /// <summary>
        /// Represents a 12-bit FAT.
        /// </summary>
        FAT12 = 12,

        /// <summary>
        /// Represents a 16-bit FAT.
        /// </summary>
        FAT16 = 16,

        /// <summary>
        /// Represents a 32-bit FAT.
        /// </summary>
        FAT32 = 32
    }

    public enum FileSystemType : byte
    {
        None = 0x0,
        FAT = 0x1
    }

    public class FAT32 : FileSystem
    { 
        // They should be private set only, so take care of this later
        internal UInt32 BytePerSector;
        internal UInt32 SectorsPerCluster;
        internal UInt32 ReservedSector;
        internal UInt32 TotalFAT;
        internal UInt32 DirectoryEntry;
        internal UInt32 TotalSectors;
        internal UInt32 SectorsPerFAT;
        internal UInt32 DataSectorCount;
        internal UInt32 ClusterCount;
        internal UInt32 SerialNo;
        internal UInt32 RootCluster;
        internal UInt32 RootSector;
        internal UInt32 RootSectorCount;
        internal UInt32 DataSector;
        internal UInt32 EntriesPerSector;
        internal UInt32 fatEntries;

        protected FatType FatType;

        protected string VolumeLabel;

        [DllImport("*")]
        private static extern void fatfs_init();
        IDEDevice device;
        public FAT32()
        {
            device = IDE.Ports[0];
         
            fatfs_init();
            bool isValid = IsFAT();
            Console.WriteLine($"FAT: {isValid}");
        }

        private unsafe bool IsFAT()
        {
            byte[] BootSector = new byte[512];

            fixed (byte* p = BootSector)
            {
                Native.Insw(device.DataPort, (ushort*)p, (ulong)(BootSector.Length / 2));

                for (int i = 0; i < BootSector.Length; i++)
                {
                    BootSector[i] = p[i];
                }
            }

           /*
            IOPort idevice = new IOPort((ushort)(device.DataPort));
            idevice.Read8(BootSector);
            */

            ushort xSig = BitConverter.ToUInt16(BootSector, 510);
            Console.WriteLine($"xSig: {xSig} == 43605");
            if (xSig != 0xAA55)
               // return false;

            /* BPB (BIOS Parameter Block) */
            BytePerSector = BitConverter.ToUInt16(BootSector, 11);
            SectorsPerCluster = BootSector[13];
            ReservedSector = BitConverter.ToUInt16(BootSector, 14);
            TotalFAT = BootSector[16];
            DirectoryEntry = BitConverter.ToUInt16(BootSector, 17);

            Console.WriteLine($"TotalFAT: {TotalFAT}");

            if (BitConverter.ToUInt16(BootSector, 19) == 0)
            {
                /* Large amount of sector on media. This field is set if there are more than 65535 sectors in the volume. */
                TotalSectors = BitConverter.ToUInt32(BootSector, 32);
            }
            else
            {
                TotalSectors = BitConverter.ToUInt16(BootSector, 19);
            }
           
            /* FAT 12 and FAT 16 ONLY */
            SectorsPerFAT = BitConverter.ToUInt16(BootSector, 22);

            if (SectorsPerFAT == 0)
            {
                /* FAT 32 ONLY */
                SectorsPerFAT = BitConverter.ToUInt32(BootSector, 36);
            }

            Console.WriteLine($"BytePerSector: {BytePerSector}");
            Console.WriteLine($"SectorsPerCluster: {SectorsPerCluster}");
            Console.WriteLine($"ReservedSector: {ReservedSector}");
            Console.WriteLine($"Total: {TotalFAT}");
            Console.WriteLine($"DirectoryEntry: {DirectoryEntry}");
            Console.WriteLine($"TotalSectors: {TotalSectors}");
            Console.WriteLine($"SectorsPerFAT: {SectorsPerFAT}");

            /* Not Necessary, To Avoid Crashes during corrupted BPB Info */
            // Just to prevent ourself from hacking
            if (TotalFAT == 0 || TotalFAT < 2 || BytePerSector == 0 || TotalSectors == 0 || SectorsPerCluster == 0)
                return false;

            /* Some basic calculations to check basic error :P */
            uint RootDirSectors = 0;
            DataSectorCount = TotalSectors - (ReservedSector + (TotalFAT * SectorsPerFAT) + RootDirSectors);
            ClusterCount = DataSectorCount / SectorsPerCluster;

            /* Finally we got key xD */
            if (ClusterCount < 4085)
                FatType = FatType.FAT12;
            else if (ClusterCount < 65525)
                FatType = FatType.FAT16;
            else
                FatType = FatType.FAT32;

            /* Now we open door of gold coins xDD */
            if (FatType == FatType.FAT32)
            {
                SerialNo = BitConverter.ToUInt32(BootSector, 39);
                fixed (byte* p = BootSector)
                {
                    VolumeLabel = new string((char*)p, 43, 11);   // for checking
                }
                RootCluster = BitConverter.ToUInt32(BootSector, 44);
                RootSector = 0;
                RootSectorCount = 0;
            }
            /* The key is of another door */
            else
            {
                SerialNo = BitConverter.ToUInt32(BootSector, 67);

                fixed (byte* p = BootSector)
                {
                    VolumeLabel = new string((char*)p, 43, 11);   // for checking
                }

                RootSector = ReservedSector + (TotalFAT * SectorsPerFAT);
                RootSectorCount = (UInt32)((DirectoryEntry * 32 + (BytePerSector - 1)) / BytePerSector);
                fatEntries = SectorsPerFAT * 512 / 4;
            }

            Console.WriteLine($"SerialNo: {SerialNo}");
            Console.WriteLine($"VolumeLabel: {VolumeLabel}");

            Console.WriteLine($"RootCluster: {RootCluster}");
            Console.WriteLine($"RootSector: {RootSector}");
            Console.WriteLine($"RootSectorCount: {RootSectorCount}");
            Console.WriteLine($"fatEntries: {fatEntries}");

            /* Now it shows our forward path ;) */
            EntriesPerSector = (UInt32)(BytePerSector / 32);
            DataSector = ReservedSector + (TotalFAT * SectorsPerFAT) + RootSectorCount;

            Console.WriteLine($"EntriesPerSector: {EntriesPerSector}");
            Console.WriteLine($"DataSector: {DataSector}");

            Console.WriteLine($"FatType: {(int)FatType}");

            //mFSType = FileSystemType.FAT;
            return true;
        }

        public override List<FileInfo> GetFiles(string Directory)
        {
          return new List<FileInfo>();
        }

        public override void Delete(string Name)
        {
           
        }

        public override byte[] ReadAllBytes(string Name)
        {
            return null;
        }

        public override void WriteAllBytes(string Name, byte[] Content)
        {
          
        }

        public override void Format()
        {
          
        }

        public override void CreateDirectory(string Name)
        {
            
        }
    }
}
