using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLib.Serial.Protocol.Analyzer.Models;

namespace NLib.Serial.Protocol.Analyzer.Parsers
{
    /// <summary>
    /// Parser for splitting log entries into packages and segments
    /// Uses detection configuration (markers, separators) to parse byte streams
    /// </summary>
    public class PackageParser
    {
        /// <summary>
        /// Splits log entries into packages using package start/end markers
        /// </summary>
        /// <param name="entries">List of log entries to parse</param>
        /// <param name="config">Detection configuration with markers</param>
        /// <returns>List of parsed packages</returns>
        public List<PackageInfo> SplitIntoPackages(List<LogEntry> entries, DetectionConfiguration config)
        {
            var packages = new List<PackageInfo>();

            if (entries == null || entries.Count == 0)
                return packages;

            // Get effective marker values (prefer manual, fall back to detected)
            byte[] startMarker = GetEffectiveMarker(config.PackageStartMarker);
            byte[] endMarker = GetEffectiveMarker(config.PackageEndMarker);

            // Handle different marker scenarios
            if (startMarker != null && endMarker != null)
            {
                // Case 1: Both start and end markers defined
                return SplitByStartAndEndMarkers(entries, startMarker, endMarker);
            }
            else if (startMarker != null)
            {
                // Case 2: Only start marker (end = next start)
                return SplitByStartMarkerOnly(entries, startMarker);
            }
            else if (endMarker != null)
            {
                // Case 3: Only end marker (start = previous end + 1)
                return SplitByEndMarkerOnly(entries, endMarker);
            }
            else
            {
                // Case 4: No markers - treat each entry as single package
                return SplitEachEntryAsPackage(entries);
            }
        }

        /// <summary>
        /// Splits a package into segments using segment separator
        /// </summary>
        /// <param name="packageBytes">Raw package bytes</param>
        /// <param name="config">Detection configuration with separator</param>
        /// <returns>List of parsed segments</returns>
        public List<SegmentInfo> SplitIntoSegments(byte[] packageBytes, DetectionConfiguration config)
        {
            var segments = new List<SegmentInfo>();

            if (packageBytes == null || packageBytes.Length == 0)
                return segments;

            // Get effective separator value
            byte[] separator = GetEffectiveMarker(config.SegmentSeparator);

            if (separator == null || separator.Length == 0)
            {
                // No separator - single segment (SinglePackage protocol)
                segments.Add(new SegmentInfo
                {
                    SegmentIndex = 0,
                    RawBytes = packageBytes
                });
                return segments;
            }

            // Split by separator (PackageBased protocol)
            return SplitBytesBySeparator(packageBytes, separator);
        }

        #region Private Helper Methods

        /// <summary>
        /// Gets effective marker value from DetectionModeInfo (manual or detected)
        /// </summary>
        private byte[] GetEffectiveMarker(DetectionModeInfo modeInfo)
        {
            if (modeInfo == null)
                return null;

            // Priority: Manual > Auto-detected > None
            if (modeInfo.Mode == DetectionMode.Manual && !string.IsNullOrEmpty(modeInfo.ManualValue))
            {
                return ParseHexString(modeInfo.ManualValue);
            }
            else if (modeInfo.Mode == DetectionMode.Auto && !string.IsNullOrEmpty(modeInfo.DetectedValue))
            {
                return ParseHexString(modeInfo.DetectedValue);
            }

            return null;
        }

        /// <summary>
        /// Parses hex string (e.g., "0D 0A" or "0D0A") to byte array
        /// </summary>
        private byte[] ParseHexString(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return null;

            // Remove spaces and convert to uppercase
            string cleanHex = hex.Replace(" ", "").Replace("-", "").ToUpper();

            // Must be even number of characters
            if (cleanHex.Length % 2 != 0)
                return null;

            var bytes = new List<byte>();
            for (int i = 0; i < cleanHex.Length; i += 2)
            {
                string hexByte = cleanHex.Substring(i, 2);
                if (!byte.TryParse(hexByte, System.Globalization.NumberStyles.HexNumber, null, out byte b))
                    return null;
                bytes.Add(b);
            }

            return bytes.ToArray();
        }

        /// <summary>
        /// Case 1: Split by both start and end markers
        /// </summary>
        private List<PackageInfo> SplitByStartAndEndMarkers(List<LogEntry> entries, byte[] startMarker, byte[] endMarker)
        {
            var packages = new List<PackageInfo>();
            var packageBytes = new List<byte>();
            bool insidePackage = false;
            int packageNumber = 1;

            foreach (var entry in entries)
            {
                for (int i = 0; i < entry.RawBytes.Length; i++)
                {
                    // Check for start marker
                    if (!insidePackage && MatchesPattern(entry.RawBytes, i, startMarker))
                    {
                        insidePackage = true;
                        packageBytes.Clear();
                    }

                    // Add byte if inside package
                    if (insidePackage)
                    {
                        packageBytes.Add(entry.RawBytes[i]);
                    }

                    // Check for end marker
                    if (insidePackage && MatchesPattern(entry.RawBytes, i, endMarker))
                    {
                        // Package complete
                        packages.Add(CreatePackageInfo(packageBytes.ToArray(), packageNumber++));
                        insidePackage = false;
                        packageBytes.Clear();
                    }
                }
            }

            // Handle incomplete package at end
            if (insidePackage && packageBytes.Count > 0)
            {
                packages.Add(CreatePackageInfo(packageBytes.ToArray(), packageNumber));
            }

            return packages;
        }

