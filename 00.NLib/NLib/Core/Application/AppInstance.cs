#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2012-01-04
=================
- AppInstance class ported.
  - Ported AppInstance and its related classes from GFA40 to NLib.
  - Merge all Memory mapped file classes in SysLib.Threads into AppInstance.cs
    and make its used internal only.

======================================================================================================================
Update 2008-03-26
=================
- AppInstance is redesign
  - AppInstance is redesign to support detected instance of application that stay in System Tray. See Example.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

#endregion

namespace NLib
{
    #region Memory Mapped Files

    #region NTKernel

    internal class NTKernel
    {
        #region Data Structures

        [StructLayout(LayoutKind.Sequential)]
        internal class SecurityAttributes
        {
            public SecurityAttributes(object securityDescriptor)
            {
                this.lpSecurityDescriptor = securityDescriptor;
            }

            uint nLegnth = 12;
            object lpSecurityDescriptor;
            [MarshalAs(UnmanagedType.VariantBool)]
            bool bInheritHandle = true;
        }

        #endregion

        #region General

        [DllImport("kernel32", EntryPoint = "CloseHandle", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.VariantBool)]
        internal static extern bool CloseHandle(uint hHandle);

        [DllImport("kernel32", EntryPoint = "GetLastError", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint GetLastError();

        #endregion

        #region Semaphore

        [DllImport("kernel32", EntryPoint = "CreateSemaphore", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint CreateSemaphore(SecurityAttributes auth, int initialCount, int maximumCount, string name);

        [DllImport("kernel32", EntryPoint = "WaitForSingleObject", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint WaitForSingleObject(uint hHandle, uint dwMilliseconds);

        [DllImport("kernel32", EntryPoint = "ReleaseSemaphore", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.VariantBool)]
        internal static extern bool ReleaseSemaphore(uint hHandle, int lReleaseCount, out int lpPreviousCount);

        #endregion

        #region Memory Mapped Files

        [DllImport("Kernel32.dll", EntryPoint = "CreateFileMapping", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateFileMapping(uint hFile, SecurityAttributes lpAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

        [DllImport("Kernel32.dll", EntryPoint = "OpenFileMapping", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr OpenFileMapping(uint dwDesiredAccess, bool bInheritHandle, string lpName);

        [DllImport("Kernel32.dll", EntryPoint = "MapViewOfFile", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        [DllImport("Kernel32.dll", EntryPoint = "UnmapViewOfFile", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.VariantBool)]
        internal static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        #endregion
    }

    #endregion

    #region Memory Mapped File View

    /// <summary>
    /// Memory Mapped File View Class.
    /// </summary>
    internal class MemoryMappedFileView : IDisposable
    {
        #region Internal Variable

        private IntPtr mappedView;
        private readonly int size;
        private readonly ViewAccess access;

        #endregion

        #region View Access Enum

        /// <summary>
        /// View Access
        /// </summary>
        public enum ViewAccess : int
        {
            /// <summary>
            /// Read and Write
            /// </summary>
            ReadWrite = 2,
            /// <summary>
            /// Read Only
            /// </summary>
            Read = 4,
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mappedView">The mapped view pointer.</param>
        /// <param name="size">The buffer size.</param>
        /// <param name="access">The view access mode.</param>
        internal MemoryMappedFileView(IntPtr mappedView, int size, ViewAccess access)
        {
            this.mappedView = mappedView;
            this.size = size;
            this.access = access;
        }

        #endregion

        #region IDisposable Member

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (mappedView != IntPtr.Zero)
            {
                if (NTKernel.UnmapViewOfFile(mappedView))
                    mappedView = IntPtr.Zero;
            }
        }

        #endregion

        #region Basic Read/Write Bytes

        /// <summary>
        /// ReadBytes
        /// </summary>
        /// <param name="data">The data to read in byte array.</param>
        public void ReadBytes(byte[] data)
        {
            ReadBytes(data, 0);
        }
        /// <summary>
        /// ReadBytes
        /// </summary>
        /// <param name="data">The data to read in byte array.</param>
        /// <param name="offset">The offset to begin read.</param>
        public void ReadBytes(byte[] data, int offset)
        {
            for (int i = 0; i < data.Length; i++)
                data[i] = Marshal.ReadByte(mappedView, offset + i);
        }
        /// <summary>
        /// WriteBytes
        /// </summary>
        /// <param name="data">The data to write in byte array.</param>
        public void WriteBytes(byte[] data)
        {
            WriteBytes(data, 0);
        }
        /// <summary>
        /// WriteBytes
        /// </summary>
        /// <param name="data">The data to write in byte array.</param>
        /// <param name="offset">The offset to begin write.</param>
        public void WriteBytes(byte[] data, int offset)
        {
            for (int i = 0; i < data.Length; i++)
                Marshal.WriteByte(mappedView, offset + i, data[i]);
        }

        #endregion

        #region Additional Accessors

        /// <summary>
        /// ReadByte
        /// </summary>
        /// <param name="offset">The offset to begin read.</param>
        /// <returns>Returns byte in byte data at specificed offset.</returns>
        public byte ReadByte(int offset)
        {
            return Marshal.ReadByte(mappedView, offset);
        }
        /// <summary>
        /// WriteByte
        /// </summary>
        /// <param name="data">The data to write in byte.</param>
        /// <param name="offset">The offset to begin write.</param>
        public void WriteByte(byte data, int offset)
        {
            Marshal.WriteByte(mappedView, offset, data);
        }
        /// <summary>
        /// ReadInt16
        /// </summary>
        /// <param name="offset">The offset to begin read.</param>
        /// <returns>Returns byte in Int16 data at specificed offset.</returns>
        public short ReadInt16(int offset)
        {
            return Marshal.ReadInt16(mappedView, offset);
        }
        /// <summary>
        /// WriteInt16
        /// </summary>
        /// <param name="data">The data to write in Int16</param>
        /// <param name="offset">The offset to begin write.</param>
        public void WriteInt16(short data, int offset)
        {
            Marshal.WriteInt16(mappedView, offset, data);
        }
        /// <summary>
        /// ReadInt32
        /// </summary>
        /// <param name="offset">The offset to begin read.</param>
        /// <returns>Returns byte in Int32 data at specificed offset.</returns>
        public int ReadInt32(int offset)
        {
            return Marshal.ReadInt32(mappedView, offset);
        }
        /// <summary>
        /// WriteInt32
        /// </summary>
        /// <param name="data">The data to write in Int32</param>
        /// <param name="offset">The offset to begin write.</param>
        public void WriteInt32(int data, int offset)
        {
            Marshal.WriteInt32(mappedView, offset, data);
        }
        /// <summary>
        /// ReadInt64
        /// </summary>
        /// <param name="offset">The offset to begin read.</param>
        /// <returns>Returns byte in Int64 data at specificed offset.</returns>
        public long ReadInt64(int offset)
        {
            return Marshal.ReadInt64(mappedView, offset);
        }
        /// <summary>
        /// WriteInt64
        /// </summary>
        /// <param name="data">The data to write in Int64</param>
        /// <param name="offset">The offset to begin write.</param>
        public void WriteInt64(long data, int offset)
        {
            Marshal.WriteInt64(mappedView, offset, data);
        }
        /// <summary>
        /// ReadStructure
        /// </summary>
        /// <param name="structureType">The target structure type.</param>
        /// <returns>Returns structure that read from buffer.</returns>
        public object ReadStructure(Type structureType)
        {
            return Marshal.PtrToStructure(mappedView, structureType);
        }
        /// <summary>
        /// WriteStructure
        /// </summary>
        /// <param name="data">The data structure to wrtie.</param>
        public void WriteStructure(object data)
        {
            Marshal.StructureToPtr(data, mappedView, true);
        }
        /// <summary>
        /// ReadDeserialize
        /// </summary>
        /// <returns>Returns deserialized object.</returns>
        public object ReadDeserialize()
        {
            return ReadDeserialize(0, size);
        }
        /// <summary>
        /// ReadDeserialize
        /// </summary>
        /// <param name="offset">The offset to begin read.</param>
        /// <returns>Returns deserialized object.</returns>
        public object ReadDeserialize(int offset)
        {
            return ReadDeserialize(offset, size - offset);
        }
        /// <summary>
        /// ReadDeserialize
        /// </summary>
        /// <param name="offset">The offset to begin read.</param>
        /// <param name="length">The size of data to read from beginning of buffer.</param>
        /// <returns>Returns deserialized object.</returns>
        public object ReadDeserialize(int offset, int length)
        {
            byte[] binaryData = new byte[length];
            ReadBytes(binaryData, offset);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter
                = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.MemoryStream ms = new System.IO.MemoryStream(binaryData, 0, length, true, true);
            object data = formatter.Deserialize(ms);
            ms.Close();
            return data;
        }
        /// <summary>
        /// Serializes the data and writes it to the file.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        public void WriteSerialize(object data)
        {
            WriteSerialize(data, 0, size);
        }
        /// <summary>
        /// Serializes the data and writes it to the file.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        /// <param name="offset">The position in the file to start.</param>
        public void WriteSerialize(object data, int offset)
        {
            WriteSerialize(data, 0, size - offset);
        }
        /// <summary>
        /// Serializes the data and writes it to the file.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        /// <param name="offset">The position in the file to start.</param>
        /// <param name="length">The buffer size in bytes.</param>
        public void WriteSerialize(object data, int offset, int length)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter
                = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            byte[] binaryData = new byte[length];
            System.IO.MemoryStream ms = new System.IO.MemoryStream(binaryData, 0, length, true, true);
            formatter.Serialize(ms, data);
            ms.Flush();
            ms.Close();
            WriteBytes(binaryData, offset);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Get Size of view.
        /// </summary>
        public int Size
        {
            get { return size; }
        }

        #endregion
    }

    #endregion

    #region Memory Mapped File

    /// <summary>
    /// Memory Mapped File Win32 Wrapper Class.
    /// </summary>
    internal class MemoryMappedFile : IDisposable
    {
        #region Internal Variable

        private IntPtr fileMapping;
        private readonly int size;
        private readonly FileAccess access;

        #endregion

        #region Enum

        /// <summary>
        /// File Access Enum
        /// </summary>
        public enum FileAccess : int
        {
            /// <summary>
            /// Read Only access
            /// </summary>
            ReadOnly = 2,
            /// <summary>
            /// Read and Write access
            /// </summary>
            ReadWrite = 4
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileMapping">The File Pointer.</param>
        /// <param name="size">The Size to mapped.</param>
        /// <param name="access">The Mapped File Access mode.</param>
        private MemoryMappedFile(IntPtr fileMapping, int size, FileAccess access)
        {
            this.fileMapping = fileMapping;
            this.size = size;
            this.access = access;
        }

        #endregion

        #region IDisposable Member

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (fileMapping != IntPtr.Zero)
            {
                if (NTKernel.CloseHandle((uint)fileMapping))
                    fileMapping = IntPtr.Zero;
            }
        }

        #endregion

        #region Create File

        /// <summary>
        /// Create a virtual memory mapped file located in the system page file.
        /// </summary>
        /// <param name="name">The name of the file. Prefix it with "Global\" or "Local\" to control its scope between NT services and user applications in Terminal Server scenarios.</param>
        /// <param name="access">Whether you need write access to the file.</param>
        /// <param name="size">The preferred size of the file in terms of bytes.</param>
        /// <returns>A MemoryMappedFile instance representing the file.</returns>
        public static MemoryMappedFile CreateFile(string name,
            FileAccess access, int size)
        {
            if (size < 0)
            {
                throw new ArgumentException("Size must not be negative", "size");
            }
            //object descriptor = null;
            //NTAdvanced.InitializeSecurityDescriptor(out descriptor,1);
            //NTAdvanced.SetSecurityDescriptorDacl(ref descriptor,true,null,false);
            //NTKernel.SecurityAttributes sa = new NTKernel.SecurityAttributes(descriptor);

            IntPtr fileMapping = NTKernel.CreateFileMapping(0xFFFFFFFFu, null, (uint)access, 0, (uint)size, name);
            if (fileMapping == IntPtr.Zero)
                throw new Exception("Cannot file mapping pointer.");

            return new MemoryMappedFile(fileMapping, size, access);
        }

        #endregion

        #region Create View

        /// <summary>
        /// Create a view of the memory mapped file, allowing to read/write bytes.
        /// </summary>
        /// <param name="offset">An optional offset to the file.</param>
        /// <param name="size">The size of the view in terms of bytes.</param>
        /// <param name="access">Whether you need write access to the view.</param>
        /// <returns>A MemoryMappedFileView instance representing the view.</returns>
        public MemoryMappedFileView CreateView(int offset, int size,
            MemoryMappedFileView.ViewAccess access)
        {
            if (this.access == FileAccess.ReadOnly && access == MemoryMappedFileView.ViewAccess.ReadWrite)
            {
                throw new ArgumentException("Only read access to views allowed on files without write access", "access");
            }
            if (offset < 0)
            {
                throw new ArgumentException("Offset must not be negative", "size");
            }
            if (size < 0)
            {
                throw new ArgumentException("Size must not be negative", "size");
            }
            IntPtr mappedView = NTKernel.MapViewOfFile(fileMapping, (uint)access, 0, (uint)offset, (uint)size);
            return new MemoryMappedFileView(mappedView, size, access);
        }

        #endregion
    }

    #endregion

    #endregion

    #region StringMappedItem

    /// <summary>
    /// String Mapped Item. The Memory mapped file for string share between process.
    /// </summary>
    public class StringMappedItem
    {
        #region Internal Variables

        private string _mappingName = string.Empty;
        private int _size = 1024;
        private MemoryMappedFile mmfRW = null;
        private MemoryMappedFileView viewRW = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        private StringMappedItem()
            : base()
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mappingNane">The mapping name.</param>
        public StringMappedItem(string mappingNane)
            : this(mappingNane, 1024)
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mappingNane">The mapping name.</param>
        /// <param name="size">The max size of mapping.</param>
        public StringMappedItem(string mappingNane, int size)
            : this()
        {
            _mappingName = mappingNane;
            _size = size;
            CreateInstance();
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~StringMappedItem()
        {
            Release();
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Create Instance
        /// </summary>
        private void CreateInstance()
        {
            if (_mappingName == string.Empty)
                return;
            if (_size <= 0)
                return;

            #region Create Memory Mapped file

            // create memory mapped file
            try
            {
                mmfRW = MemoryMappedFile.CreateFile(_mappingName,
                    MemoryMappedFile.FileAccess.ReadWrite, _size);
                viewRW = mmfRW.CreateView(0, _size,
                    MemoryMappedFileView.ViewAccess.ReadWrite);
            }
            catch //(Exception ex)
            {
                Release();
            }

            #endregion
        }
        /// <summary>
        /// Release
        /// </summary>
        private void Release()
        {
            if (mmfRW != null)
            {
                try
                {
                    mmfRW.Dispose();
                }
                catch { }
            }
            mmfRW = null;
            if (viewRW != null)
            {
                try
                {
                    viewRW.Dispose();
                }
                catch { }
            }
            viewRW = null;
        }
        /// <summary>
        /// Set Value
        /// </summary>
        /// <param name="value">The Value</param>
        private void SetValue(string value)
        {
            if (viewRW == null)
                CreateInstance();
            if (viewRW != null)
            {
                if (value == string.Empty)
                {
                    byte[] buffers = new byte[this.Size];
                    viewRW.WriteBytes(buffers, 0);
                }
                else
                {
                    byte[] dataBuffers =
                        System.Text.UTF8Encoding.UTF8.GetBytes(value);
                    byte[] buffers = new byte[this.Size]; // required empty buffer
                    // write name on empty buffer
                    Array.Copy(dataBuffers, 0, buffers, 0, dataBuffers.Length);
                    // write to File Mapping
                    viewRW.WriteBytes(buffers, 0);
                }
            }
        }
        /// <summary>
        /// Get Value.
        /// </summary>
        /// <returns>Returns value on current mapped file.</returns>
        private string GetValue()
        {
            if (viewRW == null)
                CreateInstance();
            string result = string.Empty;
            if (viewRW != null)
            {
                byte[] buffers = new byte[this.Size];
                viewRW.ReadBytes(buffers, 0);
                // convert to string
                result = System.Text.UTF8Encoding.UTF8.GetString(buffers);
                // remove all null ternimate
                result = result.Replace("\0", string.Empty);
            }
            return result.Trim();
        }
        /// <summary>
        /// Reset
        /// </summary>
        public void Reset()
        {
            SetValue(string.Empty);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Mapping Name.
        /// </summary>
        public string MappingName
        {
            get { return _mappingName; }
        }
        /// <summary>
        /// Gets size
        /// </summary>
        public int Size
        {
            get { return _size; }
        }
        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public string Value
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }

        #endregion
    }

    #endregion

    #region ShareManager

    /// <summary>
    /// Share Manager.
    /// </summary>
    public class ShareManager
    {
        #region Intenal Variables

        private Dictionary<string, StringMappedItem> _items =
            new Dictionary<string, StringMappedItem>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        internal ShareManager() : base() { }

        #endregion

        #region Private Methods

        private string GetShareItem(string mapName)
        {
            string result = string.Empty;
            string key = mapName.Trim().ToLower();
            if (key == string.Empty)
                return result;

            if (_items.ContainsKey(key) && null != _items[key])
            {
                result = _items[key].Value;
            }

            return result;
        }

        private void SetShareItem(string mapName, string value)
        {
            string key = mapName.Trim().ToLower();
            if (key == string.Empty)
                return;
            if (_items.ContainsKey(key))
            {
                if (null != _items[key])
                {
                    _items[key].Value = value;
                }
                else
                {
                    StringMappedItem item = new StringMappedItem(key);
                    item.Value = value;
                    _items[key] = item;
                }
            }
            else
            {
                StringMappedItem item = new StringMappedItem(key);
                item.Value = value;
                _items.Add(key, item);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets or sets share item by key.
        /// </summary>
        /// <param name="mapName">The unique mapped item key.</param>
        /// <returns>Returns value on mapped item.</returns>
        public string this[string mapName]
        {
            get { return GetShareItem(mapName); }
            set { SetShareItem(mapName, value); }
        }
        /// <summary>
        /// Create new share. No data write to mapped item.
        /// </summary>
        /// <param name="mapName">The unique mapped item key.</param>
        public void Create(string mapName)
        {
            string key = mapName.Trim().ToLower();
            if (key == string.Empty)
                return;
            if (!_items.ContainsKey(key))
            {
                StringMappedItem item = new StringMappedItem(key);
                _items.Add(key, item);
            }
        }
        /// <summary>
        /// Reset value on mapping item.
        /// </summary>
        /// <param name="mapName">The unique mapped item key.</param>
        public void Reset(string mapName)
        {
            string key = mapName.Trim().ToLower();
            if (key == string.Empty)
                return;
            if (_items.ContainsKey(key))
            {
                if (null != _items[key])
                {
                    _items[key].Reset();
                }
            }
        }

        #endregion
    }

    #endregion

    #region App Instance

    /// <summary>
    /// Single App Instance Helper Class. Please used App Instance as static variable if Notify Icon is used
    /// in program to solve problem with cannot find main form handle when stay in system tray.
    /// </summary>
    public class AppInstance
    {
        #region Internal variable

        private string _version = string.Empty;
        private string _minor = string.Empty;
        private string _build = string.Empty;
        private DateTime _lastUpdate = DateTime.MinValue;

        private bool bMoreInstance = false;
        private MemoryMappedFile mmfRW = null;
        private MemoryMappedFileView viewRW = null;

        #endregion

        #region API

        #region SW

        /// <summary>
        /// SW
        /// </summary>
        enum SW
        {
            /// <summary>
            /// SHOWNORMAL
            /// </summary>
            SHOWNORMAL = 1,
            /// <summary>
            /// SHOW
            /// </summary>
            SHOW = 5,
            /// <summary>
            /// RESTORE
            /// </summary>
            RESTORE = 9
        }

        #endregion

        // IsIconic
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        // ShowWindowAsync
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, SW showState);
        // SetForegroundWindow
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, SW flags);

        [DllImport("user32")]
        private static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        private static extern int RegisterWindowMessage(string message);

        [DllImport("User32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string strClassName, string strWindowName);
        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int ProcessId);

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="version">The Version information</param>
        /// <param name="minor">The Minor information</param>
        /// <param name="build">The Build information</param>
        /// <param name="lastUpdate">The Last Update information</param>
        public AppInstance(string version, string minor, string build, DateTime lastUpdate)
        {
            _version = version;
            _minor = minor;
            _build = build;
            _lastUpdate = lastUpdate;

            CheckInstance();
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public AppInstance()
        {
            CheckInstance();
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~AppInstance()
        {
            #region Free Memory mapped

            if (viewRW != null) viewRW.Dispose();
            viewRW = null;
            if (mmfRW != null) mmfRW.Dispose();
            mmfRW = null;

            #endregion
        }

        #endregion

        #region Check Instance

        private string GetMappingName(string processName)
        {
            string chkName = processName + "_single_app_instance_counter";

            if (_version.Trim().Length > 0) chkName += "x" + _version;
            if (_minor.Trim().Length > 0) chkName += "x" + _minor;
            if (_build.Trim().Length > 0) chkName += "x" + _build;
            if (_lastUpdate != DateTime.MinValue)
            {
                chkName += "x" + _lastUpdate.ToString("yyyyMMddHHmmss",
                    System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }

            return chkName;
        }

        private void CheckInstance()
        {
            // get the name of our process
            string proc = Process.GetCurrentProcess().ProcessName;

            #region Create Memory Mapped file

            // create memory mapped file
            try
            {
                mmfRW = MemoryMappedFile.CreateFile(GetMappingName(proc),
                    MemoryMappedFile.FileAccess.ReadWrite, 16);
                viewRW = mmfRW.CreateView(0, sizeof(int),
                    MemoryMappedFileView.ViewAccess.ReadWrite);
            }
            catch //(Exception ex)
            {
                if (mmfRW != null)
                {
                    try
                    {
                        mmfRW.Dispose();
                    }
                    catch { }
                }
                mmfRW = null;
                if (viewRW != null)
                {
                    try
                    {
                        viewRW.Dispose();
                    }
                    catch { }
                }
                viewRW = null;
            }

            #endregion

            // get the list of all processes by that name
            Process[] processes = Process.GetProcessesByName(proc);
            // if there is more than one process...
            if (processes.Length > 1)
            {
                #region Find Exists instance and Invoke it.

                // Assume there is our process, which we will terminate, 
                // and the other process, which we want to bring to the 
                // foreground. This assumes there are only two processes 
                // in the processes array, and we need to find out which 
                // one is NOT us.

                // get our process
                Process p = Process.GetCurrentProcess();
                int n = 0;        // assume the other process is at index 0
                // if this process id is OUR process ID...
                if (processes[0].Id == p.Id)
                {
                    // then the other process is at index 1
                    n = 1;
                }
                // get the window handle
                IntPtr hWnd = processes[n].MainWindowHandle;

                #endregion

                if (hWnd != IntPtr.Zero)
                {
                    // if iconic, we need to restore the window
                    if (IsIconic(hWnd))
                    {
                        ShowWindowAsync(hWnd, SW.RESTORE);
                        //ShowWindow(hWnd, SW.RESTORE);
                    }
                    // bring it to the foreground
                    SetForegroundWindow(hWnd);
                }
                else
                {
                    // Cannot find handle. use memory mapped file.
                    if (viewRW != null)
                    {
                        // Increase access number
                        int oldVal = viewRW.ReadInt32(0);
                        oldVal++;
                        viewRW.WriteInt32(oldVal, 0);
                    }
                }

                // exit our process
                bMoreInstance = true;

                return;
            }
        }

        #endregion

        #region Property

        /// <summary>
        /// Get to Check is More Instance is in memory
        /// </summary>
        public bool HasMoreInstance
        {
            get { return bMoreInstance; }
        }
        /// <summary>
        /// Check is application need activate. This property is true when another instance is execute.
        /// </summary>
        public bool NeedActivate
        {
            get
            {
                if (viewRW != null)
                {
                    int value = viewRW.ReadInt32(0);
                    if (value > 0)
                        return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Reset Activate. Please Call when the main form is activate.
        /// </summary>
        public void ResetActivate()
        {
            if (viewRW != null)
            {
                viewRW.WriteInt32(0, 0);
            }
        }

        #endregion
    }

    #endregion
}
