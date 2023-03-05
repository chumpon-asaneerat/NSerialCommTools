#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-07-07
=================
- Data Access Framework - OracleConnectionFactory and Tasks updated.
  - Changed all log code used MethodBase.

======================================================================================================================
Update 2014-11-30
=================
- Data Access Framework - OracleConnectionFactory
  - Redesign OracleConnectionFactory class with new implements.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

#endregion

#pragma warning disable 0618 // disable deplecated warning 

namespace NLib.Data
{
    #region Oracle (Native) Factory

    /// <summary>
    /// Oracle Connection Factory.
    /// </summary>
    public class OracleConnectionFactory : NDbFactory<
        System.Data.OracleClient.OracleConnection,
        System.Data.OracleClient.OracleTransaction,
        System.Data.OracleClient.OracleCommand,
        System.Data.OracleClient.OracleParameter,
        System.Data.OracleClient.OracleDataAdapter,
        System.Data.OracleClient.OracleCommandBuilder>
    {
        #region Abstract/Virtual Overrides

        #region InitFormatter

        /// <summary>
        /// Init formatter.
        /// </summary>
        /// <param name="formatter">The formatter instance.</param>
        protected override void InitFormatter(NDbFormatter formatter)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            #region Checks

            if (null == formatter)
            {
                "formatter is null".Err(med);
                return;
            }

            #endregion

            #region Init delegates

            formatter.GetTestQuery = () =>
            {
                return "SELECT 1 FROM DUAL";
            };
            formatter.GetServerDateQuery = () =>
            {
                return "SELECT sysdate FROM DUAL";
            };
            formatter.GetParameterPrefix = () =>
            {
                return ":";
            };
            formatter.RequiredSPInputParameterPrefix = () =>
            {
                return false;
            };
            formatter.RequiredSPOutputParameterPrefix = () =>
            {
                return false;
            };
            formatter.GetDefaultReturnParameterName = () =>
            {
                return GetParameterPrefix() + "RETURN_VALUE";
            };
            formatter.DateTimeToSqlString = (DateTime value, bool dateOnly) =>
            {
                if (dateOnly)
                {
                    string format = "dd/MM/yyyy)";
                    return "to_date('" + value.ToString(format,
                        DateTimeFormatInfo.InvariantInfo) + "', 'dd/MM/yyyy')";
                }
                else
                {
                    string format = "dd/MM/yyyy HH:mm:ss)";
                    return "to_date('" + value.ToString(format,
                        DateTimeFormatInfo.InvariantInfo) + "', 'dd/MM/yyyy HH24:mi:ss')";
                }
            };

            #endregion
        }

        #endregion

        #region CanCreateConnection

        /// <summary>
        /// Overrides to handle the validation logic to check the prerequisition
        /// for initialize the connection intance.
        /// </summary>
        /// <returns>Returns true if the can create the connection instance.</returns>
        protected override bool CanCreateConnection()
        {
            string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            string[] requiredFiles = new string[]
            {
                @"\oci.dll",
                @"\ociw32.dll",
                @"\orannzsbb11.dll",
                @"\oraociei11.dll",
                @"\OraOps11w.dll",
                @"\orasql11.dll"
            };

            MethodBase med = MethodBase.GetCurrentMethod();
            bool hasAllFiles = true;
            foreach (string fileName in requiredFiles)
            {
                if (!File.Exists(appPath + fileName))
                {
                    hasAllFiles = false;
                    break;
                }
            }
            if (!hasAllFiles)
            {
                "Cannot connect to oracle database because one or more instant client file(s) are missing.".Err(med);
                "Required file list below".Err(med);
                foreach (string fileName in requiredFiles)
                {
                    string msg = " - " + fileName;
                    msg.Err(med);
                }
            }
            return hasAllFiles;
        }

        #endregion

        #region OnDerivedDbParameters

        /// <summary>
        /// Overrides to customized the parameter's characteristic after 
        /// DerivedParameters is called. Note that the _DerivedDbParameters method
        /// is call static method DeriveParameters on the DbBuilder object so 
        /// if the DbBuilder is not contains DeriveParameters then no parameters is derived
        /// so the user code should handle whne the DbBuilder is not contains static method 
        /// DeriveParameters.
        /// </summary>
        /// <param name="command">The command instance that parameters is derived.</param>
        protected override void OnDerivedDbParameters(System.Data.OracleClient.OracleCommand command)
        {
            // check for cursor
            foreach (System.Data.OracleClient.OracleParameter param in command.Parameters)
            {
                if (param.OracleType == System.Data.OracleClient.OracleType.Cursor)
                {
                    param.Direction = ParameterDirection.Output;
                }
            }
        }

