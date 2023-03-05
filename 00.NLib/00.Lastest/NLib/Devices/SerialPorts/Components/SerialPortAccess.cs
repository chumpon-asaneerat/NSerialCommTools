#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2016-06-17
=================
- Serial Communication : SerialPortAccess component code updated.
  - Changes log code (used MethodBase).

======================================================================================================================
Update 2014-10-18
=================
- Serial Communication : SerialPortAccess component code updated.
  - Change namespace from SysLib.Components.Devices.SerialPorts to 
	NLib.Components.Devices.SerialPorts.
  - SerialPortAccess class change Exception handle related code to used new version of 
	Log Framework.
  - Change call DelegateInvoker to ApplicationManager.Instance.Invoke.

======================================================================================================================
Update 2010-01-22
=================
- Serial Communication : SerialPortAccess component code updated.
  - SerialPortAccess class change Exception handle related code to used new version of 
	ExceptionManager.

======================================================================================================================
Update 2010-01-18
=================
- Serial Communication : SerialPortAccess component code updated.
  - All code related to debug now used DebugManager instead.

======================================================================================================================
Update 2010-01-02
=================
- Serial Communication : SerialPortAccess component changed.
  - Change namespace from SysLib.Components.Hardwares.SerialPorts to
	SysLib.Components.Devices.SerialPorts.

======================================================================================================================
Update 2008-07-06
=================
- Serial Communication : SerialPortAccess component.
  - SerialPortAccess component is ported.
  - SerialPortAccess component log code is temporary removed.

======================================================================================================================
Update 2008-07-06
=================
- Serial Communication : SerialPortAccess new method add.
  - Add new method ApplyDefaultSetting for easy set the port setting with 9600, 8, n, 1 mode.

======================================================================================================================
Update 2008-07-05
=================
- Serial Communication : SerialPortAccess Fixed bug
  - Fixed bug cross thread error when RX.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Data;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using NLib.Devices.SerialPorts;

#endregion

namespace NLib.Components.Devices.SerialPorts
{
	#region Delegate

	/// <summary>
	/// TerminalStatusChangedEventHandler delegate
	/// </summary>
	public delegate void TerminalStatusChangedEventHandler(bool cts, bool dsr, bool rlsd, bool ring);
	/// <summary>
	/// TerminalRxCharEventHandler delegate
	/// </summary>
	public delegate void TerminalRxCharEventHandler(string ch, bool newline);
	/// <summary>
	/// TerminalExceptionEventHandler delegate
	/// </summary>
	public delegate void TerminalExceptionEventHandler(Exception e);
	/// <summary>
	/// TerminalMessageEventHandler delegate
	/// </summary>
	public delegate void TerminalMessageEventHandler(string message);

	#endregion

	#region SerialPortAccess

	/// <summary>
	/// Serial Port Access Component
	/// </summary>
	/// <include file='Devices\SerialPorts\Examples\Examples.xml' path='Comment/Member[@name="SerialPortAccess"]/*'/>
	[ToolboxItem(true)]
	public class SerialPortAccess : Component
	{
		#region Internal variable

		private BaseTerminal terminal = null;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor
		/// </summary>
		public SerialPortAccess()
		{
			if (null == terminal)
			{
				terminal = new BaseTerminal();
				hookPort();
			}
		}
		/// <summary>
		/// Destructor
		/// </summary>
		~SerialPortAccess()
		{
			// Free Memory
			NGC.FreeGC(this);
		}

		#endregion

		#region terminal Event Handler

		private void hookPort()
		{
			if (null == terminal)
				return;
			terminal.OnBreak += new System.EventHandler(portOnBreak);
			terminal.OnCommException += new CommExceptionEventHandler(portException);
			terminal.OnCommMessage += new CommMessageEventHandler(portMessage);
			terminal.OnPortClosing += new System.EventHandler(portOnPortClosing);
			terminal.OnPortOpenning += new System.EventHandler(portOnPortOpenning);
			terminal.OnRxChar += new RxCharEventHandler(portOnRxChar);
			terminal.OnStatusChanged += new CommStatusChangedEventHandler(portStatusChanged);
		}

