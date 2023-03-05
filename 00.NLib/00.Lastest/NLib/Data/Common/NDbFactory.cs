#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-07-22
=================
- Data Access Framework - Base Connection Factory updated.
  - Seperate RequiredSPParameterPrefix delegate to RequiredSPInputParameterPrefix, 
    RequiredSPOutputParameterPrefix with rewrite related code that call the previous one.
======================================================================================================================
Update 2015-07-07
=================
- Data Access Framework - Base Connection Factory updated.
  - Changed all log code used MethodBase.

======================================================================================================================
Update 2014-11-30
=================
- Data Access Framework - Connection Factory
  - Add Formatter supports.

======================================================================================================================
Update 2014-11-18
=================
- Data Access Framework - Connection Factory
  - ConnectionFactory class change name to NDbFactory and change namespace to NLib.Data.
  - Change log code to new log framework.

======================================================================================================================
Update 2010-02-03
=================
- Data Access Framework - common classes ported into GFA38
  - DbObjectAbstractFactory changed
    - Format some class document.
    - Change LogCategory property to LogTypeName.
    - Change GetLogCategory method to GetLogTypeName.
    - Adjust Info/Err method parameters to supported new Debug/Exception Framework.
  - ConnectionFactory class changed.
    - Merge all Schema Access abstract methods.
    - CreateConnection method changes      
      - Format some class document.
      - Return type changes from IDbConnection to DbConnection class
    - CreateCommand method changes 
      - Format some class document.
      - Parameter 'connection' change from IDbConnection to DbConnection
      - Parameter 'transaction' change from IDbTransaction to DbTransaction
      - Return type changes from IDbCommand to DbCommand class
    - CreateAdapter method changes 
      - Format some class document.
      - Parameter 'command' change from IDbCommand to DbCommand
      - Return type changes from IDbDataAdapter to DbDataAdapter class
    - DerivedParameters method changes 
      - Format some class document.
      - Parameter 'command' change from IDbCommand to DbCommand
    - AssignParameters method changes 
      - Format some class document.
      - Parameter 'command' change from IDbCommand to DbCommand
    - CreateParameter methods changes 
      - Format some class document.
      - Parameter 'command' change from IDbCommand to DbCommand
      - Return type changes from IDbDataParameter to DbParameter class
  - ConnectionFactory (generic) class added. This class provide generic version of 
    ConnectionFactory used as base class for all connection related object's factories.
    This class is implements connection/command/adaptor/parameter related abstract methods.
  - ConnectionFactory (generic) class - implementation of Schema access
    - GetMetaData fully implemented.
    - GetRestrictions fully implemented.
    - GetSchema fully implemented.

======================================================================================================================
Update 2008-09-26
=================
- ConnectionFactory implement for provide support Procedure related method.
  - Native factory implemented.
  - OleDb factory implemented.
  - Odbc factory implemented.

======================================================================================================================
Update 2008-08-04
=================
- Generate Method add and changed
  - Add New Generate Method SelectCurrentMax
  - Change generate method SelectMax to SelectNextMax

======================================================================================================================
Update 2008-06-12
=================
- Generate Method fixed bug
  - Fixed NextMaxValue is return next max value (not the current max value).

======================================================================================================================
Update 2008-06-06
=================
- Generate Method and subclass for specificed provider classes added.
  - Used for handle how to generate data in specificed column
  - current support method NextGuid, CurrentIdeneity, SystemDate, Sequence(oracle), 
    NextMaxValue(coumpond key support), Generator(InterBase, FireBird).

======================================================================================================================
Update 2008-06-04
=================
- ConnectionFactory - Methods added
  - Add New method CreateParameter and it's overload methods. Used this method to create new command parameter.

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
using System.Windows.Forms;

#endregion

namespace NLib.Data
{
    #region NDbFactory (abstract class)

    /// <summary>
    /// NDbFactory abstract class. The Data Access Factory class.
    /// </summary>
    public abstract class NDbFactory
    {
        #region Utils

        /// <summary>
        /// String Checking Utils
        /// </summary>
        protected class Utils : NLib.Utils.StringUtils { }

        #endregion

        #region Internal Variables

        private NDbFormatter _formatter = null;
        private NDbConfig _config = null;

