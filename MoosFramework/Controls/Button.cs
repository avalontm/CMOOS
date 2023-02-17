using Internal.Runtime.CompilerServices;
using Moos.Framework.Input;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Moos.Framework.Controls
{
    public class Button : ContentControl
    {
        [DllImport("ButtonCreate")]
        public static extern IntPtr ButtonCreate(IntPtr handler);

        [DllImport("ButtonText")]
        public static extern IntPtr ButtonText(IntPtr handler, string text);

        [DllImport("ButtonWidth")]
        public static extern int ButtonWidth(IntPtr handler, int width);

        [DllImport("ButtonHeight")]
        public static extern int ButtonHeight(IntPtr handler, int height);
       
        [DllImport("ButtonCommand")]
        public static extern IntPtr ButtonCommand(IntPtr handler, IntPtr command);

        [DllImport("ButtonCommandParameter")]
        public static extern IntPtr ButtonCommandParameter(IntPtr handler, IntPtr command);

        [DllImport("ButtonBackground")]
        public static extern int ButtonBackground(IntPtr handler, int color);

        string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                ButtonText(Handler, _text);
            }
        }

        int _height = 48;
        public new int Height
        {
            get { return _height; }
            set
            {
                _height = ButtonHeight(Handler, value);
            }
        }

        int _width = 100;
        public new int Width
        {
            get { return _width; }
            set
            {
                _width = ButtonWidth(Handler, value);
            }
        }

        ICommand _command;
        public ICommand Command
        {
            get { return _command; }
            set
            {
                _command = value;
                ButtonCommand(Handler, value);
            }
        }

        object _commandParameter;
        public object CommandParameter
        {
            get { return _commandParameter; }
            set
            {
                _commandParameter = value;
                ButtonCommandParameter(Handler, _commandParameter);
            }
        }

        Color _background;
        public new Color Background
        {
            get { return _background; }
            set
            {
                _background = value;
                ButtonBackground(Handler, (int)_background.ToArgb());
            }
        }

        public Button()
        {
            Handler = ButtonCreate(IntPtr.Zero);
        }

        public Button(Window owner)
        {
            Handler = ButtonCreate(owner.Handler);
        }
    }
}
