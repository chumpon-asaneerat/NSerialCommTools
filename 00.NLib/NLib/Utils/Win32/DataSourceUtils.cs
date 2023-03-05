#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-03-09
=================
- DataSourceUtils class 
  - DSNMode enum is move to RegistryUtils.

======================================================================================================================
Update 2013-03-09
=================
- DataSourceUtils class 
  - Add methods ODBCAdministrator32 to call ODBC Administrator 32 bits version on 
    windows 64 bits.

======================================================================================================================
Update 2012-01-02
=================
- DataSourceUtils class 
  - Remove all using System.Windows.Forms.

======================================================================================================================
Update 2010-01-23
=================
- DataSourceUtils class ported and related classes from GFA Library GFA37 tor GFA38v3.
  (Standard/Pro version).

======================================================================================================================
Update 2008-11-27
=================
- DataSourceUtils move from GFA.Lib to GFA.Lib.Core.

======================================================================================================================
Update 2007-11-26
=================
- DataSourceUtils add some fixed for multithread environment and optimized
  for performance

======================================================================================================================
// </[History]>

#endif
#endregion

namespace NLib.Utils
{
    #region DataSource Utilities

    /// <summary>
    /// DataSource Utility
    /// </summary>
    public class DataSourceUtils
    {
        #region hide constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private DataSourceUtils()
        {
        }

        #endregion

        #region Dsn Information

        /// <summary>
        /// Get Avaliable drivers
        /// </summary>
        /// <returns>Avaliable driver list</returns>
        public static string[] GetDrivers()
        {
            return RegistryUtils.GetDrivers();
        }
        /// <summary>
        /// get system dsn list
        /// </summary>
        /// <returns>DSN List</returns>
        public static string[] GetSystemDSNs()
        {
            return RegistryUtils.GetSystemDSNs();
        }
        /// <summary>
        /// get user dsn list
        /// </summary>
        /// <returns>DSN List</returns>
        public static string[] GetUserDSNs()
        {
            return RegistryUtils.GetUserDSNs();
        }
        /// <summary>
        /// get file dsn list
        /// </summary>
        /// <returns>DSN List</returns>
        public static string[] GetFileDSNs()
        {
            return RegistryUtils.GetFileDSNs();
        }
        /// <summary>
        /// get file dsn path list
        /// </summary>
        /// <returns>DSN Path List</returns>
        public static string[] GetFileDSNPaths()
        {
            return RegistryUtils.GetFileDSNPaths();
        }

        #endregion

        #region ODBC Administrator Applet

        /// <summary>
        /// Show ODBC Administrator
        /// </summary>
        public static void ODBCAdministrator()
        {
            ControlPanel.ODBCAdministrator();
        }
        /// <summary>
        /// Run Odbc Admin 32 bits In 64bits Systems. 
        /// On a 64bit OS when running %WINDIR%\system32\odbcad32.exe which is 64bit.
        /// But if your application is 32bit, it will never see this DSN created using 64bit odbcad32.exe. 
        /// On a64bit OSS, to create a DSN which is going to be seen by 32bit applications you have to 
        /// run %WINDIR%\syswow64\odbcad32.exe which is 32bit.
        /// Don't forget that your 64bit applications will also not see the 32bit DSNs.
        /// </summary>
        public static bool ODBCAdministrator32()
        {
            string exeName = System.IO.Path.Combine
                (System.Environment.GetFolderPath(System.Environment.SpecialFolder.Windows),
                @"SysWOW64\odbcad32.exe");

            if (!System.IO.File.Exists(exeName))
            {
                // No exe found
                return false;
            }
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            if (null == proc.StartInfo)
            {
                proc.StartInfo = new System.Diagnostics.ProcessStartInfo();
            }

            proc.StartInfo.FileName = exeName;
            proc.StartInfo.UseShellExecute = true;

            bool result = false;
            try
            {
                result = proc.Start();
            }
            catch
            {
                result = false;
            }
            return result;
        }

        #endregion

        #region Excel Dsn

        /// <summary>
        /// Create New Excel DSN
        /// </summary>
        /// <param name="mode">DSN Mode</param>
        /// <param name="DsnName">DSN Name</param>
        /// <param name="ExcelFileName">Excel (*.xls) FileName</param>
        /// <param name="UserName">User Name for connect</param>
        /// <param name="Password">Password for connect</param>
        public static void NewExcelDsn(DsnMode mode, string DsnName, string ExcelFileName, string UserName, string Password)
        {
            RegistryUtils.NewExcelDsn(mode, DsnName, ExcelFileName, UserName, Password);
        }

        #endregion
    }

    #endregion
}
