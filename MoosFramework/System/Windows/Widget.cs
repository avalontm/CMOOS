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
        internal Color _highlight_foreground;
        internal Color _old_foreground;

        internal Color _old_background;
        internal Color _highlight_background;
        public string Name { set; get; }
        public int X { set; get; }
        public int Y { set; get; }
        public int Width { set; get; }
        public int Height { set; get; }
        public bool MouseEnter { set; get; }
        public Thickness Margin { set; get; }
        public Thickness Padding { set; get; }
        public bool UseHighlight { set; get; }
        public HorizontalAlignment HorizontalOptions { set; get; }
        public VerticalAlignment VerticalOptions { set; get; }

        internal bool IsVisible { set; get; }
        internal bool IsLoaded { set; get; }

        public Color Background
        {
            set
            {
                _background = value;

                //Default
                if (_old_background == null)
                {
                    _old_background = _background;

                    onSetHighLight();
                }
            }
            get { return _background; }
        }

        internal void onSetHighLight()
        {
            Color color = _old_background;

            color.R = (byte)(color.R + 5);
            color.G = (byte)(color.G + 5);
            color.B = (byte)(color.B + 5);

            if (color.R > 255)
            {
                color.R = 255;
            }

            if (color.G > 255)
            {
                color.G = 255;
            }

            if (color.B > 255)
            {
                color.B = 255;
            }

            HighlightBackground = color;
            color.Dispose();
        }

        public Color HighlightBackground
        {
            set
            {
                _highlight_background = value;
            }
            get { return _highlight_background; }
        }

        public Color HighlightForeground
        {
            set
            {
                _highlight_foreground = value;
            }
            get { return _highlight_foreground; }
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

                //Default
                if (_old_foreground == null)
                {
                    _old_foreground = _foreground;
                }
            }
            get { return _foreground; }
        }

        public Color BorderBrush { set; get; }
        public Thickness BorderThickness { set; get; }
        public Color ColorFocus { set; get; }
        public Color ColorNormal { set; get; }
        public int GridRow { get; set; }
        public int GridColumn { get; set; }
        //public FontFamily FontFamily { get; set; }
        /*
        Window _window;
        internal Window Window
        {
            set
            {
                _window = value;
            }
            get
            {
                return _window;
            }
        }
        */
        internal Widget Parent { set; get; }

        GridCollection _pos;
        internal GridCollection Pos
        {
            set
            {
                _pos = value;
            }
            get
            {
                return _pos;
            }
        }

        public int GridColumnSpan { get; set; }
        public int GridRowSpan { get; set; }
        public bool IsFocus { get { return isFocus(); } }

        bool _isFocus;

        public Widget() : base()
        {
            Parent = this;
            UseHighlight = false;

            _background = Color.White;
            _foreground = Color.Black;
            _highlight_background = Color.Black;
            _highlight_foreground = Color.Black;

            BorderBrush = Color.FromArgb(0xFFCDC7C2);
            ColorNormal = Color.FromArgb(0xFF111111);
            ColorFocus = Color.FromArgb(0xFF141414);
            BorderThickness = new Thickness(1);
            Margin = new Thickness();
            Padding = new Thickness();
            //FontFamily = new FontFamily();
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
            //CursorManager.FocusControl = null;
            //WindowManager.HasWindowMoving = false;
            MouseEnter = false;
        }

        public virtual void OnResize()
        {

        }

        public virtual void OnDraw()
        {
            if (IsVisible)
            {
                if (Parent == null)
                {
                    return;
                }

                // Position & margin
                if (Pos == null)
                {
                    X = Parent.X + Margin.Left;
                    Y = Parent.Y + Margin.Top;
                    Width = Parent.Width - Margin.Right * 2;
                    Height = Parent.Height - Margin.Bottom * 2;
                }
                else
                {
                    X = Pos.Position.X + Margin.Left;
                    Y = Pos.Position.Y + Margin.Top;
                    Width = Pos.Position.Width - Margin.Right * 2;
                    Height = Pos.Position.Height - Margin.Bottom * 2;
                }
            }
        }

        public virtual void OnUpdate()
        {
            if (IsVisible)
            {
                if (Control.MouseButtons == MouseButtons.Left)
                {
                    if (Control.MousePosition.X > X && Control.MousePosition.X < X + Width && Control.MousePosition.Y > Y && Control.MousePosition.Y < Y + Height)
                    {
                       // WindowManager.FocusControl = this;
                        _isFocus = true;
                    }
                }

                if (Control.MousePosition.X > X && Control.MousePosition.X < X + Width && Control.MousePosition.Y > Y && Control.MousePosition.Y < Y + Height)
                {
                    //CursorManager.FocusControl = this;
                    //MouseEnter = true;
                   // WindowManager.HasWindowControl = false;
                }
                else
                {
                    MouseEnter = false;
                    if (Control.MouseButtons == MouseButtons.Left)
                    {
                        _isFocus = false;
                    }
                    //WindowManager.HasWindowControl = false;
                }
                /*
                if (WindowManager.FocusWindow == Window)
                {
                    if (UseHighlight)
                    {
                        if (MouseEnter)
                        {
                            if (_background != null)
                            {
                                if (_highlight_background != null)
                                {
                                    Background = _highlight_background;
                                }
                            }
                        }
                        else
                        {
                            if (_background != null)
                            {
                                if (_old_background != null)
                                {
                                    Background = _old_background;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (_background != null)
                    {
                        if (_old_background != null)
                        {
                            Background = _old_background;
                        }
                    }
                }
                */
            }

        }

        public void onSetParent(Widget parent, Position pos)
        {
            Parent = parent;
            Pos = new GridCollection(pos, GridRow, GridColumn);
            X = pos.X;
            Y = pos.Y;
            Width = pos.Width;
            Height = pos.Height;
        }

        public void DrawBorder()
        {
            GDI.DrawRectangle(X - BorderThickness.Left, Y - BorderThickness.Top, Width + BorderThickness.Right, Height + BorderThickness.Bottom, BorderBrush.ARGB);
        }

        bool isFocus()
        {
            if (IsVisible)
            {
                /*
                if (WindowManager.FocusWindow != null && WindowManager.FocusControl != null)
                {
                    if (WindowManager.FocusControl == this && _isFocus)
                    {
                        return true;
                    }
                }
                */
            }
            return false;
        }
    }
}
