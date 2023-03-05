#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-01-01
=================
- NLib Extension Methods for Collection and List added.
  - Add new Extenstion methods for work with Collection and List.
    - Add method: IsNullOrEmpty for check instance and elements.

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
    #region CollectionExtensionMethods

    /// <summary>
    /// The Extension methods for Collection and List.
    /// </summary>
    public static class CollectionExtensionMethods
    {
        #region ICollection

        /// <summary>
        /// Checks is specificed ICollection instance is null or empty list.
        /// </summary>
        /// <param name="value">An object variable.</param>
        /// <returns>Returns if the current ICollection instance is null or is empty.</returns>
        public static bool IsNullOrEmpty(this ICollection value)
        {
            return (null == value || value.Count <= 0);
        }

        #endregion
    }

    #endregion
}
