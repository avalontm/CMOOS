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
            if (string.IsNullOrEmpty(file))
            {
                return null;
            }

            if (file.Length > 4)
            {
                string ext = file.Substring(file.Length - 4, file.Length);

                if (ext != ".mue")
                {
                    file = file + ".mue";
                }
            }
           return Process.Start("sys/app/" + file);
        }
    }
}
