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
