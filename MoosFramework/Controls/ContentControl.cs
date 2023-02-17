using Moos.Framework.Input;
using System;
using System.Drawing;

namespace Moos.Framework.Controls
{
    public partial class ContentControl : IView
    {
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }

        public IntPtr Handler { get; internal set; }
        public Window OwnerWindow { set; get; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Thickness Margin { get; set; }
        public string Text { get; set; }
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
