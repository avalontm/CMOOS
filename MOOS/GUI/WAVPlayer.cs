using MOOS;
using MOOS.FS;
using MOOS.IOGroup;
using MOOS.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Sounds;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace MOOS.GUI
{
    internal unsafe class WAVPlayer : Window
    {
        string _song_name;

        Image audiopause;
        Image audioplay;

        bool playing;

        public WAVPlayer()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            audiopause = new PNG(File.ReadAllBytes("sys/media/audiopause.png"));
            audioplay = new PNG(File.ReadAllBytes("sys/media/audioplay.png"));
            Title = "WAV Player";
            this.Width = 450;
            this.Height= 200;

            playing = false;
            clickLock = false;
        }

        bool clickLock;

        public override void OnInput()
        {
            base.OnInput();

            if (Control.MouseButtons.HasFlag(MouseButtons.Left))
            {
                if (IsUnderMouse() && !clickLock)
                {
                    playing = !playing;
                    clickLock = true;
                }
            }
            else
            {
                clickLock = false;
            }
        }

        public override void OnDraw()
        {
            base.OnDraw();

            if (!string.IsNullOrEmpty(_song_name))
            {
                string s = $"Playing: {_song_name}";
                int len = WindowManager.font.MeasureString(s);
                WindowManager.font.DrawString(X + (Width / 2 - len / 2), Y + 25, s);
                s.Dispose();
            }

            Framebuffer.Graphics.DrawImage(X + (Width / 2 - audioplay.Width / 2), Y + (Height / 2 - audioplay.Height / 2), playing ? audiopause : audioplay);
        }

        public void Play(string file)
        {
            _song_name = file;
            playing = AudioManager.Load(file);
        }

        public override void OnClose()
        {
            base.OnClose();
            AudioManager.Stop();
        }
    }
}
