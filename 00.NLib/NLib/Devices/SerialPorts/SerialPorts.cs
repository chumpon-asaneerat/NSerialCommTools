#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2016-06-17
=================
- Serial Communication : Serial Port Access Library's related classes changed.
  - Changes log code (used MethodBase).

======================================================================================================================
Update 2014-10-18
=================
- Serial Communication : Serial Port Access Library's related classes changed.
  - Change namespace from SysLib.Devices.SerialPorts to NLib.Devices.SerialPorts.
  - Change all related forms resolution.

======================================================================================================================
Update 2010-01-02
=================
- Serial Communication : Serial Port Access Library's related classes changed.
  - Change namespace from SysLib.Hardwares.SerialPorts to SysLib.Devices.SerialPorts.

======================================================================================================================
Update 2009-12-29
=================
- Serial Communication namespace changed.
  - All Forms's namespace is now changes to SysLib.Forms.Hardwares.SerialPorts.
  - All Components's namespace is now changes to SysLib.Components.Hardwares.SerialPorts.
  - All SerialPort related classes 's namespace is now changes to SysLib.Hardwares.SerialPorts.
  - Restructures directory and update example path to group all related classes into one 
	directory.
  - The serial port classs will work on Standard (SE) and Professional version (PRO) only
	The Lite Edition (LE) will not supported.

======================================================================================================================
Update 2009-12-26
=================
- Serial Communication Classes ported from GFA37
  - Serial Ports and related classes is ported.
  - Setting Form/Terminal Form/Info Form class is ported.

======================================================================================================================
Update 2008-07-05
=================
- Serial Communication : CommBase class Fixed bug
  - Fixed bug no RX event received.

======================================================================================================================
Update 2007-12-07
=================
- Serial Communication Classes ported from GFA v27
  - Optimized for used with NET Framework 2.0
  - Tune-up internal worker thread for safe close serial port.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.ComponentModel; 
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using System.Windows.Forms;
using NLib.Design;
using NLib.Xml;

#endregion

using NLib.Forms.Devices.SerialPorts; // For Term Form that used for setting Communication Parameters

namespace NLib.Devices.SerialPorts
{
	#region Delegate

	/// <summary>
	/// CommStatusChangedEventHandler delegate
	/// </summary>
	/// <param name="cts"></param>
	/// <param name="dsr"></param>
	/// <param name="rlsd"></param>
	/// <param name="ring"></param>
	public delegate void CommStatusChangedEventHandler(bool cts, bool dsr, bool rlsd, bool ring);
	/// <summary>
	/// RxCharEventHandler delegate
	/// </summary>
	/// <param name="ch"></param>
	/// <param name="newline"></param>
	public delegate void RxCharEventHandler(string ch, bool newline);
	/// <summary>
	/// CommExceptionEventHandler delegate
	/// </summary>
	/// <param name="e"></param>
	public delegate void CommExceptionEventHandler(Exception e);
	/// <summary>
	/// CommMessageEventHandler delegate
	/// </summary>
	/// <param name="message"></param>
	public delegate void CommMessageEventHandler(string message);

	#endregion

	#region Exception Class

	/// <summary>
	/// Exception used for all errors.
	/// </summary>
	public class CommPortException : ApplicationException
	{
		/// <summary>
		/// Constructor for raising direct exceptions
		/// </summary>
		/// <param name="desc">Description of error</param>
		public CommPortException(string desc) : base(desc) {}
		/// <summary>
		/// Constructor for re-raising exceptions from receive thread
		/// </summary>
		/// <param name="e">Inner exception raised on receive thread</param>
		public CommPortException(Exception e) : base("Receive Thread Exception", e) {}
	}

	#endregion

	#region API Class

	internal class Win32Com 
	{
		/// <summary>
		/// Opening Testing and Closing the Port Handle.
		/// </summary>
		[DllImport("kernel32.dll", SetLastError=true)]
		internal static extern IntPtr CreateFile(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode,
			IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes,
			IntPtr hTemplateFile);

		//Constants for errors:
		internal const UInt32 ERROR_FILE_NOT_FOUND = 2;
		internal const UInt32 ERROR_INVALID_NAME = 123;
		internal const UInt32 ERROR_ACCESS_DENIED = 5;
		internal const UInt32 ERROR_IO_PENDING = 997;
		internal const UInt32 ERROR_IO_INCOMPLETE = 996;

		//Constants for return value:
		internal const Int32 INVALID_HANDLE_VALUE = -1;

		//Constants for dwFlagsAndAttributes:
		internal const UInt32 FILE_FLAG_OVERLAPPED = 0x40000000;

		//Constants for dwCreationDisposition:
		internal const UInt32 OPEN_EXISTING = 3;

		//Constants for dwDesiredAccess:
		internal const UInt32 GENERIC_READ = 0x80000000;
		internal const UInt32 GENERIC_WRITE = 0x40000000;

		[DllImport("kernel32.dll")]
		internal static extern Boolean CloseHandle(IntPtr hObject);

		/// <summary>
		/// Manipulating the communications settings.
		/// </summary>

		[DllImport("kernel32.dll")]
		internal static extern Boolean GetCommState(IntPtr hFile, ref DCB lpDCB);

		[DllImport("kernel32.dll")]
		internal static extern Boolean GetCommTimeouts(IntPtr hFile, out COMMTIMEOUTS lpCommTimeouts);

		[DllImport("kernel32.dll")]
		internal static extern Boolean BuildCommDCBAndTimeouts(String lpDef, ref DCB lpDCB, ref COMMTIMEOUTS lpCommTimeouts);

		[DllImport("kernel32.dll")]
		internal static extern Boolean SetCommState(IntPtr hFile, [In] ref DCB lpDCB);

		[DllImport("kernel32.dll")]
		internal static extern Boolean SetCommTimeouts(IntPtr hFile, [In] ref COMMTIMEOUTS lpCommTimeouts);

		[DllImport("kernel32.dll")]
		internal static extern Boolean SetupComm(IntPtr hFile, UInt32 dwInQueue, UInt32 dwOutQueue);

		[StructLayout( LayoutKind.Sequential )] internal struct COMMTIMEOUTS 
		{
			//JH 1.1: Changed Int32 to UInt32 to allow setting to MAXDWORD
			internal UInt32 ReadIntervalTimeout;
			internal UInt32 ReadTotalTimeoutMultiplier;
			internal UInt32 ReadTotalTimeoutConstant;
			internal UInt32 WriteTotalTimeoutMultiplier;
			internal UInt32 WriteTotalTimeoutConstant;
		}
		//JH 1.1: Added to enable use of "return immediately" timeout.
		internal const UInt32 MAXDWORD = 0xffffffff;

		[StructLayout( LayoutKind.Sequential )] internal struct DCB 
		{
			internal Int32 DCBlength;
			internal Int32 BaudRate;
			internal Int32 PackedValues;
			internal Int16 wReserved;
			internal Int16 XonLim;
			internal Int16 XoffLim;
			internal Byte  ByteSize;
			internal Byte  Parity;
			internal Byte  StopBits;
			internal Byte XonChar;
			internal Byte XoffChar;
			internal Byte ErrorChar;
			internal Byte EofChar;
			internal Byte EvtChar;
			internal Int16 wReserved1;

			internal void init(bool parity, bool outCTS, bool outDSR, int dtr, bool inDSR, bool txc, bool xOut,
				bool xIn, int rts)
			{
				//JH 1.3: Was 0x8001 ans so not setting fAbortOnError - Thanks Larry Delby!
				DCBlength = 28; PackedValues = 0x4001;
				if (parity) PackedValues |= 0x0002;
				if (outCTS) PackedValues |= 0x0004;
				if (outDSR) PackedValues |= 0x0008;
				PackedValues |= ((dtr & 0x0003) << 4);
				if (inDSR) PackedValues |= 0x0040;
				if (txc) PackedValues |= 0x0080;
				if (xOut) PackedValues |= 0x0100;
				if (xIn) PackedValues |= 0x0200;
				PackedValues |= ((rts & 0x0003) << 12);

			}
		}

		/// <summary>
		/// Reading and writing.
		/// </summary>
		[DllImport("kernel32.dll", SetLastError=true)]
		internal static extern Boolean WriteFile(IntPtr fFile, Byte[] lpBuffer, UInt32 nNumberOfBytesToWrite,
			out UInt32 lpNumberOfBytesWritten, IntPtr lpOverlapped);

		[StructLayout( LayoutKind.Sequential )] internal struct OVERLAPPED 
		{
			internal UIntPtr Internal;
			internal UIntPtr InternalHigh;
			internal UInt32 Offset;
			internal UInt32 OffsetHigh;
			internal IntPtr hEvent;
		}

		[DllImport("kernel32.dll")]
		internal static extern Boolean SetCommMask(IntPtr hFile, UInt32 dwEvtMask);

		// Constants for dwEvtMask:
		internal const UInt32 EV_RXCHAR = 0x0001;
		internal const UInt32 EV_RXFLAG = 0x0002;
		internal const UInt32 EV_TXEMPTY = 0x0004;
		internal const UInt32 EV_CTS = 0x0008;
		internal const UInt32 EV_DSR = 0x0010;
		internal const UInt32 EV_RLSD = 0x0020;
		internal const UInt32 EV_BREAK = 0x0040;
		internal const UInt32 EV_ERR = 0x0080;
		internal const UInt32 EV_RING = 0x0100;
		internal const UInt32 EV_PERR = 0x0200;
		internal const UInt32 EV_RX80FULL = 0x0400;
		internal const UInt32 EV_EVENT1 = 0x0800;
		internal const UInt32 EV_EVENT2 = 0x1000;

		[DllImport("kernel32.dll", SetLastError=true)]
		internal static extern Boolean WaitCommEvent(IntPtr hFile, IntPtr lpEvtMask, IntPtr lpOverlapped);

		[DllImport("kernel32.dll")]
		internal static extern Boolean CancelIo(IntPtr hFile);
		
		[DllImport("kernel32.dll", SetLastError=true)]
		internal static extern Boolean ReadFile(IntPtr hFile, [Out] Byte[] lpBuffer, UInt32 nNumberOfBytesToRead,
			out UInt32 nNumberOfBytesRead, IntPtr lpOverlapped);

		[DllImport("kernel32.dll")]
		internal static extern Boolean TransmitCommChar(IntPtr hFile, Byte cChar);

		/// <summary>
		/// Control port functions.
		/// </summary>
		[DllImport("kernel32.dll")]
		internal static extern Boolean EscapeCommFunction(IntPtr hFile, UInt32 dwFunc);

		// Constants for dwFunc:
		internal const UInt32 SETXOFF = 1;
		internal const UInt32 SETXON = 2;
		internal const UInt32 SETRTS = 3;
		internal const UInt32 CLRRTS = 4;
		internal const UInt32 SETDTR = 5;
		internal const UInt32 CLRDTR = 6;
		internal const UInt32 RESETDEV = 7;
		internal const UInt32 SETBREAK = 8;
		internal const UInt32 CLRBREAK = 9;
		
		[DllImport("kernel32.dll")]
		internal static extern Boolean GetCommModemStatus(IntPtr hFile, out UInt32 lpModemStat);

		// Constants for lpModemStat:
		internal const UInt32 MS_CTS_ON = 0x0010;
		internal const UInt32 MS_DSR_ON = 0x0020;
		internal const UInt32 MS_RING_ON = 0x0040;
		internal const UInt32 MS_RLSD_ON = 0x0080;

		/// <summary>
		/// Status Functions.
		/// </summary>
		[DllImport("kernel32.dll", SetLastError=true)]
		internal static extern Boolean GetOverlappedResult(IntPtr hFile, IntPtr lpOverlapped,
			out UInt32 nNumberOfBytesTransferred, Boolean bWait);

		[DllImport("kernel32.dll")]
		internal static extern Boolean ClearCommError(IntPtr hFile, out UInt32 lpErrors, IntPtr lpStat);
		[DllImport("kernel32.dll")]
		internal static extern Boolean ClearCommError(IntPtr hFile, out UInt32 lpErrors, out COMSTAT cs);

		//Constants for lpErrors:
		internal const UInt32 CE_RXOVER = 0x0001;
		internal const UInt32 CE_OVERRUN = 0x0002;
		internal const UInt32 CE_RXPARITY = 0x0004;
		internal const UInt32 CE_FRAME = 0x0008;
		internal const UInt32 CE_BREAK = 0x0010;
		internal const UInt32 CE_TXFULL = 0x0100;
		internal const UInt32 CE_PTO = 0x0200;
		internal const UInt32 CE_IOE = 0x0400;
		internal const UInt32 CE_DNS = 0x0800;
		internal const UInt32 CE_OOP = 0x1000;
		internal const UInt32 CE_MODE = 0x8000;

