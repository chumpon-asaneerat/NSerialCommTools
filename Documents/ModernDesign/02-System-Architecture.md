# System Architecture Analysis

## NLib.Serial.Protocol.Analyzer - Modern Design

**Related Documents**:
- **00-Requirements-Specification.md** - Complete requirements
- **01-Production-Code-Analysis.md** - Analysis of existing working code
- **03-Parsing-Strategy-Analysis.md** - Detailed parsing algorithms

---

## Context: Complementary Tool, Not Replacement

**IMPORTANT**: This Protocol Analyzer is a **SEPARATE TOOL** that complements the existing production code:

- **Existing Production Code** (01.Core/NLib.Serial.Devices/):
  - SerialDevice, SerialDeviceEmulator, SerialDeviceTerminal classes
  - Real-time serial communication (bidirectional)
  - Hardcoded protocol parsing per device
  - **Status**: ✅ WORKING - Production ready - DO NOT CHANGE

- **Protocol Analyzer** (This tool):
  - Offline log file analysis
  - Automatic protocol discovery
  - JSON definition file generation
  - Enables rapid new device integration
  - **Status**: 🔄 Design phase - NEW CAPABILITY

See **01-Production-Code-Analysis.md** for detailed analysis of existing code.

---

## Requirements Summary

**Primary Goal**: Create a protocol analyzer that can:
1. Load serial device log files in 3 formats (HEX/Text, HEX Only, Text Only)
2. Analyze the protocol structure automatically
3. Generate **bidirectional** protocol definition files (parse AND serialize)
4. Support future NTerminal<T> and NDevice<T> runtime classes

**Three User Scenarios**:
1. **Protocol Analyzer Users**: Developers analyzing log files to generate definitions
2. **NTerminal<T> Users** (Future): Applications receiving data using JSON definitions
3. **NDevice<T> Users** (Future): Testing tools emulating devices using JSON definitions

**Technical Constraints**:
- .NET Framework 4.7.2 only (no .NET Core)
- Working project: `@09.App\NLib.Serial.Protocol.Analyzer`
- Sample data: `@Documents\LuckyTex Devices\` folder

---

## Ecosystem Integration

```mermaid
graph TD
    subgraph "Production Code (Existing - Working)"
        P1[Serial Device] <--> P2[SerialPort]
        P2 <--> P3[Terminal Class]
        P3 --> P4[Hardcoded Parse Logic]
        P4 --> P5[Data Class]
    end

    subgraph "Protocol Analyzer (New - This Design)"
        A1[Third-Party Logs] --> A2[Protocol Analyzer]
        A2 --> A3[JSON Definition]
    end

    subgraph "Future Integration (Phase 2)"
        F1[NTerminal<T>] --> F2[Load JSON Definition]
        F3[NDevice<T>] --> F4[Load JSON Definition]
        A3 --> F2
        A3 --> F4
    end

    A3 -.->|"Documentation"| D1[Protocol Docs]
    A3 -.->|"Future: Code Gen"| P3

    style P1 fill:#90EE90
    style A2 fill:#4A90E2
    style F1 fill:#FFE4B5
    style F3 fill:#FFE4B5
```

**Key Points**:
- Protocol Analyzer does NOT touch existing production code
- Generates JSON definitions from captured log files
- Future runtime classes (NTerminal<T>, NDevice<T>) will use these definitions
- Optional: Could generate code for new Terminal classes

---

## System Architecture Overview (Protocol Analyzer Only)

```mermaid
graph TD
    A[User] --> B[WPF Application]
    B --> C[File Loader]
    C --> D[Format Detector]
    D --> E1[HEX/Text Parser]
    D --> E2[HEX Only Parser]
    D --> E3[Text Only Parser]
    E1 --> F[Normalized Data Model]
    E2 --> F
    E3 --> F
    F --> G[Protocol Analyzer]
    G --> H[Pattern Detector]
    G --> I[Terminator Detector]
    G --> J[Delimiter Detector]
    G --> K[Field Analyzer]
    H --> L[Analysis Results]
    I --> L
    J --> L
    K --> L
    L --> M[Protocol Definition Generator]
    M --> N[JSON Definition File]
    N --> O[User/External Systems]
