global using NES;
using Internal.Runtime.CompilerServices;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime;
using System.Runtime.InteropServices;

namespace NES
{
    public static unsafe class Program
    {
        [RuntimeExport("Main")]
        public static void Main()
        {
            new App().Run();
        }
    }
}