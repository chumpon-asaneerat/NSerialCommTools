#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-10-14
=================
- NLib IO Framework - Folders.
  - Remove Grant methods (that has string parameter) and Used WellKnownSid.WorldSid instead
    of EveryOne.
  - Add method to list avaliable groups that can access folder.
  - Fixed bug in Grant methods and add Identity parameter with default is null (NT User).

======================================================================================================================
Update 2013-01-15
=================
- NLib IO Framework - Folders.
  - Update code in Folders class by using new IOOperation class and 
    FilePermissionResult class

======================================================================================================================
Update 2012-12-25
=================
- NLib IO Framework - Folders.
  - Add NFolder class. This class provide wrapper function to access and work with
    application folder. This class is non serialized.

======================================================================================================================
Update 2011-12-15
=================
- NLib IO Framework - Folders utility.
  - Add Folders class. These class provide static methods for working with local folders
    which contains below methods.
    - Create method. Used for create new folder.
    - Delete method. Used for delete exists folder.
    - Exists method. Used for checks the folder is already exists.
    - Grant method. Used for grant permission to allow all users access and manages the
      folder and it's contents.
    - Merge some functions from Paths class from GFA40

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using NLib;
using NLib.IO.Security; // for UAC
using System.Security.Principal;

#endregion

namespace NLib.IO
{
    #region Folders

    /// <summary>
    /// Folders utility class.
    /// </summary>
    public sealed class Folders
    {
        #region Static class to acccess common folders

        #region OS

        /// <summary>
        /// Access various Window's common system folders.
        /// </summary>
        public sealed class OS
        {
            #region Constructor

            /// <summary>
            /// Constructor.
            /// </summary>
            private OS() : base() { }

            #endregion

            #region Public Methods (static)

            /// <summary>
            /// Get Wellknown Windows's directories.
            /// </summary>
            /// <returns></returns>
            public static List<KeyValue> GetDirectories()
            {
                List<KeyValue> results = new List<KeyValue>();
                // Windows
                results.Add(new KeyValue("System", OS.SystemDirectory));
                results.Add(new KeyValue("Windows", OS.Windows));
                results.Add(new KeyValue("ProgramFiles", OS.ProgramFiles));
                results.Add(new KeyValue("CommonProgramFiles", OS.CommonProgramFiles));
                results.Add(new KeyValue("NETRuntime", OS.NETRuntime));

                return results;
            }

            #endregion

            #region Properties (static)

            /// <summary>
            /// Get System Directory.
            /// </summary>
            public static string SystemDirectory
            {
                get
                {
                    return Environment.SystemDirectory;
                }
            }
            /// <summary>
            /// Get Windows Directory.
            /// </summary>
            public static string Windows
            {
                get
                {
                    return Environment.GetFolderPath(
                        Environment.SpecialFolder.Windows);
                }
            }
            /// <summary>
            /// Get Program Files Directory.
            /// </summary>
            public static string ProgramFiles
            {
                get
                {
                    return Environment.GetFolderPath(
                        Environment.SpecialFolder.ProgramFiles);
                }
            }
            /// <summary>
            /// Get Common Program Files Directory.
            /// </summary>
            public static string CommonProgramFiles
            {
                get
                {
                    return Environment.GetFolderPath(
                        Environment.SpecialFolder.CommonProgramFiles);
                }
            }
            /// <summary>
            /// Gets the directory that .NET Runtime is installed. 
            /// Used for access .NET framework tools.
            /// </summary>
            public static string NETRuntime
            {
                get
                {
                    return System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
                }
            }

            #endregion
        }

        #endregion

        #region Users

        /// <summary>
        /// Access various Common User's folders.
        /// </summary>
        public sealed class Users
        {
            #region Constructor

            /// <summary>
            /// Constructor.
            /// </summary>
            private Users() : base() { }

            #endregion

            #region Public Methods (static)

            /// <summary>
            /// Get Wellknown User's directories.
            /// </summary>
            /// <returns></returns>
            public static List<KeyValue> GetDirectories()
            {
                List<KeyValue> results = new List<KeyValue>();
                // Users
                results.Add(new KeyValue("Desktop", Users.Desktop));
                results.Add(new KeyValue("StartMenu", Users.StartMenu));
                results.Add(new KeyValue("Startup", Users.Startup));
                results.Add(new KeyValue("MyDocuments", Users.MyDocuments));
                results.Add(new KeyValue("LocalData", Users.LocalData));
                results.Add(new KeyValue("RoamingData", Users.RoamingData));

                return results;
            }

