using MOOS.Driver;
using MOOS.IO;
using MOOS.Memory;
using System;
using System.Collections.Generic;
using System.Common.Extentions;
using System.Diagnostics;
using System.Drawing;
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

        public bool Write(uint aSectorNo, uint aSectorCount, byte[] aData)
        {
            fixed (byte* p = aData)
            {
                if (aSectorNo + aSectorCount > mSectorCount)
                    return false;

                return mParent.Write(mStartSector + aSectorNo, aSectorCount, p);

            }
        }
    }

    public unsafe class FAT32 : FileSystem
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
        private UInt32 RootCluster;
        private UInt32 RootSector;
        private UInt32 RootSectorCount;
        private UInt32 DataSector;
        private UInt32 EntriesPerSector;
        private UInt32 fatEntries;

        private UInt32 FatCurrentDirectoryEntry;

        protected List<Partition> mPartitions;

        internal List<Partition> PartInfo
        { get { return mPartitions; } }

        IDEDevice IDevice;

        bool IsValid = false;

        public FAT32()
        {
            mPartitions = new List<Partition>();
            IDevice = IDE.Ports[0];

            IsValid = IsFAT();
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
                SerialNo = BitConverter.ToUInt32(BootSector, 67);
                VolumeLabel = ASCII.GetString(BootSector, 71, 11);   // for checking
                RootCluster = BitConverter.ToUInt32(BootSector, 44);
                RootSector = 0;
                RootSectorCount = 0;
            }
            /* The key is of another door */
            else
            {
                SerialNo = BitConverter.ToUInt32(BootSector, 39);
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

        public override List<FileInfo> GetFiles(string DirName)
        {
            ChangeDirectory(DirName);

            var xResult = new List<FileInfo>();

            byte[] aData = new byte[(UInt32)(512 * SectorsPerCluster)];

            UInt32 xSector = DataSector + ((FatCurrentDirectoryEntry - RootCluster) * SectorsPerCluster);
            PartInfo[0].Read(xSector, SectorsPerCluster, aData);

            #region ReadingCode
            bool Entry_Type; //True -> Directory & False -> File

            FileInfo Entry_Detail;
            ulong sec = 0;

            for (uint i = 0; i < SectorsPerCluster * 512; i += 32)
            {
                if (aData[i] == 0x0)
                    break;
                else
                {
                    //Find Entry Type
                    switch ((FileAttribute)aData[i + 11])
                    {
                        case FileAttribute.Directory:
                            Entry_Type = true;
                            break;
                        case FileAttribute.Archive:
                            Entry_Type = false;
                            break;
                        default:
                            continue;
                    }

                    if (aData[i] != 0xE5) //Entry Exist
                    {
                        Entry_Detail = new FileInfo()
                        {
                            Name = ASCII.GetString(aData, (int)i, 8).Trim(),
                            Param0 = sec,
                            Size = BitConverter.ToUInt32(aData, (int)(i + Entry.FileSize))
                        };

                        if (!Entry_Type)
                        {
                            Entry_Detail.Attribute = FileAttribute.Archive;
                            Entry_Detail.Ext = ASCII.GetString(aData, (int)(i + 8), 3).Trim();
                        }
                        else
                        {
                            Entry_Detail.Attribute = FileAttribute.Directory;
                            Entry_Detail.Ext = "";
                        }

                        xResult.Add(Entry_Detail);
                        sec += SizeToSec(Entry_Detail.Size);
                    }
                }
            }
            #endregion

            return xResult;
        }

        public override void Delete(string Name)
        {
           
        }

        public override void WriteAllBytes(string FileName, byte[] Content)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                Console.WriteLine($"File Name empty.");
                return;
            }

            FileName = FileName.ToUpper();

            CreateFile(FileName, Content.Length);

            var location = FindEntry(new WithName(FileName), FatCurrentDirectoryEntry);
            if (location == null)
            {
                Console.WriteLine("File not found!");
                return;
            }

            UInt32 xSector = DataSector + ((location.FirstCluster - RootCluster) * SectorsPerCluster);

            PartInfo[0].Write(xSector, SectorsPerCluster, Content);
        }

        public override void Format()
        {

        }

        public override byte[] ReadAllBytes(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            string dirName = null;
            fileName = fileName.ToUpper();

            if (fileName.IndexOf('/') == -1)
            {
                dirName = "";
            }
            else
            {
                dirName = $"{fileName.Substring(0, fileName.LastIndexOf('/'))}";
            }

            ChangeDirectory(dirName);

            if (dirName.Length > 0)
            {
                fileName = fileName.Substring(dirName.Length +1);
            }

            byte[] xFileData = new byte[(UInt32)SectorsPerCluster * 512];

            var location = FindEntry(new WithName(fileName), FatCurrentDirectoryEntry);
            if (location == null)
                return null;

            UInt32 xSector = DataSector + ((location.FirstCluster - RootCluster) * SectorsPerCluster);

            PartInfo[0].Read(xSector, SectorsPerCluster, xFileData);

            Console.WriteLine($"xFileData: {xFileData.Length}");
            return xFileData;
        }

        protected bool IsClusterFree(uint cluster)
        {
            return ((cluster & ClusterMark.fatMask) == 0x00);
        }


        unsafe uint GetClusterEntryValue(uint cluster)
        {
            uint fatoffset = 0;

            fatoffset = cluster * 4;

            uint sector = ReservedSector + (fatoffset / BytePerSector);
            uint sectorOffset = fatoffset % BytePerSector;
            uint nbrSectors = 1;

            if ((FatType == FatType.FAT12) && (sectorOffset == BytePerSector - 1))
                nbrSectors = 2;

            var xdata = new byte[512 * nbrSectors];

            PartInfo[0].Read(sector, nbrSectors, xdata);

            BinaryFormat fat = new BinaryFormat(xdata);

            uint clusterValue;

            clusterValue = fat.GetUInt(sectorOffset) & 0x0FFFFFFF;

            return clusterValue;
        }

        uint lastFreeHint = 0;

        uint AllocateCluster()
        {
            uint at = lastFreeHint + 1;

            if (at < 2)
                at = 2;

            uint last = at - 1;

            if (last == 1)
                last = fatEntries;

            while (at != last)
            {
                uint value = GetClusterEntryValue(at);

                if (IsClusterFree(value))
                {
                    SetClusterEntryValue(at, 0xFFFFFFFF /*endOfClusterMark*/);
                    lastFreeHint = at;
                    return at;
                }

                at++;

                //if (at >= fatEntries)
                //at = 2;
            }

            Console.WriteLine("No Free Cluster Found!");
            return 0;	// mean no free space
        }

        unsafe bool SetClusterEntryValue(uint cluster, uint nextcluster)
        {
            uint fatOffset = 0;

            fatOffset = cluster * 4;

            uint sector = ReservedSector + (fatOffset / BytePerSector);
            uint sectorOffset = fatOffset % BytePerSector;
            uint nbrSectors = 1;

            if ((FatType == FatType.FAT12) && (sectorOffset == BytePerSector - 1))
                nbrSectors = 2;

            var xData = new byte[512 * nbrSectors];

            fixed (byte* p = xData)
            {
                this.IDevice.Read(sector, nbrSectors, p);

                for (int i = 0; i < xData.Length; i++)
                {
                    xData[i] = p[i];
                }
            }
            BinaryFormat fat = new BinaryFormat(xData);


            fat.SetUInt(sectorOffset, nextcluster);

            fixed (byte* p = fat.Data)
            {
                this.IDevice.Write(sector, nbrSectors,p);
            }

            return true;
        }

        public uint AllocateFirstCluster()
        {
            uint newCluster = AllocateCluster();

            if (newCluster == 0)
                return 0;

            return newCluster;
        }

        public void CreateFile(string FileName, uint size)
        {
            string[] str = FileName.Split('.');

            //TODO: Same Entry Exist exception.
            FatFileLocation location = FindEntry(new WithName(FileName), FatCurrentDirectoryEntry);

            if(location == null)
            {
                location = FindEntry(new Empty(), FatCurrentDirectoryEntry);
            }

            uint FirstCluster = AllocateFirstCluster();

            var xdata = new byte[512 * SectorsPerCluster];

            PartInfo[0].Read(location.DirectorySector, SectorsPerCluster, xdata);

            BinaryFormat directory = new BinaryFormat(xdata);
            directory.SetString(Entry.DOSName + location.DirectorySectorIndex * 32, "            ", 11);
            directory.SetString(Entry.DOSName + location.DirectorySectorIndex * 32, str[0]);

            if (str.Length > 1)
            {
                directory.SetString(Entry.DOSExtension + location.DirectorySectorIndex * 32, str[1]);
            }

            directory.SetByte(Entry.FileAttributes + location.DirectorySectorIndex * 32, (byte)0x20);
            directory.SetByte(Entry.Reserved + location.DirectorySectorIndex * 32, 0);
            directory.SetByte(Entry.CreationTimeFine + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.CreationTime + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.CreationDate + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.LastAccessDate + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.LastModifiedTime + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.LastModifiedDate + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.FirstCluster + location.DirectorySectorIndex * 32, (ushort)FirstCluster);
            directory.SetUInt(Entry.FileSize + location.DirectorySectorIndex * 32, size);
            directory.SetUInt(Entry.EntrySize + location.DirectorySectorIndex * 32, size);

            PartInfo[0].Write(location.DirectorySector, SectorsPerCluster, xdata);
        }

        public override void CreateDirectory(string DirName)
        {
            if(ChangeDirectory(DirName))
            {
                Console.WriteLine("Directory already exists.");
                return;
            }

            //TODO: Same Entry Exist exception.
            FatFileLocation location = FindEntry(new Empty(), FatCurrentDirectoryEntry);

            uint FirstCluster = AllocateFirstCluster();

            var xdata = new byte[512 * SectorsPerCluster];

            PartInfo[0].Read(location.DirectorySector, SectorsPerCluster, xdata);

            BinaryFormat directory = new BinaryFormat(xdata);
            directory.SetString(Entry.DOSName + location.DirectorySectorIndex * 32, "            ", 11);
            directory.SetString(Entry.DOSName + location.DirectorySectorIndex * 32, DirName);

            directory.SetByte(Entry.FileAttributes + location.DirectorySectorIndex * 32, (byte)0x10);
            directory.SetByte(Entry.Reserved + location.DirectorySectorIndex * 32, 0);
            directory.SetByte(Entry.CreationTimeFine + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.CreationTime + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.CreationDate + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.LastAccessDate + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.LastModifiedTime + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.LastModifiedDate + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.FirstCluster + location.DirectorySectorIndex * 32, (ushort)FirstCluster);
            directory.SetUInt(Entry.FileSize + location.DirectorySectorIndex * 32, 0);

            PartInfo[0].Write(location.DirectorySector, SectorsPerCluster, xdata);

            FatFileLocation loc = FindEntry(new Empty(), FirstCluster);
            xdata = new byte[512 * SectorsPerCluster];

            PartInfo[0].Read(loc.DirectorySector, SectorsPerCluster, xdata);

            directory = new BinaryFormat(xdata);
            for (int i = 0; i < 2; i++)
            {
                directory.SetString(Entry.DOSName + loc.DirectorySectorIndex * 32, "            ", 11);
                if (i == 0)
                {
                    directory.SetString(Entry.DOSName + loc.DirectorySectorIndex * 32, ".");
                    directory.SetUShort(Entry.FirstCluster + loc.DirectorySectorIndex * 32, (ushort)FirstCluster);
                }
                else
                {
                    directory.SetString(Entry.DOSName + loc.DirectorySectorIndex * 32, "..");
                    directory.SetUShort(Entry.FirstCluster + loc.DirectorySectorIndex * 32, (ushort)FatCurrentDirectoryEntry);
                }
                directory.SetByte(Entry.FileAttributes + loc.DirectorySectorIndex * 32, (byte)0x10);
                directory.SetByte(Entry.Reserved + loc.DirectorySectorIndex * 32, 0);
                directory.SetByte(Entry.CreationTimeFine + loc.DirectorySectorIndex * 32, 0);
                directory.SetUShort(Entry.CreationTime + loc.DirectorySectorIndex * 32, 0);
                directory.SetUShort(Entry.CreationDate + loc.DirectorySectorIndex * 32, 0);
                directory.SetUShort(Entry.LastAccessDate + loc.DirectorySectorIndex * 32, 0);
                directory.SetUShort(Entry.LastModifiedTime + loc.DirectorySectorIndex * 32, 0);
                directory.SetUShort(Entry.LastModifiedDate + loc.DirectorySectorIndex * 32, 0);
                directory.SetUInt(Entry.FileSize + loc.DirectorySectorIndex * 32, 0);
                loc.DirectorySectorIndex += 1;
            }

            PartInfo[0].Write(loc.DirectorySector, SectorsPerCluster, xdata);
        }

        public override bool ChangeDirectory(string DirName)
        {
            if (!string.IsNullOrEmpty(DirName))
            {
                DirName = DirName.ToUpper();
            }

            string[] dirs = DirName.Split('/');

            FatFileLocation location = null;

            for (int i = 0; i < dirs.Length; i++)
            {
                location = FindEntry(new WithName(dirs[i]), FatCurrentDirectoryEntry);
                Console.WriteLine($"dirChange: {dirs[0]}");
            }

            if (location != null)
            {
                FatCurrentDirectoryEntry = location.FirstCluster;
                return true;
            }

            return false;
        }

        public uint GetSectorByCluster(uint cluster)
        {
            return DataSector + ((cluster - RootCluster) * SectorsPerCluster);
        }

        static public uint GetClusterEntry(byte[] data, uint index, FatType type)
        {
            BinaryFormat entry = new BinaryFormat(data);
            uint cluster = entry.GetUShort(Entry.FirstCluster + (index * Entry.EntrySize));

            if (type == FatType.FAT32)
                cluster |= ((uint)entry.GetUShort(Entry.EAIndex + (index * Entry.EntrySize))) << 16;

            if (cluster == 0)
                cluster = 2;

            return cluster;
        }

        FatFileLocation FindEntry(ACompare compare, uint startCluster)
        {
            uint activeSector = ((startCluster - RootCluster) * SectorsPerCluster) + DataSector;

            if (startCluster == 0)
                activeSector = (FatType == FatType.FAT32) ? GetSectorByCluster(RootCluster) : RootSector;

            byte[] aData = new byte[512 * SectorsPerCluster];

            PartInfo[0].Read(activeSector, SectorsPerCluster, aData);

            BinaryFormat directory = new BinaryFormat(aData);

            for (uint index = 0; index < EntriesPerSector * SectorsPerCluster; index++)
            {
                if (compare.Compare(directory.Data, index * 32, FatType))
                {
                    FatFileAttributes attribute = (FatFileAttributes)directory.GetByte((index * Entry.EntrySize) + Entry.FileAttributes);
                    return new FatFileLocation(GetClusterEntry(directory.Data, index, FatType), activeSector, index, (attribute & FatFileAttributes.SubDirectory) != 0, directory.GetUInt((index * Entry.EntrySize) + Entry.FileSize));
                }

                if (directory.GetByte(Entry.DOSName + (index * Entry.EntrySize)) == FileNameAttribute.LastEntry)
                    return null;
            }
            return null;
        }
    }

    public class Empty : ACompare
    {
        protected uint cluster;

        public Empty()
        {
        }

        public override bool Compare(byte[] data, uint offset, FatType type)
        {
            BinaryFormat entry = new BinaryFormat(data);

            byte first = entry.GetByte(offset + Entry.DOSName);

            if (first == FileNameAttribute.LastEntry)
                return true;

            //if ((first == FileSystem.FAT.FatFileSystem.FileNameAttribute.Deleted) | (first == FileSystem.FAT.FatFileSystem.FileNameAttribute.Dot))
            //    return true;

            return false;
        }
    }

    internal struct ClusterMark
    {
        internal const UInt32 fatMask = 0x0FFFFFFF;
        internal const UInt32 badClusterMark = 0x0FFFFFF7;
        internal const UInt32 endOfClusterMark = 0x0FFFFFF8;
    }

    public class WithName : ACompare
    {
        protected string name;

        public WithName(string name)
        {
            this.name = name;
        }

        public override bool Compare(byte[] data, uint offset, FatType type)
        {
            BinaryFormat entry = new BinaryFormat(data);

            byte first = entry.GetByte(offset + Entry.DOSName);

            if (first == FileNameAttribute.LastEntry)
                return false;

            if ((first == FileNameAttribute.Deleted)) //| (first == FileSystem.FAT.FatFileSystem.FileNameAttribute.Dot)
                return false;

            if (first == FileNameAttribute.Escape)
                return false;

            string entryname = ASCII.GetString(data, (int)offset, 8).Trim();
            string entryExt = ASCII.GetString(data, (int)(offset + 8), 3).Trim();

     
            string[] xStr = name.Split('.');
 
            if (xStr.Length > 1)
            {
                //Console.WriteLine($"{entryname.ToLower()} == {xStr[0].Trim().ToLower()} && {entryExt.ToLower()} == {xStr[1].Trim().ToLower()}");
                if (entryname.ToLower() == xStr[0].Trim().ToLower() && entryExt.ToLower() == xStr[1].Trim().ToLower())
                {
                    return true;
                }
            }

            if (entryname.ToLower() == this.name.Trim().ToLower())
            {
                return true;
            }

            return false;
        }
    }

    internal struct FileNameAttribute
    {
        internal const uint LastEntry = 0x00;
        internal const uint Escape = 0x05;	// special msdos hack where 0x05 really means 0xE5 (since 0xE5 was already used for delete)
        internal const uint Dot = 0x2E;
        internal const uint Deleted = 0xE5;
    }

    internal struct Entry
    {
        internal const uint DOSName = 0x00; // 8
        internal const uint DOSExtension = 0x08;	// 3
        internal const uint FileAttributes = 0x0B;	// 1
        internal const uint Reserved = 0x0C;	// 1
        internal const uint CreationTimeFine = 0x0D; // 1
        internal const uint CreationTime = 0x0E; // 2
        internal const uint CreationDate = 0x10; // 2
        internal const uint LastAccessDate = 0x12; // 2
        internal const uint EAIndex = 0x14; // 2
        internal const uint LastModifiedTime = 0x16; // 2
        internal const uint LastModifiedDate = 0x18; // 2
        internal const uint FirstCluster = 0x1A; // 2
        internal const uint FileSize = 0x1C; // 4
        internal const uint EntrySize = 32;
    }
    
    public enum FatFileAttributes : byte
    {
        /// <summary>
        /// Flag represents the file is read-only.
        /// </summary>
        ReadOnly = 0x01,

        /// <summary>
        /// Flag represents the file is hidden.
        /// </summary>
        Hidden = 0x02,

        /// <summary>
        /// Flag represents the file is a system file.
        /// </summary>
        System = 0x04,

        /// <summary>
        /// Flag represents the file entry is a volume label.
        /// </summary>
        VolumeLabel = 0x08,

        /// <summary>
        /// Flag represents the file entry is a subdirectory.
        /// </summary>
        SubDirectory = 0x10,

        /// <summary>
        /// Flag represents the file has the archive bit set.
        /// </summary>
        Archive = 0x20,

        /// <summary>
        /// Flag represents the file entry is for a device.
        /// </summary>
        Device = 0x40,

        /// <summary>
        /// Flag is unused.
        /// </summary>
        Unused = 0x80,

        /// <summary>
        /// Flag represents the file has a long file name.
        /// </summary>
        LongFileName = 0x0F
    }

    public abstract class ACompare
    {
        public abstract bool Compare(byte[] data, uint offset, FatType type);
    }

    public class FatFileLocation
    {
        public uint FirstCluster;

        public uint DirectorySector;

        public uint DirectorySectorIndex;

        public bool directory;

        private bool IsDirectory { get { return directory; } }

        public uint Size;

        public FatFileLocation()
        {

        }

        public FatFileLocation(uint startCluster, uint directorySector, uint directoryIndex, bool directory, uint size)
        {
            this.FirstCluster = startCluster;
            this.DirectorySector = directorySector;
            this.DirectorySectorIndex = directoryIndex;
            this.directory = directory;
            this.Size = size;
        }
    }

    public class BinaryFormat
    {
        private byte[] data;

        public uint Length
        {
            get
            {
                return (uint)this.data.Length;
            }
        }

        public byte[] Data
        {
            get
            {
                return this.data;
            }
        }

        public byte this[int index]
        {
            get
            {
                return this.data[index];
            }
            set
            {
                this.data[index] = value;
            }
        }

        public BinaryFormat(byte[] data)
        {
            this.data = data;
        }

        public BinaryFormat(uint length)
        {
            this.data = new byte[(int)length];
        }

        public char GetChar(uint offset)
        {
            return (char)this.data[(int)offset];
        }

        public void SetChar(uint offset, char value)
        {
            this.data[(int)offset] = (byte)value;
        }

        public char[] GetChars(uint offset, uint length)
        {
            char[] chArray = new char[(int)length];
            for (uint index = 0U; index < length; ++index)
                chArray[(int)index] = this.GetChar(offset + index);
            return chArray;
        }

        public byte[] GetBytes(uint offset, uint length)
        {
            byte[] numArray = new byte[(int)length];
            for (uint index = 0U; index < length; ++index)
                numArray[(int)index] = this.data[(int)(offset + index)];
            return numArray;
        }

        public void SetChars(uint offset, char[] value)
        {
            for (uint index = 0U; (long)index < (long)value.Length; ++index)
                this.data[(int)(offset + index)] = (byte)value[(int)index];
        }

        public void SetString(uint offset, string value)
        {
            for (int index = 0; index < value.Length; ++index)
                this.data[(uint)offset + (uint)index] = (byte)value[index];
        }

        public void SetString(uint offset, string value, uint length)
        {
            for (int index = 0; (uint)index < (uint)length; ++index)
                this.data[(uint)offset + (uint)index] = (byte)value[index];
        }

        public uint GetUInt(uint offset)
        {
            return (uint)((int)this.data[(int)offset++] + ((int)this.data[(int)offset++] << 8) + ((int)this.data[(int)offset++] << 16) + ((int)this.data[(int)offset++] << 24));
        }

        public void SetUInt(uint offset, uint value)
        {
            this.data[(int)offset++] = (byte)(value & (uint)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 8 & (uint)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 16 & (uint)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 24 & (uint)byte.MaxValue);
        }

        public void SetUIntReversed(uint offset, uint value)
        {
            this.data[(int)offset++] = (byte)(value >> 24 & (uint)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 16 & (uint)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 8 & (uint)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value & (uint)byte.MaxValue);
        }

        public void SetULong(uint offset, ulong value)
        {
            this.data[(int)offset++] = (byte)(value & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 8 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 16 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 24 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 32 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 40 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 48 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 56 & (ulong)byte.MaxValue);
        }

        public void SetULongReversed(uint offset, ulong value)
        {
            this.data[(int)offset++] = (byte)(value >> 56 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 48 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 40 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 32 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 24 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 16 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 8 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value & (ulong)byte.MaxValue);
        }

        public ushort GetUShort(uint offset)
        {
            return (ushort)((int)this.data[(int)offset++] | (int)this.data[(int)offset++] << 8);
        }

        public void SetUShort(uint offset, ushort value)
        {
            this.data[(int)offset++] = (byte)((uint)value & (uint)byte.MaxValue);
            this.data[(int)offset++] = (byte)((int)value >> 8 & (int)byte.MaxValue);
        }

        public void SetUShortReversed(uint offset, ushort value)
        {
            this.data[(int)offset++] = (byte)((int)value >> 8 & (int)byte.MaxValue);
            this.data[(int)offset++] = (byte)((uint)value & (uint)byte.MaxValue);
        }

        public ulong GetULong(uint offset)
        {
            return (ulong)this.data[(int)offset++] + (ulong)((uint)this.data[(int)offset++] << 8) + (ulong)((uint)this.data[(int)offset++] << 16) + (ulong)((uint)this.data[(int)offset++] << 24) + (ulong)this.data[(int)offset++] + (ulong)((uint)this.data[(int)offset++] << 8) + (ulong)((uint)this.data[(int)offset++] << 16) + (ulong)((uint)this.data[(int)offset++] << 24);
        }

        public void SetULong(ulong offset, ulong value)
        {
            this.data[offset++] = (byte)(value & (ulong)byte.MaxValue);
            this.data[offset++] = (byte)(value >> 8 & (ulong)byte.MaxValue);
            this.data[offset++] = (byte)(value >> 16 & (ulong)byte.MaxValue);
            this.data[offset++] = (byte)(value >> 24 & (ulong)byte.MaxValue);
            this.data[offset++] = (byte)(value >> 32 & (ulong)byte.MaxValue);
            this.data[offset++] = (byte)(value >> 40 & (ulong)byte.MaxValue);
            this.data[offset++] = (byte)(value >> 48 & (ulong)byte.MaxValue);
            this.data[offset++] = (byte)(value >> 56 & (ulong)byte.MaxValue);
        }

        public byte GetByte(uint offset)
        {
            return this.data[(int)offset];
        }

        public void SetByte(uint offset, byte value)
        {
            this.data[(int)offset] = value;
        }

        public void SetBytes(uint offset, byte[] value)
        {
            for (uint index = 0U; (long)index < (long)value.Length; ++index)
                this.data[(int)(offset + index)] = value[(int)index];
        }

        public void SetBytes(uint offset, byte[] value, uint start, uint length)
        {
            for (uint index = 0U; index < length; ++index)
                this.data[(int)(offset + index)] = value[(int)(start + index)];
        }

        public void Fill(uint offset, byte value, uint length)
        {
            for (uint index = 0U; index < length; ++index)
                this.data[(int)(offset + index)] = value;
        }
    }
}
