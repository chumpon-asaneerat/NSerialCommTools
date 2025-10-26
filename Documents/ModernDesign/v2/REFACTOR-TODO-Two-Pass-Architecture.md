# REFACTOR TODO: Two-Pass Architecture

## Status: IN PROGRESS
**Date Started**: 2025-10-24
**Priority**: CRITICAL - Architectural Fix
**Reason**: Current architecture splits data before knowing terminators (chicken-egg problem)

---

## Problem Identified

### Current (WRONG) Flow:
```
1. MessageExtractor.ExtractMessages(rawBytes)
   └─> Splits by GUESSED terminator (\r\n, \n, \r) ❌

2. PatternAnalyzer.Analyze(logData)
   └─> TerminatorDetector.Detect(logData)
   └─> Too late - data already split! ❌

Issue: We're splitting BEFORE we know what the terminator is!
```

### Root Cause:
- **Circular Dependency**: MessageExtractor needs terminator → but TerminatorDetector needs split data
- **Guessing**: Code assumes common terminators without analyzing
- **Cannot Handle**: Binary protocols, custom terminators, multi-level terminators

---

## Correct Architecture: Two-Pass System

### **Pass 1: Detection (Analyze Raw Bytes)**
```
Input: byte[] rawBytes (entire file, unsplit)

Process:
1. Detect encoding (UTF-8, ASCII, etc.)
2. Detect ALL terminators in one analysis:
   - Frame terminator (message/frame boundaries)
   - Segment terminator (segment boundaries within frame)
   - Field delimiter (field boundaries within segment)
3. Detect frame markers (^, ~, <START>, etc.)
4. Calculate confidence scores

Output: DetectionResult {
  Encoding: UTF-8
  FrameTerminator: [0x0D, 0x0A, 0x0D, 0x0A]    // Double CRLF
  SegmentTerminator: [0x0D, 0x0A]               // Single CRLF
  FieldDelimiter: [0x20]                         // Space
  FrameMarkers: {...}
  Confidence scores
}
```

### **Pass 2: Extraction (Split Using Detected Patterns)**
```
Input: byte[] rawBytes + DetectionResult

Process:
1. Split by frame terminator → byte[][] frames
2. (Optional) Split by segment terminator → byte[][][] segments
3. (Optional) Split by field delimiter → byte[][][][] fields

Output: LogData with properly split messages
```

### **Pass 3: Analysis (Analyze Structured Data)**
```
Input: LogData + DetectionResult

Process:
1. Field type inference
2. Relationship detection
3. JSON generation

Output: ProtocolDefinition (JSON)
```

---

## Implementation Plan

### Phase 1: Create New Models ✅ TODO

#### 1.1: Create DetectionResult Model
**File**: `Models\DetectionResult.cs` (NEW)
```csharp
public class DetectionResult
{
    // Encoding
    public Encoding DetectedEncoding { get; set; }
    public string EncodingName { get; set; }
    public double EncodingConfidence { get; set; }

    // Terminator Hierarchy (Binary-First Thinking)
    public TerminatorInfo FrameTerminator { get; set; }    // Frame boundaries (complete messages)
    public TerminatorInfo SegmentTerminator { get; set; }  // Segment boundaries (chunks within frame)
    public TerminatorInfo FieldDelimiter { get; set; }     // Field boundaries (data within segment)

    // Frame Markers
    public FrameMarkerInfo StartMarker { get; set; }
    public FrameMarkerInfo EndMarker { get; set; }

    // Overall Confidence
    public double OverallConfidence { get; set; }
}
```

#### 1.2: Update TerminatorInfo Model
**File**: `Models\TerminatorInfo.cs` (UPDATE)
- Add `TerminatorType` enum: Frame, Segment, Field
- Add `Level` property to indicate hierarchy (1=Frame, 2=Segment, 3=Field)

---

### Phase 2: Create ProtocolDetector (Pass 1) ✅ TODO

