using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SNES.Emulator
{
    public class ROM
    {
        public Header? Header { get; private set; }
        private byte[]? _data;
        private byte[]? _sram;
        private bool _hasSram;
        private int _banks;
        private int _sramSize;

        private SNESSystem? _system;

        public ROM()
        {
          
        }

        public ROM()
        {
          
        }

        public void LoadROM(byte[] data, Header header)
        {
            _data = data;
            Header = header;
            _sram = new byte[header.RamSize];
            _hasSram = header.Chips > 0;
            _banks = header.RomSize / 0x8000;
            _sramSize = header.RamSize;
        }

        public void SetSystem(SNESSystem system)
        {
            _system = system;
        }

        public byte Read(int bank, int adr)
        {
            
            if (adr < 0x8000)
            {
                if (bank >= 0x70 && bank < 0x7e && _hasSram)
                {
                    return _sram[(((bank - 0x70) << 15) | (adr & 0x7fff)) & (_sramSize - 1)];
                }
            }
            return _data[((bank & (_banks - 1)) << 15) | (adr & 0x7fff)];
        }

        public void Write(int bank, int adr, byte value)
        { 
            if (adr < 0x8000 && bank >= 0x70 && bank < 0x7e && _hasSram)
            {
                _sram[(((bank - 0x70) << 15) | (adr & 0x7fff)) & (_sramSize - 1)] = value;
            }
        }
    }
}
