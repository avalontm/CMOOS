using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Moos.Framework.Input
{
    //
    // Resumen:
    //     Defines an object that knows how to invoke a command.
    public interface ICommandSource
    {
        //
        // Resumen:
        //     Gets the command that will be executed when the command source is invoked.
        //
        // Devuelve:
        //     The command that will be executed when the command source is invoked.
        ICommand Command { get; }
        //
        // Resumen:
        //     Represents a user defined data value that can be passed to the command when it
        //     is executed.
        //
        // Devuelve:
        //     The command specific data.
        object CommandParameter { get; }
        //
        // Resumen:
        //     The object that the command is being executed on.
        //
        // Devuelve:
        //     The object that the command is being executed on.
        IInputElement CommandTarget { get; }
    }
}
