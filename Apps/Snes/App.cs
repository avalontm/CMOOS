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
using Moos.Framework.System;
using Moos.Framework.Fonts;

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
        static FPSMeter pfs = null;
        public static bool StartRender;

        public App()
        {
            screenWidth = GDI.GetWidth();
            screenHeight = GDI.GetHeight();

            FontManager.Load("sys/fonts/Song.btf", 18);
            pfs = new FPSMeter();

            snes = new SNESSystem();
            snes.LoadROM("roms/super_mario_world.smc");

            MoosNative.SetBindOnKeyChangedHandler(onSnesPad);

            onLoop();
        }

        void onLoop()
        {
            while (GetProcess(processID) != IntPtr.Zero)
            {
                GDI.DrawClear();
                pfs.Update();
                snes.onRender();
               // GDI.FillRectangle(0, 0, screenWidth, screenHeight, 0xFF55AAAA);
                GDI.DrawImage(snes.screenX, snes.screenY, snes.RenderBuff, false);
                string cpu = $"FPS:{pfs.FPS}";
                FontManager.font.DrawString(2, 2, cpu, 0xFFFFFFFF);
                cpu.Dispose();

                GDI.DrawUpdate();
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
                    case ConsoleKey.RShiftKey:
                        snes.SetKeyDown(SNESButton.Sel);
                        break;
                    case ConsoleKey.A:
                        snes.SetKeyDown(SNESButton.A);
                        break;
                    case ConsoleKey.Z:
                        snes.SetKeyDown(SNESButton.B);
                        break;
                    case ConsoleKey.X:
                        snes.SetKeyDown(SNESButton.X);
                        break;
                    case ConsoleKey.S:
                        snes.SetKeyDown(SNESButton.Y);
                        break;
                    case ConsoleKey.Up:
                        snes.SetKeyDown(SNESButton.Up);
                        break;
                    case ConsoleKey.Down:
                        snes.SetKeyDown(SNESButton.Down);
                        break;
                    case ConsoleKey.Left:
                        snes.SetKeyDown(SNESButton.Left);
                        break;
                    case ConsoleKey.Right:
                        snes.SetKeyDown(SNESButton.Right);
                        break;
                    case ConsoleKey.Q:
                        snes.SetKeyDown(SNESButton.L);
                        break;
                    case ConsoleKey.W:
                        snes.SetKeyDown(SNESButton.R);
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
                    case ConsoleKey.RShiftKey:
                        snes.SetKeyUp(SNESButton.Sel);
                        break;
                    case ConsoleKey.A:
                        snes.SetKeyUp(SNESButton.A);
                        break;
                    case ConsoleKey.Z:
                        snes.SetKeyUp(SNESButton.B);
                        break;
                    case ConsoleKey.X:
                        snes.SetKeyUp(SNESButton.X);
                        break;
                    case ConsoleKey.S:
                        snes.SetKeyUp(SNESButton.Y);
                        break;
                    case ConsoleKey.Up:
                        snes.SetKeyUp(SNESButton.Up);
                        break;
                    case ConsoleKey.Down:
                        snes.SetKeyUp(SNESButton.Down);
                        break;
                    case ConsoleKey.Left:
                        snes.SetKeyUp(SNESButton.Left);
                        break;
                    case ConsoleKey.Right:
                        snes.SetKeyUp(SNESButton.Right);
                        break;
                    case ConsoleKey.Q:
                        snes.SetKeyUp(SNESButton.L);
                        break;
                    case ConsoleKey.W:
                        snes.SetKeyUp(SNESButton.R);
                        break;
                }
            }

        }
    }
}
