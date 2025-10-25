#region Using

using System;
using System.Collections.Generic;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Utilities
{
    /// <summary>
    /// Binary-safe byte array splitting utilities.
    /// Replaces string-based Split() operations that lose binary data.
    /// Based on Document 02: System Architecture - TODO-001/002.
    /// </summary>
    public static class ByteArraySplitter
    {
        #region Split By Terminator

        /// <summary>
        /// Splits a byte array by a terminator sequence (binary-safe).
        /// Equivalent to string.Split() but works with byte arrays.
        /// </summary>
        /// <param name="data">Source byte array to split.</param>
        /// <param name="terminator">Terminator byte sequence to split by.</param>
        /// <param name="options">Split options (None or RemoveEmptyEntries).</param>
        /// <returns>List of byte arrays split by the terminator.</returns>
        public static List<byte[]> Split(byte[] data, byte[] terminator, SplitOptions options = SplitOptions.None)
        {
            if (data == null || data.Length == 0)
            {
                return new List<byte[]>();
            }

            if (terminator == null || terminator.Length == 0)
            {
                // No terminator - return entire array as single element
                return new List<byte[]> { data };
            }

            var result = new List<byte[]>();
            int startIndex = 0;

            for (int i = 0; i <= data.Length - terminator.Length; i++)
            {
                // Check if terminator matches at current position
                if (MatchesAt(data, terminator, i))
                {
                    // Extract segment from startIndex to i (excluding terminator)
                    int segmentLength = i - startIndex;

                    // Create segment (handle empty segments based on options)
                    if (segmentLength > 0 || options == SplitOptions.None)
                    {
                        byte[] segment = new byte[segmentLength];
                        if (segmentLength > 0)
                        {
                            Array.Copy(data, startIndex, segment, 0, segmentLength);
                        }
                        result.Add(segment);
                    }

                    // Move past the terminator
                    startIndex = i + terminator.Length;
                    i += terminator.Length - 1; // -1 because loop will increment
                }
            }

            // Add remaining data after last terminator
            if (startIndex < data.Length)
            {
                int remainingLength = data.Length - startIndex;
                byte[] remaining = new byte[remainingLength];
                Array.Copy(data, startIndex, remaining, 0, remainingLength);

                if (remainingLength > 0 || options == SplitOptions.None)
                {
                    result.Add(remaining);
                }
            }
            else if (startIndex == data.Length && options == SplitOptions.None)
            {
                // Data ends exactly with terminator - add empty array
                result.Add(new byte[0]);
            }

            return result;
        }

        /// <summary>
        /// Splits by multiple possible terminators (tries each in order).
        /// Used when protocol may have variant terminators (e.g., "\r\n" or "\n" or "\r").
        /// </summary>
        /// <param name="data">Source byte array to split.</param>
        /// <param name="terminators">Array of possible terminator sequences (tried in order).</param>
        /// <param name="options">Split options.</param>
        /// <returns>List of byte arrays split by first matching terminator.</returns>
        public static List<byte[]> SplitByAny(byte[] data, byte[][] terminators, SplitOptions options = SplitOptions.None)
        {
            if (data == null || data.Length == 0)
            {
                return new List<byte[]>();
            }

            if (terminators == null || terminators.Length == 0)
            {
                return new List<byte[]> { data };
            }

            // Try each terminator and use the one that produces most splits
            List<byte[]> bestResult = null;
            int maxSplits = 0;

            foreach (var terminator in terminators)
            {
                var result = Split(data, terminator, options);
                if (result.Count > maxSplits)
                {
                    maxSplits = result.Count;
                    bestResult = result;
                }
            }

            return bestResult ?? new List<byte[]> { data };
        }

        #endregion

        #region Split By Single Byte

        /// <summary>
        /// Splits by a single byte (fast path for simple delimiters).
        /// Equivalent to string.Split(char) but for byte arrays.
        /// </summary>
        /// <param name="data">Source byte array.</param>
        /// <param name="delimiter">Single byte delimiter.</param>
        /// <param name="options">Split options.</param>
        /// <returns>List of byte arrays split by the delimiter byte.</returns>
        public static List<byte[]> SplitBySingleByte(byte[] data, byte delimiter, SplitOptions options = SplitOptions.None)
        {
            if (data == null || data.Length == 0)
            {
                return new List<byte[]>();
            }

            var result = new List<byte[]>();
            int startIndex = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == delimiter)
                {
                    int segmentLength = i - startIndex;

                    if (segmentLength > 0 || options == SplitOptions.None)
                    {
                        byte[] segment = new byte[segmentLength];
                        if (segmentLength > 0)
                        {
                            Array.Copy(data, startIndex, segment, 0, segmentLength);
                        }
                        result.Add(segment);
                    }

                    startIndex = i + 1;
                }
            }

            // Add remaining data
            if (startIndex < data.Length)
            {
                int remainingLength = data.Length - startIndex;
                byte[] remaining = new byte[remainingLength];
                Array.Copy(data, startIndex, remaining, 0, remainingLength);

                if (remainingLength > 0 || options == SplitOptions.None)
                {
                    result.Add(remaining);
                }
            }
            else if (startIndex == data.Length && options == SplitOptions.None)
            {
                result.Add(new byte[0]);
            }

            return result;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks if a terminator sequence matches at a specific position in the data.
        /// </summary>
        private static bool MatchesAt(byte[] data, byte[] terminator, int position)
        {
            if (position + terminator.Length > data.Length)
            {
                return false;
            }

            for (int i = 0; i < terminator.Length; i++)
            {
                if (data[position + i] != terminator[i])
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Enums

        /// <summary>
        /// Split options (mimics StringSplitOptions).
        /// </summary>
        public enum SplitOptions
        {
            /// <summary>
            /// Include empty array entries in the result.
            /// </summary>
            None = 0,

            /// <summary>
            /// Remove empty array entries from the result.
            /// </summary>
            RemoveEmptyEntries = 1
        }

        #endregion

        #region Conversion Helpers

        /// <summary>
        /// Converts string terminator to byte array using specified encoding.
        /// Helper for migration from string-based to byte-based splitting.
        /// </summary>
        /// <param name="terminator">String terminator (e.g., "\r\n").</param>
        /// <param name="encoding">Text encoding to use.</param>
        /// <returns>Byte array representation of terminator.</returns>
        public static byte[] StringToBytes(string terminator, System.Text.Encoding encoding)
        {
            if (string.IsNullOrEmpty(terminator))
            {
                return new byte[0];
            }

            return encoding.GetBytes(terminator);
        }

        /// <summary>
        /// Converts multiple string terminators to byte arrays.
        /// </summary>
        public static byte[][] StringsToBytes(string[] terminators, System.Text.Encoding encoding)
        {
            if (terminators == null || terminators.Length == 0)
            {
                return new byte[0][];
            }

            var result = new byte[terminators.Length][];
            for (int i = 0; i < terminators.Length; i++)
            {
                result[i] = StringToBytes(terminators[i], encoding);
            }

            return result;
        }

        #endregion
    }
}
