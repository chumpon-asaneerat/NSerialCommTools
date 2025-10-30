using System;
using System.Collections.Generic;
using NLib.Serial.Protocol.Analyzer.Analyzers;
using NLib.Serial.Protocol.Analyzer.Parsers;

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Main shared model for the Protocol Analyzer application
    /// This model is passed to all pages and contains the complete application state
    /// </summary>
    public class ProtocolAnalyzerModel
    {
        #region Page 1: Log Data & Detection

        /// <summary>
        /// Detection configuration (Package markers, Segment separator, Encoding)
        /// </summary>
        public DetectionConfiguration DetectionConfig { get; set; }

        /// <summary>
        /// Loaded log file with all entries
        /// </summary>
        public LogFile LogFile { get; set; }

        /// <summary>
        /// Log file analyzer for auto-detection algorithms
        /// Encapsulated in model to hide implementation from UI
        /// </summary>
        public LogFileAnalyzer Analyzer { get; private set; }

        #endregion

        #region Page 2: Parsing

        /// <summary>
        /// List of parsed packages from the log data
        /// </summary>
        public List<PackageInfo> Packages { get; set; }

        /// <summary>
        /// Total number of packages
        /// </summary>
        public int PackageCount
        {
            get { return Packages != null ? Packages.Count : 0; }
        }

        /// <summary>
        /// Package parser for splitting log entries into packages and segments
        /// Encapsulated in model to hide implementation from UI
        /// </summary>
        public PackageParser Parser { get; private set; }

        #endregion

        #region Page 3: Analysis

        /// <summary>
        /// Analysis result with field definitions
        /// </summary>
        public AnalysisResult AnalysisResult { get; set; }

        /// <summary>
        /// Currently selected package index for analysis
        /// </summary>
        public int SelectedPackageIndex { get; set; }

        /// <summary>
        /// Field analyzer for running 5-stage statistical analysis pipeline
        /// Encapsulated in model to hide implementation from UI
        /// </summary>
        public FieldAnalyzer FieldAnalyzer { get; private set; }

        #endregion

        #region Page 4: JSON Schema

        /// <summary>
        /// Generated JSON schema as string
        /// </summary>
        public string JsonSchema { get; set; }

        /// <summary>
        /// Device name for the schema
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Schema version
        /// </summary>
        public string SchemaVersion { get; set; }

        #endregion

        #region General Properties

        /// <summary>
        /// Application state (which page is active, what operations are available)
        /// </summary>
        public Dictionary<string, object> AppState { get; set; }

        #endregion

        /// <summary>
        /// Constructor - initializes all properties
        /// </summary>
        public ProtocolAnalyzerModel()
        {
            // Page 1
            DetectionConfig = new DetectionConfiguration();
            LogFile = new LogFile();
            Analyzer = new LogFileAnalyzer(); // Uses default config

            // Page 2
            Packages = new List<PackageInfo>();
            Parser = new PackageParser();

            // Page 3
            AnalysisResult = new AnalysisResult();
            SelectedPackageIndex = 0;
            FieldAnalyzer = new FieldAnalyzer();

            // Page 4
            JsonSchema = string.Empty;
            DeviceName = "UnknownDevice";
            SchemaVersion = "1.0";

            // General
            AppState = new Dictionary<string, object>();
        }

        /// <summary>
        /// Resets the model to initial state
        /// </summary>
        public void Reset()
        {
            DetectionConfig.Clear();
            LogFile.Clear();
            Packages.Clear();
            AnalysisResult = new AnalysisResult();
            SelectedPackageIndex = 0;
            JsonSchema = string.Empty;
            DeviceName = "UnknownDevice";
            SchemaVersion = "1.0";
            AppState.Clear();
        }

        /// <summary>
        /// Checks if Page 2 (Parsing) can be accessed
        /// Requires: Log file loaded AND detection config complete
        /// </summary>
        /// <returns>True if Page 2 can be accessed</returns>
        public bool CanAccessPage2()
        {
            return LogFile != null &&
                   LogFile.EntryCount > 0 &&
                   DetectionConfig != null &&
                   DetectionConfig.IsComplete();
        }

        /// <summary>
        /// Checks if Page 3 (Analysis) can be accessed
        /// Requires: Packages parsed
        /// </summary>
        /// <returns>True if Page 3 can be accessed</returns>
        public bool CanAccessPage3()
        {
            return Packages != null && Packages.Count > 0;
        }

        /// <summary>
        /// Checks if Page 4 (JSON Schema) can be accessed
        /// Requires: Field definitions created
        /// </summary>
        /// <returns>True if Page 4 can be accessed</returns>
        public bool CanAccessPage4()
        {
            return AnalysisResult != null &&
                   AnalysisResult.FieldList != null &&
                   AnalysisResult.FieldList.Count > 0;
        }
    }
}
