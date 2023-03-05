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
    /// Interaction logic for WeightQAEmulatorPage.xaml
    /// </summary>
    public partial class WeightQAEmulatorPage : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public WeightQAEmulatorPage()
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
            WeightQADevice.Instance.LoadConfig();
            ctrlSetting.Setup(WeightQADevice.Instance);
        }
        /// <summary>
        /// Sync content to device.
        /// </summary>
        public void Sync()
        {
            if (!WeightQADevice.Instance.IsOpen) return;

            if (onSync) return;

            onSync = true;

            var data = WeightQADevice.Instance.Value;
            try
            {
                data.W = decimal.Parse(txtW.Text);
                data.O = int.Parse(txtO.Text);
                data.Unit = txtUnit.Text.Trim().ToUpper();
                data.Mode = txtMode.Text.Trim().ToUpper();
            }
            catch { }

            var buffers = data.ToByteArray();
            WeightQADevice.Instance.Send(buffers);

            onSync = false;
        }

        #endregion
    }
}
