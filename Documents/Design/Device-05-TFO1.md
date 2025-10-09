# Device Implementation: TFO1

**Device Type:** Industrial System
**Complexity:** ⭐⭐⭐⭐⭐ Very Complex
**Protocol:** Mixed binary/ASCII with field identifiers
**File:** `TFO1.cs`

---

## Overview

Industrial system with mixed binary/ASCII protocol and field identifiers. Uses single-character headers to identify field types, with custom encoding for dates and binary values.

### Protocol Specification

**Format:** Field-based with single char identifiers

**Field Identifiers:**
- `F`, `H`, `Q`, `X`, `A` = Decimal values (9 bytes)
- `0`, `4`, `1` = Weight decimals (9 bytes)
- `2` = Weight integer (8 bytes)
- `B` = Binary byte (1 byte)
- `C` = Complex date (26 bytes)
- `V` = Version byte (1 byte)

**Example:**
```
F      0.0\r
H      0.0\r
Q      0.0\r
X      0.0\r
A    366.0\r
0     23.0\r
4    343.5\r
1      0.0\r
2        0\r
B[0x83]\r
C20[0xF4] 02[0xF3] 2023[0xF2] MON 09:20AM\r
V[0x31]\r\n
```

**Special Bytes:**
- `0xF4` = Date separator after day
- `0xF3` = Date separator after month
- `0xF2` = Date separator after year

**Terminator:** Each field ends with `\r`, package ends with `\r\n`

---

## Class Diagram

```mermaid
classDiagram
    class SerialDeviceData {
        <<abstract>>
        +ToByteArray() byte[]
        +Parse(byte[]) void
        +Raise(propertyName) void
    }

    class TFO1Data {
        -decimal _F, _H, _Q, _X, _A
        -decimal _W0, _W4, _W1
        -int _W2
        -byte _B
        -DateTime _C
        -byte _V
        +decimal F, H, Q, X, A
        +decimal W0, W4, W1
        +int W2
        +byte B
        +DateTime C
        +byte V
        +ToByteArray() byte[]
        +Parse(byte[]) void
    }

    class SerialDeviceTerminal {
        <<abstract>>
        #ProcessRXQueue() void
        +Connect() void
        +Disconnect() void
        +Value T
    }

    class TFO1Terminal {
        -ExtractPackage() byte[]
        -UpdateValue(byte[]) void
        -UpdateValues(byte[][]) void
        #ProcessRXQueue() void
        +DeviceName string
    }

    SerialDeviceData <|-- TFO1Data
    SerialDeviceTerminal <|-- TFO1Terminal
    TFO1Terminal --> TFO1Data : uses
```

---

## Data Class Properties

### TFO1Data

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `F` | decimal | 0 | Field F value |
| `H` | decimal | 0 | Field H value |
| `Q` | decimal | 0 | Field Q value |
| `X` | decimal | 0 | Field X value |
| `A` | decimal | 0 | Field A value |
| `W0` | decimal | 0 | Weight 0 value |
| `W4` | decimal | 0 | Weight 4 value |
| `W1` | decimal | 0 | Weight 1 value |
| `W2` | int | 0 | Weight 2 value (integer) |
| `B` | byte | 0 | Binary status byte |
| `C` | DateTime | DateTime.Now | Complex date/time |
| `V` | byte | 0 | Version byte |

---

## Flowchart - Field Identifier Switching

