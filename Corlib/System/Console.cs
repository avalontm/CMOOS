using System.Runtime.InteropServices;

namespace System
{
    public static class Console
    {
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

        [DllImport("*")]
        static extern void ConsoleWrite(char c);

        [DllImport("*")]
        static extern void ConsoleWriteLine();
    }
}
