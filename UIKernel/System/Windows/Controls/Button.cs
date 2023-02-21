#if HasGUI
using MOOS;
using System;
using System.Drawing;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;

namespace System.Windows.Controls
{
    public class Button : Widget
    {
        public string Text { set; get; }
        public Binding Command { set; get; }
        public static object CommandProperty { get;  set; }
        public object CommandParameter { get; set; }
        public bool UseCircle;

        public Button()
        {
            X = 0;
            Y = 0;
            Width = 300;
            Height = 42;
            _background = new Brush(0xFF111111);
            UseHighlight = true;
            CommandParameter = string.Empty;
        }

        public override void OnLoaded()
        {
            base.OnLoaded();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (IsVisible)
            {
                if (WindowManager.FocusWindow == this.Window)
                {
                    if (MouseEnter)
                    {
                        if (IsFocus)
                        {
                            if (Control.Clicked)
                            {
                                if (Command != null && Command.Source != null)
                                {
                                    Command.Source.Execute.Invoke(CommandParameter);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool IsUnderMouse()
        {
            if (Control.MousePosition.X > X && Control.MousePosition.X < X + Width && Control.MousePosition.Y > Y && Control.MousePosition.Y < Y + Height) return true;
            return false;
        }

        public override void OnDraw()
        {
            base.OnDraw();

            if (IsVisible)
            {
                if (Background != null)
                {
                    Framebuffer.Graphics.FillRectangle(X, Y, Width, Height, Background.Value);
                }

                if (!string.IsNullOrEmpty(Text))
                {
                    WindowManager.font.DrawString(X + (Width / 2) - ((WindowManager.font.MeasureString(Text)) / 2) - 1, (Y + (Height / 2)) - (WindowManager.font.FontSize / 2) + 2, Text, Foreground.Value);
                }

                if (BorderBrush != null)
                {
                    DrawBorder();
                }
            }
        }

        public void SetBinding(object commandProperty, Binding binding)
        {
            Command = binding;
            CommandParameter = commandProperty;
        }
    }
}
#endif