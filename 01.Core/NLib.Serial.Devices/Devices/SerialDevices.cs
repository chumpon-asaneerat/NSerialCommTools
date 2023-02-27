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

#endregion

namespace NLib.Serial.Devices
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

        private void DoEvents()
        {
            Application.DoEvents();
        }

        private void Log(MethodBase med, string messsage)
        {
            // Write log
        }

        private void Error(MethodBase med, string messsage)
        {
            // Write error
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
                            // process rx queue.
                            ProcessRXQueue();
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

        protected void ProcessRXQueue()
        {

        }

        #endregion

        #endregion

        #region Protected Proeprties

        /// <summary>
        /// Checks is AppDomain is unloaded or Process is exit.
        /// </summary>
        protected bool IsExit { get { return _isExit; } }

        #endregion

        #region Public Properties

        /// <summary>
        /// Checks is thread still processing.
        /// </summary>
        public bool IsProcessing { get { return (null != _th && _isProcessing); } }

        #endregion
    }

    #endregion

    #region SerialDeviceEmulator

    /// <summary>
    /// The SerialDeviceEmulator class (abstract).
    /// </summary>
    public abstract class SerialDeviceEmulator : SerialDevice
    {
        #region Internal Variables

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialDeviceEmulator() : base() { }
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
        }
        /// <summary>
        /// Shutdown.
        /// </summary>
        public void Shutdown() 
        {
            ClosePort();
        }

        #endregion
    }

    #endregion

    #region SerialDeviceTerminal

    /// <summary>
    /// The SerialDeviceTerminal class (abstract).
    /// </summary>
    public abstract class SerialDeviceTerminal : SerialDevice
    {
        #region Internal Variables

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialDeviceTerminal() : base() { }
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
        }
        /// <summary>
        /// Disconnect.
        /// </summary>
        public void Disconnect() 
        {
            ClosePort();
        }

        #endregion
    }

    #endregion
}
