# TScale Protocol Analysis (NHB and QHW)

## Overview

Analysis of two T&T scale models based on captured HEX dump log files. Both scales use **similar but distinct protocols** - they share the same base structure but differ in field separators.

---

## Device Comparison

| Feature | TScaleNHB | TScaleQHW |
|---------|-----------|-----------|
| **Protocol Type** | CSV-like continuous stream | CSV-like continuous stream |
| **Separator Pattern** | `ST,GS ` (no comma after GS) | `ST,GS,` (comma after GS) |
| **Weight Format** | Variable width, right-aligned | Variable width, right-aligned |
| **Precision** | 1 decimal place (0.1 g) | 1 decimal place (0.1 g) |
| **Unit** | Always 'g' (grams) | Always 'g' (grams) |
| **Status Indicators** | ST (Stable) / US (Unstable) | ST (Stable) / US (Unstable) |
| **Mode** | GS (Gross/Stable) | GS (Gross/Stable) |
| **Terminator** | CR+LF (0x0D 0x0A) | CR+LF (0x0D 0x0A) |
| **Typical Use** | Light precision weighing | Medium precision weighing |

---

## Protocol Format

### TScaleNHB Format

```
ST,GS    20.7g  \r\n
```

**HEX:**
```
53 54 2C 47 53 20 20 20 20 32 30 2E 37 67 20 20 0D 0A
```

**Structure:**
- `ST` or `US` - Stability indicator (2 bytes)
- `,` - Comma separator (1 byte)
- `GS` - Mode indicator (2 bytes)
- ` ` (spaces) - Padding before weight (variable, typically 4 spaces)
- `XX.Xg` - Weight value with unit (variable width)
- `  ` - Trailing spaces (variable, typically 2 spaces)
- `\r\n` - CR+LF terminator (2 bytes)

**Total Length:** Variable (typically 18 bytes)

---

### TScaleQHW Format

```
ST,GS,   245.6 g\r\n
```

**HEX:**
```
53 54 2C 47 53 2C 20 20 20 32 34 35 2E 36 20 67 0D 0A
```

**Structure:**
- `ST` or `US` - Stability indicator (2 bytes)
- `,` - Comma separator (1 byte)
- `GS` - Mode indicator (2 bytes)
- `,` - **Second comma separator** (1 byte) - **KEY DIFFERENCE**
- ` ` (spaces) - Padding before weight (variable, typically 3 spaces)
- `XXX.X ` - Weight value (variable width)
- `g` - Unit character (1 byte)
- `\r\n` - CR+LF terminator (2 bytes)

**Total Length:** Variable (typically 18 bytes)

---

## HEX Dump Analysis

### TScaleNHB - Sample Readings

**Stable Reading (20.7g):**
```
53 54 2C 47 53 20 20 20 20 32 30 2E 37 67 20 20 0D 0A    ST,GS    20.7g  ..
```

**ASCII Breakdown:**
```
53 54    "ST" - Stable
2C       "," - separator
47 53    "GS" - Gross/Stable mode
20 20 20 20    4 spaces (padding)
32 30 2E 37    "20.7" - weight value
67       "g" - unit
20 20    2 spaces (trailing)
0D 0A    CR+LF
```

**Unstable Reading (Observed in NHB.log line 372-374):**
```
55 53 2C 47 53 20 20 20 20 32 30 2E 37 67 20 20 0D 0A    US,GS    20.7g  ..
```
- `55 53` = "US" (Unstable)

**Higher Weight Reading (85.5g - from NHB2.log):**
```
53 54 2C 47 53 20 20 20 20 38 35 2E 35 67 20 20 0D 0A    ST,GS    85.5g  ..
```

**Weight Over 100g (106.2g - from NHB2.log):**
```
53 54 2C 47 53 20 20 20 31 30 36 2E 32 67 20 20 0D 0A    ST,GS   106.2g  ..
```
- Note: 3 spaces instead of 4 (right-aligned formatting)

---

### TScaleQHW - Sample Readings

**Stable Reading (245.6g):**
```
53 54 2C 47 53 2C 20 20 20 32 34 35 2E 36 20 67 0D 0A    ST,GS,   245.6 g..
```

