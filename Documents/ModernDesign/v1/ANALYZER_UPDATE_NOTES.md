# Protocol Analyzer - Update Notes

## Issue with JIK6CAB Analysis

### Problem
When analyzing `jik_hex_1.txt` (JIK6CAB device), the analyzer was not generating the correct protocol definition because:

1. **JIK6CAB is a multi-line package protocol** (14 lines per package)
2. **The original analyzer was designed for simple CSV protocols** (single-line messages)
3. Each CRLF in JIK6CAB was being treated as a separate "message" instead of recognizing the 14-line package structure

### Example JIK6CAB Data Structure
```
^KJIK000          ← Line 1: Start marker
2023-11-07        ← Line 2: Date
17:19:26          ← Line 3: Time
  0.00 kg         ← Line 4: Tare weight
  1.94 kg         ← Line 5: Gross weight
0                 ← Line 6: Reserved
0                 ← Line 7: Reserved
  1.94 kg         ← Line 8: Net weight
  1.94 kg         ← Line 9: Display weight
    0 pcs         ← Line 10: Piece count
                  ← Line 11: Empty
                  ← Line 12: Empty
E                 ← Line 13: Status
~P1               ← Line 14: End marker
```

### Solution Implemented

**Added Package Detection Capability:**

1. **New File: `PackageDetector.cs`**
   - Detects multi-line package protocols
   - Identifies start/end markers
   - Calculates package size
   - Returns confidence score

2. **Updated: `PatternAnalyzer.cs`**
   - Now checks for packages FIRST before analyzing delimiters
   - If package detected:
     - Protocol Type = "multi-line-package"
     - Recommended Strategy = "state-machine"
     - Returns package size, start marker, end marker

3. **Updated: `AnalysisResult.cs`**
   - Added properties:
     - `PackageSize` - Number of lines per package
     - `StartMarker` - Package start marker text
     - `EndMarker` - Package end marker text

### How It Works Now

**For Simple Protocols (TScaleNHB, TScaleQHW, etc.):**
```
Flow: Load → Detect Format → Parse → Analyze Delimiters → Analyze Fields → Generate JSON
Result: CSV-style protocol definition with split parsing
```

**For Package Protocols (JIK6CAB, TFO1, etc.):**
```
Flow: Load → Detect Format → Parse → Detect Packages → Return Package Info
Result: Multi-line package protocol definition with state-machine recommendation
```

### Expected Behavior After Fix

When you analyze `jik_hex_1.txt`, you should see:

**Analysis Results:**
- Protocol Type: `multi-line-package`
- Recommended Strategy: `state-machine`
- Package Size: `14 lines`
- Start Marker: `^KJIK000`
- End Marker: `~P1`
- Confidence: `95%`

**JSON Output:**
```json
{
  "DeviceInfo": {
    "Name": "jik_hex_1",
    "Category": "unknown",
    "Description": "Auto-generated from jik_hex_1.txt"
  },
  "Protocol": {
    "Type": "multi-line-package",
    "Format": "csv",
    "Encoding": "ASCII",
    "Terminator": "\\r\\n"
  },
  "Parsing": {
    "Strategy": "state-machine"
  }
}
```

## Files Modified

1. ✅ `Analyzers/PackageDetector.cs` - NEW
2. ✅ `Analyzers/PatternAnalyzer.cs` - UPDATED
3. ✅ `Models/AnalysisResult.cs` - UPDATED
4. ⏳ `MainWindow.xaml.cs` - NEEDS UPDATE (to display package info in UI)

## Next Steps

1. Rebuild the project in Visual Studio
2. Test with `jik_hex_1.txt` again
3. Verify package detection works correctly
4. Compare with simple CSV files (TScaleNHB.txt) to ensure they still work

## Notes

- The analyzer now intelligently detects **two types of protocols**:
  1. **Simple CSV** (like weight scales) → Split parsing
  2. **Multi-line packages** (like JIK6CAB) → State machine parsing

- This matches the original design document's vision of handling different complexity levels
- Complex devices still need custom C# implementation (as per hybrid 80/20 model)
- The analyzer helps identify the protocol structure but won't generate complete state machine code
