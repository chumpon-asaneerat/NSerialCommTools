#region Using

using System;
using System.Collections.Generic;
using NLib.Serial.Protocol.Analyzer.Analyzers;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Results of pattern analysis
    /// </summary>
    public class AnalysisResult
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public AnalysisResult()
        {
            Delimiters = new List<DelimiterInfo>();
            Fields = new List<FieldInfo>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the terminator information
        /// </summary>
        public TerminatorInfo Terminator { get; set; }

        /// <summary>
        /// Gets or sets the list of detected delimiters
        /// </summary>
        public List<DelimiterInfo> Delimiters { get; set; }

        /// <summary>
        /// Gets or sets the list of detected fields
        /// </summary>
        public List<FieldInfo> Fields { get; set; }

        /// <summary>
        /// Gets or sets the total message count analyzed
        /// </summary>
        public int MessageCount { get; set; }

        /// <summary>
        /// Gets or sets the average message length
        /// </summary>
        public int AverageLength { get; set; }

        /// <summary>
        /// Gets or sets the overall confidence level (0-100%)
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Gets or sets the recommended parsing strategy
        /// </summary>
        public string RecommendedStrategy { get; set; }

        /// <summary>
        /// Gets or sets the protocol type (streaming, command-response, etc.)
        /// </summary>
        public string ProtocolType { get; set; }

        /// <summary>
        /// Gets or sets the package size (for multi-line packages)
        /// </summary>
        public int PackageSize { get; set; }

        /// <summary>
        /// Gets or sets the start marker (for package-based protocols)
        /// </summary>
        public string StartMarker { get; set; }

        /// <summary>
        /// Gets or sets the end marker (for package-based protocols)
        /// </summary>
        public string EndMarker { get; set; }

        /// <summary>
        /// Gets or sets the package information (for package-based protocols)
        /// </summary>
        public PackageInfo PackageInfo { get; set; }

        #endregion
    }
}
