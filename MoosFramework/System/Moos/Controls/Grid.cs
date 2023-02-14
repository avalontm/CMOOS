using System;
using System.Collections.Generic;
using System.Moos.Controls;
using System.Text;

namespace System.Moos.Controls
{
    public class Grid : ContentControl
    {
        public Grid() 
        { 
        
        }

        public override void Generate(Window owner)
        {
            base.Generate(owner);
            this.Handler = NativeMethod.CreateGrid(owner.Handle, X, Y, Width, Height, Content, Background.ToArgb());
        }

    }
}
