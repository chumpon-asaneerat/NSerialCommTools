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
    public class WeightQAData : SerialDeviceData
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
    #region WeightQADevice

    /// <summary>
    /// The WeightQADevice class.
    /// </summary>
    public class WeightQADevice : SerialDeviceEmulator<WeightQAData>
    {
        #region Singelton Access

        private static WeightQADevice _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static WeightQADevice Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(WeightQADevice))
                    {
                        _instance = new WeightQADevice();
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
        private WeightQADevice()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~WeightQADevice()
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
    #region WeightQATerminal

    /// <summary>
    /// The WeightQATerminal class.
    /// </summary>
    public class WeightQATerminal : SerialDeviceTerminal<WeightQAData>
    {
        #region Singelton Access

        private static WeightQATerminal _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static WeightQATerminal Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(WeightQATerminal))
                    {
                        _instance = new WeightQATerminal();
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
        private WeightQATerminal()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~WeightQATerminal()
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
