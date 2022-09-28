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
            Background = new System.Windows.Media.Brush(0xFFFFFFFF);
            Foreground = new System.Windows.Media.Brush(0xFF000000);
            Keyboard.OnKeyChanged += Keyboard_OnKeyChanged;
            Console.OnWrite += Console_OnWrite;

        }

        void Keyboard_OnKeyChanged(ConsoleKeyInfo key)
        {
            if (!this.Focus)
            {
                return;
            }

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

                    Cmd += key.KeyChar.ToString();
                }

                if (key.Key == System.ConsoleKey.Enter)
                {
                    if (string.IsNullOrEmpty(Cmd))
                    {
                        return;
                    }

                    if (Cmd.Length != 0) Cmd.Length -= 1;

                    string[] _params = ParseCommand(Cmd);
                    Cmd = _params[0];

                    // when a command is invoked
                    switch (Cmd.ToLower())
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
                            Console.WriteLine("net: network api test");
                            Console.WriteLine("ping: network");
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
                                Thread thread = new Thread(() =>
                                {
                                    Console.WriteLine("[HttpClient] Connecting...");
                                    HttpClient http = new HttpClient("192.168.1.34", 8080);
                                    Console.WriteLine($"[Response] {http.GetAsync("api/lote/1")}");
                                }).Start();
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
                                    Thread thread = new Thread(() =>
                                    {
                                        ICMPClient icmp = new ICMPClient();
                                        icmp.Connect(Address.Parse(_params[1]));
                                        icmp.SendEcho();
                                    }).Start();
                                }
                            }
                            break;
                        default:
                            Console.WriteLine($"No such command: \"{Cmd}\"");
                            break;
                    }

                    //Clear Command
                    Cmd = string.Empty;
                }
            }
        }


        string[] ParseCommand(string cmd)
        {
            string[] _params = null;

            if (string.IsNullOrEmpty(cmd))
            {
                _params = new string[1];
                _params[0] = cmd;
            }
            else
            {
                _params = cmd.Split(' ');
                if (_params == null || _params.Length == 0)
                {
                    _params = new string[1];
                    _params[0] = cmd;
                }
            }
            return _params;
        }

        public override void OnDraw()
        {
            base.OnDraw();

            string s0 = "_";
            string s1 = Data + s0;
            DrawString(X, Y, s1, Height, Width);
            s0.Dispose();
            s1.Dispose();
        }

        public void DrawString(int X, int Y, string Str, int HeightLimit = -1, int LineLimit = -1)
        {
            int w = 0, h = 0;
            for (int i = 0; i < Str.Length; i++)
            {
                w += WindowManager.font.DrawChar(Framebuffer.Graphics, X + w, Y + h, Str[i], Foreground.Value);
                if (w + WindowManager.font.FontSize > LineLimit && LineLimit != -1 || Str[i] == '\n')
                {
                    w = 0;
                    h += WindowManager.font.FontSize;

                    if (HeightLimit != -1 && h >= HeightLimit)
                    {
                        Framebuffer.Graphics.Copy(X, Y, X, Y + WindowManager.font.FontSize, LineLimit, HeightLimit - (WindowManager.font.FontSize));
                        Framebuffer.Graphics.FillRectangle(X, Y + HeightLimit - (WindowManager.font.FontSize), LineLimit, WindowManager.font.FontSize, Background.Value);
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
                DesktopManager.Terminal.ShowDialog();
                Console.WriteLine("Type help to get information!");
            }

            Data += chr.ToString();
        }
    }
}
#endif