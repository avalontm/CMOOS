namespace System
{
    public delegate void OnKeyHandler(ConsoleKeyInfo key);

    public struct ConsoleKeyInfo
    {
        public int ScanCode;
        public ConsoleKey Key;
        public char KeyChar;
        public ConsoleModifiers Modifiers;
        public ConsoleKeyState KeyState;
    }
}