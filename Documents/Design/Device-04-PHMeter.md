# Device Implementation: PHMeter

**Device Type:** pH Meter with Temperature
**Complexity:** ⭐⭐⭐ Medium-Complex
**Protocol:** Multi-line with pH, temperature, and date/time
**File:** `PHMeter.cs`

---

## Overview

pH meter with temperature compensation. Sends multiple lines including pH value, temperature, date, and time. Uses special encoding for degree symbol.

### Protocol Specification

**Format:** Multi-line with varying patterns

**Example:**
```
3.01pH 25.5°C ATC
20-Feb-2023
11:12
```

**Field Description:**
- **pH Line:** `[pH value]pH [temp]°C ATC`
  - pH value: decimal with 2 decimal places
  - Temperature: decimal in Celsius
  - 0xF8 byte for degree symbol (°)
  - ATC: Automatic Temperature Compensation indicator
- **Date Line:** `dd-MMM-yyyy` format (e.g., 20-Feb-2023)
- **Time Line:** `HH:mm` format (24-hour)

**Special Bytes:**
- `0xF8` = Degree symbol (°)

**Update Rate:** On-demand or periodic
**Precision:** pH ±0.01, Temperature ±0.1°C

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

    class PHMeterData {
        -decimal _pH
        -decimal _TempC
        -DateTime _Date
        +decimal pH
        +decimal TempC
        +DateTime Date
        +ToByteArray() byte[]
        +ToByteArray(int atcCount) byte[]
        +Parse(byte[]) void
    }

    class SerialDeviceTerminal {
        <<abstract>>
        #ProcessRXQueue() void
        +Connect() void
        +Disconnect() void
        +Value T
    }

    class PHMeterTerminal {
        -ExtractPackage() byte[]
        -UpdateValue(byte[]) void
        -UpdateValues(byte[][]) void
        #ProcessRXQueue() void
        +DeviceName string
    }

    SerialDeviceData <|-- PHMeterData
    SerialDeviceTerminal <|-- PHMeterTerminal
    PHMeterTerminal --> PHMeterData : uses
```

---

## Data Class Properties

### PHMeterData

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `pH` | decimal | 7.0 | pH value (0-14 range) |
| `TempC` | decimal | 25.0 | Temperature in Celsius |
| `Date` | DateTime | DateTime.Now | Date and time of measurement |

### ToByteArray() Overloads

```csharp
// Standard output (3 lines)
public override byte[] ToByteArray()

// Custom ATC count for testing
public byte[] ToByteArray(int atcCount)
```

---

## Flowchart - Multi-Pattern Parsing

```mermaid
flowchart TD
    Start([UpdateValue Called]) --> Convert[Convert bytes to ASCII]
    Convert --> Trim[Trim whitespace]
    Trim --> CheckPattern{Check line<br/>pattern}

    CheckPattern -->|Contains ATC AND pH| BothValues[Parse both pH and Temp]
    CheckPattern -->|Contains ATC NOT pH| TempOnly[Parse Temp only]
    CheckPattern -->|Contains pH NOT ATC| PHOnly[Parse pH only]
    CheckPattern -->|Contains -| DatePattern[Parse Date]
    CheckPattern -->|Contains :| TimePattern[Parse Time]
    CheckPattern -->|Other| End([Return])

    BothValues --> FindPH[Find pH index]
    FindPH --> SubstringPH[Substring before pH]
    SubstringPH --> ParsePH[Parse pH value]
    ParsePH --> TryParsePH{Try Parse}
    TryParsePH -->|Success| SetPH[Value.pH = parsed]
    TryParsePH -->|Exception| LogPHError[Log Error]

    SetPH --> SplitByPH[Split by pH]
    LogPHError --> SplitByPH
    SplitByPH --> FindATC[Find C ATC in second part]
    FindATC --> SubstringTemp[Substring before degree symbol]
    SubstringTemp --> ParseTemp[Parse Temp value]
    ParseTemp --> TryParseTemp{Try Parse}
    TryParseTemp -->|Success| SetTemp[Value.TempC = parsed]
    TryParseTemp -->|Exception| LogTempError[Log Error]
    SetTemp --> End
    LogTempError --> End

    TempOnly --> FindATC2[Find C ATC index]
    FindATC2 --> SubstringTemp2[Substring before degree]
    SubstringTemp2 --> ParseTemp2[Parse Temp value]
    ParseTemp2 --> TryParseTemp2{Try Parse}
    TryParseTemp2 -->|Success| SetTemp2[Value.TempC = parsed]
    TryParseTemp2 -->|Exception| LogTemp2Error[Log Error]
    SetTemp2 --> End
    LogTemp2Error --> End

    PHOnly --> FindPH2[Find pH index]
    FindPH2 --> SubstringPH2[Substring before pH]
    SubstringPH2 --> ParsePH2[Parse pH value]
    ParsePH2 --> TryParsePH2{Try Parse}
    TryParsePH2 -->|Success| SetPH2[Value.pH = parsed]
    TryParsePH2 -->|Exception| LogPH2Error[Log Error]
    SetPH2 --> End
    LogPH2Error --> End

    DatePattern --> ParseDateFormat[ParseExact dd-MMM-yyyy]
    ParseDateFormat --> TryParseDate{Try Parse}
    TryParseDate -->|Success| CombineDate[Combine with existing time]
    TryParseDate -->|Exception| LogDateError[Log Error<br/>Use current date]
    CombineDate --> SetDate[Value.Date = combined]
    LogDateError --> SetDate
    SetDate --> End

    TimePattern --> ParseTimeFormat[ParseExact HH:mm]
    ParseTimeFormat --> TryParseTime{Try Parse}
    TryParseTime -->|Success| CombineTime[Combine with existing date]
    TryParseTime -->|Exception| LogTimeError[Log Error<br/>Use current time]
    CombineTime --> SetTime[Value.Date = combined]
    LogTimeError --> SetTime
    SetTime --> End

    style Start fill:#e1f5ff
    style End fill:#e1f5ff
    style CheckPattern fill:#fff4e6
    style TryParsePH fill:#fff4e6
    style TryParseTemp fill:#fff4e6
    style LogPHError fill:#ffebee
    style LogTempError fill:#ffebee