            #endregion

            #region Properties (static)

            /// <summary>
            /// Get Desktop Directory.
            /// </summary>
            public static string Desktop
            {
                get
                {
                    return System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.Desktop);
                }
            }
            /// <summary>
            /// Get Start Menu Directory.
            /// </summary>
            public static string StartMenu
            {
                get
                {
                    return System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.StartMenu);
                }
            }
            /// <summary>
            /// Get User's Startup Program Directory.
            /// </summary>
            public static string Startup
            {
                get
                {
                    return System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.Startup);
                }
            }
            /// <summary>
            /// Get the My Documents directory.
            /// This directory is user's "My Documents" folder.
            /// </summary>
            public static string MyDocuments
            {
                get
                {
                    return System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.MyDocuments);
                }
            }
            /// <summary>
            /// Get the Local Application Data Directory.
            /// This directory used for stores app data for users on that machine and 
            /// use for non-roaming users or for machine-specific config.
            /// This is the directory for the current user that is only available 
            /// when logged on to this machine. Note Micorsoft expert is recommend 
            /// to used this directory to keep application data and config.
            /// </summary>
            public static string LocalData
            {
                get
                {
                    return System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.LocalApplicationData);
                }
            }
            /// <summary>
            /// Get the Roaming Application Data Directory.
            /// This directory used for stores app data for that user and 
            /// can be used with roaming profiles. This is the directory for the current user, 
            /// shared by all machines on the network. 
            /// </summary>
            public static string RoamingData
            {
                get
                {
                    return System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.ApplicationData);
                }
            }

            #endregion
        }

        #endregion

        #region Locals

        /// <summary>
        /// Access various Local application's common folders.
        /// </summary>
        public sealed class Locals
        {
            #region Constructor

            /// <summary>
            /// Constructor.
            /// </summary>
            private Locals() : base() { }

            #endregion

            #region Public Methods (static)

            /// <summary>
            /// Get Wellknown Application's directories.
            /// </summary>
            /// <returns></returns>
            public static List<KeyValue> GetDirectories()
            {
                List<KeyValue> results = new List<KeyValue>();
                // Application
                results.Add(new KeyValue("CommonAppData", Locals.CommonAppData));

                return results;
            }

            #endregion

            #region Properties (static)

            /// <summary>
            /// Get the Common Application Data Directory. 
            /// This directory used for stores app data common ALL users on that machine like 
            /// (DB connection information, logging settings, etc).
            /// This is the directory for storing information that is shared by all users 
            /// on all machines. 
            /// </summary>
            public static string CommonAppData
            {
                get
                {
                    return Environment.GetFolderPath(
                        Environment.SpecialFolder.CommonApplicationData);
                }
            }

            #endregion
        }

        #endregion

        #region Assemblies

        /// <summary>
        /// Access various Assemblies information.
        /// </summary>
        public sealed class Assemblies
        {
            #region Constructor

            /// <summary>
            /// Constructor.
            /// </summary>
            private Assemblies() : base() { }

            #endregion

            #region Public Methods (Static)

            /// <summary>
            /// Get Wellknown Application's directories.
            /// </summary>
            /// <returns></returns>
            public static List<KeyValue> GetDirectories()
            {
                List<KeyValue> results = new List<KeyValue>();
                // Assemblies
                results.Add(new KeyValue("CurrentExecutingAssembly", Assemblies.CurrentExecutingAssembly));
                results.Add(new KeyValue("FullName", Assemblies.FullName));
                results.Add(new KeyValue("FileName", Assemblies.FileName));
                results.Add(new KeyValue("FileNameOnly", Assemblies.FileNameOnly));

                return results;
            }
            /// <summary>
            /// Get Assembly Directory.
            /// </summary>
            /// <param name="callingAssembly">The calling assembly.</param>
            /// <returns>Return assembly full directory's name.</returns>
            public static string GetAssemblyDirectory(Assembly callingAssembly)
            {
                if (callingAssembly == null)
                {
                    return string.Empty;
                }
                else return System.IO.Path.GetDirectoryName(callingAssembly.Location);
            }
            /// <summary>
            /// Get Assembly File Name without extension (not inclide path).
            /// </summary>
            /// <param name="callingAssembly">The calling assembly.</param>
            /// <returns>Return assembly full file's name without extension.</returns>
            public static string GetAssemblyFileNameWithoutExtension(Assembly callingAssembly)
            {
                if (callingAssembly == null)
                {
                    return string.Empty;
                }
                else return System.IO.Path.GetFileNameWithoutExtension(callingAssembly.Location);
            }
            /// <summary>
            /// Get Assembly File Name with extension (not inclide path).
            /// </summary>
            /// <param name="callingAssembly">The calling assembly.</param>
            /// <returns>Return assembly full file's name.</returns>
            public static string GetAssemblyFileName(Assembly callingAssembly)
            {
                if (callingAssembly == null)
                {
                    return string.Empty;
                }
                else return System.IO.Path.GetFileName(callingAssembly.Location);
            }

            #endregion

            #region Properties (Static)

            /// <summary>
            /// Get Current Executing Assembly Directory.
            /// </summary>
            public static string CurrentExecutingAssembly
            {
                get
                {
                    return System.IO.Path.GetDirectoryName(
                        Assembly.GetExecutingAssembly().Location);
                }
            }
            /// <summary>
            /// Get Application Location Directory (where the .exe is stored).
            /// For .dll used GetApplicationSettingDirectory instead.
            /// </summary>
            public static string FullName
            {
                get
                {
                    #region test assembly

                    Assembly assem = null;
                    try { assem = Assembly.GetEntryAssembly(); }
                    catch { assem = null; }
                    if (assem == null)
                    {
                        try { assem = Assembly.GetCallingAssembly(); }
                        catch { assem = null; }
                    }
                    if (assem == null)
                    {
                        try { assem = Assembly.GetExecutingAssembly(); }
                        catch { assem = null; }
                    }

                    #endregion

                    if (assem != null)
                    {
                        try { return System.IO.Path.GetDirectoryName(assem.Location); }
                        catch
                        {
                            return string.Empty;
                        }
                    }
                    else return string.Empty;
                }
            }
            /// <summary>
            /// Get Application File Name (.exe) Without Extension.
            /// </summary>
            public static string FileNameOnly
            {
                get
                {
                    Assembly assem = null;
                    try { assem = Assembly.GetEntryAssembly(); }
                    catch { assem = null; }
                    if (assem == null)
                    {
                        try { assem = Assembly.GetCallingAssembly(); }
                        catch { assem = null; }
                    }
                    if (assem == null)
                    {
                        try { assem = Assembly.GetExecutingAssembly(); }
                        catch { assem = null; }
                    }
                    if (assem != null)
                    {
                        try { return System.IO.Path.GetFileNameWithoutExtension(assem.Location); }
                        catch
                        {
                            return string.Empty;
                        }
                    }
                    else return string.Empty;
                }
            }
            /// <summary>
            /// Get Application File Name (.exe) with Extension.
            /// </summary>
            public static string FileName
            {
                get
                {
                    Assembly assem = null;
                    try { assem = Assembly.GetEntryAssembly(); }
                    catch { assem = null; }
                    if (assem == null)
                    {
                        try { assem = Assembly.GetCallingAssembly(); }
                        catch { assem = null; }
                    }
                    if (assem == null)
                    {
                        try { assem = Assembly.GetExecutingAssembly(); }
                        catch { assem = null; }
                    }
                    if (assem != null)
                    {
                        try { return System.IO.Path.GetFileName(assem.Location); }
                        catch
                        {
                            return string.Empty;
                        }
                    }
                    else return string.Empty;
                }
            }

            #endregion
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private Folders() : base() { }

        #endregion

        #region Public (static methods)

        #region Create

        /// <summary>
        /// Create Folder with can access by all users.
        /// </summary>
        /// <param name="pathName">The path or directory's name.</param>
        /// <returns>Returns result of operation.</returns>
        public static IOOperationResult Create(string pathName)
        {
            IOOperationResult result = new IOOperationResult();

            if (Directory.Exists(pathName))
            {
                #region Folder already exists

                result.Result = IOStatus.Success;
                result.Err = null;

                #endregion
            }
            else
            {
                #region Folder not exists

                bool hasErr = false;
                try
                {
                    // create directory and it's sub directories.
                    Directory.CreateDirectory(pathName);
                }
                catch (Exception ex)
                {
                    // Failed.
                    hasErr = true;
                    result.Result = IOStatus.Error;
                    result.Err = ex;
                }
                // Rechecks if folders is created.
                if (!hasErr && Directory.Exists(pathName))
                {
                    result.Result = IOStatus.Success;
                    result.Err = null;
                }

                #endregion
            }

            return result;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete Folder.
        /// </summary>
        /// <param name="pathName">The path or directory's name.</param>
        /// <returns>Returns result of operation.</returns>
        public static IOOperationResult Delete(string pathName)
        {
            IOOperationResult result = new IOOperationResult();

            if (!Directory.Exists(pathName))
            {
                #region Directory is not exists. Assume as success

                result.Result = IOStatus.Success;
                result.Err = null;

                #endregion
            }
            else
            {
                #region Directory exists. Try to delete

                bool hasErr = false;
                try
                {
                    Directory.Delete(pathName);
                }
                catch (Exception ex)
                {
                    // Failed.
                    hasErr = true;
                    result.Result = IOStatus.Error;
                    result.Err = ex;
                }
                // rechecks and generate result if success.
                if (!hasErr && !Directory.Exists(pathName))
                {
                    result.Result = IOStatus.Success;
                    result.Err = null;
                }

                #endregion
            }

            return result;
        }

        #endregion

        #region Exists

        /// <summary>
        /// Checks is directory is exists.
        /// </summary>
        /// <param name="pathName">The path or directory's name.</param>
        /// <returns>Returns true if path or directory is exists.</returns>
        public static bool Exists(string pathName)
        {
            return Directory.Exists(pathName);
        }

        #endregion

        #region Grant

        /// <summary>
        /// Grant permission to public access to target path and it's sub directories.
        /// </summary>
        /// <param name="pathName">The target path name.</param>
        /// <param name="identity">The target identity default is NT User.</param>
        /// <returns>Returns result of operation.</returns>
        public static IOOperationResult Grant(string pathName,
            SecurityIdentifier identity = null)
        {
            IOOperationResult result = new IOOperationResult();

            bool hasError = false;
            if (!Directory.Exists(pathName))
            {
                #region Directory not found

                hasError = true;
                result.Result = IOStatus.Error;
                result.Err = new Exception("Target folders not exists.");

                #endregion
            }
            else
            {
                FolderPermissionResult permitResult = null;
                permitResult = UAC.GetFolderPermission(pathName).GetPermissions();
                if (null != permitResult && permitResult.IsPublic)
                {
                    SecurityIdentifier tempIden = null;
                    if (null == identity)
                        tempIden = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
                    else tempIden = identity;
                    // Try to grant access
                    permitResult = UAC.GetFolderPermission(pathName).GrantPermissions(tempIden);
                }
                if (null == permitResult || !permitResult.IsPublic)
                {
                    hasError = true;
                    result.Result = IOStatus.Error;
                    result.Err = new Exception("Cannot grant folder permission to public access.");
                }
            }

            if (!hasError)
            {
                result.Result = IOStatus.Success;
                result.Err = null;
            }

            return result;
        }

        #endregion

        #region From Paths class in GFA40

        /// <summary>
        /// Get the directory's name from specificed path string.
        /// </summary>
        /// <param name="path">The path string.</param>
        /// <returns>Returns the directory information for the specificed path string.</returns>
        public static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }
        /// <summary>
        /// Get the file's name from specificed path string.
        /// </summary>
        /// <param name="path">The path string.</param>
        /// <returns>Returns file's name with extension from specificed path string.</returns>
        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
        /// <summary>
        /// Get the file's name without extenstion from specificed path string.
        /// </summary>
        /// <param name="path">The path string.</param>
        /// <returns>Returns file's name without extenstion from specificed path string.</returns>
        public static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }
        /// <summary>
        /// Get the file's extension from specificed path string.
        /// </summary>
        /// <param name="path">The path string.</param>
        /// <returns>Returns file's extension from specificed path string.</returns>
        public static string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }
        /// <summary>
        /// Combine two path string.
        /// </summary>
        /// <param name="path1">The first path.</param>
        /// <param name="path2">The second path.</param>
        /// <returns>Returns the combined path.</returns>
        public static string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        #endregion

        #endregion
    }

    #endregion
}