#### 2.1: Create ProtocolDetector Class
**File**: `Analyzers\ProtocolDetector.cs` (NEW)
```csharp
public class ProtocolDetector
{
    private readonly EncodingDetector _encodingDetector;
    private readonly TerminatorDetector _terminatorDetector;
    private readonly MarkerDetector _markerDetector;

    public DetectionResult DetectProtocolStructure(byte[] rawBytes)
    {
        // Step 1: Detect encoding
        var encoding = _encodingDetector.DetectEncoding(rawBytes);

        // Step 2: Detect ALL terminators (hierarchy)
        var terminators = _terminatorDetector.DetectTerminatorHierarchy(rawBytes, encoding);

        // Step 3: Detect frame markers
        var markers = _markerDetector.DetectMarkers(rawBytes, terminators);

        // Step 4: Calculate overall confidence
        double confidence = CalculateOverallConfidence(...);

        return new DetectionResult { ... };
    }
}
```

---

### Phase 3: Refactor TerminatorDetector ✅ TODO

#### 3.1: Update TerminatorDetector
**File**: `Analyzers\TerminatorDetector.cs` (MAJOR REFACTOR)

**NEW Method**: `DetectTerminatorHierarchy`
```csharp
public TerminatorHierarchy DetectTerminatorHierarchy(byte[] rawBytes, Encoding encoding)
{
    // Analyze raw bytes to find ALL terminator types

    // Step 1: Find all byte sequences that repeat
    var candidates = FindRepeatingSequences(rawBytes);

    // Step 2: Analyze patterns
    // - Short sequences (1-2 bytes) → likely segment terminators (\r\n, \n)
    // - Medium sequences (2-4 bytes) → likely frame terminators (\r\n\r\n)
    // - Single bytes → likely field delimiters (space, tab, comma)

    // Step 3: Statistical analysis
    // - Count occurrences
    // - Check positions (end of lines? end of frames?)
    // - Calculate confidence

    // Step 4: Determine hierarchy
    // Which terminator separates frames (complete messages)?
    // Which separates segments (chunks within frame)?
    // Which separates fields (data within segment)?

    return new TerminatorHierarchy {
        FrameTerminator = ...,
        SegmentTerminator = ...,
        FieldDelimiter = ...
    };
}
```

**Key Algorithms**:
1. **FindRepeatingSequences**: Scan for byte patterns that repeat
2. **AnalyzePositions**: Where do sequences appear? (end of data blocks?)
3. **DetermineHierarchy**: Which is frame-level vs line-level vs field-level?

---

### Phase 4: Refactor MessageExtractor (Pass 2) ✅ TODO

#### 4.1: Update MessageExtractor
**File**: `Parsers\MessageExtractor.cs` (MAJOR REFACTOR)

**NEW Signature**:
```csharp
// OLD (WRONG):
public LogData ExtractMessages(byte[] rawBytes, TerminatorInfo terminator = null)

// NEW (CORRECT):
public LogData ExtractMessages(byte[] rawBytes, DetectionResult detection)
{
    // Now we KNOW all terminators - no guessing!

    // Use detected frame terminator
    byte[][] frames = ByteArraySplitter.Split(
        rawBytes,
        detection.FrameTerminator.Bytes
    );

    return new LogData { Messages = frames.ToList() };
}
```

---

### Phase 5: Update PatternAnalyzer (Pass 3) ✅ TODO

#### 5.1: Refactor PatternAnalyzer
**File**: `Analyzers\PatternAnalyzer.cs` (MAJOR REFACTOR)

**NEW Flow**:
```csharp
// OLD:
public AnalysisResult Analyze(LogData logData)
{
    // Detect encoding ❌ (should be done in Pass 1)
    // Detect terminator ❌ (should be done in Pass 1)
    // Detect delimiter ❌ (should be done in Pass 1)
    // Analyze fields
}

// NEW:
public AnalysisResult Analyze(LogData logData, DetectionResult detection)
{
    // Use pre-detected patterns (from Pass 1)

    var result = new AnalysisResult();

    // Copy detection results
    result.DetectedEncoding = detection.DetectedEncoding;
    result.Terminator = detection.SegmentTerminator;  // Already detected!

    // Analyze fields using detected patterns
    result.Fields = _fieldAnalyzer.Analyze(
        logData,
        detection.FieldDelimiter,
        detection.DetectedEncoding,
        detection.SegmentTerminator  // Use detected, not guessed!
    );

    // Detect relationships
    result.Relationships = _relationshipDetector.DetectRelationships(result.Fields);

    return result;
}
```

