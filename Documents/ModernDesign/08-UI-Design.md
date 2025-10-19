# UI Design (Draft Notes)

**Document Version**: 0.1 (Draft)
**Last Updated**: 2025-10-19
**Status**: Placeholder for future design

---

## Critical UI Requirements

### Field Editor Component (HIGH PRIORITY)

**Requirement Source**: FR-4.6 in 00-Requirements-Specification.md

**Purpose**: Allow users to rename auto-generated field names to meaningful property names for their T class.

**Problem to Solve**:
- Protocol data rarely contains field names
- Analyzer generates generic names: "Field1", "Field2", "Field3"
- Users need meaningful names for C# properties: "NetWeight", "GrossWeight", "Unit"

**Required Features**:
1. **Display Fields**
   - Show all detected fields in editable grid/list
   - Columns:
     - Auto-Generated Name (read-only)
     - Custom Name (editable)
     - Data Type (read-only)
     - Sample Values (read-only, multiple samples)
     - Confidence Score (read-only)

2. **Editing**
   - Double-click or F2 to edit custom name
   - Tab to navigate between fields
   - Enter to confirm, Esc to cancel
   - Real-time validation as user types

3. **Validation**
   - Must be valid C# identifier:
     - Start with letter or underscore
     - Contain only letters, digits, underscores
     - No C# keywords
   - Must be unique within definition
   - Show error icon/message for invalid names
   - Prevent saving if invalid names exist

4. **Smart Suggestions**
   - Auto-suggest based on sample values:
     - "1.640" → "Weight"
     - "kg" → "Unit"
     - "N", "G" → "Status"
   - Context menu: "Suggest Name"
   - Bulk operations: Prefix all with device name

5. **Preview**
   - Real-time JSON definition preview
   - Show how field names will appear in generated code
   - Highlight changes

6. **Persistence**
   - Save custom names with definition
   - Load custom names when reopening analysis
   - Preserve names across re-analysis

**UI Mockup Concept** (to be detailed):
```
┌─ Field Editor ─────────────────────────────────────────┐
│ Auto Name   │ Custom Name    │ Type    │ Sample Values │
├─────────────┼────────────────┼─────────┼───────────────┤
│ Field1      │ [NetWeight   ] │ decimal │ 1.640, 1.645  │
│ Field2      │ [Unit        ] │ string  │ kg, kg        │
│ Field3      │ [Status      ] │ char    │ N, G, S       │
└─────────────────────────────────────────────────────────┘
[Suggest Names] [Reset All] [Apply]
```

---

## Other UI Components (To Be Designed)

### Main Window Layout
- File picker
- Log display (raw and parsed)
- Analysis results view
- Confidence scores display
- Pattern override controls
- Definition preview
- Save dialog

### User Workflow
1. Load file → 2. Analyze → 3. **Edit Fields** → 4. Review → 5. Save

---

**Next Steps**:
- Create detailed wireframes
- Design all UI components
- Define user interactions
- Create UI flow diagrams
- Specify data binding requirements

See also:
- **00-Requirements-Specification.md** - FR-4 User Interface requirements
- **02-System-Architecture.md** - Data flow and component interactions
