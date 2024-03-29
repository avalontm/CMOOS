﻿using System.Runtime.InteropServices;

namespace System.IO
{
    public static class Directory
    {
        [DllImport("CreateDirectory")]
        public static extern void __CreateDirectory(string file);

        public static void CreateDirectory(string file)
        {
            __CreateDirectory(file);
        }
    }
}
