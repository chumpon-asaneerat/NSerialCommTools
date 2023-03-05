#region Using

using System;
using System.Collections;

#endregion

namespace NLib.IO
{
    #region BitwiseOps

    /// <summary>
    /// Bitwise Operation class. Provide functions for bitwise operations
    /// and memory's buffer functions.
    /// </summary>
    public class BitwiseOps
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private BitwiseOps() : base() { }

        #endregion

        #region ToString

        /// <summary>
        /// To Binary String.
        /// </summary>
        /// <param name="bits">The source bit array.</param>
        /// <returns>Returns binary string that represents the value in bit array.</returns>
        public static string ToString(BitArray bits)
        {
            string result = string.Empty;
            if (null == bits || bits.Count <= 0)
                return result;

            int iCnt = 0;
            int iSet = 0;
            foreach (bool val in bits)
            {
                result += (val) ? "1" : "0";
                ++iCnt;
                if ((iCnt % 4) == 0)
                {
                    ++iSet;
                    if (iSet < 8)
                    {
                        result += " ";
                    }
                    else
                    {
                        result += Environment.NewLine;
                        iSet = 0;
                    }
                }
            }

            return result.Trim();
        }
        /// <summary>
        /// To Binary String.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>Returns binary string that represents the value.</returns>
        public static string ToString(byte value)
        {
            return ToString(ToBitArray(value));
        }
        /// <summary>
        /// To Binary String.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>Returns binary string that represents the value.</returns>
        public static string ToString(SByte value)
        {
            return ToString(ToBitArray(value));
        }
        /// <summary>
        /// To Binary String.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>Returns binary string that represents the value.</returns>
        public static string ToString(UInt16 value)
        {
            return ToString(ToBitArray(value));
        }
        /// <summary>
        /// To Binary String.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>Returns binary string that represents the value.</returns>
        public static string ToString(Int16 value)
        {
            return ToString(ToBitArray(value));
        }
        /// <summary>
        /// To Binary String.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>Returns binary string that represents the value.</returns>
        public static string ToString(UInt32 value)
        {
            return ToString(ToBitArray(value));
        }
        /// <summary>
        /// To Binary String.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>Returns binary string that represents the value.</returns>
        public static string ToString(Int32 value)
        {
            return ToString(ToBitArray(value));
        }
        /// <summary>
        /// To Binary String.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>Returns binary string that represents the value.</returns>
        public static string ToString(UInt64 value)
        {
            return ToString(ToBitArray(value));
        }
        /// <summary>
        /// To Binary String.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>Returns binary string that represents the value.</returns>
        public static string ToString(Int64 value)
        {
            return ToString(ToBitArray(value));
        }

        #endregion

        #region ToBitArray

