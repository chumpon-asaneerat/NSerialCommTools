#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLib.Serial.Protocol.Analyzer.Models;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Analyzers
{
    /// <summary>
    /// Detects multi-line package protocols
    /// </summary>
    public class PackageDetector
    {
        #region Public Methods

        /// <summary>
        /// Detects if this is a package-based protocol
        /// </summary>
        public PackageInfo DetectPackages(LogData data)
        {
            if (data == null || data.Messages == null || data.Messages.Count < 5)
                return null;

            // Convert messages to strings for analysis
            var messages = data.Messages
                .Select(m => Encoding.ASCII.GetString(m))
                .ToList();

            // Look for common package markers
            var startMarkers = new[] { "^", "~", "$", "@", "#" };
            var endMarkers = new[] { "~", "^", "$", "@", "#" };

            foreach (var startChar in startMarkers)
            {
                // Find messages that start with this marker
                var startIndices = messages
                    .Select((msg, idx) => new { msg, idx })
                    .Where(x => x.msg.StartsWith(startChar))
                    .Select(x => x.idx)
                    .ToList();

                if (startIndices.Count < 1)
                    continue;

                // Special case: Only one package found
                if (startIndices.Count == 1)
                {
                    var firstIdx = startIndices[0];

                    // Look for end marker
                    for (int i = firstIdx + 1; i < messages.Count; i++)
                    {
                        string msg = messages[i].Trim();
                        // Check if this could be an end marker (short line starting with special char)
                        if (msg.Length < 10 && (msg.StartsWith("~") || msg.StartsWith("$") || msg.StartsWith("#")))
                        {
                            int packageSize = i - firstIdx + 1;
                            return new PackageInfo
                            {
                                IsPackageBased = true,
                                PackageSize = packageSize,
                                StartMarker = messages[firstIdx].Trim(),
                                EndMarker = msg,
                                PackageCount = 1,
                                Confidence = 85.0 // Lower confidence for single package
                            };
                        }
                    }
                }

                // Multiple packages: Calculate size from distance
                var packageSizes = new List<int>();
                for (int i = 1; i < startIndices.Count; i++)
                {
                    int size = startIndices[i] - startIndices[i - 1];
                    packageSizes.Add(size);
                }

                // Check if package size is consistent
                if (packageSizes.Count > 0)
                {
                    double avgSize = packageSizes.Average();
                    double variance = packageSizes.Average(s => Math.Abs(s - avgSize));

                    // If variance is low, we have a consistent package size
                    if (variance / avgSize < 0.1) // Less than 10% variance
                    {
                        var firstPackageStart = startIndices[0];
                        var packageSize = (int)Math.Round(avgSize);

                        // Detect end marker
                        string endMarker = null;
                        if (firstPackageStart + packageSize - 1 < messages.Count)
                        {
                            endMarker = messages[firstPackageStart + packageSize - 1].Trim();
                        }

                        return new PackageInfo
                        {
                            IsPackageBased = true,
                            PackageSize = packageSize,
                            StartMarker = messages[firstPackageStart].Trim(),
                            EndMarker = endMarker,
                            PackageCount = startIndices.Count,
                            Confidence = 95.0
                        };
                    }
                }
            }

            return new PackageInfo { IsPackageBased = false };
        }

        #endregion
    }

    /// <summary>
    /// Package detection information
    /// </summary>
    public class PackageInfo
    {
        public bool IsPackageBased { get; set; }
        public int PackageSize { get; set; }
        public string StartMarker { get; set; }
        public string EndMarker { get; set; }
        public int PackageCount { get; set; }
        public double Confidence { get; set; }
    }
}
