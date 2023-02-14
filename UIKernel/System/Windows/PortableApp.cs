using MOOS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace System.Windows
{
    public class PortableApp : Window
    {
        public Image ScreenBuf;

        public PortableApp(int X, int Y, int Width, int Height, string Title)
        {
            this.Title = Title; 
            this.X = X;
            this.Y = Y;
            this.Width = Width + 1;
            this.Height = Height + 1;
            ScreenBuf = new Image(Width, Height);
        }

        public override void OnInput()
        {
            base.OnInput();

            //TO-DO...
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

        }

        public override void OnLoaded()
        {
            base.OnLoaded();
        }

        public override void OnDraw()
        {
            base.OnDraw();

            Framebuffer.Graphics.DrawImage(this.X, this.Y, ScreenBuf, true);
        }
    }
}
