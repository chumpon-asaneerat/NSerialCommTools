# How the Protocol Analyzer Infers Properties

**Question:** How does the analyzer know the "Name", "Description", "UnitAttached", "Min", "Max", "Required" when the log data doesn't contain this information?

**Answer:** The analyzer uses **Pattern Recognition & Heuristics** - it looks at the actual data and makes intelligent guesses based on common protocol patterns.

---

## 🔍 How It Works: Step-by-Step

### Example: Analyzing JIK6CAB Log File

**Input:** `jik_hex_1.txt` contains raw data like:
```
^KJIK000
2023-11-07
17:19:26
  0.00 kg
  1.94 kg
0
0
  1.94 kg
  1.94 kg
    0 pcs
(empty line)
(empty line)
E
~P1
```

**Output:** JSON with inferred properties:
```json
{
  "LineNumber": 4,
  "Name": "TareWeight",           ← How?
  "Type": "decimal",              ← How?
  "Pattern": "\\s*(\\d+\\.\\d+)\\s*kg",
  "Unit": "kg",                   ← How?
  "UnitAttached": true,           ← How?
  "Min": 0,                       ← How?
  "Max": 999.99,                  ← How?
  "Required": true,               ← How?
  "Description": "Tare Weight (TW)" ← How?
}
```

---

## 📋 Property Inference Methods

### 1. **Name** - Pattern Matching + Position

The analyzer uses **pattern recognition** to identify what each line represents:

#### A. Marker Detection (Lines 1 & 14)
```csharp
// Line 69-74 in PackageLineAnalyzer.cs
if (lineNumber == 1 && line == packageInfo.StartMarker)
{
    structure.Name = "StartMarker";  // ← Hardcoded name
    structure.Type = "marker";
    structure.Description = "Package start marker with device ID";
    return;
}
```
**Logic:** First line that matches the start marker → Name = "StartMarker"

#### B. Date Pattern Recognition (Line 2)
```csharp
// Line 88-96
if (Regex.IsMatch(line, @"^\d{4}-\d{2}-\d{2}$"))
{
    structure.Name = "Date";  // ← Inferred from pattern
    structure.Type = "datetime";
    structure.Format = "yyyy-MM-dd";
    structure.Description = "Measurement date";
    return;
}
```
**Logic:** Line matches `YYYY-MM-DD` pattern → Name = "Date"

#### C. Time Pattern Recognition (Line 3)
```csharp
// Line 100-108
if (Regex.IsMatch(line, @"^\d{2}:\d{2}:\d{2}$"))
{
    structure.Name = "Time";  // ← Inferred from pattern
    structure.Type = "datetime";
    structure.Format = "HH:mm:ss";
    structure.Description = "Measurement time";
    return;
}
```
**Logic:** Line matches `HH:MM:SS` pattern → Name = "Time"

#### D. Weight Pattern + Position-Based Naming (Lines 4, 5, 8, 9)
```csharp
// Line 112-126
var weightMatch = Regex.Match(line, @"\s*(\d+\.\d+)\s*(kg|g|lb)");
if (weightMatch.Success)
{
    string fieldName = DetermineWeightFieldName(lineNumber);  // ← Position-based
    structure.Name = fieldName;
    structure.Type = "decimal";
    structure.Unit = weightMatch.Groups[2].Value;
    // ...
}

// Line 190-200
private string DetermineWeightFieldName(int lineNumber)
{
    switch (lineNumber)
    {
        case 4: return "TareWeight";      // ← Position 4 = Tare
        case 5: return "GrossWeight";     // ← Position 5 = Gross
        case 8: return "NetWeight";       // ← Position 8 = Net
        case 9: return "DisplayWeight";   // ← Position 9 = Display
        default: return $"Weight{lineNumber}";
    }
}
```

**Logic:**
- Line contains number + unit (kg/g/lb) → It's a weight
- **Position 4** → "TareWeight" (by convention)
- **Position 5** → "GrossWeight" (by convention)
- **Position 8** → "NetWeight" (by convention)
- **Position 9** → "DisplayWeight" (by convention)

⚠️ **This is the key assumption**: The analyzer assumes JIK6CAB follows a standard order (Tare, Gross, Net, Display). This is based on **domain knowledge** of typical scale protocols.

#### E. Piece Count Pattern (Line 10)
```csharp
// Line 130-142
var pcsMatch = Regex.Match(line, @"\s*(\d+)\s*pcs");
if (pcsMatch.Success)
{
    structure.Name = "PieceCount";  // ← Inferred from "pcs" unit
    structure.Type = "integer";
    structure.Unit = "pcs";
    // ...
}
```
**Logic:** Line contains number + "pcs" → Name = "PieceCount"

#### F. Status Indicator (Line 13)
```csharp
// Line 146-154
if (line.Length == 1 && char.IsLetter(line[0]))
{
    structure.Name = "StatusIndicator";  // ← Inferred from single letter
    structure.Type = "string";
    structure.Values = new List<string> { "E", "S", "N" };
    // ...
}
```
**Logic:** Single letter → Name = "StatusIndicator"

