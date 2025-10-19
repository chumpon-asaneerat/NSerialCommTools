# Serial Device Protocol Definitions - Index

This directory contains comprehensive JSON protocol definitions for all serial devices implemented in the NLib.Serial.Devices library.

## Overview

Each JSON file provides a complete, machine-readable specification of a serial device's communication protocol, including:
- Device metadata (manufacturer, model, category)
- Communication settings (baud rate, parity, etc.)
- Protocol structure and data format
- Parsing strategy and state machine
- Validation rules
- Output data mapping
- Test cases

## Device Definition Files

### 1. JIK6CAB-Full-Definition.json
- **Device**: JIK-6C-AB Multi-line Weight Scale
- **Manufacturer**: LuckyTex
- **Category**: Scale
- **Complexity**: Very Complex
- **Protocol Type**: Multi-line package (14 lines)
- **Key Features**:
  - State machine parser with 16 states
  - Package markers (start: ^KJIK000, end: ~P1)
  - Date/Time, Tare Weight, Gross Weight, Net Weight, Piece Count
  - Weight calculation validation (GW - TW = NW)

### 2. DEFENDER3000-Full-Definition.json
- **Device**: CordDEFENDER3000 Weight Scale
- **Manufacturer**: LuckyTex
- **Category**: Scale
- **Complexity**: Simple
- **Protocol Type**: Single-line continuous stream
- **Key Features**:
  - Simple space-delimited format: `WEIGHT UNIT OPERATION`
  - Example: `   0.360 kg    G`
  - 3 decimal places for weight (F3)
  - Operation mode indicator (typically 'G' for Gross)

### 3. MettlerMS204TS00-Full-Definition.json
- **Device**: Mettler Toledo MS204TS00 Precision Balance
- **Manufacturer**: Mettler Toledo
- **Category**: Precision Balance
- **Complexity**: Simple
- **Protocol Type**: Single-line continuous stream
- **Key Features**:
  - Optional mode character (N=Net, G=Gross, T=Tare)
  - 4 decimal places precision (0.0000)
  - Example: `     N       0.3746 g    `
  - Supports negative weights for tare

### 4. PHMeter-Full-Definition.json
- **Device**: Generic pH Meter
- **Manufacturer**: Generic
- **Category**: pH Meter
- **Complexity**: Complex
- **Protocol Type**: Multi-line variable-length report
- **Key Features**:
  - pH value with temperature (ATC)
  - Date and time reporting
  - Multiple line types parsed by pattern matching
  - Special character 0xF8 for degree symbol (°)
  - Repeatable pH+Temp lines (1-3 times)

### 5. TFO1-Full-Definition.json
- **Device**: TFO1 Textile Fiber Measurement Device
- **Manufacturer**: LuckyTex
- **Category**: Textile Measurement
- **Complexity**: Very Complex
- **Protocol Type**: Multi-line fixed format (12 lines)
- **Key Features**:
  - 12 measurement parameters (F, H, Q, X, A, W0-W4, W1, W2)
  - Binary status bytes
  - Complex datetime with special separators (0xF4, 0xF3, 0xF2)
  - Header-based line parsing (each line starts with identifier)
  - Mixed terminator: \r for most lines, \r\n for last line

### 6. WeightQA-Full-Definition.json
- **Device**: WeightQA Scale
- **Manufacturer**: LuckyTex
- **Category**: Scale
- **Complexity**: Medium
- **Protocol Type**: Single-line with fractional format
- **Key Features**:
  - **Unique fractional digit format**: Last decimal digit separated by '/'
  - Example: `+007.12/3 G S` represents 7.123 grams
  - Sign indicator (+/-)
  - Mode indicator (S=Stable)
  - Parser splits by '/' and reassembles decimal value

### 7. WeightSPUN-Full-Definition.json
- **Device**: WeightSPUN Scale
- **Manufacturer**: LuckyTex
- **Category**: Scale
- **Complexity**: Simple
- **Protocol Type**: Single-line continuous stream
- **Key Features**:
  - Similar to DEFENDER3000
  - Format: `WEIGHT UNIT OPERATION`
  - Example: `    20.0 kg    G`
  - 1 decimal place for weight (F1) vs DEFENDER3000's F3

### 8. TScaleQHW-Full-Definition.json
- **Device**: T-Scale QHW
- **Manufacturer**: Generic
- **Category**: Scale
- **Complexity**: Simple
- **Protocol Type**: Single-line CSV format
- **Key Features**:
  - CSV format: `STATUS,MODE, WEIGHT UNIT`
  - Example: `ST,GS,   245.6 g`
  - Status: ST=Stable, US=Unstable
  - Mode: GS=Gross, NT=Net, TR=Tare
  - **Space between weight and unit**

### 9. TScaleNHB-Full-Definition.json
- **Device**: T-Scale NHB
- **Manufacturer**: Generic
- **Category**: Scale
- **Complexity**: Simple
- **Protocol Type**: Single-line CSV format (compact)
- **Key Features**:
  - CSV format: `STATUS,MODE WEIGHTunit`
  - Example: `ST,GS    20.7g  `
  - Similar to TScaleQHW but **unit attached to weight** (no space)
  - Requires backward scanning to separate weight from unit