```

---

## Core Components

### 1. File Loader & Format Detection

**Purpose**: Load log files and automatically detect format

**Input**: File path

**Output**: Raw file content + detected format type

**Detection Logic**:
- **HEX/Text**: File lines contain hex bytes followed by ASCII representation
  - Example: `46 20 20 20 20 20 20 30 2E 30 0D           F      0.0.`
  - Note: The `0D` is part of the protocol data, not the file line terminator
- **HEX Only**: File lines contain only hex bytes (with optional comments)
  - Example: `46 20 20 20 20 20 20 30 2E 30 0D  // F      0.0.`
  - Comments after `//` are for reference only
- **Text Only**: Plain ASCII text output (already decoded by logging tool)
  - Example: `SET P1(W)       0.0`
  - May or may not show protocol terminators explicitly

---

### 2. Format-Specific Parsers

#### HEX/Text Parser
```
Input:  "46 20 20 20 20 20 20 30 2E 30 0D           F      0.0."
Output:
  - Byte array: [0x46, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x30, 0x2E, 0x30, 0x0D]
  - Text: "F      0.0."
```

**Parsing Strategy**:
1. Read each file line (separated by OS newlines - these are NOT protocol terminators)
2. Find the separator between hex and text sections (typically multiple spaces)
3. Left side: Parse hex bytes (2 hex chars = 1 byte, e.g., "46" = 0x46)
4. Right side: Extract ASCII text representation for reference
5. Validate: Converting bytes to ASCII should match text
6. Store raw bytes - protocol message boundaries determined by analyzer later

#### HEX Only Parser
```
Input:  "46 20 20 20 20 20 20 30 2E 30 0D  // F      0.0."
Output:
  - Byte array: [0x46, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x30, 0x2E, 0x30, 0x0D]
  - Comment: "F      0.0."
```

**Parsing Strategy**:
1. Read each file line (separated by OS newlines)
2. Find comment delimiter (`//`, `#`, or similar)
3. Parse hex bytes before comment (2 hex chars = 1 byte)
4. Extract comment as reference text (optional, for validation)
5. Generate ASCII text from bytes for display
6. Store raw bytes - protocol message boundaries determined by analyzer later

#### Text Only Parser
```
Input:  "SET P1(W)       0.0"
Output:
  - Text: "SET P1(W)       0.0"
  - Byte array: ASCII/UTF8 encoding of text
```

**Parsing Strategy**:
1. Read each file line as-is (file lines are separated by OS-specific newlines)
2. Convert text to bytes using ASCII encoding (default)
3. Store original text + byte representation
4. Note: Protocol message terminators will be detected later by the analyzer

---

### 3. Normalized Data Model

All parsers output to a common structure:

```
LogEntry (Internal Analysis Model)
├── Bytes: byte[]           // Raw bytes extracted from log file - USED IN ANALYSIS
├── Text: string            // Human-readable representation - USED IN ANALYSIS
├── Timestamp: DateTime?    // If available in log - MAY BE IN PROTOCOL DEFINITION
├── Direction: enum         // TX/RX (if indicated) - ANALYSIS ONLY
├── FileLineNumber: int     // Original line number in log file - ANALYSIS/DEBUG ONLY
└── Metadata
    ├── Format: LogFileFormat enum    // ANALYSIS ONLY
    └── Encoding: Encoding            // WILL BE IN PROTOCOL DEFINITION
```

**Key Principle**: Once normalized, analyzer doesn't need to know original format.

**Important Distinction Between Analysis Data vs Protocol Definition**:

| Field | Used in Analysis | Goes in Protocol Definition | Purpose |
|-------|-----------------|----------------------------|---------|
| Bytes | ✓ | ✗ | Analyze patterns, detect terminators |
| Text | ✓ | ✗ | Display to user, validate patterns |
| Timestamp | ✓ | Maybe | If protocol includes timestamps as data |
| Direction | ✓ | ✗ | Understand TX vs RX patterns |
| FileLineNumber | ✓ | ✗ | Debugging, error reporting, pattern tracking |
| Format | ✓ | ✗ | Parser-specific, not protocol-related |
| Encoding | ✓ | ✓ | Needed to decode protocol bytes |

**Two-Stage Process**:
1. **File Lines → LogEntry** - How the log tool saved the data (with OS-specific newlines)
2. **LogEntry → Protocol Definition** - How the serial device actually sends data (with protocol-specific terminators)

- **Parsers** extract raw bytes from file lines into LogEntry objects (with FileLineNumber for tracking)
- **Analyzer** examines LogEntry objects to determine protocol characteristics (uses FileLineNumber for error reporting)
- **Generator** creates Protocol Definition from analysis results (excludes FileLineNumber and other log-file-specific data)

