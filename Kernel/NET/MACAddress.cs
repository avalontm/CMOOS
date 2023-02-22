using System;
using System.Collections.Generic;
using System.Common.Extentions;
using System.Diagnostics;
using System.Helpers;
using System.Runtime.InteropServices;
using System.Text;

namespace MOOS.NET
{
    public class MACAddress //: IComparable
    {
        internal byte[] bytes { get; private set; }

        static MACAddress _broadcast;
        internal static MACAddress Broadcast
        {
            get {

                if (_broadcast == null)
                {
                    var xBroadcastArray = new byte[6];
                    xBroadcastArray[0] = 0xFF;
                    xBroadcastArray[1] = 0xFF;
                    xBroadcastArray[2] = 0xFF;
                    xBroadcastArray[3] = 0xFF;
                    xBroadcastArray[4] = 0xFF;
                    xBroadcastArray[5] = 0xFF;
                    _broadcast = new MACAddress(xBroadcastArray);
                }
                return _broadcast; 
            }
        }

        static MACAddress _none;
        internal static MACAddress None {
            get
            {

                if (_none == null)
                {
                    var xNoneArray = new byte[6];
                    xNoneArray[0] = 0x00;
                    xNoneArray[1] = 0x00;
                    xNoneArray[2] = 0x00;
                    xNoneArray[3] = 0x00;
                    xNoneArray[4] = 0x00;
                    xNoneArray[5] = 0x00;
                    _none = new MACAddress(xNoneArray);
                }
                return _none;
            }
        }

        public MACAddress(byte[] address)
        {
            if (address == null || address.Length != 6)
            {
                Console.WriteLine("MACAddress is null or has wrong length");
                return;
            }

            bytes = new byte[] { address[0], address[1], address[2], address[3], address[4], address[5] };
        }

        /// <summary>
        /// Create a MAC address from a byte buffer starting at the specified offset
        /// </summary>
        /// <param name="buffer">byte buffer</param>
        /// <param name="offset">offset in buffer to start from</param>
        public MACAddress(byte[] buffer, int offset)
        {
            if (buffer == null || buffer.Length < (offset + 6))
            {
                Console.WriteLine("buffer does not contain enough data starting at offset");
                return;
            }

            bytes = new byte[] { buffer[offset], buffer[offset + 1], buffer[offset + 2], buffer[offset + 3], buffer[offset + 4], buffer[offset + 5] };
        }


        public bool IsValid()
        {
            return bytes != null && bytes.Length == 6;
        }

        public int CompareTo(MACAddress other)
        {
            int i = 0;
            i = bytes[0].CompareTo(other.bytes[0]);
            if (i != 0) return i;
            i = bytes[1].CompareTo(other.bytes[1]);
            if (i != 0) return i;
            i = bytes[2].CompareTo(other.bytes[2]);
            if (i != 0) return i;
            i = bytes[3].CompareTo(other.bytes[3]);
            if (i != 0) return i;
            i = bytes[4].CompareTo(other.bytes[4]);
            if (i != 0) return i;
            i = bytes[5].CompareTo(other.bytes[5]);
            if (i != 0) return i;

            return 0;
        }

        public bool Equals(MACAddress other)
        {
            return bytes[0] == other.bytes[0] &&
                bytes[1] == other.bytes[1] &&
                bytes[2] == other.bytes[2] &&
                bytes[3] == other.bytes[3] &&
                bytes[4] == other.bytes[4] &&
                bytes[5] == other.bytes[5];
        }

        public override int GetHashCode()
        {
            return (int)Hash;
        }

        public ulong ToNumber()
        {
            return (ulong)((bytes[0] << 40) | (bytes[1] << 32) | (bytes[2] << 24) | (bytes[3] << 16) |
                (bytes[4] << 8) | bytes[5]);
        }

        public uint To32BitNumber()
        {
            return (uint)((bytes[0] << 40) | (bytes[1] << 32) | (bytes[2] << 24) | (bytes[3] << 16) |
                (bytes[4] << 8) | bytes[5]);
        }

        uint hash;
        /// <summary>
        /// Hash value for this mac. Used to uniquely identify each mac
        /// </summary>
        internal uint Hash
        {
            get
            {
                if (hash == 0)
                {
                    hash = To32BitNumber();
                }

                return hash;
            }
        }

        public override string ToString()
        {
            string str = "";

            for (int i = 0; i < bytes.Length; i++)
            {
                str += $"{bytes[i].ToHex()}:";
            }

            if (!string.IsNullOrEmpty(str))
            {
                return str.Substring(0, str.Length - 1);
            }

            return str;
        }
    }
}
