#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2010-07-23
=================
- TypeUtils class changed.
  - Change all ArrayList to List<T>.
  - Change all Hashtable to Dictionary<TKey, TValue>.
  - Add lock in singelton access.

======================================================================================================================
Update 2010-01-25
=================
- Rewrite code for TypeUtils class.

======================================================================================================================
// </[History]>

#endif
#endregion

#define ENABLE_DEBUG

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;

#endregion

namespace NLib.Reflection
{
    #region TypeUtils - <<<< Require Cache and Optimize >>>>

    /// <summary>
    /// Type utils is utility that used for implement abstract type (class) that required to reflection
    /// metadata about it's ansessor information and creation
    /// </summary>
    public class TypeUtils
    {
        #region Internal Class

        /// <summary>
        /// Type Information (used internal only)
        /// </summary>
        class TypeInformation
        {
            #region Internal Variable

            private Type _baseType = null;
            private List<Type> _inheritedTypeList = new List<Type>();

            #endregion

            #region Constructor and Destructor

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="baseType">Base Type</param>
            public TypeInformation(Type baseType)
            {
                _baseType = baseType;
            }
            /// <summary>
            /// Destructor
            /// </summary>
            ~TypeInformation()
            {
                if (_inheritedTypeList != null) _inheritedTypeList.Clear();
                _inheritedTypeList = null;
                _baseType = null;
            }

            #endregion

            #region Get Inherited Type Reference's Names

            /// <summary>
            /// Get Inherited Type Reference's Names
            /// </summary>
            /// <param name="staticPropertyName">(static) Property's Name</param>
            /// <returns>Type's Name that inhreited from specificed type</returns>
            public string[] GetInheritedTypeNames(string staticPropertyName)
            {
                Type[] types = this.GetInheritedTypes();
                if (types == null || types.Length <= 0)
                    return new string[0];

                List<string> list = new List<string>();
                foreach (Type type in types)
                {
                    PropertyInfo propInfo = type.GetProperty(staticPropertyName,
                        BindingFlags.Static | BindingFlags.Public);
                    if (propInfo == null)
                        continue;
                    else
                    {
                        object val = propInfo.GetValue(type, Type.EmptyTypes);
                        list.Add(val.ToString());
                    }
                }
                string[] results = list.ToArray();
                list.Clear();
                list = null;

                return results;
            }

            #endregion

            #region Get Inherited Types

            /// <summary>
            /// Get Inherited Types
            /// </summary>
            /// <returns>list of all inherited type in cache</returns>
            public Type[] GetInheritedTypes()
            {
                return this.GetInheritedTypes(false);
            }
            /// <summary>
            /// Get Inherited Types
            /// </summary>
            /// <param name="refresh">refresh cache</param>
            /// <returns>list of all inherited type</returns>
            public Type[] GetInheritedTypes(bool refresh)
            {
                Assembly[] assems = AppDomain.CurrentDomain.GetAssemblies();
                if (assems == null) return Type.EmptyTypes;
                if (_inheritedTypeList == null)
                    _inheritedTypeList = new List<Type>();

                if (refresh) _inheritedTypeList.Clear();
#if !ENABLE_DEBUG
                DebugManager.Instance.Info(
                    string.Format("GetInheritedTypes No of assemblies : {0}", assems.Length),
                    this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name);
#endif
                foreach (Assembly assem in assems)
                {
                    if (null == assem)
                    {
#if !ENABLE_DEBUG
                        DebugManager.Instance.Info(
                            "GetInheritedTypes found null assembly",
                            this.GetType().Name,
                            MethodBase.GetCurrentMethod().Name);
#endif
                        continue;
                    }
                    else
                    {
#if !ENABLE_DEBUG
                        DebugManager.Instance.Info(
                            string.Format("Process assembly : {0}", assem.FullName),
                            this.GetType().Name,
                            MethodBase.GetCurrentMethod().Name);
#endif
                    }
                    Type[] types = null;
                    try
                    {
                        types = assem.GetTypes();
                    }
                    catch (Exception ex)
                    {
#if !ENABLE_DEBUG
                        DebugManager.Instance.Error(
                            ex,
                            this.GetType().Name,
                            MethodBase.GetCurrentMethod().Name);
#else
                        Console.WriteLine(ex);
#endif
                    }

                    if (types == null || types.Length <= 0)
                        continue;

                    foreach (Type type in types)
                    {
                        if (type.IsAbstract)
                            continue;
                        if (!type.IsSubclassOf(_baseType))
                            continue;
                        if (!_inheritedTypeList.Contains(type))
                            _inheritedTypeList.Add(type);
                    }
                }

                Type[] results = _inheritedTypeList.ToArray();

                return results;
            }

            #endregion

            #region Create Instance