		private void unhookPort()
		{
			if (null == terminal)
				return;
			terminal.OnBreak -= new System.EventHandler(portOnBreak);
			terminal.OnCommException -= new CommExceptionEventHandler(portException);
			terminal.OnCommMessage -= new CommMessageEventHandler(portMessage);
			terminal.OnPortClosing -= new System.EventHandler(portOnPortClosing);
			terminal.OnPortOpenning -= new System.EventHandler(portOnPortOpenning);
			terminal.OnRxChar -= new RxCharEventHandler(portOnRxChar);
			terminal.OnStatusChanged -= new CommStatusChangedEventHandler(portStatusChanged);
		}

		private void portOnPortOpenning(object sender, System.EventArgs e)
		{
			//if (OnTerminalOpenning != null) OnTerminalOpenning(sender, e);
			ApplicationManager.Instance.Invoke(OnTerminalOpenning, sender, e);
		}

		private void portOnPortClosing(object sender, System.EventArgs e)
		{
			//if (OnTerminalClosing != null) OnTerminalClosing(sender, e);
			ApplicationManager.Instance.Invoke(OnTerminalClosing, sender, e);
		}

		private void portOnBreak(object sender, System.EventArgs e)
		{
			//if (OnTerminalBreak != null) OnTerminalBreak(sender, e);
			ApplicationManager.Instance.Invoke(OnTerminalBreak, sender, e);
		}

		private void portStatusChanged(bool cts, bool dsr, bool rlsd, bool ring)
		{
			//if (OnTerminalStatusChanged != null) OnTerminalStatusChanged(cts, dsr, rlsd, ring);
			ApplicationManager.Instance.Invoke(OnTerminalStatusChanged, cts, dsr, rlsd, ring);
		}

		private void portOnRxChar(string ch, bool newline)
		{
			//if (OnTerminalRx != null) OnTerminalRx(ch, newline);
			ApplicationManager.Instance.Invoke(OnTerminalRx, ch, newline);
		}

		private void portException(Exception ex)
		{
			//if (OnTerminalException != null) OnTerminalException(ex);
			ApplicationManager.Instance.Invoke(OnTerminalException, ex);
		}

		private void portMessage(string message)
		{
			//if (OnTerminalMessage != null) OnTerminalMessage(message);
			ApplicationManager.Instance.Invoke(OnTerminalMessage, message);
		}

		#endregion

		#region Public Method and Property

