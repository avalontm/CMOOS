using MOOS.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.Sounds
{
    internal unsafe static class AudioManager
    {
        static WAV.Header Header;
        static byte[] PCM;
        static int Index;
        internal static bool IsPlaying;

        internal static void Initialize()
        {
            Interrupts.EnableInterrupt(0x20, DoPlay);
        }

        internal static bool Load(string file)
        {
            byte[] wav = File.ReadAllBytes(file);
            Index = 0;
            WAV.Decode(wav, out var pcm, out var hdr);
            wav.Dispose();
            PCM = pcm;
            Header = hdr;

            IsPlaying = true;

            return IsPlaying;
        }

        internal static void Stop() 
        {
            IsPlaying = false;
            PCM?.Dispose();
        }

        static void DoPlay()
        {
            if (PCM != null && IsPlaying)
            {
                if (Audio.bytesWritten != 0) return;
                if (Index + Audio.CacheSize > PCM.Length) IsPlaying = 0;

                fixed (byte* buffer = PCM)
                {
                    Index += Audio.CacheSize;
                    Audio.snd_write(buffer + Index, Audio.CacheSize);
                }
            }
        }
    }
}
