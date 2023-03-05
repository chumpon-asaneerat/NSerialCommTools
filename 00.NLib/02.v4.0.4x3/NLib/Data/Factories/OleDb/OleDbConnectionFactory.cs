#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-07-07
=================
- Data Access Framework - OleDbConnectionFactory updated.
  - Changed all log code used MethodBase.

======================================================================================================================
Update 2014-11-30
=================
- Data Access Framework - OleDbConnectionFactory
  - Redesign OleDbConnectionFactory class with new implements.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Data;
using System.Reflection;

#endregion

namespace NLib.Data
{
    #region OleDb Factory

    /// <summary>
    /// OleDb Connection Factory.
    /// </summary>
    public abstract class OleDbConnectionFactory : NDbFactory<
        System.Data.OleDb.OleDbConnection,
        System.Data.OleDb.OleDbTransaction,
        System.Data.OleDb.OleDbCommand,
        System.Data.OleDb.OleDbParameter,
        System.Data.OleDb.OleDbDataAdapter,
        System.Data.OleDb.OleDbCommandBuilder>
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

            formatter.GetParameterPrefix = () =>
            {
                return "@";
            };
            formatter.GetDefaultReturnParameterName = () =>
            {
                return GetParameterPrefix() + "RETURN_VALUE";
            };

            #endregion
        }

        #endregion

        #region SetProviderDbTypeID

        /// <summary>
        /// Set the provide db type id to the parameter object.
        /// </summary>
        /// <param name="parameter">The parameter object.</param>
        /// <param name="providerDbTypeID">The provide type id.</param>
        protected override void SetProviderDbTypeID(System.Data.OleDb.OleDbParameter parameter, int providerDbTypeID)
        {
            parameter.OleDbType = (System.Data.OleDb.OleDbType)providerDbTypeID;
        }

        #endregion

        #region Schema Info Implements

        /// <summary>
        /// Get Table Schema Info.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <returns>Returns the Table SchemaInfo instance for DbTable.</returns>
        protected override SchemaInfo<DbTable> GetTableSchemaInfo(string owner)
        {
            // OleDb
            SchemaInfo<DbTable> info = new SchemaInfo<DbTable>();

            info.MetaData = new DbMetaData("Tables");
            info.Restrictions = new DbRestriction[] 
            {                
                new DbRestriction(1, "TABLE_CATALOG", ""),
                new DbRestriction(2, "TABLE_SCHEMA", ""),
                new DbRestriction(3, "TABLE_NAME", ""),
                new DbRestriction(4, "TABLE_TYPE", "")
            };
            info.Convert = delegate(DataRow row, SchemaInfo<DbTable> schema, NDbFactory factory)
            {
                DbTable val = null;
                if (row["TABLE_NAME"] != DBNull.Value)
                {
                    val = new DbTable();
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
        protected override SchemaInfo<DbTable> GetViewSchemaInfo(string owner)
        {
            // OleDb
            return null;
        }
        /// <summary>
        /// GetProcedureSchemaInfo
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns>Returns ProcedureSchemaInfo instance.</returns>
        protected override ProcedureSchemaInfo GetProcedureSchemaInfo(string owner)
        {
            DbRestriction[] restrictions = new DbRestriction[4];
            restrictions[0] = new DbRestriction(1, "PROCEDURE_CATALOG", string.Empty);
            restrictions[1] = new DbRestriction(2, "PROCEDURE_SCHEMA", owner);
            restrictions[2] = new DbRestriction(3, "PROCEDURE_NAME", string.Empty);
            restrictions[3] = new DbRestriction(4, "PROCEDURE_TYPE", string.Empty);

            ProcedureSchemaInfo info = new ProcedureSchemaInfo();
            info.Restrictions = restrictions;
            info.ProcedureNameColumn = "PROCEDURE_NAME";

            return info;
        }

        #endregion

        #endregion
    }

    #endregion
}
