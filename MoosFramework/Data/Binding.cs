using Moos.Framework.Input;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Moos.Framework.Data
{
    public class Binding
    {
        public Action<object> Source { get; set; }

        public Binding(string path = "")
        {
        }
    }
}
