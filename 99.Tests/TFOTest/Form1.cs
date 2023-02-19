#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO.Ports;
using System.Globalization;
using System.Threading;

#endregion

namespace TFOTest
{
    public partial class Form1 : Form
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        #endregion

        #region Form Load/Closing

        private void Form1_Load(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            InitComboboxs();
        }

        private void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            isExit = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isExit = true;
            Disconnect();
            AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
        }

        #endregion

        #region Button Handlers

        private void cmdConnectDisconnect_Click(object sender, EventArgs e)
        {
            ToggleConnection();
        }

        #endregion

        #region Internal Variables

        private SerialPort _comm = null;
        private Thread _readThread = null;
        private bool isExit = false;

        #endregion

        #region Private Methods

        private void InitComboboxs()
        {
            // init ports
            foreach (string s in SerialPort.GetPortNames())
            {
                cbPorts.Items.Add(s);
            }
            cbPorts.SelectedIndex = (cbPorts.Items.Count > 0) ? 0 : -1;

            // init parity bits
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                cbParityBits.Items.Add(s);
            }
            cbParityBits.SelectedIndex = (cbParityBits.Items.Count > 0) ? 0 : -1;

            // init stop bits
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                cbStopBits.Items.Add(s);
            }
            cbStopBits.SelectedIndex = (cbStopBits.Items.Count > 0) ? cbStopBits.Items.IndexOf("One") : -1;

            // init handshakes
            foreach (string s in Enum.GetNames(typeof(Handshake)))
            {
                cbHandshakes.Items.Add(s);
            }
            cbHandshakes.SelectedIndex = (cbHandshakes.Items.Count > 0) ? 0 : -1;
        }

        private void log(string msg)
        {
            string time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fffff ", CultureInfo.InvariantCulture);
            txtLog.Text += time + msg + Environment.NewLine;
        }

        private void Clearlog()
        {
            txtLog.Text = string.Empty;
        }

        private void Connect()
        {
            if (null != _comm)
                return; // already connected.

            Clearlog();

            // Create Serial ports
            try
            {
                _comm = new SerialPort();
                _comm.PortName = cbPorts.SelectedItem.ToString();
                _comm.BaudRate = int.Parse(txtBaudRate.Text);
                _comm.Parity = (Parity)Enum.Parse(typeof(Parity), cbParityBits.SelectedItem.ToString(), true);
                _comm.DataBits = int.Parse(txtDataBits.Text);
                _comm.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cbStopBits.SelectedItem.ToString(), true);
                _comm.Handshake = (Handshake)Enum.Parse(typeof(Handshake), cbHandshakes.SelectedItem.ToString(), true);
            }
            catch (Exception ex)
            {
                log(ex.ToString());
            }

            string msg = string.Format("Open port {0} - {1}, {2}, {3}, {4}",
                _comm.PortName, _comm.BaudRate, _comm.Parity, _comm.DataBits, _comm.StopBits);
            log(msg);

            try
            {
                _comm.Open();
            }
            catch (Exception ex2)
            {
                log(ex2.ToString());
            }


            if (null != _comm && !_comm.IsOpen)
            {
                log("Cannot open port");
                try 
                { 
                    _comm.Close();
                    _comm.Dispose();
                }
                catch { }
                _comm = null;
                return; // cannot open port
            }
            // Change button text
            cmdConnectDisconnect.Text = "Disconnect";
            // Create Read Thread
            CreateReadThread();
        }

        private void Disconnect()
        {
            // Free Thread
            FreeReadThread();
            // Free Serial ports
            if (null != _comm)
            {
                string msg = string.Format("Close port {0}", _comm.PortName);
                log(msg);

                try
                {
                    _comm.Close();
                    _comm.Dispose(); 
                }
                catch { }
            }
            _comm = null;

            // Change button text
            cmdConnectDisconnect.Text = "Connect";
        }

        private void ToggleConnection()
        {
            if (IsConnected)
            {
                Disconnect();
            }
            else
            {
                Connect();
            }
        }

        private bool IsConnected { get { return null != _comm && _comm.IsOpen; } }

        private void CreateReadThread()
        {
            // Create Read Thread
            _readThread = new Thread(ReadProcessing);
            _readThread.Name = "Serial read thread";
            _readThread.IsBackground = true;

            _readThread.Start();
        }

        private void FreeReadThread()
        {
            // Free Thread
            if (null != _readThread)
            {
                try { _readThread.Abort(); }
                catch (ThreadAbortException) { Thread.ResetAbort(); }
            }
            _readThread = null;
        }

        private void ReadProcessing()
        {
            while (null != _readThread && !isExit)
            {

                Thread.Sleep(50);
                Application.DoEvents();
            }
            // Free Thread
            FreeReadThread();
        }

        #endregion
    }
}
