using System;
using System.Collections.Generic;
using System.Linq;
using NLib.Serial.Protocol.Analyzer.Models;

namespace NLib.Serial.Protocol.Analyzer.Analyzers
{
    /// <summary>
    /// Analyzes log files to automatically detect protocol patterns using statistical analysis
    /// Uses configurable thresholds and parameters for flexible detection
    /// </summary>
    public class LogFileAnalyzer
    {
        private readonly LogFileAnalyzerConfig _config;

        /// <summary>
        /// Creates analyzer with default configuration
        /// </summary>
        public LogFileAnalyzer() : this(new LogFileAnalyzerConfig())
        {
        }

        /// <summary>
        /// Creates analyzer with custom configuration
        /// </summary>
        /// <param name="config">Configuration parameters for detection algorithms</param>
        public LogFileAnalyzer(LogFileAnalyzerConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        /// <summary>
        /// Auto-detect package start marker using DISTANCE-BASED analysis
        /// True markers appear at REGULAR INTERVALS (package boundaries), not just frequently
        /// Scans continuous byte stream for repeating patterns with consistent spacing
        /// </summary>
        /// <param name="entries">List of log entries to analyze</param>
        /// <returns>Detected start marker bytes, or null if none found</returns>
        public byte[] DetectPackageStartMarker(List<LogEntry> entries)
        {
            if (entries == null || entries.Count == 0)
                return null;

            // Get all bytes from all entries (may be one big entry or multiple)
            var allBytes = new List<byte>();
            foreach (var entry in entries)
            {
                if (entry.RawBytes != null && entry.RawBytes.Length > 0)
                {
                    allBytes.AddRange(entry.RawBytes);
                }
            }

            byte[] data = allBytes.ToArray();
            if (data.Length < 100) // Need sufficient data
                return null;

            // NEW ALGORITHM: Find sequences that appear at REGULAR INTERVALS
            // (Not just "most common" - that finds common data patterns!)

            string bestMarker = null;
            double bestScore = 0;
            int bestLength = 0;

            // Start from longest sequences and work down
            for (int seqLength = _config.MaxSequenceLength; seqLength >= _config.MinSequenceLength; seqLength--)
            {
                // Find all occurrences of each sequence of this length
                var sequencePositions = new Dictionary<string, List<int>>();

                // Scan through data
                for (int i = 0; i <= data.Length - seqLength; i++)
                {
                    byte[] sequence = new byte[seqLength];
                    Array.Copy(data, i, sequence, 0, seqLength);

                    string key = BitConverter.ToString(sequence);

                    if (!sequencePositions.ContainsKey(key))
                        sequencePositions[key] = new List<int>();

                    sequencePositions[key].Add(i);
                }

                // Analyze each sequence: Calculate spacing consistency
                foreach (var kvp in sequencePositions)
                {
                    List<int> positions = kvp.Value;

                    // Need at least 5 occurrences
                    if (positions.Count < 5)
                        continue;

                    // Calculate distances between consecutive occurrences
                    List<int> distances = new List<int>();
                    for (int i = 1; i < positions.Count; i++)
                    {
                        distances.Add(positions[i] - positions[i - 1]);
                    }

                    // Calculate average distance and standard deviation
                    double avgDistance = distances.Average();
                    double variance = distances.Select(d => Math.Pow(d - avgDistance, 2)).Average();
                    double stdDev = Math.Sqrt(variance);

                    // Calculate coefficient of variation (CV = stdDev / mean)
                    // Lower CV = more consistent spacing = more likely a boundary marker
                    double cv = stdDev / avgDistance;

                    // Score = Frequency × Length × (1 / CV)
                    // Prioritize: many occurrences, longer sequences, consistent spacing
                    double score = positions.Count * seqLength * (1.0 / (cv + 0.1)); // +0.1 to avoid division by zero

                    // Update best if this is better
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMarker = kvp.Key;
                        bestLength = seqLength;
                    }
                }

                // If we found a good marker at this length, stop searching shorter lengths
                if (bestMarker != null && bestLength == seqLength && bestScore > 100)
                {
                    return ParseHexString(bestMarker);
                }
            }

            // Return best marker found (if any)
            return bestMarker != null ? ParseHexString(bestMarker) : null;
        }

