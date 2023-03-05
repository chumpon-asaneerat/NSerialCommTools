namespace NLib.Dde.Advanced.Monitor
{
    using System;
    using System.Windows.Forms;
    using NLib.Dde.Foundation.Advanced.Monitor;

    /// <summary>
    /// This represents the kind of message contained in <c>DdeMessageActivityEventArgs</c>.
    /// </summary>
    public enum DdeMessageActivityKind
    {
        /// <summary>
        /// The message was posted by a DDE application.
        /// </summary>
        Post,

        /// <summary>
        /// The message was sent by a DDE application.
        /// </summary>
        Send

    } // enum

    /// <summary>
    /// This contains information about the <c>MessageActivity</c> event.
    /// </summary>
    public sealed class DdeMessageActivityEventArgs : DdeActivityEventArgs
    {
        private DdemlMessageActivityEventArgs _DdemlObject = null;

        internal DdeMessageActivityEventArgs(DdemlMessageActivityEventArgs args) : base(args)
        {
            _DdemlObject = args;
        }

        /// <summary>
        /// This gets the kind of message associated with this event.
        /// </summary>
        public DdeMessageActivityKind Kind
        {
            get { return (DdeMessageActivityKind)(int)_DdemlObject.Kind; }
        }

        /// <summary>
        /// This gets the message associated with this event.
        /// </summary>
        public Message Message
        {
            get { return _DdemlObject.Message; }
        }

    } // class

} // namespace