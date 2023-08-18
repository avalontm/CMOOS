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

        public static Image FromFile(string file)
        {
            string filePath = Application.Current.ExecutablePath + "Resources/" + file;
            IntPtr handler = LoadPNG(filePath);
            return Unsafe.As<IntPtr, Image>(ref handler);
        }
    }
}
