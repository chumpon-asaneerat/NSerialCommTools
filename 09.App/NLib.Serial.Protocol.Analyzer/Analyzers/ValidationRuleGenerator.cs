#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NLib.Serial.ProtocolAnalyzer.Models;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Analyzers
{
    /// <summary>
    /// Generates validation rules based on detected fields and relationships.
    /// Creates Range, Formula, and Relationship validation rules.
    /// </summary>
    public class ValidationRuleGenerator
    {
        #region Public Methods

        /// <summary>
        /// Generates validation rules for fields and relationships.
        /// </summary>
        /// <param name="fields">List of detected fields.</param>
        /// <param name="relationships">List of detected relationships.</param>
        /// <returns>List of validation rules.</returns>
        public List<NLib.Serial.ProtocolAnalyzer.Models.ValidationRule> GenerateRules(
            List<FieldInfo> fields,
            List<FieldRelationship> relationships)
        {
            var rules = new List<NLib.Serial.ProtocolAnalyzer.Models.ValidationRule>();

            if (fields == null)
            {
                return rules;
            }

            // 1. Generate Range Validation Rules for numeric fields
            var rangeRules = GenerateRangeRules(fields);
            rules.AddRange(rangeRules);

            // 2. Generate Formula Validation Rules from Calculate relationships
            if (relationships != null)
            {
                var formulaRules = GenerateFormulaRules(relationships);
                rules.AddRange(formulaRules);

                // 3. Generate Relationship Validation Rules
                var relationshipRules = GenerateRelationshipRules(relationships);
                rules.AddRange(relationshipRules);
            }

            return rules;
        }

        #endregion

        #region Range Validation

        /// <summary>
        /// Generates range validation rules for numeric fields based on sample data.
        /// </summary>
        private List<NLib.Serial.ProtocolAnalyzer.Models.ValidationRule> GenerateRangeRules(List<FieldInfo> fields)
        {
            var rules = new List<NLib.Serial.ProtocolAnalyzer.Models.ValidationRule>();

            foreach (var field in fields)
            {
                // Only generate range rules for numeric fields with data
                if (field.Action != "Parse" || field.SampleValues == null || field.SampleValues.Count == 0)
                {
                    continue;
                }

                if (field.DataType == "decimal" || field.DataType == "int" || field.FieldType == "Decimal" || field.FieldType == "Integer")
                {
                    var values = ExtractNumericValues(field.SampleValues);

                    if (values.Count > 0)
                    {
                        // Calculate min/max with some buffer
                        decimal minValue = values.Min();
                        decimal maxValue = values.Max();

                        // Add 10% buffer to accommodate variations
                        decimal range = maxValue - minValue;
                        decimal buffer = range * 0.1m;

                        if (buffer < 0.1m) buffer = 0.1m; // Minimum buffer

                        var rule = new NLib.Serial.ProtocolAnalyzer.Models.ValidationRule
                        {
                            Name = $"{field.Name}Range",
                            Type = "Range",
                            Field = field.Name,
                            Severity = "Warning",
                            Message = $"{field.Name} should be between {minValue - buffer:F2} and {maxValue + buffer:F2}",
                            MinValue = (double)(minValue - buffer),
                            MaxValue = (double)(maxValue + buffer)
                        };

                        rules.Add(rule);
                    }
                }
            }

            return rules;
        }

        /// <summary>
        /// Extracts numeric values from sample strings.
        /// </summary>
        private List<decimal> ExtractNumericValues(List<string> samples)
        {
            var values = new List<decimal>();

            foreach (var sample in samples)
            {
                if (string.IsNullOrWhiteSpace(sample))
                {
                    continue;
                }

                // Extract first number
                var match = Regex.Match(sample, @"[+-]?\d+\.?\d*");
                if (match.Success && decimal.TryParse(match.Value, out decimal value))
                {
                    values.Add(value);
                }
            }

            return values;
        }

        #endregion

        #region Formula Validation

        /// <summary>
        /// Generates formula validation rules from Calculate relationships.
        /// Example: GrossWeight - TareWeight = NetWeight
        /// </summary>
        private List<NLib.Serial.ProtocolAnalyzer.Models.ValidationRule> GenerateFormulaRules(List<FieldRelationship> relationships)
        {
            var rules = new List<NLib.Serial.ProtocolAnalyzer.Models.ValidationRule>();

            foreach (var relationship in relationships)
            {
                if (relationship.Type == "Calculate" && !string.IsNullOrEmpty(relationship.Operation))
                {
                    // Build formula string
                    string formula = $"{relationship.Operation} = {relationship.TargetField}";

                    var rule = new NLib.Serial.ProtocolAnalyzer.Models.ValidationRule
                    {
                        Name = $"{relationship.TargetField}Formula",
                        Type = "Formula",
                        Field = relationship.TargetField,
                        Severity = "Error",
                        Message = $"Formula validation failed: {formula}",
                        Formula = formula,
                        Tolerance = relationship.Tolerance
                    };

                    rules.Add(rule);
                }
            }

            return rules;
        }

        #endregion

        #region Relationship Validation

        /// <summary>
        /// Generates relationship validation rules.
        /// Example: GrossWeight >= TareWeight
        /// </summary>
        private List<NLib.Serial.ProtocolAnalyzer.Models.ValidationRule> GenerateRelationshipRules(List<FieldRelationship> relationships)
        {
            var rules = new List<NLib.Serial.ProtocolAnalyzer.Models.ValidationRule>();

            foreach (var relationship in relationships)
            {
                if (relationship.Type == "Calculate")
                {
                    // For weight calculations, gross should be >= tare
                    if (relationship.SourceFields != null && relationship.SourceFields.Count >= 2)
                    {
                        var field1 = relationship.SourceFields[0]; // Gross
                        var field2 = relationship.SourceFields[1]; // Tare

                        if (field1.IndexOf("Gross", StringComparison.OrdinalIgnoreCase) >= 0 &&
                            field2.IndexOf("Tare", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var rule = new NLib.Serial.ProtocolAnalyzer.Models.ValidationRule
                            {
                                Name = "GrossGreaterThanTare",
                                Type = "Relationship",
                                Field = field1,
                                Severity = "Error",
                                Message = $"{field1} must be greater than or equal to {field2}",
                                Condition = $"{field1} >= {field2}"
                            };

                            rules.Add(rule);
                        }
                    }
                }
            }

            return rules;
        }

        #endregion
    }
}
