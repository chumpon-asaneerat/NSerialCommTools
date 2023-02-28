#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Windows.Threading;
using System.Linq.Expressions;

#endregion

namespace NLib.Serial
{
    #region SerialPortConfig

    /// <summary>
    /// The SerialPortConfig class.
    /// </summary>
    public class SerialPortConfig
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialPortConfig() 
        {
            this.PortName = "COM1";
            this.BaudRate = 9600;
            this.Parity = Parity.None;
            this.StopBits = StopBits.One;
            this.Handshake = Handshake.None;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Port Name (i.e. COM1, COM2,... ).
        /// </summary>
        public string PortName { get; set; }
        /// <summary>
        /// Gets or sets Boad Rate (default 9600).
        /// </summary>
        public int BaudRate { get; set; }
        /// <summary>
        /// Gets or sets Parity (default None).
        /// </summary>
        public Parity Parity { get; set; }
        /// <summary>
        /// Gets or sets StopBits (default One).
        /// </summary>
        public StopBits StopBits { get; set; }
        /// <summary>
        /// Gets or sets DataBits (default 8).
        /// </summary>
        public int DataBits { get; set; }
        /// <summary>
        /// Gets or sets Handshake (default None).
        /// </summary>
        public Handshake Handshake { get; set; }

        #endregion
    }

    #endregion

    #region Delegate Helper

    /// <summary>
    /// The Delegate Helper. This class contains static method to invoke delegate without concern about
    /// cross thread problem.
    /// </summary>
    internal static class DelegateHelper
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

                    PropertyInfo prop = del.Target.GetType().GetProperty("Dispatcher");
                    if (null != prop)
                    {
                        dispatcher = prop.GetValue(del.Target, null) as Dispatcher;
                    }
                    //dispatcher = DynamicAccess.Get(del.Target, "Dispatcher") as Dispatcher;

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

    #endregion

    #region ByteArray Helper

    /// <summary>
    /// The Byte Array Helper class.
    /// </summary>
    public class ByteArrayHelper
    {
        #region ToHexString

        public static string ToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace('-', ' ');
        }

