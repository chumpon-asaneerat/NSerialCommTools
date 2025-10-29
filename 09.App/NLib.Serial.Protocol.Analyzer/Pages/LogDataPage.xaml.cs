using System.Windows.Controls;
using NLib.Serial.Protocol.Analyzer.Models;

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
            // TODO: Phase 3.5 - Implement configuration application logic
            System.Windows.MessageBox.Show("Apply Configuration - To be implemented in Phase 3.5",
                "Not Implemented",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
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

        // TODO: Phase 3.5 - Core Method Implementations
        // - LoadLogFile() - Open file, parse entries
        // - AutoDetectDelimiters() - Run 4 detection algorithms
        // - ApplyConfiguration() - Save to model

        // TODO: Phase 3.6 - Auto-Detection Algorithms
        // - DetectPackageStartMarker()
        // - DetectPackageEndMarker()
        // - DetectSegmentSeparator()
        // - DetectEncoding()
    }
}
