#region Using

using System;
using System.Linq;
using NLib.Serial.ProtocolAnalyzer.Models;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Analyzers
{
    /// <summary>
    /// Main pattern analyzer that coordinates all sub-analyzers.
    /// </summary>
    public class PatternAnalyzer
    {
        #region Internal Variables

        private readonly TerminatorDetector _terminatorDetector;
        private readonly DelimiterDetector _delimiterDetector;
        private readonly FieldAnalyzer _fieldAnalyzer;
        private readonly RelationshipDetector _relationshipDetector;
        private readonly ValidationRuleGenerator _validationRuleGenerator;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public PatternAnalyzer()
        {
            _terminatorDetector = new TerminatorDetector();
            _delimiterDetector = new DelimiterDetector();
            _fieldAnalyzer = new FieldAnalyzer();
            _relationshipDetector = new RelationshipDetector();
            _validationRuleGenerator = new ValidationRuleGenerator();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Analyzes the log data and returns complete analysis results.
        /// </summary>
        public AnalysisResult Analyze(LogData logData)
        {
            if (logData == null || logData.Messages.Count == 0)
                return null;

            var result = new AnalysisResult
            {
                MessageCount = logData.MessageCount
            };

            // Detect terminator
            result.Terminator = _terminatorDetector.Detect(logData);

            // Detect delimiters
            result.Delimiters = _delimiterDetector.Detect(logData);

            // Analyze fields using the best delimiter
            var bestDelimiter = result.Delimiters.OrderByDescending(d => d.Confidence).FirstOrDefault();
            if (bestDelimiter != null)
            {
                result.Fields = _fieldAnalyzer.Analyze(logData, bestDelimiter);
            }

            // Detect relationships between fields (Phase 5)
            if (result.Fields != null && result.Fields.Count > 0)
            {
                result.Relationships = _relationshipDetector.DetectRelationships(result.Fields);
            }

            // Generate validation rules (Phase 6)
            if (result.Fields != null && result.Fields.Count > 0)
            {
                result.ValidationRules = _validationRuleGenerator.GenerateRules(result.Fields, result.Relationships);
            }

            // Determine protocol type
            result.ProtocolType = DetectProtocolType(result);

            // Calculate overall confidence
            result.Confidence = CalculateOverallConfidence(result);

            // Suggest parsing strategy
            result.SuggestedStrategy = bestDelimiter != null
                ? $"Split by '{bestDelimiter.Character}'"
                : "Line-based parsing";

            return result;
        }

        #endregion

        #region Private Methods

        private ProtocolType DetectProtocolType(AnalysisResult result)
        {
            // Simple heuristic based on message structure
            // If we have many fields (lines), it's likely a multi-line package
            if (result.Fields.Count > 5)
            {
                return ProtocolType.MultiLine;
            }
            else if (result.Fields.Count > 0)
            {
                return ProtocolType.SingleLine;
            }

            return ProtocolType.Unknown;
        }

        private double CalculateOverallConfidence(AnalysisResult result)
        {
            double terminatorConf = result.Terminator?.Confidence ?? 0.5;
            double delimiterConf = result.Delimiters.Any()
                ? result.Delimiters.Max(d => d.Confidence)
                : 0.5;
            double fieldConf = result.Fields.Any()
                ? result.Fields.Average(f => f.Confidence)
                : 0.5;

            return (terminatorConf + delimiterConf + fieldConf) / 3.0;
        }

        #endregion
    }
}
