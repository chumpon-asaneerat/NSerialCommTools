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
    /// Analyzes fields in messages
    /// </summary>
    public class FieldAnalyzer
    {
        #region Public Methods

        /// <summary>
        /// Analyzes fields from log data using delimiter
        /// </summary>
        public List<FieldInfo> AnalyzeFields(LogData data, char delimiter)
        {
            if (data == null || data.Messages == null || data.Messages.Count == 0)
                return new List<FieldInfo>();

            // Split all messages by delimiter
            List<string[]> allFields = new List<string[]>();

            foreach (byte[] message in data.Messages)
            {
                string text = Encoding.ASCII.GetString(message).Trim();
                string[] fields = text.Split(delimiter);
                allFields.Add(fields);
            }

            // Determine max field count
            int maxFields = allFields.Max(f => f.Length);

            // Analyze each field position
            List<FieldInfo> results = new List<FieldInfo>();

            for (int i = 0; i < maxFields; i++)
            {
                var fieldValues = allFields
                    .Where(fields => i < fields.Length)
                    .Select(fields => fields[i].Trim())
                    .Where(v => !string.IsNullOrEmpty(v))
                    .ToList();

                if (fieldValues.Count == 0)
                    continue;

                FieldInfo fieldInfo = new FieldInfo
                {
                    Position = i,
                    SuggestedName = $"Field{i}",
                    MinLength = fieldValues.Min(v => v.Length),
                    MaxLength = fieldValues.Max(v => v.Length),
                    IsConstantLength = fieldValues.All(v => v.Length == fieldValues[0].Length),
                    UniqueValues = fieldValues.Distinct().Take(10).ToList(),
                    IsConstant = fieldValues.Distinct().Count() == 1
                };

                // Infer type
                InferFieldType(fieldInfo, fieldValues);

                // Calculate confidence
                fieldInfo.Confidence = CalculateConfidence(fieldInfo, fieldValues);

                results.Add(fieldInfo);
            }

            return results;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Infers the field type from values
        /// </summary>
        private void InferFieldType(FieldInfo field, List<string> values)
        {
            // Try integer
            if (values.All(v => int.TryParse(v, out _)))
            {
                field.Type = "integer";
                var intValues = values.Select(v => int.Parse(v)).ToList();
                field.MinValue = intValues.Min();
                field.MaxValue = intValues.Max();
                return;
            }

            // Try decimal
            if (values.All(v => decimal.TryParse(v.TrimEnd('g', 'k', 'l', 'b', ' '),
                out _)))
            {
                field.Type = "decimal";

                // Check for unit
                var sample = values.First();
                if (sample.EndsWith("g") || sample.EndsWith("kg") ||
                    sample.EndsWith("lb"))
                {
                    field.UnitAttached = true;
                    field.Unit = sample.Substring(sample.Length - (sample.EndsWith("kg") ? 2 : 1));
                }

                return;
            }

            // Try datetime
            if (values.All(v => DateTime.TryParse(v, out _)))
            {
                field.Type = "datetime";
                return;
            }

            // Default to string
            field.Type = "string";
        }

        /// <summary>
        /// Calculates confidence for field analysis
        /// </summary>
        private double CalculateConfidence(FieldInfo field, List<string> values)
        {
            double confidence = 50.0; // Base confidence

            // More samples = higher confidence
            if (values.Count > 10) confidence += 20;
            if (values.Count > 50) confidence += 10;

            // Constant length increases confidence
            if (field.IsConstantLength) confidence += 10;

            // Known type increases confidence
            if (field.Type != "string") confidence += 10;

            return Math.Min(100, confidence);
        }

        #endregion
    }
}
