using Moos.Framework.Controls;
using System.Windows;

namespace Moos.Framework
{
    public unsafe class Application : IApplicationBase
    {
        public static Application Current { get; internal set; }

        public string StartupUri { set; get; }
        public ResourceDictionary Resources { set; get; }
        public WindowCollection Windows { get; internal set; }
        public Window MainWindow { get; internal set; }
        public bool isRunning { set; get; }

        public Application()
        {
            Windows = new WindowCollection();
            Current = this;
        }

        public void Run(Window main)
        {
            MainWindow = main;

            if (main != null)
            {
                MainWindow.ShowDialog();
            }

            isRunning = true;

            //wait while is running
            while (Current.isRunning)
            {

            }
        }
    }
}