```mermaid
flowchart TD
    Start([UpdateValue Called]) --> CheckLength{Content<br/>length > 0?}
    CheckLength -->|No| End([Return])
    CheckLength -->|Yes| GetHeader[hdr = content 0]

    GetHeader --> Switch{Switch on<br/>header char}

    Switch -->|F| CheckF{Length >= 11?}
    Switch -->|H| CheckH{Length >= 11?}
    Switch -->|Q| CheckQ{Length >= 11?}
    Switch -->|X| CheckX{Length >= 11?}
    Switch -->|A| CheckA{Length >= 11?}
    Switch -->|0| Check0{Length >= 11?}
    Switch -->|4| Check4{Length >= 11?}
    Switch -->|1| Check1{Length >= 11?}
    Switch -->|2| Check2{Length >= 10?}
    Switch -->|B| CheckB{Length >= 3?}
    Switch -->|C| CheckC{Length >= 27?}
    Switch -->|V| CheckV{Length >= 4?}
    Switch -->|Other| End

    CheckF -->|Yes| ParseF[GetString 1 to 9<br/>Parse decimal]
    CheckF -->|No| End
    ParseF --> SetF[Value.F = parsed]
    SetF --> End

    CheckH -->|Yes| ParseH[GetString 1 to 9<br/>Parse decimal]
    CheckH -->|No| End
    ParseH --> SetH[Value.H = parsed]
    SetH --> End

    CheckQ -->|Yes| ParseQ[GetString 1 to 9<br/>Parse decimal]
    CheckQ -->|No| End
    ParseQ --> SetQ[Value.Q = parsed]
    SetQ --> End

    CheckX -->|Yes| ParseX[GetString 1 to 9<br/>Parse decimal]
    CheckX -->|No| End
    ParseX --> SetX[Value.X = parsed]
    SetX --> End

    CheckA -->|Yes| ParseA[GetString 1 to 9<br/>Parse decimal]
    CheckA -->|No| End
    ParseA --> SetA[Value.A = parsed]
    SetA --> End

    Check0 -->|Yes| Parse0[GetString 1 to 9<br/>Parse decimal]
    Check0 -->|No| End
    Parse0 --> SetW0[Value.W0 = parsed]
    SetW0 --> End

    Check4 -->|Yes| Parse4[GetString 1 to 9<br/>Parse decimal]
    Check4 -->|No| End
    Parse4 --> SetW4[Value.W4 = parsed]
    SetW4 --> End

    Check1 -->|Yes| Parse1[GetString 1 to 9<br/>Parse decimal]
    Check1 -->|No| End
    Parse1 --> SetW1[Value.W1 = parsed]
    SetW1 --> End

    Check2 -->|Yes| Parse2[GetString 1 to 8<br/>Parse integer]
    Check2 -->|No| End
    Parse2 --> SetW2[Value.W2 = parsed]
    SetW2 --> End

    CheckB -->|Yes| ParseB[Get byte at index 1]
    CheckB -->|No| End
    ParseB --> SetB[Value.B = byte]
    SetB --> End

    CheckC -->|Yes| ParseC[Parse complex date]
    CheckC -->|No| End
    ParseC --> ExtractDD[dd = GetString 1 to 2]
    ExtractDD --> ExtractMM[mm = GetString 5 to 2]
    ExtractMM --> ExtractYYYY[yyyy = GetString 9 to 4]
    ExtractYYYY --> ExtractHH[hh = GetString 19 to 2]
    ExtractHH --> ExtractMI[mi = GetString 22 to 2]
    ExtractMI --> ExtractAMPM[ampm = GetString 24 to 2]
    ExtractAMPM --> CheckAMPM{ampm == PM?}
    CheckAMPM -->|Yes| AddHours[hh += 12]
    CheckAMPM -->|No| SkipAdd[No adjustment]
    AddHours --> CreateDate[Create DateTime]
    SkipAdd --> CreateDate
    CreateDate --> SetC[Value.C = dateTime]
    SetC --> End

    CheckV -->|Yes| ParseV[Get byte at index 1]
    CheckV -->|No| End
    ParseV --> SetV[Value.V = byte]
    SetV --> End

    style Start fill:#e1f5ff
    style End fill:#e1f5ff
    style Switch fill:#fff4e6
    style ParseC fill:#ffe0b2
    style CheckAMPM fill:#fff4e6
```

---

## Sequence Diagram - Multi-Field Package

