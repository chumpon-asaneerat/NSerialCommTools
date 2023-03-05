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
            output = "F" + F.ToString("F1").PadLeft(9, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // H      0.0.
            output = "H" + H.ToString("F1").PadLeft(9, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // Q      0.0.
            output = "Q" + Q.ToString("F1").PadLeft(9, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // X      0.0.
            output = "X" + X.ToString("F1").PadLeft(9, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // A    366.0.
            output = "A" + A.ToString("F1").PadLeft(9, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // 0    23.0.
            output = "0" + W0.ToString("F1").PadLeft(9, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // 4    343.5.
            output = "4" + W4.ToString("F1").PadLeft(9, ' ') + ascii.x0D;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // 1      0.0.
            output = "1" + W1.ToString("F1").PadLeft(9, ' ') + ascii.x0D;
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