        private DateTime tmpdate = DateTime.Now;
        private DateTime datecache = DateTime.MinValue;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NDbFactory() : base()
        {
            // Create Formatter and init by inherited class.
            _formatter = new NDbFormatter();
            InitFormatter(_formatter);
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~NDbFactory()
        {
            _config = null;
            _formatter = null;
            // Force free memory
            NGC.FreeGC(this);
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets config instance.
        /// </summary>
        protected NDbConfig Config
        {
            get { return _config; }
        }

        #endregion

        #region Abstract Methods

        #region Basic Formatter abstract methods

        #region Init Formatter

        /// <summary>
        /// Init formatter.
        /// </summary>
        /// <param name="formatter">The formatter instance.</param>
        protected abstract void InitFormatter(NDbFormatter formatter);

        #endregion

        #endregion

        #region Basic Create abstract methods

        #region Connection

        /// <summary>
        /// Get New Connection Instance
        /// </summary>
        /// <returns>
        /// Returns Connection instance that inherited from DbConnection class.
        /// </returns>
        public abstract DbConnection CreateConnection();

        #endregion

        #region Command and Adaptor

        /// <summary>
        /// Create Command
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="transaction">Transaction instance</param>
        /// <returns>
        /// Returns DbCommand instance.
        /// </returns>
        public abstract DbCommand CreateCommand(DbConnection connection,
            DbTransaction transaction);
        /// <summary>
        /// Create Data Adaptor for command (Select)
        /// </summary>
        /// <param name="command">The command object instance.</param>
        /// <returns>
        /// Returns DbDataAdapter instance that match command object.
        /// </returns>
        public abstract DbDataAdapter CreateAdapter(DbCommand command);

        #endregion

        #region Parameters

        /// <summary>
        /// Derived Parameters
        /// </summary>
        /// <param name="command">Stored Procedure's command object instance.</param>
        /// <returns>Returns true if command is successfully retrived.</returns>
        public abstract bool DerivedParameters(DbCommand command);
        /// <summary>
        /// Assign Parameters
        /// </summary>
        /// <param name="command">Stored Procedure's command object instance.</param>
        /// <param name="parameterNames">array of Parameter's name</param>
        /// <param name="parameterValues">array of Parameter's value</param>
        /// <returns>Returns true if successfully assigned parameters</returns>
        public abstract bool AssignParameters(DbCommand command,
            string[] parameterNames, object[] parameterValues);
        /// <summary>
        /// Create Parameter. 
        /// Used this method instead of call create parameter directly from DbCommand.
        /// </summary>
        /// <param name="command">The command object instance.</param>
        /// <param name="parameterName">The parameter's name.</param>
        /// <param name="providerDbTypeID">The Provider DataType ID</param>
        /// <param name="size">Set the size.</param>
        /// <param name="precision">Set the precision.</param>
        /// <param name="scale">Set the scale.</param>
        /// <param name="value">Set the object to assign to the parameter.</param>
        /// <returns>Returns the instance object that inherited from DbParameter.</returns>
        public abstract DbParameter CreateParameter(
            DbCommand command,
            string parameterName,
            int providerDbTypeID,
            int size, byte precision, byte scale,
            object value);
        /// <summary>
        /// Create New instance of IDbDataParameter.
        /// </summary>
        /// <param name="parameterName">The parameter's name.</param>
        /// <param name="value">The parameter's value.</param>
        /// <returns>Returns instance of DbParameter.</returns>
        public abstract DbParameter CreateParameter(string parameterName, object value);

        #endregion

        #endregion

        #region Basic schemas abstract methods

        #region Get Metadata

        /// <summary>
        /// Get Metadata
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <returns>List of all avaliable metadata from provider.</returns>
        public abstract List<NDbMetaData> GetMetadata(DbConnection connection);

        #endregion

        #region Get Restrictions

        /// <summary>
        /// Get Restriction for specificed Metadata
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="value">specificed Metadata to find restriction information</param>
        /// <returns>List of restriction information related to specificed Metadata</returns>
        public abstract List<NDbRestriction> GetRestrictions(DbConnection connection,
            NDbMetaData value);

        #endregion

        #region Get Schema

        /// <summary>
        /// Get Schema. This Method is used for Get Schema information for specificed Meta Data information.
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="value">specificed Metadata to find information</param>
        /// <param name="restrictions">Restriction Array</param>
        /// <returns>Information about specificed metadata</returns>
        public abstract DataTable GetSchema(DbConnection connection,
            NDbMetaData value, NDbRestriction[] restrictions);

        #endregion

        #region Get Tables (and views)

        /// <summary>
        /// Get Tables (and views)
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="owner">Owner of Tables/Views</param>
        /// <returns>List of all avaliable Tables/Views from provider.</returns>
        public abstract List<NDbTable> GetTables(DbConnection connection, string owner);

        #endregion

        #region GetProcedures

        /// <summary>
        /// Get Procedures.
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="owner">Owner of Stored Procedures</param>
        /// <returns>Returns List of Stored Procedure's name.</returns>
        public abstract List<string> GetProcedures(DbConnection connection, string owner);

        #endregion

        #region GetProcedureInfo

        /// <summary>
        /// Gets Stored Procedure Information.
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="owner">Owner of Stored Procedure</param>
        /// <param name="procedureName">The Stored Procedure Name.</param>
        /// <returns>Returns the Stored Procedure Information.</returns>
        public abstract NDbProcedureInfo GetProcedureInfo(DbConnection connection,
            string owner, string procedureName);

        #endregion

        #endregion

        #endregion

        #region Public Methods

        #region CreateParameter Overload methods

        /// <summary>
        /// Create Parameter. 
        /// Used this method instead of call create parameter directly from DbCommand.
        /// </summary>
        /// <param name="command">The command object instance.</param>
        /// <param name="parameterName">The parameter's name.</param>
        /// <param name="providerDbTypeID">The Provider DataType ID</param>
        /// <param name="size">Set the size.</param>
        /// <param name="precision">Set the precision.</param>
        /// <param name="scale">Set the scale.</param>
        /// <returns>Returns the instance of DbParameter.</returns>
        public DbParameter CreateParameter(DbCommand command,
            string parameterName, int providerDbTypeID, int size, byte precision, byte scale)
        {
            return CreateParameter(command, parameterName, providerDbTypeID, size, precision, scale, null);
        }
        /// <summary>
        /// Create Parameter. 
        /// Used this method instead of call create parameter directly from DbCommand.
        /// </summary>
        /// <param name="command">The command object instance.</param>
        /// <param name="parameterName">The parameter's name.</param>
        /// <param name="providerDbTypeID">The Provider DataType ID</param>
        /// <param name="size">Set the size.</param>
        /// <param name="value">Set the object to assign to the parameter.</param>
        /// <returns>Returns the instance of DbParameter.</returns>
        public DbParameter CreateParameter(DbCommand command,
            string parameterName, int providerDbTypeID, int size, object value)
        {
            return CreateParameter(command, parameterName, providerDbTypeID, size, 0, 0, value);
        }
        /// <summary>
        /// Create Parameter. 
        /// Used this method instead of call create parameter directly from DbCommand.
        /// </summary>
        /// <param name="command">The command object instance.</param>
        /// <param name="parameterName">The parameter's name.</param>
        /// <param name="providerDbTypeID">The Provider DataType ID</param>
        /// <param name="size">Set the size.</param>
        /// <returns>Returns the instance of DbParameter.</returns>
        public DbParameter CreateParameter(DbCommand command,
            string parameterName, int providerDbTypeID, int size)
        {
            return CreateParameter(command, parameterName, providerDbTypeID, size, 0, 0, null);
        }
        /// <summary>
        /// Create Parameter. 
        /// Used this method instead of call create parameter directly from DbCommand.
        /// </summary>
        /// <param name="command">The command object instance.</param>
        /// <param name="parameterName">The parameter's name.</param>
        /// <param name="providerDbTypeID">The Provider DataType ID</param>
        /// <param name="value">Set the object to assign to the parameter.</param>
        /// <returns>Returns the instance of DbParameter.</returns>
        public DbParameter CreateParameter(DbCommand command,
            string parameterName, int providerDbTypeID, object value)
        {
            return CreateParameter(command, parameterName, providerDbTypeID, 0, 0, 0, value);
        }
        /// <summary>
        /// Create Parameter. 
        /// Used this method instead of call create parameter directly from DbCommand.
        /// </summary>
        /// <param name="command">The command object instance.</param>
        /// <param name="parameterName">The parameter's name.</param>
        /// <param name="providerDbTypeID">The Provider DataType ID</param>
        /// <returns>Returns the instance of DbParameter.</returns>
        public DbParameter CreateParameter(DbCommand command,
            string parameterName, int providerDbTypeID)
        {
            return CreateParameter(command, parameterName, providerDbTypeID, 0, 0, 0, null);
        }

        #endregion

        #region Virtual methods

        #region Init

        /// <summary>
        /// Init Factory. This method should call before main form is initialized.
        /// </summary>
        public virtual void Init() { }

        #endregion

        #region Set Config

        /// <summary>
        /// Set config instance.
        /// </summary>
        /// <param name="value">The config instance.</param>
        public virtual void SetConfig(NDbConfig value)
        {
            _config = value;
        }

        #endregion

        #region Type Conversion

        /// <summary>
        /// Change Type.
        /// </summary>
        /// <param name="dbType">The DbType value.</param>
        /// <returns>Returns .NET type that match specificed DbType.</returns>
        public virtual Type ChangeType(DbType dbType)
        {
            Type type = null;

            switch (dbType)
            {
                case DbType.AnsiString:
                    type = typeof(string);
                    break;
                case DbType.AnsiStringFixedLength:
                    type = typeof(string);
                    break;
                case DbType.Binary:
                    type = typeof(byte[]);
                    break;
                case DbType.Boolean:
                    type = typeof(bool);
                    break;
                case DbType.Byte:
                    type = typeof(byte);
                    break;
                case DbType.Currency:
                    type = typeof(decimal);
                    break;
                case DbType.Date:
                    type = typeof(DateTime);
                    break;
                case DbType.DateTime:
                    type = typeof(DateTime);
                    break;
                case DbType.DateTime2:
                    type = typeof(DateTime);
                    break;
                case DbType.DateTimeOffset:
                    type = typeof(DateTimeOffset);
                    break;
                case DbType.Decimal:
                    type = typeof(decimal);
                    break;
                case DbType.Double:
                    type = typeof(double);
                    break;
                case DbType.Guid:
                    type = typeof(Guid);
                    break;
                case DbType.Int16:
                    type = typeof(short);
                    break;
                case DbType.Int32:
                    type = typeof(int);
                    break;
                case DbType.Int64:
                    type = typeof(long);
                    break;
                case DbType.Object:
                    type = typeof(object);
                    break;
                case DbType.SByte:
                    type = typeof(sbyte);
                    break;
                case DbType.Single:
                    type = typeof(float);
                    break;
                case DbType.String:
                    type = typeof(string);
                    break;
                case DbType.StringFixedLength:
                    type = typeof(string);
                    break;
                case DbType.Time:
                    type = typeof(DateTime);
                    break;
                case DbType.UInt16:
                    type = typeof(ushort);
                    break;
                case DbType.UInt32:
                    type = typeof(uint);
                    break;
                case DbType.UInt64:
                    type = typeof(ulong);
                    break;
                case DbType.VarNumeric:
                    type = typeof(decimal);
                    break;
                case DbType.Xml:
                    type = typeof(string);
                    break;
            }

            return type;
        }

        #endregion

        #region Connection/Command Timeout (for custom connection stirng required to overrides)

        /// <summary>
        /// Checks has connection timeout.
        /// </summary>
        /// <returns>Returns true if the derived class allow to set connection timeout.</returns>
        protected virtual bool HasConnectionTimeout { get { return true; } }
        /// <summary>
        /// Gets Connection Timeout.
        /// </summary>
        protected virtual int ConnectionTimeout { get { return 360; } }
        /// <summary>
        /// Checks has command timeout.
        /// </summary>
        /// <returns>Returns true if the derived class allow to set command timeout.</returns>
        protected virtual bool HasCommandTimeout { get { return true; } }
        /// <summary>
        /// Gets Command Timeout.
        /// </summary>
        protected virtual int CommandTimeout { get { return 360; } }
        /// <summary>
        /// DoConnected method. This method should call after connected.
        /// </summary>
        /// <param name="connection">The connection instance.</param>
        /// <param name="transaction">The transaction instance.</param>
        internal void DoConnected(DbConnection connection, DbTransaction transaction)
        {
            // Call method(s) that required to execute immediately after connected.
            SetDateFormat(connection, transaction);
            // Call virtual method.
            OnConnected(connection, transaction);
        }
        /// <summary>
        /// OnConnected method. This method will call after connected.
        /// </summary>
        /// <param name="connection">The connection instance.</param>
        /// <param name="transaction">The transaction instance.</param>
        protected virtual void OnConnected(DbConnection connection, DbTransaction transaction)
        {

        }

        #endregion

        #region Formatter Methods

        /// <summary>
        /// Gets The Formatter instance.
        /// </summary>
        [Browsable(false)]
        public NDbFormatter Formatter { get { return _formatter; } }
        /// <summary>
        /// Get Parameter Prefix. 
        /// Default value depend on implements of formatter instance. 
        /// If no implement string.Empty returns.
        /// </summary>
        /// <returns>Returns string that represent parameter prefix.</returns>
        protected string GetParameterPrefix() 
        {
            if (null != _formatter)
                return _formatter.GetParameterPrefix();
            return string.Empty; 
        }
        /// <summary>
        /// Checks is required parameter prefix for input parameter.
        /// </summary>
        /// <returns>Returns true if required.</returns>
        protected bool RequiredSPInputParameterPrefix()
        {
            if (null != _formatter)
                return _formatter.RequiredSPInputParameterPrefix();
            return false;
        }
        /// <summary>
        /// Checks is required parameter prefix for output parameter.
        /// </summary>
        /// <returns>Returns true if required.</returns>
        protected bool RequiredSPOutputParameterPrefix()
        {
            if (null != _formatter)
                return _formatter.RequiredSPOutputParameterPrefix();
            return false;
        }
        /// <summary>
        /// Get Default Return parameter's name. 
        /// Default value depend on implements of formatter instance. 
        /// If no implement null value returns.
        /// </summary>
        /// <returns>Returns string that represent default parameter name.</returns>
        protected string GetDefaultReturnParameterName()
        {
            if (null != _formatter)
                return _formatter.GetDefaultReturnParameterName();
            return null;
        }

        #endregion

        #region Get Provider DataTypes

        /// <summary>
        /// Get Provider DataTypes. This method is used for Get all DataTypes for the current data provider.
        /// </summary>
        /// <param name="connection">Connection instance.</param>
        /// <returns>List of Provider's datatypes</returns>
        public virtual List<NDbProviderDataType> GetProviderDataTypes(DbConnection connection)
        {
            NDbMetaData meta = new NDbMetaData("DataTypes");
            NDbRestriction[] restrictions = new NDbRestriction[0];
            DataTable table = GetSchema(connection, meta, restrictions);

            List<NDbProviderDataType> results = new List<NDbProviderDataType>();
            if (table != null && table.Rows != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    NDbProviderDataType val = new NDbProviderDataType();

                    #region TypeName and Provider DbType ID

                    val.TypeName = row["TypeName"].ToString();

                    if (!Utils.IsInteger(row["ProviderDbType"].ToString()))
                        val.ProviderDbType = -1; // UNKNOWN used -1 insteed.
                    else val.ProviderDbType = int.Parse(row["ProviderDbType"].ToString());

                    #endregion

                    #region Column Size

                    if (!Utils.IsNumber(row["ColumnSize"].ToString()))
                        val.ColumnSize = 0; // UNKNOWN used 0 insteed.
                    else val.ColumnSize = decimal.Parse(row["ColumnSize"].ToString());

                    #endregion

                    #region Create Format and Create Parameters

                    if (row["CreateFormat"] == DBNull.Value)
                        val.CreateFormat = string.Empty;
                    else val.CreateFormat = row["CreateFormat"].ToString();

                    if (row["CreateParameters"] == DBNull.Value)
                        val.CreateParameters = string.Empty;
                    else val.CreateParameters = row["CreateParameters"].ToString();

                    #endregion

                    #region DataType

                    val.DataType = row["DataType"].ToString();

                    #endregion

                    #region IsAutoIncrementable, IsBestMatch, IsCaseSensitive, IsFixedLength, IsFixedPrecisionScale, IsLong

                    if (!Utils.IsBool(row["IsAutoIncrementable"].ToString()))
                        val.IsAutoIncrementable = false;
                    else val.IsAutoIncrementable = bool.Parse(row["IsAutoIncrementable"].ToString());

                    if (!Utils.IsBool(row["IsBestMatch"].ToString()))
                        val.IsBestMatch = false;
                    else
                        val.IsBestMatch = bool.Parse(row["IsBestMatch"].ToString());

                    if (!Utils.IsBool(row["IsCaseSensitive"].ToString()))
                        val.IsCaseSensitive = false;
                    else val.IsCaseSensitive = bool.Parse(row["IsCaseSensitive"].ToString());

                    if (!Utils.IsBool(row["IsFixedLength"].ToString()))
                        val.IsFixedLength = false;
                    else val.IsFixedLength = bool.Parse(row["IsFixedLength"].ToString());

                    if (!Utils.IsBool(row["IsFixedPrecisionScale"].ToString()))
                        val.IsFixedPrecisionScale = false;
                    else val.IsFixedPrecisionScale = bool.Parse(row["IsFixedPrecisionScale"].ToString());

                    if (!Utils.IsBool(row["IsLong"].ToString()))
                        val.IsLong = false;
                    else val.IsLong = bool.Parse(row["IsLong"].ToString());

                    #endregion

                    #region Nullable, Searchable

                    if (!Utils.IsBool(row["IsNullable"].ToString()))
                        val.Nullable = NullSupportMode.Unknown; // Unknown
                    else
                    {
                        if (bool.Parse(row["IsNullable"].ToString()))
                            val.Nullable = NullSupportMode.Allow;
                        else val.Nullable = NullSupportMode.NotAllow;
                    }

                    try
                    {
                        bool searchable = bool.Parse(row["IsSearchable"].ToString());
                        bool searchablewithlike = bool.Parse(row["IsSearchableWithLike"].ToString());

                        if (searchable && searchablewithlike)
                            val.Searchable = SearchSupportMode.Searchable;
                        else if (!searchable && searchablewithlike)
                            val.Searchable = SearchSupportMode.SupportSearchInLikeClauseOnly;
                        else if (searchable && !searchablewithlike)
                            val.Searchable = SearchSupportMode.SupportBasicSearch;
                        else
                            val.Searchable = SearchSupportMode.NotSupportSearch;
                    }
                    catch (Exception)
                    {
                        val.Searchable = SearchSupportMode.NotSupportSearch;
                    }

                    #endregion

                    #region IsUnsigned, IsConcurrencyType, IsLiteralSupported

                    if (!Utils.IsBool(row["IsUnsigned"].ToString()))
                        val.IsUnsigned = false;
                    else val.IsUnsigned = bool.Parse(row["IsUnsigned"].ToString());

                    if (!Utils.IsBool(row["IsConcurrencyType"].ToString()))
                        val.IsConcurrencyType = false;
                    else val.IsConcurrencyType = bool.Parse(row["IsConcurrencyType"].ToString());

                    if (!Utils.IsBool(row["IsLiteralSupported"].ToString()))
                        val.IsLiteralSupported = false;
                    else val.IsLiteralSupported = bool.Parse(row["IsLiteralSupported"].ToString());

                    #endregion

                    #region Min/Max Scale

                    if (!Utils.IsInteger(row["MinimumScale"].ToString()))
                        val.MinimumScale = -1; // -1 if unknown
                    else val.MinimumScale = int.Parse(row["MinimumScale"].ToString());

                    if (!Utils.IsInteger(row["MaximumScale"].ToString()))
                        val.MaximumScale = -1; // -1 if unknown
                    else val.MaximumScale = int.Parse(row["MaximumScale"].ToString());

                    #endregion

                    #region Prefix/Suffix

                    val.LiteralPrefix = (row["LiteralPrefix"] != DBNull.Value) ? row["LiteralPrefix"].ToString() : string.Empty;
                    val.LiteralSuffix = (row["LiteralSuffix"] != DBNull.Value) ? row["LiteralSuffix"].ToString() : string.Empty;

                    #endregion

                    val.Lock();

                    // Add to list
                    results.Add(val);
                }
            }

            return results;
        }

        #endregion

        #region Get Reserved words

        /// <summary>
        /// Get Reserved words. This method is used for Get all reserved words for the current data provider.
        /// </summary>
        /// <param name="connection">Connection instance.</param>
        /// <returns>List of Provider's Reserved words</returns>
        public virtual List<NDbReservedword> GetReservedwords(DbConnection connection)
        {
            NDbMetaData meta = new NDbMetaData("ReservedWords");
            NDbRestriction[] restrictions = new NDbRestriction[0];
            DataTable table = GetSchema(connection, meta, restrictions);

            List<NDbReservedword> results = new List<NDbReservedword>();
            if (table != null && table.Rows != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    NDbReservedword val = new NDbReservedword();

                    val.Reservedword = row["Reservedword"].ToString();
                    val.Lock();

                    // Add to list
                    results.Add(val);
                }
            }

            return results;
        }

