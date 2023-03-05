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

        #region Static Methods

        /// <summary>
        /// Gets avaliable port names.
        /// </summary>
        /// <returns>Returns avaliable port names</returns>
        public static List<string> GetPortNames()
        {
            var rets = new List<string>();
            foreach (string s in SerialPort.GetPortNames())
            {
                rets.Add(s);
            }
            return rets;
        }
        /// <summary>
        /// Gets avaliable supports parities.
        /// </summary>
        /// <returns>Returns supports parities</returns>
        public static List<string> GetParities()
        {
            var rets = new List<string>();
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                rets.Add(s);
            }
            return rets;
        }
        /// Gets Parity from string.
        /// </summary>
        /// <param name="value">The Parity value in string.</param>
        /// <returns>Returns Parity value.</returns>
        public static Parity GetParity(string value)
        {
            return (Parity)Enum.Parse(typeof(Parity), value, true);
        }
        /// <summary>
        /// Gets avaliable supports stop bits.
        /// </summary>
        /// <returns>Returns supports stop bits</returns>
        public static List<string> GetStopBits()
        {
            var rets = new List<string>();
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                rets.Add(s);
            }
            return rets;
        }
        /// <summary>
        /// Gets StopBits from string.
        /// </summary>
        /// <param name="value">The StopBits value in string.</param>
        /// <returns>Returns StopBits value.</returns>
        public static StopBits GetStopBits(string value)
        {
            return (StopBits)Enum.Parse(typeof(StopBits), value, true);
        }
        /// <summary>
        /// Gets avaliable supports handshakes.
        /// </summary>
        /// <returns>Returns supports handshakes</returns>
        public static List<string> GetHandshakes()
        {
            var rets = new List<string>();
            foreach (string s in Enum.GetNames(typeof(Handshake)))
            {
                rets.Add(s);
            }
            return rets;
        }
        /// <summary>
        /// Gets Handshake from string.
        /// </summary>
        /// <param name="value">The handshake value in string.</param>
        /// <returns>Returns Handshake value.</returns>
        public static Handshake GetHandshake(string value)
        {
            return (Handshake)Enum.Parse(typeof(Handshake), value, true);
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

        private SerialPortConfig _config = null;

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
            _config = new SerialPortConfig();
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~SerialDevice()
        {
            ClosePort();
        }

        #endregion

        #region Thread Related Methods

        private void CreateRXThread()
        {
            if (null != _th) return; // thread instance exist.

            MethodBase med = MethodBase.GetCurrentMethod();
            med.Info("Creeate RX Thread");

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
            med.Info("Free RX Thread");

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
            while (null != _th && _isProcessing && !ApplicationManager.Instance.IsExit)
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
                            ProcessRXQueue();
                            // Raise event.
                            OnRx.Call(this, EventArgs.Empty);
                        }
                    }
                }
                catch (TimeoutException) { }
                catch (Exception) { }

                Thread.Sleep(50);
                Application.DoEvents();
            }
            FreeRXThread();
        }

        #endregion

        #region Protected Methods

        #region Open/Close port

        /// <summary>
        /// OpenPort.
        /// </summary>
        protected void OpenPort()
        {
            if (null == _config)
                _config = new SerialPortConfig();
            OpenPort(_config);
        }
        /// <summary>
        /// OpenPort.
        /// </summary>
        /// <param name="value">The Serial Port Config instance.</param>
        protected void OpenPort(SerialPortConfig value)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            if (null == value)
            {
                med.Err("The SerialPortConfig is null.");
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
                med.Err(med, ex);
            }

            #endregion

            #region Open port

            // Open Port
            string msg = string.Format("Open port {0} - {1}, {2}, {3}, {4}",
                _comm.PortName, _comm.BaudRate, _comm.Parity, _comm.DataBits, _comm.StopBits);
            med.Info(msg);

            try
            {
                _comm.Open();
            }
            catch (Exception ex2)
            {
                med.Err(ex2.ToString());
            }

            #endregion

            #region Check port opened

            if (null != _comm && !_comm.IsOpen)
            {
                med.Info("Cannot open port. Free resource.");
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
                med.Info(msg);

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
                med.Err(med, ex);
            }
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
                PropertyChanged.Call(this, new PropertyChangedEventArgs(propertyName));
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
        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialDeviceEmulator() : base() 
        {
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
            OpenPort();
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
        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialDeviceTerminal() : base() 
        {
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
            OpenPort();
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
        /// Gets or sets content value.
        /// </summary>
        public T Value { get; set; }

        #endregion
    }

    #endregion
}
