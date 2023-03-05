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
    #region WeightQAData

    /// <summary>
    /// The WeightQAData class.
    /// </summary>
    public class WeightSPUNData : SerialDeviceData
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
    #region WeightSPUNDevice

    /// <summary>
    /// The WeightSPUNDevice class.
    /// </summary>
    public class WeightSPUNDevice : SerialDeviceEmulator<WeightSPUNData>
    {
        #region Singelton Access

        private static WeightSPUNDevice _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static WeightSPUNDevice Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(WeightSPUNDevice))
                    {
                        _instance = new WeightSPUNDevice();
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
        private WeightSPUNDevice()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~WeightSPUNDevice()
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
    }

    #endregion
}

namespace NLib.Serial.Terminals
{
    #region WeightSPUNTerminal

    /// <summary>
    /// The WeightSPUNTerminal class.
    /// </summary>
    public class WeightSPUNTerminal : SerialDeviceTerminal<WeightSPUNData>
    {
        #region Singelton Access

        private static WeightSPUNTerminal _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static WeightSPUNTerminal Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(WeightSPUNTerminal))
                    {
                        _instance = new WeightSPUNTerminal();
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
        private WeightSPUNTerminal()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~WeightSPUNTerminal()
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
    }

    #endregion
}
