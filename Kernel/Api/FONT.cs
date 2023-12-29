using Internal.Runtime.CompilerServices;
using MOOS.FS;
using MOOS.Misc;
using System;


namespace MOOS.Api
{
    internal static unsafe class FONT
    {
        public static unsafe void* HandleSystemCall(string name)
        {
            switch (name)
            {
                case "FontCreate":
                    return (delegate*<string, int, IntPtr>)&API_FontCreate;
                case "FontDrawString":
                    return (delegate*<IntPtr, int, int, string, uint, void>)&API_FontDrawString;
                case "FontMeasureString":
                    return (delegate*<IntPtr, string, int>)&API_FontMeasureString;
            }

            return null;
        }

        public static int API_FontMeasureString(IntPtr handler, string text)
        {
           // BitFontDescriptor font = Unsafe.As<IntPtr, BitFontDescriptor>(ref handler);
            return ASC16.MeasureString(text);
        }

        public static void API_FontDrawString(IntPtr handler, int x, int y, string text, uint color)
        {
            //BitFontDescriptor font = Unsafe.As<IntPtr, BitFontDescriptor>(ref handler);
            //BitFont.DrawString(font.Name, color, text, x, y);
            ASC16.DrawString(text, x, y, color);
            text.Dispose();
        }

        public static IntPtr API_FontCreate(string file, int size)
        {
            string CustomCharset = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            BitFontDescriptor bitFont = new BitFontDescriptor("Song", CustomCharset, File.ReadAllBytes(file), size);
            BitFont.RegisterBitFont(bitFont);
            return bitFont;
        }
    }
}
