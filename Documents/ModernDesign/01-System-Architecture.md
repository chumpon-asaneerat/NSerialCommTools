# System Architecture Analysis

## NLib.Serial.Protocol.Analyzer - Modern Design

---

## Requirements Summary

**Primary Goal**: Create a protocol analyzer that can:
1. Load serial device log files in 3 formats (HEX/Text, HEX Only, Text Only)
2. Analyze the protocol structure automatically
3. Generate protocol definition files for working with serial devices

**Technical Constraints**:
- .NET Framework 4.7.2 only (no .NET Core)
- Working project: `@09.App\NLib.Serial.Protocol.Analyzer`
- Sample data: `@Documents\LuckyTex Devices\` folder

---

## System Architecture Overview

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
- **HEX/Text**: Lines contain hex bytes followed by ASCII representation
  - Example: `46 20 20 20 20 20 20 30 2E 30 0D           F      0.0.`
- **HEX Only**: Lines contain only hex bytes (with optional comments)
  - Example: `46 20 20 20 20 20 20 30 2E 30 0D  // F      0.0.`
- **Text Only**: Plain ASCII text output
  - Example: `SET P1(W)       0.0`

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
1. Split line by whitespace delimiter (varies by format)
2. Left side: Parse hex bytes (2 chars = 1 byte)
3. Right side: Extract ASCII text representation
4. Validate: Convert bytes to ASCII should match text

#### HEX Only Parser
```
Input:  "46 20 20 20 20 20 20 30 2E 30 0D  // F      0.0."
Output:
  - Byte array: [0x46, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x30, 0x2E, 0x30, 0x0D]
  - Comment: "F      0.0."
```

**Parsing Strategy**:
1. Find comment delimiter (`//` or similar)
2. Parse hex bytes before comment
3. Extract comment as reference text
4. Generate ASCII text from bytes

#### Text Only Parser
```
Input:  "SET P1(W)       0.0"
Output:
  - Text: "SET P1(W)       0.0"
  - Byte array: ASCII/UTF8 encoding of text
```

**Parsing Strategy**:
1. Read text line as-is
2. Convert to bytes using ASCII encoding (default)
3. Detect line terminator (CR/LF/CRLF)
4. Store original text + byte representation

---

### 3. Normalized Data Model

All parsers output to a common structure:

```
LogEntry
├── Bytes: byte[]           // Raw bytes from serial device
├── Text: string            // Human-readable representation
├── Timestamp: DateTime?    // If available in log
├── Direction: enum         // TX/RX (if indicated)
├── LineNumber: int         // Original line in file
└── Metadata
    ├── Format: LogFileFormat enum
    ├── Encoding: Encoding
    ├── HasTerminator: bool
    └── TerminatorBytes: byte[]
```

**Key Principle**: Once normalized, analyzer doesn't need to know original format.

---

### 4. Protocol Analyzer

The analyzer examines the normalized data to detect protocol characteristics:

#### a) Message Terminators

**Detection Strategy**:
- Analyze last bytes of each message
- Count frequency of terminator patterns
- Common patterns: `0x0D`, `0x0A`, `0x0D 0x0A`, custom sequences

**Example from TFO1**:
```
Pattern: 0x0D (CR)           - Frequency: 90%
Pattern: 0x83 0x0D           - Frequency: 10% (special marker)
```

#### b) Message Delimiters

**Detection Strategy**:
- Analyze byte patterns within messages
- Detect consistent field separators
- Types: Fixed-width, space-delimited, tab-delimited, custom

**Example from TFO1**:
```
Format: Fixed-width fields
Field separator: Multiple spaces
Example: "F      0.0" (identifier + padding + value)
```

#### c) Data Patterns

**Detection Strategy**:
- Statistical analysis of message structures
- Identify repeating patterns
- Classify fields (numeric, text, timestamp, identifier)

**Example from TFO3**:
```
Repeating structure (10 lines):
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
- Key-value pairs
- Keys: Fixed text labels
- Values: Variable (numeric or datetime)
- Delimiter: Multiple spaces
- Message: 10-line block

#### d) Message Structure

**Detection Strategy**:
- Identify headers (consistent starting bytes/patterns)
- Identify footers (consistent ending patterns)
- Detect checksums (if present)
- Determine message length (fixed vs variable)

**Example from TFO1**:
```
Header:  "V1\r\n"
Body:    Multiple data lines (F, H, Q, X, A, 0, 4, 1, 2)
Footer:  "B\x83\r"
         "C20. 02. 2023. MON 09:17AM\r"
```

---

### 5. Protocol Definition Generator

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
          "description": "Field identifier (F/H/Q/X/A/0/4/1/2)"
        },
        {
          "name": "value",
          "position": 2,
          "type": "decimal",
          "format": "fixed-width",
          "width": 10,
          "alignment": "right",
          "description": "Numeric value"
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
    Analyzer-->>UI: Return analysis results
    User->>UI: Review & confirm
    UI->>Generator: Generate definition
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
    Analyzed --> Generating: User confirms & generates
    Generating --> Complete: Definition saved
    Generating --> Error: Generation failed
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

1. **Accuracy**: 90%+ correct pattern detection for sample devices
2. **Usability**: User can load file and generate definition in < 5 clicks
3. **Performance**: Process 10,000 line log file in < 5 seconds
4. **Extensibility**: Easy to add new log formats
5. **Documentation**: Clear JSON schema for definition files

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
