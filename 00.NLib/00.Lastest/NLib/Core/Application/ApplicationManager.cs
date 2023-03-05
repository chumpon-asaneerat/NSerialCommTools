#define USE_LOG_MANAGER
#define ENABLE_ENVIRONMENT

#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-12-07
=================
- NLib Application Manager update.
  - Add Preload method.

======================================================================================================================
Update 2013-08-06
=================
- NLib Application Manager update.
  - Enable supports log.

======================================================================================================================
Update 2013-07-06
=================
- NLib Application Manager redesign.
  - Redesign to decouple classes with Inversion of Control Containers, 
    Dependency Injection and Service Locator patterns.
  - Remove all delegate invoke methods. Used DelegateInvoker instead.
  - The IDelegateInvoker interface is removed. So the Controller should not need to
    implements.

======================================================================================================================
Update 2013-05-09
=================
- NLib Common Interfaces updated.
  - The ApplicationManager class changed.
    - Reduce shutdown methods to only one method.
    - Add local variable for keep exit state when no controller assigned.
- NLib Application Manager Extension Methods changed.
  - Remove Invoke method.

======================================================================================================================
Update 2013-01-01
=================
- NLib Common Interfaces updated.
  - The ApplicationManager class is split to new file ApplicationManager.cs.

======================================================================================================================
Update 2012-12-21
=================
- NLib Application Manager updated.
  - Add new Extenstion methods for Logs (Supports MethodBase parameter).
  - Add property IsLogEnable for enable or disable log.

======================================================================================================================
Update 2012-12-19
=================
- NLib Application Manager updated.
  - The interface IDelegateInvoker and IApplicationController is merged into
    ApplicationManager.cs file.
  - The Wait static method is changed to non static method.
- NLib Application Manager Extension Methods added.
  - Supports Sleep/Wait/DoEvents for multithread purpose.
  - Supports Invoke for event raiser.
  - Supports FreeGC for memory management.
  - Supports Logs methods.

======================================================================================================================
Update 2012-01-04
=================
- NLib Application Manager updated.
  - Add supports application single instance.
  - Add Share Manager.
  - Add exitCode optional parameter in Shutdown methods.

======================================================================================================================
Update 2011-12-15
=================
- NLib Application Manager. Redesign based on GFA40.
  - The Current version is still not supports all GFA40 functionals.
  - Add Static method Wait.
  - Redesign Application Controller function in IoC styles for used with multiple
    type of application like Windows Forms, WPF and Windows sercices.
  - Add new properties to work with application environments with on each company
    and each products will supports windows security to grant permission for users
    group to access the files and folders.
  - Supports custom Delegate Invoker depend on each type of applications in the
    Application's Controller.
  - Supports custom Application.DoEvents for Windows Forms and WPF.

======================================================================================================================
Update 2011-01-31
=================
- ApplicationManager class update.
  - Fixed Update Message Methods. The tag parameter is never used in last version.

======================================================================================================================
Update 2010-09-07
=================
- ApplicationManager class update.
  - Add Update Message Methods.
  - Add ApplicationMessageEventHandler and ApplicationMessageEventArgs.

======================================================================================================================
Update 2010-09-01
=================
- ApplicationManager class redesign.
  - Used code based on GFA37.
  - ShareManager and related classes added.
  - ArgsUtils class added.

======================================================================================================================
Update 2010-02-02
=================
- ApplicationManager class updated.
  - Update code to create interanl object like system tray and timer on same thread as
    MainForm thread.

======================================================================================================================
Update 2010-02-06
=================
- ApplicationManager class updated.
  - Add Start method.
  - Add Shutdown method.
  - Add IsVisible<T> method.
  - Update code to reise event by synchronized context to internal form
    if not assigned and used the MainForm if assigned.
- IApplicationService interface updated.
  - Add Init Method.
  - Add Release Method.

======================================================================================================================
Update 2010-02-03
=================
- Application Framework - Application Manager added.
  - Add new implementation of ApplicationManager class.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

using NLib.Reflection; // For delegate invoker

#endregion

namespace NLib
{
    #region ApplicationManager

    /// <summary>
    /// The Application Manager class. Provide common access variables and functions
    /// that used for control multithread accssing, related folders, etc.
    /// </summary>
    public class ApplicationManager
    {
        #region Singelton

        private static DateTime? _instanceDate = new DateTime?();
        private static Guid _instanceGUID = Guid.Empty;
        private static ApplicationManager _instance = null;
        /// <summary>
        /// Singelton Access instance of application manager.
        /// </summary>
        public static ApplicationManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(ApplicationManager))
                    {
                        _instance = new ApplicationManager();
                        _instanceDate = DateTime.Now;
                        _instanceGUID = Guid.NewGuid();
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// Gets Singelton Instance Create Date.
        /// </summary>
        public static DateTime? InstanceDate
        {
            get
            {
                return _instanceDate;
            }
        }
        /// <summary>
        /// Gets Singelton Instance Guid.
        /// </summary>
        public static Guid InstanceGuid
        {
            get
            {
                return _instanceGUID;
            }
        }

        #endregion

        #region Internal Variables

        private IApplicationController _controller = null;
#if ENABLE_ENVIRONMENT
        private Environments _env = null;
#endif
        private ShareManager _share = new ShareManager();

        private bool _isExit = false;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private ApplicationManager()
            : base()
        {
            AppDomain.CurrentDomain.DomainUnload += new EventHandler(CurrentDomain_DomainUnload);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~ApplicationManager()
        {
            Shutdown();
        }

        #endregion

        #region Private Methods

        #region Domain unload/Process exit

        void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            Shutdown();
        }

        void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Shutdown();
        }

