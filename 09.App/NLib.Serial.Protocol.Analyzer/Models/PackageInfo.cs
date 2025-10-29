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
        /// Raw data as byte array (source of truth)
        /// </summary>
        public byte[] RawBytes { get; set; }

        /// <summary>
        /// Raw data as hex string (computed from RawBytes)
        /// Example: "02 41 00 64 12 34 03 C5"
        /// </summary>
        public string RawHex
        {
            get
            {
                if (RawBytes == null || RawBytes.Length == 0)
                    return string.Empty;
                return BitConverter.ToString(RawBytes).Replace("-", " ");
            }
        }

        /// <summary>
        /// Raw data as text string (computed from RawBytes)
        /// Example: "^KJIK000\r\n2023-11-07\r\n..."
        /// </summary>
        public string RawText
        {
            get
            {
                if (RawBytes == null || RawBytes.Length == 0)
                    return string.Empty;

                try
                {
                    return System.Text.Encoding.ASCII.GetString(RawBytes);
                }
                catch
                {
                    return "[Binary Data]";
                }
            }
        }

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
                return (RawBytes != null) ? RawBytes.Length : 0;
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
            RawBytes = null;
            Segments = new List<SegmentInfo>();
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Constructor with byte array (PREFERRED)
        /// </summary>
        /// <param name="packageNumber">Package number</param>
        /// <param name="rawBytes">Raw package data bytes</param>
        public PackageInfo(int packageNumber, byte[] rawBytes)
        {
            PackageNumber = packageNumber;
            StartIndex = 0;
            EndIndex = 0;
            RawBytes = rawBytes;
            Segments = new List<SegmentInfo>();
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Constructor with text string (converts to bytes)
        /// </summary>
        /// <param name="packageNumber">Package number</param>
        /// <param name="rawData">Raw package data</param>
        public PackageInfo(int packageNumber, string rawData)
        {
            PackageNumber = packageNumber;
            StartIndex = 0;
            EndIndex = 0;
            RawBytes = System.Text.Encoding.ASCII.GetBytes(rawData ?? string.Empty);
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
        /// Display text for UI binding (e.g., "Package #1 (120 bytes)")
        /// </summary>
        public string DisplayText
        {
            get { return $"Package #{PackageNumber} ({Length} bytes)"; }
        }

        /// <summary>
        /// Gets a display string for this package
        /// </summary>
        /// <returns>Display string (e.g., "Package #1 (120 bytes)")</returns>
        public string GetDisplayString()
        {
            return DisplayText;
        }
    }
}
