using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Moos.Framework.Controls
{
    [ContentProperty(nameof(Children))]
    public class Grid : ContentControl
    {
        [DllImport("GridCreate")]
        public static extern IntPtr GridCreate(IntPtr handle, int x, int y, int width, int height, string content, uint background);


        public List<ContentControl> Children { set; get; }

        public Grid()
        {
            Children = new List<ContentControl>();
        }

    }
}
