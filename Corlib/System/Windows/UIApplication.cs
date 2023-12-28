using Internal.NativeFormat;
using Internal.Runtime.CompilerServices;
using System;
using System.Drawing;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Forms;
using static Moos.Core.System.Windows.UIApplication;


namespace Moos.Core.System.Windows
{
    public unsafe class UIApplication
    {
        [DllImport("_GUI")]
        public static extern void ModeGUI();

        [DllImport("ApplicationCreate")]
        public static extern uint ApplicationCreate(IntPtr handler);

        [DllImport("ApplicationDraw")]
        public static extern void ApplicationDraw(IntPtr hanbler, IntPtr draw);

        public uint processID { get; private set; }
        public Action DrawHandler { get; private set; }

        public UIApplication()
        {
            processID = ApplicationCreate(this);
            ModeGUI();
        }

        public void Run()
        {
            for(; ; )
            {

            }
        }

        public void setApiDraw(Action func)
        {
            while(processID == 0)
            {
                
            }
            DrawHandler = func;
            ApplicationDraw(this, func);
        }


        public void setDraw(Action func)
        {
            DrawHandler = func;
        }

        public void Draw()
        {
            DrawHandler?.Invoke();
        }
    }
}
