#region Using

using System;
using System.Collections.Generic;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Models
{
    /// <summary>
    /// Results from pattern detection and analysis.
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
