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
        /// Raw data as byte array (source of truth)
        /// </summary>
        public byte[] RawBytes { get; set; }

        /// <summary>
        /// Raw data as hex string (computed from RawBytes)
        /// Example: "5E 4B 4A 49 4B 30 30 30 0D 0A"
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
        /// Example: "^KJIK000\r\n"
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
        /// Legacy property for backward compatibility (same as RawText)
        /// </summary>
        [Obsolete("Use RawHex or RawText instead")]
        public string RawData
        {
            get { return RawText; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    RawBytes = System.Text.Encoding.ASCII.GetBytes(value);
            }
        }

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
            RawBytes = null;
            Fields = new Dictionary<string, object>();
        }

        /// <summary>
        /// Constructor with byte array (PREFERRED)
        /// </summary>
        /// <param name="segmentIndex">Segment index</param>
        /// <param name="rawBytes">Raw bytes</param>
        public SegmentInfo(int segmentIndex, byte[] rawBytes)
        {
            SegmentIndex = segmentIndex;
            RawBytes = rawBytes;
            Fields = new Dictionary<string, object>();
        }

        /// <summary>
        /// Constructor with text string (converts to bytes)
        /// </summary>
        /// <param name="segmentIndex">Segment index</param>
        /// <param name="rawData">Raw data</param>
        public SegmentInfo(int segmentIndex, string rawData)
        {
            SegmentIndex = segmentIndex;
            RawBytes = System.Text.Encoding.ASCII.GetBytes(rawData ?? string.Empty);
            Fields = new Dictionary<string, object>();
        }
    }
}
