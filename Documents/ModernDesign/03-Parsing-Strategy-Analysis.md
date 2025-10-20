# Parsing Strategy Analysis

**Related Documents**:
- **00-Requirements-Specification.md** - Complete requirements
- **01-Production-Code-Analysis.md** - How production code handles these protocols
- **02-System-Architecture.md** - Overall system design
- **04-Data-Models-Design.md** - Data models supporting these strategies
- **05-JSON-Schema-Design.md** - JSON examples for each strategy

---

## Overview: Complete Parsing Strategy Diagram

```mermaid
flowchart TD
    A[Log File] --> B[Format Detection]

    B --> C1[HEX/Text]
    B --> C2[HEX Only]
    B --> C3[Text Only]

    C1 --> D[Extract Bytes & Text]
    C2 --> D
    C3 --> D

    D --> E[Detect Message Structure]

    E --> F1[Single Line<br/>CordDEFENDER3000]
    E --> F2[Multi-Line Frame<br/>TFO1]
    E --> F3[Sequential Lines<br/>JIK6CAB ⭐]
    E --> F4[Content-Based<br/>PHMeter]

    F1 --> G1[String Split Strategy<br/>Delimiters: space, slash]
    F2 --> G2[Header Byte Switch<br/>Fixed Position Extract]
    F3 --> G3[State Machine Parser ⭐<br/>Line-by-Line Sequential]
    F4 --> G4[Content Detection<br/>Pattern Matching]

    G1 --> H[Parse Fields]
    G2 --> H
    G3 --> H
    G4 --> H

    H --> I[Apply Field Relationships ⭐<br/>Combine, Split, Calculate]

    I --> J[Apply Validation Rules ⭐<br/>Formula, Range, DateTime]

    J --> K{Validation Pass?}

    K -->|Yes| L[Accept Data<br/>Fire Event]
    K -->|No - Error| M[Reject Data<br/>Log Error]
    K -->|No - Warning| N[Accept with Warning<br/>Log Warning]

    N --> L

    style F3 fill:#FFE4B5
    style G3 fill:#FFE4B5
    style I fill:#90EE90
    style J fill:#87CEEB

    note1[⭐ NEW in v2.0]
```

---

## Real-World Log Complexity Analysis

Based on examination of actual log files in `@Documents\LuckyTex Devices\` AND analysis of production parsing code in `01.Core\NLib.Serial.Devices\`, these are **NOT simple CSV formats**. They present significant parsing challenges.

**Key Insight from Production Code**: The existing Terminal classes (WeightQATerminal, CordDEFENDER3000Terminal, PHMeterTerminal, TFO1Terminal) each use DIFFERENT parsing strategies:
- String splitting (simple protocols)
- Fixed-position extraction (fixed-width fields)
- Switch/case on first byte (header-based)
- Conditional parsing (content-dependent)

The Protocol Analyzer must automatically detect which strategy applies to each device.

---

## Challenge Categories

### 1. **Simple Single-Line Repeating Messages**

**Example: DEFENDER3000 / CordDEFENDER3000**
```
-  1.640 kg    N
-  1.640 kg    N
-  1.640 kg    N
```

**Structure**:
- Fixed-width format
- Pattern: `sign` + `spaces` + `value` + `unit` + `spaces` + `status`
- Repeating same message

**Production Code Parsing Strategy** (CordDEFENDER3000.cs:300-334):
```csharp
string line = Encoding.ASCII.GetString(content);
string[] elems = line.Split(" ", RemoveEmptyEntries);
Value.W = decimal.Parse(elems[0]);    // "1.640"
Value.Unit = elems[1];                // "kg"
Value.O = elems[2];                   // "N"
```

**Challenges**:
- Leading spaces (variable or fixed?)
- Multiple spaces as delimiter
- Status code at end (`N`, `G`, etc.)

**Protocol Analyzer Must Detect**:
- Delimiter: Multiple spaces (0x20)
- Parsing strategy: String.Split with RemoveEmptyEntries
- Field count: 3 fields
- Data types: decimal, string, char

---

### 2. **Multi-Line Message Blocks**

**Example: JIK6CAB**
```
^KJIK000
2023-11-07
17:19:38
  0.00 kg
  1.94 kg
0
0
  1.94 kg
  1.94 kg
    0 pcs


E
~P1
```

**Structure**:
- Header marker: `^KJIK000`
- Date: `YYYY-MM-DD`
- Time: `HH:MM:SS`
- Multiple data fields (different formats)
- Footer markers: `E`, `~P1`

**Challenges**:
- **Multi-line message block** (not one line = one message!)
- Mixed data types (date, time, weight, count)
- Empty lines within message
- Start/end markers
- Variable number of fields

---

### 3. **Compact Binary-Style Format**

**Example: WEIGHT QA**
```
+007.12/3 G S
+008.12/2 G S
+009.36/0 G S
```

**Structure**:
- Sign: `+` or `-`
- Value: `007.12`
- Separator: `/`
- Counter/Status: `3`
- Unit: `G`
- Status: `S`
- Terminator: `0D 0A` (CRLF)

**Production Code Parsing Strategy** (WeightQA.cs:299-335):
```csharp
string line = Encoding.ASCII.GetString(content);
// Split by "/" to separate weight from metadata
string[] elems = line.Split("/");
string sUM = elems[1].Trim();  // "3 G S"
// Split by spaces to get individual fields
string[] elems2 = sUM.Split(" ", RemoveEmptyEntries);

