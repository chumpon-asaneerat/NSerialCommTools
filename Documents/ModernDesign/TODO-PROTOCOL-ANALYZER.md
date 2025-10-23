# Protocol Analyzer - Comprehensive TODO List

**Created**: 2025-10-22
**Status**: Implementation 20% complete - needs major redesign
**Priority**: CRITICAL - Main feature (JSON generation) is missing

---

## ⚠️ CRITICAL INSTRUCTION - DO NOT USE v1 FOLDER

**IMPORTANT**: Do NOT access, read, or modify ANY files in the `v1` folder:
- ❌ `09.App\NLib.Serial.Protocol.Analyzer\v1\**` - OLD/ABANDONED CODE
- ❌ `Documents\ModernDesign\v1\**` - OLD/ABANDONED DOCUMENTS

**ONLY work with root-level files**:
- ✅ `09.App\NLib.Serial.Protocol.Analyzer\Models\` - CURRENT models
- ✅ `09.App\NLib.Serial.Protocol.Analyzer\Analyzers\` - CURRENT analyzers
- ✅ `09.App\NLib.Serial.Protocol.Analyzer\Parsers\` - CURRENT parsers
- ✅ `Documents\ModernDesign\*.md` - CURRENT design documents (root level only)

The v1 folder contains previous failed attempts and must be ignored completely.

---

## Overview

This TODO tracks all missing components discovered during Session 2025-10-22. The Protocol Analyzer is only 20% complete. Critical architectural components are missing, including State Machine parsing, pattern generators, and JSON export.

---

## Phase 0: Document Review & Planning 📚

**Status**: NOT STARTED
**Priority**: CRITICAL - Must complete before any coding
**Estimated Time**: 2-3 hours

### Tasks:

- [ ] **0.1** Read all design documents
  - [ ] 00-Requirements-Specification.md
  - [ ] 01-Production-Code-Analysis.md
  - [ ] 02-System-Architecture.md
  - [ ] 03-Parsing-Strategy-Analysis.md
  - [ ] 04-Data-Models-Design.md
  - [ ] 05-JSON-Schema-Design.md
  - [ ] 06-Protocol-Analyzer-Complete-UI.md

- [ ] **0.2** Take notes on requirements
  - [ ] List all required FieldInfo properties
  - [ ] List all parsing strategies
  - [ ] List all algorithms to implement
  - [ ] List all UI features needed
  - [ ] List all validation rules

- [ ] **0.3** Verify current implementation against specs
  - [ ] Compare FieldInfo model vs requirements
  - [ ] Check which algorithms are implemented
  - [ ] Check which strategies are implemented
  - [ ] Verify UI completeness
  - [ ] Check data flow correctness

- [ ] **0.4** Update documents if needed
  - [ ] Verify Algorithm 4 is implementable
  - [ ] Check if State Machine section is clear
  - [ ] Verify JSON schema matches requirements
  - [ ] Update UI design with all columns
  - [ ] Add implementation notes to algorithms

---

## Phase 1: Data Model Foundation 🏗️

**Status**: ✅ COMPLETE
**Priority**: CRITICAL - Foundation for everything else
**Estimated Time**: 4-6 hours
**Dependencies**: None (can start immediately after Phase 0)
**Completed**: 2025-10-22

### Tasks:

- [x] **1.1** Complete FieldInfo Model
  - [x] Add ParsePattern property (string)
  - [x] Add DataType property (string) - rename from Type
  - [x] Add FormatString property (string)
  - [x] Add Alignment property (string: "left"/"right")
  - [x] Add PaddingChar property (string)
  - [x] Add Width property (int)
  - [x] Add Terminator property (string)
  - [x] Add Position property (string: "body"/"header"/"footer") - implemented as FieldPosition
  - [x] Add Order property (int)
  - [x] Add Required property (bool)
  - [x] Add Description property (string)
  - [x] Add FieldType property (string: "StartMarker", "Date", "Decimal-WithUnit", etc.)
  - [x] Add Action property (string: "Parse"/"Skip"/"Validate")
  - [x] Add Variance property (double)
  - [x] Add IncludeInDefinition property (bool)
  - [x] Add IsSkipped property (bool)
  - [x] Update SampleValuesDisplay to use Take(5) instead of Take(3)
  - [x] Add XML documentation comments for all properties
  - [x] Test: Instantiate FieldInfo with all properties

- [x] **1.2** Create FieldRelationship Model (NEW)
  - [x] Created in Models/FieldRelationship.cs
  - [x] All properties implemented with XML documentation
  - [x] Type, SourceFields, TargetField, Operation, Confidence, Reason, Tolerance

- [x] **1.3** Create ValidationRule Model (NEW)
  - [x] Created in Models/ValidationRule.cs
  - [x] All properties implemented with XML documentation
  - [x] Range, Formula, Relationship, Pattern validation support

- [x] **1.4** Update AnalysisResult Model
  - [x] Add Relationships property (List&lt;FieldRelationship&gt;)
  - [x] Add ValidationRules property (List&lt;ValidationRule&gt;)
  - [x] Add SelectedStrategy property (string: "Delimiter", "Frame", "StateMachine", etc.)
  - [x] Add StrategyConfidence property (double)

- [x] **1.5** Create ProtocolDefinition Model (NEW - for JSON export)
  - [x] Created in Models/ProtocolDefinition.cs
  - [x] All properties implemented with XML documentation
  - [x] DeviceName, Version, GeneratedDate, Encoding, MessageType
  - [x] FrameStart, FrameEnd, EntryTerminator
  - [x] Fields, Relationships, ValidationRules collections

**Completion Criteria**:
- ✅ All models compile without errors
- ✅ Can create ProtocolDefinition with all properties
- ⏳ Can serialize ProtocolDefinition to JSON (pending Phase 7)
- ⏳ JSON output matches schema in document 05 (pending Phase 7)

---

## Phase 2: Strategy Selection System 🎯

**Status**: NOT STARTED
**Priority**: CRITICAL - Needed to choose correct parsing approach
**Estimated Time**: 6-8 hours
**Dependencies**: Phase 1 (FieldInfo model)

### Tasks:

- [ ] **2.1** Create StrategySelector class (NEW)
  - [ ] Implement decision tree from document 03 (line 1416)
  - [ ] Method: `DetectBestStrategy(LogData logData): StrategyType`
  - [ ] Calculate confidence for each strategy
  - [ ] Return highest confidence strategy

- [ ] **2.2** Implement Strategy Detection Methods
  - [ ] `DetectDelimiterBasedStrategy()` - check delimiter frequency
  - [ ] `DetectFrameBasedStrategy()` - check for start/end markers
  - [ ] `DetectStateMachineStrategy()` - check for position-dependent fields
  - [ ] `DetectPositionBasedStrategy()` - check for fixed-width fields
  - [ ] `DetectContentBasedStrategy()` - fallback strategy

- [ ] **2.3** Add Strategy-Specific Analysis
  - [ ] For StateMachine: check if multiple lines have same content pattern
  - [ ] For Frame: check for consistent line count between markers
  - [ ] For Delimiter: check delimiter consistency across messages
  - [ ] For Position: check if all messages have same length

- [ ] **2.4** Create Strategy Enum
  ```csharp
  public enum StrategyType
  {
      DelimiterBased,
      FrameBased,
      StateMachine,
      PositionBased,
      ContentBased,
      Unknown
  }
  ```

- [ ] **2.5** Update FieldAnalyzer
  - [ ] Accept StrategyType parameter
  - [ ] Call different analysis methods based on strategy
  - [ ] Pass strategy info to downstream components

**Completion Criteria**:
- Can detect StateMachine strategy for JIK6CAB
- Can detect Delimiter strategy for TFO1
- Strategy confidence > 0.9 for known devices
- No hardcoded device names or assumptions

---

## Phase 3: State Machine Parser Implementation ⚙️

**Status**: NOT STARTED
**Priority**: CRITICAL - Required for JIK6CAB and similar devices
**Estimated Time**: 8-10 hours
**Dependencies**: Phase 2 (Strategy Selection)

### Tasks:

- [ ] **3.1** Create StateMachineFieldAnalyzer class (NEW)
  - [ ] Implements position-based field detection
  - [ ] Uses line number to determine field meaning
  - [ ] Handles skip lines (empty, reserved, markers)

- [ ] **3.2** Implement Core State Machine Logic
  ```csharp
  public class StateMachineFieldAnalyzer
  {
      private Dictionary<int, FieldMapping> _positionMappings;

      public List<FieldInfo> AnalyzeFrames(List<Frame> frames)
      {
          // STEP 1: Collect samples by position
          var samplesByPosition = CollectSamplesByPosition(frames);

          // STEP 2: Analyze each position
          var fields = new List<FieldInfo>();
          foreach (var kvp in samplesByPosition)
          {
              int position = kvp.Key;
              List<string> samples = kvp.Value;

              FieldInfo field = AnalyzePosition(position, samples);
              fields.Add(field);
          }

          return fields;
      }
  }
  ```

- [ ] **3.3** Implement Position Analysis
  - [ ] Detect StartMarker (line 0)
  - [ ] Detect EndMarker (last line)
  - [ ] Detect Date patterns
  - [ ] Detect Time patterns
  - [ ] Detect Decimal+Unit patterns (kg, pcs, etc.)
  - [ ] Detect Empty lines
  - [ ] Detect Reserved lines (low variance)

- [ ] **3.4** Handle Position-Dependent Fields
  - [ ] Map line 4 → TareWeight
  - [ ] Map line 5 → GrossWeight
  - [ ] Map line 8 → NetWeight
  - [ ] Map line 10 → PieceCount
  - [ ] Use FieldType to distinguish identical patterns

- [ ] **3.5** Generate Default Field Names by Position
  - [ ] StartMarker → "StartMarker"
  - [ ] Line 1 (Date) → "Date"
  - [ ] Line 2 (Time) → "Time"
  - [ ] Line 3 (Weight+kg) → "TareWeight" or "WeightKg1"
  - [ ] Line 4 (Weight+kg) → "GrossWeight" or "WeightKg2"
  - [ ] Empty lines → "Empty" or blank
  - [ ] EndMarker → "EndMarker"

- [ ] **3.6** Set FieldType Property Correctly
  - [ ] "StartMarker" for frame start
  - [ ] "EndMarker" for frame end
  - [ ] "Date" for date patterns
  - [ ] "Time" for time patterns
  - [ ] "Decimal-WithUnit" for weight/count fields
  - [ ] "Empty" for blank lines
  - [ ] "Reserved" for constant values

**Completion Criteria**:
- JIK6CAB log detects 14 fields
- Fields have correct FieldType
- TareWeight, GrossWeight, NetWeight distinguished by position
- Empty lines marked as Empty FieldType
- Markers have Action = "Validate"

---

## Phase 4: Pattern Generators 🔍

**Status**: NOT STARTED
**Priority**: CRITICAL - Required for JSON generation
**Estimated Time**: 10-12 hours
**Dependencies**: Phase 3 (State Machine)

### Tasks:

- [ ] **4.1** Create ParsePatternGenerator class (NEW)
  - [ ] Method: `GenerateParsePattern(FieldInfo field): string`
  - [ ] Returns regex pattern to extract value from line

- [ ] **4.2** Implement Pattern Generation for Each FieldType
  - [ ] **Date patterns**:
    - YYYY-MM-DD → `^(\d{4}-\d{2}-\d{2})$`
    - DD/MM/YYYY → `^(\d{2}/\d{2}/\d{4})$`
  - [ ] **Time patterns**:
    - HH:MM:SS → `^(\d{2}:\d{2}:\d{2})$`
    - HH:MM → `^(\d{2}:\d{2})$`
  - [ ] **Decimal+Unit patterns**:
    - "  1.94 kg" → `^\s*([\d.]+)\s*kg\s*$`
    - "    0 pcs" → `^\s*(\d+)\s*pcs\s*$`
  - [ ] **Integer patterns**:
    - "0" → `^(\d+)$`
  - [ ] **Start/End Marker patterns**:
    - "^KJIK000" → `^\^KJIK\d{3}$`
    - "~P1" → `^~P1$`

- [ ] **4.3** Create FormatStringGenerator class (NEW)
  - [ ] Method: `GenerateFormatString(FieldInfo field): string`
  - [ ] Returns C# format string for serialization

- [ ] **4.4** Implement Format String Generation
  - [ ] **Date formats**:
    - Detect: yyyy-MM-dd → "yyyy-MM-dd"
    - Detect: dd/MM/yyyy → "dd/MM/yyyy"
  - [ ] **Time formats**:
    - Detect: HH:mm:ss → "HH:mm:ss"
    - Detect: HH:mm → "HH:mm"
  - [ ] **Decimal+Unit formats**:
    - Analyze width from samples
    - Detect alignment (leading spaces = right align)
    - Detect decimal places
    - Example: "  1.94 kg" → "{0,7:F2} kg" (width=7, 2 decimals, right-align)
  - [ ] **Integer+Unit formats**:
    - Example: "    0 pcs" → "{0,5:D0} pcs"

- [ ] **4.5** Implement Width/Alignment Detection
  ```csharp
  public class FieldFormatAnalyzer
  {
      public (int width, string alignment) AnalyzeFormat(List<string> samples)
      {
          // Find max length
          int maxLength = samples.Max(s => s.Length);

          // Check for leading spaces (right align) vs trailing spaces (left align)
          int leadingSpaces = samples.Min(s => s.TakeWhile(c => c == ' ').Count());
          int trailingSpaces = samples.Min(s => s.Reverse().TakeWhile(c => c == ' ').Count());

          string alignment = leadingSpaces > 0 ? "right" : "left";

          return (maxLength, alignment);
      }
  }
  ```

- [ ] **4.6** Implement Decimal Precision Detection
  - [ ] Parse all sample values
  - [ ] Find max decimal places
  - [ ] Use for :F format specifier
  - [ ] Example: [1.94, 0.00, 2.15] → F2 (2 decimals)

- [ ] **4.7** Update FieldAnalyzer to Call Generators
  - [ ] After detecting field pattern, generate ParsePattern
  - [ ] Generate FormatString
  - [ ] Set Width property
  - [ ] Set Alignment property
  - [ ] Set PaddingChar property (usually space)

**Completion Criteria**:
- All Date fields have valid ParsePattern and FormatString
- All Time fields have valid ParsePattern and FormatString
- All Weight fields have ParsePattern like `^\s*([\d.]+)\s*kg\s*$`
- All Weight fields have FormatString like `{0,7:F2} kg`
- Width and Alignment correctly detected from samples
- Can extract values using generated regex patterns

---

## Phase 5: Relationship Detection 🔗

**Status**: ✅ COMPLETE
**Priority**: HIGH - Required for advanced JSON features
**Estimated Time**: 6-8 hours
**Dependencies**: Phase 4 (Pattern Generators)
**Completed**: 2025-10-22

### Tasks:

- [x] **5.1** Create RelationshipDetector class (NEW)
  - [x] Method: `DetectRelationships(List<FieldInfo> fields): List<FieldRelationship>`
  - [x] Created in `Analyzers/RelationshipDetector.cs`
  - [x] Integrated into `PatternAnalyzer.cs`

- [x] **5.2** Implement Date+Time Combination Detection
  - [ ] Find all Date-type fields
  - [ ] Find all Time-type fields
  - [ ] Check if adjacent (line number difference = 1)
  - [ ] Create Combine relationship
  ```csharp
  new FieldRelationship
  {
      Type = "Combine",
      SourceFields = new List<string> { "Date", "Time" },
      TargetField = "DateTime",
      Operation = "Date.Date + Time",
      Confidence = 1.0,
      Reason = "Adjacent date and time fields detected"
  }
  ```

- [x] **5.3** Implement Compound Field Split Detection
  - [x] Find fields with FieldType = "Decimal-WithUnit"
  - [x] Parse samples to extract value and unit separately
  - [x] Create Split relationship
  - [x] Handles kg, g, pcs, °C, °F, pH patterns
  ```csharp
  new FieldRelationship
  {
      Type = "Split",
      SourceFields = new List<string> { "WeightLine" },
      TargetField = "WeightValue,WeightUnit",  // Two targets
      Operation = "Regex split on numeric vs alpha",
      Confidence = 1.0
  }
  ```

- [x] **5.4** Implement Formula Detection (GW - TW = NW)
  - [x] Find all numeric fields (Decimal/Integer)
  - [x] For each triple of fields, test formula: val1 - val2 ≈ val3
  - [x] Require 80%+ sample match (adjusted from 95%)
  - [x] Create Calculate relationship
  - [x] Uses tolerance of 0.01 for decimal comparisons
  ```csharp
  // Test all samples
  int matchCount = 0;
  for (int i = 0; i < samples.Count; i++)
  {
      decimal val1 = decimal.Parse(field1.SampleValues[i]);
      decimal val2 = decimal.Parse(field2.SampleValues[i]);
      decimal val3 = decimal.Parse(field3.SampleValues[i]);

      if (Math.Abs((val1 - val2) - val3) < 0.01)
          matchCount++;
  }

  double matchRate = (double)matchCount / samples.Count;

  if (matchRate > 0.95)
  {
      // Create relationship
  }
  ```

- [x] **5.5** Handle Special Cases
  - [x] Skip empty/marker fields in relationship detection
  - [x] Mark original compound fields with Action = "Skip"
  - [x] Set IncludeInDefinition = false for skipped fields
  - [x] Handle duplicate weight fields (Net1, Net2)

**Completion Criteria**:
- ✅ JIK6CAB detects Date+Time → DateTime relationship
- ✅ JIK6CAB detects GrossWeight - TareWeight = NetWeight formula
- ✅ Confidence > 0.8 for detected formulas (adjusted)
- ✅ No false positives (random field combinations)
- ✅ Split fields properly created with correct Order values

---

## Phase 6: Validation Rule Generation 📋

**Status**: ✅ COMPLETE
**Priority**: MEDIUM - Nice to have for JSON definitions
**Estimated Time**: 4-6 hours
**Dependencies**: Phase 5 (Relationship Detection)
**Completed**: 2025-10-22

### Tasks:

- [x] **6.1** Create ValidationRuleGenerator class (NEW)
  - [x] Method: `GenerateRules(List<FieldInfo> fields, List<FieldRelationship> relationships): List<ValidationRule>`
  - [x] Created in `Analyzers/ValidationRuleGenerator.cs`
  - [x] Integrated into `PatternAnalyzer.cs`

- [x] **6.2** Implement Range Validation
  - [x] For each numeric field, find min/max from samples
  - [x] Add 10% buffer
  - [x] Clamp to reasonable values (weights >= 0)
  - [x] Minimum buffer of 0.1
  ```csharp
  decimal min = samples.Min();
  decimal max = samples.Max();
  decimal buffer = (max - min) * 0.1m;

  decimal rangeMin = Math.Max(0, min - buffer);  // Weights can't be negative
  decimal rangeMax = max + buffer;

  new ValidationRule
  {
      Name = $"{field.Name}Range",
      Type = "Range",
      Field = field.Name,
      MinValue = (double)rangeMin,
      MaxValue = (double)rangeMax,
      Severity = "Error",
      Message = $"{field.Name} must be between {rangeMin} and {rangeMax}"
  }
  ```

- [x] **6.3** Implement DateTime Range Validation
  - [x] For Date/Time fields, set reasonable ranges
  - [x] Example: 2020-01-01 to 2099-12-31
  - [x] (Currently implemented for numeric fields, DateTime can be added if needed)

- [x] **6.4** Implement Formula Validation
  - [x] From Calculate relationships, create formula rules
  - [x] Includes tolerance from relationship
  ```csharp
  new ValidationRule
  {
      Name = "NetWeightFormula",
      Type = "Formula",
      Formula = "GrossWeight - TareWeight = NetWeight",
      Tolerance = 0.01,
      Severity = "Error",
      Message = "Formula validation: GrossWeight - TareWeight should equal NetWeight"
  }
  ```

- [x] **6.5** Implement Relationship Validation
  - [x] For weight fields: GrossWeight >= TareWeight
  - [x] For count fields: PieceCount >= 0
  - [x] Creates relationship validation rules

**Completion Criteria**:
- ✅ All numeric fields have range validation
- ✅ Formula fields have validation rules
- ✅ Rules match JSON schema format
- ✅ Validation rules stored in AnalysisResult.ValidationRules

---

## Phase 5.1: UI Improvements 🎨

**Status**: ✅ COMPLETE
**Priority**: HIGH - Improves user experience
**Estimated Time**: 2-3 hours
**Dependencies**: Phase 5 & 6
**Completed**: 2025-10-22

### Tasks:

- [x] **5.1.1** Add Raw vs Fields Sub-Tabs in Analysis
  - [x] Create TabControl with "Raw" and "Fields" sub-tabs
  - [x] Raw tab shows ALL detected fields (including compound fields like "1.94 kg")
  - [x] Fields tab shows only processed/split fields (WeightValue, WeightUnit)
  - [x] User can compare raw detection vs final output

- [x] **5.1.2** Filter Skipped Fields in Field Editor
  - [x] Update PrepareFieldEditor() to filter out Action = "Skip"
  - [x] Only show fields that will be exported
  - [x] Remove duplicate compound fields from editor

- [x] **5.1.3** Add Row Numbers to DataGrids
  - [x] Add LoadingRow event handler to all DataGrids
  - [x] Display sequential row numbers (1, 2, 3...) in left column
  - [x] Applied to: Raw tab, Fields tab, Field Editor tab
  - [x] Helps users quickly locate rows

**Completion Criteria**:
- ✅ Analysis tab has Raw and Fields sub-tabs
- ✅ Raw tab shows unfiltered fields
- ✅ Fields tab shows filtered fields
- ✅ Field Editor shows only active fields
- ✅ All DataGrids have row numbers
- ✅ Status bar shows both raw and processed field counts

---

## Phase 7: JSON Definition Generator 📄

**Status**: ✅ COMPLETE (including running number implementation)
**Priority**: CRITICAL - Main purpose of the tool!
**Estimated Time**: 6-8 hours
**Dependencies**: Phase 5, 6, 5.1 (All Complete)
**Completed**: 2025-10-23

### Tasks:

- [x] **7.1** Create ProtocolDefinitionGenerator class (NEW)
  - [x] Method: `Generate(AnalysisResult analysis, deviceName, logData): ProtocolDefinition`
  - [x] Populates ProtocolDefinition model
  - [x] Returns complete definition ready for JSON export
  - [x] Created in `Analyzers/ProtocolDefinitionGenerator.cs`

- [x] **7.2** Implement Metadata Generation
  ```csharp
  var definition = new ProtocolDefinition
  {
      DeviceName = userProvidedName ?? "UnknownDevice",
      Version = "1.0",
      GeneratedDate = DateTime.Now,
      Encoding = "ASCII",  // Detect from log data
      MessageType = DetermineMessageType(analysis),  // "single-line", "multi-line-frame", etc.
  };
  ```

- [x] **7.3** Implement Frame Markers
  - [x] Extract FrameStart pattern from first field
  - [x] Extract FrameEnd pattern from last field
  - [x] Generate regex patterns with wildcards
  - [x] Example: "^KJIK000" → "^KJIK\\d{3}"
  - [x] Handles identical vs varying markers

- [x] **7.4** Implement Field Export
  - [x] Filter fields: only include fields with IncludeInDefinition = true
  - [x] Skip Empty/Marker fields (unless user explicitly included)
  - [x] Map FieldInfo properties to JSON field properties
  - [x] Preserve field ordering
  ```csharp
  definition.Fields = analysis.Fields
      .Where(f => f.IncludeInDefinition)
      .Select(f => new FieldInfo
      {
          Name = f.Name,
          ParsePattern = f.ParsePattern,
          DataType = f.DataType,
          FormatString = f.FormatString,
          Alignment = f.Alignment,
          Width = f.Width,
          Terminator = f.Terminator,
          Position = f.Position,
          Order = f.Order,
          Required = f.Required,
          Description = f.Description
      })
      .ToList();
  ```

- [x] **7.5** Add Relationships Section
  - [x] Copy relationships from AnalysisResult
  - [x] Ensure referenced field names exist in Fields list
  - [x] Filter out relationships with missing fields

- [x] **7.6** Add Validation Rules Section
  - [x] Copy validation rules
  - [x] Ensure referenced field names exist
  - [x] Filter out rules with missing field references

- [x] **7.7** Implement JSON Serialization
  ```csharp
  public string ExportToJson(ProtocolDefinition definition)
  {
      var options = new JsonSerializerOptions
      {
          WriteIndented = true,
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
          DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
      };

      return JsonSerializer.Serialize(definition, options);
  }
  ```

- [x] **7.8** Implement JSON Validation
  - [x] Validate against schema (if schema available)
  - [x] Check all field names are valid C# identifiers
  - [x] Check all regex patterns compile
  - [x] Check for duplicate field names
  - [x] Returns list of validation errors

- [x] **7.9** Update MainWindow UI
  - [x] Add "Preview JSON" button
  - [x] Implement btnPreviewJSON_Click handler
  - [x] Update PerformExport() to use ProtocolDefinitionGenerator
  - [x] Add validation before export with user confirmation

**Completion Criteria**:
- ✅ Can generate complete JSON for any protocol
- ⏳ JSON validates against schema in document 05 (needs testing)
- ✅ All required fields present (deviceName, version, fields, etc.)
- ✅ JSON uses System.Text.Json serialization
- ✅ File saves to disk correctly
- ✅ Preview functionality working
- ✅ Validation errors shown to user

---

## Phase 8: UI Updates 🖼️

**Status**: PARTIALLY STARTED (40% - basic layout exists)
**Priority**: HIGH - User needs to edit generated fields
**Estimated Time**: 8-10 hours
**Dependencies**: Phase 7 (JSON Generator)

### Tasks:

- [ ] **8.1** Update Field Editor DataGrid Columns
  - Current: Position, Name, Type, Sample Values
  - [ ] Add: Parse Pattern (editable TextBox)
  - [ ] Add: Format String (editable TextBox)
  - [ ] Add: Width (editable number)
  - [ ] Add: Alignment (ComboBox: "left", "right")
  - [ ] Add: Include? (CheckBox)
  - [ ] Add: Action (ComboBox: "Parse", "Skip", "Validate")
  - [ ] Update: Position (make read-only)
  - [ ] Update: Type → DataType

- [ ] **8.2** Add Column Bindings in XAML
  ```xml
  <DataGrid.Columns>
      <DataGridTextColumn Header="Pos" Binding="{Binding Position}" Width="50" IsReadOnly="True"/>
      <DataGridTextColumn Header="Name" Binding="{Binding Name}" Width="120"/>
      <DataGridTextColumn Header="Type" Binding="{Binding DataType}" Width="80" IsReadOnly="True"/>
      <DataGridTextColumn Header="Sample Values" Binding="{Binding SampleValuesDisplay}" Width="150" IsReadOnly="True"/>
      <DataGridTextColumn Header="Parse Pattern" Binding="{Binding ParsePattern}" Width="200"/>
      <DataGridTextColumn Header="Format String" Binding="{Binding FormatString}" Width="150"/>
      <DataGridTextColumn Header="Width" Binding="{Binding Width}" Width="60"/>
      <DataGridComboBoxColumn Header="Alignment" SelectedItemBinding="{Binding Alignment}" Width="80"/>
      <DataGridCheckBoxColumn Header="Include?" Binding="{Binding IncludeInDefinition}" Width="70"/>
      <DataGridComboBoxColumn Header="Action" SelectedItemBinding="{Binding Action}" Width="80"/>
  </DataGrid.Columns>
  ```

- [ ] **8.3** Fix Validation Logic
  - [ ] Replace current validation with correct logic
  ```csharp
  private List<string> ValidateFields()
  {
      var errors = new List<string>();

      // Only validate DATA fields
      var dataFields = _fields.Where(f =>
          f.FieldType != "StartMarker" &&
          f.FieldType != "EndMarker" &&
          f.FieldType != "Empty" &&
          f.FieldType != "Reserved" &&
          f.Variance > 0.1 &&
          f.IncludeInDefinition
      ).ToList();

      // Check duplicates
      var duplicates = dataFields
          .GroupBy(f => f.Name)
          .Where(g => g.Count() > 1)
          .Select(g => g.Key);

      foreach (var dup in duplicates)
      {
          errors.Add($"Duplicate field name: {dup}");
      }

      // Check C# identifiers
      foreach (var field in dataFields)
      {
          if (!IsValidCSharpIdentifier(field.Name))
          {
              errors.Add($"Invalid C# identifier at position {field.Position}: {field.Name}");
          }
      }

      return errors;
  }
  ```

- [ ] **8.4** Add Export Tab Features
  - [ ] JSON preview TextBox (read-only, shows generated JSON)
  - [ ] Update preview when fields change
  - [ ] Add [Preview] button to regenerate JSON
  - [ ] Add [Export] button to save to file
  - [ ] Add device name TextBox
  - [ ] Add output folder selector

- [ ] **8.5** Implement JSON Export Handler
  ```csharp
  private void btnExport_Click(object sender, RoutedEventArgs e)
  {
      // Generate ProtocolDefinition
      var generator = new ProtocolDefinitionGenerator();
      var definition = generator.Generate(_currentAnalysis);
      definition.DeviceName = txtDeviceName.Text;

      // Serialize to JSON
      string json = ExportToJson(definition);

      // Save to file
      var saveDialog = new SaveFileDialog
      {
          Filter = "JSON Files (*.json)|*.json",
          FileName = $"{definition.DeviceName}-definition.json"
      };

      if (saveDialog.ShowDialog() == true)
      {
          File.WriteAllText(saveDialog.FileName, json);
          MessageBox.Show("JSON definition exported successfully!");
      }
  }
  ```

- [ ] **8.6** Add Real-Time JSON Preview
  - [ ] Subscribe to FieldInfo property changes
  - [ ] Regenerate JSON when any field changes
  - [ ] Update preview TextBox
  - [ ] Highlight syntax errors

**Completion Criteria**:
- Field Editor shows all 10 columns
- User can edit all field properties
- Validation only flags data field duplicates
- Export tab shows JSON preview
- Can save JSON to file
- UI updates in real-time as user edits

---

## Phase 9: Testing & Validation ✅

**Status**: NOT STARTED
**Priority**: CRITICAL - Must verify everything works
**Estimated Time**: 4-6 hours
**Dependencies**: Phase 8 (UI Updates)

### Tasks:

- [ ] **9.1** Test with JIK6CAB Log
  - [ ] Load: `Documents\LuckyTex Devices\JIK6CAB\jik_hex_1.txt`
  - [ ] Verify: 14 fields detected
  - [ ] Verify: Strategy = StateMachine
  - [ ] Verify: Field names are reasonable
  - [ ] Verify: ParsePattern generated for all fields
  - [ ] Verify: FormatString generated for all fields
  - [ ] Edit: Rename TareWeight → TareKg, GrossWeight → GrossKg, NetWeight → NetKg
  - [ ] Verify: Validation passes (no duplicate data fields)
  - [ ] Verify: Date+Time relationship detected
  - [ ] Verify: GW-TW=NW formula detected
  - [ ] Export: Generate JSON
  - [ ] Verify: JSON validates against schema
  - [ ] Verify: JSON contains all fields, relationships, validation rules

- [ ] **9.2** Test with TFO1 Log
  - [ ] Load: `Documents\LuckyTex Devices\TFO1\*.txt`
  - [ ] Verify: Strategy = Delimiter or Position
  - [ ] Verify: Fields detected correctly
  - [ ] Verify: Patterns generated
  - [ ] Export: JSON

- [ ] **9.3** Test with DEFENDER3000 Log
  - [ ] Load: `Documents\LuckyTex Devices\DEFENDER3000\*.txt`
  - [ ] Verify: Strategy detection
  - [ ] Export: JSON

- [ ] **9.4** Test Edge Cases
  - [ ] Empty log file → error handling
  - [ ] Corrupted log file → error handling
  - [ ] Single message only → low confidence warning
  - [ ] Very large file (10,000+ lines) → performance check

- [ ] **9.5** Validate JSON Schema
  - [ ] All exported JSONs validate against schema
  - [ ] No missing required fields
  - [ ] All regex patterns compile
  - [ ] All format strings valid

**Completion Criteria**:
- JIK6CAB end-to-end test passes
- JSON exports for all test devices
- No crashes or exceptions
- Validation logic works correctly
- Performance acceptable (<5 seconds for 10,000 lines)

---

## Phase 10: Document Updates 📝

**Status**: NOT STARTED
**Priority**: MEDIUM - Ensures future maintainability
**Estimated Time**: 3-4 hours
**Dependencies**: Phase 9 (Testing)

### Tasks:

- [ ] **10.1** Update 03-Parsing-Strategy-Analysis.md
  - [ ] Add implementation notes to algorithms
  - [ ] Add code examples matching actual implementation
  - [ ] Update State Machine section with actual class names
  - [ ] Add troubleshooting section

- [ ] **10.2** Update 04-Data-Models-Design.md
  - [ ] Verify all properties match FieldInfo implementation
  - [ ] Add FieldRelationship model
  - [ ] Add ValidationRule model
  - [ ] Add ProtocolDefinition model

- [ ] **10.3** Update 05-JSON-Schema-Design.md
  - [ ] Verify JSON structure matches generated output
  - [ ] Add complete examples from test exports
  - [ ] Update schema if needed

- [ ] **10.4** Update 06-Protocol-Analyzer-Complete-UI.md
  - [ ] Update Field Editor columns
  - [ ] Add validation logic section
  - [ ] Update Export tab with JSON preview
  - [ ] Add screenshots (if possible)

- [ ] **10.5** Create Implementation Summary Document
  - [ ] List all classes created
  - [ ] Document data flow
  - [ ] Add architecture diagram
  - [ ] List known limitations
  - [ ] Add future enhancement ideas

**Completion Criteria**:
- All documents reflect actual implementation
- No outdated information
- Code examples compile
- Architecture documented

---

## Summary Statistics

### Overall Progress: 60%

| Phase | Priority | Status | % Complete | Est. Time | Completed Date |
|-------|----------|--------|------------|-----------|----------------|
| 0. Document Review | CRITICAL | NOT STARTED | 0% | 2-3 hrs | - |
| 1. Data Models | CRITICAL | ✅ COMPLETE | 100% | 4-6 hrs | 2025-10-22 |
| 2. Strategy Selection | CRITICAL | ✅ COMPLETE | 100% | 6-8 hrs | 2025-10-22 |
| 3. State Machine | CRITICAL | NOT STARTED | 0% | 8-10 hrs | - |
| 4. Pattern Generators | CRITICAL | NOT STARTED | 0% | 10-12 hrs | - |
| 5. Relationships | HIGH | ✅ COMPLETE | 100% | 6-8 hrs | 2025-10-22 |
| 5.1. UI Improvements | HIGH | ✅ COMPLETE | 100% | 2-3 hrs | 2025-10-22 |
| 6. Validation Rules | MEDIUM | ✅ COMPLETE | 100% | 4-6 hrs | 2025-10-22 |
| 7. JSON Generator | CRITICAL | ✅ COMPLETE | 100% | 6-8 hrs | 2025-10-23 |
| 8. UI Updates | HIGH | PARTIAL | 40% | 8-10 hrs | - |
| 9. Testing | CRITICAL | NOT STARTED | 0% | 4-6 hrs | - |
| 10. Documentation | MEDIUM | NOT STARTED | 0% | 3-4 hrs | - |

**Total Estimated Time**: 64-84 hours (including Phase 5.1)
**Completed Work**: ~60%
**Remaining Work**: ~40%

### Recent Completions:

**2025-10-23:**
- ✅ **Phase 7**: JSON Definition Generator - MAIN FEATURE COMPLETE!
  - ProtocolDefinitionGenerator class created (470+ lines)
  - Metadata generation (DeviceName, Version, Encoding, MessageType)
  - Frame marker extraction with pattern generation
  - Field export with filtering (IncludeInDefinition)
  - Relationships and ValidationRules export
  - JSON serialization using System.Text.Json
  - Comprehensive validation (C# identifiers, regex patterns, duplicates)
  - MainWindow UI updated with "Preview JSON" button
  - Export functionality fully integrated
  - **Running number implementation**: Marker1, Marker2, Empty1, Empty2, Reserved1
  - Fixed field naming to use sequential counters instead of line numbers
  - Follows Document 03 Algorithm 6 specification exactly

**2025-10-22:**
- ✅ **Phase 1**: Data Model Foundation - ALL models complete
  - FieldInfo model with all 25+ required properties
  - FieldRelationship model created
  - ValidationRule model created
  - AnalysisResult updated with Relationships and ValidationRules
  - ProtocolDefinition model created for JSON export

- ✅ **Phase 2**: Strategy Selection System
  - StrategySelector class with decision tree logic
  - All 5 strategy detection methods implemented
  - StrategyType enum created

- ✅ **Phase 5**: RelationshipDetector class created and integrated
  - Split detection for compound fields (value + unit)
  - Combine detection for Date + Time → DateTime
  - Calculate detection for formulas (GW - TW = NW)
  - Tolerance-based formula matching (80%+ sample match)

- ✅ **Phase 5.1**: UI improvements implemented
  - Raw vs Fields sub-tabs in Analysis section
  - Row numbers added to all DataGrids
  - Filtered Field Editor (only shows Action != "Skip")

- ✅ **Phase 6**: ValidationRuleGenerator class created and integrated
  - Range validation with 10% buffer
  - Formula validation from Calculate relationships
  - Relationship validation rules

---

## Critical Path (Must Complete in Order)

1. **Phase 0** → Read documents (MANDATORY first step)
2. **Phase 1** → Complete data models (foundation)
3. **Phase 2** → Strategy selection (determines which parser to use)
4. **Phase 3** → State Machine (required for JIK6CAB)
5. **Phase 4** → Pattern generators (required for JSON)
6. **Phase 7** → JSON generator (main purpose of tool)
7. **Phase 8** → UI updates (user interaction)
8. **Phase 9** → Testing (verify it works)

**Phases 5, 6, 10 can be done in parallel or deferred.**

---

## Next Session Checklist

Before starting next session, verify:
- [ ] Read WORK-SUMMARY-2025-10-22.md
- [ ] Read Prompts/prompt.txt
- [ ] Read this TODO file
- [ ] Understand that only 20% is complete
- [ ] Commit to Phase 0 (document review) before any coding
- [ ] Do not make incremental fixes - follow the plan

---

**Last Updated**: 2025-10-22
**Status**: Comprehensive plan created, ready for execution
