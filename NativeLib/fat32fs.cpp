#include <string.h> /*for size_t*/
#include "..\LibC\printf.h"
#include "integer.h"

typedef struct {

    BYTE bs_jmpBoot[3];          // jmp instr to boot code
    BYTE bs_oemName[8];          // indicates what system formatted this field, default=MSWIN4.1
    INT bpb_bytesPerSec;       // Count of bytes per sector
    BYTE bpb_secPerClus;         // no.of sectors per allocation unit
    INT bpb_rsvdSecCnt;        // no.of reserved sectors in the resercved region of the volume starting at 1st sector
    BYTE bpb_numFATs;            // The count of FAT datastructures on the volume
    INT bpb_rootEntCnt;        // Count of 32-byte entries in root dir, for FAT32 set to 0
    INT bpb_totSec16;          // total sectors on the volume
    BYTE bpb_media;              // value of fixed media
    INT bpb_FATSz16;           // count of sectors occupied by one FAT
    INT bpb_secPerTrk;         // sectors per track for interrupt 0x13, only for special devices
    INT bpb_numHeads;          // no.of heads for intettupr 0x13
    LONG bpb_hiddSec;           // count of hidden sectors
    LONG bpb_totSec32;          // count of sectors on volume
    LONG bpb_FATSz32;           // define for FAT32 only
    INT bpb_extFlags;          // Reserved for FAT32
    INT bpb_FSVer;             // Major/Minor version num
    LONG bpb_RootClus;          // Clus num of 1st clus of root dir
    INT bpb_FSInfo;            // sec num of FSINFO struct
    INT bpb_bkBootSec;         // copy of boot record
    BYTE bpb_reserved[12];       // reserved for future expansion
    BYTE bs_drvNum;              // drive num
    BYTE bs_reserved1;           // for ue by NT
    BYTE bs_bootSig;             // extended boot signature
    LONG bs_volID;              // volume serial number
    BYTE bs_volLab[11];          // volume label
    BYTE bs_fileSysTye[8];       // FAT12, FAT16 etc
} bpbFat32;

typedef struct  {
    BYTE dir_name[11];           // INT name
    BYTE dir_attr;               // File sttribute
    BYTE dir_NTRes;              // Set value to 0, never chnage this
    BYTE dir_crtTimeTenth;       // millisecond timestamp for file creation time
    INT dir_crtTime;           // time file was created
    INT dir_crtDate;           // date file was created
    INT dir_lstAccDate;        // last access date
    INT dir_fstClusHI;         // high word fo this entry's first cluster number
    INT dir_wrtTime;           // time of last write
    INT dir_wrtDate;           // dat eof last write
    INT dir_fstClusLO;         // low word of this entry's first cluster number
    LONG dir_fileSize;          // 32-bit DWORD hoding this file's size in bytes
} dirEnt;


//Helper functions to print
int printFat32(bpbFat32* bpbfat32);
int printDirEnt(dirEnt* dirInfo);

//Helper functions
void safe_read(int descriptor, BYTE* buffer, size_t size, long long offset);
char* tokenize(char* string, char* path[], int* depth);

//FAT related helper functions
int initFAT32();
int initializeMBR(bpbFat32* bpbcomm, int inFile);
dirEnt initializeRootDir(bpbFat32* bpbcomm, int inFile);
int getFirstDataSec(bpbFat32* bpbfat32, int N);
int getFileDesc(dirEnt* fileEnt);


//Library Functions to support FAT read-only
int OS_cd(const char* path);
int OS_open(const char* path);
int OS_close(int fd);
int OS_read(int fildes, void* buf, int nbyte, int offset);
dirEnt* OS_readDir(const char* dirname);

//Library Functions to support FAT read-write
int OS_mkdir(const char* path);
int OS_rmdir(const char* path);
int OS_creat(const char* path);
int OS_write(int fildes, const void* buf, int nbytes);
int OS_writeDir(const char* path, dirEnt* entries, int entryCount);


