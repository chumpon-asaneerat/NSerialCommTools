#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-07-07
=================
- Data Access Framework - SqlServerConnectionFactory and Tasks updated.
  - Changed all log code used MethodBase.

======================================================================================================================
Update 2014-12-15
=================
- Data Access Framework - SqlServerConnectionFactory
  - Update supports Execute Database tasks with re-coding and re-formatting.

======================================================================================================================
Update 2014-11-30
=================
- Data Access Framework - SqlServerConnectionFactory
  - Redesign SqlServerConnectionFactory class with new implements.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Reflection;

#endregion

namespace NLib.Data
{
	#region SqlServer Factory

	/// <summary>
	/// SqlServer Connection Factory.
	/// </summary>
	public class SqlServerConnectionFactory : NDbFactory<
		System.Data.SqlClient.SqlConnection,
		System.Data.SqlClient.SqlTransaction,
		System.Data.SqlClient.SqlCommand,
		System.Data.SqlClient.SqlParameter,
		System.Data.SqlClient.SqlDataAdapter,
		System.Data.SqlClient.SqlCommandBuilder>
	{
		#region Abstract/Virtual Overrides

		#region InitFormatter

		/// <summary>
		/// Init formatter.
		/// </summary>
		/// <param name="formatter">The formatter instance.</param>
		protected override void InitFormatter(NDbFormatter formatter)
		{
			MethodBase med = MethodBase.GetCurrentMethod();

			#region Checks
			
			if (null == formatter)
			{
				"formatter is null".Err(med);
				return;
			}

			#endregion

			#region Init delegates

			formatter.GetTestQuery = () =>
			{
				return "SELECT 1";
			};
			formatter.GetServerDateQuery = () =>
			{
				return "SELECT getdate()";
			};
			formatter.SelectIdentityQuery = (string tableName, string columnName) =>
			{
				//return "select @@IDENTITY"; // This will returns new identity from global scope.
				return "SELECT SCOPE_IDENTITY()"; // This will returns new identity from current scope.
			};
			formatter.GetParameterPrefix = () => 
			{ 
				return "@"; 
			};
			formatter.GetDefaultReturnParameterName = () => 
			{ 
				return GetParameterPrefix() + "RETURN_VALUE"; 
			};
			formatter.DateTimeToSqlString = (DateTime value, bool dateOnly) =>
			{
				if (dateOnly)
				{
					string data = value.ToString("yyyy-MM-dd",
						System.Globalization.DateTimeFormatInfo.InvariantInfo);
					return string.Format("'{0}'", data);
				}
				else
				{
					string data = value.ToString("yyyy-MM-dd HH:mm:ss",
						DateTimeFormatInfo.InvariantInfo);
					return string.Format("'{0}'", data);
				}
			};

			#endregion
		}

		#endregion

		#region SetProviderDbTypeID

		/// <summary>
		/// Set the provide db type id to the parameter object.
		/// </summary>
		/// <param name="parameter">The parameter object.</param>
		/// <param name="providerDbTypeID">The provide type id.</param>
		protected override void SetProviderDbTypeID(System.Data.SqlClient.SqlParameter parameter, int providerDbTypeID)
		{
			parameter.SqlDbType = (SqlDbType)providerDbTypeID;
		}

		#endregion

		#region Schema Info Implements

		/// <summary>
		/// Get Table Schema Info.
		/// </summary>
		/// <param name="owner">The owner name.</param>
		/// <returns>Returns the Table SchemaInfo instance for DbTable.</returns>
		protected override NSchemaInfo<NDbTable> GetTableSchemaInfo(string owner)
		{
			string _owner = string.IsNullOrWhiteSpace(owner) ? string.Empty : owner.Trim();

			// SqlServer
			NSchemaInfo<NDbTable> info = new NSchemaInfo<NDbTable>();

			info.MetaData = new NDbMetaData("Tables");
			info.Restrictions = new NDbRestriction[] 
			{                
				new NDbRestriction(1, "Catalog", ""),
				new NDbRestriction(2, "Owner", _owner),
				new NDbRestriction(3, "Table", ""),
				new NDbRestriction(4, "TableType", "")
			};
			info.Convert = delegate(DataRow row, NSchemaInfo<NDbTable> schema, NDbFactory factory)
			{
				NDbTable val = null;
				if (row["TABLE_NAME"] != DBNull.Value)
				{
					val = new NDbTable();
					val.TableName = row["TABLE_NAME"].ToString();
					val.ProviderTableType = row["TABLE_TYPE"].ToString();
				}
				return val;
			};

			return info;
		}
		/// <summary>
		/// GetProcedureSchemaInfo
		/// </summary>
		/// <param name="owner">The owner.</param>
		/// <returns>Returns ProcedureSchemaInfo instance.</returns>
		protected override NProcedureSchemaInfo GetProcedureSchemaInfo(string owner)
		{
			string _owner = string.IsNullOrWhiteSpace(owner) ? string.Empty : owner.Trim();

			NDbRestriction[] restrictions = new NDbRestriction[4];
			restrictions[0] = new NDbRestriction(1, "CATALOG", string.Empty);
			restrictions[1] = new NDbRestriction(2, "OWNER", _owner);
			restrictions[2] = new NDbRestriction(3, "NAME", string.Empty);
			restrictions[3] = new NDbRestriction(4, "TYPE", string.Empty);

			NProcedureSchemaInfo info = new NProcedureSchemaInfo();
			info.Restrictions = restrictions;
			info.ProcedureNameColumn = "ROUTINE_NAME";

			return info;
		}

		#endregion

		#region GetTasks

		private NDbTask[] _tasks = null;

		/// <summary>
		/// Gets Supported Task.
		/// </summary>
		/// <returns>Returns list of all avaliable supported tasks.</returns>
		public override NDbTask[] GetTasks()
		{
			if (_tasks == null)
			{
				_tasks = new NDbTask[]
				{ 
					SqlServerCreateDatabaseTask.Create(), 
					SqlServerDeleteDatabaseTask.Create(), 
					SqlServerBackupDatabaseTask.Create(), 
					SqlServerRestoreDatabaseTask.Create(), 
					SqlServerAttachDatabaseTask.Create(), 
					SqlServerDetachDatabaseTask.Create(),
					SqlServerShrinkDatabaseTask.Create(),
					SqlServerSetRecoveryModeTask.Create()
				};
			}
			return _tasks;
		}

		#endregion

		#region Set Date Format

		/// <summary>
		/// Set Date Format.
		/// </summary>
		/// <param name="connection">The connection instance.</param>
		/// <param name="transaction">Transaction instance</param>
		protected override void SetDateFormat(System.Data.Common.DbConnection connection,
			System.Data.Common.DbTransaction transaction)
		{
			if (null != this.Config &&
				connection != null && connection.State == ConnectionState.Open)
			{
				#region Set Date Format

				MethodBase med = MethodBase.GetCurrentMethod();

				if (!string.IsNullOrWhiteSpace(this.Config.Optional.ServerDateFormat))
				{
					System.Data.Common.DbCommand cmd = connection.CreateCommand();
					cmd.CommandText =
						"SET DATEFORMAT " + this.Config.Optional.ServerDateFormat;
					if (transaction != null)
					{
						cmd.Transaction = transaction;
					}
					try
					{
						int result = cmd.ExecuteNonQuery();
						if (result == -1)
						{
							// OK.
						}
					}
					catch (Exception ex)
					{
						"SET DATEFORMAT Error.".Err(med);
						ex.Err(med);
					}

					if (cmd != null)
					{
						try { cmd.Dispose(); }
						catch { }
					}
					cmd = null;
				}

				#endregion
			}
		}

		#endregion

		#endregion
	}