        #endregion
    }

    #endregion

    #region SerialDevice

    /// <summary>
    /// The SerialDevice class (abstract).
    /// </summary>
    public abstract class SerialDevice
    {
        #region Internal Variables

        private bool _isExit = false;

        private SerialPort _comm = null;

        private bool _isProcessing = false;
        private Thread _th = null;

        private object _lock = new object();
        private List<byte> queues = new List<byte>();

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialDevice() : base() 
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~SerialDevice()
        {
            ClosePort();
            AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
        }

        #endregion

        #region Private Methods

        #region App Domain Event Handlers

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            _isExit = true;
        }

        private void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            _isExit = true;
        }

        #endregion

        #region NLib Wrapper

        protected void DoEvents()
        {
            DelegateHelper.DoEvents();
        }

        protected void Log(MethodBase med, string messsage)
        {
            // Write log
        }

        protected void Error(MethodBase med, string messsage)
        {
            // Write error
        }

        protected void Invoke(Delegate method, params object[] args)
        {
            DelegateHelper.Invoke(method, args);
        }

        #endregion

        #region Thread Related Methods

        private void CreateRXThread()
        {
            if (null != _th) return; // thread instance exist.

            MethodBase med = MethodBase.GetCurrentMethod();
            Log(med, "Creeate RX Thread");

            _th = new Thread(this.RxProcessing);
            _th.Name = "Serial RX thread";
            _th.IsBackground = true;
            _th.Priority = ThreadPriority.BelowNormal;

            _isProcessing = true; // mark flag.
            
            _th.Start();
        }

        private void FreeRXThread()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            Log(med, "Free RX Thread");

            _isProcessing = false; // mark flag.

            if (null == _th) return; // no thread instance.
            try
            {
                _th.Abort();
            }
            catch (ThreadAbortException) 
            { 
                Thread.ResetAbort(); 
            }
            _th = null;
        }

        private void RxProcessing()
        {
            while (null != _th && _isProcessing && !_isExit)
            {
                try
                {
                    if (null != _comm)
                    {
                        int byteToRead = _comm.BytesToRead;
                        if (byteToRead > 0)
                        {
                            lock (_lock)
                            {
                                byte[] buffers = new byte[byteToRead];
                                _comm.Read(buffers, 0, byteToRead);
                                queues.AddRange(buffers);
                            }
                            // process rx queue in main ui thread.
                            Invoke(new Action(() => 
                            { 
                                ProcessRXQueue();
                                // Raise event.
                                Invoke(OnRx, this, EventArgs.Empty);
                            }));
                        }
                    }
                }
                catch (TimeoutException) { }
                catch (Exception) { }

                Thread.Sleep(50);
                DoEvents();
            }
            FreeRXThread();
        }

        #endregion

        #endregion

        #region Protected Methods

        #region Open/Close port

        /// <summary>
        /// OpenPort.
        /// </summary>
        /// <param name="value">The Serial Port Config instance.</param>
        protected void OpenPort(SerialPortConfig value)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            if (null == value)
            {
                Error(med, "The SerialPortConfig is null.");
                return;
            }
            // Open Port.
            OpenPort(value.PortName, value.BaudRate, 
                value.Parity, value.DataBits, value.StopBits, 
                value.Handshake);
        }
        /// <summary>
        /// Open Serial Port connection.
        /// </summary>
        /// <param name="portName">The port name. i.e. COM1, COM2,...</param>
        /// <param name="boadRate">The boad rate. default 9600.</param>
        /// <param name="parity">The parity. default None.</param>
        /// <param name="dataBits">The data bits. default 8.</param>
        /// <param name="stopBits">The stop bits. default One.</param>
        /// <param name="handshake">The handshake. default None.</param>
        protected void OpenPort(string portName, int boadRate = 9600, 
            Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One,
            Handshake handshake = Handshake.None) 
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            if (null != _comm)
                return; // already connected.

            #region Set port

            try
            {
                _comm = new SerialPort();
                _comm.PortName = portName;
                _comm.BaudRate = boadRate;
                _comm.Parity = parity;
                _comm.DataBits = dataBits;
                _comm.StopBits = stopBits;
                _comm.Handshake = handshake;
            }
            catch (Exception ex)
            {
                Error(med, ex.ToString());
            }

            #endregion

            #region Open port

            // Open Port
            string msg = string.Format("Open port {0} - {1}, {2}, {3}, {4}",
                _comm.PortName, _comm.BaudRate, _comm.Parity, _comm.DataBits, _comm.StopBits);
            Log(med, msg);

            try
            {
                _comm.Open();
            }
            catch (Exception ex2)
            {
                Error(med, ex2.ToString());
            }

            #endregion

            #region Check port opened

            if (null != _comm && !_comm.IsOpen)
            {
                Error(med, "Cannot open port. Free resource.");
                try
                {
                    _comm.Close();
                    _comm.Dispose();
                }
                catch { }
                _comm = null;

                return; // cannot open port
            }
            // Port opened so Create Read Thread
            CreateRXThread();

            #endregion
        }
        /// <summary>
        /// Close Serial Port connection.
        /// </summary>
        protected void ClosePort()
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            FreeRXThread();
            // Free Serial ports
            if (null != _comm)
            {
                string msg = string.Format("Close port {0}", _comm.PortName);
                Log(med, msg);

                try
                {
                    _comm.Close();
                    _comm.Dispose();
                }
                catch { }
            }
            _comm = null;
        }

        #endregion

        #region RX Data processing

        /// <summary>
        /// Gets RX Queues.
        /// </summary>
        public List<byte> Queues { get { return queues; } }
        /// <summary>
        /// Process RX Queue.
        /// </summary>
        protected abstract void ProcessRXQueue();

        #endregion

        #endregion

        #region Protected Proeprties

        /// <summary>
        /// Checks is AppDomain is unloaded or Process is exit.
        /// </summary>
        protected bool IsExit { get { return _isExit; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Send.
        /// </summary>
        /// <param name="data">The data buffer to send.</param>
        public void Send(byte[] data)
        {
            if (null != _comm && !_comm.IsOpen)
                return;
            if (null == data || data.Length <= 0)
                return;
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                _comm.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Error(med, ex.ToString());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Checks is thread still processing.
        /// </summary>
        public bool IsProcessing { get { return (null != _th && _isProcessing); } }
        /// <summary>
        /// Checks is port opened.
        /// </summary>
        public bool IsOpen { get { return (null != _comm && _comm.IsOpen); } }

        #endregion

        #region Public Events

        /// <summary>
        /// The OnRx event handler.
        /// </summary>
        public event EventHandler OnRx;

        #endregion
    }

    #endregion

    #region SerialDeviceData

    /// <summary>
    /// The SerialDeviceData class.
    /// </summary>
    public abstract class SerialDeviceData : INotifyPropertyChanged
    {
        #region Consts

        public class ascii
        {
            public static string x0D = "\x0D";
            public static string x0A = "\x0A";
        }

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialDeviceData() : base() { }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~SerialDeviceData()
        {

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Internal Raise Property Changed event (Lamda function).
        /// </summary>
        /// <param name="selectorExpression">The Expression function.</param>

        private void InternalRaise<T>(Expression<Func<T>> selectorExpression)
        {
            if (null == selectorExpression)
            {
                throw new ArgumentNullException("selectorExpression");
                // return;
            }
            var me = selectorExpression.Body as MemberExpression;

            // Nullable properties can be nested inside of a convert function
            if (null == me)
            {
                var ue = selectorExpression.Body as UnaryExpression;
                if (null != ue)
                {
                    me = ue.Operand as MemberExpression;
                }
            }

            if (null == me)
            {
                throw new ArgumentException("The body must be a member expression");
                // return;
            }
            Raise(me.Member.Name);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raise Property Changed event.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        protected void Raise(string propertyName)
        {
            // raise event.
            if (null != PropertyChanged)
            {
                DelegateHelper.Invoke(PropertyChanged, this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /// <summary>
        /// Raise Property Changed event (Lamda function).
        /// </summary>
        /// <param name="actions">The array of lamda expression's functions.</param>
        protected void Raise(params Expression<Func<object>>[] actions)
        {
            if (null != actions && actions.Length > 0)
            {
                foreach (var item in actions)
                {
                    if (null != item) InternalRaise(item);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Convert content to byte array.
        /// </summary>
        /// <returns>Returns content in byte array.</returns>
        public abstract byte[] ToByteArray();
        /// <summary>
        /// Parse byte array and update content.
        /// </summary>
        /// <param name="buffers">The buffer data.</param>
        public abstract void Parse(byte[] buffers);

        #endregion

        #region Public Events

        /// <summary>
        /// The PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    #endregion

    #region SerialDeviceEmulator

    /// <summary>
    /// The SerialDeviceEmulator class (abstract).
    /// </summary>
    public abstract class SerialDeviceEmulator<T> : SerialDevice
        where T : SerialDeviceData, new()
    {
        #region Internal Variables

        private SerialPortConfig _config = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialDeviceEmulator() : base() 
        {
            _config = new SerialPortConfig();
            Value = new T();
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~SerialDeviceEmulator()
        {
            Shutdown();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start.
        /// </summary>
        public void Start() 
        {
            if (null == _config)
                _config = new SerialPortConfig();
            OpenPort(_config);
        }
        /// <summary>
        /// Shutdown.
        /// </summary>
        public void Shutdown() 
        {
            ClosePort();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Serial Port Config.
        /// </summary>
        public SerialPortConfig Config 
        {
            get
            {
                if (null == _config) _config = new SerialPortConfig();
                return _config;
            }
            set { _config = value; }  
        }
        /// <summary>
        /// Gets or sets content value.
        /// </summary>
        public T Value { get; set; }

        #endregion
    }

    #endregion

    #region SerialDeviceTerminal

    /// <summary>
    /// The SerialDeviceTerminal class (abstract).
    /// </summary>
    public abstract class SerialDeviceTerminal<T> : SerialDevice
        where T : SerialDeviceData, new()
    {
        #region Internal Variables

        private SerialPortConfig _config = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialDeviceTerminal() : base() 
        {
            _config = new SerialPortConfig();
            Value = new T();
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~SerialDeviceTerminal()
        {
            Disconnect();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Connect.
        /// </summary>
        public void Connect() 
        {
            if (null == _config) 
                _config = new SerialPortConfig();
            OpenPort(_config);
        }
        /// <summary>
        /// Disconnect.
        /// </summary>
        public void Disconnect() 
        {
            ClosePort();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Serial Port Config.
        /// </summary>
        public SerialPortConfig Config
        {
            get 
            { 
                if (null == _config) _config = new SerialPortConfig();
                return _config; 
            }
            set { _config = value; }
        }
        /// <summary>
        /// Gets or sets content value.
        /// </summary>
        public T Value { get; set; }

        #endregion
    }

    #endregion
}
