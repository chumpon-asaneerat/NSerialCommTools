#region Using

using System;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Represents the format of a serial log file
    /// </summary>
    public enum LogFileFormat
    {
        /// <summary>
        /// Unknown format
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Hex bytes on left, ASCII text on right
        /// Example: "46 20 20 30 2E 30 0D    F  0.0."
        /// </summary>
        HexAndAscii = 1,

        /// <summary>
        /// Pure hex bytes only (no ASCII column)
        /// Example: "5E 4B 4A 49 4B 30 30 30 0D 0A"
        /// </summary>
        PureHex = 2,

        /// <summary>
        /// Pure text/ASCII (already decoded)
        /// Example: "ST,GS    20.7g  "
        /// </summary>
        PureText = 3
    }
}