	#endregion
}

namespace NLib.Data
{
	#region Using For IO/Security (Used by task)

	using System.IO;
	using System.Security;
	using System.Security.AccessControl;
	using System.Security.Permissions;

	#endregion

	using NLib.Components; // For NDbConnection.
	using NLib.Data.Design; // For Designer.

	#region SqlServer Tasks

	#region Create

	/// <summary>
	/// SqlServer Create Database Parameter.
	/// </summary>
	public class SqlServerCreateDatabaseParameter : NDbTaskParameter
	{
		#region Internal Variable

		private string _databaseName = string.Empty;
		private string physicalTargetMDFFile = string.Empty;
		private string physicalTargetLDFFile = string.Empty;
		private string script = string.Empty;
		private string _accountName = "NETWORK SERVICE";

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerCreateDatabaseParameter() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerCreateDatabaseParameter() { }

		#endregion

		#region Override

		/// <summary>
		/// Checks Is Parameter Valid.
		/// </summary>
		/// <returns>Returns true if parameter is valid.</returns>
		public override bool IsValid()
		{
			if (string.IsNullOrWhiteSpace(_databaseName)) return false;
			if (string.IsNullOrWhiteSpace(physicalTargetMDFFile)) return false;
			if (string.IsNullOrWhiteSpace(physicalTargetLDFFile)) return false;
			return true;
		}

		#endregion

		#region Public Property