---

### Phase 6: Update MainWindow (Pipeline Integration) ✅ TODO

#### 6.1: Refactor Analysis Pipeline
**File**: `MainWindow.xaml.cs` (UPDATE)

**NEW Flow**:
```csharp
// OLD (WRONG):
byte[] rawBytes = _hexLogParser.ParseLogFile(filePath);
LogData logData = _messageExtractor.ExtractMessages(rawBytes);  // ❌ Guesses terminator
AnalysisResult result = _patternAnalyzer.Analyze(logData);      // ❌ Too late

// NEW (CORRECT):
// Step 1: Load file
byte[] rawBytes = _hexLogParser.ParseLogFile(filePath);

// Step 2: PASS 1 - Detect all patterns (encoding, terminators, markers)
DetectionResult detection = _protocolDetector.DetectProtocolStructure(rawBytes);

// Step 3: PASS 2 - Extract messages using detected patterns
LogData logData = _messageExtractor.ExtractMessages(rawBytes, detection);

// Step 4: PASS 3 - Analyze fields using detected patterns
AnalysisResult result = _patternAnalyzer.Analyze(logData, detection);

// Step 5: Generate JSON
ProtocolDefinition definition = _generator.Generate(result, deviceName, logData, detection);
```

---

## Terminator Detection Algorithm (Key Logic)

### Detect Frame Terminator vs Segment Terminator vs Field Delimiter

```
Algorithm: TerminatorHierarchy Detection

Input: byte[] rawBytes, Encoding encoding

Step 1: Find Candidate Sequences
  - Scan for repeating byte patterns (1-4 bytes)
  - Common candidates:
    * 0x0D 0x0A (\r\n)
    * 0x0A (\n)
    * 0x0D (\r)
    * 0x0D 0x0A 0x0D 0x0A (double CRLF)
    * 0x20 (space)
    * 0x09 (tab)
    * 0x2C (comma)
    * 0x00 (null)
    * 0x03 (ETX)

Step 2: Analyze Occurrence Patterns
  For each candidate:
    - Count total occurrences
    - Analyze positions (where in file?)
    - Check spacing (evenly distributed? clustered?)
    - Check what follows (more data? end of file?)

Step 3: Determine Hierarchy Level

  Field Delimiter Characteristics:
    - Single byte (usually)
    - High frequency (appears often)
    - Evenly spaced within lines
    - Examples: space (0x20), comma (0x2C), tab (0x09)

  Segment Terminator Characteristics:
    - 1-2 bytes
    - Medium frequency
    - Creates regular structure within frames
    - Examples: \r\n (0x0D 0x0A), \n (0x0A)

  Frame Terminator Characteristics:
    - 2-4 bytes (often repeated segment terminator)
    - Low frequency (fewer than segments)
    - Separates complete messages/frames
    - Examples: \r\n\r\n (double CRLF), frame markers, ETX

Step 4: Confidence Scoring
  For each level:
    - Consistency check (same terminator used throughout?)
    - Frequency analysis (expected count vs actual?)
    - Pattern validation (creates valid structure?)

  Return confidence 0.0-1.0

Output: TerminatorHierarchy {
  FrameTerminator: { Bytes, Confidence }
  SegmentTerminator: { Bytes, Confidence }
  FieldDelimiter: { Bytes, Confidence }
}
```

### Example: JIK6CAB Protocol

