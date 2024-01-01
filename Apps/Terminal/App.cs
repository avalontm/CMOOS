using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime;
using Terminal.Managers;
using System.Runtime.InteropServices;
using System.Media;

namespace Terminal
{
    public unsafe class App : Application
    {
        #region NativeMethods
        [RuntimeExport("malloc")]
        public static nint malloc(ulong size) => Allocate(size);

        [DllImport("Allocate")]
        public static extern nint Allocate(ulong size);

        [DllImport("ReadAllBytes")]
        public static extern void ReadAllBytes(string name, out ulong size, out byte* data);

        [DllImport("Lock")]
        public static extern void ALock();

        [DllImport("Unlock")]
        public static extern void AUnlock();

        [RuntimeExport("Lock")]
        public static void Lock() => ALock();

        [RuntimeExport("Unlock")]
        public static void Unlock() => AUnlock();

        [DllImport("DebugWrite")]
        public static extern void ADebugWrite(char c);

        [DllImport("DebugWriteLine")]
        public static extern void ADebugWriteLine();

        [RuntimeExport("DebugWrite")]
        public static void DebugWrite(char c) => ADebugWrite(c);

        [RuntimeExport("DebugWriteLine")]
        public static void DebugWriteLine() => ADebugWriteLine();

        [DllImport("ConsoleWrite")]
        public static extern void AConsoleWrite(char c);

        [DllImport("ConsoleWriteLine")]
        public static extern void AConsoleWriteLine();

        [RuntimeExport("ConsoleWrite")]
        public static void ConsoleWrite(char c) => AConsoleWrite(c);

        [RuntimeExport("ConsoleWriteLine")]
        public static void ConsoleWriteLine() => AConsoleWriteLine();

        [DllImport("Free")]
        public static extern ulong AFree(nint ptr);

        [RuntimeExport("free")]
        public static ulong free(nint ptr) => AFree(ptr);

        [RuntimeExport("__security_cookie")]
        public static void SecurityCookie()
        {
        }

        #endregion

        Process process = null;

        public App()
        {
            Console.Clear();

            Console.WriteLine($"CMOOS [Version 1.0.0.0] ");
            Console.WriteLine($"(c) AvalonTM. Todos los derechos reservados.");
            Console.WriteLine();

            onLoop();
        }

        void onLoop()
        {
            while (GetProcess(processID) != IntPtr.Zero)
            {
                Console.Write("cmoos>");
                string s = Console.ReadLine(6);
                onCommand(s);
            }
        }

        void onCommand(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return;
            }

            if (!string.IsNullOrEmpty(s))
            {
                Console.WriteLine();
            }

            string[] args = s.Split(' ');

            switch (args[0].ToLower())
            {
                case "play":
                    onAudioPlay();
                    break;
                case "info":
                    onSystemInfo();
                    break;
                case "time":
                    onGetTime();
                    break;
                case "pid":
                    Console.WriteLine("Proceso: " + processID);
                    break;
                case "reboot":
                    PowerManger.Reboot();
                    break;
                case "shutdown":
                    PowerManger.ShutDown();
                    break;
                case "cls":
                    Console.Clear();
                    break;
                case "tasklist":
                    onTaskList();
                    break;
                case "kill":
                    onTaskKill(args);
                    break;
                case "exit":
                    onTerminate();
                    return;
                default:
                    if (!string.IsNullOrEmpty(s))
                    {
                        process = Shell.Start(s);

                        if (process == null)
                        {
                            Console.WriteLine(@"""" + s + @"""" + " no se reconoce como un comando interno o externo,\nprograma o archivo por lotes ejecutable.");
                        }
                        else
                        {
                            while (GetProcess(process.ProcessID) != IntPtr.Zero)
                            {

                            }

                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Clear();

                            return;
                        }
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(s))
            {
                Console.WriteLine();
            }
        }

        void onAudioPlay()
        {
            Console.WriteLine($"HasAudioDevice: {MoosNative.HasAudioDevice()}");
            Wav wav = new Wav("sys/sounds/shutdown.wav");
            wav.Play();
        }

        void onGetTime()
        {
            ulong time = MoosNative.GetTime();

            ulong hourMask = 0xFF000000; // Máscara para aislar la hora
            ulong minuteMask = 0x00FF0000; // Máscara para aislar los minutos

            //Obtiene la hora y los minutos utilizando las máscaras y desplazamientos
            int hour = (int)((time & hourMask) >> 24);
            int minute = (int)((time & minuteMask) >> 16);

            Console.WriteLine($"{hour}:{(minute < 10 ? "0" : "")}{minute}");
        }

        void onTaskKill(string[] args)
        {
            if (args.Length <= 1)
            {
                Console.WriteLine("Error: Sintaxis incorrecta.");
                Console.WriteLine("Escriba: kill processID");
                return;
            }

            uint processID = Convert.ToUInt32(args[1]);

            bool status = KillProcess(processID);

            if (status)
            {
                if (process != null && process.ProcessID == processID)
                {
                    process = null;
                }
            }
            else
            {
                Console.WriteLine(@"ERROR: no se encontro el proceso """ + processID + @""".");
            }
        }

        void onTaskList()
        {
            Console.WriteLine($"Nombre de imagen               PID Nombre de sesión Núm. de ses Uso de memor");
            Console.WriteLine($"========================= ======== ================ =========== ============");
        }

        void onTerminate()
        {
            if (KillProcess(processID))
            {
                if (process != null)
                {
                    if (KillProcess(process.ProcessID))
                    {
                        process = null;
                    }
                }
            }
        }

        void onSystemInfo()
        {
            Console.WriteLine("Nombre de host:                            CMOOS");
            Console.WriteLine("Nombre del sistema operativo:              CMOOS 1.0 Alpha");
            Console.WriteLine("Tipo de compilación del sistema operativo: Multiprocessor Free");
            Console.WriteLine("Tipo de sistema:                           x64-based PC");
        }
    }
}