#define SUCCESS 1       // Flag to indicate success
#define FAILURE -1      // Flag to indicate failure

#define READDIR 1       // Flag to indicate readDir operation type
#define CD 2            // Flag to indicate cd operation type
#define OPENFILE 3      // Flag to indicate file open operation type
#define READFILE 4      // Flag to indicate file read operation type
#define MAX_OPEN 129

bpbFat32 bpbcomm;       // global structure to store MBR
dirEnt rootDir;         // globar structure to store rootDir entry
dirEnt cwd;             // current working directory
int inFile;             // file desriptor to point to raw disk
char* cwdPath;          // current working dir name
int fdCount;            // no.of open file descriptor count
dirEnt fdTable[MAX_OPEN];       // array to store directory entries of open files

int init = 0;           // global variable to indicate if FAT initialization was done or not, 1 indicates initialized


// function to tokenize path based on / as delimiter
char* tokenize(char* string, char* path[], int* depth) {
    char* token;

    char* fullpath;
    char* relpath;
    int relative = 0;

    //If relative path
    if (string[0] != '/') {
        //add cwd to begining of path
        int newlen = strlen(string) + strlen(cwdPath) + 1;
        fullpath = (char*)malloc(sizeof(char) * newlen);
        relpath = (char*)malloc(sizeof(char) * newlen);
        relpath[0] = '\0';
        fullpath[0] = '\0';
        strcat(fullpath, cwdPath);
        strcat(fullpath, "/");
        strcat(fullpath, string);
        fullpath[newlen] = '\0';
        relative = 1;
    }
    else {        // not replative path

        int newlen = strlen(string) + 1;
        fullpath = (char*)malloc(sizeof(char) * newlen);
        relpath = (char*)malloc(sizeof(char) * newlen);
        relpath[0] = '\0';
        fullpath[0] = '\0';
        strcat(fullpath, string);
    }

    token = strtok(fullpath, "/");
    path[0] = token;
    int count = 1;
    while (token != NULL) {
        token = strtok(NULL, "/");
        if (token == NULL)
            break;
        if (strncmp(token, "..", 2) == 0) {
            count--;
            continue;
        }
        else if (strncmp(token, ".", 1) == 0) {
            path[count] = token;
            continue;
        }
        else {
            path[count++] = token;
        }
    }
    (*depth) = count;
    path[count] = NULL;

    if (relative == 1) {
        strcat(relpath, "/");
        for (int i = 0; i < count; i++) {
            strcat(relpath, path[i]);
            strcat(relpath, "/");
        }
    }
    else {
        relpath = string;
    }
    return relpath;
}

// general read function to seek to a offset and read data into buffer
void safe_read(int descriptor, BYTE* buffer, BYTE size, long long offset) {
    lseek(descriptor, offset, SEEK_SET);
    int remaining = size;
    int read_size;
    BYTE* pos = buffer;
    do {
        read_size = read(descriptor, pos, remaining);
        pos += read_size;
        remaining -= read_size;
    } while (remaining > 0);
}


// function to initialize root directory struture
dirEnt initializeRootDir(bpbFat32* bpbcomm, int inFile) {
    dirEnt dirInfo;
    memset(&dirInfo, 0, sizeof(dirEnt));
    int first_data_sec = getFirstDataSec(bpbcomm, bpbcomm->bpb_RootClus);
    safe_read(inFile, (BYTE*)&dirInfo, sizeof(dirEnt), first_data_sec * bpbcomm->bpb_bytesPerSec);
    return dirInfo;
}

