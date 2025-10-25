#region Using

using System;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Models
{
    /// <summary>
    /// Information about a detected field delimiter.
    /// </summary>
    public class DelimiterInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the delimiter character.
        /// </summary>
        public char Character { get; set; }

        /// <summary>
        /// Gets or sets the display name of the delimiter (e.g., "Comma", "Space", "Tab").
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the frequency (0.0 to 1.0) of this delimiter appearing.
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// Gets or sets the confidence (0.0 to 1.0) that this is a structural delimiter.
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Gets or sets whether this delimiter is structural (used for field separation).
        /// </summary>
        public bool IsStructural { get; set; }

        /// <summary>
        /// Gets or sets the number of occurrences across all messages.
        /// </summary>
        public int OccurrenceCount { get; set; }

        #endregion
    }
}