string w = elems[0].Trim() + elems2[0].Trim(); // "+007.12" + "3"
Value.W = decimal.Parse(w);    // 007.123
Value.Unit = elems2[1];        // "G"
Value.Mode = elems2[2];        // "S"
```

**Challenges**:
- Compact format (no extra spaces)
- Multiple delimiters (`/`, spaces)
- Mixed numeric fields (value, counter)
- Status codes

**Protocol Analyzer Must Detect**:
- Primary delimiter: "/" (0x2F)
- Secondary delimiter: Space (0x20)
- Parsing strategy: Nested splitting
- Weight encoding: Integer part + "/" + fraction part
- Field reconstruction logic

---

### 4. **Mixed HEX and Text with Multi-Field Structure**

**Example: PH Meter**
```
Text view:
3.01pH 25.5°C ATC
20-Feb-2023
11:11

3.01pH
25.5°C ATC
Auto EP Standard
Blank
```

**HEX view shows:**
```
33 2E 30 31 70 48 20 32 35 2E 35 F8 43 20 41 54    3.01pH 25.5.C AT
43 0D 0A 33 2E 30 31 70 48 20 32 35 2E 35 F8 43    C..3.01pH 25.5.C
20 41 54 43 0D 0A 32 30 2D 46 65 62 2D 32 30 32     ATC..20-Feb-202
33 0D 0A 31 31 3A 31 31 0D 0A 20 0D 0A           3..11:11.. ..
```

**Structure**:
- Compound value: `3.01pH` (no space!)
- Temperature: `25.5°C` (special character `0xF8` for degree symbol)
- Mode: `ATC`
- Timestamp (multi-line)
- Mode description
- Sample type

**Production Code Parsing Strategy** (PHMeter.cs:358-462):
```csharp
string line = Encoding.ASCII.GetString(content).Trim();

if (line.Contains("ATC") && line.Contains("pH")) {
    // Parse both pH and temperature
    int iPh = line.IndexOf("pH");
    string sPh = line.Substring(0, iPh);
    Value.pH = decimal.Parse(sPh);

    int iTmp = lNext.IndexOf("C ATC");
    string sTmp = lNext.Substring(0, iTmp - 1);
    Value.TempC = decimal.Parse(sTmp);
}
else if (line.Contains("-")) {
    // Parse date: "20-Feb-2023"
    Value.Date = DateTime.ParseExact(line, "dd-MMM-yyyy", ...);
}
else if (line.Contains(":")) {
    // Parse time: "11:11"
    Value.Date = DateTime.ParseExact(line, "HH:mm", ...);
}
```

**Challenges**:
- **Non-ASCII characters** (`0xF8` = degree symbol)
- Compound values (number + unit together)
- Multi-line timestamp
- Mixed message types (reading vs print report)
- Empty line as field

**Protocol Analyzer Must Detect**:
- Content-based parsing (if/else on line content)
- Multiple line patterns in same message
- Special byte handling (0xF8)
- DateTime format detection ("dd-MMM-yyyy", "HH:mm")
- Compound field detection ("3.01pH", "25.5°C")

---

### 5. **Mettler Toledo Scientific Format**

**Example: MS204TS00**
```
     N       0.3749 g   ..
     N       0.3747 g   ..
