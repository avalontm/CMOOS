using Internal.Runtime.CompilerServices;
using MOOS.Driver;
using MOOS.FS;
using MOOS.Misc;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime;
using static IDT;
using static Internal.Runtime.CompilerHelpers.InteropHelpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Image = System.Drawing.Image;
using MOOS.Api;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Moos.Core.System.Windows;

namespace MOOS
{
    public static unsafe class API
    {
        internal static List<Process> process { private set; get; } = new List<Process>();

        public static unsafe void* HandleSystemCall(string name)
        {
            switch (name)
            {
                case "GetCurrentProcess":
                    return (delegate*<uint>)&API_GetCurrentProcess;
                case "KillProcess":
                    return (delegate*<uint, bool>)&API_KillProcess;
                case "GetProcess":
                    return (delegate*<uint, IntPtr>)&API_GetProcess;
                case "ApplicationCreate":
                    return (delegate*<IntPtr, uint>)&API_ApplicationCreate;
                case "_GUI":
                    return (delegate*<void>)&API_GUI;
                case "_getGUI":
                    return (delegate*<bool>)&API_GetGUI;
                case "LoadPNG":
                    return (delegate*<string, IntPtr>)&API_LoadPNG;
                case "GetWindowScreenBuf":
                    return (delegate*<IntPtr, IntPtr>)&API_GetWindowScreenBuf;
                case "WriteLine":
                    return (delegate*<void>)&API_WriteLine;
                case "DebugWriteLine":
                    return (delegate*<void>)&API_DebugWriteLine;
                case "ConsoleWriteLine":
                    return (delegate*<void>)&API_ConsoleWriteLine;
                case "ConsoleClear":
                    return (delegate*<void>)&API_ConsoleClear;
                case "ConsoleSetBackgroundColor":
                     return (delegate*<uint, void>)&API_ConsoleSetBackgroundColor;
                case "ConsoleGetBackgroundColor":
                    return (delegate*<uint>)&API_ConsoleGetBackgroundColor;
                case "ConsoleSetForegroundColor":
                    return (delegate*<uint, void>)&API_ConsoleSetForegroundColor;
                case "ConsoleGetForegroundColor":
                    return (delegate*<uint>)&API_ConsoleGetForegroundColor;
                case "Allocate":
                    return (delegate*<ulong, nint>)&API_Allocate;
                case "Reallocate":
                    return (delegate*<nint, ulong, nint>)&API_Reallocate;
                case "Free":
                    return (delegate*<nint, ulong>)&API_Free;
                case "Sleep":
                    return (delegate*<ulong, void>)&API_Sleep;
                case "GetTick":
                    return (delegate*<ulong>)&API_GetTick;
                case "CreateDirectory":
                    return (delegate*<string, void>)&API_CreateDirectory;
                case "ReadAllBytes":
                    return (delegate*<string, ulong*, byte**, void>)&API_ReadAllBytes;
                case "WriteAllBytes":
                    return (delegate*<string, ulong, byte*, void>)&API_WriteAllBytes;
                case "Write":
                    return (delegate*<char, void>)&API_Write;
                case "DebugWrite":
                    return (delegate*<char, void>)&API_DebugWrite;
                case "ConsoleWrite":
                    return (delegate*<char, void>)&API_ConsoleWrite;
                case "ConsoleReadLine":
                    return (delegate*<byte**, void>)&API_ConsoleReadLine;
                case "ConsoleReadKey":
                    return (delegate*<byte>)&API_ConsoleReadKey;
                case "ConsoleReadLineWithStart":
                    return (delegate*<int, byte**, void>)&API_ConsoleReadLineWithStart;
                case "SwitchToMode":
                    return (delegate*<bool, void>)&API_SwitchToMode;
                case "DrawPoint":
                    return (delegate*<int, int, uint, void>)&API_DrawPoint;
                case "Lock":
                    return (delegate*<void>)&API_Lock;
                case "Unlock":
                    return (delegate*<void>)&API_Unlock;
                case "Clear":
                    return (delegate*<uint, void>)&API_Clear;
                case "NativeHlt":
                    return (delegate*<void>)&API_NativeHlt;
                case "NativeCli":
                    return (delegate*<void>)&API_NativeCli;
                case "NativeSti":
                    return (delegate*<void>)&API_NativeSti;
                case "Update":
                    return (delegate*<void>)&API_Update;
                case "Width":
                    return (delegate*<uint>)&API_Width;
                case "Height":
                    return (delegate*<uint>)&API_Height;
                case "WriteString":
                    return (delegate*<string, void>)&API_WriteString;
                case "GetTime":
                    return (delegate*<ulong>)&API_GetTime;
                case "GetCPU":
                    return (delegate*<uint>)&API_Get_CPUUsage;
                case "GetMemory":
                    return (delegate*<ulong>)&API_GetMemory;
                case "Error":
                    return (delegate*<string, bool, void>)&API_Error;
                case "StartThread":
                    return (delegate*<delegate*<void>, uint>)&API_StartThread;
                case "StartThreadWithParameters":
                    return (delegate*<delegate*<void>, IntPtr, IntPtr>)&API_StartThreadWithParameters;
                case "BindOnKeyChangedHandler":
                    return (delegate*<IntPtr, void>)&API_BindOnKeyChangedHandler;
                case "Calloc":
                    return (delegate*<ulong, ulong, void*>)&API_Calloc;
                case "ShutDown":
                    return (delegate*<void>)&API_ShutDown;
                case "Reboot":
                    return (delegate*<void>)&API_Reboot;
                case "GetPanic":
                    return (delegate*<bool>)&API_GetPanic;
            }

            #region API Controls

            //Call RTC
            void* _rtc = APIRTC.HandleSystemCall(name);

            if (_rtc != null)
            {
                return _rtc;
            }

            //Call MOUSE
            void* _mouse = MOUSE.HandleSystemCall(name);

            if (_mouse != null)
            {
                return _mouse;
            }

            //Call GDI 
            void* _gdi = GDI.HandleSystemCall(name);

            if (_gdi != null)
            {
                return _gdi;
            }

            //Call FONT
            void* _font = FONT.HandleSystemCall(name);

            if (_font != null)
            {
                return _font;
            }

            void* _snd = AUDIO.HandleSystemCall(name);

            if (_snd != null)
            {
                return _snd;
            }

            Panic.Error($"System call \"{name}\" is not found");
            return null;
        }

