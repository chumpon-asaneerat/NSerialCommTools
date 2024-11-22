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
using System.CodeDom;
using static NLib.NGC;
using Newtonsoft.Json.Linq;

#endregion

namespace NLib.Serial.Devices
{
    #region MettlerMS204TS00Data

    /// <summary>
    /// The MettlerMS204TS00Data class.
    /// </summary>
    public class MettlerMS204TS00Data : SerialDeviceData
    {
        #region Internal Variables

        private string _mode = "N"; // Net
        private decimal _W = decimal.Zero;
        private string _Unit = "g"; // g

        #endregion

        #region Override Methods

        /// <summary>
        /// Convert content to byte array.
        /// </summary>
        /// <returns>Returns content in byte array.</returns>
        public override byte[] ToByteArray()
        {
            List<byte> buffers = new List<byte>();

            string mode = (string.IsNullOrEmpty(Mode.Trim())) ? string.Empty : Mode.Trim();
            if (mode.Length > 2)
            {
                mode = mode.Substring(0, 2);
            }
            string unit = (string.IsNullOrEmpty(Unit.Trim())) ? "g" : Unit.Trim();
            if (unit.Length > 2)
            {
                unit = unit.Substring(0, 2);
            }

            string output = "";

            // MODE (Net)
            //      N
            // 20 20 20 20 20 4E
            output += " " + mode.PadLeft(5, ' ');

            // Weight
            //        0.3749
            // 20 20 20 20 20 20 20 30 2E 33 37 34 36
            output += " " + ((double)W).ToString("0.0000").PadLeft(12, ' ');

            // Unit
            //  g, kg
            // 20 67 20 20 20
            output += " " + unit.PadRight(4, ' ');
            // Space and end of package
            // ..
            // 0D 0A
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

        public string Mode
        {
            get { return _mode; }
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    Raise(() => this.Mode);
                }
            }
        }

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

        #endregion
    }

    #endregion
}

namespace NLib.Serial.Emulators
{
    #region MettlerMS204TS00Device

    /// <summary>
    /// The MettlerMS204TS00Device class.
    /// </summary>
    public class MettlerMS204TS00Device : SerialDeviceEmulator<MettlerMS204TS00Data>
    {
        #region Singelton Access

        private static MettlerMS204TS00Device _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static MettlerMS204TS00Device Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(MettlerMS204TS00Device))
                    {
                        _instance = new MettlerMS204TS00Device();
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
        private MettlerMS204TS00Device()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~MettlerMS204TS00Device()
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
        public override string DeviceName { get { return "Mettler Toledo - MS204TS00"; } }

        #endregion
    }

    #endregion
}

namespace NLib.Serial.Terminals
{
    #region MettlerMS204TS00Terminal

    /// <summary>
    /// The MettlerMS204TS00Terminal class.
    /// </summary>
    public class MettlerMS204TS00Terminal : SerialDeviceTerminal<MettlerMS204TS00Data>
    {
        #region Singelton Access

        private static MettlerMS204TS00Terminal _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static MettlerMS204TS00Terminal Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(MettlerMS204TS00Terminal))
                    {
                        _instance = new MettlerMS204TS00Terminal();
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
        private MettlerMS204TS00Terminal()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~MettlerMS204TS00Terminal()
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
            {
                return;
            }

            line = line.Trim(); // trim string to remove new line.

            // after trim check first char
            if (line[0] == 'N' || line[0] == 'G' || line[0] == 'T' || !char.IsNumber(line[0]))
            {
                Value.Mode = line[0].ToString();
                // remove mode char.
                line = line.Substring(1, line.Length - 1);
            }
            else
            {
                Value.Mode = ""; // No mode.
            }

            // define default value
            Value.W = decimal.Zero;
            Value.Unit = "g"; // default Unit.

            // find weight value and unit
            string val = line.Trim().ToUpper();

            if (!string.IsNullOrEmpty(val))
            {
                string[] wgs = null;
                if (val.Contains("KG"))
                {
                    Value.Unit = "kg"; // update Unit
                    wgs = val.Split(new string[] { "KG" }, StringSplitOptions.RemoveEmptyEntries);
                }
                else if (val.Contains("G"))
                {
                    Value.Unit = "g"; // update Unit
                    wgs = val.Split(new string[] { "G" }, StringSplitOptions.RemoveEmptyEntries);
                }

                if (null != wgs && wgs.Length >= 1)
                {
                    try
                    {
                        var wg = wgs[0].Trim();
                        Value.W = decimal.Parse(wg);
                    }
                    catch (Exception ex)
                    {
                        med.Err(ex);
                    }
                }
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
        public override string DeviceName { get { return "Mettler Toledo - MS204TS00"; } }

        #endregion
    }

    #endregion
}
