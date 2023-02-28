using Internal.Runtime.CompilerServices;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Moos.Framework.Controls
{
    [ContentProperty(nameof(Content))]
    public class IWindow : Layout
    {
        [DllImport("WindowGetScreenBuf")]
        public static extern IntPtr WindowGetScreenBuf(IntPtr handler);

        [DllImport("WindowCreate")]
        public static extern IntPtr WindowCreate();

        [DllImport("WindowTitle")]
        public static extern IntPtr WindowTitle(IntPtr handler, string title);

        [DllImport("WindowShowDialog")]
        public static extern IntPtr WindowShowDialog(IntPtr handler);

        [DllImport("WindowClose")]
        public static extern void WindowClose(IntPtr handler);

        [DllImport("WindowWidth")]
        public static extern int WindowWidth(IntPtr handler, int width);

        [DllImport("WindowHeight")]
        public static extern int WindowHeight(IntPtr handler, int height);

        [DllImport("WindowContent")]
        public static extern IntPtr WindowContent(IntPtr handler, IntPtr content);

        public Image ScreenBuf { get; private set; }
        public IntPtr Handler { get; private set; }

        internal bool IsHandler { get; private set; }

        string _title = "Title";
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                WindowTitle(Handler, _title);
            }
        }

        double _height = 150;
        public double Height
        {
            get { return _height; }
            set
            {
                _height = WindowHeight(Handler, (int)value);
            }
        }

        double _width = 300;
        public double Width
        {
            get { return _width; }
            set
            {
                _width = WindowWidth(Handler, (int)value);
            }
        }

        ContentControl _content;
        public ContentControl Content
        {
            get { return _content; }
            set
            {
                _content = value;
                //WindowContent(Handler, _content.Handler);
            }
        }

        public IWindow()
        {
            Handler = WindowCreate();
            var screenBufHandler = WindowGetScreenBuf(Handler);
            ScreenBuf = Unsafe.As<IntPtr, Image>(ref screenBufHandler);
            IsHandler = true;
        }


        public virtual void OnLoaded()
        {
            this.Content?.OnLoaded();
        }

        public virtual void OnUnloaded()
        {

        }

        public void ShowDialog()
        {
            OnLoaded();
            WindowShowDialog(Handler);
        }


        public void Close()
        {
            WindowClose(Handler);
        }
    }
}
