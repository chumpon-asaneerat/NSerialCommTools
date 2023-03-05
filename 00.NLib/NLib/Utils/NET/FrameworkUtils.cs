#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-12-27
=================
- Framework Utils class Add.
  - Add property NETRuntime. This property is directly call NLib.IO.Folders.OS.NETRuntime property.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System.Runtime.InteropServices;

#endregion

namespace NLib.Utils
{
    /// <summary>
    /// The .NET Framework Utils class.
    /// </summary>
    public class FrameworkUtils
    {
        #region Public Method(s)

        // TODO - Port install/uninstall utils for windows service.

        #endregion

        #region Public Properties (static)

        /// <summary>
        /// Gets .Net Framework Folder. Same as NLib.IO.Folders.OS.NETRuntime.
        /// </summary>
        public static string NETRuntime
        {
            get
            {
                return NLib.IO.Folders.OS.NETRuntime;
            }
        }

        #endregion

        /// <summary>
        /// Test.
        /// </summary>
        public static void Test()
        {
            // Required MSBuild, MSBuild.Utilities.v40
            //Microsoft.Build.Utilities.ToolLocationHelper
        }
    }
}
