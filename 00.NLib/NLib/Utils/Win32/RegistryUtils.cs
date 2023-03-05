#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-17
=================
- RegistryUtils class updated
  - Changes log code (used MethodBase).

======================================================================================================================
Update 2013-03-09
=================
- RegistryUtils class 
  - Add checks code to access ODBC related registry that set by Odbc Admin 32 bits on
    windows 64 bits.

======================================================================================================================
Update 2012-12-27
=================
- RegistryUtils class 
  - Changes log code (used new DebugManager).

======================================================================================================================
Update 2012-01-02
=================
- RegistryUtils class 
  - Remove all using System.Windows.Forms.

======================================================================================================================
Update 2010-01-23
=================
- RegistryUtils class ported from GFA Library GFA37 tor GFA38v3. (Standard/Pro version).
  - Changed all exception handling code to used new ExceptionManager class instread of 
    LogManager.

======================================================================================================================
Update 2008-11-27
=================
- RegistryUtils move from GFA.Lib to GFA.Lib.Core.

======================================================================================================================
Update 2007-11-26
=================
- RegistryUtils add some fixed for multithread environment and optimized
  for performance

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

#endregion

namespace NLib.Utils
{
    #region DSN Mode Enum

    /// <summary>
    /// Dsn Mode for Registry Utils
    /// </summary>
    public enum DsnMode
    {
        /// <summary>
        /// System Dsn
        /// </summary>
        System,
        /// <summary>
        /// User Dsn
        /// </summary>
        User,
        /// <summary>
        /// File Dsn
        /// </summary>
        File
    }

    #endregion

    #region Note for 64 bits

    // On a 64bit OS, if you run "Control Panel/ODBC Data Source Administrator" 
    // you're running %WINDIR%\system32\odbcad32.exe which is 64bit. 
    // Let's say you've created a DSN using this, as long as you ran this 
    // ("Control Panel/ODBC Data Source Administrator" you're running 
    // %WINDIR%\system32\odbcad32.exe which is 64bit), you will see your DSN there
    // But if your application is 32bit, it will never see this DSN created using 64bit odbcad32.exe. 
    // On a64bit OSS, to create a DSN which is going to be seen by 32bit applications you have to 
    // run %WINDIR%\syswow64\odbcad32.exe which is 32bit.
    // Don't forget that your 64bit applications will also not see the 32bit DSNs.
    // If these are not the reasons for your scenario, high likely you're creating too many DSNs 
    // and odbcad32.exe (32bit or 64bit) cannot show it though it's stored in registry 
    // HKEY_LOCAL_MACHINE\SOFTWARE\ODBC\ODBC.INI (If your OS is 64bit and you create a DSN 
    // using 32bit odbcad32.exe, the registry hive will be  
    // HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\ODBC\ODBC.INI)
    //P.S. : Refer to our support.microsoft.com/.../256986 article before making any changes in registry.

    #endregion

    #region Registry Utilities

    /// <summary>
    /// Registry Access Utility
    /// </summary>
    public class RegistryUtils
    {
        #region hide constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private RegistryUtils()
        {
        }

        #endregion

        #region Data Source Utils

        #region Dsn Information

