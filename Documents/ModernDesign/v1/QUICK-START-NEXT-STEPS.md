# Protocol Analyzer - Quick Start Next Steps

**Quick Reference Guide for Completing the Protocol Analyzer**

---

## 🎯 What We Have Now

### ✅ Working Features
1. **Log File Analysis** - Can analyze hex dumps and detect patterns
2. **Basic Definition Generation** - Creates JSON structure
3. **Package Detection** - Identifies multi-line protocols (JIK6CAB, TFO1)
4. **Field Analysis** - Detects fields, types, and delimiters

### ❌ Missing Critical Features
1. **Runtime Engine** - Cannot parse protocol using definitions yet
2. **Serializer** - Cannot convert C# objects back to protocol
3. **Complete Definitions** - Generated definitions missing validation, tests, etc.

---

## 🚀 Priority Implementation Order

### **PRIORITY 1: Complete Definition Generation** (Do This First!)
**Why:** We have 9 hand-crafted definitions as reference. Generated definitions should match.

**What to Build:**
```
Generators/EnhancedDefinitionGenerator.cs
├── GenerateValidationRules()      // From data analysis
├── GenerateTestCases()            // From sample messages
├── GenerateOutputMapping()        // C# property mapping
└── GenerateDocumentation()        // Notes and examples
```

**Example Output:**
Before (current):
```json
{
  "protocol": {
    "type": "csv",
    "fields": [...]
  }
}
```

After (enhanced):
```json
{
  "protocol": { ... },
  "validation": {
    "rules": [
      {"name": "WeightRange", "min": 0, "max": 9999.9, ...}
    ]
  },
  "testing": {
    "testCases": [
      {"input": "...", "expectedOutput": {...}}
    ]
  },
  "output": {
    "dataClass": "TScaleNHBData",
    "properties": [...]
  }
}
```

**Files to Create:**
1. `Generators/ValidationRuleGenerator.cs`
2. `Generators/TestCaseGenerator.cs`
3. `Generators/OutputMappingGenerator.cs`
4. Update `Generators/DefinitionGenerator.cs` to use new generators

---

### **PRIORITY 2: Runtime Protocol Engine** (Do This Second!)
**Why:** Most valuable feature - parse protocol data using JSON definitions

**What to Build:**
```
Runtime/
├── ProtocolEngine.cs              // Main API
├── DefinitionLoader.cs            // Load JSON
├── Parsers/
│   ├── CSVParser.cs              // For TScale, etc.
│   └── PackageParser.cs          // For JIK6CAB, etc.
└── FieldExtractor.cs              // Parse individual fields
```

**Example Usage:**
```csharp
// Load definition
var engine = new ProtocolEngine("TScaleNHB-Full-Definition.json");

// Parse data
byte[] data = new byte[] { 0x53, 0x54, 0x2C, ... }; // "ST,GS 20.7g\r\n"
var result = engine.Parse<TScaleNHBData>(data);

// Use result
Console.WriteLine($"Weight: {result.W} {result.Unit}");
// Output: Weight: 20.7 g
```

**Files to Create:**
1. `Runtime/ProtocolEngine.cs` - Main entry point
2. `Runtime/DefinitionLoader.cs` - Load and validate JSON
3. `Runtime/Parsers/CSVParser.cs` - Parse delimited protocols
4. `Runtime/Parsers/PackageParser.cs` - Parse multi-line packages
5. `Runtime/FieldExtractor.cs` - Extract and convert field values

---

### **PRIORITY 3: Protocol Serializer** (Do This Third!)
**Why:** Complete the round-trip - C# object → Protocol bytes

**What to Build:**
```
Serialization/
├── ProtocolSerializer.cs          // Main API
├── Formatters/
│   ├── CSVFormatter.cs           // Format CSV output
│   └── PackageFormatter.cs       // Format multi-line packages
└── FieldFormatter.cs              // Format individual fields
```

**Example Usage:**
```csharp
// Load definition
var serializer = new ProtocolSerializer("TScaleNHB-Full-Definition.json");

// Create data
var data = new TScaleNHBData
{
    Status = "ST",
    Mode = "GS",
    W = 20.7m,
    Unit = "g"
};

// Serialize
byte[] output = serializer.Serialize(data);
// Result: "ST,GS    20.7g  \r\n" (as bytes)
```

**Files to Create:**
1. `Serialization/ProtocolSerializer.cs` - Main entry point
2. `Serialization/Formatters/CSVFormatter.cs` - CSV formatting
3. `Serialization/Formatters/PackageFormatter.cs` - Package formatting
4. `Serialization/FieldFormatter.cs` - Field-level formatting

---

## 📋 Week-by-Week Plan

