#region Using

using NLib.Serial.ProtocolAnalyzer.Models;
using NLib.Serial.ProtocolAnalyzer.Parsers;
using System;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Analyzers
{
    /// <summary>
    /// PASS 1: Protocol Detection Phase
    /// Orchestrates all detection activities on raw bytes BEFORE any splitting occurs.
    /// Detects encoding, terminator hierarchy (Frame/Segment/Field), and frame markers.
    /// Output (DetectionResult) is used by MessageExtractor in Pass 2.
    /// Based on Document: REFACTOR-TODO-Two-Pass-Architecture.md
    /// </summary>
    public class ProtocolDetector
    {
        #region Private Fields

        private readonly EncodingDetector _encodingDetector;
        private readonly TerminatorDetector _terminatorDetector;
        private readonly MarkerDetector _markerDetector;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ProtocolDetector class.
        /// </summary>
        public ProtocolDetector()
        {
            _encodingDetector = new EncodingDetector();
            _terminatorDetector = new TerminatorDetector();
            // MarkerDetector will be created later (not critical for Phase 1)
            _markerDetector = null; // TODO: Implement MarkerDetector in future phase
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Detects all protocol structure information from raw bytes (UNSPLIT data).
        /// This is PASS 1 - Detection Phase.
        /// NO SPLITTING happens here - only analysis of byte patterns.
        /// </summary>
        /// <param name="rawBytes">The entire file content as raw bytes (unsplit).</param>
        /// <returns>Complete detection results including encoding, terminators, markers, and structure.</returns>
        public DetectionResult DetectProtocolStructure(byte[] rawBytes)
        {
            if (rawBytes == null || rawBytes.Length == 0)
            {
                return CreateEmptyDetectionResult("No data provided");
            }

            var result = new DetectionResult();

            try
            {
                // STEP 1: Detect Encoding (FIRST - needed for text analysis)
                var encodingInfo = _encodingDetector.DetectEncoding(rawBytes);
                result.DetectedEncoding = encodingInfo.Encoding;
                result.EncodingName = encodingInfo.EncodingName;
                result.EncodingConfidence = encodingInfo.Confidence;

                // STEP 2: Detect Terminator Hierarchy (Frame → Segment → Field)
                // This is the CRITICAL step - must detect ALL levels before splitting
                var terminatorHierarchy = _terminatorDetector.DetectTerminatorHierarchy(
                    rawBytes,
                    encodingInfo.Encoding
                );

                // Populate terminator hierarchy in result
                result.FrameTerminator = terminatorHierarchy.FrameTerminator;
                result.SegmentTerminator = terminatorHierarchy.SegmentTerminator;
                result.FieldDelimiter = terminatorHierarchy.FieldDelimiter;

                // Set structure flags
                result.HasSegmentStructure = (terminatorHierarchy.SegmentTerminator != null &&
                                              terminatorHierarchy.SegmentTerminator.Confidence >= 0.5);
                result.HasFieldDelimiter = (terminatorHierarchy.FieldDelimiter != null &&
                                           terminatorHierarchy.FieldDelimiter.Confidence >= 0.5);

                // STEP 3: Detect Frame Markers (Optional - not all protocols have markers)
                if (_markerDetector != null)
                {
                    var markers = _markerDetector.DetectMarkers(rawBytes, terminatorHierarchy);
                    result.StartMarker = markers.StartMarker;
                    result.EndMarker = markers.EndMarker;
                }

                // STEP 4: Determine Protocol Structure Type
                result.Structure = DetermineProtocolStructure(result);

                // STEP 5: Calculate Overall Confidence
                result.OverallConfidence = CalculateOverallConfidence(result);
            }
            catch (Exception ex)
            {
                // If detection fails, return safe defaults with low confidence
                result.OverallConfidence = 0.0;
                // Log error: ex.Message
                // For now, defaults from constructor are used
            }

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the overall protocol structure based on detected terminators.
        /// </summary>
        private ProtocolStructure DetermineProtocolStructure(DetectionResult result)
        {
            bool hasFrame = (result.FrameTerminator != null && result.FrameTerminator.Confidence >= 0.5);
            bool hasSegment = result.HasSegmentStructure;
            bool hasField = result.HasFieldDelimiter;

            // Check for binary protocol indicators
            bool isBinary = false;
            if (result.FrameTerminator != null)
            {
                // Binary protocols often use control characters as terminators
                byte[] termBytes = result.FrameTerminator.Bytes;
                if (termBytes != null && termBytes.Length > 0)
                {
                    // Check for common binary markers: STX (0x02), ETX (0x03), etc.
                    if (termBytes[0] == 0x02 || termBytes[0] == 0x03 ||
                        termBytes[0] == 0x1E || termBytes[0] == 0x1F)
                    {
                        isBinary = true;
                    }
                }
            }

            if (isBinary)
            {
                return ProtocolStructure.Binary;
            }

            // Determine structure based on hierarchy levels
            if (hasSegment && hasField)
            {
                // Multi-segment frames with field delimiters (e.g., JIK6CAB)
                return ProtocolStructure.SegmentedDelimited;
            }
            else if (hasSegment && !hasField)
            {
                // Multi-segment frames with fixed positions
                return ProtocolStructure.SegmentedFixedPosition;
            }
            else if (!hasSegment && hasField)
            {
                // Single-segment frames with field delimiters (e.g., simple CSV)
                return ProtocolStructure.FlatDelimited;
            }
            else if (!hasSegment && !hasField)
            {
                // Single-segment frames with fixed positions
                return ProtocolStructure.FlatFixedPosition;
            }

            return ProtocolStructure.Unknown;
        }

        /// <summary>
        /// Calculates the overall confidence score from all detection confidences.
        /// </summary>
        private double CalculateOverallConfidence(DetectionResult result)
        {
            double totalConfidence = 0.0;
            int componentCount = 0;

            // Encoding confidence (always present)
            totalConfidence += result.EncodingConfidence;
            componentCount++;

            // Frame terminator confidence
            if (result.FrameTerminator != null)
            {
                totalConfidence += result.FrameTerminator.Confidence;
                componentCount++;
            }

            // Segment terminator confidence (if present)
            if (result.SegmentTerminator != null && result.HasSegmentStructure)
            {
                totalConfidence += result.SegmentTerminator.Confidence;
                componentCount++;
            }

            // Field delimiter confidence (if present)
            if (result.FieldDelimiter != null && result.HasFieldDelimiter)
            {
                totalConfidence += result.FieldDelimiter.Confidence;
                componentCount++;
            }

            // Average of all component confidences
            return componentCount > 0 ? totalConfidence / componentCount : 0.0;
        }

        /// <summary>
        /// Creates an empty detection result with safe defaults.
        /// </summary>
        private DetectionResult CreateEmptyDetectionResult(string reason)
        {
            var result = new DetectionResult();
            // Constructor already sets safe defaults (ASCII, Unknown structure, 0.0 confidence)
            return result;
        }

        #endregion
    }

    #region Helper Classes (Placeholders)

    /// <summary>
    /// Placeholder for MarkerDetector (to be implemented in future phase).
    /// Detects frame markers like STX, ETX, or custom start/end markers.
    /// </summary>
    internal class MarkerDetector
    {
        public MarkerDetectionResult DetectMarkers(byte[] rawBytes, TerminatorHierarchyResult terminators)
        {
            // TODO: Implement marker detection
            return new MarkerDetectionResult
            {
                StartMarker = null,
                EndMarker = null
            };
        }
    }

    /// <summary>
    /// Result from marker detection.
    /// </summary>
    internal class MarkerDetectionResult
    {
        public FrameMarkerInfo StartMarker { get; set; }
        public FrameMarkerInfo EndMarker { get; set; }
    }

    /// <summary>
    /// Result from terminator hierarchy detection.
    /// Returned by TerminatorDetector.DetectTerminatorHierarchy().
    /// </summary>
    public class TerminatorHierarchyResult
    {
        public TerminatorInfo FrameTerminator { get; set; }
        public TerminatorInfo SegmentTerminator { get; set; }
        public TerminatorInfo FieldDelimiter { get; set; }
    }

    #endregion
}
