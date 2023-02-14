using Internal.Runtime.CompilerServices;
using System;
using System.Drawing;
using System.Runtime;

namespace System.Moos
{
    public unsafe class Application
    {
        public static Application Current { get; private set; }
      
        public string StartupUri { set; get; }
        public ResourceDictionary Resources { set; get; }
        public WindowCollection Windows { get; private set; }

        [RuntimeExport("Main")]
        public static void Main()
        {
            Current = new Application();

            for (; ; )
            {

            }
        }

        public Application()
        {
            Windows = new WindowCollection();
            OnStartup();
        }

        public virtual void OnStartup()
        {
            //Start Main Window (from StartupUri)
            Windows.Add(new AppDemo.MainWindow()); // method temp
        }

        internal static void LoadComponent(object component, string resourceLocator)
        {
            
        }
    }
}
