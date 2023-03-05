//#define ENABLE_STACKTRACE_VERSION

#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-17
=================
- Debug Framework update.
  - DebugManager Remove all method that used StackFrame (or StackTrace).

======================================================================================================================
Update 2013-08-04
=================
- Debug Framework update.
  - DebugManager annd IsEnable property.

======================================================================================================================
Update 2013-07-09
=================
- Debug Framework changed.
  - Redesign all related classes. No longer to keep compatible with the previous versions.
  - DumpCache class is removed and replace with VarCache class in NLib.Reflection namespace.
  - MessageTypes enum is not changed.
  - VariableInfo class is changed to NVariableInfo.
  - Add new classes for keep debug information.
    - NDebugInfo class added (abstract class).
    - NDebugInfo<T> class added (abstract class).
    - NDebugMessage class added. This class for source of debug is string.
    - NDebugException class added. This class for source of debug is exception.
    - NDebugVariable class added. This class for source of debug is NVariableInfo list.
  - In all classes the callingMethod and declarationType that is string is no longer
    supports. So used MethodBase.GetCurrentMethod() to get the calling method instead.
  - NVariableInfo class supports auto format multiple line variable's value (auto detect
    newline character '\n').
  - The NDebug EventHandler and EventArgs is not changed.
  - NDebugger class. The event will raise by delegate extenstion methods for prevent 
    cross-thread problem and decouple from ApplicationManager.
  - DebugManager class is ported and add lock code to prevent access violation when add/remove
    or clear debugger.
    - Ported Info methods (only 2 methods) and remove all DeclaringType paramter.
    - Ported Error methods (only 4 methods) and remove all DeclaringType paramter.
    - Ported Dump methods (only 2 methods) and remove all DeclaringType paramter and
      fixed bug when check the parameter is null or empty.
    - All Dump methods is removed the header line [Variables] so when need to display
      the dump header so the info need to write by caller via Info or Error method before
      call Dump method.

======================================================================================================================
Update 2013-05-13
=================
- Debug Framework changed.
  - Remove IDebugable interface.
  - Remove extenstion methods.
  - All DebugInfo's properties changes to automatic proprties.
  - The Debugger class changed name to NDebugger.
  - The DebuggerManager class changed name to NDebuggerManager.

======================================================================================================================
Update 2012-12-19
=================
- Debug Framework changed.
  - Add IDebugable interface for used with debug extenstion methods.
  - Add extenstion methods for Debug manager.
  - Rearrange parameters for supports auto string format.
  - Add new Info/Error/Dump methods that supports used of StackFrame.

======================================================================================================================
Update 2011-12-15
=================
- Debug Framework ported.
  - Merge ExceptionManager class into DebugManager.

======================================================================================================================
Update 2010-10-20
=================
- Debug Framework ported.
  - Ported code from GFA40 to NLib40.
  - Remove DebugOptions class.
  - Change RuntimeDebugEventHandler and RuntimeDebugEventArgs to DebugEventHandler and 
    DebugEventArgs.
  - Change RuntimeDebugger to Debugger.
  - Decouple with access to DelegateInvoker and used IApplicationManager that assigned 
    in constructor instead.

======================================================================================================================
Update 2010-08-31
=================
- Debug Framework ported.
  - Ported code from GFA38v3 to GFA40.

======================================================================================================================
Update 2010-01-23
=================
- Debug Framework update.
  - DebugManager class add methods for more generic used.
    - Info methods added (3 methods).
    - Error methods added (6 methods).
    - Dump methods added (3 methods).
  - Add delegate and related class for RuntimeDebugger class.
  - DebugInfo class and overload constructors and new properties
    - Constructor now has 3 overload methods.
    - ThreadId - for keep creation thread id.
    - Exception - for keep Exception instance.

======================================================================================================================
Update 2010-01-20
=================
- Debug Framework update.
  - Change DebugManager class for more generic used.
  - Add RuntimeDebugger class.
  - Change DebugOptions class for more generic used.
  - Change DebugInfo class for more generic used.
  - Change VariableInfo class for more generic used.

