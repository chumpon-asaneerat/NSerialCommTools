#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLib.Serial.ProtocolAnalyzer.Models;
using NLib.Serial.ProtocolAnalyzer.Utilities;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Parsers
{
    /// <summary>
    /// Extracts individual messages from raw byte stream following Algorithm 1 from
    /// 03-Parsing-Strategy-Analysis.md
    /// </summary>
    public class MessageExtractor
    {
        #region Public Methods

        /// <summary>
        /// Extracts messages by detecting frame markers and analyzing gaps.
        /// Algorithm 1: Message Boundary Detection
        /// BINARY-SAFE: Uses byte[] splitting with optional terminator.
        /// </summary>
        /// <param name="rawBytes">Raw byte data from log file.</param>
        /// <param name="terminator">Optional detected terminator (if null, will detect on-the-fly).</param>
        /// <param name="encoding">Optional encoding (defaults to ASCII).</param>
        /// <returns>LogData with extracted messages.</returns>
        public LogData ExtractMessages(byte[] rawBytes, TerminatorInfo terminator = null, Encoding encoding = null)
        {
            if (rawBytes == null || rawBytes.Length == 0)
                return new LogData();

            // Use provided encoding or default to ASCII
            Encoding textEncoding = encoding ?? Encoding.ASCII;

            // Get terminator bytes (use provided or fallback to common ones)
            byte[][] terminatorBytes = GetTerminatorBytes(terminator, textEncoding);

            // BINARY-SAFE SPLIT: Split bytes by terminator
            List<byte[]> lineBytes = ByteArraySplitter.SplitByAny(rawBytes, terminatorBytes, ByteArraySplitter.SplitOptions.None);

            // Convert to strings for pattern analysis (only for marker detection)
            string[] lines = lineBytes.Select(bytes => textEncoding.GetString(bytes)).ToArray();

            // STEP 1: Detect Start/End Markers
            var markerResult = DetectFrameMarkers(lines);

            if (markerResult != null && markerResult.Confidence > 0.8)
            {
                // Frame-based structure detected
                return ExtractFrames(rawBytes, markerResult, terminatorBytes, textEncoding);
            }

            // STEP 2: Fall back to single-line detection
            return ExtractSingleLineMessages(lineBytes);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets terminator byte sequences (uses detected or fallback to common ones).
        /// </summary>
        private byte[][] GetTerminatorBytes(TerminatorInfo terminator, Encoding encoding)
        {
            // If we have a detected terminator with high confidence, use it
            if (terminator != null && terminator.Bytes != null && terminator.Confidence >= 0.8)
            {
                return new byte[][] { terminator.Bytes };
            }

            // Fallback: Try common terminators in order of likelihood
            return new byte[][]
            {
                encoding.GetBytes("\r\n"),  // CRLF (Windows) - most common
                encoding.GetBytes("\n"),     // LF (Unix)
                encoding.GetBytes("\r")      // CR (Mac Classic)
            };
        }

        #endregion

        #region Private Methods - Frame Detection

        /// <summary>
        /// STEP 2 & 3: Detect Start/End Markers and analyze positions
        /// </summary>
        private FrameMarkerResult DetectFrameMarkers(string[] lines)
        {
            var markerCandidates = new Dictionary<string, MarkerCandidate>();

            // Scan for lines that start with special characters
            char[] specialChars = { '^', '~', '<', '>', '@', '#', '$', '\x02', '\x03' };

            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;

                char firstChar = lines[i][0];

                if (specialChars.Contains(firstChar))
                {
                    // Extract pattern (first few characters)
                    string pattern = lines[i].Length >= 5 ? lines[i].Substring(0, 5) : lines[i];

                    if (!markerCandidates.ContainsKey(pattern))
                    {
                        markerCandidates[pattern] = new MarkerCandidate
                        {
                            Pattern = pattern,
                            Positions = new List<int>(),
                            LineSamples = new List<string>()
                        };
                    }

                    markerCandidates[pattern].Positions.Add(i);
                    markerCandidates[pattern].LineSamples.Add(lines[i]);
                }
            }

            // Analyze marker positions to find consistent gaps
            MarkerCandidate bestStartMarker = null;
            double bestConfidence = 0;

            foreach (var marker in markerCandidates.Values)
            {
                if (marker.Positions.Count == 0)
                    continue;

                // Handle single frame case
                if (marker.Positions.Count == 1)
                {
                    // Look for end marker pattern
                    int startPos = marker.Positions[0];
                    string endMarker = FindEndMarkerSingleFrame(lines, startPos);

                    if (endMarker != null)
                    {
                        // Found start and end marker - this is likely a frame
                        // Calculate frame size
                        int frameLines = 0;
                        for (int i = startPos; i < lines.Length; i++)
                        {
                            frameLines++;
                            if (lines[i].Trim() == endMarker.Trim())
                                break;
                        }

                        if (frameLines > 1)
                        {
                            marker.ExpectedLinesPerFrame = frameLines;
                            marker.Confidence = 0.85; // Lower confidence for single frame
                            marker.EndMarker = endMarker;

                            if (marker.Confidence > bestConfidence)
                            {
                                bestConfidence = marker.Confidence;
                                bestStartMarker = marker;
                            }
                        }
                    }
                    continue;
                }

                // Multiple frames - calculate gaps
                var gaps = new List<int>();
                for (int i = 1; i < marker.Positions.Count; i++)
                {
                    gaps.Add(marker.Positions[i] - marker.Positions[i - 1]);
                }

                if (gaps.Count == 0)
                    continue;

                double avgGap = gaps.Average();
                double stdDev = CalculateStdDev(gaps);

                // Confidence is high if gaps are consistent
                double confidence = avgGap > 0 ? 1.0 - (stdDev / avgGap) : 0;

                // Only consider markers that appear in multi-line frames
                if (avgGap > 1 && confidence > bestConfidence)
                {
                    bestConfidence = confidence;
                    bestStartMarker = marker;
                    marker.ExpectedLinesPerFrame = (int)Math.Round(avgGap);
                    marker.Confidence = confidence;

                    // Look for end marker
                    marker.EndMarker = FindEndMarker(lines, marker);
                }
            }

            if (bestStartMarker != null && bestConfidence > 0.8)
            {
                return new FrameMarkerResult
                {
                    StartMarker = bestStartMarker.Pattern,
                    EndMarker = bestStartMarker.EndMarker,
                    LinesPerFrame = bestStartMarker.ExpectedLinesPerFrame,
                    Confidence = bestConfidence,
                    StartPositions = bestStartMarker.Positions
                };
            }

            return null;
        }

        /// <summary>
        /// Find the end marker for a single frame
        /// </summary>
        private string FindEndMarkerSingleFrame(string[] lines, int startPos)
        {
            // Look for lines with special ending characters (~ or short lines)
            char[] endMarkerChars = { '~', '<', '>', '\x03' }; // ETX

            for (int i = startPos + 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                // Check for end marker characteristics
                if (!string.IsNullOrEmpty(line) && line.Length < 10)
                {
                    if (endMarkerChars.Contains(line[0]))
                    {
                        return line;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Find the end marker by looking at expected position
        /// </summary>
        private string FindEndMarker(string[] lines, MarkerCandidate startMarker)
        {
            if (startMarker.Positions.Count == 0 || startMarker.ExpectedLinesPerFrame == 0)
                return null;

            // Look at the line before the next start marker
            var endCandidates = new Dictionary<string, int>();

            for (int i = 0; i < startMarker.Positions.Count - 1; i++)
            {
                int startPos = startMarker.Positions[i];
                int nextStartPos = startMarker.Positions[i + 1];
                int expectedEndPos = nextStartPos - 1;

                if (expectedEndPos >= 0 && expectedEndPos < lines.Length)
                {
                    string endLine = lines[expectedEndPos].Trim();
                    if (!string.IsNullOrEmpty(endLine))
                    {
                        if (!endCandidates.ContainsKey(endLine))
                            endCandidates[endLine] = 0;
                        endCandidates[endLine]++;
                    }
                }
            }

            // Return the most common end line
            return endCandidates.OrderByDescending(x => x.Value).FirstOrDefault().Key;
        }

        /// <summary>
        /// Extract complete frames using detected markers.
        /// BINARY-SAFE: Uses byte[] splitting with detected terminator.
        /// </summary>
        private LogData ExtractFrames(byte[] rawBytes, FrameMarkerResult frameInfo, byte[][] terminatorBytes, Encoding encoding)
        {
            // BINARY-SAFE SPLIT: Split bytes by terminator
            List<byte[]> lineBytes = ByteArraySplitter.SplitByAny(rawBytes, terminatorBytes, ByteArraySplitter.SplitOptions.None);
            string[] lines = lineBytes.Select(bytes => encoding.GetString(bytes)).ToArray();

            var messages = new List<byte[]>();

            for (int i = 0; i < frameInfo.StartPositions.Count; i++)
            {
                int startLine = frameInfo.StartPositions[i];
                int endLine;

                if (i < frameInfo.StartPositions.Count - 1)
                {
                    endLine = frameInfo.StartPositions[i + 1] - 1;
                }
                else
                {
                    endLine = Math.Min(startLine + frameInfo.LinesPerFrame - 1, lines.Length - 1);
                }

                // Combine lines into a single frame
                var frameLines = new List<string>();
                for (int j = startLine; j <= endLine; j++)
                {
                    if (j < lines.Length)
                        frameLines.Add(lines[j]);
                }

                string frameText = string.Join("\r\n", frameLines);
                if (!string.IsNullOrEmpty(frameText))
                {
                    byte[] frameBytes = Encoding.ASCII.GetBytes(frameText + "\r\n");
                    messages.Add(frameBytes);
                }
            }

            if (messages.Count == 0)
                return new LogData();

            int totalBytes = messages.Sum(m => m.Length);
            return new LogData
            {
                Messages = messages,
                MessageCount = messages.Count,
                TotalBytes = totalBytes,
                AverageMessageLength = totalBytes / messages.Count
            };
        }

        #endregion

        #region Private Methods - Single Line Detection

        /// <summary>
        /// Extract single-line messages when no frame markers found.
        /// BINARY-SAFE: Works with byte arrays directly.
        /// </summary>
        private LogData ExtractSingleLineMessages(List<byte[]> lineBytes)
        {
            var messages = new List<byte[]>();
            int totalBytes = 0;

            // Filter out empty lines
            foreach (var line in lineBytes)
            {
                if (line != null && line.Length > 0)
                {
                    messages.Add(line);
                    totalBytes += line.Length;
                }
            }

            if (messages.Count == 0 && lineBytes.Count > 0)
            {
                // No valid lines - return first line as single message
                byte[] firstLine = lineBytes[0];
                return new LogData {
                    Messages = new List<byte[]> { firstLine },
                    MessageCount = 1,
                    TotalBytes = firstLine.Length,
                    AverageMessageLength = firstLine.Length
                };
            }

            totalBytes = messages.Sum(m => m.Length);
            return new LogData
            {
                Messages = messages,
                MessageCount = messages.Count,
                TotalBytes = totalBytes,
                AverageMessageLength = totalBytes / messages.Count
            };
        }

        #endregion

        #region Helper Methods

        private double CalculateStdDev(List<int> values)
        {
            if (values.Count == 0)
                return 0;

            double avg = values.Average();
            double sumSquares = values.Sum(v => Math.Pow(v - avg, 2));
            return Math.Sqrt(sumSquares / values.Count);
        }

        #endregion

        #region Internal Classes

        private class MarkerCandidate
        {
            public string Pattern { get; set; }
            public List<int> Positions { get; set; }
            public List<string> LineSamples { get; set; }
            public int ExpectedLinesPerFrame { get; set; }
            public double Confidence { get; set; }
            public string EndMarker { get; set; }
        }

        private class FrameMarkerResult
        {
            public string StartMarker { get; set; }
            public string EndMarker { get; set; }
            public int LinesPerFrame { get; set; }
            public double Confidence { get; set; }
            public List<int> StartPositions { get; set; }
        }

        #endregion
    }
}
