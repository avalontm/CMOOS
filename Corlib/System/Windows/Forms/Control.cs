using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms
{
    public class Mouse
    {
        public static Point Position = new Point(0,0);
        public static Point OffSet = new Point();
        public static MouseButtons Buttons = MouseButtons.None;
        static MouseButtons lastButtons = MouseButtons.None;
        public static bool Clicked { private set; get; }

        public static void Update()
        {
            if (lastButtons == MouseButtons.None && Buttons == MouseButtons.Left)
            {
                Clicked = true;
            }
            else
            {
                Clicked = false;
            }
            lastButtons = Buttons;
        }
    }
}
