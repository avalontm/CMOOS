using Explorer.Controls;
using Explorer.Managers;
using Moos.Framework.Fonts;
using Moos.Framework.Graphics;
using Moos.Framework.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Explorer
{
    public unsafe partial class App : UIApplication
    {
        static Image Wallpaper = null;
        static int screenWidth = 0;
        static int screenHeight = 0;
        static Button start = null;
        static MenuBar menu = null;
        static Container clock = null;
        static Point LastPoint = new Point();
        static FPSMeter pfs = null;

        public App() 
        {
            FontManager.Load("sys/fonts/Song.btf", 18);
            pfs = new FPSMeter();
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
            start.Width = 90;
            start.Height = 38;
            start.Icon = PNG.FromFile("sys/media/Start.png").ResizeImage(24,24);
            start.Text = "start";
            start.Clicked = start_clicked;
            start.OnLoaded();

            menu = new MenuBar();
            menu.X = 2;
            menu.Y = (screenHeight - 48) - menu.Height;
            menu.OnLoaded();

            clock = new Container();
            clock.X = screenWidth - clock.Width - 5;
            clock.Y = screenHeight - clock.Height -5;
            clock.Text = GetHourAndMinute();
            clock.OnLoaded();


            MoosNative.SetBindOnKeyChangedHandler(onKeyBoardHandler);

            onStartup();

            onLoop();
        }

        private void onKeyBoardHandler(object sender, ConsoleKeyInfo e)
        {
            if (e.KeyState == ConsoleKeyState.Pressed)
            {
                if (e.Key == ConsoleKey.Z || e.Key == ConsoleKey.LeftWindows || e.Key == ConsoleKey.RightWindows)
                {
                    if (menu != null)
                    {
                        menu.IsVisible = !menu.IsVisible;
                    }
                }
            }
        }

        void onStartup()
        {
            Wav wav = new Wav("sys/sounds/startup.wav");
            wav.Play();
        }

        void onLoop()
        {
            while(GetProcess(processID) != IntPtr.Zero)
            {
                pfs.Update();

                CursorManager.Update();
                start.OnUpdate();
                menu.OnUpdate();
                clock.OnUpdate();

                DrawWallPaper();
                DrawSelection();
                DrawBottomBar();

                DrawMessagBox();

                string cpu = $"FPS:{pfs.FPS} | CPU Usage:{MoosNative.CPUUsage()}% | Used Memory: {(MoosNative.MemoryInUse() / 1024)}kbytes";
                FontManager.font.DrawString(2, 2, cpu, 0xFFFFFFFF);

                CursorManager.Draw();

                GDI.DrawUpdate();
            }
        }

        string GetHourAndMinute()
        {
            ulong time = MoosNative.GetTime();

            ulong hourMask = 0xFF000000; // Máscara para aislar la hora
            ulong minuteMask = 0x00FF0000; // Máscara para aislar los minutos

            //Obtiene la hora y los minutos utilizando las máscaras y desplazamientos
            int hour = (int)((time & hourMask) >> 24);
            int minute = (int)((time & minuteMask) >> 16);

            return $"{hour}:{(minute < 10 ? "0" : "")}{minute}";
        }

        void DrawMessagBox()
        {
            MessageBox.Instance.OnUpdate();
            MessageBox.Instance.OnDraw();
        }

        void DrawSelection()
        {
            if (Mouse.Clicked)
            {
                LastPoint = new Point(Mouse.Position.X, Mouse.Position.Y);
            }

            if (Mouse.Buttons == MouseButtons.Left)
            {

                if (Mouse.Position.X > LastPoint.X && Mouse.Position.Y > LastPoint.Y)
                {
                    GDI.AFillRectangle(
                        LastPoint.X,
                        LastPoint.Y,
                        Mouse.Position.X - LastPoint.X,
                        Mouse.Position.Y - LastPoint.Y,
                        0x7F2E86C1);
                }

                if (Mouse.Position.X < LastPoint.X && Mouse.Position.Y < LastPoint.Y)
                {
                    GDI.AFillRectangle(
                        Mouse.Position.X,
                        Mouse.Position.Y,
                        LastPoint.X - Mouse.Position.X,
                        LastPoint.Y - Mouse.Position.Y,
                        0x7F2E86C1);
                }

                if (Mouse.Position.X < LastPoint.X && Mouse.Position.Y > LastPoint.Y)
                {
                    GDI.AFillRectangle(
                        Mouse.Position.X,
                        LastPoint.Y,
                        LastPoint.X - Mouse.Position.X,
                        Mouse.Position.Y - LastPoint.Y,
                        0x7F2E86C1);
                }

                if (Mouse.Position.X > LastPoint.X && Mouse.Position.Y < LastPoint.Y)
                {
                    GDI.AFillRectangle(
                        LastPoint.X,
                        Mouse.Position.Y,
                        Mouse.Position.X - LastPoint.X,
                        LastPoint.Y - Mouse.Position.Y,
                        0x7F2E86C1);
                }

            }
            else if (Mouse.Buttons == MouseButtons.None)
            {
                LastPoint = new Point(-1, -1);
            }
        }

        void DrawWallPaper()
        {
            if (Wallpaper == null)
            {
                GDI.FillRectangle(0, 0, screenWidth, screenHeight, 0xFF55AAAA);
            }
            else
            {
                GDI.DrawImage(0, 0, Wallpaper, false);
            }
        }

        void DrawBottomBar()
        {
            GDI.AFillRectangle(0, screenHeight - 48, screenWidth, 48, 0xFFC3C7CB);
            GDI.DrawLine(0, screenHeight - 46, screenWidth, screenHeight - 46, 0xFFFFFFFF);

            start.OnDraw();
            menu.OnDraw();
            clock.Text = GetHourAndMinute();
            clock.OnDraw();
        }

        void start_clicked(object sender, object e)
        {
            menu.IsVisible = !menu.IsVisible;
        }
    }
}
