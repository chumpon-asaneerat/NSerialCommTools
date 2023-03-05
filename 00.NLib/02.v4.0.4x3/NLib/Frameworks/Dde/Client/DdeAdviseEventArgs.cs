namespace NLib.Dde.Client
{
    using System;
    using System.Text;
    using NLib.Dde.Foundation.Client;

    /// <summary>
    /// This contains information about the <c>Advise</c> event.
    /// </summary>
    /// <threadsafety static="true" instance="false" />
    public sealed class DdeAdviseEventArgs : DdeEventArgs
    {
        private DdemlAdviseEventArgs _DdemlObject = null;
        private Encoding             _Encoding    = null;

        internal DdeAdviseEventArgs(DdemlAdviseEventArgs args, Encoding encoding)
        {
            _DdemlObject = args;
            _Encoding = encoding;
        }

        /// <summary>
        /// This gets the item name associated with this notification.
        /// </summary>
        public string Item
        {
            get { return _DdemlObject.Item; }
        }

        /// <summary>
        /// This gets the format of the data included in this notification.
        /// </summary>
        public int Format
        {
            get { return _DdemlObject.Format; }
        }

        /// <summary>
        /// This gets an application defined data object associated with this advise loop.
        /// </summary>
        public object State
        {
            get { return _DdemlObject.State; }
        }

        /// <summary>
        /// This gets the data associated with this notification or null if this is not a hot advise loop.
        /// </summary>
        public byte[] Data
        {
            get { return _DdemlObject.Data; }
        }

        /// <summary>
        /// This gets the text associated with this notification or null if this is not a hot advise loop.
        /// </summary>
        public string Text
        {
            get 
            {
                if (_DdemlObject.Data != null)
                {
                    return _Encoding.GetString(_DdemlObject.Data);
                }
                return null;
            }
        }

    } // class

} // namespace