### **Week 1: Enhanced Definition Generation**

**Day 1: Advanced Pattern Recognition**
```csharp
// File: Analyzers/AdvancedPatternRecognizer.cs
public class AdvancedPatternRecognizer
{
    public bool IsFractionalFormat(string sample)
    {
        // Detect: +007.12/3 → 7.123
        return Regex.IsMatch(sample, @"[+-]\d+\.\d+/\d");
    }

    public List<byte> DetectSpecialCharacters(List<byte[]> messages)
    {
        // Detect: 0xF8 (°), 0xF4, 0xF3, 0xF2
        var special = new List<byte>();
        foreach (var msg in messages)
        {
            foreach (var b in msg)
            {
                if (b > 0x7F) special.Add(b);
            }
        }
        return special.Distinct().ToList();
    }

    public string DetectUnitAttachment(List<string> samples)
    {
        // "20.7g" → attached
        // "20.7 g" → separated
        bool hasSpace = samples.Any(s => Regex.IsMatch(s, @"\d\s+[a-z]"));
        return hasSpace ? "separated" : "attached";
    }
}
```

**Day 2: Validation Rules**
```csharp
// File: Generators/ValidationRuleGenerator.cs
public class ValidationRuleGenerator
{
    public List<ValidationRule> GenerateRules(List<FieldInfo> fields)
    {
        var rules = new List<ValidationRule>();

        foreach (var field in fields)
        {
            if (field.Type == "decimal" || field.Type == "integer")
            {
                // Range validation
                rules.Add(new ValidationRule
                {
                    Name = $"{field.Name}Range",
                    Type = "range",
                    Field = field.Name,
                    Min = field.MinValue,
                    Max = field.MaxValue,
                    Severity = "error",
                    Message = $"{field.Name} must be between {field.MinValue} and {field.MaxValue}"
                });
            }

            if (field.IsConstant)
            {
                // Enum validation
                rules.Add(new ValidationRule
                {
                    Name = $"{field.Name}Valid",
                    Type = "enum",
                    Field = field.Name,
                    Values = field.UniqueValues.ToList(),
                    Severity = "error",
                    Message = $"{field.Name} must be one of: {string.Join(", ", field.UniqueValues)}"
                });
            }
        }

        return rules;
    }
}
```

**Day 3: Test Cases**
```csharp
// File: Generators/TestCaseGenerator.cs
public class TestCaseGenerator
{
    public List<TestCase> GenerateTestCases(LogData logData, AnalysisResult analysis)
    {
        var testCases = new List<TestCase>();

        // Take first 3 messages as test cases
        for (int i = 0; i < Math.Min(3, logData.Messages.Count); i++)
        {
            var message = logData.Messages[i];
            var parsed = ParseMessage(message, analysis);

            testCases.Add(new TestCase
            {
                Name = $"ValidMessage{i + 1}",
                Description = "Representative sample message",
                Input = BitConverter.ToString(message).Replace("-", " "),
                ExpectedOutput = parsed,
                ShouldSucceed = true
            });
        }

        // Add edge cases
        testCases.Add(CreateZeroWeightTest());
        testCases.Add(CreateMaxWeightTest());

        return testCases;
    }
}
```

**Day 4: Output Mapping**
```csharp
// File: Generators/OutputMappingGenerator.cs
public class OutputMappingGenerator
{
    public OutputMapping GenerateMapping(List<FieldInfo> fields, string deviceName)
    {
        return new OutputMapping
        {
            DataClass = $"{deviceName}Data",
            Properties = fields.Select(f => new PropertyMapping
            {
                Name = f.SuggestedName,
                Type = GetCSharpType(f.Type),
                Mapping = f.Name,
                Default = GetDefaultValue(f)
            }).ToList(),
            EventTrigger = "LineComplete",
            EventName = "DataReceived"
        };
    }

    private string GetCSharpType(string fieldType)
    {
        return fieldType switch
        {
            "decimal" => "decimal",
            "integer" => "int",
            "datetime" => "DateTime",
            _ => "string"
        };
    }
}
```

**Day 5: Integration**
```csharp
// Update: Generators/DefinitionGenerator.cs
public ProtocolDefinition Generate(AnalysisResult analysis, ...)
{
    var definition = new ProtocolDefinition { ... };

    // NEW: Add validation rules
    var validationGen = new ValidationRuleGenerator();
    definition.Validation = new ValidationInfo
    {
        Rules = validationGen.GenerateRules(analysis.Fields)
    };

    // NEW: Add test cases
    var testGen = new TestCaseGenerator();
    definition.Testing = new TestingInfo
    {
        TestCases = testGen.GenerateTestCases(logData, analysis)
    };

    // NEW: Add output mapping
    var outputGen = new OutputMappingGenerator();
    definition.Output = outputGen.GenerateMapping(analysis.Fields, deviceName);

    return definition;
}
```

