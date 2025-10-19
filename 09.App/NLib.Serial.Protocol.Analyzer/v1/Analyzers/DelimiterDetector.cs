#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLib.Serial.Protocol.Analyzer.Models;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Analyzers
{
    /// <summary>
    /// Detects delimiters in messages
    /// </summary>
    public class DelimiterDetector
    {
        #region Public Methods

        /// <summary>
        /// Detects delimiters from log data
        /// </summary>
        public List<DelimiterInfo> DetectDelimiters(LogData data)
        {
            if (data == null || data.Messages == null || data.Messages.Count == 0)
                return new List<DelimiterInfo>();

            // Common delimiters to check
            var candidates = new char[] { ',', ';', '\t', ' ', '|', ':' };
            var results = new List<DelimiterInfo>();

            foreach (char delimiter in candidates)
            {
                int messagesWithDelimiter = 0;
                int totalCount = 0;

                foreach (byte[] message in data.Messages)
                {
                    string text = Encoding.ASCII.GetString(message);
                    int count = text.Count(c => c == delimiter);

                    if (count > 0)
                    {
                        messagesWithDelimiter++;
                        totalCount += count;
                    }
                }

                if (messagesWithDelimiter > 0)
                {
                    double frequency = (double)messagesWithDelimiter / data.Messages.Count * 100.0;
                    double avgCount = (double)totalCount / messagesWithDelimiter;

                    // Determine if structural (consistent count)
                    bool isStructural = IsStructuralDelimiter(data.Messages, delimiter);

                    // Calculate confidence
                    double confidence = frequency;
                    if (isStructural) confidence = Math.Min(100, confidence * 1.2);

                    results.Add(new DelimiterInfo
                    {
                        Delimiter = delimiter,
                        CharacterName = GetCharacterName(delimiter),
                        Frequency = frequency,
                        AverageCount = avgCount,
                        IsStructural = isStructural,
                        Confidence = confidence
                    });
                }
            }

            return results.OrderByDescending(d => d.Confidence).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if delimiter appears consistently across messages
        /// </summary>
        private bool IsStructuralDelimiter(List<byte[]> messages, char delimiter)
        {
            if (messages.Count < 2)
                return false;

            var counts = messages
                .Select(m => Encoding.ASCII.GetString(m).Count(c => c == delimiter))
                .Where(c => c > 0)
                .ToList();

            if (counts.Count == 0)
                return false;

            // Check if counts are consistent (within 20% variance)
            double avg = counts.Average();
            double variance = counts.Average(c => Math.Abs(c - avg));

            return variance / avg < 0.2; // Less than 20% variance
        }

        /// <summary>
        /// Gets display name for character
        /// </summary>
        private string GetCharacterName(char c)
        {
            switch (c)
            {
                case ',': return "Comma";
                case ';': return "Semicolon";
                case '\t': return "Tab";
                case ' ': return "Space";
                case '|': return "Pipe";
                case ':': return "Colon";
                default: return c.ToString();
            }
        }

        #endregion
    }
}
