using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Managers
{
    public static class PowerManger
    {
        [DllImport("ShutDown")]
        public static extern void ShutDown();
        [DllImport("Reboot")]
        public static extern void Reboot();
    }
}
