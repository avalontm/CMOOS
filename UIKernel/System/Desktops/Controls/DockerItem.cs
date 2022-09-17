using MOOS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace System.Desktops.Controls
{
    public class DockerItem : DesktopControl
    {
        public string Name { set; get; }
        Image _icon;
        public Image Icon
        {
            set
            {
                _icon = value;

                if (IconNormal == null)
                {
                    IconNormal = _icon.ResizeImage(_icon.Width, _icon.Height);
                }
                if (IconZoom == null)
                {
                    IconZoom = _icon.ResizeImage((int)(_icon.Width * zoom), (int)(_icon.Height * zoom));
                }
            }
            get { return _icon; }
        }

        Image IconNormal;
        Image IconZoom;

        bool _isFocus;

        double zoom = 1.1;
        bool isZoom;

        public DockerItem()
        {
            Background = Brushes.White;
            Width = 48;
            Height = 48;
        }

        public override void Update()
        {
            base.Update();

            if (!WindowManager.HasWindowMoving && Control.MousePosition.X > (X - (Width/2)) && Control.MousePosition.X < (X + Width) && Control.MousePosition.Y > Y && Control.MousePosition.Y < (Y + Height))
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

            if (_isFocus)
            {
                if (!isZoom)
                {
                    isZoom = true;
                    Icon = IconZoom;
                }
            }
            else
            {
                if (isZoom)
                {
                    isZoom = false;
                    Icon = IconNormal;
                }
            }

            if (Icon != null)
            {
                if (_isFocus)
                {
                    Framebuffer.Graphics.DrawImage((X - (Icon.Width / 3)), ((Y + (Height / 2)) - ((Icon.Height / 2) + 5)), Icon, true);
                }
                else
                {
                    Framebuffer.Graphics.DrawImage((X - (Icon.Width / 3)), ((Y + (Height / 2)) - (Icon.Height / 2)), Icon, true);
                }
            }
        }
    }
}