        /// <summary>
        /// Convert value To BitArray.
        /// </summary>
        /// <param name="value">The source value to convert.</param>
        /// <returns>
        /// Returns bit array that fill with bool values that match source value.
        /// </returns>
        public static BitArray ToBitArray(Byte value)
        {
            int sz = sizeof(Byte) * 8;
            // Binary : 1000 0000
            // Hex    :    8    0
            Byte mask = 0x80;

            BitArray bits = new BitArray(sz);

            for (int i = 0; i < sz; ++i)
            {
                bits[i] = (((value << i) & mask) == mask);
            }

            return bits;
        }
        /// <summary>
        /// Convert value To BitArray.
        /// </summary>
        /// <param name="value">The source value to convert.</param>
        /// <returns>
        /// Returns bit array that fill with bool values that match source value.
        /// </returns>
        public static BitArray ToBitArray(SByte value)
        {
            return ToBitArray((Byte)value);
        }
        /// <summary>
        /// Convert value To BitArray.
        /// </summary>
        /// <param name="value">The source value to convert.</param>
        /// <returns>
        /// Returns bit array that fill with bool values that match source value.
        /// </returns>
        public static BitArray ToBitArray(Int16 value)
        {
            return ToBitArray((UInt16)value);
        }
        /// <summary>
        /// Convert value To BitArray.
        /// </summary>
        /// <param name="value">The source value to convert.</param>
        /// <returns>
        /// Returns bit array that fill with bool values that match source value.
        /// </returns>
        public static BitArray ToBitArray(UInt16 value)
        {
            int sz = sizeof(UInt16) * 8;
            // Binary : 1000 0000 0000 0000
            // Hex    :    8    0    0    0
            UInt16 mask = 0x8000;

            BitArray bits = new BitArray(sz);

            for (int i = 0; i < sz; ++i)
            {
                bits[i] = (((value << i) & mask) == mask);
            }

            return bits;
        }
        /// <summary>
        /// Convert value To BitArray.
        /// </summary>
        /// <param name="value">The source value to convert.</param>
        /// <returns>
        /// Returns bit array that fill with bool values that match source value.
        /// </returns>
        public static BitArray ToBitArray(Int32 value)
        {
            return ToBitArray((UInt32)value);
        }
        /// <summary>
        /// Convert value To BitArray.
        /// </summary>
        /// <param name="value">The source value to convert.</param>
        /// <returns>
        /// Returns bit array that fill with bool values that match source value.
        /// </returns>
        public static BitArray ToBitArray(UInt32 value)
        {
            int sz = sizeof(UInt32) * 8;
            // Binary : 1000 0000 0000 0000 
            //          0000 0000 0000 0000
            // Hex    :    8    0    0    0    
            //             0    0    0    0
            UInt32 mask = 0x80000000;

            BitArray bits = new BitArray(sz);

            for (int i = 0; i < sz; ++i)
            {
                bits[i] = (((value << i) & mask) == mask);
            }

            return bits;
        }
        /// <summary>
        /// Convert value To BitArray.
        /// </summary>
        /// <param name="value">The source value to convert.</param>
        /// <returns>
        /// Returns bit array that fill with bool values that match source value.
        /// </returns>
        public static BitArray ToBitArray(Int64 value)
        {
            return ToBitArray((UInt64)value);
        }
        /// <summary>
        /// Convert value To BitArray.
        /// </summary>
        /// <param name="value">The source value to convert.</param>
        /// <returns>
        /// Returns bit array that fill with bool values that match source value.
        /// </returns>
        public static BitArray ToBitArray(UInt64 value)
        {
            int sz = sizeof(UInt64) * 8;
            // Binary : 1000 0000 0000 0000 
            //          0000 0000 0000 0000 
            //          0000 0000 0000 0000 
            //          0000 0000 0000 0000
            // Hex    :    8    0    0    0    
            //             0    0    0    0
            //             0    0    0    0
            //             0    0    0    0
            UInt64 mask = 0x8000000000000000u;

            BitArray bits = new BitArray(sz);

            for (int i = 0; i < sz; ++i)
            {
                bits[i] = (((value << i) & mask) == mask);
            }

            return bits;
        }

        #endregion

        #region Convert String to data types

