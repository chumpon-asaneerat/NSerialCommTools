#define ENAABLE_LOGS

#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-17
=================
- NLib IO Framework - NFolder.
  - Changes log code (used MethodBase).

======================================================================================================================
Update 2013-05-07
=================
- NLib IO Framework - NFolder.
  - NFolder class is move to seperate file.
  - CreateFolder method changed to Folder method.
  - Add supports indexer access sub folder.
  - Remove method CreateSubFolders.

======================================================================================================================
Update 2013-01-15
=================
- NLib IO Framework - NFolder.
  - Update code in Folders class by using new IOOperation class and 
    FilePermissionResult class

======================================================================================================================
Update 2012-12-25
=================
- NLib IO Framework - NFolder.
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

#endregion

namespace NLib.IO
{
    #region NFolder

    /// <summary>
    /// The NFolder class. This class provide wrapper function to access and work with
    /// application folder. This class is non serialized.
    /// </summary>
    public partial class NFolder
    {
        #region Internal Variables

        private DirectoryInfo _dir = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor. (Hide).
        /// </summary>
        private NFolder()
            : base()
        {
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The path.</param>
        public NFolder(string path)
            : this()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                _dir = new DirectoryInfo(path);
            }
            catch (Exception ex)
            {
                ex.Err(med);
            }
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~NFolder()
        {
            _dir = null;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns>Returns the hash code value.</returns>
        public override int GetHashCode()
        {
            return string.Format("{0}", this.FullName.ToLower()).GetHashCode();
        }
        /// <summary>
        /// Equals.
        /// </summary>
        /// <param name="obj">An object instance to compare.</param>
        /// <returns>Returns true if object is equals.</returns>
        public override bool Equals(object obj)
        {
            if (null == obj || !(obj.GetType() == this.GetType()))
                return false;
            return this.GetHashCode().Equals(obj.GetHashCode());
        }
        /// <summary>
        /// ToString.
        /// </summary>
        /// <returns>Returns string that represents the key pair value.</returns>
        public override string ToString()
        {
            string val = string.Empty;
            try
            {
                val = (!string.IsNullOrWhiteSpace(this.FullName)) ?
                    this.FullName : string.Empty;
            }
            catch { val = string.Empty; }

            return string.Format("{0}]", val);
        }

        #endregion

        #region Public Methods

        #region Folders

        /// <summary>
        /// Check parent and auto create if required.
        /// </summary>
        /// <returns>Returns true if parent folder is exists.</returns>
        private bool IsParentExists()
        {
            bool hasError = false;

            if (string.IsNullOrWhiteSpace(this.Parent))
            {
                // Parent not assigned
                return true;
            }

            if (!Folders.Exists(this.Parent))
            {
                // create if required.
                hasError = !Folders.Create(this.Parent).Success;
            }

            if (!hasError)
            {
                // grant if required.
                hasError = !Folders.Grant(this.Parent).Success;
            }
            else
            {
                // cannot create folder or grant permission
                hasError = true; // Has error
            }

            return !hasError;
        }
        /// <summary>
        /// Create the current folder.
        /// </summary>
        /// <returns>Returns true if folder is create and can grant access by current user.</returns>
        public bool Create()
        {
            bool hasError = false;

            if (!IsParentExists() ||
                string.IsNullOrWhiteSpace(this.FullName))
            {
                // Parnet not exists or no folder name.
                hasError = true;
            }
            else
            {
                // Parent Exist and File name may be OK
                if (!Folders.Exists(this.FullName))
                {
                    // create if required.
                    hasError = !Folders.Create(this.FullName).Success;
                }

                if (!hasError)
                {
                    // grant if required.
                    hasError = !Folders.Grant(this.FullName).Success;
                }
                else
                {
                    // cannot create folder or grant permission
                    hasError = true; // Has error
                }
            }

            return !hasError;
        }
        /// <summary>
        /// Access sub folders.
        /// </summary>
        /// <param name="path">The sub folder name.</param>
        /// <param name="autoCreate">True for auto create if not exists.</param>
        /// <returns>Returns NFolder's instance for specificed sub folder.</returns>
        public NFolder this[string path, bool autoCreate = false]
        {
            get
            {
                NFolder result = null;
                if (string.IsNullOrWhiteSpace(path))
                    return result;

                string subPathFullName = Path.Combine(this.FullName, path);

                result = new NFolder(subPathFullName);
                if (autoCreate)
                {
                    result.Create();
                }
                return result;
            }
        }
        /// <summary>
        /// Gets File System Infos.
        /// </summary>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchOption">The search option.</param>
        /// <returns>Returns array of file system in current folder.</returns>
        public FileSystemInfo[] GetFileSystemInfos(string searchPattern = "*",
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            FileSystemInfo[] results = null;

            if (null == _dir)
                results = new FileSystemInfo[] { };
            else results = _dir.GetFileSystemInfos(searchPattern, searchOption);

            return results;
        }
        /// <summary>
        /// Gets Files.
        /// </summary>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchOption">The search option.</param>
        /// <returns>Returns array of file in current folder.</returns>
        public FileInfo[] GetFiles(string searchPattern = "*",
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            FileInfo[] results = null;

            if (null == _dir)
                results = new FileInfo[] { };
            else results = _dir.GetFiles(searchPattern, searchOption);

            return results;
        }
        /// <summary>
        /// Gets Directories.
        /// </summary>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchOption">The search option.</param>
        /// <returns>Returns array of directory in current folder.</returns>
        public DirectoryInfo[] GetDirectories(string searchPattern = "*",
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            DirectoryInfo[] results = null;

            if (null == _dir)
                results = new DirectoryInfo[] { };
            else results = _dir.GetDirectories(searchPattern, searchOption);

            return results;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Parent full path's name.
        /// </summary>
        [Category("Folder")]
        [Description("Gets the Parent full path's name.")]
        public string Parent
        {
            get
            {
                if (null == _dir || null == _dir.Parent)
                    return string.Empty;
                return _dir.Parent.FullName;
            }
        }
        /// <summary>
        /// Gets the Path's name. This is the sub folder in parent folder.
        /// </summary>
        [Category("Folder")]
        [Description("Gets the Path's name. This is the sub folder in parent folder.")]
        public string Name
        {
            get
            {
                if (null != _dir)
                {
                    return _dir.Name;
                }
                else return string.Empty;
            }
        }
        /// <summary>
        /// Gets the Full Path Name.
        /// </summary>
        [Category("Folder")]
        [Description("Gets the Full Path Name.")]
        public string FullName
        {
            get
            {
                string result = string.Empty;
                if (null != _dir)
                {
                    result = _dir.FullName;
                }
                return result;
            }
        }
        /// <summary>
        /// Checks is folder exists.
        /// </summary>
        [Category("Folder")]
        [Description("Checks is folder exists.")]
        public bool IsExists
        {
            get
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                bool result = false;
                try
                {
                    result = Directory.Exists(this.FullName);
                }
                catch (Exception ex)
                {
#if ENAABLE_LOGS
                    ex.Err(med);
#else
                    Console.WriteLine(ex);
#endif
                    result = false;
                }
                return result;
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Create new folder access object.
        /// </summary>
        /// <param name="path">The folder full path name.</param>
        /// <param name="createIfNotExits">Auto create if not exists.</param>
        /// <returns>Returns new NFolder instance for specificed path.</returns>
        public static NFolder Folder(string path, bool createIfNotExits = false)
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            NFolder inst = null;
            try
            {
                inst = new NFolder(path);
                if (null != inst)
                {
                    if (createIfNotExits)
                    {
                        inst.Create();
                    }
                }
            }
            catch (Exception ex)
            {
#if ENAABLE_LOGS
                ex.Err(med);
#else
                Console.WriteLine(ex);
#endif
            }

            return inst;
        }

        #endregion
    }

    #endregion
}