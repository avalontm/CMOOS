using Explorer.Controls;
using Explorer.Managers;
using Moos.Core.System.Windows;
using Moos.Framework.Fonts;
using Moos.Framework.Graphics;
using Moos.Framework.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        static Point LastPoint = new Point();
        static FPSMeter pfs = null;

        public App() 
        {
            FontManager.Load("sys/media/Yahei.png", 18);
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
            start.Width = 75;
            start.Height = 38;
            start.Text = "start";
            start.Clicked = start_clicked;
            start.OnLoaded();

            menu = new MenuBar();
            menu.X = 2;
            menu.Y = (screenHeight - 48) - menu.Height;
            menu.OnLoaded();

            setApiDraw(onDraw);
        }

        static void onDraw()
        {
            pfs.Update();

            CursorManager.Update();
            start.OnUpdate();
            menu.OnUpdate();

            DrawWallPaper();
            DrawSelection();
            DrawBottomBar();

            DrawMessagBox();

            FontManager.font.DrawString(0, 0, string.Format("FPS: {0}", pfs.FPS), 0xFFFFFFFF);

            CursorManager.Draw();

        }


        private static void DrawMessagBox()
        {
            MessageBox.Instance.OnUpdate();
            MessageBox.Instance.OnDraw();
        }

        static void DrawSelection()
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

        static void DrawWallPaper()
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

        static void DrawBottomBar()
        {
            GDI.AFillRectangle(0, screenHeight - 48, screenWidth, 48, 0xFFC3C7CB);
            GDI.DrawLine(0, screenHeight - 46, screenWidth, screenHeight - 46, 0xFFFFFFFF);

           start.OnDraw();
           menu.OnDraw();
        }

        static void start_clicked(object sender, object e)
        {
            menu.IsVisible = !menu.IsVisible;
        }
    }
}
