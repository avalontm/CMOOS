﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MOOS
{
    internal unsafe class Strings
    {
        public static int strlen(byte* c)
        {
            int i = 0;
            while (c[i] != 0) i++;
            return i;
        }
    }
}