        /// <summary>
        /// Case 2: Split by start marker only (each package ends at next start)
        /// </summary>
        private List<PackageInfo> SplitByStartMarkerOnly(List<LogEntry> entries, byte[] startMarker)
        {
            var packages = new List<PackageInfo>();
            var packageBytes = new List<byte>();
            int packageNumber = 1;
            bool firstPackage = true;

            foreach (var entry in entries)
            {
                for (int i = 0; i < entry.RawBytes.Length; i++)
                {
                    // Check for start marker
                    if (MatchesPattern(entry.RawBytes, i, startMarker))
                    {
                        // Save previous package (except for first marker)
                        if (!firstPackage && packageBytes.Count > 0)
                        {
                            packages.Add(CreatePackageInfo(packageBytes.ToArray(), packageNumber++));
                            packageBytes.Clear();
                        }
                        firstPackage = false;
                    }

                    packageBytes.Add(entry.RawBytes[i]);
                }
            }

            // Add final package
            if (packageBytes.Count > 0)
            {
                packages.Add(CreatePackageInfo(packageBytes.ToArray(), packageNumber));
            }

            return packages;
        }

        /// <summary>
        /// Case 3: Split by end marker only (each package starts after previous end)
        /// </summary>
        private List<PackageInfo> SplitByEndMarkerOnly(List<LogEntry> entries, byte[] endMarker)
        {
            var packages = new List<PackageInfo>();
            var packageBytes = new List<byte>();
            int packageNumber = 1;

            foreach (var entry in entries)
            {
                for (int i = 0; i < entry.RawBytes.Length; i++)
                {
                    packageBytes.Add(entry.RawBytes[i]);

                    // Check for end marker
                    if (MatchesPattern(entry.RawBytes, i, endMarker))
                    {
                        // Package complete
                        packages.Add(CreatePackageInfo(packageBytes.ToArray(), packageNumber++));
                        packageBytes.Clear();
                    }
                }
            }

            // Handle incomplete package at end
            if (packageBytes.Count > 0)
            {
                packages.Add(CreatePackageInfo(packageBytes.ToArray(), packageNumber));
            }

            return packages;
        }

        /// <summary>
        /// Case 4: No markers - each entry is a separate package
        /// </summary>
        private List<PackageInfo> SplitEachEntryAsPackage(List<LogEntry> entries)
        {
            var packages = new List<PackageInfo>();
            int packageNumber = 1;

            foreach (var entry in entries)
            {
                if (entry.RawBytes.Length > 0)
                {
                    packages.Add(CreatePackageInfo(entry.RawBytes, packageNumber++));
                }
            }

            return packages;
        }

        /// <summary>
        /// Splits byte array by separator into segments
        /// </summary>
        private List<SegmentInfo> SplitBytesBySeparator(byte[] data, byte[] separator)
        {
            var segments = new List<SegmentInfo>();
            var currentSegment = new List<byte>();
            int segmentIndex = 0;

            for (int i = 0; i < data.Length; i++)
            {
                // Check if separator pattern matches
                if (MatchesPattern(data, i, separator))
                {
                    // Segment complete (exclude separator from segment)
                    if (currentSegment.Count > 0)
                    {
                        segments.Add(new SegmentInfo
                        {
                            SegmentIndex = segmentIndex++,
                            RawBytes = currentSegment.ToArray()
                        });
                        currentSegment.Clear();
                    }

                    // Skip separator bytes
                    i += separator.Length - 1;
                }
                else
                {
                    currentSegment.Add(data[i]);
                }
            }

            // Add final segment
            if (currentSegment.Count > 0)
            {
                segments.Add(new SegmentInfo
                {
                    SegmentIndex = segmentIndex,
                    RawBytes = currentSegment.ToArray()
                });
            }

            return segments;
        }

        /// <summary>
        /// Checks if pattern matches at given position
        /// </summary>
        private bool MatchesPattern(byte[] data, int position, byte[] pattern)
        {
            if (pattern == null || pattern.Length == 0)
                return false;

            if (position + pattern.Length > data.Length)
                return false;

            for (int i = 0; i < pattern.Length; i++)
            {
                if (data[position + i] != pattern[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Creates PackageInfo object from raw bytes
        /// </summary>
        private PackageInfo CreatePackageInfo(byte[] bytes, int packageNumber)
        {
            return new PackageInfo
            {
                PackageNumber = packageNumber,
                RawBytes = bytes
            };
        }

        #endregion
    }
}
