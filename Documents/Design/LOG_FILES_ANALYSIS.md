# Log Files Analysis Summary

**Date:** 2025-10-08
**Purpose:** Analysis of serial communication log files captured from third-party tools

---

## Overview

The log files in `Documents/LuckyTex Devices/` contain actual serial communication data captured from various weighing scales and measurement devices. These logs were used as reference for implementing the Core serial classes in `NLib.Serial.Devices`.

---

## Device Protocol Analysis

### 1. DEFENDER3000 (Weight Scale)
**Log Files:**
- `DEFENDER3000_txt.txt` - Text format communication
- `DEFENDER3000_hex.txt` - Hexadecimal format communication

**Protocol Characteristics:**
- **Output Format:** `[sign] [weight] kg [status]`
- **Examples:**
  - `-  1.640 kg    N` (Negative weight, Net mode)
  - `   0.360 kg    G` (Gross mode)
- **Status Indicators:**
  - `N` - Net weight mode
  - `G` - Gross weight mode
  - `?N` or `?G` - Unstable reading (weight still changing)
- **Update Rate:** Continuous streaming (multiple readings per second)
- **Line Terminator:** Likely CR LF (`\r\n`)
- **Weight Range:** Observed from 0.000 kg to 1.695 kg
- **Precision:** 3 decimal places (0.005 kg resolution)

**Key Observations:**
- Device continuously sends weight readings
- Stability indicator changes from `?` to stable when weight settles
- Format is fixed-width text for easy parsing

---

### 2. JIK6CAB (Weight Scale)
**Log Files:**
- `jik_txt_1.txt`, `jik_txt_2.txt` - Text format
- `jik_hex_1.txt`, `jik_hex_2.txt` - Hex format

**Protocol Characteristics:**
- **Output Format:** Structured multi-line response
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

**Field Breakdown:**
- Line 1: `^KJIK000` - Device identifier/header
- Line 2: Date (YYYY-MM-DD)
- Line 3: Time (HH:MM:SS)
- Line 4: Tare weight
- Line 5: Gross weight
- Lines 6-7: Unknown counters or flags
- Line 8: Net weight
- Line 9: Display weight
- Line 10: Piece count
- Lines 11-12: Empty
- Line 13: Status code `E`
- Line 14: `~P1` - Command response or mode indicator

**Key Observations:**
- More complex protocol with multiple data points
- Includes date/time stamps
- Supports piece counting functionality
- Command-response based communication

---

### 3. MS204TS00 (Mettler Toledo Precision Balance)
**Log Files:**
- `Mettler Toledo (MS204TS00) - NPT Mettler QDMS.txt` - Text format
- `Mettler Toledo (MS204TS00) -  NPT Mettler QDMS Hex.txt` - Hex format
- Images: `332318.jpg`, `77698A3E-076D-4D41-82F0-1FECA45DB13E.jpg`

**Protocol Characteristics:**
- **Output Format:** `[status] [weight] g [precision]`
- **Example:** `     N       0.3746 g   ..`

**Protocol Details:**
- Status character: `N` (appears to be stability or mode indicator)
- Weight format: Right-aligned, multiple spaces
- Unit: grams (g)
- Precision indicators: `..` at end of line
- High precision: 4 decimal places (0.0001 g resolution)

**Key Observations:**
- Professional laboratory balance protocol
- Very high precision measurements
- Mettler Toledo QDMS protocol (Query Display Measurement System)
- Minimal output format - weight on demand

---

### 4. PH Meter
**Log Files:**
- `Serial_Log PH.txt` - Combined text and hex
- `Serial_Log PH.Attemp.1.txt`
- Images showing device display

**Protocol Characteristics:**
- **Output Format:** `[value]pH [temp]°C [mode]`
- **Examples:**
  - `3.01pH 25.5°C ATC` (Auto Temperature Compensation)
  - `4.77pH 24.7°C ATC`

