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
using NLib.Serial.Terminals;

#endregion

namespace NLib.Serial.Terminal.App.Controls
{
    /// <summary>
    /// Interaction logic for MettlerMS204TS00TerminalPage.xaml
    /// </summary>
    public partial class MettlerMS204TS00TerminalPage : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MettlerMS204TS00TerminalPage()
        {
            InitializeComponent();
        }

        #endregion

        #region Loaded/Unloader

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MettlerMS204TS00Terminal.Instance.OnRx += Instance_OnRx;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            MettlerMS204TS00Terminal.Instance.OnRx -= Instance_OnRx;
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
            var val = MettlerMS204TS00Terminal.Instance.Value;
            txtW.Text = val.W.ToString("0.0000");
            txtMode.Text = val.Mode;
            txtUnit.Text = val.Unit;
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
