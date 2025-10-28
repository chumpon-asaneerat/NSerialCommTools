using System;
using System.Collections.Generic;

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Represents a single segment within a package
    /// A segment is a sub-unit of a package (like a line in a multi-line package)
    /// </summary>
    public class SegmentInfo
    {
        /// <summary>
        /// Segment index within the package (0-based)
        /// </summary>
        public int SegmentIndex { get; set; }

        /// <summary>
        /// Raw data of this segment
        /// </summary>
        public string RawData { get; set; }

        /// <summary>
        /// Raw data as byte array
        /// </summary>
        public byte[] RawBytes { get; set; }

        /// <summary>
        /// Length of segment in bytes
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
        /// Parsed fields within this segment (if analyzed)
        /// </summary>
        public Dictionary<string, object> Fields { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SegmentInfo()
        {
            SegmentIndex = 0;
            RawData = string.Empty;
            Fields = new Dictionary<string, object>();
        }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="segmentIndex">Segment index</param>
        /// <param name="rawData">Raw data</param>
        public SegmentInfo(int segmentIndex, string rawData)
        {
            SegmentIndex = segmentIndex;
            RawData = rawData ?? string.Empty;
            Fields = new Dictionary<string, object>();
        }

        /// <summary>
        /// Constructor with byte array
        /// </summary>
        /// <param name="segmentIndex">Segment index</param>
        /// <param name="rawBytes">Raw bytes</param>
        public SegmentInfo(int segmentIndex, byte[] rawBytes)
        {
            SegmentIndex = segmentIndex;
            RawBytes = rawBytes;
            RawData = System.Text.Encoding.ASCII.GetString(rawBytes);
            Fields = new Dictionary<string, object>();
        }
    }
}
