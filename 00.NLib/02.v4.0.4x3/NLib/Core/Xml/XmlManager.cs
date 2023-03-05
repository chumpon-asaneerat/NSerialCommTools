#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-16
=================
- Xml Manager updated
  - use ApplicationManager Invoke instead of delegate extension method.

======================================================================================================================
Update 2013-07-12
=================
- Xml Manager updated
  - Remove XmlProcInstruction and XmlFileOption class.
  - Change all result of Load/Save xml to NResult<T>.
  - Improve sightly performance a little by add some checking process to detected error
    and return immediately with run all code.

======================================================================================================================
Update 2013-05-22
=================
- Xml Manager updated
  - Add XmlProcInstruction and XmlFileOption class.

======================================================================================================================
Update 2013-05-07
=================
- Xml Manager updated
  - XmlSetting and related classes is temporary removed.

======================================================================================================================
Update 2013-02-22
=================
- Xml Manager updated
  - Supports non generic load and save to file.

======================================================================================================================
Update 2012-12-25
=================
- Xml Setting changed code.
  - XmlSetting class now supports Key that is case in-sensitive.

======================================================================================================================
Update 2012-12-19
=================
- Xml Manager changed code.
  - Split Error Event to SaveError and OpenError event.

======================================================================================================================
Update 2011-11-07
=================
- XmlFile ported from GFA40 to NLib.
  - Removed all code related to ExceptionMode.
  - Removed all code that used OpenFileDialog and SaveFileDialog.
  - Removed all code related to path and auto file name in XmlSettingManager.

======================================================================================================================
Update 2010-08-31
=================
- XmlFile ported from GFA38v3 to GFA40 and GFA20.

======================================================================================================================
Update 2010-02-01
=================
- Xml Setting Manager ported code from GFA37 to GFA38v3 for compatibility.

======================================================================================================================
Update 2010-01-23
=================
- Xml Manager updated code.
  - XmlManager class updated with new XmlFile class.

======================================================================================================================
Update 2009-12-26
=================
- Xml Manager update code.
  - XmlManager class and XmlFile class's property DisableLog is re-enable.

======================================================================================================================
Update 2009-12-25
=================
- Xml Manager ported code.
  - XmlManagerExceptionHandler and XmlManagerExceptionArgs class is obsolated.
  - XmlExceptionEventHandler and XmlExceptionEventArgs class is added and replace
    to XmlManagerExceptionHandler and XmlManagerExceptionArgs class.
  - XmlManager class rewrite in C# 2.0 language spec.
  - XmlManager class is now wrap around XmlFile class to make it's compatible with XmlManager
    in GFA37.
  - XmlManager class's property ExceptionMode is now supports new 
    Ignore enum value.
  - XmlManager class's Error event is changed.
  - XmlManager class's property ShowException is obsolate.
  - XmlManager class's property DisableLog is temporary obsolate.

======================================================================================================================
Update 2009-08-23
=================
- Xml Serialization
  - Add GdiCharset property in FontSerializer.

======================================================================================================================
Update 2009-07-14
=================
- Xml Setting Manager fixed path issue in vista.

======================================================================================================================
Update 2008-11-26
=================
- Xml Manager move from GFA.Lib to GFA.Lib.Core.

======================================================================================================================
Update 2008-10-15
=================
- Xml Manager
  - Add code to prevent dead lock between XmlManager and LogManager.

======================================================================================================================
Update 2008-10-06
=================
- Xml Manager
  - Add Retry check in Save/Load xml file.
  - Add more exception handler in Save/Load method.
  - Add more log to keep error tracking (internal used).
  - Add security permission.

======================================================================================================================
Update 2008-04-13
=================
- Xml Manager
  - Add New property ExceptionMode and Error event.
  - Add new Class and Delegate XmlManagerEventHandler/XmlManagerEventArgs.
  - Collection Serializer class is added. but not tested.
  - Test with CollectionBase/ArrayList and List<T>
    Note. when implement CollectionBase we should implement Indexer Access like 
      
      public interface ITest { }

      [Serializable]
      public abstract class XXX : ITest
      {
      } 

      public class YYY : XXX {}
      public class ZZZ : XXX {}

      public class XXXCollection : CollectionBase
      {
         ...
         public XXX this[int index] { get { return (XXX)List[index]; } }
         ...
      }

      where XXX is abstract class or normal class.

      because the XmlSerialization will used the indexer access to test type of object in list (via ICollection).
      And if need to serialize interface we should create abstract class and implement the interface and used that
      abstract class to implement collection like above example.

======================================================================================================================
Update 2007-11-26
=================
- Xml Manager and Serialization support easy to create xml from class via .NET Xml serialization
  with wrapper class that handle all exception internal (log exception information in log system).
  see XmlManager

======================================================================================================================
// </[History]>

#endif
#endregion

#define USE_APP_EXTENSIONS

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Permissions;

#endregion

namespace NLib.Xml
{
    #region Xml Manager

