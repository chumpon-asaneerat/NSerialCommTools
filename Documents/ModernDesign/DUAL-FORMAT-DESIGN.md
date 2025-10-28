# Dual-Format Design Pattern (Hex + Text)

**Document:** Dual-Format Data Model Design
**Version:** 1.1
**Date:** 2025-10-28
**Purpose:** Explain the Hex + Text dual representation pattern used throughout the Protocol Analyzer

**Updates:**
- v1.0: Initial dual-format design
- v1.1: Removed obsolete RawData properties - clean implementation

---

## Overview

The Protocol Analyzer supports **BOTH text-based and binary protocols** by storing data as **byte arrays** and providing **dual computed properties** for viewing the same data in different formats.

### Design Philosophy

```
Source of Truth: byte[] RawBytes
         ↓
    ┌────┴────┐
    ↓         ↓
RawHex    RawText
(Computed) (Computed)
```

**Key Principle:** Store bytes once, display multiple ways.

---

## Pattern Implementation

### Classes Using Dual-Format Pattern

1. **LogEntry** - Individual log entries from captured data
2. **PackageInfo** - Parsed packages from log data
3. **SegmentInfo** - Individual segments within packages
4. **FieldInfo** - Field definitions with sample values

### Standard Pattern Structure

```csharp
public class DataClass
{
    // SOURCE OF TRUTH (stored)
    public byte[] RawBytes { get; set; }

    // HEX REPRESENTATION (computed)
    public string RawHex
    {
        get
        {
            if (RawBytes == null || RawBytes.Length == 0)
                return string.Empty;
            return BitConverter.ToString(RawBytes).Replace("-", " ");
        }
    }

    // TEXT REPRESENTATION (computed)
    public string RawText
    {
        get
        {
            if (RawBytes == null || RawBytes.Length == 0)
                return string.Empty;
            try
            {
                return System.Text.Encoding.ASCII.GetString(RawBytes);
            }
            catch
            {
                return "[Binary Data]";
            }
        }
    }

    // LENGTH (computed)
    public int Length
    {
        get { return (RawBytes != null) ? RawBytes.Length : 0; }
    }
}
```

---

## Benefits of Dual-Format Design

### ✅ Supports All Protocol Types

| Protocol Type | Example Device | Primary View | Secondary View |
|---------------|----------------|--------------|----------------|
| Text-based | DEFENDER3000 | RawText | RawHex (for debugging) |
| Binary | BinaryScaleDevice | RawHex | RawText (shows unprintable chars) |
| Mixed | TFO1 | Both equally important | Both |

### ✅ Debugging & Analysis

Users can see:
- **Text View:** `"  0.360 kg    G\r\n"`
- **Hex View:** `20 20 20 30 2E 33 36 30 20 6B 67 20 20 20 20 47 0D 0A`

Perfect for:
- Finding hidden characters (spaces, tabs, CR/LF)
- Detecting encoding issues
- Analyzing binary protocols
- Verifying checksums

### ✅ Aligns with Document 05 Examples

Document 05 v2.2 shows ALL examples with both formats:

```
Text:   "   0.360 kg    G\r\n"
Hex:    20 20 20 30 2E 33 36 30 20 6B 67 20 20 20 20 47 0D 0A
```

The Model classes now match this design!

---

## Usage Examples

### Example 1: Loading Text Protocol Data

```csharp
// Load from text log
string textData = "  0.360 kg    G\r\n";
var entry = new LogEntry(DateTime.Now, textData, "RX");

// View in different formats
Console.WriteLine($"Text: {entry.RawText}");
// Output: "  0.360 kg    G\r\n"

Console.WriteLine($"Hex:  {entry.RawHex}");
// Output: "20 20 20 30 2E 33 36 30 20 6B 67 20 20 20 20 47 0D 0A"
```

### Example 2: Loading Binary Protocol Data

```csharp
// Load from binary log
byte[] binaryData = new byte[] { 0x02, 0x41, 0x00, 0x64, 0x12, 0x34, 0x03, 0xC5 };
var entry = new LogEntry(DateTime.Now, binaryData, "RX");

// View in different formats
Console.WriteLine($"Hex:  {entry.RawHex}");
// Output: "02 41 00 64 12 34 03 C5"

Console.WriteLine($"Text: {entry.RawText}");
// Output: "\x02A\x00d\x124\x03Å" (shows binary as text - not human readable)
```

