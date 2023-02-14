using System;

namespace System.Moos.Input
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
