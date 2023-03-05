namespace NLib.Dde.Advanced.Monitor
{
    using System;
    using NLib.Dde.Foundation.Advanced.Monitor;

    /// <summary>
    /// This contains information about the <c>ErrorActivity</c> event.
    /// </summary>
    public sealed class DdeErrorActivityEventArgs : DdeActivityEventArgs
    {
        private DdemlErrorActivityEventArgs _DdemlObject = null;

        internal DdeErrorActivityEventArgs(DdemlErrorActivityEventArgs args) : base(args)
        {
            _DdemlObject = args;
        }

        /// <summary>
        /// This gets an error code returned by the DDEML.
        /// </summary>
        public int Code
        {
            get { return _DdemlObject.Code; }
        }

    } // class

} // namespace