### Example 3: UI DataGrid Display

```xml
<!-- Show BOTH columns in DataGrid -->
<DataGrid ItemsSource="{Binding LogEntries}">
    <DataGrid.Columns>
        <DataGridTextColumn Header="#" Binding="{Binding Index}"/>
        <DataGridTextColumn Header="Timestamp" Binding="{Binding Timestamp}"/>
        <DataGridTextColumn Header="Hex" Binding="{Binding RawHex}" Width="300"/>
        <DataGridTextColumn Header="Text" Binding="{Binding RawText}" Width="200"/>
        <DataGridTextColumn Header="Length" Binding="{Binding Length}"/>
    </DataGrid.Columns>
</DataGrid>
```

Result:
```
# | Timestamp | Hex                                      | Text           | Length
--+-----------+------------------------------------------+----------------+-------
1 | 10:30:15  | 20 20 20 30 2E 33 36 30 20 6B 67 20 ... |   0.360 kg  G  | 18
2 | 10:30:16  | 02 41 00 64 12 34 03 C5                  | [Binary Data]  | 8
```

---

## Special Cases

### FieldInfo - With Encoding Support

`FieldInfo` has **encoding-aware** text conversion:

```csharp
public class FieldInfo
{
    public byte[] SampleBytes { get; set; }
    public EncodingType EncodingType { get; set; }

    // Uses the specified encoding
    public string SampleText
    {
        get
        {
            var encoding = GetEncoding(); // ASCII, UTF-8, UTF-16, Latin-1
            return encoding.GetString(SampleBytes);
        }
    }
}
```

This handles:
- UTF-8 multi-byte characters (°C = C2 B0)
- UTF-16 wide characters
- Latin-1 extended ASCII

---

## UI Display Recommendations

### Log Data View (Page 1)
- Show BOTH Hex and Text columns
- User can compare side-by-side
- Hex column helps identify terminators (0D 0A)

### Package Detail View (Page 2)
- Toggle between Hex/Text view
- Or show both in split view
- Highlight selected bytes in both views

### Field Editor (Page 3)
- Show sample values in BOTH formats
- Helps user understand what they're parsing
- Example: "30 2E 33 36 30" = "0.360"

---

## Performance Considerations

### ✅ Efficient
- Computed properties only evaluate when accessed
- No duplicate storage
- Single source of truth (RawBytes)

### ⚠️ Consider Caching for Large Data
If performance becomes an issue with very large logs:

```csharp
private string _cachedHex;
public string RawHex
{
    get
    {
        if (_cachedHex == null && RawBytes != null)
            _cachedHex = BitConverter.ToString(RawBytes).Replace("-", " ");
        return _cachedHex ?? string.Empty;
    }
}
```

---

## Summary

### ✅ Benefits Achieved

1. **Universal Protocol Support** - Works for text, binary, and mixed protocols
2. **Better Debugging** - See exact byte values
3. **User Choice** - Users pick their preferred view
4. **Document Alignment** - Matches Document 05 v2.2 examples
5. **Flexibility** - Easy to add more representations later

### 📊 Implementation Status

| Class | Dual Format | Status |
|-------|-------------|--------|
| LogEntry | ✅ | Implemented |
| PackageInfo | ✅ | Implemented |
| SegmentInfo | ✅ | Implemented |
| FieldInfo | ✅ | Implemented (with encoding support) |

---

**This design ensures the Protocol Analyzer can handle ANY serial protocol - from simple ASCII text to complex binary formats with checksums!**

**Last Updated:** 2025-10-28
**Version:** 1.1

---

## Summary of Changes

### v1.1 Updates (2025-10-28)
- ✅ Removed all `[Obsolete]` RawData properties
- ✅ Clean, modern API with only RawBytes, RawHex, RawText
- ✅ Simplified Length property (direct computation)
- ✅ No backward compatibility needed (fresh implementation)
