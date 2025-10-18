#region Using

using System;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Models
{
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