    /// <summary>
    /// XmlManager class. Wrapper class for handle XML Serialization for Custom Object
    /// see XmlFile class for more information.
    /// </summary>
    //[FileIOPermission(SecurityAction.Demand, Unrestricted = true)]
    public sealed class XmlManager
    {
        #region Internal variable

        private static XmlFile _xml = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private XmlManager() : base() { }

        #endregion

        #region Private Method

        #region CheckInstance

        private static void CheckInstance()
        {
            if (null == _xml)
            {
                lock (typeof(XmlManager))
                {
                    _xml = new XmlFile();
                    _xml.OpenFileError += new XmlExceptionEventHandler(_xml_OpenFileError);
                    _xml.SaveFileError += new XmlExceptionEventHandler(_xml_SaveFileError);
                }
            }
        }

        #endregion

        #region Xml File Event Handler

        static void _xml_OpenFileError(object sender, XmlExceptionEventArgs e)
        {
            if (OpenFileError != null)
            {
                // raise event by direct pass event parameters.
#if !USE_APP_EXTENSIONS
                OpenFileError.Call(sender, e);
#else
                ApplicationManager.Instance.Invoke(OpenFileError,
                    sender, e);
#endif
            }
        }

        static void _xml_SaveFileError(object sender, XmlExceptionEventArgs e)
        {
            if (SaveFileError != null)
            {
                // raise event by direct pass event parameters.
#if !USE_APP_EXTENSIONS
                SaveFileError.Call(sender, e);
#else
                ApplicationManager.Instance.Invoke(SaveFileError,
                    sender, e);
#endif
            }
        }

        #endregion

        #endregion

        #region Public Method

        /// <summary>
        /// Save Object to XML File with specificed filename (auto backup when processing).
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="FileName">FileName to Save</param>
        /// <param name="value">The object instance to save</param>
        /// <returns>Return NResult with result is true if Save is completed</returns>
        public static NResult<bool> SaveToFile<T>(string FileName, T value)
        {
            CheckInstance();
            return _xml.SaveToFile<T>(FileName, value);
        }
        /// <summary>
        /// Save Object to XML File with specificed filename (auto backup when processing).
        /// </summary>
        /// <param name="FileName">FileName to Save</param>
        /// <param name="objectType">The target object type.</param>
        /// <param name="value">The object instance to save</param>
        /// <returns>Return NResult with result is true if Save is completed</returns>
        public static NResult<bool> SaveToFile(string FileName, Type objectType, object value)
        {
            CheckInstance();
            return _xml.SaveToFile(FileName, objectType, value);
        }
        /// <summary>
        /// Load Object Instance from Specificed FileName.
        /// </summary>
        /// <typeparam name="T">The target object type.</typeparam>
        /// <param name="FileName">The target file name to load.</param>
        /// <returns>Return NResult of T that loaded from file</returns>
        public static NResult<T> LoadFromFile<T>(string FileName)
        {
            CheckInstance();
            return _xml.LoadFromFile<T>(FileName);
        }
        /// <summary>
        /// Load Object Instance from Specificed FileName.
        /// </summary>
        /// <param name="FileName">The target file name to load.</param>
        /// <param name="objectType">The target object type.</param>
        /// <returns>Return NResult of object that loaded from file</returns>
        public static NResult<object> LoadFromFile(string FileName, Type objectType)
        {
            CheckInstance();
            return _xml.LoadFromFile(FileName, objectType);
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets or sets Exrta Types used to serialize and deserizlize in collection
        /// </summary>
        [Category("Xml")]
        [Description("Gets or sets Exrta Types used to serialize and deserizlize in collection")]
        public static Type[] ExtraTypes
        {
            get
            {
                CheckInstance();
                return _xml.ExtraTypes;
            }
            set
            {
                CheckInstance();
                _xml.ExtraTypes = value;
            }
        }
        /// <summary>
        /// Gets or sets the option to disable to write exception to Debug manager.
        /// </summary>
        [Category("Xml")]
        [Description("Gets or sets the option to disable to write exception to Debug manager.")]
        public static bool DisableDebug
        {
            get
            {
                CheckInstance();
                return _xml.DisableDebug;
            }
            set
            {
                CheckInstance();
                _xml.DisableDebug = value;
            }
        }
        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        [Category("Xml")]
        [Description("Gets or sets the encoding.")]
        public static Encoding Encoding
        {
            get
            {
                CheckInstance();
                return _xml.Encoding;
            }
            set
            {
                CheckInstance();
                _xml.Encoding = value;
            }
        }

        #endregion

        #region Static events

        /// <summary>
        /// The OpenFileError event. Occur when load operation is detected exception.
        /// </summary>
        [Category("Xml")]
        [Description("The OpenFileError event. Occur when load operation is detected exception.")]
        public static event XmlExceptionEventHandler OpenFileError;
        /// <summary>
        /// The SaveFileError event. Occur when save operation is detected exception.
        /// </summary>
        [Category("Xml")]
        [Description("The SaveFileError event. Occur when save operation is detected exception.")]
        public static event XmlExceptionEventHandler SaveFileError;

        #endregion
    }

    #endregion
}