======================================================================================================================
Update 2010-01-16
=================
- Debug Framework added.
  - Add new DebugManager class to provide common places to runtime debugging.
  - Add new VariableInfo class. Used to keep variable information for dump data.
  - Add new DebugInfo class. Used to keep debug information.
  - Add new DebugOptions class. Used to provide various option to customized how to debug.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace NLib
{
    #region MessageTypes

    /// <summary>
    /// The Message Types enums.
    /// </summary>
    public enum MessageTypes : byte
    {
        /// <summary>
        /// Interesting runtime events.
        /// Expect these to be immediately visible on a console, 
        /// so be conservative and keep to a minimum.
        /// </summary>
        Info = 5,
        /// <summary>
        /// Other runtime errors or unexpected conditions.
        /// Expect these to be immediately visible on a status console.
        /// </summary>
        Error = 7
    }

    #endregion

    #region NVariableInfo
    
    /// <summary>
    /// NVariableInfo class. This class is used for keep pair of variable's name and it's value.
    /// </summary>
    public class NVariableInfo
    {
        #region Constructor
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public NVariableInfo() : base()
        {
            this.Name = string.Empty;
            this.Value = null;
            this.Format = string.Empty;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="value">The variable's value.</param>
        /// <param name="format">The format for variable value.</param>
        public NVariableInfo(string name, object value, string format = "")
            : this()
        {
            this.Name = name;
            this.Value = value;
            this.Format = format;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Equals. see Object.Equals for more information.
        /// </summary>
        /// <param name="obj">see Object.Equals for more information.</param>
        /// <returns>Returns true if object is equal.</returns>
        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;
            return (this.GetHashCode() == obj.GetHashCode());
        }
        /// <summary>
        /// GetHashCode. see Object.Equals for more information.
        /// </summary>
        /// <returns>Returns hash code. see Object.GetHashCode for more information.</returns>
        public override int GetHashCode()
        {
            string fmt = string.Format("{0}", this.Name);
            return fmt.GetHashCode();
        }
        /// <summary>
        /// ToString.
        /// </summary>
        /// <returns>Returns string that represents an object.</returns>
        public override string ToString()
        {
            string val = string.Empty;
            if (string.IsNullOrWhiteSpace(this.Format))
                val = (null != this.Value) ? this.Value.ToString() : "(null)";
            else val = string.Format(this.Format, this.Value);
            string namePart = string.Format("{0} = ", this.Name);

            string valPart = string.Empty;
            string[] vallines = val.Split(new char[] { '\n' }, StringSplitOptions.None);

            if (vallines.Length > 1)
            {
                // more that one line is detected so need to generate blank prefix on next line
                // for formatting output.
                string blankPart = string.Empty;
                for (int i = 0; i < namePart.Length; ++i) blankPart += " "; // append space

                for (int i = 0; i < vallines.Length; ++i)
                {
                    if (i == 0)
                    {
                        // first line not need to append space.
                        valPart = vallines[i];
                    }
                    else
                    {
                        valPart += blankPart + vallines[i];
                    }
                }
            }
            else valPart = val;

            return namePart + valPart;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the variable name.
        /// </summary>
        [Category("Variables")]
        [Description("Gets or sets the variable name.")]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the variable value.
        /// </summary>
        [Category("Variables")]
        [Description("Gets or sets the variable value.")]
        public object Value { get; set; }
        /// <summary>
        /// Gets or sets variable's format.
        /// </summary>
        [Category("Variables")]
        [Description("Gets or sets variable's format.")]
        public string Format { get; set; }

        #endregion

        #region Static methods

        /// <summary>
        /// Create.
        /// </summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="value">The variable's value.</param>
        /// <param name="format">The variable's format.</param>
        /// <returns>Returns new instance of VariableInfo.</returns>
        public static NVariableInfo Create(string name,
            object value, string format = "")
        {
            NVariableInfo inst = new NVariableInfo(name,
                value, format);
            return inst;
        }

        #endregion
    }

    #endregion

    #region NDebugInfo

    /// <summary>
    /// NDebugInfo class. This is base classs for all debug information.
    /// </summary>
    public abstract class NDebugInfo
    {
        #region Internal Variables

        private DateTime _createTime = DateTime.Now;
        private int _threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

        #endregion

        #region Abstract methods

        /// <summary>
        /// Gets debug string.
        /// </summary>
        /// <returns>Returns debug string.</returns>
        protected abstract string GetDebugString();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the debug message create time.
        /// </summary>
        [Category("Debug")]
        [Description("Gets the debug message create time.")]
        public DateTime CreateTime { get { return _createTime; } }
        /// <summary>
        /// Gets create thread id.
        /// </summary>
        [Category("Debug")]
        [Description("Gets create thread id.")]
        public int ThreadId { get { return _threadId; } }
        /// <summary>
        /// Gets or sets the debug message type.
        /// </summary>
        [Category("Debug")]
        [Description("Gets or sets the debug message type.")]
        public MessageTypes DebugType { get; set; }
        /// <summary>
        /// Gets or sets the calling method.
        /// </summary>
        [Category("Debug")]
        [Description("Gets or sets the calling method.")]
        public MethodBase CallingMethod { get; set; }
        /// <summary>
        ///  Gets the the calling method in string.
        /// </summary>
        [Category("Debug")]
        [Description("Gets the the calling method in string.")]
        public string MethodName
        {
            get
            {
                if (null == this.CallingMethod)
                    return string.Empty;
                else return this.CallingMethod.Name;
            }
        }
        /// <summary>
        /// Gets the declare type that is the owner of calling method.
        /// </summary>
        [Category("Debug")]
        [Description("Gets the declare type that is the owner of calling method.")]
        public Type DeclaringType
        {
            get 
            {
                if (null == this.CallingMethod)
                    return null;
                else return this.CallingMethod.DeclaringType;
            }
        }
        /// <summary>
        ///  Gets the declare type that is the owner of calling method in string (include namespace).
        /// </summary>
        [Category("Debug")]
        [Description("Gets the declare type that is the owner of calling method in string.")]
        public string ClassFullName
        {
            get
            {
                if (null == this.DeclaringType)
                    return string.Empty;
                else return this.DeclaringType.FullName;
            }
        }
        /// <summary>
        ///  Gets the declare type that is the owner of calling method in string (not include namespace).
        /// </summary>
        [Category("Debug")]
        [Description("Gets the declare type that is the owner of calling method in string.")]
        public string ClassName
        {
            get
            {
                if (null == this.DeclaringType)
                    return string.Empty;
                else return this.DeclaringType.Name;
            }
        }
        /// <summary>
        /// Gets debug message.
        /// </summary>
        [Category("Debug")]
        [Description("Gets debug message.")]
        public string Message { get { return this.GetDebugString(); } }

        #endregion
    }

    #endregion

    #region NDebugInfo<T>

    /// <summary>
    /// NDebugInfo of T class. This is base classs for all debug information and provide the proeprty
    /// to access the source instance of the debug string.
    /// </summary>
    /// <typeparam name="T">The target instance type.</typeparam>
    public abstract class NDebugInfo<T> : NDebugInfo
    {
        #region Overrides

        /// <summary>
        /// Gets debug string.
        /// </summary>
        /// <returns>Returns debug string.</returns>
        protected override string GetDebugString()
        {
            string result = string.Empty;

            if (null != this.Value)
            {
                try 
                { 
                    result = this.Value.ToString(); 
                }
                catch { }
            }

            return result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the instance value that is source of debug string.
        /// </summary>
        [Category("Debug")]
        [Description("Gets or sets the instance value that is source of debug string.")]
        public T Value { get; set; }

        #endregion
    }

    #endregion

    #region NDebugMessage

    /// <summary>
    /// NDebugMessage class. This class is is used for the source of debug is string.
    /// </summary>
    public class NDebugMessage : NDebugInfo<string>
    {
        #region Static Methods

        /// <summary>
        /// Create.
        /// </summary>
        /// <param name="med">The calling method.</param>
        /// <param name="debugType">The debug type.</param>
        /// <param name="value">The debug source instance.</param>
        /// <returns>Return instance of NDebugMessage.</returns>
        public static NDebugMessage Create(MethodBase med, MessageTypes debugType,
            string value)
        {
            // checks is parameter is assigned.
            if (string.IsNullOrWhiteSpace(value))
                return null;

            NDebugMessage inst = new NDebugMessage();
            inst.DebugType = debugType;
            inst.CallingMethod = med;
            inst.Value = value;
            return inst;
        }

        #endregion
    }

    #endregion

    #region NDebugException

    /// <summary>
    /// NDebugException class. This class is is used for the source of debug is exception.
    /// </summary>
    public class NDebugException : NDebugInfo<Exception>
    {
        #region Overrides

        /// <summary>
        /// Gets debug string.
        /// </summary>
        /// <returns>Returns debug string.</returns>
        protected override string GetDebugString()
        {
            string result = string.Empty;

            if (null != this.Value)
            {
                try
                {
                    result = this.Value.Message;
                }
                catch { }
            }

            return result;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Create.
        /// </summary>
        /// <param name="med">The calling method.</param>
        /// <param name="value">The debug source instance.</param>
        /// <returns>Return instance of NDebugException.</returns>
        public static NDebugException Create(MethodBase med, 
            Exception value)
        {
            // checks is parameter is not null.
            if (null == value)
                return null;

            NDebugException inst = new NDebugException();
            inst.DebugType = MessageTypes.Error;
            inst.CallingMethod = med;
            inst.Value = value;
            return inst;
        }

        #endregion
    }

    #endregion

    #region NDebugVariable

    /// <summary>
    /// NDebugVariable class. This class is is used for the source of debug is NVariableInfo.
    /// </summary>
    public class NDebugVariable : NDebugInfo<List<NVariableInfo>>
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public NDebugVariable() : base()
        {
            this.Value = new List<NVariableInfo>();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets debug string.
        /// </summary>
        /// <returns>Returns debug string.</returns>
        protected override string GetDebugString()
        {
            string result = string.Empty;

            if (null != this.Value && this.Value.Count > 0)
            {
                try
                {
                    NVariableInfo[] values = null;
                    lock (this)
                    {
                        values = this.Value.ToArray();
                    }
                    int iCnt = 0;
                    foreach (NVariableInfo info in values)
                    {
                        result += info.ToString();
                        ++iCnt;
                        if (iCnt < values.Length)
                        {
                            result += Environment.NewLine;
                        }
                    }
                }
                catch { }
            }

            return result;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Create.
        /// </summary>
        /// <param name="med">The calling method.</param>
        /// <param name="debugType">The debug type.</param>
        /// <param name="values">The array of debug source instance.</param>
        /// <returns>Return instance of NDebugVariable.</returns>
        public static NDebugVariable Create(MethodBase med, MessageTypes debugType,
            params NVariableInfo[] values)
        {
            // checks is parameter is not null and the name is assigned.
            if (null == values || values.Length <= 0)
                return null;

            NDebugVariable inst = new NDebugVariable();
            inst.DebugType = debugType;
            inst.CallingMethod = med;

            foreach (NVariableInfo value in values)
            {
                if (null == value || string.IsNullOrWhiteSpace(value.Name))
                    continue;
                int index = inst.Value.IndexOf(value);
                if (index < 0)
                {
                    inst.Value.Add(value); // add
                }
                else inst.Value[index] = value; // override
            }
            return inst;
        }
        /// <summary>
        /// Create.
        /// </summary>
        /// <param name="med">The calling method.</param>
        /// <param name="debugType">The debug type.</param>
        /// <param name="name">The variable's name.</param>
        /// <param name="value">The variable's value.</param>
        /// <param name="format">The format for variable's value.</param>
        /// <returns>Return instance of NDebugVariable.</returns>
        public static NDebugVariable Create(MethodBase med, MessageTypes debugType,
            string name, object value, string format = "")
        {
            NVariableInfo inst = new NVariableInfo(name, value, format);
            // create instance.
            return NDebugVariable.Create(med, debugType, inst);
        }

        #endregion
    }

    #endregion

    #region NDebug EventHandler and EventArgs

    /// <summary>
    /// NDebugEventHandler delegate.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The DebugEventArgs instance.</param>
    public delegate void NDebugEventHandler(object sender, NDebugEventArgs e);
    /// <summary>
    /// NDebugEventArgs class.
    /// </summary>
    public class NDebugEventArgs
    {
        #region Internal Variables

        private NDebugger _debugger = null;
        private NDebugInfo _info = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        private NDebugEventArgs() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="debugger">The debugger instance.</param>
        /// <param name="info">The debug info instance.</param>
        public NDebugEventArgs(NDebugger debugger,
            NDebugInfo info)
            : this()
        {
            _debugger = debugger;
            _info = info;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NDebugEventArgs()
        {
            _debugger = null;
            _info = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets debugger instance.
        /// </summary>
        public NDebugger Debugger { get { return _debugger; } }
        /// <summary>
        /// Gets debug info.
        /// </summary>
        public NDebugInfo Info { get { return _info; } }

        #endregion
    }

    #endregion

    #region NDebugger

    /// <summary>
    /// NDebugger class. 
    /// This class is cannot created by user code.
    /// </summary>
    public sealed class NDebugger
    {
        #region Internal Varaible

        private string _debuggerName;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        private NDebugger() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="debuggerName">The debugger name.</param>
        internal NDebugger(string debuggerName)
            : this()
        {
            _debuggerName = debuggerName;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~NDebugger()
        {
        }

        #endregion

        #region Override

        /// <summary>
        /// Equals. see Object.Equals for more information.
        /// </summary>
        /// <param name="obj">see Object.Equals for more information.</param>
        /// <returns>Returns true if object is equal.</returns>
        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;
            return (this.GetHashCode() == obj.GetHashCode());
        }
        /// <summary>
        /// GetHashCode. see Object.Equals for more information.
        /// </summary>
        /// <returns>Returns hash code. see Object.GetHashCode for more information.</returns>
        public override int GetHashCode()
        {
            string fmt = string.Format("{0}", this._debuggerName);
            return fmt.GetHashCode();
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="info">The debug information.</param>
        public void Debug(NDebugInfo info)
        {
            if (null != OnDebug && null != info)
            {
                NDebugEventArgs evt = new NDebugEventArgs(this, info);
                /* Call by delegate extension method */
                OnDebug.Call(this, evt);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets debugger name.
        /// </summary>
        [Category("Debug")]
        [Description("Gets debugger name.")]
        public string DebuggerName
        {
            get { return _debuggerName; }
        }

        #endregion

        #region Public Events

        /// <summary>
        /// OnDebug event. Occur when the debug information is arrived.
        /// </summary>
        [Category("Debug")]
        [Description("OnDebug event. Occur when the debug information is arrived.")]
        public event NDebugEventHandler OnDebug;

        #endregion
    }

    #endregion

    #region DebugManager

    /// <summary>
    /// Debug Manager class. Provide common places to debugging.
    /// </summary>
    public sealed class DebugManager
    {
        #region Singelton Access

        private static DebugManager _instance = null;
        /// <summary>
        /// Singleton access instance.
        /// </summary>
        public static DebugManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (typeof(DebugManager))
                    {
                        _instance = new DebugManager();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Internal Variable

        private bool _isEnable = true;
        private List<NDebugger> _debuggers = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        private DebugManager()
            : base()
        {
            _debuggers = new List<NDebugger>();
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~DebugManager()
        {
            if (null != _debuggers)
            {
                lock (this)
                {
                    _debuggers.Clear();
                }
            }
            _debuggers = null;
        }

        #endregion

        #region Public Methods

        #region Begin/End

        /// <summary>
        /// Begin Debug.
        /// </summary>
        /// <param name="debuggerName">The debugger's name.</param>
        /// <returns>Returns new instance of RunTime Debugger.</returns>
        public NDebugger BeginDebug(string debuggerName)
        {
            NDebugger inst = new NDebugger(debuggerName);
            if (null == _debuggers)
            {
                lock (this)
                {
                    _debuggers = new List<NDebugger>();
                }
            }
            if (null == _debuggers)
                return null;

            lock (this)
            {
                int index = _debuggers.IndexOf(inst);
                if (index <= -1)
                {
                    _debuggers.Add(inst);
                }
                else inst = _debuggers[index];
            }

            return inst;
        }
        /// <summary>
        /// End Debug.
        /// </summary>
        /// <param name="debuggerName">The debugger's name.</param>
        public void EndDebug(string debuggerName)
        {
            if (null == _debuggers)
                return;

            lock (this)
            {
                // The instance of IApplicationManager is not required here.
                NDebugger test = new NDebugger(debuggerName);

                int index = _debuggers.IndexOf(test);
                if (index >= 0 && index < _debuggers.Count)
                {
                    _debuggers.RemoveAt(index);
                }
            }
        }

        #endregion

        #region Debug methods

        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="info">The debug information.</param>
        public void Debug(NDebugInfo info)
        {
            if (!IsEnable)
                return; // debugger is disable.

            if (null == info)
                return;
            if (null == _debuggers)
            {
                lock (this)
                {
                    _debuggers = new List<NDebugger>();
                }
            }
            NDebugger[] debuggers = null;
            lock (this)
            {
                debuggers = _debuggers.ToArray();
            }
            if (null != debuggers && debuggers.Length > 0)
            {
                foreach (NDebugger debugger in debuggers)
                {
                    debugger.Debug(info);
                }
            }
        }

        #endregion

        #region Info (2 methods)

        /// <summary>
        /// Write Info to all debugger.
        /// </summary>
        /// <param name="callingMethod">
        /// The proper way to set this value is used
        /// System.Reflection.MethodBase.GetCurrentMethod() to read 
        /// the calling method.
        /// </param>
        /// <param name="message">
        /// The info message. This value cannot be null or empty or contains only whitespace.
        /// </param>
        /// <param name="args">The optional args for info message.</param>
        public void Info(MethodBase callingMethod,
            string message, params object[] args)
        {
            if (!IsEnable)
                return; // debugger is disable.

            if (string.IsNullOrWhiteSpace(message))
                return;

            if (null == callingMethod)
                return;

            // Formatting
            string msg = string.Empty;
            try
            {
                if (null == args || args.Length <= 0)
                    msg = message;
                else msg = string.Format(message, args);
            }
            catch { msg = message; }

            // Write
            NDebugMessage inst =
                NDebugMessage.Create(callingMethod, MessageTypes.Info, msg);

            Debug(inst); // write debug info
        }
#if ENABLE_STACKTRACE_VERSION
        /// <summary>
        /// Write Info to all debugger.
        /// </summary>
        /// <param name="message">
        /// The info message. This value cannot be null or empty or contains only whitespace.
        /// </param>
        /// <param name="args">The optional args for info message.</param>
        public void Info(string message, params object[] args)
        {
            if (!IsEnable)
                return; // debugger is disable.

            if (string.IsNullOrWhiteSpace(message))
                return;

            StackFrame sf = new StackFrame(1, false);
            MethodBase callingMethod = sf.GetMethod();

            if (null == callingMethod)
                return;

            // Formatting
            string msg = string.Empty;
            try
            {
                if (null == args || args.Length <= 0)
                    msg = message;
                else msg = string.Format(message, args);
            }
            catch { msg = message; }
            // Write
            NDebugMessage inst =
                NDebugMessage.Create(callingMethod, MessageTypes.Info, msg);

            Debug(inst); // write debug info
        }
#endif
        #endregion

        #region Error (4 methods)

        /// <summary>
        /// Write Error to all debugger.
        /// </summary>
        /// <param name="callingMethod">
        /// The proper way to set this value is used
        /// System.Reflection.MethodBase.GetCurrentMethod() to read 
        /// the calling method.
        /// </param>
        /// <param name="message">
        /// The error message. This value cannot be null or empty or contains only whitespace.
        /// </param>
        /// <param name="args">The optional args for error message.</param>
        public void Error(MethodBase callingMethod,
            string message, params object[] args)
        {
            if (!IsEnable)
                return; // debugger is disable.

            if (string.IsNullOrWhiteSpace(message))
                return;

            if (null == callingMethod)
                return;

            // Formatting
            string msg = string.Empty;
            try
            {
                if (null == args || args.Length <= 0)
                    msg = message;
                else msg = string.Format(message, args);
            }
            catch { msg = message; }

            // Write
            NDebugMessage inst =
                NDebugMessage.Create(callingMethod, MessageTypes.Error, msg);

            Debug(inst); // write debug info
        }
#if ENABLE_STACKTRACE_VERSION
        /// <summary>
        /// Write Error to all debugger.
        /// </summary>
        /// <param name="message">
        /// The error message. This value cannot be null or empty or contains only whitespace.
        /// </param>
        /// <param name="args">The optional args for error message.</param>
        public void Error(string message, params object[] args)
        {
            if (!IsEnable)
                return; // debugger is disable.

            if (string.IsNullOrWhiteSpace(message))
                return;

            StackFrame sf = new StackFrame(1, false);
            MethodBase callingMethod = sf.GetMethod();

            if (null == callingMethod)
                return;

            // Formatting
            string msg = string.Empty;
            try
            {
                if (null == args || args.Length <= 0)
                    msg = message;
                else msg = string.Format(message, args);
            }
            catch { msg = message; }

            // Write
            NDebugMessage inst =
                NDebugMessage.Create(callingMethod, MessageTypes.Error, msg);

            Debug(inst); // write debug info
        }
#endif
        /// <summary>
        /// Write Error to all debugger.
        /// </summary>
        /// <param name="callingMethod">
        /// The proper way to set this value is used
        /// System.Reflection.MethodBase.GetCurrentMethod() to read 
        /// the calling method.
        /// </param>
        /// <param name="ex">The Exception instance. This value cannot be null.</param>
        public void Error(MethodBase callingMethod,
            Exception ex)
        {
            if (!IsEnable)
                return; // debugger is disable.

            if (null == ex)
                return;

            if (null == callingMethod)
                return;

            // Write
            NDebugException inst =
                NDebugException.Create(callingMethod, ex);

            Debug(inst); // write debug info
        }
#if ENABLE_STACKTRACE_VERSION
        /// <summary>
        /// Write Error to all debugger.
        /// </summary>
        /// <param name="ex">The Exception instance. This value cannot be null.</param>
        public void Error(Exception ex)
        {
            if (!IsEnable)
                return; // debugger is disable.

            if (null == ex)
                return;

            StackFrame sf = new StackFrame(1, false);
            MethodBase callingMethod = sf.GetMethod();

            if (null == callingMethod)
                return;

            // Write
            NDebugException inst =
                NDebugException.Create(callingMethod, ex);

            Debug(inst); // write debug info
        }
#endif
        #endregion

        #region Dump (2 methods)

        /// <summary>
        /// Write Dump Variables to all debugger.
        /// </summary>
        /// <param name="callingMethod">
        /// The proper way to set this value is used
        /// System.Reflection.MethodBase.GetCurrentMethod() to read 
        /// the calling method.
        /// </param>
        /// <param name="vars">The list of variable info.</param>
        public void Dump(MethodBase callingMethod,
            params NVariableInfo[] vars)
        {
            if (!IsEnable)
                return; // debugger is disable.

            if (null == vars || vars.Length <= 0)
                return;

            if (null == callingMethod)
                return;

            // Write
            NDebugVariable inst =
                NDebugVariable.Create(callingMethod, MessageTypes.Info, vars);

            Debug(inst); // write debug info
        }
#if ENABLE_STACKTRACE_VERSION
        /// <summary>
        /// Write Dump to all debugger.
        /// </summary>
        /// <param name="vars">The list of variable info.</param>
        public void Dump(params NVariableInfo[] vars)
        {
            if (!IsEnable)
                return; // debugger is disable.

            if (null == vars || vars.Length <= 0)
                return;

            StackFrame sf = new StackFrame(1, false);
            MethodBase callingMethod = sf.GetMethod();

            if (null == callingMethod)
                return;

            // Write
            NDebugVariable inst =
                NDebugVariable.Create(callingMethod, MessageTypes.Info, vars);

            Debug(inst); // write debug info
        }
#endif
        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets is debugger is enable or  disable.
        /// </summary>
        [Browsable(false)]
        public bool IsEnable { get { return _isEnable; } set { _isEnable = value; } }

        #endregion
    }

    #endregion
}