```mermaid
sequenceDiagram
    participant Device as TFO1 Device
    participant Terminal as TFO1Terminal
    participant Parser as UpdateValue
    participant Data as TFO1Data

    Note over Device: Sending complete package<br/>with all fields

    Device->>Terminal: "F      0.0\r"
    Terminal->>Parser: UpdateValue(bytes)
    Parser->>Parser: hdr = F, len = 11
    Parser->>Parser: GetString(1, 9) = "      0.0"
    Parser->>Data: Value.F = 0.0

    Device->>Terminal: "A    366.0\r"
    Terminal->>Parser: UpdateValue(bytes)
    Parser->>Parser: hdr = A, len = 11
    Parser->>Parser: GetString(1, 9) = "    366.0"
    Parser->>Data: Value.A = 366.0

    Device->>Terminal: "0     23.0\r"
    Terminal->>Parser: UpdateValue(bytes)
    Parser->>Parser: hdr = 0, len = 11
    Parser->>Parser: GetString(1, 9) = "     23.0"
    Parser->>Data: Value.W0 = 23.0

    Device->>Terminal: "4    343.5\r"
    Terminal->>Parser: UpdateValue(bytes)
    Parser->>Parser: hdr = 4, len = 11
    Parser->>Parser: GetString(1, 9) = "    343.5"
    Parser->>Data: Value.W4 = 343.5

    Device->>Terminal: "2        0\r"
    Terminal->>Parser: UpdateValue(bytes)
    Parser->>Parser: hdr = 2, len = 10
    Parser->>Parser: GetString(1, 8) = "       0"
    Parser->>Data: Value.W2 = 0

    Device->>Terminal: "B[0x83]\r"
    Terminal->>Parser: UpdateValue(bytes)
    Parser->>Parser: hdr = B, len = 3
    Parser->>Parser: Get byte[1] = 0x83
    Parser->>Data: Value.B = 0x83

    Device->>Terminal: "C20[0xF4] 02[0xF3] 2023[0xF2] MON 09:20AM\r"
    Terminal->>Parser: UpdateValue(bytes)
    Parser->>Parser: hdr = C, len = 27
    Parser->>Parser: Extract: dd=20, mm=02, yyyy=2023
    Parser->>Parser: Extract: hh=09, mi=20, ampm=AM
    Parser->>Parser: Check AM/PM (BUG: adds 12 for AM)
    Parser->>Data: Value.C = DateTime(2023,02,20,21,20,0)

    Device->>Terminal: "V[0x31]\r\n"
    Terminal->>Parser: UpdateValue(bytes)
    Parser->>Parser: hdr = V, len = 4
    Parser->>Parser: Get byte[1] = 0x31
    Parser->>Data: Value.V = 0x31

    Note over Data: All fields updated<br/>PropertyChanged events raised
```

---

## State Diagram - Field Processing

```mermaid
stateDiagram-v2
    [*] --> WaitingForField

    WaitingForField --> ReceivingField: Byte received

    state ReceivingField {
        [*] --> ReadHeader
        ReadHeader --> DetermineType: Header char read

        state DetermineType {
            [*] --> CheckFieldType
            CheckFieldType --> NumericField: F/H/Q/X/A/0-4
            CheckFieldType --> BinaryField: B/V
            CheckFieldType --> DateField: C
        }

        state NumericField {
            [*] --> ReadValue
            ReadValue: Read 9 ASCII chars
            ReadValue --> ParseDecimal
            ParseDecimal --> StoreValue
        }

        state BinaryField {
            [*] --> ReadByte
            ReadByte: Read 1 binary byte
            ReadByte --> StoreValue
        }

        state DateField {
            [*] --> ReadComponents
            ReadComponents: Read date parts
            ReadComponents --> ParseDateTime
            ParseDateTime --> StoreValue
        }
    }

    ReceivingField --> FieldComplete: CR received
    FieldComplete --> WaitingForField: Ready for next field

    FieldComplete --> PackageComplete: LF after CR
    PackageComplete --> [*]
```

---

## Implementation Details

### Key Parsing Method

```csharp
private void UpdateValue(byte[] content)
{
    if (content == null || content.Length == 0) return;

    char hdr = (char)content[0];

    switch (hdr)
    {
        case 'F':
            if (content.Length >= 11)
            {
                string str = Encoding.ASCII.GetString(content, 1, 9);
                Value.F = decimal.Parse(str.Trim());
            }
            break;

        case 'A':
            if (content.Length >= 11)
            {
                string str = Encoding.ASCII.GetString(content, 1, 9);
                Value.A = decimal.Parse(str.Trim());
            }
            break;

        case '0':
            if (content.Length >= 11)
            {
                string str = Encoding.ASCII.GetString(content, 1, 9);
                Value.W0 = decimal.Parse(str.Trim());
            }
            break;

        case 'B':
            if (content.Length >= 3)
            {
                Value.B = content[1];
            }
            break;

        case 'C':
            if (content.Length >= 27)
            {
                ParseComplexDate(content);
            }
            break;

        // ... other cases
    }
}
```

