#define ENABLE_LOG_MANAGER
#define ENABLE_CPM

#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-08-06
=================
- NLib Application Controller.
  - WinAppContoller is enable supports log.

======================================================================================================================
Update 2013-08-05
=================
- NLib Application Controller.
  - WinAppContoller is split to new file.
  - remove ununsed methods.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.ComponentModel;
using System.Windows.Forms;

#endregion

#region Windows Form Controller

namespace NLib
{
    #region WinAppContoller

    /// <summary>
    /// Windows Application Contoller.
    /// </summary>
    public class WinAppContoller : ApplicationContext, IApplicationController
    {
        #region Singelton Access

        private static WinAppContoller _instance = null;
        /// <summary>
        /// Singelton Access instance of Window application controller.
        /// </summary>
        public static WinAppContoller Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(WinAppContoller))
                    {
                        _instance = new WinAppContoller();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Internal Variables

        private bool _isAppRun = false;
        private bool _isExit = false;

        private bool _exitOnClose = true;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private WinAppContoller()
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
        ~WinAppContoller()
        {
            Dispose(true);
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
            _isAppRun = true; // this mean app is still running but in exit process
        }

        void Application_ApplicationExit(object sender, EventArgs e)
        {
            _isAppRun = true; // this mean app is still running but in exit process
        }

        #endregion

        #endregion

        #region Overrides

        #region Dispose

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">Is disposing.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

        #region ExitThreadCore

        /// <summary>
        /// ExitThreadCore.
        /// </summary>
        protected override void ExitThreadCore()
        {
            _isExit = true;
            // Release resource.
            Dispose(true);
            _isAppRun = false; // reset flag
            base.ExitThreadCore();
        }

        #endregion

        #region OnMainFormClosed

        /// <summary>
        /// OnMainFormClosed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        protected override void OnMainFormClosed(object sender, EventArgs e)
        {
            if (_exitOnClose)
            {
                if (sender != null && sender is Form)
                {
                    Form form = (sender as Form);
                    Type formType = form.GetType();
                    if (null != formType)
                    {
                        //FormContextManager.Instance.Unregister(formType);
                    }
                }
                ExitThreadCore();
                base.OnMainFormClosed(sender, e);
            }
            else
            {
                // Not exit when close form
            }
        }

        #endregion

        #endregion

        #region Run

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="form">The form instance</param>
        public void Run(Form form)
        {
            if (null != form && !_isAppRun)
            {
                _isAppRun = true;
                _isExit = false;

                Application.EnableVisualStyles();
                Application.DoEvents();

                this.MainForm = form;
                this.MainForm.Show();

                Application.Run(this);
            }
            else
            {
                _isAppRun = false;
                _isExit = true;
            }
        }

        #endregion

        #region Exit

        /// <summary>
        /// Exit an application.
        /// </summary>
        public void Exit()
        {
            // call exit thread core to release related resource.
            this.ExitThreadCore();
        }

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
            _isAppRun = false;
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
