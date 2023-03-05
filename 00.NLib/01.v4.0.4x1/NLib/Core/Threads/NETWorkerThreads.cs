#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2010-03-22
=================
- Fiexd bug in NETWorkerThread class.
  - Changed code order in Start method that cause processing loop immediately exit.

======================================================================================================================
Update 2010-02-06
=================
- Fiexd bug in NETWorkerThread class.
  - Fixed access null object acsess.
 
======================================================================================================================
Update 2010-02-02
=================
- New Thread Framework added.
  - NETWorkerThread class added. Used this instead of AbstractWorkerThread.
  - NETWorkerMonitor class added. Used to monitoring the worker threads.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Threading;

#endregion

namespace NLib.Threads
{
    #region NETWorkerMonitor

    /// <summary>
    /// NETWorkerMonitor class. Provide basic worker threads monitor.
    /// </summary>
    public sealed class NETWorkerMonitor : IDisposable
    {
        #region Singelton

        private static NETWorkerMonitor _instance;

        /// <summary>
        /// Singelton Access instance.
        /// </summary>
        public static NETWorkerMonitor Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(NETWorkerMonitor))
                    {
                        _instance = new NETWorkerMonitor();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Internal Variables

        private List<NETWorkerThread> _workers = new List<NETWorkerThread>();

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        private NETWorkerMonitor() : base() { }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NETWorkerMonitor()
        {
            Dispose();
        }

        #endregion

        #region Public Methods

        #region Dispose

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            if (null != _workers)
            {
                #region Copy Threads

                NETWorkerThread[] threads = null;
                if (_workers.Count > 0)
                {
                    threads = new NETWorkerThread[_workers.Count];
                    lock (this)
                    {
                        _workers.CopyTo(threads, 0);
                    }
                }
                lock (this)
                {
                    _workers.Clear();
                }

                #endregion

                #region Dispose all threads

                if (null != threads)
                {
                    foreach (NETWorkerThread worker in threads)
                    {
                        worker.Dispose();
                    }
                }
                threads = null;

                #endregion
            }
        }

        #endregion

        #region Register/Unregister

        /// <summary>
        /// Register worker thread to monitor list.
        /// </summary>
        /// <param name="worker">The worker thread instance.</param>
        public void Register(NETWorkerThread worker)
        {
            if (null == worker)
                return;
            if (_workers.Contains(worker))
                return; // already registered
            // add to monitor list
            lock (this)
            {
                _workers.Add(worker);
            }
        }
        /// <summary>
        /// Unregister worker thread from monitor list.
        /// </summary>
        /// <param name="worker">The worker thread instance.</param>
        public void Unregister(NETWorkerThread worker)
        {
            if (null == worker)
                return;
            if (_workers.Contains(worker))
            {
                // remove from monitor list
                lock (this)
                {
                    _workers.Remove(worker);
                }
            }
        }

        #endregion

        #region Count/GetWorkerThreads

        /// <summary>
        /// Gets number of worker threads in monitor list.
        /// </summary>
        public int Count
        {
            get
            {
                if (null == _workers)
                    return 0;
                else return _workers.Count;
            }
        }
        /// <summary>
        /// Gets list of worker thread on m0nitor.
        /// </summary>
        /// <returns>Returns array of worker threads.</returns>
        public NETWorkerThread[] GetWorkerThreads()
        {
            NETWorkerThread[] results = null;
            if (null == _workers)
                return results;

            results = new NETWorkerThread[_workers.Count];
            lock (this)
            {
                _workers.CopyTo(results, 0);
            }

            return results;
        }

        #endregion

