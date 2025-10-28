using System;

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Text encoding types for serial communication
    /// </summary>
    public enum EncodingType
    {
        /// <summary>
        /// ASCII (7-bit) encoding
        /// </summary>
        ASCII,

        /// <summary>
        /// UTF-8 encoding (variable length 1-4 bytes)
        /// </summary>
        UTF8,

        /// <summary>
        /// UTF-16 encoding (2 or 4 bytes)
        /// </summary>
        UTF16,

        /// <summary>
        /// Latin-1 / ISO-8859-1 encoding
        /// </summary>
        Latin1
    }
}
