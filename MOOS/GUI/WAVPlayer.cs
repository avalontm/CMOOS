using MOOS;
using MOOS.FS;
using MOOS.IOGroup;
using MOOS.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace MOOS.GUI
{
    internal unsafe class WAVPlayer : Window
    {
        static byte[] _pcm;
        static int _index;
        static WAV.Header _header;
        static WAVPlayer _player;
        static string _song_name;

        Image audiopause;
        Image audioplay;

        static bool playing;
        static bool isLoaded;

        public WAVPlayer()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            audiopause = new PNG(File.ReadAllBytes("sys/media/audiopause.png"));
            audioplay = new PNG(File.ReadAllBytes("sys/media/audioplay.png"));
            Title = "WAV Player";
            this.Width = 450;
            this.Height= 200;
            _pcm = null;
            _index = 0;
       
            playing = false;
            clickLock = false;
            _player = this;

            if (!isLoaded)
            {
                isLoaded = true;
                Interrupts.EnableInterrupt(0x20, &DoPlay);
            }
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
            byte[] wav = File.ReadAllBytes(file);
            _index = 0;
            WAV.Decode(wav, out var pcm, out var hdr);
            wav.Dispose();
            _pcm = pcm;
            _header = hdr;
            _song_name = file;

            playing = true;
        }

        static void DoPlay()
        {
            if (_pcm != null && _player.IsVisible && playing)
            {
                if (Audio.bytesWritten != 0) return;
                if (_index + Audio.CacheSize > _pcm.Length) _index = 0;

                MessageBox.Show($"Audio.bytesWritten: {Audio.bytesWritten}", "DoPlay");

                fixed (byte* buffer = _pcm)
                {
                    _index += Audio.CacheSize;
                    Audio.snd_write(buffer + _index, Audio.CacheSize);
                }
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            playing = false;
            _pcm?.Dispose();
            _player?.Dispose();
        }
    }
}
