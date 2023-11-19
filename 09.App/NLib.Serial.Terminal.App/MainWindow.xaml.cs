#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using NLib.Serial.Devices;
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
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
            InitJIK6CAB();
        }

        private void FreeDevices()
        {
            FreeJIK6CAB();
            FreeWeightSPUN();
            FreeWeightQA();
            FreePHMeter();
            FreeTFO1();
        }

        #endregion

        #region TFO1

        private void InitTFO1()
        {
            TFO1Terminal.Instance.LoadConfig();
            TFO1Terminal.Instance.OnRx += TFO1_OnRx;
            TFO1Page.Setup(TFO1Terminal.Instance);
        }

        private void FreeTFO1()
        {
            TFO1Terminal.Instance.Disconnect();
            TFO1Terminal.Instance.OnRx -= TFO1_OnRx;
        }

        private void TFO1_OnRx(object sender, EventArgs e)
        {
            /*
            lock (TFO1Terminal.Instance)
            {
                var buffers = TFO1Terminal.Instance.Queues.ToArray();
                TFO1Terminal.Instance.Queues.Clear();

                List<byte> allbytes = new List<byte>();
                var originals = viewer.GetBytes();
                if (null != originals && originals.Length > 0) allbytes.AddRange(originals);
                if (null != buffers && buffers.Length > 0) allbytes.AddRange(buffers);
                viewer.SetBytes(allbytes.ToArray());
            }
            */
        }

        #endregion

        #region PHMeter

        private void InitPHMeter()
        {
            PHMeterTerminal.Instance.LoadConfig();
            PHMeterTerminal.Instance.OnRx += PHMeter_OnRx;
            PHMeterPage.Setup(PHMeterTerminal.Instance);
        }

        private void FreePHMeter()
        {
            PHMeterTerminal.Instance.Disconnect();
            PHMeterTerminal.Instance.OnRx -= PHMeter_OnRx;
        }

        private void PHMeter_OnRx(object sender, EventArgs e)
        {

        }

        #endregion

        #region Weight QA

        private void InitWeightQA()
        {
            WeightQATerminal.Instance.LoadConfig();
            WeightQATerminal.Instance.OnRx += WeightSPUN_OnRx;
            WeightQAPage.Setup(WeightQATerminal.Instance);
        }

        private void FreeWeightQA()
        {
            WeightQATerminal.Instance.Disconnect();
            WeightQATerminal.Instance.OnRx -= WeightQA_OnRx;
        }

        private void WeightQA_OnRx(object sender, EventArgs e)
        {

        }

        #endregion

        #region Weight SPUN

        private void InitWeightSPUN()
        {
            WeightSPUNTerminal.Instance.LoadConfig();
            WeightSPUNTerminal.Instance.OnRx += WeightSPUN_OnRx;
            WeightSPUNPage.Setup(WeightSPUNTerminal.Instance);
        }

        private void FreeWeightSPUN()
        {
            WeightSPUNTerminal.Instance.Disconnect();
            WeightSPUNTerminal.Instance.OnRx -= WeightSPUN_OnRx;
        }

        private void WeightSPUN_OnRx(object sender, EventArgs e)
        {

        }

        #endregion

        #region JIK6CAB

        private void InitJIK6CAB()
        {
            JIK6CABTerminal.Instance.LoadConfig();
            JIK6CABTerminal.Instance.OnRx += JIK6CAB_OnRx;
            JIK6CABPage.Setup(JIK6CABTerminal.Instance);
        }

        private void FreeJIK6CAB()
        {
            JIK6CABTerminal.Instance.Disconnect();
            JIK6CABTerminal.Instance.OnRx -= JIK6CAB_OnRx;
        }

        private void JIK6CAB_OnRx(object sender, EventArgs e)
        {

        }

        #endregion

        #endregion
    }
}
