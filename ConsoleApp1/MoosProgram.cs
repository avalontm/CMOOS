using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;

namespace MoosApplication
{
    public static unsafe class MoosProgram
    {
        [RuntimeExport("Main")]
        public static void Main()
        {
            App app = new App();
            app.Run(new MainWindow());
        }
    }
}
