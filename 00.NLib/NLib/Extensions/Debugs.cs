//#define ENABLE_STACKTRACE_VERSION

#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-17
=================
- NLib Extension Methods for Debug framework update.
  - Remove all method that used StackFrame (or StackTrace).
  - Add new methods that supports MethodBase (provide various parameters order for each method).

======================================================================================================================
Update 2014-11-20
=================
- NLib Extension Methods for Debug framework update.
  - Info methods add supports MethodBase.
  - Err methods add supports MethodBase.

======================================================================================================================
Update 2013-07-09
=================
- NLib Extension Methods for Debug framework changed.
  - Redesign code to work with new Debug Framework.
    - Info methods ported and fixed invalid call (original code is call Error method 
      in debug manager).
    - Err methods ported.
    - Dump methods ported.

======================================================================================================================
Update 2013-01-01
=================
- NLib Extension Methods for Debug framework added.
  - Add new Extenstion methods for work with Debug framework.
    Add methods: Info.
    Add methods: Error.
    Add methods: Dump.

======================================================================================================================
// </[History]>
#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using NLib.Reflection;

#endregion

namespace NLib
{
    #region DebugExtensionMethods

    /// <summary>
    /// The Extension methods for Debug framework.
    /// </summary>
    public static class DebugExtensionMethods
    {
        #region Info
#if ENABLE_STACKTRACE_VERSION
        /// <summary>
        /// Write Info to log.
        /// </summary>
        /// <param name="message">The source message.</param>
        /// <param name="args">
        /// The args for replace in string format in case the message is contains format.
        /// </param>
        public static void Info(this string message, params object[] args)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // debugger is disable.

            if (string.IsNullOrWhiteSpace(message))
                return;

            StackFrame sf = new StackFrame(1, false);
            MethodBase callingMethod = sf.GetMethod();

            if (null == callingMethod)
                return;

            string msg = string.Empty;
            try
            {
                if (null != args && args.Length > 0)
                    msg = string.Format(message, args);
                else msg = message;
                // write to log
                DebugManager.Instance.Info(callingMethod, msg);
            }
            catch (Exception ex)
            {
                // should improper format
                DebugManager.Instance.Error(callingMethod, ex);
            }
        }
        /// <summary>
        /// Write Info to log.
        /// </summary>
        /// <typeparam name="T">The target object type.</typeparam>
        /// <param name="inst">The source of extension method.</param>
        /// <param name="message">The message to write to log.</param>
        /// <param name="args">
        /// The args for replace in string format in case the message is contains format.
        /// </param>
        public static void Info<T>(this T inst, string message, params object[] args)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // debugger is disable.

            if (null == inst)
                return;
            if (string.IsNullOrWhiteSpace(message))
                return;

            StackFrame sf = new StackFrame(1, false);
            MethodBase callingMethod = sf.GetMethod();

            if (null == callingMethod)
                return;

            string msg = string.Empty;
            try
            {
                if (null != args && args.Length > 0)
                    msg = string.Format(message, args);
                else msg = message;
                // write to log
                DebugManager.Instance.Info(callingMethod, msg);
            }
            catch (Exception ex)
            {
                // should improper format
                DebugManager.Instance.Error(callingMethod, ex);
            }
        }
#endif
        /// <summary>
        /// Write Info to log.
        /// </summary>
        /// <typeparam name="T">The target object type.</typeparam>
        /// <param name="value">The object instance.</param>
        /// <param name="callingMethod">The calling method.</param>
        /// <param name="message">The info message.</param>
        /// <param name="args">The optional args for info message.</param>
        public static void Info<T>(this T value, MethodBase callingMethod,
            string message, params object[] args)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // Log is not enable.

            if (string.IsNullOrWhiteSpace(message))
                return;

            Type declaringType = (null != callingMethod) ?
                callingMethod.DeclaringType : null;
            if (null == declaringType || null == callingMethod)
                return;

            DebugManager.Instance.Info(callingMethod, message, args);
        }
        /// <summary>
        /// Write Info to log.
        /// </summary>
        /// <param name="message">The info message.</param>
        /// <param name="callingMethod">The calling method.</param>
        /// <param name="args">The optional args for info message.</param>
        public static void Info(this string message, MethodBase callingMethod, 
            params object[] args)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // Log is not enable.

            if (string.IsNullOrWhiteSpace(message))
                return;

            Type declaringType = (null != callingMethod) ?
                callingMethod.DeclaringType : null;
            if (null == declaringType || null == callingMethod)
                return;

            DebugManager.Instance.Info(callingMethod, message, args);
        }
        /// <summary>
        /// Write Info to log.
        /// </summary>
        /// <param name="callingMethod">The calling method.</param>
        /// <param name="message">The info message.</param>
        /// <param name="args">The optional args for info message.</param>
        public static void Info(this MethodBase callingMethod, string message,
            params object[] args)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // Log is not enable.

            if (string.IsNullOrWhiteSpace(message))
                return;

            Type declaringType = (null != callingMethod) ?
                callingMethod.DeclaringType : null;
            if (null == declaringType || null == callingMethod)
                return;

            DebugManager.Instance.Info(callingMethod, message, args);
        }

        #endregion

        #region Err
