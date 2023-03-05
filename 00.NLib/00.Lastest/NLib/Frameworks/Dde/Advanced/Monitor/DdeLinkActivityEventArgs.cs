namespace NLib.Dde.Advanced.Monitor
{
    using System;
    using NLib.Dde.Foundation.Advanced.Monitor;

    /// <summary>
    /// This contains information about the <c>LinkActivity</c> event.
    /// </summary>
    public sealed class DdeLinkActivityEventArgs : DdeActivityEventArgs
    {
        private DdemlLinkActivityEventArgs _DdemlObject = null;

        internal DdeLinkActivityEventArgs(DdemlLinkActivityEventArgs args) : base(args)
        {
            _DdemlObject = args;
        }

        /// <summary>
        /// This gets the service name associated with the link.
        /// </summary>
        public string Service
        {
            get { return _DdemlObject.Service; }
        }

        /// <summary>
        /// This gets the topic name associated with the link.
        /// </summary>
        public string Topic
        {
            get { return _DdemlObject.Topic; }
        }

        /// <summary>
        /// This gets the item name associated with the link.
        /// </summary>
        public string Item
        {
            get { return _DdemlObject.Item; }
        }

        /// <summary>
        /// This gets the format of the data associated with the link.
        /// </summary>
        public int Format
        {
            get { return _DdemlObject.Format; }
        }

        /// <summary>
        /// This gets a bool indicating whether the link is hot.
        /// </summary>
        public bool IsHot
        {
            get { return _DdemlObject.IsHot; }
        }

        /// <summary>
        /// This gets a bool indicating whether the link is being established.
        /// </summary>
        /// <remarks>
        /// The value returned by this property will be true if the conversation is being established.  If the conversation
        /// is being terminated then the value will be false.
        /// </remarks>
        public bool IsEstablished
        {
            get { return _DdemlObject.IsEstablished; }
        }

        /// <summary>
        /// This gets a bool indicating whether the link was terminated by the server.
        /// </summary>
        public bool IsServerInitiated
        {
            get { return _DdemlObject.IsServerInitiated; }
        }

        /// <summary>
        /// This gets the handle to the client application associated with the link.
        /// </summary>
        public IntPtr ClientHandle
        {
            get { return _DdemlObject.ClientHandle; }
        }

        /// <summary>
        /// This gets the handle to the server application associated with the link.
        /// </summary>
        public IntPtr ServerHandle
        {
            get { return _DdemlObject.ServerHandle; }
        }

    } // class

} // namespace