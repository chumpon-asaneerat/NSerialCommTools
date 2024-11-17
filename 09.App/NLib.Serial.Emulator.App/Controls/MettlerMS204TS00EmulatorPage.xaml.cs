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
    /// Interaction logic for MettlerMS204TS00EmulatorPage.xaml
    /// </summary>
    public partial class MettlerMS204TS00EmulatorPage : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MettlerMS204TS00EmulatorPage()
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
            MettlerMS204TS00Device.Instance.LoadConfig();
            ctrlSetting.Setup(MettlerMS204TS00Device.Instance);
        }
        /// <summary>
        /// Sync content to device.
        /// </summary>
        public void Sync()
        {
            if (!MettlerMS204TS00Device.Instance.IsOpen) return;

            if (onSync) return;

            onSync = true;

            var data = MettlerMS204TS00Device.Instance.Value;
            try
            {
                data.W = decimal.Parse(txtW.Text);
                data.Mode = txtMode.Text.Trim();
                data.Unit = txtUnit.Text.Trim();
            }
            catch { }

            var buffers = data.ToByteArray();
            MettlerMS204TS00Device.Instance.Send(buffers);

            onSync = false;
        }

        #endregion
    }
}
