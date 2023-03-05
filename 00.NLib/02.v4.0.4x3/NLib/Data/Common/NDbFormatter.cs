#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-07-22
=================
- Data Access Framework - NDbFormatter.
  - Seperate RequiredSPParameterPrefix delegate to RequiredSPInputParameterPrefix, 
    RequiredSPOutputParameterPrefix with rewrite related code that call the previous one.
======================================================================================================================
Update 2014-11-30
=================
- Data Access Framework - NDbFormatter
  - Redesign Formatter class by using delegate property for allow more customizable by
    user code.
======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.ComponentModel;
using System.Globalization;

#endregion

namespace NLib.Data
{
    /// <summary>
    /// NDbFormatter class. Provide ability to format various value that can changed later by
    /// the factory class.
    /// </summary>
    public class NDbFormatter
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NDbFormatter()
            : base()
        {
            InitDefaultDelegates();
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~NDbFormatter()
        {
        }

        #endregion

        #region Init Default delegate(s)

        private void InitDefaultDelegates()
        {
            this.GetTestQuery = () =>
            {
                return "SELECT 1";
            };
            this.GetServerDateQuery = () =>
            {
                return string.Empty;
            };
            this.SelectIdentityQuery = (string tableName, string columnName) =>
            {
                return this.SelectMax(tableName, null, null, columnName, string.Empty);
            };
            this.SelectMaxQuery = (string tableName, string[] compoundKeys,
                string[] compoundValues, string columnName, string criteria) =>
            {
                if (string.IsNullOrWhiteSpace(tableName))
                    return string.Empty;
                if (null == compoundKeys && null != compoundValues)
                    return string.Empty;
                if (null != compoundKeys && null == compoundValues)
                    return string.Empty;
                if (null != compoundKeys && null != compoundValues &&
                    compoundKeys.Length != compoundValues.Length) return string.Empty;
                if (string.IsNullOrWhiteSpace(columnName))
                    return string.Empty;

                int iKey = 0;
                if (null != compoundKeys) 
                    iKey = compoundKeys.Length;

                string query = string.Empty;
                query += "SELECT MAX(" + columnName + ") FROM " + 
                    this.FormatTableOrViewName(string.Empty, tableName);

                if (null != compoundKeys && compoundKeys.Length > 0)
                {
                    query += " WHERE ";
                    for (int i = 0; i < iKey; i++)
                    {
                        string key = compoundKeys[i];
                        string val = compoundValues[i];
                        query += "(" + key + " = " + val + ")";
                        if (i < (iKey - 1)) query += " AND ";
                    }

                    if (!string.IsNullOrWhiteSpace(criteria))
                    {
                        query += " AND (" + criteria + ")";
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(criteria))
                    {
                        query += " where (" + criteria + ")";
                    }
                }

                return query;
            };
            this.GetParameterPrefix = () =>
            {
                return string.Empty;
            };
            this.RequiredSPInputParameterPrefix = () =>
            {
                return true;
            };
            this.RequiredSPOutputParameterPrefix = () =>
            {
                return true;
            };
            this.GetDefaultReturnParameterName = () =>
            {
                return GetParameterPrefix() + "RETURN_VALUE";
            };
            // Format delegates
            this.FormatParameterName = (string parameterName) =>
            {
                string result = parameterName
                    .Replace("?", "")
                    .Replace("@", "")
                    .Replace(":", "");
                return GetParameterPrefix() + result;
            };
            this.FormatTableOrViewName = (string owner, string objectName) =>
            {
                if (string.IsNullOrWhiteSpace(objectName))
                    return objectName; // No object name assigned.

                if (string.IsNullOrWhiteSpace(owner))
                    return string.Format("[{0}]", objectName);
                else return string.Format("[{0}].[{1}]", owner, objectName);
            };
            this.FormatColumnName = (string owner, string objectName, string columnName) =>
            {
                if (string.IsNullOrWhiteSpace(columnName))
                    return columnName; // No column name assigned.

                if (string.IsNullOrWhiteSpace(objectName))
                    return columnName; // No object name assigned.

                // Both object name and column name is exists.
                if (string.IsNullOrWhiteSpace(owner))
                    return string.Format("[{0}].[{1}]", objectName, columnName);
                else return string.Format("[{0}].[{1}].[{2}]", owner, objectName, columnName);
            };
            // ToSqlString delegates
            this.DateTimeToSqlString = (DateTime value, bool dateOnly) =>
            {
                if (dateOnly)
                {
                    string format = "dd/MM/yyyy";
                    return "'" + value.ToString(format, DateTimeFormatInfo.InvariantInfo) + "'";
                }
                else
                {
                    string format = "dd/MM/yyyy HH:mm:ss";
                    return "'" + value.ToString(format, DateTimeFormatInfo.InvariantInfo) + "'";
                }
            };
            this.StringToSqlString = (string value) =>
            {
                return "'" + value.ToString() + "'";
            };
            this.DecimalToSqlString = (decimal value) =>
            {
                return value.ToString();
            };
            this.FloatToSqlString = (float value) =>
            {
                return value.ToString();
            };
            this.SByteToSqlString = (sbyte value) =>
            {
                return value.ToString();
            };
            this.Int16ToSqlString = (short value) =>
            {
                return value.ToString();
            };
            this.Int32ToSqlString = (int value) =>
            {
                return value.ToString();
            };
            this.Int64ToSqlString = (long value) =>
            {
                return value.ToString();
            };
            this.ByteToSqlString = (byte value) =>
            {
                return value.ToString();
            };
            this.UInt16ToSqlString = (ushort value) =>
            {
                return value.ToString();
            };
            this.UInt32ToSqlString = (uint value) =>
            {
                return value.ToString();
            };
            this.UInt64ToSqlString = (ulong value) =>
            {
                return value.ToString();
            };
            this.BooleanToSqlString = (bool value) =>
            {
                if (value) return "1";
                else return "0";
            };
            this.GuidToSqlString = (Guid value) =>
            {
                return "'" + value.ToString() + "'";
            };
        }

        #endregion

        #region Wrapper Methods

        #region ToSqlString

        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(DateTime value)
        {
            return this.ToSqlString(value, false); // Date and Time.
        }
        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <param name="dateOnly">Use only date part.</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(DateTime value, bool dateOnly)
        {
            #region Checks
            
            if (null == this.DateTimeToSqlString)
            {
                return string.Empty;
            }

            #endregion

            return this.DateTimeToSqlString(value, dateOnly);
        }
        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(string value)
        {
            #region Checks

            if (null == this.StringToSqlString)
            {
                return string.Empty;
            }

            #endregion

            return this.StringToSqlString(value);
        }
        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(decimal value)
        {
            #region Checks

            if (null == this.DecimalToSqlString)
            {
                return string.Empty;
            }

            #endregion

            return this.DecimalToSqlString(value);
        }
        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(float value)
        {
            #region Checks

            if (null == this.FloatToSqlString)
            {
                return string.Empty;
            }

            #endregion

            return this.FloatToSqlString(value);
        }
        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(sbyte value)
        {
            #region Checks

            if (null == this.SByteToSqlString)
            {
                return string.Empty;
            }

            #endregion

            return this.SByteToSqlString(value);
        }
        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(short value)
        {
            #region Checks

            if (null == this.Int16ToSqlString)
            {
                return string.Empty;
            }

            #endregion

            return this.Int16ToSqlString(value);
        }
        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(int value)
        {
            #region Checks

            if (null == this.Int32ToSqlString)
            {
                return string.Empty;
            }

            #endregion

            return this.Int32ToSqlString(value);
        }
        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(long value)
        {
            #region Checks

            if (null == this.Int64ToSqlString)
            {
                return string.Empty;
            }

            #endregion

            return this.Int64ToSqlString(value);
        }
        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(byte value)
        {
            #region Checks

            if (null == this.ByteToSqlString)
            {
                return string.Empty;
            }

            #endregion

            return this.ByteToSqlString(value);
        }
        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(ushort value)
        {
            #region Checks

            if (null == this.UInt16ToSqlString)
            {
                return string.Empty;
            }

            #endregion

            return this.UInt16ToSqlString(value);
        }
        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(uint value)
        {
            #region Checks

            if (null == this.UInt32ToSqlString)
            {
                return string.Empty;
            }

            #endregion

            return this.UInt32ToSqlString(value);
        }
        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(ulong value)
        {
            #region Checks

            if (null == this.UInt64ToSqlString)
            {
                return string.Empty;
            }

            #endregion

            return this.UInt64ToSqlString(value);
        }
        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(bool value)
        {
            #region Checks

            if (null == this.BooleanToSqlString)
            {
                return string.Empty;
            }

            #endregion

            return this.BooleanToSqlString(value);
        }
        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(Guid value)
        {
            #region Checks

            if (null == this.GuidToSqlString)
            {
                return string.Empty;
            }

            #endregion

            return this.GuidToSqlString(value);                
        }
        /// <summary>
        /// ToSqlString.
        /// </summary>
        /// <param name="value">object to convert to string</param>
        /// <returns>string that can used in sql statement</returns>
        public string ToSqlString(object value)
        {
            if (value == null)
                return string.Empty;
            Type type = value.GetType();
            if (type == typeof(string)) return ToSqlString((string)value);
            else if (type == typeof(string)) return ToSqlString((string)value);
            else if (type == typeof(Guid)) return ToSqlString((Guid)value);
            else if (type == typeof(bool)) return ToSqlString((bool)value);
            else if (type == typeof(UInt64)) return ToSqlString((UInt64)value);
            else if (type == typeof(UInt32)) return ToSqlString((UInt32)value);
            else if (type == typeof(UInt16)) return ToSqlString((UInt16)value);
            else if (type == typeof(Int64)) return ToSqlString((Int64)value);
            else if (type == typeof(int)) return ToSqlString((int)value);
            else if (type == typeof(float)) return ToSqlString((float)value);
            else if (type == typeof(decimal)) return ToSqlString((decimal)value);
            else if (type == typeof(DateTime)) return ToSqlString((DateTime)value);
            else return string.Empty;
        }

        #endregion

        #region Select Queries

        /// <summary>
        /// Select Identity.
        /// </summary>
        /// <param name="tableName">Target table's name.</param>
        /// <param name="columnName">Target column's name.</param>
        /// <returns>Returns query that used to find identity value for specificed column.</returns>
        public string SelectIdentity(string tableName, string columnName)
        {
            if (null == SelectIdentityQuery)
                return string.Empty;
            string result = SelectIdentityQuery(tableName, columnName);
            return result;
        }
        /// <summary>
        /// Select Max(FieldName) - should call before inserted.
        /// </summary>
        /// <param name="tableName">Target Table.</param>
        /// <param name="compoundKeys">Compound Keys not include target Field Name.</param>
        /// <param name="compoundValues">Compound Key values.</param>
        /// <param name="columnName">Target Column Name to get max value.</param>
        /// <param name="criteria">Optional Criteria.</param>
        /// <returns>Returns the qyert for select max.</returns>
        public string SelectMax(string tableName, string[] compoundKeys,
            string[] compoundValues, string columnName, string criteria)
        {
            if (null == SelectMaxQuery)
                return string.Empty;
            string result = SelectMaxQuery(tableName, compoundKeys, compoundValues,
                columnName, criteria);
            return result;
        }

        #endregion

        #endregion

        #region Delegate Properties

        #region TestQuery

        /// <summary>
        /// Gets or sets delegate method for get query that used in test connection process.
        /// </summary>
        [Category("Testings")]
        [Description("Gets or sets delegate method for get query that used in test connection process.")]
        public Func<string> GetTestQuery { get; set; }

        #endregion

        #region GetServerDataQuery

        /// <summary>
        /// Gets or sets delegate method for get query for get server datetime.
        /// </summary>
        [Category("Testings")]
        [Description("Gets or sets delegate method for get query for get server datetime.")]
        public Func<string> GetServerDateQuery { get; set; }

        #endregion

        #region Stored Procedure(s)

        /// <summary>
        /// Gets or sets delegate method for get prefix for stored procedure's parameter.
        /// The delegate should returns the stored procedure's parameter prefix as result.
        /// </summary>
        [Category("Stored Procedures")]
        [Description("Gets or sets delegate method for get prefix for stored procedure's parameter.")]
        public Func<string> GetParameterPrefix { get; set; }
        /// <summary>
        /// Gets or sets delegate method for get default parameter name when call stored procedure.
        /// The delegate should returns the default stored procedure's parameter name as result.
        /// </summary>
        [Category("Stored Procedures")]
        [Description("Gets or sets delegate method for get default parameter name when call stored procedure.")]
        public Func<string> GetDefaultReturnParameterName { get; set; }
        /// <summary>
        /// Gets or sets delegate method for format parameter's name (normally add prefix befor name).
        /// The delegate input is parameter name (without prefix) and result is auto append prefix in front
        /// of the name.
        /// </summary>
        [Category("Stored Procedures")]
        [Description("Gets or sets delegate method for format parameter's name (normally add prefix befor name).")]
        public Func<string, string> FormatParameterName { get; set; }
        /// <summary>
        /// Gets or sets delegate method for check need prefix for stored procedure's input parameter.
        /// The delegate should returns true or false. Default is true.
        /// </summary>
        [Category("Stored Procedures")]
        [Description("Gets or sets delegate method for check need prefix for stored procedure's input parameter.")]
        public Func<bool> RequiredSPInputParameterPrefix { get; set; }
        /// <summary>
        /// Gets or sets delegate method for check need prefix for stored procedure's output parameter.
        /// The delegate should returns true or false. Default is true.
        /// </summary>
        [Category("Stored Procedures")]
        [Description("Gets or sets delegate method for check need prefix for stored procedure's output parameter.")]
        public Func<bool> RequiredSPOutputParameterPrefix { get; set; }

        #endregion

        #region Table/View/Column

        /// <summary>
        /// Gets or sets delegate method for format Table or View Name.
        /// The delegate input is owner, table name and result is 
        /// full format table name.
        /// </summary>
        [Category("Table, View and Column")]
        [Description("Gets or sets delegate method for format Table or View Name.")]
        public Func<string, string, string> FormatTableOrViewName { get; set; }
        /// <summary>
        /// Gets or sets delegate method for format Column's Name.
        /// The delegate input is owner, table name, column name and result is 
        /// full format column name.
        /// </summary>
        [Category("Table, View and Column")]
        [Description("Gets or sets delegate method for format Column's Name.")]
        public Func<string, string, string, string> FormatColumnName { get; set; }

        #endregion

        #region ToSqlString

        /// <summary>
        /// Gets or sets delegate method for format value to string that can used in sql query.
        /// </summary>
        [Category("To SQL String")]
        [Description("Gets or sets delegate method for format value to string that can used in sql query.")]
        public Func<DateTime, bool, string> DateTimeToSqlString { get; set; }
        /// <summary>
        /// Gets or sets delegate method for format value to string that can used in sql query.
        /// </summary>
        [Category("To SQL String")]
        [Description("Gets or sets delegate method for format value to string that can used in sql query.")]
        public Func<string, string> StringToSqlString { get; set; }
        /// <summary>
        /// Gets or sets delegate method for format value to string that can used in sql query.
        /// </summary>
        [Category("To SQL String")]
        [Description("Gets or sets delegate method for format value to string that can used in sql query.")]
        public Func<decimal, string> DecimalToSqlString { get; set; }
        /// <summary>
        /// Gets or sets delegate method for format value to string that can used in sql query.
        /// </summary>
        [Category("To SQL String")]
        [Description("Gets or sets delegate method for format value to string that can used in sql query.")]
        public Func<float, string> FloatToSqlString { get; set; }
        /// <summary>
        /// Gets or sets delegate method for format value to string that can used in sql query.
        /// </summary>
        [Category("To SQL String")]
        [Description("Gets or sets delegate method for format value to string that can used in sql query.")]
        public Func<sbyte, string> SByteToSqlString { get; set; }
        /// <summary>
        /// Gets or sets delegate method for format value to string that can used in sql query.
        /// </summary>
        [Category("To SQL String")]
        [Description("Gets or sets delegate method for format value to string that can used in sql query.")]
        public Func<short, string> Int16ToSqlString { get; set; }
        /// <summary>
        /// Gets or sets delegate method for format value to string that can used in sql query.
        /// </summary>
        [Category("To SQL String")]
        [Description("Gets or sets delegate method for format value to string that can used in sql query.")]
        public Func<int, string> Int32ToSqlString { get; set; }
        /// <summary>
        /// Gets or sets delegate method for format value to string that can used in sql query.
        /// </summary>
        [Category("To SQL String")]
        [Description("Gets or sets delegate method for format value to string that can used in sql query.")]
        public Func<long, string> Int64ToSqlString { get; set; }
        /// <summary>
        /// Gets or sets delegate method for format value to string that can used in sql query.
        /// </summary>
        [Category("To SQL String")]
        [Description("Gets or sets delegate method for format value to string that can used in sql query.")]
        public Func<byte, string> ByteToSqlString { get; set; }
        /// <summary>
        /// Gets or sets delegate method for format value to string that can used in sql query.
        /// </summary>
        [Category("To SQL String")]
        [Description("Gets or sets delegate method for format value to string that can used in sql query.")]
        public Func<ushort, string> UInt16ToSqlString { get; set; }
        /// <summary>
        /// Gets or sets delegate method for format value to string that can used in sql query.
        /// </summary>
        [Category("To SQL String")]
        [Description("Gets or sets delegate method for format value to string that can used in sql query.")]
        public Func<uint, string> UInt32ToSqlString { get; set; }
        /// <summary>
        /// Gets or sets delegate method for format value to string that can used in sql query.
        /// </summary>
        [Category("To SQL String")]
        [Description("Gets or sets delegate method for format value to string that can used in sql query.")]
        public Func<ulong, string> UInt64ToSqlString { get; set; }
        /// <summary>
        /// Gets or sets delegate method for format value to string that can used in sql query.
        /// </summary>
        [Category("To SQL String")]
        [Description("Gets or sets delegate method for format value to string that can used in sql query.")]
        public Func<bool, string> BooleanToSqlString { get; set; }
        /// <summary>
        /// Gets or sets delegate method for format value to string that can used in sql query.
        /// </summary>
        [Category("To SQL String")]
        [Description("Gets or sets delegate method for format value to string that can used in sql query.")]
        public Func<Guid, string> GuidToSqlString { get; set; }

        #endregion

        #region Select

        /// <summary>
        /// Gets or sets delegate method for get query that used in test select identity.
        /// The delegate input is table name, column name and result is string that represent query 
        /// for get identity value.
        /// </summary>
        [Category("Queries")]
        [Description("Gets or sets delegate method for get query that used in test select identity.")]
        public Func<string, string, string> SelectIdentityQuery { get; set; }
        /// <summary>
        /// Gets or sets delegate method for get query that used in test select max key.
        /// The delegate input is table name, list of column name, , list of column value, 
        /// column name (for get max value), criteria (for filter) and result is string that 
        /// represent query for get max value for target column.
        /// </summary>
        [Category("Queries")]
        [Description("Gets or sets delegate method for get query that used in test select max key.")]
        public Func<string, string[], string[], string, string, string> SelectMaxQuery { get; set; }

        #endregion

        #endregion
    }
}