using MOOS;
using MOOS.FS;
using MOOS.GUI;
using MOOS.Misc;
using MOOS.NET;
using MOOS.NET.Config;
using System;
using System.Collections.Generic;
using System.Desktops.Controls;
using System.Diagnostics;
using System.Drawing;
using System.Explorers;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using static MOOS.stdio;

namespace System.Desktops
{
    public static class DesktopManager
    {
        static DesktopBar bar { set; get; }
        static DesktopDocker docker { set; get; }
        static List<DesktopControl> barMenu { set; get; }
        static List<IconFile> icons { set; get; }

        public static Image Wallpaper;

        public static string Prefix;
        public static string Dir;
        public static string User;

        public static bool IsAtRoot => Dir.Length < 1;
        public static ICommand IconClickCommand { get; set; }
        public static ICommand IconNativeClickCommand { get; set; }
        public static ICommand IconDirectoryClickCommand { get; set; }
        public static Terminal Terminal { get;  set; }
        public static Point LastPoint;

        public static void Initialize()
        {
            User = "moos";
            Dir = "";

#if Chinese
			Prefix = $" 管理员@{User}: ";
#else
            Prefix = $" root@{User}: ";
#endif

            //Image from unsplash
            Wallpaper = new PNG(File.ReadAllBytes("sys/media/Wallpaper2.png"));

            BitFont.Initialize();

            string CustomCharset = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            BitFont.RegisterBitFont(new BitFontDescriptor("Song", CustomCharset, File.ReadAllBytes("sys/fonts/Song.btf"), 16));

            Image wall = Wallpaper;
            Wallpaper = wall.ResizeImage(Framebuffer.Width, Framebuffer.Height);
            wall.Dispose();

            DesktopIcons.Initialize();
            WindowManager.Initialize();
            CursorManager.Initialize();

            bar = new DesktopBar();
            docker = new DesktopDocker();
            barMenu = new List<DesktopControl>();
            icons = new List<IconFile>();

            //Bar Elements
            DesktopBarItem item = new DesktopBarItem();
            item.Icon = DesktopIcons.StartIcon.ResizeImage(24, 24);
            item.X = 0;
            item.Y = 0;
            item.Command = new ICommand(onItemStart);
            barMenu.Add(item);

            DesktopBarItem wlan = new DesktopBarItem();
            wlan.HorizontalAlignment = Windows.HorizontalAlignment.Right;
            wlan.X = 96;
            wlan.Y = 0;
            wlan.Icon = DesktopIcons.WLanIcon.ResizeImage(24,24);
            wlan.Command = new ICommand(onItemWlan);
            barMenu.Add(wlan);

            DesktopBarItem volume = new DesktopBarItem();
            volume.HorizontalAlignment = Windows.HorizontalAlignment.Right;
            volume.X = 64;
            volume.Y = 0;
            volume.Icon = DesktopIcons.Volume_OnIcon.ResizeImage(24, 24);
            volume.Command = new ICommand(onItemVolume);
            barMenu.Add(volume);

            DesktopBarClock clock = new DesktopBarClock();
            clock.HorizontalAlignment = Windows.HorizontalAlignment.Right;
            clock.X = 5;
            clock.Y = 0;
            clock.Command = new ICommand(onItemClock);
            barMenu.Add(clock);

            onLoadIcons();

            Lockscreen.Initialize();

            #region Animation of entering Desktop
            Framebuffer.Graphics.DrawImage((Framebuffer.Width / 2) - (DesktopManager.Wallpaper.Width / 2), (Framebuffer.Height / 2) - (DesktopManager.Wallpaper.Height / 2), DesktopManager.Wallpaper, false);
            DesktopManager.Update();
            WindowManager.Draw();
            Framebuffer.Graphics.DrawImage(Control.MousePosition.X, Control.MousePosition.Y, CursorManager.GetCursor);
            Image _screen = Framebuffer.Graphics.Save();
            Framebuffer.Graphics.Clear(0x0);

            var SizedScreens = new Image[60];
            int startat = 40;
            for (int i = startat; i < SizedScreens.Length; i++)
            {
                SizedScreens[i] = _screen.ResizeImage(
                    (int)(_screen.Width * (i / ((float)SizedScreens.Length))),
                    (int)(_screen.Height * (i / ((float)SizedScreens.Length)))
                    );
            }

            Animation ani = new Animation()
            {
                Value = startat + 1,
                MinimumValue = startat + 1,
                MaximumValue = SizedScreens.Length - 1,
                ValueChangesInPeriod = 1,
                PeriodInMS = 16
            };
            Animator.AddAnimation(ani);
            while (ani.Value < SizedScreens.Length - 1)
            {
                int i = ani.Value;
                Image img = SizedScreens[i];
                Framebuffer.Graphics.Clear(0x0);
                Framebuffer.Graphics.ADrawImage(
                    (Framebuffer.Graphics.Width / 2) - (img.Width / 2),
                    (Framebuffer.Graphics.Height / 2) - (img.Height / 2),
                    img, (byte)(255 * (i / (float)(SizedScreens.Length - startat))));
                Framebuffer.Update();
            }
            Animator.DisposeAnimation(ani);

            _screen.Dispose();
            for (int i = 0; i < SizedScreens.Length; i++) SizedScreens[i]?.Dispose();
            SizedScreens.Dispose();
            #endregion

            NotificationManager.Initialize();

        }

