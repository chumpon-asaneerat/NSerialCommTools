#region Using

using System;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Models
{
    /// <summary>
    /// Information about a detected message terminator.
    /// </summary>
    public class TerminatorInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the terminator as a string (e.g., "\r\n").
        /// </summary>
        public string String { get; set; }

        /// <summary>
        /// Gets or sets the terminator as a byte array.
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// Gets or sets the display name (e.g., "CRLF", "LF", "CR").
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the frequency (0.0 to 1.0) of this terminator appearing.
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// Gets or sets the confidence (0.0 to 1.0) that this is the correct terminator.
        /// </summary>
        public double Confidence { get; set; }

        #endregion
    }
}
