using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace System.IO
{
    public unsafe static partial class File
    {
        [DllImport("ReadAllBytes")]
        public static extern void ReadAllBytes(string file, out ulong size, out byte* data);

        public static byte[] ReadAllBytes(string file)
        {
            ReadAllBytes(file, out var size, out var data);

            byte[] buffer = new byte[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = data[i];
            }
            
            return buffer;
        }

        public static bool Exists([NotNullWhen(true)] string? path)
        {
            //return FileSystem.FileExists(path);
            return false;
        }

        public static void Delete(string path)
        {
            if (Exists(path))
            {
                //FileSystem.DeleteFile(Path.GetFullPath(path));
            }
        }

        public static string GetDirectory(string file)
        {
            string result = "";

            if (!string.IsNullOrEmpty(file))
            {
                string[] str = file.Split('/');

                for (int i = 0; i < (str.Length - 1); i++)
                {
                    result += str[i] + "/";
                }
            }

            return result;
        }
    }
}