```

**Structure**:
- Leading spaces
- Status: `N` (Net/Stable?)
- Value with precision
- Unit
- Suffix: `..` (stability indicator?)

**Challenges**:
- Fixed-width fields with leading spaces
- Multiple space delimiter
- Cryptic status codes
- Suffix indicators

---

## Core Problems Identified

### Problem 1: **Message Boundary Detection**
**NOT** one file line = one message!

Examples:
- **JIK6CAB**: 14 file lines = 1 message block
- **PH Meter**: Variable lines per message (simple reading vs print report)
- **TFO1**: Multi-line frame structure

**Solution Needed**:
1. Parse all bytes from all file lines
2. Build continuous byte stream
3. Detect message boundaries by:
   - Header/footer markers
   - Terminator patterns
   - Message length patterns
   - Repeating structures

---

### Problem 2: **Mixed File Formats**
Some logs mix format types!

**Example: WEIGHT QA**
```
Line 1-12:  Text only
Line 13:    Empty
Line 14:    Empty
Line 15-22: HEX/Text mixed
Line 23-28: Text only
Line 29:    Empty
Line 30:    Comment "-- Generate"
Line 31+:   HEX/Text mixed
```

**Solution Needed**:
- Detect format **per-section**, not per-file
- Handle format transitions
- Ignore comments and separators

---

### Problem 3: **Non-Printable and Special Characters**

**Examples**:
- `0xF8` = Degree symbol (°)
- `0x83` = Special marker in TFO1
- `0x0D 0x0A` = CRLF
- `^K` = Control character
- `~P1` = Command marker

**Solution Needed**:
- Preserve ALL bytes (don't assume ASCII)
- Detect non-printable characters
- Handle encoding variations
- Support custom character mappings

---

### Problem 4: **Variable Field Delimiters**

Different devices use different delimiters (from both log analysis AND production code):

| Device | Delimiter Type | Production Code Strategy | Example |
|--------|---------------|-------------------------|---------|
| DEFENDER3000 | Multiple spaces (variable count) | String.Split(" ", RemoveEmptyEntries) | `- 1.640 kg    N` |
| WEIGHT QA | Slash + space | Nested Split("/" then " ") | `+007.12/3 G S` |
| JIK6CAB | Line breaks | Each field on new line | Multi-line block |
| PH Meter | Space (but compound values!) | IndexOf + Substring | `25.5°C` no space |
| MS204TS00 | Multiple spaces (fixed width) | String.Split with padding | `     N       0.3749 g` |
| TFO1 | Spaces (fixed width) | Switch + GetString(offset, length) | `F      0.0` |

**TFO1's Advanced Parsing** (TFO1.cs:464-685):
```csharp
char hdr = (char)content[0];  // First byte identifies field type
switch (hdr) {
    case 'F':
        // Fixed position extraction
        string val = Encoding.ASCII.GetString(content, 1, 9);
        Value.F = decimal.Parse(val);
        break;
    case 'B':
        Value.B = content[1];  // Direct binary byte
        break;
    case 'C':  // Complex: mix of ASCII + special bytes
        string _dd = ASCII.GetString(content, 1, 2);
        // content[3] is 0xF4 (separator)
        string _mm = ASCII.GetString(content, 5, 2);
        // content[7] is 0xF3 (separator)
        Value.C = new DateTime(yy, mm, dd, hh, mi, 0);
        break;
}
```

**Solution Needed**:
- Statistical analysis of spacing patterns
- Detect fixed-width vs delimited
- Handle compound values (no delimiter)
- Multiple delimiter types in same message
- Support header-based field identification (first byte switch/case)
- Mix of ASCII text and binary bytes in same protocol

---

### Problem 5: **Embedded Metadata vs Data**

Some messages contain both:

**PH Meter Example**:
```
3.01pH              ← DATA (measurement)
25.5°C ATC          ← DATA (temperature + mode)
20-Feb-2023         ← METADATA (timestamp)
11:11               ← METADATA (time)
Auto EP Standard    ← METADATA (method)
Blank               ← METADATA (sample type)
```

**Solution Needed**:
- Classify fields as data vs metadata
- Metadata might not repeat in every message
- Data fields typically repeat/vary
- Statistical analysis of field variance

---

## Proposed Parsing Strategy

### Stage 1: **Byte Extraction**

```mermaid
flowchart TD
    A[Load File] --> B{Detect Line Format}
    B -->|HEX/Text| C[Extract HEX bytes]
    B -->|HEX Only| D[Parse HEX + Comments]
    B -->|Text Only| E[Convert to bytes]
    B -->|Mixed| F[Split by section]
    C --> G[Byte Stream]
    D --> G
    E --> G
    F --> G
    G --> H[Store as LogEntry array]
```

**Key Points**:
- Each LogEntry = bytes from ONE file line
- Preserve FileLineNumber
- Don't assume message boundaries yet
- Handle mixed formats

---

### Stage 2: **Message Boundary Detection**

```mermaid
flowchart TD
    A[LogEntry Array] --> B[Statistical Analysis]
    B --> C[Detect Terminators]
    C --> D[Detect Headers/Footers]
    D --> E[Detect Repeating Patterns]
    E --> F[Group into Messages]
    F --> G[Message Array]
```

**Algorithms**:

#### A. **Terminator Detection**
```
For each LogEntry:
  - Check last 1-4 bytes
  - Count frequency of patterns
  - Common patterns: 0x0D, 0x0A, 0x0D0A, custom sequences
  - Score by consistency (95%+ = likely terminator)
```

#### B. **Header/Footer Detection**
```
For LogEntry array:
  - Find entries with unique starting bytes
  - Find entries with unique ending bytes
  - Check if they appear regularly
  - Pattern: Header -> Data -> Footer -> Header...
```

#### C. **Block Pattern Detection**
```
Analyze entry sequences:
  - Look for repeating N-line blocks
  - Example: Lines 1-14, 15-28, 29-42 (14-line blocks)
  - Validate structure consistency within blocks
```

---

### Stage 3: **Field Structure Analysis**

```mermaid
flowchart TD
    A[Message Array] --> B[Analyze Each Message Type]
    B --> C{Delimiter Type?}
    C -->|Fixed-width| D[Detect Field Positions]
    C -->|Delimited| E[Find Delimiters]
    C -->|Mixed| F[Hybrid Analysis]
    D --> G[Field Map]
    E --> G
    F --> G
    G --> H[Field Definitions]
```

**Algorithms**:

#### A. **Fixed-Width Detection**
```
For each message:
  - Find positions where characters change
  - Detect space-padding patterns
  - Identify field boundaries by:
    - Consistent spacing
    - Data type transitions (alpha to numeric)
    - Alignment patterns (left/right)
```

#### B. **Delimiter Detection**
```
For each message:
  - Count byte frequency
  - High-frequency non-alphanumeric bytes = delimiter candidates
  - Common: 0x20 (space), 0x2F (slash), 0x2C (comma), 0x09 (tab)
  - Validate consistency across messages
```

#### C. **Compound Value Detection**
```
Detect patterns like "3.01pH" or "25.5°C":
  - Number immediately followed by alpha
  - No delimiter between
  - Treat as single field
```

---

### Stage 4: **Field Classification**

```mermaid
flowchart TD
    A[Field Definitions] --> B{Analyze Content}
    B --> C{Data Type?}
    C -->|Numeric| D[Detect Format]
    C -->|Alpha| E[Detect Type]
    C -->|Mixed| F[Compound Type]
    D --> G{Variance?}
    E --> G
    F --> G
    G -->|High| H[Data Field]
    G -->|Low| I[Status/Metadata]
    G -->|Zero| J[Fixed Label]
