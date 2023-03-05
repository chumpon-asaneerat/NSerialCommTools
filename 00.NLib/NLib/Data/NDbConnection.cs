#region Using

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;

#endregion

#region Extra Using

using NLib;
using NLib.Data;

#endregion

namespace NLib.Data
{
    #region Exception Class

    /// <summary>
    /// Data Access Exception Class.
    /// </summary>
    public class DataAccessException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public DataAccessException(string message) : base(message) { }
    }

    #endregion

    #region Exception EventHandler and EventArgs

    /// <summary>
    /// Execute Exception Event Handler.
    /// </summary>
    /// <param name="sender">Sender instance.</param>
    /// <param name="e">Exception instance.</param>
    public delegate void ExecuteExceptionEventHandler(object sender, ExecuteExceptionEventArgs e);
    /// <summary>
    /// Execute Exception EventArgs.
    /// </summary>
    public class ExecuteExceptionEventArgs
    {
        #region Internal Variable

        private Exception _ex = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private ExecuteExceptionEventArgs() { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ex">Exception instance.</param>
        public ExecuteExceptionEventArgs(Exception ex) { _ex = ex; }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~ExecuteExceptionEventArgs()
        {
            _ex = null;
            // Force free memory
            NGC.FreeGC(this);
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets Exception instance.
        /// </summary>
        public Exception Exception { get { return _ex; } }

        #endregion
    }

    #endregion

    #region Connection Ref

    /// <summary>
    /// The Connection Ref class.
    /// </summary>
    internal class ConnectionRef
    {
        #region Internal Variable

        private string _uniqueName = string.Empty;
        private System.Data.Common.DbConnection _connection = null;
        private int _referenceCount = 0;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private ConnectionRef() { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="uniqueName">Unique Connection Name.</param>
        /// <param name="connection">Connection Instance.</param>
        public ConnectionRef(string uniqueName, System.Data.Common.DbConnection connection)
        {
            _uniqueName = uniqueName.ToUpper().Trim();
            _connection = connection;
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~ConnectionRef()
        {
            Free();
            // Force free memory.
            NGC.FreeGC(this);
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Add Reference.
        /// </summary>
        public void AddRef()
        {
            lock (this)
            {
                ++_referenceCount;
            }
        }
        /// <summary>
        /// Release Reference.
        /// </summary>
        public void ReleaseRef()
        {
            lock (this)
            {
                --_referenceCount;
            }
        }
        /// <summary>
        /// Free and dispose.
        /// </summary>
        public void Free()
        {
            lock (this)
            {
                if (null != _connection)
                {
                    MethodBase med = MethodBase.GetCurrentMethod();
                    try
                    {
                        if (_connection.State != ConnectionState.Closed)
                        {
                            _connection.Close();
                        }
                    }
                    catch (Exception ex1)
                    {
                        //Console.WriteLine(ex1.ToString());
                        ex1.Err(med);
                    }
                    try { _connection.Dispose(); }
                    catch (Exception ex2)
                    {
                        //Console.WriteLine(ex2.ToString());
                        ex2.Err(med);
                    }
                }
                // Force free memory
                NGC.FreeGC(_connection);

                _connection = null;

                _referenceCount = 0;
            }
            // Free Memory
            NGC.FreeGC();
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get Unique Name
        /// </summary>
        public string UniqueName
        {
            get { return _uniqueName; }
        }
        /// <summary>
        /// Get Connection Instance
        /// </summary>
        public System.Data.Common.DbConnection DbConnection
        {
            get { return _connection; }
        }
        /// <summary>
        /// Get Reference Count
        /// </summary>
        public int ReferenceCount
        {
            get { return _referenceCount; }
        }

        #endregion
    }

    #endregion

    #region Connection Pools

    /// <summary>
    /// The Connection Pools. Provide cache machanishm to share connections.
    /// </summary>
    internal sealed class ConnectionPools
    {
        #region Internal Variable

        private uint _connectionCounter = 0;
        Dictionary<string, ConnectionRef> _connetcions = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        private ConnectionPools()
            : base()
        {
            _connetcions = new Dictionary<string, ConnectionRef>();
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~ConnectionPools()
        {
            FreeAllConnections();

            // Force free memory
            NGC.FreeGC(_connetcions);

            _connetcions = null;

            // Force free memory
            NGC.FreeGC(this);
        }

        #endregion

        #region Singelton

        private static ConnectionPools _instance = null;
        /// <summary>
        /// Singelton Access
        /// </summary>
        public static ConnectionPools Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(ConnectionPools))
                    {
                        _instance = new ConnectionPools();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Private Method

        private void FreeAllConnections()
        {
            lock (this)
            {
                if (null != _connetcions && _connetcions.Count > 0)
                {
                    foreach (ConnectionRef connRef in _connetcions.Values)
                    {
                        try
                        {
                            connRef.Free();
                        }
                        catch { }
                    }
                    _connetcions.Clear();
                }
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Is Exists.
        /// </summary>
        /// <param name="connectionUniqueName">Unique Name to check.</param>
        /// <returns>Returns true if name is already exists.</returns>        
        public bool IsExists(string connectionUniqueName)
        {
            if (null == _connetcions)
                return false;
            return _connetcions.ContainsKey(connectionUniqueName.ToUpper().Trim());
        }
        /// <summary>
        /// Indexer access.
        /// </summary>
        /// <param name="connectionUniqueName">Unique Name to check.</param>
        /// <returns>Returns ConnectionRef instance that match Unique Name.</returns>
        public ConnectionRef this[string connectionUniqueName]
        {
            get
            {
                string key = connectionUniqueName.ToUpper().Trim();
                if (!IsExists(key))
                    return null;
                else return _connetcions[key];
            }
        }
        /// <summary>
        /// Add New ConnectionRef.
        /// </summary>
        /// <param name="connectionUniqueName">Unique Name to check.</param>
        /// <param name="connection">DbConnection instance.</param>
        /// <returns>Returns ConnectionRef instance that match Unique Name.</returns>
        public ConnectionRef Add(string connectionUniqueName, DbConnection connection)
        {
            lock (this)
            {
                string key = connectionUniqueName.ToUpper().Trim();
                if (IsExists(key))
                {
                    // Already exists
                    return null;
                }
                else
                {
                    // Not exists - OK
                    ConnectionRef connRef = new ConnectionRef(connectionUniqueName, connection);
                    // add to collection
                    if (null != connRef)
                    {
                        _connetcions.Add(connRef.UniqueName, connRef);
                    }

                    return connRef;
                }
            }
        }
        /// <summary>
        /// Remove connection reference.
        /// </summary>
        /// <param name="connectionUniqueName">Unique Name to check.</param>
        public void Remove(string connectionUniqueName)
        {
            lock (this)
            {
                string key = connectionUniqueName.ToUpper().Trim();
                if (IsExists(key))
                {
                    ConnectionRef connRef = this[connectionUniqueName];
                    if (null != connRef)
                    {
                        connRef.ReleaseRef();
                    }
                    if (null != connRef && connRef.ReferenceCount <= 0)
                    {
                        _connetcions.Remove(key);
                        if (null != connRef) 
                            connRef.Free();
                    }
                    connRef = null;
                }
            }
        }
        /// <summary>
        /// Get next Unique id.
        /// </summary>
        /// <returns>Returns Next unique id.</returns>
        public uint GetNextUId()
        {
            return ++_connectionCounter;
        }

        #endregion
    }

    #endregion
}

namespace NLib.Components
{
    #region NDbConnection - Common properties
    
    // Common properties implements.
    partial class NDbConnection
    {
        #region Helper Utils class

        /// <summary>
        /// Get String Utils
        /// </summary>
        protected sealed class Utils : NLib.Utils.StringUtils { }

        #endregion

        #region Internal Variables

        private bool _enableDetailDebug = false;
        private bool __onExecuting = false;
        /// <summary>
        /// Gets or sets On Executing flag.
        /// </summary>
        protected bool _onExecuting
        {
            get { return __onExecuting;  }
            set
            {
                if (__onExecuting != value)
                {
                    __onExecuting = value;
                    if (_enableDetailDebug)
                    {
                        MethodBase med = MethodBase.GetCurrentMethod();
                        string msg = "Executing Flag is change to " + __onExecuting.ToString();
                        msg.Info(med);
                    }
                }
            }
        }

        private bool _autoChangedCursor = false; // for handle cursor when execute

        #endregion

        #region Private/Protected methods

        #region Cursors

        /// <summary>
        /// For WPF to find active windows.
        /// </summary>
        /// <returns>Returns WPF active window instance.</returns>
        private System.Windows.Window CheckActiveWindow()
        {
            System.Windows.Window window = null;
            try
            {
                window = window = System.Windows.Application.Current.Windows
                    .OfType<System.Windows.Window>().FirstOrDefault(x => x.IsActive);
            }
            catch /* (Exception ex)*/ 
            { 
                // No windows active or has multiple active windows!!!!
                window = null;
            }
            return window;
        }
        /// <summary>
        /// Change Active Form Cursor.
        /// </summary>
        /// <param name="cursor">The cursor instance.</param>
        protected void ChangeActiveFormCursor(System.Windows.Forms.Cursor cursor)
        {
            if (!_autoChangedCursor)
                return; // not auto changed cursor by internal code.

            if (null == cursor ||
                null == System.Windows.Forms.Form.ActiveForm ||
                System.Windows.Forms.Form.ActiveForm.Disposing ||
                System.Windows.Forms.Form.ActiveForm.IsDisposed)
            {
                return;
            }
            Action del = () =>
            {
                try
                {
                    if (null == System.Windows.Forms.Form.ActiveForm ||
                        System.Windows.Forms.Form.ActiveForm.Disposing ||
                        System.Windows.Forms.Form.ActiveForm.IsDisposed)
                    {
                        return;
                    }
                    System.Windows.Forms.Form.ActiveForm.Cursor = cursor;
                }
                catch { }
            };
            try
            {
                // call delegate code.
                del.Invoke();
            }
            catch { }
        }
        /// <summary>
        /// Change Active Window Cursor.
        /// </summary>
        /// <param name="cursor">The cursor instance.</param>
        protected void ChangeActiveWindowCursor(System.Windows.Input.Cursor cursor)
        {
            if (!_autoChangedCursor)
                return; // not auto changed cursor by internal code.

            System.Windows.Window window = CheckActiveWindow();

            if (null == cursor || null == window)
            {
                return;
            }
            Action del = () =>
                {
                    try
                    {
                        if (null == window)
                        {
                            return;
                        }
                        window.Cursor = cursor;
                    }
                    catch { }
                };
            try
            {
                // call delegate code.
                del.Invoke();
            }
            catch { }
        }
        /// <summary>
        /// Show Busy cursor
        /// </summary>
        protected void ShowBusyCursor()
        {
            if (null == System.Windows.Forms.Form.ActiveForm)
                ChangeActiveWindowCursor(System.Windows.Input.Cursors.AppStarting);
            else ChangeActiveFormCursor(System.Windows.Forms.Cursors.AppStarting);
        }
        /// <summary>
        /// Show Normal cursor.
        /// </summary>
        protected void ShowNormalCursor()
        {
            if (null == System.Windows.Forms.Form.ActiveForm)
                ChangeActiveWindowCursor(System.Windows.Input.Cursors.Arrow);
            else ChangeActiveFormCursor(System.Windows.Forms.Cursors.Default);
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets to show detail debug. Default is false.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets to show detail debug. Default is false.")]
        [Browsable(true)]
        [XmlAttribute]
        public bool EnableDetailDebug
        {
            get { return _enableDetailDebug;  }
            set
            {
                if (_enableDetailDebug != value)
                {
                    _enableDetailDebug = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets auto change cursor when executing database command.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets auto change cursor when executing database command.")]
        [Browsable(true)]
        [XmlAttribute]
        public bool AutoChangeCursor
        {
            get { return _autoChangedCursor; }
            set 
            {
                if (_autoChangedCursor != value)
                {
                    _autoChangedCursor = value;
                }
            }
        }

        #endregion
    }

    #endregion

    #region NDbConnection - Connect/Disconnect

    // Connect/Disconnect implements.
    partial class NDbConnection
    {
        #region Internal Variables

        private NDbConfig _config = null;

        private uint _connectionUid = 0;
        private bool _shareConnection = true;

        private bool _connecting = false; // connecting flag

        private System.Threading.Timer _timer;

        private DateTime _lastCheckTime = DateTime.Now;
        private DateTime _lastBrokenTime = DateTime.Now;

        #endregion

        #region Private Methods

        #region Timer methods

        #region Init/Release timer

        private void InitTimer()
        {
            if (null != _timer)
            {
                ReleaseTimer();
            }

            if (null == Config || !Config.Optional.AutoDetectBroken)
                return; // skip if not config assigned or not set as auto detected.

            // create timer with wait 250 ms and not start until call Timer.Change method
            _timer = new System.Threading.Timer(
                new System.Threading.TimerCallback(this.Tick),
                null, System.Threading.Timeout.Infinite, 250);
            if (null != _timer)
            {
                _timer.Change(0, 250); // start timer
            }
        }

        private void ReleaseTimer()
        {
            if (null != _timer)
            {
                // Dispose timer
                try { _timer.Dispose(); }
                catch { }
            }
            _timer = null;
        }

        #endregion

        #region Timer function

        private void Tick(object state)
        {
            if (null == this.Config || !this.Config.Optional.AutoDetectBroken)
                return;

            if (this.IsConnecting)
            {
                return; // ignore if on connecting.
            }

            TimeSpan ts = DateTime.Now - _lastCheckTime;
            if (ts.TotalSeconds >= this.Config.Optional.AutoDetectInterval)
            {
                MethodBase med = MethodBase.GetCurrentMethod();

                _lastCheckTime = DateTime.Now;

                if (this._onExecuting)
                {
                    #region Write debug

                    if (this.EnableDetailDebug)
                    {
                        "Detected some executing method. assume this should be OK.".Info(med);
                    }

                    #endregion
                    
                    DoAlive();
                    return; // Some recordset is on processing.
                }

                if (IsConnectionInUsed())
                {
                    #region Write debug

                    if (this.EnableDetailDebug)
                    {
                        "Detected some Reader seem to be still in used. assume this should be OK.".Info(med);
                    }

                    #endregion
                    
                    DoAlive();
                    return; // Some recordset is on processing.
                }

                string query = string.Empty;
                if (null != this.Config.Factory &&
                    null != this.Config.Factory.Formatter)
                {
                    query = this.Config.Factory.Formatter.GetTestQuery();
                }

                if (string.IsNullOrWhiteSpace(query))
                    return;
                if (!IsConnected)
                    return; // already disconnected.

                bool success = RunTestQuery(query);
                if (!success)
                {
                    // broken
                    _lastBrokenTime = DateTime.Now;
                    DoBroken();
                    Disconnect(); // call disconnect
                }
                else DoAlive();
            }
        }

        #endregion

        #endregion

        #region Config methods

        void _config_ConfigChanged(object sender, EventArgs e)
        {
            // Auto disconnect if connection information is changed.
            Disconnect();
        }

        private void HookConfigEvents()
        {
            if (null != _config)
            {
                // Add event
                _config.ConfigChanged += new EventHandler(_config_ConfigChanged);
            }
        }

        private void UnhookConfigEvents()
        {
            if (null != _config)
            {
                // Remove event
                _config.ConfigChanged -= new EventHandler(_config_ConfigChanged);
            }
        }

        #endregion

        #region RaiseEvent
        
        /// <summary>
        /// Raise Event with automatically synchronized thread if required.
        /// </summary>
        /// <param name="del">delegate reference or event instance</param>
        /// <param name="args">argument to send as parameter</param>
        protected void RaiseEvent(Delegate del, params object[] args)
        {
            if (null == del)
                return;
            ApplicationManager.Instance.Invoke(del, args);
        }

        #endregion

        #region Connect/Disconnect methods

        #region GetUniqueName

        private string GetUniqueName()
        {
            if (null == _config)
                return string.Empty;
            string uniqueName = _config.UniqueName.Trim();
            if (_connectionUid == 0 && !_shareConnection)
            {
                // read next uid
                _connectionUid = ConnectionPools.Instance.GetNextUId();
            }
            if (_shareConnection)
            {
                uniqueName += "_" + _connectionUid.ToString();
            }

            return uniqueName;
        }

        #endregion

        #region ConnectToDatabase

        /// <summary>
        /// Split connection code to method to make it easy to call it Asynchronous
        /// </summary>
        private void ConnectToDatabase()
        {
            if (IsConnected)
                return; // Already connected.

            MethodBase med = MethodBase.GetCurrentMethod();
            if (_connecting)
            {
                "Ignore because call connect when application still in connecting process.".Info(med);
                return; // in connecting process
            }
            DoConnecting(); // Raise event
            if (null == _config)
            {
                DoConnectFailed(); // Raise event
                return;
            }
            string uniqueName = GetUniqueName();
            if (string.IsNullOrWhiteSpace(uniqueName))
            {
                DoConnectFailed(); // Raise event
                return;
            }
            else
            {
                lock (this)
                {
                    #region Check and Initialize

                    DbConnection conn = null;
                    ConnectionRef connRef = null;

                    if (!ConnectionPools.Instance.IsExists(uniqueName))
                    {
                        #region Create DbConnection and Check is connection can create

                        conn = _config.Factory.CreateConnection();
                        if (null == conn)
                        {
                            "ConnectToDatabase : Cannot create connection instance.".Info(med);
                            DoConnectFailed(); // Raise event
                            return;
                        }

                        #endregion

                        connRef = ConnectionPools.Instance.Add(uniqueName, conn);
                    }
                    else
                    {
                        connRef = ConnectionPools.Instance[uniqueName];
                    }

                    // This step connection reference should exists.
                    if (null == connRef)
                    {
                        "ConnectToDatabase : Cannot get connection reference.".Info(med);
                        DoConnectFailed(); // Raise event
                        return;
                    }
                    connRef.AddRef(); // Increase reference

                    #endregion

                    #region Connecting

                    if (connRef.DbConnection.State != ConnectionState.Open)
                    {
                        string connectionString = string.Empty;
                        try
                        {
                            connectionString = _config.ConnectionString;
                            connRef.DbConnection.ConnectionString = connectionString;
                            connRef.DbConnection.Open(); // Open connection
                        }
                        catch (Exception ex)
                        {
                            connRef.ReleaseRef(); // Release reference

                            #region Write Error

                            string msg = "Failed to connect." + Environment.NewLine +
                                "[Connection String] : " + connectionString;
                            med.Err(ex, msg);
                            RaiseErr(ex);

                            #endregion

                            DoConnectFailed(); // Raise event
                        }
                        finally
                        {
                        }
                    }

                    #endregion

                    #region Result

                    // Cannot used IsConnected Here because connecting variable still not changed.
                    if (null != this.DbConnection &&
                        this.DbConnection.State == ConnectionState.Open)
                    {
                        // connect success call config DoConnected to init format or all
                        // other require scripts that provide by ConnectionConfig.Factory.
                        if (null != this.Factory)
                        {
                            this.Factory.DoConnected(this.DbConnection, null);
                        }
                        DoConnected(); // Raise event
                    }
                    else DoConnectFailed(); // Raise event                    

                    #endregion
                }
            }
        }

        #endregion

        #region DoXXX methods
        
        /// <summary>
        /// DoConnecting
        /// </summary>
        private void DoConnecting()
        {
            _connecting = true;

            OnConnecting();
            if (null == this.Connecting)
                return;
            RaiseEvent(this.Connecting, new object[] { this, System.EventArgs.Empty });
        }
        /// <summary>
        /// DoConnected
        /// </summary>
        private void DoConnected()
        {
            _connecting = false;
            OnConnected();

            InitTimer(); // init timer when connected

            if (null == this.Connected)
                return;
            RaiseEvent(this.Connected, new object[] { this, System.EventArgs.Empty });
        }
        /// <summary>
        /// DoConnectFailed
        /// </summary>
        private void DoConnectFailed()
        {
            _connecting = false;

            OnConnectFailed();
            if (null == this.ConnectFailed)
                return;
            RaiseEvent(this.ConnectFailed, new object[] { this, System.EventArgs.Empty });
        }
        /// <summary>
        /// DoDisonnecting
        /// </summary>
        private void DoDisonnecting()
        {
            _connecting = false;

            OnDisconnecting();
            if (null == this.Disconnecting)
                return;
            RaiseEvent(this.Disconnecting, new object[] { this, System.EventArgs.Empty });
        }
        /// <summary>
        /// DoDisconnected
        /// </summary>
        private void DoDisconnected()
        {
            _connecting = false;

            ReleaseTimer(); // release timer when disconnected

            OnDisconnected();
            
            // Clear internal variables.
            ClearRecordsets();
            ClearProviderTypes();
            
            if (null == this.Disconnected)
                return;

            RaiseEvent(this.Disconnected, new object[] { this, System.EventArgs.Empty });
        }
        /// <summary>
        /// DoBroken
        /// </summary>
        private void DoBroken()
        {
            if (null == this.ConnectionBroken)
                return;
            RaiseEvent(this.ConnectionBroken, new object[] { this, System.EventArgs.Empty });
        }
        /// <summary>
        /// DoAlive
        /// </summary>
        private void DoAlive()
        {
            if (null == this.ConnectionAlive)
                return;
            RaiseEvent(this.ConnectionAlive, new object[] { this, System.EventArgs.Empty });
        }
        /// <summary>
        /// Raise OnException event.
        /// </summary>
        /// <param name="msg">The error message.</param>
        protected void RaiseErr(string msg)
        {
            if (null != OnException)
            {
                RaiseErr(new DataAccessException(msg));
            }
        }
        /// <summary>
        /// Raise OnException event.
        /// </summary>
        /// <param name="ex">The exception.</param>
        protected void RaiseErr(Exception ex)
        {
            if (null != OnException)
            {
                ExecuteExceptionEventArgs evt = new ExecuteExceptionEventArgs(ex);
                RaiseEvent(OnException, new object[] { this, evt });
            }
        }

        #endregion

        #endregion

        #endregion

        #region Protected Methods

        #region Factory

        /// <summary>
        /// Access Connection Factory.
        /// </summary>
        protected internal NDbFactory Factory
        {
            get
            {
                if (null == this.Config)
                    return null;
                return this.Config.Factory;
            }
        }

        #endregion

        #endregion

        #region Virtual Methods

        #region For Connect/Disconnet process

        /// <summary>
        /// OnConnecting
        /// </summary>
        protected virtual void OnConnecting() { }
        /// <summary>
        /// OnConnected
        /// </summary>
        protected virtual void OnConnected() { }
        /// <summary>
        /// OnConnectFailed
        /// </summary>
        protected virtual void OnConnectFailed() { }
        /// <summary>
        /// OnDisconnecting
        /// </summary>
        protected virtual void OnDisconnecting() { }
        /// <summary>
        /// OnDisconnected
        /// </summary>
        protected virtual void OnDisconnected() { }

        #endregion

        #region For autodetected methods

        /// <summary>
        /// Overrides to provide information about other resource is still
        /// used the connection to prevent auto detected borken check routune activate.
        /// Note. If inherited class is not override this method the result always be false
        /// if no record set is still active. When inherited make sure to call base class to 
        /// get the result before do another logic and combine result to make sure no
        /// record set is still alive.
        /// </summary>
        /// <returns>Returns true if other resource is still used the connection.</returns>
        protected virtual bool IsConnectionInUsed()
        {
            bool result = false;
            if (!result)
            {
                // the base class is not used so check is recordset is alive or not
                return IsRecordsetsStillAlive();
            }
            else
            {
                // the base class is in used so return the base class state
                return result;
            }
        }
        /// <summary>
        /// Run Test Query. Overrides to execute the test query. When inherited make sure to call base class to 
        /// get the result before do another logic and combine result to make sure no
        /// record set is still alive.
        /// </summary>
        /// <param name="testQuery">The test query.</param>
        /// <returns>Returns true if test query is successfully executed.</returns>
        protected virtual bool RunTestQuery(string testQuery)
        {
            bool success = true;
            if (!string.IsNullOrWhiteSpace(testQuery))
            {
                // execute when base status is success only.
                DataTable table = this.Query(testQuery);
                success = (table != null && table.Rows.Count > 0);
                if (table != null) table.Dispose();
                table = null;
            }

            return success;
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Connect to database.
        /// </summary>
        /// <returns>Return true if connect process is success.</returns>
        public bool Connect()
        {
            if (IsConnected)
                return true; // Already connected.
            ConnectToDatabase();
            return this.IsConnected;
        }
        /// <summary>
        /// Disconnect from database. When call this method the connection config would not changed
        /// so when call connect again the exists connection config will used to connect to database.
        /// </summary>
        public void Disconnect()
        {
            if (!this.IsConnected)
                return; // Already disconnected
            DoDisonnecting(); // Raise event
            if (null == this.DbConnection || null == _config)
            {
                DoDisconnected(); // Raise event
                return;
            }
            lock (this)
            {
                #region Remove from connection pools

                try
                {
                    string uniqueName = this.GetUniqueName();
                    if (string.IsNullOrWhiteSpace(uniqueName))
                    {
                        DoDisconnected(); // Raise event
                        return;
                    }
                    // Remove and release reference
                    // this method will automatically release reference and auto close if no reference
                    // exists.
                    ConnectionPools.Instance.Remove(uniqueName);
                }
                catch (Exception ex)
                {
                    MethodBase med = MethodBase.GetCurrentMethod();
                    // Close connection failed.
                    "Disconnect Failed.".Info(med);
                    ex.Err(med);
                    RaiseErr(ex);
                }
                finally
                {
                    DoDisconnected(); // Raise event
                }

                #endregion
            }
            // Free Memory
            NGC.FreeGC();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Connection Configuration.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets Connection Configuration.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NDbConfig Config
        {
            get { return _config; }
            set
            {
                if (_config != value)
                {
                    UnhookConfigEvents();
                    Disconnect();
                    _config = value;
                    HookConfigEvents();
                }
            }
        }
        /// <summary>
        /// Gets DbConnection insatnce.
        /// </summary>
        [Category("Connection")]
        [Description("Gets DbConnection insatnce.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DbConnection DbConnection
        {
            get
            {
                if (null == _config)
                    return null;
                string uniqueName = GetUniqueName();
                if (string.IsNullOrWhiteSpace(uniqueName))
                {
                    return null;
                }
                ConnectionRef connRef = ConnectionPools.Instance[uniqueName];
                if (null != connRef && null != connRef.DbConnection)
                    return connRef.DbConnection;
                else return null;
            }
        }
        /// <summary>
        /// Check Is database connected.
        /// </summary>
        [Category("Connection")]
        [Description("Check Is database connected.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsConnected
        {
            get
            {
                if (_connecting)
                    return false;
                else return (null != this.DbConnection && 
                    this.DbConnection.State == ConnectionState.Open);
            }
        }
        /// <summary>
        /// Checks is in connecting process.
        /// </summary>
        [Category("Connection")]
        [Description("Checks is in connecting process.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsConnecting { get { return _connecting; } }
        /// <summary>
        /// Gets or sets is shared connection.
        /// </summary>
        [Category("Options")]
        [Description("Gets or sets is shared connection.")]
        [Browsable(true)]
        [XmlAttribute]
        public bool SharedConnection
        {
            get { return _shareConnection; }
            set
            {
                if (_shareConnection != value)
                {
                    Disconnect(); // disconnect first
                    _shareConnection = value;
                }
            }
        }

        #endregion

        #region Public Events

        /// <summary>
        /// OnException Event. 
        /// Occur when Exception is occur.
        /// </summary>
        [Category("Connections")]
        [Description("OnException Event. Occur when Exception is occur.")]
        public event ExecuteExceptionEventHandler OnException;
        /// <summary>
        /// Connecting Event. Occur when Connect method is called.
        /// </summary>
        [Category("Connections")]
        [Description("Connecting Event. Occur when Connect method is called.")]
        public event System.EventHandler Connecting;
        /// <summary>
        /// Connected Event. 
        /// Occur when Connect process is success.
        /// </summary>
        [Category("Connections")]
        [Description("Connected Event. Occur when Connect process is success.")]
        public event System.EventHandler Connected;
        /// <summary>
        /// ConnectFailed Event. 
        /// Occur when Connect process is failed.
        /// </summary>
        [Category("Connections")]
        [Description("ConnectFailed Event. Occur when Connect process is failed.")]
        public event System.EventHandler ConnectFailed;
        /// <summary>
        /// Disconnecting Event. 
        /// Occur when Disconnect method is called.
        /// </summary>
        [Category("Connections")]
        [Description("Disconnecting Event. Occur when Disconnect method is called.")]
        public event System.EventHandler Disconnecting;
        /// <summary>
        /// Disconnected Event. 
        /// Occur when Disconnect process is completed.
        /// </summary>
        [Category("Connections")]
        [Description("Disconnected Event. Occur when Disconnect process is completed.")]
        public event System.EventHandler Disconnected;
        /// <summary>
        /// ConnectionBroken Event. 
        /// Occur when Connection is broken.
        /// </summary>
        [Category("Connections")]
        [Description("ConnectionBroken Event. Occur when Connection is broken.")]
        public event System.EventHandler ConnectionBroken;
        /// <summary>
        /// ConnectionAlive Event. 
        /// Occur when Connection check process occur and still alive.
        /// </summary>
        [Category("Connections")]
        [Description("ConnectionAlive Event. Occur when Connection check process occur and still alive.")]
        public event System.EventHandler ConnectionAlive;

        #endregion
    }

    #endregion

    #region NDbConnection - Command (Query/Execute/ExecuteStoredProcedure)

    // Command (Query/Execute/ExecuteStoredProcedure)
    partial class NDbConnection
    {
        #region Internal Variables

        private List<RecordsetReference> _recordsets = new List<RecordsetReference>();

        #endregion

        #region Private Methods

        private void ClearRecordsets()
        {
            if (null != _recordsets)
            {
                RecordsetReference[] rsRefs = null;
                lock (this)
                {
                    rsRefs = _recordsets.ToArray();
                }
                if (null != rsRefs)
                {
                    MethodBase med = MethodBase.GetCurrentMethod();

                    if (this.EnableDetailDebug)
                    {
                        "Begin release all recordsets.".Info(med);
                        string msg = string.Format(
                            "Number of recordset reference {0}.", rsRefs.Length);
                        msg.Info(med);
                    }
                    foreach (RecordsetReference rsRef in rsRefs)
                    {
                        if (null == rsRef)
                            continue;
                        if (this.EnableDetailDebug)
                        {
                            "Free Recordset Reference (Call by ClearRecordsets)".Info(med);
                        }
                        rsRef.Free(); // Free the recordset
                    }
                    if (this.EnableDetailDebug)
                    {
                        "Finished release all recordsets.".Info(med);
                    }
                }
                lock (this)
                {
                    _recordsets.Clear();
                }
            }
        }

        private void FreeRecordsets()
        {
            //ClearRecordsets();
            _recordsets = null;
        }

        private bool IsRecordsetsStillAlive()
        {
            if (null == _recordsets || _recordsets.Count <= 0)
                return false;
            RecordsetReference[] allRS = _recordsets.ToArray();
            foreach (RecordsetReference rsRef in allRS)
            {
                if (null != rsRef && !rsRef.IsAlive())
                {
                    MethodBase med = MethodBase.GetCurrentMethod();
                    if (this.EnableDetailDebug)
                    {
                        "Remove and Free Recordset Reference".Info(med);
                    }
                    _recordsets.Remove(rsRef); // Recordset is dead or unused remove from list
                    rsRef.Free(); // Free the recordset
                }
                else
                {
                    // Some record set still alive
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Public Methods

        #region Query

        /// <summary>
        /// Query.
        /// </summary>
        /// <param name="queryText">command text.</param>
        /// <param name="transaction">Transaction instance.</param>
        /// <returns>Result DataTable that return from database.</returns>
        public ExecuteResult<DataTable> Query(string queryText, 
            DbTransaction transaction = null)
        {
            ExecuteResult<DataTable> executeResult = new ExecuteResult<DataTable>();

            if (!string.IsNullOrWhiteSpace(queryText) &&
                null != this.Factory &&
                null != this.DbConnection && this.IsConnected)
            {
                MethodBase med = MethodBase.GetCurrentMethod();

                DbCommand command = this.Factory.CreateCommand(this.DbConnection,
                    transaction);

                if (null == command)
                    return executeResult;
                else
                {
                    command.CommandType = CommandType.Text; // Set as command text
                    command.CommandText = queryText;
                    try
                    {
                        executeResult = Query(command, transaction);
                    }
                    catch (Exception ex)
                    {
                        ex.Err(med);
                    }
                    finally
                    {
                        #region Free command

                        if (null != command)
                        {
                            try { command.Dispose(); }
                            catch (Exception ex)
                            {
                                "Dispose command detected exception.".Err(med);
                                ex.Err(med);
                            }
                            finally
                            {
                                // Free Memory
                                NGC.FreeGC(command);
                                command = null;
                            }
                        }

                        #endregion

                        ShowNormalCursor();
                    }
                    return executeResult;
                }
            }
            else return executeResult;
        }
        /// <summary>
        /// Query.
        /// </summary>
        /// <param name="command">Command object that already initialized parameters.</param>
        /// <param name="transaction">Transaction instance.</param>
        /// <returns>Result DataTable that return from database.</returns>
        public ExecuteResult<DataTable> Query(DbCommand command,
            DbTransaction transaction = null)
        {
            ExecuteResult<DataTable> executeResult = new ExecuteResult<DataTable>();

            if (null != this.Factory &&
                null != command && null != command.Connection &&
                command.Connection.State == ConnectionState.Open)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                // Free Memory
                NGC.FreeGC();
                if (null != transaction &&
                    command.Transaction != transaction)
                {
                    DbTransaction oldTrans = command.Transaction; // Keep original
                    try 
                    { 
                        command.Transaction = transaction; // Assigned Transaction 
                    }
                    catch (Exception ex)
                    {
                        "Detected exception in assigned transaction.".Err(med);
                        ex.Err(med);
                        command.Transaction = oldTrans; // Assigned original
                    }
                }

                DbDataAdapter adaptor = this.Factory.CreateAdapter(command);
                if (null != adaptor)
                {
                    DataSet dataSet = new DataSet();
                    try
                    {
                        #region Fill

                        ShowBusyCursor();
                        _onExecuting = true; // Mark executing flag
                        lock (this)
                        {
                            adaptor.Fill(dataSet);
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        if (null != command)
                        {
                            string msg = "Fill adaptor error. CommandText : " + 
                                Environment.NewLine +
                                command.CommandText;
                            msg.Err(med);
                        }
                        ex.Err(med);
                        // Set Exception
                        executeResult.SetException(ex);
                    }
                    finally
                    {
                        #region Free adaptor

                        if (null != adaptor)
                        {
                            try
                            {
                                adaptor.Dispose();
                            }
                            catch (Exception ex)
                            {
                                "Dispose adaptor detected exception.".Err(med);
                                ex.Err(med);
                            }
                        }
                        adaptor = null;

                        #endregion

                        ShowNormalCursor();
                        _onExecuting = false; // Reset flag
                    }

                    if (null != dataSet && null != dataSet.Tables &&
                        dataSet.Tables.Count > 0)
                    {
                        ShowNormalCursor(); // Make sure cursor is display as normal
                        executeResult.SetResult(dataSet.Tables[0]); // Set Result
                    }

                    #region Free dataset

                    if (null != dataSet)
                    {
                        try { dataSet.Dispose(); }
                        catch (Exception ex)
                        {
                            if (this.EnableDetailDebug)
                            {
                                "Dispose DataSet error.".Err(med);
                                ex.Err(med);
                            }
                        }
                        finally
                        {
                            ShowNormalCursor();
                        }
                    }
                    dataSet = null;

                    #endregion

                    return executeResult;
                }
                else
                {
                    // Cannot create adaptor
                    "Cannot create Adaptor.".Info(med);
                    return executeResult;
                }
            }
            else return executeResult;
        }

        #endregion

        #region Execute Non Query

        /// <summary>
        /// Execute Non Query.
        /// </summary>
        /// <param name="commandText">command text.</param>
        /// <param name="transaction">Transaction instance.</param>
        /// <returns>Result that return after execute command.</returns>
        public ExecuteResult<int> ExecuteNonQuery(string commandText, 
            DbTransaction transaction = null)
        {
            ExecuteResult<int> executeResult = new ExecuteResult<int>(-1);

            if (!string.IsNullOrWhiteSpace(commandText) &&
                null != this.Factory &&
                null != this.DbConnection && this.IsConnected)
            {
                MethodBase med = MethodBase.GetCurrentMethod();

                DbCommand command = this.Factory.CreateCommand(this.DbConnection,
                    transaction);
                if (null == command)
                    return executeResult;
                else
                {
                    command.CommandType = CommandType.Text; // Set as command text
                    command.CommandText = commandText;
                    try
                    {
                        executeResult = ExecuteNonQuery(command, transaction);
                    }
                    catch (Exception ex)
                    {
                        ex.Err(med);
                    }
                    finally
                    {
                        if (null != command)
                        {
                            try { command.Dispose(); }
                            catch (Exception ex)
                            {
                                "Dispose command detected exception.".Err(med);
                                ex.Err(med);
                            }
                            finally
                            {
                                // Free Memory
                                NGC.FreeGC(command);
                                command = null;
                            }
                        }
                        ShowNormalCursor();
                    }
                    return executeResult;
                }
            }
            else return executeResult;
        }
        /// <summary>
        /// Execute Non Query
        /// </summary>
        /// <param name="command">Command object that already initialized parameters</param>
        /// <param name="transaction">Transaction instance</param>
        /// <returns>Result that return after execute command</returns>
        public ExecuteResult<int> ExecuteNonQuery(DbCommand command, 
            DbTransaction transaction = null)
        {
            ExecuteResult<int> executeResult = new ExecuteResult<int>(-1);

            if (null != this.Factory &&
                null != command && null != command.Connection &&
                command.Connection.State == ConnectionState.Open)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                // Free Memory
                NGC.FreeGC();

                if (null != transaction &&
                    command.Transaction != transaction)
                {
                    DbTransaction oldTrans = command.Transaction; // Keep original
                    try
                    {
                        command.Transaction = transaction; // Assigned Transaction
                    }
                    catch (Exception ex)
                    {
                        "Detected exception in assigned transaction.".Err(med);
                        ex.Err(med);
                        command.Transaction = oldTrans; // Assigned original
                    }
                }

                int result = -1;
                try
                {
                    ShowBusyCursor();
                    _onExecuting = true; // Mark executing flag
                    lock (this)
                    {
                        result = command.ExecuteNonQuery();
                    }
                    ShowNormalCursor();
                }
                catch (Exception ex)
                {
                    if (null != command)
                    {
                        string msg = "Execute Error. CommandText : " + Environment.NewLine +
                            command.CommandText;
                        msg.Err(med);
                    }
                    ex.Err(med);
                    executeResult.SetException(ex); // Set Exception
                    result = -1;
                }
                finally
                {
                    _onExecuting = false; // Reset flag
                    ShowNormalCursor();
                }
                executeResult.SetResult(result); // Set Result
                return executeResult;
            }
            else return executeResult;
        }

        #endregion

        #region Execute Scalar

        /// <summary>
        /// Execute Scalar.
        /// </summary>
        /// <param name="commandText">command text.</param>
        /// <param name="transaction">Transaction instance.</param>
        /// <returns>Result that return after execute command.</returns>
        public ExecuteResult<object> ExecuteScalar(string commandText, 
            DbTransaction transaction = null)
        {
            ExecuteResult<object> executeResult = new ExecuteResult<object>();

            if (!string.IsNullOrWhiteSpace(commandText) &&
                null != this.Factory &&
                null != this.DbConnection && this.IsConnected)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                DbCommand command = this.Factory.CreateCommand(this.DbConnection, transaction);
                if (command == null)
                {
                    return executeResult;
                }
                else
                {
                    command.CommandType = CommandType.Text; // Set as command text
                    command.CommandText = commandText;
                    try
                    {
                        executeResult = ExecuteScalar(command, transaction);
                    }
                    catch (Exception ex)
                    {
                        ex.Err(med);
                    }
                    finally
                    {
                        if (command != null)
                        {
                            try { command.Dispose(); }
                            catch (Exception ex)
                            {
                                if (this.EnableDetailDebug)
                                {
                                    "Dispose command detected exception.".Err(med);
                                    ex.Err(med);
                                }
                            }
                            finally
                            {
                                // Free Memory
                                NGC.FreeGC(command);
                                command = null;
                            }
                        }
                        ShowNormalCursor();
                    }
                    return executeResult;
                }
            }
            else return executeResult;
        }
        /// <summary>
        /// Execute Scalar.
        /// </summary>
        /// <param name="command">Command object that already initialized parameters.</param>
        /// <param name="transaction">Transaction instance.</param>
        /// <returns>Result that return after execute command.</returns>
        public ExecuteResult<object> ExecuteScalar(DbCommand command, 
            DbTransaction transaction = null)
        {
            ExecuteResult<object> executeResult = new ExecuteResult<object>();

            if (null != this.Factory &&
                null != command && null != command.Connection &&
                command.Connection.State == ConnectionState.Open)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                // Free Memory
                NGC.FreeGC();

                if (null != transaction &&
                    command.Transaction != transaction)
                {
                    DbTransaction oldTrans = command.Transaction; // Keep original
                    try
                    {
                        command.Transaction = transaction; // Assigned Transaction
                    }
                    catch (Exception ex)
                    {
                        "Detected exception in assigned transaction.".Err(med);
                        ex.Err(med);
                        command.Transaction = oldTrans; // Assigned original
                    }
                }

                object result = null;
                try
                {
                    ShowBusyCursor();
                    _onExecuting = true; // Mark executing flag
                    lock (this)
                    {
                        result = command.ExecuteScalar();
                    }
                    ShowNormalCursor();
                }
                catch (Exception ex)
                {
                    if (null != command)
                    {
                        string msg = "ExecuteScalar Error. CommandText : " + Environment.NewLine +
                            command.CommandText;
                        msg.Err(med);
                    }
                    ex.Err(med);
                    executeResult.SetException(ex); // Set Exception
                    result = null;
                }
                finally
                {
                    _onExecuting = false; // Reset flag
                    ShowNormalCursor();
                }

                executeResult.SetResult(result); // Set Result

                return executeResult;
            }
            else return executeResult;
        }

        #endregion

        #region Execute Reader

        /// <summary>
        /// Execute Reader.
        /// </summary>
        /// <param name="commandText">command text.</param>
        /// <param name="transaction">Transaction instance.</param>
        /// <returns>Array of DataTable instance that used to retrived returned data from database.</returns>
        public ExecuteResult<DataTable[]> ExecuteReader(string commandText, 
            DbTransaction transaction = null)
        {
            ExecuteResult<DataTable[]> executeResult = new ExecuteResult<DataTable[]>();

            if (!string.IsNullOrWhiteSpace(commandText) &&
                null != this.Factory &&
                null != this.DbConnection && this.IsConnected)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                DbCommand command = this.Factory.CreateCommand(this.DbConnection, transaction);
                if (null == command)
                {
                    return executeResult;
                }
                else
                {
                    command.CommandType = CommandType.Text; // Set as command text
                    command.CommandText = commandText;
                    try
                    {
                        executeResult = ExecuteReader(command, transaction);
                    }
                    catch (Exception ex)
                    {
                        ex.Err(med);
                    }
                    finally
                    {
                        if (null != command)
                        {
                            try { command.Dispose(); }
                            catch (Exception ex)
                            {
                                if (this.EnableDetailDebug)
                                {
                                    "Dispose command detected exception.".Err(med);
                                    ex.Err(med);
                                }
                            }
                            finally
                            {
                                // Free Memory
                                NGC.FreeGC(command);
                                command = null;
                            }
                        }
                        ShowNormalCursor();
                    }
                    return executeResult;
                }
            }
            else return executeResult;
        }
        /// <summary>
        /// Execute Reader.
        /// </summary>
        /// <param name="command">Command object that already initialized parameters.</param>
        /// <returns>Array of DataTable instance that used to retrived returned data from database.</returns>
        public ExecuteResult<DataTable[]> ExecuteReader(DbCommand command)
        {
            return ExecuteReader(command, null);
        }
        /// <summary>
        /// Execute Reader.
        /// </summary>
        /// <param name="command">Command object that already initialized parameters.</param>
        /// <param name="transaction">Transaction instance.</param>
        /// <returns>Array of DataTable instance that used to retrived returned data from database.</returns>
        public ExecuteResult<DataTable[]> ExecuteReader(DbCommand command, DbTransaction transaction)
        {
            ExecuteResult<DataTable[]> executeResult = new ExecuteResult<DataTable[]>();

            if (null != this.Factory &&
                null != command && null != command.Connection &&
                command.Connection.State == ConnectionState.Open)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                // Free Memory
                NGC.FreeGC();

                if (null != transaction &&
                    command.Transaction != transaction)
                {
                    DbTransaction oldTrans = command.Transaction; // Keep original
                    try
                    {
                        command.Transaction = transaction; // Assigned Transaction
                    }
                    catch (Exception ex)
                    {
                        "Detected exception in assigned transaction.".Err(med);
                        ex.Err(med);
                        command.Transaction = oldTrans; // Assigned original
                    }
                }

                DataTable[] results = null;
                DbDataReader reader = null;

                try
                {
                    ShowBusyCursor();
                    _onExecuting = true; // Mark executing flag
                    lock (this)
                    {
                        reader = command.ExecuteReader(CommandBehavior.Default);
                    }
                    if (null != reader)
                    {
                        List<DataTable> tables = new List<DataTable>();
                        DataReaderAdapter adaptor = new DataReaderAdapter();
                        DataTable table = null;
                        try
                        {
                            do
                            {
                                #region Fill

                                try
                                {
                                    table = new DataTable();
                                    lock (this)
                                    {
                                        // Used Default Data Reader Adaptor
                                        adaptor.FillFromReader(table, reader);
                                        // a little bit faster but not much.
                                        //adaptor.FillFromReaderOptimize(table, reader);
                                    }
                                }
                                catch (Exception ex0)
                                {
                                    "Detected exception in Fill operation.".Err(med);
                                    ex0.Err(med);

                                    #region Free table

                                    if (null != table)
                                    {
                                        try { table.Dispose(); }
                                        catch { } // Ignore
                                    }
                                    // Free Memory
                                    NGC.FreeGC(table);
                                    table = null;

                                    #endregion
                                }
                                if (null != table) tables.Add(table);

                                #endregion
                            }
                            while (reader.NextResult());
                            // All OK
                            results = tables.ToArray();
                        }
                        catch (Exception err)
                        {
                            "Detected exception in Read Next operation.".Err(med);
                            err.Err(med);
                        }

                        #region Close

                        try
                        {
                            if (null != reader && !reader.IsClosed)
                                reader.Close();
                        }
                        catch (Exception ex1)
                        {
                            "Detected exception in Close Reader operation.".Err(med);
                            ex1.Err(med);
                        }

                        #endregion

                        #region Dispose

                        try
                        {
                            reader.Dispose();
                            // Free Memory
                            NGC.FreeGC(reader);
                        }
                        catch (Exception ex2)
                        {
                            "Detected exception in Dispose Reader operation.".Err(med);
                            ex2.Err(med);
                        }

                        #endregion

                        reader = null;
                    }
                    ShowNormalCursor();
                }
                catch (Exception ex)
                {
                    if (null != command)
                    {
                        string msg = "ExecuteReader Error. CommandText : " + Environment.NewLine +
                            command.CommandText;
                        msg.Err(med);
                    }
                    ex.Err(med);
                    results = null;
                }
                finally
                {
                    _onExecuting = false; // Reset flag
                    ShowNormalCursor();
                }
                executeResult.SetResult(results); // Set Result
                return executeResult;
            }
            else return executeResult;
        }

        #endregion

        #region Execute Procedure

        /// <summary>
        /// Execute Procedure.
        /// </summary>
        /// <param name="procedureName">Procedure's Name.</param>
        /// <param name="parameterNames">list of procedure's parameter name.</param>
        /// <param name="parameterValues">list of procedure's parameter value.</param>
        /// <param name="transaction">The transaction instance.</param>
        /// <returns>Returns stored procedure result.</returns>
        public ExecuteResult<StoredProcedureResult> ExecuteProcedure(string procedureName,
            string[] parameterNames, object[] parameterValues,
            DbTransaction transaction = null)
        {
            ExecuteResult<StoredProcedureResult> executeResult = new ExecuteResult<StoredProcedureResult>();

            if (!string.IsNullOrWhiteSpace(procedureName) &&
                null != this.Factory &&
                null != this.DbConnection && this.IsConnected)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                if (null != parameterNames && null != parameterValues &&
                    parameterNames.Length != parameterValues.Length)
                {
                    string msg = "Parameter Names and Values is not equal. [" + procedureName + "]";
                    msg.Err(med);
                    return executeResult;
                }

                DbCommand command = this.Factory.CreateCommand(this.DbConnection, transaction);
                if (null == command)
                {
                    return executeResult;
                }
                else
                {
                    command.CommandType = CommandType.StoredProcedure; // Set as Stored Procedure
                    command.CommandText = procedureName;
                    try
                    {
                        executeResult = ExecuteProcedure(command, parameterNames, parameterValues, transaction);
                    }
                    catch (Exception ex)
                    {
                        ex.Err(med);
                    }
                    finally
                    {
                        if (null != command)
                        {
                            try { command.Dispose(); }
                            catch (Exception ex)
                            {
                                if (this.EnableDetailDebug)
                                {
                                    "Dispose command detected exception.".Err(med);
                                    ex.Err(med);
                                }
                            }
                            finally
                            {
                                // Free Memory
                                NGC.FreeGC(command);
                                command = null;
                            }
                        }
                        ShowNormalCursor();
                    }

                    return executeResult;
                }
            }
            else return executeResult;
        }
        /// <summary>
        /// Execute Procedure.
        /// </summary>
        /// <param name="command">Command object that already initialized parameters.</param>
        /// <param name="parameterNames">list of procedure's parameter name.</param>
        /// <param name="parameterValues">list of procedure's parameter value.</param>
        /// <param name="transaction">Transaction instance.</param>
        /// <returns>Returns stored procedure result.</returns>
        public ExecuteResult<StoredProcedureResult> ExecuteProcedure(DbCommand command,
            string[] parameterNames, object[] parameterValues,
            DbTransaction transaction = null)
        {
            ExecuteResult<StoredProcedureResult> executeResult = new ExecuteResult<StoredProcedureResult>();

            if (null != this.Factory &&
                null != command && null != command.Connection &&
                command.Connection.State == ConnectionState.Open)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                // Free Memory
                NGC.FreeGC();

                if (null != parameterNames && null != parameterValues &&
                    parameterNames.Length != parameterValues.Length)
                {
                    string msg = "Parameter Names and Values is not equal. [" + command.CommandText + "]";
                    msg.Err(med);
                    return executeResult;
                }

                if (null != transaction &&
                    command.Transaction != transaction)
                {
                    DbTransaction oldTrans = command.Transaction; // Keep original
                    try
                    {
                        command.Transaction = transaction; // Assigned Transaction
                    }
                    catch (Exception ex)
                    {
                        "Detected exception in assigned transaction.".Err(med);
                        ex.Err(med);
                        command.Transaction = oldTrans; // Assigned original
                    }
                }
                bool successDerived = false;
                try
                {
                    successDerived = this.Factory.DerivedParameters(command);
                }
                catch (Exception ex)
                {
                    "Detected exception in Derived Parameters operation.".Err(med);
                    ex.Err(med);
                }

                if (!successDerived)
                    return executeResult;

                bool successAssigned = false;

                try
                {
                    successAssigned = this.Factory.AssignParameters(command,
                        parameterNames,
                        parameterValues);
                }
                catch (Exception ex)
                {
                    "Detected exception in Assign Parameters operation.".Err(med);
                    ex.Err(med);
                }

                StoredProcedureResult results = null;
                DataTable table = null;
                DbDataAdapter adaptor = null;
                try
                {
                    ShowBusyCursor();
                    _onExecuting = true; // Mark executing flag
                    lock (this)
                    {
                        adaptor = this.Factory.CreateAdapter(command);
                        if (null != adaptor)
                        {
                            table = new DataTable();
                            adaptor.Fill(table);
                        }
                    }
                    StoredProcedureResult spResult = new StoredProcedureResult();
                    foreach (DbParameter para in command.Parameters)
                    {
                        if (para.Direction == ParameterDirection.InputOutput ||
                            para.Direction == ParameterDirection.Output ||
                            para.Direction == ParameterDirection.ReturnValue)
                        {
                            if (!spResult.OutParameters.ContainsKey(para.ParameterName))
                                spResult.OutParameters.Add(para.ParameterName, para.Value);
                        }
                    }
                    // set result table.
                    spResult.Table = table;
                    results = spResult;
                    ShowNormalCursor();
                }
                catch (Exception ex)
                {
                    if (null != command)
                    {
                        string msg = "Execute Procedure Error. CommandText : " + Environment.NewLine +
                            command.CommandText;
                        msg.Err(med);
                    }
                    ex.Err(med);
                    executeResult.SetException(ex); // Set Exception
                    results = null;
                }
                finally
                {
                    if (null != adaptor)
                    {
                        try { adaptor.Dispose(); }
                        catch (Exception ex)
                        {
                            "Dispose adaptor detected exception.".Err(med);
                            ex.Err(med);
                        }
                    }
                    adaptor = null;
                    _onExecuting = false; // Reset flag
                    ShowNormalCursor();
                }
                executeResult.SetResult(results);
                return executeResult;
            }
            else return executeResult;
        }

        #endregion

        #region Execute Data Reader

        /// <summary>
        /// Execute Data Reader.
        /// </summary>
        /// <param name="commandText">command text.</param>
        /// <param name="transaction">Transaction instance.</param>
        /// <returns>Array of Recordset instance that used to retrived returned data from database.</returns>
        public ExecuteResult<Recordset> ExecuteDataReader(string commandText, 
            DbTransaction transaction = null)
        {
            ExecuteResult<Recordset> executeResult = new ExecuteResult<Recordset>();

            if (!string.IsNullOrWhiteSpace(commandText) &&
                null != this.Factory &&
                null != this.DbConnection && this.IsConnected)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                DbCommand command = this.Factory.CreateCommand(this.DbConnection,
                    transaction);
                if (null == command)
                    return executeResult;
                else
                {
                    command.CommandType = CommandType.Text; // Set as command text
                    command.CommandText = commandText;
                    try
                    {
                        executeResult = ExecuteDataReader(command, transaction);
                    }
                    catch (Exception ex)
                    {
                        ex.Err(med);
                    }
                    finally
                    {
                        // Execute not required to dispose object at this state
                        ShowNormalCursor();
                    }
                    return executeResult;
                }
            }
            else return executeResult;
        }
        /// <summary>
        /// Execute Data Reader.
        /// </summary>
        /// <param name="command">Command object that already initialized parameters.</param>
        /// <param name="transaction">Transaction instance.</param>
        /// <returns>Array of DataReaderResult instance that used to retrived returned data from database.</returns>
        public ExecuteResult<Recordset> ExecuteDataReader(DbCommand command, 
            DbTransaction transaction = null)
        {
            ExecuteResult<Recordset> executeResult = new ExecuteResult<Recordset>();

            if (null != this.Factory &&
                null != command && null != command.Connection &&
                command.Connection.State == ConnectionState.Open)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                // Free Memory
                NGC.FreeGC();
                if (null != transaction &&
                    command.Transaction != transaction)
                {
                    DbTransaction oldTrans = command.Transaction; // Keep original
                    try
                    {
                        command.Transaction = transaction; // Assigned Transaction
                    }
                    catch (Exception ex)
                    {
                        "Detected exception in assigned transaction.".Err(med);
                        ex.Err(med);
                        command.Transaction = oldTrans; // Assigned original
                    }
                }

                Recordset result = null;
                DbDataReader reader = null;

                try
                {
                    ShowBusyCursor();
                    _onExecuting = true; // Mark executing flag
                    lock (this)
                    {
                        reader = command.ExecuteReader(CommandBehavior.Default);
                    }
                    if (null != reader)
                    {
                        result = new Recordset();
                        result.Init(command, reader);
                        // create internal reference
                        if (null != _recordsets)
                        {
                            RecordsetReference rsRef = new RecordsetReference(command, reader);
                            _recordsets.Add(rsRef);
                            if (this.EnableDetailDebug)
                            {
                                "Recordset Reference is registered.".Info(med);
                            }
                        }
                        else
                        {
                            if (this.EnableDetailDebug)
                            {
                                "Cannot add recordset reference into null list.".Info(med);
                            }
                        }
                    }
                    ShowNormalCursor();
                }
                catch (Exception ex)
                {
                    if (null != command)
                    {
                        string msg = "Execute Data Reader Error. CommandText : " + Environment.NewLine +
                            command.CommandText;
                        msg.Err(med);
                    }
                    ex.Err(med);
                    result = null;
                }
                finally
                {
                    _onExecuting = false; // Reset flag
                    ShowNormalCursor();
                }
                executeResult.SetResult(result); // Set Result
                return executeResult;
            }
            else return executeResult;
        }

        #endregion

        #region Database Functions

        /// <summary>
        /// Get System Date Time.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <returns>Current DateTime from database server if connected otherwise return DateTime.Now.</returns>
        public DateTime GetSystemDateTime(DbTransaction transaction = null)
        {
            DateTime dt = DateTime.Now;

            if (null != this.DbConnection &&
                this.DbConnection.State == ConnectionState.Open)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                try
                {
                    ShowBusyCursor();
                    _onExecuting = true; // Mark executing flag
                    //lock (this) // not need to lock because the config is already contain lock statement
                    {
                        dt = this.Factory.GetCurrentDate(this.DbConnection, transaction);
                    }
                    ShowNormalCursor();
                }
                catch (Exception ex)
                {
                    "Detected exception in GetSystemDate operation.".Err(med);
                    ex.Err(med);
                    dt = DateTime.Now;
                }
                finally
                {
                    _onExecuting = false; // Reset flag
                    ShowNormalCursor();
                }
            }
            return dt;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Checks is on executing.
        /// </summary>
        [Category("Options")]
        [Description("Checks is on executing.")]
        [Browsable(false)]
        [XmlIgnore]
        public bool IsExecuting
        {
            get 
            {
                if (!IsConnected)
                    return false;
                return (IsRecordsetsStillAlive() || _onExecuting);
            }
        }

        #endregion
    }

    #endregion

    #region NDbConnection - Schema

    // Schema Access
    partial class NDbConnection
    {
        #region Internal Variables

        private List<NDbProviderDataType> _providerTypes = null;

        #endregion

        #region Private Method

        private void ClearProviderTypes()
        {
            // clear and release reference.
            lock (this)
            {
                if (_providerTypes != null)
                {
                    _providerTypes.Clear();
                }
                _providerTypes = null;
            }
        }

        private string FormatTableName(string tableName, string owner = null)
        {
            if (null == this.Factory ||
                null == this.Factory.Formatter)
            {
                return string.Empty;
            }
            else
            {
                return this.Factory.Formatter.FormatTableOrViewName(owner, tableName);
            }
        }

        #endregion

        #region Public Methods

        #region Get Meta Data Collection

        /// <summary>
        /// Get Meta Data Collection.
        /// </summary>
        /// <returns>List of avaliable metadata information.</returns>
        public List<NDbMetaData> GetMetaDataCollection()
        {
            if (null != this.Factory)
            {
                _onExecuting = true; // Mark executing flag
                List<NDbMetaData> results = this.Factory.GetMetadata(this.DbConnection);
                _onExecuting = false; // Mark executing flag
                return results;
            }
            return null;
        }

        #endregion

        #region Get Provider DataTypes

        /// <summary>
        /// Get Provider DataTypes. This method is used for Get all DataTypes for the current data provider.
        /// </summary>
        /// <returns>List of Provider datatypes.</returns>
        public List<NDbProviderDataType> GetProviderDataTypes()
        {
            if (null != this.Factory)
            {
                _onExecuting = true; // Mark executing flag
                List<NDbProviderDataType> results = 
                    this.Factory.GetProviderDataTypes(this.DbConnection);
                _onExecuting = false; // Mark executing flag
                return results;
            }
            return null;
        }
        /// <summary>
        /// Get Provider Type By ID.
        /// </summary>
        /// <param name="providerTypeID">The Provider Type ID.</param>
        /// <returns>Return match DbProviderDataType instance if found otherwise return null.</returns>
        public NDbProviderDataType GetProviderTypeByID(int providerTypeID)
        {
            NDbProviderDataType result = null;
            if (_providerTypes == null)
            {
                _providerTypes = this.GetProviderDataTypes();
            }
            if (_providerTypes != null)
            {
                foreach (NDbProviderDataType type in _providerTypes)
                {
                    if (type.ProviderDbType == providerTypeID)
                    {
                        result = type;
                        break;
                    }
                }
            }
            return result;
        }

        #endregion

        #region Get Reserved Words

        /// <summary>
        /// Get Reserved Words. This method is used for Get all reserved words for the current data provider.
        /// </summary>
        /// <returns>List of Reserved words.</returns>
        public List<NDbReservedword> GetReservedWords()
        {
            if (null != this.Factory)
            {
                _onExecuting = true; // Mark executing flag
                List<NDbReservedword> results = 
                    this.Factory.GetReservedwords(this.DbConnection);
                _onExecuting = false; // Mark executing flag
                return results;
            }
            return null;
        }

        #endregion

        #region Get Restrictions

        /// <summary>
        /// Get Restrictions for specificed Metadata.
        /// </summary>
        /// <param name="value">MetaData object to find information.</param>
        /// <returns>List of Restriction that need to get information.</returns>
        public List<NDbRestriction> GetRestrictions(NDbMetaData value)
        {
            if (null != this.Factory)
            {
                _onExecuting = true; // Mark executing flag
                List<NDbRestriction> results = 
                    this.Factory.GetRestrictions(this.DbConnection, value);
                _onExecuting = false; // Mark executing flag
                return results;
            }
            return null;
        }

        #endregion

        #region Get Schema

        /// <summary>
        /// Get Schema. This Method is used for Get Schema information for specificed Meta Data information.
        /// </summary>
        /// <param name="value">specificed Metadata to find information.</param>
        /// <returns>Information about specificed metadata.</returns>
        public DataTable GetSchema(NDbMetaData value)
        {
            return GetSchema(value, null);
        }
        /// <summary>
        /// Get Schema. This Method is used for Get Schema information for specificed Meta Data information.
        /// </summary>
        /// <param name="value">specificed Metadata to find information.</param>
        /// <param name="restrictions">Restriction Array.</param>
        /// <returns>Information about specificed metadata.</returns>
        public DataTable GetSchema(NDbMetaData value, NDbRestriction[] restrictions)
        {
            if (null == value)
                return null;
            if (null != this.Factory)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                // Free Memory
                NGC.FreeGC();
                DataTable results = null;
                try
                {
                    _onExecuting = true; // Mark executing flag
                    lock (this)
                    {
                        results = this.Factory.GetSchema(
                            this.DbConnection, value, restrictions);
                    }
                }
                catch (Exception ex)
                {
                    "Detected exception in GetSchema operation.".Err(med);
                    ex.Err(med);
                    results = null;
                }
                finally
                {
                    _onExecuting = false; // Mark executing flag
                }

                return results;
            }
            else return null;
        }

        #endregion

        #region Get Schema Table

        /// <summary>
        /// Get Schema Table. This method is used to retrived Schema information about specificed
        /// Query i.e. column information.
        /// </summary>
        /// <param name="queryText">command text.</param>
        /// <returns>Result Schema Table.</returns>
        public DataTable GetSchemaTable(string queryText)
        {
            return GetSchemaTable(queryText, null);
        }
        /// <summary>
        /// Get Schema Table. This method is used to retrived Schema information about specificed
        /// Query i.e. column information.
        /// </summary>
        /// <param name="queryText">command text.</param>
        /// <param name="transaction">Transaction instance.</param>
        /// <returns>Result Schema Table.</returns>
        public DataTable GetSchemaTable(string queryText, DbTransaction transaction)
        {
            if (!string.IsNullOrWhiteSpace(queryText) &&
                null != this.Factory &&
                null != this.DbConnection && this.IsConnected)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                DbCommand command = this.Factory.CreateCommand(this.DbConnection,
                    transaction);
                if (null == command)
                    return null;
                else
                {
                    command.CommandType = CommandType.Text; // Set as command text
                    command.CommandText = queryText;
                    DataTable result = null;
                    try
                    {
                        result = GetSchemaTable(command, transaction);
                    }
                    catch (Exception ex)
                    {
                        ex.Err(med);
                    }
                    finally
                    {
                        if (command != null)
                        {
                            try { command.Dispose(); }
                            catch (Exception ex)
                            {
                                if (this.EnableDetailDebug)
                                {
                                    "Dispose command detected exception.".Err(med);
                                    ex.Err(med);
                                }
                            }
                            finally
                            {
                                // Free Memory
                                NGC.FreeGC(command);
                                command = null;
                            }
                        }
                    }
                    return result;
                }
            }
            else return null;
        }
        /// <summary>
        /// Get Schema Table. This method is used to retrived Schema information about specificed
        /// Command i.e. column information.
        /// </summary>
        /// <param name="command">Command object that already initialized parameters.</param>
        /// <returns>Result Schema Table.</returns>
        public DataTable GetSchemaTable(DbCommand command)
        {
            return GetSchemaTable(command, null);
        }
        /// <summary>
        /// Get Schema Table. This method is used to retrived Schema information about specificed
        /// Command i.e. column information.
        /// </summary>
        /// <param name="command">Command object that already initialized parameters.</param>
        /// <param name="transaction">Transaction instance.</param>
        /// <returns>Result Schema Table.</returns>
        public DataTable GetSchemaTable(DbCommand command, DbTransaction transaction)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            #region Write Debug info

            if (this.EnableDetailDebug)
            {
                "GetSchemaTable is call.".Info(med);
                if (null != command)
                {
                    string msg = "CommandText : " + command.CommandText;
                    msg.Info(med);
                }
                else
                {
                    "Command object is null".Info(med);
                }
            }

            #endregion

            if (null != this.DbConnection &&
                this.DbConnection.State == ConnectionState.Open &&
                null != this.Factory)
            {
                // Free Memory
                NGC.FreeGC();
                DataTable results = null;
                try
                {
                    _onExecuting = true; // Mark executing flag
                    results = this.Factory.GetSchemaTable(command, transaction);
                }
                catch (Exception ex)
                {
                    "Detected exception in GetSchemaTable operation.".Err(med);
                    ex.Err(med);
                    results = null;
                }
                finally
                {
                    _onExecuting = false; // Mark executing flag
                }

                return results;
            }
            else return null;
        }

        #endregion

        #region Get Tables

        /// <summary>
        /// Get Tables (and views).
        /// </summary>
        /// <returns>List of all avaliable Tables/Views from provider.</returns>
        public List<NDbTable> GetTables()
        {
            if (null == this.Config)
                return null;
            return GetTables(this.Config.DefaultOwner);
        }
        /// <summary>
        /// Get Tables (and views).
        /// </summary>
        /// <param name="owner">Owner of Tables/Views.</param>
        /// <returns>List of all avaliable Tables/Views from provider.</returns>
        public List<NDbTable> GetTables(string owner)
        {
            if (null != this.Factory)
            {
                _onExecuting = true; // Mark executing flag
                List<NDbTable> results = this.Factory.GetTables(this.DbConnection, owner);
                _onExecuting = false; // Mark executing flag
                return results;
            }
            return null;
        }

        #endregion

        #region Get Procedures

        /// <summary>
        /// Get Procedures.
        /// </summary>
        /// <returns>Returns List of Stored Procedure's name.</returns>
        public List<string> GetProcedures()
        {
            if (null == this.Config)
                return null;
            return GetProcedures(this.Config.DefaultOwner);
        }
        /// <summary>
        /// Get Procedures.
        /// </summary>
        /// <param name="owner">Owner of Stored Procedures.</param>
        /// <returns>Returns List of Stored Procedure's name.</returns>
        public List<string> GetProcedures(string owner)
        {
            if (null != this.Factory)
            {
                _onExecuting = true; // Mark executing flag
                List<string> results =
                    this.Factory.GetProcedures(this.DbConnection, owner);
                _onExecuting = false; // Mark executing flag

                return results;
            }
            return null;
        }

        #endregion

        #region GetProcedureInfo

        /// <summary>
        /// Gets Stored Procedure info.
        /// </summary>
        /// <param name="procedureName">Stored Procedure Name.</param>
        /// <returns>Returns Stored procedure information.</returns>
        public NDbProcedureInfo GetProcedureInfo(string procedureName)
        {
            return GetProcedureInfo(this.Config.DefaultOwner, procedureName);
        }
        /// <summary>
        /// Gets Stored Procedure info.
        /// </summary>
        /// <param name="owner">Owner of Stored Procedure.</param>
        /// <param name="procedureName">Stored Procedure Name.</param>
        /// <returns>Returns Stored procedure information.</returns>
        public NDbProcedureInfo GetProcedureInfo(string owner, string procedureName)
        {
            if (null != this.Factory)
            {
                _onExecuting = true; // Mark executing flag
                NDbProcedureInfo result = this.Factory.GetProcedureInfo(
                    this.DbConnection, owner, procedureName);
                _onExecuting = false; // Mark executing flag
                return result;
            }
            return null;
        }

        #endregion

        #region Get Columns

        /// <summary>
        /// Get Columns by table's name.
        /// </summary>
        /// <param name="tableName">The table or view name.</param>
        /// <returns>Returns list of columns.</returns>
        public List<NDbColumn> GetTableColumns(string tableName)
        {
            return GetTableColumns(tableName, null);
        }
        /// <summary>
        /// Get Columns by table's name.
        /// </summary>
        /// <param name="tableName">The table or view name.</param>
        /// <param name="transaction">Transaction instance.</param>
        /// <returns>Returns list of columns.</returns>
        public List<NDbColumn> GetTableColumns(string tableName, DbTransaction transaction)
        {
            string queryText = string.Empty;
            string fmtTableName = FormatTableName(tableName);
            if (string.IsNullOrWhiteSpace(fmtTableName))
            {
                queryText += "SELECT * FROM " + fmtTableName;
                return GetQueryColumns(queryText, transaction);
            }
            else return null;
        }
        /// <summary>
        /// Get Columns by specificed query.
        /// </summary>
        /// <param name="queryText">The query text.</param>
        /// <returns>Returns list of columns.</returns>
        public List<NDbColumn> GetQueryColumns(string queryText)
        {
            return GetQueryColumns(queryText, null);
        }
        /// <summary>
        /// Get Columns by specificed query
        /// </summary>
        /// <param name="queryText">The query text.</param>
        /// <param name="transaction">Transaction instance.</param>
        /// <returns>Returns list of columns.</returns>
        public List<NDbColumn> GetQueryColumns(string queryText, DbTransaction transaction)
        {
            DataTable table = GetSchemaTable(queryText, transaction);
            if (null == table) return null;
            List<NDbColumn> results = new List<NDbColumn>();
            foreach (DataRow row in table.Rows)
            {
                NDbColumn result = new NDbColumn();

                #region Assigned Data

                #region string data

                if (table.Columns.Contains("ColumnName"))
                {
                    if (row["ColumnName"] == DBNull.Value)
                        result.ColumnName = "";
                    else result.ColumnName = row["ColumnName"].ToString().Trim();
                }

                if (table.Columns.Contains("BaseServerName"))
                {
                    if (row["BaseServerName"] == DBNull.Value)
                        result.BaseServerName = "";
                    else result.BaseServerName = row["BaseServerName"].ToString().Trim();
                }

                if (table.Columns.Contains("BaseCatalogName"))
                {
                    if (row["BaseCatalogName"] == DBNull.Value)
                        result.BaseCatalogName = "";
                    else result.BaseCatalogName = row["BaseCatalogName"].ToString().Trim();
                }

                if (table.Columns.Contains("BaseSchemaName"))
                {
                    if (row["BaseSchemaName"] == DBNull.Value)
                        result.BaseSchemaName = "";
                    else result.BaseSchemaName = row["BaseSchemaName"].ToString().Trim();
                }

                if (table.Columns.Contains("BaseTableName"))
                {
                    if (row["BaseTableName"] == DBNull.Value)
                        result.BaseTableName = "";
                    else result.BaseTableName = row["BaseTableName"].ToString().Trim();
                }

                if (table.Columns.Contains("BaseColumnName"))
                {
                    if (row["BaseColumnName"] == DBNull.Value)
                        result.BaseColumnName = "";
                    else result.BaseColumnName = row["BaseColumnName"].ToString().Trim();
                }

                #endregion

                #region Integer data

                if (table.Columns.Contains("ColumnOrdinal"))
                {
                    if (!Utils.IsInteger(row["ColumnOrdinal"].ToString()))
                        result.Ordinal = 0;
                    else result.Ordinal = int.Parse(row["ColumnOrdinal"].ToString().Trim());
                }

                if (table.Columns.Contains("ColumnSize"))
                {
                    if (!Utils.IsInteger(row["ColumnSize"].ToString()))
                        result.ColumnSize = 0;
                    else result.ColumnSize = int.Parse(row["ColumnSize"].ToString().Trim());
                }

                if (table.Columns.Contains("NumericPrecision"))
                {
                    if (!Utils.IsInteger(row["NumericPrecision"].ToString()))
                        result.NumericPrecision = 0;
                    else result.NumericPrecision = int.Parse(row["NumericPrecision"].ToString().Trim());
                }

                if (table.Columns.Contains("NumericScale"))
                {
                    if (!Utils.IsInteger(row["NumericScale"].ToString()))
                        result.NumericScale = 0;
                    else result.NumericScale = int.Parse(row["NumericScale"].ToString().Trim());
                }

                #endregion

                #region bool data

                if (table.Columns.Contains("IsUnique"))
                {
                    if (!Utils.IsBool(row["IsUnique"].ToString()))
                        result.IsUnique = false;
                    else result.IsUnique = bool.Parse(row["IsUnique"].ToString().Trim());
                }

                if (table.Columns.Contains("IsKey"))
                {
                    if (!Utils.IsBool(row["IsKey"].ToString()))
                        result.IsKey = false;
                    else result.IsKey = bool.Parse(row["IsKey"].ToString().Trim());
                }

                if (table.Columns.Contains("AllowDbNull"))
                {
                    if (!Utils.IsBool(row["AllowDbNull"].ToString()))
                        result.AllowDbNull = true;
                    else result.AllowDbNull = bool.Parse(row["AllowDbNull"].ToString().Trim());
                }

                if (table.Columns.Contains("IsAliased"))
                {
                    if (!Utils.IsBool(row["IsAliased"].ToString()))
                        result.IsAliased = false;
                    else result.IsAliased = bool.Parse(row["IsAliased"].ToString().Trim());
                }

                if (table.Columns.Contains("IsExpression"))
                {
                    if (!Utils.IsBool(row["IsExpression"].ToString()))
                        result.IsExpression = false;
                    else result.IsExpression = bool.Parse(row["IsExpression"].ToString().Trim());
                }

                if (table.Columns.Contains("IsIdentity"))
                {
                    if (!Utils.IsBool(row["IsIdentity"].ToString()))
                        result.IsIdentity = false;
                    else result.IsIdentity = bool.Parse(row["IsIdentity"].ToString().Trim());
                }

                if (table.Columns.Contains("IsAutoIncrement"))
                {
                    if (!Utils.IsBool(row["IsAutoIncrement"].ToString()))
                        result.IsAutoIncrement = false;
                    else result.IsAutoIncrement = bool.Parse(row["IsAutoIncrement"].ToString().Trim());
                }

                if (table.Columns.Contains("IsRowVersion"))
                {
                    if (!Utils.IsBool(row["IsRowVersion"].ToString()))
                        result.IsRowVersion = false;
                    else result.IsRowVersion = bool.Parse(row["IsRowVersion"].ToString().Trim());
                }

                if (table.Columns.Contains("IsHidden"))
                {
                    if (!Utils.IsBool(row["IsHidden"].ToString()))
                        result.IsHidden = false;
                    else result.IsHidden = bool.Parse(row["IsHidden"].ToString().Trim());
                }

                if (table.Columns.Contains("IsLong"))
                {
                    if (!Utils.IsBool(row["IsLong"].ToString()))
                        result.IsLong = false;
                    else result.IsLong = bool.Parse(row["IsLong"].ToString().Trim());
                }

                if (table.Columns.Contains("IsReadOnly"))
                {
                    if (!Utils.IsBool(row["IsReadOnly"].ToString()))
                        result.IsReadOnly = false;
                    else result.IsReadOnly = bool.Parse(row["IsReadOnly"].ToString().Trim());
                }

                #endregion

                #region ProviderType and DataType

                if (table.Columns.Contains("ProviderType"))
                {
                    if (!Utils.IsInteger(row["ProviderType"].ToString()))
                        result.ProviderTypeID = 0;
                    else result.ProviderTypeID = int.Parse(row["ProviderType"].ToString().Trim());
                }

                if (table.Columns.Contains("DataType"))
                {
                    if (row["DataType"] == DBNull.Value)
                        result.DataTypeName = "";
                    else result.DataTypeName = row["DataType"].ToString().Trim();
                }

                #endregion

                #endregion

                result.Lock(); // lock
                results.Add(result);
            }

            #region Release table

            if (null != table)
            {
                try
                {
                    table.Dispose();
                    // Free Memory
                    NGC.FreeGC(table);
                }
                catch { }
            }
            table = null;

            #endregion

            return results;
        }

        #endregion

        #endregion
    }

    #endregion
}

namespace NLib.Components
{
    #region NDbConnection (Constructor/Destructor)
    
    /// <summary>
    /// The NDbConnection aka. Connection Manager.
    /// </summary>
    public partial class NDbConnection : Component
    {
        #region Constructor and Destructor
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public NDbConnection() : base()
        {

        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~NDbConnection()
        {
            Disconnect();

            FreeRecordsets();

            ReleaseTimer(); // release timer
            UnhookConfigEvents();

            _config = null;
            // Force free memory
            NGC.FreeGC(this);
        }

        #endregion
    }

    #endregion
}