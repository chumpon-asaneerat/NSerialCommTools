#region Using

using System;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Models
{
    /// <summary>
    /// Information about a detected message terminator.
    /// </summary>
    public class TerminatorInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the terminator as a string (e.g., "\r\n").
        /// </summary>
        public string String { get; set; }

        /// <summary>
        /// Gets or sets the terminator as a byte array.
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// Gets or sets the display name (e.g., "CRLF", "LF", "CR").
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the frequency (0.0 to 1.0) of this terminator appearing.
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// Gets or sets the confidence (0.0 to 1.0) that this is the correct terminator.
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Gets or sets the type of terminator (Frame, Segment, or Field).
        /// Indicates the level in the terminator hierarchy.
        /// </summary>
        public TerminatorType Type { get; set; }

        /// <summary>
        /// Gets or sets the hierarchy level (1=Frame, 2=Segment, 3=Field).
        /// Lower number = higher level in hierarchy (Frame is top level).
        /// </summary>
        public int Level { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines the type of terminator in the hierarchy.
    /// Binary-first thinking: Frame → Segment → Field
    /// </summary>
    public enum TerminatorType
    {
        /// <summary>
        /// Unknown or not set.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Frame terminator - separates complete messages/frames (TOP level).
        /// Examples: Double CRLF, ETX marker, frame boundary bytes.
        /// Level = 1 (highest level)
        /// </summary>
        Frame = 1,

        /// <summary>
        /// Segment terminator - separates segments/chunks within a frame (MIDDLE level).
        /// Examples: Single CRLF, LF, record separator.
        /// Level = 2
        /// NOTE: NOT "line" - that's text-thinking! Segments are binary chunks.
        /// </summary>
        Segment = 2,

        /// <summary>
        /// Field delimiter - separates fields within a segment (BOTTOM level).
        /// Examples: Space, comma, tab, unit separator.
        /// Level = 3 (lowest level)
        /// </summary>
        Field = 3
    }
}
