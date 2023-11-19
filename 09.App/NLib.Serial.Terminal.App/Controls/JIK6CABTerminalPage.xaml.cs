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
using NLib.Serial.Terminals;

#endregion

namespace NLib.Serial.Terminal.App.Controls
{
    /// <summary>
    /// Interaction logic for JIK6CABTerminalPage.xaml
    /// </summary>
    public partial class JIK6CABTerminalPage : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public JIK6CABTerminalPage()
        {
            InitializeComponent();
        }

        #endregion

        #region Loaded/Unloader

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            JIK6CABTerminal.Instance.OnRx += Instance_OnRx;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            JIK6CABTerminal.Instance.OnRx -= Instance_OnRx;
        }

        #endregion

        #region Rx

        private void Instance_OnRx(object sender, EventArgs e)
        {
            UpdateTextBoxs();
        }

        #endregion

        #region Private Methods

        private void UpdateTextBoxs()
        {
            var val = JIK6CABTerminal.Instance.Value;
            txtDateTime.Text = val.Date.ToString("yyyy-MM-dd HH:mm",
                System.Globalization.DateTimeFormatInfo.InvariantInfo);
            txtTW.Text = val.TW.ToString("n2");
            txtTU.Text = val.TUnit;
            txtNW.Text = val.NW.ToString("n2");
            txtNU.Text = val.NUnit;
            txtGW.Text = val.GW.ToString("n2");
            txtGU.Text = val.GUnit;
            txtPCS.Text = val.PCS.ToString("n2");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Setup.
        /// </summary>
        /// <param name="device">The device terminal.</param>
        public void Setup(ISerialDeviceTerminal device)
        {
            ctrlSetting.Setup(device);
        }

        #endregion
    }
}
