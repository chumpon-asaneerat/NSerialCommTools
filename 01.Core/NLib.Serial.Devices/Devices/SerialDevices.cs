#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Globalization;
using System.Threading;

#endregion

namespace NLib.Serial.Devices
{
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


    #region SerialDevice

    /// <summary>
    /// The SerialDevice class (abstract).
    /// </summary>
    public abstract class SerialDevice
    {
        #region Internal Variables

        private bool _isExit = false;

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

        #endregion

        #region Protected Methods

        /// <summary>
        /// Open Serial Port connection.
        /// </summary>
        protected void OpenPort() 
        { 

        }
        /// <summary>
        /// Close Serial Port connection.
        /// </summary>
        protected void ClosePort()
        {

        }

        #endregion

        #region Protected Proeprties

        /// <summary>
        /// Checks is AppDomain is unloaded or Process is exit.
        /// </summary>
        protected bool IsExit { get { return _isExit; } }

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
