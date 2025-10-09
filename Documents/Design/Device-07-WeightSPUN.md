# Device Implementation: WeightSPUN

**Device Type:** Dynamic Weight Scale
**Complexity:** ⭐ Simple
**Protocol:** Single-line continuous streaming (similar to DEFENDER3000)
**File:** `WeightSPUN.cs`

---

## Overview

Dynamic weighing scale for spinning/moving loads, similar to DEFENDER3000 but with higher capacity. Used for weighing objects in motion or on rotating platforms.

### Protocol Specification

**Format:** `[spaces][weight] kg [spaces][status]\r\n`

**Example:**
```
    20.0 kg    G
```

**Field Description:**
- **Weight:** Variable length, right-aligned with spaces
- **Unit:** Typically "kg" for higher capacity
- **Status:** 4 characters, padded left
  - `G` = Gross weight (stable)
  - `N` = Net weight (stable)
  - `?G` = Gross weight (unstable/dynamic)
  - `?N` = Net weight (unstable/dynamic)
- **Terminator:** `\r\n` (0x0D 0x0A)

**Update Rate:** Continuous (high frequency during motion)
**Precision:** 0.1 kg
**Capacity:** 100+ kg (high capacity for industrial use)

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

    class WeightSPUNData {
        -decimal _W
        -string _Unit
        -string _O
        +decimal W
        +string Unit
        +string O
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

    class WeightSPUNTerminal {
        -ExtractPackage() byte[]
        -UpdateValue(byte[]) void
        -UpdateValues(byte[][]) void
        #ProcessRXQueue() void
        +DeviceName string
    }

    SerialDeviceData <|-- WeightSPUNData
    SerialDeviceTerminal <|-- WeightSPUNTerminal
    WeightSPUNTerminal --> WeightSPUNData : uses
```

---

## Data Class Properties

### WeightSPUNData

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `W` | decimal | 0 | Weight value |
| `Unit` | string | "kg" | Measurement unit |
| `O` | string | "G" | Status/Mode (G, N, ?G, ?N) |

---

## Flowchart - Parsing Logic (Same as DEFENDER3000)

```mermaid
flowchart TD
    Start([ProcessRXQueue Called]) --> Extract[Call ExtractPackage]
    Extract --> CheckNull{Package<br/>extracted?}
    CheckNull -->|No| End([Return])
    CheckNull -->|Yes| Split[Split by CRLF]

    Split --> Loop{For each<br/>line}
    Loop -->|Next line| Convert[Convert bytes to ASCII]
    Convert --> CheckEmpty{String<br/>empty?}
    CheckEmpty -->|Yes| Loop
    CheckEmpty -->|No| SplitSpaces[Split by spaces<br/>RemoveEmptyEntries]

    SplitSpaces --> CheckCount{Count >= 3?}
    CheckCount -->|No| Loop
    CheckCount -->|Yes| ParseWeight[Get element 0]

    ParseWeight --> TryCatch{Try Parse<br/>Decimal}
    TryCatch -->|Success| SetW[Value.W = weight]
    TryCatch -->|Exception| LogError[Log Error]
    LogError --> Loop

    SetW --> ParseUnit[Get element 1]
    ParseUnit --> SetUnit[Value.Unit = unit]
    SetUnit --> ParseStatus[Get element 2]
    ParseStatus --> SetStatus[Value.O = status]
    SetStatus --> Notify[Raise PropertyChanged]
    Notify --> Loop

    Loop -->|Done| End

    style Start fill:#e1f5ff
    style End fill:#e1f5ff
    style TryCatch fill:#fff4e6
    style LogError fill:#ffebee
    style Notify fill:#e8f5e9
```

---

## Sequence Diagram - Dynamic Weight Loading

```mermaid
sequenceDiagram
    participant Scale as WeightSPUN Scale
    participant Terminal as WeightSPUNTerminal
    participant Parser as UpdateValue
    participant Data as WeightSPUNData
    participant UI as User Interface

    Note over Scale,UI: Dynamic loading scenario

    Scale->>Terminal: "    19.8 kg    G\r\n"
    Terminal->>Parser: UpdateValue(bytes)
    Parser->>Data: Value.W = 19.8 (Tare)
    Data->>UI: Display: 19.8 kg Stable

    Note over Scale: Start adding weight...

    Scale->>Terminal: "    25.3 kg   ?G\r\n"
    Terminal->>Parser: UpdateValue(bytes)
    Parser->>Data: Value.W = 25.3, O = ?G
    Data->>UI: Display: 25.3 kg UNSTABLE

    Scale->>Terminal: "    45.7 kg   ?G\r\n"
    Terminal->>Parser: UpdateValue(bytes)
    Parser->>Data: Value.W = 45.7, O = ?G
    Data->>UI: Display: 45.7 kg UNSTABLE

    Scale->>Terminal: "    78.2 kg   ?G\r\n"
    Terminal->>Parser: UpdateValue(bytes)
    Parser->>Data: Value.W = 78.2, O = ?G
    Data->>UI: Display: 78.2 kg UNSTABLE

    Scale->>Terminal: "    94.6 kg   ?G\r\n"
    Terminal->>Parser: UpdateValue(bytes)
    Parser->>Data: Value.W = 94.6, O = ?G
    Data->>UI: Display: 94.6 kg UNSTABLE

    Note over Scale: Loading complete, stabilizing...

    Scale->>Terminal: "    91.3 kg   ?G\r\n"
    Terminal->>Parser: UpdateValue(bytes)
    Parser->>Data: Value.W = 91.3, O = ?G
    Data->>UI: Display: 91.3 kg UNSTABLE

    Scale->>Terminal: "    90.5 kg    G\r\n"
    Terminal->>Parser: UpdateValue(bytes)
    Parser->>Data: Value.W = 90.5, O = G
    Data->>UI: Display: 90.5 kg STABLE checkmark

    Note over UI: Final weight accepted
```

---

## State Diagram - Dynamic vs Static Weighing

```mermaid
stateDiagram-v2
    [*] --> Idle

    Idle --> Static: No movement
    Idle --> Dynamic: Movement detected

    state Static {
        [*] --> TareWeight
        TareWeight: Status = G (Stable)
        TareWeight: Weight = 19.8 kg

        TareWeight --> WaitingForLoad
        WaitingForLoad --> Loaded: Weight added
        Loaded: Status = G
        Loaded --> WaitingForLoad: Weight removed
    }

    state Dynamic {
        [*] --> Loading
        Loading: Status = ?G (Unstable)
        Loading: Weight increasing

        Loading --> Settling: Loading stopped
        Settling: Status = ?G
        Settling: Weight oscillating

        Settling --> Stabilized: Movement ceased
        Stabilized: Status = G
        Stabilized: Final weight
    }

    Static --> Dynamic: Movement starts
    Dynamic --> Static: Movement stops and stable

    Stabilized --> Recorded: User accepts weight
    Recorded --> [*]

    note right of Dynamic
        High capacity scale
        100+ kg range
        ?G indicates dynamic state
    end note

    note right of Static
        G indicates stable
        Ready for measurement
    end note
```

---

## Implementation Details

### Key Methods

#### ExtractPackage()
```csharp
private byte[] ExtractPackage()
{
    if (Queues == null || Queues.Count <= 0) return null;

    byte[] endPatterns = new byte[] { 0x0D, 0x0A }; // \r\n
    byte[] buffers = Queues.ToArray();

    int idx = IndexOf(buffers, endPatterns);
    if (idx != -1)
    {
        int len = idx + endPatterns.Length;
        byte[] package = new byte[len];
        Array.Copy(buffers, package, len);
        Queues.RemoveRange(0, len);
        return package;
    }
    return null;
}
```

#### UpdateValue()
```csharp
private void UpdateValue(byte[] content)
{
    string line = Encoding.ASCII.GetString(content);
    string[] elems = line.Split(new string[] { " " },
                                  StringSplitOptions.RemoveEmptyEntries);

    if (elems.Length < 3) return;

    try
    {
        Value.W = decimal.Parse(elems[0].Trim());
        Value.Unit = elems[1].Trim();
        Value.O = elems[2].Trim();
    }
    catch (Exception ex)
    {
        MethodBase.GetCurrentMethod().Err(ex);
    }
}
```

**Note:** Implementation is identical to CordDEFENDER3000, only difference is device capacity and typical usage scenario.

---

## Usage Example

### Emulator (Sending Data)
```csharp
var emulator = WeightSPUNDevice.Instance;
emulator.LoadConfig();
emulator.Start();

// Simulate dynamic weighing
emulator.Value.W = 45.7m;
emulator.Value.Unit = "kg";
emulator.Value.O = "?G"; // Unstable/dynamic
byte[] data = emulator.Value.ToByteArray();
// Automatically transmitted
```

### Terminal (Receiving Data)
```csharp
var terminal = WeightSPUNTerminal.Instance;
terminal.LoadConfig();
terminal.Connect();

// Listen for weight updates
terminal.OnRx += (s, e) => {
    Console.WriteLine($"Weight: {terminal.Value.W} {terminal.Value.Unit}");
    Console.WriteLine($"Status: {terminal.Value.O}");

    bool isStable = terminal.Value.O == "G" || terminal.Value.O == "N";
    bool isDynamic = terminal.Value.O.Contains("?");

    if (isStable)
        Console.WriteLine("STABLE - Ready to record");
    else if (isDynamic)
        Console.WriteLine("DYNAMIC - Weight changing");
};
```

### Dynamic Weighing Application
```csharp
private decimal maxWeight = 0;
private List<decimal> weightHistory = new List<decimal>();

terminal.OnRx += (s, e) => {
    decimal currentWeight = terminal.Value.W;
    bool isDynamic = terminal.Value.O.Contains("?");

    if (isDynamic)
    {
        // Track maximum weight during dynamic loading
        if (currentWeight > maxWeight)
            maxWeight = currentWeight;

        // Record weight history for analysis
        weightHistory.Add(currentWeight);

        UpdateDynamicChart(weightHistory);
    }
    else
    {
        // Stable reading - finalize
        Console.WriteLine($"Final Weight: {currentWeight} kg");
        Console.WriteLine($"Max During Loading: {maxWeight} kg");

        // Reset for next measurement
        maxWeight = 0;
        weightHistory.Clear();
    }
};
```

---

## Protocol Examples

### Static Weighing
```
    19.8 kg    G    # Tare, stable
    45.0 kg    G    # Loaded, stable
    19.8 kg    G    # Unloaded, stable
```

### Dynamic Weighing (Loading)
```
    19.8 kg    G    # Initial tare
    25.3 kg   ?G    # Adding weight (unstable)
    45.7 kg   ?G    # Still adding (unstable)
    78.2 kg   ?G    # Continue adding (unstable)
    94.6 kg   ?G    # Near complete (unstable)
    91.3 kg   ?G    # Settling (unstable)
    90.5 kg    G    # Stabilized - FINAL
```

### Spinning Platform
```
    15.0 kg    G    # Empty platform, stable
    85.0 kg   ?G    # Platform spinning with load
    87.0 kg   ?G    # Oscillating while spinning
    84.0 kg   ?G    # Still spinning
    86.0 kg   ?G    # Slowing down
    85.5 kg    G    # Stopped and stable
```

---

## Testing Notes

- **Dynamic Detection:** `?` prefix indicates unstable/moving weight
- **High Capacity:** Designed for 100+ kg loads
- **Update Frequency:** High during dynamic weighing
- **Peak Detection:** Application can track max weight during loading
- **Settling Time:** May take several seconds after motion stops
- **Environmental:** Sensitive to vibration and platform movement

---

## Comparison with CordDEFENDER3000

| Feature | CordDEFENDER3000 | WeightSPUN |
|---------|------------------|------------|
| Protocol | Identical | Identical |
| Capacity | Low (< 10 kg) | High (100+ kg) |
| Precision | 0.005 kg (5g) | 0.1 kg (100g) |
| Use Case | Small items | Large/dynamic loads |
| Typical Unit | kg | kg |
| Dynamic Range | Limited | Wide |

**Key Difference:** WeightSPUN is designed for industrial applications with heavier loads and more dynamic weighing scenarios (spinning platforms, moving conveyors, etc.).

---

## Application Scenarios

### 1. Spinning Platform
- Measure weight of objects on rotating platform
- Track weight changes during rotation
- Detect imbalance or load shift

### 2. Conveyor Belt
- Weigh items moving on conveyor
- High-speed weight capture
- Quality control in production line

### 3. Dynamic Loading
- Measure weight as material is being added
- Track loading rate
- Detect completion of loading

### 4. Bulk Material
- Weigh large quantities
- Handle weight oscillations
- Average multiple readings for accuracy

---

## HEX Dump from Log Files

Raw serial data captured from the WeightSPUN device. This data was captured using third-party serial monitoring tools and serves as reference for protocol implementation.

**Source:** `Documents/LuckyTex Devices/WEIGHT SPUN/Serial_Logspun.txt`

### Sample Data Format

**ASCII Text:**
```
     0.0 kg    G
```

**HEX Dump:**
```
20 20 20 20 20 30 2E 30 20 6B 67 20 20 20 20 47 0D 0A
```

### Byte-by-Byte Breakdown

```
20 20 20 20 20    5 spaces (leading padding)
30 2E 30          "0.0" - weight value
20                space separator
6B 67             "kg" - unit
20 20 20 20       4 spaces (trailing padding)
47                "G" - status indicator (Gross, stable)
0D 0A             CR+LF terminator
```

**Total Length:** 18 bytes per reading (same as DEFENDER3000)

### Protocol Analysis

**Identical to CordDEFENDER3000:**

The WeightSPUN uses the exact same protocol as the CordDEFENDER3000 scale:
- Same 18-byte format
- Same padding scheme
- Same status indicators (G, N, ?G, ?N)
- Same CR+LF termination

**Format:** `[5 spaces][weight] kg [4 spaces][status]\r\n`

### Log File Header Bytes

**Special Header (First 4 bytes):**
```
00 00 7F 7F    Special marker/header bytes
```

These bytes appear at the start of the log file but are not part of the weight transmission protocol. They may be:
- Log file markers added by the capture tool
- Device initialization sequence
- Synchronization bytes

**Following the header, normal weight readings begin:**
```
20 20 20 20 20 30 2E 30 20 6B 67 20 20 20 20 47 0D 0A     0.0 kg    G
20 20 20 20 20 30 2E 30 20 6B 67 20 20 20 20 47 0D 0A     0.0 kg    G
20 20 20 20 20 30 2E 30 20 6B 67 20 20 20 20 47 0D 0A     0.0 kg    G
```

### Example Continuous Stream

**Zero Weight (Stable):**
```
20 20 20 20 20 30 2E 30 20 6B 67 20 20 20 20 47 0D 0A
20 20 20 20 20 30 2E 30 20 6B 67 20 20 20 20 47 0D 0A
20 20 20 20 20 30 2E 30 20 6B 67 20 20 20 20 47 0D 0A
```

**ASCII:**
```
     0.0 kg    G
     0.0 kg    G
     0.0 kg    G
```

### Status Indicators

Same as CordDEFENDER3000:

| Hex | ASCII | Meaning |
|-----|-------|---------|
| 47 | G | Gross weight, stable |
| 4E | N | Net weight, stable |
| 3F 47 | ?G | Gross weight, unstable/dynamic |
| 3F 4E | ?N | Net weight, unstable/dynamic |

### Protocol Observations from Logs

1. **Identical Format:** Uses exact same protocol as DEFENDER3000
2. **Fixed Width:** Always 18 bytes per transmission
3. **Continuous Stream:** High-frequency updates during weighing
4. **Header Bytes:** Log file starts with `00 00 7F 7F` (tool artifact)
5. **Stability Indication:** Question mark (?) prefix for dynamic/unstable
6. **Unit Consistency:** Always reports in 'kg'

### Key Differences from DEFENDER3000

**Protocol Level:**
- No differences in serial protocol format
- Identical byte structure
- Same parsing logic applies

**Hardware/Application Level:**
- **Capacity:** WeightSPUN handles much heavier loads (100+ kg vs < 10 kg)
- **Precision:** Lower precision acceptable for large weights (0.1 kg vs 0.005 kg)
- **Environment:** Designed for dynamic/moving loads (spinning, conveyor)
- **Update Rate:** May transmit more frequently during motion

### Parsing Strategy

**Use same parser as CordDEFENDER3000:**

```csharp
// Identical implementation
string line = Encoding.ASCII.GetString(content);
string[] elems = line.Split(new string[] { " " },
                              StringSplitOptions.RemoveEmptyEntries);

if (elems.Length < 3) return;

Value.W = decimal.Parse(elems[0].Trim());      // Weight
Value.Unit = elems[1].Trim();                  // Unit (kg)
Value.O = elems[2].Trim();                     // Status (G/N/?G/?N)
```

### Industrial Application Example

**Spinning Platform Sequence:**
```
ASCII                    Status              Interpretation
     0.0 kg    G        Stable, zero        Empty platform
   125.3 kg   ?G        Unstable            Item placed, platform starting
   126.8 kg   ?G        Unstable            Spinning, weight shifting
   127.2 kg   ?G        Unstable            Still spinning
   127.5 kg   ?G        Unstable            Slowing down
   127.8 kg    G        Stable              Platform stopped, final weight
```

### Implementation Notes

**Code Reuse:**
- WeightSPUN can share the same data class as DEFENDER3000
- Same terminal implementation works for both
- Only difference is configuration/calibration

**Differentiation:**
- Use device name/model for identification
- Configure different ranges in application
- Apply appropriate validation rules for weight range

### Comparison Table: Protocol vs Application

| Aspect | CordDEFENDER3000 | WeightSPUN |
|--------|------------------|------------|
| **Protocol Format** | Identical | Identical |
| **Byte Structure** | 18 bytes | 18 bytes |
| **Parsing Logic** | Same | Same |
| **Code Reuse** | Yes | Yes |
| **Weight Range** | 0-10 kg | 0-200 kg |
| **Typical Precision** | ±5 g | ±100 g |
| **Use Case** | Laboratory/small items | Industrial/bulk |
| **Environment** | Stable surface | Dynamic/moving |

---

## Related Files

- **Data Class:** `NLib.Serial.Devices.WeightSPUNData`
- **Emulator:** `NLib.Serial.Emulators.WeightSPUNDevice`
- **Terminal:** `NLib.Serial.Terminals.WeightSPUNTerminal`
- **Log Reference:** `Documents/LuckyTex Devices/WeightSPUN/`

---

## See Also

- [Device Comparison](CODE_ANALYSIS_NLib.Serial.Devices.md#device-implementations)
- [Base Classes](CODE_ANALYSIS_NLib.Serial.Devices.md#base-class-framework)
- [CordDEFENDER3000 Device](Device-01-CordDEFENDER3000.md) - Identical protocol, different capacity
- [WeightQA Device](Device-06-WeightQA.md) - Another quality scale with stability indicator