**ASCII Breakdown:**
```
53 54       "ST" - Stable
2C          "," - first separator
47 53       "GS" - Gross/Stable mode
2C          "," - second separator (KEY DIFFERENCE)
20 20 20    3 spaces (padding)
32 34 35 2E 36    "245.6" - weight value
20          space before unit
67          "g" - unit
0D 0A       CR+LF
```

**Lower Weight Reading (8.0g - from QHW2.log):**
```
53 54 2C 47 53 2C 20 20 20 20 20 38 2E 30 20 67 0D 0A    ST,GS,     8.0 g..
```
- Note: 5 spaces before weight (more padding for smaller numbers)

**Weight Changing (7.9g - from QHW2.log):**
```
53 54 2C 47 53 2C 20 20 20 20 20 37 2E 39 20 67 0D 0A    ST,GS,     7.9 g..
```

---

## Field Analysis

### Stability Status

| Hex | ASCII | Meaning | Frequency in Logs |
|-----|-------|---------|-------------------|
| 53 54 | ST | Stable weight | Most common |
| 55 53 | US | Unstable weight | During weight changes |

**Observations:**
- **ST** appears when weight has stabilized
- **US** appears briefly during weight changes (adding/removing items)
- Transition: `ST → US → ST` when weight changes

---

### Mode Indicator

| Hex | ASCII | Meaning |
|-----|-------|---------|
| 47 53 | GS | Gross / Stable mode |

**Note:** Only "GS" observed in all log files - no other modes detected.

---

### Weight Value Format

**Characteristics:**
- **Format:** `XX.X` or `XXX.X` (1 decimal place)
- **Precision:** 0.1 g (100 mg)
- **Right-Aligned:** Leading spaces for smaller values
- **No Sign:** Only positive weights observed
- **Unit:** Always lowercase 'g'

**Examples:**
```
NHB:     20.7g      (2 digits before decimal)
NHB:     85.5g      (2 digits before decimal)
NHB:    106.2g      (3 digits before decimal, note extra space)
QHW:    245.6 g     (3 digits, space before unit)
QHW:      8.0 g     (1 digit, more padding)
```

---

## Protocol Observations

### NHB-Specific Patterns

1. **No comma after GS:** `ST,GS ` not `ST,GS,`
2. **Weight range observed:** 20.7g to 106.2g
3. **Stability transitions:** ST ↔ US transitions observed in log
4. **Unit attached:** Unit 'g' directly follows weight with no space

### QHW-Specific Patterns

1. **Extra comma:** `ST,GS,` with comma after GS
2. **Weight range observed:** 7.9g to 245.6g (wider range)
3. **Space before unit:** Weight value, space, then 'g'
4. **More padding:** Uses up to 5 spaces for alignment

---

## Parsing Strategy

### Common Elements (Both Scales)

```csharp
// 1. Wait for CR+LF terminator
// 2. Extract line
// 3. Convert to ASCII string
// 4. Parse fields
```

### TScaleNHB Parsing

```csharp
string line = "ST,GS    20.7g  ";

// Method 1: String Split
string[] parts = line.Split(',');  // ["ST", "GS    20.7g  "]
string stability = parts[0].Trim();  // "ST"

string rest = parts[1];
string mode = rest.Substring(0, 2);  // "GS"
string weightPart = rest.Substring(2).Trim();  // "20.7g"

// Extract weight value (remove 'g')
string weightStr = weightPart.TrimEnd('g').Trim();  // "20.7"
decimal weight = decimal.Parse(weightStr);  // 20.7
```

### TScaleQHW Parsing

```csharp
string line = "ST,GS,   245.6 g";

// Method 1: String Split by comma
string[] parts = line.Split(',');  // ["ST", "GS", "   245.6 g"]
string stability = parts[0].Trim();  // "ST"
string mode = parts[1].Trim();  // "GS"
string weightPart = parts[2].Trim();  // "245.6 g"

// Extract weight value (remove 'g')
string weightStr = weightPart.TrimEnd('g').Trim();  // "245.6"
decimal weight = decimal.Parse(weightStr);  // 245.6
```

---

## Implementation Recommendations

### Shared Base Class

Both scales can share a common data structure:

```csharp
public class TScaleData : SerialDeviceData
{
    private string _stability;  // "ST" or "US"
    private string _mode;       // "GS"
    private decimal _weight;    // Weight value
    private string _unit;       // "g"

    public string Stability { get; set; }
    public string Mode { get; set; }
    public decimal Weight { get; set; }
    public string Unit { get; set; }
    public bool IsStable => Stability == "ST";
}
```

