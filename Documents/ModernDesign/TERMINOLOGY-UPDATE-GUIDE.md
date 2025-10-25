# Terminology Update Guide

**Date**: 2025-10-26
**Purpose**: Complete guide for updating all documentation and code from text-biased terminology to binary-compatible terminology

---

## Overview

The project is transitioning from text-based terminology (Line, Frame) to protocol-agnostic terminology (Package, Segment) to support both text AND binary protocols.

**Status**: ✅ **Document 03 Updated**, Documents 04-06 need manual updates

---

## Complete Terminology Mapping

### Core Terms

| Old Term (Text-Biased) | New Term (Binary-Compatible) | Context |
|------------------------|------------------------------|---------|
| **Line** | **Segment** | Sub-unit within a package |
| **Frame** | **Package** | Complete data unit from device |
| **Message** | **Package** | When referring to protocol data units |
| **Multi-Line** | **Multi-Segment** or **PackageBased** | Protocol with multiple segments |
| **SingleLine** | **SinglePackage** | Protocol with one segment per package |

### Properties & Variables

| Old | New | Usage |
|-----|-----|-------|
| `LineNumber` | `SegmentIndex` | Position within package (0-based or 1-based) |
| `FileLineNumber` | `FileLineNumber` | **Keep** - refers to log file line number |
| `lines_per_frame` | `segments_per_package` | Count of segments in package |
| `line_count` | `segment_count` | Total segments |
| `expected_lines` | `expected_segments` | Expected count |
| `currentLine` | `currentSegment` | State machine position tracker |

### Classes & Models

| Old Class Name | New Class Name | Purpose |
|----------------|----------------|---------|
| `LineStructure` | `SegmentStructure` | Structure of segment in package |
| `LineDefinition` | `SegmentDefinition` | Definition of single segment |
| `LineSequenceConfig` | `SegmentSequenceConfig` | Sequential parsing config |
| `LineAction` | `SegmentAction` | Action for segment (Parse/Skip/Validate) |
| `FrameInfo` | `PackageInfo` | ❌ Actually OK - `PackageInfo` already used |
| `FrameTerminator` | `PackageTerminator` | Terminator for package |

### Enumerations

#### MessageStructure Enum
```csharp
// OLD
public enum MessageStructure
{
    Unknown = 0,
    SingleLine = 1,
    MultiLineFrame = 2,
    ...
}

// NEW
public enum MessageStructure
{
    Unknown = 0,
    SinglePackage = 1,      // One segment = one package
    MultiPackage = 2,       // Multi-segment package
    ...
}
```

#### ParsingStrategy Enum
```csharp
// OLD
public enum ParsingStrategy
{
    Unknown = 0,
    SingleLine = 1,
    MultiLineFrame = 2,
    StateMachine = 3,
    ContentBased = 4,
    ...
}

// NEW
public enum ParsingStrategy
{
    Unknown = 0,
    SinglePackage = 1,      // One segment per package
    PackageBased = 2,       // Multi-segment package
    StateMachine = 3,       // Segment position determines meaning
    ContentBased = 4,       // Segment content determines type
    ...
}
```

#### SegmentAction Enum (formerly LineAction)
```csharp
// OLD
public enum LineAction
{
    Parse = 1,
    Skip = 2,
    Validate = 3,
    Marker = 4
}

// NEW
public enum SegmentAction
{
    Parse = 1,          // Parse value from this segment
    Skip = 2,           // Skip this segment
    Validate = 3,       // Validate segment exists
    Marker = 4          // Segment is start/end marker
}
```

### Methods & Functions

| Old Method Name | New Method Name |
|-----------------|-----------------|
| `ExtractLine()` | `ExtractSegment()` |
| `ProcessFrame()` | `ProcessPackage()` |
| `SplitFrames()` | `SplitPackages()` |
| `ParseLine()` | `ParseSegment()` |
| `AnalyzeLinePattern()` | `AnalyzeSegmentPattern()` |
| `DetectMessageBoundaries()` | `DetectPackageBoundaries()` |

### JSON Schema Properties

| Old Property | New Property |
|--------------|--------------|
| `"messageStructure": "single-line"` | `"messageStructure": "single-package"` |
| `"messageStructure": "multi-line-frame"` | `"messageStructure": "multi-package"` |
| `"lineSequence"` | `"segmentSequence"` |
| `"lines"` | `"segments"` |
| `"lineNumber"` | `"segmentIndex"` |

---

## Files That Need Updates

### ✅ Already Updated
- `03-Parsing-Strategy-Analysis.md` - **COMPLETE**

### ⏳ Need Manual Updates

#### Document 04 (04-Data-Models-Design.md) - 60 occurrences
Key updates needed:
```csharp
// Line 51-77: LogEntry class comments
"Represents a single line from the log file"
→ "Represents a single entry from the log file"

// Line 1371-1388: LineSequenceConfig class
class LineSequenceConfig → class SegmentSequenceConfig
ExpectedLineCount → ExpectedSegmentCount
List<LineDefinition> Lines → List<SegmentDefinition> Segments

// Line 1422-1468: LineDefinition class
class LineDefinition → class SegmentDefinition
int LineNumber → int SegmentIndex
"Line action" → "Segment action"

// Line 1559-1560: MessageStructure enum
SingleLine = 1 → SinglePackage = 1
MultiLineFrame = 2 → MultiPackage = 2

// Line 1680-1688: LineAction enum
enum LineAction → enum SegmentAction

// Line 1701-1704: ParsingStrategy enum
SingleLine = 1 → SinglePackage = 1
MultiLineFrame = 2 → PackageBased = 2
"Sequential line-by-line" → "Sequential segment-by-segment"
```

