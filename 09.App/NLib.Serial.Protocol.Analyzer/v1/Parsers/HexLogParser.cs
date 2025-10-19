#region Using

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NLib.Serial.Protocol.Analyzer.Models;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Parsers
{
    /// <summary>
    /// Parses hex log files in various formats
    /// </summary>
    public class HexLogParser
    {
        #region Private Fields

        private LogFormatDetector _detector = new LogFormatDetector();

        #endregion

        #region Public Methods

        /// <summary>
        /// Parses a log file and returns raw bytes
        /// </summary>
        /// <param name="filePath">Path to the log file</param>
        /// <returns>Raw byte array</returns>
        public byte[] ParseLogFile(string filePath)
        {
            LogFileFormat format = _detector.DetectFormat(filePath);
            string content = File.ReadAllText(filePath);

            switch (format)
            {
                case LogFileFormat.HexAndAscii:
                    return ParseHexAndAscii(content);

                case LogFileFormat.PureHex:
                    return ParsePureHex(content);

                case LogFileFormat.PureText:
                    return ParsePureText(content);

                default:
                    throw new InvalidOperationException(
                        "Unable to detect log file format");
            }
        }

        #endregion

        #region Private Methods - Format Parsers

        /// <summary>
        /// Parses Format 1: Hex + ASCII
        /// </summary>
        private byte[] ParseHexAndAscii(string content)
        {
            List<byte> bytes = new List<byte>();
            string[] lines = content.Split(new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                // Split by multiple spaces (4+) to separate hex from ASCII
                string[] parts = Regex.Split(line, @"\s{4,}");

                if (parts.Length < 1)
                    continue;

                // Extract hex part (left side)
                string hexPart = parts[0].Trim();

                // Parse hex bytes
                string[] hexBytes = hexPart.Split(new[] { ' ', '\t' },
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (string hex in hexBytes)
                {
                    if (hex.Length == 2 &&
                        byte.TryParse(hex, NumberStyles.HexNumber,
                            CultureInfo.InvariantCulture, out byte b))
                    {
                        bytes.Add(b);
                    }
                }
            }

            return bytes.ToArray();
        }

        /// <summary>
        /// Parses Format 2: Pure Hex
        /// </summary>
        private byte[] ParsePureHex(string content)
        {
            List<byte> bytes = new List<byte>();

            // Extract all hex patterns (XX format)
            MatchCollection matches = Regex.Matches(content, @"[0-9A-Fa-f]{2}");

            foreach (Match match in matches)
            {
                if (byte.TryParse(match.Value, NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture, out byte b))
                {
                    bytes.Add(b);
                }
            }

            return bytes.ToArray();
        }

        /// <summary>
        /// Parses Format 3: Pure Text
        /// </summary>
        private byte[] ParsePureText(string content)
        {
            // Already in text format, just convert to bytes
            return Encoding.ASCII.GetBytes(content);
        }

        #endregion
    }
}
