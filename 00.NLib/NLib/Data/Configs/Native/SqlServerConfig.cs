#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-12-07
=================
- DataAccess : SqlServerConfig
  - Re-Implement Database Config classes for SqlServer.

======================================================================================================================
Update 2013-03-15
=================
- DataAccess : SqlServer
  - Change connection string -> 'Timeout' to 'Connection Timeout'

======================================================================================================================
Update 2013-03-08
=================
- DataAccess : SqlServer
  - All SqlServer data access classes is redesign.
  - Change name space to NLib.Data and NLib.Data.Design
  - Remove SqlServerConfigTypeConverter class.
  - SqlServerDatabaseNameEditor class is tempory disable.

======================================================================================================================
Update 2012-03-08
=================
- DataAccess : SqlServer
  - SqlServer Config remove all default value attributes.

======================================================================================================================
Update 2010-02-03
=================
- DataAccess : SqlServer
  - SqlServerConnectionFactory ported and re-implements uses new generic base class.
  - SqlServer sql model and related class ported.

======================================================================================================================
Update 2008-11-23
=================
- DataAccess : SqlServer
  - Add try-catch in CreateParameter when set Precision/Scale.

======================================================================================================================
Update 2008-11-17
=================
- SqlServerConfig updated
  - Add New Property ServerDateFormat.

======================================================================================================================
Update 2008-10-25
=================
- Sql Model (Native) updated.
  [SqlServer]
  - Fixed Identity type.
- DataAccess library Task changed
  - SqlServer Create Database Task now not need to connected before Create new Database.
  - SqlServer Attach Database Task now not need to connected before Attach Database.

======================================================================================================================
Update 2008-10-21
=================
- Sql Model (Native) updated.
  [SqlServer]
  - Add new class SqlServerSqlModel.SqlServerValueFormatter for handle DDL generate script.
  - Implement method CreateDDLFormatter.
  - Implement method GenerateViewScript (incompleted) in it's DDLFormatter.
  - Implement method GenerateTableScript in it's DDLFormatter.
  - Implement method GenerateTableColumnScript in it's DDLFormatter.
  - Implement method GenerateTableConstraintScript in it's DDLFormatter.

======================================================================================================================
Update 2008-10-20
=================
- DataAccess : SqlServer
  - Fixed Connection check null bug (in case original code used logical operator && instead of ||).

======================================================================================================================
Update 2008-10-16
=================
- SqlServerConfig add support new Database's task.
  - Add support Set Recovery Mode Task.
  - Add new property AutoFixedServerName to prevent add invalid slash in server's name.
  - Add new property AutoAppendServiceName to automaticall add service name after server name.
  - Add new property ServiceName used for generate actual Server Name that used for 
	connect to database.
- New design time editor added.
  - Add Design Time editor : SqlServerDatabaseNameEditor.
  - Add Design Time editor : NetworkConputerNameEditor.
- DataAccess library new Task for SQL Server added.
  - SqlServerSetRecoverMode Task and parameter added.

======================================================================================================================
Update 2008-09-22
=================
- DataAccess library fixed security issue for SQL SERVER
  - Backup Task -> add permission for Access Backup file.
  - Restore Task -> add permission for Access Backup/MDF/LDF file.

