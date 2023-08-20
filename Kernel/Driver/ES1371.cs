using MOOS.Driver;
using MOOS.Misc;
using System;
using static MOOS.Misc.MMIO;

namespace MOOS
{
    //TO-DO 44.1khz
    public static unsafe class ES1371
    {
        internal static uint Base;
        internal static byte* Buffer;
        const int CacheSize = 0xFFFFF;

        internal static void Initialize()
        {
            PCIDevice device = PCI.GetDevice(0x1274, 0x1371);
            if (device == null) return;

            device.Claimed = true;
            device.EnableDevice();

            device.WriteRegister(0x04, 0x04 | 0x02 | 0x01);

            Base = (uint)(device.Bar0 & (~0x3));
            Buffer = (byte*)Allocator.Allocate(CacheSize);
            Console.WriteLine($"[ES1371] Base: 0x{((ulong)Base).ToStringHex()}");

            Native.Out32(Base + 0x14, 0x00020000);
            Native.Out32(Base + 0x14, 0x00180000);
            Native.Out32(Base + 0x10, 0xeb403800);
            Native.Out32(Base + 0x0c, 0x0c);
            Native.Out32(Base + 0x38, (uint)Buffer);
            Native.Out32(Base + 0x3c, Audio.SizePerPacket / 4);
            Native.Out32(Base + 0x28, 0x7FFF);
            Native.Out32(Base + 0x20, 0x0020020C);
            Native.Out32(Base + 0x00, 0x00000020);

            Interrupts.EnableInterrupt(0x20, &OnInterrupt);
            Audio.HasAudioDevice = true;
        }

        public static void OnInterrupt()
        {
            uint sts = Native.In32(Base + 0x04);
            if (BitHelpers.IsBitSet(sts, 1))
            {
                Native.Out32(Base + 0x20, Native.In32(Base + 0x20) & 0xFFFFFDFF);

                Native.Stosb(Buffer, 0, CacheSize);
                Audio.require(Buffer);
            }
        }
    }
}
