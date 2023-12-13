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

namespace MOOS
{
    public static unsafe class API
    {
        internal static Process process { set; get; }

        public static unsafe void* HandleSystemCall(string name)
        {
            switch (name)
            {
                case "ApplicationCreate":
                    return (delegate*<IntPtr, void>)&API_ApplicationCreate;
                case "_GUI":
                    return (delegate*<void>)&API_GUI;
                case "LoadPNG":
                    return (delegate*<string, IntPtr>)&API_LoadPNG;
                case "WriteLine":
                    return (delegate*<void>)&API_WriteLine;
                case "DebugWriteLine":
                    return (delegate*<void>)&API_DebugWriteLine;
                case "ConsoleWriteLine":
                    return (delegate*<void>)&API_ConsoleWriteLine;
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
                case "SwitchToConsoleMode":
                    return (delegate*<void>)&API_SwitchToConsoleMode;
                case "DrawPoint":
                    return (delegate*<int, int, uint, void>)&API_DrawPoint;
                case "Lock":
                    return (delegate*<void>)&API_Lock;
                case "Unlock":
                    return (delegate*<void>)&API_Unlock;
                case "Clear":
                    return (delegate*<uint, void>)&API_Clear;
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
                case "Error":
                    return (delegate*<string, bool, void>)&API_Error;
                case "StartThread":
                    return (delegate*<delegate*<void>, void>)&API_StartThread;
                case "StartThreadWithParameters":
                    return (delegate*<delegate*<void>, IntPtr, void>)&API_StartThreadWithParameters;
                case "BindOnKeyChangedHandler":
                    return (delegate*<EventHandler<ConsoleKeyInfo>, void>)&API_BindOnKeyChangedHandler;
                case "Calloc":
                    return (delegate*<ulong, ulong, void*>)&API_Calloc;
                case "SndWrite":
                    return (delegate*<byte*, int, int>)&API_SndWrite;
                case "ShutDown":
                    return (delegate*<void>)&API_ShutDown;
                case "Reboot":
                    return (delegate*<void>)&API_Reboot;
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

            Panic.Error($"System call \"{name}\" is not found");
            return null;
        }

        public static void API_ShutDown()
        {
            Power.Shutdown();
        }

        public static void API_Reboot()
        {
            Power.Reboot();
        }
      
        public static void API_GUI()
        {
            Framebuffer.TripleBuffered = true;
        }

        public static void API_ApplicationCreate(IntPtr handler)
        {
            IApplicationBase _base = Unsafe.As<IntPtr, IApplicationBase>(ref handler);

            if (_base != null)
            {
               _base.SetExecutablePath(process.startInfo.WorkingDirectory);
            }
        }

        public static IntPtr API_LoadPNG(string file)
        {
            return new PNG(file);
        }


        public static int API_SndWrite(byte* buffer, int len)
        {
            return Audio.snd_write(buffer, len);
        }

        public static void* API_Calloc(ulong num, ulong size)
        {
            return stdlib.calloc(num, size);
        }

        public static void API_BindOnKeyChangedHandler(EventHandler<ConsoleKeyInfo> handler)
        {
            Keyboard.OnKeyChanged += handler;

        }

        public static void API_StartThread(delegate*<void> func)
        {
            new Thread(func).Start();
        }

        public static void API_StartThreadWithParameters(delegate*<void> func, IntPtr handler)
        {
            process = new Process();
            process.startInfo = Unsafe.As<IntPtr, ProcessStartInfo>(ref handler);

            new Thread(func).Start();
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

        public static void API_SwitchToConsoleMode()
        {
            Framebuffer.TripleBuffered = false;
        }

        public static void API_CreateDirectory(string name)
        {
            File.Instance.CreateDirectory(name);
        }

        public static void API_ReadAllBytes(string name, ulong* length, byte** data)
        {
            byte[] buffer = File.Instance.ReadAllBytes(name);

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