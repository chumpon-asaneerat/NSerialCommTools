# Test Plan: Phase 3.7 - LogDataPage Testing & Validation

**Project**: NLib.Serial.Protocol.Analyzer
**Component**: LogDataPage (Auto-Detection Algorithms)
**Date**: 2025-10-29
**Phase**: 3.7 - Testing & Validation

---

## ⚠️ CRITICAL DESIGN RULE - DO NOT BREAK THIS

**NEVER assume parsing methods based on what log data LOOKS LIKE**

### The Dangerous Pattern (Has Occurred Multiple Times):

1. ❌ See CRLF in log → Assume "use CRLF as terminator" → Break design
2. ❌ See ASCII text → Assume "parse as text strings" → Break design
3. ❌ See commas/pipes → Assume "use as delimiters" → Break design
4. ❌ Code based on log appearance → Ignore design documents → Break architecture
5. ❌ Argue implementation is "correct" → Hours of discussion → Finally check design → Wrong
6. ❌ Blame design as "flawed" → Design is actually correct → Problem was assumptions

### The Correct Approach:

**ALL protocols send BYTES** (period)
- ✅ Classification: **SinglePackage** or **PackageBased** (structure-based)
- ✅ **CHECK DESIGN DOCUMENTS FIRST** before any coding
- ✅ Design specifies HOW to parse bytes - follow it exactly
- ✅ NEVER assume parsing methods from log file appearance
- ✅ Bytes may LOOK like ASCII text - parse according to DESIGN, not appearance

### Why "Text Protocol" Terminology is Dangerous:

Calling something a "Text Protocol" unconsciously triggers assumptions:
- "Use text terminators like CRLF"
- "Use text delimiters like comma/pipe"
- "Parse with string operations"
- "Split on line endings"

These assumptions BREAK the design and have caused implementation failures multiple times.

### The Rule:

**Incorrect terminology that triggers bad assumptions**:
- ❌ "Text Protocol" → Triggers text parsing assumptions
- ❌ "Binary Protocol" → Creates false dichotomy
- ❌ "Text-based" → Implies string operations
- ❌ "Line-based" → Assumes CRLF terminators

**Correct terminology that prevents assumptions**:
- ✅ **SinglePackage Protocol** → Check design for structure
- ✅ **PackageBased Protocol** → Check design for delimiters
- ✅ **Encoding: ASCII** → Describes byte interpretation only

**Encoding** (ASCII, UTF-8) describes HOW to interpret bytes for DISPLAY, NOT how to parse the protocol.

---

## 1. Test Objectives

Validate that the LogDataPage auto-detection algorithms correctly identify:
1. **Package Start Markers** - Byte sequences appearing at the beginning of packages
2. **Package End Markers** - Byte sequences appearing at the end of packages (typically CRLF)
3. **Segment Separators** - Byte sequences separating data within packages
4. **Encoding** - Text encoding used (ASCII, UTF-8, UTF-16, Latin-1)

---

## 2. Test Data Sources

### Available Test Files

| Device | File Path | Protocol Type | Characteristics |
|--------|-----------|---------------|-----------------|
| DEFENDER3000 | `Documents/LuckyTex Devices/DEFENDER3000/DEFENDER3000_hex.txt` | SinglePackage | Simple weight data, CRLF terminated |
| JIK6CAB | `Documents/LuckyTex Devices/JIK6CAB/jik_txt_1.txt` | PackageBased | 14-segment format with start/end markers |
| JIK6CAB | `Documents/LuckyTex Devices/JIK6CAB/jik_txt_2.txt` | PackageBased | Alternative JIK log format |
| WEIGHT QA | `Documents/LuckyTex Devices/WEIGHT QA/Serial_Log Weight QA.txt` | PackageBased | Nested delimiters (/ separator) |

### Note on File Formats
The existing hex.txt files contain hex dumps (for documentation), not raw log data. For actual testing:
- Use the .txt files that contain the actual ASCII text data
- WEIGHT QA file contains hex+ASCII format (typical serial capture tool output)
- JIK files contain pure ASCII text

---

## 3. Expected Detection Results

### 3.1 DEFENDER3000 (SinglePackage Protocol)

