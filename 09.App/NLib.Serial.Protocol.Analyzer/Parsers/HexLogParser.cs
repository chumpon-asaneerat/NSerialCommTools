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
            // Check for hex dump format (e.g., "00 01 02 03" or "00:01:02:03")
            return Regex.IsMatch(content, @"[0-9A-Fa-f]{2}[\s:][0-9A-Fa-f]{2}");
        }

        private bool IsPureHexFormat(string content)
        {
            // Check for continuous hex (e.g., "00010203")
            return Regex.IsMatch(content.Replace("\r", "").Replace("\n", "").Replace(" ", ""), @"^[0-9A-Fa-f]+$");
        }

        private byte[] ParseHexDump(string content)
        {
            var bytes = new List<byte>();
            var matches = Regex.Matches(content, @"[0-9A-Fa-f]{2}");

            foreach (Match match in matches)
            {
                bytes.Add(Convert.ToByte(match.Value, 16));
            }

            return bytes.ToArray();
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
