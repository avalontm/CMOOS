using Moos.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Explorer.Managers
{
    public class CursorManager
    {
        [DllImport("GetMouseButtons")]
        public static extern int GetMouseButtons();


        public static Point MousePosition = new Point();
        public static Point MouseOffSet = new Point();
        static Image CursorNormal { set; get; }
        static Image CursorMoving { set; get; }
        static Image CursorTextSelect { set; get; }
        static Image CursorHand { set; get; }
        static Image CursorHorizontal { set; get; }
        static Image CursorVertical { set; get; }
        public static Cursor State { set; get; }
       // public static Widget FocusControl { set; get; }

        public static void Initialize()
        {
            //Sized width to 512
            //https://gitlab.com/Enthymeme/hackneyed-x11-cursors/-/blob/master/theme/right-handed-white.svg
            CursorNormal = PNG.FromFile("sys/media/Cursor.png");
            CursorMoving = PNG.FromFile("sys/media/Grab.png");
            CursorTextSelect = PNG.FromFile("sys/media/CursorTextSelect.png");
            CursorHand = PNG.FromFile("sys/media/CursorHand.png");
            CursorHorizontal = PNG.FromFile("sys/media/CursorHorizontalArrow.png");
            CursorVertical = PNG.FromFile("sys/media/CursorVerticalArrow.png");

            State = new Cursor(CursorState.Normal);
        }

        public static Image GetCursor
        {
            get
            {
                switch (State.Value)
                {
                    case CursorState.Normal:
                        return CursorNormal;
                    case CursorState.Grab:
                        return CursorMoving;
                    case CursorState.TextSelect:
                        return CursorTextSelect;
                    case CursorState.Hand:
                        return CursorHand;
                    case CursorState.Horizontal:
                        return CursorHorizontal;
                    case CursorState.Vertical:
                        return CursorVertical;
                    default:
                        return CursorNormal;
                }
            }
        }

        public static void Update()
        {
            Mouse.Buttons = (MouseButtons)GetMouseButtons();
            Mouse.Update();
            MousePosition = new Point(MoosNative.GetMouseX(), MoosNative.GetMouseY());
           

            /*
            if (WindowManager.HasWindowFocusResizing)
            {
                Control.MouseOffSet.X = -(CursorHorizontal.Width / 2);
                Control.MouseOffSet.Y = -(CursorHorizontal.Height / 2);
                return;
            }

            if (WindowManager.HasWindowMoving)
            {
                Control.MouseOffSet.X = 0;
                Control.MouseOffSet.Y = 0;
                State.Value = CursorState.Grab;
                return;
            }

            if (FocusControl != null)
            {
                if (FocusControl.MouseEnter)
                {
                    if (FocusControl.Cursor.Value != CursorState.None)
                    {
                        Control.MouseOffSet.X = 0;
                        Control.MouseOffSet.Y = 0;
                        State.Value = FocusControl.Cursor.Value;
                        return;
                    }
                }
            }
            */

            MouseOffSet.X = 0;
            MouseOffSet.Y = 0;
            State.Value = CursorState.Normal;
        }

        public static void Draw()
        {
            //Mouse
            GDI.DrawImage(CursorManager.MousePosition.X + CursorManager.MouseOffSet.X, CursorManager.MousePosition.Y + CursorManager.MouseOffSet.Y, CursorManager.GetCursor, true);
        }
    }
}
