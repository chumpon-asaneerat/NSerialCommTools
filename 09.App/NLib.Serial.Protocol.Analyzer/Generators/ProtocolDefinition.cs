#region Using

using System;
using System.Collections.Generic;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Generators
{
    /// <summary>
    /// Represents a protocol definition (to be exported as JSON)
    /// </summary>
    public class ProtocolDefinition
    {
        public DeviceInfo DeviceInfo { get; set; }
        public ProtocolInfo Protocol { get; set; }
        public ParsingInfo Parsing { get; set; }
    }

    public class DeviceInfo
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
    }

    public class ProtocolInfo
    {
        public string Type { get; set; }
        public string Format { get; set; }
        public string Encoding { get; set; }
        public string Terminator { get; set; }
        public List<FieldDefinition> Fields { get; set; }

        // Package-based protocol properties
        public int? PackageSize { get; set; }
        public string StartMarker { get; set; }
        public string EndMarker { get; set; }
    }

    public class FieldDefinition
    {
        public string Name { get; set; }
        public int Position { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public bool? UnitAttached { get; set; }
        public List<string> Values { get; set; }
    }

    public class ParsingInfo
    {
        public string Strategy { get; set; }
        public string Delimiter { get; set; }
    }
}
