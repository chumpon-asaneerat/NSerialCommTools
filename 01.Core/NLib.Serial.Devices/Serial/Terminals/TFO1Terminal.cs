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
    #region TFO1Terminal

    /// <summary>
    /// The TFO1Terminal class.
    /// </summary>
    public class TFO1Terminal : SerialDeviceTerminal<TFO1Data>
    {
        #region Singelton Access

        private static TFO1Terminal _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static TFO1Terminal Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(TFO1Terminal))
                    {
                        _instance = new TFO1Terminal();
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
        private TFO1Terminal()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~TFO1Terminal()
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