        /// <summary>
        /// Helper: Compare two byte arrays for equality
        /// </summary>
        private bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Auto-detect package end marker using DISTANCE-BASED analysis
        /// True markers appear at REGULAR INTERVALS (package boundaries), not just frequently
        /// Scans continuous byte stream for repeating patterns with consistent spacing
        /// </summary>
        /// <param name="entries">List of log entries to analyze</param>
        /// <returns>Detected end marker bytes, or null if none found</returns>
        public byte[] DetectPackageEndMarker(List<LogEntry> entries)
        {
            if (entries == null || entries.Count == 0)
                return null;

            // Get all bytes from all entries (may be one big entry or multiple)
            var allBytes = new List<byte>();
            foreach (var entry in entries)
            {
                if (entry.RawBytes != null && entry.RawBytes.Length > 0)
                {
                    allBytes.AddRange(entry.RawBytes);
                }
            }

            byte[] data = allBytes.ToArray();
            if (data.Length < 100) // Need sufficient data
                return null;

            // NEW ALGORITHM: Find sequences that appear at REGULAR INTERVALS
            // (Not just "most common" - that finds common data patterns!)

            string bestMarker = null;
            double bestScore = 0;
            int bestLength = 0;

            // Start from longest sequences and work down
            for (int seqLength = _config.MaxSequenceLength; seqLength >= _config.MinSequenceLength; seqLength--)
            {
                // Find all occurrences of each sequence of this length
                var sequencePositions = new Dictionary<string, List<int>>();

                // Scan through data
                for (int i = 0; i <= data.Length - seqLength; i++)
                {
                    byte[] sequence = new byte[seqLength];
                    Array.Copy(data, i, sequence, 0, seqLength);

                    string key = BitConverter.ToString(sequence);

                    if (!sequencePositions.ContainsKey(key))
                        sequencePositions[key] = new List<int>();

                    sequencePositions[key].Add(i);
                }

                // Analyze each sequence: Calculate spacing consistency
                foreach (var kvp in sequencePositions)
                {
                    List<int> positions = kvp.Value;

                    // Need at least 5 occurrences
                    if (positions.Count < 5)
                        continue;

                    // Calculate distances between consecutive occurrences
                    List<int> distances = new List<int>();
                    for (int i = 1; i < positions.Count; i++)
                    {
                        distances.Add(positions[i] - positions[i - 1]);
                    }

                    // Calculate average distance and standard deviation
                    double avgDistance = distances.Average();
                    double variance = distances.Select(d => Math.Pow(d - avgDistance, 2)).Average();
                    double stdDev = Math.Sqrt(variance);

                    // Calculate coefficient of variation (CV = stdDev / mean)
                    // Lower CV = more consistent spacing = more likely a boundary marker
                    double cv = stdDev / avgDistance;

                    // Score = Frequency × Length × (1 / CV)
                    // Prioritize: many occurrences, longer sequences, consistent spacing
                    double score = positions.Count * seqLength * (1.0 / (cv + 0.1)); // +0.1 to avoid division by zero

                    // Update best if this is better
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMarker = kvp.Key;
                        bestLength = seqLength;
                    }
                }

                // If we found a good marker at this length, stop searching shorter lengths
                if (bestMarker != null && bestLength == seqLength && bestScore > 100)
                {
                    return ParseHexString(bestMarker);
                }
            }

            // Return best marker found (if any)
            return bestMarker != null ? ParseHexString(bestMarker) : null;
        }