        #endregion
    }

    #endregion

    #region NETWorkerThread

    /// <summary>
    /// NETWorkerThread class.
    /// This class is .NET ThreadPool's Worker Thread that provide the 
    /// basic functional for worker thread.
    /// </summary>
    public abstract class NETWorkerThread : IDisposable
    {
        #region Internal Variable

        private System.Threading.Thread _threadObj;
        private bool _processing;
        private ThreadPriority _piority = ThreadPriority.Normal;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NETWorkerThread()
            : base()
        {
            Init();
            NETWorkerMonitor.Instance.Register(this);
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NETWorkerThread()
        {
            Kill();
            NETWorkerMonitor.Instance.Unregister(this);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Kill();
        }

        #endregion

        #region Private Method and Property

        #region Init

        /// <summary>
        /// Init Variables
        /// </summary>
        private void Init()
        {
            _processing = false;
            _threadObj = null;
            _threadObj = new System.Threading.Thread(this.Run);
            _threadObj.Name = ".NET worker thread";
            _threadObj.IsBackground = true;
        }

        #endregion

        #region IsProcessing Property

        /// <summary>
        /// Get/Set is in processing state.
        /// </summary>
        protected bool IsProcessing
        {
            get
            {
                return _processing;
            }
            set
            {
                _processing = value;
            }
        }

        #endregion

        #region Kill

        /// <summary>
        /// Kill Thread. Join and release mutex and wait-handle
        /// </summary>
        protected void Kill()
        {
            if (!IsAlive)
            {
                return;
            }

            // set as not processing
            IsProcessing = false;

            if (_threadObj != null)
            {
                try
                {
                    _threadObj.Abort();
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort(); // Reset Abort
                }
            }
            _threadObj = null;
        }

        #endregion

        #region Run

        /// <summary>
        /// Run. Core Code
        /// </summary>
        private void Run()
        {
            try
            {
                Processing();
            }
            catch (ObjectDisposedException)
            {
                // Object is already disposed
            }
            finally
            {
            }
        }

        #endregion

        #endregion

        #region Override

        /// <summary>
        /// Get Hash Code
        /// </summary>
        /// <returns>hash code value for object.</returns>
        public override int GetHashCode()
        {
            if (null == _threadObj)
                return base.GetHashCode();
            return _threadObj.GetHashCode();
        }
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj">object to check.</param>
        /// <returns>true if object is equal.</returns>
        public override bool Equals(object obj)
        {
            if (null == _threadObj)
                return false;
            return _threadObj.Equals(obj);
        }

        #endregion

        #region Protected Method

        /// <summary>
        /// Gets worker thread's name.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Occur when Start method is called
        /// </summary>
        protected virtual void OnStarting() { }
        /// <summary>
        /// Occur when Stop method is called
        /// </summary>
        protected virtual void OnStopping() { }
        /// <summary>
        /// Processing
        /// </summary>
        protected abstract void Processing();

        #endregion

        #region Public Method

        /// <summary>
        /// Start
        /// </summary>
        public void Start()
        {
            //System.Diagnostics.Debug.Assert(_threadObj != null);
            //System.Diagnostics.Debug.Assert(!_threadObj.IsAlive);
            if (IsStart)
                return; // Already start

            if (null != _threadObj &&
                _threadObj.ThreadState != System.Threading.ThreadState.Unstarted)
            {
                Stop();
                _threadObj = null;
            }
            if (_threadObj == null)
            {
                Init();
            }

            OnStarting(); // call virtual method

            IsProcessing = true; // mark as processing before execute thread
            _threadObj.Start();
        }
        /// <summary>
        /// Stop
        /// </summary>
        public void Stop()
        {
            OnStopping(); // call virtual method

            if (_threadObj == null)
                return; // Thread object is null

            if (!IsAlive)
            {
                return;
            }

            IsProcessing = false; // mark as not processing
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Thread Piority
        /// </summary>
        protected ThreadPriority Piority
        {
            get { return _piority; }
            set
            {
                if (_piority == value)
                    return;
                if (_threadObj == null)
                {
                    _piority = value;
                }
            }
        }
        /// <summary>
        /// Is Thread Start
        /// </summary>
        public bool IsStart
        {
            get
            {
                if (_threadObj == null)
                    return false;
                if (_threadObj.ThreadState == System.Threading.ThreadState.Unstarted)
                    return false;
                else return IsProcessing;
            }
        }
        /// <summary>
        /// Get/Set Thread Name
        /// </summary>
        public string ThreadName
        {
            get
            {
                if (null == _threadObj)
                    return string.Empty;
                return _threadObj.Name;
            }
            set
            {
                if (null == _threadObj)
                    return;
                _threadObj.Name = value;
            }
        }
        /// <summary>
        /// Check is Thread is alive.
        /// </summary>
        public bool IsAlive
        {
            get
            {
                //System.Diagnostics.Debug.Assert(_threadObj != null);

                if (_threadObj == null)
                    return false; // Thread object is null

                if (_threadObj.ThreadState == System.Threading.ThreadState.Unstarted)
                    return false; // Thread not start

                if (_threadObj.ThreadState == System.Threading.ThreadState.Aborted)
                    return false; // Thread is abort

                if (_threadObj.ThreadState == System.Threading.ThreadState.AbortRequested)
                    return false; // Thread is about to Abort

                if (_threadObj.ThreadState == System.Threading.ThreadState.Stopped)
                    return false; // Thread is stoped

                if (_threadObj.ThreadState == System.Threading.ThreadState.StopRequested)
                    return false; // Thread is about to Stop

                if (null == _threadObj)
                    return false;
                return _threadObj.IsAlive;
            }
        }
        /// <summary>
        /// Get Manage Thread ID
        /// </summary>
        public int ManageThreadId
        {
            get
            {
                if (null == _threadObj)
                    return -1;
                return _threadObj.ManagedThreadId;
            }
        }
        /// <summary>
        /// Get Thread instance
        /// </summary>
        public System.Threading.Thread Thread { get { return _threadObj; } }

        #endregion
    }

    #endregion
}
