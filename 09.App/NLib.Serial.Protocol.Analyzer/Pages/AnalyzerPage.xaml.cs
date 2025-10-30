using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NLib.Serial.Protocol.Analyzer.Models;

namespace NLib.Serial.Protocol.Analyzer.Pages
{
    /// <summary>
    /// Page 2: Analysis Page
    /// Purpose: Run 5-stage statistical analysis pipeline and display results
    /// </summary>
    public partial class AnalyzerPage : UserControl
    {
        private ProtocolAnalyzerModel _model;

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

            // Wire up event handlers
            RunAnalysisButton.Click += RunAnalysisButton_Click;

            // Initialize UI state
            UpdateUIState();
        }

        #region Event Handlers

        /// <summary>
        /// Handles Run Analysis button click
        /// </summary>
        private void RunAnalysisButton_Click(object sender, RoutedEventArgs e)
        {
            RunAnalysis();
        }

        #endregion

        #region Core Method Implementations

        /// <summary>
        /// Runs the 5-stage analysis pipeline and displays results
        /// </summary>
        private void RunAnalysis()
        {
            try
            {
                // Validate prerequisites
                if (_model.LogFile == null || _model.LogFile.Entries == null || _model.LogFile.Entries.Count == 0)
                {
                    MessageBox.Show(
                        "No log data available. Please load a log file first in the Input tab.",
                        "Cannot Run Analysis",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (_model.DetectionConfig == null || !_model.DetectionConfig.IsComplete())
                {
                    MessageBox.Show(
                        "Detection configuration is not complete. Please complete the configuration in the Input tab.",
                        "Cannot Run Analysis",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Update status
                AnalysisStatusText.Text = "Running analysis...";
                AnalysisStatusText.Foreground = new SolidColorBrush(Colors.Blue);
                RunAnalysisButton.IsEnabled = false;

                // Run the 5-stage analysis pipeline
                var analysisResult = _model.FieldAnalyzer.RunFullAnalysis(_model.LogFile, _model.DetectionConfig);

                // Store result in model
                _model.AnalysisResult = analysisResult;

                // Update UI with results
                DisplayAnalysisResults(analysisResult);

                // Update status
                AnalysisStatusText.Text = $"✅ Analysis complete! Found {analysisResult.DetectedFields.Count} fields";
                AnalysisStatusText.Foreground = new SolidColorBrush(Colors.Green);

                // Show completion message
                MessageBox.Show(
                    $"Analysis complete!\n\n" +
                    $"Total Entries: {analysisResult.TotalEntries}\n" +
                    $"Total Packages: {analysisResult.TotalPackages}\n" +
                    $"Detected Fields: {analysisResult.DetectedFields.Count}\n" +
                    $"Overall Confidence: {analysisResult.OverallConfidence:F1}%",
                    "Analysis Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                AnalysisStatusText.Text = "❌ Analysis failed";
                AnalysisStatusText.Foreground = new SolidColorBrush(Colors.Red);

                MessageBox.Show(
                    $"Error running analysis:\n\n{ex.Message}",
                    "Analysis Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                RunAnalysisButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Displays the analysis results in the UI
        /// </summary>
        /// <param name="result">Analysis result to display</param>
        private void DisplayAnalysisResults(AnalysisResult result)
        {
            if (result == null)
                return;

            // Update overall confidence
            double confidence = result.OverallConfidence;
            ConfidenceText.Text = $"{confidence:F1}%";
            ConfidenceProgressBar.Value = confidence;

            // Color-code confidence
            if (confidence >= 80)
                ConfidenceProgressBar.Foreground = new SolidColorBrush(Colors.Green);
            else if (confidence >= 60)
                ConfidenceProgressBar.Foreground = new SolidColorBrush(Colors.Orange);
            else
                ConfidenceProgressBar.Foreground = new SolidColorBrush(Colors.Red);

            // Update detection summary panels
            if (result.DetectionSummary != null)
            {
                DisplayDetectionSummary(result.DetectionSummary);
            }

            // Update fields preview DataGrid
            FieldsPreviewDataGrid.ItemsSource = result.DetectedFields;
        }

        /// <summary>
        /// Displays the detection summary in the three panels
        /// </summary>
        /// <param name="summary">Detection summary to display</param>
        private void DisplayDetectionSummary(DetectionSummary summary)
        {
            // Panel 1: Terminator Detection
            TerminatorHexText.Text = summary.PackageTerminator ?? "-";
            TerminatorTextRepText.Text = GetTextRepresentation(summary.PackageTerminator);
            TerminatorOccursText.Text = summary.PackageTerminatorOccurrences > 0
                ? $"{summary.PackageTerminatorOccurrences} times"
                : "-";
            TerminatorConfText.Text = summary.PackageTerminatorOccurrences > 0
                ? "100%"
                : "-";

            // Panel 2: Delimiter Detection
            DelimiterHexText.Text = summary.SegmentDelimiter ?? "-";
            DelimiterTextRepText.Text = GetTextRepresentation(summary.SegmentDelimiter);

            // Calculate delimiter frequency if available
            if (summary.SegmentDelimiter != null && summary.SegmentDelimiter != "None")
            {
                DelimiterFreqText.Text = "Variable";
            }
            else
            {
                DelimiterFreqText.Text = "-";
            }

            // Panel 3: Protocol Type
            ProtocolTypeText.Text = summary.ProtocolType ?? "-";

            // Determine strategy based on protocol type
            if (summary.ProtocolType != null && summary.ProtocolType.Contains("Segments"))
            {
                StrategyText.Text = "Delimiter-based";
            }
            else
            {
                StrategyText.Text = "Position-based";
            }

            FieldCountText.Text = summary.FieldCount.ToString();
        }

        /// <summary>
        /// Converts hex byte sequence to human-readable text representation
        /// </summary>
        /// <param name="hexSequence">Hex sequence (e.g., "0D 0A")</param>
        /// <returns>Text representation (e.g., "\\r\\n (CRLF)")</returns>
        private string GetTextRepresentation(string hexSequence)
        {
            if (string.IsNullOrWhiteSpace(hexSequence) || hexSequence == "None")
                return "-";

            try
            {
                // Parse hex string to bytes
                var hexParts = hexSequence.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
                var bytes = new byte[hexParts.Length];

                for (int i = 0; i < hexParts.Length; i++)
                {
                    bytes[i] = Convert.ToByte(hexParts[i], 16);
                }

                // Build text representation
                var text = new System.Text.StringBuilder();

                foreach (var b in bytes)
                {
                    if (b == 0x0D)
                        text.Append("\\r");
                    else if (b == 0x0A)
                        text.Append("\\n");
                    else if (b == 0x09)
                        text.Append("\\t");
                    else if (b == 0x20)
                        text.Append("(space)");
                    else if (b == 0x2C)
                        text.Append("','");
                    else if (b >= 0x20 && b <= 0x7E)
                        text.Append($"'{(char)b}'");
                    else
                        text.Append($"[0x{b:X2}]");
                }

                // Add common names
                string result = text.ToString();
                if (result == "\\r\\n")
                    result += " (CRLF)";
                else if (result == "\\n")
                    result += " (LF)";
                else if (result == "\\r")
                    result += " (CR)";

                return result;
            }
            catch
            {
                return hexSequence;
            }
        }

        /// <summary>
        /// Updates the UI state based on model availability
        /// </summary>
        private void UpdateUIState()
        {
            if (_model == null)
                return;

            // Enable/disable Run Analysis button based on prerequisites
            bool canRunAnalysis = _model.LogFile != null &&
                                  _model.LogFile.Entries != null &&
                                  _model.LogFile.Entries.Count > 0 &&
                                  _model.DetectionConfig != null &&
                                  _model.DetectionConfig.IsComplete();

            RunAnalysisButton.IsEnabled = canRunAnalysis;

            if (!canRunAnalysis)
            {
                AnalysisStatusText.Text = "Please load data and complete detection configuration first";
                AnalysisStatusText.Foreground = new SolidColorBrush(Colors.Gray);
            }

            // If analysis result already exists, display it
            if (_model.AnalysisResult != null && _model.AnalysisResult.DetectedFields != null)
            {
                DisplayAnalysisResults(_model.AnalysisResult);
            }
        }

        #endregion
    }
}