// function to initilaize FAT, read MBR and Root Directory entries
int initFAT32() {

    //get env for FAT dir
    char* rawDisk = "";

    if (rawDisk == NULL) {
        printf("Please export FAT_FS_PATH env\n");
        init = 0;
        return 0;
    }

    //open FAT disk file
    open(rawDisk, "");

    //initialize cwd
    cwdPath = (char*)"/";
    fdCount = 0;

    //get MBR
    memset(&bpbcomm, 0, sizeof(bpbFat32));
    safe_read(inFile, (BYTE*)&bpbcomm, sizeof(bpbFat32), 0x00);

    //get root directory info
    memset(&rootDir, 0, sizeof(dirEnt));
    rootDir = initializeRootDir(&bpbcomm, inFile);

    //initialize fdTable
    //fdTable[MAX_OPEN] = { {0} };

    init = 1;

    return SUCCESS;

}

// function to get fist data sector given cluster number
int getFirstDataSec(bpbFat32* bpbcomm, int N) {
    int root_sec = ((bpbcomm->bpb_rootEntCnt * 32) + (bpbcomm->bpb_bytesPerSec - 1)) / bpbcomm->bpb_bytesPerSec;
    int first_data_sec = bpbcomm->bpb_rsvdSecCnt + (bpbcomm->bpb_numFATs * bpbcomm->bpb_FATSz32) + root_sec;
    int first_sec_of_cluster = ((N - 2) * bpbcomm->bpb_secPerClus) + first_data_sec;
    return first_sec_of_cluster;
}

// function to read directory entries of given path
dirEnt* getDirEs(dirEnt dirInfo, bpbFat32* bpbcomm, int inFile, int cluster, int* count) {

    dirEnt* dirs = NULL;
    int i = 0;
    int first_data_sec = getFirstDataSec(bpbcomm, cluster);
    int root_offset = first_data_sec * bpbcomm->bpb_bytesPerSec;
    int next = sizeof(dirEnt);
    while (dirInfo.dir_name[0] != 0) {
        next += sizeof(dirEnt);
        safe_read(inFile, (BYTE*)&dirInfo, sizeof(dirEnt), root_offset + next);
        if ((dirInfo.dir_fileSize != -1) && (dirInfo.dir_fstClusLO != 0)) {
            //printDirEnt(&dirInfo);
            dirs = (dirEnt*)realloc(dirs, sizeof * dirs * next);
            dirs[i] = dirInfo;
            i++;
        }
    }
    //*count = i-1;
    *count = i;
    return dirs;
}

// get file discrptor when file, does not handle if entry already exists
int getFileDesc(dirEnt* fileEnt) {

    //check if the file is already open
    for (int i = 0; i < MAX_OPEN; i++) {
        if (strcmp((char*)fdTable[i].dir_name, (char*)fileEnt->dir_name) == 0)
            return i;
    }
    for (int i = 0; i < MAX_OPEN; i++) {
        if (fdTable[i].dir_attr == 0 && fdCount != MAX_OPEN) {
            fdCount = fdCount + 1;
            fdTable[i] = *fileEnt;
            return i;
        }
    }
    //something went wrong
    return -1;
}



