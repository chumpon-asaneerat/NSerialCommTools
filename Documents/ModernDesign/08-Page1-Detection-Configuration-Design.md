# Page 1: LogDataPage - Auto-Detect with Manual Override Design

**Document:** Page 1 redesign with protocol detection configuration
**Version:** 1.0
**Date:** 2025-10-26
**Purpose:** Allow users to configure protocol detection settings with auto-detect assistance

---

## Overview

**New Workflow:**
1. User selects log file
2. Clicks **"Load & Auto-Detect"**
3. System parses file AND runs quick detection
4. **Detection results auto-fill editable controls**
5. User can:
   - ✅ Accept auto-detected values (do nothing)
   - ✏️ Edit specific values (e.g., change delimiter)
   - 🔄 Clear and manually enter all
   - 💾 Save settings for reuse
   - 📂 Import settings from definition file
6. Click **"Next: Analyze"** → Proceeds to Page 2 with configured settings

**Key Principle:** Auto-detect first, then let user refine.

---

## UI Layout (Top to Bottom)

```
┌──────────────────────────────────────────────────────────────────┐
│ 📁 Log File Selection                                            │
│ ┌─────────────────────────────────┐ [Browse...]                 │
│ │ C:\Logs\capture.txt             │                              │
│ └─────────────────────────────────┘                              │
│ ○ Auto-detect ○ HEX+Text ○ HEX Only ○ Text                      │
│ [📂 Load & Auto-Detect] [💾 Import Settings...] [Clear]         │
├──────────────────────────────────────────────────────────────────┤
│ 📊 File Statistics                                               │
│ ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐                            │
│ │ 1247 │ │18942 │ │  15  │ │42 KB │                            │
│ │Entry │ │Bytes │ │Avg   │ │Size  │                            │
│ └──────┘ └──────┘ └──────┘ └──────┘                            │
├──────────────────────────────────────────────────────────────────┤
│ ⚙️ Protocol Detection Configuration                              │
│ ┌────────────────────────────────────────────────────────────┐  │
│ │ Status: ✅ Auto-detection complete (Overall: 95.3%)        │  │
│ │ ────────────────────────────────────────────────────────── │  │
│ │                                                            │  │
│ │ 🔚 Package Terminator: (Required)                         │  │
│ │ ○ Auto (98.5% ✅)  ○ Manual                               │  │
│ │ Bytes: [0x0D] [0x0A] [+ Add] [Clear]                      │  │
│ │ Preview: \r\n (CRLF)                                       │  │
│ │ Occurrences: 1,247                                         │  │
│ │                                                            │  │
│ │ ────────────────────────────────────────────────────────── │  │
│ │                                                            │  │
│ │ ✂️ Field Delimiter: (Optional)                            │  │
│ │ ○ Auto (95.2% ✅)  ○ Manual  ☐ None                      │  │
│ │ Bytes: [0x2C] [+ Add] [Clear]                             │  │
│ │ Preview: , (comma)                                         │  │
│ │ Occurrences: 3,741                                         │  │
│ │                                                            │  │
│ │ ────────────────────────────────────────────────────────── │  │
│ │                                                            │  │
│ │ 🏁 Start Marker: (Optional)                               │  │
│ │ ○ Auto (Not detected)  ○ Manual  ☑ None                  │  │
│ │ Bytes: [0x__] [+ Add] [Clear]                             │  │
│ │ Preview: (empty)                                           │  │
│ │                                                            │  │
│ │ ────────────────────────────────────────────────────────── │  │
│ │                                                            │  │
│ │ 🏁 End Marker: (Optional)                                 │  │
│ │ ○ Auto (Not detected)  ○ Manual  ☑ None                  │  │
│ │ Bytes: [0x__] [+ Add] [Clear]                             │  │
│ │ Preview: (empty)                                           │  │
│ │                                                            │  │
│ │ ────────────────────────────────────────────────────────── │  │
│ │                                                            │  │
│ │ 📝 Encoding: (Auto-detected)                              │  │
│ │ ○ Auto (ASCII ✅)  ○ Manual: [ASCII ▼]                   │  │
│ │                                                            │  │
│ │ ────────────────────────────────────────────────────────── │  │
│ │                                                            │  │
│ │ Quick Presets:                                             │  │
│ │ [CRLF \r\n] [LF \n] [CR \r] [Comma ,] [Space] [Tab]       │  │
│ │                                                            │  │
│ │ Actions:                                                   │  │
│ │ [💾 Save Settings...] [📂 Import Settings...] [🔄 Reset] │  │
│ └────────────────────────────────────────────────────────────┘  │
├──────────────────────────────────────────────────────────────────┤
│ 🔍 Hex Preview (First 50 entries)                               │
│ ┌────────────────────────────────────────────────────────────┐  │
│ │ Line 1: 53 54 2C 47 53 20 20 20 32 30 2E 37 67 0D 0A      │  │
│ │ Line 2: 55 53 2C 47 53 20 20 20 32 31 2E 31 67 0D 0A      │  │
│ └────────────────────────────────────────────────────────────┘  │
├──────────────────────────────────────────────────────────────────┤
│ 📝 Text Preview (ASCII)                                          │
│ ┌────────────────────────────────────────────────────────────┐  │
│ │ ST,GS    20.7g                                             │  │
│ │ US,GS    21.1g                                             │  │
│ └────────────────────────────────────────────────────────────┘  │
├──────────────────────────────────────────────────────────────────┤
│                                [Clear] [▶ Next: Analyze (Page 2)]│
└──────────────────────────────────────────────────────────────────┘
```

