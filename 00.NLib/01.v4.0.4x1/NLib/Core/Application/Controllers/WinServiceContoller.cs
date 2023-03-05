﻿#define ENABLE_LOG_MANAGER

#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-08-06
=================
- NLib Application Controller.
  - Enable supports log.

======================================================================================================================
Update 2013-08-05
=================
- NLib Application Controller.
  - WinServiceContoller is split to new file.
  - remove ununsed methods.

======================================================================================================================
Update 2013-05-09
=================
- NLib Windows Service Framework redesign and refer as NLib Application Controller.
  - Merge Windows Form controller, Windows Service Controller and WPF Controller into
    single file.
  - Change namespace NLib.Services.Windows to NLib.ServiceProcess and change class name
    from WindowServiceManager to NServiceInstaller and from WindowServiceBase to NServiceBase.
  - Add ConsoleController class for work with console.

======================================================================================================================
Update 2013-01-16
=================
- NLib Windows Service Framework.
  - Change namespace from NLib.Windows.Services to NLib.Services.Windows.

======================================================================================================================
Update 2011-12-15
=================
- NLib Windows Service Framework.
  - Design and implements WindowServiceManager. This class is inherited from Install class 
    and add feature to supports install/uninstall service via command line parameters 
    for install service used (-i,/i,-install,/install) switch and for uninstall service 
    used (-u,/u,-uninstall,/uninstall) switch.
  - Design WindowServiceBase class. This class is inherited from ServiceBase class.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using NLib.ServiceProcess;

#endregion

#region Windows Service Controller and NServiceBase/NServiceInstaller

namespace NLib.ServiceProcess
{
    #region NServiceInstaller

    /// <summary>
    /// This is a custom project installer.
    /// Applies a unique name to the service using the /name switch
    /// Sets description to the service using the /desc switch
    /// Sets user name and password using the /user and /password switches
    /// Allows the use of a local account using the /account switch
    /// </summary>
    [RunInstaller(true)]
    public class NServiceInstaller : Installer
    {
        #region Internal Variables

        private ServiceProcessInstaller processInstaller;
        private ServiceInstaller serviceInstaller;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NServiceInstaller()
        {
            // Init process installer
            processInstaller = new ServiceProcessInstaller();
            processInstaller.Account = ServiceAccount.LocalSystem;
            processInstaller.Username = null;
            processInstaller.Password = null;
            //processInstaller.Parent = this;

            // Init service installer
            serviceInstaller = new ServiceInstaller();
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "MyService";
            serviceInstaller.DisplayName = string.Empty;
            serviceInstaller.Description = string.Empty;
            //serviceInstaller.Parent = this;

            // add to parent installer
            Installers.AddRange(new Installer[] 
            { 
                processInstaller, 
                serviceInstaller
            });
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~NServiceInstaller()
        {
        }

        #endregion

        #region Private Methods

        #region CallInstallUtil

        /// <summary>
        /// Run InstallUtil with specific params
        /// </summary>
        /// <param name="installUtilArguments">CommandLine params</param>
        /// <returns>Status of installation</returns>
        private bool CallInstallUtil(string installUtilArguments)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = System.IO.Path.Combine(
                NLib.IO.Folders.OS.NETRuntime,
                "installutil.exe");
            proc.StartInfo.Arguments = installUtilArguments;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;

            proc.Start();
            string outputResult = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            // check result
            if (proc.ExitCode != 0)
            {
                //Console.WriteLine("{0} : failed with code {1}", DateTime.Now, proc.ExitCode);
                //Console.WriteLine(outputResult);
                //Console.ReadLine();
                return false;
            }

            return true;
        }

        #endregion

        #region GenerateInstallutilInstallArgs

        /// <summary>
        /// Generate Service Installer args in install process.
        /// </summary>
        /// <returns>Returns arugments that need for InstallUtil.exe</returns>
        private string GenerateServiceInstallerArgs()
        {
            return GenerateServiceInstallerArgs(string.Empty);
        }
        /// <summary>
        /// Generate Service Installer args in install process.
        /// </summary>
        /// <param name="logFilePath">The logfile full file name.</param>
        /// <returns>Returns arugments that need for InstallUtil.exe</returns>
        private string GenerateServiceInstallerArgs(string logFilePath)
        {
            string installUtilArguments = " /account=\"" + this.Account + "\"";
            if (this.ServiceName != string.Empty)
            {
                installUtilArguments += " /name=\"" + this.ServiceName + "\"";
            }
            if (this.Description != string.Empty)
            {
                installUtilArguments += " /desc=\"" + this.Description + "\"";
            }
            if (this.Account == ServiceAccount.User)
            {
                installUtilArguments += " /user=\"" + this.UserName + "\" /password=\"" + this.Password + "\"";
            }

            installUtilArguments += " \"" + this.ServiceAssembly.Location + "\"";
            if (logFilePath.Trim() != string.Empty)
            {
                installUtilArguments += " /LogFile=\"" + logFilePath + "\"";
            }

            return installUtilArguments;
        }

