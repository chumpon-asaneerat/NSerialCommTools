#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-16
=================
- Instance Metadata Framework updated.
  - All exception and error message is write to console for decouple used of log framework.

======================================================================================================================
Update 2013-08-06
=================
- Instance Metadata Framework updated.
  - Add MetaDataInfo class for share data between object instance that has same type.
  - Optimized code by add cache machanism into MetaDataInfo and removed iteration code
    that not need.
  - Add exception handle code that send to debugger when error occur.
  - Change Properties proeprty from List<PropertyInfo> to Dictionary<string, PropertyInfo>
    and move to MetaDataInfo class.
  - Change Events proeprty from List<EventInfo> to Dictionary<string, EventInfo> and move 
    to MetaDataInfo class.

======================================================================================================================
Update 2013-08-04
=================
- Instance Metadata Framework updated.
  - Add VariableAccess class with supports read all public events.

======================================================================================================================
Update 2013-07-09
=================
- Instance Metadata Framework added.
  - Add VarCache class. The original is DumpCache class.
  - Add ObjectInfo class.
  - Add VariableAccess class with supports read all public properties.

======================================================================================================================
// </[History]>

#endif
#endregion

#define DISABLE_LOG

#region Using

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

#endregion

namespace NLib.Reflection
{
    #region VarCache

    /// <summary>
    /// The Var'Cache class. Internal used for dump variable name and value.
    /// </summary>
    /// <typeparam name="T">The target object type.</typeparam>
    static class VarCache<T>
    {
        #region Public Fields (Readonly)

        /// <summary>
        /// Gets Name.
        /// </summary>
        public static readonly string Name;
        /// <summary>
        /// Gets Properties.
        /// </summary>
        public static readonly PropertyInfo[] properties;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor (static).
        /// </summary>
        static VarCache()
        {
            Type tType = typeof(T);
            // default name
            Name = "(no name)";

            if (null != tType)
            {
                // this should always be only one property.
                // because it's anonymous type that has only open variable assigned
                properties = tType.GetProperties(); 
                if (properties.Length == 1)
                {
                    Name = properties[0].Name;
                }
            }
        }

        #endregion
    }

    #endregion

    #region MetaDataInfo
    
    /// <summary>
    /// The MetaDataInfo class. This class keep the object's type metadata.
    /// </summary>
    public class MetaDataInfo
    {
        #region Static Acccess

        private static Dictionary<Type, MetaDataInfo> _caches = new Dictionary<Type, MetaDataInfo>();
        /// <summary>
        /// Gets MetaDataInfo in cache. This method is used internally by Variable Access and ObjectInfo.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>Returns null if not found. Otherwise return MetaDataInfo that match type.</returns>
        public static MetaDataInfo GetMetaData(Type type)
        {
            MetaDataInfo result = null;
            if (_caches.ContainsKey(type))
            {
                lock (typeof(MetaDataInfo))
                {
                    result = _caches[type];
                }
            }

            return result;
        }
        /// <summary>
        /// Register new meta data. This method is used internally by Variable Access and ObjectInfo.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The meta data instance.</param>
        /// <param name="autoReplace">True if need to replace exists type with new meta data.</param>
        /// <returns>Returns true if register success.</returns>
        public static bool Register(Type type, MetaDataInfo value, 
            bool autoReplace = false)
        {
            bool result = false;
            if (null == type || null == value)
                return result;
            if (null == _caches)
            {
                lock (typeof(MetaDataInfo))
                {
                    _caches = new Dictionary<Type, MetaDataInfo>();
                }
            }
            if (!_caches.ContainsKey(type))
            {
                lock (typeof(MetaDataInfo))
                {
                    try
                    {
                        _caches.Add(type, value);
                        result = true;
                    }
                    catch (Exception ex)
                    {
#if DISABLE_LOG
                        Console.WriteLine(ex);
#else
                        MethodBase med = MethodBase.GetCurrentMethod();
                        ex.Err(med);
#endif
                        result = false;
                    }
                }
            }
            else
            {
                // already exists so replace it.
                if (autoReplace)
                {
                    _caches[type] = value;
                }
                result = true;
            }

            return result;
        }

        #endregion

        #region Internal Variables

        private Dictionary<string, PropertyInfo> _properties = null;
        private Dictionary<string, EventInfo> _events = null;

        #endregion

        #region Constructor and Destructor
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public MetaDataInfo() : base() { }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~MetaDataInfo()
        {

        }

        #endregion

        #region Private Methods

