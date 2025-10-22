#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using NLib.Serial.ProtocolAnalyzer.Analyzers;
using NLib.Serial.ProtocolAnalyzer.Models;
using NLib.Serial.ProtocolAnalyzer.Parsers;

#endregion

namespace NLib
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Internal Variables

        private LogData _currentLogData;
        private AnalysisResult _currentAnalysis;
        private List<FieldInfo> _fields;

        private readonly PatternAnalyzer _analyzer;
        private readonly HexLogParser _hexLogParser;
        private readonly MessageExtractor _messageExtractor;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _analyzer = new PatternAnalyzer();
            _hexLogParser = new HexLogParser();
            _messageExtractor = new MessageExtractor();

            _fields = new List<FieldInfo>();

            // Set default output folder
            txtOutputFolder.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        #endregion

        #region Toolbar Event Handlers

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            // Reset the application
            _currentLogData = null;
            _currentAnalysis = null;
            _fields.Clear();

            txtFilePath.Text = string.Empty;
            txtPreview.Text = string.Empty;
            txtDeviceName.Text = string.Empty;
            txtExportPreview.Text = string.Empty;

            dgDelimiters.ItemsSource = null;
            dgFieldsDetected.ItemsSource = null;
            dgFields.ItemsSource = null;

            tabMain.SelectedIndex = 0;
            UpdateStatus("Ready");
        }

        private void btnOpenLog_Click(object sender, RoutedEventArgs e)
        {
            btnBrowse_Click(sender, e);
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (_fields == null || _fields.Count == 0)
            {
                MessageBox.Show("Please complete the analysis and field editing first.", "Export",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            tabMain.SelectedIndex = 3; // Go to Export tab
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Protocol Analyzer Tool\n\nVersion 1.0\n\nAnalyzes serial device protocols from log files.",
                "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Input Tab Event Handlers

        private void rbSerialPort_Checked(object sender, RoutedEventArgs e)
        {
            if (grpSerialPort != null && grpLogFile != null)
            {
                grpSerialPort.Visibility = Visibility.Visible;
                grpLogFile.Visibility = Visibility.Collapsed;
            }
        }

        private void rbLogFile_Checked(object sender, RoutedEventArgs e)
        {
            if (grpSerialPort != null && grpLogFile != null)
            {
                grpSerialPort.Visibility = Visibility.Collapsed;
                grpLogFile.Visibility = Visibility.Visible;
            }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Select Serial Log File"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    LoadLogFile(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading file:\n{ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            if (_currentLogData == null)
            {
                MessageBox.Show("Please load a log file first.", "Analyze",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                PerformAnalysis();
                tabMain.SelectedIndex = 1; // Go to Analysis tab
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during analysis:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Analysis Tab Event Handlers

        private void btnNextToFieldEditor_Click(object sender, RoutedEventArgs e)
        {
            if (_currentAnalysis == null || _currentAnalysis.Fields.Count == 0)
            {
                MessageBox.Show("No fields detected. Please check your log file.", "Field Editor",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            PrepareFieldEditor();
            tabMain.SelectedIndex = 2; // Go to Field Editor tab
        }

        #endregion

        #region Field Editor Tab Event Handlers

        private void btnValidateFields_Click(object sender, RoutedEventArgs e)
        {
            bool valid = ValidateFields();
            if (valid)
            {
                MessageBox.Show("All fields are valid!", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnNextToExport_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            PrepareExport();
            tabMain.SelectedIndex = 3; // Go to Export tab
        }

        #endregion

        #region Export Tab Event Handlers

        private void btnBrowseOutput_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select Output Folder"
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtOutputFolder.Text = dialog.SelectedPath;
            }
        }

        private void btnPerformExport_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDeviceName.Text))
            {
                MessageBox.Show("Please enter a device name.", "Export",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                PerformExport();
                MessageBox.Show("Export completed successfully!", "Export",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during export:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Private Methods - File Loading

        private void LoadLogFile(string filePath)
        {
            UpdateStatus("Loading log file...");

            txtFilePath.Text = filePath;

            // Parse the log file
            byte[] rawBytes = _hexLogParser.ParseLogFile(filePath);

            // Extract messages
            _currentLogData = _messageExtractor.ExtractMessages(rawBytes);

            // Show preview
            ShowFilePreview(rawBytes);

            UpdateStatus($"Loaded {_currentLogData.MessageCount} messages ({_currentLogData.TotalBytes} bytes)");
        }

        private void ShowFilePreview(byte[] rawBytes)
        {
            var sb = new StringBuilder();
            int previewLength = Math.Min(500, rawBytes.Length);

            for (int i = 0; i < previewLength; i++)
            {
                sb.Append(rawBytes[i].ToString("X2"));
                sb.Append(" ");

                if ((i + 1) % 16 == 0)
                {
                    sb.AppendLine();
                }
            }

            if (rawBytes.Length > previewLength)
            {
                sb.AppendLine();
                sb.AppendLine("...");
            }

            txtPreview.Text = sb.ToString();
        }

        #endregion

        #region Private Methods - Analysis

        private void PerformAnalysis()
        {
            UpdateStatus("Analyzing protocol...");

            _currentAnalysis = _analyzer.Analyze(_currentLogData);

            // Update Terminator section
            if (_currentAnalysis.Terminator != null)
            {
                txtTerminatorType.Text = _currentAnalysis.Terminator.DisplayName;
                txtTerminatorBytes.Text = BitConverter.ToString(_currentAnalysis.Terminator.Bytes).Replace("-", " ");
                txtTerminatorConfidence.Text = $"{(_currentAnalysis.Terminator.Confidence * 100):F0}%";
            }

            // Update Delimiters section
            dgDelimiters.ItemsSource = _currentAnalysis.Delimiters;

            // Update Fields section - using FieldInfo directly with SampleValuesDisplay property
            dgFieldsDetected.ItemsSource = _currentAnalysis.Fields;

            UpdateStatus($"Analysis complete - {_currentAnalysis.Fields.Count} fields detected");
        }

        #endregion

        #region Private Methods - Field Editor

        private void PrepareFieldEditor()
        {
            UpdateStatus("Preparing field editor...");

            // Just use the fields directly - no conversion needed
            _fields = _currentAnalysis.Fields;

            dgFields.ItemsSource = _fields;

            UpdateStatus("Field editor ready");
        }

        private bool ValidateFields()
        {
            var errors = new List<string>();

            // Check for duplicate names
            var nameGroups = _fields.GroupBy(f => f.Name);
            foreach (var group in nameGroups)
            {
                if (group.Count() > 1)
                {
                    errors.Add($"Duplicate field name: {group.Key}");
                }
            }

            // Check for valid C# identifiers
            foreach (var field in _fields)
            {
                if (string.IsNullOrWhiteSpace(field.Name))
                {
                    errors.Add($"Field at position {field.Order} has no name");
                }
                else if (!IsValidCSharpIdentifier(field.Name))
                {
                    errors.Add($"Invalid field name: {field.Name}");
                }
            }

            if (errors.Any())
            {
                MessageBox.Show("Validation errors:\n\n" + string.Join("\n", errors), "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool IsValidCSharpIdentifier(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            if (!char.IsLetter(name[0]) && name[0] != '_')
                return false;

            for (int i = 1; i < name.Length; i++)
            {
                if (!char.IsLetterOrDigit(name[i]) && name[i] != '_')
                    return false;
            }

            // Check for C# keywords
            var keywords = new[] { "class", "namespace", "public", "private", "int", "string", "bool", "void", "return" };
            return !keywords.Contains(name.ToLower());
        }

        #endregion

        #region Private Methods - Export

        private void PrepareExport()
        {
            UpdateStatus("Preparing export...");

            var definition = BuildProtocolDefinition();
            string json = NLib.NJson.ToJson(definition, true);

            txtExportPreview.Text = json;

            UpdateStatus("Export preview ready");
        }

        private void PerformExport()
        {
            UpdateStatus("Exporting...");

            var definition = BuildProtocolDefinition();
            string deviceName = txtDeviceName.Text;
            string outputFolder = txtOutputFolder.Text;

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            if (chkExportJSON.IsChecked == true)
            {
                string jsonPath = Path.Combine(outputFolder, $"{deviceName}_Protocol.json");
                string json = NLib.NJson.ToJson(definition, true);
                File.WriteAllText(jsonPath, json);
            }

            if (chkExportReport.IsChecked == true)
            {
                string reportPath = Path.Combine(outputFolder, $"{deviceName}_Report.txt");
                string report = GenerateAnalysisReport();
                File.WriteAllText(reportPath, report);
            }

            UpdateStatus($"Export completed to: {outputFolder}");
        }

        private ProtocolDefinition BuildProtocolDefinition()
        {
            var definition = new ProtocolDefinition
            {
                DeviceName = txtDeviceName.Text,
                Version = "1.0",
                GeneratedDate = DateTime.Now,
                Description = $"Auto-generated protocol definition with {_currentAnalysis.Confidence * 100:F0}% confidence",
                Encoding = "ASCII",
                MessageType = "single-line", // TODO: Detect from analysis
                EntryTerminator = _currentAnalysis.Terminator?.String ?? "\\r\\n",
                Fields = _fields.Select(f => new FieldInfo
                {
                    Order = f.Order,
                    Name = f.Name,
                    DataType = f.DataType,
                    SampleValues = f.SampleValues,
                    Confidence = f.Confidence,
                    IsConstant = f.IsConstant,
                    MinLength = f.MinLength,
                    MaxLength = f.MaxLength,
                    Required = !f.IsConstant,
                    IncludeInDefinition = f.IncludeInDefinition
                }).ToList(),
                Relationships = new List<FieldRelationship>(),
                ValidationRules = new List<NLib.Serial.ProtocolAnalyzer.Models.ValidationRule>()
            };

            return definition;
        }

        private string GenerateAnalysisReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Protocol Analysis Report");
            sb.AppendLine("======================");
            sb.AppendLine();
            sb.AppendLine($"Device: {txtDeviceName.Text}");
            sb.AppendLine($"Date: {DateTime.Now}");
            sb.AppendLine($"Messages Analyzed: {_currentAnalysis.MessageCount}");
            sb.AppendLine($"Overall Confidence: {(_currentAnalysis.Confidence * 100):F1}%");
            sb.AppendLine();
            sb.AppendLine("Terminator");
            sb.AppendLine("----------");
            sb.AppendLine($"Type: {_currentAnalysis.Terminator?.DisplayName}");
            sb.AppendLine($"Confidence: {(_currentAnalysis.Terminator?.Confidence * 100):F0}%");
            sb.AppendLine();
            sb.AppendLine("Delimiters");
            sb.AppendLine("----------");
            foreach (var delim in _currentAnalysis.Delimiters)
            {
                sb.AppendLine($"  {delim.DisplayName} ({delim.Character}) - Confidence: {(delim.Confidence * 100):F0}%");
            }
            sb.AppendLine();
            sb.AppendLine("Fields");
            sb.AppendLine("------");
            foreach (var field in _fields)
            {
                sb.AppendLine($"  {field.Order}: {field.Name} ({field.DataType}) - Confidence: {(field.Confidence * 100):F0}%");
                sb.AppendLine($"     Samples: {string.Join(", ", field.SampleValues.Take(3))}");
            }

            return sb.ToString();
        }

        #endregion

        #region Private Methods - UI Helpers

        private void UpdateStatus(string message)
        {
            txtStatus.Text = message;
        }

        #endregion
    }
}
