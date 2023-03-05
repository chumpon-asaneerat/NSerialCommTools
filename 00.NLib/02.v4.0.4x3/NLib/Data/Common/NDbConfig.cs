#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-12-25
=================
- Data Access Framework - ConnectionConfig and ConnectionStrings
  - Fixed bug in Parse method. Ignore space character checks because some key has 
    space character. For example the MS Access Connection String has Key = 'Engine Type' 
    so if ignore space the key will not parse properly. The below line is full connection
    string for MS Access (mdb).
    Provider=Microsoft.Jet.OLEDB.4.0;Jet OLEDB:Engine Type=5;Persist Security Info=False;

======================================================================================================================
Update 2013-12-07
=================
- Data Access Framework - ConnectionConfig and ConnectionStrings
  - Add Prepare method for preload assembly which required to load related assembly into
    AppDomain and can be access it's static method to get provider information.

======================================================================================================================
Update 2014-11-12
=================
- Data Access Framework - ConnectionConfig and ConnectionStrings
  - Create NDbConfig Stub class.

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
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using NLib.Design;

#endregion

namespace NLib.Data
{
    #region NDbConfig Common classes

    #region Consts for Connection Config property orders

    /// <summary>
    /// Connection Config Property Orders constants.
    /// </summary>
    public sealed class ConfigOrders
    {
        /// <summary>
        /// The Property Order for UniqueName. Value is -9.
        /// </summary>
        public const int UniqueName = -9;
        /// <summary>
        /// The Property Order for ConnectionString. Value is -8.
        /// </summary>
        public const int ConnectionString = -8;
        /// <summary>
        /// The Property Order for DataSource. Value is 1.
        /// </summary>
        public const int DataSource = 1;
        /// <summary>
        /// The Property Order for Header option. Value is 2.
        /// </summary>
        public const int Header = 2;
        /// <summary>
        /// The Property Order for Security. Value is 5.
        /// </summary>
        public const int Security = 5;
        /// <summary>
        /// The Property Order for Timeout. Value is 998.
        /// </summary>
        public const int Timeout = 998;
        /// <summary>
        /// The Property Order for Optional. Value is 999.
        /// </summary>
        public const int Optional = 999;
    }

    #endregion

    #region Common Enums for Data access

    #region Persist Security Info Mode

    /// <summary>
    /// Persist Security Info Mode
    /// </summary>
    public enum PersistSecurityMode
    {
        /// <summary>
        /// Use Default Value from Provider (not specificed in connection string)
        /// </summary>
        Default,
        /// <summary>
        /// Set Persist Security Info to yes in connection string.
        /// This mode security-sensitive informatio will persist as part of connection.
        /// </summary>
        Yes,
        /// <summary>
        /// Set Persist Security Info to no in connection string. 
        /// This mode security-sensitive information, such as the password, is not returned as part
        /// of the connection if the connection is open or has ever been in an open state.
        /// Resetting the connection string resets all connection string values including the password
        /// </summary>
        No
    }

    #endregion

    #region Authentication Mode

    /// <summary>
    /// Authentication Mode
    /// used for define Database Server Connection Authentication.
    /// 
    /// see Integrated Security -or- Trusted_Connection on connection string for more information
    /// </summary>
    public enum AuthenticationMode
    {
        /// <summary>
        /// Server Authentication Mode this mode will allow user connect using Database Server Account 
        /// such as 'sa' in sql server when used this mode Integrated Security -or- Trusted_Connection key 
        /// in connection string will set to false
        /// </summary>
        Server,
        /// <summary>
        /// Windows Authentication mode is mode that the connection will uses Windows account credentials 
        /// for authentication. in this mode Integrated Security -or- Trusted_Connection key in 
        /// connection string will set to true (or equivalent to SSPI)
        /// </summary>
        Windows
    }

    #endregion

    #endregion

    #region ConnectionSetting - For Quick and dirty save/load xml

    /// <summary>
    /// Connection Setting class.
    /// </summary>
    [Serializable]
    public class ConnectionSetting
    {
        #region Internal Variable