        /// <summary>
        /// Get Avaliable drivers
        /// </summary>
        /// <returns>Returns Avaliable driver list</returns>
        public static string[] GetDrivers()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                RegistryKey reg = Registry.LocalMachine;
                // Try 64 bits first
                RegistryKey subkey =
                    reg.OpenSubKey(@"SOFTWARE\Wow6432Node\ODBC\ODBC.INI\ODBC Drivers", true);
                if (null == subkey)
                {
                    // Try with 32 bits
                    subkey =
                        reg.OpenSubKey(@"SOFTWARE\ODBC\ODBCINST.INI\ODBC Drivers", false);
                }
                if (subkey != null)
                {
                    string[] result = subkey.GetValueNames();
                    subkey.Close();
                    reg.Close();
                    return result;
                }
                else return new string[] { };
            }
            catch (Exception ex)
            {
                ex.Err(med);
                return new string[] { };
            }

        }
        /// <summary>
        /// Get system dsn list
        /// </summary>
        /// <returns>Returns DSN List</returns>
        public static string[] GetSystemDSNs()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                RegistryKey reg = Registry.LocalMachine;

                // Try 64 bits first
                RegistryKey subkey =
                    reg.OpenSubKey(@"SOFTWARE\Wow6432Node\ODBC\ODBC.INI\ODBC Data Sources", false);
                if (null == subkey)
                {
                    // Try 32 bits
                    subkey = reg.OpenSubKey(@"SOFTWARE\ODBC\ODBC.INI\ODBC Data Sources", false);
                }

                if (subkey != null)
                {
                    string[] result = subkey.GetValueNames();
                    subkey.Close();
                    reg.Close();
                    return result;
                }
                else
                {
                    return new string[] { };
                }
            }
            catch (Exception ex)
            {
                ex.Err(med);
                return new string[] { };
            }
        }
        /// <summary>
        /// Get user dsn list
        /// </summary>
        /// <returns>Returns DSN List</returns>
        public static string[] GetUserDSNs()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                RegistryKey reg = Registry.CurrentUser;
                // Try 64 bits first
                RegistryKey subkey =
                    reg.OpenSubKey(@"SOFTWARE\Wow6432Node\ODBC\ODBC.INI\ODBC Data Sources", false);
                if (null == subkey)
                {
                    // Try 32 bits
                    reg.OpenSubKey(@"SOFTWARE\ODBC\ODBC.INI\ODBC Data Sources", false);
                }

                if (subkey != null)
                {
                    string[] result = subkey.GetValueNames();
                    subkey.Close();
                    reg.Close();
                    return result;
                }
                else return new string[] { };
            }
            catch (Exception ex)
            {
                ex.Err(med);
                return new string[] { };
            }
        }
        /// <summary>
        /// Get file dsn list
        /// </summary>
        /// <returns>Returns DSN List</returns>
        public static string[] GetFileDSNs()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                ArrayList list = new ArrayList();
                string[] dirs = GetFileDSNPaths();
                string[] eachs;
                foreach (string name in dirs)
                {
                    eachs = Directory.GetFiles(name);
                    list.AddRange(eachs);
                }
                return (string[])list.ToArray(typeof(string));
            }
            catch (Exception ex)
            {
                ex.Err(med);
                return new string[] { };
            }

        }
        /// <summary>
        /// Get file dsn path list
        /// </summary>
        /// <returns>Returns DSN Path List</returns>
        public static string[] GetFileDSNPaths()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                RegistryKey reg = Registry.LocalMachine;
                // Try 64 bits first
                RegistryKey subkey =
                    reg.OpenSubKey(@"SOFTWARE\Wow6432Node\ODBC\ODBC.INI\ODBC File DSN", false);
                if (null == subkey)
                {
                    // Try 32 bits
                    reg.OpenSubKey(@"SOFTWARE\ODBC\ODBC.INI\ODBC File DSN", false);
                }

                if (subkey != null)
                {
                    string[] results = subkey.GetValueNames();
                    string resultdir;
                    ArrayList list = new ArrayList();
                    foreach (string name in results)
                    {
                        resultdir = subkey.GetValue(name, "").ToString();
                        if (resultdir.Length <= 0) continue;
                        if (!Directory.Exists(resultdir)) continue;
                        list.Add(resultdir);
                    }
                    subkey.Close();
                    reg.Close();
                    return (string[])list.ToArray(typeof(string));
                }
                else return new string[] { };
            }
            catch (Exception ex)
            {
                ex.Err(med);
                return new string[] { };
            }
        }

        #endregion

        #region Text File Dsn

        #endregion

        #region MySQL Dsn

        #endregion

        #region Access Dsn

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
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                RegistryKey reg = null;
                RegistryKey mainkey, subkey, engine, excel;
                switch (mode)
                {
                    case DsnMode.User:
                        reg = Registry.CurrentUser;
                        break;
                    case DsnMode.System:
                        reg = Registry.LocalMachine;
                        break;
                    case DsnMode.File:
                        break;
                }
                if (mode != DsnMode.File)
                {
                    if (reg == null) return;
                    // Create New Entry in ODBC Data Source
                    // Try 64 bits first
                    subkey =
                        reg.OpenSubKey(@"SOFTWARE\Wow6432Node\ODBC\ODBC.INI\ODBC Data Sources", true);
                    if (null == subkey)
                    {
                        // Try 32 bits
                        reg.OpenSubKey(@"SOFTWARE\ODBC\ODBC.INI\ODBC Data Sources", true);
                    }

                    subkey.SetValue(DsnName, "Microsoft Excel Driver (*.xls)");
                    subkey.Flush();
                    subkey.Close();

                    // Create New Section in ODBC.INI Key
                    mainkey =
                        reg.OpenSubKey(@"SOFTWARE\ODBC\ODBC.INI", true);
                    mainkey.Flush();
                    subkey = mainkey.CreateSubKey(DsnName);
                    // Set Value
                    subkey.SetValue("DBQ", ExcelFileName);
                    subkey.SetValue("DefaultDir", Path.GetDirectoryName(ExcelFileName));
                    subkey.SetValue("Driver", Environment.SystemDirectory + @"\odbcjt32.dll");
                    subkey.SetValue("DriverId", 790);
                    subkey.SetValue("FIL", "excel 8.0;");
                    subkey.SetValue("ReadOnly", new byte[] { 1 });
                    subkey.SetValue("SafeTransactions", (int)0);
                    subkey.SetValue("UID", UserName);
                    subkey.Flush();
                    // create subkey engine and excel
                    engine = subkey.CreateSubKey("Engines");
                    engine.Flush();
                    excel = engine.CreateSubKey("Excel");
                    excel.SetValue("FirstRowHasNames", new byte[] { 1 });
                    excel.SetValue("ImplicitCommitSync", "");
                    excel.SetValue("MaxScanRows", (int)8);
                    excel.SetValue("Threads", (int)3);
                    excel.SetValue("UserCommitSync", "Yes");
                    excel.Flush();
                    // close all key
                    excel.Close();
                    engine.Close();
                    mainkey.Close();
                    subkey.Close();
                    // end create
                    reg.Close();
                }
            }
            catch (Exception ex)
            {
                ex.Err(med);
            }
        }

        #endregion

        #endregion
    }

    #endregion
}
