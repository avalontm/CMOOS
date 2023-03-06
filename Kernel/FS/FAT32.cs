using MOOS.Misc;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static MOOS.Misc.Interrupts;

namespace MOOS.FS
{
    public unsafe class FAT32FS : FileSystem
    {
        struct bpbFat32
        {
            public fixed byte bs_jmpBoot[3];          // jmp instr to boot code
            public fixed byte bs_oemName[8];          // indicates what system formatted this field, default=MSWIN4.1
            public int bpb_bytesPerSec;       // Count of bytes per sector
            public byte bpb_secPerClus;         // no.of sectors per allocation unit
            public int bpb_rsvdSecCnt;        // no.of reserved sectors in the resercved region of the volume starting at 1st sector
            public byte bpb_numFATs;            // The count of FAT datastructures on the volume
            public int bpb_rootEntCnt;        // Count of 32-byte entries in root dir, for FAT32 set to 0
            public int bpb_totSec16;          // total sectors on the volume
            public byte bpb_media;              // value of fixed media
            public int bpb_FATSz16;           // count of sectors occupied by one FAT
            public int bpb_secPerTrk;         // sectors per track for interrupt 0x13, only for special devices
            public int bpb_numHeads;          // no.of heads for intettupr 0x13
            public int bpb_hiddSec;           // count of hidden sectors
            public int bpb_totSec32;          // count of sectors on volume
            public int bpb_FATSz32;           // define for FAT32 only
            public int bpb_extFlags;          // Reserved for FAT32
            public int bpb_FSVer;             // Major/Minor version num
            public int bpb_RootClus;          // Clus num of 1st clus of root dir
            public int bpb_FSInfo;            // sec num of FSINFO struct
            public int bpb_bkBootSec;         // copy of boot record
            public fixed byte bpb_reserved[12];       // reserved for future expansion
            public byte bs_drvNum;              // drive num
            public byte bs_reserved1;           // for ue by NT
            public byte bs_bootSig;             // extended boot signature
            public int bs_volID;              // volume serial number
            public fixed byte bs_volLab[11];          // volume label
            public fixed byte bs_fileSysTye[8];       // FAT12, FAT16 etc
        };

        struct dirEnt
        {
            public fixed byte dir_name[11];           // INT name
            public byte dir_attr;               // File sttribute
            public byte dir_NTRes;              // Set value to 0, never chnage this
            public byte dir_crtTimeTenth;       // millisecond timestamp for file creation time
            public uint dir_crtTime;           // time file was created
            public uint dir_crtDate;           // date file was created
            public uint dir_lstAccDate;        // last access date
            public uint dir_fstClusHI;         // high word fo this entry's first cluster number
            public uint dir_wrtTime;           // time of last write
            public uint dir_wrtDate;           // dat eof last write
            public uint dir_fstClusLO;         // low word of this entry's first cluster number
            public uint dir_fileSize;          // 32-bit DWORD hoding this file's size in bytes
        }

        static int MAX_OPEN = 129;

        bpbFat32 bpbcomm;       // global structure to store MBR
        dirEnt rootDir;         // globar structure to store rootDir entry
        dirEnt cwd;             // current working directory
        int inFile;             // file desriptor to point to raw disk
        string cwdPath;          // current working dir name
        int fdCount;            // no.of open file descriptor count
        dirEnt[] fdTable = new dirEnt[MAX_OPEN];       // array to store directory entries of open files

        int init = 0;           // global variable to indicate if FAT initialization was done or not, 1 indicates initialized
        int SEEK_SET = 0;

        public FAT32FS()
        {
            Console.WriteLine("[Initrd] Initializing FAT32");
            //get env for FAT dir
            string rawDisk = "";

            //open FAT disk file
            OS_open(rawDisk);

            //initialize cwd
            cwdPath = "/";
            fdCount = 0;

            fixed (bpbFat32* _bpbcomm = &bpbcomm)
            {
                //get MBR
                Native.Stosb(_bpbcomm, 0, sizeof(bpbFat32));

                //safe_read(inFile, (BYTE*)&bpbcomm, sizeof(bpbFat32), 0x00);

                //get root directory info
                fixed (void* _rootDir = &rootDir)
                {
                    Native.Stosb(_rootDir, 0, sizeof(dirEnt));
                }

                rootDir = initializeRootDir(_bpbcomm, inFile);
                Console.WriteLine($"[rootDir] {rootDir.dir_wrtTime}");
            }
            //initialize fdTable
            //fdTable[MAX_OPEN] = { {0} };

            init = 1;

        }

        // general read function to seek to a offset and read data into buffer
        void safe_read(int descriptor, byte* buffer, int size, long offset)
        {
            Native.lSeek(descriptor, offset, SEEK_SET);
            int remaining = size;
            int read_size;
            byte* pos = buffer;
            do
            {
                read_size = Native.Read(descriptor, pos, remaining);
                pos += read_size;
                remaining -= read_size;
            } while (remaining > 0);
        }

        // function to initialize root directory struture
        dirEnt initializeRootDir(bpbFat32* bpbcomm, int inFile)
        {
            dirEnt dirInfo;
            Native.Stosd(&dirInfo, 0, sizeof(dirEnt));
            int first_data_sec = getFirstDataSec(bpbcomm, bpbcomm->bpb_RootClus);
            safe_read(inFile, (byte*)&dirInfo, sizeof(dirEnt), first_data_sec * bpbcomm->bpb_bytesPerSec);
            return dirInfo;
        }

        // open a file
        int OS_open(string dirpath) 
        {
            return 0;
        }

        public override List<FileInfo> GetFiles(string Directory)
        {
            return null;  
        }

        public override void Delete(string Name)
        {
            
        }

        public override byte[] ReadAllBytes(string Name)
        {
            char[] path_tokens = new char[1000];
            int depth = 0;
            int cluster = 0;
            int fd = -1;
            bool found = false;
            dirEnt lookupDir = rootDir;
            dirEnt* dirs = null;
            dirEnt prevCwd;
            string prevPath;

            prevCwd = cwd;
            prevPath = cwdPath;
  
            string path = Name;
            cluster = bpbcomm.bpb_RootClus;

            Panic.Error($"{Name} is not found!");
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

        // function to get fist data sector given cluster number
        int getFirstDataSec(bpbFat32* bpbcomm, int N)
        {
            int root_sec = (int)(((bpbcomm->bpb_rootEntCnt * 32) + (bpbcomm->bpb_bytesPerSec - 1)) / bpbcomm->bpb_bytesPerSec);
            int first_data_sec = (int)(bpbcomm->bpb_rsvdSecCnt + (bpbcomm->bpb_numFATs * bpbcomm->bpb_FATSz32) + root_sec);
            int first_sec_of_cluster = ((N - 2) * bpbcomm->bpb_secPerClus) + first_data_sec;
            return first_sec_of_cluster;
        }
    }
}
