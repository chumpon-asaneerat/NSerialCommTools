#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-17
=================
- WinSys class updated
  - Changes log code (used MethodBase).

======================================================================================================================
Update 2013-08-18
=================
- WinSys class ported from NLib40x2 to NLib40x5.
  - Change log code to new Debug framework.
  - Required to re test.
  - Required to add method to Capture WPF control.

======================================================================================================================
Update 2012-12-27
=================
- WinSys class ported from GFA38v3 to NLib40x3.
  - Change namespace from SysLib to NLib.
  - Change log code to new DebugManager.

======================================================================================================================
Update 2010-01-24
=================
- WinUtils class ported and related classes from GFA Library GFA37 tor GFA38v3.
  (Standard/Pro version).
  - Changed all exception handling code to used new ExceptionManager class instread of 
	LogManager.

======================================================================================================================
Update 2008-11-14
=================
- WinUtils -> Fixed bug
  - Fixed bug when used SystemKeyHook and SwitchForm method.

======================================================================================================================
Update 2008-09-04
=================
- WinUtils -> Add New Features
  - Change Local Date/Time functions.

======================================================================================================================
Update 2008-07-02
=================
- WinUtils -> Add New Features
  - Create Short cut method added.

======================================================================================================================
Update 2008-06-10
=================
- WinUtils
  - Fixed method that cannot used with window vista.
	Note that On window vista when Hide Start button we need to hide System Tray because
	on window vista the system tray background (in vista) has fake button image embeded 
	on the systen tray bar.
  - Add new method To Show/Hide Quick Launch bar.

