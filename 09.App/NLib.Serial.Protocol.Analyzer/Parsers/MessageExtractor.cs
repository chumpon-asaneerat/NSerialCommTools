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

            // First, try to detect if this is a multi-line package format
            var packageData = TryExtractPackages(rawBytes);
            if (packageData != null && packageData.MessageCount > 0)
                return packageData;

            // Otherwise, try different terminators for single-line format
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

        private LogData TryExtractPackages(byte[] rawBytes)
        {
            // Look for common package boundary markers
            // Check if data contains start markers like ^, STX (0x02), etc.
            // and corresponding end markers like ~, ETX (0x03), etc.

            // Pattern 1: ^ (0x5E) ... ~ (0x7E) - common in many devices
            var pattern1 = ExtractByMarkers(rawBytes, 0x5E, 0x7E);
            if (pattern1.MessageCount > 0)
                return pattern1;

            // Pattern 2: STX (0x02) ... ETX (0x03) - standard control characters
            var pattern2 = ExtractByMarkers(rawBytes, 0x02, 0x03);
            if (pattern2.MessageCount > 0)
                return pattern2;

            return null;
        }

        private LogData ExtractByMarkers(byte[] rawBytes, byte startMarker, byte endMarker)
        {
            var messages = new List<byte[]>();
            int i = 0;

            while (i < rawBytes.Length)
            {
                // Find start marker
                int startPos = -1;
                for (int j = i; j < rawBytes.Length; j++)
                {
                    if (rawBytes[j] == startMarker)
                    {
                        startPos = j;
                        break;
                    }
                }

                if (startPos == -1)
                    break;

                // Find end marker after start
                int endPos = -1;
                for (int j = startPos + 1; j < rawBytes.Length; j++)
                {
                    if (rawBytes[j] == endMarker)
                    {
                        endPos = j;
                        break;
                    }
                }

                if (endPos == -1)
                    break;

                // Skip trailing line terminators after end marker
                int actualEnd = endPos + 1;
                while (actualEnd < rawBytes.Length &&
                       (rawBytes[actualEnd] == 0x0D || rawBytes[actualEnd] == 0x0A ||
                        rawBytes[actualEnd] == 0x20))
                {
                    actualEnd++;
                }

                // Extract package
                int length = actualEnd - startPos;
                byte[] message = new byte[length];
                Array.Copy(rawBytes, startPos, message, 0, length);
                messages.Add(message);

                i = actualEnd;
            }

            if (messages.Count == 0)
                return new LogData();

            int totalBytes = messages.Sum(m => m.Length);
            return new LogData
            {
                Messages = messages,
                MessageCount = messages.Count,
                TotalBytes = totalBytes,
                AverageMessageLength = totalBytes / messages.Count
            };
        }

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
