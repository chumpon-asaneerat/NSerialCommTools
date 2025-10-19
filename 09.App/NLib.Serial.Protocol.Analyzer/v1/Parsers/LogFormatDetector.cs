#region Using

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NLib.Serial.Protocol.Analyzer.Models;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Parsers
{
    /// <summary>
    /// Detects the format of a serial log file
    /// </summary>
    public class LogFormatDetector
    {
        #region Public Methods

        /// <summary>
        /// Detects the format of a log file
        /// </summary>
        /// <param name="filePath">Path to the log file</param>
        /// <returns>Detected format</returns>
        public LogFileFormat DetectFormat(string filePath)
        {
            if (!File.Exists(filePath))
                return LogFileFormat.Unknown;

            string[] lines = File.ReadAllLines(filePath).Take(10).ToArray();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Check for Format 1: Hex + ASCII
                // Pattern: "XX XX XX ... (4+ spaces) ASCII text"
                var hexAsciiMatch = Regex.Match(line,
                    @"^([0-9A-Fa-f]{2}\s+){8,}\s{4,}.+$");
                if (hexAsciiMatch.Success)
                    return LogFileFormat.HexAndAscii;

                // Check for Format 2: Pure Hex
                // Pattern: "XX XX XX XX" (only hex bytes)
                var pureHexMatch = Regex.Match(line,
                    @"^([0-9A-Fa-f]{2}\s*)+$");
                if (pureHexMatch.Success && line.Length > 16)
                    return LogFileFormat.PureHex;

                // Check for Format 3: Pure Text
                // Pattern: Contains non-hex characters
                if (!Regex.IsMatch(line, @"^[0-9A-Fa-f\s]+$"))
                    return LogFileFormat.PureText;
            }

            return LogFileFormat.Unknown;
        }

        #endregion
    }
}
