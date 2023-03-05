#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2020-06-25
=================
- Reflection library updated.
  - Update Nlib PeropertyMapNameExtensionMethods class update AssignTo method (change type checking).
  - Update Nlib PeropertyMapNameExtensionMethods class add CloneTo method.

======================================================================================================================
Update 2020-06-19
=================
- Add code for PropertyMapName and related classes.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace NLib.Reflection
{
    #region PropertyMapNameAttribute

    /// <summary>
    /// The PropertyMapName Attribute class.
    /// </summary>
    public class PropertyMapNameAttribute : Attribute
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private PropertyMapNameAttribute() : base() { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The map name.</param>
        /// <param name="baseType">The base type.</param>
        public PropertyMapNameAttribute(string name, Type baseType) : base()
        {
            this.Name = name;
            this.BaseType = baseType;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The map name.</param>
        public PropertyMapNameAttribute(string name) : this(name, null) { }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets map name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets base type.
        /// </summary>
        public Type BaseType { get; set; }

        #endregion
    }

    #endregion

    #region PeropertyMapNameCache (internal)

    /// <summary>
    /// The PeropertyMapName Cache class (internal).
    /// </summary>
    internal class PeropertyMapNameCache
    {
        #region Internal Variables

        private Dictionary<Type, PropertyMapName> _map = new Dictionary<Type, PropertyMapName>();

        #endregion

        #region Private Methods

        private void InitType(Type type)
        {
            PropertyInfo[] props = type.GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            foreach (var prop in props)
            {
                /*
                // .NET 4.0
                object[] attrs = prop.GetCustomAttributes(typeof(PropertyMapNameAttribute), true);
                if (null != attrs && attrs.Length > 0 && attrs[0] is PropertyMapNameAttribute)
                {
                    PropertyMapNameAttribute map = attrs[0] as PropertyMapNameAttribute;
                    if (null != map)
                    {
                        PeropertyMapName mapInfo;
                        if (!_map.ContainsKey(type))
                        {
                            mapInfo = new PeropertyMapName();
                            _map.Add(type, mapInfo);
                        }
                        else mapInfo = _map[type];

                        if (!mapInfo.ContainsKey(map.Name))
                        {
                            mapInfo.Add(map.Name, prop);
                        }
                    }
                }
                */
                // .NET 4.5
                PropertyMapNameAttribute map = prop.GetCustomAttribute<PropertyMapNameAttribute>(true);
                if (null != map)
                {
                    PropertyMapName mapInfo;
                    if (!_map.ContainsKey(type))
                    {
                        mapInfo = new PropertyMapName();
                        _map.Add(type, mapInfo);
                    }
                    else mapInfo = _map[type];

                    if (!mapInfo.ContainsKey(map.Name))
                    {
                        mapInfo.Add(map.Name, new PropertyMapInfo(prop, map.BaseType));
                    }
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Indexer access.
        /// </summary>
        /// <param name="value">The target type.</param>
        /// <returns>
        /// Returns PeropertyMapName instance that match target type. If not found returns null.
        /// </returns>
        public PropertyMapName this[Type value]
        {
            get
            {
                if (!_map.ContainsKey(value)) InitType(value);
                if (!_map.ContainsKey(value)) return null;
                return _map[value];
            }
        }

        #endregion
    }

    #endregion

    #region PropertyMapName (internal)

    /// <summary>
    /// The PropertyMapName class (internal).
    /// </summary>
    internal class PropertyMapName
    {
        #region Internal Variables

        private Dictionary<string, PropertyMapInfo> _map = new Dictionary<string, PropertyMapInfo>();

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public PropertyMapName() : base()
        {
            MapNames = new List<string>();
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~PropertyMapName()
        {
            if (null != MapNames)
            {
                MapNames.Clear();
            }
            MapNames = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add.
        /// </summary>
        /// <param name="name">The map name</param>
        /// <param name="value">The PeropertyMapInfo instance for specificed map name.</param>
        public void Add(string name, PropertyMapInfo value)
        {
            if (!_map.ContainsKey(name)) _map.Add(name, value);
            if (!MapNames.Contains(name)) MapNames.Add(name); // keep map name.
            else _map[name] = value;
        }
        /// <summary>
        /// Contains Key.
        /// </summary>
        /// <param name="value">The name to check exist.</param>
        /// <returns>Returns true if name is exist.</returns>
        public bool ContainsKey(string value)
        {
            return _map.ContainsKey(value);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Indexer access.
        /// </summary>
        /// <param name="value">The map name.</param>
        /// <returns>
        /// Returns PeropertyMapInfo instance if found otherwise returns null.
        /// </returns>
        public PropertyMapInfo this[string value]
        {
            get
            {
                if (!_map.ContainsKey(value)) _map.Add(value, null);
                return _map[value];
            }
        }
        /// <summary>
        /// Gets List of Map Names.
        /// </summary>
        public List<string> MapNames { get; private set; }

        #endregion
    }

    #endregion

    #region PropertyMapInfo (internal)

    /// <summary>
    /// The PropertyMapInfo class (internal)
    /// </summary>
    internal class PropertyMapInfo
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public PropertyMapInfo() : base() { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="property">The property info.</param>
        /// <param name="baseType">The owner class base type.</param>
        public PropertyMapInfo(PropertyInfo property, Type baseType) : base()
        {
            this.Property = property;
            this.BaseType = baseType;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="property">The property info.</param>
        public PropertyMapInfo(PropertyInfo property) : this(property, null) { }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the property info instance.
        /// </summary>
        public PropertyInfo Property { get; set; }
        /// <summary>
        /// Gets or sets Base Type (of owner class)
        /// </summary>
        public Type BaseType { get; set; }

        #endregion
    }

    #endregion

    #region PeropertyMapNameExtensionMethods

    /// <summary>
    /// The PeropertyMapName Extension Methods.
    /// </summary>
    public static class PeropertyMapNameExtensionMethods
    {
        #region Internal Static Variables

        private static PeropertyMapNameCache _caches = new PeropertyMapNameCache();

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Assign from source object to target object that match all 
        /// property name attributes.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="target">The target instance.</param>
        public static void AssignTo<TSource, TTarget>(this TSource source, TTarget target)
        {
            if (null == source || null == target) return;
            Type scrType = source.GetType();
            Type dstType = target.GetType();
            PropertyMapName scrProp = _caches[scrType];
            PropertyMapName dstProp = _caches[dstType];
            PropertyMapInfo scrInfo, dstInfo;
            foreach (string name in scrProp.MapNames)
            {
                scrInfo = scrProp[name];
                dstInfo = dstProp[name];
                if (null == scrInfo || null == dstInfo) 
                    continue;
                if (scrInfo.Property == null || dstInfo.Property == null) 
                    continue;
                if (scrInfo.Property.PropertyType == null || 
                    dstInfo.Property.PropertyType == null)
                    continue;

                if (scrInfo.Property.PropertyType != dstInfo.Property.PropertyType) 
                    continue;

                // check dest type allow source type assigned.
                if (dstInfo.BaseType != null &&
                    scrType != dstInfo.BaseType && 
                    !scrType.IsSubclassOf(dstInfo.BaseType))
                {
                    //Console.WriteLine("Mismatch dest owner base type.");
                    continue;
                }
                var val = PropertyAccess.GetValue(source, scrInfo.Property.Name);
                PropertyAccess.SetValue(target, dstInfo.Property.Name, val);
            }
        }
        /// <summary>
        /// Clone source object to new target object that match all 
        /// property name attributes.
        /// </summary>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <returns>
        /// Returns new instance of TTarget with assigned all 
        /// properties that match all property name attributes.
        /// </returns>
        public static TTarget CloneTo<TTarget>(this object source)
            where TTarget : new()
        {
            if (null == source) return default;
            TTarget target = new TTarget();
            source.AssignTo(target);
            return target;
        }

        #endregion
    }

    #endregion
}
