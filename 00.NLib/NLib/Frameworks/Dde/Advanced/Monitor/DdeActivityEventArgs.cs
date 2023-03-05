namespace NLib.Dde.Advanced.Monitor
{
    using System;
    using NLib.Dde.Foundation.Advanced.Monitor;

    /// <summary>
    /// This contains information about events on <c>DdeMonitor</c>.
    /// </summary>
    public abstract class DdeActivityEventArgs : DdeEventArgs
    {
        private DdemlActivityEventArgs _DdemlObject = null;

        internal DdeActivityEventArgs(DdemlActivityEventArgs args)
        {
            _DdemlObject = args;
        }

        /// <summary>
        /// This gets the task handle of the application associated with this event.
        /// </summary>
        public IntPtr TaskHandle
        {
            get { return _DdemlObject.TaskHandle; }
        }

    } // class

} // namespace