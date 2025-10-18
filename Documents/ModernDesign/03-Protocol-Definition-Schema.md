# Protocol Definition Schema Specification

**Document:** JSON Schema for Protocol Definitions
**Version:** 1.0
**Date:** 2025-10-18
**Status:** Design Phase

---

## Table of Contents
1. [Schema Overview](#schema-overview)
2. [Complete JSON Schema](#complete-json-schema)
3. [Field Reference](#field-reference)
4. [Parsing Strategies](#parsing-strategies)
5. [Examples by Complexity](#examples-by-complexity)

---

## Schema Overview

### Purpose

The Protocol Definition Schema provides a standardized, declarative format for describing serial device communication protocols without writing C# code.

### Design Principles

1. **Self-Documenting** - Definitions explain the protocol
2. **Validate-able** - JSON Schema validation before runtime
3. **Extensible** - Support new strategies without schema changes
4. **Human-Readable** - Easy to understand and modify
5. **Machine-Parseable** - Unambiguous for parsing engine

### File Naming Convention

```
[DeviceName].json           - Primary definition
[DeviceName]-v2.json        - Version 2 of protocol
[DeviceName]-alternate.json - Alternative configuration
```

---

## Complete JSON Schema

### Root Schema Structure

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "title": "Serial Device Protocol Definition",
  "version": "1.0",
  "type": "object",
  "required": ["deviceInfo", "protocol"],
  "properties": {
    "deviceInfo": { "$ref": "#/definitions/DeviceInfo" },
    "protocol": { "$ref": "#/definitions/Protocol" },
    "parsing": { "$ref": "#/definitions/Parsing" },
    "serialization": { "$ref": "#/definitions/Serialization" },
    "validation": { "$ref": "#/definitions/Validation" },
    "testCases": {
      "type": "array",
      "items": { "$ref": "#/definitions/TestCase" }
    }
  }
}
```

---

### Device Info Section

```json
"deviceInfo": {
  "type": "object",
  "required": ["name"],
  "properties": {
    "name": {
      "type": "string",
      "description": "Unique device identifier"
    },
    "manufacturer": {
      "type": "string",
      "description": "Device manufacturer"
    },
    "model": {
      "type": "string",
      "description": "Device model number"
    },
    "description": {
      "type": "string",
      "description": "Human-readable description"
    },
    "category": {
      "type": "string",
      "enum": ["scale", "meter", "sensor", "controller", "other"],
      "description": "Device category"
    },
    "version": {
      "type": "string",
      "pattern": "^\\d+\\.\\d+(\\.\\d+)?$",
      "description": "Protocol version (semver)"
    }
  }
}
```

**Example:**
```json
{
  "deviceInfo": {
    "name": "TScaleNHB",
    "manufacturer": "T&T",
    "model": "NHB",
    "description": "Weight scale with CSV-like protocol",
    "category": "scale",
    "version": "1.0"
  }
}
```

---

### Protocol Section

```json
"protocol": {
  "type": "object",
  "required": ["type", "encoding", "terminator", "fields"],
  "properties": {
    "type": {
      "type": "string",
      "enum": ["streaming", "command-response", "request-reply"],
      "description": "Communication pattern"
    },
    "format": {
      "type": "string",
      "enum": ["csv", "fixed-width", "binary", "mixed", "custom"],
      "description": "Data format type"
    },
    "encoding": {
      "type": "string",
      "enum": ["ASCII", "UTF-8", "UTF-16", "binary"],
      "default": "ASCII"
    },
    "terminator": {
      "type": "string",
      "description": "Message terminator (\\r\\n, \\n, etc.)"
    },
    "terminatorBytes": {
      "type": "string",
      "pattern": "^([0-9A-F]{2}\\s?)+$",
      "description": "Hex representation of terminator"
    },
    "fields": {
      "type": "array",
      "items": { "$ref": "#/definitions/Field" },
      "minItems": 1
    },
    "updateRate": {
      "type": "string",
      "description": "How often device sends data"
    }
  }
}
```

---

### Field Definition

```json
"Field": {
  "type": "object",
  "required": ["name", "position", "type"],
  "properties": {
    "name": {
      "type": "string",
      "pattern": "^[A-Za-z][A-Za-z0-9_]*$",
      "description": "Field name (valid C# property name)"
    },
    "position": {
      "type": "integer",
      "minimum": 0,
      "description": "Field position (0-indexed)"
    },
    "type": {
      "type": "string",
      "enum": ["string", "decimal", "integer", "datetime", "boolean", "byte[]"],
      "description": "Data type"
    },
    "length": {
      "type": "integer",
      "minimum": 0,
      "description": "Fixed length (for fixed-width)"
    },
    "separator": {
      "type": "string",
      "description": "Separator after this field"
    },
    "format": {
      "type": "string",
      "description": "Format string (e.g., F1, yyyy-MM-dd)"
    },
    "alignment": {
      "type": "string",
      "enum": ["left", "right", "center"],
      "default": "left"
    },
    "padding": {
      "type": "string",
      "maxLength": 1,
      "description": "Padding character"
    },
    "unit": {
      "type": "string",
      "description": "Unit of measurement (g, kg, pH, etc.)"
    },
    "unitAttached": {
      "type": "boolean",
      "default": false,
      "description": "Is unit attached to value? (20.7g vs 20.7 g)"
    },
    "values": {
      "type": "array",
      "items": { "type": "string" },
      "description": "Allowed values (for enums)"
    },
    "isConstant": {
      "type": "boolean",
      "default": false,
      "description": "Does field have constant value?"
    },
    "min": {
      "type": "number",
      "description": "Minimum value (for numeric types)"
    },
    "max": {
      "type": "number",
      "description": "Maximum value (for numeric types)"
    },
    "description": {
      "type": "string",
      "description": "Field documentation"
    },
    "confidence": {
      "type": "number",
      "minimum": 0,
      "maximum": 100,
      "description": "Analyzer confidence (0-100%)"
    }
  }
}
```

---

### Parsing Section

```json
"parsing": {
  "type": "object",
  "required": ["strategy"],
  "properties": {
    "strategy": {
      "type": "string",
      "enum": ["split", "regex", "fixed-width", "state-machine", "custom"],
      "description": "Parsing strategy to use"
    },
    "delimiter": {
      "type": "string",
      "description": "Delimiter for split strategy"
    },
    "trim": {
      "type": "boolean",
      "default": true,
      "description": "Trim whitespace from fields"
    },
    "removeEmpty": {
      "type": "boolean",
      "default": false,
      "description": "Remove empty entries after split"
    },
    "pattern": {
      "type": "string",
      "description": "Regex pattern for regex strategy"
    },
    "groups": {
      "type": "array",
      "items": { "type": "string" },
      "description": "Named groups in regex"
    },
    "fieldPositions": {
      "type": "array",
      "items": {
        "type": "object",
        "properties": {
          "name": { "type": "string" },
          "start": { "type": "integer" },
          "length": { "type": "integer" }
        }
      },
      "description": "Field positions for fixed-width"
    },
    "states": {
      "type": "array",
      "items": { "$ref": "#/definitions/State" },
      "description": "States for state machine"
    },
    "steps": {
      "type": "array",
      "items": { "type": "string" },
      "description": "Human-readable parsing steps"
    },
    "errorHandling": {
      "type": "object",
      "properties": {
        "onParseError": {
          "type": "string",
          "enum": ["log", "throw", "ignore"],
          "default": "log"
        },
        "onInvalidValue": {
          "type": "string",
          "enum": ["useDefault", "throw", "useNull"],
          "default": "useDefault"
        },
        "onMissingField": {
          "type": "string",
          "enum": ["useDefault", "throw", "useNull"],
          "default": "useDefault"
        }
      }
    }
  }
}
```

---

## Field Reference

### Field Type Mapping

| JSON Type | C# Type | Example Values |
|-----------|---------|----------------|
| `string` | `string` | "ST", "GS", "Stable" |
| `decimal` | `decimal` | 20.7, 245.6, 0.0001 |
| `integer` | `int` | 1, 42, -5 |
| `datetime` | `DateTime` | "2023-11-07", "17:19:38" |
| `boolean` | `bool` | true, false |
| `byte[]` | `byte[]` | [0x53, 0x54] |

### Field Properties Reference

#### Basic Properties

- **name** - Property name in generated data class
- **position** - 0-indexed field position after splitting
- **type** - Data type (see table above)
- **description** - Documentation for this field

#### Format Properties

- **format** - .NET format string
  - Decimal: "F1", "F2", "F3" (decimal places)
  - DateTime: "yyyy-MM-dd", "HH:mm:ss"
  - Integer: "D4" (zero-padded)

- **alignment** - Text alignment
  - "left" - Left-aligned
  - "right" - Right-aligned (common for numbers)
  - "center" - Center-aligned

- **padding** - Padding character
  - " " - Space (most common)
  - "0" - Zero padding
  - "-" - Dash, etc.

#### Delimiter Properties

- **separator** - Character(s) after field
  - "," - Comma
  - " " - Space
  - "\t" - Tab
  - null - No separator (end of line)

#### Value Constraints

- **unit** - Unit of measurement
  - "g", "kg", "pH", "°C", "pcs"

- **unitAttached** - Is unit part of value?
  - true: `20.7g` (unit attached)
  - false: `20.7 g` (space-separated)

- **values** - Allowed enumeration values
  - ["ST", "US"] - Only these values allowed
  - Used for validation

- **isConstant** - Always same value?
  - true - Field never changes (e.g., mode "GS")
  - false - Field varies

- **min/max** - Numeric range constraints

---

## Parsing Strategies

### Strategy 1: Split Parsing

**When to Use:**
- CSV-like protocols (comma, semicolon)
- Space-delimited protocols
- Tab-separated values

**Configuration:**
```json
{
  "parsing": {
    "strategy": "split",
    "delimiter": ",",
    "trim": true,
    "removeEmpty": false
  }
}
```

**Example: TScaleQHW**
```
Input:  "ST,GS,   245.6 g\r\n"
Split:  ["ST", "GS", "   245.6 g"]
Trim:   ["ST", "GS", "245.6 g"]
Result: {Status="ST", Mode="GS", Weight=245.6}
```

---

### Strategy 2: Regex Parsing

**When to Use:**
- Complex patterns
- Mixed formats in single line
- Extract specific patterns

**Configuration:**
```json
{
  "parsing": {
    "strategy": "regex",
    "pattern": "(?<pH>\\d+\\.\\d+)pH\\s+(?<temp>\\d+\\.\\d+)°C\\s+(?<mode>\\w+)",
    "groups": ["pH", "temp", "mode"]
  }
}
```

**Example: PHMeter**
```
Input:  "3.01pH 25.5°C ATC\r\n"
Regex:  Groups: pH=3.01, temp=25.5, mode=ATC
Result: {pH=3.01, TempC=25.5, Mode="ATC"}
```

---

### Strategy 3: Fixed-Width Parsing

**When to Use:**
- Fixed column positions
- No delimiters
- Legacy mainframe-style formats

**Configuration:**
```json
{
  "parsing": {
    "strategy": "fixed-width",
    "fieldPositions": [
      {"name": "Weight", "start": 0, "length": 8},
      {"name": "Unit", "start": 9, "length": 2},
      {"name": "Status", "start": 16, "length": 1}
    ]
  }
}
```

**Example:**
```
Input:  "   20.70 kg    G\r\n"
        01234567890123456789
Extract: [0-7]="   20.70", [9-10]="kg", [16]="G"
Result: {Weight=20.70, Unit="kg", Status="G"}
```

---

### Strategy 4: State Machine Parsing

**When to Use:**
- Multi-line protocols
- Package-based protocols
- Context-dependent parsing

**Configuration:**
```json
{
  "parsing": {
    "strategy": "state-machine",
    "states": [
      {
        "name": "header",
        "pattern": "^KJIK000",
        "action": "startPackage",
        "next": "date"
      },
      {
        "name": "date",
        "pattern": "\\d{4}-\\d{2}-\\d{2}",
        "field": "Date",
        "type": "datetime",
        "format": "yyyy-MM-dd",
        "next": "time"
      },
      {
        "name": "time",
        "pattern": "\\d{2}:\\d{2}:\\d{2}",
        "field": "Time",
        "type": "datetime",
        "format": "HH:mm:ss",
        "next": "tare"
      }
    ]
  }
}
```

**Example: JIK6CAB Multi-Line**
```
^KJIK000          ← State: header
2023-11-07        ← State: date (extract)
17:19:38          ← State: time (extract)
  0.00 kg         ← State: tare (extract)
...
```

---

## Examples by Complexity

### Simple: TScaleNHB (CSV-like, Single Comma)

```json
{
  "deviceInfo": {
    "name": "TScaleNHB",
    "category": "scale"
  },
  "protocol": {
    "type": "streaming",
    "format": "csv",
    "encoding": "ASCII",
    "terminator": "\\r\\n",
    "fields": [
      {
        "name": "Status",
        "position": 0,
        "type": "string",
        "separator": ",",
        "values": ["ST", "US"]
      },
      {
        "name": "Mode",
        "position": 1,
        "type": "string",
        "values": ["GS"]
      },
      {
        "name": "Weight",
        "position": 2,
        "type": "decimal",
        "unit": "g",
        "unitAttached": true
      }
    ]
  },
  "parsing": {
    "strategy": "split",
    "delimiter": ","
  }
}
```

---

### Medium: DEFENDER3000 (Space-Delimited)

```json
{
  "deviceInfo": {
    "name": "DEFENDER3000",
    "category": "scale"
  },
  "protocol": {
    "type": "streaming",
    "format": "fixed-width",
    "encoding": "ASCII",
    "terminator": "\\r\\n",
    "fields": [
      {
        "name": "W",
        "position": 0,
        "type": "decimal",
        "format": "F3",
        "alignment": "right",
        "padding": " "
      },
      {
        "name": "Unit",
        "position": 1,
        "type": "string",
        "values": ["kg"]
      },
      {
        "name": "O",
        "position": 2,
        "type": "string",
        "values": ["G", "N", "?G", "?N"]
      }
    ]
  },
  "parsing": {
    "strategy": "split",
    "delimiter": " ",
    "removeEmpty": true
  }
}
```

---

### Complex: PHMeter (Regex Pattern)

```json
{
  "deviceInfo": {
    "name": "PHMeter",
    "category": "meter"
  },
  "protocol": {
    "type": "streaming",
    "format": "custom",
    "encoding": "ASCII",
    "terminator": "\\r\\n",
    "fields": [
      {"name": "pH", "type": "decimal"},
      {"name": "TempC", "type": "decimal"},
      {"name": "Mode", "type": "string"}
    ]
  },
  "parsing": {
    "strategy": "regex",
    "pattern": "(?<pH>\\d+\\.\\d+)pH\\s+(?<temp>\\d+\\.\\d+)°C\\s+(?<mode>\\w+)",
    "groups": ["pH", "temp", "mode"]
  }
}
```

---

### Very Complex: JIK6CAB (State Machine)

```json
{
  "deviceInfo": {
    "name": "JIK6CAB",
    "category": "scale"
  },
  "protocol": {
    "type": "command-response",
    "format": "mixed",
    "encoding": "ASCII",
    "terminator": "\\r\\n"
  },
  "parsing": {
    "strategy": "state-machine",
    "states": [
      {
        "name": "header",
        "pattern": "^KJIK000",
        "action": "reset",
        "next": "date"
      },
      {
        "name": "date",
        "field": "Date",
        "type": "datetime",
        "format": "yyyy-MM-dd",
        "next": "time"
      },
      {
        "name": "time",
        "field": "Time",
        "type": "datetime",
        "format": "HH:mm:ss",
        "next": "tare"
      },
      {
        "name": "tare",
        "field": "TW",
        "type": "decimal",
        "pattern": "\\s*(\\d+\\.\\d+)\\s*kg",
        "next": "gross"
      }
    ]
  }
}
```

---

## Validation Schema

### JSON Schema for Validation

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "title": "Protocol Definition Validator",
  "definitions": {
    "Field": {
      "allOf": [
        {"required": ["name", "position", "type"]},
        {
          "if": {"properties": {"type": {"const": "decimal"}}},
          "then": {"properties": {"format": {"pattern": "^F\\d+$"}}}
        },
        {
          "if": {"properties": {"type": {"const": "datetime"}}},
          "then": {"required": ["format"]}
        }
      ]
    }
  }
}
```

### Validation Rules

1. **Required Fields:** name, position, type
2. **Valid Types:** Must be from enum list
3. **Format Consistency:** Decimal requires F format, DateTime requires date format
4. **Unique Positions:** No duplicate field positions
5. **Valid Regex:** Regex patterns must compile
6. **Enum Values:** If values specified, must be non-empty array

---

## Best Practices

### Naming Conventions

```
✓ Good:
- "Status", "Weight", "Temperature"
- PascalCase for field names
- Short but descriptive

✗ Bad:
- "s", "w", "temp"
- snake_case, kebab-case
- Too verbose: "StatusIndicatorValue"
```

### Documentation

```json
{
  "fields": [
    {
      "name": "Status",
      "description": "Stability indicator - ST means stable, US means unstable"
      // ✓ Clear documentation
    }
  ]
}
```

### Performance

```json
{
  "parsing": {
    "trim": false,  // ✓ Disable if not needed
    "errorHandling": {
      "onParseError": "ignore"  // ✗ Don't ignore in development!
    }
  }
}
```

---

## Next Steps

1. → Create complete example definitions for all 9 devices
2. → Implement JSON Schema validator
3. → Build protocol engine that reads these definitions
4. → Test with real log files

---

## Related Documents

- [System Architecture](01-System-Architecture.md)
- [Protocol Analyzer Tool](02-Protocol-Analyzer-Tool.md)
- [Protocol Examples](04-Protocol-Examples.md)