#if ENABLE_STACKTRACE_VERSION
        /// <summary>
        /// Write Error to log.
        /// </summary>
        /// <param name="errorMessage">The source error message.</param>
        /// <param name="args">
        /// The args for replace in string format in case the message is contains format.
        /// </param>
        public static void Err(this string errorMessage, params object[] args)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // debugger is disable.

            if (string.IsNullOrWhiteSpace(errorMessage))
                return;

            StackFrame sf = new StackFrame(1, false);
            MethodBase callingMethod = sf.GetMethod();

            if (null == callingMethod)
                return;

            string msg = string.Empty;
            try
            {
                if (null != args && args.Length > 0)
                    msg = string.Format(errorMessage, args);
                else msg = errorMessage;
                // write to log
                DebugManager.Instance.Error(callingMethod, msg);
            }
            catch (Exception ex)
            {
                // should improper format
                DebugManager.Instance.Error(callingMethod, ex);
            }
        }
        /// <summary>
        /// Write Error to log.
        /// </summary>
        /// <typeparam name="T">The target object type.</typeparam>
        /// <param name="inst">The source of extension method.</param>
        /// <param name="errorMessage">The message to write to log.</param>
        /// <param name="args">
        /// The args for replace in string format in case the message is contains format.
        /// </param>
        public static void Err<T>(this T inst, string errorMessage, params object[] args)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // debugger is disable.

            if (null == inst)
                return;
            if (string.IsNullOrWhiteSpace(errorMessage))
                return;

            StackFrame sf = new StackFrame(1, false);
            MethodBase callingMethod = sf.GetMethod();

            if (null == callingMethod)
                return;

            string msg = string.Empty;
            try
            {
                if (null != args && args.Length > 0)
                    msg = string.Format(errorMessage, args);
                else msg = errorMessage;
                // write to log
                DebugManager.Instance.Error(callingMethod, msg);
            }
            catch (Exception ex)
            {
                // should improper format
                DebugManager.Instance.Error(callingMethod, ex);
            }
        }
        /// <summary>
        /// Write Error to log.
        /// </summary>
        /// <param name="ex">The source exception.</param>
        public static void Err(this Exception ex)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // debugger is disable.

            if (null == ex)
                return;

            StackFrame sf = new StackFrame(1, false);
            MethodBase callingMethod = sf.GetMethod();

            if (null == callingMethod)
                return;
            // write to log
            DebugManager.Instance.Error(callingMethod, ex);
        }
        /// <summary>
        /// Write Error to log.
        /// </summary>
        /// <typeparam name="T">The target object type.</typeparam>
        /// <param name="inst">The source of extension method.</param>
        /// <param name="ex">The exception to write to log.</param>
        public static void Err<T>(this T inst, Exception ex)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // debugger is disable.

            if (null == inst)
                return;
            if (null == ex)
                return;

            StackFrame sf = new StackFrame(1, false);
            MethodBase callingMethod = sf.GetMethod();

            if (null == callingMethod)
                return;
            // write to log
            DebugManager.Instance.Error(callingMethod, ex);
        }
