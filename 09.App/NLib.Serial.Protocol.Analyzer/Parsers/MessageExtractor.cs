#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using NLib.Serial.ProtocolAnalyzer.Models;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Parsers
{
    /// <summary>
    /// Extracts individual messages from raw byte stream.
    /// </summary>
    public class MessageExtractor
    {
        #region Public Methods

        /// <summary>
        /// Extracts messages from raw bytes using common terminators.
        /// </summary>
        public LogData ExtractMessages(byte[] rawBytes)
        {
            if (rawBytes == null || rawBytes.Length == 0)
                return new LogData();

            // Try different terminators
            var terminators = new List<byte[]>
            {
                new byte[] { 0x0D, 0x0A }, // CRLF
                new byte[] { 0x0A },       // LF
                new byte[] { 0x0D }        // CR
            };

            LogData best = null;
            int maxMessages = 0;

            foreach (var terminator in terminators)
            {
                var logData = ExtractWithTerminator(rawBytes, terminator);
                if (logData.MessageCount > maxMessages)
                {
                    maxMessages = logData.MessageCount;
                    best = logData;
                }
            }

            return best ?? new LogData();
        }

        #endregion

        #region Private Methods

        private LogData ExtractWithTerminator(byte[] rawBytes, byte[] terminator)
        {
            var messages = new List<byte[]>();
            int start = 0;

            for (int i = 0; i <= rawBytes.Length - terminator.Length; i++)
            {
                bool matches = true;
                for (int j = 0; j < terminator.Length; j++)
                {
                    if (rawBytes[i + j] != terminator[j])
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches)
                {
                    // Found terminator
                    int length = i - start + terminator.Length;
                    byte[] message = new byte[length];
                    Array.Copy(rawBytes, start, message, 0, length);
                    messages.Add(message);

                    start = i + terminator.Length;
                    i += terminator.Length - 1;
                }
            }

            // Handle remaining bytes
            if (start < rawBytes.Length)
            {
                int length = rawBytes.Length - start;
                byte[] message = new byte[length];
                Array.Copy(rawBytes, start, message, 0, length);
                messages.Add(message);
            }

            return new LogData
            {
                Messages = messages,
                MessageCount = messages.Count,
                TotalBytes = rawBytes.Length,
                AverageMessageLength = messages.Count > 0 ? rawBytes.Length / messages.Count : 0
            };
        }

        #endregion
    }
}
