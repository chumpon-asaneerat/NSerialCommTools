namespace NLib.Dde
{
    using System;
    using System.Runtime.Serialization;
    using NLib.Dde.Foundation;
    using NLib.Dde;

    /// <summary>
    /// This is thrown when a DDE exception occurs.
    /// </summary>
    /// <threadsafety static="true" instance="false" />
    [Serializable]
    public class DdeException : Exception
    {
        private DdemlException _DdemlObject = null;

        internal DdeException(string message) : this(new DdemlException(message))
        {
        }

        internal DdeException(DdemlException exception) : base(exception.Message, exception)
        { 
            _DdemlObject = exception;
        }

        /// <summary>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected DdeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _DdemlObject = (DdemlException)info.GetValue("NLib.Dde.DdeException.DdemlObject", typeof(DdemlException));
        }

        /// <summary>
        /// This gets an error code returned by the DDEML.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value is zero if the exception was not thrown because of the DDEML.
        /// </para>
        /// <para>
        /// <list type="bullet">
        /// <item><term>0x0000</term><description>DMLERR_NO_DMLERROR</description></item>
        /// <item><term>0x4000</term><description>DMLERR_ADVACKTIMEOUT</description></item>
        /// <item><term>0x4001</term><description>DMLERR_BUSY</description></item>
        /// <item><term>0x4002</term><description>DMLERR_DATAACKTIMEOUT</description></item>
        /// <item><term>0x4003</term><description>DMLERR_DLL_NOT_INITIALIZED</description></item>
        /// <item><term>0x4004</term><description>DMLERR_DLL_USAGE</description></item>
        /// <item><term>0x4005</term><description>DMLERR_EXECACKTIMEOUT</description></item>
        /// <item><term>0x4006</term><description>DMLERR_INVALIDPARAMETER</description></item>
        /// <item><term>0x4007</term><description>DMLERR_LOW_MEMORY</description></item>
        /// <item><term>0x4008</term><description>DMLERR_MEMORY_DMLERROR</description></item>
        /// <item><term>0x4009</term><description>DMLERR_NOTPROCESSED</description></item>
        /// <item><term>0x400A</term><description>DMLERR_NO_CONV_ESTABLISHED</description></item>
        /// <item><term>0x400B</term><description>DMLERR_POKEACKTIMEOUT</description></item>
        /// <item><term>0x400C</term><description>DMLERR_POSTMSG_FAILED</description></item>
        /// <item><term>0x400D</term><description>DMLERR_REENTRANCY</description></item>
        /// <item><term>0x400E</term><description>DMLERR_SERVER_DIED</description></item>
        /// <item><term>0x400F</term><description>DMLERR_SYS_DMLERROR</description></item>
        /// <item><term>0x4010</term><description>DMLERR_UNADVACKTIMEOUT</description></item>
        /// <item><term>0x4011</term><description>DMLERR_UNFOUND_QUEUE_ID</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public int Code
        {
            get { return _DdemlObject.Code; }
        }

        /// <summary>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("NLib.Dde.DdeException.DdemlObject", _DdemlObject);
            base.GetObjectData(info, context);
        }

    } // class

} // namespace