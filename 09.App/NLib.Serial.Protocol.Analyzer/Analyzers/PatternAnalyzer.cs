#region Using

using System;
using System.Linq;
using NLib.Serial.ProtocolAnalyzer.Models;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Analyzers
{
    /// <summary>
    /// PASS 3: Pattern Analysis Phase
    /// Main pattern analyzer that coordinates field analysis and relationship detection.
    /// Uses pre-detected patterns from Pass 1 (DetectionResult) - NO redundant detection.
    /// Based on Document: REFACTOR-TODO-Two-Pass-Architecture.md
    /// </summary>
    public class PatternAnalyzer
    {
        #region Internal Variables

        private readonly DelimiterDetector _delimiterDetector;
        private readonly FieldAnalyzer _fieldAnalyzer;
        private readonly RelationshipDetector _relationshipDetector;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public PatternAnalyzer()
        {
            _delimiterDetector = new DelimiterDetector();
            _fieldAnalyzer = new FieldAnalyzer();
            _relationshipDetector = new RelationshipDetector();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Analyzes the log data using pre-detected patterns (TWO-PASS ARCHITECTURE).
        /// This is PASS 3 - uses patterns from Pass 1, NO redundant detection.
        /// </summary>
        /// <param name="logData">Extracted messages from Pass 2 (MessageExtractor).</param>
        /// <param name="detection">Detection results from Pass 1 (ProtocolDetector).</param>
        /// <returns>Complete analysis results with fields and relationships.</returns>
        public AnalysisResult Analyze(LogData logData, DetectionResult detection)
        {
            if (logData == null || logData.Messages.Count == 0)
            {
                return new AnalysisResult
                {
                    MessageCount = 0,
                    Confidence = 0.0
                };
            }

            if (detection == null)
            {
                throw new ArgumentNullException(nameof(detection),
                    "DetectionResult is required. Run ProtocolDetector.DetectProtocolStructure() first (PASS 1).");
            }

            var result = new AnalysisResult
            {
                MessageCount = logData.MessageCount
            };

            // Copy pre-detected encoding from Pass 1 (NO redundant detection)
            result.DetectedEncoding = detection.DetectedEncoding;
            result.EncodingName = detection.EncodingName;
            result.EncodingConfidence = detection.EncodingConfidence;

            // Copy pre-detected terminator from Pass 1 (NO redundant detection)
            result.Terminator = detection.SegmentTerminator; // Use segment terminator for line parsing

            // Detect delimiters (still needed for backward compatibility)
            // TODO: This could also be replaced with detection.FieldDelimiter in future
            result.Delimiters = _delimiterDetector.Detect(logData);

            // If we have a detected field delimiter from Pass 1, create DelimiterInfo for it
            DelimiterInfo bestDelimiter = null;
            if (detection.FieldDelimiter != null && detection.FieldDelimiter.Confidence >= 0.5)
            {
                // Use detected field delimiter from Pass 1
                bestDelimiter = CreateDelimiterInfoFromTerminator(detection.FieldDelimiter);
            }
            else if (result.Delimiters != null && result.Delimiters.Count > 0)
            {
                // Fall back to delimiter detector results
                bestDelimiter = result.Delimiters.OrderByDescending(d => d.Confidence).FirstOrDefault();
            }

            // Analyze fields using detected encoding, segment terminator, and field delimiter
            if (bestDelimiter != null)
            {
                result.Fields = _fieldAnalyzer.Analyze(
                    logData,
                    bestDelimiter,
                    detection.DetectedEncoding,
                    detection.SegmentTerminator
                );
            }
            else
            {
                // No delimiter - try field analysis without delimiter
                result.Fields = _fieldAnalyzer.Analyze(
                    logData,
                    null,
                    detection.DetectedEncoding,
                    detection.SegmentTerminator
                );
            }

            // Detect relationships between fields (compound fields, etc.)
            if (result.Fields != null && result.Fields.Count > 0)
            {
                result.Relationships = _relationshipDetector.DetectRelationships(result.Fields);
            }

            // Determine protocol type based on detected structure
            result.ProtocolType = MapProtocolStructureToType(detection.Structure);

            // Calculate overall confidence
            result.Confidence = CalculateOverallConfidence(result, detection);

            // Suggest parsing strategy based on detected structure
            result.SuggestedStrategy = GenerateParsingStrategy(detection, bestDelimiter);

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a DelimiterInfo from a TerminatorInfo (for field delimiter).
        /// </summary>
        private DelimiterInfo CreateDelimiterInfoFromTerminator(TerminatorInfo terminator)
        {
            if (terminator == null || terminator.Bytes == null || terminator.Bytes.Length == 0)
                return null;

            // Convert byte to character (assumes single-byte delimiter)
            char delimiterChar = terminator.Bytes.Length == 1
                ? (char)terminator.Bytes[0]
                : ' '; // Default to space if multi-byte

            return new DelimiterInfo
            {
                Character = delimiterChar,
                Frequency = terminator.Frequency,
                Confidence = terminator.Confidence
            };
        }

        /// <summary>
        /// Maps DetectionResult.Structure to ProtocolType enum.
        /// Uses binary-first terminology: SingleSegment, MultiSegment, Binary.
        /// </summary>
        private ProtocolType MapProtocolStructureToType(ProtocolStructure structure)
        {
            switch (structure)
            {
                case ProtocolStructure.FlatDelimited:
                case ProtocolStructure.FlatFixedPosition:
                    // Single-segment frame - flat structure
                    return ProtocolType.SingleSegment;

                case ProtocolStructure.SegmentedDelimited:
                case ProtocolStructure.SegmentedFixedPosition:
                    // Multi-segment frame - hierarchical structure
                    return ProtocolType.MultiSegment;

                case ProtocolStructure.Binary:
                    // Binary protocol with control characters
                    return ProtocolType.Binary;

                default:
                    return ProtocolType.Unknown;
            }
        }

        /// <summary>
        /// Calculates overall confidence from all detection components.
        /// </summary>
        private double CalculateOverallConfidence(AnalysisResult result, DetectionResult detection)
        {
            // Use overall confidence from Pass 1 (primary)
            double detectionConfidence = detection.OverallConfidence;

            // Add field analysis confidence if available
            double fieldConf = result.Fields != null && result.Fields.Any()
                ? result.Fields.Average(f => f.Confidence)
                : 0.5;

            // Weighted average: Detection (70%), Field Analysis (30%)
            return (detectionConfidence * 0.7) + (fieldConf * 0.3);
        }

        /// <summary>
        /// Generates parsing strategy description based on detected structure.
        /// </summary>
        private string GenerateParsingStrategy(DetectionResult detection, DelimiterInfo delimiter)
        {
            switch (detection.Structure)
            {
                case ProtocolStructure.FlatDelimited:
                    return delimiter != null
                        ? $"Single-line delimited (split by '{delimiter.Character}')"
                        : "Single-line delimited";

                case ProtocolStructure.SegmentedDelimited:
                    return delimiter != null
                        ? $"Multi-segment delimited (segments by {detection.SegmentTerminator?.DisplayName}, fields by '{delimiter.Character}')"
                        : $"Multi-segment (segments by {detection.SegmentTerminator?.DisplayName})";

                case ProtocolStructure.FlatFixedPosition:
                    return "Single-line fixed-position";

                case ProtocolStructure.SegmentedFixedPosition:
                    return $"Multi-segment fixed-position (segments by {detection.SegmentTerminator?.DisplayName})";

                case ProtocolStructure.Binary:
                    return "Binary protocol";

                default:
                    return "Unknown structure - manual analysis required";
            }
        }

        #endregion
    }
}
