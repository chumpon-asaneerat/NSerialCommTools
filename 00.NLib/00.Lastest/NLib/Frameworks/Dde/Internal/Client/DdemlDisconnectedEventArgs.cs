namespace NLib.Dde.Foundation.Client
{
    using System;

    internal sealed class DdemlDisconnectedEventArgs : DdemlEventArgs
    {
        private bool _ServerInitiated = false;
        private bool _Disposed        = false;

        public DdemlDisconnectedEventArgs(bool serverInitiated, bool disposed)
        {
            _ServerInitiated = serverInitiated;
            _Disposed = disposed;
        }

        public bool IsServerInitiated
        {
            get { return _ServerInitiated; }
        }

        public bool IsDisposed
        {
            get { return _Disposed; }
        }

    } // class

} // namespace