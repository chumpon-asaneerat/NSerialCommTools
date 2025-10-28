using System;

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Detection mode for protocol configuration parameters
    /// </summary>
    public enum DetectionMode
    {
        /// <summary>
        /// No detection or configuration set
        /// </summary>
        None,

        /// <summary>
        /// Automatically detected from log data
        /// </summary>
        Auto,

        /// <summary>
        /// Manually entered by user
        /// </summary>
        Manual
    }
}
