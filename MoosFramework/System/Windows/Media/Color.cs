namespace System.Windows.Media
{
    public class Color
    {
        public Color()
        {
            ARGB = ToArgb(0,0,0);
        }

        public Color(int argb)
        {
            ARGB = argb;
        }

        public Color(uint argb)
        {
            ARGB = argb;
        }

        public Color(int r, int g, int b)
        {
            ARGB = (uint)(255 << 24 | r << 16 | g << 8 | b);
        }

        public Color(int a, int r, int g, int b)
        {
            ARGB = (uint)(a << 24 | r << 16 | g << 8 | b);
        }

        public uint ARGB;

        public byte A
        {
            get
            {
                return ((byte)((ARGB >> 24) & 0xFF));
            }
            set
            {
                ARGB &= ~0xFF000000;
                ARGB |= (uint)(value << 24);
            }
        }

        public byte R
        {
            get
            {
                return ((byte)((ARGB >> 16) & 0xFF));
            }
            set
            {
                ARGB &= ~0x00FF0000U;
                ARGB |= (uint)(value << 16);
            }
        }

        public byte G
        {
            get
            {
                return ((byte)((ARGB >> 8) & 0xFF));
            }
            set
            {
                ARGB &= ~0x0000FF00U;
                ARGB |= (uint)(value << 8);
            }
        }

        public byte B
        {
            get
            {
                return ((byte)((ARGB >> 0) & 0xFF));
            }
            set
            {
                ARGB &= ~0x000000FFU;
                ARGB |= (uint)(value << 0);
            }
        }

        public uint ToArgb()
        {
            return ARGB;
        }

        public static uint ToArgb(byte r, byte g, byte b)
        {
            return (uint)(255 << 24 | r << 16 | g << 8 | b);
        }

        public static uint ToArgb(byte a, byte r, byte g, byte b)
        {
            return (uint)(a << 24 | r << 16 | g << 8 | b);
        }

        public static Color FromArgb(byte red, byte green, byte blue)
        {
            return new Color() { ARGB = ToArgb(red, green, blue) };
        }

        public static Color FromArgb(int alpha, int red, int green, int blue)
        {
            return new Color() { ARGB = ToArgb((byte)alpha, (byte)red, (byte)green, (byte)blue) };
        }

        public static Color FromArgb(int alpha, Color color)
        {
            return new Color() { ARGB = ToArgb((byte)alpha, color.R, color.G, color.B) };
        }

        public static Color FromArgb(int red, int green, int blue)
        {
            return new Color() { ARGB = ToArgb((byte)red, (byte)green, (byte)blue) };
        }

        public static Color FromArgb(byte alpha, byte red, byte green, byte blue)
        {
            return new Color() { ARGB = ToArgb(alpha, red, green, blue) };
        }

        public static Color FromArgb(uint argb)
        {
            return new Color() { ARGB = argb };
        }
    }
}
