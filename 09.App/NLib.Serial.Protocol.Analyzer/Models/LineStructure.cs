#region Using

using System;
using System.Collections.Generic;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Represents the structure of a single line in a multi-line package
    /// </summary>
    public class LineStructure
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public LineStructure()
        {
            Values = new List<string>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the line number (1-based)
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the field name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the data type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the regex pattern
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the format string (for datetime/decimal)
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the unit
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets whether unit is attached
        /// </summary>
        public bool? UnitAttached { get; set; }

        /// <summary>
        /// Gets or sets minimum value
        /// </summary>
        public decimal? Min { get; set; }

        /// <summary>
        /// Gets or sets maximum value
        /// </summary>
        public decimal? Max { get; set; }

        /// <summary>
        /// Gets or sets whether this line is required
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets possible values (for enums)
        /// </summary>
        public List<string> Values { get; set; }

        /// <summary>
        /// Gets or sets raw sample data
        /// </summary>
        public string RawSample { get; set; }

        #endregion
    }
}
