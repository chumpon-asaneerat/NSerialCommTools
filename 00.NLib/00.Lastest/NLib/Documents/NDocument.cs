#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-08-21
=================
- Document Framework - Database Project Document.
  - Add DbObjectPropertyMap and DbObjectPropertyMapManager  class.
  - NDatabaseProjectDocument<T> class add Mappings property.
  - Add DomainGenerateOption class.
  - Add ModelGenerateOption class.
  - Add ServiceModelGenerateOption class.
======================================================================================================================
Update 2015-08-20
=================
- Document Framework - Database Project Document.
  - Add NDatabaseProjectDocument and NDatabaseProjectDocument<T> class.
    - Add ConnectionConfig property.
  - Add SqlServerProject class.
  - Add OracleProject class.
  - Add MsAccessProject class.
  - Add ExcelProject class.
======================================================================================================================
Update 2015-08-17
=================
- Document Framework - NDocument
  - Add NDocument abstract class with some static method(s).
======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NLib.Documents;

#endregion

namespace NLib.Documents
{
    #region NDocument (abstract)
    
    /// <summary>
    /// The NDocument Class.
    /// </summary>
    [Serializable]
    public abstract class NDocument
    {
        #region Internal Variables

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NDocument()
            : base()
        {
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NDocument()
        {
            // Force free memory
            NGC.FreeGC(this);
        }

        #endregion

        #region Public Properties

        #region ReadOnly
        
        /// <summary>
        /// Gets Document Type's Name.
        /// </summary>
        [Category("Documents")]
        [Description("Gets Document Type's Name.")]
        [Browsable(true)]
        [XmlIgnore]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DocumentTypeName
        {
            get
            {
                return NLib.Reflection.TypeUtils.GetReferenceName(this, staticPropertyName);
            }
        }

        #endregion

        #endregion

        #region Static Methods

        #region DocumentTypes

        #region Static variables

        private static Type _baseType = typeof(NDocument);
        private static string staticPropertyName = "NDocumentTypeName";

        #endregion

        #region Create NDocument By Provider Name

        /// <summary>
        /// Create NDocument By Document Type Name.
        /// </summary>
        /// <param name="documentType">The Document Type Name.</param>
        /// <returns>NDocument's Instance.</returns>
        public static NDocument Create(string documentType)
        {
            NDocument instance = null;
            Type[] types = GetNDocumentTypes();
            if (types == null || types.Length <= 0)
                instance = null;
            else
            {
                object val = NLib.Reflection.TypeUtils.Create(_baseType, staticPropertyName,
                    documentType, "Create");
                if (val != null && val.GetType().IsSubclassOf(_baseType))
                    instance = (val as NDocument);
                else return instance;
            }
            return instance;
        }

        #endregion

        #region Get NDocument Type Names

        /// <summary>
        /// Get NDocument Type Names.
        /// </summary>
        /// <returns>Returns list of NDocument Type's Names.</returns>
        public static string[] GetNDocumentTypeNames()
        {
            return NLib.Reflection.TypeUtils.GetInheritedTypeNames(_baseType, staticPropertyName);
        }

        #endregion

        #region Get NDocument Types

        /// <summary>
        /// Gets NDocument Types.
        /// </summary>
        /// <returns>Returns list of Avaliable NDocument Types that inherited from NDocument.</returns>
        public static Type[] GetNDocumentTypes()
        {
            return NLib.Reflection.TypeUtils.GetInheritedTypes(_baseType);
        }
        /// <summary>
        /// Gets NDocument Types.
        /// </summary>
        /// <param name="refresh">true for refresh cache.</param>
        /// <returns>Returns list of Avaliable NDocuments Type that inherited from NDocument.</returns>
        public static Type[] GetNDocumentTypes(bool refresh)
        {
            return NLib.Reflection.TypeUtils.GetInheritedTypes(_baseType, refresh);
        }

        #endregion

        #region Get NDocument Type Name

        /// <summary>
        /// Gets NDocument Type Name.
        /// </summary>
        public static string NDocumentTypeName
        {
            get { return "Not Specificed"; }
        }

        #endregion

        #endregion

        #region Create

        /// <summary>
        /// Create new NDocument Instance.
        /// </summary>
        /// <returns>Returns NDocument Instance.</returns>
        public static NDocument Create() { return null; }

        #endregion

        #region Prepare Methods

        /// <summary>
        /// Prepare.
        /// </summary>
        public static void Prepare() { }

        #endregion

        #endregion
    }

