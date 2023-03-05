#define ENABLE_LOG_MANAGER
#define ENABLE_CPM

#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-08-06
=================
- NLib Application Controller.
  - WpfAppContoller is enable supports log.

======================================================================================================================
Update 2013-08-05
=================
- NLib Application Controller.
  - WpfAppContoller is split to new file.
  - remove ununsed methods.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows;

#endregion

#region WPF Controller

namespace NLib
{
    #region WpfAppContoller

    /// <summary>
    /// WPF Application Contoller.
    /// </summary>
    public class WpfAppContoller : DispatcherObject, IApplicationController, IDisposable
    {
        #region Singelton Access

        private static WpfAppContoller _instance = null;
        /// <summary>
        /// Singelton Access instance of Window application controller.
        /// </summary>
        public static WpfAppContoller Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(WpfAppContoller))
                    {
                        _instance = new WpfAppContoller();
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

        private Window _mainWindow = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private WpfAppContoller()
            : base()
        {
            // lock when construct
            if (null != Application.Current)
            {
                Application.Current.Exit += new ExitEventHandler(Current_Exit);
                Application.Current.SessionEnding += new SessionEndingCancelEventHandler(Current_SessionEnding);
            }
            // self register
            ApplicationManager.Instance.Init(this);
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~WpfAppContoller()
        {
            Dispose();
            Shutdown();
            if (null != Application.Current)
            {
                Application.Current.SessionEnding -= new SessionEndingCancelEventHandler(Current_SessionEnding);
                Application.Current.Exit -= new ExitEventHandler(Current_Exit);
            }
        }

        #endregion

        #region Private Methods

        #region Current Exit and Current SessionEnding Events

        void Current_Exit(object sender, ExitEventArgs e)
        {
            // this event raise before application exit
            Exit();
        }

        void Current_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            // this event raise when window is logoff or shutdown
            Exit();
        }

        #endregion

        #endregion

        #region Overrides

        #region Dispose

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            if (null != _mainWindow)
            {
                _mainWindow.Closing -= new CancelEventHandler(_mainWindow_Closing);
            }
            _mainWindow = null;
        }

        #endregion

        #region OnWindowClosing

        /// <summary>
        /// Window Closing Handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The cancel event args.</param>
        void _mainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_exitOnClose)
            {
                if (!e.Cancel)
                {
                    Dispose();
                }
            }
        }

        #endregion

        #endregion

        #region Run

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="window">The window instance</param>
        public void Run(Window window)
        {
            if (null != window && !_isAppRun)
            {
                _isAppRun = true;
                _isExit = false;

                if (Application.Current.MainWindow != window)
                {
                    Application.Current.MainWindow = window;
                }

                if (null != _mainWindow)
                {
                    _mainWindow.Closing -= new CancelEventHandler(_mainWindow_Closing);
                }
                // keep reference.
                _mainWindow = Application.Current.MainWindow;
                if (null != _mainWindow)
                {
                    _mainWindow.Closing += new CancelEventHandler(_mainWindow_Closing);
                }

                if (null != _mainWindow)
                {
                    _mainWindow.Show();
                }
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
            _isExit = true;

            if (null != _mainWindow)
            {
                _mainWindow.Close(); // close main window.
            }

            _isAppRun = false; // reset flag
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
            // required to check if used singleton
            if (null != ApplicationManager.Instance.Environments &&
                null != ApplicationManager.Instance.Environments.Options &&
                null != ApplicationManager.Instance.Environments.Options.Behaviors &&
                ApplicationManager.Instance.Environments.Options.Behaviors.IsSingleAppInstance &&
                exitCode == 0)
            {
#if ENABLE_CPM
                if (autokill)
                {
                    // normally shutdown
                    NLib.Utils.CPM.Instance.Shutdown(autokill, autoKillInMs);
                }
#endif
            }

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
