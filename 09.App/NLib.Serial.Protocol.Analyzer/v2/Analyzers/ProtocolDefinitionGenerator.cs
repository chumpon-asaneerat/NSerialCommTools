#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using NLib.Serial.ProtocolAnalyzer.Models;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Analyzers
{
    /// <summary>
    /// Generates complete ProtocolDefinition for JSON export.
    /// This is the main output of the Protocol Analyzer tool.
    /// Generated JSON is used by NTerminal&lt;T&gt; (parsing) and NDevice&lt;T&gt; (serialization).
    /// </summary>
    public class ProtocolDefinitionGenerator
    {
        #region Public Methods

        /// <summary>
        /// Generates a complete protocol definition from analysis results.
        /// </summary>
        /// <param name="analysis">The analysis results containing fields, relationships, and validation rules.</param>
        /// <param name="deviceName">The device name (user-provided or default).</param>
        /// <param name="logData">The original log data for frame marker extraction.</param>
        /// <returns>Complete ProtocolDefinition ready for JSON export.</returns>
        public ProtocolDefinition Generate(AnalysisResult analysis, string deviceName, LogData logData)
        {
            if (analysis == null)
            {
                throw new ArgumentNullException(nameof(analysis));
            }

            var definition = new ProtocolDefinition();

            // 1. Generate Metadata
            GenerateMetadata(definition, deviceName, analysis);

            // 2. Detect and set Protocol Type
            GenerateProtocolInfo(definition, analysis, logData);

            // 3. Extract Frame Markers
            ExtractFrameMarkers(definition, analysis, logData);

            // 4. Export Fields (only those marked for inclusion)
            ExportFields(definition, analysis);

            // 5. Export Relationships
            ExportRelationships(definition, analysis);

            // Validation rules removed - user implements validation in their application layer

            return definition;
        }

        /// <summary>
        /// Serializes a ProtocolDefinition to JSON string.
        /// </summary>
        /// <param name="definition">The protocol definition to serialize.</param>
        /// <returns>JSON string with indented formatting.</returns>
        public string ExportToJson(ProtocolDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Serialize(definition, options);
        }

        /// <summary>
        /// Validates a ProtocolDefinition for correctness.
        /// Returns list of validation errors (empty if valid).
        /// </summary>
        /// <param name="definition">The protocol definition to validate.</param>
        /// <returns>List of validation error messages.</returns>
        public List<string> ValidateDefinition(ProtocolDefinition definition)
        {
            var errors = new List<string>();

            if (definition == null)
            {
                errors.Add("Definition is null");
                return errors;
            }

            // Check required metadata
            if (string.IsNullOrWhiteSpace(definition.DeviceName))
            {
                errors.Add("DeviceName is required");
            }

            if (string.IsNullOrWhiteSpace(definition.Version))
            {
                errors.Add("Version is required");
            }

            // Check fields
            if (definition.Fields == null || definition.Fields.Count == 0)
            {
                errors.Add("No fields defined in protocol");
            }
            else
            {
                // Validate field names are valid C# identifiers
                foreach (var field in definition.Fields)
                {
                    if (!IsValidCSharpIdentifier(field.Name))
                    {
                        errors.Add($"Invalid C# identifier: {field.Name}");
                    }
                }

                // Check for duplicate field names (only for actual data fields)
                // Structural markers (StartMarker/EndMarker) have already been deduplicated during export
                var dataFieldDuplicates = definition.Fields
                    .Where(f => f.Name != "StartMarker" && f.Name != "EndMarker") // Markers already filtered
                    .GroupBy(f => f.Name)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);

                foreach (var dup in dataFieldDuplicates)
                {
                    errors.Add($"Duplicate field name: '{dup}' - Please rename in Field Editor");
                }

                // Validate regex patterns compile
                foreach (var field in definition.Fields.Where(f => !string.IsNullOrEmpty(f.ParsePattern)))
                {
                    try
                    {
                        var regex = new Regex(field.ParsePattern);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Invalid regex pattern in field {field.Name}: {ex.Message}");
                    }
                }
            }

            return errors;
        }

        #endregion

        #region Metadata Generation

        /// <summary>
        /// Generates metadata section of protocol definition.
        /// </summary>
        private void GenerateMetadata(ProtocolDefinition definition, string deviceName, AnalysisResult analysis)
        {
            definition.DeviceName = string.IsNullOrWhiteSpace(deviceName) ? "UnknownDevice" : deviceName;
            definition.Version = "1.0";
            definition.GeneratedDate = DateTime.Now;

            // Use detected encoding from analysis (defaults to ASCII if not detected)
            definition.Encoding = analysis.EncodingName ?? "ASCII";

            definition.Description = $"Auto-generated protocol definition for {definition.DeviceName}";
        }

        #endregion

        #region Protocol Information

        /// <summary>
        /// Generates protocol type and message structure information.
        /// </summary>
        private void GenerateProtocolInfo(ProtocolDefinition definition, AnalysisResult analysis, LogData logData)
        {
            // Determine message type based on protocol characteristics
            if (logData != null && logData.Messages != null && logData.Messages.Count > 0)
            {
                // Check if multi-line messages (frames)
                var firstMessage = logData.Messages[0];
                var lineCount = CountLines(firstMessage);

                if (lineCount > 1)
                {
                    // Check for frame markers
                    bool hasStartMarker = analysis.Fields.Any(f => f.FieldType == "StartMarker");
                    bool hasEndMarker = analysis.Fields.Any(f => f.FieldType == "EndMarker");

                    if (hasStartMarker && hasEndMarker)
                    {
                        definition.MessageType = "multi-line-frame";
                    }
                    else
                    {
                        definition.MessageType = "multi-line-block";
                    }
                }
                else
                {
                    definition.MessageType = "single-line";
                }
            }
            else
            {
                definition.MessageType = "unknown";
            }

            // Set entry terminator from analysis
            if (analysis.Terminator != null)
            {
                definition.EntryTerminator = ConvertToEscapeSequence(analysis.Terminator.Bytes);
            }
        }

        /// <summary>
        /// Counts the number of lines in a message (based on line terminators).
        /// </summary>
        private int CountLines(byte[] message)
        {
            if (message == null || message.Length == 0)
                return 0;

            int count = 1; // Start with 1 line
            for (int i = 0; i < message.Length - 1; i++)
            {
                if (message[i] == 0x0D && message[i + 1] == 0x0A) // \r\n
                {
                    count++;
                    i++; // Skip next byte
                }
                else if (message[i] == 0x0A) // \n
                {
                    count++;
                }
            }

            return count;
        }

        #endregion

        #region Frame Marker Extraction

        /// <summary>
        /// Extracts frame start and end markers from detected fields.
        /// </summary>
        private void ExtractFrameMarkers(ProtocolDefinition definition, AnalysisResult analysis, LogData logData)
        {
            if (definition.MessageType != "multi-line-frame")
            {
                return; // Frame markers only relevant for frame-based protocols
            }

            // Find StartMarker field
            var startMarkerField = analysis.Fields.FirstOrDefault(f => f.FieldType == "StartMarker");
            if (startMarkerField != null && startMarkerField.SampleValues != null && startMarkerField.SampleValues.Count > 0)
            {
                definition.FrameStart = GenerateMarkerPattern(startMarkerField.SampleValues);
            }

            // Find EndMarker field
            var endMarkerField = analysis.Fields.FirstOrDefault(f => f.FieldType == "EndMarker");
            if (endMarkerField != null && endMarkerField.SampleValues != null && endMarkerField.SampleValues.Count > 0)
            {
                definition.FrameEnd = GenerateMarkerPattern(endMarkerField.SampleValues);
            }

            // If markers include terminator, append it
            if (analysis.Terminator != null)
            {
                string terminator = ConvertToEscapeSequence(analysis.Terminator.Bytes);

                if (!string.IsNullOrEmpty(definition.FrameStart) && !definition.FrameStart.EndsWith(terminator))
                {
                    definition.FrameStart += terminator;
                }

                if (!string.IsNullOrEmpty(definition.FrameEnd) && !definition.FrameEnd.EndsWith(terminator))
                {
                    definition.FrameEnd += terminator;
                }
            }
        }

        /// <summary>
        /// Generates a regex pattern for frame markers.
        /// Example: "^KJIK000", "^KJIK001", "^KJIK002" → "^KJIK\\d{3}"
        /// </summary>
        private string GenerateMarkerPattern(List<string> samples)
        {
            if (samples == null || samples.Count == 0)
                return string.Empty;

            // If all samples are identical, use literal pattern
            if (samples.Distinct().Count() == 1)
            {
                return Regex.Escape(samples[0]);
            }

            // Find common prefix
            string commonPrefix = samples[0];
            foreach (var sample in samples.Skip(1))
            {
                int minLength = Math.Min(commonPrefix.Length, sample.Length);
                int i = 0;
                while (i < minLength && commonPrefix[i] == sample[i])
                {
                    i++;
                }
                commonPrefix = commonPrefix.Substring(0, i);
            }

            // Analyze the varying part
            string pattern = Regex.Escape(commonPrefix);

            // Check if remaining part is numeric
            var remainingParts = samples.Select(s => s.Substring(commonPrefix.Length)).ToList();
            if (remainingParts.All(r => r.All(char.IsDigit)))
            {
                // All numeric, use \d{n}
                int length = remainingParts[0].Length;
                pattern += $"\\d{{{length}}}";
            }
            else
            {
                // Mixed, use more generic pattern
                pattern += ".*";
            }

            return pattern;
        }

        /// <summary>
        /// Converts byte array to escape sequence string.
        /// Example: [0x0D, 0x0A] → "\\r\\n"
        /// </summary>
        private string ConvertToEscapeSequence(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;

            var result = string.Empty;
            foreach (var b in bytes)
            {
                switch (b)
                {
                    case 0x0D: result += "\\r"; break;
                    case 0x0A: result += "\\n"; break;
                    case 0x09: result += "\\t"; break;
                    case 0x00: result += "\\0"; break;
                    default:
                        if (b >= 32 && b <= 126)
                            result += (char)b;
                        else
                            result += $"\\x{b:X2}";
                        break;
                }
            }

            return result;
        }

        #endregion

        #region Field Export

        /// <summary>
        /// Exports fields to protocol definition.
        /// Intelligently filters fields - user should only need to rename actual data fields.
        /// Automatically excludes: duplicate markers, empty lines, constant unit fields.
        /// </summary>
        private void ExportFields(ProtocolDefinition definition, AnalysisResult analysis)
        {
            if (analysis.Fields == null || analysis.Fields.Count == 0)
            {
                return;
            }

            // Intelligent filtering: exclude structural/duplicate fields automatically
            var fieldsToExport = analysis.Fields
                .Where(f => {
                    // INCLUDE: Empty lines for State Machine protocols FIRST
                    // Terminal needs Empty fields to maintain position counting
                    // Check this BEFORE IncludeInDefinition to ensure they're always exported
                    if (f.FieldType == "Empty")
                        return true;

                    // Must be marked for inclusion by user
                    if (!f.IncludeInDefinition)
                        return false;

                    // INCLUDE: Unit fields with Action="Validate" (needed for parsing!)
                    // Terminal validates unit exists: "1.94 kg" → validates "kg"
                    if (f.Action == "Validate" && f.Name != null && f.Name.EndsWith("Unit"))
                        return true;

                    // INCLUDE: Markers (StartMarker, EndMarker, Marker) with Action="Validate"
                    // Terminal needs these for frame boundaries and position counting
                    if (f.FieldType == "StartMarker" || f.FieldType == "EndMarker" || f.FieldType == "Marker")
                        return true;

                    // INCLUDE: Data fields with Action="Parse"
                    if (f.Action == "Parse")
                        return true;

                    // EXCLUDE: Everything else (like Action="Skip" compound fields)
                    return false;
                })
                .OrderBy(f => f.Order)
                .ToList();

            definition.Fields = fieldsToExport;
        }

        #endregion

        #region Relationship Export

        /// <summary>
        /// Exports field relationships to protocol definition.
        /// </summary>
        private void ExportRelationships(ProtocolDefinition definition, AnalysisResult analysis)
        {
            if (analysis.Relationships == null)
            {
                return;
            }

            // Export all relationships - they are documentation/metadata only
            // Relationships show how fields were detected/analyzed but don't affect parsing/serialization
            // Terminal/Device work with individual field definitions, not relationships
            definition.Relationships = analysis.Relationships;
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Checks if a string is a valid C# identifier.
        /// </summary>
        private bool IsValidCSharpIdentifier(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Check first character (must be letter or underscore)
            if (!char.IsLetter(name[0]) && name[0] != '_')
                return false;

            // Check remaining characters (letter, digit, or underscore)
            for (int i = 1; i < name.Length; i++)
            {
                if (!char.IsLetterOrDigit(name[i]) && name[i] != '_')
                    return false;
            }

            // Check against C# keywords
            var keywords = new HashSet<string>
            {
                "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char",
                "checked", "class", "const", "continue", "decimal", "default", "delegate",
                "do", "double", "else", "enum", "event", "explicit", "extern", "false",
                "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit",
                "in", "int", "interface", "internal", "is", "lock", "long", "namespace",
                "new", "null", "object", "operator", "out", "override", "params", "private",
                "protected", "public", "readonly", "ref", "return", "sbyte", "sealed",
                "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
                "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked",
                "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
            };

            return !keywords.Contains(name.ToLower());
        }

        #endregion
    }
}
