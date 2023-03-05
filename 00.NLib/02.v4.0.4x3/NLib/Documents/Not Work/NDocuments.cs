//#define USE_NDOCUMENT1

#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using NLib;
using NLib.Data;
using NLib.Design;
using NLib.Reflection;
using NLib.Xml;

#endregion

#if USE_NDOCUMENT1
namespace NLib.Documents
{
    #region NXmlContentType
    
    /// <summary>
    /// The NXmlContentType class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public class NXmlContentType
    {
        #region Constructor
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public NXmlContentType()
            : base()
        {
            this.DataType = string.Empty;
            this.AssemblyName = string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets data type in .NET BCL.
        /// </summary>
        [PropertyOrder(1)]
        [XmlAttribute]
        [Category("Types")]
        [Description("Gets or sets data type in .NET BCL.")]
        public string DataType { get; set; }
        /// <summary>
        /// Gets or sets Assembly of specificed DataType.
        /// </summary>
        [PropertyOrder(2)]
        [XmlAttribute]
        [Category("Types")]
        [Description("Gets or sets Assembly of specificed DataType.")]
        public string AssemblyName { get; set; }

        #endregion
    }

    #endregion

    #region NXmlDocumentProperties
    
    /// <summary>
    /// The NXmlDocumentProperties class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public class NXmlDocumentProperties
    {
        #region Constructor
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public NXmlDocumentProperties()
            : base()
        {
            this.DocType = "General";
            this.Version = "1.0";
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets document type.
        /// </summary>
        [PropertyOrder(1)]
        [XmlAttribute]
        [Category("Documents")]
        [Description("Gets or sets document type.")]
        public string DocType { get; set; }
        /// <summary>
        /// Gets or sets document version.
        /// </summary>
        [PropertyOrder(2)]
        [XmlAttribute]
        [Category("Documents")]
        [Description("Gets or sets document version.")]
        public string Version { get; set; }

        #endregion
    }

    #endregion

    #region NXmlContent
    
    /// <summary>
    /// NXmlContent class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    [XmlRoot("Content")]
    public class NXmlContent
    {
        #region Constructor
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public NXmlContent()
            : base()
        {
            this.Type = new NXmlContentType();
            this.XmlData = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets document's content type.
        /// </summary>
        [PropertyOrder(1)]
        [XmlElement]
        [Category("Contents")]
        [Description("Gets or sets document's content type.")]
        public NXmlContentType Type { get; set; }
        /// <summary>
        /// Gets or sets document's content data that keep in xml file.
        /// </summary>
        [PropertyOrder(2)]
        [XmlElement("Data")]
        [Category("Contents")]
        [Description("Gets or sets document's content data that keep in xml file.")]
        [Browsable(false)]
        public object XmlData { get; set; }

        #endregion
    }

    #endregion

    #region NXmlContent<T>
    
    /// <summary>
    /// NXmlContent of T class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    [XmlRoot("Content")]
    public class NXmlContent<T> : NXmlContent
        where T : new()
    {
        #region Constructor
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public NXmlContent() 
            : base()
        {
            this.Type.DataType = typeof(T).FullName;
            this.Type.AssemblyName = Path.GetFileNameWithoutExtension(typeof(T).Assembly.Location);            
            this.Content = new T();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets document's content data.
        /// </summary>
        [PropertyOrder(3)]
        [XmlIgnore]
        [Category("Contents")]
        [Description("Gets or sets document's content data.")]
        public T Content
        {
            get 
            {
                if (null == this.XmlData)
                    this.XmlData = new T();
                return (T)this.XmlData; 
            }
            set
            {
                if (value.GetType() == typeof(T))
                {
                    this.XmlData = value;
                }
                else
                {
                    Console.WriteLine("Invalid Content Type.");
                }
            }
        }

        #endregion
    }

    #endregion

    #region NXmlDocument

    /// <summary>
    /// The NXmlDocument class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public class NXmlDocument
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NXmlDocument()
            : base()
        {
            this.Properties = new NXmlDocumentProperties();
            this.Contents = new List<NXmlContent>();
        }

        #endregion

        #region Public Propeties

        /// <summary>
        /// Gets or sets document properties.
        /// </summary>
        [PropertyOrder(-2)]
        [XmlElement]
        [Category("Documents")]
        [Description("Gets or sets Content properties.")]
        public NXmlDocumentProperties Properties { get; set; }
        /// <summary>
        /// Access Document's contents List.
        /// </summary>
        [PropertyOrder(0)]
        [XmlElement]
        [Category("Contents")]
        [Description("Access Document's contents List.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<NXmlContent> Contents { get; set; }

        #endregion

        #region Static Methods

        /// <summary>
        /// Save To File.
        /// </summary>
        /// <typeparam name="T">The NXmlDocument type.</typeparam>
        /// <param name="fileName">Filename to save.</param>
        /// <param name="obj">The NXmlDocument instance to save.</param>
        /// <returns>Returns NResult instance that contains exception if error occur.</returns>
        public static NResult<bool> SaveToFile<T>(string fileName, T obj)
            where T : NXmlDocument, new()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            NResult<bool> result = new NResult<bool>();
            result.Result = false;
            try
            {
                if (null != obj)
                {
                    List<Type> contentTypes = new List<Type>();
                    // Add Type of object itselt
                    contentTypes.Add(obj.GetType());
                    // Check content types.
                    if (null != obj.Contents && obj.Contents.Count > 0)
                    {
                        foreach (object content in obj.Contents)
                        {
                            if (null == content)
                                continue;
                            Type type = content.GetType();
                            // Checks Generic Type.
                            if (type.IsGenericType)
                            {
                                Type[] genTypes = type.GetGenericArguments();

                                foreach (Type genType in genTypes)
                                {
                                    if (null == genType)
                                        continue;
                                    if (!contentTypes.Contains(genType))
                                    {
                                        contentTypes.Add(genType);
                                    }
                                }
                            }
                            // Add Type It self.
                            if (!contentTypes.Contains(type))
                            {
                                contentTypes.Add(type);
                            }
                        }
                    }
                    // Kepp extra types.
                    Type[] originals = XmlManager.ExtraTypes;
                    // Add content types to Extra Types.
                    XmlManager.ExtraTypes = (null != contentTypes) ? 
                        contentTypes.ToArray() : null;
                    result = XmlManager.SaveToFile(fileName, obj.GetType(), obj);
                    // Restore extra types.
                    XmlManager.ExtraTypes = originals;
                }
            }
            catch (Exception ex)
            {
                med.Err(ex);
                // Set result.
                result.Err = ex;
            }

            return result;
        }

        public static NResult<D> LoadFromFile<D>(string fileName)
            where D : NXmlDocument, new()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            string dataTypeName = string.Empty;
            string assemblyName = string.Empty;
            XElement xmlDoc = null;

            NResult<D> result = new NResult<D>();

            try
            {
                // Load data for check information.
                xmlDoc = XElement.Load(fileName);
                if (null != xmlDoc)
                {
                    XElement infoElement = xmlDoc.Descendants("Properties").FirstOrDefault();

                    if (null != infoElement)
                    {
                        /*
                        XElement contentTypeElement = infoElement.Descendants("ContentType").FirstOrDefault();
                        if (null != contentTypeElement)
                        {
                            // Gets Data's Type.
                            XAttribute dataTypeAttr =
                                contentTypeElement.Attribute("DataType");
                            dataTypeName = (null != dataTypeAttr) ?
                                dataTypeAttr.Value : string.Empty;
                            // Gets Config's Assembly.
                            XAttribute assmNameAttr =
                                contentTypeElement.Attribute("AssemblyName");
                            assemblyName = (null != assmNameAttr) ?
                                assmNameAttr.Value : string.Empty;

                            if (!string.IsNullOrWhiteSpace(dataTypeName))
                            {
                                // Trick to add dll name after type 
                                // in this case is NLib.dll or NLib.Firebird.dll or NLib.ODP.dll.
                                Type contentType = null;
                                if (!string.IsNullOrWhiteSpace(assemblyName))
                                {
                                    contentType = Type.GetType(dataTypeName + ", " + assemblyName);
                                }
                                else
                                {
                                    contentType = Type.GetType(dataTypeName);
                                }
                                // Create Generic Type for NXmlDocument<T>.
                                Type type = typeof(NXmlDocument<>).MakeGenericType(contentType);
                                // Re-Load from file
                                result = XmlManager.LoadFromFile<D>(fileName);
                            }
                        }
                        */
                    }
                }
            }
            catch (Exception ex)
            {
                med.Err(ex);
            }
            finally
            {
                xmlDoc = null;
            }

            return result;
        }

        #endregion
    }

    #endregion

    #region NDatabaseContent<T>
    
    /// <summary>
    /// The NDatabaseProject class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public class NDatabaseContent<T> : NXmlContent<T>
        where T : NDbConfig, new()
    {
        #region Constructor
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public NDatabaseContent() 
            : base()
        {

        }

        #endregion
    }

    #endregion

    #region NDatabaseProject<T>
    
    /// <summary>
    /// The NDatabaseProject class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public class NDatabaseProject<T> : NXmlDocument
        where T : NDbConfig, new()
    {
        #region Constructor
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public NDatabaseProject() 
            : base()
        {
            this.Properties.DocType = "Database";
            this.Properties.Version = "1";
            // Add contents
            this.Contents.Add(new NDatabaseContent<T>());
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets database config.
        /// </summary>
        [PropertyOrder(1)]
        [XmlIgnore]
        [Category("Contents")]
        [Description("Gets or sets database config.")]
        public T Config
        {
            get 
            {
                if (this.Contents.Count > 0 &&
                    this.Contents[0] is NDatabaseContent<T>)
                    return (this.Contents[0] as NDatabaseContent<T>).Content;
                else return default(T);
            }
            set
            {
                if (this.Contents.Count <= 0)
                {
                    // Add contents
                    this.Contents.Add(new NDatabaseContent<T>());
                }
                (this.Contents[0] as NDatabaseContent<T>).Content = value;   
            }
        }

        #endregion
    }

    #endregion
}
#endif