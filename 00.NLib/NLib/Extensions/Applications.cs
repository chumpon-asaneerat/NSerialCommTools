#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-08-05
=================
- NLib Application Manager Extension Methods changed.
  - Remove DoEvents methods. Used DelegateExtensionMethod instead.

======================================================================================================================
Update 2013-05-13
=================
- NLib Application Manager Extension Methods moved.
  - Split Application Manager Extension Methods to new file.
  - Optimized method for box/unbox issue.
  - Remove Call methods.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Reflection;

#endregion

namespace NLib
{
    #region ApplicationManager Extension Methods

    /// <summary>
    /// The Extenstion methods for Application manager.
    /// </summary>
    public static class ApplicationManagerExtensions
    {
        #region Sleep/Wait/DoEvents

        /// <summary>
        /// Suspends the current thread for a specified time.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The object instance.</param>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds for which the thread is blocked. Specify zero (0) to indicate 
        /// that this thread should be suspended to allow other waiting threads to execute. 
        /// Specify Infinite to block the thread indefinitely. 
        /// </param>
        public static void Sleep<T>(this T value, int millisecondsTimeout)
        {
            // used <T> instead of System.Object to prevent penalty when
            //  pass a value type in then the value type will be boxed. 
            // That creates a performance penalty of allocating the box and doing the copy, 
            // plus of course later having to garbage collect the box.

            ApplicationManager.Instance.Sleep(millisecondsTimeout);
        }
        /// <summary>
        /// Wait.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The object instance.</param>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait. 
        /// </param>
        public static void Wait<T>(this T value, int millisecondsTimeout)
        {
            // used <T> instead of System.Object to prevent penalty when
            //  pass a value type in then the value type will be boxed. 
            // That creates a performance penalty of allocating the box and doing the copy, 
            // plus of course later having to garbage collect the box.

            ApplicationManager.Instance.Wait(millisecondsTimeout);
        }

        #endregion

        #region GC

        /// <summary>
        /// Force Free Garbage Collector.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The object instance.</param>
        public static void FreeGC<T>(this T value)
        {
            // used <T> instead of System.Object to prevent penalty when
            //  pass a value type in then the value type will be boxed. 
            // That creates a performance penalty of allocating the box and doing the copy, 
            // plus of course later having to garbage collect the box.

            NGC.FreeGC();
        }
        /// <summary>
        /// Force Free Garbage Collector.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The object instance.</param>
        /// <param name="obj">object reference to release memory.</param>
        public static void FreeGC<T>(this T value, object obj)
        {
            // used <T> instead of System.Object to prevent penalty when
            //  pass a value type in then the value type will be boxed. 
            // That creates a performance penalty of allocating the box and doing the copy, 
            // plus of course later having to garbage collect the box.

            NGC.FreeGC(obj);
        }

        #endregion
    }

    #endregion
}
