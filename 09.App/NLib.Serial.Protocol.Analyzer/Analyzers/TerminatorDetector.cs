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
    /// </summary>
    public class TerminatorDetector
    {
        #region Public Methods

        /// <summary>
        /// Detects the terminator by analyzing the endings of all messages.
        /// </summary>
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

        #endregion

        #region Internal Class

        private class TerminatorCandidate
        {
            public byte[] Bytes { get; set; }
            public int Count { get; set; }
        }

        #endregion
    }
}
