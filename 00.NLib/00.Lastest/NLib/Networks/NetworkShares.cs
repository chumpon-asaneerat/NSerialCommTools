#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-17
=================
- Netwrok framework updated
  - Changes log code (used MethodBase).

======================================================================================================================
Update 2013-08-19
=================
- Netwrok framework.
  - Network shares and Network drive related classes is split to NetworkShares.cs.
  - Add Exception detecion code in some methods.
  - All code need to re-testing.

======================================================================================================================
Update 2010-02-04
=================
- NetwrokUtils class ported from GFA37 to GFA38v3.

======================================================================================================================
Update 2008-09-10
=================
- NetwrokUtils class added.
  - PingService class added. This class is service that handle multithread ping for multiple hosts
    at same time.
  - Ping Manager component added. This is the wrapper component of PingService class.

======================================================================================================================
Update 2008-09-07
=================
- NetwrokUtils class added.
  - GetLocalHostName method added.
  - GetHostAddress method added.

======================================================================================================================
Update 2008-08-24
=================
- NetworkAPI class added. Network API provide wrapper structures, constants and functions 
  in multiple network dlls.
- NetwrokUtils class added.
  - FindNetworkComputers method added.
  - IsOtherComputerConnected static property added. used for check is computer is on network.
  - IsNT and IsW2KUp property added. used for check OS platform type.
- NetworkShare class added. This clas is used for find network share information.
- NetworkDrive class added. This class is used to map/unmap network drive.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Security;

#endregion

namespace NLib.Networks
{
    #region NetworkAPI

    /// <summary>
    /// Network API provide wrapper structures, constants and functions 
    /// in multiple network dlls.
    /// </summary>
    public class NetworkAPI
    {
        #region Constants

        #region for net enumerations

        /// <summary>Maximum path length</summary>
        public const int MAX_PATH = 260;
        /// <summary>No error</summary>
        public const int NO_ERROR = 0;
        /// <summary>Access denied</summary>
        public const int ERROR_ACCESS_DENIED = 5;
        /// <summary>Access denied</summary>
        public const int ERROR_WRONG_LEVEL = 124;
        /// <summary>More data available</summary>
        public const int ERROR_MORE_DATA = 234;
        /// <summary>Not connected</summary>
        public const int ERROR_NOT_CONNECTED = 2250;
        /// <summary>Level 1</summary>
        public const int UNIVERSAL_NAME_INFO_LEVEL = 1;
        /// <summary>Max extries (9x)</summary>
        public const int MAX_SI50_ENTRIES = 20;
        /// <summary>Use Wildcard</summary>
        public const uint USE_WILDCARD = 0xFFFFFFFF;

        #endregion

        #region For drive mapping

        /// <summary>
        /// RESOURCE TYPE DISK
        /// </summary>
        public const int RESOURCETYPE_DISK = 0x1;
        /// <summary>
        /// CONNECT INTERACTIVE
        /// </summary>
        public const int CONNECT_INTERACTIVE = 0x00000008;
        /// <summary>
        /// CONNECT PROMPT
        /// </summary>
        public const int CONNECT_PROMPT = 0x00000010;
        /// <summary>
        /// CONNECT UPDATE PROFILE
        /// </summary>
        public const int CONNECT_UPDATE_PROFILE = 0x00000001;
        /// <summary>
        /// CONNECT REDIRECT - ie4+
        /// </summary>
        public const int CONNECT_REDIRECT = 0x00000080;
        /// <summary>
        /// CONNECT COMMANDLINE- nt5+
        /// </summary>
        public const int CONNECT_COMMANDLINE = 0x00000800;
        /// <summary>
        /// CONNECT CMD SAVECRED - nt5+
        /// </summary>
        public const int CONNECT_CMD_SAVECRED = 0x00001000;

        #endregion

        #endregion

        #region Structs

        #region SERVER_INFO_100

        /// <summary>
        /// Create a SERVER_INFO_100 STRUCTURE
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SERVER_INFO_100
        {
            /// <summary>
            /// Platform ID
            /// </summary>
            public int sv100_platform_id;
            /// <summary>
            /// Name
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string sv100_name;
        }

        #endregion

        #region UNIVERSAL_NAME_INFO

        /// <summary>
        /// UNIVERSAL_NAME_INFO
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct UNIVERSAL_NAME_INFO
        {
            /// <summary>
            /// The universal name field.
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpUniversalName;
        }

        #endregion

        #region SHARE_INFO_2

        /// <summary>
        /// SHARE_INFO_2. Used for Share information, NT, level 2
        /// </summary>
        /// <remarks>
        /// Requires admin rights to work. 
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHARE_INFO_2
        {
            /// <summary>
            /// Net Name field.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string NetName;
            /// <summary>
            /// Share Type field.
            /// </summary>
            public ShareType ShareType;
            /// <summary>
            /// Remark field.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Remark;
            /// <summary>
            /// Permission value field.
            /// </summary>
            public int Permissions;
            /// <summary>
            /// Max users field.
            /// </summary>
            public int MaxUsers;
            /// <summary>
            /// Current user field.
            /// </summary>
            public int CurrentUsers;
            /// <summary>
            /// Path field.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Path;
            /// <summary>
            /// Password field.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Password;
        }

        #endregion

        #region SHARE_INFO_1

        /// <summary>
        /// SHARE_INFO_1. Used for Share information, NT, level 1.
        /// </summary>
        /// <remarks>
        /// Fallback when no admin rights.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHARE_INFO_1
        {
            /// <summary>
            /// Net Name field.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string NetName;
            /// <summary>
            /// Share Type.
            /// </summary>
            public ShareType ShareType;
            /// <summary>
            /// Remark field.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Remark;
        }

        #endregion

        #region SHARE_INFO_50

