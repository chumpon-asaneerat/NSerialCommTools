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
    /// Interaction logic for JIK6CABEmulatorPage.xaml
    /// </summary>
    public partial class JIK6CABEmulatorPage : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public JIK6CABEmulatorPage()
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
            JIK6CABDevice.Instance.LoadConfig();
            ctrlSetting.Setup(JIK6CABDevice.Instance);
        }
        /// <summary>
        /// Sync content to device.
        /// </summary>
        public void Sync()
        {
            if (!JIK6CABDevice.Instance.IsOpen) return;

            if (onSync) return;

            onSync = true;

            DateTime dt = DateTime.Now;

            txtDateTime.Text = dt.ToString("yyyy-MM-dd HH:mm:ss",
                System.Globalization.DateTimeFormatInfo.InvariantInfo);

            var data = JIK6CABDevice.Instance.Value;
            try
            {
                data.Date = dt;
                data.TW = decimal.Parse(txtTW.Text);
                data.TUnit = txtTU.Text;
                data.NW = decimal.Parse(txtNW.Text);
                data.NUnit = txtNU.Text;
                data.GW = decimal.Parse(txtGW.Text);
                data.GUnit = txtGU.Text;
                data.PCS = decimal.Parse(txtPCS.Text);
            }
            catch { }

            var buffers = data.ToByteArray();
            JIK6CABDevice.Instance.Send(buffers);

            onSync = false;
        }

        #endregion
    }
}
