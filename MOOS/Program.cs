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

        Serial.WriteLine("Hello World");
        Console.WriteLine("Hello, World!");
        Console.WriteLine("Use Native AOT (Core RT) Technology.");

        Audio.Initialize();
        AC97.Initialize();
        ES1371.Initialize();

        //Network Config
        Network.Initialize();
        NetworkStack.Initialize();

        if (NetworkDevice.Devices.Count > 0)
        {
            // Send a DHCP Discover packet 
            //This will automatically set the IP config after DHCP response
            DHCPClient xClient = new DHCPClient();
            xClient.SendDiscoverPacket();

            Console.WriteLine($"[MACAddress] {NetworkDevice.Devices[0].MACAddress}");
            Console.WriteLine($"[CurrentAddress] {NetworkConfiguration.CurrentAddress.ToString()}");

        }

        SMain();
    }

    public static void SMain()
    {
        Framebuffer.TripleBuffered = true;

        DesktopManager.Initialize();

        #region Animation of entering Desktop
        Framebuffer.Graphics.DrawImage((Framebuffer.Width / 2) - (DesktopManager.Wallpaper.Width / 2), (Framebuffer.Height / 2) - (DesktopManager.Wallpaper.Height / 2), DesktopManager.Wallpaper, false);
        DesktopManager.Update();
        WindowManager.DrawAll();
        Framebuffer.Graphics.DrawImage(Control.MousePosition.X, Control.MousePosition.Y, CursorManager.GetCursor);
        Image _screen = Framebuffer.Graphics.Save();
        Framebuffer.Graphics.Clear(0x0);

        var SizedScreens = new Image[60];
        int startat = 40;
        for (int i = startat; i < SizedScreens.Length; i++)
        {
            SizedScreens[i] = _screen.ResizeImage(
                (int)(_screen.Width * (i / ((float)SizedScreens.Length))),
                (int)(_screen.Height * (i / ((float)SizedScreens.Length)))
                );
        }

        Animation ani = new Animation()
        {
            Value = startat + 1,
            MinimumValue = startat + 1,
            MaximumValue = SizedScreens.Length - 1,
            ValueChangesInPeriod = 1,
            PeriodInMS = 16
        };
        Animator.AddAnimation(ani);
        while (ani.Value < SizedScreens.Length - 1)
        {
            int i = ani.Value;
            Image img = SizedScreens[i];
            Framebuffer.Graphics.Clear(0x0);
            Framebuffer.Graphics.ADrawImage(
                (Framebuffer.Graphics.Width / 2) - (img.Width / 2),
                (Framebuffer.Graphics.Height / 2) - (img.Height / 2),
                img, (byte)(255 * (i / (float)(SizedScreens.Length - startat))));
            Framebuffer.Update();
        }
        Animator.DisposeAnimation(ani);

        _screen.Dispose();
        for (int i = 0; i < SizedScreens.Length; i++) SizedScreens[i]?.Dispose();
        SizedScreens.Dispose();
        #endregion

        for (; ; )
        {
            Control.OnUpdate();
            WindowManager.InputAll();

            //UIKernel
            DesktopManager.Update();
            DesktopManager.Draw();
            WindowManager.UpdateAll();
            WindowManager.DrawAll();
            //NotificationManager.Update();
            CursorManager.Update();

            //Mouse
            Framebuffer.Graphics.DrawImage(Control.MousePosition.X, Control.MousePosition.Y, CursorManager.GetCursor, true);
            Framebuffer.Update();

            fpsMeter.Update();
        }
    }
}

