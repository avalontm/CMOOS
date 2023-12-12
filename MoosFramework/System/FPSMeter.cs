using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Moos.Framework.System
{

    public class FPSMeter
    {
        [DllImport("getTimeSeconds")]
        static extern byte GetTimeSeconds();


        public int FPS = 0;

        public int LastS = -1;
        public int Tick = 0;
        private byte seconds = 0;

        public void Update()
        {
            seconds = GetTimeSeconds();

            if (LastS == -1)
            {
                LastS = seconds;
            }
            if (seconds - LastS != 0)
            {
                if (seconds > LastS)
                {
                    FPS = Tick / (seconds - LastS);
                }
                LastS = seconds;
                Tick = 0;
            }
            Tick++;
        }
    }
}
