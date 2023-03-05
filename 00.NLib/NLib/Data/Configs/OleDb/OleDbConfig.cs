#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-12-07
=================
- DataAccess : OleDbConfig
  - Re-Implement Database Config classes for OleDb (abstract).

======================================================================================================================
Update 2013-03-08
=================
- DataAccess : OleDb
  - All OleDb data access classes is redesign.
  - Change name space to NLib.Data and NLib.Data.Design

======================================================================================================================
Update 2010-02-03
=================
- DataAccess : OleDb
  - OleDbFactory ported and re-implements uses new generic base class.
  - OleDbConfig class ported and re-implements.
  - OleDb sql model and related class ported.

======================================================================================================================
Update 2008-11-23
=================
- DataAccess : OleDb
  - Add try-catch in CreateParameter when set Precision/Scale.

======================================================================================================================
Update 2008-10-21
=================
- Sql Model (OleDb) updated.  
  [OleDb(abstract)]
  - Add new class OleDbSqlModel.OleDbDDLFormatter for handle DDL generate script.
  - Implement method CreateDDLFormatter.
  - Implement method GenerateViewScript (incompleted) in it's DDLFormatter 
    (alway return string.Empty).
  - Implement method GenerateTableScript in it's DDLFormatter.
  - Implement method GenerateTableColumnScript in it's DDLFormatter.
  - Implement method GenerateTableConstraintScript in it's DDLFormatter.

======================================================================================================================
Update 2008-10-20
=================
- DataAccess : OleDb
  - Fixed Connection check null bug (in case original code used logical operator && instead of ||).

======================================================================================================================
Update 2008-09-27
=================
- New Class added
  - OracleConnectionFactory class added. This class is re-implement DerivedParameters method.

======================================================================================================================
Update 2008-06-08
=================
- New Class added
  - CsvReader class added.

======================================================================================================================
Update 2008-04-11
=================
- OleDb classes changed.
  - All SqlValueFormatter that related with OleDb config is no longer access via public.

======================================================================================================================
Update 2008-03-26
=================
- New Class added
  - DBF Reader/Writer class added. This 2 class is provide Native access to DBF File.
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
    #region OleDbConfig

    /// <summary>
    /// OleDb based Connection Config class (abstract).
    /// </summary>
    [Serializable]
    //[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public abstract class OleDbConfig : NDbConfig
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public OleDbConfig()
            : base()
        {

        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~OleDbConfig()
        {

        }

        #endregion
    }

    #endregion
}
