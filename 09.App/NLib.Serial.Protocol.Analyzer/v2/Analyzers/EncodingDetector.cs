#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Analyzers
{
    /// <summary>
    /// Detects text encoding from raw byte data.
    /// Uses BOM detection, byte pattern analysis, and heuristics.
    /// Based on Document 02: System Architecture - TODO-003.
    /// </summary>
    public class EncodingDetector
    {
        #region Public Methods

        /// <summary>
        /// Detects the most likely encoding from raw byte data.
        /// Returns encoding with confidence score.
        /// </summary>
        /// <param name="data">Raw byte data to analyze.</param>
        /// <returns>Detected encoding information with confidence score.</returns>
        public EncodingInfo DetectEncoding(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return new EncodingInfo
                {
                    Encoding = Encoding.ASCII,
                    EncodingName = "ASCII",
                    Confidence = 0.0,
                    Reason = "No data provided"
                };
            }

            // Priority 1: Check for BOM (Byte Order Mark)
            var bomResult = DetectByBOM(data);
            if (bomResult.Confidence >= 1.0)
            {
                return bomResult;
            }

            // Priority 2: Analyze byte patterns
            var patternResults = new List<EncodingInfo>
            {
                AnalyzeAsASCII(data),
                AnalyzeAsUTF8(data),
                AnalyzeAsUTF16LE(data),
                AnalyzeAsUTF16BE(data)
            };

            // Return highest confidence result
            var bestResult = patternResults.OrderByDescending(r => r.Confidence).First();

            // Default to ASCII if confidence is too low
            if (bestResult.Confidence < 0.5)
            {
                return new EncodingInfo
                {
                    Encoding = Encoding.ASCII,
                    EncodingName = "ASCII",
                    Confidence = 0.7,
                    Reason = "Low confidence in other encodings, defaulting to ASCII"
                };
            }

            return bestResult;
        }

        #endregion

        #region BOM Detection

        /// <summary>
        /// Detects encoding by Byte Order Mark (BOM).
        /// BOM provides 100% confidence when present.
        /// </summary>
        private EncodingInfo DetectByBOM(byte[] data)
        {
            // UTF-8 BOM: EF BB BF
            if (data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF)
            {
                return new EncodingInfo
                {
                    Encoding = Encoding.UTF8,
                    EncodingName = "UTF-8",
                    Confidence = 1.0,
                    Reason = "UTF-8 BOM detected (EF BB BF)"
                };
            }

            // UTF-16 LE BOM: FF FE
            if (data.Length >= 2 && data[0] == 0xFF && data[1] == 0xFE)
            {
                return new EncodingInfo
                {
                    Encoding = Encoding.Unicode, // UTF-16 LE
                    EncodingName = "UTF-16LE",
                    Confidence = 1.0,
                    Reason = "UTF-16 LE BOM detected (FF FE)"
                };
            }

            // UTF-16 BE BOM: FE FF
            if (data.Length >= 2 && data[0] == 0xFE && data[1] == 0xFF)
            {
                return new EncodingInfo
                {
                    Encoding = Encoding.BigEndianUnicode, // UTF-16 BE
                    EncodingName = "UTF-16BE",
                    Confidence = 1.0,
                    Reason = "UTF-16 BE BOM detected (FE FF)"
                };
            }

            // UTF-32 LE BOM: FF FE 00 00
            if (data.Length >= 4 && data[0] == 0xFF && data[1] == 0xFE && data[2] == 0x00 && data[3] == 0x00)
            {
                return new EncodingInfo
                {
                    Encoding = Encoding.UTF32,
                    EncodingName = "UTF-32LE",
                    Confidence = 1.0,
                    Reason = "UTF-32 LE BOM detected (FF FE 00 00)"
                };
            }

            // UTF-32 BE BOM: 00 00 FE FF
            if (data.Length >= 4 && data[0] == 0x00 && data[1] == 0x00 && data[2] == 0xFE && data[3] == 0xFF)
            {
                return new EncodingInfo
                {
                    Encoding = new UTF32Encoding(bigEndian: true, byteOrderMark: true),
                    EncodingName = "UTF-32BE",
                    Confidence = 1.0,
                    Reason = "UTF-32 BE BOM detected (00 00 FE FF)"
                };
            }

            // No BOM found
            return new EncodingInfo
            {
                Encoding = null,
                EncodingName = "Unknown",
                Confidence = 0.0,
                Reason = "No BOM detected"
            };
        }

        #endregion

        #region Pattern Analysis

        /// <summary>
        /// Analyzes data as ASCII encoding.
        /// ASCII: All bytes should be 0x00-0x7F.
        /// </summary>
        private EncodingInfo AnalyzeAsASCII(byte[] data)
        {
            int validCount = 0;
            int printableCount = 0;
            int controlCount = 0;

            foreach (byte b in data)
            {
                if (b <= 0x7F)
                {
                    validCount++;

                    // Printable ASCII (0x20-0x7E)
                    if (b >= 0x20 && b <= 0x7E)
                    {
                        printableCount++;
                    }
                    // Common control characters (CR, LF, TAB, etc.)
                    else if (b == 0x0D || b == 0x0A || b == 0x09 || b == 0x00)
                    {
                        controlCount++;
                    }
                }
            }

            double validRatio = (double)validCount / data.Length;
            double printableRatio = (double)printableCount / data.Length;

            // High confidence if all bytes are valid ASCII
            if (validRatio >= 0.98)
            {
                return new EncodingInfo
                {
                    Encoding = Encoding.ASCII,
                    EncodingName = "ASCII",
                    Confidence = 0.95,
                    Reason = $"98%+ bytes are valid ASCII (printable: {printableRatio:P1})"
                };
            }

            // Medium confidence if mostly ASCII
            if (validRatio >= 0.80)
            {
                return new EncodingInfo
                {
                    Encoding = Encoding.ASCII,
                    EncodingName = "ASCII",
                    Confidence = 0.70,
                    Reason = $"80%+ bytes are valid ASCII (printable: {printableRatio:P1})"
                };
            }

            // Low confidence
            return new EncodingInfo
            {
                Encoding = Encoding.ASCII,
                EncodingName = "ASCII",
                Confidence = 0.30,
                Reason = $"Only {validRatio:P1} bytes are valid ASCII"
            };
        }

        /// <summary>
        /// Analyzes data as UTF-8 encoding.
        /// UTF-8: Multi-byte sequences must follow specific patterns.
        /// </summary>
        private EncodingInfo AnalyzeAsUTF8(byte[] data)
        {
            int i = 0;
            int validSequences = 0;
            int invalidSequences = 0;
            int totalBytes = data.Length;

            while (i < data.Length)
            {
                byte b = data[i];

                // Single-byte (ASCII): 0xxxxxxx
                if ((b & 0x80) == 0)
                {
                    validSequences++;
                    i++;
                    continue;
                }

                // Multi-byte sequence start: 110xxxxx, 1110xxxx, 11110xxx
                int sequenceLength = 0;

                if ((b & 0xE0) == 0xC0) // 2-byte: 110xxxxx 10xxxxxx
                {
                    sequenceLength = 2;
                }
                else if ((b & 0xF0) == 0xE0) // 3-byte: 1110xxxx 10xxxxxx 10xxxxxx
                {
                    sequenceLength = 3;
                }
                else if ((b & 0xF8) == 0xF0) // 4-byte: 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
                {
                    sequenceLength = 4;
                }
                else
                {
                    // Invalid UTF-8 start byte
                    invalidSequences++;
                    i++;
                    continue;
                }

                // Validate continuation bytes
                bool validSequence = true;
                for (int j = 1; j < sequenceLength; j++)
                {
                    if (i + j >= data.Length || (data[i + j] & 0xC0) != 0x80)
                    {
                        validSequence = false;
                        break;
                    }
                }

                if (validSequence)
                {
                    validSequences++;
                    i += sequenceLength;
                }
                else
                {
                    invalidSequences++;
                    i++;
                }
            }

            int totalSequences = validSequences + invalidSequences;
            double validRatio = totalSequences > 0 ? (double)validSequences / totalSequences : 0;

            // High confidence if all sequences are valid
            if (validRatio >= 0.98 && invalidSequences == 0)
            {
                return new EncodingInfo
                {
                    Encoding = Encoding.UTF8,
                    EncodingName = "UTF-8",
                    Confidence = 0.95,
                    Reason = $"All byte sequences are valid UTF-8 ({validSequences} sequences)"
                };
            }

            // Medium confidence
            if (validRatio >= 0.90)
            {
                return new EncodingInfo
                {
                    Encoding = Encoding.UTF8,
                    EncodingName = "UTF-8",
                    Confidence = 0.80,
                    Reason = $"90%+ byte sequences are valid UTF-8 ({validSequences}/{totalSequences})"
                };
            }

            // Low confidence
            return new EncodingInfo
            {
                Encoding = Encoding.UTF8,
                EncodingName = "UTF-8",
                Confidence = 0.20,
                Reason = $"Only {validRatio:P1} sequences are valid UTF-8"
            };
        }

        /// <summary>
        /// Analyzes data as UTF-16 Little Endian.
        /// UTF-16 LE: Every other byte tends to be 0x00 for ASCII-range characters.
        /// </summary>
        private EncodingInfo AnalyzeAsUTF16LE(byte[] data)
        {
            if (data.Length < 2 || data.Length % 2 != 0)
            {
                return new EncodingInfo
                {
                    Encoding = Encoding.Unicode,
                    EncodingName = "UTF-16LE",
                    Confidence = 0.0,
                    Reason = "Data length not suitable for UTF-16"
                };
            }

            int nullHighBytes = 0; // Count of high bytes that are 0x00
            int totalChars = data.Length / 2;

            for (int i = 0; i < data.Length; i += 2)
            {
                byte lowByte = data[i];
                byte highByte = data[i + 1];

                // For ASCII-range characters in UTF-16 LE: low byte = ASCII, high byte = 0x00
                if (highByte == 0x00 && lowByte <= 0x7F)
                {
                    nullHighBytes++;
                }
            }

            double nullHighRatio = (double)nullHighBytes / totalChars;

            // High confidence if most chars have null high byte (ASCII range)
            if (nullHighRatio >= 0.80)
            {
                return new EncodingInfo
                {
                    Encoding = Encoding.Unicode,
                    EncodingName = "UTF-16LE",
                    Confidence = 0.85,
                    Reason = $"80%+ characters have null high byte (typical for UTF-16 LE ASCII)"
                };
            }

            // Low confidence
            return new EncodingInfo
            {
                Encoding = Encoding.Unicode,
                EncodingName = "UTF-16LE",
                Confidence = 0.20,
                Reason = $"Only {nullHighRatio:P1} characters match UTF-16 LE pattern"
            };
        }

        /// <summary>
        /// Analyzes data as UTF-16 Big Endian.
        /// UTF-16 BE: Every other byte tends to be 0x00 for ASCII-range characters (reversed from LE).
        /// </summary>
        private EncodingInfo AnalyzeAsUTF16BE(byte[] data)
        {
            if (data.Length < 2 || data.Length % 2 != 0)
            {
                return new EncodingInfo
                {
                    Encoding = Encoding.BigEndianUnicode,
                    EncodingName = "UTF-16BE",
                    Confidence = 0.0,
                    Reason = "Data length not suitable for UTF-16"
                };
            }

            int nullHighBytes = 0; // Count of high bytes that are 0x00
            int totalChars = data.Length / 2;

            for (int i = 0; i < data.Length; i += 2)
            {
                byte highByte = data[i];
                byte lowByte = data[i + 1];

                // For ASCII-range characters in UTF-16 BE: high byte = 0x00, low byte = ASCII
                if (highByte == 0x00 && lowByte <= 0x7F)
                {
                    nullHighBytes++;
                }
            }

            double nullHighRatio = (double)nullHighBytes / totalChars;

            // High confidence if most chars have null high byte (ASCII range)
            if (nullHighRatio >= 0.80)
            {
                return new EncodingInfo
                {
                    Encoding = Encoding.BigEndianUnicode,
                    EncodingName = "UTF-16BE",
                    Confidence = 0.85,
                    Reason = $"80%+ characters have null high byte (typical for UTF-16 BE ASCII)"
                };
            }

            // Low confidence
            return new EncodingInfo
            {
                Encoding = Encoding.BigEndianUnicode,
                EncodingName = "UTF-16BE",
                Confidence = 0.20,
                Reason = $"Only {nullHighRatio:P1} characters match UTF-16 BE pattern"
            };
        }

        #endregion
    }

    #region EncodingInfo Class

    /// <summary>
    /// Result of encoding detection with confidence score.
    /// </summary>
    public class EncodingInfo
    {
        /// <summary>
        /// Detected encoding.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Encoding name (ASCII, UTF-8, UTF-16LE, etc.).
        /// </summary>
        public string EncodingName { get; set; }

        /// <summary>
        /// Confidence score (0.0 - 1.0).
        /// 1.0 = BOM detected (100% confident)
        /// 0.95+ = High confidence from pattern analysis
        /// 0.70-0.94 = Medium confidence
        /// &lt; 0.70 = Low confidence
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Reason for detection (for logging/debugging).
        /// </summary>
        public string Reason { get; set; }
    }

    #endregion
}
