using System.Collections.Generic;
using System.Text;

namespace Moos.Framework.Controls
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
            Handler = Program.CreateGrid(owner.Handle, X, Y, Width, Height, Text, Background.ToArgb());
        }

    }
}
