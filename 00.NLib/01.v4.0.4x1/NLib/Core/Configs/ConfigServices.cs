#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-08-07
=================
- NLib Services - ConfigService.
  - Add ConfigService class.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using NLib.Configs;

#endregion

namespace NLib.Services
{
    #region ConfigService

    /// <summary>
    /// ConfigService class.
    /// </summary>
    public class ConfigService
    {
        #region Singelton

        private static ConfigService _instance = null;
        /// <summary>
        /// Singelton Access Instance.
        /// </summary>
        public static ConfigService Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(ConfigService))
                    {
                        _instance = new ConfigService();
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Internal Variables

        private Dictionary<Type, IConfigManager> _configs = 
            new Dictionary<Type, IConfigManager>();

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private ConfigService() : base() { }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~ConfigService() { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Register config manager.
        /// </summary>
        /// <typeparam name="T">The type of config manager.</typeparam>
        /// <param name="configManager">The instance of config manager.</param>
        /// <returns>Returns instance of ConfigService.</returns>
        public ConfigService Register<T>(T configManager)
            where T : IConfigManager
        {
            Type type = typeof(T);
            if (null != configManager)
            {
                if (!_configs.ContainsKey(type))
                {
                    lock (this)
                    {
                        _configs.Add(type, configManager);
                    }
                    "{0} is registered.".Info(type.Name);
                }
                else
                {
                    "{0} is already registered.".Info(type.Name);
                }
            }
            else
            {
                "{0} instance is null.".Err(type.Name);
            }

            return this;
        }
        /// <summary>
        /// End.
        /// </summary>
        public void End()
        {

        }
        /// <summary>
        /// Clear all registered config manager.
        /// </summary>
        /// <returns></returns>
        public ConfigService Clear()
        {
            lock (this)
            {
                _configs.Clear();
            }
            return this;
        }

        /// <summary>
        /// Gets Config Manager instance by type.
        /// </summary>
        /// <typeparam name="T">The Config Manager type.</typeparam>
        /// <returns>Returns IConfigManager instance.</returns>
        public T GetConfig<T>()
            where T : IConfigManager
        {
            T result = default(T);

            Type type = typeof(T);
            if (!_configs.ContainsKey(type))
            {
                "{0} is not found.".Info(type.Name);
            }
            else
            {
                IConfigManager inst = _configs[type];
                if (null == inst || !inst.GetType().Equals(type))
                {
                    "{0} instance is null or not same as specificed type.".Info(type.Name);

                }
                else
                {
                    try
                    {
                        result = (T)inst;
                    }
                    catch (Exception ex)
                    {
                        ex.Err();
                        result = default(T);
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Gets all registered config manager.
        /// </summary>
        /// <returns>Returns list of all config manager that registered into config service.</returns>
        public List<IConfigManager> GetConfigs()
        {
            List<IConfigManager> results = null;
            lock (this)
            {
                results = new List<IConfigManager>();
                results.AddRange(_configs.Values.ToArray());
            }
            return results;
        }

        #endregion
    }

    #endregion
}