#### G. Reserved/Unknown Fields (Lines 6, 7)
```csharp
// Line 158-165
if (Regex.IsMatch(line, @"^\d+$"))
{
    structure.Name = $"Reserved{lineNumber}";  // ← Generic name with position
    structure.Type = "string";
    structure.Description = "Reserved field";
    return;
}
```
**Logic:** Line is just a number → Name = "Reserved{LineNumber}"

---

### 2. **Description** - Hardcoded Based on Name

```csharp
// Line 205-214
private string GetWeightDescription(string fieldName)
{
    switch (fieldName)
    {
        case "TareWeight": return "Tare Weight (TW)";           // ← Hardcoded
        case "GrossWeight": return "Gross Weight (GW)";         // ← Hardcoded
        case "NetWeight": return "Net Weight (NW) = GW - TW";   // ← Hardcoded
        case "DisplayWeight": return "Display weight (duplicate of NW)"; // ← Hardcoded
        default: return "Weight measurement";
    }
}
```

**Logic:** Once the Name is determined, the Description is looked up from a **predefined mapping table**.

---

### 3. **UnitAttached** - Always True for Detected Patterns

```csharp
// Line 120
structure.UnitAttached = true;  // ← Hardcoded to true
```

**Logic:** When the pattern detector finds a unit in the same line as the value (e.g., "0.00 kg"), it sets `UnitAttached = true`.

**Current Limitation:** The analyzer doesn't distinguish between:
- `"0.00 kg"` (attached, space-separated)
- `"0.00kg"` (attached, no space)
- `"0.00" "kg"` (separated in different fields)

All are marked as `UnitAttached = true`.

---

### 4. **Min / Max** - Hardcoded Defaults

```csharp
// Line 122-123
structure.Min = 0;         // ← Hardcoded default
structure.Max = 999.99m;   // ← Hardcoded default
```

**Logic:** Hardcoded reasonable defaults based on field type:
- **Weights (decimal):** Min=0, Max=999.99
- **Piece Count (integer):** Min=0, Max=99999

**Current Limitation:** The analyzer does NOT analyze the actual data to find real min/max values. It just uses defaults.

**What it SHOULD do (future enhancement):**
```csharp
// Analyze all messages to find actual range
structure.Min = allWeightValues.Min();  // e.g., 0.00
structure.Max = allWeightValues.Max();  // e.g., 125.50
```

---

### 5. **Required** - Heuristic Rules

```csharp
// Examples from the code:
structure.Required = true;   // For markers, dates, times, weights
structure.Required = false;  // For reserved fields, empty lines, status
```

**Logic Rules:**
- **Always Required:**
  - Start/End markers (line 72, 82)
  - Date and Time (line 94, 106)
  - Weight values (line 124)

- **Optional (not required):**
  - Reserved fields (line 163)
  - Empty lines (line 174)
  - Status indicators (line 152)
  - Piece count (line 140)

**Heuristic:** Critical protocol elements are marked required, auxiliary data is optional.

---

### 6. **Type** - Pattern-Based Inference

```csharp
// Type inference examples:
if (line matches date pattern)    → Type = "datetime"
if (line matches time pattern)    → Type = "datetime"
if (line matches decimal + unit)  → Type = "decimal"
if (line matches integer + unit)  → Type = "integer"
if (line is single letter)        → Type = "string"
if (line is marker)               → Type = "marker"
```

**Logic:** The data format determines the type.

---

## 🎯 Summary: Where Each Property Comes From

| Property | Source | Method |
|----------|--------|--------|
| **Name** | Pattern + Position | Regex matching + positional lookup table |
| **Description** | Hardcoded | Lookup table based on Name |
| **Type** | Pattern | Regex pattern determines type |
| **Pattern** | Generated | Regex pattern created from detected format |
| **Unit** | Extracted | Captured from regex match (kg, g, pcs) |
| **UnitAttached** | Hardcoded | Always `true` when unit detected in line |
| **Min** | Hardcoded | Default values (0 for weights/counts) |
| **Max** | Hardcoded | Default values (999.99 for weights, 99999 for counts) |
| **Required** | Heuristic | Critical fields = true, auxiliary = false |
| **Format** | Type-based | "F2" for decimals, "yyyy-MM-dd" for dates, etc. |
| **Values** | Hardcoded | Predefined for status ("E", "S", "N") |

---

## ⚠️ Current Limitations

### 1. **Position-Dependent Naming**
The analyzer **assumes** JIK6CAB always has:
- Line 4 = Tare Weight
- Line 5 = Gross Weight
- Line 8 = Net Weight
- Line 9 = Display Weight

**Problem:** If a different device has weights in different positions, the names will be wrong.

**Solution:** Need to analyze the actual data relationships (e.g., detect that line 8 = line 5 - line 4).

---

### 2. **Hardcoded Min/Max Values**
```csharp
structure.Min = 0;         // Always 0
structure.Max = 999.99m;   // Always 999.99
```

**Problem:** Not based on actual data. If real max is 50.0 kg, it still says 999.99.

