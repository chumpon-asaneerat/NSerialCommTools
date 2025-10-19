#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using NLib.Serial.Protocol.Analyzer.Models;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Analyzers
{
    /// <summary>
    /// Detects message terminators in log data
    /// </summary>
    public class TerminatorDetector
    {
        #region Public Methods

        /// <summary>
        /// Detects terminator from log data
        /// </summary>
        public TerminatorInfo DetectTerminator(LogData data)
        {
            if (data == null || data.Messages == null || data.Messages.Count == 0)
                return new TerminatorInfo { Detected = false };

            // Try common terminators
            var terminators = new[]
            {
                new { Bytes = new byte[] { 0x0D, 0x0A }, Type = "CRLF" },
                new { Bytes = new byte[] { 0x0A }, Type = "LF" },
                new { Bytes = new byte[] { 0x0D }, Type = "CR" }
            };

            TerminatorInfo bestMatch = null;
            int bestCount = 0;

            foreach (var term in terminators)
            {
                int count = CountMessagesEndingWith(data.Messages, term.Bytes);

                if (count > bestCount)
                {
                    bestCount = count;
                    bestMatch = new TerminatorInfo
                    {
                        Detected = true,
                        Type = term.Type,
                        Bytes = term.Bytes,
                        Frequency = count,
                        TotalMessages = data.Messages.Count,
                        Confidence = (double)count / data.Messages.Count * 100.0
                    };
                }
            }

            if (bestMatch == null)
            {
                return new TerminatorInfo { Detected = false };
            }

            return bestMatch;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Counts how many messages end with specific terminator
        /// </summary>
        private int CountMessagesEndingWith(List<byte[]> messages, byte[] terminator)
        {
            int count = 0;

            foreach (byte[] message in messages)
            {
                if (message.Length < terminator.Length)
                    continue;

                bool matches = true;
                for (int i = 0; i < terminator.Length; i++)
                {
                    if (message[message.Length - terminator.Length + i] != terminator[i])
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches)
                    count++;
            }

            return count;
        }

        #endregion
    }
}
