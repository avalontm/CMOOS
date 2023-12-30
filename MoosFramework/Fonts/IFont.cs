using Internal.Runtime.CompilerServices;
using System;
using System.Drawing;
using System.Runtime.InteropServices;


namespace Moos.Framework.Fonts
{
    public class Font 
    {
        [DllImport("FontCreate")]
        static extern IntPtr FontCreate(string file, int size);
        [DllImport("FontDrawString")]
        static extern void FontDrawString(IntPtr handle, int x, int y, string text, uint color);
        [DllImport("FontMeasureString")]
        static extern int FontMeasureString(IntPtr handle, string text);

        internal IntPtr handler { private set;  get; }
        public int Size { private set; get; }
 
        public Font(string file, int size) {
            handler = FontCreate(file, size);
            Size = size;
        }

        public void DrawString(int x, int y, string msg, Color color)
        {
            FontDrawString(handler, x, y, msg, color.ARGB);
        }

        public void DrawString(int x, int y, string msg, uint color)
        {
            FontDrawString(handler, x, y, msg, color);
        }

        public int MeasureString(string text)
        {
            int measure = FontMeasureString(handler, text);
            return measure;
        }


    }
}
