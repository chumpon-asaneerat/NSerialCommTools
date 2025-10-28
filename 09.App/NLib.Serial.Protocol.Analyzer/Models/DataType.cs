using System;

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Field data types supported by the Protocol Analyzer
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// String/text data
        /// </summary>
        String,

        /// <summary>
        /// Integer number (int, long)
        /// </summary>
        Integer,

        /// <summary>
        /// Floating point number (float, double, decimal)
        /// </summary>
        Float,

        /// <summary>
        /// Hexadecimal data
        /// </summary>
        Hex,

        /// <summary>
        /// Binary data (byte array)
        /// </summary>
        Binary,

        /// <summary>
        /// Date and time
        /// </summary>
        DateTime
    }
}