        static void onLoadIcons()
        {
            int _barHeight = bar.Height + 20;
            int _separator = 60;
            int _x = 0;
            int _y = _barHeight;
            string _devider = "/";

            IconClickCommand = new ICommand(onDesktopIconClick);
            IconDirectoryClickCommand = new ICommand(onDesktopDirectoryClick);

            string dirDesktop = $"home/{User}/Desktop";

            List<FileInfo> files = File.GetFiles(dirDesktop + _devider);

            for (int i = 0; i < files.Count; i++)
            {
                if (_y + DesktopIcons.FileIcon.Height + _separator > Framebuffer.Graphics.Height - _separator)
                {
                    _y = _barHeight;
                    _x += DesktopIcons.FileIcon.Width + (_separator / 2);
                }

                if (files[i].Attribute == FileAttribute.Hidden || files[i].Attribute == FileAttribute.System)
                {
                    continue;
                }

                IconFile icon = new IconFile();
                icon.Content = files[i].Name;
                icon.Path = dirDesktop + _devider;
                icon.FilePath = dirDesktop + _devider + icon.Content;
                icon.FileInfo = files[i];
                icon.X = _x + 15;
                icon.Y = _y;

                if (files[i].Attribute == FileAttribute.Directory)
                {
                    icon.isDirectory = true;
                    icon.Command = IconDirectoryClickCommand;
                }
                else
                {
                    icon.Command = IconClickCommand;
                }

                icon.onLoadIconExtention();

                icons.Add(icon);

                _y += DesktopIcons.FileIcon.Height + _separator;
            }

            files.Dispose();
        }

        static void onDesktopDirectoryClick(object obj)
        {
            IconFile file = obj as IconFile;

            if (file.isPkg)
            {
                Process.Start(file.FilePath + "/Content/" + "app.mue");
                return;
            }

            ExplorerManager explorer = new ExplorerManager();
            explorer.Title = file.Content;
            explorer.Dir = file.FilePath;
            explorer.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            explorer.ShowDialog();
        }

        static unsafe void onDesktopIconClick(object obj)
        {
            IconFile file = obj as IconFile;

            if (string.IsNullOrEmpty(file.FilePath))
            {
                MessageBox.Show("Can't open file.", "Not Found");
                return;
            }

            switch (file.Extention)
            {
                case "png":
                    {
                        byte[] buffer = File.ReadAllBytes(file.FilePath);
                        PNG png = new(buffer);
                        buffer.Dispose();
                        ImageViewer img = new ImageViewer(100, 100);
                        img.SetImage(png);
                        img.ShowDialog();
                    }
                    break;
                case "mue":
                    {
                        Process.Start(file.FilePath);
                    }
                    break;
                case "wav":
                    {
                        if (Audio.HasAudioDevice)
                        {
                            WAVPlayer wavplayer = new WAVPlayer();
                            wavplayer.ShowDialog();
                            wavplayer.Play(file.FilePath);
                        }
                        else
                        {
                            MessageBox.Show("Audio controller is unavailable!", "Error");
                        }
                    }
                    break;
                default:
                    {
                        MessageBox.Show("The system did not find a suitable program to execute this file.", "Can not be executed");
                    }
                    break;
            }
        }

