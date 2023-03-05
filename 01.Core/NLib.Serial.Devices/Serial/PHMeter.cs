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
    #region PHMeterData

    /// <summary>
    /// The PHMeterData class.
    /// </summary>
    public class PHMeterData : SerialDeviceData
    {
        #region Override Methods

        /// <summary>
        /// Convert content to byte array.
        /// </summary>
        /// <returns>Returns content in byte array.</returns>
        public override byte[] ToByteArray()
        {
            return ToByteArray(2);
        }

        public byte[] ToByteArray(int atcCount)
        {
            List<byte> buffers = new List<byte>();
            string output;

            if (atcCount <= 0) atcCount = 1;
            // ATC
            for (int i = 0; i < atcCount; ++i)
            {
                // 3.01pH 25.5.C ATC..
                // 33 2E 30 31 70 48 20 32 35 2E 35 F8 43 20 41 54 43 0D 0A
                output = pH.ToString("F2") + "pH " + TempC.ToString("F1");
                buffers.AddRange(Encoding.ASCII.GetBytes(output));
                buffers.Add(0xF8);
                output = "C ATC" + ascii.x0D + ascii.x0A;
                buffers.AddRange(Encoding.ASCII.GetBytes(output));
            }

            // DATE + NEW LINE
            // 20-Feb-2023..
            // 32 30 2D 46 65 62 2D 32 30 32 33 0D 0A
            output = Date.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo) + ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));

            // TIME + NEW LINE
            // 11:11..
            // 31 31 3A 31 31 0D 0A
            output = Date.ToString("HH:mm", DateTimeFormatInfo.InvariantInfo) + ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));

            // SPACE + NEW LINE
            //  ..
            // 20 0D 0A
            output = " " + ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));

            // PH + NEW LINE
            // 3.01pH..
            // 33 2E 30 31 70 48 0D 0A
            output = pH.ToString("F2") + "pH" + ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));

            // Temp C + ATC + NEW LINE
            // 25.5.C ATC..
            // 32 35 2E 35 F8 43 20 41 54 43 0D 0A
            output = TempC.ToString("F1");
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            buffers.Add(0xF8);
            output = "C ATC" + ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));

            // Auto EP Standard + NEW LINE
            // Auto EP Standard..
            // 41 75 74 6F 20 45 50 20 53 74 61 6E 64 61 72 64 0D 0A
            output = "Auto EP Standard" + ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));

            // Blank + NEW LINE
            // Blank..
            // 42 6C 61 6E 6B 0D 0A
            output = "Blank" + ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));

            // Tripple empty line
            // ..
            // 0D 0A 
            // ..
            // 0D 0A 
            // ..
            // 0D 0A 
            output = ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            buffers.AddRange(Encoding.ASCII.GetBytes(output));

            return buffers.ToArray();
        }
        /// <summary>
        /// Parse byte array and update content.
        /// </summary>
        /// <param name="buffers">The buffer data.</param>
        public override void Parse(byte[] buffers)
        {

        }

        #endregion

        #region Public Properties

        private decimal _pH = decimal.Zero;
        private decimal _TempC = decimal.Zero;
        private DateTime _Date = DateTime.Now;

        public decimal pH 
        {
            get { return _pH; }
            set
            {
                if (_pH != value)
                {
                    _pH = value;
                    Raise(() => this.pH);
                }
            }
        }

        public decimal TempC
        {
            get { return _TempC; }
            set
            {
                if (_TempC != value)
                {
                    _TempC = value;
                    Raise(() => this.TempC);
                }
            }
        }

        public DateTime Date
        {
            get { return _Date; }
            set
            {
                if (_Date != value)
                {
                    _Date = value;
                    Raise(() => this.Date);
                }
            }
        }

        #endregion
    }

    #endregion
}

namespace NLib.Serial.Emulators
{
    #region PHMeterDevice

    /// <summary>
    /// The PHMeterDevice class.
    /// </summary>
    public class PHMeterDevice : SerialDeviceEmulator<PHMeterData>
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

        #region Private Methods

        #endregion

        #region Onverride method(s)

        /// <summary>
        /// ProcessRXQueue.
        /// </summary>
        protected override void ProcessRXQueue()
        {

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Device Name.
        /// </summary>
        public override string DeviceName { get { return "PHMeter"; } }

        #endregion
    }

    #endregion
}

namespace NLib.Serial.Terminals
{
    #region PHMeterTerminal

    /// <summary>
    /// The PHMeterTerminal class.
    /// </summary>
    public class PHMeterTerminal : SerialDeviceTerminal<PHMeterData>
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

        #region Public Properties

        /// <summary>
        /// Gets Device Name.
        /// </summary>
        public override string DeviceName { get { return "PHMeter"; } }

        #endregion
    }

    #endregion
}