		/// <summary>
		/// Opens the com port and configures it with the required settings
		/// </summary>
		/// <returns>false if the port could not be opened</returns>
		public bool Open()
		{
			if (null == terminal)
			{
				terminal = new BaseTerminal();
				hookPort();
			}
			return terminal.Open();
		}
		/// <summary>
		/// Closes the com port.
		/// </summary>
		public void Close()
		{
			if (null != terminal)
			{
				MethodBase med = MethodBase.GetCurrentMethod();

				unhookPort();
				try { terminal.Close(); }
				catch (Exception ex) { ex.Err(med); }
				try { terminal.Dispose(); }
				catch (Exception ex) { ex.Err(med); }
			}
			// Free resource.
			NGC.FreeGC(terminal);
			terminal = null;
			NGC.FreeGC(this);
		}
		/// <summary>
		/// Block until all bytes in the queue have been transmitted.
		/// </summary>
		public void Flush()
		{
			if (null != terminal)
			{
				terminal.Flush();
			}
		}
		/// <summary>
		/// Get the status of the queues
		/// </summary>
		/// <returns>Queue status object</returns>
		public QueueStatus GetQueueStatus()
		{
			if (null != terminal)
			{
				return terminal.GetQueueStatus();
			}
			else return new QueueStatus();
		}
		/// <summary>
		/// Get/Set Immediate Mode.
		/// </summary>
		[Category("SerialPorts")]
		[Description("Get/Set Immediate Mode.")]
		public bool Immediate
		{
			get 
			{
				if (null == terminal)
					return false;
				return terminal.Immediate; 
			}
			set 
			{
				if (null != terminal)
				{
					terminal.Immediate = value;
				}
			}
		}
		/// <summary>
		/// Test if the line is congested (data being queued for send faster than it is being dequeued)
		/// This detects if baud rate is too slow or if handshaking is not allowing enough transmission
		/// time. It should be called at reasonably long fixed intervals. If data has been sent during
		/// the interval, congestion is reported if the queue was never empty during the interval.
		/// </summary>
		/// <returns>True if congested</returns>
		[Browsable(false)]
		public bool IsCongested()
		{
			if (null != terminal)
			{
				return terminal.IsCongested();
			}
			else return false;
		}
		/// <summary>
		/// Check Port is avaliable
		/// </summary>
		/// <param name="portname">name of port</param>
		/// <returns>Port Status</returns>
		public PortStatus IsPortAvailable(string portname)
		{
			if (null != terminal)
			{
				return terminal.IsPortAvailable(portname);
			}
			else return PortStatus.unavailable;
		}
		/// <summary>
		/// True if online.
		/// </summary>
		[Browsable(false)]
		public bool Online
		{
			get 
			{
				if (null == terminal)
					return false;
				return terminal.Online; 
			}
		}
		/// <summary>
		/// Send
		/// </summary>
		/// <param name="dataToSend">Data to Send in string</param>
		/// <param name="includeNewLine">Include new line character at and of byte.</param>
		public void Send(string dataToSend, bool includeNewLine)
		{
			Send(System.Text.ASCIIEncoding.ASCII.GetBytes(dataToSend), includeNewLine);
		}
		/// <summary>
		/// Send
		/// </summary>
		/// <param name="dataToSend">Data to Send in string</param>
		public void Send(string dataToSend)
		{
			Send(dataToSend, true);
		}
		/// <summary>
		/// Send
		/// </summary>
		/// <param name="buffers">Data to Send in byte array</param>
		public void Send(byte[] buffers)
		{
			Send(buffers, false);
		}
		/// <summary>
		/// Send
		/// </summary>
		/// <param name="buffers">Data to Send in byte array</param>
		/// <param name="includeNewLine">Include new line character at and of byte.</param>
		public void Send(byte[] buffers, bool includeNewLine)
		{
			if (null == terminal || !this.Online || buffers == null || buffers.Length <= 0)
				return; // No data


			try
			{
				// Send string
				for (int i = 0; i < buffers.Length; i++)
				{
					this.SendChar(buffers[i]);
				}
				if (includeNewLine)
				{
					this.SendChar(0x0D); // CR
					this.SendChar(0x0A); // LF
				}
			}
			catch (Exception ex)
			{
				MethodBase med = MethodBase.GetCurrentMethod();
				string msg = "Detected Send error.";
				ex.Err(med, msg);
			}
		}
		/// <summary>
		/// Send Character to port
		/// </summary>
		/// <param name="c">Byte to send.</param>
		public void SendChar(byte c)
		{
			if (null != terminal)
			{
				terminal.SendChar(c);
			}
		}
		/// <summary>
		/// Send control's string to port
		/// </summary>
		/// <param name="s">Control string to send.</param>
		public void SendCtrl(string s)
		{
			if (null != terminal)
			{
				terminal.SendCtrl(s);
			}
		}
		/// <summary>
		/// Send FileStream to port
		/// </summary>
		/// <param name="fs">The FileStream object.</param>
		public void SendFile(FileStream fs)
		{
			if (null != terminal)
			{
				terminal.SendFile(fs);
			}
		}
		/// <summary>
		/// Gets Port Settings instance.
		/// </summary>
		[Browsable(false)]
		public SerialPortSettings Settings
		{
			get 
			{
				if (null == terminal)
					return null;
				return terminal.Settings; 
			}
		}
		/// <summary>
		/// Apply Default Setting. The default setting is assign the serial port
		/// with baudrate -> 9600, parity -> none, data bits -> 8, stop bits -> one
		/// or (9600, 8, n, 1) and this method will also set Immediate property to true and set
		/// autoReport setting to false.
		/// Note when call this method if the port is online
		/// it's will automatically close.
		/// </summary>
		/// <param name="portName">The serial port name. like COM1:</param>
		public void ApplyDefaultSetting(string portName)
		{
			bool isOnline = this.Online;

			if (isOnline)
				Close();
			terminal = null;
			// create new instance terminal
			terminal = new BaseTerminal();
			try
			{
				this.Settings.SetStandard(portName, 9600, Handshake.XonXoff);
				this.Immediate = true;
				this.Settings.Common.AutoReopen = false;
			}
			catch (Exception ex)
			{
				MethodBase med = MethodBase.GetCurrentMethod();
				string msg = "Detected error when apply default setting.";
				ex.Err(med, msg);
			}
			hookPort();
		}
		/// <summary>
		/// Load Setting
		/// </summary>
		/// <param name="configfile">The config file name.</param>
		public void LoadSetting(string configfile)
		{
			if (terminal != null)
			{
				Close();
			}
			terminal = null;
			// create new instance terminal
			terminal = new BaseTerminal();
			SerialPortConfig cfg = new SerialPortConfig();
			if (cfg.LoadFromFile(configfile) && cfg.Settings != null)
			{
				terminal.Settings = cfg.Settings;

				if (OnTerminalSettingChanged != null)
					OnTerminalSettingChanged(this, System.EventArgs.Empty);
			}
			else
			{
				MethodBase med = MethodBase.GetCurrentMethod();
				"Error cannot load xml file".Err(med);
			}
			hookPort();
		}
		/// <summary>
		/// Save Setting
		/// </summary>
		/// <param name="configfile">The config file name.</param>
		public void SaveSetting(string configfile)
		{
			if (terminal != null)
			{
				try
				{
					SerialPortConfig cfg = new SerialPortConfig();
					cfg.Settings = terminal.Settings;
					cfg.SaveToFile(configfile);
				}
				catch (Exception ex)
				{
					MethodBase med = MethodBase.GetCurrentMethod();
					ex.Err(med, "Detected Save XML error.");
				}
			}
		}

