using System;

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Tracks a single configuration parameter with detection mode
    /// Supports Auto, Manual, or None detection modes
    /// </summary>
    public class DetectionModeInfo
    {
        /// <summary>
        /// Current detection mode (None, Auto, Manual)
        /// </summary>
        public DetectionMode Mode { get; set; }

        /// <summary>
        /// Value detected automatically (null if not detected)
        /// </summary>
        public string DetectedValue { get; set; }

        /// <summary>
        /// Value entered manually by user (null if not set)
        /// </summary>
        public string ManualValue { get; set; }

        /// <summary>
        /// Gets the effective value based on current mode
        /// Returns ManualValue if Mode==Manual, DetectedValue if Mode==Auto, null if Mode==None
        /// </summary>
        public string EffectiveValue
        {
            get
            {
                switch (Mode)
                {
                    case DetectionMode.Manual:
                        return ManualValue;
                    case DetectionMode.Auto:
                        return DetectedValue;
                    case DetectionMode.None:
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Constructor - initializes to None mode
        /// </summary>
        public DetectionModeInfo()
        {
            Mode = DetectionMode.None;
            DetectedValue = null;
            ManualValue = null;
        }

        /// <summary>
        /// Sets auto-detected value and switches to Auto mode
        /// </summary>
        /// <param name="value">The detected value</param>
        public void SetAutoDetected(string value)
        {
            DetectedValue = value;
            Mode = DetectionMode.Auto;
        }

        /// <summary>
        /// Sets manual value and switches to Manual mode
        /// </summary>
        /// <param name="value">The manual value</param>
        public void SetManual(string value)
        {
            ManualValue = value;
            Mode = DetectionMode.Manual;
        }

        /// <summary>
        /// Clears all values and resets to None mode
        /// </summary>
        public void Clear()
        {
            Mode = DetectionMode.None;
            DetectedValue = null;
            ManualValue = null;
        }
    }
}
