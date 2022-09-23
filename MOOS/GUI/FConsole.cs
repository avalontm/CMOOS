#if HasGUI

using MOOS.Driver;
using MOOS.Graph;
using MOOS.Misc;
using MOOS.NET;
using MOOS.NET.IPv4;
using MOOS.NET.IPv4.TCP;
using MOOS.NET.IPv4.UDP.DNS;
using System;
using System.Collections.Generic;
using System.Desktops;
using System.Drawing;
using System.Net.Http;
using System.Windows;

namespace MOOS.GUI
{
    public class Terminal : Window
    {
        string Data;
        public Image ScreenBuf;
        string Cmd;

        public Terminal()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Width = 640;
            this.Height = 320;
#if Chinese
            Title = "控制台";
#else
            Title = "Terminal";
#endif
            Cmd = string.Empty;
            Data = string.Empty;
            ScreenBuf = new Image(640, 320);

            Rebind();
            Console.WriteLine("Type help to get information!");

            Console.OnWrite += Console_OnWrite;
        }

        public void Rebind()
        {
            Keyboard.OnKeyChanged += Keyboard_OnKeyChanged;
        }

        void Keyboard_OnKeyChanged(ConsoleKeyInfo key)
        {
            if (key.KeyState == System.ConsoleKeyState.Pressed)
            {
                if (key.Key == System.ConsoleKey.Backspace)
                {
                    if (Data.Length != 0)
                        Data.Length -= 1;
                }
                else if (key.KeyChar != '\0')
                {
                    Console_OnWrite(key.KeyChar);

                    string cs = key.KeyChar.ToString();
                    string cache1 = Cmd;
                    Cmd = cache1 + cs;
                    cache1.Dispose();
                }

                if (key.Key == System.ConsoleKey.Enter)
                {
                    if (Cmd.Length != 0) Cmd.Length -= 1;


                    string[] _params = GetCommand(Cmd);
                    Cmd = _params[0];

                    // when a command is invoked
                    switch (Cmd)
                    {
                        case "hello":
                            int a = 1;
                            int b = 0;
                            int c = a / b;
                            break;

                        case "help":
                            Console.WriteLine("help: to get this information");
                            Console.WriteLine("shutdown: power off");
                            Console.WriteLine("reboot: reboot this computer");
                            Console.WriteLine("cpu: list cpu information");
                            Console.WriteLine("hello: issue kernel panic");
                            break;

                        case "shutdown":
                            Power.Shutdown();
                            break;

                        case "cpu":
                            Console.WriteLine("multi-processor IDs:");
                            for (int i = 0; i < ACPI.LocalAPIC_CPUIDs.Count; i++)
                                Console.WriteLine($" cpu id:{ACPI.LocalAPIC_CPUIDs[i]}");
                            Console.WriteLine($"frequency: {Timer.CPU_Clock / 1048576}mhz");
                            break;

                        case "span":
                            string test = "hello world from MOOS span test!";
                            Span<char> span = stackalloc char[test.Length];
                            test.AsSpan().CopyTo(span);
                            span = span.Slice(17, 15);

                            for (int i = 0; i < span.Length; i++)
                            {
                                Console.Write(span[i]);
                            }
                            Console.WriteLine();
                            break;

                        case "null":
                            unsafe
                            {
                                uint* ptr = null;
                                *ptr = 0xDEADBEEF;
                            }
                            break;

                        case "reboot":
                            Power.Reboot();
                            break;
                        case "net":
                            if (NetworkDevice.Devices.Count == 0)
                            {
                                Console.Write("No Network Device.");
                            }
                            else
                            {
                                Console.WriteLine("[HttpClient] Connecting...");
                                HttpClient http = new HttpClient("192.168.1.34", 8080);
                                Console.WriteLine($"[Response] {http.GetAsync("api/lote/1")}");
                            }
                            break;
                        case "ping":
                            if (NetworkDevice.Devices.Count == 0)
                            {
                                Console.Write("No Network Device.");
                            }
                            else
                            {
                                if (_params.Length <= 1)
                                {
                                    Console.Write("Command ping required parameters.");
                                }
                                else
                                {
                                    ICMPClient icmp = new ICMPClient();
                                    icmp.Connect(Address.Parse(_params[1]));
                                    icmp.SendEcho();
                                }
                            }
                            break;
                        default:
                            Console.Write("No such command: \"");
                            Console.Write(Cmd);
                            Console.WriteLine("\"");
                            break;
                    }

                    Cmd.Dispose();
                    Cmd = string.Empty;
                }
                else if (key.Key == System.ConsoleKey.Backspace) if (Cmd.Length != 0) Cmd.Length -= 1;
            }
        }


        string[] GetCommand(string cmd)
        {
            string[] _params = cmd.Split(' ');
            if (_params.Length == 0)
            {
                _params = new string[1];
                _params[0] = cmd;
            }
            return _params;
        }

        public override void OnDraw()
        {
            base.OnDraw();
            int w = 0, h = 0;

            string s0 = "_";
            string s1 = Data + s0;
            //BitFont.DrawString("Song", 0xFFFFFFFF, s, X, Y, Width);
            DrawString(X, Y, s1, Height, Width);
            s0.Dispose();
            s1.Dispose();
            //BitFont.DrawString("Song", 0xFFFFFFFF, Data, X, Y, 640);
        }

        public void DrawString(int X, int Y, string Str, int HeightLimit = -1, int LineLimit = -1)
        {
            int w = 0, h = 0;
            for (int i = 0; i < Str.Length; i++)
            {
                w += WindowManager.font.DrawChar(Framebuffer.Graphics, X + w, Y + h, Str[i]);
                if (w + WindowManager.font.FontSize > LineLimit && LineLimit != -1 || Str[i] == '\n')
                {
                    w = 0;
                    h += WindowManager.font.FontSize;

                    if (HeightLimit != -1 && h >= HeightLimit)
                    {
                        Framebuffer.Graphics.Copy(X, Y, X, Y + WindowManager.font.FontSize, LineLimit, HeightLimit - (WindowManager.font.FontSize));
                        Framebuffer.Graphics.FillRectangle(X, Y + HeightLimit - (WindowManager.font.FontSize), LineLimit, WindowManager.font.FontSize, 0xFF222222);
                        h -= WindowManager.font.FontSize;
                    }
                }
            }
        }

        void Console_OnWrite(char chr)
        {
            if (DesktopManager.Terminal == null)
            {
                DesktopManager.Terminal = new Terminal();
            }

            string cs = chr.ToString();
            string cache = Data;
            Data = cache + cs;
            cs.Dispose();
            cache.Dispose();

            DesktopManager.Terminal.ShowDialog();
        }
    }
}
#endif