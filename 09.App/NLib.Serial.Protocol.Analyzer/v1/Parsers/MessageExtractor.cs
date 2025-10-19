#region Using

using System;
using System.Collections.Generic;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Parsers
{
    /// <summary>
    /// Extracts individual messages from raw byte stream
    /// </summary>
    public class MessageExtractor
    {
        #region Public Methods

        /// <summary>
        /// Extracts messages from raw bytes
        /// </summary>
        /// <param name="rawBytes">Raw byte array</param>
        /// <param name="terminator">Message terminator (null for auto-detect)</param>
        /// <returns>List of individual messages</returns>
        public List<byte[]> ExtractMessages(byte[] rawBytes,
            byte[] terminator = null)
        {
            // If terminator not specified, try to detect
            if (terminator == null)
                terminator = DetectTerminator(rawBytes);

            List<byte[]> messages = new List<byte[]>();
            List<byte> currentMessage = new List<byte>();

            for (int i = 0; i < rawBytes.Length; i++)
            {
                currentMessage.Add(rawBytes[i]);

                // Check if we have a terminator match
                if (EndsWithTerminator(currentMessage, terminator))
                {
                    messages.Add(currentMessage.ToArray());
                    currentMessage.Clear();
                }
            }

            // Add remaining bytes if any
            if (currentMessage.Count > 0)
                messages.Add(currentMessage.ToArray());

            return messages;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Detects the most common terminator
        /// </summary>
        private byte[] DetectTerminator(byte[] bytes)
        {
            // Try common terminators
            byte[] crLf = new byte[] { 0x0D, 0x0A };
            byte[] lf = new byte[] { 0x0A };
            byte[] cr = new byte[] { 0x0D };

            int crLfCount = CountOccurrences(bytes, crLf);
            int lfCount = CountOccurrences(bytes, lf);
            int crCount = CountOccurrences(bytes, cr);

            if (crLfCount > 0 && crLfCount >= lfCount / 2)
                return crLf;
            else if (lfCount > crCount)
                return lf;
            else if (crCount > 0)
                return cr;

            return crLf; // Default
        }

        /// <summary>
        /// Counts occurrences of a pattern in data
        /// </summary>
        private int CountOccurrences(byte[] data, byte[] pattern)
        {
            int count = 0;
            for (int i = 0; i <= data.Length - pattern.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (data[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    count++;
                    i += pattern.Length - 1;
                }
            }
            return count;
        }

        /// <summary>
        /// Checks if message ends with terminator
        /// </summary>
        private bool EndsWithTerminator(List<byte> message, byte[] terminator)
        {
            if (message.Count < terminator.Length)
                return false;

            for (int i = 0; i < terminator.Length; i++)
            {
                if (message[message.Count - terminator.Length + i] != terminator[i])
                    return false;
            }

            return true;
        }

        #endregion
    }
}
