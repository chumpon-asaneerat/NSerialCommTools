#region Using

using System;
using System.Linq;
using NLib.Serial.Protocol.Analyzer.Models;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Analyzers
{
    /// <summary>
    /// Main pattern analyzer that coordinates all detection algorithms
    /// </summary>
    public class PatternAnalyzer
    {
        #region Private Fields

        private TerminatorDetector _terminatorDetector = new TerminatorDetector();
        private DelimiterDetector _delimiterDetector = new DelimiterDetector();
        private FieldAnalyzer _fieldAnalyzer = new FieldAnalyzer();
        private PackageDetector _packageDetector = new PackageDetector();

        #endregion

        #region Public Methods

        /// <summary>
        /// Analyzes log data and returns analysis results
        /// </summary>
        public AnalysisResult Analyze(LogData data)
        {
            if (data == null || data.Messages == null || data.Messages.Count == 0)
                return null;

            var result = new AnalysisResult();

            // 0. Check for package-based protocol first
            var packageInfo = _packageDetector.DetectPackages(data);
            if (packageInfo != null && packageInfo.IsPackageBased)
            {
                // This is a multi-line package protocol (like JIK6CAB)
                result.ProtocolType = "multi-line-package";
                result.RecommendedStrategy = "state-machine";
                result.PackageSize = packageInfo.PackageSize;
                result.StartMarker = packageInfo.StartMarker;
                result.EndMarker = packageInfo.EndMarker;
                result.Confidence = packageInfo.Confidence;
                result.PackageInfo = packageInfo; // Store for later use

                // Set basic terminator info
                result.Terminator = _terminatorDetector.DetectTerminator(data);

                result.MessageCount = data.MessageCount;
                result.AverageLength = data.AverageMessageLength;

                return result;
            }

            // Regular single-line protocol analysis
            // 1. Detect terminator
            result.Terminator = _terminatorDetector.DetectTerminator(data);

            // 2. Detect delimiters
            result.Delimiters = _delimiterDetector.DetectDelimiters(data);

            // 3. Analyze fields using best delimiter
            if (result.Delimiters.Count > 0)
            {
                var bestDelimiter = result.Delimiters.OrderByDescending(d => d.Confidence).First();
                result.Fields = _fieldAnalyzer.AnalyzeFields(data, bestDelimiter.Delimiter);
            }

            // 4. Set statistics
            result.MessageCount = data.MessageCount;
            result.AverageLength = data.AverageMessageLength;

            // 5. Determine protocol type
            result.ProtocolType = DetermineProtocolType(data);

            // 6. Recommend parsing strategy
            result.RecommendedStrategy = RecommendStrategy(result);

            // 7. Calculate overall confidence
            result.Confidence = CalculateOverallConfidence(result);

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the protocol type
        /// </summary>
        private string DetermineProtocolType(LogData data)
        {
            // If messages are similar length and frequent, likely streaming
            if (data.Messages.Count > 10)
            {
                var lengths = data.Messages.Select(m => m.Length).ToList();
                double avgLength = lengths.Average();
                double variance = lengths.Average(l => Math.Abs(l - avgLength));

                if (variance / avgLength < 0.2)
                    return "streaming";
            }

            return "command-response";
        }

        /// <summary>
        /// Recommends parsing strategy
        /// </summary>
        private string RecommendStrategy(AnalysisResult result)
        {
            // If we have structural delimiters, use split
            if (result.Delimiters.Any(d => d.IsStructural))
                return "split";

            // If complex patterns, use regex
            if (result.Fields.Count > 5)
                return "regex";

            // Default to split
            return "split";
        }

        /// <summary>
        /// Calculates overall confidence
        /// </summary>
        private double CalculateOverallConfidence(AnalysisResult result)
        {
            double confidence = 0;
            int factors = 0;

            if (result.Terminator != null && result.Terminator.Detected)
            {
                confidence += result.Terminator.Confidence;
                factors++;
            }

            if (result.Delimiters.Count > 0)
            {
                confidence += result.Delimiters.Max(d => d.Confidence);
                factors++;
            }

            if (result.Fields.Count > 0)
            {
                confidence += result.Fields.Average(f => f.Confidence);
                factors++;
            }

            return factors > 0 ? confidence / factors : 0;
        }

        #endregion
    }
}
