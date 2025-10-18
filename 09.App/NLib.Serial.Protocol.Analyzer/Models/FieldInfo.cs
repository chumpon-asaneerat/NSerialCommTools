#region Using

using System;
using System.Collections.Generic;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Information about a detected field
    /// </summary>
    public class FieldInfo
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public FieldInfo()
        {
            UniqueValues = new List<string>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the field position/index
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the suggested field name
        /// </summary>
        public string SuggestedName { get; set; }

        /// <summary>
        /// Gets or sets the detected type (string, decimal, integer, datetime)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the minimum length
        /// </summary>
        public int MinLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum length
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets whether length is constant
        /// </summary>
        public bool IsConstantLength { get; set; }

        /// <summary>
        /// Gets or sets the list of unique values found
        /// </summary>
        public List<string> UniqueValues { get; set; }

        /// <summary>
        /// Gets or sets whether this field has constant value
        /// </summary>
        public bool IsConstant { get; set; }

        /// <summary>
        /// Gets or sets the minimum numeric value (for decimal/integer types)
        /// </summary>
        public decimal? MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum numeric value (for decimal/integer types)
        /// </summary>
        public decimal? MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the detected unit (if any)
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets whether unit is attached to value
        /// </summary>
        public bool UnitAttached { get; set; }

        /// <summary>
        /// Gets or sets confidence level (0-100%)
        /// </summary>
        public double Confidence { get; set; }

        #endregion
    }
}
