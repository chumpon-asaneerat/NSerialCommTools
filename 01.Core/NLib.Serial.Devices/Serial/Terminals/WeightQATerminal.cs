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
    #region WeightQATerminal

    /// <summary>
    /// The WeightQATerminal class.
    /// </summary>
    public class WeightQATerminal : SerialDeviceTerminal
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
