/*************************************************/
/*          MOOS API CONTROL WINDOW              */
/************************************************/
using System.Windows;

namespace System.Apis
{
    public unsafe static class ApiWindow
    {
        public static unsafe void* HandleSystemCall(string name)
        {
            switch (name)
            {
                case "CreateWindow":
                    return (delegate*<int, int, int, int, string, IntPtr>)&API_CreateWindow;
            }

            return null;
        }

        public static IntPtr API_CreateWindow(int X, int Y, int Width, int Height, string Title)
        {
            PortableApp papp = new PortableApp(X, Y, Width, Height, Title);
            papp.ShowDialog();
            return papp;
        }
    }
}
