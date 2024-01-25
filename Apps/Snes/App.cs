using Internal.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime;
using System.Windows;
using Moos.Framework.Graphics;
using SNES.Emulator;

namespace SNES
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

        #endregion

        public static int screenWidth = 0;
        public static int screenHeight = 0;
        static SNESSystem snes;

        public App()
        {
            screenWidth = GDI.GetWidth();
            screenHeight = GDI.GetHeight();

            snes = new SNESSystem();
            snes.LoadROM("roms/super_mario_world.smc");
           
            MoosNative.SetBindOnKeyChangedHandler(onSnesPad);

            while (GetProcess(processID) != IntPtr.Zero)
            {
                if (!MoosNative.GetPanic())
                {
                    snes.onRender();
                    //GDI.FillRectangle(0, 0, screenWidth, screenHeight, 0xFF55AAAA);
                    GDI.DrawImage(250, 100, snes.Render, false);
                    GDI.DrawUpdate();
                }
            }
        }

        void onSnesPad(object sender, ConsoleKeyInfo e)
        {
            if (e.KeyState == ConsoleKeyState.Pressed)
            {
                switch(e.Key)
                {
                    case ConsoleKey.Enter:
                        snes.SetKeyDown(SNESButton.Start);
                        break;
                    case ConsoleKey.A:
                        snes.SetKeyDown(SNESButton.A);
                        break;
                }
            }
            else if (e.KeyState == ConsoleKeyState.Released)
            {
                switch (e.Key)
                {
                    case ConsoleKey.Enter:
                        snes.SetKeyUp(SNESButton.Start);
                        break;
                    case ConsoleKey.A:
                        snes.SetKeyUp(SNESButton.A);
                        break;
                }
            }

        }
    }
}
