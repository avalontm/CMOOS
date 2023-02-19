using MOOS.FS;
using MOOS.Misc;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace System.Desktops
{
    public static class DesktopIcons
    {
        public static Image AppIcon { set; get; }
        public static PNG AudioIcon { get; set; }
        public static Image BuiltInAppIcon { set; get; }
        public static PNG FolderIcon { get; set; }
        public static PNG DoomIcon { get; set; }
        public static Image AppTerminal { set; get; }
        public static Image WLanIcon { set; get; }
        public static Image FileIcon { get;  set; }
        public static PNG ImageIcon { get; set; }
        public static PNG GameIcon { get; set; }
        public static Image StartIcon { get; set; }
        public static Image Volume_OnIcon { get; set; }
        public static Image AppPkg { get; set; }
        public static void Initialize()
        {
            StartIcon = new PNG(File.Instance.ReadAllBytes("sys/media/Start.png"));
            FileIcon = new PNG(File.ReadAllBytes("sys/media/file.png"));
            ImageIcon = new PNG(File.ReadAllBytes("sys/media/Image.png"));
            GameIcon = new PNG(File.ReadAllBytes("sys/media/Game.png"));
            AppIcon = new PNG(File.Instance.ReadAllBytes("sys/media/App.png"));
            AudioIcon = new PNG(File.ReadAllBytes("sys/media/Audio.png"));
            BuiltInAppIcon = new PNG(File.Instance.ReadAllBytes("sys/media/BApp.png"));
            FolderIcon = new PNG(File.ReadAllBytes("sys/media/folder.png"));
            DoomIcon = new PNG(File.ReadAllBytes("sys/media/Doom1.png"));
            AppTerminal = new PNG(File.Instance.ReadAllBytes("sys/media/Terminal.png"));
            WLanIcon = new PNG(File.Instance.ReadAllBytes("sys/media/Wlan.png"));
            Volume_OnIcon = new PNG(File.Instance.ReadAllBytes("sys/media/volume_on.png"));
            AppPkg = new PNG(File.Instance.ReadAllBytes("sys/media/pkg.png"));
        }
    }
}