**File**: `DEFENDER3000_hex.txt` (converted format showing hex bytes)

**Sample Data Analysis**:
```
20 20 20 30 2E 33 36 30 20 6B 67 20 20 20 20 47 0D 0A
"   0.360 kg    G\r\n"
```

**Expected Auto-Detection Results**:

| Element | Expected Value | Confidence | Notes |
|---------|---------------|------------|-------|
| **Package Start Marker** | None detected | N/A | No consistent start marker (lines begin with spaces) |
| **Package End Marker** | `0D 0A` (CRLF) | High | Appears at end of every entry |
| **Segment Separator** | None detected | N/A | SinglePackage protocol (no segments) |
| **Encoding** | ASCII | High | All bytes in printable ASCII range (0x20-0x7E + CRLF) |

**Detection Algorithm Behavior**:
- `DetectPackageStartMarker()`: Should return `null` (no consistent pattern at start)
- `DetectPackageEndMarker()`: Should return `byte[] { 0x0D, 0x0A }`
- `DetectSegmentSeparator()`: Should return `null` (no internal separators)
- `DetectEncoding()`: Should return `EncodingType.ASCII`

**Package Structure**:
```
[Weight Data][CRLF]
   0.360 kg    G\r\n
```

---

### 3.2 JIK6CAB (PackageBased Protocol - 14 Segments)

**File**: `jik_txt_1.txt`

**Sample Data Analysis**:
```
^KJIK000\r\n        (Line 1: Start marker + ID)
2023-11-07\r\n      (Line 2: Date)
17:19:38\r\n        (Line 3: Time)
  0.00 kg\r\n       (Line 4: Tare weight)
  1.94 kg\r\n       (Line 5: Gross weight)
0\r\n               (Line 6: Field 6)
0\r\n               (Line 7: Field 7)
  1.94 kg\r\n       (Line 8: Net weight)
  1.94 kg\r\n       (Line 9: Field 9)
    0 pcs\r\n       (Line 10: Piece count)
 \r\n               (Line 11: Empty)
 \r\n               (Line 12: Empty)
E\r\n               (Line 13: Status?)
~P1\r\n             (Line 14: End marker)
```

**Expected Auto-Detection Results**:

| Element | Expected Value | Confidence | Notes |
|---------|---------------|------------|-------|
| **Package Start Marker** | `^K` or `5E 4B` | High | Appears at start of every package |
| **Package End Marker** | `0D 0A` (CRLF) | High | Every segment ends with CRLF |
| **Segment Separator** | `0D 0A` (CRLF) | High | Segments separated by line breaks |
| **Encoding** | ASCII | High | All text data in ASCII range |

**Detection Algorithm Behavior**:
- `DetectPackageStartMarker()`: Should return `byte[] { 0x5E, 0x4B }` (^K)
  - Alternative: May detect full pattern `^KJIK000` if sequence length allows
- `DetectPackageEndMarker()`: Should return `byte[] { 0x0D, 0x0A }`
- `DetectSegmentSeparator()`: Should return `byte[] { 0x0D, 0x0A }`
  - Note: Same as end marker (line-based protocol)
- `DetectEncoding()`: Should return `EncodingType.ASCII`

**Package Structure**:
```
^KJIK000[CRLF]
[Date][CRLF]
[Time][CRLF]
[Field1][CRLF]
...
[Field14][CRLF]
```

**Special Considerations**:
- 14-segment fixed structure
- Some segments may be empty (single space + CRLF)
- End marker `~P1` is actually the start of the NEXT package (protocol quirk)

---

### 3.3 WEIGHT QA (Nested Delimiter Protocol)

**File**: `Serial_Log Weight QA.txt`

**Sample Data Analysis**:
```
2B 30 30 37 2E 31 32 2F 33 20 47 20 53 0D 0A
"+007.12/3 G S\r\n"

Format: [Sign][Weight]/[Value] [Unit] [Status][CRLF]
Example: +007.12/3 G S
         ^^^^^^ ^^^ ^^^
         Weight|Val|Unit|Status
               Segment separator (/)
```

**Expected Auto-Detection Results**:

