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
            InitComboboxs();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

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

        private void Connect()
        {

        }

        private void Disconnect()
        {

        }

        #endregion
    }
}
