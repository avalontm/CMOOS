using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls;

namespace Moos.Framework.Controls
{
    [ContentProperty(nameof(Children))]
    public class Grid : ContentControl
    {
        [DllImport("GridCreate")]
        public static extern IntPtr GridCreate(IntPtr owner);
        [DllImport("GridRowDefinitions")]
        public static extern IntPtr GridRowDefinitions(IntPtr handler, IntPtr rows);
        [DllImport("GridColumnDefinitions")]
        public static extern IntPtr GridColumnDefinitions(IntPtr handler, IntPtr cols);
        [DllImport("GridChildrenAdd")]
        public static extern IntPtr GridChildrenAdd(IntPtr handler, IntPtr control);
        [DllImport("GridSetRow")]
        public static extern int GridSetRow(IntPtr control, int row);
        [DllImport("GridSetColumn")]
        public static extern int GridSetColumn(IntPtr handler, IntPtr control, int column);


        List<ContentControl> _children;
        public List<ContentControl> Children
        {
            set
            {
                _children = value;
            }
            get { return _children; }
        }
        

        RowDefinitionCollection _rowDefinitions;
        public RowDefinitionCollection RowDefinitions
        {
            set
            {
                _rowDefinitions = value;
                GridRowDefinitions(Handler, _rowDefinitions);
            }
            get { return _rowDefinitions; }
        }

        ColumnDefinitionCollection _columnDefinitions;
        public ColumnDefinitionCollection ColumnDefinitions
        {
            set
            {
                _columnDefinitions = value;
                GridColumnDefinitions(Handler, _columnDefinitions);
            }
            get { return _columnDefinitions; }
        }

        public Grid(Window owner)
        {
            RowDefinitions = new RowDefinitionCollection();
            ColumnDefinitions = new ColumnDefinitionCollection();
            Children = new List<ContentControl>();
            Handler = GridCreate(owner.Handler);
        }

        public void SetRow(ContentControl control, int row)
        {
            Children.Add(control);
            control.GridRow = GridSetRow(control.Handler, row);
            GridChildrenAdd(Handler, control.Handler);
        }
    }
}
