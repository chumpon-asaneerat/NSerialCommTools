#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-08-07
=================
- NLib Configuration framwork.
  - Add IConfigManager interface.
  - Add NConfigManager class.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

#endregion

namespace NLib.Configs
{
    #region IConfigManager
    
    /// <summary>
    /// IConfigManager interface.
    /// </summary>
    public interface IConfigManager
    {
        #region Public Methods

        /// <summary>
        /// Save Config.
        /// </summary>
        /// <returns>Returns instance of NResult.</returns>
        NResult<bool> SaveConfig();
        /// <summary>
        /// Load Config.
        /// </summary>
        /// <returns>Returns instance of NResult.</returns>
        NResult<bool> LoadConfig();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Configuration File Name.
        /// </summary>
        [Category("Configs")]
        [Description("Gets Configuration File Name.")]
        string ConfigFileName { get; }

        #endregion
    }

    #endregion

    #region NConfigManager

    /// <summary>
    /// NConfigManager abstract classs.
    /// </summary>
    public abstract class NConfigManager : IConfigManager
    {
        #region Protected Methods

        /// <summary>
        /// Gets entry assembly file name only.
        /// </summary>
        protected string EntryAssemblyName
        {
            get
            {
                string result = string.Empty;
                Assembly assem = null;
                assem = Assembly.GetEntryAssembly();
                if (null != assem)
                {
                    result = Path.GetFileNameWithoutExtension(assem.Location);
                }
                else result = "app";

                return result;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// ToString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.GetType().Name;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Get Config File Name.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetConfigFileName();

        #endregion

        #region Public Methods

        /// <summary>
        /// Save Config.
        /// </summary>
        /// <returns>Returns instance of NResult.</returns>
        public abstract NResult<bool> SaveConfig();
        /// <summary>
        /// Load Config.
        /// </summary>
        /// <returns>Returns instance of NResult.</returns>
        public abstract NResult<bool> LoadConfig();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Configuration File Name.
        /// </summary>
        [Category("Configs")]
        [Description("Gets Configuration File Name.")]
        public string ConfigFileName { get { return GetConfigFileName(); } }

        #endregion
    }

    #endregion
}