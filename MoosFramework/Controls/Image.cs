using System.Runtime.InteropServices;
using System;
using Internal.Runtime.CompilerHelpers;
using System.Windows;

namespace Moos.Framework.Controls
{
    public class Image : ContentControl
    {
        [DllImport("ImageCreate")]
        public static extern IntPtr ImageCreate();

        [DllImport("ImageSource")]
        public static extern IntPtr ImageSource(IntPtr handler, IntPtr source);

        [DllImport("ImageWidth")]
        public static extern int ImageWidth(IntPtr handler, int width);

        [DllImport("ImageHeight")]
        public static extern int ImageHeight(IntPtr handler, int height);
        [DllImport("ImageMargin")]
        public static extern void ImageMargin(IntPtr handler, int left, int top, int right, int bottom);
        

        Image _source;
        public Image Source 
        {
            get { return _source; }
            set
            {
                _source = value;
                ImageSource(Handler, _source);
            }
        }

        int _height;
        public new int Height
        {
            get { return _height; }
            set
            {
                _height = ImageHeight(Handler, value);
            }
        }

        int _width;
        public new int Width
        {
            get { return _width; }
            set
            {
                _width = ImageWidth(Handler, value);
            }
        }

        Thickness _margin;
        public new Thickness Margin
        {
            get { return _margin; }
            set
            {
                _margin = value;
                ImageMargin(Handler, _margin.Left, _margin.Top, _margin.Right, _margin.Bottom);
            }
        }

        public Image() 
        { 
            Handler = ImageCreate();
        }
    }
}
