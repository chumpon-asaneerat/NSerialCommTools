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

            // STEP 2: Analyze each line position
            var results = new List<FieldInfo>();

            foreach (var kvp in fieldsByLineNumber.OrderBy(x => x.Key))
            {
                int position = kvp.Key;
                List<string> samples = kvp.Value;

                if (samples.Count == 0)
                    continue;

                var fieldInfo = new FieldInfo
                {
                    Position = position,
                    AutoName = $"Line{position}",
                    SampleValues = samples.Distinct().Take(5).ToList(),
                    MinLength = samples.Min(s => s.Length),
                    MaxLength = samples.Max(s => s.Length),
                    IsConstant = samples.Distinct().Count() == 1
                };

                // Infer type based on line content
                InferFieldType(fieldInfo, samples);

                results.Add(fieldInfo);
            }

            return results;
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
                    Position = position,
                    AutoName = $"Field{position}",
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
                    Position = position,
                    AutoName = $"Line{position}",
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
                fieldInfo.Type = "int";
                fieldInfo.Confidence = intRatio;
            }
            else if (decimalRatio > 0.9 || (intCount + decimalCount) > total * 0.9)
            {
                fieldInfo.Type = "decimal";
                fieldInfo.Confidence = (intRatio + decimalRatio) / (double)total;
            }
            else
            {
                fieldInfo.Type = "string";
                fieldInfo.Confidence = 0.95;
            }
        }

        #endregion
    }
}
