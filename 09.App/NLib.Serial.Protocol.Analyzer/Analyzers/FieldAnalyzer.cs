using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLib.Serial.Protocol.Analyzer.Models;

namespace NLib.Serial.Protocol.Analyzer.Analyzers
{
    /// <summary>
    /// Analyzes log data to extract fields using 5-stage pipeline
    /// Executes statistical analysis and field extraction based on detection configuration
    /// </summary>
    public class FieldAnalyzer
    {
        /// <summary>
        /// Executes full 5-stage analysis pipeline
        /// </summary>
        /// <param name="logFile">Log file with entries</param>
        /// <param name="config">Detection configuration from Page 1</param>
        /// <returns>Analysis result with detected fields</returns>
        public AnalysisResult RunFullAnalysis(LogFile logFile, DetectionConfiguration config)
        {
            if (logFile == null || logFile.Entries == null || logFile.Entries.Count == 0)
                throw new ArgumentException("Log file is empty or null");

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var result = new AnalysisResult();

            // Stage 1: Byte Extraction (already done - we have log entries)
            result.TotalEntries = logFile.Entries.Count;

            // Stage 2: Package Boundary Detection
            var packages = DetectPackageBoundaries(logFile.Entries.ToList(), config);
            result.TotalPackages = packages.Count;

            // Stage 3: Field Structure Analysis
            var fieldBoundaries = AnalyzeFieldStructure(packages, config);

            // Stage 4: Field Classification
            var fields = ClassifyFields(packages, fieldBoundaries);
            result.DetectedFields = fields;

            // Stage 5: Relationship Detection
            DetectFieldRelationships(fields);

            // Calculate overall confidence
            result.OverallConfidence = CalculateOverallConfidence(packages, fields);

            // Store detection summary
            result.DetectionSummary = CreateDetectionSummary(config, packages, fields);

            return result;
        }

        #region Stage 2: Package Boundary Detection

        /// <summary>
        /// Stage 2: Detect package boundaries using configuration markers/terminators
        /// </summary>
        private List<PackageData> DetectPackageBoundaries(List<LogEntry> entries, DetectionConfiguration config)
        {
            var packages = new List<PackageData>();

            // Get active terminator/marker from configuration
            byte[] packageTerminator = GetActiveTerminator(config);

            if (packageTerminator == null || packageTerminator.Length == 0)
            {
                // No terminator - treat each entry as a package
                for (int i = 0; i < entries.Count; i++)
                {
                    packages.Add(new PackageData
                    {
                        PackageNumber = i + 1,
                        RawBytes = entries[i].RawBytes,
                        SourceEntryNumber = entries[i].EntryNumber
                    });
                }
            }
            else
            {
                // Split entries by terminator
                // For simplicity, assuming one package per entry for now
                // (More complex splitting can be added later)
                for (int i = 0; i < entries.Count; i++)
                {
                    packages.Add(new PackageData
                    {
                        PackageNumber = i + 1,
                        RawBytes = entries[i].RawBytes,
                        SourceEntryNumber = entries[i].EntryNumber
                    });
                }
            }

            return packages;
        }

        /// <summary>
        /// Get the active package terminator from configuration
        /// </summary>
        private byte[] GetActiveTerminator(DetectionConfiguration config)
        {
            // Check for end marker first (most common for terminators)
            if (config.PackageEndMarker != null && !string.IsNullOrWhiteSpace(config.PackageEndMarker.EffectiveValue))
            {
                return ParseHexString(config.PackageEndMarker.EffectiveValue);
            }

            // Check for start marker (less common for terminators)
            if (config.PackageStartMarker != null && !string.IsNullOrWhiteSpace(config.PackageStartMarker.EffectiveValue))
            {
                return ParseHexString(config.PackageStartMarker.EffectiveValue);
            }

            return null;
        }

        #endregion

        #region Stage 3: Field Structure Analysis

        /// <summary>
        /// Stage 3: Analyze field structure (delimiter-based or position-based)
        /// </summary>
        private List<FieldBoundary> AnalyzeFieldStructure(List<PackageData> packages, DetectionConfiguration config)
        {
            if (packages.Count == 0)
                return new List<FieldBoundary>();

            // Get segment separator from configuration
            byte[] separator = GetActiveSeparator(config);

            if (separator != null && separator.Length > 0)
            {
                // Delimiter-based parsing
                return AnalyzeDelimiterBased(packages, separator);
            }
            else
            {
                // Position-based parsing (analyze fixed positions)
                return AnalyzePositionBased(packages);
            }
        }

