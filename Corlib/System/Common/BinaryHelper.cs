﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace System.Common
{
    /// <summary>
    /// Contains various helpermethods to make bitfiddling easier.
    /// </summary>
    public class BinaryHelper
    {
        /// <summary>
        /// Bitwise checks if the given bit is set in the data.
        /// </summary>
        /// <param name="data">A 16-bit unsigned int data</param>
        /// <param name="bit">The zero-based position of a bit. I.e. bit 1 is the second bit.</param>
        /// <returns>Returns TRUE if bit is set.</returns>
        public static bool CheckBit(ushort data, ushort bit)
        {
            //A single bit is LEFT SHIFTED the number a given number of bits.
            //and bitwise AND'ed together with the data.
            //So the initial value is   :       0000 0000.
            //Left shifting a bit 3 bits:       0000 0100
            //And'ed together with the data:    0101 0101 AND 0000 01000 => 0000 0100 (which is greater than zero - so bit is set).

            ushort mask = (ushort)(1 << (ushort)bit);
            return (data & mask) != 0;
        }

        /// <summary>
        /// Bitwise checks if the given bit is set in the data.
        /// </summary>
        /// <param name="data">A 32-bit unsigned int data.</param>
        /// <param name="bit">The zero-based position of a bit. I.e. bit 1 is the second bit.</param>
        /// <returns>Returns TRUE if bit is set.</returns>
        public static bool CheckBit(uint data, ushort bit)
        {
            uint mask = (uint)(1 << (int)bit);
            return (data & mask) != 0;
        }

        public static bool CheckBit(byte data, byte bit)
        {
            byte mask = (byte)(1 << bit);
            return (data & mask) != 0;
        }

        /// <summary>
        /// Flips the bit value at the given position in the data, from low to high, or from high to low.
        /// </summary>
        /// <param name="data">A byte of data.</param>
        /// <param name="bitposition">A bit position to change.</param>
        /// <returns>The same data, but with one bit changed.</returns>
        public static byte FlipBit(byte data, ushort bitposition)
        {
            byte mask = (byte)(1 << bitposition);
            if (CheckBit(data, bitposition))
                return (byte)(data & ~mask);
            else
                return (byte)(data | mask);
        }

        /// <summary>
        /// Flips the bit value at the given position in the data, from low to high, or from high to low.
        /// </summary>
        /// <param name="data">A 32-bit unsigned int of data.</param>
        /// <param name="bitposition">A bit position to change.</param>
        /// <returns>The same data, but with one bit changed.</returns>
        public static uint FlipBit(uint data, ushort bitposition)
        {
            uint mask = (uint)(1 << bitposition);
            if (CheckBit(data, bitposition))
                return (uint)(data & ~mask);
            else
                return (uint)(data | mask);
        }


        /// <summary>
        /// Get 32-bit unsigned int from 32-bit unsigned int, starting from a given offset.
        /// </summary>
        /// <param name="data">A 32-bit unsigned int data.</param>
        /// <param name="offset">The offset (in bytes) from the start of the data.</param>
        /// <exception cref="ArgumentException">Thrown when offset is greater then 24.</exception>
        /// <returns>Extracted 8-bit unsigned int (byte).</returns>
        public static byte GetByteFrom32bit(uint data, byte offset)
        {
            if (offset > 24)
            {
                //throw new ArgumentException("Offset can not move outside the 32 bit range");
                Debug.WriteLine("Offset can not move outside the 32 bit range");
                return 00;
            }

            data = data >> offset;
            return (byte)data;
        }

        /// <summary>
        /// Returns the HEX value of a given bitnumber.
        /// </summary>
        [Flags]
        public enum BitPos : uint
        {
            BIT0 = 0x1,
            BIT1 = 0x2,
            BIT2 = 0x4,
            BIT3 = 0x8,
            BIT4 = 0x10,
            BIT5 = 0x20,
            BIT6 = 0x40,
            BIT7 = 0x80,
            BIT8 = 0x100,
            BIT9 = 0x200,
            BIT10 = 0x400,
            BIT11 = 0x800,
            BIT12 = 0x1000,
            BIT13 = 0x2000,
            BIT14 = 0x4000,
            BIT15 = 0x8000,
            BIT16 = 0x10000,
            BIT17 = 0x20000,
            BIT18 = 0x40000,
            BIT19 = 0x80000,
            BIT20 = 0x100000,
            BIT21 = 0x200000,
            BIT22 = 0x400000,
            BIT23 = 0x800000,
            BIT24 = 0x1000000,
            BIT25 = 0x2000000,
            BIT26 = 0x4000000,
            BIT27 = 0x8000000,
            BIT28 = 0x10000000,
            BIT29 = 0x20000000,
            BIT30 = 0x40000000,
            BIT31 = 0x80000000
        }
    }
}
