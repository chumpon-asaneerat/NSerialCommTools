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

namespace NLib.Serial.Terminals
{
    #region PHMeterTerminal

    /// <summary>
    /// The PHMeterTerminal class.
    /// </summary>
    public class PHMeterTerminal : SerialDeviceTerminal
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
    }

    #endregion
}