```

**Classification Rules**:

| Variance | Classification | Example |
|----------|---------------|---------|
| 0% (always same) | Fixed Label | "Auto EP Standard" |
| <10% | Status Code | "N", "G", "S" (few values) |
| 10-90% | Data Field | "1.640", "3.01pH" (many values) |
| >90% | Timestamp/ID | Always changing |

---

### Stage 5: **Pattern Validation**

```mermaid
flowchart TD
    A[Detected Patterns] --> B[Validate Against All Messages]
    B --> C{Confidence Score}
    C -->|>95%| D[Accept Pattern]
    C -->|80-95%| E[Flag for Review]
    C -->|<80%| F[Reject Pattern]
    D --> G[Protocol Definition]
    E --> H[User Validation]
    F --> I[Try Alternative]
```

**Confidence Scoring**:
```
Score = (Matching Messages / Total Messages) × 100

Examples:
- Terminator 0x0D found in 950/1000 messages = 95% → ACCEPT
- Delimiter "/" found in 850/1000 messages = 85% → REVIEW
- Pattern detected in 500/1000 messages = 50% → REJECT
```

---

## Implementation Considerations

### 1. **Multi-Pass Analysis**
Cannot determine everything in one pass:
- **Pass 1**: Extract bytes
- **Pass 2**: Detect message boundaries
- **Pass 3**: Analyze field structure
- **Pass 4**: Classify fields
- **Pass 5**: Validate and score

### 2. **Probabilistic Approach**
Use statistics, not rules:
- Frequency analysis
- Variance analysis
- Pattern matching
- Confidence scoring

### 3. **User Override**
For the 5% that can't be auto-detected:
- Show detected patterns with confidence scores
- Allow user to confirm/reject
- Manual field definition option
- Save user corrections for learning

### 4. **Incremental Processing**
Don't try to solve everything at once:
- Start with terminator detection (easiest)
- Then field delimiters
- Then field types
- Then metadata classification
- Each stage uses previous results

---

## Algorithm Priority

### High Priority (Must Solve)
1. ✓ **Byte extraction from all 3 file formats**
2. ✓ **Protocol terminator detection** (0x0D, 0x0A, etc.)
3. ✓ **Message boundary detection** (multi-line blocks)
4. ✓ **Field delimiter detection** (spaces, /, etc.)

### Medium Priority (Important)
5. ✓ **Fixed-width field detection**
6. ✓ **Field data type classification**
7. ✓ **Header/footer marker detection**

### Lower Priority (Nice to Have)
8. ◯ **Metadata vs data classification**
9. ◯ **Compound value detection** ("3.01pH")
10. ◯ **Non-ASCII character mapping** (0xF8 → °)

---

## Success Metrics

To achieve **95%+ accuracy**:

1. **Terminator Detection**: Must correctly identify in 95%+ of devices
2. **Message Boundaries**: Must group multi-line messages correctly
3. **Field Delimiters**: Must detect primary delimiter type
4. **Field Count**: Must identify correct number of fields ±1
5. **Data Type**: Must classify numeric vs text fields correctly

---

## Example: JIK6CAB Analysis

**Input (14 file lines)**:
```
^KJIK000
2023-11-07
17:19:38
  0.00 kg
  1.94 kg
0
0
  1.94 kg
  1.94 kg
    0 pcs


