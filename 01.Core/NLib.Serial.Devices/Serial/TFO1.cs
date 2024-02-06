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
using NLib.Devices.SerialPorts;

#endregion

namespace NLib.Serial.Devices
{
    #region TFO1Data

    /// <summary>
    /// The TFO1Data class.
    /// </summary>
    public class TFO1Data : SerialDeviceData
    {
        #region Internal Variables

        private decimal _F = decimal.Zero;
        private decimal _H = decimal.Zero;
        private decimal _Q = decimal.Zero;
        private decimal _X = decimal.Zero;
        private decimal _A = decimal.Zero;
        private decimal _W0 = decimal.Zero;
        private decimal _W4 = decimal.Zero;
        private decimal _W1 = decimal.Zero;
        private int _W2 = 0;
        private byte _B = 0x83;
        private DateTime _C = DateTime.Now;
        private byte _V = 0x31;

        #endregion

        #region Override Methods

        /// <summary>
        /// Convert content to byte array.
        /// </summary>
        /// <returns>Returns content in byte array.</returns>
        public override byte[] ToByteArray()
        {
            List<byte> buffers = new List<byte>();
            string output;
            // F      0.0
            output = "F" + ((double)F).ToString("F1").PadLeft(9, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // H      0.0.
            output = "H" + ((double)H).ToString("F1").PadLeft(9, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // Q      0.0.
            output = "Q" + ((double)Q).ToString("F1").PadLeft(9, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // X      0.0.
            output = "X" + ((double)X).ToString("F1").PadLeft(9, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // A    366.0.
            output = "A" + ((double)A).ToString("F1").PadLeft(9, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // 0    23.0.
            output = "0" + ((double)W0).ToString("F1").PadLeft(9, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // 4    343.5.
            output = "4" + ((double)W4).ToString("F1").PadLeft(9, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // 1      0.0.
            output = "1" + ((double)W1).ToString("F1").PadLeft(9, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // 2       0.
            output = "2" + W2.ToString("D0").PadLeft(8, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // B..
            //output += "B" + ascii.GetString(B) + ascii.x0D;
            output = "B";
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            buffers.Add(B);
            buffers.Add(0x0D);
            // C20. 02. 2023. MON 09:20AM.
            output = "C";
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            output = C.Day.ToString("D2");
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            buffers.Add(0xF4);
            output = " " + C.Month.ToString("D2");
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            buffers.Add(0xF3);
            output = " " + C.Year.ToString("D4");
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            buffers.Add(0xF2);
            output = " " + C.ToString("ddd HH:mmtt", DateTimeFormatInfo.InvariantInfo).ToUpperInvariant();
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            buffers.Add(0x0D);
            // V1..
            output = "V";
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            buffers.Add(V);
            buffers.Add(0x0D);
            buffers.Add(0x0A);

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

        public decimal F
        {
            get { return _F; }
            set
            {
                if (_F != value)
                {
                    _F = value;
                    Raise(() => this.F);
                }
            }
        }

        public decimal H
        {
            get { return _H; }
            set
            {
                if (_H != value)
                {
                    _H = value;
                    Raise(() => this.H);
                }
            }
        }

        public decimal Q
        {
            get { return _Q; }
            set
            {
                if (_Q != value)
                {
                    _Q = value;
                    Raise(() => this.Q);
                }
            }
        }

        public decimal X
        {
            get { return _X; }
            set
            {
                if (_X != value)
                {
                    _X = value;
                    Raise(() => this.X);
                }
            }
        }

        public decimal A
        {
            get { return _A; }
            set
            {
                if (_A != value)
                {
                    _A = value;
                    Raise(() => this.A);
                }
            }
        }

        public decimal W0
        {
            get { return _W0; }
            set
            {
                if (_W0 != value)
                {
                    _W0 = value;
                    Raise(() => this.W0);
                }
            }
        }

        public decimal W4
        {
            get { return _W4; }
            set
            {
                if (_W4 != value)
                {
                    _W4 = value;
                    Raise(() => this.W4);
                }
            }
        }

        public decimal W1
        {
            get { return _W1; }
            set
            {
                if (_W1 != value)
                {
                    _W1 = value;
                    Raise(() => this.W1);
                }
            }
        }

        public int W2
        {
            get { return _W2; }
            set
            {
                if (_W2 != value)
                {
                    _W2 = value;
                    Raise(() => this.W2);
                }
            }
        }

        public byte B
        {
            get { return _B; }
            set
            {
                if (_B != value)
                {
                    _B = value;
                    Raise(() => this.B);
                }
            }
        }

        public DateTime C
        {
            get { return _C; }
            set
            {
                if (_C != value)
                {
                    _C = value;
                    Raise(() => this.C);
                }
            }
        }

        public byte V
        {
            get { return _V; }
            set
            {
                if (_V != value)
                {
                    _V = value;
                    Raise(() => this.V);
                }
            }
        }

        #endregion
    }

    #endregion
}

namespace NLib.Serial.Emulators
{
    #region TFO1Device

    /// <summary>
    /// The TFO1Device class.
    /// </summary>
    public class TFO1Device : SerialDeviceEmulator<TFO1Data>
    {
        #region Singelton Access

        private static TFO1Device _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static TFO1Device Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(TFO1Device))
                    {
                        _instance = new TFO1Device();
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
        private TFO1Device()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~TFO1Device()
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
        public override string DeviceName { get { return "TFO1"; } }

        #endregion
    }

    #endregion
}

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

            char hdr = (char)content[0];
            switch (hdr)
            {
                case 'F':
                    if (content.Length < 11) 
                        break;
                    {
                        // F      0.0.
                        // 46 20 20 20 20 20 20 30 2E 30 0D
                        try
                        {
                            string val = Encoding.ASCII.GetString(content, 1, 9);
                            Value.F = decimal.Parse(val);
                        }
                        catch (Exception ex)
                        {
                            med.Err(ex);
                        }
                    }
                    break;
                case 'H':
                    if (content.Length < 11)
                        break;
                    {
                        // H      0.0.
                        // 48 20 20 20 20 20 20 30 2E 30 0D
                        try
                        {
                            string val = Encoding.ASCII.GetString(content, 1, 9);
                            Value.H = decimal.Parse(val);
                        }
                        catch (Exception ex)
                        {
                            med.Err(ex);
                        }
                    }
                    break;
                case 'Q':
                    if (content.Length < 11)
                        break;
                    {
                        // Q      0.0.
                        // 51 20 20 20 20 20 20 30 2E 30 0D 
                        try
                        {
                            string val = Encoding.ASCII.GetString(content, 1, 9);
                            Value.Q = decimal.Parse(val);
                        }
                        catch (Exception ex)
                        {
                            med.Err(ex);
                        }
                    }
                    break;
                case 'X':
                    if (content.Length < 11)
                        break;
                    {
                        // X      0.0.
                        // 58 20 20 20 20 20 20 30 2E 30 0D
                        try
                        {
                            string val = Encoding.ASCII.GetString(content, 1, 9);
                            Value.X = decimal.Parse(val);
                        }
                        catch (Exception ex)
                        {
                            med.Err(ex);
                        }
                    }
                    break;
                case 'A':
                    if (content.Length < 11)
                        break;
                    {
                        // A    366.0.
                        // 41 20 20 20 20 33 36 36 2E 30 0D
                        try
                        {
                            string val = Encoding.ASCII.GetString(content, 1, 9);
                            Value.A = decimal.Parse(val);
                        }
                        catch (Exception ex)
                        {
                            med.Err(ex);
                        }
                    }
                    break;
                case '0':
                    if (content.Length < 11)
                        break;
                    {
                        // 0    23.0.
                        // 30 20 20 20 20 20 32 33 2E 30 0D 
                        try
                        {
                            string val = Encoding.ASCII.GetString(content, 1, 9);
                            Value.W0 = decimal.Parse(val);
                        }
                        catch (Exception ex)
                        {
                            med.Err(ex);
                        }
                    }
                    break;
                case '4':
                    if (content.Length < 11)
                        break;
                    {
                        // 4    343.5.
                        // 34 20 20 20 20 33 34 33 2E 35 0D
                        try
                        {
                            string val = Encoding.ASCII.GetString(content, 1, 9);
                            Value.W4 = decimal.Parse(val);
                        }
                        catch (Exception ex)
                        {
                            med.Err(ex);
                        }
                    }
                    break;
                case '1':
                    if (content.Length < 11)
                        break;
                    {
                        // 1      0.0.
                        // 31 20 20 20 20 20 20 30 2E 30 0D 
                        try
                        {
                            string val = Encoding.ASCII.GetString(content, 1, 9);
                            Value.W1 = decimal.Parse(val);
                        }
                        catch (Exception ex)
                        {
                            med.Err(ex);
                        }
                    }
                    break;
                case '2':
                    if (content.Length < 10)
                        break;
                    {
                        // 2       0.
                        // 32 20 20 20 20 20 20 20 30 0D 
                        try
                        {
                            string val = Encoding.ASCII.GetString(content, 1, 8);
                            Value.W2 = int.Parse(val);
                        }
                        catch (Exception ex)
                        {
                            med.Err(ex);
                        }
                    }
                    break;
                case 'B':
                    if (content.Length < 3)
                        break;
                    {
                        // B..
                        // 42 83 0D 
                        try
                        {
                            Value.B = content[1];
                        }
                        catch (Exception ex)
                        {
                            med.Err(ex);
                        }
                    }
                    break;
                case 'C':
                    if (content.Length < 27)
                        break;
                    {
                        // C20. 02. 2023. MON 09:20AM.
                        // 43 32 30 F4 20 30 32 F3 20 32 30 32 33 F2 20 4D 4F 4E 20 30 39 3A 32 30 41 4D 0D
                        try
                        {
                            string _dd = Encoding.ASCII.GetString(content, 1, 2);
                            string _mm = Encoding.ASCII.GetString(content, 5, 2);
                            string _yyyy = Encoding.ASCII.GetString(content, 9, 4);
                            //string _ddd = Encoding.ASCII.GetString(content, 15, 3);
                            string _hh = Encoding.ASCII.GetString(content, 19, 2);
                            string _mi = Encoding.ASCII.GetString(content, 22, 2);
                            string _ampm = Encoding.ASCII.GetString(content, 24, 2);

                            int dd = int.Parse(_dd);
                            int mm = int.Parse(_mm);
                            int yy = int.Parse(_yyyy);
                            int hh = int.Parse(_hh);
                            int mi = int.Parse(_mi);
                            if (_ampm == "AM") mi += 12;

                            Value.C = new DateTime(yy, mm, dd, hh, mi, 0);
                        }
                        catch (Exception ex)
                        {
                            med.Err(ex);
                        }
                    }
                    break;
                case 'V':
                    if (content.Length < 4)
                        break;
                    {
                        // V1..
                        // 56 31 0D 0A 
                        Value.V = content[1];
                    }
                    break;
                default:
                    break;
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

            byte[] separaters = new byte[] { 0x0D };
            byte[][] contents = Split(rawPackage, separaters);
            UpdateValues(contents);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Device Name.
        /// </summary>
        public override string DeviceName { get { return "TFO1"; } }

        #endregion
    }

    #endregion
}
