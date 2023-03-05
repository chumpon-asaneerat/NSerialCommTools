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
    /// Interaction logic for TFO1TerminalPage.xaml
    /// </summary>
    public partial class TFO1TerminalPage : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public TFO1TerminalPage()
        {
            InitializeComponent();
        }

        #endregion

        #region Loaded/Unloader

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TFO1Terminal.Instance.OnRx += Instance_OnRx;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            TFO1Terminal.Instance.OnRx -= Instance_OnRx;
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
            var val = TFO1Terminal.Instance.Value;
            txtF.Text = val.F.ToString("n1");
            txtH.Text = val.H.ToString("n1");
            txtQ.Text = val.Q.ToString("n1");
            txtX.Text = val.X.ToString("n1");
            txtA.Text = val.A.ToString("n1");
            txtW0.Text = val.W0.ToString("n1");
            txtW4.Text = val.W4.ToString("n1");
            txtW1.Text = val.W1.ToString("n1");
            txtW2.Text = val.W2.ToString("n0");
            txtB.Text = val.B.ToString();
            txtC.Text = val.C.ToString("yyyy-MM-dd HH:mm", 
                System.Globalization.DateTimeFormatInfo.InvariantInfo);
            txtV.Text = val.V.ToString();
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
