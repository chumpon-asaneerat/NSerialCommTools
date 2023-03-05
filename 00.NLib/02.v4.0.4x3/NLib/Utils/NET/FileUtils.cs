#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-17
=================
- FileUtils class Added.
  - Change Debug code to using MethodBase instead of StackFrame.

======================================================================================================================
Update 2013-08-19
=================
- FileUtils class Added.
  - Add GetFileHash method.

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
using System.Reflection;
using System.Security.Cryptography;

#endregion

namespace NLib.Utils
{
    /// <summary>
    /// File Utils.
    /// </summary>
    public static class FileUtils
    {
        #region Get File Hash

        /// <summary>
        /// Gets File Hash string.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>Returns string.Empty is something is invalid.</returns>
        public static string GetFileHash(string fileName)
        {
            string result = string.Empty;
            MethodBase med = MethodBase.GetCurrentMethod();

            if (!File.Exists(fileName))
            {
                //med.Info("File not found : '{0}'", fileName);
                "File not found : '{0}'".Err(med, fileName);
                return result;
            }
            try
            {
                int bufferSize = 1200000;
                using (var stream = new BufferedStream(File.OpenRead(fileName), bufferSize))
                {
                    using (SHA256Managed sha = new SHA256Managed())
                    {
                        byte[] checksum = sha.ComputeHash(stream);
                        // Make is as standard.
                        result = BitConverter.ToString(checksum)
                            .Replace("-", String.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Err(med);
                result = string.Empty;
            }

            return result;
        }

        #endregion
    }
}