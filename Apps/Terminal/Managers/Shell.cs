using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terminal.Managers
{
    public static class Shell
    {
        public static Process Start(string file)
        {
           return Process.Start("sys/app/" + file);
        }
    }
}
