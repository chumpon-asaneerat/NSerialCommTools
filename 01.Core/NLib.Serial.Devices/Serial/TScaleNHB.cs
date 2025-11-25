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
    #region TScaleNHBData

    /// <summary>
    /// The TScaleNHBData class.
    /// </summary>
    public class TScaleNHBData : SerialDeviceData
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
            // ST,GS    20.7g  ..
            // 53 54 2C 47 53 20 20 20 20 32 30 2E 37 67 20 20 0D 0A

            string actualW = ((double)W).ToString("F3");
            output = status + "," + mode + " ";
            output += actualW.PadLeft(7, ' ');
            output += unit + "  ";
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
    #region TScaleNHBDevice

    /// <summary>
    /// The TScaleNHBDevice class.
    /// </summary>
    public class TScaleNHBDevice : SerialDeviceEmulator<TScaleNHBData>
    {
        #region Singelton Access

        private static TScaleNHBDevice _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static TScaleNHBDevice Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(TScaleNHBDevice))
                    {
                        _instance = new TScaleNHBDevice();
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
        private TScaleNHBDevice()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~TScaleNHBDevice()
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
        public override string DeviceName { get { return "TScaleNHB"; } }

        #endregion
    }

    #endregion
}

namespace NLib.Serial.Terminals
{
    #region TScaleNHBTerminal

    /// <summary>
    /// The TScaleNHBTerminal class.
    /// </summary>
    public class TScaleNHBTerminal : SerialDeviceTerminal<TScaleNHBData>
    {
        #region Singelton Access

        private static TScaleNHBTerminal _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static TScaleNHBTerminal Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(TScaleNHBTerminal))
                    {
                        _instance = new TScaleNHBTerminal();
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
        private TScaleNHBTerminal()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~TScaleNHBTerminal()
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

            // Protocol: ST,GS    20.7g  \r\n
            // Format: STATUS,MODE WEIGHT UNIT
            // Example: ST,GS    20.7g

            try
            {
                // Split by comma first to separate STATUS,MODE from rest
                string[] parts = line.Split(new char[] { ',' }, StringSplitOptions.None);
                if (null == parts || parts.Length < 2) return;

                string status = parts[0].Trim();

                // The second part contains: "GS    20.7g  "
                string remainder = parts[1];

                // Split by space to separate MODE and WEIGHT+UNIT
                string[] spaceParts = remainder.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (null == spaceParts || spaceParts.Length < 2) return;

                string mode = spaceParts[0].Trim();
                string weightWithUnit = spaceParts[1].Trim();

                // Parse weight and unit
                // weightWithUnit format: "20.7g"
                string weightStr = "";
                string unit = "";

                // Find where the numeric part ends (look for 'g' or 'kg')
                int unitStartIdx = -1;
                for (int i = weightWithUnit.Length - 1; i >= 0; i--)
                {
                    if (char.IsDigit(weightWithUnit[i]) || weightWithUnit[i] == '.')
                    {
                        unitStartIdx = i + 1;
                        break;
                    }
                }

                if (unitStartIdx > 0)
                {
                    weightStr = weightWithUnit.Substring(0, unitStartIdx);
                    unit = weightWithUnit.Substring(unitStartIdx);
                }
                else
                {
                    weightStr = weightWithUnit;
                    unit = "g";
                }

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
        public override string DeviceName { get { return "TScaleNHB"; } }

        #endregion
    }

    #endregion
}
