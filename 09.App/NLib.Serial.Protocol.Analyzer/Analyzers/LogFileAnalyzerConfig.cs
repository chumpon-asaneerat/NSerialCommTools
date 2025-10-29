namespace NLib.Serial.Protocol.Analyzer.Analyzers
{
    /// <summary>
    /// Configuration parameters for LogFileAnalyzer detection algorithms
    /// All thresholds and ranges are configurable for flexibility
    /// </summary>
    public class LogFileAnalyzerConfig
    {
        /// <summary>
        /// Minimum number of log entries required for reliable detection
        /// Default: 5 entries
        /// </summary>
        public int MinimumSampleSize { get; set; } = 5;

        /// <summary>
        /// Minimum sequence length to check for markers (in bytes)
        /// Default: 1 byte
        /// </summary>
        public int MinSequenceLength { get; set; } = 1;

        /// <summary>
        /// Maximum sequence length to check for markers (in bytes)
        /// Default: 4 bytes
        /// </summary>
        public int MaxSequenceLength { get; set; } = 4;

        /// <summary>
        /// Frequency threshold for start/end marker detection (0.0 to 1.0)
        /// A marker must appear in at least this percentage of entries
        /// Default: 0.30 (30%)
        /// </summary>
        public double MarkerFrequencyThreshold { get; set; } = 0.30;

        /// <summary>
        /// Frequency threshold for separator detection (0.0 to 1.0)
        /// A separator must appear in at least this percentage of entries
        /// Default: 0.20 (20%)
        /// </summary>
        public double SeparatorFrequencyThreshold { get; set; } = 0.20;

        /// <summary>
        /// Confidence threshold for encoding detection (0.0 to 1.0)
        /// Encoding must have at least this match ratio to be selected
        /// Default: 0.95 (95%)
        /// </summary>
        public double EncodingConfidenceThreshold { get; set; } = 0.95;

        /// <summary>
        /// Number of bytes to skip from start when detecting separator
        /// Avoids detecting start marker as separator
        /// Default: 2 bytes
        /// </summary>
        public int SeparatorSkipBytesFromStart { get; set; } = 2;

        /// <summary>
        /// Number of bytes to skip from end when detecting separator
        /// Avoids detecting end marker as separator
        /// Default: 2 bytes
        /// </summary>
        public int SeparatorSkipBytesFromEnd { get; set; } = 2;

        /// <summary>
        /// Minimum printable ASCII character code
        /// Default: 0x20 (space)
        /// </summary>
        public byte AsciiPrintableMin { get; set; } = 0x20;

        /// <summary>
        /// Maximum printable ASCII character code
        /// Default: 0x7E (~)
        /// </summary>
        public byte AsciiPrintableMax { get; set; } = 0x7E;

        /// <summary>
        /// Valid ASCII whitespace characters (TAB, LF, CR)
        /// Default: { 0x09, 0x0A, 0x0D }
        /// </summary>
        public byte[] AsciiWhitespaceChars { get; set; } = new byte[] { 0x09, 0x0A, 0x0D };

        /// <summary>
        /// Creates a default configuration with standard detection parameters
        /// </summary>
        public LogFileAnalyzerConfig()
        {
            // All defaults set via property initializers
        }

        /// <summary>
        /// Creates a configuration for strict detection (higher thresholds)
        /// </summary>
        public static LogFileAnalyzerConfig CreateStrictConfig()
        {
            return new LogFileAnalyzerConfig
            {
                MinimumSampleSize = 10,
                MarkerFrequencyThreshold = 0.50,  // 50%
                SeparatorFrequencyThreshold = 0.40,  // 40%
                EncodingConfidenceThreshold = 0.98  // 98%
            };
        }

        /// <summary>
        /// Creates a configuration for lenient detection (lower thresholds)
        /// </summary>
        public static LogFileAnalyzerConfig CreateLenientConfig()
        {
            return new LogFileAnalyzerConfig
            {
                MinimumSampleSize = 3,
                MarkerFrequencyThreshold = 0.15,  // 15%
                SeparatorFrequencyThreshold = 0.10,  // 10%
                EncodingConfidenceThreshold = 0.85  // 85%
            };
        }

        /// <summary>
        /// Creates a configuration optimized for binary protocols
        /// </summary>
        public static LogFileAnalyzerConfig CreateBinaryProtocolConfig()
        {
            return new LogFileAnalyzerConfig
            {
                MinSequenceLength = 1,
                MaxSequenceLength = 8,  // Longer sequences for binary
                MarkerFrequencyThreshold = 0.25,
                SeparatorFrequencyThreshold = 0.15,
                AsciiPrintableMin = 0x00,  // Allow all bytes
                AsciiPrintableMax = 0xFF
            };
        }
    }
}
