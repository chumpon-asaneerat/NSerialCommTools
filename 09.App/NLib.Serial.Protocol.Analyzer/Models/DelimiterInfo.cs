#region Using

using System;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Information about detected delimiter
    /// </summary>
    public class DelimiterInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the delimiter character
        /// </summary>
        public char Delimiter { get; set; }

        /// <summary>
        /// Gets or sets the character name (for display)
        /// </summary>
        public string CharacterName { get; set; }

        /// <summary>
        /// Gets or sets the frequency of occurrence (0-100%)
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// Gets or sets average count per message
        /// </summary>
        public double AverageCount { get; set; }

        /// <summary>
        /// Gets or sets confidence level (0-100%)
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Gets or sets whether this is a structural delimiter
        /// </summary>
        public bool IsStructural { get; set; }

        #endregion
    }
}
