#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-07-07
=================
- Data Access Framework - MsAccessConnectionFactory and Tasks updated.
  - Changed all log code used MethodBase.

======================================================================================================================
Update 2014-12-15
=================
- Data Access Framework - MsAccessConnectionFactory
  - Update supports Execute Database tasks with re-coding and re-formatting.

======================================================================================================================
Update 2014-11-30
=================
- Data Access Framework - MsAccessConnectionFactory
  - Add MsAccessConnectionFactory class with new implements.

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
	#region Ms Access Connection Factory

	/// <summary>
	/// Ms Access Connection Factory.
	/// </summary>
	public class MsAccessConnectionFactory : OleDbConnectionFactory
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

			base.InitFormatter(formatter); // call base class for init formatter.

			#region Init delegates

			formatter.FormatTableOrViewName = (string owner, string objectName) =>
			{
				if (string.IsNullOrWhiteSpace(objectName))
				{
					"Table's Name is not assigned.".Info(med);
					return string.Empty;
				}
				return string.Format("[{0}]", objectName);
			};
			formatter.FormatColumnName = (string owner, string objectName, string columnName) =>
			{
				if (string.IsNullOrWhiteSpace(objectName) && 
					string.IsNullOrWhiteSpace(columnName))
				{
					"Table's Name and Column's Name is not assigned.".Info(med);
					return string.Empty;
				}
				if (objectName.Trim().Length <= 0)
					return string.Format("[{0}]", columnName);
				else return string.Format("[{0}].[{1}]", objectName, columnName);
			};
			formatter.DateTimeToSqlString = (DateTime value, bool dateOnly) =>
			{
				if (dateOnly)
				{
					string format = "dd/MMM/yyyy";
					return "#" + value.ToString(format, DateTimeFormatInfo.InvariantInfo) + "#";
				}
				else
				{
					string format = "dd/MMM/yyyy HH:mm:ss";
					return "#" + value.ToString(format, DateTimeFormatInfo.InvariantInfo) + "#";
				}
			};

			#endregion
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
					MsAccessCreateDatabaseTask.Create(), 
					MsAccessDeleteDatabaseTask.Create(),
					MsAccessCompactDatabaseTask.Create()
				};
			}
			return _tasks;
		}

		#endregion

		#endregion
	}

	#endregion
}

namespace NLib.Data
{
	using NLib.Components; // For NDbConnection
	using NLib.Data.Design; // For Designer.

	#region MsAccess Tasks

	#region Create

	/// <summary>
	/// MsAccess Create Database Parameter.
	/// </summary>
	public class MsAccessCreateDatabaseParameter : NDbTaskParameter
	{
		#region Internal Variable

