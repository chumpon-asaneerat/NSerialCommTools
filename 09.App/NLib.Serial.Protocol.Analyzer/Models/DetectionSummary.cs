using System;

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Summary of detection results from field analysis
    /// Contains terminator/delimiter info and protocol classification
    /// </summary>
    public class DetectionSummary
    {
        /// <summary>
        /// Package terminator (hex string, e.g., "0D 0A")
        /// </summary>
        public string PackageTerminator { get; set; }

        /// <summary>
        /// Number of times package terminator was found
        /// </summary>
        public int PackageTerminatorOccurrences { get; set; }

        /// <summary>
        /// Segment delimiter (hex string, e.g., "2C" for comma)
        /// </summary>
        public string SegmentDelimiter { get; set; }

        /// <summary>
        /// Detected protocol type (e.g., "SinglePackage", "PackageBased with Segments")
        /// </summary>
        public string ProtocolType { get; set; }

        /// <summary>
        /// Number of fields detected
        /// </summary>
        public int FieldCount { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DetectionSummary()
        {
            PackageTerminator = "None";
            PackageTerminatorOccurrences = 0;
            SegmentDelimiter = "None";
            ProtocolType = "Unknown";
            FieldCount = 0;
        }
    }
}
