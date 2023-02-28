using Internal.Runtime.CompilerServices;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Moos.Framework.Controls
{
    [ContentProperty(nameof(Content))]
    public class Window : Layout
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

        bool _contentLoaded;

        public Image ScreenBuf { get; private set; }
        public IntPtr Handler { get; private set; }

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

        int _height = 150;
        public int Height
        {
            get { return _height; }
            set
            {
                _height = WindowHeight(Handler, value);
            }
        }

        int _width = 300;
        public int Width
        {
            get { return _width; }
            set
            {
                _width = WindowWidth(Handler, value);
            }
        }

        public object DataContext { get; set; }

        object _content;
        public object Content
        {
            get { return _content; }
            set
            {
                _content = value;
                WindowContent(Handler, _content);
            }
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title");

        public Window()
        {
            Handler = WindowCreate();
            var screenBufHandler = WindowGetScreenBuf(Handler);
            ScreenBuf = Unsafe.As<IntPtr, Image>(ref screenBufHandler);
        }

        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }

            _contentLoaded = true;

            string resourceLocater = "/AppDemo;component/mainwindow.xaml";
            Application.LoadComponent(this, resourceLocater);

            Debug.WriteLine($"[InitializeComponent] {resourceLocater}");
            resourceLocater.Dispose();
        }

        public virtual void OnLoaded()
        {

        }

        public virtual void OnUnloaded()
        {

        }

        public void ShowDialog()
        {
            WindowShowDialog(Handler);
        }


        public void Close()
        {
            WindowClose(Handler);
        }
    }
}