## Protocol Complexity Levels

### Simple Protocols
- **DEFENDER3000**: Space-delimited single line
- **MettlerMS204TS00**: Space-delimited with optional mode
- **WeightSPUN**: Space-delimited single line
- **TScaleQHW**: CSV format with space before unit
- **TScaleNHB**: CSV format with attached unit

### Medium Protocols
- **WeightQA**: Fractional format with '/' separator

### Complex Protocols
- **PHMeter**: Variable multi-line with pattern matching

### Very Complex Protocols
- **JIK6CAB**: 14-line package with state machine
- **TFO1**: 12-line fixed format with special characters

## Common Protocol Patterns

### 1. Single-Line Continuous Stream
- DEFENDER3000, WeightSPUN, MettlerMS204TS00
- Simple parsing, space-delimited fields

### 2. CSV-Based Protocols
- TScaleQHW, TScaleNHB
- Comma-separated with variations in unit formatting

### 3. Multi-Line Package Protocols
- JIK6CAB: Fixed package size with markers
- PHMeter: Variable lines with pattern matching
- TFO1: Fixed line count with header identifiers

### 4. Special Formats
- WeightQA: Fractional digit separator
- TFO1: Binary bytes and special datetime separators

## JSON Schema Structure

Each definition file follows this structure:

```json
{
  "$schema": "...",
  "version": "1.0",
  "lastUpdated": "YYYY-MM-DD",

  "deviceInfo": { ... },
  "communication": { ... },
  "protocol": { ... },
  "parsing": { ... },
  "validation": { ... },
  "output": { ... },
  "testing": { ... },
  "documentation": { ... },
  "migration": { ... }
}
```

## Key Sections Explained

### deviceInfo
Device metadata including name, manufacturer, category, model, description, and complexity level.

### communication
Serial port settings: baud rate, data bits, parity, stop bits, handshake, encoding, timeouts.

### protocol
Protocol structure including type, format, terminator, package size, and detailed line/field structure.

### parsing
Parsing strategy with state machine definitions, including states, transitions, and actions.

### validation
Validation rules for data integrity including range checks, enum validation, formula validation.

### output
C# data class mapping with property names, types, and event triggers.

### testing
Test cases with input/output examples and expected coverage metrics.

### documentation
References to related documentation, log files, and implementation notes.

### migration
Information about existing C# implementation compatibility.

## Usage

These JSON definitions can be used for:

1. **Documentation**: Human-readable protocol specifications
2. **Code Generation**: Generate parser implementations from definitions
3. **Testing**: Use test cases for validation
4. **Protocol Analysis**: Understand device communication patterns
5. **Migration**: Reference for porting to other platforms/languages
6. **Validation**: Ensure implementations match specifications

## Device Categories

### Scales (Weight Measurement)
- DEFENDER3000
- JIK6CAB
- WeightQA
- WeightSPUN
- TScaleQHW
- TScaleNHB

### Precision Balance
- MettlerMS204TS00

### pH Measurement
- PHMeter

### Textile Measurement
- TFO1

## Comparison: TScaleQHW vs TScaleNHB

These two devices are very similar with one key difference:

**TScaleQHW**:
```
ST,GS,   245.6 g
        ^^^^^^ ^
        weight unit (space separator)
```

**TScaleNHB**:
```
ST,GS    20.7g
         ^^^^
         weight+unit (no space)
```

## File Naming Convention

All definition files follow this pattern:
```
{DeviceName}-Full-Definition.json
```

## Version History

- **v1.0** (2025-10-19): Initial creation of all device definitions
  - Created 9 comprehensive device protocol definitions
  - All definitions based on existing C# implementation analysis
  - Backward-compatible with current codebase

## Related Files

- `JIK6CAB-Full-Definition.json`: JIK6CAB device definition (formerly EXAMPLE file)
- `03-Protocol-Definition-Schema.md`: JSON schema documentation
- `00-INDEX.md`: Main documentation index
- `01-System-Architecture.md`: System architecture overview
- `02-Protocol-Analyzer-Tool.md`: Protocol analysis tool documentation

## Notes

1. All definitions are backward-compatible with existing C# implementations
2. Special characters (0xF8, 0xF4, 0xF3, 0xF2) are documented in detail
3. Each definition includes test cases for validation
4. Complexity levels help understand implementation effort
5. State machines are fully documented for complex protocols
6. All definitions include validation rules and error handling

## Future Enhancements

Potential improvements to these definitions:
- Add mermaid diagrams for state machines
- Include sample raw data captures
- Add performance benchmarks
- Document timing characteristics
- Add troubleshooting guides
- Include common error scenarios

---

**Generated**: 2025-10-19
**Total Devices**: 9
**Based on**: NLib.Serial.Devices v4.0.4x3
