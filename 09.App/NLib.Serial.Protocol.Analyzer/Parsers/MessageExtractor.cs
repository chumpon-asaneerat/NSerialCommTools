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
        /// Extracts frames using detected frame terminator.
        /// BINARY-SAFE: All operations on byte[], no string conversion.
        /// </summary>
        private List<byte[]> ExtractFrames(byte[] rawBytes, DetectionResult detection)
        {
            var frames = new List<byte[]>();

            // Check if we have a frame terminator
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
