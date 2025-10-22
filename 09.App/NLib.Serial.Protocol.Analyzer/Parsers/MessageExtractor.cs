#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLib.Serial.ProtocolAnalyzer.Models;

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
        /// </summary>
        public LogData ExtractMessages(byte[] rawBytes)
        {
            if (rawBytes == null || rawBytes.Length == 0)
                return new LogData();

            // Convert to text lines for pattern analysis
            string text = Encoding.ASCII.GetString(rawBytes);
            string[] lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            // STEP 1: Detect Start/End Markers
            var markerResult = DetectFrameMarkers(lines);

            if (markerResult != null && markerResult.Confidence > 0.8)
            {
                // Frame-based structure detected
                return ExtractFrames(rawBytes, markerResult);
            }

            // STEP 2: Fall back to single-line detection
            return ExtractSingleLineMessages(rawBytes, lines);
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
                if (marker.Positions.Count < 2)
                    continue;

                // Calculate gaps between occurrences
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
        /// Extract complete frames using detected markers
        /// </summary>
        private LogData ExtractFrames(byte[] rawBytes, FrameMarkerResult frameInfo)
        {
            string text = Encoding.ASCII.GetString(rawBytes);
            string[] lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

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
        /// Extract single-line messages when no frame markers found
        /// </summary>
        private LogData ExtractSingleLineMessages(byte[] rawBytes, string[] lines)
        {
            var messages = new List<byte[]>();

            // Filter out empty lines and convert back to bytes
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    byte[] lineBytes = Encoding.ASCII.GetBytes(line + "\r\n");
                    messages.Add(lineBytes);
                }
            }

            if (messages.Count == 0)
                return new LogData { Messages = new List<byte[]> { rawBytes }, MessageCount = 1, TotalBytes = rawBytes.Length, AverageMessageLength = rawBytes.Length };

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
