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
        private LogFileAnalyzer _analyzer;

        /// <summary>
        /// Initializes a new instance of the LogDataPage
        /// </summary>
        public LogDataPage()
        {
            InitializeComponent();
            _analyzer = new LogFileAnalyzer();
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

            // TODO: Phase 3.4 - Initialize UI from model if data exists
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
        }

        // Phase 3.5 - Core Method Implementations

        /// <summary>
        /// Load and parse a log file
        /// </summary>
        /// <param name="filePath">Path to the log file</param>
        private void LoadLogFile(string filePath)
        {
            FileInfoLabel.Text = "Loading...";

            // Clear existing entries
            _model.LogFile.Entries.Clear();

            // Read all lines from the file
            string[] lines = System.IO.File.ReadAllLines(filePath);

            // Parse each line into a LogEntry
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // Create a LogEntry
                LogEntry entry = new LogEntry
                {
                    EntryNumber = i + 1,
                    Timestamp = System.DateTime.Now, // Placeholder - real logs may have timestamps
                    Direction = "RX", // Placeholder - real logs may have direction markers
                    RawBytes = System.Text.Encoding.ASCII.GetBytes(line) // Convert line to bytes
                };

                // Add to model (RawHex and RawText are computed properties)
                _model.LogFile.Entries.Add(entry);
            }

            // Update model file path
            _model.LogFile.FilePath = filePath;

            // Update file info label
            string fileName = System.IO.Path.GetFileName(filePath);
            FileInfoLabel.Text = $"{fileName} - {lines.Length:N0} entries";

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
            // Algorithm 1: Detect Package Start Marker
            if (StartMarkerAutoRadio.IsChecked == true)
            {
                byte[] startMarker = _analyzer.DetectPackageStartMarker(_model.LogFile.Entries.ToList());
                if (startMarker != null)
                {
                    string hexValue = System.BitConverter.ToString(startMarker).Replace("-", " ");
                    StartMarkerDetectedLabel.Text = $"(Auto-detected: {hexValue})";
                    _model.DetectionConfig.PackageStartMarker.AutoDetectedValue = startMarker;
                }
                else
                {
                    StartMarkerDetectedLabel.Text = "(Auto-detected: None found)";
                }
            }

            // Algorithm 2: Detect Package End Marker
            if (EndMarkerAutoRadio.IsChecked == true)
            {
                byte[] endMarker = _analyzer.DetectPackageEndMarker(_model.LogFile.Entries.ToList());
                if (endMarker != null)
                {
                    string hexValue = System.BitConverter.ToString(endMarker).Replace("-", " ");
                    EndMarkerDetectedLabel.Text = $"(Auto-detected: {hexValue})";
                    _model.DetectionConfig.PackageEndMarker.AutoDetectedValue = endMarker;
                }
                else
                {
                    EndMarkerDetectedLabel.Text = "(Auto-detected: None found)";
                }
            }

            // Algorithm 3: Detect Segment Separator
            if (SeparatorAutoRadio.IsChecked == true)
            {
                byte[] separator = _analyzer.DetectSegmentSeparator(_model.LogFile.Entries.ToList());
                if (separator != null)
                {
                    string hexValue = System.BitConverter.ToString(separator).Replace("-", " ");
                    SeparatorDetectedLabel.Text = $"(Auto-detected: {hexValue})";
                    _model.DetectionConfig.SegmentSeparator.AutoDetectedValue = separator;
                }
                else
                {
                    SeparatorDetectedLabel.Text = "(Auto-detected: None found)";
                }
            }

            // Algorithm 4: Detect Encoding
            if (EncodingAutoRadio.IsChecked == true)
            {
                EncodingType encoding = _analyzer.DetectEncoding(_model.LogFile.Entries.ToList());
                EncodingDetectedLabel.Text = $"(Auto-detected: {encoding})";
                _model.DetectionConfig.Encoding.AutoDetectedValue = new byte[] { (byte)encoding };
            }
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
                // Parse manual value from TextBox (hex format)
                if (!string.IsNullOrWhiteSpace(StartMarkerTextBox.Text))
                {
                    _model.DetectionConfig.PackageStartMarker.ManualValue = ParseHexString(StartMarkerTextBox.Text);
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
                    _model.DetectionConfig.PackageEndMarker.ManualValue = ParseHexString(EndMarkerTextBox.Text);
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
                    _model.DetectionConfig.SegmentSeparator.ManualValue = ParseHexString(SeparatorTextBox.Text);
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

                _model.DetectionConfig.Encoding.ManualValue = new byte[] { (byte)selectedEncoding };
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

        // Phase 3.6 - Auto-Detection Algorithms

        /// <summary>
        /// Auto-detect package start marker using frequency analysis
        /// </summary>
        private byte[] DetectPackageStartMarker(System.Collections.Generic.List<LogEntry> entries)
        {
            if (entries == null || entries.Count < 5)
                return null; // Need minimum sample size

            // Dictionary to track frequency of byte sequences at the start
            var sequenceFrequency = new System.Collections.Generic.Dictionary<string, int>();

            // Analyze 1-4 byte sequences at the beginning of each entry
            foreach (var entry in entries)
            {
                if (entry.RawBytes == null || entry.RawBytes.Length == 0)
                    continue;

                // Try 1-byte, 2-byte, 3-byte, and 4-byte sequences
                for (int seqLength = 1; seqLength <= 4 && seqLength <= entry.RawBytes.Length; seqLength++)
                {
                    byte[] sequence = new byte[seqLength];
                    System.Array.Copy(entry.RawBytes, 0, sequence, 0, seqLength);

                    // Use hex string as dictionary key
                    string key = System.BitConverter.ToString(sequence);

                    if (!sequenceFrequency.ContainsKey(key))
                        sequenceFrequency[key] = 0;

                    sequenceFrequency[key]++;
                }
            }

            // Find sequence with highest frequency
            string mostCommonKey = null;
            int maxCount = 0;

            foreach (var kvp in sequenceFrequency)
            {
                if (kvp.Value > maxCount)
                {
                    maxCount = kvp.Value;
                    mostCommonKey = kvp.Key;
                }
            }

            // Check if frequency meets threshold (30% of entries)
            double threshold = 0.30;
            double frequency = (double)maxCount / entries.Count;

            if (frequency >= threshold && mostCommonKey != null)
            {
                // Convert hex string back to byte array
                string[] hexBytes = mostCommonKey.Split('-');
                byte[] result = new byte[hexBytes.Length];

                for (int i = 0; i < hexBytes.Length; i++)
                {
                    result[i] = System.Convert.ToByte(hexBytes[i], 16);
                }

                return result;
            }

            return null; // No consistent start marker found
        }

        /// <summary>
        /// Auto-detect package end marker using frequency analysis
        /// </summary>
        private byte[] DetectPackageEndMarker(System.Collections.Generic.List<LogEntry> entries)
        {
            if (entries == null || entries.Count < 5)
                return null; // Need minimum sample size

            // Dictionary to track frequency of byte sequences at the end
            var sequenceFrequency = new System.Collections.Generic.Dictionary<string, int>();

            // Analyze 1-4 byte sequences at the end of each entry
            foreach (var entry in entries)
            {
                if (entry.RawBytes == null || entry.RawBytes.Length == 0)
                    continue;

                // Try 1-byte, 2-byte, 3-byte, and 4-byte sequences from the end
                for (int seqLength = 1; seqLength <= 4 && seqLength <= entry.RawBytes.Length; seqLength++)
                {
                    byte[] sequence = new byte[seqLength];
                    int startPos = entry.RawBytes.Length - seqLength;
                    System.Array.Copy(entry.RawBytes, startPos, sequence, 0, seqLength);

                    // Use hex string as dictionary key
                    string key = System.BitConverter.ToString(sequence);

                    if (!sequenceFrequency.ContainsKey(key))
                        sequenceFrequency[key] = 0;

                    sequenceFrequency[key]++;
                }
            }

            // Find sequence with highest frequency
            string mostCommonKey = null;
            int maxCount = 0;

            foreach (var kvp in sequenceFrequency)
            {
                if (kvp.Value > maxCount)
                {
                    maxCount = kvp.Value;
                    mostCommonKey = kvp.Key;
                }
            }

            // Check if frequency meets threshold (30% of entries)
            double threshold = 0.30;
            double frequency = (double)maxCount / entries.Count;

            if (frequency >= threshold && mostCommonKey != null)
            {
                // Convert hex string back to byte array
                string[] hexBytes = mostCommonKey.Split('-');
                byte[] result = new byte[hexBytes.Length];

                for (int i = 0; i < hexBytes.Length; i++)
                {
                    result[i] = System.Convert.ToByte(hexBytes[i], 16);
                }

                return result;
            }

            return null; // No consistent end marker found
        }

        /// <summary>
        /// Auto-detect segment separator using frequency analysis
        /// </summary>
        private byte[] DetectSegmentSeparator(System.Collections.Generic.List<LogEntry> entries)
        {
            if (entries == null || entries.Count < 5)
                return null; // Need minimum sample size

            // Dictionary to track frequency of byte sequences within entries
            var sequenceFrequency = new System.Collections.Generic.Dictionary<string, int>();
            int totalEntries = 0;

            // Analyze 1-2 byte sequences within each entry (excluding start/end)
            foreach (var entry in entries)
            {
                if (entry.RawBytes == null || entry.RawBytes.Length < 5)
                    continue; // Need sufficient length to have middle portion

                totalEntries++;

                // Skip first 2 and last 2 bytes to avoid start/end markers
                int startPos = System.Math.Min(2, entry.RawBytes.Length / 4);
                int endPos = entry.RawBytes.Length - System.Math.Min(2, entry.RawBytes.Length / 4);

                if (endPos <= startPos)
                    continue;

                // Track which sequences appear in this entry (for consistency)
                var seenInThisEntry = new System.Collections.Generic.HashSet<string>();

                // Try 1-byte and 2-byte sequences in the middle portion
                for (int i = startPos; i < endPos; i++)
                {
                    // 1-byte sequence
                    byte[] seq1 = new byte[] { entry.RawBytes[i] };
                    string key1 = System.BitConverter.ToString(seq1);

                    if (!seenInThisEntry.Contains(key1))
                    {
                        seenInThisEntry.Add(key1);
                        if (!sequenceFrequency.ContainsKey(key1))
                            sequenceFrequency[key1] = 0;
                        sequenceFrequency[key1]++;
                    }

                    // 2-byte sequence
                    if (i < endPos - 1)
                    {
                        byte[] seq2 = new byte[] { entry.RawBytes[i], entry.RawBytes[i + 1] };
                        string key2 = System.BitConverter.ToString(seq2);

                        if (!seenInThisEntry.Contains(key2))
                        {
                            seenInThisEntry.Add(key2);
                            if (!sequenceFrequency.ContainsKey(key2))
                                sequenceFrequency[key2] = 0;
                            sequenceFrequency[key2]++;
                        }
                    }
                }
            }

            if (totalEntries == 0)
                return null;

            // Find sequence with highest frequency
            string mostCommonKey = null;
            int maxCount = 0;

            foreach (var kvp in sequenceFrequency)
            {
                if (kvp.Value > maxCount)
                {
                    maxCount = kvp.Value;
                    mostCommonKey = kvp.Key;
                }
            }

            // Check if frequency meets threshold (20% of entries must contain it)
            double threshold = 0.20;
            double frequency = (double)maxCount / totalEntries;

            if (frequency >= threshold && mostCommonKey != null)
            {
                // Convert hex string back to byte array
                string[] hexBytes = mostCommonKey.Split('-');
                byte[] result = new byte[hexBytes.Length];

                for (int i = 0; i < hexBytes.Length; i++)
                {
                    result[i] = System.Convert.ToByte(hexBytes[i], 16);
                }

                return result;
            }

            return null; // No consistent separator found
        }

        /// <summary>
        /// Auto-detect encoding using valid character ratio analysis
        /// </summary>
        private EncodingType DetectEncoding(System.Collections.Generic.List<LogEntry> entries)
        {
            if (entries == null || entries.Count == 0)
                return EncodingType.ASCII; // Default

            // Collect all bytes from all entries
            var allBytes = new System.Collections.Generic.List<byte>();
            foreach (var entry in entries)
            {
                if (entry.RawBytes != null && entry.RawBytes.Length > 0)
                {
                    allBytes.AddRange(entry.RawBytes);
                }
            }

            if (allBytes.Count == 0)
                return EncodingType.ASCII;

            byte[] data = allBytes.ToArray();

            // Test each encoding and calculate valid character ratio
            double asciiRatio = TestASCII(data);
            double utf8Ratio = TestUTF8(data);
            double utf16Ratio = TestUTF16(data);
            double latin1Ratio = TestLatin1(data);

            // Return encoding with highest ratio (must be > 95% to be confident)
            double threshold = 0.95;

            if (asciiRatio >= threshold && asciiRatio >= utf8Ratio && asciiRatio >= utf16Ratio && asciiRatio >= latin1Ratio)
                return EncodingType.ASCII;

            if (utf8Ratio >= threshold && utf8Ratio >= asciiRatio && utf8Ratio >= utf16Ratio && utf8Ratio >= latin1Ratio)
                return EncodingType.UTF8;

            if (utf16Ratio >= threshold && utf16Ratio >= asciiRatio && utf16Ratio >= utf8Ratio && utf16Ratio >= latin1Ratio)
                return EncodingType.UTF16;

            if (latin1Ratio >= threshold && latin1Ratio >= asciiRatio && latin1Ratio >= utf8Ratio && latin1Ratio >= utf16Ratio)
                return EncodingType.Latin1;

            // Default to ASCII if uncertain
            return EncodingType.ASCII;
        }

        /// <summary>
        /// Test if data is valid ASCII (printable + whitespace)
        /// </summary>
        private double TestASCII(byte[] data)
        {
            int validCount = 0;

            foreach (byte b in data)
            {
                // Valid ASCII: printable (0x20-0x7E) + CR/LF/TAB
                if ((b >= 0x20 && b <= 0x7E) || b == 0x09 || b == 0x0A || b == 0x0D)
                {
                    validCount++;
                }
            }

            return (double)validCount / data.Length;
        }

        /// <summary>
        /// Test if data is valid UTF-8
        /// </summary>
        private double TestUTF8(byte[] data)
        {
            try
            {
                // Try to decode as UTF-8
                var encoding = System.Text.Encoding.UTF8;
                string decoded = encoding.GetString(data);

                // Re-encode and compare
                byte[] reencoded = encoding.GetBytes(decoded);

                // Calculate how many bytes match
                int matchCount = 0;
                int minLength = System.Math.Min(data.Length, reencoded.Length);

                for (int i = 0; i < minLength; i++)
                {
                    if (data[i] == reencoded[i])
                        matchCount++;
                }

                return (double)matchCount / data.Length;
            }
            catch
            {
                return 0.0; // Invalid UTF-8
            }
        }

        /// <summary>
        /// Test if data is valid UTF-16
        /// </summary>
        private double TestUTF16(byte[] data)
        {
            try
            {
                // UTF-16 requires even number of bytes
                if (data.Length % 2 != 0)
                    return 0.0;

                // Try to decode as UTF-16
                var encoding = System.Text.Encoding.Unicode;
                string decoded = encoding.GetString(data);

                // Re-encode and compare
                byte[] reencoded = encoding.GetBytes(decoded);

                // Calculate how many bytes match
                int matchCount = 0;
                int minLength = System.Math.Min(data.Length, reencoded.Length);

                for (int i = 0; i < minLength; i++)
                {
                    if (data[i] == reencoded[i])
                        matchCount++;
                }

                return (double)matchCount / data.Length;
            }
            catch
            {
                return 0.0; // Invalid UTF-16
            }
        }

        /// <summary>
        /// Test if data is valid Latin-1 (ISO-8859-1)
        /// </summary>
        private double TestLatin1(byte[] data)
        {
            try
            {
                // Latin-1 can encode all byte values 0x00-0xFF
                // Test for common Latin-1 patterns
                var encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                string decoded = encoding.GetString(data);

                // Re-encode and compare
                byte[] reencoded = encoding.GetBytes(decoded);

                // Calculate how many bytes match
                int matchCount = 0;
                int minLength = System.Math.Min(data.Length, reencoded.Length);

                for (int i = 0; i < minLength; i++)
                {
                    if (data[i] == reencoded[i])
                        matchCount++;
                }

                return (double)matchCount / data.Length;
            }
            catch
            {
                return 0.0; // Invalid Latin-1
            }
        }
    }
}