		[StructLayout( LayoutKind.Sequential )] internal struct COMSTAT 
		{
			internal const uint fCtsHold = 0x1;
			internal const uint fDsrHold = 0x2;
			internal const uint fRlsdHold = 0x4;
			internal const uint fXoffHold = 0x8;
			internal const uint fXoffSent = 0x10;
			internal const uint fEof = 0x20;
			internal const uint fTxim = 0x40;
			internal UInt32 Flags;
			internal UInt32 cbInQue;
			internal UInt32 cbOutQue;
		}
		[DllImport("kernel32.dll")]
		internal static extern Boolean GetCommProperties(IntPtr hFile, out COMMPROP cp);

		[StructLayout( LayoutKind.Sequential )] internal struct COMMPROP
		{
			internal UInt16 wPacketLength; 
			internal UInt16 wPacketVersion; 
			internal UInt32 dwServiceMask; 
			internal UInt32 dwReserved1; 
			internal UInt32 dwMaxTxQueue; 
			internal UInt32 dwMaxRxQueue; 
			internal UInt32 dwMaxBaud; 
			internal UInt32 dwProvSubType; 
			internal UInt32 dwProvCapabilities; 
			internal UInt32 dwSettableParams; 
			internal UInt32 dwSettableBaud; 
			internal UInt16 wSettableData; 
			internal UInt16 wSettableStopParity; 
			internal UInt32 dwCurrentTxQueue; 
			internal UInt32 dwCurrentRxQueue; 
			internal UInt32 dwProvSpec1; 
			internal UInt32 dwProvSpec2; 
			internal Byte wcProvChar; 
		}
	}

	#endregion

	#region Enums

	#region Parity

	/// <summary>
	/// Parity settings
	/// </summary>
	public enum Parity 
	{
		/// <summary>
		/// Characters do not have a parity bit.
		/// </summary>
		none = 0,
		/// <summary>
		/// If there are an odd number of 1s in the data bits, the parity bit is 1.
		/// </summary>
		odd = 1,
		/// <summary>
		/// If there are an even number of 1s in the data bits, the parity bit is 1.
		/// </summary>
		even = 2,
		/// <summary>
		/// The parity bit is always 1.
		/// </summary>
		mark = 3,
		/// <summary>
		/// The parity bit is always 0.
		/// </summary>
		space = 4
	};

	#endregion

	#region StopBits

	/// <summary>
	/// Stop bit settings
	/// </summary>
	public enum StopBits
	{
		/// <summary>
		/// Line is asserted for 1 bit duration at end of each character
		/// </summary>
		one = 0,
		/// <summary>
		/// Line is asserted for 1.5 bit duration at end of each character
		/// </summary>
		onePointFive = 1,
		/// <summary>
		/// Line is asserted for 2 bit duration at end of each character
		/// </summary>
		two = 2
	};

	#endregion

	#region HSOutput

	/// <summary>
	/// Uses for RTS or DTR pins
	/// </summary>
	public enum HSOutput
	{
		/// <summary>
		/// Pin is asserted when this station is able to receive data.
		/// </summary>
		handshake = 2,
		/// <summary>
		/// Pin is asserted when this station is transmitting data (RTS on NT, 2000 or XP only).
		/// </summary>
		gate = 3,
		/// <summary>
		/// Pin is asserted when this station is online (port is open).
		/// </summary>
		online = 1,
		/// <summary>
		/// Pin is never asserted.
		/// </summary>
		none = 0
	};

	#endregion

	#region Handshake

	/// <summary>
	/// Standard handshake methods
	/// </summary>
	public enum Handshake
	{
		/// <summary>
		/// No handshaking
		/// </summary>
		none,
		/// <summary>
		/// Software handshaking using Xon / Xoff
		/// </summary>
		XonXoff,
		/// <summary>
		/// Hardware handshaking using CTS / RTS
		/// </summary>
		CtsRts,
		/// <summary>
		/// Hardware handshaking using DSR / DTR
		/// </summary>
		DsrDtr
	}
		
	#endregion

	#region Ascii

	//JH 1.3: Corrected STH -> STX (Thanks - Johan Thelin!)
	/// <summary>
	/// Byte type with enumeration constants for ASCII control codes.
	/// </summary>
	public enum ASCII : byte
	{
		/// <summary>
		/// NULL
		/// </summary>
		NULL = 0x00,
		/// <summary>
		/// SOH
		/// </summary>
		SOH = 0x01,
		/// <summary>
		/// STX
		/// </summary>
		STX = 0x02,
		/// <summary>
		/// ETX
		/// </summary>
		ETX = 0x03,
		/// <summary>
		/// EOT
		/// </summary>
		EOT = 0x04,
		/// <summary>
		/// ENQ
		/// </summary>
		ENQ = 0x05,
		/// <summary>
		/// ACK
		/// </summary>
		ACK = 0x06,
		/// <summary>
		/// BELL
		/// </summary>
		BELL = 0x07,
		/// <summary>
		/// BS
		/// </summary>
		BS = 0x08,
		/// <summary>
		/// HT
		/// </summary>
		HT = 0x09,
		/// <summary>
		/// LF
		/// </summary>
		LF = 0x0A,
		/// <summary>
		/// VT
		/// </summary>
		VT = 0x0B,
		/// <summary>
		/// FF
		/// </summary>
		FF = 0x0C,
		/// <summary>
		/// CR
		/// </summary>
		CR = 0x0D,
		/// <summary>
		/// SO
		/// </summary>
		SO = 0x0E,
		/// <summary>
		/// SI
		/// </summary>
		SI = 0x0F,
		/// <summary>
		/// DC1
		/// </summary>
		DC1 = 0x11,
		/// <summary>
		/// DC2
		/// </summary>
		DC2 = 0x12,
		/// <summary>
		/// DC3
		/// </summary>
		DC3 = 0x13,
		/// <summary>
		/// DC4
		/// </summary>
		DC4 = 0x14,
		/// <summary>
		/// NAK
		/// </summary>
		NAK = 0x15,
		/// <summary>
		/// SYN
		/// </summary>
		SYN = 0x16,
		/// <summary>
		/// ETB
		/// </summary>
		ETB = 0x17,
		/// <summary>
		/// CAN
		/// </summary>
		CAN = 0x18,
		/// <summary>
		/// EM
		/// </summary>
		EM = 0x19,
		/// <summary>
		/// SUB
		/// </summary>
		SUB = 0x1A,
		/// <summary>
		/// ESC
		/// </summary>
		ESC = 0x1B,
		/// <summary>
		/// FS
		/// </summary>
		FS = 0x1C,
		/// <summary>
		/// GS
		/// </summary>
		GS = 0x1D,
		/// <summary>
		/// RS
		/// </summary>
		RS = 0x1E,
		/// <summary>
		/// US
		/// </summary>
		US = 0x1F,
		/// <summary>
		/// SP
		/// </summary>
		SP = 0x20,
		/// <summary>
		/// DEL
		/// </summary>
		DEL = 0x7F
	}

	#endregion

	#region PortStatus

	/// <summary>
	/// Availability status of a port
	/// </summary>
	public enum PortStatus
	{
		/// <summary>
		/// Port exists but is unavailable (may be open to another program)
		/// </summary>
		unavailable = 0,
		/// <summary>
		/// Available for use
		/// </summary>
		available = 1,
		/// <summary>
		/// Port does not exist
		/// </summary>
		absent = -1
	}

	#endregion
		
	#endregion

	#region Structure

	#region Modem Status Structure

	/// <summary>
	/// Represents the status of the modem control input signals.
	/// </summary>
	public struct ModemStatus
	{
		private uint status;
		internal ModemStatus(uint val) { status = val; }
		/// <summary>
		/// Condition of the Clear To Send signal.
		/// </summary>
		public bool cts { get { return ((status & Win32Com.MS_CTS_ON) != 0); } }
		/// <summary>
		/// Condition of the Data Set Ready signal.
		/// </summary>
		public bool dsr { get { return ((status & Win32Com.MS_DSR_ON) != 0); } }
		/// <summary>
		/// Condition of the Receive Line Status Detection signal.
		/// </summary>
		public bool rlsd { get { return ((status & Win32Com.MS_RLSD_ON) != 0); } }
		/// <summary>
		/// Condition of the Ring Detection signal.
		/// </summary>
		public bool ring { get { return ((status & Win32Com.MS_RING_ON) != 0); } }
	}

	#endregion

	#region QueueStatus Structure

	/// <summary>
	/// Represents the current condition of the port queues.
	/// </summary>
	public struct QueueStatus
	{
		#region Internal Variables
		
		private uint status;
		private uint inQueue;
		private uint outQueue;
		private uint inQueueSize;
		private uint outQueueSize;

		#endregion

		#region Constructor
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="stat"></param>
		/// <param name="inQ"></param>
		/// <param name="outQ"></param>
		/// <param name="inQs"></param>
		/// <param name="outQs"></param>
		internal QueueStatus(uint stat, uint inQ, uint outQ, uint inQs, uint outQs)
		{
			status = stat; 
			inQueue = inQ; 
			outQueue = outQ; 
			inQueueSize = inQs; 
			outQueueSize = outQs;
		}

		#endregion

		#region Public Properties
		
		/// <summary>
		/// Output is blocked by CTS handshaking.
		/// </summary>
		public bool ctsHold { get { return ((status & Win32Com.COMSTAT.fCtsHold) != 0); } }
		/// <summary>
		/// Output is blocked by DRS handshaking.
		/// </summary>
		public bool dsrHold { get { return ((status & Win32Com.COMSTAT.fDsrHold) != 0); } }
		/// <summary>
		/// Output is blocked by RLSD handshaking.
		/// </summary>
		public bool rlsdHold { get { return ((status & Win32Com.COMSTAT.fRlsdHold) != 0); } }
		/// <summary>
		/// Output is blocked because software handshaking is enabled and XOFF was received.
		/// </summary>
		public bool xoffHold { get { return ((status & Win32Com.COMSTAT.fXoffHold) != 0); } }
		/// <summary>
		/// Output was blocked because XOFF was sent and this station is not yet ready to receive.
		/// </summary>
		public bool xoffSent { get { return ((status & Win32Com.COMSTAT.fXoffSent) != 0); } }
		/// <summary>
		/// There is a character waiting for transmission in the immediate buffer.
		/// </summary>
		public bool immediateWaiting { get { return ((status & Win32Com.COMSTAT.fTxim) != 0); } }
		/// <summary>
		/// Number of bytes waiting in the input queue.
		/// </summary>
		public long InQueue { get { return (long)inQueue; } }
		/// <summary>
		/// Number of bytes waiting for transmission.
		/// </summary>
		public long OutQueue { get { return (long)outQueue; } }
		/// <summary>
		/// Total size of input queue (0 means information unavailable)
		/// </summary>
		public long InQueueSize { get { return (long)inQueueSize; } }
		/// <summary>
		/// Total size of output queue (0 means information unavailable)
		/// </summary>
		public long OutQueueSize { get { return (long)outQueueSize; } }

		#endregion
		
		#region ToString

		/// <summary>
		/// ToString
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder m = new StringBuilder("The reception queue is ", 60);
			if (inQueueSize == 0)
			{
				m.Append("of unknown size and ");
			}
			else
			{
				m.Append(inQueueSize.ToString() + " bytes long and ");
			}
			if (inQueue == 0)
			{
				m.Append("is empty.");
			}
			else if (inQueue == 1)
			{
				m.Append("contains 1 byte.");
			}
			else
			{
				m.Append("contains ");
				m.Append(inQueue.ToString());
				m.Append(" bytes.");
			}
			m.Append(" The transmission queue is ");
			if (outQueueSize == 0)
			{
				m.Append("of unknown size and ");
			}
			else
			{
				m.Append(outQueueSize.ToString() + " bytes long and ");
			}
			if (outQueue == 0)
			{
				m.Append("is empty");
			}
			else if (outQueue == 1)
			{
				m.Append("contains 1 byte. It is ");
			}
			else
			{
				m.Append("contains ");
				m.Append(outQueue.ToString());
				m.Append(" bytes. It is ");
			}
			if (outQueue > 0)
			{
				if (ctsHold || dsrHold || rlsdHold || xoffHold || xoffSent)
				{
					m.Append("holding on");
					if (ctsHold) m.Append(" CTS");
					if (dsrHold) m.Append(" DSR");
					if (rlsdHold) m.Append(" RLSD");
					if (xoffHold) m.Append(" Rx XOff");
					if (xoffSent) m.Append(" Tx XOff");
				}
				else
				{
					m.Append("pumping data");
				}
			}
			m.Append(". The immediate buffer is ");
			if (immediateWaiting)
				m.Append("full.");
			else
				m.Append("empty.");
			return m.ToString();
		}

