using Internal.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Moos.Framework.Graphics;

namespace NES
{
    public unsafe class App : UIApplication
    {
        #region NativeMethods
        [RuntimeExport("malloc")]
        public static nint malloc(ulong size) => Allocate(size);

        [DllImport("Allocate")]
        public static extern nint Allocate(ulong size);

        [DllImport("ReadAllBytes")]
        public static extern void ReadAllBytes(string name, out ulong size, out byte* data);

        [DllImport("Lock")]
        public static extern void ALock();

        [DllImport("Unlock")]
        public static extern void AUnlock();

        [RuntimeExport("Lock")]
        public static void Lock() => ALock();

        [RuntimeExport("Unlock")]
        public static void Unlock() => AUnlock();

        [DllImport("DebugWrite")]
        public static extern void ADebugWrite(char c);

        [DllImport("DebugWriteLine")]
        public static extern void ADebugWriteLine();

        [RuntimeExport("DebugWrite")]
        public static void DebugWrite(char c) => ADebugWrite(c);

        [RuntimeExport("DebugWriteLine")]
        public static void DebugWriteLine() => ADebugWriteLine();

        [DllImport("ConsoleWrite")]
        public static extern void AConsoleWrite(char c);

        [DllImport("ConsoleWriteLine")]
        public static extern void AConsoleWriteLine();

        [RuntimeExport("ConsoleWrite")]
        public static void ConsoleWrite(char c) => AConsoleWrite(c);

        [RuntimeExport("ConsoleWriteLine")]
        public static void ConsoleWriteLine() => AConsoleWriteLine();

        [DllImport("Free")]
        public static extern ulong AFree(nint ptr);

        [RuntimeExport("free")]
        public static ulong free(nint ptr) => AFree(ptr);

        [RuntimeExport("__security_cookie")]
        public static void SecurityCookie()
        {
        }

        #endregion

        public static Image ScreenBuf;
        static int screenWidth = 0;
        static int screenHeight = 0;

        public App()
        {
            screenWidth = GDI.GetWidth();
            screenHeight = GDI.GetHeight();
            ScreenBuf = new Image(300, 300);

            byte[] data = File.ReadAllBytes("roms/super_mario.nes");

            if(data == null)
            {
                Console.WriteLine("Rom not found!.");
                return;
            }

            NES nes = new NES();
            nes.openROM(data);
            data.Dispose();
            Console.WriteLine("Loaded!");

            while (GetProcess(processID) != IntPtr.Zero)
            {
                GDI.FillRectangle(0, 0, screenWidth, screenHeight, 0xFF55AAAA);
                GDI.DrawImage(0, 0, ScreenBuf, false);
                GDI.DrawUpdate();
            }
        }

    }
}
