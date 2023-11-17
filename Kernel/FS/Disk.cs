namespace MOOS.FS
{
    public abstract unsafe class Disk
    {
        public static Disk Instance;

        public Disk()
        {
            Instance = this;
        }

        public abstract bool Read(ulong sector, uint count, byte* data);
        public abstract bool Write(ulong sector, uint count, byte* data);
    }

    public abstract unsafe class DiskVirtual
    {
        public static DiskVirtual Instance;

        public DiskVirtual()
        {
            Instance = this;
        }

        public abstract bool Read(ulong sector, uint count, byte* data);
        public abstract bool Write(ulong sector, uint count, byte* data);
    }
}