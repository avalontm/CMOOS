#define ASCII

using MOOS.Driver;
using System;
using System.Drawing;

namespace MOOS
{
    public static unsafe class Console
    {
        public static int Width { get => Framebuffer.Width / 8; }
        public static int Height { get => Framebuffer.Height / 16; }

        public static int CursorX = 0;
        public static int CursorY = 0;

        public delegate void OnWriteHandler(char chr);
        public static event OnWriteHandler OnWrite;

        private static uint[] ColorsFB;

        public static ConsoleColor ForegroundColor;
        public static ConsoleColor BackgroundColor;

        internal static void Setup()
        {
            OnWrite = null;

            ColorsFB = new uint[16]
            {
                Color.Black.ToArgb(),
                Color.BlueDark.ToArgb(),
                Color.Green.ToArgb(),
                Color.Cyan.ToArgb(),
                Color.Red.ToArgb(),
                Color.Purple.ToArgb(),
                Color.Brown.ToArgb(),
                Color.GrayLight.ToArgb(),
                Color.DarkGray.ToArgb(),
                Color.LightBlue.ToArgb(),
                Color.LightGreen.ToArgb(),
                Color.LightCyan.ToArgb(),
                Color.MediumVioletRed.ToArgb(),
                Color.MediumPurple.ToArgb(),
                Color.Yellow.ToArgb(),
                Color.White.ToArgb(),
            };

            ForegroundColor = ConsoleColor.White;
            BackgroundColor = ConsoleColor.Black;

            Clear();

        }

        public static void Wait(ref bool b)
        {
            int phase = 0;
            while (!b)
            {
                switch (phase)
                {
                    case 0:
                        Console.Write('/', true);
                        break;
                    case 1:
                        Console.Write('-', true);
                        break;
                    case 2:
                        Console.Write('\\', true);
                        break;
                    case 3:
                        Console.Write('|', true);
                        break;
                    case 4:
                        Console.Write('/', true);
                        break;
                    case 5:
                        Console.Write('-', true);
                        break;
                    case 6:
                        Console.Write('\\', true);
                        break;
                    case 7:
                        Console.Write('|', true);
                        break;
                }
                phase++;
                phase %= 8;
                Console.CursorX--;
                ACPITimer.Sleep(100000);
            }
        }

        public static void Wait(uint* provider, int bit)
        {
            int phase = 0;
            while (!BitHelpers.IsBitSet(*provider, bit))
            {
                switch (phase)
                {
                    case 0:
                        Console.Write('/', true);
                        break;
                    case 1:
                        Console.Write('-', true);
                        break;
                    case 2:
                        Console.Write('\\', true);
                        break;
                    case 3:
                        Console.Write('|', true);
                        break;
                    case 4:
                        Console.Write('/', true);
                        break;
                    case 5:
                        Console.Write('-', true);
                        break;
                    case 6:
                        Console.Write('\\', true);
                        break;
                    case 7:
                        Console.Write('|', true);
                        break;
                }
                phase++;
                phase %= 8;
                Console.CursorX--;
                ACPITimer.Sleep(100000);
            }
        }

        public static bool Wait(delegate*<bool> func, int timeOutMS = -1)
        {
            ulong prev = Timer.Ticks;

            int phase = 0;
            while (!func())
            {
                if (timeOutMS >= 0 && Timer.Ticks > (prev + (uint)timeOutMS))
                {
                    return false;
                }
                switch (phase)
                {
                    case 0:
                        Console.Write('/', true);
                        break;
                    case 1:
                        Console.Write('-', true);
                        break;
                    case 2:
                        Console.Write('\\', true);
                        break;
                    case 3:
                        Console.Write('|', true);
                        break;
                    case 4:
                        Console.Write('/', true);
                        break;
                    case 5:
                        Console.Write('-', true);
                        break;
                    case 6:
                        Console.Write('\\', true);
                        break;
                    case 7:
                        Console.Write('|', true);
                        break;
                }
                phase++;
                phase %= 8;
                Console.CursorX--;
                ACPITimer.Sleep(100000);
            }
            return true;
        }

