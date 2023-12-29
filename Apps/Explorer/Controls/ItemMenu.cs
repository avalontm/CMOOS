using Moos.Framework.Fonts;
using Moos.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Explorer.Controls
{
    public class ItemMenu : Widget
    {
        public Image Icon { get; set; }
        public string Text { get; set; }
        public EventHandler<object> Clicked { set; get; }
        private bool _selected;

        public ItemMenu()
        {
            Background = Color.FromArgb(0xFF000080);
        }

        public override void OnLoaded()
        {
            base.OnLoaded();
            IsVisible = true;
        }

        public override void OnUnloaded()
        {
            base.OnUnloaded();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (IsLoaded && IsVisible)
            {
                if (IsUnderMouse())
                {
                    _selected = true;
                    if (Mouse.Clicked)
                    {
                        Clicked?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    _selected = false;
                }
            }
        }


        public override void OnDraw()
        {
            base.OnDraw();

            if (IsLoaded && IsVisible)
            {
                if (Background != null)
                {
                    if (_selected)
                    {
                        GDI.FillRectangle(X, Y, Width, Height, Background.ARGB);
                    }
                }

                if(Icon != null)
                {
                    GDI.DrawImage(X + 5, (Y + (Icon.Height/3)), Icon, true);
                }

                if (!string.IsNullOrEmpty(Text))
                {
                    uint color = 0xFF000000;

                    if (_selected)
                    {
                        color = 0xFFFFFFFF;
                    }

                    FontManager.font.DrawString(X + Icon.Width + 20, Y + Height / 2 - FontManager.font.Size / 2 + 2, Text, color);
                }

            }
        }

        public bool IsUnderMouse()
        {
            if (Mouse.Position.X > X && Mouse.Position.X < (X + Width) && Mouse.Position.Y > Y && Mouse.Position.Y < (Y + Height)) return true;
            return false;
        }
    }
}
