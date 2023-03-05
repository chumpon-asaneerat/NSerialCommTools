#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-09-01
=================
- EnunUtils class updated.
  - Refactor code by change ArrayList ot List<T>.

======================================================================================================================
Update 2013-08-19
=================
- EnunUtils class ported from NLib40x3 to NLib40x5.
  - Required to changed ArrayList to List<T>.

======================================================================================================================
Update 2012-12-27
=================
- EnunUtils class ported from GFA38v3 to NLib40x3.
  - Change namespace from SysLib to NLib.

======================================================================================================================
Update 2010-01-24
=================
- EnunUtils class ported and related classes from GFA Library GFA37 tor GFA38v3.
  (Standard/Pro version)

======================================================================================================================
Update 2008-11-27
=================
- EnunUtils move from GFA.Lib to GFA.Lib.Core.
- EnunSource class move from GFA.Lib to GFA.Lib.Core.
- Enun Flag Manager move from GFA.Lib to GFA.Lib.Core.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

#endregion

namespace NLib.Utils
{
    #region Enum Utils

    /// <summary>
    /// Enum DataType util
    /// </summary>
    public class EnumUtils
    {
        #region Get Description

        /// <summary>
        /// Get Enum Description
        /// </summary>
        /// <param name="enumDataType">Enum Type's insatance</param>
        /// <param name="value">Value in enumerate type</param>
        /// <returns>Description for specificed value</returns>
        public static string GetDescription(Type enumDataType, object value)
        {
            FieldInfo fi = enumDataType.GetField(value.ToString());
            DescriptionAttribute[] attributes = 
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// Get Enum Description
        /// </summary>
        /// <param name="value">Value in enumerate type</param>
        /// <returns>Description for specificed value</returns>
        public static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = 
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        #endregion
    }

    #endregion

    #region Enum Source

    /// <summary>
    /// Enum Source Class
    /// </summary>
    public class EnumDataSource
    {
        #region Internal Variable

        private string _enumDesc = "";
        private Type _enumType;
        private object _enumValue = null;

        #endregion

        #region Constructor

