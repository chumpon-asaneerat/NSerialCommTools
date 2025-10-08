# Device Implementation: JIK6CAB

**Device Type:** Weight Scale
**Complexity:** ⭐⭐⭐⭐ Complex
**Protocol:** Multi-line structured package with state machine parser
**File:** `JIK6CAB.cs`

---

## Overview

Complex multi-line weight scale with package markers and state machine parser. Sends structured 14-line packages with date, time, and multiple weight measurements.

### Protocol Specification

**Format:** 14-line structured package

**Markers:**
- **Start:** `^KJIK000`
- **End:** `~P1`

**Example Package:**
```
^KJIK000
2023-11-07
17:19:38
  0.00 kg
  1.94 kg
  0.00 kg
  0.00 kg
  1.94 kg
  1.94 kg
    0 pcs
(empty line)
E
(empty line)
~P1
```

**Field Description:**
- **Line 1:** Start marker with device ID
- **Line 2:** Date (YYYY-MM-DD format)
- **Line 3:** Time (HH:mm:ss format)
- **Line 4:** Tare Weight (TW)
- **Line 5:** Gross Weight (GW)
- **Lines 6-7:** Unknown values (typically 0)
- **Line 8:** Net Weight (NW)
- **Line 9:** Display weight (duplicate)
- **Line 10:** Piece count (pcs)
- **Lines 11-13:** Empty or status characters
- **Line 14:** End marker

