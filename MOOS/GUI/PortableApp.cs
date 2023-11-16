using System;
using System.Drawing;
using System.Windows.Controls;

namespace MOOS.GUI
{
    internal class PortableApp 
    {
        public int X { set; get; } 
        public int Y { set; get; }
        public int Width { set; get; }
        public int Height { set; get; }

        public Image ScreenBuf;

        public PortableApp(int X, int Y, int Width, int Height) 
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;

            ScreenBuf = new Image(Width, Height);
        }

        public virtual void OnInput()
        {
            //TO-DO...
        }

        public virtual void OnDraw()
        {
            Framebuffer.Graphics.DrawImage(this.X, this.Y, ScreenBuf, false);
        }
    }
}
