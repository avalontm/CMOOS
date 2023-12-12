using Moos.Framework.Fonts;
using Moos.Framework.Graphics;
using System.Drawing;
using System.Windows.Forms;

namespace System.Windows.Controls
{
    public class Button : Widget
    {
        public string Text { set; get; }
        public EventHandler<object> Clicked { set; get; }

        public Button() : base()
        {
            X = 0;
            Y = 0;
            Width = 300;
            Height = 42;
            Background = Color.FromArgb(0xFFC3C7CB);
            Foreground = Color.Black;
            BorderBrush = Color.FromArgb(0xFFFFFFFF);

            IsVisible = true;
        }

        public override void OnLoaded()
        {
            base.OnLoaded();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (IsLoaded && IsVisible)
            {
                if (IsUnderMouse() && Mouse.Clicked)
                {
                    Clicked?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool IsUnderMouse()
        {
            if (Mouse.Position.X > X && Mouse.Position.X < (X + Width) && Mouse.Position.Y > Y && Mouse.Position.Y < (Y + Height)) return true;
            return false;
        }

        public override void OnDraw()
        {
            base.OnDraw();

            if (IsLoaded && IsVisible)
            {
                if (Background != null)
                {
                    GDI.FillRectangle(X, Y, Width, Height, Background.ARGB);
                }

                if (!string.IsNullOrEmpty(Text))
                {
                    FontManager.font.DrawString(X + Width / 2 - FontManager.font.MeasureString(Text) / 2 - 1, Y + Height / 2 - FontManager.font.Size / 2 + 2, Text, Foreground);
                }

                if (IsUnderMouse() && Mouse.Buttons == MouseButtons.Left)
                {
                    DrawPressedBorder();
                }
                else
                {
                    DrawNormalBorder();
                }
            }
        }

        void DrawNormalBorder()
        {
            if (IsLoaded && IsVisible)
            {
                //white
                GDI.DrawLine(X, Y, X + Width, Y, 0xFFFFFFFF);
                GDI.DrawLine(X, Y, X, Y + Height - 2, 0xFFFFFFFF);
                
                //gray
                GDI.DrawLine(X + Width , Y, X + Width , Y + Height - 2, 0xFF868a8e);
                GDI.DrawLine(X - 2, Y + Height -1, X + Width, Y + Height - 1, 0xFF868a8e);

                //black
                GDI.DrawLine(X + Width +1, Y , X + Width +1, Y + Height -1, 0xFF000000);
                GDI.DrawLine(X-2, Y + Height, X + Width , Y + Height , 0xFF000000);
            }
        }

        void DrawPressedBorder()
        {
            if (IsLoaded && IsVisible)
            {
                //black
                GDI.DrawLine(X, Y, X + Width -1, Y, 0xFF000000);
                GDI.DrawLine(X, Y, X , Y + Height, 0xFF000000);

                //gray
                GDI.DrawLine(X + 1, Y, X + 1, Y + Height - 2, 0xFF868a8e);
                GDI.DrawLine(X + 1, Y , X + 1, Y + Height - 1, 0xFF868a8e);

                //white
                GDI.DrawLine(X, Y + Height, X + Width, Y + Height, 0xFFFFFFFF);
                GDI.DrawLine(X + Width, Y, X + Width, Y + Height - 2, 0xFFFFFFFF);
            }
        }
    }
}