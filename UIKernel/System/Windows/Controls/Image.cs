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
            _background = null;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnDraw()
        {
            base.OnDraw();

            if (ImageSource != null)
            {
                Framebuffer.Graphics.DrawImage(X, Y, ImageSource);
            }
        }

        internal void OnReSize(int w, int h)
        {
            ImageSource = _originalSource.ResizeImage(w, h);
        }
    }
}
