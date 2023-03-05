#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-05-04
=================
- NLib - Common.
  - Add NResult<T> class.
  - Add OperationResult<T> class.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;

#endregion

namespace NLib
{
    #region NResult<T>

    /// <summary>
    /// NResult class.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    public class NResult<T>
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NResult()
            : base()
        {
            this.Result = default(T);
            this.Err = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets result instance.
        /// </summary>
        public T Result { get; set; }
        /// <summary>
        /// Gets or sets error instance.
        /// </summary>
        public Exception Err { get; set; }
        /// <summary>
        /// Checks is error instance is exits.
        /// </summary>
        public bool HasError { get { return (null != this.Err); } }

        #endregion

        #region Operator Overloading

        /// <summary>
        /// implicit convert to T
        /// </summary>
        /// <param name="value">The NResult instance.</param>
        /// <returns>Returns value that represent data in T instance.</returns>
        public static implicit operator T(NResult<T> value)
        {
            if (value == null)
                return default(T);
            else return value.Result; // Return Result
        }

        #endregion
    }

    #endregion

    #region OperationResult<T>

    /// <summary>
    /// OperationResult class.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    public class OperationResult<T> : NResult<T>
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public OperationResult()
            : base()
        {
            this.Success = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets is success. 
        /// Note. In some case the Result may be null but operation is success.
        /// </summary>
        public bool Success { get; set; }

        #endregion

        #region Operator Overloading

        /// <summary>
        /// implicit convert to T.
        /// </summary>
        /// <param name="value">The OperationResult instance.</param>
        /// <returns>string that represent data in T instance.</returns>
        public static implicit operator T(OperationResult<T> value)
        {
            if (value == null)
                return default(T);
            else return value.Result; // Return Result
        }
        /// <summary>
        /// implicit convert to bool.
        /// </summary>
        /// <param name="value">The OperationResult instance.</param>
        /// <returns>string that represent data in T instance.</returns>
        public static implicit operator bool(OperationResult<T> value)
        {
            if (value == null)
                return false;
            else return value.Success; // Return Result
        }

        #endregion
    }

    #endregion
}

