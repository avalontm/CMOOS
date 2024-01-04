using MOOS.Driver;
using MOOS.IO;
using MOOS.Memory;
using System;
using System.Collections.Generic;
using System.Common.Extentions;
using System.Diagnostics;
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

    public unsafe class Partition
    {
        protected IDEDevice mParent;
        protected uint mStartSector;
        protected uint mSectorCount;


        public Partition(IDEDevice device, uint aStartSector, uint aSectorCount)
        {
            this.mParent = device;
            this.mStartSector = aStartSector;
            this.mSectorCount = aSectorCount;
        }

        public bool Read(uint aSectorNo, uint aSectorCount, byte[] aData)
        {
            fixed (byte* p = aData)
            {
                if (aSectorNo + aSectorCount > mSectorCount)
                    return false;

                bool status =  mParent.Read(mStartSector + aSectorNo, aSectorCount, p);

                for (int i = 0; i < aData.Length; i++)
                {
                    aData[i] = p[i];
                }

                return status;
            }
        }
    }

    public class FAT32 : FileSystem
    {
        private UInt32 BytePerSector;
        private UInt32 SectorsPerCluster;
        private UInt32 ReservedSector;
        private UInt32 TotalFAT;
        private UInt32 DirectoryEntry;
        private UInt32 TotalSectors;
        private UInt32 SectorsPerFAT;
        private UInt32 DataSectorCount;
        private UInt32 ClusterCount;
        private FatType FatType;
        private UInt32 SerialNo;
        private UInt32 RootCluster;
        private UInt32 RootSector;
        private UInt32 RootSectorCount;
        private UInt32 DataSector;
        private UInt32 EntriesPerSector;
        private UInt32 fatEntries;
        private string VolumeLabel;
        private UInt32 FatCurrentDirectoryEntry;


        protected List<Partition> mPartitions;

        internal List<Partition> PartInfo
        { get { return mPartitions; } }

        [DllImport("*")]
        private static extern void fatfs_init();
        IDEDevice IDevice;

        bool IsValid = false;

        public FAT32()
        {
            mPartitions = new List<Partition>();
            IDevice = IDE.Ports[0];

            IsValid = IsFAT();

            FlushDetails();
        }

        public void FlushDetails()
        {
            if (IsValid)
            {
                Console.WriteLine("====================================================");
                Console.WriteLine("FAT Version:" + ((FatType == FatType.FAT32) ? "FAT32" : "FAT16/12"));
                Console.WriteLine("Disk Volume:" + (VolumeLabel == "NO NAME" ? VolumeLabel + "<Extended>" : VolumeLabel));
                Console.WriteLine("Bytes Per Sector:" + BytePerSector.ToString());
                Console.WriteLine("Sectors Per Cluster:" + SectorsPerCluster.ToString());
                Console.WriteLine("Reserved Sector:" + ReservedSector.ToString());
                Console.WriteLine("Total FAT:" + TotalFAT.ToString());
                Console.WriteLine("Direactory Entry:" + DirectoryEntry.ToString());
                Console.WriteLine("Total Sectors:" + TotalSectors.ToString());
                Console.WriteLine("Sectors Per FAT:" + SectorsPerFAT.ToString());
                Console.WriteLine("Data Sector Count:" + DataSectorCount.ToString());
                Console.WriteLine("Cluster Count:" + ClusterCount.ToString());
                Console.WriteLine("Serial Number:" + SerialNo.ToString());
                Console.WriteLine("Root Cluster:" + RootCluster.ToString());
                Console.WriteLine("Root Sector:" + RootSector.ToString());
                Console.WriteLine("Root Sector Count:" + RootSectorCount.ToString());
                Console.WriteLine("Data Sector:" + DataSector.ToString());
                Console.WriteLine("====================================================");
            }
            else
                Console.WriteLine("No fat available");
        }

        private void ParseData(byte[] aMBR, Int32 aLoc)
        {
            byte xSystemID = aMBR[aLoc + 4];
            if (xSystemID == 0x5 || xSystemID == 0xF || xSystemID == 0x85)
            {
                // Extended Partition Detected
                // DOS only knows about 05, Windows 95 introduced 0F, Linux introduced 85
                // Search for logical volumes
                // http://thestarman.pcministry.com/asm/mbr/PartTables2.htm
                Console.Write("[MBR]: EBR Partition Found!\n");
            }
            else if (xSystemID != 0)
            {
                UInt32 xSectorCount = BitConverter.ToUInt32(aMBR, aLoc + 12);
                UInt32 xStartSector = BitConverter.ToUInt32(aMBR, aLoc + 8);
                mPartitions.Add(new Partition(IDevice, xStartSector, xSectorCount));
                //Console.WriteLine($"Partition: {xStartSector} | {xSectorCount}");
            }
        }

        private unsafe bool IsFAT()
        {
            byte[] aMBR = new byte[512];

            fixed (byte* p = aMBR)
            {
                IDevice.Read(0U, 1U, p);

                for (int i = 0; i < SectorSize; i++)
                {
                    aMBR[i] = p[i];
                }
            }

            ParseData(aMBR, 446);
            ParseData(aMBR, 462);
            ParseData(aMBR, 478);
            ParseData(aMBR, 494);

            byte[] BootSector = new byte[512];

            PartInfo[0].Read(0U, 1U, BootSector);

            ushort xSig = BitConverter.ToUInt16(BootSector, 510);

            if (xSig != 0xAA55)
                return false;

            /* BPB (BIOS Parameter Block) */
            BytePerSector = BitConverter.ToUInt16(BootSector, 11);
            SectorsPerCluster = BitConverter.ToInt8(BootSector,13);
            ReservedSector = BitConverter.ToUInt16(BootSector, 14);
            TotalFAT = BitConverter.ToInt8(BootSector, 16);
            DirectoryEntry = BitConverter.ToUInt16(BootSector, 17);

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

            /* Not Necessary, To Avoid Crashes during corrupted BPB Info */
            // Just to prevent ourself from hacking
            if (TotalFAT == 0 || TotalFAT > 2 || BytePerSector == 0 || TotalSectors == 0 || SectorsPerCluster == 0)
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
                VolumeLabel = ASCII.GetString(BootSector, 71, 11);   // for checking
                RootCluster = BitConverter.ToUInt32(BootSector, 44);
                RootSector = 0;
                RootSectorCount = 0;
            }
            /* The key is of another door */
            else
            {
                SerialNo = BitConverter.ToUInt32(BootSector, 67);
                VolumeLabel = ASCII.GetString(BootSector, 43, 11);

                RootSector = ReservedSector + (TotalFAT * SectorsPerFAT);
                RootSectorCount = (UInt32)((DirectoryEntry * 32 + (BytePerSector - 1)) / BytePerSector);
                fatEntries = SectorsPerFAT * 512 / 4;
            }

            /* Now it shows our forward path ;) */
            EntriesPerSector = (UInt32)(BytePerSector / 32);
            DataSector = ReservedSector + (TotalFAT * SectorsPerFAT) + RootSectorCount;

            FatCurrentDirectoryEntry = RootCluster;
 
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
            /*
            byte[] xFileData = new byte[(UInt32)SectorsPerCluster * 512];

            var location = FindEntry(new FileSystem.Find.WithName(FileName), FatCurrentDirectoryEntry);
            if (location == null)
                return null;

            byte[] xReturnData = new byte[location.Size];
            UInt32 xSector = DataSector + ((location.FirstCluster - RootCluster) * SectorsPerCluster);
            this.IDevice.Read(xSector, SectorsPerCluster, xFileData);
            Array.Copy(xFileData, 0, xReturnData, 0, location.Size);
            return xReturnData;
            */

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
