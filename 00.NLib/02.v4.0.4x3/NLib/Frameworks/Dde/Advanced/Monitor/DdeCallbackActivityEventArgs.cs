namespace NLib.Dde.Advanced.Monitor
{
    using System;
    using NLib.Dde.Foundation.Advanced.Monitor;

    /// <summary>
    /// This contains information about the <c>CallbackActivity</c> event.
    /// </summary>
    public sealed class DdeCallbackActivityEventArgs : DdeActivityEventArgs
    {
        private DdemlCallbackActivityEventArgs _DdemlObject = null;

        internal DdeCallbackActivityEventArgs(DdemlCallbackActivityEventArgs args) : base(args)
        {
            _DdemlObject = args;
        }

        /// <summary>
        /// See the MSDN documentation for information about this member.
        /// </summary>
        public int uType
        {
            get { return _DdemlObject.uType; }
        }

        /// <summary>
        /// See the MSDN documentation for information about this member.
        /// </summary>
        public int uFmt
        {
            get { return _DdemlObject.uFmt; }
        }

        /// <summary>
        /// See the MSDN documentation for information about this member.
        /// </summary>
        public IntPtr hConv
        {
            get { return _DdemlObject.hConv; }
        }

        /// <summary>
        /// See the MSDN documentation for information about this member.
        /// </summary>
        public IntPtr hsz1
        {
            get { return _DdemlObject.hsz1; }
        }

        /// <summary>
        /// See the MSDN documentation for information about this member.
        /// </summary>
        public IntPtr hsz2
        {
            get { return _DdemlObject.hsz2; }
        }

        /// <summary>
        /// See the MSDN documentation for information about this member.
        /// </summary>
        public IntPtr hData
        {
            get { return _DdemlObject.hData; }
        }

        /// <summary>
        /// See the MSDN documentation for information about this member.
        /// </summary>
        public IntPtr dwData1
        {
            get { return _DdemlObject.dwData1; }
        }

        /// <summary>
        /// See the MSDN documentation for information about this member.
        /// </summary>
        public IntPtr dwData2
        {
            get { return _DdemlObject.dwData2; }
        }

        /// <summary>
        /// This gets the return value of the DDEML callback function.  See the MSDN documentation for information about this member.
        /// </summary>
        public IntPtr dwRet
        {
            get { return _DdemlObject.dwRet; }
        }

    } // class

} // namespace