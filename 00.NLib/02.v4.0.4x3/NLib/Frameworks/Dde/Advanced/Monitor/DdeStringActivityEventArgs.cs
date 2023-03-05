namespace NLib.Dde.Advanced.Monitor
{
    using System;
    using NLib.Dde.Foundation.Advanced.Monitor;

    /// <summary>
    /// This represents the type of string activity.
    /// </summary>
    public enum DdeStringActivityType
    {
        /// <summary>
        /// The DDE application called DdeUninitialize.
        /// </summary>
        CleanUp = DdemlStringActivityType.CleanUp,

        /// <summary>
        /// The DDE application is creating a string handle.
        /// </summary>
        Create = DdemlStringActivityType.Create,

        /// <summary>
        /// The DDE application is deleting a string handle.
        /// </summary>
        Delete = DdemlStringActivityType.Delete,

        /// <summary>
        /// The DDE application is incrementing the reference count of an existing string handle.
        /// </summary>
        Keep = DdemlStringActivityType.Keep

    } // enum

    /// <summary>
    /// This contains information about the <c>StringActivity</c> event.
    /// </summary>
    public sealed class DdeStringActivityEventArgs : DdeActivityEventArgs
    {
        private DdemlStringActivityEventArgs _DdemlObject = null;

        internal DdeStringActivityEventArgs(DdemlStringActivityEventArgs args) : base(args)
        {
            _DdemlObject = args;
        }

        /// <summary>
        /// This gets the text associated with the string handle.
        /// </summary>
        public string Value
        {
            get { return _DdemlObject.Value; }
        }

        /// <summary>
        /// This gets the action being performed.
        /// </summary>
        public DdeStringActivityType Action
        {
            get { return (DdeStringActivityType)((int)_DdemlObject.Action); }
        }

    } // class

} // namespace