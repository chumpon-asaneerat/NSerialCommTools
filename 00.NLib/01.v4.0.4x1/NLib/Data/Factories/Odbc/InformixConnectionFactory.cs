#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-11-30
=================
- Data Access Framework - InformixConnectionFactory
  - Redesign InformixConnectionFactory class with new implements. Note. In near future the
    base class will changed to OleDb and add code for format DateTime to informix query.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Data;
using System.Globalization;

#endregion

namespace NLib.Data
{
    #region Informix Connection Factory

    /// <summary>
    /// Informix Connection Factory.
    /// </summary>
    public class InformixConnectionFactory : OdbcConnectionFactory
    {
        #region Abstract/Virtual Overrides

        #region InitFormatter

        /// <summary>
        /// Init formatter.
        /// </summary>
        /// <param name="formatter">The formatter instance.</param>
        protected override void InitFormatter(NDbFormatter formatter)
        {
            #region Checks

            if (null == formatter)
            {
                "formatter is null".Err();
                return;
            }

            #endregion

            base.InitFormatter(formatter); // call base class for init formatter.

            #region Init delegates

            formatter.GetTestQuery = () =>
            {
                return "SELECT 1";
            };
            formatter.GetServerDateQuery = () =>
            {
                return string.Empty;
            };
            formatter.FormatTableOrViewName = (string owner, string objectName) =>
            {
                if (string.IsNullOrWhiteSpace(objectName))
                {
                    "Table's Name is not assigned.".Info();
                    return string.Empty;
                }
                return string.Format("[{0}]", objectName);
            };
            formatter.FormatColumnName = (string owner, string objectName, string columnName) =>
            {
                if (string.IsNullOrWhiteSpace(objectName) &&
                    string.IsNullOrWhiteSpace(columnName))
                {
                    "Table's Name and Column's Name is not assigned.".Info();
                    return string.Empty;
                }
                if (objectName.Trim().Length <= 0)
                    return string.Format("[{0}]", columnName);
                else return string.Format("[{0}].[{1}]", objectName, columnName);
            };

            #endregion
        }

        #endregion

        #endregion
    }

    #endregion
}

namespace NLib.Data
{
    #region Informix Tasks

    #endregion
}

