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

using NLib.Serial;
using NLib.Serial.Devices;
using NLib.Serial.Terminals;

#endregion

namespace NLib.Serial.Terminal.App.Controls
{
    /// <summary>
    /// Interaction logic for SerialCommSetting.xaml
    /// </summary>
    public partial class SerialCommSetting : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialCommSetting()
        {
            InitializeComponent();
        }

        #endregion

        #region Internal Variables

        private SerialDevice _device = null;

        #endregion

        #region Private Methods

        private void EnableInputs(bool enabled)
        {
            cbPortNames.IsEnabled = enabled;
            txtBoadRate.IsEnabled = enabled;
            cbParities.IsEnabled = enabled;
            txtDataBit.IsEnabled = enabled;
            cbStopBits.IsEnabled = enabled;
            cbHandshakes.IsEnabled = enabled;
        }

        private void InitControls()
        {
            if (null == _device)
            {
                EnableInputs(false);
                return;
            }

            var portNames = SerialPortConfig.GetPortNames();
            foreach (var s in portNames) { cbPortNames.Items.Add(s); }
            cbPortNames.SelectedIndex = (portNames.Count > 0) ? 0 : -1;

            txtBoadRate.Text = "9600";

            var parities = SerialPortConfig.GetParities();
            foreach (var s in parities) { cbParities.Items.Add(s); }
            cbParities.SelectedIndex = (parities.Count > 0) ? 0 : -1;

            txtDataBit.Text = "8";

            var stopbits = SerialPortConfig.GetStopBits();
            foreach (var s in stopbits) { cbStopBits.Items.Add(s); }
            cbStopBits.SelectedIndex = (stopbits.Count > 0) ? 0 : -1;

            var handshakes = SerialPortConfig.GetHandshakes();
            foreach (var s in handshakes) { cbHandshakes.Items.Add(s); }
            cbHandshakes.SelectedIndex = (handshakes.Count > 0) ? 0 : -1;

            EnableInputs(true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Setup.
        /// </summary>
        /// <param name="device">The device.</param>
        public void Setup(SerialDevice device)
        {
            _device = device;
            InitControls();
        }

        #endregion
    }
}
