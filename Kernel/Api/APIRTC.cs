using System;
using System.Collections.Generic;
using System.Text;

namespace MOOS.Api
{
    internal static unsafe class APIRTC
    {
        public static unsafe void* HandleSystemCall(string name)
        {
            switch (name)
            {
                case "getTimeSeconds":
                    return (delegate*<byte>)&API_GetTimeSeconds;
            }

            return null;
        }

        static byte API_GetTimeSeconds()
        {
           return RTC.Second;
        }
    }
}