### Complex Date Parsing

```csharp
private void ParseComplexDate(byte[] content)
{
    // C20[0xF4] 02[0xF3] 2023[0xF2] MON 09:20AM\r
    // Position: 0123456789012345678901234567

    string dd = Encoding.ASCII.GetString(content, 1, 2);   // pos 1-2
    string mm = Encoding.ASCII.GetString(content, 5, 2);   // pos 5-6
    string yyyy = Encoding.ASCII.GetString(content, 9, 4); // pos 9-12
    string hh = Encoding.ASCII.GetString(content, 19, 2);  // pos 19-20
    string mi = Encoding.ASCII.GetString(content, 22, 2);  // pos 22-23
    string ampm = Encoding.ASCII.GetString(content, 24, 2); // pos 24-25

    int day = int.Parse(dd);
    int month = int.Parse(mm);
    int year = int.Parse(yyyy);
    int hour = int.Parse(hh);
    int minute = int.Parse(mi);

    // BUG: Original code has inverted AM/PM logic
    // if (ampm.ToUpper() == "AM") hour += 12; // WRONG!
    // Correct implementation should be:
    if (ampm.ToUpper() == "PM" && hour < 12)
        hour += 12;
    else if (ampm.ToUpper() == "AM" && hour == 12)
        hour = 0;

    Value.C = new DateTime(year, month, day, hour, minute, 0);
}
```

**KNOWN BUG:** The original implementation has inverted AM/PM logic. It adds 12 hours for AM instead of PM. This should be fixed in production code.

---

## Usage Example

### Emulator (Sending Data)
```csharp
var emulator = TFO1Device.Instance;
emulator.LoadConfig();
emulator.Start();

// Simulate complete package
emulator.Value.F = 0.0m;
emulator.Value.A = 366.0m;
emulator.Value.W0 = 23.0m;
emulator.Value.W4 = 343.5m;
emulator.Value.W2 = 0;
emulator.Value.B = 0x83;
emulator.Value.C = DateTime.Now;
emulator.Value.V = 0x31;
byte[] data = emulator.Value.ToByteArray();
// Automatically transmitted
```

### Terminal (Receiving Data)
```csharp
var terminal = TFO1Terminal.Instance;
terminal.LoadConfig();
terminal.Connect();

// Listen for field updates
terminal.OnRx += (s, e) => {
    Console.WriteLine($"F: {terminal.Value.F}");
    Console.WriteLine($"A: {terminal.Value.A}");
    Console.WriteLine($"W0: {terminal.Value.W0}");
    Console.WriteLine($"W4: {terminal.Value.W4}");
    Console.WriteLine($"W2: {terminal.Value.W2}");
    Console.WriteLine($"B: 0x{terminal.Value.B:X2}");
    Console.WriteLine($"Date: {terminal.Value.C:yyyy-MM-dd HH:mm}");
    Console.WriteLine($"V: 0x{terminal.Value.V:X2}");
};
```

---

## Protocol Examples

### Complete Package
```
F      0.0\r
H      0.0\r
Q      0.0\r
X      0.0\r
A    366.0\r
0     23.0\r
4    343.5\r
1      0.0\r
2        0\r
B[0x83]\r
C20[0xF4] 02[0xF3] 2023[0xF2] MON 09:20AM\r
V[0x31]\r\n
```

### Date Format Details
```
C20[0xF4] 02[0xF3] 2023[0xF2] MON 09:20AM\r
 ^^        ^^        ^^^^        ^^  ^^  ^^
 day       month     year        DOW hr  min ampm
```

---

## Testing Notes