        private Dictionary<string, PropertyInfo> GetProperties()
        {
            Dictionary<string, PropertyInfo> results = null;

            PropertyInfo[] props = null;

            if (null == this.Type)
                return results;
            // Gets properties fron type.
            props = this.Type.GetProperties();
            if (null != props && props.Length > 0)
            {
                // Create Dictionary
                results = new Dictionary<string, PropertyInfo>();

                foreach (PropertyInfo prop in props)
                {
                    if (!results.ContainsKey(prop.Name))
                    {
                        results.Add(prop.Name, prop);
                    }
                    else
                    {
                        //Console.WriteLine("property already added.");
                    }
                }
            }

            return results;
        }

        private Dictionary<string, EventInfo> GetEvents()
        {
            Dictionary<string, EventInfo> events = null;

            if (null == this.Type)
                return events;

            // Get events for type
            EventInfo[] typeEvents = this.Type.GetEvents();
            if (null != typeEvents && typeEvents.Length > 0)
            {
                // create dictionary
                events = new Dictionary<string, EventInfo>();

                foreach (EventInfo typeEvent in typeEvents)
                {
                    if (!events.ContainsKey(typeEvent.Name))
                    {
                        events.Add(typeEvent.Name, typeEvent);
                    }
                    else
                    {
                        //Console.WriteLine("event already added.");
                    }
                }
            }
            // Check is interface.
            if (this.Type.IsInterface)
            {
                Type[] allinterfaces = this.Type.GetInterfaces();
                if (null != allinterfaces && allinterfaces.Length > 0)
                {
                    foreach (Type intfType in allinterfaces)
                    {
                        EventInfo[] evts = intfType.GetEvents();
                        if (null == evts || evts.Length <= 0)
                            continue;

                        // create dictionary if required
                        if (null == events)
                        {
                            events = new Dictionary<string, EventInfo>();
                        }

                        foreach (EventInfo evt in evts)
                        {
                            if (!events.ContainsKey(evt.Name))
                            {
                                events.Add(evt.Name, evt);
                            }
                            else
                            {
                                //Console.WriteLine("event already added.");
                            }
                        }
                    }
                }
            }

            return events;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets (or internal sets) instance data type.
        /// </summary>
        [Category("MetaData")]
        [Description("Gets (or internal sets) instance data type.")]
        public Type Type { get; internal set; }
        /// <summary>
        /// Gets all avaliable properties for specificed type.
        /// </summary>
        [Category("MetaData")]
        [Description("Gets all avaliable properties for specificed type.")]
        public Dictionary<string, PropertyInfo> Properties 
        { 
            get
            {
                if (null == _properties)
                {
                    lock (this)
                    {
                        _properties = GetProperties();
                        if (null == _properties)
                        {
                            // no properties found.
                            _properties = new Dictionary<string, PropertyInfo>();
                        }
                    }
                }
                return _properties;
            }
        }
        /// <summary>
        /// Gets all avaliable events for specificed type.
        /// </summary>
        [Category("MetaData")]
        [Description("Gets all avaliable events for specificed type.")]
        public Dictionary<string, EventInfo> Events 
        { 
            get 
            {
                if (null == _events)
                {
                    lock (this)
                    {
                        _events = GetEvents();
                        if (null == _events)
                        {
                            // no event found.
                            _events = new Dictionary<string, EventInfo>();
                        }
                    }
                }
                return _events; 
            } 
        }

        #endregion
    }

    #endregion

    #region ObjectInfo

    /// <summary>
    /// The Object Info class. This class keep the object's instance metadata.
    /// </summary>
    public class ObjectInfo
    {
        #region Public Methods

        #region Get Property Value
        
