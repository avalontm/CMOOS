/*************************************************/
/*          MOOS API CONTROL WINDOW              */
/************************************************/
using Internal.Runtime.CompilerServices;
using MOOS.Driver;
using System.Windows;
using System.Windows.Controls;

namespace System.Apis
{
    public unsafe static class ApiWindow
    {
        public static unsafe void* HandleSystemCall(string name)
        {
            switch (name)
            {
                case "WindowGetScreenBuf":
                    return (delegate*<IntPtr, IntPtr>)&API_WindowGetScreenBuf;
                case "WindowCreate":
                    return (delegate*<IntPtr>)&API_WindowCreate;
                case "WindowTitle":
                    return (delegate*<IntPtr, string, IntPtr>)&API_WindowTitle;
                case "WindowWidth":
                    return (delegate*<IntPtr, int, int>)&API_WindowWidth;
                case "WindowHeight":
                    return (delegate*<IntPtr, int, int>)&API_WindowHeight;
                case "WindowContent":
                    return (delegate*<IntPtr, IntPtr, IntPtr>)&API_WindowContent;
                case "WindowShowDialog":
                    return (delegate*<IntPtr, IntPtr>)&API_WindowShowDialog;
                case "WindowClose":
                    return (delegate*<IntPtr, void>)&API_WindowClose;
            }

            return null;
        }

        public static IntPtr API_WindowGetScreenBuf(IntPtr handle)
        {
            PortableApp papp = Unsafe.As<IntPtr, PortableApp>(ref handle);

            if (papp != null)
            {
                return papp.ScreenBuf;
            }

            return IntPtr.Zero;
        }

        public static IntPtr API_WindowCreate()
        {
            PortableApp papp = new PortableApp();

            if (papp != null)
            {
                return papp;
            }

            return IntPtr.Zero;
        }

        public static IntPtr API_WindowShowDialog(IntPtr handler)
        {
            PortableApp papp = Unsafe.As<IntPtr, PortableApp>(ref handler);

            if (papp != null)
            {
                papp.ShowDialog();
                return papp;
            }

            return IntPtr.Zero;
        }
        public static IntPtr API_WindowTitle(IntPtr handler, string title)
        {
            PortableApp papp = Unsafe.As<IntPtr, PortableApp>(ref handler);

            if (papp != null)
            {
                papp.Title = title;
                return papp.Title;
            }

            return IntPtr.Zero;
        }

        public static int API_WindowWidth(IntPtr handler, int width)
        {
            PortableApp papp = Unsafe.As<IntPtr, PortableApp>(ref handler);

            if (papp != null)
            {
                papp.Width = width;
                return papp.Width;
            }

            return 0;
        }

        public static int API_WindowHeight(IntPtr handler, int height)
        {
            PortableApp papp = Unsafe.As<IntPtr, PortableApp>(ref handler);

            if (papp != null)
            {
                papp.Height = height;
                return papp.Height;
            }

            return 0;
        }

        public static IntPtr API_WindowContent(IntPtr owner, IntPtr content)
        {
            PortableApp papp = Unsafe.As<IntPtr, PortableApp>(ref owner);

            if (papp != null)
            {
                Widget _content = Unsafe.As<IntPtr, Widget>(ref content);

                if (_content != null)
                {
                    papp.Content = _content;
                }

                return papp;
            }

            return IntPtr.Zero;
        }

        public static void API_WindowClose(IntPtr handler)
        {
            PortableApp papp = Unsafe.As<IntPtr, PortableApp>(ref handler);

            if (papp != null)
            {
                papp.OnClose();
            }
        }

    }
}
