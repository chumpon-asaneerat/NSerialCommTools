#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLib.Serial.ProtocolAnalyzer.Models;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Analyzers
{
    /// <summary>
    /// Selects the best parsing strategy for a protocol based on analysis.
    /// Implements the decision tree from Document 03: Parsing Strategy Analysis.
    /// </summary>
    public class StrategySelector
    {
        #region Public Methods

        /// <summary>
        /// Detects the best parsing strategy for the given log data.
        /// Returns the strategy type and confidence score.
        /// </summary>
        /// <param name="logData">The log data to analyze.</param>
        /// <returns>Tuple of (StrategyType, Confidence)</returns>
        public (StrategyType Strategy, double Confidence) DetectBestStrategy(LogData logData)
        {
            if (logData == null || logData.Messages == null || logData.Messages.Count == 0)
            {
                return (StrategyType.Unknown, 0.0);
            }

            // Calculate confidence for each strategy
            var scores = new Dictionary<StrategyType, double>
            {
                { StrategyType.DelimiterBased, DetectDelimiterBasedStrategy(logData) },
                { StrategyType.FrameBased, DetectFrameBasedStrategy(logData) },
                { StrategyType.StateMachine, DetectStateMachineStrategy(logData) },
                { StrategyType.PositionBased, DetectPositionBasedStrategy(logData) },
                { StrategyType.ContentBased, DetectContentBasedStrategy(logData) }
            };

            // Return highest confidence strategy
            var best = scores.OrderByDescending(kvp => kvp.Value).First();
            return (best.Key, best.Value);
        }

        #endregion

        #region Strategy Detection Methods

        /// <summary>
        /// Detects if protocol uses delimiter-based parsing.
        /// Checks for consistent delimiters (comma, space, tab) across messages.
        /// </summary>
        private double DetectDelimiterBasedStrategy(LogData logData)
        {
            // Look for consistent delimiters
            var delimiterCandidates = new[] { (byte)',', (byte)'\t', (byte)';', (byte)'|', (byte)'/' };

            foreach (var delimiter in delimiterCandidates)
            {
                var counts = new List<int>();

                foreach (var messageBytes in logData.Messages)
                {
                    if (messageBytes == null || messageBytes.Length == 0) continue;

                    int count = messageBytes.Count(b => b == delimiter);
                    counts.Add(count);
                }

                if (counts.Count == 0) continue;

                // Check consistency (same count per message)
                double avg = counts.Average();
                if (avg > 0)
                {
                    double stddev = CalculateStandardDeviation(counts);
                    double consistency = 1.0 - (stddev / (avg + 0.01)); // Avoid division by zero

                    // High consistency + frequency indicates delimiter-based
                    if (consistency > 0.8 && avg >= 1.0)
                    {
                        return consistency;
                    }
                }
            }

            return 0.0;
        }

        /// <summary>
        /// Detects if protocol uses frame-based parsing.
        /// Checks for start/end markers and multi-line messages.
        /// </summary>
        private double DetectFrameBasedStrategy(LogData logData)
        {
            // Look for special characters at start of messages
            var startMarkers = new[] { (byte)'^', (byte)'~', (byte)'<', (byte)'>', (byte)'@', (byte)'#', (byte)'$', (byte)'V' };
            var markerCounts = new Dictionary<byte, int>();

            foreach (var messageBytes in logData.Messages)
            {
                if (messageBytes == null || messageBytes.Length == 0) continue;

                byte firstByte = messageBytes[0];
                if (startMarkers.Contains(firstByte))
                {
                    if (!markerCounts.ContainsKey(firstByte))
                        markerCounts[firstByte] = 0;
                    markerCounts[firstByte]++;
                }
            }

            if (markerCounts.Count == 0)
                return 0.0;

            // Calculate marker frequency
            var bestMarker = markerCounts.OrderByDescending(kvp => kvp.Value).First();
            double markerFrequency = (double)bestMarker.Value / logData.Messages.Count;

            // If markers are consistent and messages vary, it's frame-based
            if (markerFrequency > 0.1) // At least 10% of messages have markers
            {
                return markerFrequency;
            }

            return 0.0;
        }

        /// <summary>
        /// Detects if protocol requires State Machine parsing.
        /// CRITICAL: Checks if identical patterns appear at different positions with different meanings.
        /// Example: "  1.94 kg" appears 3 times but means Tare, Gross, Net based on position.
        /// </summary>
        private double DetectStateMachineStrategy(LogData logData)
        {
            // First check if it has frame markers (required for state machine)
            double frameConfidence = DetectFrameBasedStrategy(logData);
            if (frameConfidence < 0.5)
                return 0.0; // State machine requires frames

            // Analyze pattern repetition within frames
            var patterns = new Dictionary<string, List<int>>();

            for (int i = 0; i < logData.Messages.Count; i++)
            {
                var messageBytes = logData.Messages[i];
                if (messageBytes == null || messageBytes.Length == 0) continue;

                // Convert to string for pattern analysis
                string message = Encoding.ASCII.GetString(messageBytes).Trim();
                if (string.IsNullOrEmpty(message)) continue;

                // Extract pattern (e.g., "  X.XX kg" becomes pattern)
                string pattern = ExtractPattern(message);

                if (!patterns.ContainsKey(pattern))
                    patterns[pattern] = new List<int>();
                patterns[pattern].Add(i);
            }

            if (patterns.Count == 0)
                return 0.0;

            // Check for patterns that repeat at different positions
            int repeatedPatternCount = patterns.Count(kvp => kvp.Value.Count > 1);
            double repetitionRatio = (double)repeatedPatternCount / patterns.Count;

            // High repetition + frames = State Machine needed
            if (repetitionRatio > 0.3 && frameConfidence > 0.5)
            {
                return Math.Min(frameConfidence + repetitionRatio, 1.0);
            }

            return 0.0;
        }

        /// <summary>
        /// Detects if protocol uses fixed position parsing.
        /// Checks if all messages have the same length (fixed-width fields).
        /// </summary>
        private double DetectPositionBasedStrategy(LogData logData)
        {
            if (logData.Messages.Count < 2)
                return 0.0;

            var lengths = logData.Messages
                .Where(m => m != null && m.Length > 0)
                .Select(m => m.Length)
                .ToList();

            if (lengths.Count == 0)
                return 0.0;

            // Check if all messages have same length
            int firstLength = lengths[0];
            bool allSameLength = lengths.All(len => len == firstLength);

            if (allSameLength && firstLength > 10) // Minimum length for fixed-width
            {
                return 0.9;
            }

            // Check length variance
            double avgLength = lengths.Average();
            if (avgLength < 1) return 0.0;

            double stddev = CalculateStandardDeviation(lengths.Select(l => (double)l).ToList());
            double consistency = 1.0 - (stddev / avgLength);

            if (consistency > 0.95)
                return consistency;

            return 0.0;
        }

        /// <summary>
        /// Content-based strategy (fallback).
        /// Used when no clear pattern is detected.
        /// </summary>
        private double DetectContentBasedStrategy(LogData logData)
        {
            // Content-based is the fallback - always has some confidence
            // but lower than specific strategies
            return 0.5;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Extracts a pattern from a message by replacing digits with wildcards.
        /// Example: "  1.94 kg" becomes "  #.## kg"
        /// </summary>
        private string ExtractPattern(string message)
        {
            var pattern = message.ToCharArray();
            for (int i = 0; i < pattern.Length; i++)
            {
                if (char.IsDigit(pattern[i]))
                    pattern[i] = '#';
            }
            return new string(pattern);
        }

        /// <summary>
        /// Calculates standard deviation of a list of numbers.
        /// </summary>
        private double CalculateStandardDeviation(List<int> values)
        {
            return CalculateStandardDeviation(values.Select(v => (double)v).ToList());
        }

        /// <summary>
        /// Calculates standard deviation of a list of doubles.
        /// </summary>
        private double CalculateStandardDeviation(List<double> values)
        {
            if (values.Count == 0)
                return 0.0;

            double avg = values.Average();
            double sumSquaredDiffs = values.Sum(v => Math.Pow(v - avg, 2));
            return Math.Sqrt(sumSquaredDiffs / values.Count);
        }

        #endregion
    }
}
