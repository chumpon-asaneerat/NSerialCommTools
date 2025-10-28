#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using NLib.Serial.Protocol.Analyzer.Models;

#endregion

namespace NLib
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Main window for the Serial Protocol Analyzer application
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        /// <summary>
        /// Single shared model instance for the entire application
        /// </summary>
        private ProtocolAnalyzerModel _model;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor - Initializes the window and shared model
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Create THE model (single instance shared across all pages)
            _model = new ProtocolAnalyzerModel();

            // Setup all pages with the model
            // Pages are created by XAML, we just call Setup() to inject the model
            LogDataPage.Setup(_model);
            AnalyzerPage.Setup(_model);
            FieldEditorPage.Setup(_model);
            ExportPage.Setup(_model);

            // Initialize status bar
            UpdateStatusBar();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles tab selection changes - validates that prerequisites are met
        /// </summary>
        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl == null || _model == null)
                return;

            // Validate data before allowing tab switch
            if (MainTabControl.SelectedIndex == 1) // Analysis tab
            {
                if (!_model.CanAccessPage2())
                {
                    MessageBox.Show(
                        "Please load log data and configure detection settings first in the Input Data tab.",
                        "Prerequisites Not Met",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    MainTabControl.SelectedIndex = 0;
                }
            }
            else if (MainTabControl.SelectedIndex == 2) // Field Editor tab
            {
                if (!_model.CanAccessPage3())
                {
                    MessageBox.Show(
                        "Please run analysis first in the Analysis tab.",
                        "Prerequisites Not Met",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    MainTabControl.SelectedIndex = 1;
                }
            }
            else if (MainTabControl.SelectedIndex == 3) // Export tab
            {
                if (!_model.CanAccessPage4())
                {
                    MessageBox.Show(
                        "Please define fields first in the Field Editor tab.",
                        "Prerequisites Not Met",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    MainTabControl.SelectedIndex = 2;
                }
            }

            // Update status bar when tab changes
            UpdateStatusBar();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the status bar with current model state
        /// </summary>
        private void UpdateStatusBar()
        {
            if (_model == null)
                return;

            // Update entry count
            if (_model.LogFile != null && _model.LogFile.Entries != null)
                EntryCountText.Text = $"{_model.LogFile.EntryCount} entries";
            else
                EntryCountText.Text = "0 entries";

            // Update confidence (placeholder - will be implemented later)
            if (_model.AnalysisResult != null && _model.AnalysisResult.FieldCount > 0)
            {
                ConfidenceStatusText.Text = $"Fields: {_model.AnalysisResult.FieldCount}";
            }
            else
                ConfidenceStatusText.Text = "Confidence: N/A";

            // Update general status
            if (!string.IsNullOrEmpty(_model.JsonSchema))
                StatusText.Text = "✅ Ready to export";
            else if (_model.AnalysisResult != null && _model.AnalysisResult.FieldCount > 0)
                StatusText.Text = "✅ Analysis complete";
            else if (_model.PackageCount > 0)
                StatusText.Text = "✅ Packages parsed";
            else if (_model.LogFile != null && _model.LogFile.EntryCount > 0)
                StatusText.Text = "✅ Data loaded";
            else
                StatusText.Text = "Ready";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Public method to refresh the status bar (called from pages)
        /// </summary>
        public void RefreshStatusBar()
        {
            UpdateStatusBar();
        }

        #endregion
    }
}