        /// <summary>
        /// Auto-detect segment separator using frequency analysis
        /// </summary>
        /// <param name="entries">List of log entries to analyze</param>
        /// <returns>Detected separator bytes, or null if none found</returns>
        public byte[] DetectSegmentSeparator(List<LogEntry> entries)
        {
            if (entries == null || entries.Count < _config.MinimumSampleSize)
                return null; // Need minimum sample size

            // Dictionary to track frequency of byte sequences within entries
            var sequenceFrequency = new Dictionary<string, int>();
            int totalEntries = 0;

            // Analyze sequences within each entry (excluding start/end)
            foreach (var entry in entries)
            {
                int minLength = _config.SeparatorSkipBytesFromStart + _config.SeparatorSkipBytesFromEnd + 1;
                if (entry.RawBytes == null || entry.RawBytes.Length < minLength)
                    continue; // Need sufficient length to have middle portion

                totalEntries++;

                // Skip configured bytes from start/end to avoid detecting markers as separators
                int startPos = Math.Min(_config.SeparatorSkipBytesFromStart, entry.RawBytes.Length / 4);
                int endPos = entry.RawBytes.Length - Math.Min(_config.SeparatorSkipBytesFromEnd, entry.RawBytes.Length / 4);

                if (endPos <= startPos)
                    continue;

                // Track which sequences appear in this entry (for consistency)
                var seenInThisEntry = new HashSet<string>();

                // Try sequences of varying lengths in the middle portion
                for (int i = startPos; i < endPos; i++)
                {
                    for (int seqLength = _config.MinSequenceLength;
                         seqLength <= _config.MaxSequenceLength && i + seqLength <= endPos;
                         seqLength++)
                    {
                        byte[] sequence = new byte[seqLength];
                        Array.Copy(entry.RawBytes, i, sequence, 0, seqLength);

                        string key = BitConverter.ToString(sequence);

                        if (!seenInThisEntry.Contains(key))
                        {
                            seenInThisEntry.Add(key);
                            if (!sequenceFrequency.ContainsKey(key))
                                sequenceFrequency[key] = 0;
                            sequenceFrequency[key]++;
                        }
                    }
                }
            }

            if (totalEntries == 0)
                return null;

            // Find sequence with highest frequency
            string mostCommonKey = null;
            int maxCount = 0;

            foreach (var kvp in sequenceFrequency)
            {
                if (kvp.Value > maxCount)
                {
                    maxCount = kvp.Value;
                    mostCommonKey = kvp.Key;
                }
            }

            // Check if frequency meets configured threshold
            double frequency = (double)maxCount / totalEntries;

            if (frequency >= _config.SeparatorFrequencyThreshold && mostCommonKey != null)
            {
                // Convert hex string back to byte array
                return ParseHexString(mostCommonKey);
            }

            return null; // No consistent separator found
        }

        /// <summary>
        /// Auto-detect encoding using valid character ratio analysis
        /// </summary>
        /// <param name="entries">List of log entries to analyze</param>
        /// <returns>Detected encoding type</returns>
        public EncodingType DetectEncoding(List<LogEntry> entries)
        {
            if (entries == null || entries.Count == 0)
                return EncodingType.ASCII; // Default

            // Collect all bytes from all entries
            var allBytes = new List<byte>();
            foreach (var entry in entries)
            {
                if (entry.RawBytes != null && entry.RawBytes.Length > 0)
                {
                    allBytes.AddRange(entry.RawBytes);
                }
            }

            if (allBytes.Count == 0)
                return EncodingType.ASCII;

            byte[] data = allBytes.ToArray();

            // Test each encoding and calculate valid character ratio
            double asciiRatio = TestASCII(data);
            double utf8Ratio = TestUTF8(data);
            double utf16Ratio = TestUTF16(data);
            double latin1Ratio = TestLatin1(data);

            // Return encoding with highest ratio (must meet configured confidence threshold)
            if (asciiRatio >= _config.EncodingConfidenceThreshold && asciiRatio >= utf8Ratio && asciiRatio >= utf16Ratio && asciiRatio >= latin1Ratio)
                return EncodingType.ASCII;

            if (utf8Ratio >= _config.EncodingConfidenceThreshold && utf8Ratio >= asciiRatio && utf8Ratio >= utf16Ratio && utf8Ratio >= latin1Ratio)
                return EncodingType.UTF8;

            if (utf16Ratio >= _config.EncodingConfidenceThreshold && utf16Ratio >= asciiRatio && utf16Ratio >= utf8Ratio && utf16Ratio >= latin1Ratio)
                return EncodingType.UTF16;

            if (latin1Ratio >= _config.EncodingConfidenceThreshold && latin1Ratio >= asciiRatio && latin1Ratio >= utf8Ratio && latin1Ratio >= utf16Ratio)
                return EncodingType.Latin1;

            // Default to ASCII if uncertain
            return EncodingType.ASCII;
        }

