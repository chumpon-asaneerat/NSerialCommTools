using System;
using System.Collections.Generic;
using System.Linq;

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Represents the result of protocol analysis
    /// Contains field definitions and statistics
    /// </summary>
    public class AnalysisResult
    {
        /// <summary>
        /// Total number of log entries analyzed
        /// </summary>
        public int TotalEntries { get; set; }

        /// <summary>
        /// Total number of packages detected
        /// </summary>
        public int TotalPackages { get; set; }

        /// <summary>
        /// List of detected fields from analysis
        /// </summary>
        public List<FieldInfo> DetectedFields { get; set; }

        /// <summary>
        /// Overall confidence score (0.0 to 1.0)
        /// </summary>
        public double OverallConfidence { get; set; }

        /// <summary>
        /// Detection summary information (terminators, delimiters, protocol type)
        /// </summary>
        public DetectionSummary DetectionSummary { get; set; }

        /// <summary>
        /// Total number of packages analyzed (legacy property)
        /// </summary>
        public int PackageCount
        {
            get { return TotalPackages; }
            set { TotalPackages = value; }
        }

        /// <summary>
        /// List of field definitions discovered/created (legacy property)
        /// </summary>
        public List<FieldInfo> FieldList
        {
            get { return DetectedFields; }
            set { DetectedFields = value; }
        }

        /// <summary>
        /// Number of fields defined
        /// </summary>
        public int FieldCount
        {
            get { return FieldList != null ? FieldList.Count : 0; }
        }

        /// <summary>
        /// Analysis statistics (key-value pairs)
        /// Examples: "AvgPackageSize", "MaxSegments", "MinSegments"
        /// </summary>
        public Dictionary<string, object> Statistics { get; set; }

        /// <summary>
        /// When the analysis was performed
        /// </summary>
        public DateTime AnalyzedAt { get; set; }

        /// <summary>
        /// Analysis notes or warnings
        /// </summary>
        public List<string> Notes { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AnalysisResult()
        {
            TotalEntries = 0;
            TotalPackages = 0;
            DetectedFields = new List<FieldInfo>();
            OverallConfidence = 0.0;
            DetectionSummary = new DetectionSummary();
            Statistics = new Dictionary<string, object>();
            Notes = new List<string>();
            AnalyzedAt = DateTime.Now;
        }

        /// <summary>
        /// Adds a field to the analysis result
        /// </summary>
        /// <param name="field">Field to add</param>
        public void AddField(FieldInfo field)
        {
            if (FieldList == null)
                FieldList = new List<FieldInfo>();

            FieldList.Add(field);
        }

        /// <summary>
        /// Adds a statistic
        /// </summary>
        /// <param name="key">Statistic name</param>
        /// <param name="value">Statistic value</param>
        public void AddStatistic(string key, object value)
        {
            if (Statistics == null)
                Statistics = new Dictionary<string, object>();

            Statistics[key] = value;
        }

        /// <summary>
        /// Adds a note
        /// </summary>
        /// <param name="note">Note text</param>
        public void AddNote(string note)
        {
            if (Notes == null)
                Notes = new List<string>();

            Notes.Add(note);
        }

        /// <summary>
        /// Gets a summary string of the analysis
        /// </summary>
        /// <returns>Summary string</returns>
        public string GetSummary()
        {
            return $"Analyzed {PackageCount} packages, found {FieldCount} fields";
        }
    }
}
