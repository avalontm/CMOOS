using Internal.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;

namespace System.Apis
{
    public static unsafe class ApiShared
    {
        public static unsafe void* HandleSystemCall(string name)
        {
            switch (name)
            {
                case "ControlBackground":
                    return (delegate*<IntPtr, uint, uint>)&API_ControlBackground;
                case "ControlForeground":
                    return (delegate*<IntPtr, uint, uint>)&API_ControlForeground;

            }

            return null;
        }

        static uint API_ControlForeground(IntPtr handler, uint color)
        {
            Widget control = Unsafe.As<IntPtr, Widget>(ref handler);

            if (control != null)
            {
                control.Foreground = new Windows.Media.Brush(color);
                control._old_foreground = new Windows.Media.Brush(color);
                return color;
            }

            return 0;
        }

        static uint API_ControlBackground(IntPtr handler, uint color)
        {
            Widget control = Unsafe.As<IntPtr, Widget>(ref handler);

            if (control != null)
            {
                control.Background = new Windows.Media.Brush(color);
                control._old_background = new Windows.Media.Brush(color);
                control.onSetHighLight();

                return color;
            }

            return 0;
        }
    }
}
