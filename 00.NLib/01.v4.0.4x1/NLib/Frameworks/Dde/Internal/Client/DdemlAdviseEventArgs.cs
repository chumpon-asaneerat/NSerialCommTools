namespace NLib.Dde.Foundation.Client
{
    using System;

    internal sealed class DdemlAdviseEventArgs : DdemlEventArgs
    {
        private string _Item   = "";
        private int    _Format = 0;
        private object _State  = null;
        private byte[] _Data   = null;

        public DdemlAdviseEventArgs(string item, int format, object state, byte[] data)
        {
            _Item = item;
            _Format = format;
            _State = state;
            _Data = data;
        }

        public string Item
        {
            get { return _Item; }
        }

        public int Format
        {
            get { return _Format; }
        }

        public object State
        {
            get { return _State; }
        }

        public byte[] Data
        {
            get { return _Data; }
        }

    } // class

} // namespace