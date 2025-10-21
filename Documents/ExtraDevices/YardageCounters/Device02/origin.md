# Device Origin Information

## Device Details

**Manufacturer**: [Company Name]
**Model**: [Model Number/Name]
**Device Type**: [Weight Machine / pH Meter / Yardage Counter]
**Series**: [Product series if applicable]

---

## Communication Specifications

**Interface**: [RS232 / RS485 / USB virtual COM]
**Baud Rate**: [e.g., 9600, 19200, 115200]
**Data Bits**: [7 or 8]
**Parity**: [None, Even, Odd]
**Stop Bits**: [1 or 2]
**Flow Control**: [None, Hardware (RTS/CTS), Software (XON/XOFF)]

---

## Protocol Characteristics

**Message Structure**: [Single-line / Multi-line / Frame-based / State Machine]
**Delimiter**: [Space / Comma / Tab / Slash / Special character / None (fixed position)]
**Terminator**: [CR (0x0D), LF (0x0A), CRLF (0x0D 0x0A), Custom]
**Binary Data**: [Yes/No - if yes, specify which bytes]
**Start Marker**: [If applicable, e.g., ^KJIK000]
**End Marker**: [If applicable, e.g., ~P1]

---

## Data Fields

List expected fields in order:

1. **Field Name**: [Description]
   - Data Type: [Integer / Decimal / String / DateTime / Binary]
   - Unit: [kg, g, pH, °C, yards, etc.]
   - Range: [Min - Max values]
   - Format: [e.g., "±XXX.XX" or "YYYY-MM-DD"]

2. **Field Name**: [Description]
   - Data Type: [...]
   - ...

---

## Source Documentation

**Manual URL**: [Direct link to PDF or webpage]
**Protocol Spec URL**: [Link if separate from manual]
**GitHub Repository**: [If reverse-engineered protocol exists]
**Forum/Discussion**: [Any relevant threads with protocol info]

**Downloaded Date**: [YYYY-MM-DD]
**Archive Location**: [Local path if you saved PDFs]

---

## Sample Message Format

Describe the expected message format:

```
[Show ASCII representation here]
Example:
-  1.640 kg    N
```

OR for multi-line:

```
Line 1: [Description]
Line 2: [Description]
...
```

---

## Protocol Analysis

**Detected Strategy**: [Delimiter-Based / Frame-Based / Position-Based / State Machine / Content-Based / Hybrid]

**Parsing Complexity**: [Low / Medium / High]

**Special Features**:
- [ ] Field relationships (combine, split, calculate)
- [ ] Validation rules (checksums, formulas)
- [ ] Multiple message types
- [ ] Binary bytes mixed with ASCII
- [ ] Variable message length
- [ ] State tracking required

---

## Notes

Document any special considerations:
- Quirks or unusual behavior
- Device-specific requirements
- Configuration needed for continuous output
- Known issues or limitations
- Version differences

---

## Test Expectations

When testing with Protocol Analyzer, this device should demonstrate:

- [ ] Message boundary detection: [Expected result]
- [ ] Delimiter detection: [Expected result]
- [ ] Field extraction: [Expected # of fields]
- [ ] Data type inference: [Expected types]
- [ ] Relationship detection: [Expected relationships if any]
- [ ] Validation rules: [Expected rules if any]

---

**Status**: [NOT POPULATED / IN PROGRESS / COMPLETE]
**Last Updated**: [YYYY-MM-DD]
