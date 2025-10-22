#region Using

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Models
{
    /// <summary>
    /// Information about a detected field in the protocol.
    /// Complete model with all properties required for bidirectional JSON definition generation.
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
            DataType = "string";
            Alignment = "left";
            PaddingChar = " ";
            FieldPosition = "body";
            Action = "Parse";
            Required = true;
            IncludeInDefinition = true;
        }

        #endregion

        #region Basic Properties

        /// <summary>
        /// Gets or sets the field order (0-based index) in the message sequence.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the field name (user-editable, starts as Field0, Field1...).
        /// Must be valid C# identifier for use in NTerminal&lt;T&gt; and NDevice&lt;T&gt;.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the detected data type (e.g., "string", "int", "decimal", "DateTime", "TimeSpan").
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Gets or sets sample values from the captured data.
        /// CRITICAL: User needs to SEE this data to decide what to name the field!
        /// </summary>
        public List<string> SampleValues { get; set; }

        /// <summary>
        /// Gets or sets the confidence (0.0 to 1.0) in the type detection.
        /// </summary>
        public double Confidence { get; set; }

        #endregion

        #region Parsing Direction (for NTerminal&lt;T&gt;)

        /// <summary>
        /// Gets or sets the regex pattern to extract value from bytes.
        /// Example: @"^\s*([\d.]+)\s*kg\s*$" to extract weight from "  1.94 kg"
        /// </summary>
        public string ParsePattern { get; set; }

        #endregion

        #region Serialization Direction (for NDevice&lt;T&gt;)

        /// <summary>
        /// Gets or sets the C# format string for serialization.
        /// Example: "{0,7:F2} kg" for right-aligned weight with 2 decimals and kg unit.
        /// </summary>
        public string FormatString { get; set; }

        /// <summary>
        /// Gets or sets text alignment ("left" or "right").
        /// </summary>
        public string Alignment { get; set; }

        /// <summary>
        /// Gets or sets the padding character (usually space).
        /// </summary>
        public string PaddingChar { get; set; }

        /// <summary>
        /// Gets or sets the total width of the formatted field.
        /// </summary>
        public int Width { get; set; }

        #endregion

        #region Common Properties

        /// <summary>
        /// Gets or sets the message terminator for this field.
        /// Example: "\r\n" or "\r"
        /// </summary>
        public string Terminator { get; set; }

        /// <summary>
        /// Gets or sets the position in message structure ("body", "header", "footer").
        /// </summary>
        public string FieldPosition { get; set; }

        /// <summary>
        /// Gets or sets whether this field is required in every message.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the field description (user-provided or auto-generated).
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region State Machine Properties

        /// <summary>
        /// Gets or sets the field type classification for State Machine parsing.
        /// Values: "StartMarker", "EndMarker", "Date", "Time", "Decimal-WithUnit",
        /// "Integer", "String", "Empty", "Reserved"
        /// </summary>
        public string FieldType { get; set; }

        /// <summary>
        /// Gets or sets the action to perform on this field.
        /// Values: "Parse" (extract data), "Skip" (ignore), "Validate" (check but don't extract)
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the variance ratio (0.0 to 1.0) indicating how much values change.
        /// 0.0 = always same (constant/marker), 1.0 = always different (unique ID)
        /// </summary>
        public double Variance { get; set; }

        #endregion

        #region UI Control Properties

        /// <summary>
        /// Gets or sets whether to include this field in the JSON definition export.
        /// User can uncheck to exclude markers, empty lines, etc.
        /// </summary>
        public bool IncludeInDefinition { get; set; }

        /// <summary>
        /// Gets or sets whether this field is skipped in parsing.
        /// </summary>
        public bool IsSkipped { get; set; }

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

        #endregion

        #region Display Properties

        /// <summary>
        /// Gets a display string of sample values (first 5) for UI binding.
        /// User needs to see actual data to decide field names!
        /// </summary>
        public string SampleValuesDisplay
        {
            get
            {
                if (SampleValues == null || SampleValues.Count == 0)
                    return string.Empty;

                var samples = SampleValues.Take(5);
                return string.Join(", ", samples);
            }
        }

        #endregion
    }
}
