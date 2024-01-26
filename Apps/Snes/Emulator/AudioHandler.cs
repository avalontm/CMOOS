using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SNES.Emulator
{
    public unsafe class AudioHandler
    {
        public byte[] SampleBufferL { get; set; } = new byte[735];
        public byte[] SampleBufferR { get; set; } = new byte[735];

        private readonly byte[] _inputBufferL = new byte[4096];
        private readonly byte[] _inputBufferR = new byte[4096];
        private int _inputBufferPos = 0;
        private int _inputReadPos = 0;

        private readonly byte[] _inputBuffer = new byte[4096];

        int CacheSize = 0;
        public bool isPlaying;

        static AudioHandler Instance { set; get; }

        public AudioHandler()
        {
            Instance = this;
            CacheSize  = MoosNative.AudioCacheSize();
            MoosNative.SndDoPlay(&doPlay);
            Process();
        }

        static void doPlay()
        {
            if (Instance.isPlaying)
            {
                Instance.Process();

                byte[] wavHeader = CreateWavHeader(Instance._inputBuffer.Length);
                // Crear el buffer para todo el archivo WAV (encabezado + datos de audio)
                byte[] wavFileBuffer = new byte[wavHeader.Length + Instance._inputBuffer.Length];

                // Copiar el encabezado WAV al buffer del archivo completo
                Buffer.BlockCopy(wavHeader, 0, wavFileBuffer, 0, wavHeader.Length);

                // Copiar los datos de audio al buffer del archivo completo
                Buffer.BlockCopy(Instance._inputBuffer, 0, wavFileBuffer, wavHeader.Length, Instance._inputBuffer.Length);

                fixed (byte* p = wavFileBuffer)
                {
                    MoosNative.SndWrite(p, Instance.CacheSize);
                }
                wavHeader.Dispose();
                wavFileBuffer.Dispose();

            }
        }

        // Método para crear el encabezado WAV
        static byte[] CreateWavHeader(int dataLength)
        {
            int sampleRate = 44100; // Frecuencia de muestreo en Hz
            int channels = 1; // Mono
            int bitDepth = 16; // Profundidad de bits

            byte[] header = new byte[44];

            // RIFF header
            WriteBytes(header, 0, "RIFF");
            WriteInt32LittleEndian(header, 4, dataLength + 36);

            // WAVEfmt header
            WriteBytes(header, 8, "WAVEfmt ");
            WriteInt32LittleEndian(header, 16, 16); // Tamaño del bloque fmt
            WriteInt16LittleEndian(header, 20, 1);  // Formato de audio (PCM)
            WriteInt16LittleEndian(header, 22, (short)channels); // Número de canales
            WriteInt32LittleEndian(header, 24, sampleRate); // Frecuencia de muestreo
            WriteInt32LittleEndian(header, 28, sampleRate * channels * bitDepth / 8); // Tasa de bits por segundo
            WriteInt16LittleEndian(header, 32, (short)(channels * bitDepth / 8)); // Bytes por muestra
            WriteInt16LittleEndian(header, 34, (short)bitDepth); // Profundidad de bits

            // data header
            WriteBytes(header, 36, "data");
            WriteInt32LittleEndian(header, 40, dataLength); // Tamaño de los datos

            return header;
        }


        static void WriteBytes(byte[] destination, int offset, string source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                destination[offset + i] = (byte)source[i];
            }
        }

        // Método auxiliar para escribir un array de bytes en otro array
        static void WriteBytes(byte[] destination, int offset, char[] source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                destination[offset + i] = (byte)source[i];
            }
        }

        // Método auxiliar para escribir un entero de 16 bits en formato little-endian
        static void WriteInt16LittleEndian(byte[] destination, int offset, short value)
        {
            destination[offset] = (byte)value;
            destination[offset + 1] = (byte)(value >> 8);
        }

        // Método auxiliar para escribir un entero de 32 bits en formato little-endian
        static void WriteInt32LittleEndian(byte[] destination, int offset, int value)
        {
            destination[offset] = (byte)value;
            destination[offset + 1] = (byte)(value >> 8);
            destination[offset + 2] = (byte)(value >> 16);
            destination[offset + 3] = (byte)(value >> 24);
        }

        void Process()
        {
            if (_inputReadPos + 2048 > _inputBufferPos)
            {
                _inputReadPos = _inputBufferPos - 2048;
            }
            if (_inputReadPos + 4096 < _inputBufferPos)
            {
                _inputReadPos += 2048;
            }
            for (var i = 0; i < 2048; i++)
            {
                int position = _inputReadPos & 0xfff;
                _inputBuffer[i * 2] = _inputBufferL[position];
                _inputBuffer[i * 2 + 1] = _inputBufferR[position];
                _inputReadPos++;
            }
        }

        public  void NextBuffer()
        {
            for (var i = 0; i < 735; i++)
            {
                byte valL = SampleBufferL[i];
                byte valR = SampleBufferR[i];
                int position = _inputBufferPos & 0xfff;
                _inputBufferL[position] = valL;
                _inputBufferR[position] = valR;
                _inputBufferPos++;
           }
 
        }

        public void Pauze()
        {
            isPlaying = false;
        }

        public void Resume()
        {
            isPlaying = true;
        }
    }
}
