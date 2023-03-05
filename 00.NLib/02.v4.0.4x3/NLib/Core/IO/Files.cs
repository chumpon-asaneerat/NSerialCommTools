#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-10-14
=================
- NLib IO Framework - Files.
  - Remove Grant methods (that has string parameter) and Used WellKnownSid.WorldSid instead
    of EveryOne.
  - Add method to list avaliable groups that can access file.
  - Fixed bug in Grant methods and add Identity parameter with default is null (NT User).

======================================================================================================================
Update 2013-01-15
=================
- NLib IO Framework - Files.
  - Update code in Files class by using new IOOperation class and 
    FilePermissionResult class

======================================================================================================================
Update 2012-12-25
=================
- NLib IO Framework - Files.
  - Add NFile class. This class provide wrapper function to access and work with
    application file. This class is non serialized and still in progress.

======================================================================================================================
Update 2011-12-15
=================
- NLib IO Framework - Files utility.
  - Add Files class. These class provide static methods for working with local files
    which contains below methods.
    - Create method. Used for create new file with replace option.
    - Copy method. Used for copy file with overwrite option.
    - Move method. Used for rename or move file.
    - Delete method. Used for delete exists file.
    - Exists method. Used for checks the file is already exists.
    - Grant method. Used for grant permission to allow all users read/write/execute the
      file.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.IO;
using System.ComponentModel;
using System.Security.Principal;

using NLib;
using NLib.IO.Security;

#endregion

namespace NLib.IO
{
    #region Files

    /// <summary>
    /// Files utility class.
    /// </summary>
    public sealed class Files
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private Files() : base() { }

        #endregion

        #region Public (static methods)

        #region Create

        /// <summary>
        /// Create New Empty File.
        /// </summary>
        /// <param name="fileName">The File's Name.</param>
        /// <param name="replace">True if file is already exists and force to replace it.</param>
        /// <returns>Returns result of operation.</returns>
        public static IOOperationResult Create(string fileName, bool replace = false)
        {
            IOOperationResult result = new IOOperationResult();

            bool hasErr = false;

            if (File.Exists(fileName))
            {
                #region File already exists so if need replace try to remove

                if (replace)
                {
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch (Exception ex)
                    {
                        // Failed.
                        hasErr = true;
                        result.Result = IOStatus.Error;
                        result.Err = ex;
                    }
                }

                #endregion
            }

            // checks file is not exists
            if (!hasErr && !File.Exists(fileName))
            {
                #region Try to create

                hasErr = false;
                try
                {
                    FileStream fs = File.Create(fileName);
                    if (null != fs)
                    {
                        try { fs.Flush(); }
                        catch { }
                        try { fs.Dispose(); }
                        catch { }
                    }
                    fs = null;
                }
                catch (Exception ex)
                {
                    // Failed.
                    hasErr = true;
                    result.Result = IOStatus.Error;
                    result.Err = ex;
                }
                // Rechecks and generate result
                if (!hasErr && File.Exists(fileName))
                {
                    result.Result = IOStatus.Success;
                    result.Err = null;
                }

                #endregion
            }
            else if (!hasErr)
            {
                #region File already exists

                result.Result = IOStatus.Error;
                result.Err = new Exception("File is already exists and not assigned to force replace.");

                #endregion
            }

            return result;
        }

        #endregion

        #region Copy

