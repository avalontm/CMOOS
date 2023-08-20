
using MOOS;
using MOOS.Driver;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Windows.Controls
{
    public abstract class Widget
    {
        internal Cursor _cursor;
        internal Brush _background;
        internal Brush _foreground;
        internal Brush _highlight_foreground;
        internal Brush _old_foreground;

        internal Brush _old_background;
        internal Brush _highlight_background;
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
        internal bool IsLoaded {set;get;}

        public Brush Background
        {
            set
            {
                _background = value;

                //Default
                if (_old_background == null)
                {
                    _old_background = new Brush(_background.Value);

                    onSetHighLight();
                }
            }
            get { return _background; }
        }

        internal void onSetHighLight()
        {
            Color color = Color.FromArgb(_old_background.Value);

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

            _highlight_background = new Brush(color);
            color.Dispose();
        }

        public Brush HighlightBackground
        {
            set
            {
                _highlight_background = value;
            }
            get { return _highlight_background; }
        }

        public Brush HighlightForeground
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

        public Brush Foreground
        {
            set
            {
                _foreground = value;

                //Default
                if (_old_foreground == null)
                {
                    _old_foreground = new Brush(_foreground.Value);
                }
            }
            get { return _foreground; }
        }

        public Brush BorderBrush { set; get; }
        public Thickness BorderThickness { set; get; }
        public Brush ColorFocus { set; get; }
        public Brush ColorNormal { set; get; }
        public int GridRow { get; set; }
        public int GridColumn { get; set; }
        public FontFamily FontFamily { get; set; }

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

            _background = Brushes.White;
            _foreground = Brushes.Black;
            _highlight_background = Brushes.Black;
            _highlight_foreground = Brushes.Black;

            BorderBrush = new Brush(0xFFCDC7C2);
            ColorNormal = new Brush(0xFF111111);
            ColorFocus = new Brush(0xFF141414);
            BorderThickness = new Thickness(1);
            Margin = new Thickness();
            Padding = new Thickness();
            FontFamily = new FontFamily();
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
            CursorManager.FocusControl = null;
            WindowManager.HasWindowMoving = false;
            MouseEnter = false;
        }

        public virtual void OnResize()
        {

        }

        public virtual void OnDraw()
        {
            if (IsVisible)
            {
                if (this.Parent == null)
                {
                    return;
                }

                // Position & margin
                if (Pos == null)
                {
                    X = this.Parent.X + this.Margin.Left;
                    Y = this.Parent.Y + this.Margin.Top;
                    Width = this.Parent.Width - (this.Margin.Right * 2);
                    Height = this.Parent.Height - (this.Margin.Bottom * 2);
                }
                else
                {
                    X = this.Pos.Position.X + this.Margin.Left;
                    Y = this.Pos.Position.Y + this.Margin.Top;
                    Width = this.Pos.Position.Width - (this.Margin.Right * 2);
                    Height = this.Pos.Position.Height - (this.Margin.Bottom * 2);
                }
            }
        }

        public virtual void OnUpdate()
        {
            if (IsVisible)
            {
                if (Control.MouseButtons == MouseButtons.Left)
                {
                    if (Control.MousePosition.X > X && Control.MousePosition.X < (X + Width) && Control.MousePosition.Y > Y && Control.MousePosition.Y < (Y + Height))
                    {
                        WindowManager.FocusControl = this;
                        _isFocus = true;
                    }
                }

                if (Control.MousePosition.X > X && Control.MousePosition.X < (X + Width) && Control.MousePosition.Y > Y && Control.MousePosition.Y < (Y + Height))
                {
                    CursorManager.FocusControl = this;
                    MouseEnter = true;
                    WindowManager.HasWindowControl = false;
                }
                else
                {
                    MouseEnter = false;
                    if (Control.MouseButtons == MouseButtons.Left)
                    {
                        _isFocus = false;
                    }
                    WindowManager.HasWindowControl = false;
                }

                if (WindowManager.FocusWindow == this.Window)
                {
                    if (UseHighlight)
                    {
                        if (MouseEnter)
                        {
                            if (_background != null)
                            {
                                if (_highlight_background != null)
                                {
                                    Background.Value = _highlight_background.Value;
                                }
                            }
                        }
                        else
                        {
                            if (_background != null)
                            {
                                if (_old_background != null)
                                {
                                    Background.Value = _old_background.Value;
                                }
                            }
                        }
                    }
                }
            }

        }

        public void onSetParent(Widget parent, Position pos)
        {
            this.Parent = parent;
            this.Pos = new GridCollection(pos, GridRow,  GridColumn);
            X =  Pos.Position.X;
            Y = Pos.Position.Y;
            Width = Pos.Position.Width;
            Height = Pos.Position.Height;
        }

        public void DrawBorder()
        {
            Framebuffer.Graphics.DrawRectangle((X - BorderThickness.Left), (Y - BorderThickness.Top), (Width + BorderThickness.Right), (Height + BorderThickness.Bottom), BorderBrush.Value);
        }

        bool isFocus()
        {
            if (IsVisible)
            {
                if (WindowManager.FocusWindow != null && WindowManager.FocusControl != null)
                {
                    if (WindowManager.FocusControl == this && _isFocus)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