        private NDbConfig _config = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectionSetting() : base() { }
        /// <summary>
        /// Destructor
        /// </summary>
        ~ConnectionSetting()
        {
            _config = null;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Connection Config instance.
        /// </summary>
        [Category("Configs")]
        [Description("Get/Set Connection Config instance.")]
        [XmlElement]
        public NDbConfig Config { get { return _config; } set { _config = value; } }

        #endregion
    }

    #endregion

    #region Common Data access configuration models (Security, Timeout, Optional)

    #region NDbSecurityOptions

    /// <summary>
    /// The Database Connection's Security options.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public class NDbSecurityOptions
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NDbSecurityOptions()
            : base()
        {
            this.Domain = string.Empty;
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.PersistSecurity = PersistSecurityMode.Default;
            this.Authentication = AuthenticationMode.Server;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets Hash Code.
        /// </summary>
        /// <returns>Returns hashcode of current object.</returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        /// <summary>
        /// Conpare if object is equals.
        /// </summary>
        /// <param name="obj">The target objct to compare.</param>
        /// <returns>Returns true if object is the same.</returns>
        public override bool Equals(object obj)
        {
            if (null == obj || obj.GetType() != this.GetType())
                return false;
            return this.GetHashCode().Equals(obj.GetHashCode());
        }
        /// <summary>
        /// ToString.
        /// </summary>
        /// <returns>Returns string that represents an object.</returns>
        public override string ToString()
        {
            if (this.Domain.IsNullOrWhiteSpace())
            {
                return string.Format(@"{0}:{1}, Mode:{2}-{3}",
                    this.UserName, this.Password,
                    this.PersistSecurity, this.Authentication);
            }
            else
            {
                return string.Format(@"{0}\{1}:{2}, Mode:{3}-{4}",
                    this.Domain,
                    this.UserName, this.Password,
                    this.PersistSecurity, this.Authentication);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Domain Name.
        /// </summary>
        [Category("Security")]
        [Description("Gets or sets Domain Name.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [PropertyOrder(1)]
        public string Domain { get; set; }
        /// <summary>
        /// Gets or sets User Name.
        /// </summary>
        [Category("Security")]
        [Description("Gets or sets User Name.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [PropertyOrder(2)]
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets Password.
        /// </summary>
        [Category("Security")]
        [Description("Gets or sets Password.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [PropertyOrder(3)]
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets Persist Security Info Mode.
        /// </summary>
        [Category("Security")]
        [Description("Gets or sets Persist Security Info Mode.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [PropertyOrder(4)]
        public PersistSecurityMode PersistSecurity { get; set; }
        /// <summary>
        /// Gets or sets the Authentication Mode.
        /// </summary>
        [Category("Security")]
        [Description("Gets or sets the Authentication Mode.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [PropertyOrder(5)]
        public AuthenticationMode Authentication { get; set; }

        #endregion
    }

    #endregion

    #region NDbTimeouts

    /// <summary>
    /// The Database Connection and Command Timeout options.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public class NDbTimeouts
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NDbTimeouts()
            : base()
        {
            this.ConnectionTimeoutInSeconds = 5;
            this.CommandTimeoutInSeconds = 30;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// ToString.
        /// </summary>
        /// <returns>Returns string that represents an object.</returns>
        public override string ToString()
        {
            return string.Format("Connection {0} s, Command {1} s",
                this.ConnectionTimeoutInSeconds,
                this.CommandTimeoutInSeconds);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Connection Timeout (default = 5 s)
        /// </summary>
        [Category("Security")]
        [Description("Gets or sets Connection Timeout (default = 5 s).")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [PropertyOrder(1)]
        public int ConnectionTimeoutInSeconds { get; set; }
        /// <summary>
        /// Gets or sets Command Timeout (default = 30 s).
        /// </summary>
        [Category("Security")]
        [Description("Gets or sets Command Timeout (default = 30 s).")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [PropertyOrder(2)]
        public int CommandTimeoutInSeconds { get; set; }

        #endregion
    }

    #endregion

    #region NDbOptions

    /// <summary>
    /// The Database Optional Connection Information.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public class NDbOptions
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NDbOptions()
            : base()
        {
            this.AutoDetectBroken = false;
            this.AutoDetectInterval = 30;
            this.ServerDateFormat = string.Empty;
            this.UseDateCache = true;
            this.EnableMARS = false;
            this.ExtendConnectionString = string.Empty;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// ToString.
        /// </summary>
        /// <returns>Returns string that represents an object.</returns>
        public override string ToString()
        {
            return string.Format("{0}", this.ExtendConnectionString);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets auto detect broken connection. Default is False.
        /// Note that when used ExecuteReader or ExecuteDataReader
        /// from NDbConnection please make sure to set this proeprty to false before execute and 
        /// set this property back to true when execute completed to prevent access connection that in
        /// execution process.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets auto detect broken connection. Note that when used ExecuteReader or ExecuteDataReader from NDbConnection please make sure to set this proeprty to false before execute and set this property back to true when execute completed to prevent access connection that in execution process.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [PropertyOrder(1)]
        public bool AutoDetectBroken { get; set; }
        /// <summary>
        /// Get/Set Auto detect broken Interval in second. (default 30 second.)
        /// </summary>
        [Category("Auto Detect")]
        [Description("Get/Set Auto detect broken Interval in second. (default 30 second.)")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [PropertyOrder(2)]
        public int AutoDetectInterval { get; set; }
        /// <summary>
        /// Gets or sets Server Date Format. When assigned if the connect config class is supports
        /// the date format will apply to server imediately after connected.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets Server Date Format. When assigned if the connect config class is supports the date format will apply to server imediately after connected.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [PropertyOrder(3)]
        public string ServerDateFormat { get; set; }
        /// <summary>
        /// Gets or sets to use cache for reduce traffic when sync date with server.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets to use cache for reduce traffic when sync date with server.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [PropertyOrder(4)]
        public bool UseDateCache { get; set; }
        /// <summary>
        /// Gets or sets to Enable Multiple Active Result Sets. Default is False.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets to Enable Multiple Active Result Sets. Default is False.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [PropertyOrder(5)]
        public bool EnableMARS { get; set; }
        /// <summary>
        /// Gets or sets Extended Connection String.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets Extended Connection String.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlAttribute]
        [PropertyOrder(10)]
        public string ExtendConnectionString { get; set; }

        #endregion
    }

    #endregion

    #endregion

    #region Connection String related classes

    #region NParameter
    
    /// <summary>
    /// The NParameter class. This class is key-value pair.
    /// </summary>
    [Serializable]
    public class NParameter
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NParameter() : this(string.Empty, string.Empty) { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">The parameter key or name.</param>
        /// <param name="value">The parameter value.</param>
        public NParameter(string key, string value) : base() 
        {
            this.Key = key;
            this.Value = value;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Equals.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (null == obj || obj.GetType() != typeof(NParameter))
                return false;
            return this.GetHashCode().Equals(obj.GetHashCode());
        }
        /// <summary>
        /// GetHashCode.
        /// </summary>
        /// <returns>Returns hash code.</returns>
        public override int GetHashCode()
        {
            return this.Key.ToLower().GetHashCode();
        }
        /// <summary>
        /// ToString.
        /// </summary>
        /// <returns>Returns string that represents and object instance.</returns>
        public override string ToString()
        {
            return string.Format("{0} = {1}", this.Key, this.Value);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets key.
        /// </summary>
        [Category("Parameters")]
        [Description("Gets or sets key.")]
        [XmlAttribute]
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets value.
        /// </summary>
        [Category("Parameters")]
        [Description("Gets or sets value.")]
        [XmlAttribute]
        public string Value { get; set; }

        #endregion
    }

    #endregion

    #region ConnectionStrings

    /// <summary>
    /// The ConnectionStrings class. This class is provides functional to parse the connection string in to
    /// list of NParameter and combine list of NParameter back into single connection string.
    /// </summary>
    public class ConnectionStrings
    {
        #region Internal Variables

        private BindingList<NParameter> _parameters = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConnectionStrings()
            : base()
        {
            lock (this)
            {
                _parameters = new BindingList<NParameter>();
            }
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~ConnectionStrings()
        {
            if (null != _parameters)
            {
                lock (this)
                {
                    _parameters.Clear();
                }
            }
            _parameters = null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Find index of parameter.
        /// </summary>
        /// <param name="value">The parameter instance.</param>
        /// <returns>Returns index of parameter. If not exists -1 would returns.</returns>
        private int FindIndex(NParameter value)
        {
            if (null == _parameters)
            {
                lock (this)
                {
                    _parameters = new BindingList<NParameter>();
                }
            }
            int index = -1;
            lock (this)
            {
                index = _parameters.IndexOf(value);
            }
            return index;
        }
        /// <summary>
        /// Update Parameter.
        /// </summary>
        /// <param name="value">The parameter instance.</param>
        private void Update(NParameter value)
        {
            if (null == value || string.IsNullOrWhiteSpace(value.Key))
                return;
            int index = FindIndex(value);
            if (index == -1)
            {
                // not exists
                lock (this)
                {
                    _parameters.Add(value);
                }
            }
            else
            {
                // exists
                lock (this)
                {
                    _parameters[index] = value;
                }
            }
        }
        /// <summary>
        /// Update Parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private void Update(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            NParameter inst = new NParameter(key, value);
            int index = FindIndex(inst);
            if (index == -1)
            {
                // not exists
                lock (this)
                {
                    _parameters.Add(inst);
                }
            }
            else
            {
                // exists
                lock (this)
                {
                    _parameters[index] = inst;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clear all parameters.
        /// </summary>
        public void Clear()
        {
            if (null == _parameters)
            {
                lock (this)
                {
                    _parameters = new BindingList<NParameter>();
                }
            }
            _parameters.Clear();
        }
        /// <summary>
        /// Gets Connection String.
        /// </summary>
        /// <returns>Returns string that represents all parameters in parameter list.</returns>
        public string GetConnectionString()
        {
            string result = string.Empty;

            if (null == _parameters)
            {
                lock (this)
                {
                    _parameters = new BindingList<NParameter>();
                }
            }
            if (_parameters.Count <= 0)
                return result;

            foreach (NParameter para in _parameters)
            {
                if (string.IsNullOrEmpty(para.Key))
                    continue;
                if (string.IsNullOrEmpty(para.Value))
                    continue;

                result += string.Format("{0}={1};", para.Key, para.Value);
            }

            return result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Indexer access Value by key.
        /// </summary>
        /// <param name="key">The key to search index.</param>
        /// <returns>
        /// Returns value that match key. If not exists new item is auto append with empty string otherwise 
        /// the exists value would returns.
        /// </returns>
        public string this[string key]
        {
            get
            {
                NParameter inst = new NParameter(key, string.Empty);
                int index = this.FindIndex(inst);
                if (index == -1)
                    this.Update(inst);
                else inst = _parameters[index];

                return inst.Value;
            }
            set
            {
                Update(key, value);
            }
        }
        /// <summary>
        /// Access List of parameters.
        /// </summary>
        [Category("Connection Strings")]
        [Description("Access List of parameters.")]
        public BindingList<NParameter> Parameters
        {
            get
            {
                if (null == _parameters)
                {
                    lock (this)
                    {
                        _parameters = new BindingList<NParameter>();
                    }
                }
                return _parameters;
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Parse the connection string into ConnectionStrings instance.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>Returns ConnectionStrings instance.</returns>
        public static ConnectionStrings Parse(string connectionString)
        {
            ConnectionStrings result = new ConnectionStrings();

            if (string.IsNullOrWhiteSpace(connectionString))
                return result;

            string[] lines = connectionString.Split(new char[] 
                { 
                    ';', '\r', '\n', '\t' 
                }, StringSplitOptions.RemoveEmptyEntries);
            if (null != lines && lines.Length > 0)
            {
                foreach (string line in lines)
                {
                    int eqIndex = line.Trim().IndexOf("=");
                    if (eqIndex <= 0)
                        continue;

                    string key = string.Empty;
                    string value = string.Empty;
                    // Extract key
                    try
                    {
                        key = line.Substring(0, eqIndex).Trim();
                    }
                    catch { key = string.Empty; }
                    // Extract value
                    try
                    {
                        value = line.Substring(eqIndex + 1, line.Length - eqIndex - 1).Trim();
                    }
                    catch { value = string.Empty; }

                    if (string.IsNullOrEmpty(key))
                        continue;

                    NParameter inst = new NParameter(key, value);
                    // Add results if not exists
                    if (!result.Parameters.Contains(inst))
                        result.Parameters.Add(inst);
                }
            }

            return result;
        }

        #endregion
    }

    #endregion

    #endregion

    #endregion
}

namespace NLib.Data
{
    #region NDbConfig abstract class

    /// <summary>
    /// NDbConfig abstract class
    /// </summary>
    [Serializable]
    //[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public abstract class NDbConfig
    {
        #region Internal Variables

        private NDbFactory _connectionFactory = null;
        private ConnectionStrings _connectionStrings = null;
        // common properties for all connection config.
        private NDbTimeouts _timeouts = null;
        private NDbSecurityOptions _security = null;
        private NDbOptions _optionals = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NDbConfig()
            : base()
        {
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NDbConfig()
        {
            _connectionStrings = null;
            _connectionFactory = null;
            
            _timeouts = null;
            _security = null;
            _optionals = null;
            // Force free memory
            NGC.FreeGC(this);
        }

        #endregion

        #region Abstract Methods

        #region GetUniqueName/GetConnectionString/GetFactory

        /// <summary>
        /// Define Each Connection Unique Name
        /// </summary>
        /// <returns>Unique Name for Connection</returns>
        protected abstract string GetUniqueName();
        /// <summary>
        /// Get Connection String
        /// </summary>
        /// <returns>Connection string based on property settings</returns>
        protected abstract string GetConnectionString();
        /// <summary>
        /// Create database factory provider.
        /// </summary>
        /// <returns>Returns instance of database factory provider.</returns>
        protected abstract NDbFactory CreateFactory();

        #endregion

        #endregion

        #region Protected Methods

        #region Config methods
        
        /// <summary>
        /// Call to raised event ConfigChanged
        /// </summary>
        protected void RaiseConfigChanged()
        {
            //_connectionFactory = null; // Reset connection factory
            OnConfigChanged();

            ConfigChanged.Call(this, EventArgs.Empty);
        }
        /// <summary>
        /// Overrides to handles internal data when the one or more connection configuration 
        /// is changed. This methods is called before event is raises.
        /// </summary>
        protected virtual void OnConfigChanged() { }

        #endregion

        #region Helper methods
        
        /// <summary>
        /// Check the variable instance. If variable is null the new instance is auto created.
        /// </summary>
        /// <typeparam name="T">The variable type. Should be class and has default constructor.</typeparam>
        /// <param name="value">The variable instance.</param>
        /// <param name="initAction">The Action delegate for init default value of new object.</param>
        /// <returns>
        /// Returns the variable instance if the value is not null otherwise return new instance.
        /// </returns>
        protected T CheckVar<T>(T value, Action<T> initAction = null)
            where T : class, new()
        {
            T result = value;
            if (null == value)
            {
                lock (this)
                {
                    result = new T();
                    if (null != initAction)
                    {
                        // call init for setup default properties of new object.
                        initAction(result);
                    }
                }
            }
            return result;
        }

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets database factory provider.
        /// </summary>
        /// <returns>Returns instance of database factory provider.</returns>
        protected virtual NDbFactory GetFactory()
        {
            if (null == _connectionFactory)
            {
                lock (this)
                {
                    _connectionFactory = this.CreateFactory();
                }
            }
            return _connectionFactory;
        }
        /// <summary>
        /// Set database factory provider.
        /// </summary>
        /// <param name="value">The database factory provider instance.</param>
        protected virtual void SetFactory(NDbFactory value)
        {
            lock (this)
            {
                _connectionFactory = value;
                if (null != _connectionFactory)
                {
                    _connectionFactory.SetConfig(this);
                }
            }
            RaiseConfigChanged();
        }

        #endregion

        #region Connection methods

        /// <summary>
        /// Gets Connection String Builders.
        /// </summary>
        protected ConnectionStrings ConnectionStrings
        {
            get
            {
                if (null == _connectionStrings)
                {
                    lock (this)
                    {
                        _connectionStrings = new ConnectionStrings();
                    }
                }
                return _connectionStrings;
            }
        }

        #endregion

        #region Defaults

        /// <summary>
        /// Gets the default database object owner.
        /// </summary>
        /// <returns>Returns the default database object owner.</returns>
        protected virtual string GetDefaultOwner()
        {
            return string.Empty;
        }

        #endregion

        #endregion

        #region Public Methods

        #region SaveToFile

        /// <summary>
        /// Quick and dirty save current config to file.
        /// </summary>
        /// <param name="fileName">Target config file name.</param>
        /// <returns>Return true if save operation is success.</returns>
        public bool SaveToFile(string fileName)
        {
            return NDbConfig.SaveToFile(fileName, this);
        }

        #endregion

        #endregion

        #region Public Properties

        #region ReadOnly Properties
        
        /// <summary>
        /// Gets Provider's Name.
        /// </summary>
        [Category("Connection")]
        [Description("Gets Provider's Name")]
        [Browsable(true)]
        [XmlIgnore]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ProviderName
        {
            get
            {
                return NLib.Reflection.TypeUtils.GetReferenceName(this, staticPropertyName);
            }
        }
        /// <summary>
        /// Gets the connection unique name.
        /// </summary>
        [Category("Connection")]
        [Description("Gets the connection unique name.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlIgnore]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [PropertyOrder(ConfigOrders.UniqueName)]
        public string UniqueName { get { return this.GetUniqueName(); } }
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        [Category("Connection")]
        [Description("Gets the connection string.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlIgnore]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [PropertyOrder(ConfigOrders.ConnectionString)]
        public string ConnectionString { get { return this.GetConnectionString(); } }
        /// <summary>
        /// Gets the default owner.
        /// </summary>
        [Category("Connection")]
        [Description("Gets the default owner.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlIgnore]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [PropertyOrder(ConfigOrders.ConnectionString)]
        public string DefaultOwner { get { return GetDefaultOwner(); } }
        /// <summary>
        /// Gets the Connection Factory.
        /// </summary>
        [Category("Connection")]
        [Description("Gets the connection factory.")]
        [Browsable(false)]
        [XmlIgnore]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NDbFactory Factory { get { return this.GetFactory(); } }

        #endregion

        #region Security

        /// <summary>
        /// Gets or sets Security information.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets Security information.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlElement]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PropertyOrder(ConfigOrders.Security)]
        public NDbSecurityOptions Security
        {
            get
            {
                _security = this.CheckVar(_security);
                return _security;
            }
            set
            {
                _security = value;
                _security = this.CheckVar(_security);
                RaiseConfigChanged();
            }
        }

        #endregion

        #region Timeout

        /// <summary>
        /// Gets or sets Timeout information.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets Timeout information.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlElement]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PropertyOrder(ConfigOrders.Timeout)]
        public NDbTimeouts Timeout
        {
            get
            {
                _timeouts = this.CheckVar(_timeouts);
                return _timeouts;
            }
            set
            {
                _timeouts = value;
                _timeouts = this.CheckVar(_timeouts);
                RaiseConfigChanged();
            }
        }

        #endregion

        #region Optional

        /// <summary>
        /// Gets or sets Optional information.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets Optional information.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlElement]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PropertyOrder(ConfigOrders.Optional)]
        public NDbOptions Optional
        {
            get
            {
                _optionals = this.CheckVar(_optionals);
                return _optionals;
            }
            set
            {
                _optionals = value;
                _optionals = this.CheckVar(_optionals);
                RaiseConfigChanged();
            }
        }

        #endregion

        #endregion

        #region Public Events

        /// <summary>
        /// ConnectionConfigChanged event. Raised when some connection config is changed.
        /// </summary>
        [Category("Connection")]
        [Description("ConnectionConfigChanged event. Raised when some connection config is changed.")]
        public event System.EventHandler ConfigChanged;

        #endregion

        #region Static Methods

        #region Providers

        #region Static variables

        private static Type _baseType = typeof(NDbConfig);
        private static string staticPropertyName = "DbProviderName";

        #endregion

        #region Create NDbConfig By Provider Name

        /// <summary>
        /// Create Connection Config By Provider Name
        /// </summary>
        /// <param name="providerName">Provider Name</param>
        /// <returns>NDbConfig's Instance</returns>
        public static NDbConfig Create(string providerName)
        {
            NDbConfig instance = null;
            Type[] types = GetDbProviders();
            if (types == null || types.Length <= 0)
                instance = null;
            else
            {
                object val = NLib.Reflection.TypeUtils.Create(_baseType, staticPropertyName,
                    providerName, "Create");
                if (val != null && val.GetType().IsSubclassOf(_baseType))
                    instance = (val as NDbConfig);
                else return instance;
            }
            return instance;
        }

        #endregion

        #region Get Provider Names

        /// <summary>
        /// Get Provider Names
        /// </summary>
        /// <returns>ConnectionConfig Provider's Names</returns>
        public static string[] GetProviderNames()
        {
            return NLib.Reflection.TypeUtils.GetInheritedTypeNames(_baseType, staticPropertyName);
        }

        #endregion

        #region Get Providers

        /// <summary>
        /// Get DbProviders
        /// </summary>
        /// <returns>List of Avaliable Type that inherited from ConnectionConfig</returns>
        public static Type[] GetDbProviders()
        {
            return NLib.Reflection.TypeUtils.GetInheritedTypes(_baseType);
        }
        /// <summary>
        /// Get DbProviders
        /// </summary>
        /// <param name="refresh">refresh cache</param>
        /// <returns>List of Avaliable Type that inherited from ConnectionConfig</returns>
        public static Type[] GetDbProviders(bool refresh)
        {
            return NLib.Reflection.TypeUtils.GetInheritedTypes(_baseType, refresh);
        }

        #endregion

        #region Get Provider Name

        /// <summary>
        /// Get Db Provider Name
        /// </summary>
        public static string DbProviderName
        {
            get { return "Not Specificed"; }
        }

        #endregion

        #endregion

        #region Create

        /// <summary>
        /// Create new NDbConfig Instance
        /// </summary>
        /// <returns>Returns NDbConfig Instance.</returns>
        public static NDbConfig Create() { return null; }

        #endregion

        #region Prepare Methods

        /// <summary>
        /// Prepare Factory.
        /// </summary>
        public static void Prepare() { }

        #endregion

        #region Load/Seve

        /// <summary>
        /// Quick and dirty save config to file.
        /// </summary>
        /// <param name="fileName">Target config file name.</param>
        /// <param name="config">The config instance to save.</param>
        /// <returns>Return true if save operation is success.</returns>
        public static bool SaveToFile(string fileName, NDbConfig config)
        {
            // keep original
            Type[] types = NLib.Xml.XmlManager.ExtraTypes;
            // load provider types
            NLib.Xml.XmlManager.ExtraTypes = GetDbProviders();

            ConnectionSetting setting = new ConnectionSetting();
            setting.Config = config;

            bool result =
                NLib.Xml.XmlManager.SaveToFile<ConnectionSetting>(fileName, setting);

            // restore type;
            NLib.Xml.XmlManager.ExtraTypes = types;

            return result;
        }
        /// <summary>
        /// Quick and dirty load The config from File.
        /// </summary>
        /// <param name="fileName">Target config file name.</param>
        /// <returns>Return new instance of connection config is operation is success.</returns>
        public static NDbConfig LoadFromFile(string fileName)
        {
            // keep original
            Type[] types = NLib.Xml.XmlManager.ExtraTypes;
            // load provider types
            NLib.Xml.XmlManager.ExtraTypes = GetDbProviders();

            NDbConfig result = null;

            ConnectionSetting obj =
                NLib.Xml.XmlManager.LoadFromFile<ConnectionSetting>(fileName);
            if (null != obj)
            {
                result = (obj as ConnectionSetting).Config;
            }

            // restore type;
            NLib.Xml.XmlManager.ExtraTypes = types;

            return result;
        }

        #endregion

        #endregion
    }

    #endregion
}