**Solution:** Analyze all messages and calculate real min/max:
```csharp
var allWeights = messages
    .Select(m => ExtractWeight(m))
    .ToList();

structure.Min = allWeights.Min();  // Real minimum
structure.Max = allWeights.Max();  // Real maximum
```

---

### 3. **Status Values Are Hardcoded**
```csharp
structure.Values = new List<string> { "E", "S", "N" };  // Hardcoded
```

**Problem:** If the log only contains "E", it still lists "S" and "N".

**Solution:** Extract unique values from actual data:
```csharp
var allStatusValues = messages
    .Where(m => m.Length == 1)
    .Select(m => m)
    .Distinct()
    .ToList();

structure.Values = allStatusValues;  // ["E"] if only E appears
```

---

### 4. **No Formula Detection**
The description says "Net Weight (NW) = GW - TW" but this is just **text**.

**Problem:** The analyzer doesn't actually verify that Line8 = Line5 - Line4.

**Solution:** Add validation rule generation:
```json
{
  "validation": {
    "rules": [
      {
        "name": "WeightCalculation",
        "type": "formula",
        "formula": "NetWeight = GrossWeight - TareWeight",
        "tolerance": 0.01,
        "severity": "warning"
      }
    ]
  }
}
```

---

## 🚀 Recommended Improvements

### Phase 1: Data-Driven Min/Max (Week 1, Day 1)
```csharp
public class EnhancedPackageLineAnalyzer
{
    private void AnalyzeWeightRange(LineStructure structure, List<string> allMessages, int lineNumber)
    {
        var weights = allMessages
            .Skip(lineNumber - 1)
            .Where((_, i) => i % packageSize == lineNumber - 1)
            .Select(line => ExtractWeightValue(line))
            .Where(w => w.HasValue)
            .Select(w => w.Value)
            .ToList();

        if (weights.Any())
        {
            structure.Min = weights.Min();  // Real min from data
            structure.Max = weights.Max();  // Real max from data
        }
    }
}
```

### Phase 2: Actual Value Extraction (Week 1, Day 2)
```csharp
private void DetectStatusValues(LineStructure structure, List<string> allMessages, int lineNumber)
{
    var statusValues = allMessages
        .Skip(lineNumber - 1)
        .Where((_, i) => i % packageSize == lineNumber - 1)
        .Where(line => line.Length == 1 && char.IsLetter(line[0]))
        .Distinct()
        .OrderBy(v => v)
        .ToList();

    structure.Values = statusValues;  // Real values: ["E", "S"] (no "N" if not present)
}
```

### Phase 3: Formula Detection (Week 1, Day 3)
```csharp
private void DetectWeightFormulas(List<LineStructure> lines)
{
    var tareIndex = lines.FindIndex(l => l.Name == "TareWeight");
    var grossIndex = lines.FindIndex(l => l.Name == "GrossWeight");
    var netIndex = lines.FindIndex(l => l.Name == "NetWeight");

    if (tareIndex >= 0 && grossIndex >= 0 && netIndex >= 0)
    {
        // Add validation rule
        ValidationRule formula = new ValidationRule
        {
            Name = "NetWeightCalculation",
            Type = "formula",
            Formula = "NetWeight = GrossWeight - TareWeight",
            Tolerance = 0.01m,
            Severity = "warning",
            Message = "Net weight should equal gross minus tare"
        };

        // Add to definition
        AddValidationRule(formula);
    }
}
```

---

## 📖 Key Takeaways

1. **Pattern Recognition:** The analyzer uses regex patterns to identify field types (date, time, weight, count, etc.)

2. **Position-Based Naming:** Field names for weights are determined by line position (4=Tare, 5=Gross, 8=Net)

3. **Hardcoded Defaults:** Min/Max, Required, Values are mostly hardcoded based on field type

4. **Domain Knowledge:** The analyzer has built-in knowledge of common protocol patterns (dates, times, weights)

5. **Limitations:** Does not analyze actual data relationships or calculate real statistics

6. **Future Enhancement:** Should analyze all messages to calculate real min/max, extract actual enum values, and detect formulas

---

## 🔧 How to Improve It

**Current Code Location:**
- `09.App/NLib.Serial.Protocol.Analyzer/Analyzers/PackageLineAnalyzer.cs`

**Files to Create/Modify (Week 1):**
1. `Analyzers/DataStatisticsAnalyzer.cs` - Analyze actual data for min/max/values
2. `Analyzers/FormulaDetector.cs` - Detect mathematical relationships
3. Update `PackageLineAnalyzer.cs` - Use real statistics instead of hardcoded values

**Expected Improvement:**
- **Before:** Min=0, Max=999.99 (hardcoded)
- **After:** Min=0.00, Max=125.50 (from actual data)

- **Before:** Values=["E", "S", "N"] (hardcoded)
- **After:** Values=["E", "S"] (actually present in log)

- **Before:** Description="Net Weight (NW) = GW - TW" (text only)
- **After:** Validation rule verifies the formula actually works

---

**Last Updated:** 2025-10-19
**Next Steps:** Implement data-driven statistics (see QUICK-START-NEXT-STEPS.md, Week 1)
