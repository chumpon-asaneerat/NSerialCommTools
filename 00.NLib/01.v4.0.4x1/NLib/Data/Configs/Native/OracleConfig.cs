#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-12-07
=================
- DataAccess : OracleConfig
  - Re-Implement Database Config classes for Oracle.

======================================================================================================================
Update 2010-08-31
=================
- DataAccess : OracleDirect
  - In .NET 4.0 the OracleClient dll will deprecated. Need to implement 3rd party library.

======================================================================================================================
Update 2010-02-03
=================
- DataAccess : OracleDirect
  - OracleDirectConnectionFactory ported and re-implements uses new generic base class.
  - Oracle Direct sql model and related class ported.
  - Current Oracle Direct utils is not completed due to deployment problem.

======================================================================================================================
Update 2008-11-23
=================
- DataAccess : OracleDirect
  - Add try-catch in CreateParameter when set Precision/Scale.

======================================================================================================================
Update 2008-10-23
=================
- Sql Model (Native) updated.
  [OracleDirect]
  - Add support generate Sequence script.

======================================================================================================================
Update 2008-10-21
=================
- Sql Model (Native) updated.
  [OracleDirect]
  - Add new class OracleDirectSqlModel.OracleDirectDDLFormatter for handle DDL generate script.
  - Implement method CreateDDLFormatter.
  - Implement method GenerateViewScript (incompleted) in it's DDLFormatter.
  - Implement method GenerateTableScript in it's DDLFormatter.
  - Implement method GenerateTableColumnScript in it's DDLFormatter.
  - Implement method GenerateTableConstraintScript in it's DDLFormatter.

======================================================================================================================
Update 2008-10-20
=================
- DataAccess : OracleDirect
  - Fixed Connection check null bug (in case original code used logical operator && instead of ||).

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

#pragma warning disable 0618

namespace NLib.Data.Design
{

}

namespace NLib.Data
{
    #region Common classes and Enums

    #region Oracle Version Enum

    /// <summary>
    /// Oracle's Version
    /// </summary>
    public enum OracleVersion
    {
        /// <summary>
        /// Oracle 7.xx
        /// </summary>
        Oracle7x,
        /// <summary>
        /// Oracle 8i
        /// </summary>
        Oracle8i,
        /// <summary>
        /// Oracle 9i
        /// </summary>
        Oracle9i,
        /// <summary>
        /// Oracle 10g
        /// </summary>
        Oracle10g,
        /// <summary>
        /// Oracle 11g
        /// </summary>    
        Oracle11g,
        /// <summary>
        /// Oracle 12
        /// </summary>    
        Oracle12,
        /// <summary>
        /// Oracle XE
        /// </summary>    
        OracleXE,
        /// <summary>
        /// Oracle Lite
        /// </summary>    
        OracleLite
    }

    #endregion

    #region OracleProtocol
    
    /// <summary>
    /// The oracle protocol.
    /// </summary>
    public enum OracleProtocol
    {
        /// <summary>
        /// IPC. This Protocol type required Key that recommend to used Oracle SID (aka service name).
        /// </summary>
        ipc,
        /// <summary>
        /// Named pipes. This Protocol type required Host Name and Pipe name.
        /// </summary>
        nmp,
        /// <summary>
        /// SDP. This Protocol type required Host Name and Port Number.
        /// </summary>
        sdp,
        /// <summary>
        /// TCP/IP. This Protocol type required Host Name and Port Number.
        /// </summary>
        tcp,
        /// <summary>
        /// TCP/IP with SSL. This Protocol type required Host Name and Port Number.
        /// </summary>
        tcps
    }

    #endregion;

    #region Common classes for Serialization connection config