        /// <summary>
        /// SHARE_INFO_50. Used for Share information, Win9x
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct SHARE_INFO_50
        {
            /// <summary>
            /// Net Name field.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
            public string NetName;
            /// <summary>
            /// Share Type field. (ushort value).
            /// </summary>
            public byte bShareType;
            /// <summary>
            /// Flags fields
            /// </summary>
            public ushort Flags;
            /// <summary>
            /// Remark field.
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string Remark;
            /// <summary>
            /// Path field.
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string Path;
            /// <summary>
            /// Password Read/Write field.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
            public string PasswordRW;
            /// <summary>
            /// Password Read only field.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
            public string PasswordRO;
            /// <summary>
            /// Get Share Type.
            /// </summary>
            public ShareType ShareType
            {
                get { return (ShareType)((int)bShareType & 0x7F); }
            }
        }

        #endregion

        #region SHARE_INFO_1_9x

        /// <summary>
        /// SHARE_INFO_1_9x. Used for Share information level 1, Win9x
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct SHARE_INFO_1_9x
        {
            /// <summary>
            /// Net Name field.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
            public string NetName;
            /// <summary>
            /// Padding field.
            /// </summary>
            public byte Padding;
            /// <summary>
            /// Share Type field. (ushort value).
            /// </summary>
            public ushort bShareType;
            /// <summary>
            /// Remark field.
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string Remark;
            /// <summary>
            /// Get Share Type.
            /// </summary>
            public ShareType ShareType
            {
                get { return (ShareType)((int)bShareType & 0x7FFF); }
            }
        }

        #endregion

        #region NET_RESOURCE

        /// <summary>
        /// NET_RESOURCE structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_RESOURCE
        {
            /// <summary>
            /// Scope field.
            /// </summary>
            public int Scope;
            /// <summary>
            /// Type  field.
            /// </summary>
            public int Type;
            /// <summary>
            /// Display Type field.
            /// </summary>
            public int DisplayType;
            /// <summary>
            /// Usage field.
            /// </summary>
            public int Usage;
            /// <summary>
            /// Local Drive field.
            /// </summary>
            public string LocalDrive;
            /// <summary>
            /// Remote Name field.
            /// </summary>
            public string RemoteName;
            /// <summary>
            /// Comment field.
            /// </summary>
            public string Comment;
            /// <summary>
            /// Provider field.
            /// </summary>
            public string Provider;
        }

        #endregion

        #endregion

        #region API

        #region Netapi32

        #region NetServerEnum

        /// <summary>
        /// Netapi32.dll : The NetServerEnum function lists all servers
        /// of the specified type that are visible in a domain. For example, an
        /// application can call NetServerEnum to list all domain controllers only
        /// or all SQL servers only.
        /// You can combine bit masks to list several types. For example, a value
        /// of 0x00000003 combines the bit masks for SV_TYPE_WORKSTATION
        /// (0x00000001) and SV_TYPE_SERVER (0x00000002)
        /// </summary>
        /// <param name="ServerNane">See MSDN.</param>
        /// <param name="dwLevel">See MSDN.</param>
        /// <param name="pBuf">See MSDN.</param>
        /// <param name="dwPrefMaxLen">See MSDN.</param>
        /// <param name="dwEntriesRead">See MSDN.</param>
        /// <param name="dwTotalEntries">See MSDN.</param>
        /// <param name="dwServerType">See MSDN.</param>
        /// <param name="domain">See MSDN.</param>
        /// <param name="dwResumeHandle">See MSDN.</param>
        /// <returns></returns>
        [DllImport("Netapi32", CharSet = CharSet.Auto, SetLastError = true),
        SuppressUnmanagedCodeSecurityAttribute]
        public static extern int NetServerEnum(string ServerNane, int dwLevel, ref IntPtr pBuf, int dwPrefMaxLen, out int dwEntriesRead,
            out int dwTotalEntries, int dwServerType, string domain, out int dwResumeHandle);

        #endregion

        #region NetApiBufferFree

        /// <summary>
        /// Netapi32.dll : The NetApiBufferFree function frees
        /// the memory that the NetApiBufferAllocate function allocates.
        /// Call NetApiBufferFree to free the memory that other network
        /// management functions return.
        /// </summary>
        /// <param name="pBuf">The pointer for buffer.</param>
        /// <returns>See return code from MSDN.</returns>
        [DllImport("Netapi32", SetLastError = true),
        SuppressUnmanagedCodeSecurityAttribute]
        public static extern int NetApiBufferFree(IntPtr pBuf);

        #endregion

        #region NetShareEnum

        /// <summary>
        /// Enumerate shares (NT).
        /// </summary>
        /// <param name="lpServerName">See MSDN.</param>
        /// <param name="dwLevel">See MSDN.</param>
        /// <param name="lpBuffer">See MSDN.</param>
        /// <param name="dwPrefMaxLen">See MSDN.</param>
        /// <param name="entriesRead">See MSDN.</param>
        /// <param name="totalEntries">See MSDN.</param>
        /// <param name="hResume">See MSDN.</param>
        /// <returns>See MSDN.</returns>
        [DllImport("netapi32", CharSet = CharSet.Unicode)]
        public static extern int NetShareEnum(string lpServerName, int dwLevel,
            out IntPtr lpBuffer, int dwPrefMaxLen, out int entriesRead,
            out int totalEntries, ref int hResume);
        /// <summary>
        /// Enumerate shares (9x).
        /// </summary>
        /// <param name="lpServerName">See MSDN.</param>
        /// <param name="dwLevel">See MSDN.</param>
        /// <param name="lpBuffer">See MSDN.</param>
        /// <param name="cbBuffer">See MSDN.</param>
        /// <param name="entriesRead">See MSDN.</param>
        /// <param name="totalEntries">See MSDN.</param>
        /// <returns>See MSDN.</returns>
        [DllImport("svrapi", CharSet = CharSet.Ansi)]
        public static extern int NetShareEnum(
            [MarshalAs(UnmanagedType.LPTStr)] string lpServerName, int dwLevel,
            IntPtr lpBuffer, ushort cbBuffer, out ushort entriesRead,
            out ushort totalEntries);

        #endregion

        #endregion

        #region MPR API - WNetXXX functions

        #region WNetGetUniversalName

