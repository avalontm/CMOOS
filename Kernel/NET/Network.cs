using MOOS.Driver;
using MOOS.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MOOS.NET
{
    public class Network
    {
        public static void Initialize()
        {
            int NetworkDeviceID = 0;

            Console.WriteLine("Searching for Ethernet Controllers...");

            for (int i = 0; i < PCI.Devices.Count;i++)
            {
                if ((PCI.Devices[i].ClassCode == 0x02) && (PCI.Devices[i].Subclass == 0x00) && // is Ethernet Controller
                    PCI.Devices[i] == PCI.GetDevice(PCI.Devices[i].Bus, PCI.Devices[i].Slot, PCI.Devices[i].Function))
                {
                    Console.WriteLine("Found " + PCIDevice.DeviceClass.GetDeviceString(PCI.Devices[i]) + " on PCI " + PCI.Devices[i].Bus + ":" + PCI.Devices[i].Slot + ":" + PCI.Devices[i].Function);

                    #region INTELNIC
                    if (Intel8254X.GetDevice())
                    {
                        var INTELNIC = new Intel8254X(PCI.Devices[i]);

                        if (INTELNIC != null)
                        {
                            INTELNIC.NameID = ("eth" + NetworkDeviceID);
                            INTELNIC.Enable();
                            NetworkDeviceID++;
                        }
                    }
                    #endregion

                    #region PCNETII

                    if (PCI.Devices[i].VendorID == (ushort)VendorID.AMD && PCI.Devices[i].DeviceID == (ushort)DeviceID.PCNETII)
                    {
                        var AMDPCNetIIDevice = new AMDPCNetII(PCI.Devices[i]);
                        AMDPCNetIIDevice.NameID = ("eth" + NetworkDeviceID);
                        AMDPCNetIIDevice.Enable();
                        NetworkDeviceID++;
                    }

                    #endregion
                    #region RTL8139

                    if (PCI.Devices[i].VendorID == 0x10EC && PCI.Devices[i].DeviceID == 0x8139)
                    {
                        var RTL8139Device = new RTL8139(PCI.Devices[i]);
                        RTL8139Device.NameID = ("eth" + NetworkDeviceID);
                        RTL8139Device.Enable();
                        NetworkDeviceID++;
                    }

                    #endregion
                }
            }

            if (NetworkDevice.Devices.Count == 0)
            {
                Console.WriteLine("No supported network card found!!");
            }
            else
            {
                Console.WriteLine("Network initialization done!");
            }
        }
    }
}
