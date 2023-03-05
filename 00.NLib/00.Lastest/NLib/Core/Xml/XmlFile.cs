#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-16
=================
- XmlFile updated
  - use string format instead of string extension method.
  - use ApplicationManager Invoke instead of delegate extension method.

======================================================================================================================
Update 2014-10-14
=================
- XmlFile updated
  - Add Code to Grant ACL when create directory.

======================================================================================================================
Update 2013-07-12
=================
- XmlFile updated
  - Remove XmlProcInstruction and XmlFileOption class.
  - Change all result of Load/Save xml to NResult<T>.
  - Improve sightly performance a little by add some checking process to detected error
    and return immediately with run all code.

======================================================================================================================
Update 2013-05-22
=================
- XmlFile updated
  - Add XmlProcInstruction and XmlFileOption class.

======================================================================================================================
Update 2013-04-22
=================
- XmlFile rework
  - Redesign XmlFile class.

======================================================================================================================
Update 2013-03-21
=================
- XmlFile updated
  - SaveToFile and SaveToFile<T> remove check for the default encoding so now
    when save the namespace is removed.

======================================================================================================================
Update 2013-02-22
=================
- XmlFile updated
  - Supports non generic load and save to file.

======================================================================================================================
Update 2012-12-19
=================
- XmlFile changed
  - Remove XmlOps enum.
  - Changed default encoding to UTF8.
  - Change call DebugManager call to Extension method call.
  - Split Error Event to SaveError and OpenError event.

======================================================================================================================
Update 2011-12-15
=================
- XmlFile updated 
  - Remove all ExceptionManager related code and used DebugManager instread.

======================================================================================================================
Update 2011-11-07
=================
- XmlFile port from GFA4.0 to NLib
  - Remove rarely used methods.
  - ExceptionMode is ignore and always raise Error event.
  - All serialization classes is not ported.

======================================================================================================================
Update 2011-08-31
=================
- XmlFile omit namespace.

======================================================================================================================
Update 2011-07-26
=================
- XmlManager and XmlFile add encoding supports.

======================================================================================================================
Update 2010-08-31
=================
- XmlManager and related classes ported from GFA38v3 to GFA40 and GFA20.

======================================================================================================================
Update 2010-01-23
=================
- XmlFile code updated.
  - XmlFile class is update code to used ExceptionManager instrad LogManager.
  - XmlFile class changed property name from EnableLog to EnableDebug.

======================================================================================================================
Update 2009-12-26
=================
- Xml Serialization classes ported.
  - Serializer class ported.
  - ColorSerializer class ported.
  - FontSerializer class ported.
  - TypeSerializer class ported.
  - ImageSerializer class ported.
- XmlFile update code.
  - XmlFile class's property DisableLog is re-enable.

======================================================================================================================
Update 2009-12-25
=================
- Xml File classes added.
  - XmlFile class added. This class is acts like the XmlManager class in GFA37 but not a
    static class. This class is used for general purpose for save/load Xml serialization
    and not has problem like XmlManager in GFA37 because it's not static access so we
    can freely used it in multithread manner to access multiple Xml files at same time.
  - XmlFile class's Error event is changed.
  - XmlFile class's property ShowException is obsolate.
  - XmlFile class's property DisableLog is temporary obsolate.

======================================================================================================================
// </[History]>

#endif
#endregion

//#define ENABLE_LOGS
#define USE_APP_EXTENSIONS

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using NLib;
using NLib.IO;
using NLib.Reflection;

#endregion

namespace NLib.Xml
{
    #region Xml Exception EventHandler and EventArgs

    /// <summary>
    /// Xml Exception Event Handler
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">An Xml Exception Event Args.</param>
    public delegate void XmlExceptionEventHandler(object sender, XmlExceptionEventArgs e);
    /// <summary>
    /// Xml Exception Exception Args
    /// </summary>
    public class XmlExceptionEventArgs
    {
        #region Internal Variable

