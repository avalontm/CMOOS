using System;
using System.Collections.Generic;

namespace MOOS
{
    public static class Keyboard
    {
        public static ConsoleKeyInfo KeyInfo;

        public static event EventHandler<ConsoleKeyInfo> OnKeyChanged
        {
            add
            {
                _KeyKeyChangeds.Add(value);
            }

            remove
            {
                _KeyKeyChangeds.Remove(value);
            }
        }

        static List<EventHandler<ConsoleKeyInfo>> _KeyKeyChangeds;
        static List<EventHandler<ConsoleKeyInfo>> KeyKeyChangeds { get { return _KeyKeyChangeds; } }

        public static void Initialize()
        {
            _KeyKeyChangeds = new List<EventHandler<ConsoleKeyInfo>>();
        }

        public static void InvokeOnKeyChanged(ConsoleKeyInfo info) 
        {
            for (int i = 0; i < KeyKeyChangeds.Count; i++)
            {
                KeyKeyChangeds[i]?.Invoke(KeyKeyChangeds, info);
            }
        }

        public static void CleanKeyInfo(bool NoModifiers = false)
        {
            KeyInfo.KeyChar = '\0';
            KeyInfo.ScanCode = 0;
            KeyInfo.KeyState = ConsoleKeyState.None;
            if (!NoModifiers)
            {
                KeyInfo.Modifiers = ConsoleModifiers.None;
            }
        }
    }
}
