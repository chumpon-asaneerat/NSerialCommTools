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
    #region WeightQAData

    /// <summary>
    /// The WeightQAData class.
    /// </summary>
    public class WeightQAData : SerialDeviceData
    {
        #region Internal Variables

        private decimal _W = decimal.Zero;
        private string _Unit = "G";
        private string _Mode = "S";

        #endregion

        #region Override Methods

        /// <summary>
        /// Convert content to byte array.
        /// </summary>
        /// <returns>Returns content in byte array.</returns>
        public override byte[] ToByteArray()
        {
            List<byte> buffers = new List<byte>();

            string unit = string.IsNullOrWhiteSpace(Unit) ? "G" : Unit.Trim().ToUpper();
            string mode = string.IsNullOrWhiteSpace(Mode) ? "S" : Mode.Trim().ToUpper();

            string output;
            // +007.12/3 G S..
            // 2B 30 30 37 2E 31 32 2F 33 20 47 20 53 0D 0A

            string actualW = ((double)W).ToString("F3").PadLeft(7, '0');
            string firstPart = actualW.Substring(0, actualW.Length - 1);
            string lastDigit = actualW.Substring(actualW.Length - 1, 1);
            output = "+" + firstPart;
            output += "/" + lastDigit;
            output += " " + unit;
            output += " " + mode;
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
    #region WeightQADevice

    /// <summary>
    /// The WeightQADevice class.
    /// </summary>
    public class WeightQADevice : SerialDeviceEmulator<WeightQAData>
    {
        #region Singelton Access

        private static WeightQADevice _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static WeightQADevice Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(WeightQADevice))
                    {
                        _instance = new WeightQADevice();
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
        private WeightQADevice()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~WeightQADevice()
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
        public override string DeviceName { get { return "WeightQA"; } }

        #endregion
    }

    #endregion
}

namespace NLib.Serial.Terminals
{
    #region WeightQATerminal

    /// <summary>
    /// The WeightQATerminal class.
    /// </summary>
    public class WeightQATerminal : SerialDeviceTerminal<WeightQAData>
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

            string[] elems = line.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if (null == elems || elems.Length < 2) return;
            string sUM = elems[1].Trim();
            string[] elems2 = sUM.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (null == elems2 || elems2.Length < 3) return;


            string w = elems[0].Trim() + elems2[0].Trim();
            try
            {
                Value.W = decimal.Parse(w);
            }
            catch (Exception ex)
            {
                med.Err(ex);
            }

            try
            {
                Value.Unit = elems2[1].Trim().ToUpper();
                Value.Mode = elems2[2].Trim().ToUpper();
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
        public override string DeviceName { get { return "WeightQA"; } }

        #endregion
    }

    #endregion
}