======================================================================================================================
Update 2008-01-02
=================
- WinUtils Port Utilities WinSys from V2.27

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace NLib.Utils
{
	#region Window System

	/// <summary>
	/// The Window Utilities class.
	/// </summary>
	public sealed class WinUtils
	{
		#region LowLevelKeyboardHook

		/// <summary>
		/// LowLevelKeyboardHook
		/// </summary>
		class LowLevelKeyboardHook : IDisposable
		{
			#region API

			private delegate int LowLevelKeyboardProcDelegate(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);
			[DllImport("user32.dll", EntryPoint = "SetWindowsHookExA", CharSet = CharSet.Ansi)]
			private static extern int SetWindowsHookEx(int idHook, LowLevelKeyboardProcDelegate lpfn, int hMod, int dwThreadId);
			[DllImport("user32.dll", EntryPoint = "UnhookWindowsHookEx", CharSet = CharSet.Ansi)]
			private static extern int UnHookWindowsEx(int hHook);
			[DllImport("user32.dll", EntryPoint = "CallNextHookEx", CharSet = CharSet.Ansi)]
			private static extern int CallNextHookEx(int hHook, int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);
			const int WH_KEYBOARD_LL = 13;
			public struct KBDLLHOOKSTRUCT
			{
				public int vkCode;
				int scanCode;
				public int flags;
				int time;
				int dwExtraInfo;
			}

			#endregion

			#region Vars

			private int intLLKey = 0;

			// variable to keep unmanaged reference function call.
			private LowLevelKeyboardProcDelegate llKeyFunc = null;

			#endregion

			#region Constructor and Destructor

			/// <summary>
			/// Constructor
			/// </summary>
			public LowLevelKeyboardHook()
			{
				// create reference.
				llKeyFunc = new LowLevelKeyboardProcDelegate(LowLevelKeyboardProc);
			}
			/// <summary>
			/// Destructor
			/// </summary>
			~LowLevelKeyboardHook()
			{
				Unhook();
				llKeyFunc = null;
			}

			#endregion

			#region Handler for Hook

			private int LowLevelKeyboardProc(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam)
			{
				bool blnEat = false;
				switch (wParam)
				{
					//case (256 | 257 | 260 | 261):
					case 256:
					case 257:
					case 260:
					case 261:
						//Alt+Tab, Alt+Esc, Ctrl+Esc, Windows Key
						if (((lParam.vkCode == 9) && (lParam.flags == 32)) || ((lParam.vkCode == 27) && (lParam.flags == 32)) || ((lParam.vkCode == 27) && (lParam.flags == 0)) || ((lParam.vkCode == 91) && (lParam.flags == 1)) || ((lParam.vkCode == 92) && (lParam.flags == 1)))
						{
							blnEat = true;
						}
						break;
				}
				if (blnEat)
					return 1;
				else return CallNextHookEx(0, nCode, wParam, ref lParam);

			}

			#endregion

			#region Public Methods

			/// <summary>
			/// Hook
			/// </summary>
			public void Hook()
			{
				if (intLLKey != 0) return;
				intLLKey = SetWindowsHookEx(WH_KEYBOARD_LL,
					llKeyFunc,
					System.Runtime.InteropServices.Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]).ToInt32(), 0);
			}
			/// <summary>
			/// Unhook
			/// </summary>
			public void Unhook()
			{
				if (intLLKey == 0) return;
				UnHookWindowsEx(intLLKey);
				intLLKey = 0;
			}
			/// <summary>
			/// Check is hook.
			/// </summary>
			public bool IsHook
			{
				get { return (intLLKey != 0); }
			}

			#endregion

			#region IDisposable Members

			/// <summary>
			/// Dispose
			/// </summary>
			public void Dispose()
			{
				Unhook();
			}

			#endregion
		}

		#endregion

		#region Private Property

		private static LowLevelKeyboardHook _llSysKey = null;
		private static LowLevelKeyboardHook SystemKeys
		{
			get
			{
				if (_llSysKey == null)
				{
					_llSysKey = new LowLevelKeyboardHook();
				}
				return _llSysKey;
			}
		}

		#endregion

		#region Enums

		/// <summary>
		/// ShutDownMode
		/// </summary>
		enum ShutDownMode : uint
		{
			EWX_LOGOFF = 0x00000000,
			EWX_SHUTDOWN = 0x00000001,
			EWX_REBOOT = 0x00000002,
			EWX_FORCE = 0x00000004,
			EWX_POWEROFF = 0x00000008,
			EWX_FORCEIFHUNG = 0x00000010
		}
		/// <summary>
		/// ReasonMajor
		/// </summary>
		enum ReasonMajor : uint
		{
			SHTDN_REASON_MAJOR_OTHER = 0x00000000,
			SHTDN_REASON_MAJOR_NONE = 0x00000000,
			SHTDN_REASON_MAJOR_HARDWARE = 0x00010000,
			SHTDN_REASON_MAJOR_OPERATIONSYSTEM = 0x00020000,
			SHTDN_REASON_MAJOR_SOFTWARE = 0x00030000,
			SHTDN_REASON_MAJOR_APPLICATION = 0x00040000,
			SHTDN_REASON_MAJOR_SYSTEM = 0x00050000,
			SHTDN_REASON_MAJOR_POWER = 0x00060000
		}
		/// <summary>
		/// ReasonMinor
		/// </summary>
		enum ReasonMinor : uint
		{
			SHTDN_REASON_MINOR_OTHER = 0x00000000,
			SHTDN_REASON_MINOR_NONE = 0x000000FF,
			SHTDN_REASON_MINOR_MAINTENANCE = 0x00000001,
			SHTDN_REASON_MINOR_INSTALLATION = 0x00000002,
			SHTDN_REASON_MINOR_UPGRADE = 0x00000003,
			SHTDN_REASON_MINOR_RECONFIG = 0x00000004,
			SHTDN_REASON_MINOR_HUNG = 0x00000005,
			SHTDN_REASON_MINOR_UNSTABLE = 0x00000006,
			SHTDN_REASON_MINOR_DISK = 0x00000007,
			SHTDN_REASON_MINOR_PROCESSOR = 0x00000008,
			SHTDN_REASON_MINOR_NETWORKCARD = 0x00000009,
			SHTDN_REASON_MINOR_POWER_SUPPLY = 0x0000000A,
			SHTDN_REASON_MINOR_CORDUNPLUGGED = 0x0000000B,
			SHTDN_REASON_MINOR_ENVIRONMENT = 0x0000000C,
			SHTDN_REASON_MINOR_HARDWARE_DRIVER = 0x0000000D,
			SHTDN_REASON_MINOR_OTHERDRIVER = 0x0000000E,
			SHTDN_REASON_MINOR_BLUESCREEN = 0x0000000F,
			SHTDN_REASON_UNKNOWN = SHTDN_REASON_MINOR_NONE
		}
		/// <summary>
		/// ReasonFlag
		/// </summary>
		enum ReasonFlag : uint
		{
			SHTDN_REASON_FLAG_USER_DEFINED = 0x40000000,
			SHTDN_REASON_FLAG_PLANNED = 0x80000000
		}

		#region SW

		/// <summary>
		/// SW
		/// </summary>
		enum SW
		{
			/// <summary>
			/// HIDE
			/// </summary>
			HIDE = 0,
			/// <summary>
			/// SHOWNORMAL
			/// </summary>
			SHOWNORMAL = 1,
			/// <summary>
			/// NORMAL
			/// </summary>
			NORMAL = 1,
			/// <summary>
			/// SHOWMINIMIZED
			/// </summary>
			SHOWMINIMIZED = 2,
			/// <summary>
			/// SHOWMAXIMIZED
			/// </summary>
			SHOWMAXIMIZED = 3,
			/// <summary>
			/// MAXIMIZE
			/// </summary>
			MAXIMIZE = 3,
			/// <summary>
			/// SHOWNOACTIVATE
			/// </summary>
			SHOWNOACTIVATE = 4,
			/// <summary>
			/// SHOW
			/// </summary>
			SHOW = 5,
			/// <summary>
			/// MINIMIZE
			/// </summary>
			MINIMIZE = 6,
			/// <summary>
			/// SHOWMINNOACTIVE
			/// </summary>
			SHOWMINNOACTIVE = 7,
			/// <summary>
			/// SHOWNA
			/// </summary>
			SHOWNA = 8,
			/// <summary>
			/// RESTORE
			/// </summary>
			RESTORE = 9,
			/// <summary>
			/// SHOWDEFAULT
			/// </summary>
			SHOWDEFAULT = 10,
			/// <summary>
			/// FORCEMINIMIZE
			/// </summary>
			FORCEMINIMIZE = 11,
			/// <summary>
			/// MAX
			/// </summary>
			MAX = 11
		}

		#endregion

		/// <summary>
		/// Enumeration to be used for those Win32 function that return BOOL
		/// </summary>
		enum Bool
		{
			/// <summary>
			/// False Value
			/// </summary>
			False = 0,
			/// <summary>
			/// True value
			/// </summary>
			True
		};

		/// <summary>
		/// Enumeration for the raster operations used in BitBlt.
		/// In C++ these are actually #define. But to use these
		/// constants with C#, a new enumeration type is defined.
		/// </summary>
		enum TernaryRasterOperations
		{
			/// <summary>
			/// SRCCOPY
			/// </summary>
			SRCCOPY = 0x00CC0020, /* dest = source                   */
			/// <summary>
			/// SRCPAINT
			/// </summary>
			SRCPAINT = 0x00EE0086, /* dest = source OR dest           */
			/// <summary>
			/// SRCAND
			/// </summary>
			SRCAND = 0x008800C6, /* dest = source AND dest          */
			/// <summary>
			/// SRCINVERT
			/// </summary>
			SRCINVERT = 0x00660046, /* dest = source XOR dest          */
			/// <summary>
			/// SRCERASE
			/// </summary>
			SRCERASE = 0x00440328, /* dest = source AND (NOT dest )   */
			/// <summary>
			/// NOTSRCCOPY
			/// </summary>
			NOTSRCCOPY = 0x00330008, /* dest = (NOT source)             */
			/// <summary>
			/// NOTSRCERASE
			/// </summary>
			NOTSRCERASE = 0x001100A6, /* dest = (NOT src) AND (NOT dest) */
			/// <summary>
			/// MERGECOPY
			/// </summary>
			MERGECOPY = 0x00C000CA, /* dest = (source AND pattern)     */
			/// <summary>
			/// MERGEPAINT
			/// </summary>
			MERGEPAINT = 0x00BB0226, /* dest = (NOT source) OR dest     */
			/// <summary>
			/// PATCOPY
			/// </summary>
			PATCOPY = 0x00F00021, /* dest = pattern                  */
			/// <summary>
			/// PATPAINT
			/// </summary>
			PATPAINT = 0x00FB0A09, /* dest = DPSnoo                   */
			/// <summary>
			/// PATINVERT
			/// </summary>
			PATINVERT = 0x005A0049, /* dest = pattern XOR dest         */
			/// <summary>
			/// DSTINVERT
			/// </summary>
			DSTINVERT = 0x00550009, /* dest = (NOT dest)               */
			/// <summary>
			/// BLACKNESS
			/// </summary>
			BLACKNESS = 0x00000042, /* dest = BLACK                    */
			/// <summary>
			/// WHITENESS
			/// </summary>
			WHITENESS = 0x00FF0062, /* dest = WHITE                    */
		};

		#endregion

		#region API

		// ExitWindowsEx
		[DllImport("User32.DLL")]
		private static extern int ExitWindowsEx(UInt32 flags, UInt32 dwReason);
		// FindWindowEx
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childAfter, string sClassName, IntPtr sWindowTitle);
		// FindWindowEx
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childAfter, string sClassName, string sWindowTitle);
		// ShowWindow
		[DllImport("User32.DLL", EntryPoint = "ShowWindow")]
		private static extern IntPtr ShowWindow(IntPtr hwnd, SW showState);
		// FindWindow
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr FindWindow(string lpClassName, IntPtr lpWindowName);
		// EnableWindow
		[DllImport("User32.DLL", EntryPoint = "EnableWindow")]
		private static extern bool EnableWindow(System.IntPtr hWnd, bool bEnable);
		/// <summary>
		/// GetDC
		/// </summary>
		/// <param name="hWnd"></param>
		/// <returns></returns>
		// GetDC
		[DllImport("User32.dll")]
		private extern static System.IntPtr GetDC(System.IntPtr hWnd);
		/// <summary>
		/// GetDesktopWindow
		/// </summary>
		/// <returns></returns>
		// GetDesktopWindow
		[DllImport("User32.dll")]
		private static extern IntPtr GetDesktopWindow();
		/// <summary>
		/// GetWindowDC
		/// </summary>
		/// <param name="hwnd"></param>
		/// <returns></returns>
		// GetWindowDC
		[DllImport("User32.DLL", EntryPoint = "GetWindowDC")]
		private static extern IntPtr GetWindowDC(IntPtr hwnd);
		/// <summary>
		/// ReleaseDC
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="hDC"></param>
		/// <returns></returns>
		// ReleaseDC
		[DllImport("User32.dll")]
		private extern static IntPtr ReleaseDC(System.IntPtr hWnd, System.IntPtr hDC); //modified to include hWnd
		/// <summary>
		/// CreateCompatibleBitmap
		/// </summary>
		/// <param name="hObject"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		// CreateCompatibleBitmap
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr CreateCompatibleBitmap(IntPtr hObject, int width, int height);
		/// <summary>
		/// CreateCompatibleDC
		/// </summary>
		/// <param name="hDC"></param>
		/// <returns></returns>
		// CreateCompatibleDC
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr CreateCompatibleDC(IntPtr hDC);
		/// <summary>
		/// BitBlt
		/// </summary>
		/// <param name="hObject"></param>
		/// <param name="nXDest"></param>
		/// <param name="nYDest"></param>
		/// <param name="nWidth"></param>
		/// <param name="nHeight"></param>
		/// <param name="hObjSource"></param>
		/// <param name="nXSrc"></param>
		/// <param name="nYSrc"></param>
		/// <param name="dwRop"></param>
		/// <returns></returns>
		// BitBlt
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern Bool BitBlt(IntPtr hObject,
			int nXDest,
			int nYDest,
			int nWidth,
			int nHeight,
			IntPtr hObjSource,
			int nXSrc,
			int nYSrc,
			TernaryRasterOperations dwRop);
		/// <summary>
		/// DeleteDC
		/// </summary>
		/// <param name="hdc"></param>
		/// <returns></returns>
		// DeleteDC
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern Bool DeleteDC(IntPtr hdc);
		/// <summary>
		/// DeleteObject
		/// </summary>
		/// <param name="hObject"></param>
		/// <returns></returns>
		// DeleteObject
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern Bool DeleteObject(IntPtr hObject);
		/// <summary>
		/// GetDeviceCaps
		/// </summary>
		/// <param name="hdc"></param>
		/// <param name="nIndex"></param>
		/// <returns></returns>
		// GetDeviceCaps
		[DllImport("GDI32.dll")]
		private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
		/// <summary>
		/// SelectObject
		/// </summary>
		/// <param name="hDC"></param>
		/// <param name="hObject"></param>
		/// <returns></returns>
		// SelectObject
		[DllImport("gdi32.dll", ExactSpelling = true)]
		private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern int SetLocalTime(ref __SystemTime lpSystemTime);

		//struct for date/time apis
		private struct __SystemTime
		{
			public short wYear;
			public short wMonth;
			public short wDayOfWeek;
			public short wDay;
			public short wHour;
			public short wMinute;
			public short wSecond;
			public short wMilliseconds;
		}

		#endregion

		#region Private Method

		#region Set Window Visible

		/// <summary>
		/// Set Window Visible
		/// </summary>
		/// <param name="baseClassName">Base ClassName</param>
		/// <param name="parentClassName">Parent ClassName</param>
		/// <param name="targetClassName">Target ClassName</param>
		/// <param name="visible">is visible</param>
		private static void SetWindowVisible(string baseClassName,
			string parentClassName, string targetClassName, bool visible)
		{
			IntPtr wndHwnd = IntPtr.Zero;
			IntPtr findClass = IntPtr.Zero;
			IntPtr findParent = IntPtr.Zero;

			findClass = FindWindow(baseClassName, IntPtr.Zero);
			if (parentClassName.Length <= 0)
			{
				if (targetClassName.Length <= 0)
				{
					wndHwnd = findClass;
				}
				else
				{
					wndHwnd = FindWindowEx(findClass, IntPtr.Zero, targetClassName, IntPtr.Zero);
				}
			}
			else
			{
				findParent = FindWindowEx(findClass, IntPtr.Zero, parentClassName, IntPtr.Zero);
				if (targetClassName.Length <= 0)
				{
					wndHwnd = findParent;
				}
				else
				{
					wndHwnd = FindWindowEx(findParent, IntPtr.Zero, targetClassName, IntPtr.Zero);
				}
			}

			if (visible)
			{
				ShowWindow(wndHwnd, SW.SHOW);
			}
			else
			{
				ShowWindow(wndHwnd, SW.HIDE);
			}
		}
		/// <summary>
		/// Set Window Visible
		/// </summary>
		/// <param name="baseClassName">Base ClassName</param>
		/// <param name="parentClassName">Parent ClassName</param>
		/// <param name="targetClassName">Target ClassName</param>
		/// <param name="windowTitle">Window Title Text.</param>
		/// <param name="visible">is visible</param>
		private static void SetWindowVisible(string baseClassName,
			string parentClassName, string targetClassName, string windowTitle, bool visible)
		{
			IntPtr wndHwnd = IntPtr.Zero;
			IntPtr findClass = IntPtr.Zero;
			IntPtr findParent = IntPtr.Zero;

			findClass = FindWindow(baseClassName, IntPtr.Zero);
			if (parentClassName.Length <= 0)
			{
				if (targetClassName.Length <= 0)
				{
					wndHwnd = findClass;
				}
				else
				{
					if (windowTitle.Length > 0)
					{
						wndHwnd = FindWindowEx(findClass, IntPtr.Zero, targetClassName, windowTitle);
					}
					else
					{
						wndHwnd = FindWindowEx(findClass, IntPtr.Zero, targetClassName, IntPtr.Zero);
					}
				}
			}
			else
			{
				findParent = FindWindowEx(findClass, IntPtr.Zero, parentClassName, IntPtr.Zero);
				if (targetClassName.Length <= 0)
				{
					wndHwnd = findParent;
				}
				else
				{
					if (windowTitle.Length > 0)
					{
						wndHwnd = FindWindowEx((findParent == IntPtr.Zero) ? findClass : findParent,
							IntPtr.Zero, targetClassName, windowTitle);
					}
					else
					{
						wndHwnd = FindWindowEx((findParent == IntPtr.Zero) ? findClass : findParent,
							IntPtr.Zero, targetClassName, IntPtr.Zero);
					}
				}
			}

			if (visible)
			{
				ShowWindow(wndHwnd, SW.SHOW);
			}
			else
			{
				ShowWindow(wndHwnd, SW.HIDE);
			}
		}

		#endregion

		#endregion

		#region Exit Windows

		#region Misc Exit Windows Functions

		/// <summary>
		/// LogOff
		/// </summary>
		public static void LogOff()
		{
			UInt32 flags = (UInt32)ShutDownMode.EWX_LOGOFF;
			UInt32 dwReason = (UInt32)ReasonMajor.SHTDN_REASON_MAJOR_APPLICATION &
				(UInt32)ReasonMinor.SHTDN_REASON_MINOR_MAINTENANCE &
				(UInt32)ReasonFlag.SHTDN_REASON_FLAG_PLANNED;
			ExitWindowsEx(flags, dwReason);
		}
		/// <summary>
		/// Shutdown
		/// </summary>
		public static void Shutdown()
		{
			UInt32 flags = (UInt32)ShutDownMode.EWX_FORCE &
				(UInt32)ShutDownMode.EWX_SHUTDOWN;
			UInt32 dwReason = (UInt32)ReasonMajor.SHTDN_REASON_MAJOR_APPLICATION &
				(UInt32)ReasonMinor.SHTDN_REASON_MINOR_MAINTENANCE &
				(UInt32)ReasonFlag.SHTDN_REASON_FLAG_PLANNED;
			ExitWindowsEx(flags, dwReason);
		}
		/// <summary>
		/// Reboot
		/// </summary>
		public static void Reboot()
		{
			UInt32 flags = (UInt32)ShutDownMode.EWX_FORCE &
				(UInt32)ShutDownMode.EWX_REBOOT;
			UInt32 dwReason = (UInt32)ReasonMajor.SHTDN_REASON_MAJOR_APPLICATION &
				(UInt32)ReasonMinor.SHTDN_REASON_MINOR_MAINTENANCE &
				(UInt32)ReasonFlag.SHTDN_REASON_FLAG_PLANNED;
			ExitWindowsEx(flags, dwReason);
		}
		/// <summary>
		/// PowerOff
		/// </summary>
		public static void PowerOff()
		{
			UInt32 flags = (UInt32)ShutDownMode.EWX_FORCE &
				(UInt32)ShutDownMode.EWX_SHUTDOWN &
				(UInt32)ShutDownMode.EWX_POWEROFF;
			UInt32 dwReason = (UInt32)ReasonMajor.SHTDN_REASON_MAJOR_APPLICATION &
				(UInt32)ReasonMinor.SHTDN_REASON_MINOR_MAINTENANCE &
				(UInt32)ReasonFlag.SHTDN_REASON_FLAG_PLANNED;
			ExitWindowsEx(flags, dwReason);
		}

		#endregion

		#endregion

		#region DesktopIcons

		/// <summary>
		/// Show Desktop Icons
		/// </summary>
		/// <param name="visible">is visible</param>
		public static void ShowDesktopIcons(bool visible)
		{
			IntPtr iconsHwnd;
			iconsHwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Progman", IntPtr.Zero);
			if (visible)
			{
				if (iconsHwnd.ToInt32() == 1) return;
			}
			else
			{
				if (iconsHwnd.ToInt32() == 0) return;
			}
			if (visible) ShowWindow(iconsHwnd, SW.SHOW);
			else ShowWindow(iconsHwnd, SW.HIDE);
		}
		/// <summary>
		/// Show Desktop Icons
		/// </summary>
		public static void ShowDesktopIcons()
		{
			ShowDesktopIcons(true);
		}
		/// <summary>
		/// Hide Desktop Icons
		/// </summary>
		public static void HideDesktopIcons()
		{
			ShowDesktopIcons(false);
		}

		#endregion

		#region System Tray and related objects

		#region TrayClock

		/// <summary>
		/// Show Tray Clock
		/// </summary>
		/// <param name="visible">is visible</param>
		public static void ShowTrayClock(bool visible)
		{
			SetWindowVisible("Shell_TrayWnd", "TrayNotifyWnd",
				"TrayClockWClass", visible);
		}
		/// <summary>
		/// Show Tray Clock
		/// </summary>
		public static void ShowTrayClock()
		{
			ShowTrayClock(true);
		}
		/// <summary>
		/// Hide Tray Clock
		/// </summary>
		public static void HideTrayClock()
		{
			ShowTrayClock(false);
		}

		#endregion

		#region Start Button

		/// <summary>
		/// Show Start Button. Note that in window vista should used with Show/Hide System Tray
		/// </summary>
		/// <param name="visible">is visible</param>
		public static void ShowStartButton(bool visible)
		{
			// For version before vista
			SetWindowVisible("Shell_TrayWnd", "Button", "", visible);
			// For window vista should used with Show/Hide System Tray
			SetWindowVisible("Start", "Button", "", visible);
		}
		/// <summary>
		/// Show Start Button
		/// </summary>
		public static void ShowStartButton()
		{
			ShowStartButton(true);
		}
		/// <summary>
		/// Hide Start Button
		/// </summary>
		public static void HideStartButton()
		{
			ShowStartButton(false);
		}

		#endregion

		#region System Tray

		/// <summary>
		/// Show System Tray
		/// </summary>
		/// <param name="visible">is visible</param>
		public static void ShowSystemTray(bool visible)
		{
			SetWindowVisible("Shell_TrayWnd", "", "", visible);
		}
		/// <summary>
		/// Show System Tray
		/// </summary>
		public static void ShowSystemTray()
		{
			ShowSystemTray(true);
		}
		/// <summary>
		/// Hide System Tray
		/// </summary>
		public static void HideSystemTray()
		{
			ShowSystemTray(false);
		}

		#endregion

		#region Tray Icons

		/// <summary>
		/// Show Tray Icons
		/// </summary>
		/// <param name="visible">is visible</param>
		public static void ShowTrayIcons(bool visible)
		{
			SetWindowVisible("Shell_TrayWnd", "TrayNotifyWnd", "", visible);
		}
		/// <summary>
		/// Show System Tray Icons
		/// </summary>
		public static void ShowTrayIcons()
		{
			ShowTrayIcons(true);
		}
		/// <summary>
		/// Hide System Tray Icons
		/// </summary>
		public static void HideTrayIcons()
		{
			ShowTrayIcons(false);
		}

		#endregion

		#region Quick Lanch

		/// <summary>
		/// Show Quick Launch bar. Tested on Vista only.
		/// </summary>
		/// <param name="visible">The visible flag</param>
		public static void ShowQuickLaunch(bool visible)
		{
			// For vista
			SetWindowVisible("Shell_TrayWnd", "ReBarWindow32", "ToolBarWindow32",
				"Quick Launch", visible);
		}
		/// <summary>
		/// Show Quick Launch bar. Tested on Vista only.
		/// </summary>
		public static void ShowQuickLaunch()
		{
			ShowQuickLaunch(true);
		}
		/// <summary>
		/// Hide Quick Launch bar. Tested on Vista only.
		/// </summary>
		public static void HideQuickLaunch()
		{
			ShowQuickLaunch(false);
		}

		#endregion

		#endregion

		#region Enable/Disable

		#region Task Bar

		/// <summary>
		/// Disable TaskBar
		/// </summary>
		/// <param name="disable">is disable</param>
		public static void DisableTaskBar(bool disable)
		{
			IntPtr hwnd = FindWindow("Shell_traywnd", IntPtr.Zero);
			EnableWindow(hwnd, !disable);
		}
		/// <summary>
		/// Disable TaskBar
		/// </summary>
		public static void DisableTaskBar()
		{
			DisableTaskBar(true);
		}
		/// <summary>
		/// Enable TaskBar
		/// </summary>
		public static void EnableTaskBar()
		{
			DisableTaskBar(false);
		}

		#endregion

		#region Task Manager

		/// <summary>
		/// Disable TaskManager
		/// </summary>
		/// <param name="disable">is disable</param>
		public static void DisableTaskManager(bool disable)
		{
			MethodBase med = MethodBase.GetCurrentMethod();
			try
			{
				string KEY_DisableTaskMgr = @"Software\Microsoft\Windows\CurrentVersion\Policies\System";
				string VAL_DisableTaskMgr = @"DisableTaskMgr";
				System.UInt32 val = 1;

				RegistryKey reg = Registry.CurrentUser;
				RegistryKey subkey = reg.OpenSubKey(KEY_DisableTaskMgr, true);
				if (subkey == null) subkey = reg.CreateSubKey(KEY_DisableTaskMgr);

				if (subkey != null)
				{
					if (disable)
					{
						subkey.SetValue(VAL_DisableTaskMgr, val);
					}
					else
					{
						subkey.DeleteValue(VAL_DisableTaskMgr, false);
					}
					subkey.Close();
					subkey = null;
				}
				reg.Close();
				reg = null;
			}
			catch (Exception ex)
			{
				// Write Debug
				ex.Err(med);
			}
		}
		/// <summary>
		/// Disable Task Manager
		/// </summary>
		public static void DisableTaskManager()
		{
			DisableTaskManager(true);
		}
		/// <summary>
		/// Enable Task Manager
		/// </summary>
		public static void EnableTaskManager()
		{
			DisableTaskManager(false);
		}

		#endregion

		#region System Keys

		/// <summary>
		/// Check is system key is hook.
		/// </summary>
		/// <returns>Return ture if system key is hook.</returns>
		public static bool IsHookSystemHey()
		{
			return SystemKeys.IsHook;
		}
		/// <summary>
		/// Disable System Keys
		/// </summary>
		/// <param name="disable">is disable</param>
		public static void DisableSystemKeys(bool disable)
		{
			if (disable) SystemKeys.Hook();
			else SystemKeys.Unhook();
		}
		/// <summary>
		/// Disable System Keys
		/// </summary>
		public static void DisableSystemKeys()
		{
			DisableSystemKeys(true);
		}
		/// <summary>
		/// Enable System Keys
		/// </summary>
		public static void EnableSystemKeys()
		{
			DisableSystemKeys(false);
		}

		#endregion

		#endregion

		#region Shell

		/// <summary>
		/// Create Process
		/// </summary>
		/// <param name="executeFileName">executable file name</param>
		/// <returns>Process Instance</returns>
		public static Process CreateProcess(string executeFileName)
		{
			return CreateProcess(executeFileName, ProcessWindowStyle.Normal);
		}
		/// <summary>
		/// Create Process
		/// </summary>
		/// <param name="executeFileName">executable file name</param>
		/// <param name="style">Process Window Style flags</param>
		/// <returns>Process Instance</returns>
		public static Process CreateProcess(string executeFileName, ProcessWindowStyle style)
		{
			if (!System.IO.File.Exists(executeFileName)) return null;
			Process p = new Process();
			p.StartInfo.FileName = executeFileName;
			p.StartInfo.WindowStyle = style;
			p.StartInfo.UseShellExecute = true;
			p.EnableRaisingEvents = true; // enable event for Process.Exited
			return p;
		}
		/// <summary>
		/// Execute
		/// </summary>
		/// <param name="executeFileName">executable file name</param>
		/// <param name="waitForExit">true for wait until executable file is exitd</param>
		public static void Execute(string executeFileName, bool waitForExit)
		{
			Process p = CreateProcess(executeFileName);
			p.Start();
			System.Windows.Forms.Application.DoEvents();
			if (waitForExit)
			{
				p.WaitForExit();
				p.Close();
			}
		}
		/// <summary>
		/// Open web page in Internet Explorer
		/// </summary>
		/// <param name="url">url string</param>
		public static void OpenUrlIE(string url)
		{
			Process p = new Process();
			// Get the process start information of iexplore
			ProcessStartInfo startInfo = new ProcessStartInfo("iexplore.exe", url);
			// Assign 'StartInfo' of outlook to 'StartInfo' of 'myProcess' object.
			p.StartInfo = startInfo;
			// Create a outlook
			p.Start();
		}

		#endregion

		#region StartUp

		/// <summary>
		/// Load On Startup
		/// </summary>
		/// <param name="serviceName">Service Name</param>
		/// <param name="fileName">Execute file's name</param>
		public static void LoadOnStartup(string serviceName, string fileName)
		{
			MethodBase med = MethodBase.GetCurrentMethod();
			try
			{
				if (serviceName.Length <= 0) return;
				if (fileName.Length <= 0) return;
				if (!System.IO.File.Exists(fileName)) return;
				string KEY_Run = @"Software\Microsoft\Windows\CurrentVersion\Run";
				string VAR_App = serviceName;
				RegistryKey reg = Registry.LocalMachine;
				RegistryKey subkey = reg.OpenSubKey(KEY_Run, true);
				if (subkey == null) subkey = reg.CreateSubKey(KEY_Run);
				if (subkey != null)
				{
					subkey.SetValue(VAR_App, fileName);
					subkey.Close();
					subkey = null;
				}
				reg.Close();
				reg = null;
			}
			catch (Exception ex)
			{
				// Write Debug
				ex.Err(med);
			}
		}
		/// <summary>
		/// Remove From StartUp
		/// </summary>
		/// <param name="serviceName">Service Name</param>
		public static void RemoveFromStartUp(string serviceName)
		{
			MethodBase med = MethodBase.GetCurrentMethod();
			try
			{
				if (serviceName.Length <= 0) return;
				string KEY_Run = @"Software\Microsoft\Windows\CurrentVersion\Run";
				string VAR_App = serviceName;
				RegistryKey reg = Registry.LocalMachine;
				RegistryKey subkey = reg.OpenSubKey(KEY_Run, true);
				if (subkey != null)
				{
					subkey.DeleteValue(VAR_App, false);
					subkey.Close();
					subkey = null;
				}
				reg.Close();
				reg = null;
			}
			catch (Exception ex)
			{
				// Write Debug
				ex.Err(med);
			}
		}
		/// <summary>
		/// IsLoadOnStartup
		/// </summary>
		/// <param name="serviceName">Service Name</param>
		/// <returns>true if service is already load on startup</returns>
		public static bool IsLoadOnStartup(string serviceName)
		{
			MethodBase med = MethodBase.GetCurrentMethod();
			try
			{
				if (serviceName.Length <= 0) return false;

				bool exists = false;
				string KEY_Run = @"Software\Microsoft\Windows\CurrentVersion\Run";
				string VAR_App = serviceName;
				RegistryKey reg = Registry.LocalMachine;
				RegistryKey subkey = reg.OpenSubKey(KEY_Run, true);
				if (subkey != null)
				{
					object result = subkey.GetValue(VAR_App);
					if (result != null) exists = true;
					subkey.Close();
					subkey = null;
				}
				reg.Close();
				reg = null;
				return exists;
			}
			catch (Exception ex)
			{
				// Write Debug
				ex.Err(med);
				return false;
			}
		}

		#endregion

		#region CaptureScreen

		/// <summary>
		/// CaptureScreen
		/// </summary>
		/// <returns></returns>
		public static Bitmap CaptureScreen()
		{
			System.IntPtr desktopDC = GetDC(System.IntPtr.Zero);
			Bitmap bm = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
			Graphics g = Graphics.FromImage(bm);
			System.IntPtr bmDC = g.GetHdc();
			BitBlt(bmDC, 0, 0, bm.Width, bm.Height, desktopDC, 0, 0, TernaryRasterOperations.SRCCOPY  /* 0x00CC0020 */);
			ReleaseDC(System.IntPtr.Zero, desktopDC);
			g.ReleaseHdc(bmDC);
			g.Dispose();
			return bm;
		}
		/// <summary>
		/// CaptureScreen
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public static Bitmap CaptureScreen(Control control)
		{
			if (control == null) return null;
			if ((control.Width <= 0) || (control.Height <= 0)) return null;
			System.IntPtr desktopDC = GetDC(System.IntPtr.Zero);
			Bitmap bm = new Bitmap(control.Width, control.Height);
			Graphics g = Graphics.FromImage(bm);
			System.IntPtr bmDC = g.GetHdc();
			Point pt = control.PointToScreen(control.ClientRectangle.Location);
			BitBlt(bmDC, 0, 0, bm.Width, bm.Height, desktopDC, pt.X, pt.Y, TernaryRasterOperations.SRCCOPY  /* 0x00CC0020 */);
			ReleaseDC(System.IntPtr.Zero, desktopDC);
			g.ReleaseHdc(bmDC);
			g.Dispose();
			return bm;
		}

		#endregion

		#region CaptureControl

		/// <summary>
		/// CaptureControl
		/// </summary>
		/// <param name="ctrl"></param>
		/// <returns></returns>
		public static Bitmap CaptureControl(Control ctrl)
		{
			System.IntPtr desktopDC = GetDC(System.IntPtr.Zero);
			System.IntPtr srcDC = GetDC(ctrl.Handle);
			Bitmap bm = new Bitmap(ctrl.Width, ctrl.Height);
			Graphics g = Graphics.FromImage(bm);
			System.IntPtr bmDC = g.GetHdc();
			BitBlt(bmDC, 0, 0, bm.Width, bm.Height, srcDC, 0, 0, TernaryRasterOperations.SRCCOPY  /* 0x00CC0020 */);
			ReleaseDC(ctrl.Handle, srcDC);
			g.ReleaseHdc(bmDC);
			g.Dispose();
			return bm;
		}

		#endregion

		#region Capture Screen with Save option

		/// <summary>
		/// CaptureScreen
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="imageFormat"></param>
		public static void CaptureScreen(string fileName, ImageFormat imageFormat)
		{
			IntPtr hdcSrc = GetWindowDC(GetDesktopWindow());
			IntPtr hdcDest = CreateCompatibleDC(hdcSrc);
			IntPtr hBitmap = CreateCompatibleBitmap(hdcSrc, GetDeviceCaps(hdcSrc, 8), GetDeviceCaps(hdcSrc, 10));

			SelectObject(hdcDest, hBitmap);
			BitBlt(hdcDest, 0, 0, GetDeviceCaps(hdcSrc, 8), GetDeviceCaps(hdcSrc, 10),
				hdcSrc, 0, 0, TernaryRasterOperations.SRCCOPY);

			SaveImageAs(hBitmap, fileName, imageFormat);
			Cleanup(hBitmap, hdcSrc, hdcDest);
		}
		/// <summary>
		/// Clean DC/reource by handle
		/// </summary>
		/// <param name="hBitmap"></param>
		/// <param name="hdcSrc"></param>
		/// <param name="hdcDest"></param>
		private static void Cleanup(IntPtr hBitmap, IntPtr hdcSrc, IntPtr hdcDest)
		{
			ReleaseDC(GetDesktopWindow(), hdcSrc);
			DeleteDC(hdcDest);
			DeleteObject(hBitmap);
		}

		private static void SaveImageAs(IntPtr hBitmap, string fileName, ImageFormat imageFormat)
		{
			Bitmap bmp = Image.FromHbitmap(hBitmap);
			Bitmap image = new Bitmap(bmp, bmp.Width, bmp.Height);
			image.Save(fileName, imageFormat);
		}

		#endregion

		#region Create Shortcut

		/// <summary>
		/// Create Desktop Shortcut
		/// </summary>
		/// <param name="shortcutName">The shortcut name. (only name not include drive, path and extension).</param>
		/// <param name="sourceFileName">The target file to make shortcut.</param>
		public static void CreateDesktopShortcut(string shortcutName,
			string sourceFileName)
		{
			MethodBase med = MethodBase.GetCurrentMethod();
			try
			{
				string targetLNKFileName =
					Path.Combine(NLib.IO.Folders.Users.Desktop, shortcutName);

				int i = 1;
				string targetLNKFullFileName = targetLNKFileName + ".lnk";
				while (System.IO.File.Exists(targetLNKFullFileName))
				{
					targetLNKFullFileName = targetLNKFileName + i.ToString() + ".lnk";
					i++;
				}

				Shortcut lnk = new Shortcut(targetLNKFullFileName);
				lnk.Path = sourceFileName;
				lnk.Save();
			}
			catch (Exception ex)
			{
				// Write Debug
				ex.Err(med);
			}
		}
		/// <summary>
		/// Create Shortcut
		/// </summary>
		/// <param name="shortcutFileName">The shortcut file's name (should include .lnk)</param>
		/// <param name="sourceFileName">The target file to make shortcut.</param>
		public static void CreateShortcut(string shortcutFileName,
			string sourceFileName)
		{
			MethodBase med = MethodBase.GetCurrentMethod();
			try
			{
				Shortcut lnk = new Shortcut(shortcutFileName);
				lnk.Path = sourceFileName;
				lnk.Save();
			}
			catch (Exception ex)
			{
				ex.Err(med);
			}
		}
		/// <summary>
		/// Create Shortcut
		/// </summary>
		/// <param name="shortcutFileName">The shortcut file's name (should include .lnk)</param>
		/// <param name="sourceFileName">The target file to make shortcut.</param>
		/// <param name="description">The description string.</param>
		public static void CreateShortcut(string shortcutFileName,
			string sourceFileName, string description)
		{
			MethodBase med = MethodBase.GetCurrentMethod();
			try
			{
				Shortcut lnk = new Shortcut(shortcutFileName);
				lnk.Path = sourceFileName;
				lnk.Description = description;
				lnk.Save();
			}
			catch (Exception ex)
			{
				ex.Err(med);
			}
		}
		/// <summary>
		/// Create Shortcut
		/// </summary>
		/// <param name="shortcutFileName">The shortcut file's name (should include .lnk)</param>
		/// <param name="sourceFileName">The target file to make shortcut.</param>
		/// <param name="args">The Argument string for execute.</param>
		/// <param name="description">The description string.</param>
		public static void CreateShortcut(string shortcutFileName,
			string sourceFileName, string args, string description)
		{
			MethodBase med = MethodBase.GetCurrentMethod();
			try
			{
				Shortcut lnk = new Shortcut(shortcutFileName);
				lnk.Path = sourceFileName;
				lnk.Description = description;
				lnk.Arguments = args;
				lnk.Save();
			}
			catch (Exception ex)
			{
				ex.Err(med);
			}
		}

		#endregion

		#region Change Local Date/Time

		/// <summary>
		/// Change Local Date Time
		/// </summary>
		/// <param name="value">The DateTime value to set as current datetime.</param>
		public static void ChangeLocalDateTime(DateTime value)
		{
			MethodBase med = MethodBase.GetCurrentMethod();
			try
			{
				__SystemTime systNew = new __SystemTime();
				//set the properties
				systNew.wDay = (short)value.Day;
				systNew.wMonth = (short)value.Month;
				systNew.wYear = (short)value.Year;
				systNew.wHour = (short)value.Hour;
				systNew.wMinute = (short)value.Minute;
				systNew.wSecond = (short)value.Second;
				//and update using the api
				SetLocalTime(ref systNew);
			}
			catch (Exception ex)
			{
				ex.Err(med);
			}
		}

		#endregion
	}

	#endregion
}
