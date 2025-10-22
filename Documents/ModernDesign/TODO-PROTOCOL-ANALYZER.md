# Protocol Analyzer - Comprehensive TODO List

**Created**: 2025-10-22
**Status**: Implementation 20% complete - needs major redesign
**Priority**: CRITICAL - Main feature (JSON generation) is missing

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

**Status**: IN PROGRESS (25% - basic properties only)
**Priority**: CRITICAL - Foundation for everything else
**Estimated Time**: 4-6 hours
**Dependencies**: None (can start immediately after Phase 0)

### Tasks:

- [ ] **1.1** Complete FieldInfo Model
  - [ ] Add ParsePattern property (string)
  - [ ] Add DataType property (string) - rename from Type
  - [ ] Add FormatString property (string)
  - [ ] Add Alignment property (string: "left"/"right")
  - [ ] Add PaddingChar property (string)
  - [ ] Add Width property (int)
  - [ ] Add Terminator property (string)
  - [ ] Add Position property (string: "body"/"header"/"footer") - change from int
  - [ ] Add Order property (int)
  - [ ] Add Required property (bool)
  - [ ] Add Description property (string)
  - [ ] Add FieldType property (string: "StartMarker", "Date", "Decimal-WithUnit", etc.)
  - [ ] Add Action property (string: "Parse"/"Skip"/"Validate")
  - [ ] Add Variance property (double)
  - [ ] Add IncludeInDefinition property (bool)
  - [ ] Add IsSkipped property (bool)
  - [ ] Update SampleValuesDisplay to use Take(5) instead of Take(3)
  - [ ] Add XML documentation comments for all properties
  - [ ] Test: Instantiate FieldInfo with all properties

- [ ] **1.2** Create FieldRelationship Model (NEW)
  ```csharp
  public class FieldRelationship
  {
      public string Type { get; set; }  // "Combine", "Split", "Calculate"
      public List<string> SourceFields { get; set; }
      public string TargetField { get; set; }
      public string Operation { get; set; }
      public double Confidence { get; set; }
      public string Reason { get; set; }
      public double Tolerance { get; set; }  // For Calculate type
  }
  ```

- [ ] **1.3** Create ValidationRule Model (NEW)
  ```csharp
  public class ValidationRule
  {
      public string Name { get; set; }
      public string Type { get; set; }  // "Range", "DateTime", "Formula", "Relationship"
      public string Field { get; set; }
      public string Severity { get; set; }  // "Error", "Warning"
      public string Message { get; set; }

      // Range validation
      public double? MinValue { get; set; }
      public double? MaxValue { get; set; }

      // Formula validation
      public string Formula { get; set; }
      public double? Tolerance { get; set; }

      // Relationship validation
      public string Condition { get; set; }
  }
  ```

- [ ] **1.4** Update AnalysisResult Model
  - [ ] Add Relationships property (List&lt;FieldRelationship&gt;)
  - [ ] Add ValidationRules property (List&lt;ValidationRule&gt;)
  - [ ] Add SelectedStrategy property (string: "Delimiter", "Frame", "StateMachine", etc.)
  - [ ] Add StrategyConfidence property (double)

- [ ] **1.5** Create ProtocolDefinition Model (NEW - for JSON export)
  ```csharp
  public class ProtocolDefinition
  {
      public string DeviceName { get; set; }
      public string Version { get; set; }
      public DateTime GeneratedDate { get; set; }
      public string Encoding { get; set; }
      public string MessageType { get; set; }
      public string FrameStart { get; set; }
      public string FrameEnd { get; set; }
      public string EntryTerminator { get; set; }
      public List<FieldInfo> Fields { get; set; }
      public List<FieldRelationship> Relationships { get; set; }
      public List<ValidationRule> ValidationRules { get; set; }
  }
  ```

**Completion Criteria**:
- All models compile without errors
- Can create ProtocolDefinition with all properties
- Can serialize ProtocolDefinition to JSON
- JSON output matches schema in document 05

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

**Status**: NOT STARTED
**Priority**: HIGH - Required for advanced JSON features
**Estimated Time**: 6-8 hours
**Dependencies**: Phase 4 (Pattern Generators)

### Tasks:

- [ ] **5.1** Create RelationshipDetector class (NEW)
  - [ ] Method: `DetectRelationships(List<FieldInfo> fields): List<FieldRelationship>`

- [ ] **5.2** Implement Date+Time Combination Detection
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

- [ ] **5.3** Implement Compound Field Split Detection
  - [ ] Find fields with FieldType = "Decimal-WithUnit"
  - [ ] Parse samples to extract value and unit separately
  - [ ] Create Split relationship
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

- [ ] **5.4** Implement Formula Detection (GW - TW = NW)
  - [ ] Find all numeric fields (Decimal/Integer)
  - [ ] For each triple of fields, test formula: val1 - val2 ≈ val3
  - [ ] Require 95%+ sample match
  - [ ] Create Calculate relationship
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