```

---

## Sequence Diagram - pH and Temperature Reading

```mermaid
sequenceDiagram
    participant Meter as pH Meter
    participant Terminal as PHMeterTerminal
    participant Parser as UpdateValue
    participant Data as PHMeterData

    Note over Meter: Sending reading with<br/>pH and Temperature

    Meter->>Terminal: "3.01pH 25.5°C ATC\r\n"
    Terminal->>Parser: UpdateValue(line)

    activate Parser
    Parser->>Parser: Contains ATC AND pH? Yes
    Parser->>Parser: Find pH at index 4
    Parser->>Parser: Substring(0, 4) = 3.01
    Parser->>Data: Value.pH = 3.01

    Parser->>Parser: Split by pH: ["3.01", " 25.5°C ATC"]
    Parser->>Parser: Second part: " 25.5°C ATC"
    Parser->>Parser: Find C ATC at index 9
    Parser->>Parser: Substring before degree: 25.5
    Parser->>Data: Value.TempC = 25.5
    deactivate Parser

    Meter->>Terminal: "20-Feb-2023\r\n"
    Terminal->>Parser: UpdateValue(line)
    activate Parser
    Parser->>Parser: Contains -? Yes
    Parser->>Parser: ParseExact("dd-MMM-yyyy")
    Parser->>Parser: Combine with existing time
    Parser->>Data: Value.Date = 2023-02-20 + time
    deactivate Parser

    Meter->>Terminal: "11:12\r\n"
    Terminal->>Parser: UpdateValue(line)
    activate Parser
    Parser->>Parser: Contains :? Yes
    Parser->>Parser: ParseExact("HH:mm")
    Parser->>Parser: Combine with existing date
    Parser->>Data: Value.Date = date + 11:12:00
    deactivate Parser

    Data-->>Data: PropertyChanged events
