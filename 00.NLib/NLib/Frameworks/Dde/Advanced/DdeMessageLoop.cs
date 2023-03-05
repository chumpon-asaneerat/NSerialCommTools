namespace NLib.Dde.Advanced
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// This is a synchronizing object that can run a message loop on any thread.
    /// </summary>
    /// <threadsafety static="true" instance="false" />
    public sealed class DdeMessageLoop : IDisposable, ISynchronizeInvoke
    {
        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        private int  _ThreadId = GetCurrentThreadId();
        private Form _Form     = new HiddenForm();

        /// <summary>
        /// This initializes a new instance of the <c>DdeMessageLoop</c> class.
        /// </summary>
        public DdeMessageLoop()
        {
        }

        /// <summary>
        /// This releases all resources held by this instance.
        /// </summary>
        public void Dispose()
        {
            _Form.Dispose();
        }

        /// <summary>
        /// This begins an asynchronous operation to execute a delegate on the thread hosting this object.
        /// </summary>
        /// <param name="method">
        /// The delegate to execute.
        /// </param>
        /// <param name="args">
        /// The arguments to pass to the delegate.
        /// </param>
        /// <returns>
        /// An <c>IAsyncResult</c> object for this operation.
        /// </returns>
        IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
        {
            return _Form.BeginInvoke(method, args);
        }

        /// <summary>
        /// This returns the object that the delegate returned in the operation.
        /// </summary>
        /// <param name="asyncResult">
        /// The <c>IAsyncResult</c> object returned by a call to <c>BeginInvoke</c>.
        /// </param>
        /// <returns>
        /// The object returned by the delegate.
        /// </returns>
        object ISynchronizeInvoke.EndInvoke(IAsyncResult asyncResult)
        {
            return _Form.EndInvoke(asyncResult);
        }

        /// <summary>
        /// This executes a delegate on the thread hosting this object.
        /// </summary>
        /// <param name="method">
        /// The delegate to execute.
        /// </param>
        /// <param name="args">
        /// The arguments to pass to the delegate.
        /// </param>
        /// <returns>
        /// The object returned by the delegate.
        /// </returns>
        object ISynchronizeInvoke.Invoke(Delegate method, object[] args)
        {
            if (Thread.VolatileRead(ref _ThreadId) != GetCurrentThreadId())
            {
                return _Form.Invoke(method, args);
            }
            else
            {
                return method.DynamicInvoke(args);
            }
        }

        /// <summary>
        /// This gets a bool indicating whether the caller must use Invoke.
        /// </summary>
        bool ISynchronizeInvoke.InvokeRequired
        {
            get { return Thread.VolatileRead(ref _ThreadId) != GetCurrentThreadId(); }
        }

        /// <summary>
        /// This starts a message loop on the current thread.
        /// </summary>
        public void Run()
        {
            _Form.Show();
            Application.Run();
        }

        /// <summary>
        /// This starts a message loop on the current thread and shows the specified form.
        /// </summary>
        /// <param name="form">
        /// The Form to display.
        /// </param>
        public void Run(Form form)
        {
            _Form.Show();
            Application.Run(form);
        }

        /// <threadsafety static="true" instance="false" />
        private sealed class HiddenForm : Form
        {
            [DllImport("user32.dll")]
            private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hwndNewParent);

            public HiddenForm()
            {
                this.Load += this.HiddenForm_Load;
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    const int WS_POPUP = unchecked((int)0x80000000);
                    const int WS_EX_TOOLWINDOW = 0x80;

                    CreateParams cp = base.CreateParams;
                    cp.ExStyle = WS_EX_TOOLWINDOW;
                    cp.Style = WS_POPUP;
                    cp.Height = 0;
                    cp.Width = 0;
                    return cp;
                }
            }

            private void HiddenForm_Load(object source, EventArgs e)
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 5)
                {
                    // Make this a message only window if the OS is WinXP or higher.
                    const int HWND_MESSAGE = -1;
                    SetParent(this.Handle, new IntPtr(HWND_MESSAGE));
                }
            }

        } // class

    } // class

} // namespace