        #endregion

        #region Get Schema Table

        /// <summary>
        /// Get Schema Table. This method is used to retrived Schema information about specificed
        /// Command i.e. column information.
        /// </summary>
        /// <param name="command">Command object that already initialized parameters</param>
        /// <param name="transaction">Transaction instance</param>
        /// <returns>Result Schema Table</returns>
        public virtual DataTable GetSchemaTable(DbCommand command, DbTransaction transaction)
        {
            if (command != null && command.Connection != null &&
                command.Connection.State == ConnectionState.Open)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                // Free Memory
                NGC.FreeGC();

                if (transaction != null &&
                    command.Transaction != transaction)
                {
                    DbTransaction oldTrans = command.Transaction; // Keep original
                    try
                    {
                        command.Transaction = transaction; // Assigned Transaction
                    }
                    catch (Exception ex)
                    {
                        string msg = "Assigned transaction failed.";
                        msg.Err(med);
                        ex.Err(med);
                        command.Transaction = oldTrans; // Assigned original
                    }
                }

                DataTable result = null;
                IDataReader reader = null;

                try
                {
                    //ShowBusyCursor();
                    lock (this)
                    {
                        reader = command.ExecuteReader(CommandBehavior.KeyInfo);
                    }

                    if (reader != null)
                    {
                        DataTable table = null;
                        try
                        {
                            table = reader.GetSchemaTable();

                            result = table;
                        }
                        catch (Exception ex0)
                        {
                            string msg = "Cannot get schema table.";
                            msg.Err(med);
                            ex0.Err(med);

                            if (table != null)
                            {
                                try
                                {
                                    table.Dispose();
                                }
                                catch { } // Ignore
                            }
                            table = null;
                        }

                        try
                        {
                            if (!reader.IsClosed)
                                reader.Close();
                        }
                        catch (Exception ex1)
                        {
                            string msg = "Close Reader operation.";
                            msg.Err(med);
                            ex1.Err(med);
                        }
                        try { reader.Dispose(); }
                        catch (Exception ex2)
                        {
                            string msg = "Dispose Reader operation.";
                            msg.Err(med);
                            ex2.Err(med);
                        }
                        reader = null;
                    }
                    //ShowNormalCursor();
                }
                catch (Exception ex)
                {
                    string msg = "Execute reader operation failed.";
                    msg.Err(med);
                    ex.Err(med);

                    result = null;
                }
                finally
                {
                    //ShowNormalCursor();
                }

                return result;
            }
            else return null;
        }

