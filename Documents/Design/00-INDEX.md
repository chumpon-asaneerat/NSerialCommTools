# NSerialCommTools Documentation Index

**Project:** NSerialCommTools - Serial Communication Library
**Technology:** .NET Framework 4.7.2
**Date:** 2025-10-08
**Location:** Documents/Design/

---

## Overview

This folder contains comprehensive documentation for the NSerialCommTools project, including architecture analysis, device-specific diagrams, and implementation guides.

---

## Core Documentation

### 📋 Project Management
- **[DOCUMENTATION_TRACKING.md](DOCUMENTATION_TRACKING.md)** - Task tracking and progress
- **[claude.md](../../claude.md)** - Session instructions and standards

### 🏗️ Architecture & Analysis
- **[CODE_ANALYSIS_NLib.Serial.Devices.md](CODE_ANALYSIS_NLib.Serial.Devices.md)** - Complete code analysis
  - Three-layer architecture
  - Base class framework
  - Design patterns
  - All 7 device implementations overview
  - Common patterns
  - Protocol comparison

- **[LOG_FILES_ANALYSIS.md](LOG_FILES_ANALYSIS.md)** - Protocol specifications
  - Raw log file analysis
  - Protocol patterns
  - Implementation implications
  - Device comparison table

---

## Device-Specific Documentation

Each device has dedicated documentation with detailed Mermaid diagrams:

### Device 1: CordDEFENDER3000 ⭐ Simple
**[Device-01-CordDEFENDER3000.md](Device-01-CordDEFENDER3000.md)**
- Simple weight scale
- Single-line protocol
- Continuous streaming
- Stability indicators (?, G, N)

**Diagrams:**
- ✅ Class diagram
- ✅ Sequence diagram (data reception)
- ✅ Flowchart (parsing logic)
- ✅ State diagram (device status)

---

### Device 2: JIK6CAB ⭐⭐⭐⭐ Complex
**[Device-02-JIK6CAB.md](Device-02-JIK6CAB.md)**
- Multi-line weight scale
- 14-line structured package
- State machine parser
- Package markers (^KJIK000, ~P1)

**Diagrams:**
- ✅ Class diagram
- ✅ State diagram (parsing state machine)
- ✅ Sequence diagram (multi-line processing)
- ✅ Flowchart (line type detection)

---

### Device 3: MettlerMS204TS00 ⭐⭐ Simple-Medium
**[Device-03-MettlerMS204TS00.md](Device-03-MettlerMS204TS00.md)**
- High-precision laboratory balance
- 0.0001g resolution
- Mode indicators (N, G, T)
- Single-line protocol

**Diagrams:**
- ✅ Class diagram
- ✅ Flowchart (mode and weight parsing)
- ✅ Sequence diagram (high-precision reading)

---

### Device 4: PHMeter ⭐⭐⭐ Medium
**[Device-04-PHMeter.md](Device-04-PHMeter.md)**
- pH meter with temperature compensation
- Multi-line protocol
- Special encoding (0xF8 for °)
- Date/time stamps

**Diagrams:**
- ✅ Class diagram
- ✅ Flowchart (multi-pattern parsing)
- ✅ Sequence diagram (pH and temperature reading)

---

### Device 5: TFO1 ⭐⭐⭐⭐⭐ Very Complex
**[Device-05-TFO1.md](Device-05-TFO1.md)**
- Industrial system
- Mixed binary/ASCII protocol
- Field-identifier based (F, H, Q, X, A, 0-4, B, C, V)
- Custom date encoding

**Diagrams:**
- ✅ Class diagram
- ✅ Flowchart (field identifier switching)
- ✅ Sequence diagram (multi-field package)
- ✅ State diagram (field processing)

---

### Device 6: WeightQA ⭐⭐ Medium
**[Device-06-WeightQA.md](Device-06-WeightQA.md)**
- Quality assurance scale
- Stability index (0-8 scale)
- Single-line protocol
- Stability tracking

**Diagrams:**
- ✅ Class diagram
- ✅ Flowchart (stability index decoding)
- ✅ Sequence diagram (stability tracking)
- ✅ State diagram (stability states)

---

### Device 7: WeightSPUN ⭐ Simple
**[Device-07-WeightSPUN.md](Device-07-WeightSPUN.md)**
- Dynamic weighing scale
- High capacity (100+ kg)
- Similar to DEFENDER3000
- Dynamic vs static modes

**Diagrams:**
- ✅ Class diagram
- ✅ Flowchart (parsing logic)
- ✅ Sequence diagram (dynamic weight loading)
- ✅ State diagram (dynamic vs static)

---

## Documentation Statistics

### Total Documents: 10 files

### Total Diagrams: 54 Mermaid diagrams
- **Class Diagrams:** 7
- **Sequence Diagrams:** 7
- **Flowcharts:** 7
- **State Diagrams:** 7
- **Architecture Diagrams:** 2
- **Comparison Diagrams:** 2