        public static void Write(string s)
        {
            ConsoleColor col = Console.ForegroundColor;
            for (byte i = 0; i < s.Length; i++)
            {
                if (s[i] == '[')
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                Console.Write(s[i]);
                if (s[i] == ']')
                {
                    Console.ForegroundColor = col;
                }
            }
           // s.Dispose();
        }

        public static void Back()
        {
            if (CursorX == 0) return;
            WriteFramebuffer(' ');
            CursorX--;
            WriteFramebuffer(' ');
            UpdateCursor();
        }


        public static void Back(int start)
        {
            if (CursorX == 0) return;
            if (CursorX == start) return;
            WriteFramebuffer(' ');
            CursorX--;
            WriteFramebuffer(' ');
            UpdateCursor();
        }

        public static void Write(char chr, bool dontInvoke = false)
        {
            if (chr == '\n')
            {
                WriteLine();
                return;
            }
#if ASCII
            if (chr >= 0x20 && chr <= 0x7E)
#else
            unsafe
#endif
            {
                if (!dontInvoke)
                {
                    OnWrite?.Invoke(chr);
                }

                WriteFramebuffer(chr);

                CursorX++;
                if (CursorX == Width)
                {
                    CursorX = 0;
                    CursorY++;
                }
                MoveUp();
                UpdateCursor();
            }
        }

        private static void WriteFramebuffer(char chr)
        {
            if (Framebuffer.VideoMemory != null && !Framebuffer.TripleBuffered)
            {
                int X = (Framebuffer.Graphics.Width / 2) - ((Width * 8) / 2) + (CursorX * 8);
                int Y = (Framebuffer.Graphics.Height / 2) - ((Height * 16) / 2) + (CursorY * 16);
                Framebuffer.Graphics.FillRectangle(X, Y, 8, 16, ColorsFB[(int)BackgroundColor]);
                ASC16.DrawChar(chr, X, Y, ColorsFB[(int)ForegroundColor]);
            }
        }

        public static ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            Keyboard.CleanKeyInfo(true);
            while (Keyboard.KeyInfo.KeyChar == '\0') Native.Hlt();
            if (!intercept)
            {
                switch (Keyboard.KeyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        break;
                    case ConsoleKey.Delete:
                    case ConsoleKey.Backspace:
                        Console.Back();
                        break;
                    case ConsoleKey.Up:
                    case ConsoleKey.Down:
                    case ConsoleKey.Left:
                    case ConsoleKey.Right:

                    case ConsoleKey.F1:
                    case ConsoleKey.F2:
                    case ConsoleKey.F3:
                    case ConsoleKey.F4:
                    case ConsoleKey.F5:
                    case ConsoleKey.F6:
                    case ConsoleKey.F7:
                    case ConsoleKey.F8:
                    case ConsoleKey.F9:
                    case ConsoleKey.F10:
                    case ConsoleKey.F11:
                    case ConsoleKey.F12:

                    case ConsoleKey.LeftWindows:
                    case ConsoleKey.RightWindows:
                    default:
                        Console.Write(Keyboard.KeyInfo.KeyChar);
                        break;
                }
            }
            return Keyboard.KeyInfo;
        }

        public static ConsoleKeyInfo ReadKey(int start, bool intercept = false)
        {
            Keyboard.CleanKeyInfo(true);
            while (Keyboard.KeyInfo.KeyChar == '\0') Native.Hlt();
            if (!intercept)
            {
                switch (Keyboard.KeyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        break;
                    case ConsoleKey.Delete:
                    case ConsoleKey.Backspace:
                        Console.Back(start);
                        break;
                    case ConsoleKey.Up:
                    case ConsoleKey.Down:
                    case ConsoleKey.Left:
                    case ConsoleKey.Right:

                    case ConsoleKey.F1:
                    case ConsoleKey.F2:
                    case ConsoleKey.F3:
                    case ConsoleKey.F4:
                    case ConsoleKey.F5:
                    case ConsoleKey.F6:
                    case ConsoleKey.F7:
                    case ConsoleKey.F8:
                    case ConsoleKey.F9:
                    case ConsoleKey.F10:
                    case ConsoleKey.F11:
                    case ConsoleKey.F12:

                    case ConsoleKey.LeftWindows:
                    case ConsoleKey.RightWindows:

                    default:
                        Console.Write(Keyboard.KeyInfo.KeyChar);
                        break;
                }
            }
            return Keyboard.KeyInfo;
        }