```

---

## Implementation Details

### Key Parsing Methods

#### Parse pH and Temperature Together
```csharp
if (content.Contains("ATC") && content.Contains("pH"))
{
    // Parse pH
    int idxPH = content.IndexOf("pH");
    string phStr = content.Substring(0, idxPH);
    try
    {
        Value.pH = decimal.Parse(phStr.Trim());
    }
    catch (Exception ex)
    {
        MethodBase.GetCurrentMethod().Err(ex);
    }

    // Parse Temperature
    string[] parts = content.Split(new[] { "pH" }, StringSplitOptions.None);
    if (parts.Length > 1)
    {
        string tempPart = parts[1];
        int idxATC = tempPart.IndexOf("C ATC");
        if (idxATC > 0)
        {
            // Extract temperature before degree symbol
            string tempStr = "";
            for (int i = 0; i < idxATC; i++)
            {
                if (char.IsDigit(tempPart[i]) || tempPart[i] == '.' || tempPart[i] == '-')
                    tempStr += tempPart[i];
            }
            try
            {
                Value.TempC = decimal.Parse(tempStr.Trim());
            }
            catch (Exception ex)
            {
                MethodBase.GetCurrentMethod().Err(ex);
            }
        }
    }
}
```

#### Parse Date
```csharp
if (content.Contains("-") && !content.Contains(":"))
{
    try
    {
        DateTime date = DateTime.ParseExact(content, "dd-MMM-yyyy",
                                            CultureInfo.InvariantCulture);
        // Combine with existing time
        Value.Date = new DateTime(date.Year, date.Month, date.Day,
                                  Value.Date.Hour, Value.Date.Minute, 0);
    }
    catch (Exception ex)
    {
        MethodBase.GetCurrentMethod().Err(ex);
    }
}
```

#### Parse Time
```csharp
if (content.Contains(":") && !content.Contains("-"))
{
    try
    {
        DateTime time = DateTime.ParseExact(content, "HH:mm",
                                            CultureInfo.InvariantCulture);
        // Combine with existing date
        Value.Date = new DateTime(Value.Date.Year, Value.Date.Month, Value.Date.Day,
                                  time.Hour, time.Minute, 0);
    }
    catch (Exception ex)
    {
        MethodBase.GetCurrentMethod().Err(ex);
    }
}
```

### Degree Symbol Handling

The device uses byte `0xF8` for the degree symbol (°). This is not standard ASCII but is handled correctly when converting to/from byte arrays.

---

## Usage Example

### Emulator (Sending Data)
```csharp
var emulator = PHMeterDevice.Instance;
emulator.LoadConfig();
emulator.Start();

// Simulate pH measurement
emulator.Value.pH = 3.01m;
emulator.Value.TempC = 25.5m;
emulator.Value.Date = DateTime.Now;
byte[] data = emulator.Value.ToByteArray();
// Automatically transmitted
```

### Terminal (Receiving Data)
```csharp
var terminal = PHMeterTerminal.Instance;
terminal.LoadConfig();
terminal.Connect();

// Listen for pH readings
terminal.OnRx += (s, e) => {
    Console.WriteLine($"pH: {terminal.Value.pH:F2}");
    Console.WriteLine($"Temperature: {terminal.Value.TempC:F1}°C");
    Console.WriteLine($"Measured: {terminal.Value.Date:yyyy-MM-dd HH:mm}");

    // pH classification
    string classification = terminal.Value.pH < 7 ? "ACIDIC" :
                           terminal.Value.pH > 7 ? "ALKALINE" : "NEUTRAL";
    Console.WriteLine($"Classification: {classification}");
};
```

---

## Protocol Examples

### Complete Reading
```
3.01pH 25.5°C ATC
20-Feb-2023
11:12
```

### pH Only
```
7.42pH
```

### Temperature Only
```
24.8°C ATC
```

### Different pH Values
```
1.23pH 25.0°C ATC    # Strong acid
7.00pH 25.0°C ATC    # Neutral
13.45pH 25.0°C ATC   # Strong base
```

---

## Testing Notes

- **Pattern Matching:** Parser checks for different line patterns
- **ATC:** Automatic Temperature Compensation always present
- **Date Format:** Uses 3-letter month abbreviation (Feb, Mar, etc.)
- **Time Format:** 24-hour format (HH:mm)
- **Temperature Effect:** pH is temperature-dependent, hence ATC
- **Calibration:** Regular calibration with pH 4.01, 7.00, and 10.01 buffers

---

## pH Ranges Reference

| pH Range | Classification | Examples |
|----------|---------------|----------|
| 0-3 | Strong Acid | Battery acid, stomach acid |
| 3-6 | Weak Acid | Orange juice, coffee |
| 6-8 | Neutral | Pure water, milk |
| 8-11 | Weak Base | Baking soda, seawater |
| 11-14 | Strong Base | Bleach, drain cleaner |

---

## Related Files

- **Data Class:** `NLib.Serial.Devices.PHMeterData`
- **Emulator:** `NLib.Serial.Emulators.PHMeterDevice`
- **Terminal:** `NLib.Serial.Terminals.PHMeterTerminal`
- **Log Reference:** `Documents/LuckyTex Devices/PHMeter/`

---

## See Also

- [Device Comparison](CODE_ANALYSIS_NLib.Serial.Devices.md#device-implementations)
- [Base Classes](CODE_ANALYSIS_NLib.Serial.Devices.md#base-class-framework)
- [JIK6CAB Device](Device-02-JIK6CAB.md) - Another multi-line protocol
