using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace SNES.Emulator
{
    public class AudioHandler
    {
        public byte[] SampleBufferL { get; set; } = new byte[735];
        public byte[] SampleBufferR { get; set; } = new byte[735];

        private readonly byte[] _inputBufferL = new byte[4096];
        private readonly byte[] _inputBufferR = new byte[4096];
        private int _inputBufferPos = 0;
        private int _inputReadPos = 0;

        private readonly Wav _sourceVoice;
        private readonly byte[] _inputBuffer = new byte[4096];

        public AudioHandler()
        {
            _sourceVoice = new Wav();
            //Process(new IntPtr(0));
        }

        private void Process(IntPtr e)
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

            _sourceVoice.SubmitSourceBuffer(_inputBuffer);
        }

        public void NextBuffer()
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
            _sourceVoice.Stop();
        }

        public void Resume()
        {
            _sourceVoice.Play();
        }
    }
}
