//#define NETWORK

using MOOS;
using MOOS.Driver;
using MOOS.FS;
using MOOS.Misc;
using MOOS.NET;
using MOOS.NET.IPv4.UDP.DHCP;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static MOOS.Misc.Interrupts;
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

        #region NETWORK

        /* 
         //Network Config (AvalonTM)
         Network.Initialize();
         NetworkStack.Initialize();

         if (NetworkDevice.Devices.Count > 0)
         {
             //Send a DHCP Discover packet 
             //This will automatically set the IP config after DHCP response
             DHCPClient xClient = new DHCPClient();
             int result = xClient.SendDiscoverPacket();

             if (result > 0)
             {
                 HttpClient http = new HttpClient("raw.githubusercontent.com", 443);
                 var response = http.GetAsync("avalontm/CMOOS/master/Api/version.json");
                 Console.WriteLine($"[RESPONSE] {response.Content}");
             }
         }
         */
       
        //Network Config (AvalonTM)
        Network.Initialize();
        NetworkStack.Initialize();

        if (NetworkDevice.Devices.Count > 0)
        {
            //Send a DHCP Discover packet 
            //This will automatically set the IP config after DHCP response
            DHCPClient xClient = new DHCPClient();
            int result = xClient.SendDiscoverPacket();
            xClient.Close();

            if (result > 0)
            {
               // HttpClient http = new HttpClient("raw.githubusercontent.com", 443);
               // var response = http.GetAsync("avalontm/CMOOS/master/Api/version.json");
               // Console.WriteLine($"[RESPONSE] {response.Content}");
            }
        }
        
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
        Console.WriteLine($"[SetResolution] 640x480");
        Console.ReadKey();
        Framebuffer.SetResolution();

        /*
        string text = "terminal = sys/app/terminal.mue\nshell = sys/app/explorer.mue";

        File.Instance.WriteAllBytes("startup.ini", Encoding.UTF8.GetBytes(text));
        string dir = "";

        Console.WriteLine($"");
        Console.WriteLine($"Volume in drive 0 is {File.Instance.VolumeLabel}");
        Console.WriteLine($"Volume Serial Number is {File.Instance.SerialNo.ToSeparatorHex()}");
        Console.WriteLine($@"Directory of drive 0 {dir}");
        Console.WriteLine($"");

        int _files = 0;
        int _dirs = 0;
        ulong _fSize = 0;

        var files = File.Instance.GetFiles(dir);
       
        for (int i = 0; i < files.Count; i++)
        {
            if (files[i].Attribute == FileAttribute.Directory)
            {
                Console.WriteLine($"{files[i].Name}     DIR");
                _dirs++;
            }
            else
            {
                _files++;
                _fSize += files[i].Size;

                Console.WriteLine($"{files[i].Name}   {files[i].Ext}    {files[i].Size.ToString()}");
            }
        }

        Console.WriteLine($"");
        Console.WriteLine($"{_files} file(s)                    {_fSize} bytes");
        Console.WriteLine($"{_dirs} dir(s)                    {(1024 * (1024*1024)-_fSize)}  bytes free");
        Console.WriteLine($"");
        */

        Console.WriteLine("Press any key to [ENTER] desktop...");
        Console.ReadKey();

        byte[] bytes = RamFile.ReadAllBytes("startup.ini");

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

                if (parts.Length > 1)
                {
                    // Añade el parámetro al diccionario
                    dictionary.Add(parts[0].Trim(), parts[1].Trim());
                }
            }
        }

        string terminal = dictionary["terminal"];
        Console.WriteLine($"execute: {terminal}");

        /*
        byte[] data = RamFile.Instance.ReadAllBytes(terminal);
        
        Console.WriteLine();

        if (data != null)
        {
            Console.WriteLine("Copy");

            File.Instance.WriteAllBytes("terminal.mue", data);
        }
        */

        var process = System.Diagnostics.Process.Start(terminal);

        if (process == null)
        {
            Console.WriteLine("cant run.");
        }

        bytes.Dispose();
        texto.Dispose();
        lineas.Dispose();
        terminal.Dispose();

        while (GetProcess(process.ProcessID) != IntPtr.Zero)
        {

        }
    }


}

