﻿#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-08-19
=================
- Convert class ported from NLib40x3 to NLib40x5.

======================================================================================================================
Update 2009-12-25
=================
- Convert Utils class changed.
  - ConvertUtils move from GFA37.
  - Change class name from ConvertUtils to Converts.
  - Add new class. The Convert class (generic version) added. Used for converts a base data type to 
    another base data type.
  - Add new class. The ConvertTo class (generic version) added. Used for converts a base data type to 
    another base data type.

======================================================================================================================
Update 2008-11-27
=================
- ConvertUtils move from GFA.Lib to GFA.Lib.Core.

======================================================================================================================
Update 2007-11-26
=================
- ConvertUtils add some fixed for multithread environment and optimized
  for performance

======================================================================================================================
// </[History]>

#endif
#endregion

namespace NLib.Utils
{
    #region Using

    using System;
    using System.Collections;
    using System.Data.SqlTypes;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using NLib.Reflection;

    #endregion

    #region Converts

    /// <summary>
    /// Convert class. A wrapper to convert value to specificed type
    /// </summary>
    public class Converts
    {
        #region Scalar Types

        #region String

        // Scalar Types.

        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(SByte p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Int16 p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Int32 p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Int64 p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Byte p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(UInt16 p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(UInt32 p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(UInt64 p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Single p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Double p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Boolean p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Decimal p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Char p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(TimeSpan p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(DateTime p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Guid p) { return p.ToString(); }

        // Nullable Types.
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(SByte? p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Int16? p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Int32? p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Int64? p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Byte? p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(UInt16? p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(UInt32? p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(UInt64? p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Single? p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Double? p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Boolean? p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Decimal? p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Char? p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(TimeSpan? p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(DateTime? p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Guid? p) { return p.ToString(); }

        // SqlTypes.
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(SqlString p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(SqlByte p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(SqlInt16 p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(SqlInt32 p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(SqlInt64 p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(SqlSingle p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(SqlDouble p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(SqlDecimal p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(SqlMoney p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(SqlBoolean p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(SqlGuid p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(SqlBinary p) { return p.ToString(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(Type p) { return p == null ? null : p.FullName; }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>string that convert from parater value</returns>
        public static String ToString(object p)
        {
            if (p == null) return String.Empty;

            // Scalar Types.
            //
            if (p is Char) return ToString((Char)p);
            if (p is TimeSpan) return ToString((TimeSpan)p);
            if (p is DateTime) return ToString((DateTime)p);
            if (p is Guid) return ToString((Guid)p);

            // Nullable Types.
            //
            if (p is Char?) return ToString((Char?)p);
            if (p is TimeSpan?) return ToString((TimeSpan?)p);
            if (p is DateTime?) return ToString((DateTime?)p);
            if (p is Guid?) return ToString((Guid?)p);

            // SqlTypes.
            //
            if (p is SqlGuid) return ToString((SqlGuid)p);
            if (p is SqlBinary) return ToString((SqlBinary)p);
            if (p is Type) return ToString((Type)p);

            if (p is IConvertible) return ((IConvertible)p).ToString(null);
            if (p is IFormattable) return ((IFormattable)p).ToString(null, null);

            return p.ToString();
        }

        #endregion

        #region SByte

        // Scalar Types.

        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(String p) { return p == null ? (SByte)0 : SByte.Parse(p); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Int16 p) { return checked((SByte)p); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Int32 p) { return checked((SByte)p); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Int64 p) { return checked((SByte)p); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Byte p) { return checked((SByte)p); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(UInt16 p) { return checked((SByte)p); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(UInt32 p) { return checked((SByte)p); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(UInt64 p) { return checked((SByte)p); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Single p) { return checked((SByte)p); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Double p) { return checked((SByte)p); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Decimal p) { return checked((SByte)p); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Boolean p) { return (SByte)(p ? 1 : 0); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Char p) { return checked((SByte)p); }

        // Nullable Types.

        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(SByte? p) { return p.HasValue ? p.Value : (SByte)0; }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Int16? p) { return p.HasValue ? checked((SByte)p.Value) : (SByte)0; }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Int32? p) { return p.HasValue ? checked((SByte)p.Value) : (SByte)0; }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Int64? p) { return p.HasValue ? checked((SByte)p.Value) : (SByte)0; }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Byte? p) { return p.HasValue ? checked((SByte)p.Value) : (SByte)0; }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(UInt16? p) { return p.HasValue ? checked((SByte)p.Value) : (SByte)0; }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(UInt32? p) { return p.HasValue ? checked((SByte)p.Value) : (SByte)0; }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(UInt64? p) { return p.HasValue ? checked((SByte)p.Value) : (SByte)0; }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Single? p) { return p.HasValue ? checked((SByte)p.Value) : (SByte)0; }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Double? p) { return p.HasValue ? checked((SByte)p.Value) : (SByte)0; }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Decimal? p) { return p.HasValue ? checked((SByte)p.Value) : (SByte)0; }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Char? p) { return p.HasValue ? checked((SByte)p.Value) : (SByte)0; }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(Boolean? p) { return (p.HasValue && p.Value) ? (SByte)1 : (SByte)0; }

        // SqlTypes.

        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(SqlString p) { return p.IsNull ? (SByte)0 : ToSByte(p.Value); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(SqlByte p) { return p.IsNull ? (SByte)0 : ToSByte(p.Value); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(SqlInt16 p) { return p.IsNull ? (SByte)0 : ToSByte(p.Value); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(SqlInt32 p) { return p.IsNull ? (SByte)0 : ToSByte(p.Value); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(SqlInt64 p) { return p.IsNull ? (SByte)0 : ToSByte(p.Value); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(SqlSingle p) { return p.IsNull ? (SByte)0 : ToSByte(p.Value); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(SqlDouble p) { return p.IsNull ? (SByte)0 : ToSByte(p.Value); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(SqlDecimal p) { return p.IsNull ? (SByte)0 : ToSByte(p.Value); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(SqlMoney p) { return p.IsNull ? (SByte)0 : ToSByte(p.Value); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(SqlBoolean p) { return p.IsNull ? (SByte)0 : ToSByte(p.Value); }
        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte that convert from parater value</returns>

        public static SByte ToSByte(object p)
        {
            if (p == null) return 0;

            // Scalar Types.
            //
            if (p is String) return ToSByte((String)p);

            if (p is Boolean) return ToSByte((Boolean)p);
            if (p is Char) return ToSByte((Char)p);

            // Nullable Types.
            //
            if (p is SByte?) return ToSByte((SByte?)p);

            if (p is Char?) return ToSByte((Char?)p);
            if (p is Boolean?) return ToSByte((Boolean?)p);

            // SqlTypes.
            //

            if (p is IConvertible) return ((IConvertible)p).ToSByte(null);

            throw CreateInvalidCastException(p.GetType(), typeof(SByte));
        }

        #endregion

        #region Int16

        // Scalar Types.

        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(String p) { return p == null ? (Int16)0 : Int16.Parse(p); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>

        public static Int16 ToInt16(SByte p) { return checked((Int16)p); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Int32 p) { return checked((Int16)p); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Int64 p) { return checked((Int16)p); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Byte p) { return checked((Int16)p); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>

        public static Int16 ToInt16(UInt16 p) { return checked((Int16)p); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>

        public static Int16 ToInt16(UInt32 p) { return checked((Int16)p); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>

        public static Int16 ToInt16(UInt64 p) { return checked((Int16)p); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Single p) { return checked((Int16)p); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Double p) { return checked((Int16)p); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Decimal p) { return checked((Int16)p); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Boolean p) { return (Int16)(p ? 1 : 0); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Char p) { return checked((Int16)p); }

        // Nullable Types.

        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Int16? p) { return p.HasValue ? p.Value : (Int16)0; }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>

        public static Int16 ToInt16(SByte? p) { return p.HasValue ? checked((Int16)p.Value) : (Int16)0; }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Int32? p) { return p.HasValue ? checked((Int16)p.Value) : (Int16)0; }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Int64? p) { return p.HasValue ? checked((Int16)p.Value) : (Int16)0; }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Byte? p) { return p.HasValue ? checked((Int16)p.Value) : (Int16)0; }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>

        public static Int16 ToInt16(UInt16? p) { return p.HasValue ? checked((Int16)p.Value) : (Int16)0; }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>

        public static Int16 ToInt16(UInt32? p) { return p.HasValue ? checked((Int16)p.Value) : (Int16)0; }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>

        public static Int16 ToInt16(UInt64? p) { return p.HasValue ? checked((Int16)p.Value) : (Int16)0; }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Single? p) { return p.HasValue ? checked((Int16)p.Value) : (Int16)0; }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Double? p) { return p.HasValue ? checked((Int16)p.Value) : (Int16)0; }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Decimal? p) { return p.HasValue ? checked((Int16)p.Value) : (Int16)0; }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Char? p) { return p.HasValue ? checked((Int16)p.Value) : (Int16)0; }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(Boolean? p) { return (p.HasValue && p.Value) ? (Int16)1 : (Int16)0; }

        // SqlTypes.
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(SqlInt16 p) { return p.IsNull ? (Int16)0 : p.Value; }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(SqlString p) { return p.IsNull ? (Int16)0 : ToInt16(p.Value); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(SqlByte p) { return p.IsNull ? (Int16)0 : ToInt16(p.Value); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(SqlInt32 p) { return p.IsNull ? (Int16)0 : ToInt16(p.Value); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(SqlInt64 p) { return p.IsNull ? (Int16)0 : ToInt16(p.Value); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(SqlSingle p) { return p.IsNull ? (Int16)0 : ToInt16(p.Value); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(SqlDouble p) { return p.IsNull ? (Int16)0 : ToInt16(p.Value); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(SqlDecimal p) { return p.IsNull ? (Int16)0 : ToInt16(p.Value); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(SqlMoney p) { return p.IsNull ? (Int16)0 : ToInt16(p.Value); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(SqlBoolean p) { return p.IsNull ? (Int16)0 : ToInt16(p.Value); }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16 that convert from parater value</returns>
        public static Int16 ToInt16(object p)
        {
            if (p == null) return 0;

            // Scalar Types.
            //
            if (p is String) return ToInt16((String)p);

            if (p is Boolean) return ToInt16((Boolean)p);
            if (p is Char) return ToInt16((Char)p);

            // Nullable Types.
            //
            if (p is Int16?) return ToInt16((Int16?)p);

            if (p is Char?) return ToInt16((Char?)p);
            if (p is Boolean?) return ToInt16((Boolean?)p);

            // SqlTypes.
            //
            if (p is SqlInt16) return ToInt16((SqlInt16)p);

            if (p is IConvertible) return ((IConvertible)p).ToInt16(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Int16));
        }

        #endregion

        #region Int32

        // Scalar Types.
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(String p) { return p == null ? 0 : Int32.Parse(p); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>

        public static Int32 ToInt32(SByte p) { return checked((Int32)p); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Int16 p) { return checked((Int32)p); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Int64 p) { return checked((Int32)p); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Byte p) { return checked((Int32)p); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>

        public static Int32 ToInt32(UInt16 p) { return checked((Int32)p); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>

        public static Int32 ToInt32(UInt32 p) { return checked((Int32)p); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>

        public static Int32 ToInt32(UInt64 p) { return checked((Int32)p); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Single p) { return checked((Int32)p); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Double p) { return checked((Int32)p); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Decimal p) { return checked((Int32)p); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Boolean p) { return p ? 1 : 0; }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Char p) { return checked((Int32)p); }

        // Nullable Types.
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Int32? p) { return p.HasValue ? p.Value : 0; }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>

        public static Int32 ToInt32(SByte? p) { return p.HasValue ? checked((Int32)p.Value) : 0; }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Int16? p) { return p.HasValue ? checked((Int32)p.Value) : 0; }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Int64? p) { return p.HasValue ? checked((Int32)p.Value) : 0; }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Byte? p) { return p.HasValue ? checked((Int32)p.Value) : 0; }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>

        public static Int32 ToInt32(UInt16? p) { return p.HasValue ? checked((Int32)p.Value) : 0; }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>

        public static Int32 ToInt32(UInt32? p) { return p.HasValue ? checked((Int32)p.Value) : 0; }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>

        public static Int32 ToInt32(UInt64? p) { return p.HasValue ? checked((Int32)p.Value) : 0; }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Single? p) { return p.HasValue ? checked((Int32)p.Value) : 0; }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Double? p) { return p.HasValue ? checked((Int32)p.Value) : 0; }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Decimal? p) { return p.HasValue ? checked((Int32)p.Value) : 0; }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Char? p) { return p.HasValue ? checked((Int32)p.Value) : 0; }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(Boolean? p) { return (p.HasValue && p.Value) ? 1 : 0; }

        // SqlTypes.
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(SqlInt32 p) { return p.IsNull ? 0 : p.Value; }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(SqlString p) { return p.IsNull ? 0 : ToInt32(p.Value); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(SqlByte p) { return p.IsNull ? 0 : ToInt32(p.Value); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(SqlInt16 p) { return p.IsNull ? 0 : ToInt32(p.Value); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(SqlInt64 p) { return p.IsNull ? 0 : ToInt32(p.Value); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(SqlSingle p) { return p.IsNull ? 0 : ToInt32(p.Value); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(SqlDouble p) { return p.IsNull ? 0 : ToInt32(p.Value); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(SqlDecimal p) { return p.IsNull ? 0 : ToInt32(p.Value); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(SqlMoney p) { return p.IsNull ? 0 : ToInt32(p.Value); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(SqlBoolean p) { return p.IsNull ? 0 : ToInt32(p.Value); }
        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32 that convert from parater value</returns>
        public static Int32 ToInt32(object p)
        {
            if (p == null) return 0;

            // Scalar Types.
            //
            if (p is String) return ToInt32((String)p);

            if (p is Boolean) return ToInt32((Boolean)p);
            if (p is Char) return ToInt32((Char)p);

            // Nullable Types.
            //
            if (p is Int32?) return ToInt32((Int32?)p);

            if (p is Char?) return ToInt32((Char?)p);
            if (p is Boolean?) return ToInt32((Boolean?)p);

            // SqlTypes.
            //
            if (p is SqlInt32) return ToInt32((SqlInt32)p);

            if (p is IConvertible) return ((IConvertible)p).ToInt32(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Int32));
        }

        #endregion

        #region Int64

        // Scalar Types.
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(String p) { return p == null ? 0 : Int64.Parse(p); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>

        public static Int64 ToInt64(SByte p) { return checked((Int64)p); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Int16 p) { return checked((Int64)p); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Int32 p) { return checked((Int64)p); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Byte p) { return checked((Int64)p); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>

        public static Int64 ToInt64(UInt16 p) { return checked((Int64)p); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>

        public static Int64 ToInt64(UInt32 p) { return checked((Int64)p); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>

        public static Int64 ToInt64(UInt64 p) { return checked((Int64)p); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Single p) { return checked((Int64)p); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Double p) { return checked((Int64)p); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Decimal p) { return checked((Int64)p); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Char p) { return checked((Int64)p); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Boolean p) { return p ? 1 : 0; }
        /// <summary>
        /// Int64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(DateTime p) { return (p - DateTime.MinValue).Ticks; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(TimeSpan p) { return p.Ticks; }

        // Nullable Types.
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Int64? p) { return p.HasValue ? p.Value : 0; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>

        public static Int64 ToInt64(SByte? p) { return p.HasValue ? checked((Int64)p.Value) : 0; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Int16? p) { return p.HasValue ? checked((Int64)p.Value) : 0; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Int32? p) { return p.HasValue ? checked((Int64)p.Value) : 0; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Byte? p) { return p.HasValue ? checked((Int64)p.Value) : 0; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>

        public static Int64 ToInt64(UInt16? p) { return p.HasValue ? checked((Int64)p.Value) : 0; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>

        public static Int64 ToInt64(UInt32? p) { return p.HasValue ? checked((Int64)p.Value) : 0; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>

        public static Int64 ToInt64(UInt64? p) { return p.HasValue ? checked((Int64)p.Value) : 0; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Single? p) { return p.HasValue ? checked((Int64)p.Value) : 0; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Double? p) { return p.HasValue ? checked((Int64)p.Value) : 0; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Decimal? p) { return p.HasValue ? checked((Int64)p.Value) : 0; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Char? p) { return p.HasValue ? checked((Int64)p.Value) : 0; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(Boolean? p) { return (p.HasValue && p.Value) ? 1 : 0; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(DateTime? p) { return p.HasValue ? (p.Value - DateTime.MinValue).Ticks : 0; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(TimeSpan? p) { return p.HasValue ? p.Value.Ticks : 0; }

        // SqlTypes.
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(SqlInt64 p) { return p.IsNull ? 0 : p.Value; }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(SqlString p) { return p.IsNull ? 0 : ToInt64(p.Value); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(SqlByte p) { return p.IsNull ? 0 : ToInt64(p.Value); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(SqlInt16 p) { return p.IsNull ? 0 : ToInt64(p.Value); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(SqlInt32 p) { return p.IsNull ? 0 : ToInt64(p.Value); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(SqlSingle p) { return p.IsNull ? 0 : ToInt64(p.Value); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(SqlDouble p) { return p.IsNull ? 0 : ToInt64(p.Value); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(SqlDecimal p) { return p.IsNull ? 0 : ToInt64(p.Value); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(SqlMoney p) { return p.IsNull ? 0 : ToInt64(p.Value); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(SqlBoolean p) { return p.IsNull ? 0 : ToInt64(p.Value); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(SqlDateTime p) { return p.IsNull ? 0 : ToInt64(p.Value); }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64 that convert from parater value</returns>
        public static Int64 ToInt64(object p)
        {
            if (p == null) return 0;

            // Scalar Types.
            //
            if (p is String) return ToInt64((String)p);

            if (p is Char) return ToInt64((Char)p);
            if (p is Boolean) return ToInt64((Boolean)p);
            if (p is DateTime) return ToInt64((DateTime)p);
            if (p is TimeSpan) return ToInt64((TimeSpan)p);

            // Nullable Types.
            //
            if (p is Int64?) return ToInt64((Int64?)p);

            if (p is Char?) return ToInt64((Char?)p);
            if (p is Boolean?) return ToInt64((Boolean?)p);
            if (p is DateTime?) return ToInt64((DateTime?)p);
            if (p is TimeSpan?) return ToInt64((TimeSpan?)p);

            // SqlTypes.
            //
            if (p is SqlInt64) return ToInt64((SqlInt64)p);
            if (p is SqlDateTime) return ToInt64((SqlDateTime)p);

            if (p is IConvertible) return ((IConvertible)p).ToInt64(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Int64));
        }

        #endregion

        #region Byte

        // Scalar Types.
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(String p) { return p == null ? (Byte)0 : Byte.Parse(p); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>

        public static Byte ToByte(SByte p) { return checked((Byte)p); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Int16 p) { return checked((Byte)p); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Int32 p) { return checked((Byte)p); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Int64 p) { return checked((Byte)p); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>

        public static Byte ToByte(UInt16 p) { return checked((Byte)p); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>

        public static Byte ToByte(UInt32 p) { return checked((Byte)p); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>

        public static Byte ToByte(UInt64 p) { return checked((Byte)p); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Single p) { return checked((Byte)p); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Double p) { return checked((Byte)p); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Decimal p) { return checked((Byte)p); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Boolean p) { return (Byte)(p ? 1 : 0); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Char p) { return checked((Byte)p); }

        // Nullable Types.
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Byte? p) { return p.HasValue ? p.Value : (Byte)0; }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>

        public static Byte ToByte(SByte? p) { return p.HasValue ? checked((Byte)p.Value) : (Byte)0; }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Int16? p) { return p.HasValue ? checked((Byte)p.Value) : (Byte)0; }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Int32? p) { return p.HasValue ? checked((Byte)p.Value) : (Byte)0; }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Int64? p) { return p.HasValue ? checked((Byte)p.Value) : (Byte)0; }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>

        public static Byte ToByte(UInt16? p) { return p.HasValue ? checked((Byte)p.Value) : (Byte)0; }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>

        public static Byte ToByte(UInt32? p) { return p.HasValue ? checked((Byte)p.Value) : (Byte)0; }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>

        public static Byte ToByte(UInt64? p) { return p.HasValue ? checked((Byte)p.Value) : (Byte)0; }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Single? p) { return p.HasValue ? checked((Byte)p.Value) : (Byte)0; }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Double? p) { return p.HasValue ? checked((Byte)p.Value) : (Byte)0; }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Decimal? p) { return p.HasValue ? checked((Byte)p.Value) : (Byte)0; }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Char? p) { return p.HasValue ? checked((Byte)p.Value) : (Byte)0; }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(Boolean? p) { return (p.HasValue && p.Value) ? (Byte)1 : (Byte)0; }

        // SqlTypes.
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(SqlByte p) { return p.IsNull ? (Byte)0 : p.Value; }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(SqlString p) { return p.IsNull ? (Byte)0 : ToByte(p.Value); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(SqlInt16 p) { return p.IsNull ? (Byte)0 : ToByte(p.Value); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(SqlInt32 p) { return p.IsNull ? (Byte)0 : ToByte(p.Value); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(SqlInt64 p) { return p.IsNull ? (Byte)0 : ToByte(p.Value); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(SqlSingle p) { return p.IsNull ? (Byte)0 : ToByte(p.Value); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(SqlDouble p) { return p.IsNull ? (Byte)0 : ToByte(p.Value); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(SqlDecimal p) { return p.IsNull ? (Byte)0 : ToByte(p.Value); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(SqlMoney p) { return p.IsNull ? (Byte)0 : ToByte(p.Value); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(SqlBoolean p) { return p.IsNull ? (Byte)0 : ToByte(p.Value); }
        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte that convert from parater value</returns>
        public static Byte ToByte(object p)
        {
            if (p == null) return Byte.MinValue;

            // Scalar Types.
            //
            if (p is String) return ToByte((String)p);

            if (p is Boolean) return ToByte((Boolean)p);
            if (p is Char) return ToByte((Char)p);

            // Nullable Types.
            //
            if (p is Byte?) return ToByte((Byte?)p);

            if (p is Char?) return ToByte((Char?)p);
            if (p is Boolean?) return ToByte((Boolean?)p);

            // SqlTypes.
            //
            if (p is SqlByte) return ToByte((SqlByte)p);

            if (p is IConvertible) return ((IConvertible)p).ToByte(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Byte));
        }

        #endregion

        #region UInt16

        // Scalar Types.
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(String p) { return p == null ? (UInt16)0 : UInt16.Parse(p); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(SByte p) { return checked((UInt16)p); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Int16 p) { return checked((UInt16)p); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Int32 p) { return checked((UInt16)p); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Int64 p) { return checked((UInt16)p); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Byte p) { return checked((UInt16)p); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(UInt32 p) { return checked((UInt16)p); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(UInt64 p) { return checked((UInt16)p); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Single p) { return checked((UInt16)p); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Double p) { return checked((UInt16)p); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Decimal p) { return checked((UInt16)p); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Boolean p) { return (UInt16)(p ? 1 : 0); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Char p) { return checked((UInt16)p); }

        // Nullable Types.
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(UInt16? p) { return p.HasValue ? p.Value : (UInt16)0; }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(SByte? p) { return p.HasValue ? checked((UInt16)p.Value) : (UInt16)0; }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Int16? p) { return p.HasValue ? checked((UInt16)p.Value) : (UInt16)0; }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Int32? p) { return p.HasValue ? checked((UInt16)p.Value) : (UInt16)0; }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Int64? p) { return p.HasValue ? checked((UInt16)p.Value) : (UInt16)0; }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Byte? p) { return p.HasValue ? checked((UInt16)p.Value) : (UInt16)0; }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(UInt32? p) { return p.HasValue ? checked((UInt16)p.Value) : (UInt16)0; }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(UInt64? p) { return p.HasValue ? checked((UInt16)p.Value) : (UInt16)0; }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Single? p) { return p.HasValue ? checked((UInt16)p.Value) : (UInt16)0; }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Double? p) { return p.HasValue ? checked((UInt16)p.Value) : (UInt16)0; }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Decimal? p) { return p.HasValue ? checked((UInt16)p.Value) : (UInt16)0; }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Char? p) { return p.HasValue ? checked((UInt16)p.Value) : (UInt16)0; }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(Boolean? p) { return (p.HasValue && p.Value) ? (UInt16)1 : (UInt16)0; }

        // SqlTypes.
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(SqlString p) { return p.IsNull ? (UInt16)0 : ToUInt16(p.Value); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(SqlByte p) { return p.IsNull ? (UInt16)0 : ToUInt16(p.Value); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(SqlInt16 p) { return p.IsNull ? (UInt16)0 : ToUInt16(p.Value); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(SqlInt32 p) { return p.IsNull ? (UInt16)0 : ToUInt16(p.Value); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(SqlInt64 p) { return p.IsNull ? (UInt16)0 : ToUInt16(p.Value); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(SqlSingle p) { return p.IsNull ? (UInt16)0 : ToUInt16(p.Value); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(SqlDouble p) { return p.IsNull ? (UInt16)0 : ToUInt16(p.Value); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(SqlDecimal p) { return p.IsNull ? (UInt16)0 : ToUInt16(p.Value); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(SqlMoney p) { return p.IsNull ? (UInt16)0 : ToUInt16(p.Value); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(SqlBoolean p) { return p.IsNull ? (UInt16)0 : ToUInt16(p.Value); }
        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16 that convert from parater value</returns>

        public static UInt16 ToUInt16(object p)
        {
            if (p == null) return 0;

            // Scalar Types.
            //
            if (p is String) return ToUInt16((String)p);

            if (p is Boolean) return ToUInt16((Boolean)p);
            if (p is Char) return ToUInt16((Char)p);

            // Nullable Types.
            //
            if (p is UInt16?) return ToUInt16((UInt16?)p);

            if (p is Char?) return ToUInt16((Char?)p);
            if (p is Boolean?) return ToUInt16((Boolean?)p);

            // SqlTypes.
            //

            if (p is IConvertible) return ((IConvertible)p).ToUInt16(null);

            throw CreateInvalidCastException(p.GetType(), typeof(UInt16));
        }

        #endregion

        #region UInt32

        // Scalar Types.
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(String p) { return p == null ? (UInt32)0 : UInt32.Parse(p); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(SByte p) { return checked((UInt32)p); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Int16 p) { return checked((UInt32)p); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Int32 p) { return checked((UInt32)p); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Int64 p) { return checked((UInt32)p); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Byte p) { return checked((UInt32)p); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(UInt16 p) { return checked((UInt32)p); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(UInt64 p) { return checked((UInt32)p); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Single p) { return checked((UInt32)p); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Double p) { return checked((UInt32)p); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Decimal p) { return checked((UInt32)p); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Boolean p) { return (UInt32)(p ? 1 : 0); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Char p) { return checked((UInt32)p); }

        // Nullable Types.
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(UInt32? p) { return p.HasValue ? p.Value : (UInt32)0; }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(SByte? p) { return p.HasValue ? checked((UInt32)p.Value) : (UInt32)0; }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Int16? p) { return p.HasValue ? checked((UInt32)p.Value) : (UInt32)0; }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Int32? p) { return p.HasValue ? checked((UInt32)p.Value) : (UInt32)0; }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Int64? p) { return p.HasValue ? checked((UInt32)p.Value) : (UInt32)0; }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Byte? p) { return p.HasValue ? checked((UInt32)p.Value) : (UInt32)0; }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(UInt16? p) { return p.HasValue ? checked((UInt32)p.Value) : (UInt32)0; }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(UInt64? p) { return p.HasValue ? checked((UInt32)p.Value) : (UInt32)0; }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Single? p) { return p.HasValue ? checked((UInt32)p.Value) : (UInt32)0; }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Double? p) { return p.HasValue ? checked((UInt32)p.Value) : (UInt32)0; }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Decimal? p) { return p.HasValue ? checked((UInt32)p.Value) : (UInt32)0; }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Char? p) { return p.HasValue ? checked((UInt32)p.Value) : (UInt32)0; }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(Boolean? p) { return (p.HasValue && p.Value) ? (UInt32)1 : (UInt32)0; }

        // SqlTypes.
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(SqlString p) { return p.IsNull ? (UInt32)0 : ToUInt32(p.Value); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(SqlByte p) { return p.IsNull ? (UInt32)0 : ToUInt32(p.Value); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(SqlInt16 p) { return p.IsNull ? (UInt32)0 : ToUInt32(p.Value); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(SqlInt32 p) { return p.IsNull ? (UInt32)0 : ToUInt32(p.Value); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(SqlInt64 p) { return p.IsNull ? (UInt32)0 : ToUInt32(p.Value); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(SqlSingle p) { return p.IsNull ? (UInt32)0 : ToUInt32(p.Value); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(SqlDouble p) { return p.IsNull ? (UInt32)0 : ToUInt32(p.Value); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(SqlDecimal p) { return p.IsNull ? (UInt32)0 : ToUInt32(p.Value); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(SqlMoney p) { return p.IsNull ? (UInt32)0 : ToUInt32(p.Value); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(SqlBoolean p) { return p.IsNull ? (UInt32)0 : ToUInt32(p.Value); }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32 that convert from parater value</returns>

        public static UInt32 ToUInt32(object p)
        {
            if (p == null) return UInt32.MinValue;

            // Scalar Types.
            //
            if (p is String) return ToUInt32((String)p);

            if (p is Boolean) return ToUInt32((Boolean)p);
            if (p is Char) return ToUInt32((Char)p);

            // Nullable Types.
            //
            if (p is UInt32?) return ToUInt32((UInt32?)p);

            if (p is Char?) return ToUInt32((Char?)p);
            if (p is Boolean?) return ToUInt32((Boolean?)p);

            // SqlTypes.
            //

            if (p is IConvertible) return ((IConvertible)p).ToUInt32(null);

            throw CreateInvalidCastException(p.GetType(), typeof(UInt32));
        }

        #endregion

        #region UInt64

        // Scalar Types.
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(String p) { return p == null ? (UInt64)0 : UInt64.Parse(p); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(SByte p) { return checked((UInt64)p); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Int16 p) { return checked((UInt64)p); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Int32 p) { return checked((UInt64)p); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Int64 p) { return checked((UInt64)p); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Byte p) { return checked((UInt64)p); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(UInt16 p) { return checked((UInt64)p); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(UInt32 p) { return checked((UInt64)p); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Single p) { return checked((UInt64)p); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Double p) { return checked((UInt64)p); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Decimal p) { return checked((UInt64)p); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Boolean p) { return (UInt64)(p ? 1 : 0); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Char p) { return checked((UInt64)p); }

        // Nullable Types.
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(UInt64? p) { return p.HasValue ? p.Value : (UInt64)0; }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(SByte? p) { return p.HasValue ? checked((UInt64)p.Value) : (UInt64)0; }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Int16? p) { return p.HasValue ? checked((UInt64)p.Value) : (UInt64)0; }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Int32? p) { return p.HasValue ? checked((UInt64)p.Value) : (UInt64)0; }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Int64? p) { return p.HasValue ? checked((UInt64)p.Value) : (UInt64)0; }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Byte? p) { return p.HasValue ? checked((UInt64)p.Value) : (UInt64)0; }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(UInt16? p) { return p.HasValue ? checked((UInt64)p.Value) : (UInt64)0; }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(UInt32? p) { return p.HasValue ? checked((UInt64)p.Value) : (UInt64)0; }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Single? p) { return p.HasValue ? checked((UInt64)p.Value) : (UInt64)0; }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Double? p) { return p.HasValue ? checked((UInt64)p.Value) : (UInt64)0; }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Decimal? p) { return p.HasValue ? checked((UInt64)p.Value) : (UInt64)0; }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Char? p) { return p.HasValue ? checked((UInt64)p.Value) : (UInt64)0; }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(Boolean? p) { return (p.HasValue && p.Value) ? (UInt64)1 : (UInt64)0; }

        // SqlTypes.
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(SqlString p) { return p.IsNull ? (UInt64)0 : ToUInt64(p.Value); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(SqlByte p) { return p.IsNull ? (UInt64)0 : ToUInt64(p.Value); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(SqlInt16 p) { return p.IsNull ? (UInt64)0 : ToUInt64(p.Value); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(SqlInt32 p) { return p.IsNull ? (UInt64)0 : ToUInt64(p.Value); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(SqlInt64 p) { return p.IsNull ? (UInt64)0 : ToUInt64(p.Value); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(SqlSingle p) { return p.IsNull ? (UInt64)0 : ToUInt64(p.Value); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(SqlDouble p) { return p.IsNull ? (UInt64)0 : ToUInt64(p.Value); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(SqlDecimal p) { return p.IsNull ? (UInt64)0 : ToUInt64(p.Value); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(SqlMoney p) { return p.IsNull ? (UInt64)0 : ToUInt64(p.Value); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(SqlBoolean p) { return p.IsNull ? (UInt64)0 : ToUInt64(p.Value); }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64 that convert from parater value</returns>

        public static UInt64 ToUInt64(object p)
        {
            if (p == null) return UInt64.MinValue;

            // Scalar Types.
            //
            if (p is String) return ToUInt64((String)p);

            if (p is Boolean) return ToUInt64((Boolean)p);
            if (p is Char) return ToUInt64((Char)p);

            // Nullable Types.
            //
            if (p is UInt64?) return ToUInt64((UInt64?)p);

            if (p is Char?) return ToUInt64((Char?)p);
            if (p is Boolean?) return ToUInt64((Boolean?)p);

            // SqlTypes.
            //

            if (p is IConvertible) return ((IConvertible)p).ToUInt64(null);

            throw CreateInvalidCastException(p.GetType(), typeof(UInt64));
        }

        #endregion

        #region Char

        // Scalar Types.
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(String p) { return p == null ? (Char)0 : Char.Parse(p); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>

        public static Char ToChar(SByte p) { return checked((Char)p); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Int16 p) { return checked((Char)p); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Int32 p) { return checked((Char)p); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Int64 p) { return checked((Char)p); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Byte p) { return checked((Char)p); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>

        public static Char ToChar(UInt16 p) { return checked((Char)p); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>

        public static Char ToChar(UInt32 p) { return checked((Char)p); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>

        public static Char ToChar(UInt64 p) { return checked((Char)p); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Single p) { return checked((Char)p); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Double p) { return checked((Char)p); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Decimal p) { return checked((Char)p); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Boolean p) { return (Char)(p ? 1 : 0); }

        // Nullable Types.
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Char? p) { return p.HasValue ? p.Value : (Char)0; }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>

        public static Char ToChar(SByte? p) { return p.HasValue ? checked((Char)p.Value) : (Char)0; }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Int16? p) { return p.HasValue ? checked((Char)p.Value) : (Char)0; }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Int32? p) { return p.HasValue ? checked((Char)p.Value) : (Char)0; }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Int64? p) { return p.HasValue ? checked((Char)p.Value) : (Char)0; }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Byte? p) { return p.HasValue ? checked((Char)p.Value) : (Char)0; }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>

        public static Char ToChar(UInt16? p) { return p.HasValue ? checked((Char)p.Value) : (Char)0; }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>

        public static Char ToChar(UInt32? p) { return p.HasValue ? checked((Char)p.Value) : (Char)0; }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>

        public static Char ToChar(UInt64? p) { return p.HasValue ? checked((Char)p.Value) : (Char)0; }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Single? p) { return p.HasValue ? checked((Char)p.Value) : (Char)0; }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Double? p) { return p.HasValue ? checked((Char)p.Value) : (Char)0; }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Decimal? p) { return p.HasValue ? checked((Char)p.Value) : (Char)0; }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(Boolean? p) { return (p.HasValue && p.Value) ? (Char)1 : (Char)0; }

        // SqlTypes.
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(SqlString p) { return p.IsNull ? (Char)0 : ToChar(p.Value); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(SqlByte p) { return p.IsNull ? (Char)0 : ToChar(p.Value); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(SqlInt16 p) { return p.IsNull ? (Char)0 : ToChar(p.Value); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(SqlInt32 p) { return p.IsNull ? (Char)0 : ToChar(p.Value); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(SqlInt64 p) { return p.IsNull ? (Char)0 : ToChar(p.Value); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(SqlSingle p) { return p.IsNull ? (Char)0 : ToChar(p.Value); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(SqlDouble p) { return p.IsNull ? (Char)0 : ToChar(p.Value); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(SqlDecimal p) { return p.IsNull ? (Char)0 : ToChar(p.Value); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(SqlMoney p) { return p.IsNull ? (Char)0 : ToChar(p.Value); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(SqlBoolean p) { return p.IsNull ? (Char)0 : ToChar(p.Value); }
        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char that convert from parater value</returns>
        public static Char ToChar(object p)
        {
            if (p == null) return Char.MinValue;

            // Scalar Types.
            //
            if (p is String) return ToChar((String)p);

            if (p is Boolean) return ToChar((Boolean)p);

            // Nullable Types.
            //
            if (p is Char?) return ToChar((Char?)p);

            if (p is Boolean?) return ToChar((Boolean?)p);

            // SqlTypes.
            //

            if (p is IConvertible) return ((IConvertible)p).ToChar(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Char));
        }

        #endregion

        #region Single

        // Scalar Types.
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(String p) { return p == null ? 0.0f : Single.Parse(p); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>

        public static Single ToSingle(SByte p) { return checked((Single)p); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Int16 p) { return checked((Single)p); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Int32 p) { return checked((Single)p); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Int64 p) { return checked((Single)p); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Byte p) { return checked((Single)p); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>

        public static Single ToSingle(UInt16 p) { return checked((Single)p); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>

        public static Single ToSingle(UInt32 p) { return checked((Single)p); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>

        public static Single ToSingle(UInt64 p) { return checked((Single)p); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Double p) { return checked((Single)p); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Decimal p) { return checked((Single)p); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Boolean p) { return p ? 1.0f : 0.0f; }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Char p) { return checked((Single)p); }

        // Nullable Types.
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Single? p) { return p.HasValue ? p.Value : 0.0f; }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>

        public static Single ToSingle(SByte? p) { return p.HasValue ? checked((Single)p.Value) : 0.0f; }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Int16? p) { return p.HasValue ? checked((Single)p.Value) : 0.0f; }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Int32? p) { return p.HasValue ? checked((Single)p.Value) : 0.0f; }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Int64? p) { return p.HasValue ? checked((Single)p.Value) : 0.0f; }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Byte? p) { return p.HasValue ? checked((Single)p.Value) : 0.0f; }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>

        public static Single ToSingle(UInt16? p) { return p.HasValue ? checked((Single)p.Value) : 0.0f; }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>

        public static Single ToSingle(UInt32? p) { return p.HasValue ? checked((Single)p.Value) : 0.0f; }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>

        public static Single ToSingle(UInt64? p) { return p.HasValue ? checked((Single)p.Value) : 0.0f; }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Double? p) { return p.HasValue ? checked((Single)p.Value) : 0.0f; }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Decimal? p) { return p.HasValue ? checked((Single)p.Value) : 0.0f; }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Char? p) { return p.HasValue ? checked((Single)p.Value) : 0.0f; }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(Boolean? p) { return (p.HasValue && p.Value) ? 1.0f : 0.0f; }

        // SqlTypes.
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(SqlSingle p) { return p.IsNull ? 0.0f : p.Value; }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(SqlString p) { return p.IsNull ? 0.0f : ToSingle(p.Value); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(SqlByte p) { return p.IsNull ? 0.0f : ToSingle(p.Value); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(SqlInt16 p) { return p.IsNull ? 0.0f : ToSingle(p.Value); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(SqlInt32 p) { return p.IsNull ? 0.0f : ToSingle(p.Value); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(SqlInt64 p) { return p.IsNull ? 0.0f : ToSingle(p.Value); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(SqlDouble p) { return p.IsNull ? 0.0f : ToSingle(p.Value); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(SqlDecimal p) { return p.IsNull ? 0.0f : ToSingle(p.Value); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(SqlMoney p) { return p.IsNull ? 0.0f : ToSingle(p.Value); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(SqlBoolean p) { return p.IsNull ? 0.0f : ToSingle(p.Value); }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single that convert from parater value</returns>
        public static Single ToSingle(object p)
        {
            if (p == null) return 0.0f;

            // Scalar Types.
            //
            if (p is String) return ToSingle((String)p);

            if (p is Boolean) return ToSingle((Boolean)p);
            if (p is Char) return ToSingle((Char)p);

            // Nullable Types.
            //
            if (p is Single?) return ToSingle((Single?)p);

            if (p is Char?) return ToSingle((Char?)p);
            if (p is Boolean?) return ToSingle((Boolean?)p);

            // SqlTypes.
            //
            if (p is SqlSingle) return ToSingle((SqlSingle)p);

            if (p is IConvertible) return ((IConvertible)p).ToSingle(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Single));
        }

        #endregion

        #region Double

        // Scalar Types.
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(String p) { return p == null ? 0.0 : Double.Parse(p); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>

        public static Double ToDouble(SByte p) { return checked((Double)p); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Int16 p) { return checked((Double)p); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Int32 p) { return checked((Double)p); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Int64 p) { return checked((Double)p); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Byte p) { return checked((Double)p); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>

        public static Double ToDouble(UInt16 p) { return checked((Double)p); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>

        public static Double ToDouble(UInt32 p) { return checked((Double)p); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>

        public static Double ToDouble(UInt64 p) { return checked((Double)p); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Single p) { return checked((Double)p); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Decimal p) { return checked((Double)p); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Boolean p) { return p ? 1.0 : 0.0; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Char p) { return checked((Double)p); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(DateTime p) { return (p - DateTime.MinValue).TotalDays; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(TimeSpan p) { return p.TotalDays; }

        // Nullable Types.
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Double? p) { return p.HasValue ? p.Value : 0.0; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>

        public static Double ToDouble(SByte? p) { return p.HasValue ? checked((Double)p.Value) : 0.0; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Int16? p) { return p.HasValue ? checked((Double)p.Value) : 0.0; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Int32? p) { return p.HasValue ? checked((Double)p.Value) : 0.0; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Int64? p) { return p.HasValue ? checked((Double)p.Value) : 0.0; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Byte? p) { return p.HasValue ? checked((Double)p.Value) : 0.0; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>

        public static Double ToDouble(UInt16? p) { return p.HasValue ? checked((Double)p.Value) : 0.0; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>

        public static Double ToDouble(UInt32? p) { return p.HasValue ? checked((Double)p.Value) : 0.0; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>

        public static Double ToDouble(UInt64? p) { return p.HasValue ? checked((Double)p.Value) : 0.0; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Single? p) { return p.HasValue ? checked((Double)p.Value) : 0.0; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Decimal? p) { return p.HasValue ? checked((Double)p.Value) : 0.0; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Char? p) { return p.HasValue ? checked((Double)p.Value) : 0.0; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(Boolean? p) { return (p.HasValue && p.Value) ? 1.0 : 0.0; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(DateTime? p) { return p.HasValue ? (p.Value - DateTime.MinValue).TotalDays : 0.0; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(TimeSpan? p) { return p.HasValue ? p.Value.TotalDays : 0.0; }

        // SqlTypes.
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(SqlDouble p) { return p.IsNull ? 0.0 : p.Value; }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(SqlString p) { return p.IsNull ? 0.0 : ToDouble(p.Value); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(SqlByte p) { return p.IsNull ? 0.0 : ToDouble(p.Value); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(SqlInt16 p) { return p.IsNull ? 0.0 : ToDouble(p.Value); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(SqlInt32 p) { return p.IsNull ? 0.0 : ToDouble(p.Value); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(SqlInt64 p) { return p.IsNull ? 0.0 : ToDouble(p.Value); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(SqlSingle p) { return p.IsNull ? 0.0 : ToDouble(p.Value); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(SqlDecimal p) { return p.IsNull ? 0.0 : ToDouble(p.Value); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(SqlMoney p) { return p.IsNull ? 0.0 : ToDouble(p.Value); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(SqlBoolean p) { return p.IsNull ? 0.0 : ToDouble(p.Value); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(SqlDateTime p) { return p.IsNull ? 0.0 : ToDouble(p.Value); }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double that convert from parater value</returns>
        public static Double ToDouble(object p)
        {
            if (p == null) return 0.0;

            // Scalar Types.
            //
            if (p is String) return ToDouble((String)p);

            if (p is Boolean) return ToDouble((Boolean)p);
            if (p is Char) return ToDouble((Char)p);
            if (p is DateTime) return ToDouble((DateTime)p);
            if (p is TimeSpan) return ToDouble((TimeSpan)p);

            // Nullable Types.
            //
            if (p is Double?) return ToDouble((Double?)p);

            if (p is Char?) return ToDouble((Char?)p);
            if (p is Boolean?) return ToDouble((Boolean?)p);
            if (p is DateTime?) return ToDouble((DateTime?)p);
            if (p is TimeSpan?) return ToDouble((TimeSpan?)p);

            // SqlTypes.
            //
            if (p is SqlDouble) return ToDouble((SqlDouble)p);
            if (p is SqlDateTime) return ToDouble((SqlDateTime)p);

            if (p is IConvertible) return ((IConvertible)p).ToDouble(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Double));
        }

        #endregion

        #region Boolean

        // Scalar Types.
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(String p) { return p == null ? false : Boolean.Parse(p); }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>

        public static Boolean ToBoolean(SByte p) { return p != 0; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Int16 p) { return p != 0; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Int32 p) { return p != 0; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Int64 p) { return p != 0; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Byte p) { return p != 0; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>

        public static Boolean ToBoolean(UInt16 p) { return p != 0; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>

        public static Boolean ToBoolean(UInt32 p) { return p != 0; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>

        public static Boolean ToBoolean(UInt64 p) { return p != 0; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Single p) { return p != 0; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Double p) { return p != 0; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Decimal p) { return p != 0; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Char p)
        {
            switch (p)
            {
                case (Char)0: return false; // Allow int <=> Char <=> Boolean
                case '0': return false;
                case 'n': return false;
                case 'N': return false;
                case 'f': return false;
                case 'F': return false;

                case (Char)1: return true; // Allow int <=> Char <=> Boolean
                case '1': return true;
                case 'y': return true;
                case 'Y': return true;
                case 't': return true;
                case 'T': return true;
            }

            throw new InvalidCastException(string.Format(
                "Invalid cast from {0} to {1}", typeof(Char).FullName, typeof(Boolean).FullName));

        }

        // Nullable Types.
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Boolean? p) { return p.HasValue ? p.Value : false; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>

        public static Boolean ToBoolean(SByte? p) { return p.HasValue ? p.Value != 0 : false; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Int16? p) { return p.HasValue ? p.Value != 0 : false; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Int32? p) { return p.HasValue ? p.Value != 0 : false; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Int64? p) { return p.HasValue ? p.Value != 0 : false; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Byte? p) { return p.HasValue ? p.Value != 0 : false; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>

        public static Boolean ToBoolean(UInt16? p) { return p.HasValue ? p.Value != 0 : false; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>

        public static Boolean ToBoolean(UInt32? p) { return p.HasValue ? p.Value != 0 : false; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>

        public static Boolean ToBoolean(UInt64? p) { return p.HasValue ? p.Value != 0 : false; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Single? p) { return p.HasValue ? p.Value != 0 : false; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Double? p) { return p.HasValue ? p.Value != 0 : false; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Decimal? p) { return p.HasValue ? p.Value != 0 : false; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(Char? p) { return p.HasValue ? ToBoolean(p.Value) : false; }

        // SqlTypes.
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(SqlBoolean p) { return p.IsNull ? false : p.Value; }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(SqlString p) { return p.IsNull ? false : ToBoolean(p.Value); }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(SqlByte p) { return p.IsNull ? false : ToBoolean(p.Value); }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(SqlInt16 p) { return p.IsNull ? false : ToBoolean(p.Value); }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(SqlInt32 p) { return p.IsNull ? false : ToBoolean(p.Value); }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(SqlInt64 p) { return p.IsNull ? false : ToBoolean(p.Value); }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(SqlSingle p) { return p.IsNull ? false : ToBoolean(p.Value); }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(SqlDouble p) { return p.IsNull ? false : ToBoolean(p.Value); }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(SqlDecimal p) { return p.IsNull ? false : ToBoolean(p.Value); }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(SqlMoney p) { return p.IsNull ? false : ToBoolean(p.Value); }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean that convert from parater value</returns>
        public static Boolean ToBoolean(object p)
        {
            if (p == null) return false;

            // Scalar Types.
            //
            if (p is String) return ToBoolean((String)p);


            if (p is Char) return ToBoolean((Char)p);

            // Nullable Types.
            //
            if (p is Boolean?) return ToBoolean((Boolean?)p);


            if (p is Char?) return ToBoolean((Char?)p);

            // SqlTypes.
            //
            if (p is SqlBoolean) return ToBoolean((SqlBoolean)p);

            if (p is IConvertible) return ((IConvertible)p).ToBoolean(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Boolean));
        }

        #endregion

        #region Decimal

        // Scalar Types.
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(String p) { return p == null ? 0.0m : Decimal.Parse(p); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>

        public static Decimal ToDecimal(SByte p) { return checked((Decimal)p); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Int16 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Int32 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Int64 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Byte p) { return checked((Decimal)p); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>

        public static Decimal ToDecimal(UInt16 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>

        public static Decimal ToDecimal(UInt32 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>

        public static Decimal ToDecimal(UInt64 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Single p) { return checked((Decimal)p); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Double p) { return checked((Decimal)p); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Boolean p) { return p ? 1.0m : 0.0m; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Char p) { return checked((Decimal)p); }

        // Nullable Types.
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Decimal? p) { return p.HasValue ? p.Value : 0.0m; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>

        public static Decimal ToDecimal(SByte? p) { return p.HasValue ? checked((Decimal)p.Value) : 0.0m; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Int16? p) { return p.HasValue ? checked((Decimal)p.Value) : 0.0m; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Int32? p) { return p.HasValue ? checked((Decimal)p.Value) : 0.0m; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Int64? p) { return p.HasValue ? checked((Decimal)p.Value) : 0.0m; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Byte? p) { return p.HasValue ? checked((Decimal)p.Value) : 0.0m; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>

        public static Decimal ToDecimal(UInt16? p) { return p.HasValue ? checked((Decimal)p.Value) : 0.0m; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>

        public static Decimal ToDecimal(UInt32? p) { return p.HasValue ? checked((Decimal)p.Value) : 0.0m; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>

        public static Decimal ToDecimal(UInt64? p) { return p.HasValue ? checked((Decimal)p.Value) : 0.0m; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Single? p) { return p.HasValue ? checked((Decimal)p.Value) : 0.0m; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Double? p) { return p.HasValue ? checked((Decimal)p.Value) : 0.0m; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Char? p) { return p.HasValue ? checked((Decimal)p.Value) : 0.0m; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(Boolean? p) { return (p.HasValue && p.Value) ? 1.0m : 0.0m; }

        // SqlTypes.
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(SqlDecimal p) { return p.IsNull ? 0.0m : p.Value; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(SqlMoney p) { return p.IsNull ? 0.0m : p.Value; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(SqlString p) { return p.IsNull ? 0.0m : ToDecimal(p.Value); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(SqlByte p) { return p.IsNull ? 0.0m : ToDecimal(p.Value); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(SqlInt16 p) { return p.IsNull ? 0.0m : ToDecimal(p.Value); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(SqlInt32 p) { return p.IsNull ? 0.0m : ToDecimal(p.Value); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(SqlInt64 p) { return p.IsNull ? 0.0m : ToDecimal(p.Value); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(SqlSingle p) { return p.IsNull ? 0.0m : ToDecimal(p.Value); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(SqlDouble p) { return p.IsNull ? 0.0m : ToDecimal(p.Value); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(SqlBoolean p) { return p.IsNull ? 0.0m : ToDecimal(p.Value); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal that convert from parater value</returns>
        public static Decimal ToDecimal(object p)
        {
            if (p == null) return 0.0m;

            // Scalar Types.
            //
            if (p is String) return ToDecimal((String)p);

            if (p is Boolean) return ToDecimal((Boolean)p);
            if (p is Char) return ToDecimal((Char)p);

            // Nullable Types.
            //
            if (p is Decimal?) return ToDecimal((Decimal?)p);

            if (p is Char?) return ToDecimal((Char?)p);
            if (p is Boolean?) return ToDecimal((Boolean?)p);

            // SqlTypes.
            //
            if (p is SqlDecimal) return ToDecimal((SqlDecimal)p);
            if (p is SqlMoney) return ToDecimal((SqlMoney)p);

            if (p is IConvertible) return ((IConvertible)p).ToDecimal(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Decimal));
        }

        #endregion

        #region DateTime

        // Scalar Types.
        /// <summary>
        /// ToDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime that convert from parater value</returns>
        public static DateTime ToDateTime(String p) { return p == null ? DateTime.MinValue : DateTime.Parse(p); }
        /// <summary>
        /// ToDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime that convert from parater value</returns>
        public static DateTime ToDateTime(TimeSpan p) { return DateTime.MinValue + p; }
        /// <summary>
        /// ToDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime that convert from parater value</returns>
        public static DateTime ToDateTime(Int64 p) { return DateTime.MinValue + TimeSpan.FromTicks(p); }
        /// <summary>
        /// ToDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime that convert from parater value</returns>
        public static DateTime ToDateTime(Double p) { return DateTime.MinValue + TimeSpan.FromDays(p); }

        // Nullable Types.
        /// <summary>
        /// ToDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime that convert from parater value</returns>
        public static DateTime ToDateTime(DateTime? p) { return p.HasValue ? p.Value : DateTime.MinValue; }
        /// <summary>
        /// ToDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime that convert from parater value</returns>
        public static DateTime ToDateTime(TimeSpan? p) { return p.HasValue ? DateTime.MinValue + p.Value : DateTime.MinValue; }
        /// <summary>
        /// ToDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime that convert from parater value</returns>
        public static DateTime ToDateTime(Int64? p) { return p.HasValue ? DateTime.MinValue + TimeSpan.FromTicks(p.Value) : DateTime.MinValue; }
        /// <summary>
        /// ToDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime that convert from parater value</returns>
        public static DateTime ToDateTime(Double? p) { return p.HasValue ? DateTime.MinValue + TimeSpan.FromDays(p.Value) : DateTime.MinValue; }

        // SqlTypes.
        /// <summary>
        /// ToDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime that convert from parater value</returns>
        public static DateTime ToDateTime(SqlDateTime p) { return p.IsNull ? DateTime.MinValue : p.Value; }
        /// <summary>
        /// ToDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime that convert from parater value</returns>
        public static DateTime ToDateTime(SqlString p) { return p.IsNull ? DateTime.MinValue : ToDateTime(p.Value); }
        /// <summary>
        /// ToDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime that convert from parater value</returns>
        public static DateTime ToDateTime(SqlInt64 p) { return p.IsNull ? DateTime.MinValue : DateTime.MinValue + TimeSpan.FromTicks(p.Value); }
        /// <summary>
        /// ToDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime that convert from parater value</returns>
        public static DateTime ToDateTime(SqlDouble p) { return p.IsNull ? DateTime.MinValue : DateTime.MinValue + TimeSpan.FromDays(p.Value); }
        /// <summary>
        /// ToDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime that convert from parater value</returns>
        public static DateTime ToDateTime(object p)
        {
            if (p == null) return DateTime.MinValue;

            // Scalar Types.
            //
            if (p is String) return ToDateTime((String)p);
            if (p is TimeSpan) return ToDateTime((TimeSpan)p);
            if (p is Int64) return ToDateTime((Int64)p);
            if (p is Double) return ToDateTime((Double)p);

            // Nullable Types.
            //
            if (p is DateTime?) return ToDateTime((DateTime?)p);
            if (p is TimeSpan?) return ToDateTime((TimeSpan?)p);
            if (p is Int64?) return ToDateTime((Int64?)p);
            if (p is Double?) return ToDateTime((Double?)p);

            // SqlTypes.
            //
            if (p is SqlDateTime) return ToDateTime((SqlDateTime)p);
            if (p is SqlString) return ToDateTime((SqlString)p);
            if (p is SqlInt64) return ToDateTime((SqlInt64)p);
            if (p is SqlDouble) return ToDateTime((SqlDouble)p);

            if (p is IConvertible) return ((IConvertible)p).ToDateTime(null);

            throw CreateInvalidCastException(p.GetType(), typeof(DateTime));
        }

        #endregion

        #region TimeSpan

        // Scalar Types.
        /// <summary>
        /// ToTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan that convert from parater value</returns>
        public static TimeSpan ToTimeSpan(String p) { return p == null ? TimeSpan.MinValue : TimeSpan.Parse(p); }
        /// <summary>
        /// ToTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan that convert from parater value</returns>
        public static TimeSpan ToTimeSpan(DateTime p) { return p - DateTime.MinValue; }
        /// <summary>
        /// ToTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan that convert from parater value</returns>
        public static TimeSpan ToTimeSpan(Int64 p) { return TimeSpan.FromTicks(p); }
        /// <summary>
        /// ToTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan that convert from parater value</returns>
        public static TimeSpan ToTimeSpan(Double p) { return TimeSpan.FromDays(p); }

        // Nullable Types.
        /// <summary>
        /// ToTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan that convert from parater value</returns>
        public static TimeSpan ToTimeSpan(TimeSpan? p) { return p.HasValue ? p.Value : TimeSpan.MinValue; }
        /// <summary>
        /// ToTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan that convert from parater value</returns>
        public static TimeSpan ToTimeSpan(DateTime? p) { return p.HasValue ? p.Value - DateTime.MinValue : TimeSpan.MinValue; }
        /// <summary>
        /// ToTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan that convert from parater value</returns>
        public static TimeSpan ToTimeSpan(Int64? p) { return p.HasValue ? TimeSpan.FromTicks(p.Value) : TimeSpan.MinValue; }
        /// <summary>
        /// ToTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan that convert from parater value</returns>
        public static TimeSpan ToTimeSpan(Double? p) { return p.HasValue ? TimeSpan.FromDays(p.Value) : TimeSpan.MinValue; }

        // SqlTypes.
        /// <summary>
        /// ToTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan that convert from parater value</returns>
        public static TimeSpan ToTimeSpan(SqlString p) { return p.IsNull ? TimeSpan.MinValue : TimeSpan.Parse(p.Value); }
        /// <summary>
        /// ToTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan that convert from parater value</returns>
        public static TimeSpan ToTimeSpan(SqlDateTime p) { return p.IsNull ? TimeSpan.MinValue : p.Value - DateTime.MinValue; }
        /// <summary>
        /// ToTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan that convert from parater value</returns>
        public static TimeSpan ToTimeSpan(SqlInt64 p) { return p.IsNull ? TimeSpan.MinValue : TimeSpan.FromTicks(p.Value); }
        /// <summary>
        /// ToTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan that convert from parater value</returns>
        public static TimeSpan ToTimeSpan(SqlDouble p) { return p.IsNull ? TimeSpan.MinValue : TimeSpan.FromDays(p.Value); }
        /// <summary>
        /// ToTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan that convert from parater value</returns>
        public static TimeSpan ToTimeSpan(object p)
        {
            if (p == null) return TimeSpan.MinValue;

            // Scalar Types.
            //
            if (p is String) return ToTimeSpan((String)p);
            if (p is DateTime) return ToTimeSpan((DateTime)p);
            if (p is Int64) return ToTimeSpan((Int64)p);
            if (p is Double) return ToTimeSpan((Double)p);

            // Nullable Types.
            //
            if (p is TimeSpan?) return ToTimeSpan((TimeSpan?)p);
            if (p is DateTime?) return ToTimeSpan((DateTime?)p);
            if (p is Int64?) return ToTimeSpan((Int64?)p);
            if (p is Double?) return ToTimeSpan((Double?)p);

            // SqlTypes.
            //
            if (p is SqlString) return ToTimeSpan((SqlString)p);
            if (p is SqlDateTime) return ToTimeSpan((SqlDateTime)p);
            if (p is SqlInt64) return ToTimeSpan((SqlInt64)p);
            if (p is SqlDouble) return ToTimeSpan((SqlDouble)p);

            throw CreateInvalidCastException(p.GetType(), typeof(TimeSpan));
        }

        #endregion

        #region Guid

        // Scalar Types.
        /// <summary>
        /// ToGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid that convert from parater value</returns>
        public static Guid ToGuid(String p) { return p == null ? Guid.Empty : new Guid(p); }

        // Nullable Types.
        /// <summary>
        /// ToGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid that convert from parater value</returns>
        public static Guid ToGuid(Guid? p) { return p.HasValue ? p.Value : Guid.Empty; }

        // SqlTypes.
        /// <summary>
        /// ToGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid that convert from parater value</returns>
        public static Guid ToGuid(SqlGuid p) { return p.IsNull ? Guid.Empty : p.Value; }
        /// <summary>
        /// ToGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid that convert from parater value</returns>
        public static Guid ToGuid(SqlString p) { return p.IsNull ? Guid.Empty : new Guid(p.Value); }
        /// <summary>
        /// ToGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid that convert from parater value</returns>
        public static Guid ToGuid(SqlBinary p) { return p.IsNull ? Guid.Empty : p.ToSqlGuid().Value; }

        // Other Types.
        /// <summary>
        /// ToGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid that convert from parater value</returns>
        public static Guid ToGuid(Byte[] p) { return p == null ? Guid.Empty : new Guid(p); }
        /// <summary>
        /// ToGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid that convert from parater value</returns>
        public static Guid ToGuid(Type p) { return p == null ? Guid.Empty : p.GUID; }
        /// <summary>
        /// ToGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid that convert from parater value</returns>
        public static Guid ToGuid(object p)
        {
            if (p == null) return Guid.Empty;

            // Scalar Types.
            //
            if (p is String) return ToGuid((String)p);

            // Nullable Types.
            //
            if (p is Guid?) return ToGuid((Guid?)p);

            // SqlTypes.
            //
            if (p is SqlGuid) return ToGuid((SqlGuid)p);
            if (p is SqlString) return ToGuid((SqlString)p);
            if (p is SqlBinary) return ToGuid((SqlBinary)p);

            // Other Types.
            //
            if (p is Byte[]) return ToGuid((Byte[])p);
            if (p is Type) return ToGuid((Type)p);

            throw CreateInvalidCastException(p.GetType(), typeof(Guid));
        }

        #endregion

        #endregion

        #region Nullable Types

        #region SByte?

        // Scalar Types.
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(SByte p) { return p; }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(String p) { return p == null ? null : (SByte?)SByte.Parse(p); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Int16 p) { return checked((SByte?)p); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Int32 p) { return checked((SByte?)p); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Int64 p) { return checked((SByte?)p); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Byte p) { return checked((SByte?)p); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(UInt16 p) { return checked((SByte?)p); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(UInt32 p) { return checked((SByte?)p); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(UInt64 p) { return checked((SByte?)p); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Single p) { return checked((SByte?)p); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Double p) { return checked((SByte?)p); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Decimal p) { return checked((SByte?)p); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Char p) { return checked((SByte?)p); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Boolean p) { return (SByte?)(p ? 1 : 0); }

        // Nullable Types.
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Int16? p) { return p.HasValue ? checked((SByte?)p.Value) : null; }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Int32? p) { return p.HasValue ? checked((SByte?)p.Value) : null; }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Int64? p) { return p.HasValue ? checked((SByte?)p.Value) : null; }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Byte? p) { return p.HasValue ? checked((SByte?)p.Value) : null; }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(UInt16? p) { return p.HasValue ? checked((SByte?)p.Value) : null; }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(UInt32? p) { return p.HasValue ? checked((SByte?)p.Value) : null; }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(UInt64? p) { return p.HasValue ? checked((SByte?)p.Value) : null; }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Single? p) { return p.HasValue ? checked((SByte?)p.Value) : null; }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Double? p) { return p.HasValue ? checked((SByte?)p.Value) : null; }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Decimal? p) { return p.HasValue ? checked((SByte?)p.Value) : null; }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Char? p) { return p.HasValue ? checked((SByte?)p.Value) : null; }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(Boolean? p) { return p.HasValue ? (SByte?)(p.Value ? 1 : 0) : null; }

        // SqlTypes.
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(SqlString p) { return p.IsNull ? null : ToNullableSByte(p.Value); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(SqlByte p) { return p.IsNull ? null : ToNullableSByte(p.Value); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(SqlInt16 p) { return p.IsNull ? null : ToNullableSByte(p.Value); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(SqlInt32 p) { return p.IsNull ? null : ToNullableSByte(p.Value); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(SqlInt64 p) { return p.IsNull ? null : ToNullableSByte(p.Value); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(SqlSingle p) { return p.IsNull ? null : ToNullableSByte(p.Value); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(SqlDouble p) { return p.IsNull ? null : ToNullableSByte(p.Value); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(SqlDecimal p) { return p.IsNull ? null : ToNullableSByte(p.Value); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(SqlMoney p) { return p.IsNull ? null : ToNullableSByte(p.Value); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(SqlBoolean p) { return p.IsNull ? null : ToNullableSByte(p.Value); }
        /// <summary>
        /// ToNullableSByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SByte? that convert from parater value</returns>

        public static SByte? ToNullableSByte(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is SByte) return ToNullableSByte((SByte)p);
            if (p is String) return ToNullableSByte((String)p);

            if (p is Char) return ToNullableSByte((Char)p);
            if (p is Boolean) return ToNullableSByte((Boolean)p);

            // Nullable Types.
            //
            if (p is Char?) return ToNullableSByte((Char?)p);
            if (p is Boolean?) return ToNullableSByte((Boolean?)p);

            // SqlTypes.
            //

            if (p is IConvertible) return ((IConvertible)p).ToSByte(null);

            throw CreateInvalidCastException(p.GetType(), typeof(SByte?));
        }

        #endregion

        #region Int16?

        // Scalar Types.
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Int16 p) { return p; }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(String p) { return p == null ? null : (Int16?)Int16.Parse(p); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>

        public static Int16? ToNullableInt16(SByte p) { return checked((Int16?)p); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Int32 p) { return checked((Int16?)p); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Int64 p) { return checked((Int16?)p); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Byte p) { return checked((Int16?)p); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>

        public static Int16? ToNullableInt16(UInt16 p) { return checked((Int16?)p); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>

        public static Int16? ToNullableInt16(UInt32 p) { return checked((Int16?)p); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>

        public static Int16? ToNullableInt16(UInt64 p) { return checked((Int16?)p); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Single p) { return checked((Int16?)p); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Double p) { return checked((Int16?)p); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Decimal p) { return checked((Int16?)p); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Char p) { return checked((Int16?)p); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Boolean p) { return (Int16?)(p ? 1 : 0); }
        // Nullable Types.
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>

        public static Int16? ToNullableInt16(SByte? p) { return p.HasValue ? checked((Int16?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Int32? p) { return p.HasValue ? checked((Int16?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Int64? p) { return p.HasValue ? checked((Int16?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Byte? p) { return p.HasValue ? checked((Int16?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>

        public static Int16? ToNullableInt16(UInt16? p) { return p.HasValue ? checked((Int16?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>

        public static Int16? ToNullableInt16(UInt32? p) { return p.HasValue ? checked((Int16?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>

        public static Int16? ToNullableInt16(UInt64? p) { return p.HasValue ? checked((Int16?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Single? p) { return p.HasValue ? checked((Int16?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Double? p) { return p.HasValue ? checked((Int16?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Decimal? p) { return p.HasValue ? checked((Int16?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Char? p) { return p.HasValue ? checked((Int16?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(Boolean? p) { return p.HasValue ? (Int16?)(p.Value ? 1 : 0) : null; }
        // SqlTypes.
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(SqlInt16 p) { return p.IsNull ? null : (Int16?)p.Value; }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(SqlString p) { return p.IsNull ? null : ToNullableInt16(p.Value); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(SqlByte p) { return p.IsNull ? null : ToNullableInt16(p.Value); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(SqlInt32 p) { return p.IsNull ? null : ToNullableInt16(p.Value); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(SqlInt64 p) { return p.IsNull ? null : ToNullableInt16(p.Value); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(SqlSingle p) { return p.IsNull ? null : ToNullableInt16(p.Value); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(SqlDouble p) { return p.IsNull ? null : ToNullableInt16(p.Value); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(SqlDecimal p) { return p.IsNull ? null : ToNullableInt16(p.Value); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(SqlMoney p) { return p.IsNull ? null : ToNullableInt16(p.Value); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(SqlBoolean p) { return p.IsNull ? null : ToNullableInt16(p.Value); }
        /// <summary>
        /// ToNullableInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int16? that convert from parater value</returns>
        public static Int16? ToNullableInt16(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is Int16) return ToNullableInt16((Int16)p);
            if (p is String) return ToNullableInt16((String)p);

            if (p is Char) return ToNullableInt16((Char)p);
            if (p is Boolean) return ToNullableInt16((Boolean)p);

            // Nullable Types.
            //
            if (p is Char?) return ToNullableInt16((Char?)p);
            if (p is Boolean?) return ToNullableInt16((Boolean?)p);

            // SqlTypes.
            //
            if (p is SqlInt16) return ToNullableInt16((SqlInt16)p);

            if (p is IConvertible) return ((IConvertible)p).ToInt16(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Int16?));
        }

        #endregion

        #region Int32?

        // Scalar Types.
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Int32 p) { return p; }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(String p) { return p == null ? null : (Int32?)Int32.Parse(p); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>

        public static Int32? ToNullableInt32(SByte p) { return checked((Int32?)p); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Int16 p) { return checked((Int32?)p); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Int64 p) { return checked((Int32?)p); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Byte p) { return checked((Int32?)p); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>

        public static Int32? ToNullableInt32(UInt16 p) { return checked((Int32?)p); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>

        public static Int32? ToNullableInt32(UInt32 p) { return checked((Int32?)p); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>

        public static Int32? ToNullableInt32(UInt64 p) { return checked((Int32?)p); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Single p) { return checked((Int32?)p); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Double p) { return checked((Int32?)p); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Decimal p) { return checked((Int32?)p); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Char p) { return checked((Int32?)p); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Boolean p) { return p ? 1 : 0; }
        // Nullable Types.
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>

        public static Int32? ToNullableInt32(SByte? p) { return p.HasValue ? checked((Int32?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Int16? p) { return p.HasValue ? checked((Int32?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Int64? p) { return p.HasValue ? checked((Int32?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Byte? p) { return p.HasValue ? checked((Int32?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>

        public static Int32? ToNullableInt32(UInt16? p) { return p.HasValue ? checked((Int32?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>

        public static Int32? ToNullableInt32(UInt32? p) { return p.HasValue ? checked((Int32?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>

        public static Int32? ToNullableInt32(UInt64? p) { return p.HasValue ? checked((Int32?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Single? p) { return p.HasValue ? checked((Int32?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Double? p) { return p.HasValue ? checked((Int32?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Decimal? p) { return p.HasValue ? checked((Int32?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Char? p) { return p.HasValue ? checked((Int32?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(Boolean? p) { return p.HasValue ? (Int32?)(p.Value ? 1 : 0) : null; }
        // SqlTypes.
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(SqlInt32 p) { return p.IsNull ? null : (Int32?)p.Value; }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(SqlString p) { return p.IsNull ? null : ToNullableInt32(p.Value); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(SqlByte p) { return p.IsNull ? null : ToNullableInt32(p.Value); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(SqlInt16 p) { return p.IsNull ? null : ToNullableInt32(p.Value); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(SqlInt64 p) { return p.IsNull ? null : ToNullableInt32(p.Value); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(SqlSingle p) { return p.IsNull ? null : ToNullableInt32(p.Value); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(SqlDouble p) { return p.IsNull ? null : ToNullableInt32(p.Value); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(SqlDecimal p) { return p.IsNull ? null : ToNullableInt32(p.Value); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(SqlMoney p) { return p.IsNull ? null : ToNullableInt32(p.Value); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(SqlBoolean p) { return p.IsNull ? null : ToNullableInt32(p.Value); }
        /// <summary>
        /// ToNullableInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int32? that convert from parater value</returns>
        public static Int32? ToNullableInt32(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is Int32) return ToNullableInt32((Int32)p);
            if (p is String) return ToNullableInt32((String)p);

            if (p is Char) return ToNullableInt32((Char)p);
            if (p is Boolean) return ToNullableInt32((Boolean)p);

            // Nullable Types.
            //
            if (p is Char?) return ToNullableInt32((Char?)p);
            if (p is Boolean?) return ToNullableInt32((Boolean?)p);

            // SqlTypes.
            //
            if (p is SqlInt32) return ToNullableInt32((SqlInt32)p);

            if (p is IConvertible) return ((IConvertible)p).ToInt32(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Int32?));
        }

        #endregion

        #region Int64?

        // Scalar Types.
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Int64 p) { return p; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(String p) { return p == null ? null : (Int64?)Int64.Parse(p); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>

        public static Int64? ToNullableInt64(SByte p) { return checked((Int64?)p); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Int16 p) { return checked((Int64?)p); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Int32 p) { return checked((Int64?)p); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Byte p) { return checked((Int64?)p); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>

        public static Int64? ToNullableInt64(UInt16 p) { return checked((Int64?)p); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>

        public static Int64? ToNullableInt64(UInt32 p) { return checked((Int64?)p); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>

        public static Int64? ToNullableInt64(UInt64 p) { return checked((Int64?)p); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Single p) { return checked((Int64?)p); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Double p) { return checked((Int64?)p); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Decimal p) { return checked((Int64?)p); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Char p) { return checked((Int64?)p); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Boolean p) { return p ? 1 : 0; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(DateTime p) { return (p - DateTime.MinValue).Ticks; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(TimeSpan p) { return p.Ticks; }

        // Nullable Types.
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>

        public static Int64? ToNullableInt64(SByte? p) { return p.HasValue ? checked((Int64?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Int16? p) { return p.HasValue ? checked((Int64?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Int32? p) { return p.HasValue ? checked((Int64?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Byte? p) { return p.HasValue ? checked((Int64?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>

        public static Int64? ToNullableInt64(UInt16? p) { return p.HasValue ? checked((Int64?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>

        public static Int64? ToNullableInt64(UInt32? p) { return p.HasValue ? checked((Int64?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>

        public static Int64? ToNullableInt64(UInt64? p) { return p.HasValue ? checked((Int64?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Single? p) { return p.HasValue ? checked((Int64?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Double? p) { return p.HasValue ? checked((Int64?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Decimal? p) { return p.HasValue ? checked((Int64?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Char? p) { return p.HasValue ? checked((Int64?)p.Value) : null; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(Boolean? p) { return p.HasValue ? (Int64?)(p.Value ? 1 : 0) : null; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(DateTime? p) { return p.HasValue ? (Int64?)(p.Value - DateTime.MinValue).Ticks : null; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(TimeSpan? p) { return p.HasValue ? (Int64?)p.Value.Ticks : null; }

        // SqlTypes.
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(SqlInt64 p) { return p.IsNull ? null : (Int64?)p.Value; }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(SqlString p) { return p.IsNull ? null : ToNullableInt64(p.Value); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(SqlByte p) { return p.IsNull ? null : ToNullableInt64(p.Value); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(SqlInt16 p) { return p.IsNull ? null : ToNullableInt64(p.Value); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(SqlInt32 p) { return p.IsNull ? null : ToNullableInt64(p.Value); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(SqlSingle p) { return p.IsNull ? null : ToNullableInt64(p.Value); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(SqlDouble p) { return p.IsNull ? null : ToNullableInt64(p.Value); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(SqlDecimal p) { return p.IsNull ? null : ToNullableInt64(p.Value); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(SqlMoney p) { return p.IsNull ? null : ToNullableInt64(p.Value); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(SqlBoolean p) { return p.IsNull ? null : ToNullableInt64(p.Value); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(SqlDateTime p) { return p.IsNull ? null : ToNullableInt64(p.Value); }
        /// <summary>
        /// ToNullableInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Int64? that convert from parater value</returns>
        public static Int64? ToNullableInt64(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is Int64) return ToNullableInt64((Int64)p);
            if (p is String) return ToNullableInt64((String)p);

            if (p is Char) return ToNullableInt64((Char)p);
            if (p is Boolean) return ToNullableInt64((Boolean)p);
            if (p is DateTime) return ToNullableInt64((DateTime)p);
            if (p is TimeSpan) return ToNullableInt64((TimeSpan)p);
            // Nullable Types.
            //
            if (p is Char?) return ToNullableInt64((Char?)p);
            if (p is Boolean?) return ToNullableInt64((Boolean?)p);
            if (p is DateTime?) return ToNullableInt64((DateTime?)p);
            if (p is TimeSpan?) return ToNullableInt64((TimeSpan?)p);
            // SqlTypes.
            //
            if (p is SqlInt64) return ToNullableInt64((SqlInt64)p);
            if (p is SqlDateTime) return ToNullableInt64((SqlDateTime)p);

            if (p is IConvertible) return ((IConvertible)p).ToInt64(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Int64?));
        }

        #endregion

        #region Byte?

        // Scalar Types.
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Byte p) { return p; }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(String p) { return p == null ? null : (Byte?)Byte.Parse(p); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>

        public static Byte? ToNullableByte(SByte p) { return checked((Byte?)p); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Int16 p) { return checked((Byte?)p); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Int32 p) { return checked((Byte?)p); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Int64 p) { return checked((Byte?)p); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>

        public static Byte? ToNullableByte(UInt16 p) { return checked((Byte?)p); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>

        public static Byte? ToNullableByte(UInt32 p) { return checked((Byte?)p); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>

        public static Byte? ToNullableByte(UInt64 p) { return checked((Byte?)p); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Single p) { return checked((Byte?)p); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Double p) { return checked((Byte?)p); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Decimal p) { return checked((Byte?)p); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Char p) { return checked((Byte?)p); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Boolean p) { return (Byte?)(p ? 1 : 0); }
        // Nullable Types.
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>

        public static Byte? ToNullableByte(SByte? p) { return p.HasValue ? checked((Byte?)p.Value) : null; }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Int16? p) { return p.HasValue ? checked((Byte?)p.Value) : null; }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Int32? p) { return p.HasValue ? checked((Byte?)p.Value) : null; }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Int64? p) { return p.HasValue ? checked((Byte?)p.Value) : null; }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>

        public static Byte? ToNullableByte(UInt16? p) { return p.HasValue ? checked((Byte?)p.Value) : null; }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>

        public static Byte? ToNullableByte(UInt32? p) { return p.HasValue ? checked((Byte?)p.Value) : null; }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>

        public static Byte? ToNullableByte(UInt64? p) { return p.HasValue ? checked((Byte?)p.Value) : null; }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Single? p) { return p.HasValue ? checked((Byte?)p.Value) : null; }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Double? p) { return p.HasValue ? checked((Byte?)p.Value) : null; }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Decimal? p) { return p.HasValue ? checked((Byte?)p.Value) : null; }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Char? p) { return p.HasValue ? checked((Byte?)p.Value) : null; }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(Boolean? p) { return p.HasValue ? (Byte?)(p.Value ? 1 : 0) : null; }
        // SqlTypes.
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(SqlByte p) { return p.IsNull ? null : (Byte?)p.Value; }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(SqlString p) { return p.IsNull ? null : ToNullableByte(p.Value); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(SqlInt16 p) { return p.IsNull ? null : ToNullableByte(p.Value); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(SqlInt32 p) { return p.IsNull ? null : ToNullableByte(p.Value); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(SqlInt64 p) { return p.IsNull ? null : ToNullableByte(p.Value); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(SqlSingle p) { return p.IsNull ? null : ToNullableByte(p.Value); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(SqlDouble p) { return p.IsNull ? null : ToNullableByte(p.Value); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(SqlDecimal p) { return p.IsNull ? null : ToNullableByte(p.Value); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(SqlMoney p) { return p.IsNull ? null : ToNullableByte(p.Value); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(SqlBoolean p) { return p.IsNull ? null : ToNullableByte(p.Value); }
        /// <summary>
        /// ToNullableByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte? that convert from parater value</returns>
        public static Byte? ToNullableByte(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is Byte) return ToNullableByte((Byte)p);
            if (p is String) return ToNullableByte((String)p);

            if (p is Char) return ToNullableByte((Char)p);
            if (p is Boolean) return ToNullableByte((Boolean)p);

            // Nullable Types.
            //
            if (p is Char?) return ToNullableByte((Char?)p);
            if (p is Boolean?) return ToNullableByte((Boolean?)p);

            // SqlTypes.
            //
            if (p is SqlByte) return ToNullableByte((SqlByte)p);

            if (p is IConvertible) return ((IConvertible)p).ToByte(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Byte?));
        }

        #endregion

        #region UInt16?

        // Scalar Types.
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(UInt16 p) { return p; }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(String p) { return p == null ? null : (UInt16?)UInt16.Parse(p); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(SByte p) { return checked((UInt16?)p); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Int16 p) { return checked((UInt16?)p); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Int32 p) { return checked((UInt16?)p); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Int64 p) { return checked((UInt16?)p); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Byte p) { return checked((UInt16?)p); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(UInt32 p) { return checked((UInt16?)p); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(UInt64 p) { return checked((UInt16?)p); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Single p) { return checked((UInt16?)p); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Double p) { return checked((UInt16?)p); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Decimal p) { return checked((UInt16?)p); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Char p) { return checked((UInt16?)p); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Boolean p) { return (UInt16?)(p ? 1 : 0); }

        // Nullable Types.
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(SByte? p) { return p.HasValue ? checked((UInt16?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Int16? p) { return p.HasValue ? checked((UInt16?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Int32? p) { return p.HasValue ? checked((UInt16?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Int64? p) { return p.HasValue ? checked((UInt16?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Byte? p) { return p.HasValue ? checked((UInt16?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(UInt32? p) { return p.HasValue ? checked((UInt16?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(UInt64? p) { return p.HasValue ? checked((UInt16?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Single? p) { return p.HasValue ? checked((UInt16?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Double? p) { return p.HasValue ? checked((UInt16?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Decimal? p) { return p.HasValue ? checked((UInt16?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Char? p) { return p.HasValue ? checked((UInt16?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(Boolean? p) { return p.HasValue ? (UInt16?)(p.Value ? 1 : 0) : null; }

        // SqlTypes.
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(SqlString p) { return p.IsNull ? null : ToNullableUInt16(p.Value); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(SqlByte p) { return p.IsNull ? null : ToNullableUInt16(p.Value); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(SqlInt16 p) { return p.IsNull ? null : ToNullableUInt16(p.Value); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(SqlInt32 p) { return p.IsNull ? null : ToNullableUInt16(p.Value); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(SqlInt64 p) { return p.IsNull ? null : ToNullableUInt16(p.Value); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(SqlSingle p) { return p.IsNull ? null : ToNullableUInt16(p.Value); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(SqlDouble p) { return p.IsNull ? null : ToNullableUInt16(p.Value); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(SqlDecimal p) { return p.IsNull ? null : ToNullableUInt16(p.Value); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(SqlMoney p) { return p.IsNull ? null : ToNullableUInt16(p.Value); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(SqlBoolean p) { return p.IsNull ? null : ToNullableUInt16(p.Value); }
        /// <summary>
        /// ToNullableUInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt16? that convert from parater value</returns>

        public static UInt16? ToNullableUInt16(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is UInt16) return ToNullableUInt16((UInt16)p);
            if (p is String) return ToNullableUInt16((String)p);

            if (p is Char) return ToNullableUInt16((Char)p);
            if (p is Boolean) return ToNullableUInt16((Boolean)p);

            // Nullable Types.
            //
            if (p is Char?) return ToNullableUInt16((Char?)p);
            if (p is Boolean?) return ToNullableUInt16((Boolean?)p);

            // SqlTypes.
            //

            if (p is IConvertible) return ((IConvertible)p).ToUInt16(null);

            throw CreateInvalidCastException(p.GetType(), typeof(UInt16?));
        }

        #endregion

        #region UInt32?

        // Scalar Types.
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(UInt32 p) { return p; }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(String p) { return p == null ? null : (UInt32?)UInt32.Parse(p); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(SByte p) { return checked((UInt32?)p); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Int16 p) { return checked((UInt32?)p); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Int32 p) { return checked((UInt32?)p); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Int64 p) { return checked((UInt32?)p); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Byte p) { return checked((UInt32?)p); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(UInt16 p) { return checked((UInt32?)p); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(UInt64 p) { return checked((UInt32?)p); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Single p) { return checked((UInt32?)p); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Double p) { return checked((UInt32?)p); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Decimal p) { return checked((UInt32?)p); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Char p) { return checked((UInt32?)p); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Boolean p) { return (UInt32?)(p ? 1 : 0); }

        // Nullable Types.
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(SByte? p) { return p.HasValue ? checked((UInt32?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Int16? p) { return p.HasValue ? checked((UInt32?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Int32? p) { return p.HasValue ? checked((UInt32?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Int64? p) { return p.HasValue ? checked((UInt32?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Byte? p) { return p.HasValue ? checked((UInt32?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(UInt16? p) { return p.HasValue ? checked((UInt32?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(UInt64? p) { return p.HasValue ? checked((UInt32?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Single? p) { return p.HasValue ? checked((UInt32?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Double? p) { return p.HasValue ? checked((UInt32?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Decimal? p) { return p.HasValue ? checked((UInt32?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Char? p) { return p.HasValue ? checked((UInt32?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(Boolean? p) { return p.HasValue ? (UInt32?)(p.Value ? 1 : 0) : null; }

        // SqlTypes.
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(SqlString p) { return p.IsNull ? null : ToNullableUInt32(p.Value); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(SqlByte p) { return p.IsNull ? null : ToNullableUInt32(p.Value); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(SqlInt16 p) { return p.IsNull ? null : ToNullableUInt32(p.Value); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(SqlInt32 p) { return p.IsNull ? null : ToNullableUInt32(p.Value); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(SqlInt64 p) { return p.IsNull ? null : ToNullableUInt32(p.Value); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(SqlSingle p) { return p.IsNull ? null : ToNullableUInt32(p.Value); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(SqlDouble p) { return p.IsNull ? null : ToNullableUInt32(p.Value); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(SqlDecimal p) { return p.IsNull ? null : ToNullableUInt32(p.Value); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(SqlMoney p) { return p.IsNull ? null : ToNullableUInt32(p.Value); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(SqlBoolean p) { return p.IsNull ? null : ToNullableUInt32(p.Value); }
        /// <summary>
        /// ToNullableUInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt32? that convert from parater value</returns>

        public static UInt32? ToNullableUInt32(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is UInt32) return ToNullableUInt32((UInt32)p);
            if (p is String) return ToNullableUInt32((String)p);

            if (p is Char) return ToNullableUInt32((Char)p);
            if (p is Boolean) return ToNullableUInt32((Boolean)p);

            // Nullable Types.
            //
            if (p is Char?) return ToNullableUInt32((Char?)p);
            if (p is Boolean?) return ToNullableUInt32((Boolean?)p);

            // SqlTypes.
            //

            if (p is IConvertible) return ((IConvertible)p).ToUInt32(null);

            throw CreateInvalidCastException(p.GetType(), typeof(UInt32?));
        }

        #endregion

        #region UInt64?

        // Scalar Types.
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(UInt64 p) { return p; }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(String p) { return p == null ? null : (UInt64?)UInt64.Parse(p); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(SByte p) { return checked((UInt64?)p); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Int16 p) { return checked((UInt64?)p); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Int32 p) { return checked((UInt64?)p); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Int64 p) { return checked((UInt64?)p); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Byte p) { return checked((UInt64?)p); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(UInt16 p) { return checked((UInt64?)p); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(UInt32 p) { return checked((UInt64?)p); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Single p) { return checked((UInt64?)p); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Double p) { return checked((UInt64?)p); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Decimal p) { return checked((UInt64?)p); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Char p) { return checked((UInt64?)p); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Boolean p) { return (UInt64?)(p ? 1 : 0); }

        // Nullable Types.
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(SByte? p) { return p.HasValue ? checked((UInt64?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Int16? p) { return p.HasValue ? checked((UInt64?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Int32? p) { return p.HasValue ? checked((UInt64?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Int64? p) { return p.HasValue ? checked((UInt64?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Byte? p) { return p.HasValue ? checked((UInt64?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(UInt16? p) { return p.HasValue ? checked((UInt64?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(UInt32? p) { return p.HasValue ? checked((UInt64?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Single? p) { return p.HasValue ? checked((UInt64?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Double? p) { return p.HasValue ? checked((UInt64?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Decimal? p) { return p.HasValue ? checked((UInt64?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Char? p) { return p.HasValue ? checked((UInt64?)p.Value) : null; }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(Boolean? p) { return p.HasValue ? (UInt64?)(p.Value ? 1 : 0) : null; }

        // SqlTypes.
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(SqlString p) { return p.IsNull ? null : ToNullableUInt64(p.Value); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(SqlByte p) { return p.IsNull ? null : ToNullableUInt64(p.Value); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(SqlInt16 p) { return p.IsNull ? null : ToNullableUInt64(p.Value); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(SqlInt32 p) { return p.IsNull ? null : ToNullableUInt64(p.Value); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(SqlInt64 p) { return p.IsNull ? null : ToNullableUInt64(p.Value); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(SqlSingle p) { return p.IsNull ? null : ToNullableUInt64(p.Value); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(SqlDouble p) { return p.IsNull ? null : ToNullableUInt64(p.Value); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(SqlDecimal p) { return p.IsNull ? null : ToNullableUInt64(p.Value); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(SqlMoney p) { return p.IsNull ? null : ToNullableUInt64(p.Value); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(SqlBoolean p) { return p.IsNull ? null : ToNullableUInt64(p.Value); }
        /// <summary>
        /// ToNullableUInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>UInt64? that convert from parater value</returns>

        public static UInt64? ToNullableUInt64(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is UInt64) return ToNullableUInt64((UInt64)p);
            if (p is String) return ToNullableUInt64((String)p);

            if (p is Char) return ToNullableUInt64((Char)p);
            if (p is Boolean) return ToNullableUInt64((Boolean)p);

            // Nullable Types.
            //
            if (p is Char?) return ToNullableUInt64((Char?)p);
            if (p is Boolean?) return ToNullableUInt64((Boolean?)p);

            // SqlTypes.
            //

            if (p is IConvertible) return ((IConvertible)p).ToUInt64(null);

            throw CreateInvalidCastException(p.GetType(), typeof(UInt64?));
        }

        #endregion

        #region Char?

        // Scalar Types.
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Char p) { return p; }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(String p) { return p == null ? null : (Char?)Char.Parse(p); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>

        public static Char? ToNullableChar(SByte p) { return checked((Char?)p); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Int16 p) { return checked((Char?)p); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Int32 p) { return checked((Char?)p); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Int64 p) { return checked((Char?)p); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Byte p) { return checked((Char?)p); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>

        public static Char? ToNullableChar(UInt16 p) { return checked((Char?)p); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>

        public static Char? ToNullableChar(UInt32 p) { return checked((Char?)p); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>

        public static Char? ToNullableChar(UInt64 p) { return checked((Char?)p); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Single p) { return checked((Char?)p); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Double p) { return checked((Char?)p); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Decimal p) { return checked((Char?)p); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Boolean p) { return (Char?)(p ? 1 : 0); }

        // Nullable Types.
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>

        public static Char? ToNullableChar(SByte? p) { return p.HasValue ? checked((Char?)p.Value) : null; }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Int16? p) { return p.HasValue ? checked((Char?)p.Value) : null; }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Int32? p) { return p.HasValue ? checked((Char?)p.Value) : null; }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Int64? p) { return p.HasValue ? checked((Char?)p.Value) : null; }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Byte? p) { return p.HasValue ? checked((Char?)p.Value) : null; }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>

        public static Char? ToNullableChar(UInt16? p) { return p.HasValue ? checked((Char?)p.Value) : null; }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>

        public static Char? ToNullableChar(UInt32? p) { return p.HasValue ? checked((Char?)p.Value) : null; }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>

        public static Char? ToNullableChar(UInt64? p) { return p.HasValue ? checked((Char?)p.Value) : null; }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Single? p) { return p.HasValue ? checked((Char?)p.Value) : null; }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Double? p) { return p.HasValue ? checked((Char?)p.Value) : null; }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Decimal? p) { return p.HasValue ? checked((Char?)p.Value) : null; }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(Boolean? p) { return p.HasValue ? (Char?)(p.Value ? 1 : 0) : null; }
        // SqlTypes.
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(SqlString p) { return p.IsNull ? null : ToNullableChar(p.Value); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(SqlByte p) { return p.IsNull ? null : ToNullableChar(p.Value); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(SqlInt16 p) { return p.IsNull ? null : ToNullableChar(p.Value); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(SqlInt32 p) { return p.IsNull ? null : ToNullableChar(p.Value); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(SqlInt64 p) { return p.IsNull ? null : ToNullableChar(p.Value); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(SqlSingle p) { return p.IsNull ? null : ToNullableChar(p.Value); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(SqlDouble p) { return p.IsNull ? null : ToNullableChar(p.Value); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(SqlDecimal p) { return p.IsNull ? null : ToNullableChar(p.Value); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(SqlMoney p) { return p.IsNull ? null : ToNullableChar(p.Value); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(SqlBoolean p) { return p.IsNull ? null : ToNullableChar(p.Value); }
        /// <summary>
        /// ToNullableChar
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char? that convert from parater value</returns>
        public static Char? ToNullableChar(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is Char) return ToNullableChar((Char)p);
            if (p is String) return ToNullableChar((String)p);

            if (p is Boolean) return ToNullableChar((Boolean)p);

            // Nullable Types.
            //
            if (p is Boolean?) return ToNullableChar((Boolean?)p);

            // SqlTypes.
            //

            if (p is IConvertible) return ((IConvertible)p).ToChar(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Char?));
        }

        #endregion

        #region Single?

        // Scalar Types.
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Single p) { return p; }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(String p) { return p == null ? null : (Single?)Single.Parse(p); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>

        public static Single? ToNullableSingle(SByte p) { return checked((Single?)p); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Int16 p) { return checked((Single?)p); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Int32 p) { return checked((Single?)p); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Int64 p) { return checked((Single?)p); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Byte p) { return checked((Single?)p); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>

        public static Single? ToNullableSingle(UInt16 p) { return checked((Single?)p); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>

        public static Single? ToNullableSingle(UInt32 p) { return checked((Single?)p); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>

        public static Single? ToNullableSingle(UInt64 p) { return checked((Single?)p); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Double p) { return checked((Single?)p); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Decimal p) { return checked((Single?)p); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Char p) { return checked((Single?)p); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Boolean p) { return p ? 1.0f : 0.0f; }

        // Nullable Types.
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>

        public static Single? ToNullableSingle(SByte? p) { return p.HasValue ? checked((Single?)p.Value) : null; }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Int16? p) { return p.HasValue ? checked((Single?)p.Value) : null; }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Int32? p) { return p.HasValue ? checked((Single?)p.Value) : null; }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Int64? p) { return p.HasValue ? checked((Single?)p.Value) : null; }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Byte? p) { return p.HasValue ? checked((Single?)p.Value) : null; }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>

        public static Single? ToNullableSingle(UInt16? p) { return p.HasValue ? checked((Single?)p.Value) : null; }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>

        public static Single? ToNullableSingle(UInt32? p) { return p.HasValue ? checked((Single?)p.Value) : null; }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>

        public static Single? ToNullableSingle(UInt64? p) { return p.HasValue ? checked((Single?)p.Value) : null; }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Double? p) { return p.HasValue ? checked((Single?)p.Value) : null; }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Decimal? p) { return p.HasValue ? checked((Single?)p.Value) : null; }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Char? p) { return p.HasValue ? checked((Single?)p.Value) : null; }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(Boolean? p) { return p.HasValue ? (Single?)(p.Value ? 1.0f : 0.0f) : null; }
        // SqlTypes.
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(SqlSingle p) { return p.IsNull ? null : (Single?)p.Value; }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(SqlString p) { return p.IsNull ? null : ToNullableSingle(p.Value); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(SqlByte p) { return p.IsNull ? null : ToNullableSingle(p.Value); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(SqlInt16 p) { return p.IsNull ? null : ToNullableSingle(p.Value); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(SqlInt32 p) { return p.IsNull ? null : ToNullableSingle(p.Value); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(SqlInt64 p) { return p.IsNull ? null : ToNullableSingle(p.Value); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(SqlDouble p) { return p.IsNull ? null : ToNullableSingle(p.Value); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(SqlDecimal p) { return p.IsNull ? null : ToNullableSingle(p.Value); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(SqlMoney p) { return p.IsNull ? null : ToNullableSingle(p.Value); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(SqlBoolean p) { return p.IsNull ? null : ToNullableSingle(p.Value); }
        /// <summary>
        /// ToNullableSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Single? that convert from parater value</returns>
        public static Single? ToNullableSingle(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is Single) return ToNullableSingle((Single)p);
            if (p is String) return ToNullableSingle((String)p);

            if (p is Char) return ToNullableSingle((Char)p);
            if (p is Boolean) return ToNullableSingle((Boolean)p);

            // Nullable Types.
            //
            if (p is Char?) return ToNullableSingle((Char?)p);
            if (p is Boolean?) return ToNullableSingle((Boolean?)p);

            // SqlTypes.
            //
            if (p is SqlSingle) return ToNullableSingle((SqlSingle)p);

            if (p is IConvertible) return ((IConvertible)p).ToSingle(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Single?));
        }

        #endregion

        #region Double?

        // Scalar Types.
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Double p) { return p; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(String p) { return p == null ? null : (Double?)Double.Parse(p); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>

        public static Double? ToNullableDouble(SByte p) { return checked((Double?)p); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Int16 p) { return checked((Double?)p); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Int32 p) { return checked((Double?)p); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Int64 p) { return checked((Double?)p); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Byte p) { return checked((Double?)p); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>

        public static Double? ToNullableDouble(UInt16 p) { return checked((Double?)p); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>

        public static Double? ToNullableDouble(UInt32 p) { return checked((Double?)p); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>

        public static Double? ToNullableDouble(UInt64 p) { return checked((Double?)p); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Single p) { return checked((Double?)p); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Decimal p) { return checked((Double?)p); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Char p) { return checked((Double?)p); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Boolean p) { return p ? 1.0 : 0.0; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(DateTime p) { return (p - DateTime.MinValue).TotalDays; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(TimeSpan p) { return p.TotalDays; }
        // Nullable Types.
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>

        public static Double? ToNullableDouble(SByte? p) { return p.HasValue ? checked((Double?)p.Value) : null; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Int16? p) { return p.HasValue ? checked((Double?)p.Value) : null; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Int32? p) { return p.HasValue ? checked((Double?)p.Value) : null; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Int64? p) { return p.HasValue ? checked((Double?)p.Value) : null; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Byte? p) { return p.HasValue ? checked((Double?)p.Value) : null; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>

        public static Double? ToNullableDouble(UInt16? p) { return p.HasValue ? checked((Double?)p.Value) : null; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>

        public static Double? ToNullableDouble(UInt32? p) { return p.HasValue ? checked((Double?)p.Value) : null; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>

        public static Double? ToNullableDouble(UInt64? p) { return p.HasValue ? checked((Double?)p.Value) : null; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Single? p) { return p.HasValue ? checked((Double?)p.Value) : null; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Decimal? p) { return p.HasValue ? checked((Double?)p.Value) : null; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Char? p) { return p.HasValue ? checked((Double?)p.Value) : null; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(Boolean? p) { return p.HasValue ? (Double?)(p.Value ? 1.0 : 0.0) : null; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(DateTime? p) { return p.HasValue ? (Double?)(p.Value - DateTime.MinValue).TotalDays : null; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(TimeSpan? p) { return p.HasValue ? (Double?)p.Value.TotalDays : null; }
        // SqlTypes.
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(SqlDouble p) { return p.IsNull ? null : (Double?)p.Value; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(SqlString p) { return p.IsNull ? null : ToNullableDouble(p.Value); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(SqlByte p) { return p.IsNull ? null : ToNullableDouble(p.Value); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(SqlInt16 p) { return p.IsNull ? null : ToNullableDouble(p.Value); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(SqlInt32 p) { return p.IsNull ? null : ToNullableDouble(p.Value); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(SqlInt64 p) { return p.IsNull ? null : ToNullableDouble(p.Value); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(SqlSingle p) { return p.IsNull ? null : ToNullableDouble(p.Value); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(SqlDecimal p) { return p.IsNull ? null : ToNullableDouble(p.Value); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(SqlMoney p) { return p.IsNull ? null : ToNullableDouble(p.Value); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(SqlBoolean p) { return p.IsNull ? null : ToNullableDouble(p.Value); }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(SqlDateTime p) { return p.IsNull ? null : (Double?)(p.Value - DateTime.MinValue).TotalDays; }
        /// <summary>
        /// ToNullableDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Double? that convert from parater value</returns>
        public static Double? ToNullableDouble(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is Double) return ToNullableDouble((Double)p);
            if (p is String) return ToNullableDouble((String)p);

            if (p is Char) return ToNullableDouble((Char)p);
            if (p is Boolean) return ToNullableDouble((Boolean)p);
            if (p is DateTime) return ToNullableDouble((DateTime)p);
            if (p is TimeSpan) return ToNullableDouble((TimeSpan)p);

            // Nullable Types.
            //
            if (p is Char?) return ToNullableDouble((Char?)p);
            if (p is Boolean?) return ToNullableDouble((Boolean?)p);
            if (p is DateTime?) return ToNullableDouble((DateTime?)p);
            if (p is TimeSpan?) return ToNullableDouble((TimeSpan?)p);

            // SqlTypes.
            //
            if (p is SqlDouble) return ToNullableDouble((SqlDouble)p);
            if (p is SqlDateTime) return ToNullableDouble((SqlDateTime)p);

            if (p is IConvertible) return ((IConvertible)p).ToDouble(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Double?));
        }

        #endregion

        #region Boolean?

        // Scalar Types.
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Boolean p) { return p; }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(String p) { return p == null ? null : (Boolean?)Boolean.Parse(p); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>

        public static Boolean? ToNullableBoolean(SByte p) { return ToBoolean(p); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Int16 p) { return ToBoolean(p); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Int32 p) { return ToBoolean(p); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Int64 p) { return ToBoolean(p); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Byte p) { return ToBoolean(p); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>

        public static Boolean? ToNullableBoolean(UInt16 p) { return ToBoolean(p); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>

        public static Boolean? ToNullableBoolean(UInt32 p) { return ToBoolean(p); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>

        public static Boolean? ToNullableBoolean(UInt64 p) { return ToBoolean(p); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Single p) { return ToBoolean(p); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Double p) { return ToBoolean(p); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Decimal p) { return ToBoolean(p); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Char p) { return ToBoolean(p); }

        // Nullable Types.
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>

        public static Boolean? ToNullableBoolean(SByte? p) { return p.HasValue ? (Boolean?)ToBoolean(p.Value) : null; }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Int16? p) { return p.HasValue ? (Boolean?)ToBoolean(p.Value) : null; }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Int32? p) { return p.HasValue ? (Boolean?)ToBoolean(p.Value) : null; }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Int64? p) { return p.HasValue ? (Boolean?)ToBoolean(p.Value) : null; }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Byte? p) { return p.HasValue ? (Boolean?)ToBoolean(p.Value) : null; }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>

        public static Boolean? ToNullableBoolean(UInt16? p) { return p.HasValue ? (Boolean?)ToBoolean(p.Value) : null; }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>

        public static Boolean? ToNullableBoolean(UInt32? p) { return p.HasValue ? (Boolean?)ToBoolean(p.Value) : null; }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>

        public static Boolean? ToNullableBoolean(UInt64? p) { return p.HasValue ? (Boolean?)ToBoolean(p.Value) : null; }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Single? p) { return p.HasValue ? (Boolean?)ToBoolean(p.Value) : null; }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Double? p) { return p.HasValue ? (Boolean?)ToBoolean(p.Value) : null; }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Decimal? p) { return p.HasValue ? (Boolean?)ToBoolean(p.Value) : null; }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(Char? p) { return p.HasValue ? (Boolean?)ToBoolean(p.Value) : null; }
        // SqlTypes.
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(SqlBoolean p) { return p.IsNull ? null : (Boolean?)p.Value; }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(SqlString p) { return p.IsNull ? null : (Boolean?)ToBoolean(p.Value); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(SqlByte p) { return p.IsNull ? null : (Boolean?)ToBoolean(p.Value); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(SqlInt16 p) { return p.IsNull ? null : (Boolean?)ToBoolean(p.Value); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(SqlInt32 p) { return p.IsNull ? null : (Boolean?)ToBoolean(p.Value); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(SqlInt64 p) { return p.IsNull ? null : (Boolean?)ToBoolean(p.Value); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(SqlSingle p) { return p.IsNull ? null : (Boolean?)ToBoolean(p.Value); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(SqlDouble p) { return p.IsNull ? null : (Boolean?)ToBoolean(p.Value); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(SqlDecimal p) { return p.IsNull ? null : (Boolean?)ToBoolean(p.Value); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(SqlMoney p) { return p.IsNull ? null : (Boolean?)ToBoolean(p.Value); }
        /// <summary>
        /// ToNullableBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Boolean? that convert from parater value</returns>
        public static Boolean? ToNullableBoolean(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is Boolean) return ToNullableBoolean((Boolean)p);
            if (p is String) return ToNullableBoolean((String)p);

            if (p is Char) return ToNullableBoolean((Char)p);

            // Nullable Types.
            //
            if (p is Char?) return ToNullableBoolean((Char?)p);

            // SqlTypes.
            //
            if (p is SqlBoolean) return ToNullableBoolean((SqlBoolean)p);

            if (p is IConvertible) return ((IConvertible)p).ToBoolean(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Boolean?));
        }

        #endregion

        #region Decimal?

        // Scalar Types.
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Decimal p) { return p; }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(String p) { return p == null ? null : (Decimal?)Decimal.Parse(p); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>

        public static Decimal? ToNullableDecimal(SByte p) { return checked((Decimal?)p); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Int16 p) { return checked((Decimal?)p); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Int32 p) { return checked((Decimal?)p); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Int64 p) { return checked((Decimal?)p); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Byte p) { return checked((Decimal?)p); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>

        public static Decimal? ToNullableDecimal(UInt16 p) { return checked((Decimal?)p); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>

        public static Decimal? ToNullableDecimal(UInt32 p) { return checked((Decimal?)p); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>

        public static Decimal? ToNullableDecimal(UInt64 p) { return checked((Decimal?)p); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Single p) { return checked((Decimal?)p); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Double p) { return checked((Decimal?)p); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Char p) { return checked((Decimal?)p); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Boolean p) { return p ? 1.0m : 0.0m; }
        // Nullable Types.
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>

        public static Decimal? ToNullableDecimal(SByte? p) { return p.HasValue ? checked((Decimal?)p.Value) : null; }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Int16? p) { return p.HasValue ? checked((Decimal?)p.Value) : null; }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Int32? p) { return p.HasValue ? checked((Decimal?)p.Value) : null; }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Int64? p) { return p.HasValue ? checked((Decimal?)p.Value) : null; }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Byte? p) { return p.HasValue ? checked((Decimal?)p.Value) : null; }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>

        public static Decimal? ToNullableDecimal(UInt16? p) { return p.HasValue ? checked((Decimal?)p.Value) : null; }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>

        public static Decimal? ToNullableDecimal(UInt32? p) { return p.HasValue ? checked((Decimal?)p.Value) : null; }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>

        public static Decimal? ToNullableDecimal(UInt64? p) { return p.HasValue ? checked((Decimal?)p.Value) : null; }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Single? p) { return p.HasValue ? checked((Decimal?)p.Value) : null; }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Double? p) { return p.HasValue ? checked((Decimal?)p.Value) : null; }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Char? p) { return p.HasValue ? checked((Decimal?)p.Value) : null; }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(Boolean? p) { return p.HasValue ? (Decimal?)(p.Value ? 1.0m : 0.0m) : null; }
        // SqlTypes.
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(SqlDecimal p) { return p.IsNull ? null : (Decimal?)p.Value; }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(SqlMoney p) { return p.IsNull ? null : (Decimal?)p.Value; }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(SqlString p) { return p.IsNull ? null : ToNullableDecimal(p.Value); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(SqlByte p) { return p.IsNull ? null : ToNullableDecimal(p.Value); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(SqlInt16 p) { return p.IsNull ? null : ToNullableDecimal(p.Value); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(SqlInt32 p) { return p.IsNull ? null : ToNullableDecimal(p.Value); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(SqlInt64 p) { return p.IsNull ? null : ToNullableDecimal(p.Value); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(SqlSingle p) { return p.IsNull ? null : ToNullableDecimal(p.Value); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(SqlDouble p) { return p.IsNull ? null : ToNullableDecimal(p.Value); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(SqlBoolean p) { return p.IsNull ? null : ToNullableDecimal(p.Value); }
        /// <summary>
        /// ToNullableDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Decimal? that convert from parater value</returns>
        public static Decimal? ToNullableDecimal(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is Decimal) return ToNullableDecimal((Decimal)p);
            if (p is String) return ToNullableDecimal((String)p);

            if (p is Char) return ToNullableDecimal((Char)p);
            if (p is Boolean) return ToNullableDecimal((Boolean)p);

            // Nullable Types.
            //
            if (p is Char?) return ToNullableDecimal((Char?)p);
            if (p is Boolean?) return ToNullableDecimal((Boolean?)p);

            // SqlTypes.
            //
            if (p is SqlDecimal) return ToNullableDecimal((SqlDecimal)p);
            if (p is SqlMoney) return ToNullableDecimal((SqlMoney)p);

            if (p is IConvertible) return ((IConvertible)p).ToDecimal(null);

            throw CreateInvalidCastException(p.GetType(), typeof(Decimal?));
        }

        #endregion

        #region DateTime?

        // Scalar Types.
        /// <summary>
        /// ToNullableDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime? that convert from parater value</returns>
        public static DateTime? ToNullableDateTime(DateTime p) { return p; }
        /// <summary>
        /// ToNullableDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime? that convert from parater value</returns>
        public static DateTime? ToNullableDateTime(String p) { return p == null ? null : (DateTime?)DateTime.Parse(p); }
        /// <summary>
        /// ToNullableDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime? that convert from parater value</returns>
        public static DateTime? ToNullableDateTime(TimeSpan p) { return DateTime.MinValue + p; }
        /// <summary>
        /// ToNullableDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime? that convert from parater value</returns>
        public static DateTime? ToNullableDateTime(Int64 p) { return DateTime.MinValue + TimeSpan.FromTicks(p); }
        /// <summary>
        /// ToNullableDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime? that convert from parater value</returns>
        public static DateTime? ToNullableDateTime(Double p) { return DateTime.MinValue + TimeSpan.FromDays(p); }
        // Nullable Types.
        /// <summary>
        /// ToNullableDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime? that convert from parater value</returns>
        public static DateTime? ToNullableDateTime(TimeSpan? p) { return p.HasValue ? DateTime.MinValue + p.Value : (DateTime?)null; }
        /// <summary>
        /// ToNullableDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime? that convert from parater value</returns>
        public static DateTime? ToNullableDateTime(Int64? p) { return p.HasValue ? DateTime.MinValue + TimeSpan.FromTicks(p.Value) : (DateTime?)null; }
        /// <summary>
        /// ToNullableDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime? that convert from parater value</returns>
        public static DateTime? ToNullableDateTime(Double? p) { return p.HasValue ? DateTime.MinValue + TimeSpan.FromDays(p.Value) : (DateTime?)null; }

        // SqlTypes.
        /// <summary>
        /// ToNullableDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime? that convert from parater value</returns>
        public static DateTime? ToNullableDateTime(SqlDateTime p) { return p.IsNull ? (DateTime?)null : p.Value; }
        /// <summary>
        /// ToNullableDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime? that convert from parater value</returns>
        public static DateTime? ToNullableDateTime(SqlString p) { return p.IsNull ? (DateTime?)null : ToDateTime(p.Value); }
        /// <summary>
        /// ToNullableDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime? that convert from parater value</returns>
        public static DateTime? ToNullableDateTime(SqlInt64 p) { return p.IsNull ? (DateTime?)null : DateTime.MinValue + TimeSpan.FromTicks(p.Value); }
        /// <summary>
        /// ToNullableDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime? that convert from parater value</returns>
        public static DateTime? ToNullableDateTime(SqlDouble p) { return p.IsNull ? (DateTime?)null : DateTime.MinValue + TimeSpan.FromDays(p.Value); }
        /// <summary>
        /// ToNullableDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>DateTime? that convert from parater value</returns>
        public static DateTime? ToNullableDateTime(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is DateTime) return ToNullableDateTime((DateTime)p);
            if (p is String) return ToNullableDateTime((String)p);

            if (p is TimeSpan) return ToNullableDateTime((TimeSpan)p);
            if (p is Int64) return ToNullableDateTime((Int64)p);
            if (p is Double) return ToNullableDateTime((Double)p);

            // Nullable Types.
            //
            if (p is TimeSpan?) return ToNullableDateTime((TimeSpan?)p);
            if (p is Int64?) return ToNullableDateTime((Int64?)p);
            if (p is Double?) return ToNullableDateTime((Double?)p);

            // SqlTypes.
            //
            if (p is SqlDateTime) return ToNullableDateTime((SqlDateTime)p);
            if (p is SqlString) return ToNullableDateTime((SqlString)p);
            if (p is SqlInt64) return ToNullableDateTime((SqlInt64)p);
            if (p is SqlDouble) return ToNullableDateTime((SqlDouble)p);

            if (p is IConvertible) return ((IConvertible)p).ToDateTime(null);

            throw CreateInvalidCastException(p.GetType(), typeof(DateTime?));
        }

        #endregion

        #region TimeSpan?

        // Scalar Types.
        /// <summary>
        /// ToNullableTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan? that convert from parater value</returns>
        public static TimeSpan? ToNullableTimeSpan(TimeSpan p) { return p; }
        /// <summary>
        /// ToNullableTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan? that convert from parater value</returns>
        public static TimeSpan? ToNullableTimeSpan(String p) { return p == null ? null : (TimeSpan?)TimeSpan.Parse(p); }
        /// <summary>
        /// ToNullableTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan? that convert from parater value</returns>
        public static TimeSpan? ToNullableTimeSpan(DateTime p) { return p - DateTime.MinValue; }
        /// <summary>
        /// ToNullableTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan? that convert from parater value</returns>
        public static TimeSpan? ToNullableTimeSpan(Int64 p) { return TimeSpan.FromTicks(p); }
        /// <summary>
        /// ToNullableTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan? that convert from parater value</returns>
        public static TimeSpan? ToNullableTimeSpan(Double p) { return TimeSpan.FromDays(p); }
        // Nullable Types.
        /// <summary>
        /// ToNullableTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan? that convert from parater value</returns>
        public static TimeSpan? ToNullableTimeSpan(DateTime? p) { return p.HasValue ? p.Value - DateTime.MinValue : (TimeSpan?)null; }
        /// <summary>
        /// ToNullableTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan? that convert from parater value</returns>
        public static TimeSpan? ToNullableTimeSpan(Int64? p) { return p.HasValue ? TimeSpan.FromTicks(p.Value) : (TimeSpan?)null; }
        /// <summary>
        /// ToNullableTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan? that convert from parater value</returns>
        public static TimeSpan? ToNullableTimeSpan(Double? p) { return p.HasValue ? TimeSpan.FromDays(p.Value) : (TimeSpan?)null; }
        // SqlTypes.
        /// <summary>
        /// ToNullableTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan? that convert from parater value</returns>
        public static TimeSpan? ToNullableTimeSpan(SqlString p) { return p.IsNull ? (TimeSpan?)null : TimeSpan.Parse(p.Value); }
        /// <summary>
        /// ToNullableTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan? that convert from parater value</returns>
        public static TimeSpan? ToNullableTimeSpan(SqlDateTime p) { return p.IsNull ? (TimeSpan?)null : p.Value - DateTime.MinValue; }
        /// <summary>
        /// ToNullableTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan? that convert from parater value</returns>
        public static TimeSpan? ToNullableTimeSpan(SqlInt64 p) { return p.IsNull ? (TimeSpan?)null : TimeSpan.FromTicks(p.Value); }
        /// <summary>
        /// ToNullableTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan? that convert from parater value</returns>
        public static TimeSpan? ToNullableTimeSpan(SqlDouble p) { return p.IsNull ? (TimeSpan?)null : TimeSpan.FromDays(p.Value); }
        /// <summary>
        /// ToNullableTimeSpan
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>TimeSpan? that convert from parater value</returns>
        public static TimeSpan? ToNullableTimeSpan(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is TimeSpan) return ToNullableTimeSpan((TimeSpan)p);
            if (p is String) return ToNullableTimeSpan((String)p);
            if (p is DateTime) return ToNullableTimeSpan((DateTime)p);
            if (p is Int64) return ToNullableTimeSpan((Int64)p);
            if (p is Double) return ToNullableTimeSpan((Double)p);

            // Nullable Types.
            //
            if (p is DateTime?) return ToNullableTimeSpan((DateTime?)p);
            if (p is Int64?) return ToNullableTimeSpan((Int64?)p);
            if (p is Double?) return ToNullableTimeSpan((Double?)p);

            // SqlTypes.
            //
            if (p is SqlString) return ToNullableTimeSpan((SqlString)p);
            if (p is SqlDateTime) return ToNullableTimeSpan((SqlDateTime)p);
            if (p is SqlInt64) return ToNullableTimeSpan((SqlInt64)p);
            if (p is SqlDouble) return ToNullableTimeSpan((SqlDouble)p);

            throw CreateInvalidCastException(p.GetType(), typeof(TimeSpan?));
        }

        #endregion

        #region Guid?

        // Scalar Types.
        /// <summary>
        /// ToNullableGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid? that convert from parater value</returns>
        public static Guid? ToNullableGuid(Guid p) { return p; }
        /// <summary>
        /// ToNullableGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid? that convert from parater value</returns>
        public static Guid? ToNullableGuid(String p) { return p == null ? null : (Guid?)new Guid(p); }
        // SqlTypes.
        /// <summary>
        /// ToNullableGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid? that convert from parater value</returns>
        public static Guid? ToNullableGuid(SqlGuid p) { return p.IsNull ? null : (Guid?)p.Value; }
        /// <summary>
        /// ToNullableGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid? that convert from parater value</returns>
        public static Guid? ToNullableGuid(SqlString p) { return p.IsNull ? null : (Guid?)new Guid(p.Value); }
        /// <summary>
        /// ToNullableGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid? that convert from parater value</returns>
        public static Guid? ToNullableGuid(SqlBinary p) { return p.IsNull ? null : (Guid?)p.ToSqlGuid().Value; }
        // Other Types.
        /// <summary>
        /// ToNullableGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid? that convert from parater value</returns>
        public static Guid? ToNullableGuid(Type p) { return p == null ? null : (Guid?)p.GUID; }
        /// <summary>
        /// ToNullableGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid? that convert from parater value</returns>
        public static Guid? ToNullableGuid(Byte[] p) { return p == null ? null : (Guid?)new Guid(p); }
        /// <summary>
        /// ToNullableGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Guid? that convert from parater value</returns>
        public static Guid? ToNullableGuid(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is Guid) return ToNullableGuid((Guid)p);
            if (p is String) return ToNullableGuid((String)p);

            // SqlTypes.
            //
            if (p is SqlGuid) return ToNullableGuid((SqlGuid)p);
            if (p is SqlString) return ToNullableGuid((SqlString)p);
            if (p is SqlBinary) return ToNullableGuid((SqlBinary)p);

            // Other Types.
            //
            if (p is Type) return ToNullableGuid((Type)p);
            if (p is Byte[]) return ToNullableGuid((Byte[])p);

            throw CreateInvalidCastException(p.GetType(), typeof(Guid?));
        }

        #endregion

        #endregion

        #region SqlTypes

        #region SqlString

        // Scalar Types.
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(String p) { return p == null ? SqlString.Null : p; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>

        public static SqlString ToSqlString(SByte p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Int16 p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Int32 p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Int64 p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Byte p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>

        public static SqlString ToSqlString(UInt16 p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>

        public static SqlString ToSqlString(UInt32 p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>

        public static SqlString ToSqlString(UInt64 p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Single p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Double p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Boolean p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Decimal p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Char p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(TimeSpan p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(DateTime p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Guid p) { return p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Char[] p) { return new String(p); }

        // Nullable Types.
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>

        public static SqlString ToSqlString(SByte? p) { return p.HasValue ? p.ToString() : SqlString.Null; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Int16? p) { return p.HasValue ? p.ToString() : SqlString.Null; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Int32? p) { return p.HasValue ? p.ToString() : SqlString.Null; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Int64? p) { return p.HasValue ? p.ToString() : SqlString.Null; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Byte? p) { return p.HasValue ? p.ToString() : SqlString.Null; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>

        public static SqlString ToSqlString(UInt16? p) { return p.HasValue ? p.ToString() : SqlString.Null; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>

        public static SqlString ToSqlString(UInt32? p) { return p.HasValue ? p.ToString() : SqlString.Null; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>

        public static SqlString ToSqlString(UInt64? p) { return p.HasValue ? p.ToString() : SqlString.Null; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Single? p) { return p.HasValue ? p.ToString() : SqlString.Null; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Double? p) { return p.HasValue ? p.ToString() : SqlString.Null; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Boolean? p) { return p.HasValue ? p.ToString() : SqlString.Null; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Decimal? p) { return p.HasValue ? p.ToString() : SqlString.Null; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Char? p) { return p.HasValue ? p.ToString() : SqlString.Null; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(TimeSpan? p) { return p.HasValue ? p.ToString() : SqlString.Null; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(DateTime? p) { return p.HasValue ? p.ToString() : SqlString.Null; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Guid? p) { return p.HasValue ? p.ToString() : SqlString.Null; }

        // SqlTypes.
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(SqlByte p) { return p.ToSqlString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(SqlInt16 p) { return p.ToSqlString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(SqlInt32 p) { return p.ToSqlString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(SqlInt64 p) { return p.ToSqlString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(SqlSingle p) { return p.ToSqlString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(SqlDouble p) { return p.ToSqlString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(SqlDecimal p) { return p.ToSqlString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(SqlMoney p) { return p.ToSqlString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(SqlBoolean p) { return p.ToSqlString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(SqlChars p) { return p.ToSqlString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(SqlGuid p) { return p.ToSqlString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(SqlDateTime p) { return p.ToSqlString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(SqlBinary p) { return p.IsNull ? SqlString.Null : p.ToString(); }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(Type p) { return p == null ? SqlString.Null : p.FullName; }
        /// <summary>
        /// ToSqlString
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlString that convert from parater value</returns>
        public static SqlString ToSqlString(object p)
        {
            if (p == null) return SqlString.Null;

            // Scalar Types.
            //
            if (p is String) return ToSqlString((String)p);

            if (p is Char) return ToSqlString((Char)p);
            if (p is TimeSpan) return ToSqlString((TimeSpan)p);
            if (p is DateTime) return ToSqlString((DateTime)p);
            if (p is Guid) return ToSqlString((Guid)p);
            if (p is Char[]) return ToSqlString((Char[])p);

            // Nullable Types.
            //
            if (p is Char?) return ToSqlString((Char?)p);
            if (p is TimeSpan?) return ToSqlString((TimeSpan?)p);
            if (p is DateTime?) return ToSqlString((DateTime?)p);
            if (p is Guid?) return ToSqlString((Guid?)p);

            // SqlTypes.
            //
            if (p is SqlChars) return ToSqlString((SqlChars)p);
            if (p is SqlGuid) return ToSqlString((SqlGuid)p);
            if (p is SqlDateTime) return ToSqlString((SqlDateTime)p);
            if (p is SqlBinary) return ToSqlString((SqlBinary)p);
            if (p is Type) return ToSqlString((Type)p);

            return ToString(p);
        }

        #endregion

        #region SqlByte

        // Scalar Types.
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Byte p) { return p; }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(String p) { return p == null ? SqlByte.Null : SqlByte.Parse(p); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>

        public static SqlByte ToSqlByte(SByte p) { return checked((Byte)p); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Int16 p) { return checked((Byte)p); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Int32 p) { return checked((Byte)p); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Int64 p) { return checked((Byte)p); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>

        public static SqlByte ToSqlByte(UInt16 p) { return checked((Byte)p); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>

        public static SqlByte ToSqlByte(UInt32 p) { return checked((Byte)p); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>

        public static SqlByte ToSqlByte(UInt64 p) { return checked((Byte)p); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Single p) { return checked((Byte)p); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Double p) { return checked((Byte)p); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Decimal p) { return checked((Byte)p); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Char p) { return checked((Byte)p); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Boolean p) { return (Byte)(p ? 1 : 0); }

        // Nullable Types.
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Byte? p) { return p.HasValue ? p.Value : SqlByte.Null; }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>

        public static SqlByte ToSqlByte(SByte? p) { return p.HasValue ? ToByte(p.Value) : SqlByte.Null; }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Int16? p) { return p.HasValue ? ToByte(p.Value) : SqlByte.Null; }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Int32? p) { return p.HasValue ? ToByte(p.Value) : SqlByte.Null; }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Int64? p) { return p.HasValue ? ToByte(p.Value) : SqlByte.Null; }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>

        public static SqlByte ToSqlByte(UInt16? p) { return p.HasValue ? ToByte(p.Value) : SqlByte.Null; }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>

        public static SqlByte ToSqlByte(UInt32? p) { return p.HasValue ? ToByte(p.Value) : SqlByte.Null; }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>

        public static SqlByte ToSqlByte(UInt64? p) { return p.HasValue ? ToByte(p.Value) : SqlByte.Null; }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Single? p) { return p.HasValue ? ToByte(p.Value) : SqlByte.Null; }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Double? p) { return p.HasValue ? ToByte(p.Value) : SqlByte.Null; }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Boolean? p) { return p.HasValue ? ToByte(p.Value) : SqlByte.Null; }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Decimal? p) { return p.HasValue ? ToByte(p.Value) : SqlByte.Null; }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(Char? p) { return p.HasValue ? ToByte(p.Value) : SqlByte.Null; }

        // SqlTypes.
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(SqlString p) { return p.ToSqlByte(); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(SqlInt16 p) { return p.ToSqlByte(); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(SqlInt32 p) { return p.ToSqlByte(); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(SqlInt64 p) { return p.ToSqlByte(); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(SqlSingle p) { return p.ToSqlByte(); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(SqlDouble p) { return p.ToSqlByte(); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(SqlDecimal p) { return p.ToSqlByte(); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(SqlMoney p) { return p.ToSqlByte(); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(SqlBoolean p) { return p.ToSqlByte(); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(SqlDateTime p) { return p.IsNull ? SqlByte.Null : ToByte(p.Value); }
        /// <summary>
        /// ToSqlByte
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlByte that convert from parater value</returns>
        public static SqlByte ToSqlByte(object p)
        {
            if (p == null) return SqlByte.Null;

            // Scalar Types.
            //
            if (p is Byte) return ToSqlByte((Byte)p);
            if (p is String) return ToSqlByte((String)p);

            if (p is Char) return ToSqlByte((Char)p);
            if (p is Boolean) return ToSqlByte((Boolean)p);

            // Nullable Types.
            //
            if (p is Byte?) return ToSqlByte((Byte?)p);
            if (p is Char?) return ToSqlByte((Char?)p);

            // SqlTypes.
            //
            if (p is SqlDateTime) return ToSqlByte((SqlDateTime)p);

            return ToByte(p);
        }

        #endregion

        #region SqlInt16

        // Scalar Types.
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Int16 p) { return p; }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(String p) { return p == null ? SqlInt16.Null : SqlInt16.Parse(p); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>

        public static SqlInt16 ToSqlInt16(SByte p) { return checked((Int16)p); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Int32 p) { return checked((Int16)p); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Int64 p) { return checked((Int16)p); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Byte p) { return checked((Int16)p); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>

        public static SqlInt16 ToSqlInt16(UInt16 p) { return checked((Int16)p); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>

        public static SqlInt16 ToSqlInt16(UInt32 p) { return checked((Int16)p); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>

        public static SqlInt16 ToSqlInt16(UInt64 p) { return checked((Int16)p); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Single p) { return checked((Int16)p); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Double p) { return checked((Int16)p); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Decimal p) { return checked((Int16)p); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Char p) { return checked((Int16)p); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Boolean p) { return (Int16)(p ? 1 : 0); }

        // Nullable Types.
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Int16? p) { return p.HasValue ? p.Value : SqlInt16.Null; }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>

        public static SqlInt16 ToSqlInt16(SByte? p) { return p.HasValue ? ToInt16(p.Value) : SqlInt16.Null; }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Int32? p) { return p.HasValue ? ToInt16(p.Value) : SqlInt16.Null; }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Int64? p) { return p.HasValue ? ToInt16(p.Value) : SqlInt16.Null; }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Byte? p) { return p.HasValue ? ToInt16(p.Value) : SqlInt16.Null; }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>

        public static SqlInt16 ToSqlInt16(UInt16? p) { return p.HasValue ? ToInt16(p.Value) : SqlInt16.Null; }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>

        public static SqlInt16 ToSqlInt16(UInt32? p) { return p.HasValue ? ToInt16(p.Value) : SqlInt16.Null; }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>

        public static SqlInt16 ToSqlInt16(UInt64? p) { return p.HasValue ? ToInt16(p.Value) : SqlInt16.Null; }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Single? p) { return p.HasValue ? ToInt16(p.Value) : SqlInt16.Null; }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Double? p) { return p.HasValue ? ToInt16(p.Value) : SqlInt16.Null; }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Boolean? p) { return p.HasValue ? ToInt16(p.Value) : SqlInt16.Null; }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Decimal? p) { return p.HasValue ? ToInt16(p.Value) : SqlInt16.Null; }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(Char? p) { return p.HasValue ? ToInt16(p.Value) : SqlInt16.Null; }
        // SqlTypes.
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(SqlString p) { return p.ToSqlInt16(); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(SqlByte p) { return p.ToSqlInt16(); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(SqlInt32 p) { return p.ToSqlInt16(); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(SqlInt64 p) { return p.ToSqlInt16(); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(SqlSingle p) { return p.ToSqlInt16(); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(SqlDouble p) { return p.ToSqlInt16(); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(SqlDecimal p) { return p.ToSqlInt16(); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(SqlMoney p) { return p.ToSqlInt16(); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(SqlBoolean p) { return p.ToSqlInt16(); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(SqlDateTime p) { return p.IsNull ? SqlInt16.Null : ToInt16(p.Value); }
        /// <summary>
        /// ToSqlInt16
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt16 that convert from parater value</returns>
        public static SqlInt16 ToSqlInt16(object p)
        {
            if (p == null) return SqlInt16.Null;

            // Scalar Types.
            //
            if (p is Int16) return ToSqlInt16((Int16)p);
            if (p is String) return ToSqlInt16((String)p);

            if (p is Char) return ToSqlInt16((Char)p);
            if (p is Boolean) return ToSqlInt16((Boolean)p);

            // Nullable Types.
            //
            if (p is Int16?) return ToSqlInt16((Int16?)p);
            if (p is Char?) return ToSqlInt16((Char?)p);

            // SqlTypes.
            //
            if (p is SqlDateTime) return ToSqlInt16((SqlDateTime)p);

            return ToInt16(p);
        }

        #endregion

        #region SqlInt32

        // Scalar Types.
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Int32 p) { return p; }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(String p) { return p == null ? SqlInt32.Null : SqlInt32.Parse(p); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(SByte p) { return checked((Int32)p); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Int16 p) { return checked((Int32)p); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Int64 p) { return checked((Int32)p); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Byte p) { return checked((Int32)p); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(UInt16 p) { return checked((Int32)p); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(UInt32 p) { return checked((Int32)p); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(UInt64 p) { return checked((Int32)p); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Single p) { return checked((Int32)p); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Double p) { return checked((Int32)p); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Decimal p) { return checked((Int32)p); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Char p) { return checked((Int32)p); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Boolean p) { return p ? 1 : 0; }

        // Nullable Types.
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Int32? p) { return p.HasValue ? p.Value : SqlInt32.Null; }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(SByte? p) { return p.HasValue ? ToInt32(p.Value) : SqlInt32.Null; }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Int16? p) { return p.HasValue ? ToInt32(p.Value) : SqlInt32.Null; }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Int64? p) { return p.HasValue ? ToInt32(p.Value) : SqlInt32.Null; }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Byte? p) { return p.HasValue ? ToInt32(p.Value) : SqlInt32.Null; }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(UInt16? p) { return p.HasValue ? ToInt32(p.Value) : SqlInt32.Null; }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(UInt32? p) { return p.HasValue ? ToInt32(p.Value) : SqlInt32.Null; }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(UInt64? p) { return p.HasValue ? ToInt32(p.Value) : SqlInt32.Null; }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Single? p) { return p.HasValue ? ToInt32(p.Value) : SqlInt32.Null; }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Double? p) { return p.HasValue ? ToInt32(p.Value) : SqlInt32.Null; }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Boolean? p) { return p.HasValue ? ToInt32(p.Value) : SqlInt32.Null; }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Decimal? p) { return p.HasValue ? ToInt32(p.Value) : SqlInt32.Null; }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(Char? p) { return p.HasValue ? ToInt32(p.Value) : SqlInt32.Null; }

        // SqlTypes.
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(SqlString p) { return p.ToSqlInt32(); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(SqlByte p) { return p.ToSqlInt32(); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(SqlInt16 p) { return p.ToSqlInt32(); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(SqlInt64 p) { return p.ToSqlInt32(); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(SqlSingle p) { return p.ToSqlInt32(); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(SqlDouble p) { return p.ToSqlInt32(); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(SqlDecimal p) { return p.ToSqlInt32(); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(SqlMoney p) { return p.ToSqlInt32(); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(SqlBoolean p) { return p.ToSqlInt32(); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(SqlDateTime p) { return p.IsNull ? SqlInt32.Null : ToInt32(p.Value); }
        /// <summary>
        /// ToSqlInt32
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt32 that convert from parater value</returns>
        public static SqlInt32 ToSqlInt32(object p)
        {
            if (p == null) return SqlInt32.Null;

            // Scalar Types.
            //
            if (p is Int32) return ToSqlInt32((Int32)p);
            if (p is String) return ToSqlInt32((String)p);

            if (p is Char) return ToSqlInt32((Char)p);
            if (p is Boolean) return ToSqlInt32((Boolean)p);

            // Nullable Types.
            //
            if (p is Int32?) return ToSqlInt32((Int32?)p);
            if (p is Char?) return ToSqlInt32((Char?)p);

            // SqlTypes.
            //
            if (p is SqlDateTime) return ToSqlInt32((SqlDateTime)p);

            return ToInt32(p);
        }

        #endregion

        #region SqlInt64

        // Scalar Types.
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Int64 p) { return p; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(String p) { return p == null ? SqlInt64.Null : SqlInt64.Parse(p); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(SByte p) { return checked((Int64)p); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Int16 p) { return checked((Int64)p); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Int32 p) { return checked((Int64)p); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Byte p) { return checked((Int64)p); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(UInt16 p) { return checked((Int64)p); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(UInt32 p) { return checked((Int64)p); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(UInt64 p) { return checked((Int64)p); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Single p) { return checked((Int64)p); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Double p) { return checked((Int64)p); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Decimal p) { return checked((Int64)p); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Char p) { return checked((Int64)p); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Boolean p) { return p ? 1 : 0; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(DateTime p) { return (p - DateTime.MinValue).Ticks; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(TimeSpan p) { return p.Ticks; }

        // Nullable Types.
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Int64? p) { return p.HasValue ? p.Value : SqlInt64.Null; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(SByte? p) { return p.HasValue ? ToInt64(p.Value) : SqlInt64.Null; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Int16? p) { return p.HasValue ? ToInt64(p.Value) : SqlInt64.Null; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Int32? p) { return p.HasValue ? ToInt64(p.Value) : SqlInt64.Null; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Byte? p) { return p.HasValue ? ToInt64(p.Value) : SqlInt64.Null; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(UInt16? p) { return p.HasValue ? ToInt64(p.Value) : SqlInt64.Null; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(UInt32? p) { return p.HasValue ? ToInt64(p.Value) : SqlInt64.Null; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(UInt64? p) { return p.HasValue ? ToInt64(p.Value) : SqlInt64.Null; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Single? p) { return p.HasValue ? ToInt64(p.Value) : SqlInt64.Null; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Double? p) { return p.HasValue ? ToInt64(p.Value) : SqlInt64.Null; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Boolean? p) { return p.HasValue ? ToInt64(p.Value) : SqlInt64.Null; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Decimal? p) { return p.HasValue ? ToInt64(p.Value) : SqlInt64.Null; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(Char? p) { return p.HasValue ? ToInt64(p.Value) : SqlInt64.Null; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(DateTime? p) { return p.HasValue ? ToInt64(p.Value) : SqlInt64.Null; }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(TimeSpan? p) { return p.HasValue ? ToInt64(p.Value) : SqlInt64.Null; }

        // SqlTypes.
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(SqlString p) { return p.ToSqlInt64(); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(SqlByte p) { return p.ToSqlInt64(); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(SqlInt16 p) { return p.ToSqlInt64(); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(SqlInt32 p) { return p.ToSqlInt64(); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(SqlSingle p) { return p.ToSqlInt64(); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(SqlDouble p) { return p.ToSqlInt64(); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(SqlDecimal p) { return p.ToSqlInt64(); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(SqlMoney p) { return p.ToSqlInt64(); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(SqlBoolean p) { return p.ToSqlInt64(); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(SqlDateTime p) { return p.IsNull ? SqlInt64.Null : ToInt64(p.Value); }
        /// <summary>
        /// ToSqlInt64
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlInt64 that convert from parater value</returns>
        public static SqlInt64 ToSqlInt64(object p)
        {
            if (p == null) return SqlInt64.Null;

            // Scalar Types.
            //
            if (p is Int64) return ToSqlInt64((Int64)p);
            if (p is String) return ToSqlInt64((String)p);

            if (p is Char) return ToSqlInt64((Char)p);
            if (p is Boolean) return ToSqlInt64((Boolean)p);
            if (p is DateTime) return ToSqlInt64((DateTime)p);
            if (p is TimeSpan) return ToSqlInt64((TimeSpan)p);

            // Nullable Types.
            //
            if (p is Int64?) return ToSqlInt64((Int64?)p);
            if (p is Char?) return ToSqlInt64((Char?)p);
            if (p is DateTime?) return ToSqlInt64((DateTime?)p);
            if (p is TimeSpan?) return ToSqlInt64((TimeSpan?)p);

            // SqlTypes.
            //
            if (p is SqlDateTime) return ToSqlInt64((SqlDateTime)p);

            return ToInt64(p);
        }

        #endregion

        #region SqlSingle

        // Scalar Types.
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Single p) { return p; }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(String p) { return p == null ? SqlSingle.Null : SqlSingle.Parse(p); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(SByte p) { return checked((Single)p); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Int16 p) { return checked((Single)p); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Int32 p) { return checked((Single)p); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Int64 p) { return checked((Single)p); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Byte p) { return checked((Single)p); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(UInt16 p) { return checked((Single)p); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(UInt32 p) { return checked((Single)p); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(UInt64 p) { return checked((Single)p); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Double p) { return checked((Single)p); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Decimal p) { return checked((Single)p); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Char p) { return checked((Single)p); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Boolean p) { return p ? 1.0f : 0.0f; }

        // Nullable Types.
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Single? p) { return p.HasValue ? p.Value : SqlSingle.Null; }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(SByte? p) { return p.HasValue ? ToSingle(p.Value) : SqlSingle.Null; }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Int16? p) { return p.HasValue ? ToSingle(p.Value) : SqlSingle.Null; }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Int32? p) { return p.HasValue ? ToSingle(p.Value) : SqlSingle.Null; }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Int64? p) { return p.HasValue ? ToSingle(p.Value) : SqlSingle.Null; }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Byte? p) { return p.HasValue ? ToSingle(p.Value) : SqlSingle.Null; }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(UInt16? p) { return p.HasValue ? ToSingle(p.Value) : SqlSingle.Null; }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(UInt32? p) { return p.HasValue ? ToSingle(p.Value) : SqlSingle.Null; }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(UInt64? p) { return p.HasValue ? ToSingle(p.Value) : SqlSingle.Null; }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Double? p) { return p.HasValue ? ToSingle(p.Value) : SqlSingle.Null; }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Boolean? p) { return p.HasValue ? ToSingle(p.Value) : SqlSingle.Null; }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(Decimal? p) { return p.HasValue ? ToSingle(p.Value) : SqlSingle.Null; }

        // SqlTypes.
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(SqlString p) { return p.ToSqlSingle(); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(SqlByte p) { return p.ToSqlSingle(); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(SqlInt16 p) { return p.ToSqlSingle(); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(SqlInt32 p) { return p.ToSqlSingle(); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(SqlInt64 p) { return p.ToSqlSingle(); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(SqlDouble p) { return p.ToSqlSingle(); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(SqlDecimal p) { return p.ToSqlSingle(); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(SqlMoney p) { return p.ToSqlSingle(); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(SqlBoolean p) { return p.ToSqlSingle(); }
        /// <summary>
        /// ToSqlSingle
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlSingle that convert from parater value</returns>
        public static SqlSingle ToSqlSingle(object p)
        {
            if (p == null) return SqlSingle.Null;

            // Scalar Types.
            //
            if (p is Single) return ToSqlSingle((Single)p);
            if (p is String) return ToSqlSingle((String)p);

            if (p is Char) return ToSqlSingle((Char)p);
            if (p is Boolean) return ToSqlSingle((Boolean)p);

            // Nullable Types.
            //
            if (p is Single?) return ToSqlSingle((Single?)p);

            // SqlTypes.
            //

            return ToSingle(p);
        }

        #endregion

        #region SqlDouble

        // Scalar Types.
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Double p) { return p; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(String p) { return p == null ? SqlDouble.Null : SqlDouble.Parse(p); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(SByte p) { return checked((Double)p); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Int16 p) { return checked((Double)p); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Int32 p) { return checked((Double)p); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Int64 p) { return checked((Double)p); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Byte p) { return checked((Double)p); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(UInt16 p) { return checked((Double)p); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(UInt32 p) { return checked((Double)p); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(UInt64 p) { return checked((Double)p); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Single p) { return checked((Double)p); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Decimal p) { return checked((Double)p); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Char p) { return checked((Double)p); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Boolean p) { return p ? 1.0 : 0.0; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(DateTime p) { return (p - DateTime.MinValue).TotalDays; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(TimeSpan p) { return p.TotalDays; }

        // Nullable Types.
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Double? p) { return p.HasValue ? p.Value : SqlDouble.Null; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(SByte? p) { return p.HasValue ? ToDouble(p.Value) : SqlDouble.Null; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Int16? p) { return p.HasValue ? ToDouble(p.Value) : SqlDouble.Null; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Int32? p) { return p.HasValue ? ToDouble(p.Value) : SqlDouble.Null; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Int64? p) { return p.HasValue ? ToDouble(p.Value) : SqlDouble.Null; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Byte? p) { return p.HasValue ? ToDouble(p.Value) : SqlDouble.Null; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(UInt16? p) { return p.HasValue ? ToDouble(p.Value) : SqlDouble.Null; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(UInt32? p) { return p.HasValue ? ToDouble(p.Value) : SqlDouble.Null; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(UInt64? p) { return p.HasValue ? ToDouble(p.Value) : SqlDouble.Null; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Single? p) { return p.HasValue ? ToDouble(p.Value) : SqlDouble.Null; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Boolean? p) { return p.HasValue ? ToDouble(p.Value) : SqlDouble.Null; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(Decimal? p) { return p.HasValue ? ToDouble(p.Value) : SqlDouble.Null; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(DateTime? p) { return p.HasValue ? ToDouble(p.Value) : SqlDouble.Null; }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(TimeSpan? p) { return p.HasValue ? ToDouble(p.Value) : SqlDouble.Null; }

        // SqlTypes.
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(SqlString p) { return p.ToSqlDouble(); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(SqlByte p) { return p.ToSqlDouble(); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(SqlInt16 p) { return p.ToSqlDouble(); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(SqlInt32 p) { return p.ToSqlDouble(); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(SqlInt64 p) { return p.ToSqlDouble(); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(SqlSingle p) { return p.ToSqlDouble(); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(SqlDecimal p) { return p.ToSqlDouble(); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(SqlMoney p) { return p.ToSqlDouble(); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(SqlBoolean p) { return p.ToSqlDouble(); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(SqlDateTime p) { return p.IsNull ? SqlDouble.Null : ToDouble(p.Value); }
        /// <summary>
        /// ToSqlDouble
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDouble that convert from parater value</returns>
        public static SqlDouble ToSqlDouble(object p)
        {
            if (p == null) return SqlDouble.Null;

            // Scalar Types.
            //
            if (p is Double) return ToSqlDouble((Double)p);
            if (p is String) return ToSqlDouble((String)p);

            if (p is Char) return ToSqlDouble((Char)p);
            if (p is Boolean) return ToSqlDouble((Boolean)p);
            if (p is DateTime) return ToSqlDouble((DateTime)p);
            if (p is TimeSpan) return ToSqlDouble((TimeSpan)p);

            // Nullable Types.
            //
            if (p is Double?) return ToSqlDouble((Double?)p);
            if (p is DateTime?) return ToSqlDouble((DateTime?)p);
            if (p is TimeSpan?) return ToSqlDouble((TimeSpan?)p);

            // SqlTypes.
            //
            if (p is SqlDateTime) return ToSqlDouble((SqlDateTime)p);

            return ToDouble(p);
        }

        #endregion

        #region SqlDecimal

        // Scalar Types.
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Decimal p) { return p; }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(String p) { return p == null ? SqlDecimal.Null : SqlDecimal.Parse(p); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(SByte p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Int16 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Int32 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Int64 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Byte p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(UInt16 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(UInt32 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(UInt64 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Single p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Double p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Char p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Boolean p) { return p ? 1.0m : 0.0m; }

        // Nullable Types.
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Decimal? p) { return p.HasValue ? p.Value : SqlDecimal.Null; }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(SByte? p) { return p.HasValue ? ToDecimal(p.Value) : SqlDecimal.Null; }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Int16? p) { return p.HasValue ? ToDecimal(p.Value) : SqlDecimal.Null; }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Int32? p) { return p.HasValue ? ToDecimal(p.Value) : SqlDecimal.Null; }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Int64? p) { return p.HasValue ? ToDecimal(p.Value) : SqlDecimal.Null; }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Byte? p) { return p.HasValue ? ToDecimal(p.Value) : SqlDecimal.Null; }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(UInt16? p) { return p.HasValue ? ToDecimal(p.Value) : SqlDecimal.Null; }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(UInt32? p) { return p.HasValue ? ToDecimal(p.Value) : SqlDecimal.Null; }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(UInt64? p) { return p.HasValue ? ToDecimal(p.Value) : SqlDecimal.Null; }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Single? p) { return p.HasValue ? ToDecimal(p.Value) : SqlDecimal.Null; }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Double? p) { return p.HasValue ? ToDecimal(p.Value) : SqlDecimal.Null; }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(Boolean? p) { return p.HasValue ? ToDecimal(p.Value) : SqlDecimal.Null; }

        // SqlTypes.
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(SqlString p) { return p.ToSqlDecimal(); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(SqlByte p) { return p.ToSqlDecimal(); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(SqlInt16 p) { return p.ToSqlDecimal(); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(SqlInt32 p) { return p.ToSqlDecimal(); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(SqlInt64 p) { return p.ToSqlDecimal(); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(SqlSingle p) { return p.ToSqlDecimal(); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(SqlDouble p) { return p.ToSqlDecimal(); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(SqlMoney p) { return p.ToSqlDecimal(); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(SqlBoolean p) { return p.ToSqlDecimal(); }
        /// <summary>
        /// ToSqlDecimal
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDecimal that convert from parater value</returns>
        public static SqlDecimal ToSqlDecimal(object p)
        {
            if (p == null) return SqlDecimal.Null;

            // Scalar Types.
            //
            if (p is Decimal) return ToSqlDecimal((Decimal)p);
            if (p is String) return ToSqlDecimal((String)p);

            if (p is Char) return ToSqlDecimal((Char)p);
            if (p is Boolean) return ToSqlDecimal((Boolean)p);

            // Nullable Types.
            //
            if (p is Decimal?) return ToSqlDecimal((Decimal?)p);

            // SqlTypes.
            //

            return ToDecimal(p);
        }

        #endregion

        #region SqlMoney

        // Scalar Types.
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Decimal p) { return p; }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(String p) { return p == null ? SqlMoney.Null : SqlMoney.Parse(p); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(SByte p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Int16 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Int32 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Int64 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Byte p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(UInt16 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(UInt32 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(UInt64 p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Single p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Double p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Char p) { return checked((Decimal)p); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Boolean p) { return p ? 1.0m : 0.0m; }

        // Nullable Types.
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Decimal? p) { return p.HasValue ? p.Value : SqlMoney.Null; }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(SByte? p) { return p.HasValue ? ToDecimal(p.Value) : SqlMoney.Null; }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Int16? p) { return p.HasValue ? ToDecimal(p.Value) : SqlMoney.Null; }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Int32? p) { return p.HasValue ? ToDecimal(p.Value) : SqlMoney.Null; }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Int64? p) { return p.HasValue ? ToDecimal(p.Value) : SqlMoney.Null; }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Byte? p) { return p.HasValue ? ToDecimal(p.Value) : SqlMoney.Null; }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(UInt16? p) { return p.HasValue ? ToDecimal(p.Value) : SqlMoney.Null; }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(UInt32? p) { return p.HasValue ? ToDecimal(p.Value) : SqlMoney.Null; }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(UInt64? p) { return p.HasValue ? ToDecimal(p.Value) : SqlMoney.Null; }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Single? p) { return p.HasValue ? ToDecimal(p.Value) : SqlMoney.Null; }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Double? p) { return p.HasValue ? ToDecimal(p.Value) : SqlMoney.Null; }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(Boolean? p) { return p.HasValue ? ToDecimal(p.Value) : SqlMoney.Null; }

        // SqlTypes.
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(SqlString p) { return p.ToSqlMoney(); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(SqlByte p) { return p.ToSqlMoney(); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(SqlInt16 p) { return p.ToSqlMoney(); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(SqlInt32 p) { return p.ToSqlMoney(); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(SqlInt64 p) { return p.ToSqlMoney(); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(SqlSingle p) { return p.ToSqlMoney(); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(SqlDouble p) { return p.ToSqlMoney(); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(SqlDecimal p) { return p.ToSqlMoney(); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(SqlBoolean p) { return p.ToSqlMoney(); }
        /// <summary>
        /// ToSqlMoney
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlMoney that convert from parater value</returns>
        public static SqlMoney ToSqlMoney(object p)
        {
            if (p == null) return SqlMoney.Null;

            // Scalar Types.
            //
            if (p is Decimal) return ToSqlMoney((Decimal)p);
            if (p is String) return ToSqlMoney((String)p);

            if (p is Char) return ToSqlMoney((Char)p);
            if (p is Boolean) return ToSqlMoney((Boolean)p);

            // Nullable Types.
            //
            if (p is Decimal?) return ToSqlMoney((Decimal?)p);

            // SqlTypes.
            //

            return ToDecimal(p);
        }

        #endregion

        #region SqlBoolean

        // Scalar Types.
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Boolean p) { return p; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(String p) { return p == null ? SqlBoolean.Null : SqlBoolean.Parse(p); }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(SByte p) { return p != 0; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Int16 p) { return p != 0; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Int32 p) { return p != 0; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Int64 p) { return p != 0; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Byte p) { return p != 0; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(UInt16 p) { return p != 0; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(UInt32 p) { return p != 0; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(UInt64 p) { return p != 0; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Single p) { return p != 0; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Double p) { return p != 0; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Decimal p) { return p != 0; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Char p) { return p != 0; }

        // Nullable Types.
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Boolean? p) { return p.HasValue ? p.Value : SqlBoolean.Null; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(SByte? p) { return p.HasValue ? ToBoolean(p.Value) : SqlBoolean.Null; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Int16? p) { return p.HasValue ? ToBoolean(p.Value) : SqlBoolean.Null; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Int32? p) { return p.HasValue ? ToBoolean(p.Value) : SqlBoolean.Null; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Int64? p) { return p.HasValue ? ToBoolean(p.Value) : SqlBoolean.Null; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Byte? p) { return p.HasValue ? ToBoolean(p.Value) : SqlBoolean.Null; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(UInt16? p) { return p.HasValue ? ToBoolean(p.Value) : SqlBoolean.Null; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(UInt32? p) { return p.HasValue ? ToBoolean(p.Value) : SqlBoolean.Null; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(UInt64? p) { return p.HasValue ? ToBoolean(p.Value) : SqlBoolean.Null; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Single? p) { return p.HasValue ? ToBoolean(p.Value) : SqlBoolean.Null; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Double? p) { return p.HasValue ? ToBoolean(p.Value) : SqlBoolean.Null; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Decimal? p) { return p.HasValue ? ToBoolean(p.Value) : SqlBoolean.Null; }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(Char? p) { return p.HasValue ? ToBoolean(p.Value) : SqlBoolean.Null; }

        // SqlTypes.
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(SqlString p) { return p.ToSqlBoolean(); }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(SqlByte p) { return p.ToSqlBoolean(); }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(SqlInt16 p) { return p.ToSqlBoolean(); }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(SqlInt32 p) { return p.ToSqlBoolean(); }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(SqlInt64 p) { return p.ToSqlBoolean(); }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(SqlSingle p) { return p.ToSqlBoolean(); }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(SqlDouble p) { return p.ToSqlBoolean(); }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(SqlDecimal p) { return p.ToSqlBoolean(); }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(SqlMoney p) { return p.ToSqlBoolean(); }
        /// <summary>
        /// ToSqlBoolean
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBoolean that convert from parater value</returns>
        public static SqlBoolean ToSqlBoolean(object p)
        {
            if (p == null) return SqlBoolean.Null;

            // Scalar Types.
            //
            if (p is Boolean) return ToSqlBoolean((Boolean)p);
            if (p is String) return ToSqlBoolean((String)p);

            if (p is Char) return ToSqlBoolean((Char)p);

            // Nullable Types.
            //
            if (p is Boolean?) return ToSqlBoolean((Boolean?)p);
            if (p is Char?) return ToSqlBoolean((Char?)p);

            // SqlTypes.
            //

            return ToBoolean(p);
        }

        #endregion

        #region SqlDateTime

        // Scalar Types.
        /// <summary>
        /// ToSqlDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDateTime that convert from parater value</returns>
        public static SqlDateTime ToSqlDateTime(String p) { return p == null ? SqlDateTime.Null : SqlDateTime.Parse(p); }
        /// <summary>
        /// ToSqlDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDateTime that convert from parater value</returns>
        public static SqlDateTime ToSqlDateTime(DateTime p) { return p; }
        /// <summary>
        /// ToSqlDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDateTime that convert from parater value</returns>
        public static SqlDateTime ToSqlDateTime(TimeSpan p) { return ToDateTime(p); }
        /// <summary>
        /// ToSqlDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDateTime that convert from parater value</returns>
        public static SqlDateTime ToSqlDateTime(Int64 p) { return ToDateTime(p); }
        /// <summary>
        /// ToSqlDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDateTime that convert from parater value</returns>
        public static SqlDateTime ToSqlDateTime(Double p) { return ToDateTime(p); }

        // Nullable Types.
        /// <summary>
        /// ToSqlDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDateTime that convert from parater value</returns>
        public static SqlDateTime ToSqlDateTime(DateTime? p) { return p.HasValue ? p.Value : SqlDateTime.Null; }
        /// <summary>
        /// ToSqlDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDateTime that convert from parater value</returns>
        public static SqlDateTime ToSqlDateTime(TimeSpan? p) { return p.HasValue ? ToDateTime(p.Value) : SqlDateTime.Null; }
        /// <summary>
        /// ToSqlDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDateTime that convert from parater value</returns>
        public static SqlDateTime ToSqlDateTime(Int64? p) { return p.HasValue ? ToDateTime(p.Value) : SqlDateTime.Null; }
        /// <summary>
        /// ToSqlDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDateTime that convert from parater value</returns>
        public static SqlDateTime ToSqlDateTime(Double? p) { return p.HasValue ? ToDateTime(p.Value) : SqlDateTime.Null; }

        // SqlTypes.
        /// <summary>
        /// ToSqlDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDateTime that convert from parater value</returns>
        public static SqlDateTime ToSqlDateTime(SqlString p) { return p.ToSqlDateTime(); }
        /// <summary>
        /// ToSqlDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDateTime that convert from parater value</returns>
        public static SqlDateTime ToSqlDateTime(SqlInt64 p) { return p.IsNull ? SqlDateTime.Null : ToDateTime(p); }
        /// <summary>
        /// ToSqlDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDateTime that convert from parater value</returns>
        public static SqlDateTime ToSqlDateTime(SqlDouble p) { return p.IsNull ? SqlDateTime.Null : ToDateTime(p); }
        /// <summary>
        /// ToSqlDateTime
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlDateTime that convert from parater value</returns>
        public static SqlDateTime ToSqlDateTime(object p)
        {
            if (p == null) return SqlDateTime.Null;

            // Scalar Types.
            //
            if (p is String) return ToSqlDateTime((String)p);
            if (p is DateTime) return ToSqlDateTime((DateTime)p);
            if (p is TimeSpan) return ToSqlDateTime((TimeSpan)p);
            if (p is Int64) return ToSqlDateTime((Int64)p);
            if (p is Double) return ToSqlDateTime((Double)p);

            // Nullable Types.
            //
            if (p is DateTime?) return ToSqlDateTime((DateTime?)p);
            if (p is TimeSpan?) return ToSqlDateTime((TimeSpan?)p);
            if (p is Int64?) return ToSqlDateTime((Int64?)p);
            if (p is Double?) return ToSqlDateTime((Double?)p);

            // SqlTypes.
            //
            if (p is SqlString) return ToSqlDateTime((SqlString)p);
            if (p is SqlInt64) return ToSqlDateTime((SqlInt64)p);
            if (p is SqlDouble) return ToSqlDateTime((SqlDouble)p);

            return ToDateTime(p);
        }

        #endregion

        #region SqlGuid

        // Scalar Types.
        /// <summary>
        /// ToSqlGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlGuid that convert from parater value</returns>
        public static SqlGuid ToSqlGuid(Guid p) { return p; }
        /// <summary>
        /// ToSqlGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlGuid that convert from parater value</returns>
        public static SqlGuid ToSqlGuid(String p) { return p == null ? SqlGuid.Null : SqlGuid.Parse(p); }

        // Nullable Types.
        /// <summary>
        /// ToSqlGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlGuid that convert from parater value</returns>
        public static SqlGuid ToSqlGuid(Guid? p) { return p.HasValue ? p.Value : SqlGuid.Null; }

        // SqlTypes.
        /// <summary>
        /// ToSqlGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlGuid that convert from parater value</returns>
        public static SqlGuid ToSqlGuid(SqlBinary p) { return p.ToSqlGuid(); }
        /// <summary>
        /// ToSqlGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlGuid that convert from parater value</returns>
        public static SqlGuid ToSqlGuid(SqlBytes p) { return p.ToSqlBinary().ToSqlGuid(); }
        /// <summary>
        /// ToSqlGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlGuid that convert from parater value</returns>
        public static SqlGuid ToSqlGuid(SqlString p) { return p.ToSqlGuid(); }

        // Other Types.
        /// <summary>
        /// ToSqlGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlGuid that convert from parater value</returns>
        public static SqlGuid ToSqlGuid(Type p) { return p == null ? SqlGuid.Null : p.GUID; }
        /// <summary>
        /// ToSqlGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlGuid that convert from parater value</returns>
        public static SqlGuid ToSqlGuid(Byte[] p) { return p == null ? SqlGuid.Null : new SqlGuid(p); }
        /// <summary>
        /// ToSqlGuid
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlGuid that convert from parater value</returns>
        public static SqlGuid ToSqlGuid(object p)
        {
            if (p == null) return SqlGuid.Null;

            // Scalar Types.
            //
            if (p is Guid) return ToSqlGuid((Guid)p);
            if (p is String) return ToSqlGuid((String)p);

            // Nullable Types.
            //
            if (p is Guid?) return ToSqlGuid((Guid?)p);

            // SqlTypes.
            //
            if (p is SqlBinary) return ToSqlGuid((SqlBinary)p);
            if (p is SqlBytes) return ToSqlGuid((SqlBytes)p);
            if (p is SqlString) return ToSqlGuid((SqlString)p);

            // Other Types.
            //
            if (p is Type) return ToSqlGuid((Type)p);
            if (p is Byte[]) return ToSqlGuid((Byte[])p);

            return ToGuid(p);
        }

        #endregion

        #region SqlBinary

        // Scalar Types.
        /// <summary>
        /// ToSqlBinary
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBinary that convert from parater value</returns>
        public static SqlBinary ToSqlBinary(Byte[] p) { return p; }
        /// <summary>
        /// ToSqlBinary
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBinary that convert from parater value</returns>
        public static SqlBinary ToSqlBinary(Guid p) { return p == Guid.Empty ? SqlBinary.Null : new SqlGuid(p).ToSqlBinary(); }

        // Nullable Types.
        /// <summary>
        /// ToSqlBinary
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBinary that convert from parater value</returns>
        public static SqlBinary ToSqlBinary(Guid? p) { return p.HasValue ? new SqlGuid(p.Value).ToSqlBinary() : SqlBinary.Null; }

        // SqlTypes.
        /// <summary>
        /// ToSqlBinary
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBinary that convert from parater value</returns>
        public static SqlBinary ToSqlBinary(SqlBytes p) { return p.ToSqlBinary(); }
        /// <summary>
        /// ToSqlBinary
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBinary that convert from parater value</returns>
        public static SqlBinary ToSqlBinary(SqlGuid p) { return p.ToSqlBinary(); }
        /// <summary>
        /// ToSqlBinary
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBinary that convert from parater value</returns>
        public static SqlBinary ToSqlBinary(object p)
        {
            if (p == null) return SqlBinary.Null;

            // Scalar Types.
            //
            if (p is Byte[]) return ToSqlBinary((Byte[])p);
            if (p is Guid) return ToSqlBinary((Guid)p);

            // Nullable Types.
            //
            if (p is Guid?) return ToSqlBinary((Guid?)p);

            // SqlTypes.
            //
            if (p is SqlBytes) return ToSqlBinary((SqlBytes)p);
            if (p is SqlGuid) return ToSqlBinary((SqlGuid)p);

            return ToByteArray(p);
        }

        #endregion

        #region SqlBytes

        // Scalar Types.
        /// <summary>
        /// ToSqlBytes
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBytes that convert from parater value</returns>
        public static SqlBytes ToSqlBytes(Byte[] p) { return p == null ? SqlBytes.Null : new SqlBytes(p); }
        /// <summary>
        /// ToSqlBytes
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBytes that convert from parater value</returns>
        public static SqlBytes ToSqlBytes(Stream p) { return p == null ? SqlBytes.Null : new SqlBytes(p); }
        /// <summary>
        /// ToSqlBytes
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBytes that convert from parater value</returns>
        public static SqlBytes ToSqlBytes(Guid p) { return p == Guid.Empty ? SqlBytes.Null : new SqlBytes(p.ToByteArray()); }

        // Nullable Types.
        /// <summary>
        /// ToSqlBytes
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBytes that convert from parater value</returns>
        public static SqlBytes ToSqlBytes(Guid? p) { return p.HasValue ? new SqlBytes(p.Value.ToByteArray()) : SqlBytes.Null; }

        // SqlTypes.
        /// <summary>
        /// ToSqlBytes
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBytes that convert from parater value</returns>
        public static SqlBytes ToSqlBytes(SqlBinary p) { return p.IsNull ? SqlBytes.Null : new SqlBytes(p); }
        /// <summary>
        /// ToSqlBytes
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBytes that convert from parater value</returns>
        public static SqlBytes ToSqlBytes(SqlGuid p) { return p.IsNull ? SqlBytes.Null : new SqlBytes(p.ToByteArray()); }
        /// <summary>
        /// ToSqlBytes
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlBytes that convert from parater value</returns>
        public static SqlBytes ToSqlBytes(object p)
        {
            if (p == null) return SqlBytes.Null;

            // Scalar Types.
            //
            if (p is Byte[]) return ToSqlBytes((Byte[])p);
            if (p is Stream) return ToSqlBytes((Stream)p);
            if (p is Guid) return ToSqlBytes((Guid)p);

            // Nullable Types.
            //
            if (p is Guid?) return ToSqlBytes((Guid?)p);

            // SqlTypes.
            //
            if (p is SqlBinary) return ToSqlBytes((SqlBinary)p);
            if (p is SqlGuid) return ToSqlBytes((SqlGuid)p);

            return new SqlBytes(ToByteArray(p));
        }

        #endregion

        #region SqlChars

        // Scalar Types.
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(String p) { return p == null ? SqlChars.Null : new SqlChars(p.ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Char[] p) { return p == null ? SqlChars.Null : new SqlChars(p); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(SByte p) { return new SqlChars(ToString(p).ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Int16 p) { return new SqlChars(ToString(p).ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Int32 p) { return new SqlChars(ToString(p).ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Int64 p) { return new SqlChars(ToString(p).ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Byte p) { return new SqlChars(ToString(p).ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(UInt16 p) { return new SqlChars(ToString(p).ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(UInt32 p) { return new SqlChars(ToString(p).ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(UInt64 p) { return new SqlChars(ToString(p).ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Single p) { return new SqlChars(ToString(p).ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Double p) { return new SqlChars(ToString(p).ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Boolean p) { return new SqlChars(ToString(p).ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Decimal p) { return new SqlChars(ToString(p).ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Char p) { return new SqlChars(ToString(p).ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(TimeSpan p) { return new SqlChars(ToString(p).ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(DateTime p) { return new SqlChars(ToString(p).ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Guid p) { return new SqlChars(ToString(p).ToCharArray()); }

        // Nullable Types.
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(SByte? p) { return p.HasValue ? new SqlChars(p.ToString().ToCharArray()) : SqlChars.Null; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Int16? p) { return p.HasValue ? new SqlChars(p.ToString().ToCharArray()) : SqlChars.Null; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Int32? p) { return p.HasValue ? new SqlChars(p.ToString().ToCharArray()) : SqlChars.Null; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Int64? p) { return p.HasValue ? new SqlChars(p.ToString().ToCharArray()) : SqlChars.Null; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Byte? p) { return p.HasValue ? new SqlChars(p.ToString().ToCharArray()) : SqlChars.Null; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(UInt16? p) { return p.HasValue ? new SqlChars(p.ToString().ToCharArray()) : SqlChars.Null; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(UInt32? p) { return p.HasValue ? new SqlChars(p.ToString().ToCharArray()) : SqlChars.Null; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(UInt64? p) { return p.HasValue ? new SqlChars(p.ToString().ToCharArray()) : SqlChars.Null; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Single? p) { return p.HasValue ? new SqlChars(p.ToString().ToCharArray()) : SqlChars.Null; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Double? p) { return p.HasValue ? new SqlChars(p.ToString().ToCharArray()) : SqlChars.Null; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Boolean? p) { return p.HasValue ? new SqlChars(p.ToString().ToCharArray()) : SqlChars.Null; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Decimal? p) { return p.HasValue ? new SqlChars(p.ToString().ToCharArray()) : SqlChars.Null; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Char? p) { return p.HasValue ? new SqlChars(new Char[] { p.Value }) : SqlChars.Null; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(TimeSpan? p) { return p.HasValue ? new SqlChars(p.ToString().ToCharArray()) : SqlChars.Null; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(DateTime? p) { return p.HasValue ? new SqlChars(p.ToString().ToCharArray()) : SqlChars.Null; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Guid? p) { return p.HasValue ? new SqlChars(p.ToString().ToCharArray()) : SqlChars.Null; }

        // SqlTypes.
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(SqlString p) { return (SqlChars)p; }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(SqlByte p) { return (SqlChars)p.ToSqlString(); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(SqlInt16 p) { return (SqlChars)p.ToSqlString(); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(SqlInt32 p) { return (SqlChars)p.ToSqlString(); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(SqlInt64 p) { return (SqlChars)p.ToSqlString(); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(SqlSingle p) { return (SqlChars)p.ToSqlString(); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(SqlDouble p) { return (SqlChars)p.ToSqlString(); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(SqlDecimal p) { return (SqlChars)p.ToSqlString(); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(SqlMoney p) { return (SqlChars)p.ToSqlString(); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(SqlBoolean p) { return (SqlChars)p.ToSqlString(); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(SqlGuid p) { return (SqlChars)p.ToSqlString(); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(SqlDateTime p) { return (SqlChars)p.ToSqlString(); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(SqlBinary p) { return p.IsNull ? SqlChars.Null : new SqlChars(p.ToString().ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(Type p) { return p == null ? SqlChars.Null : new SqlChars(p.FullName.ToCharArray()); }
        /// <summary>
        /// ToSqlChars
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlChars that convert from parater value</returns>
        public static SqlChars ToSqlChars(object p) { return new SqlChars(ToString(p).ToCharArray()); }

        #endregion

        #region SqlXml

        // Scalar Types.
        /// <summary>
        /// ToSqlXml
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlXml that convert from parater value</returns>
        public static SqlXml ToSqlXml(String p) { return p == null ? SqlXml.Null : new SqlXml(new XmlTextReader(new StringReader(p))); }
        /// <summary>
        /// ToSqlXml
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlXml that convert from parater value</returns>
        public static SqlXml ToSqlXml(Stream p) { return p == null ? SqlXml.Null : new SqlXml(p); }
        /// <summary>
        /// ToSqlXml
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlXml that convert from parater value</returns>
        public static SqlXml ToSqlXml(XmlReader p) { return p == null ? SqlXml.Null : new SqlXml(p); }
        /// <summary>
        /// ToSqlXml
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlXml that convert from parater value</returns>
        public static SqlXml ToSqlXml(Char[] p) { return p == null ? SqlXml.Null : new SqlXml(new XmlTextReader(new StringReader(new string(p)))); }
        /// <summary>
        /// ToSqlXml
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlXml that convert from parater value</returns>
        public static SqlXml ToSqlXml(Byte[] p) { return p == null ? SqlXml.Null : new SqlXml(new MemoryStream(p)); }

        // SqlTypes.
        /// <summary>
        /// ToSqlXml
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlXml that convert from parater value</returns>
        public static SqlXml ToSqlXml(SqlString p) { return p.IsNull ? SqlXml.Null : new SqlXml(new XmlTextReader(new StringReader(p.Value))); }
        /// <summary>
        /// ToSqlXml
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlXml that convert from parater value</returns>
        public static SqlXml ToSqlXml(SqlChars p) { return p.IsNull ? SqlXml.Null : new SqlXml(new XmlTextReader(new StringReader(p.ToSqlString().Value))); }
        /// <summary>
        /// ToSqlXml
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlXml that convert from parater value</returns>
        public static SqlXml ToSqlXml(SqlBinary p) { return p.IsNull ? SqlXml.Null : new SqlXml(new MemoryStream(p.Value)); }
        /// <summary>
        /// ToSqlXml
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlXml that convert from parater value</returns>
        public static SqlXml ToSqlXml(SqlBytes p) { return p.IsNull ? SqlXml.Null : new SqlXml(p.Stream); }
        /// <summary>
        /// ToSqlXml
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>SqlXml that convert from parater value</returns>
        public static SqlXml ToSqlXml(object p)
        {
            if (p == null) return SqlXml.Null;

            // Scalar Types.
            //
            if (p is String) return ToSqlXml((String)p);

            if (p is Stream) return ToSqlXml((Stream)p);
            if (p is XmlReader) return ToSqlXml((XmlReader)p);

            if (p is Char[]) return ToSqlXml((Char[])p);
            if (p is Byte[]) return ToSqlXml((Byte[])p);

            // SqlTypes.
            //
            if (p is SqlString) return ToSqlXml((SqlString)p);
            if (p is SqlChars) return ToSqlXml((SqlChars)p);
            if (p is SqlBinary) return ToSqlXml((SqlBinary)p);
            if (p is SqlBytes) return ToSqlXml((SqlBytes)p);

            throw CreateInvalidCastException(p.GetType(), typeof(SqlXml));
        }

        #endregion

        #endregion

        #region Other types

        #region Type

        // Scalar Types.
        /// <summary>
        /// ToType
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Type that convert from parater value</returns>
        public static Type ToType(String p) { return p == null ? null : Type.GetType(p); }
        /// <summary>
        /// ToType
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Type that convert from parater value</returns>
        public static Type ToType(Char[] p) { return p == null ? null : Type.GetType(new string(p)); }
        /// <summary>
        /// ToType
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Type that convert from parater value</returns>
        public static Type ToType(Guid p) { return p == Guid.Empty ? null : Type.GetTypeFromCLSID(p); }

        // Nullable Types.
        /// <summary>
        /// ToType
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Type that convert from parater value</returns>
        public static Type ToType(Guid? p) { return p.HasValue ? Type.GetTypeFromCLSID(p.Value) : null; }

        // SqlTypes.
        /// <summary>
        /// ToType
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Type that convert from parater value</returns>
        public static Type ToType(SqlString p) { return p.IsNull ? null : Type.GetType(p.Value); }
        /// <summary>
        /// ToType
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Type that convert from parater value</returns>
        public static Type ToType(SqlChars p) { return p.IsNull ? null : Type.GetType(new string(p.Value)); }
        /// <summary>
        /// ToType
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Type that convert from parater value</returns>
        public static Type ToType(SqlGuid p) { return p.IsNull ? null : Type.GetTypeFromCLSID(p.Value); }
        /// <summary>
        /// ToType
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Type that convert from parater value</returns>
        public static Type ToType(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is String) return ToType((String)p);
            if (p is Char[]) return ToType((Char[])p);
            if (p is Guid) return ToType((Guid)p);

            // Nullable Types.
            //
            if (p is Guid?) return ToType((Guid?)p);

            // SqlTypes.
            //
            if (p is SqlString) return ToType((SqlString)p);
            if (p is SqlChars) return ToType((SqlChars)p);
            if (p is SqlGuid) return ToType((SqlGuid)p);

            throw CreateInvalidCastException(p.GetType(), typeof(Type));
        }

        #endregion

        #region Stream

        // Scalar Types.
        /// <summary>
        /// ToStream
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Stream that convert from parater value</returns>
        public static Stream ToStream(Guid p) { return p == Guid.Empty ? Stream.Null : new MemoryStream(p.ToByteArray()); }
        /// <summary>
        /// ToStream
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Stream that convert from parater value</returns>
        public static Stream ToStream(Byte[] p) { return p == null ? Stream.Null : new MemoryStream(p); }

        // Nullable Types.
        /// <summary>
        /// ToStream
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Stream that convert from parater value</returns>
        public static Stream ToStream(Guid? p) { return p.HasValue ? new MemoryStream(p.Value.ToByteArray()) : Stream.Null; }

        // SqlTypes.
        /// <summary>
        /// ToStream
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Stream that convert from parater value</returns>
        public static Stream ToStream(SqlBytes p) { return p.IsNull ? Stream.Null : p.Stream; }
        /// <summary>
        /// ToStream
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Stream that convert from parater value</returns>
        public static Stream ToStream(SqlBinary p) { return p.IsNull ? Stream.Null : new MemoryStream(p.Value); }
        /// <summary>
        /// ToStream
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Stream that convert from parater value</returns>
        public static Stream ToStream(SqlGuid p) { return p.IsNull ? Stream.Null : new MemoryStream(p.Value.ToByteArray()); }
        /// <summary>
        /// ToStream
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Stream that convert from parater value</returns>
        public static Stream ToStream(object p)
        {
            if (p == null) return Stream.Null;

            // Scalar Types.
            //
            if (p is Guid) return ToStream((Guid)p);
            if (p is Byte[]) return ToStream((Byte[])p);

            // Nullable Types.
            //
            if (p is Guid?) return ToStream((Guid?)p);

            // SqlTypes.
            //
            if (p is SqlBytes) return ToStream((SqlBytes)p);
            if (p is SqlBinary) return ToStream((SqlBinary)p);
            if (p is SqlGuid) return ToStream((SqlGuid)p);

            throw CreateInvalidCastException(p.GetType(), typeof(Stream));
        }

        #endregion

        #region Byte[]

        // Scalar Types.
        /// <summary>
        /// ToByteArray
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte[] that convert from parater value</returns>
        public static Byte[] ToByteArray(Stream p)
        {
            if (p == null) return null;
            if (p is MemoryStream) return ((MemoryStream)p).ToArray();

            long position = p.Seek(0, SeekOrigin.Begin);
            Byte[] bytes = new Byte[p.Length];
            p.Read(bytes, 0, bytes.Length);
            p.Position = position;

            return bytes;

        }
        /// <summary>
        /// ToByteArray
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte[] that convert from parater value</returns>
        public static Byte[] ToByteArray(Guid p) { return p == Guid.Empty ? null : p.ToByteArray(); }
        // Nullable Types.
        /// <summary>
        /// ToByteArray
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte[] that convert from parater value</returns>
        public static Byte[] ToByteArray(Guid? p) { return p.HasValue ? p.Value.ToByteArray() : null; }
        // SqlTypes.
        /// <summary>
        /// ToByteArray
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte[] that convert from parater value</returns>
        public static Byte[] ToByteArray(SqlBinary p) { return p.IsNull ? null : p.Value; }
        /// <summary>
        /// ToByteArray
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte[] that convert from parater value</returns>
        public static Byte[] ToByteArray(SqlBytes p) { return p.IsNull ? null : p.Value; }
        /// <summary>
        /// ToByteArray
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte[] that convert from parater value</returns>
        public static Byte[] ToByteArray(SqlGuid p) { return p.IsNull ? null : p.ToByteArray(); }
        /// <summary>
        /// ToByteArray
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Byte[] that convert from parater value</returns>
        public static Byte[] ToByteArray(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is Stream) return ToByteArray((Stream)p);
            if (p is Guid) return ToByteArray((Guid)p);

            // Nullable Types.
            //
            if (p is Guid?) return ToByteArray((Guid?)p);

            // SqlTypes.
            //
            if (p is SqlBinary) return ToByteArray((SqlBinary)p);
            if (p is SqlBytes) return ToByteArray((SqlBytes)p);
            if (p is SqlGuid) return ToByteArray((SqlGuid)p);

            throw CreateInvalidCastException(p.GetType(), typeof(Byte[]));
        }

        #endregion

        #region Char[]

        // Scalar Types.
        /// <summary>
        /// ToCharArray
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char[] that convert from parater value</returns>
        public static Char[] ToCharArray(String p) { return p == null ? null : p.ToCharArray(); }

        // SqlTypes.
        /// <summary>
        /// ToCharArray
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char[] that convert from parater value</returns>
        public static Char[] ToCharArray(SqlString p) { return p.IsNull ? null : p.Value.ToCharArray(); }
        /// <summary>
        /// ToCharArray
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char[] that convert from parater value</returns>
        public static Char[] ToCharArray(SqlChars p) { return p.IsNull ? null : p.Value; }
        /// <summary>
        /// ToCharArray
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>Char[] that convert from parater value</returns>
        public static Char[] ToCharArray(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is String) return ToCharArray((String)p);

            // SqlTypes.
            //
            if (p is SqlString) return ToCharArray((SqlString)p);
            if (p is SqlChars) return ToCharArray((SqlChars)p);
            return ToString(p).ToCharArray();
        }

        #endregion

        #region XmlReader

        // Scalar Types.
        /// <summary>
        /// ToXmlReader
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>XmlReader that convert from parater value</returns>
        public static XmlReader ToXmlReader(String p) { return p == null ? null : new XmlTextReader(new StringReader(p)); }

        // SqlTypes.
        /// <summary>
        /// ToXmlReader
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>XmlReader that convert from parater value</returns>
        public static XmlReader ToXmlReader(SqlString p) { return p.IsNull ? null : new XmlTextReader(new StringReader(p.Value)); }
        /// <summary>
        /// ToXmlReader
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>XmlReader that convert from parater value</returns>
        public static XmlReader ToXmlReader(SqlXml p) { return p.IsNull ? null : p.CreateReader(); }
        /// <summary>
        /// ToXmlReader
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>XmlReader that convert from parater value</returns>
        public static XmlReader ToXmlReader(SqlChars p) { return p.IsNull ? null : new XmlTextReader(new StringReader(p.ToSqlString().Value)); }
        /// <summary>
        /// ToXmlReader
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>XmlReader that convert from parater value</returns>
        public static XmlReader ToXmlReader(SqlBinary p) { return p.IsNull ? null : new XmlTextReader(new MemoryStream(p.Value)); }

        // Other Types.
        /// <summary>
        /// ToXmlReader
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>XmlReader that convert from parater value</returns>
        public static XmlReader ToXmlReader(Stream p) { return p == null ? null : new XmlTextReader(p); }
        /// <summary>
        /// ToXmlReader
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>XmlReader that convert from parater value</returns>
        public static XmlReader ToXmlReader(TextReader p) { return p == null ? null : new XmlTextReader(p); }
        /// <summary>
        /// ToXmlReader
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>XmlReader that convert from parater value</returns>
        public static XmlReader ToXmlReader(Char[] p) { return p == null ? null : new XmlTextReader(new StringReader(new string(p))); }
        /// <summary>
        /// ToXmlReader
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>XmlReader that convert from parater value</returns>
        public static XmlReader ToXmlReader(Byte[] p) { return p == null ? null : new XmlTextReader(new MemoryStream(p)); }
        /// <summary>
        /// ToXmlReader
        /// </summary>
        /// <param name="p">value to convert</param>
        /// <returns>XmlReader that convert from parater value</returns>
        public static XmlReader ToXmlReader(object p)
        {
            if (p == null) return null;

            // Scalar Types.
            //
            if (p is String) return ToXmlReader((String)p);

            // SqlTypes.
            //
            if (p is SqlString) return ToXmlReader((SqlString)p);

            if (p is SqlXml) return ToXmlReader((SqlXml)p);
            if (p is SqlChars) return ToXmlReader((SqlChars)p);

            if (p is SqlBinary) return ToXmlReader((SqlBinary)p);

            // Other Types.
            //
            if (p is Stream) return ToXmlReader((Stream)p);
            if (p is TextReader) return ToXmlReader((TextReader)p);

            if (p is Char[]) return ToXmlReader((Char[])p);
            if (p is Byte[]) return ToXmlReader((Byte[])p);

            throw CreateInvalidCastException(p.GetType(), typeof(XmlReader));
        }

        #endregion

        #endregion

        #region Exception Helper Method

        private static InvalidCastException CreateInvalidCastException(Type originalType, Type conversionType)
        {
            return new InvalidCastException(
                string.Format("Invalid cast from {0} to {1}",
                originalType.FullName, conversionType.FullName));
        }

        #endregion
    }

    #endregion

    #region Nullable Convert (internal class)

    /// <summary>
    /// Nullable Convert (internal class).
    /// </summary>
    /// <typeparam name="T">Destination data type.</typeparam>
    /// <typeparam name="P">Source data type.</typeparam>
    internal static class NullableConvert<T, P>
        where T : struct
        where P : struct
    {
        /// <summary>
        /// From Nullable.
        /// </summary>
        /// <param name="p">The parent.</param>
        /// <returns></returns>
        public static T? FromNullable(P? p)
        {
            return p.HasValue ? From(p.Value) : null;
        }
        /// <summary>
        /// From
        /// </summary>
        /// <param name="p">The parent.</param>
        /// <returns></returns>
        public static T? From(P p)
        {
            return Convert<T, P>.From(p);
        }
    }

    #endregion

    #region Convert (Generic version)

    /// <summary>
    /// Converts a base data type to another base data type.
    /// </summary>
    /// <typeparam name="P">Source data type.</typeparam>
    /// <typeparam name="T">Destination data type.</typeparam>
    public static class Convert<T, P>
    {
        #region Static Method

        private static P SameType(P p) { return p; }
        private static T Assignable(P p) { return (T)(object)p; }
        private static T Default(P p) { return (T)System.Convert.ChangeType(p, typeof(T)); }
        private static MethodInfo FindTypeCastOperator(Type t)
        {
            foreach (MethodInfo mi in t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (mi.IsSpecialName && mi.ReturnType == typeof(T) && (mi.Name == "op_Implicit" || mi.Name == "op_Explicit"))
                {
                    ParameterInfo[] parameters = mi.GetParameters();

                    if (1 == parameters.Length && parameters[0].ParameterType == typeof(P))
                        return mi;
                }
            }

            return null;
        }

        #endregion

        #region variable and delegate

        ///<summary>
        /// The Converter instance.
        ///</summary>
        public static ConvertMethod From = GetConverter();
        /// <summary>
        /// Represents a method that converts an object from one type to another type.
        /// </summary>
        /// <param name="p">A value to convert to the target type.</param>
        /// <returns>The <typeparamref name="T"/> that represents the converted <paramref name="p"/>.</returns>
        public delegate T ConvertMethod(P p);

        #endregion

        #region Public Access

        /// <summary>Converts an array of one type to an array of another type.</summary>
        /// <returns>An array of the target type containing the converted elements from the source array.</returns>
        /// <param name="src">The one-dimensional, zero-based <see cref="T:System.Array"></see> to convert to a target type.</param>
        /// <exception cref="T:System.ArgumentNullException">array is null.-or-converter is null.</exception>
        public static T[] FromArray(P[] src)
        {
            // Note that type parameters are in reverse order.
            return Array.ConvertAll<P, T>(src, (Converter<P, T>)((object)From));
        }
        ///<summary>
        /// Initializes converter instance.
        ///</summary>
        ///<returns>Converter instance.</returns>
        public static ConvertMethod GetConverter()
        {
            Type from = typeof(P);
            Type to = typeof(T);

            // Convert to the same type.
            //
            if (to == from)
                return (ConvertMethod)(object)(Convert<P, P>.ConvertMethod)SameType;

            if (from.IsEnum)
                from = Enum.GetUnderlyingType(from);

            if (to.IsEnum)
                to = Enum.GetUnderlyingType(to);

            if (TypeEx.IsSameOrParent(to, from))
                return Assignable;

            string methodName;

            if (TypeEx.IsNullable(to))
                methodName = "ToNullable" + to.GetGenericArguments()[0].Name;
            else if (to.IsArray)
                methodName = "To" + to.GetElementType().Name + "Array";
            else
                methodName = "To" + to.Name;

            MethodInfo mi = typeof(Convert).GetMethod(methodName,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.ExactBinding,
                null, new Type[] { from }, null) ?? FindTypeCastOperator(to) ?? FindTypeCastOperator(from);

            if (mi == null && TypeEx.IsNullable(to))
            {
                // To-nullable conversion.
                // We have to use reflection to enforce some constraints.
                Type toType = to.GetGenericArguments()[0];
                Type fromType = TypeEx.IsNullable(from) ? from.GetGenericArguments()[0] : from;
                methodName = TypeEx.IsNullable(from) ? "FromNullable" : "From";

                mi = typeof(NullableConvert<,>)
                    .MakeGenericType(toType, fromType)
                    .GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            }

            if (mi != null)
                return (ConvertMethod)Delegate.CreateDelegate(typeof(ConvertMethod), mi);

            return Default;
        }

        #endregion
    }

    #endregion

    #region ConvertTo (Generic version)

    /// <summary>
    /// Converts a base data type to another base data type.
    /// </summary>
    /// <typeparam name="T">Destination data type.</typeparam>
    public static class ConvertTo<T>
    {
        /// <summary>Returns an <typeparamref name="T"/> whose value is equivalent to the specified value.</summary>
        /// <returns>The <typeparamref name="T"/> that represents the converted <paramref name="p"/>.</returns>
        /// <param name="p">A value to convert to the target type.</param>
        public static T From<P>(P p)
        {
            return Convert<T, P>.From(p);
        }
    }

    #endregion
}

