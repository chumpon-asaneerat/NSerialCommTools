#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-05-14
=================
- NLib Security Framework - User Access Control (UAL) Security.
  - UAC change method name Files() to GetFilePermission(). 
  - UAC change method name Folders() to GetFolderPermission(). 
  - Fixed bug in Grant methods and add Identity parameter with default is null (NT User).
  - Add internal class BuildInGroups for Easy to setup Identifier (Currently supports
    NT Domain/Administrators, NT Domain/Users and EveryOne).
  - Fixed bug in Grant methods to allow Full Control/Read/Write...etc except special.

======================================================================================================================
Update 2013-01-15
=================
- NLib Security Framework - User Access Control (UAL) Security.
  - Change namespace from NLib.Security to NLib.IO.Security.
  - Change class name FolderPermissions to FolderPermissionResult
  - Change class name FilePermissions to FilePermissionResult
  - Add Initialized value in constructor for FolderPermissionResult and 
    FilePermissionResult class.
  - Add some check null access vairable in LogOnUser class.
  - Add check null access variable in GrantPermissions methods in FolderPermission class.
  - Add check null access variable in GrantPermissions methods in FilePermission class.

======================================================================================================================
Update 2011-12-15
=================
- NLib Security Framework - User Access Control (UAL) Security.
  - Add UAC class. These class is acts as single access point to working with security
    issue on users, folders and files. The UAC class is actually wrapper class that
    provide methods and properties to access FolderPermission class, FilePermission class
    and LogOnUser class.
  - Add LoOnUser class. This class is keep information about current log on user.
  - Add FolderPermission class. This class is provide 2 important methods. GetPermissions
    and GrantPermissions both class will returns instance of FolderPermissions class that
    used for checks the current permissions on the folder.
  - Add FilePermission class. This class is provide 2 important methods. GetPermissions
    and GrantPermissions both class will returns instance of FilePermissions class that
    used for checks the current permissions on the file.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Principal;
using System.Security.AccessControl;

#endregion

namespace NLib.IO.Security
{
    #region LogOnUser

    /// <summary>
    /// LogOn User.
    /// </summary>
    public sealed class LogOnUser
    {
        #region Internal Variables

        private WindowsIdentity _user = null;
        private List<string> _userGroups = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        internal LogOnUser()
            : base()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~LogOnUser()
        {
            if (null != _userGroups)
            {
                _userGroups.Clear();
            }
            _userGroups = null;
        }

        #endregion

        #region Private Methods

        private void RefreshMemberGroups()
        {
            if (null == _userGroups)
                _userGroups = new List<string>();
            else _userGroups.Clear();

            if (null == this.Current)
                return;
            IdentityReferenceCollection grpRefs = this.Current.Groups;
            foreach (IdentityReference grpRef in grpRefs)
            {
                if (grpRef == null)
                    continue;

                string ntGroup = string.Empty;
                try
                {
                    ntGroup = grpRef.Translate(typeof(NTAccount)).Value;
                }
                catch (Exception)
                {
                    //Console.WriteLine(ex);
                    ntGroup = grpRef.Value; // used original SID value.
                }
                _userGroups.Add(ntGroup);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Current User Identity.
        /// </summary>
        public WindowsIdentity Current
        {
            get
            {
                WindowsIdentity currUser = WindowsIdentity.GetCurrent();
                if (_user != currUser)
                {
                    _user = currUser;
                    // clear original user groups
                    if (null != _userGroups)
                        _userGroups.Clear();
                    else _userGroups = null;
                }
                return _user;
            }
        }
        /// <summary>
        /// Gets Current User Log On Name.
        /// </summary>
        public string LogOnName
        {
            get { return (this.Current != null) ? this.Current.Name : "Unknown"; }
        }
        /// <summary>
        /// Gets User Authentication Type.
        /// </summary>
        public string AuthenticationType
        {
            get { return (null != this.Current) ? this.Current.AuthenticationType : "Unknown"; }
        }
        /// <summary>
        /// Checks is Anonymous user.
        /// </summary>
        public bool IsAnonymous
        {
            get { return (null != this.Current) ? this.Current.IsAnonymous : false; }
        }
        /// <summary>
        /// Checks is Authenticated user.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return (null != this.Current) ? this.Current.IsAuthenticated : false; }
        }
        /// <summary>
        /// Checks is Guest user.
        /// </summary>
        public bool IsGuest
        {
            get { return (null != this.Current) ? this.Current.IsGuest : false; }
        }
        /// <summary>
        /// Checks is System user.
        /// </summary>
        public bool IsSystem
        {
            get { return (null != this.Current) ? this.Current.IsSystem : false; }
        }
        /// <summary>
        /// Gets Current user's member groups
        /// </summary>
        public List<string> Members
        {
            get
            {
                if (null == _userGroups || _userGroups.Count <= 0)
                {
                    RefreshMemberGroups();
                }
                return _userGroups;
            }
        }

        #endregion
    }

