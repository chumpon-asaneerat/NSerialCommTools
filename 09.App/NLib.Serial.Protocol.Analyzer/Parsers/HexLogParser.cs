#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Parsers
{
    /// <summary>
    /// Parses hex log files into raw byte arrays.
    /// </summary>
    public class HexLogParser
    {
        #region Public Methods

        /// <summary>
        /// Parses a hex log file and returns the raw bytes.
        /// </summary>
        public byte[] ParseLogFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Log file not found.", filePath);

            string content = File.ReadAllText(filePath);

            // Try different formats
            if (IsHexDumpFormat(content))
            {
                return ParseHexDump(content);
            }
            else if (IsPureHexFormat(content))
            {
                return ParsePureHex(content);
            }
            else
            {
                // Assume plain text
                return Encoding.ASCII.GetBytes(content);
            }
        }

        #endregion

        #region Private Methods

        private bool IsHexDumpFormat(string content)
        {
            // Check for hex dump format (e.g., "5E 4B 4A 49 4B 30" or "5E:4B:4A:49")
            // Must have at least 4 consecutive hex byte patterns (8+ bytes in sequence)
            // This prevents false positives from timestamps like "17:19:38"
            return Regex.IsMatch(content, @"[0-9A-Fa-f]{2}[\s:][0-9A-Fa-f]{2}[\s:][0-9A-Fa-f]{2}[\s:][0-9A-Fa-f]{2}");
        }

        private bool IsPureHexFormat(string content)
        {
            // Check for continuous hex (e.g., "00010203")
            return Regex.IsMatch(content.Replace("\r", "").Replace("\n", "").Replace(" ", ""), @"^[0-9A-Fa-f]+$");
        }

        private byte[] ParseHexDump(string content)
        {
            var bytes = new List<byte>();

            // Split by lines to process each line
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                // Check if this is a hex dump line with text preview (e.g., "5E 4B 4A 49 ... ^KJIK")
                // Extract only the hex part (before the text preview)
                string hexPart = line;

                // Look for the text preview separator (usually multiple spaces or a specific character)
                // Common formats:
                // "5E 4B 4A 49    ^KJIK" (multiple spaces)
                // "5E 4B 4A 49  |  ^KJIK" (pipe separator)
                int textStartIndex = FindTextPreviewStart(line);

                if (textStartIndex > 0)
                {
                    hexPart = line.Substring(0, textStartIndex);
                }

                // Extract all hex bytes from this line (only from hex part)
                var matches = Regex.Matches(hexPart, @"[0-9A-Fa-f]{2}");

                foreach (Match match in matches)
                {
                    bytes.Add(Convert.ToByte(match.Value, 16));
                }
            }

            return bytes.ToArray();
        }

        /// <summary>
        /// Finds where the text preview starts in a hex dump line.
        /// Returns -1 if no text preview detected.
        /// </summary>
        private int FindTextPreviewStart(string line)
        {
            // Look for common patterns that indicate text preview start:
            // 1. Four or more consecutive spaces (hex bytes are separated by 1-2 spaces)
            // 2. A pipe character |
            // 3. Two spaces followed by non-hex characters

            var match = Regex.Match(line, @"    "); // 4+ spaces
            if (match.Success)
            {
                return match.Index;
            }

            // No text preview found
            return -1;
        }

        private byte[] ParsePureHex(string content)
        {
            string hex = content.Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace("\t", "");
            var bytes = new List<byte>();

            for (int i = 0; i < hex.Length; i += 2)
            {
                if (i + 1 < hex.Length)
                {
                    bytes.Add(Convert.ToByte(hex.Substring(i, 2), 16));
                }
            }

            return bytes.ToArray();
        }

        #endregion
    }
}
