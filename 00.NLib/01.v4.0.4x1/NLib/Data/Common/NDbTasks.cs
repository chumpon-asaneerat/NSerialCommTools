#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-12-15
=================
- DataAccess library Task ported from NLibv401 to NLibv404.
  - Change class name to NDbTask, NDbTaskParameter, NDbTaskResult.

======================================================================================================================
Update 2008-10-25
=================
- DataAccess library Task ported from GFA37 to GFA38v3.

======================================================================================================================
Update 2008-07-07
=================
- DataAccess library new Features add
  - Database supported related task classes added.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.ComponentModel;

#endregion

#region Using Extras

using NLib.Components;

#endregion

#region Base class

namespace NLib.Data
{
    #region NDbTask (abstract class)

    /// <summary>
    /// NDbTask (abstract class)
    /// </summary>
    public abstract class NDbTask
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NDbTask()
            : base()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~NDbTask()
        {
        }

        #endregion

        #region Override Method

        /// <summary>
        /// Display String for Instance see Object.ToString()
        /// </summary>
        /// <returns>see Object.ToString()</returns>
        public override string ToString()
        {
            string taskName = GetTaskName();
            if (string.IsNullOrWhiteSpace(taskName))
                return base.ToString();
            return taskName;
        }

        #endregion

        #region Public Method

        #region Get Task Name

        /// <summary>
        /// Get Task Name
        /// </summary>
        public string GetTaskName()
        {
            return NLib.Reflection.TypeUtils.GetReferenceName(this, staticPropertyName);
        }

        #endregion

        #endregion

        #region Abstract Method

        /// <summary>
        /// Gets Task Parameter.
        /// </summary>
        /// <param name="manager">The DbConnection that used for Get parameter for task.</param>
        /// <returns>Returns NDbTask Parameter that used for execute Task.</returns>
        public abstract NDbTaskParameter GetParameter(NDbConnection manager);
        /// <summary>
        /// Execute Task.
        /// </summary>
        /// <param name="manager">The DbConnection that used for execute Task.</param>
        /// <param name="parameter">Task Parameter that used for execute Task.</param>
        /// <returns>Returns NDbTask Result instance after task is execute.</returns>
        public abstract NDbTaskResult Execute(NDbConnection manager, NDbTaskParameter parameter);

        #endregion

        #region Static Access

        private static Type _baseType = typeof(NDbTask);
        private static string staticPropertyName = "TaskName";

        #region Create DbTask By Task Name

        /// <summary>
        /// Create DbTask instance By Task Name.
        /// </summary>
        /// <param name="taskName">Task's Name.</param>
        /// <returns>Returns DbTask's Instance.</returns>
        public static NDbTask Create(string taskName)
        {
            NDbTask instance = null;
            Type[] types = GetTasks();
            if (types == null || types.Length <= 0)
                instance = null;
            else
            {
                object val = NLib.Reflection.TypeUtils.Create(_baseType, 
                    staticPropertyName, taskName, "Create");
                if (val != null && val.GetType().IsSubclassOf(_baseType))
                    instance = (val as NDbTask);
                else return instance;
            }
            return instance;
        }

        #endregion

        #region Get Task Names

        /// <summary>
        /// Gets Task Names.
        /// </summary>
        /// <returns>Returns Avaliable Task's Names.</returns>
        public static string[] GetTaskNames()
        {
            return NLib.Reflection.TypeUtils.GetInheritedTypeNames(_baseType, staticPropertyName);
        }

        #endregion

        #region Get Tasks

        /// <summary>
        /// Gets Tasks.
        /// </summary>
        /// <returns>Returns List of Avaliable Type that inherited from NDbTask.</returns>
        public static Type[] GetTasks()
        {
            return NLib.Reflection.TypeUtils.GetInheritedTypes(_baseType);
        }
        /// <summary>
        /// Gets Tasks.
        /// </summary>
        /// <param name="refresh">true for refresh cache</param>
        /// <returns>Returns List of Avaliable Type that inherited from NDbTask.</returns>
        public static Type[] GetTasks(bool refresh)
        {
            return NLib.Reflection.TypeUtils.GetInheritedTypes(_baseType, refresh);
        }

        #endregion

        #region Get Task Name

        /// <summary>
        /// Gets Task Name.
        /// </summary>
        public static string TaskName
        {
            get { return "abstract task"; }
        }

        #endregion

        #region Create

        /// <summary>
        /// Create DbTask Instance.
        /// </summary>
        /// <returns>DbTask Instance.</returns>
        public static NDbTask Create() { return null; }

        #endregion

        #endregion
    }

    #endregion

    #region NDbTask Parameter (abstract class)

    /// <summary>
    /// NDbTask Parameter (abstract class).
    /// </summary>
    public abstract class NDbTaskParameter
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NDbTaskParameter() : base() { }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NDbTaskParameter()
        {
        }

        #endregion

        #region Abstract Method

        /// <summary>
        /// Checks Is Parameter Valid.
        /// </summary>
        /// <returns>Returns true if parameter is valid.</returns>
        public abstract bool IsValid();

        #endregion
    }

    #endregion

    #region NDbTask Result

    /// <summary>
    /// NDbTask Result.
    /// </summary>
    public class NDbTaskResult
    {
        #region Internal Variable

        private string _result = string.Empty;
        private bool _completed = false;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NDbTaskResult()
            : base()
        {
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="result">The Result.</param>
        public NDbTaskResult(string result)
        {
            _result = result;
            _completed = false;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="result">The Result.</param>
        /// <param name="completed">The completed flag.</param>
        public NDbTaskResult(string result, bool completed)
        {
            _result = result;
            _completed = completed;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NDbTaskResult()
        {
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets or sets Result.
        /// </summary>
        [Browsable(false)]
        public string ResultText { get { return _result; } set { _result = value; } }
        /// <summary>
        /// Gets or sets Is Task Process completed.
        /// </summary>
        [Browsable(false)]
        public bool Completed { get { return _completed; } set { _completed = value; } }

        #endregion
    }

    #endregion
}

#endregion