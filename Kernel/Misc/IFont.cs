using MOOS.Graph;
using MOOS.Media;
using System;
using System.Diagnostics;
using System.Drawing;

namespace MOOS.Misc
{
    public class IFont
    {
        private readonly Image image;
        private readonly string charset;

        public int FontSize;

        public int NumRow => image.Width / FontSize;

        public IFont(Image _img, string _charset, int size)
        {
            image = _img;
            charset = _charset;
            FontSize = size;
        }

        public int DrawChar(Graphics g, int X, int Y, char Chr, uint color = 0xFF000000)
        {
            int index = charset.IndexOf(Chr);
            if (index == -1)
            {
                if (Chr == ' ') return FontSize / 2;
                return 0;
            }

            int baseX = 0, baseY = 0;
            for (int i = 0; i <= index; i++)
            {
                if ((i % NumRow) == 0 && i != 0)
                {
                    baseX = 0;
                    baseY += FontSize;
                }
                if (i != index)
                    baseX += FontSize;
            }

            for (int w = 0; w < FontSize; w++)
            {
                int counter = 0;
                for (int h = 0; h < FontSize; h++)
                {
                    uint _color = image.GetPixel(baseX + w, baseY + h);

                    if (X >= 0 && Y >= 0 && X <= g.Width && Y <= g.Height)
                    {
                        //Replace pixel color
                        //_color = ColorConverter.ConvertPixel(_color, color);
                        g.DrawPoint((X + w), (Y + h), _color, true);
                    }

                    if ((_color & 0xFF000000) == 0)
                    {
                        counter++;
                    }
                    _color.Dispose();
                }
                if (w > (FontSize / 3) && counter == FontSize) return w;
            }

            return FontSize;
        }


        public int DrawString(int X, int Y, string Str, Graphics g)
        {
            int w = 0, h = 0;
            for (int i = 0; i < Str.Length; i++)
            {
                w += DrawChar(g, X + w, Y + h, Str[i]);
            }
            Str.Dispose();
            return w;
        }

        public int MeasureString(string Str)
        {
            int w = 0;
            for (int i = 0; i < Str.Length; i++)
            {
                w += 9;// DrawChar(Framebuffer.Graphics,-1, -1, Str[i]);
            }
            Str.Dispose();
            return w;
        }


        public int DrawString(int X, int Y, string Str, uint color = 0xFF000000, int LineLimit = -1, int HeightLimit = -1)
        {
            int w = 0, h = 0;
            for (int i = 0; i < Str.Length; i++)
            {
                if (h != 0 && w == 0 && Str[i] == ' ') continue;
                w += DrawChar(Framebuffer.Graphics, X + w, Y + h, Str[i], color);
                if (w + FontSize > LineLimit && LineLimit != -1 || Str[i] == '\n')
                {
                    w = 0;
                    h += FontSize;

                    if (HeightLimit != -1 && h >= HeightLimit)
                    {
                        break;
                    }
                }
            }

            Str.Dispose();
            return w;
        }
    }
}