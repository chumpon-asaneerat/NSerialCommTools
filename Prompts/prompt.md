# Protocol Analyzer - Current Session Context (2025-10-22)

## ⚠️ CRITICAL: Implementation Status - Only 20% Complete

The Protocol Analyzer implementation is fundamentally incomplete. Major architectural components are missing.

---

## ⚠️ CRITICAL INSTRUCTION - DO NOT USE v1/v2 FOLDERS

**IMPORTANT**: Do NOT access, read, or modify ANY files in the `v1` or `v2` folders:
- ❌ `09.App\NLib.Serial.Protocol.Analyzer\v1\**` - OLD/ABANDONED CODE
- ❌ `Documents\ModernDesign\v1\**` - OLD/ABANDONED DOCUMENTS
- ❌ `Documents\ModernDesign\v2\**` - ARCHIVED HISTORICAL DOCUMENTS

**ONLY work with root-level files**:
- ✅ `09.App\NLib.Serial.Protocol.Analyzer\Models\` - CURRENT models
- ✅ `09.App\NLib.Serial.Protocol.Analyzer\Analyzers\` - CURRENT analyzers
- ✅ `09.App\NLib.Serial.Protocol.Analyzer\Parsers\` - CURRENT parsers
- ✅ `Documents\ModernDesign\*.md` - CURRENT design documents (root level only)

The v1 and v2 folders contain previous failed attempts and archived historical documents. They must be ignored completely.

---

## Project Purpose

Create a **Protocol Analyzer Tool** that:
1. Loads serial device log files (HEX/Text format)
2. **Automatically analyzes** protocol structure using pattern detection algorithms
3. **Generates bidirectional JSON definition files**
4. Definitions are used by **NTerminal&lt;T>** (parsing) and **NDevice&lt;T>** (serialization)

---

## Technology Stack

- **.NET Framework 4.7.2** (NOT .NET Core)
- **WPF Application** (Windows Presentation Foundation)
- **System.Text.Json** v9.0.0.0 (already in project)
- **C# Windows Forms/WPF**

---

## Key Architecture Components

### What's Implemented (20%)

✅ **Log File Loading**
- HEX/Text format parsing
- Message extraction
- Frame marker detection (^KJIK, ~P1)
- Multi-line frame extraction

✅ **Basic UI**
- TabControl with 4 tabs (Input, Analysis, Field Editor, Export)
- Field Editor with editable grid
- Sample values display

✅ **Pattern Analysis** (Partial)
- Detects Date/Time/Decimal patterns
- Analyzes line-by-line in frames
- Calculates variance and confidence

### What's Missing (80%) 🔴

❌ **Strategy Selection System**
- NO decision tree to choose parsing strategy
- Should select from 5 strategies: Delimiter, Frame, StateMachine, Position, Content

❌ **State Machine Parsing** ⭐ CRITICAL
- Cannot distinguish fields with identical patterns
- Example: "0.00 kg", "1.94 kg", "1.94 kg" → Cannot tell Tare vs Gross vs Net
- Needs position-based parsing: `switch(lineNumber)`

❌ **ParsePattern Generation**
- Should generate regex patterns for each field
- Example: `^\s*([\\d.]+)\s*kg\s*$` to extract weight value

❌ **FormatString Generation**
- Should generate format strings for serialization
- Example: `{0,7:F2} kg` for right-aligned weight with 2 decimals

❌ **Field Relationship Detection**
- Date + Time → DateTime combination
- Compound fields: "1.94 kg" → Value + Unit split
- Formula detection: GrossWeight - TareWeight = NetWeight

❌ **Validation Rule Generation**
- Range validation (min/max from samples)
- Formula validation (95%+ sample match)
- Relationship validation (GW >= TW)

❌ **JSON Definition Generator**
- The MAIN PURPOSE of the tool
- Should export complete JSON with fields, relationships, validation rules

❌ **Complete FieldInfo Model**
- Missing 15+ required properties
- Cannot hold ParsePattern, FormatString, etc.

❌ **Correct Validation Logic**
- Currently validates ALL fields for duplicates
- Should only validate DATA fields
- Markers/empty lines can have duplicate names

---

## Critical Mistakes Made This Session

### MISTAKE #1: Not Reading Documents Before Coding 🔴
- Continued from session summary without reading requirements
- Made incremental fixes instead of understanding complete architecture
- Wasted time on minor changes

### MISTAKE #2: Incomplete FieldInfo Model 🔴
**Current**: Only has Position, Name, Type, SampleValues, Confidence, IsConstant, MinLength, MaxLength

**Missing** (from requirements):
```csharp
// Parsing Direction (for NTerminal<T>)
public string ParsePattern { get; set; }     // Regex to extract value
public string DataType { get; set; }

