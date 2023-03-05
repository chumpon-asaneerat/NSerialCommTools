#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-17
=================
- NLib Extension Methods for Delegates updated.
  - Invoke method change type of first parameter from object to <T>.

======================================================================================================================
Update 2013-01-01
=================
- NLib Extension Methods for Delegates added.
  - Add new Extenstion methods for work with delegates.
    - Add method: DoEvents for acts like Application.DoEvents in windows forms.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;

#endregion

namespace NLib
{
    #region DelegateExtensionMethods

    /// <summary>
    /// The Extension methods for Delegate.
    /// </summary>
    public static class DelegateExtensionMethods
    {
        #region DoEvents

        /// <summary>
        /// Application DoEvents.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">An object variable.</param>
        public static void DoEvents<T>(this T value)
        {
            // used <T> instead of System.Object to prevent penalty when
            //  pass a value type in then the value type will be boxed. 
            // That creates a performance penalty of allocating the box and doing the copy, 
            // plus of course later having to garbage collect the box.

            ApplicationManager.Instance.DoEvents();
        }

        #endregion

        #region Invoke/Call

        /// <summary>
        /// Executes the specified delegate, on the thread that owns the 
        /// UI object's underlying window handle, with the specified list of arguments.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The object instance.</param>
        /// <param name="del">
        /// A delegate to a method that takes parameters of the same number and type that 
        /// are contained in the args parameter.
        /// </param>
        /// <param name="args">
        /// An array of objects to pass as arguments to the specified method. 
        /// This parameter can be null if the method takes no arguments. 
        /// </param>
        /// <returns>
        /// An Object that contains the return value from the delegate being invoked, 
        /// or null if the delegate has no return value.
        /// </returns>
        public static object Invoke<T>(this T value,
            Delegate del, params object[] args)
        {
            return ApplicationManager.Instance.Invoke(del, args);
        }
        /// <summary>
        /// Executes the specified delegate, on the thread that owns the 
        /// UI object's underlying window handle, with the specified list of arguments.
        /// </summary>
        /// <param name="del">
        /// A delegate to a method that takes parameters of the same number and type that 
        /// are contained in the args parameter.
        /// </param>
        /// <param name="args">
        /// An array of objects to pass as arguments to the specified method. 
        /// This parameter can be null if the method takes no arguments. 
        /// </param>
        /// <returns>
        /// An Object that contains the return value from the delegate being invoked, 
        /// or null if the delegate has no return value.
        /// </returns>
        public static object Call(this Delegate del, params object[] args)
        {
            if (null == del)
                return null;
            return ApplicationManager.Instance.Invoke(del, args);
        }
        /// <summary>
        /// Executes the specified delegate, on the thread that owns the 
        /// UI object's underlying window handle, with the specified list of arguments.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="del">
        /// A delegate to a method that takes parameters of the same number and type that 
        /// are contained in the args parameter.
        /// </param>
        /// <param name="args">
        /// An array of objects to pass as arguments to the specified method. 
        /// This parameter can be null if the method takes no arguments. 
        /// </param>
        /// <returns>
        /// An Object that contains the return value from the delegate being invoked, 
        /// or null if the delegate has no return value.
        /// </returns>
        public static T Call<T>(this Delegate del, params object[] args)
        {
            if (null == del)
                return default(T);
            object val = null;
            try
            {
                val = ApplicationManager.Instance.Invoke(del, args);
                if (null == val)
                    return default(T);
                else return (T)val;
            }
            catch (Exception)
            {
                // Error occur.
                return default(T);
            }
        }

        #endregion
    }

    #endregion
}
