using MOOS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Desktops.Controls
{
    public class DesktopBarItem : DesktopControl
    {
        public Brush FocusBackground { set; get; }
        bool _isFocus;
        public DesktopBarItem()
        {
            FocusBackground = new Brush(Color.ToArgb(50, 100, 150, 240));
            Width = 32;
            Height = 32;
            HorizontalAlignment = HorizontalAlignment.Left;
        }

        public override void Update()
        {
            base.Update();

            int minWidth = 10;

            if (!string.IsNullOrEmpty(Content))
            {
                minWidth = (WindowManager.font.MeasureString(Content) + 10);
            }

            if (Width == 0 || Width < minWidth)
            {
                Width = minWidth;
            }


            int _x = X;
            int _y = Y;

            if (HorizontalAlignment == HorizontalAlignment.Right)
            {
                _x = (Framebuffer.Graphics.Width - X) - Width;
            }

            if (!WindowManager.HasWindowMoving && Control.MousePosition.X > _x && Control.MousePosition.X < (_x + Width) && Control.MousePosition.Y > _y && Control.MousePosition.Y < (_y + Height))
            {
                _isFocus = true;
                if (Control.MouseButtons == MouseButtons.Left)
                {
                    if (Command != null && Command != null)
                    {
                        if (Control.Clicked)
                        {
                            Command.Execute.Invoke(CommandParameter);
                        }
                    }
                }
            }
            else
            {
                _isFocus = false;
            }

        }

        public override void Draw()
        {
            base.Draw();

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    if (!string.IsNullOrEmpty(Content))
                    {
                        WindowManager.font.DrawString(X + ((Width / 2) - (WindowManager.font.MeasureString(Content) / 2)), Y + (WindowManager.font.FontSize / 2), Content, Foreground.Value);
                    }
                    if (Icon != null)
                    {
                        if (_isFocus)
                        {
                            Framebuffer.Graphics.AFillRectangle(X, Y, Width, Height, FocusBackground.Value);
                        }

                        Framebuffer.Graphics.DrawImage(X + ((Width / 2) - (Icon.Width / 2)), Y+5, Icon);
                    }
                    break;
                case HorizontalAlignment.Center:
                    //Nothing
                    break;
                case HorizontalAlignment.Right:
                    if (!string.IsNullOrEmpty(Content))
                    {
                        WindowManager.font.DrawString((Framebuffer.Graphics.Width - X) - ((Width / 2) + (WindowManager.font.MeasureString(Content) / 2)), Y + (WindowManager.font.FontSize / 2), Content, Foreground.Value);
                    }
                    if (Icon != null)
                    {
                        if (_isFocus)
                        {
                            Framebuffer.Graphics.AFillRectangle((Framebuffer.Graphics.Width - X) - Width, Y, Width, Height, FocusBackground.Value);
                        }

                        Framebuffer.Graphics.DrawImage((Framebuffer.Graphics.Width - X) - ((Width / 2) + (Icon.Width / 2)), Y + 5, Icon);
                    }
                    break;
            }
         
        }
    }
}