**Additional Data:**
- Date/Time stamps: `20-Feb-2023` / `11:12`
- Measurement mode: `Auto EP Standard`
- Sample identifier: `Blank`
- Temperature compensation: `ATC` (Automatic Temperature Compensation)

**Hex Data Analysis:**
```
33 2E 30 31 70 48 20 32 35 2E 35 F8 43 20 41 54 43 0D 0A
"3.01pH 25.5°C ATC\r\n"
```
- Note: `F8` is degree symbol (°)
- Line terminator: `0D 0A` (CR LF)

**Key Observations:**
- Combines pH value with temperature
- Automatic temperature compensation
- Measurement stability monitoring
- Timestamp support for data logging
- Standards-based calibration tracking

---

### 5. TFO1 (Industrial Weight System)
**Log Files:**
- `TFO.Weight.Response.txt` - Hex format response
- `Serial_Log.txt`, `Serial_Log1.txt`
- `Serial_Loghex_text.txt`
- Multiple attempt logs
- Images of device

**Protocol Characteristics:**
- Complex binary protocol with ASCII mixed data
- **Hex Sample:**
```
20 30 2E 30 0D 58 20 20 20 20 20 20 30 2E 30 0D
41 20 20 20 20 38 31 32 2E 35 0D ...
```

**Decoded Fields:**
- `20 30 2E 30` = " 0.0"
- `58` = 'X' (command/field identifier)
- `41` = 'A' (command/field identifier)
- `38 31 32 2E 35` = "812.5" (weight value)
- `0D` = CR (field separator)
- Date/Time: `C13ô 02ó 2023ò MON 02:56PM` (custom encoding)
- Version: `V1` identifier

**Key Field Identifiers:**
- `X` - Unknown parameter (0.0 value)
- `A` - Weight value (812.5)
- `0` - Temperature? (24.0)
- `4` - Another weight? (788.5)
- `1` - Zero value
- `2` - Counter
- `B` - Status byte (0x81, 0x83)
- `C` - Date/time stamp
- `V` - Version info
- `F`, `H`, `Q` - Additional parameters

**Key Observations:**
- Industrial-grade protocol with multiple parameters
- Binary data mixed with ASCII text
- Timestamp with custom encoding
- Version tracking
- Multiple weight channels/modes
- Status flags encoded as bytes

---

### 6. Weight QA (Quality Assurance Scale)
**Log Files:**
- `Serial_Log Weight QA.txt`
- Images of device

**Protocol Characteristics:**
- **Output Format:** `[sign][weight]/[stability] [unit] [mode]`
- **Examples:**
  - `+000.00/0 G S` (Zero, stable, Gross, Static)
  - `+001.22/1 G S` (1.22, stability level 1)
  - `+007.12/4 G S` (7.12, stability level 4)

**Field Breakdown:**
- **Sign:** `+` (positive weight)
- **Weight:** 6 digits total, 2 decimal places (format: `XXX.XX`)
- **Stability Index:** `/[0-9]` - Digital stability indicator
  - `0` = Perfectly stable
  - `1-8` = Increasing instability levels
- **Unit:** `G` (grams)
- **Mode:** `S` (likely "Static" or "Standard")

**Hex Format:**
```
2B 30 30 37 2E 31 32 2F 33 20 47 20 53 0D 0A
"+007.12/3 G S\r\n"
```
- Line terminator: `0D 0A` (CR LF)

**Key Observations:**
- Unique stability index system (0-8 scale)
- Quality control focused (stability tracking)
- Continuous measurement updates
- Fixed format for easy parsing

---

### 7. Weight SPUN (Spinning/Dynamic Weight)
**Log Files:**
- `Serial_Logspuntext.txt`
- `Serial_Logspun.txt`
- Video: `51396f61-a182-42c2-a28e-255ac7f0acf5.mp4`

**Protocol Characteristics:**
- **Output Format:** `[spaces][weight] kg [spaces][status]`
- **Examples:**
  - `    20.0 kg    G` (20.0 kg, Gross mode)
  - `    19.8 kg   ?G` (19.8 kg, Unstable Gross)

