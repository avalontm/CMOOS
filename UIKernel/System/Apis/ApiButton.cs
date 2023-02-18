/*************************************************/
/*          MOOS API CONTROL BUTTON             */
/************************************************/

using Internal.Runtime.CompilerServices;
using MOOS.Driver;
using MOOS.NET.IPv4.TCP;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace System.Apis
{
    public static unsafe class ApiButton
    {
        public static unsafe void* HandleSystemCall(string name)
        {
            switch (name)
            {
                case "ButtonCreate":
                    return (delegate*<IntPtr>)&API_ButtonCreate;
                case "ButtonText":
                    return (delegate*<IntPtr, string, IntPtr>)&API_ButtonText;
                case "ButtonWidth":
                    return (delegate*<IntPtr, int, int>)&API_ButtonWidth;
                case "ButtonHeight":
                    return (delegate*<IntPtr, int, int>)&API_ButtonHeight;
                case "ButtonCommand":
                    return (delegate*<IntPtr, IntPtr, IntPtr>)&API_ButtonCommand;
                case "ButtonCommandParameter":
                    return (delegate*<IntPtr, IntPtr, IntPtr>)&API_ButtonCommandParameter;
                case "ButtonBackground":
                    return (delegate*<IntPtr, int, int>)&API_ButtonBackground;
                case "ButtonForeground":
                    return (delegate*<IntPtr, int, int>)&API_ButtonForeground;
                case "ButtonMargin":
                    return (delegate*<IntPtr, int,int, int, int, void>)&API_ButtonMargin;
            }

            return null;
        }

        public static IntPtr API_ButtonCreate()
        {
            Button button = new Button();
            return button;
        }

        public static IntPtr API_ButtonText(IntPtr handler, string text)
        {
            Button control = Unsafe.As<IntPtr, Button>(ref handler);
            
            if (control != null)
            {
                control.Text = text;
                return control.Text;
            }

            return IntPtr.Zero;
        }

        public static int API_ButtonWidth(IntPtr handler, int width)
        {
            Button control = Unsafe.As<IntPtr, Button>(ref handler);

            if (control != null)
            {
                control.Width = width;
                return control.Width;
            }

            return 0;
        }

        public static int API_ButtonHeight(IntPtr handler, int height)
        {
            Button control = Unsafe.As<IntPtr, Button>(ref handler);

            if (control != null)
            {
                control.Height = height;
                return control.Height;
            }

            return 0;
        }

        public static IntPtr API_ButtonCommand(IntPtr handler, IntPtr command)
        {
            Button control = Unsafe.As<IntPtr, Button>(ref handler);

            if (control != null)
            {
                control.Command = new System.Windows.Data.Binding();
                control.Command.Source = Unsafe.As<IntPtr, ICommand>(ref command);
                return control;
            }

            return IntPtr.Zero;
        }

        public static IntPtr API_ButtonCommandParameter(IntPtr handler, IntPtr commandParameter)
        {
            Button control = Unsafe.As<IntPtr, Button>(ref handler);

            if (control != null)
            {
                control.CommandParameter = Unsafe.As<IntPtr, ICommand>(ref commandParameter);
                return control;
            }

            return IntPtr.Zero;
        }

        public static int API_ButtonBackground(IntPtr handler, int color)
        {
            Button control = Unsafe.As<IntPtr, Button>(ref handler);

            if (control != null)
            {
                control.Background = new Windows.Media.Brush(color);
                return (int)control.Background.Value;
            }

            return 0;
        }

        public static int API_ButtonForeground(IntPtr handler, int color)
        {
            Button control = Unsafe.As<IntPtr, Button>(ref handler);

            if (control != null)
            {
                control.Foreground = new Windows.Media.Brush(color);
                return (int)control.Foreground.Value;
            }

            return 0;
        }
        
        public static void API_ButtonMargin(IntPtr handler, int left, int top, int right, int bottom)
        {
            Button control = Unsafe.As<IntPtr, Button>(ref handler);

            if (control != null)
            {
                control.Margin = new Thickness(left, top, right, bottom);
            }
        }
    }
}
