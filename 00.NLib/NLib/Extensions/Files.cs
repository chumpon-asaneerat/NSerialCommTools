#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-23
=================
- NLib IO Framework - Updated.
  - ToFileInfo method add try-catch code to write exception to console.

======================================================================================================================
Update 2014-10-15
=================
- NLib IO Framework - Updated.
  - CreateEmptyFile add grant access rights parameter.
  - Add methods GetFileHash.

======================================================================================================================
Update 2013-01-01
=================
- NLib IO Framework - Updated.
  - FileInfoExtensionMethods is now seperated to new file.

======================================================================================================================
Update 2012-12-20
=================
- NLib IO Framework - Common Updated.
  - Rename class FileInfoExtensionMethods to FileExtensionMethods.
  - Fixed Problem max file limit work improper in Rolling method.

======================================================================================================================
Update 2011-12-15
=================
- NLib IO Framework - Common.
  - Add Extension Methods for Rolling file (work with FileInfo class).

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.IO;
using System.Collections.Generic;

#endregion

namespace NLib
{
    #region FileExtensionMethods

    /// <summary>
    /// File Extension Methods
    /// </summary>
    public static class FileExtensionMethods
    {
        #region Const

        /// <summary>
        /// The Default Rename Pattern
        /// </summary>
        //public const string DEFAULT_RENAME_PATTERN = "yyyy.MM.dd.HH.mm.REV";
        public const string DEFAULT_RENAME_PATTERN = "yyyy.MM.dd.REV";
        /// <summary>
        /// Unlimit Revision constant
        /// </summary>
        public const uint REVISION_UNLIMIT = 0;
        /// <summary>
        /// Max value of Revision constant
        /// </summary>
        public const uint REVISION_MAXLIMIT = 1000;

        #endregion

        #region Internal class

        class FileNamePair
        {
            public string Source;
            public string Target;

            public FileNamePair(string source, string target)
            {
                this.Source = source;
                this.Target = target;
            }
        }

        #endregion

        #region Private Methods (Static)