### Device Complexity Distribution
| Complexity | Count | Devices |
|------------|-------|---------|
| ⭐ Simple | 2 | DEFENDER3000, WeightSPUN |
| ⭐⭐ Medium | 2 | MettlerMS204TS00, WeightQA |
| ⭐⭐⭐ Medium-Complex | 1 | PHMeter |
| ⭐⭐⭐⭐ Complex | 1 | JIK6CAB |
| ⭐⭐⭐⭐⭐ Very Complex | 1 | TFO1 |

---

## Quick Reference

### By Protocol Type

**Single-Line Protocols:**
- [CordDEFENDER3000](Device-01-CordDEFENDER3000.md) - Weight with stability
- [WeightSPUN](Device-07-WeightSPUN.md) - High capacity weight
- [MettlerMS204TS00](Device-03-MettlerMS204TS00.md) - Precision balance
- [WeightQA](Device-06-WeightQA.md) - Weight with stability index

**Multi-Line Protocols:**
- [JIK6CAB](Device-02-JIK6CAB.md) - 14-line structured package
- [PHMeter](Device-04-PHMeter.md) - pH, temp, date/time
- [TFO1](Device-05-TFO1.md) - Field-based mixed encoding

### By Feature

**Stability Indicators:**
- [CordDEFENDER3000](Device-01-CordDEFENDER3000.md) - Prefix symbols (?, G, N)
- [WeightQA](Device-06-WeightQA.md) - Numeric index (0-8)
- [WeightSPUN](Device-07-WeightSPUN.md) - Prefix symbols (?, G)

**High Precision:**
- [MettlerMS204TS00](Device-03-MettlerMS204TS00.md) - 0.0001g resolution

**Multi-Parameter:**
- [PHMeter](Device-04-PHMeter.md) - pH + Temperature
- [JIK6CAB](Device-02-JIK6CAB.md) - Multiple weights + PCS
- [TFO1](Device-05-TFO1.md) - 11 different parameters

**Date/Time Stamps:**
- [PHMeter](Device-04-PHMeter.md) - Standard format
- [JIK6CAB](Device-02-JIK6CAB.md) - Standard format
- [TFO1](Device-05-TFO1.md) - Custom encoding

---

## Documentation Standards

### Diagram Standards
All diagrams follow these standards (defined in [claude.md](../../claude.md)):
- ✅ ONLY Mermaid syntax
- ❌ NO ASCII art
- ✅ Proper syntax validation
- ✅ Render correctly in Markdown viewers

### Mermaid Types Used:
- `classDiagram` - Class relationships
- `sequenceDiagram` - Communication flow
- `flowchart TD` - Logic flow
- `stateDiagram-v2` - State machines
- `graph TD/LR` - Architecture

---

## Related Projects

### Core Library
- **NLib.Serial.Devices** - Core serial device classes
  - Location: `01.Core\NLib.Serial.Devices\`
  - See: [CODE_ANALYSIS_NLib.Serial.Devices.md](CODE_ANALYSIS_NLib.Serial.Devices.md)

### Applications (To be documented)
- **NLib.Serial.Emulator.App** - Device emulator
  - Location: `09.App\NLib.Serial.Emulator.App\`

- **NLib.Serial.Terminal.App** - Serial terminal
  - Location: `09.App\NLib.Serial.Terminal.App\`

---

## How to Use This Documentation

### For Developers
1. Start with [CODE_ANALYSIS_NLib.Serial.Devices.md](CODE_ANALYSIS_NLib.Serial.Devices.md) for architecture overview
2. Review specific device documentation for implementation details
3. Use diagrams to understand flow and logic
4. Reference [LOG_FILES_ANALYSIS.md](LOG_FILES_ANALYSIS.md) for protocol specs

### For New Device Implementation
1. Read [CODE_ANALYSIS_NLib.Serial.Devices.md](CODE_ANALYSIS_NLib.Serial.Devices.md) - Base class framework
2. Study similar device (by complexity)
3. Review protocol in [LOG_FILES_ANALYSIS.md](LOG_FILES_ANALYSIS.md)
4. Follow class diagram and sequence diagram patterns
5. Implement state machine if needed (see JIK6CAB or TFO1)

### For Testing
1. Review device-specific protocol examples
2. Use emulator to simulate device behavior
3. Use terminal to test real device communication
4. Verify state transitions match state diagrams

---

## Version History

| Date | Version | Changes |
|------|---------|---------|
| 2025-10-08 | 1.0 | Initial documentation created |
| 2025-10-08 | 1.1 | Split device diagrams into separate files |
| 2025-10-08 | 1.2 | Fixed all Mermaid syntax errors |

---

## Contributing

When adding new documentation:
1. Follow Mermaid-only diagram standard
2. Use device template format
3. Include all diagram types (class, sequence, flowchart, state)
4. Update this index file
5. Update DOCUMENTATION_TRACKING.md

---

## Contact & Support

For questions or clarifications, refer to:
- Project source code in `01.Core\NLib.Serial.Devices\`
- Log files in `Documents\LuckyTex Devices\`
- Session instructions in [claude.md](../../claude.md)