        /// <summary>
        /// Get the active segment separator from configuration
        /// </summary>
        private byte[] GetActiveSeparator(DetectionConfiguration config)
        {
            if (config.SegmentSeparator != null && !string.IsNullOrWhiteSpace(config.SegmentSeparator.EffectiveValue))
            {
                return ParseHexString(config.SegmentSeparator.EffectiveValue);
            }
            return null;
        }

        /// <summary>
        /// Analyze delimiter-based field structure
        /// </summary>
        private List<FieldBoundary> AnalyzeDelimiterBased(List<PackageData> packages, byte[] separator)
        {
            var fieldBoundaries = new List<FieldBoundary>();

            // Use first package as template to find field count
            var firstPackage = packages[0];
            var segments = SplitByDelimiter(firstPackage.RawBytes, separator);

            // Create field boundary for each segment position
            for (int i = 0; i < segments.Count; i++)
            {
                fieldBoundaries.Add(new FieldBoundary
                {
                    FieldIndex = i,
                    IsDelimiterBased = true,
                    Delimiter = separator
                });
            }

            return fieldBoundaries;
        }

        /// <summary>
        /// Analyze position-based field structure (no delimiter)
        /// </summary>
        private List<FieldBoundary> AnalyzePositionBased(List<PackageData> packages)
        {
            // For position-based, we analyze the entire package as one field
            // (More sophisticated position analysis can be added later)
            return new List<FieldBoundary>
            {
                new FieldBoundary
                {
                    FieldIndex = 0,
                    IsDelimiterBased = false,
                    StartPosition = 0,
                    EndPosition = packages[0].RawBytes.Length
                }
            };
        }

