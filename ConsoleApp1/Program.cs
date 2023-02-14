using Internal.Runtime.CompilerServices;
using System.Drawing;
using System;
using System.Runtime;
using System.Runtime.InteropServices;

namespace ConsoleApp1
{
    static unsafe class Program
    {
        public static Image ScreenBuf;

        [RuntimeExport("Main")]
        public static void Main()
        {
            var handle = NativeMethod.CreateWindow(0, 0, 256, 240, "APP Portable");
            var screenBufHandle = NativeMethod.GetWindowScreenBuf(handle);
            ScreenBuf = Unsafe.As<IntPtr, Image>(ref screenBufHandle);

            for (; ; )
            {
               
            }
        }
    }
}