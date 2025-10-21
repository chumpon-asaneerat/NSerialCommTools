# Extra Devices - Protocol Research Collection

**Purpose**: Collect real-world device protocols to test and validate the Protocol Analyzer's universal detection algorithms.

**Target**: 15 different devices across 3 categories

---

## Folder Structure

```
ExtraDevices/
├── WeightMachines/         (5 devices)
│   ├── Device01/
│   │   ├── origin.md       ← Device info, manufacturer, links
│   │   └── log_data.txt    ← HEX/Text format log data
│   ├── Device02/
│   ├── Device03/
│   ├── Device04/
│   └── Device05/
├── PhMeters/               (5 devices)
│   ├── Device01/
│   ├── Device02/
│   ├── Device03/
│   ├── Device04/
│   └── Device05/
└── YardageCounters/        (5 devices)
    ├── Device01/
    ├── Device02/
    ├── Device03/
    ├── Device04/
    └── Device05/
```

---

## Device Categories

### 1. Weight Machines (Laboratory/Industrial Scales)

**Target Manufacturers**:
- Mettler Toledo (MT-SICS protocol)
- Ohaus (Discovery, Explorer, Adventurer series)
- A&D Weighing (FX-i, GX-A series)
- Sartorius (Entris, Quintix, Cubis series)
- Avery Weigh-Tronix

**What to Look For**:
- RS232/RS485 serial protocol documentation
- Continuous output mode (streaming weight data)
- Example output messages
- Command/response protocols

**Typical Protocol Features**:
- Weight value + unit (kg, g, lb)
- Stability indicator (S/D/N)
- Sign (+/-)
- Tare/Gross/Net weight modes
- Piece counting

---

### 2. pH Meters

**Target Manufacturers**:
- Mettler Toledo (SevenCompact, SevenExcellence)
- Hanna Instruments (HI series)
- Thermo Scientific (Orion series)
- Hach (HQ series)
- WTW (inoLab, ProfiLine)

**What to Look For**:
- RS232 interface documentation
- Continuous measurement mode
- Data output format examples
- Temperature compensation (ATC/MTC)

**Typical Protocol Features**:
- pH value (0-14)
- Temperature in °C or °F
- mV readings
- Calibration status
- Electrode condition
- Date/time stamps

---

### 3. Yardage/Length Counters

**Target Manufacturers**:
- Veeder-Root (counters)
- Kuebler (length measurement)
- Redington Counters
- Trumeter (count/rate indicators)
- Kubler (encoders with display)

**What to Look For**:
- Serial output specifications
- Pulse-to-distance conversion
- Real-time counter value output
- Reset/preset commands

**Typical Protocol Features**:
- Counter value (yards, meters, feet)
- Speed/rate measurement
- Batch counting
- Preset comparison
- Direction sensing

---

## How to Populate Each Device Folder

### Step 1: Find Device Documentation

Search for:
1. **Official manual** - Manufacturer's technical documentation
2. **Protocol specification** - RS232/RS485 communication details
3. **Example data** - Sample output messages
4. **GitHub/Forums** - Reverse-engineered protocols

**Search Terms**:
- "[Manufacturer] [Model] RS232 protocol manual"
- "[Manufacturer] [Model] serial communication specification"
- "[Manufacturer] [Model] interface protocol PDF"
- "[Device Type] RS232 protocol example"

---

### Step 2: Create origin.md

Template:

```markdown
# Device Origin Information

## Device Details

**Manufacturer**: [Company Name]
**Model**: [Model Number/Name]
**Device Type**: [Weight Machine / pH Meter / Yardage Counter]
**Series**: [Product series if applicable]

---

## Communication Specifications

**Interface**: RS232 / RS485 / USB (virtual COM)
**Baud Rate**: [e.g., 9600, 19200]
**Data Bits**: [7 or 8]
**Parity**: [None, Even, Odd]
**Stop Bits**: [1 or 2]
**Flow Control**: [None, Hardware, Software]

---

## Protocol Characteristics

**Message Structure**: [Single-line / Multi-line / Frame-based]
**Delimiter**: [Space / Comma / Tab / Special character]
**Terminator**: [CR, LF, CRLF, Custom]
**Binary Data**: [Yes/No - if yes, specify]

---

## Data Fields

List expected fields:
- Field 1: [Description, data type, unit]
- Field 2: [Description, data type, unit]
- ...

---

## Source Documentation

**Manual URL**: [Link to PDF or webpage]
**Protocol Spec URL**: [Link if separate from manual]
**GitHub**: [If reverse-engineered protocol exists]
**Forum/Discussion**: [Any relevant threads]

**Downloaded Date**: [YYYY-MM-DD]
**Archive Location**: [If you saved PDFs locally]

---

## Sample Message Format

Describe the expected message format here:

\`\`\`
Example text representation of a typical message
\`\`\`

---

## Notes

Any special considerations:
- Quirks or unusual behavior
- Multiple message types
- Configuration requirements
- Known issues
```

