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
    #region PHMeterDevice

    /// <summary>
    /// The PHMeterDevice class.
    /// </summary>
    public class PHMeterDevice : SerialDeviceEmulator
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
    }

    #endregion
}
