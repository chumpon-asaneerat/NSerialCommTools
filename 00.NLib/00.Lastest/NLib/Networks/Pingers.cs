#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-08-19
=================
- Netwrok framework.
  - Ping, PingManager and related classes is split to Pingers.cs.
  - All code need to re-testing.
  - Required to re-design code for check duplicate items, etc.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Net.NetworkInformation;
using NLib.Networks;

#endregion

namespace NLib.Networks
{
    #region Ping

    #region Event Handler and Event Args

    /// <summary>
    /// Ping Response Event Handler.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The Ping Response Event Args instance.</param>
    public delegate void PingResponseEventHandler(object sender, PingResponseEventArgs e);
    /// <summary>
    /// Ping Response Event Args.
    /// </summary>
    public class PingResponseEventArgs
    {
        #region Internal Variable

        private string _hostAddr;
        private PingReply _reply = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        private PingResponseEventArgs()
            : base()
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hostName">The host name.</param>
        /// <param name="reply">The reply result.</param>
        public PingResponseEventArgs(string hostName, PingReply reply)
            : this()
        {
            _hostAddr = hostName;
            _reply = reply;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~PingResponseEventArgs()
        {
            _reply = null;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get Host or IP.
        /// </summary>
        public string HostName
        {
            get { return _hostAddr; }
        }
        /// <summary>
        /// Get Reply result. Note. this property can be null if timeout is detected.
        /// </summary>
        public PingReply Reply
        {
            get { return _reply; }
        }

        #endregion
    }

    #endregion

    #region Ping Service

    /// <summary>
    /// Ping Service class. This class is service that handle multithread ping for multiple hosts
    /// at same time.
    /// </summary>
    public class PingService
    {
        #region Internal Class

        /// <summary>
        /// Pingger
        /// </summary>
        class Pingger
        {
            #region Internal Variable

            private string _hostAddr = string.Empty;
            private int _timeout = 5000;

            private AutoResetEvent resetEvent = new AutoResetEvent(false);

            private bool _isCancel = false;
            private bool _isError = false;
            private Exception _exception = null;
            private PingReply _reply = null;

            private bool _finished = false;

            private bool _isHandled = false;

            #endregion

            #region Private Method

            private void SendPing()
            {
                Ping pingger = new Ping();
                // reset flag.
                _isCancel = false;
                _isError = false;
                _exception = null;
                _reply = null;
                _finished = false;

                // Create an event handler for ping complete
                pingger.PingCompleted += new PingCompletedEventHandler(pingComplete);
                // Create a buffer of 16 bytes of data to be transmitted.
                byte[] buffer = Encoding.ASCII.GetBytes("x..............x");
                // Jump though 50 routing nodes tops, and don't fragment the packet
                PingOptions packetOptions = new PingOptions(50, true);
                // Send the ping asynchronously
                pingger.SendAsync(_hostAddr, _timeout, buffer, packetOptions, resetEvent);
            }

            private void pingComplete(object sender, PingCompletedEventArgs e)
            {
                // If the operation was canceled, display a message to the user.
                if (e.Cancelled)
                {
                    _finished = true; // mark finished flag.
                    _isCancel = true;
                    // The main thread can resume
                    ((AutoResetEvent)e.UserState).Set();
                }
                else if (e.Error != null)
                {
                    _finished = true; // mark finished flag.
                    _isError = true;
                    _exception = e.Error;
                    // The main thread can resume
                    ((AutoResetEvent)e.UserState).Set();
                }
                else
                {
                    _finished = true; // mark finished flag.
                    _reply = e.Reply;
                    // Call the method that displays the ping results, and pass the information with it
                    Update(_reply);
                }
            }

            private void Update(PingReply response)
            {
                _finished = true; // mark finished flag.
                if (response == null)
                {
                    // We got no response
                    return;
                }
                else if (response.Status == IPStatus.Success)
                {
                    // We got a response, let's see the statistics
                    /*
                    response.Address;
                    response.Buffer.Length;
                    response.RoundtripTime;
                    response.Options.Ttl;
                    */
                }
                else
                {
                    // The packet didn't get back as expected, explain why
                    // unsuccessful ping.
                    //response.Status;
                }
            }

            #endregion

            #region Public Method

            /// <summary>
            /// Ping
            /// </summary>
            /// <param name="hostName">Host to ping.</param>
            public void Ping(string hostName)
            {
                Ping(hostName, 5000);
            }
            /// <summary>
            /// Ping
            /// </summary>
            /// <param name="hostName">Host to ping.</param>
            /// <param name="timeout">Timeout in millisecond.</param>
            public void Ping(string hostName, int timeout)
            {
                _hostAddr = hostName;
                _timeout = timeout;
                SendPing();
            }

            #endregion

            #region Public Property

            /// <summary>
            /// Check Is Operation Finished.
            /// </summary>
            public bool Finished { get { return _finished; } }
            /// <summary>
            /// Check Is Operation Cancel.
            /// </summary>
            public bool Canceled { get { return _isCancel; } }
            /// <summary>
            /// Check Is Operation Has Error.
            /// </summary>
            public bool HasError { get { return _isError; } }
            /// <summary>
            /// Get Host or IP.
            /// </summary>
            public string HostName
            {
                get { return _hostAddr; }
            }
            /// <summary>
            /// Get Reply result.
            /// </summary>
            public PingReply Reply
            {
                get { return _reply; }
            }
            /// <summary>
            /// Get/Set Is handle. Note. when ping is created this property is set to false.
            /// when marked this property to true it cannot set to false again. (write once).
            /// </summary>
            public bool IsHandled
            {
                get { return _isHandled; }
                set
                {
                    if (_isHandled == true)
                        return; // already handle.
                    if (_isHandled != value)
                    {
                        _isHandled = value;
                    }
                }
            }

            #endregion
        }

        #endregion

        #region Internal Variable

        private Thread _thread = null;
        private List<string> _hosts = new List<string>();
        private bool _started = false;
        private bool _appExit = false;
        private int _interval = 30000;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public PingService()
        {
            Application.ThreadExit += new EventHandler(Application_ThreadExit);
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~PingService()
        {
            if (_hosts != null)
            {
                lock (_hosts)
                {
                    _hosts.Clear();
                }
            }
            Stop();
            Application.ThreadExit -= new EventHandler(Application_ThreadExit);
        }

        #endregion

        #region Thread Exit Handler

        void Application_ThreadExit(object sender, EventArgs e)
        {
            _appExit = true;
        }

        #endregion

        #region Thread Method

        #region Main Process

        private void Process()
        {
            DateTime chkTime = DateTime.Now;

            while (_started && !_appExit)
            {
                TimeSpan diffTime = DateTime.Now - chkTime;
                if (diffTime.TotalMilliseconds > _interval)
                {
                    // raise event.
                    RaisePingProcessStart();

                    string[] hostToPings = null;

                    lock (_hosts) // lock host list.
                    {
                        hostToPings = _hosts.ToArray();
                    }

                    if (hostToPings != null && hostToPings.Length > 0)
                    {
                        #region init pingers

                        int iFinCount = 0;
                        int iAllCount = hostToPings.Length;
                        Pingger[] pings = new Pingger[iAllCount];
                        for (int i = 0; i < iAllCount; i++)
                        {
                            pings[i] = new Pingger();
                            // ping host
                            pings[i].Ping(hostToPings[i]);
                        }

                        #endregion

                        #region Main Loop

                        DateTime dt = DateTime.Now;
                        bool _timeout = false;
                        while (iFinCount < iAllCount && _started && !_appExit)
                        {
                            TimeSpan ts = DateTime.Now - dt;
                            if (ts.TotalSeconds > 5)
                            {
                                // timeout in 5 second.
                                _timeout = true;
                                break;
                            }
                            // loop for check ping finished status.
                            for (int i = 0; i < iAllCount; i++)
                            {
                                if (pings[i].Finished && !pings[i].IsHandled)
                                {
                                    pings[i].IsHandled = true;
                                    // raise event
                                    RaiseEventReply(pings[i].HostName, pings[i].Reply);
                                    // increase counter
                                    ++iFinCount;
                                }
                            }
                            Application.DoEvents();
                            Thread.Sleep(20); // give cpu a little break.
                        }

                        #endregion

                        #region Handle Timeout

                        if (_timeout && _started && !_appExit)
                        {
                            for (int i = 0; i < iAllCount; i++)
                            {
                                if (!pings[i].Finished && !pings[i].IsHandled)
                                {
                                    // not finished and not handle.
                                    // raise event with null to make it's as error to caller user.
                                    RaiseEventReply(pings[i].HostName, null);
                                }
                            }
                        }

                        #endregion
                    }

                    // raise event.
                    RaisePingProcessFinished();

                    // update check time.
                    chkTime = DateTime.Now;
                }

                Application.DoEvents();
                Thread.Sleep(30); // give cpu a little break.
            }
        }

        #endregion

        #region Aboort Thread

        private void AboortThread()
        {
            if (_thread != null)
            {
                try
                {
                    _thread.Join(500); // join thread.
                }
                catch { }
                try
                {
                    _thread.Abort();
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                }
            }
            _thread = null;
        }

        #endregion

        #endregion

        #region Event Raiser

        #region Raise event

        /// <summary>
        /// Raise event.
        /// </summary>
        /// <param name="del">The delegate.</param>
        /// <param name="args">Tee delegate args.</param>
        private void RaiseEvent(Delegate del, params object[] args)
        {
            if (del == null)
                return;
            ApplicationManager.Instance.Invoke(del, args);
        }

        #endregion

        private void RaiseEventReply(string hostName, PingReply reply)
        {
            if (OnReply != null)
            {
                PingResponseEventArgs evt = new PingResponseEventArgs(hostName, reply);
                RaiseEvent(OnReply, this, evt);
            }
        }

        private void RaiseHostListChanged()
        {
            if (HostListChanged != null)
            {
                RaiseEvent(HostListChanged, this, System.EventArgs.Empty);
            }
        }

        private void RaisePingProcessStart()
        {
            if (PingProcessStart != null)
            {
                RaiseEvent(PingProcessStart, this, System.EventArgs.Empty);
            }
        }

        private void RaisePingProcessFinished()
        {
            if (PingProcessFinished != null)
            {
                RaiseEvent(PingProcessFinished, this, System.EventArgs.Empty);
            }
        }

        #endregion

        #region Public Method

        #region Start/Stop

        /// <summary>
        /// Start service.
        /// </summary>
        public void Start()
        {
            Start(true);
        }
        /// <summary>
        /// Start service.
        /// </summary>
        /// <param name="autoRestart">Auto restart if already started.</param>
        public void Start(bool autoRestart)
        {
            if (autoRestart)
            {
                _started = false;
                AboortThread();
            }
            if (_started || _thread != null)
                return; // thread is already start.

            _started = true;

            ThreadStart ts = new ThreadStart(this.Process);
            _thread = new Thread(ts);
            _thread.Name = "Ping thread";
            _thread.Priority = ThreadPriority.BelowNormal;
            _thread.IsBackground = true;
            _thread.Start(); // start thread.
        }
        /// <summary>
        /// Stop service.
        /// </summary>
        public void Stop()
        {
            _started = false;
            AboortThread();
            _thread = null;
        }

        #endregion

        #region Add/Remove/Clear

        private bool _lock = false;
        /// <summary>
        /// Add Host. Note. that host name will convert internally to lowercase. So
        /// when event is raise the hostname information is the lowercase too.
        /// </summary>
        /// <param name="hostName">The host name to add in ping list.</param>
        public void Add(string hostName)
        {
            if (hostName.Trim() == string.Empty)
            {
                return;
            }
            if (_hosts != null && !_hosts.Contains(hostName.Trim().ToLower()))
            {
                lock (_hosts)
                {
                    _hosts.Add(hostName.Trim().ToLower());
                }
                if (!_lock)
                {
                    RaiseHostListChanged();
                }
            }
        }
        /// <summary>
        /// Add Host name's array. Note. that host name will convert internally to lowercase. So
        /// when event is raise the hostname information is the lowercase too.
        /// </summary>
        /// <param name="hostNames">The Host Name arrays</param>
        public void AddRange(string[] hostNames)
        {
            if (hostNames == null || hostNames.Length <= 0)
                return;
            _lock = true;
            foreach (string hostName in hostNames)
            {
                Add(hostName); // call add indirect.
            }
            _lock = false;
            RaiseHostListChanged();
        }
        /// <summary>
        /// Remove Host
        /// </summary>
        /// <param name="hostName">The host name to remove.</param>
        public void Remove(string hostName)
        {
            if (hostName.Trim() == string.Empty)
            {
                return;
            }
            if (_hosts != null && _hosts.Contains(hostName.Trim().ToLower()))
            {
                lock (_hosts)
                {
                    _hosts.Remove(hostName.Trim().ToLower());
                }
                RaiseHostListChanged();
            }
        }
        /// <summary>
        /// Clear all hosts
        /// </summary>
        public void Clear()
        {
            if (_hosts != null)
            {
                lock (_hosts)
                {
                    _hosts.Clear();
                }
                RaiseHostListChanged();
            }
        }

        #endregion

        #region Get Host List

        /// <summary>
        /// Get Host List
        /// </summary>
        /// <returns>Return array of host name in ping list.</returns>
        public string[] GetHosts()
        {
            string[] results = null;
            if (_hosts != null)
            {
                lock (_hosts)
                {
                    results = _hosts.ToArray();
                }
            }
            return results;
        }

        #endregion

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Ping Interval in millisecond. min value 500 ms. default 30000 ms.
        /// </summary>
        [Category("Ping")]
        [Description("Get/Set Ping Interval in millisecond. min value 500 ms. default 30000 ms.")]
        public int Interval
        {
            get { return _interval; }
            set
            {
                if (_started)
                    return;
                _interval = value;
                if (_interval < 500)
                {
                    _interval = 500;
                }
            }
        }

        #endregion

        #region Public Event

        /// <summary>
        /// OnReply Event. occur when ping is replied or timeout after ping is called.
        /// </summary>
        [Category("Ping")]
        [Description("OnReply Event. occur when ping is replied or timeout after ping is called.")]
        public event PingResponseEventHandler OnReply;
        /// <summary>
        /// HostListChanged Event. Occur when host list is add/remove or clear.
        /// </summary>
        [Category("Ping")]
        [Description("HostListChanged Event. Occur when host list is add/remove or clear.")]
        public event System.EventHandler HostListChanged;
        /// <summary>
        /// PingProcessStart Event. Occur when ping interval is reach and process is start.
        /// </summary>
        [Category("Ping")]
        [Description("PingProcessStart Event. Occur when ping process is start.")]
        public event System.EventHandler PingProcessStart;
        /// <summary>
        /// PingProcessFinished Event. Occur when ping process is finished.
        /// </summary>
        [Category("Ping")]
        [Description("PingProcessFinished Event. Occur when ping process is finished.")]
        public event System.EventHandler PingProcessFinished;

        #endregion
    }

    #endregion

    #endregion
}

namespace NLib.Components
{
    #region Ping Manager

    /// <summary>
    /// Ping Manager component. This component is wrapper component that provide
    /// ping (in multithread) service.
    /// </summary>
    public class PingManager : Component
    {
        #region Internal Variable

        private PingService _ping;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public PingManager()
            : base()
        {
            _ping = new PingService();
            if (_ping != null)
            {
                _ping.HostListChanged += new EventHandler(_ping_HostListChanged);
                _ping.OnReply += new PingResponseEventHandler(_ping_OnReply);
                _ping.PingProcessStart += new EventHandler(_ping_PingProcessStart);
                _ping.PingProcessFinished += new EventHandler(_ping_PingProcessFinished);
            }
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~PingManager()
        {
            if (_ping != null)
            {
                _ping.HostListChanged -= new EventHandler(_ping_HostListChanged);
                _ping.OnReply -= new PingResponseEventHandler(_ping_OnReply);
                _ping.PingProcessStart -= new EventHandler(_ping_PingProcessStart);
                _ping.PingProcessFinished -= new EventHandler(_ping_PingProcessFinished);
            }
            Stop();
            _ping = null;
        }

        #endregion

        #region Private Method

        #region Ping Event Handlers

        void _ping_OnReply(object sender, PingResponseEventArgs e)
        {
            RaiseEventReply(e);
        }

        void _ping_HostListChanged(object sender, EventArgs e)
        {
            RaiseHostListChanged();
        }

        void _ping_PingProcessStart(object sender, EventArgs e)
        {
            RaisePingProcessStart();
        }

        void _ping_PingProcessFinished(object sender, EventArgs e)
        {
            RaisePingProcessFinished();
        }

        #endregion

        #region Event Raiser

        /// <summary>
        /// Raise event.
        /// </summary>
        /// <param name="del">The delegate.</param>
        /// <param name="args">Tee delegate args.</param>
        private void RaiseEvent(Delegate del, params object[] args)
        {
            if (del == null)
                return;
            ApplicationManager.Instance.Invoke(del, args);
        }

        private void RaiseEventReply(PingResponseEventArgs evt)
        {
            if (OnReply != null)
            {
                RaiseEvent(OnReply, this, evt);
            }
        }

        private void RaiseHostListChanged()
        {
            if (HostListChanged != null)
            {
                RaiseEvent(HostListChanged, this, System.EventArgs.Empty);
            }
        }

        private void RaisePingProcessStart()
        {
            if (PingProcessStart != null)
            {
                RaiseEvent(PingProcessStart, this, System.EventArgs.Empty);
            }
        }

        private void RaisePingProcessFinished()
        {
            if (PingProcessFinished != null)
            {
                RaiseEvent(PingProcessFinished, this, System.EventArgs.Empty);
            }
        }

        #endregion

        #endregion

        #region Public Method

        #region Start/Stop

        /// <summary>
        /// Start service.
        /// </summary>
        public void Start()
        {
            if (_ping != null) _ping.Start();
        }
        /// <summary>
        /// Start service.
        /// </summary>
        /// <param name="autoRestart">Auto restart if already started.</param>
        public void Start(bool autoRestart)
        {
            if (_ping != null) _ping.Start(autoRestart);
        }
        /// <summary>
        /// Stop service.
        /// </summary>
        public void Stop()
        {
            if (_ping != null) _ping.Stop();
        }

        #endregion

        #region Add/Remove/Clear

        /// <summary>
        /// Add Host. Note. that host name will convert internally to lowercase. So
        /// when event is raise the hostname information is the lowercase too.
        /// </summary>
        /// <param name="hostName">The host name to add in ping list.</param>
        public void Add(string hostName)
        {
            if (_ping != null)
            {
                _ping.Add(hostName);
            }
        }
        /// <summary>
        /// Add Host name's array. Note. that host name will convert internally to lowercase. So
        /// when event is raise the hostname information is the lowercase too.
        /// </summary>
        /// <param name="hostNames">The Host Name arrays</param>
        public void AddRange(string[] hostNames)
        {
            if (_ping != null)
            {
                _ping.AddRange(hostNames);
            }
        }
        /// <summary>
        /// Remove Host
        /// </summary>
        /// <param name="hostName">The host name to remove.</param>
        public void Remove(string hostName)
        {
            if (_ping != null)
            {
                _ping.Remove(hostName);
            }
        }
        /// <summary>
        /// Clear all hosts
        /// </summary>
        public void Clear()
        {
            if (_ping != null)
            {
                _ping.Clear();
            }
        }

        #endregion

        #region Get Host List

        /// <summary>
        /// Get Host List
        /// </summary>
        /// <returns>Return array of host name in ping list.</returns>
        public string[] GetHosts()
        {
            if (_ping != null)
            {
                return _ping.GetHosts();
            }
            else return null;
        }

        #endregion

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Ping Interval in millisecond. min value 500 ms. default 30000 ms.
        /// </summary>
        [Category("Ping")]
        [Description("Get/Set Ping Interval in millisecond. min value 500 ms. default 30000 ms.")]
        public int Interval
        {
            get
            {
                if (_ping != null)
                    return _ping.Interval;
                else return -1;
            }
            set
            {
                if (_ping == null)
                    return;
                _ping.Interval = value;
            }
        }

        #endregion

        #region Public Events

        /// <summary>
        /// OnReply Event. occur when ping is replied or timeout after ping is called.
        /// </summary>
        [Category("Ping")]
        [Description("OnReply Event. occur when ping is replied or timeout after ping is called.")]
        public event PingResponseEventHandler OnReply;
        /// <summary>
        /// HostListChanged Event. Occur when host list is add/remove or clear.
        /// </summary>
        [Category("Ping")]
        [Description("HostListChanged Event. Occur when host list is add/remove or clear.")]
        public event System.EventHandler HostListChanged;
        /// <summary>
        /// PingProcessStart Event. Occur when ping interval is reach and process is start.
        /// </summary>
        [Category("Ping")]
        [Description("PingProcessStart Event. Occur when ping process is start.")]
        public event System.EventHandler PingProcessStart;
        /// <summary>
        /// PingProcessFinished Event. Occur when ping process is finished.
        /// </summary>
        [Category("Ping")]
        [Description("PingProcessFinished Event. Occur when ping process is finished.")]
        public event System.EventHandler PingProcessFinished;

        #endregion
    }

    #endregion
}
