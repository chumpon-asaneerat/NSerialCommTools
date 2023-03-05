#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2011-11-07
=================
- NETTypeCache class ported from GFA Library GFA40 to NLib 

======================================================================================================================
Update 2010-01-23
=================
- NETTypeCache class ported from GFA Library GFA37 to GFA38v3
  - Changed internal Hashtable to Dictionary<>
  - Changed all exception handling code to used new ExceptionManager class instread of 
    LogManager.

======================================================================================================================
Update 2007-11-26
=================
- internal NETTypeCache class add some fixed for multithread environment and optimized
  for performance

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

#endregion

namespace NLib.Utils
{
    #region NET Type Cache

    /// <summary>
    /// NET Type Cache
    /// </summary>
    public sealed class NETTypeCache
    {
        #region Singleton

        private static NETTypeCache _instance = null;
        private NETTypeCache() { }
        /// <summary>
        /// Singelton Instance Access
        /// </summary>
        public static NETTypeCache Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(NETTypeCache))
                    {
                        _instance = new NETTypeCache();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region For Type Access

        private Dictionary<string, Type> _hash = new Dictionary<string, Type>();

        /// <summary>
        /// Find Type but not keep in cache
        /// </summary>
        /// <param name="typeName">Type name to find.</param>
        /// <returns>Returns match Type if found. Otherwise return null.</returns>
        public Type FindType(string typeName)
        {
            if (typeName.Length <= 0)
                return null;
            bool isArray = (typeName.IndexOf("[]") != -1) ? true : false;

            Assembly[] assems = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assem in assems)
            {
                Type[] types = assem.GetTypes();
                if (null == types || types.Length <= 0) continue;
                foreach (Type type in types)
                {
                    if (typeName == type.FullName)
                    {
                        return type;
                    }
                    else if (isArray)
                    {
                        if (typeName.StartsWith(type.FullName))
                        {
                            return Type.GetType(type.FullName + "[]");
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Check is type is contains
        /// </summary>
        /// <param name="typeName">The Type name to checks.</param>
        /// <returns>Returns true if type is already in cache.</returns>
        public bool Contains(string typeName)
        {
            return _hash.ContainsKey(typeName);
        }
        /// <summary>
        /// Append type into hahs
        /// </summary>
        /// <param name="type">The type to add.</param>
        public void Add(Type type)
        {
            if (!_hash.ContainsKey(type.FullName)) _hash.Add(type.FullName, type);
        }
        /// <summary>
        /// Indexer access for Type.
        /// </summary>
        /// <param name="typeName">Type name to find.</param>
        /// <returns>Returns match Type if found. Otherwise return null.</returns>
        public Type this[string typeName]
        {
            get
            {
                if (!_hash.ContainsKey(typeName))
                    return null;
                return _hash[typeName];
            }
        }

        #endregion
    }

    #endregion
}
