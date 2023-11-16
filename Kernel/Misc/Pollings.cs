using System;
using System.Collections.Generic;
using System.Text;
using static MOOS.Misc.Interrupts;

namespace MOOS.Misc
{
    public static unsafe class Pollings
    {
        public static void AddPoll(INTDelegate method)
        {
            Interrupts.EnableInterrupt(0x20, method);
        }
    }
}