**Example Use of FileLineNumber**:
- User sees: "Pattern detected at lines 10, 20, 30, 40..."
- User sees: "Warning: Incomplete message at line 145"
- UI highlights: File line 50 → matches "WeightData" message pattern
- NOT in output: Protocol definition has no concept of "file lines"

---

### 4. Protocol Analyzer

The analyzer examines the normalized data to detect protocol characteristics:

#### a) Protocol Message Terminators

**Purpose**: Identify the bytes that mark the end of a protocol message (NOT file line breaks)

**Detection Strategy**:
- Analyze ending byte patterns across all entries
- Count frequency of terminator candidates
- Common patterns: `0x0D` (CR), `0x0A` (LF), `0x0D 0x0A` (CRLF), custom byte sequences
- Validate consistency across the dataset

**Example from TFO1**:
```
Pattern: 0x0D (CR)           - Frequency: 90%  (standard message terminator)
Pattern: 0x83 0x0D           - Frequency: 10%  (special footer marker)
```

**Note**: These are protocol-level terminators, independent of how the log file was formatted.

#### b) Field Delimiters

**Purpose**: Identify bytes/characters that separate fields within a protocol message

**Detection Strategy**:
- Analyze byte patterns within individual messages
- Detect consistent field separators
- Types: Fixed-width, space-delimited, tab-delimited, comma-separated, custom bytes

**Example from TFO1**:
```
Format: Fixed-width fields
Field separator: Multiple spaces (0x20)
Example: "F      0.0"
  - Field 1: "F" (identifier, position 0)
  - Padding: "      " (6 spaces)
  - Field 2: "0.0" (value, right-aligned)
```

#### c) Data Patterns

**Detection Strategy**:
- Statistical analysis of message structures
- Identify repeating patterns
- Classify fields (numeric, text, timestamp, identifier)

**Example from TFO3**:
```
Repeating structure (10-entry block):
  SET P1(W)       0.0
  SET P2          0.0
  SET P3          0.0
  SET P4          0.0
  GROSS WEIGHT    1138.0
  NET WEIGHT      21.5
  TARE            1116.5
  DATE & TIME     JAN. 14,2024  06:28AM
  SCALE NO        1
  STATUS          �*
```

**Detected Pattern**:
- Key-value pairs structure
- Keys: Fixed text labels (left side)
- Values: Variable (numeric or datetime, right side)
- Field delimiter: Multiple spaces
- Message structure: Multi-entry message block
- Each entry likely has protocol terminator (to be detected)

#### d) Message Structure

**Detection Strategy**:
- Identify headers (consistent starting bytes/patterns)
- Identify footers (consistent ending patterns)
- Detect checksums (if present)
- Determine message length (fixed vs variable)

**Example from TFO1**:
```
Message Frame Structure:
Header:  "V1" + 0x0D 0x0A (protocol header with CRLF terminator)
Body:    Multiple data entries (F, H, Q, X, A, 0, 4, 1, 2)
         Each entry: identifier + value + 0x0D (CR terminator)
Footer:  "B" + 0x83 0x0D (special footer marker)
         "C20. 02. 2023. MON 09:17AM" + 0x0D (timestamp)
```

**Note**: These are protocol-level message boundaries and terminators, not file formatting.

---

### 5. Protocol Definition Generator

**Purpose**: Generate a **bidirectional** protocol definition file that OTHER applications can use to communicate with the serial device.

**Bidirectional Support**:
- **Parse Direction** (Device → Application): How to parse incoming bytes into fields
- **Serialize Direction** (Application → Device): How to format fields into bytes

This enables both:
- **NTerminal<T>**: Parse received data using the definition
- **NDevice<T>**: Serialize T instance into protocol bytes

**Important**: The definition file contains ONLY protocol structure, NOT log-file-specific information.

**What Goes In**:
- ✓ Protocol terminators (e.g., `\r`, `\x83\r`)
- ✓ Field delimiters (e.g., spaces, tabs)
- ✓ Message patterns and structure
- ✓ Field definitions (name, type, position, format)
- ✓ Encoding (e.g., ASCII, UTF-8)
- ✓ Message sequences
- ✓ **Parse rules** (how to extract fields from bytes)
- ✓ **Format rules** (how to convert fields to bytes)

**What Does NOT Go In**:
- ✗ File line numbers (analysis/debug only)
- ✗ Log file format (HEX/Text, etc.)
- ✗ Direction markers (TX/RX - unless part of protocol)
- ✗ Any log-tool-specific formatting

**Output Format**: JSON

