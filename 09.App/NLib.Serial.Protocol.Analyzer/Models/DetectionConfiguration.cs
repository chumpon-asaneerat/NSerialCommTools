using System;

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Detection configuration for protocol analysis
    /// Tracks Package Start, Package End, Segment Separator, and Encoding with detection modes
    /// </summary>
    public class DetectionConfiguration
    {
        /// <summary>
        /// Package Start Marker (e.g., "^KJIK000", STX byte)
        /// </summary>
        public DetectionModeInfo PackageStartMarker { get; set; }

        /// <summary>
        /// Package End Marker (e.g., "~P1", ETX byte)
        /// </summary>
        public DetectionModeInfo PackageEndMarker { get; set; }

        /// <summary>
        /// Segment Separator within a package (e.g., "\r\n", ",", "|")
        /// </summary>
        public DetectionModeInfo SegmentSeparator { get; set; }

        /// <summary>
        /// Text encoding (ASCII, UTF-8, UTF-16, Latin-1)
        /// </summary>
        public DetectionModeInfo Encoding { get; set; }

        /// <summary>
        /// Constructor - initializes all parameters
        /// </summary>
        public DetectionConfiguration()
        {
            PackageStartMarker = new DetectionModeInfo();
            PackageEndMarker = new DetectionModeInfo();
            SegmentSeparator = new DetectionModeInfo();
            Encoding = new DetectionModeInfo();
        }

        /// <summary>
        /// Sets all parameters to auto-detected values
        /// </summary>
        /// <param name="packageStart">Detected package start marker</param>
        /// <param name="packageEnd">Detected package end marker</param>
        /// <param name="segmentSep">Detected segment separator</param>
        /// <param name="encoding">Detected encoding</param>
        public void SetAutoDetected(string packageStart, string packageEnd, string segmentSep, string encoding)
        {
            if (!string.IsNullOrEmpty(packageStart))
                PackageStartMarker.SetAutoDetected(packageStart);

            if (!string.IsNullOrEmpty(packageEnd))
                PackageEndMarker.SetAutoDetected(packageEnd);

            if (!string.IsNullOrEmpty(segmentSep))
                SegmentSeparator.SetAutoDetected(segmentSep);

            if (!string.IsNullOrEmpty(encoding))
                Encoding.SetAutoDetected(encoding);
        }

        /// <summary>
        /// Sets manual values for specified parameters
        /// </summary>
        /// <param name="packageStart">Manual package start marker (null to skip)</param>
        /// <param name="packageEnd">Manual package end marker (null to skip)</param>
        /// <param name="segmentSep">Manual segment separator (null to skip)</param>
        /// <param name="encoding">Manual encoding (null to skip)</param>
        public void SetManual(string packageStart, string packageEnd, string segmentSep, string encoding)
        {
            if (packageStart != null)
                PackageStartMarker.SetManual(packageStart);

            if (packageEnd != null)
                PackageEndMarker.SetManual(packageEnd);

            if (segmentSep != null)
                SegmentSeparator.SetManual(segmentSep);

            if (encoding != null)
                Encoding.SetManual(encoding);
        }

        /// <summary>
        /// Clears all detection configuration
        /// </summary>
        public void Clear()
        {
            PackageStartMarker.Clear();
            PackageEndMarker.Clear();
            SegmentSeparator.Clear();
            Encoding.Clear();
        }

        /// <summary>
        /// Checks if configuration is complete (has values for essential parameters)
        /// </summary>
        /// <returns>True if at least segment separator or package markers are set</returns>
        public bool IsComplete()
        {
            bool hasSegmentSeparator = !string.IsNullOrEmpty(SegmentSeparator.EffectiveValue);
            bool hasPackageMarkers = !string.IsNullOrEmpty(PackageStartMarker.EffectiveValue) ||
                                      !string.IsNullOrEmpty(PackageEndMarker.EffectiveValue);

            return hasSegmentSeparator || hasPackageMarkers;
        }

        /// <summary>
        /// Applies this configuration to a parsing context (placeholder for future implementation)
        /// </summary>
        public void ApplyTo(object parserContext)
        {
            // TODO: Implement when parser is created
            // This method will apply the effective values to the parser
        }
    }
}