#endif
        /// <summary>
        /// Write Error to all debugger.
        /// </summary>
        /// <typeparam name="T">The target object type.</typeparam>
        /// <param name="value">The object instance.</param>
        /// <param name="callingMethod">The calling method.</param>
        /// <param name="ex">The Exception instance.</param>
        public static void Err<T>(this T value, MethodBase callingMethod,
            Exception ex)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // Log is not enable.

            if (null == ex)
                return;

            Type declaringType = (null != callingMethod) ?
                callingMethod.DeclaringType : null;
            if (null == declaringType || null == callingMethod)
                return;

            DebugManager.Instance.Error(callingMethod, ex);
        }
        /// <summary>
        /// Write Error to all debugger.
        /// </summary>
        /// <typeparam name="T">The target object type.</typeparam>
        /// <param name="value">The object instance.</param>
        /// <param name="callingMethod">The calling method.</param>
        /// <param name="errorMessage">The Error message.</param>
        /// <param name="args">The optional args for error message.</param>
        public static void Err<T>(this T value, MethodBase callingMethod,
            string errorMessage, params object[] args)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // Log is not enable.

            if (string.IsNullOrWhiteSpace(errorMessage))
                return;

            Type declaringType = (null != callingMethod) ?
                callingMethod.DeclaringType : null;
            if (null == declaringType || null == callingMethod)
                return;

            DebugManager.Instance.Error(callingMethod, errorMessage, args);
        }
        /// <summary>
        /// Write Error to all debugger.
        /// </summary>
        /// <typeparam name="T">The target object type.</typeparam>
        /// <param name="value">The object instance.</param>
        /// <param name="callingMethod">The calling method.</param>
        /// <param name="ex">The Exception instance.</param>
        /// <param name="errorMessage">The Error message.</param>
        /// <param name="args">The optional args for error message.</param>
        public static void Err<T>(this T value, MethodBase callingMethod,
            Exception ex, string errorMessage, params object[] args)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // Log is not enable.

            if (null == ex && string.IsNullOrWhiteSpace(errorMessage))
                return;

            Type declaringType = (null != callingMethod) ?
                callingMethod.DeclaringType : null;
            if (null == declaringType || null == callingMethod)
                return;

            DebugManager.Instance.Error(callingMethod, ex);
            DebugManager.Instance.Error(callingMethod, errorMessage, args);
        }
        /// <summary>
        /// Write Error to all debugger.
        /// </summary>
        /// <param name="callingMethod">The calling method.</param>
        /// <param name="ex">The source exception.</param>
        public static void Err(this MethodBase callingMethod, Exception ex)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // Log is not enable.

            if (null == ex)
                return;

            Type declaringType = (null != callingMethod) ?
                callingMethod.DeclaringType : null;
            if (null == declaringType || null == callingMethod)
                return;

            DebugManager.Instance.Error(callingMethod, ex);
        }
        /// <summary>
        /// Write Error to all debugger.
        /// </summary>
        /// <param name="callingMethod">The calling method.</param>
        /// <param name="errorMessage">The Error message.</param>
        /// <param name="args">The optional args for error message.</param>
        public static void Err(this MethodBase callingMethod, string errorMessage,
            params object[] args)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // Log is not enable.

            if (string.IsNullOrWhiteSpace(errorMessage))
                return;

            Type declaringType = (null != callingMethod) ?
                callingMethod.DeclaringType : null;
            if (null == declaringType || null == callingMethod)
                return;

            DebugManager.Instance.Error(callingMethod, errorMessage, args);
        }
        /// <summary>
        /// Write Error to all debugger.
        /// </summary>
        /// <param name="callingMethod">The calling method.</param>
        /// <param name="ex">The source exception.</param>
        /// <param name="errorMessage">The Error message.</param>
        /// <param name="args">The optional args for error message.</param>
        public static void Err(this MethodBase callingMethod, Exception ex,
            string errorMessage, params object[] args)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // Log is not enable.

            if (null == ex && string.IsNullOrWhiteSpace(errorMessage))
                return;

            Type declaringType = (null != callingMethod) ?
                callingMethod.DeclaringType : null;
            if (null == declaringType || null == callingMethod)
                return;

            DebugManager.Instance.Error(callingMethod, ex);
            DebugManager.Instance.Error(callingMethod, errorMessage, args);
        }
        /// <summary>
        /// Write Error to all debugger.
        /// </summary>
        /// <param name="ex">The source exception.</param>
        /// <param name="callingMethod">The calling method.</param>
        public static void Err(this Exception ex, MethodBase callingMethod)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // Log is not enable.

            if (null == ex)
                return;

            Type declaringType = (null != callingMethod) ?
                callingMethod.DeclaringType : null;
            if (null == declaringType || null == callingMethod)
                return;

            DebugManager.Instance.Error(callingMethod, ex);
        }
        /// <summary>
        /// Write Error to all debugger.
        /// </summary>
        /// <param name="errorMessage">The Error message.</param>
        /// <param name="callingMethod">The calling method.</param>
        /// <param name="args">The optional args for error message.</param>
        public static void Err(this string errorMessage, MethodBase callingMethod, 
            params object[] args)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // Log is not enable.

            if (string.IsNullOrWhiteSpace(errorMessage))
                return;

            Type declaringType = (null != callingMethod) ?
                callingMethod.DeclaringType : null;
            if (null == declaringType || null == callingMethod)
                return;

            DebugManager.Instance.Error(callingMethod, errorMessage, args);
        }
        /// <summary>
        /// Write Error to all debugger.
        /// </summary>
        /// <param name="ex">The source exception.</param>
        /// <param name="callingMethod">The calling method.</param>
        /// <param name="errorMessage">The Error message.</param>
        /// <param name="args">The optional args for error message.</param>
        public static void Err(this Exception ex, MethodBase callingMethod,
            string errorMessage, params object[] args)
        {
            if (!DebugManager.Instance.IsEnable)
                return; // Log is not enable.

            if (null == ex && string.IsNullOrWhiteSpace(errorMessage))
                return;

            Type declaringType = (null != callingMethod) ?
                callingMethod.DeclaringType : null;
            if (null == declaringType || null == callingMethod)
                return;

            DebugManager.Instance.Error(callingMethod, ex);
            DebugManager.Instance.Error(callingMethod, errorMessage, args);
        }

        #endregion

        #region Dump
