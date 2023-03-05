#define USE_LOGS

#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-17
=================
- DdeClientComponent and related classes code updated.
  - Changes log code (used MethodBase).

======================================================================================================================
Update 2010-01-22
=================
- DdeClientComponent and related classes code updated.
  - DdeClientComponent class change Exception handle related code to used new version of 
    ExceptionManager.

======================================================================================================================
Update 2010-01-19
=================
- DdeClientComponent and related classes code updated.
  - Change misspell name to DdeClientComponent.
  - ExcelDdeClient Change misspell base class name to DdeClientComponent.
  - InTouchDdeClient Change misspell base class name to DdeClientComponent.
  - All code related to debug now used DebugManager instead.

======================================================================================================================
Update 2010-01-03
=================
- The DDE Client components classes updated.
  - SCADA DDE Client (abstract) class moved from GFA37 to GFA38.
  - Excel DDE Client class moved from GFA37 to GFA38.
  - InTouch DDE Client class  moved from GFA37 to GFA38.
  - Changes namespace from SysLib.SCADA.Lib.Dde and SysLib.SCADA.Components
    to SysLib.Components.Scada.
  - SCADADDEClient class changed name to DdeClientComponet
  - Temporary remove log
  - ExcelDDEClient class changed name to ExcelDdeClient
  - InTouchDDEClient class changed name to InTouchDdeClient

======================================================================================================================
Update 2008-09-29
=================
- The DDE Client Framework classes added.
  - SCADA DDE Client (abstract) class added.
  - Excel DDE Client class added.
  - InTouch DDE Client class added.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

#endregion

#region Extra Using

using NLib.Dde.Client;

#endregion

namespace NLib.Components.Scada
{
    #region DdeClientComponent

    /// <summary>
    /// DDE Client abstract class.
    /// </summary>
    public abstract class DdeClientComponent : Component
    {
        #region Internal Variable

        private DdeClient _client = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public DdeClientComponent() : base() { }
        /// <summary>
        /// Destructor
        /// </summary>
        ~DdeClientComponent()
        {
            Disconnect();
        }

        #endregion

        #region Abstract Method

        /// <summary>
        /// Get Service's Name.
        /// </summary>
        protected abstract string ServiceName { get; }
        /// <summary>
        /// Get Topic's Name.
        /// </summary>
        protected abstract string TopicName { get; }

        #endregion

        #region Connect/Disconnect

