using System.Drawing;
using System.Moos;

namespace System.Moos.Controls
{
    public partial class ContentControl
    {
        public IntPtr Handler { get; internal set; }
        public Window OwnerWindow { set; get; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Thickness Margin { get; set; }
        public string Content { get; set; }
        public Color Background { get; set; }

        public ContentControl()
        { 

        }

        public virtual void Generate(Window owner)
        {
            OwnerWindow = owner;
        }
    }
}
