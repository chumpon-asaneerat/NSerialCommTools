#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-17
=================
- Shell API related classes updated
  - Changes log code (used MethodBase).

======================================================================================================================
Update 2013-09-01
=================
- Shell API related classes.
  - Update code by used new Debug framework.
  - Replate ArrayList to List<T>
  - Add more exception handle code for more code stability.

======================================================================================================================
Update 2012-12-27
=================
- Shell API ported from GFA38v3 to NLib40x3.
  - Change namespace from SysLib to NLib.

======================================================================================================================
Update 2010-01-24
=================
- Shell API related classes ported from GFA Library GFA37 tor GFA38v3. (Standard/Pro version).

======================================================================================================================
Update 2008-07-02
=================
- Shell API related class added.
  - Shortcut class added. This class is used for Create Shell Link (shortcut).

======================================================================================================================
// </[History]>

#endif
#endregion

#define UNICODE // used unicode

#region Using

using System;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;

#endregion

namespace NLib.Utils
{
    #region Native Class

    // IShellLink.Resolve fFlags
    [Flags()]
    internal enum SLR_FLAGS
    {
        SLR_NO_UI = 0x1,
        SLR_ANY_MATCH = 0x2,
        SLR_UPDATE = 0x4,
        SLR_NOUPDATE = 0x8,
        SLR_NOSEARCH = 0x10,
        SLR_NOTRACK = 0x20,
        SLR_NOLINKINFO = 0x40,
        SLR_INVOKE_MSI = 0x80
    }

