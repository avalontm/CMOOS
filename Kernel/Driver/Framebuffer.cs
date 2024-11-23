using MOOS.Driver;
using MOOS.Graph;
using MOOS.IO;
using MOOS.Memory;
using MOOS.Misc;
using MOOS.NET.ARP;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MOOS
{
    
    public static unsafe class Framebuffer
    {
        public static ushort Width;
        public static ushort Height;
        public static ushort BPP;

        public static uint* VideoMemory { get; private set; }

        public static uint* FirstBuffer;
        public static uint* SecondBuffer;

        public static Graphics Graphics;

        static bool _TripleBuffered = false;

        /// <summary>
        /// Since you enabled TripleBuffered you have to call Framebuffer.Graphics.Update() in order to make it display
        /// </summary>
        public static bool TripleBuffered 
        {
            get 
            {
                return _TripleBuffered;
            }
            set 
            {
                if (Graphics == null) return;
                if (_TripleBuffered == value) return;

                Graphics.Clear(0x0);
                Graphics.VideoMemory = value ? FirstBuffer : VideoMemory;
                _TripleBuffered = value;
                if (!_TripleBuffered)
                {
                    Console.Clear();
                }
            }
        }

        public static void Update()
        {
            /*
            if (TripleBuffered)
            {
                for(int i = 0; i < Width * Height; i++) 
                {
                    if(FirstBuffer[i] != SecondBuffer[i]) 
                    {
                        VideoMemory[i] = FirstBuffer[i];
                    }
                }
                Native.Movsd(SecondBuffer, FirstBuffer, (ulong)(Width * Height));
            }
            if(Graphics != null) Graphics.Update();*/
        }


        public static void Initialize(ushort XRes, ushort YRes, ushort bpp, uint* FB)
        {
            Width = XRes;
            Height = YRes;
            BPP = bpp;
            FirstBuffer = (uint*)Allocator.Allocate((ulong)(XRes * YRes * (bpp / 8)));
            SecondBuffer = (uint*)Allocator.Allocate((ulong)(XRes * YRes * (bpp / 8)));
            Native.Stosd(FirstBuffer, 0, (ulong)(XRes * YRes));
            Native.Stosd(SecondBuffer, 0, (ulong)(XRes * YRes));
            Mouse.Position.X = XRes / 2;
            Mouse.Position.Y = YRes / 2;
            Graphics = new Graphics(Width, Height, FB);
            VideoMemory = FB;
            Console.Clear();
        }

        public static void SetResolution()
        {
            VGAScreen vga = new VGAScreen();
            vga.SetMode0();

            Width = vga.Width;
            Height = vga.Height;
        
            vga.Clear();

        }

        public static void ChangeResolution(ushort xres, ushort yres, ushort bpp)
        {
            if (ISAModeAvailable())
            {
                DisableDisplay();
                SetXResolution(xres);
                SetYResolution(yres);
                SetDisplayBPP(bpp);
                EnableDisplay(EnableValues.Enabled | EnableValues.UseLinearFrameBuffer | EnableValues.NoClearMemory);

                Initialize(xres, yres, bpp, (uint*)new MemoryBlock(0xE0000000, xres * yres * 4).Base);

                Console.WriteLine($"Width: {Width}x{Height}x{BPP}");

            }
        }

        static bool ISAModeAvailable()
        {
            //This code wont work as long as Bochs uses BGA ISA, since it wont discover it in PCI
            // return HAL.PCI.GetDevice(VendorID.Bochs, DeviceID.BGA) != null;
            return VBERead(RegisterIndex.DisplayID) == 0xB0C5;
        }

        static void SetXResolution(ushort xres)
        {
            Console.WriteLine($"VBE Setting X resolution to {xres}");
            VBEWrite(RegisterIndex.DisplayXResolution, xres);
        }

        static void SetYResolution(ushort yres)
        {
            Console.WriteLine($"VBE Setting Y resolution to {yres}");
            VBEWrite(RegisterIndex.DisplayYResolution, yres);
        }

        static void SetDisplayBPP(ushort bpp)
        {
            Console.WriteLine($"VBE Setting BPP to {bpp}");
            VBEWrite(RegisterIndex.DisplayBPP, bpp);
        }

        static void EnableDisplay(EnableValues EnableFlags)
        {
            Console.WriteLine($"VBE Enabling display with EnableFlags (ushort){EnableFlags}");
            VBEWrite(RegisterIndex.DisplayEnable, (ushort)EnableFlags);
        }

        static void DisableDisplay()
        {
            Console.WriteLine($"Disabling VBE display");
            VBEWrite(RegisterIndex.DisplayEnable, (ushort)EnableValues.Disabled);
        }

        public const int VBEIndex = 0x01CE;
        /// <summary>
        /// Data IOPort.
        /// </summary>
        public const int VBEData = 0x01CF;

        static ushort VBERead(RegisterIndex index)
        {
            Native.Out16(VBEIndex, (ushort)index);
            return Native.In16(VBEData);
        }

        static void VBEWrite(RegisterIndex index, ushort value)
        {
            Native.Out16(VBEIndex, (ushort)index);
            Native.Out16(VBEData, value);
        }

        [Flags]
        private enum EnableValues
        {
            Disabled = 0x00,
            Enabled,
            UseLinearFrameBuffer = 0x40,
            NoClearMemory = 0x80,
        };

        private enum RegisterIndex
        {
            DisplayID = 0x00,
            DisplayXResolution,
            DisplayYResolution,
            DisplayBPP,
            DisplayEnable,
            DisplayBankMode,
            DisplayVirtualWidth,
            DisplayVirtualHeight,
            DisplayXOffset,
            DisplayYOffset
        };
    }
}