        private static void API_ConsoleSetBackgroundColor(uint color)
        {
            Console.BackgroundColor = (ConsoleColor)color;
        }

        public static uint API_ConsoleGetBackgroundColor()
        {
          return  (uint)Console.BackgroundColor;
        }

        private static void API_ConsoleSetForegroundColor(uint color)
        {
            Console.ForegroundColor = (ConsoleColor)color;
        }

        public static uint API_ConsoleGetForegroundColor()
        {
            return (uint)Console.ForegroundColor;
        }

        public static void API_NativeHlt()
        {
            Native.Hlt();
        }

        public static void API_NativeCli()
        {
            Native.Cli();
        }

        public static void API_NativeSti()
        {
            Native.Sti();
        }

        public static void API_ShutDown()
        {
            Power.Shutdown();
        }

        public static void API_Reboot()
        {
            Power.Reboot();
        }
      
        public static bool API_GetPanic()
        {
            return Panic.isPanic;
        }

        public static void API_GUI()
        {
            Framebuffer.TripleBuffered = true;
        }

        public static bool API_GetGUI()
        {
            return Framebuffer.TripleBuffered;
        }

        public static IntPtr API_GetWindowScreenBuf(IntPtr handle)
        {
            //PortableApp papp = Unsafe.As<IntPtr, PortableApp>(ref handle);
            return IntPtr.Zero;
        }

        static IntPtr API_GetProcess(uint processID)
        {
            if (!Panic.isPanic)
            {
                for (int i = 0; i < process.Count; i++)
                {
                    if (process[i] != null && process[i].ProcessID == processID)
                    {
                        if (ThreadPool.Threads[i].State != ThreadState.Dead)
                        {
                            return (IntPtr)process[i];
                        }
                    }
                }
            }
            return IntPtr.Zero;
        }

        private static bool API_KillProcess(uint processID)
        {
            for(int i = 0; i < process.Count; i++)
            {
                if (process[i].ProcessID == processID)
                {
                    ThreadPool.Threads[i].State = ThreadState.Dead;
                    process[i].Dispose();
                    process[i] = null;
                    ThreadPool.Schedule_Next();
                    return true;
                }
            }
            return false;
        }

        public static uint API_GetCurrentProcess()
        {
            for(int i = process.Count -1; i >= 0; i--)
            {
                if (process[i] != null)
                {
                    if (ThreadPool.Threads[i].State != ThreadState.Dead)
                    {
                        return process[i].ProcessID;
                    }
                }
            }
            return -1;
        }

        public static uint API_ApplicationCreate(IntPtr handler)
        {
            Process _process = process[process.Count - 1];
            _process.Handler = handler;
            return _process.ProcessID;
        }

        public static IntPtr API_LoadPNG(string file)
        {
            return new PNG(file);
        }

        public static void* API_Calloc(ulong num, ulong size)
        {
            return stdlib.calloc(num, size);
        }

        public static void API_BindOnKeyChangedHandler(IntPtr handler)
        {
            EventHandler<ConsoleKeyInfo> keyboard = Unsafe.As<IntPtr, EventHandler<ConsoleKeyInfo>>(ref handler);

            Keyboard.OnKeyChanged +=  keyboard;
        }

        public static uint API_StartThread(delegate*<void> func)
        {
            Thread thread = new Thread(func).Start();
            return thread.ProcessID;
        }

        public static IntPtr API_StartThreadWithParameters(delegate*<void> func, IntPtr handler)
        {
            Thread thread = new Thread(func).Start();
            Process _process = Unsafe.As<IntPtr, Process>(ref handler);
            _process.ProcessID = thread.ProcessID;
            process.Add(_process);
            return (IntPtr)_process;
        }

        public static void API_Error(string s, bool skippable)
        {
            Panic.Error(s, skippable);
        }

