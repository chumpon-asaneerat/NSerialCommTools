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
    /// Detects relationships between fields.
    /// Handles: Split (value+unit), Combine (Date+Time), Calculate (formulas).
    /// Based on Document 03: Parsing Strategy Analysis - Algorithm 6.
    /// </summary>
    public class RelationshipDetector
    {
        #region Public Methods

        /// <summary>
        /// Detects all relationships between fields.
        /// Returns list of FieldRelationship objects and may modify the fields list.
        /// </summary>
        /// <param name="fields">List of detected fields (may be modified to add split fields).</param>
        /// <returns>List of detected relationships.</returns>
        public List<FieldRelationship> DetectRelationships(List<FieldInfo> fields)
        {
            var relationships = new List<FieldRelationship>();

            if (fields == null || fields.Count == 0)
            {
                return relationships;
            }

            // 1. Detect Split Fields (Compound → Value + Unit)
            var splitResults = DetectSplitFields(fields);
            relationships.AddRange(splitResults.Relationships);

            // Add newly created split fields to the main list
            fields.AddRange(splitResults.NewFields);

            // 2. Detect Combine Fields (Date + Time → DateTime)
            var combineRelationships = DetectCombineFields(fields);
            relationships.AddRange(combineRelationships);

            // 3. Detect Calculate Fields (GrossWeight - TareWeight = NetWeight)
            var calculateRelationships = DetectCalculateFields(fields);
            relationships.AddRange(calculateRelationships);

            return relationships;
        }

        #endregion

        #region Split Detection (Value + Unit)

        /// <summary>
        /// Result of split detection containing both relationships and newly created fields.
        /// </summary>
        private class SplitDetectionResult
        {
            public List<FieldRelationship> Relationships { get; set; } = new List<FieldRelationship>();
            public List<FieldInfo> NewFields { get; set; } = new List<FieldInfo>();
        }

        /// <summary>
        /// Detects compound fields that should be split into Value + Unit.
        /// Example: "1.94 kg" → Value (1.94) + Unit ("kg")
        /// </summary>
        private SplitDetectionResult DetectSplitFields(List<FieldInfo> fields)
        {
            var result = new SplitDetectionResult();

            // Patterns for compound fields (value + unit)
            var compoundPatterns = new[]
            {
                new { Name = "WeightKg", Pattern = new Regex(@"^\s*([+-]?\d+\.?\d*)\s*(kg)\s*$", RegexOptions.IgnoreCase) },
                new { Name = "WeightG", Pattern = new Regex(@"^\s*([+-]?\d+\.?\d*)\s*(g)\s*$", RegexOptions.IgnoreCase) },
                new { Name = "CountPcs", Pattern = new Regex(@"^\s*(\d+)\s*(pcs)\s*$", RegexOptions.IgnoreCase) },
                new { Name = "Temperature", Pattern = new Regex(@"^\s*([+-]?\d+\.?\d*)\s*(°C|°F|C|F)\s*$", RegexOptions.IgnoreCase) },
                new { Name = "pH", Pattern = new Regex(@"^\s*(\d+\.?\d*)\s*(pH)\s*$", RegexOptions.IgnoreCase) }
            };

            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];

                // Check if any sample matches a compound pattern
                foreach (var compoundPattern in compoundPatterns)
                {
                    if (IsCompoundField(field, compoundPattern.Pattern, out var valueSamples, out var unitSamples))
                    {
                        // Keep the full field name including the running number
                        // Example: WeightKg1 → WeightKg1Value and WeightKg1Unit
                        //          WeightKg2 → WeightKg2Value and WeightKg2Unit
                        string baseName = field.Name;

                        // Create Value field
                        var valueField = new FieldInfo
                        {
                            Order = field.Order * 100, // Sub-field ordering
                            Name = $"{baseName}Value",
                            DataType = "decimal",
                            FieldType = "Decimal",
                            SampleValues = valueSamples,
                            Confidence = field.Confidence,
                            MinLength = valueSamples.Min(s => s.Length),
                            MaxLength = valueSamples.Max(s => s.Length),
                            IsConstant = false,
                            Action = "Parse",
                            Required = field.Required,
                            ParsePattern = @"^([+-]?\d+\.?\d*)$",
                            FormatString = "{0:F2}",
                            Alignment = "right"
                        };

                        // Create Unit field
                        var unitField = new FieldInfo
                        {
                            Order = field.Order * 100 + 1, // After value field
                            Name = $"{baseName}Unit",
                            DataType = "string",
                            FieldType = "Reserved",
                            SampleValues = unitSamples.Distinct().ToList(),
                            Confidence = 1.0,
                            MinLength = unitSamples.Min(s => s.Length),
                            MaxLength = unitSamples.Max(s => s.Length),
                            IsConstant = unitSamples.Distinct().Count() == 1,
                            Action = "Validate",
                            Required = false,
                            ParsePattern = $"^({string.Join("|", unitSamples.Distinct().Select(Regex.Escape))})$",
                            FormatString = unitSamples.First(),
                            Alignment = "left"
                        };

                        // Add new fields to result
                        result.NewFields.Add(valueField);
                        result.NewFields.Add(unitField);

                        // Create relationship
                        var relationship = new FieldRelationship
                        {
                            Type = "Split",
                            SourceFields = new List<string> { field.Name },
                            TargetField = null, // Multiple targets
                            Operation = $"Split '{field.Name}' into Value and Unit",
                            Confidence = field.Confidence,
                            Reason = $"Compound field detected with pattern: value + unit"
                        };

                        result.Relationships.Add(relationship);

                        // Mark original field as skipped (it's been split)
                        field.Action = "Skip";
                        field.IncludeInDefinition = false;

                        break; // Found match, move to next field
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if a field is a compound field and extracts value and unit samples.
        /// </summary>
        private bool IsCompoundField(FieldInfo field, Regex pattern, out List<string> valueSamples, out List<string> unitSamples)
        {
            valueSamples = new List<string>();
            unitSamples = new List<string>();

            if (field.SampleValues == null || field.SampleValues.Count == 0)
            {
                return false;
            }

            int matchCount = 0;

            foreach (var sample in field.SampleValues)
            {
                var match = pattern.Match(sample);
                if (match.Success && match.Groups.Count >= 3)
                {
                    valueSamples.Add(match.Groups[1].Value.Trim());
                    unitSamples.Add(match.Groups[2].Value.Trim());
                    matchCount++;
                }
            }

            // Consider it compound if 80%+ samples match the pattern
            double matchRate = (double)matchCount / field.SampleValues.Count;
            return matchRate > 0.8;
        }

        #endregion

        #region Combine Detection (Date + Time)

        /// <summary>
        /// Detects adjacent Date and Time fields that should be combined into DateTime.
        /// </summary>
        private List<FieldRelationship> DetectCombineFields(List<FieldInfo> fields)
        {
            var relationships = new List<FieldRelationship>();

            for (int i = 0; i < fields.Count - 1; i++)
            {
                var field1 = fields[i];
                var field2 = fields[i + 1];

                // Check for Date + Time pattern
                if (IsDateField(field1) && IsTimeField(field2))
                {
                    var relationship = new FieldRelationship
                    {
                        Type = "Combine",
                        SourceFields = new List<string> { field1.Name, field2.Name },
                        TargetField = "DateTime",
                        Operation = $"{field1.Name}.Date + {field2.Name}",
                        Confidence = Math.Min(field1.Confidence, field2.Confidence),
                        Reason = "Adjacent Date and Time fields detected"
                    };

                    relationships.Add(relationship);
                }
            }

            return relationships;
        }

        /// <summary>
        /// Checks if a field is a Date field.
        /// </summary>
        private bool IsDateField(FieldInfo field)
        {
            return field.FieldType == "Date" ||
                   field.DataType == "DateTime" ||
                   (field.Name != null && field.Name.IndexOf("Date", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        /// <summary>
        /// Checks if a field is a Time field.
        /// </summary>
        private bool IsTimeField(FieldInfo field)
        {
            return field.FieldType == "Time" ||
                   field.DataType == "TimeSpan" ||
                   (field.Name != null && field.Name.IndexOf("Time", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        #endregion

        #region Calculate Detection (Formulas)

        /// <summary>
        /// Detects calculated fields based on common formulas.
        /// Example: GrossWeight - TareWeight = NetWeight
        /// </summary>
        private List<FieldRelationship> DetectCalculateFields(List<FieldInfo> fields)
        {
            var relationships = new List<FieldRelationship>();

            // Find weight fields
            var weightFields = fields.Where(f =>
                f.Name != null &&
                (f.Name.IndexOf("Weight", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 f.Name.IndexOf("Kg", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 f.Name.IndexOf("Mass", StringComparison.OrdinalIgnoreCase) >= 0) &&
                f.Action == "Parse"
            ).ToList();

            // Look for Tare, Gross, Net pattern
            var tareField = weightFields.FirstOrDefault(f => f.Name.IndexOf("Tare", StringComparison.OrdinalIgnoreCase) >= 0);
            var grossField = weightFields.FirstOrDefault(f => f.Name.IndexOf("Gross", StringComparison.OrdinalIgnoreCase) >= 0);
            var netField = weightFields.FirstOrDefault(f => f.Name.IndexOf("Net", StringComparison.OrdinalIgnoreCase) >= 0);

            if (tareField != null && grossField != null && netField != null)
            {
                // Verify the formula with samples if possible
                bool formulaValid = VerifyFormula(tareField, grossField, netField);

                if (formulaValid)
                {
                    var relationship = new FieldRelationship
                    {
                        Type = "Calculate",
                        SourceFields = new List<string> { grossField.Name, tareField.Name },
                        TargetField = netField.Name,
                        Operation = $"{grossField.Name} - {tareField.Name}",
                        Confidence = 0.9,
                        Reason = "Formula GrossWeight - TareWeight = NetWeight detected",
                        Tolerance = 0.01
                    };

                    relationships.Add(relationship);
                }
            }

            return relationships;
        }

        /// <summary>
        /// Verifies if GrossWeight - TareWeight = NetWeight for sample data.
        /// </summary>
        private bool VerifyFormula(FieldInfo tareField, FieldInfo grossField, FieldInfo netField)
        {
            // Need at least 3 samples to verify
            int sampleCount = Math.Min(tareField.SampleValues.Count,
                                      Math.Min(grossField.SampleValues.Count, netField.SampleValues.Count));

            if (sampleCount < 3)
            {
                return false;
            }

            int matchCount = 0;

            for (int i = 0; i < sampleCount; i++)
            {
                // Try to parse numeric values
                if (TryExtractNumericValue(tareField.SampleValues[i], out decimal tare) &&
                    TryExtractNumericValue(grossField.SampleValues[i], out decimal gross) &&
                    TryExtractNumericValue(netField.SampleValues[i], out decimal net))
                {
                    // Check if Gross - Tare = Net (with tolerance)
                    decimal calculated = gross - tare;
                    decimal difference = Math.Abs(calculated - net);

                    if (difference < 0.01m) // Tolerance of 0.01
                    {
                        matchCount++;
                    }
                }
            }

            // Consider formula valid if 80%+ samples match
            double matchRate = (double)matchCount / sampleCount;
            return matchRate > 0.8;
        }

        /// <summary>
        /// Extracts numeric value from a string that may contain units.
        /// Example: "1.94 kg" → 1.94
        /// </summary>
        private bool TryExtractNumericValue(string text, out decimal value)
        {
            value = 0;

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            // Extract first number (with optional sign and decimal point)
            var match = Regex.Match(text, @"[+-]?\d+\.?\d*");
            if (match.Success)
            {
                return decimal.TryParse(match.Value, out value);
            }

            return false;
        }

        #endregion
    }
}
