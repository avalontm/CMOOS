using MOOS;
using MOOS.NET.IPv4;
using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Controls
{
    public class Picture : Widget
    {
        System.Drawing.Image _originalSource;
        System.Drawing.Image _imageSource;

        public System.Drawing.Image ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                if (_originalSource == null)
                {
                    _originalSource = _imageSource.ResizeImage(_imageSource.Width, _imageSource.Height);
                }
            }
        }

        public Picture()
        {
            X = 0;
            Y = 0;
            Width = 100;
            Height = 100;
            _background = null;
        }

        public override void OnLoaded()
        {
            OnParentResize();
            base.OnLoaded();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnDraw()
        {
            base.OnDraw();

            if (IsLoaded && IsVisible)
            {
                if (ImageSource != null)
                {
                    Framebuffer.Graphics.DrawImage(X, Y, ImageSource);
                }
            }
        }

        void OnParentResize()
        {
            if (ImageSource != null)
            {
                if (Parent != null)
                {
                    int prevWidth = Width;
                    int prevHeight = Height;

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

                    int newWidth = (Height / prevHeight) * Width;
                    int newHeight = (Width / prevWidth) * Height;

                    OnResize(newWidth, newHeight);
                }
            }
        }

        internal void OnResize(int w, int h)
        {
            ImageSource = _originalSource.ResizeImage(w, h);
        }

        public override void OnResize()
        {
            base.OnResize();
            OnParentResize();
        }
    }
}