        /// <summary>
        /// Move or rename file with can access by all users.
        /// </summary>
        /// <param name="sourceFileName">The source file's name.</param>
        /// <param name="targetFileName">The target file's name.</param>
        /// <param name="overwrite">True to force overwrite file.</param>
        /// <returns>Returns result of operation.</returns>
        public static IOOperationResult Copy(string sourceFileName, string targetFileName,
            bool overwrite = true)
        {
            IOOperationResult result = new IOOperationResult();

            bool hasError = false;
            if (!File.Exists(sourceFileName))
            {
                hasError = true;
                result.Result = IOStatus.Error;
                result.Err = new Exception("Source file not exists.");
            }
            if (!hasError && File.Exists(targetFileName))
            {
                try
                {
                    File.Delete(targetFileName);
                }
                catch (Exception ex)
                {
                    hasError = true;
                    result.Result = IOStatus.Error;
                    result.Err = ex;
                }
            }

            if (!hasError && !File.Exists(targetFileName))
            {
                try
                {
                    File.Copy(sourceFileName, targetFileName, overwrite);
                }
                catch (Exception ex)
                {
                    hasError = true;
                    result.Result = IOStatus.Error;
                    result.Err = ex;
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

        #region Move

        /// <summary>
        /// Move or rename file with can access by all users.
        /// </summary>
        /// <param name="sourceFileName">The file's name to rename or move.</param>
        /// <param name="newFileName">The new file's name to rename or move.</param>
        /// <returns>Returns result of operation.</returns>
        public static IOOperationResult Move(string sourceFileName, string newFileName)
        {
            IOOperationResult result = new IOOperationResult();

            bool hasError = false;
            if (!File.Exists(sourceFileName))
            {
                hasError = true;
                result.Result = IOStatus.Error;
                result.Err = new Exception("Source file not exists.");
            }
            if (!hasError && File.Exists(newFileName))
            {
                try
                {
                    File.Delete(newFileName);
                }
                catch (Exception ex)
                {
                    hasError = true;
                    result.Result = IOStatus.Error;
                    result.Err = ex;
                }
            }

            if (!hasError && !File.Exists(newFileName))
            {
                try
                {
                    File.Move(sourceFileName, newFileName);
                }
                catch (Exception ex)
                {
                    hasError = true;
                    result.Result = IOStatus.Error;
                    result.Err = ex;
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

        #region Delete

        /// <summary>
        /// Delete File.
        /// </summary>
        /// <param name="fileName">The File's Name.</param>
        /// <returns>Returns result of operation.</returns>
        public static IOOperationResult Delete(string fileName)
        {
            IOOperationResult result = new IOOperationResult();

            if (!File.Exists(fileName))
            {
                #region No file exists. So assume operation is success

                result.Result = IOStatus.Success;
                result.Err = null;

                #endregion
            }
            else
            {
                #region File exists. Try to delete it

                bool hasErr = false;
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    hasErr = true;
                    result.Result = IOStatus.Error;
                    result.Err = ex;
                }
                // rechecks and generate result if success.
                if (!hasErr && !File.Exists(fileName))
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
        /// Checks is file is exists.
        /// </summary>
        /// <param name="fileName">The file's name.</param>
        /// <returns>Returns true if file is exists.</returns>
        public static bool Exists(string fileName)
        {
            return File.Exists(fileName);
        }

        #endregion

        #region Grant

        /// <summary>
        /// Grant permission to public access to target file.
        /// </summary>
        /// <param name="fileName">The target file name.</param>
        /// <param name="identity">The target identity default is NT User.</param>
        /// <returns>Returns result of operation.</returns>
        public static IOOperationResult Grant(string fileName,
            SecurityIdentifier identity = null)
        {
            IOOperationResult result = new IOOperationResult();

            bool hasError = false;
            if (!File.Exists(fileName))
            {
                #region File not found

                hasError = true;
                result.Result = IOStatus.Error;
                result.Err = new Exception("Target file not exists.");

                #endregion
            }
            else
            {
                FilePermissionResult permitResult = null;
                permitResult = UAC.GetFilePermission(fileName).GetPermissions();
                if (null != permitResult && permitResult.IsPublic)
                {
                    SecurityIdentifier tempIden = null;
                    if (null == identity)
                        tempIden = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
                    else tempIden = identity;
                    // Try to grant access                    
                    permitResult = UAC.GetFilePermission(fileName).GrantPermissions(tempIden);
                }
                if (null == permitResult || !permitResult.IsPublic)
                {
                    hasError = true;
                    result.Result = IOStatus.Error;
                    result.Err = new Exception("Cannot grant file permission to public access.");
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

        #endregion
    }

    #endregion
}