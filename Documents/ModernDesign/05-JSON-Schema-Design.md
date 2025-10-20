# JSON Schema Design

## Protocol Definition File Format

**Document Version**: 2.0
**Last Updated**: 2025-10-19
**Status**: Design Phase - Complete

**Related Documents**:
- **00-Requirements-Specification.md** - Requirements for definition files
- **01-Production-Code-Analysis.md** - Real protocol examples
- **04-Data-Models-Design.md** - C# data models that map to this schema

---

## Table of Contents

1. [Overview](#overview)
2. [Complete JSON Schema](#complete-json-schema)
3. [Field Naming Rules](#field-naming-rules)
4. [Bidirectional Field Definitions](#bidirectional-field-definitions)
5. [Examples by Device](#examples-by-device)
6. [Validation Rules](#validation-rules)

---

## Overview

The Protocol Definition File is a JSON document that describes how to:
- **Parse** bytes received from a serial device (for NTerminal<T>)
- **Serialize** data back to bytes for transmission (for NDevice<T>)

### Design Principles

1. **Bidirectional**: Every field has both parse and serialize configuration
2. **Self-Documenting**: Include descriptions, sample values, confidence scores
3. **Version Controlled**: Git-friendly format
4. **Validation-Ready**: Include validation rules in schema
5. **Human-Readable**: Easy to edit manually if needed

---

## Complete JSON Schema

### Root Schema

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "title": "NSerialCommTools Protocol Definition",
  "description": "Protocol definition for serial device communication",
  "type": "object",
  "required": ["deviceName", "version", "encoding", "fields"],

  "properties": {
    "deviceName": {
      "type": "string",
      "description": "Name of the device",
      "minLength": 1
    },

    "version": {
      "type": "string",
      "description": "Definition version (e.g., '1.0')",
      "pattern": "^\\d+\\.\\d+$"
    },

    "generatedDate": {
      "type": "string",
      "format": "date-time",
      "description": "When this definition was generated"
    },

    "description": {
      "type": "string",
      "description": "Optional description or notes"
    },

    "encoding": {
      "type": "string",
      "enum": ["ASCII", "UTF-8", "UTF-16"],
      "description": "Text encoding for protocol"
    },

    "messageTerminator": {
      "type": "string",
      "description": "Message terminator (hex string, e.g., '0D 0A')"
    },

    "messageStructure": {
      "type": "string",
      "enum": ["single-line", "multi-line-frame", "multi-line-block", "variable-length"],
      "description": "Message structure type"
    },

    "messageHeader": {
      "type": "string",
      "description": "Header pattern for multi-line messages"
    },

    "messageFooter": {
      "type": "string",
      "description": "Footer pattern for multi-line messages"
    },

    "fields": {
      "type": "array",
      "minItems": 1,
      "items": { "$ref": "#/definitions/field" }
    },

    "messages": {
      "type": "array",
      "items": { "$ref": "#/definitions/message" }
    },

    "commands": {
      "type": "array",
      "items": { "$ref": "#/definitions/command" }
    }
  },

  "definitions": {
    "field": {
      "type": "object",
      "required": ["name", "dataType", "position"],
      "properties": {
        "name": {
          "type": "string",
          "pattern": "^[a-zA-Z_][a-zA-Z0-9_]*$",
          "description": "Field name (must be valid C# identifier)"
        },
        "dataType": {
          "type": "string",
          "enum": ["int", "decimal", "double", "string", "char", "datetime", "timespan", "bool", "binary"],
          "description": "Data type"
        },
        "position": {
          "type": "integer",
          "minimum": 0,
          "description": "Field position/order (0-based)"
        },
        "required": {
          "type": "boolean",
          "default": true,
          "description": "Is this field required?"
        },
        "description": {
          "type": "string",
          "description": "Field description or notes"
        },
        "parse": {
          "$ref": "#/definitions/parseConfig"
        },
        "serialize": {
          "$ref": "#/definitions/serializeConfig"
        }
      }
    },

    "parseConfig": {
      "type": "object",
      "required": ["method"],
      "properties": {
        "method": {
          "type": "string",
          "enum": ["regex", "fixed-position", "delimited", "header-byte"],
          "description": "Parse method"
        },
        "pattern": {
          "type": "string",
          "description": "Regex pattern (if method=regex)"
        },
        "group": {
          "type": "integer",
          "minimum": 0,
          "description": "Regex capture group (if method=regex)"
        },
        "offset": {
          "type": "integer",
          "minimum": 0,
          "description": "Byte offset (if method=fixed-position)"
        },
        "length": {
          "type": "integer",
          "minimum": 1,
          "description": "Byte length (if method=fixed-position)"
        },
        "delimiter": {
          "type": "string",
          "description": "Delimiter (if method=delimited)"
        },
        "index": {
          "type": "integer",
          "minimum": 0,
          "description": "Index after split (if method=delimited)"
        },
        "trim": {
          "type": "boolean",
          "default": true,
          "description": "Trim whitespace?"
        },
        "format": {
          "type": "string",
          "description": "Format string (e.g., DateTime format)"
        }
      }
    },

    "serializeConfig": {
      "type": "object",
      "properties": {
        "format": {
          "type": "string",
          "description": "Format string (e.g., 'F3', 'D2', 'yyyy-MM-dd')"
        },
        "padding": {
          "type": "string",
          "enum": ["none", "left", "right"],
          "default": "none",
          "description": "Padding direction"
        },
        "paddingChar": {
          "type": "string",
          "maxLength": 1,
          "default": " ",
          "description": "Padding character"
        },
        "width": {
          "type": "integer",
          "minimum": 1,
          "description": "Total width after padding"
        },
        "alignment": {
          "type": "string",
          "enum": ["left", "right", "center"],
          "default": "left",
          "description": "Text alignment"
        }
      }
    },

    "message": {
      "type": "object",
      "required": ["messageId", "messageType"],
      "properties": {
        "messageId": {
          "type": "string",
          "description": "Message identifier"
        },
        "messageType": {
          "type": "string",
          "enum": ["request", "response", "event", "command"],
          "description": "Message type"
        },
        "pattern": {
          "type": "string",
          "description": "Pattern to identify this message type"
        },
        "fieldNames": {
          "type": "array",
          "items": { "type": "string" },
          "description": "Fields in this message"
        },
        "terminator": {
          "type": "string",
          "description": "Message-specific terminator"
        }
      }
    },

    "command": {
      "type": "object",
      "required": ["name", "sendBytes"],
      "properties": {
        "name": {
          "type": "string",
          "description": "Command name"
        },
        "sendBytes": {
          "type": "string",
          "pattern": "^([0-9A-Fa-f]{2}\\s?)+$",
          "description": "Hex bytes to send (e.g., '52 45 51 0D')"
        },
        "expectResponse": {
          "type": "boolean",
          "default": false,
          "description": "Does command expect a response?"
        },
        "responseTimeout": {
          "type": "integer",
          "minimum": 0,
          "description": "Response timeout (milliseconds)"
        },
        "description": {
          "type": "string",
          "description": "Command description"
        }
      }
    }
  }
}
```

---

## Field Naming Rules

### Name Property Requirements

**Single source of truth**: Field names start as auto-generated, user edits directly.

```
1. Analyzer creates: { "name": "Field1", ... }
2. User edits:       Field1 → NetWeight
3. Save to JSON:     { "name": "NetWeight", ... }
```

### Validation Rules

1. **Must be valid C# identifier**:
   - Pattern: `^[a-zA-Z_][a-zA-Z0-9_]*$`
   - Start with letter or underscore
   - Contain only letters, digits, underscores

2. **Cannot be C# keyword**:
   - class, int, string, void, public, private, return, etc.

3. **Must be unique** within definition

4. **Recommended**: PascalCase for consistency

### Smart Name Suggestions

Algorithm for suggesting names based on sample values:

| Sample Values | Data Type | Suggested Name |
|--------------|-----------|----------------|
| 1.640, 1.645, 1.650 | decimal | Weight |
| kg, g, lb | string | Unit |
| N, G, S | char | Status |
| 2023-11-07 | date | Date or MeasurementDate |
| 17:19:38 | time | Time or MeasurementTime |
| 0, 1, 2, 3 | int | Counter or Index |
| true, false | bool | Flag or IsEnabled |

---

## Bidirectional Field Definitions

Every field must support **both directions**:

### Parse Direction (Device → Application)

How **NTerminal<T>** extracts values from incoming bytes.

**Methods**:
1. **regex**: Use regex pattern with capture group
2. **fixed-position**: Extract from fixed byte offset/length
3. **delimited**: Split by delimiter and take index
4. **header-byte**: Switch on first byte (like TFO1)

### Serialize Direction (Application → Device)

How **NDevice<T>** converts T properties to bytes.

**Configuration**:
- Format string (e.g., "F3" for 3 decimal places)
- Padding (left/right/none)
- Width and alignment

### Example: NetWeight Field

```json
{
  "name": "NetWeight",
  "dataType": "decimal",
  "position": 0,
  "required": true,
  "description": "Net weight value in kg",

  "parse": {
    "method": "regex",
    "pattern": "^([\\d.]+)",
    "group": 1,
    "trim": true
  },

  "serialize": {
    "format": "F3",
    "padding": "left",
    "paddingChar": " ",
    "width": 8,
    "alignment": "right"
  }
}
```

**Usage**:

```csharp
// NTerminal<T> - Parse
// Incoming bytes: "   1.640 kg    N\r\n"
// Pattern: ^([\d.]+) extracts "1.640"
// Result: data.NetWeight = 1.640m

// NDevice<T> - Serialize
// data.NetWeight = 1.640m
// Format: "F3" = "1.640"
// Pad left to width 8: "   1.640"
// Output bytes: "   1.640"
```

---

## Examples by Device

### Example 1: CordDEFENDER3000 (Simple Space-Delimited)

**Protocol**: `   0.360 kg    G\r\n`

**Production Code Strategy**: String.Split(" ", RemoveEmptyEntries)

```json
{
  "deviceName": "CordDEFENDER3000",
  "version": "1.0",
  "generatedDate": "2025-10-19T12:00:00Z",
  "description": "Cord weight scale - simple space-delimited protocol",
  "encoding": "ASCII",
  "messageTerminator": "0D 0A",
  "messageStructure": "single-line",

  "fields": [
    {
      "name": "Weight",
      "dataType": "decimal",
      "position": 0,
      "required": true,
      "description": "Weight value",

      "parse": {
        "method": "delimited",
        "delimiter": " ",
        "index": 0,
        "trim": true
      },

      "serialize": {
        "format": "F3",
        "padding": "left",
        "paddingChar": " ",
        "width": 8,
        "alignment": "right"
      }
    },

    {
      "name": "Unit",
      "dataType": "string",
      "position": 1,
      "required": true,
      "description": "Unit of measurement (kg, g)",

      "parse": {
        "method": "delimited",
        "delimiter": " ",
        "index": 1,
        "trim": true
      },

      "serialize": {
        "format": null,
        "padding": "none",
        "width": 2
      }
    },

    {
      "name": "Status",
      "dataType": "char",
      "position": 2,
      "required": true,
      "description": "Status code (N=Net, G=Gross)",

      "parse": {
        "method": "delimited",
        "delimiter": " ",
        "index": 2,
        "trim": true
      },

      "serialize": {
        "format": null,
        "padding": "left",
        "paddingChar": " ",
        "width": 5,
        "alignment": "right"
      }
    }
  ]
}
```

---

### Example 2: WeightQA (Nested Delimiters)

**Protocol**: `+007.12/3 G S\r\n`

**Production Code Strategy**: Split("/") then Split(" "), reconstruct weight

```json
{
  "deviceName": "WeightQA",
  "version": "1.0",
  "generatedDate": "2025-10-19T12:00:00Z",
  "description": "Quality assurance weight scale - nested delimiter protocol",
  "encoding": "ASCII",
  "messageTerminator": "0D 0A",
  "messageStructure": "single-line",

  "fields": [
    {
      "name": "Weight",
      "dataType": "decimal",
      "position": 0,
      "required": true,
      "description": "Weight value (reconstructed from int/frac parts)",

      "parse": {
        "method": "regex",
        "pattern": "^([+-]?\\d+\\.\\d+)/(\\d)",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": "F2",
        "padding": "none"
      }
    },

    {
      "name": "Fraction",
      "dataType": "int",
      "position": 1,
      "required": true,
      "description": "Fractional part after slash",

      "parse": {
        "method": "regex",
        "pattern": "/(\\d)",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": "D",
        "padding": "none"
      }
    },

    {
      "name": "Unit",
      "dataType": "string",
      "position": 2,
      "required": true,
      "description": "Unit (G=grams)",

      "parse": {
        "method": "regex",
        "pattern": "/\\d\\s+([A-Z])",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": null,
        "padding": "none"
      }
    },

    {
      "name": "Mode",
      "dataType": "string",
      "position": 3,
      "required": true,
      "description": "Mode (S=Stable)",

      "parse": {
        "method": "regex",
        "pattern": "([A-Z])\\s*$",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": null,
        "padding": "none"
      }
    }
  ]
}
```

---

### Example 3: TFO1 (Header Byte + Fixed Position)

**Protocol**: Multi-line frame with header byte switch

```
F      0.0\r
H      0.0\r
Q      0.0\r
...
B<0x83>\r
C20<0xF4> 02<0xF3> 2023<0xF2> MON 09:20AM\r
V<0x31>\r\n
```

**Production Code Strategy**: Switch on first byte, GetString(offset, length)

```json
{
  "deviceName": "TFO1",
  "version": "1.0",
  "generatedDate": "2025-10-19T12:00:00Z",
  "description": "TFO1 device - complex multi-line protocol with binary bytes",
  "encoding": "ASCII",
  "messageTerminator": "0D",
  "messageStructure": "multi-line-frame",
  "messageHeader": "V31 0D 0A",
  "messageFooter": "56 0D 0A",

  "fields": [
    {
      "name": "F",
      "dataType": "decimal",
      "position": 0,
      "required": true,
      "description": "F field value",

      "parse": {
        "method": "fixed-position",
        "offset": 1,
        "length": 9,
        "trim": true
      },

      "serialize": {
        "format": "F1",
        "padding": "left",
        "paddingChar": " ",
        "width": 9,
        "alignment": "right"
      }
    },

    {
      "name": "H",
      "dataType": "decimal",
      "position": 1,
      "required": true,
      "description": "H field value",

      "parse": {
        "method": "fixed-position",
        "offset": 1,
        "length": 9,
        "trim": true
      },

      "serialize": {
        "format": "F1",
        "padding": "left",
        "paddingChar": " ",
        "width": 9,
        "alignment": "right"
      }
    },

    {
      "name": "Q",
      "dataType": "decimal",
      "position": 2,
      "required": true,
      "description": "Q field value",

      "parse": {
        "method": "fixed-position",
        "offset": 1,
        "length": 9,
        "trim": true
      },

      "serialize": {
        "format": "F1",
        "padding": "left",
        "paddingChar": " ",
        "width": 9,
        "alignment": "right"
      }
    },

    {
      "name": "A",
      "dataType": "decimal",
      "position": 5,
      "required": true,
      "description": "A field value (main weight)",

      "parse": {
        "method": "fixed-position",
        "offset": 1,
        "length": 9,
        "trim": true
      },

      "serialize": {
        "format": "F1",
        "padding": "left",
        "paddingChar": " ",
        "width": 9,
        "alignment": "right"
      }
    },

    {
      "name": "B",
      "dataType": "binary",
      "position": 9,
      "required": true,
      "description": "B marker (binary byte 0x83)",

      "parse": {
        "method": "fixed-position",
        "offset": 1,
        "length": 1
      },

      "serialize": {
        "format": "X2"
      }
    },

    {
      "name": "Timestamp",
      "dataType": "datetime",
      "position": 10,
      "required": true,
      "description": "Date/time with special byte separators",

      "parse": {
        "method": "regex",
        "pattern": "^C(\\d{2}).\\s(\\d{2}).\\s(\\d{4}).\\s([A-Z]{3})\\s(\\d{2}):(\\d{2})([AP]M)",
        "group": 0,
        "format": "dd MM yyyy ddd HH:mmtt"
      },

      "serialize": {
        "format": "dd MM yyyy ddd HH:mmtt"
      }
    },

    {
      "name": "Version",
      "dataType": "binary",
      "position": 11,
      "required": true,
      "description": "Version byte",

      "parse": {
        "method": "fixed-position",
        "offset": 1,
        "length": 1
      },

      "serialize": {
        "format": "X2"
      }
    }
  ],

  "messages": [
    {
      "messageId": "DataFrame",
      "messageType": "response",
      "pattern": "^[FHQXA01245B]",
      "fieldNames": ["F", "H", "Q", "X", "A", "W0", "W4", "W1", "W2", "B", "Timestamp", "Version"],
      "terminator": "0D"
    }
  ]
}
```

---

### Example 4: PHMeter (Content-Based Multi-Line)

**Protocol**: Multi-line with different content types

```
3.01pH 25.5°C ATC\r\n
20-Feb-2023\r\n
11:11\r\n
 \r\n
3.01pH\r\n
25.5°C ATC\r\n
Auto EP Standard\r\n
Blank\r\n
```

**Production Code Strategy**: if/else on line content (Contains("pH"), Contains("-"), Contains(":"))

```json
{
  "deviceName": "PHMeter",
  "version": "1.0",
  "generatedDate": "2025-10-19T12:00:00Z",
  "description": "pH Meter - content-based multi-line protocol",
  "encoding": "ASCII",
  "messageTerminator": "0D 0A",
  "messageStructure": "multi-line-block",

  "fields": [
    {
      "name": "pH",
      "dataType": "decimal",
      "position": 0,
      "required": true,
      "description": "pH value",

      "parse": {
        "method": "regex",
        "pattern": "^(\\d+\\.\\d+)pH",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": "F2",
        "padding": "none"
      }
    },

    {
      "name": "Temperature",
      "dataType": "decimal",
      "position": 1,
      "required": true,
      "description": "Temperature in °C",

      "parse": {
        "method": "regex",
        "pattern": "(\\d+\\.\\d+).C",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": "F1",
        "padding": "none"
      }
    },

    {
      "name": "Mode",
      "dataType": "string",
      "position": 2,
      "required": true,
      "description": "Measurement mode (ATC=Auto Temp Compensation)",

      "parse": {
        "method": "regex",
        "pattern": "([A-Z]{3})$",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": null,
        "padding": "none"
      }
    },

    {
      "name": "Date",
      "dataType": "datetime",
      "position": 3,
      "required": false,
      "description": "Measurement date",

      "parse": {
        "method": "regex",
        "pattern": "^(\\d{2}-[A-Za-z]{3}-\\d{4})$",
        "group": 1,
        "format": "dd-MMM-yyyy"
      },

      "serialize": {
        "format": "dd-MMM-yyyy"
      }
    },

    {
      "name": "Time",
      "dataType": "timespan",
      "position": 4,
      "required": false,
      "description": "Measurement time",

      "parse": {
        "method": "regex",
        "pattern": "^(\\d{2}:\\d{2})$",
        "group": 1,
        "format": "HH:mm"
      },

      "serialize": {
        "format": "HH:mm"
      }
    },

    {
      "name": "Method",
      "dataType": "string",
      "position": 5,
      "required": false,
      "description": "Measurement method",

      "parse": {
        "method": "regex",
        "pattern": "^([A-Za-z ]+)$",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": null,
        "padding": "none"
      }
    },

    {
      "name": "SampleType",
      "dataType": "string",
      "position": 6,
      "required": false,
      "description": "Sample type",

      "parse": {
        "method": "regex",
        "pattern": "^([A-Za-z]+)$",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": null,
        "padding": "none"
      }
    }
  ],

  "messages": [
    {
      "messageId": "Reading",
      "messageType": "response",
      "pattern": "pH.*ATC",
      "fieldNames": ["pH", "Temperature", "Mode"],
      "terminator": "0D 0A"
    },
    {
      "messageId": "FullReport",
      "messageType": "response",
      "pattern": "pH.*[\\r\\n]+.*[\\r\\n]+.*",
      "fieldNames": ["pH", "Temperature", "Mode", "Date", "Time", "Method", "SampleType"],
      "terminator": "0D 0A"
    }
  ]
}
```

---

### Example 5: JIK6CAB (Most Complex - State Machine Multi-Line)

**Protocol**: 14-line package with start/end markers and state machine parsing

```
^KJIK000\r\n
2023-11-07\r\n
17:19:38\r\n
  0.00 kg\r\n
  1.94 kg\r\n
0\r\n
0\r\n
  1.94 kg\r\n
  1.94 kg\r\n
    0 pcs\r\n
 \r\n
 \r\n
E\r\n
~P1\r\n
```

**Production Code Strategy**: State machine with package markers (^KJIK000 start, ~P1 end), line-by-line extraction with content detection (Contains("kg"), Contains("pcs"), Contains(":"), Contains("-"))

**Complexity Features**:
- Multi-line package (14 lines)
- Start marker detection
- End marker validation
- Combined DateTime (date + time)
- Multiple weight fields with units
- Reserved/skip lines
- State tracking (bCompleted flag)

```json
{
  "deviceName": "JIK6CAB",
  "version": "1.0",
  "generatedDate": "2025-10-19T12:00:00Z",
  "description": "JADEVER JIK-6CAB - Complex multi-line package protocol with state machine",
  "encoding": "ASCII",
  "messageTerminator": "0D 0A",
  "messageStructure": "multi-line-frame",
  "messageHeader": "^KJIK000",
  "messageFooter": "~P1",

  "fields": [
    {
      "name": "StartMarker",
      "dataType": "string",
      "position": 0,
      "required": true,
      "description": "Package start marker",

      "parse": {
        "method": "regex",
        "pattern": "^\\^KJIK000$",
        "group": 0,
        "trim": true
      },

      "serialize": {
        "format": null,
        "padding": "none"
      }
    },

    {
      "name": "Date",
      "dataType": "datetime",
      "position": 1,
      "required": true,
      "description": "Measurement date (combined with Time)",

      "parse": {
        "method": "regex",
        "pattern": "^(\\d{4}-\\d{2}-\\d{2})$",
        "group": 1,
        "trim": true,
        "format": "yyyy-MM-dd"
      },

      "serialize": {
        "format": "yyyy-MM-dd"
      }
    },

    {
      "name": "Time",
      "dataType": "timespan",
      "position": 2,
      "required": true,
      "description": "Measurement time (combined with Date)",

      "parse": {
        "method": "regex",
        "pattern": "^(\\d{2}:\\d{2}:\\d{2})$",
        "group": 1,
        "trim": true,
        "format": "HH:mm:ss"
      },

      "serialize": {
        "format": "HH:mm:ss"
      }
    },

    {
      "name": "TareWeight",
      "dataType": "decimal",
      "position": 3,
      "required": true,
      "description": "Tare weight (TW) in kg",

      "parse": {
        "method": "regex",
        "pattern": "^\\s*(\\d+\\.\\d+)\\s*kg",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": "F2",
        "padding": "left",
        "paddingChar": " ",
        "width": 6,
        "alignment": "right"
      }
    },

    {
      "name": "TareUnit",
      "dataType": "string",
      "position": 4,
      "required": true,
      "description": "Tare weight unit",

      "parse": {
        "method": "regex",
        "pattern": "(kg|g)$",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": null,
        "padding": "left",
        "paddingChar": " ",
        "width": 2,
        "alignment": "left"
      }
    },

    {
      "name": "GrossWeight",
      "dataType": "decimal",
      "position": 5,
      "required": true,
      "description": "Gross weight (GW) in kg",

      "parse": {
        "method": "regex",
        "pattern": "^\\s*(\\d+\\.\\d+)\\s*kg",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": "F2",
        "padding": "left",
        "paddingChar": " ",
        "width": 6,
        "alignment": "right"
      }
    },

    {
      "name": "GrossUnit",
      "dataType": "string",
      "position": 6,
      "required": true,
      "description": "Gross weight unit",

      "parse": {
        "method": "regex",
        "pattern": "(kg|g)$",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": null,
        "padding": "left",
        "paddingChar": " ",
        "width": 2,
        "alignment": "left"
      }
    },

    {
      "name": "Reserved1",
      "dataType": "int",
      "position": 7,
      "required": false,
      "description": "Reserved field 1 (typically 0)",

      "parse": {
        "method": "regex",
        "pattern": "^(\\d+)$",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": "D",
        "padding": "none"
      }
    },

    {
      "name": "Reserved2",
      "dataType": "int",
      "position": 8,
      "required": false,
      "description": "Reserved field 2 (typically 0)",

      "parse": {
        "method": "regex",
        "pattern": "^(\\d+)$",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": "D",
        "padding": "none"
      }
    },

    {
      "name": "NetWeight",
      "dataType": "decimal",
      "position": 9,
      "required": true,
      "description": "Net weight (NW) = GW - TW",

      "parse": {
        "method": "regex",
        "pattern": "^\\s*(\\d+\\.\\d+)\\s*kg",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": "F2",
        "padding": "left",
        "paddingChar": " ",
        "width": 6,
        "alignment": "right"
      }
    },

    {
      "name": "NetUnit",
      "dataType": "string",
      "position": 10,
      "required": true,
      "description": "Net weight unit",

      "parse": {
        "method": "regex",
        "pattern": "(kg|g)$",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": null,
        "padding": "left",
        "paddingChar": " ",
        "width": 2,
        "alignment": "left"
      }
    },

    {
      "name": "DisplayWeight",
      "dataType": "decimal",
      "position": 11,
      "required": false,
      "description": "Display weight (duplicate of NW)",

      "parse": {
        "method": "regex",
        "pattern": "^\\s*(\\d+\\.\\d+)\\s*kg",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": "F2",
        "padding": "left",
        "paddingChar": " ",
        "width": 6,
        "alignment": "right"
      }
    },

    {
      "name": "PieceCount",
      "dataType": "int",
      "position": 12,
      "required": false,
      "description": "Piece count",

      "parse": {
        "method": "regex",
        "pattern": "^\\s*(\\d+)\\s*pcs",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": "D",
        "padding": "left",
        "paddingChar": " ",
        "width": 5,
        "alignment": "right"
      }
    },

    {
      "name": "StatusIndicator",
      "dataType": "char",
      "position": 13,
      "required": false,
      "description": "Status indicator (E=Empty, S=Stable, N=Normal)",

      "parse": {
        "method": "regex",
        "pattern": "^([A-Z])$",
        "group": 1,
        "trim": true
      },

      "serialize": {
        "format": null,
        "padding": "none"
      }
    },

    {
      "name": "EndMarker",
      "dataType": "string",
      "position": 14,
      "required": true,
      "description": "Package end marker",

      "parse": {
        "method": "regex",
        "pattern": "^~P1$",
        "group": 0,
        "trim": true
      },

      "serialize": {
        "format": null,
        "padding": "none"
      }
    }
  ],

  "messages": [
    {
      "messageId": "WeightPackage",
      "messageType": "response",
      "pattern": "^\\^KJIK000",
      "fieldNames": [
        "StartMarker",
        "Date",
        "Time",
        "TareWeight",
        "TareUnit",
        "GrossWeight",
        "GrossUnit",
        "Reserved1",
        "Reserved2",
        "NetWeight",
        "NetUnit",
        "DisplayWeight",
        "PieceCount",
        "StatusIndicator",
        "EndMarker"
      ],
      "terminator": "0D 0A"
    }
  ],

  "validation": {
    "rules": [
      {
        "name": "DateTimeValid",
        "type": "datetime-range",
        "field": "Date",
        "minDate": "2020-01-01",
        "maxDate": "2099-12-31"
      },
      {
        "name": "WeightCalculation",
        "type": "formula",
        "formula": "GrossWeight - TareWeight = NetWeight",
        "tolerance": 0.01
      },
      {
        "name": "GrossVsTare",
        "type": "custom",
        "condition": "GrossWeight >= TareWeight"
      }
    ]
  }
}
```

**Key Implementation Notes**:

1. **State Machine Parsing**:
   - Start: Detect `^KJIK000` → Reset all variables
   - Process 14 lines sequentially
   - End: Validate `~P1` → Fire data received event

2. **DateTime Combination**:
   ```csharp
   // Parse separately
   DateTime date = ParseDate(line2);  // 2023-11-07
   TimeSpan time = ParseTime(line3);  // 17:19:38

   // Combine into single DateTime property
   data.Date = date.Date + time;  // 2023-11-07 17:19:38
   ```

3. **Unit Extraction**:
   - Each weight value has separate unit field
   - Pattern: `"  1.94 kg"` → Weight=1.94, Unit="kg"

4. **Skip Lines**:
   - Lines 11-12: Empty/whitespace (` \r\n`)
   - Can skip during parsing if not needed

5. **Production Code Mapping**:
   ```csharp
   // Existing C# class
   public class JIK6CABData
   {
       public DateTime Date { get; set; }  // Combined date+time
       public decimal TW { get; set; }     // TareWeight
       public decimal GW { get; set; }     // GrossWeight
       public decimal NW { get; set; }     // NetWeight
       public decimal PCS { get; set; }    // PieceCount
       public string TUnit { get; set; }   // TareUnit
       public string GUnit { get; set; }   // GrossUnit
       public string NUnit { get; set; }   // NetUnit
   }
   ```

---

## Validation Rules

### Definition File Validation

When loading a definition file, validate:

1. **Required Fields Present**:
   - deviceName
   - version
   - encoding
   - At least one field

2. **Field Names**:
   - All unique
   - All valid C# identifiers
   - Not C# keywords

3. **Field Positions**:
   - Sequential (0, 1, 2, ...)
   - No gaps

4. **Parse Configuration**:
   - If method=regex, pattern must be valid regex
   - If method=fixed-position, offset >= 0 and length > 0
   - If method=delimited, delimiter must be non-empty

5. **Serialize Configuration**:
   - Format strings valid for data type
   - Width > 0 if specified
   - PaddingChar is single character

6. **Data Types**:
   - Match supported types
   - Format strings appropriate for type

### Validation Example

```csharp
// From 04-Data-Models-Design.md: ProtocolDefinition.Validate()
List<string> errors = definition.Validate();

if (errors.Count > 0)
{
    foreach (var error in errors)
        Console.WriteLine($"Validation Error: {error}");
    return false;
}
return true;
```

---

## Usage Examples

### NTerminal<T> - Parse Incoming Data

```csharp
// Define your data class
public class WeightData
{
    public decimal Weight { get; set; }
    public string Unit { get; set; }
    public char Status { get; set; }
}

// Load definition and create terminal
var terminal = new NTerminal<WeightData>("CordDEFENDER3000-definition.json");
terminal.Connect("COM3", 9600);

// Receive data
terminal.OnDataReceived += (sender, data) =>
{
    Console.WriteLine($"Weight: {data.Weight} {data.Unit} [{data.Status}]");
};

// Protocol Analyzer generated the JSON, NTerminal uses it to parse bytes
```

### NDevice<T> - Serialize Outgoing Data

```csharp
// Same data class
public class WeightData
{
    public decimal Weight { get; set; } = 1.640m;
    public string Unit { get; set; } = "kg";
    public char Status { get; set; } = 'G';
}

// Load definition and create device emulator
var device = new NDevice<WeightData>("CordDEFENDER3000-definition.json");
device.Listen("COM4", 9600);

// Send data
var data = new WeightData();
device.SendData(data);

// Protocol Analyzer generated the JSON, NDevice uses it to serialize bytes
```

---

## Summary

This JSON schema enables:

✅ **Bidirectional Communication** - Parse AND Serialize
✅ **4 Parse Methods** - Regex, Fixed-Position, Delimited, Header-Byte
✅ **Flexible Formatting** - Padding, alignment, width control
✅ **Type-Safe** - Maps to C# types (int, decimal, string, datetime, etc.)
✅ **Self-Documenting** - Includes descriptions and metadata
✅ **Validation-Ready** - JSON Schema validation built-in
✅ **Production-Tested** - Based on real device protocols

### Complete Examples Provided:
1. ✅ CordDEFENDER3000 - Simple space-delimited
2. ✅ WeightQA - Nested delimiters with reconstruction
3. ✅ TFO1 - Fixed-position with binary bytes
4. ✅ PHMeter - Content-based multi-line
5. ✅ JIK6CAB - Most complex: State machine multi-line package with markers

All examples include both **parse** and **serialize** configurations for every field.

---

**Document Version**: 2.1
**Last Updated**: 2025-10-21
**Status**: Complete - Ready for Implementation
**Changes**:
- v2.0: Complete rewrite with full schema and 4 device examples
- v2.1: Added JIK6CAB example (most complex protocol)
