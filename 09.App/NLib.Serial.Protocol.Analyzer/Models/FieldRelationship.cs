#region Using

using System;
using System.Collections.Generic;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Models
{
    /// <summary>
    /// Defines relationships between multiple fields.
    /// Used for combined fields (e.g., Date + Time → DateTime),
    /// split fields (e.g., "1.94 kg" → Value + Unit),
    /// or calculated fields (e.g., GrossWeight - TareWeight = NetWeight).
    /// </summary>
    public class FieldRelationship
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public FieldRelationship()
        {
            SourceFields = new List<string>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the relationship type.
        /// Values: "Combine", "Split", "Calculate", "Derive"
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the source field names (fields to combine/use in calculation).
        /// Example: ["Date", "Time"] for combining into DateTime.
        /// </summary>
        public List<string> SourceFields { get; set; }

        /// <summary>
        /// Gets or sets the target field name (result of combination/calculation).
        /// Example: "DateTime" for combined Date+Time.
        /// </summary>
        public string TargetField { get; set; }

        /// <summary>
        /// Gets or sets the operation description.
        /// Example: "Date.Date + Time" for DateTime combination.
        /// Example: "GrossWeight - TareWeight" for NetWeight calculation.
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// Gets or sets the confidence score (0.0 to 1.0) for this relationship.
        /// Based on how many sample values match the relationship rule.
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Gets or sets the reason why this relationship was detected.
        /// Example: "Adjacent date and time fields detected"
        /// Example: "Formula GW - TW = NW matched 95% of samples"
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the tolerance for numeric calculations.
        /// Used when Type = "Calculate" to allow for rounding differences.
        /// Example: 0.01 for weight calculations (within 0.01kg).
        /// </summary>
        public double Tolerance { get; set; }

        #endregion
    }
}
