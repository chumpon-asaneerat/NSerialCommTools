#define ENABLE_LOG_MANAGER
#define ENABLE_CPM

#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-08-06
=================
- NLib Application Controller.
  - ConsoleController is enable supports log.

======================================================================================================================
Update 2013-08-05
=================
- NLib Application Controller.
  - ConsoleController is split to new file.
  - remove ununsed methods.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.ComponentModel;

#endregion

#region Console Controller

namespace NLib
{
    #region ConsoleController

    /// <summary>
    /// Console Controller.
    /// </summary>
    public class ConsoleController : IApplicationController
    {
        #region Singelton Access

        private static ConsoleController _instance = null;
        /// <summary>
        /// Singelton Access instance of Window application controller.
        /// </summary>
        public static ConsoleController Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(ConsoleController))
                    {
                        _instance = new ConsoleController();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Internal Variables

        private bool _isAppRun = false;
        private bool _isExit = false;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private ConsoleController()
            : base()
        {
            // lock when construct
            System.Windows.Forms.Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            System.Windows.Forms.Application.ThreadExit += new EventHandler(Application_ThreadExit);
            // self register
            ApplicationManager.Instance.Init(this);
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~ConsoleController()
        {
            Shutdown();

            System.Windows.Forms.Application.ThreadExit -= new EventHandler(Application_ThreadExit);
            System.Windows.Forms.Application.ApplicationExit -= new EventHandler(Application_ApplicationExit);
        }

        #endregion

        #region Private Methods

        #region ThreadExit and AppExit Events

        void Application_ThreadExit(object sender, EventArgs e)
        {
            // this event raise before application exit
            _isAppRun = true;
        }

        void Application_ApplicationExit(object sender, EventArgs e)
        {
            _isAppRun = true;
        }

        #endregion

        #endregion

        #region Setup

        /// <summary>
        /// Setup application environment's options.
        /// </summary>
        /// <param name="option">The environment's options.</param>
        public void Setup(EnvironmentOptions option)
        {
            ApplicationManager.Instance.Environments.Setup(option);
            if (null != ApplicationManager.Instance.Environments &&
                null != ApplicationManager.Instance.Environments.Product &&
                null != ApplicationManager.Instance.Environments.Product.Logs)
            {
#if ENABLE_LOG_MANAGER
                Logs.LogManager.Instance.Start();
#endif
                _isAppRun = true;
                _isExit = false;
            }
        }

        #endregion

        #region Shutdown(s)

        /// <summary>
        /// Shutdown. No auto kill process.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        public void Shutdown(int exitCode = 0)
        {
            Shutdown(false, 0, exitCode);
        }
        /// <summary>
        /// Shutdown with auto kill process.
        /// </summary>
        /// <param name="autokill">True for autokill</param>
        /// <param name="exitCode">The exit code.</param>
        public void Shutdown(bool autokill, int exitCode = 0)
        {
            Shutdown(autokill, 1000, exitCode);
        }
        /// <summary>
        /// Shutdown
        /// </summary>
        /// <param name="autokill">True for autokill</param>
        /// <param name="autoKillInMs">The time to force process auto 
        /// kill in millisecond. if this parameter is less than 100 ms. 
        /// so no auto kill process running.</param>
        /// <param name="exitCode">The exit code.</param>
        public void Shutdown(bool autokill, uint autoKillInMs, int exitCode = 0)
        {
            if (_isExit)
                return; // already exit.

            _isExit = true;
            _isAppRun = false;

            // Shutdown related services
#if ENABLE_LOG_MANAGER
            Logs.LogManager.Instance.Shutdown();
#endif

#if ENABLE_CPM
            if (autokill)
            {
                NLib.Utils.CPM.Instance.Shutdown(autokill, autoKillInMs);
            }
#endif
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Checks is application is running.
        /// </summary>
        public bool IsRunning
        {
            get { return _isAppRun; }
        }
        /// <summary>
        /// Check is application is exit.
        /// </summary>
        [Category("Application")]
        [Browsable(false)]
        public bool IsExit
        {
            get { return _isExit; }
        }
        /// <summary>
        /// Checks is application has more instance than one.
        /// </summary>
        [Category("Services")]
        [Browsable(false)]
        public bool HasMoreInstance
        {
            get
            {
                return ApplicationManager.Instance.Environments.HasMoreInstance;
            }
        }

        #endregion
    }

    #endregion
}

#endregion
