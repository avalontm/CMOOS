using MOOS.IO;
using MOOS.Memory;
using System.Collections.Generic;
using System.Drawing;

namespace MOOS.Driver
{
    public unsafe class VGAScreen
    {
        #region IO Ports
        private readonly IOPort AttributeController_Index;
        private readonly IOPort AttributeController_Read;
        private readonly IOPort AttributeController_Write;

        private readonly IOPort MiscellaneousOutput_Write;

        private readonly IOPort Sequencer_Index;
        private readonly IOPort Sequencer_Data;

        private readonly IOPort DACIndex_Read;
        private readonly IOPort DACIndex_Write;
        private readonly IOPort DAC_Data;

        private readonly IOPort GraphicsController_Index;
        private readonly IOPort GraphicsController_Data;
        private readonly IOPort CRTController_Index;
        private readonly IOPort CRTController_Data;
        private readonly IOPort Instat_Read;
        #endregion

        private const byte NumSeqRegs = 5;
        private const byte NumCRTCRegs = 25;
        private const byte NumGCRegs = 9;
        private const byte NumACRegs = 21;

        public MemoryBlock08 VideoMemory { private set; get; }

        private ushort width;
        private ushort height;
        private ushort bpp;

        public ushort Width
        { get { return width; } }

        public ushort Height
        { get { return height; } }

        public ushort Bpp
        { get { return bpp; } }

        public VGAScreen()
        {
            AttributeController_Index = new IOPort(0x3C0);
            AttributeController_Read = new IOPort(0x3C1);
            AttributeController_Write = new IOPort(0x3C0);
            MiscellaneousOutput_Write = new IOPort(0x3C2);
            Sequencer_Index = new IOPort(0x3C4);
            Sequencer_Data = new IOPort(0x3C5);
            DACIndex_Read = new IOPort(0x3C7);
            DACIndex_Write = new IOPort(0x3C8);
            DAC_Data = new IOPort(0x3C9);
            GraphicsController_Index = new IOPort(0x3CE);
            GraphicsController_Data = new IOPort(0x3CF);
            CRTController_Index = new IOPort(0x3D4);
            CRTController_Data = new IOPort(0x3D5);
            Instat_Read = new IOPort(0x3DA);

            VideoMemory = new MemoryBlock08(0xA0000, 0);
        }

        public void SetMode0()
        {
            short[] g_640x480x16 =
                {
                    /* MISC */
			        0xE3,
			        /* SEQ */
			        0x03, 0x01, 0x08, 0x00, 0x06,
			        /* CRTC */
			        0x5F, 0x4F, 0x50, 0x82, 0x54, 0x80, 0x0B, 0x3E,
                    0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0xEA, 0x0C, 0xDF, 0x28, 0x00, 0xE7, 0x04, 0xE3,
                    0xFF,
			        /* GC */
			        0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x05, 0x0F,
                    0xFF,
			        /* AC */
			        0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x14, 0x07,
                    0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
                    0x01, 0x00, 0x0F, 0x00, 0x00
                };

            WriteRegister(g_640x480x16);
            
            width = 640;
            height = 480;
            bpp = 16;
            
            byte[] xHex = new byte[] { 0x00, 0x33, 0x66, 0x99, 0xCC, 0xFF };
            int c = 0;
            for (byte i = 0; i < 6; i++)
                for (byte j = 0; j < 6; j++)
                    for (byte k = 0; k < 6; k++)
                    {
                        SetPaletteEntry(c, xHex[i], xHex[j], xHex[k]);
                        c++;
                    }
        }

        private void WriteRegister(short[] registers)
        {
            int xIdx = 0;
            byte i;

            /* write MISCELLANEOUS reg */
            MiscellaneousOutput_Write.Byte = (byte)registers[xIdx];
            xIdx++;

            /* write SEQUENCER regs */
            for (i = 0; i < NumSeqRegs; i++)
            {
                Sequencer_Index.Byte = i;
                Sequencer_Data.Byte = (byte)registers[xIdx];
                xIdx++;
            }

            /* unlock CRTC registers */
            CRTController_Index.Byte = 0x03;
            CRTController_Data.Byte = (byte)(CRTController_Data.Byte | 0x80);
            CRTController_Index.Byte = 0x11;
            CRTController_Data.Byte = (byte)(CRTController_Data.Byte & ~0x80);

            /* make sure they remain unlocked */
            registers[0x03] |= 0x80;
            registers[0x11] &= ~0x80;

            /* write CRTC regs */
            for (i = 0; i < NumCRTCRegs; i++)
            {
                CRTController_Index.Byte = i;
                CRTController_Data.Byte = (byte)registers[xIdx];
                xIdx++;
            }

            /* write GRAPHICS CONTROLLER regs */
            for (i = 0; i < NumGCRegs; i++)
            {
                GraphicsController_Index.Byte = i;
                GraphicsController_Data.Byte = (byte)registers[xIdx];
                xIdx++;
            }

            /* write ATTRIBUTE CONTROLLER regs */
            for (i = 0; i < NumACRegs; i++)
            {
                var xDoSomething = Instat_Read.Byte;
                AttributeController_Index.Byte = i;
                AttributeController_Write.Byte = (byte)registers[xIdx];
                xIdx++;
            }

            /* lock 16-color palette and unblank display */
            var xNothing = Instat_Read.Byte;
            AttributeController_Index.Byte = 0x20;
        }