		#endregion

		#region Event

		/// <summary>
		/// OnTerminalSettingChanged event.
		/// </summary>
		[Category("SerialPorts")]
		[Description("Raise OnTerminalSettingChanged event.")]
		public event System.EventHandler OnTerminalSettingChanged;
		/// <summary>
		/// OnTerminalOpenning event.
		/// </summary>
		[Category("SerialPorts")]
		[Description("Raise OnTerminalOpenning event.")]
		public event System.EventHandler OnTerminalOpenning;
		/// <summary>
		/// OnTerminalClosing event.
		/// </summary>
		[Category("SerialPorts")]
		[Description("Raise OnTerminalClosing event.")]
		public event System.EventHandler OnTerminalClosing;
		/// <summary>
		/// OnTerminalBreak event.
		/// </summary>
		[Category("SerialPorts")]
		[Description("Raise OnTerminalBreak event.")]
		public event System.EventHandler OnTerminalBreak;
		/// <summary>
		/// OnTerminalRx event.
		/// </summary>
		[Category("SerialPorts")]
		[Description("Raise OnTerminalRx event.")]
		public event TerminalRxCharEventHandler OnTerminalRx;
		/// <summary>
		/// OnTerminalStatusChanged event.
		/// </summary>
		[Category("SerialPorts")]
		[Description("Raise OnTerminalStatusChanged event.")]
		public event TerminalStatusChangedEventHandler OnTerminalStatusChanged;
		/// <summary>
		/// OnTerminalMessage event.
		/// </summary>
		[Category("SerialPorts")]
		[Description("Raise OnTerminalMessage event.")]
		public event TerminalMessageEventHandler OnTerminalMessage;
		/// <summary>
		/// OnTerminalException event.
		/// </summary>
		[Category("SerialPorts")]
		[Description("Raise OnTerminalException event.")]
		public event TerminalExceptionEventHandler OnTerminalException;

		#endregion
	}

	#endregion
}
