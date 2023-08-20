using Moos.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Moos.Framework.Controls
{
    public class Button : ContentControl, ICommandSource
    {
        public static object CommandProperty;

        public Thickness Margin { get; set; }
        public string Text { get; set; }
        public ICommand Command { get; set; }
        public EventHandler Click { get; set; }
        public object CommandParameter { get; set; }
        public Color Background { get; set; }
        public Color Foreground { get; set; }
        public IInputElement CommandTarget { get; set; }
    }

}
