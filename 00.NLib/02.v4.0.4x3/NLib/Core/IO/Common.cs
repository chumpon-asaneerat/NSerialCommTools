using System;
using System.ComponentModel;

namespace NLib
{
    #region IOStatus

    /// <summary>
    /// IOStatus enum.
    /// </summary>
    public enum IOStatus
    {
        /// <summary>
        /// Operation is success.
        /// </summary>
        Success,
        /// <summary>
        /// Operation is failed.
        /// </summary>
        Error
    }

    #endregion

    #region IOOperationResult

    /// <summary>
    /// IOOperationResult class.
    /// </summary>
    public class IOOperationResult : NResult<IOStatus>
    {
        #region Public Properties

        /// <summary>
        /// Checks is operation is success or not.
        /// </summary>
        public bool Success { get { return this.Result == IOStatus.Success; } }

        #endregion
    }

    #endregion

    #region KeyValue

    /// <summary>
    /// KeyValue class. Used for keep reference name and full path name.
    /// </summary>
    public sealed class KeyValue
    {
        #region Internal Variable

        private string _name = string.Empty;
        private string _value = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private KeyValue() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The unique name.</param>
        /// <param name="value">The value associate with the name.</param>
        public KeyValue(string name, string value)
            : this()
        {
            _name = name;
            _value = value;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get the Reference name or common name.
        /// </summary>
        [Category("Directory")]
        [Description("Get the Reference name or common name.")]
        public string Name { get { return _name; } }
        /// <summary>
        /// Get the Value associated with the specificed name.
        /// </summary>
        [Category("Directory")]
        [Description("Get the Value associated with the specificed name.")]
        public string Value { get { return _value; } }

        #endregion
    }

    #endregion
}
