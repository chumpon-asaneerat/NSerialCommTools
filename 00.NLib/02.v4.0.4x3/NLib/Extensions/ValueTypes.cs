#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace NLib
{
    #region Enums

    #region DataTypes Enum

    /// <summary>
    /// The Data Types Enum.
    /// </summary>
    public enum DataTypes
    {
        /// <summary>
        /// Unsigned Byte.
        /// </summary>
        Byte,
        /// <summary>
        /// Signed Byte.
        /// </summary>
        SByte,
        /// <summary>
        /// Unsigned Integer 16 bits or Word.
        /// </summary>
        UShort,
        /// <summary>
        /// Signed Integer 16 bits or Word.
        /// </summary>
        Short,
        /// <summary>
        /// Unsigned Integer 32 bits or Double Word.
        /// </summary>
        UInt,
        /// <summary>
        /// Signed Integer 32 bits or Double Word.
        /// </summary>
        Int,
        /// <summary>
        /// Unsigned Integer 64 bits or Quad Word.
        /// </summary>
        Long,
        /// <summary>
        /// Unsigned Integer 64 bits or Quad Word.
        /// </summary>
        ULong,
        /// <summary>
        /// Single Precision (or float) 32 bites or 4 bytes.
        /// </summary>
        Single,
        /// <summary>
        /// Double Precision (or double) 64 bites or 8 bytes.
        /// </summary>
        Double
    }

    #endregion

    #region InputTypes Enum

    /// <summary>
    /// The Input Types Enum.
    /// </summary>
    public enum InputTypes
    {
        /// <summary>
        /// Display As Decimal.
        /// </summary>
        Decimal,
        /// <summary>
        /// Display as Hex.
        /// </summary>
        Hex
    }

    #endregion

    #endregion

    #region ValueTypeExtensionMethods

    /// <summary>
    /// The Value Type Extension Methods.
    /// </summary>
    public static class ValueTypeExtensionMethods
    {
        #region Static Variables

        // For Check Hex string.
        static readonly System.Text.RegularExpressions.Regex r =
            new System.Text.RegularExpressions.Regex(@"\A\b[0-9a-fA-F]+\b\Z");

        private static readonly Dictionary<DataTypes, int> _dataTypeSizes = null;
        private static readonly Dictionary<Type, int> _valueTypeSizes = null;

        #endregion

        #region Static Constructor

        /// <summary>
        /// Static Constructor.
        /// </summary>
        static ValueTypeExtensionMethods()
        {
            #region Init DataType Size

            _dataTypeSizes = new Dictionary<DataTypes, int>();
            _dataTypeSizes.Add(DataTypes.SByte, 1);
            _dataTypeSizes.Add(DataTypes.Byte, 1);
            _dataTypeSizes.Add(DataTypes.Short, 2);
            _dataTypeSizes.Add(DataTypes.UShort, 2);
            _dataTypeSizes.Add(DataTypes.Int, 4);
            _dataTypeSizes.Add(DataTypes.UInt, 4);
            _dataTypeSizes.Add(DataTypes.Long, 8);
            _dataTypeSizes.Add(DataTypes.ULong, 8);
            _dataTypeSizes.Add(DataTypes.Single, sizeof(float));
            _dataTypeSizes.Add(DataTypes.Double, sizeof(double));

            #endregion

            #region Init ValueType Size

            _valueTypeSizes = new Dictionary<Type, int>();
            _valueTypeSizes.Add(typeof(sbyte), 1);
            _valueTypeSizes.Add(typeof(byte), 1);
            _valueTypeSizes.Add(typeof(short), 2);
            _valueTypeSizes.Add(typeof(ushort), 2);
            _valueTypeSizes.Add(typeof(int), 4);
            _valueTypeSizes.Add(typeof(uint), 4);
            _valueTypeSizes.Add(typeof(long), 8);
            _valueTypeSizes.Add(typeof(ulong), 8);
            _valueTypeSizes.Add(typeof(float), sizeof(float));
            _valueTypeSizes.Add(typeof(double), sizeof(double));

            #endregion
        }

        #endregion

        #region Private Methods

        #region GetSize

        /// <summary>
        /// Gets number of byte used for specificed data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Returns size of byte required to keep data for specificed data type.</returns>
        private static int GetSize(DataTypes dataType)
        {
            if (null == _dataTypeSizes || !_dataTypeSizes.ContainsKey(dataType))
                return 0;
            else return _dataTypeSizes[dataType];
        }
        /// <summary>
        /// Gets number of byte used for specificed Value Type (byte, short, int, etc.).
        /// </summary>
        /// <param name="valueType">The value to find size.</param>
        /// <returns>Returns size of byte required to keep data for specificed value type.</returns>
        private static int GetSize(ValueType valueType)
        {
            Type type = valueType.GetType();
            if (null == _valueTypeSizes || !_valueTypeSizes.ContainsKey(type))
                return 0;
            else return _valueTypeSizes[type];
        }

        #endregion

        #region GetBytes

        /// <summary>
        /// Gets proper bytes. Used for convert from source bytes to target bytes with auto fill or
        /// remove mis-size of data type.
        /// </summary>
        /// <param name="targetSize">The target data size.</param>
        /// <param name="dataSize">The value (or source) data size.</param>
        /// <param name="values">The value byte array.</param>
        /// <returns>Returns data that match target size.</returns>
        private static byte[] GetBytes(int targetSize, int dataSize, byte[] values)
        {
            byte[] results = null;
            if (targetSize == dataSize)
            {
                results = values;
            }
            else if (targetSize > dataSize)
            {
                results = new byte[targetSize];
                // Target Size : 16 bytes
                // Data Size = 2 bytes
                // Start Index = 16 - 2 = 14.
                int startDataIndex = targetSize - dataSize;
                int iIndex = 0;
                while (iIndex < targetSize)
                {
                    if (iIndex < startDataIndex)
                        results[iIndex] = 0x00; // Fill Zero byte
                    else results[iIndex] = values[iIndex - startDataIndex]; // Fill Data byte
                    // Increase index.
                    ++iIndex;
                }
            }
            else //if (targetSize < dataSize)
            {
                results = new byte[targetSize];
                // Target Size : 2 bytes
                // Data Size =  16 bytes
                // Start Index = Data Size - TargetSize - 1 (read only last 2 bytes(or target size)).
                int startDataIndex = dataSize - targetSize - 1;
                int iIndex = 0;
                while (iIndex < targetSize)
                {
                    results[iIndex] = values[startDataIndex + iIndex]; // Fill Data byte
                    // Increase index.
                    ++iIndex;
                }
            }

            return results;
        }
        /// <summary>
        /// Gets Empty bytes Or Zero bytes from DataTypes.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Returns bytes with fill all with zero.</returns>
        private static byte[] GetBytes(DataTypes dataType)
        {
            byte[] results = null;
            string value = "0";
            switch (dataType)
            {
                case DataTypes.SByte:
                    results = value.ToSByte().GetBytes(dataType);
                    break;
                case DataTypes.Byte:
                    results = value.ToByte().GetBytes(dataType);
                    break;
                case DataTypes.Short:
                    results = value.ToInt16().GetBytes(dataType);
                    break;
                case DataTypes.UShort:
                    results = value.ToUInt16().GetBytes(dataType);
                    break;
                case DataTypes.Int:
                    results = value.ToInt32().GetBytes(dataType);
                    break;
                case DataTypes.UInt:
                    results = value.ToUInt32().GetBytes(dataType);
                    break;
                case DataTypes.Long:
                    results = value.ToInt64().GetBytes(dataType);
                    break;
                case DataTypes.ULong:
                    results = value.ToUInt64().GetBytes(dataType);
                    break;
                case DataTypes.Single:
                    results = value.ToSingle().GetBytes(dataType);
                    break;
                case DataTypes.Double:
                    results = value.ToDouble().GetBytes(dataType);
                    break;
            }
            return results;
        }

        #endregion

        #region Decimal String to ValueType

        /// <summary>
        /// Convert Decimal string to Byte.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static byte DecimalToByte(string value)
        {
            byte result;
            if (string.IsNullOrWhiteSpace(value) ||
                !byte.TryParse(value, out result))
            {
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// Convert Decimal string to Signed Byte.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static sbyte DecimalToSByte(string value)
        {
            sbyte result;
            if (string.IsNullOrWhiteSpace(value) ||
                !sbyte.TryParse(value, out result))
            {
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// Convert Decimal string to Int16.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static short DecimalToInt16(string value)
        {
            short result;
            if (string.IsNullOrWhiteSpace(value) ||
                !short.TryParse(value, out result))
            {
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// Convert Decimal string to UInt16.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static ushort DecimalToUInt16(string value)
        {
            ushort result;
            if (string.IsNullOrWhiteSpace(value) ||
                !ushort.TryParse(value, out result))
            {
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// Convert Decimal string to Int32.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static int DecimalToInt32(string value)
        {
            int result;
            if (string.IsNullOrWhiteSpace(value) ||
                !int.TryParse(value, out result))
            {
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// Convert Decimal string to UInt32.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static uint DecimalToUInt32(string value)
        {
            uint result;
            if (string.IsNullOrWhiteSpace(value) ||
                !uint.TryParse(value, out result))
            {
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// Convert Decimal string to Int64.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static long DecimalToInt64(string value)
        {
            long result;
            if (string.IsNullOrWhiteSpace(value) ||
                !long.TryParse(value, out result))
            {
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// Convert Decimal string to UInt64.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static ulong DecimalToUInt64(string value)
        {
            ulong result;
            if (string.IsNullOrWhiteSpace(value) ||
                !ulong.TryParse(value, out result))
            {
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// Convert Decimal string to single.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static float DecimalToSingle(string value)
        {
            float result;
            if (string.IsNullOrWhiteSpace(value) ||
                !float.TryParse(value, out result))
            {
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// Convert Decimal string to double.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static double DecimalToDouble(string value)
        {
            double result;
            if (string.IsNullOrWhiteSpace(value) ||
                !double.TryParse(value, out result))
            {
                result = 0;
            }
            return result;
        }

        #endregion

        #region Hex String to ValueType

        /// <summary>
        /// Convert Hex string to byte array.
        /// </summary>
        /// <param name="hex">The hex string.</param>
        /// <returns>Returns byte arrays of source string.</returns>
        private static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        /// <summary>
        /// Convert Hex string to Byte.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static byte HexToByte(string value)
        {
            byte result = 0;
            if (!value.IsHex())
            {
                return result;
            }

            string val = value;
            while ((val.Length % 2) != 0)
            {
                val = "0" + val; // append zero in front of string.
            }
            byte[] hexArray = HexStringToByteArray(val);
            if (null != hexArray && hexArray.Length > 0)
            {
                result = hexArray[hexArray.Length - 1];
            }

            return result;
        }
        /// <summary>
        /// Convert Hex string to SByte.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static sbyte HexToSByte(string value)
        {
            sbyte result = 0;
            if (!value.IsHex())
            {
                return result;
            }

            string val = value;
            while ((val.Length % 2) != 0)
            {
                val = "0" + val; // append zero in front of string.
            }
            byte[] hexArray = HexStringToByteArray(val);
            if (null != hexArray && hexArray.Length > 0)
            {
                result = (sbyte)hexArray[hexArray.Length - 1];
            }

            return result;
        }
        /// <summary>
        /// Convert Hex string to Int16.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static short HexToInt16(string value)
        {
            short result = 0;
            if (!value.IsHex())
            {
                return result;
            }

            int byteLen = GetSize(DataTypes.Short);
            int charLen = byteLen * 2;
            string val = value;
            while ((val.Length % charLen) != 0)
            {
                val = "0" + val; // append zero in front of string.
            }
            byte[] hexArray = HexStringToByteArray(val);
            if (null != hexArray && hexArray.Length > 0)
            {
                hexArray.Reverse(); // Reverse byte order
                byte[] data = new byte[byteLen];
                int iCnt = 0;
                int maxLen = (hexArray.Length > data.Length) ? data.Length : hexArray.Length;
                while (iCnt < maxLen)
                {
                    data[byteLen - iCnt - 1] = hexArray[iCnt];
                    ++iCnt;
                }
                // If the system architecture is little-endian (that is, little end first), 
                // reverse the byte array. 
                //if (BitConverter.IsLittleEndian)
                //    Array.Reverse(data);

                result = BitConverter.ToInt16(data, 0);
            }

            return result;
        }
        /// <summary>
        /// Convert Hex string to UInt16.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static ushort HexToUInt16(string value)
        {
            ushort result = 0;
            if (!value.IsHex())
            {
                return result;
            }

            int byteLen = GetSize(DataTypes.UShort);
            int charLen = byteLen * 2;
            string val = value;
            while ((val.Length % charLen) != 0)
            {
                val = "0" + val; // append zero in front of string.
            }
            byte[] hexArray = HexStringToByteArray(val);
            if (null != hexArray && hexArray.Length > 0)
            {
                hexArray.Reverse(); // Reverse byte order
                byte[] data = new byte[byteLen];
                int iCnt = 0;
                int maxLen = (hexArray.Length > data.Length) ? data.Length : hexArray.Length;
                while (iCnt < maxLen)
                {
                    data[byteLen - iCnt - 1] = hexArray[iCnt];
                    ++iCnt;
                }
                // If the system architecture is little-endian (that is, little end first), 
                // reverse the byte array. 
                //if (BitConverter.IsLittleEndian)
                //    Array.Reverse(data);

                result = BitConverter.ToUInt16(data, 0);
            }

            return result;
        }
        /// <summary>
        /// Convert Hex string to Int32.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static int HexToInt32(string value)
        {
            int result = 0;
            if (!value.IsHex())
            {
                return result;
            }

            int byteLen = GetSize(DataTypes.Int);
            int charLen = byteLen * 2;
            string val = value;
            while ((val.Length % charLen) != 0)
            {
                val = "0" + val; // append zero in front of string.
            }
            byte[] hexArray = HexStringToByteArray(val);
            if (null != hexArray && hexArray.Length > 0)
            {
                hexArray.Reverse(); // Reverse byte order
                byte[] data = new byte[byteLen];
                int iCnt = 0;
                int maxLen = (hexArray.Length > data.Length) ? data.Length : hexArray.Length;
                while (iCnt < maxLen)
                {
                    data[byteLen - iCnt - 1] = hexArray[iCnt];
                    ++iCnt;
                }
                // If the system architecture is little-endian (that is, little end first), 
                // reverse the byte array. 
                //if (BitConverter.IsLittleEndian)
                //    Array.Reverse(data);

                result = BitConverter.ToInt32(data, 0);
            }

            return result;
        }
        /// <summary>
        /// Convert Hex string to UInt32.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static uint HexToUInt32(string value)
        {
            uint result = 0;
            if (!value.IsHex())
            {
                return result;
            }

            int byteLen = GetSize(DataTypes.UInt);
            int charLen = byteLen * 2;
            string val = value;
            while ((val.Length % charLen) != 0)
            {
                val = "0" + val; // append zero in front of string.
            }
            byte[] hexArray = HexStringToByteArray(val);
            if (null != hexArray && hexArray.Length > 0)
            {
                hexArray.Reverse(); // Reverse byte order
                byte[] data = new byte[byteLen];
                int iCnt = 0;
                int maxLen = (hexArray.Length > data.Length) ? data.Length : hexArray.Length;
                while (iCnt < maxLen)
                {
                    data[byteLen - iCnt - 1] = hexArray[iCnt];
                    ++iCnt;
                }
                // If the system architecture is little-endian (that is, little end first), 
                // reverse the byte array. 
                //if (BitConverter.IsLittleEndian)
                //    Array.Reverse(data);

                result = BitConverter.ToUInt32(data, 0);
            }

            return result;
        }
        /// <summary>
        /// Convert Hex string to Int64.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static long HexToInt64(string value)
        {
            long result = 0;
            if (!value.IsHex())
            {
                return result;
            }

            int byteLen = GetSize(DataTypes.Long);
            int charLen = byteLen * 2;
            string val = value;
            while ((val.Length % charLen) != 0)
            {
                val = "0" + val; // append zero in front of string.
            }
            byte[] hexArray = HexStringToByteArray(val);
            if (null != hexArray && hexArray.Length > 0)
            {
                hexArray.Reverse(); // Reverse byte order
                byte[] data = new byte[byteLen];
                int iCnt = 0;
                int maxLen = (hexArray.Length > data.Length) ? data.Length : hexArray.Length;
                while (iCnt < maxLen)
                {
                    data[byteLen - iCnt - 1] = hexArray[iCnt];
                    ++iCnt;
                }
                // If the system architecture is little-endian (that is, little end first), 
                // reverse the byte array. 
                //if (BitConverter.IsLittleEndian)
                //    Array.Reverse(data);

                result = BitConverter.ToInt64(data, 0);
            }

            return result;
        }
        /// <summary>
        /// Convert Hex string to UInt64.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static ulong HexToUInt64(string value)
        {
            ulong result = 0;
            if (!value.IsHex())
            {
                return result;
            }

            int byteLen = GetSize(DataTypes.ULong);
            int charLen = byteLen * 2;
            string val = value;
            while ((val.Length % charLen) != 0)
            {
                val = "0" + val; // append zero in front of string.
            }
            byte[] hexArray = HexStringToByteArray(val);
            if (null != hexArray && hexArray.Length > 0)
            {
                hexArray.Reverse(); // Reverse byte order
                byte[] data = new byte[byteLen];
                int iCnt = 0;
                int maxLen = (hexArray.Length > data.Length) ? data.Length : hexArray.Length;
                while (iCnt < maxLen)
                {
                    data[byteLen - iCnt - 1] = hexArray[iCnt];
                    ++iCnt;
                }
                // If the system architecture is little-endian (that is, little end first), 
                // reverse the byte array. 
                //if (BitConverter.IsLittleEndian)
                //    Array.Reverse(data);

                result = BitConverter.ToUInt64(data, 0);
            }

            return result;
        }
        /// <summary>
        /// Convert Hex string to Single.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static float HexToSingle(string value)
        {
            float result = 0;
            if (!value.IsHex())
            {
                return result;
            }

            int byteLen = GetSize(DataTypes.Single);
            int charLen = byteLen * 2;
            string val = value;
            while ((val.Length % charLen) != 0)
            {
                val = "0" + val; // append zero in front of string.
            }
            byte[] hexArray = HexStringToByteArray(val);
            if (null != hexArray && hexArray.Length > 0)
            {
                hexArray.Reverse(); // Reverse byte order
                byte[] data = new byte[byteLen];
                int iCnt = 0;
                int maxLen = (hexArray.Length > data.Length) ? data.Length : hexArray.Length;
                while (iCnt < maxLen)
                {
                    data[byteLen - iCnt - 1] = hexArray[iCnt];
                    ++iCnt;
                }
                // If the system architecture is little-endian (that is, little end first), 
                // reverse the byte array. 
                //if (BitConverter.IsLittleEndian)
                //    Array.Reverse(data);

                result = BitConverter.ToSingle(data, 0);
            }

            return result;
        }
        /// <summary>
        /// Convert Hex string to Double.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        private static double HexToDouble(string value)
        {
            double result = 0;
            if (!value.IsHex())
            {
                return result;
            }

            int byteLen = GetSize(DataTypes.Double);
            int charLen = byteLen * 2;
            string val = value;
            while ((val.Length % charLen) != 0)
            {
                val = "0" + val; // append zero in front of string.
            }
            byte[] hexArray = HexStringToByteArray(val);
            if (null != hexArray && hexArray.Length > 0)
            {
                hexArray.Reverse(); // Reverse byte order
                byte[] data = new byte[byteLen];
                int iCnt = 0;
                int maxLen = (hexArray.Length > data.Length) ? data.Length : hexArray.Length;
                while (iCnt < maxLen)
                {
                    data[byteLen - iCnt - 1] = hexArray[iCnt];
                    ++iCnt;
                }
                // If the system architecture is little-endian (that is, little end first), 
                // reverse the byte array. 
                //if (BitConverter.IsLittleEndian)
                //    Array.Reverse(data);

                result = BitConverter.ToDouble(data, 0);
            }

            return result;
        }

        #endregion

        #endregion

        #region GetBytes (from ValueType)

        /// <summary>
        /// Convert Signed Byte to Target Type and returns output in byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dataType">The target data type.</param>
        /// <returns>Returns the result byte array.</returns>
        public static byte[] GetBytes(this sbyte value, DataTypes dataType)
        {
            byte[] results = null;
            // Gets Data Size and Target Size
            int dataSize = GetSize(value);
            int targetSize = GetSize(dataType);
            // Gets Value in byte array.
            byte[] hexArray = new byte[] { (byte)value };
            // Build output array.
            results = GetBytes(targetSize, dataSize, hexArray);
            return results;
        }
        /// <summary>
        /// Convert Byte to Target Type and returns output in byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dataType">The target data type.</param>
        /// <returns>Returns the result byte array.</returns>
        public static byte[] GetBytes(this byte value, DataTypes dataType)
        {
            byte[] results = null;
            // Gets Data Size and Target Size
            int dataSize = GetSize(value);
            int targetSize = GetSize(dataType);
            // Gets Value in byte array.
            byte[] hexArray = new byte[] { value };
            // Build output array.
            results = GetBytes(targetSize, dataSize, hexArray);
            return results;
        }
        /// <summary>
        /// Convert Int16 to Target Type and returns output in byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dataType">The target data type.</param>
        /// <returns>Returns the result byte array.</returns>
        public static byte[] GetBytes(this short value, DataTypes dataType)
        {
            byte[] results = null;
            // Gets Data Size and Target Size
            int dataSize = GetSize(value);
            int targetSize = GetSize(dataType);
            // Gets Value in byte array.
            byte[] hexArray = BitConverter.GetBytes(value);
            // If the system architecture is little-endian (that is, little end first), 
            // reverse the byte array. 
            if (BitConverter.IsLittleEndian)
                Array.Reverse(hexArray);
            // Build output array.
            results = GetBytes(targetSize, dataSize, hexArray);
            return results;
        }
        /// <summary>
        /// Convert UInt16 to Target Type and returns output in byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dataType">The target data type.</param>
        /// <returns>Returns the result byte array.</returns>
        public static byte[] GetBytes(this ushort value, DataTypes dataType)
        {
            byte[] results = null;
            // Gets Data Size and Target Size
            int dataSize = GetSize(value);
            int targetSize = GetSize(dataType);
            // Gets Value in byte array.
            byte[] hexArray = BitConverter.GetBytes(value);
            // If the system architecture is little-endian (that is, little end first), 
            // reverse the byte array. 
            if (BitConverter.IsLittleEndian)
                Array.Reverse(hexArray);
            // Build output array.
            results = GetBytes(targetSize, dataSize, hexArray);
            return results;
        }
        /// <summary>
        /// Convert Int32 to Target Type and returns output in byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dataType">The target data type.</param>
        /// <returns>Returns the result byte array.</returns>
        public static byte[] GetBytes(this int value, DataTypes dataType)
        {
            byte[] results = null;
            // Gets Data Size and Target Size
            int dataSize = GetSize(value);
            int targetSize = GetSize(dataType);
            // Gets Value in byte array.
            byte[] hexArray = BitConverter.GetBytes(value);
            // If the system architecture is little-endian (that is, little end first), 
            // reverse the byte array. 
            if (BitConverter.IsLittleEndian)
                Array.Reverse(hexArray);
            // Build output array.
            results = GetBytes(targetSize, dataSize, hexArray);
            return results;
        }
        /// <summary>
        /// Convert UInt32 to Target Type and returns output in byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dataType">The target data type.</param>
        /// <returns>Returns the result byte array.</returns>
        public static byte[] GetBytes(this uint value, DataTypes dataType)
        {
            byte[] results = null;
            // Gets Data Size and Target Size
            int dataSize = GetSize(value);
            int targetSize = GetSize(dataType);
            // Gets Value in byte array.
            byte[] hexArray = BitConverter.GetBytes(value);
            // If the system architecture is little-endian (that is, little end first), 
            // reverse the byte array. 
            if (BitConverter.IsLittleEndian)
                Array.Reverse(hexArray);
            // Build output array.
            results = GetBytes(targetSize, dataSize, hexArray);
            return results;
        }
        /// <summary>
        /// Convert Int64 to Target Type and returns output in byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dataType">The target data type.</param>
        /// <returns>Returns the result byte array.</returns>
        public static byte[] GetBytes(this long value, DataTypes dataType)
        {
            byte[] results = null;
            // Gets Data Size and Target Size
            int dataSize = GetSize(value);
            int targetSize = GetSize(dataType);
            // Gets Value in byte array.
            byte[] hexArray = BitConverter.GetBytes(value);
            // If the system architecture is little-endian (that is, little end first), 
            // reverse the byte array. 
            if (BitConverter.IsLittleEndian)
                Array.Reverse(hexArray);
            // Build output array.
            results = GetBytes(targetSize, dataSize, hexArray);
            return results;
        }
        /// <summary>
        /// Convert UInt64 to Target Type and returns output in byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dataType">The target data type.</param>
        /// <returns>Returns the result byte array.</returns>
        public static byte[] GetBytes(this ulong value, DataTypes dataType)
        {
            byte[] results = null;
            // Gets Data Size and Target Size
            int dataSize = GetSize(value);
            int targetSize = GetSize(dataType);
            // Gets Value in byte array.
            byte[] hexArray = BitConverter.GetBytes(value);
            // If the system architecture is little-endian (that is, little end first), 
            // reverse the byte array. 
            if (BitConverter.IsLittleEndian)
                Array.Reverse(hexArray);
            // Build output array.
            results = GetBytes(targetSize, dataSize, hexArray);
            return results;
        }
        /// <summary>
        /// Convert Single to Target Type and returns output in byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dataType">The target data type.</param>
        /// <returns>Returns the result byte array.</returns>
        public static byte[] GetBytes(this float value, DataTypes dataType)
        {
            byte[] results = null;
            // Gets Data Size and Target Size
            int dataSize = GetSize(value);
            int targetSize = GetSize(dataType);
            // Gets Value in byte array.
            byte[] hexArray = BitConverter.GetBytes(value);
            // If the system architecture is little-endian (that is, little end first), 
            // reverse the byte array. 
            if (BitConverter.IsLittleEndian)
                Array.Reverse(hexArray);
            // Build output array.
            results = GetBytes(targetSize, dataSize, hexArray);
            return results;
        }
        /// <summary>
        /// Convert Double to Target Type and returns output in byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dataType">The target data type.</param>
        /// <returns>Returns the result byte array.</returns>
        public static byte[] GetBytes(this double value, DataTypes dataType)
        {
            byte[] results = null;
            // Gets Data Size and Target Size
            int dataSize = GetSize(value);
            int targetSize = GetSize(dataType);
            // Gets Value in byte array.
            byte[] hexArray = BitConverter.GetBytes(value);
            // If the system architecture is little-endian (that is, little end first), 
            // reverse the byte array. 
            if (BitConverter.IsLittleEndian)
                Array.Reverse(hexArray);
            // Build output array.
            results = GetBytes(targetSize, dataSize, hexArray);
            return results;
        }

        #endregion

        #region GetBytes (from string)

        /// <summary>
        /// Convert string to Data Type and returns output in byte array.
        /// </summary>
        /// <param name="value">The string to convert and get result byte array.</param>
        /// <param name="dataType">The data type.</param>
        /// <param name="inputType">The input type.</param>
        /// <returns>Returns the result byte array.</returns>
        public static byte[] GetBytes(this string value, DataTypes dataType,
            InputTypes inputType)
        {
            byte[] results = null;
            if (string.IsNullOrWhiteSpace(value))
            {
                return results;
            }
            switch (dataType)
            {
                case DataTypes.SByte:
                    results = value.ToSByte(inputType).GetBytes(dataType);
                    break;
                case DataTypes.Byte:
                    results = value.ToByte(inputType).GetBytes(dataType);
                    break;
                case DataTypes.Short:
                    results = value.ToInt16(inputType).GetBytes(dataType);
                    break;
                case DataTypes.UShort:
                    results = value.ToUInt16(inputType).GetBytes(dataType);
                    break;
                case DataTypes.Int:
                    results = value.ToInt32(inputType).GetBytes(dataType);
                    break;
                case DataTypes.UInt:
                    results = value.ToUInt32(inputType).GetBytes(dataType);
                    break;
                case DataTypes.Long:
                    results = value.ToInt64(inputType).GetBytes(dataType);
                    break;
                case DataTypes.ULong:
                    results = value.ToUInt64(inputType).GetBytes(dataType);
                    break;
                case DataTypes.Single:
                    results = value.ToSingle(inputType).GetBytes(dataType);
                    break;
                case DataTypes.Double:
                    results = value.ToDouble(inputType).GetBytes(dataType);
                    break;
            }
            return results;
        }
        /// <summary>
        /// Convert Multiple lines value to list of byte array.
        /// </summary>
        /// <param name="values">The string values to get bytes.</param>
        /// <param name="dataType">The data type.</param>
        /// <param name="inputType">The input type.</param>
        /// <returns>Returns the result list of byte array.</returns>
        public static List<byte[]> GetBytes(this IList<string> values, DataTypes dataType,
            InputTypes inputType)
        {
            List<byte[]> results = null;

            if (null == values || values.Count <= 0)
                return results;

            results = new List<byte[]>();

            foreach (string line in values)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    results.Add(GetBytes(dataType));
                }
                else
                {
                    results.Add(line.GetBytes(dataType, inputType));
                }
            }

            return results;
        }

        #endregion

        #region To Value Types

        /// <summary>
        /// Convert string to Byte.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="inputType">The Input Type.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        public static byte ToByte(this string value,
            InputTypes inputType = InputTypes.Decimal)
        {
            if (inputType == InputTypes.Decimal)
                return DecimalToByte(value);
            else return HexToByte(value);
        }
        /// <summary>
        /// Convert string to Signed Byte.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="inputType">The Input Type.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        public static sbyte ToSByte(this string value,
            InputTypes inputType = InputTypes.Decimal)
        {
            if (inputType == InputTypes.Decimal)
                return DecimalToSByte(value);
            else return HexToSByte(value);
        }
        /// <summary>
        /// Convert string to Int16.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="inputType">The Input Type.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        public static short ToInt16(this string value,
            InputTypes inputType = InputTypes.Decimal)
        {
            if (inputType == InputTypes.Decimal)
                return DecimalToInt16(value);
            else return HexToInt16(value);
        }
        /// <summary>
        /// Convert string to UInt16.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="inputType">The Input Type.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        public static ushort ToUInt16(this string value,
            InputTypes inputType = InputTypes.Decimal)
        {
            if (inputType == InputTypes.Decimal)
                return DecimalToUInt16(value);
            else return HexToUInt16(value);
        }
        /// <summary>
        /// Convert string to Int32.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="inputType">The Input Type.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        public static int ToInt32(this string value,
            InputTypes inputType = InputTypes.Decimal)
        {
            if (inputType == InputTypes.Decimal)
                return DecimalToInt32(value);
            else return HexToInt32(value);
        }
        /// <summary>
        /// Convert string to UInt32.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="inputType">The Input Type.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        public static uint ToUInt32(this string value,
            InputTypes inputType = InputTypes.Decimal)
        {
            if (inputType == InputTypes.Decimal)
                return DecimalToUInt32(value);
            else return HexToUInt32(value);
        }
        /// <summary>
        /// Convert string to Int64.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="inputType">The Input Type.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        public static long ToInt64(this string value,
            InputTypes inputType = InputTypes.Decimal)
        {
            if (inputType == InputTypes.Decimal)
                return DecimalToInt64(value);
            else return HexToInt64(value);
        }
        /// <summary>
        /// Convert string to UInt64.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="inputType">The Input Type.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        public static ulong ToUInt64(this string value,
            InputTypes inputType = InputTypes.Decimal)
        {
            if (inputType == InputTypes.Decimal)
                return DecimalToUInt64(value);
            else return HexToUInt64(value);
        }
        /// <summary>
        /// Convert string to single.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="inputType">The Input Type.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        public static float ToSingle(this string value,
            InputTypes inputType = InputTypes.Decimal)
        {
            if (inputType == InputTypes.Decimal)
                return DecimalToSingle(value);
            else return HexToSingle(value);
        }
        /// <summary>
        /// Convert string to double.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="inputType">The Input Type.</param>
        /// <returns>Returns convert value if error zero returns.</returns>
        public static double ToDouble(this string value,
            InputTypes inputType = InputTypes.Decimal)
        {
            double result;
            if (string.IsNullOrWhiteSpace(value) ||
                !double.TryParse(value, out result))
            {
                result = 0;
            }
            return result;
        }

        #endregion

        #region Dec/Hex Methods

        /// <summary>
        /// Gets Hex String from bytes.
        /// </summary>
        /// <param name="values">The list of bytes.</param>
        /// <param name="addSpace">True for add space every byte.</param>
        /// <returns>Returns hex string.</returns>
        public static string ToHex(this IList<byte> values, bool addSpace = false)
        {
            string result = string.Empty;
            if (null == values || values.Count <= 0)
                return result;
            foreach (byte hex in values)
            {
                result += hex.ToString("X2");
                if (addSpace) result += " ";
            }
            return result.Trim();
        }
        /// <summary>
        /// Checks is string is hex string.
        /// </summary>
        /// <param name="value">The string to checks.</param>
        /// <returns>Returns true if string is hex string.</returns>
        public static bool IsHex(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            return r.Match(value).Success;
        }
        /// <summary>
        /// Gets Decimal String from bytes.
        /// </summary>
        /// <param name="values">The list of bytes.</param>
        /// <param name="dataType">The target decimal data type.</param>
        /// <returns>Returns decimal string.</returns>
        public static string ToDec(this IList<byte> values, DataTypes dataType)
        {
            string result = string.Empty;
            if (null == values || values.Count <= 0)
                return result;
            int byteLen = GetSize(dataType);
            List<byte> caches = new List<byte>(values);
            byte[] buffs = new byte[byteLen];
            int iCnt = 0;
            switch (dataType)
            {
                case DataTypes.SByte:
                    result = ((sbyte)caches[caches.Count - 1]).ToString();
                    break;
                case DataTypes.Byte:
                    result = caches[caches.Count - 1].ToString();
                    break;
                case DataTypes.Short:
                    //caches.Reverse(); // Reverse byte order
                    iCnt = 0;
                    while (iCnt < caches.Count)
                    {
                        buffs[byteLen - iCnt - 1] = caches[iCnt];
                        ++iCnt;
                    }
                    // If the system architecture is little-endian (that is, little end first), 
                    // reverse the byte array. 
                    //if (BitConverter.IsLittleEndian)
                    //    Array.Reverse(buffs);
                    result = BitConverter.ToInt16(buffs, 0).ToString();
                    break;
                case DataTypes.UShort:
                    //caches.Reverse(); // Reverse byte order
                    iCnt = 0;
                    while (iCnt < caches.Count)
                    {
                        buffs[byteLen - iCnt - 1] = caches[iCnt];
                        ++iCnt;
                    }
                    // If the system architecture is little-endian (that is, little end first), 
                    // reverse the byte array. 
                    //if (BitConverter.IsLittleEndian)
                    //    Array.Reverse(buffs);
                    result = BitConverter.ToUInt16(buffs, 0).ToString();
                    break;
                case DataTypes.Int:
                    //caches.Reverse(); // Reverse byte order
                    iCnt = 0;
                    while (iCnt < caches.Count)
                    {
                        buffs[byteLen - iCnt - 1] = caches[iCnt];
                        ++iCnt;
                    }
                    // If the system architecture is little-endian (that is, little end first), 
                    // reverse the byte array. 
                    //if (BitConverter.IsLittleEndian)
                    //    Array.Reverse(buffs);
                    result = BitConverter.ToInt32(buffs, 0).ToString();
                    break;
                case DataTypes.UInt:
                    //caches.Reverse(); // Reverse byte order
                    iCnt = 0;
                    while (iCnt < caches.Count)
                    {
                        buffs[byteLen - iCnt - 1] = caches[iCnt];
                        ++iCnt;
                    }
                    // If the system architecture is little-endian (that is, little end first), 
                    // reverse the byte array. 
                    //if (BitConverter.IsLittleEndian)
                    //    Array.Reverse(buffs);
                    result = BitConverter.ToUInt32(buffs, 0).ToString();
                    break;
                case DataTypes.Long:
                    //caches.Reverse(); // Reverse byte order
                    iCnt = 0;
                    while (iCnt < caches.Count)
                    {
                        buffs[byteLen - iCnt - 1] = caches[iCnt];
                        ++iCnt;
                    }
                    // If the system architecture is little-endian (that is, little end first), 
                    // reverse the byte array. 
                    //if (BitConverter.IsLittleEndian)
                    //    Array.Reverse(buffs);
                    result = BitConverter.ToInt64(buffs, 0).ToString();
                    break;
                case DataTypes.ULong:
                    //caches.Reverse(); // Reverse byte order
                    iCnt = 0;
                    while (iCnt < caches.Count)
                    {
                        buffs[byteLen - iCnt - 1] = caches[iCnt];
                        ++iCnt;
                    }
                    // If the system architecture is little-endian (that is, little end first), 
                    // reverse the byte array. 
                    //if (BitConverter.IsLittleEndian)
                    //    Array.Reverse(buffs);
                    result = BitConverter.ToUInt64(buffs, 0).ToString();
                    break;
                case DataTypes.Single:
                    //caches.Reverse(); // Reverse byte order
                    iCnt = 0;
                    while (iCnt < caches.Count)
                    {
                        buffs[byteLen - iCnt - 1] = caches[iCnt];
                        ++iCnt;
                    }
                    // If the system architecture is little-endian (that is, little end first), 
                    // reverse the byte array. 
                    //if (BitConverter.IsLittleEndian)
                    //    Array.Reverse(buffs);
                    result = BitConverter.ToSingle(buffs, 0).ToString();
                    break;
                case DataTypes.Double:
                    //caches.Reverse(); // Reverse byte order
                    iCnt = 0;
                    while (iCnt < caches.Count)
                    {
                        buffs[byteLen - iCnt - 1] = caches[iCnt];
                        ++iCnt;
                    }
                    // If the system architecture is little-endian (that is, little end first), 
                    // reverse the byte array. 
                    //if (BitConverter.IsLittleEndian)
                    //    Array.Reverse(buffs);
                    result = BitConverter.ToDouble(buffs, 0).ToString();
                    break;
            }
            return result;
        }

        #endregion
    }

    #endregion
}
