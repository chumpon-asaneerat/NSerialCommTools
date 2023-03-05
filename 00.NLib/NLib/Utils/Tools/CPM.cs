#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-08-06
=================
- CPM updated.
  - change target output path to company's temp path.
  - remove private property AppName.

======================================================================================================================
Update 2013-01-16
=================
- CPM updated.
  - change namespace from NLib.Resources to NLib.Resource.

======================================================================================================================
Update 2012-12-25
=================
- CPM updated.
  - Update code to used new NFolder in environment class.

======================================================================================================================
Update 2012-01-02
=================
- CPM first version.
  - Support auto kill process function.
  - Support new Resource Access Model.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;

using NLib.Resource;

using System.IO;
using System.Threading;
using System.Diagnostics;

#endregion

namespace NLib.Utils
{
    #region CPM

    /// <summary>
    /// Application Close Process monitor.
    /// </summary>
    public class CPM
    {
        #region Singelton

        private static CPM _instance = null;
        /// <summary>
        /// Singelton Access instance.
        /// </summary>
        public static CPM Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(CPM))
                    {
                        _instance = new CPM();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Internal Variables

        private uint __AUTOKILL = 0; // wait n milisecond before run process minitor. 
        private Thread _thClose = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private CPM() : base() { }

        #endregion

        #region Public Methods

        private void RunProcessMon()
        {
            if (__AUTOKILL <= 100)
                return;
            DateTime dt = DateTime.Now;
            TimeSpan ts = DateTime.Now - dt;
            while (ts.TotalMilliseconds < __AUTOKILL)
            {
                ts = DateTime.Now - dt;
                ApplicationManager.Instance.DoEvents();
                Thread.Sleep(50);
            }
            // build args
            Process proc = Process.GetCurrentProcess();
            string arg = (null != proc) ? proc.ProcessName + " " : string.Empty;

            using (WindowFormsResourceAccess resAccess = new WindowFormsResourceAccess())
            {
                ResourceExecuteOptions option = new ResourceExecuteOptions()
                {
                    ResourceName = CPMConsts.CloseProcessMonitor,
                    AutoCreate = true,
                    // Note that the CPM is the program to kill current application so it's cannot be deleted
                    // no matter AutoDelete is set to true or false.
                    AutoDelete = true,
                    ShowWindow = true,
                    CallerType = typeof(CPMConsts),
                    TargetPath = ApplicationManager.Instance.Environments.Company.Temp.FullName,
                    TargetFileName = @"CPM.exe",
                    Argument = arg.Trim()
                };

                resAccess.Execute(option);
            }

            KillProcessMonThread();
        }

        private void KillProcessMonThread()
        {
            if (null != _thClose)
            {
                try
                {
                    _thClose.Abort();
                }
                catch { Thread.ResetAbort(); }
            }
            _thClose = null;
        }

        private void StartProcessMonThread()
        {
            KillProcessMonThread();
            if (__AUTOKILL <= 100)
                return;
            _thClose = new Thread(new ThreadStart(RunProcessMon));
            _thClose.Name = "Close Application Process monitor thread";
            _thClose.Priority = ThreadPriority.Lowest;
            _thClose.IsBackground = false; // alway be foreground thread.
            _thClose.Start();
        }

        /// <summary>
        /// Shutdown
        /// </summary>
        /// <param name="autokill">True for autokill</param>
        /// <param name="autoKillInMs">The time to force process auto 
        /// kill in millisecond. if this parameter is less than 100 ms. 
        /// so no auto kill process running.</param>
        public void Shutdown(bool autokill, uint autoKillInMs)
        {
            if (autokill)
            {
                if (__AUTOKILL != autoKillInMs)
                {
                    __AUTOKILL = autoKillInMs;
                }

                // start process monitor thread.
                StartProcessMonThread();
            }
        }

        #endregion
    }

    #endregion
}
