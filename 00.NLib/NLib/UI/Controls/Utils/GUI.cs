#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-10-19
=================
- Controls Library updated.
  - Ported GUI Utils class from GFA40 to NLib v4.0.4.1.

======================================================================================================================
Update 2010-09-07
=================
- Controls Library updated.
  - Add supports uxtheme load in DynamicFlickerFree class
  - Change DynamicFlickerFree class name to GUI class.
  - Change Method DisbleFlicker to Boost for more proper name.

======================================================================================================================
Update 2010-02-01
=================
- Controls Library updated.
  - Ported DynamicFlickerFree class from GFA37 to GFA38v3.

======================================================================================================================
Update 2009-04-15
=================
- DynamicFlickerFree class updated. Solve flicker problem for ListView and TreeView.
  This will work only used Common Control that has version over than 6.0 basically on XP system
  or OS above XP version.

======================================================================================================================
Update 2008-08-21
=================
- DynamicFlickerFree class added. This class provide reflection method to override ControlStyle
  for control that has flicker problem like listview for better paint quality or less flicker.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

#endregion

namespace NLib.Controls.Utils
{
    #region Control helper class

    /// <summary>
    /// GUI class. 
    /// This class provide reflection method to override ControlStyle
    /// for control that has flicker problem like listview for better 
    /// paint quality or less flicker and also add supports to enable
    /// visual style like theme on controls.
    /// </summary>
    public sealed class GUI
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private GUI() { }

        #endregion

        #region ListView/TreeView Related API

        #region LVS_EX

        enum LVS_EX
        {
            LVS_EX_GRIDLINES = 0x00000001,
            LVS_EX_SUBITEMIMAGES = 0x00000002,
            LVS_EX_CHECKBOXES = 0x00000004,
            LVS_EX_TRACKSELECT = 0x00000008,
            LVS_EX_HEADERDRAGDROP = 0x00000010,
            LVS_EX_FULLROWSELECT = 0x00000020,
            LVS_EX_ONECLICKACTIVATE = 0x00000040,
            LVS_EX_TWOCLICKACTIVATE = 0x00000080,
            LVS_EX_FLATSB = 0x00000100,
            LVS_EX_REGIONAL = 0x00000200,
            LVS_EX_INFOTIP = 0x00000400,
            LVS_EX_UNDERLINEHOT = 0x00000800,
            LVS_EX_UNDERLINECOLD = 0x00001000,
            LVS_EX_MULTIWORKAREAS = 0x00002000,
            LVS_EX_LABELTIP = 0x00004000,
            LVS_EX_BORDERSELECT = 0x00008000,
            LVS_EX_DOUBLEBUFFER = 0x00010000,
            LVS_EX_HIDELABELS = 0x00020000,
            LVS_EX_SINGLEROW = 0x00040000,
            LVS_EX_SNAPTOGRID = 0x00080000,
            LVS_EX_SIMPLESELECT = 0x00100000
        }

        #endregion

        #region LVM

        enum LVM
        {
            LVM_FIRST = 0x1000,
            LVM_SETEXTENDEDLISTVIEWSTYLE = (LVM_FIRST + 54),
            LVM_GETEXTENDEDLISTVIEWSTYLE = (LVM_FIRST + 55),
            LVM_SETTEXTBKCOLOR = 0x1026
        }

        #endregion

        #region TVS_EX

        enum TVS_EX
        {
            TVS_EX_AUTOHSCROLL = 0x0020,
            TVS_EX_FADEINOUTEXPANDOS = 0x0040,
            TVS_EX_DOUBLEBUFFER = 0x0004,
        }

        #endregion

        #region TVM

        enum TVM
        {
            TVM_FIRST = 0x1100,
            TVM_SETEXTENDEDSTYLE = TVM_FIRST + 44,
            TVM_GETEXTENDEDSTYLE = TVM_FIRST + 45,
            TVM_SETAUTOSCROLLINFO = TVM_FIRST + 59
        }