- **Field Order:** Fields can arrive in any order
- **Missing Fields:** Not all fields are always present
- **Binary Bytes:** Bytes B and V are binary, not ASCII
- **Date Separators:** Special bytes 0xF4, 0xF3, 0xF2 are critical
- **AM/PM Bug:** Be aware of the inverted AM/PM logic bug
- **Precision:** Decimal fields have variable precision
- **Thread Safety:** Process fields sequentially, not in parallel

---

## Known Issues

### AM/PM Logic Bug
The original implementation incorrectly adds 12 hours for AM times:
```csharp
// WRONG:
if (ampm.ToUpper() == "AM") hour += 12;

// CORRECT:
if (ampm.ToUpper() == "PM" && hour < 12)
    hour += 12;
else if (ampm.ToUpper() == "AM" && hour == 12)
    hour = 0;
```

This bug causes:
- 09:20 AM → interpreted as 21:20 (9:20 PM)
- 03:45 PM → interpreted as 03:45 (3:45 AM)

**Recommendation:** Fix this bug in the next release.

---

## HEX Dump from Log Files

Raw serial data captured from the TFO1 device. This data was captured using third-party serial monitoring tools and serves as reference for protocol implementation.

**Source:** `Documents/LuckyTex Devices/TFO1/TFO.Weight.Attemp.02.HEX.txt`

### Sample Data Package

**ASCII/Binary Mixed Format:**
```
// F      0.0.
46 20 20 20 20 20 20 30 2E 30 0D
// H      0.0.
48 20 20 20 20 20 20 30 2E 30 0D
// Q      0.0.
51 20 20 20 20 20 20 30 2E 30 0D
// X      0.0.
58 20 20 20 20 20 20 30 2E 30 0D
// A    366.0.
41 20 20 20 20 33 36 36 2E 30 0D
// 0    23.0.
30 20 20 20 20 20 32 33 2E 30 0D
// 4    343.5.
34 20 20 20 20 33 34 33 2E 35 0D
// 1      0.0.
31 20 20 20 20 20 20 30 2E 30 0D
// 2       0.
32 20 20 20 20 20 20 20 30 0D
// B..
42 83 0D
// C20. 02. 2023. MON 09:20AM.
43 32 30 F4 20 30 32 F3 20 32 30 32 33 F2 20 4D 4F 4E 20 30 39 3A 32 30 41 4D 0D
// V1..
56 31 0D 0A
```

### Field-by-Field Breakdown

**F Field (F - Fabric Weight):**
```
46                "F" - field identifier
20 20 20 20 20 20 spaces (padding)
30 2E 30          "0.0" - value
0D                CR terminator
```

**H Field (H - Unknown):**
```
48                "H" - field identifier
20 20 20 20 20 20 spaces (padding)
30 2E 30          "0.0" - value
0D                CR terminator
```

**Q Field (Q - Unknown):**
```
51                "Q" - field identifier
20 20 20 20 20 20 spaces (padding)
30 2E 30          "0.0" - value
0D                CR terminator
```

**X Field (X - Unknown):**
```
58                "X" - field identifier
20 20 20 20 20 20 spaces (padding)
30 2E 30          "0.0" - value
0D                CR terminator
```

**A Field (A - Total Weight):**
```
41                "A" - field identifier
20 20 20 20       spaces (padding)
33 36 36 2E 30    "366.0" - value
0D                CR terminator
```

**0 Field (0 - Tare Weight):**
```
30                "0" - field identifier
20 20 20 20 20    spaces (padding)
32 33 2E 30       "23.0" - value
0D                CR terminator
```

**4 Field (4 - Net Weight):**
```
34                "4" - field identifier
20 20 20 20       spaces (padding)
33 34 33 2E 35    "343.5" - value
0D                CR terminator
```

**1 Field (1 - Unknown):**
```
31                "1" - field identifier
20 20 20 20 20 20 spaces (padding)
30 2E 30          "0.0" - value
0D                CR terminator
```

**2 Field (2 - Count):**
```
32                "2" - field identifier
20 20 20 20 20 20 20 spaces (padding)
30                "0" - value (no decimal)
0D                CR terminator
```

**B Field (B - Binary Status):**
```
42                "B" - field identifier
83                binary byte (status/mode indicator)
0D                CR terminator
```

