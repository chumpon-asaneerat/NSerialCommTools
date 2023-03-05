#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-11-20
=================
- NLib IO Framework - Window Explorer Shell.
  - Add Extension Methods for Open Folder in Window Explorer.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

#endregion

namespace NLib
{
    /// <summary>
    /// Window Explorer Shell Extension Methods.
    /// </summary>
    public static class WindowExplorerShellExtensions
    {
        #region Open Folder
        
        /// <summary>
        /// Open Folder in Window Explorer.
        /// </summary>
        /// <param name="folderName">The Folder Name.</param>
        public static void OpenFolder(this string folderName)
        {
            if (folderName == string.Empty || !Directory.Exists(folderName))
                return;
            Process proc = new Process();
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.FileName = "\"" + folderName + "\"";
            proc.Start();
            ProcessManager.Instance.KillConsoleIME();
        }

        #endregion

        #region Open File in Default Program

        /// <summary>
        /// Open File with default program.
        /// </summary>
        /// <param name="fileName">The Folder Name.</param>
        public static void OpenFileInDefaultProgram(this string fileName)
        {
            if (fileName == string.Empty || !File.Exists(fileName))
                return;
            Process proc = new Process();
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.FileName = "\"" + fileName + "\"";
            proc.Start();
            ProcessManager.Instance.KillConsoleIME();
        }

        #endregion
    }
}
