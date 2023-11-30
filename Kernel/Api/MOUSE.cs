using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MOOS.Api
{
    internal static unsafe class MOUSE
    {
        public static unsafe void* HandleSystemCall(string name)
        {
            switch (name)
            {
                case "GetMouseX":
                    return (delegate*<uint>)&API_MouseX;
                case "GetMouseY":
                    return (delegate*<uint>)&API_MouseY;

                case "GetMouseButtons":
                    return (delegate*<uint>)&API_MouseButtons;
            }

            return null;
        }

        public static uint API_MouseX() => Mouse.Position.X;
        public static uint API_MouseY() => Mouse.Position.Y;
        public static uint API_MouseButtons() => (uint)Mouse.Buttons;


    }
}
