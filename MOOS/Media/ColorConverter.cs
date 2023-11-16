using System;
using System.Drawing;

namespace MOOS.Media
{
    internal static class ColorConverter
    {
        public static uint ConvertFromString(string hex)
        {
            string hexColor = "";

            if (string.IsNullOrEmpty(hex))
            {
                return 0;
            }

            hex = hex.ToUpper();

            if (hex[0] == '#')
            {
                hex = hex.Substring(1);
            }

            if (hex.Length < 8)
            {
                hexColor = "FF" + hex;
            }
            else
            {
                hexColor = hex;
            }

            int i = hexColor.Length > 1 && hexColor[0] == '0' && (hexColor[1] == 'x' || hexColor[1] == 'X') ? 2 : 0;
            uint value = 0;

            while (i < hexColor.Length)
            {
                uint x = hexColor[i++];

                if (x >= '0' && x <= '9') x = x - '0';
                else if (x >= 'A' && x <= 'F') x = (x - 'A') + 10;
                else if (x >= 'a' && x <= 'f') x = (x - 'a') + 10;
                else return 0;

                value = 16 * value + x;
            }

            return value;
        }

        public static uint ConvertPixel(uint pixel, uint color)
        {
            Color _base = Color.FromArgb(pixel);
            Color _color = Color.FromArgb(color);

            int a = (_base.A * _color.A) / 255;
            int r = (_base.R * _color.R) / 255;
            int g = (_base.G * _color.G) / 255;
            int b = (_base.B * _color.B) / 255;

            return (uint)(a << 24 | r << 16 | g << 8 | b);
        }
    }
}