**Status Indicators:**
- `G` - Gross weight, stable
- `?G` - Gross weight, unstable (weight changing)
- Stability change visible during loading/unloading

**Hex Format:**
```
20 20 20 20 32 30 2E 30 20 6B 67 20 20 20 20 47 0D 0A
"    20.0 kg    G\r\n"
```

**Weight Range Observed:**
- Static: 19.8 - 20.0 kg
- Dynamic: 20.0 - 94.6 kg (during loading)
- Shows full loading cycle from 20 → 94.6 → 78.6 kg (settling) → back to 19.8 kg

**Key Observations:**
- Similar to DEFENDER3000 format
- Higher capacity (up to 100+ kg)
- Clear stability indication during dynamic weighing
- Fixed-width formatting with padding spaces

---

### 8. TScaleNHB (T&T Weight Scale - NHB Model)
**Log Files:**
- `TScaleNHB/NHB.txt` - Hex dump format
- `TScaleNHB/NHB2.txt` - Hex dump format
- Images: `442216_0.jpg`, `442217_0.jpg`, `442218.jpg`

**Protocol Characteristics:**
- **Output Format:** `STATUS,MODE[spaces][weight]unit[spaces]\r\n`
- **Examples:**
  - `ST,GS    20.7g  ` (Stable, 20.7 grams)
  - `US,GS    20.9g  ` (Unstable, 20.9 grams)

**Field Breakdown:**
- **Status:** 2 characters
  - `ST` = Stable
  - `US` = Unstable
- **Mode:** 2 characters (after first comma)
  - `GS` = Gross/Stable mode
- **Weight:** Variable width, right-aligned with leading spaces (1 decimal place)
- **Unit:** 1-2 characters, directly attached to weight (typically "g")
- **Terminator:** `\r\n` (0x0D 0x0A)

**Hex Format:**
```
53 54 2C 47 53 20 20 20 20 32 30 2E 37 67 20 20 0D 0A
"ST,GS    20.7g  \r\n"
```

**Key Protocol Feature:** Single comma separator `ST,GS ` (note: one comma only)

**Weight Range Observed:**
- NHB.log: Consistently 20.7g
- NHB2.log: 85.5g to 106.2g

**Key Observations:**
- CSV-like format with single comma
- Unit directly attached to weight (no space): `20.7g`
- Continuous streaming protocol
- Stability transitions: ST ↔ US during weight changes
- Right-aligned weight values with variable padding

---

### 9. TScaleQHW (T&T Weight Scale - QHW Model)
**Log Files:**
- `TScaleQHW/QHW.txt` - Hex dump format
- `TScaleQHW/QHW2.txt` - Hex dump format

**Protocol Characteristics:**
- **Output Format:** `STATUS,MODE,[spaces][weight] unit\r\n`
- **Examples:**
  - `ST,GS,   245.6 g` (Stable, 245.6 grams)
  - `US,GS,    92.6 g` (Unstable, 92.6 grams)

**Field Breakdown:**
- **Status:** 2 characters
  - `ST` = Stable
  - `US` = Unstable
- **Mode:** 2 characters (after first comma)
  - `GS` = Gross/Stable mode
- **Weight:** Variable width, right-aligned with leading spaces (1 decimal place)
- **Unit:** 1-2 characters, space-separated from weight (typically "g")
- **Terminator:** `\r\n` (0x0D 0x0A)

**Hex Format:**
```
53 54 2C 47 53 2C 20 20 20 32 34 35 2E 36 20 67 0D 0A
"ST,GS,   245.6 g\r\n"
```

**Key Protocol Feature:** Double comma separator `ST,GS,` (note: two commas)

**Weight Range Observed:**
- QHW.log: Consistently 245.5-245.6g
- QHW2.log: 7.9g to 106.2g