		private string _databaseName = string.Empty;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public MsAccessCreateDatabaseParameter() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~MsAccessCreateDatabaseParameter() { }

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
		/// Gets or sets File Name.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets File Name.")]
		[DefaultValue("")]
		[Editor(typeof(SaveMsAccessFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string FileName
		{
			get { return _databaseName; }
			set { _databaseName = value; }
		}

		#endregion
	}
	/// <summary>
	/// MsAccess Create Database Task.
	/// </summary>
	public class MsAccessCreateDatabaseTask : NDbTask
	{
		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public MsAccessCreateDatabaseTask() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~MsAccessCreateDatabaseTask() { }

		#endregion

		#region Abstract Implement

		/// <summary>
		/// Gets Task Parameter.
		/// </summary>
		/// <param name="manager">The DbConnection that used for Get parameter for task.</param>
		/// <returns>Returns NDbTask Parameter that used for execute Task.</returns>
		public override NDbTaskParameter GetParameter(NDbConnection manager)
		{
			return new MsAccessCreateDatabaseParameter();
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
			if (!(parameter is MsAccessCreateDatabaseParameter))
				return new NDbTaskResult("Parameter Type is not match.");
			if (!parameter.IsValid())
				return new NDbTaskResult("Invalid Parameter.");

			#endregion

			MsAccessCreateDatabaseParameter param = (parameter as MsAccessCreateDatabaseParameter);
			try
			{
				if (NLib.Utils.MsDocumentUtils.CreateMsAccess2003File(param.FileName, true))
					return new NDbTaskResult("Ms Access File Created Completed.", true);
				else
					return new NDbTaskResult("Ms Access File creation incompleted");
			}
			catch (Exception ex)
			{
				return new NDbTaskResult("Ms Access File creation incompleted : " + ex.Message);
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
			get { return "Create Database"; }
		}

		#endregion

		#region Create

		/// <summary>
		/// Create DbTask Instance.
		/// </summary>
		/// <returns>DbTask Instance.</returns>
		public new static NDbTask Create() { return new MsAccessCreateDatabaseTask(); }

		#endregion

		#endregion
	}

	#endregion

	#region Delete

	/// <summary>
	/// MsAccess Delete Database Parameter.
	/// </summary>
	public class MsAccessDeleteDatabaseParameter : NDbTaskParameter
	{
		#region Internal Variable

		private string _databaseName = string.Empty;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public MsAccessDeleteDatabaseParameter() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~MsAccessDeleteDatabaseParameter() { }

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
		/// Gets or sets File Name.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets File Name.")]
		[DefaultValue("")]
		[Editor(typeof(OpenMsAccessFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string FileName
		{
			get { return _databaseName; }
			set { _databaseName = value; }
		}

		#endregion
	}
	/// <summary>
	/// MsAccess Delete Database Task.
	/// </summary>
	public class MsAccessDeleteDatabaseTask : NDbTask
	{
		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public MsAccessDeleteDatabaseTask() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~MsAccessDeleteDatabaseTask() { }

		#endregion

		#region Abstract Implement

		/// <summary>
		/// Gets Task Parameter.
		/// </summary>
		/// <param name="manager">The DbConnection that used for Get parameter for task.</param>
		/// <returns>Returns NDbTask Parameter that used for execute Task.</returns>
		public override NDbTaskParameter GetParameter(NDbConnection manager)
		{
			return new MsAccessDeleteDatabaseParameter();
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
			if (!(parameter is MsAccessDeleteDatabaseParameter))
				return new NDbTaskResult("Parameter Type is not match.");
			if (!parameter.IsValid())
				return new NDbTaskResult("Invalid Parameter.");

			#endregion

			MsAccessDeleteDatabaseParameter param = (parameter as MsAccessDeleteDatabaseParameter);

			try
			{
				if (System.IO.File.Exists(param.FileName))
				{
					System.IO.File.Delete(param.FileName);
					return new NDbTaskResult("Ms Access File Deleted.", true);
				}
				else return new NDbTaskResult("File Not Found.");
			}
			catch (Exception ex)
			{
				return new NDbTaskResult("Cannot delete File : " + ex.Message);
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
		/// <returns>DbTask Instance.</returns>
		public new static NDbTask Create() { return new MsAccessDeleteDatabaseTask(); }

		#endregion

		#endregion
	}

	#endregion

	#region Compact

	/// <summary>
	/// MsAccess Compact Database Parameter.
	/// </summary>
	public class MsAccessCompactDatabaseParameter : NDbTaskParameter
	{
		#region Internal Variable

		private string _databaseName = string.Empty;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public MsAccessCompactDatabaseParameter() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~MsAccessCompactDatabaseParameter() { }

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
		/// Gets or sets File Name.
		/// </summary>
		[Category("Parameter")]
		[Description("Gets or sets File Name.")]
		[DefaultValue("")]
		[Editor(typeof(OpenMsAccessFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string FileName
		{
			get { return _databaseName; }
			set { _databaseName = value; }
		}

		#endregion
	}
	/// <summary>
	/// MsAccess Compact Database Task.
	/// </summary>
	public class MsAccessCompactDatabaseTask : NDbTask
	{
		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public MsAccessCompactDatabaseTask() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~MsAccessCompactDatabaseTask() { }

		#endregion

		#region Abstract Implement

		/// <summary>
		/// Gets Task Parameter.
		/// </summary>
		/// <param name="manager">The DbConnection that used for Get parameter for task.</param>
		/// <returns>Returns NDbTask Parameter that used for execute Task.</returns>
		public override NDbTaskParameter GetParameter(NDbConnection manager)
		{
			return new MsAccessCompactDatabaseParameter();
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
			if (!(parameter is MsAccessCompactDatabaseParameter))
				return new NDbTaskResult("Parameter Type is not match.");
			if (!parameter.IsValid())
				return new NDbTaskResult("Invalid Parameter.");

			#endregion

			MsAccessCompactDatabaseParameter param = (parameter as MsAccessCompactDatabaseParameter);

			object objJRO = null;

			MsAccessConfig _tempConfig = new MsAccessConfig();
			try
			{
				if (System.IO.File.Exists(param.FileName))
				{
					_tempConfig.DataSource.FileName = param.FileName;

					object[] oParams;

					//create an inctance of a Jet Replication Object
					objJRO = Activator.CreateInstance(Type.GetTypeFromProgID("JRO.JetEngine"));
					string fileName = System.IO.Path.GetTempFileName();

					System.IO.File.Delete(fileName); // Remove file first because GetTempFileName will create blank file

					string connectionString = _tempConfig.ConnectionString;

					if (objJRO != null)
					{
						// filling Parameters array
						// cnahge "Jet OLEDB:Engine Type=5" to an appropriate value
						// or leave it as is if you db is JET4X format (access 2000,2002)
						// (yes, jetengine5 is for JET4X, no misprint here)
						oParams = new object[] 
						{
							connectionString,
							"Provider=Microsoft.Jet.OLEDB.4.0;Data" + 
							" Source=" + fileName + ";Jet OLEDB:Engine Type=5"
						};

						// invoke a CompactDatabase method of a JRO object
						// pass Parameters array
						objJRO.GetType().InvokeMember("CompactDatabase",
							System.Reflection.BindingFlags.InvokeMethod,
							null,
							objJRO,
							oParams);

						// database is compacted now
						// to a new file Temp File
						// let's copy it over an old one and delete it

						string backupFileName = param.FileName + ".bak.mdb";

						if (System.IO.File.Exists(backupFileName))
							System.IO.File.Delete(backupFileName); // Remove backup file

						System.IO.File.Move(param.FileName, backupFileName); // Move original
						System.IO.File.Move(fileName, param.FileName); // move compacted file

						//clean up (just in case)
						System.Runtime.InteropServices.Marshal.ReleaseComObject(objJRO);

						objJRO = null;

						return new NDbTaskResult("Ms Access File Compactd.", true);
					}
					else
					{
						return new NDbTaskResult("Cannot create JRO Object.");
					}
				}
				else return new NDbTaskResult("File Not Found.");
			}
			catch (Exception ex)
			{
				if (objJRO != null)
				{
					try
					{
						System.Runtime.InteropServices.Marshal.ReleaseComObject(objJRO);
					}
					catch { }
				}
				return new NDbTaskResult("Cannot Compact File : " + ex.ToString());
			}
			finally
			{
				_tempConfig = null;
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
			get { return "Compact Database"; }
		}

		#endregion

		#region Create

		/// <summary>
		/// Create DbTask Instance.
		/// </summary>
		/// <returns>DbTask Instance.</returns>
		public new static NDbTask Create() { return new MsAccessCompactDatabaseTask(); }

		#endregion

		#endregion
	}

	#endregion

	#endregion
}
