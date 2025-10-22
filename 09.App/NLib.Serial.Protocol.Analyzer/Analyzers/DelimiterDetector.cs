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
    /// Detects field delimiters in serial messages.
    /// </summary>
    public class DelimiterDetector
    {
        #region Public Methods

        /// <summary>
        /// Detects delimiters from the given log data.
        /// </summary>
        public List<DelimiterInfo> Detect(LogData logData)
        {
            if (logData == null || logData.Messages.Count == 0)
                return new List<DelimiterInfo>();

            // Common delimiter characters
            char[] candidateDelimiters = { ',', ';', '\t', ' ', '|', ':', '=' };

            var delimiterCounts = new Dictionary<char, int>();
            foreach (char delim in candidateDelimiters)
            {
                delimiterCounts[delim] = 0;
            }

            int totalMessages = logData.Messages.Count;

            // Count occurrences of each delimiter
            foreach (var message in logData.Messages)
            {
                string text = Encoding.ASCII.GetString(message);
                foreach (char delim in candidateDelimiters)
                {
                    delimiterCounts[delim] += text.Count(c => c == delim);
                }
            }

            // Convert to DelimiterInfo list
            var results = new List<DelimiterInfo>();
            foreach (var kvp in delimiterCounts)
            {
                if (kvp.Value > 0)
                {
                    double avgPerMessage = (double)kvp.Value / totalMessages;
                    bool isStructural = avgPerMessage > 1.0; // Appears multiple times per message

                    results.Add(new DelimiterInfo
                    {
                        Character = kvp.Key,
                        DisplayName = GetDelimiterName(kvp.Key),
                        Frequency = avgPerMessage / 10.0, // Normalize
                        Confidence = isStructural ? 0.9 : 0.5,
                        IsStructural = isStructural,
                        OccurrenceCount = kvp.Value
                    });
                }
            }

            return results.OrderByDescending(d => d.Confidence).ToList();
        }

        #endregion

        #region Private Methods

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
                default: return delimiter.ToString();
            }
        }

        #endregion
    }
}