        private Exception _ex = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Construcctor
        /// </summary>
        private XmlExceptionEventArgs()
            : base() { }
        /// <summary>
        /// Construcctor
        /// </summary>
        /// <param name="ex">An Exception instance.</param>
        public XmlExceptionEventArgs(Exception ex)
            : base()
        {
            _ex = ex;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~XmlExceptionEventArgs()
        {
            GC.SuppressFinalize(this);
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get Exception instance.
        /// </summary>
        [Category("Xml")]
        [Description("Get Exception instance.")]
        public Exception Exception { get { return _ex; } }

        #endregion
    }

    #endregion

    #region XmlFile

    /// <summary>
    /// XmlFile class. Wrapper class for handle XML Serialization for Custom Object
    /// </summary>
    //[FileIOPermission(SecurityAction.Demand, Unrestricted = true)]
    public class XmlFile
    {
        #region Internal variable

        private Type[] _extraType = Type.EmptyTypes;
        private bool _disableLog = false;
        private Encoding _encode = Encoding.UTF8;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public XmlFile()
            : base()
        {
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~XmlFile()
        {
        }

        #endregion

        #region Private Methods

        private void RaiseSaveError(Exception ex)
        {
            if (null != SaveFileError && null != ex)
            {
#if !USE_APP_EXTENSIONS
                SaveFileError.Call(this, new XmlExceptionEventArgs(ex));
#else
                ApplicationManager.Instance.Invoke(SaveFileError, 
                    this, new XmlExceptionEventArgs(ex));
#endif
            }
        }

        private void RaiseOpenError(Exception ex)
        {
            if (null != OpenFileError && null != ex)
            {
#if !USE_APP_EXTENSIONS
                OpenFileError.Call(this, new XmlExceptionEventArgs(ex));
#else
                ApplicationManager.Instance.Invoke(OpenFileError, 
                    this, new XmlExceptionEventArgs(ex));
#endif
            }
        }

        #endregion

        #region Public Method

        #region Save To File - Generic version

        /// <summary>
        /// Save Object to XML File with specificed filename (auto backup when processing).
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="FileName">FileName to Save</param>
        /// <param name="value">The object instance to save</param>
        /// <returns>Return NResult with result is true if Save is completed</returns>
        public NResult<bool> SaveToFile<T>(string FileName, T value)
        {
            #region Local vars

            MethodBase med = MethodBase.GetCurrentMethod();

            NResult<bool> inst = new NResult<bool>();
            inst.Result = false;

            bool isPathOK = false;
            bool isFileOK = false;

            string tmpName = FileName + ".tmp";
            string tmpPath = System.IO.Path.GetDirectoryName(FileName);

            #endregion

            #region Check path and create if required.

            if (!System.IO.Directory.Exists(tmpPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(tmpPath);
                    if (System.IO.Directory.Exists(tmpPath))
                    {
                        // Grant Permission when create
                        Folders.Grant(tmpPath);
                    }
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Detected create directory error.";
                        msg.Err();
                        ex.Err();
#else
                        if (null != ex) { Console.WriteLine(ex); }
#endif
                    }

                    #endregion
                }
            }

            if (!System.IO.Directory.Exists(tmpPath))
            {
                inst.Err = new Exception("Cannot create temp path.");
                inst.Result = false;
                return inst;
            }

            #endregion

            #region Prepare backup file and recheck condition

            // Copy original file to tmp and remove the original
            if (File.Exists(FileName))
            {
                #region Copy original file to tmp

                try
                {
                    // use simple copy method because temp file is create by
                    // current user so it's should has proper permission.
                    File.Copy(FileName, tmpName, true);
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Detected create temp file error.";
                        msg.Err();
                        ex.Err();
#endif
                    }

                    #endregion

                    // Set error
                    inst.Err = ex;
                    inst.Result = false;
                    return inst;
                }

                #endregion

                #region Delete original file

                try
                {
                    File.Delete(FileName);
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Detected delete file error.";
                        msg.Err();
                        ex.Err();
#endif
                    }

                    #endregion

                    // Set error
                    inst.Err = ex;
                    inst.Result = false;
                    return inst;
                }

                #endregion
            }

            // recheck path and file.
            isPathOK = System.IO.Directory.Exists(tmpPath);
            isFileOK = isPathOK && !File.Exists(FileName);

            #endregion

            #region Create File Stream

            Stream stm = null;
            int iRetry = 0;
            int maxRetry = 5;

            // retry 3 times.
            while (null == stm && iRetry < maxRetry)
            {
                try
                {
                    stm = new FileStream(FileName,
                        FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        "Detected create stream error. Retry No. {0}.".Err(iRetry + 1);
                        ex.Err();
#endif
                    }

                    #endregion

                    #region Reset variable and increase retry counter

                    // File is lock access for some reason.
                    stm = null; // set to null
                    ++iRetry;
                    // sleep a little
#if !USE_APP_EXTENSIONS
                    this.Sleep(50);
#else
                    ApplicationManager.Instance.Sleep(50);
#endif
                    #endregion

                    // Set error
                    inst.Err = ex;
                    inst.Result = false;

                    #region Throw/Raise error

                    if (iRetry >= maxRetry)
                    {
                        // raise event
                        RaiseSaveError(ex);
                    }

                    #endregion
                }
            }

            #endregion

            #region Write data to stream

            if (stm != null)
            {
                try
                {
                    // replace result from Save to stream
                    inst = SaveToFile<T>(stm, value);
                    stm.Flush();
                    stm.Close();
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Detected error when save stream.";
                        msg.Err();
                        ex.Err();
#endif
                    }

                    #endregion

                    // Set error
                    inst.Err = ex;
                    inst.Result = false;

                    #region Throw/Raise error

                    // raise event
                    RaiseSaveError(ex);

                    #endregion
                }
                stm = null;
            }

