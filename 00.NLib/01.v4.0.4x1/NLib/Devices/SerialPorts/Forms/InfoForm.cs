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
	/// Information Form
	/// </summary>
	internal class InfoForm : System.Windows.Forms.Form
	{
		#region Generate Code

		private System.Timers.Timer timer;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxTxS;
		private System.Windows.Forms.TextBox textBoxTxQ;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxTxI;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelTxS;
		private System.Windows.Forms.Label labelCongested;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textBoxRxS;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxRxQ;
		private System.Windows.Forms.Label label5;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Constructor
		/// </summary>
		public InfoForm()
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
				if(components != null)
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
			this.timer = new System.Timers.Timer();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelCongested = new System.Windows.Forms.Label();
			this.labelTxS = new System.Windows.Forms.Label();
			this.textBoxTxI = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxTxQ = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxTxS = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textBoxRxQ = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxRxS = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.timer)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// timer
			// 
			this.timer.Enabled = true;
			this.timer.Interval = 1000D;
			this.timer.SynchronizingObject = this;
			this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Elapsed);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelCongested);
			this.groupBox1.Controls.Add(this.labelTxS);
			this.groupBox1.Controls.Add(this.textBoxTxI);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.textBoxTxQ);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.textBoxTxS);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(53, 13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(211, 148);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Transmission Queue";
			// 
			// labelCongested
			// 
			this.labelCongested.Location = new System.Drawing.Point(10, 120);
			this.labelCongested.Name = "labelCongested";
			this.labelCongested.Size = new System.Drawing.Size(192, 18);
			this.labelCongested.TabIndex = 7;
			this.labelCongested.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelTxS
			// 
			this.labelTxS.Location = new System.Drawing.Point(10, 102);
			this.labelTxS.Name = "labelTxS";
			this.labelTxS.Size = new System.Drawing.Size(192, 18);
			this.labelTxS.TabIndex = 6;
			this.labelTxS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textBoxTxI
			// 
			this.textBoxTxI.Location = new System.Drawing.Point(154, 74);
			this.textBoxTxI.Name = "textBoxTxI";
			this.textBoxTxI.ReadOnly = true;
			this.textBoxTxI.Size = new System.Drawing.Size(38, 22);
			this.textBoxTxI.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 76);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(134, 19);
			this.label3.TabIndex = 4;
			this.label3.Text = "Immediate Buffer:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxTxQ
			// 
			this.textBoxTxQ.Location = new System.Drawing.Point(86, 46);
			this.textBoxTxQ.Name = "textBoxTxQ";
			this.textBoxTxQ.ReadOnly = true;
			this.textBoxTxQ.Size = new System.Drawing.Size(106, 22);
			this.textBoxTxQ.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(76, 19);
			this.label2.TabIndex = 2;
			this.label2.Text = "Queued:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxTxS
			// 
			this.textBoxTxS.Location = new System.Drawing.Point(86, 18);
			this.textBoxTxS.Name = "textBoxTxS";
			this.textBoxTxS.ReadOnly = true;
			this.textBoxTxS.Size = new System.Drawing.Size(106, 22);
			this.textBoxTxS.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(76, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "Size:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textBoxRxQ);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.textBoxRxS);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Location = new System.Drawing.Point(53, 174);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(211, 83);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Reception Queue";
			// 
			// textBoxRxQ
			// 
			this.textBoxRxQ.Location = new System.Drawing.Point(91, 46);
			this.textBoxRxQ.Name = "textBoxRxQ";
			this.textBoxRxQ.ReadOnly = true;
			this.textBoxRxQ.Size = new System.Drawing.Size(106, 22);
			this.textBoxRxQ.TabIndex = 5;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(14, 48);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(77, 19);
			this.label5.TabIndex = 4;
			this.label5.Text = "Queued:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxRxS
			// 
			this.textBoxRxS.Location = new System.Drawing.Point(91, 18);
			this.textBoxRxS.Name = "textBoxRxS";
			this.textBoxRxS.ReadOnly = true;
			this.textBoxRxS.Size = new System.Drawing.Size(106, 22);
			this.textBoxRxS.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(14, 21);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(77, 18);
			this.label4.TabIndex = 2;
			this.label4.Text = "Size:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// InfoForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.ClientSize = new System.Drawing.Size(311, 280);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "InfoForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "CommBase Queue Status";
			this.TopMost = true;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.InfoForm_Closing);
			((System.ComponentModel.ISupportInitialize)(this.timer)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		#endregion

		#region Setting()

		private CommTerminal term = null;

		/// <summary>
		/// Set Terminal
		/// </summary>
		/// <param name="terminal"></param>
		public void SetTerminal(CommTerminal terminal)
		{
			term = terminal;
			if (term != null)
				this.Text = "BaseTerm Q Status: " + term.Settings.Common.Port;
			else
				this.Text = "BaseTerm Q Status: none.";
			Update();
		}

		#endregion

		private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			Update();
		}

		private new void Update()
		{
			if (term == null) return;
			long v; string s;
			QueueStatus q = term.GetQueueStatus();
			v = q.OutQueueSize;
			textBoxTxS.Text = (v > 0)? v.ToString() : "Unknown";
			textBoxTxQ.Text = q.OutQueue.ToString();
			textBoxTxI.Text = q.immediateWaiting? "1" : "0";
			s = "";
			if (q.ctsHold) s = s + "CTS ";
			if (q.dsrHold) s = s + "DSR ";
			if (q.rlsdHold) s = s + "RLSD ";
			if (q.ctsHold) s = s + "RxXoff ";
			if (q.ctsHold) s = s + "TxXoff ";

			if (s == "")
				labelTxS.Text = "Transmitting";
			else
				labelTxS.Text = "Blocked: " + s;

			if (term.IsCongested())
				labelCongested.Text = "Congested";
			else
				labelCongested.Text = "Not Congested";
			v = q.InQueueSize;
			textBoxRxS.Text = (v > 0)? v.ToString() : "Unknown";
			textBoxRxQ.Text = q.InQueue.ToString();
		}

		private void InfoForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			timer.Enabled = false;
		}
	}
}
