#define USE_NLIB_IO

#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2012-12-25
=================
- ResourceUtils class changed.
  - Directory and File now used NLib IO classes.
  - Add some log code.

======================================================================================================================
Update 2012-12-19
=================
- ResourceUtils class changed.
  - Change call DebugManager call to Extension method call.

======================================================================================================================
Update 2012-01-02
=================
- ResourceUtils class 
  - Remove all using System.Windows.Forms.
  - Remove ResourceUtils classs.
  - Design Resource Access related classes.

======================================================================================================================
Update 2010-08-31
=================
- Resources Utils ported from GFA38v3 to GFA40 and GFA20.

======================================================================================================================
Update 2010-01-23
=================
- ResourceUtils class ported from GFA Library GFA37 tor GFA38v3
  - Changed all exception handling code to used new ExceptionManager class instread of 
    LogManager.

======================================================================================================================
Update 2008-11-27
=================
- ResourceUtils move from GFA.Lib to GFA.Lib.Core.

======================================================================================================================
Update 2007-12-01
=================
- ResourceUtils class added
  1. ResourceUtils class is wrapper class that used for load resource into Stream/Image
  2. OracleUtils class provide automatic install oracle client if need. This will used
     with oracle10g only. Note that this class will automaticall used internally with 
     OracleDirectConnectionConfig please do not call directly.
  3. Ported ImageProcessing classes from GFA Library v.2.27.

======================================================================================================================
Update 2007-11-26
=================
- Utilities provides support classes with some fixed for multithread environment and optimized
  for performance

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Diagnostics;

#if USE_NLIB_IO
using NLib.IO;
#endif

#endregion

namespace NLib.Utils
{
    #region Option classes for Resource Access

    /// <summary>
    /// Resource Access Abstract Options.
    /// </summary>
    public abstract class ResourceAccessAbstractOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets caller type.
        /// </summary>
        public Type CallerType { get; set; }
        /// <summary>
        /// Gets the resource assembly based on Caller Type.
        /// </summary>
        public Assembly CallerAssembly
        {
            get
            {
                if (null == this.CallerType)
                    return null;
                else return this.CallerType.Assembly;
            }
        }
        /// <summary>
        /// Gets or sets resource name.
        /// </summary>
        public string ResourceName { get; set; }

