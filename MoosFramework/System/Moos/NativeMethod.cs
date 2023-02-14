using System;
using System.Runtime;
using System.Runtime.InteropServices;

public static unsafe class NativeMethod
{
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

    [DllImport("Free")]
    public static extern ulong AFree(nint ptr);

    [RuntimeExport("free")]
    public static ulong free(nint ptr) => AFree(ptr);

    [DllImport("CreateWindow")]
    public static extern IntPtr CreateWindow(int X, int Y, int Width, int Height, string Title);

    [DllImport("GetWindowScreenBuf")]
    public static extern IntPtr GetWindowScreenBuf(IntPtr handle);

    [DllImport("BindOnKeyChangedHandler")]
    public static extern void BindOnKeyChangedHandler(IntPtr handler);

    [DllImport("CreateButton")]
    public static extern void CreateButton(IntPtr handle, int x, int y, int width, int height, string content, uint background, IntPtr command, IntPtr command_parameter);
    [DllImport("MessageBox")]
    public static extern void MessageBox(string title, string message);
}