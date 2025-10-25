#region Using

using System;

#endregion

namespace NLib.Serial.ProtocolAnalyzer.Models
{
    /// <summary>
    /// Enumeration of parsing strategies.
    /// The analyzer automatically selects the best strategy based on protocol characteristics.
    /// </summary>
    public enum StrategyType
    {
        /// <summary>
        /// Unknown or undetermined strategy.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Delimiter-Based Strategy.
        /// Fields are separated by consistent delimiters (space, comma, tab, etc.).
        /// Example: "ST,GS    20.7g" where comma separates fields.
        /// </summary>
        DelimiterBased = 1,

        /// <summary>
        /// Frame-Based Strategy.
        /// Multi-line messages with start/end markers, content determines field meaning.
        /// Example: TFO1 with "V1" header and frame markers.
        /// </summary>
        FrameBased = 2,

        /// <summary>
        /// State Machine Strategy (Position-Dependent).
        /// Multi-line messages where LINE POSITION determines field meaning.
        /// CRITICAL: Same pattern appears multiple times but means different things.
        /// Example: JIK6CAB where line 4="Tare", line 5="Gross", line 8="Net" (all "X.XX kg").
        /// </summary>
        StateMachine = 3,

        /// <summary>
        /// Position-Based Strategy.
        /// Fixed-width fields at specific byte offsets.
        /// Example: Fields always at byte positions 0-10, 11-20, 21-30.
        /// </summary>
        PositionBased = 4,

        /// <summary>
        /// Content-Based Strategy.
        /// Variable structure, pattern matching determines field type.
        /// Example: PHMeter where line content pattern identifies the field.
        /// </summary>
        ContentBased = 5
    }
}