E
~P1
```

**Analysis Process**:

### Pass 1: Byte Extraction
```
14 LogEntry objects created
FileLineNumber: 1-14
Bytes extracted (ASCII encoding)
```

### Pass 2: Message Boundary Detection
```
Detected patterns:
- Starts with: ^KJIK000 (100% confidence)
- Ends with: ~P1 (100% confidence)
- Lines 11-12: Empty (0x0D0A only)
- Message = Lines 1-14 (one block)
```

### Pass 3: Field Structure
```
Line 1:  Header marker
Line 2:  Date (YYYY-MM-DD pattern)
Line 3:  Time (HH:MM:SS pattern)
Line 4-10: Data fields (mixed format)
Line 11-12: Empty lines
Line 13-14: Footer markers
```

### Pass 4: Field Classification
```
Field 1: Header (fixed: "^KJIK000")
Field 2: Date (metadata, format: YYYY-MM-DD)
Field 3: Time (metadata, format: HH:MM:SS)
Field 4: Tare weight (data: decimal + unit)
Field 5: Gross weight (data: decimal + unit)
Field 6-7: Status codes (data: integer)
Field 8-9: Net weight (data: decimal + unit)
Field 10: Count (data: integer + unit)
Field 11: Footer 1 (fixed: "E")
Field 12: Footer 2 (fixed: "~P1")
```

### Pass 5: Generate Definition
```json
{
  "deviceName": "JIK6CAB",
  "messageStructure": "multi-line-block",
  "messageStart": "^KJIK000",
  "messageEnd": "~P1",
  "lineTerminator": "\\r\\n",
  "fields": [...]
}
```

---

## Production Code Insights Summary

From analyzing the existing Terminal classes, we identified **four distinct parsing patterns**:

| Pattern | Devices | Characteristics | Auto-Detection Strategy |
|---------|---------|----------------|------------------------|
| **String Splitting** | WeightQA, CordDEFENDER3000 | Simple delimiters (/, spaces) | High delimiter frequency analysis |
| **Content-Based** | PHMeter | If/else on line content | Pattern variance across lines |
| **Fixed-Position** | TFO1 | Switch on header byte + byte offsets | Consistent field positions |
| **Hybrid** | JIK6CAB | Multi-line blocks with markers | Header/footer marker detection |

**Key Learning**: The Protocol Analyzer must automatically determine which pattern(s) apply to each device by:
1. Statistical analysis (frequency of delimiters, terminators)
2. Positional analysis (fixed vs variable width fields)
3. Content analysis (header bytes, markers, patterns)
4. Variance analysis (which fields change vs stay constant)

This validates the **5-stage parsing strategy** defined earlier in this document.

---

## Advanced Parsing Strategies

### State Machine Parsing (Sequential Lines)

**Purpose**: Parse complex multi-line protocols where each line must be processed sequentially in order.

**Best For**: JIK6CAB-style protocols with:
- Fixed line count per message
- Start/end markers
- Each line has specific meaning based on position
- Mixed data types across lines

#### State Machine Diagram

```mermaid
stateDiagram-v2
    [*] --> Idle

    Idle --> ParsingStarted : Detect StartMarker\n(^KJIK000)

    ParsingStarted --> ParseLine1 : lineCount = 1
    ParseLine1 --> ParseLine2 : Parse Date
    ParseLine2 --> ParseLine3 : Parse Time
    ParseLine3 --> ParseLine4 : Parse TareWeight
    ParseLine4 --> ParseLine5 : Parse GrossWeight
    ParseLine5 --> SkipLine6 : Parse Reserved1
    SkipLine6 --> SkipLine7 : Skip Reserved2
    SkipLine7 --> ParseLine8 : Parse NetWeight
    ParseLine8 --> SkipLine9 : Skip DisplayWeight
    SkipLine9 --> ParseLine10 : Parse PieceCount
    ParseLine10 --> SkipLine11 : Skip Empty1
    SkipLine11 --> SkipLine12 : Skip Empty2
    SkipLine12 --> ValidateLine13 : Skip StatusIndicator
    ValidateLine13 --> Complete : Validate EndMarker\n(~P1)

    ParsingStarted --> Error : Timeout exceeded
    ValidateLine13 --> Error : Invalid EndMarker

    Complete --> ApplyRelationships : Combine Date+Time
    ApplyRelationships --> ApplyValidation : Apply field relationships
    ApplyValidation --> FireEvent : Validate data
    FireEvent --> Idle : Fire DataReceived event

    Error --> Idle : Reset state

    note right of ParsingStarted
        Reset all variables
        lineCount = 0
        bCompleted = false
    end note

    note right of Complete
        Package complete
        All 14 lines processed
    end note
```

#### Parsing Flow Diagram

```mermaid
flowchart TD
    A[Receive Line] --> B{Current State?}

    B -->|Idle| C{StartMarker?}
    C -->|Yes| D[State = ParsingStarted<br/>Reset Variables<br/>lineCount = 0]
    C -->|No| A

    B -->|ParsingStarted| E[lineCount++]
    E --> F{lineCount}

    F -->|1| G1[Parse Date<br/>YYYY-MM-DD]
    F -->|2| G2[Parse Time<br/>HH:MM:SS]
    F -->|3| G3[Parse TareWeight<br/>decimal + kg]
    F -->|4| G4[Parse GrossWeight<br/>decimal + kg]
    F -->|5-6| G5[Skip Reserved<br/>Not needed]
    F -->|7| G6[Parse NetWeight<br/>decimal + kg]
    F -->|8| G7[Skip DisplayWeight<br/>Duplicate]
    F -->|9| G8[Parse PieceCount<br/>integer + pcs]
    F -->|10-12| G9[Skip Empty Lines<br/>Whitespace only]
    F -->|13| G10{EndMarker?}

    G1 --> A
    G2 --> A
    G3 --> A
    G4 --> A
    G5 --> A
    G6 --> A
    G7 --> A
    G8 --> A
    G9 --> A

    G10 -->|Valid ~P1| H[State = Complete]
    G10 -->|Invalid| I[State = Error]

    B -->|Complete| J[Combine Date + Time]
    J --> K[Apply Validation Rules]
    K --> L[Fire DataReceived Event]
    L --> M[State = Idle]

    B -->|Error| N[Reset to Idle]

    I --> N
    M --> A
    N --> A
```

#### State Machine Algorithm

```
State: IDLE
├─ Receive line
├─ IF line matches StartMarker (e.g., "^KJIK000")
│  ├─ TRANSITION to PARSING_STARTED
│  ├─ Reset all field variables
│  ├─ lineCount = 0
│  └─ Set completedFlag = false
└─ ELSE stay in IDLE

State: PARSING_STARTED
├─ lineCount++
├─ SWITCH on lineCount:
│  ├─ CASE 1: Parse Date (pattern: YYYY-MM-DD)
│  ├─ CASE 2: Parse Time (pattern: HH:MM:SS)
│  ├─ CASE 3: Parse TareWeight (pattern: \d+\.\d+ kg)
│  ├─ CASE 4: Parse GrossWeight (pattern: \d+\.\d+ kg)
│  ├─ CASE 5-6: Skip (reserved fields)
│  ├─ CASE 7: Parse NetWeight (pattern: \d+\.\d+ kg)
│  ├─ CASE 8: Skip (duplicate display weight)
│  ├─ CASE 9: Parse PieceCount (pattern: \d+ pcs)
│  ├─ CASE 10-11: Skip (empty lines)
│  ├─ CASE 12: Skip (status indicator)
│  └─ CASE 13: Validate EndMarker (pattern: "~P1")
│     ├─ IF valid: TRANSITION to COMPLETE
│     └─ ELSE: ERROR → IDLE
└─ IF timeout exceeded: ERROR → IDLE