        /// <summary>
        /// Convert string to Byte.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>
        /// Returns Byte value that convert from string. 
        /// If error occur the zero value will return.
        /// </returns>
        public static Byte ToByte(string value)
        {
            return ToByte(value, 0);
        }
        /// <summary>
        /// Convert string to Byte.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Returns Byte value that convert from string. 
        /// If error occur the default value will return.
        /// </returns>
        public static Byte ToByte(string value, Byte defaultValue)
        {
            Byte val = defaultValue;
            try { val = Convert.ToByte(value); }
            catch { val = defaultValue; }
            return val;
        }
        /// <summary>
        /// Convert string to SByte.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>
        /// Returns SByte value that convert from string. 
        /// If error occur the zero value will return.
        /// </returns>
        public static SByte ToSByte(string value)
        {
            return ToSByte(value, 0);
        }
        /// <summary>
        /// Convert string to SByte.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Returns SByte value that convert from string. 
        /// If error occur the default value will return.
        /// </returns>
        public static SByte ToSByte(string value, SByte defaultValue)
        {
            SByte val = defaultValue;
            try { val = Convert.ToSByte(value); }
            catch { val = defaultValue; }
            return val;
        }
        /// <summary>
        /// Convert string to UInt16.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>
        /// Returns UInt16 value that convert from string. 
        /// If error occur the zero value will return.
        /// </returns>
        public static UInt16 ToUInt16(string value)
        {
            return ToUInt16(value, 0);
        }
        /// <summary>
        /// Convert string to UInt16.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Returns UInt16 value that convert from string. 
        /// If error occur the default value will return.
        /// </returns>
        public static UInt16 ToUInt16(string value, UInt16 defaultValue)
        {
            UInt16 val = defaultValue;
            try { val = Convert.ToUInt16(value); }
            catch { val = defaultValue; }
            return val;
        }
        /// <summary>
        /// Convert string to Int16.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>
        /// Returns Int16 value that convert from string. 
        /// If error occur the zero value will return.
        /// </returns>
        public static Int16 ToInt16(string value)
        {
            return ToInt16(value, 0);
        }
        /// <summary>
        /// Convert string to Int16.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Returns Int16 value that convert from string. 
        /// If error occur the default value will return.
        /// </returns>
        public static Int16 ToInt16(string value, Int16 defaultValue)
        {
            Int16 val = defaultValue;
            try { val = Convert.ToInt16(value); }
            catch { val = defaultValue; }
            return val;
        }
        /// <summary>
        /// Convert string to UInt32.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>
        /// Returns UInt32 value that convert from string. 
        /// If error occur the default value will return.
        /// </returns>
        public static UInt32 ToUInt32(string value)
        {
            return ToUInt32(value, 0);
        }
        /// <summary>
        /// Convert string to UInt32.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Returns UInt32 value that convert from string. 
        /// If error occur the default value will return.
        /// </returns>
        public static UInt32 ToUInt32(string value, UInt32 defaultValue)
        {
            UInt32 val = defaultValue;
            try { val = Convert.ToUInt32(value); }
            catch { val = defaultValue; }
            return val;
        }
        /// <summary>
        /// Convert string to Int32.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>
        /// Returns Int32 value that convert from string. 
        /// If error occur the default value will return.
        /// </returns>
        public static Int32 ToInt32(string value)
        {
            return ToInt32(value, 0);
        }
        /// <summary>
        /// Convert string to Int32.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Returns Int32 value that convert from string. 
        /// If error occur the default value will return.
        /// </returns>
        public static Int32 ToInt32(string value, Int32 defaultValue)
        {
            Int32 val = defaultValue;
            try { val = Convert.ToInt32(value); }
            catch { val = defaultValue; }
            return val;
        }
        /// <summary>
        /// Convert string to UInt64.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>
        /// Returns UInt64 value that convert from string. 
        /// If error occur the default value will return.
        /// </returns>
        public static UInt64 ToUInt64(string value)
        {
            return ToUInt64(value, 0);
        }
        /// <summary>
        /// Convert string to UInt64.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Returns UInt64 value that convert from string. 
        /// If error occur the zero value will return.
        /// </returns>
        public static UInt64 ToUInt64(string value, UInt64 defaultValue)
        {
            UInt64 val = defaultValue;
            try { val = Convert.ToUInt64(value); }
            catch { val = defaultValue; }
            return val;
        }
        /// <summary>
        /// Convert string to Int64.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>
        /// Returns Int64 value that convert from string. 
        /// If error occur the zero value will return.
        /// </returns>
        public static Int64 ToInt64(string value)
        {
            return ToInt64(value, 0);
        }
        /// <summary>
        /// Convert string to Int64.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Returns Int64 value that convert from string. 
        /// If error occur the default value will return.
        /// </returns>
        public static Int64 ToInt64(string value, Int64 defaultValue)
        {
            Int64 val = defaultValue;
            try { val = Convert.ToInt64(value); }
            catch { val = defaultValue; }
            return val;
        }

        #endregion

        #region Ror

