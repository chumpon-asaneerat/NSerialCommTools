#region Using

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

#endregion

using NLib.Devices.SerialPorts;

namespace NLib.Forms.Devices.SerialPorts
{
	/// <summary>
	/// Terminal Form
	/// </summary>
	public class TermForm : System.Windows.Forms.Form
	{
		#region Internal Variable

		private bool inEscape = false;
		private string esc;
		private string mark;
		private InfoForm info;
		private CommTerminal baseterm;

		#endregion

		/// <summary>
		/// Get/Set Terminal (BaseTerm)
		/// </summary>
		public CommTerminal Terminal
		{
			get { return baseterm; }
			internal set { baseterm = value; }
		}

		#region Generate Code

		private System.Windows.Forms.TextBox textBoxS;
		private System.Windows.Forms.Button buttonO;
		private System.Windows.Forms.Button buttonStatus;
		private System.Windows.Forms.Button buttonSet;
		private System.Windows.Forms.RichTextBox textBoxR;
		private System.Windows.Forms.Button buttonClr;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonRLSD;
		private System.Windows.Forms.RadioButton radioButtonDSR;
		private System.Windows.Forms.RadioButton radioButtonCTS;
		private System.Windows.Forms.CheckBox checkBoxBK;
		private System.Windows.Forms.CheckBox checkBoxDTR;
		private System.Windows.Forms.CheckBox checkBoxRTS;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.RadioButton radioButtonRng;
		private System.Windows.Forms.Button buttonImm;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Constructor
		/// </summary>
		public TermForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (baseterm != null) baseterm.Close();
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.textBoxS = new System.Windows.Forms.TextBox();
			this.buttonO = new System.Windows.Forms.Button();
			this.buttonStatus = new System.Windows.Forms.Button();
			this.buttonSet = new System.Windows.Forms.Button();
			this.textBoxR = new System.Windows.Forms.RichTextBox();
			this.buttonClr = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioButtonRng = new System.Windows.Forms.RadioButton();
			this.checkBoxBK = new System.Windows.Forms.CheckBox();
			this.checkBoxDTR = new System.Windows.Forms.CheckBox();
			this.checkBoxRTS = new System.Windows.Forms.CheckBox();
			this.radioButtonRLSD = new System.Windows.Forms.RadioButton();
			this.radioButtonDSR = new System.Windows.Forms.RadioButton();
			this.radioButtonCTS = new System.Windows.Forms.RadioButton();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.buttonImm = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxS
			// 
			this.textBoxS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxS.BackColor = System.Drawing.Color.White;
			this.textBoxS.CausesValidation = false;
			this.textBoxS.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxS.Location = new System.Drawing.Point(10, 9);
			this.textBoxS.Name = "textBoxS";
			this.textBoxS.ReadOnly = true;
			this.textBoxS.Size = new System.Drawing.Size(559, 26);
			this.textBoxS.TabIndex = 0;
			this.textBoxS.TabStop = false;
			this.toolTip.SetToolTip(this.textBoxS, "Type character to send, or type \'<\' then ASCII control name or number 0..255, the" +
		"n \'>\' to send. Type \'<\' twice for that character");
			// 
			// buttonO
			// 
			this.buttonO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonO.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonO.Location = new System.Drawing.Point(10, 326);
			this.buttonO.Name = "buttonO";
			this.buttonO.Size = new System.Drawing.Size(90, 32);
			this.buttonO.TabIndex = 3;
			this.buttonO.TabStop = false;
			this.buttonO.Tag = "0";
			this.buttonO.Text = "Offline";
			this.toolTip.SetToolTip(this.buttonO, "Press to change between Offline and Online");
			this.buttonO.Click += new System.EventHandler(this.buttonO_Click);
			// 
			// buttonStatus
			// 
			this.buttonStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonStatus.Enabled = false;
			this.buttonStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonStatus.Location = new System.Drawing.Point(413, 327);
			this.buttonStatus.Name = "buttonStatus";
			this.buttonStatus.Size = new System.Drawing.Size(90, 31);
			this.buttonStatus.TabIndex = 11;
			this.buttonStatus.TabStop = false;
			this.buttonStatus.Text = "Status";
			this.toolTip.SetToolTip(this.buttonStatus, "Press to show the status of the transmission and reception queues");
			this.buttonStatus.Click += new System.EventHandler(this.buttonStatus_Click);
			// 
			// buttonSet
			// 
			this.buttonSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonSet.Location = new System.Drawing.Point(10, 290);
			this.buttonSet.Name = "buttonSet";
			this.buttonSet.Size = new System.Drawing.Size(90, 30);
			this.buttonSet.TabIndex = 12;
			this.buttonSet.TabStop = false;
			this.buttonSet.Text = "Settings";
			this.toolTip.SetToolTip(this.buttonSet, "Press to show the settings dialog");
			this.buttonSet.Click += new System.EventHandler(this.buttonSet_Click);
			// 
			// textBoxR
			// 
			this.textBoxR.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxR.BackColor = System.Drawing.Color.LightGray;
			this.textBoxR.DetectUrls = false;
			this.textBoxR.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxR.HideSelection = false;
			this.textBoxR.Location = new System.Drawing.Point(10, 44);
			this.textBoxR.Name = "textBoxR";
			this.textBoxR.ReadOnly = true;
			this.textBoxR.ShowSelectionMargin = true;
			this.textBoxR.Size = new System.Drawing.Size(658, 228);
			this.textBoxR.TabIndex = 13;
			this.textBoxR.Text = "";
			// 
			// buttonClr
			// 
			this.buttonClr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonClr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonClr.Location = new System.Drawing.Point(413, 290);
			this.buttonClr.Name = "buttonClr";
			this.buttonClr.Size = new System.Drawing.Size(90, 30);
			this.buttonClr.TabIndex = 14;
			this.buttonClr.TabStop = false;
			this.buttonClr.Text = "Clear";
			this.toolTip.SetToolTip(this.buttonClr, "Press to clear the send and receive text boxes");
			this.buttonClr.Click += new System.EventHandler(this.buttonClr_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.radioButtonRng);
			this.groupBox1.Controls.Add(this.checkBoxBK);
			this.groupBox1.Controls.Add(this.checkBoxDTR);
			this.groupBox1.Controls.Add(this.checkBoxRTS);
			this.groupBox1.Controls.Add(this.radioButtonRLSD);
			this.groupBox1.Controls.Add(this.radioButtonDSR);
			this.groupBox1.Controls.Add(this.radioButtonCTS);
			this.groupBox1.Location = new System.Drawing.Point(108, 284);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(298, 71);
			this.groupBox1.TabIndex = 15;
			this.groupBox1.TabStop = false;
			// 
			// radioButtonRng
			// 
			this.radioButtonRng.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioButtonRng.AutoCheck = false;
			this.radioButtonRng.Enabled = false;
			this.radioButtonRng.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radioButtonRng.Location = new System.Drawing.Point(221, 16);
			this.radioButtonRng.Name = "radioButtonRng";
			this.radioButtonRng.Size = new System.Drawing.Size(67, 22);
			this.radioButtonRng.TabIndex = 15;
			this.radioButtonRng.Text = "Ring";
			this.toolTip.SetToolTip(this.radioButtonRng, "Ringing detection input");
			// 
			// checkBoxBK
			// 
			this.checkBoxBK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxBK.Enabled = false;
			this.checkBoxBK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBoxBK.Location = new System.Drawing.Point(192, 44);
			this.checkBoxBK.Name = "checkBoxBK";
			this.checkBoxBK.Size = new System.Drawing.Size(67, 18);
			this.checkBoxBK.TabIndex = 14;
			this.checkBoxBK.Tag = "2";
			this.checkBoxBK.Text = "Break";
			this.toolTip.SetToolTip(this.checkBoxBK, "Break condition on transmission output");
			this.checkBoxBK.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			// 
			// checkBoxDTR
			// 
			this.checkBoxDTR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxDTR.Enabled = false;
			this.checkBoxDTR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBoxDTR.Location = new System.Drawing.Point(115, 44);
			this.checkBoxDTR.Name = "checkBoxDTR";
			this.checkBoxDTR.Size = new System.Drawing.Size(67, 18);
			this.checkBoxDTR.TabIndex = 13;
			this.checkBoxDTR.Tag = "1";
			this.checkBoxDTR.Text = "DTR";
			this.toolTip.SetToolTip(this.checkBoxDTR, "Data Terminal Ready output");
			this.checkBoxDTR.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			// 
			// checkBoxRTS
			// 
			this.checkBoxRTS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxRTS.Enabled = false;
			this.checkBoxRTS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBoxRTS.Location = new System.Drawing.Point(38, 44);
			this.checkBoxRTS.Name = "checkBoxRTS";
			this.checkBoxRTS.Size = new System.Drawing.Size(68, 18);
			this.checkBoxRTS.TabIndex = 12;
			this.checkBoxRTS.Tag = "0";
			this.checkBoxRTS.Text = "RTS";
			this.toolTip.SetToolTip(this.checkBoxRTS, "Request To Send output");
			this.checkBoxRTS.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			// 
			// radioButtonRLSD
			// 
			this.radioButtonRLSD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioButtonRLSD.AutoCheck = false;
			this.radioButtonRLSD.Enabled = false;
			this.radioButtonRLSD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radioButtonRLSD.Location = new System.Drawing.Point(148, 16);
			this.radioButtonRLSD.Name = "radioButtonRLSD";
			this.radioButtonRLSD.Size = new System.Drawing.Size(67, 22);
			this.radioButtonRLSD.TabIndex = 10;
			this.radioButtonRLSD.Text = "RLSD";
			this.toolTip.SetToolTip(this.radioButtonRLSD, "Receive Line Signal Detect input");
			// 
			// radioButtonDSR
			// 
			this.radioButtonDSR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioButtonDSR.AutoCheck = false;
			this.radioButtonDSR.Enabled = false;
			this.radioButtonDSR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radioButtonDSR.Location = new System.Drawing.Point(80, 16);
			this.radioButtonDSR.Name = "radioButtonDSR";
			this.radioButtonDSR.Size = new System.Drawing.Size(68, 22);
			this.radioButtonDSR.TabIndex = 9;
			this.radioButtonDSR.Text = "DSR";
			this.toolTip.SetToolTip(this.radioButtonDSR, "Data Set Ready input");
			// 
			// radioButtonCTS
			// 
			this.radioButtonCTS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioButtonCTS.AutoCheck = false;
			this.radioButtonCTS.Enabled = false;
			this.radioButtonCTS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radioButtonCTS.Location = new System.Drawing.Point(13, 16);
			this.radioButtonCTS.Name = "radioButtonCTS";
			this.radioButtonCTS.Size = new System.Drawing.Size(67, 22);
			this.radioButtonCTS.TabIndex = 8;
			this.radioButtonCTS.Text = "CTS";
			this.toolTip.SetToolTip(this.radioButtonCTS, "Clear To Send input");
			// 
			// buttonImm
			// 
			this.buttonImm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonImm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonImm.Location = new System.Drawing.Point(579, 7);
			this.buttonImm.Name = "buttonImm";
			this.buttonImm.Size = new System.Drawing.Size(90, 31);
			this.buttonImm.TabIndex = 16;
			this.buttonImm.Tag = "0";
			this.buttonImm.Text = "Queued";
			this.toolTip.SetToolTip(this.buttonImm, "Press to change between Queued and Immediate transmission.");
			this.buttonImm.Click += new System.EventHandler(this.buttonImm_Click);
			// 
			// TermForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.ClientSize = new System.Drawing.Size(677, 375);
			this.Controls.Add(this.buttonImm);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonClr);
			this.Controls.Add(this.textBoxR);
			this.Controls.Add(this.buttonSet);
			this.Controls.Add(this.buttonStatus);
			this.Controls.Add(this.buttonO);
			this.Controls.Add(this.textBoxS);
			this.KeyPreview = true;
			this.Name = "TermForm";
			this.Text = "BaseTerm: Offline";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TermForm_FormClosing);
			this.Load += new System.EventHandler(this.BaseTermForm_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form_KeyPress);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#endregion

		private void Form_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (baseterm == null) return;
			byte c;
			c = (byte)e.KeyChar;
			if ((c == 13) && (!inEscape))
			{
				textBoxS.AppendText("<CR>");
				baseterm.SendCtrl("CR");
				return;
			}
			if ((c < 0x20) || (c > 0x7E)) return;
			if (inEscape)
			{
				if (c == (byte)'<')
				{
					baseterm.SendChar(c);
					textBoxS.Text = mark;
					inEscape = false;
					textBoxS.BackColor = Color.White;
				}
				else
				{
					if (c == (byte)'>')
					{
						if (!baseterm.SendCtrl(esc))
						{
							textBoxS.Text = mark;
							c = 0;
						}
						inEscape = false;
						textBoxS.BackColor = Color.White;
					} 
					else 
					{
						esc += ((char)c).ToString();
					}
				}
			} 
			else 
			{
				if (c == (byte)'<')
				{
					inEscape = true;
					esc = "";
					mark = textBoxS.Text;
					textBoxS.BackColor = Color.Yellow;
				}
				else
				{
					baseterm.SendChar(c);
				}
			}
			if (c >= (byte)' ') textBoxS.AppendText(e.KeyChar.ToString());
			e.Handled = true;
		}

		private void buttonO_Click(object sender, System.EventArgs e)
		{
			if (baseterm == null) return;
			if (buttonO.Tag.ToString() == "1")
			{
				if (info != null) info.Close();
				info = null;
				baseterm.Close();
				buttonO.Tag = "0";
				buttonO.Text = "Offline";
			}
			else
			{
				try
				{
					if (baseterm.Open())
					{
						ShowMsg(">>>> ONLINE");
						buttonO.Tag = "1";
						buttonO.Text = "Online";
					}
					else
					{
						ShowMsg(">>>> PORT UNAVAILABLE");
					}
				}
				catch (CommPortException ex)
				{
					ShowException(ex);
				}
			}
			textBoxS.Focus();
		}

		/// <summary>
		/// ShowException
		/// </summary>
		/// <param name="e"></param>
		public void ShowException(CommPortException e)
		{
			Color c = textBoxR.SelectionColor;
			textBoxR.SelectionColor = Color.Red;
			textBoxR.AppendText("\r\n>>>> EXCEPTION\r\n");
			textBoxR.SelectionColor = Color.Red;
			textBoxR.AppendText(e.Message);
			if (e.InnerException != null)
			{
				textBoxR.AppendText("\r\n");
				textBoxR.SelectionColor = Color.Red;
				textBoxR.AppendText(e.InnerException.Message);
			}
			textBoxR.SelectionColor = Color.Red;
			textBoxR.AppendText("\r\n>>>> END EXCEPTION\r\n");
			textBoxR.SelectionColor = c;
		}

		// JH 1.3: Follow the rules by marshalling cross-thread calls using BeginInvoke. No problems reported
		// with not doing this, but I should follow the rules and set a good example!

		private delegate void InvokeDelegate(string s, int t);

		/// <summary>
		/// ShowChar
		/// </summary>
		/// <param name="s"></param>
		/// <param name="nl"></param>
		public void ShowChar(string s, bool nl)
		{
			object[] p = new object[2];
			int t = nl? 1: 2;
			p[0] = s;
			p[1] = t;
			if (this.InvokeRequired)
				this.BeginInvoke(new InvokeDelegate(InvokeFunction), p);
			else
				InvokeFunction(s, t);
		}

		/// <summary>
		/// ShowMsg
		/// </summary>
		/// <param name="s"></param>
		public void ShowMsg(string s)
		{
			object[] p = new object[2];
			int t = 3;
			p[0] = s;
			p[1] = t;
			if (this.InvokeRequired)
				this.BeginInvoke(new InvokeDelegate(InvokeFunction), p);
			else
				InvokeFunction(s, t);
		}

		/// <summary>
		/// SetIndics
		/// </summary>
		/// <param name="CTS"></param>
		/// <param name="DSR"></param>
		/// <param name="RLSD"></param>
		/// <param name="Rng"></param>
		public void SetIndics(bool CTS, bool DSR, bool RLSD, bool Rng)
		{
			object[] p = new object[2];
			string s = (CTS? "1":"0") + (DSR? "1":"0") + (RLSD? "1":"0") + (Rng? "1":"0");
			int t = 4;
			p[0] = s;
			p[1] = t;
			if (this.InvokeRequired)
				this.BeginInvoke(new InvokeDelegate(InvokeFunction), p);
			else
				InvokeFunction(s, t);
		}

		private void InvokeFunction(string s, int t)
		{
			switch (t)
			{
				case 1:
					textBoxR.AppendText(s);
					textBoxR.AppendText("\r\n");
					break;
				case 2:
					textBoxR.AppendText(s);
					break;
				case 3:
					Color c = textBoxR.SelectionColor;
					textBoxR.SelectionColor = Color.Green;
					textBoxR.AppendText("\r\n" + s + "\r\n");
					textBoxR.SelectionColor = c;
					break;
				case 4:
					radioButtonCTS.Checked = s.Substring(0, 1) == "1";
					radioButtonDSR.Checked = s.Substring(1, 1) == "1";
					radioButtonRLSD.Checked = s.Substring(2, 1) == "1";
					radioButtonRng.Checked = s.Substring(3, 1) == "1";
					break;
			}
		}

		//========

		private void Form_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Back)
			{
				textBoxS.Text = "";
				e.Handled = true;
			}
			if (e.KeyCode == Keys.Delete)
			{
				textBoxS.Text = "";
				textBoxR.Text = "";
				e.Handled = true;
			}
		}

		private void checkBox_CheckedChanged(object sender, System.EventArgs e)
		{
			if (baseterm == null) return;
			baseterm.OPClick((CheckBox)sender);
		}

		/// <summary>
		/// OnClose
		/// </summary>
		public void OnClose()
		{
			textBoxR.BackColor = Color.LightGray;

			buttonStatus.Enabled = false;
			buttonSet.Enabled = true;

			radioButtonCTS.Checked = false;
			radioButtonDSR.Checked = false;
			radioButtonRLSD.Checked = false;
			radioButtonRng.Checked = false;
			checkBoxRTS.Checked = false;
			checkBoxDTR.Checked = false;
			checkBoxBK.Checked = false;

			radioButtonCTS.Enabled = false;
			radioButtonDSR.Enabled = false;
			radioButtonRLSD.Enabled = false;
			radioButtonRng.Enabled = false;
			checkBoxRTS.Enabled = false;
			checkBoxDTR.Enabled = false;
			checkBoxBK.Enabled = false;
			buttonO.Tag = "0";
			buttonO.Text = "Offline";

			if (baseterm == null) 
			{
				this.Text = "Terminal: Not Assigned";
				return;
			}
			
			this.Text = "Terminal: Offline";
		}

		/// <summary>
		/// OnOpen
		/// </summary>
		public void OnOpen()
		{
			buttonO.Tag = "1";
			buttonO.Text = "Online";
			radioButtonCTS.Enabled = true;
			radioButtonDSR.Enabled = true;
			radioButtonRLSD.Enabled = true;
			radioButtonRng.Enabled = true;
			if (baseterm != null)
			{
				baseterm.setOPTicks(checkBoxDTR);
				baseterm.setOPTicks(checkBoxRTS);
				baseterm.setOPTicks(checkBoxBK);
			}
			buttonStatus.Enabled = true;
			buttonSet.Enabled = false;
			textBoxR.BackColor = Color.White;
			if (baseterm == null || baseterm.Settings == null)
			{
				this.Text = "Terminal: Not Assigned";
				return;
			}
			this.Text = "Terminal Online: " + baseterm.Settings.Common.Port;
		}

		private void buttonStatus_Click(object sender, System.EventArgs e)
		{
			if (info == null) info = new InfoForm();
			if (info.IsDisposed) info = new InfoForm();
			info.SetTerminal(this.Terminal); // set terminal.
			info.Show();
			textBoxS.Focus();
		}

		private void buttonSet_Click(object sender, System.EventArgs e)
		{
			if (baseterm == null) return;
			baseterm.ShowSettings();
			textBoxS.Focus();
		}

		private void buttonClr_Click(object sender, System.EventArgs e)
		{
			textBoxS.Text = "";
			textBoxR.Text = "";
			inEscape = false;
			esc = "";
			mark = "";
			textBoxS.BackColor = Color.White;
			textBoxS.Focus();
		}

		private void BaseTermForm_Load(object sender, System.EventArgs e)
		{
			textBoxS.Focus();
		}

		private void buttonImm_Click(object sender, System.EventArgs e)
		{
			if (baseterm == null) return;
			if (buttonImm.Tag.ToString() == "0")
			{
				buttonImm.Tag = "1";
				buttonImm.Text = "Immediate";
				baseterm.Immediate = true;
			}
			else
			{
				buttonImm.Tag = "0";
				buttonImm.Text = "Queued";
				baseterm.Immediate = false;
			}
			textBoxS.Focus();
		}

		private void TermForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (null != info && !info.IsDisposed && !info.Disposing)
			{
				info.Close();
			}
			info = null;
		}
	}
}
