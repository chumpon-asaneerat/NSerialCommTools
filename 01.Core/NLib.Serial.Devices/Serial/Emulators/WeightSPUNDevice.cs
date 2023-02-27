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
