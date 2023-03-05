#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-07-23
=================
- NLib Application Environments updated.
  - CompanyFolders class add Shares, Data properties.
======================================================================================================================
Update 2015-06-23
=================
- NLib Application Environments updated.
  - AppInfo class add DisplayText property.
======================================================================================================================
Update 2015-06-17
=================
- NLib Application Environments updated.
  - Changes log code (used MethodBase).
  - Add Product Folder - PlugIns.
  - Add Product Folder - Data.
======================================================================================================================
Update 2013-08-04
=================
- NLib Application Environments redesign.
  - All classes and emums are removed for reconsider the proper way to supports more
    flexible features.
  - Add NAppFolder enum. (original is DataFolder).
  - Add NAppInformation class to define company, product, application version.
  - Add NAppStorage class to define root data folder type.
  - Add NAppBehaviors class to define application behaviors like single instance, 
    enable debug, etc.
======================================================================================================================
Update 2013-05-13
=================
- NLib Application Environments redesign.
  - Add ConfigFileName class.
  - Add ConfigFileNames class.
======================================================================================================================
Update 2013-05-08
=================
- NLib Application Environments redesign.
  - AppDataRootFolder enum changed to DataFolder enum.
  - All properties in EnvironmentOptions class is now used auto properties.
  - Add Indexer access in CompanyFolders and ProductFolders class.
  - Remove some unused folders that can add later by user code like image, medias.
======================================================================================================================
Update 2012-12-25
=================
- NLib Application Environments updated.
  - Change Environments's folder access to NFolder instance.
  - CompanyFolders class change access to NFolder instance.
  - ProductFolders class change access to NFolder instance.
======================================================================================================================
Update 2012-12-21
=================
- NLib Application Environments updated.
  - Add Custom in AppDataRootFolder enum type.
======================================================================================================================
Update 2012-01-04
=================
- NLib Application Environments updated.
  - Add supports application single instance.
======================================================================================================================
Update 2011-12-15
=================
- NLib Application Environments.
  - Add new EnvironmentOptions class. For custom define where to generate folders
    for keep data for each company and company's related products which currently
    supports to keep application local data on User profile, Common Program Data
    and on root folder in drive that windows installed.
  - Add CompanyFolders class. For keep information about common folders that may
    need for share among all product or application for company.
  - Add ProductFolders class. For keep information about common folders that may
    need for used to read/write/execute by the application itself.
  - All Company's Folders and Product's Folders is now checks and grants permission
    to access by all users on local system. In future may need to optimized for speed
    when make sure that the administrator will not change the permission on folders or
    files during application is running.
  - Add Enviromments class. This class is basically ported the common folder access
    from GFA40 in SysLib.IO.Paths class and add wrapper properties for access Company's
    Folders, Product's Folders and EnvironmentOptions. When used in application the 
    class that implements IApplicationController should setting EnvironmentOptions 
    before access data in folders.
  - Changes PathKeyPair class in GFA40 to KeyValue in NLib for general used in another 
    classes for future used.
======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

#endregion

using NLib.IO;

namespace NLib
{
    #region NAppFolder

    /// <summary>
    /// Application Root's folder location for keep data like config, logs, resources, image, etc.
    /// </summary>
    public enum NAppFolder
    {
        /// <summary>
        /// Use current user profile folder as root directory to keep file and folders.
        /// </summary>
        LocalUser,
        /// <summary>
        /// Use Commmon Program's Data folder as root directory to keep file and folders.
        /// </summary>
        ProgramData,
        /// <summary>
        /// Use root of drive that windows installed as root directory to keep file and folders.
        /// </summary>
        WindowsRootDrive,
        /// <summary>
        /// Use appliation location as root directory to keep file and folders.
        /// </summary>
        Application,
        /// <summary>
        /// Use custom path as root directory to keep file and folders.
        /// </summary>
        Custom
    }

    #endregion

    #region NAppInformation

