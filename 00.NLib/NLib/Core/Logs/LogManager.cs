#define ENABLE_CONFIGS

#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-23
=================
- NLib Log framwork update.
  - Add Check File Exists before move.
  - LogTextFile class rewrite all code.
  - Changed default value of Log Config (Limit 5 Files, each only 1 MB).
  - LogManager Start method. Add startup text for write log message when application start.

======================================================================================================================
Update 2015-06-17
=================
- NLib Log framwork update.
  - Changes log code (used MethodBase).

======================================================================================================================
Update 2015-01-20
=================
- NLib Log framwork update.
  - Log Manager fixed log call when Save file success (from ERR to INFO).

======================================================================================================================
Update 2013-08-18
=================
- NLib Log framwork rework - update.
  - Add StartTime in Log Manager.
  - Add more serialization classes for log system.
  - Add internal classes LogRuntimeInfo and LogConfigManager for used internal.
  - All methods that used for write debug info is now keep error or information
    to Console instead of log output to prevent circular reference code.

======================================================================================================================
Update 2013-08-06
=================
- NLib Log framwork rework - serialization.
  - Change serialization classes for log system.

======================================================================================================================
Update 2013-05-07
=================
- NLib Log framwork rework - serialization.
  - Add serialization classes for log system.

======================================================================================================================
Update 2012-12-27
=================
- Log Manager update.
  - Change log code to used new DebugManager.

======================================================================================================================
Update 2012-01-02
=================
- Log Manager update.
  - Add support special method to write eventlog for unhandle exception case when used
    in Windows service.

======================================================================================================================
Update 2011-12-15
=================
- Log Manager ported.
  - Ported log manager from GFA40 to NLib

======================================================================================================================
Update 2010-12-09
=================
- Log Manager fixed bug.
  - Open folder methods fixed to open folder even if file not exists in the folder.

======================================================================================================================
Update 2010-02-04
=================
- Log Manager ported from GFA37 to GFA38v3.
  - Change to supports redirect DebugManager information to file.
  - Information/Error methods is obsolated used DebugManager.Instance.Info or DebugManager.Err
    instread.
  - changed default configurations/logs path to default ApplicationData folder.
  - Add open folder method

======================================================================================================================
Update 2009-07-14
=================
- Log Manager updated
  - Fixed path issue on vista.

======================================================================================================================
Update 2009-02-28
=================
- Log Manager updated
  - add overload method for GetConfigFileName and GetLogFileName.
  - add overload method for LoadConfig.

======================================================================================================================
Update 2008-11-26
=================
- Log Manager move from GFA.Lib to GFA.Lib.Core.

======================================================================================================================
Update 2008-10-15
=================
- SysLib.IO.PathUtils
  - Add CurrentExecutingDllPath to determine current assembly location that executing.

======================================================================================================================
Update 2008-10-15
=================
- LogManager updated.
  - fixed code that cause dead lock in Load/Save config and Information/Error.

======================================================================================================================
Update 2008-10-06
=================
- LogManager updated.
  - add internal IsRunning variable for handle message that call when log system is stop.

======================================================================================================================
Update 2008-10-06
=================
- LogManager updated.
  - Add Security Permission.

======================================================================================================================
Update 2008-09-29
=================
- LogManager updated.
  - Log Date Time format is now display second fraction 3 digit to dislpay millisecond.

======================================================================================================================
Update 2008-09-10
=================
- LogManager fixed code.
  - Fixed Shotdown method is not save config when set autosave parameter.
  - Options property is enabled for public access.

======================================================================================================================
Update 2008-04-03
=================
- LogManager Add New Options
  - Add MaxFileLimit in to Log option.
  - Add AutoEnable in to Log option.

======================================================================================================================
Update 2008-01-02
=================
- LogManager Add Events and Methods
  - SysLib.Forms.ApplicationManager Add New Method UpdateMessage to provide application wide
    message with notify new Event MessageArrived (this event will support multithread).
  - SysLib.Logs.LogManager new Events OnInformation and OnError with support multithread.

======================================================================================================================
Update 2007-11-26
=================
- Log System provide basic log system with support multithread, option to log by specificed
  categories and automatic rolling log files with easy to customizable.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Security.Permissions;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;

#endregion

using NLib;
using NLib.IO;
using NLib.Configs;
using NLib.Xml;
using NLib.Logs;

namespace NLib.Logs
{
    #region LogConfigManager

