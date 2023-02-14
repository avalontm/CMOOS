using Internal.Runtime.CompilerServices;
using System;
using System.Diagnostics;
using System.Drawing;

namespace System.Moos
{
    public class Window
    {
        bool _contentLoaded;
        Image ScreenBuf;
        public IntPtr Handle { get; private set; }
        public string Title { get; set; } = "Title";
        public int Height { set; get; } = 400;
        public int Width { set; get; } = 600;

        public object DataContext { get; set; }
        public object Content { get; set; }

        public Window()
        {
            Handle = NativeMethod.CreateWindow(0, 0, Width, Height, Title);
            var screenBufHandle = NativeMethod.GetWindowScreenBuf(Handle);
            ScreenBuf = Unsafe.As<IntPtr, Image>(ref screenBufHandle);
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
        }

        public virtual void OnLoaded()
        { 
        
        }

        public virtual void OnUnloaded()
        {

        }
    }
}
