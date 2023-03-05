#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-07-07
=================
- NThread framework updated.
  - Changed all log code used MethodBase.

======================================================================================================================
Update 2013-01-27
=================
- NThread framework added.
  - Supports Init, Execute, Finished handler.
  - Supports custom user state.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

#endregion

#region Extra using

using NLib;

#endregion

namespace NLib.Threads
{
    #region NThreadState

    /// <summary>
    /// NThread State class.
    /// </summary>
    public class NThreadState
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        internal NThreadState()
            : base()
        {
            this.IsCanceled = false;
            this.Timeout = -1;
            this.IsTimeout = false;
            this.IsCompleted = false;
            this.IsFinished = false;

            this.StartTime = DateTime.Now;
            this.FinishedTime = DateTime.Now;
        }
        /// <summary>
        /// Constrcutor.
        /// </summary>
        /// <param name="timeout">The timeout in millisecond. -1 for no timeout.</param>
        internal NThreadState(int timeout)
            : this()
        {
            this.Timeout = timeout;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Cancel current execution code.
        /// </summary>
        public void Cancel()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            med.Info("Cancel is called.");
            this.IsCanceled = true;
        }
        /// <summary>
        /// Call for update that current operation is completed.
        /// </summary>
        public void Completed()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            med.Info("Completed is called.");
            this.IsCompleted = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the start time.
        /// </summary>
        public DateTime StartTime { get; internal set; }
        /// <summary>
        /// Gets the finished time.
        /// </summary>
        public DateTime FinishedTime { get; internal set; }
        /// <summary>
        /// Checks is thread is cancel.
        /// </summary>
        public bool IsCanceled { get; internal set; }
        /// <summary>
        /// Checks is thread is completed.
        /// </summary>
        public bool IsCompleted { get; internal set; }
        /// <summary>
        /// Gets the timeout in millisecond. -1 for no timeout.
        /// </summary>
        public int Timeout { get; internal set; }
        /// <summary>
        /// Checks is thread is timeout.
        /// </summary>
        public bool IsTimeout { get; internal set; }
        /// <summary>
        /// Checks is operation is finished.
        /// </summary>
        public bool IsFinished { get; internal set; }
        /// <summary>
        /// Gets the total execute time.
        /// </summary>
        public TimeSpan ExecuteTime
        {
            get
            {
                TimeSpan ts;
                if (!this.IsFinished)
                {
                    ts = DateTime.Now - this.StartTime;
                }
                else ts = this.FinishedTime - this.StartTime;

                return ts;
            }
        }
        /// <summary>
        /// Gets or sets user state.
        /// </summary>
        public object UserState { get; set; }

