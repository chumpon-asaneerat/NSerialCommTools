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
            ValidationRules = new List<ValidationRule>();
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
        /// Gets or sets the list of generated validation rules.
        /// Includes range validations, formula validations, and relationship constraints.
        /// </summary>
        public List<ValidationRule> ValidationRules { get; set; }

        #endregion
    }

    /// <summary>
    /// Enumeration of protocol types.
    /// </summary>
    public enum ProtocolType
    {
        /// <summary>
        /// Unknown protocol type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Single-line streaming protocol.
        /// </summary>
        SingleLine,

        /// <summary>
        /// Multi-line package protocol.
        /// </summary>
        MultiLine,

        /// <summary>
        /// Command-response protocol.
        /// </summary>
        CommandResponse
    }
}
