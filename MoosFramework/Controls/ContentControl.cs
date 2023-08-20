using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace Moos.Framework.Controls
{
    public abstract partial class ContentControl : Layout, IView
    {
        
        [DllImport("ControlBackground")]
        public static extern int ControlBackground(IntPtr control, uint color);
        [DllImport("ControlForeground")]
        public static extern int ControlForeground(IntPtr control, uint color);
        [DllImport("GridSetRow")]
        public static extern int GridSetRow(IntPtr control, int row);
        [DllImport("GridSetColumn")]
        public static extern int GridSetColumn(IntPtr control, int column);

        public string Name { get; set; }

        public IntPtr Handler { get; internal set; }
        public Window OwnerWindow { set; get; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Thickness Margin { get; set; }
        public LayoutOptions HorizontalOptions { set; get; }
        public LayoutOptions VerticalOptions { set; get; }
        public Color Background { get; set; }
        public bool IsVisisble { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public ContentControl()
        {
            IsVisisble = true;
        }

        public virtual void OnLoaded()
        {
            Row = GridSetRow(Handler, Row);
            Column = GridSetColumn(Handler, Column);
        }
    }
}
