#region Using

using System;
using System.Collections.Generic;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Represents loaded and preprocessed log data
    /// </summary>
    public class LogData
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public LogData()
        {
            Messages = new List<byte[]>();
            Statistics = new Dictionary<string, int>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the list of individual messages extracted from log
        /// </summary>
        public List<byte[]> Messages { get; set; }

        /// <summary>
        /// Gets or sets total number of bytes in raw log
        /// </summary>
        public int TotalBytes { get; set; }

        /// <summary>
        /// Gets or sets total number of messages
        /// </summary>
        public int MessageCount { get; set; }

        /// <summary>
        /// Gets or sets average message length in bytes
        /// </summary>
        public int AverageMessageLength { get; set; }

        /// <summary>
        /// Gets or sets the detected format of the log file
        /// </summary>
        public LogFileFormat DetectedFormat { get; set; }

        /// <summary>
        /// Gets or sets the source file path
        /// </summary>
        public string SourceFilePath { get; set; }

        /// <summary>
        /// Gets or sets additional statistics
        /// </summary>
        public Dictionary<string, int> Statistics { get; set; }

        #endregion
    }
}