		/// <summary>
		/// Gets or Sets Database Name.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or Sets Database Name.")]
		[DefaultValue("")]
		public string DatabaseName
		{
			get { return _databaseName; }
			set { _databaseName = value; }
		}
		/// <summary>
		/// Gets or Sets Physical Master Data File.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or Sets Physical Master Data File")]
		[DefaultValue("")]
		[Editor(typeof(SaveMDFFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string PhysicalMDF
		{
			get { return physicalTargetMDFFile; }
			set { physicalTargetMDFFile = value; }
		}
		/// <summary>
		/// Gets or Sets Physical Log Data File.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or Sets Physical Log Data File.")]
		[DefaultValue("")]
		[Editor(typeof(SaveLDFFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string PhysicalLDF
		{
			get { return physicalTargetLDFFile; }
			set { physicalTargetLDFFile = value; }
		}
		/// <summary>
		/// Gets or Sets Database Script.
		/// </summary>
		[Category("Parameter")]
		[Description("Get/Set Database Script.")]
		[DefaultValue("")]
		[Editor(typeof(SqlScriptFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string DatabaseScript
		{
			get { return script; }
			set { script = value; }
		}
		/// <summary>
		/// Gets or Sets Account's Name to grant full access. Required if use SQLEXPRESS.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or Sets Account's Name to grant full access. Required if use SQLEXPRESS.")]
		public string Account
		{
			get { return _accountName; }
			set { _accountName = value; }
		}

		#endregion
	}
	/// <summary>
	/// SqlServer Create Database Task.
	/// </summary>
	public class SqlServerCreateDatabaseTask : NDbTask
	{
		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerCreateDatabaseTask() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerCreateDatabaseTask() { }

		#endregion

		#region Abstract Implement

		/// <summary>
		/// Gets Task Parameter.
		/// </summary>
		/// <param name="manager">The DbConnection that used for Get parameter for task.</param>
		/// <returns>Returns NDbTask Parameter that used for execute Task.</returns>
		public override NDbTaskParameter GetParameter(NDbConnection manager)
		{
			return new SqlServerCreateDatabaseParameter();
		}
		/// <summary>
		/// Execute Task.
		/// </summary>
		/// <param name="manager">The DbConnection that used for execute Task.</param>
		/// <param name="parameter">Task Parameter that used for execute Task.</param>
		/// <returns>Returns NDbTask Result instance after task is execute.</returns>
		public override NDbTaskResult Execute(NDbConnection manager, NDbTaskParameter parameter)
		{
			#region Check

			if (null == parameter)
				return new NDbTaskResult("No Parameters.");
			if (!(parameter is SqlServerCreateDatabaseParameter))
				return new NDbTaskResult("Parameter Type is not match.");
			if (!parameter.IsValid())
				return new NDbTaskResult("Invalid Parameter.");
			if (null == manager)
				return new NDbTaskResult("Not Assigned Connection.");
			if (null == manager.Config)
				return new NDbTaskResult("Not Assigned Config.");
			if (!(manager.Config is SqlServerConfig))
				return new NDbTaskResult("Invalid Config Type.");

			string lastDbName = string.Empty;
			bool isConnected = manager.IsConnected;

			if (!manager.IsConnected)
			{
				// keep original database's name.
				lastDbName = (manager.Config as SqlServerConfig).DataSource.DatabaseName;
				(manager.Config as SqlServerConfig).DataSource.DatabaseName = "master";
				// connect to database.
				manager.Connect();
			}

			if (!manager.IsConnected)
				return new NDbTaskResult("Connection is not connected.");

			#endregion

			SqlServerCreateDatabaseParameter param = (parameter as SqlServerCreateDatabaseParameter);

			if (System.IO.File.Exists(param.PhysicalMDF))
				return new NDbTaskResult("MDF File Exists");
			if (System.IO.File.Exists(param.PhysicalLDF))
				return new NDbTaskResult("LDF File Exists");

			string code = "CREATE DATABASE " + param.DatabaseName + " ON PRIMARY " +
				"(NAME = " + param.DatabaseName + "_Data, " +
				"FILENAME = N'" + param.PhysicalMDF + "', " +
				"SIZE = 5MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
				"LOG ON (NAME = " + param.DatabaseName + "_Log, " +
				"FILENAME = N'" + param.PhysicalLDF + "', " +
				"SIZE = 1MB, MAXSIZE = 5MB, FILEGROWTH = 10%);";

			NDbTaskResult result = null;
			try
			{
				//manager.Execute(code, false);
				ExecuteResult<int> result1 = manager.ExecuteNonQuery(code);
				if (result1.HasException)
					result = new NDbTaskResult("Database creation incompleted : " + result1.Exception.Message);

				if (param.DatabaseScript.Length <= 0)
					result = new NDbTaskResult("Database Created.", true);
				else
				{
					try
					{
						//manager.Execute(param.DatabaseScript, false);
						ExecuteResult<int> result2 = manager.ExecuteNonQuery(param.DatabaseScript);
						if (result1.HasException)
							result = new NDbTaskResult("Database creation completed but script error : " + result2.Exception.Message);
						else result = new NDbTaskResult("Database with script Created.", true);
					}
					catch (Exception ex)
					{
						result = new NDbTaskResult("Database creation completed but script error : " + ex.Message);
					}
				}
			}
			catch (Exception ex)
			{
				result = new NDbTaskResult("Database creation incompleted : " + ex.Message);
			}

			// disconnect if the prior state is not connected.
			if (!isConnected)
			{
				manager.Disconnect();
				(manager.Config as SqlServerConfig).DataSource.DatabaseName = lastDbName;
			}

			return result;
		}

		#endregion

		#region Static

		#region Get Task Name

		/// <summary>
		/// Gets Task Name.
		/// </summary>
		public new static string TaskName
		{
			get { return "Create Database"; }
		}

		#endregion

		#region Create

		/// <summary>
		/// Create DbTask Instance.
		/// </summary>
		/// <returns>Returns DbTask Instance.</returns>
		public new static NDbTask Create() { return new SqlServerCreateDatabaseTask(); }

		#endregion

		#endregion
	}

	#endregion

	#region Delete

	/// <summary>
	/// SqlServer Delete Database Parameter.
	/// </summary>
	public class SqlServerDeleteDatabaseParameter : NDbTaskParameter
	{
		#region Internal Variable

		private string _databaseName = string.Empty;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerDeleteDatabaseParameter() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerDeleteDatabaseParameter() { }

		#endregion

		#region Override

		/// <summary>
		/// Checks Is Parameter Valid.
		/// </summary>
		/// <returns>Returns true if parameter is valid.</returns>
		public override bool IsValid()
		{
			if (string.IsNullOrWhiteSpace(_databaseName)) return false;
			return true;
		}

		#endregion

		#region Public Property

		/// <summary>
		/// Gets or sets Database Name.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets Database Name.")]
		[DefaultValue("")]
		public string DatabaseName
		{
			get { return _databaseName; }
			set { _databaseName = value; }
		}

		#endregion
	}
	/// <summary>
	/// SqlServer Delete Database Task.
	/// </summary>
	public class SqlServerDeleteDatabaseTask : NDbTask
	{
		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerDeleteDatabaseTask() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerDeleteDatabaseTask() { }

		#endregion

		#region Abstract Implement

		/// <summary>
		/// Gets Task Parameter.
		/// </summary>
		/// <param name="manager">The DbConnection that used for Get parameter for task.</param>
		/// <returns>Returns NDbTask Parameter that used for execute Task.</returns>
		public override NDbTaskParameter GetParameter(NDbConnection manager)
		{
			return new SqlServerDeleteDatabaseParameter();
		}
		/// <summary>
		/// Execute Task.
		/// </summary>
		/// <param name="manager">The DbConnection that used for execute Task.</param>
		/// <param name="parameter">Task Parameter that used for execute Task.</param>
		/// <returns>Returns NDbTask Result instance after task is execute.</returns>
		public override NDbTaskResult Execute(NDbConnection manager, NDbTaskParameter parameter)
		{
			#region Check

			if (null == parameter)
				return new NDbTaskResult("No Parameters.");
			if (!(parameter is SqlServerDeleteDatabaseParameter))
				return new NDbTaskResult("Parameter Type is not match.");
			if (!parameter.IsValid())
				return new NDbTaskResult("Invalid Parameter.");
			if (null == manager)
				return new NDbTaskResult("Not Assigned Connection.");
			if (null == manager.Config)
				return new NDbTaskResult("Not Assigned Config.");
			if (!(manager.Config is SqlServerConfig))
				return new NDbTaskResult("Invalid Config Type.");
			if (!manager.IsConnected)
				return new NDbTaskResult("Connection is not connected.");

			#endregion

			SqlServerDeleteDatabaseParameter param = (parameter as SqlServerDeleteDatabaseParameter);

			string code = "DROP DATABASE " + param.DatabaseName;

			try
			{
				//manager.Execute(code, false);
				ExecuteResult<int> result1 = manager.ExecuteNonQuery(code);
				if (result1.HasException)
					return new NDbTaskResult("Cannot delete database : " + result1.Exception.Message);
				else return new NDbTaskResult("Database Deleted.", true);
			}
			catch (Exception ex)
			{
				return new NDbTaskResult("Cannot delete database : " + ex.Message);
			}
		}

		#endregion

		#region Static

		#region Get Task Name

		/// <summary>
		/// Gets Task Name.
		/// </summary>
		public new static string TaskName
		{
			get { return "Delete Database"; }
		}

		#endregion

		#region Create

		/// <summary>
		/// Create DbTask Instance.
		/// </summary>
		/// <returns>Returns DbTask Instance.</returns>
		public new static NDbTask Create() { return new SqlServerDeleteDatabaseTask(); }

		#endregion

		#endregion
	}

	#endregion

	#region Backup

	/// <summary>
	/// SqlServer Backup Database Parameter.
	/// </summary>
	public class SqlServerBackupDatabaseParameter : NDbTaskParameter
	{
		#region Internal Variable

		private string _databaseName = string.Empty;
		private string _backupFileName = string.Empty;
		private string _accountName = "NETWORK SERVICE";

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerBackupDatabaseParameter() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerBackupDatabaseParameter() { }

		#endregion

		#region Override

		/// <summary>
		/// Checks Is Parameter Valid.
		/// </summary>
		/// <returns>Returns true if parameter is valid.</returns>
		public override bool IsValid()
		{
			if (string.IsNullOrWhiteSpace(_databaseName)) return false;
			if (string.IsNullOrWhiteSpace(_backupFileName)) return false;
			return true;
		}

		#endregion

		#region Public Property

		/// <summary>
		/// Gets or sets Database Name.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets Database Name.")]
		[DefaultValue("")]
		public string DatabaseName
		{
			get { return _databaseName; }
			set { _databaseName = value; }
		}
		/// <summary>
		/// Gets or sets Backup File Name.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets Backup File Name.")]
		[DefaultValue("")]
		[Editor(typeof(SaveBAKFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string BackupFileName
		{
			get { return _backupFileName; }
			set { _backupFileName = value; }
		}
		/// <summary>
		/// Gets or sets Account's Name to grant full access. Required if use SQLEXPRESS.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets Account's Name to grant full access. Required if use SQLEXPRESS.")]
		public string Account
		{
			get { return _accountName; }
			set { _accountName = value; }
		}

		#endregion
	}
	/// <summary>
	/// SqlServer Backup Database Task.
	/// </summary>
	public class SqlServerBackupDatabaseTask : NDbTask
	{
		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerBackupDatabaseTask() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerBackupDatabaseTask() { }

		#endregion

		#region Abstract Implement

		/// <summary>
		/// Gets Task Parameter.
		/// </summary>
		/// <param name="manager">The DbConnection that used for Get parameter for task.</param>
		/// <returns>Returns NDbTask Parameter that used for execute Task.</returns>
		public override NDbTaskParameter GetParameter(NDbConnection manager)
		{
			return new SqlServerBackupDatabaseParameter();
		}
		/// <summary>
		/// Execute Task.
		/// </summary>
		/// <param name="manager">The DbConnection that used for execute Task.</param>
		/// <param name="parameter">Task Parameter that used for execute Task.</param>
		/// <returns>Returns NDbTask Result instance after task is execute.</returns>
		public override NDbTaskResult Execute(NDbConnection manager, NDbTaskParameter parameter)
		{
			MethodBase med = MethodBase.GetCurrentMethod();

			#region Check

			if (null == parameter)
				return new NDbTaskResult("No Parameters.");
			if (!(parameter is SqlServerBackupDatabaseParameter))
				return new NDbTaskResult("Parameter Type is not match.");
			if (!parameter.IsValid())
				return new NDbTaskResult("Invalid Parameter.");
			if (null == manager)
				return new NDbTaskResult("Not Assigned Connection.");
			if (null == manager.Config)
				return new NDbTaskResult("Not Assigned Config.");
			if (!(manager.Config is SqlServerConfig))
				return new NDbTaskResult("Invalid Config Type.");
			if (!manager.IsConnected)
				return new NDbTaskResult("Connection is not connected.");

			#endregion

			SqlServerBackupDatabaseParameter param = (parameter as SqlServerBackupDatabaseParameter);
			if (!System.IO.File.Exists(param.BackupFileName))
			{
				string dir = System.IO.Path.GetDirectoryName(param.BackupFileName);
				if (!System.IO.Directory.Exists(dir)) 
					System.IO.Directory.CreateDirectory(dir);

				System.IO.StreamWriter writer = System.IO.File.CreateText(param.BackupFileName);
				if (writer != null)
				{
					writer.Flush();
					writer.Close();
					writer = null;
				}
				else
				{
					new NDbTaskResult("Cannot create backup file " + param.BackupFileName);
				}
			}
			// recheck
			if (!System.IO.File.Exists(param.BackupFileName))
			{
				new NDbTaskResult("Backup file is not found." + param.BackupFileName);
			}

			#region Security for SQLEXPRESS

			string account = param.Account;
			//string account = "NETWORK SERVICE"; // SQLEXPRESS runs under this account

			FileSystemRights rights = FileSystemRights.FullControl;
			AccessControlType controlType = AccessControlType.Allow;
			FileSecurity fSecurity = null;

			try
			{
				"Grant permission for Backup File.".Info(med);
				// Get a FileSecurity object that represents the
				// current security settings.
				fSecurity = File.GetAccessControl(param.BackupFileName);
				// Add the FileSystemAccessRule to the security settings.
				fSecurity.AddAccessRule(new FileSystemAccessRule(account,
					rights, controlType));

				// Set the new access settings.
				File.SetAccessControl(param.BackupFileName, fSecurity);
			}
			catch (Exception ex)
			{
				"Grant Permission Failed.".Err(med);
				ex.Err(med);
			}

			#endregion

			string deviceName = "xxBackupxx";
			string code = string.Empty;
			code += @"use master";
			code += @";";
			code += @"EXEC sp_addumpdevice 'disk', '" + deviceName + "', N'" + param.BackupFileName + "'";
			code += @";";
			code += @"BACKUP DATABASE " + param.DatabaseName + " TO " + deviceName + "";
			code += @";";
			code += @"EXEC sp_dropdevice '" + deviceName + "'";
			code += @";";
			code += @"USE " + (manager.Config as NLib.Data.SqlServerConfig).DataSource.DatabaseName;
			code += @";";

			NDbTaskResult result;
			try
			{
				//manager.Execute(code, false);
				ExecuteResult<int> result1 = manager.ExecuteNonQuery(code);
				if (result1.HasException)
					result = new NDbTaskResult("Backup incompleted : " + result1.Exception.Message);
				else result = new NDbTaskResult("Backup Completed", true);
			}
			catch (Exception ex)
			{
				result = new NDbTaskResult("Backup incompleted : " + ex.Message);
			}
			finally
			{
			}

			#region Remove Security Access

			try
			{
				if (null != fSecurity)
				{
					"Revoke permission for Backup File.".Info(med);
					// Remove the FileSystemAccessRule from the security settings.
					fSecurity.RemoveAccessRule(new FileSystemAccessRule(account,
						rights, controlType));
				}
			}
			catch (Exception ex)
			{
				"Revoke Permission Failed.".Err(med);
				ex.Err(med);
			}

			#endregion

			return result;
		}

		#endregion

		#region Static

		#region Get Task Name

		/// <summary>
		/// Gets Task Name.
		/// </summary>
		public new static string TaskName
		{
			get { return "Backup Database"; }
		}

		#endregion

		#region Create

		/// <summary>
		/// Create DbTask Instance.
		/// </summary>
		/// <returns>Returns DbTask Instance.</returns>
		public new static NDbTask Create() { return new SqlServerBackupDatabaseTask(); }

		#endregion

		#endregion
	}

	#endregion

	#region Restore

	/// <summary>
	/// SqlServer Restore Database Parameter.
	/// </summary>
	public class SqlServerRestoreDatabaseParameter : NDbTaskParameter
	{
		#region Internal Variable

		private string _databaseName = string.Empty;
		private string _backupFileName = string.Empty;
		private string _accountName = "NETWORK SERVICE";

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerRestoreDatabaseParameter() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerRestoreDatabaseParameter() { }

		#endregion

		#region Override

		/// <summary>
		/// Checks Is Parameter Valid.
		/// </summary>
		/// <returns>Returns true if parameter is valid.</returns>
		public override bool IsValid()
		{
			if (string.IsNullOrWhiteSpace(_databaseName)) return false;
			if (string.IsNullOrWhiteSpace(_backupFileName)) return false;
			return true;
		}

		#endregion

		#region Public Property

		/// <summary>
		/// Gets or sets Database Name to Restore.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets Database Name to Restore.")]
		[DefaultValue("")]
		public string DatabaseName
		{
			get { return _databaseName; }
			set { _databaseName = value; }
		}
		/// <summary>
		/// Gets or sets Backup File Name.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets Backup File Name.")]
		[DefaultValue("")]
		[Editor(typeof(OpenBAKFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string BackupFileName
		{
			get { return _backupFileName; }
			set { _backupFileName = value; }
		}
		/// <summary>
		/// Gets or sets Account's Name to grant full access. Required if use SQLEXPRESS.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets Account's Name to grant full access. Required if use SQLEXPRESS.")]
		public string Account
		{
			get { return _accountName; }
			set { _accountName = value; }
		}

		#endregion
	}
	/// <summary>
	/// SqlServer Restore Database Task.
	/// </summary>
	public class SqlServerRestoreDatabaseTask : NDbTask
	{
		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerRestoreDatabaseTask() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerRestoreDatabaseTask() { }

		#endregion

		#region Abstract Implement

		/// <summary>
		/// Gets Task Parameter.
		/// </summary>
		/// <param name="manager">The DbConnection that used for Get parameter for task.</param>
		/// <returns>Returns NDbTask Parameter that used for execute Task.</returns>
		public override NDbTaskParameter GetParameter(NDbConnection manager)
		{
			return new SqlServerRestoreDatabaseParameter();
		}
		/// <summary>
		/// Execute Task.
		/// </summary>
		/// <param name="manager">The DbConnection that used for execute Task.</param>
		/// <param name="parameter">Task Parameter that used for execute Task.</param>
		/// <returns>Returns NDbTask Result instance after task is execute.</returns>
		public override NDbTaskResult Execute(NDbConnection manager, NDbTaskParameter parameter)
		{
			MethodBase med = MethodBase.GetCurrentMethod();

			#region Check

			if (null == parameter)
				return new NDbTaskResult("No Parameters.");
			if (!(parameter is SqlServerRestoreDatabaseParameter))
				return new NDbTaskResult("Parameter Type is not match.");
			if (!parameter.IsValid())
				return new NDbTaskResult("Invalid Parameter.");
			if (null == manager)
				return new NDbTaskResult("Not Assigned Connection.");
			if (null == manager.Config)
				return new NDbTaskResult("Not Assigned Config.");
			if (!(manager.Config is SqlServerConfig))
				return new NDbTaskResult("Invalid Config Type.");
			if (!manager.IsConnected)
				return new NDbTaskResult("Connection is not connected.");

			#endregion

			SqlServerRestoreDatabaseParameter param = (parameter as SqlServerRestoreDatabaseParameter);

			if (!System.IO.File.Exists(param.BackupFileName))
			{
				new NDbTaskResult("Backup file is not found." + param.BackupFileName);
			}

			string BackupLogicalDataName = string.Empty;
			string BackupLogicalLogName = string.Empty;
			string TargetMDF = string.Empty;
			string TargetLDF = string.Empty;

			string str = string.Empty;
			str += "USE " + param.DatabaseName;
			str += ";";
			str += "exec sp_helpfile";
			str += ";";
			str += @"USE " + (manager.Config as NLib.Data.SqlServerConfig).DataSource.DatabaseName;
			str += @";";

			#region Get Backup Information

			string str2 = "RESTORE FILELISTONLY FROM DISK = N'" + param.BackupFileName + "'";
			try
			{
				//DataTable table = manager.Query(str, false);
				DataTable table = manager.Query(str);
				//DataTable table2 = manager.Query(str2, false);
				DataTable table2 = manager.Query(str2);
				if (null == table || null == table2 ||
					table.Rows.Count != 2 || table2.Rows.Count != 2)
				{
					new NDbTaskResult("Cannot find information about database " + param.DatabaseName);
				}

				string fileName;
				// first record
				fileName = table.Rows[0]["filename"].ToString().Trim();
				if (fileName.Substring(fileName.Length - 3, 3).ToUpper() == "MDF")
				{
					TargetMDF = fileName;
				}
				else if (fileName.Substring(fileName.Length - 3, 3).ToUpper() == "LDF")
				{
					TargetLDF = fileName;
				}
				// second record
				fileName = table.Rows[1]["filename"].ToString().Trim();
				if (fileName.Substring(fileName.Length - 3, 3).ToUpper() == "MDF")
				{
					TargetMDF = fileName;
				}
				else if (fileName.Substring(fileName.Length - 3, 3).ToUpper() == "LDF")
				{
					TargetLDF = fileName;
				}

				// first backup info row
				if (table2.Rows[0]["Type"].ToString().Trim() == "D")
				{
					BackupLogicalDataName = table2.Rows[0]["LogicalName"].ToString().Trim();
				}
				else if (table2.Rows[0]["Type"].ToString().Trim() == "L")
				{
					BackupLogicalLogName = table2.Rows[0]["LogicalName"].ToString().Trim();
				}
				// second backup info row
				if (table2.Rows[1]["Type"].ToString().Trim() == "D")
				{
					BackupLogicalDataName = table2.Rows[1]["LogicalName"].ToString().Trim();
				}
				else if (table2.Rows[1]["Type"].ToString().Trim() == "L")
				{
					BackupLogicalLogName = table2.Rows[1]["LogicalName"].ToString().Trim();
				}
			}
			catch (Exception ex)
			{
				new NDbTaskResult("Cannot find information about database " + param.DatabaseName + " - " + ex.ToString());
			}
			finally
			{
			}

			#endregion

			if (!System.IO.File.Exists(TargetMDF))
			{
				new NDbTaskResult("MDF file is not found " + TargetMDF);
			}

			if (!System.IO.File.Exists(TargetLDF))
			{
				new NDbTaskResult("LDF file is not found " + TargetLDF);
			}

			#region Security for SQLEXPRESS

			string account = param.Account;
			//string account = "NETWORK SERVICE"; // SQLEXPRESS runs under this account

			FileSystemRights rights = FileSystemRights.FullControl;
			AccessControlType controlType = AccessControlType.Allow;
			FileSecurity fSecurity1 = null;
			FileSecurity fSecurity2 = null;
			FileSecurity fSecurity3 = null;

			try
			{
				#region For Backup File

				"Grant permission for Backup File.".Info(med);
				// Get a FileSecurity object that represents the
				// current security settings.
				fSecurity1 = File.GetAccessControl(param.BackupFileName);
				// Add the FileSystemAccessRule to the security settings.
				fSecurity1.AddAccessRule(new FileSystemAccessRule(account,
					rights, controlType));

				// Set the new access settings.
				File.SetAccessControl(param.BackupFileName, fSecurity1);

				#endregion

				#region For MDF File

				"Grant permission for MDF File.".Info(med);
				// Get a FileSecurity object that represents the
				// current security settings.
				fSecurity2 = File.GetAccessControl(TargetMDF);
				// Add the FileSystemAccessRule to the security settings.
				fSecurity2.AddAccessRule(new FileSystemAccessRule(account,
					rights, controlType));

				// Set the new access settings.
				File.SetAccessControl(TargetMDF, fSecurity2);

				#endregion

				#region For LDF File

				"Grant permission for LDF File.".Info(med);
				// Get a FileSecurity object that represents the
				// current security settings.
				fSecurity3 = File.GetAccessControl(TargetLDF);
				// Add the FileSystemAccessRule to the security settings.
				fSecurity3.AddAccessRule(new FileSystemAccessRule(account,
					rights, controlType));

				// Set the new access settings.
				File.SetAccessControl(TargetLDF, fSecurity3);

				#endregion
			}
			catch (Exception ex)
			{
				"Grant Permission Failed.".Err(med);
				ex.Err(med);
			}

			#endregion

			string code = string.Empty;
			code += @"USE master";
			code += @";";
			code += @"alter database " + param.DatabaseName + " set single_user with rollback immediate";
			code += @";";
			code += @"RESTORE FILELISTONLY FROM DISK = N'" + param.BackupFileName + "'";
			code += @";";
			code += @"RESTORE DATABASE " + param.DatabaseName + " FROM DISK = N'" + param.BackupFileName + "'";
			code += @" WITH REPLACE, MOVE '" + BackupLogicalDataName + @"' TO N'" + TargetMDF + "',";
			code += @" REPLACE, MOVE '" + BackupLogicalLogName + @"' TO N'" + TargetLDF + "'";
			code += @";";
			code += @"alter database " + param.DatabaseName + " set multi_user";
			code += @";";
			code += @"USE " + (manager.Config as NLib.Data.SqlServerConfig).DataSource.DatabaseName;
			code += @";";

			NDbTaskResult result;

			try
			{
				int original = (manager.Config as NLib.Data.SqlServerConfig).Timeout.CommandTimeoutInSeconds;
				// Command timeout infinite
				(manager.Config as NLib.Data.SqlServerConfig).Timeout.CommandTimeoutInSeconds = 0;
				//manager.Execute(code, false);
				ExecuteResult<int> result1 = manager.ExecuteNonQuery(code);
				// restore timeout
				(manager.Config as NLib.Data.SqlServerConfig).Timeout.CommandTimeoutInSeconds = original;

				if (result1.HasException)
					result = new NDbTaskResult("Restore database incompleted : " + result1.Exception.Message);
				else result = new NDbTaskResult("Restore database Completed", true);
			}
			catch (Exception ex)
			{
				result = new NDbTaskResult("Restore database incompleted : " + ex.Message);
			}
			finally
			{
			}

			#region Remove Security Access

			try
			{
				if (fSecurity1 != null)
				{
					"Revoke permission for Backup File.".Info(med);
					// Remove the FileSystemAccessRule from the security settings.
					fSecurity1.RemoveAccessRule(new FileSystemAccessRule(account,
						rights, controlType));
				}
				if (fSecurity2 != null)
				{
					"Ignore Revoke permission for MDF File.".Info(med);
					// Remove the FileSystemAccessRule from the security settings.
					/*
					fSecurity2.RemoveAccessRule(new FileSystemAccessRule(account,
						rights, controlType));
					*/
				}
				if (fSecurity3 != null)
				{
					"Ignore Revoke permission for LDF File.".Info(med);
					// Remove the FileSystemAccessRule from the security settings.
					/*
					fSecurity3.RemoveAccessRule(new FileSystemAccessRule(account,
						rights, controlType));
					*/
				}
			}
			catch (Exception ex)
			{
				"Revoke Permission Failed.".Err(med);
				ex.Err(med);
			}

			#endregion

			return result;
		}

		#endregion

		#region Static

		#region Get Task Name

		/// <summary>
		/// Gets Task Name.
		/// </summary>
		public new static string TaskName
		{
			get { return "Restore Database"; }
		}

		#endregion

		#region Create

		/// <summary>
		/// Create DbTask Instance.
		/// </summary>
		/// <returns>Returns DbTask Instance.</returns>
		public new static NDbTask Create() { return new SqlServerRestoreDatabaseTask(); }

		#endregion

		#endregion
	}

	#endregion

	#region Attach

	/// <summary>
	/// SqlServer Attach Database Parameter.
	/// </summary>
	public class SqlServerAttachDatabaseParameter : NDbTaskParameter
	{
		#region Internal Variable

		private string _databaseName = string.Empty;
		private string physicalTargetMDFFile = string.Empty;
		private string physicalTargetLDFFile = string.Empty;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerAttachDatabaseParameter() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerAttachDatabaseParameter() { }

		#endregion

		#region Override

		/// <summary>
		/// Checks Is Parameter Valid.
		/// </summary>
		/// <returns>Returns true if parameter is valid.</returns>
		public override bool IsValid()
		{
			if (string.IsNullOrWhiteSpace(_databaseName)) return false;
			if (string.IsNullOrWhiteSpace(physicalTargetMDFFile)) return false;
			// not used for single file attach
			//if (physicalTargetLDFFile.Length <= 0) return false;
			return true;
		}

		#endregion

		#region Public Property

		/// <summary>
		/// Gets or sets Database Name.
		/// </summary>
		[Category("Parameter")]
		[Description("Get/Set Database Name.")]
		[DefaultValue("")]
		public string DatabaseName
		{
			get { return _databaseName; }
			set { _databaseName = value; }
		}
		/// <summary>
		/// Gets or sets Physical Master Data File.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets Physical Master Data File.")]
		[DefaultValue("")]
		[Editor(typeof(OpenMDFFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string PhysicalMDF
		{
			get { return physicalTargetMDFFile; }
			set { physicalTargetMDFFile = value; }
		}
		/// <summary>
		/// Gets or sets Physical Log Data File.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets Physical Log Data File.")]
		[DefaultValue("")]
		[Editor(typeof(OpenLDFFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string PhysicalLDF
		{
			get { return physicalTargetLDFFile; }
			set { physicalTargetLDFFile = value; }
		}


		#endregion
	}
	/// <summary>
	/// SqlServer Attach Database Task.
	/// </summary>
	public class SqlServerAttachDatabaseTask : NDbTask
	{
		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerAttachDatabaseTask() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerAttachDatabaseTask() { }

		#endregion

		#region Abstract Implement

		/// <summary>
		/// Gets Task Parameter.
		/// </summary>
		/// <param name="manager">The DbConnection that used for Get parameter for task.</param>
		/// <returns>Returns NDbTask Parameter that used for execute Task.</returns>
		public override NDbTaskParameter GetParameter(NDbConnection manager)
		{
			return new SqlServerAttachDatabaseParameter();
		}
		/// <summary>
		/// Execute Task.
		/// </summary>
		/// <param name="manager">The DbConnection that used for execute Task.</param>
		/// <param name="parameter">Task Parameter that used for execute Task.</param>
		/// <returns>Returns NDbTask Result instance after task is execute.</returns>
		public override NDbTaskResult Execute(NDbConnection manager, NDbTaskParameter parameter)
		{
			#region Check

			if (null == parameter)
				return new NDbTaskResult("No Parameters.");
			if (!(parameter is SqlServerAttachDatabaseParameter))
				return new NDbTaskResult("Parameter Type is not match.");
			if (!parameter.IsValid())
				return new NDbTaskResult("Invalid Parameter.");
			if (null == manager)
				return new NDbTaskResult("Not Assigned Connection.");
			if (null == manager.Config)
				return new NDbTaskResult("Not Assigned Config.");
			if (!(manager.Config is SqlServerConfig))
				return new NDbTaskResult("Invalid Config Type.");


			string lastDbName = string.Empty;
			bool isConnected = manager.IsConnected;

			if (!manager.IsConnected)
			{
				// keep original database's name.
				lastDbName = (manager.Config as SqlServerConfig).DataSource.DatabaseName;
				(manager.Config as SqlServerConfig).DataSource.DatabaseName = "master";
				// connect to database.
				manager.Connect();
			}

			if (!manager.IsConnected)
				return new NDbTaskResult("Connection is not connected.");

			#endregion

			SqlServerAttachDatabaseParameter param = (parameter as SqlServerAttachDatabaseParameter);

			if (!System.IO.File.Exists(param.PhysicalMDF))
				return new NDbTaskResult("MDF File Not Exists");

			bool singlefile = false;

			if (!System.IO.File.Exists(param.PhysicalLDF)) singlefile = true;

			string code = string.Empty;
			if (singlefile)
			{
				code = "EXECUTE sp_attach_single_file_db @dbname = N'" + param.DatabaseName + "', " +
					"@physname = N'" + param.PhysicalMDF + "'";
			}
			else
			{
				code = "EXECUTE sp_attach_db @dbname = N'" + param.DatabaseName + "', " +
					"@filename1 = N'" + param.PhysicalMDF + "', " +
					"@filename2 = N'" + param.PhysicalLDF + "'";
			}

			NDbTaskResult result = null;
			try
			{
				//manager.Execute(code, false);
				ExecuteResult<int> result1 = manager.ExecuteNonQuery(code);
				if (result1.HasException)
					result = new NDbTaskResult("Database attach incompleted : " + result1.Exception.Message);
				else result = new NDbTaskResult("Database Attached.", true);
			}
			catch (Exception ex)
			{
				result = new NDbTaskResult("Database attach incompleted : " + ex.Message);
			}

			// disconnect if the prior state is not connected.
			if (!isConnected)
			{
				manager.Disconnect();
				(manager.Config as SqlServerConfig).DataSource.DatabaseName = lastDbName;
			}

			return result;
		}

		#endregion

		#region Static

		#region Get Task Name

		/// <summary>
		/// Gets Task Name.
		/// </summary>
		public new static string TaskName
		{
			get { return "Attach Database"; }
		}

		#endregion

		#region Create

		/// <summary>
		/// Create DbTask Instance.
		/// </summary>
		/// <returns>Returns DbTask Instance.</returns>
		public new static NDbTask Create() { return new SqlServerAttachDatabaseTask(); }

		#endregion

		#endregion
	}

	#endregion

	#region Detach

	/// <summary>
	/// SqlServer Detach Database Parameter.
	/// </summary>
	public class SqlServerDetachDatabaseParameter : NDbTaskParameter
	{
		#region Internal Variable

		private string _dbname = string.Empty;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerDetachDatabaseParameter() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerDetachDatabaseParameter() { }

		#endregion

		#region Override

		/// <summary>
		/// Checks Is Parameter Valid.
		/// </summary>
		/// <returns>Returns true if parameter is valid.</returns>
		public override bool IsValid()
		{
			if (string.IsNullOrWhiteSpace(_dbname)) return false;
			return true;
		}

		#endregion

		#region Public Property

		/// <summary>
		/// Gets or sets Database Name to Detach.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets Database Name to Detach.")]
		[DefaultValue("")]
		public string DatabaseName { get { return _dbname; } set { _dbname = value; } }

		#endregion
	}
	/// <summary>
	/// SqlServer Detach Database Task.
	/// </summary>
	public class SqlServerDetachDatabaseTask : NDbTask
	{
		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerDetachDatabaseTask() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerDetachDatabaseTask() { }

		#endregion

		#region Abstract Implement

		/// <summary>
		/// Gets Task Parameter.
		/// </summary>
		/// <param name="manager">The DbConnection that used for Get parameter for task.</param>
		/// <returns>Returns NDbTask Parameter that used for execute Task.</returns>
		public override NDbTaskParameter GetParameter(NDbConnection manager)
		{
			return new SqlServerDetachDatabaseParameter();
		}
		/// <summary>
		/// Execute Task.
		/// </summary>
		/// <param name="manager">The DbConnection that used for execute Task.</param>
		/// <param name="parameter">Task Parameter that used for execute Task.</param>
		/// <returns>Returns NDbTask Result instance after task is execute.</returns>
		public override NDbTaskResult Execute(NDbConnection manager, NDbTaskParameter parameter)
		{
			#region Check

			if (null == parameter)
				return new NDbTaskResult("No Parameters.");
			if (!(parameter is SqlServerDetachDatabaseParameter))
				return new NDbTaskResult("Parameter Type is not match.");
			if (!parameter.IsValid())
				return new NDbTaskResult("Invalid Parameter.");
			if (null == manager)
				return new NDbTaskResult("Not Assigned Connection.");
			if (null == manager.Config)
				return new NDbTaskResult("Not Assigned Config.");
			if (!(manager.Config is SqlServerConfig))
				return new NDbTaskResult("Invalid Config Type.");
			if (!manager.IsConnected)
				return new NDbTaskResult("Connection is not connected.");

			#endregion

			SqlServerDetachDatabaseParameter param = (parameter as SqlServerDetachDatabaseParameter);
			string code = string.Empty;
			code += @"USE master";
			code += @";";
			code += @"EXECUTE sp_detach_db @dbname = N'" + param.DatabaseName + "'";
			code += @";";
			try
			{
				//manager.Execute(code, false);
				ExecuteResult<int> result1 = manager.ExecuteNonQuery(code);
				if (result1.HasException)
					return new NDbTaskResult("Detach database incompleted : " + result1.Exception.Message);
				else return new NDbTaskResult("Detach database Completed", true);
			}
			catch (Exception ex)
			{
				return new NDbTaskResult("Detach database incompleted : " + ex.Message);
			}
		}

		#endregion

		#region Static

		#region Get Task Name

		/// <summary>
		/// Gets Task Name.
		/// </summary>
		public new static string TaskName
		{
			get { return "Detach Database"; }
		}

		#endregion

		#region Create

		/// <summary>
		/// Create DbTask Instance.
		/// </summary>
		/// <returns>Returns DbTask Instance.</returns>
		public new static NDbTask Create() { return new SqlServerDetachDatabaseTask(); }

		#endregion

		#endregion
	}

	#endregion

	#region Shrink

	/// <summary>
	/// SqlServer Shrink Database Parameter.
	/// </summary>
	public class SqlServerShrinkDatabaseParameter : NDbTaskParameter
	{
		#region Internal Variable

		private string _dbname = string.Empty;
		private int _size = 10;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerShrinkDatabaseParameter() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerShrinkDatabaseParameter() { }

		#endregion

		#region Override

		/// <summary>
		/// Checks Is Parameter Valid.
		/// </summary>
		/// <returns>Returns true if parameter is valid.</returns>
		public override bool IsValid()
		{
			if (string.IsNullOrWhiteSpace(_dbname)) return false;
			if (_size <= 1) _size = 1;
			return true;
		}

		#endregion

		#region Public Property

		/// <summary>
		/// Gets or sets Database Name to Shrink.
		/// </summary>
		[Category("Parameter")]
		[Description("Get/Set Database Name to Shrink.")]
		[DefaultValue("")]
		public string DatabaseName { get { return _dbname; } set { _dbname = value; } }
		/// <summary>
		/// Gets or sets Shrink Size.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets Shrink Size.")]
		[DefaultValue(10)]
		public int ShrinkToSize { get { return _size; } set { _size = value; } }

		#endregion
	}
	/// <summary>
	/// SqlServer Shrink Database Task.
	/// </summary>
	public class SqlServerShrinkDatabaseTask : NDbTask
	{
		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerShrinkDatabaseTask() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerShrinkDatabaseTask() { }

		#endregion

		#region Abstract Implement

		/// <summary>
		/// Gets Task Parameter.
		/// </summary>
		/// <param name="manager">The DbConnection that used for Get parameter for task.</param>
		/// <returns>Returns NDbTask Parameter that used for execute Task.</returns>
		public override NDbTaskParameter GetParameter(NDbConnection manager)
		{
			return new SqlServerShrinkDatabaseParameter();
		}
		/// <summary>
		/// Execute Task.
		/// </summary>
		/// <param name="manager">The DbConnection that used for execute Task.</param>
		/// <param name="parameter">Task Parameter that used for execute Task.</param>
		/// <returns>Returns NDbTask Result instance after task is execute.</returns>
		public override NDbTaskResult Execute(NDbConnection manager, NDbTaskParameter parameter)
		{
			#region Check

			if (null == parameter)
				return new NDbTaskResult("No Parameters.");
			if (!(parameter is SqlServerShrinkDatabaseParameter))
				return new NDbTaskResult("Parameter Type is not match.");
			if (!parameter.IsValid())
				return new NDbTaskResult("Invalid Parameter.");
			if (null == manager)
				return new NDbTaskResult("Not Assigned Connection.");
			if (null == manager.Config)
				return new NDbTaskResult("Not Assigned Config.");
			if (!(manager.Config is SqlServerConfig))
				return new NDbTaskResult("Invalid Config Type.");
			if (!manager.IsConnected)
				return new NDbTaskResult("Connection is not connected.");

			#endregion

			SqlServerShrinkDatabaseParameter param = (parameter as SqlServerShrinkDatabaseParameter);

			/*
			-- SHRINK DATABASE allow 10 percent free space 
			use master;
			DBCC SHRINKDATABASE (OHS, 10);
			
			-- SHINK FILE (log) reduce to 10MB
			use OHS;
			DBCC SHRINKFILE (OHS_Log, 10)
			*/

			string code = string.Empty;
			code += @"USE master";
			code += @";";
			code += @"DBCC SHRINKDATABASE (" + param.DatabaseName + ", " + param.ShrinkToSize.ToString() + ")";
			code += @";";

			bool completed = false;
			try
			{
				//manager.Execute(code, false);
				ExecuteResult<int> result1 = manager.ExecuteNonQuery(code);
				if (result1.HasException)
				{
					completed = false;
					return new NDbTaskResult("Shrink database incompleted : " + result1.Exception.Message);
				}
				else completed = true;
			}
			catch (Exception ex)
			{
				return new NDbTaskResult("Shrink database incompleted : " + ex.Message);
			}

			code = string.Empty;
			code += @"USE " + param.DatabaseName;
			code += @";";
			code += @"exec sp_helpfile";
			code += @";";

			try
			{
				//manager.Execute(code, false);
				DataTable table = manager.Query(code);
				if (null != table && table.Rows.Count > 0)
				{
					string logFName = "";
					foreach (DataRow row in table.Rows)
					{
						if (row["filename"] == DBNull.Value && row["name"] == DBNull.Value)
							continue;
						string fileName = row["filename"].ToString().Trim();
						if (string.Compare(fileName, fileName.Length - 3, "ldf", 0, 3) == 0)
						{
							logFName = row["name"].ToString().Trim();
							break;
						}
					}

					if (string.IsNullOrWhiteSpace(logFName))
					{
						code = string.Empty;
						code += "DBCC SHRINKFILE (" + logFName + ", " + param.ShrinkToSize.ToString() + ")";
						code += ";";
						try
						{
							//manager.Execute(code, false);
							ExecuteResult<int> result2 = manager.ExecuteNonQuery(code);
							if (result2.HasException)
							{
								completed = false;
								return new NDbTaskResult("Shrink database incompleted : " + result2.Exception.Message);
							}
							else completed = true;
						}
						catch (Exception ex2)
						{
							return new NDbTaskResult("Shrink database incompleted : " + ex2.Message);
						}
					}
				}
			}
			catch (Exception ex)
			{
				return new NDbTaskResult("Shrink database incompleted : " + ex.Message);
			}
			if (completed)
			{
				return new NDbTaskResult("Shrink database completed : ", true);
			}
			else return new NDbTaskResult("Shrink database incompleted.");
		}

		#endregion

		#region Static

		#region Get Task Name

		/// <summary>
		/// Gets Task Name.
		/// </summary>
		public new static string TaskName
		{
			get { return "Shrink Database"; }
		}

		#endregion

		#region Create

		/// <summary>
		/// Create DbTask Instance.
		/// </summary>
		/// <returns>Returns DbTask Instance.</returns>
		public new static NDbTask Create() { return new SqlServerShrinkDatabaseTask(); }

		#endregion

		#endregion
	}

	#endregion

	#region Set Recovery Mode

	/// <summary>
	/// The SqlServer Recover Mode.
	/// </summary>
	public enum SqlServerRecoveryMode
	{
		/// <summary>
		/// Simple.
		/// </summary>
		Simple,
		/// <summary>
		/// Bulk Log.
		/// </summary>
		BulkLog,
		/// <summary>
		/// Full.
		/// </summary>
		Full
	}

	/// <summary>
	/// SqlServer Set Recovery Mode Parameter.
	/// </summary>
	public class SqlServerSetRecoveryModeParameter : NDbTaskParameter
	{
		#region Internal Variable

		private string _dbname = string.Empty;
		private SqlServerRecoveryMode _mode = SqlServerRecoveryMode.Simple;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerSetRecoveryModeParameter() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerSetRecoveryModeParameter() { }

		#endregion

		#region Override

		/// <summary>
		/// Checks Is Parameter Valid.
		/// </summary>
		/// <returns>Returns true if parameter is valid.</returns>
		public override bool IsValid()
		{
			if (string.IsNullOrWhiteSpace(_dbname)) return false;
			return true;
		}

		#endregion

		#region Public Property

		/// <summary>
		/// Gets or sets Database Name to Change Recovery Mode.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets Database Name to Change Recovery Mode.")]
		[DefaultValue("")]
		public string DatabaseName { get { return _dbname; } set { _dbname = value; } }
		/// <summary>
		/// Gets or sets Recovery Mode.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets Recovery Mode.")]
		[DefaultValue(SqlServerRecoveryMode.Simple)]
		public SqlServerRecoveryMode Mode { get { return _mode; } set { _mode = value; } }

		#endregion
	}
	/// <summary>
	/// SqlServer Set Recovery Mode Task.
	/// </summary>
	public class SqlServerSetRecoveryModeTask : NDbTask
	{
		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerSetRecoveryModeTask() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~SqlServerSetRecoveryModeTask() { }

		#endregion

		#region Abstract Implement

		/// <summary>
		/// Gets Task Parameter.
		/// </summary>
		/// <param name="manager">The DbConnection that used for Get parameter for task.</param>
		/// <returns>Returns NDbTask Parameter that used for execute Task.</returns>
		public override NDbTaskParameter GetParameter(NDbConnection manager)
		{
			return new SqlServerSetRecoveryModeParameter();
		}
		/// <summary>
		/// Execute Task.
		/// </summary>
		/// <param name="manager">The DbConnection that used for execute Task.</param>
		/// <param name="parameter">Task Parameter that used for execute Task.</param>
		/// <returns>Returns NDbTask Result instance after task is execute.</returns>
		public override NDbTaskResult Execute(NDbConnection manager, NDbTaskParameter parameter)
		{
			#region Check

			if (null == parameter)
				return new NDbTaskResult("No Parameters.");
			if (!(parameter is SqlServerSetRecoveryModeParameter))
				return new NDbTaskResult("Parameter Type is not match.");
			if (!parameter.IsValid())
				return new NDbTaskResult("Invalid Parameter.");
			if (null == manager)
				return new NDbTaskResult("Not Assigned Connection.");
			if (null == manager.Config)
				return new NDbTaskResult("Not Assigned Config.");
			if (!(manager.Config is SqlServerConfig))
				return new NDbTaskResult("Invalid Config Type.");
			if (!manager.IsConnected)
				return new NDbTaskResult("Connection is not connected.");

			#endregion

			SqlServerSetRecoveryModeParameter param = (parameter as SqlServerSetRecoveryModeParameter);

			/*
			use master;
			ALTER DATABASE [database_name] SET RECOVERY [FULL|BULK_LOGGED|SIMPLE];
			*/

			string dbName = string.Empty;
			string mode = string.Empty;

			#region Check database's name

			if (param.DatabaseName.Trim().StartsWith("["))
				dbName += param.DatabaseName.Trim();
			else dbName += "[" + param.DatabaseName.Trim();

			if (!dbName.EndsWith("]"))
			{
				dbName += "]";
			}

			if (param.Mode == SqlServerRecoveryMode.Simple)
			{
				mode = "SIMPLE";
			}
			else if (param.Mode == SqlServerRecoveryMode.BulkLog)
			{
				mode = "BULK_LOGGED";
			}
			else
			{
				mode = "FULL";
			}

			#endregion

			string code = string.Empty;
			code += @"USE master";
			code += @";";
			code += @"ALTER DATABASE " + dbName + " SET RECOVERY " + mode;
			code += @";";
			code += @"USE " + (manager.Config as SqlServerConfig).DataSource.DatabaseName; // change database's naeme
			code += @";";

			bool completed = false;
			try
			{
				//manager.Execute(code, false);
				ExecuteResult<int> result1 = manager.ExecuteNonQuery(code);
				if (result1.HasException)
				{
					completed = false;
					return new NDbTaskResult("Set recover mode incompleted : " + result1.Exception.Message);
				}
				else completed = true;
			}
			catch (Exception ex)
			{
				return new NDbTaskResult("Set recover mode incompleted : " + ex.Message);
			}

			if (completed)
			{
				return new NDbTaskResult("Set recover mode completed : ", true);
			}
			else return new NDbTaskResult("Set recover mode incompleted.");
		}

		#endregion

		#region Static

		#region Get Task Name

		/// <summary>
		/// Gets Task Name.
		/// </summary>
		public new static string TaskName
		{
			get { return "Set recover mode"; }
		}

		#endregion

		#region Create

		/// <summary>
		/// Create DbTask Instance.
		/// </summary>
		/// <returns>Returns DbTask Instance.</returns>
		public new static NDbTask Create() { return new SqlServerSetRecoveryModeTask(); }

		#endregion

		#endregion
	}

	#endregion

	#endregion
}
