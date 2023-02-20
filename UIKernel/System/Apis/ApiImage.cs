using Internal.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace System.Apis
{
    public static unsafe class ApiImage
    {
        public static unsafe void* HandleSystemCall(string name)
        {
            switch (name)
            {
                case "ImageCreate":
                    return (delegate*<IntPtr>)&API_ImageCreate;
                case "ImageSource":
                    return (delegate*<IntPtr, IntPtr, IntPtr>)&API_ImageSource;
                case "ImageWidth":
                    return (delegate*<IntPtr, int, int>)&API_ImageWidth;
                case "ImageHeight":
                    return (delegate*<IntPtr, int, int>)&API_ImageHeight;
                case "ImageMargin":
                    return (delegate*<IntPtr, int, int, int, int, void>)&API_ImageMargin;
            }

            return null;
        }

        public static IntPtr API_ImageCreate()
        {
            Picture image = new Picture();
            return image;
        }

        public static IntPtr API_ImageSource(IntPtr handler, IntPtr source)
        {
            Picture control = Unsafe.As<IntPtr, Picture>(ref handler);

            if (control != null)
            {
                System.Drawing.Image imageSource = Unsafe.As<IntPtr, System.Drawing.Image>(ref source);
                control.ImageSource = imageSource;
                control.Width = imageSource.Width;
                control.Height = imageSource.Height;
                return control.ImageSource;
            }

            return IntPtr.Zero;
        }

        public static int API_ImageWidth(IntPtr handler, int width)
        {
            Picture control = Unsafe.As<IntPtr, Picture>(ref handler);

            if (control != null)
            {
                control.Width = width;
                control.OnResize(control.Width, control.Height);
                return control.Width;
            }

            return 0;
        }

        public static int API_ImageHeight(IntPtr handler, int height)
        {
            Picture control = Unsafe.As<IntPtr, Picture>(ref handler);

            if (control != null)
            {
                control.Height = height;
                control.OnResize(control.Width, control.Height);
                return control.Height;
            }

            return 0;
        }

        public static void API_ImageMargin(IntPtr handler, int left, int top, int right, int bottom)
        {
            Button control = Unsafe.As<IntPtr, Button>(ref handler);

            if (control != null)
            {
                control.Margin = new Thickness(left, top, right, bottom);
            }
        }

    }
}