**C Field (C - Date/Time):**
```
43                "C" - field identifier
32 30             "20" - day
F4                special separator byte (date separator 1)
20                space
30 32             "02" - month
F3                special separator byte (date separator 2)
20                space
32 30 32 33       "2023" - year
F2                special separator byte (date separator 3)
20                space
4D 4F 4E          "MON" - day of week
20                space
30 39 3A 32 30    "09:20" - time HH:mm
41 4D             "AM" - AM/PM indicator
0D                CR terminator
```

**V Field (V - Version/Binary):**
```
56                "V" - field identifier
31                binary byte (version number?)
0D 0A             CR+LF terminator
```

### Special Bytes Analysis

**Date Separators:**
- `0xF4` - Used between day and month
- `0xF3` - Used between month and year
- `0xF2` - Used between year and day-of-week

These are non-ASCII special characters that act as field delimiters in the date string.

**Binary Fields:**
- Field `B`: Second byte (0x83) is binary status
- Field `V`: Second byte (0x31 = ASCII '1') appears to be version

### Protocol Observations from Logs

1. **Variable Field Order:** Fields can arrive in any sequence
2. **Single Character IDs:** Each field identified by one ASCII character
3. **Mixed Termination:** Most fields use CR (0x0D), some use CR+LF (0x0D 0x0A)
4. **Special Separators:** Date field uses non-ASCII bytes 0xF2, 0xF3, 0xF4
5. **Variable Padding:** Field values have different padding lengths
6. **Decimal Precision:** Some fields use decimals (366.0), others don't (0)
7. **Binary Data:** Fields B and V contain non-ASCII binary bytes
8. **Total Length:** Approximately 150-200 bytes per complete package
9. **No Start/End Markers:** No explicit package boundary markers

### Field Mapping

| Field ID | ASCII | Meaning | Example Value | Format |
|----------|-------|---------|---------------|--------|
| F | 0x46 | Fabric Weight | 0.0 | Decimal |
| H | 0x48 | Unknown | 0.0 | Decimal |
| Q | 0x51 | Unknown | 0.0 | Decimal |
| X | 0x58 | Unknown | 0.0 | Decimal |
| A | 0x41 | Total Weight | 366.0 | Decimal |
| 0 | 0x30 | Tare Weight | 23.0 | Decimal |
| 4 | 0x34 | Net Weight | 343.5 | Decimal |
| 1 | 0x31 | Unknown | 0.0 | Decimal |
| 2 | 0x32 | Count | 0 | Integer |
| B | 0x42 | Status (Binary) | 0x83 | Binary |
| C | 0x43 | Date/Time | 20·02·2023·MON 09:20AM | Special |
| V | 0x56 | Version (Binary) | 0x31 | Binary |

### Parsing Strategy

1. **Line-by-Line Processing:** Split on CR/LF
2. **Field Identification:** First byte determines field type
3. **Value Extraction:** Remaining bytes (after padding) are the value
4. **Special Handling:** Date field requires separator byte processing
5. **State Accumulation:** Collect all fields until package complete
6. **Validation:** Verify critical fields (A, 0, 4, C) are present

### Parsing Challenges

1. **No Package Boundaries:** Must detect completion by field presence
2. **Variable Order:** Cannot rely on sequence
3. **Binary in ASCII Stream:** Must handle binary bytes in B and V fields
4. **Special Date Format:** Non-standard separators require byte-level parsing
5. **AM/PM Bug:** Implementation has inverted AM/PM logic (see Known Issues)

---

## Related Files

- **Data Class:** `NLib.Serial.Devices.TFO1Data`
- **Emulator:** `NLib.Serial.Emulators.TFO1Device`
- **Terminal:** `NLib.Serial.Terminals.TFO1Terminal`
- **Log Reference:** `Documents/LuckyTex Devices/TFO1/`

---

## See Also

- [Device Comparison](CODE_ANALYSIS_NLib.Serial.Devices.md#device-implementations)
- [Base Classes](CODE_ANALYSIS_NLib.Serial.Devices.md#base-class-framework)
- [JIK6CAB Device](Device-02-JIK6CAB.md) - Another complex multi-field device
