namespace NLib.Dde.Foundation.Server
{
    using System;

    internal sealed class DdemlConversation
    {
        private IntPtr _Handle  = IntPtr.Zero;
        private string _Service = "";
        private string _Topic   = "";
        private int    _Waiting = 0;
        private object _Tag     = null;

        internal event EventHandler StateChange;

        public DdemlConversation(IntPtr handle, string service, string topic)
        {
            _Handle = handle;
            _Service = service;
            _Topic = topic;
        }

        public IntPtr Handle
        {
            get { return _Handle; }
        }

        public string Topic
        {
            get { return _Topic; }
        }

        public string Service
        {
            get { return _Service; }
        }

        public bool IsPaused
        {
            get { return _Waiting > 0; }
        }

        public object Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }

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

        internal void IncrementWaiting()
        {
            _Waiting++;
            if (StateChange != null)
            {
                StateChange(this, EventArgs.Empty);
            }
        }

        internal void DecrementWaiting()
        {
            _Waiting--;
            if (StateChange != null)
            {
                StateChange(this, EventArgs.Empty);
            }
        }

    } // class

} // namespace