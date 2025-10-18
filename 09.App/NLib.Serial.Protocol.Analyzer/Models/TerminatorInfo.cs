#region Using

using System;
using System.Linq;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Information about detected message terminator
    /// </summary>
    public class TerminatorInfo
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public TerminatorInfo()
        {
            Bytes = new byte[0];
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets whether a terminator was detected
        /// </summary>
        public bool Detected { get; set; }

        /// <summary>
        /// Gets or sets the terminator type (CR, LF, CRLF, Custom)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the terminator bytes
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// Gets or sets the hex representation
        /// </summary>
        public string HexString
        {
            get
            {
                if (Bytes == null || Bytes.Length == 0)
                    return string.Empty;
                return string.Join(" ", Bytes.Select(b => b.ToString("X2")));
            }
        }

        /// <summary>
        /// Gets or sets the frequency of occurrence
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// Gets or sets the total messages analyzed
        /// </summary>
        public int TotalMessages { get; set; }

        /// <summary>
        /// Gets the frequency percentage
        /// </summary>
        public double FrequencyPercent
        {
            get
            {
                if (TotalMessages == 0) return 0;
                return (double)Frequency / TotalMessages * 100.0;
            }
        }

        /// <summary>
        /// Gets or sets confidence level (0-100%)
        /// </summary>
        public double Confidence { get; set; }

        #endregion
    }
}
