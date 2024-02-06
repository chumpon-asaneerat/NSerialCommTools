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
    #region JIK6CABData

    /// <summary>
    /// The JIK6CABData class.
    /// </summary>
    public class JIK6CABData : SerialDeviceData
    {
        #region Internal Variables

        private DateTime _dt;
        private decimal _TW = decimal.Zero;
        private decimal _NW = decimal.Zero;
        private decimal _GW = decimal.Zero;
        private decimal _PCS = decimal.Zero;
        private string _TUnit = "kg"; // kg or g
        private string _NUnit = "kg"; // kg or g
        private string _GUnit = "kg"; // kg or g

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
            // ^KJIK000
            // 5E 4B 4A 49 4B 30 30 30 0D 0A
            output = "^KJIK000";
            output += ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));

            // DATE + NEW LINE
            // 2023-11-07
            // 32 30 32 33 2D 31 31 2D 30 37 0D 0A
            output = Date.ToString("dd-MM-yyyy", DateTimeFormatInfo.InvariantInfo) + ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));

            // TIME + NEW LINE
            // 17:19:38
            // 31 37 3A 31 39 3A 32 36 0D 0A
            output = Date.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));

            // T.W with Unit.
            //   0.00 kg
            // 20 20 30 2E 30 30 20 6B 67 0D 0A
            output = ((double)TW).ToString("F2").PadLeft(6, ' ');
            output += " " + TUnit.PadLeft(2, ' ');
            output += ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // G.W.
            //   1.94 kg
            // 20 20 31 2E 39 34 20 6B 67 0D 0A
            output = ((double)GW).ToString("F2").PadLeft(6, ' ');
            output += " " + GUnit.PadLeft(2, ' ');
            output += ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // Unknown 1
            // 0
            // 30 0D 0A
            output = "0";
            output += ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // Unknown 2
            // 0
            // 30 0D 0A
            output = "0";
            output += ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // N.W.
            //   1.94 kg
            // 20 20 31 2E 39 34 20 6B 67 0D 0A
            output = ((double)NW).ToString("F2").PadLeft(6, ' ');
            output += " " + NUnit.PadLeft(2, ' ');
            output += ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // ?? (same value as N.W.)
            //   1.94 kg
            // 20 20 31 2E 39 34 20 6B 67 0D 0A
            output = ((double)NW).ToString("F2").PadLeft(6, ' ');
            output += " " + NUnit.PadLeft(2, ' ');
            output += ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // PCS
            //     0 pcs
            // 20 20 20 20 30 20 70 63 73 0D 0A
            output = PCS.ToString("G").PadLeft(5, ' ');
            output += " pcs";
            output += ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // Blank line 1
            //  
            // 20 0D 0A
            output = " ";
            output += ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // Blank line 2
            //  
            // 20 0D 0A
            output = " ";
            output += ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // End Package 1
            // E
            // 45 0D 0A
            output = "E";
            output += ascii.x0D + ascii.x0A;
            buffers.AddRange(Encoding.ASCII.GetBytes(output));
            // End Package 2
            // ~P1
            // 7E 50 31 0D 0A
            output = "~P1";
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

        public decimal TW
        {
            get { return _TW; }
            set
            {
                if (_TW != value)
                {
                    _TW = value;
                    Raise(() => this.TW);
                }
            }
        }

        public decimal NW
        {
            get { return _NW; }
            set
            {
                if (_NW != value)
                {
                    _NW = value;
                    Raise(() => this.NW);
                }
            }
        }

        public decimal GW
        {
            get { return _GW; }
            set
            {
                if (_GW != value)
                {
                    _GW = value;
                    Raise(() => this.GW);
                }
            }
        }

        public decimal PCS
        {
            get { return _PCS; }
            set
            {
                if (_PCS != value)
                {
                    _PCS = value;
                    Raise(() => this.PCS);
                }
            }
        }

        public string TUnit
        {
            get { return _TUnit; }
            set
            {
                if (_TUnit != value)
                {
                    _TUnit = value;
                    Raise(() => this.TUnit);
                }
            }
        }

        public string NUnit
        {
            get { return _NUnit; }
            set
            {
                if (_NUnit != value)
                {
                    _NUnit = value;
                    Raise(() => this.NUnit);
                }
            }
        }

        public string GUnit
        {
            get { return _GUnit; }
            set
            {
                if (_GUnit != value)
                {
                    _GUnit = value;
                    Raise(() => this.GUnit);
                }
            }
        }

        public DateTime Date 
        {
            get { return _dt; }
            set
            {
                if (_dt != value)
                {
                    _dt = value;
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
    #region JIK6CABDevice

    /// <summary>
    /// The JIK6CABDevice class.
    /// </summary>
    public class JIK6CABDevice : SerialDeviceEmulator<JIK6CABData>
    {
        #region Singelton Access

        private static JIK6CABDevice _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static JIK6CABDevice Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(JIK6CABDevice))
                    {
                        _instance = new JIK6CABDevice();
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
        private JIK6CABDevice()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~JIK6CABDevice()
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
        public override string DeviceName { get { return "JADEVER - JIK-6CAB"; } }

        #endregion
    }

    #endregion
}

namespace NLib.Serial.Terminals
{
    #region JIK6CABTerminal

    /// <summary>
    /// The JIK6CABTerminal class.
    /// </summary>
    public class JIK6CABTerminal : SerialDeviceTerminal<JIK6CABData>
    {
        #region Singelton Access

        private static JIK6CABTerminal _instance = null;
        /// <summary>
        /// Singelton Access.
        /// </summary>
        public static JIK6CABTerminal Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(JIK6CABTerminal))
                    {
                        _instance = new JIK6CABTerminal();
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
        private JIK6CABTerminal()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~JIK6CABTerminal()
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

        private bool bCompleted = true;
        private DateTime? date = new DateTime?();
        private decimal? tw = new decimal?();
        private string tu = null;
        private decimal? nw = new decimal?();
        private string nu = null;
        private decimal? gw = new decimal?();
        private string gu = null;
        private decimal? pcs = new decimal?();

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

            if (line.Contains("g"))
            {
                // weight
                try
                {
                    if (!bCompleted)
                    {
                        string[] elems = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (!tw.HasValue)
                        {
                            string w = (elems.Length > 0) ? elems[0] : "0";
                            tw = decimal.Parse(w);
                            tu = (elems.Length > 1) ? elems[1] : "kg";
                        }
                        else if (tw.HasValue && !gw.HasValue)
                        {
                            string w = (elems.Length > 0) ? elems[0] : "0";
                            gw = decimal.Parse(w);
                            gu = (elems.Length > 1) ? elems[1] : "kg";
                        }
                        else if (tw.HasValue && gw.HasValue && !nw.HasValue)
                        {
                            string w = (elems.Length > 0) ? elems[0] : "0";
                            nw = decimal.Parse(w);
                            // extract unit
                            nu = (elems.Length > 1) ? elems[1] : "kg";
                        }
                        else
                        {

                        }
                    }
                }
                catch (Exception ex)
                {
                    med.Err(ex);
                }
            }
            else if (line.Contains("pcs"))
            {
                // pcs only
                try
                {
                    if (!bCompleted)
                    {
                        string[] elems = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (!pcs.HasValue)
                        {
                            string sPCS = (elems.Length > 0) ? elems[0] : "0";
                            pcs = decimal.Parse(sPCS);
                        }
                    }
                }
                catch (Exception ex)
                {
                    med.Err(ex);
                }
            }
            else if (line.Contains("-") || line.Contains("/"))
            {
                if (!bCompleted)
                {
                    try
                    {
                        if (line.Contains("-"))
                        {
                            DateTime val = DateTime.ParseExact(line, "dd-MMM-yyyy",
                                DateTimeFormatInfo.InvariantInfo);
                            date = new DateTime(val.Year, val.Month, val.Day);
                        }
                        else if (line.Contains("/"))
                        {
                            DateTime val = DateTime.ParseExact(line, "dd/MMM/yyyy",
                                DateTimeFormatInfo.InvariantInfo);
                            date = new DateTime(val.Year, val.Month, val.Day);
                        }
                    }
                    catch (Exception ex)
                    {
                        med.Err(ex);
                        date = new DateTime?();
                    }
                }
            }
            else if (line.Contains(":"))
            {
                if (!bCompleted)
                {
                    // assume time
                    DateTime dt = (date.HasValue) ? date.Value : DateTime.Now;
                    try
                    {
                        DateTime val = DateTime.ParseExact(line, "HH:mm:ss",
                            DateTimeFormatInfo.InvariantInfo);

                        date = new DateTime(dt.Year, dt.Month, dt.Day, val.Hour, val.Minute, val.Second);
                    }
                    catch (Exception ex)
                    {
                        med.Err(ex);
                        date = dt;
                    }
                }
            }
            else if (line.Contains("KJIK"))
            {
                if (bCompleted)
                {
                    bCompleted = false; // Set start package flag
                    date = new DateTime?();
                    tw = new decimal?();
                    tu = null;
                    gw = new decimal?();
                    gu = null;
                    nw = new decimal?();
                    nu = null;
                    pcs = new decimal?();
                }
            }
            else if (line.Contains("P1"))
            {
                if (!bCompleted)
                {
                    // Detected End of pagkage so assign to object.
                    Value.Date = (date.HasValue) ? date.Value : DateTime.Now;
                    Value.TW = (tw.HasValue) ? tw.Value : decimal.Zero;
                    Value.TUnit = tu;
                    Value.NW = (nw.HasValue) ? nw.Value : decimal.Zero;
                    Value.NUnit = nu;
                    Value.GW = (gw.HasValue) ? gw.Value : decimal.Zero;
                    Value.GUnit = gu;
                    Value.PCS = (pcs.HasValue) ? pcs.Value : decimal.Zero;
                    bCompleted = true; // Set end package flag
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
        public override string DeviceName { get { return "JADEVER - JIK-6CAB"; } }

        #endregion
    }

    #endregion
}
