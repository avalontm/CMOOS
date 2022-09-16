using MOOS;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace System.Desktops.Controls
{
    public class DesktopBarItem : DesktopControl
    {
        public DesktopBarItem()
        {
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

            if (Control.MouseButtons == MouseButtons.Left)
            {
                if (!WindowManager.HasWindowMoving && Control.MousePosition.X > X && Control.MousePosition.X < (X + Width) && Control.MousePosition.Y > Y && Control.MousePosition.Y < (Y + Height))
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
                        Framebuffer.Graphics.DrawImage((Framebuffer.Graphics.Width - X) - ((Width / 2) + (Icon.Width / 2)), Y + 5, Icon);
                    }
                    break;
            }
         
        }
    }
}