    #endregion
}

namespace NLib.Data
{
    #region DbObjectPropertyMap

    /// <summary>
    /// The DbObjectPropertyMap class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(NLib.Design.PropertySorterSupportExpandableTypeConverter))]
    public class DbObjectPropertyMap
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public DbObjectPropertyMap()
            : base()
        {

        }

        #endregion

        #region Overrides

        /// <summary>
        /// Equals.
        /// </summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>Returns true if obj is equals to current object.</returns>
        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;
            return this.GetHashCode().Equals(obj.GetHashCode());
        }
        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Returns hash code of object.</returns>
        public override int GetHashCode()
        {
            string result = string.Format("{0}_{1}", this.DbName, this.PropertyName);
            return result.GetHashCode();
        }
        /// <summary>
        /// To String.
        /// </summary>
        /// <returns>Returns string that represents an object instance.</returns>
        public override string ToString()
        {
            string result = "";
            if (!string.IsNullOrWhiteSpace(this.DbName))
            {
                result += this.DbName.Trim();
            }
            else result += "(null)";

            result += " = ";

            if (!string.IsNullOrWhiteSpace(this.PropertyName))
            {
                result += this.PropertyName.Trim();
            }
            else result += "(null)";

            return result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Database's Object Name.
        /// </summary>
        [Category("Mapping")]
        [Description("Gets or sets Database's Object Name.")]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(1)]
        public string DbName { get; set; }
        /// <summary>
        /// Gets or sets Class's property Name.
        /// </summary>
        [Category("Mapping")]
        [Description("Gets or sets Class's property Name.")]
        [XmlAttribute]
        [NLib.Design.PropertyOrder(2)]
        public string PropertyName { get; set; }

        #endregion
    }

    #endregion

    #region DbObjectPropertyMapManager

    /// <summary>
    /// The DbObjectPropertyMapManager class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(NLib.Design.PropertySorterSupportExpandableTypeConverter))]
    public class DbObjectPropertyMapManager
    {
        #region Internal Variables

        private List<DbObjectPropertyMap> _items = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public DbObjectPropertyMapManager()
            : base()
        {
            ValidateItemsInstance();
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~DbObjectPropertyMapManager()
        {
            if (null != _items)
                _items.Clear();
            _items = null;
        }

        #endregion

        #region Private Methods

        private void ValidateItemsInstance()
        {
            if (null == _items)
            {
                lock (this)
                {
                    _items = new List<DbObjectPropertyMap>();
                }
            }
        }

        #endregion

        #region Public Methods

        #region Find(s)

        /// <summary>
        /// Find Class's property name that mapped to specificed database's object name.
        /// </summary>
        /// <param name="dbName">The database's object name.</param>
        /// <returns>Returns null if not found otherwise returns DbObjectPropertyMap instance.</returns>
        public DbObjectPropertyMap FindByDbName(string dbName)
        {
            DbObjectPropertyMap result = null;

            if (null != _items)
            {
                // Find item by Column Name.
                DbObjectPropertyMap match = _items.First(
                    x => (string.Compare(x.DbName, dbName, true) == 0));
                // Gets Property Name,
                if (null == match || string.IsNullOrWhiteSpace(match.PropertyName))
                {
                    result = null;
                }
                else result = match;
            }
            else
            {
                ValidateItemsInstance();
            }

            return result;
        }
        /// <summary>
        /// Find Database's object name that mapped to specificed class's property name.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>Returns null if not found otherwise returns DbObjectPropertyMap instance.</returns>
        public DbObjectPropertyMap FindByProperty(string propertyName)
        {
            DbObjectPropertyMap result = null;

            if (null != _items)
            {
                // Find item by Property Name.
                DbObjectPropertyMap match = _items.First(
                    x => (string.Compare(x.PropertyName, propertyName, true) == 0));
                // Gets Column Name.
                if (null == match || string.IsNullOrWhiteSpace(match.DbName))
                {
                    result = null;
                }
                else result = match;
            }
            else
            {
                ValidateItemsInstance();
            }

            return result;
        }

        #endregion

        #region Add/Update

        /// <summary>
        /// Add data to items collection.
        /// </summary>
        /// <param name="dbName">The database's object name.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>Returns index of added instance.</returns>
        public int Add(string dbName, string propertyName = null)
        {
            int result = -1;
            if (string.IsNullOrWhiteSpace(dbName))
            {
                return result;
            }
            if (null == _items)
            {
                ValidateItemsInstance();
            }
            DbObjectPropertyMap match = this.FindByDbName(dbName);
            if (null == match)
            {
                match = new DbObjectPropertyMap();
                match.DbName = dbName.Trim();
                match.PropertyName = (string.IsNullOrWhiteSpace(propertyName)) ?
                    match.DbName : propertyName.Trim();
                _items.Add(match);
                result = _items.IndexOf(match);
            }
            return result;
        }
        /// <summary>
        /// Update item in items collection. If item not exists the new instance is added.
        /// </summary>
        /// <param name="dbName">The database's object name.</param>
        /// <param name="propertyName">The property name.</param>
        public void Update(string dbName, string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(dbName))
            {
                return;
            }
            DbObjectPropertyMap match = this.FindByDbName(dbName);
            if (null != match)
            {
                match.PropertyName = (string.IsNullOrWhiteSpace(propertyName)) ?
                    string.Empty : propertyName.Trim();
            }
            else
            {
                if (null == _items)
                    ValidateItemsInstance();
                match = new DbObjectPropertyMap();
                match.DbName = dbName.Trim();
                match.PropertyName = (string.IsNullOrWhiteSpace(propertyName)) ?
                    match.DbName : propertyName.Trim();
                _items.Add(match);
            }
        }
        /// <summary>
        /// Find Index by database object's name.
        /// </summary>
        /// <param name="dbName">The database's object name.</param>
        /// <returns>Returns index of item in collection that match database object's name.</returns>
        public int IndexOf(string dbName)
        {
            int result = -1;
            if (string.IsNullOrWhiteSpace(dbName))
            {
                return result;
            }
            if (null == _items)
            {
                ValidateItemsInstance();
                return result;
            }
            DbObjectPropertyMap match = this.FindByDbName(dbName);
            if (null != match)
            {
                result = _items.IndexOf(match);
            }
            return result;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets DbObjectPropertyMap List.
        /// </summary>
        [Category("Mapping")]
        [Description("Gets or sets DbObjectPropertyMap List.")]
        [XmlElement]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NLib.Design.PropertyOrder(10)]
        public List<DbObjectPropertyMap> Items
        {
            get
            {
                ValidateItemsInstance();
                return _items;
            }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    ValidateItemsInstance();
                }
            }
        }

        #endregion
    }

    #endregion
}

