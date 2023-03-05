#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-01-16
=================
- Comperssion Framework updated.
  - Change namespace from NLib.Compressions to NLib.Utils.

======================================================================================================================
Update 2013-01-16
=================
- Comperssion Framework updated.
  - change namespace from NLib.Resources to NLib.Resource.

======================================================================================================================
Update 2012-12-25
=================
- Comperssion Framework updated.
  - reduce code by refactor the common code.

======================================================================================================================
Update 2012-01-02
=================
- Comperssion Framework updated.
  - replace all resouce access classs with new resource access model.

======================================================================================================================
Update 2011-11-07
=================
- Comperssion Framework ported from GFA40 to NLib.
  - All compression class are removed excepts SevenZipManager class.
  - SevenZipManager add checks directory code in Create7ZipCommandLine method.
  - All example code is removed excepts example code for SevenZipManager class.

======================================================================================================================

Update 2010-08-31
=================
- Comperssion Framework ported from GFA38v3 to GFA40 and GFA20.

======================================================================================================================
Update 2010-01-31
=================
- Comperssion Framework ported from GFA37 to GFA38v2.
  - Ported Comperssion related class with update code used new Log/Exception framework.

======================================================================================================================
Update 2007-11-30
=================
- Comperssion Framework provides ablilities to Compress/Decompress File or Folder in wellknown
  Format like BZip2, Gz, Tar, Zip amd 7z Format. (note that Rar format is not support because
  lack information about Rar format.). Note that the Comperssion Framework still not support
  multithread so be careful to used with in multithread environment.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System.IO;

using NLib.Resource;

#endregion

namespace NLib.Utils
{
    #region Seven Zip Manager

    /// <summary>
    /// Seven Zip Manager
    /// </summary>
    public sealed class SevenZipManager
    {
        private static void Execute(string source, string target, string args)
        {
            using (WindowFormsResourceAccess resAccess = new WindowFormsResourceAccess())
            {
                ResourceExecuteOptions option = new ResourceExecuteOptions()
                {
                    ResourceName = SevenZipConsts.SevenZip,
                    AutoCreate = true,
                    AutoDelete = true,
                    ShowWindow = false,
                    CallerType = typeof(SevenZipConsts),
                    TargetPath = ApplicationManager.Instance.Environments.Company.Temp.FullName,
                    TargetFileName = @"7za.exe",
                    Argument = args
                };

                resAccess.Execute(option);
            }
        }

        /// <summary>
        /// Compress
        /// </summary>
        /// <param name="source">Source File to compress</param>
        public static void Compress(string source)
        {
            if (!File.Exists(source))
            {
                return;
            }
            string target = source + ".7z";
            Compress(source, target);
        }
        /// <summary>
        /// Compress
        /// </summary>
        /// <param name="source">Source File to compress</param>
        /// <param name="target">Output file name</param>
        public static void Compress(string source, string target)
        {
            if (!File.Exists(source))
            {
                return;
            }

            string args = "a -y \"" + target + "\" \"" + source + "\"";
            Execute(source, target, args);
        }
        /// <summary>
        /// Compress Directory
        /// </summary>
        /// <param name="source">Source Path Name to compress</param>
        /// <param name="target">Output file name</param>
        public static void CompressDirectory(string source, string target)
        {
            if (!Directory.Exists(source))
            {
                return;
            }

            string args = "a -y \"" + target + "\" \"" + source + "\"";
            Execute(source, target, args);
        }
        /// <summary>
        /// Compress Directory
        /// </summary>
        /// <param name="source">Source Path Name to compress</param>
        /// <param name="target">Output file name</param>
        /// <param name="useZip">Used zip format.</param>
        public static void CompressDirectory(string source, string target, bool useZip)
        {
            if (!Directory.Exists(source))
            {
                return;
            }
            if (!useZip)
            {
                // used 7z
                CompressDirectory(source, target);
            }
            else
            {
                // use zip
                string args = "a -tzip -y \"" + target + "\" \"" + source + "\"";
                Execute(source, target, args);
            }
        }
        /// <summary>
        /// Decompress
        /// </summary>
        /// <param name="source">Source File to decompress</param>
        public static void Decompress(string source)
        {
            if (!File.Exists(source))
            {
                return;
            }
            string target = Path.GetDirectoryName(source);
            Decompress(source, target);
        }
        /// <summary>
        /// Decompress
        /// </summary>
        /// <param name="source">Source File to decompress</param>
        /// <param name="target">Output Path</param>
        public static void Decompress(string source, string target)
        {
            if (!File.Exists(source))
            {
                return;
            }

            string args = "x -y -o\"" + target + "\" \"" + source + "\"";
            Execute(source, target, args);
        }
    }

    #endregion
}