---

### Step 3: Create log_data.txt

**CRITICAL**: Use HEX/Text format (NOT text-only)

Format:
```
HEX_BYTES_WITH_SPACES   TEXT_REPRESENTATION
```

**Example for Weight Machine**:
```
2D 20 20 31 2E 36 34 30 20 6B 67 20 20 20 20 4E   -  1.640 kg    N
2D 20 20 31 2E 36 34 32 20 6B 67 20 20 20 20 4E   -  1.642 kg    N
2D 20 20 31 2E 36 33 38 20 6B 67 20 20 20 20 4E   -  1.638 kg    N
```

**Requirements**:
- Minimum 20 messages (more is better)
- Include variety (different values, modes, etc.)
- Show complete message cycles
- Include start/end markers if applicable
- Preserve all bytes including control characters

**How to Get HEX Data**:

1. **From Documentation**: Some manuals include HEX examples
2. **Serial Port Monitor**: Use tool like RealTerm, Termite, or Serial Monitor
3. **Convert from Text**: If you only have text, convert each character to HEX
4. **Simulation**: Create realistic synthetic data based on protocol spec

**Conversion Tools**:
- Online: HexEd.it, RapidTables ASCII to HEX converter
- Local: Python script, PowerShell, or C# utility

---

## Research Strategy

### Priority Order

1. **Mettler Toledo** - Excellent documentation, widely used
2. **Ohaus** - Good manuals, many models
3. **A&D Weighing** - Comprehensive protocol specs
4. **Sartorius** - Well-documented interfaces
5. **Others** - Fill in remaining slots

### Search Locations

1. **Manufacturer Websites**:
   - Support/Downloads section
   - Technical documentation
   - Product manuals

2. **Google Search**:
   - Use exact model numbers
   - Add "filetype:pdf" to find manuals
   - Search for "protocol" or "interface" specifically

3. **GitHub**:
   - Search for device drivers
   - Look for reverse-engineered protocols
   - Check IoT/automation projects

4. **Forums**:
   - Stack Overflow (serial communication tags)
   - EEVblog forums
   - Reddit r/embedded, r/AskElectronics

5. **Internet Archive**:
   - Old manuals no longer on manufacturer sites
   - Wayback Machine for discontinued products

---

## Validation Criteria

Each device should demonstrate at least one of these protocol characteristics:

### Message Structure Patterns
- [ ] Single-line repeating (like CordDEFENDER3000)
- [ ] Multi-line frame with markers (like JIK6CAB)
- [ ] Header-byte based (like TFO1)
- [ ] Content-based (like PHMeter)
- [ ] Binary protocol
- [ ] Mixed ASCII/binary

### Delimiter Patterns
- [ ] Space-delimited
- [ ] Comma-delimited (CSV)
- [ ] Tab-delimited
- [ ] Hierarchical (multiple delimiter types)
- [ ] Fixed-width fields
- [ ] No delimiters (pure positional)

### Field Complexity
- [ ] Simple value + unit
- [ ] Compound fields (value/precision/unit)
- [ ] Date + Time combination
- [ ] Calculated relationships (GW - TW = NW)
- [ ] Binary flags/status bytes
- [ ] Checksum/CRC validation

**Goal**: Collect diverse protocols to test all algorithm paths in the Protocol Analyzer.

---

## Progress Tracker

### Weight Machines
- [ ] Device01: _______________ (Manufacturer/Model)
- [ ] Device02: _______________
- [ ] Device03: _______________
- [ ] Device04: _______________
- [ ] Device05: _______________

### pH Meters
- [ ] Device01: _______________
- [ ] Device02: _______________
- [ ] Device03: _______________
- [ ] Device04: _______________
- [ ] Device05: _______________

### Yardage Counters
- [ ] Device01: _______________
- [ ] Device02: _______________
- [ ] Device03: _______________
- [ ] Device04: _______________
- [ ] Device05: _______________

---

## Next Steps

1. Start with Mettler Toledo devices (excellent documentation)
2. Fill in Device01 folders for each category first
3. Test Protocol Analyzer algorithms with each new device
4. Document any patterns not covered by existing algorithms
5. Update parsing strategy documents with findings

---

**Last Updated**: 2025-10-21
**Status**: Ready for population - folders created, awaiting device research