---

## Protocol Detection Configuration Panel - Detailed Design

### Section 1: Package Terminator 🔚

**Required:** Yes (cannot be None)

**Controls:**

| Control | Type | Purpose | Behavior |
|---------|------|---------|----------|
| Mode Radio | Radio buttons | Auto/Manual selection | • Auto: Lock fields, show detected<br>• Manual: Unlock fields for editing |
| Confidence | Label | Show detection quality | • Green ✅ if ≥80%<br>• Yellow ⚠️ if 60-79%<br>• Red ❌ if <60% |
| Bytes | Byte editors | Hex byte sequence | • Each byte: TextBox with format "0xHH"<br>• Auto-uppercase<br>• Validation: 00-FF |
| + Add | Button | Add byte to sequence | • Adds new byte textbox<br>• Max 8 bytes |
| Clear | Button | Clear all bytes | • Removes all bytes<br>• Resets to empty |
| Preview | Label | ASCII representation | • Shows \r, \n, or printable char<br>• Example: "0x0D 0x0A" → "\r\n (CRLF)" |
| Occurrences | Label | Count found in data | • Only shown after auto-detect<br>• Hidden in Manual mode |

**Workflow:**

**Auto Mode (Default):**
1. User clicks "Load & Auto-Detect"
2. System detects terminator: `0x0D 0x0A` (98.5% confidence)
3. Radio button "Auto" is selected
4. Byte fields are **populated and locked** (read-only)
5. Preview shows: "\r\n (CRLF)"
6. Occurrences shows: "1,247"
7. User sees green checkmark ✅ (high confidence)

**Switch to Manual:**
1. User clicks "Manual" radio button
2. Byte fields **unlock** (become editable)
3. Existing auto-detected bytes remain (user can edit)
4. Confidence/Occurrences hide (not applicable)
5. User can modify: Change 0x0A → 0x0D (just CR)
6. Or click Clear → Start fresh

**Manual Entry from Scratch:**
1. No file loaded yet, or user clicked Reset
2. User selects "Manual" radio button
3. User enters bytes: [0x0D] [0x0A]
4. Clicks "+ Add" if needs more bytes
5. Preview updates in real-time

---

### Section 2: Field Delimiter ✂️

