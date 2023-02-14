using Internal.Runtime.CompilerServices;
using System.Drawing;
using System.Moos.Controls;
using System.Moos.Input;

namespace System.Moos.Controls
{
    public class Button : ContentControl
    {
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }

        public Button()
        {
          
        }

        public override void Generate(Window owner)
        {
            base.Generate(owner);
            this.Handler = NativeMethod.CreateButton(owner.Handle, X, Y, Width, Height, Content, Background.ToArgb(), Command, CommandParameter);
        }
    }
}