        #endregion
    }
    /// <summary>
    /// Resource Stream Options.
    /// </summary>
    public class ResourceStreamOptions : ResourceAccessAbstractOptions
    {
        #region Public Properties
        
        /// <summary>
        /// Gets or set target path.
        /// </summary>
        public string TargetPath { get; set; }
        /// <summary>
        /// Gets or sets target file name.
        /// </summary>
        public string TargetFileName { get; set; }
        /// <summary>
        /// Gets the target full file name based on TargetPath and TargetFileName property.
        /// </summary>
        public string TargetFullFileName
        {
            get
            {
                return Path.Combine(this.TargetPath, this.TargetFileName);
            }
        }
        /// <summary>
        /// Checks is required file's name properties is valid.
        /// </summary>
        public bool IsValidFileName
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.TargetPath) &&
                    !string.IsNullOrWhiteSpace(this.TargetFileName);
            }
        }

        #endregion
    }
    /// <summary>
    /// Resource Tool Execute Options.
    /// </summary>
    public class ResourceExecuteOptions : ResourceStreamOptions
    {
        #region Public Properties
        
        /// <summary>
        /// Gets or sets auto create resource temp file before execute.
        /// </summary>
        public bool AutoCreate { get; set; }
        /// <summary>
        /// Gets or sets auto delete resource temp file after execute.
        /// </summary>
        public bool AutoDelete { get; set; }
        /// <summary>
        /// Gets or sets show application window or hidden it.
        /// </summary>
        public bool ShowWindow { get; set; }
        /// <summary>
        /// Gets or sets arguments for executing target file name.
        /// </summary>
        public string Argument { get; set; }

        #endregion
    }

    #endregion

    #region ResourceAccess (abstract)

    /// <summary>
    /// IResourceAccess interface.
    /// </summary>
    public abstract class ResourceAccess : IDisposable
    {
        #region Internal Variable

        private bool _showException = false;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ResourceAccess() : base() { }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~ResourceAccess()
        {
            Dispose(false);
        }

        #endregion

        #region Public Methods

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
        /// <param name="disposing">True if in disposing process.</param>
        public virtual void Dispose(bool disposing)
        {
        }
        /// <summary>
        /// Get Stream by specificed option.
        /// </summary>
        /// <param name="option">The resouce stream option.</param>
        /// <returns>Return Null if not found and Return Target Resource Stream is found</returns>
        public virtual Stream GetStream(ResourceStreamOptions option)
        {
            return null;
        }
        /// <summary>
        /// Get Stream by specificed option.
        /// </summary>
        /// <param name="option">The resouce stream option.</param>
        /// <returns>Return Null if not found and Return Target Resource Stream is found</returns>
        public virtual Image GetImage(ResourceStreamOptions option)
        {
            return null;
        }
        /// <summary>
        /// Create File from resource name.
        /// </summary>
        /// <param name="option">The resouce stream option.</param>
        /// <returns>
        /// Returns true if file is successfully extract from resource and save into file.
        /// </returns>
        public bool CreateFile(ResourceStreamOptions option)
        {
            bool result = false;
            if (null == option ||
                string.IsNullOrWhiteSpace(option.ResourceName) ||
                !option.IsValidFileName)
            {
                return result;
            }
            string FullFileName = Path.Combine(option.TargetPath, option.TargetFileName);
            string dir = Path.GetDirectoryName(FullFileName);
            if (!Directory.Exists(dir))
            {
#if USE_NLIB_IO
                bool hasError = !Folders.Create(dir).Success;
                if (!hasError)
                {
                    // grant if required.
                    hasError = !Folders.Grant(dir).Success;
                }
                else
                {
                    // cannot create folder or grant permission
                    hasError = true; // Has error
                }
#else
                Directory.CreateDirectory(dir);
#endif

            }
            if (Directory.Exists(dir) && !File.Exists(FullFileName))
            {
                Stream stream = this.GetStream(option);
                if (null != stream)
                {
                    FileStream fs = null;
                    MethodBase med = MethodBase.GetCurrentMethod();

                    #region Create File Stream

                    try
                    {
                        fs = new FileStream(FullFileName, FileMode.CreateNew);
                    }
                    catch (Exception ex)
                    {
                        ex.Err(med);
                    }

                    #endregion

                    #region Write bytes

                    if (null != fs)
                    {
                        try
                        {
                            for (int i = 0; i < stream.Length; i++)
                                fs.WriteByte((byte)stream.ReadByte());
                        }
                        catch (Exception ex)
                        {
                            ex.Err(med);
                        }
                    }

                    #endregion

                    #region Free File stream

                    try
                    {
                        if (null != fs)
                        {
                            fs.Flush();
                            fs.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Err(med);
                    }
                    finally { fs = null; }

                    #endregion

                    #region Close Stream

                    if (null != stream)
                    {
                        try { stream.Close(); }
                        catch { }
                    }

                    #endregion

                    stream = null;
                    result = true;
                }
            }
            // Check is file exists
            result = File.Exists(FullFileName);

            return result;
        }
        /// <summary>
        /// Delete File.
        /// </summary>
        /// <param name="option">The resouce stream option.</param>
        /// <returns>Returns true if file is not exists or successfully deleted.</returns>
        public bool DeleteFile(ResourceStreamOptions option)
        {
            if (null == option ||
                null == option.CallerType ||
                null == option.CallerAssembly ||
                !option.IsValidFileName)
                return false;
            string FullFileName = option.TargetFullFileName;
            if (File.Exists(FullFileName))
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                try
                {
                    File.Delete(FullFileName);
                    return true;
                }
                catch (Exception ex)
                {
                    // Write Debug Message
                    ex.Err(med);
                    if (this.ShowException)
                        throw ex;
                    else return false;
                }
            }
            else return true;
        }
        /// <summary>
        /// Execute application in hidden or background process and wail until finished.
        /// </summary>
        /// <param name="option">The resouce tool execute option.</param>
        public virtual void Execute(ResourceExecuteOptions option)
        {
            if (null == option ||
                null == option.CallerType ||
                null == option.CallerAssembly ||
                !option.IsValidFileName)
                return;
            
            MethodBase med = MethodBase.GetCurrentMethod();

            if (option.AutoCreate)
            {
                if (!CreateFile(option))
                {                    
                    // Write Debug Message
                    Exception ex = new Exception("Cannot create file for execute.");
                    ex.Err(med);
                    return; // create file failed.
                }
            }

            string oldDir = System.IO.Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(option.TargetPath); // Change current directory
            Process process = new Process();
            if (process.StartInfo == null)
            {
                // Create new if required.
                process.StartInfo = new ProcessStartInfo();
            }
            process.StartInfo.CreateNoWindow = !option.ShowWindow; // create no window or not
            process.StartInfo.WorkingDirectory = option.TargetPath;
            process.StartInfo.FileName = option.TargetFullFileName; // target app full name
            // check is has argument.
            if (!string.IsNullOrWhiteSpace(option.Argument))
            {
                process.StartInfo.Arguments = option.Argument; // add argument if required.
            }
            // check for process window style.
            if (!option.ShowWindow)
            {
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; // Hidden
            }

            if (process.Start())
            {
                process.WaitForExit(); // Execute and wait for exit.
            }
            else
            {
                // Write Debug Message
                Exception ex = new Exception("Cannot execute command line.");
                ex.Err(med);
            }

            if (option.AutoDelete)
            {
                if (!DeleteFile(option))
                {
                    // delete file failed.
                    Exception ex = new Exception("Cannot delete file.");
                    ex.Err(med);
                }
            }

            System.IO.Directory.SetCurrentDirectory(oldDir); // Change current directory
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set to show exception
        /// </summary>
        [Description("Get/Set to show exception")]
        public bool ShowException
        {
            get { return _showException; }
            set { _showException = value; }
        }

        #endregion
    }

    #endregion

    #region Windows Forms

    /// <summary>
    /// WindowFormsResourceAccess class.
    /// </summary>
    public class WindowFormsResourceAccess : ResourceAccess
    {
        #region Internal Variables

        private Assembly _resAssem = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public WindowFormsResourceAccess() : base() { }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~WindowFormsResourceAccess()
        {
            _resAssem = null;
        }

        #endregion

        #region Overrides

        #region GetStream

        /// <summary>
        /// Get Stream by specificed option.
        /// </summary>
        /// <param name="option">The resouce stream option.</param>
        /// <returns>Return Null if not found and Return Target Resource Stream is found</returns>
        public override Stream GetStream(ResourceStreamOptions option)
        {
            if (option == null ||
                null == option.CallerType ||
                null == option.CallerAssembly)
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(option.ResourceName))
            {
                return null;
            }
            MethodBase med = MethodBase.GetCurrentMethod();

            try
            {
                Assembly resAssem = option.CallerAssembly;
                Stream result = resAssem.GetManifestResourceStream(option.ResourceName);
                if (result != null) return result;
                else
                {
                    // not found
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Write Debug message
                ex.Err(med);
                if (this.ShowException)
                    throw (ex);
                else return null;
            }
        }

        #endregion

        #region GetImage

        /// <summary>
        /// Get Stream by specificed option.
        /// </summary>
        /// <param name="option">The resouce stream option.</param>
        /// <returns>Return Null if not found and Return Target Resource Stream is found</returns>
        public override Image GetImage(ResourceStreamOptions option)
        {
            if (option == null ||
                null == option.CallerType ||
                null == option.CallerAssembly)
            {
                return null;
            }
            Stream stream = GetStream(option);
            Image result = null;
            MethodBase med = MethodBase.GetCurrentMethod();

            if (stream != null)
            {
                try
                {
                    result = Image.FromStream(stream);
                }
                catch (Exception ex)
                {
                    // Write Debug message
                    ex.Err(med);
                    if (this.ShowException)
                        throw (ex);
                    else return null;
                }
            }
            stream = null;

            return result;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets assembly that contains resource.
        /// </summary>
        public Assembly Assembly
        {
            get { return _resAssem; }
            set
            {
                if (_resAssem != value)
                {
                    _resAssem = value;
                }
            }
        }

        #endregion
    }

    #endregion

    #region WPF

    /// <summary>
    /// WPFResourceAccess class.
    /// </summary>
    public class WPFResourceAccess : ResourceAccess
    {
        #region Internal Variables

        private Assembly _resAssem = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public WPFResourceAccess() : base() { }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~WPFResourceAccess()
        {
            _resAssem = null;
        }

        #endregion

        #region Overrides


        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets assembly that contains resource.
        /// </summary>
        public Assembly Assembly
        {
            get { return _resAssem; }
            set
            {
                if (_resAssem != value)
                {
                    _resAssem = value;
                }
            }
        }

        #endregion
    }

    #endregion
}
