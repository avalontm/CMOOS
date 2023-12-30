using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.Media
{
    public unsafe class Wav
    {
        internal byte* pcm { get; private set; }
        internal ulong size { get; private set; }
        internal int index { get; private set; }
        internal static int CacheSize { get; private set; }

        public bool isPlaying { get; private set; }
        public bool isLoaded { get; private set; }

        internal bool isSetDoPlay { get; private set; }

        internal static List<Wav> players = new List<Wav>();

        public Wav(string file)
        {
            index = 0;
            CacheSize = MoosNative.AudioCacheSize();
            isLoaded = MoosNative.SndLoad(file, out ulong _size, out byte* _pcm);

            pcm = _pcm;
            size = _size;

            players.Add(this);

            if (!isSetDoPlay)
            {
                isSetDoPlay = true;
                MoosNative.SndDoPlay(&doPlay);
            }
        }

        public void Play()
        {
            isPlaying = true;
        }

        public void Stop()
        {
            index = 0;
            isPlaying = false;
        }

        static void doPlay()
        {
            for (int i = 0; i < players.Count; i++)
            {
                Wav player = players[i];
                
                if (player.isLoaded && player.isPlaying)
                {
                    if (MoosNative.AudioBytesWritten() != 0) return;

                    if (player.index > player.size)
                    {
                        player.Stop();
                        return;
                    }

                    MoosNative.SndWrite(player.pcm + player.index, CacheSize);
                    player.index += CacheSize;
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            pcm = null;
            size = 0;
            index = 0;
        }
    }
}
