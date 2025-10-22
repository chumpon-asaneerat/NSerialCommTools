#region Using

using System;
using System.Collections.Generic;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Models
{
    /// <summary>
    /// Represents captured or loaded serial log data.
    /// </summary>
    public class LogData
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogData()
        {
            Messages = new List<byte[]>();
            CapturedTime = DateTime.Now;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the list of individual messages (as byte arrays).
        /// </summary>
        public List<byte[]> Messages { get; set; }

        /// <summary>
        /// Gets or sets the total number of bytes across all messages.
        /// </summary>
        public int TotalBytes { get; set; }

        /// <summary>
        /// Gets or sets the total number of messages.
        /// </summary>
        public int MessageCount { get; set; }

        /// <summary>
        /// Gets or sets the average message length in bytes.
        /// </summary>
        public int AverageMessageLength { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the data was captured.
        /// </summary>
        public DateTime CapturedTime { get; set; }

        #endregion
    }
}
