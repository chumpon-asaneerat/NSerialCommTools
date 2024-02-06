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

#endregion

namespace NLib.Serial.Devices
{
    #region WeightQAData

    /// <summary>
    /// The WeightQAData class.
    /// </summary>
    public class WeightSPUNData : SerialDeviceData
    {
        #region Internal Variables

        private decimal _W = decimal.Zero;
        private string _Unit = "kg";
        private string _O = "G";

        #endregion

        #region Override Methods

        /// <summary>
        /// Convert content to byte array.
        /// </summary>
        /// <returns>Returns content in byte array.</returns>
        public override byte[] ToByteArray()
        {
            List<byte> buffers = new List<byte>();

            string unit = (string.IsNullOrEmpty(Unit.Trim())) ? "kg" : Unit.Trim();
            if (unit.Length > 2)
            {
                unit = unit.Substring(0, 2);
            }

            string output;

            //     20.0 kg    G..
            // 20 20 20 20 32 30 2E 30 20 6B 67 20 20 20 20 47 0D 0A
            output = ((double)W).ToString("F1").PadLeft(8, ' ');
            output += " " + unit.PadLeft(2, ' ');
            output += " " + O.PadLeft(4, ' ');
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

        public string O
        {
            get { return _O; }
            set
            {
                if (_O != value)
                {
                    _O = value;
                    Raise(() => this.O);
                }
            }
        }

        #endregion
    }

    #endregion
}

namespace NLib.Serial.Emulators
{
    #region WeightSPUNDevice

    /// <summary>
    /// The WeightSPUNDevice class.
    /// </summary>
    public class WeightSPUNDevice : SerialDeviceEmulator<WeightSPUNData>
    {
        #region Singelton Access

        private static WeightSPUNDevice _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static WeightSPUNDevice Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(WeightSPUNDevice))
                    {
                        _instance = new WeightSPUNDevice();
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
        private WeightSPUNDevice()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~WeightSPUNDevice()
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
        public override string DeviceName { get { return "WeightSPUN"; } }

        #endregion
    }

    #endregion
}

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

            string[] elems = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (null == elems || elems.Length < 3) return;
            string w = elems[0].Trim();
            try
            {
                Value.W = decimal.Parse(w);
            }
            catch (Exception ex)
            {
                med.Err(ex);
            }

            string unit = elems[1].Trim();
            string o = elems[2].Trim();

            try
            {
                Value.O = o;
                Value.Unit = unit;
            }
            catch (Exception ex2)
            {
                med.Err(ex2);
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
        public override string DeviceName { get { return "WeightSPUN"; } }

        #endregion
    }

    #endregion
}
