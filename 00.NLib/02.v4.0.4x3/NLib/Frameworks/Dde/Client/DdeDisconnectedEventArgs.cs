namespace NLib.Dde.Client
{
    using System;
    using NLib.Dde.Foundation.Client;

    /// <summary>
    /// This contains information about the <c>Disconnected</c> event.
    /// </summary>
    /// <threadsafety static="true" instance="false" />
    public sealed class DdeDisconnectedEventArgs : DdeEventArgs
    {
        private DdemlDisconnectedEventArgs _DdemlObject = null;

        internal DdeDisconnectedEventArgs(DdemlDisconnectedEventArgs args)
        {
            _DdemlObject = args;
        }

        /// <summary>
        /// This gets a bool indicating whether the client disconnected because of the server.
        /// </summary>
        public bool IsServerInitiated
        {
            get { return _DdemlObject.IsServerInitiated; }
        }

        /// <summary>
        /// This gets a bool indicating whether the client disconnected because <c>Dispose</c> was explicitly called.
        /// </summary>
        /// <remarks>
        /// The value will be true if <c>Dispose</c> was explicitly called on <c>DdeClient</c>.  The <c>DdeClient</c> sending this event has 
        /// been disposed and can no longer be accessed.  Any exception thrown in the currently executing method will be ignored.
        /// </remarks>
        public bool IsDisposed
        {
            get { return _DdemlObject.IsDisposed; }
        }

    } // class

} // namespace