---

### **Week 2: Runtime Engine Basics**

**Day 1: Definition Loader**
```csharp
// File: Runtime/DefinitionLoader.cs
public class DefinitionLoader
{
    private static Dictionary<string, ProtocolDefinition> _cache
        = new Dictionary<string, ProtocolDefinition>();

    public static ProtocolDefinition Load(string filePath)
    {
        if (_cache.ContainsKey(filePath))
            return _cache[filePath];

        string json = File.ReadAllText(filePath);
        var definition = json.FromJson<ProtocolDefinition>();

        // Validate
        if (definition.Protocol == null)
            throw new InvalidDataException("Invalid protocol definition");

        _cache[filePath] = definition;
        return definition;
    }
}
```

**Day 2-3: CSV Parser**
```csharp
// File: Runtime/Parsers/CSVParser.cs
public class CSVParser
{
    private ProtocolDefinition _definition;

    public CSVParser(ProtocolDefinition definition)
    {
        _definition = definition;
    }

    public Dictionary<string, object> Parse(byte[] data)
    {
        // Convert to string
        string line = Encoding.ASCII.GetString(data).Trim();

        // Get delimiter
        string delimiter = _definition.Parsing.Delimiter ?? ",";

        // Split
        string[] parts = line.Split(new[] { delimiter },
            StringSplitOptions.None);

        // Extract fields
        var result = new Dictionary<string, object>();
        foreach (var field in _definition.Protocol.Fields)
        {
            if (field.Position < parts.Length)
            {
                string value = parts[field.Position].Trim();
                result[field.Name] = ConvertValue(value, field);
            }
        }

        return result;
    }

    private object ConvertValue(string value, FieldDefinition field)
    {
        // Remove unit if attached
        if (field.UnitAttached && !string.IsNullOrEmpty(field.Unit))
        {
            value = value.Replace(field.Unit, "").Trim();
        }

        // Convert based on type
        return field.Type switch
        {
            "decimal" => decimal.Parse(value),
            "integer" => int.Parse(value),
            "datetime" => DateTime.Parse(value),
            _ => value
        };
    }
}
```

**Day 4: Protocol Engine**
```csharp
// File: Runtime/ProtocolEngine.cs
public class ProtocolEngine
{
    private ProtocolDefinition _definition;
    private CSVParser _csvParser;
    private PackageParser _packageParser;

    public ProtocolEngine(string definitionFile)
    {
        _definition = DefinitionLoader.Load(definitionFile);

        if (_definition.Protocol.Type == "multi-line-package")
            _packageParser = new PackageParser(_definition);
        else
            _csvParser = new CSVParser(_definition);
    }

    public T Parse<T>(byte[] data) where T : new()
    {
        // Parse to dictionary
        var parsed = _definition.Protocol.Type == "multi-line-package"
            ? _packageParser.Parse(data)
            : _csvParser.Parse(data);

        // Map to C# object
        return MapToObject<T>(parsed);
    }

    private T MapToObject<T>(Dictionary<string, object> parsed) where T : new()
    {
        var obj = new T();
        var type = typeof(T);

        foreach (var prop in _definition.Output.Properties)
        {
            if (parsed.ContainsKey(prop.Mapping))
            {
                var property = type.GetProperty(prop.Name);
                if (property != null)
                {
                    property.SetValue(obj, parsed[prop.Mapping]);
                }
            }
        }

        return obj;
    }
}
```

**Day 5: Testing**
```csharp
// Test the engine
var engine = new ProtocolEngine("TScaleNHB-Full-Definition.json");

byte[] data = Encoding.ASCII.GetBytes("ST,GS    20.7g  \r\n");
var result = engine.Parse<TScaleNHBData>(data);

Assert.AreEqual("ST", result.Status);
Assert.AreEqual("GS", result.Mode);
Assert.AreEqual(20.7m, result.W);
Assert.AreEqual("g", result.Unit);
```

---

### **Week 3: Serializer Basics**

