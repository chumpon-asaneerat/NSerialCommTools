#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-07-07
=================
- Data Access Framework - Schema access related classes updated.
  - Changed all log code used MethodBase.

======================================================================================================================
Update 2010-02-03
=================
- Schema access related classes ported from GFA37 to GFA38v3.
  - Add new delegate SchemaInfoConvertor<T>.
  - Add new class SchemaInfo.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

#endregion

namespace NLib.Data
{
    #region Enums for Provider Type information

    #region Null Support Mode

    /// <summary>
    /// Null value support enum
    /// </summary>
    public enum NullSupportMode
    {
        /// <summary>
        /// Null value is allow
        /// </summary>
        [Description("Null value is allow")]
        Allow,
        /// <summary>
        /// Null value is not allow
        /// </summary>
        [Description("Null value is not allow")]
        NotAllow,
        /// <summary>
        /// Unknown
        /// </summary>
        [Description("Unknown")]
        Unknown
    }

    #endregion

    #region Search Support Mode

    /// <summary>
    /// Search mode support enum
    /// </summary>
    public enum SearchSupportMode
    {
        /// <summary>
        /// Not Supprt Searching
        /// </summary>
        [Description("Not Supprt Searching")]
        NotSupportSearch,
        /// <summary>
        /// Support search in Like clause only
        /// </summary>
        [Description("Support search in Like clause only")]
        SupportSearchInLikeClauseOnly,
        /// <summary>
        /// Support search in all condition except in Like clause
        /// </summary>
        [Description("Support search in all condition except in Like clause")]
        SupportBasicSearch,
        /// <summary>
        /// Full searchable
        /// </summary>
        [Description("Full searchable")]
        Searchable
    }

    #endregion

    #endregion

    #region Schema Exception Delegate

    /// <summary>
    /// Delegate for Schema Exception
    /// </summary>
    public delegate void SchemaExceptionEventHandler(Exception ex);

    #endregion

    #region DbConnection Exception

    /// <summary>
    /// DbConnection Exception
    /// </summary>
    public class DbConnectionException : Exception
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public DbConnectionException() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception Message</param>
        public DbConnectionException(string message) : base(message) { }

        #endregion

        #region Static

        /// <summary>
        /// Create Connection Is Null Exception
        /// </summary>
        /// <returns></returns>
        public static DbConnectionException CreateConnectionIsNull()
        {
            return new DbConnectionException("Connection Instance Is Not Exists");
        }
        /// <summary>
        /// Create Connection Is Close Exception
        /// </summary>
        /// <returns></returns>
        public static DbConnectionException CreateConnectionIsClose()
        {
            return new DbConnectionException("Connection is not opened");
        }

