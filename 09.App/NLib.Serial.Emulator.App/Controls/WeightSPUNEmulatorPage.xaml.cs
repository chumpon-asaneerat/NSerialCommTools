#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using NLib.Serial.Devices;
using NLib.Serial.Emulators;

#endregion

namespace NLib.Serial.Emulator.App.Controls
{
    /// <summary>
    /// Interaction logic for WeightSPUNEmulatorPage.xaml
    /// </summary>
    public partial class WeightSPUNEmulatorPage : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public WeightSPUNEmulatorPage()
        {
            InitializeComponent();
        }

        #endregion

        #region Loaded/Unloader

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
