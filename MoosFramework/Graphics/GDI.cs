using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Moos.Framework.Graphics
{
    public static class GDI
    {
        [DllImport("DrawRectangle")]
        public static extern void DrawRectangle(int x, int y, int wight, int height, uint color);

        [DllImport("AFillRectangle")]
        public static extern void AFillRectangle(int x, int y, int wight, int height, uint color);


        [DllImport("FillRectangle")]
        public static extern void FillRectangle(int x, int y, int wight, int height, uint color);

        [DllImport("DrawPoint")]
        public static extern void DrawPoint(int x, int y, uint color, bool alpha);
        
        [DllImport("DrawLine")]
        public static extern void DrawLine(int x0, int y0, int x1, int y1, uint color);

        [DllImport("DrawImage")]
        public static extern void DrawImage(int x, int y, IntPtr image, bool alpha);
        
        [DllImport("Width")]
        public static extern int GetWidth();

        [DllImport("Height")]
        public static extern int GetHeight();

        [DllImport("DrawUpdate")]
        public static extern void DrawUpdate();

        [DllImport("DrawClear")]
        public static extern void DrawClear();
        
    }
}