        #endregion

        #region GenerateServiceUninstallerArgs

        /// <summary>
        /// Generate Service Installer args in uninstall process.
        /// </summary>
        /// <returns>Returns arugments that need for InstallUtil.exe</returns>
        private string GenerateServiceUninstallerArgs()
        {
            return GenerateServiceUninstallerArgs(string.Empty);
        }
        /// <summary>
        /// Generate Service Installer args in uninstall process.
        /// </summary>
        /// <param name="logFilePath">The logfile full file name.</param>
        /// <returns>Returns arugments that need for InstallUtil.exe</returns>
        private string GenerateServiceUninstallerArgs(string logFilePath)
        {
            string installUtilArguments = " /u ";
            if (this.ServiceName != "")
            {
                installUtilArguments += " /name=\"" + this.ServiceName + "\"";
            }
            installUtilArguments += " \"" + this.ServiceAssembly.Location + "\"";
            if (logFilePath.Trim() != string.Empty)
            {
                installUtilArguments += " /LogFile=\"" + logFilePath + "\"";
            }

            return installUtilArguments;
        }

        #endregion

        #endregion

        #region Overrides

        /// <summary>
        /// This method is run before the install process.
        /// This method is overriden to set the following parameters:
        /// service name (/name switch)
        /// service description (/desc switch)
        /// account type (/account switch)
        /// for a user account user name (/user switch)
        /// for a user account password (/password switch)
        /// Note that when using a user account,
        /// if the user name or password is not set,
        /// the installing user is prompted for the credentials to use.
        /// </summary>
        /// <PARAM name="savedState">The save state dictionary list.</PARAM>
        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);

            bool isUserAccount = false;

            // Decode the command line switches
            string name = GetContextParameter("name").Trim();
            if (!string.IsNullOrEmpty(name))
            {
                serviceInstaller.ServiceName = name;
            }
            string desc = GetContextParameter("desc").Trim();
            if (!string.IsNullOrEmpty(desc))
            {
                serviceInstaller.Description = desc;
            }

            // What type of credentials to use to run the service
            string acct = GetContextParameter("account");
            switch (acct.ToLower())
            {
                case "user":
                    processInstaller.Account = ServiceAccount.User;
                    isUserAccount = true;
                    break;
                case "localservice":
                    processInstaller.Account = ServiceAccount.LocalService;
                    break;
                case "localsystem":
                    processInstaller.Account = ServiceAccount.LocalSystem;
                    break;
                case "networkservice":
                    processInstaller.Account = ServiceAccount.NetworkService;
                    break;
            }

            // User name and password
            string username = GetContextParameter("user").Trim();
            string password = GetContextParameter("password").Trim();

