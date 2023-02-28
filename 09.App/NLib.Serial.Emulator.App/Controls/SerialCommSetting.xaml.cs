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
