using Moos.Framework.Graphics;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace System.Windows
{
    public abstract class Widget
    {
        internal Cursor _cursor;
        internal Color _background;
        internal Color _foreground;

        public string Name { set; get; }
        public int X { set; get; }
        public int Y { set; get; }
        public int Width { set; get; }
        public int Height { set; get; }
        public bool MouseEnter { set; get; }

        internal bool IsVisible { set; get; }
        internal bool IsLoaded { set; get; }

        public Color Background
        {
            set
            {
                _background = value;
            }
            get { return _background; }
        }

        public Cursor Cursor
        {
            set
            {
                _cursor = value;
            }
            get { return _cursor; }
        }

        public Color Foreground
        {
            set
            {
                _foreground = value;
            }
            get { return _foreground; }
        }

        public Color BorderBrush { set; get; }
        public Thickness BorderThickness { set; get; }


        public Widget() : base()
        {
            _background = Color.White;
            _foreground = Color.Black;

            BorderBrush = Color.FromArgb(0xFFCDC7C2);

            BorderThickness = new Thickness(1);

            Cursor = new Cursor(CursorState.None);
            IsVisible = true;
        }

        public virtual void OnLoaded()
        {
            IsLoaded = true;
        }

        public virtual void OnUnloaded()
        {
            IsVisible = false;
            IsLoaded = false;
            MouseEnter = false;
        }

        public virtual void OnResize()
        {

        }

        public virtual void OnDraw()
        {
            if (IsLoaded && IsVisible)
            {

            }
        }

        public virtual void OnUpdate()
        {
            if (IsLoaded && IsVisible)
            {
               
            }

        }

        public void DrawBorder()
        {
            if (IsLoaded && IsVisible)
            {
                GDI.DrawRectangle(X - BorderThickness.Left, Y - BorderThickness.Top, Width + BorderThickness.Right, Height + BorderThickness.Bottom, BorderBrush.ARGB);
            }
        }

    }
}
