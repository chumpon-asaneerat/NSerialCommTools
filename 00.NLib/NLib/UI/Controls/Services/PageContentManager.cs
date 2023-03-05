#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-10-07
=================
- Controls Library updated - Service for controls.
  - PageContentManager class update code in Current property.

======================================================================================================================
Update 2014-10-19
=================
- Controls Library updated - Service for controls.
  - PageContentManager class added (WPF).

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;

#endregion

using NLib;

namespace NLib.Services
{
    #region StatusMessage EventHandler and EventArgs

    /// <summary>
    /// Status Message Event Args
    /// </summary>
    public class StatusMessageEventArgs
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public StatusMessageEventArgs()
            : base()
        {
            this.Message = string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the status messsage.
        /// </summary>
        public string Message { get; set; }

        #endregion
    }
    /// <summary>
    /// Status Message Event Handler.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The EventArgs.</param>
    public delegate void StatusMessageEventHandler(object sender, StatusMessageEventArgs e);

    #endregion

    #region Page Content Manager

    /// <summary>
    /// Application GUI (Page) Content Manager class.
    /// </summary>
    public class PageContentManager
    {
        #region Singelton Access

        private static PageContentManager _instance = null;
        /// <summary>
        /// Singelton access instance.
        /// </summary>
        public static PageContentManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(PageContentManager))
                    {
                        _instance = new PageContentManager();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Internal Varaibles

        private Stack<ContentControl> _currents = new Stack<ContentControl>();

        private DateTime _lastStatusUpdate = DateTime.Now;

        private DispatcherTimer _timer = null;
        private int _timerInterval = 1; // timer in second.

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private PageContentManager() : base() { }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~PageContentManager()
        {
            try
            {
                if (null != _currents)
                    _currents.Clear();
                _currents = null;
            }
            catch { }
            finally
            {
                Shutdown();
            }
        }

        #endregion

        #region Public Methods

        #region Start/Shutdown

        /// <summary>
        /// Start service.
        /// </summary>
        public void Start()
        {
            InitTimer();
        }
        /// <summary>
        /// Shutdown service.
        /// </summary>
        public void Shutdown()
        {
            ReleaseTimer();
            Close();
        }

        #endregion

        #region Z-Order managements

        /// <summary>
        /// Step back one sub screen.
        /// </summary>
        public void Back()
        {
            if (null != _currents && _currents.Count > 0)
            {
                ContentControl last = _currents.Pop();
                if (null != last)
                {
                    Console.Write("pup last object out");
                }
                // Raise event
                ContentChanged.Call(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Close all sub screens.
        /// </summary>
        public void Close()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                if (_currents != null)
                {
                    lock (this)
                    {
                        _currents.Clear();
                    }

                    if (_currents.Count == 0)
                    {
                        // Raise event
                        ContentChanged.Call(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Err(med);
            }
        }

        #endregion

        #region Status Messages

        /// <summary>
        /// Update Status Message
        /// </summary>
        /// <param name="format">The status message that support format</param>
        /// <param name="args">The arguments for format.</param>
        public void UpdateStatusMessage(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            // set last status update time.
            _lastStatusUpdate = DateTime.Now;

            if (null != StatusUpdated)
            {
                // Raise event.
                StatusUpdated.Call(this, new StatusMessageEventArgs() { Message = msg });
            }
        }

        #endregion

        #region Timer methids

        private void InitTimer()
        {
            if (null != _timer)
                return;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(_timerInterval);
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }

        private void ReleaseTimer()
        {
            if (null != _timer)
            {
                _timer.Tick -= new EventHandler(_timer_Tick);
                _timer.Stop();
            }
            _timer = null;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (null != OnTick)
            {
                OnTick.Call(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Show/Hide Waiting Progress

        /// <summary>
        /// Show Waiting Progress.
        /// </summary>
        public void ShowWaitingProgress()
        {

        }
        /// <summary>
        /// Hide Waiting Progress.
        /// </summary>
        public void HideWaitingProgress()
        {

        }

        #endregion

        #endregion

        #region Public Properties

        #region For Page Z-Order managements

        /// <summary>
        /// Gets or sets current content control that need to display on main window's
        /// content area.
        /// </summary>
        public ContentControl Current
        {
            get
            {
                if (null != _currents && _currents.Count > 0)
                {
                    ContentControl ctrl = null;
                    lock (this)
                    {
                        ctrl = _currents.Peek();
                    }
                    return ctrl;
                }
                else return null;
            }
            set
            {
                if (null == value)
                    return;

                ContentControl last = null;
                if (null != _currents && _currents.Count > 0)
                {
                    lock (this)
                    {
                        last = _currents.Peek();
                    }
                }

                Type lastPageType = (null != last) ? last.GetType() : null;

                Type newPageType = value.GetType();

                if (lastPageType != newPageType)
                {
                    lock (this)
                    {
                        if (null == _currents)
                        {
                            _currents = new Stack<ContentControl>();
                        }
                        // keep to stack
                        _currents.Push(value);
                    }
                    // Raise event
                    ContentChanged.Call(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets number of content page is on Z order stack.
        /// </summary>
        public int Count
        {
            get
            {
                if (null == _currents)
                    return 0;
                else return _currents.Count;
            }
        }

        #endregion

        #region Status Message

        /// <summary>
        /// Gets is last Status message date and time.
        /// </summary>
        public DateTime LastStatusUpdate { get { return _lastStatusUpdate; } }

        #endregion

        #endregion

        #region Public Events

        /// <summary>
        /// ContentChanged event.
        /// </summary>
        public event System.EventHandler ContentChanged;
        /// <summary>
        /// StatusUpdated event.
        /// </summary>
        public event StatusMessageEventHandler StatusUpdated;
        /// <summary>
        /// OnTick Event.
        /// </summary>
        public event System.EventHandler OnTick;

        #endregion
    }

    #endregion

    #region Page Sub Content Manager

    /// <summary>
    /// Application GUI (Page) Sub Content Manager class.
    /// </summary>
    public class PageSubContentManager
    {
        #region Internal Varaibles

        private Stack<ContentControl> _currents = new Stack<ContentControl>();

        private DateTime _lastStatusUpdate = DateTime.Now;

        private DispatcherTimer _timer = null;
        private int _timerInterval = 1; // timer in second.

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public PageSubContentManager() : base() { }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~PageSubContentManager()
        {
            try
            {
                if (null != _currents)
                    _currents.Clear();
                _currents = null;
            }
            catch { }
            finally
            {
                Shutdown();
            }
        }

        #endregion

        #region Public Methods

        #region Start/Shutdown

        /// <summary>
        /// Start service.
        /// </summary>
        public void Start()
        {
            InitTimer();
        }
        /// <summary>
        /// Shutdown service.
        /// </summary>
        public void Shutdown()
        {
            ReleaseTimer();
            Close();
        }

        #endregion

        #region Z-Order managements

        /// <summary>
        /// Step back one sub screen.
        /// </summary>
        public void Back()
        {
            if (null != _currents && _currents.Count > 0)
            {
                ContentControl last = _currents.Pop();
                if (null != last)
                {
                    Console.Write("pup last object out");
                }
                // Raise event
                ContentChanged.Call(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Close all sub screens.
        /// </summary>
        public void Close()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                if (_currents != null)
                {
                    lock (this)
                    {
                        _currents.Clear();
                    }

                    if (_currents.Count == 0)
                    {
                        // Raise event
                        ContentChanged.Call(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Err(med);
            }
        }

        #endregion

        #region Status Messages

        /// <summary>
        /// Update Status Message
        /// </summary>
        /// <param name="format">The status message that support format</param>
        /// <param name="args">The arguments for format.</param>
        public void UpdateStatusMessage(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            // set last status update time.
            _lastStatusUpdate = DateTime.Now;

            if (null != StatusUpdated)
            {
                // Raise event.
                StatusUpdated.Call(this, new StatusMessageEventArgs() { Message = msg });
            }
        }

        #endregion

        #region Timer methids

        private void InitTimer()
        {
            if (null != _timer)
                return;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(_timerInterval);
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }

        private void ReleaseTimer()
        {
            if (null != _timer)
            {
                _timer.Tick -= new EventHandler(_timer_Tick);
                _timer.Stop();
            }
            _timer = null;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (null != OnTick)
            {
                OnTick.Call(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Show/Hide Waiting Progress

        /// <summary>
        /// Show Waiting Progress.
        /// </summary>
        public void ShowWaitingProgress()
        {

        }
        /// <summary>
        /// Hide Waiting Progress.
        /// </summary>
        public void HideWaitingProgress()
        {

        }

        #endregion

        #endregion

        #region Public Properties

        #region For Page Z-Order managements

        /// <summary>
        /// Gets or sets current content control that need to display on main window's
        /// content area.
        /// </summary>
        public ContentControl Current
        {
            get
            {
                if (null != _currents && _currents.Count > 0)
                {
                    ContentControl ctrl = null;
                    lock (this)
                    {
                        ctrl = _currents.Peek();
                    }
                    return ctrl;
                }
                else return null;
            }
            set
            {
                if (null == value)
                    return;

                ContentControl last = null;
                if (null != _currents && _currents.Count > 0)
                {
                    lock (this)
                    {
                        last = _currents.Peek();
                    }
                }

                Type lastPageType = (null != last) ? last.GetType() : null;

                Type newPageType = value.GetType();

                if (lastPageType != newPageType)
                {
                    lock (this)
                    {
                        if (null == _currents)
                        {
                            _currents = new Stack<ContentControl>();
                        }
                        // keep to stack
                        _currents.Push(value);
                    }
                    // Raise event
                    ContentChanged.Call(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets number of content page is on Z order stack.
        /// </summary>
        public int Count
        {
            get
            {
                if (null == _currents)
                    return 0;
                else return _currents.Count;
            }
        }

        #endregion

        #region Status Message

        /// <summary>
        /// Gets is last Status message date and time.
        /// </summary>
        public DateTime LastStatusUpdate { get { return _lastStatusUpdate; } }

        #endregion

        #endregion

        #region Public Events

        /// <summary>
        /// ContentChanged event.
        /// </summary>
        public event System.EventHandler ContentChanged;
        /// <summary>
        /// StatusUpdated event.
        /// </summary>
        public event StatusMessageEventHandler StatusUpdated;
        /// <summary>
        /// OnTick Event.
        /// </summary>
        public event System.EventHandler OnTick;

        #endregion
    }

    #endregion
}