        /// <summary>
        /// hide Constructor
        /// </summary>
        private EnumDataSource() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enumType">Enum Type</param>
        /// <param name="enumValue">Enum Field Value</param>
        public EnumDataSource(Type enumType, object enumValue)
        {
            _enumType = enumType;
            _enumValue = enumValue;
            _enumDesc = _enumValue.ToString();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enumType">Enum Type</param>
        /// <param name="enumValue">Enum Field Value</param>
        /// <param name="enumDesc">Description for specificed emum's value</param>
        public EnumDataSource(Type enumType, object enumValue, string enumDesc)
        {
            _enumType = enumType;
            _enumValue = enumValue;
            _enumDesc = enumDesc;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Name
        /// </summary>
        public string Name
        {
            get { return _enumValue.ToString(); }
            set { _enumValue = Enum.Parse(_enumType, value, true); }
        }

        /// <summary>
        /// Get/Set Destription
        /// </summary>
        public string Description
        {
            get { return _enumDesc; }
            set { _enumDesc = value; }
        }

        /// <summary>
        /// Get/Set Value
        /// </summary>
        public object Value
        {
            get { return _enumValue; }
            set { _enumValue = value; }
        }

        #endregion

        #region Static Method

        /// <summary>
        /// Create Enum DataSource
        /// </summary>
        /// <param name="enumDataType">Enum DataType</param>
        /// <returns>EnumDataSource Array that can bind to control with databinding</returns>
        public static EnumDataSource[] ToDataSource(Type enumDataType)
        {
            if (enumDataType == null || !enumDataType.IsEnum) return null;
            Array values = Enum.GetValues(enumDataType);

            int i = 0;
            EnumDataSource[] sources = new EnumDataSource[values.Length];
            foreach (object enumVal in values)
            {
                sources[i] = new EnumDataSource(enumDataType, enumVal, 
                    EnumUtils.GetDescription(enumDataType, enumVal));
                i++;
            }
            return sources;
        }

        #endregion
    }

    #endregion

    #region Enum Flag Manager

    /// <summary>
    /// Enum Flag Manager
    /// </summary>
    public class EnumFlagManager
    {
        #region Internal Variable

        private Type _enumType = null;
        private int _enumSize = 0;
        private BitArray bitArray = null;
        private ArrayList _enumArray = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public EnumFlagManager() { }
        /// <summary>
        /// Destructor
        /// </summary>
        ~EnumFlagManager()
        {
            ResetVars();
            _enumType = null;
        }

        #endregion

        #region Private Method

        private void ResetVars()
        {
            _enumSize = 0;
            if (_enumArray != null) _enumArray.Clear();
            _enumArray = null;
            bitArray = null;
        }

        private void AssignEnumType(Type type)
        {
            // reset
            ResetVars();
            _enumType = type;
            if (_enumType != null && _enumType.IsEnum)
            {
                Array array = Enum.GetValues(_enumType);
                if (array != null)
                {
                    _enumArray = new ArrayList(array);
                    _enumSize = array.Length;
                }
            }

            if (_enumSize > 0)
            {
                bitArray = new BitArray(_enumSize);
            }
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get Size of Enum Value
        /// </summary>
        [Description("Get Size of Enum Value")]
        [Browsable(false)]
        public int Size { get { return _enumSize; } }
        /// <summary>
        /// Get/Set Enum Type
        /// </summary>
        [Description("Get/Set Enum Type")]
        [Browsable(false)]
        public Type EnumType
        {
            get { return _enumType; }
            set
            {
                if (_enumType != value)
                {
                    if (value != null && !value.IsEnum) return;
                    AssignEnumType(value);
                }
            }
        }

        #endregion

        #region To and From String

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (bitArray == null) return "";
            string result = "";
            foreach (bool bitVal in bitArray)
            {
                result += (bitVal) ? "1" : "0";
            }
            return result;
        }
        /// <summary>
        /// FromString
        /// </summary>
        /// <param name="value"></param>
        public void FromString(string value)
        {
            if (bitArray == null) return;
            if (value.Length < 0)
            {
                bitArray.SetAll(false);
            }
            else
            {
                int index = 0;
                foreach (char ch in value)
                {
                    if (ch == '0') bitArray.Set(index, false);
                    else if (ch == '1') bitArray.Set(index, true);
                    else
                    {
                        // unchanged
                    }
                    index++;
                    if (index >= _enumSize) break; // out of range
                }
            }
        }

        #endregion

        #region Get Names and Descs

        /// <summary>
        /// Get On Flag's Names
        /// </summary>
        /// <returns></returns>
        public string[] GetOnFlagNames()
        {
            string[] results = new string[0];
            if (bitArray == null || _enumArray == null) return results;

            List<string> list = new List<string>();
            for (int i = 0; i < _enumSize; i++)
            {
                if (bitArray[i])
                {
                    list.Add(Enum.GetName(_enumType, _enumArray[i]));
                }
            }

            results = list.ToArray();
            list.Clear();
            list = null;

            return results;
        }
        /// <summary>
        /// Get Off Flag's Names
        /// </summary>
        /// <returns></returns>
        public string[] GetOffFlagNames()
        {
            string[] results = new string[0];
            if (bitArray == null || _enumArray == null) return results;

            List<string> list = new List<string>();
            for (int i = 0; i < _enumSize; i++)
            {
                if (!bitArray[i])
                {
                    list.Add(Enum.GetName(_enumType, _enumArray[i]));
                }
            }

            results = list.ToArray();
            list.Clear();
            list = null;

            return results;
        }

        #endregion

        #region Indexer Access

        /// <summary>
        /// Indexer Access
        /// </summary>
        public bool this[object enumValue]
        {
            get
            {
                if (bitArray == null ||
                    _enumArray == null || enumValue == null) return false;
                else
                {
                    int index = _enumArray.IndexOf(enumValue);
                    if (index >= 0 && index < _enumSize)
                        return bitArray[index];
                    else
                        return false;
                }
            }
            set
            {
                if (bitArray == null ||
                    _enumArray == null || enumValue == null) return;
                else
                {
                    int index = _enumArray.IndexOf(enumValue);
                    if (index >= 0 && index < _enumSize)
                        bitArray[index] = value;
                }
            }
        }

        #endregion
    }

    #endregion
}
