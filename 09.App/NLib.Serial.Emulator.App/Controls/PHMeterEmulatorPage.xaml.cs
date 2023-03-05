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
    /// Interaction logic for PHMeterEmulatorPage.xaml
    /// </summary>
    public partial class PHMeterEmulatorPage : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public PHMeterEmulatorPage()
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
            PHMeterDevice.Instance.LoadConfig();
            ctrlSetting.Setup(PHMeterDevice.Instance);
        }
        /// <summary>
        /// Sync content to device.
        /// </summary>
        public void Sync()
        {
            if (!PHMeterDevice.Instance.IsOpen) return;

            if (onSync) return;

            onSync = true;

            DateTime dt = DateTime.Now;

            txtDateTime.Text = dt.ToString("yyyy-MM-dd HH:mm",
                System.Globalization.DateTimeFormatInfo.InvariantInfo);

            var data = PHMeterDevice.Instance.Value;
            try
            {
                data.pH = decimal.Parse(txtpH.Text);
                data.TempC = decimal.Parse(txtTempC.Text);
                data.Date = dt;
            }
            catch { }

            var buffers = data.ToByteArray();
            PHMeterDevice.Instance.Send(buffers);

            onSync = false;
        }

        #endregion
    }
}
