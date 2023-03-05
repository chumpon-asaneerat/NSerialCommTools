#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
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
            InitTimer();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            FreeTimer();
            FreeDevices();
        }

        #endregion

        #region Internal Variables

        private Timer _timer;
        private bool _onSync = false;

        #endregion

        #region Private Methods

        #region Timers

        private void InitTimer()
        {
            if (null != _timer) FreeTimer();
            _timer = new Timer();
            _timer.Interval = 500; // 500 ms.
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void FreeTimer()
        {
            if (null != _timer)
            {
                try
                {
                    _timer.Stop();
                    _timer.Elapsed -= _timer_Elapsed;
                    _timer.Dispose();
                }
                catch { }
            }
            _timer = null;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_onSync)
                return;

            _onSync = true;

            Dispatcher.Invoke(new Action(() => { SyncAndSendAll(); }));

            _onSync = false;
        }

        #endregion

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

        private void SyncAndSendAll()
        {
            SendTFO1();
            SendPHMeter();
            SendWeightQA();
            SendWeightSPUN();
        }

        #endregion

        #region TFO1

        private void InitTFO1()
        {
            TFO1Page.Setup();
        }

        private void FreeTFO1()
        {
            TFO1Device.Instance.Shutdown();
        }

        private void SendTFO1()
        {
            TFO1Page.Sync();
        }

        #endregion

        #region PHMeter

        private void InitPHMeter()
        {
            PHMeterPage.Setup();
        }

        private void FreePHMeter()
        {
            PHMeterDevice.Instance.Shutdown();
        }

        private void SendPHMeter()
        {
            PHMeterPage.Sync();
        }

        #endregion

        #region Weight QA

        private void InitWeightQA()
        {
            WeightQAPage.Setup();
        }

        private void FreeWeightQA()
        {
            WeightQADevice.Instance.Shutdown();
        }

        private void SendWeightQA()
        {
            WeightQAPage.Sync();
        }

        #endregion

        #region Weight SPUN

        private void InitWeightSPUN()
        {
            WeightSPUNPage.Setup();
        }

        private void FreeWeightSPUN()
        {
            WeightSPUNDevice.Instance.Shutdown();
        }

        private void SendWeightSPUN()
        {
            WeightSPUNPage.Sync();
        }

        #endregion

        #endregion
    }
}
