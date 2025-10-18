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

        private PackageLineAnalyzer _lineAnalyzer = new PackageLineAnalyzer();

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
        /// Generates package structure with detailed line analysis
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
                    Description = "Package start marker with device ID",
                    Action = "reset-state"
                },
                End = new MarkerDetail
                {
                    Pattern = analysis.EndMarker,
                    Description = "Package end marker",
                    Action = "complete-package"
                }
            };

            // Analyze each line in the package
            var lineStructures = _lineAnalyzer.AnalyzePackageLines(logData, packageInfo);

            // Convert to LineDefinition
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
                    UnitAttached = ls.UnitAttached,
                    Min = ls.Min,
                    Max = ls.Max,
                    Required = ls.Required,
                    Description = ls.Description,
                    Values = ls.Values?.Count > 0 ? ls.Values : null
                }).ToList()
            };
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
