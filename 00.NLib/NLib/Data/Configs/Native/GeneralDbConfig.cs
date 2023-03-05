#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-12-07
=================
- DataAccess : GeneralDbConfig
  - Re-Implement Database Config classes for General Connection string.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using NLib;
using NLib.Design;
using NLib.Data;
using NLib.Data.Design;
using NLib.Reflection;
using NLib.Xml;
using NLib.Utils;

#endregion

namespace NLib.Data.Design
{

}

namespace NLib.Data
{
    #region GeneralDbConfig
    
    /// <summary>
    /// The General Database Config class. Used to wrap around connection string.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public class GeneralDbConfig : NDbConfig
    {
        #region Internal Variables

        private string _connectionName = string.Empty;
        private string _connectionString = string.Empty;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public GeneralDbConfig()
            : base()
        {
            _connectionName = GeneralDbConfig.GetDefaultConnectionName();
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~GeneralDbConfig()
        {
            
        }

        #endregion

        #region Abstract Implements and Private methods for set properties

        #region GetUniqueName/GetConnectionString/GetFactory

        /// <summary>
        /// Define Each Connection Unique Name
        /// </summary>
        /// <returns>Unique Name for Connection</returns>
        protected override string GetUniqueName()
        {
            return _connectionName;
        }
        /// <summary>
        /// Set Unique Name.
        /// </summary>
        /// <param name="value">The new connection name.</param>
        private void SetUniqueName(string value)
        {
            if (_connectionName != value)
            {
                _connectionName = value;
                RaiseConfigChanged();
            }
        }
        /// <summary>
        /// Get Connection String.
        /// </summary>
        /// <returns>Connection string based on property settings</returns>
        protected override string GetConnectionString()
        {
            return _connectionString;
        }
        /// <summary>
        /// Set Connection String.
        /// </summary>
        /// <param name="value">The new connection string.</param>
        private void SetConnectionString(string value)
        {
            if (_connectionString != value)
            {
                _connectionString = value;
                RaiseConfigChanged();
            }
        }
        /// <summary>
        /// Create database factory provider.
        /// </summary>
        /// <returns>Returns instance of database factory provider.</returns>
        protected override NDbFactory CreateFactory()
        {
            return null;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the connection unique name.
        /// </summary>
        [Category("Connection")]
        [Description("Gets the connection unique name.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlIgnore]
        [PropertyOrder(ConfigOrders.UniqueName)]
        public new string UniqueName 
        { 
            get { return this.GetUniqueName(); }
            set { this.SetUniqueName(value); }
        }
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        [Category("Connection")]
        [Description("Gets the connection string.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlIgnore]
        [PropertyOrder(ConfigOrders.ConnectionString)]
        public new string ConnectionString 
        { 
            get { return this.GetConnectionString(); }
            set { this.SetConnectionString(value); }
        }
        /// <summary>
        /// Gets or sets the Connection Factory.
        /// </summary>
        [Category("Connection")]
        [Description("Gets the connection factory.")]
        [XmlIgnore]
        [Browsable(false)]
        public new NDbFactory Factory 
        { 
            get { return this.GetFactory(); }
            set { this.SetFactory(value); }
        }

        #endregion

        #region Static Methods

        #region Static Variable
        
        private static long connectionId = 1;

        #endregion

        #region Get Default Connection Name

        /// <summary>
        /// Get Default Connection Name.
        /// </summary>
        /// <returns>Returns Unique Default Connection Name.</returns>
        private static string GetDefaultConnectionName()
        {
            string result = "connection" + connectionId;
            lock (typeof(GeneralDbConfig))
            {
                if (connectionId + 1 >= long.MaxValue)
                    connectionId = 1;
                else ++connectionId;
            }
            return result;
        }

        #endregion

        #region Get Provider Name

        /// <summary>
        /// Get Connection Provider Name.
        /// </summary>
        public static new string DbProviderName
        {
            get { return "General Database Config (Native)"; }
        }

        #endregion

        #region Create

        /// <summary>
        /// Create new NDbConfig Instance.
        /// </summary>
        /// <returns>Returns NDbConfig Instance.</returns>
        public static new NDbConfig Create() { return new GeneralDbConfig(); }

        #endregion

        #endregion
    }

    #endregion
}
