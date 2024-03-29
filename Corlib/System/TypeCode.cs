﻿namespace System
{
    //
    // Resumen:
    //     Specifies the type of an object.
    public enum TypeCode
    {
        //
        // Resumen:
        //     A null reference.
        Empty = 0,
        //
        // Resumen:
        //     A general type representing any reference or value type not explicitly represented
        //     by another TypeCode.
        Object = 1,
        //
        // Resumen:
        //     A database null (column) value.
        DBNull = 2,
        //
        // Resumen:
        //     A simple type representing Boolean values of true or false.
        Boolean = 3,
        //
        // Resumen:
        //     An integral type representing unsigned 16-bit integers with values between 0
        //     and 65535. The set of possible values for the System.TypeCode.Char type corresponds
        //     to the Unicode character set.
        Char = 4,
        //
        // Resumen:
        //     An integral type representing signed 8-bit integers with values between -128
        //     and 127.
        SByte = 5,
        //
        // Resumen:
        //     An integral type representing unsigned 8-bit integers with values between 0 and
        //     255.
        Byte = 6,
        //
        // Resumen:
        //     An integral type representing signed 16-bit integers with values between -32768
        //     and 32767.
        Int16 = 7,
        //
        // Resumen:
        //     An integral type representing unsigned 16-bit integers with values between 0
        //     and 65535.
        UInt16 = 8,
        //
        // Resumen:
        //     An integral type representing signed 32-bit integers with values between -2147483648
        //     and 2147483647.
        Int32 = 9,
        //
        // Resumen:
        //     An integral type representing unsigned 32-bit integers with values between 0
        //     and 4294967295.
        UInt32 = 10,
        //
        // Resumen:
        //     An integral type representing signed 64-bit integers with values between -9223372036854775808
        //     and 9223372036854775807.
        Int64 = 11,
        //
        // Resumen:
        //     An integral type representing unsigned 64-bit integers with values between 0
        //     and 18446744073709551615.
        UInt64 = 12,
        //
        // Resumen:
        //     A floating point type representing values ranging from approximately 1.5 x 10
        //     -45 to 3.4 x 10 38 with a precision of 7 digits.
        Single = 13,
        //
        // Resumen:
        //     A floating point type representing values ranging from approximately 5.0 x 10
        //     -324 to 1.7 x 10 308 with a precision of 15-16 digits.
        Double = 14,
        //
        // Resumen:
        //     A simple type representing values ranging from 1.0 x 10 -28 to approximately
        //     7.9 x 10 28 with 28-29 significant digits.
        Decimal = 15,
        //
        // Resumen:
        //     A type representing a date and time value.
        DateTime = 16,
        //
        // Resumen:
        //     A sealed class type representing Unicode character strings.
        String = 18
    }
}