        /// <summary>
        /// Check is connected or not.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return (_client != null && _client.IsConnected);
            }
        }
        /// <summary>
        /// Connect to DDE Server.
        /// </summary>
        public void Connect()
        {
            if (_client != null)
                return;
            try
            {
                _client = new DdeClient(this.ServiceName, this.TopicName);
                _client.Connect();
            }
            catch (Exception ex)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                string msg = "Connect detected exception.";
#if USE_LOGS
                msg.Err(med);
                ex.Err(med);
#else
                Console.WriteLine(msg);
                Console.WriteLine(ex);
#endif
                _client = null;
            }
            finally
            {
            }
        }
        /// <summary>
        /// Disconnect from DDE Server.
        /// </summary>
        public void Disconnect()
        {
            if (_client == null)
                return;
            try
            {
                _client.Disconnect();
                _client.Dispose();
            }
            catch (Exception ex)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                string msg = "Disconnect detected exception.";
#if USE_LOGS
                msg.Err(med);
                ex.Err(med);
#else
                Console.WriteLine(msg);
                Console.WriteLine(ex);
#endif
            }
            finally
            {
                _client = null;
            }
        }

        #endregion

        #region Get Data (generic)

        /// <summary>
        /// Get Data (Generic version.)
        /// </summary>
        /// <typeparam name="T">The target data type.</typeparam>
        /// <param name="itemName">The item's name to get data.</param>
        /// <returns>Return data that read from DDE server. If error occur the default value
        /// of speficied type is return.</returns>
        public T GetData<T>(string itemName)
        {
            return GetData<T>(itemName, default(T));
        }
        /// <summary>
        /// Get Data (Generic version.)
        /// </summary>
        /// <typeparam name="T">The target data type.</typeparam>
        /// <param name="itemName">The item's name to get data.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Return data that read from DDE server. If error occur the default value
        /// of speficied type is return.</returns>
        public T GetData<T>(string itemName, T defaultValue)
        {
            bool timeout = false;
            string result = GetData(itemName, 1000, ref timeout);

            try
            {
                T obj = (T)Convert.ChangeType(result, typeof(T));
                return obj;
            }
            catch (Exception ex)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                string msg = "GetData detected exception.";
#if USE_LOGS
                msg.Err(med);
                ex.Err(med);
#else
                Console.WriteLine(msg);
                Console.WriteLine(ex);
#endif
                return defaultValue;
            }
        }

        #endregion

        #region Get Data

        /// <summary>
        /// Get Data
        /// </summary>
        /// <param name="itemName">The item's name to get data.</param>
        /// <returns>Return data in string that read from DDE server.</returns>
        public string GetData(string itemName)
        {
            bool timeout = false;
            return GetData(itemName, 1000, ref timeout);
        }
        /// <summary>
        /// Get Data
        /// </summary>
        /// <param name="itemName">The item's name to get data.</param>
        /// <param name="timeout">The timeout interval.</param>
        /// <param name="isTimeout">The reference variable to notify timeoout flag.</param>
        /// <returns>Return data in string that read from DDE server.</returns>
        public string GetData(string itemName, int timeout, ref bool isTimeout)
        {
            string result = string.Empty;

            if (_client == null)
            {
                isTimeout = true;
                return result;
            }

            try
            {
                result = _client.Request(itemName, timeout);
            }
            catch (Exception ex)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                string msg = "GetData detected exception.";
#if USE_LOGS
                msg.Err(med);
                ex.Err(med);
#else
                Console.WriteLine(msg);
                Console.WriteLine(ex);
#endif
            }

            return result;
        }

        #endregion
    }

    #endregion
}

namespace NLib.Components.Scada
{
    #region Excel Dde Client

    /// <summary>
    /// Excel Dde Client
    /// </summary>
    public sealed class ExcelDdeClient : DdeClientComponent
    {
        #region Internal Variable

        private string _sheetName = "SHEET1";

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ExcelDdeClient() : base() { }
        /// <summary>
        /// Destructor
        /// </summary>
        ~ExcelDdeClient()
        {
        }

        #endregion

        #region Abstract override

        /// <summary>
        /// Get Service's Name.
        /// </summary>
        protected override string ServiceName
        {
            get { return "EXCEL"; }
        }
        /// <summary>
        /// Get Topic's Name.
        /// </summary>
        protected override string TopicName
        {
            get { return _sheetName; }
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Sheet Name to read/write data.
        /// </summary>
        public string SheetName
        {
            get { return _sheetName; }
            set
            {
                if (_sheetName != value)
                {
                    _sheetName = value;
                }
            }
        }

        #endregion
    }

    #endregion
}

namespace NLib.Components.Scada
{
    #region InTouch Dde Client

    /// <summary>
    /// InTouch Dde Client.
    /// </summary>
    public sealed class InTouchDdeClient : DdeClientComponent
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public InTouchDdeClient() : base() { }
        /// <summary>
        /// Destructor
        /// </summary>
        ~InTouchDdeClient()
        {
        }

        #endregion

        #region Abstract override

        /// <summary>
        /// Get Service's Name.
        /// </summary>
        protected override string ServiceName
        {
            get { return "VIEW"; }
        }
        /// <summary>
        /// Get Topic's Name.
        /// </summary>
        protected override string TopicName
        {
            get { return "TAGNAME"; }
        }

        #endregion
    }

    #endregion
}
