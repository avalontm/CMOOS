using Internal.NativeFormat;
using Internal.Runtime.CompilerServices;
using System;
using System.Drawing;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Forms;


namespace System.Windows
{
    public unsafe class Application
    {
        #region Native
        [DllImport("SwitchToMode")]
        public static extern void SwitchToMode(bool gui);

        [DllImport("GetProcess")]
        public static extern IntPtr GetProcess(uint processID);

        [DllImport("ApplicationCreate")]
        public static extern uint ApplicationCreate(IntPtr handler);

        [DllImport("StartThread")]
        public static extern void StartThread(delegate*<void> ptr);

        [DllImport("GetCurrentProcess")]
        public static extern uint GetCurrentProcess();

        [DllImport("KillProcess")]
        public static extern bool KillProcess(uint processID);
        #endregion

        public static uint processID { get; private set; }

        public Application()
        {
            processID = ApplicationCreate(this);
            SwitchToMode(false);
        }

        public void Run()
        {
            for(; ; )
            {

            }
        }
    }
}