    #endregion

    #region FolderPermissionResult

    /// <summary>
    /// Folder Permission Result. 
    /// The result class for folder permission operations.
    /// </summary>
    public sealed class FolderPermissionResult
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        internal FolderPermissionResult()
            : base()
        {
            this.PathName = string.Empty;
            this.OwnerShip = string.Empty;
            this.ChangePermission = false;
            this.Delete = false;
            this.ListDirectory = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets (or internal sets) target folder.
        /// </summary>
        public string PathName { get; internal set; }
        /// <summary>
        /// Gets the owner of file or folder.
        /// </summary>
        public string OwnerShip { get; internal set; }
        /// <summary>
        /// Gets (or internal sets) that current user can chance permisssion 
        /// for file or folder.
        /// </summary>
        public bool ChangePermission { get; internal set; }
        /// <summary>
        /// Gets (or internal sets) that current user can create file or folder.
        /// </summary>
        public bool Create { get; internal set; }
        /// <summary>
        /// Gets (or internal sets) that current user can delete file or folder.
        /// </summary>
        public bool Delete { get; internal set; }
        /// <summary>
        /// Gets (or internal sets) can get list the contents of target folder.
        /// </summary>
        public bool ListDirectory { get; internal set; }
        /// <summary>
        /// Checks if folder exists.
        /// </summary>
        public bool Exists
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(PathName) && Directory.Exists(PathName));
            }
        }
        /// <summary>
        /// Checks is folder allow to create file/folder and sub-directories
        /// and allow to delete file/folder and sub-directories and 
        /// allow to list its file and sub-directories contents.
        /// </summary>
        public bool IsPublic
        {
            get { return this.Create && this.Delete && this.ListDirectory; }
        }
        /// <summary>
        /// Gets avaliable groups.
        /// </summary>
        public List<string> Groups { get; internal set; }

        #endregion
    }

    #endregion

    #region FolderPermission

    /// <summary>
    /// Folder Permission.
    /// </summary>
    public sealed class FolderPermission
    {
        #region Internal Variables

        private string _pathName = string.Empty;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        internal FolderPermission() { }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~FolderPermission()
        {
        }

        #endregion

        #region Private Methods

        #region GetOwnerShip

        /// <summary>
        /// Gets current ownership on target file system object.
        /// </summary>
        /// <returns>Returns NTUser that take ownership on specificed file system object.</returns>
        private string GetOwnerShip()
        {
            string _ownerShip = string.Empty;
            DirectoryInfo di = null;
            try
            {
                di = new DirectoryInfo(_pathName);
            }
            catch
            {
                di = null;
            }
            if (null != di && System.IO.Directory.Exists(_pathName))
            {
                DirectorySecurity dsec = di.GetAccessControl();

                IdentityReference downer = (null != dsec) ?
                    dsec.GetOwner(typeof(NTAccount)) : null;

                if (null != downer)
                {
                    _ownerShip = downer.Translate(typeof(NTAccount)).Value;
                }
                else
                {
                    _ownerShip = string.Empty;
                }
            }

            return _ownerShip;
        }

        #endregion

        #region GetUserGroups

        /// <summary>
        /// Gets avaliable user's groups that can access folder.
        /// </summary>
        /// <returns>Returns avaliable user's groups that can access folder.</returns>
        private List<string> GetUserGroups()
        {
            List<string> results = new List<string>();
            DirectorySecurity security = Directory.GetAccessControl(_pathName);
            AuthorizationRuleCollection acl = security.GetAccessRules(
               true, true, typeof(System.Security.Principal.NTAccount));
            foreach (FileSystemAccessRule ace in acl)
            {
                if (null == ace || null == ace.IdentityReference ||
                    string.IsNullOrWhiteSpace(ace.IdentityReference.Value))
                    continue;
                string account = ace.IdentityReference.Value;
                results.Add(account);
            }

            return results;
        }

        #endregion

        #region CheckPermissions

        /// <summary>
        /// Check Access Control rights for specificed user on target file or folder.
        /// </summary>
        /// <param name="requiredPermissions">
        /// The required permissions for target file or folder.
        /// </param>
        /// <returns>Returns true if required permissions is allow.</returns>
        private bool CheckPermissions(FileSystemRights requiredPermissions)
        {
            #region Check file info and load user rules each time method call

            DirectoryInfo di = null;
            try
            {
                di = new DirectoryInfo(_pathName);
            }
            catch
            {
                di = null;
            }

            IEnumerable<AuthorizationRule> _userRules = null;
            if (null != di && System.IO.Directory.Exists(_pathName))
            {
                DirectorySecurity dsec = di.GetAccessControl();

                AuthorizationRuleCollection _acl = (null != dsec) ?
                    dsec.GetAccessRules(true, true, typeof(SecurityIdentifier)) : null;

                // gets rules that concern the user and his groups
                if (null != this.Current && null != _acl)
                {
                    _userRules = from AuthorizationRule rule in _acl
                                 where this.Current.User.Equals(rule.IdentityReference) ||
                                       this.Current.Groups.Contains(rule.IdentityReference)
                                 select rule;
                }
            }
            else
            {
                _userRules = null;
            }

            #endregion

            #region Compare is allow or deny

            FileSystemRights denyRights = 0;
            FileSystemRights allowRights = 0;

            if (null != _userRules)
            {
                // iterates on rules to compute denyRights and allowRights
                foreach (FileSystemAccessRule rule in _userRules)
                {
                    if (null == rule)
                        continue;

                    if (rule.AccessControlType.Equals(AccessControlType.Deny))
                    {
                        denyRights = denyRights | rule.FileSystemRights;
                    }
                    else if (rule.AccessControlType.Equals(AccessControlType.Allow))
                    {
                        allowRights = allowRights | rule.FileSystemRights;
                    }
                }

                // allowRights = allowRights - denyRights
                allowRights = allowRights & ~denyRights;
            }

            #endregion

            // are rights sufficient?
            return (allowRights & requiredPermissions) == requiredPermissions;
        }

        #endregion

        #endregion

        #region Public Methods

        #region GetPermissions

        /// <summary>
        /// Gets Folder access permissions.
        /// </summary>
        /// <returns>Returns the avaliable permissions for target folder.</returns>
        public FolderPermissionResult GetPermissions()
        {
            FolderPermissionResult result = new FolderPermissionResult();
            // Owner and Name
            result.OwnerShip = this.GetOwnerShip();
            result.PathName = this.PathName;
            // Change permission
            result.ChangePermission = CheckPermissions(FileSystemRights.ChangePermissions);
            // Folder related permissions
            result.Create = CheckPermissions(
                // right to create files
                FileSystemRights.CreateFiles |
                // right to create folder
                FileSystemRights.CreateDirectories
                );
            result.Delete = CheckPermissions(
                // right to delete folder or file.
                FileSystemRights.Delete |
                // right to delete target folder and its sub directories and files
                // contains in the folder.
                FileSystemRights.DeleteSubdirectoriesAndFiles
                );
            result.ListDirectory = CheckPermissions(
                // right to list contents of directory
                FileSystemRights.ListDirectory
                );
            // Gets Groups that can access folder.
            result.Groups = GetUserGroups();

            return result;
        }

        #endregion

        #region GrantPermissions

        /// <summary>
        /// Grants access premissions for folder with default FileSystemRights for
        /// public users.
        /// </summary>
        /// <param name="identity">The identity like Everyone or Users or User's identity name.</param>
        /// <returns>Returns premissions after grant to rechecked is grant success.</returns>
        public FolderPermissionResult GrantPermissions(IdentityReference identity)
        {
            // Grant rights
            // Create a new DirectoryInfo object.
            System.IO.DirectoryInfo dInfo = null;
            try
            {
                dInfo = new System.IO.DirectoryInfo(_pathName);
            }
            catch { dInfo = null; }

            // Get a DirectorySecurity object that represents the 
            // current security settings.
            DirectorySecurity dSecurity = (null != dInfo) ?
                dInfo.GetAccessControl() : null;

            if (null == dSecurity)
                return null; // detected null 

            // set rule to allow to change permission
            dSecurity.AddAccessRule(new FileSystemAccessRule(
                identity, 
                FileSystemRights.FullControl |
                FileSystemRights.ChangePermissions |
                FileSystemRights.Modify |
                FileSystemRights.Synchronize,
                InheritanceFlags.ContainerInherit |
                InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow));

            // Add the FileSystemAccessRule to the security settings. 
            dSecurity.AddAccessRule(new FileSystemAccessRule(
                identity,
                FileSystemRights.CreateDirectories |
                FileSystemRights.CreateFiles |
                FileSystemRights.Delete |
                FileSystemRights.DeleteSubdirectoriesAndFiles |
                FileSystemRights.ListDirectory,
                InheritanceFlags.ContainerInherit |
                InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow));

            // Set the new access settings.
            if (null != dInfo)
                dInfo.SetAccessControl(dSecurity);

            return GetPermissions();
        }
        /// <summary>
        /// Grants access premissions for folder.
        /// </summary>
        /// <param name="identity">The identity like Everyone or Users or User's identity name.</param>
        /// <param name="rightsToGrant">The FileSystemRights to grants.</param>
        /// <returns>Returns premissions after grant to rechecked is grant success.</returns>
        public FolderPermissionResult GrantPermissions(IdentityReference identity,
            FileSystemRights rightsToGrant)
        {
            // Grant rights
            // Create a new DirectoryInfo object.
            System.IO.DirectoryInfo dInfo = null;
            try
            {
                dInfo = new System.IO.DirectoryInfo(_pathName);
            }
            catch { dInfo = null; }

            // Get a DirectorySecurity object that represents the 
            // current security settings.
            DirectorySecurity dSecurity = (null != dInfo) ?
                dInfo.GetAccessControl() : null;

            if (null == dSecurity)
                return null; // detected null 

            // Add the FileSystemAccessRule to the security settings. 
            dSecurity.AddAccessRule(new FileSystemAccessRule(
                identity, rightsToGrant |
                FileSystemRights.FullControl |
                FileSystemRights.ChangePermissions | 
                FileSystemRights.Modify |
                FileSystemRights.Synchronize,
                InheritanceFlags.ContainerInherit |
                InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow));

            // Set the new access settings.
            if (null != dInfo)
                dInfo.SetAccessControl(dSecurity);

            return GetPermissions();
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Current User Identity.
        /// </summary>
        public WindowsIdentity Current
        {
            get
            {
                return WindowsIdentity.GetCurrent();
            }
        }
        /// <summary>
        /// Gets or sets target path name to checks or grants permission or rights.
        /// </summary>
        public string PathName
        {
            get { return _pathName; }
            set
            {
                if (_pathName != value)
                {
                    _pathName = value;
                }
            }
        }
        /// <summary>
        /// Checks if folder exists.
        /// </summary>
        public bool Exists
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(_pathName) && Directory.Exists(_pathName));
            }
        }

        #endregion
    }

    #endregion

    #region FilePermissionResult

    /// <summary>
    /// File Permission Result.
    /// The result class for file permission operations.
    /// </summary>
    public sealed class FilePermissionResult
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        internal FilePermissionResult()
            : base()
        {
            this.FileName = string.Empty;
            this.OwnerShip = string.Empty;
            this.ChangePermission = false;
            this.Delete = false;
            this.Read = false;
            this.Write = false;
            this.Execute = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets (or internal sets) target file.
        /// </summary>
        public string FileName { get; internal set; }
        /// <summary>
        /// Gets the owner of file or folder.
        /// </summary>
        public string OwnerShip { get; internal set; }
        /// <summary>
        /// Gets (or internal sets) that current user can chance permisssion 
        /// for file or folder.
        /// </summary>
        public bool ChangePermission { get; internal set; }
        /// <summary>
        /// Gets (or internal sets) that current user can delete file or folder.
        /// </summary>
        public bool Delete { get; internal set; }
        /// <summary>
        /// Gets (or internal sets) that current user can read file.
        /// </summary>
        public bool Read { get; internal set; }
        /// <summary>
        /// Gets (or internal sets) that current user can create/append/write file.
        /// </summary>
        public bool Write { get; internal set; }
        /// <summary>
        /// Gets (or internal sets) that current user can read and execute file.
        /// </summary>
        public bool Execute { get; internal set; }
        /// <summary>
        /// Checks if folder exists.
        /// </summary>
        public bool Exists
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(FileName) && File.Exists(FileName));
            }
        }
        /// <summary>
        /// Checks is file is allow to Read-Write and execute.
        /// </summary>
        public bool IsPublic
        {
            get { return this.Read && this.Write && this.Delete && this.Execute; }
        }
        /// <summary>
        /// Gets avaliable groups.
        /// </summary>
        public List<string> Groups { get; internal set; }

        #endregion
    }

    #endregion

    #region FilePermission

    /// <summary>
    /// File Permission.
    /// </summary>
    public sealed class FilePermission
    {
        #region Internal Variables

        private string _fileName = string.Empty;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        internal FilePermission() { }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~FilePermission()
        {
        }

        #endregion

        #region Private Methods

        #region GetOwnerShip

        /// <summary>
        /// Gets current ownership on target file system object.
        /// </summary>
        /// <returns>Returns NTUser that take ownership on specificed file system object.</returns>
        private string GetOwnerShip()
        {
            string _ownerShip = string.Empty;
            FileInfo fi = null;
            try
            {
                fi = new FileInfo(_fileName);
            }
            catch
            {
                fi = null;
            }
            if (null != fi && System.IO.File.Exists(_fileName))
            {
                FileSecurity fsec = fi.GetAccessControl();

                IdentityReference fowner = (null != fsec) ?
                    fsec.GetOwner(typeof(NTAccount)) : null;

                if (null != fowner)
                {
                    _ownerShip = fowner.Translate(typeof(NTAccount)).Value;
                }
                else
                {
                    _ownerShip = string.Empty;
                }
            }

            return _ownerShip;
        }

        #endregion

        #region GetUserGroups

        /// <summary>
        /// Gets avaliable user's groups that can access file.
        /// </summary>
        /// <returns>Returns avaliable user's groups that can access file.</returns>
        private List<string> GetUserGroups()
        {
            List<string> results = new List<string>();
            FileSecurity security = File.GetAccessControl(_fileName);
            AuthorizationRuleCollection acl = security.GetAccessRules(
               true, true, typeof(System.Security.Principal.NTAccount));
            foreach (FileSystemAccessRule ace in acl)
            {
                if (null == ace || null == ace.IdentityReference ||
                    string.IsNullOrWhiteSpace(ace.IdentityReference.Value))
                    continue;
                string account = ace.IdentityReference.Value;
                results.Add(account);
            }

            return results;
        }

        #endregion

        #region CheckPermissions

        /// <summary>
        /// Check Access Control permissions for specificed user on 
        /// target file or folder.
        /// </summary>
        /// <param name="requiredPermissions">
        /// The required permissions for target file or folder.
        /// </param>
        /// <returns>Returns true if required permissions is allow.</returns>
        private bool CheckPermissions(FileSystemRights requiredPermissions)
        {
            #region Check file info and load user rules each time method call

            FileInfo fi = null;
            try
            {
                fi = new FileInfo(_fileName);
            }
            catch
            {
                fi = null;
            }

            IEnumerable<AuthorizationRule> _userRules = null;
            if (null != fi && System.IO.File.Exists(_fileName))
            {
                FileSecurity fsec = fi.GetAccessControl();

                AuthorizationRuleCollection _acl = (null != fsec) ?
                    fsec.GetAccessRules(true, true, typeof(SecurityIdentifier)) : null;
                // gets rules that concern the user and his groups
                if (null != this.Current && null != _acl)
                {
                    _userRules = from AuthorizationRule rule in _acl
                                 where this.Current.User.Equals(rule.IdentityReference) ||
                                       this.Current.Groups.Contains(rule.IdentityReference)
                                 select rule;
                }
            }
            else
            {
                _userRules = null;
            }

            #endregion

            #region Compare is allow or deny

            FileSystemRights denyRights = 0;
            FileSystemRights allowRights = 0;

            if (null != _userRules)
            {
                // iterates on rules to compute denyRights and allowRights
                foreach (FileSystemAccessRule rule in _userRules)
                {
                    if (null == rule)
                        continue;

                    if (rule.AccessControlType.Equals(AccessControlType.Deny))
                    {
                        denyRights = denyRights | rule.FileSystemRights;
                    }
                    else if (rule.AccessControlType.Equals(AccessControlType.Allow))
                    {
                        allowRights = allowRights | rule.FileSystemRights;
                    }
                }

                // allowRights = allowRights - denyRights
                allowRights = allowRights & ~denyRights;
            }

            #endregion

            // are rights sufficient?
            return (allowRights & requiredPermissions) == requiredPermissions;
        }

        #endregion

        #endregion

        #region Public Methods

        #region GetPermissions

        /// <summary>
        /// Gets File Access Permissions.
        /// </summary>
        /// <returns>Returns the avaliable permissions for target file.</returns>
        public FilePermissionResult GetPermissions()
        {
            FilePermissionResult result = new FilePermissionResult();
            // Owner and Name
            result.OwnerShip = this.GetOwnerShip();
            result.FileName = this.FileName;
            // Change permission
            result.ChangePermission = CheckPermissions(FileSystemRights.ChangePermissions);
            // File related permissions
            result.Delete = CheckPermissions(
                // right to create folder or file
                FileSystemRights.Delete
                );
            result.Read = CheckPermissions(
                // right to open and copy folder or file as read only. 
                // this right contains rights:
                // - ReadData, 
                // - ReadExtendedAttribute, 
                // - ReadAttributes, 
                // - ReadPermissions.
                FileSystemRights.Read
                );
            result.Write = CheckPermissions(
                // right to create file or folder and add or remove data from file 
                // this right contains rights :
                // - WriteData, 
                // - AppendData, 
                // - WriteExtendedAttribute, 
                // - WriteAttributes
                FileSystemRights.Write
                );
            result.Execute = CheckPermissions(
                // right to open file as readonly and execute an application
                // this contains rights for 
                // - Read 
                // - Execute
                FileSystemRights.ReadAndExecute
                );
            // Gets Groups that can access file.
            result.Groups = GetUserGroups();

            return result;
        }

        #endregion

        #region GrantPermissions

        /// <summary>
        /// Grants access permissions for file with default FileSystemRights for
        /// public users.
        /// </summary>
        /// <param name="identity">The identity like Everyone or Users or User's identity name.</param>
        /// <returns>Returns permissions after grant to rechecked is grant success.</returns>
        public FilePermissionResult GrantPermissions(IdentityReference identity = null)
        {
            // Grant rights
            // Create a new FileInfo object.
            System.IO.FileInfo fInfo = null;
            try
            {
                fInfo = new System.IO.FileInfo(_fileName);
            }
            catch (Exception)
            {
                fInfo = null;
            }

            // Get a FileSecurity object that represents the 
            // current security settings.
            FileSecurity fSecurity = (null != fInfo) ?
                fInfo.GetAccessControl() : null;

            if (null == fSecurity)
                return null; // detected null 

            IdentityReference tempIden = null;
            if (null == identity)
            {
                tempIden = new SecurityIdentifier(
                    WellKnownSidType.BuiltinUsersSid, null);
            }
            else tempIden = identity;

            // set rule to allow access
            fSecurity.AddAccessRule(new FileSystemAccessRule(
                tempIden,
                FileSystemRights.Read |
                FileSystemRights.Write |
                FileSystemRights.Delete |
                FileSystemRights.ReadAndExecute |
                FileSystemRights.FullControl |
                FileSystemRights.ChangePermissions |
                FileSystemRights.Modify |
                FileSystemRights.Synchronize,
                AccessControlType.Allow));

            // Set the new access settings.
            if (null != fInfo)
                fInfo.SetAccessControl(fSecurity);

            return GetPermissions();
        }
        /// <summary>
        /// Grants access permissions for file.
        /// </summary>
        /// <param name="identity">The identity like Everyone or Users or User's identity name.</param>
        /// <param name="rightsToGrant">The FileSystemRights to grants.</param>
        /// <returns>Returns permissions after grant to rechecked is grant success.</returns>
        public FilePermissionResult GrantPermissions(IdentityReference identity,
            FileSystemRights rightsToGrant)
        {
            // Grant rights
            // Create a new FileInfo object.
            System.IO.FileInfo fInfo = null;
            try
            {
                fInfo = new System.IO.FileInfo(_fileName);
            }
            catch (Exception)
            {
                fInfo = null;
            }

            // Get a FileSecurity object that represents the 
            // current security settings.
            FileSecurity fSecurity = (null != fInfo) ?
                fInfo.GetAccessControl() : null;

            if (null == fSecurity)
                return null; // detected null 

            IdentityReference tempIden = null;
            if (null == identity)
            {
                tempIden = new SecurityIdentifier(
                    WellKnownSidType.BuiltinUsersSid, null);
            }
            else tempIden = identity;

            // set rule to allow to change permission
            fSecurity.AddAccessRule(new FileSystemAccessRule(
                tempIden, rightsToGrant |
                FileSystemRights.FullControl |
                FileSystemRights.ChangePermissions |
                FileSystemRights.Modify |
                FileSystemRights.Synchronize,
                AccessControlType.Allow));

            // Set the new access settings.
            if (null != fInfo)
                fInfo.SetAccessControl(fSecurity);

            return GetPermissions();
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Current User Identity.
        /// </summary>
        public WindowsIdentity Current
        {
            get
            {
                return WindowsIdentity.GetCurrent();
            }
        }
        /// <summary>
        /// Gets or sets target file name to checks or grants permission or rights.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                }
            }
        }
        /// <summary>
        /// Checks if file exists.
        /// </summary>
        public bool Exists
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(_fileName) && File.Exists(_fileName));
            }
        }

        #endregion
    }

    #endregion

    #region UAC

    /// <summary>
    /// UAC class. Provide methods for checks and grants access permission to
    /// files and folders.
    /// </summary>
    public class UAC
    {
        #region Internal Variables

        private static LogOnUser _user = new LogOnUser();

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private UAC()
            : base()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~UAC()
        {
            _user = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets Folder Permission.
        /// </summary>
        /// <param name="pathName">Target Path or Folder Name.</param>
        /// <returns>Returns instance of FolderPermission.</returns>
        public static FolderPermission GetFolderPermission(string pathName)
        {
            FolderPermission result = new FolderPermission();
            result.PathName = pathName;

            return result;
        }
        /// <summary>
        /// Gets File Permission.
        /// </summary>
        /// <param name="fileName">Target File Name.</param>
        /// <returns>Returns instance of FilePermission.</returns>
        public static FilePermission GetFilePermission(string fileName)
        {
            FilePermission result = new FilePermission();
            result.FileName = fileName;

            return result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Current LogOn User information.
        /// </summary>
        public static LogOnUser User { get { return _user; } }
        /// <summary>
        /// Access Build In Groups.
        /// </summary>
        public class BuildInGroups
        {
            /// <summary>
            /// Build In Identifier for NT Domain/Users.
            /// </summary>
            public static readonly SecurityIdentifier NTUser = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            /// <summary>
            /// Build In Identifier for NT Domain/Administrators.
            /// </summary>
            public static readonly SecurityIdentifier NTAdmin = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
            /// <summary>
            /// Build In Identifier for EveryOne.
            /// </summary>
            public static readonly SecurityIdentifier EveryOne = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
        }

        #endregion
    }

    #endregion
}
