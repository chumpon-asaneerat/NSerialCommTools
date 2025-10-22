#region Using

using System;
using System.Collections.Generic;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Models
{
    /// <summary>
    /// Information about a detected field in the protocol.
    /// </summary>
    public class FieldInfo
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public FieldInfo()
        {
            SampleValues = new List<string>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the field position (0-based index).
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the auto-generated field name.
        /// </summary>
        public string AutoName { get; set; }

        /// <summary>
        /// Gets or sets the detected data type (e.g., "string", "int", "decimal").
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets sample values from the captured data.
        /// </summary>
        public List<string> SampleValues { get; set; }

        /// <summary>
        /// Gets or sets the confidence (0.0 to 1.0) in the type detection.
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Gets or sets the minimum length observed for this field.
        /// </summary>
        public int MinLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum length observed for this field.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets whether the field appears to be constant across all messages.
        /// </summary>
        public bool IsConstant { get; set; }

        #endregion
    }
}
