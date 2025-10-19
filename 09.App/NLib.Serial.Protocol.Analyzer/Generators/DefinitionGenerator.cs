#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLib.Serial.Protocol.Analyzer.Analyzers;
using NLib.Serial.Protocol.Analyzer.Models;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Generators
{
    /// <summary>
    /// Generates protocol definitions from analysis results
    /// </summary>
    public class DefinitionGenerator
    {
        #region Private Fields

        private DataDrivenLineAnalyzer _lineAnalyzer = new DataDrivenLineAnalyzer();
        private RelationshipDetector _relationshipDetector = new RelationshipDetector();

        #endregion

        #region Public Methods

        /// <summary>
        /// Generates a protocol definition from analysis results
        /// </summary>
        public ProtocolDefinition Generate(AnalysisResult analysis, string deviceName, string sourceFile, LogData logData, PackageInfo packageInfo)
        {
            if (analysis == null)
                throw new ArgumentNullException(nameof(analysis));

            var definition = new ProtocolDefinition
            {
                Schema = "http://json-schema.org/draft-07/schema#",
                Version = "1.0",
                LastUpdated = DateTime.Now.ToString("yyyy-MM-dd"),

                DeviceInfo = new DeviceInfo
                {
                    Name = deviceName,
                    Manufacturer = "Unknown",
                    Category = "scale",
                    Model = deviceName,
                    Description = $"Auto-generated from {Path.GetFileName(sourceFile)}",
                    Complexity = analysis.ProtocolType == "multi-line-package" ? "very-complex" : "simple"
                },

                Communication = new CommunicationInfo
                {
                    BaudRate = 9600,
                    DataBits = 8,
                    Parity = "None",
                    StopBits = "One",
                    Handshake = "None",
                    Encoding = "ASCII",
                    ReadTimeout = 5000,
                    WriteTimeout = 5000
                },

                Protocol = new ProtocolInfo
                {
                    Type = analysis.ProtocolType,
                    Format = analysis.ProtocolType == "multi-line-package" ? "multi-line-package" : "csv",
                    Encoding = "ASCII",
                    Terminator = GetTerminatorString(analysis.Terminator),
                    UpdateRate = "on-demand"
                },

                Parsing = new ParsingInfo
                {
                    Strategy = analysis.RecommendedStrategy,
                    Delimiter = analysis.Delimiters.Count > 0
                        ? analysis.Delimiters.First().Delimiter.ToString()
                        : ","
                }
            };

            // Generate package-based or simple protocol structure
            if (analysis.ProtocolType == "multi-line-package" && packageInfo != null)
            {
                GeneratePackageStructure(definition, analysis, logData, packageInfo);
            }
            else
            {
                // Simple CSV protocol
                definition.Protocol.Fields = GenerateFields(analysis.Fields);
            }

            return definition;
        }

        /// <summary>
        /// Converts protocol definition to JSON string
        /// </summary>
        public string ToJson(ProtocolDefinition definition)
        {
            return definition.ToJson(minimized: false);
        }

        /// <summary>
        /// Saves protocol definition to file
        /// </summary>
        public bool SaveToFile(ProtocolDefinition definition, string fileName)
        {
            return definition.SaveToFile(fileName, minimized: false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates package structure with detailed line analysis (data-driven)
        /// </summary>
        private void GeneratePackageStructure(ProtocolDefinition definition, AnalysisResult analysis,
            LogData logData, PackageInfo packageInfo)
        {
            definition.Protocol.PackageSize = analysis.PackageSize;

            // Generate markers
            definition.Protocol.Markers = new MarkerInfo
            {
                Start = new MarkerDetail
                {
                    Pattern = analysis.StartMarker,
                    Description = "Package start marker",
                    Action = "reset-state"
                },
                End = new MarkerDetail
                {
                    Pattern = analysis.EndMarker,
                    Description = "Package end marker",
                    Action = "complete-package"
                }
            };

            // Analyze each line in the package using data-driven approach
            var lineStructures = AnalyzePackageLinesDataDriven(logData, packageInfo, analysis);

            // Convert to LineDefinition (only essential properties)
            definition.Protocol.Structure = new StructureInfo
            {
                Lines = lineStructures.Select(ls => new LineDefinition
                {
                    LineNumber = ls.LineNumber,
                    Name = ls.Name,
                    Type = ls.Type,
                    Pattern = ls.Pattern,
                    Format = ls.Format,
                    Unit = ls.Unit,
                    // Only include Min/Max if they're different (multiple samples observed)
                    Min = (ls.Min.HasValue && ls.Max.HasValue && ls.Min != ls.Max) ? ls.Min : null,
                    Max = (ls.Min.HasValue && ls.Max.HasValue && ls.Min != ls.Max) ? ls.Max : null,
                    Values = ls.Values?.Count > 0 ? ls.Values : null
                }).ToList()
            };
        }

        /// <summary>
        /// Analyzes package lines using data-driven approach
        /// </summary>
        private List<LineStructure> AnalyzePackageLinesDataDriven(LogData logData, PackageInfo packageInfo, AnalysisResult analysis)
        {
            List<LineStructure> lineStructures = new List<LineStructure>();

            if (packageInfo == null || !packageInfo.IsPackageBased)
                return lineStructures;

            // Get all messages and group them by line position within packages
            List<List<byte[]>> lineGroups = GroupMessagesByLinePosition(logData.Messages, packageInfo.PackageSize);

            char delimiter = analysis.Delimiters.Count > 0
                ? analysis.Delimiters.First().Delimiter
                : ',';

            // Analyze each line position
            for (int lineNumber = 0; lineNumber < lineGroups.Count; lineNumber++)
            {
                var lineGroup = lineGroups[lineNumber];

                // Use data-driven analyzer on this line group's messages
                var tempLogData = new LogData { Messages = lineGroup };
                var fields = _lineAnalyzer.AnalyzeFields(tempLogData, delimiter);

                // Convert FieldInfo to LineStructure
                LineStructure lineStructure = ConvertFieldInfoToLineStructure(fields, lineNumber + 1, lineGroup);
                lineStructures.Add(lineStructure);
            }

            return lineStructures;
        }

        /// <summary>
        /// Groups messages by their position within packages
        /// </summary>
        private List<List<byte[]>> GroupMessagesByLinePosition(List<byte[]> messages, int packageSize)
        {
            List<List<byte[]>> lineGroups = new List<List<byte[]>>();

            // Initialize groups for each line position
            for (int i = 0; i < packageSize; i++)
            {
                lineGroups.Add(new List<byte[]>());
            }

            // Distribute messages into line groups
            for (int i = 0; i < messages.Count; i++)
            {
                int linePosition = i % packageSize;
                lineGroups[linePosition].Add(messages[i]);
            }

            return lineGroups;
        }

        /// <summary>
        /// Converts FieldInfo collection to a single LineStructure (purely data-driven)
        /// </summary>
        private LineStructure ConvertFieldInfoToLineStructure(List<FieldInfo> fields, int lineNumber, List<byte[]> samples)
        {
            LineStructure structure = new LineStructure
            {
                LineNumber = lineNumber,
                Name = $"Line{lineNumber}",
                RawSample = samples.Count > 0 ? System.Text.Encoding.ASCII.GetString(samples[0]).Trim() : ""
            };

            // If single field, use it directly
            if (fields.Count == 1)
            {
                var field = fields[0];
                structure.Type = field.Type;
                structure.Unit = field.Unit;
                structure.Min = field.MinValue;
                structure.Max = field.MaxValue;
                structure.Values = field.UniqueValues.Count > 0 ? field.UniqueValues : null;
            }
            else if (fields.Count > 1)
            {
                // Multiple fields - describe what we observed
                structure.Type = "multi-field";

                // Collect all unique values from all fields
                var allValues = new List<string>();
                foreach (var field in fields)
                {
                    if (field.UniqueValues != null)
                        allValues.AddRange(field.UniqueValues);
                }
                structure.Values = allValues.Distinct().ToList();
            }
            else
            {
                // No fields detected
                structure.Type = "string";
            }

            return structure;
        }

        /// <summary>
        /// Gets terminator string representation
        /// </summary>
        private string GetTerminatorString(TerminatorInfo terminator)
        {
            if (terminator == null || !terminator.Detected)
                return "\\r\\n";

            switch (terminator.Type)
            {
                case "CRLF": return "\\r\\n";
                case "LF": return "\\n";
                case "CR": return "\\r";
                default: return "\\r\\n";
            }
        }

        /// <summary>
        /// Generates field definitions
        /// </summary>
        private List<FieldDefinition> GenerateFields(List<FieldInfo> fields)
        {
            var result = new List<FieldDefinition>();

            foreach (var field in fields)
            {
                var fieldDef = new FieldDefinition
                {
                    Name = field.SuggestedName,
                    Position = field.Position,
                    Type = field.Type,
                    Unit = field.Unit
                };

                if (!string.IsNullOrEmpty(field.Unit))
                {
                    fieldDef.UnitAttached = field.UnitAttached;
                }

                if (field.IsConstant && field.UniqueValues.Count > 0)
                {
                    fieldDef.Values = field.UniqueValues;
                }

                result.Add(fieldDef);
            }

            return result;
        }

        #endregion
    }
}
