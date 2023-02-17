
using MOOS;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Windows.Controls
{
    public class Position
    {
        public int X { set; get; }
        public int Y { set; get; }
        public int Width { set; get; }
        public int Height { set; get; }
    }

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

        public Brush Background
        {
            set
            {
                _background = value;

                //Default
                if (_old_background == null)
                {
                    _old_background = new Brush(_background.Value);
                }

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
            get { return _background; }
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

        Widget _parent;
        public Widget Parent
        {
            set
            {
                _parent = value;
                onSetParent(_parent);
            }
            get
            {
                return _parent;
            }
        }

        GridCollection _pos;
        public GridCollection Pos
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
        }

        public virtual void OnDraw()
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

        public virtual void OnUpdate()
        {
            if (Control.MouseButtons == MouseButtons.Left)
            {
                if (!WindowManager.HasWindowMoving && Control.MousePosition.X > X && Control.MousePosition.X < (X + Width) && Control.MousePosition.Y > Y && Control.MousePosition.Y < (Y + Height))
                {
                    WindowManager.FocusControl = this;
                    _isFocus = true;
                }
            }

            if (!WindowManager.HasWindowMoving && Control.MousePosition.X > X && Control.MousePosition.X < (X + Width) && Control.MousePosition.Y > Y && Control.MousePosition.Y < (Y + Height))
            {
                CursorManager.FocusControl = this;
                MouseEnter = true;
            }
            else
            {
                MouseEnter = false;
                if (Control.MouseButtons == MouseButtons.Left)
                {
                    _isFocus = false;
                }
            }

            if (UseHighlight)
            {
                if (MouseEnter)
                {
                    if (Background != null)
                    {
                        _background.Value = HighlightBackground.Value;
                    }

                    if (HighlightBackground != null)
                    {
                        Color color = Color.FromArgb(HighlightBackground.Value);
                        double luminosity = (((0.2126 * color.R) + (0.7152 * color.G) + (0.0722 * color.B)) / 255);

                        if (luminosity >= 0.5) //Light
                        {
                            BorderBrush = Brushes.Black;
                        }
                        else
                        {
                            BorderBrush = Brushes.White;
                        }
                        color.Dispose();
                    }
                }
                else
                {
                    if (Background != null)
                    {
                        _background.Value = _old_background.Value;
                    }
                    if (HighlightBackground != null)
                    {
                        BorderBrush.Value = _old_background.Value;
                    }
                }
            }

        }

        public void onSetParent(Widget parent)
        {
            X = parent.X;
            Y = parent.Y;
            Width = parent.Width;
            Height = parent.Height;
        }

        public void DrawBorder()
        {
            Framebuffer.Graphics.DrawRectangle(X - BorderThickness.Left, Y - BorderThickness.Top , Width  + (int)(BorderThickness.Right), Height+ (int)(BorderThickness.Bottom), BorderBrush.Value);
        }

        public void onMouseFocus()
        {
            // Background.Value = ColorFocus.Value;
        }

        public void onMouseLostFocus()
        {

        }

        bool isFocus()
        {
            if (WindowManager.FocusWindow != null && WindowManager.FocusControl != null)
            {
                if (WindowManager.FocusControl == this && _isFocus)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
