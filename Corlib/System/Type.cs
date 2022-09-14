using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System
{
    public unsafe class Type
    {
        public string Name { set; get; }

        public override string ToString()
        {
            return $"Type: {Name}";
        }
    }
}
