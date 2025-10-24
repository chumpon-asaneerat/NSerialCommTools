#region Using

using System.Text;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Models
{
    /// <summary>
    /// Contains all detection results from Pass 1 (Protocol Detection Phase).
    /// Holds detected encoding, terminator hierarchy, frame markers, and confidence scores.
    /// This is the output of ProtocolDetector.DetectProtocolStructure() and input to MessageExtractor.
    /// </summary>
    public class DetectionResult
    {
        #region Public Properties

        #region Encoding Detection

        /// <summary>
        /// Gets or sets the detected text encoding (UTF-8, ASCII, UTF-16, etc.).
        /// Used for converting byte[] to string for display/analysis only, NOT for splitting.
        /// </summary>
        public Encoding DetectedEncoding { get; set; }

        /// <summary>
        /// Gets or sets the encoding name as string (e.g., "UTF-8", "ASCII", "UTF-16LE").
        /// </summary>
        public string EncodingName { get; set; }

        /// <summary>
        /// Gets or sets the confidence (0.0 to 1.0) that the detected encoding is correct.
        /// 1.0 = BOM detected (certain), 0.95+ = strong pattern match, 0.7+ = likely match.
        /// </summary>
        public double EncodingConfidence { get; set; }

        #endregion

        #region Terminator Hierarchy (Binary-First: Frame → Segment → Field)

        /// <summary>
        /// Gets or sets the frame terminator (separates complete messages/frames).
        /// Examples: Double CRLF [0x0D 0x0A 0x0D 0x0A], ETX [0x03], frame markers.
        /// This is the TOP level of the hierarchy.
        /// </summary>
        public TerminatorInfo FrameTerminator { get; set; }

        /// <summary>
        /// Gets or sets the segment terminator (separates segments/chunks within a frame).
        /// Examples: Single CRLF [0x0D 0x0A], LF [0x0A], record separator [0x1E].
        /// This is the MIDDLE level of the hierarchy.
        /// NOTE: NOT "line terminator" - that's text-thinking! Use binary-first terminology.
        /// </summary>
        public TerminatorInfo SegmentTerminator { get; set; }

        /// <summary>
        /// Gets or sets whether the protocol has segment structure (multi-segment frames).
        /// true = Frame contains multiple segments (e.g., JIK6CAB: 10 segments per frame)
        /// false = Frame is single segment (e.g., simple CSV)
        /// </summary>
        public bool HasSegmentStructure { get; set; }

        /// <summary>
        /// Gets or sets the field delimiter (separates fields within a segment).
        /// Examples: Space [0x20], Comma [0x2C], Tab [0x09], unit separator [0x1F].
        /// This is the BOTTOM level of the hierarchy.
        /// </summary>
        public TerminatorInfo FieldDelimiter { get; set; }

        /// <summary>
        /// Gets or sets whether the protocol has field delimiters.
        /// true = Delimited protocol (CSV, space-separated, tab-separated)
        /// false = Fixed-position or binary protocol
        /// </summary>
        public bool HasFieldDelimiter { get; set; }

        #endregion

        #region Frame Markers

        /// <summary>
        /// Gets or sets the start marker (optional frame boundary marker at beginning).
        /// Examples: STX [0x02], '^', '~', '&lt;START&gt;'.
        /// null if no start marker detected.
        /// </summary>
        public FrameMarkerInfo StartMarker { get; set; }

        /// <summary>
        /// Gets or sets the end marker (optional frame boundary marker at end).
        /// Examples: ETX [0x03], frame footer.
        /// null if no end marker detected.
        /// </summary>
        public FrameMarkerInfo EndMarker { get; set; }

        #endregion

        #region Protocol Structure

        /// <summary>
        /// Gets or sets the detected protocol structure type.
        /// Determines the overall parsing strategy.
        /// </summary>
        public ProtocolStructure Structure { get; set; }

        /// <summary>
        /// Gets or sets the overall confidence (0.0 to 1.0) of the entire detection.
        /// Calculated from encoding confidence + terminator confidences.
        /// 0.9+ = Very confident, 0.7-0.9 = Confident, 0.5-0.7 = Uncertain, &lt;0.5 = Guessing
        /// </summary>
        public double OverallConfidence { get; set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DetectionResult class.
        /// </summary>
        public DetectionResult()
        {
            // Default to ASCII encoding (most common, safest fallback)
            DetectedEncoding = Encoding.ASCII;
            EncodingName = "ASCII";
            EncodingConfidence = 0.5; // Low confidence - just a guess

            // Default structure
            Structure = ProtocolStructure.Unknown;
            OverallConfidence = 0.0;

            // Flags default to false
            HasSegmentStructure = false;
            HasFieldDelimiter = false;
        }

        #endregion
    }

    #region Enums

    /// <summary>
    /// Defines the overall structure of the protocol.
    /// Determines how the data should be parsed in Pass 2 (Extraction).
    /// </summary>
    public enum ProtocolStructure
    {
        /// <summary>
        /// Unknown structure - detection failed or ambiguous.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Flat delimited structure (no segments).
        /// Example: Single-line CSV (Frame → Fields directly)
        /// Frame terminator = CRLF, Field delimiter = Comma
        /// </summary>
        FlatDelimited = 1,

        /// <summary>
        /// Segmented delimited structure (multi-segment frames).
        /// Example: JIK6CAB (Frame → Segments → Fields)
        /// Frame terminator = Double CRLF, Segment terminator = CRLF, Field delimiter = Space
        /// </summary>
        SegmentedDelimited = 2,

        /// <summary>
        /// Flat fixed-position structure (no segments, no delimiters).
        /// Example: Fixed-width single-line records
        /// Frame terminator = CRLF, Fields at fixed positions
        /// </summary>
        FlatFixedPosition = 3,

        /// <summary>
        /// Segmented fixed-position structure (multi-segment, fixed positions).
        /// Example: Multi-line records with fixed-width fields
        /// Frame terminator = Double CRLF, Segment terminator = CRLF, Fields at fixed positions
        /// </summary>
        SegmentedFixedPosition = 4,

        /// <summary>
        /// Pure binary protocol (byte sequences, no text delimiters).
        /// Example: Binary protocol with length prefixes or special markers
        /// Terminators are binary bytes (STX, ETX, etc.)
        /// </summary>
        Binary = 5
    }

    /// <summary>
    /// Information about frame markers (optional boundary markers).
    /// </summary>
    public class FrameMarkerInfo
    {
        /// <summary>
        /// Gets or sets the marker as a byte array.
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// Gets or sets the display name (e.g., "STX", "ETX", "StartMarker").
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the confidence (0.0 to 1.0) that this is a frame marker.
        /// </summary>
        public double Confidence { get; set; }
    }

    #endregion
}
