using MOOS;
using MOOS.FS;
using System;
using System.Collections.Generic;
using System.Desktops;
using System.Desktops.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Explorers
{
    public class ExplorerManager : Window
    {
        public string Dir { set; get; }
        public List<IconFile> Files { private set; get; }

        public ExplorerManager()
        {
            Foreground = Brushes.Black;
            Width = 800;
            Height = 600;

            Files = new List<IconFile>();
        }

        public override void OnLoaded()
        {
            base.OnLoaded();

            int _separate = 60;
            int _x = 15;
            int _y = 0;
            string _devider = "/";


            List<FileInfo> files = File.GetFiles(Dir + _devider);

            for (int i = 0; i < files.Count; i++)
            {
                if ((_y + (DesktopIcons.FileIcon.Height + _separate)) > (this.Height - this.BarHeight))
                {
                    _y = 0;
                    _x += DesktopIcons.FileIcon.Width + (_separate / 2);
                }
                
                if (files[i].Attribute == FileAttribute.Hidden || files[i].Attribute == FileAttribute.System)
                {
                    continue;
                }
                
                IconFile icon = new IconFile();
                icon.OwnerWindow = this;
                icon.Content = files[i].Name;
                icon.Foreground = Brushes.Black;
                icon.Path = Dir + _devider;
                icon.FilePath = Dir + _devider + icon.Content;
                icon.FileInfo = files[i];
                icon.X = _x;
                icon.Y = _y + 15;

                if (files[i].Attribute == FileAttribute.Directory)
                {
                    icon.isDirectory = true;
                    icon.Command = DesktopManager.IconDirectoryClickCommand;
                }
                else
                {
                    icon.Command = DesktopManager.IconClickCommand;
                }

                icon.onLoadIconExtention();

                Files.Add(icon);

                _y += DesktopIcons.FileIcon.Height + _separate;
            }

            files.Dispose();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            for (int i = 0; i < Files.Count; i++)
            {
                Files[i].Update();
            }
        }

        public override void OnDraw()
        {
            base.OnDraw();

            for (int i = 0; i < Files.Count; i++)
            {
                Files[i].Draw();
            }
        }

        public override void OnClose()
        {
            Files.Clear();
            Files.Dispose();

            base.OnClose();
        }
    }
}