#### Document 05 (05-JSON-Schema-Design.md) - 30 occurrences
Key updates needed:
```json
// Line 92-94: messageStructure property
"enum": ["single-line", "multi-line-frame", ...]
→ "enum": ["single-package", "multi-package", ...]

// Line 96-99: messageHeader/messageFooter comments
"Header pattern for multi-line messages"
→ "Header pattern for multi-segment packages"

// Throughout: Field definitions
"lineNumber" → "segmentIndex"
"lineAction" → "segmentAction"
```

#### Document 06 (06-Protocol-Analyzer-Complete-UI.md) - 3 occurrences
Minimal updates needed (verify only)

---

## Code Files - Current Project State

### ❌ IMPORTANT: v2 Folder - DO NOT ACCESS

**The v2 folder contains archived/cleaned up code and should NOT be accessed or referenced.**

### Current Implementation (09.App/NLib.Serial.Protocol.Analyzer/)

**Current Status**: Clean slate - MainWindow.xaml/.cs are empty and ready for new implementation.

```
09.App/NLib.Serial.Protocol.Analyzer/
├── MainWindow.xaml          ✅ Empty - Ready for new implementation
├── MainWindow.xaml.cs       ✅ Empty - Ready for new implementation
├── App.xaml                 ✅ Application entry point
├── v1/                      ⚠️  OLD CODE - Reference only (may have outdated terminology)
└── v2/                      ❌ DO NOT ACCESS - Archived code
```

### Implementation Guidelines:

**✅ DO:**
- Start fresh with empty MainWindow
- Create new folders: Models/, Analyzers/, Parsers/, ViewModels/
- Use Package/Segment terminology from the start
- Follow design documents (03-06)

**❌ DO NOT:**
- Access or copy code from v2 folder
- Copy v1 code without updating terminology
- Use old Line/Frame terminology

---

## Update Checklist

### Phase 1: Documentation
- [x] Update 03-Parsing-Strategy-Analysis.md
- [ ] Update 04-Data-Models-Design.md
- [ ] Update 05-JSON-Schema-Design.md
- [ ] Update 06-Protocol-Analyzer-Complete-UI.md

### Phase 2: Code Models
- [ ] Rename `LineStructure.cs` to `SegmentStructure.cs`
- [ ] Update `SegmentStructure` properties
- [ ] Create/Update `SegmentSequenceConfig` class
- [ ] Create/Update `SegmentDefinition` class
- [ ] Update `SegmentAction` enum

### Phase 3: Code Implementation (Start Fresh)
- [ ] Create new Models/ folder with Package/Segment classes
- [ ] Create new Analyzers/ folder with statistical detectors
- [ ] Create new Parsers/ folder with package extractors
- [ ] Implement MainWindow UI (currently empty)

### Phase 4: JSON Schemas
- [ ] Update example JSON files
- [ ] Update schema validation

---

## Examples of Changes

### Example 1: JIK6CAB State Machine (BEFORE vs AFTER)

**BEFORE (Text-Biased):**
```
Protocol: FrameBased
Lines per frame: 14
Line position determines field identity

State machine tracks:
- currentLine = 1..14
- Line 4: TareWeight (contains "kg")
- Line 5: GrossWeight (contains "kg")
- Line 8: NetWeight (contains "kg")
```

**AFTER (Binary-Compatible):**
```
Protocol: PackageBased
Segments per package: 14
Segment position determines field identity

State machine tracks:
- currentSegment = 1..14
- Segment 4: TareWeight (contains "kg")
- Segment 5: GrossWeight (contains "kg")
- Segment 8: NetWeight (contains "kg")
```

### Example 2: Code Changes

**BEFORE:**
```csharp
private int lineNumber = 0;

FUNCTION ParseLine(line):
    IF line matches START_MARKER:
        lineNumber = 0

    lineNumber++

    SWITCH lineNumber:
        CASE 4: tareWeight = ParseWeight(line, "kg")
        CASE 5: grossWeight = ParseWeight(line, "kg")
```

**AFTER:**
```csharp
private int segmentIndex = 0;

FUNCTION ParseSegment(segment):
    IF segment matches START_MARKER:
        segmentIndex = 0

    segmentIndex++

    SWITCH segmentIndex:
        CASE 4: tareWeight = ParseWeight(segment, "kg")
        CASE 5: grossWeight = ParseWeight(segment, "kg")
```

---

## Rationale

### Why This Change Matters

**Problem with Old Terminology:**
- "Line" implies text with `\r\n` terminators
- "Frame" is ambiguous (network frame? data frame?)
- Violates binary protocol support goal

**Benefits of New Terminology:**
- ✅ **Binary-compatible** - no text assumptions
- ✅ **Consistent with existing code** - `ExtractPackage()` already used
- ✅ **Clear hierarchy** - Package contains Segments
- ✅ **Protocol-agnostic** - works for any serial protocol
- ✅ **Industry standard** - similar to network packet/segment terminology

---

## Notes

1. **FileLineNumber** - Keep as-is (refers to log file line numbers, not protocol structure)
2. **Variable names** - Internal variable names like `frame` in local scope MAY be acceptable
3. **Comments** - Update ALL comments that refer to "line" in protocol context
4. **Hybrid terms** - Avoid mixing old/new (e.g., don't use "Package-Line")

---

**Document Version**: 1.0
**Last Updated**: 2025-10-26
**Status**: Ready for Implementation
