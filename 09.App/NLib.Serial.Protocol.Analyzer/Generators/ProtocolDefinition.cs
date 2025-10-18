#region Using

using System;
using System.Collections.Generic;
using NLib.Serial.Protocol.Analyzer.Models;

#endregion

namespace NLib.Serial.Protocol.Analyzer.Generators
{
    /// <summary>
    /// Represents a protocol definition (to be exported as JSON)
    /// </summary>
    public class ProtocolDefinition
    {
        public string Schema { get; set; }
        public string Version { get; set; }
        public string LastUpdated { get; set; }

        public DeviceInfo DeviceInfo { get; set; }
        public CommunicationInfo Communication { get; set; }
        public ProtocolInfo Protocol { get; set; }
        public ParsingInfo Parsing { get; set; }
    }

    public class DeviceInfo
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Category { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public string Complexity { get; set; }
    }

    public class CommunicationInfo
    {
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public string Parity { get; set; }
        public string StopBits { get; set; }
        public string Handshake { get; set; }
        public string Encoding { get; set; }
        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }
    }

    public class ProtocolInfo
    {
        public string Type { get; set; }
        public string Format { get; set; }
        public string Encoding { get; set; }
        public string Terminator { get; set; }
        public int? PackageSize { get; set; }
        public string UpdateRate { get; set; }

        // For simple CSV protocols
        public List<FieldDefinition> Fields { get; set; }

        // For package-based protocols
        public MarkerInfo Markers { get; set; }
        public StructureInfo Structure { get; set; }
    }

    public class MarkerInfo
    {
        public MarkerDetail Start { get; set; }
        public MarkerDetail End { get; set; }
    }

    public class MarkerDetail
    {
        public string Pattern { get; set; }
        public string Description { get; set; }
        public string Action { get; set; }
    }

    public class StructureInfo
    {
        public List<LineDefinition> Lines { get; set; }
    }

    public class LineDefinition
    {
        public int LineNumber { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Pattern { get; set; }
        public string Format { get; set; }
        public string Unit { get; set; }
        public bool? UnitAttached { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public bool Required { get; set; }
        public string Description { get; set; }
        public List<string> Values { get; set; }
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
