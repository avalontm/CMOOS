using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Moos.Framework.Controls
{
    [ContentProperty(nameof(Children))]
    public partial class IGrid : ContentControl
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

        public IGrid()
        {
            RowDefinitions = new RowDefinitionCollection();
            ColumnDefinitions = new ColumnDefinitionCollection();
        }

        public override void OnLoaded()
        {
            base.OnLoaded();
            Handler = GridCreate(Application.Current.MainWindow.Handler);

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].OnLoaded();
                if (Children[i].Handler != IntPtr.Zero)
                {
                    IntPtr cHandler = GridChildrenAdd(Handler, Children[i].Handler);

                    if (cHandler != IntPtr.Zero)
                    {
                        Debug.WriteLine($"[Children] {cHandler} == {Children[i].Handler}");
                    }
                }
            }
        }

        public static void SetRow(ContentControl control, int row)
        {
           // this.Add(control);
            control.GridRow = GridSetRow(control.Handler, row);
           // GridChildrenAdd(Handler, control.Handler);
        }
    }
}
