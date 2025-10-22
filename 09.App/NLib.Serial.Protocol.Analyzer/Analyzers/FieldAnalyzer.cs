#region Using

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NLib.Serial.ProtocolAnalyzer.Models;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Analyzers
{
    /// <summary>
    /// Analyzes and classifies fields in serial messages.
    /// </summary>
    public class FieldAnalyzer
    {
        #region Public Methods

        /// <summary>
        /// Analyzes fields based on the message structure.
        /// Following Algorithm 4: Multi-Line Frame Field Extraction
        /// </summary>
        public List<FieldInfo> Analyze(LogData logData, DelimiterInfo delimiter)
        {
            if (logData == null || logData.Messages.Count == 0)
                return new List<FieldInfo>();

            // Check if messages are multi-line (contain \r\n or \n inside)
            bool isMultiLine = CheckIfMultiLine(logData);

            if (isMultiLine)
            {
                // Algorithm 4: Frame-based extraction
                // Each line position is a field
                return AnalyzeMultiLineFrames(logData);
            }

            // If delimiter is not found or confidence is low, try analyzing by lines
            if (delimiter == null || delimiter.Confidence < 0.6)
            {
                // Still might be multi-line without good delimiters
                return AnalyzeMultiLine(logData);
            }

            // Single-line delimiter-based analysis
            return AnalyzeDelimiterBased(logData, delimiter);
        }

        /// <summary>
        /// Check if messages contain line breaks (multi-line frames)
        /// </summary>
        private bool CheckIfMultiLine(LogData logData)
        {
            foreach (var message in logData.Messages)
            {
                string text = Encoding.ASCII.GetString(message);
                // Count line breaks
                int lineBreaks = text.Count(c => c == '\n' || c == '\r');
                if (lineBreaks > 1) // More than just trailing terminator
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Algorithm 4: Extract fields from multi-line frames
        /// Each line position is treated as a separate field
        /// </summary>
        private List<FieldInfo> AnalyzeMultiLineFrames(LogData logData)
        {
            // STEP 1: Collect samples for each line position
            var fieldsByLineNumber = new Dictionary<int, List<string>>();

            foreach (var message in logData.Messages)
            {
                string text = Encoding.ASCII.GetString(message);
                string[] lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

                for (int lineNum = 0; lineNum < lines.Length; lineNum++)
                {
                    string line = lines[lineNum];

                    // Skip completely empty lines at the end
                    if (lineNum >= lines.Length - 2 && string.IsNullOrWhiteSpace(line))
                        continue;

                    if (!fieldsByLineNumber.ContainsKey(lineNum))
                        fieldsByLineNumber[lineNum] = new List<string>();

                    fieldsByLineNumber[lineNum].Add(line);
                }
            }

            // STEP 2: Analyze each line position using AnalyzeLinePattern
            var results = new List<FieldInfo>();

            foreach (var kvp in fieldsByLineNumber.OrderBy(x => x.Key))
            {
                int position = kvp.Key;
                List<string> samples = kvp.Value;

                if (samples.Count == 0)
                    continue;

                // Use AnalyzeLinePattern from Algorithm 4
                var fieldInfo = AnalyzeLinePattern(position, samples);
                results.Add(fieldInfo);
            }

            return results;
        }

        /// <summary>
        /// Algorithm 4: AnalyzeLinePattern
        /// Detects the field type and generates Name based on pattern
        /// </summary>
        private FieldInfo AnalyzeLinePattern(int lineNumber, List<string> samples)
        {
            var fieldInfo = new FieldInfo
            {
                Order = lineNumber,
                SampleValues = samples.Distinct().Take(5).ToList(),
                MinLength = samples.Min(s => s.Length),
                MaxLength = samples.Max(s => s.Length),
                IsConstant = samples.Distinct().Count() == 1
            };

            // Check for Marker Lines
            if (lineNumber == 0)
            {
                fieldInfo.Name = "StartMarker";
                fieldInfo.DataType = "string";
                fieldInfo.Confidence = 1.0;
                return fieldInfo;
            }

            var uniqueSamples = samples.Distinct().ToList();
            if (uniqueSamples.Count == 1 && uniqueSamples[0].Length < 5)
            {
                fieldInfo.Name = "EndMarker";
                fieldInfo.DataType = "string";
                fieldInfo.Confidence = 1.0;
                return fieldInfo;
            }

            // Check for Empty/Whitespace Lines
            if (samples.All(s => string.IsNullOrWhiteSpace(s)))
            {
                fieldInfo.Name = "Empty";
                fieldInfo.DataType = "string";
                fieldInfo.Confidence = 1.0;
                return fieldInfo;
            }

            // Detect Data Patterns
            var patterns = new[]
            {
                new { Name = "Date", Regex = new System.Text.RegularExpressions.Regex(@"^\d{4}-\d{2}-\d{2}$") },
                new { Name = "Time", Regex = new System.Text.RegularExpressions.Regex(@"^\d{2}:\d{2}:\d{2}$") },
                new { Name = "WeightKg", Regex = new System.Text.RegularExpressions.Regex(@"^\s*[+-]?\d+\.\d+\s*kg\s*$") },
                new { Name = "WeightG", Regex = new System.Text.RegularExpressions.Regex(@"^\s*[+-]?\d+\.\d+\s*g\s*$") },
                new { Name = "CountPcs", Regex = new System.Text.RegularExpressions.Regex(@"^\s*\d+\s*pcs\s*$") },
                new { Name = "Decimal", Regex = new System.Text.RegularExpressions.Regex(@"^\s*[+-]?\d+\.\d+\s*$") },
                new { Name = "Integer", Regex = new System.Text.RegularExpressions.Regex(@"^\s*\d+\s*$") }
            };

            foreach (var pattern in patterns)
            {
                int matchCount = samples.Count(s => pattern.Regex.IsMatch(s));
                double matchRate = (double)matchCount / samples.Count;

                if (matchRate > 0.9)
                {
                    fieldInfo.Name = pattern.Name;
                    fieldInfo.DataType = pattern.Name.Contains("Weight") || pattern.Name.Contains("Count") || pattern.Name.Contains("Decimal") ? "decimal" :
                                     pattern.Name == "Integer" ? "int" : "string";
                    fieldInfo.Confidence = matchRate;
                    return fieldInfo;
                }
            }

            // No Pattern Matched - Check variance
            double variance = CalculateVariance(samples);

            if (variance < 0.1)
            {
                fieldInfo.Name = "Reserved";
                fieldInfo.DataType = "string";
                fieldInfo.Confidence = 0.7;
            }
            else
            {
                fieldInfo.Name = "Unknown";
                fieldInfo.DataType = "string";
                fieldInfo.Confidence = 0.5;
            }

            return fieldInfo;
        }

        private double CalculateVariance(List<string> samples)
        {
            if (samples.Count == 0)
                return 0;

            var uniqueCount = samples.Distinct().Count();
            return (double)uniqueCount / samples.Count;
        }

        /// <summary>
        /// Delimiter-based field analysis for single-line messages
        /// </summary>
        private List<FieldInfo> AnalyzeDelimiterBased(LogData logData, DelimiterInfo delimiter)
        {
            char delim = delimiter.Character;
            var fieldData = new Dictionary<int, List<string>>();

            // Split each message by the delimiter
            foreach (var message in logData.Messages)
            {
                string text = Encoding.ASCII.GetString(message).Trim();
                string[] parts = text.Split(delim);

                for (int i = 0; i < parts.Length; i++)
                {
                    if (!fieldData.ContainsKey(i))
                        fieldData[i] = new List<string>();

                    fieldData[i].Add(parts[i].Trim());
                }
            }

            // Analyze each field
            var results = new List<FieldInfo>();
            foreach (var kvp in fieldData.OrderBy(x => x.Key))
            {
                int position = kvp.Key;
                List<string> values = kvp.Value;

                var fieldInfo = new FieldInfo
                {
                    Order = position,
                    Name = $"Field{position}",
                    SampleValues = values.Distinct().Take(10).ToList(),
                    MinLength = values.Min(v => v.Length),
                    MaxLength = values.Max(v => v.Length),
                    IsConstant = values.Distinct().Count() == 1
                };

                // Infer type
                InferFieldType(fieldInfo, values);

                results.Add(fieldInfo);
            }

            return results;
        }

        /// <summary>
        /// Analyzes multi-line messages where each line is a field.
        /// </summary>
        private List<FieldInfo> AnalyzeMultiLine(LogData logData)
        {
            var fieldData = new Dictionary<int, List<string>>();

            // Split each message by line breaks
            foreach (var message in logData.Messages)
            {
                string text = Encoding.ASCII.GetString(message);
                string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    if (!fieldData.ContainsKey(i))
                        fieldData[i] = new List<string>();

                    fieldData[i].Add(line);
                }
            }

            // Analyze each field (line)
            var results = new List<FieldInfo>();
            foreach (var kvp in fieldData.OrderBy(x => x.Key))
            {
                int position = kvp.Key;
                List<string> values = kvp.Value;

                if (values.Count == 0)
                    continue;

                var fieldInfo = new FieldInfo
                {
                    Order = position,
                    Name = $"Line{position}",
                    SampleValues = values.Distinct().Take(5).ToList(),
                    MinLength = values.Min(v => v.Length),
                    MaxLength = values.Max(v => v.Length),
                    IsConstant = values.Distinct().Count() == 1
                };

                // Infer type
                InferFieldType(fieldInfo, values);

                results.Add(fieldInfo);
            }

            return results;
        }

        #endregion

        #region Private Methods

        private void InferFieldType(FieldInfo fieldInfo, List<string> values)
        {
            int intCount = 0;
            int decimalCount = 0;
            int stringCount = 0;

            foreach (string value in values)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    stringCount++;
                    continue;
                }

                if (int.TryParse(value, out _))
                {
                    intCount++;
                }
                else if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    decimalCount++;
                }
                else
                {
                    stringCount++;
                }
            }

            int total = values.Count;
            double intRatio = (double)intCount / total;
            double decimalRatio = (double)decimalCount / total;

            if (intRatio > 0.9)
            {
                fieldInfo.DataType = "int";
                fieldInfo.Confidence = intRatio;
            }
            else if (decimalRatio > 0.9 || (intCount + decimalCount) > total * 0.9)
            {
                fieldInfo.DataType = "decimal";
                fieldInfo.Confidence = (intRatio + decimalRatio) / (double)total;
            }
            else
            {
                fieldInfo.DataType = "string";
                fieldInfo.Confidence = 0.95;
            }
        }

        #endregion
    }
}
