using Moos.Framework.Fonts;
using Moos.Framework.Graphics;
using System.Drawing;
using System.Windows.Forms;

namespace System.Windows.Controls
{
    public class Button : Widget
    {
        public string Text { set; get; }

        public Button() : base()
        {
            X = 0;
            Y = 0;
            Width = 300;
            Height = 42;
            Background = Color.FromArgb(255, 212, 212, 212);
            Foreground = Color.Black;
            BorderBrush = Color.FromArgb(0xFFCCCCCC);

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
                
            }
        }

        public bool IsUnderMouse()
        {
            if (Mouse.Position.X > X && Mouse.Position.X < X + Width && Mouse.Position.Y > Y && Mouse.Position.Y < Y + Height) return true;
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

                if (BorderBrush != null)
                {
                    DrawBorder();
                }
            }
        }

    }
}