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
        /// Analyzes fields based on the detected delimiter.
        /// </summary>
        public List<FieldInfo> Analyze(LogData logData, DelimiterInfo delimiter)
        {
            if (logData == null || logData.Messages.Count == 0 || delimiter == null)
                return new List<FieldInfo>();

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
