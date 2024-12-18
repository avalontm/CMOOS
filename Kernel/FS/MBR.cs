﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MOOS.FS
{
    internal class MBR
    {
        /*
        protected Storage mDisk;
        protected IList<Partition> mPartitions;

        internal IList<Partition> PartInfo
        { get { return mPartitions; } }

        internal MBR(Storage aDisk)
        {
            mDisk = aDisk;
            mPartitions = new IList<Partition>();

            var aMBR = new byte[512];
            mDisk.Read(0U, 1U, aMBR);
            ParseData(aMBR, 446);
            ParseData(aMBR, 462);
            ParseData(aMBR, 478);
            ParseData(aMBR, 494);
        }

        private void ParseData(byte[] aMBR, Int32 aLoc)
        {
            byte xSystemID = aMBR[aLoc + 4];
            if (xSystemID == 0x5 || xSystemID == 0xF || xSystemID == 0x85)
            {
                // Extended Partition Detected
                // DOS only knows about 05, Windows 95 introduced 0F, Linux introduced 85
                // Search for logical volumes
                // http://thestarman.pcministry.com/asm/mbr/PartTables2.htm
                Debug.Write("[MBR]: EBR Partition Found!\n");
            }
            else if (xSystemID != 0)
            {
                UInt32 xSectorCount = BitConverter.ToUInt32(aMBR, aLoc + 12);
                UInt32 xStartSector = BitConverter.ToUInt32(aMBR, aLoc + 8);
                mPartitions.Add(new Partition(mDisk, xStartSector, xSectorCount));
            }
        }

        */
    }
}
