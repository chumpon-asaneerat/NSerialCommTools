#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-16
=================
- Reflection library updated.
  - Add Cache to keep for each type that is WPF or not for improve overall performance.
  - Add DispatcherPriority.Normal in Dispacher.Invoke method. 
  - Add Dispatcher.CheckAccess() code to check is is same as UI thread (so no need to used
    Dispatcher.Invoke method. This made speed gain when method is in UI thread.

======================================================================================================================
Update 2013-07-09
=================
- Reflection library updated.
  - DelegateInvoker is reimplements base on Application Controller.
  - Remove IsExit checking for decouble relation with another class.

======================================================================================================================
Update 2010-08-31
=================
- Invoker library ported.
  - DelegateInvoker is ported from GFA38v3 to GFA40.
  - DelegateMarshaler is no longer supported in GFA40.

======================================================================================================================
Update 2010-04-05
=================
- Delegate Marshaler added.
  - Ported Delegate Marshaler from 
    http://blog.quantumbitdesigns.com/2008/07/22/delegatemarshaler-replace-controlinvokerequired-and-controlinvoke/

======================================================================================================================
Update 2010-01-22
=================
- Delegate Invoker updated
  - DelegateInvoker class change Exception handle related code to used new version of 
    ExceptionManager.

======================================================================================================================
Update 2010-01-16
=================
- Delegate Invoker updated
  - All code related to write error log will now used ExceptionManager instead. 

======================================================================================================================
Update 2009-12-27
=================
- Delegate Invoker updated
  - DelegateInvoker class added some check for match arguments that required to 
    invoke delegate if arguments is not match the exception will throw for
    make caller warning and fixed it.

======================================================================================================================
Update 2009-12-25
=================
- Delegate Invoker ported
  - DelegateInvoker class is ported from GFA37 to GFA38
  - DelegateInvoker class log code is temporary removed.

======================================================================================================================
Update 2008-11-26
=================
- Delegate Invoker move from GFA.Lib

======================================================================================================================
Update 2008-05-30
=================
- Delegate Invoker add log category for log filter purpose.

======================================================================================================================
Update 2008-05-30
=================
- Reflection Library add new function
  - Delegate Invoker add new method Invoke with return value to used with delegate than can return value after execute.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;

#endregion

namespace NLib.Reflection
{
    /// <summary>
    /// The Delegate Invoker. This class contains static method to invoke delegate without concern about
    /// cross thread problem.
    /// </summary>
    public static class DelegateInvoker
    {
        #region Delegate

        private delegate void EmptyDelegate();

        #endregion

        #region Internal Variables

        private static object _lock = new object();
        private static Dictionary<Type, bool> _caches = new Dictionary<Type, bool>();

        #endregion

        #region Private Methods

        private static bool HasDispatcher(object obj)
        {
            bool result = false;
            Type targetType = (null != obj) ? obj.GetType() : null;
            if (null != targetType)
            {
                if (!_caches.ContainsKey(targetType))
                {
                    bool isDispatcherObject =
                        targetType.IsSubclassOf(typeof(System.Windows.Threading.DispatcherObject)) ||
                        targetType.IsSubclassOf(typeof(System.Windows.DependencyObject));

                    if (!isDispatcherObject)
                    {
                        System.Reflection.PropertyInfo prop =
                            targetType.GetProperty("Dispatcher");
                        result = (null != prop); // property found.
                    }
                    else
                    {
                        // target is inherited from DispatcherObject or DependencyObject
                        result = true;
                    }

                    lock (_lock)
                    {
                        _caches.Add(targetType, result);
                    }
                }
                else
                {
                    lock (_lock)
                    {
                        result = _caches[targetType];
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Invoke Delegate.
        /// </summary>
        /// <param name="del">delegate to invoke</param>
        /// <param name="args">args for delegate</param>
        /// <returns>Return result of invocation.</returns>
        private static object __Invoke(Delegate del, object[] args)
        {
            object result = null;
            if (null == del)
                return result;

            // if we can find propery to handle it so this flag will set to be true
            bool isHandled = false;

            if (null != del.Target)
            {
                // Checks is windows forms UI
                if (!HasDispatcher(del.Target))
                {
                    ISynchronizeInvoke syncInvoke =
                        (null != del.Target && del.Target is ISynchronizeInvoke) ?
                        (del.Target as ISynchronizeInvoke) : null;
                    if (null != syncInvoke && syncInvoke.InvokeRequired)
                    {
                        // Detected target is ISynchronizeInvoke and in anothre thread
                        // This is the general case when used in windows forms.
                        isHandled = true;
                        result = syncInvoke.Invoke(del, args);
                    }
                }

                if (!isHandled)
                {
                    // Checks is WPF UI
                    Dispatcher dispatcher = null;
                    dispatcher = DynamicAccess.Get(del.Target, "Dispatcher") as Dispatcher;
                    if (null != dispatcher && !dispatcher.CheckAccess())
                    {
                        // Dispatcher detected so it's is WPF object that the delegate should
                        // invoke via dispatcher.
                        isHandled = true;
                        result = dispatcher.Invoke(del, DispatcherPriority.Normal, args);
                    }
                }
            }

            if (!isHandled)
            {
                // cannot find the way to handle it or it's run in same as UI thread
                // so it's should be no problem in UI thread.
                result = del.DynamicInvoke(args);
            }

            return result;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Application DoEvents.
        /// </summary>
        public static void DoEvents()
        {
            bool handled = false;
            if (null != Dispatcher.CurrentDispatcher)
            {
                // Detected is WPF
                handled = true;
                try
                {
                    Wpf.DoEvents(DispatcherPriority.Background, true);
                    //Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
                }
                catch (Exception)
                {
                    //Console.WriteLine(ex);
                }
            }
            if (!handled)
            {
                // Used Windows Forms
                try
                {
                    handled = true;
                    System.Windows.Forms.Application.DoEvents();
                }
                catch (Exception)
                {
                    //Console.WriteLine(ex);
                }
            }
            if (!handled)
            {
                // Non UI type application so no need to implements
            }
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
        public static object Invoke(Delegate del, params object[] args)
        {
            object result = null;

            if (del == null || del.Method == null)
                return result;

            int requiredParameters = del.Method.GetParameters().Length;
            // Check that the correct number of arguments have been supplied
            if (requiredParameters != args.Length)
            {
                throw new ArgumentException(string.Format(
                     "{0} arguments provided when {1} {2} required.",
                     args.Length, requiredParameters,
                     ((requiredParameters == 1) ? "is" : "are")));
            }

            // Get a local copy of the invocation list in case it changes
            Delegate[] delegates = del.GetInvocationList();
            if (delegates != null && delegates.Length > 0)
            {
                foreach (Delegate sink in delegates)
                {
                    try
                    {
                        result = __Invoke(sink, args);
                    }
                    catch (ObjectDisposedException) { }
                    catch (Exception)
                    {
                        //Console.WriteLine(ex);
                    }
                    finally { }
                }
            }
            return result;
        }

        #endregion

        #region Wpf helper class

        /// <summary>
        /// Wpf helper class.
        /// </summary>
        public static class Wpf
        {
            /// <summary>
            /// Application DoEvents (WPF).
            /// </summary>
            /// <param name="dp">The DispatcherPriority mode.</param>
            /// <param name="simple">True for simple mode.</param>
            public static void DoEvents(DispatcherPriority dp = DispatcherPriority.Render, bool simple = true)
            {
                if (!simple)
                {
                    if (null != Dispatcher.CurrentDispatcher)
                    {
                        var frame = new DispatcherFrame();
                        Dispatcher.CurrentDispatcher.BeginInvoke(dp,
                            new DispatcherOperationCallback((object parameter) =>
                            {
                                ((DispatcherFrame)parameter).Continue = false;
                                return null;
                            }), frame);
                        Dispatcher.PushFrame(frame);
                    }
                }
                else
                {
                    if (null != Dispatcher.CurrentDispatcher)
                    {
                        Dispatcher.CurrentDispatcher.Invoke(dp, new Action(() => { }));
                    }
                }
            }
        }

        #endregion
    }
}
