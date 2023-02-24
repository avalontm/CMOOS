using Internal.Runtime.CompilerServices;
using Moos.Framework.Controls;
using Moos.Framework.IO;
using System;
using System.Drawing;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace Moos.Framework
{
    public unsafe class Application : IApplicationBase
    {
        public static Application Current { get; internal set; }

        public string StartupUri { set; get; }
        public ResourceDictionary Resources { set; get; }
        public WindowCollection Windows { get; private set; }
        public Window MainWindow { get; internal set; }

        public Application()
        {
            Windows = new WindowCollection();
            Current = this;
        }

        public void Run(Window main)
        {
            if (main != null)
            {
                MainWindow = main;
                MainWindow.ShowDialog();
            }

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
