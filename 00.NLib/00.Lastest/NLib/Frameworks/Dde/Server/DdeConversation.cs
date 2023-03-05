namespace NLib.Dde.Server
{
    using System;
    using NLib.Dde.Foundation.Server;
    
    /// <summary>
    /// This represents a DDE conversation established on a <c>DdeServer</c>.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public sealed class DdeConversation
    {
        private Object _LockObject = new Object();

        private DdemlConversation _DdemlObject = null;
        private object            _Tag = null;

        // These are used to cache the property values of the DdemlConversation.
        private string _Service  = "";
        private string _Topic    = "";
        private IntPtr _Handle   = IntPtr.Zero;
        private bool   _IsPaused = false;

        internal DdeConversation(DdemlConversation conversation)
        {
            _DdemlObject = conversation;
            _DdemlObject.StateChange += this.OnStateChange;
            _Service = _DdemlObject.Service;
            _Topic = _DdemlObject.Topic;
            _Handle = _DdemlObject.Handle;
            _IsPaused = _DdemlObject.IsPaused;
        }

        internal DdemlConversation DdemlObject
        {
            get
            {
                lock (_LockObject)
                {
                    return _DdemlObject;
                }
            }
        }

        /// <summary>
        /// This gets the service name associated with this conversation.
        /// </summary>
        public string Service
        {
            get
            {
                lock (_LockObject)
                {
                    return _Service;
                }
            }
        }

        /// <summary>
        /// This gets the topic name associated with this conversation.
        /// </summary>
        public string Topic
        {
            get
            {
                lock (_LockObject)
                {
                    return _Topic;
                }
            }
        }

        /// <summary>
        /// This gets the DDEML handle associated with this conversation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This can be used in any DDEML function requiring a conversation handle.
        /// </para>
        /// <para>
        /// <note type="caution">
        /// Incorrect usage of the DDEML can cause this object to function incorrectly and can lead to resource leaks.
        /// </note>
        /// </para>
        /// </remarks>
        public IntPtr Handle
        {
            get
            {
                lock (_LockObject)
                {
                    return _Handle;
                }
            }
        }

        /// <summary>
        /// This gets a bool indicating whether this conversation is paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                lock (_LockObject)
                {
                    return _IsPaused;
                }
            }
        }

        /// <summary>
        /// This gets an application defined data object associated with this conversation.
        /// </summary>
        /// <remarks>
        /// Use this property to carry state information with the conversation.
        /// </remarks>
        public object Tag
        {
            get
            {
                lock (_LockObject)
                {
                    return _Tag;
                }
            }
            set
            {
                lock (_LockObject)
                {
                    _Tag = value;
                }
            }
        }

        /// <summary>
        /// This returns a string containing the current values of all properties.
        /// </summary>
        /// <returns>
        /// A string containing the current values of all properties.
        /// </returns>
        public override string ToString()
        {
            string s = "";
            foreach (System.Reflection.PropertyInfo property in this.GetType().GetProperties())
            {
                if (s.Length == 0)
                {
                    s += property.Name + "=" + property.GetValue(this, null).ToString();
                }
                else
                {
                    s += " " + property.Name + "=" + property.GetValue(this, null).ToString();
                }
            }
            return s;
        }

        private void OnStateChange(object sender, EventArgs args)
        {
            lock (_LockObject)
            {
                _Service = _DdemlObject.Service;
                _Topic = _DdemlObject.Topic;
                _Handle = _DdemlObject.Handle;
                _IsPaused = _DdemlObject.IsPaused;
            }
        }

    } // class

} // namespace