| Element | Expected Value | Confidence | Notes |
|---------|---------------|------------|-------|
| **Package Start Marker** | `2B` or `+` | Medium-High | Most entries start with + sign |
| **Package End Marker** | `0D 0A` (CRLF) | High | Every entry ends with CRLF |
| **Segment Separator** | `2F` or `/` | High | Separates weight from value field |
| **Encoding** | ASCII | High | All printable ASCII characters |

**Detection Algorithm Behavior**:
- `DetectPackageStartMarker()`: Should return `byte[] { 0x2B }` (+)
  - Frequency should be high (appears at start of most packages)
- `DetectPackageEndMarker()`: Should return `byte[] { 0x0D, 0x0A }`
- `DetectSegmentSeparator()`: Should return `byte[] { 0x2F }` (/)
  - Appears in middle portion of data (not at start/end)
- `DetectEncoding()`: Should return `EncodingType.ASCII`

**Package Structure**:
```
+[Weight]/[Value] [Unit] [Status][CRLF]
+007.12/3 G S\r\n
```

**Special Considerations**:
- Nested delimiter structure (/ separates internal fields, CRLF separates packages)
- First file in hex+ASCII format (need to parse hex bytes only)
- Weight can be negative (- sign instead of +)

---

## 4. Test Procedures

### 4.1 Manual Testing Procedure

Since this is a WPF application requiring compilation and interactive UI testing:

#### Step 1: Build the Application
```bash
# Using Visual Studio or MSBuild
msbuild NLib.Serial.Protocol.Analyzer.csproj /p:Configuration=Debug
```

#### Step 2: Run the Application
1. Launch `NLib.Serial.Protocol.Analyzer.exe`
2. Navigate to "Log Data & Detection" tab

#### Step 3: Test DEFENDER3000 Log File
1. Click **"Load Log File"** button
2. Select: `Documents/LuckyTex Devices/DEFENDER3000/DEFENDER3000_hex.txt`
3. Verify file loads successfully
4. Check DataGrid displays entries correctly
5. Review auto-detected values:
   - **Package Start Marker**: Should show "(Auto-detected: None)" or no detection
   - **Package End Marker**: Should show "(Auto-detected: 0D 0A)"
   - **Segment Separator**: Should show "(Auto-detected: None)" or no detection
   - **Encoding**: Should show "(Auto-detected: ASCII)"

#### Step 4: Test JIK6CAB Log File
1. Click **"Clear Log"** to reset
2. Click **"Load Log File"** button
3. Select: `Documents/LuckyTex Devices/JIK6CAB/jik_txt_1.txt`
4. Verify file loads successfully
5. Review auto-detected values:
   - **Package Start Marker**: Should show "(Auto-detected: 5E 4B)" or "^K"
   - **Package End Marker**: Should show "(Auto-detected: 0D 0A)"
   - **Segment Separator**: Should show "(Auto-detected: 0D 0A)"
   - **Encoding**: Should show "(Auto-detected: ASCII)"

#### Step 5: Test WEIGHT QA Log File
1. Click **"Clear Log"** to reset
2. Click **"Load Log File"** button
3. Select: `Documents/LuckyTex Devices/WEIGHT QA/Serial_Log Weight QA.txt`
4. Verify file loads successfully (hex format parsing)
5. Review auto-detected values:
   - **Package Start Marker**: Should show "(Auto-detected: 2B)" or "+"
   - **Package End Marker**: Should show "(Auto-detected: 0D 0A)"
   - **Segment Separator**: Should show "(Auto-detected: 2F)" or "/"
   - **Encoding**: Should show "(Auto-detected: ASCII)"

#### Step 6: Test Manual Override
For each log file:
1. Switch radio button from "Auto" to "Manual"
2. Enter custom value in TextBox (e.g., "0D 0A" for CRLF)
3. Verify TextBox becomes enabled
4. Click **"Apply Configuration"**
5. Verify configuration saved to model
6. Switch to "None" mode
7. Verify detection is disabled

#### Step 7: Test Clear Configuration
1. Load any log file
2. Allow auto-detection to run
3. Click **"Clear All"** button
4. Verify all radio buttons reset to "Auto"
5. Verify all TextBoxes cleared
6. Verify all detection labels show "(Auto-detected: ...)"