    /// <summary>
    /// Log Config Manager. Used internal.
    /// </summary>
    internal class LogConfigManager
    {
        #region Singelton

        private static LogConfigManager _instance = null;
        /// <summary>
        /// Singelton Access Instance.
        /// </summary>
        public static LogConfigManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(LogConfigManager))
                    {
                        _instance = new LogConfigManager();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Internal Variables

        // used for save/load XML option because in XmlManager write Log too. So we need to lock
        // access class Log class.
        private object _lockConfig = new object();
        private string _configFileName = string.Empty;
        private LogConfig _config = new LogConfig();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogConfigManager()
            : base()
        {

        }

        #endregion

        #region Public Methods

        private string GetConfigFileName()
        {
            if (_configFileName == string.Empty)
            {
                _configFileName = Path.Combine(
                    ApplicationManager.Instance.Environments.Product.Configs.FullName,
                    "app.log.config.xml");
            }
            return _configFileName;
        }
        /// <summary>
        /// Load Config.
        /// </summary>
        public void LoadConfig()
        {
            LogConfig cfg = null;
            MethodBase med = MethodBase.GetCurrentMethod();

            try
            {
                string fileName = GetConfigFileName();
                if (!System.IO.File.Exists(fileName))
                {
                    SaveConfig(); // Save First
                }

                lock (_lockConfig)
                {
                    cfg = XmlManager.LoadFromFile<LogConfig>(fileName);
                }

                if (cfg == null)
                {
                    // Some thing error.
                    string msg = 
                        "Cannot log load configuration file. So no changed in log configration";
                    msg.Err(med);
                }
            }
            catch (Exception ex)
            {
                ex.Err(med);
                cfg = null;
            }
            finally
            {
                if (null != cfg)
                {
                    lock (_lockConfig)
                    {
                        if (cfg.FileConfigs.Count < 0)
                        {
                            // add default config
                            cfg.FileConfigs.Add(new LogFileConfig());
                        }
                        _config = cfg;
                    }
                }
            }
        }
        /// <summary>
        /// Save Config.
        /// </summary>
        public void SaveConfig()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                string fileName = GetConfigFileName();

                bool saved = false;
                LogConfig cfg = this.Config;
                lock (_lockConfig)
                {
                    if (cfg.FileConfigs.Count <= 0)
                    {
                        // add default config
                        cfg.FileConfigs.Add(new LogFileConfig());
                    }
                    saved = XmlManager.SaveToFile<LogConfig>(fileName, cfg);
                }

                if (!saved)
                {
                    "Cannot save Log options.".Err(med);
                }
                else "Log options save success.".Info(med);
            }
            catch (Exception ex)
            {
                ex.Err(med);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the config instance.
        /// </summary>
        [Category("Logs")]
        [Browsable(false)]
        public LogConfig Config
        {
            get
            {
                if (null == _config)
                {
                    lock (_lockConfig)
                    {
                        _config = new LogConfig();
                    }
                }
                return _config;
            }
        }

        #endregion
    }

    #endregion
}

namespace NLib.Configs
{
    #region LogFileGenerateMode

    /// <summary>
    /// Log File Generate Mode.
    /// </summary>
    public enum LogFileGenerateMode
    {
        /// <summary>
        /// Generate file when application is start.
        /// </summary>
        Start,
        /// <summary>
        /// Generate file when date changed.
        /// </summary>
        Daily
    }

    #endregion

    #region LogFormat

    /// <summary>
    /// Log Format.
    /// </summary>
    [Serializable]
    public class LogFormat
    {
        #region Internal Variables

