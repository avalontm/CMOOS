using System.Runtime.InteropServices;
using System;
using Internal.Runtime.CompilerServices;
using System.IO;
using Moos.Framework.IO;

namespace Moos.Framework.Controls
{
    public class ImageSource
    {
        [DllImport("LoadPNG")]
        public static extern IntPtr LoadPNG(string file);

        public static System.Drawing.Image FromFile(string file)
        {
            //string filePath = Application.ExecutablePath + "/Content/Resources/" + file;
            string filePath = "home/moos/Desktop/appdemo.muepkg" + "/Content/Resources/" + file;
            IntPtr handler = LoadPNG(filePath);
            return Unsafe.As<IntPtr, System.Drawing.Image>(ref handler);
        }
    }
}