        public static ulong API_GetTime()
        {
            ulong century = RTC.Century;
            ulong year = RTC.Year;
            ulong month = RTC.Month;
            ulong day = RTC.Day;
            ulong hour = RTC.Hour;
            ulong minute = RTC.Minute;
            ulong second = RTC.Second;

            ulong time = 0;

            time |= century << 56;
            time |= year << 48;
            time |= month << 40;
            time |= day << 32;
            time |= hour << 24;
            time |= minute << 16;
            time |= second << 8;

            return time;
        }


        public static uint API_Get_CPUUsage()
        {
            return ThreadPool.CPUUsage;
        }

        public static ulong API_GetMemory()
        {
            return Allocator.MemoryInUse;
        }

        public static void API_WriteString(string s)
        {
            Console.Write(s);
            s.Dispose();
        }

        public static uint API_Width() => Framebuffer.Width;

        public static uint API_Height() => Framebuffer.Height;


        public static void API_Update()
        {
            Framebuffer.Update();
        }

        public static void API_Clear(uint color) => Framebuffer.Graphics.Clear(color);

        [RuntimeExport("DebugWrite")]
        public static void API_DebugWrite(char c)
        {
            Serial.Write(c);
        }

        [RuntimeExport("ConsoleWrite")]
        public static void API_ConsoleWrite(char c)
        {
            Console.Write(c);
        }
        
        [RuntimeExport("DebugWriteLine")]
        public static void API_DebugWriteLine()
        {
            Serial.WriteLine();
        }

        [RuntimeExport("ConsoleWriteLine")]
        public static void API_ConsoleWriteLine()
        {
            Console.WriteLine();
        }

        public static void API_ConsoleClear()
        {
            Console.Clear();
        }

        public static byte API_ConsoleReadKey()
        {
            var key = Console.ReadKey();

            return (byte)key.Key;
        }

        public static void API_ConsoleReadLine(byte** data)
        {
            string buffer = Console.ReadLine();

            ulong length = buffer.Length;
            *data = (byte*)Allocator.Allocate((ulong)length);
       
            fixed (byte* p = UTF8Encoding.UTF8.GetBytes(buffer))
            {
                Native.Movsb(*data, p, length);
            }
            buffer.Dispose();
        }

        private static void API_ConsoleReadLineWithStart(int start, byte** data)
        {
            string buffer = Console.ReadLine(start);

            ulong length = buffer.Length;
            *data = (byte*)Allocator.Allocate((ulong)length);

            fixed (byte* p = UTF8Encoding.UTF8.GetBytes(buffer))
            {
                Native.Movsb(*data, p, length);
            }
            buffer.Dispose();
        }

        [RuntimeExport("Lock")]
        public static void API_Lock()
        {
            if (ThreadPool.CanLock)
            {
                if (!ThreadPool.Locked)
                {
                    ThreadPool.Lock();
                }
            }
        }

        [RuntimeExport("Unlock")]
        public static void API_Unlock()
        {
            if (ThreadPool.CanLock)
            {
                if (ThreadPool.Locked)
                {
                    if (ThreadPool.Locker == SMP.ThisCPU)
                    {
                        ThreadPool.UnLock();
                    }
                }
            }
        }

        public static void API_DrawPoint(int x, int y, uint color)
        {
            Framebuffer.Graphics.DrawPoint(x, y, color);
        }

        public static void API_SwitchToMode(bool gui)
        {
            Framebuffer.TripleBuffered = gui;
        }

        public static void API_CreateDirectory(string name)
        {
            File.Instance.CreateDirectory(name);
        }

        public static void API_ReadAllBytes(string name, ulong* length, byte** data)
        {
            byte[] buffer = RamFile.Instance.ReadAllBytes(name);

            if(buffer == null)
            {
                return;
            }

            *data = (byte*)Allocator.Allocate((ulong)buffer.Length);
            *length = (ulong)buffer.Length;
            fixed (byte* p = buffer) Native.Movsb(*data, p, *length);

            buffer.Dispose();
        }

        public static void API_WriteAllBytes(string name, ulong length, byte* data)
        {
            byte[] buffer = new byte[length];

            for (int i = 0; i < length; i++)
            {
                buffer[i] = data[i];
            }

            File.Instance.WriteAllBytes(name, buffer);
            buffer.Dispose();
        }
        

        public static void API_Sleep(ulong ms)
        {
            Thread.Sleep(ms);
        }

        public static ulong API_GetTick()
        {
            return Timer.Ticks;
        }

        public static void API_Write(char c)
        {
            Console.Write(c);
        }

        public static void API_WriteLine()
        {
            Console.WriteLine();
        }

        public static nint API_Allocate(ulong size)
        {
            //Debug.WriteLine($"API_Allocate {size}");
            return Allocator.Allocate(size);
        }

        public static ulong API_Free(nint ptr)
        {
            //Debug.WriteLine($"API_Free 0x{((ulong)ptr).ToString("x2")}");
            return Allocator.Free(ptr);
        }

        public static nint API_Reallocate(nint intPtr, ulong size)
        {
            return Allocator.Reallocate(intPtr, size);
        }

    }
}

#endregion