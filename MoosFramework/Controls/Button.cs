using Internal.Runtime.CompilerServices;
using Moos.Framework.Input;
using System.Drawing;

namespace Moos.Framework.Controls
{
    public class Button : ContentControl
    {

        public Button()
        {

        }

        public override void Generate(Window owner)
        {
            base.Generate(owner);
            Handler = Program.CreateButton(owner.Handle, X, Y, Width, Height, Text, Background.ToArgb(), Command, CommandParameter);
        }
    }
}
