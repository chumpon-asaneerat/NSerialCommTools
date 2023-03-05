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
using NLib.Serial.Devices;

#endregion

namespace NLib.Serial.Devices
{
    #region PHMeterData

    /// <summary>
    /// The PHMeterData class.
    /// </summary>
    public class PHMeterData : SerialDeviceData
    {
        #region Override Methods

        /// <summary>
        /// Convert content to byte array.
        /// </summary>
        /// <returns>Returns content in byte array.</returns>
        public override byte[] ToByteArray()
        {
            return null;
        }
        /// <summary>
        /// Parse byte array and update content.
        /// </summary>
        /// <param name="buffers">The buffer data.</param>
        public override void Parse(byte[] buffers)
        {

        }

        #endregion
    }

    #endregion
}

namespace NLib.Serial.Emulators
{
    #region PHMeterDevice

    /// <summary>
    /// The PHMeterDevice class.
    /// </summary>
    public class PHMeterDevice : SerialDeviceEmulator<PHMeterData>
    {
        #region Singelton Access

        private static PHMeterDevice _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static PHMeterDevice Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(PHMeterDevice))
                    {
                        _instance = new PHMeterDevice();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private PHMeterDevice()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~PHMeterDevice()
        {
            Shutdown();
        }

        #endregion

        #region Onverride method(s)

        /// <summary>
        /// ProcessRXQueue.
        /// </summary>
        protected override void ProcessRXQueue()
        {

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Device Name.
        /// </summary>
        public override string DeviceName { get { return "PHMeter"; } }

        #endregion
    }

    #endregion
}

namespace NLib.Serial.Terminals
{
    #region PHMeterTerminal

    /// <summary>
    /// The PHMeterTerminal class.
    /// </summary>
    public class PHMeterTerminal : SerialDeviceTerminal<PHMeterData>
    {
        #region Singelton Access

        private static PHMeterTerminal _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static PHMeterTerminal Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(PHMeterTerminal))
                    {
                        _instance = new PHMeterTerminal();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private PHMeterTerminal()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~PHMeterTerminal()
        {
            Disconnect();
        }

        #endregion

        #region Onverride method(s)

        /// <summary>
        /// ProcessRXQueue.
        /// </summary>
        protected override void ProcessRXQueue()
        {

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Device Name.
        /// </summary>
        public override string DeviceName { get { return "PHMeter"; } }

        #endregion
    }

    #endregion
}