            #endregion

            #region Clear unused file

            if (inst.Result && File.Exists(tmpName))
            {
                #region Clear Tempory File if new file is successfully created

                try
                {
                    File.Delete(tmpName);
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Save success but delete temp file detected error.";
                        msg.Err();
                        ex.Err();
#else
                        if (null != ex) { Console.WriteLine(ex); }
#endif
                    }

                    #endregion
                }

                #endregion
            }
            else if (!inst.Result && File.Exists(tmpName))
            {
                #region Copy Tempory File back to original if create new file is failed

                #region Copy Temp file to target file

                try
                {
                    File.Copy(tmpName, FileName, true);
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Save failed and restore file from temp detected error.";
                        msg.Err();
                        ex.Err();
#else
                        if (null != ex) { Console.WriteLine(ex); }
#endif
                    }

                    #endregion
                }

                #endregion

                #region Delete temp file

                try
                {
                    File.Delete(tmpName);
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Save failed and delete temp file detected error.";
                        msg.Err();
                        ex.Err();
#else
                        if (null != ex) { Console.WriteLine(ex); }
#endif
                    }

                    #endregion
                }

                #endregion

                #endregion
            }

            #endregion

            return inst;
        }
        /// <summary>
        /// Save Object to File Stream.
        /// </summary>
        /// <typeparam name="T">The target object type.</typeparam>
        /// <param name="s">The Stream to Save.</param>
        /// <param name="value">The Object instance to Save.</param>
        /// <returns>Return NResult with result is true if Save is completed</returns>
        public NResult<bool> SaveToFile<T>(Stream s, T value)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            NResult<bool> inst = new NResult<bool>();
            inst.Result = false;

            XmlSerializer sr = null;

            #region Check parameters

            if (null == s)
            {
                inst.Err = new Exception("stream is null.");
                inst.Result = false;
                return inst;
            }

            if (null == value)
            {
                #region Throw/Raise error

                // raise event
                RaiseSaveError(new Exception("object is null"));

                #endregion

                inst.Err = new Exception("object to save is null.");
                inst.Result = false;
                return inst;
            }

            #endregion

            try
            {
                #region Serializing

                if (null == _extraType || Type.EmptyTypes == _extraType)
                {
                    // no extra types
                    sr = new XmlSerializer(typeof(T));
                }
                else
                {
                    // has extra types
                    sr = new XmlSerializer(typeof(T), _extraType);
                }

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = (null != _encode) ? _encode : Encoding.Default;
                settings.Indent = true;
                XmlWriter writer = XmlWriter.Create(s, settings);

                XmlSerializerNamespaces ns = null;

                ns = new XmlSerializerNamespaces();
                // note: for last check 2013-05-22 the below line is declare
                // not allow by microsoft but then test is seem still work
                ns.Add(string.Empty, string.Empty); // omit namespace

                sr.Serialize(writer, value, ns);

                try { writer.Close(); }
                catch { }
                writer = null;

                inst.Err = null;
                inst.Result = true;

                #endregion
            }
            catch (Exception ex)
            {
                #region Write Debug

                if (!DisableDebug)
                {
#if ENABLE_LOGS
                    string msg = "Detected stream serialize error.";
                    msg.Err();
                    ex.Err();
#endif
                }

                #endregion

                // set error
                inst.Err = ex;
                inst.Result = false;

                #region Throw/Raise error

                // raise event
                RaiseSaveError(ex);

                #endregion
            }
            finally
            {
                if (null != sr)
                {
                }
                sr = null;
            }

            return inst;
        }

        #endregion

        #region Save To File - Non Generic version

        /// <summary>
        /// Save Object to XML File with specificed filename (auto backup when processing).
        /// </summary>
        /// <param name="FileName">FileName to Save</param>
        /// <param name="objectType">The object instance to serialized.</param>
        /// <param name="value">The object instance to save</param>
        /// <returns>Return NResult with result is true if Save is completed</returns>
        public NResult<bool> SaveToFile(string FileName, Type objectType, object value)
        {
            #region Local vars

            MethodBase med = MethodBase.GetCurrentMethod();

            NResult<bool> inst = new NResult<bool>();
            inst.Result = false;

            bool isPathOK = false;
            bool isFileOK = false;

            string tmpName = FileName + ".tmp";
            string tmpPath = System.IO.Path.GetDirectoryName(FileName);

            #endregion

            #region Check path and create if required.

            if (!System.IO.Directory.Exists(tmpPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(tmpPath);
                    if (System.IO.Directory.Exists(tmpPath))
                    {
                        // Grant Permission when create
                        Folders.Grant(tmpPath);
                    }
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Detected create directory error.";
                        msg.Err();
                        ex.Err();
#else
                        if (null != ex) { Console.WriteLine(ex); }
#endif
                    }

                    #endregion
                }
            }

            if (!System.IO.Directory.Exists(tmpPath))
            {
                inst.Err = new Exception("Cannot create temp path.");
                inst.Result = false;
                return inst;
            }

            #endregion

            #region Prepare backup file and recheck condition

            // Copy original file to tmp and remove the original
            if (File.Exists(FileName))
            {
                #region Copy original file to tmp

                try
                {
                    // use simple copy method because temp file is create by
                    // current user so it's should has proper permission.
                    File.Copy(FileName, tmpName, true);
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Detected create temp file error.";
                        msg.Err();
                        ex.Err();
#endif
                    }

                    #endregion

                    // Set error
                    inst.Err = ex;
                    inst.Result = false;
                    return inst;
                }

                #endregion

                #region Delete original file

                try
                {
                    File.Delete(FileName);
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Detected delete file error.";
                        msg.Err();
                        ex.Err();
#endif
                    }

                    #endregion

                    // Set error
                    inst.Err = ex;
                    inst.Result = false;
                    return inst;
                }

                #endregion
            }

            // recheck path and file.
            isPathOK = System.IO.Directory.Exists(tmpPath);
            isFileOK = isPathOK && !File.Exists(FileName);

            #endregion

            #region Create File Stream

            Stream stm = null;
            int iRetry = 0;
            int maxRetry = 5;

            // retry 3 times.
            while (null == stm && iRetry < maxRetry)
            {
                try
                {
                    stm = new FileStream(FileName,
                        FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        "Detected create stream error. Retry No. {0}.".Err(iRetry + 1);
                        ex.Err();
#endif
                    }

                    #endregion

                    #region Reset variable and increase retry counter

                    // File is lock access for some reason.
                    stm = null; // set to null
                    ++iRetry;
                    // sleep a little
#if !USE_APP_EXTENSIONS
                    this.Sleep(50);
#else
                    ApplicationManager.Instance.Sleep(50);
#endif

                    #endregion

                    // Set error
                    inst.Err = ex;
                    inst.Result = false;

                    #region Throw/Raise error

                    if (iRetry >= maxRetry)
                    {
                        // raise event
                        RaiseSaveError(ex);
                    }

                    #endregion
                }
            }

            #endregion

            #region Write data to stream

            if (stm != null)
            {
                try
                {
                    // replace result from Save to stream
                    inst = SaveToFile(stm, objectType, value);
                    stm.Flush();
                    stm.Close();
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Detected error when save stream.";
                        msg.Err();
                        ex.Err();
#endif
                    }

                    #endregion

                    // Set error
                    inst.Err = ex;
                    inst.Result = false;

                    #region Throw/Raise error

                    // raise event
                    RaiseSaveError(ex);

                    #endregion
                }
                stm = null;
            }

            #endregion

            #region Clear unused file

            if (inst.Result && File.Exists(tmpName))
            {
                #region Clear Tempory File if new file is successfully created

                try
                {
                    File.Delete(tmpName);
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Save success but delete temp file detected error.";
                        msg.Err();
                        ex.Err();
#else
                        if (null != ex) { Console.WriteLine(ex); }
#endif
                    }

                    #endregion
                }

                #endregion
            }
            else if (!inst.Result && File.Exists(tmpName))
            {
                #region Copy Tempory File back to original if create new file is failed

                #region Copy Temp file to target file

                try
                {
                    File.Copy(tmpName, FileName, true);
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Save failed and restore file from temp detected error.";
                        msg.Err();
                        ex.Err();
#else
                        if (null != ex) { Console.WriteLine(ex); }
#endif
                    }

                    #endregion
                }

                #endregion

                #region Delete temp file

                try
                {
                    File.Delete(tmpName);
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Save failed and delete temp file detected error.";
                        msg.Err();
                        ex.Err();
#else
                        if (null != ex) { Console.WriteLine(ex); }
#endif
                    }

                    #endregion
                }

                #endregion

                #endregion
            }

            #endregion

            return inst;
        }
        /// <summary>
        /// Save Object to File Stream.
        /// </summary>
        /// <param name="s">The Stream to Save.</param>
        /// <param name="objectType">The target object type.</param>
        /// <param name="value">The object instance to serialized.</param>
        /// <returns>Return NResult with result is true if Save is completed</returns>
        public NResult<bool> SaveToFile(Stream s, Type objectType, object value)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            NResult<bool> inst = new NResult<bool>();
            inst.Result = false;

            XmlSerializer sr = null;

            #region Check parameters

            if (null == s)
            {
                inst.Err = new Exception("Stream is null.");
                inst.Result = false;
                return inst;
            }

            if (null == value)
            {
                #region Throw/Raise error

                // raise event
                RaiseSaveError(new Exception("object is null"));

                #endregion

                inst.Err = new Exception("object to save is null.");
                inst.Result = false;
                return inst;
            }

            if (value.GetType() != objectType)
            {
                #region Throw/Raise error

                // raise event
                RaiseSaveError(new Exception("object is type is not match."));

                #endregion

                inst.Err = new Exception("object to save is mismatch type.");
                inst.Result = false;
                return inst;
            }

            #endregion

            try
            {
                #region Serializing

                if (null == _extraType || Type.EmptyTypes == _extraType)
                {
                    // no extra types
                    sr = new XmlSerializer(value.GetType());
                }
                else
                {
                    // has extra types
                    sr = new XmlSerializer(value.GetType(), _extraType);
                }

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = (null != _encode) ? _encode : Encoding.Default;
                settings.Indent = true;
                XmlWriter writer = XmlWriter.Create(s, settings);

                XmlSerializerNamespaces ns = null;

                ns = new XmlSerializerNamespaces();
                // note: for last check 2013-05-22 the below line is declare
                // not allow by microsoft but then test is seem still work
                ns.Add(string.Empty, string.Empty); // omit namespace

                sr.Serialize(writer, value, ns);

                try { writer.Close(); }
                catch { }
                writer = null;

                // success
                inst.Err = null;
                inst.Result = true;

                #endregion
            }
            catch (Exception ex)
            {
                #region Write Debug

                if (!DisableDebug)
                {
#if ENABLE_LOGS
                    string msg = "Detected stream serialize error.";
                    msg.Err();
                    ex.Err();
#endif
                }

                #endregion

                // set error
                inst.Err = ex;
                inst.Result = false;

                #region Throw/Raise error

                // raise event
                RaiseSaveError(ex);

                #endregion
            }
            finally
            {
                if (null != sr)
                {
                }
                sr = null;
            }

            return inst;
        }

        #endregion

        #region Load From File - Generic version

        /// <summary>
        /// Load Object Instance from Specificed FileName.
        /// </summary>
        /// <typeparam name="T">The target object type.</typeparam>
        /// <param name="FileName">The target file name to load.</param>
        /// <returns>Return NResult of T that loaded from file</returns>
        public NResult<T> LoadFromFile<T>(string FileName)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            NResult<T> inst = new NResult<T>();
            inst.Result = default(T);

            Type objectType = typeof(T);

            if (!File.Exists(FileName))
            {
#if ENABLE_LOGS
                "File not found. {0}".Err(FileName);
#endif
                // create exception
                inst.Err = new FileNotFoundException(
                    string.Format("File not found. {0}", FileName));
                return inst;
            }

            #region Local vars

            Stream stm = null;
            int iRetry = 0;
            int maxRetry = 5;

            #endregion

            #region Try to open file stream

            while (null == stm && iRetry < maxRetry)
            {
                try
                {
                    stm = new FileStream(FileName, FileMode.Open, FileAccess.Read,
                        FileShare.Read);
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        "Detected stream create error. Retry No. {0}.".Err(iRetry + 1);
                        ex.Err();
#endif
                    }

                    #endregion

                    #region Reset variable and increase retry counter

                    stm = null;
                    ++iRetry;
                    // sleep a little
#if !USE_APP_EXTENSIONS
                    this.Sleep(50);
#else
                    ApplicationManager.Instance.Sleep(50);
#endif

                    #endregion

                    #region Set exception

                    // set error
                    inst.Err = ex;
                    inst.Result = default(T);

                    #endregion

                    #region Throw/Raise error

                    // Raise Event
                    RaiseOpenError(ex);

                    #endregion
                }
            }

            #endregion

            #region Load instance from stream

            if (null != stm)
            {
                // create stream success
                inst.Err = null;
                inst.Result = default(T);

                #region Load instance

                try
                {
                    // replace result instance from load from (stream)
                    inst = LoadFromFile<T>(stm);
                }
                catch (Exception ex)
                {
                    #region Write log

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Detected error during load from stream.";
                        msg.Err();
                        ex.Err();
#endif
                    }

                    #endregion

                    // set error
                    inst.Err = ex;
                    inst.Result = default(T); // something error set as default

                    #region Throw/Raise error

                    // Raise Event
                    RaiseOpenError(ex);

                    #endregion
                }

                #endregion

                #region Close stream

                try
                {
                    stm.Close();
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Detected error during close stream.";
                        msg.Err();
                        ex.Err();
#else
                        if (null != ex) { Console.WriteLine(ex); }
#endif
                    }

                    #endregion
                }

                #endregion

                stm = null;
            }

            #endregion

            return inst;
        }
        /// <summary>
        /// Load Object From Stream.
        /// </summary>
        /// <typeparam name="T">The target object type.</typeparam>
        /// <param name="s">The Stream to load object.</param>
        /// <returns>Return NResult of T that loaded from stream.</returns>
        public NResult<T> LoadFromFile<T>(Stream s)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            NResult<T> inst = new NResult<T>();
            inst.Result = default(T);

            Type objectType = typeof(T);
            object obj;
            XmlSerializer sr = null;
            try
            {
                if (null == s)
                {
                    inst.Err = new Exception("Stream is null.");
                    return inst;
                }

                if (null == objectType)
                {
                    #region Throw/Raise error

                    // Raise Event
                    RaiseOpenError(new Exception("Target Type is null"));

                    #endregion
                }

                #region Deserialize and init instance

                if (_extraType == null || _extraType == Type.EmptyTypes)
                {
                    // no extra type
                    sr = new XmlSerializer(objectType);
                }
                else
                {
                    // has extra types
                    sr = new XmlSerializer(objectType, _extraType);
                }

                obj = sr.Deserialize(s);
                if (null != obj)
                {
                    inst.Result = (T)obj; // cast type
                }
                else
                {
                    inst.Err = null;
                    inst.Result = default(T); // load null
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Write log

                if (!DisableDebug)
                {
#if ENABLE_LOGS
                    string msg = "Detected stream deserialize error.";
                    msg.Err();
                    ex.Err();
#endif
                }

                #endregion

                // set error
                inst.Err = ex;
                inst.Result = default(T); // reset instance to default value

                #region Throw/Raise error

                // Raise Event
                RaiseOpenError(ex);

                #endregion
            }
            finally
            {
                if (null != sr)
                {
                }
                sr = null;
            }

            return inst;
        }

        #endregion

        #region Load From File - Non Generic version

        /// <summary>
        /// Load Object Instance from Specificed FileName.
        /// </summary>
        /// <param name="FileName">The target file name to load.</param>
        /// <param name="objectType">The target object type.</param>
        /// <returns>Return NResult of object that loaded from file.</returns>
        public NResult<object> LoadFromFile(string FileName, Type objectType)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            NResult<object> inst = new NResult<object>();
            inst.Result = null;

            if (!File.Exists(FileName))
            {
#if ENABLE_LOGS
                "File not found. {0}".Err(FileName);
#endif
                // create exception
                inst.Err = new FileNotFoundException(
                    string.Format("File not found. {0}", FileName));
                return inst;
            }

            #region Local vars

            Stream stm = null;
            int iRetry = 0;
            int maxRetry = 5;

            #endregion

            #region Try to open file stream

            while (null == stm && iRetry < maxRetry)
            {
                try
                {
                    stm = new FileStream(FileName, FileMode.Open, FileAccess.Read,
                        FileShare.Read);
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        "Detected stream create error. Retry No. {0}.".Err(iRetry + 1);
                        ex.Err();
#endif
                    }

                    #endregion

                    #region Reset variable and increase retry counter

                    stm = null;
                    ++iRetry;
                    // sleep a little
#if !USE_APP_EXTENSIONS
                    this.Sleep(50);
#else
                    ApplicationManager.Instance.Sleep(50);
#endif

                    #endregion

                    #region Set exception

                    // set error
                    inst.Err = ex;
                    inst.Result = null;

                    #endregion

                    #region Throw/Raise error

                    // Raise Event
                    RaiseOpenError(ex);

                    #endregion
                }
            }

            #endregion

            #region Load instance from stream

            if (null != stm)
            {
                // create stream success
                inst.Err = null;
                inst.Result = null;

                #region Load instance

                try
                {
                    // replace result instance from load from (stream)
                    inst = LoadFromFile(stm, objectType);
                }
                catch (Exception ex)
                {
                    #region Write log

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Detected error during load from stream.";
                        msg.Err();
                        ex.Err();
#endif
                    }

                    #endregion

                    // Set error
                    inst.Err = ex;
                    inst.Result = null; // something error set as null

                    #region Throw/Raise error

                    // Raise Event
                    RaiseOpenError(ex);

                    #endregion
                }

                #endregion

                #region Close stream

                try
                {
                    stm.Close();
                }
                catch (Exception ex)
                {
                    #region Write Debug

                    if (!DisableDebug)
                    {
#if ENABLE_LOGS
                        string msg = "Detected error during close stream.";
                        msg.Err();
                        ex.Err();
#else
                        if (null != ex) { Console.WriteLine(ex); }
#endif
                    }

                    #endregion
                }

                #endregion

                stm = null;
            }

            #endregion

            return inst;
        }
        /// <summary>
        /// Load Object From Stream.
        /// </summary>
        /// <param name="s">The Stream to load object.</param>
        /// <param name="objectType">The target object type.</param>
        /// <returns>Return NResult of object instance from stream.</returns>
        public NResult<object> LoadFromFile(Stream s, Type objectType)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            NResult<object> inst = new NResult<object>();
            inst.Result = null;

            object obj;
            XmlSerializer sr = null;
            try
            {
                if (null == s)
                {
                    inst.Err = new Exception("Stream is null.");
                    return inst;
                }

                if (null == objectType)
                {
                    #region Throw/Raise error

                    // Raise Event
                    RaiseOpenError(new Exception("Target Type is null"));

                    #endregion
                }

                #region Deserialize and init instance

                if (_extraType == null || _extraType == Type.EmptyTypes)
                {
                    // no extra type
                    sr = new XmlSerializer(objectType);
                }
                else
                {
                    // has extra types
                    sr = new XmlSerializer(objectType, _extraType);
                }

                obj = sr.Deserialize(s);
                if (null != obj)
                {
                    inst.Result = obj;
                }
                else inst.Result = null; // load null

                #endregion
            }
            catch (Exception ex)
            {
                #region Write log

                if (!DisableDebug)
                {
#if ENABLE_LOGS
                    string msg = "Detected stream deserialize error.";
                    msg.Err();
                    ex.Err();
#endif
                }

                #endregion

                // set exception
                inst.Err = ex;
                inst.Result = null;

                #region Throw/Raise error

                // Raise Event
                RaiseOpenError(ex);

                #endregion
            }
            finally
            {
                if (null != sr)
                {
                }
                sr = null;
            }

            return inst;
        }

        #endregion

        #endregion

        #region Public Property

        /// <summary>
        /// Gets or sets Exrta Types used to serialize and deserizlize in collection.
        /// </summary>
        [Category("Xml")]
        [Description("Gets or sets Exrta Types used to serialize and deserizlize in collection.")]
        public Type[] ExtraTypes
        {
            get { return _extraType; }
            set { _extraType = value; }
        }
        /// <summary>
        /// Gets or sets the option to disable to write exception to debug manager.
        /// </summary>
        [Category("Xml")]
        [Description("Gets or sets the option to disable to write exception to debug manager.")]
        public bool DisableDebug { get { return _disableLog; } set { _disableLog = value; } }
        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        [Category("Xml")]
        [Description("Gets or sets the encoding.")]
        public Encoding Encoding
        {
            get { return _encode; }
            set
            {
                if (_encode != value)
                {
                    _encode = value;
                    if (null == value)
                    {
                        _encode = Encoding.UTF8;
                    }
                }
            }
        }

        #endregion

        #region Public Event

        /// <summary>
        /// The OpenFileError event. Occur when load operation is detected exception.
        /// </summary>
        [Category("Xml")]
        [Description("The OpenFileError event. Occur when load operation is detected exception.")]
        public event XmlExceptionEventHandler OpenFileError;
        /// <summary>
        /// The SaveFileError event. Occur when save operation is detected exception.
        /// </summary>
        [Category("Xml")]
        [Description("The SaveFileError event. Occur when save operation is detected exception.")]
        public event XmlExceptionEventHandler SaveFileError;

        #endregion
    }

    #endregion
}
