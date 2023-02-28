//#define NETWORK

using System.IO;
using MOOS;
using MOOS.Driver;
using MOOS.FS;
using MOOS.GUI;
using MOOS.Misc;
using MOOS.NET;
using MOOS.NET.IPv4.UDP.DHCP;
using System.Desktops;
using System.Net.Http;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;

unsafe class Program
{
    static void Main() { }

    static bool USBMouseTest()
    {
        HID.GetMouseThings(HID.Mouse, out sbyte AxisX, out sbyte AxisY, out var Buttons);
        return Buttons != MouseButtons.None;
    }

    static bool USBKeyboardTest()
    {
        HID.GetKeyboardThings(HID.Keyboard, out var ScanCode, out var Key);
        return ScanCode != 0;
    }

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

        Hub.Initialize();
        HID.Initialize();
        EHCI.Initialize();

        if (HID.Mouse != null)
        {
            Console.Write("[Warning] Press please press Mouse any key to validate USB Mouse ");
            bool res = Console.Wait(&USBMouseTest, 2000);
            Console.WriteLine();
            if (!res)
            {
                lock (null)
                {
                    USB.NumDevice--;
                    HID.Mouse = null;
                }
            }
        }

        if (HID.Keyboard != null)
        {
            Console.Write("[Warning] Press please press any key to validate USB keyboard ");
            bool res = Console.Wait(&USBKeyboardTest, 2000);
            Console.WriteLine();
            if (!res)
            {
                lock (null)
                {
                    USB.NumDevice--;
                    HID.Keyboard = null;
                }
            }
        }

        USB.StartPolling();

        //Use qemu for USB debug
        //VMware won't connect virtual USB HIDs
        if (HID.Mouse == null)
        {
            Console.WriteLine("USB Mouse not present");
        }
        if (HID.Keyboard == null)
        {
            Console.WriteLine("USB Keyboard not present");
        }

        Audio.Initialize();
        AC97.Initialize();
        ES1371.Initialize();

        #region NETWORK
        /*       
               //Network Config (AvalonTM)
               Network.Initialize();
               NetworkStack.Initialize();

               Timer.Sleep(1000);

               if (NetworkDevice.Devices.Count > 0)
               {
                   //Send a DHCP Discover packet 
                   //This will automatically set the IP config after DHCP response
                   DHCPClient xClient = new DHCPClient();
                   xClient.SendDiscoverPacket();
                   Timer.Sleep(1000);

                   HttpClient http = new HttpClient("raw.githubusercontent.com", 443);
                   var response = http.GetAsync("avalontm/CMOOS/master/Api/version.json");
                   Console.WriteLine($"[RESPONSE] {response.Status}");
               }
             */
        #endregion

        /*
        XmlReader reader = XmlReader.Create("info.xml");

        Console.WriteLine();
        Console.WriteLine($"[RADER] Name = {reader.GetNode("name").Values[2]}");
        Console.WriteLine($"[RADER] Main = {reader.GetNode("main").Values[1]}");
        Console.WriteLine($"[RADER] Icon = {reader.GetNode("icon").Values[1]}");
        Console.WriteLine($"[RADER] Author = {reader.GetNode("author").Values[1]}");
        Console.WriteLine($"[RADER] Version = {reader.GetNode("version").Values[1]}");
        Console.WriteLine();
        */
       
        SMain();
    }

    public static void SMain()
    {
        Console.WriteLine("Press any key to [ENTER] desktop...");
        Console.ReadKey();

        Framebuffer.TripleBuffered = true;

        DesktopManager.Initialize();

        for (; ; )
        {
            Control.Update();

            //UIKernel
            DesktopManager.Update();
            WindowManager.Update();

            CursorManager.Update();

            DesktopManager.Draw();
            WindowManager.Draw();
            NotificationManager.Draw();

            //Mouse
            Framebuffer.Graphics.DrawImage(Control.MousePosition.X + Control.MouseOffSet.X, Control.MousePosition.Y + Control.MouseOffSet.Y, CursorManager.GetCursor, true);
            Framebuffer.Update();

            fpsMeter.Update();
        }
    }
}