    /// <summary>
    /// The OracleServerOptions class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public class OracleServerOptions
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public OracleServerOptions()
            : base()
        {
            this.HostName = Environment.MachineName;
            this.ServiceName = "orcl";
            this.PortNumber = 1521;
            this.Protocol = OracleProtocol.tcp;
            this.Version = OracleVersion.Oracle11g;
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
            string result = string.Empty;

            switch (this.Protocol)
            {
                case OracleProtocol.ipc:
                    result = string.Format(@"{0}/{1}:{2}-({3})(v.{4})",
                        this.HostName,
                        this.ServiceName,
                        this.PortNumber,
                        this.Protocol,
                        this.Version);
                    break;
                case OracleProtocol.nmp:
                    result = string.Format(@"{0}/{1}:{2}-({3})(v.{4})",
                        this.HostName,
                        this.ServiceName,
                        this.PipeName,
                        this.Protocol,
                        this.Version);
                    break;
                case OracleProtocol.sdp:
                case OracleProtocol.tcp:
                case OracleProtocol.tcps:
                    result = string.Format(@"{0}/{1}:{2}-({3})(v.{4})",
                        this.HostName,
                        this.ServiceName,
                        this.PortNumber,
                        this.Protocol,
                        this.Version);
                    break;
            }

            return result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Oracle Server Name or Host Name (or IP).
        /// </summary>
        [Category("Database")]
        [Description("Gets or sets Oracle Server or Host Name (or IP).")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [PropertyOrder(1)]
        [XmlAttribute]
        public string HostName { get; set; }
        /// <summary>
        /// Gets or sets Oracle service name or Oracle SID.
        /// </summary>
        [Category("Database")]
        [Description("Gets or sets Oracle service name.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [PropertyOrder(2)]
        [XmlAttribute]
        public string ServiceName { get; set; }
        /// <summary>
        /// Gets or sets Port Number. Default is 1521.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets Port Number. Default is 1521.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [PropertyOrder(3)]
        [XmlAttribute]
        public int PortNumber { get; set; }
        /// <summary>
        /// Gets or sets the Oracle Protocol. Default is tcp.
        /// </summary>
        [Category("Database")]
        [Description("Gets or sets the Oracle Protocol. Default is tcp.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [PropertyOrder(5)]
        [XmlAttribute]
        public OracleProtocol Protocol { get; set; }
        /// <summary>
        /// Gets or sets the Oracle Version.
        /// </summary>
        [Category("Database")]
        [Description("Gets or sets the Oracle Version.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [PropertyOrder(5)]
        [XmlAttribute]
        public OracleVersion Version { get; set; }
        /// <summary>
        /// Gets or sets Pipe name.
        /// </summary>
        [Category("Database")]
        [Description("Gets or sets Pipe name.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [PropertyOrder(6)]
        [XmlAttribute]
        public string PipeName { get; set; }

        #endregion
    }

    #endregion

    #endregion

    #region OracleConfig

    /// <summary>
    /// Oracle Direct Connection Config class. This class will used System.Data.OracleClient to
    /// connect to database and support automatic assigned TNS without used of oracle client installed.
    /// This class can used with Oracle 10g and above.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public class OracleConfig : NDbConfig
    {
        #region Internal Variables

        private OracleServerOptions _datasource = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public OracleConfig()
            : base()
        {
            // set default user name, password and authentication mode.
            this.Security.UserName = "SYSTEM";
            this.Security.Password = string.Empty;
            this.Security.Authentication = AuthenticationMode.Server;
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~OracleConfig()
        {

        }

        #endregion

        #region Abstract Implements

        #region GetUniqueName/GetConnectionString/GetFactory

        /// <summary>
        /// Define Each Connection Unique Name.
        /// </summary>
        /// <returns>Unique Name for Connection</returns>
        protected override string GetUniqueName()
        {
            return this.DataSource.ToString();
        }
        /// <summary>
        /// Get Connection String.
        /// </summary>
        /// <returns>Connection string based on property settings</returns>
        protected override string GetConnectionString()
        {
            // Note: ODP.Net used difference connection string from Oracle Client.

            #region Sample of connection string

            /*
                      textBox1.Text = "SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=ORACLE10G)" +
                                        "(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=PLC_DB)));" +
                                        "User ID=PLC;Password=PLC;";
                      */

            #endregion

            string format = string.Empty;
            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(this.DataSource.HostName) ||
                string.IsNullOrWhiteSpace(this.DataSource.ServiceName) ||
                string.IsNullOrWhiteSpace(this.Security.UserName))
            {
                // Required information is not assigned.
                return result;
            }
            if (this.DataSource.Protocol == OracleProtocol.nmp &&
                string.IsNullOrWhiteSpace(this.DataSource.PipeName))
            {
                // Required information is not assigned.
                return result;
            }

            // Init TNS string
            switch (this.DataSource.Protocol)
            {
                case OracleProtocol.ipc:
                    // This may required tnsnames.ora
                    format = "SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL={0})(KEY={1}))" +
                        "(CONNECT_DATA=(SID={1})));";
                    result = format.args
                        (
                            this.DataSource.Protocol.ToString().ToUpper(),
                            this.DataSource.ServiceName.ToString().ToUpper()
                        );
                    break;
                case OracleProtocol.nmp:
                    // Not tested
                    format = "SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL={0})(SERVER={1})(PIPE={2}))" +
                        "(CONNECT_DATA=(SERVICE_NAME={3})));";
                    result = format.args
                        (
                            this.DataSource.Protocol.ToString().ToUpper(),
                            this.DataSource.HostName.ToString().ToUpper(),
                            this.DataSource.PipeName.ToString().ToUpper(),
                            this.DataSource.ServiceName.ToString().ToUpper()
                        );
                    break;
                case OracleProtocol.sdp:
                case OracleProtocol.tcp:
                case OracleProtocol.tcps:
                    format = "SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL={0})(HOST={1})(PORT={2}))" +
                        "(CONNECT_DATA=(SERVICE_NAME={3})));";
                    result = format.args
                        (
                            this.DataSource.Protocol.ToString().ToUpper(),
                            this.DataSource.HostName.ToString().ToUpper(),
                            this.DataSource.PortNumber.ToString().ToUpper(),
                            this.DataSource.ServiceName.ToString().ToUpper()
                        );
                    break;
            }

            // Clear and Setup parameters
            this.ConnectionStrings.Clear();

            this.ConnectionStrings["User ID"] = this.Security.UserName;
            this.ConnectionStrings["Password"] = this.Security.Password;

            // Build result connection string.
            result += this.ConnectionStrings.GetConnectionString();

            if (!string.IsNullOrWhiteSpace(this.Optional.ExtendConnectionString))
            {
                // Append extend connection string.
                result += this.Optional.ExtendConnectionString;
            }

            return result;
        }
        /// <summary>
        /// Create database factory provider.
        /// </summary>
        /// <returns>Returns instance of database factory provider.</returns>
        protected override NDbFactory CreateFactory()
        {
            OracleConnectionFactory result = new OracleConnectionFactory();
            result.SetConfig(this);
            return result;
        }
        /// <summary>
        /// Gets the default database object owner.
        /// </summary>
        /// <returns>Returns the default database object owner.</returns>
        protected override string GetDefaultOwner()
        {
            if (string.IsNullOrWhiteSpace(this.Security.UserName))
                return string.Empty;
            return this.Security.UserName;
        }

        #endregion

        #endregion

        #region Public Properties

        #region Data Source

        /// <summary>
        /// Gets or sets Oracle Connection Options.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets Oracle Connection Options.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlElement]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PropertyOrder(ConfigOrders.DataSource)]
        public OracleServerOptions DataSource
        {
            get
            {
                _datasource = this.CheckVar(_datasource);
                return _datasource;
            }
            set
            {
                _datasource = value;
                _datasource = this.CheckVar(_datasource);
                RaiseConfigChanged();
            }
        }

        #endregion

        #endregion

        #region Static Access

        #region Get Provider Name

        /// <summary>
        /// Get Connection Provider Name.
        /// </summary>
        public static new string DbProviderName
        {
            get { return "Oracle (Native)"; }
        }

        #endregion

        #region Create

        /// <summary>
        /// Create new NDbConfig Instance.
        /// </summary>
        /// <returns>Returns NDbConfig Instance.</returns>
        public static new NDbConfig Create() { return new OracleConfig(); }

        #endregion

        #endregion
    }

    #endregion
}

#pragma warning restore 0618