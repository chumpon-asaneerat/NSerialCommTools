#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2012-12-15
=================
- Persistance Factory ported from NLibV401 to NLibV404.
  - Re-Format code(s).
  - Update comments.
  - TableNameAttribute changed to MapSource. 
  - ColumnNameAttribute changed to MapMember. 
  - ORMDataColumn clanged to ORMDataMember

======================================================================================================================
Update 2010-02-03
=================
- Persistance Factory ported from GFA37 to GFA38v3.

======================================================================================================================
Update 2009-09-17
=================
- Persistance Factory - fixed bug
  - Fixed bug in Select<T> method. This bug will occur when execption occur and
    the original command's text is not restore into command object so new check 
    routine is added to make sure the original command text will not contain where cause.

======================================================================================================================
Update 2009-08-23
=================
- Persistance Factory - fixed bug
  - Fixed bug in Select<T> method. This bug will occur when execption occur and
    the original command's text is not restore into command object because the return statement.

======================================================================================================================
Update 2008-09-25
=================
- Persistance Factory - optimize spped in Select<> Method.
  - Add support assign from DataRow.

======================================================================================================================
Update 2008-09-24
=================
- Persistance Factory - optimize spped in Select<> Method.

======================================================================================================================
Update 2008-09-04
=================
- Light ORM is no longer supported.
- Fixed MS access bug by .NET framework the command's connection is lost with no reason.

======================================================================================================================
Update 2008-08-04
=================
- Persistance Factory - Fixed MS Access update parameters' order.
  - In Ms Access the Command parameter's order must match with the Update Command Text.

======================================================================================================================
Update 2008-07-06
=================
- Persistance Factory - transaction implement and tested.
  - Add transaction implements and change the return type of Select/Insert/Update/Delete
    to ExecuteResult<T>.

======================================================================================================================
Update 2008-07-02
=================
- Persistance Factory no longer access via public. Used Connection Manager instead.

======================================================================================================================
Update 2008-06-12
=================
- Persistance Factory
  - Add AutoGenerateAttribute class. Used for marked column that it's value is create by
    Database service.
  - Add new method -> T Create<T>(bool autoAssignDefaultValue). Used for create new object 
    instance when assign default value from The DefaultValueAttribute.
  - Add Transaction support but not tested.

======================================================================================================================
Update 2008-06-07
=================
- Persistance Factory
  - support Select/Insert/Update/Delete.
  - Support generate method. 
    Test Result For SqlServer :    
      Identity (after insert only) -> PASS.
      SelectMax -> PASS.
      SystemDate (Before/After insert and Update) -> PASS.
      NewGuid -> PASS.
      Generator -> Not support.
      Sequence -> Not support.
    Test Result For OracleDirect :    
      Identity (after insert only) -> Not support.
      SelectMax -> PASS.
      SystemDate (Before/After insert and Update) -> PASS.
      NewGuid -> PASS.
      Generator (after insert only) -> Not support.
      Sequence (after insert only) -> PASS.
    Test Result For Oracle (OleDb) :    
      Identity (after insert only) -> Not support.
      SelectMax -> Not TEST.
      SystemDate (Before/After insert and Update) -> Not TEST.
      NewGuid -> Not TEST.
      Generator (after insert only) -> Not support.
      Sequence (after insert only) -> Not TEST.
    Test Result For MsAccess :    
      Identity (after insert only) -> Not support.
      SelectMax -> Not TEST.
      SystemDate (Before/After insert and Update) -> Not TEST.
      NewGuid -> Not TEST.
      Generator (after insert only) -> Not support.
      Sequence (after insert only) -> Not support.
    Test Result For Excel :    
      Identity (after insert only) -> Not support.
      SelectMax -> Not Support.
      SystemDate (Before/After insert and Update) -> Not TEST.
      NewGuid -> Not TEST.
      Generator (after insert only) -> Not support.
      Sequence (after insert only) -> Not support.
    Test Result For Informix :    
      Identity (after insert only) -> Not support.
      SelectMax -> Not TEST.
      SystemDate (Before/After insert and Update) -> Not TEST.
      NewGuid -> Not TEST.
      Generator (after insert only) -> Not support.
      Sequence (after insert only) -> Not support.
    Test Result For InterBase/FireBird :    
      Identity (after insert only) -> Not support.
      SelectMax -> Not TEST.
      SystemDate (Before/After insert and Update) -> Not TEST.
      NewGuid -> Not TEST.
      Generator (after insert only) -> Not TEST.
      Sequence (after insert only) -> Not support.

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
using System.Text;

