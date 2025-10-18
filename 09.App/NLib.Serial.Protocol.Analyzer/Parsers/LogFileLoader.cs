#region Using

using System;
using System.Collections.Generic;
using NLib.Serial.Protocol.Analyzer.Models;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Parsers
{
    /// <summary>
    /// Loads and processes log files
    /// </summary>
    public class LogFileLoader
    {
        #region Private Fields

        private HexLogParser _parser = new HexLogParser();
        private MessageExtractor _extractor = new MessageExtractor();
        private LogFormatDetector _detector = new LogFormatDetector();

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads a log file and extracts messages
        /// </summary>
        /// <param name="path">Path to the log file</param>
        /// <returns>LogData with extracted messages</returns>
        public LogData LoadFile(string path)
        {
            // Step 1: Detect format
            LogFileFormat format = _detector.DetectFormat(path);

            // Step 2: Parse hex log to byte array
            byte[] rawBytes = _parser.ParseLogFile(path);

            // Step 3: Extract individual messages
            List<byte[]> messages = _extractor.ExtractMessages(rawBytes);

            // Step 4: Build LogData object
            LogData data = new LogData
            {
                Messages = messages,
                TotalBytes = rawBytes.Length,
                MessageCount = messages.Count,
                AverageMessageLength = messages.Count > 0
                    ? rawBytes.Length / messages.Count
                    : 0,
                DetectedFormat = format,
                SourceFilePath = path
            };

            return data;
        }

        #endregion
    }
}