            // Should I use a user account?
            if (isUserAccount)
            {
                // If we need to use a user account,
                // set the user name and password
                if (!string.IsNullOrEmpty(username))
                {
                    processInstaller.Username = username;
                }
                if (!string.IsNullOrEmpty(password))
                {
                    processInstaller.Password = password;
                }
            }
        }
        /// <summary>
        /// Uninstall based on the service name
        /// </summary>
        /// <PARAM name="savedState">The save state dictionary list.</PARAM>
        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);

            // Set the service name based on the command line
            string name = GetContextParameter("name").Trim();
            if (!string.IsNullOrEmpty(name))
            {
                serviceInstaller.ServiceName = name;
            }

            if (null != serviceInstaller)
            {
                // Auto Stop the Service Once Installation is Finished.
                ServiceController controller =
                    new ServiceController(serviceInstaller.ServiceName);
                controller.Stop();
            }
        }
        /// <summary>
        /// OnCommitted.
        /// </summary>
        /// <PARAM name="savedState">The save state dictionary list.</PARAM>
        protected override void OnCommitted(IDictionary savedState)
        {
            base.OnCommitted(savedState);
            if (null != serviceInstaller &&
                serviceInstaller.StartType == ServiceStartMode.Automatic)
            {
                // Auto Start the Service Once Installation is Finished.
                ServiceController controller = new ServiceController(serviceInstaller.ServiceName);
                controller.Start();
            }
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Gets to service instance.
        /// </summary>
        public virtual NServiceBase Service { get { return null; } }

        #endregion

        #region Public Methods

        #region GetContextParameter

        /// <summary>
        /// Return the value of the parameter in dicated by key
        /// </summary>
        /// <PARAM name="key">Context parameter key</PARAM>
        /// <returns>Context parameter specified by key</returns>
        public string GetContextParameter(string key)
        {
            string sValue = string.Empty;
            try
            {
                sValue = this.Context.Parameters[key].ToString();
            }
            catch
            {
                sValue = string.Empty;
            }

            return sValue;
        }

        #endregion

        #region Run

        /// <summary>
        /// Run the service.
        /// </summary>
        public void Run()
        {
            if (null != this.Service)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { this.Service };
                ServiceBase.Run(ServicesToRun);
            }
        }

        #endregion

        #region Stop

        /// <summary>
        /// Stop the service.
        /// </summary>
        public void Stop()
        {
            if (null != this.Service && this.Service.CanStop)
            {
                try
                {
                    this.Service.Stop();
                }
                catch (Exception ex)
                {
                    // stop failed.
                    throw ex;
                }
            }
        }

        #endregion

        #region Install/Uninstall

        /// <summary>
        /// Install service.
        /// </summary>
        public void Install()
        {
            Install(false);
        }
        /// <summary>
        /// Install service.
        /// </summary>
        /// <param name="useInstallUtil">Use installutil.exe to install service.</param>
        public void Install(bool useInstallUtil)
        {
            Assembly servAssem = this.ServiceAssembly;
            if (null == servAssem)
                return;
            if (useInstallUtil)
            {
                string installUtilArguments = GenerateServiceInstallerArgs(string.Empty);
                bool success = CallInstallUtil(installUtilArguments);
                if (success)
                {
                }
                else
                {
                }
            }
            else
            {
                // call static method
                Install(servAssem);
            }
        }
        /// <summary>
        /// Uninstall service.
        /// </summary>
        public void Uninstall()
        {
            Uninstall(false);
        }
        /// <summary>
        /// Uninstall service.
        /// </summary>
        /// <param name="useInstallUtil">Use installutil.exe to uninstall service.</param>
        public void Uninstall(bool useInstallUtil)
        {
            Assembly servAssem = this.ServiceAssembly;
            if (null == servAssem)
                return;
            if (useInstallUtil)
            {
                string uninstallUtilArguments = GenerateServiceUninstallerArgs(string.Empty);
                bool success = CallInstallUtil(uninstallUtilArguments);
                if (success)
                {
                }
                else
                {
                }
            }
            else
            {
                // call static method
                Uninstall(servAssem);
            }
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Service Name that shown in service manager.
        /// </summary>
        public string ServiceName
        {
            get { return serviceInstaller.ServiceName; }
            set
            {
                if (serviceInstaller.ServiceName != value)
                {
                    serviceInstaller.ServiceName = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets Service Display Name that shown in service manager.
        /// </summary>
        public string DisplayName
        {
            get { return serviceInstaller.DisplayName; }
            set
            {
                if (serviceInstaller.DisplayName != value)
                {
                    serviceInstaller.DisplayName = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets Service Description that shown in service manager.
        /// </summary>
        public string Description
        {
            get { return serviceInstaller.Description; }
            set
            {
                if (serviceInstaller.Description != value)
                {
                    serviceInstaller.Description = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets Service Start Type.
        /// </summary>
        public ServiceStartMode StartType
        {
            get { return serviceInstaller.StartType; }
            set
            {
                if (serviceInstaller.StartType != value)
                {
                    serviceInstaller.StartType = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets Service Startup Account.
        /// </summary>
        public ServiceAccount Account
        {
            get { return processInstaller.Account; }
            set
            {
                if (processInstaller.Account != value)
                {
                    processInstaller.Account = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets User Name if required to start/stop service.
        /// </summary>
        public string UserName
        {
            get { return processInstaller.Username; }
            set
            {
                if (processInstaller.Username != value)
                {
                    processInstaller.Username = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets Password if required to start/stop service.
        /// </summary>
        public string Password
        {
            get { return processInstaller.Password; }
            set
            {
                if (processInstaller.Password != value)
                {
                    processInstaller.Password = value;
                }
            }
        }
        /// <summary>
        /// Gets the service assembly.
        /// </summary>
        public Assembly ServiceAssembly
        {
            get
            {
                if (null == this.Service)
                    return null;
                else return this.Service.GetType().Assembly;
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Install service.
        /// </summary>
        /// <param name="serviceAssembly">The service assembly.</param>
        public static void Install(Assembly serviceAssembly)
        {
            if (!System.Environment.UserInteractive)
                return; // when no user login so cannot call install.
            if (null == serviceAssembly)
                return;
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] 
                { 
                    serviceAssembly.Location 
                });
            }
            catch (Exception ex)
            {
                // Install failed.
                //throw ex;
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// Uninstall service.
        /// </summary>
        /// <param name="serviceAssembly">The service assembly.</param>
        public static void Uninstall(Assembly serviceAssembly)
        {
            if (!System.Environment.UserInteractive)
                return; // when no user login so cannot call uninstall.
            if (null == serviceAssembly)
                return;
            try
            {
                ManagedInstallerClass.InstallHelper(new string[]
                { 
                    "/u", serviceAssembly.Location 
                });
            }
            catch (Exception ex)
            {
                // Uninstall failed
                //throw ex;
                Console.WriteLine(ex);
            }
        }

        #endregion
    }

    #endregion

    #region NServiceBase

    /// <summary>
    /// NLib Window Service Base abstract class.
    /// </summary>
    public abstract class NServiceBase : ServiceBase
    {

    }

    #endregion
}

namespace NLib
{
    #region WinServiceContoller

    /// <summary>
    /// Windows Services Contoller.
    /// </summary>
    public class WinServiceContoller : IApplicationController, IDisposable
    {
        #region Singelton Access

        private static WinServiceContoller _instance = null;
        /// <summary>
        /// Singelton Access instance of Window application controller.
        /// </summary>
        public static WinServiceContoller Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(WinServiceContoller))
                    {
                        _instance = new WinServiceContoller();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Internal Variables

        private bool _isAppRun = false;
        private bool _isExit = false;

        private NServiceInstaller _serviceInstaller;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private WinServiceContoller()
            : base()
        {
            // self register
            ApplicationManager.Instance.Init(this);
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~WinServiceContoller()
        {
            Dispose(true);
            Shutdown();
        }

        #endregion

        #region Private Methods

        #region ThreadExit and AppExit Events

        void Application_ThreadExit(object sender, EventArgs e)
        {
            // this event raise before application exit
            _isAppRun = true; //? may be not required.
        }

        void Application_ApplicationExit(object sender, EventArgs e)
        {
            _isAppRun = true; //? may be not required.
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
            Dispose(true);
        }
        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">Is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (null != _serviceInstaller)
            {
                _serviceInstaller.Stop();
            }
        }

        #endregion

        #endregion

        #region Run

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="serviceInstaller">
        /// The service manager with attach the actual service to run.
        /// </param>
        /// <param name="args">
        /// The service parameter like -install or -uninstall
        /// </param>
        public void Run(NServiceInstaller serviceInstaller, params string[] args)
        {
            bool isServiceRun = false;
            // assign new service manager.
            _serviceInstaller = serviceInstaller;

            if (null != _serviceInstaller && !_isAppRun)
            {
                if (System.Environment.UserInteractive)
                {
                    // for install/uninstall so used for register
                    // the service into window's service management sub system
                    // no actual service is running.
                    if (args.Length > 0)
                    {
                        #region For install or uninstall

                        if (null != _serviceInstaller.Service)
                        {
                            switch (args[0])
                            {
                                case "-install":
                                case "/install":
                                case "-i":
                                case "/i":
                                    {
                                        _serviceInstaller.Install();
                                        break;
                                    }
                                case "-uninstall":
                                case "/uninstall":
                                case "-u":
                                case "/u":
                                    {
                                        _serviceInstaller.Uninstall();
                                        break;
                                    }
                            }
                        }

                        #endregion
                    }
                }
                else
                {
                    if (null != _serviceInstaller)
                    {
                        try
                        {
                            _serviceInstaller.Run();

                            _isAppRun = true;
                            _isExit = false;

                            isServiceRun = true;

                            this.DoEvents();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            if (!isServiceRun)
            {
                _serviceInstaller = null;
                _isAppRun = false;
                _isExit = true;
            }
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
#if ENABLE_LOG_MANAGER
            //Logs.LogManager.Instance.Shutdown();
#endif
            // not used process monitor to kill app.
            //NLib.Threads.ProcessManager.Instance.Shutdown(autokill, autoKillInMs);
            _isAppRun = false;

            if (null != _serviceInstaller)
            {
                _serviceInstaller.Stop();
            }
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
        [Category("Services")]
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