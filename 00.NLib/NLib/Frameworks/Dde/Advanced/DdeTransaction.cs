namespace NLib.Dde.Advanced
{
    using System;
    using NLib.Dde.Foundation.Advanced;

    /// <summary>
    /// This contains the parameters of the DDEML callback function.
    /// </summary>
    /// <threadsafety static="true" instance="false" />
    /// <remarks>
    /// <para>
    /// The <c>dwRet</c> property contains the value returned by the DDEML callback function and is the only member that can be modified.  See the
    /// MSDN documentation for more information about the members of this class.
    /// </para>
    /// <para>
    /// <note type="caution">
    /// Incorrect usage of the DDEML can cause this library to function incorrectly and can lead to resource leaks.
    /// </note>
    /// </para>
    /// </remarks>
    public sealed class DdeTransaction
    {
        private DdemlTransaction _DdemlObject = null;

        internal DdeTransaction(DdemlTransaction transaction)
        {
            _DdemlObject = transaction;
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
        /// <remarks>
        /// This will be ignored if the PreFilterTransaction method returns false.
        /// </remarks>
        public IntPtr dwRet
        {
            get { return _DdemlObject.dwRet; }
            set { _DdemlObject.dwRet = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

    } // class

} // namespace