State: COMPLETE
├─ Combine Date + Time → DateTime property
├─ Apply validation rules
├─ Fire DataReceived event
└─ TRANSITION to IDLE
```

#### Implementation Pattern

```csharp
public class SequentialLineParser
{
    private ParserState state = ParserState.Idle;
    private int lineCount = 0;
    private Dictionary<string, object> fieldValues = new Dictionary<string, object>();
    private DateTime? date;
    private TimeSpan? time;

    public void ProcessLine(string line)
    {
        switch (state)
        {
            case ParserState.Idle:
                if (line.Contains(StartMarker))
                {
                    state = ParserState.Parsing;
                    lineCount = 0;
                    fieldValues.Clear();
                }
                break;

            case ParserState.Parsing:
                lineCount++;
                LineDefinition lineDef = config.Lines[lineCount - 1];

                switch (lineDef.Action)
                {
                    case LineAction.Parse:
                        var value = ExtractValue(line, lineDef);
                        fieldValues[lineDef.FieldName] = value;
                        break;

                    case LineAction.Skip:
                        // Do nothing, just advance
                        break;

                    case LineAction.Validate:
                        if (!ValidatePattern(line, lineDef.Pattern))
                            state = ParserState.Error;
                        break;

                    case LineAction.Marker:
                        if (lineDef.Pattern == EndMarker && line.Contains(EndMarker))
                        {
                            state = ParserState.Complete;
                            OnPackageComplete();
                        }
                        break;
                }
                break;

            case ParserState.Complete:
                // Already handled
                state = ParserState.Idle;
                break;
        }
    }
}
```

#### Key Features:
- ✅ **Ordered Processing**: Lines must be processed in sequence
- ✅ **State Tracking**: Knows current position in message
- ✅ **Skip Lines**: Can skip reserved/empty lines
- ✅ **Marker Validation**: Verifies start/end markers
- ✅ **Timeout Handling**: Resets if incomplete package

---

### Validation Rules Strategy

**Purpose**: Apply data integrity checks after parsing or before serialization.

#### Validation Architecture Diagram

```mermaid
flowchart TD
    A[Parsed Data Object] --> B[Validation Engine]

    B --> C{For Each Rule}

    C --> D1[Range Validation<br/>Min/Max Check]
    C --> D2[Formula Validation<br/>GW - TW = NW]
    C --> D3[DateTime Validation<br/>Date Range Check]
    C --> D4[Field Relationship<br/>GW >= TW]
    C --> D5[Custom Expression<br/>User Defined]

    D1 --> E1{Pass?}
    D2 --> E2{Pass?}
    D3 --> E3{Pass?}
    D4 --> E4{Pass?}
    D5 --> E5{Pass?}

    E1 -->|Yes| F[Continue]
    E1 -->|No| G1{Severity?}
    G1 -->|Error| H[Reject Data]
    G1 -->|Warning| I[Accept with Warning]
    G1 -->|Info| F

    E2 -->|Yes| F
    E2 -->|No| G2{Severity?}
    G2 -->|Error| H
    G2 -->|Warning| I
    G2 -->|Info| F

    E3 -->|Yes| F
    E3 -->|No| G3{Severity?}
    G3 -->|Error| H
    G3 -->|Warning| I
    G3 -->|Info| F

    E4 -->|Yes| F
    E4 -->|No| G4{Severity?}
    G4 -->|Error| H
    G4 -->|Warning| I
    G4 -->|Info| F

    E5 -->|Yes| F
    E5 -->|No| G5{Severity?}
    G5 -->|Error| H
    G5 -->|Warning| I
    G5 -->|Info| F

    F --> J{More Rules?}
    J -->|Yes| C
    J -->|No| K[All Passed]

    K --> L[Accept Data]
    I --> L
    H --> M[Return Error Result]
```

#### Formula Validation Detail

```mermaid
flowchart LR
    A["Formula:<br/>GW - TW = NW<br/>Tolerance: 0.01"] --> B[Parse Formula]

    B --> C[Extract Fields<br/>GrossWeight<br/>TareWeight<br/>NetWeight]

    C --> D[Get Field Values<br/>GW = 1.94<br/>TW = 0.00<br/>NW = 1.94]

    D --> E[Evaluate Left Side<br/>GW - TW<br/>= 1.94 - 0.00<br/>= 1.94]

    E --> F[Evaluate Right Side<br/>NW<br/>= 1.94]

    F --> G[Calculate Difference<br/>|1.94 - 1.94|<br/>= 0.00]

    G --> H{diff <= tolerance?<br/>0.00 <= 0.01}

    H -->|Yes| I[PASS]
    H -->|No| J[FAIL<br/>Show Message]
```

#### Validation Types

1. **Range Validation**
   ```csharp
   // Field value must be within min-max
   Rule: TareWeight >= 0 && TareWeight <= 999.99
   ```

2. **Formula Validation**
   ```csharp
   // Formula must evaluate correctly
   Rule: GrossWeight - TareWeight = NetWeight
   Tolerance: ±0.01

   Algorithm:
   1. Parse formula: "GrossWeight - TareWeight = NetWeight"
   2. Extract field names: [GrossWeight, TareWeight, NetWeight]
   3. Get field values from parsed data
   4. Evaluate: |GW - TW - NW| <= tolerance
   5. If true: PASS, else: FAIL
   ```

3. **DateTime Range Validation**
   ```csharp
   // DateTime must be within range
   Rule: Date >= 2020-01-01 && Date <= 2099-12-31
   ```

4. **Field Relationship Validation**
   ```csharp
   // One field must be related to another
   Rule: GrossWeight >= TareWeight
   ```

5. **Custom Expression Validation**
   ```csharp
   // Custom condition
   Rule: IF PieceCount > 0 THEN NetWeight > 0
   ```

#### Validation Execution Flow

```
Parse Data
   ↓
