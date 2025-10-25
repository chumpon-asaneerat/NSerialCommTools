#region Using

using System;
using System.Collections.Generic;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Models
{
    /// <summary>
    /// Results from pattern detection and analysis.
    /// Contains all detected patterns, fields, relationships, and validation rules.
    /// </summary>
    public class AnalysisResult
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public AnalysisResult()
        {
            Delimiters = new List<DelimiterInfo>();
            Fields = new List<FieldInfo>();
            Relationships = new List<FieldRelationship>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the detected terminator information.
        /// </summary>
        public TerminatorInfo Terminator { get; set; }

        /// <summary>
        /// Gets or sets the list of detected delimiters.
        /// </summary>
        public List<DelimiterInfo> Delimiters { get; set; }

        /// <summary>
        /// Gets or sets the list of detected fields.
        /// </summary>
        public List<FieldInfo> Fields { get; set; }

        /// <summary>
        /// Gets or sets the total number of messages analyzed.
        /// </summary>
        public int MessageCount { get; set; }

        /// <summary>
        /// Gets or sets the overall confidence (0.0 to 1.0) in the analysis.
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Gets or sets the suggested parsing strategy.
        /// </summary>
        public string SuggestedStrategy { get; set; }

        /// <summary>
        /// Gets or sets the detected protocol type.
        /// </summary>
        public ProtocolType ProtocolType { get; set; }

        /// <summary>
        /// Gets or sets the selected parsing strategy.
        /// Values: "Delimiter", "Frame", "StateMachine", "Position", "Content"
        /// </summary>
        public string SelectedStrategy { get; set; }

        /// <summary>
        /// Gets or sets the confidence score (0.0 to 1.0) for the selected strategy.
        /// </summary>
        public double StrategyConfidence { get; set; }

        /// <summary>
        /// Gets or sets the list of detected field relationships.
        /// Includes Date+Time combinations, compound field splits, and formula calculations.
        /// </summary>
        public List<FieldRelationship> Relationships { get; set; }

        /// <summary>
        /// Gets or sets the detected text encoding (ASCII, UTF-8, UTF-16, etc.).
        /// Used for byte-to-string conversions during parsing.
        /// </summary>
        public System.Text.Encoding DetectedEncoding { get; set; }

        /// <summary>
        /// Gets or sets the detected encoding name (e.g., "ASCII", "UTF-8", "UTF-16LE").
        /// Human-readable encoding identifier for display and JSON export.
        /// </summary>
        public string EncodingName { get; set; }

        /// <summary>
        /// Gets or sets the confidence score (0.0 to 1.0) for the detected encoding.
        /// 1.0 = BOM detected, 0.95+ = high confidence from pattern analysis.
        /// </summary>
        public double EncodingConfidence { get; set; }

        #endregion
    }

    /// <summary>
    /// Enumeration of protocol types.
    /// Binary-first terminology: Frame → Segment → Field hierarchy.
    /// </summary>
    public enum ProtocolType
    {
        /// <summary>
        /// Unknown protocol type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Single-segment frame protocol (flat structure).
        /// Each frame contains exactly one segment.
        /// Example: Simple CSV, single-record messages.
        /// </summary>
        SingleSegment,

        /// <summary>
        /// Multi-segment frame protocol (hierarchical structure).
        /// Each frame contains multiple segments.
        /// Example: JIK6CAB (10 segments per frame), multi-record messages.
        /// </summary>
        MultiSegment,

        /// <summary>
        /// Command-response protocol.
        /// Request-reply pattern with paired messages.
        /// </summary>
        CommandResponse,

        /// <summary>
        /// Binary protocol with control characters.
        /// Uses binary terminators (STX, ETX, etc.) not text terminators.
        /// Example: Protocols with 0x02 (STX), 0x03 (ETX) markers.
        /// </summary>
        Binary
    }
}