        #endregion
    }

    #endregion

    #region NThreadExecuteHandler

    /// <summary>
    /// NThread Execute Handler.
    /// </summary>
    /// <param name="state">The thread state variable.</param>
    public delegate void NThreadExecuteHandler(NThreadState state);

    #endregion

    #region NThread

    /// <summary>
    /// NThread class. This class is wrapper class that used for handle user code in another thread.
    /// The NThread class is provide basic thread handler and automatic detected cleanup thread
    /// that not shutdown properly.
    /// </summary>
    public class NThread
    {
        #region Internal Variables

        private Thread _th = null;
        private bool _isRunning = false;
        private string _name = string.Empty;

        private NThreadExecuteHandler _initHandler = null;
        private NThreadExecuteHandler _executeHandler = null;
        private NThreadExecuteHandler _finihseHandler = null;

        private NThreadState _state = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private NThread() : base() { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The thread name.</param>
        internal NThread(string name)
            : this()
        {
            _name = name;
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~NThread()
        {
            Shutdown();
        }

        #endregion

        #region Private Methods

        #region Event Raisers

        private void Invoke(NThreadExecuteHandler del, NThreadState state)
        {
            if (null != del)
            {
                ApplicationManager.Instance.Invoke(del, state);
            }
        }

        private void RaiseInitHandler()
        {
            // Invoke finished process handler
            if (null != _state)
            {
                Invoke(_initHandler, _state); // finished action code.
            }
        }

        private void RaiseFinishedHandler()
        {
            // Invoke finished process handler
            if (null != _state)
            {
                // set finished time.
                _state.FinishedTime = DateTime.Now;
                _state.IsFinished = true;
                Invoke(_finihseHandler, _state); // finished action code.
            }
        }

        #endregion

        #region Helper methods

        private bool IsExit(NThreadState state)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            if (ApplicationManager.Instance.IsExit)
            {
                med.Info("Thread '{0}' is exit due to application is exit."
                    .args(this.Name));
                return true;
            }
            if (null == state)
            {
                return false;
            }
            else
            {
                if (state.IsCompleted)
                {
                    med.Info("Thread '{0}' is completed."
                        .args(this.Name));

                    return true; // completed.
                }

                if (state.IsCanceled)
                {
                    med.Info("Thread '{0}' is cancel by user."
                        .args(this.Name));

                    return true; // cancel by user.
                }

                if (state.Timeout < 0)
                {
                    return false; // No timeout
                }
                else
                {
                    // Has timeout.
                    TimeSpan ts = DateTime.Now - state.StartTime;
                    if (ts.TotalMilliseconds >= state.Timeout)
                    {
                        // Timeout is met
                        med.Info("Thread '{0}' is detected timeout."
                            .args(this.Name));

                        state.IsTimeout = true; // set timeout flags
                        return true;
                    }
                    else
                    {
                        // still in time
                        return false;
                    }
                }
            }
        }

        #endregion

        #region Thread methods

        private void CreateThread(int timeout)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            if (_isRunning || null != _th)
            {
                // already has thread running
                med.Info("Thread '{0}' is already running. Kill exist thread"
                    .args(this.Name));
                KillThread();
            }

            if (null != _th)
            {
                // Thread is already running
                med.Info("Thread '{0}' is still running. It's seem to be a problem."
                    .args(this.Name));
                return;
            }

            lock (this)
            {
                _th = new Thread(this.Processing);
                _th.Name = _name;
                _th.Priority = ThreadPriority.BelowNormal;
                _th.IsBackground = true;

                _isRunning = true; // set flag
                // create thread state and set time
                _state = new NThreadState(timeout);
                _state.IsFinished = false;
                _state.IsCompleted = false;

                _state.StartTime = DateTime.Now;
                _state.FinishedTime = DateTime.Now;

                med.Info("Thread '{0}' is create and about to run."
                    .args(this.Name));

                _th.Start();
            }
        }

        private void Processing()
        {
            bool isExit = IsExit(_state);

            RaiseInitHandler(); // call Init handler

            while (null != _executeHandler &&
                null != _th && _isRunning && !isExit)
            {
                Invoke(_executeHandler, _state); // execute action code.

                // Update exit variable.
                isExit = IsExit(_state);

                if (!isExit)
                {
                    Thread.Sleep(25); // Give CPU a little break.
                    // Pump message
                    ApplicationManager.Instance.DoEvents();
                }
            }

            RaiseFinishedHandler();

            _isRunning = false; // make sure flag is reset
            _th = null; // set thread variable to null.

            NThread.Remove(this.Name); // unregister thread
        }

        private void KillThread()
        {
            if (!_isRunning && null == _th)
                return;

            _isRunning = false; // reset flag

            NThread.Remove(this.Name); // unregister thread

            MethodBase med = MethodBase.GetCurrentMethod();
            lock (this)
            {
                if (null != _th)
                {
                    try
                    {
                        med.Info("Thread '{0}' is abort."
                            .args(this.Name));

                        _th.Abort();
                    }
                    catch (ThreadAbortException) { Thread.ResetAbort(); }
                }
                _th = null;
            }
            // reset state
            _state = null;
        }

        #endregion

        #endregion

        #region Public Methods

        #region Start/Shutdown

        /// <summary>
        /// Start.
        /// </summary>
        /// <param name="timeout">The timeout in millisecond. -1 for no timeout.</param>
        /// <returns>Returns current instance.</returns>
        public NThread Start(int timeout = -1)
        {
            CreateThread(timeout);
            return this;
        }
        /// <summary>
        /// Shutdown.
        /// </summary>
        public void Shutdown()
        {
            KillThread();
        }

        #endregion

        #region Handler assigns

        /// <summary>
        /// Assign initialization code.
        /// </summary>
        /// <param name="action">The initialization action code.</param>
        /// <returns>Returns current instance.</returns>
        public NThread OnInit(NThreadExecuteHandler action)
        {
            if (!_isRunning)
            {
                lock (this)
                {
                    _initHandler = action;
                }
            }
            return this;
        }
        /// <summary>
        /// Assign execute code.
        /// </summary>
        /// <param name="action">The excute action code.</param>
        /// <returns>Returns current instance.</returns>
        public NThread Execute(NThreadExecuteHandler action)
        {
            if (!_isRunning)
            {
                lock (this)
                {
                    _executeHandler = action;
                }
            }
            return this;
        }
        /// <summary>
        /// Assign finished code.
        /// </summary>
        /// <param name="action">The finished action code.</param>
        /// <returns>Returns current instance.</returns>
        public NThread OnFinished(NThreadExecuteHandler action)
        {
            if (!_isRunning)
            {
                lock (this)
                {
                    _finihseHandler = action;
                }
            }

            return this;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the thread name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        /// <summary>
        /// Checks is current thread is running.
        /// </summary>
        public bool IsRunning { get { return (_isRunning || (null != _th)); } }
        /// <summary>
        /// Gets thread state.
        /// </summary>
        public NThreadState State
        {
            get { return _state; }
        }

        #endregion

        #region Static Methods

        // Loca thread cache variables
        private static Dictionary<string, NThread> _threads =
            new Dictionary<string, NThread>();

        /// <summary>
        /// Gets thrad count.
        /// </summary>
        public static int ThreadCount { get { return _threads.Count; } }
        /// <summary>
        /// Remove thread by name.
        /// </summary>
        /// <param name="name">The thread name.</param>
        private static void Remove(string name)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            if (string.IsNullOrWhiteSpace(name))
            {
                // Not allow thead without name.
                med.Info("Not allow thead without name.");
                return;
            }
            NThread th = null;
            string key = name.Trim().ToLower();
            if (_threads.ContainsKey(key))
            {
                lock (typeof(NThread))
                {
                    med.Info("Thread '{0}' is remove from list."
                        .args(name));
                    try
                    {
                        th = _threads[key];
                        _threads.Remove(key);
                    }
                    catch (Exception)
                    {
                        th = null;
                    }
                    if (null != th)
                    {
                        th.Shutdown(); // auto kill thread
                    }
                    th = null;
                }
            }
        }
        /// <summary>
        /// Create new Thread.
        /// </summary>
        /// <param name="name">The thread name.</param>
        /// <returns>Returns instance of new thread.</returns>
        public static NThread Create(string name)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            if (string.IsNullOrWhiteSpace(name))
            {
                med.Info("Not allow thead without name.");
                return null;
            }
            NThread th = null;
            string key = name.Trim().ToLower();
            if (_threads.ContainsKey(key))
            {
                lock (typeof(NThread))
                {
                    th = _threads[key];

                    med.Info("Thread '{0}' already exists in list."
                        .args(name));
                }
            }
            else
            {
                lock (typeof(NThread))
                {
                    th = new NThread(name);
                    _threads.Add(key, th);

                    med.Info("Thread '{0}' created and add to list."
                        .args(name));
                }
            }
            return th;
        }
        /// <summary>
        /// Shutdown all threads.
        /// </summary>
        public static void FreeThreads()
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            lock (typeof(NThread))
            {
                med.Info("Free all threads.");

                NThread[] threads = _threads.Values.ToArray();
                if (null == threads || threads.Length <= 0)
                    return;

                _threads.Clear(); // clear list

                foreach (NThread th in threads)
                {
                    if (null != th)
                    {
                        th.Shutdown();
                    }
                }
            }
        }

        #endregion
    }

    #endregion
}