        /// <summary>
        /// Get a UNC name.
        /// </summary>
        /// <param name="lpLocalPath">See MSDN.</param>
        /// <param name="dwInfoLevel">See MSDN.</param>
        /// <param name="lpBuffer">See MSDN.</param>
        /// <param name="lpBufferSize">See MSDN.</param>
        /// <returns></returns>
        [DllImport("mpr", CharSet = CharSet.Auto)]
        public static extern int WNetGetUniversalName(string lpLocalPath,
            int dwInfoLevel, ref UNIVERSAL_NAME_INFO lpBuffer, ref int lpBufferSize);
        /// <summary>
        /// Get a UNC name.
        /// </summary>
        /// <param name="lpLocalPath">See MSDN.</param>
        /// <param name="dwInfoLevel">See MSDN.</param>
        /// <param name="lpBuffer">See MSDN.</param>
        /// <param name="lpBufferSize">See MSDN.</param>
        /// <returns>See MSDN.</returns>
        [DllImport("mpr", CharSet = CharSet.Auto)]
        public static extern int WNetGetUniversalName(string lpLocalPath,
            int dwInfoLevel, IntPtr lpBuffer, ref int lpBufferSize);

        #endregion

        #region WNetAddConnection/WNetCancelConnection/WNetRestoreConnection