#endregion

namespace NLib.Data
{
    #region Attributes

    #region Map Source attribute

    /// <summary>
    /// Map Source attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MapSourceAttribute : Attribute
    {
        #region Internal Variable

        private string _sourceName = string.Empty;
        private bool _readOnly = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceName">The specificed Source Name (Table Name or Class Name).</param>
        public MapSourceAttribute(string sourceName)
        {
            _sourceName = sourceName;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets Source's Name.
        /// </summary>
        public string SourceName { get { return _sourceName; } }
        /// <summary>
        /// Gets Is readonly.
        /// </summary>
        public bool ReadOnly { get { return _readOnly; } set { _readOnly = value; } }

        #endregion
    }

    #endregion

    #region Map Member attribute

    /// <summary>
    /// Map Member attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MapMemberAttribute : Attribute
    {
        #region Internal Variable

        private string _memberName = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="memberName">The specificed Column Name or Property Name.</param>
        public MapMemberAttribute(string memberName)
        {
            _memberName = memberName;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets Member's Name
        /// </summary>
        public string MemberName { get { return _memberName; } }

        #endregion
    }

    #endregion

    #region Auto Generate Attribute

    /// <summary>
    /// Auto Generate Attribute. Use this attribute to marked the column is auto generate value by database
    /// used when column is auto assign value via Trigger, Sequence or Generator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoGenerateAttribute : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AutoGenerateAttribute() : base() { }
    }

    #endregion

    #region MethodCall Enum

    /// <summary>
    /// MethodCall Enum
    /// </summary>
    public enum MethodCall
    {
        /// <summary>
        /// Call before insert.
        /// </summary>
        BeforeInsert,
        /// <summary>
        /// Call after insert.
        /// </summary>
        AfterInsert,
        /// <summary>
        /// Call before update.
        /// </summary>
        BeforeUpdate,
        /// <summary>
        /// Call after update.
        /// </summary>
        AfterUpdate
    }

    #endregion

    #region Generate Method Attribute

    /// <summary>
    /// GenerateMethod attribute. Used for markd that the specificed need to retrived data from user
    /// Generate Method before insertion occur.
    /// </summary>
    public class GenerateMethodAttribute : Attribute
    {
        #region Internal Variable

        private MethodCall _call = MethodCall.AfterInsert;
        private string _generateMethodName = string.Empty;
        private string _generateMethodParams = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private GenerateMethodAttribute() : base() { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="call">Set when to call method.</param>
        /// <param name="generateMethodName">The Generate Method Name.</param>
        public GenerateMethodAttribute(MethodCall call, string generateMethodName)
            : this()
        {
            _call = call;
            _generateMethodName = generateMethodName;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="call">Set when to call method.</param>
        /// <param name="generateMethodName">The Generate Method Name.</param>
        /// <param name="parameters">The Generate Method parameter string.</param>
        public GenerateMethodAttribute(MethodCall call, string generateMethodName, string parameters)
            : this()
        {
            _call = call;
            _generateMethodName = generateMethodName;
            _generateMethodParams = parameters;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets when method is need.
        /// </summary>
        public MethodCall Call
        {
            get { return _call; }
        }
        /// <summary>
        /// Gets Generate Method Name.
        /// </summary>
        public string GenerateMethodName
        {
            get { return _generateMethodName; }
        }
        /// <summary>
        /// Gets Generate Method parameter(s) string.
        /// </summary>
        public string Parameters
        {
            get { return _generateMethodParams; }
        }

        #endregion
    }

    #endregion

    #endregion
}
