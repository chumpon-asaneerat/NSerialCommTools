#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using NLib.Serial.Protocol.Analyzer.Models;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Analyzers
{
    /// <summary>
    /// Detects mathematical relationships between fields (formulas).
    /// Works purely by observation - no assumptions about what fields should relate to what.
    /// </summary>
    public class RelationshipDetector
    {
        #region Constants

        private const double TOLERANCE = 0.01; // 1% tolerance for floating point comparisons

        #endregion

        #region Public Methods

        /// <summary>
        /// Detects potential formulas between numeric fields
        /// </summary>
        public List<FormulaInfo> DetectFormulas(List<FieldInfo> fields, List<byte[]> messages, char delimiter)
        {
            List<FormulaInfo> formulas = new List<FormulaInfo>();

            // Get numeric fields only
            var numericFields = fields
                .Where(f => f.Type == "integer" || f.Type == "decimal")
                .ToList();

            if (numericFields.Count < 2)
                return formulas; // Need at least 2 numeric fields for relationships

            // Parse all messages into field value arrays
            List<decimal[]> allValues = ParseNumericValues(messages, delimiter, fields.Count);

            if (allValues.Count == 0)
                return formulas;

            // Check each numeric field to see if it's derived from other fields
            foreach (var targetField in numericFields)
            {
                var formula = DetectFormulaForField(targetField, numericFields, allValues);
                if (formula != null)
                {
                    formulas.Add(formula);
                }
            }

            return formulas;
        }

        #endregion

        #region Private Methods - Parsing

        /// <summary>
        /// Parses all messages into arrays of numeric values
        /// </summary>
        private List<decimal[]> ParseNumericValues(List<byte[]> messages, char delimiter, int fieldCount)
        {
            List<decimal[]> allValues = new List<decimal[]>();

            foreach (byte[] message in messages)
            {
                string text = System.Text.Encoding.ASCII.GetString(message).Trim();
                string[] fields = text.Split(delimiter);

                decimal[] values = new decimal[fieldCount];
                bool hasValues = false;

                for (int i = 0; i < Math.Min(fields.Length, fieldCount); i++)
                {
                    string field = fields[i].Trim();

                    // Try to parse as decimal (extract numeric part if needed)
                    if (TryParseNumeric(field, out decimal value))
                    {
                        values[i] = value;
                        hasValues = true;
                    }
                    else
                    {
                        values[i] = 0; // Non-numeric field
                    }
                }

                if (hasValues)
                {
                    allValues.Add(values);
                }
            }

            return allValues;
        }

        /// <summary>
        /// Tries to parse a field value as numeric (handles attached units)
        /// </summary>
        private bool TryParseNumeric(string field, out decimal value)
        {
            value = 0;

            if (string.IsNullOrEmpty(field))
                return false;

            // Try direct parse first
            if (decimal.TryParse(field, out value))
                return true;

            // Try extracting numeric part (for attached units)
            System.Text.StringBuilder numeric = new System.Text.StringBuilder();
            foreach (char c in field)
            {
                if (char.IsDigit(c) || c == '.' || c == '-' || c == '+')
                    numeric.Append(c);
                else
                    break;
            }

            return decimal.TryParse(numeric.ToString(), out value);
        }

        #endregion

        #region Private Methods - Formula Detection

        /// <summary>
        /// Detects if a field can be derived from other fields using a formula
        /// </summary>
        private FormulaInfo DetectFormulaForField(FieldInfo targetField, List<FieldInfo> numericFields, List<decimal[]> allValues)
        {
            int targetPos = targetField.Position;

            // Try to detect various formula patterns
            // Pattern 1: Target = Field_A + Field_B
            var additionFormula = TryDetectAddition(targetPos, numericFields, allValues);
            if (additionFormula != null) return additionFormula;

            // Pattern 2: Target = Field_A - Field_B
            var subtractionFormula = TryDetectSubtraction(targetPos, numericFields, allValues);
            if (subtractionFormula != null) return subtractionFormula;

            // Pattern 3: Target = Field_A * Field_B
            var multiplicationFormula = TryDetectMultiplication(targetPos, numericFields, allValues);
            if (multiplicationFormula != null) return multiplicationFormula;

            // Pattern 4: Target = Field_A / Field_B
            var divisionFormula = TryDetectDivision(targetPos, numericFields, allValues);
            if (divisionFormula != null) return divisionFormula;

            return null;
        }

        /// <summary>
        /// Tries to detect: Target = A + B
        /// </summary>
        private FormulaInfo TryDetectAddition(int targetPos, List<FieldInfo> numericFields, List<decimal[]> allValues)
        {
            // Try all pairs of fields
            foreach (var fieldA in numericFields)
            {
                if (fieldA.Position == targetPos) continue;

                foreach (var fieldB in numericFields)
                {
                    if (fieldB.Position == targetPos) continue;
                    if (fieldB.Position <= fieldA.Position) continue; // Avoid duplicates

                    if (TestFormula(allValues, targetPos, fieldA.Position, fieldB.Position,
                        (a, b) => a + b))
                    {
                        return new FormulaInfo
                        {
                            TargetField = targetPos,
                            Formula = $"Field{targetPos} = Field{fieldA.Position} + Field{fieldB.Position}",
                            SourceFields = new List<int> { fieldA.Position, fieldB.Position },
                            Operation = "addition"
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to detect: Target = A - B
        /// </summary>
        private FormulaInfo TryDetectSubtraction(int targetPos, List<FieldInfo> numericFields, List<decimal[]> allValues)
        {
            foreach (var fieldA in numericFields)
            {
                if (fieldA.Position == targetPos) continue;

                foreach (var fieldB in numericFields)
                {
                    if (fieldB.Position == targetPos) continue;
                    if (fieldB.Position == fieldA.Position) continue;

                    if (TestFormula(allValues, targetPos, fieldA.Position, fieldB.Position,
                        (a, b) => a - b))
                    {
                        return new FormulaInfo
                        {
                            TargetField = targetPos,
                            Formula = $"Field{targetPos} = Field{fieldA.Position} - Field{fieldB.Position}",
                            SourceFields = new List<int> { fieldA.Position, fieldB.Position },
                            Operation = "subtraction"
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to detect: Target = A * B
        /// </summary>
        private FormulaInfo TryDetectMultiplication(int targetPos, List<FieldInfo> numericFields, List<decimal[]> allValues)
        {
            foreach (var fieldA in numericFields)
            {
                if (fieldA.Position == targetPos) continue;

                foreach (var fieldB in numericFields)
                {
                    if (fieldB.Position == targetPos) continue;
                    if (fieldB.Position <= fieldA.Position) continue;

                    if (TestFormula(allValues, targetPos, fieldA.Position, fieldB.Position,
                        (a, b) => a * b))
                    {
                        return new FormulaInfo
                        {
                            TargetField = targetPos,
                            Formula = $"Field{targetPos} = Field{fieldA.Position} * Field{fieldB.Position}",
                            SourceFields = new List<int> { fieldA.Position, fieldB.Position },
                            Operation = "multiplication"
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to detect: Target = A / B
        /// </summary>
        private FormulaInfo TryDetectDivision(int targetPos, List<FieldInfo> numericFields, List<decimal[]> allValues)
        {
            foreach (var fieldA in numericFields)
            {
                if (fieldA.Position == targetPos) continue;

                foreach (var fieldB in numericFields)
                {
                    if (fieldB.Position == targetPos) continue;
                    if (fieldB.Position == fieldA.Position) continue;

                    if (TestFormula(allValues, targetPos, fieldA.Position, fieldB.Position,
                        (a, b) => b != 0 ? a / b : 0))
                    {
                        return new FormulaInfo
                        {
                            TargetField = targetPos,
                            Formula = $"Field{targetPos} = Field{fieldA.Position} / Field{fieldB.Position}",
                            SourceFields = new List<int> { fieldA.Position, fieldB.Position },
                            Operation = "division"
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Tests if a formula holds true across all samples
        /// </summary>
        private bool TestFormula(List<decimal[]> allValues, int targetPos, int posA, int posB,
            Func<decimal, decimal, decimal> operation)
        {
            int matchCount = 0;
            int totalCount = 0;

            foreach (var values in allValues)
            {
                // Skip if any position is out of range
                if (targetPos >= values.Length || posA >= values.Length || posB >= values.Length)
                    continue;

                decimal target = values[targetPos];
                decimal a = values[posA];
                decimal b = values[posB];

                // Skip zero values (likely non-numeric fields)
                if (target == 0 && a == 0 && b == 0)
                    continue;

                decimal expected = operation(a, b);
                totalCount++;

                // Check if formula matches within tolerance
                decimal tolerance = (decimal)TOLERANCE;
                if (Math.Abs(target - expected) <= Math.Max(Math.Abs(target), Math.Abs(expected)) * tolerance)
                {
                    matchCount++;
                }
            }

            // Require at least 90% match rate and minimum 3 samples
            return totalCount >= 3 && (matchCount / (double)totalCount) >= 0.90;
        }

        #endregion
    }

    /// <summary>
    /// Information about a detected formula
    /// </summary>
    public class FormulaInfo
    {
        public int TargetField { get; set; }
        public string Formula { get; set; }
        public List<int> SourceFields { get; set; }
        public string Operation { get; set; }

        public FormulaInfo()
        {
            SourceFields = new List<int>();
        }
    }
}