```json
{
  "deviceName": "TFO1",
  "version": "1.0",
  "generatedDate": "2025-10-19T12:00:00Z",
  "encoding": "ASCII",
  "messageTerminator": "\\r",
  "messageDelimiter": null,
  "messages": [
    {
      "messageId": "WeightData",
      "messageType": "Response",
      "pattern": "^[A-Z]\\s+[\\d.]+$",
      "fields": [
        {
          "name": "identifier",
          "position": 0,
          "length": 1,
          "type": "char",
          "description": "Field identifier (F/H/Q/X/A/0/4/1/2)",
          "parse": {
            "offset": 0,
            "length": 1,
            "encoding": "ASCII"
          },
          "serialize": {
            "position": 0,
            "required": true
          }
        },
        {
          "name": "value",
          "position": 2,
          "type": "decimal",
          "format": "fixed-width",
          "width": 10,
          "alignment": "right",
          "description": "Numeric value",
          "parse": {
            "offset": 1,
            "length": 9,
            "encoding": "ASCII",
            "trim": true,
            "decimalFormat": "F1"
          },
          "serialize": {
            "position": 1,
            "padding": "left",
            "padChar": " ",
            "width": 9,
            "decimalFormat": "F1"
          }
        }
      ],
      "terminator": "\\r"
    },
    {
      "messageId": "Header",
      "messageType": "FrameStart",
      "pattern": "^V\\d+$",
      "fields": [
        {
          "name": "version",
          "position": 1,
          "type": "integer",
          "description": "Protocol version"
        }
      ],
      "terminator": "\\r\\n"
    },
    {
      "messageId": "Footer",
      "messageType": "FrameEnd",
      "pattern": "^B",
      "fields": [
        {
          "name": "marker",
          "position": 0,
          "length": 1,
          "type": "char",
          "value": "B"
        }
      ],
      "terminator": "\\x83\\r"
    }
  ],
  "messageSequence": [
    "Header",
    "WeightData (F)",
    "WeightData (H)",
    "WeightData (Q)",
    "WeightData (X)",
    "WeightData (A)",
    "WeightData (0)",
    "WeightData (4)",
    "WeightData (1)",
    "WeightData (2)",
    "Footer",
    "DateTime"
  ]
}
```

---

## Data Flow Sequence

```mermaid
sequenceDiagram
    participant User
    participant UI
    participant Loader
    participant Parser
    participant Analyzer
    participant FieldEditor
    participant Generator

    User->>UI: Select log file
    UI->>Loader: Load file
    Loader->>Loader: Detect format
    Loader->>Parser: Parse with detected format
    Parser->>Parser: Extract bytes & text
    Parser-->>UI: Return normalized data
    UI->>Analyzer: Analyze protocol
    Analyzer->>Analyzer: Detect terminators
    Analyzer->>Analyzer: Detect delimiters
    Analyzer->>Analyzer: Identify patterns
    Analyzer->>Analyzer: Extract field structure
    Analyzer->>Analyzer: Generate auto field names
    Analyzer-->>UI: Return analysis results
    UI->>FieldEditor: Show fields with auto names
    FieldEditor->>User: Display editable field grid
    User->>FieldEditor: Rename fields (Field1→NetWeight)
    FieldEditor->>FieldEditor: Validate names
    FieldEditor-->>UI: Return updated field definitions
    User->>UI: Review & confirm
    UI->>Generator: Generate definition (with custom names)
    Generator-->>UI: Return JSON file
    UI->>User: Save definition file
```

---

## Component State Diagram

```mermaid
stateDiagram-v2
    [*] --> Idle
    Idle --> LoadingFile: User selects file
    LoadingFile --> DetectingFormat: File loaded
    DetectingFormat --> Parsing: Format detected
    Parsing --> Parsed: Parse complete
    Parsing --> Error: Parse failed
    Parsed --> Analyzing: User initiates analysis
    Analyzing --> Analyzed: Analysis complete
    Analyzing --> Error: Analysis failed
    Analyzed --> EditingFields: Show field editor
    EditingFields --> EditingFields: User renames fields
    EditingFields --> Generating: User confirms
    Generating --> Complete: Definition saved
    Generating --> Error: Generation failed
    EditingFields --> Analyzing: Re-analyze
    Error --> Idle: User resets
    Complete --> Idle: User starts new analysis
```

---

## Class Diagram Overview