======================================================================================================================
Update 2008-07-07
=================
- DataAccess library new Features add
  - Implement Task for SqlServer maintance routine.

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
	#region Designer Editors
	
	#region Open BAK

	/// <summary>
	/// *.Bak Open File Editor
	/// </summary>
	public class OpenBAKFileNameEditor : System.Windows.Forms.Design.FileNameEditor
	{
		#region Override For Dialog

		/// <summary>
		/// Initialize Dialog before process Opening
		/// </summary>
		/// <param name="openFileDialog">Open Dialog Instance</param>
		protected override void InitializeDialog(
			System.Windows.Forms.OpenFileDialog openFileDialog)
		{
			base.InitializeDialog(openFileDialog);
			openFileDialog.Filter = "Bak Files (*.bak)|*.bak|All Files (*.*)|*.*";
		}

		#endregion
	}

	#endregion

	#region Save BAK

	/// <summary>
	/// *.Bak Save File Editor
	/// </summary>
	public class SaveBAKFileNameEditor : SaveFileEditor
	{
		#region Override For Dialog

		/// <summary>
		/// Initialize Dialog before process Saving
		/// </summary>
		/// <param name="saveFileDialog">Save Dialog Instance</param>
		protected override void InitializeDialog(
			System.Windows.Forms.SaveFileDialog saveFileDialog)
		{
			base.InitializeDialog(saveFileDialog);
			saveFileDialog.Filter = "Bak Files (*.bak)|*.bak|All Files (*.*)|*.*";
		}

		#endregion
	}

	#endregion

	#region Open MDF

	/// <summary>
	/// *.MDF Open File Editor
	/// </summary>
	public class OpenMDFFileNameEditor : System.Windows.Forms.Design.FileNameEditor
	{
		#region Override For Dialog

		/// <summary>
		/// Initialize Dialog before process Opening
		/// </summary>
		/// <param name="openFileDialog">Open Dialog Instance</param>
		protected override void InitializeDialog(
			System.Windows.Forms.OpenFileDialog openFileDialog)
		{
			base.InitializeDialog(openFileDialog);
			openFileDialog.Filter = "MDF Files (*.mdf)|*.mdf|All Files (*.*)|*.*";
		}

		#endregion
	}

	#endregion

	#region Open LDF

	/// <summary>
	/// *.LDF Open File Editor
	/// </summary>
	public class OpenLDFFileNameEditor : System.Windows.Forms.Design.FileNameEditor
	{
		#region Override For Dialog

		/// <summary>
		/// Initialize Dialog before process Opening
		/// </summary>
		/// <param name="openFileDialog">Open Dialog Instance</param>
		protected override void InitializeDialog(
			System.Windows.Forms.OpenFileDialog openFileDialog)
		{
			base.InitializeDialog(openFileDialog);
			openFileDialog.Filter = "LDF Files (*.ldf)|*.ldf|All Files (*.*)|*.*";
		}

		#endregion
	}

	#endregion

	#region Save MDF

	/// <summary>
	/// *.MDF Save File Editor
	/// </summary>
	public class SaveMDFFileNameEditor : SaveFileEditor
	{
		#region Override For Dialog

		/// <summary>
		/// Initialize Dialog before process Saving
		/// </summary>
		/// <param name="saveFileDialog">Save Dialog Instance</param>
		protected override void InitializeDialog(
			System.Windows.Forms.SaveFileDialog saveFileDialog)
		{
			base.InitializeDialog(saveFileDialog);
			saveFileDialog.Filter = "MDF Files (*.mdf)|*.mdf|All Files (*.*)|*.*";
		}

		#endregion
	}

	#endregion

	#region Save LDF

	/// <summary>
	/// *.LDF Save File Editor
	/// </summary>
	public class SaveLDFFileNameEditor : SaveFileEditor
	{
		#region Override For Dialog

		/// <summary>
		/// Initialize Dialog before process Saving
		/// </summary>
		/// <param name="saveFileDialog">Save Dialog Instance</param>
		protected override void InitializeDialog(
			System.Windows.Forms.SaveFileDialog saveFileDialog)
		{
			base.InitializeDialog(saveFileDialog);
			saveFileDialog.Filter = "LDF Files (*.ldf)|*.ldf|All Files (*.*)|*.*";
		}

		#endregion
	}

	#endregion

	#region Network Conputer Name Editor

	/// <summary>
	/// Network Conputer Name Editor
	/// </summary>
	public class NetworkConputerNameEditor : CustomDropDownPropertyEditor
	{
		#region Init Listbox Value

		/// <summary>
		/// SetEditorProps
		/// </summary>
		/// <param name="editingInstance">object instance to edit</param>
		/// <param name="editor">control that used as editor</param>
		protected override void SetEditorProps(object editingInstance,
			System.Windows.Forms.ListBox editor)
		{
			editor.Items.Clear();
			// local.
			editor.Items.Add("(local)");
			// couputer's name.
			editor.Items.Add(NetworkUtils.GetLocalHostName().ToUpper());

			try
			{
				List<string> names = NetworkUtils.FindNetworkComputers();
				if (names != null && names.Count > 0)
				{
					foreach (string compName in names)
					{
						if (!editor.Items.Contains(compName.ToUpper()))
						{
							editor.Items.Add(compName.ToUpper());
						}
					}
				}
			}
			catch { }
		}

		#endregion
	}

	#endregion

	#region SqlServer Database Name Editor

	/// <summary>
	/// SqlServer Database Name Editor
	/// </summary>
	public class SqlServerDatabaseNameEditor : CustomDropDownPropertyEditor
	{
		#region Init Listbox Value

		/// <summary>
		/// SetEditorProps
		/// </summary>
		/// <param name="editingInstance">object instance to edit</param>
		/// <param name="editor">control that used as editor</param>
		protected override void SetEditorProps(object editingInstance,
			System.Windows.Forms.ListBox editor)
		{
			editor.Items.Clear();
			if (editingInstance.GetType() == typeof(SqlServerConfig))
			{
				SqlServerConfig config = (SqlServerConfig)editingInstance;

				if (config != null && config.Factory != null)
				{
					DbConnection conn = config.Factory.CreateConnection();

					if (conn == null)
						return;

					try
					{
						conn.ConnectionString = config.ConnectionString;
						conn.Open();
					}
					catch { }
					if (conn != null && conn.State != ConnectionState.Open)
					{
						try
						{
							conn.Close();
							conn.Dispose();
							conn = null;
						}
						catch { }
						return;
					}

					DbCommand cmd = config.Factory.CreateCommand(conn, null);
					if (cmd != null)
					{
						cmd.CommandTimeout = 30;
						cmd.CommandText = "exec sp_databases";
						IDataReader reader = null;
						try
						{
							reader = cmd.ExecuteReader();
						}
						catch { }

						if (reader != null)
						{
							try
							{
								while (reader.Read())
								{
									string databaseName = reader["DATABASE_NAME"].ToString();
									editor.Items.Add(databaseName);
								}
							}
							catch { }
						}

						if (reader != null)
						{
							try
							{
								reader.Close();
								reader.Dispose();
							}
							catch { }
						}
						reader = null;
					}

					if (cmd != null)
					{
						try
						{
							cmd.Dispose();
						}
						catch { }
					}
					cmd = null;
					if (conn != null)
					{
						try
						{
							conn.Close();
							conn.Dispose();
						}
						catch { }
					}
					conn = null;
				}
			}
		}

		#endregion
	}

	#endregion

	#endregion
}

