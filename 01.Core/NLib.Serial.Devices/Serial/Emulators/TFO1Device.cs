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
    #region TFO1Device

    /// <summary>
    /// The TFO1Device class.
    /// </summary>
    public class TFO1Device : SerialDeviceEmulator<TFO1Data>
    {
        #region Singelton Access

        private static TFO1Device _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static TFO1Device Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(TFO1Device))
                    {
                        _instance = new TFO1Device();
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
        private TFO1Device()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~TFO1Device()
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
