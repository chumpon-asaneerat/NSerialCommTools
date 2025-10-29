using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using NLib.Serial.Protocol.Analyzer.Models;

namespace NLib.Serial.Protocol.Analyzer.Pages
{
    /// <summary>
    /// Page 2: Package Analyzer (Parsing)
    /// Purpose: Parse log entries into packages and segments using detection configuration
    /// </summary>
    public partial class AnalyzerPage : UserControl
    {
        private ProtocolAnalyzerModel _model;
        private PackageInfo _selectedPackage;

        /// <summary>
        /// Initializes a new instance of the AnalyzerPage
        /// </summary>
        public AnalyzerPage()
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

            // Check if detection config is complete
            if (_model.DetectionConfig != null && _model.DetectionConfig.IsComplete())
            {
                ParsePackagesButton.IsEnabled = true;
            }
            else
            {
                ParsePackagesButton.IsEnabled = false;
            }

            // Wire up event handlers
            ParsePackagesButton.Click += ParsePackagesButton_Click;
            ClearResultsButton.Click += ClearResultsButton_Click;
            PackageListBox.SelectionChanged += PackageListBox_SelectionChanged;

            // Bind to existing packages if any
            if (_model.Packages != null && _model.Packages.Count > 0)
            {
                PackageListBox.ItemsSource = _model.Packages;
                UpdatePackageCount();
            }
        }

        #region Event Handlers

        /// <summary>
        /// Handles Parse Packages button click
        /// </summary>
        private void ParsePackagesButton_Click(object sender, RoutedEventArgs e)
        {
            ParsePackages();
        }

        /// <summary>
        /// Handles Clear Results button click
        /// </summary>
        private void ClearResultsButton_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
        }

        /// <summary>
        /// Handles package selection change in ListBox
        /// </summary>
        private void PackageListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PackageListBox.SelectedItem is PackageInfo package)
            {
                OnPackageSelected(package);
            }
        }

        #endregion

        #region Core Method Implementations

        /// <summary>
        /// Parses log entries into packages and segments
        /// </summary>
        private void ParsePackages()
        {
            try
            {
                // Validate detection configuration
                if (_model.DetectionConfig == null || !_model.DetectionConfig.IsComplete())
                {
                    MessageBox.Show(
                        "Detection configuration is not complete. Please complete Page 1 first.",
                        "Cannot Parse",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Validate log entries exist
                if (_model.LogFile == null || _model.LogFile.EntryCount == 0)
                {
                    MessageBox.Show(
                        "No log entries to parse. Please load a log file first.",
                        "Cannot Parse",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Clear existing packages
                _model.Packages.Clear();

                // Use the parser from the model (encapsulation pattern)
                var packages = _model.Parser.SplitIntoPackages(
                    _model.LogFile.Entries.ToList(),
                    _model.DetectionConfig);

                // Split each package into segments
                foreach (var package in packages)
                {
                    var segments = _model.Parser.SplitIntoSegments(
                        package.RawBytes,
                        _model.DetectionConfig);

                    package.Segments = segments;
                }

                // Update model
                _model.Packages = packages;

                // Update UI
                PackageListBox.ItemsSource = _model.Packages;
                UpdatePackageCount();

                // Show completion message
                MessageBox.Show(
                    $"Parsing complete!\n\nPackages found: {_model.Packages.Count}",
                    "Parse Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Auto-select first package
                if (_model.Packages.Count > 0)
                {
                    PackageListBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error parsing packages:\n\n{ex.Message}",
                    "Parse Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles package selection - updates detail panel
        /// </summary>
        /// <param name="package">Selected package</param>
        private void OnPackageSelected(PackageInfo package)
        {
            try
            {
                _selectedPackage = package;

                if (package == null)
                {
                    ClearDetailPanel();
                    return;
                }

                // Update header labels
                PackageNumberLabel.Text = $"Package #{package.PackageNumber}";
                PackageTotalBytesLabel.Text = $"Total: {package.Length} bytes";
                PackageSegmentCountLabel.Text = $"Segments: {package.SegmentCount}";
                PackageTimestampLabel.Text = $"Timestamp: {package.Timestamp:yyyy-MM-dd HH:mm:ss}";

                // Update segments DataGrid
                SegmentsDataGrid.ItemsSource = package.Segments;

                // Update raw package display
                UpdateRawPackageDisplay(package);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error displaying package:\n\n{ex.Message}",
                    "Display Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Clears all parsing results
        /// </summary>
        private void ClearResults()
        {
            try
            {
                // Clear model
                _model.Packages.Clear();

                // Clear UI
                PackageListBox.ItemsSource = null;
                UpdatePackageCount();
                ClearDetailPanel();

                MessageBox.Show(
                    "Parsing results cleared.",
                    "Cleared",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error clearing results:\n\n{ex.Message}",
                    "Clear Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Updates the package count label
        /// </summary>
        private void UpdatePackageCount()
        {
            int count = _model.Packages != null ? _model.Packages.Count : 0;
            PackageCountLabel.Text = $"Packages: {count}";
        }

        /// <summary>
        /// Updates the raw package display (Hex and Text views)
        /// </summary>
        /// <param name="package">Package to display</param>
        private void UpdateRawPackageDisplay(PackageInfo package)
        {
            if (package == null)
            {
                HexViewTextBox.Text = string.Empty;
                TextViewTextBox.Text = string.Empty;
                return;
            }

            // Hex View: Format with line breaks every 32 bytes (16 hex pairs)
            HexViewTextBox.Text = FormatHexWithLineBreaks(package.RawHex, 32);

            // Text View: Show raw text
            TextViewTextBox.Text = package.RawText;
        }

        /// <summary>
        /// Formats hex string with line breaks every N characters
        /// </summary>
        /// <param name="hex">Hex string (e.g., "02 41 00 64...")</param>
        /// <param name="charsPerLine">Characters per line</param>
        /// <returns>Formatted hex string with line breaks</returns>
        private string FormatHexWithLineBreaks(string hex, int charsPerLine)
        {
            if (string.IsNullOrEmpty(hex))
                return string.Empty;

            var result = new System.Text.StringBuilder();
            int currentLineLength = 0;

            for (int i = 0; i < hex.Length; i++)
            {
                result.Append(hex[i]);
                currentLineLength++;

                // Add line break after N characters (but not at end)
                if (currentLineLength >= charsPerLine && i < hex.Length - 1)
                {
                    result.AppendLine();
                    currentLineLength = 0;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Clears the detail panel
        /// </summary>
        private void ClearDetailPanel()
        {
            PackageNumberLabel.Text = "Package #: -";
            PackageTotalBytesLabel.Text = "Total: 0 bytes";
            PackageSegmentCountLabel.Text = "Segments: 0";
            PackageTimestampLabel.Text = "Timestamp: -";
            SegmentsDataGrid.ItemsSource = null;
            HexViewTextBox.Text = string.Empty;
            TextViewTextBox.Text = string.Empty;
        }

        #endregion
    }
}