namespace NLib.Data
{
	#region Common classes and Enums

	#region SqlServerVersion Enum

	/// <summary>
	/// Specificed SqlServer Version
	/// </summary>
	public enum SqlServerVersion
	{
		/// <summary>
		/// SqlServer 7
		/// </summary>
		SqlServer7,
		/// <summary>
		/// SqlServer 2000
		/// </summary>
		SqlServer2K,
		/// <summary>
		/// Microsoft Database Engine 7
		/// </summary>
		MSDE7,
		/// <summary>
		/// Microsoft Database Engine 2000
		/// </summary>
		MSDE2K,
		/// <summary>
		/// SqlServer 2005.
		/// </summary>
		SqlServer2005,
		/// <summary>
		/// SqlServer 2008.
		/// </summary>
		SqlServer2008,
		/// <summary>
		/// SqlServer 2012.
		/// </summary>
		SqlServer2012
	}

	#endregion

	#region Common classes for Serialization connection config

	/// <summary>
	/// The SqlServerOptions class.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
	public class SqlServerOptions
	{
		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerOptions()
			: base()
		{
			this.ServerName = Environment.MachineName;
			this.ServiceName = string.Empty;
			this.DatabaseName = "master";
			this.Version = SqlServerVersion.SqlServer2008;
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
			if (string.IsNullOrWhiteSpace(this.ServiceName))
			{
				return string.Format(@"{0} - {1}",
					this.ServerName,
					this.DatabaseName);
			}
			else
			{
				return string.Format(@"{0}\{1} - {2}",
					this.ServerName,
					this.ServiceName,
					this.DatabaseName);
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the Server host name.
		/// </summary>
		[Category("Database")]
		[Description("Gets or sets the Server host name.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[PropertyOrder(1)]
		[XmlAttribute]
		public string ServerName { get; set; }
		/// <summary>
		/// Gets or sets Sql Server instance name.
		/// </summary>
		[Category("Database")]
		[Description("Gets or sets Sql Server instance name.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[PropertyOrder(2)]
		[XmlAttribute]
		public string ServiceName { get; set; }
		/// <summary>
		/// Gets or sets the database name.
		/// </summary>
		[Category("Database")]
		[Description("Gets or sets the database name.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[PropertyOrder(3)]
		[XmlAttribute]
		public string DatabaseName { get; set; }
		/// <summary>
		/// Gets or sets the Sql Server Version.
		/// </summary>
		[Category("Database")]
		[Description("Gets or sets the Sql Server Version.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[PropertyOrder(4)]
		[XmlAttribute]
		public SqlServerVersion Version { get; set; }

		#endregion
	}

	#endregion

	#endregion

	#region SqlServerConfig

	/// <summary>
	/// Microsoft Sql Server Connection Config class.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
	public class SqlServerConfig : NDbConfig
	{
		#region Internal Variables

		private SqlServerOptions _datasource = null;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerConfig() : base()
		{
			// set default user name, password and authentication mode.
			this.Security.UserName = "sa";
			this.Security.Password = string.Empty;
			this.Security.Authentication = AuthenticationMode.Server;
			// set default server and database name.
			this.DataSource.ServerName = Environment.MachineName;
			this.DataSource.DatabaseName = "master";
			// set default server date format.
			this.Optional.ServerDateFormat = "YMD";
			this.Optional.EnableMARS = true;
		}
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerConfig()
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
			#region Information for Connection string and Integrated Security, Persist Security Info

			//	-- SqlConnection --
			//	*******************
			//
			//	Integrated Security -or- Trusted_Connection 
			//	===========================================
			//	default value : 'false'
			//	description :	When false, User ID and Password are specified in the connection. 
			//					When true, the current Windows account credentials are used for authentication.
			//					Recognized values are true, false, yes, no, and sspi (strongly recommended), 
			//					which is equivalent to true.
			//
			//	Persist Security Info
			//	=====================
			//	default value : 'false'
			//	description :	When set to false or no (strongly recommended), 
			//					security-sensitive information, such as the password, is not returned as part 
			//					of the connection if the connection is open or has ever been in an open state. 
			//					Resetting the connection string resets all connection string values including 
			//					the password. Recognized values are true, false, yes, and no.
			//
			//	-- OleDbConnection --
			//	*********************
			//	Remarks
			//	The ConnectionString is designed to match OLE DB connection string format as closely as possible 
			//	with the following exceptions: 
			//	The "Provider = value " clause is required. However, you cannot use "Provider = MSDASQL" 
			//	because the .NET Framework Data Provider for OLE DB does not support the OLE DB Provider 
			//	for ODBC (MSDASQL). To access ODBC data sources, use the OdbcConnection object, which is in 
			//	the System.Data.Odbc namespace. 
			//	Unlike OLE DB or ADO, the connection string that is returned is the same as the user-set 
			//	ConnectionString, minus security information if Persist Security Info is set to false (default). 
			//	The .NET Framework Data Provider for OLE DB does not persist or return the password in a 
			//	connection string unless you set the Persist Security Info keyword to true (not recommended). 
			//	To maintain the highest level of security, it is strongly recommended that you use the 
			//	Integrated Security keyword with Persist Security Info set to false. 


			#endregion

			#region Sample for Connection for difference Connection Type

			//	OleDbConnection
			//	===============
			//	# Standard Security:
			//		"Provider=sqloledb;Data Source=Aron1;Initial Catalog=pubs;User Id=sa;Password=asdasd;"
			//	#  Trusted Connection:
			//		"Provider=sqloledb;Data Source=Aron1;Initial Catalog=pubs;Integrated Security=SSPI;"
			//		(use serverName\instanceName as Data Source to use an specifik SQLServer instance, only SQLServer2000)
			//	#  Prompt for username and password:
			//		oConn.Provider = "sqloledb"
			//		oConn.Properties("Prompt") = adPromptAlways
			//		oConn.Open "Data Source=Aron1;Initial Catalog=pubs;"
			//	#  Connect via an IP address:
			//		"Provider=sqloledb;Data Source=190.190.200.100,1433;Network Library=DBMSSOCN;Initial Catalog=pubs;User ID=sa;Password=asdasd;"
			//		(DBMSSOCN=TCP/IP instead of Named Pipes, at the end of the Data Source is the port to use (1433 is the default))
			//
			//	SqlConnection
			//	=============
			//	#   Standard Security:
			//		"Data Source=Aron1;Initial Catalog=pubs;User Id=sa;Password=asdasd;"
			//		- or -
			//		"Server=Aron1;Database=pubs;User ID=sa;Password=asdasd;Trusted_Connection=False"
			//		(both connection strings produces the same result)
			//	#  Trusted Connection:
			//		"Data Source=Aron1;Initial Catalog=pubs;Integrated Security=SSPI;"
			//		- or -
			//		"Server=Aron1;Database=pubs;Trusted_Connection=True;"
			//		(both connection strings produces the same result)
			//		(use serverName\instanceName as Data Source to use an specifik SQLServer instance, only SQLServer2000)
			//	#  Connect via an IP address:
			//	"Data Source=190.190.200.100,1433;Network Library=DBMSSOCN;Initial Catalog=pubs;User ID=sa;Password=asdasd;"
			//		(DBMSSOCN=TCP/IP instead of Named Pipes, at the end of the Data Source is the port to use (1433 is the default))
			//	#  Declare the SqlConnection:
			//		using System.Data.SqlClient;
			//		SqlConnection oSQLConn = new SqlConnection();
			//		oSQLConn.ConnectionString="my connection string";
			//		oSQLConn.Open(); 
			//	# MS Data Shape
			//		"Provider=MSDataShape;Data Provider=SQLOLEDB;Data Source=Aron1;Initial Catalog=pubs;User ID=sa;Password=asdasd;"
			//		Want to learn data shaping? Check out 4GuyfFromRolla's great article about Data Shaping >>			

			#endregion

			string result = string.Empty;

			if (string.IsNullOrWhiteSpace(this.DataSource.ServerName) ||
				string.IsNullOrWhiteSpace(this.DataSource.DatabaseName) ||
				string.IsNullOrWhiteSpace(this.Security.UserName))
			{
				// Required information is not assigned.
				return result;
			}

			string fullServerName = this.DataSource.ServerName.Trim();
			if (!string.IsNullOrWhiteSpace(this.DataSource.ServiceName))
			{
				fullServerName = this.DataSource.ServerName.Trim() + @"\" + 
					this.DataSource.ServiceName;
			}

			// Clear and Setup parameters
			this.ConnectionStrings.Clear();

			this.ConnectionStrings["Data Source"] = fullServerName;
			this.ConnectionStrings["Initial Catalog"] = this.DataSource.DatabaseName;
			if (this.Security.Authentication == AuthenticationMode.Server)
			{
				// Standard Security
				this.ConnectionStrings["Trusted_Connection"] = "False";
			}
			else
			{
				// Trusted Connection
				this.ConnectionStrings["Integrated Security"] = "SSPI";
			}
			if (this.Security.PersistSecurity == PersistSecurityMode.Yes)
			{
				this.ConnectionStrings["Persist Security Info"] = "True";
			}
			else if (this.Security.PersistSecurity == PersistSecurityMode.No)
			{
				this.ConnectionStrings["Persist Security Info"] = "False";
			}
			else if (this.Security.PersistSecurity == PersistSecurityMode.Default)
			{
				// Default. Do Nothing,
			}

			if (!string.IsNullOrWhiteSpace(this.Security.Domain) &&
				this.Security.Authentication == AuthenticationMode.Windows)
			{
				// For windows authentication supports domain/user
				this.ConnectionStrings["User Id"] = this.Security.Domain + @"\" +
					this.Security.UserName;
			}
			else
			{
				// no domain or mixed mode
				this.ConnectionStrings["User Id"] = this.Security.UserName;
			}			
			this.ConnectionStrings["Password"] = this.Security.Password;

			this.ConnectionStrings["Packet Size"] = "4096";

			this.ConnectionStrings["Connection Timeout"] = this.Timeout.ConnectionTimeoutInSeconds.ToString();
			if (this.Optional.EnableMARS)
			{
				this.ConnectionStrings["MultipleActiveResultSets"] = "true";
			}
			// Build result connection string.
			result = this.ConnectionStrings.GetConnectionString();

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
			SqlServerConnectionFactory result = new SqlServerConnectionFactory();
			result.SetConfig(this);
			return result;
		}

		#endregion

		#endregion

		#region Public Properties

		#region Data Source

		/// <summary>
		/// Gets or sets Sql Server Connection Options.
		/// </summary>
		[Category("Connection")]
		[Description("Gets or sets Sql Server Connection Options.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[XmlElement]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PropertyOrder(ConfigOrders.DataSource)]
		public SqlServerOptions DataSource
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

		#region Static Methods

		#region Get Provider Name

		/// <summary>
		/// Get Connection Provider Name.
		/// </summary>
		public static new string DbProviderName
		{
			get { return "Microsoft SQL Server (Native)"; }
		}

		#endregion

		#region Create

		/// <summary>
		/// Create new NDbConfig Instance.
		/// </summary>
		/// <returns>Returns NDbConfig Instance.</returns>
		public static new NDbConfig Create() { return new SqlServerConfig(); }

		#endregion

		#endregion
	}

	#endregion
}
