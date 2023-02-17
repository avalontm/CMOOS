using System;

namespace Moos.Framework.Input
{
    public class ICommand
    {
        public Action<object> Execute { set; get; }

        public ICommand(Action<object> action)
        {
            Execute = action;
        }
    }
}
