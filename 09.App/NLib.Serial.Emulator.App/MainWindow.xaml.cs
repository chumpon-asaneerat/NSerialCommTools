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
using NLib.Serial.Emulator.App.Controls;
using NLib.Serial.Emulators;
using NLib.Serial.Terminals;

#endregion

namespace NLib
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Loaded/Unloaded

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            InitDevices();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            FreeDevices();
        }

        #endregion

        #region Private Methods

        #region Commons

        private void InitDevices()
        {
            InitTFO1();
            InitPHMeter();
            InitWeightQA();
            InitWeightSPUN();
        }

        private void FreeDevices()
        {
            FreeWeightSPUN();
            FreeWeightQA();
            FreePHMeter();
            FreeTFO1();
        }

        #endregion

        #region TFO1

        private void InitTFO1()
        {
            TFO1Device.Instance.LoadConfig();
            TFO1Page.Setup(TFO1Device.Instance);
        }

        private void FreeTFO1()
        {
            TFO1Device.Instance.Shutdown();
        }

        #endregion

        #region PHMeter

        private void InitPHMeter()
        {
            PHMeterDevice.Instance.LoadConfig();
            PHMeterPage.Setup(PHMeterDevice.Instance);
        }

        private void FreePHMeter()
        {
            PHMeterDevice.Instance.Shutdown();
        }

        #endregion

        #region Weight QA

        private void InitWeightQA()
        {
            WeightQADevice.Instance.LoadConfig();
            WeightQAPage.Setup(WeightQADevice.Instance);
        }

        private void FreeWeightQA()
        {
            WeightQADevice.Instance.Shutdown();
        }

        #endregion

        #region Weight SPUN

        private void InitWeightSPUN()
        {
            WeightSPUNDevice.Instance.LoadConfig();
            WeightSPUNPage.Setup(WeightSPUNDevice.Instance);
        }

        private void FreeWeightSPUN()
        {
            WeightSPUNDevice.Instance.Shutdown();
        }

        #endregion

        #endregion

        private void cmdTFO1Send_Click(object sender, RoutedEventArgs e)
        {
            var data = TFO1Device.Instance.Value;
            data.F = 0;
            data.A = 366;
            data.W0 = 23;
            data.W4 = (decimal)343.5;
            var buffers = data.ToByteArray();
            TFO1Device.Instance.Send(buffers);
        }
    }
}