Store in Data Object
   ↓
Apply Validation Rules
   ↓
   ├─ Rule 1: Range checks → PASS/FAIL
   ├─ Rule 2: Formula checks → PASS/FAIL
   ├─ Rule 3: DateTime checks → PASS/FAIL
   └─ Rule N: Custom checks → PASS/FAIL
   ↓
Collect Results
   ↓
   ├─ All PASS → Accept Data
   ├─ Any ERROR severity → Reject Data
   └─ Only WARNING severity → Accept with Warnings
```

#### Implementation Pattern

```csharp
public class ValidationEngine
{
    public ValidationResult Validate(object data, List<ValidationRule> rules)
    {
        var result = new ValidationResult { IsValid = true };

        foreach (var rule in rules.Where(r => r.Enabled))
        {
            switch (rule.Type)
            {
                case ValidationType.Range:
                    ValidateRange(data, rule, result);
                    break;

                case ValidationType.Formula:
                    ValidateFormula(data, rule, result);
                    break;

                case ValidationType.DateTimeRange:
                    ValidateDateTimeRange(data, rule, result);
                    break;

                case ValidationType.FieldRelationship:
                    ValidateFieldRelationship(data, rule, result);
                    break;

                case ValidationType.Custom:
                    ValidateCustom(data, rule, result);
                    break;
            }

            // If any ERROR severity fails, mark entire validation as failed
            if (rule.Severity == ValidationSeverity.Error && !result.IsValid)
                break;
        }

        return result;
    }

    private void ValidateFormula(object data, ValidationRule rule, ValidationResult result)
    {
        // Example: "GrossWeight - TareWeight = NetWeight"
        var formula = ParseFormula(rule.Formula);

        double leftSide = EvaluateExpression(formula.LeftSide, data);
        double rightSide = EvaluateExpression(formula.RightSide, data);

        double diff = Math.Abs(leftSide - rightSide);

        if (diff > (rule.Tolerance ?? 0.001))
        {
            result.AddError(rule.Severity, rule.Message);
            if (rule.Severity == ValidationSeverity.Error)
                result.IsValid = false;
        }
    }
}
```

---

### Field Relationship Strategy

**Purpose**: Handle combined, split, or calculated fields.

#### Field Relationship Architecture

```mermaid
flowchart TD
    A[Parsed Fields] --> B[Field Relationship Processor]

    B --> C{Relationship Type?}

    C -->|Combine| D1[Combine Fields]
    C -->|Split| D2[Split Field]
    C -->|Calculate| D3[Calculate Field]
    C -->|Derive| D4[Derive Field]

    D1 --> E1["Example:<br/>Date + Time → DateTime"]
    D2 --> E2["Example:<br/>'1.94 kg' → Value + Unit"]
    D3 --> E3["Example:<br/>GW - TW → NetWeight"]
    D4 --> E4["Example:<br/>Extract Unit from Weight"]

    E1 --> F1[Get Source Fields<br/>Date: 2023-11-07<br/>Time: 17:19:38]
    E2 --> F2[Get Source Field<br/>'  1.94 kg']
    E3 --> F3[Get Source Fields<br/>GW: 1.94<br/>TW: 0.00]
    E4 --> F4[Get Source Field<br/>'1.94 kg']

    F1 --> G1[Apply Operation<br/>Date.Date + Time]
    F2 --> G2[Apply Regex<br/>Extract \d+\.\d+]
    F3 --> G3[Apply Formula<br/>GW - TW]
    F4 --> G4[Apply Pattern<br/>Extract kg|g]

    G1 --> H1[Set Target Field<br/>DateTime = 2023-11-07 17:19:38]
    G2 --> H2[Set Target Field<br/>TareWeight = 1.94]
    G3 --> H3[Set Target Field<br/>NetWeight = 1.94]
    G4 --> H4[Set Target Field<br/>TareUnit = 'kg']

    H1 --> I[Updated Data Object]
    H2 --> I
    H3 --> I
    H4 --> I
```

#### Combine Fields Detail (Date + Time → DateTime)

```mermaid
flowchart LR
    A[Line 2:<br/>'2023-11-07'] --> B[Parse as Date<br/>DateTime object]
    C[Line 3:<br/>'17:19:38'] --> D[Parse as Time<br/>TimeSpan object]

    B --> E[Date Field<br/>Year: 2023<br/>Month: 11<br/>Day: 7<br/>Time: 00:00:00]

    D --> F[Time Field<br/>Hours: 17<br/>Minutes: 19<br/>Seconds: 38]

    E --> G[Combine Operation<br/>Date.Date + Time]
    F --> G

    G --> H[Result:<br/>DateTime Property<br/>2023-11-07 17:19:38]

    style H fill:#90EE90
```

#### Split Field Detail (Value + Unit Extraction)

```mermaid
flowchart TD
    A["Line 4:<br/>'  1.94 kg'"] --> B[Apply Regex 1<br/>Pattern: \d+\.\d+]
    A --> C[Apply Regex 2<br/>Pattern: kg|g]

    B --> D[Match: '1.94'<br/>Parse as decimal]
    C --> E[Match: 'kg'<br/>Store as string]

    D --> F[TareWeight Property<br/>Type: decimal<br/>Value: 1.94]
    E --> G[TareUnit Property<br/>Type: string<br/>Value: 'kg']

    style F fill:#87CEEB
    style G fill:#87CEEB