namespace NLib.Data
{
    #region DomainGenerateOption

    /// <summary>
    /// The DomainGenerateOption class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(NLib.Design.PropertySorterSupportExpandableTypeConverter))]
    public class DomainGenerateOption
    {
        #region Internal Variables

        private string _domainNamespace = "Domains";
        private string _dbManagerNamespace = "Services";
        private bool _generateDbManager = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public DomainGenerateOption() : base()
        {

        }

        #endregion

        #region Public Propeties

        /// <summary>
        /// Gets or sets the domain sub namespace.
        /// </summary>
        [Category("Option")]
        [Description("Gets or sets the domain sub namespace.")]
        [XmlAttribute]
        public string DomainNamespace
        {
            get { return _domainNamespace; }
            set
            {
                if (_domainNamespace != value)
                {
                    _domainNamespace = value;
                    if (string.IsNullOrWhiteSpace(_domainNamespace))
                    {
                        _domainNamespace = "Domains";
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the database manager sub namespace.
        /// </summary>
        [Category("Option")]
        [Description("Gets or sets the database manager sub namespace.")]
        [XmlAttribute]
        public string DatabaseManagerNamespace
        {
            get { return _dbManagerNamespace; }
            set
            {
                if (_dbManagerNamespace != value)
                {
                    _dbManagerNamespace = value;
                    if (string.IsNullOrWhiteSpace(_dbManagerNamespace))
                    {
                        _dbManagerNamespace = "Services";
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets is generate database manager.
        /// </summary>
        [Category("Option")]
        [Description("Gets or sets is generate database manager.")]
        [XmlAttribute]
        public bool GenerateDatabaseManager 
        { 
            get { return _generateDbManager; } 
            set
            {
                if (_generateDbManager != value)
                {
                    _generateDbManager = value;
                }
            }
        }

        #endregion
    }

    #endregion

    #region ModelGenerateOption

    /// <summary>
    /// The ModelGenerateOption class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(NLib.Design.PropertySorterSupportExpandableTypeConverter))]
    public class ModelGenerateOption
    {
        #region Internal Variables

        private string _modelNamespace = "Models";
        private string _extensionNamespace = "Models";
        private bool _generateExtensionMethod = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ModelGenerateOption()
            : base()
        {

        }

        #endregion

        #region Public Propeties

        /// <summary>
        /// Gets or sets the model sub namespace.
        /// </summary>
        [Category("Option")]
        [Description("Gets or sets the model sub namespace.")]
        [XmlAttribute]
        public string ModelNamespace
        {
            get { return _modelNamespace; }
            set
            {
                if (_modelNamespace != value)
                {
                    _modelNamespace = value;
                    if (string.IsNullOrWhiteSpace(_modelNamespace))
                    {
                        _modelNamespace = "Models";
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the extension methods sub namespace.
        /// </summary>
        [Category("Option")]
        [Description("Gets or sets the extension methods sub namespace.")]
        [XmlAttribute]
        public string ExtensionMethodsNamespace
        {
            get { return _extensionNamespace; }
            set
            {
                if (_extensionNamespace != value)
                {
                    _extensionNamespace = value;
                    if (string.IsNullOrWhiteSpace(_extensionNamespace))
                    {
                        _extensionNamespace = "Models";
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets is generate extension methods.
        /// </summary>
        [Category("Option")]
        [Description("Gets or sets is generate extension methods.")]
        [XmlAttribute]
        public bool GenerateExtensionMethods
        {
            get { return _generateExtensionMethod; }
            set
            {
                if (_generateExtensionMethod != value)
                {
                    _generateExtensionMethod = value;
                }
            }
        }

        #endregion
    }

    #endregion

    #region ServiceModelGenerateOption
    
    /// <summary>
    /// The ServiceModelGenerateOption class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(NLib.Design.PropertySorterSupportExpandableTypeConverter))]
    public class ServiceModelGenerateOption
    {
        #region Internal Variables

        private string _serviceModelNamespace = "ServiceModel";
        private string _extensionNamespace = "ServiceModel";
        private bool _generateExtensionMethod = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ServiceModelGenerateOption() : base()
        {

        }

        #endregion

        #region Public Propeties

        /// <summary>
        /// Gets or sets the model sub namespace.
        /// </summary>
        [Category("Option")]
        [Description("Gets or sets the model sub namespace.")]
        [XmlAttribute]
        public string ServiceModelNamespace
        {
            get { return _serviceModelNamespace; }
            set
            {
                if (_serviceModelNamespace != value)
                {
                    _serviceModelNamespace = value;
                    if (string.IsNullOrWhiteSpace(_serviceModelNamespace))
                    {
                        _serviceModelNamespace = "ServiceModel";
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the extension methods sub namespace.
        /// </summary>
        [Category("Option")]
        [Description("Gets or sets the extension methods sub namespace.")]
        [XmlAttribute]
        public string ExtensionMethodsNamespace
        {
            get { return _extensionNamespace; }
            set
            {
                if (_extensionNamespace != value)
                {
                    _extensionNamespace = value;
                    if (string.IsNullOrWhiteSpace(_extensionNamespace))
                    {
                        _extensionNamespace = "ServiceModel";
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets is generate extension methods.
        /// </summary>
        [Category("Option")]
        [Description("Gets or sets is generate extension methods.")]
        [XmlAttribute]
        public bool GenerateExtensionMethods
        {
            get { return _generateExtensionMethod; }
            set
            {
                if (_generateExtensionMethod != value)
                {
                    _generateExtensionMethod = value;
                }
            }
        }

        #endregion
    }

    #endregion

    #region CSharpCodeGenerateOptions

    /// <summary>
    /// The CSharpCodeGenerateOptions class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(NLib.Design.PropertySorterSupportExpandableTypeConverter))]
    public class CSharpCodeGenerateOptions
    {
        #region Internal Variables

        private DomainGenerateOption _domainOption = null;
        private ModelGenerateOption _modelOption = null;
        private ServiceModelGenerateOption _serviceModelOption = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public CSharpCodeGenerateOptions() : base()
        {
            InitOptions();
        }

        #endregion

        #region Private Methods

        private void InitOptions()
        {
            lock (this)
            {
                if (null == _domainOption)
                {
                    _domainOption = new DomainGenerateOption()
                    {
                        GenerateDatabaseManager = true
                    };
                }
                if (null == _modelOption)
                {
                    _modelOption = new ModelGenerateOption()
                    {
                        GenerateExtensionMethods = true
                    };
                }
                if (null == _serviceModelOption)
                {
                    _serviceModelOption = new ServiceModelGenerateOption()
                    {
                        GenerateExtensionMethods = true
                    };
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets domain code generate  option.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets domain code generate option.")]
        [XmlElement]
        public DomainGenerateOption DomainOption
        {
            get 
            {
                if (null == _domainOption) InitOptions();
                return _domainOption;
            }
            set
            {
                if (_domainOption != value)
                {
                    _domainOption = value;
                    if (null == _domainOption) InitOptions();
                }
            }
        }
        /// <summary>
        /// Gets or sets model code generate  option.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets model code generate option.")]
        [XmlElement]
        public ModelGenerateOption ModelOption
        {
            get
            {
                if (null == _modelOption) InitOptions();
                return _modelOption;
            }
            set
            {
                if (_modelOption != value)
                {
                    _modelOption = value;
                    if (null == _modelOption) InitOptions();
                }
            }
        }
        /// <summary>
        /// Gets or sets service model code generate option.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets service model code generate option.")]
        [XmlElement]
        public ServiceModelGenerateOption ServiceModelOption
        {
            get
            {
                if (null == _serviceModelOption) InitOptions();
                return _serviceModelOption;
            }
            set
            {
                if (_serviceModelOption != value)
                {
                    _serviceModelOption = value;
                    if (null == _serviceModelOption) InitOptions();
                }
            }
        }

        #endregion
    }

    #endregion
}

namespace NLib.Data
{
    #region NDatabaseProjectDocument (abstract)

    /// <summary>
    /// The NDatabaseProjectDocument class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(NLib.Design.PropertySorterSupportExpandableTypeConverter))]
    public abstract class NDatabaseProjectDocument : NDocument
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NDatabaseProjectDocument()
            : base()
        {
            
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~NDatabaseProjectDocument()
        {
            
        }

        #endregion
    }

    #endregion

    #region NDatabaseProjectDocument<T> (abstract)

    /// <summary>
    /// The NDatabaseProjectDocument (generic) class.
    /// </summary>
    /// <typeparam name="T">The NDbConfig Type.</typeparam>
    [Serializable]
    [TypeConverter(typeof(NLib.Design.PropertySorterSupportExpandableTypeConverter))]
    public abstract class NDatabaseProjectDocument<T> : NDatabaseProjectDocument
        where T: NDbConfig, new ()
    {
        #region Internal Variables

        private T _connConfig = default(T);
        private DbObjectPropertyMapManager _mapManager = null;
        private CSharpCodeGenerateOptions _generateOptions = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NDatabaseProjectDocument() : base() 
        {
            _connConfig = new T();
            ValidateMapManagerInstance();
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~NDatabaseProjectDocument()
        {
            _mapManager = null;
            _connConfig = null;
        }

        #endregion

        #region Private Methods

        private void ValidateConnectionConfig()
        {
            if (null == _connConfig)
            {
                lock (this)
                {
                    _connConfig = new T();
                }
            }
        }

        private void ValidateMapManagerInstance()
        {
            if (null == _mapManager)
            {
                lock (this)
                {
                    _mapManager = new DbObjectPropertyMapManager();
                }
            }
        }

        private void ValidateGenerateOptionsInstance()
        {
            if (null == _generateOptions)
            {
                lock (this)
                {
                    _generateOptions = new CSharpCodeGenerateOptions();
                }
            }
        }

        #endregion

        #region Public Properties

        #region Connection
        
        /// <summary>
        /// Gets or sets Connection Config.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets Connection Config.")]
        [XmlElement]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NLib.Design.PropertyOrder(-50)]
        public T ConnectionConfig
        {
            get 
            {
                ValidateConnectionConfig();
                return _connConfig; 
            }
            set 
            {
                if (_connConfig != value)
                {
                    _connConfig = value;
                    ValidateConnectionConfig();
                }
            }
        }

        #endregion

        #region Mappings
        
        /// <summary>
        /// Gets or sets Mapping Manager.
        /// </summary>
        [Category("Mapping")]
        [Description("Gets Mapping Manager.")]
        [XmlElement]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NLib.Design.PropertyOrder(-40)]
        public DbObjectPropertyMapManager Mappings
        {
            get
            {
                ValidateMapManagerInstance();
                return _mapManager;
            }
            set
            {
                if (_mapManager != value)
                {
                    _mapManager = value;
                    ValidateMapManagerInstance();
                }
            }
        }

        #endregion

        #region GenerateOptions

        /// <summary>
        /// Gets or sets Generate Options.
        /// </summary>
        [Category("Generate Options")]
        [Description("Gets or sets Generate Options.")]
        [XmlElement]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NLib.Design.PropertyOrder(-30)]
        public CSharpCodeGenerateOptions GenerateOptions
        {
            get
            {
                ValidateGenerateOptionsInstance();
                return _generateOptions;
            }
            set
            {
                if (_generateOptions != value)
                {
                    _generateOptions = value;
                    ValidateGenerateOptionsInstance();
                }
            }
        }

        #endregion

        #endregion
    }

    #endregion
}

namespace NLib.Data
{
    #region SqlServerProject
    
    /// <summary>
    /// The SqlServerProject class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(NLib.Design.PropertySorterSupportExpandableTypeConverter))]
    public class SqlServerProject : NDatabaseProjectDocument<SqlServerConfig>
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SqlServerProject() : base()
        {

        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~SqlServerProject()
        {

        }

        #endregion

        #region Static Access

        #region Get NDocument Type Name

        /// <summary>
        /// Gets NDocument Type Name.
        /// </summary>
        public static new string NDocumentTypeName
        {
            get { return "Microsoft SQL Server (Native)"; }
        }

        #endregion

        #region Create

        /// <summary>
        /// Create new NDocument Instance.
        /// </summary>
        /// <returns>Returns NDocument Instance.</returns>
        public static new NDocument Create() { return new SqlServerProject(); }

        #endregion

        #endregion
    }

    #endregion
}

namespace NLib.Data
{
    #region OracleProject

    /// <summary>
    /// The OracleProject class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(NLib.Design.PropertySorterSupportExpandableTypeConverter))]
    public class OracleProject : NDatabaseProjectDocument<OracleConfig>
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public OracleProject()
            : base()
        {

        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~OracleProject()
        {

        }

        #endregion

        #region Static Access

        #region Get NDocument Type Name

        /// <summary>
        /// Gets NDocument Type Name.
        /// </summary>
        public static new string NDocumentTypeName
        {
            get { return "Oracle (Native)"; }
        }

        #endregion

        #region Create

        /// <summary>
        /// Create new NDocument Instance.
        /// </summary>
        /// <returns>Returns NDocument Instance.</returns>
        public static new NDocument Create() { return new OracleProject(); }

        #endregion

        #endregion
    }

    #endregion
}

namespace NLib.Data
{
    #region MsAccessProject

    /// <summary>
    /// The MsAccessProject class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(NLib.Design.PropertySorterSupportExpandableTypeConverter))]
    public class MsAccessProject : NDatabaseProjectDocument<MsAccessConfig>
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MsAccessProject()
            : base()
        {

        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~MsAccessProject()
        {

        }

        #endregion

        #region Static Access

        #region Get NDocument Type Name

        /// <summary>
        /// Gets NDocument Type Name.
        /// </summary>
        public static new string NDocumentTypeName
        {
            get { return "MS Access (OLEDB)"; }
        }

        #endregion

        #region Create

        /// <summary>
        /// Create new NDocument Instance.
        /// </summary>
        /// <returns>Returns NDocument Instance.</returns>
        public static new NDocument Create() { return new MsAccessProject(); }

        #endregion

        #endregion
    }

    #endregion
}

namespace NLib.Data
{
    #region ExcelProject

    /// <summary>
    /// The ExcelProject class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(NLib.Design.PropertySorterSupportExpandableTypeConverter))]
    public class ExcelProject : NDatabaseProjectDocument<ExcelConfig>
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExcelProject()
            : base()
        {

        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~ExcelProject()
        {

        }

        #endregion

        #region Static Access

        #region Get NDocument Type Name

        /// <summary>
        /// Gets NDocument Type Name.
        /// </summary>
        public static new string NDocumentTypeName
        {
            get { return "MS Excel (OLEDB)"; }
        }

        #endregion

        #region Create

        /// <summary>
        /// Create new NDocument Instance.
        /// </summary>
        /// <returns>Returns NDocument Instance.</returns>
        public static new NDocument Create() { return new ExcelProject(); }

        #endregion

        #endregion
    }

    #endregion
}
