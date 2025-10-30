using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using NLib.Serial.Protocol.Analyzer.Models;
using NLib.Serial.Protocol.Analyzer.Analyzers;

namespace NLib.Serial.Protocol.Analyzer.Pages
{
    /// <summary>
    /// Page 1: Log Data and Detection Configuration
    /// Purpose: Load log files, auto-detect protocol parameters, configure detection settings
    /// </summary>
    public partial class LogDataPage : UserControl
    {
        private ProtocolAnalyzerModel _model;

        /// <summary>
        /// Initializes a new instance of the LogDataPage
        /// </summary>
        public LogDataPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Setup method to inject the shared model
        /// Called by MainWindow during initialization
        /// </summary>
        /// <param name="model">The shared ProtocolAnalyzerModel instance</param>
        public void Setup(ProtocolAnalyzerModel model)
        {
            _model = model;

            // Phase 3.4 - Wire up event handlers
            WireUpEventHandlers();

            // Phase 3.3/3.4 - Bind DataGrid to model's LogFile.Entries
            if (_model.LogFile != null)
            {
                LogEntriesDataGrid.ItemsSource = _model.LogFile.Entries;
            }
        }

        /// <summary>
        /// Wire up all event handlers for controls
        /// </summary>
        private void WireUpEventHandlers()
        {
            // Package Start Marker radio button handlers
            StartMarkerAutoRadio.Checked += StartMarkerMode_Changed;
            StartMarkerManualRadio.Checked += StartMarkerMode_Changed;
            StartMarkerNoneRadio.Checked += StartMarkerMode_Changed;

            // Package End Marker radio button handlers
            EndMarkerAutoRadio.Checked += EndMarkerMode_Changed;
            EndMarkerManualRadio.Checked += EndMarkerMode_Changed;
            EndMarkerNoneRadio.Checked += EndMarkerMode_Changed;

            // Segment Separator radio button handlers
            SeparatorAutoRadio.Checked += SeparatorMode_Changed;
            SeparatorManualRadio.Checked += SeparatorMode_Changed;
            SeparatorNoneRadio.Checked += SeparatorMode_Changed;

            // Encoding radio button handlers
            EncodingAutoRadio.Checked += EncodingMode_Changed;
            EncodingManualRadio.Checked += EncodingMode_Changed;

            // Detection Configuration button handlers
            ApplyConfigButton.Click += ApplyConfiguration_Click;
            ClearConfigButton.Click += ClearConfiguration_Click;

            // Log Data toolbar button handlers
            LoadLogFileButton.Click += LoadLogFile_Click;
            ClearLogButton.Click += ClearLog_Click;
        }

        // Phase 3.4 - Event Handlers