        private string _logFormat = string.Empty;
        private string _outFormat = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogFormat()
            : base()
        {
            // == Support format == 
            // {0} - Date Time
            // {1} - Log Type
            // {2} - Thread Id
            // {3} - Class name
            // {4} - Method name
            // {5} - Line Type
            // {6} - Message

            // Short Format 
            //this.Value = "[$type$]|$message$";
            // Full Format
            //this.Value = "[$type$]|<$thread$>|<$classname$.$methodname$>|$linetype$|$message$";

            this.Value = "[$type$]|<$thread$>|<$classname$.$methodname$>|$message$";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Log Message.
        /// </summary>
        /// <param name="info">The debug info</param>
        /// <returns>Retruns list of format string that used for write to log output.</returns>
        public List<string> GetLogMessage(NDebugInfo info)
        {
            List<string> results = null;
            if (null == info)
                return results;
#if ENABLE_CONFIGS
            bool isError = (info.DebugType == MessageTypes.Error);
            string datetime = info.CreateTime.ToString("yyyy/MM/dd HH:mm:ss.ffff",
                DateTimeFormatInfo.InvariantInfo);
            string msgType = (isError) ? "ERR." : "INFO";
            string threadId = info.ThreadId.ToString("D4");
            string className = info.ClassName;
            string methodName = info.MethodName;
            string lineType = "+";
            int iCnt = 0;

            string[] lines = info.Message.Split(new char[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            if (null == lines || lines.Length <= 0)
            {
                return results;
            }

            results = new List<string>(); // create result list.

            #region Write message

            foreach (string msg in lines)
            {
                if (iCnt == 0)
                {
                    lineType = "[+]";
                }
                else lineType = "[ ]";

                // == Support format == 
                // {0} - Date Time
                // {1} - Log Type
                // {2} - Thread Id
                // {3} - Class name
                // {4} - Method name
                // {5} - Line Type
                // {6} - Message
                string output = string.Format(_outFormat,
                    datetime, msgType, threadId,
                    className, methodName, lineType,
                    msg);
                // keep to list.
                results.Add(output);

                iCnt++;
            }

            #endregion
#endif
            return results;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets log format. 
        /// The support format is contains 
        /// $type$ for display log type sach as Info, Error.
        /// $thread$ for display current thread id.
        /// $classname$ for display the class name.
        /// $methodname$ for display method name.
        /// $linetype$ for [+] or [ ] in case that the messsage is contain new line so the [+] will display at first line.
        /// $message$ for display the message.
        /// </summary>
        [Category("Logs")]
        [Description("Gets or sets log line format.")]
        [XmlAttribute]
        public string Value
        {
            get { return _logFormat; }
            set
            {
                if (_logFormat != value)
                {
                    _logFormat = value;
                    // == Support format == 
                    // {0} - Date Time
                    // {1} - Log Type
                    // {2} - Thread Id
                    // {3} - Class name
                    // {4} - Method name
                    // {5} - Line Type
                    // {6} - Message

                    // Short Format 
                    //_outFormat = "[$type$]|$message$";
                    // Full Format
                    //_outFormat = "[$type$]|<$thread$>|<$classname$.$methodname$>|$linetype$|$message$";

                    _outFormat = "{0}|" +
                        _logFormat.Replace("$type$", "{1}")
                        .Replace("$thread$", "{2}")
                        .Replace("$classname$", "{3}")
                        .Replace("$methodname$", "{4}")
                        .Replace("$linetype$", "{5}")
                        .Replace("$message$", "{6}");
                }
            }
        }

        #endregion
    }

    #endregion

    #region LogFilter

    /// <summary>
    /// Log Filter.
    /// </summary>
    [Serializable]
    public class LogFilter
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogFilter()
            : base()
        {
            this.Key = string.Empty;
            this.Enable = true;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="enable">Is enable.</param>
        public LogFilter(string key, bool enable)
            : base()
        {
            this.Key = key;
            this.Enable = enable;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Equals.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;
            return this.GetHashCode().Equals(obj.GetHashCode());
        }
        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (string.IsNullOrWhiteSpace(this.Key))
                this.Key = string.Empty;
            return this.Key.Trim().GetHashCode();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets is type's name.
        /// </summary>
        [Category("Logs")]
        [Description("Gets or sets is type's name.")]
        [XmlAttribute]
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets is enable or disable log for specificed key.
        /// </summary>
        [Category("Logs")]
        [Description("Gets or sets is enable or disable log for specificed key.")]
        [XmlAttribute]
        public bool Enable { get; set; }

        #endregion
    }

    #endregion

    #region LogFilterCollection

    /// <summary>
    /// Log Filter Collection.
    /// </summary>
    [Serializable]
    [ListBindable(BindableSupport.No)]
    public class LogFilterCollection : CollectionBase
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public LogFilterCollection()
            : base()
        {
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~LogFilterCollection() { }

        #endregion

        #region List Access

        /// <summary>
        /// Add LogTypeFilter instance into Collection.
        /// </summary>
        /// <param name="value">The LogTypeFilter instance.</param>
        /// <returns>Returns index of added LogTypeFilter.</returns>
        public int Add(LogFilter value)
        {
            if (value == null || value.Key.Trim() == string.Empty)
                return -1;
            if (GetByKey(value.Key) != null)
                return -1; // Already exists
            return List.Add(value);
        }
        /// <summary>
        /// Add array of LogTypeFilter into collections.
        /// </summary>
        /// <param name="values">The array of LogTypeFilter.</param>
        public void AddRange(LogFilter[] values)
        {
            foreach (LogFilter value in values) this.Add(value);
        }
        /// <summary>
        /// Remove LogTypeFilter from Collection.
        /// </summary>
        /// <param name="value">The LogTypeFilter instance to remove.</param>
        public void Remove(LogFilter value)
        {
            if (value == null)
                return;
            List.Remove(value);
        }
        /// <summary>
        /// Indexer to access LogTypeFilter instance.
        /// </summary>
        public LogFilter this[int index]
        {
            get { return (LogFilter)List[index]; }
        }
        /// <summary>
        /// Export all LogTypeFilter in collection to Array.
        /// </summary>
        /// <returns>Returns array of LogTypeFilter.</returns>
        public LogFilter[] ToArray()
        {
            return (LogFilter[])InnerList.ToArray(typeof(LogFilter));
        }
        /// <summary>
        /// Get By Key.
        /// </summary>
        /// <param name="key">The Key to check.</param>
        /// <returns>Returns LogTypeFilter that match key.</returns>
        public LogFilter GetByKey(string key)
        {
            lock (this)
            {
                foreach (LogFilter setting in List)
                {
                    if (string.Compare(setting.Key.Trim(), key.Trim(), true) == 0)
                    {
                        return setting;
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// Check Is Enable.
        /// </summary>
        /// <param name="key">The Key to check.</param>
        /// <returns>Returns true if key is exists in collection and set as enable.</returns>
        public bool IsEnable(string key)
        {
            LogFilter setting = GetByKey(key);
            if (setting == null) return false; // No setting found
            else return setting.Enable;
        }

        #endregion
    }

    #endregion

    #region LogFilterOptions

    /// <summary>
    /// Log Filter Options.
    /// </summary>
    [Serializable]
    public class LogFilterOptions
    {
        #region Internal Variables

        private LogFilterCollection _filters = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogFilterOptions()
            : base()
        {
            this.AutoEnable = true;
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~LogFilterOptions() { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add LogTypeFilter instance into Collection.
        /// </summary>
        /// <param name="value">The LogTypeFilter instance.</param>
        /// <returns>Returns index of added LogTypeFilter.</returns>
        public int Add(LogFilter value)
        {
            return this.Filters.Add(value);
        }
        /// <summary>
        /// Get By Key.
        /// </summary>
        /// <param name="key">The Key to check.</param>
        /// <returns>Returns LogTypeFilter that match key.</returns>
        public LogFilter GetByKey(string key)
        {
            return this.Filters.GetByKey(key);
        }
        /// <summary>
        /// Check Is Enable.
        /// </summary>
        /// <param name="key">The Key to check.</param>
        /// <returns>Returns true if key is exists in collection and set as enable.</returns>
        public bool IsEnable(string key)
        {
            return this.Filters.IsEnable(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets is auto enable.
        /// </summary>
        [Category("Logs")]
        [Description("Gets or sets is auto enable.")]
        [XmlAttribute]
        public bool AutoEnable { get; set; }
        /// <summary>
        /// Get Log Filters.
        /// </summary>
        [Category("Logs")]
        [Description("Get/Set is Log Filters.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LogFilterCollection Filters 
        { 
            get
            {
                if (null == _filters)
                {
                    lock (this)
                    {
                        _filters = new LogFilterCollection();
                    }
                }
                return _filters;
            }
            set { _filters = value; }
        }

        #endregion
    }

    #endregion

    #region LogFileConfig

    /// <summary>
    /// Log File Configuration.
    /// </summary>
    [Serializable]
    public class LogFileConfig
    {
        #region Internal Variables

        // used for save/load XML option because in XmlManager write Log too. So we need to lock
        // access class Log class.
        private object _lockLog = new object();

        private string _logFileName = string.Empty;
        private LogFilterOptions _filterOptions = null;
        private LogFormat _formater = null;

        private LogTextFileWriter _logger = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogFileConfig()
            : base()
        {
            this.Prefix = "app";
            this.Enable = true;
            this.Mode = LogFileGenerateMode.Start;

            this.FileLimit = 5;
            this.FileSize = 1024 * 1024;

            CheckFilters();
            CheckFormatters();
        }

        #endregion

        #region Private Methods

        private string GetLogFileName()
        {
            string logFileNameOnly = string.Empty;
            // Check mode
            if (this.Mode == LogFileGenerateMode.Start)
            {
                #region Start mode

                if (_logFileName != string.Empty)
                {
                    // Check is need to rolling file or not by check file size for 
                    // rolling if size is over limit..
                    bool bMove = false;
                    FileInfo fInfo = _logFileName.ToFileInfo();
                    if (null != fInfo && fInfo.Exists)
                    {
                        bMove = fInfo.Length >= this.FileSize;
                    }

                    if (bMove)
                    {
                        lock (this)
                        {
                            if (null != fInfo)
                            {
                                fInfo.Rolling(this.FileLimit);
                            }
                        }

                        _logFileName = string.Empty; // reset file name.
                    }
                }

                if (_logFileName == string.Empty)
                {
                    logFileNameOnly = string.Empty;

                    if (!string.IsNullOrWhiteSpace(this.Prefix))
                    {
                        // append prefix
                        logFileNameOnly = this.Prefix;
                    }
                    else logFileNameOnly = "app";

                    // Build log file name.
                    _logFileName = Path.Combine(
                        ApplicationManager.Instance.Environments.Product.Logs.FullName,
                        logFileNameOnly + ".log");
                }

                #endregion
            }
            else
            {
                #region Daily mode

                logFileNameOnly = DateTime.Now.ToString("yyyy.MM.dd",
                    DateTimeFormatInfo.InvariantInfo);

                if (!string.IsNullOrWhiteSpace(this.Prefix))
                {
                    // append prefix
                    logFileNameOnly = this.Prefix + "." + logFileNameOnly;
                }
                // Build log file name.
                _logFileName = Path.Combine(
                    ApplicationManager.Instance.Environments.Product.Logs.FullName,
                    logFileNameOnly + ".log");

                #endregion
            }

            return _logFileName;
        }

        private void CheckFilters()
        {
            if (null == _filterOptions)
            {
                lock (this)
                {
                    _filterOptions = new LogFilterOptions();
                }
            }
        }

        private void CheckFormatters()
        {
            if (null == _formater)
            {
                lock (this)
                {
                    _formater = new LogFormat();
                }
            }
        }

        private void CheckLogger()
        {
            if (null == _logger)
            {
                lock (this)
                {
                    _logger = new LogTextFileWriter();
                }
            }
            string logFileName = this.GetLogFileName();
            if (_logger.FullFileName != logFileName)
            {
                lock (this)
                {
                    _logger.FullFileName = logFileName;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Write Information message to all log.
        /// </summary>
        /// <param name="info">The debug info</param>
        public void Write(NDebugInfo info)
        {
            if (null == info)
                return;
#if ENABLE_CONFIGS
            string category = (null != info.ClassName) ?
                info.ClassName : "Program";
            try
            {
                #region Check category

                if (this.FilterOptions.GetByKey(category) == null)
                {
                    // New key found auto append config
                    lock (this)
                    {
                        LogFilter setting = new LogFilter(category,
                            this.FilterOptions.AutoEnable);
                        // add to list
                        this.FilterOptions.Add(setting);
                    }
                    // Save config when need.
                    LogConfigManager.Instance.SaveConfig();
                }

                #endregion

                if (!this.FilterOptions.IsEnable(category))
                    return; // Not Enable

                // Formatting lines and write to logger.
                List<string> lines = this.Format.GetLogMessage(info);
                if (null != lines && lines.Count > 0)
                {
                    CheckLogger();
                    lock (_lockLog)
                    {
                        foreach (string msg in lines)
                        {
                            _logger.WriteLine(msg.Trim());
                        }
                    }
                }
            }
            finally { }
#endif
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets file prefix. Default is app.
        /// </summary>
        [Category("Logs")]
        [Description("Gets or sets file prefix. Default is app.")]
        [XmlAttribute]
        public string Prefix { get; set; }
        /// <summary>
        /// Gets or sets is log enable or disable.
        /// </summary>
        [Category("Logs")]
        [Description("Gets or sets is log enable or disable.")]
        [XmlAttribute]
        public bool Enable { get; set; }
        /// <summary>
        /// Gets or sets log file generate mode.
        /// </summary>
        [Category("Logs")]
        [Description("Gets or sets log file generate mode.")]
        [XmlAttribute]
        public LogFileGenerateMode Mode { get; set; }
        /// <summary>
        /// Gets or sets File Size. Default is 10 MB. This value used when mode is Start.
        /// </summary>
        [Category("Logs")]
        [Description("Gets or sets File Size. Default is 10 MB.")]
        [XmlAttribute]
        public long FileSize { get; set; }
        /// <summary>
        /// Gets or sets File Limit. Default is 10.
        /// </summary>
        [Category("Logs")]
        [Description("Gets or sets File Limit. Default is 10.")]
        [XmlAttribute]
        public uint FileLimit { get; set; }
        /// <summary>
        /// Gets or sets Log format.
        /// </summary>
        [Category("Logs")]
        [Description("Gets or sets Log Line format.")]
        [XmlElement]
        public LogFormat Format
        {
            get 
            {
                CheckFormatters();
                return _formater;
            }
            set 
            {
                if (_formater != value)
                {
                    lock (this)
                    {
                        _formater = value;
                    }
                    CheckFormatters();
                }
            }
        }
        /// <summary>
        /// Gets or sets is log filters options.
        /// </summary>
        [Category("Logs")]
        [Description("Gets or sets is log filters options.")]
        [XmlElement]
        public LogFilterOptions FilterOptions
        {
            get
            {
                CheckFilters();
                return _filterOptions;
            }
            set
            {
                if (_filterOptions != value)
                {
                    lock (this)
                    {
                        _filterOptions = value;
                    }
                    CheckFilters();
                }
            }
        }

        #endregion
    }

    #endregion

    #region LogConfig

    /// <summary>
    /// Log Config.
    /// </summary>
    [Serializable]
    public class LogConfig
    {
        #region Internal Variables

        private List<LogFileConfig> _fileConfigs = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogConfig()
            : base()
        {
            _fileConfigs = new List<LogFileConfig>();
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~LogConfig()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Write Information message to all logs
        /// </summary>
        /// <param name="info">The debug info</param>
        public void Write(NDebugInfo info)
        {
            if (null == info)
                return;
            if (null == this.FileConfigs)
            {
                Console.WriteLine("File Configs instance is null.");
                return;
            }
            else
            {
                LogFileConfig[] configs = null;
                lock (this)
                {
                    configs = (null != this.FileConfigs) ?
                        this.FileConfigs.ToArray() : null;
                }
                if (null != configs)
                {
                    if (configs.Length <= 0)
                    {
                        Console.WriteLine("No file config in list.");
                        return;
                    }
                    foreach (LogFileConfig fileCfg in configs)
                    {
                        if (null == fileCfg)
                            continue;
                        try
                        {
                            // write data to each file.
                            fileCfg.Write(info);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets log file config list.
        /// </summary>
        [Category("Logs")]
        [Description("Gets or sets log file config list.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<LogFileConfig> FileConfigs
        {
            get 
            {
                if (null == _fileConfigs)
                {
                    lock (this)
                    {
                        _fileConfigs = new List<LogFileConfig>();
                    }
                }
                return _fileConfigs; 
            }
        }

        #endregion
    }

    #endregion
}

namespace NLib.Logs
{
    #region Writers

    #region ILogWtiter Interface

    /// <summary>
    /// ILogWtiter Interface
    /// </summary>
    public interface ILogWtiter
    {
        /// <summary>
        /// Write message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void WriteLine(string message);
    }

    #endregion

    #region Abstract Log Writer

    /// <summary>
    /// Abstract Log Writer
    /// </summary>
    public abstract class LogWriter
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public LogWriter()
            : base()
        {
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~LogWriter()
        {
        }

        #endregion
    }

    #endregion

    #region Log Console Writer

    /// <summary>
    /// Log Console Writer
    /// </summary>
    public class LogConsoleWriter : LogWriter, ILogWtiter
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public LogConsoleWriter()
            : base()
        {
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~LogConsoleWriter()
        {
        }

        #endregion

        #region Public Method

        /// <summary>
        /// WriteLine
        /// </summary>
        /// <param name="message">Message to Write</param>
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        #endregion
    }

    #endregion

    #region Log Text File Writer

    /// <summary>
    /// Log Text File Writer
    /// </summary>
    [FileIOPermission(SecurityAction.Demand, Unrestricted = true)]
    public class LogTextFileWriter : LogWriter, ILogWtiter
    {
        #region Internal Variable

        private string _fullFileName = string.Empty;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public LogTextFileWriter()
            : base()
        {
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~LogTextFileWriter()
        {
        }

        #endregion

        #region Private Methods

        private bool IsPathValid()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            bool result = false;

            #region check file name

            if (_fullFileName.Length <= 0)
            {
                return result;
            }

            string path = Path.GetDirectoryName(_fullFileName);

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    #region Write Exception

                    if (ex != null)
                    {
                        ex.Err(med);
                    }
                    else
                    {
                        "Create Directory Unknown exception.".Err(med);
                    }

                    #endregion
                }
            }

            result = Directory.Exists(path);

            #endregion

            return result;
        }

        private FileStream CreateFileStream()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            FileStream fs = null;
            lock (this)
            {
                try
                {
                    #region Create File Stream

                    fs = System.IO.File.Open(_fullFileName,
                        System.IO.FileMode.Append,
                        System.IO.FileAccess.Write,
                        System.IO.FileShare.ReadWrite);

                    #endregion
                }
                catch (Exception ex)
                {
                    #region Write Exception to Console

                    if (ex != null)
                    {
                        ex.Err(med);
                    }
                    else
                    {
                        "Create FileStream Unknown exception.".Err(med);
                    }

                    #endregion
                }
            }
            return fs;
        }

        private void FreeFileStream(FileStream fs)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            if (fs != null)
            {
                try
                {
                    fs.Close();
                }
                catch (Exception ex)
                {
                    if (ex != null)
                    {
                        ex.Err(med);
                    }
                    else
                    {
                        "Close FileStream Unknown exception.".Err(med);
                    }
                }
            }
            fs = null;
        }

        private StreamWriter CreateStreamWriter(FileStream fs)
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            StreamWriter sw = null;
            if (null == fs)
                return sw;
            try
            {
                lock (this)
                {
                    sw = new StreamWriter(fs);
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ex.Err(med);
                }
                else
                {
                    "Create StreamWriter Unknown exception.".Err(med);
                }
            }
            return sw;
        }

        private void WriteToStream(StreamWriter sw, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            if (sw != null)
            {
                lock (this)
                {
                    sw.WriteLine(message);
                }
            }
        }

        private void FreeStreamWriter(StreamWriter sw)
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            if (sw != null)
            {
                try
                {
                    sw.Flush();
                    sw.Close();
                }
                catch (Exception ex)
                {
                    if (ex != null)
                    {
                        ex.Err(med);
                    }
                    else
                    {
                        "Flush and Close StreamWriter Unknown exception.".Err(med);
                    }
                }
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// WriteLine
        /// </summary>
        /// <param name="message">Message to Write</param>
        public void WriteLine(string message)
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            // Validate message
            if (string.IsNullOrWhiteSpace(message))
                return;
            if (!IsPathValid())
                return;

            #region Create FileStream and StreamWriter and Write Message

            lock (this)
            {
                FileStream fs = null;
                StreamWriter sw = null;

                try
                {
                    #region Create File Stream

                    fs = CreateFileStream();

                    #endregion

                    if (fs != null)
                    {
                        #region Create Writer and Write Message

                        sw = CreateStreamWriter(fs);
                        WriteToStream(sw, message);

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    #region Write Exception to Console

                    if (ex != null)
                    {
                        ex.Err(med);
                    }
                    else
                    {
                        "Create and Write StremWriter Unknown exception.".Err(med);
                    }

                    #endregion
                }
                finally
                {
                    #region Free Resources

                    // Flush and Close Stream Writer
                    FreeStreamWriter(sw);
                    sw = null;
                    // Free FileStream
                    FreeFileStream(fs);
                    fs = null;

                    #endregion
                }
            }

            #endregion
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets or sets Full File's Name
        /// </summary>
        [Category("Files")]
        [Description("Gets or sets Full File's Name")]
        public string FullFileName
        {
            get
            {
                return _fullFileName;
            }
            set
            {
                if (_fullFileName != value)
                {
                    _fullFileName = value;
                }
            }
        }

        #endregion
    }

    #endregion

    #endregion
}

namespace NLib.Logs
{
    #region LogManager

    /// <summary>
    /// Log Manager
    /// </summary>
    //[FileIOPermission(SecurityAction.Demand, Unrestricted = true)]
    public class LogManager
    {
        #region Singelton
        
        private static LogManager _instance = null;
        /// <summary>
        /// Singelton Access Instance.
        /// </summary>
        public static LogManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(LogManager))
                    {
                        _instance = new LogManager();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Internal Variables

        private bool _isRunning = false;
        private static NDebugger _debugger = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogManager() : base()
        {
            AppDomain.CurrentDomain.DomainUnload += new EventHandler(CurrentDomain_DomainUnload);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~LogManager()
        {
            AppDomain.CurrentDomain.DomainUnload -= new EventHandler(CurrentDomain_DomainUnload);
            AppDomain.CurrentDomain.ProcessExit -= new EventHandler(CurrentDomain_ProcessExit);
            Shutdown();
        }

        #endregion

        #region Private Methods

        #region AppDomain Event Handlers

        private void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            //Console.WriteLine("Domain Unload");
            "Domain Unload...".Info(med);
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            //Console.WriteLine("Process Exit");
            "Process Exit...".Info(med);
        }

        #endregion

        #region Write

        /// <summary>
        /// Write Information message to log
        /// </summary>
        /// <param name="info">The debug info</param>
        private void Write(NDebugInfo info)
        {
            if (null == info)
                return;
            if (null != this.Config)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                try 
                { 
                    this.Config.Write(info); 
                }
                catch (Exception ex)
                {
                    ex.Err(med);
                }
            }
        }

        #endregion

        #region On Debug

        void _debugger_OnDebug(object sender, NDebugEventArgs e)
        {
            Write(e.Info);
        }

        #endregion

        #region KillConsoleIME

        private void KillConsoleIME()
        {
            ProcessManager.Instance.KillConsoleIME();
        }

        #endregion

        #endregion

        #region Public Methods

        #region Start/Shutdown

        /// <summary>
        /// Start service.
        /// </summary>
        public void Start()
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            if (!_isRunning)
            {
                if (null == _debugger)
                {
                    _debugger = DebugManager.Instance.BeginDebug("LogManager");
                    if (null != _debugger)
                    {
                        _debugger.OnDebug += new NDebugEventHandler(_debugger_OnDebug);
                    }
                }
#if ENABLE_CONFIGS
                LogConfigManager.Instance.LoadConfig();
#endif
                _isRunning = true;

                string msg = string.Empty;
                string productText =
                    ApplicationManager.Instance.Environments.Options.AppInfo.DisplayText;
                msg += "=============================================================" + Environment.NewLine;
                msg += "# " + Environment.NewLine;
                msg += "# " + productText + Environment.NewLine;
                msg += "# " + Environment.NewLine;
                msg += "=============================================================";
                msg.Info(med);
                "Log system Start...".Info(med);
            }
        }
        /// <summary>
        /// Shutdown service.
        /// </summary>
        /// <param name="autoSaveConfig">True for auto save configuration when shutdown.</param>
        public void Shutdown(bool autoSaveConfig = false)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            "Log system Shutdown...".Info(med);
            DebugManager.Instance.EndDebug("LogManager");
            if (null != _debugger)
            {
                _debugger.OnDebug -= new NDebugEventHandler(_debugger_OnDebug);
            }
            _debugger = null;

            _isRunning = false;

            if (autoSaveConfig)
            {
#if ENABLE_CONFIGS
                LogConfigManager.Instance.SaveConfig();
#endif
            }
        }

        #endregion

        #region Special for Event log

        /// <summary>
        /// Write Event Log.
        /// </summary>
        /// <param name="sourceName">The evenlog source name.</param>
        /// <param name="ex">The exception instance.</param>
        /// <param name="logName">The log name default is Application.</param>
        public void WriteEventLog(string sourceName, Exception ex,
            string logName = "Application")
        {
            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, logName);
            }

            EventLog eventLog = new EventLog();
            eventLog.Source = sourceName;
            string message = string.Empty;
            if (null != ex)
            {
                message = string.Format("Exception: {0} \n\nStack: {1}",
                    ex.Message, ex.StackTrace);
            }
            else
            {
                message = "Exception occur but exception instance is null.";
            }

            eventLog.WriteEntry(message, EventLogEntryType.Error);

            eventLog.Dispose();
            eventLog = null;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the config instance.
        /// </summary>
        [Category("Logs")]
        [Browsable(false)]
        public LogConfig Config 
        {
            get { return LogConfigManager.Instance.Config; }
        }

        #endregion
    }

    #endregion
}