        #endregion

        #region SetProviderDbTypeID
        
        /// <summary>
        /// Set the provide db type id to the parameter object.
        /// </summary>
        /// <param name="parameter">The parameter object.</param>
        /// <param name="providerDbTypeID">The provide type id.</param>
        protected override void SetProviderDbTypeID(System.Data.OracleClient.OracleParameter parameter, int providerDbTypeID)
        {
            parameter.OracleType = (System.Data.OracleClient.OracleType)providerDbTypeID;
        }

        #endregion

        #region Schema Info Implements

        /// <summary>
        /// Get Table Schema Info.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <returns>Returns the Table SchemaInfo instance for DbTable.</returns>
        protected override NSchemaInfo<NDbTable> GetTableSchemaInfo(string owner)
        {
            string _owner = string.IsNullOrWhiteSpace(owner) ? string.Empty : owner.Trim();

            // Oracle Direct
            NSchemaInfo<NDbTable> info = new NSchemaInfo<NDbTable>();

            info.MetaData = new NDbMetaData("Tables");
            info.Restrictions = new NDbRestriction[] 
            {                
                new NDbRestriction(1, "Owner", _owner.ToUpper()),
                new NDbRestriction(2, "Table", "")
            };
            info.Convert = delegate(DataRow row, NSchemaInfo<NDbTable> schema, NDbFactory factory)
            {
                NDbTable val = null;
                if (row["TABLE_NAME"] != DBNull.Value)
                {
                    val = new NDbTable();
                    val.TableName = row["TABLE_NAME"].ToString();
                    val.ProviderTableType = row["TABLE_TYPE"].ToString();
                }
                return val;
            };

            return info;
        }
        /// <summary>
        /// Get View Schema Info.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <returns>Returns the Table SchemaInfo instance for DbTable.</returns>
        protected override NSchemaInfo<NDbTable> GetViewSchemaInfo(string owner)
        {
            string _owner = string.IsNullOrWhiteSpace(owner) ? string.Empty : owner.Trim();

            // Oracle Direct
            NSchemaInfo<NDbTable> info = new NSchemaInfo<NDbTable>();

            info.MetaData = new NDbMetaData("Views");
            info.Restrictions = new NDbRestriction[] 
            {                
                new NDbRestriction(1, "Owner", _owner.ToUpper()),
                new NDbRestriction(2, "View", "")
            };
            info.Convert = delegate(DataRow row, NSchemaInfo<NDbTable> schema, NDbFactory factory)
            {
                NDbTable val = null;
                if (row["VIEW_NAME"] != DBNull.Value)
                {
                    val = new NDbTable();
                    val.TableName = row["VIEW_NAME"].ToString();
                    val.ProviderTableType = row["VIEW_TYPE"].ToString();
                    if (val.ProviderTableType == string.Empty)
                    {
                        val.ProviderTableType = "VIEW";
                    }
                }
                return val;
            };

            return info;
        }
        /// <summary>
        /// GetProcedureSchemaInfo
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns>Returns ProcedureSchemaInfo instance.</returns>
        protected override NProcedureSchemaInfo GetProcedureSchemaInfo(string owner)
        {
            string _owner = string.IsNullOrWhiteSpace(owner) ? string.Empty : owner.Trim();
            NDbRestriction[] restrictions = new NDbRestriction[2];
            restrictions[0] = new NDbRestriction(1, "OWNER", _owner);
            restrictions[1] = new NDbRestriction(2, "NAME", string.Empty);

            NProcedureSchemaInfo info = new NProcedureSchemaInfo();
            info.Restrictions = restrictions;
            info.ProcedureNameColumn = "OBJECT_NAME";

            return info;
        }

        #endregion

        #region GetTasks (not implements)

        /// <summary>
        /// Gets Supported Task.
        /// </summary>
        /// <returns>Returns list of all avaliable supported tasks.</returns>
        public override NDbTask[] GetTasks() { return null; }

        #endregion

        #endregion
    }

    #endregion
}

namespace NLib.Data
{
    #region Oracle (Native) Tasks

    #endregion
}

#pragma warning restore 0618 // restore deplecated warning 