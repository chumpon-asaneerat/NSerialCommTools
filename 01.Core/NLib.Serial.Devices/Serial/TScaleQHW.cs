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
    #region TScaleQHWData

    /// <summary>
    /// The TScaleQHWData class.
    /// </summary>
    public class TScaleQHWData : SerialDeviceData
    {
        #region Internal Variables

        private decimal _W = decimal.Zero;
        private string _Unit = "g";
        private string _Status = "ST";
        private string _Mode = "GS";

        #endregion

        #region Override Methods

        /// <summary>
        /// Convert content to byte array.
        /// </summary>
        /// <returns>Returns content in byte array.</returns>
        public override byte[] ToByteArray()
        {
            List<byte> buffers = new List<byte>();

            string status = string.IsNullOrWhiteSpace(Status) ? "ST" : Status.Trim().ToUpper();
            string mode = string.IsNullOrWhiteSpace(Mode) ? "GS" : Mode.Trim().ToUpper();
            string unit = string.IsNullOrWhiteSpace(Unit) ? "g" : Unit.Trim();

            string output;
            // ST,GS,   245.6 g..
            // 53 54 2C 47 53 2C 20 20 20 32 34 35 2E 36 20 67 0D 0A

            string actualW = ((double)W).ToString("F1");
            output = status + "," + mode + ",";
            output += actualW.PadLeft(8, ' ');
            output += " " + unit;
            output += ascii.x0D + ascii.x0A;
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

        public decimal W
        {
            get { return _W; }
            set
            {
                if (_W != value)
                {
                    _W = value;
                    Raise(() => this.W);
                }
            }
        }

        public string Unit
        {
            get { return _Unit; }
            set
            {
                if (_Unit != value)
                {
                    _Unit = value;
                    Raise(() => this.Unit);
                }
            }
        }

        public string Status
        {
            get { return _Status; }
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    Raise(() => this.Status);
                }
            }
        }

        public string Mode
        {
            get { return _Mode; }
            set
            {
                if (_Mode != value)
                {
                    _Mode = value;
                    Raise(() => this.Mode);
                }
            }
        }

        #endregion
    }

    #endregion
}

namespace NLib.Serial.Emulators
{
    #region TScaleQHWDevice

    /// <summary>
    /// The TScaleQHWDevice class.
    /// </summary>
    public class TScaleQHWDevice : SerialDeviceEmulator<TScaleQHWData>
    {
        #region Singelton Access

        private static TScaleQHWDevice _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static TScaleQHWDevice Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(TScaleQHWDevice))
                    {
                        _instance = new TScaleQHWDevice();
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
        private TScaleQHWDevice()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~TScaleQHWDevice()
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

        #region Public Properties

        /// <summary>
        /// Gets Device Name.
        /// </summary>
        public override string DeviceName { get { return "TScaleQHW"; } }

        #endregion
    }

    #endregion
}

namespace NLib.Serial.Terminals
{
    #region TScaleQHWTerminal

    /// <summary>
    /// The TScaleQHWTerminal class.
    /// </summary>
    public class TScaleQHWTerminal : SerialDeviceTerminal<TScaleQHWData>
    {
        #region Singelton Access

        private static TScaleQHWTerminal _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static TScaleQHWTerminal Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(TScaleQHWTerminal))
                    {
                        _instance = new TScaleQHWTerminal();
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
        private TScaleQHWTerminal()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~TScaleQHWTerminal()
        {
            Disconnect();
        }

        #endregion

        #region Private Methods

        private byte[] ExtractPackage()
        {
            byte[] rawPackages = null;

            if (null == this.Queues || this.Queues.Count <= 0)
                return rawPackages;

            byte[] buffers;
            byte[] endPatterns = new byte[] { 0x0D, 0x0A };

            if (null == endPatterns || endPatterns.Length <= 0)
                return rawPackages;

            lock (_lock)
            {
                // create temp buffer.
                buffers = this.Queues.ToArray();
            }

            int idx = this.IndexOf(buffers, endPatterns);

            if (idx != -1)
            {
                // calc length.
                int len = idx + endPatterns.Length;
                // prepare array size
                rawPackages = new byte[len];
                // copy data
                Array.Copy(buffers, rawPackages, len);

                lock (_lock)
                {
                    // remove extract data from queue.
                    this.Queues.RemoveRange(0, len);
                }
            }

            return rawPackages;
        }

        private void UpdateValues(byte[][] contents)
        {
            if (null == contents || contents.Length <= 0)
                return;
            int len = contents.Length;
            for (int i = 0; i < len; i++)
            {
                UpdateValue(contents[i]);
            }
        }

        private void UpdateValue(byte[] content)
        {
            if (null == content || content.Length <= 0)
                return;
            MethodBase med = MethodBase.GetCurrentMethod();

            string line = Encoding.ASCII.GetString(content);
            if (string.IsNullOrEmpty(line))
                return;

            // Protocol: ST,GS,   245.6 g\r\n
            // Format: STATUS,MODE, WEIGHT UNIT
            // Example: ST,GS,   245.6 g

            try
            {
                // Split by comma to separate STATUS, MODE, and rest
                string[] parts = line.Split(new char[] { ',' }, StringSplitOptions.None);
                if (null == parts || parts.Length < 3) return;

                string status = parts[0].Trim();
                string mode = parts[1].Trim();

                // The third part contains: "   245.6 g"
                string remainder = parts[2].Trim();

                // Split by space to separate WEIGHT and UNIT
                string[] spaceParts = remainder.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (null == spaceParts || spaceParts.Length < 2) return;

                string weightStr = spaceParts[0].Trim();
                string unit = spaceParts[1].Trim();

                // Update values
                Value.Status = status;
                Value.Mode = mode;
                Value.Unit = unit;

                if (!string.IsNullOrEmpty(weightStr))
                {
                    Value.W = decimal.Parse(weightStr, CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                med.Err(ex);
            }
        }

        #endregion

        #region Onverride method(s)

        /// <summary>
        /// ProcessRXQueue.
        /// </summary>
        protected override void ProcessRXQueue()
        {
            byte[] rawPackage = ExtractPackage();
            if (null == rawPackage)
                return; // no package extract.

            byte[] separaters = new byte[] { 0x0D, 0x0A };
            byte[][] contents = Split(rawPackage, separaters);
            UpdateValues(contents);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Device Name.
        /// </summary>
        public override string DeviceName { get { return "TScaleQHW"; } }

        #endregion
    }

    #endregion
}
