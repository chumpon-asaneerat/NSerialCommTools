#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

        #endregion

        #region Private Methods

        private void Connect()
        {

        }

        private void Disconnect()
        {

        }

        #endregion
    }
}
