using System;

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Represents a single log entry from a captured serial communication log
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Timestamp of the log entry
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Raw data as byte array (source of truth - original binary form)
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
        /// Raw data as text string (computed from RawBytes using ASCII)
        /// Example: "  0.360 kg    G"
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
                // If setting from text, convert to bytes
                if (!string.IsNullOrEmpty(value))
                    RawBytes = System.Text.Encoding.ASCII.GetBytes(value);
            }
        }

        /// <summary>
        /// Direction of communication (TX/RX, Send/Receive, etc.)
        /// </summary>
        public string Direction { get; set; }

        /// <summary>
        /// Length of raw data in bytes
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
        /// Constructor
        /// </summary>
        public LogEntry()
        {
            Timestamp = DateTime.Now;
            RawBytes = null;
            Direction = string.Empty;
        }

        /// <summary>
        /// Constructor with byte array (PREFERRED)
        /// </summary>
        /// <param name="timestamp">Timestamp</param>
        /// <param name="rawBytes">Raw data bytes</param>
        /// <param name="direction">Direction (TX/RX)</param>
        public LogEntry(DateTime timestamp, byte[] rawBytes, string direction)
        {
            Timestamp = timestamp;
            RawBytes = rawBytes;
            Direction = direction ?? string.Empty;
        }

        /// <summary>
        /// Constructor with text string (converts to bytes)
        /// </summary>
        /// <param name="timestamp">Timestamp</param>
        /// <param name="rawData">Raw data string</param>
        /// <param name="direction">Direction (TX/RX)</param>
        public LogEntry(DateTime timestamp, string rawData, string direction)
        {
            Timestamp = timestamp;
            RawBytes = System.Text.Encoding.ASCII.GetBytes(rawData ?? string.Empty);
            Direction = direction ?? string.Empty;
        }
    }
}