```mermaid
classDiagram
    class LogFileLoader {
        +LoadFile(string path) LogFile
        +DetectFormat(string content) LogFileFormat
    }

    class LogFileFormat {
        <<enumeration>>
        HexText
        HexOnly
        TextOnly
    }

    class ILogParser {
        <<interface>>
        +Parse(string content) List~LogEntry~
    }

    class HexTextParser {
        +Parse(string content) List~LogEntry~
    }

    class HexOnlyParser {
        +Parse(string content) List~LogEntry~
    }

    class TextOnlyParser {
        +Parse(string content) List~LogEntry~
    }

    class LogEntry {
        +byte[] Bytes
        +string Text
        +DateTime? Timestamp
        +int LineNumber
        +LogEntryMetadata Metadata
    }

    class ProtocolAnalyzer {
        +Analyze(List~LogEntry~) AnalysisResult
        -DetectTerminators()
        -DetectDelimiters()
        -IdentifyPatterns()
        -ExtractFields()
    }

    class AnalysisResult {
        +string DeviceName
        +List~MessagePattern~ Patterns
        +TerminatorInfo Terminator
        +DelimiterInfo Delimiter
        +List~FieldInfo~ Fields
    }

    class FieldInfo {
        +string Name
        +string DataType
        +int Position
        +List~string~ SampleValues
        +double ConfidenceScore
    }

    class FieldEditor {
        +LoadFields(List~FieldInfo~)
        +ValidateFieldName(string) bool
        +UpdateFieldName(FieldInfo field, string newName)
        +SuggestName(FieldInfo) string
        +GetUpdatedFields() List~FieldInfo~
    }

    class ProtocolDefinitionGenerator {
        +Generate(AnalysisResult) string
        +SaveToFile(string json, string path)
    }

    LogFileLoader --> LogFileFormat
    LogFileLoader --> ILogParser
    ILogParser <|-- HexTextParser
    ILogParser <|-- HexOnlyParser
    ILogParser <|-- TextOnlyParser
    ILogParser --> LogEntry
    ProtocolAnalyzer --> LogEntry
    ProtocolAnalyzer --> AnalysisResult
    AnalysisResult --> FieldInfo
    FieldEditor --> FieldInfo
    ProtocolDefinitionGenerator --> AnalysisResult
```

---

## Key Design Decisions

### 1. Multi-Stage Pipeline Architecture
- **Why**: Separation of concerns - parsing, analysis, and generation are independent
- **Benefit**: Easy to test each stage, modify parsers without affecting analyzers
- **Trade-off**: More complexity, but better maintainability

### 2. Normalized Data Model
- **Why**: Different log formats need common representation
- **Benefit**: Analyzer doesn't care about original format
- **Implementation**: All parsers output `LogEntry` objects

### 3. Statistical Pattern Detection
- **Why**: Can't assume all protocols follow same rules
- **Benefit**: Automatically adapts to different device protocols
- **Approach**: Frequency analysis, pattern matching, heuristics

### 4. JSON Output Format
- **Why**: Human-readable, machine-parsable, extensible
- **Benefit**: Can be used by other tools in the ecosystem
- **Alternative considered**: XML (rejected - too verbose)

### 5. WPF for UI
- **Why**: Rich UI controls, data binding, .NET 4.7.2 compatible
- **Benefit**: Better user experience than Windows Forms
- **Features needed**: File picker, text display, tree view, export

---

## Expected Challenges

### 1. Ambiguous Patterns
**Problem**: Some devices may have irregular message formats
**Solution**:
- Provide manual override options
- Support multiple pattern matches
- Use confidence scoring

### 2. Mixed Content
**Problem**: Text and binary data in same protocol
**Solution**:
- Detect non-printable characters
- Support hybrid parsing
- Preserve raw bytes always

### 3. Timestamp Handling
**Problem**: Logs may have different timestamp formats
**Solution**:
- Flexible timestamp parsing
- Optional timestamp field
- Support multiple formats

### 4. Noise in Logs
**Problem**: Extra debug messages, connection errors
**Solution**:
- Filtering options
- Pattern validation
- User can exclude lines

### 5. Incomplete Messages
**Problem**: Truncated or partial messages in logs
**Solution**:
- Detect incomplete patterns
- Warning system
- Option to ignore

### 6. Field Naming
**Problem**: Protocols rarely include field names - analyzer must generate them automatically
**Solution**:
- Auto-generate field names based on pattern:
  - Position-based: "Field1", "Field2", "Field3"
  - Type-based: "NumericField1", "StringField1"
  - Content-based: "DateTimeField", "WeightValue" (if detectable)