**Key Observations:**
- CSV-like format with double comma (key difference from NHB)
- Unit space-separated from weight: `245.6 g`
- Continuous streaming protocol
- Stability transitions: ST ↔ US during weight changes
- Right-aligned weight values with variable padding
- Simpler parsing due to space-separated unit

---

## Common Protocol Patterns

### 1. **Line Terminators**
- Most devices use: `\r\n` (CR LF, hex: `0D 0A`)
- Consistent across different manufacturers

### 2. **Stability Indicators**
Multiple methods used:
- **Prefix/Suffix symbols:** `?` indicates unstable
- **Numeric index:** `/0` to `/8` scale
- **Status letters:** `N` (Net), `G` (Gross)

### 3. **Number Formatting**
- **Fixed-width:** Spaces for alignment
- **Right-aligned:** Weight values
- **Decimal precision:** Varies by device capability
  - High precision: 0.0001 g (Mettler)
  - Standard: 0.005 kg or 0.1 g
  - Industrial: 0.1 kg

### 4. **Units**
- `kg` - Kilograms (DEFENDER3000, Weight SPUN)
- `g` - Grams (MS204TS00, Weight QA)
- `pH` - pH units (PH Meter)

### 5. **Communication Modes**
- **Continuous Streaming:** Weight scales send continuous updates
- **Command-Response:** JIK6CAB requires command to get data
- **On-Demand:** Mettler responds to queries

---

## Implementation Implications

### Parsing Requirements
1. **Fixed-width text parsing** for most weight scales
2. **Regex patterns** for extracting numbers with signs
3. **Status character** extraction and interpretation
4. **Multi-line buffering** for complex protocols (JIK6CAB, TFO1)
5. **Binary data handling** for TFO1 custom encoding

### Communication Handling
1. **Serial port settings** (likely 9600-19200 baud, 8N1)
2. **Continuous read** for streaming devices
3. **Command send/response** for query-based devices
4. **Timeout handling** for device response
5. **Buffer management** for multi-line responses

### Data Validation
1. **Stability checking** before accepting reading
2. **Range validation** per device specs
3. **Checksum/validation** where applicable
4. **Unit consistency** checking

### User Interface Needs
1. **Real-time display** of weight values
2. **Stability indicator** visualization
3. **Multiple units** support (kg/g conversion)
4. **Timestamp** recording for logging
5. **Trend visualization** for dynamic weighing

---

## Device Comparison Table

| Device | Format | Precision | Update Rate | Complexity | Stability Method |
|--------|--------|-----------|-------------|------------|------------------|
| DEFENDER3000 | Simple Text | 0.005 kg | Continuous | Low | `?` prefix |
| JIK6CAB | Multi-line | 0.01 kg | On-demand | High | Status code |
| MS204TS00 | Simple Text | 0.0001 g | On-query | Low | Status char |
| PH Meter | Text+Temp | 0.01 pH | Continuous | Medium | Auto |
| TFO1 | Binary+ASCII | Variable | Continuous | Very High | Status byte |
| Weight QA | Text+Index | 0.01 g | Continuous | Medium | `/0-8` index |
| Weight SPUN | Simple Text | 0.1 kg | Continuous | Low | `?` prefix |
| TScaleNHB | CSV-like | 0.1 g | Continuous | Low | `ST/US` prefix |
| TScaleQHW | CSV-like | 0.1 g | Continuous | Low | `ST/US` prefix |

---

## Notes

- All log files include both stable and unstable readings showing device behavior
- Hex dumps are invaluable for understanding exact byte sequences
- Special characters (°, ô, ó, ò) may have encoding issues (likely CP-437 or similar)
- Some devices include photos showing physical hardware setup
- Video file for Weight SPUN likely shows dynamic weighing in action

---

## Next Steps for Code Analysis

1. Compare log protocols with C# implementation in `NLib.Serial.Devices/Serial/*.cs`
2. Verify parsing logic matches actual device output
3. Document any protocol extensions or customizations
4. Create test cases based on real log data
5. Ensure all edge cases (unstable, overflow, errors) are handled