// change directory from current to new directory
int OS_cd(const char* dirpath) {

    char* path_tokens[1000];
    int depth = 0;
    int cluster = 0;
    char* path;
    BOOL found;
    dirEnt lookupDir = rootDir;
    dirEnt* dirs = NULL;
    dirEnt prevCwd;
    char* prevPath;

    if (init == 0) {
        int ret = initFAT();
        if (ret != SUCCESS) {
            printf("FAT initialization Failed.. exiting\n");
            return FAILURE;
        }
    }

    prevCwd = cwd;
    prevPath = cwdPath;


    path = (char*)malloc(strlen(dirpath + 1) * sizeof(char*));
    cluster = bpbcomm.bpb_RootClus;

    strcpy(path, dirpath);
    char* retpath = tokenize(path, path_tokens, &depth);
    if (strcmp(retpath, "/") == 0) {
        cwdPath = retpath;
        return SUCCESS;
    }

    for (int i = 0; i < depth; i++) {
        int count = 0;
        found = FALSE;

        dirs = getDirEs(lookupDir, &bpbcomm, inFile, cluster, &count);

        if (depth == 1)
            count = count + 1;

        for (int j = 0; j < count; j++) {
            if (strncmp(path_tokens[i], (char*)dirs[j].dir_name, strlen(path_tokens[i])) == 0) {

                lookupDir = dirs[j];
                cluster = dirs[j].dir_fstClusLO;
                if ((i == depth - 1) && dirs[j].dir_attr != 0x10) { //reached end, no directory found
                    free(dirs);
                    return FAILURE;
                }
                else {
                    cwd = lookupDir;
                    found = TRUE;
                    break;
                }
            }
            else { // directory name compare didnt match
                found = FALSE;
            } //end of else
        } //end of inner for
        free(dirs);
        dirs = NULL;
    } //end of outer for

    if (found) {
        cwdPath = (char*)retpath;
        return SUCCESS;
    }
    else {
        cwd = prevCwd;
        cwdPath = prevPath;
        printf("ERROR: Failed to change directory %s\n", dirpath);
        return FAILURE;
    }
}

// read directory entries for given path
dirEnt* OS_readDir(const char* dirpath) {

    if (init == 0) {
        int ret = initFAT();
        if (ret != SUCCESS) {
            printf("FAT initialization Failed.. exiting\n");
            return NULL;
        }
    }

    char* path_tokens[1000];
    int depth = 0;
    int cluster = 0;
    int fd = -1;
    char* path;
    BOOL found = FALSE;
    dirEnt lookupDir = rootDir;
    dirEnt* dirs = NULL;
    dirEnt prevCwd;
    char* prevPath;

    //save old paths
    prevCwd = cwd;
    prevPath = cwdPath;

    path = (char*)malloc(strlen(dirpath + 1) * sizeof(char*));
    cluster = bpbcomm.bpb_RootClus;

    strcpy(path, dirpath);
    char* retpath = tokenize(path, path_tokens, &depth);

    int k;
    if (strcmp(retpath, "/") == 0) {
        int count = 0;
        dirs = getDirEs(lookupDir, &bpbcomm, inFile, cluster, &count);

        //resize dirEnts
        for (k = 0; k < 64; k++) {
            if (dirs[k].dir_fstClusHI != 0) break;
        }
        if (k != 64) {
            dirs = (dirEnt*)realloc(dirs, k * sizeof(dirEnt));
        }
        return dirs;
    }

    for (int i = 0; i < depth; i++) {
        int count = 0;
        found = FALSE;

        dirs = getDirEs(lookupDir, &bpbcomm, inFile, cluster, &count);

        if (depth == 1)
            count = count + 1;

        for (int j = 0; j < count; j++) {
            if (strncmp(path_tokens[i], (char*)dirs[j].dir_name, strlen(path_tokens[i])) == 0) {

                lookupDir = dirs[j];
                cluster = dirs[j].dir_fstClusLO;

                if ((i == depth - 1) && dirs[j].dir_attr != 0x10) { //reached end, no directory found
                    free(dirs);
                    return NULL;
                }
                else {
                    found = TRUE;
                    dirs = getDirEs(lookupDir, &bpbcomm, inFile, cluster, &count);
                    break;
                }
            }
            else { // directory name compare didnt match
                found = FALSE;
            } //end of else
        } //end of inner for
    } //end of outer for

    //resize dirEnts
    for (k = 0; k < 64; k++) {
        if (dirs[k].dir_fstClusHI != 0) break;
    }
    if (k != 64) {
        dirs = (dirEnt*)realloc(dirs, k * sizeof(dirEnt));
    }

    if (found) {
        return dirs;
    }
    else {
        free(dirs);
        printf("ERROR: Failed to read directory %s\n", dirpath);
        return NULL;
    }

}

