#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLib.Serial.Protocol.Analyzer.Models;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Analyzers
{
    /// <summary>
    /// Analyzes message lines in a purely data-driven way without any hardcoded assumptions.
    /// This analyzer works like a SCIENTIST: observe, measure, and report.
    /// It makes ZERO assumptions about device types or field meanings.
    /// </summary>
    public class DataDrivenLineAnalyzer
    {
        #region Public Methods

        /// <summary>
        /// Analyzes all fields from log data using delimiter (purely data-driven)
        /// </summary>
        public List<FieldInfo> AnalyzeFields(LogData data, char delimiter)
        {
            if (data == null || data.Messages == null || data.Messages.Count == 0)
                return new List<FieldInfo>();

            // Parse all messages into fields
            List<string[]> allFields = ParseMessages(data.Messages, delimiter);

            if (allFields.Count == 0)
                return new List<FieldInfo>();

            // Determine maximum field count observed
            int maxFields = allFields.Max(f => f.Length);

            // Analyze each position independently
            List<FieldInfo> results = new List<FieldInfo>();

            for (int position = 0; position < maxFields; position++)
            {
                FieldInfo fieldInfo = AnalyzePosition(position, allFields);
                if (fieldInfo != null)
                {
                    results.Add(fieldInfo);
                }
            }

            return results;
        }

        #endregion

        #region Private Methods - Message Parsing

        /// <summary>
        /// Parses all messages into field arrays
        /// </summary>
        private List<string[]> ParseMessages(List<byte[]> messages, char delimiter)
        {
            List<string[]> allFields = new List<string[]>();

            foreach (byte[] message in messages)
            {
                string text = Encoding.ASCII.GetString(message).Trim();
                string[] fields = text.Split(delimiter);
                allFields.Add(fields);
            }

            return allFields;
        }

        #endregion

        #region Private Methods - Position Analysis

        /// <summary>
        /// Analyzes a specific position across all messages
        /// </summary>
        private FieldInfo AnalyzePosition(int position, List<string[]> allFields)
        {
            // Extract all values at this position
            List<string> values = ExtractValuesAtPosition(position, allFields);

            if (values.Count == 0)
                return null;

            // Create field info with observed data only
            FieldInfo fieldInfo = new FieldInfo
            {
                Position = position,
                SuggestedName = $"Field{position}",

                // Length observations
                MinLength = values.Min(v => v.Length),
                MaxLength = values.Max(v => v.Length),
                IsConstantLength = values.All(v => v.Length == values[0].Length),

                // Value observations
                UniqueValues = values.Distinct().ToList(),
                IsConstant = values.Distinct().Count() == 1,

                // Confidence based on sample size
                Confidence = CalculateConfidence(values.Count)
            };

            // Infer data type from observed values
            InferDataType(fieldInfo, values);

            return fieldInfo;
        }

        /// <summary>
        /// Extracts all values at a specific position
        /// </summary>
        private List<string> ExtractValuesAtPosition(int position, List<string[]> allFields)
        {
            return allFields
                .Where(fields => position < fields.Length)
                .Select(fields => fields[position].Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .ToList();
        }

        #endregion

        #region Private Methods - Type Inference

        /// <summary>
        /// Infers data type based purely on observed values (no assumptions)
        /// </summary>
        private void InferDataType(FieldInfo field, List<string> values)
        {
            // Try to parse as integer
            if (TryInferInteger(field, values))
                return;

            // Try to parse as decimal
            if (TryInferDecimal(field, values))
                return;

            // Try to parse as datetime
            if (TryInferDateTime(field, values))
                return;

            // Default to string (no assumptions)
            field.Type = "string";
        }

        /// <summary>
        /// Attempts to infer integer type from values
        /// </summary>
        private bool TryInferInteger(FieldInfo field, List<string> values)
        {
            List<int> parsedValues = new List<int>();

            foreach (string value in values)
            {
                if (!int.TryParse(value, out int parsed))
                    return false;

                parsedValues.Add(parsed);
            }

            // All values parsed as integers
            field.Type = "integer";
            field.MinValue = parsedValues.Min();
            field.MaxValue = parsedValues.Max();
            return true;
        }

        /// <summary>
        /// Attempts to infer decimal type from values
        /// </summary>
        private bool TryInferDecimal(FieldInfo field, List<string> values)
        {
            List<decimal> parsedValues = new List<decimal>();

            foreach (string value in values)
            {
                // Try to parse the value directly
                if (decimal.TryParse(value, out decimal parsed))
                {
                    parsedValues.Add(parsed);
                    continue;
                }

                // Try to extract numeric part (in case of attached units)
                string numericPart = ExtractNumericPart(value);
                if (!string.IsNullOrEmpty(numericPart) && decimal.TryParse(numericPart, out parsed))
                {
                    parsedValues.Add(parsed);

                    // Detect unit attachment (but don't assume what it means)
                    if (numericPart != value)
                    {
                        field.UnitAttached = true;
                        field.Unit = value.Substring(numericPart.Length).Trim();
                    }
                    continue;
                }

                // Cannot parse this value as decimal
                return false;
            }

            // All values parsed as decimals
            field.Type = "decimal";
            field.MinValue = parsedValues.Min();
            field.MaxValue = parsedValues.Max();
            return true;
        }

        /// <summary>
        /// Extracts the numeric part from a string (handles attached units)
        /// </summary>
        private string ExtractNumericPart(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            StringBuilder numeric = new StringBuilder();

            foreach (char c in value)
            {
                if (char.IsDigit(c) || c == '.' || c == '-' || c == '+')
                    numeric.Append(c);
                else
                    break; // Stop at first non-numeric character
            }

            return numeric.ToString();
        }

        /// <summary>
        /// Attempts to infer datetime type from values
        /// </summary>
        private bool TryInferDateTime(FieldInfo field, List<string> values)
        {
            foreach (string value in values)
            {
                if (!DateTime.TryParse(value, out _))
                    return false;
            }

            // All values parsed as datetime
            field.Type = "datetime";
            return true;
        }

        #endregion

        #region Private Methods - Confidence Calculation

        /// <summary>
        /// Calculates confidence based on sample size (no subjective assumptions)
        /// </summary>
        private double CalculateConfidence(int sampleCount)
        {
            // Confidence based purely on sample size
            if (sampleCount >= 100) return 100.0;
            if (sampleCount >= 50) return 90.0;
            if (sampleCount >= 20) return 80.0;
            if (sampleCount >= 10) return 70.0;
            if (sampleCount >= 5) return 60.0;
            return 50.0;
        }

        #endregion
    }
}