#if ENABLE_STACKTRACE_VERSION
        /// <summary>
        /// Dump using Reflecion and caching. This is the faster version.
        /// Usage. new Dump().(new { var1 }).
        /// </summary>
        /// <typeparam name="I">The instance type. Can be any object.</typeparam>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="inst">The dump instance.</param>
        /// <param name="item">The instance of item.</param>
        /// <param name="propertyName">
        /// The instance of property to read. Do not required if property's value can read properlys.
        /// </param>
        public static void Dump<I, T>(this I inst, T item,
            string propertyName = "")
            where T : class
        {
            if (!DebugManager.Instance.IsEnable)
                return; // debugger is disable.

            // used <I> instead of System.Object to prevent penalty when
            //  pass a value type in then the value type will be boxed. 
            // That creates a performance penalty of allocating the box and doing the copy, 
            // plus of course later having to garbage collect the box.
            if (null == inst)
                return;
            if (null == item)
                return;
            StackFrame sf = new StackFrame(1, false);
            MethodBase callingMethod = sf.GetMethod();

            if (null == callingMethod)
                return;

            NVariableInfo varInfo = new NVariableInfo();
            ObjectInfo objInfo = VariableAccess.GetMetadata(item);

            if (null != objInfo)
            {
                varInfo.Name = objInfo.Name;
                varInfo.Value = objInfo.GetValue(propertyName);

                // write to log
                DebugManager.Instance.Dump(callingMethod, varInfo);
            }
        }
#endif
        #endregion
        /// <summary>
        /// Dump using Reflecion and caching. This is the faster version.
        /// Usage. new Dump().(MethodBase.GetCurrentMethod(), new { var1 }).
        /// </summary>
        /// <typeparam name="I">The instance type. Can be any object.</typeparam>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="inst">The dump instance.</param>
        /// <param name="callingMethod">The calling method.</param>
        /// <param name="item">The instance of item.</param>
        /// <param name="propertyName">
        /// The instance of property to read. Do not required if property's value can read properlys.
        /// </param>
        public static void Dump<I, T>(this I inst, MethodBase callingMethod, T item,
            string propertyName = "")
            where T : class
        {
            if (!DebugManager.Instance.IsEnable)
                return; // debugger is disable.

            // used <I> instead of System.Object to prevent penalty when
            //  pass a value type in then the value type will be boxed. 
            // That creates a performance penalty of allocating the box and doing the copy, 
            // plus of course later having to garbage collect the box.
            if (null == inst)
                return;
            if (null == item)
                return;
            if (null == callingMethod)
                return;

            NVariableInfo varInfo = new NVariableInfo();
            ObjectInfo objInfo = VariableAccess.GetMetadata(item);

            if (null != objInfo)
            {
                varInfo.Name = objInfo.Name;
                varInfo.Value = objInfo.GetValue(propertyName);

                // write to log
                DebugManager.Instance.Dump(callingMethod, varInfo);
            }
        }
        /// <summary>
        /// Dump using Reflecion and caching. This is the faster version.
        /// Usage. MethodBase.GetCurrentMethod().Dump().(new { var1 }).
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="callingMethod">The calling method.</param>
        /// <param name="item">The instance of item.</param>
        /// <param name="propertyName">
        /// The instance of property to read. Do not required if property's value can read properlys.
        /// </param>
        public static void Dump<T>(this MethodBase callingMethod, T item,
            string propertyName = "")
            where T : class
        {
            if (!DebugManager.Instance.IsEnable)
                return; // debugger is disable.

            // used <I> instead of System.Object to prevent penalty when
            //  pass a value type in then the value type will be boxed. 
            // That creates a performance penalty of allocating the box and doing the copy, 
            // plus of course later having to garbage collect the box.
            if (null == item)
                return;
            if (null == callingMethod)
                return;

            NVariableInfo varInfo = new NVariableInfo();
            ObjectInfo objInfo = VariableAccess.GetMetadata(item);

            if (null != objInfo)
            {
                varInfo.Name = objInfo.Name;
                varInfo.Value = objInfo.GetValue(propertyName);

                // write to log
                DebugManager.Instance.Dump(callingMethod, varInfo);
            }
        }
    }

    #endregion
}
