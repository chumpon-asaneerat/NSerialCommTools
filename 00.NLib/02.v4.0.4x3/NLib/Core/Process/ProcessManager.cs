#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2016-06-17
=================
- Process Manager updated.
  - Used new CPM (new version) that fixed DPI-Aware.

======================================================================================================================
Update 2012-12-19
=================
- Process Manager updated.
  - CPM and NLib403 is now seperated code.

======================================================================================================================
Update 2012-01-02
=================
- Process Manager (share code) updated.
  - Support used new ApplicationManager model and Standalone exe.

======================================================================================================================
Update 2010-01-01
=================
- Process Manager (share code) updated.
  - Process Manager class changed
    - The namespace for Process Manager class is moved from namespace SysLib.Forms to 
      SysLib.Threads.
    - Update internal class ProcessDelegateInvoker. Add more checking code parameters for 
      invoke delegate.
    - Add new method KillConsoleIME to kill conime.exe service that will automatic spawn
      when console window in windows that supports asian language is executed.

======================================================================================================================
Update 2009-12-26
=================
- Process Manager (share code) ported from GFA37 to GFA38.
  - Process Manager class is ported and move to Share->Source Folder.

======================================================================================================================
Update 2008-09-12
=================
- Process Manager (share code).
  - Add new kill method by used WMI.
  - Kill method now support retry and return status (true/false).
  - Add new event handler for Progress update and Exception.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;

#endregion

namespace NLib
{
    #region Process Progress EventHandler and EventArgs

    /// <summary>
    /// Process Progress EventHandler
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The Event Args.</param>
    public delegate void ProcessProgressEventHandler(object sender, ProcessProgressEventArgs e);

    /// <summary>
    /// Process Progress EventArgs
    /// </summary>
    public class ProcessProgressEventArgs
    {
        #region Internal Variable

