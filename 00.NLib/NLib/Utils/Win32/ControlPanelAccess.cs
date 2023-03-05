#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-17
=================
- ControlPanelAccess class updated
  - Changes log code (used MethodBase).

======================================================================================================================
Update 2012-12-27
=================
- ControlPanelAccess class 
  - Changes log code (used new DebugManager).

======================================================================================================================
Update 2012-01-02
=================
- ControlPanelAccess class 
  - Remove all using System.Windows.Forms.

======================================================================================================================
Update 2010-01-23
=================
- ControlPanelAccess class ported from GFA Library GFA37 tor GFA38v3. (Standard/Pro version).
  - Changed all exception handling code to used new ExceptionManager class instread of 
    LogManager.

======================================================================================================================
Update 2008-11-27
=================
- ControlPanelAccess move from GFA.Lib to GFA.Lib.Core.

======================================================================================================================
Update 2007-11-26
=================
- ControlPanelAccess add some fixed for multithread environment and optimized
  for performance

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Reflection;

#endregion

namespace NLib.Utils
{
    #region Control Panel Access

    /// <summary>
    /// Control Panel Access Class
    /// </summary>
    public class ControlPanel
    {
        #region Hide Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private ControlPanel()
        {
        }

        #endregion

        #region Execute

        #region Common

        /// <summary>
        /// OpenApplet
        /// </summary>
        /// <param name="AppletName">Applet's Name</param>
        private static void OpenApplet(string AppletName)
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.RedirectStandardOutput = false;
                p.StartInfo.FileName = "CONTROL";
                p.StartInfo.Arguments = AppletName;
                p.StartInfo.UseShellExecute = true;
                p.Start();
                p.Dispose();
            }
            catch (Exception ex)
            {
                "AppletName : {0}".Err(med, AppletName);
                ex.Err(med);
            }
        }

        #endregion

        /// <summary>
        /// Show Display Settings
        /// </summary>
        public static void DisplaySettings()
        {
            DisplaySettings(0);
        }
        /// <summary>
        /// Show Display Settings
        /// </summary>
        /// <param name="TabCode">Tab Code to make Default Page</param>
        public static void DisplaySettings(int TabCode)
        {
            OpenApplet(@"DESK.CPL,," + TabCode.ToString());
        }
        /// <summary>
        /// Show Regional Settings
        /// </summary>
        public static void RegionalSettings()
        {
            OpenApplet(@"INTL.CPL");
        }
        /// <summary>
        /// Show ODBC Administrator
        /// </summary>
        public static void ODBCAdministrator()
        {
            OpenApplet(@"odbccp32.cpl");
        }
        /// <summary>
        /// Show DateTime Settings
        /// </summary>
        public static void DateTimeSettings()
        {
            OpenApplet(@"TIMEDATE.CPL");
        }
        /// <summary>
        /// Show Add Remove Programs
        /// </summary>
        public static void AddRemovePrograms()
        {
            OpenApplet(@"APPWIZ.CPL");
        }
        /// <summary>
        /// Show Keyboard Settings
        /// </summary>
        public static void KeyboardSettings()
        {
            OpenApplet(@"main.cpl @1");
        }
        /// <summary>
        /// Show Mouse Settings
        /// </summary>
        public static void MouseSettings()
        {
            OpenApplet(@"main.cpl @0");
        }
        /// <summary>
        /// Show Internet Options
        /// </summary>
        public static void InternetOptions()
        {
            OpenApplet(@"inetcpl.cpl");
        }
        /// <summary>
        /// Show Modem Options
        /// </summary>
        public static void ModemOptions()
        {
            OpenApplet(@"modem.cpl");
        }
        /// <summary>
        /// Show Telephone Options
        /// </summary>
        public static void TelephoneOptions()
        {
            OpenApplet(@"telephon.cpl");
        }
        /// <summary>
        /// Show Sound Settings
        /// </summary>
        public static void SoundSettings()
        {
            OpenApplet(@"MMSYS.CPL");
        }
        /// <summary>
        /// Show Power Settings
        /// </summary>
        public static void PowerSettings()
        {
            OpenApplet(@"powercfg.CPL");
        }
        /// <summary>
        /// Show System Setting
        /// </summary>
        public static void SystemSetting()
        {
            OpenApplet(@"sysdm.cpl,,0");
        }
        /*
        public static void DeviceManager()
        {
            OpenApplet(@"sysdm.cpl,,1");
        }
        */
        /// <summary>
        /// Show Network Setting
        /// </summary>
        public static void NetworkSetting()
        {
            OpenApplet(@"ncpa.cpl");
            //OpenApplet(@"netcfg.cpl");
        }
        /// <summary>
        /// Show Add New Hardware
        /// </summary>
        public static void AddNewHardware()
        {
            OpenApplet(@"hdwwiz.cpl");
        }


        #endregion
    }

    #endregion
}