**Update Rate:** On-demand (when weight is captured)

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

    class JIK6CABData {
        -DateTime _dt
        -decimal _TW
        -decimal _NW
        -decimal _GW
        -decimal _PCS
        -string _TUnit
        -string _NUnit
        -string _GUnit
        +DateTime Date
        +decimal TW
        +decimal NW
        +decimal GW
        +decimal PCS
        +string TUnit
        +string NUnit
        +string GUnit
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

    class JIK6CABTerminal {
        -bool bCompleted
        -DateTime? date
        -decimal? tw
        -decimal? nw
        -decimal? gw
        -decimal? pcs
        -string tu, nu, gu
        -ExtractPackage() byte[]
        -UpdateValue(byte[]) void
        -UpdateValues(byte[][]) void
        #ProcessRXQueue() void
        +DeviceName string
    }

    SerialDeviceData <|-- JIK6CABData
    SerialDeviceTerminal <|-- JIK6CABTerminal
    JIK6CABTerminal --> JIK6CABData : uses
```

---

## Data Class Properties

### JIK6CABData

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Date` | DateTime | DateTime.Now | Date and time of measurement |
| `TW` | decimal | 0 | Tare Weight |
| `NW` | decimal | 0 | Net Weight |
| `GW` | decimal | 0 | Gross Weight |
| `PCS` | decimal | 0 | Piece count |
| `TUnit` | string | "kg" | Tare weight unit |
| `NUnit` | string | "kg" | Net weight unit |
| `GUnit` | string | "kg" | Gross weight unit |

---

## State Diagram - Parsing State Machine

```mermaid
stateDiagram-v2
    [*] --> Idle

    state Idle {
        [*] --> WaitingForStart
        WaitingForStart: bCompleted = true
    }

    Idle --> PackageStarted: Detect KJIK

    state PackageStarted {
        [*] --> ClearVariables
        ClearVariables: Set bCompleted = false
        ClearVariables: Clear date, tw, nw, gw, pcs
        ClearVariables --> Ready
    }

    PackageStarted --> Parsing

    state Parsing {
        [*] --> CheckLineType

        CheckLineType --> ParseDate: Contains - or /
        CheckLineType --> ParseTime: Contains :
        CheckLineType --> ParseWeight: Contains g or kg
        CheckLineType --> ParsePCS: Contains pcs
        CheckLineType --> CheckEnd: Contains P1

        ParseDate --> CheckLineType: date = parsed
        ParseTime --> CheckLineType: time = parsed

        state ParseWeight {
            [*] --> CheckWhichWeight
            CheckWhichWeight --> SetTW: not tw HasValue
            CheckWhichWeight --> SetGW: tw HasValue and not gw HasValue
            CheckWhichWeight --> SetNW: tw and gw set, not nw HasValue
        }

        ParseWeight --> CheckLineType
        ParsePCS --> CheckLineType: pcs = parsed
    }

    Parsing --> PackageComplete: Detect P1

    state PackageComplete {
        [*] --> AssignValues
        AssignValues: Value Date = date
        AssignValues: Value TW = tw
        AssignValues: Value GW = gw
        AssignValues: Value NW = nw
        AssignValues: Value PCS = pcs
        AssignValues --> SetCompleted
        SetCompleted: bCompleted = true
    }

    PackageComplete --> Idle: Ready for next package
```

---

## Sequence Diagram - Multi-Line Package Processing

```mermaid
sequenceDiagram
    participant Device as JIK6CAB Device
    participant Terminal as JIK6CABTerminal
    participant Parser as UpdateValue
    participant Data as JIK6CABData

    Note over Terminal: Initial State: bCompleted = true

    Device->>Terminal: Line 1: ^KJIK000
    Terminal->>Parser: UpdateValue(line)
    Parser->>Parser: Detect KJIK
    Parser->>Parser: Set bCompleted = false
    Parser->>Parser: Clear all temp variables

    Device->>Terminal: Line 2: 2023-11-07
    Terminal->>Parser: UpdateValue(line)
    Parser->>Parser: Contains -
    Parser->>Parser: Parse date format
    Parser->>Parser: date = 2023-11-07

    Device->>Terminal: Line 3: 17:19:38
    Terminal->>Parser: UpdateValue(line)
    Parser->>Parser: Contains :
    Parser->>Parser: Parse time format
    Parser->>Parser: Combine with date
    Parser->>Parser: date = 2023-11-07 17:19:38

    Device->>Terminal: Line 4: 0.00 kg
    Terminal->>Parser: UpdateValue(line)
    Parser->>Parser: Contains kg
    Parser->>Parser: Check: not tw HasValue
    Parser->>Parser: tw = 0.00, tu = kg

    Device->>Terminal: Line 5: 1.94 kg
    Terminal->>Parser: UpdateValue(line)
    Parser->>Parser: Contains kg
    Parser->>Parser: Check: tw set, not gw HasValue
    Parser->>Parser: gw = 1.94, gu = kg

    Note over Device,Terminal: Lines 6-7: Unknown values (0)

    Device->>Terminal: Line 8: 1.94 kg
    Terminal->>Parser: UpdateValue(line)
    Parser->>Parser: Contains kg
    Parser->>Parser: Check: tw and gw set, not nw HasValue
    Parser->>Parser: nw = 1.94, nu = kg

    Note over Device,Terminal: Line 9: Display weight (duplicate)

    Device->>Terminal: Line 10: 0 pcs
    Terminal->>Parser: UpdateValue(line)
    Parser->>Parser: Contains pcs
    Parser->>Parser: pcs = 0

    Note over Device,Terminal: Lines 11-13: Empty + E

    Device->>Terminal: Line 14: ~P1
    Terminal->>Parser: UpdateValue(line)
    Parser->>Parser: Detect P1
    Parser->>Data: Assign all values
    Data-->>Data: PropertyChanged events
    Parser->>Parser: Set bCompleted = true

    Note over Terminal: Ready for next package
```

---

## Flowchart - Line Type Detection

```mermaid
flowchart TD
    Start([UpdateValue Called]) --> CheckEmpty{String<br/>empty?}
    CheckEmpty -->|Yes| End([Return])
    CheckEmpty -->|No| CheckCompleted{bCompleted<br/>== true?}

    CheckCompleted -->|Yes| CheckKJIK{Contains<br/>KJIK?}
    CheckKJIK -->|Yes| StartPackage[Set bCompleted = false<br/>Clear all variables]
    StartPackage --> End
    CheckKJIK -->|No| End

    CheckCompleted -->|No| CheckType{Check line<br/>content}

    CheckType -->|Contains g| ParseWeightFlow[Parse Weight Flow]
    CheckType -->|Contains pcs| ParsePCS[Parse PCS value]
    CheckType -->|Contains - or /| ParseDate[Parse Date]
    CheckType -->|Contains :| ParseTime[Parse Time]
    CheckType -->|Contains P1| CompletePackage[Complete Package Flow]
    CheckType -->|Other| End

    ParseWeightFlow --> CheckTW{tw<br/>HasValue?}
    CheckTW -->|No| SetTW[Set TW + TUnit]
    CheckTW -->|Yes| CheckGW{gw<br/>HasValue?}
    CheckGW -->|No| SetGW[Set GW + GUnit]
    CheckGW -->|Yes| CheckNW{nw<br/>HasValue?}
    CheckNW -->|No| SetNW[Set NW + NUnit]
    CheckNW -->|Yes| End

    SetTW --> End
    SetGW --> End
    SetNW --> End
    ParsePCS --> End
    ParseDate --> End
    ParseTime --> End

    CompletePackage --> Assign[Assign all values to Value object]
    Assign --> SetCompleted[bCompleted = true]
    SetCompleted --> NotifyAll[Raise all PropertyChanged]
    NotifyAll --> End

    style Start fill:#e1f5ff
    style End fill:#e1f5ff
    style CheckType fill:#fff4e6
    style CompletePackage fill:#e8f5e9
    style NotifyAll fill:#e8f5e9
```

---

## Implementation Details

### State Machine Variables

```csharp
private bool bCompleted = true;
private DateTime? date = null;
private decimal? tw = null;
private decimal? nw = null;
private decimal? gw = null;
private decimal? pcs = null;
private string tu = "kg", nu = "kg", gu = "kg";
```

### Key Parsing Logic

The parser uses a state machine approach:

1. **Idle State:** `bCompleted = true`, waiting for start marker
2. **Start Detection:** When "KJIK" is found, clear all temp variables
3. **Line-by-Line Parsing:** Each line is examined for specific patterns
4. **Weight Assignment:** Weights are assigned in order (TW, GW, NW)
5. **Completion:** When "P1" is detected, assign all values and set `bCompleted = true`

### UpdateValue() Core Logic

```csharp
private void UpdateValue(byte[] content)
{
    string line = Encoding.ASCII.GetString(content).Trim();

    if (string.IsNullOrEmpty(line)) return;

    if (bCompleted)
    {
        // Looking for package start
        if (line.Contains("KJIK"))
        {
            bCompleted = false;
            date = null;
            tw = nw = gw = pcs = null;
            tu = nu = gu = "kg";
        }
        return;
    }

    // Process line based on content
    if (line.Contains("g") || line.Contains("G"))
    {
        // Parse weight
        ParseWeight(line);
    }
    else if (line.Contains("pcs"))
    {
        // Parse piece count
        ParsePCS(line);
    }
    else if (line.Contains("-") || line.Contains("/"))
    {
        // Parse date
        ParseDate(line);
    }
    else if (line.Contains(":"))
    {
        // Parse time
        ParseTime(line);
    }
    else if (line.Contains("P1"))
    {
        // Complete package
        CompletePackage();
    }
}
```

---

## Usage Example

### Emulator (Sending Data)
```csharp
var emulator = JIK6CABDevice.Instance;
emulator.LoadConfig();
emulator.Start();

// Simulate complete weighing
emulator.Value.Date = DateTime.Now;
emulator.Value.TW = 0.00m;
emulator.Value.GW = 1.94m;
emulator.Value.NW = 1.94m;
emulator.Value.PCS = 0;
byte[] data = emulator.Value.ToByteArray();
// Automatically transmitted via background thread
```

### Terminal (Receiving Data)
```csharp
var terminal = JIK6CABTerminal.Instance;
terminal.LoadConfig();
terminal.Connect();

// Listen for complete package
terminal.OnRx += (s, e) => {
    Console.WriteLine($"Date/Time: {terminal.Value.Date}");
    Console.WriteLine($"Tare: {terminal.Value.TW} {terminal.Value.TUnit}");
    Console.WriteLine($"Gross: {terminal.Value.GW} {terminal.Value.GUnit}");
    Console.WriteLine($"Net: {terminal.Value.NW} {terminal.Value.NUnit}");
    Console.WriteLine($"Pieces: {terminal.Value.PCS}");
};
```

---

## Protocol Examples

### Complete Package
```
^KJIK000
2023-11-07
17:19:38
  0.00 kg    # Tare Weight
  1.94 kg    # Gross Weight
  0.00 kg    # Unknown
  0.00 kg    # Unknown
  1.94 kg    # Net Weight
  1.94 kg    # Display (duplicate)
    0 pcs    # Piece count
               # Empty line
E              # Status
               # Empty line
~P1            # End marker
```

---

## Testing Notes

- **State Machine:** Must receive complete 14-line package
- **Line Order:** Critical for correct weight assignment
- **Package Markers:** Both start and end markers must be present
- **Partial Packages:** Incomplete packages are discarded
- **Thread Safety:** State machine is not thread-safe, ensure single thread access

---

## Related Files

- **Data Class:** `NLib.Serial.Devices.JIK6CABData`
- **Emulator:** `NLib.Serial.Emulators.JIK6CABDevice`
- **Terminal:** `NLib.Serial.Terminals.JIK6CABTerminal`
- **Log Reference:** `Documents/LuckyTex Devices/JIK6CAB/`

---

## See Also

- [Device Comparison](CODE_ANALYSIS_NLib.Serial.Devices.md#device-implementations)
- [Base Classes](CODE_ANALYSIS_NLib.Serial.Devices.md#base-class-framework)
- [TFO1 Device](Device-05-TFO1.md) - Another complex multi-field device