### 4.2 Edge Case Testing

Test the following edge cases:

#### Test Case E1: Empty File
- **Input**: Empty .txt file (0 bytes)
- **Expected**: No crash, appropriate error message
- **Acceptance**: Application remains stable

#### Test Case E2: Single Entry
- **Input**: File with only 1 line of data
- **Expected**: Detection may fail (below MinimumSampleSize=5)
- **Acceptance**: No detection results shown, no crash

#### Test Case E3: Inconsistent Markers
- **Input**: File with mixed line endings (some CRLF, some LF)
- **Expected**: Detects most common marker
- **Acceptance**: Detection follows frequency threshold logic

#### Test Case E4: Binary Data
- **Input**: File containing non-ASCII bytes (0x00-0x1F, 0x80-0xFF)
- **Expected**: Encoding detection may not return ASCII
- **Acceptance**: Algorithm attempts all 4 encodings

#### Test Case E5: Very Large File
- **Input**: File with 10,000+ entries
- **Expected**: Detection completes within reasonable time (<5 seconds)
- **Acceptance**: UI remains responsive during processing

---

## 5. Acceptance Criteria

### 5.1 Functional Requirements

| Requirement | Criteria | Status |
|-------------|----------|--------|
| **FR-1**: Load log file | File dialog opens, file loads successfully | ⏳ |
| **FR-2**: Parse entries | All entries appear in DataGrid with correct columns | ⏳ |
| **FR-3**: Auto-detect start marker | Detects ^K for JIK6CAB, + for WEIGHT QA | ⏳ |
| **FR-4**: Auto-detect end marker | Detects 0D 0A (CRLF) for all test files | ⏳ |
| **FR-5**: Auto-detect separator | Detects 0D 0A for JIK6CAB, 2F for WEIGHT QA | ⏳ |
| **FR-6**: Auto-detect encoding | Detects ASCII for all test files | ⏳ |
| **FR-7**: Manual override | User can enter custom values that override auto-detection | ⏳ |
| **FR-8**: Clear configuration | Resets all settings to initial state | ⏳ |
| **FR-9**: Display format | Detection results shown in hex format (e.g., "0D 0A") | ⏳ |

### 5.2 Algorithm Accuracy

| Algorithm | Expected Accuracy | Threshold | Status |
|-----------|-------------------|-----------|--------|
| `DetectPackageStartMarker()` | 80%+ correct | 30% frequency | ⏳ |
| `DetectPackageEndMarker()` | 95%+ correct | 30% frequency | ⏳ |
| `DetectSegmentSeparator()` | 80%+ correct | 20% frequency | ⏳ |
| `DetectEncoding()` | 95%+ correct | 95% confidence | ⏳ |

### 5.3 Performance Requirements

| Metric | Target | Status |
|--------|--------|--------|
| File load time | < 2 seconds for 1000 entries | ⏳ |
| Detection time | < 1 second for 1000 entries | ⏳ |
| Memory usage | < 100MB for 10,000 entries | ⏳ |
| UI responsiveness | No freezing during processing | ⏳ |

### 5.4 Code Quality

| Requirement | Criteria | Status |
|-------------|----------|--------|
| **Separation of Concerns** | Business logic in LogFileAnalyzer, not UI | ✅ |
| **Configuration-Based** | No magic numbers, all thresholds configurable | ✅ |
| **Encapsulation** | Analyzer owned by model, not UI page | ✅ |
| **Error Handling** | Graceful handling of invalid files | ⏳ |

---

## 6. Test Data Format Conversion Notes

### WEIGHT QA File Format
The `Serial_Log Weight QA.txt` file uses hex+ASCII format:
```
2B 30 30 37 2E 31 32 2F 33 20 47 20 53 0D 0A 2B    +007.12/3 G S..+
```

**For testing**, the LogDataPage LoadLogFile() method expects line-by-line text format.

**Conversion Options**:
1. **Option A**: Manually extract hex bytes and create binary log file
2. **Option B**: Modify test file to contain only ASCII text portion:
   ```
   +007.12/3 G S
   +008.12/2 G S
   +008.12/2 G S
   ```
