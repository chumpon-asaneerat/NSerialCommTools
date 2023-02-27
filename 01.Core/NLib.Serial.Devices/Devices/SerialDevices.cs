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

                Thread.Sleep(50);
                DoEvents();
            }
            FreeRXThread();
        }

        #endregion

        #endregion

        #region Protected Methods

        /// <summary>
        /// Open Serial Port connection.
        /// </summary>
        protected void OpenPort() 
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            if (null != _comm)
                return; // already connected.
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
