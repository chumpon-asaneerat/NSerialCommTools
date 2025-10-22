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
            ValidationRules = new List<ValidationRule>();
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

        /// <summary>
        /// Gets or sets the list of validation rules.
        /// Includes range validations, formula validations, relationship constraints.
        /// </summary>
        public List<ValidationRule> ValidationRules { get; set; }

        #endregion
    }

    /// <summary>
    /// Device information.
    /// </summary>
    public class DeviceInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the device name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer name.
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the device model.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date this definition was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the analysis confidence score.
        /// </summary>
        public double AnalysisConfidence { get; set; }

        #endregion
    }

    /// <summary>
    /// Protocol specification.
    /// </summary>
    public class ProtocolSpec
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ProtocolSpec()
        {
            Fields = new List<FieldSpec>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the protocol type (e.g., "streaming", "request-response").
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the message format (e.g., "text", "binary").
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the text encoding (e.g., "ASCII", "UTF-8").
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// Gets or sets the message terminator.
        /// </summary>
        public string Terminator { get; set; }

        /// <summary>
        /// Gets or sets the list of field specifications.
        /// </summary>
        public List<FieldSpec> Fields { get; set; }

        #endregion
    }

    /// <summary>
    /// Field specification.
    /// </summary>
    public class FieldSpec
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public FieldSpec()
        {
            AllowedValues = new List<string>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the field name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the field position (0-based index).
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the data type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the field description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the allowed values (if enumerated).
        /// </summary>
        public List<string> AllowedValues { get; set; }

        /// <summary>
        /// Gets or sets the confidence score for this field.
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Gets or sets whether this field is required.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets whether this field is constant.
        /// </summary>
        public bool IsConstant { get; set; }

        /// <summary>
        /// Gets or sets the unit of measurement (if applicable).
        /// </summary>
        public string Unit { get; set; }

        #endregion
    }

    /// <summary>
    /// Parsing specification.
    /// </summary>
    public class ParsingSpec
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ParsingSpec()
        {
            Steps = new List<string>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the parsing strategy name.
        /// </summary>
        public string Strategy { get; set; }

        /// <summary>
        /// Gets or sets the field delimiter.
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// Gets or sets the parsing steps.
        /// </summary>
        public List<string> Steps { get; set; }

        #endregion
    }

    /// <summary>
    /// Validation specification.
    /// </summary>
    public class ValidationSpec
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidationSpec()
        {
            Required = new List<string>();
            Constraints = new Dictionary<string, Constraint>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the list of required field names.
        /// </summary>
        public List<string> Required { get; set; }

        /// <summary>
        /// Gets or sets the constraints per field.
        /// </summary>
        public Dictionary<string, Constraint> Constraints { get; set; }

        #endregion
    }

    /// <summary>
    /// Field constraint.
    /// </summary>
    public class Constraint
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public object Min { get; set; }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public object Max { get; set; }

        /// <summary>
        /// Gets or sets the pattern (regex).
        /// </summary>
        public string Pattern { get; set; }

        #endregion
    }

    /// <summary>
    /// Test case for validation.
    /// </summary>
    public class TestCase
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCase()
        {
            ExpectedOutput = new Dictionary<string, object>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the test case name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the input byte array.
        /// </summary>
        public byte[] InputBytes { get; set; }

        /// <summary>
        /// Gets or sets the expected parsed output.
        /// </summary>
        public Dictionary<string, object> ExpectedOutput { get; set; }

        #endregion
    }
}
