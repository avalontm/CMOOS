using Internal.Runtime.CompilerServices;
using MOOS.Graph;
using MOOS.Misc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MOOS.Api
{
    internal static unsafe class GDI
    {
        public static unsafe void* HandleSystemCall(string name)
        {
            switch (name)
            {
                case "DrawRectangle":
                    return (delegate*<int, int, int, int , uint, void>)&API_DrawRectangle;
                case "AFillRectangle":
                    return (delegate*<int, int, int, int, uint, void>)&API_AFillRectangle;
                case "FillRectangle":
                    return (delegate*<int, int, int, int, uint, void>)&API_FillRectangle;
                case "DrawPoint":
                    return (delegate*<int, int, uint, bool, void>)&API_DrawPoint;
                case "DrawLine":
                    return (delegate*<int, int, int, int, uint, void>)&API_DrawLine;
                case "DrawImage":
                    return (delegate*<int, int, IntPtr, bool, void>)&API_DrawImage;
                case "DrawClear":
                    return (delegate*<void>)&API_DrawClear;
                case "DrawUpdate":
                    return (delegate*<void>)&API_DrawUpdate;
            }

            return null;
        }


        public static void API_DrawImage(int x, int y, IntPtr handler, bool alpha)
        {
            if (Framebuffer.TripleBuffered)
            {
                Image image = Unsafe.As<IntPtr, Image>(ref handler);
                Framebuffer.Graphics.DrawImage(x, y, image, alpha);
            }
        }

        public static void API_DrawRectangle(int x, int y, int wight, int height, uint color)
        {
            Framebuffer.Graphics.DrawRectangle(x, y, wight, height, color);
        }

        public static void API_AFillRectangle(int x, int y, int wight, int height, uint color)
        {
            Framebuffer.Graphics.AFillRectangle(x, y, wight, height, color);
        }

        public static void API_FillRectangle(int x, int y, int wight, int height, uint color)
        {
            Framebuffer.Graphics.FillRectangle(x, y, wight, height, color);
        }

        public static void API_DrawPoint(int x, int y, uint color, bool alpha)
        {
            Framebuffer.Graphics.DrawPoint(x, y, color, alpha);
        }

        public static void API_DrawLine(int x0, int y0, int x1, int y1, uint color)
        {
            Framebuffer.Graphics.DrawLine(x0, y0, x1, y1, color);
        }

        public static void API_DrawClear()
        {
            if (Framebuffer.TripleBuffered)
            {
                Framebuffer.Graphics.Clear(0x0);
            }
        }

        public static void API_DrawUpdate()
        {
            if (Framebuffer.TripleBuffered)
            {
                Framebuffer.Update();
            }
        }
    }
}
