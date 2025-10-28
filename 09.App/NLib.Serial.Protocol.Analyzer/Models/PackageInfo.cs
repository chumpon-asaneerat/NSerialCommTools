using System;
using System.Collections.Generic;
using System.Linq;

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Represents a parsed package from the log data
    /// A package is a complete data unit from a device (may contain multiple segments)
    /// </summary>
    public class PackageInfo
    {
        /// <summary>
        /// Package number (1-based for display)
        /// </summary>
        public int PackageNumber { get; set; }

        /// <summary>
        /// Start index in the original log entries
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// End index in the original log entries
        /// </summary>
        public int EndIndex { get; set; }

        /// <summary>
        /// Raw data of the entire package
        /// </summary>
        public string RawData { get; set; }

        /// <summary>
        /// Raw data as byte array
        /// </summary>
        public byte[] RawBytes { get; set; }

        /// <summary>
        /// List of segments within this package
        /// </summary>
        public List<SegmentInfo> Segments { get; set; }

        /// <summary>
        /// Total length in bytes
        /// </summary>
        public int Length
        {
            get
            {
                if (RawBytes != null)
                    return RawBytes.Length;
                if (!string.IsNullOrEmpty(RawData))
                    return RawData.Length;
                return 0;
            }
        }

        /// <summary>
        /// Number of segments in this package
        /// </summary>
        public int SegmentCount
        {
            get { return Segments != null ? Segments.Count : 0; }
        }

        /// <summary>
        /// Timestamp of the package (from first log entry)
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public PackageInfo()
        {
            PackageNumber = 0;
            StartIndex = 0;
            EndIndex = 0;
            RawData = string.Empty;
            Segments = new List<SegmentInfo>();
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="packageNumber">Package number</param>
        /// <param name="rawData">Raw package data</param>
        public PackageInfo(int packageNumber, string rawData)
        {
            PackageNumber = packageNumber;
            StartIndex = 0;
            EndIndex = 0;
            RawData = rawData ?? string.Empty;
            Segments = new List<SegmentInfo>();
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Adds a segment to this package
        /// </summary>
        /// <param name="segment">Segment to add</param>
        public void AddSegment(SegmentInfo segment)
        {
            if (Segments == null)
                Segments = new List<SegmentInfo>();

            Segments.Add(segment);
        }

        /// <summary>
        /// Gets a display string for this package
        /// </summary>
        /// <returns>Display string (e.g., "Package #1 (120 bytes)")</returns>
        public string GetDisplayString()
        {
            return $"Package #{PackageNumber} ({Length} bytes)";
        }
    }
}