        private static string BuildRevMask(string renamePattern)
        {
            string revMask = string.Empty;

            if (renamePattern.IndexOf("d") > 0 ||
                renamePattern.IndexOf("M") > 0 ||
                renamePattern.IndexOf("m") > 0 ||
                renamePattern.IndexOf("h") > 0 ||
                renamePattern.IndexOf("y") > 0)
            {
                DateTime dt = DateTime.Now;
                revMask += dt.ToString(renamePattern,
                    System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }
            else revMask += renamePattern;

            return revMask;
        }

        private static List<FileNamePair> BuildFilePairs(FileInfo file,
            string renamePattern, uint maxRevision)
        {
            if (null == file)
                return null;

            string revMask = BuildRevMask(renamePattern);
            List<FileNamePair> filePairs = new List<FileNamePair>();

            string pathWithfileName = Path.Combine(file.DirectoryName,
                Path.GetFileNameWithoutExtension(file.FullName));

            uint iRev = 1;
            string sourceFileName = file.FullName;
            string targetFileName = pathWithfileName + "." +
                    revMask.Replace("REV", iRev.ToString("D3")) + file.Extension;
            while (iRev <= maxRevision)
            {
                filePairs.Add(new FileNamePair(sourceFileName, targetFileName));
                // set target file as new source file
                sourceFileName = targetFileName;
                ++iRev;
                // build new target file name.
                targetFileName = pathWithfileName + "." +
                    revMask.Replace("REV", iRev.ToString("D3")) + file.Extension;
            }

            // valid rev file names
            Console.WriteLine("==== valid rev file names ====");
            foreach (FileNamePair pair in filePairs)
            {
                Console.WriteLine(pair.Source + " => " + pair.Target);
            }

            return filePairs;
        }

        #endregion

        #region Public Methods (Static)

        #region ToFileInfo
        
        /// <summary>
        /// Convert string to FileInfo instance.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>Returns FileInfo instance.</returns>
        public static FileInfo ToFileInfo(this string fileName)
        {
            FileInfo result = null;

            try 
            {
                result = new FileInfo(fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }

        #endregion

        #region CreateEmptyFile

        /// <summary>
        /// Create new file.
        /// </summary>
        /// <param name="file">The FileInfo instance.</param>
        /// <param name="allowAllUsers">True for auto grant access rights for all users.</param>
        public static void CreateEmptyFile(this FileInfo file, bool allowAllUsers = true)
        {
            if (null == file)
                return;

            file.Refresh();
            if (!file.Exists)
            {
                FileStream fs = file.Create();
                if (null != fs)
                {
                    try
                    {
                        fs.Flush();
                        fs.Close();
                        fs.Dispose();

                    }
                    catch { }
                }
                fs = null;
                
                if (allowAllUsers)
                {
                    // Grant access rights
                    NLib.IO.Files.Grant(file.FullName);
                }
            }
        }

        #endregion

        #region Rolling

        /// <summary>
        /// Rolling file.
        /// </summary>
        /// <param name="file">The FileInfo instance.</param>
        /// <param name="searchPattern">
        /// The remove search pattern. Default is *.*
        /// </param>
        /// <param name="skipDeleteFiles">
        /// The file to skip to remove. The file name is only file name in directory not full file name.
        /// </param>
        public static void Rolling(this FileInfo file,
            string searchPattern = "*.*",
            params string[] skipDeleteFiles)
        {
            Rolling(file, DEFAULT_RENAME_PATTERN, 10,
                searchPattern, skipDeleteFiles);
        }
        /// <summary>
        /// Rolling file.
        /// </summary>
        /// <param name="file">The FileInfo instance.</param>
        /// <param name="maxRevision">The maxuimum revision.</param>
        /// <param name="searchPattern">
        /// The remove search pattern. Default is *.*
        /// </param>
        /// <param name="skipDeleteFiles">
        /// The file to skip to remove. The file name is only file name in directory not full file name.
        /// </param>
        public static void Rolling(this FileInfo file, uint maxRevision,
            string searchPattern = "*.*",
            params string[] skipDeleteFiles)
        {
            Rolling(file, DEFAULT_RENAME_PATTERN, maxRevision,
                searchPattern, skipDeleteFiles);
        }
        /// <summary>
        /// Rolling file.
        /// </summary>
        /// <param name="file">The FileInfo instance.</param>
        /// <param name="renamePattern">The rename patterns.</param>
        /// <param name="maxRevision">The maxuimum revision.</param>
        /// <param name="searchPattern">
        /// The remove search pattern. Default is *.*
        /// </param>
        /// <param name="skipDeleteFiles">
        /// The file to skip to remove. The file name is only file name in directory not full file name.
        /// </param>
        public static void Rolling(this FileInfo file,
            string renamePattern,
            uint maxRevision,
            string searchPattern = "*.*",
            params string[] skipDeleteFiles)
        {
            if (null == file || !file.Exists)
                return;

            #region Check Dir is exists

            DirectoryInfo dir = file.Directory;
            if (null == dir)
            {
                // create if required. 
                // Security handle code is required. <---------
                dir = Directory.CreateDirectory(file.DirectoryName);
            }

            #endregion

            #region Bulid Skip file list

            List<string> skipFiles = new List<string>();
            if (null != skipDeleteFiles && skipDeleteFiles.Length > 0)
            {
                foreach (string skipDeleteFile in skipDeleteFiles)
                {
                    string fullSkipFileName =
                        Path.Combine(file.DirectoryName, skipDeleteFile);
                    // Add full file name to list
                    skipFiles.Add(fullSkipFileName);
                }
            }

            #endregion

            List<FileNamePair> filePairs =
                BuildFilePairs(file, renamePattern, maxRevision);
            if (null != filePairs)
            {
                filePairs.Reverse(); // reverse order of list

                #region Rename process

                foreach (FileNamePair filePair in filePairs)
                {
                    if (null == filePair)
                        continue;
                    // remove exist target file.
                    if (File.Exists(filePair.Source) && File.Exists(filePair.Target))
                        File.Delete(filePair.Target);
                    // rename source to target file
                    if (File.Exists(filePair.Source) && !File.Exists(filePair.Target))
                        File.Move(filePair.Source, filePair.Target);
                }

                #endregion

                // Remove all files that is not in file pairs
                if (null != dir)
                {
                    FileInfo[] existFiles = dir.GetFiles(searchPattern);
                    if (null != existFiles && existFiles.Length > 0)
                    {
                        #region Build Remove list

                        List<FileInfo> removeFiles = new List<FileInfo>();
                        FileNamePair fileInst = null;
                        foreach (FileInfo existFile in existFiles)
                        {
                            #region Find delegate method

                            fileInst = filePairs.Find((FileNamePair match) =>
                            {
                                bool result = false;

                                if (null == match)
                                    return result;

                                bool found =
                                    (string.Compare(match.Target, existFile.FullName, true) == 0);

                                if (found)
                                {
                                    #region Check is skip file

                                    if (null == skipFiles ||
                                        skipFiles.Count <= 0)
                                    {
                                        // No skip files
                                        result = found;
                                    }
                                    else
                                    {
                                        // Check if file is in skip list.
                                        result = !skipFiles.Contains(existFile.FullName);
                                    }

                                    #endregion
                                }
                                return result;
                            });

                            #endregion

                            if (null == fileInst)
                            {
                                // not in file pair so remove it.
                                removeFiles.Add(existFile);
                            }
                        }

                        #endregion

                        if (null != removeFiles)
                        {
                            int iFileCount = 0;
                            if (null != existFiles)
                            {
                                iFileCount = existFiles.Length;
                            }

                            int iRemoveCount = 0;
                            foreach (FileInfo fi in removeFiles)
                            {
                                if (iFileCount > 0)
                                {
                                    if (iFileCount <= maxRevision)
                                        break; // when reach max revision so do not need remove
                                }
                                try
                                {
                                    File.Delete(fi.FullName);
                                    ++iRemoveCount; // increase remove counter
                                    if (iFileCount > 0)
                                    {
                                        --iFileCount; // decrease file counter
                                    }
                                }
                                catch { }

                            }
                            removeFiles.Clear();
                        }
                        removeFiles = null;
                    }
                }
                filePairs.Clear();
            }
            filePairs = null;

            // recreate empty file for next operation.
            file.CreateEmptyFile();
        }

        #endregion

        #region GetFileHash

        /// <summary>
        /// Gets File Hash string.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>Returns string.Empty is something is invalid.</returns>
        public static string GetFileHash(this string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;
            return Utils.FileUtils.GetFileHash(fileName);
        }
        /// <summary>
        /// Gets File Hash string.
        /// </summary>
        /// <param name="fileInfo">The FileInfo instance.</param>
        /// <returns>Returns string.Empty is something is invalid.</returns>
        public static string GetFileHash(this FileInfo fileInfo)
        {
            if (null == fileInfo)
                return string.Empty;
            return Utils.FileUtils.GetFileHash(fileInfo.FullName);
        }

        #endregion

        #endregion
    }

    #endregion
}
