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
    /// Interaction logic for TScaleQHWEmulatorPage.xaml
    /// </summary>
    public partial class TScaleQHWEmulatorPage : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public TScaleQHWEmulatorPage()
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
            TScaleQHWDevice.Instance.LoadConfig();
            ctrlSetting.Setup(TScaleQHWDevice.Instance);
        }
        /// <summary>
        /// Sync content to device.
        /// </summary>
        public void Sync()
        {
            if (!TScaleQHWDevice.Instance.IsOpen) return;

            if (onSync) return;

            onSync = true;

            var data = TScaleQHWDevice.Instance.Value;
            try
            {
                data.W = decimal.Parse(txtW.Text);
                data.Unit = txtUnit.Text.Trim();
                data.Status = txtStatus.Text.Trim().ToUpper();
                data.Mode = txtMode.Text.Trim().ToUpper();
            }
            catch { }

            var buffers = data.ToByteArray();
            TScaleQHWDevice.Instance.Send(buffers);

            onSync = false;
        }

        #endregion
    }
}
