using Internal.Runtime.CompilerServices;
using Moos.Framework.Controls;
using System.Drawing;
using System.Runtime;
using System.Runtime.InteropServices;

namespace Moos.Framework
{
    public unsafe class Application
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
            Windows.Add(main);

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
