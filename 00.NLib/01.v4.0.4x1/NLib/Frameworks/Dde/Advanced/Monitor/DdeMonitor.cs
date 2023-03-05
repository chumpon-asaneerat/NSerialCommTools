namespace NLib.Dde.Advanced.Monitor
{
    using System;
    using System.Threading;
    using NLib.Dde.Foundation;
    using NLib.Dde.Foundation.Advanced.Monitor;
    using NLib.Dde;

    /// <summary>
    /// This specifies the different kinds of DDE activity that can be monitored.
    /// </summary>
    [Flags]
    public enum DdeMonitorFlags
    {
        /// <summary>
        /// This indicates activity caused by the execution of a DDEML callback.
        /// </summary>
        Callback = DdemlMonitorFlags.Callback,

        /// <summary>
        /// This indicates activity caused by conversation.
        /// </summary>
        Conversation = DdemlMonitorFlags.Conversation,

        /// <summary>
        /// This indicates activity caused by an error.
        /// </summary>
        Error = DdemlMonitorFlags.Error,

        ///// <summary>
        ///// 
        ///// </summary>
        //String = DdemlMonitorFlags.String,

        /// <summary>
        /// This indicates activity caused by an advise loop.
        /// </summary>
        Link = DdemlMonitorFlags.Link,

        /// <summary>
        /// This indicates activity caused by DDE messages.
        /// </summary>
        Message = DdemlMonitorFlags.Message,

    } // enum

    /// <summary>
    /// This is used to monitor DDE activity.
    /// </summary>
    public sealed class DdeMonitor : IDisposable
    {
        private Object _LockObject = new Object();

        private DdemlMonitor _DdemlObject = null; // This has lazy initialization through a property.
        private DdeContext   _Context     = null;
        
        private event EventHandler<DdeCallbackActivityEventArgs>     _CallbackActivityEvent     = null;
        private event EventHandler<DdeConversationActivityEventArgs> _ConversationActivityEvent = null;
        private event EventHandler<DdeErrorActivityEventArgs>        _ErrorActivityEvent        = null;
        private event EventHandler<DdeLinkActivityEventArgs>         _LinkActivityEvent         = null;
        private event EventHandler<DdeMessageActivityEventArgs>      _MessageActivityEvent      = null;
        //private event EventHandler<DdeStringActivityEventArgs>       _StringActivityEvent       = null;

        /// <summary>
        /// This is raised anytime a DDEML callback is executed.
        /// </summary>
        public event EventHandler<DdeCallbackActivityEventArgs> CallbackActivity
        {
            add
            {
                lock (_LockObject)
                {
                    _CallbackActivityEvent += value;
                }
            }
            remove
            {
                lock (_LockObject)
                {
                    _CallbackActivityEvent -= value;
                }
            }
        }

        /// <summary>
        /// This is raised anytime a conversation is established or terminated.
        /// </summary>
        public event EventHandler<DdeConversationActivityEventArgs> ConversationActivity
        {
            add
            {
                lock (_LockObject)
                {
                    _ConversationActivityEvent += value;
                }
            }
            remove
            {
                lock (_LockObject)
                {
                    _ConversationActivityEvent -= value;
                }
            }
        }

        /// <summary>
        /// This is raised anytime there is an error.
        /// </summary>
        public event EventHandler<DdeErrorActivityEventArgs> ErrorActivity
        {
            add
            {
                lock (_LockObject)
                {
                    _ErrorActivityEvent += value;
                }
            }
            remove
            {
                lock (_LockObject)
                {
                    _ErrorActivityEvent -= value;
                }
            }
        }

        /// <summary>
        /// This is raised anytime an advise loop is established or terminated.
        /// </summary>
        public event EventHandler<DdeLinkActivityEventArgs> LinkActivity
        {
            add
            {
                lock (_LockObject)
                {
                    _LinkActivityEvent += value;
                }
            }
            remove
            {
                lock (_LockObject)
                {
                    _LinkActivityEvent -= value;
                }
            }
        }

        /// <summary>
        /// This is raised anytime a DDE message is sent or posted.
        /// </summary>
        public event EventHandler<DdeMessageActivityEventArgs> MessageActivity
        {
            add
            {
                lock (_LockObject)
                {
                    _MessageActivityEvent += value;
                }
            }
            remove
            {
                lock (_LockObject)
                {
                    _MessageActivityEvent -= value;
                }
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public event EventHandler<DdeStringActivityEventArgs> StringActivity
        //{
        //    add
        //    {
        //        lock (_LockObject)
        //        {
        //            _StringActivityEvent += value;
        //        }
        //    }
        //    remove
        //    {
        //        lock (_LockObject)
        //        {
        //            _StringActivityEvent -= value;
        //        }
        //    }
        //}

        /// <summary>
        /// This initializes a new instance of the <c>DdeMonitor</c> class.
        /// </summary>
        public DdeMonitor()
        {
            Context = new DdeContext();
        }

        /// <summary>
        /// This releases all resources held by this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThreadStart method = delegate()
                {
                    DdemlObject.Dispose();
                };

                try
                {
                    Context.Invoke(method);
                }
                catch
                {
                    // Swallow any exception that occurs.
                }
            }
        }

        /// <summary>
        /// This gets the context associated with this instance.
        /// </summary>
        public DdeContext Context
        {
            get
            {
                lock (_LockObject)
                {
                    return _Context;
                }
            }
            private set
            {
                lock (_LockObject)
                {
                    _Context = value;
                }
            }
        }

        /// <summary>
        /// This starts monitoring the system for DDE activity.
        /// </summary>
        /// <param name="flags">
        /// A bitwise combination of <c>DdeMonitorFlags</c> that indicate what DDE activity will be monitored.
        /// </param>
        public void Start(DdeMonitorFlags flags)
        {
            ThreadStart method = delegate()
            {
                DdemlObject.Start((DdemlMonitorFlags)(int)flags);
            };

            try
            {
                Context.Invoke(method);
            }
            catch (DdemlException e)
            {
                throw new DdeException(e);
            }
            catch (ObjectDisposedException e)
            {
                throw new ObjectDisposedException(this.GetType().ToString(), e);
            }
        }

        internal DdemlMonitor DdemlObject
        {
            get
            {
                lock (_LockObject)
                {
                    if (_DdemlObject == null)
                    {
                        _DdemlObject = new DdemlMonitor(Context.DdemlObject);
                        _DdemlObject.CallbackActivity += new EventHandler<DdemlCallbackActivityEventArgs>(this.OnCallbackActivity);
                        _DdemlObject.ConversationActivity += new EventHandler<DdemlConversationActivityEventArgs>(this.OnConversationActivity);
                        _DdemlObject.ErrorActivity += new EventHandler<DdemlErrorActivityEventArgs>(this.OnErrorActivity);
                        _DdemlObject.LinkActivity += new EventHandler<DdemlLinkActivityEventArgs>(this.OnLinkActivity);
                        _DdemlObject.MessageActivity += new EventHandler<DdemlMessageActivityEventArgs>(this.OnMessageActivity);
                        //_DdemlObject.StringActivity += new EventHandler<DdemlStringActivityEventArgs>(this.OnStringActivity);
                    }
                    return _DdemlObject;
                }
            }
        }

        //private void OnStringActivity(object sender, DdemlStringActivityEventArgs e)
        //{
        //    EventHandler<DdeStringActivityEventArgs> copy;

        //    // To make this thread-safe we need to hold a local copy of the reference to the invocation list.  This works because delegates are
        //    //immutable.
        //    lock (_LockObject)
        //    {
        //        copy = _StringActivityEvent;
        //    }

        //    if (copy != null)
        //    {
        //        copy(this, new DdeStringActivityEventArgs(e));
        //    }
        //}

        private void OnMessageActivity(object sender, DdemlMessageActivityEventArgs e)
        {
            EventHandler<DdeMessageActivityEventArgs> copy;

            // To make this thread-safe we need to hold a local copy of the reference to the invocation list.  This works because delegates are
            //immutable.
            lock (_LockObject)
            {
                copy = _MessageActivityEvent;
            }

            if (copy != null)
            {
                copy(this, new DdeMessageActivityEventArgs(e));
            }
        }

        private void OnLinkActivity(object sender, DdemlLinkActivityEventArgs e)
        {
            EventHandler<DdeLinkActivityEventArgs> copy;

            // To make this thread-safe we need to hold a local copy of the reference to the invocation list.  This works because delegates are
            //immutable.
            lock (_LockObject)
            {
                copy = _LinkActivityEvent;
            }

            if (copy != null)
            {
                copy(this, new DdeLinkActivityEventArgs(e));
            }
        }

        private void OnErrorActivity(object sender, DdemlErrorActivityEventArgs e)
        {
            EventHandler<DdeErrorActivityEventArgs> copy;

            // To make this thread-safe we need to hold a local copy of the reference to the invocation list.  This works because delegates are
            //immutable.
            lock (_LockObject)
            {
                copy = _ErrorActivityEvent;
            }

            if (copy != null)
            {
                copy(this, new DdeErrorActivityEventArgs(e));
            }
        }

        private void OnConversationActivity(object sender, DdemlConversationActivityEventArgs e)
        {
            EventHandler<DdeConversationActivityEventArgs> copy;

            // To make this thread-safe we need to hold a local copy of the reference to the invocation list.  This works because delegates are
            //immutable.
            lock (_LockObject)
            {
                copy = _ConversationActivityEvent;
            }

            if (copy != null)
            {
                copy(this, new DdeConversationActivityEventArgs(e));
            }
        }

        private void OnCallbackActivity(object sender, DdemlCallbackActivityEventArgs e)
        {
            EventHandler<DdeCallbackActivityEventArgs> copy;

            // To make this thread-safe we need to hold a local copy of the reference to the invocation list.  This works because delegates are
            //immutable.
            lock (_LockObject)
            {
                copy = _CallbackActivityEvent;
            }

            if (copy != null)
            {
                copy(this, new DdeCallbackActivityEventArgs(e));
            }
        }

    } // class

} // namespace