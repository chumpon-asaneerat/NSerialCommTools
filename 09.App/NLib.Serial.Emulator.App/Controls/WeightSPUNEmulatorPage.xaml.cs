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

        #region Loaded/Unloaded

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region Internal Variables

        private bool onSync = false;

        #endregion

        #region Public Methods

        /// <summary>
        /// Setup.
        /// </summary>
        public void Setup()
        {
            WeightSPUNDevice.Instance.LoadConfig();
            ctrlSetting.Setup(WeightSPUNDevice.Instance);
        }
        /// <summary>
        /// Sync content to device.
        /// </summary>
        public void Sync()
        {
            if (!WeightSPUNDevice.Instance.IsOpen) return;

            if (onSync) return;

            onSync = true;

            var data = WeightSPUNDevice.Instance.Value;
            var buffers = data.ToByteArray();
            WeightSPUNDevice.Instance.Send(buffers);

            onSync = false;
        }

        #endregion
    }
}