        private string _processName = string.Empty;
        private string _methodName = string.Empty;
        private int _iRetryIndex = 0;
        private int _iRetryCount = 0;
        private int _iProcessIndex = 0;
        private int _iProcessCount = 0;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        private ProcessProgressEventArgs() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="processName">The Pprocess Name</param>
        /// <param name="methodName">The Execute Method Name.</param>
        /// <param name="processIndex">The process index in list that has same process name.</param>
        /// <param name="processCount">The process count that has same process name.</param>
        /// <param name="retryTime">The retry time.</param>
        /// <param name="retryMax">The retry max.</param>
        public ProcessProgressEventArgs(string processName, string methodName,
            int processIndex, int processCount,
            int retryTime, int retryMax)
            : this()
        {
            _processName = processName;
            _methodName = methodName;
            _iProcessIndex = processIndex;
            _iProcessCount = processCount;
            _iRetryIndex = retryTime;
            _iRetryCount = retryMax;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~ProcessProgressEventArgs()
        {
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get the process name on operation.
        /// </summary>
        public string ProcessName { get { return _processName; } }
        /// <summary>
        /// Get Operation Method Name.
        /// </summary>
        public string MethodName { get { return _methodName; } }
        /// <summary>
        /// Get current process index.
        /// </summary>
        public int ProcessIndex { get { return _iProcessIndex; } }
        /// <summary>
        /// Get Number of process that has same process name.
        /// </summary>
        public int NumberOfProcess { get { return _iProcessCount; } }
        /// <summary>
        /// Get the retry time.
        /// </summary>
        public int Retry { get { return _iProcessIndex; } }
        /// <summary>
        /// Get the maximum retry time.
        /// </summary>
        public int MaxRetry { get { return _iProcessCount; } }

        #endregion
    }

    #endregion

    #region Process Operation Exception EventHandler and EventArgs

    /// <summary>
    /// Process Operation Exception EventHandler
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The Event Args.</param>
    public delegate void ProcessOperationExceptionEventHandler(object sender, ProcessOperationExceptionEventArgs e);

    /// <summary>
    /// Process Operation Exception EventArgs
    /// </summary>
    public class ProcessOperationExceptionEventArgs
    {
        #region Internal Variable

        private string _processName = string.Empty;
        private string _methodName = string.Empty;
        private Exception _ex = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        private ProcessOperationExceptionEventArgs() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="processName">The Pprocess Name</param>
        /// <param name="methodName">The Execute Method Name.</param>
        /// <param name="ex">The Exception instance.</param>
        public ProcessOperationExceptionEventArgs(string processName, string methodName,
            Exception ex)
            : this()
        {
            _processName = processName;
            _methodName = methodName;
            _ex = ex;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~ProcessOperationExceptionEventArgs()
        {
            _ex = null;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get the process name on operation.
        /// </summary>
        public string ProcessName { get { return _processName; } }
        /// <summary>
        /// Get Operation Method Name.
        /// </summary>
        public string MethodName { get { return _methodName; } }
        /// <summary>
        /// Get Exception Instance.
        /// </summary>
        public Exception Exception { get { return _ex; } }

        #endregion
    }

    #endregion

    #region Process Manager

    /// <summary>
    /// Process Manager
    /// </summary>
    public class ProcessManager
    {
        #region Singelton

        private static ProcessManager _instance = null;
        /// <summary>
        /// Get Application Process Manager Instance
        /// </summary>
        public static ProcessManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(ProcessManager))
                    {
                        _instance = new ProcessManager();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private ProcessManager() : base() { }

        #endregion

        #region Get All Process

        /// <summary>
        /// Get All Process
        /// </summary>
        /// <returns>all process avaliable at current time</returns>
        public Process[] GetProcesses()
        {
            Process[] processes = Process.GetProcesses();

            if (processes == null || processes.Length <= 0)
                return null;
            else return processes;
        }

        #endregion

        #region GetProcessNames

        /// <summary>
        /// GetProcessNames
        /// </summary>
        /// <returns>all process's name avaliable at current time</returns>
        public string[] GetProcessNames()
        {
            Process[] processes = GetProcesses();
            if (processes == null || processes.Length <= 0)
                return null;

            string[] results = new string[processes.Length];

            int i = 0;
            foreach (Process process in processes)
            {
                results[i] = process.ProcessName;
                i++;
            }

            return results;
        }

        #endregion

        #region Is Process Exists

        /// <summary>
        /// Is Process Exists
        /// </summary>
        /// <param name="processName">Target Process's Name</param>
        /// <returns>all process that match process's name</returns>
        public Process[] GetProcessByName(string processName)
        {
            if (processName.Trim() == string.Empty)
                return null;

            Process[] processes = Process.GetProcesses();

            if (processes == null || processes.Length <= 0)
                return null;
            else
            {
                Process[] results = null;
                ArrayList list = new ArrayList();
                foreach (Process process in processes)
                {
                    if (string.Compare(process.ProcessName, processName, true) == 0)
                    {
                        list.Add(process);
                    }
                }
                results = (Process[])list.ToArray(typeof(Process));
                list.Clear();
                list = null;
                return results;
            }
        }

        #endregion

        #region Raise events

        private void RaiseKillEvent(string processName, string methodName,
            int currentIndex, int maxCount, int retry)
        {
            if (OnKill != null)
            {
                ProcessProgressEventArgs evt = new ProcessProgressEventArgs(processName,
                    methodName, currentIndex, maxCount, retry, MaxRetry);

                OnKill.Call(this, evt);
            }
        }

        private void RaiseExceptionEvent(string processName, string methodName,
            Exception ex)
        {
            if (OnException != null)
            {
                ProcessOperationExceptionEventArgs evt = new ProcessOperationExceptionEventArgs(
                    processName, methodName, ex);

                OnException.Call(this, evt);
            }
        }

        #endregion

        private int MaxRetry = 50;

        #region Kill Process

        /// <summary>
        /// Kill Process
        /// </summary>
        /// <param name="processName">Target Process's Name</param>
        /// <returns>Return true if process is killed.</returns>
        public bool Kill(string processName)
        {
            bool isKill = false;

            int iRetryCount = 0;

            while (!isKill && iRetryCount < MaxRetry)
            {
                Process[] processes = GetProcessByName(processName);
                // No process match
                if (processes == null || processes.Length <= 0)
                {
                    //MessageBox.Show("No process found. " + processName);
                    isKill = true;
                }
                if (isKill)
                    break; // already kill.

                int iMax = processes.Length;
                for (int i = 0; i < iMax; i++)
                {
                    Process process = processes[i];

                    IntPtr ptr = process.MainWindowHandle;
                    bool isClosed = false;
                    if (ptr != IntPtr.Zero)
                    {
                        isClosed = process.CloseMainWindow();
                    }

                    if (!isClosed)
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch (Win32Exception winEx)
                        {
                            RaiseExceptionEvent(processName, "Kill", winEx);
                        }
                        catch (InvalidOperationException invEx)
                        {
                            RaiseExceptionEvent(processName, "Kill", invEx);
                        }
                        catch (NotSupportedException notSupportEx)
                        {
                            RaiseExceptionEvent(processName, "Kill", notSupportEx);
                        }
                        catch (Exception ex)
                        {
                            RaiseExceptionEvent(processName, "Kill", ex);
                        }
                    }

                    // raise event.
                    RaiseKillEvent(processName, "Kill", i, iMax, iRetryCount);
                }

                // increase counter.
                iRetryCount++;

                // Sleep 150 ms.
                System.Threading.Thread.Sleep(150);
                this.DoEvents();
            }

            if (!isKill)
            {
                isKill = KillWithWMI(processName);
            }

            return isKill;
        }
        /// <summary>
        /// Kill Console IME process.
        /// </summary>
        public void KillConsoleIME()
        {
            Kill("conime");
        }

        #endregion

        #region Kill Process (WMI)

        /// <summary>
        /// Kill Process With WMI
        /// </summary>
        /// <param name="processName">Target Process's Name</param>
        /// <returns>Return true if process is killed.</returns>
        public bool KillWithWMI(string processName)
        {
            bool isKill = false;

            int iRetryCount = 0;

            while (!isKill && iRetryCount < MaxRetry)
            {
                ManagementObjectCollection theCollection = null;
                try
                {
                    ManagementScope theScope = new ManagementScope("\\\\.\\root\\cimv2"); // local
                    ObjectQuery theQuery = new ObjectQuery("SELECT * FROM Win32_Process WHERE Name='" + processName.Trim() + ".exe'");
                    ManagementObjectSearcher theSearcher = new ManagementObjectSearcher(theScope, theQuery);
                    theCollection = theSearcher.Get();
                }
                catch (Exception ex)
                {
                    RaiseExceptionEvent(processName, "Kill (WMI)", ex);
                }

                if (theCollection == null || theCollection.Count <= 0)
                {
                    //MessageBox.Show("No process found. " + processName);
                    isKill = true;
                }
                if (isKill)
                    break; // already kill.
                int i = 0;
                int iMax = theCollection.Count;
                foreach (ManagementObject theCurObject in theCollection)
                {
                    if (theCurObject != null)
                    {
                        try
                        {
                            theCurObject.InvokeMethod("Terminate", null);
                        }
                        catch (Exception ex)
                        {
                            RaiseExceptionEvent(processName, "Kill (WMI)", ex);
                        }
                        // raise event.
                        RaiseKillEvent(processName, "Kill (WMI)", i, iMax, iRetryCount);
                    }
                    // increase counter.
                    iRetryCount++;

                    i++; // increase loop count

                    // Sleep 150 ms.
                    System.Threading.Thread.Sleep(150);
                    this.DoEvents();
                }
            }

            return isKill;
        }

        #endregion

        #region Run

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="applicationName">Application's Name to start</param>
        /// <returns>true if process is started</returns>
        public bool Run(string applicationName)
        {
            return Run(applicationName, null);
        }
        /// <summary>
        /// Run
        /// </summary>
        /// <param name="applicationName">Application's Name to start</param>
        /// <param name="args">optional argument to send to application</param>
        /// <returns>true if process is started</returns>
        public bool Run(string applicationName, string[] args)
        {
            return Run(applicationName, args, ProcessWindowStyle.Normal);
        }
        /// <summary>
        /// Run
        /// </summary>
        /// <param name="applicationName">Application's Name to start</param>
        /// <param name="args">optional argument to send to application</param>
        /// <param name="style">Process window style</param>
        /// <returns>true if process is started</returns>
        public bool Run(string applicationName, string[] args, ProcessWindowStyle style)
        {
            if (applicationName.Trim().Length <= 0)
            {
                // No appliation name
                return false;
            }

            if (!System.IO.File.Exists(applicationName.Trim()))
            {
                // Application File Name not exits
                return false;
            }

            Process process = new Process();
            process.StartInfo = new ProcessStartInfo();
            process.StartInfo.FileName = applicationName;
            if (args != null && args.Length > 0)
            {
                process.StartInfo.Arguments = args[0];
            }
            process.StartInfo.WindowStyle = style;
            process.StartInfo.CreateNoWindow = false;

            try
            {
                if (process.Start())
                {
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {
                // Exception occur
                return false;
            }
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get Maximum retry loop.
        /// </summary>
        [Category("Process")]
        [Description("Get Maximum retry loop.")]
        public int MaximumRetryLoop
        {
            get { return MaxRetry; }
            set { MaxRetry = value; }
        }

        #endregion

        #region Public Event

        /// <summary>
        /// OnKill event. Occur when kill method is call.
        /// </summary>
        [Category("Process")]
        [Description("OnKill event. Occur when kill method is call.")]
        public event ProcessProgressEventHandler OnKill;
        /// <summary>
        /// OnException event. Occur when exception is occur.
        /// </summary>
        [Category("Process")]
        [Description("OnException event. Occur when exception is occur.")]
        public event ProcessOperationExceptionEventHandler OnException;

        #endregion
    }

    #endregion
}
