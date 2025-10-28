using System;

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Byte order for multi-byte numeric values
    /// </summary>
    public enum EndianType
    {
        /// <summary>
        /// Little-endian (least significant byte first)
        /// Example: 0x1234 stored as [34 12]
        /// </summary>
        LittleEndian,

        /// <summary>
        /// Big-endian (most significant byte first)
        /// Example: 0x1234 stored as [12 34]
        /// </summary>
        BigEndian
    }
}
