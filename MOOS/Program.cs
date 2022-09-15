//#define NETWORK

using Internal.Runtime.CompilerHelpers;
using MOOS;
using MOOS.Driver;
using MOOS.FS;
using MOOS.Graph;
using MOOS.GUI;
using MOOS.Misc;
using MOOS.NET;
using MOOS.GUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MOOS.NET.IPv4.UDP.DHCP;
using MOOS.NET.Config;
using System.Windows;
using System.Desktops;
using MOOS.NET.IPv4.TCP;
using MOOS.NET.IPv4;

unsafe class Program
{
    static void Main() { }

    [DllImport("*")]
    public static extern void test();

    public static FPSMeter fpsMeter;

    /*
     * Minimum system requirement:
     * 1024MiB of RAM
     * Memory Map:
     * 256 MiB - 512MiB   -> System
     * 512 MiB - ∞     -> Free to use
     */
    //Check out Kernel/Misc/EntryPoint.cs
    [RuntimeExport("KMain")]
    static void KMain()
    {
        Animator.Initialize();

        fpsMeter = new FPSMeter();

        Console.WriteLine("Use Native AOT (Core RT) Technology.");

        Audio.Initialize();
        AC97.Initialize();
        ES1371.Initialize();

    /*    
        //Network Config
        Network.Initialize();
        NetworkStack.Initialize();

        Thread task = new Thread(&onHandleInterrupt);
        task.Start();

        if (NetworkDevice.Devices.Count > 0)
        {
            //Send a DHCP Discover packet 
            //This will automatically set the IP config after DHCP response
            DHCPClient xClient = new DHCPClient();
            xClient.SendDiscoverPacket();

            Console.WriteLine($"[MACAddress] {NetworkDevice.Devices[0].MACAddress}");
            Console.WriteLine($"[CurrentAddress] {NetworkConfiguration.CurrentAddress}");
        }
    */
        SMain();
    }

    static void onHandleInterrupt()
    {
        for (; ; )
        {
            Interrupts.Update(); //CALL HandleInterrupt
            Native.Hlt();
        }
    }

    public static void SMain()
    {
        Framebuffer.TripleBuffered = true;

        DesktopManager.Initialize();

        for (; ; )
        {
            Control.OnUpdate();
            WindowManager.InputAll();

            //UIKernel
            DesktopManager.Update();
            WindowManager.UpdateAll();
            NotificationManager.Update();
            CursorManager.Update();

            DesktopManager.Draw();
            WindowManager.DrawAll();

            //Mouse
            Framebuffer.Graphics.DrawImage(Control.MousePosition.X, Control.MousePosition.Y, CursorManager.GetCursor, true);
            Framebuffer.Update();

            fpsMeter.Update();
        }
    }
}

