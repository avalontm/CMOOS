//#define NETWORK

using Internal.Runtime.CompilerServices;
using Moos.Core.System.Windows;
using MOOS;
using MOOS.Driver;
using MOOS.FS;
using MOOS.Misc;
using MOOS.NET;
using MOOS.NET.IPv4.UDP.DHCP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Console = MOOS.Console;

unsafe class Program
{
    [DllImport("GetProcess")]
    public static extern IntPtr GetProcess(uint processID);

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
        BitFont.Initialize();

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

  /*
        #region NETWORK

        //Network Config (AvalonTM)
        Network.Initialize();
        NetworkStack.Initialize();


        if (NetworkDevice.Devices.Count > 0)
        {
            //Send a DHCP Discover packet 
            //This will automatically set the IP config after DHCP response
            DHCPClient xClient = new DHCPClient();
            int result = xClient.SendDiscoverPacket();

            HttpClient http = new HttpClient("raw.githubusercontent.com", 443);
            var response = http.GetAsync("avalontm/CMOOS/master/Api/version.json");
            Console.WriteLine($"[RESPONSE] {response.Content}");
            
        }
        
        #endregion
        */

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
        var files = File.Instance.GetFiles("");

        for(int i = 0; i < files.Count; i++)
        {
            if (files[i].Attribute == FileAttribute.Directory)
            {
                Console.WriteLine($"Dir: {files[i].Name}");
            }
            else
            {
                Console.WriteLine($"File: {files[i].Name}");
            }
        }
       
        Console.WriteLine("Press any key to [ENTER] desktop...");
        Console.ReadKey();

        byte[] bytes = File.ReadAllBytes("startup.ini");

        if (bytes == null)
        {
            Console.WriteLine("File not found!");
            return;
        }

        // Convierte los bytes a texto
        string texto = Encoding.UTF8.GetString(bytes);

        // Divide el texto por saltos de linea
        string[] lineas = texto.Split('\n');

        // Crea un diccionario
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        // Imprime las lineas
        for (int i = 0; i < lineas.Length; i++)
        {
            if (!string.IsNullOrEmpty(lineas[i]))
            {
                // Extrae el nombre y el valor del parámetro
                string[] parts = lineas[i].Split('=');

                if (parts.Length >= 2)
                {
                    // Añade el parámetro al diccionario
                    dictionary.Add(parts[0].Trim(), parts[1].Trim());
                }
            }
        }

        string terminal = dictionary["terminal"];
        System.Diagnostics.Process.Start($"sys/app/terminal.mue");

        bytes.Dispose();
        texto.Dispose();
        lineas.Dispose();
        terminal.Dispose();

        for (; ; )
        {

        }
    }
}

