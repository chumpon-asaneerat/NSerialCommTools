#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-17
=================
- NLib Extension Methods for DataRow/DataRowView updated.
  - Changes log code (used MethodBase).

======================================================================================================================
Update 2014-10-15
=================
- NLib Extension Methods for DataRow/DataRowView added.
    - Add method: As<T> which convert value from each column in DataRow/DataRowView to
      target type T.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

#endregion

namespace NLib
{
    #region NDataRowExtensions

    /// <summary>
    /// The NLib DataRow Extensions.
    /// </summary>
    public static class NDataRowExtensions
    {
        /// <summary>
        /// Convert value of column in target data row. 
        /// If the T is Nullable type the DBNull value can returns as output otherwise
        /// the defaultValue will return instead.
        /// </summary>
        /// <typeparam name="T">The terget data type.</typeparam>
        /// <param name="row">The target data row.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns value in column on target data row.</returns>
        public static T As<T>(this DataRow row,
            string columnName, T defaultValue = default(T))
        {
            Type type = typeof(T);
            T result = defaultValue;
            MethodBase med = MethodBase.GetCurrentMethod();
            if (null != row)
            {
                try
                {
                    if (type.IsGenericType &&
                        type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        // Nullable type so DBNull can direct convert.
                        result = row.Field<T>(columnName);
                    }
                    else if (type.IsValueType)
                    {
                        if (!row.IsNull(columnName))
                        {
                            // allow only data that is not DBNull
                            // Note. Some type like decimal cannot direct cast T on row so
                            // need to used Convert class instead direct cast.
                            result = (T)Convert.ChangeType(row[columnName], type);
                        }
                    }
                    else
                    {
                        if (!row.IsNull(columnName))
                        {
                            // allow only data that is not DBNull
                            result = row.Field<T>(columnName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.Err(med);
                    // error assigned default value.
                    result = defaultValue;
                }
            }

            return result;
        }
        /// <summary>
        /// Convert value of column in target data row view. 
        /// If the T is Nullable type the DBNull value can returns as output otherwise
        /// the defaultValue will return instead.
        /// </summary>
        /// <typeparam name="T">The terget data type.</typeparam>
        /// <param name="row">The target data row view.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns value in column on target data row view.</returns>
        public static T As<T>(this DataRowView row,
            string columnName, T defaultValue = default(T))
        {
            Type type = typeof(T);
            T result = defaultValue;
            MethodBase med = MethodBase.GetCurrentMethod();

            if (null != row && null != row.Row)
            {
                try
                {
                    if (type.IsGenericType &&
                        type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        // Nullable type so DBNull can direct convert.
                        result = row.Row.Field<T>(columnName);
                    }
                    else if (type.IsValueType)
                    {
                        if (!row.Row.IsNull(columnName))
                        {
                            // allow only data that is not DBNull
                            // Note. Some type like decimal cannot direct cast T on row so
                            // need to used Convert class instead direct cast.
                            result = (T)Convert.ChangeType(row.Row[columnName], type);
                        }
                    }
                    else
                    {
                        if (!row.Row.IsNull(columnName))
                        {
                            // allow only data that is not DBNull
                            result = row.Row.Field<T>(columnName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.Err(med);
                    // error assigned default value.
                    result = defaultValue;
                }
            }

            return result;
        }
    }

    #endregion
}