        /// <summary>
        /// WNetAddConnection
        /// </summary>
        /// <param name="netResStruct">See MSDN.</param>
        /// <param name="password">See MSDN.</param>
        /// <param name="username">See MSDN.</param>
        /// <param name="flags">See MSDN.</param>
        /// <returns>See MSDN.</returns>
        [DllImport("mpr.dll", EntryPoint = "WNetAddConnection2A", CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        public static extern int WNetAddConnection(ref NET_RESOURCE netResStruct, string password, string username, int flags);
        /// <summary>
        /// WNetCancelConnection
        /// </summary>
        /// <param name="name">See MSDN.</param>
        /// <param name="flags">See MSDN.</param>
        /// <param name="force">See MSDN.</param>
        /// <returns>See MSDN.</returns>
        [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2A", CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        public static extern int WNetCancelConnection(string name, int flags, int force);
        /// <summary>
        /// WNetRestoreConnection
        /// </summary>
        /// <param name="hWnd">See MSDN.</param>
        /// <param name="localDrive">See MSDN.</param>
        /// <returns>See MSDN.</returns>
        [DllImport("mpr.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        public static extern int WNetRestoreConnection(int hWnd, string localDrive);

        #endregion

        #region WNetConnectionDialog/WNetDisconnectDialog

        /// <summary>
        /// WNetConnectionDialog
        /// </summary>
        /// <param name="hWnd">See MSDN.</param>
        /// <param name="type">See MSDN.</param>
        /// <returns>See MSDN.</returns>
        [DllImport("mpr.dll", EntryPoint = "WNetConnectionDialog", SetLastError = true)]
        public static extern int WNetConnectionDialog(int hWnd, int type);
        /// <summary>
        /// WNetDisconnectDialog
        /// </summary>
        /// <param name="hWnd">See MSDN.</param>
        /// <param name="type">See MSDN.</param>
        /// <returns>See MSDN.</returns>
        [DllImport("mpr.dll", EntryPoint = "WNetDisconnectDialog", SetLastError = true)]
        public static extern int WNetDisconnectDialog(int hWnd, int type);

        #endregion

        #region WNetGetConnection

        /// <summary>
        /// WNetGetConnection
        /// </summary>
        /// <param name="localDrive">See MSDN.</param>
        /// <param name="remoteName">See MSDN.</param>
        /// <param name="bufferLength">See MSDN.</param>
        /// <returns></returns>
        [DllImport("mpr.dll", EntryPoint = "WNetGetConnection", SetLastError = true)]
        public static extern int WNetGetConnection(string localDrive, byte[] remoteName, ref int bufferLength);

        #endregion

        #endregion

        #region for network assess (related function used with WNet API)

        /// <summary>
        /// PathIsNetworkPath
        /// </summary>
        /// <param name="localDrive">See MSDN.</param>
        /// <returns>See MSDN.</returns>
        [DllImport("shlwapi.dll", EntryPoint = "PathIsNetworkPath", SetLastError = true)]
        public static extern bool PathIsNetworkPath(string localDrive);
        /// <summary>
        /// GetDriveType
        /// </summary>
        /// <param name="localDrive">See MSDN.</param>
        /// <returns>See MSDN.</returns>
        [DllImport("kernel32.dll", EntryPoint = "GetDriveType", SetLastError = true)]
        public static extern int GetDriveType(string localDrive);

        #endregion

        #endregion
    }

    #endregion

    #region Share Type

    /// <summary>
    /// Type of share
    /// </summary>
    [Flags]
    public enum ShareType
    {
        /// <summary>Disk share</summary>
        Disk = 0,
        /// <summary>Printer share</summary>
        Printer = 1,
        /// <summary>Device share</summary>
        Device = 2,
        /// <summary>IPC share</summary>
        IPC = 3,
        /// <summary>Special share</summary>
        Special = -2147483648, // 0x80000000,
    }

    #endregion

    #region Network shares

    #region Network Share

    /// <summary>
    /// Information about a local share
    /// </summary>
    public class NetworkShare
    {
        #region Internal Variables

        private string _server;
        private string _netName;
        private string _path;
        private ShareType _shareType;
        private string _remark;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="server">The name of the computer that this share belongs to.</param>
        /// <param name="netName">The share's name that used for display on network.</param>
        /// <param name="path">The local path.</param>
        /// <param name="shareType">Resource share type.</param>
        /// <param name="remark">The Remark string.</param>
        public NetworkShare(string server, string netName, string path,
            ShareType shareType, string remark)
        {
            if (ShareType.Special == shareType && "IPC$" == netName)
            {
                shareType |= ShareType.IPC;
            }

            _server = server;
            _netName = netName;
            _path = path;
            _shareType = shareType;
            _remark = remark;
        }

        #endregion

        #region Overrides

        #region ToString

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>Return string that represent path to this share.</returns>
        public override string ToString()
        {
            if (null == _server || 0 == _server.Length)
            {
                return string.Format(@"\\{0}\{1}", Environment.MachineName, _netName);
            }
            else
                return string.Format(@"\\{0}\{1}", _server, _netName);
        }

        #endregion

        #endregion

        #region Public Method

        /// <summary>
        /// Returns true if this share matches the local path
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>Return true if the specificed path is match with this share.</returns>
        public bool MatchesPath(string path)
        {
            if (!IsFileSystem)
                return false;
            if (null == path || 0 == path.Length)
                return false;

            return path.ToLower().StartsWith(_path.ToLower());
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the computer that this share belongs to
        /// </summary>
        public string Server { get { return _server; } }
        /// <summary>
        /// Get Share name
        /// </summary>
        public string NetName { get { return _netName; } }
        /// <summary>
        /// Get Local path
        /// </summary>
        public string Path { get { return _path; } }
        /// <summary>
        /// Get Share type
        /// </summary>
        public ShareType ShareType { get { return _shareType; } }
        /// <summary>
        /// Get Remark or Comment.
        /// </summary>
        public string Remark { get { return _remark; } }
        /// <summary>
        /// Returns true if this is a file system share
        /// </summary>
        public bool IsFileSystem
        {
            get
            {
                // Shared device
                if (0 != (_shareType & ShareType.Device)) return false;
                // IPC share
                if (0 != (_shareType & ShareType.IPC)) return false;
                // Shared printer
                if (0 != (_shareType & ShareType.Printer)) return false;

                // Standard disk share
                if (0 == (_shareType & ShareType.Special)) return true;

                // Special disk share (e.g. C$)
                if (ShareType.Special == _shareType && null != _netName && 0 != _netName.Length)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// Get the root of a disk-based share
        /// </summary>
        public DirectoryInfo Root
        {
            get
            {
                if (IsFileSystem)
                {
                    if (null == _server || 0 == _server.Length)
                        if (null == _path || 0 == _path.Length)
                            return new DirectoryInfo(ToString());
                        else
                            return new DirectoryInfo(_path);
                    else
                        return new DirectoryInfo(ToString());
                }
                else
                    return null;
            }
        }

        #endregion

        #region Static methods

        #region Platform

        /// <summary>
        /// Is this an NT platform?
        /// </summary>
        private static bool IsNT
        {
            get { return (PlatformID.Win32NT == Environment.OSVersion.Platform); }
        }

        /// <summary>
        /// Returns true if this is Windows 2000 or higher
        /// </summary>
        private static bool IsW2KUp
        {
            get
            {
                OperatingSystem os = Environment.OSVersion;
                if (PlatformID.Win32NT == os.Platform && os.Version.Major >= 5)
                    return true;
                else
                    return false;
            }
        }

        #endregion

        #region Enumerate shares

        /// <summary>
        /// Enumerates the shares on Windows NT
        /// </summary>
        /// <param name="server">The server name</param>
        /// <param name="shares">The NetworkShareCollection</param>
        internal static void EnumerateSharesNT(string server, NetworkShareCollection shares)
        {
            int level = 2;
            int entriesRead, totalEntries, nRet, hResume = 0;
            IntPtr pBuffer = IntPtr.Zero;

            try
            {
                nRet = NetworkAPI.NetShareEnum(server, level, out pBuffer, -1,
                    out entriesRead, out totalEntries, ref hResume);

                if (NetworkAPI.ERROR_ACCESS_DENIED == nRet)
                {
                    // Need admin for level 2, drop to level 1
                    level = 1;
                    nRet = NetworkAPI.NetShareEnum(server, level, out pBuffer, -1,
                        out entriesRead, out totalEntries, ref hResume);
                }

                if (NetworkAPI.NO_ERROR == nRet && entriesRead > 0)
                {
                    Type t = (2 == level) ? typeof(NetworkAPI.SHARE_INFO_2) : typeof(NetworkAPI.SHARE_INFO_1);
                    int offset = Marshal.SizeOf(t);

                    for (int i = 0, lpItem = pBuffer.ToInt32(); i < entriesRead; i++, lpItem += offset)
                    {
                        IntPtr pItem = new IntPtr(lpItem);
                        if (1 == level)
                        {
                            NetworkAPI.SHARE_INFO_1 si = (NetworkAPI.SHARE_INFO_1)Marshal.PtrToStructure(pItem, t);
                            shares.Add(si.NetName, string.Empty, si.ShareType, si.Remark);
                        }
                        else
                        {
                            NetworkAPI.SHARE_INFO_2 si = (NetworkAPI.SHARE_INFO_2)Marshal.PtrToStructure(pItem, t);
                            shares.Add(si.NetName, si.Path, si.ShareType, si.Remark);
                        }
                    }
                }
                else if (NetworkAPI.ERROR_ACCESS_DENIED == nRet)
                {
                    // access denine again so may be need to log in.
                }
            }
            catch (Exception ex)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                ex.Err(med);
            }
            finally
            {
                // Clean up buffer allocated by system
                if (IntPtr.Zero != pBuffer)
                    NetworkAPI.NetApiBufferFree(pBuffer);
            }
        }
        /// <summary>
        /// Enumerates the shares on Windows 9x
        /// </summary>
        /// <param name="server">The server name</param>
        /// <param name="shares">The NetworkShareCollection</param>
        internal static void EnumerateShares9x(string server, NetworkShareCollection shares)
        {
            int level = 50;
            int nRet = 0;
            ushort entriesRead, totalEntries;

            Type t = typeof(NetworkAPI.SHARE_INFO_50);
            int size = Marshal.SizeOf(t);
            ushort cbBuffer = (ushort)(NetworkAPI.MAX_SI50_ENTRIES * size);
            //On Win9x, must allocate buffer before calling API
            IntPtr pBuffer = Marshal.AllocHGlobal(cbBuffer);

            try
            {
                nRet = NetworkAPI.NetShareEnum(server, level, pBuffer, cbBuffer,
                    out entriesRead, out totalEntries);

                if (NetworkAPI.ERROR_WRONG_LEVEL == nRet)
                {
                    level = 1;
                    t = typeof(NetworkAPI.SHARE_INFO_1_9x);
                    size = Marshal.SizeOf(t);

                    nRet = NetworkAPI.NetShareEnum(server, level, pBuffer, cbBuffer,
                        out entriesRead, out totalEntries);
                }

                if (NetworkAPI.NO_ERROR == nRet || NetworkAPI.ERROR_MORE_DATA == nRet)
                {
                    for (int i = 0, lpItem = pBuffer.ToInt32(); i < entriesRead; i++, lpItem += size)
                    {
                        IntPtr pItem = new IntPtr(lpItem);

                        if (1 == level)
                        {
                            NetworkAPI.SHARE_INFO_1_9x si = (NetworkAPI.SHARE_INFO_1_9x)Marshal.PtrToStructure(pItem, t);
                            shares.Add(si.NetName, string.Empty, si.ShareType, si.Remark);
                        }
                        else
                        {
                            NetworkAPI.SHARE_INFO_50 si = (NetworkAPI.SHARE_INFO_50)Marshal.PtrToStructure(pItem, t);
                            shares.Add(si.NetName, si.Path, si.ShareType, si.Remark);
                        }
                    }
                }
                else
                {
                    //Console.WriteLine(nRet);
                }

            }
            catch (Exception ex)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                ex.Err(med);
            }
            finally
            {
                //Clean up buffer
                Marshal.FreeHGlobal(pBuffer);
            }
        }
        /// <summary>
        /// Enumerates the shares
        /// </summary>
        /// <param name="server">The server name</param>
        /// <param name="shares">The ShareCollection</param>
        internal static void EnumerateShares(string server, NetworkShareCollection shares)
        {
            if (null != server && 0 != server.Length && !NetworkShare.IsW2KUp)
            {
                server = server.ToUpper();

                // On NT4, 9x and Me, server has to start with "\\"
                if (!('\\' == server[0] && '\\' == server[1]))
                    server = @"\\" + server;
            }

            if (NetworkShare.IsNT)
                EnumerateSharesNT(server, shares);
            else
                EnumerateShares9x(server, shares);
        }

        #endregion

        #region Static Access

        /// <summary>
        /// Return the local shares
        /// </summary>
        public static NetworkShareCollection LocalShares
        {
            get
            {
                return new NetworkShareCollection();
            }
        }
        /// <summary>
        /// Returns true if fileName is a valid local file-name of the form:
        /// X:\, where X is a drive letter from A-Z
        /// </summary>
        /// <param name="fileName">The filename to check</param>
        /// <returns></returns>
        public static bool IsValidFilePath(string fileName)
        {
            if (null == fileName || 3 > fileName.Length) return false;

            char drive = char.ToUpper(fileName[0]);
            if ('A' > drive || drive > 'Z')
                return false;

            else if (System.IO.Path.VolumeSeparatorChar != fileName[1])
                return false;
            else if (System.IO.Path.DirectorySeparatorChar != fileName[2])
                return false;
            else
                return true;
        }
        /// <summary>
        /// Returns the UNC path for a mapped drive or local share.
        /// </summary>
        /// <param name="fileName">The path to map</param>
        /// <returns>The UNC path (if available)</returns>
        public static string PathToUnc(string fileName)
        {
            if (null == fileName || 0 == fileName.Length) return string.Empty;

            fileName = System.IO.Path.GetFullPath(fileName);
            if (!IsValidFilePath(fileName)) return fileName;

            int nRet = 0;
            NetworkAPI.UNIVERSAL_NAME_INFO rni = new NetworkAPI.UNIVERSAL_NAME_INFO();
            int bufferSize = Marshal.SizeOf(rni);

            nRet = NetworkAPI.WNetGetUniversalName(
                fileName, NetworkAPI.UNIVERSAL_NAME_INFO_LEVEL,
                ref rni, ref bufferSize);

            if (NetworkAPI.ERROR_MORE_DATA == nRet)
            {
                IntPtr pBuffer = Marshal.AllocHGlobal(bufferSize); ;
                try
                {
                    nRet = NetworkAPI.WNetGetUniversalName(
                        fileName, NetworkAPI.UNIVERSAL_NAME_INFO_LEVEL,
                        pBuffer, ref bufferSize);

                    if (NetworkAPI.NO_ERROR == nRet)
                    {
                        rni = (NetworkAPI.UNIVERSAL_NAME_INFO)Marshal.PtrToStructure(pBuffer,
                            typeof(NetworkAPI.UNIVERSAL_NAME_INFO));
                    }
                }
                catch (Exception ex)
                {
                    MethodBase med = MethodBase.GetCurrentMethod();
                    ex.Err(med);
                }
                finally
                {
                    Marshal.FreeHGlobal(pBuffer);
                }
            }

            switch (nRet)
            {
                case NetworkAPI.NO_ERROR:
                    return rni.lpUniversalName;

                case NetworkAPI.ERROR_NOT_CONNECTED:
                    //Local file-name
                    NetworkShareCollection shi = LocalShares;
                    if (null != shi)
                    {
                        NetworkShare share = shi[fileName];
                        if (null != share)
                        {
                            string path = share.Path;
                            if (null != path && 0 != path.Length)
                            {
                                int index = path.Length;
                                if (System.IO.Path.DirectorySeparatorChar != path[path.Length - 1])
                                    index++;

                                if (index < fileName.Length)
                                    fileName = fileName.Substring(index);
                                else
                                    fileName = string.Empty;

                                fileName = System.IO.Path.Combine(share.ToString(), fileName);
                            }
                        }
                    }
                    return fileName;
                default:
                    //Console.WriteLine("Unknown return value: {0}", nRet);
                    return string.Empty;
            }
        }
        /// <summary>
        /// Returns the local <see cref="NetworkShare"/> object with the best match
        /// to the specified path.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static NetworkShare PathToShare(string fileName)
        {
            if (null == fileName || 0 == fileName.Length) return null;

            fileName = System.IO.Path.GetFullPath(fileName);
            if (!IsValidFilePath(fileName)) return null;

            NetworkShareCollection shi = LocalShares;
            if (null == shi)
                return null;
            else
                return shi[fileName];
        }
        /// <summary>
        /// Return the shares for a specified machine
        /// </summary>
        /// <param name="server">The server to get share.</param>
        /// <returns>Return collection of share of specificed server.</returns>
        public static NetworkShareCollection GetShares(string server)
        {
            return new NetworkShareCollection(server);
        }

        #endregion

        #endregion
    }

    #endregion

    #region Network Share Collection

    /// <summary>
    /// A collection of network shares
    /// </summary>
    public class NetworkShareCollection : ReadOnlyCollectionBase
    {
        #region Internal Variables

        /// <summary>The name of the server this collection represents</summary>
        private string _server;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor - local machine
        /// </summary>
        internal NetworkShareCollection()
        {
            _server = string.Empty;
            NetworkShare.EnumerateShares(_server, this);
        }
        /// <summary>
        /// Constructor - remote or server machine
        /// </summary>
        /// <param name="server">The server name.</param>
        public NetworkShareCollection(string server)
        {
            _server = server;
            NetworkShare.EnumerateShares(_server, this);
        }

        #endregion

        #region Add

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="share">The network share instance.</param>
        internal void Add(NetworkShare share)
        {
            InnerList.Add(share);
        }
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="netName">The Net Name.</param>
        /// <param name="path">The Path.</param>
        /// <param name="shareType">The share value.</param>
        /// <param name="remark">The remark value.</param>
        internal void Add(string netName, string path, ShareType shareType, string remark)
        {
            InnerList.Add(new NetworkShare(_server, netName, path, shareType, remark));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the name of the server this collection represents
        /// </summary>
        public string Server { get { return _server; } }
        /// <summary>
        /// Returns the <see cref="NetworkShare"/> at the specified index.
        /// </summary>
        public NetworkShare this[int index] { get { return (NetworkShare)InnerList[index]; } }
        /// <summary>
        /// Returns the <see cref="NetworkShare"/> which matches a given local path
        /// </summary>
        /// <param name="path">The path to match</param>
        public NetworkShare this[string path]
        {
            get
            {
                if (null == path || 0 == path.Length) return null;

                path = System.IO.Path.GetFullPath(path);
                if (!NetworkShare.IsValidFilePath(path)) return null;

                NetworkShare match = null;

                for (int i = 0; i < InnerList.Count; i++)
                {
                    NetworkShare s = (NetworkShare)InnerList[i];

                    if (s.IsFileSystem && s.MatchesPath(path))
                    {
                        //Store first match
                        if (null == match)
                            match = s;

                        // If this has a longer path,
                        // and this is a disk share or match is a special share, 
                        // then this is a better match
                        else if (match.Path.Length < s.Path.Length)
                        {
                            if (ShareType.Disk == s.ShareType || ShareType.Disk != match.ShareType)
                                match = s;
                        }
                    }
                }

                return match;
            }
        }

        #endregion

        #region Implementation of ICollection

        /// <summary>
        /// Copy this collection to an array
        /// </summary>
        /// <param name="array">The sorce share array.</param>
        /// <param name="index">The array index to begin copy.</param>
        public void CopyTo(NetworkShare[] array, int index)
        {
            InnerList.CopyTo(array, index);
        }

        #endregion
    }

    #endregion

    #endregion

    #region Network Drive

    /// <summary>
    /// Network Drive Mapping class. Used for Map, ummap and provide general functions 
    /// for network drives.
    /// </summary>
    public sealed class NetworkDrive
    {
        #region Internal Variable

        private bool _saveCredentials = false;
        private bool _persistent = false;
        private bool _force = false;
        private bool _promptForCredentials = false;
        private bool _findNextFreeDrive = false;
        private string _localDrive = null;
        private string _shareName = "";

        #endregion

        #region Private Methods

        /// <summary>
        /// Map network drive
        /// </summary>
        /// <param name="username">User Name</param>
        /// <param name="password">Password</param>
        private void InternalMapDrive(string username, string password)
        {
            // if drive property is set to auto select, collect next free drive			
            if (_findNextFreeDrive)
            {
                _localDrive = InternalNextFreeDrive();
                if (_localDrive == null || _localDrive.Length == 0)
                    throw new Exception("Could not find valid free drive name");
            }
            // create struct data to pass to the api function
            NetworkAPI.NET_RESOURCE stNetRes = new NetworkAPI.NET_RESOURCE();
            stNetRes.Scope = 2;
            stNetRes.Type = NetworkAPI.RESOURCETYPE_DISK;
            stNetRes.DisplayType = 3;
            stNetRes.Usage = 1;
            stNetRes.RemoteName = _shareName;
            stNetRes.LocalDrive = _localDrive;

            // prepare flags for drive mapping options
            int iFlags = 0;
            if (_saveCredentials)
                iFlags += NetworkAPI.CONNECT_CMD_SAVECRED;
            if (_persistent)
                iFlags += NetworkAPI.CONNECT_UPDATE_PROFILE;
            if (_promptForCredentials)
                iFlags += NetworkAPI.CONNECT_INTERACTIVE + NetworkAPI.CONNECT_PROMPT;

            // prepare username / password params
            if (username != null && username.Length == 0)
                username = null;
            if (password != null && password.Length == 0)
                password = null;

            // if force, unmap ready for new connection
            if (_force)
            {
                try
                {
                    this.InternalUnMapDrive();
                }
                catch
                {
                }
            }

            // call and return
            int i = NetworkAPI.WNetAddConnection(ref stNetRes, password, username, iFlags);
            if (i > 0)
                throw new Win32Exception(i);

        }
        /// <summary>
        /// Unmap network drive	
        /// </summary>
        private void InternalUnMapDrive()
        {
            // prep vars and call unmap
            int iFlags = 0;
            int iRet = 0;

            // if persistent, set flag
            if (_persistent)
            {
                iFlags += NetworkAPI.CONNECT_UPDATE_PROFILE;
            }

            // if local drive is null, unmap with use connection
            if (_localDrive == null)
            {
                // unmap use connection, passing the share name, as local drive
                iRet = NetworkAPI.WNetCancelConnection(_shareName, iFlags,
                    Convert.ToInt32(_force));
            }
            else
            {
                // unmap drive
                iRet = NetworkAPI.WNetCancelConnection(_localDrive, iFlags,
                    Convert.ToInt32(_force));
            }

            // if errors, throw exception
            if (iRet > 0)
                throw new Win32Exception(iRet);

        }
        /// <summary>
        /// check / restore a network drive
        /// </summary>
        /// <param name="driveName">Drive's Name.</param>
        private void InternalRestoreDrive(string driveName)
        {
            // call restore and return
            int i = NetworkAPI.WNetRestoreConnection(0, driveName);
            // if error returned, throw
            if (i > 0)
                throw new Win32Exception(i);
        }
        /// <summary>
        /// Display windows dialog
        /// </summary>
        /// <param name="wndHandle">The window handle.</param>
        /// <param name="dialogToShow">Show flag.</param>
        private void InternalDisplayDialog(System.IntPtr wndHandle, int dialogToShow)
        {
            // prep variables
            int i = -1;
            int iHandle = 0;

            // get parent handle
            if (wndHandle != System.IntPtr.Zero)
                iHandle = wndHandle.ToInt32();

            // choose dialog to show bassed on 
            if (dialogToShow == 1)
                i = NetworkAPI.WNetConnectionDialog(iHandle,
                    NetworkAPI.RESOURCETYPE_DISK);
            else if (dialogToShow == 2)
                i = NetworkAPI.WNetDisconnectDialog(iHandle,
                    NetworkAPI.RESOURCETYPE_DISK);

            // if error returned, throw
            if (i > 0)
                throw new Win32Exception(i);

        }
        /// <summary>
        /// Find Next Free Drive letter.
        /// </summary>
        /// <returns>Returns the next viable drive name  (with colon) to use for mapping</returns>
        private string InternalNextFreeDrive()
        {
            // loop from c to z and check that drive is free
            string retValue = null;
            for (int i = 67; i <= 90; i++)
            {
                if (NetworkAPI.GetDriveType(((char)i).ToString() + ":") == 1)
                {
                    retValue = ((char)i).ToString() + ":";
                    break;
                }
            }
            // return selected drive
            return retValue;
        }

        #endregion

        #region Public Methods

        #region Map Drive

        /// <summary>
        /// Map network drive
        /// </summary>
        public void MapDrive()
        {
            InternalMapDrive(null, null);
        }
        /// <summary>
        /// Map network drive (using supplied Username and Password)
        /// </summary>
        /// <param name="username">Username passed for permissions / credintals ('Username' may be passed as null, to map using only a password)</param>
        /// <param name="password">Password passed for permissions / credintals</param>
        public void MapDrive(string username, string password)
        {
            InternalMapDrive(username, password);
        }
        /// <summary>
        /// Set common propertys, then map the network drive
        /// </summary>
        /// <param name="localDrive">LocalDrive to use for connection</param>
        /// <param name="shareName">Share name for the connection (eg. '\\Computer\Share')</param>
        /// <param name="force">Option to force dis/connection</param>
        public void MapDrive(string localDrive, string shareName, bool force)
        {
            _localDrive = localDrive;
            _shareName = shareName;
            _force = force;
            InternalMapDrive(null, null);
        }
        /// <summary>
        /// Set common propertys, then map the network drive
        /// </summary>
        /// <param name="localDrive">Password passed for permissions / credintals</param>
        /// <param name="force">Option to force dis/connection</param>
        public void MapDrive(string localDrive, bool force)
        {
            _localDrive = localDrive;
            _force = force;
            InternalMapDrive(null, null);
        }

        #endregion

        #region Unmap Drive

        /// <summary>
        /// Unmap network drive
        /// </summary>
        public void UnMapDrive()
        {
            InternalUnMapDrive();
        }
        /// <summary>
        /// Unmap network drive
        /// </summary>
        /// <param name="localDrive">The local drive to unmap.</param>
        public void UnMapDrive(string localDrive)
        {
            _localDrive = localDrive;
            InternalUnMapDrive();
        }
        /// <summary>
        /// Unmap network drive
        /// </summary>
        /// <param name="localDrive">The local drive to unmap.</param>
        /// <param name="force">Option to force dis/connection</param>
        public void UnMapDrive(string localDrive, bool force)
        {
            _localDrive = localDrive;
            _force = force;
            InternalUnMapDrive();
        }

        #endregion

        #region Restore Drives

        /// <summary>
        /// Check / restore persistent network drive
        /// </summary>
        public void RestoreDrives()
        {
            InternalRestoreDrive(null);
        }
        /// <summary>
        /// Check / restore persistent network drive
        /// </summary>
        /// <param name="localDrive">The local drive's name.</param>
        public void RestoreDrive(string localDrive)
        {
            InternalRestoreDrive(localDrive);
        }

        #endregion

        #region Show Connect/Disconnect Dialog

        /// <summary>
        /// Display windows dialog for mapping a network drive (using Desktop as parent form)
        /// </summary>		
        public void ShowConnectDialog()
        {
            InternalDisplayDialog(System.IntPtr.Zero, 1);
        }
        /// <summary>
        /// Display windows dialog for mapping a network drive
        /// </summary>
        /// <param name="parentFormHandle">Form used as a parent for the dialog</param>
        public void ShowConnectDialog(System.IntPtr parentFormHandle)
        {
            InternalDisplayDialog(parentFormHandle, 1);
        }
        /// <summary>
        /// Display windows dialog for disconnecting a network drive (using Desktop as parent form)
        /// </summary>		
        public void ShowDisconnectDialog()
        {
            InternalDisplayDialog(System.IntPtr.Zero, 2);
        }
        /// <summary>
        /// Display windows dialog for disconnecting a network drive
        /// </summary>
        /// <param name="parentFormHandle">Form used as a parent for the dialog</param>
        public void ShowDisconnectDialog(System.IntPtr parentFormHandle)
        {
            InternalDisplayDialog(parentFormHandle, 2);
        }

        #endregion

        #region GetMappedShareName

        /// <summary>
        /// Get Mapped Share's Name. Returns the share name of a connected network drive
        /// </summary>
        /// <param name="localDrive">Drive name (eg. 'X:')</param>
        /// <returns>Share name (eg. \\computer\share)</returns>
        public string GetMappedShareName(string localDrive)
        {
            // collect and clean the passed LocalDrive param
            if (localDrive == null || localDrive.Length == 0)
                throw new Exception("Invalid 'localDrive' passed, 'localDrive' parameter cannot be 'empty'");
            localDrive = localDrive.Substring(0, 1);

            // call api to collect LocalDrive's share name 
            int i = 255;
            byte[] bSharename = new byte[i];
            int iCallStatus = NetworkAPI.WNetGetConnection(localDrive + ":", bSharename, ref i);
            switch (iCallStatus)
            {
                case 1201:
                    throw new Exception("Cannot collect 'ShareName', Passed 'DriveName' is valid but currently not connected (API: ERROR_CONNECTION_UNAVAIL)");
                case 1208:
                    throw new Exception("API function 'WNetGetConnection' failed (API: ERROR_EXTENDED_ERROR:" + iCallStatus.ToString() + ")");
                case 1203:
                case 1222:
                    throw new Exception("Cannot collect 'ShareName', No network connection found (API: ERROR_NO_NETWORK / ERROR_NO_NET_OR_BAD_PATH)");
                case 2250:
                    throw new Exception("Invalid 'DriveName' passed, Drive is not a network drive (API: ERROR_NOT_CONNECTED)");
                case 1200:
                    throw new Exception("Invalid / Malfored 'Drive Name' passed to 'GetShareName' function (API: ERROR_BAD_DEVICE)");
                case 234:
                    throw new Exception("Invalid 'Buffer' length, buffer is too small (API: ERROR_MORE_DATA)");
            }
            // return collected share name
            return System.Text.Encoding.GetEncoding(1252).GetString(bSharename, 0, i).TrimEnd((char)0);
        }

        #endregion

        #region IsNetworkDrive

        /// <summary>
        /// Check Is Network Drive. Returns true if passed drive is a network drive
        /// </summary>
        /// <param name="localDrive">Drive name (eg. 'X:')</param>
        /// <returns>Return 'True' if the passed drive is a mapped network drive</returns>
        public bool IsNetworkDrive(string localDrive)
        {

            // collect and clean the passed LocalDrive param
            if (localDrive == null || localDrive.Trim().Length == 0)
                throw new System.Exception("Invalid 'localDrive' passed, 'localDrive' parameter cannot be 'empty'");
            localDrive = localDrive.Substring(0, 1);

            // return status of drive type
            return NetworkAPI.PathIsNetworkPath(localDrive + ":");

        }

        #endregion

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Share address to map drive to. (eg. '\\Computer\C$').
        /// </summary>
        [Category("Mapped Drive")]
        [Description(@"Get/Set Share address to map drive to. (eg. '\\Computer\C$').")]
        public string ShareName
        {
            get { return _shareName; }
            set
            {
                if (_shareName != value)
                {
                    _shareName = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Option to save credentials on reconnection.
        /// </summary>
        [Category("Mapped Drive")]
        [Description("Get/Set Option to save credentials on reconnection.")]
        public bool SaveCredentials
        {
            get { return _saveCredentials; }
            set
            {
                if (_saveCredentials != value)
                {
                    _saveCredentials = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Option to reconnect drive after log off / reboot.
        /// </summary>
        [Category("Mapped Drive")]
        [Description("Get/Set Option to reconnect drive after log off / reboot.")]
        public bool Persistent
        {
            get { return _persistent; }
            set
            {
                if (_persistent != value)
                {
                    _persistent = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Option to force connection if drive is already mapped 
        /// or force disconnection if network path is not responding.
        /// </summary>
        [Category("Mapped Drive")]
        [Description("Get/Set Option to force connection if drive is already mapped or force disconnection if network path is not responding.")]
        public bool Force
        {
            get { return _force; }
            set
            {
                if (_force != value)
                {
                    _force = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Option to prompt for user credintals when mapping a drive.
        /// </summary>
        [Category("Mapped Drive")]
        [Description("Get/Set Option to prompt for user credintals when mapping a drive.")]
        public bool PromptForCredentials
        {
            get { return _promptForCredentials; }
            set
            {
                if (_promptForCredentials != value)
                {
                    _promptForCredentials = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Option to auto select the 'LocalDrive' property to next free driver letter when mapping a network drive.
        /// </summary>
        [Category("Mapped Drive")]
        [Description("Get/Set Option to auto select the 'LocalDrive' property to next free driver letter when mapping a network drive.")]
        public bool FindNextFreeDrive
        {
            get { return _findNextFreeDrive; }
            set
            {
                if (_findNextFreeDrive != value)
                {
                    _findNextFreeDrive = value;
                }
            }
        }
        /// <summary>
        /// Get/Set Drive to be used in mapping / unmapping (eg. 's:').
        /// </summary>
        [Category("Mapped Drive")]
        [Description("Get/Set Drive to be used in mapping / unmapping (eg. 's:').")]
        public string LocalDrive
        {
            get { return _localDrive; }
            set
            {
                if (_localDrive != value)
                {
                    if (value == null || value.Length == 0)
                    {
                        _localDrive = null;
                    }
                    else
                    {

                        _localDrive = value.Substring(0, 1) + ":";
                    }
                }
            }
        }
        /// <summary>
        /// Get a string array of currently mapped network drives.
        /// </summary>
        [Browsable(false)]
        public string[] MappedDrives
        {
            get
            {
                System.Collections.ArrayList driveArray = new System.Collections.ArrayList();
                foreach (string driveLetter in System.IO.Directory.GetLogicalDrives())
                {
                    if (NetworkAPI.PathIsNetworkPath(driveLetter))
                    {
                        driveArray.Add(driveLetter);
                    }
                }
                return ((string[])driveArray.ToArray(typeof(string)));
            }
        }

        #endregion
    }

    #endregion
}
