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


        public RowDefinitionCollection RowDefinitions { set; get; }
        public ColumnDefinitionCollection ColumnDefinitions { set; get; }
        public List<ContentControl> Children { set; get; }

        public Grid(Window owner)
        {
            RowDefinitions = new RowDefinitionCollection();
            ColumnDefinitions = new ColumnDefinitionCollection();
            Children = new List<ContentControl>();
            GridCreate(owner);
        }

    }
}
