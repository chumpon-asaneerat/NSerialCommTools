#region Using

using System;
using System.Collections.Generic;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Models
{
    /// <summary>
    /// Complete protocol definition for JSON export.
    /// This is the root model that gets serialized to JSON and used by NTerminal&lt;T&gt; and NDevice&lt;T&gt;.
    /// </summary>
    public class ProtocolDefinition
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ProtocolDefinition()
        {
            Fields = new List<FieldInfo>();
            Relationships = new List<FieldRelationship>();
            Version = "1.0";
            GeneratedDate = DateTime.Now;
            Encoding = "ASCII";
        }

        #endregion

        #region Device Information

        /// <summary>
        /// Gets or sets the device name.
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Gets or sets the definition version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the date when this definition was generated.
        /// </summary>
        public DateTime GeneratedDate { get; set; }

        /// <summary>
        /// Gets or sets the description or notes.
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region Protocol Information

        /// <summary>
        /// Gets or sets the text encoding (ASCII, UTF-8, etc.).
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// Gets or sets the message type.
        /// Values: "single-line", "multi-line-frame", "multi-line-block", "variable-length"
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// Gets or sets the frame start marker pattern (for multi-line-frame messages).
        /// Example: "^KJIK\\d{3}\\r\\n"
        /// </summary>
        public string FrameStart { get; set; }

        /// <summary>
        /// Gets or sets the frame end marker pattern (for multi-line-frame messages).
        /// Example: "~P1\\r\\n"
        /// </summary>
        public string FrameEnd { get; set; }

        /// <summary>
        /// Gets or sets the entry terminator (line terminator within messages).
        /// Example: "\\r\\n" or "\\r"
        /// </summary>
        public string EntryTerminator { get; set; }

        #endregion

        #region Fields and Relationships

        /// <summary>
        /// Gets or sets the list of field definitions.
        /// Only includes fields where IncludeInDefinition = true.
        /// </summary>
        public List<FieldInfo> Fields { get; set; }

        /// <summary>
        /// Gets or sets the list of field relationships.
        /// Includes Date+Time combinations, compound field splits, formula calculations.
        /// </summary>
        public List<FieldRelationship> Relationships { get; set; }


        #endregion
    }
}
