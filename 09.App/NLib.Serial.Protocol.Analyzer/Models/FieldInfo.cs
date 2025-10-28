using System;

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Represents a field definition within a protocol
    /// Used for defining how to parse specific data fields from packages
    /// </summary>
    public class FieldInfo
    {
        /// <summary>
        /// Field name (must be valid C# identifier)
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Start index within the segment/package (0-based)
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Length of the field in characters/bytes
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Data type of the field
        /// </summary>
        public DataType DataType { get; set; }

        /// <summary>
        /// Encoding type (for string fields)
        /// </summary>
        public EncodingType EncodingType { get; set; }

        /// <summary>
        /// Endian type (for numeric fields)
        /// </summary>
        public EndianType EndianType { get; set; }

        /// <summary>
        /// Format string for parsing/display (optional)
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Sample value from analysis
        /// </summary>
        public string SampleValue { get; set; }

        /// <summary>
        /// Segment index (if field belongs to a specific segment in multi-segment package)
        /// -1 means applies to whole package or single-package protocol
        /// </summary>
        public int SegmentIndex { get; set; }

        /// <summary>
        /// Description or notes about this field
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public FieldInfo()
        {
            FieldName = string.Empty;
            StartIndex = 0;
            Length = 0;
            DataType = DataType.String;
            EncodingType = EncodingType.ASCII;
            EndianType = EndianType.LittleEndian;
            Format = string.Empty;
            SampleValue = string.Empty;
            SegmentIndex = -1;
            Description = string.Empty;
        }

        /// <summary>
        /// Constructor with basic parameters
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="startIndex">Start index</param>
        /// <param name="length">Length</param>
        /// <param name="dataType">Data type</param>
        public FieldInfo(string fieldName, int startIndex, int length, DataType dataType)
        {
            FieldName = fieldName ?? string.Empty;
            StartIndex = startIndex;
            Length = length;
            DataType = dataType;
            EncodingType = EncodingType.ASCII;
            EndianType = EndianType.LittleEndian;
            Format = string.Empty;
            SampleValue = string.Empty;
            SegmentIndex = -1;
            Description = string.Empty;
        }

        /// <summary>
        /// Validates if the field name is a valid C# identifier
        /// </summary>
        /// <returns>True if valid</returns>
        public bool IsValidFieldName()
        {
            if (string.IsNullOrEmpty(FieldName))
                return false;

            // Must start with letter or underscore
            if (!char.IsLetter(FieldName[0]) && FieldName[0] != '_')
                return false;

            // Must contain only letters, digits, underscores
            foreach (char c in FieldName)
            {
                if (!char.IsLetterOrDigit(c) && c != '_')
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a display string for this field
        /// </summary>
        /// <returns>Display string</returns>
        public string GetDisplayString()
        {
            return $"{FieldName} [{StartIndex}:{Length}] {DataType}";
        }
    }
}