```
Raw data structure:
[data][0x0D][0x0A]  ← Segment 1
[data][0x0D][0x0A]  ← Segment 2
...
[data][0x0D][0x0A]  ← Segment 10
[0x0D][0x0A]         ← Empty segment (frame terminator!)
[data][0x0D][0x0A]  ← Segment 1 of next frame
...

Detection Results:
- 0x0D 0x0A appears 55 times (50 segments + 5 frames)
- 0x0D 0x0A 0x0D 0x0A appears 5 times (frame separators)
- 0x20 (space) appears 100 times (between value and unit)

Hierarchy:
- FrameTerminator: [0x0D, 0x0A, 0x0D, 0x0A] (double CRLF) - Confidence 0.95
- SegmentTerminator: [0x0D, 0x0A] (single CRLF) - Confidence 0.95
- FieldDelimiter: [0x20] (space) - Confidence 0.80
```

---

## Testing Strategy

### Test Cases:

1. **JIK6CAB (Multi-segment frames, double CRLF separator)**
   - Expected: Detect double CRLF as frame terminator
   - Expected: Detect single CRLF as segment terminator
   - Expected: Detect space as field delimiter

2. **CSV Protocol (Single-segment frames, comma-delimited)**
   - Expected: Detect CRLF as both frame and segment terminator
   - Expected: Detect comma as field delimiter

3. **Binary Protocol (Binary terminators)**
   - Expected: Detect 0x03 (ETX) as frame terminator
   - Expected: Detect 0x1F (Unit Separator) as field delimiter
   - Expected: No segment terminator (binary data)

4. **Mixed Protocol (Variable terminators)**
   - Expected: Handle both \r\n and \n
   - Expected: Choose most common as primary

---

## Implementation Checklist

### Phase 1: Models ⬜
- [ ] Create `DetectionResult.cs`
- [ ] Update `TerminatorInfo.cs` (add Type, Level)
- [ ] Create `TerminatorHierarchy.cs`

### Phase 2: Detection ⬜
- [ ] Create `ProtocolDetector.cs`
- [ ] Implement `DetectProtocolStructure()`

### Phase 3: Terminator Detection ⬜
- [ ] Refactor `TerminatorDetector.cs`
- [ ] Implement `DetectTerminatorHierarchy()`
- [ ] Implement `FindRepeatingSequences()`
- [ ] Implement `DetermineHierarchy()`

### Phase 4: Extraction ⬜
- [ ] Refactor `MessageExtractor.cs`
- [ ] Update signature to accept `DetectionResult`
- [ ] Remove guessing logic

### Phase 5: Analysis ⬜
- [ ] Refactor `PatternAnalyzer.cs`
- [ ] Update to use pre-detected patterns
- [ ] Remove redundant detection

### Phase 6: Integration ⬜
- [ ] Update `MainWindow.xaml.cs`
- [ ] Implement two-pass pipeline
- [ ] Add error handling

### Phase 7: Testing ⬜
- [ ] Test with JIK6CAB data
- [ ] Test with CSV data
- [ ] Test with binary data
- [ ] Verify JSON output correctness

---

## Expected Benefits

✅ **Correctness**: No more guessing terminators
✅ **Flexibility**: Handles ANY terminator type
✅ **Binary Support**: Works with binary protocols
✅ **Clarity**: Clear separation of concerns
✅ **Testability**: Each phase can be tested independently
✅ **Maintainability**: Logical flow is clear

---

## Risks & Mitigation

### Risk 1: Complex Detection Algorithm
- **Mitigation**: Start with simple heuristics, add complexity incrementally
- **Fallback**: If detection fails, use common terminators with low confidence

### Risk 2: Performance
- **Mitigation**: Detection pass analyzes raw bytes only once
- **Optimization**: Cache frequently accessed patterns

### Risk 3: Edge Cases
- **Mitigation**: Comprehensive test suite with various protocol types
- **Logging**: Add detailed logging for detection decisions

---

## Notes for Next Session

1. **Current State**: Architecture designed, not yet implemented
2. **Next Step**: Start with Phase 1 (create models)
3. **Priority**: Focus on TerminatorDetector refactor (most critical)
4. **Testing**: Have JIK6CAB data ready for testing

---

**Document Version**: 1.0
**Last Updated**: 2025-10-24
**Status**: Planning Complete - Ready to Implement