        /// <summary>
        /// Rotate right operation.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate right from source value.</returns>
        public static Byte Ror(Byte value, uint nums)
        {
            Byte result = value;
            if (nums == 0)
                return result;
            int sz = sizeof(Byte) * 8;
            int actualNums = Convert.ToInt32(nums % sz);
            if (actualNums == 0)
                return result;

            BitArray bits = ToBitArray(value);

            if (null != bits && bits.Length == sz)
            {
                int iBit = 0;
                BitArray outbits = new BitArray(sz);

                int rightSz = actualNums;

                for (int i = 0; i < rightSz; ++i)
                {
                    int rIndex = sz - rightSz + i;
                    bool val = bits[rIndex];
                    outbits[iBit] = val;
                    ++iBit;
                }

                int leftSz = sz - actualNums;

                for (int i = 0; i < leftSz; ++i)
                {
                    int lIndex = i;
                    bool val = bits[i];
                    outbits[iBit] = val;
                    ++iBit;
                }

                result = 0;
                for (int i = 0; i < sz; ++i)
                {
                    int bitIndex = sz - i - 1;
                    bool bitVal = outbits[bitIndex];
                    if (bitVal)
                    {
                        Byte val = Convert.ToByte(Math.Pow(2, i));
                        result += val;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Rotate right operation.
        /// Please beware sign flag will cause rotate value misplace.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate right from source value.</returns>
        public static SByte Ror(SByte value, uint nums)
        {
            return (SByte)Ror((Byte)value, nums);
        }
        /// <summary>
        /// Rotate right operation.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate right from source value.</returns>
        public static UInt16 Ror(UInt16 value, uint nums)
        {
            UInt16 result = value;
            if (nums == 0)
                return result;
            int sz = sizeof(UInt16) * 8;
            int actualNums = Convert.ToInt32(nums % sz);
            if (actualNums == 0)
                return result;

            BitArray bits = ToBitArray(value);

            if (null != bits && bits.Length == sz)
            {
                int iBit = 0;
                BitArray outbits = new BitArray(sz);

                int rightSz = actualNums;

                for (int i = 0; i < rightSz; ++i)
                {
                    int rIndex = sz - rightSz + i;
                    bool val = bits[rIndex];
                    outbits[iBit] = val;
                    ++iBit;
                }

                int leftSz = sz - actualNums;

                for (int i = 0; i < leftSz; ++i)
                {
                    int lIndex = i;
                    bool val = bits[i];
                    outbits[iBit] = val;
                    ++iBit;
                }

                result = 0;
                for (int i = 0; i < sz; ++i)
                {
                    int bitIndex = sz - i - 1;
                    bool bitVal = outbits[bitIndex];
                    if (bitVal)
                    {
                        UInt16 val = Convert.ToUInt16(Math.Pow(2, i));
                        result += val;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Rotate right operation.
        /// Please beware sign flag will cause rotate value misplace.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate right from source value.</returns>
        public static Int16 Ror(Int16 value, uint nums)
        {
            return (Int16)Ror((UInt16)value, nums);
        }
        /// <summary>
        /// Rotate right operation.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate right from source value.</returns>
        public static UInt32 Ror(UInt32 value, uint nums)
        {
            UInt32 result = value;
            if (nums == 0)
                return result;
            int sz = sizeof(UInt32) * 8;
            int actualNums = Convert.ToInt32(nums % sz);
            if (actualNums == 0)
                return result;

            BitArray bits = ToBitArray(value);

            if (null != bits && bits.Length == sz)
            {
                int iBit = 0;
                BitArray outbits = new BitArray(sz);

                int rightSz = actualNums;

                for (int i = 0; i < rightSz; ++i)
                {
                    int rIndex = sz - rightSz + i;
                    bool val = bits[rIndex];
                    outbits[iBit] = val;
                    ++iBit;
                }

                int leftSz = sz - actualNums;

                for (int i = 0; i < leftSz; ++i)
                {
                    int lIndex = i;
                    bool val = bits[i];
                    outbits[iBit] = val;
                    ++iBit;
                }

                result = 0;
                for (int i = 0; i < sz; ++i)
                {
                    int bitIndex = sz - i - 1;
                    bool bitVal = outbits[bitIndex];
                    if (bitVal)
                    {
                        UInt32 val = Convert.ToUInt32(Math.Pow(2, i));
                        result += val;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Rotate right operation.
        /// Please beware sign flag will cause rotate value misplace.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate right from source value.</returns>
        public static Int32 Ror(Int32 value, uint nums)
        {
            return (Int32)Ror((UInt32)value, nums);
        }
        /// <summary>
        /// Rotate right operation.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate right from source value.</returns>
        public static UInt64 Ror(UInt64 value, uint nums)
        {
            UInt64 result = value;
            if (nums == 0)
                return result;
            int sz = sizeof(UInt64) * 8;
            int actualNums = Convert.ToInt32(nums % sz);
            if (actualNums == 0)
                return result;

            BitArray bits = ToBitArray(value);

            if (null != bits && bits.Length == sz)
            {
                int iBit = 0;
                BitArray outbits = new BitArray(sz);

                int rightSz = actualNums;

                for (int i = 0; i < rightSz; ++i)
                {
                    int rIndex = sz - rightSz + i;
                    bool val = bits[rIndex];
                    outbits[iBit] = val;
                    ++iBit;
                }

                int leftSz = sz - actualNums;

                for (int i = 0; i < leftSz; ++i)
                {
                    int lIndex = i;
                    bool val = bits[i];
                    outbits[iBit] = val;
                    ++iBit;
                }

                result = 0;
                for (int i = 0; i < sz; ++i)
                {
                    int bitIndex = sz - i - 1;
                    bool bitVal = outbits[bitIndex];
                    if (bitVal)
                    {
                        UInt64 val = Convert.ToUInt64(Math.Pow(2, i));
                        result += val;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Rotate right operation.
        /// Please beware sign flag will cause rotate value misplace.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate right from source value.</returns>
        public static Int64 Ror(Int64 value, uint nums)
        {
            return (Int64)Ror((UInt64)value, nums);
        }
        /// <summary>
        /// Rotate right operation on byte array.
        /// </summary>
        /// <param name="values">The source value array.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>
        /// Returns array of byte that each byte rotate right bit from source array.
        /// </returns>
        public static Byte[] Ror(Byte[] values, uint nums)
        {
            int sz = sizeof(Byte) * 8;
            int actualNums = Convert.ToInt32(nums % sz);
            if (null == values || values.Length <= 0 ||
                actualNums == 0)
                return values;

            Byte[] results = new Byte[values.Length];

            for (int i = 0; i < values.Length; ++i)
                results[i] = Ror(values[i], nums);

            return results;
        }

        #endregion

        #region Rol

        /// <summary>
        /// Rotate left operation.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate left from source value.</returns>
        public static Byte Rol(Byte value, uint nums)
        {
            Byte result = value;
            if (nums == 0)
                return result;
            int sz = sizeof(Byte) * 8;
            int actualNums = Convert.ToInt32(nums % sz);
            if (actualNums == 0)
                return result;

            BitArray bits = ToBitArray(value);

            if (null != bits && bits.Length == sz)
            {
                int iBit = 0;
                BitArray outbits = new BitArray(sz);

                int rightSz = sz;

                for (int i = actualNums; i < rightSz; ++i)
                {
                    int rIndex = i;
                    bool val = bits[i];
                    outbits[iBit] = val;
                    ++iBit;
                }

                int leftSz = actualNums;

                for (int i = 0; i < leftSz; ++i)
                {
                    int lIndex = i;
                    bool val = bits[lIndex];
                    outbits[iBit] = val;
                    ++iBit;
                }

                result = 0;
                for (int i = 0; i < sz; ++i)
                {
                    int bitIndex = sz - i - 1;
                    bool bitVal = outbits[bitIndex];
                    if (bitVal)
                    {
                        Byte val = Convert.ToByte(Math.Pow(2, i));
                        result += val;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Rotate left operation.
        /// Please beware sign flag will cause rotate value misplace.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate left from source value.</returns>
        public static SByte Rol(SByte value, uint nums)
        {
            return (SByte)Rol((Byte)value, nums);
        }
        /// <summary>
        /// Rotate left operation.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate left from source value.</returns>
        public static UInt16 Rol(UInt16 value, uint nums)
        {
            UInt16 result = value;
            if (nums == 0)
                return result;
            int sz = sizeof(UInt16) * 8;
            int actualNums = Convert.ToInt32(nums % sz);
            if (actualNums == 0)
                return result;

            BitArray bits = ToBitArray(value);

            if (null != bits && bits.Length == sz)
            {
                int iBit = 0;
                BitArray outbits = new BitArray(sz);

                int rightSz = sz;

                for (int i = actualNums; i < rightSz; ++i)
                {
                    int rIndex = i;
                    bool val = bits[i];
                    outbits[iBit] = val;
                    ++iBit;
                }

                int leftSz = actualNums;

                for (int i = 0; i < leftSz; ++i)
                {
                    int lIndex = i;
                    bool val = bits[lIndex];
                    outbits[iBit] = val;
                    ++iBit;
                }

                result = 0;
                for (int i = 0; i < sz; ++i)
                {
                    int bitIndex = sz - i - 1;
                    bool bitVal = outbits[bitIndex];
                    if (bitVal)
                    {
                        UInt16 val = Convert.ToUInt16(Math.Pow(2, i));
                        result += val;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Rotate left operation.
        /// Please beware sign flag will cause rotate value misplace.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate left from source value.</returns>
        public static Int16 Rol(Int16 value, uint nums)
        {
            return (Int16)Rol((UInt16)value, nums);
        }
        /// <summary>
        /// Rotate left operation.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate left from source value.</returns>
        public static UInt32 Rol(UInt32 value, uint nums)
        {
            UInt32 result = value;
            if (nums == 0)
                return result;
            int sz = sizeof(UInt32) * 8;
            int actualNums = Convert.ToInt32(nums % sz);
            if (actualNums == 0)
                return result;

            BitArray bits = ToBitArray(value);

            if (null != bits && bits.Length == sz)
            {
                int iBit = 0;
                BitArray outbits = new BitArray(sz);

                int rightSz = sz;

                for (int i = actualNums; i < rightSz; ++i)
                {
                    int rIndex = i;
                    bool val = bits[i];
                    outbits[iBit] = val;
                    ++iBit;
                }

                int leftSz = actualNums;

                for (int i = 0; i < leftSz; ++i)
                {
                    int lIndex = i;
                    bool val = bits[lIndex];
                    outbits[iBit] = val;
                    ++iBit;
                }

                result = 0;
                for (int i = 0; i < sz; ++i)
                {
                    int bitIndex = sz - i - 1;
                    bool bitVal = outbits[bitIndex];
                    if (bitVal)
                    {
                        UInt32 val = Convert.ToUInt32(Math.Pow(2, i));
                        result += val;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Rotate left operation.
        /// Please beware sign flag will cause rotate value misplace.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate left from source value.</returns>
        public static Int32 Rol(Int32 value, uint nums)
        {
            return (Int32)Rol((UInt32)value, nums);
        }
        /// <summary>
        /// Rotate left operation.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate left from source value.</returns>
        public static UInt64 Rol(UInt64 value, uint nums)
        {
            UInt64 result = value;
            if (nums == 0)
                return result;
            int sz = sizeof(UInt64) * 8;
            int actualNums = Convert.ToInt32(nums % sz);
            if (actualNums == 0)
                return result;

            BitArray bits = ToBitArray(value);

            if (null != bits && bits.Length == sz)
            {
                int iBit = 0;
                BitArray outbits = new BitArray(sz);

                int rightSz = sz;

                for (int i = actualNums; i < rightSz; ++i)
                {
                    int rIndex = i;
                    bool val = bits[i];
                    outbits[iBit] = val;
                    ++iBit;
                }

                int leftSz = actualNums;

                for (int i = 0; i < leftSz; ++i)
                {
                    int lIndex = i;
                    bool val = bits[lIndex];
                    outbits[iBit] = val;
                    ++iBit;
                }

                result = 0;
                for (int i = 0; i < sz; ++i)
                {
                    int bitIndex = sz - i - 1;
                    bool bitVal = outbits[bitIndex];
                    if (bitVal)
                    {
                        UInt64 val = Convert.ToUInt64(Math.Pow(2, i));
                        result += val;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Rotate left operation.
        /// Please beware sign flag will cause rotate value misplace.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>Returns bit rotate left from source value.</returns>
        public static Int64 Rol(Int64 value, uint nums)
        {
            return (Int64)Rol((UInt64)value, nums);
        }
        /// <summary>
        /// Rotate left operation on byte array.
        /// </summary>
        /// <param name="values">The source value array.</param>
        /// <param name="nums">The number of bit to rotate.</param>
        /// <returns>
        /// Returns array of byte that each byte rotate left bit from source array.
        /// </returns>
        public static Byte[] Rol(Byte[] values, uint nums)
        {
            int sz = sizeof(Byte) * 8;
            int actualNums = Convert.ToInt32(nums % sz);
            if (null == values || values.Length <= 0 ||
                actualNums == 0)
                return values;

            Byte[] results = new Byte[values.Length];

            for (int i = 0; i < values.Length; ++i)
                results[i] = Rol(values[i], nums);

            return results;
        }

        #endregion

        #region Xor

        /// <summary>
        /// Xor with specificed mask.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="mask">The mask value.</param>
        /// <returns>Returns result of value xor with mask.</returns>
        public static byte Xor(Byte value, Byte mask)
        {
            return (Byte)(value ^ mask);
        }
        /// <summary>
        /// Xor byte array with mask array.
        /// </summary>
        /// <param name="values">The byte array to mask.</param>
        /// <param name="masks">The mask array.</param>
        /// <returns>
        /// Returns byte array that generated from value array that xor with mask array.
        /// </returns>
        public static Byte[] Xor(Byte[] values, Byte[] masks)
        {
            if (null == values || values.Length <= 0 ||
                null == masks || masks.Length <= 0)
                return values;

            int iValLen = values.Length;
            int iMskLen = masks.Length;
            int iVal = 0;
            int iMsk = 0;

            Byte[] results = new Byte[iValLen];
            while (iVal < iValLen)
            {
                if (iMsk >= iMskLen)
                    iMsk = 0;
                results[iVal] = (Byte)(values[iVal] ^ masks[iMsk]);
                ++iMsk;
                ++iVal;
            }
            return results;
        }

        #endregion

        #region Byte Array operations

        /// <summary>
        /// Create byte array.
        /// </summary>
        /// <param name="size">Size of block in bytes.</param>
        /// <returns>
        /// Returns byte array that fill with zero value.
        /// If error occur null value returns.
        /// </returns>
        public static Byte[] CreateByteArray(uint size)
        {
            return CreateByteArray(size, 0);
        }
        /// <summary>
        /// Create byte array.
        /// </summary>
        /// <param name="size">Size of block in bytes.</param>
        /// <param name="defaultValue">The default value on each byte.</param>
        /// <returns>
        /// Returns byte array that fill with default value.
        /// If error occur null value returns.
        /// </returns>
        public static Byte[] CreateByteArray(uint size, Byte defaultValue)
        {
            Byte[] results = new Byte[size];
            int fillSize = FillByteArray(results, defaultValue);
            if (fillSize < 0)
                return null; // error occur
            return results;
        }
        /// <summary>
        /// Fill byte array with specificed value.
        /// </summary>
        /// <param name="buffers">The byte array to fill.</param>
        /// <param name="value">The value to fill.</param>
        /// <returns>Returns number of byte fill. If error -1 returns.</returns>
        public static int FillByteArray(Byte[] buffers, Byte value)
        {
            if (null == buffers)
                return -1;
            int size = buffers.Length;
            for (int i = 0; i < size; ++i)
            {
                buffers[i] = value;
            }
            return size;
        }
        /// <summary>
        /// Create Byte array with fill with random value.
        /// </summary>
        /// <param name="size">Size of block in bytes.</param>
        /// <returns>
        /// Returns byte array that fill with random value.
        /// If error occur null value returns.
        /// </returns>
        public static Byte[] CreateRandomByteArray(uint size)
        {
            Byte[] results = CreateByteArray(size);

            Random rand = new Random();
            for (int i = 0; i < size; i++)
            {
                uint nextVal = Convert.ToUInt32(Math.Truncate((rand.NextDouble() * 1000)) +
                    rand.Next(Convert.ToInt32(DateTime.Now.ToFileTimeUtc() % int.MaxValue)));

                results[i] = Convert.ToByte(nextVal & 0xFF);
            }


            return results;
        }

        #endregion
    }

    #endregion
}
