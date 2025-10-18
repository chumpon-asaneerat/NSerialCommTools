#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLib.Serial.Protocol.Analyzer.Models;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Generators
{
    /// <summary>
    /// Generates protocol definitions from analysis results
    /// </summary>
    public class DefinitionGenerator
    {
        #region Public Methods

        /// <summary>
        /// Generates a protocol definition from analysis results
        /// </summary>
        public ProtocolDefinition Generate(AnalysisResult analysis, string deviceName, string sourceFile)
        {
            var definition = new ProtocolDefinition
            {
                DeviceInfo = new DeviceInfo
                {
                    Name = deviceName,
                    Category = "unknown",
                    Description = $"Auto-generated from {Path.GetFileName(sourceFile)}"
                },

                Protocol = new ProtocolInfo
                {
                    Type = analysis.ProtocolType,
                    Format = analysis.ProtocolType == "multi-line-package" ? "multi-line-package" : "csv",
                    Encoding = "ASCII",
                    Terminator = GetTerminatorString(analysis.Terminator),
                    Fields = GenerateFields(analysis.Fields)
                },

                Parsing = new ParsingInfo
                {
                    Strategy = analysis.RecommendedStrategy,
                    Delimiter = analysis.Delimiters.Count > 0
                        ? analysis.Delimiters.First().Delimiter.ToString()
                        : ","
                }
            };

            // Add package information if detected
            if (analysis.ProtocolType == "multi-line-package")
            {
                definition.Protocol.PackageSize = analysis.PackageSize;
                definition.Protocol.StartMarker = analysis.StartMarker;
                definition.Protocol.EndMarker = analysis.EndMarker;
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
