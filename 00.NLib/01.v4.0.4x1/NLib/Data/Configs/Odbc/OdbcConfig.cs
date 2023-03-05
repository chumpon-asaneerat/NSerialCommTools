#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-12-07
=================
- DataAccess : OdbcConfig
  - Re-Implement Database Config classes for Odbc (abstract).

======================================================================================================================
Update 2013-03-08
=================
- DataAccess : Odbc
  - All Odbc data access classes is redesign.
  - Change name space to NLib.Data and NLib.Data.Design

======================================================================================================================
Update 2010-02-03
=================
- DataAccess : Odbc
  - OdbcConnectionFactory ported and re-implements uses new generic base class.
  - OdbcConfig class ported and re-implements.
  - Odbc sql model and related class ported.

======================================================================================================================
Update 2008-11-23
=================
- DataAccess : Odbc
  - Add try-catch in CreateParameter when set Precision/Scale.

======================================================================================================================
Update 2008-10-21
=================
- Sql Model (Odbc) updated.
  [Odbc(abstract)]
  - Add new class OdbcSqlModel.OdbcDDLFormatter for handle DDL generate script.
  - Implement method CreateDDLFormatter.
  - Implement method GenerateViewScript (incompleted) in it's DDLFormatter.
  - Implement method GenerateTableScript in it's DDLFormatter.
  - Implement method GenerateTableColumnScript in it's DDLFormatter.
  - Implement method GenerateTableConstraintScript in it's DDLFormatter
    (alway return string.Empty).

======================================================================================================================
Update 2008-10-20
=================
- DataAccess : Odbc
  - Fixed Connection check null bug (in case original code used logical operator && instead of ||).

======================================================================================================================
Update 2008-04-11
=================
- Odbc classes changed.
  - All SqlValueFormatter that related with Odbc config is no longer access via public.

======================================================================================================================
Update 2008-03-26
=================
- Feature added
  - Add related SqlValueFormatter class for each Provider.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using NLib;
using NLib.Design;
using NLib.Data;
using NLib.Data.Design;
using NLib.Reflection;
using NLib.Xml;
using NLib.Utils;

#endregion

namespace NLib.Data
{
    #region OdbcConfig

    /// <summary>
    /// Odbc based Connection Config class (abstract).
    /// </summary>
    [Serializable]
    //[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public abstract class OdbcConfig : NDbConfig
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public OdbcConfig()
            : base()
        {

        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~OdbcConfig()
        {

        }

        #endregion
    }

    #endregion
}