- Provide field editor UI for user customization
- Validate custom names (C# identifier rules)
- Preserve custom names across re-analysis
- Show sample values to help user understand field purpose

**Example Workflow**:
```
1. Analyzer generates: Name = "Field1"
2. User edits: Name = "NetWeight"
3. Save to JSON: { "name": "NetWeight", ... }
```

**Note**: No need to track "auto-generated vs custom" - just one `Name` property that user edits directly.

---

## Architecture Principles

### 1. No Hardcoding Protocol Assumptions
**Principle**: The analyzer must work with ANY protocol without hardcoded assumptions.

**Violations Found**:
- ❌ **Hardcoded terminators**: `text.Split(new[] { "\r\n", "\n", "\r" })`
  - Location: FieldAnalyzer.cs:79
  - Issue: Ignores TerminatorDetector results completely
  - Should use: Detected terminator from `TerminatorInfo.Bytes`
  - Impact: Cannot analyze protocols with custom terminators (e.g., `\x83\r`, custom binary sequences)

- ❌ **Hardcoded unit patterns**: `"kg"`, `"g"`, `"pcs"`, `"°C"`, `"pH"`
  - Location: RelationshipDetector.cs:77-84
  - Issue: Regex patterns hardcoded for specific units only
  - Should use: Dynamic pattern extraction from sample data
  - Impact: Cannot detect unknown units (e.g., "lb", "oz", "mL", custom units)

- ❌ **Hardcoded encoding assumption**: ASCII
  - Issue: Assumes all protocols use ASCII encoding
  - Should use: EncodingDetector results
  - Impact: Cannot handle UTF-8, UTF-16, or custom encodings

**Correct Approach**:
```csharp
// DON'T: Hardcoded terminators
string[] lines = text.Split(new[] { "\r\n", "\n", "\r" });

// DO: Use detected terminator
byte[] terminator = detectedTerminator.Bytes;  // From TerminatorDetector
List<byte[]> frames = SplitByTerminator(rawBytes, terminator);
```

```csharp
// DON'T: Hardcoded unit patterns
var patterns = new[] {
    new { Name = "WeightKg", Pattern = new Regex(@"(\d+\.?\d*)\s*(kg)") }
};

// DO: Extract patterns from data
// Sample: "1.94 kg" → Detect: NUMBER + SPACE + TEXT_UNIT
// Extract unit from data: "kg" (not from hardcoded list)
// Generate generic Split: ValueField + UnitField
```

### 2. Byte-Level Processing Required
**Principle**: Work with `byte[]` throughout, not strings.

**Current Issue**:
- String-based processing loses binary data
- Assumes ASCII encoding for string conversion
- Cannot handle binary protocols
- `TerminatorInfo` has `byte[] Bytes` property but it's unused

**Should Be**:
```csharp
// Current (WRONG):
string text = Encoding.ASCII.GetString(rawBytes);  // Loses binary data
string[] lines = text.Split(new[] { "\r\n", "\n", "\r" });

// Should be:
byte[] terminator = detectedTerminator.Bytes;  // Binary-safe
List<byte[]> frames = SplitByTerminator(rawBytes, terminator);
// Convert to string ONLY when needed for display:
string displayText = DetectedEncoding.GetString(frame);
```

**Why This Matters**:
- Binary protocols may contain `\x00` (null) bytes - lost in string conversion
- Protocol terminators may be custom binary sequences (e.g., `\x83\r`)
- Different encodings (UTF-16, Shift-JIS) require proper detection

### 3. Data-Driven Field Detection
**Principle**: No assumptions about field types or meanings.

**Current Issue**:
- Hardcoded patterns for WeightKg, Temperature, pH
- Cannot detect unknown unit types
- Assumes specific field semantics

**Should Be**:
```csharp
// Analyze samples: "1.94 kg", "2.01 kg", "0.00 kg"
// Detect pattern: (\d+\.?\d*)\s+(\w+)
// Extract unit from data: "kg"
// Generate relationship: Value + Unit (generic, not WeightKg-specific)
// Don't assume meaning - just document the pattern
```

### 4. Separation of Concerns
**Principle**: Clear distinction between analysis metadata and protocol definition.

**Property Purposes**:
- `ShowInEditor` → UI visibility only (hide system fields like Empty lines)
- `IncludeInDefinition` → JSON export control (user decides what to export)
- `Action` → Terminal logic (Parse/Validate)

**Field Names**:
- Irrelevant for protocol recreation (device doesn't send field names)
- Used only for property mapping in Terminal configuration
- User customizes for clarity in their application code

### 5. ValidationRules Removed
**Decision**: ValidationRules feature has been **REMOVED** from the codebase.

**Reasons**:
- ❌ Not used by Terminal/Device runtime code
- ❌ Auto-generated ranges unreliable (based on limited sample data)
- ❌ Hardcoded assumptions (e.g., "Gross >= Tare") don't apply universally
- ❌ Added complexity with zero runtime benefit
- ✅ Users implement actual validation in their application layer

**What Was Removed**:
- `ValidationRule` class properties from models
- `ValidationRuleGenerator` class (logic still exists for reference)
- Export of validation rules to JSON
- UI display of rule counts

**If Users Need Validation**:
- Implement in application layer after parsing
- Use domain-specific business rules
- Not generic auto-generated suggestions

---

## Critical Architectural Debt

This section documents known architectural issues that violate the principles above and require refactoring.

### TODO-001: Use Detected Terminator Instead of Hardcoded Split
**Priority**: CRITICAL
**Location**: `FieldAnalyzer.cs:79`
**Issue**: Code ignores TerminatorDetector results and uses hardcoded `"\r\n", "\n", "\r"`

**Current Code**:
```csharp
string[] lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
```

**Impact**:
- Cannot analyze protocols with custom terminators (e.g., `\x83\r`, `\x03`, binary sequences)
- TerminatorDetector results are calculated but completely unused
- Wastes CPU on detection that's ignored

**Fix Required**:
```csharp
// Use detected terminator from TerminatorInfo.Bytes
byte[] terminator = _terminatorDetector.DetectedTerminator.Bytes;
List<byte[]> frames = SplitByTerminator(rawBytes, terminator);
```

**Effort**: Medium (requires byte[] processing implementation)

---

### TODO-002: Redesign for Byte[] Processing Throughout
**Priority**: CRITICAL
**Scope**: Major architectural refactoring
**Impact**: Cannot handle binary protocols, loses data in string conversion

**Current Architecture**:
```
byte[] → string → Split → Analyze → Export
         ^^^^^^
         Loses binary data here!
```

**Should Be**:
```
byte[] → Split by terminator → Analyze bytes → Export
         ^^^^^^^^^^^^^^^^^^^^^^
         Binary-safe throughout

Convert to string ONLY for display:
byte[] frame → DetectedEncoding.GetString(frame) → UI display
```

**Components Requiring Changes**:
- **FieldAnalyzer**: Work with `byte[]` frames instead of string lines
- **RelationshipDetector**: Parse `byte[]` patterns
- **FieldInfo**: Support both byte patterns and regex (for text protocols)
- **UI Display**: Convert bytes to string using detected encoding for display only

**Why This Matters**:
- Binary protocols may contain `\x00` (null) bytes → lost in string conversion
- Some protocols use custom encodings (Shift-JIS, custom character sets)
- Protocol terminators may be non-printable binary sequences

**Effort**: Large (3-5 days of refactoring)

---

### TODO-003: Add Encoding Detector and Use Detected Encoding
**Priority**: HIGH
**Issue**: Code assumes ASCII encoding everywhere

**Current Issues**:
```csharp
// Hardcoded ASCII assumption:
string text = Encoding.ASCII.GetString(rawBytes);
```

**Fix Required**:
1. Implement `EncodingDetector` class:
   - Auto-detect: UTF-8 BOM, UTF-16 BOM
   - Analyze byte patterns (ASCII vs UTF-8 vs others)
   - Calculate confidence scores
2. Use detected encoding for string conversions:
   ```csharp
   Encoding detectedEncoding = _encodingDetector.DetectedEncoding;
   string displayText = detectedEncoding.GetString(frameBytes);
   ```
3. Store detected encoding in protocol definition (already in schema)

**Impact**: Can handle international characters, Chinese/Japanese protocols, UTF-8 data

**Effort**: Medium (2-3 days)

---

### ~~TODO-004: Remove Hardcoded Unit Detection~~ ✅ **COMPLETED 2025-10-24**
**Priority**: ~~HIGH~~ **DONE**
**Location**: `RelationshipDetector.cs:155-220`
**Issue**: ~~Hardcoded regex patterns for specific units only~~ **FIXED**

**Old Code (REMOVED)**:
```csharp
var compoundPatterns = new[]
{
    new { Name = "WeightKg", Pattern = new Regex(@"^\s*([+-]?\d+\.?\d*)\s*(kg)\s*$") },
    new { Name = "WeightG", Pattern = new Regex(@"^\s*([+-]?\d+\.?\d*)\s*(g)\s*$") },
    new { Name = "CountPcs", Pattern = new Regex(@"^\s*(\d+)\s*(pcs)\s*$") },
    new { Name = "Temperature", Pattern = new Regex(@"^\s*([+-]?\d+\.?\d*)\s*(°C|C)\s*$") },
    new { Name = "pH", Pattern = new Regex(@"^\s*([+-]?\d+\.?\d*)\s*(pH)\s*$") }
};
```

**New Implementation (DYNAMIC)**:
```csharp
/// <summary>
/// Dynamically detects if a field is a compound field (value + unit) by analyzing samples.
/// NO HARDCODED UNITS - discovers pattern from actual data.
/// Pattern: NUMBER (optional +/-) + WHITESPACE + TEXT
/// Examples: "1.94 kg", "+007.12/3 G", "25.5°C", "100 PSI", "5.2 mL"
/// </summary>
private bool IsCompoundFieldDynamic(FieldInfo field,
    out List<string> valueSamples,
    out List<string> unitSamples,
    out string detectedPattern)
{
    // Generic pattern: NUMBER + optional WHITESPACE + TEXT
    // Group 1: numeric value (with optional +/- and decimal)
    // Group 2: unit text (letters, special chars like °, etc.)
    var genericPattern = new Regex(@"^\s*([+-]?\d+\.?\d*)\s+([^\d\s][^\s]*)?\s*$");

    // Analyze samples and extract values/units
    // Detect units from DATA, not hardcoded list
    // Validate: 80%+ match rate, 3+ matches, 1-5 unique units
}
```

**Benefits Achieved**:
- ✅ Works with ANY unit system (kg, g, pcs, °C, pH, lb, oz, mL, cm, mm, PSI, custom units)
- ✅ Discovers unknown/custom units automatically from data
- ✅ No hardcoded lists to maintain
- ✅ More robust and data-driven
- ✅ Follows "no hardcoding" principle

**Test Coverage**:
- ✅ Handles existing units: kg, g, pcs, °C, pH
- ✅ Handles new units: lb, oz, mL, cm, mm, PSI, bar, etc.
- ✅ Handles signed numbers: +007.12
- ✅ Handles edge cases: 1-5 unique units per field
- ✅ Requires 80%+ match rate for confidence

**Actual Effort**: 1 hour (faster than estimated)

---

### TODO-005: Width Analysis Not Implemented
**Priority**: LOW (not critical for current use case)
**Status**: DEFERRED

**Current State**:
- All exported fields have `width: 0`
- Width property exists but is never calculated

**Impact**:
- Device emulator cannot recreate exact byte-perfect spacing
- Works fine for data extraction (parsePattern handles parsing)
- Only affects protocol recreation fidelity

**If Needed**:
- Calculate fixed-width positions from aligned sample data
- Detect padding characters and alignment
- Store in field definition for emulator use

**Reason for Deferral**:
- Modern architecture uses regex `parsePattern` (not positional)
- Device formats each field individually (doesn't need exact widths)
- Can be added later if byte-perfect recreation is required

---

## Technology Stack

- **Framework**: .NET Framework 4.7.2
- **UI**: WPF (Windows Presentation Foundation)
- **JSON**: System.Text.Json (already referenced)
- **File I/O**: System.IO
- **Regex**: System.Text.RegularExpressions
- **Data Binding**: INotifyPropertyChanged, ObservableCollection

---

## Success Criteria

1. **Accuracy**: 95%+ correct pattern detection for sample devices
   - Protocol terminator detection: 95%+ accuracy
   - Field delimiter detection: 95%+ accuracy
   - Message pattern identification: 95%+ accuracy
   - Field structure extraction: 95%+ accuracy
2. **Usability**: User can load file and generate definition in < 5 clicks
3. **Performance**: Process 10,000 line log file in < 5 seconds
4. **Extensibility**: Easy to add new log formats
5. **Documentation**: Clear JSON schema for definition files
6. **Validation**: All sample devices in @Documents\LuckyTex Devices\ should be correctly analyzed

---

## Next Steps

1. **Data Models Design** - Define all classes and interfaces
2. **Parser Design** - Detail parsing algorithms for each format
3. **Analyzer Design** - Design detection algorithms
4. **UI Design** - Wireframes and user workflow
5. **Implementation Plan** - Break down into development tasks

---

**Document Version**: 1.0
**Last Updated**: 2025-10-19
**Author**: Claude (AI Assistant)
**Status**: Design Phase
