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
                Debug.WriteLine($"[API_ControlForeground] {color}");
                control.Foreground = new Windows.Media.Brush(color);
                control._old_foreground = control.Foreground;
                return color;
            }

            return 0;
        }

        static uint API_ControlBackground(IntPtr handler, uint color)
        {
            Widget control = Unsafe.As<IntPtr, Widget>(ref handler);

            if (control != null)
            {
                Debug.WriteLine($"[API_ControlBackground] {color}");
                control.Background = new Windows.Media.Brush(color);
                control._old_background = control.Background;
                control.onSetHighLight();

                return color;
            }

            return 0;
        }
    }
}
