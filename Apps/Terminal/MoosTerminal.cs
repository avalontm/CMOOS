using Internal.Runtime.CompilerServices;
using Moos.Framework.Fonts;
using Moos.Framework.Graphics;
using Moos.Framework.System;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.IO;
using System.Media;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;
using Terminal.Managers;


namespace MoosExplorer
{
    public static unsafe class MoosTerminal
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

        [DllImport("StartThread")]
        public static extern void StartThread(delegate*<void> ptr);

        [DllImport("GetCurrentProcess")]
        public static extern uint GetCurrentProcess();

        [DllImport("KillProcess")]
        public static extern bool KillProcess(uint processID);
        [DllImport("GetProcess")]
        public static extern IntPtr GetProcess(uint processID);
        #endregion

        static Process process = null;
        static uint mainProcess  = 0;
 
        [RuntimeExport("Main")]
        public static void Main()
        {
            mainProcess = GetCurrentProcess();
            Console.Clear();

            Console.WriteLine($"CMOOS [Version 1.0.0.0] ");
            Console.WriteLine($"(c) AvalonTM. Todos los derechos reservados.");
            Console.WriteLine();

            while (GetProcess(mainProcess) != IntPtr.Zero)
            {
                Console.Write("cmoos>");
                string s = Console.ReadLine(6);
                onCommand(s);
            }
        }

        static void onCommand(string s)
        {
            if(string.IsNullOrEmpty(s))
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
                case "info":
                    onSystemInfo();
                    break;
                case "pid":
                    Console.WriteLine("Proceso: " + mainProcess);
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
                            while (process != null && GetProcess(process.ProcessID) != IntPtr.Zero)
                            {

                            }

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

        static void onTaskKill(string[] args)
        {
            if (args.Length <= 1)
            {
                Console.WriteLine("Error: Sintaxis incorrecta.");
                Console.WriteLine("Escriba: kill processID");
                return;
            }

            uint processID = Convert.ToUInt32(args[1]);

            bool status = KillProcess(processID);

            if(status)
            {
                if(process != null && process.ProcessID == processID)
                {
                    process = null;
                }
            }
            else
            {
                Console.WriteLine(@"ERROR: no se encontro el proceso """ + processID + @""".");
            }
        }

        static void onTaskList()
        {
            Console.WriteLine($"Nombre de imagen               PID Nombre de sesión Núm. de ses Uso de memor");
            Console.WriteLine($"========================= ======== ================ =========== ============");
        }

        static void onTerminate()
        {
            if (KillProcess(mainProcess))
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

        static void onSystemInfo()
        {
            Console.WriteLine("Nombre de host:                            CMOOS");
            Console.WriteLine("Nombre del sistema operativo:              CMOOS 1.0 Alpha");
            Console.WriteLine("Tipo de compilación del sistema operativo: Multiprocessor Free");
            Console.WriteLine("Tipo de sistema:                           x64-based PC");
        }
    }
}
