﻿using Explorer.Managers;
using Moos.Framework.Fonts;
using Moos.Framework.Graphics;
using Moos.Framework.Input;
using Moos.Framework.System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.IO;
using System.Media;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace MoosExplorer
{
    public static unsafe class MoosExplorer
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


        static Image Wallpaper = null;
        static int screenWidth = 0;
        static int screenHeight = 0;
        static Button start = null;

        [RuntimeExport("Main")]
        public static void Main()
        {
            Console.WriteLine("Explorer");
            MoosNative.ModeGUI();

            FontManager.Load("sys/media/Yahei.png", 18);
            FPSMeter pfs = new FPSMeter();
            screenWidth = GDI.GetWidth();
            screenHeight = GDI.GetHeight();

            /*
            Wallpaper = PNG.FromFile("sys/media/Wallpaper2.png");
            Wallpaper = Wallpaper.ResizeImage(screenWidth, screenHeight);
            */

            CursorManager.Initialize();
   
            start = new Button();
            start.X = 5;
            start.Y = screenHeight - 42;
            start.Width = 75;
            start.Height = 38;
            start.Text = "start";
            start.Clicked = start_clicked;
            start.OnLoaded();
            
            for (; ; )
            {

                CursorManager.Update();
                start.OnUpdate();
                pfs.Update();

                if (Wallpaper == null)
                {
                    GDI.AFillRectangle(0, 0, screenWidth, screenHeight, 0xFF55AAAA);
                }
                else
                {
                    GDI.DrawImage(0, 0, Wallpaper, false);
                }

                DrawBottomBar();

                FontManager.font.DrawString(0, 0, string.Format("FPS: {0}", pfs.FPS), 0xFFFFFFFF); 

                CursorManager.Draw();

                GDI.DrawUpdate();

            }
        }

        static void DrawBottomBar()
        {
            GDI.AFillRectangle(0, screenHeight - 48, screenWidth, 48, 0xFFC3C7CB);
            GDI.DrawLine(0, screenHeight - 46, screenWidth, screenHeight - 46, 0xFFFFFFFF);

            start.OnDraw();
        }

        static void start_clicked(object sender, object e)
        {
            if (Wallpaper == null)
            {
                Wallpaper = PNG.FromFile("sys/media/Wallpaper2.png");
                Wallpaper = Wallpaper.ResizeImage(screenWidth, screenHeight);
            }
        }
    }
}