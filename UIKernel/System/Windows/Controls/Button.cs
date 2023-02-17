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

        bool clicked;

        public Button()
        {
            X = 0;
            Y = 0;
            Width = 300;
            Height = 42;
            Background = new Brush(0xFF111111);
            UseHighlight = true;
            CommandParameter = string.Empty;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (MouseEnter)
            {
                this.onMouseFocus();

                if (IsFocus)
                {
                    if (Control.MouseButtons == MouseButtons.Left)
                    {
                        if (Command != null && Command.Source != null)
                        {
                            if (!clicked)
                            {
                                clicked = true;

                                Command.Source.Execute.Invoke(CommandParameter);
                            }
                        }
                    }
                }
            }
            else
            {
                this.onMouseLostFocus();
            }

            if (Control.MouseButtons == MouseButtons.None)
            {
                clicked = false;
            }
        }

        public override void OnDraw()
        {
            base.OnDraw();

            if (UseCircle)
            {
                Framebuffer.Graphics.FillCircle(X, Y, Height, Background.Value);
            }
            else
            {
                Framebuffer.Graphics.FillRectangle(X, Y, Width, Height, Background.Value);
            }
         
            if (!string.IsNullOrEmpty(Text))
            {
                WindowManager.font.DrawString(X + (Width / 2) - ((WindowManager.font.MeasureString(Text)) / 2) - 1,(Y + (Height / 2) ) - (WindowManager.font.FontSize/2) + 2 , Text, Foreground.Value);
            }

            if (BorderBrush != null)
            {
                if (!UseCircle)
                {
                    DrawBorder();
                }
            }
        }

        public void SetBinding(object commandProperty, Binding binding)
        {
            Command = binding;
        }
    }
}
#endif