        /// <summary>
        /// Get Value.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="propertyName">
        /// The property's name. If the variable is value type or string this value should be null or empty string.
        /// </param>
        /// <returns>Returns value of the variable.</returns>
        public T GetValue<T>(string propertyName = "")
        {
            T result = default(T);

            if (null != this.MetaData && null != this.MetaData.Type &&
                null != this.MetaData.Properties &&
                null != this.Instance)
            {
                if (!string.IsNullOrWhiteSpace(propertyName))
                {
                    // The DynamicAccess class is already contains cache. So no need to
                    // optimized or find match property.
                    try
                    {
                        object val = DynamicAccess.Get(this.Instance, propertyName);
                        if (null != val && val.GetType().Equals(typeof(T)))
                        {
                            result = (T)val;
                        }
                    }
                    catch (Exception ex)
                    {
#if DISABLE_LOG
                        Console.WriteLine(ex);
#else
                        MethodBase med = MethodBase.GetCurrentMethod();
                        ex.Err(med);
#endif
                    }
                }
                else
                {
                    // no property name so assume it's value type or string.
                    try
                    {
                        if (null != this.Instance && this.Instance.GetType().Equals(typeof(T)))
                        {
                            result = (T)this.Instance;
                        }
                    }
                    catch (Exception ex)
                    {
#if DISABLE_LOG
                        Console.WriteLine(ex);
#else
                        MethodBase med = MethodBase.GetCurrentMethod();
                        ex.Err(med);
#endif
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Get Value.
        /// </summary>
        /// <param name="propertyName">
        /// The property's name. If the variable is value type or string this value should be null or empty string.
        /// </param>
        /// <returns>Returns value of the variable.</returns>
        public object GetValue(string propertyName = "")
        {
            object result = null;

            if (null != this.MetaData && null != this.MetaData.Type &&
                null != this.MetaData.Properties &&
                null != this.Instance)
            {
                if (!string.IsNullOrWhiteSpace(propertyName))
                {
                    // The DynamicAccess class is already contains cache. So no need to
                    // optimized or find match property.
                    try
                    {
                        result = DynamicAccess.Get(this.Instance, propertyName);
                    }
                    catch (Exception ex)
                    {
#if DISABLE_LOG
                        Console.WriteLine(ex);
#else
                        MethodBase med = MethodBase.GetCurrentMethod();
                        ex.Err(med);
#endif
                    }
                }
                else
                {
                    // no property name so assume it's value type or string.
                    try
                    {
                        result = this.Instance;
                    }
                    catch (Exception ex)
                    {
#if DISABLE_LOG
                        Console.WriteLine(ex);
#else
                        MethodBase med = MethodBase.GetCurrentMethod();
                        ex.Err(med);
#endif
                    }
                }
            }

            return result;
        }

        #endregion

        #region Set Property Value

        /// <summary>
        /// Set Value.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="propertyName">The property's name.</param>
        /// <param name="value">The property's value.</param>
        public void SetValue<T>(string propertyName, T value = default(T))
        {
            if (null != this.MetaData && null != this.MetaData.Type &&
                null != this.MetaData.Properties &&
                null != this.Instance)
            {
                if (!string.IsNullOrWhiteSpace(propertyName))
                {
                    // The DynamicAccess class is already contains cache. So no need to
                    // optimized or find match property.
                    try
                    {
                        DynamicAccess.Set(this.Instance, propertyName, value);
                    }
                    catch (Exception ex)
                    {
#if DISABLE_LOG
                        Console.WriteLine(ex);
#else
                        MethodBase med = MethodBase.GetCurrentMethod();
                        ex.Err(med);
#endif
                    }
                }
                else
                {
                    // no property name so assume it's value type or string.
                    if (null != this.Instance && this.Instance.GetType().Equals(typeof(T)))
                    {
                        try
                        {
                            this.Instance = value;
                        }
                        catch (Exception ex)
                        {
#if DISABLE_LOG
                            Console.WriteLine(ex);
#else
                            MethodBase med = MethodBase.GetCurrentMethod();
                            ex.Err(med);
#endif
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Set Value.
        /// </summary>
        /// <param name="propertyName">The property's name.</param>
        /// <param name="value">The property's value.</param>
        public void SetValue(string propertyName, object value = null)
        {
            if (null != this.MetaData && null != this.MetaData.Type &&
                null != this.MetaData.Properties &&
                null != this.Instance)
            {
                if (!string.IsNullOrWhiteSpace(propertyName))
                {
                    // The DynamicAccess class is already contains cache. So no need to
                    // optimized or find match property.
                    try
                    {
                        DynamicAccess.Set(this.Instance, propertyName, value);
                    }
                    catch (Exception ex)
                    {
#if DISABLE_LOG
                        Console.WriteLine(ex);
#else
                        MethodBase med = MethodBase.GetCurrentMethod();
                        ex.Err(med);
#endif
                    }
                }
                else
                {
                    // no property name so assume it's value type or string.
                    try
                    {
                        this.Instance = value;
                    }
                    catch (Exception ex)
                    {
#if DISABLE_LOG
                        Console.WriteLine(ex);
#else
                        MethodBase med = MethodBase.GetCurrentMethod();
                        ex.Err(med);
#endif
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets (or internal sets) instance MetaData.
        /// </summary>
        [Category("MetaData")]
        [Description("Gets (or internal sets) instance MetaData.")]
        public MetaDataInfo MetaData { get; internal set; }
        /// <summary>
        /// Gets (or internal sets) instance name.
        /// </summary>
        [Category("MetaData")]
        [Description("Gets (or internal sets) instance name.")]
        public string Name { get; internal set; }
        /// <summary>
        /// Gets (or internal sets) target instance.
        /// </summary>
        [Category("MetaData")]
        [Description("Gets (or internal sets) target instance.")]
        public object Instance { get; internal set; }

        #endregion
    }

    #endregion

    #region VariableAccess
    
    /// <summary>
    /// Variable Access.
    /// </summary>
    public class VariableAccess
    {
        #region Constructor
        
        /// <summary>
        /// Constructor.
        /// </summary>
        private VariableAccess() : base() { }

        #endregion

        private static Dictionary<Type, bool> _isAnonymousTypes = new Dictionary<Type, bool>();
        #region Private Methods

        /// <summary>
        /// CheckIfAnonymousType.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns>Retruns true if type is anonymous type.</returns>
        private static bool CheckIfAnonymousType2(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // Note: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type,
                typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType
                && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic
                && (string.IsNullOrWhiteSpace(type.Namespace));
        }
        /// <summary>
        /// CheckIfAnonymousType.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns>Retruns true if type is anonymous type.</returns>
        private static bool CheckIfAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!_isAnonymousTypes.ContainsKey(type))
            {
                // Note: The only way to detect anonymous types right now.
                bool isAnonymousType = Attribute.IsDefined(type,
                    typeof(CompilerGeneratedAttribute), false)
                    && type.IsGenericType
                    && type.Name.Contains("AnonymousType")
                    && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                    && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic
                    && (string.IsNullOrWhiteSpace(type.Namespace));

                _isAnonymousTypes.Add(type, isAnonymousType);

                return isAnonymousType;
            }
            else return _isAnonymousTypes[type];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Metadata. (This method required external classes to optimized for performance).
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="item">The anonymous type of actual type.</param>
        /// <returns>Returns instance of variable that match the item.</returns>
        public static ObjectInfo GetMetadata<T>(T item)
        {
            ObjectInfo result = null;
            if (null == item)
                return result;

            Type TType = typeof(T);
            bool isAnonymousType = CheckIfAnonymousType(TType);

            Type itemType = (isAnonymousType) ? TType : item.GetType();
            Type actualItemType = null;
            PropertyInfo varPropInfo = null;

            object valInst = null; // for variable instance.

            string name = string.Empty;

            #region Check is AnonymousType
            
            if (isAnonymousType)
            {
                // Anonymous type so extract type generic parameter/event
                name = VarCache<T>.Name; // read variable name
                varPropInfo = (VarCache<T>.properties.Length >= 1) ? 
                    VarCache<T>.properties[0] : null;
            }
            else
            {
                // Not anonymous type so extract type from item parameter
                varPropInfo = itemType.GetProperty("Name", 
                    BindingFlags.Public | BindingFlags.Instance);

                if (null != varPropInfo)
                {
                    object nameVal = DynamicAccess<string>.Get(item, "Name");
                    if (null != nameVal && nameVal.GetType() == typeof(string))
                    {
                        name = (string)nameVal;
                    }
                }
            }

            #endregion

            #region Check is generic type or not and re-read propert info if required

            if (null != itemType)
            {
                if (itemType.IsGenericType)
                {
                    try
                    {
                        // Find generic type arguments - in some control like Ribbon
                        // the control may contain generic type.
                        actualItemType = itemType.GetGenericArguments()[0];
                    }
                    catch (Exception ex)
                    {
#if DISABLE_LOG
                        Console.WriteLine(ex);
#else
                        MethodBase med = MethodBase.GetCurrentMethod();
                        ex.Err(med);
#endif
                    }
                }
                else
                {
                    actualItemType = itemType;
                }
            }

            #endregion

            #region Find target instance

            if (null != actualItemType && null != varPropInfo)
            {
                if (isAnonymousType)
                {
                    // Get instance that used for read/write property's value
                    valInst = varPropInfo.GetValue(item, Type.EmptyTypes);
                }
                else
                {
                    valInst = item;
                }
            }

            #endregion

            #region Create Instance
            
            if (null != actualItemType)
            {
                // Find meta data info
                MetaDataInfo metaData = MetaDataInfo.GetMetaData(actualItemType);
                if (null == metaData)
                {
                    metaData = new MetaDataInfo();
                    metaData.Type = actualItemType;
                    // register the meta data
                    if (!MetaDataInfo.Register(actualItemType, metaData))
                    {
#if DISABLE_LOG
                        Console.WriteLine("Cannot register metadata.");
#else
                        MethodBase med = MethodBase.GetCurrentMethod();
                        "Cannot register metadata.".Err(med);
#endif
                        metaData = null;
                    }
                }

                // Crerate instance
                result = new ObjectInfo();
                // Assigned information.
                result.MetaData = metaData;
                result.Name = name;
                result.Instance = valInst;
            }

            #endregion

            return result;
        }

        #endregion
    }

    #endregion
}