**Required:** No (optional - some protocols don't have delimiters)

**Controls:** Same as Terminator, PLUS:

| Control | Type | Purpose |
|---------|------|---------|
| None Checkbox | Checkbox | No delimiter protocol | • If checked: Disables byte editors<br>• For fixed-length protocols |

**Workflow:**

**Auto Mode (Delimiter Detected):**
- Auto-detect finds: `0x2C` (comma) with 95.2% confidence
- Fields populate: `[0x2C]`
- Preview: ", (comma)"
- User accepts or switches to Manual to change

**Auto Mode (No Delimiter Detected):**
- Auto-detect finds: Confidence < 50% (no clear delimiter)
- Radio button: "Auto (Not detected)"
- "None" checkbox auto-checked
- Byte fields disabled
- User can:
  - Accept (no delimiter)
  - Uncheck "None" and manually enter delimiter
  - Switch to Manual mode

---

### Section 3 & 4: Start/End Markers 🏁

**Required:** No (optional - for protocols with frame markers)

**Controls:** Same as Delimiter (with None checkbox)

**Use Cases:**

**Protocol WITH markers:**
- Example: STX...ETX protocol
- Start: 0x02 (STX)
- End: 0x03 (ETX)
- User enables and enters manually (rare bytes, auto-detect may miss)

**Protocol WITHOUT markers:**
- Most common (uses terminator only)
- "None" checkbox checked by default
- Fields disabled

**Workflow:**

**Auto-Detect (No Markers):**
1. System doesn't find start/end markers (rare bytes)
2. Radio: "Auto (Not detected)"
3. "None" checkbox auto-checked
4. User can manually enable if they know markers exist

**Manual Entry:**
1. User unchecks "None"
2. Selects "Manual"
3. Enters: Start = 0x02, End = 0x03
4. Analysis will use these to find package boundaries

---

### Section 5: Encoding 📝

**Required:** Yes (default: ASCII)

**Controls:**

| Control | Type | Options | Purpose |
|---------|------|---------|---------|
| Mode Radio | Radio buttons | Auto/Manual | Auto uses detected encoding |
| Encoding Dropdown | ComboBox | ASCII, UTF-8, Binary | Manual selection |

**Options:**
- ASCII (7-bit)
- UTF-8 (Unicode)
- Binary (no text encoding)

**Workflow:**

**Auto Mode:**
- System detects encoding based on byte ranges
- 0x00-0x7F only → ASCII
- 0x80-0xFF present → UTF-8 or Binary
- User sees: "Auto (ASCII ✅)"

**Manual Mode:**
- User selects from dropdown
- Used for text preview and field type detection

---

## Quick Presets

**Purpose:** One-click common values

**Preset Buttons:**

| Button | Fills | Use Case |
|--------|-------|----------|
| [CRLF \r\n] | Terminator: 0x0D 0x0A | Windows line ending |
| [LF \n] | Terminator: 0x0A | Unix line ending |
| [CR \r] | Terminator: 0x0D | Mac classic line ending |
| [Comma ,] | Delimiter: 0x2C | CSV-like format |
| [Space] | Delimiter: 0x20 | Space-separated |
| [Tab] | Delimiter: 0x09 | TSV format |

**Behavior:**
- Click preset → Fills active section
- Active section = last clicked byte editor
- Example: User clicks in Delimiter field → Clicks [Comma] → Fills 0x2C

---

## Actions Panel

### Save Settings

**Button:** "💾 Save Settings..."

**Action:**
1. Opens file dialog: "Save Detection Settings"
2. Default filename: "[DeviceName]-Settings.json"
3. Saves JSON with current detection config:

```json
{
  "settingsVersion": "1.0",
  "deviceName": "Weight Scale QA",
  "detectionSettings": {
    "terminator": {
      "bytes": "0x0D 0x0A",
      "mode": "Manual"
    },
    "delimiter": {
      "bytes": "0x2C",
      "mode": "Auto"
    },
    "startMarker": null,
    "endMarker": null,
    "encoding": "ASCII"
  }
}
```

**Use Case:**
- User analyzes new device, manually configures detection
- Saves settings for future use
- Next time: Import instead of re-configuring

---

### Import Settings

**Button:** "📂 Import Settings..."

**Action:**
1. Opens file dialog: "Import Detection Settings or Definition"
2. Accepts:
   - Settings file (Settings.json)
   - **OR Full Definition file** (Definition.json)
3. If Definition file → Extracts just `detectionSettings` section
4. Populates all fields

**Workflow:**
1. User previously exported full definition (from Page 4)
2. Analyzing same device again
3. Clicks "Import Settings"
4. Selects previous Definition.json
5. System extracts detection settings
6. All fields auto-populate
7. User clicks "Next: Analyze" (skip manual config)

**Smart Import:**
- If Definition file has `detectionSettings` → Use it
- If old Definition (no settings) → Show warning "No detection settings in file"
- If Settings file → Load directly

---

### Reset

**Button:** "🔄 Reset"

**Action:**
1. Clears all manual inputs
2. Resets all modes to "Auto"
3. Re-runs auto-detection if file is loaded
4. Or clears everything if no file

**Use Case:**
- User made mistakes in manual entry
- Wants to start over with auto-detection

---

## Data Flow

### Load & Auto-Detect Process

**Step 1: Parse File**
```
User clicks "Load & Auto-Detect"
  ↓
ParserService.ParseFile(path, format)
  ↓
LogFile object created
  ↓
RawData extracted (byte[])
```

**Step 2: Run Quick Detection**
```
RawData (byte[])
  ↓
QuickDetectionService.Detect(rawData)
  ↓
Analyzes ONLY:
  - Terminator (find repeated sequences at regular intervals)
  - Delimiter (find high-frequency bytes within packages)
  - Encoding (check byte ranges)
  - Markers (find rare bytes at start/end positions)
  ↓
Returns: QuickDetectionResult
  {
    terminator: { bytes, confidence },
    delimiter: { bytes, confidence },
    startMarker: null,
    endMarker: null,
    encoding: "ASCII"
  }
```

**Step 3: Populate UI**
```
QuickDetectionResult
  ↓
For each detection:
  - Set Mode radio to "Auto"
  - Fill byte editors (locked)
  - Show confidence badge
  - Show occurrences
  - Update preview
  ↓
User sees pre-populated form
```

---

### Next: Analyze Process

**User clicks "▶ Next: Analyze"**

**System validates:**
1. ✓ File loaded
2. ✓ Terminator defined (required)
3. ✓ At least one byte in terminator
4. ⚠️ If Manual mode with low confidence → Show warning

**System packages settings:**
```csharp
var detectionConfig = new DetectionConfiguration
{
    Terminator = GetBytes(TerminatorByteEditors),
    Delimiter = HasDelimiter ? GetBytes(DelimiterByteEditors) : null,
    StartMarker = HasStartMarker ? GetBytes(StartMarkerByteEditors) : null,
    EndMarker = HasEndMarker ? GetBytes(EndMarkerByteEditors) : null,
    Encoding = SelectedEncoding
};

_model.DetectionConfiguration = detectionConfig;
_model.RawData = rawData;
_model.LogFile = logFile;
```

**Navigate to Page 2:**
- Page 2 receives `DetectionConfiguration`
- Analysis uses configured settings as **constraints**
- Field detection runs with known structure

---

## Updated Data Model

### New Class: DetectionConfiguration

```csharp
/// <summary>
/// User-configured or auto-detected protocol detection settings.
/// Used to constrain analysis algorithms.
/// </summary>
public class DetectionConfiguration
{
    /// <summary>
    /// Package terminator bytes (required).
    /// Marks end of a complete package/message.
    /// </summary>
    public byte[] Terminator { get; set; }

    /// <summary>
    /// Field delimiter bytes (optional).
    /// Separates fields within a package.
    /// Null if no delimiter (fixed-length protocol).
    /// </summary>
    public byte[] Delimiter { get; set; }

    /// <summary>
    /// Start marker bytes (optional).
    /// Marks beginning of a package (e.g., STX = 0x02).
    /// Null if no start marker.
    /// </summary>
    public byte[] StartMarker { get; set; }

    /// <summary>
    /// End marker bytes (optional).
    /// Marks end of a package, in addition to terminator (e.g., ETX = 0x03).
    /// Null if no end marker.
    /// </summary>
    public byte[] EndMarker { get; set; }

    /// <summary>
    /// Text encoding type.
    /// </summary>
    public EncodingType Encoding { get; set; }

    /// <summary>
    /// Detection mode for each setting (Auto vs Manual).
    /// </summary>
    public DetectionModeInfo ModeInfo { get; set; }
}

public class DetectionModeInfo
{
    public DetectionMode TerminatorMode { get; set; }
    public double TerminatorConfidence { get; set; }

    public DetectionMode DelimiterMode { get; set; }
    public double DelimiterConfidence { get; set; }

    public DetectionMode StartMarkerMode { get; set; }
    public DetectionMode EndMarkerMode { get; set; }

    public DetectionMode EncodingMode { get; set; }
}

public enum DetectionMode
{
    Auto,      // Auto-detected
    Manual,    // User-specified
    None       // Not applicable (e.g., no delimiter)
}
```

### Updated ProtocolAnalyzerModel

```csharp
public class ProtocolAnalyzerModel : INotifyPropertyChanged
{
    // Page 1 → Page 2
    public LogFile LogFile { get; set; }
    public byte[] RawData { get; set; }
    public DetectionConfiguration DetectionConfig { get; set; }  // ← NEW

    // Page 2 → Page 3
    public AnalysisResult AnalysisResult { get; set; }
    public List<FieldInfo> Fields { get; set; }

    // Page 4 → Export
    public ProtocolDefinition ProtocolDefinition { get; set; }
}
```

---

## Validation Rules

### Before "Next: Analyze"

**Required Checks:**

| Check | Rule | Error Message |
|-------|------|---------------|
| File Loaded | LogFile != null | "Please load a log file first." |
| Terminator Defined | Terminator has ≥1 byte | "Package terminator is required." |
| Valid Hex | All bytes 0x00-0xFF | "Invalid hex byte: {value}" |
| Unique Bytes | No duplicate sequences | Warning: "Delimiter and terminator are the same - is this correct?" |

**Optional Warnings:**

| Condition | Warning |
|-----------|---------|
| Manual mode + No file loaded | "Cannot verify bytes without loaded data. Analysis may fail." |
| Terminator confidence < 60% | "Low confidence detection (45%). Consider manual review." |
| Delimiter = Space (0x20) | "Space delimiter detected. Fixed-length fields may work better." |

---

## Keyboard Shortcuts

| Key | Action |
|-----|--------|
| F5 | Load & Auto-Detect |
| Ctrl+S | Save Settings |
| Ctrl+O | Import Settings |
| Ctrl+R | Reset |
| Enter | Next: Analyze (if valid) |

---

## Edge Cases

### Case 1: No Terminator Detected

**Scenario:** Binary protocol with non-standard terminator

**Auto-Detect Result:**
- Confidence: 35% (low)
- Best guess: 0x00 (NULL)

**UI Behavior:**
- Shows: "Auto (35% ⚠️ Low confidence)"
- Yellow warning badge
- Byte fields show: [0x00]
- User should review and likely switch to Manual

---

### Case 2: Multiple Possible Delimiters

**Scenario:** Protocol has comma AND space (CSV-like: "Field1, Field2, Field3")

**Auto-Detect Result:**
- Comma: 45% confidence
- Space: 42% confidence
- Close call

**UI Behavior:**
- Shows highest: "Auto (45% ⚠️)"
- Fills: [0x2C] (comma)
- User might need to test both

**Future Enhancement:** Show "Alternative: 0x20 (42%)" hint

---

### Case 3: Multi-Byte Terminator

**Scenario:** Double CRLF terminator: 0x0D 0x0A 0x0D 0x0A

**Auto-Detect Result:**
- Detects 4-byte sequence

**UI Behavior:**
- Fills 4 bytes: [0x0D] [0x0A] [0x0D] [0x0A]
- Preview: "\r\n\r\n (Double CRLF)"
- All byte editors shown

---

## Summary: What Changes

### From Old Page 1 to New Page 1

**Added:**
- ✅ Protocol Detection Configuration panel
- ✅ Auto-detect on file load
- ✅ Manual override controls
- ✅ Quick presets
- ✅ Save/Import settings
- ✅ Confidence indicators

**Removed:**
- Nothing (everything from old Page 1 kept)

**Modified:**
- "Load File" button → "Load & Auto-Detect" (runs detection immediately)

---

### From Old Page 2 to New Page 2

**Removed:**
- ❌ Terminator detection panel (moved to Page 1)
- ❌ Delimiter detection panel (moved to Page 1)
- ❌ Protocol type panel (moved to Page 1)
- ❌ "Run Analysis" button (Page 1 does quick detection, Page 2 does field analysis)

**Kept:**
- ✅ Overall confidence
- ✅ Detected fields DataGrid
- ✅ Field preview

**Modified:**
- Focus shifts to **field detection** only
- Analysis runs with pre-configured detection settings
- Simpler, focused page

---

## Benefits of New Design

1. **Upfront Configuration:**
   - User configures protocol structure BEFORE full analysis
   - Page 2 focuses on field detection only
   - Cleaner separation of concerns

2. **Auto-Detect Assistance:**
   - System does the heavy lifting
   - User just verifies/refines
   - Faster workflow for most cases

3. **Manual Override:**
   - Expert users can configure exactly
   - Low-confidence cases get user input
   - Flexible for all protocol types

4. **Reusable Settings:**
   - Save settings for device types
   - Import from previous definitions
   - No re-configuration needed

5. **Better UX:**
   - One-time setup (Page 1)
   - Automatic field detection (Page 2)
   - Edit field names (Page 3)
   - Export (Page 4)
   - Linear, logical workflow

---

**Document Version**: 1.0
**Last Updated**: 2025-10-26
**Related Documents**:
- 06-Protocol-Analyzer-Complete-UI.md - Overall UI design
- 07-Page-Content-Overview.md - Page content reference
- 04-Data-Models-Design.md - Data model definitions
