#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-12-15
=================
- Generate Method related classes ported from NLibV401 to NLibV404.
  - Update comments.

======================================================================================================================
Update 2010-02-03
=================
- Generate Method related classes ported from GFA37 to GFA38v3.

======================================================================================================================
Update 2008-08-04
=================
- Generate Method add and changed
  - Add New Generate Method SelectCurrentMax
  - Change generate method SelectMax to SelectNextMax

======================================================================================================================
Update 2008-06-12
=================
- Generate Method fixed bug
  - Fixed NextMaxValue is return next max value (not the current max value).

======================================================================================================================
Update 2008-06-06
=================
- Generate Method and subclass for specificed provider classes added.
  - Used for handle how to generate data in specificed column
  - current support method NextGuid, CurrentIdeneity, SystemDate, Sequence(oracle), 
    NextMaxValue(coumpond key support), Generator(InterBase, FireBird).

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Data.Common;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;

#endregion

namespace NLib.Data
{
    #region Generate Result

    /// <summary>
    /// The Generate Result class.
    /// </summary>
    public sealed class GenerateResult
    {
        #region Internal Variable

        private bool _success = false;
        private object _value = null;
        private Exception _exception = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private GenerateResult()
            : base() { }
        /// <summary>
        /// Constructor. Create with exception. The success flag is automatically set to true.
        /// </summary>
        /// <param name="value">The value after generate.</param>
        public GenerateResult(object value)
            : this()
        {
            _value = value;
            _success = true;
        }
        /// <summary>
        /// Constructor. Create with exception. The success flag is automatically set to false.
        /// </summary>
        /// <param name="ex">The exception after generate.</param>
        public GenerateResult(Exception ex)
            : this()
        {
            _exception = ex;
            _success = false;
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~GenerateResult()
        {
            _value = null;
            _exception = null;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Checks Is operation success.
        /// </summary>
        public bool IsSuccess { get { return _success; } }
        /// <summary>
        /// Gets the result value.
        /// </summary>
        public object Value { get { return _value; } }
        /// <summary>
        /// Gets the exception value.
        /// </summary>
        public Exception Exception { get { return _exception; } }

        #endregion
    }

    #endregion

    #region Generate Method class (abstract)

    /// <summary>
    /// Generate Method class (abstract).
    /// </summary>
    public abstract class GenerateMethod
    {
        #region Internal Variable

        private string _tableName;
        private string _columnName;
        private string _propertyName;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public GenerateMethod() : base() { }

        #endregion

        #region Virtual Method

        /// <summary>
        /// Gets Parameter string.
        /// </summary>
        /// <returns>Returns string that represent all parameters in an object.</returns>
        public virtual string GetParameters() { return string.Empty; }
        /// <summary>
        /// Sets Parameter string.
        /// </summary>
        /// <param name="value">The string that represent all parameters in an object.</param>
        public virtual void SetParameters(string value)
        {
        }

        #endregion

        #region Abstract Method

        /// <summary>
        /// Generate.
        /// </summary>
        /// <param name="manager">The connection manager instance.</param>
        /// <param name="transaction">The database transaction instance.</param>
        /// <returns>Returns the GenerateResult instance.</returns>
        public abstract GenerateResult Generate(NLib.Components.NDbConnection manager,
            DbTransaction transaction);

        #endregion

        #region Public Property

        /// <summary>
        /// Gets or Sets Target Table's Name.
        /// </summary>
        [XmlIgnore]
        public string TableName { get { return _tableName; } set { _tableName = value; } }
        /// <summary>
        /// Gets or Sets Target Column's Name.
        /// </summary>
        [XmlIgnore]
        public string ColumnName { get { return _columnName; } set { _columnName = value; } }
        /// <summary>
        /// Gets or Sets Target Property's Name.
        /// </summary>
        [XmlIgnore]
        public string PropertyName { get { return _propertyName; } set { _propertyName = value; } }

        #endregion

        #region Static

        private static Type _baseType = typeof(GenerateMethod);
        private static string staticPropertyName = "GenerateMethodName";

        #region Create Generate Method By Name

        /// <summary>
        /// Create Generate Method By Name.
        /// </summary>
        /// <param name="generateMethodName">Generate Method's Name.</param>
        /// <returns>Returns Generate Method's Instance. if not found null return.</returns>
        public static GenerateMethod Create(string generateMethodName)
        {
            GenerateMethod instance = null;
            MethodBase med = MethodBase.GetCurrentMethod();
            string msg = string.Format("GenerateMethod.Create : {0}", generateMethodName);
            msg.Info(med);

            Type[] types = GetGenerateMethods();
            if (types == null || types.Length <= 0)
                instance = null;
            else
            {
                object val = NLib.Reflection.TypeUtils.Create(_baseType, staticPropertyName,
                    generateMethodName, "Create");
                if (val != null && val.GetType().IsSubclassOf(_baseType))
                    instance = (val as GenerateMethod);
                else return instance;
            }

            return instance;
        }

        #endregion

        #region Get Generate Method Names

        /// <summary>
        /// Gets Generate Method Names.
        /// </summary>
        /// <returns>Returns list of all Type's FullName that inherited from GenerateMethod.</returns>
        public static string[] GetGenerateMethodNames()
        {
            return NLib.Reflection.TypeUtils.GetInheritedTypeNames(
                _baseType, staticPropertyName);
        }

        #endregion

        #region Get Generate Methods

        /// <summary>
        /// Gets Generate Methods.
        /// </summary>
        /// <returns>Returns list of all Type that inherited from GenerateMethod in cache.</returns>
        public static Type[] GetGenerateMethods()
        {
            return NLib.Reflection.TypeUtils.GetInheritedTypes(_baseType);
        }
        /// <summary>
        /// Gets Generate Methods.
        /// </summary>
        /// <param name="refresh">refresh cache.</param>
        /// <returns>Returns list of all Type that inherited from GenerateMethod.</returns>
        public static Type[] GetGenerateMethods(bool refresh)
        {
            return NLib.Reflection.TypeUtils.GetInheritedTypes(_baseType, refresh);
        }

        #endregion

        #region Generate Method Name Property

        /// <summary>
        /// Gets Generate Method Name.
        /// </summary>
        public static string GenerateMethodName { get { return string.Empty; } }

        #endregion

        #region Create

        /// <summary>
        /// Create New Generate Method Instance.
        /// </summary>
        /// <returns>Returns new Generate Method Instance.</returns>
        public static GenerateMethod Create() { return null; }

        #endregion

        #endregion

        #region Const

        /// <summary>
        /// Identity
        /// </summary>
        public const string Identity = "CurrentIdentityValue";
        /// <summary>
        /// Next Guid
        /// </summary>
        public const string NextGuid = "NextGuidValue";
        /// <summary>
        /// System Date
        /// </summary>
        public const string SystemDate = "CurrentDateTimeValue";
        /// <summary>
        /// Select Next Max value.
        /// </summary>
        public const string SelectNextMax = "NextMaxValue";
        /// <summary>
        /// Select Current Max value.
        /// </summary>
        public const string SelectCurrentMax = "CurrentMaxValue";
        /// <summary>
        /// Sequence
        /// </summary>
        public const string Sequence = "CurrentSequenceValue";
        /// <summary>
        /// Generator
        /// </summary>
        public const string Generator = "CurrentGeneratorValue";

        #endregion
    }

    #endregion

    #region Generate Method related classes

    #region Next Guid Value

    /// <summary>
    /// Get Next Guid Value Generate method class.
    /// </summary>
    public class NextGuidValue : GenerateMethod
    {
        #region Abstract Implements

        /// <summary>
        /// Generate.
        /// </summary>
        /// <param name="manager">The connection manager instance.</param>
        /// <param name="transaction">The database transaction instance.</param>
        /// <returns>Returns the GenerateResult instance.</returns>
        public override GenerateResult Generate(NLib.Components.NDbConnection manager,
            DbTransaction transaction)
        {
            GenerateResult result = new GenerateResult(Guid.NewGuid());

            return result;
        }

        #endregion

        #region Static Access

        /// <summary>
        /// Gets Generate Method Name.
        /// </summary>
        public new static string GenerateMethodName
        {
            get { return "NextGuidValue"; }
        }
        /// <summary>
        /// Create Generate Method Instance.
        /// </summary>
        /// <returns>Returns new instance of Generate Method.</returns>
        public new static GenerateMethod Create()
        {
            return new NextGuidValue();
        }

        #endregion
    }

    #endregion

    #region Current Identity Value

    /// <summary>
    /// Get Identity Value Generate method class.
    /// </summary>
    public class CurrentIdentityValue : GenerateMethod
    {
        #region Abstract Implements

        /// <summary>
        /// Generate.
        /// </summary>
        /// <param name="manager">The connection manager instance.</param>
        /// <param name="transaction">The database transaction instance.</param>
        /// <returns>Returns the GenerateResult instance.</returns>
        public override GenerateResult Generate(NLib.Components.NDbConnection manager,
            DbTransaction transaction)
        {
            GenerateResult result = null;

            if (null == manager || !manager.IsConnected)
            {
                result = new GenerateResult(
                    new Exception("No connection assigned or connection is not connected."));
            }
            else
            {
                if (null == manager.Factory || null == manager.Factory.Formatter)
                {
                    result = new GenerateResult(
                        new Exception("Db Connection not assigned formatter."));
                    return result;
                }
                
                string query = manager.Factory.Formatter.SelectIdentity(
                    this.TableName, this.ColumnName);

                ExecuteResult<object> val = manager.ExecuteScalar(query, transaction);
                if (val.Exception != null)
                {
                    result = new GenerateResult(val.Exception);
                }
                else
                {
                    result = new GenerateResult(val.Result);
                }
            }

            return result;
        }

        #endregion

        #region Static Access

        /// <summary>
        /// Gets Generate Method Name.
        /// </summary>
        public new static string GenerateMethodName
        {
            get { return "CurrentIdentityValue"; }
        }
        /// <summary>
        /// Create Generate Method Instance.
        /// </summary>
        /// <returns>Returns new instance of Generate Method.</returns>
        public new static GenerateMethod Create()
        {
            return new CurrentIdentityValue();
        }

        #endregion
    }

    #endregion

    #region Current DateTime Value

    /// <summary>
    /// Get Current System DateTime Generate method class.
    /// </summary>
    public class CurrentDateTimeValue : GenerateMethod
    {
        #region Abstract Implements

        /// <summary>
        /// Generate.
        /// </summary>
        /// <param name="manager">The connection manager instance.</param>
        /// <param name="transaction">The database transaction instance.</param>
        /// <returns>Returns the GenerateResult instance.</returns>
        public override GenerateResult Generate(NLib.Components.NDbConnection manager,
            DbTransaction transaction)
        {
            GenerateResult result = null;

            if (null == manager || !manager.IsConnected)
            {
                result = new GenerateResult(
                    new Exception("No connection assigned or connection is not connected."));
            }
            else
            {
                result = new GenerateResult(manager.GetSystemDateTime(transaction));
            }

            return result;
        }

        #endregion

        #region Static Access

        /// <summary>
        /// Gets Generate Method Name.
        /// </summary>
        public new static string GenerateMethodName
        {
            get { return "CurrentDateTimeValue"; }
        }
        /// <summary>
        /// Create Generate Method Instance.
        /// </summary>
        /// <returns>Returns new instance of Generate Method.</returns>
        public new static GenerateMethod Create()
        {
            return new CurrentDateTimeValue();
        }

        #endregion
    }

    #endregion

    #region Next Max Value

    /// <summary>
    /// Get Next Max Value Generate method class.
    /// </summary>
    public class NextMaxValue : GenerateMethod
    {
        #region Internal Variables

        private string[] _compoundKeyNames = null;
        private string[] _compoundKeyValues = null;

        #endregion

        #region Abstract Implements

        /// <summary>
        /// Generate.
        /// </summary>
        /// <param name="manager">The connection manager instance.</param>
        /// <param name="transaction">The database transaction instance.</param>
        /// <returns>Returns the GenerateResult instance.</returns>
        public override GenerateResult Generate(NLib.Components.NDbConnection manager,
            DbTransaction transaction)
        {
            GenerateResult result = null;

            if (null == manager || !manager.IsConnected)
            {
                result = new GenerateResult(
                    new Exception("No connection assigned or connection is not connected."));
            }
            else
            {
                if (null == manager.Factory || null == manager.Factory.Formatter)
                {
                    result = new GenerateResult(
                        new Exception("Db Connection not assigned formatter."));
                    return result;
                }

                string query = manager.Factory.Formatter.SelectMax(this.TableName,
                    this.CompoundKeyNames, this.ColumnKeyValues, this.ColumnName, string.Empty);

                ExecuteResult<object> val = manager.ExecuteScalar(query, transaction);
                if (val.Exception != null)
                {
                    result = new GenerateResult(val.Exception);
                }
                else
                {
                    int maxVal = 0;
                    if (val.Result == null || val.Result == DBNull.Value ||
                        !NLib.Utils.StringUtils.IsInteger(val.Result.ToString()))
                    {
                        maxVal = 1;
                    }
                    else
                    {
                        int resultVal = Convert.ToInt32(val.Result.ToString());
                        maxVal = resultVal + 1;
                    }
                    result = new GenerateResult(maxVal);
                }
            }

            return result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Coupound Key's Name.
        /// </summary>
        [XmlIgnore]
        public string[] CompoundKeyNames
        {
            get { return _compoundKeyNames; }
            set
            {
                if (_compoundKeyNames != value)
                {
                    _compoundKeyNames = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets Coupound Key's Value.
        /// </summary>
        [XmlIgnore]
        public string[] ColumnKeyValues
        {
            get { return _compoundKeyValues; }
            set
            {
                if (_compoundKeyValues != value)
                {
                    _compoundKeyValues = value;
                }
            }
        }

        #endregion

        #region Static Access

        /// <summary>
        /// Gets Generate Method Name.
        /// </summary>
        public new static string GenerateMethodName
        {
            get { return "NextMaxValue"; }
        }
        /// <summary>
        /// Create Generate Method Instance.
        /// </summary>
        /// <returns>Returns new instance of Generate Method.</returns>
        public new static GenerateMethod Create()
        {
            return new NextMaxValue();
        }

        #endregion
    }

    #endregion

    #region Current Max Value

    /// <summary>
    /// Get Current Max Value Generate method class.
    /// </summary>
    public class CurrentMaxValue : GenerateMethod
    {
        #region Internal Variables

        private string[] _compoundKeyNames = null;
        private string[] _compoundKeyValues = null;

        #endregion

        #region Abstract Implements

        /// <summary>
        /// Generate.
        /// </summary>
        /// <param name="manager">The connection manager instance.</param>
        /// <param name="transaction">The database transaction instance.</param>
        /// <returns>Returns the GenerateResult instance.</returns>
        public override GenerateResult Generate(NLib.Components.NDbConnection manager,
            DbTransaction transaction)
        {
            GenerateResult result = null;

            if (null == manager || !manager.IsConnected)
            {
                result = new GenerateResult(
                    new Exception("No connection assigned or connection is not connected."));
            }
            else
            {
                if (null == manager.Factory || null == manager.Factory.Formatter)
                {
                    result = new GenerateResult(
                        new Exception("Db Connection not assigned formatter."));
                    return result;
                }

                string query = manager.Factory.Formatter.SelectMax(this.TableName,
                    this.CompoundKeyNames, this.ColumnKeyValues, this.ColumnName, string.Empty);

                ExecuteResult<object> val = manager.ExecuteScalar(query, transaction);
                if (val.Exception != null)
                {
                    result = new GenerateResult(val.Exception);
                }
                else
                {
                    int maxVal = 0;
                    if (null == val.Result || val.Result == DBNull.Value ||
                        !NLib.Utils.StringUtils.IsInteger(val.Result.ToString()))
                    {
                        maxVal = -1;
                    }
                    else
                    {
                        maxVal = Convert.ToInt32(val.Result.ToString());
                    }
                    result = new GenerateResult(maxVal);
                }
            }

            return result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Coupound Key's Name.
        /// </summary>
        [XmlIgnore]
        public string[] CompoundKeyNames
        {
            get { return _compoundKeyNames; }
            set
            {
                if (_compoundKeyNames != value)
                {
                    _compoundKeyNames = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets Coupound Key's Value.
        /// </summary>
        [XmlIgnore]
        public string[] ColumnKeyValues
        {
            get { return _compoundKeyValues; }
            set
            {
                if (_compoundKeyValues != value)
                {
                    _compoundKeyValues = value;
                }
            }
        }

        #endregion

        #region Static Access

        /// <summary>
        /// Gets Generate Method Name.
        /// </summary>
        public new static string GenerateMethodName
        {
            get { return "CurrentMaxValue"; }
        }
        /// <summary>
        /// Create Generate Method Instance.
        /// </summary>
        /// <returns>Returns new instance of Generate Method.</returns>
        public new static GenerateMethod Create()
        {
            return new CurrentMaxValue();
        }

        #endregion
    }

    #endregion

    #region Current Sequence Value

    /// <summary>
    /// Get Current Sequence Value Generate method class.
    /// </summary>
    public class CurrentSequenceValue : GenerateMethod
    {
        #region Internal Variable

        private string _sequenceName = string.Empty;

        #endregion

        #region Abstract Implements

        /// <summary>
        /// Generate.
        /// </summary>
        /// <param name="manager">The connection manager instance.</param>
        /// <param name="transaction">The database transaction instance.</param>
        /// <returns>Returns the GenerateResult instance.</returns>
        public override GenerateResult Generate(NLib.Components.NDbConnection manager,
            DbTransaction transaction)
        {
            GenerateResult result = null;

            if (null == manager || !manager.IsConnected)
            {
                result = new GenerateResult(
                    new Exception("No connection assigned or connection is not connected."));
            }
            else
            {
                string query = "SELECT " + _sequenceName + ".CURRVAL FROM DUAL";
                ExecuteResult<object> val = manager.ExecuteScalar(query, transaction);
                if (val.Exception != null)
                {
                    result = new GenerateResult(val.Exception);
                }
                else
                {
                    result = new GenerateResult(val.Result);
                }
            }

            return result;
        }

        #endregion

        #region Override Method

        /// <summary>
        /// Gets Parameter string.
        /// </summary>
        /// <returns>Returns the string that represent all parameters in an object.</returns>
        public override string GetParameters()
        {
            return "SequenceName=" + this.SequenceName;
        }
        /// <summary>
        /// Sets Parameter string.
        /// </summary>
        /// <param name="value">The string that represent all parameters in an object.</param>
        public override void SetParameters(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                this.SequenceName = string.Empty;
            }
            else
            {
                string[] keyPairs = value.Split(',');
                if (null == keyPairs || keyPairs.Length <= 0)
                {
                    this.SequenceName = string.Empty;
                }
                else
                {
                    foreach (string keyPair in keyPairs)
                    {
                        if (keyPair.Trim() == string.Empty)
                            continue;
                        string[] keyValues = keyPair.Split('=');
                        if (null == keyValues || keyValues.Length <= 0)
                            continue;

                        if (keyValues.Length >= 1)
                        {
                            if (keyValues[0].Trim() == "SequenceName")
                            {
                                this.SequenceName = string.Empty;
                            }
                        }
                        if (keyValues.Length == 2)
                        {
                            this.SequenceName = keyValues[1].Trim();
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets or sets Sequence's Name.
        /// </summary>
        [XmlIgnore]
        public string SequenceName
        {
            get { return _sequenceName; }
            set
            {
                if (_sequenceName != value)
                {
                    _sequenceName = value;
                }
            }
        }

        #endregion

        #region Static Access

        /// <summary>
        /// Gets Generate Method Name.
        /// </summary>
        public new static string GenerateMethodName
        {
            get { return "CurrentSequenceValue"; }
        }
        /// <summary>
        /// Create Generate Method Instance.
        /// </summary>
        /// <returns>Returns new instance of Generate Method.</returns>
        public new static GenerateMethod Create()
        {
            return new CurrentSequenceValue();
        }

        #endregion
    }

    #endregion

    #region Current Generator Value

    /// <summary>
    /// Get Current Generator Value Generate method class.
    /// </summary>
    public class CurrentGeneratorValue : GenerateMethod
    {
        #region Internal Variable

        private string _generatorName = string.Empty;

        #endregion

        #region Abstract Implements

        /// <summary>
        /// Generate.
        /// </summary>
        /// <param name="manager">The connection manager instance.</param>
        /// <param name="transaction">The database transaction instance.</param>
        /// <returns>Returns the GenerateResult instance.</returns>
        public override GenerateResult Generate(NLib.Components.NDbConnection manager,
            DbTransaction transaction)
        {
            GenerateResult result = null;

            if (null == manager || !manager.IsConnected)
            {
                result = new GenerateResult(
                    new Exception("No connection assigned or connection is not connected."));
            }
            else
            {
                string query = "SELECT GEN_ID(" + _generatorName + ", 0) FROM RDB$DATABASE";
                ExecuteResult<object> val = manager.ExecuteScalar(query, transaction);
                if (val.Exception != null)
                {
                    result = new GenerateResult(val.Exception);
                }
                else
                {
                    result = new GenerateResult(val.Result);
                }
            }

            return result;
        }

        #endregion

        #region Override Method

        /// <summary>
        /// Gets Parameter string.
        /// </summary>
        /// <returns>Returns string that represent all parameters in an object.</returns>
        public override string GetParameters()
        {
            return "GeneratorName=" + this.GeneratorName;
        }
        /// <summary>
        /// Sets Parameter string.
        /// </summary>
        /// <param name="value">The string that represent all parameters in an object.</param>
        public override void SetParameters(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                this.GeneratorName = string.Empty;
            }
            else
            {
                string[] keyPairs = value.Split(',');
                if (null == keyPairs || keyPairs.Length <= 0)
                {
                    this.GeneratorName = string.Empty;
                }
                else
                {
                    foreach (string keyPair in keyPairs)
                    {
                        if (keyPair.Trim() == string.Empty)
                            continue;
                        string[] keyValues = keyPair.Split('=');
                        if (null == keyValues || keyValues.Length <= 0)
                            continue;

                        if (keyValues.Length >= 1)
                        {
                            if (keyValues[0].Trim() == "GeneratorName")
                            {
                                this.GeneratorName = string.Empty;
                            }
                        }
                        if (keyValues.Length == 2)
                        {
                            this.GeneratorName = keyValues[1].Trim();
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets or sets Generator's Name.
        /// </summary>
        [XmlAttribute]
        public string GeneratorName
        {
            get { return _generatorName; }
            set
            {
                if (_generatorName != value)
                {
                    _generatorName = value;
                }
            }
        }

        #endregion

        #region Static Access

        /// <summary>
        /// Gets Generate Method Name.
        /// </summary>
        public new static string GenerateMethodName
        {
            get { return "CurrentGeneratorValue"; }
        }
        /// <summary>
        /// Create Generate Method Instance.
        /// </summary>
        /// <returns>Returns new instance of Generate Method.</returns>
        public new static GenerateMethod Create()
        {
            return new CurrentGeneratorValue();
        }

        #endregion
    }

    #endregion

    #endregion
}
