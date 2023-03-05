namespace NLib.Dde.Advanced
{
    using System;
    using NLib.Dde.Foundation.Advanced;

    /// <summary>
    /// This contains information about the <c>Register</c> and <c>Unregister</c> events.
    /// </summary>
    /// <threadsafety static="true" instance="false" />
    public sealed class DdeRegistrationEventArgs : DdeEventArgs
    {
        private DdemlRegistrationEventArgs _DdemlObject = null;

        internal DdeRegistrationEventArgs(DdemlRegistrationEventArgs args)
        {
        }

        /// <summary>
        /// This gets the service name associated with this event.
        /// </summary>
        public string Service
        {
            get { return _DdemlObject.Service; }
        }

    } // class

} // namespace