        /// <summary>
        /// Handle Package Start Marker mode changes
        /// </summary>
        private void StartMarkerMode_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            if (StartMarkerAutoRadio.IsChecked == true)
            {
                StartMarkerTextBox.IsEnabled = false;
                StartMarkerDetectedLabel.Visibility = System.Windows.Visibility.Visible;
            }
            else if (StartMarkerManualRadio.IsChecked == true)
            {
                StartMarkerTextBox.IsEnabled = true;
                StartMarkerDetectedLabel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else // None
            {
                StartMarkerTextBox.IsEnabled = false;
                StartMarkerDetectedLabel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handle Package End Marker mode changes
        /// </summary>
        private void EndMarkerMode_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            if (EndMarkerAutoRadio.IsChecked == true)
            {
                EndMarkerTextBox.IsEnabled = false;
                EndMarkerDetectedLabel.Visibility = System.Windows.Visibility.Visible;
            }
            else if (EndMarkerManualRadio.IsChecked == true)
            {
                EndMarkerTextBox.IsEnabled = true;
                EndMarkerDetectedLabel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else // None
            {
                EndMarkerTextBox.IsEnabled = false;
                EndMarkerDetectedLabel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handle Segment Separator mode changes
        /// </summary>
        private void SeparatorMode_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SeparatorAutoRadio.IsChecked == true)
            {
                SeparatorTextBox.IsEnabled = false;
                SeparatorDetectedLabel.Visibility = System.Windows.Visibility.Visible;
            }
            else if (SeparatorManualRadio.IsChecked == true)
            {
                SeparatorTextBox.IsEnabled = true;
                SeparatorDetectedLabel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else // None
            {
                SeparatorTextBox.IsEnabled = false;
                SeparatorDetectedLabel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handle Encoding mode changes
        /// </summary>
        private void EncodingMode_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            if (EncodingAutoRadio.IsChecked == true)
            {
                EncodingComboBox.IsEnabled = false;
                EncodingDetectedLabel.Visibility = System.Windows.Visibility.Visible;
            }
            else // Manual
            {
                EncodingComboBox.IsEnabled = true;
                EncodingDetectedLabel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Apply detection configuration to the model
        /// </summary>
        private void ApplyConfiguration_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                ApplyConfigurationToModel();

                System.Windows.MessageBox.Show(
                    "Detection configuration has been applied to the model.\n\n" +
                    "You can now proceed to the Analyzer page to view the detected package structure.",
                    "Configuration Applied",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Error applying configuration:\n{ex.Message}",
                    "Configuration Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Clear all detection configuration
        /// </summary>
        private void ClearConfiguration_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Reset all radio buttons to Auto/default state
            StartMarkerAutoRadio.IsChecked = true;
            EndMarkerAutoRadio.IsChecked = true;
            SeparatorAutoRadio.IsChecked = true;
            EncodingAutoRadio.IsChecked = true;

            // Clear text boxes
            StartMarkerTextBox.Text = string.Empty;
            EndMarkerTextBox.Text = string.Empty;
            SeparatorTextBox.Text = string.Empty;

            // Reset labels
            StartMarkerDetectedLabel.Text = "(Auto-detected: ...)";
            EndMarkerDetectedLabel.Text = "(Auto-detected: ...)";
            SeparatorDetectedLabel.Text = "(Auto-detected: ...)";
            EncodingDetectedLabel.Text = "(Auto-detected: ...)";

            // Reset ComboBox to default
            EncodingComboBox.SelectedIndex = 0; // ASCII
        }

        /// <summary>
        /// Load a log file for analysis
        /// </summary>
        private void LoadLogFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select Log File",
                Filter = "Log Files (*.log;*.txt)|*.log;*.txt|All Files (*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    LoadLogFile(openFileDialog.FileName);
                }
                catch (System.Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        $"Error loading log file:\n{ex.Message}",
                        "Load Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);

                    FileInfoLabel.Text = "No file loaded";
                }
            }
        }

        /// <summary>
        /// Clear the loaded log data
        /// </summary>
        private void ClearLog_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Clear the DataGrid
            LogEntriesDataGrid.ItemsSource = null;

            // Reset file info label
            FileInfoLabel.Text = "No file loaded";

            // Clear the model's log file if it exists
            if (_model != null && _model.LogFile != null)
            {
                _model.LogFile.Entries.Clear();
                _model.LogFile.FilePath = string.Empty;
            }

            // Hide detection summary
            DetectionSummaryExpander.Visibility = System.Windows.Visibility.Collapsed;
        }

        // Phase 3.5 - Core Method Implementations

        /// <summary>
        /// Load and parse a log file
        /// </summary>
        /// <param name="filePath">Path to the log file</param>
        private void LoadLogFile(string filePath)
        {
            FileInfoLabel.Text = "Loading...";

            // Unbind DataGrid to prevent ItemsControl inconsistency error
            LogEntriesDataGrid.ItemsSource = null;

            // Clear existing entries
            _model.LogFile.Entries.Clear();

            // Read ENTIRE file as continuous byte stream (NOT split by lines!)
            // This is CRITICAL for proper package boundary detection
            // The analyzer will split into packages based on detected start/end markers
            byte[] allBytes = System.IO.File.ReadAllBytes(filePath);

            // Create ONE LogEntry with all bytes
            LogEntry entry = new LogEntry
            {
                EntryNumber = 1,
                Timestamp = System.DateTime.Now,
                Direction = "RX",
                RawBytes = allBytes // Entire file as raw bytes - NO line splitting!
            };

            // Add to model
            _model.LogFile.Entries.Add(entry);

            // Rebind DataGrid after all entries are loaded
            LogEntriesDataGrid.ItemsSource = _model.LogFile.Entries;

            // Update model file path
            _model.LogFile.FilePath = filePath;

            // Update file info label
            string fileName = System.IO.Path.GetFileName(filePath);
            long fileSize = allBytes.Length;
            string sizeText = fileSize < 1024 ? $"{fileSize} bytes"
                            : fileSize < 1024 * 1024 ? $"{fileSize / 1024.0:F1} KB"
                            : $"{fileSize / (1024.0 * 1024.0):F1} MB";
            FileInfoLabel.Text = $"{fileName} - {sizeText} (1 entry = full file)";

            // Run auto-detection if in Auto mode
            if (StartMarkerAutoRadio.IsChecked == true ||
                EndMarkerAutoRadio.IsChecked == true ||
                SeparatorAutoRadio.IsChecked == true ||
                EncodingAutoRadio.IsChecked == true)
            {
                AutoDetectDelimiters();
            }
        }

        /// <summary>
        /// Auto-detect package markers, separators, and encoding using the analyzer
        /// </summary>
        private void AutoDetectDelimiters()
        {
            List<LogEntry> entries = _model.LogFile.Entries.ToList();
            int totalEntries = entries.Count;

            // Algorithm 1: Detect Package Start Marker
            byte[] startMarker = null;
            int startMarkerCount = 0;
            if (StartMarkerAutoRadio.IsChecked == true)
            {
                startMarker = _model.Analyzer.DetectPackageStartMarker(entries);
                if (startMarker != null)
                {
                    string hexValue = System.BitConverter.ToString(startMarker).Replace("-", " ");
                    StartMarkerDetectedLabel.Text = $"(Auto-detected: {hexValue})";
                    _model.DetectionConfig.PackageStartMarker.SetAutoDetected(hexValue);

                    // Calculate frequency for statistics
                    startMarkerCount = CountOccurrences(entries, startMarker, true);
                    double frequency = (double)startMarkerCount / totalEntries * 100;
                    StartMarkerStatsText.Text = $"{hexValue} - Found in {startMarkerCount}/{totalEntries} entries ({frequency:F1}% frequency)";
                }
                else
                {
                    StartMarkerDetectedLabel.Text = "(Auto-detected: None found)";
                    StartMarkerStatsText.Text = "None detected (< 30% frequency threshold)";
                }
            }

            // Algorithm 2: Detect Package End Marker
            byte[] endMarker = null;
            int endMarkerCount = 0;
            if (EndMarkerAutoRadio.IsChecked == true)
            {
                endMarker = _model.Analyzer.DetectPackageEndMarker(entries);
                if (endMarker != null)
                {
                    string hexValue = System.BitConverter.ToString(endMarker).Replace("-", " ");
                    EndMarkerDetectedLabel.Text = $"(Auto-detected: {hexValue})";
                    _model.DetectionConfig.PackageEndMarker.SetAutoDetected(hexValue);

                    // Calculate frequency for statistics
                    endMarkerCount = CountOccurrences(entries, endMarker, false);
                    double frequency = (double)endMarkerCount / totalEntries * 100;
                    string textRepr = GetTextRepresentation(endMarker);
                    EndMarkerStatsText.Text = $"{hexValue} ({textRepr}) - Found in {endMarkerCount}/{totalEntries} entries ({frequency:F1}% frequency)";
                }
                else
                {
                    EndMarkerDetectedLabel.Text = "(Auto-detected: None found)";
                    EndMarkerStatsText.Text = "None detected (< 30% frequency threshold)";
                }
            }

            // Algorithm 3: Detect Segment Separator
            byte[] separator = null;
            if (SeparatorAutoRadio.IsChecked == true)
            {
                separator = _model.Analyzer.DetectSegmentSeparator(entries);
                if (separator != null)
                {
                    string hexValue = System.BitConverter.ToString(separator).Replace("-", " ");
                    SeparatorDetectedLabel.Text = $"(Auto-detected: {hexValue})";
                    _model.DetectionConfig.SegmentSeparator.SetAutoDetected(hexValue);

                    // Calculate internal occurrences for statistics
                    int totalOccurrences = CountInternalOccurrences(entries, separator);
                    string textRepr = GetTextRepresentation(separator);
                    SeparatorStatsText.Text = $"{hexValue} ({textRepr}) - Found {totalOccurrences} times within entries (> 20% frequency threshold)";
                }
                else
                {
                    SeparatorDetectedLabel.Text = "(Auto-detected: None found)";
                    SeparatorStatsText.Text = "None detected (< 20% frequency threshold)";
                }
            }

            // Algorithm 4: Detect Encoding
            if (EncodingAutoRadio.IsChecked == true)
            {
                EncodingType encoding = _model.Analyzer.DetectEncoding(entries);
                EncodingDetectedLabel.Text = $"(Auto-detected: {encoding})";
                _model.DetectionConfig.Encoding.SetAutoDetected(encoding.ToString());

                EncodingStatsText.Text = $"{encoding} - Selected based on character validity analysis (> 95% valid characters)";
            }

            // Determine Protocol Type
            string protocolType = DetermineProtocolType(startMarker, endMarker, separator);
            ProtocolTypeText.Text = protocolType;

            // Calculate Overall Confidence
            double overallConfidence = CalculateOverallConfidence(startMarker, endMarker, separator, startMarkerCount, endMarkerCount, totalEntries);
            OverallConfidenceText.Text = $"{overallConfidence:F0}%";
            OverallConfidenceBar.Value = overallConfidence;

            // Show and expand the detection summary
            DetectionSummaryExpander.Visibility = System.Windows.Visibility.Visible;
            DetectionSummaryExpander.IsExpanded = true;
        }

        /// <summary>
        /// Count occurrences of a byte sequence at start or end of entries
        /// </summary>
        private int CountOccurrences(List<LogEntry> entries, byte[] sequence, bool atStart)
        {
            int count = 0;
            foreach (var entry in entries)
            {
                if (entry.RawBytes == null || entry.RawBytes.Length < sequence.Length)
                    continue;

                bool matches = true;
                if (atStart)
                {
                    // Check at start
                    for (int i = 0; i < sequence.Length; i++)
                    {
                        if (entry.RawBytes[i] != sequence[i])
                        {
                            matches = false;
                            break;
                        }
                    }
                }
                else
                {
                    // Check at end
                    int offset = entry.RawBytes.Length - sequence.Length;
                    for (int i = 0; i < sequence.Length; i++)
                    {
                        if (entry.RawBytes[offset + i] != sequence[i])
                        {
                            matches = false;
                            break;
                        }
                    }
                }

                if (matches)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Count total occurrences of a byte sequence within entries (not at ends)
        /// </summary>
        private int CountInternalOccurrences(List<LogEntry> entries, byte[] sequence)
        {
            int total = 0;
            foreach (var entry in entries)
            {
                if (entry.RawBytes == null || entry.RawBytes.Length < sequence.Length)
                    continue;

                // Search within entry (skip first and last 25% to exclude markers)
                int skipStart = entry.RawBytes.Length / 4;
                int skipEnd = entry.RawBytes.Length - (entry.RawBytes.Length / 4);

                for (int i = skipStart; i <= skipEnd - sequence.Length; i++)
                {
                    bool matches = true;
                    for (int j = 0; j < sequence.Length; j++)
                    {
                        if (entry.RawBytes[i + j] != sequence[j])
                        {
                            matches = false;
                            break;
                        }
                    }
                    if (matches)
                        total++;
                }
            }
            return total;
        }

        /// <summary>
        /// Get text representation of byte sequence for display
        /// </summary>
        private string GetTextRepresentation(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return "";

            // Check for common sequences
            if (bytes.Length == 2 && bytes[0] == 0x0D && bytes[1] == 0x0A)
                return "CRLF \\r\\n";
            if (bytes.Length == 1 && bytes[0] == 0x0A)
                return "LF \\n";
            if (bytes.Length == 1 && bytes[0] == 0x0D)
                return "CR \\r";
            if (bytes.Length == 1 && bytes[0] == 0x20)
                return "Space";
            if (bytes.Length == 1 && bytes[0] == 0x09)
                return "Tab \\t";
            if (bytes.Length == 1 && bytes[0] == 0x2C)
                return "Comma";

            // Try ASCII conversion for printable characters
            string text = System.Text.Encoding.ASCII.GetString(bytes);
            if (text.All(c => c >= 0x20 && c <= 0x7E))
                return $"\"{text}\"";

            return "binary";
        }

        /// <summary>
        /// Determine protocol type based on detected markers
        /// </summary>
        private string DetermineProtocolType(byte[] startMarker, byte[] endMarker, byte[] separator)
        {
            bool hasStartMarker = startMarker != null;
            bool hasEndMarker = endMarker != null;
            bool hasSeparator = separator != null;

            if (hasStartMarker && hasSeparator)
                return "PackageBased with Segments (Multi-segment protocol)";
            else if (hasStartMarker && !hasSeparator)
                return "PackageBased without Segments (Frame protocol)";
            else if (hasEndMarker && hasSeparator)
                return "SinglePackage with Segments (Terminated multi-field protocol)";
            else if (hasEndMarker && !hasSeparator)
                return "SinglePackage without Segments (Simple terminated protocol)";
            else
                return "Unknown (Insufficient markers detected)";
        }

        /// <summary>
        /// Calculate overall confidence score based on detection results
        /// </summary>
        private double CalculateOverallConfidence(byte[] startMarker, byte[] endMarker, byte[] separator,
                                                  int startCount, int endCount, int totalEntries)
        {
            double startConfidence = startMarker != null ? (double)startCount / totalEntries * 100 : 0;
            double endConfidence = endMarker != null ? (double)endCount / totalEntries * 100 : 0;
            double separatorConfidence = separator != null ? 80.0 : 0; // Separator confidence is harder to measure

            // Average of detected items
            int detectedCount = (startMarker != null ? 1 : 0) + (endMarker != null ? 1 : 0) + (separator != null ? 1 : 0);
            if (detectedCount == 0)
                return 0;

            double totalConfidence = startConfidence + endConfidence + separatorConfidence;
            return totalConfidence / detectedCount;
        }

        /// <summary>
        /// Apply the selected detection configuration to the model
        /// </summary>
        private void ApplyConfigurationToModel()
        {
            // Apply Package Start Marker
            if (StartMarkerAutoRadio.IsChecked == true)
            {
                _model.DetectionConfig.PackageStartMarker.Mode = DetectionMode.Auto;
            }
            else if (StartMarkerManualRadio.IsChecked == true)
            {
                _model.DetectionConfig.PackageStartMarker.Mode = DetectionMode.Manual;
                // Store manual value as hex string (e.g., "0D 0A")
                if (!string.IsNullOrWhiteSpace(StartMarkerTextBox.Text))
                {
                    _model.DetectionConfig.PackageStartMarker.ManualValue = StartMarkerTextBox.Text;
                }
            }
            else
            {
                _model.DetectionConfig.PackageStartMarker.Mode = DetectionMode.None;
            }

            // Apply Package End Marker
            if (EndMarkerAutoRadio.IsChecked == true)
            {
                _model.DetectionConfig.PackageEndMarker.Mode = DetectionMode.Auto;
            }
            else if (EndMarkerManualRadio.IsChecked == true)
            {
                _model.DetectionConfig.PackageEndMarker.Mode = DetectionMode.Manual;
                if (!string.IsNullOrWhiteSpace(EndMarkerTextBox.Text))
                {
                    _model.DetectionConfig.PackageEndMarker.ManualValue = EndMarkerTextBox.Text;
                }
            }
            else
            {
                _model.DetectionConfig.PackageEndMarker.Mode = DetectionMode.None;
            }

            // Apply Segment Separator
            if (SeparatorAutoRadio.IsChecked == true)
            {
                _model.DetectionConfig.SegmentSeparator.Mode = DetectionMode.Auto;
            }
            else if (SeparatorManualRadio.IsChecked == true)
            {
                _model.DetectionConfig.SegmentSeparator.Mode = DetectionMode.Manual;
                if (!string.IsNullOrWhiteSpace(SeparatorTextBox.Text))
                {
                    _model.DetectionConfig.SegmentSeparator.ManualValue = SeparatorTextBox.Text;
                }
            }
            else
            {
                _model.DetectionConfig.SegmentSeparator.Mode = DetectionMode.None;
            }

            // Apply Encoding
            if (EncodingAutoRadio.IsChecked == true)
            {
                _model.DetectionConfig.Encoding.Mode = DetectionMode.Auto;
            }
            else
            {
                _model.DetectionConfig.Encoding.Mode = DetectionMode.Manual;
                // Get selected encoding from ComboBox
                EncodingType selectedEncoding = EncodingType.ASCII;
                if (EncodingComboBox.SelectedIndex == 1) selectedEncoding = EncodingType.UTF8;
                else if (EncodingComboBox.SelectedIndex == 2) selectedEncoding = EncodingType.UTF16;
                else if (EncodingComboBox.SelectedIndex == 3) selectedEncoding = EncodingType.Latin1;

                _model.DetectionConfig.Encoding.ManualValue = selectedEncoding.ToString();
            }
        }

        /// <summary>
        /// Parse hex string (e.g., "0D 0A" or "0D0A") to byte array
        /// </summary>
        private byte[] ParseHexString(string hex)
        {
            // Remove spaces and convert to byte array
            hex = hex.Replace(" ", "").Replace("-", "");

            if (hex.Length % 2 != 0)
            {
                throw new System.FormatException("Hex string must have even number of characters");
            }

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = System.Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        // Note: Auto-detection algorithms moved to LogFileAnalyzer class (Analyzers/LogFileAnalyzer.cs)
        // This separation of concerns keeps UI logic separate from business logic
    }
}
