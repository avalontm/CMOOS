using Moos.Framework.Input;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Moos.Framework.Controls
{
    public partial class ContentControl : IView
    {
        public string Name { get; set; }

        public IntPtr Handler { get; internal set; }
        public Window OwnerWindow { set; get; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Thickness Margin { get; set; }

        public Color Background { get; set; }

        public ContentControl()
        {

        }
    }
}
