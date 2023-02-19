using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.IO
{
    public static partial class File
    {
        public static bool Exists([NotNullWhen(true)] string? path)
        {
            //return FileSystem.FileExists(path);
            return false;
        }

        public static void Delete(string path)
        {
            if (string.IsNullOrEmpty(path))
            { 

            }

            //FileSystem.DeleteFile(Path.GetFullPath(path));
        }
    }
}
