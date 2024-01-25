using MOOS.Driver;
using System;

namespace MOOS.Misc
{
    public static class Panic
    {
        public static bool isPanic {  get; private set; }

        public static void Error(string msg, bool skippable = false)
        {
            isPanic = true;
            //Kill all CPUs
            LocalAPIC.SendAllInterrupt(0xFD);
            IDT.Disable();
            Framebuffer.TripleBuffered = false;

            ConsoleColor color = Console.ForegroundColor;

            Console.ForegroundColor = System.ConsoleColor.Red;
            Console.Write("PANIC: ");
            Console.WriteLine(msg);
            Console.WriteLine("All CPU Halted Now!");

            Console.ForegroundColor = color;

            if (!skippable)
            {
                Framebuffer.Update();
                for (; ; );
            }
        }
    }
}