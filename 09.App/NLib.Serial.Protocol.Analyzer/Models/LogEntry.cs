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
        /// Raw data as string (may contain hex representation or text)
        /// </summary>
        public string RawData { get; set; }

        /// <summary>
        /// Raw data as byte array (original binary form)
        /// </summary>
        public byte[] RawBytes { get; set; }

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
            RawData = string.Empty;
            Direction = string.Empty;
        }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="timestamp">Timestamp</param>
        /// <param name="rawData">Raw data string</param>
        /// <param name="direction">Direction (TX/RX)</param>
        public LogEntry(DateTime timestamp, string rawData, string direction)
        {
            Timestamp = timestamp;
            RawData = rawData ?? string.Empty;
            Direction = direction ?? string.Empty;
        }

        /// <summary>
        /// Constructor with byte array
        /// </summary>
        /// <param name="timestamp">Timestamp</param>
        /// <param name="rawBytes">Raw data bytes</param>
        /// <param name="direction">Direction (TX/RX)</param>
        public LogEntry(DateTime timestamp, byte[] rawBytes, string direction)
        {
            Timestamp = timestamp;
            RawBytes = rawBytes;
            RawData = BitConverter.ToString(rawBytes).Replace("-", " "); // Hex representation
            Direction = direction ?? string.Empty;
        }
    }
}
