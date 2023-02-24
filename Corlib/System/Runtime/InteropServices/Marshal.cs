namespace System.Runtime.InteropServices
{
    public static partial class Marshal
    {
        [RuntimeExport("FreeCoTaskMem")]
        public static void FreeCoTaskMem(IntPtr ptr)
        { 
        }
    }
}
