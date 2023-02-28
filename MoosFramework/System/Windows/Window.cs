using Moos.Framework.Controls;
using System.Windows.Markup;

namespace System.Windows
{
    public partial class Window : IWindow, IComponentConnector
    {
        private bool _contentLoaded;

        public Window()
        {

        }

        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
        }

        public void Connect(int connectionId, object target)
        {
            _contentLoaded = true;
        }

    }
}