**Day 1-2: CSV Formatter**
```csharp
// File: Serialization/Formatters/CSVFormatter.cs
public class CSVFormatter
{
    private ProtocolDefinition _definition;

    public CSVFormatter(ProtocolDefinition definition)
    {
        _definition = definition;
    }

    public byte[] Format(Dictionary<string, object> data)
    {
        var parts = new List<string>();

        foreach (var field in _definition.Protocol.Fields.OrderBy(f => f.Position))
        {
            if (data.ContainsKey(field.Name))
            {
                string value = FormatField(data[field.Name], field);
                parts.Add(value);
            }
        }

        string line = string.Join(_definition.Parsing.Delimiter, parts);
        line += _definition.Protocol.Terminator;

        return Encoding.ASCII.GetBytes(line);
    }

    private string FormatField(object value, FieldDefinition field)
    {
        string formatted = "";

        // Format based on type
        if (field.Type == "decimal" && field.Format != null)
        {
            decimal d = Convert.ToDecimal(value);
            formatted = d.ToString(field.Format);
        }
        else
        {
            formatted = value.ToString();
        }

        // Add unit if needed
        if (field.UnitAttached && !string.IsNullOrEmpty(field.Unit))
        {
            formatted += field.Unit;
        }

        // Apply padding
        if (field.TotalWidth > 0)
        {
            formatted = field.Alignment == "left"
                ? formatted.PadRight(field.TotalWidth)
                : formatted.PadLeft(field.TotalWidth);
        }

        return formatted;
    }
}
```

**Day 3: Protocol Serializer**
```csharp
// File: Serialization/ProtocolSerializer.cs
public class ProtocolSerializer
{
    private ProtocolDefinition _definition;
    private CSVFormatter _csvFormatter;

    public ProtocolSerializer(string definitionFile)
    {
        _definition = DefinitionLoader.Load(definitionFile);
        _csvFormatter = new CSVFormatter(_definition);
    }

    public byte[] Serialize<T>(T obj)
    {
        // Convert C# object to dictionary
        var data = ObjectToDictionary(obj);

        // Format using appropriate formatter
        return _csvFormatter.Format(data);
    }

    private Dictionary<string, object> ObjectToDictionary<T>(T obj)
    {
        var result = new Dictionary<string, object>();
        var type = typeof(T);

        foreach (var prop in _definition.Output.Properties)
        {
            var property = type.GetProperty(prop.Name);
            if (property != null)
            {
                result[prop.Name] = property.GetValue(obj);
            }
        }

        return result;
    }
}
```

**Day 4-5: Testing & Round-Trip**
```csharp
// Test round-trip
var engine = new ProtocolEngine("TScaleNHB-Full-Definition.json");
var serializer = new ProtocolSerializer("TScaleNHB-Full-Definition.json");

// Parse
byte[] original = Encoding.ASCII.GetBytes("ST,GS    20.7g  \r\n");
var parsed = engine.Parse<TScaleNHBData>(original);

// Serialize
byte[] serialized = serializer.Serialize(parsed);

// Compare
Assert.AreEqual(original, serialized); // Should match!
```

---

## 🎯 Success Metrics

### Week 1 Success
- [ ] Generated definitions have validation rules
- [ ] Generated definitions have test cases
- [ ] Generated definitions have output mapping
- [ ] Generated definitions ≥90% match manual quality

### Week 2 Success
- [ ] Can parse TScaleNHB using JSON definition
- [ ] Can parse at least 3 simple CSV devices
- [ ] Extracted data matches expected values
- [ ] All test cases pass

### Week 3 Success
- [ ] Can serialize TScaleNHB back to protocol
- [ ] Round-trip test passes (parse → serialize → compare)
- [ ] Works for at least 3 devices
- [ ] Byte-exact match with original

---

## 📁 Files to Create (Summary)

### Week 1 (Enhanced Generation)
1. `Analyzers/AdvancedPatternRecognizer.cs`
2. `Generators/ValidationRuleGenerator.cs`
3. `Generators/TestCaseGenerator.cs`
4. `Generators/OutputMappingGenerator.cs`
5. Update `Generators/DefinitionGenerator.cs`

### Week 2 (Runtime Engine)
1. `Runtime/DefinitionLoader.cs`
2. `Runtime/ProtocolEngine.cs`
3. `Runtime/Parsers/CSVParser.cs`
4. `Runtime/Parsers/PackageParser.cs`
5. `Runtime/FieldExtractor.cs`

### Week 3 (Serializer)
1. `Serialization/ProtocolSerializer.cs`
2. `Serialization/Formatters/CSVFormatter.cs`
3. `Serialization/Formatters/PackageFormatter.cs`
4. `Serialization/FieldFormatter.cs`

---

## 🔗 References

- **Full Tracker:** `PROTOCOL-ANALYZER-COMPLETION-TRACKER.md`
- **Design Doc:** `02-Protocol-Analyzer-Tool.md`
- **Manual Definitions:** `*-Full-Definition.json` files
- **Existing Code:** `09.App/NLib.Serial.Protocol.Analyzer/`

---

**Start Here:** Week 1, Day 1 - Advanced Pattern Recognition
**Next Milestone:** Complete enhanced definition generation by end of Week 1
**End Goal:** Full bidirectional protocol tool (analyze, parse, serialize)
