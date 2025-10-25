#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLib.Serial.ProtocolAnalyzer.Models;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Analyzers
{
    /// <summary>
    /// Detects message terminators dynamically by analyzing message endings.
    /// NEW: Now supports two-pass architecture with DetectTerminatorHierarchy().
    /// </summary>
    public class TerminatorDetector
    {
        #region Public Methods

        /// <summary>
        /// NEW METHOD - TWO-PASS ARCHITECTURE
        /// Detects ALL terminator levels (Frame, Segment, Field) from raw bytes in ONE analysis.
        /// This is PASS 1 - Detection happens BEFORE any splitting.
        /// </summary>
        /// <param name="rawBytes">The entire file content as raw bytes (UNSPLIT).</param>
        /// <param name="encoding">Detected encoding (for text analysis if needed).</param>
        /// <param name="isSingleMessage">User hint: true=single message, false=multi-message, null=auto-detect.</param>
        /// <returns>Complete terminator hierarchy with confidence scores.</returns>
        public TerminatorHierarchyResult DetectTerminatorHierarchy(byte[] rawBytes, Encoding encoding, bool? isSingleMessage = null)
        {
            if (rawBytes == null || rawBytes.Length == 0)
            {
                return CreateEmptyHierarchy();
            }

            var result = new TerminatorHierarchyResult();

            // STEP 1: Find all repeating byte sequences (candidates)
            var candidates = FindRepeatingSequences(rawBytes);

            // STEP 2: Analyze each candidate to determine its characteristics
            var analyzed = AnalyzeCandidates(candidates, rawBytes);

            // STEP 3: Classify candidates into hierarchy levels
            // Use user hint if provided, otherwise auto-detect
            if (isSingleMessage == true)
            {
                // USER SAYS: Single message - don't detect frame terminator
                result.FrameTerminator = null; // No frame boundaries
                result.SegmentTerminator = ClassifySegmentTerminator(analyzed, rawBytes, null);
                result.FieldDelimiter = ClassifyFieldDelimiter(analyzed, rawBytes);
            }
            else if (isSingleMessage == false)
            {
                // USER SAYS: Multi-message - detect frame terminator
                result.FrameTerminator = ClassifyFrameTerminator(analyzed, rawBytes);
                result.SegmentTerminator = ClassifySegmentTerminator(analyzed, rawBytes, result.FrameTerminator);
                result.FieldDelimiter = ClassifyFieldDelimiter(analyzed, rawBytes);
            }
            else
            {
                // AUTO-DETECT (null) - try to figure it out
                result.FrameTerminator = ClassifyFrameTerminator(analyzed, rawBytes);
                result.SegmentTerminator = ClassifySegmentTerminator(analyzed, rawBytes, result.FrameTerminator);
                result.FieldDelimiter = ClassifyFieldDelimiter(analyzed, rawBytes);
            }

            return result;
        }

        /// <summary>
        /// OLD METHOD - LEGACY (for backward compatibility during transition)
        /// Detects the terminator by analyzing the endings of all messages.
        /// NOTE: This method works on already-split data (wrong approach).
        /// Use DetectTerminatorHierarchy() for new two-pass architecture.
        /// </summary>
        [Obsolete("Use DetectTerminatorHierarchy() instead - this method requires pre-split data")]
        public TerminatorInfo Detect(LogData logData)
        {
            if (logData == null || logData.Messages.Count < 2)
                return null;

            // Analyze the last N bytes of each message to find common endings
            var endingCandidates = new Dictionary<string, TerminatorCandidate>();

            // Try different ending lengths (1-4 bytes)
            for (int length = 1; length <= 4; length++)
            {
                foreach (var message in logData.Messages)
                {
                    if (message.Length < length)
                        continue;

                    // Extract last 'length' bytes
                    byte[] ending = new byte[length];
                    Array.Copy(message, message.Length - length, ending, 0, length);

                    string key = BitConverter.ToString(ending);

                    if (!endingCandidates.ContainsKey(key))
                    {
                        endingCandidates[key] = new TerminatorCandidate
                        {
                            Bytes = ending,
                            Count = 0
                        };
                    }

                    endingCandidates[key].Count++;
                }
            }

            // Find the most common ending
            var best = endingCandidates.Values
                .Where(c => c.Count >= logData.MessageCount * 0.5) // Appears in at least 50% of messages
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.Bytes.Length) // Prefer shorter terminators
                .FirstOrDefault();

            if (best == null)
                return null;

            double frequency = (double)best.Count / logData.MessageCount;

            return new TerminatorInfo
            {
                String = GetEscapedString(best.Bytes),
                Bytes = best.Bytes,
                DisplayName = GetDisplayName(best.Bytes),
                Frequency = frequency,
                Confidence = frequency
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Converts bytes to escaped string representation.
        /// </summary>
        private string GetEscapedString(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                if (b == 0x0D)
                    sb.Append("\\r");
                else if (b == 0x0A)
                    sb.Append("\\n");
                else if (b == 0x00)
                    sb.Append("\\0");
                else if (b == 0x09)
                    sb.Append("\\t");
                else if (b == 0x02)
                    sb.Append("STX");
                else if (b == 0x03)
                    sb.Append("ETX");
                else if (b >= 32 && b <= 126) // Printable ASCII
                    sb.Append((char)b);
                else
                    sb.Append($"\\x{b:X2}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets a human-readable display name for the terminator.
        /// </summary>
        private string GetDisplayName(byte[] bytes)
        {
            // Check for common patterns
            if (bytes.Length == 2 && bytes[0] == 0x0D && bytes[1] == 0x0A)
                return "CRLF";
            if (bytes.Length == 1 && bytes[0] == 0x0A)
                return "LF";
            if (bytes.Length == 1 && bytes[0] == 0x0D)
                return "CR";
            if (bytes.Length == 1 && bytes[0] == 0x00)
                return "NULL";
            if (bytes.Length == 1 && bytes[0] == 0x03)
                return "ETX";
            if (bytes.Length == 1 && bytes[0] == 0x02)
                return "STX";

            // Otherwise, show hex representation
            return $"0x{BitConverter.ToString(bytes).Replace("-", " 0x")}";
        }

        /// <summary>
        /// Creates an empty hierarchy result (fallback when detection fails).
        /// </summary>
        private TerminatorHierarchyResult CreateEmptyHierarchy()
        {
            return new TerminatorHierarchyResult
            {
                FrameTerminator = null,
                SegmentTerminator = null,
                FieldDelimiter = null
            };
        }

        /// <summary>
        /// STEP 1: Finds all repeating byte sequences in the raw data.
        /// Uses DYNAMIC discovery to find ANY repeating patterns (markers, terminators, delimiters).
        /// IMPORTANT: Check LONGER patterns first to avoid double-counting overlaps!
        /// </summary>
        private Dictionary<string, ByteSequenceCandidate> FindRepeatingSequences(byte[] rawBytes)
        {
            var candidates = new Dictionary<string, ByteSequenceCandidate>();
            var excludedPositions = new HashSet<int>(); // Positions already matched by longer patterns

            // PHASE 1: DYNAMIC DISCOVERY - Find ANY repeating patterns (3-16 bytes)
            // This discovers custom markers like ^KJIK000, ~P1, STX/ETX sequences, etc.
            var dynamicPatterns = DiscoverRepeatingPatterns(rawBytes, minLength: 3, maxLength: 16, minOccurrences: 2);

            // PHASE 2: Build complete pattern list (dynamic + predefined)
            var patternsToCheck = new List<byte[]>();

            // Add discovered patterns first (longest to shortest)
            patternsToCheck.AddRange(dynamicPatterns.OrderByDescending(p => p.Length));

            // Add common predefined patterns (for fallback)
            patternsToCheck.AddRange(new List<byte[]>
            {
                // 4-byte patterns
                new byte[] { 0x0D, 0x0A, 0x0D, 0x0A },  // Double CRLF

                // 2-byte patterns
                new byte[] { 0x0D, 0x0A },               // CRLF

                // 1-byte patterns
                new byte[] { 0x0A },                     // LF
                new byte[] { 0x0D },                     // CR
                new byte[] { 0x20 },                     // Space
                new byte[] { 0x09 },                     // Tab
                new byte[] { 0x2C },                     // Comma
                new byte[] { 0x3B },                     // Semicolon
                new byte[] { 0x7C },                     // Pipe |
                new byte[] { 0x00 },                     // NULL
                new byte[] { 0x02 },                     // STX
                new byte[] { 0x03 },                     // ETX
                new byte[] { 0x1E },                     // Record Separator
                new byte[] { 0x1F }                      // Unit Separator
            });

            // Search for each pattern (longest first)
            foreach (var pattern in patternsToCheck)
            {
                var positions = FindAllOccurrences(rawBytes, pattern, excludedPositions);
                if (positions.Count > 0)
                {
                    string key = BitConverter.ToString(pattern);

                    // Skip if already added
                    if (candidates.ContainsKey(key))
                        continue;

                    candidates[key] = new ByteSequenceCandidate
                    {
                        Bytes = pattern,
                        Positions = positions,
                        Count = positions.Count
                    };

                    // Mark these positions as used to avoid overlap
                    foreach (int pos in positions)
                    {
                        for (int offset = 0; offset < pattern.Length; offset++)
                        {
                            excludedPositions.Add(pos + offset);
                        }
                    }
                }
            }

            return candidates;
        }

        /// <summary>
        /// DYNAMIC PATTERN DISCOVERY: Finds repeating byte sequences of any length.
        /// Binary-first approach - no assumptions about text or specific patterns.
        /// </summary>
        private List<byte[]> DiscoverRepeatingPatterns(byte[] data, int minLength, int maxLength, int minOccurrences)
        {
            var patternDict = new Dictionary<string, PatternInfo>();

            // For small files (<100KB), scan every position for accuracy
            // For large files, use sampling to avoid performance issues
            int sampleInterval = data.Length < 100000 ? 1 : Math.Max(1, data.Length / 10000);

            for (int length = minLength; length <= maxLength; length++)
            {
                for (int i = 0; i < data.Length - length; i += sampleInterval)
                {
                    byte[] pattern = new byte[length];
                    Array.Copy(data, i, pattern, 0, length);

                    string key = BitConverter.ToString(pattern);

                    if (!patternDict.ContainsKey(key))
                    {
                        patternDict[key] = new PatternInfo { Bytes = pattern, Count = 0 };
                    }

                    patternDict[key].Count++;
                }
            }

            // For small files with no sampling, use minOccurrences directly
            // For sampled files, adjust the minimum
            int effectiveMin = (sampleInterval == 1)
                ? minOccurrences
                : Math.Max(1, minOccurrences / sampleInterval);

            return patternDict.Values
                .Where(p => p.Count >= effectiveMin)
                .OrderByDescending(p => p.Bytes.Length)  // Longer patterns first
                .ThenBy(p => p.Count)                     // Then fewer occurrences
                .Select(p => p.Bytes)
                .Take(50) // Top 50 patterns
                .ToList();
        }

        private class PatternInfo
        {
            public byte[] Bytes { get; set; }
            public int Count { get; set; }
        }

        /// <summary>
        /// Finds all positions where a pattern occurs in the data.
        /// Skips positions that have already been matched by longer patterns (to avoid double-counting).
        /// </summary>
        private List<int> FindAllOccurrences(byte[] data, byte[] pattern, HashSet<int> excludedPositions)
        {
            var positions = new List<int>();

            for (int i = 0; i <= data.Length - pattern.Length; i++)
            {
                // Skip if this position is already part of a longer pattern
                if (excludedPositions != null && excludedPositions.Contains(i))
                    continue;

                bool match = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (data[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    positions.Add(i);
                    i += pattern.Length - 1; // Skip past this occurrence
                }
            }

            return positions;
        }

        /// <summary>
        /// STEP 2: Analyzes each candidate to determine characteristics.
        /// </summary>
        private List<AnalyzedCandidate> AnalyzeCandidates(
            Dictionary<string, ByteSequenceCandidate> candidates,
            byte[] rawBytes)
        {
            var analyzed = new List<AnalyzedCandidate>();

            foreach (var kvp in candidates.Values)
            {
                var candidate = new AnalyzedCandidate
                {
                    Bytes = kvp.Bytes,
                    Positions = kvp.Positions,
                    Count = kvp.Count,
                    Frequency = (double)kvp.Count / rawBytes.Length,
                    AverageSpacing = CalculateAverageSpacing(kvp.Positions),
                    AppearsAtEnd = CheckIfAppearsAtEnd(kvp.Positions, rawBytes.Length)
                };

                analyzed.Add(candidate);
            }

            return analyzed;
        }

        /// <summary>
        /// Calculates average spacing between occurrences.
        /// </summary>
        private double CalculateAverageSpacing(List<int> positions)
        {
            if (positions.Count < 2)
                return 0;

            double totalSpacing = 0;
            for (int i = 1; i < positions.Count; i++)
            {
                totalSpacing += positions[i] - positions[i - 1];
            }

            return totalSpacing / (positions.Count - 1);
        }

        /// <summary>
        /// Checks if pattern appears near the end of data blocks.
        /// </summary>
        private bool CheckIfAppearsAtEnd(List<int> positions, int dataLength)
        {
            if (positions.Count == 0)
                return false;

            // Check if last position is near end of file
            int lastPosition = positions[positions.Count - 1];
            return (dataLength - lastPosition) < 10; // Within 10 bytes of end
        }

        /// <summary>
        /// STEP 3a: Classifies frame terminator (top level - separates complete messages).
        /// </summary>
        private TerminatorInfo ClassifyFrameTerminator(List<AnalyzedCandidate> candidates, byte[] rawBytes)
        {
            // Frame terminators characteristics:
            // - Low frequency (fewer occurrences)
            // - Longer sequences (2-4 bytes)
            // - Regular spacing (separates frames)

            var frameCandidate = candidates
                .Where(c => c.Bytes.Length >= 2)  // At least 2 bytes
                .Where(c => c.Count >= 1)         // At least 1 occurrence (supports single-frame logs!)
                .OrderByDescending(c => c.Bytes.Length) // FIRST: Prefer longer sequences (4-byte over 2-byte)
                .ThenBy(c => c.Count)            // THEN: Prefer fewer occurrences (frames)
                .FirstOrDefault();

            if (frameCandidate != null)
            {
                double confidence = Math.Min(0.95, 0.7 + (frameCandidate.Count > 0 ? 0.25 : 0));

                return new TerminatorInfo
                {
                    Bytes = frameCandidate.Bytes,
                    String = GetEscapedString(frameCandidate.Bytes),
                    DisplayName = GetDisplayName(frameCandidate.Bytes),
                    Frequency = frameCandidate.Frequency,
                    Confidence = confidence,
                    Type = TerminatorType.Frame,
                    Level = 1
                };
            }

            return null;
        }

        /// <summary>
        /// STEP 3b: Classifies segment terminator (middle level - separates segments within frame).
        /// </summary>
        private TerminatorInfo ClassifySegmentTerminator(
            List<AnalyzedCandidate> candidates,
            byte[] rawBytes,
            TerminatorInfo frameTerminator)
        {
            // Segment terminators characteristics:
            // - Medium frequency (more than frames, less than fields)
            // - Exactly 2 bytes (e.g., CRLF, not single bytes like Space)
            // - Often part of frame terminator (e.g., CRLF vs Double CRLF)
            // - Single-byte patterns are field delimiters, not segment terminators!

            var segmentCandidate = candidates
                .Where(c => c.Bytes.Length == 2)  // EXACTLY 2 bytes (not 1!) - single bytes are field delimiters
                .Where(c => c.Count >= 1)         // At least 1 occurrence (supports single-segment frames)
                .Where(c => frameTerminator == null || !ByteArrayEquals(c.Bytes, frameTerminator.Bytes)) // Not same as frame
                .OrderByDescending(c => c.Count)  // Prefer MORE occurrences (segments)
                .ThenBy(c => c.Bytes.Length)      // Prefer shorter
                .FirstOrDefault();

            if (segmentCandidate != null)
            {
                // Higher confidence if this is a subset of frame terminator
                double confidence = 0.8;
                if (frameTerminator != null && IsSubsequence(segmentCandidate.Bytes, frameTerminator.Bytes))
                {
                    confidence = 0.95; // Very confident - it's part of frame terminator
                }

                return new TerminatorInfo
                {
                    Bytes = segmentCandidate.Bytes,
                    String = GetEscapedString(segmentCandidate.Bytes),
                    DisplayName = GetDisplayName(segmentCandidate.Bytes),
                    Frequency = segmentCandidate.Frequency,
                    Confidence = confidence,
                    Type = TerminatorType.Segment,
                    Level = 2
                };
            }

            return null;
        }

        /// <summary>
        /// STEP 3c: Classifies field delimiter (bottom level - separates fields within segment).
        /// </summary>
        private TerminatorInfo ClassifyFieldDelimiter(List<AnalyzedCandidate> candidates, byte[] rawBytes)
        {
            // Field delimiter characteristics:
            // - High frequency (many occurrences)
            // - Single byte (usually)
            // - NOT line-ending characters

            var fieldCandidate = candidates
                .Where(c => c.Bytes.Length == 1)  // Single byte
                .Where(c => c.Bytes[0] != 0x0D && c.Bytes[0] != 0x0A) // NOT CR/LF
                .Where(c => c.Count >= 2)         // At least 2 occurrences (need at least 2 fields to detect)
                .OrderByDescending(c => c.Count)  // Prefer MOST occurrences (fields)
                .FirstOrDefault();

            if (fieldCandidate != null)
            {
                double confidence = Math.Min(0.95, 0.6 + (fieldCandidate.Count / (double)rawBytes.Length) * 100);

                return new TerminatorInfo
                {
                    Bytes = fieldCandidate.Bytes,
                    String = GetEscapedString(fieldCandidate.Bytes),
                    DisplayName = GetDisplayName(fieldCandidate.Bytes),
                    Frequency = fieldCandidate.Frequency,
                    Confidence = confidence,
                    Type = TerminatorType.Field,
                    Level = 3
                };
            }

            return null;
        }

        /// <summary>
        /// Checks if one byte array is a subsequence of another.
        /// </summary>
        private bool IsSubsequence(byte[] shorter, byte[] longer)
        {
            if (shorter.Length >= longer.Length)
                return false;

            for (int i = 0; i <= longer.Length - shorter.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < shorter.Length; j++)
                {
                    if (longer[i + j] != shorter[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if two byte arrays are equal.
        /// </summary>
        private bool ByteArrayEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null)
                return a == b;
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }

        #endregion

        #region Internal Classes

        private class TerminatorCandidate
        {
            public byte[] Bytes { get; set; }
            public int Count { get; set; }
        }

        private class ByteSequenceCandidate
        {
            public byte[] Bytes { get; set; }
            public List<int> Positions { get; set; }
            public int Count { get; set; }
        }

        private class AnalyzedCandidate
        {
            public byte[] Bytes { get; set; }
            public List<int> Positions { get; set; }
            public int Count { get; set; }
            public double Frequency { get; set; }
            public double AverageSpacing { get; set; }
            public bool AppearsAtEnd { get; set; }
        }

        #endregion
    }
}
