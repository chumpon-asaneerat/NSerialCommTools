#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-07-07
=================
- Data Access Framework - ExcelConnectionFactory and Tasks updated.
  - Changed all log code used MethodBase.

======================================================================================================================
Update 2014-12-15
=================
- Data Access Framework - ExcelConnectionFactory
  - Update supports Execute Database tasks with re-coding and re-formatting.

======================================================================================================================
Update 2014-11-30
=================
- Data Access Framework - ExcelConnectionFactory
  - Add ExcelConnectionFactory class with new implements.

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
	#region Excel Connection Factory

	/// <summary>
	/// Excel Connection Factory.
	/// </summary>
	public class ExcelConnectionFactory : OleDbConnectionFactory
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
				return string.Format("[${0}]", objectName);
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
				else return string.Format("[${0}].[{1}]", objectName, columnName);
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
					ExcelCreateDatabaseTask.Create(), 
					ExcelDeleteDatabaseTask.Create() 
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

	#region Excel Tasks

	#region Create

	/// <summary>
	/// Excel Create Database Parameter.
	/// </summary>
	public class ExcelCreateDatabaseParameter : NDbTaskParameter
	{
		#region Internal Variable

		private string _databaseName = string.Empty;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExcelCreateDatabaseParameter() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~ExcelCreateDatabaseParameter() { }

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
		[Editor(typeof(SaveExcelFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string FileName
		{
			get { return _databaseName; }
			set { _databaseName = value; }
		}

		#endregion
	}
	/// <summary>
	/// Excel Create Database Task.
	/// </summary>
	public class ExcelCreateDatabaseTask : NDbTask
	{
		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExcelCreateDatabaseTask() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~ExcelCreateDatabaseTask() { }

		#endregion

		#region Abstract Implement

		/// <summary>
		/// Gets Task Parameter.
		/// </summary>
		/// <param name="manager">The DbConnection that used for Get parameter for task.</param>
		/// <returns>Returns NDbTask Parameter that used for execute Task.</returns>
		public override NDbTaskParameter GetParameter(NDbConnection manager)
		{
			return new ExcelCreateDatabaseParameter();
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
			if (!(parameter is ExcelCreateDatabaseParameter))
				return new NDbTaskResult("Parameter Type is not match.");
			if (!parameter.IsValid())
				return new NDbTaskResult("Invalid Parameter.");

			#endregion

			ExcelCreateDatabaseParameter param = (parameter as ExcelCreateDatabaseParameter);
			try
			{
				if (NLib.Utils.MsDocumentUtils.CreateExcel2003File(param.FileName, true))
					return new NDbTaskResult("Excel File Created Completed.", true);
				else
					return new NDbTaskResult("Excel File creation incompleted");
			}
			catch (Exception ex)
			{
				return new NDbTaskResult("Excel File creation incompleted : " + ex.Message);
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
		public new static NDbTask Create() { return new ExcelCreateDatabaseTask(); }

		#endregion

		#endregion
	}

	#endregion

	#region Delete

	/// <summary>
	/// Excel Delete Database Parameter.
	/// </summary>
	public class ExcelDeleteDatabaseParameter : NDbTaskParameter
	{
		#region Internal Variable

		private string _databaseName = string.Empty;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExcelDeleteDatabaseParameter() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~ExcelDeleteDatabaseParameter() { }

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
		[Editor(typeof(OpenExcelFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string FileName
		{
			get { return _databaseName; }
			set { _databaseName = value; }
		}

		#endregion
	}
	/// <summary>
	/// Excel Delete Database Task
	/// </summary>
	public class ExcelDeleteDatabaseTask : NDbTask
	{
		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExcelDeleteDatabaseTask() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~ExcelDeleteDatabaseTask() { }

		#endregion

		#region Abstract Implement

		/// <summary>
		/// Gets Task Parameter.
		/// </summary>
		/// <param name="manager">The DbConnection that used for Get parameter for task.</param>
		/// <returns>Returns NDbTask Parameter that used for execute Task.</returns>
		public override NDbTaskParameter GetParameter(NDbConnection manager)
		{
			return new ExcelDeleteDatabaseParameter();
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
			if (!(parameter is ExcelDeleteDatabaseParameter))
				return new NDbTaskResult("Parameter Type is not match.");
			if (!parameter.IsValid())
				return new NDbTaskResult("Invalid Parameter.");

			#endregion

			ExcelDeleteDatabaseParameter param = (parameter as ExcelDeleteDatabaseParameter);

			try
			{
				if (System.IO.File.Exists(param.FileName))
				{
					System.IO.File.Delete(param.FileName);
					return new NDbTaskResult("Excel File Deleted.", true);
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
		public new static NDbTask Create() { return new ExcelDeleteDatabaseTask(); }

		#endregion

		#endregion
	}

	#endregion

	#endregion
}
