using MOOS.Misc;
using System;
using System.Runtime.InteropServices;

namespace MOOS
{
    public static unsafe class AVX
    {
        public static void init_avx()
        {
            CPUID* cpuid = Native.CPUID(1);
            Console.WriteLine($"[ECX] {cpuid->ECX}");
            if (!BitHelpers.IsBitSet(cpuid->ECX, 28))
            {
                Console.WriteLine("[AVX] Warning: this CPU doesn't support AVX!");
                return;
            }

            Console.WriteLine("[AVX] this CPU support AVX!");
            enable_avx();
        }

        [DllImport("*")]
        public static extern void enable_avx();

        [DllImport("*")]
        public static extern void avx_memcpy(void* pvDest, void* pvSrc, ulong nBytes);
    }
}