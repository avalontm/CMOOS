using MOOS.Misc;
using System;

namespace MOOS.Driver
{
    public static unsafe class IOAPIC
    {
        private const int IOREGSEL = 0x00;
        private const int IOWIN = 0x10;

        private const int IOAPICID = 0x00;
        private const int IOAPICVER = 0x01;
        private const int IOAPICARB = 0x02;
        private const int IOREDTBL = 0x10;
        public static uint io_apic_count = 0;

        public static void Initialize()
        {
            if (ACPI.IO_APIC == null)
            {
                Panic.Error("[I/O APIC] Can't initialize I/O APIC");
                return;
            }
           
            uint value = In(IOAPICVER);
            io_apic_count = ((value >> 16) & 0xFF) + 1;

            //Disable All Entries
            for (uint i = 0; i < io_apic_count; ++i)
            {
                SetEntry((byte)i, 1 << 16);
            }

            Console.WriteLine($"[IOAPIC] {io_apic_count}");
            Console.WriteLine("[I/O APIC] I/O APIC Initialized");
        }

        public static uint In(byte reg)
        {
            MMIO.Out32((uint*)(ACPI.IO_APIC->IOApicAddress + IOREGSEL), reg);
            return MMIO.In32((uint*)(ACPI.IO_APIC->IOApicAddress + IOWIN));
        }

        public static void Out(byte reg, uint value)
        {
            MMIO.Out32((uint*)(ACPI.IO_APIC->IOApicAddress + IOREGSEL), reg);
            MMIO.Out32((uint*)(ACPI.IO_APIC->IOApicAddress + IOWIN), value);
        }

        public static void SetEntry(byte index, ulong data)
        {
            Out((byte)(IOREDTBL + index * 2), (UInt32)data);
            Out((byte)(IOREDTBL + index * 2 + 1), (UInt32)(data >> 32));
        }

        public static void SetEntry(uint irq)
        {
            SetEntry((byte)ACPI.RemapIRQ(irq - 0x20), irq);
        }
    }
}