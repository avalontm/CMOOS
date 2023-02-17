/*************************************************/
/*           MOOS API CONTROL GRID              */
/************************************************/

using Internal.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace System.Apis
{
    public static unsafe class ApiGrid
    {
        public static unsafe void* HandleSystemCall(string name)
        {
            switch (name)
            {
                case "GridCreate":
                    return (delegate*<IntPtr, IntPtr>)&API_GridCreate;
                case "GridRowDefinitions":
                    return (delegate*<IntPtr, IntPtr, IntPtr>)&API_GridRowDefinitions;
                case "GridColumnDefinitions":
                    return (delegate*<IntPtr, IntPtr, IntPtr>)&API_GridColumnDefinitions;
            }

            return null;
        }

        public static IntPtr API_GridCreate(IntPtr owner)
        {
            Grid control = new Grid();

            PortableApp papp = Unsafe.As<IntPtr, PortableApp>(ref owner);

            if (papp != null)
            {
                control.Parent = papp;
                papp.Content = control;
            }

            return control;
        }

        public static IntPtr API_GridRowDefinitions(IntPtr handler, IntPtr rows)
        {

            Grid grid = Unsafe.As<IntPtr, Grid>(ref handler);

            if (grid != null)
            {
                grid.RowDefinitions = Unsafe.As<IntPtr, RowDefinitionCollection>(ref rows);
            }

            return grid;
        }

        public static IntPtr API_GridColumnDefinitions(IntPtr handler, IntPtr cols)
        {

            Grid grid = Unsafe.As<IntPtr, Grid>(ref handler);

            if (grid != null)
            {
                grid.ColumnDefinitions = Unsafe.As<IntPtr, ColumnDefinitionCollection>(ref cols);
            }

            return grid;
        }
    }
}