        public void SetPaletteEntry(int index, byte r, byte g, byte b)
        {
            DACIndex_Write.Byte = (byte)index;
            DAC_Data.Byte = (byte)(r >> 2);
            DAC_Data.Byte = (byte)(g >> 2);
            DAC_Data.Byte = (byte)(b >> 2);
        }

        public void SetPixel_8(uint x, uint y, byte color)
        {
            var xOffset = (uint)((y * Width) + x);
            VideoMemory[xOffset] = color;
        }

        public void SetPixel_640_480(uint x, uint y, byte color)
        {
            int wd_in_bytes = width / 8;
            int off = (int)((wd_in_bytes * y) + (int)(x / 8));
            int posx = (int)((x & 7) * 1);
            int mask = 0x80 >> posx;
            int pmask = 1;
            for (int p = 0; p < 4; p++)
            {
                Setplane(p);
                if ((pmask & color) != 0)
                {
                    VideoMemory[(uint)off] |= (byte)mask;
                }
                else
                {
                    VideoMemory[(uint)off] &= (byte)~mask;
                }
                pmask <<= 1;
            }
        }

        private void Setplane(int p)
        {
            byte pmask;
            int p2 = p & 3;
            pmask = (byte)(1 << p2);

            GraphicsController_Index.Byte = 4;
            GraphicsController_Data.Byte = (byte)p2;

            Sequencer_Index.Byte = 2;
            Sequencer_Data.Byte = pmask;
        }

        /**
        * Clears the VGA screen
        */
        public void Clear()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    SetPixel_640_480(x, y, 0x0F);
                }
            }
        }
    }

    public unsafe class Modes
    {
        public static short[] g_320x200x256 = 
        {
            /* MISC */
	        0x63,
            /* SEQ */
	        0x03, 0x01, 0x0F, 0x00, 0x0E,
            /* CRTC */
	        0x5F, 0x4F, 0x50, 0x82, 0x54, 0x80, 0xBF, 0x1F,
            0x00, 0x41, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x9C, 0x0E, 0x8F, 0x28, 0x40, 0x96, 0xB9, 0xA3,
            0xFF,
            /* GC */
	        0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x05, 0x0F,
            0xFF,
            /* AC */
	        0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
            0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            0x41, 0x00, 0x0F, 0x00, 0x00
        };

        public static short[] g_640x480x16 = 
        {
            /* MISC */
			0xE3,
			/* SEQ */
			0x03, 0x01, 0x08, 0x00, 0x06,
			/* CRTC */
			0x5F, 0x4F, 0x50, 0x82, 0x54, 0x80, 0x0B, 0x3E,
            0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xEA, 0x0C, 0xDF, 0x28, 0x00, 0xE7, 0x04, 0xE3,
            0xFF,
			/* GC */
			0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x05, 0x0F,
            0xFF,
			/* AC */
			0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x14, 0x07,
            0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
            0x01, 0x00, 0x0F, 0x00, 0x00
        };

        public static short[] g_720x480x256 =
        {
            /* MISC
             * 1110 0111b
             * I/OAS    --> 1
             * RAM En.  --> 1
             * Clock    --> 01 (28Mhz)
             * O/E Page --> High page
             * HSYNCP   --> Negative Horizontal Sync Polarity
             * VSYNCP   --> Negative Vertical Sync Polarity
             */
	        0xE7,
            /* SEQ */
	        0x03, 0x01, 0x08, 0x00, 0x06,
            /* CRTC */
	        0x6B, 0x59, 0x5A, 0x82, 0x60, 0x8D, 0x0B, 0x3E,
            0x00, 0x40, 0x06, 0x07, 0x00, 0x00, 0x00, 0x00,
            0xEA, 0x0C, 0xDF, 0x2D, 0x08, 0xE8, 0x05, 0xE3,
            0xFF,
            /* GC */
	        0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x05, 0x0F,
            0xFF,
            /* AC */
	        0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
            0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            0x01, 0x00, 0x0F, 0x00, 0x00
        };
    }
}