        #region Helper Methods

        /// <summary>
        /// Parse hex string (e.g., "0D-0A") to byte array
        /// </summary>
        private byte[] ParseHexString(string hexString)
        {
            string[] hexBytes = hexString.Split('-');
            byte[] result = new byte[hexBytes.Length];

            for (int i = 0; i < hexBytes.Length; i++)
            {
                result[i] = Convert.ToByte(hexBytes[i], 16);
            }

            return result;
        }

        /// <summary>
        /// Test if data is valid ASCII (printable + whitespace)
        /// Uses configured ASCII ranges and whitespace characters
        /// </summary>
        private double TestASCII(byte[] data)
        {
            int validCount = 0;

            foreach (byte b in data)
            {
                // Check if byte is in printable range or is a valid whitespace character
                bool isInPrintableRange = (b >= _config.AsciiPrintableMin && b <= _config.AsciiPrintableMax);
                bool isWhitespace = Array.IndexOf(_config.AsciiWhitespaceChars, b) >= 0;

                if (isInPrintableRange || isWhitespace)
                {
                    validCount++;
                }
            }

            return (double)validCount / data.Length;
        }

        /// <summary>
        /// Test if data is valid UTF-8
        /// </summary>
        private double TestUTF8(byte[] data)
        {
            try
            {
                // Try to decode as UTF-8
                var encoding = System.Text.Encoding.UTF8;
                string decoded = encoding.GetString(data);

                // Re-encode and compare
                byte[] reencoded = encoding.GetBytes(decoded);

                // Calculate how many bytes match
                int matchCount = 0;
                int minLength = Math.Min(data.Length, reencoded.Length);

                for (int i = 0; i < minLength; i++)
                {
                    if (data[i] == reencoded[i])
                        matchCount++;
                }

                return (double)matchCount / data.Length;
            }
            catch
            {
                return 0.0; // Invalid UTF-8
            }
        }

        /// <summary>
        /// Test if data is valid UTF-16
        /// </summary>
        private double TestUTF16(byte[] data)
        {
            try
            {
                // UTF-16 requires even number of bytes
                if (data.Length % 2 != 0)
                    return 0.0;

                // Try to decode as UTF-16
                var encoding = System.Text.Encoding.Unicode;
                string decoded = encoding.GetString(data);

                // Re-encode and compare
                byte[] reencoded = encoding.GetBytes(decoded);

                // Calculate how many bytes match
                int matchCount = 0;
                int minLength = Math.Min(data.Length, reencoded.Length);

                for (int i = 0; i < minLength; i++)
                {
                    if (data[i] == reencoded[i])
                        matchCount++;
                }

                return (double)matchCount / data.Length;
            }
            catch
            {
                return 0.0; // Invalid UTF-16
            }
        }

        /// <summary>
        /// Test if data is valid Latin-1 (ISO-8859-1)
        /// </summary>
        private double TestLatin1(byte[] data)
        {
            try
            {
                // Latin-1 can encode all byte values 0x00-0xFF
                // Test for common Latin-1 patterns
                var encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                string decoded = encoding.GetString(data);

                // Re-encode and compare
                byte[] reencoded = encoding.GetBytes(decoded);

                // Calculate how many bytes match
                int matchCount = 0;
                int minLength = Math.Min(data.Length, reencoded.Length);

                for (int i = 0; i < minLength; i++)
                {
                    if (data[i] == reencoded[i])
                        matchCount++;
                }

                return (double)matchCount / data.Length;
            }
            catch
            {
                return 0.0; // Invalid Latin-1
            }
        }

        #endregion
    }
}
