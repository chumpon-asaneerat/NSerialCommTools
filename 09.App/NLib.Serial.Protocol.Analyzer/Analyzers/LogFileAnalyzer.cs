using System;
using System.Collections.Generic;
using System.Linq;
using NLib.Serial.Protocol.Analyzer.Models;

namespace NLib.Serial.Protocol.Analyzer.Analyzers
{
    /// <summary>
    /// Analyzes log files to automatically detect protocol patterns using statistical analysis
    /// </summary>
    public class LogFileAnalyzer
    {
        /// <summary>
        /// Auto-detect package start marker using frequency analysis
        /// </summary>
        /// <param name="entries">List of log entries to analyze</param>
        /// <returns>Detected start marker bytes, or null if none found</returns>
        public byte[] DetectPackageStartMarker(List<LogEntry> entries)
        {
            if (entries == null || entries.Count < 5)
                return null; // Need minimum sample size

            // Dictionary to track frequency of byte sequences at the start
            var sequenceFrequency = new Dictionary<string, int>();

            // Analyze 1-4 byte sequences at the beginning of each entry
            foreach (var entry in entries)
            {
                if (entry.RawBytes == null || entry.RawBytes.Length == 0)
                    continue;

                // Try 1-byte, 2-byte, 3-byte, and 4-byte sequences
                for (int seqLength = 1; seqLength <= 4 && seqLength <= entry.RawBytes.Length; seqLength++)
                {
                    byte[] sequence = new byte[seqLength];
                    Array.Copy(entry.RawBytes, 0, sequence, 0, seqLength);

                    // Use hex string as dictionary key
                    string key = BitConverter.ToString(sequence);

                    if (!sequenceFrequency.ContainsKey(key))
                        sequenceFrequency[key] = 0;

                    sequenceFrequency[key]++;
                }
            }

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

            // Check if frequency meets threshold (30% of entries)
            double threshold = 0.30;
            double frequency = (double)maxCount / entries.Count;

            if (frequency >= threshold && mostCommonKey != null)
            {
                // Convert hex string back to byte array
                return ParseHexString(mostCommonKey);
            }

            return null; // No consistent start marker found
        }

        /// <summary>
        /// Auto-detect package end marker using frequency analysis
        /// </summary>
        /// <param name="entries">List of log entries to analyze</param>
        /// <returns>Detected end marker bytes, or null if none found</returns>
        public byte[] DetectPackageEndMarker(List<LogEntry> entries)
        {
            if (entries == null || entries.Count < 5)
                return null; // Need minimum sample size

            // Dictionary to track frequency of byte sequences at the end
            var sequenceFrequency = new Dictionary<string, int>();

            // Analyze 1-4 byte sequences at the end of each entry
            foreach (var entry in entries)
            {
                if (entry.RawBytes == null || entry.RawBytes.Length == 0)
                    continue;

                // Try 1-byte, 2-byte, 3-byte, and 4-byte sequences from the end
                for (int seqLength = 1; seqLength <= 4 && seqLength <= entry.RawBytes.Length; seqLength++)
                {
                    byte[] sequence = new byte[seqLength];
                    int startPos = entry.RawBytes.Length - seqLength;
                    Array.Copy(entry.RawBytes, startPos, sequence, 0, seqLength);

                    // Use hex string as dictionary key
                    string key = BitConverter.ToString(sequence);

                    if (!sequenceFrequency.ContainsKey(key))
                        sequenceFrequency[key] = 0;

                    sequenceFrequency[key]++;
                }
            }

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

            // Check if frequency meets threshold (30% of entries)
            double threshold = 0.30;
            double frequency = (double)maxCount / entries.Count;

            if (frequency >= threshold && mostCommonKey != null)
            {
                // Convert hex string back to byte array
                return ParseHexString(mostCommonKey);
            }

            return null; // No consistent end marker found
        }

        /// <summary>
        /// Auto-detect segment separator using frequency analysis
        /// </summary>
        /// <param name="entries">List of log entries to analyze</param>
        /// <returns>Detected separator bytes, or null if none found</returns>
        public byte[] DetectSegmentSeparator(List<LogEntry> entries)
        {
            if (entries == null || entries.Count < 5)
                return null; // Need minimum sample size

            // Dictionary to track frequency of byte sequences within entries
            var sequenceFrequency = new Dictionary<string, int>();
            int totalEntries = 0;

            // Analyze 1-2 byte sequences within each entry (excluding start/end)
            foreach (var entry in entries)
            {
                if (entry.RawBytes == null || entry.RawBytes.Length < 5)
                    continue; // Need sufficient length to have middle portion

                totalEntries++;

                // Skip first 2 and last 2 bytes to avoid start/end markers
                int startPos = Math.Min(2, entry.RawBytes.Length / 4);
                int endPos = entry.RawBytes.Length - Math.Min(2, entry.RawBytes.Length / 4);

                if (endPos <= startPos)
                    continue;

                // Track which sequences appear in this entry (for consistency)
                var seenInThisEntry = new HashSet<string>();

                // Try 1-byte and 2-byte sequences in the middle portion
                for (int i = startPos; i < endPos; i++)
                {
                    // 1-byte sequence
                    byte[] seq1 = new byte[] { entry.RawBytes[i] };
                    string key1 = BitConverter.ToString(seq1);

                    if (!seenInThisEntry.Contains(key1))
                    {
                        seenInThisEntry.Add(key1);
                        if (!sequenceFrequency.ContainsKey(key1))
                            sequenceFrequency[key1] = 0;
                        sequenceFrequency[key1]++;
                    }

                    // 2-byte sequence
                    if (i < endPos - 1)
                    {
                        byte[] seq2 = new byte[] { entry.RawBytes[i], entry.RawBytes[i + 1] };
                        string key2 = BitConverter.ToString(seq2);

                        if (!seenInThisEntry.Contains(key2))
                        {
                            seenInThisEntry.Add(key2);
                            if (!sequenceFrequency.ContainsKey(key2))
                                sequenceFrequency[key2] = 0;
                            sequenceFrequency[key2]++;
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

            // Check if frequency meets threshold (20% of entries must contain it)
            double threshold = 0.20;
            double frequency = (double)maxCount / totalEntries;

            if (frequency >= threshold && mostCommonKey != null)
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

            // Return encoding with highest ratio (must be > 95% to be confident)
            double threshold = 0.95;

            if (asciiRatio >= threshold && asciiRatio >= utf8Ratio && asciiRatio >= utf16Ratio && asciiRatio >= latin1Ratio)
                return EncodingType.ASCII;

            if (utf8Ratio >= threshold && utf8Ratio >= asciiRatio && utf8Ratio >= utf16Ratio && utf8Ratio >= latin1Ratio)
                return EncodingType.UTF8;

            if (utf16Ratio >= threshold && utf16Ratio >= asciiRatio && utf16Ratio >= utf8Ratio && utf16Ratio >= latin1Ratio)
                return EncodingType.UTF16;

            if (latin1Ratio >= threshold && latin1Ratio >= asciiRatio && latin1Ratio >= utf8Ratio && latin1Ratio >= utf16Ratio)
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
        /// </summary>
        private double TestASCII(byte[] data)
        {
            int validCount = 0;

            foreach (byte b in data)
            {
                // Valid ASCII: printable (0x20-0x7E) + CR/LF/TAB
                if ((b >= 0x20 && b <= 0x7E) || b == 0x09 || b == 0x0A || b == 0x0D)
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