		#endregion
	}

	#endregion

	#endregion

	#region Setting Classes

	#region BasicSetting
	
	/// <summary>
	/// The Serial Port Basic Setting.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
	public class BasicSetting
	{
		#region Constructor
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public BasicSetting()
			: base()
		{
			// Common
			this.Port = "COM1:";
			this.BaudRate = 2400;
			this.Parity = Parity.none;
			this.DataBits = 8;
			this.StopBits = StopBits.one;
			this.AutoReopen = false;
			this.CheckAllSends = true;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Port Name (default: "COM1:")
		/// </summary>
		[Category("Common")]
		[Description("Port Name (default: \"COM1:\")")]
		[XmlAttribute]
		[PropertyOrder(1)]
		public string Port { get; set; }
		/// <summary>
		/// Baud Rate (default: 2400) unsupported rates will throw "Bad settings"
		/// </summary>
		[Category("Common")]
		[Description("Baud Rate (default: 2400) unsupported rates will throw \"Bad settings\"")]
		[XmlAttribute]
		[PropertyOrder(2)]
		public int BaudRate { get; set; }
		/// <summary>
		/// The parity checking scheme (default: none)
		/// </summary>
		[Category("Common")]
		[Description("The parity checking scheme (default: none)")]
		[XmlAttribute]
		[PropertyOrder(3)]
		public Parity Parity { get; set; }
		/// <summary>
		/// Number of databits 1..8 (default: 8) unsupported values will throw "Bad settings"
		/// </summary>
		[Category("Common")]
		[Description("Number of databits 1..8 (default: 8) unsupported values will throw \"Bad settings\"")]
		[XmlAttribute]
		[PropertyOrder(4)]
		public uint DataBits { get; set; }
		/// <summary>
		/// Number of stop bits (default: one)
		/// </summary>
		[Category("Common")]
		[Description("Number of stop bits (default: one)")]
		[XmlAttribute]
		[PropertyOrder(5)]
		public StopBits StopBits { get; set; }
		/// <summary>
		/// If true, the port will automatically re-open on next send if it was previously closed due
		/// to an error (default: false)
		/// </summary>
		[Category("Common")]
		[Description("If true, the port will automatically re-open on next send if it was previously closed due to an error (default: false)")]
		[XmlAttribute]
		[PropertyOrder(6)]
		public bool AutoReopen { get; set; }
		/// <summary>
		/// If true, subsequent Send commands wait for completion of earlier ones enabling the results
		/// to be checked. If false, errors, including timeouts, may not be detected, but performance
		/// may be better.
		/// </summary>
		[Category("Common")]
		[Description("If true, subsequent Send commands wait for completion of earlier ones enabling the results to be checked. If false, errors, including timeouts, may not be detected, but performance may be better.")]
		[XmlAttribute]
		[PropertyOrder(7)]
		public bool CheckAllSends { get; set; }

		#endregion
	}

	#endregion

	#region TxSetting

	/// <summary>
	/// The Serial Port Tx Settings.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
	public class TxSetting
	{
		#region Constructor
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public TxSetting()
			: base()
		{
			// Tx
			this.TxFlowCTS = false;
			this.TxFlowDSR = false;
			this.TxFlowX = false;
			this.TxWhenRxXoff = true;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// If true, transmission is halted unless CTS is asserted by the remote station (default: false)
		/// </summary>
		[Category("Tx")]
		[Description("If true, transmission is halted unless CTS is asserted by the remote station (default: false)")]
		[XmlAttribute]
		[PropertyOrder(1)]
		public bool TxFlowCTS { get; set; }
		/// <summary>
		/// If true, transmission is halted unless DSR is asserted by the remote station (default: false)
		/// </summary>
		[Category("Tx")]
		[Description("If true, transmission is halted unless DSR is asserted by the remote station (default: false)")]
		[XmlAttribute]
		[PropertyOrder(2)]
		public bool TxFlowDSR { get; set; }
		/// <summary>
		/// If true, transmission is halted when Xoff is received and restarted when Xon is received (default: false)
		/// </summary>
		[Category("Tx")]
		[Description("If true, transmission is halted when Xoff is received and restarted when Xon is received (default: false)")]
		[XmlAttribute]
		[PropertyOrder(3)]
		public bool TxFlowX { get; set; }
		/// <summary>
		/// If false, transmission is suspended when this station has sent Xoff to the remote station (default: true)
		/// Set false if the remote station treats any character as an Xon.
		/// </summary>
		[Category("Tx")]
		[Description("If false, transmission is suspended when this station has sent Xoff to the remote station (default: true). Set false if the remote station treats any character as an Xon.")]
		[XmlAttribute]
		[PropertyOrder(4)]
		public bool TxWhenRxXoff { get; set; }

		#endregion
	}

	#endregion

	#region RxSetting

	/// <summary>
	/// The Serial Port Rx Settings.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
	public class RxSetting
	{
		#region Constructor
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public RxSetting()
			: base()
		{
			// Rx
			this.RxGateDSR = false;
			this.RxFlowX = false;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// If true, received characters are ignored unless DSR is asserted by the remote station (default: false)
		/// </summary>
		[Category("Rx")]
		[Description("If true, received characters are ignored unless DSR is asserted by the remote station (default: false)")]
		[XmlAttribute]
		[PropertyOrder(1)]
		public bool RxGateDSR { get; set; }
		/// <summary>
		/// If true, Xon and Xoff characters are sent to control the data flow from the remote station (default: false)
		/// </summary>
		[Category("Rx")]
		[Description("If true, Xon and Xoff characters are sent to control the data flow from the remote station (default: false)")]
		[XmlAttribute]
		[PropertyOrder(2)]
		public bool RxFlowX { get; set; }

		#endregion
	}

	#endregion

	#region HSOutputSetting
	
	/// <summary>
	/// The Serial Port HSOutput Settings.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
	public class HSOutputSetting
	{
		#region Constructor
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public HSOutputSetting()
			: base()
		{
			// HSOutput
			this.UseRTS = HSOutput.none;
			this.UseDTR = HSOutput.none;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Specifies the use to which the RTS output is put (default: none)
		/// </summary>
		[Category("HSOutput")]
		[Description("Specifies the use to which the RTS output is put (default: none)")]
		[XmlAttribute]
		[PropertyOrder(1)]
		public HSOutput UseRTS { get; set; }
		/// <summary>
		/// Specidies the use to which the DTR output is put (default: none)
		/// </summary>
		[Category("HSOutput")]
		[Description("Specidies the use to which the DTR output is put (default: none)")]
		[XmlAttribute]
		[PropertyOrder(2)]
		public HSOutput UseDTR { get; set; }

		#endregion
	}

	#endregion

	#region XOnXOffSetting

	/// <summary>
	/// The Serial Port XOn/XOff Settings.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
	public class XOnXOffSetting
	{
		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public XOnXOffSetting()
			: base()
		{
			// XonXoff
			this.XonChar = ASCII.DC1;
			this.XoffChar = ASCII.DC3;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// The character used to signal Xon for X flow control (default: DC1)
		/// </summary>
		[Category("XOnXOff")]
		[Description("The character used to signal Xon for X flow control (default: DC1)")]
		[XmlAttribute]
		[PropertyOrder(1)]
		public ASCII XonChar { get; set; }
		/// <summary>
		/// The character used to signal Xoff for X flow control (default: DC3)
		/// </summary>
		[Category("XOnXOff")]
		[Description("The character used to signal Xoff for X flow control (default: DC3)")]
		[XmlAttribute]
		[PropertyOrder(2)]
		public ASCII XoffChar { get; set; }

		#endregion
	}

	#endregion

	#region WaterLevelSetting

	/// <summary>
	/// The Serial Port Rx Water Level Settings.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
	public class WaterLevelSetting
	{
		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public WaterLevelSetting()
			: base()
		{
			// Water Level
			this.RxHighWater = 0;
			this.RxLowWater = 0;
		}

		#endregion

		#region Public Properties

		//JH 1.2: Next two defaults changed to 0 to use new defaulting mechanism dependant on queue size.
		/// <summary>
		/// The number of free bytes in the reception queue at which flow is disabled
		/// (Default: 0 = Set to 1/10th of actual rxQueue size)
		/// </summary>
		[Category("Water Level")]
		[Description("The number of free bytes in the reception queue at which flow is disabled (Default: 0 = Set to 1/10th of actual rxQueue size)")]
		[XmlAttribute]
		[PropertyOrder(1)]
		public int RxHighWater { get; set; }
		/// <summary>
		/// The number of bytes in the reception queue at which flow is re-enabled
		/// (Default: 0 = Set to 1/10th of actual rxQueue size)
		/// </summary>
		[Category("Water Level")]
		[Description("The number of bytes in the reception queue at which flow is re-enabled (Default: 0 = Set to 1/10th of actual rxQueue size)")]
		[XmlAttribute]
		[PropertyOrder(2)]
		public int RxLowWater { get; set; }

		#endregion
	}

	#endregion

	#region TimeoutSetting

	/// <summary>
	/// The Serial Port Timeout Settings.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
	public class TimeoutSetting
	{
		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public TimeoutSetting()
			: base()
		{
			// TimeOut
			this.SendTimeoutMultiplier = 0;
			this.SendTimeoutConstant = 0;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Multiplier. Max time for Send in ms = (Multiplier * Characters) + Constant
		/// (default: 0 = No timeout)
		/// </summary>
		[Category("Timeout")]
		[Description("Multiplier. Max time for Send in ms = (Multiplier * Characters) + Constant (default: 0 = No timeout)")]
		[XmlAttribute]
		[PropertyOrder(1)]
		public uint SendTimeoutMultiplier { get; set; }
		/// <summary>
		/// Constant.  Max time for Send in ms = (Multiplier * Characters) + Constant (default: 0)
		/// </summary>
		[Category("Timeout")]
		[Description("Constant.  Max time for Send in ms = (Multiplier * Characters) + Constant (default: 0)")]
		[XmlAttribute]
		[PropertyOrder(2)]
		public uint SendTimeoutConstant { get; set; }

		#endregion
	}

	#endregion

	#region QueueSetting

	/// <summary>
	/// The Serial Port Queue Settings.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
	public class QueueSetting
	{
		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public QueueSetting()
			: base()
		{
			// Queue
			this.RxQueue = 0;
			this.TxQueue = 0;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Requested size for receive queue (default: 0 = use operating system default)
		/// </summary>
		[Category("Queue")]
		[Description("Requested size for receive queue (default: 0 = use operating system default)")]
		[XmlAttribute]
		[PropertyOrder(1)]
		public uint RxQueue { get; set; }
		/// <summary>
		/// Requested size for transmit queue (default: 0 = use operating system default)
		/// </summary>
		[Category("Queue")]
		[Description("Requested size for transmit queue (default: 0 = use operating system default)")]
		[XmlAttribute]
		[PropertyOrder(2)]
		public uint TxQueue { get; set; }

		#endregion
	}

	#endregion

	#region CommLineSetting
	
	/// <summary>
	/// The Serial Port Comm Line Settings.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
	public class CommLineSetting
	{
		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public CommLineSetting()
			: base()
		{
			// CommLine
			this.RxStringBufferSize = 256;
			this.RxTerminator = ASCII.CR;
			this.RxFilter = null;
			this.TransactTimeout = 500;
			this.TxTerminator = null;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Maximum size of received string (default: 256)
		/// </summary>
		[Category("CommLine")]
		[Description("Maximum size of received string (default: 256)")]
		[XmlAttribute]
		[PropertyOrder(1)]
		public uint RxStringBufferSize { get; set; }
		/// <summary>
		/// ASCII code that terminates a received string (default: CR)
		/// </summary>
		[Category("CommLine")]
		[Description("ASCII code that terminates a received string (default: CR)")]
		[XmlAttribute]
		[PropertyOrder(2)]
		public ASCII RxTerminator { get; set; }
		/// <summary>
		/// ASCII codes that will be ignored in received string (default: null)
		/// </summary>
		[Category("CommLine")]
		[Description("ASCII codes that will be ignored in received string (default: null)")]
		[XmlAttribute]
		[PropertyOrder(3)]
		public ASCII[] RxFilter { get; set; }
		/// <summary>
		/// Maximum time (ms) for the Transact method to complete (default: 500)
		/// </summary>
		[Category("CommLine")]
		[Description("Maximum time (ms) for the Transact method to complete (default: 500)")]
		[XmlAttribute]
		[PropertyOrder(4)]
		public uint TransactTimeout { get; set; }
		/// <summary>
		/// ASCII codes transmitted after each Send string (default: null)
		/// </summary>
		[Category("CommLine")]
		[Description("ASCII codes transmitted after each Send string (default: null)")]
		[XmlAttribute]
		[PropertyOrder(5)]
		public ASCII[] TxTerminator { get; set; }

		#endregion
	}

	#endregion

	#region CommPingPongSetting
	
	/// <summary>
	/// The Serial Port Comm Ping-Pong Settings.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
	public class CommPingPongSetting
	{
		#region Constructor
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public CommPingPongSetting()
			: base()
		{
			this.TransactTimeout = 500;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Maximum time (ms) for the Transact method to complete (default: 500)
		/// </summary>
		[Category("CommPingPong")]
		[Description("Maximum time (ms) for the Transact method to complete (default: 500)")]
		[XmlAttribute]
		[PropertyOrder(1)]
		public uint TransactTimeout { get; set; }

		#endregion
	}

	#endregion

	#region TerminalSetting
	
	/// <summary>
	/// The Serial Port Terminal Settings.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
	public class TerminalSetting
	{
		#region Constructor
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public TerminalSetting()
			: base()
		{
			this.ShowAsHex = false;
			this.BreakLineOnChar = false;
			this.LineBreakChar = 0;
			this.CharsInLine = 0;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets show data show as hex.  default is false.
		/// </summary>
		[Category("Terminal")]
		[Description("Gets or sets show data show as hex. default is false.")]
		[XmlAttribute]
		[PropertyOrder(1)]
		public bool ShowAsHex { get; set; }
		/// <summary>
		/// Gets or sets break line on specificed char.  default is false.
		/// </summary>
		[Category("Terminal")]
		[Description("Gets or sets break line on specificed char.  default is false.")]
		[XmlAttribute]
		[PropertyOrder(2)]
		public bool BreakLineOnChar { get; set; }
		/// <summary>
		/// Gets or sets line break char. default is 0.
		/// </summary>
		[Category("Terminal")]
		[Description("Gets or sets line break char. default is 0.")]
		[XmlAttribute]
		[PropertyOrder(3)]
		public ASCII LineBreakChar { get; set; }
		/// <summary>
		/// Gets or sets Chars In Line. default is 0.
		/// </summary>
		[Category("Terminal")]
		[Description("Gets or sets Chars In Line. default is 0.")]
		[XmlAttribute]
		[PropertyOrder(4)]
		public uint CharsInLine { get; set; }

		#endregion
	}

	#endregion
	
	#region SerialPortSettings

	/// <summary>
	/// The Serial Port Settings.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
	public class SerialPortSettings
	{
		#region Internal Variables

		private BasicSetting _basicCfg = null;
		private TxSetting _txCfg = null;
		private RxSetting _rxCfg = null;
		private HSOutputSetting _hsOutputCfg = null;
		private XOnXOffSetting _xonxoffCfg = null;
		private WaterLevelSetting _waterLevelCfg = null;
		private TimeoutSetting _timeoutCfg = null;
		private QueueSetting _queueCfg = null;

		private CommLineSetting _commLineCfg = null;
		private CommPingPongSetting _commPingPongCfg = null;
		private TerminalSetting _termCfg = null;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SerialPortSettings()
			: base()
		{
			InitVars();
		}
		/// <summary>
		/// Destructor.
		/// </summary>
		~SerialPortSettings()
		{
			ResetVars();
		}

		#endregion

		#region Private Methods

		private void InitVars()
		{

		}

		private void ResetVars()
		{
			_basicCfg = null;
			_txCfg = null;
			_rxCfg = null;
			_hsOutputCfg = null;
			_xonxoffCfg = null;
			_waterLevelCfg = null;
			_timeoutCfg = null;
			_queueCfg = null;

			_commLineCfg = null;
			_commPingPongCfg = null;
			_termCfg = null;
		}

		private void CheckBasicVar()
		{
			if (null == _basicCfg)
			{
				lock (this) 
				{ 
					_basicCfg = new BasicSetting(); 
				}
			}
		}

		private void CheckTxVar()
		{
			if (null == _txCfg)
			{
				lock (this)
				{
					_txCfg = new TxSetting();
				}
			}
		}

		private void CheckRxVar()
		{
			if (null == _rxCfg)
			{
				lock (this)
				{
					_rxCfg = new RxSetting();
				}
			}
		}

		private void CheckHSOutputVar()
		{
			if (null == _hsOutputCfg)
			{
				lock (this)
				{
					_hsOutputCfg = new HSOutputSetting();
				}
			}
		}

		private void CheckXOnXoffVar()
		{
			if (null == _xonxoffCfg)
			{
				lock (this)
				{
					_xonxoffCfg = new XOnXOffSetting();
				}
			}
		}

		private void CheckWaterLevelVar()
		{
			if (null == _waterLevelCfg)
			{
				lock (this)
				{
					_waterLevelCfg = new WaterLevelSetting();
				}
			}
		}

		private void CheckTimeoutVar()
		{
			if (null == _timeoutCfg)
			{
				lock (this)
				{
					_timeoutCfg = new TimeoutSetting();
				}
			}
		}

		private void CheckQueueVar()
		{
			if (null == _queueCfg)
			{
				lock (this)
				{
					_queueCfg = new QueueSetting();
				}
			}
		}

		private void CheckCommLineVar()
		{
			if (null == _commLineCfg)
			{
				lock (this)
				{
					_commLineCfg = new CommLineSetting();
				}
			}
		}

		private void CheckCommPingPongVar()
		{
			if (null == _commPingPongCfg)
			{
				lock (this)
				{
					_commPingPongCfg = new CommPingPongSetting();
				}
			}
		}

		private void CheckTermVar()
		{
			if (null == _termCfg)
			{
				lock (this)
				{
					_termCfg = new TerminalSetting();
				}
			}
		}

		#endregion

		#region Public Methods

		#region Port Setting

		/// <summary>
		/// Pre-configures settings for most modern devices: Baud 9600, 8 databits, 1 stop bit, no parity and
		/// one of the common handshake protocols. Change individual settings later if necessary.
		/// </summary>
		/// <param name="Port">The port to use (i.e. "COM1:")</param>
		/// <param name="Baud">The baud rate (Default is 9600).</param>
		/// <param name="Hs">The handshake protocol (Default is XonXoff).</param>
		public void SetStandard(string Port, 
			int Baud = 9600, 
			Handshake Hs = Handshake.XonXoff)
		{
			this.Common.Port = Port;
			this.Common.BaudRate = Baud;
			this.Common.DataBits = 8;
			this.Common.StopBits = StopBits.one;
			this.Common.Parity = Parity.none;
			switch (Hs)
			{
				case Handshake.none:
					this.Tx.TxFlowCTS = false;
					this.Tx.TxFlowDSR = false;
					this.Tx.TxFlowX = false;
					this.Rx.RxFlowX = false;
					this.HSOuput.UseRTS = HSOutput.online;
					this.HSOuput.UseDTR = HSOutput.online;
					this.Tx.TxWhenRxXoff = true;
					this.Rx.RxGateDSR = false;
					break;
				case Handshake.XonXoff:
					this.Tx.TxFlowCTS = false;
					this.Tx.TxFlowDSR = false;
					this.Tx.TxFlowX = true;
					this.Rx.RxFlowX = true;
					this.HSOuput.UseRTS = HSOutput.online;
					this.HSOuput.UseDTR = HSOutput.online;
					this.Tx.TxWhenRxXoff = true;
					this.Rx.RxGateDSR = false;
					this.XOnXoff.XonChar = ASCII.DC1;
					this.XOnXoff.XoffChar = ASCII.DC3;
					break;
				case Handshake.CtsRts:
					this.Tx.TxFlowCTS = true;
					this.Tx.TxFlowDSR = false;
					this.Tx.TxFlowX = false;
					this.Rx.RxFlowX = false;
					this.HSOuput.UseRTS = HSOutput.handshake;
					this.HSOuput.UseDTR = HSOutput.online;
					this.Tx.TxWhenRxXoff = true;
					this.Rx.RxGateDSR = false;
					break;
				case Handshake.DsrDtr:
					this.Tx.TxFlowCTS = false;
					this.Tx.TxFlowDSR = true;
					this.Tx.TxFlowX = false;
					this.Rx.RxFlowX = false;
					this.HSOuput.UseRTS = HSOutput.online;
					this.HSOuput.UseDTR = HSOutput.handshake;
					this.Tx.TxWhenRxXoff = true;
					this.Rx.RxGateDSR = false;
					break;
			}
		}

		#endregion

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets serial port's basic options.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets serial port's basic options.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[XmlElement]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PropertyOrder(1)]
		public BasicSetting Common
		{
			get
			{
				CheckBasicVar();
				return _basicCfg;
			}
			set
			{
				_basicCfg = value;
				CheckBasicVar();
			}
		}
		/// <summary>
		/// Gets or sets serial port's tx options.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets serial port's tx options.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[XmlElement]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PropertyOrder(2)]
		public TxSetting Tx
		{
			get
			{
				CheckTxVar();
				return _txCfg;
			}
			set
			{
				_txCfg = value;
				CheckTxVar();
			}
		}
		/// <summary>
		/// Gets or sets serial port's rx options.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets serial port's rx options.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[XmlElement]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PropertyOrder(3)]
		public RxSetting Rx
		{
			get
			{
				CheckRxVar();
				return _rxCfg;
			}
			set
			{
				_rxCfg = value;
				CheckRxVar();
			}
		}
		/// <summary>
		/// Gets or sets serial port's HSOuput options.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets serial port's HSOuput options.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[XmlElement]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PropertyOrder(4)]
		public HSOutputSetting HSOuput
		{
			get
			{
				CheckHSOutputVar();
				return _hsOutputCfg;
			}
			set
			{
				_hsOutputCfg = value;
				CheckHSOutputVar();
			}
		}
		/// <summary>
		/// Gets or sets serial port's XOn-Xoff options.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets serial port's XOn-Xoff options.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[XmlElement]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PropertyOrder(5)]
		public XOnXOffSetting XOnXoff
		{
			get
			{
				CheckXOnXoffVar();
				return _xonxoffCfg;
			}
			set
			{
				_xonxoffCfg = value;
				CheckXOnXoffVar();
			}
		}
		/// <summary>
		/// Gets or sets serial port's rx water level options.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets serial port's rx water level options.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[XmlElement]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PropertyOrder(6)]
		public WaterLevelSetting WaterLevel
		{
			get
			{
				CheckWaterLevelVar();
				return _waterLevelCfg;
			}
			set
			{
				_waterLevelCfg = value;
				CheckWaterLevelVar();
			}
		}
		/// <summary>
		/// Gets or sets serial port's timeout options.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets serial port's timeout options.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[XmlElement]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PropertyOrder(7)]
		public TimeoutSetting Timeout
		{
			get
			{
				CheckTimeoutVar();
				return _timeoutCfg;
			}
			set
			{
				_timeoutCfg = value;
				CheckTimeoutVar();
			}
		}
		/// <summary>
		/// Gets or sets serial port's queue options.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets serial port's queue options.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[XmlElement]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PropertyOrder(8)]
		public QueueSetting Queue
		{
			get
			{
				CheckQueueVar();
				return _queueCfg;
			}
			set
			{
				_queueCfg = value;
				CheckQueueVar();
			}
		}
		/// <summary>
		/// Gets or sets serial port's comm line options.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets serial port's comm line options.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[XmlElement]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PropertyOrder(9)]
		public CommLineSetting CommLine
		{
			get
			{
				CheckCommLineVar();
				return _commLineCfg;
			}
			set
			{
				_commLineCfg = value;
				CheckCommLineVar();
			}
		}
		/// <summary>
		/// Gets or sets serial port's comm ping pong options.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets serial port's comm ping pong options.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[XmlElement]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PropertyOrder(10)]
		public CommPingPongSetting CommPingPong
		{
			get
			{
				CheckCommPingPongVar();
				return _commPingPongCfg;
			}
			set
			{
				_commPingPongCfg = value;
				CheckCommPingPongVar();
			}
		}
		/// <summary>
		/// Gets or sets serial port's terminal options.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets serial port's terminal options.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[XmlElement]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PropertyOrder(11)]
		public TerminalSetting Terminal
		{
			get
			{
				CheckTermVar();
				return _termCfg;
			}
			set
			{
				_termCfg = value;
				CheckTermVar();
			}
		}

		#endregion
	}

	#endregion

	#region SerialPortConfig
	
	/// <summary>
	/// The main Serial Port Configuration class.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
	public class SerialPortConfig
	{
		#region Internal Variables
		
		private SerialPortSettings _settings = null;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SerialPortConfig()
			: base()
		{

		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Load settting from target file and assign as current settings.
		/// </summary>
		/// <param name="fileName">Target file name.</param>
		/// <returns>Returns true if load success.</returns>
		public bool LoadFromFile(string fileName)
		{
			SerialPortSettings tmp = XmlManager.LoadFromFile<SerialPortSettings>(fileName);
			if (null == tmp)
			{
				return false; // cannot load
			}
			else
			{
				this.Settings = tmp; // assigned setting from file.
				return true;
			}
		}
		/// <summary>
		/// Save current setting to file.
		/// </summary>
		/// <param name="fileName">Target file name.</param>
		/// <returns>Returns true if save success.</returns>
		public bool SaveToFile(string fileName)
		{
			return XmlManager.SaveToFile<SerialPortSettings>(fileName, this.Settings);
		}

		#endregion

		#region Public Properties
		
		/// <summary>
		/// Gets or sets Serial Port Options.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets Serial Port Options.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[XmlElement]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PropertyOrder(1)]
		public SerialPortSettings Settings
		{
			get
			{
				if (null == _settings)
				{
					lock (this)
					{
						_settings = new SerialPortSettings();
					}
				}
				return _settings;
			}
			set
			{
				_settings = value;
				if (null == _settings)
				{
					lock (this)
					{
						_settings = new SerialPortSettings();
					}
				}
			}
		}

		#endregion
	}

	#endregion

	#endregion

	#region CommBase (abstract)

	/// <summary>
	/// Lowest level Com driver handling all Win32 API calls and processing send and receive in terms of
	/// individual bytes. Used as a base class for higher level drivers.
	/// </summary>
	public abstract class CommBase : IDisposable
	{
		#region Internal variable

		private IntPtr hPort;
		private IntPtr ptrUWO = IntPtr.Zero;
		private Thread rxThread = null;
		private bool online = false;
		private bool auto = false;
		private bool checkSends = true;
		private Exception rxException = null;
		private bool rxExceptionReported = false;
		private int writeCount = 0;
		private ManualResetEvent writeEvent = new ManualResetEvent(false);
		//JH 1.2: Added below to improve robustness of thread start-up.
		private ManualResetEvent startEvent = new ManualResetEvent(false);
		private int stateRTS = 2;
		private int stateDTR = 2;
		private int stateBRK = 2;
		//JH 1.3: Added to support the new congestion detection scheme (following two lines):
		private bool[] empty = new bool[1];
		private bool dataQueued = false;

		#endregion

		#region AltName

		//JH 1.3: Added AltName function, PortStatus enum and IsPortAvailable function.
		/// <summary>
		/// Returns the alternative name of a com port i.e. \\.\COM1 for COM1:
		/// Some systems require this form for double or more digit com port numbers.
		/// </summary>
		/// <param name="s">Name in form COM1 or COM1:</param>
		/// <returns>Name in form \\.\COM1</returns>
		private string AltName(string s)
		{
			string r = s.Trim();
			if (s.EndsWith(":")) s = s.Substring(0, s.Length - 1);
			if (s.StartsWith(@"\")) return s;
			return @"\\.\" + s;
		}

		#endregion

		#region Check Port status

		/// <summary>
		/// Tests the availability of a named comm port.
		/// </summary>
		/// <param name="s">Name of port</param>
		/// <returns>Availability of port</returns>
		public PortStatus IsPortAvailable(string s)
		{
			IntPtr h;

			h = Win32Com.CreateFile(s,
				Win32Com.GENERIC_READ | Win32Com.GENERIC_WRITE, 0,
				IntPtr.Zero,
				Win32Com.OPEN_EXISTING, Win32Com.FILE_FLAG_OVERLAPPED, IntPtr.Zero);
			if (h == (IntPtr)Win32Com.INVALID_HANDLE_VALUE)
			{
				if (Marshal.GetLastWin32Error() == Win32Com.ERROR_ACCESS_DENIED)
				{
					return PortStatus.unavailable;
				}
				else
				{
					//JH 1.3: Automatically try AltName if supplied name fails:
					h = Win32Com.CreateFile(AltName(s),
						Win32Com.GENERIC_READ | Win32Com.GENERIC_WRITE,
						0, IntPtr.Zero,
						Win32Com.OPEN_EXISTING, Win32Com.FILE_FLAG_OVERLAPPED, IntPtr.Zero);
					if (h == (IntPtr)Win32Com.INVALID_HANDLE_VALUE)
					{
						if (Marshal.GetLastWin32Error() == Win32Com.ERROR_ACCESS_DENIED)
						{
							return PortStatus.unavailable;
						}
						else
						{
							return PortStatus.absent;
						}
					}
				}
			}
			Win32Com.CloseHandle(h);
			return PortStatus.available;
		}

		#endregion

		#region Open

		/// <summary>
		/// Opens the com port and configures it with the required settings
		/// </summary>
		/// <returns>false if the port could not be opened</returns>
		public bool Open()
		{
			Win32Com.DCB PortDCB = new Win32Com.DCB();
			Win32Com.COMMTIMEOUTS CommTimeouts = new Win32Com.COMMTIMEOUTS();
			SerialPortSettings cs;
			Win32Com.OVERLAPPED wo = new Win32Com.OVERLAPPED();
			Win32Com.COMMPROP cp;

			if (online) 
				return false;
			cs = GetSettings();

			hPort = Win32Com.CreateFile(cs.Common.Port, 
				Win32Com.GENERIC_READ | Win32Com.GENERIC_WRITE, 0, IntPtr.Zero,
				Win32Com.OPEN_EXISTING, Win32Com.FILE_FLAG_OVERLAPPED, IntPtr.Zero);
			if (hPort == (IntPtr)Win32Com.INVALID_HANDLE_VALUE)
			{
				if (Marshal.GetLastWin32Error() == Win32Com.ERROR_ACCESS_DENIED)
				{
					return false;
				}
				else
				{
					//JH 1.3: Try alternative name form if main one fails:
					hPort = Win32Com.CreateFile(AltName(cs.Common.Port), 
						Win32Com.GENERIC_READ | Win32Com.GENERIC_WRITE, 0, IntPtr.Zero,
						Win32Com.OPEN_EXISTING, Win32Com.FILE_FLAG_OVERLAPPED, IntPtr.Zero);
					if (hPort == (IntPtr)Win32Com.INVALID_HANDLE_VALUE)
					{
						if (Marshal.GetLastWin32Error() == Win32Com.ERROR_ACCESS_DENIED)
						{
							return false;
						}
						else
						{
							throw new CommPortException("Port Open Failure");
						}
					}
				}
			}

			online = true;

			//JH1.1: Changed from 0 to "magic number" to give instant return on ReadFile:
			CommTimeouts.ReadIntervalTimeout = Win32Com.MAXDWORD;
			CommTimeouts.ReadTotalTimeoutConstant = 0;
			CommTimeouts.ReadTotalTimeoutMultiplier = 0;

			//JH1.2: 0 does not seem to mean infinite on non-NT platforms, so default it to 10
			//seconds per byte which should be enough for anyone.
			if (cs.Timeout.SendTimeoutMultiplier == 0)
			{
				if (System.Environment.OSVersion.Platform == System.PlatformID.Win32NT)
				{
					CommTimeouts.WriteTotalTimeoutMultiplier = 0;
				}
				else
				{
					CommTimeouts.WriteTotalTimeoutMultiplier = 10000;
				}
			}
			else
			{
				CommTimeouts.WriteTotalTimeoutMultiplier = cs.Timeout.SendTimeoutMultiplier;
			}
			CommTimeouts.WriteTotalTimeoutConstant = cs.Timeout.SendTimeoutConstant;

			PortDCB.init(((cs.Common.Parity == Parity.odd) || (cs.Common.Parity == Parity.even)), 
				cs.Tx.TxFlowCTS, 
				cs.Tx.TxFlowDSR,
				(int)cs.HSOuput.UseDTR, 
				cs.Rx.RxGateDSR,
				!cs.Tx.TxWhenRxXoff, 
				cs.Tx.TxFlowX, 
				cs.Rx.RxFlowX, 
				(int)cs.HSOuput.UseRTS);
			PortDCB.BaudRate = cs.Common.BaudRate;
			PortDCB.ByteSize = (byte)cs.Common.DataBits;
			PortDCB.Parity = (byte)cs.Common.Parity;
			PortDCB.StopBits = (byte)cs.Common.StopBits;
			PortDCB.XoffChar = (byte)cs.XOnXoff.XoffChar;
			PortDCB.XonChar = (byte)cs.XOnXoff.XonChar;
			if ((cs.Queue.RxQueue != 0) || (cs.Queue.TxQueue != 0))
				if (!Win32Com.SetupComm(hPort,
					(uint)cs.Queue.RxQueue, (uint)cs.Queue.TxQueue)) 
					ThrowException("Bad queue settings");

			//JH 1.2: Defaulting mechanism for handshake thresholds - prevents problems of setting specific
			//defaults which may violate the size of the actually granted queue. If the user specifically sets
			//these values, it's their problem!
			if ((cs.WaterLevel.RxLowWater == 0) || (cs.WaterLevel.RxHighWater == 0))
			{
				if (!Win32Com.GetCommProperties(hPort, out cp)) cp.dwCurrentRxQueue = 0;
				if (cp.dwCurrentRxQueue > 0)
				{
					//If we can determine the queue size, default to 1/10th, 8/10ths, 1/10th.
					//Note that HighWater is measured from top of queue.
					PortDCB.XoffLim = PortDCB.XonLim = (short)((int)cp.dwCurrentRxQueue / 10);
				}
				else
				{
					//If we do not know the queue size, set very low defaults for safety.
					PortDCB.XoffLim = PortDCB.XonLim = 8;
				}
			}
			else
			{
				PortDCB.XoffLim = (short)cs.WaterLevel.RxHighWater;
				PortDCB.XonLim = (short)cs.WaterLevel.RxLowWater;
			}

			if (!Win32Com.SetCommState(hPort, ref PortDCB)) ThrowException("Bad com settings");
			if (!Win32Com.SetCommTimeouts(hPort, ref CommTimeouts)) ThrowException("Bad timeout settings");

			stateBRK = 0;
			if (cs.HSOuput.UseDTR == HSOutput.none) stateDTR = 0;
			if (cs.HSOuput.UseDTR == HSOutput.online) stateDTR = 1;
			if (cs.HSOuput.UseRTS == HSOutput.none) stateRTS = 0;
			if (cs.HSOuput.UseRTS == HSOutput.online) stateRTS = 1;

			checkSends = cs.Common.CheckAllSends;
			wo.Offset = 0;
			wo.OffsetHigh = 0;
			if (checkSends)
				//wo.hEvent = writeEvent.Handle;
				wo.hEvent = writeEvent.SafeWaitHandle.DangerousGetHandle();
			else
				wo.hEvent = IntPtr.Zero;

			ptrUWO = Marshal.AllocHGlobal(Marshal.SizeOf(wo));

			Marshal.StructureToPtr(wo, ptrUWO, true);
			writeCount = 0;
			//JH1.3:
			empty[0] = true;
			dataQueued = false;

			rxException = null;
			rxExceptionReported = false;
			rxThread = new Thread(new ThreadStart(this.ReceiveThread));
			rxThread.Name = "CommBaseRx";
			rxThread.Priority = ThreadPriority.AboveNormal;
			rxThread.IsBackground = true; // Add by Joe.
			rxThread.Start();

			//JH1.2: More robust thread start-up wait.
			startEvent.WaitOne(500, false);

			auto = false;
			if (AfterOpen())
			{
				auto = cs.Common.AutoReopen;
				return true;
			}
			else
			{
				Close();
				return false;
			}

		}

		#endregion

		#region Close

		/// <summary>
		/// Closes the com port.
		/// </summary>
		public void Close()
		{
			if (online)
			{
				auto = false;
				BeforeClose(false);
				InternalClose();
				rxException = null;
			}
		}

		#endregion

		#region Internal Close

		private void InternalClose()
		{
			Win32Com.CancelIo(hPort);
			if (rxThread != null)
			{
				try
				{
					rxThread.Abort();
					//JH 1.3: Improve robustness of Close in case were followed by Open:
					rxThread.Join(100);
				}
				catch (ThreadAbortException)
				{
					System.Threading.Thread.ResetAbort();
				}
				finally
				{
					rxThread = null;
				}
			}
			try
			{
				Win32Com.CloseHandle(hPort);
			}
			catch (Exception ex)
			{
				MethodBase med = MethodBase.GetCurrentMethod();
				ex.Err(med, "Win32 Close Handler Error.");
			}
			
			if (ptrUWO != IntPtr.Zero) 
				Marshal.FreeHGlobal(ptrUWO);

			stateRTS = 2;
			stateDTR = 2;
			stateBRK = 2;
			online = false;
		}

		#endregion

		#region Dispose

		/// <summary>
		/// For IDisposable
		/// </summary>
		public void Dispose() 
		{
			Close();
		}

		#endregion

		#region Destructor

		/// <summary>
		/// Destructor (just in case)
		/// </summary>
		~CommBase() { Close(); }

		#endregion

		#region Online Status

		/// <summary>
		/// True if online.
		/// </summary>
		public bool Online 
		{ 
			get 
			{ 
				if (!online) 
					return false; 
				else return CheckOnline(); 
			} 
		}

		#endregion

		#region Flush Data

		/// <summary>
		/// Block until all bytes in the queue have been transmitted.
		/// </summary>
		public void Flush()
		{
			CheckOnline();
			CheckResult();
		}

		#endregion

		#region Exception Method

		/// <summary>
		/// Use this to throw exceptions in derived classes. Correctly handles threading issues
		/// and closes the port if necessary.
		/// </summary>
		/// <param name="reason">Description of fault</param>
		protected void ThrowException(string reason)
		{
			if (Thread.CurrentThread == rxThread)
			{
				throw new CommPortException(reason);
			}
			else
			{
				if (online)
				{
					BeforeClose(true);
					InternalClose();
				}
				if (rxException == null)
				{
					throw new CommPortException(reason);
				}
				else
				{
					throw new CommPortException(rxException);
				}
			}
		}

		#endregion

		#region Send

		/// <summary>
		/// Queues bytes for transmission. 
		/// </summary>
		/// <param name="tosend">Array of bytes to be sent</param>
		protected void Send(byte[] tosend)
		{
			uint sent = 0;
			CheckOnline();
			CheckResult();
			writeCount = tosend.GetLength(0);
			if (Win32Com.WriteFile(hPort, tosend, (uint)writeCount, out sent, ptrUWO))
			{
				writeCount -= (int)sent;
			}
			else
			{
				if (Marshal.GetLastWin32Error() != Win32Com.ERROR_IO_PENDING) ThrowException("Send failed");
				//JH1.3:
				dataQueued = true;
			}
		}
		/// <summary>
		/// Queues a single byte for transmission.
		/// </summary>
		/// <param name="tosend">Byte to be sent</param>
		protected void Send(byte tosend)
		{
			byte[] b = new byte[1];
			b[0] = tosend;
			Send(b);
		}

		#endregion

		#region CheckResult

		private void CheckResult()
		{
			uint sent = 0;

			//JH 1.3: Fixed a number of problems working with checkSends == false. Byte counting was unreliable because
			//occasionally GetOverlappedResult would return true with a completion having missed one or more previous
			//completions. The test for ERROR_IO_INCOMPLETE was incorrectly for ERROR_IO_PENDING instead.
			if (writeCount > 0)
			{
				if (Win32Com.GetOverlappedResult(hPort, ptrUWO, out sent, checkSends))
				{
					if (checkSends)
					{
						writeCount -= (int)sent;
						if (writeCount != 0) ThrowException("Send Timeout");
						writeCount = 0;
					}
				}
				else
				{
					if (Marshal.GetLastWin32Error() != Win32Com.ERROR_IO_INCOMPLETE) ThrowException("Write Error");
				}
			}
		}

		#endregion

		#region SendImmediate

		/// <summary>
		/// Sends a protocol byte immediately ahead of any queued bytes.
		/// </summary>
		/// <param name="tosend">Byte to send</param>
		protected void SendImmediate(byte tosend)
		{
			CheckOnline();
			if (!Win32Com.TransmitCommChar(hPort, tosend)) ThrowException("Transmission failure");
		}

		#endregion

		#region Sleep

		/// <summary>
		/// Delay processing.
		/// </summary>
		/// <param name="milliseconds">Milliseconds to delay by</param>
		protected void Sleep(int milliseconds)
		{
			Thread.Sleep(milliseconds);
		}

		#endregion

		#region Get ModemStatus

		/// <summary>
		/// Gets the status of the modem control input signals.
		/// </summary>
		/// <returns>Modem status object</returns>
		protected ModemStatus GetModemStatus()
		{
			uint f;

			CheckOnline();
			if (!Win32Com.GetCommModemStatus(hPort, out f)) 
				ThrowException("Unexpected failure");
			return new ModemStatus(f);
		}

		#endregion

		#region Get QueueStatus

		/// <summary>
		/// Get the status of the queues
		/// </summary>
		/// <returns>Queue status object</returns>
		protected QueueStatus GetQueueStatus()
		{
			Win32Com.COMSTAT cs;
			Win32Com.COMMPROP cp;
			uint er;

			CheckOnline();
			if (!Win32Com.ClearCommError(hPort, out er, out cs)) 
				ThrowException("Unexpected failure");
			if (!Win32Com.GetCommProperties(hPort, out cp)) 
				ThrowException("Unexpected failure");
			return new QueueStatus(cs.Flags, cs.cbInQue, cs.cbOutQue, cp.dwCurrentRxQueue, cp.dwCurrentTxQueue);
		}

		#endregion

		#region IsCongested

		// JH 1.3. Added for this version.
		/// <summary>
		/// Test if the line is congested (data being queued for send faster than it is being dequeued)
		/// This detects if baud rate is too slow or if handshaking is not allowing enough transmission
		/// time. It should be called at reasonably long fixed intervals. If data has been sent during
		/// the interval, congestion is reported if the queue was never empty during the interval.
		/// </summary>
		/// <returns>True if congested</returns>
		protected bool IsCongested()
		{
			bool e;
			if (!dataQueued) return false;
			lock (empty) { e = empty[0]; empty[0] = false; }
			dataQueued = false;
			return !e;
		}

		#endregion

		#region RTS

		/// <summary>
		/// Set the state of the RTS modem control output
		/// </summary>
		protected bool RTS
		{
			set
			{
				if (stateRTS > 1) return;
				CheckOnline();
				if (value)
				{
					if (Win32Com.EscapeCommFunction(hPort, Win32Com.SETRTS))
						stateRTS = 1;
					else
						ThrowException("Unexpected Failure");
				}
				else
				{
					if (Win32Com.EscapeCommFunction(hPort, Win32Com.CLRRTS))
						//JH 1.3: Was 1, should be 0:
						stateRTS = 0;
					else
						ThrowException("Unexpected Failure");
				}
			}
			get
			{
				return (stateRTS == 1);
			}
		}

		#endregion

		#region DTR

		/// <summary>
		/// The state of the DTR modem control output
		/// </summary>
		protected bool DTR
		{
			set
			{
				if (stateDTR > 1) return;
				CheckOnline();
				if (value)
				{
					if (Win32Com.EscapeCommFunction(hPort, Win32Com.SETDTR))
						stateDTR = 1;
					else
						ThrowException("Unexpected Failure");
				}
				else
				{
					if (Win32Com.EscapeCommFunction(hPort, Win32Com.CLRDTR))
						stateDTR = 0;
					else
						ThrowException("Unexpected Failure");
				}
			}
			get
			{
				return (stateDTR == 1);
			}
		}

		#endregion

		#region Check Available

		/// <summary>
		/// True if the RTS pin is controllable via the RTS property
		/// </summary>
		protected bool RTSavailable { get { return (stateRTS < 2); } }

		/// <summary>
		/// True if the DTR pin is controllable via the DTR property
		/// </summary>
		protected bool DTRavailable { get { return (stateDTR < 2); } }

		#endregion

		#region Break

		/// <summary>
		/// Assert or remove a break condition from the transmission line
		/// </summary>
		protected bool Break
		{
			set
			{
				if (stateBRK > 1) return;
				CheckOnline();
				if (value)
				{
					if (Win32Com.EscapeCommFunction(hPort, Win32Com.SETBREAK))
						stateBRK = 0;
					else
						ThrowException("Unexpected Failure");
				}
				else
				{
					if (Win32Com.EscapeCommFunction(hPort, Win32Com.CLRBREAK))
						stateBRK = 0;
					else
						ThrowException("Unexpected Failure");
				}
			}
			get
			{
				return (stateBRK == 1);
			}
		}

		#endregion

		#region GetSettings
		
		/// <summary>
		/// Gets new settings instance. (NB this is called during Open method)
		/// </summary>
		/// <returns>Returns SerialPortSettings instance.</returns>
		protected virtual SerialPortSettings GetSettings() { return new SerialPortSettings(); }

		#endregion

		#region Virtual Method

		/// <summary>
		/// Override this to provide processing after the port is openned (i.e. to configure remote
		/// device or just check presence).
		/// </summary>
		/// <returns>false to close the port again</returns>
		protected virtual bool AfterOpen() { return true; }
		/// <summary>
		/// Override this to provide processing prior to port closure.
		/// </summary>
		/// <param name="error">True if closing due to an error</param>
		protected virtual void BeforeClose(bool error) { }

		/// <summary>
		/// Override this to process received bytes.
		/// </summary>
		/// <param name="ch">The byte that was received</param>
		protected virtual void DoRxChar(byte ch) { }
		/// <summary>
		/// Override this to take action when transmission is complete (i.e. all bytes have actually
		/// been sent, not just queued).
		/// </summary>
		protected virtual void DoTxDone() { }
		/// <summary>
		/// Override this to take action when a break condition is detected on the input line.
		/// </summary>
		protected virtual void DoBreak() { }
		//JH 1.3: Deleted OnRing() which was never called: use DoStatusChange instead (Thanks Jim Foster)
		/// <summary>
		/// Override this to take action when one or more modem status inputs change state
		/// </summary>
		/// <param name="mask">The status inputs that have changed state</param>
		/// <param name="state">The state of the status inputs</param>
		protected virtual void DoStatusChange(ModemStatus mask, ModemStatus state) { }
		/// <summary>
		/// Override this to take action when the reception thread closes due to an exception being thrown.
		/// </summary>
		/// <param name="e">The exception which was thrown</param>
		protected virtual void DoRxException(Exception e) { }

		#endregion

		#region Received Thread

		private void ReceiveThread()
		{
			byte[] buf = new Byte[1];
			uint gotbytes;
			bool starting;

			starting = true;
			AutoResetEvent sg = new AutoResetEvent(false);
			Win32Com.OVERLAPPED ov = new Win32Com.OVERLAPPED();

			IntPtr unmanagedOv;
			IntPtr uMask;
			uint eventMask = 0;
			unmanagedOv = Marshal.AllocHGlobal(Marshal.SizeOf(ov));
			uMask = Marshal.AllocHGlobal(Marshal.SizeOf(eventMask));

			ov.Offset = 0; ov.OffsetHigh = 0;

			ov.hEvent = sg.SafeWaitHandle.DangerousGetHandle();

			Marshal.StructureToPtr(ov, unmanagedOv, true);

			try
			{
				//while (true)
				while (null != rxThread && 
					!ApplicationManager.Instance.IsExit)
				{
					if (!Win32Com.SetCommMask(hPort, Win32Com.EV_RXCHAR | Win32Com.EV_TXEMPTY | Win32Com.EV_CTS | Win32Com.EV_DSR
						| Win32Com.EV_BREAK | Win32Com.EV_RLSD | Win32Com.EV_RING | Win32Com.EV_ERR))
					{
						throw new CommPortException("IO Error [001]");
					}
					Marshal.WriteInt32(uMask, 0);
					//JH 1.2: Tells the main thread that this thread is ready for action.
					if (starting) { startEvent.Set(); starting = false; }
					if (!Win32Com.WaitCommEvent(hPort, uMask, unmanagedOv))
					{
						if (Marshal.GetLastWin32Error() == Win32Com.ERROR_IO_PENDING)
						{
							// Original
							//sg.WaitOne();

							// Add By Joe.
							if (null != rxThread &&
								!ApplicationManager.Instance.IsExit)
							{
								sg.WaitOne();
							}
						}
						else
						{
							throw new CommPortException("IO Error [002]");
						}
					}
					eventMask = (uint)Marshal.ReadInt32(uMask);
					if ((eventMask & Win32Com.EV_ERR) != 0)
					{
						UInt32 errs;
						if (Win32Com.ClearCommError(hPort, out errs, IntPtr.Zero))
						{
							//JH 1.2: BREAK condition has an error flag and and an event flag. Not sure if both
							//are always raised, so if CE_BREAK is only error flag ignore it and set the EV_BREAK
							//flag for normal handling. Also made more robust by handling case were no recognised
							//error was present in the flags. (Thanks to Fred Pittroff for finding this problem!)
							int ec = 0;
							StringBuilder s = new StringBuilder("UART Error: ", 40);
							if ((errs & Win32Com.CE_FRAME) != 0) { s = s.Append("Framing,"); ec++; }
							if ((errs & Win32Com.CE_IOE) != 0) { s = s.Append("IO,"); ec++; }
							if ((errs & Win32Com.CE_OVERRUN) != 0) { s = s.Append("Overrun,"); ec++; }
							if ((errs & Win32Com.CE_RXOVER) != 0) { s = s.Append("Receive Cverflow,"); ec++; }
							if ((errs & Win32Com.CE_RXPARITY) != 0) { s = s.Append("Parity,"); ec++; }
							if ((errs & Win32Com.CE_TXFULL) != 0) { s = s.Append("Transmit Overflow,"); ec++; }
							if (ec > 0)
							{
								s.Length = s.Length - 1;
								throw new CommPortException(s.ToString());
							}
							else
							{
								if (errs == Win32Com.CE_BREAK)
								{
									eventMask |= Win32Com.EV_BREAK;
								}
								else
								{
									throw new CommPortException("IO Error [003]");
								}
							}
						}
						else
						{
							throw new CommPortException("IO Error [003]");
						}
					}
					if ((eventMask & Win32Com.EV_RXCHAR) != 0)
					{
						do
						{
							gotbytes = 0;
							if (!Win32Com.ReadFile(hPort, buf, 1, out gotbytes, unmanagedOv))
							{
								//JH 1.1: Removed ERROR_IO_PENDING handling as comm timeouts have now
								//been set so ReadFile returns immediately. This avoids use of CancelIo
								//which was causing loss of data. Thanks to Daniel Moth for suggesting this
								//might be a problem, and to many others for reporting that it was!

								int x = Marshal.GetLastWin32Error();

								throw new CommPortException("IO Error [004]");
							}
							if (gotbytes == 1) DoRxChar(buf[0]);
						} while (gotbytes > 0);
					}
					if ((eventMask & Win32Com.EV_TXEMPTY) != 0)
					{
						//JH1.3:
						lock (empty) empty[0] = true;
						DoTxDone();
					}
					if ((eventMask & Win32Com.EV_BREAK) != 0) DoBreak();

					uint i = 0;
					if ((eventMask & Win32Com.EV_CTS) != 0) i |= Win32Com.MS_CTS_ON;
					if ((eventMask & Win32Com.EV_DSR) != 0) i |= Win32Com.MS_DSR_ON;
					if ((eventMask & Win32Com.EV_RLSD) != 0) i |= Win32Com.MS_RLSD_ON;
					if ((eventMask & Win32Com.EV_RING) != 0) i |= Win32Com.MS_RING_ON;
					if (i != 0)
					{
						uint f;
						if (!Win32Com.GetCommModemStatus(hPort, out f)) throw new CommPortException("IO Error [005]");
						DoStatusChange(new ModemStatus(i), new ModemStatus(f));
					}
				}
			}
			catch (Exception e)
			{
				//JH 1.3: Added for shutdown robustness (Thanks to Fred Pittroff, Mark Behner and Kevin Williamson!), .
				Win32Com.CancelIo(hPort);
				if (uMask != IntPtr.Zero) Marshal.FreeHGlobal(uMask);
				if (unmanagedOv != IntPtr.Zero) Marshal.FreeHGlobal(unmanagedOv);

				if (!(e is ThreadAbortException))
				{
					rxException = e;
					DoRxException(e);
				}
				else
				{
					System.Threading.Thread.ResetAbort();
				}
			}
		}

		#endregion

		#region CheckOnline

		private bool CheckOnline()
		{
			if ((rxException != null) && (!rxExceptionReported))
			{
				rxExceptionReported = true;
				ThrowException("rx");
			}
			if (online)
			{
				//JH 1.1: Avoid use of GetHandleInformation for W98 compatability.
				if (hPort != (System.IntPtr)Win32Com.INVALID_HANDLE_VALUE) return true;
				ThrowException("Offline");
				return false;
			}
			else
			{
				if (auto)
				{
					if (Open()) return true;
				}
				ThrowException("Offline");
				return false;
			}
		}

		#endregion
	}

	#endregion

	#region CommLine (abstract)

	/// <summary>
	/// Overlays CommBase to provide line or packet oriented communications to derived classes. Strings
	/// are sent and received and the Transact method is added which transmits a string and then blocks until
	/// a reply string has been received (subject to a timeout).
	/// </summary>
	public abstract class CommLine : CommBase
	{
		#region Internal Variable

		private byte[] RxBuffer;
		private uint RxBufferP = 0;
		private ASCII RxTerm;
		private ASCII[] TxTerm;
		private ASCII[] RxFilter;
		private string RxString = "";
		private ManualResetEvent TransFlag = new ManualResetEvent(true);
		private uint TransTimeout;

		#endregion

		#region Send

		/// <summary>
		/// Queue the ASCII representation of a string and then the set terminator bytes for sending.
		/// </summary>
		/// <param name="toSend">String to be sent.</param>
		protected void Send(string toSend)
		{
			//JH 1.1: Use static encoder for efficiency. Thanks to Prof. Dr. Peter Jesorsky!
			uint l = (uint)Encoding.ASCII.GetByteCount(toSend);
			if (TxTerm != null) l += (uint)TxTerm.GetLength(0);
			byte[] b = new byte[l];
			byte[] s = Encoding.ASCII.GetBytes(toSend);
			int i;
			for (i = 0; (i <= s.GetUpperBound(0)); i++) b[i] = s[i];
			if (TxTerm != null) for (int j = 0; (j <= TxTerm.GetUpperBound(0)); j++, i++) b[i] = (byte)TxTerm[j];
			Send(b);
		}

		#endregion

		#region Transact (Transmit)

		/// <summary>
		/// Transmits the ASCII representation of a string followed by the set terminator bytes and then
		/// awaits a response string.
		/// </summary>
		/// <param name="toSend">The string to be sent.</param>
		/// <returns>The response string.</returns>
		protected string Transact(string toSend)
		{
			Send(toSend);
			TransFlag.Reset();
			if (!TransFlag.WaitOne((int)TransTimeout, false)) ThrowException("Timeout");
			string s;
			lock (RxString) { s = RxString; }
			return s;
		}

		#endregion

		#region Setup

		/// <summary>
		/// If a derived class overrides ComSettings(), it must call this prior to returning the settings to
		/// the base class.
		/// </summary>
		/// <param name="s">Class containing the appropriate settings.</param>
		protected void Setup(SerialPortSettings s)
		{
			RxBuffer = new byte[s.CommLine.RxStringBufferSize];
			RxTerm = s.CommLine.RxTerminator;
			RxFilter = s.CommLine.RxFilter;
			TransTimeout = s.CommLine.TransactTimeout;
			TxTerm = s.CommLine.TxTerminator;
		}

		#endregion

		#region DoRxLine

		/// <summary>
		/// Override this to process unsolicited input lines (not a result of Transact).
		/// </summary>
		/// <param name="s">String containing the received ASCII text.</param>
		protected virtual void DoRxLine(string s) { }

		#endregion

		#region DoRxChar

		/// <summary>
		/// DoRxChar
		/// </summary>
		/// <param name="ch"></param>
		protected override void DoRxChar(byte ch)
		{
			ASCII ca = (ASCII)ch;
			if ((ca == RxTerm) || (RxBufferP > RxBuffer.GetUpperBound(0)))
			{
				//JH 1.1: Use static encoder for efficiency. Thanks to Prof. Dr. Peter Jesorsky!
				lock (RxString) 
				{ 
					RxString = Encoding.ASCII.GetString(RxBuffer, 0, (int)RxBufferP); 
				}
				RxBufferP = 0;
				if (TransFlag.WaitOne(0, false))
				{
					DoRxLine(RxString);
				}
				else
				{
					TransFlag.Set();
				}
			}
			else
			{
				bool wr = true;
				if (RxFilter != null)
				{
					for (int i = 0; i <= RxFilter.GetUpperBound(0); i++)
					{
						if (RxFilter[i] == ca) wr = false;
					}
				}
				if (wr)
				{
					RxBuffer[RxBufferP] = ch;
					RxBufferP++;
				}
			}
		}

		#endregion
	}

	#endregion

	#region ComPingPong (abstract)

	/// <summary>
	/// Overlays CommBase to provide byte-level ping-pong communications were each transmitted byte
	/// illicits a single byte response which must be absorbed before sending the next byte.
	/// There is a default response timeout of 500ms after which a Timeout exception will be raised.
	/// This timeout can be changed by changing the transactTimeout parameter in the settings object.
	/// Use the Transact method for all communications.
	/// </summary>
	public abstract class CommPingPong : CommBase
	{
		#region Internal Variable

		private byte[] RxByte;
		private ManualResetEvent TransFlag = new ManualResetEvent(true);
		private uint TransTimeout;

		#endregion

		#region Transact

		/// <summary>
		/// Transmits a byte and waits for and returns the response byte.
		/// </summary>
		/// <param name="toSend">The byte to be sent.</param>
		/// <returns>The response byte.</returns>
		protected byte Transact(byte toSend)
		{
			if (RxByte == null) RxByte = new byte[1];
			Send(toSend);
			TransFlag.Reset();
			if (!TransFlag.WaitOne((int)TransTimeout, false)) 
				ThrowException("Timeout");
			byte s;
			lock (RxByte) { s = RxByte[0]; }
			return s;
		}

		#endregion

		#region Setup

		/// <summary>
		/// If a derived class overrides ComSettings(), it must call this prior to returning the settings to
		/// the base class.
		/// </summary>
		/// <param name="s">Class containing the appropriate settings.</param>
		protected void Setup(SerialPortSettings s)
		{
			TransTimeout = (uint)s.CommPingPong.TransactTimeout;
		}

		#endregion

		#region DoRxChar override

		/// <summary>
		/// DoRxChar
		/// </summary>
		/// <param name="ch"></param>
		protected override void DoRxChar(byte ch)
		{
			lock (RxByte) { RxByte[0] = ch; }
			if (!TransFlag.WaitOne(0, false))
			{
				TransFlag.Set();
			}
		}

		#endregion
	}

	#endregion

	#region Base Terminal

	/// <summary>
	/// Base Terminal
	/// </summary>
	public class BaseTerminal : CommBase
	{
		#region Internal Variable

		private int lineCount = 0;
		private SerialPortSettings _settings = new SerialPortSettings();

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public BaseTerminal()
			: base()
		{
			this.Immediate = false;
		}
		/// <summary>
		/// Destructor
		/// </summary>
		~BaseTerminal()
		{
			// Free Memory
			NGC.FreeGC(this);
		}

		#endregion
		
		#region Protected Methods

		#region GetSettings

		/// <summary>
		/// Gets new settings instance. (NB this is called during Open method)
		/// </summary>
		/// <returns>Returns SerialPortSettings instance.</returns>
		protected override SerialPortSettings GetSettings() { return _settings; }

		#endregion

		#region DoRxChar

		/// <summary>
		/// DoRxChar
		/// </summary>
		/// <param name="c"></param>
		protected override void DoRxChar(byte c)
		{
			if (this.Settings == null) return;
			string s;
			bool nl = false;
			ASCII v = (ASCII)c;
			if (this.Settings.Terminal.CharsInLine > 0)
			{
				nl = (++lineCount >= Settings.Terminal.CharsInLine);
			}
			if (this.Settings.Terminal.BreakLineOnChar)
			{
				if (v == Settings.Terminal.LineBreakChar)
					nl = true;
			}
			if (nl) lineCount = 0;
			if (this.Settings.Terminal.ShowAsHex)
			{
				s = c.ToString("X2");
				if (!nl) s += " ";
			}
			else
			{
				if ((c < 0x20) || (c > 0x7E))
				{
					s = "<" + v.ToString() + ">";
				}
				else
				{
					s = new string((char)c, 1);
				}
			}
			if (OnRxChar != null) OnRxChar(s, nl);
			//frm.ShowChar(s, nl);
		}

		#endregion

		#region DoBreak

		/// <summary>
		/// DoBreak
		/// </summary>
		protected override void DoBreak()
		{
			if (OnBreak != null) OnBreak(this, System.EventArgs.Empty);
			//frm.ShowMsg(">>>> BREAK");
			if (OnCommMessage != null) OnCommMessage(">>>> BREAK");
		}

		#endregion

		#region AfterOpen

		/// <summary>
		/// AfterOpen
		/// </summary>
		/// <returns></returns>
		protected override bool AfterOpen()
		{
			//frm.OnOpen();
			if (OnPortOpenning != null) OnPortOpenning(this, System.EventArgs.Empty);
			ModemStatus m = GetModemStatus();
			//frm.SetIndics(m.cts, m.dsr, m.rlsd, m.ring);
			if (OnStatusChanged != null) OnStatusChanged(m.cts, m.dsr, m.rlsd, m.ring);
			return true;
		}

		#endregion

		#region BeforeClose

		/// <summary>
		/// BeforeClose
		/// </summary>
		/// <param name="e"></param>
		protected override void BeforeClose(bool e)
		{
			if (this.Settings == null) return;
			if ((this.Settings.Common.AutoReopen) && (e))
			{
				//frm.OnOpen();
				if (OnPortOpenning != null) OnPortOpenning(this, System.EventArgs.Empty);
			}
			else
			{
				//frm.OnClose();
				if (OnPortClosing != null) OnPortClosing(this, System.EventArgs.Empty);
				//frm.ShowMsg(">>>> OFFLINE");
				if (OnCommMessage != null) OnCommMessage(">>>> OFFLINE");
			}
		}

		#endregion

		#region DoStatusChange

		/// <summary>
		/// DoStatusChange
		/// </summary>
		/// <param name="c"></param>
		/// <param name="v"></param>
		protected override void DoStatusChange(ModemStatus c, ModemStatus v)
		{
			//frm.SetIndics(v.cts, v.dsr, v.rlsd, v.ring);
			if (OnStatusChanged != null) OnStatusChanged(v.cts, v.dsr, v.rlsd, v.ring);
		}

		#endregion

		#endregion

		#region Public Methods

		#region SendChar

		/// <summary>
		/// SendChar
		/// </summary>
		/// <param name="c"></param>
		public void SendChar(byte c)
		{
			try
			{
				if (Immediate)
					SendImmediate(c);
				else
					Send(c);
			}
			catch (CommPortException e)
			{
				//frm.ShowException(e);
				//throw(e);
				if (OnCommException != null) OnCommException(e);
			}
		}

		#endregion

		#region SendFile (stream)

		/// <summary>
		/// SendFile (stream)
		/// </summary>
		/// <param name="fs"></param>
		public void SendFile(FileStream fs)
		{
			byte[] buffer = new byte[fs.Length];
			fs.Read(buffer, 0, (int)fs.Length);
			Send(buffer);
		}

		#endregion

		#region SendCtrl

		/// <summary>
		/// Send Ctrl
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public bool SendCtrl(string s)
		{
			ASCII a = 0;
			try
			{
				a = (ASCII)ASCII.Parse(a.GetType(), s, true);
			}
			catch
			{
				return false;
			}
			SendChar((byte)a);
			return true;
		}

		#endregion

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets is Immediate mode.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets is Immediate mode.")]
		public bool Immediate { get; set; }
		/// <summary>
		/// Gets or sets terminal (serial port) setting.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets terminal (serial port) setting.")]
		public SerialPortSettings Settings
		{
			get
			{
				if (null == _settings)
				{
					_settings = this.GetSettings(); // get default.
				}
				return _settings;
			}
			set
			{
				_settings = value;
				if (null == _settings)
				{
					_settings = this.GetSettings(); // get default.
				}
			}
		}
		/// <summary>
		/// Get Queue Status
		/// </summary>
		/// <returns></returns>
		[Category("SerialPort")]
		[Description("Get Queue Status.")]
		public new QueueStatus GetQueueStatus()
		{
			return base.GetQueueStatus();
		}
		/// <summary>
		/// Gets Is Congested.
		/// </summary>
		/// <returns></returns>
		[Category("SerialPort")]
		[Description("Gets Is Congested.")]
		public new bool IsCongested()
		{
			return base.IsCongested();
		}

		#endregion

		#region Public Events

		/// <summary>
		/// OnPortOpenning event
		/// </summary>
		public event System.EventHandler OnPortOpenning;
		/// <summary>
		/// OnPortClosing event
		/// </summary>
		public event System.EventHandler OnPortClosing;
		/// <summary>
		/// OnBreak event
		/// </summary>
		public event System.EventHandler OnBreak;
		/// <summary>
		/// OnRxChar event
		/// </summary>
		public event RxCharEventHandler OnRxChar;
		/// <summary>
		/// OnStatusChanged event
		/// </summary>
		public event CommStatusChangedEventHandler OnStatusChanged;
		/// <summary>
		/// OnCommMessage event
		/// </summary>
		public event CommMessageEventHandler OnCommMessage;
		/// <summary>
		/// OnCommException event
		/// </summary>
		public event CommExceptionEventHandler OnCommException;

		#endregion
	}

	#endregion

	#region Comm Terminal

	/// <summary>
	/// Comm Terminal
	/// </summary>
	public class CommTerminal : CommBase
	{
		#region Internal Variables

		private int lineCount = 0;
		private TermForm frm = new TermForm();
		private SerialPortSettings _settings = new SerialPortSettings();

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public CommTerminal()
			: base()
		{
			this.Immediate = false;
			frm.Terminal = this;
		}
		/// <summary>
		/// Destructor.
		/// </summary>
		~CommTerminal()
		{
			if (null != frm && !frm.IsDisposed && !frm.Disposing)
			{
				try { frm.Close(); }
				catch { }
			}
			NGC.FreeGC(frm);
			frm = null;
		}

		#endregion

		#region Protected Methods

		#region GetSettings

		/// <summary>
		/// Gets new settings instance. (NB this is called during Open method)
		/// </summary>
		/// <returns>Returns SerialPortSettings instance.</returns>
		protected override SerialPortSettings GetSettings() { return _settings; }

		#endregion

		#region DoRxChar

		/// <summary>
		/// DoRxChar
		/// </summary>
		/// <param name="c"></param>
		protected override void DoRxChar(byte c)
		{
			if (this.Settings == null) return;
			string s;
			bool nl = false;
			ASCII v = (ASCII)c;
			if (this.Settings.Terminal.CharsInLine > 0)
			{
				nl = (++lineCount >= this.Settings.Terminal.CharsInLine);
			}
			if (this.Settings.Terminal.BreakLineOnChar)
			{
				if (v == this.Settings.Terminal.LineBreakChar) nl = true;
			}
			if (nl) lineCount = 0;
			if (this.Settings.Terminal.ShowAsHex)
			{
				s = c.ToString("X2");
				if (!nl) s += " ";
			}
			else
			{
				if ((c < 0x20) || (c > 0x7E))
				{
					s = "<" + v.ToString() + ">";
				}
				else
				{
					s = new string((char)c, 1);
				}
			}
			frm.ShowChar(s, nl);
		}

		#endregion

		#region DoBreak

		/// <summary>
		/// DoBreak
		/// </summary>
		protected override void DoBreak()
		{
			frm.ShowMsg(">>>> BREAK");
		}

		#endregion

		#region AfterOpen

		/// <summary>
		/// AfterOpen
		/// </summary>
		/// <returns></returns>
		protected override bool AfterOpen()
		{
			frm.OnOpen();

			ModemStatus m = GetModemStatus();
			frm.SetIndics(m.cts, m.dsr, m.rlsd, m.ring);

			return true;
		}

		#endregion

		#region BeforeClose

		/// <summary>
		/// BeforeClose
		/// </summary>
		/// <param name="e"></param>
		protected override void BeforeClose(bool e)
		{
			if (this.Settings.Terminal == null) return;
			if ((this.Settings.Common.AutoReopen) && (e))
			{
				frm.OnOpen();
			}
			else
			{
				frm.OnClose();
				frm.ShowMsg(">>>> OFFLINE");
			}
		}

		#endregion

		#region DoStatusChange

		/// <summary>
		/// DoStatusChange
		/// </summary>
		/// <param name="c"></param>
		/// <param name="v"></param>
		protected override void DoStatusChange(ModemStatus c, ModemStatus v)
		{
			frm.SetIndics(v.cts, v.dsr, v.rlsd, v.ring);
		}

		#endregion

		#endregion

		#region Public Methods

		#region SendChar

		/// <summary>
		/// SendChar
		/// </summary>
		/// <param name="c"></param>
		public void SendChar(byte c)
		{
			try
			{
				if (Immediate)
					SendImmediate(c);
				else
					Send(c);
			}
			catch (CommPortException e)
			{
				frm.ShowException(e);
			}
		}

		#endregion

		#region SendFile (stream)

		/// <summary>
		/// SendFile
		/// </summary>
		/// <param name="fs"></param>
		public void SendFile(FileStream fs)
		{
			byte[] buffer = new byte[fs.Length];
			fs.Read(buffer, 0, (int)fs.Length);
			Send(buffer);
		}

		#endregion

		#region SendCtrl

		/// <summary>
		/// SendCtrl
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public bool SendCtrl(string s)
		{
			ASCII a = 0;
			try
			{
				a = (ASCII)ASCII.Parse(a.GetType(), s, true);
			}
			catch
			{
				return false;
			}
			SendChar((byte)a);
			return true;
		}

		#endregion

		#region Terminal (Form) Methods

		/// <summary>
		/// Get Terminal (Form)
		/// </summary>
		[Browsable(false)]
		public TermForm Terminal { get { return frm; } }
		/// <summary>
		/// Show Terminal.
		/// </summary>
		/// <param name="dialog"></param>
		public void ShowTerminal(bool dialog)
		{
			if (null == frm || frm.IsDisposed || frm.Disposing)
			{
				frm = null; // Release reference.
				NGC.FreeGC();

				frm = new TermForm();
			}
			if (!dialog)
				frm.Show();
			else
				frm.ShowDialog();
		}
		/// <summary>
		/// Show Settings.
		/// </summary>
		public void ShowSettings()
		{
			SettingsForm f = new SettingsForm();
			f.Terminal = this;
			f.ShowDialog();
		}
		/// <summary>
		/// setOPTicks
		/// </summary>
		/// <param name="chk"></param>
		public void setOPTicks(System.Windows.Forms.CheckBox chk)
		{
			switch (int.Parse(chk.Tag.ToString()))
			{
				case 0:
					chk.Enabled = base.RTSavailable;
					chk.Checked = base.RTS;
					break;
				case 1:
					chk.Enabled = base.DTRavailable;
					chk.Checked = base.DTR;
					break;
				case 2:
					chk.Enabled = true;
					chk.Checked = base.Break;
					break;
			}
		}
		/// <summary>
		/// OPClick
		/// </summary>
		/// <param name="chk"></param>
		public void OPClick(System.Windows.Forms.CheckBox chk)
		{
			try
			{
				switch (int.Parse(chk.Tag.ToString()))
				{
					case 0: base.RTS = chk.Checked; break;
					case 1: base.DTR = chk.Checked; break;
					case 2: base.Break = chk.Checked; break;
				}
			}
			catch (CommPortException e)
			{
				frm.ShowException(e);
			}
		}

		#endregion

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets is Immediate mode.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets is Immediate mode.")]
		public bool Immediate { get; set; }
		/// <summary>
		/// Gets or sets terminal (serial port) setting.
		/// </summary>
		[Category("SerialPort")]
		[Description("Gets or sets terminal (serial port) setting.")]
		public SerialPortSettings Settings
		{
			get
			{
				if (null == _settings)
				{
					_settings = this.GetSettings(); // get default.
				}
				return _settings;
			}
			set
			{
				_settings = value;
				if (null == _settings)
				{
					_settings = this.GetSettings(); // get default.
				}
			}
		}
		/// <summary>
		/// Get Queue Status
		/// </summary>
		/// <returns></returns>
		[Category("SerialPort")]
		[Description("Get Queue Status.")]
		public new QueueStatus GetQueueStatus()
		{
			return base.GetQueueStatus();
		}
		/// <summary>
		/// Gets Is Congested.
		/// </summary>
		/// <returns></returns>
		[Category("SerialPort")]
		[Description("Gets Is Congested.")]
		public new bool IsCongested()
		{
			return base.IsCongested();
		}

		#endregion
	}

	#endregion
}
