#region Using

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Models
{
    /// <summary>
    /// Information about a detected field in the protocol.
    /// Simple structure - only ONE name property that user can edit.
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
            Name = string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the field position (0-based index).
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the field name (user-editable, starts as Field0, Field1...).
        /// NO separate Auto/Custom - just ONE name that user edits.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the detected data type (e.g., "string", "int", "decimal").
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets sample values from the captured data.
        /// CRITICAL: User needs to SEE this data to decide what to name the field!
        /// </summary>
        public List<string> SampleValues { get; set; }

        /// <summary>
        /// Gets or sets the confidence (0.0 to 1.0) in the type detection.
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Gets or sets whether the field appears to be constant across all messages.
        /// </summary>
        public bool IsConstant { get; set; }

        /// <summary>
        /// Gets or sets the minimum length observed.
        /// </summary>
        public int MinLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum length observed.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets a display string of sample values (first 3) for UI binding.
        /// User needs to see actual data to decide field names!
        /// </summary>
        public string SampleValuesDisplay
        {
            get
            {
                if (SampleValues == null || SampleValues.Count == 0)
                    return string.Empty;

                var samples = SampleValues.Take(3);
                return string.Join(", ", samples);
            }
        }

        #endregion
    }
}