    /// <summary>
    /// The Application Information.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NAppInformation
    {
        #region Constructor
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public NAppInformation() : base()
        {
            this.CompanyName = "MyCompany";
            this.ProductName = "MyProduct";
            this.Version = "1.0";
            this.Minor = "0";
            this.Build = "0";
            this.LastUpdate = new DateTime(2015, 1, 1, 0, 0, 0);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Company Name. This value is used for create root path
        /// for all applications that made from the same company.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets Company Name. This value is used for create root path for all applications that made from the same company.")]
        public string CompanyName { get; set; }
        /// <summary>
        /// Gets Application or Product Name.
        /// This value is used for create sub directory for current application in company root path.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets Application or Product Name. This value is used for create sub directory for current application in company.")]
        public string ProductName { get; set; }
        /// <summary>
        /// Gets or sets Product Version.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets Application or Product Version. This value is used for detected application instance.")]
        public string Version { get; set; }
        /// <summary>
        /// Gets or sets Product Minor Version.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets Application or Product Minor Version. This value is used for detected application instance.")]
        public string Minor { get; set; }
        /// <summary>
        /// Gets or sets Product Build number.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets Application or Product Build number. This value is used for detected application instance.")]
        public string Build { get; set; }
        /// <summary>
        /// Gets or sets Product Last Update Date.
        /// </summary>
        [Category("Options")]
        [Description("Gets Application or Last Update Date. This value is used for detected application instance.")]
        public DateTime LastUpdate { get; set; }
        /// <summary>
        /// Gets display text for product info.
        /// </summary>
        [XmlIgnore]
        [Category("Options")]
        [Description("Gets display text for product info.")]
        public string DisplayText
        {
            get
            {
                return string.Format("{0} v{1}.{2} build {3} {4:yyyy/MM/dd HH:mm}",
                    this.ProductName, this.Version, this.Minor, this.Build, this.LastUpdate);
            }
        }

        #endregion
    }

    #endregion

    #region NAppStorage
    
    /// <summary>
    /// NAppStorage class. This class provide information about root folder for keep application data.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NAppStorage
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NAppStorage() : base()
        {
            this.StorageType = NAppFolder.Application;
            this.CustomFolder = string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets application's runtime root data's folder.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets application's runtime root data's folder.")]
        [XmlIgnore]
        public NAppFolder StorageType { get; set; }
        /// <summary>
        /// Gets or sets application's runtime custom root data's folder.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets application's runtime custom root data's folder.")]
        [Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [XmlIgnore]
        public string CustomFolder { get; set; }

        #endregion
    }

    #endregion

    #region NAppBehaviors

    /// <summary>
    /// NAppBehaviors class. This class provide application behaviors like single instance, etc.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class NAppBehaviors
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NAppBehaviors() : base()
        {
            this.IsSingleAppInstance = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets is current application can allow only single application instance.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets is current application can allow only single application instance.")]
        [XmlIgnore]
        public bool IsSingleAppInstance { get; set; }
        /// <summary>
        /// Gets or sets is EnableDebuggers is enable or disable. This value should always be true.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets is EnableDebuggers is enable or disable.")]
        [XmlIgnore]
        public bool EnableDebuggers 
        {
            get { return DebugManager.Instance.IsEnable; }
            set { DebugManager.Instance.IsEnable = value; } 
        }

        #endregion
    }

    #endregion

    #region EnvironmentOptions

    /// <summary>
    /// Enviroment Options class.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class EnvironmentOptions
    {
        #region Internal Variables

        private NAppInformation _appInfo = null;
        private NAppStorage _appStorage = null;
        private NAppBehaviors _appBehaviors = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public EnvironmentOptions()
            : base()
        {
            _appInfo = new NAppInformation();
            _appStorage = new NAppStorage();
            _appBehaviors = new NAppBehaviors();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the application information instance.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets the application information instance.")]
        [XmlIgnore]
        public NAppInformation AppInfo 
        { 
            get 
            {
                if (null == _appInfo)
                {
                    lock (this)
                    {
                        _appInfo = new NAppInformation();
                    }
                }
                return _appInfo; 
            }
            set 
            { 
                _appInfo = value;
                if (null == _appInfo)
                {
                    lock (this)
                    {
                        _appInfo = new NAppInformation();
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the application data storage instance.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets the data application storage instance.")]
        [XmlIgnore]
        public NAppStorage Storage
        {
            get
            {
                if (null == _appStorage)
                {
                    lock (this)
                    {
                        _appStorage = new NAppStorage();
                    }
                }
                return _appStorage;
            }
            set
            {
                _appStorage = value;
                if (null == _appStorage)
                {
                    lock (this)
                    {
                        _appStorage = new NAppStorage();
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the application behaviors instance.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets the application behaviors instance.")]
        [XmlElement]
        public NAppBehaviors Behaviors
        {
            get
            {
                if (null == _appBehaviors)
                {
                    lock (this)
                    {
                        _appBehaviors = new NAppBehaviors();
                    }
                }
                return _appBehaviors;
            }
            set
            {
                _appBehaviors = value;
                if (null == _appBehaviors)
                {
                    lock (this)
                    {
                        _appBehaviors = new NAppBehaviors();
                    }
                }
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Checks is an environment options is valid.
        /// </summary>
        /// <param name="option">An environment options instance.</param>
        /// <returns>Returns true if environment options instance is valid.</returns>
        public static bool IsValid(EnvironmentOptions option)
        {
            bool result = true;
            if (result && null == option || null == option.AppInfo ||
                string.IsNullOrWhiteSpace(option.AppInfo.CompanyName) ||
                string.IsNullOrWhiteSpace(option.AppInfo.ProductName) ||
                null == option.Storage || null == option.Behaviors)
            {
                result = false;
                throw new ArgumentException("The environment option cannot be null and it's data should assigned properly.",
                    "option");
            }
            return result;
        }

        #endregion
    }

    #endregion

    #region CommonAppFolders

    /// <summary>
    /// Common App Folders abstract class.
    /// </summary>
    public abstract class CommonAppFolders
    {
        #region Internal Variables

        private EnvironmentOptions _options = new EnvironmentOptions();

        #endregion

        #region Protected Methods

        /// <summary>
        /// OnSetup. Occur when Setup method is called.
        /// </summary>
        protected virtual void OnSetup()
        {

        }
        /// <summary>
        /// Checks is all required variables that used for create folders is valid.
        /// </summary>
        /// <returns>Returns true if all variables is valid.</returns>
        protected bool Valid()
        {
            return EnvironmentOptions.IsValid(this._options);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Setup Environment options.
        /// </summary>
        /// <param name="option">The Environment options instance.</param>
        public void Setup(EnvironmentOptions option)
        {
            if (null == option ||
                null == option.AppInfo ||
                null == option.Storage ||
                null == option.Behaviors)
            {
                throw new ArgumentException("The environment option cannot be null and it's data should assigned properly.",
                    "option");
            }
            // assign option to local variable.
            _options = option;

            OnSetup();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Environment Options.
        /// </summary>
        [Browsable(false)]
        public EnvironmentOptions Options { get { return _options; } }

        #endregion
    }

    #endregion

    #region CompanyFolders

    /// <summary>
    /// The Company related's data folders.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class CompanyFolders : CommonAppFolders
    {
        #region Internal Variables

        private NFolder _companyFolder = null;

        private NFolder _tempFolder = null;
        private NFolder _configFolder = null;

        private NFolder _shareFolder = null;
        private NFolder _dataFolder = null;

        private NFolder _allProductFolder = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        internal CompanyFolders() : base() { }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~CompanyFolders()
        {
            _allProductFolder = null;

            _shareFolder = null;
            _dataFolder = null;

            _tempFolder = null;
            _configFolder = null;

            _companyFolder = null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the Root folder that used for common places to keep all
        /// applications data for the same company.
        /// </summary>
        internal string GetRootFolder()
        {
            string result = string.Empty;

            if (!Valid())
            {
                return result;
            }

            string rootFolder = string.Empty;
            MethodBase med = MethodBase.GetCurrentMethod();

            switch (this.Options.Storage.StorageType)
            {
                case NAppFolder.LocalUser:
                    {
                        // To Users profile AppData
                        rootFolder =
                            Folders.Users.LocalData;
                    }
                    break;
                case NAppFolder.ProgramData:
                    {
                        // To ProgramData
                        rootFolder =
                            Folders.Locals.CommonAppData;
                    }
                    break;
                case NAppFolder.Application:
                    {
                        // To Application location path
                        Assembly assem = null;
                        try
                        {
                            assem = Assembly.GetEntryAssembly();
                            rootFolder = Path.GetDirectoryName(assem.Location);
                        }
                        catch (Exception ex)
                        {
                            ex.Err(med);
                            rootFolder = string.Empty;
                        }
                    }
                    break;
                case NAppFolder.Custom:
                    {
                        // User Custom Path.
                        try
                        {
                            rootFolder =
                                Path.GetFullPath(this.Options.Storage.CustomFolder);
                        }
                        catch (Exception ex)
                        {
                            ex.Err(med);
                            rootFolder = string.Empty;
                        }
                    }
                    break;
                default:
                    {
                        try
                        {
                            // To drive that installed windows
                            rootFolder =
                                Path.GetPathRoot(Folders.OS.Windows);
                        }
                        catch (Exception ex)
                        {
                            ex.Err(med);
                            rootFolder = string.Empty;
                        }
                    }
                    break;
            }

            if (rootFolder != string.Empty)
            {
                // Build company common path for all apps.
                result = Path.Combine(rootFolder, this.Options.AppInfo.CompanyName);
                bool hasError = false;
                if (!Folders.Exists(result))
                {
                    // create if required.
                    hasError = !Folders.Create(result).Success;
                }

                if (!hasError)
                {
                    // grant if required.
                    hasError = !Folders.Grant(result).Success;
                }
                else
                {
                    // cannot create folder or grant permission
                    result = string.Empty;
                }
            }
            return result;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// OnSetup. Occur when Setup method is called.
        /// </summary>
        protected override void OnSetup()
        {
            string rootPath = GetRootFolder();
            if (string.IsNullOrWhiteSpace(rootPath))
                return; // something is invalid.

            _companyFolder = new NFolder(rootPath);

            // Init common folders for company
            _tempFolder = _companyFolder["Temp"];
            _configFolder = _companyFolder["Configs"];
            _shareFolder = _companyFolder["Shares"];
            _dataFolder = _companyFolder["Data"];
            _allProductFolder = _companyFolder["Products"];
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the root folder full name.
        /// </summary>
        [Category("Folders")]
        [Description("Gets the root folder full name.")]
        public string Root { get { return this.GetRootFolder(); } }
        /// <summary>
        /// Indexer access sub folder in Company folder.
        /// </summary>
        /// <param name="subfolder">The sub folder path.</param>
        /// <param name="autoCreateIfNotExist">True for auto create.</param>
        /// <returns>Returns instance of NFolder that link to the sub folder.</returns>
        [Category("Folders")]
        [Description("Indexer access sub folder in Company folder.")]
        public NFolder this[string subfolder, bool autoCreateIfNotExist = false]
        {
            get
            {
                if (null == _companyFolder)
                    return null;
                return _companyFolder[subfolder, autoCreateIfNotExist];
            }
        }
        /// <summary>
        /// Gets the Company common configs folder that used for all applications.
        /// </summary>
        [Category("Folders")]
        [Description("Gets the Company common configs folder that used for all applications.")]
        public NFolder Configs { get { return _configFolder; } }
        /// <summary>
        /// Gets the Company common temp folder that used for all applications.
        /// </summary>
        [Category("Folders")]
        [Description("Gets the Company common temp folder that used for all applications.")]
        public NFolder Temp { get { return _tempFolder; } }
        /// <summary>
        /// Gets the Company common share folder that used for all applications.
        /// </summary>
        [Category("Folders")]
        [Description("Gets the Company common share folder that used for all applications.")]
        public NFolder Share { get { return _shareFolder; } }
        /// <summary>
        /// Gets the Company common data folder that used for all applications.
        /// </summary>
        [Category("Folders")]
        [Description("Gets the Company common data folder that used for all applications.")]
        public NFolder Data { get { return _dataFolder; } }
        /// <summary>
        /// Gets the Company common products folder that used for all applications.
        /// </summary>
        [Category("Folders")]
        [Description("Gets the Company common products folder that used for all applications.")]
        public NFolder Products { get { return _allProductFolder; } }

        #endregion
    }

    #endregion

    #region ProductFolders

    /// <summary>
    /// The Product related's data folders.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class ProductFolders : CommonAppFolders
    {
        #region Internal Variables

        private CompanyFolders _companyFolders = null;
        private NFolder _productFolder = null;
        private NFolder _configFolder = null;
        private NFolder _logsFolder = null;
        private NFolder _tempFolder = null;

        private NFolder _plugInFolder = null;
        private NFolder _dataFolder = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private ProductFolders() : base() { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="companyFolders">The company folders instance.</param>
        internal ProductFolders(CompanyFolders companyFolders)
            : this()
        {
            _companyFolders = companyFolders;
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~ProductFolders()
        {
            _configFolder = null;
            _logsFolder = null;
            _tempFolder = null;
            _plugInFolder = null;

            _productFolder = null;
            _dataFolder = null;

            _companyFolders = null;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// OnSetup. Occur when Setup method is called.
        /// </summary>
        protected override void OnSetup()
        {
            if (null == _companyFolders)
                return;

            NFolder _allProducts = _companyFolders.Products;
            if (null != _allProducts)
            {
                _productFolder = _allProducts[this.Options.AppInfo.ProductName];

                // Init common folders for company
                _configFolder = _productFolder["Configs"];
                _logsFolder = _productFolder["Logs"];
                _tempFolder = _productFolder["Temp"];
                _plugInFolder = _productFolder["PlugIns"];
                _dataFolder = _productFolder["Data"];
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Indexer access sub folder in current application folder.
        /// </summary>
        /// <param name="subfolder">The sub folder path.</param>
        /// <param name="autoCreateIfNotExist">True for auto create.</param>
        /// <returns>Returns instance of NFolder that link to the sub folder.</returns>
        [Category("Folders")]
        [Description("Indexer access sub folder in current application folder.")]
        public NFolder this[string subfolder, bool autoCreateIfNotExist = false]
        {
            get
            {
                if (null == _productFolder)
                    return null;
                return _productFolder[subfolder, autoCreateIfNotExist];
            }
        }
        /// <summary>
        /// Gets the program common configs folder that used for current application.
        /// </summary>
        [Category("Folders")]
        [Description("Gets the program common configs folder that used for current application.")]
        public NFolder Configs { get { return _configFolder; } }
        /// <summary>
        /// Gets the program common logs folder that used for current application.
        /// </summary>
        [Category("Folders")]
        [Description("Gets the program common logs folder that used for current application.")]
        public NFolder Logs { get { return _logsFolder; } }
        /// <summary>
        /// Gets the program common temp folder that used for all applications.
        /// </summary>
        [Category("Folders")]
        [Description("Gets the program common temp folder that used for current application.")]
        public NFolder Temp { get { return _tempFolder; } }
        /// <summary>
        /// Gets the program common Plug In(s) folder that used for all applications.
        /// </summary>
        [Category("Folders")]
        [Description("Gets the program common Plug In(s) folder that used for current application.")]
        public NFolder PlugIns { get { return _plugInFolder; } }
        /// <summary>
        /// Gets the program common Data folder that used for all applications.
        /// </summary>
        [Category("Folders")]
        [Description("Gets the program common Data folder that used for current application.")]
        public NFolder Data { get { return _dataFolder; } }

        #endregion
    }

    #endregion

    #region Environments

    /// <summary>
    /// Application's Environments class.
    /// </summary>
    public sealed class Environments
    {
        #region Static Access to App instance

        private static AppInstance _appinstance = null;
        /// <summary>
        /// Gets App Instance. Used for checks is allow to run only one instance.
        /// </summary>
        public static AppInstance AppInstance
        {
            get { return _appinstance; }
            private set { _appinstance = value; }
        }

        #endregion

        #region Internal Variables

        private EnvironmentOptions _options = new EnvironmentOptions();
        private CompanyFolders _companyFolder = null;
        private ProductFolders _productFolder = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public Environments() : base() { }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~Environments()
        {
            _productFolder = null;
            _companyFolder = null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Init Application Data root folder and setup or create Company folder with auto grant
        /// access secruity.
        /// </summary>
        private void InitAppFolders()
        {
            // Reset variables
            _companyFolder = null;

            if (null != _options && EnvironmentOptions.IsValid(this._options))
            {
                _companyFolder = new CompanyFolders();
                _companyFolder.Setup(this._options);

                _productFolder = new ProductFolders(_companyFolder);
                _productFolder.Setup(this._options);
            }
        }

        private void InitSingleAppInstance()
        {
            if (null == _options || null == _options.Behaviors)
            {
                // set to null.
                Environments.AppInstance = null;
            }
            else
            {
                if (_options.Behaviors.IsSingleAppInstance)
                {
                    // Create mew app instance. That used for check later.
                    Environments.AppInstance = new AppInstance();
                    // The full version of Create mew app instance.
                    /*
                    Environments.AppInstance = new AppInstance(_options.Version, _options.Minor,
                        _options.Build, _options.LastUpdate);
                    */
                }
                else
                {
                    Environments.AppInstance = null;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Setup Essential values for application environments. 
        /// This value should set at first time before application
        /// start or run.
        /// </summary>
        /// <param name="option">The Environment's option.</param>
        public void Setup(EnvironmentOptions option)
        {
            if (null == option ||
                null == option.AppInfo ||
                null == option.Storage ||
                null == option.Behaviors)
            {
                throw new ArgumentException("The environment option cannot be null and it's data should assigned properly.",
                    "option");
            }
            // assign option to local variable.
            _options = option;
            // init required folders
            InitAppFolders();
            // set static access single application instance.
            InitSingleAppInstance();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Enviroment Options.
        /// </summary>
        [Category("Options")]
        [Description("Gets the Enviroment Options.")]
        public EnvironmentOptions Options { get { return _options; } }
        /// <summary>
        /// Gets the company folder. This is the root folder for keep all folders and fles that
        /// work with application for specificed company. This value is exists after an options
        /// was setup.
        /// </summary>
        [Category("Folders")]
        [Description("Gets the company folder. This is the root folder for keep all folders and fles that work with application for specificed company.")]
        public CompanyFolders Company { get { return _companyFolder; } }
        /// <summary>
        /// Gets the current product folder. This is the root folder for current product.
        /// </summary>
        [Category("Folders")]
        [Description("Gets the current product folder. This is the root folder for current product.")]
        public ProductFolders Product { get { return _productFolder; } }
        /// <summary>
        /// Checks that the is application is running more than one instance.
        /// </summary>
        [Category("Appliaction")]
        [Description("Checks that the is application is running more than one instance.")]
        public bool HasMoreInstance
        {
            get
            {
                return (Environments.AppInstance != null &&
                    Environments.AppInstance.HasMoreInstance);
            }
        }

        #endregion
    }

    #endregion
}