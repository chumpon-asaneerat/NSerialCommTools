#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-01-07
=================
- Add High Performance Timers class.
  - Add NStopWatch class.
  - Add MicroStopWatch class.
  - Add MicroTimer class and related delegate/event args class.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

#endregion

namespace NLib.Utils
{
    #region NStopWatch

    /// <summary>
    /// The high-resolution performance timer class used Win32 API.
    /// </summary>
    public class NStopWatch
    {
        #region Win32

        /// <summary>
        /// QueryPerformanceFrequency.
        /// </summary>
        /// <param name="lpPerformanceCount">The counter.</param>
        /// <returns>Returns true if OS Supports.</returns>
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
        /// <summary>
        /// QueryPerformanceFrequency.
        /// </summary>
        /// <param name="lpFrequency">The Frequency.</param>
        /// <returns>Returns true if OS Supports.</returns>
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        #endregion

        #region Internal Variables

        private long startTime, stopTime;
        private long freq;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NStopWatch() : base()
        {
            startTime = 0;
            stopTime = 0;

            if (!QueryPerformanceFrequency(out freq))
            {
                // high-performance counter not supported
                throw new Win32Exception();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start timer.
        /// </summary>
        public void Start()
        {
            // lets do the waiting threads there work
            Thread.Sleep(0);
            QueryPerformanceCounter(out startTime);
        }
        /// <summary>
        /// Stop timer.
        /// </summary>
        public void Stop()
        {
            QueryPerformanceCounter(out stopTime);
        }

        #endregion

        #region Public Properties
        
        /// <summary>
        /// Returns the duration of the timer (in seconds).
        /// </summary>
        public double Duration
        {
            get
            {
                return (double)(stopTime - startTime) / (double)freq;
            }
        }

        #endregion
    }

    #endregion

    #region MicroStopwatch

    /// <summary>
    /// The MicroStopWatch class.
    /// </summary>
    public class MicroStopWatch : System.Diagnostics.Stopwatch
    {
        #region Internal Variables

        private readonly double _microSecPerTick =
            1000000D / Stopwatch.Frequency;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MicroStopWatch()
        {
            if (!Stopwatch.IsHighResolution)
            {
                throw new Exception("On this system the high-resolution " +
                                    "performance counter is not available");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Elapsed Time in micro second.
        /// </summary>
        public long ElapsedMicroseconds
        {
            get
            {
                return (long)(ElapsedTicks * _microSecPerTick);
            }
        }

        #endregion
    }

    #endregion

    #region MicroTimerElapsedEventHandler and MicroTimerEventArgs

    /// <summary>
    /// The MicroTimer Elapsed Event Handler.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="timerEventArgs">The MicroTimer Event Argument.</param>
    public delegate void MicroTimerElapsedEventHandler(object sender, 
        MicroTimerEventArgs timerEventArgs);
    /// <summary>
    /// MicroTimer Event Argument class
    /// </summary>
    public class MicroTimerEventArgs : EventArgs
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timerCount">
        /// The Simple counter, number times timed event (callback function) executed.
        /// </param>
        /// <param name="elapsedMicroseconds">
        /// The Time when timed event was called since timer started.
        /// </param>
        /// <param name="timerLateBy">
        /// The value that determine how late the timer was compared to when it should have been called.
        /// </param>
        /// <param name="callbackFunctionExecutionTime">
        /// The Time it took to execute previous call to callback function (OnTimedEvent).
        /// </param>
        public MicroTimerEventArgs(int timerCount,
                                   long elapsedMicroseconds,
                                   long timerLateBy,
                                   long callbackFunctionExecutionTime)
        {
            TimerCount = timerCount;
            ElapsedMicroseconds = elapsedMicroseconds;
            TimerLateBy = timerLateBy;
            CallbackFunctionExecutionTime = callbackFunctionExecutionTime;
        }
        
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Simple counter, number times timed event (callback function) executed.
        /// </summary>
        public int TimerCount { get; private set; }
        /// <summary>
        /// Gets Time when timed event was called since timer started.
        /// </summary>
        public long ElapsedMicroseconds { get; private set; }
        /// <summary>
        /// Gets How late the timer was compared to when it should have been called.
        /// </summary>
        public long TimerLateBy { get; private set; }
        /// <summary>
        /// Gets Time it took to execute previous call to callback function (OnTimedEvent).
        /// </summary>
        public long CallbackFunctionExecutionTime { get; private set; }

        #endregion
    }

    #endregion

    #region MicroTimer
    
    /// <summary>
    /// The MicroTimer class. Like Timer but can tick in microsecond.
    /// Note. The internal thread is execute in Highest piority. 
    /// </summary>
    public class MicroTimer
    {
        #region Internal Variables

        private Thread _threadTimer = null;
        private long _ignoreEventIfLateBy = long.MaxValue;
        private long _timerIntervalInMicroSec = 0;
        private bool _stopTimer = true;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MicroTimer() : base()
        {

        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timerIntervalInMicroseconds">The tick interval.</param>
        public MicroTimer(long timerIntervalInMicroseconds)
        {
            Interval = timerIntervalInMicroseconds;
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~MicroTimer()
        {
            // Stop the timer, wait for up to 1 sec for current event to finish,
            //  if it does not finish within this time abort the timer thread
            if (!StopAndWait(1000))
            {
                Abort();
            }
        }

        #endregion

        #region Private Methods

        private void NotificationTimer(ref long timerIntervalInMicroSec,
                               ref long ignoreEventIfLateBy,
                               ref bool stopTimer)
        {
            int timerCount = 0;
            long nextNotification = 0;

            MicroStopWatch microStopwatch = new MicroStopWatch();
            microStopwatch.Start();

            while (!stopTimer && !ApplicationManager.Instance.IsExit)
            {
                long callbackFunctionExecutionTime =
                    microStopwatch.ElapsedMicroseconds - nextNotification;

                long timerIntervalInMicroSecCurrent =
                    Interlocked.Read(ref timerIntervalInMicroSec);
                long ignoreEventIfLateByCurrent =
                    Interlocked.Read(ref ignoreEventIfLateBy);

                nextNotification += timerIntervalInMicroSecCurrent;
                timerCount++;
                long elapsedMicroseconds = 0;

                while ((elapsedMicroseconds = microStopwatch.ElapsedMicroseconds)
                        < nextNotification)
                {
                    Thread.SpinWait(10);
                }

                long timerLateBy = elapsedMicroseconds - nextNotification;
                if (timerLateBy >= ignoreEventIfLateByCurrent)
                {
                    continue;
                }

                MicroTimerEventArgs microTimerEventArgs = new MicroTimerEventArgs(
                    timerCount,
                    elapsedMicroseconds,
                    timerLateBy,
                    callbackFunctionExecutionTime);
                // Raise event.
                if (null != MicroTimerElapsed)
                {
                    MicroTimerElapsed.Call(this, microTimerEventArgs);
                }
            }

            microStopwatch.Stop();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start timer.
        /// </summary>
        public void Start()
        {
            if (Enabled || Interval <= 0)
            {
                return;
            }

            _stopTimer = false;

            ThreadStart threadStart = delegate()
            {
                NotificationTimer(ref _timerIntervalInMicroSec,
                                  ref _ignoreEventIfLateBy,
                                  ref _stopTimer);
            };

            _threadTimer = new Thread(threadStart);
            _threadTimer.Priority = ThreadPriority.Highest;
            _threadTimer.Name = "Micro Timer Thread";
            _threadTimer.Start();
        }
        /// <summary>
        /// Stop timer.
        /// </summary>
        public void Stop()
        {
            _stopTimer = true;
        }
        /// <summary>
        /// Stop and wait.
        /// </summary>
        public void StopAndWait()
        {
            StopAndWait(Timeout.Infinite);
        }
        /// <summary>
        /// Stop and wait.
        /// </summary>
        /// <param name="timeoutInMilliSec">The timeout in millisecond.</param>
        /// <returns>Returns true if successfully stop.</returns>
        public bool StopAndWait(int timeoutInMilliSec)
        {
            _stopTimer = true;

            if (!Enabled || 
                _threadTimer.ManagedThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                return true;
            }

            return _threadTimer.Join(timeoutInMilliSec);
        }
        /// <summary>
        /// Abort timer.
        /// </summary>
        public void Abort()
        {
            _stopTimer = true;

            if (Enabled)
            {
                try 
                {
                    _threadTimer.Abort(); 
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                }
                finally
                {
                    _threadTimer = null;
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Interval in microsecond.
        /// </summary>
        public long Interval
        {
            get
            {
                return Interlocked.Read(ref _timerIntervalInMicroSec);
            }
            set
            {
                Interlocked.Exchange(ref _timerIntervalInMicroSec, value);
            }
        }
        /// <summary>
        /// Gets or sets if event should be fired if current time is late by interval.
        /// </summary>
        public long IgnoreEventIfLateBy
        {
            get
            {
                return Interlocked.Read(ref _ignoreEventIfLateBy);
            }
            set
            {
                Interlocked.Exchange(
                    ref _ignoreEventIfLateBy, value <= 0 ? long.MaxValue : value);
            }
        }
        /// <summary>
        /// Gets or sets is timer enable or disable.
        /// </summary>
        public bool Enabled
        {
            set
            {
                if (value)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
            get
            {
                return (_threadTimer != null && _threadTimer.IsAlive);
            }
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The MicroTimerElapsed event.
        /// </summary>
        public event MicroTimerElapsedEventHandler MicroTimerElapsed;

        #endregion
    }

    #endregion
}
