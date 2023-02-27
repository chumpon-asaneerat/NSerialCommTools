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