// Serialization Direction (for NDevice<T>)
public string FormatString { get; set; }     // Format template
public string Alignment { get; set; }        // "left" or "right"
public string PaddingChar { get; set; }
public int Width { get; set; }

// Common
public string Terminator { get; set; }
public string Position { get; set; }         // "body"/"header"/"footer"
public int Order { get; set; }
public bool Required { get; set; }
public string Description { get; set; }

// Algorithm 4 Properties
public string FieldType { get; set; }        // "StartMarker", "Date", "Decimal-WithUnit", "Empty", etc.
public string Action { get; set; }           // "Parse", "Skip", "Validate"
public double Variance { get; set; }

// User Control
public bool IncludeInDefinition { get; set; }
public bool IsSkipped { get; set; }
```

### MISTAKE #3: No State Machine Parsing 🔴
**The Problem**: JIK6CAB has multiple lines with same pattern:
```
Line 4:  "  0.00 kg"   ← Tare Weight
Line 5:  "  1.94 kg"   ← Gross Weight
Line 8:  "  1.94 kg"   ← Net Weight
```

**Current**: Pattern analysis says "all are decimal kg" - cannot distinguish

**Required**: State Machine knows "Line 4 is ALWAYS Tare, Line 5 is ALWAYS Gross, Line 8 is ALWAYS Net"

```csharp
switch (lineNumber)
{
    case 4: tareWeight = ParseWeight(line, "kg"); break;
    case 5: grossWeight = ParseWeight(line, "kg"); break;
    case 8: netWeight = ParseWeight(line, "kg"); break;
}
```

### MISTAKE #4: Wrong Validation Logic 🔴
**Current**: Validates ALL fields for duplicate names

**Problem**: Empty lines and markers flagged as duplicates even though user doesn't care

**Correct**: Only validate DATA fields (FieldType != "Empty"/"Marker"/"Reserved")

### MISTAKE #5-10: See WORK-SUMMARY-2025-10-22.md for complete list

---

## 5 Parsing Strategies (from Document 03)

The Protocol Analyzer must choose the correct strategy:

1. **Delimiter-Based** - fields separated by delimiters (space, comma, etc.)
2. **Frame-Based** - start/end markers, content-based field detection
3. **State Machine** ⭐ - start/end markers, **position-based** field detection
4. **Position-Based** - fixed-width fields at byte offsets
5. **Content-Based** - variable structure, pattern matching

**JIK6CAB requires State Machine strategy** - position determines meaning, not content.

---

## Required JSON Output Format

```json
{
  "deviceName": "JIK6CAB",
  "version": "1.0",
  "encoding": "ASCII",
  "messageType": "multi-line-frame",
  "frameStart": "^KJIK\\d{3}\\r\\n",
  "frameEnd": "~P1\\r\\n",
  "fields": [
    {
      "name": "Date",
      "parsePattern": "^(\\d{4}-\\d{2}-\\d{2})$",
      "dataType": "DateTime",
      "formatString": "yyyy-MM-dd",
      "terminator": "\\r\\n",
      "position": "body",
      "order": 2,
      "required": true
    },
    {
      "name": "Time",
      "parsePattern": "^(\\d{2}:\\d{2}:\\d{2})$",
      "dataType": "TimeSpan",
      "formatString": "HH:mm:ss",
      "terminator": "\\r\\n",
      "position": "body",
      "order": 3,
      "required": true
    },
    {
      "name": "TareWeight",
      "parsePattern": "^\\s*([\\d.]+)\\s*kg\\s*$",
      "dataType": "decimal",
      "formatString": "{0,7:F2} kg",
      "alignment": "right",
      "width": 10,
      "terminator": "\\r\\n",
      "position": "body",
      "order": 4,
      "required": true
    },
    {
      "name": "GrossWeight",
      "parsePattern": "^\\s*([\\d.]+)\\s*kg\\s*$",
      "dataType": "decimal",
      "formatString": "{0,7:F2} kg",
      "alignment": "right",
      "width": 10,
      "terminator": "\\r\\n",
      "position": "body",
      "order": 5,
      "required": true
    },
    {
      "name": "NetWeight",
      "parsePattern": "^\\s*([\\d.]+)\\s*kg\\s*$",
      "dataType": "decimal",
      "formatString": "{0,7:F2} kg",
      "alignment": "right",
      "width": 10,
      "terminator": "\\r\\n",
      "position": "body",
      "order": 8,
      "required": true
    }
  ],
  "relationships": [
    {
      "type": "Combine",
      "sourceFields": ["Date", "Time"],
      "targetField": "DateTime",
      "operation": "Date.Date + Time"
    },
    {
      "type": "Calculate",
      "sourceFields": ["GrossWeight", "TareWeight"],
      "targetField": "NetWeight",
      "operation": "GrossWeight - TareWeight",
      "tolerance": 0.01
    }
  ],
  "validationRules": [
    {
      "type": "Range",
      "field": "TareWeight",
      "min": 0,
      "max": 100
    },
    {
      "type": "Formula",
      "formula": "GrossWeight - TareWeight = NetWeight",
      "tolerance": 0.01
    }
  ]
}
```

**This JSON generation is completely missing from current implementation!**

---

## Field Editor UI Requirements

**Current Columns**: Position, Name, Type, Sample Values

**Required Columns**:
- Position (read-only)
- Name (editable)
- Type (read-only, detected)
- Sample Values (read-only, shows examples)
- **Parse Pattern** (editable, regex)
- **Format String** (editable, C# format)
- **Width** (editable, number)
- **Alignment** (dropdown: left/right)
- **Include?** (checkbox)
- **Action** (dropdown: Parse/Skip/Validate)

---

## Validation Rules (from production app)

**WRONG Current Logic**:
```csharp
// Checks ALL fields - even markers and empty lines
var duplicates = fields.GroupBy(f => f.Name)
                       .Where(g => g.Count() > 1);
