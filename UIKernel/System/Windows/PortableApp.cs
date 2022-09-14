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

        public PortableApp(int X, int Y, int Width, int Height) : base(X, Y, Width, Height)
        {
            ScreenBuf = new Image(Width, Height);
        }

        public override void OnInput()
        {
            base.OnInput();

            //TO-DO...
        }

        public override void OnDraw()
        {
            base.OnDraw();

            Framebuffer.Graphics.DrawImage(this.X, this.Y, ScreenBuf, false);

            base.DrawBorder();
        }
    }
}