        #endregion

        #region TVS

        enum TVS
        {
            TVS_NOHSCROLL = 0x8000
        }

        #endregion

        #region Local vars

        private static bool _isUxThemeLoad = false;
        private static IntPtr _uxthemeHwnd = IntPtr.Zero;

        #endregion

        #region Win API

        /*
        /// <summary>
        /// Send Message
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32")]
        private static extern bool SendMessage(IntPtr hwnd, 
            uint msg, IntPtr wParam, IntPtr lParam);
        */
        /// <summary>
        /// Send Message
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="messg"></param>
        /// <param name="wparam"></param>
        /// <param name="lparam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr handle,
            int messg, int wparam, int lparam);
        /// <summary>
        /// Load Library
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);
        /// <summary>
        /// Set Window Theme
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="textSubAppName"></param>
        /// <param name="textSubIdList"></param>
        /// <returns></returns>
        [DllImport("uxtheme", CharSet = CharSet.Unicode)]
        private extern static Int32 SetWindowTheme(IntPtr hWnd,
            String textSubAppName, String textSubIdList);

        #endregion

        #endregion

        #region Private Method

        private static void EnableUxTheme()
        {
            if (!_isUxThemeLoad)
            {
                _isUxThemeLoad = true;
                _uxthemeHwnd = LoadLibrary("uxtheme.dll");
            }
        }

        private static void SetListViewExStyles(IntPtr listviewHandle)
        {
            LVS_EX styles = (LVS_EX)SendMessage(listviewHandle,
                (int)LVM.LVM_GETEXTENDEDLISTVIEWSTYLE, 0, 0);
            styles |= LVS_EX.LVS_EX_DOUBLEBUFFER | LVS_EX.LVS_EX_BORDERSELECT;
            SendMessage(listviewHandle,
                (int)LVM.LVM_SETEXTENDEDLISTVIEWSTYLE, 0, (int)styles);
            // Check is lib load
            EnableUxTheme();
            // set uxtheme
            if (_uxthemeHwnd != IntPtr.Zero)
            {
                SetWindowTheme(listviewHandle, "explorer", null);
            }
        }

        private static void SetTreeViewExStyles(IntPtr treeviewHandle)
        {
            int styles = SendMessage(treeviewHandle,
                (int)TVM.TVM_GETEXTENDEDSTYLE, 0, 0);
            styles |= (int)TVS_EX.TVS_EX_DOUBLEBUFFER | (int)TVS_EX.TVS_EX_AUTOHSCROLL;
            SendMessage(treeviewHandle,
                (int)TVM.TVM_SETEXTENDEDSTYLE, 0, styles);
            // Check is lib load
            EnableUxTheme();
            // set uxtheme
            if (_uxthemeHwnd != IntPtr.Zero)
            {
                SetWindowTheme(treeviewHandle, "explorer", null);
            }
        }

        #endregion

        #region Pulic Methods

        /// <summary>
        /// Boost method. Enhance visual style performance for 
        /// control like flicker free, enable theme, etc.
        /// </summary>
        /// <param name="ctrl">The control instance.</param>
        public static void Boost(System.Windows.Forms.Control ctrl)
        {
            if (ctrl == null || ctrl.IsDisposed || ctrl.Disposing)
                return;

            try
            {
                MethodInfo method = ctrl.GetType().GetMethod("SetStyle",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                if (method != null)
                {
                    method.Invoke(ctrl, new object[] 
                        { 
                            ControlStyles.OptimizedDoubleBuffer | 
                            ControlStyles.AllPaintingInWmPaint, true 
                        });

                    if (ctrl is ListView)
                    {
                        SetListViewExStyles(ctrl.Handle);
                    }
                    else if (ctrl is TreeView)
                    {
                        SetTreeViewExStyles(ctrl.Handle);
                    }
                }
            }
            catch { }
        }

        #endregion
    }

    #endregion
}