```

#### Relationship Types

**1. Combine Fields (Date + Time → DateTime)**

```csharp
// Input fields:
Line 2: "2023-11-07"  → Date field
Line 3: "17:19:38"    → Time field

// Relationship:
Type: Combine
SourceFields: ["Date", "Time"]
TargetField: "DateTime"
Operation: "Date.Date + Time"

// Output:
data.DateTime = new DateTime(2023, 11, 7, 17, 19, 38)
```

**2. Split Field (Value + Unit)**

```csharp
// Input field:
Line 4: "  1.94 kg"

// Relationship 1:
Type: Split
SourceFields: ["TareWeightLine"]
TargetField: "TareWeight"
Operation: "Extract(\d+\.\d+)"
Result: 1.94

// Relationship 2:
Type: Split
SourceFields: ["TareWeightLine"]
TargetField: "TareUnit"
Operation: "Extract(kg|g)"
Result: "kg"
```

**3. Calculate Field (NetWeight = Gross - Tare)**

```csharp
// Input fields:
GrossWeight: 1.94
TareWeight: 0.00

// Relationship:
Type: Calculate
SourceFields: ["GrossWeight", "TareWeight"]
TargetField: "NetWeight"
Operation: "GrossWeight - TareWeight"

// Output:
data.NetWeight = 1.94 - 0.00 = 1.94
```

#### Implementation Pattern

```csharp
public class FieldRelationshipProcessor
{
    public void ApplyRelationships(object data, List<FieldRelationship> relationships)
    {
        foreach (var rel in relationships)
        {
            switch (rel.Type)
            {
                case RelationshipType.Combine:
                    CombineFields(data, rel);
                    break;

                case RelationshipType.Split:
                    SplitField(data, rel);
                    break;

                case RelationshipType.Calculate:
                    CalculateField(data, rel);
                    break;
            }
        }
    }

    private void CombineFields(object data, FieldRelationship rel)
    {
        // Example: Date + Time → DateTime
        var dateValue = GetFieldValue<DateTime>(data, rel.SourceFields[0]);
        var timeValue = GetFieldValue<TimeSpan>(data, rel.SourceFields[1]);

        var combined = dateValue.Date + timeValue;

        SetFieldValue(data, rel.TargetField, combined);
    }

    private void SplitField(object data, FieldRelationship rel)
    {
        // Example: "1.94 kg" → Value=1.94, Unit="kg"
        var sourceValue = GetFieldValue<string>(data, rel.SourceFields[0]);

        var match = Regex.Match(sourceValue, rel.Operation);
        if (match.Success)
        {
            var extractedValue = match.Groups[1].Value;
            SetFieldValue(data, rel.TargetField, extractedValue);
        }
    }

    private void CalculateField(object data, FieldRelationship rel)
    {
        // Example: NetWeight = GrossWeight - TareWeight
        var values = rel.SourceFields
            .Select(f => GetFieldValue<decimal>(data, f))
            .ToArray();

        var result = EvaluateExpression(rel.Operation, values);

        SetFieldValue(data, rel.TargetField, result);
    }
}
```

---

## Enhanced Production Code Insights Summary

From analyzing the existing Terminal classes and adding advanced strategies, we identified **six parsing patterns**:

| Pattern | Devices | Characteristics | Auto-Detection Strategy |
|---------|---------|----------------|------------------------|
| **String Splitting** | WeightQA, CordDEFENDER3000 | Simple delimiters (/, spaces) | High delimiter frequency analysis |
| **Content-Based** | PHMeter | If/else on line content | Pattern variance across lines |
| **Fixed-Position** | TFO1 | Switch on header byte + byte offsets | Consistent field positions |
| **Sequential Lines** | JIK6CAB | Multi-line with state machine | Start/end markers + fixed line count ⭐ NEW |
| **Field Relationships** | JIK6CAB (Date+Time) | Combined/calculated fields | Multiple related fields ⭐ NEW |
| **Validation Rules** | All devices | Data integrity checks | Formula/range validation ⭐ NEW |

**Key Learning**: The Protocol Analyzer must automatically determine which pattern(s) apply to each device by:
1. Statistical analysis (frequency of delimiters, terminators)
2. Positional analysis (fixed vs variable width fields)
3. Content analysis (header bytes, markers, patterns)
4. Variance analysis (which fields change vs stay constant)
5. **Marker detection** (start/end patterns for state machine) ⭐ NEW
6. **Field correlation** (related fields for relationships) ⭐ NEW

This validates and enhances the **5-stage parsing strategy** defined earlier in this document.

---

**Document Version**: 2.1
**Last Updated**: 2025-10-21
**Status**: Design Phase - Complete with Diagrams & Advanced Strategies
**Changes**:
- v1.0: Initial parsing strategy with production code examples
- v1.1: Added production code parsing examples and insights
- v2.0: Added state machine parsing, validation rules, field relationships
- v2.1: Added comprehensive Mermaid diagrams (7 diagrams total):
  - Overview parsing strategy diagram
  - State machine diagram (stateDiagram-v2)
  - Parsing flow diagram (flowchart)
  - Validation architecture diagram
  - Formula validation detail diagram
  - Field relationship architecture diagram
  - Combine fields detail diagram
  - Split field detail diagram