    // IShellLink.GetPath fFlags
    [Flags()]
    internal enum SLGP_FLAGS
    {
        SLGP_SHORTPATH = 0x1,
        SLGP_UNCPRIORITY = 0x2,
        SLGP_RAWPATH = 0x4
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct WIN32_FIND_DATAA
    {
        public int dwFileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
        public int nFileSizeHigh;
        public int nFileSizeLow;
        public int dwReserved0;
        public int dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName;
        private const int MAX_PATH = 260;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WIN32_FIND_DATAW
    {
        public int dwFileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
        public int nFileSizeHigh;
        public int nFileSizeLow;
        public int dwReserved0;
        public int dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName;
        private const int MAX_PATH = 260;
    }

    [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
      Guid("0000010B-0000-0000-C000-000000000046")]
    internal interface IPersistFile
    {
        #region Methods inherited from IPersist

        void GetClassID(
          out Guid pClassID);

        #endregion

        [PreserveSig()]
        int IsDirty();

        void Load(
          [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
          int dwMode);

        void Save(
          [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
          [MarshalAs(UnmanagedType.Bool)] bool fRemember);

        void SaveCompleted(
          [MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

        void GetCurFile(
          out IntPtr ppszFileName);

    }

    [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
      Guid("000214EE-0000-0000-C000-000000000046")]
    internal interface IShellLinkA
    {
        void GetPath(
          [Out(), MarshalAs(UnmanagedType.LPStr)] StringBuilder pszFile,
          int cchMaxPath,
          out WIN32_FIND_DATAA pfd,
          SLGP_FLAGS fFlags);

        void GetIDList(
          out IntPtr ppidl);

        void SetIDList(
          IntPtr pidl);

        void GetDescription(
          [Out(), MarshalAs(UnmanagedType.LPStr)] StringBuilder pszName,
          int cchMaxName);

        void SetDescription(
          [MarshalAs(UnmanagedType.LPStr)] string pszName);

        void GetWorkingDirectory(
          [Out(), MarshalAs(UnmanagedType.LPStr)] StringBuilder pszDir,
          int cchMaxPath);

        void SetWorkingDirectory(
          [MarshalAs(UnmanagedType.LPStr)] string pszDir);

        void GetArguments(
          [Out(), MarshalAs(UnmanagedType.LPStr)] StringBuilder pszArgs,
          int cchMaxPath);

        void SetArguments(
          [MarshalAs(UnmanagedType.LPStr)] string pszArgs);

        void GetHotkey(
          out short pwHotkey);

        void SetHotkey(
          short wHotkey);

        void GetShowCmd(
          out int piShowCmd);

        void SetShowCmd(
          int iShowCmd);

        void GetIconLocation(
          [Out(), MarshalAs(UnmanagedType.LPStr)] StringBuilder pszIconPath,
          int cchIconPath,
          out int piIcon);

        void SetIconLocation(
          [MarshalAs(UnmanagedType.LPStr)] string pszIconPath,
          int iIcon);

        void SetRelativePath(
          [MarshalAs(UnmanagedType.LPStr)] string pszPathRel,
          int dwReserved);

        void Resolve(
          IntPtr hwnd,
          SLR_FLAGS fFlags);

        void SetPath(
          [MarshalAs(UnmanagedType.LPStr)] string pszFile);

    }

    [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
      Guid("000214F9-0000-0000-C000-000000000046")]
    internal interface IShellLinkW
    {
        void GetPath(
          [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
          int cchMaxPath,
          out WIN32_FIND_DATAW pfd,
          SLGP_FLAGS fFlags);

        void GetIDList(
          out IntPtr ppidl);

        void SetIDList(
          IntPtr pidl);

        void GetDescription(
          [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName,
          int cchMaxName);

        void SetDescription(
          [MarshalAs(UnmanagedType.LPWStr)] string pszName);

        void GetWorkingDirectory(
          [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir,
          int cchMaxPath);

        void SetWorkingDirectory(
          [MarshalAs(UnmanagedType.LPWStr)] string pszDir);

        void GetArguments(
          [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs,
          int cchMaxPath);

        void SetArguments(
          [MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

        void GetHotkey(
          out short pwHotkey);

        void SetHotkey(
          short wHotkey);

        void GetShowCmd(
          out int piShowCmd);

        void SetShowCmd(
          int iShowCmd);

        void GetIconLocation(
          [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath,
          int cchIconPath,
          out int piIcon);

        void SetIconLocation(
          [MarshalAs(UnmanagedType.LPWStr)] string pszIconPath,
          int iIcon);

        void SetRelativePath(
          [MarshalAs(UnmanagedType.LPWStr)] string pszPathRel,
          int dwReserved);

        void Resolve(
          IntPtr hwnd,
          SLR_FLAGS fFlags);

        void SetPath(
          [MarshalAs(UnmanagedType.LPWStr)] string pszFile);

    }

    [ComImport(), Guid("00021401-0000-0000-C000-000000000046")]
    internal class ShellLink  // : IPersistFile, IShellLinkA, IShellLinkW 
    {
    }

    #endregion

    #region Shortcut

    /// <remarks>
    /// Shortcut class. This is the .NET friendly wrapper for the ShellLink class
    /// </remarks>
    public class Shortcut : IDisposable
    {
        #region Native Win32 API functions

        private class Native
        {
            /// <summary>
            /// Extract Icon
            /// </summary>
            /// <param name="hInst"></param>
            /// <param name="lpszExeFileName"></param>
            /// <param name="nIconIndex"></param>
            /// <returns></returns>
            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);
            /// <summary>
            /// Destroy Icon
            /// </summary>
            /// <param name="hIcon"></param>
            /// <returns></returns>
            [DllImport("user32.dll")]
            public static extern bool DestroyIcon(IntPtr hIcon);
        }

        #endregion

        #region Internal Const

        private const int INFOTIPSIZE = 1024;
        private const int MAX_PATH = 260;

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWMINNOACTIVE = 7;

        #endregion

        #region Internal Variable

#if UNICODE
        private IShellLinkW m_Link;
#else
        private IShellLinkA m_Link;
#endif
        private string m_sPath;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name='linkPath'>Path to new or existing shortcut file (.lnk).</param>
        public Shortcut(string linkPath)
        {
            IPersistFile pf;

            m_sPath = linkPath;
#if UNICODE
            m_Link = (IShellLinkW)new ShellLink();
#else
            m_Link = (IShellLinkA)new ShellLink();
#endif
            if (File.Exists(linkPath))
            {
                pf = (IPersistFile)m_Link;
                pf.Load(linkPath, 0);
            }
        }

        #endregion

        #region IDisposable Implements

        /// <summary>
        /// Dispose
        /// </summary>
        void IDisposable.Dispose()
        {
            if (m_Link != null)
            {
                Marshal.ReleaseComObject(m_Link);
                m_Link = null;
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        ///   Saves the shortcut to disk.
        /// </summary>
        public void Save()
        {
            IPersistFile pf = (IPersistFile)m_Link;
            pf.Save(m_sPath, true);
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets or sets the argument list of the shortcut.
        /// </summary>
        public string Arguments
        {
            get
            {
                StringBuilder sb = new StringBuilder(INFOTIPSIZE);
                m_Link.GetArguments(sb, sb.Capacity);
                return sb.ToString();
            }
            set { m_Link.SetArguments(value); }
        }
        /// <summary>
        /// Gets or sets a description of the shortcut.
        /// </summary>
        public string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder(INFOTIPSIZE);
                m_Link.GetDescription(sb, sb.Capacity);
                return sb.ToString();
            }
            set { m_Link.SetDescription(value); }
        }
        /// <summary>
        /// Gets or sets the working directory (aka start in directory) of the shortcut.
        /// </summary>
        public string WorkingDirectory
        {
            get
            {
                StringBuilder sb = new StringBuilder(MAX_PATH);
                m_Link.GetWorkingDirectory(sb, sb.Capacity);
                return sb.ToString();
            }
            set { m_Link.SetWorkingDirectory(value); }
        }
        /// <summary>
        /// Gets or sets the target path of the shortcut. 
        /// Note If Path returns an empty string, the shortcut is associated with
        /// a PIDL instead, which can be retrieved with IShellLink.GetIDList().
        /// This is beyond the scope of this wrapper class.
        /// </summary>
        public string Path
        {
            get
            {
#if UNICODE
                WIN32_FIND_DATAW wfd = new WIN32_FIND_DATAW();
#else
                WIN32_FIND_DATAA wfd = new WIN32_FIND_DATAA();
#endif
                StringBuilder sb = new StringBuilder(MAX_PATH);

                m_Link.GetPath(sb, sb.Capacity, out wfd, SLGP_FLAGS.SLGP_UNCPRIORITY);
                return sb.ToString();
            }
            set { m_Link.SetPath(value); }
        }
        /// <summary>
        /// Gets or sets the path of the <see cref="Icon"/> assigned to the shortcut.
        /// </summary>
        public string IconPath
        {
            get
            {
                StringBuilder sb = new StringBuilder(MAX_PATH);
                int nIconIdx;
                m_Link.GetIconLocation(sb, sb.Capacity, out nIconIdx);
                return sb.ToString();
            }
            set { m_Link.SetIconLocation(value, IconIndex); }
        }
        /// <summary>
        /// Gets or sets the index of the <see cref="Icon"/> assigned to the shortcut.
        /// Set to zero when the <see cref="IconPath"/> property specifies a .ICO file.
        /// </summary>
        public int IconIndex
        {
            get
            {
                StringBuilder sb = new StringBuilder(MAX_PATH);
                int nIconIdx;
                m_Link.GetIconLocation(sb, sb.Capacity, out nIconIdx);
                return nIconIdx;
            }
            set { m_Link.SetIconLocation(IconPath, value); }
        }
        /// <summary>
        /// Retrieves the Icon of the shortcut as it will appear in Explorer.
        /// Use the <see cref="IconPath"/> and <see cref="IconIndex"/>
        /// properties to change it.
        /// </summary>
        public Icon Icon
        {
            get
            {
                StringBuilder sb = new StringBuilder(MAX_PATH);
                int nIconIdx;
                IntPtr hIcon, hInst;
                Icon ico, clone;

                m_Link.GetIconLocation(sb, sb.Capacity, out nIconIdx);
                hInst = Marshal.GetHINSTANCE(this.GetType().Module);
                hIcon = Native.ExtractIcon(hInst, sb.ToString(), nIconIdx);
                if (hIcon == IntPtr.Zero)
                    return null;

                // Return a cloned Icon, because we have to free the original ourselves.
                ico = Icon.FromHandle(hIcon);
                clone = (Icon)ico.Clone();
                ico.Dispose();
                Native.DestroyIcon(hIcon);
                return clone;
            }
        }
        /// <summary>
        /// Gets or sets the System.Diagnostics.ProcessWindowStyle value
        /// that decides the initial show state of the shortcut target. Note that
        /// ProcessWindowStyle.Hidden is not a valid property value.
        /// </summary>
        public ProcessWindowStyle WindowStyle
        {
            get
            {
                int nWS;
                m_Link.GetShowCmd(out nWS);
                switch (nWS)
                {
                    case SW_SHOWMINIMIZED:
                    case SW_SHOWMINNOACTIVE:
                        return ProcessWindowStyle.Minimized;
                    case SW_SHOWMAXIMIZED:
                        return ProcessWindowStyle.Maximized;
                    default:
                        return ProcessWindowStyle.Normal;
                }
            }
            set
            {
                int nWS;
                switch (value)
                {
                    case ProcessWindowStyle.Normal:
                        nWS = SW_SHOWNORMAL;
                        break;
                    case ProcessWindowStyle.Minimized:
                        nWS = SW_SHOWMINNOACTIVE;
                        break;
                    case ProcessWindowStyle.Maximized:
                        nWS = SW_SHOWMAXIMIZED;
                        break;
                    default: // ProcessWindowStyle.Hidden
                        throw new ArgumentException("Unsupported ProcessWindowStyle value.");
                }
                m_Link.SetShowCmd(nWS);

            }
        }
        /// <summary>
        /// Gets or sets the hotkey for the shortcut.
        /// </summary>
        public Keys Hotkey
        {
            get
            {
                short wHotkey;
                int dwHotkey;

                m_Link.GetHotkey(out wHotkey);

                //
                // Convert from IShellLink 16-bit format to Keys enumeration 32-bit value
                // IShellLink: 0xMMVK
                // Keys:  0x00MM00VK        
                //   MM = Modifier (Alt, Control, Shift)
                //   VK = Virtual key code
                //       
                dwHotkey = ((wHotkey & 0xFF00) << 8) | (wHotkey & 0xFF);
                return (Keys)dwHotkey;
            }
            set
            {
                short wHotkey;

                if ((value & Keys.Modifiers) == 0)
                    throw new ArgumentException("Hotkey must include a modifier key.");

                //    
                // Convert from Keys enumeration 32-bit value to IShellLink 16-bit format
                // IShellLink: 0xMMVK
                // Keys:  0x00MM00VK        
                //   MM = Modifier (Alt, Control, Shift)
                //   VK = Virtual key code
                //       
                wHotkey = unchecked((short)(((int)(value & Keys.Modifiers) >> 8) | (int)(value & Keys.KeyCode)));
                m_Link.SetHotkey(wHotkey);

            }
        }
        /// <summary>
        /// Returns a reference to the internal ShellLink object,
        /// which can be used to perform more advanced operations
        /// not supported by this wrapper class, by using the
        /// IShellLink interface directly.
        /// </summary>
        internal object ShellLink
        {
            get { return m_Link; }
        }

        #endregion
    }

    #endregion
}

namespace NLib.Utils
{
    #region Using

    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using Microsoft.Win32;

    #endregion

    #region Helper Class for File Association

    /// <summary>
    /// File Association Shell Helper
    /// </summary>
    internal class FileAssociationShellHelper
    {
        #region Win32

        private const int SEE_MASK_INVOKEIDLIST = 0xC;

        [StructLayout(LayoutKind.Explicit)]
        private struct DUMMYUNIONNAME
        {
            [FieldOffset(0)]
            public IntPtr hIcon;
            [FieldOffset(0)]
            public IntPtr hMonitor;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public int fMask;
            public IntPtr hwnd;
            public string lpVerb;
            public string lpFile;
            public string lpParameters;
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public int lpIDList;
            public string lpClass;
            public int hkeyClass;
            public int dwHotKey;
            public DUMMYUNIONNAME dun;
            public IntPtr hProcess;
        }

        [DllImport("shell32.dll")]
        private static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO si);

        #endregion

        #region File and Process

        /// <summary>
        /// Show File Properties
        /// </summary>
        /// <param name="fileName"></param>
        public static void ShowFileProperties(string fileName)
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                SHELLEXECUTEINFO si = new SHELLEXECUTEINFO();
                si.cbSize = Marshal.SizeOf(typeof(SHELLEXECUTEINFO));
                si.lpFile = fileName;
                si.lpVerb = "properties";
                si.fMask = SEE_MASK_INVOKEIDLIST;
                ShellExecuteEx(ref si);
            }
            catch (Exception ex)
            {
                ex.Err(med);
            }
        }
        /// <summary>
        /// Run PPT.
        /// </summary>
        /// <param name="fileName">The power point file name.</param>
        public static void RunPPT(string fileName)
        {
            if (fileName == string.Empty || !File.Exists(fileName))
                return;

            string pptFileName = fileName;
            // start power point
            Process process = new Process();
            process.StartInfo.FileName = "POWERPNT.EXE";
            if (System.IO.File.Exists(pptFileName))
            {
                process.StartInfo.Arguments = "\"" + pptFileName + "\"";
            }

            process.StartInfo.UseShellExecute = true;

            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                // cannot run ppt
                ex.Err(med);
            }
        }

        #endregion

        #region Create Icon resource

        /// <summary>
        /// Create Icon Resource.
        /// </summary>
        /// <param name="assm">The resource assembly.</param>
        /// <param name="resourceName">
        /// The resource icon full path name i.e. MyChoice.Resource.MyChoiceDb.ico
        /// </param>
        /// <param name="FullFileName">Target icon file.</param>
        /// <returns>Returns true if icon exist.</returns>
        public static bool CreateIconResource(Assembly assm,
            string resourceName, string FullFileName)
        {
            //string resourceName = "MyChoice.Resource.MyChoiceDb.ico";
            bool fileExists = File.Exists(FullFileName);
            if (fileExists)
            {
                // not overwrite and file is already exists
                return fileExists;
            }

            MethodBase med = MethodBase.GetCurrentMethod();
            // check directory
            string targetDir = Path.GetDirectoryName(FullFileName);
            if (!Directory.Exists(targetDir))
            {
                try
                {
                    // not extist create it.
                    Directory.CreateDirectory(targetDir);
                }
                catch (Exception ex)
                {
                    ex.Err(med);
                }
            }

            if (!Directory.Exists(targetDir))
            {
                // cannot create target directory.
                return false;
            }

            bool hasError = false;

            if (!File.Exists(FullFileName) && 
                null != assm)
            {
                // Get current assembly and read resource stream
                Stream stream = assm.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    #region Free Resource
                    
                    // Close/Dispose stream.
                    try { if (null != stream) stream.Close(); }
                    catch { }
                    try { if (null != stream) stream.Dispose(); }
                    catch { }
                    stream = null;

                    #endregion

                    return false;
                }

                FileStream fs = null;

                #region Create File Stream to write
                
                try
                {
                    fs = new FileStream(FullFileName, FileMode.CreateNew);
                }
                catch (Exception ex)
                {
                    ex.Err(med);

                    #region Free Resource when error

                    // Make sure to close and dispose file stream.
                    try { if (null != fs) fs.Close(); }
                    catch { }
                    try { if (null != fs) fs.Dispose(); }
                    catch { }
                    fs = null;

                    #endregion
                }

                #endregion

                #region Read data byte by byte

                if (null != fs)
                {
                    for (int i = 0; i < stream.Length; i++)
                    {
                        try
                        {
                            fs.WriteByte(Convert.ToByte(stream.ReadByte()));
                        }
                        catch (Exception ex)
                        {
                            ex.Err(med);
                            hasError = true;
                            break; // has error so stop immediately
                        }
                    }
                }

                #endregion

                #region Free Resource

                // Flush, Close and Dispose.
                try { if (null != fs) fs.Flush(); }
                catch { }
                try { if (null != fs) fs.Close(); }
                catch { }
                try { if (null != fs) fs.Dispose(); }
                catch { }
                fs = null;
                // Close/Dispose stream.
                try { if (null != stream) stream.Close(); }
                catch { }
                try { if (null != stream) stream.Dispose(); }
                catch { }
                stream = null;

                #endregion
            }

            if (hasError)
            {
                // Has error when write some byte.
                return false;
            }

            // Returns true if file is exists.
            return File.Exists(FullFileName);
        }

        #endregion
    }

    #endregion

    #region File Association classes

    /// <summary>List of commands.</summary>
    internal struct CommandList
    {
        /// <summary>
        /// Holds the names of the commands.
        /// </summary>
        public List<string> Captions;
        /// <summary>
        /// Holds the commands.
        /// </summary>
        public List<string> Commands;
    }

    /// <summary>Properties of the file association.</summary>
    internal struct FileType
    {
        /// <summary>
        /// Holds the command names and the commands.
        /// </summary>
        public CommandList Commands;
        /// <summary>
        /// Holds the extension of the file type.
        /// </summary>
        public string Extension;
        /// <summary>
        /// Holds the proper name of the file type.
        /// </summary>
        public string ProperName;
        /// <summary>
        /// Holds the full name of the file type.
        /// </summary>
        public string FullName;
        /// <summary>
        /// Holds the name of the content type of the file type.
        /// </summary>
        public string ContentType;
        /// <summary>
        /// Holds the path to the resource with the icon of this file type.
        /// </summary>
        public string IconPath;
        /// <summary>
        /// Holds the icon index in the resource file.
        /// </summary>
        public short IconIndex;
    }
    /// <summary>Creates file associations for your programs.</summary>
    /// <example>The following example creates a file association for the type XYZ with a non-existent program.
    /// <br></br><br>VB.NET code</br>
    /// <code>
    /// Dim FA as New FileAssociation
    /// FA.Extension = "xyz"
    /// FA.ContentType = "application/myprogram"
    /// FA.FullName = "My XYZ Files!"
    /// FA.ProperName = "XYZ File"
    /// FA.AddCommand("open", "C:\mydir\myprog.exe %1")
    /// FA.Create
    /// </code>
    /// <br>C# code</br>
    /// <code>
    /// FileAssociation FA = new FileAssociation();
    /// FA.Extension = "xyz";
    /// FA.ContentType = "application/myprogram";
    /// FA.FullName = "My XYZ Files!";
    /// FA.ProperName = "XYZ File";
    /// FA.AddCommand("open", "C:\\mydir\\myprog.exe %1");
    /// FA.Create();
    /// </code>
    /// </example>
    public class FileAssociation
    {
        /// <summary>
        /// Initializes an instance of the FileAssociation class.
        /// </summary>
        public FileAssociation()
        {
            FileInfo = new FileType();
            FileInfo.Commands.Captions = new List<string>();
            FileInfo.Commands.Commands = new List<string>();
        }
        /// <summary>
        /// Gets or sets the proper name of the file type.
        /// </summary>
        /// <value>A String representing the proper name of the file type.</value>
        public string ProperName
        {
            get
            {
                return FileInfo.ProperName;
            }
            set
            {
                FileInfo.ProperName = value;
            }
        }
        /// <summary>Gets or sets the full name of the file type.</summary>
        /// <value>A String representing the full name of the file type.</value>
        public string FullName
        {
            get
            {
                return FileInfo.FullName;
            }
            set
            {
                FileInfo.FullName = value;
            }
        }
        /// <summary>
        /// Gets or sets the content type of the file type.
        /// </summary>
        /// <value>A String representing the content type of the file type.</value>
        public string ContentType
        {
            get
            {
                return FileInfo.ContentType;
            }
            set
            {
                FileInfo.ContentType = value;
            }
        }
        /// <summary>
        /// Gets or sets the extension of the file type.
        /// </summary>
        /// <value>A String representing the extension of the file type.</value>
        /// <remarks>If the extension doesn't start with a dot ("."), a dot is automatically added.</remarks>
        public string Extension
        {
            get
            {
                return FileInfo.Extension;
            }
            set
            {
                if (value.Substring(0, 1) != ".")
                    value = "." + value;
                FileInfo.Extension = value;
            }
        }
        /// <summary>
        /// Gets or sets the index of the icon of the file type.
        /// </summary>
        /// <value>A short representing the index of the icon of the file type.</value>
        public short IconIndex
        {
            get
            {
                return FileInfo.IconIndex;
            }
            set
            {
                FileInfo.IconIndex = value;
            }
        }
        /// <summary>
        /// Gets or sets the path of the resource that contains the icon for the file type.
        /// </summary>
        /// <value>A String representing the path of the resource that contains the icon for the file type.</value>
        /// <remarks>This resource can be an executable or a DLL.</remarks>
        public string IconPath
        {
            get
            {
                return FileInfo.IconPath;
            }
            set
            {
                FileInfo.IconPath = value;
            }
        }
        /// <summary>
        /// Adds a new command to the command list.
        /// </summary>
        /// <param name="Caption">The name of the command.</param>
        /// <param name="Command">The command to execute.</param>
        /// <exceptions cref="ArgumentNullException">Caption -or- Command is null (VB.NET: Nothing).</exceptions>
        public void AddCommand(string Caption, string Command)
        {
            if (Caption == null || Command == null)
                throw new ArgumentNullException();
            FileInfo.Commands.Captions.Add(Caption);
            FileInfo.Commands.Commands.Add(Command);
        }
        /// <summary>
        /// Creates the file association.
        /// </summary>
        /// <exceptions cref="ArgumentNullException">Extension -or- ProperName is null (VB.NET: Nothing).</exceptions>
        /// <exceptions cref="ArgumentException">Extension -or- ProperName is empty.</exceptions>
        /// <exceptions cref="System.Security.SecurityException">The user does not have registry write access.</exceptions>
        public void Create()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            // remove the extension to avoid incompatibilities [such as DDE links]
            try
            {
                Remove();
            }
            catch (ArgumentException) { } // the extension doesn't exist
            // create the exception
            if (Extension == "" || ProperName == "")
                throw new ArgumentException();
            int cnt;
            try
            {
                RegistryKey RegKey = Registry.ClassesRoot.CreateSubKey(Extension);
                RegKey.SetValue("", ProperName);
                if (ContentType != null && ContentType != "")
                    RegKey.SetValue("Content Type", ContentType);
                RegKey.Close();
                RegKey = Registry.ClassesRoot.CreateSubKey(ProperName);
                RegKey.SetValue("", FullName);
                RegKey.Close();
                if (IconPath != "")
                {
                    RegKey = Registry.ClassesRoot.CreateSubKey(ProperName + "\\" + "DefaultIcon");
                    RegKey.SetValue("", IconPath + "," + IconIndex.ToString());
                    RegKey.Close();
                }
                for (cnt = 0; cnt < FileInfo.Commands.Captions.Count; cnt++)
                {
                    RegKey = Registry.ClassesRoot.CreateSubKey(ProperName + "\\" + "Shell" + "\\" + (String)FileInfo.Commands.Captions[cnt]);
                    RegKey = RegKey.CreateSubKey("Command");
                    RegKey.SetValue("", FileInfo.Commands.Commands[cnt]);
                    RegKey.Close();
                }
            }
            catch (Exception ex)
            {
                ex.Err(med);
                //throw new System.Security.SecurityException();
            }
        }
        /// <summary>
        /// Removes the file association.
        /// </summary>
        /// <exceptions cref="ArgumentNullException">Extension -or- ProperName is null (VB.NET: Nothing).</exceptions>
        /// <exceptions cref="ArgumentException">Extension -or- ProperName is empty -or- the specified extension doesn't exist.</exceptions>
        /// <exceptions cref="System.Security.SecurityException">The user does not have registry delete access.</exceptions>
        public void Remove()
        {
            if (Extension == null || ProperName == null)
                throw new ArgumentNullException();
            if (Extension == "" || ProperName == "")
                throw new ArgumentException();
            Registry.ClassesRoot.DeleteSubKeyTree(Extension);
            Registry.ClassesRoot.DeleteSubKeyTree(ProperName);
        }
        /// <summary>Holds the properties of the file type.</summary>
        private FileType FileInfo;
    }

    #endregion
}