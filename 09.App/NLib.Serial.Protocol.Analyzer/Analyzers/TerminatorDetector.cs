#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using NLib.Serial.ProtocolAnalyzer.Models;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Analyzers
{
    /// <summary>
    /// Detects message terminators in serial data.
    /// </summary>
    public class TerminatorDetector
    {
        #region Public Methods

        /// <summary>
        /// Detects the terminator from the given log data.
        /// </summary>
        public TerminatorInfo Detect(LogData logData)
        {
            if (logData == null || logData.Messages.Count == 0)
                return null;

            // Common terminators to check
            var candidates = new List<TerminatorCandidate>
            {
                new TerminatorCandidate { Bytes = new byte[] { 0x0D, 0x0A }, Name = "CRLF" },
                new TerminatorCandidate { Bytes = new byte[] { 0x0A }, Name = "LF" },
                new TerminatorCandidate { Bytes = new byte[] { 0x0D }, Name = "CR" },
                new TerminatorCandidate { Bytes = new byte[] { 0x00 }, Name = "NULL" }
            };

            int totalMessages = logData.Messages.Count;

            foreach (var candidate in candidates)
            {
                int count = 0;
                foreach (var message in logData.Messages)
                {
                    if (message.Length >= candidate.Bytes.Length)
                    {
                        bool matches = true;
                        for (int i = 0; i < candidate.Bytes.Length; i++)
                        {
                            if (message[message.Length - candidate.Bytes.Length + i] != candidate.Bytes[i])
                            {
                                matches = false;
                                break;
                            }
                        }
                        if (matches)
                            count++;
                    }
                }

                candidate.Frequency = (double)count / totalMessages;
            }

            // Select the terminator with the highest frequency
            var best = candidates.OrderByDescending(c => c.Frequency).First();

            return new TerminatorInfo
            {
                String = GetEscapedString(best.Bytes),
                Bytes = best.Bytes,
                DisplayName = best.Name,
                Frequency = best.Frequency,
                Confidence = best.Frequency
            };
        }

        #endregion

        #region Private Methods

        private string GetEscapedString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes)
            {
                if (b == 0x0D) result += "\\r";
                else if (b == 0x0A) result += "\\n";
                else if (b == 0x00) result += "\\0";
                else result += (char)b;
            }
            return result;
        }

        #endregion

        #region Internal Class

        private class TerminatorCandidate
        {
            public byte[] Bytes { get; set; }
            public string Name { get; set; }
            public double Frequency { get; set; }
        }

        #endregion
    }
}
