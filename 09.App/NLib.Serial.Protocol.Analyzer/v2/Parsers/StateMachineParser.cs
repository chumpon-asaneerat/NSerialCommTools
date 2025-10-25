#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NLib.Serial.ProtocolAnalyzer.Models;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Parsers
{
    /// <summary>
    /// Implements State Machine parsing strategy for position-dependent protocols.
    /// CRITICAL: Handles cases where identical patterns appear at different positions with different meanings.
    /// Example: JIK6CAB has "  1.94 kg" at line 4 (Tare), line 5 (Gross), line 8 (Net).
    /// </summary>
    public class StateMachineParser
    {
        #region Public Methods

        /// <summary>
        /// Analyzes log data using state machine approach.
        /// Each line position has a specific meaning regardless of content pattern.
        /// </summary>
        /// <param name="logData">The log data to analyze.</param>
        /// <returns>List of detected fields with position-specific meanings.</returns>
        public List<FieldInfo> Analyze(LogData logData)
        {
            if (logData == null || logData.Messages == null || logData.Messages.Count == 0)
            {
                return new List<FieldInfo>();
            }

            // Extract frames (multi-line messages)
            var frames = ExtractFrames(logData.Messages);

            if (frames.Count == 0)
            {
                return new List<FieldInfo>();
            }

            // Group lines by position across all frames
            var linesByPosition = GroupLinesByPosition(frames);

            // Analyze each line position
            var fields = new List<FieldInfo>();
            foreach (var kvp in linesByPosition.OrderBy(x => x.Key))
            {
                int lineNumber = kvp.Key;
                List<string> samples = kvp.Value;

                var field = AnalyzeLineAtPosition(lineNumber, samples);
                if (field != null)
                {
                    fields.Add(field);
                }
            }

            return fields;
        }

        #endregion

        #region Frame Extraction

        /// <summary>
        /// Extracts multi-line frames from messages.
        /// A frame is a complete multi-line message from start marker to end marker.
        /// </summary>
        private List<List<string>> ExtractFrames(List<byte[]> messages)
        {
            var frames = new List<List<string>>();
            var currentFrame = new List<string>();

            foreach (var messageBytes in messages)
            {
                if (messageBytes == null || messageBytes.Length == 0) continue;

                string line = Encoding.ASCII.GetString(messageBytes);

                // Check for frame start markers
                if (IsFrameStart(line))
                {
                    // Start new frame
                    if (currentFrame.Count > 0)
                    {
                        frames.Add(currentFrame);
                    }
                    currentFrame = new List<string> { line };
                }
                else if (IsFrameEnd(line))
                {
                    // End current frame
                    currentFrame.Add(line);
                    if (currentFrame.Count > 0)
                    {
                        frames.Add(currentFrame);
                    }
                    currentFrame = new List<string>();
                }
                else
                {
                    // Add to current frame
                    currentFrame.Add(line);
                }
            }

            // Add last frame if not empty
            if (currentFrame.Count > 0)
            {
                frames.Add(currentFrame);
            }

            return frames;
        }

        /// <summary>
        /// Checks if a line is a frame start marker.
        /// Examples: "^KJIK123", "V1", "~START"
        /// </summary>
        private bool IsFrameStart(string line)
        {
            if (string.IsNullOrEmpty(line)) return false;

            // Common start markers
            char firstChar = line[0];
            return firstChar == '^' || firstChar == '~' || firstChar == '<' ||
                   firstChar == '@' || firstChar == '#' || line.StartsWith("V");
        }

        /// <summary>
        /// Checks if a line is a frame end marker.
        /// Examples: "~P1", "END", "$"
        /// </summary>
        private bool IsFrameEnd(string line)
        {
            if (string.IsNullOrEmpty(line)) return false;

            string trimmed = line.Trim();

            // Common end markers
            return trimmed.StartsWith("~") ||
                   trimmed.Equals("END", StringComparison.OrdinalIgnoreCase) ||
                   trimmed == "$" ||
                   trimmed.Length <= 3; // Short markers
        }

        #endregion

        #region Line Analysis

        /// <summary>
        /// Groups lines by their position across all frames.
        /// Example: All line 0s together, all line 1s together, etc.
        /// </summary>
        private Dictionary<int, List<string>> GroupLinesByPosition(List<List<string>> frames)
        {
            var linesByPosition = new Dictionary<int, List<string>>();

            foreach (var frame in frames)
            {
                for (int i = 0; i < frame.Count; i++)
                {
                    if (!linesByPosition.ContainsKey(i))
                    {
                        linesByPosition[i] = new List<string>();
                    }
                    linesByPosition[i].Add(frame[i]);
                }
            }

            return linesByPosition;
        }

        /// <summary>
        /// Analyzes samples at a specific line position to determine field characteristics.
        /// CRITICAL: Position determines meaning, not just pattern.
        /// </summary>
        private FieldInfo AnalyzeLineAtPosition(int lineNumber, List<string> samples)
        {
            if (samples == null || samples.Count == 0)
            {
                return null;
            }

            var field = new FieldInfo
            {
                Order = lineNumber,
                SampleValues = samples.Distinct().Take(5).ToList(),
                MinLength = samples.Min(s => s.Length),
                MaxLength = samples.Max(s => s.Length),
                IsConstant = samples.Distinct().Count() == 1
            };

            // Detect field type and generate appropriate name
            DetectFieldType(field, samples);

            // Generate parse pattern (regex)
            GenerateParsePattern(field, samples);

            // Generate format string (for serialization)
            GenerateFormatString(field, samples);

            return field;
        }

        /// <summary>
        /// Detects the field type based on pattern analysis.
        /// Sets Name, DataType, FieldType, and Confidence.
        /// </summary>
        private void DetectFieldType(FieldInfo field, List<string> samples)
        {
            // Check for markers
            if (field.Order == 0 || (field.IsConstant && samples[0].Length < 10))
            {
                field.Name = field.Order == 0 ? "StartMarker" : "Marker";
                field.DataType = "string";
                field.FieldType = field.Order == 0 ? "StartMarker" : "EndMarker";
                field.Confidence = 1.0;
                field.Action = "Validate";
                return;
            }

            // Check for empty lines
            if (samples.All(s => string.IsNullOrWhiteSpace(s)))
            {
                field.Name = $"Empty{field.Order}";
                field.DataType = "string";
                field.FieldType = "Empty";
                field.Confidence = 1.0;
                field.Action = "Skip";
                return;
            }

            // Pattern detection
            var patterns = new[]
            {
                new { Name = "Date", Regex = new Regex(@"^\d{4}-\d{2}-\d{2}$"), Type = "DateTime" },
                new { Name = "Time", Regex = new Regex(@"^\d{2}:\d{2}:\d{2}$"), Type = "TimeSpan" },
                new { Name = "WeightKg", Regex = new Regex(@"^\s*[+-]?\d+\.\d+\s*kg\s*$", RegexOptions.IgnoreCase), Type = "decimal" },
                new { Name = "WeightG", Regex = new Regex(@"^\s*[+-]?\d+\.\d+\s*g\s*$", RegexOptions.IgnoreCase), Type = "decimal" },
                new { Name = "CountPcs", Regex = new Regex(@"^\s*\d+\s*pcs\s*$", RegexOptions.IgnoreCase), Type = "int" },
                new { Name = "Decimal", Regex = new Regex(@"^\s*[+-]?\d+\.\d+\s*$"), Type = "decimal" },
                new { Name = "Integer", Regex = new Regex(@"^\s*[+-]?\d+\s*$"), Type = "int" }
            };

            foreach (var pattern in patterns)
            {
                int matchCount = samples.Count(s => pattern.Regex.IsMatch(s));
                double matchRate = (double)matchCount / samples.Count;

                if (matchRate > 0.8)
                {
                    field.Name = $"{pattern.Name}{field.Order}";
                    field.DataType = pattern.Type;
                    field.FieldType = pattern.Name;
                    field.Confidence = matchRate;
                    field.Action = "Parse";
                    return;
                }
            }

            // Calculate variance for unknown fields
            double variance = CalculateVariance(samples);

            if (variance < 0.1)
            {
                field.Name = $"Reserved{field.Order}";
                field.DataType = "string";
                field.FieldType = "Reserved";
                field.Confidence = 0.7;
                field.Action = "Skip";
            }
            else
            {
                field.Name = $"Field{field.Order}";
                field.DataType = "string";
                field.FieldType = "String";
                field.Confidence = 0.5;
                field.Action = "Parse";
            }
        }

        /// <summary>
        /// Generates a regex pattern to extract the value from this field.
        /// Used by NTerminal for parsing.
        /// </summary>
        private void GenerateParsePattern(FieldInfo field, List<string> samples)
        {
            if (field.FieldType == "StartMarker" || field.FieldType == "EndMarker")
            {
                // Escape special regex characters
                string sample = Regex.Escape(samples[0]);
                field.ParsePattern = $"^{sample}$";
                return;
            }

            if (field.FieldType == "Empty")
            {
                field.ParsePattern = @"^\s*$";
                return;
            }

            // Generate pattern based on field type
            switch (field.FieldType)
            {
                case "Date":
                    field.ParsePattern = @"^(\d{4}-\d{2}-\d{2})$";
                    break;

                case "Time":
                    field.ParsePattern = @"^(\d{2}:\d{2}:\d{2})$";
                    break;

                case "WeightKg":
                    field.ParsePattern = @"^\s*([+-]?\d+\.\d+)\s*kg\s*$";
                    break;

                case "WeightG":
                    field.ParsePattern = @"^\s*([+-]?\d+\.\d+)\s*g\s*$";
                    break;

                case "CountPcs":
                    field.ParsePattern = @"^\s*(\d+)\s*pcs\s*$";
                    break;

                case "Decimal":
                    field.ParsePattern = @"^\s*([+-]?\d+\.\d+)\s*$";
                    break;

                case "Integer":
                    field.ParsePattern = @"^\s*([+-]?\d+)\s*$";
                    break;

                default:
                    // Generic string pattern - capture entire line trimmed
                    field.ParsePattern = @"^\s*(.+?)\s*$";
                    break;
            }
        }

        /// <summary>
        /// Generates a C# format string for serializing this field.
        /// Used by NDevice for generating output.
        /// </summary>
        private void GenerateFormatString(FieldInfo field, List<string> samples)
        {
            if (field.FieldType == "StartMarker" || field.FieldType == "EndMarker")
            {
                field.FormatString = samples[0]; // Constant value
                field.Alignment = "left";
                field.Width = samples[0].Length;
                return;
            }

            if (field.FieldType == "Empty")
            {
                field.FormatString = "";
                field.Alignment = "left";
                field.Width = 0;
                return;
            }

            // Analyze sample formatting
            var firstSample = samples.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));
            if (string.IsNullOrEmpty(firstSample))
            {
                field.FormatString = "{0}";
                return;
            }

            // Detect alignment and width
            int leadingSpaces = firstSample.Length - firstSample.TrimStart().Length;
            int trailingSpaces = firstSample.Length - firstSample.TrimEnd().Length;

            field.Alignment = leadingSpaces > trailingSpaces ? "right" : "left";
            field.Width = field.MaxLength;

            // Generate format based on type
            switch (field.FieldType)
            {
                case "Date":
                    field.FormatString = "{0:yyyy-MM-dd}";
                    break;

                case "Time":
                    field.FormatString = "{0:HH:mm:ss}";
                    break;

                case "WeightKg":
                    field.FormatString = field.Alignment == "right"
                        ? $"{{0,{field.Width}:F2}} kg"
                        : "{0:F2} kg";
                    break;

                case "WeightG":
                    field.FormatString = field.Alignment == "right"
                        ? $"{{0,{field.Width}:F2}} g"
                        : "{0:F2} g";
                    break;

                case "CountPcs":
                    field.FormatString = field.Alignment == "right"
                        ? $"{{0,{field.Width}}} pcs"
                        : "{0} pcs";
                    break;

                case "Decimal":
                    int decimalPlaces = DetectDecimalPlaces(samples);
                    field.FormatString = field.Alignment == "right"
                        ? $"{{0,{field.Width}:F{decimalPlaces}}}"
                        : $"{{0:F{decimalPlaces}}}";
                    break;

                case "Integer":
                    field.FormatString = field.Alignment == "right"
                        ? $"{{0,{field.Width}}}"
                        : "{0}";
                    break;

                default:
                    field.FormatString = field.Alignment == "right"
                        ? $"{{0,{field.Width}}}"
                        : "{0}";
                    break;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Calculates variance ratio (0.0 to 1.0) indicating how much values differ.
        /// 0.0 = all same, 1.0 = all unique
        /// </summary>
        private double CalculateVariance(List<string> samples)
        {
            if (samples == null || samples.Count == 0)
                return 0.0;

            int uniqueCount = samples.Distinct().Count();
            return (double)uniqueCount / samples.Count;
        }

        /// <summary>
        /// Detects the number of decimal places from sample values.
        /// </summary>
        private int DetectDecimalPlaces(List<string> samples)
        {
            int maxDecimalPlaces = 2; // Default

            foreach (var sample in samples)
            {
                var match = Regex.Match(sample, @"\.(\d+)");
                if (match.Success)
                {
                    int places = match.Groups[1].Length;
                    if (places > maxDecimalPlaces)
                        maxDecimalPlaces = places;
                }
            }

            return maxDecimalPlaces;
        }

        #endregion
    }
}