        /// <summary>
        /// Split bytes by delimiter
        /// </summary>
        private List<byte[]> SplitByDelimiter(byte[] data, byte[] delimiter)
        {
            var segments = new List<byte[]>();
            int start = 0;

            for (int i = 0; i <= data.Length - delimiter.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < delimiter.Length; j++)
                {
                    if (data[i + j] != delimiter[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    // Found delimiter - extract segment
                    int length = i - start;
                    if (length > 0)
                    {
                        byte[] segment = new byte[length];
                        Array.Copy(data, start, segment, 0, length);
                        segments.Add(segment);
                    }
                    start = i + delimiter.Length;
                    i += delimiter.Length - 1; // Skip delimiter
                }
            }

            // Add final segment
            if (start < data.Length)
            {
                int length = data.Length - start;
                byte[] segment = new byte[length];
                Array.Copy(data, start, segment, 0, length);
                segments.Add(segment);
            }

            return segments;
        }

        #endregion

        #region Stage 4: Field Classification

        /// <summary>
        /// Stage 4: Classify fields by data type
        /// </summary>
        private List<FieldInfo> ClassifyFields(List<PackageData> packages, List<FieldBoundary> boundaries)
        {
            var fields = new List<FieldInfo>();

            for (int i = 0; i < boundaries.Count; i++)
            {
                var boundary = boundaries[i];
                var field = new FieldInfo
                {
                    Position = i,
                    Name = $"Field{i}",
                    AutoGeneratedName = $"Field{i}"
                };

                // Extract sample values
                var samples = ExtractFieldSamples(packages, boundary, maxSamples: 10);
                field.SampleValues = samples;

                // Detect data type (returns DataType enum)
                field.DataType = DetectDataTypeEnum(samples);

                // Calculate variance (how much the field changes)
                field.Variance = CalculateFieldVariance(samples);

                // Calculate confidence (how consistent the detection is)
                field.DetectionConfidence = CalculateFieldConfidence(samples, field.DataType);

                fields.Add(field);
            }

            return fields;
        }

        /// <summary>
        /// Extract sample values for a field from packages (returns raw bytes)
        /// </summary>
        private List<byte[]> ExtractFieldSamples(List<PackageData> packages, FieldBoundary boundary, int maxSamples)
        {
            var samples = new List<byte[]>();
            int count = Math.Min(packages.Count, maxSamples);

            for (int i = 0; i < count; i++)
            {
                byte[] fieldData;

                if (boundary.IsDelimiterBased)
                {
                    // Extract by splitting with delimiter
                    var segments = SplitByDelimiter(packages[i].RawBytes, boundary.Delimiter);
                    if (boundary.FieldIndex < segments.Count)
                    {
                        fieldData = segments[boundary.FieldIndex];
                    }
                    else
                    {
                        continue; // Skip if field doesn't exist in this package
                    }
                }
                else
                {
                    // Extract by position
                    int length = boundary.EndPosition - boundary.StartPosition;
                    if (boundary.StartPosition + length <= packages[i].RawBytes.Length)
                    {
                        fieldData = new byte[length];
                        Array.Copy(packages[i].RawBytes, boundary.StartPosition, fieldData, 0, length);
                    }
                    else
                    {
                        continue; // Skip if position is out of bounds
                    }
                }

                // Store raw bytes (NO string conversion)
                // Bytes are the source of truth - follow RULE #1
                if (fieldData != null && fieldData.Length > 0)
                {
                    samples.Add(fieldData);
                }
            }

            return samples;
        }

        /// <summary>
        /// Detect data type from sample values using byte-level pattern analysis
        /// NO string conversion - works with raw bytes following RULE #1
        /// </summary>
        private DataType DetectDataTypeEnum(List<byte[]> samples)
        {
            if (samples.Count == 0)
                return DataType.String;

            int intCount = 0;
            int decimalCount = 0;
            int dateCount = 0;
            int timeCount = 0;

            foreach (var sample in samples)
            {
                if (IsNumericBytes(sample))
                    intCount++;
                else if (IsDecimalBytes(sample))
                    decimalCount++;
                else if (IsDateBytes(sample))
                    dateCount++;
                else if (IsTimeBytes(sample))
                    timeCount++;
            }

            double intRatio = (double)intCount / samples.Count;
            double decimalRatio = (double)decimalCount / samples.Count;
            double dateRatio = (double)dateCount / samples.Count;
            double timeRatio = (double)timeCount / samples.Count;

            // Prioritize most specific type first
            if (dateRatio > 0.8 || timeRatio > 0.8)
                return DataType.DateTime;
            else if (decimalRatio > 0.8)
                return DataType.Float;
            else if (intRatio > 0.8)
                return DataType.Integer;
            else
                return DataType.String;
        }

        /// <summary>
        /// Calculate field variance (0 = constant, 1 = highly variable)
        /// Works with byte arrays using byte-level comparison
        /// </summary>
        private double CalculateFieldVariance(List<byte[]> samples)
        {
            if (samples.Count == 0)
                return 0;

            // Count unique byte sequences
            var uniqueValues = new List<byte[]>();

            foreach (var sample in samples)
            {
                bool isDuplicate = false;
                foreach (var existing in uniqueValues)
                {
                    if (ByteArraysEqual(sample, existing))
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    uniqueValues.Add(sample);
                }
            }

            return (double)uniqueValues.Count / samples.Count;
        }

        /// <summary>
        /// Calculate field detection confidence using byte-level pattern matching
        /// </summary>
        private double CalculateFieldConfidence(List<byte[]> samples, DataType dataType)
        {
            if (samples.Count == 0)
                return 0;

            // Base confidence on consistency of detected type
            int matchCount = 0;

            foreach (var sample in samples)
            {
                bool matches = false;

                switch (dataType)
                {
                    case DataType.Integer:
                        matches = IsNumericBytes(sample);
                        break;
                    case DataType.Float:
                        matches = IsDecimalBytes(sample);
                        break;
                    case DataType.DateTime:
                        matches = IsDateBytes(sample) || IsTimeBytes(sample);
                        break;
                    case DataType.String:
                    case DataType.Hex:
                    case DataType.Binary:
                        matches = true; // All byte sequences can be treated as string/hex/binary
                        break;
                }

                if (matches)
                    matchCount++;
            }

            return (double)matchCount / samples.Count * 100;
        }

        #endregion

        #region Stage 5: Relationship Detection

        /// <summary>
        /// Stage 5: Detect relationships between fields
        /// </summary>
        private void DetectFieldRelationships(List<FieldInfo> fields)
        {
            // Look for date+time combinations, compound fields, etc.
            // Simplified implementation for now
            for (int i = 0; i < fields.Count - 1; i++)
            {
                if (fields[i].DataType == DataType.DateTime && fields[i + 1].DataType == DataType.DateTime)
                {
                    // Potential date+time combination
                    fields[i].Relationships = new List<string> { $"Possible date-time pair with {fields[i + 1].Name}" };
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Calculate overall confidence score
        /// </summary>
        private double CalculateOverallConfidence(List<PackageData> packages, List<FieldInfo> fields)
        {
            if (fields.Count == 0)
                return 0;

            double avgConfidence = fields.Average(f => f.DetectionConfidence);
            return avgConfidence;
        }

        /// <summary>
        /// Create detection summary for display
        /// </summary>
        private DetectionSummary CreateDetectionSummary(DetectionConfiguration config, List<PackageData> packages, List<FieldInfo> fields)
        {
            var summary = new DetectionSummary();

            // Terminator info
            summary.PackageTerminator = config.PackageEndMarker?.EffectiveValue ?? "None";
            summary.PackageTerminatorOccurrences = packages.Count;

            // Delimiter info
            summary.SegmentDelimiter = config.SegmentSeparator?.EffectiveValue ?? "None";

            // Protocol type
            bool hasSeparator = !string.IsNullOrWhiteSpace(config.SegmentSeparator?.EffectiveValue);
            bool hasStartMarker = !string.IsNullOrWhiteSpace(config.PackageStartMarker?.EffectiveValue);
            bool hasEndMarker = !string.IsNullOrWhiteSpace(config.PackageEndMarker?.EffectiveValue);

            if (hasStartMarker && hasSeparator)
                summary.ProtocolType = "PackageBased with Segments";
            else if (hasStartMarker)
                summary.ProtocolType = "PackageBased without Segments";
            else if (hasEndMarker && hasSeparator)
                summary.ProtocolType = "SinglePackage with Segments";
            else if (hasEndMarker)
                summary.ProtocolType = "SinglePackage without Segments";
            else
                summary.ProtocolType = "Unknown";

            summary.FieldCount = fields.Count;

            return summary;
        }

        /// <summary>
        /// Parse hex string to byte array (e.g., "0D 0A" or "0D-0A")
        /// </summary>
        private byte[] ParseHexString(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return null;

            // Remove separators and whitespace
            string cleaned = hex.Replace(" ", "").Replace("-", "").Replace("0x", "");

            if (cleaned.Length % 2 != 0)
                return null; // Invalid hex string

            byte[] bytes = new byte[cleaned.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(cleaned.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        /// <summary>
        /// Check if byte array contains only numeric digits (0x30-0x39)
        /// </summary>
        private bool IsNumericBytes(byte[] data)
        {
            if (data == null || data.Length == 0)
                return false;

            // Trim whitespace bytes (0x20, 0x09, 0x0D, 0x0A)
            int start = 0;
            int end = data.Length - 1;

            while (start < data.Length && IsWhitespaceByte(data[start]))
                start++;

            while (end >= 0 && IsWhitespaceByte(data[end]))
                end--;

            if (start > end)
                return false; // All whitespace

            // All remaining bytes must be digits 0x30-0x39
            for (int i = start; i <= end; i++)
            {
                if (data[i] < 0x30 || data[i] > 0x39)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check if byte array is a decimal number (digits + 0x2E + digits)
        /// </summary>
        private bool IsDecimalBytes(byte[] data)
        {
            if (data == null || data.Length == 0)
                return false;

            // Trim whitespace
            int start = 0;
            int end = data.Length - 1;

            while (start < data.Length && IsWhitespaceByte(data[start]))
                start++;

            while (end >= 0 && IsWhitespaceByte(data[end]))
                end--;

            if (start > end)
                return false;

            // Find decimal point (0x2E)
            int dotIndex = -1;
            for (int i = start; i <= end; i++)
            {
                if (data[i] == 0x2E) // '.'
                {
                    dotIndex = i;
                    break;
                }
            }

            if (dotIndex == -1)
                return false; // No decimal point

            // Must have at least one digit before dot
            if (dotIndex == start)
                return false;

            // Must have at least one digit after dot
            if (dotIndex == end)
                return false;

            // Check before dot: all digits (allow optional + or -)
            for (int i = start; i < dotIndex; i++)
            {
                if (i == start && (data[i] == 0x2B || data[i] == 0x2D)) // '+' or '-'
                    continue;

                if (data[i] < 0x30 || data[i] > 0x39)
                    return false;
            }

            // Check after dot: all digits
            for (int i = dotIndex + 1; i <= end; i++)
            {
                if (data[i] < 0x30 || data[i] > 0x39)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check if byte array matches date pattern (YYYY-MM-DD, DD/MM/YYYY, etc.)
        /// </summary>
        private bool IsDateBytes(byte[] data)
        {
            if (data == null || data.Length < 8) // Minimum date length
                return false;

            // Trim whitespace
            int start = 0;
            int end = data.Length - 1;

            while (start < data.Length && IsWhitespaceByte(data[start]))
                start++;

            while (end >= 0 && IsWhitespaceByte(data[end]))
                end--;

            int length = end - start + 1;
            if (length < 8 || length > 10)
                return false;

            // Pattern: YYYY-MM-DD (10 chars) or DD/MM/YYYY (10 chars)
            // Check for separators at positions 2 and 5, or 4 and 7
            byte sep1 = 0, sep2 = 0;

            if (length == 10)
            {
                // Could be YYYY-MM-DD or DD/MM/YYYY
                if ((data[start + 4] == 0x2D || data[start + 4] == 0x2F) &&  // '-' or '/'
                    (data[start + 7] == 0x2D || data[start + 7] == 0x2F))
                {
                    // YYYY-MM-DD format
                    return IsDigitSequence(data, start, 4) &&
                           IsDigitSequence(data, start + 5, 2) &&
                           IsDigitSequence(data, start + 8, 2);
                }
                else if ((data[start + 2] == 0x2D || data[start + 2] == 0x2F) &&
                         (data[start + 5] == 0x2D || data[start + 5] == 0x2F))
                {
                    // DD/MM/YYYY format
                    return IsDigitSequence(data, start, 2) &&
                           IsDigitSequence(data, start + 3, 2) &&
                           IsDigitSequence(data, start + 6, 4);
                }
            }

            return false;
        }

        /// <summary>
        /// Check if byte array matches time pattern (HH:MM:SS or HH:MM)
        /// </summary>
        private bool IsTimeBytes(byte[] data)
        {
            if (data == null || data.Length < 5) // Minimum HH:MM
                return false;

            // Trim whitespace
            int start = 0;
            int end = data.Length - 1;

            while (start < data.Length && IsWhitespaceByte(data[start]))
                start++;

            while (end >= 0 && IsWhitespaceByte(data[end]))
                end--;

            int length = end - start + 1;
            if (length != 5 && length != 8) // HH:MM or HH:MM:SS
                return false;

            // Check for colon separators
            if (data[start + 2] != 0x3A) // ':'
                return false;

            // HH:MM format
            if (length == 5)
            {
                return IsDigitSequence(data, start, 2) &&
                       IsDigitSequence(data, start + 3, 2);
            }

            // HH:MM:SS format
            if (length == 8)
            {
                if (data[start + 5] != 0x3A) // ':'
                    return false;

                return IsDigitSequence(data, start, 2) &&
                       IsDigitSequence(data, start + 3, 2) &&
                       IsDigitSequence(data, start + 6, 2);
            }

            return false;
        }

        /// <summary>
        /// Check if specified range contains only digit bytes
        /// </summary>
        private bool IsDigitSequence(byte[] data, int start, int length)
        {
            if (start + length > data.Length)
                return false;

            for (int i = start; i < start + length; i++)
            {
                if (data[i] < 0x30 || data[i] > 0x39)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check if byte is whitespace (space, tab, CR, LF)
        /// </summary>
        private bool IsWhitespaceByte(byte b)
        {
            return b == 0x20 || b == 0x09 || b == 0x0D || b == 0x0A;
        }

        /// <summary>
        /// Compare two byte arrays for equality
        /// </summary>
        private bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
                return true;

            if (a == null || b == null)
                return false;

            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }

        #endregion
    }

    #region Helper Classes

    /// <summary>
    /// Represents a detected package
    /// </summary>
    internal class PackageData
    {
        public int PackageNumber { get; set; }
        public byte[] RawBytes { get; set; }
        public int SourceEntryNumber { get; set; }
    }

    /// <summary>
    /// Represents field boundary information
    /// </summary>
    internal class FieldBoundary
    {
        public int FieldIndex { get; set; }
        public bool IsDelimiterBased { get; set; }
        public byte[] Delimiter { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }
    }

    #endregion
}
