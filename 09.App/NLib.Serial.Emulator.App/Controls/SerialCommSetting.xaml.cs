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
using NLib.Serial.Emulators;

#endregion

namespace NLib.Serial.Emulator.App.Controls
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

        private ISerialDeviceEmulator _device = null;

        #endregion

        #region Button Handlers

        private void cmdTogleConnect_Click(object sender, RoutedEventArgs e)
        {
            if (null == _device)
                return;
            if (!_device.IsOpen)
            {
                // current is Shutdown so Start
                UpdateCurrentSetting();
                _device.Start();
            }
            else
            {
                // current is Started so Shutdown
                _device.Shutdown();
            }

            cmdTogleConnect.Content = (!_device.IsOpen) ? "Start" : "Shutdown";
        }

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

        private void UpdateCurrentSetting()
        {
            if (null == _device || null == _device.Config)
                return;
            try
            {
                _device.Config.PortName = cbPortNames.SelectedItem.ToString();
                _device.Config.BaudRate = int.Parse(txtBoadRate.Text);
                _device.Config.Parity = SerialPortConfig.GetParity(cbStopBits.SelectedItem.ToString());
                _device.Config.DataBits = int.Parse(txtDataBit.Text);
                _device.Config.StopBits = SerialPortConfig.GetStopBits(cbStopBits.SelectedItem.ToString());
                _device.Config.Handshake = SerialPortConfig.GetHandshake(cbHandshakes.SelectedItem.ToString());

                _device.SaveConfig();
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Setup.
        /// </summary>
        /// <param name="device">The device emulator.</param>
        public void Setup(ISerialDeviceEmulator device)
        {
            _device = device;
            InitControls();
        }

        #endregion
    }
}
