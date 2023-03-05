namespace NLib.Dde.Advanced.Monitor
{
    using System;
    using NLib.Dde.Foundation.Advanced.Monitor;

    /// <summary>
    /// This contains information about the <c>ConversationActivity</c> event.
    /// </summary>
    public sealed class DdeConversationActivityEventArgs : DdeActivityEventArgs
    {
        private DdemlConversationActivityEventArgs _DdemlObject = null;

        internal DdeConversationActivityEventArgs(DdemlConversationActivityEventArgs args) : base(args)
        {
            _DdemlObject = args;
        }

        /// <summary>
        /// This gets the service name associated with the conversation.
        /// </summary>
        public string Service
        {
            get { return _DdemlObject.Service; }
        }

        /// <summary>
        /// This gets the topic name associated with the conversation.
        /// </summary>
        public string Topic
        {
            get { return _DdemlObject.Topic; }
        }

        /// <summary>
        /// This gets a bool indicating whether the conversation is being established.
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
        /// This gets the handle to the client application associated with the conversation.
        /// </summary>
        public IntPtr ClientHandle
        {
            get { return _DdemlObject.ClientHandle; }
        }

        /// <summary>
        /// This gets the handle to the server application associated with the conversation.
        /// </summary>
        public IntPtr ServerHandle
        {
            get { return _DdemlObject.ServerHandle; }
        }

    } // class

} // namespace