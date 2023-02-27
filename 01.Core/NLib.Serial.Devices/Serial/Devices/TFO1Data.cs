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
using System.Runtime.InteropServices;

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
            string output = string.Empty;
            // F      0.0
            output += F.ToString("F1").PadLeft(9, ' ');

            var buffers = Encoding.ASCII.GetBytes(output);
            return buffers;
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