            /// <summary>
            /// Create New Instance by reference name
            /// </summary>
            /// <param name="staticPropertyName">(static) Property's Name</param>
            /// <param name="refName">reference's name</param>
            /// <param name="createInstanceMethodName">Method's name that used to create instance (this method should has no parameter</param>
            /// <returns>New Object Instance</returns>
            public object Create(string staticPropertyName, string refName, string createInstanceMethodName)
            {
                object instance = null;
                Type[] types = this.GetInheritedTypes();
                if (types == null || types.Length <= 0)
                    instance = null;
                else
                {
                    Type targetType = null;
                    foreach (Type type in types)
                    {
                        PropertyInfo propInfo = type.GetProperty(staticPropertyName,
                            BindingFlags.Static | BindingFlags.Public);
                        if (propInfo == null)
                            continue;
                        else
                        {
                            object val = propInfo.GetValue(type, Type.EmptyTypes);
                            if (val != null && val.ToString() == refName)
                            {
                                targetType = type;
                                break;
                            }
                        }
                    }
                    if (targetType != null)
                    {
                        MethodInfo medInfo = targetType.GetMethod(createInstanceMethodName,
                            BindingFlags.Public | BindingFlags.Static);
                        if (medInfo != null)
                        {
                            instance = medInfo.Invoke(targetType, Type.EmptyTypes);
                        }
                    }
                    else
                    {
                    }
                }
                return instance;
            }

            #endregion
        }

        #endregion

        #region Hide Constructor

        private TypeUtils() { }

        #endregion

        #region Singelton