// open a file
int OS_open(const char* dirpath) {

    if (init == 0) {
        int ret = initFAT();
        if (ret != SUCCESS) {
            printf("FAT initialization Failed.. exiting\n");
            return FAILURE;
        }
    }

    char* path_tokens[1000];
    int depth = 0;
    int cluster = 0;
    int fd = -1;
    BOOL found = FALSE;
    dirEnt lookupDir = rootDir;
    dirEnt* dirs = NULL;
    dirEnt prevCwd;
    char* prevPath;

    prevCwd = cwd;
    prevPath = cwdPath;

    char* path = (char*)malloc(strlen(dirpath + 1) * sizeof(char*));
    cluster = bpbcomm.bpb_RootClus;

    strcpy(path, dirpath);
    char* retpath = tokenize(path, path_tokens, &depth);

    if (strcmp(retpath, "/") == 0) {
        return FAILURE;
    }


    for (int i = 0; i < depth; i++) {
        int count = 0;
        found = FALSE;

        dirs = getDirEs(lookupDir, &bpbcomm, inFile, cluster, &count);

        if (count == 0 && dirs != NULL)
            count = count + 1;

        for (int j = 0; j < count; j++) {
            if (strncmp(path_tokens[i], (char*)dirs[j].dir_name, strlen(path_tokens[i])) == 0) {

                lookupDir = dirs[j];
                cluster = dirs[j].dir_fstClusLO;

                if ((i == depth - 1) && dirs[j].dir_attr != 0x20) { //reached end, no file found
                    free(dirs);
                    dirs = NULL;
                    return FAILURE;
                }
                else if ((i == depth - 1) && dirs[j].dir_attr == 0x20) {
                    fd = getFileDesc(&lookupDir);
                    found = TRUE;
                    break;
                }
                else {
                    found = FALSE;
                    continue;
                }
            }
            else { // directory name compare didnt match
                found = FALSE;
            } //end of else
        } //end of inner for
        free(dirs);
        dirs = NULL;
    } //end of outer for

    if (found) {
        return fd;
    }
    else {
        return FAILURE;
    }
}

// close a file
int OS_close(int fd) {

    if (init == 0) {
        int ret = initFAT();
        if (ret != SUCCESS) {
            printf("FAT initialization Failed.. exiting\n");
            return FAILURE;
        }
    }

    if (fd > -1 && fd <= MAX_OPEN) {
        if (fdTable[fd].dir_attr != 0) {
            //fdTable[fd] = { { 0 } };
            fdCount = fdCount - 1;
            return SUCCESS;
        }
        else {
            printf("ERROR: File not open\n");
            return FAILURE;
        }
    }
    else {
        printf("ERROR: Invalid file descriptor\n");
        return FAILURE;
    }

}


/*      Reading contents of the file
*       look up dirEnt in fdTable
*       get cluster number if dirEnt exists
*       seek to right offset
*       read data
*       return bytes read
*/
int OS_read(int fd, void* readbuf, int nbytes, int offset) {

    if (init == 0) {
        int ret = initFAT();
        if (ret != SUCCESS) {
            printf("FAT initialization Failed.. exiting\n");
            return FAILURE;
        }
    }


    if (fd > -1 && fd <= MAX_OPEN) {
        dirEnt fileInfo = fdTable[fd];
        if (fileInfo.dir_attr != 0x00) {
            int first_sec_of_cluster = getFirstDataSec(&bpbcomm, fileInfo.dir_fstClusLO);

           // BYTE buf[nbytes];
           // safe_read(inFile, (BYTE*)&buf, nbytes, first_sec_of_cluster * bpbcomm.bpb_bytesPerSec + offset);
           // strcpy((char*)readbuf, (char*)buf);
            return nbytes;
        }
        else {
            printf("ERROR: File not open\n");
            return FAILURE;
        }
    }
    else {
        printf("ERROR: Invalid file descriptor\n");
        return FAILURE;
    }

}