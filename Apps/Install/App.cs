using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime;
using Terminal.Managers;
using System.Runtime.InteropServices;
using System.Media;
using Moos.Framework.Graphics;
using System.Windows.Media;
using Moos.Framework.Fonts;
using Install.Screens;


namespace Install
{
    public unsafe class App : Application
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

        public static int screenWidth = 0;
        public static int screenHeight = 0;
        public static Color Gray = null;
        public static Color Black = null;
        public static Color Green = null;

        public static int IndexScreen = 0;

        public App()
        {
            FontManager.Load("sys/fonts/Song.btf", 18);

            screenWidth = GDI.GetWidth();
            screenHeight = GDI.GetHeight();

            Gray = new Color(170, 170, 170);
            Black = new Color(0, 0, 0);
            Green = new Color(183, 235, 52);

            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
           
            onLoop();
        }

        void onLoop()
        {
            while (GetProcess(processID) != IntPtr.Zero)
            {
                switch (IndexScreen)
                {
                    case 0:
                        Screen1.Draw();
                        break;
                    case 1:
                        Screen2.Draw();
                        break;
                    case 2:
                        Screen3.Draw();
                        break;
                    default:
                        KillProcess(processID);
                    break;
                }
            }
        }
    }
}