### Separate Terminal Classes

**TScaleNHBTerminal:**
```csharp
// Parse without second comma
// Unit directly follows weight
```

**TScaleQHWTerminal:**
```csharp
// Parse with second comma (3 fields)
// Space between weight and unit
```

---

## Comparison with Existing Devices

### vs WeightQA

| Feature | TScale (NHB/QHW) | WeightQA |
|---------|------------------|----------|
| **Format** | ST,GS or ST,GS, | +XXX.XX/S |
| **Stability** | ST/US prefix | Numeric 0-8 |
| **Precision** | 0.1g | 0.01kg (10g) |
| **Separator** | Comma | Slash |
| **Sign** | No | Yes (+/-) |
| **Complexity** | ⭐⭐ Simple | ⭐⭐ Simple |

### vs DEFENDER3000/WeightSPUN

| Feature | TScale (NHB/QHW) | DEFENDER3000 |
|---------|------------------|--------------|
| **Format** | ST,GS,XXX.Xg | `   X.XXX kg    G` |
| **Stability** | ST/US | ?G/?N |
| **Field Count** | 2-3 fields | 3 fields |
| **Separator** | Comma | Spaces |
| **Unit Position** | End of weight | Separate field |
| **Complexity** | ⭐⭐ Simple | ⭐ Simple |

---

## Log File Statistics

### TScaleNHB Logs

**NHB.log:**
- **Readings:** ~460 lines
- **Stable:** ~455 (ST)
- **Unstable:** ~5 (US) - lines 372-374
- **Weight:** Consistently 20.7g

**NHB2.log:**
- **Readings:** ~522 lines
- **Weight progression:** 85.5g → varying weights → 106.2g (stable)
- **Unstable transitions:** Lines 159-162, 192-366, 426-495

### TScaleQHW Logs

**QHW.log:**
- **Readings:** Many (large file)
- **Weight:** Consistently 245.6-245.5g
- **All stable** (ST only)

**QHW2.log:**
- **Readings:** Many (large file)
- **Weight:** Varying 7.9g to 8.0g
- **All stable** (ST only)

---

## Implementation Checklist

### For TScaleNHB

- [ ] Create `TScaleNHBData` class
- [ ] Create `TScaleNHBTerminal` class
- [ ] Implement parsing for `ST,GS XXX.Xg` format
- [ ] Handle ST/US stability transitions
- [ ] Test with weights 1-999.9g
- [ ] Handle right-alignment padding

### For TScaleQHW

- [ ] Create `TScaleQHWData` class (or reuse TScaleNHBData)
- [ ] Create `TScaleQHWTerminal` class
- [ ] Implement parsing for `ST,GS,XXX.X g` format (note extra comma)
- [ ] Handle space before unit character
- [ ] Test with weights 1-999.9g
- [ ] Handle right-alignment padding

### Shared Components

- [ ] Consider base `TScaleData` class
- [ ] Document differences in protocol
- [ ] Create unit tests for both formats
- [ ] Add to emulator applications

---

## Key Differences Summary

| Aspect | NHB | QHW |
|--------|-----|-----|
| **Comma Count** | 1 | 2 |
| **Pattern** | `ST,GS ` | `ST,GS,` |
| **Space Before Unit** | No | Yes |
| **Weight Format** | `XX.Xg` | `XX.X g` |
| **Parsing Complexity** | Slightly harder | Easier (CSV-like) |

---

## Recommended Approach

### Option 1: Separate Implementations

- **Pros:** Clear separation, easier to maintain
- **Cons:** Code duplication

### Option 2: Unified with Flag

- **Pros:** Code reuse
- **Cons:** Added complexity with conditional logic

### **Recommendation:** Option 1 (Separate)

Given the subtle but important differences in parsing (comma count, space before unit), maintaining separate terminal classes with shared data class is cleaner and less error-prone.

---

## Next Steps

1. Implement `TScaleNHBTerminal` and `TScaleNHBData`
2. Implement `TScaleQHWTerminal` (reuse or extend NHB data class)
3. Add to emulator/terminal applications
4. Create comprehensive unit tests
5. Document in main device documentation
