using MOOS;
using MOOS.Driver;
using MOOS.FS;
using MOOS.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Explorers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Desktops.Controls
{
    public class IconFile : DesktopControl
    {
        public int Key { set; get; }
        public bool isDirectory { set; get; }
        public bool isPkg { set; get; }
        public Image icon { set; get; }
        public string Path { set; get; }
        public string FilePath { set; get; }
        public Brush FocusBackground { set; get; }
        public FileInfo FileInfo { get; set; }
        public ExplorerManager OwnerWindow { get; set; }
        public string Extention { private set; get; }
        bool _isUnknown;
        bool _isFocus;
        int offsetX, offsetY;
        int _clickCount;
        ulong _timer;

        public IconFile()
        {
            Foreground = Brushes.White;
            FocusBackground = new Brush(Color.ToArgb(50, 100, 150, 240));

            icon = DesktopIcons.FileIcon;
            Width = DesktopIcons.FileIcon.Width;
            Height = DesktopIcons.FileIcon.Height;
            offsetX = 10;
            offsetY = 10;
        }

        public void onLoadIconExtention()
        {
            if (!string.IsNullOrEmpty(Content))
            {
                string ext = Content.ToLower();

                if (ext.EndsWith(".png"))
                {
                    icon = DesktopIcons.ImageIcon;
                    Extention = "png";
                }
                else if (ext.EndsWith(".mue"))
                {
                    icon = DesktopIcons.AppIcon;
                    Extention = "mue";
                }
                else if (ext.EndsWith(".app"))
                {
                    isPkg = true;

                    icon = DesktopIcons.AppPkg;
                    Extention = "app";
                }
                else if (ext.EndsWith(".wav"))
                {
                    icon = DesktopIcons.AudioIcon;
                    Extention = "wav";
                }
                else if (ext.EndsWith(".nes"))
                {
                    icon = DesktopIcons.GameIcon;
                    Extention = "nes";
                }
                else if (isDirectory)
                {
                    icon = DesktopIcons.FolderIcon;
                }
                else
                {
                    icon = DesktopIcons.FileIcon;
                    string[] strings = ext.Split('.');
                    if (strings.Length > 0)
                    {
                        Extention = strings[strings.Length - 1];
                    }
                    else
                    {
                        Extention = "unk";
                        _isUnknown = true;
                    }
                }
                ext.Dispose();
                OnPackage();
            }
        }

        void OnPackage()
        {
            if (isPkg)
            {
                //changeIcon from PKG
                string _icon = File.Instance.GetDirectory(FilePath) + Content + "/Content/" + "icon.png";
                PNG tmp = new PNG(File.Instance.ReadAllBytes(_icon));
                icon = tmp.ResizeImage(48, 48);
                tmp.Dispose();

                //remove Extention
                Content = Content.Substring(0, (Content.Length - (Extention.Length + 1)));
            }

            if (!isDirectory & !_isUnknown)
            {
                //remove the known extension
                Content = Content.Substring(0, (Content.Length - (Extention.Length + 1)));
            }
        }

        public override void Update()
        {
            base.Update();
            int _x = this.X;
            int _y = this.Y;

            if (OwnerWindow != null)
            {
                _x = OwnerWindow.X + this.X;
                _y = OwnerWindow.Y + this.Y;
            }

            if (Control.MousePosition.X > (_x - offsetX) && Control.MousePosition.X < ((_x - offsetX) + Width) && Control.MousePosition.Y > _y && Control.MousePosition.Y < (_y + Height))
            {
                _isFocus = true;

                if (Control.Clicked)
                {
                    if (Command != null)
                    {
                        if (_clickCount == 1) //Double Click
                        {
                            _clickCount = 0;

                            if (isDirectory)
                            {
                                Command.Execute.Invoke(this);
                                return;
                            }
                            else
                            {
                                if (Key == 0)
                                {
                                    Command.Execute.Invoke(this);
                                }
                                else
                                {
                                    Command.Execute.Invoke(this);
                                }
                                return;
                            }
                        }
                    }

                    _clickCount++;
                    _timer = Timer.Ticks + 500; //500ms
                }

            }
            else
            {
                _isFocus = false;
                _clickCount = 0;
            }

            if (_clickCount > 0)
            {
                if (Timer.Ticks > _timer)
                {
                    _clickCount = 0;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();

            int _x = this.X;
            int _y = this.Y;

            if (OwnerWindow != null)
            {
                _x = OwnerWindow.X + this.X;
                _y = OwnerWindow.Y + this.Y;
            }

            if (_isFocus)
            {
                Framebuffer.Graphics.AFillRectangle((_x- offsetX), (_y - offsetY), (Width + (offsetX*2)), (Height + ((WindowManager.font.FontSize * 3) + offsetX)), FocusBackground.Value);
            }

            Framebuffer.Graphics.DrawImage(_x, _y, icon, true);

            if (!string.IsNullOrEmpty(Content))
            {
                WindowManager.font.DrawString(_x, (_y + icon.Height), Content, Foreground.Value, (icon.Width + 8), (WindowManager.font.FontSize * 3));
            }
        }
    }
}
