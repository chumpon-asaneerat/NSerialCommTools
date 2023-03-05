#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-07-07
=================
- Ms Document Utils updated.
  - Changed all log code used MethodBase.

======================================================================================================================
Update 2013-09-01
=================
- Ms Document Utils updated.
  - Refactor common code and add exception handel with write to log.

======================================================================================================================
Update 2013-08-16
=================
- Ms Document Utils updated.
  - MS Access 2003 file is removed DAO reference and repack into embeded resource.

======================================================================================================================
Update 2013-01-16
=================
- Ms Document Utils updated.
  - change namespace from NLib.Resources to NLib.Resource.

======================================================================================================================
Update 2012-12-25
=================
- Ms Document Utils updated.
  - Update code to used new NFolder in environment class.

======================================================================================================================
Update 2012-01-02
=================
- Ms Document Utils updated.
  - Rewrite code to used new Resource Acccess Model.

======================================================================================================================
Update 2010-08-31
=================
- Ms Document Utils ported from GFA38v3 to GFA40 and GFA20.

======================================================================================================================
Update 2010-02-01
=================
- Ms Document Utils ported from GFA37 to GFA38v3
  - change all log/exception to used new debug/exception framework.
  - Remove all methods that used internal Open/Save dialog. For user code that used
    these methods should used new SysLib.IO.FileTypes classes and create own open/save 
    dialog instead used the internal dialog code.

======================================================================================================================
Update 2008-05-20
=================
- Ms Document Utils Changed
  - Refactor common code.
  - Add auto create directory in case the target directory is not exists.

======================================================================================================================
Update 2007-11-26
=================
- MsDocumentUtils add some fixed for multithread environment and optimized
  for performance

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.IO;
using System.Reflection;
using NLib.Resource;

#endregion

namespace NLib.Utils
{
    #region MsDocument Helper Class

    /// <summary>
    /// Utility Class for Create Microsoft Office Document
    /// </summary>
    public sealed class MsDocumentUtils
    {
        #region Create Resource

        private static bool CreateResource(string resourceName,
            string FullFileName, bool AutoOverwrite)
        {
            if (File.Exists(FullFileName))
            {
                if (!AutoOverwrite)
                    throw (new Exception("File " + FullFileName + " is Exist"));
                File.Delete(FullFileName);
            }

            using (WindowFormsResourceAccess resAccess = new WindowFormsResourceAccess())
            {
                ResourceStreamOptions option = new ResourceStreamOptions()
                {
                    ResourceName = resourceName,
                    CallerType = typeof(MsDocumentUtils),
                    TargetPath = Path.GetDirectoryName(FullFileName),
                    TargetFileName = Path.GetFileName(FullFileName) + ".7z"
                };

                resAccess.CreateFile(option);
            }

            return File.Exists(FullFileName + ".7z");
        }

        private static void MoveFile(string sourceFileName, string targetFileName)
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            // move file
            if (File.Exists(sourceFileName))
            {
                try
                {
                    File.Move(sourceFileName, targetFileName);
                }
                catch (Exception ex)
                {
                    ex.Err(med);
                }
            }
        }

