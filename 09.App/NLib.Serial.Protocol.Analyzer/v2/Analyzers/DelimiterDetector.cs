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
    /// Detects field delimiters dynamically by analyzing character frequency and patterns.
    /// </summary>
    public class DelimiterDetector
    {
        #region Public Methods

        /// <summary>
        /// Detects delimiters by analyzing all characters in messages.
        /// </summary>
        public List<DelimiterInfo> Detect(LogData logData)
        {
            if (logData == null || logData.Messages.Count == 0)
                return new List<DelimiterInfo>();

            // Count frequency of each byte value across all messages
            var byteCounts = new Dictionary<byte, int>();
            int totalBytes = 0;

            foreach (var message in logData.Messages)
            {
                foreach (byte b in message)
                {
                    // Skip control characters except common delimiters
                    if (b < 0x20 && b != 0x09 && b != 0x0D && b != 0x0A)
                        continue;
                    if (b > 0x7E) // Skip non-ASCII
                        continue;

                    if (!byteCounts.ContainsKey(b))
                        byteCounts[b] = 0;

                    byteCounts[b]++;
                    totalBytes++;
                }
            }

            // Analyze each byte to determine if it's likely a delimiter
            var results = new List<DelimiterInfo>();

            foreach (var kvp in byteCounts.OrderByDescending(x => x.Value))
            {
                byte b = kvp.Key;
                int count = kvp.Value;

                char c = (char)b;

                // Calculate frequency
                double frequency = (double)count / totalBytes;

                // Determine if this is likely a structural delimiter
                // Heuristics:
                // 1. Appears frequently (but not too frequently)
                // 2. Is a common delimiter character
                // 3. Appears multiple times per message on average

                double avgPerMessage = (double)count / logData.MessageCount;
                bool isCommonDelimiter = IsCommonDelimiterChar(c);
                bool goodFrequency = frequency > 0.01 && frequency < 0.5; // 1% to 50%
                bool multiplePerMessage = avgPerMessage >= 1.5;

                double confidence = 0.0;

                if (isCommonDelimiter)
                    confidence += 0.4;

                if (goodFrequency)
                    confidence += 0.3;

                if (multiplePerMessage)
                    confidence += 0.3;

                // Only include if confidence is reasonable
                if (confidence >= 0.3)
                {
                    bool isStructural = multiplePerMessage && confidence >= 0.6;

                    results.Add(new DelimiterInfo
                    {
                        Character = c,
                        DisplayName = GetDelimiterName(c),
                        Frequency = frequency,
                        Confidence = confidence,
                        IsStructural = isStructural,
                        OccurrenceCount = count
                    });
                }
            }

            return results.OrderByDescending(d => d.Confidence).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if a character is commonly used as a delimiter.
        /// </summary>
        private bool IsCommonDelimiterChar(char c)
        {
            // Common delimiters in protocols
            return c == ',' || c == ';' || c == '\t' || c == ' ' ||
                   c == '|' || c == ':' || c == '=' || c == '-' ||
                   c == '/' || c == '\\';
        }

        /// <summary>
        /// Gets a human-readable name for the delimiter character.
        /// </summary>
        private string GetDelimiterName(char delimiter)
        {
            switch (delimiter)
            {
                case ',': return "Comma";
                case ';': return "Semicolon";
                case '\t': return "Tab";
                case ' ': return "Space";
                case '|': return "Pipe";
                case ':': return "Colon";
                case '=': return "Equals";
                case '-': return "Dash";
                case '/': return "Slash";
                case '\\': return "Backslash";
                default:
                    if (char.IsLetterOrDigit(delimiter))
                        return $"'{delimiter}' (Alphanumeric)";
                    else
                        return $"'{delimiter}' (0x{((byte)delimiter):X2})";
            }
        }

        #endregion
    }
}
