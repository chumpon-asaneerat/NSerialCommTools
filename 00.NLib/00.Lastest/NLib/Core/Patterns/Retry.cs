#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-12-07
=================
- Design Pattern Framework - Retry
  - Ported Retry classes from GFA40.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#endregion

namespace NLib
{
    #region Retry Actions (Static Style)

    /// <summary>
    /// Retry Actions class. This class run in UI Thread.
    /// </summary>
    public static class RetryActions
    {
        /// <summary>
        /// Retry action.
        /// </summary>
        /// <typeparam name="T">Return Result Type.</typeparam>
        /// <param name="function">The function to process.</param>
        /// <param name="numberOfRetries">Number to retry if error occur.</param>
        /// <param name="msPause">Time to pause before next retry.</param>
        /// <param name="throwExceptions">True to throw exception.</param>
        /// <returns>Results Result from execute function.</returns>
        public static T Retry<T>(Func<T> function, int numberOfRetries, int msPause, 
            bool throwExceptions)
        {
            int counter = 0;
        BeginLabel:
            try
            {
                counter++;
                return function.Invoke();

            }
            catch (Exception ex)
            {
                if (counter > numberOfRetries)
                {
                    if (throwExceptions) throw ex;
                    else return default(T);
                }
                else
                {
                    Thread.Sleep(msPause);
                    goto BeginLabel;
                }
            }
        }
        /// <summary>
        /// Retry action.
        /// </summary>
        /// <param name="action">The action to process.</param>
        /// <param name="numberOfRetries">Number to retry if error occur.</param>
        /// <param name="msPause">Time to pause before next retry.</param>
        /// <param name="throwExceptions">True to throw exception.</param>
        /// <returns>Results True if action is successfully execute without error.</returns>
        public static bool Retry(Action action, int numberOfRetries, int msPause, 
            bool throwExceptions)
        {
            int counter = 0;
        BeginLabel:
            try
            {
                counter++;
                action.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                if (counter > numberOfRetries)
                {
                    if (throwExceptions) throw ex;
                    else return false;
                }
                else
                {
                    Thread.Sleep(msPause);
                    goto BeginLabel;
                }
            }
        }
    }

    #endregion

    #region Retry (Functional Style)

    /// <summary>
    /// The Retry class. This class run in UI Thread.
    /// </summary>
    public class Retry
    {
        #region Internal Variables
        
        private Predicate<Exception> _shouldRetry;
        private Type _type = null;
        private uint _times = 1;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public Retry() : base()
        {
            _shouldRetry = DefaultShouldRetry;
        }

        #endregion

        #region Private Method

        private bool DefaultShouldRetry(Exception e)
        {
            if (_type == null)
                return true;
            if (!_type.IsAssignableFrom(e.GetType()))
                return false;
            return true;
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Set number to retry.
        /// </summary>
        /// <param name="times">The Number to retry.</param>
        /// <returns>Returns instance of Retry class.</returns>
        public static Retry Times(uint times)
        {
            Retry retry = new Retry();
            retry._times = times;
            return retry;
        }
        /// <summary>
        /// Checks when target excetion detected the action. The T type will used in default 
        /// should retry internal method.
        /// </summary>
        /// <typeparam name="T">The target exeption type.</typeparam>
        /// <returns>Always returns this instance to provide functional programming style.</returns>
        public Retry When<T>() where T : Exception
        {
            _type = typeof(T);
            return this;
        }
        /// <summary>
        /// Set delegate to handler if match exception detected the action mehod should retry. 
        /// The predicate delegate will replace default should retry internal method.
        /// This method should be set before call Do method.
        /// Or we can write psudo code look like 'Retry' X 'Times] 'If' exception detect so 'Do' method.
        /// </summary>
        /// <param name="predicate">The exception matching function.</param>
        /// <returns>Always returns this instance to provide functional programming style.</returns>
        public Retry If(Predicate<Exception> predicate)
        {
            _shouldRetry = predicate;
            return this;
        }
        /// <summary>
        /// Execute Action. (The method will run in UI Thread).
        /// </summary>
        /// <param name="method">The Action delegate.</param>
        public void Do(Action method)
        {
            for (int count = 0; count < _times; count++)
            {
                try
                {
                    method();
                    break; // if no exception occur immediately terminate retry loop.
                }
                catch (Exception e)
                {
                    if (!_shouldRetry(e))
                        throw e;
                }
            }
        }

        #endregion
    }

    #endregion
}
