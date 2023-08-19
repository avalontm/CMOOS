/*************************************************/
/*           MOOS API CONTROL GRID              */
/************************************************/

using Internal.Runtime.CompilerServices;
using MOOS.Driver;
using System.Diagnostics;
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
                case "GridChildrenAdd":
                    return (delegate*<IntPtr, IntPtr, IntPtr>)&API_GridChildrenAdd;
                case "GridSetRow":
                    return (delegate*<IntPtr, int, int>)&API_GridSetRow;
                case "GridSetColumn":
                    return (delegate*<IntPtr, int, int>)&API_GridSetColumn;
                    
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
                papp.Content= control;
            }

            return control;
        }

        public static IntPtr API_GridRowDefinitions(IntPtr handler, IntPtr rows)
        {
            Grid grid = Unsafe.As<IntPtr, Grid>(ref handler);

            if (grid != null)
            {
                grid.RowDefinitions = Unsafe.As<IntPtr, RowDefinitionCollection>(ref rows);
                return grid.RowDefinitions;
            }

            return IntPtr.Zero;
        }

        public static IntPtr API_GridColumnDefinitions(IntPtr handler, IntPtr cols)
        {
            Grid grid = Unsafe.As<IntPtr, Grid>(ref handler);

            if (grid != null)
            {
                grid.ColumnDefinitions = Unsafe.As<IntPtr, ColumnDefinitionCollection>(ref cols);
                return grid.ColumnDefinitions;
            }

            return IntPtr.Zero;
        }

        public static IntPtr API_GridChildrenAdd(IntPtr handler, IntPtr control)
        {
            Grid grid = Unsafe.As<IntPtr, Grid>(ref handler);
          
            if (grid != null)
            {
                Widget widget = Unsafe.As<IntPtr, Widget>(ref control);
                widget.Parent = grid;
                grid.Children.Add(widget);
                return widget;
            }

            return IntPtr.Zero;
        }

        public static int API_GridSetRow(IntPtr control, int row)
        {
            Widget widget = Unsafe.As<IntPtr, Widget>(ref control);

            if (widget != null)
            {
                Grid.SetRow(widget, row);
                return widget.GridRow;
            }

            return 0;
        }

        public static int API_GridSetColumn(IntPtr control, int column)
        {
            Widget widget = Unsafe.As<IntPtr, Widget>(ref control);

            if (widget != null)
            {
                Grid.SetColumn(widget, column);
                return widget.GridColumn;
            }

            return 0;
        }
    }
}