        #endregion

        #region GetTasks
        
        /// <summary>
        /// Gets Supported Task.
        /// </summary>
        /// <returns>Returns list of all avaliable supported tasks.</returns>
        public virtual NDbTask[] GetTasks()
        {
            return null;
        }

        #endregion

        #region Set Date Format

        /// <summary>
        /// Set Date Format.
        /// </summary>
        /// <param name="connection">The connection instance.</param>
        /// <param name="transaction">Transaction instance</param>
        protected virtual void SetDateFormat(DbConnection connection, 
            DbTransaction transaction)
        {

        }

        #endregion

        #region Get Server Date

        /// <summary>
        /// Get Server Date.
        /// </summary>
        /// <param name="connection">The connection instance.</param>
        /// <param name="transaction">Transaction instance</param>
        /// <returns>Result Current DateTime.</returns>
        protected virtual DateTime GetServerDate(DbConnection connection, 
            DbTransaction transaction)
        {
            bool executeOK = false;
            DateTime serverDate = DateTime.Now;

            if (connection == null || connection.State != ConnectionState.Open)
                return DateTime.Now;

            DbCommand cmd = connection.CreateCommand();
            if (transaction != null)
            {
                cmd.Transaction = transaction; // setup transaction
            }
            // set custom command text for get date from database server.
            cmd.CommandText = (null != this.Formatter) ? 
                this.Formatter.GetServerDateQuery() : string.Empty;

            if (!string.IsNullOrWhiteSpace(cmd.CommandText))
            {
                lock (this)
                {
                    object value = null;
                    try { value = cmd.ExecuteScalar(); }
                    catch { }

                    if (value != null && value != DBNull.Value &&
                        value.GetType() == typeof(DateTime))
                    {
                        serverDate = (DateTime)value;
                    }
                }

                if (cmd != null) cmd.Dispose();
                cmd = null;
            }
            else
            {
                executeOK = false; // cannot execute.
            }

            return (executeOK) ? serverDate : DateTime.Now;
        }
        /// <summary>
        /// Get Date Internal based on config setting.
        /// </summary>
        /// <param name="connection">The connection instance.</param>
        /// <param name="transaction">Transaction instance</param>
        /// <returns>Result Current DateTime.</returns>
        private DateTime GetDate(DbConnection connection, DbTransaction transaction)
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                return DateTime.Now;
            }
            if (datecache == DateTime.MinValue)
            {
                datecache = GetServerDate(connection, transaction);
                // keep value to checking
                tmpdate = DateTime.Now;

                return datecache;
            }
            if (null != this.Config && null != this.Config.Optional &&
                this.Config.Optional.UseDateCache)
            {
                return datecache.AddTicks(DateTime.Now.Ticks - tmpdate.Ticks);
            }
            else
            {
                datecache = GetServerDate(connection, transaction);
                // keep value to checking
                tmpdate = DateTime.Now;

                return datecache;
            }
        }

        #endregion

        #endregion

        #region Database Methods

        /// <summary>
        /// Gets Current DateTime.
        /// </summary>
        /// <param name="connection">The connection instance.</param>
        /// <param name="transaction">Transaction instance.</param>
        /// <returns>Result Current DateTime.</returns>
        public DateTime GetCurrentDate(DbConnection connection,
            DbTransaction transaction = null)
        {
            return GetDate(connection, transaction);
        }

        #endregion

        #endregion
    }

    #endregion

    #region NDbFactory (abstract generic class)

    /// <summary>
    /// NDbFactory (generic) class. This abstract class is provide generic version of
    /// NDbFactory that used as base class for all database object's factories.
    /// </summary>
    /// <typeparam name="TConnection">The DbConnection Type.</typeparam>
    /// <typeparam name="TTransaction">The DbTransaction Type.</typeparam>
    /// <typeparam name="TCommand">The DbCommand Type.</typeparam>
    /// <typeparam name="TParameter">The DbParameter Type.</typeparam>
    /// <typeparam name="TAdaptor">The DbDataAdaptor Type.</typeparam>
    /// <typeparam name="TBuilder">The DbCommandBuilder Type.</typeparam>
    public abstract class NDbFactory<TConnection, TTransaction, 
        TCommand, TParameter, TAdaptor, TBuilder> : NDbFactory
        where TConnection : DbConnection, new()
        where TTransaction : DbTransaction
        where TCommand : DbCommand, new()
        where TParameter : DbParameter, new()
        where TAdaptor : DbDataAdapter, new()
        where TBuilder : DbCommandBuilder
    {
        #region Virtual Methods

        /// <summary>
        /// Overrides to handle the validation logic to check the prerequisition
        /// for initialize the connection intance.
        /// </summary>
        /// <returns>Returns true if the can create the connection instance.</returns>
        protected virtual bool CanCreateConnection() { return true; }
        /// <summary>
        /// Overrides to customized the parameter's characteristic after 
        /// DerivedParameters is called. Note that the _DerivedDbParameters method
        /// is call static method DeriveParameters on the DbBuilder object so 
        /// if the DbBuilder is not contains DeriveParameters then no parameters is derived
        /// so the user code should handle whne the DbBuilder is not contains static method 
        /// DeriveParameters.
        /// </summary>
        /// <param name="command">The command instance that parameters is derived.</param>
        protected virtual void OnDerivedDbParameters(TCommand command)
        {
        }
        /// <summary>
        /// Set the provide db type id to the parameter object.
        /// </summary>
        /// <param name="parameter">The parameter object.</param>
        /// <param name="providerDbTypeID">The provide type id.</param>
        protected virtual void SetProviderDbTypeID(TParameter parameter, int providerDbTypeID)
        {
        }
        /// <summary>
        /// Get Table Schema Info.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <returns>Returns the Table SchemaInfo instance for DbTable.</returns>
        protected virtual NSchemaInfo<NDbTable> GetTableSchemaInfo(string owner) { return null; }
        /// <summary>
        /// Get View Schema Info.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <returns>Returns the Table SchemaInfo instance for DbTable.</returns>
        protected virtual NSchemaInfo<NDbTable> GetViewSchemaInfo(string owner) { return null; }

        #endregion

        #region Protected methods

        #region Connection

        /// <summary>
        /// Get New Connection Instance (internl used).
        /// </summary>
        /// <returns>
        /// Returns Connection instance that inherited from DbConnection class.
        /// </returns>
        protected TConnection _CreateDbConnection()
        {
            if (!CanCreateConnection())
                return null;
            MethodBase med = MethodBase.GetCurrentMethod();
            TConnection inst = null;
            try
            {
                inst = new TConnection();
            }
            catch (Exception ex)
            {
                string msg = "Cannot create connection object.";
                msg.Err(med);
                ex.Err(med);

                inst = null;
            }
            return inst;
        }

        #endregion

        #region Commands

        /// <summary>
        /// Create Command (internl used).
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="transaction">Transaction instance</param>
        /// <returns>
        /// Returns DbCommand instance.
        /// </returns>
        protected TCommand _CreateDbCommand(TConnection connection, TTransaction transaction)
        {
            if (connection == null || connection.State != ConnectionState.Open)
                return null;
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                //TCommand command = new TCommand();
                TCommand command = connection.CreateCommand() as TCommand;
                if (null != command)
                {
                    command.Connection = connection;
                    if (this.HasCommandTimeout)
                    {
                        command.CommandTimeout = this.CommandTimeout;
                    }
                    if (transaction != null && transaction is TTransaction)
                        command.Transaction = (transaction as TTransaction);
                }
                return command;
            }
            catch (Exception ex)
            {
                string msg = "Cannot create command object.";
                msg.Err(med);
                ex.Err(med);

                return null;
            }
        }
        /// <summary>
        /// Create Data Adaptor for command (Select)
        /// </summary>
        /// <param name="command">The command object instance.</param>
        /// <returns>
        /// Returns DbDataAdapter instance that match command object.
        /// </returns>
        protected TAdaptor _CreateDbAdapter(TCommand command)
        {
            if (command == null || command.Connection == null ||
                command.Connection.State != ConnectionState.Open)
                return null;

            MethodBase med = MethodBase.GetCurrentMethod();
            TAdaptor adaptor = null;
            try
            {
                adaptor = new TAdaptor();
                adaptor.SelectCommand = command;
            }
            catch (Exception ex)
            {
                string msg = "Cannot create adaptor object.";
                msg.Err(med);
                ex.Err(med);

                return null;
            }
            return adaptor;
        }
        /// <summary>
        /// Derived Parameters
        /// </summary>
        /// <param name="command">Stored Procedure's command object instance.</param>
        /// <returns>Returns true if command is successfully retrived.</returns>
        protected bool _DerivedDbParameters(TCommand command)
        {
            if (command == null || command.Connection == null ||
                command.Connection.State != ConnectionState.Open)
                return false;

            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                // call static method
                NLib.Reflection.DynamicAccess<TBuilder>.CallStatic("DeriveParameters",
                    command);
                // check custom change by user code
                OnDerivedDbParameters(command);

                return true;
            }
            catch (Exception ex)
            {
                string msg = "Cannot derived parameters.";
                msg.Err(med);
                ex.Err(med);

                return false;
            }
        }
        /// <summary>
        /// Assign Parameters.
        /// </summary>
        /// <param name="command">Stored Procedure's command object instance.</param>
        /// <param name="parameterNames">array of Parameter's name</param>
        /// <param name="parameterValues">array of Parameter's value</param>
        /// <returns>Returns true if successfully assigned parameters</returns>
        protected bool _AssignDbParameters(TCommand command,
            string[] parameterNames, object[] parameterValues)
        {
            if (command == null || command.Connection == null ||
                command.Connection.State != ConnectionState.Open)
                return false;

            MethodBase med = MethodBase.GetCurrentMethod();

            #region Check parameters

            if (parameterNames == null && parameterValues == null)
                return true; // Assign no parameter

            if (parameterNames != null && parameterValues == null ||
                parameterNames == null && parameterValues != null)
            {
                string msg = "Cannot assigned parameters : parameter names or parameter values is null.";
                msg.Err(med);

                return false; // Mismatch
            }

            if (parameterNames != null && parameterValues != null &&
                parameterNames.Length != parameterValues.Length)
            {
                string msg = "Cannot assigned parameters : parameter names and parameter values length not match.";
                msg.Err(med);

                return false; // Mismatch Length
            }

            #endregion

            string inputPararameterPrefix = string.Empty;
            if (this.RequiredSPInputParameterPrefix())
            {
                inputPararameterPrefix = GetParameterPrefix(); // read prameter prefix
            }

            int len = parameterNames.Length;
            for (int i = 0; i < len; i++)
            {
                string paraName = inputPararameterPrefix + parameterNames[i].Trim();
                object paraVal = parameterValues[i];
                if (!command.Parameters.Contains(paraName))
                {
                    string msg = string.Format(
                        "The Parameter name : {0} not in avaliable parameter's list.", paraName);
                    msg.Err(med);

                    return false; // Mismatch Name
                }
                try
                {
                    TParameter spPara = command.Parameters[paraName] as TParameter;
                    if (spPara != null)
                    {
                        spPara.Value = (paraVal == null) ? DBNull.Value : paraVal;
                    }
                    else
                    {
                        string msg = string.Format(
                            "The Parameter name : {0} is null.", paraName);
                        msg.Err(med);

                        return false; // Cannot find parameter by name
                    }
                }
                catch (Exception ex)
                {
                    string msg = "Assigned parameters failed.";
                    msg.Err(med);
                    ex.Err(med);

                    return false;
                }
            }

            return true; // All OK
        }
        /// <summary>
        /// Create Parameter. 
        /// Used this method instead of call create parameter directly from DbCommand.
        /// </summary>
        /// <param name="command">The command object instance.</param>
        /// <param name="parameterName">The parameter's name.</param>
        /// <param name="providerDbTypeID">The Provider DataType ID</param>
        /// <param name="size">Set the size.</param>
        /// <param name="precision">Set the precision.</param>
        /// <param name="scale">Set the scale.</param>
        /// <param name="value">Set the object to assign to the parameter.</param>
        /// <returns>Returns the instance object that inherited from DbParameter.</returns>
        protected TParameter _CreateDbParameter(
            TCommand command,
            string parameterName,
            int providerDbTypeID,
            int size, byte precision, byte scale,
            object value)
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            TParameter result = null;
            // check connection
            if (command == null || command.Connection == null ||
                command.Connection.State != ConnectionState.Open)
                return result;
            IDbDataParameter para = null;

            try
            {
                para = (command.CreateParameter() as IDbDataParameter);
                if (null == para)
                    return result; // cannot create parameter
                para.ParameterName = parameterName;
                // set provide type
                SetProviderDbTypeID(para as TParameter, providerDbTypeID);
                // set size
                if (size > 0) para.Size = size;
                // set value
                para.Value = value;
                // assign precision and scale.
                if (precision > 0)
                {
                    #region Try to assign

                    try { para.Precision = precision; }
                    catch (Exception ex1)
                    {
                        string msg = "Invalid precision.";
                        msg.Err(med);
                        ex1.Err(med);
                    }

                    #endregion
                }
                if (scale > 0)
                {
                    #region Try to assign

                    try { para.Scale = scale; }
                    catch (Exception ex2)
                    {
                        string msg = "Invalid scale.";
                        msg.Err(med);
                        ex2.Err(med);
                    }

                    #endregion
                }
                // update to result
                result = (null != para) ? (para as TParameter) : null;
                return result; // return to caller
            }
            catch (Exception ex)
            {
                string msg = "Cannot create parameter.";
                msg.Err(med);
                ex.Err(med);
                return result;
            }
        }
        /// <summary>
        /// Create New instance of DbParameter.
        /// </summary>
        /// <param name="parameterName">The parameter's name.</param>
        /// <param name="value">The parameter's value.</param>
        /// <returns>Returns instance of DbParameter.</returns>
        public TParameter _CreateDbParameter(string parameterName, object value)
        {
            TParameter inst = null;
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                inst = new TParameter();
            }
            catch (Exception ex)
            {
                string msg = string.Format(
                    "Cannot create parameter : {0} is null.", parameterName);
                msg.Err(med);
                ex.Err(med);
            }
            return inst;
        }

        #endregion

        #region Schemas

        #region Get Metadata

        /// <summary>
        /// Get Metadata
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <returns>List of all avaliable metadata from provider.</returns>
        protected List<NDbMetaData> _GetDbMetadata(TConnection connection)
        {
            if (connection == null ||
                connection.State != ConnectionState.Open)
                return null;

            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                List<NDbMetaData> results = new List<NDbMetaData>();

                DataTable table = connection.GetSchema("MetaDataCollections");
                if (table != null && table.Rows != null && table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row["CollectionName"] != DBNull.Value)
                        {
                            NDbMetaData val = new NDbMetaData();

                            val.CollectionName = row["CollectionName"].ToString();
                            // Number Of Restriction
                            val.NoOfRestriction = (row["NumberOfRestrictions"] != DBNull.Value) ?
                                (int)row["NumberOfRestrictions"] : 0;
                            // Number Of Identifier Parts
                            val.NoOfIdentifierParts = (row["NumberOfIdentifierParts"] != DBNull.Value) ?
                                (int)row["NumberOfIdentifierParts"] : 0;

                            val.Lock(); // lock object

                            results.Add(val);
                        }
                    }
                }
                else
                {
                    "No metadata return.".Info(med);
                }

                try
                {
                    if (table != null) table.Dispose();
                }
                catch { }

                return results;
            }
            catch (Exception ex)
            {
                "GetMetaData error.".Err(med);
                ex.Err(med);
                return null;
            }
        }

        #endregion

        #region Get Restrictions

        /// <summary>
        /// Get Restriction for specificed Metadata
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="value">specificed Metadata to find restriction information</param>
        /// <returns>List of restriction information related to specificed Metadata</returns>
        protected List<NDbRestriction> _GetDbRestrictions(TConnection connection,
            NDbMetaData value)
        {
            if (value == null)
                return null;
            if (connection == null ||
                connection.State != ConnectionState.Open)
                return null;

            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                List<NDbRestriction> results = new List<NDbRestriction>();

                DataTable table = connection.GetSchema("Restrictions");
                if (table != null && table.Rows != null && table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row["RestrictionName"] != DBNull.Value)
                        {
                            NDbRestriction val = new NDbRestriction();

                            val.RestrictionName = row["RestrictionName"].ToString();
                            // Ord
                            val.Ordinal = (row["RestrictionNumber"] != DBNull.Value) ?
                                (int)row["RestrictionNumber"] : 0;

                            if (row["CollectionName"] != DBNull.Value &&
                                row["CollectionName"].ToString() == value.CollectionName)
                            {
                                val.Lock(); // lock object

                                results.Add(val);
                            }
                        }
                    }

                }
                else
                {
                    "No restriction data return.".Info(med);
                }

                try
                {
                    if (table != null) table.Dispose();
                }
                catch { }

                return results;
            }
            catch (Exception ex)
            {
                "GetRestrictions error.".Err(med);
                ex.Err(med);
                return null;
            }
        }

        #endregion

        #region Get Schema

        /// <summary>
        /// Get Schema. This Method is used for Get Schema information for specificed Meta Data information.
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="value">specificed Metadata to find information</param>
        /// <param name="restrictions">Restriction Array</param>
        /// <returns>Information about specificed metadata</returns>
        protected DataTable _GetDbSchema(TConnection connection,
            NDbMetaData value, NDbRestriction[] restrictions)
        {
            if (value == null)
                return null;
            if (connection == null ||
                connection.State != ConnectionState.Open)
                return null;
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                DataTable table = null;
                if (restrictions == null || restrictions.Length <= 0)
                    table = connection.GetSchema(value.CollectionName);
                else
                {
                    string[] restrictionValues = new string[restrictions.Length];
                    for (int i = 0; i < restrictions.Length; i++)
                    {
                        if (restrictions[i].Value != string.Empty)
                            restrictionValues[i] = restrictions[i].Value;
                    }
                    table = connection.GetSchema(value.CollectionName, restrictionValues);
                }

                return table;
            }
            catch (Exception ex)
            {
                "GetSchema error.".Err(med);
                ex.Err(med);
                return null;
            }
        }

        #endregion

        #region Get Tables (and views)

        /// <summary>
        /// Get Tables (and views)
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="owner">Owner of Tables/Views</param>
        /// <returns>List of all avaliable Tables/Views from provider.</returns>
        protected List<NDbTable> _GetDbTables(TConnection connection, string owner)
        {
            if (connection == null ||
                connection.State != ConnectionState.Open)
                return null;

            MethodBase med = MethodBase.GetCurrentMethod();
            List<NDbTable> results = null;
            DataTable table;
            NSchemaInfo<NDbTable> info;
            try
            {
                results = new List<NDbTable>();

                #region Table

                info = this.GetTableSchemaInfo(owner);
                if (null != info && null != info.MetaData && null != info.Restrictions ||
                    null != info.Convert)
                {
                    table = GetSchema(connection,
                        info.MetaData,
                        info.Restrictions);
                    if (table != null && table.Rows != null && table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            NDbTable val = info.Convert(row, info, this);
                            if (null != val)
                            {
                                val.Lock(); // lock object
                                results.Add(val);
                            }
                        }
                    }
                    else
                    {
                        "The table schema no data return.".Info(med);
                    }

                    try
                    {
                        if (table != null) table.Dispose();
                    }
                    catch { }
                }
                else
                {
                    "The table schema info is null.".Info(med);
                }

                #endregion

                #region View

                info = GetViewSchemaInfo(owner);
                if (null != info && null != info.MetaData && null != info.Restrictions ||
                    null != info.Convert)
                {
                    table = GetSchema(connection,
                        info.MetaData,
                        info.Restrictions);
                    if (table != null && table.Rows != null && table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            NDbTable val = info.Convert(row, info, this);
                            if (null != val)
                            {
                                val.Lock(); // lock object
                                results.Add(val);
                            }
                        }
                    }
                    else
                    {
                        "The view schema no data return.".Info(med);
                    }
                }
                else
                {
                    "The view schema info is null.".Info(med);
                }

                #endregion
            }
            catch (Exception ex)
            {
                "Get Tables Error.".Err(med);
                ex.Err(med);
                return null;
            }

            return results;
        }

        #endregion

        #region GetProcedureNames

        /// <summary>
        /// GetProcedureSchemaInfo
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns>Returns ProcedureSchemaInfo instance.</returns>
        protected virtual NProcedureSchemaInfo GetProcedureSchemaInfo(string owner)
        {
            return null;
        }
        /// <summary>
        /// Get Procedure Names.
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="owner">Owner of Stored Procedures</param>
        /// <returns>Returns List of Stored Procedure's name.</returns>
        protected List<string> _GetProcedures(TConnection connection,
            string owner)
        {
            List<string> results = null;

            if (connection == null ||
                connection.State != ConnectionState.Open)
                return null;

            NProcedureSchemaInfo info = this.GetProcedureSchemaInfo(owner);
            if (null == info)
                return null;

            NDbMetaData meta = new NDbMetaData("Procedures");
            NDbRestriction[] restrictions = info.Restrictions;

            DataTable table = GetSchema(connection, meta, restrictions);

            if (table != null && table.Rows != null && table.Rows.Count > 0)
            {
                results = new List<string>();
                foreach (DataRow row in table.Rows)
                {
                    if (null != row[info.ProcedureNameColumn] &&
                        row[info.ProcedureNameColumn] != DBNull.Value)
                    {
                        // Add to list
                        results.Add(row[info.ProcedureNameColumn].ToString());
                    }
                }
            }

            return results;
        }

        #endregion

        #region GetProcedureInfo

        /// <summary>
        /// Gets Stored Procedure Information.
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="owner">Owner of Stored Procedure</param>
        /// <param name="procedureName">The Stored Procedure Name.</param>
        /// <returns>Returns the Stored Procedure Information.</returns>
        protected NDbProcedureInfo _GetProcedureInfo(TConnection connection,
            string owner, string procedureName)
        {
            NDbProcedureInfo result = null;
            if (connection == null ||
                connection.State != ConnectionState.Open)
                return null;
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                TCommand command = (TCommand)connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = procedureName;
                if (this.DerivedParameters(command))
                {
                    // Create result instance.
                    result = new NDbProcedureInfo(procedureName);

                    #region Parameters

                    // Local var
                    NDbProcedureParameterInfo paraInfo;
                    // Each parameters
                    foreach (DbParameter para in command.Parameters)
                    {
                        string paraName = para.ParameterName.Replace(
                            this.GetParameterPrefix(), string.Empty);
                        Type paraType = this.ChangeType(para.DbType);
                        ParameterDirection direction = para.Direction;
                        // create new instance.
                        paraInfo = new NDbProcedureParameterInfo(paraName,
                            paraType, direction);
                        // keep to list
                        result.Parameters.Add(paraInfo);

                        // Initialize parameter value for execute later to retrived result
                        if (para.Direction == ParameterDirection.Input ||
                            para.Direction == ParameterDirection.InputOutput)
                        {
                            // assignd value.
                            para.Value = DBNull.Value;
                        }
                    }

                    #endregion

                    #region Test SP for Result

                    // Test Run SP to retrived result table
                    DataTable table = null;
                    DbDataAdapter adaptor = null;

                    try
                    {
                        #region Fill Table

                        adaptor = this.CreateAdapter(command);
                        if (null != adaptor)
                        {
                            table = new DataTable();
                            adaptor.Fill(table);
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        "Fill adaptor detected exception.".Err(med);
                        ex.Err(med);
                    }
                    finally
                    {
                        #region Free

                        if (null != adaptor)
                        {
                            try
                            {
                                adaptor.Dispose();
                            }
                            catch (Exception ex)
                            {
                                "Dispose adaptor detected exception.".Err(med);
                                ex.Err(med);
                            }
                        }
                        adaptor = null;

                        #endregion
                    }

                    #endregion

                    #region Results

                    if (null != table && table.Columns.Count > 0)
                    {
                        // Local var
                        NDbProcedureResultInfo resultInfo;
                        foreach (DataColumn col in table.Columns)
                        {
                            // Create result instance
                            resultInfo = new NDbProcedureResultInfo(col.ColumnName,
                                col.DataType);
                            // Add to list.
                            result.Results.Add(resultInfo);
                        }
                    }

                    if (null != table)
                    {
                        table.Dispose();
                    }
                    table = null;

                    #endregion
                }
            }
            catch (Exception ex)
            {
                "Get Procedure Info Error.".Err(med);
                ex.Err(med);
                return null;
            }

            return result;
        }

        #endregion

        #endregion

        #endregion

        #region Abstract Implements

        #region Connection

        /// <summary>
        /// Get New Connection Instance.
        /// </summary>
        /// <returns>
        /// Returns Connection instance that inherited from DbConnection class.
        /// </returns>
        public override DbConnection CreateConnection()
        {
            return this._CreateDbConnection();
        }

        #endregion

        #region Commands

        /// <summary>
        /// Create Command.
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="transaction">Transaction instance</param>
        /// <returns>
        /// Returns DbCommand instance.
        /// </returns>
        public override DbCommand CreateCommand(DbConnection connection,
            DbTransaction transaction)
        {
            if (null == connection || !(connection is TConnection) ||
                connection.State != ConnectionState.Open)
                return null;
            if (null != transaction && transaction is TTransaction)
                return _CreateDbCommand(connection as TConnection, transaction as TTransaction);
            else return _CreateDbCommand(connection as TConnection, null);
        }
        /// <summary>
        /// Create Data Adaptor for command (Select)
        /// </summary>
        /// <param name="command">The command object instance.</param>
        /// <returns>
        /// Returns DbDataAdapter instance that match command object.
        /// </returns>
        public override DbDataAdapter CreateAdapter(DbCommand command)
        {
            if (command == null || !(command is TCommand) || command.Connection == null ||
                command.Connection.State != ConnectionState.Open)
                return null;

            return _CreateDbAdapter(command as TCommand);
        }
        /// <summary>
        /// Derived Parameters
        /// </summary>
        /// <param name="command">Stored Procedure's command object instance.</param>
        /// <returns>Returns true if command is successfully retrived.</returns>
        public override bool DerivedParameters(DbCommand command)
        {
            if (command == null || !(command is TCommand) || command.Connection == null ||
                command.Connection.State != ConnectionState.Open)
                return false;

            return _DerivedDbParameters(command as TCommand);
        }
        /// <summary>
        /// Assign Parameters
        /// </summary>
        /// <param name="command">Stored Procedure's command object instance.</param>
        /// <param name="parameterNames">array of Parameter's name</param>
        /// <param name="parameterValues">array of Parameter's value</param>
        /// <returns>Returns true if successfully assigned parameters</returns>
        public override bool AssignParameters(DbCommand command,
            string[] parameterNames, object[] parameterValues)
        {
            if (command == null || !(command is TCommand) || command.Connection == null ||
                command.Connection.State != ConnectionState.Open)
                return false;

            return _AssignDbParameters(command as TCommand, parameterNames, parameterValues);
        }
        /// <summary>
        /// Create Parameter. 
        /// Used this method instead of call create parameter directly from DbCommand.
        /// </summary>
        /// <param name="command">The command object instance.</param>
        /// <param name="parameterName">The parameter's name.</param>
        /// <param name="providerDbTypeID">The Provider DataType ID</param>
        /// <param name="size">Set the size.</param>
        /// <param name="precision">Set the precision.</param>
        /// <param name="scale">Set the scale.</param>
        /// <param name="value">Set the object to assign to the parameter.</param>
        /// <returns>Returns the instance object that inherited from DbParameter.</returns>
        public override DbParameter CreateParameter(
            DbCommand command,
            string parameterName,
            int providerDbTypeID,
            int size, byte precision, byte scale,
            object value)
        {
            if (command == null || !(command is TCommand) || command.Connection == null ||
                command.Connection.State != ConnectionState.Open)
                return null;

            return _CreateDbParameter(command as TCommand, parameterName, providerDbTypeID,
                size, precision, scale, value);
        }
        /// <summary>
        /// Create New instance of DbParameter.
        /// </summary>
        /// <param name="parameterName">The parameter's name.</param>
        /// <param name="value">The parameter's value.</param>
        /// <returns>Returns instance of DbParameter.</returns>
        public override DbParameter CreateParameter(string parameterName, object value)
        {
            return _CreateDbParameter(parameterName, value);
        }

        #endregion

        #region Schemas

        #region Get Metadata

        /// <summary>
        /// Get Metadata
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <returns>List of all avaliable metadata from provider.</returns>
        public override List<NDbMetaData> GetMetadata(DbConnection connection)
        {
            if (null == connection || !(connection is TConnection) ||
                connection.State != ConnectionState.Open)
                return null;
            return this._GetDbMetadata(connection as TConnection);
        }

        #endregion

        #region Get Restrictions

        /// <summary>
        /// Get Restriction for specificed Metadata
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="value">specificed Metadata to find restriction information</param>
        /// <returns>List of restriction information related to specificed Metadata</returns>
        public override List<NDbRestriction> GetRestrictions(DbConnection connection,
            NDbMetaData value)
        {
            if (null == connection || !(connection is TConnection) ||
                connection.State != ConnectionState.Open)
                return null;
            return this._GetDbRestrictions(connection as TConnection, value);
        }

        #endregion

        #region Get Schema

        /// <summary>
        /// Get Schema. This Method is used for Get Schema information for specificed Meta Data information.
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="value">specificed Metadata to find information</param>
        /// <param name="restrictions">Restriction Array</param>
        /// <returns>Information about specificed metadata</returns>
        public override DataTable GetSchema(DbConnection connection,
            NDbMetaData value, NDbRestriction[] restrictions)
        {
            if (null == connection || !(connection is TConnection) ||
                connection.State != ConnectionState.Open)
                return null;
            return this._GetDbSchema(connection as TConnection, value, restrictions);
        }

        #endregion

        #region Get Tables (and views)

        /// <summary>
        /// Get Tables (and views)
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="owner">Owner of Tables/Views</param>
        /// <returns>List of all avaliable Tables/Views from provider.</returns>
        public override List<NDbTable> GetTables(DbConnection connection, string owner)
        {
            if (null == connection || !(connection is TConnection) ||
                connection.State != ConnectionState.Open)
                return null;

            return _GetDbTables(connection as TConnection, owner);
        }

        #endregion

        #region GetProcedures

        /// <summary>
        /// Get Procedures.
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="owner">Owner of Stored Procedures</param>
        /// <returns>Returns List of Stored Procedure's name.</returns>
        public override List<string> GetProcedures(DbConnection connection, string owner)
        {
            if (null == connection || !(connection is TConnection) ||
                connection.State != ConnectionState.Open)
                return null;

            return _GetProcedures(connection as TConnection, owner);
        }

        #endregion

        #region GetProcedureInfo

        /// <summary>
        /// Gets Stored Procedure Information.
        /// </summary>
        /// <param name="connection">Connection instance</param>
        /// <param name="owner">Owner of Stored Procedure</param>
        /// <param name="procedureName">The Stored Procedure Name.</param>
        /// <returns>Returns the Stored Procedure Information.</returns>
        public override NDbProcedureInfo GetProcedureInfo(DbConnection connection,
            string owner, string procedureName)
        {
            if (null == connection || !(connection is TConnection) ||
                connection.State != ConnectionState.Open)
                return null;

            return _GetProcedureInfo(connection as TConnection, owner, procedureName);
        }

        #endregion

        #endregion

        #endregion
    }

    #endregion
}