        #endregion
    }

    #endregion

    #region NDbSchemaObject (abstract)

    /// <summary>
    /// NDbSchemaObject (abstract)
    /// </summary>
    public abstract class NDbSchemaObject
    {
        private bool _lock = false;
        /// <summary>
        /// Lock
        /// </summary>
        protected internal void Lock() { _lock = true; }
        /// <summary>
        /// Unlock
        /// </summary>
        protected internal void Unlock() { _lock = false; }
        /// <summary>
        /// Is object lock
        /// </summary>
        protected internal bool Locked { get { return _lock; } }
    }

    #endregion

    #region NDbMetaData

    /// <summary>
    /// NDbMetaData class.
    /// </summary>
    [Serializable]
    public class NDbMetaData : NDbSchemaObject
    {
        #region Internal Variable

        private string _collectionName = string.Empty;
        private int _noOfRestriction = 0;
        private int _noOfIdentifierParts = 0;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NDbMetaData()
            : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collectionName"></param>
        public NDbMetaData(string collectionName)
            : this()
        {
            this._collectionName = collectionName;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="noOfRestriction"></param>
        /// <param name="noOfIdentifierParts"></param>
        public NDbMetaData(string collectionName, int noOfRestriction, int noOfIdentifierParts)
            : this()
        {
            this._collectionName = collectionName;
            this._noOfRestriction = noOfRestriction;
            this._noOfIdentifierParts = noOfIdentifierParts;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NDbMetaData()
        {
            // Free Memory
            //GarbageCollector.FreeGC(this);
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get Information
        /// </summary>
        [Category("Information")]
        [Description("Get/Set Collection Name")]
        [XmlIgnore]
        public string Information
        {
            get { return string.Format("{0}, {1}", _collectionName, _noOfRestriction); }
        }
        /// <summary>
        /// Get/Set Collection Name
        /// </summary>
        [Category("Information")]
        [Description("Get/Set Collection Name")]
        [Browsable(true)]
        [XmlAttribute]
        public string CollectionName
        {
            get { return _collectionName; }
            set
            {
                if (Locked) return;
                if (_collectionName != value)
                {
                    _collectionName = value;
                }
            }
        }
        /// <summary>
        /// Get/Set No Of Restriction
        /// </summary>
        [Category("Information")]
        [Description("Get/Set No Of Restriction")]
        [Browsable(true)]
        [XmlAttribute]
        public int NoOfRestriction
        {
            get { return _noOfRestriction; }
            set
            {
                if (Locked) return;
                if (_noOfRestriction != value)
                {
                    _noOfRestriction = value;
                }
            }
        }
        /// <summary>
        /// Get/Set No Of Identifier Parts
        /// </summary>
        [Category("Information")]
        [Description("Get/Set No Of Identifier Parts")]
        [Browsable(true)]
        [XmlAttribute]
        public int NoOfIdentifierParts
        {
            get { return _noOfIdentifierParts; }
            set
            {
                if (Locked) return;
                if (_noOfIdentifierParts != value)
                {
                    _noOfIdentifierParts = value;
                }
            }
        }

        #endregion
    }

    #endregion

    #region NDbRestriction

    /// <summary>
    /// NDbRestriction class.
    /// </summary>
    [Serializable]
    public class NDbRestriction : NDbSchemaObject
    {
        #region Internal Variable

        private int _restrictionNumber = 0;
        private string _restrictionName = string.Empty;
        private string _value = string.Empty;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NDbRestriction()
            : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ordinal">Ordinal</param>
        /// <param name="restrictionName">Restriction Name</param>
        /// <param name="value">Restriction Value</param>
        public NDbRestriction(int ordinal, string restrictionName, string value)
            : this()
        {
            _restrictionNumber = ordinal;
            _restrictionName = restrictionName;
            _value = value;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NDbRestriction()
        {
            // Free Memory
            //GarbageCollector.FreeGC(this);
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Ordinal
        /// </summary>
        [Category("Information")]
        [Description("Get/Set Ordinal")]
        [Browsable(true)]
        [XmlAttribute]
        public int Ordinal
        {
            get { return _restrictionNumber; }
            set
            {
                if (Locked) return;
                if (_restrictionNumber != value)
                {
                    _restrictionNumber = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Restriction Name
        /// </summary>
        [Category("Information")]
        [Description("Get/Set Restriction Name")]
        [Browsable(true)]
        [XmlAttribute]
        public string RestrictionName
        {
            get { return _restrictionName; }
            set
            {
                if (Locked) return;
                if (_restrictionName != value)
                {
                    _restrictionName = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Restriction Value
        /// </summary>
        [Category("Information")]
        [Description("Get/Set Restriction Value")]
        [Browsable(true)]
        [XmlAttribute]
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                }
            }
        }

        #endregion
    }

    #endregion

    #region NDbProviderDataType

    /// <summary>
    /// NDbProviderDataType Class that contain DataType information that retrived from database's driver like ODBC and OLEDB Provider
    /// this class is support for structure of ODBC 2.0
    /// </summary>
    [Serializable]
    public class NDbProviderDataType : NDbSchemaObject
    {
        #region Internal Variable

        private string _typeName = string.Empty;

        private int _providerDbType = -1;
        private decimal _columnSize = 0;

        private string _createFormat = "";
        private string _createParams = "";

        private string _dataType = "";
        private bool _autoUniqueValue = false;
        private bool _bestMatch = false;
        private bool _caseSensitive = false;
        private bool _isFixedLength = false;
        private bool _fixprec = true;
        private bool _isLong = false;

        private NullSupportMode _nullable = NullSupportMode.Allow;
        private SearchSupportMode _searchable = SearchSupportMode.Searchable;

        private bool _unsignedattribute = true;

        private int _minScale = 0;
        private int _maxScale = 0;

        private bool _isConcurrencyType = false;
        private bool _isliteralSupport = false;

        private string _literalPrefix = "";
        private string _literalSuffix = "";

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NDbProviderDataType() : base() { }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NDbProviderDataType()
        {
            // Free Memory
            //GarbageCollector.FreeGC(this);
        }

        #endregion

        #region Public Property

        #region TypeName

        /// <summary>
        /// Get/Set TypeName. 
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///		Provider-specific data type name.
        ///			</para>
        ///			</description>
        ///		</item>
        ///		<item>
        ///			<term>ODBC Note.</term>
        ///			<description>
        ///			<para>
        ///		This property is Data source–dependent data-type name.
        ///		for example, "CHAR()", "VARCHAR()", "MONEY", "LONG VARBINARY", or "CHAR ( ) FOR BIT DATA".
        ///		Applications must use this name in CREATE TABLE and ALTER TABLE statements.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set TypeName.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(1)]
        public string TypeName
        {
            get { return _typeName; }
            set
            {
                if (Locked) return;
                if (_typeName != value)
                {
                    _typeName = value;
                }
            }
        }

        #endregion

        #region ProviderDbType

        /// <summary>
        /// Get/Set Provider DbType ID (-1 for unknown)
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///		The indicator of the data type
        ///			</para>
        ///			</description>
        ///		</item>
        ///		<item>
        ///			<term>ODBC Note.</term>
        ///			<description>
        ///			<para>
        ///		SQL data type. This can be an ODBC SQL data type or a driver-specific SQL data type.
        ///		For datetime or interval data types, this column returns the concise data type 
        ///		(such as SQL_TYPE_TIME or SQL_INTERVAL_YEAR_TO_MONTH). 
        ///		For a list of valid ODBC SQL data types, see SQL Data Types in ODBC Reference Appendix D: Data Types. 
        ///		For information about driver-specific SQL data types, see the ODBC Reference driver’s documentation.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Provider DbType ID")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(2)]
        public int ProviderDbType
        {
            get { return _providerDbType; }
            set
            {
                if (Locked) return;
                if (_providerDbType != value)
                {
                    _providerDbType = value;
                }
            }
        }

        #endregion

        #region ColumnSize

        /// <summary>
        /// Get/Set Column Size (maximum). 
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///		The length of a non-numeric column or parameter refers to either the maximum or the length 
        ///		defined for this type by the provider. For character data, this is the maximum or defined length 
        ///		in characters. For datetime data types, this is the length of the string representation 
        ///		(assuming the maximum allowed precision of the fractional seconds component). 
        ///		If the data type is numeric, this is the upper bound on the maximum precision of the data type. 
        ///		For the maximum precision of all numeric data types, 
        ///		see "Precision of Numeric Data Types" in OLEDB API Reference Appendix A.
        ///			</para>
        ///			</description>
        ///		</item>
        ///		<item>
        ///			<term>ODBC Note.</term>
        ///			<description>
        ///			<para>
        ///		if provider return result is null 0 will be used insteed.
        ///		The maximum column size that the server supports for this data type. 
        ///		For numeric data, this is the maximum precision. For string data, 
        ///		this is the length in characters. For datetime data types, this is the length in characters 
        ///		of the string representation (assuming the maximum allowed precision of the fractional seconds 
        ///		component). NULL is returned for data types where column size is not applicable. 
        ///		For interval data types, this is the number of characters in the character representation of 
        ///		the interval literal (as defined by the interval leading precision; see Interval Data Type Length" 
        ///		in ODBC Reference Appendix D: Data Types). 
        ///		For more information on column size, see Column Size, Decimal Digits, Transfer Octet Length, 
        ///		and Display Size in ODBC Reference Appendix D: Data Types.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Column Size (maximum)")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(3)]
        public decimal ColumnSize
        {
            get { return _columnSize; }
            set
            {
                if (Locked) return;
                if (_columnSize != value)
                {
                    _columnSize = value;
                }
            }
        }

        #endregion

        #region CreateFormat

        /// <summary>
        /// Get/Set Format that used for Type create or alter.
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Format that used for Type create or alter.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(4)]
        public string CreateFormat
        {
            get { return _createFormat; }
            set
            {
                if (Locked) return;
                if (_createFormat != value)
                {
                    _createFormat = value;
                }
            }
        }

        #endregion

        #region CreateParameters

        /// <summary>
        /// Get/Set Paramaters that used for Type create or alter.
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///		The creation parameters are specified by the consumer when creating a column of this data type. 
        ///		For example, the SQL data type DECIMAL needs a precision and a scale. In this case, 
        ///		the creation parameters might be the string "precision,scale". In a text command to create a 
        ///		DECIMAL column with a precision of 10 and a scale of 2, the value of the 
        ///		TYPE_NAME column might be DECIMAL() and the complete type specification would be DECIMAL(10,2). 
        ///		The creation parameters appear as a comma-separated list of values, in the order they are to 
        ///		be supplied and with no surrounding parentheses. If a creation parameter is length, 
        ///		maximum length, precision, scale, seed, or increment "length", "max length", "precision", "scale", 
        ///		"seed", and "increment" should be used, respectively. 
        ///		If the creation parameters are some other value, it is provider-specific what text is used 
        ///		to describe the creation parameter. If the data type requires creation parameters, "()" 
        ///		usually appears in the type name. This indicates the position at which to insert the 
        ///		creation parameters. If the type name does not include "()", the creation parameters are enclosed 
        ///		in parentheses and appended to the data type name.
        ///			</para>
        ///			</description>
        ///		</item>
        ///		<item>
        ///			<term>ODBC Note.</term>
        ///			<description>
        ///			<para>
        ///		A list of keywords, separated by commas, corresponding to each parameter that the application 
        ///		may specify in parentheses when using the name that is returned in the TYPE_NAME field. 
        ///		The keywords in the list can be any of the following: length, precision, or scale. 
        ///		They appear in the order that the syntax requires them to be used. 
        ///		For example, 
        ///			CREATE_PARAMS for DECIMAL would be "precision,scale"; 
        ///			CREATE_PARAMS for VARCHAR would equal "length." 
        ///			NULL is returned if there are no parameters for the data type definition; for example, INTEGER. 
        ///		The driver supplies the CREATE_PARAMS text in the language of the country/region where it is used.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Paramaters that used for Type create or alter")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(5)]
        public string CreateParameters
        {
            get { return _createParams; }
            set
            {
                if (Locked) return;
                if (_createParams != value)
                {
                    _createParams = value;
                }
            }
        }

        #endregion

        #region DataType

        /// <summary>
        /// Get/Set .NET data type.
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set .NET data type.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(6)]
        public string DataType
        {
            get { return _dataType; }
            set
            {
                if (Locked) return;
                if (_dataType != value)
                {
                    _dataType = value;
                }
            }
        }

        #endregion

        #region IsAutoIncrementable

        /// <summary>
        /// Get/Set Is Auto Incrementable.
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///			VARIANT_TRUE—Values of this type can be autoincrementing. 
        ///			VARIANT_FALSE—Values of this type cannot be autoincrementing.
        ///			Note.
        ///			If this value is VARIANT_TRUE, whether or not a column of this type is always autoincrementing 
        ///			depends on the provider's DBPROP_COL_AUTOINCREMENT column property. 
        ///			If the DBPROP_COL_AUTOINCREMENT property is read/write, whether or not a column of this type 
        ///			is autoincrementing depends on the setting of the DBPROP_COL_AUTOINCREMENT property. 
        ///			If DBPROP_COL_AUTOINCREMENT is a read-only property, either all or none of the columns of 
        ///			this type are autoincrementing.
        ///			</para>
        ///			</description>
        ///		</item>
        ///		<item>
        ///			<term>ODBC Note.</term>
        ///			<description>
        ///			<para>
        ///		Whether the data type is autoincrementing: 
        ///			SQL_TRUE if the data type is autoincrementing.
        ///			SQL_FALSE if the data type is not autoincrementing.
        ///			NULL is returned if the attribute is not applicable to the data type or 
        ///			the data type is not numeric. An application can insert values into a column having 
        ///			this attribute, but typically cannot update the values in the column. 
        ///			When an insert is made into an auto-increment column, a unique value is inserted into 
        ///			the column at insert time. The increment is not defined, but is data source–specific. 
        ///			An application should not assume that an auto-increment column starts at 
        ///			any particular point or increments by any particular value.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Is Auto Incrementable.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(7)]
        public bool IsAutoIncrementable
        {
            get { return _autoUniqueValue; }
            set
            {
                if (Locked) return;
                if (_autoUniqueValue != value)
                {
                    _autoUniqueValue = value;
                }
            }
        }

        #endregion

        #region IsBestMatch

        /// <summary>
        /// Get/Set Is Best Match Type. false if unknown result found.
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///		VARIANT_TRUE — The data type is the best match between all data types in the data store 
        ///			and the OLE DB data type indicated by the value in the DATA_TYPE column. 
        ///		VARIANT_FALSE — The data type is not the best match.
        ///		
        ///		For each set of rows in which the value of the DATA_TYPE column is the same, 
        ///		the BEST_MATCH column is set to VARIANT_TRUE in only one row.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Is Best Match Type")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(8)]
        public bool IsBestMatch
        {
            get { return _bestMatch; }
            set
            {
                if (Locked) return;
                if (_bestMatch != value)
                {
                    _bestMatch = value;
                }
            }
        }

        #endregion

        #region IsCaseSensitive

        /// <summary>
        /// Get/Set is character data type is case-sensitive in collations and comparisons
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///		Return value should be
        ///			VARIANT_TRUE—The data type is a character type and is case-sensitive. 
        ///			VARIANT_FALSE—The data type is not a character type or is not case-sensitive.
        ///			</para>
        ///			</description>
        ///		</item>
        ///		<item>
        ///			<term>ODBC Note.</term>
        ///			<description>
        ///			<para>
        ///		Whether a character data type is case-sensitive in collations and comparisons: 
        ///			SQL_TRUE if the data type is a character data type and is case-sensitive.
        ///			SQL_FALSE if the data type is not a character data type or is not case-sensitive.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set is character data type is case-sensitive in collations and comparisons")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(9)]
        public bool IsCaseSensitive
        {
            get { return _caseSensitive; }
            set
            {
                if (Locked) return;
                if (_caseSensitive != value)
                {
                    _caseSensitive = value;
                }
            }
        }

        #endregion

        #region IsFixedLength

        /// <summary>
        /// Get/Set Is Fixed Length data type
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///		VARIANT_TRUE — Columns of this type created by the data definition language (DDL) 
        ///			will be of fixed length. 
        ///		VARIANT_FALSE — Columns of this type created by the DDL will be of variable length.
        ///		
        ///		If the field is NULL, it is not known whether the provider will map this field with a 
        ///		fixed-length or variable-length column.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Is Fixed Length data type")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(10)]
        public bool IsFixedLength
        {
            get { return _isFixedLength; }
            set
            {
                if (Locked) return;
                if (_isFixedLength != value)
                {
                    _isFixedLength = value;
                }
            }
        }

        #endregion

        #region Is Fixed Precision-Scale

        /// <summary>
        /// Get/Set is data type is fixed precision-scale
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///		Whether the data type has predefined fixed precision and scale
        ///			VARIANT_TRUE — The data type has a fixed precision and scale. 
        ///			VARIANT_FALSE—The data type does not have a fixed precision and scale.
        ///			</para>
        ///			</description>
        ///		</item>
        ///		<item>
        ///			<term>ODBC Note.</term>
        ///			<description>
        ///			<para>
        ///		Whether the data type has predefined fixed precision and scale (which are data source–specific), 
        ///		such as a money data type: 
        ///			SQL_TRUE if it has predefined fixed precision and scale.
        ///			SQL_FALSE if it does not have predefined fixed precision and scale.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set is data type is fixed precision-scale")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(11)]
        public bool IsFixedPrecisionScale
        {
            get { return _fixprec; }
            set
            {
                if (Locked) return;
                if (_fixprec != value)
                {
                    _fixprec = value;
                }
            }
        }

        #endregion

        #region IsLong

        /// <summary>
        /// Get/Set Is Long (binary) Data Type
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///		VARIANT_TRUE — The data type is a BLOB that contains very long data; the definition of 
        ///			very long data is provider-specific. 
        ///		VARIANT_FALSE — The data type is a BLOB that does not contain very long data or is not a BLOB.
        ///		
        ///		This value determines the setting of the DBCOLUMNFLAGS_ISLONG flag returned by 
        ///		GetColumnInfo in IColumnsInfo and GetParameterInfo in ICommandWithParameters. 
        ///		For more information, see GetColumnInfo, GetParameterInfo, 
        ///		and "Accessing BLOB Data" in Chapter 7: Blobs and COM Objects.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Is Long (binary) Data Type")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(12)]
        public bool IsLong
        {
            get { return _isLong; }
            set
            {
                if (Locked) return;
                if (_isLong != value)
                {
                    _isLong = value;
                }
            }
        }

        #endregion

        #region Nullable

        /// <summary>
        /// Get/Set Type is Null support.
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///		Return value should be
        ///			VARIANT_TRUE — The data type is nullable. 
        ///			VARIANT_FALSE — The data type is not nullable.
        ///			NULL — It is not known whether the data type is nullable.
        ///			</para>
        ///			</description>
        ///		</item>
        ///		<item>
        ///			<term>ODBC Note.</term>
        ///			<description>
        ///			<para>
        ///		Whether the data type accepts a NULL value: 
        ///			SQL_NO_NULLS if the data type does not accept NULL values.
        ///			SQL_NULLABLE if the data type accepts NULL values.
        ///			SQL_NULLABLE_UNKNOWN if it is not known whether the column accepts NULL values.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Type is Null support")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(13)]
        public NullSupportMode Nullable
        {
            get { return _nullable; }
            set
            {
                if (Locked) return;
                if (_nullable != value)
                {
                    _nullable = value;
                }
            }
        }

        #endregion

        #region Searchable

        /// <summary>
        /// Get/Set  how the data type is used in a WHERE clause
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///		If the provider supports ICommandText, this column is an integer indicating the searchability of a data type; otherwise, this column is NULL. One of the following: 
        ///			DB_UNSEARCHABLE — The data type cannot be used in a WHERE clause.
        ///			DB_LIKE_ONLY — The data type can be used in a WHERE clause only with the LIKE predicate.
        ///			DB_ALL_EXCEPT_LIKE — The data type can be used in a WHERE clause with all comparison operators except LIKE.
        ///			DB_SEARCHABLE — The data type can be used in a WHERE clause with any comparison operator.
        ///			</para>
        ///			</description>
        ///		</item>
        ///		<item>
        ///			<term>ODBC Note.</term>
        ///			<description>
        ///			<para>
        ///		How the data type is used in a WHERE clause: 
        ///			SQL_PRED_NONE if the column cannot be used in a WHERE clause. 
        ///				(This is the same as the SQL_UNSEARCHABLE value in ODBC 2.x.)
        ///			SQL_PRED_CHAR if the column can be used in a WHERE clause, 
        ///				but only with the LIKE predicate. (This is the same as the SQL_LIKE_ONLY value in ODBC 2.x.)
        ///			SQL_PRED_BASIC if the column can be used in a WHERE clause with all the comparison operators 
        ///				except LIKE (comparison, quantified comparison, BETWEEN, DISTINCT, IN, MATCH, and UNIQUE). 
        ///				(This is the same as the SQL_ALL_EXCEPT_LIKE value in ODBC 2.x.)
        ///			SQL_SEARCHABLE if the column can be used in a WHERE clause with any comparison operator.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set  how the data type is used in a WHERE clause")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(14)]
        public SearchSupportMode Searchable
        {
            get { return _searchable; }
            set
            {
                if (Locked) return;
                if (_searchable != value)
                {
                    _searchable = value;
                }
            }
        }

        #endregion

        #region IsUnsigned

        /// <summary>
        /// Get/Set is data type is unsigned data type (for numeric data type only the other data type alway 
        /// set to true)
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///		Whether the data type is unsigned: 
        ///			VARIANT_TRUE—The data type is unsigned. 
        ///			VARIANT_FALSE—The data type is signed.
        ///			NULL—Not applicable to data type.
        ///			</para>
        ///			</description>
        ///		</item>
        ///		<item>
        ///			<term>ODBC Note.</term>
        ///			<description>
        ///			<para>
        ///		Whether the data type is unsigned: 
        ///			SQL_TRUE if the data type is unsigned.
        ///			SQL_FALSE if the data type is signed.
        ///			NULL is returned if the attribute is not applicable to the data type or 
        ///			the data type is not numeric.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set is data type is unsigned data type (for numeric data type only the other data type alway set to true)")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(15)]
        public bool IsUnsigned
        {
            get { return _unsignedattribute; }
            set
            {
                if (Locked) return;
                if (_unsignedattribute != value)
                {
                    _unsignedattribute = value;
                }
            }
        }

        #endregion

        #region Minimum and Maximum Scale

        /// <summary>
        /// Get/Set Minimum Scale (-1 for data type that not support scale)
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///	If the type indicator is DBTYPE_VARNUMERIC, DBTYPE_DECIMAL, or DBTYPE_NUMERIC, this is the 
        ///	minimum number of digits allowed to the right of the decimal point. Otherwise, this is NULL.
        ///			</para>
        ///			</description>
        ///		</item>
        ///		<item>
        ///			<term>ODBC Note.</term>
        ///			<description>
        ///			<para>
        ///	The minimum scale of the data type on the data source. If a data type has a fixed scale, 
        ///	the MINIMUM_SCALE and MAXIMUM_SCALE columns both contain this value. For example, 
        ///	an SQL_TYPE_TIMESTAMP column might have a fixed scale for fractional seconds. 
        ///	NULL is returned where scale is not applicable. For more information, see Column Size, Decimal Digits, 
        ///	Transfer Octet Length, and Display Size in Appendix D: Data Types.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Minimum Scale (-1 for data type that not support scale)")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(16)]
        public int MinimumScale
        {
            get { return _minScale; }
            set
            {
                if (Locked) return;
                if (_minScale != value)
                {
                    _minScale = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Maximum Scale (-1 for data type that not support scale)
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///	If the type indicator is DBTYPE_VARNUMERIC, DBTYPE_DECIMAL, or DBTYPE_NUMERIC, this is the 
        ///	maximum number of digits allowed to the right of the decimal point. Otherwise, this is NULL.
        ///			</para>
        ///			</description>
        ///		</item>
        ///		<item>
        ///			<term>ODBC Note.</term>
        ///			<description>
        ///			<para>
        ///	The maximum scale of the data type on the data source. 
        ///	NULL is returned where scale is not applicable. If the maximum scale is not defined separately 
        ///	on the data source, but is instead defined to be the same as the maximum precision, 
        ///	this column contains the same value as the COLUMN_SIZE column. For more information, 
        ///	see Column Size, Decimal Digits, Transfer Octet Length, and Display Size in Appendix D: Data Types.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Maximum Scale (-1 for data type that not support scale)")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(17)]
        public int MaximumScale
        {
            get { return _maxScale; }
            set
            {
                if (Locked) return;
                if (_maxScale != value)
                {
                    _maxScale = value;
                }
            }
        }
        /// <summary>
        /// Get whether that data type is support scale
        /// </summary>
        [Browsable(false)]
        [Category("Information")]
        [Description("Get whether that data type is support scale")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlIgnore]
        public bool IsSupportScale
        {
            get
            {
                return (_minScale != -1) && (_maxScale != -1);
            }
        }

        #endregion

        #region IsConcurrencyType

        /// <summary>
        /// Get/Set is data type is Concurrency Type.
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set is data type is Concurrency Type.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(18)]
        public bool IsConcurrencyType
        {
            get { return _isConcurrencyType; }
            set
            {
                if (Locked) return;
                if (_isConcurrencyType != value)
                {
                    _isConcurrencyType = value;
                }
            }
        }

        #endregion

        #region IsLiteralSupported

        /// <summary>
        /// Get/Set is data type is Support Literal.
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set is data type is Support Literal.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(19)]
        public bool IsLiteralSupported
        {
            get { return _isliteralSupport; }
            set
            {
                if (Locked) return;
                if (_isliteralSupport != value)
                {
                    _isliteralSupport = value;
                }
            }
        }

        #endregion

        #region Literal Prefix

        /// <summary>
        /// Get/Set Literal Prefix that used for datatype.
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///		Character or characters used to prefix a literal of this type in a text command.
        ///			</para>
        ///			</description>
        ///		</item>
        ///		<item>
        ///			<term>ODBC Note.</term>
        ///			<description>
        ///			<para>
        ///		Character or characters used to prefix a literal; 
        ///		for example, a single quotation mark (') for character data types or 0x for binary data types; 
        ///		NULL is returned for data types where a literal prefix is not applicable.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Literal Prefix")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(20)]
        public string LiteralPrefix
        {
            get { return _literalPrefix; }
            set
            {
                if (Locked) return;
                if (_literalPrefix != value)
                {
                    _literalPrefix = value;
                }
            }
        }

        #endregion

        #region Literal Suffix

        /// <summary>
        /// Get/Set Literal Suffix that used for datatype.
        /// <list type="table">
        ///		<item>
        ///			<term>OLEDB Note.</term>
        ///			<description>
        ///			<para>
        ///		Character or characters used to suffix a literal of this type in a text command.
        ///			</para>
        ///			</description>
        ///		</item>
        ///		<item>
        ///			<term>ODBC Note.</term>
        ///			<description>
        ///			<para>
        ///		Character or characters used to terminate a literal; for example, a single quotation mark (') 
        ///		for character data types; NULL is returned for data types where a literal suffix is not applicable.
        ///			</para>
        ///			</description>
        ///		</item>
        /// </list>
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Literal Suffix")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(21)]
        public string LiteralSuffix
        {
            get { return _literalSuffix; }
            set
            {
                if (Locked) return;
                if (_literalSuffix != value)
                {
                    _literalSuffix = value;
                }
            }
        }

        #endregion

        #endregion
    }

    #endregion

    #region NDbReservedword

    /// <summary>
    /// NDbReservedword Class
    /// </summary>
    [Serializable]
    public class NDbReservedword : NDbSchemaObject
    {
        #region Internal Variable

        private string _keyword = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NDbReservedword() : base() { }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NDbReservedword()
        {
            // Free Memory
            //GarbageCollector.FreeGC(this);
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Reserved word String
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Reserved word String")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(1)]
        public string Reservedword
        {
            get { return _keyword; }
            set
            {
                if (Locked) return;
                if (_keyword != value)
                {
                    _keyword = value;
                }
            }
        }

        #endregion
    }

    #endregion

    #region NDbTable

    /// <summary>
    /// NDbTable Class.
    /// </summary>
    [Serializable]
    public class NDbTable : NDbSchemaObject
    {
        #region Internal Variable

        private string _tableName;
        private string _providerTableType = string.Empty;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NDbTable()
            : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="providerTableType">Provider TableType</param>
        public NDbTable(string tableName, string providerTableType)
            : this()
        {
            this._tableName = tableName;
            this._providerTableType = providerTableType;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NDbTable()
        {
            // Free Memory
            //GarbageCollector.FreeGC(this);
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Table Name
        /// </summary>
        [Category("Information")]
        [Description("Get/Set Table Name")]
        [Browsable(true)]
        [XmlAttribute]
        public string TableName
        {
            get { return _tableName; }
            set
            {
                if (Locked) return;
                if (_tableName != value)
                {
                    _tableName = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Provider TableType
        /// </summary>
        [Category("Information")]
        [Description("Get/Set Provider TableType")]
        [Browsable(true)]
        [XmlAttribute]
        public string ProviderTableType
        {
            get { return _providerTableType; }
            set
            {
                if (Locked) return;
                if (_providerTableType != value)
                {
                    _providerTableType = value;
                }
            }
        }

        #endregion
    }

    #endregion

    #region NDbColumn

    /// <summary>
    /// NDbColumn Class. wrapper the result from GetSchemaTable()
    /// </summary>
    [Serializable]
    public class NDbColumn : NDbSchemaObject
    {
        #region Internal Variable

        private string _columnName = string.Empty;

        private string _baseServerName = string.Empty;
        private string _baseCatalogName = string.Empty;
        private string _baseSchemaName = string.Empty;
        private string _baseTableName = string.Empty;
        private string _baseColumnName = string.Empty;

        private int _ordinal = 0;
        private int _columnSize = 0;
        private int _numericPrecision = 0;
        private int _numericScale = 0;

        private bool _isUnique = false;
        private bool _isKey = false;
        private bool _allowDbNull = true;
        private bool _isAliased = false;
        private bool _isExpression = false;
        private bool _isIdentity = false;
        private bool _isAutoIncrement = false;
        private bool _isRowVersion = false;
        private bool _isHidden = false;
        private bool _isLong = false;
        private bool _isReadOnly = false;

        private int _providerTypeId = 0;
        private Type _dataType = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NDbColumn() : base() { }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NDbColumn()
        {
            // Free Memory
            //GarbageCollector.FreeGC(this);
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Column name
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Column name")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(1)]
        public string ColumnName
        {
            get { return _columnName; }
            set
            {
                if (this.Locked) return;
                if (_columnName != value)
                {
                    _columnName = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Base ServerName
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Base ServerName")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(2)]
        public string BaseServerName
        {
            get { return _baseServerName; }
            set
            {
                if (this.Locked) return;
                if (_baseServerName != value)
                {
                    _baseServerName = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Base Catalog name. NULL if the provider does not support catalogs.
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Base Catalog name")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(3)]
        public string BaseCatalogName
        {
            get { return _baseCatalogName; }
            set
            {
                if (this.Locked) return;
                if (_baseCatalogName != value)
                {
                    _baseCatalogName = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Base Unqualified schema name. NULL if the provider does not support schemas.
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Base Unqualified schema name")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(4)]
        public string BaseSchemaName
        {
            get { return _baseSchemaName; }
            set
            {
                if (this.Locked) return;
                if (_baseSchemaName != value)
                {
                    _baseSchemaName = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Base Table Name
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Base Table name")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(5)]
        public string BaseTableName
        {
            get { return _baseTableName; }
            set
            {
                if (this.Locked) return;
                if (_baseTableName != value)
                {
                    _baseTableName = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Base Column name
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Base Column name")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(6)]
        public string BaseColumnName
        {
            get { return _baseColumnName; }
            set
            {
                if (this.Locked) return;
                if (_baseColumnName != value)
                {
                    _baseColumnName = value;
                }
            }
        }
        /// <summary>
        /// Get/Set order of the column
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set order of the column")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(7)]
        public int Ordinal
        {
            get { return _ordinal; }
            set
            {
                if (this.Locked) return;
                if (_ordinal != value)
                {
                    _ordinal = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Column Size
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Column Size")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(8)]
        public int ColumnSize
        {
            get { return _columnSize; }
            set
            {
                if (this.Locked) return;
                if (_columnSize != value)
                {
                    _columnSize = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Numeric Precision.
        /// If the column's data type is of a numeric data type other than VARNUMERIC, 
        /// this is the maximum precision of the column. The precision of columns with a data 
        /// type of DBTYPE_DECIMAL or DBTYPE_NUMERIC depends on the definition of the column. 
        /// For the precision of all other numeric data types.
        /// If the column's data type is not numeric or is VARNUMERIC, this is NULL
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Numeric Precision")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(9)]
        public int NumericPrecision
        {
            get { return _numericPrecision; }
            set
            {
                if (this.Locked) return;
                if (_numericPrecision != value)
                {
                    _numericPrecision = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Numeric Scale.
        /// If the column's type indicator is DBTYPE_DECIMAL, DBTYPE_NUMERIC, or DBTYPE_VARNUMERIC, 
        /// this is the number of digits to the right of the decimal point. Otherwise, this is NULL.
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set Numeric Scale")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(10)]
        public int NumericScale
        {
            get { return _numericScale; }
            set
            {
                if (this.Locked) return;
                if (_numericScale != value)
                {
                    _numericScale = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column is unique
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set column is unique")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(11)]
        public bool IsUnique
        {
            get { return _isUnique; }
            set
            {
                if (this.Locked) return;
                if (_isUnique != value)
                {
                    _isUnique = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column is Key
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set column is Key")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(12)]
        public bool IsKey
        {
            get { return _isKey; }
            set
            {
                if (this.Locked) return;
                if (_isKey != value)
                {
                    _isKey = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column can contain null value
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set is column can contain null value")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(13)]
        public bool AllowDbNull
        {
            get { return _allowDbNull; }
            set
            {
                if (this.Locked) return;
                if (_allowDbNull != value)
                {
                    _allowDbNull = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column is aliased
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set column is aliased")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(14)]
        public bool IsAliased
        {
            get { return _isAliased; }
            set
            {
                if (this.Locked) return;
                if (_isAliased != value)
                {
                    _isAliased = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column is expression
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set column is expression")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(15)]
        public bool IsExpression
        {
            get { return _isExpression; }
            set
            {
                if (this.Locked) return;
                if (_isExpression != value)
                {
                    _isExpression = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column is identity column
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set column is identity column")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(16)]
        public bool IsIdentity
        {
            get { return _isIdentity; }
            set
            {
                if (this.Locked) return;
                if (_isIdentity != value)
                {
                    _isIdentity = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column is auto increment
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set column is auto increment")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(17)]
        public bool IsAutoIncrement
        {
            get { return _isAutoIncrement; }
            set
            {
                if (this.Locked) return;
                if (_isAutoIncrement != value)
                {
                    _isAutoIncrement = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column is row version
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set column is row version")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(18)]
        public bool IsRowVersion
        {
            get { return _isRowVersion; }
            set
            {
                if (this.Locked) return;
                if (_isRowVersion != value)
                {
                    _isRowVersion = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column is hidden column
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set column is hidden column")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(19)]
        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                if (this.Locked) return;
                if (_isHidden != value)
                {
                    _isHidden = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column is long datatype
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set column is long datatype")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(20)]
        public bool IsLong
        {
            get { return _isLong; }
            set
            {
                if (this.Locked) return;
                if (_isLong != value)
                {
                    _isLong = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column is readonly
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set column is readonly")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(21)]
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                if (this.Locked) return;
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column Provider's Type id
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set column Provider's Type id")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(22)]
        public int ProviderTypeID
        {
            get { return _providerTypeId; }
            set
            {
                if (this.Locked) return;
                if (_providerTypeId != value)
                {
                    _providerTypeId = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column DataType (.NET type)
        /// </summary>
        [Browsable(false)]
        [Category("Information")]
        [Description("Get/Set column DataType (.NET type)")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlIgnore]
        [NLib.Design.PropertyOrder(23)]
        public Type DataType
        {
            get { return _dataType; }
            set
            {
                if (this.Locked) return;
                if (_dataType != value)
                {
                    _dataType = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column DataType (.NET type) FullName
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set .NET Type's Name Of Object FullName")]
        [RefreshProperties(RefreshProperties.Repaint)]
        //[XmlIgnore]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(23)]
        public string DataTypeName
        {
            get
            {
                if (_dataType != null) return _dataType.FullName;
                else
                    return "";
            }
            set
            {
                if (this.Locked) return;
                if (value == null || value == string.Empty) _dataType = null;
                else DataType = NLib.Utils.NETTypeCache.Instance.FindType(value);
            }
        }

        #endregion
    }

    #endregion

    #region NSchemaInfo

    /// <summary>
    /// Schema Info Convertor delegate.
    /// </summary>
    /// <typeparam name="T">The target object type.</typeparam>
    /// <param name="row">The DataRow that contains metadata attribute.</param>
    /// <param name="info">The schema info that own the convertor.</param>
    /// <param name="factory">The caller connection factory.</param>
    /// <returns>Returns new instance of object that convert from DataRow.</returns>
    public delegate T NSchemaInfoConvertor<T>(DataRow row, NSchemaInfo<T> info, NDbFactory factory)
        where T : NDbSchemaObject;
    /// <summary>
    /// Schema Info
    /// </summary>
    /// <typeparam name="T">The target object type.</typeparam>
    public class NSchemaInfo<T>
        where T : NDbSchemaObject
    {
        #region Internal Variable

        private NDbMetaData _metadata = null;
        private NDbRestriction[] _restrictions = null;
        private NSchemaInfoConvertor<T> _converter;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NSchemaInfo()
            : base()
        {
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NSchemaInfo()
        {
            _converter = null;
            _metadata = null;
            _restrictions = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets meta data.
        /// </summary>
        public NDbMetaData MetaData
        {
            get { return _metadata; }
            set { _metadata = value; }
        }
        /// <summary>
        /// Gets or sets restriction for the collection.
        /// </summary>
        public NDbRestriction[] Restrictions
        {
            get { return _restrictions; }
            set { _restrictions = value; }
        }
        /// <summary>
        /// Gets or sets convert callback function to convert from DataRow to target object.
        /// </summary>
        public NSchemaInfoConvertor<T> Convert
        {
            get { return _converter; }
            set { _converter = value; }
        }

        #endregion
    }

    #endregion

    #region NProcedureSchemaInfo

    /// <summary>
    /// NProcedureSchemaInfo class.
    /// </summary>
    public class NProcedureSchemaInfo
    {
        /// <summary>
        /// Gets or sets the Restrictions for Procedures Metadata collection.
        /// </summary>
        public NDbRestriction[] Restrictions { get; set; }
        /// <summary>
        /// Gets or sets the Column's Name in result table that used for identify the
        /// procedure's name.
        /// </summary>
        public string ProcedureNameColumn { get; set; }
    }

    #endregion

    #region NDbProcedureParameterInfo

    /// <summary>
    /// NDbProcedureParameterInfo class.
    /// </summary>
    [Serializable]
    public class NDbProcedureParameterInfo : NDbSchemaObject
    {
        #region Internal Variable

        private string _parameterName = string.Empty;
        private Type _dataType = null;
        private ParameterDirection _direction = ParameterDirection.InputOutput;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NDbProcedureParameterInfo()
            : base()
        {
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parameterName">Parameter Name.</param>
        /// <param name="dataType">Parameter Type.</param>
        /// <param name="direction">Parameter Direction.</param>
        public NDbProcedureParameterInfo(string parameterName,
            Type dataType, ParameterDirection direction)
            : this()
        {
            this._parameterName = parameterName;
            this._dataType = dataType;
            this._direction = direction;
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~NDbProcedureParameterInfo()
        {
            _dataType = null;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Parameter Name
        /// </summary>
        [Category("Information")]
        [Description("Get/Set Parameter Name")]
        [Browsable(true)]
        [XmlAttribute]
        public string ParameterName
        {
            get { return _parameterName; }
            set
            {
                if (Locked) return;
                if (_parameterName != value)
                {
                    _parameterName = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Parameter Direction.
        /// </summary>
        [Category("Information")]
        [Description("Get/Set Parameter Direction")]
        [Browsable(true)]
        [XmlAttribute]
        public ParameterDirection Direction
        {
            get { return _direction; }
            set
            {
                if (Locked) return;
                if (_direction != value)
                {
                    _direction = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Data Type
        /// </summary>
        [Category("Information")]
        [Description("Get/Set Data Type")]
        [Browsable(true)]
        [XmlIgnore]
        public Type DataType
        {
            get { return _dataType; }
            set
            {
                if (Locked) return;
                if (_dataType != value)
                {
                    _dataType = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column DataType (.NET type) FullName
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set .NET Type's Name Of Object FullName")]
        [RefreshProperties(RefreshProperties.Repaint)]
        //[XmlIgnore]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(23)]
        public string DataTypeName
        {
            get
            {
                if (_dataType != null) return _dataType.FullName;
                else
                    return "";
            }
            set
            {
                if (this.Locked) return;
                if (value == null || value == string.Empty) _dataType = null;
                else DataType = NLib.Utils.NETTypeCache.Instance.FindType(value);
            }
        }

        #endregion
    }

    #endregion

    #region NDbProcedureResultInfo 

    /// <summary>
    /// NDbProcedureResultInfo class.
    /// </summary>
    [Serializable]
    public class NDbProcedureResultInfo : NDbSchemaObject
    {
        #region Internal Variable

        private string _columnName = string.Empty;
        private Type _dataType = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NDbProcedureResultInfo()
            : base()
        {
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="columnName">The Column Name.</param>
        /// <param name="dataType">The column DataType.</param>
        public NDbProcedureResultInfo(string columnName, Type dataType)
            : this()
        {
            this._columnName = columnName;
            this._dataType = dataType;
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~NDbProcedureResultInfo()
        {
            _dataType = null;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Column Name
        /// </summary>
        [Category("Information")]
        [Description("Get/Set Column Name")]
        [Browsable(true)]
        [XmlAttribute]
        public string ColumnName
        {
            get { return _columnName; }
            set
            {
                if (Locked) return;
                if (_columnName != value)
                {
                    _columnName = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Data Type
        /// </summary>
        [Category("Information")]
        [Description("Get/Set Data Type")]
        [Browsable(true)]
        [XmlIgnore]
        public Type DataType
        {
            get { return _dataType; }
            set
            {
                if (Locked) return;
                if (_dataType != value)
                {
                    _dataType = value;
                }
            }
        }
        /// <summary>
        /// Get/Set column DataType (.NET type) FullName
        /// </summary>
        [Browsable(true)]
        [Category("Information")]
        [Description("Get/Set .NET Type's Name Of Object FullName")]
        [RefreshProperties(RefreshProperties.Repaint)]
        //[XmlIgnore]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(23)]
        public string DataTypeName
        {
            get
            {
                if (_dataType != null) return _dataType.FullName;
                else
                    return "";
            }
            set
            {
                if (this.Locked) return;
                if (value == null || value == string.Empty) _dataType = null;
                else DataType = NLib.Utils.NETTypeCache.Instance.FindType(value);
            }
        }

        #endregion
    }

    #endregion

    #region NDbProcedureInfo

    /// <summary>
    /// NDbProcedureInfo class.
    /// </summary>
    [Serializable]
    public class NDbProcedureInfo : NDbSchemaObject
    {
        #region Internal Variable

        private string _procedureName;
        private List<NDbProcedureParameterInfo> _params = new List<NDbProcedureParameterInfo>();
        private List<NDbProcedureResultInfo> _results = new List<NDbProcedureResultInfo>();

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NDbProcedureInfo()
            : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="procedureName">Stored Procedure Name</param>
        public NDbProcedureInfo(string procedureName)
            : this()
        {
            this._procedureName = procedureName;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NDbProcedureInfo()
        {
            if (null != _results)
            {
                _results.Clear();
            }
            _results = null;
            if (null != _params)
            {
                _params.Clear();
            }
            _params = null;
            // Free Memory
            //GarbageCollector.FreeGC(this);
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Procedure Name
        /// </summary>
        [Category("Information")]
        [Description("Get/Set Procedure Name")]
        [Browsable(true)]
        [XmlAttribute]
        public string ProcedureName
        {
            get { return _procedureName; }
            set
            {
                if (Locked) return;
                if (_procedureName != value)
                {
                    _procedureName = value;
                }
            }
        }
        /// <summary>
        /// Gets the parameter list.
        /// </summary>
        [Category("Information")]
        [Description("Gets the parameter list.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<NDbProcedureParameterInfo> Parameters
        {
            get { return _params; }
        }
        /// <summary>
        /// Gets the result list.
        /// </summary>
        [Category("Information")]
        [Description("Gets the result list.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<NDbProcedureResultInfo> Results
        {
            get { return _results; }
        }

        #endregion
    }

    #endregion
}
