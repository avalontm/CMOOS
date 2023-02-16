using MoosFramework;
using System;
using System.Collections.Generic;
using System.Moos.Controls;
using System.Text;

namespace System.Moos.Controls
{
    [ContentProperty(nameof(Children))]
    public class Grid : ContentControl
    {
        public List<ContentControl> Children { set; get; }

        public Grid() 
        {
            Children = new List<ContentControl>();
        }

        public override void Generate(Window owner)
        {
            base.Generate(owner);
            this.Handler = Program.CreateGrid(owner.Handle, X, Y, Width, Height, Content, Background.ToArgb());
        }

    }
}
