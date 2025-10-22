#region Using

using System;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Models
{
    /// <summary>
    /// Validation rule for data integrity checks.
    /// Applied to parsed or serialized data to ensure correctness.
    /// </summary>
    public class ValidationRule
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidationRule()
        {
            Severity = "Error";
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the rule name (unique identifier).
        /// Example: "TareWeightRange", "NetWeightFormula"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the validation type.
        /// Values: "Range", "DateTime", "Formula", "Relationship", "Pattern"
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the field name to validate (for single-field rules).
        /// Example: "TareWeight" for range validation.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the validation severity.
        /// Values: "Error" (prevents data acceptance), "Warning" (allows with warning)
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// Gets or sets the error/warning message to display if validation fails.
        /// Example: "TareWeight must be between 0 and 100"
        /// </summary>
        public string Message { get; set; }

        #endregion

        #region Range Validation Properties

        /// <summary>
        /// Gets or sets the minimum value (for range validation).
        /// </summary>
        public double? MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value (for range validation).
        /// </summary>
        public double? MaxValue { get; set; }

        #endregion

        #region Formula Validation Properties

        /// <summary>
        /// Gets or sets the formula for calculation-based validation.
        /// Example: "GrossWeight - TareWeight = NetWeight"
        /// Used when Type = "Formula".
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// Gets or sets the tolerance for formula validation.
        /// Example: 0.01 for decimal comparisons (within 0.01 difference is acceptable).
        /// </summary>
        public double? Tolerance { get; set; }

        #endregion

        #region Relationship Validation Properties

        /// <summary>
        /// Gets or sets the custom condition expression (for relationship validation).
        /// Example: "GrossWeight >= TareWeight"
        /// </summary>
        public string Condition { get; set; }

        #endregion

        #region Pattern Validation Properties

        /// <summary>
        /// Gets or sets the regex pattern (for pattern validation).
        /// Field value must match this pattern.
        /// </summary>
        public string Pattern { get; set; }

        #endregion
    }
}