```

**CORRECT Logic**:
```csharp
// Only validate DATA fields
var dataFields = fields.Where(f =>
    f.FieldType != "StartMarker" &&
    f.FieldType != "EndMarker" &&
    f.FieldType != "Empty" &&
    f.FieldType != "Reserved" &&
    f.Variance > 0.1  // Has varying data
);

var duplicates = dataFields.GroupBy(f => f.Name)
                          .Where(g => g.Count() > 1);
```

**Why**: User doesn't care about naming empty lines or markers - only DATA fields need unique names for C# properties.

---

## Sample Data Location

```
Documents\LuckyTex Devices\
  ├── JIK6CAB\           ← Multi-line frame, State Machine parsing
  ├── TFO1\              ← Single-line, Delimiter-based
  ├── DEFENDER3000\      ← Delimiter-based
  ├── MS204TS00\         ← Position-based
  ├── PH Meter\          ← Content-based
  ├── TFO3\
  ├── WEIGHT QA\
  ├── WEIGHT SPUN\
  ├── TScaleNHB\
  └── TScaleQHW\
```

---

## Success Criteria

### For Next Session:

1. ✅ **Read ALL documents first** (00-06)
2. ✅ **Complete FieldInfo model** with all 25+ properties
3. ✅ **Implement State Machine parsing** for JIK6CAB
4. ✅ **Generate ParsePattern** from sample data
5. ✅ **Generate FormatString** from sample data
6. ✅ **Fix validation logic** (only validate data fields)
7. ✅ **Implement JSON generator** (main feature!)
8. ✅ **Test end-to-end**: Load JIK6CAB → Analyze → Edit → Export JSON

### Definition of Done:

- Load `Documents\LuckyTex Devices\JIK6CAB\jik_hex_1.txt`
- Auto-detect: Multi-line frame with State Machine strategy
- Extract 14 fields with correct meanings:
  - StartMarker, Date, Time, TareWeight, GrossWeight, Reserved, Reserved, NetWeight, NetWeightDup, PieceCount, Empty, Empty, Status, EndMarker
- User renames: TareWeight → TareKg, GrossWeight → GrossKg, NetWeight → NetKg
- Generate ParsePattern for each field (regex)
- Generate FormatString for each field (C# format)
- Detect relationships:
  - Date + Time → DateTime
  - GrossWeight - TareWeight = NetWeight
- Generate validation rules:
  - Range: TareKg (0-100), GrossKg (0-100), NetKg (0-100)
  - Formula: GrossKg - TareKg = NetKg (tolerance 0.01)
- Export complete JSON definition file
- JSON validates against schema
- File can be used by NTerminal&lt;T> and NDevice&lt;T>

---

## Key Documents (MUST READ FIRST)

1. **00-Requirements-Specification.md** - Complete requirements, use cases, field properties
2. **03-Parsing-Strategy-Analysis.md** - 6 algorithms, 5 strategies, State Machine details
3. **04-Data-Models-Design.md** - Data structures (verify against FieldInfo)
4. **05-JSON-Schema-Design.md** - JSON output format and schema
5. **06-Protocol-Analyzer-Complete-UI.md** - UI design, all tabs, all features

**DO NOT CODE UNTIL ALL DOCUMENTS ARE READ AND UNDERSTOOD!**

---

## Critical Instructions for Next Session

### STEP 1: Document Review (MANDATORY)
1. Read documents 00, 01, 02, 03, 04, 05, 06
2. Take notes on requirements
3. Verify current implementation against specs
4. Identify ALL gaps (not just obvious ones)

### STEP 2: Planning (MANDATORY)
1. Create comprehensive TODO file with all missing tasks
2. Break down into phases
3. Identify dependencies
4. Prioritize critical path

### STEP 3: Update Documents (if needed)
1. Check if algorithms match implementation needs
2. Update field models if incomplete
3. Add implementation notes to algorithm docs

### STEP 4: Implementation (Only After Steps 1-3)
1. Start with FieldInfo model (foundation)
2. Then implement Strategy Selection
3. Then implement State Machine parsing
4. Then implement Pattern Generators
5. Then implement JSON Generator
6. Finally update UI

### STEP 5: Testing
1. Test with JIK6CAB first (most complex)
2. Verify JSON output is complete
3. Test validation logic
4. Check end-to-end workflow

---

## DO NOT:

❌ Code before reading documents
❌ Make incremental fixes to broken architecture
❌ Assume previous implementation is correct
❌ Skip planning phase
❌ Implement UI before data model is complete
❌ Forget to generate JSON (main purpose!)

## DO:

✅ Read ALL documents first
✅ Plan comprehensively
✅ Build foundation first (FieldInfo model)
✅ Implement core algorithms (Strategy Selection, State Machine)
✅ Generate ParsePattern and FormatString
✅ Export complete JSON definitions
✅ Test end-to-end

---

## Mistakes Summary (This Session)

1. Not reading documents before coding
2. Incomplete FieldInfo model (missing 15+ properties)
3. No State Machine parsing
4. Wrong validation logic (validates all fields)
5. No strategy selection system
6. No ParsePattern generation
7. No FormatString generation
8. No field relationship detection
9. No validation rule generation
10. No JSON definition generator

**Result**: Only 20% complete, need major redesign

---

## Reference Files

- **Work Summary**: `Documents\ModernDesign\WORK-SUMMARY-2025-10-22.md`
- **TODO File**: `Documents\ModernDesign\TODO-PROTOCOL-ANALYZER.md`
- **Current Code**: `09.App\NLib.Serial.Protocol.Analyzer\`

---

**Last Updated**: 2025-10-22
**Status**: Implementation incomplete - requires comprehensive redesign
**Next Session**: Document review → Planning → Core implementation