- [ ] **5.5** Handle Special Cases
  - [ ] Skip empty/marker fields in relationship detection
  - [ ] Only detect relationships for fields with IncludeInDefinition = true
  - [ ] Handle duplicate weight fields (Net1, Net2)

**Completion Criteria**:
- JIK6CAB detects Date+Time → DateTime relationship
- JIK6CAB detects GrossWeight - TareWeight = NetWeight formula
- Confidence > 0.95 for detected formulas
- No false positives (random field combinations)

---

## Phase 6: Validation Rule Generation 📋

**Status**: NOT STARTED
**Priority**: MEDIUM - Nice to have for JSON definitions
**Estimated Time**: 4-6 hours
**Dependencies**: Phase 5 (Relationship Detection)

### Tasks:

- [ ] **6.1** Create ValidationRuleGenerator class (NEW)
  - [ ] Method: `GenerateRules(List<FieldInfo> fields, List<FieldRelationship> relationships): List<ValidationRule>`

- [ ] **6.2** Implement Range Validation
  - [ ] For each numeric field, find min/max from samples
  - [ ] Add 10% buffer
  - [ ] Clamp to reasonable values (weights >= 0)
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

- [ ] **6.3** Implement DateTime Range Validation
  - [ ] For Date/Time fields, set reasonable ranges
  - [ ] Example: 2020-01-01 to 2099-12-31

- [ ] **6.4** Implement Formula Validation
  - [ ] From Calculate relationships, create formula rules
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

- [ ] **6.5** Implement Relationship Validation
  - [ ] For weight fields: GrossWeight >= TareWeight
  - [ ] For count fields: PieceCount >= 0

**Completion Criteria**:
- All numeric fields have range validation
- Formula fields have validation rules
- Rules match JSON schema format

---

## Phase 7: JSON Definition Generator 📄

**Status**: NOT STARTED
**Priority**: CRITICAL - Main purpose of the tool!
**Estimated Time**: 6-8 hours
**Dependencies**: Phase 6 (Validation Rules)

### Tasks:

- [ ] **7.1** Create ProtocolDefinitionGenerator class (NEW)
  - [ ] Method: `Generate(AnalysisResult analysis): ProtocolDefinition`
  - [ ] Populates ProtocolDefinition model
  - [ ] Returns complete definition ready for JSON export

- [ ] **7.2** Implement Metadata Generation
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

- [ ] **7.3** Implement Frame Markers
  - [ ] Extract FrameStart pattern from first field
  - [ ] Extract FrameEnd pattern from last field
  - [ ] Generate regex patterns with wildcards
  - [ ] Example: "^KJIK000" → "^KJIK\\d{3}"

- [ ] **7.4** Implement Field Export
  - [ ] Filter fields: only include fields with IncludeInDefinition = true
  - [ ] Skip Empty/Marker fields (unless user explicitly included)
  - [ ] Map FieldInfo properties to JSON field properties
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

- [ ] **7.5** Add Relationships Section
  - [ ] Copy relationships from AnalysisResult
  - [ ] Ensure referenced field names exist in Fields list

- [ ] **7.6** Add Validation Rules Section
  - [ ] Copy validation rules
  - [ ] Ensure referenced field names exist

- [ ] **7.7** Implement JSON Serialization
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

- [ ] **7.8** Implement JSON Validation
  - [ ] Validate against schema (if schema available)
  - [ ] Check all field names are valid C# identifiers
  - [ ] Check all regex patterns compile
  - [ ] Check all format strings are valid

**Completion Criteria**:
- Can generate complete JSON for JIK6CAB
- JSON validates against schema in document 05
- All required fields present (deviceName, version, fields, etc.)
- JSON can be parsed back to ProtocolDefinition
- File saves to disk correctly

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

### Overall Progress: 20%

| Phase | Priority | Status | % Complete | Est. Time |
|-------|----------|--------|------------|-----------|
| 0. Document Review | CRITICAL | NOT STARTED | 0% | 2-3 hrs |
| 1. Data Models | CRITICAL | IN PROGRESS | 25% | 4-6 hrs |
| 2. Strategy Selection | CRITICAL | NOT STARTED | 0% | 6-8 hrs |
| 3. State Machine | CRITICAL | NOT STARTED | 0% | 8-10 hrs |
| 4. Pattern Generators | CRITICAL | NOT STARTED | 0% | 10-12 hrs |
| 5. Relationships | HIGH | NOT STARTED | 0% | 6-8 hrs |
| 6. Validation Rules | MEDIUM | NOT STARTED | 0% | 4-6 hrs |
| 7. JSON Generator | CRITICAL | NOT STARTED | 0% | 6-8 hrs |
| 8. UI Updates | HIGH | PARTIAL | 40% | 8-10 hrs |
| 9. Testing | CRITICAL | NOT STARTED | 0% | 4-6 hrs |
| 10. Documentation | MEDIUM | NOT STARTED | 0% | 3-4 hrs |

**Total Estimated Time**: 61-81 hours
**Remaining Work**: ~80%

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
