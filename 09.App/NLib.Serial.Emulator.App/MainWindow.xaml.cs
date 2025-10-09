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

        #region Loaded/Closing

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            InitDevices();
            InitTimer();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
            InitJIK6CAB();
            InitDEFENDER3000();
            InitMettlerMS204TS00();
            InitTScaleNHB();
            InitTScaleQHW();
        }

        private void FreeDevices()
        {
            FreeTScaleQHW();
            FreeTScaleNHB();
            FreeMettlerMS204TS00();
            FreeDEFENDER3000();
            FreeJIK6CAB();
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
            SendJIK6CAB();
            SendDEFENDER3000();
            SendMettlerMS204TS00();
            SendTScaleNHB();
            SendTScaleQHW();
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

        #region JIK6CAB

        private void InitJIK6CAB()
        {
            JIK6CABPage.Setup();
        }

        private void FreeJIK6CAB()
        {
            JIK6CABDevice.Instance.Shutdown();
        }

        private void SendJIK6CAB()
        {
            JIK6CABPage.Sync();
        }

        #endregion

        #region DEFENDER3000

        private void InitDEFENDER3000()
        {
            CordDEFENDER3000Page.Setup();
        }

        private void FreeDEFENDER3000()
        {
            CordDEFENDER3000Device.Instance.Shutdown();
        }

        private void SendDEFENDER3000()
        {
            CordDEFENDER3000Page.Sync();
        }

        #endregion

        #region MettlerMS204TS00

        private void InitMettlerMS204TS00()
        {
            MettlerMS204TS00Page.Setup();
        }

        private void FreeMettlerMS204TS00()
        {
            MettlerMS204TS00Device.Instance.Shutdown();
        }

        private void SendMettlerMS204TS00()
        {
            MettlerMS204TS00Page.Sync();
        }

        #endregion

        #region TScaleNHB

        private void InitTScaleNHB()
        {
            TScaleNHBPage.Setup();
        }

        private void FreeTScaleNHB()
        {
            TScaleNHBDevice.Instance.Shutdown();
        }

        private void SendTScaleNHB()
        {
            TScaleNHBPage.Sync();
        }

        #endregion

        #region TScaleQHW

        private void InitTScaleQHW()
        {
            TScaleQHWPage.Setup();
        }

        private void FreeTScaleQHW()
        {
            TScaleQHWDevice.Instance.Shutdown();
        }

        private void SendTScaleQHW()
        {
            TScaleQHWPage.Sync();
        }

        #endregion

        #endregion
    }
}
