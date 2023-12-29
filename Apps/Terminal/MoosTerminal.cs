using Internal.Runtime.CompilerServices;
using Moos.Framework.Fonts;
using Moos.Framework.Graphics;
using Moos.Framework.System;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.IO;
using System.Media;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;
using Terminal;
using Terminal.Managers;


namespace MoosExplorer
{
    public static unsafe class MoosTerminal
    {
        [RuntimeExport("Main")]
        public static void Main()
        {
            new App().Run();
        }
    }
}