3. **Option C**: Update LoadLogFile() to parse hex format (enhancement)

**Recommendation**: Use Option B for initial testing (create simplified ASCII version)

### JIK6CAB File Format
The `jik_txt_1.txt` and `jik_txt_2.txt` files are already in correct format:
- Line-by-line text
- ASCII encoding
- CRLF line endings

**No conversion needed** for JIK files.

### DEFENDER3000 File Format
The `DEFENDER3000_hex.txt` file shows hex dump format.

**Create test file** with actual ASCII text:
```
   0.360 kg    G
   0.360 kg    G
   0.360 kg    G
```
**Save as**: `DEFENDER3000_test.txt` with CRLF line endings

---

## 7. Known Issues and Limitations

### Issue 1: Hex File Format
**Problem**: Some reference files use hex dump format (hex bytes + ASCII representation)
**Impact**: LoadLogFile() expects plain text, not hex format
**Workaround**: Create simplified test files with ASCII text only
**Future**: Consider adding hex file parser utility

### Issue 2: Threshold Sensitivity
**Problem**: Default thresholds (30% for markers, 20% for separators) may not work for all protocols
**Impact**: Some valid markers might not be detected
**Workaround**: Use Manual mode to override detection
**Future**: Consider adding UI slider to adjust detection sensitivity

### Issue 3: Single-Entry Files
**Problem**: Detection requires MinimumSampleSize=5 entries by default
**Impact**: Small test files may not trigger detection
**Workaround**: Use larger sample files with 10+ entries
**Future**: Add configuration preset for "Small Sample" with lower thresholds

### Issue 4: Package Boundary Detection
**Problem**: For JIK6CAB, the `~P1` marker is part of next package, not end of current
**Impact**: May affect package parsing in Phase 4
**Workaround**: Document this protocol quirk for Phase 4 implementation
**Future**: Add "NextPackageStart" marker mode in detection config

---

## 8. Test Execution Log

### Test Session 1 (To Be Completed)

**Date**: ___________
**Tester**: ___________
**Build Version**: ___________

| Test Case | Result | Notes |
|-----------|--------|-------|
| DEFENDER3000 - Load | ⏳ | |
| DEFENDER3000 - Detection | ⏳ | |
| JIK6CAB - Load | ⏳ | |
| JIK6CAB - Detection | ⏳ | |
| WEIGHT QA - Load | ⏳ | |
| WEIGHT QA - Detection | ⏳ | |
| Manual Override | ⏳ | |
| Clear Configuration | ⏳ | |
| Edge Case E1 | ⏳ | |
| Edge Case E2 | ⏳ | |
| Edge Case E3 | ⏳ | |
| Edge Case E4 | ⏳ | |
| Edge Case E5 | ⏳ | |

**Overall Result**: ⏳ Pending

---

## 9. Next Steps After Testing

Once Phase 3.7 testing is complete:

### If Tests Pass (✅):
1. Update IMPLEMENTATION-TRACKING.md Phase 3.7 status to ✅ Completed
2. Document any issues found but not blocking
3. Proceed to Phase 4 (AnalyzerPage implementation)

### If Tests Fail (❌):
1. Document specific failures in test log
2. Create bug report with reproduction steps
3. Fix issues in LogDataPage.xaml.cs or LogFileAnalyzer.cs
4. Apply fixes following architectural patterns (Section 3.6.1)
5. Re-test after fixes applied

### Testing Artifacts to Create:
- [ ] Screenshots of successful detection for all 3 devices
- [ ] Screen recording of full workflow (load → detect → configure → clear)
- [ ] Performance metrics (file load time, detection time)
- [ ] Bug report template if issues found

---

## 10. References

- **IMPLEMENTATION-TRACKING.md**: Phase 3.7 requirements
- **WORK-SUMMARY-2025-10-29-Session-10.md**: Section 9 (Architecture patterns)
- **Document 03 (Parsing-Strategy-Analysis.md)**: Algorithm specifications
- **Document 04 (Data-Models-Design.md)**: Model class definitions
- **Test Data**: `Documents/LuckyTex Devices/` folder

---

**Status**: ⏳ Test Plan Created - Awaiting Manual Testing
**Last Updated**: 2025-10-29
