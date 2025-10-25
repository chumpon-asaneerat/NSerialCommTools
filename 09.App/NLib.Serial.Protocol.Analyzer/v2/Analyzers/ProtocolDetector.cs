#region Using

using NLib.Serial.ProtocolAnalyzer.Models;
using NLib.Serial.ProtocolAnalyzer.Parsers;
using System;
using System.Collections.Generic;

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
        /// <param name="isSingleMessage">User hint: true if file contains single message/frame, false for multi-message. Null for auto-detect.</param>
        /// <returns>Complete detection results including encoding, terminators, markers, and structure.</returns>
        public DetectionResult DetectProtocolStructure(byte[] rawBytes, bool? isSingleMessage = null)
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
                    encodingInfo.Encoding,
                    isSingleMessage
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
            catch (Exception)
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
    /// Detects frame markers like STX, ETX, or custom start/end markers.
    /// Basic implementation that looks for common ASCII markers at frame boundaries.
    /// </summary>
    internal class MarkerDetector
    {
        public MarkerDetectionResult DetectMarkers(byte[] rawBytes, TerminatorHierarchyResult terminators)
        {
            var result = new MarkerDetectionResult
            {
                StartMarker = null,
                EndMarker = null
            };

            // Only detect markers if we have a frame terminator
            if (terminators == null || terminators.FrameTerminator == null)
                return result;

            byte[] frameTermBytes = terminators.FrameTerminator.Bytes;

            // Find all positions where frames might start (after frame terminators)
            var frameStartPositions = new List<int> { 0 }; // File start is always a potential frame start

            for (int i = 0; i <= rawBytes.Length - frameTermBytes.Length; i++)
            {
                if (BytesMatch(rawBytes, i, frameTermBytes))
                {
                    int nextPos = i + frameTermBytes.Length;
                    if (nextPos < rawBytes.Length)
                    {
                        frameStartPositions.Add(nextPos);
                    }
                }
            }

            // Look for common start markers at frame boundaries
            result.StartMarker = DetectStartMarker(rawBytes, frameStartPositions);
            result.EndMarker = DetectEndMarker(rawBytes, frameTermBytes);

            return result;
        }

        private FrameMarkerInfo DetectStartMarker(byte[] rawBytes, List<int> frameStartPositions)
        {
            // Look for printable ASCII sequences that appear at frame starts
            // Common patterns: ^, STX (0x02), SOH (0x01), or custom markers like "^KJIK"

            if (frameStartPositions.Count < 2)
                return null; // Need at least 2 frames to detect a pattern

            // Check for consistent byte sequences at frame starts (up to 10 bytes)
            for (int len = 3; len <= 10; len++)
            {
                byte[] candidateMarker = null;
                int matchCount = 0;

                foreach (int pos in frameStartPositions)
                {
                    if (pos + len > rawBytes.Length)
                        continue;

                    byte[] sequence = new byte[len];
                    Array.Copy(rawBytes, pos, sequence, 0, len);

                    if (candidateMarker == null)
                    {
                        candidateMarker = sequence;
                        matchCount = 1;
                    }
                    else if (BytesEqual(sequence, candidateMarker))
                    {
                        matchCount++;
                    }
                }

                // If this sequence appears at 80%+ of frame starts, it's likely a start marker
                if (candidateMarker != null && matchCount >= frameStartPositions.Count * 0.8)
                {
                    return new FrameMarkerInfo
                    {
                        Bytes = candidateMarker,
                        DisplayName = "StartMarker",
                        Confidence = (double)matchCount / frameStartPositions.Count
                    };
                }
            }

            return null;
        }

        private FrameMarkerInfo DetectEndMarker(byte[] rawBytes, byte[] frameTerminator)
        {
            // Look for consistent sequences that appear BEFORE the frame terminator
            // Common patterns: ETX (0x03), EOT (0x04), or custom markers like "~P1"

            var endPositions = new List<int>();

            // Find all frame terminator positions
            for (int i = 0; i <= rawBytes.Length - frameTerminator.Length; i++)
            {
                if (BytesMatch(rawBytes, i, frameTerminator))
                {
                    endPositions.Add(i);
                }
            }

            if (endPositions.Count < 2)
                return null;

            // Check for consistent byte sequences before frame terminators (up to 10 bytes)
            for (int len = 2; len <= 10; len++)
            {
                byte[] candidateMarker = null;
                int matchCount = 0;

                foreach (int termPos in endPositions)
                {
                    int markerStart = termPos - len;
                    if (markerStart < 0)
                        continue;

                    byte[] sequence = new byte[len];
                    Array.Copy(rawBytes, markerStart, sequence, 0, len);

                    if (candidateMarker == null)
                    {
                        candidateMarker = sequence;
                        matchCount = 1;
                    }
                    else if (BytesEqual(sequence, candidateMarker))
                    {
                        matchCount++;
                    }
                }

                // If this sequence appears at 80%+ of frame ends, it's likely an end marker
                if (candidateMarker != null && matchCount >= endPositions.Count * 0.8)
                {
                    return new FrameMarkerInfo
                    {
                        Bytes = candidateMarker,
                        DisplayName = "EndMarker",
                        Confidence = (double)matchCount / endPositions.Count
                    };
                }
            }

            return null;
        }

        private bool BytesMatch(byte[] data, int position, byte[] pattern)
        {
            if (position + pattern.Length > data.Length)
                return false;

            for (int i = 0; i < pattern.Length; i++)
            {
                if (data[position + i] != pattern[i])
                    return false;
            }
            return true;
        }

        private bool BytesEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
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
