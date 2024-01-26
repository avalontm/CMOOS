using Internal.Runtime.CompilerServices;
using MOOS.FS;
using MOOS.Misc;
using System;
using System.Collections.Generic;
using System.Text;
using static MOOS.Misc.Interrupts;
using static MOOS.Misc.WAV;

namespace MOOS.Api
{
    public static unsafe class AUDIO
    {
        public static unsafe void* HandleSystemCall(string name)
        {
            switch (name)
            {
                case "SndLoad":
                    return (delegate*<string, ulong*, byte**, bool>)&API_SndLoad;
                case "SndLoadBuffer":
                    return (delegate*<byte*, ulong*, byte**, bool>)&API_SndLoadBuffer;
                case "AudioCacheSize":
                    return (delegate*<int>)&API_AudioCacheSize;
                case "API_AudioBytesWritten":
                    return (delegate*<int>)&API_AudioBytesWritten;
                case "SndDoPlay":
                    return (delegate*<delegate*<void>, void>)&API_SndDoPlay;
                case "SndWrite":
                    return (delegate*<byte*, int, int>)&API_SndWrite;
                case "SndClear":
                    return (delegate*<void>)&API_SndClear;
                case "HasAudioDevice":
                    return (delegate*<bool>)&API_HasAudioDevice;
            }

            return null;
        }

        public static bool API_HasAudioDevice()
        {
            return Audio.HasAudioDevice;
        }

        public static int API_SndWrite(byte* buffer, int len)
        {
            return Audio.snd_write(buffer, len);
        }

        private static void API_SndDoPlay(delegate*<void> func)
        {
            Interrupts.EnableInterrupt(0x20, func);
        }

        private static int API_AudioCacheSize()
        {
            return Audio.CacheSize;
        }

        private static int API_AudioBytesWritten()
        {
            return Audio.bytesWritten;
        }

        static void API_SndClear()
        {
            Audio.snd_clear();
        }

        private static bool API_SndLoad(string file, ulong* length, byte** pcm)
        {
            byte[] data = RamFile.ReadAllBytes(file);

            if (data == null)
            {
                return false;
            }

            WAV.Decode(data, out byte[] _pcm, out Header header);

            *pcm = (byte*)Allocator.Allocate((ulong)_pcm.Length);
            *length = (ulong)_pcm.Length;
            fixed (byte* p = _pcm) Native.Movsb(*pcm, p, *length);

            data.Dispose();

            return true;
        }

        private static bool API_SndLoadBuffer(byte* data, ulong* length, byte** pcm)
        {
            WAV.Decode(data, out byte[] _pcm, out Header header);

            *pcm = (byte*)Allocator.Allocate((ulong)_pcm.Length);
            *length = (ulong)_pcm.Length;
            fixed (byte* p = _pcm) Native.Movsb(*pcm, p, *length);

            return true;
        }
    }
}