        #endregion

        #endregion

        #region Public Methods

        #region Init

        /// <summary>
        /// Init the application controller.
        /// </summary>
        /// <param name="controller">The application controller.</param>
        public void Init(IApplicationController controller)
        {
            _controller = controller;
            if (null == _controller)
            {
                throw new ArgumentException("Application Controller instance is not assigned.", "controller");
            }
        }

        #endregion

        #region Shutdown(s)

        /// <summary>
        /// Shutdown and release current controller.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        public void Shutdown(int exitCode = 0)
        {
#if USE_LOG_MANAGER
            Logs.LogManager.Instance.Shutdown();
#endif
            if (null != _controller)
            {
                _controller.Shutdown(exitCode);
            }
            _controller = null;

            _isExit = true;
        }
        /// <summary>
        /// Shutdown with auto kill process and release current controller.
        /// </summary>
        /// <param name="autokill">True for autokill</param>
        /// <param name="exitCode">The exit code.</param>
        public void Shutdown(bool autokill, int exitCode = 0)
        {
#if USE_LOG_MANAGER
            Logs.LogManager.Instance.Shutdown();
#endif
            if (null != _controller)
            {
                _controller.Shutdown(autokill, exitCode);
            }
            _controller = null;

            _isExit = true;
        }
        /// <summary>
        /// Shutdown application manager and release current controller.
        /// </summary>
        /// <param name="autokill">True for autokill</param>
        /// <param name="autoKillInMs">The time to force process auto 
        /// kill in millisecond. if this parameter is less than 100 ms. 
        /// so no auto kill process running.</param>
        /// <param name="exitCode">The exit code.</param>
        public void Shutdown(bool autokill, uint autoKillInMs, int exitCode = 0)
        {
#if USE_LOG_MANAGER
            Logs.LogManager.Instance.Shutdown();
#endif
            if (null != _controller)
            {
                _controller.Shutdown(autokill, autoKillInMs, exitCode);
            }
            _controller = null;

            _isExit = true;
        }

        #endregion

        #region Sleep/Wait/DoEvents

        /// <summary>
        /// Suspends the current thread for a specified time.
        /// </summary>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds for which the thread is blocked. Specify zero (0) to indicate 
        /// that this thread should be suspended to allow other waiting threads to execute. 
        /// Specify Infinite to block the thread indefinitely. 
        /// </param>
        public void Sleep(int millisecondsTimeout)
        {
            Thread.Sleep(millisecondsTimeout);
        }
        /// <summary>
        /// Wait.
        /// </summary>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait. 
        /// </param>
        public void Wait(int millisecondsTimeout)
        {
            DateTime dt = DateTime.Now;
            TimeSpan ts = DateTime.Now - dt;
            int sum = 0;
            while (ts.TotalMilliseconds < millisecondsTimeout &&
                null != ApplicationManager.Instance &&
                !ApplicationManager.Instance.IsExit)
            {
                sum = 0;
                for (int i = 0; i < 100; i++)
                {
                    sum += i;
                    if (ApplicationManager.Instance.IsExit)
                        break;
                }
                DoEvents();
                ts = DateTime.Now - dt;
            }
        }
        /// <summary>
        /// Processes all Windows messages currently in the message queue.
        /// </summary>
        public void DoEvents()
        {
            DelegateInvoker.DoEvents();
        }

        #endregion

        #region Invoke

        /// <summary>
        /// Executes the specified delegate, on the thread that owns the 
        /// UI object's underlying window handle, with the specified list of arguments.
        /// </summary>
        /// <param name="del">
        /// A delegate to a method that takes parameters of the same number and type that 
        /// are contained in the args parameter.
        /// </param>
        /// <param name="args">
        /// An array of objects to pass as arguments to the specified method. 
        /// This parameter can be null if the method takes no arguments. 
        /// </param>
        /// <returns>
        /// An Object that contains the return value from the delegate being invoked, 
        /// or null if the delegate has no return value.
        /// </returns>
        public object Invoke(Delegate del, params object[] args)
        {
            return DelegateInvoker.Invoke(del, args);
        }

        #endregion

        #region Preload

        /// <summary>
        /// Preload routine for make sure required assembly is load init app domain.
        /// </summary>
        /// <param name="method">The Preload method.</param>
        public void Preload(Action method)
        {
            if (null != method)
            {
                method();
            }
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Check is application is exit.
        /// </summary>
        [Category("Application")]
        [Browsable(false)]
        public bool IsExit
        {
            get
            {
                if (null == _controller)
                {
                    return _isExit; // No controller returns local exit variable
                }
                return _controller.IsExit;
            }
        }
        /// <summary>
        /// Gets current application controller.
        /// </summary>
        [Browsable(false)]
        public IApplicationController Controller { get { return _controller; } }
#if ENABLE_ENVIRONMENT
        /// <summary>
        /// Access Application Enviromnents.
        /// </summary>
        [Category("Applications")]
        [Description("Access Application Enviromnents.")]
        public Environments Environments
        {
            get
            {
                if (null == _env)
                {
                    lock (this)
                    {
                        _env = new Environments();
                    }
                }
                return _env;
            }
        }
#endif
        /// <summary>
        /// Access Share manager.
        /// </summary>
        [Category("Application")]
        [Browsable(false)]
        public ShareManager Shares
        {
            get { return _share; }
        }

        #endregion
    }

    #endregion
}
