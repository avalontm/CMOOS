using Explorer.Managers;
using Moos.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows;

namespace Explorer.Controls
{
    public class MenuBar : Widget
    {
        private Image banner { set; get; }
        private List<Widget> items { set; get; }
        private int wContainer = 25;
        private int hContainer = 48;

        public MenuBar()
        {
            Width = 250;
            Height = 450;

            Background = Color.FromArgb(0xFFC3C7CB);
            Foreground = Color.Black;
            BorderBrush = Color.FromArgb(0xFFFFFFFF);

            items = new List<Widget>();
            banner = PNG.FromFile("sys/media/menu_banner.png");
        }

        public void Show() { IsVisible = true; }

        public void Hide() { IsVisible = false; }

        public override void OnLoaded()
        {
            base.OnLoaded();
            IsVisible = false;


            ItemMenu shutdown = new ItemMenu();
            shutdown.Height = hContainer;
            shutdown.Width = this.Width - wContainer - 3;
            shutdown.X = this.X + wContainer + 1;
            shutdown.Y = this.Y + this.Height - shutdown.Height -4;
            shutdown.Icon = PNG.FromFile("sys/media/menu_shutdown.png");
            shutdown.Text = "Shut Down";
            shutdown.Clicked = onShutDown;
            shutdown.OnLoaded();

            items.Add(shutdown);

            Separator separator = new Separator();
            separator.Height = 2;
            separator.Width = this.Width - wContainer - 3;
            separator.X =  this.X + wContainer + 1;
            separator.Y = this.Y + this.Height - hContainer - 4;

            separator.OnLoaded();

            items.Add(separator);
        }

        void onShutDown(object sender, object e)
        {
            PowerManger.ShutDown();
        }

        public override void OnUnloaded()
        {
            base.OnUnloaded();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnDraw()
        {
            base.OnDraw();

            if (IsLoaded && IsVisible)
            {
                if (Background != null)
                {
                    GDI.FillRectangle(X, Y, Width, Height, Background.ARGB);
                }

                DrawNormalBorder();
                DrawBarVertical();
                DrawItems();
            }
        }

        void DrawItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is Separator)
                {
                    ((Separator)items[i]).OnUpdate();
                    ((Separator)items[i]).OnDraw();
                }
                else if (items[i] is ItemMenu)
                {
                    ((ItemMenu)items[i]).OnUpdate();
                    ((ItemMenu)items[i]).OnDraw();
                }
            }
        }

        void DrawBarVertical()
        {
            GDI.FillRectangle(X, Y, wContainer, (Height-4), 0xFF808080);
            GDI.DrawImage(X, Y + (Height - banner.Height- 4), banner, true);
        }

        void DrawNormalBorder()
        {
            if (IsLoaded && IsVisible)
            {
                //white
                GDI.DrawLine(X, Y, X + Width, Y, 0xFFFFFFFF);
                GDI.DrawLine(X, Y, X, Y + Height - 2, 0xFFFFFFFF);

                //gray
                GDI.DrawLine(X + Width, Y, X + Width, Y + Height - 2, 0xFF868a8e);
                GDI.DrawLine(X - 2, Y + Height - 1, X + Width, Y + Height - 1, 0xFF868a8e);

                //black
                GDI.DrawLine(X + Width + 1, Y, X + Width + 1, Y + Height - 1, 0xFF000000);
                GDI.DrawLine(X - 2, Y + Height, X + Width, Y + Height, 0xFF000000);
            }
        }
    }
}
