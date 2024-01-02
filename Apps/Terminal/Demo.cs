using Moos.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terminal
{
    public class Demo : IApp
    {
        public void Draw()
        {
            Console.WriteLine("DEMO interface");
        }
    }
}
