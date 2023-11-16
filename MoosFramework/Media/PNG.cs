using Internal.Runtime.CompilerServices;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;


namespace System.Media
{
    public unsafe class PNG 
    {
        public static Image FromFile(string file)
        {
            IntPtr handler = MoosNative.LoadPNG(file);
            return Unsafe.As<IntPtr, Image>(ref handler);
        }
    }
}
