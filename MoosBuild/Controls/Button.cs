using Moos.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Moos.Framework.Controls
{
    public class Button : ContentControl, ICommandSource
    {
        public static object CommandProperty;

        public string Text { get; set; }
        public ICommand Command { get; set; }

        public object CommandParameter { get; set; }

        public IInputElement CommandTarget { get; set; }
    }

}
