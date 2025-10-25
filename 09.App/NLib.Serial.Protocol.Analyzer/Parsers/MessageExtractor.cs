#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using NLib.Serial.ProtocolAnalyzer.Models;
using NLib.Serial.ProtocolAnalyzer.Utilities;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Parsers
{
    /// <summary>
    /// PASS 2: Message Extraction Phase
    /// Extracts individual messages/frames from raw byte stream using pre-detected patterns.
    /// NO GUESSING - uses DetectionResult from Pass 1 (ProtocolDetector).
    /// Binary-safe throughout - all operations on byte[], string conversion only for validation.
    /// Based on Document: REFACTOR-TODO-Two-Pass-Architecture.md
    /// </summary>
    public class MessageExtractor
    {
        #region Public Methods

        /// <summary>
        /// Extracts messages using pre-detected protocol structure (TWO-PASS ARCHITECTURE).
        /// This is PASS 2 - uses patterns detected in Pass 1, NO GUESSING.
        /// </summary>
        /// <param name="rawBytes">Raw byte data from log file (UNSPLIT).</param>
        /// <param name="detection">Detection results from Pass 1 (ProtocolDetector).</param>
        /// <returns>LogData with extracted messages as byte arrays.</returns>
        public LogData ExtractMessages(byte[] rawBytes, DetectionResult detection)
        {
            if (rawBytes == null || rawBytes.Length == 0)
            {
                return new LogData
                {
                    Messages = new List<byte[]>(),
                    MessageCount = 0,
                    TotalBytes = 0,
                    AverageMessageLength = 0
                };
            }

            if (detection == null)
            {
                throw new ArgumentNullException(nameof(detection),
                    "DetectionResult is required. Run ProtocolDetector.DetectProtocolStructure() first (PASS 1).");
            }

            // Extract frames based on detected structure
            List<byte[]> frames = ExtractFrames(rawBytes, detection);

            // Build LogData result
            if (frames.Count == 0)
            {
                // Fallback: treat entire file as single message
                frames.Add(rawBytes);
            }

            int totalBytes = frames.Sum(f => f.Length);

            return new LogData
            {
                Messages = frames,
                MessageCount = frames.Count,
                TotalBytes = totalBytes,
                AverageMessageLength = frames.Count > 0 ? totalBytes / frames.Count : 0
            };
        }

        #endregion

        #region Private Methods - Frame Extraction

        /// <summary>
        /// Extracts frames using detected markers or frame terminator.
        /// PRIORITY: Markers > Terminator
        /// BINARY-SAFE: All operations on byte[], no string conversion.
        /// </summary>
        private List<byte[]> ExtractFrames(byte[] rawBytes, DetectionResult detection)
        {
            var frames = new List<byte[]>();

            // PRIORITY 1: Check for frame markers (most reliable)
            if (detection.StartMarker != null &&
                detection.StartMarker.Bytes != null &&
                detection.StartMarker.Confidence >= 0.7)
            {
                // Split by start marker
                return ExtractFramesByStartMarker(rawBytes, detection.StartMarker.Bytes);
            }

            if (detection.EndMarker != null &&
                detection.EndMarker.Bytes != null &&
                detection.EndMarker.Confidence >= 0.7)
            {
                // Split by end marker
                return ExtractFramesByEndMarker(rawBytes, detection.EndMarker.Bytes);
            }

            // PRIORITY 2: Check for frame terminator
            if (detection.FrameTerminator == null ||
                detection.FrameTerminator.Bytes == null ||
                detection.FrameTerminator.Confidence < 0.5)
            {
                // No reliable frame terminator detected
                // Fallback: treat entire data as single frame
                frames.Add(rawBytes);
                return frames;
            }

            // BINARY-SAFE SPLIT: Use detected frame terminator
            byte[] frameTerminatorBytes = detection.FrameTerminator.Bytes;

            // Split by frame terminator
            List<byte[]> splitFrames = ByteArraySplitter.Split(
                rawBytes,
                frameTerminatorBytes,
                ByteArraySplitter.SplitOptions.RemoveEmptyEntries
            );

            // Filter out empty frames
            foreach (var frame in splitFrames)
            {
                if (frame != null && frame.Length > 0)
                {
                    frames.Add(frame);
                }
            }

            return frames;
        }

        /// <summary>
        /// Extracts frames by splitting on start markers.
        /// Each frame begins with the start marker.
        /// </summary>
        private List<byte[]> ExtractFramesByStartMarker(byte[] rawBytes, byte[] startMarker)
        {
            var frames = new List<byte[]>();
            var frameStartPositions = new List<int>();

            // Find all positions where start marker occurs
            for (int i = 0; i <= rawBytes.Length - startMarker.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < startMarker.Length; j++)
                {
                    if (rawBytes[i + j] != startMarker[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    frameStartPositions.Add(i);
                }
            }

            // Extract frames between start markers
            for (int i = 0; i < frameStartPositions.Count; i++)
            {
                int startPos = frameStartPositions[i];
                int endPos = (i + 1 < frameStartPositions.Count)
                    ? frameStartPositions[i + 1]
                    : rawBytes.Length;

                int frameLength = endPos - startPos;
                byte[] frame = new byte[frameLength];
                Array.Copy(rawBytes, startPos, frame, 0, frameLength);

                frames.Add(frame);
            }

            return frames;
        }

        /// <summary>
        /// Extracts frames by splitting on end markers.
        /// Each frame ends with the end marker.
        /// </summary>
        private List<byte[]> ExtractFramesByEndMarker(byte[] rawBytes, byte[] endMarker)
        {
            var frames = new List<byte[]>();
            int lastEndPos = 0;

            // Find all positions where end marker occurs
            for (int i = 0; i <= rawBytes.Length - endMarker.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < endMarker.Length; j++)
                {
                    if (rawBytes[i + j] != endMarker[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    // Found end marker - extract frame from last position to here
                    int frameLength = (i + endMarker.Length) - lastEndPos;
                    byte[] frame = new byte[frameLength];
                    Array.Copy(rawBytes, lastEndPos, frame, 0, frameLength);

                    frames.Add(frame);
                    lastEndPos = i + endMarker.Length;
                }
            }

            // Add any remaining data as the last frame
            if (lastEndPos < rawBytes.Length)
            {
                int frameLength = rawBytes.Length - lastEndPos;
                byte[] frame = new byte[frameLength];
                Array.Copy(rawBytes, lastEndPos, frame, 0, frameLength);

                if (frame.Length > 0)
                {
                    frames.Add(frame);
                }
            }

            return frames;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Validates that frames were extracted successfully.
        /// </summary>
        private bool ValidateFrames(List<byte[]> frames, byte[] rawBytes)
        {
            if (frames == null || frames.Count == 0)
                return false;

            // Check that total extracted bytes is reasonable compared to input
            int totalExtracted = frames.Sum(f => f.Length);
            double ratio = (double)totalExtracted / rawBytes.Length;

            // Should extract at least 50% of original data
            return ratio >= 0.5;
        }

        #endregion
    }
}