        private static void DeleteFile(string fileName)
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            // delete file
            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    ex.Err(med);
                }
            }
        }

        private static bool CreateFileFromResource(string embededResourceName,
            string outputFileNameOnly, string FullFileName, bool AutoOverwrite)
        {
            if (string.IsNullOrWhiteSpace(embededResourceName) ||
                string.IsNullOrWhiteSpace(outputFileNameOnly) ||
                string.IsNullOrWhiteSpace(FullFileName))
                return false;

            if (CreateResource(embededResourceName, FullFileName, AutoOverwrite))
            {
                // decompress file.
                SevenZipManager.Decompress(FullFileName + ".7z");
                // Rename
                string resourceFileOutput =
                    Path.GetDirectoryName(FullFileName) + outputFileNameOnly;
                // Move resource output file  to target file
                MoveFile(resourceFileOutput, FullFileName);
            }
            // delete file
            DeleteFile(FullFileName + ".7z");

            return true;
        }

        #endregion

        #region Excel

        /// <summary>
        /// Create Excel 97 File
        /// </summary>
        /// <param name="FullFileName">File Name</param>
        /// <returns>true if file is created</returns>
        public static bool CreateExcel97File(string FullFileName)
        {
            return CreateExcel97File(FullFileName, true);
        }
        /// <summary>
        /// Create Excel 97 File
        /// </summary>
        /// <param name="FullFileName">FileName</param>
        /// <param name="AutoOverwrite">Force Overwrite</param>
        /// <returns>true if file is created</returns>
        public static bool CreateExcel97File(string FullFileName, bool AutoOverwrite)
        {
            return CreateFileFromResource(MSOfficeConsts.Excel97,
                @"\excel97.xls", FullFileName, AutoOverwrite);
        }
        /// <summary>
        /// Create Excel 2003 File
        /// </summary>
        /// <param name="FullFileName">File Name</param>
        /// <returns>true if file is created</returns>
        public static bool CreateExcel2003File(string FullFileName)
        {
            return CreateExcel2003File(FullFileName, true);
        }
        /// <summary>
        /// Create Excel 2003 File
        /// </summary>
        /// <param name="FullFileName">FileName</param>
        /// <param name="AutoOverwrite">Force Overwrite</param>
        /// <returns>true if file is created</returns>
        public static bool CreateExcel2003File(string FullFileName, bool AutoOverwrite)
        {
            return CreateFileFromResource(MSOfficeConsts.Excel2003,
                @"\excel2003.xls", FullFileName, AutoOverwrite);
        }

        #endregion

        #region MsAccess

        /// <summary>
        /// Create Access 2000 File
        /// </summary>
        /// <param name="FullFileName">File Name</param>
        /// <returns>true if file is created</returns>
        public static bool CreateMsAccess2KFile(string FullFileName)
        {
            return CreateMsAccess2KFile(FullFileName, true);
        }
        /// <summary>
        /// Create Access 2000 File
        /// </summary>
        /// <param name="FullFileName">FileName</param>
        /// <param name="AutoOverwrite">Force Overwrite</param>
        /// <returns>true if file is created</returns>
        public static bool CreateMsAccess2KFile(string FullFileName, bool AutoOverwrite)
        {
            return CreateFileFromResource(MSOfficeConsts.Access2K,
                @"\access2K.mdb", FullFileName, AutoOverwrite);
        }
        /// <summary>
        /// Create Access 2003 File
        /// </summary>
        /// <param name="FullFileName">File Name</param>
        /// <returns>true if file is created</returns>
        public static bool CreateMsAccess2003File(string FullFileName)
        {
            return CreateMsAccess2003File(FullFileName, true);
        }
        /// <summary>
        /// Create Access 2003 File
        /// </summary>
        /// <param name="FullFileName">FileName</param>
        /// <param name="AutoOverwrite">Force Overwrite</param>
        /// <returns>true if file is created</returns>
        public static bool CreateMsAccess2003File(string FullFileName, bool AutoOverwrite)
        {
            return CreateFileFromResource(MSOfficeConsts.Access2003,
                @"\access2003.mdb", FullFileName, AutoOverwrite);
        }

        #endregion

        #region Word

        /// <summary>
        /// Create Word XP  File
        /// </summary>
        /// <param name="FullFileName">File Name</param>
        /// <returns>true if file is created</returns>
        public static bool CreateWordXPFile(string FullFileName)
        {
            return CreateWordXPFile(FullFileName, true);
        }
        /// <summary>
        /// Create Word XP  File
        /// </summary>
        /// <param name="FullFileName">FileName</param>
        /// <param name="AutoOverwrite">Force Overwrite</param>
        /// <returns>true if file is created</returns>
        public static bool CreateWordXPFile(string FullFileName, bool AutoOverwrite)
        {
            return CreateFileFromResource(MSOfficeConsts.WordXP,
                @"\WordXP.doc", FullFileName, AutoOverwrite);
        }
        /// <summary>
        /// Create Word 2003  File
        /// </summary>
        /// <param name="FullFileName">File Name</param>
        /// <returns>true if file is created</returns>
        public static bool CreateWord2003File(string FullFileName)
        {
            return CreateWord2003File(FullFileName, true);
        }
        /// <summary>
        /// Create Word 2003  File
        /// </summary>
        /// <param name="FullFileName">FileName</param>
        /// <param name="AutoOverwrite">Force Overwrite</param>
        /// <returns>true if file is created</returns>
        public static bool CreateWord2003File(string FullFileName, bool AutoOverwrite)
        {
            return CreateFileFromResource(MSOfficeConsts.Word2003,
                @"\Word2003.doc", FullFileName, AutoOverwrite);
        }

        #endregion
    }

    #endregion
}