        private static TypeUtils _instance = null;
        /// <summary>
        /// Singelton Instance Access
        /// </summary>
        private static TypeUtils Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(TypeUtils))
                    {
                        _instance = new TypeUtils();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Internal Variable

        private Dictionary<Type, TypeUtils.TypeInformation> _baseTypeHash =
            new Dictionary<Type, TypeUtils.TypeInformation>();

        #endregion

        #region Private Access

        private TypeUtils.TypeInformation GetTypeInfo(Type baseType)
        {
            if (baseType == null)
                return null;
            if (_baseTypeHash.ContainsKey(baseType))
            {
                return (TypeUtils.TypeInformation)_baseTypeHash[baseType];
            }
            else return null;
        }

        private TypeUtils.TypeInformation AddNewType(Type baseType)
        {
            if (baseType == null)
                return null;
            if (_baseTypeHash.ContainsKey(baseType))
                return null; // Not allow to add type that exists in hash
            lock (this)
            {
                _baseTypeHash.Add(baseType, new TypeUtils.TypeInformation(baseType));
            }
            return GetTypeInfo(baseType);
        }

        private bool Contains(Type baseType)
        {
            if (baseType == null)
                return false;
            else return _baseTypeHash.ContainsKey(baseType);
        }

        private TypeUtils.TypeInformation this[Type baseType]
        {
            get
            {
                if (!Contains(baseType))
                {
                    AddNewType(baseType);
                }
                return GetTypeInfo(baseType);
            }
        }

        #endregion

        #region Static Method

        #region Get Inherited Type Reference's Names

        /// <summary>
        /// Get Inherited Type Reference's Names
        /// </summary>
        /// <param name="baseType">Base Type</param>
        /// <param name="staticPropertyName">(static) Property's Name</param>
        /// <returns>Type's Name that inhreited from specificed type</returns>
        public static string[] GetInheritedTypeNames(Type baseType, string staticPropertyName)
        {
            TypeUtils.TypeInformation typeInfo = TypeUtils.Instance[baseType];
            if (typeInfo == null)
                return null;
            else return typeInfo.GetInheritedTypeNames(staticPropertyName);
        }

        #endregion

        #region Get Inherited Types

        /// <summary>
        /// Get Inherited Types
        /// </summary>
        /// <param name="baseType">Base Type</param>
        /// <returns>list of all inherited type in cache</returns>
        public static Type[] GetInheritedTypes(Type baseType)
        {
            TypeUtils.TypeInformation typeInfo = TypeUtils.Instance[baseType];
            if (typeInfo == null)
                return null;
            else return typeInfo.GetInheritedTypes();
        }
        /// <summary>
        /// Get Inherited Types
        /// </summary>
        /// <param name="baseType">Base Type</param>
        /// <param name="refresh">refresh cache</param>
        /// <returns>list of all inherited type</returns>
        public static Type[] GetInheritedTypes(Type baseType, bool refresh)
        {
            TypeUtils.TypeInformation typeInfo = TypeUtils.Instance[baseType];
            if (typeInfo == null)
                return null;
            else return typeInfo.GetInheritedTypes(refresh);
        }

        #endregion

        #region Create Instance

        /// <summary>
        /// Create New Instance by reference name
        /// </summary>
        /// <param name="baseType">Base Type</param>
        /// <param name="staticPropertyName">(static) Property's Name</param>
        /// <param name="refName">reference's name</param>
        /// <param name="createInstanceMethodName">Method's name that used to create instance (this method should has no parameter</param>
        /// <returns>New Object Instance</returns>
        public static object Create(Type baseType, string staticPropertyName, string refName,
            string createInstanceMethodName)
        {
            TypeUtils.TypeInformation typeInfo = TypeUtils.Instance[baseType];
            if (typeInfo == null)
            {
                return null;
            }
            else
            {
                return typeInfo.Create(staticPropertyName, refName, createInstanceMethodName);
            }
        }

        #endregion

        #region Get Reference Name

        /// <summary>
        /// Get Reference Name
        /// </summary>
        /// <param name="instance">Instance to read information</param>
        /// <param name="staticPropertyName">(static) Property's Name to call</param>
        /// <returns>return Reference's Name</returns>
        public static string GetReferenceName(object instance, string staticPropertyName)
        {
            if (instance == null)
                return string.Empty;
            Type type = instance.GetType();
            PropertyInfo propInfo = type.GetProperty(staticPropertyName,
                BindingFlags.Static | BindingFlags.Public);
            if (propInfo == null)
                return "";
            else
            {
                object val = propInfo.GetValue(type, Type.EmptyTypes);
                if (val != null)
                {
                    return val.ToString();
                }
                else return "";
            }
        }

        #endregion

        #region Get Attributes

        /// <summary>
        /// Get Attributes
        /// </summary>
        /// <param name="componentType">Type to find property attribute</param>
        /// <param name="attributeType">target attribute's type</param>
        /// <returns>Attribute instance that assigned to specificed type</returns>
        public static Attribute[] GetAttributes(Type componentType, Type attributeType)
        {
            if (componentType == null || attributeType == null)
                return null;

            Attribute[] results = null;

            /*
            // Used Type Descriptor
            AttributeCollection attrCols = TypeDescriptor.GetAttributes(componentType);
            if (attrCols != null && attrCols.Count > 0)
            {
                ArrayList list = new ArrayList();
                foreach (Attribute attr in attrCols)
                {
                    if (attr.GetType() == attributeType ||
                        attr.GetType().IsSubclassOf(attributeType)) list.Add(attr);
                }
                results = (Attribute[] )list.ToArray(attributeType);
            }
            */
            // Used Reflection if Type descriptor not found
            if (results == null)
            {
                object[] attrs = componentType.GetCustomAttributes(attributeType, true);
                if (attrs != null && attrs.Length > 0)
                {
                    results = (attrs as Attribute[]);
                }
            }

            return results;
        }

        #endregion

        #region Get Attribute

        /// <summary>
        /// Get Attribute
        /// </summary>
        /// <param name="componentType">Type to find property attribute</param>
        /// <param name="attributeType">target attribute's type</param>
        /// <returns>Attribute instance that assigned to specificed type</returns>
        public static Attribute GetAttribute(Type componentType, Type attributeType)
        {
            if (componentType == null || attributeType == null)
                return null;

            Attribute result = null;

            // Used Type Descriptor
            AttributeCollection attrCols = TypeDescriptor.GetAttributes(componentType);
            if (attrCols != null && attrCols.Count > 0)
            {
                result = attrCols[attributeType];
            }
            // Used Reflection if Type descriptor not found
            if (result == null)
            {
                object[] attrs = componentType.GetCustomAttributes(attributeType, true);
                if (attrs != null && attrs.Length > 0)
                {
                    result = (attrs[0] as Attribute);
                }
            }

            return result;
        }
        /// <summary>
        /// Get Propertyt Attribute
        /// </summary>
        /// <param name="componentType">Type to find property attribute</param>
        /// <param name="propertyName">Property's Name</param>
        /// <param name="attributeType">target attribute's type</param>
        /// <returns>Attribute instance that assigned to specificed type</returns>
        public static Attribute GetPropertytAttribute(Type componentType, string propertyName, Type attributeType)
        {
            if (componentType == null || attributeType == null || propertyName.Length <= 0)
                return null;

            Attribute result = null;

            // Used Type Descriptor
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(componentType);
            if (props != null && props.Count > 0)
            {
                if (props[propertyName] != null)
                {
                    result = props[propertyName].Attributes[attributeType];
                }
            }
            // Used Reflection if Type descriptor not found
            if (result == null)
            {
                PropertyInfo info = componentType.GetProperty(propertyName);
                if (info != null)
                {
                    object[] attrs = info.GetCustomAttributes(attributeType, true);
                    if (attrs != null && attrs.Length > 0)
                    {
                        result = (attrs[0] as Attribute);
                    }
                }
            }

            return result;
        }

        #endregion

        #endregion
    }

    #endregion
}
