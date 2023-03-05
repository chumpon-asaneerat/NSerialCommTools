#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-01-01
=================
- NLib Extension Methods for Object added.
  - Add new Extenstion methods for work with object.
    - Add method: IsNull for check instance.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace NLib
{
    #region ObjectExtensionMethods

    /// <summary>
    /// The Extension methods for Object.
    /// </summary>
    public static class ObjectExtensionMethods
    {
        #region Objects

        /// <summary>
        /// Checks is object is null.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">An object variable.</param>
        /// <returns>Returns if the object is null.</returns>
        public static bool IsNull<T>(this T value)
        {
            // used <T> instead of System.Object to prevent penalty when
            //  pass a value type in then the value type will be boxed. 
            // That creates a performance penalty of allocating the box and doing the copy, 
            // plus of course later having to garbage collect the box.

            return (null == value);
        }
        /// <summary>
        /// Get the value. If value is null new instance will automatically create and returns otherwise
        /// the value itself will returns. The Value need to be class and supports only class that has 
        /// default constructor only. Note. This method will lock the inst variable for make it thread 
        /// safe.
        /// </summary>
        /// <typeparam name="I">The target instance.</typeparam>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="inst">The instance.</param>
        /// <param name="value">The value to ckecks normally is local variable.</param>
        /// <returns>
        /// Returns value if the value is not null or return new instance of T if value is null.
        /// </returns>
        public static T GetValueOrDefault<I, T>(this I inst, T value)
            where I : class
            where T : class, new()
        {
            if (null == inst)
                throw new ArgumentException("The call instance is null.");
            T result = value;
            if (null == value)
            {
                lock (inst)
                {
                    result = new T();
                }
            }
            return result;
        }

        #endregion
    }

    #endregion
}
