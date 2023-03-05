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
    /// Interaction logic for TFO1EmulatorPage.xaml
    /// </summary>
    public partial class TFO1EmulatorPage : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public TFO1EmulatorPage()
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
            TFO1Device.Instance.LoadConfig();
            ctrlSetting.Setup(TFO1Device.Instance);
        }
        /// <summary>
        /// Sync content to device.
        /// </summary>
        public void Sync()
        {
            if (!TFO1Device.Instance.IsOpen) return;
            if (onSync) return;

            onSync = true;

            DateTime dt = DateTime.Now;
            
            txtC.Text = dt.ToString("yyyy-MM-dd HH:mm", 
                System.Globalization.DateTimeFormatInfo.InvariantInfo);

            var data = TFO1Device.Instance.Value;
            try
            {
                data.F = decimal.Parse(txtF.Text);
                data.H = decimal.Parse(txtH.Text);
                data.Q = decimal.Parse(txtQ.Text);
                data.X = decimal.Parse(txtX.Text);
                data.A = decimal.Parse(txtA.Text);
                data.W0 = decimal.Parse(txtW0.Text);
                data.W4 = decimal.Parse(txtW4.Text);
                data.W1 = decimal.Parse(txtW1.Text);
                data.W2 = int.Parse(txtW2.Text);
                //data.B = byte.Parse(txtB.Text);
                data.C = dt;
                //data.V = byte.Parse(txtV.Text);
            }
            catch { }

            var buffers = data.ToByteArray();
            TFO1Device.Instance.Send(buffers);

            onSync = false;
        }

        #endregion
    }
}
