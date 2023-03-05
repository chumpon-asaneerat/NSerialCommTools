#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-07-07
=================
- Data Access Framework - OdbcConnectionFactory updated.
  - Changed all log code used MethodBase.

======================================================================================================================
Update 2014-11-30
=================
- Data Access Framework - OdbcConnectionFactory
  - Redesign OdbcConnectionFactory class with new implements.

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
    #region Odbc Factory

    /// <summary>
    /// Odbc Connection Factory.
    /// </summary>
    public abstract class OdbcConnectionFactory : NDbFactory<
        System.Data.Odbc.OdbcConnection,
        System.Data.Odbc.OdbcTransaction,
        System.Data.Odbc.OdbcCommand,
        System.Data.Odbc.OdbcParameter,
        System.Data.Odbc.OdbcDataAdapter,
        System.Data.Odbc.OdbcCommandBuilder>
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
        protected override void SetProviderDbTypeID(System.Data.Odbc.OdbcParameter parameter, int providerDbTypeID)
        {
            parameter.OdbcType = (System.Data.Odbc.OdbcType)providerDbTypeID;
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
            // Odbc
            NSchemaInfo<NDbTable> info = new NSchemaInfo<NDbTable>();

            info.MetaData = new NDbMetaData("Tables");
            info.Restrictions = new NDbRestriction[] 
            {                
                new NDbRestriction(1, "TABLE_CAT", ""),
                new NDbRestriction(2, "TABLE_SCHEM", ""),
                new NDbRestriction(3, "TABLE_NAME", "")
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
            // Odbc
            NSchemaInfo<NDbTable> info = new NSchemaInfo<NDbTable>();

            info.MetaData = new NDbMetaData("Views");
            info.Restrictions = new NDbRestriction[] 
            {                
                new NDbRestriction(1, "TABLE_CAT", ""),
                new NDbRestriction(2, "TABLE_SCHEM", ""),
                new NDbRestriction(3, "TABLE_NAME", "")
            };
            info.Convert = delegate(DataRow row, NSchemaInfo<NDbTable> schema, NDbFactory factory)
            {
                NDbTable val = null;
                if (row["VIEW_NAME"] != DBNull.Value)
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
        /// GetProcedureSchemaInfo
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns>Returns ProcedureSchemaInfo instance.</returns>
        protected override NProcedureSchemaInfo GetProcedureSchemaInfo(string owner)
        {
            NDbRestriction[] restrictions = new NDbRestriction[3];
            restrictions[0] = new NDbRestriction(1, "PROCEDURE_CAT", string.Empty);
            restrictions[1] = new NDbRestriction(2, "PROCEDURE_SCHEM", owner);
            restrictions[2] = new NDbRestriction(3, "PROCEDURE_NAME", string.Empty);
            //restrictions[3] = new DbRestriction(4, "PROCEDURE_TYPE", string.Empty);

            NProcedureSchemaInfo info = new NProcedureSchemaInfo();
            info.Restrictions = restrictions;
            info.ProcedureNameColumn = "PROCEDURE_NAME";

            return info;
        }

        #endregion

        #endregion
    }

    #endregion
}
