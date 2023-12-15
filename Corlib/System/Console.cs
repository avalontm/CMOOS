using System.Runtime.InteropServices;
using System.Text;

namespace System
{
    public unsafe static class Console
    {
        [DllImport("*")]
        static extern void ConsoleWrite(char c);

        [DllImport("*")]
        static extern void ConsoleWriteLine();

        [DllImport("ConsoleClear")]
        static extern void ConsoleClear();

        [DllImport("ConsoleReadLine")]
        static extern void ConsoleReadLine(out byte* data);

        [DllImport("ConsoleReadLineWithStart")]
        static extern void ConsoleReadLineWithStart(int start, out byte* data);

        public static void WriteLine(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                ConsoleWrite(s[i]);
            }
            ConsoleWriteLine();
            s.Dispose();
        }

        public static void WriteLine()
        {
            ConsoleWriteLine();
        }

        public static void Write(char c)
        {
            ConsoleWrite(c);
        }

        public static void Write(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                ConsoleWrite(s[i]);
            }
            s.Dispose();
        }

        public static string ReadLine()
        {
            ConsoleReadLine(out var data);

            return UTF8Encoding.UTF8.GetString(data);
        }

        public static string ReadLine(int start)
        {
            ConsoleReadLineWithStart(start, out var data);

            return UTF8Encoding.UTF8.GetString(data);
        }

        public static void Clear()
        {
            ConsoleClear();
        }
    }
}