        public static string ReadLine()
        {
            string s = string.Empty;
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey()).Key != ConsoleKey.Enter)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Delete:
                    case ConsoleKey.Backspace:
                        if (s.Length == 0) continue;
                        s.Length -= 1;
                        break;
                    default:
                        string cache1 = key.KeyChar.ToString();
                        string cache2 = s + cache1;
                        s.Dispose();
                        cache1.Dispose();
                        s = cache2;
                        break;

                }
                Native.Hlt();
            }
            return s;
        }

        public static string ReadLine(int start)
        {
            string s = string.Empty;
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(start)).Key != ConsoleKey.Enter)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Delete:
                    case ConsoleKey.Backspace:
                        if (s.Length == 0) continue;
                        if (start + s.Length > start)
                        {
                            s.Length -= 1;
                        }
                        break;
                    default:
                        string cache1 = key.KeyChar.ToString();
                        string cache2 = s + cache1;
                        s.Dispose();
                        cache1.Dispose();
                        s = cache2;
                        break;

                }
                Native.Hlt();
            }
            return s;
        }

        private static void MoveUp()
        {
            if (CursorY >= Height - 1)
            {
                MoveUpFramebuffer();
                CursorY--;
            }
        }

        private static void MoveUpFramebuffer()
        {
            if (Framebuffer.VideoMemory != null && !Framebuffer.TripleBuffered)
            {
                Framebuffer.Graphics.Copy(
                    (Framebuffer.Graphics.Width / 2) - (Width * 8 / 2),
                    (Framebuffer.Graphics.Height / 2) - (Height * 16 / 2),

                    (Framebuffer.Graphics.Width / 2) - (Width * 8 / 2),
                    (Framebuffer.Graphics.Height / 2) - (Height * 16 / 2) + 16,

                    Width * 8,
                    Height * 16
                    );
            }
        }

        private static void UpdateCursor()
        {
            UpdateCursorFramebuffer();
        }

        private static void UpdateCursorFramebuffer()
        {
            if (Framebuffer.VideoMemory != null && !Framebuffer.TripleBuffered)
            {
                uint color = 0x0;

                if (ColorsFB != null)
                {
                    color = ColorsFB[(int)ForegroundColor];
                }

                ASC16.DrawChar('_',
                            (Framebuffer.Graphics.Width / 2) - ((Width * 8) / 2) + ((CursorX) * 8),
                            (Framebuffer.Graphics.Height / 2) - ((Height * 16) / 2) + (CursorY * 16),
                            color
                            );
            }
        }

        public static void WriteLine(string s)
        {
            string _s = string.Copy(s);
            Write(_s);
            OnWrite?.Invoke('\n');
            WriteFramebuffer(' ');
            CursorX = 0;
            CursorY++;
            MoveUp();
            UpdateCursor();
            //_s.Dispose();
        }

        public static void WriteLine()
        {
            OnWrite?.Invoke('\n');
            WriteFramebuffer(' ');
            CursorX = 0;
            CursorY++;
            MoveUp();
            UpdateCursor();
        }

        public static void Clear()
        {
            CursorX = 0;
            CursorY = 0;
            ClearFramebuffer();
        }

        private static void ClearFramebuffer()
        {
            if (Framebuffer.VideoMemory != null && !Framebuffer.TripleBuffered)
            {
                uint color = 0x0;

                if (ColorsFB != null)
                {
                    color = ColorsFB[(int)BackgroundColor];
                }
                Framebuffer.Graphics.FillRectangle
                    (
                            (Framebuffer.Graphics.Width / 2) - ((Width * 8) / 2) + ((CursorX) * 8),
                            (Framebuffer.Graphics.Height / 2) - ((Height * 16) / 2) + (CursorY * 16),
                            Width * 8,
                            Height * 16,
                            color
                    );

            }
        }
    }
}