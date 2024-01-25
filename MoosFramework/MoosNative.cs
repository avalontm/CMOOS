using System;
using System.Runtime;
using System.Runtime.InteropServices;

public static unsafe class MoosNative
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

    [DllImport("LoadPNG")]
    public static extern IntPtr LoadPNG(string file);

    [DllImport("_GUI")]
    public static extern void ModeGUI();

    [DllImport("GetMouseX")]
    public static extern int GetMouseX();

    [DllImport("GetMouseY")]
    public static extern int GetMouseY();

    [RuntimeExport("__security_cookie")]
    public static void SecurityCookie()
    { 
    }

    [DllImport("GetTime")]
    public static extern ulong GetTime();

    [DllImport("GetMemory")]
    public static extern uint MemoryInUse();

    [DllImport("GetCPU")]
    public static extern ulong CPUUsage();

    [DllImport("SndWrite")]
    public static extern void SndWrite(byte* buffer, uint size);

    [DllImport("SndLoad")]
    public static extern bool SndLoad(string name, out ulong size, out byte* data);

    [DllImport("AudioCacheSize")]
    public static extern int AudioCacheSize();

    [DllImport("API_AudioBytesWritten")]
    public static extern int AudioBytesWritten();

    [DllImport("SndDoPlay")]
    public static extern int SndDoPlay(delegate*<void> handler);

    [DllImport("HasAudioDevice")]
    public static extern bool HasAudioDevice();

    [DllImport("Sleep")]
    public static extern bool Sleep(ulong ms);

    [DllImport("BindOnKeyChangedHandler")]
    static extern void BindOnKeyChangedHandler(IntPtr handler);

    [DllImport("GetWindowScreenBuf")]
    public static extern IntPtr GetWindowScreenBuf(IntPtr handle);

    [DllImport("GetPanic")]
    public static extern bool GetPanic();

    #endregion


    public static void SetBindOnKeyChangedHandler(EventHandler<ConsoleKeyInfo> PS2Keyboard_OnKeyChangedHandler)
    {
        IntPtr handler = (IntPtr)PS2Keyboard_OnKeyChangedHandler;
        BindOnKeyChangedHandler(handler);
    }
}

