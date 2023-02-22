using Internal.Runtime.CompilerServices;
using Moos.Framework.Controls;
using Moos.Framework.IO;
using System;
using System.Drawing;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;

namespace Moos.Framework
{
    public unsafe class Application
    {
        public static Application Current { get; internal set; }

        public string StartupUri { set; get; }
        public ResourceDictionary Resources { set; get; }
        public WindowCollection Windows { get; private set; }
        public Window MainWindow { get; internal set; }
        internal IntPtr Handler {private set; get; }

        static string _executablePath;
        public static string ExecutablePath
        {
            get
            {
                if (string.IsNullOrEmpty(_executablePath))
                {
                    _executablePath = "home/moos/Desktop/appdemo.app";
                }

                return _executablePath;
            }
        }

        public Application()
        {
            Windows = new WindowCollection();
            Current = this;
        }

        public void Run(Window main)
        {
            MainWindow = main;
            MainWindow.ShowDialog();

            //wait while is running
            for (; ; )
            {

            }
        }

        internal static void LoadComponent(object component, string resourceLocator)
        {

        }
    }
}
