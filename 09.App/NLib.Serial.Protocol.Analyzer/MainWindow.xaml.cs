#region Using

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using NLib.Serial.Protocol.Analyzer.Analyzers;
using NLib.Serial.Protocol.Analyzer.Generators;
using NLib.Serial.Protocol.Analyzer.Models;
using NLib.Serial.Protocol.Analyzer.Parsers;

#endregion

namespace NLib
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields

        private LogFileLoader _loader = new LogFileLoader();
        private PatternAnalyzer _analyzer = new PatternAnalyzer();
        private DefinitionGenerator _generator = new DefinitionGenerator();

        private LogData _currentLogData = null;
        private AnalysisResult _currentAnalysis = null;
        private ProtocolDefinition _currentDefinition = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers - Buttons

        /// <summary>
        /// Browse button click
        /// </summary>
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Title = "Select Log File",
                Filter = "Log Files (*.txt;*.log)|*.txt;*.log|All Files (*.*)|*.*",
                Multiselect = false
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    // Load file
                    _currentLogData = _loader.LoadFile(dlg.FileName);

                    // Update UI
                    txtLogFilePath.Text = dlg.FileName;
                    txtDetectedFormat.Text = _currentLogData.DetectedFormat.ToString();
                    txtMessageCount.Text = _currentLogData.MessageCount.ToString();

                    // Show raw messages preview (first 50)
                    lstRawMessages.Items.Clear();
                    foreach (var message in _currentLogData.Messages.Take(50))
                    {
                        string text = Encoding.ASCII.GetString(message);
                        string display = text.Replace("\r", "\\r").Replace("\n", "\\n");
                        lstRawMessages.Items.Add(display);
                    }

                    if (_currentLogData.MessageCount > 50)
                    {
                        lstRawMessages.Items.Add($"... and {_currentLogData.MessageCount - 50} more messages");
                    }

                    // Enable analyze button
                    btnAnalyze.IsEnabled = true;

                    MessageBox.Show($"Loaded {_currentLogData.MessageCount} messages successfully!",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading file: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Analyze button click
        /// </summary>
        private void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            if (_currentLogData == null)
                return;

            try
            {
                // Analyze
                _currentAnalysis = _analyzer.Analyze(_currentLogData);

                // Update Terminator UI
                if (_currentAnalysis.Terminator != null && _currentAnalysis.Terminator.Detected)
                {
                    txtTerminatorType.Text = _currentAnalysis.Terminator.Type;
                    txtTerminatorHex.Text = _currentAnalysis.Terminator.HexString;
                    txtTerminatorConfidence.Text = $"{_currentAnalysis.Terminator.Confidence:F1}%";
                }
                else
                {
                    txtTerminatorType.Text = "Not detected";
                    txtTerminatorHex.Text = "-";
                    txtTerminatorConfidence.Text = "0%";
                }

                // Update Delimiters grid
                gridDelimiters.ItemsSource = _currentAnalysis.Delimiters;

                // Update Fields grid
                gridFields.ItemsSource = _currentAnalysis.Fields;

                // Generate protocol definition
                string deviceName = Path.GetFileNameWithoutExtension(_currentLogData.SourceFilePath);
                _currentDefinition = _generator.Generate(_currentAnalysis, deviceName, _currentLogData.SourceFilePath,
                    _currentLogData, _currentAnalysis.PackageInfo);

                // Show JSON
                txtJsonOutput.Text = _generator.ToJson(_currentDefinition);

                // Enable export button
                btnExportJson.IsEnabled = true;

                MessageBox.Show($"Analysis complete!\n\n" +
                    $"Protocol Type: {_currentAnalysis.ProtocolType}\n" +
                    $"Recommended Strategy: {_currentAnalysis.RecommendedStrategy}\n" +
                    $"Overall Confidence: {_currentAnalysis.Confidence:F1}%",
                    "Analysis Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                string errorDetails = $"Error during analysis:\n\n{ex.Message}\n\n";
                if (ex.InnerException != null)
                {
                    errorDetails += $"Inner Exception: {ex.InnerException.Message}\n\n";
                }
                errorDetails += $"Stack Trace:\n{ex.StackTrace}";

                MessageBox.Show(errorDetails, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Export JSON button click
        /// </summary>
        private void btnExportJson_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDefinition == null)
                return;

            SaveFileDialog dlg = new SaveFileDialog
            {
                Title = "Save Protocol Definition",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                FileName = $"{_currentDefinition.DeviceInfo.Name}.json"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    bool success = _generator.SaveToFile(_currentDefinition, dlg.FileName);

                    if (success)
                    {
                        MessageBox.Show($"Protocol definition saved to:\n{dlg.FileName}",
                            "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to save protocol definition.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Clear button click
        /// </summary>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            // Clear data
            _currentLogData = null;
            _currentAnalysis = null;
            _currentDefinition = null;

            // Clear UI
            txtLogFilePath.Text = string.Empty;
            txtDetectedFormat.Text = "None";
            txtMessageCount.Text = "0";

            txtTerminatorType.Text = string.Empty;
            txtTerminatorHex.Text = string.Empty;
            txtTerminatorConfidence.Text = string.Empty;

            gridDelimiters.ItemsSource = null;
            gridFields.ItemsSource = null;

            txtJsonOutput.Text = string.Empty;
            lstRawMessages.Items.Clear();

            // Disable buttons
            btnAnalyze.IsEnabled = false;
            btnExportJson.IsEnabled = false;
        }

        #endregion
    }
}
