using System;
using System.Collections.Generic;
using System.Drawing;


namespace Moos.Framework.Fonts
{
    public static class FontManager
    {
        public static Font font;

        public static void Load(string file, int size )
        {
            font = new Font(file, size);
        }
    }

}