        static void onItemStart(object obj)
        {
            NotificationManager.Add(new Nofity($"Welcome to MOOS", NotificationLevel.None));
        }

        static void onItemWlan(object obj)
        {
            if (NetworkDevice.Devices.Count > 0)
            {
                NotificationManager.Add(new Nofity($"Info: Network device {NetworkDevice.Devices[0].NameID} ({NetworkConfiguration.CurrentAddress.ToString()})", NotificationLevel.None));
            }
            else
            {
                NotificationManager.Add(new Nofity($"Warn: No Network device found on this PC", NotificationLevel.Error));
            }
        }

        static void onItemVolume(object obj)
        {
            if (Audio.HasAudioDevice)
            {
                NotificationManager.Add(new Nofity(Audio.HasAudioDevice ? "Info: Audio controller available" : "Warn: No audio controller found on this PC", Audio.HasAudioDevice ? NotificationLevel.None : NotificationLevel.Error));
            }
        }

        static void onItemClock(object obj)
        {
            Debug.WriteLine($"[Item] Clock");
        }

        public static void Update()
        {
            for (int i = 0; i < icons.Count; i++)
            {
                icons[i].Update();
            }
            if (docker != null)
            {
                docker.Update();
            }
            if (bar != null)
            {
                bar.Update();
            }
            for (int i = 0; i < barMenu.Count; i++)
            {
                barMenu[i].Update();
            }
        }

        public static void Draw()
        {
            Framebuffer.Graphics.DrawImage((Framebuffer.Width / 2) - (Wallpaper.Width / 2), (Framebuffer.Height / 2) - (Wallpaper.Height / 2), Wallpaper);

            for (int i = 0; i < icons.Count; i++)
            {
                icons[i].Draw();
            }

            if (Control.MouseButtons.HasFlag(MouseButtons.Left) && !WindowManager.HasWindowMoving && !WindowManager.MouseHandled)
            {
                if (LastPoint.X == -1 && LastPoint.Y == -1)
                {
                    LastPoint.X = Control.MousePosition.X;
                    LastPoint.Y = Control.MousePosition.Y;
                }
                else
                {
                    if (Control.MousePosition.X > LastPoint.X && Control.MousePosition.Y > LastPoint.Y)
                    {
                        Framebuffer.Graphics.AFillRectangle(
                            LastPoint.X,
                            LastPoint.Y,
                            Control.MousePosition.X - LastPoint.X,
                            Control.MousePosition.Y - LastPoint.Y,
                            0x7F2E86C1);
                    }

                    if (Control.MousePosition.X < LastPoint.X && Control.MousePosition.Y < LastPoint.Y)
                    {
                        Framebuffer.Graphics.AFillRectangle(
                            Control.MousePosition.X,
                            Control.MousePosition.Y,
                            LastPoint.X - Control.MousePosition.X,
                            LastPoint.Y - Control.MousePosition.Y,
                            0x7F2E86C1);
                    }

                    if (Control.MousePosition.X < LastPoint.X && Control.MousePosition.Y > LastPoint.Y)
                    {
                        Framebuffer.Graphics.AFillRectangle(
                            Control.MousePosition.X,
                            LastPoint.Y,
                            LastPoint.X - Control.MousePosition.X,
                            Control.MousePosition.Y - LastPoint.Y,
                            0x7F2E86C1);
                    }

                    if (Control.MousePosition.X > LastPoint.X && Control.MousePosition.Y < LastPoint.Y)
                    {
                        Framebuffer.Graphics.AFillRectangle(
                            LastPoint.X,
                            Control.MousePosition.Y,
                            Control.MousePosition.X - LastPoint.X,
                            LastPoint.Y - Control.MousePosition.Y,
                            0x7F2E86C1);
                    }
                }

            }
            else
            {
                LastPoint.X = -1;
                LastPoint.Y = -1;
            }
            if (bar != null)
            {
                bar.Draw();
            }
            if (docker != null)
            {
                docker.Draw();
            }
            for (int i = 0; i < barMenu.Count; i++)
            {
                barMenu[i].Draw();
            }

        }
    }
}
