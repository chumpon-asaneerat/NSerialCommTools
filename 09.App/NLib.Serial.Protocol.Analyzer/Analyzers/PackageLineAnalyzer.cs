#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NLib.Serial.Protocol.Analyzer.Models;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Analyzers
{
    /// <summary>
    /// Analyzes individual lines within a multi-line package
    /// </summary>
    public class PackageLineAnalyzer
    {
        #region Public Methods

        /// <summary>
        /// Analyzes the structure of lines within a package
        /// </summary>
        public List<LineStructure> AnalyzePackageLines(LogData data, PackageInfo packageInfo)
        {
            if (packageInfo == null || !packageInfo.IsPackageBased)
                return new List<LineStructure>();

            var lines = new List<LineStructure>();

            // Get the first package's messages
            var messages = data.Messages
                .Take(packageInfo.PackageSize)
                .Select(m => Encoding.ASCII.GetString(m).Trim())
                .ToList();

            for (int i = 0; i < messages.Count; i++)
            {
                string line = messages[i];
                int lineNumber = i + 1;

                var lineStructure = new LineStructure
                {
                    LineNumber = lineNumber,
                    RawSample = line
                };

                // Analyze line content
                AnalyzeLineContent(lineStructure, line, lineNumber, packageInfo);

                lines.Add(lineStructure);
            }

            return lines;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Analyzes the content of a single line
        /// </summary>
        private void AnalyzeLineContent(LineStructure structure, string line, int lineNumber, PackageInfo packageInfo)
        {
            // Check if this is a marker line
            if (lineNumber == 1 && line == packageInfo.StartMarker)
            {
                structure.Name = "StartMarker";
                structure.Type = "marker";
                structure.Pattern = Regex.Escape(line);
                structure.Required = true;
                structure.Description = "Package start marker with device ID";
                return;
            }

            if (lineNumber == packageInfo.PackageSize && line == packageInfo.EndMarker)
            {
                structure.Name = "EndMarker";
                structure.Type = "marker";
                structure.Pattern = Regex.Escape(line);
                structure.Required = true;
                structure.Description = "Package end marker";
                return;
            }

            // Check for date pattern (YYYY-MM-DD)
            if (Regex.IsMatch(line, @"^\d{4}-\d{2}-\d{2}$"))
            {
                structure.Name = "Date";
                structure.Type = "datetime";
                structure.Format = "yyyy-MM-dd";
                structure.Pattern = @"\d{4}-\d{2}-\d{2}";
                structure.Required = true;
                structure.Description = "Measurement date";
                return;
            }

            // Check for time pattern (HH:mm:ss)
            if (Regex.IsMatch(line, @"^\d{2}:\d{2}:\d{2}$"))
            {
                structure.Name = "Time";
                structure.Type = "datetime";
                structure.Format = "HH:mm:ss";
                structure.Pattern = @"\d{2}:\d{2}:\d{2}";
                structure.Required = true;
                structure.Description = "Measurement time";
                return;
            }

            // Check for weight pattern (decimal + unit)
            var weightMatch = Regex.Match(line, @"\s*(\d+\.\d+)\s*(kg|g|lb)");
            if (weightMatch.Success)
            {
                string fieldName = DetermineWeightFieldName(lineNumber);
                structure.Name = fieldName;
                structure.Type = "decimal";
                structure.Pattern = @"\s*(\d+\.\d+)\s*" + weightMatch.Groups[2].Value;
                structure.Unit = weightMatch.Groups[2].Value;
                structure.UnitAttached = true;
                structure.Format = "F2";
                structure.Min = 0;
                structure.Max = 999.99m;
                structure.Required = true;
                structure.Description = GetWeightDescription(fieldName);
                return;
            }

            // Check for piece count pattern
            var pcsMatch = Regex.Match(line, @"\s*(\d+)\s*pcs");
            if (pcsMatch.Success)
            {
                structure.Name = "PieceCount";
                structure.Type = "integer";
                structure.Pattern = @"\s*(\d+)\s*pcs";
                structure.Unit = "pcs";
                structure.UnitAttached = true;
                structure.Min = 0;
                structure.Max = 99999;
                structure.Required = false;
                structure.Description = "Piece count";
                return;
            }

            // Check for single character (status indicator)
            if (line.Length == 1 && char.IsLetter(line[0]))
            {
                structure.Name = "StatusIndicator";
                structure.Type = "string";
                structure.Pattern = "[A-Z]";
                structure.Values = new List<string> { "E", "S", "N" };
                structure.Required = false;
                structure.Description = "Status indicator";
                return;
            }

            // Check for numeric only (reserved fields)
            if (Regex.IsMatch(line, @"^\d+$"))
            {
                structure.Name = $"Reserved{lineNumber}";
                structure.Type = "string";
                structure.Pattern = @"\d+";
                structure.Required = false;
                structure.Description = "Reserved field";
                return;
            }

            // Empty or whitespace line
            if (string.IsNullOrWhiteSpace(line))
            {
                structure.Name = $"Empty{lineNumber}";
                structure.Type = "string";
                structure.Pattern = @"\s*";
                structure.Required = false;
                structure.Description = "Empty line or whitespace";
                return;
            }

            // Unknown pattern
            structure.Name = $"Field{lineNumber}";
            structure.Type = "string";
            structure.Pattern = ".*";
            structure.Required = false;
            structure.Description = "Unknown field";
        }

        /// <summary>
        /// Determines weight field name based on line position
        /// </summary>
        private string DetermineWeightFieldName(int lineNumber)
        {
            switch (lineNumber)
            {
                case 4: return "TareWeight";
                case 5: return "GrossWeight";
                case 8: return "NetWeight";
                case 9: return "DisplayWeight";
                default: return $"Weight{lineNumber}";
            }
        }

        /// <summary>
        /// Gets description for weight field
        /// </summary>
        private string GetWeightDescription(string fieldName)
        {
            switch (fieldName)
            {
                case "TareWeight": return "Tare Weight (TW)";
                case "GrossWeight": return "Gross Weight (GW)";
                case "NetWeight": return "Net Weight (NW) = GW - TW";
                case "DisplayWeight": return "Display weight (duplicate of NW)";
                default: return "Weight measurement";
            }
        }

        #endregion
    }
}
