#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-11-15
=================
- Test Case Framework ported.
  - Change namespace from Tests to UnitTests.

======================================================================================================================
Update 2013-02-04
=================
- Test Case Framework ported.
  - Ported Test Case Framework from GFA38v3 to NLib30v302.
  - Test Case Framework change MethodInvoker to Action for more generic usage.

======================================================================================================================
Update 2010-01-08
=================
- Test Case Framework update.
  - Replace Activator.CreateInstance to SysLib.Reflection.DynamicAccess.Create method for
    increase speed and performance.

======================================================================================================================
Update 2009-12-27
=================
- Test Case Framework added.
  - Add new TestCase framework for Windows Forms (not UnitTest). The Test Case Framework
    in this case is used for make the test code writing flows can easy to manatainance
    by supported define Precondition subroutine and Post condition subroutine writing
    in function programming style that make each test case can define all condition for
    test case in one place. For Example.

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // create test case manager instance
            TestCaseManager tester = new TestCaseManager();

            // test case 1
            tester.Define<Form1>();
            // test case 2
            tester.Define<Form2>(                    
                (MethodInvoker)delegate()
                {
                    /* Pre conditions */
                    // need to start server before create the Windows Forms instance.
                    Server.Instance.Start();
                },                    
                (MethodInvoker)delegate()
                {
                    /* Post conditions */
                    // stop server after application is close and about to exit
                    Server.Instance.Stop();
                });
            // test case 3
            tester.Define<Form3>();
            ....

            // Select Case to test
            //Cases toTest = Cases.Of<Form1>();
            Cases toTest = Cases.Of<Form2>();
            //Cases toTest = Cases.Of<Form3>();
            ....

            TestCase testCase;
            testCase = tester[toTest];
            // Init Test case to call Pre condition sub routine.
            testCase.Init();

            // The Form variable.
            Form form;
            // Create Form instance
            form = testCase.Create();
            // Run Form
            Application.Run(form);

            // cleanup all test cases by automatically call 
            // post condition subroutine on each test case.
            tester.Cleanup();
        }
    }

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;

#endregion

namespace NLib.UnitTests
{
    #region Cases

    /// <summary>
    /// Cases class. The helper class that provide wrapper to access
    /// TestCase in TestCase Manager via indexer.
    /// </summary>
    public sealed class Cases
    {
        #region Internal Variable

        private Type _objectType = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private Cases() : base() { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="objectType">An object type.</param>
        private Cases(Type objectType) : this() { _objectType = objectType; }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~Cases() { _objectType = null; }

        #endregion

        #region Public Property

        /// <summary>
        /// Get object type.
        /// </summary>
        public Type ObjectType { get { return _objectType; } }

        #endregion

        #region Static Method

        /// <summary>
        /// Get New Case object that associated with specificed object (Form) tyoe T.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>Return new instance of Cases that used with TestCaseManager to
        /// select TestCase in list.</returns>
        public static Cases Of<T>()
        {
            return new Cases(typeof(T));
        }

        #endregion
    }

    #endregion

    #region TestCaseManager

    /// <summary>
    /// Test Case Manager class. Provide functions to manage test cases that
    /// make the code writing flows can easy to manatainance.
    /// </summary>
    public sealed class TestCaseManager
    {
        #region Internal Variable

        private List<TestCase> _testCases = new List<TestCase>();

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor. Initialized the new instance of TestCaseManager.
        /// </summary>
        public TestCaseManager()
            : base()
        {
        }
        /// <summary>
        /// Destructor. Release and automatically cleanup all test cases in list.
        /// </summary>
        ~TestCaseManager()
        {
            Cleanup();
        }

        #endregion

        #region Public Method

        #region Define

        /// <summary>
        /// Define new test case.
        /// </summary>
        /// <typeparam name="T">The object type for test case.</typeparam>
        public void Define<T>()
        {
            TestCase testCase = new TestCase(typeof(T));
            if (!_testCases.Contains(testCase))
            {
                _testCases.Add(testCase);
            }
        }
        /// <summary>
        /// Define new test case. Add new test case and define subroutine that used
        /// for Init process and Cleanup process in test case.
        /// </summary>
        /// <typeparam name="T">The object type for test case.</typeparam>
        /// <param name="preProcess">The init process subroutine.</param>
        /// <param name="postProcess">The cleanup process subroutine.</param>
        public void Define<T>(Action preProcess, Action postProcess)
        {
            TestCase testCase = new TestCase(typeof(T));
            if (!_testCases.Contains(testCase))
            {
                testCase.Define(preProcess, postProcess);
                _testCases.Add(testCase);
            }
        }
        /// <summary>
        /// Define new test case. Add new test case and define subroutine that used
        /// for Cleanup process in test case.
        /// </summary>
        /// <typeparam name="T">The object type for test case.</typeparam>
        /// <param name="postProcess">The cleanup process subroutine.</param>
        public void Define<T>(Action postProcess)
        {
            TestCase testCase = new TestCase(typeof(T));
            if (!_testCases.Contains(testCase))
            {
                testCase.Define(postProcess);
                _testCases.Add(testCase);
            }
        }

        #endregion

        #region Get Case

        /// <summary>
        /// Get the test case.
        /// </summary>
        /// <param name="objectType">The object type for test case.</param>
        /// <returns>Returns test case in list that match object type.</returns>
        public TestCase GetCase(Type objectType)
        {
            TestCase _test = new TestCase(objectType);
            int index = _testCases.IndexOf(_test);
            if (index > -1)
                return _testCases[index];
            else return null;
        }
        /// <summary>
        /// Get the test case.
        /// </summary>
        /// <typeparam name="T">The object type for test case.</typeparam>
        /// <returns>Returns test case in list that match object type.</returns>
        public TestCase GetCase<T>()
        {
            return GetCase(typeof(T));
        }

        #endregion

        #region Indexer Access

        /// <summary>
        /// Get the test case.
        /// </summary>
        /// <param name="objectType">The object type in test case.</param>
        /// <returns>Returns test case in list that match object type.</returns>
        public TestCase this[Type objectType]
        {
            get
            {
                return GetCase(objectType);
            }
        }
        /// <summary>
        /// Get the test case.
        /// </summary>
        /// <param name="cases">The cases object.</param>
        /// <returns>Returns test case in list that match object type in cases objects.</returns>
        public TestCase this[Cases cases]
        {
            get
            {
                return GetCase(cases.ObjectType);
            }
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Cleanup all test cases.
        /// </summary>
        public void Cleanup()
        {
            foreach (TestCase testCase in _testCases)
            {
                testCase.Cleanup(); // cleanup
            }
            _testCases.Clear();
        }

        #endregion

        #endregion
    }

    #endregion

    #region TestCase

    /// <summary>
    /// Test Case class. Provide basic function for test Windows Forms by
    /// define Pre process and Post process in functional programming style.
    /// </summary>
    public sealed class TestCase
    {
        #region Internal Variable

        private Type _objectType = null;
        private object _objectInst = null;
        private Action _preProcess = null;
        private Action _postProcess = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        private TestCase() : base() { }
        /// <summary>
        /// Constructor. Initializes instance without define 
        /// Init(pre process)/Cleanup(post process) subroutine.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        public TestCase(Type objectType)
            : this()
        {
            if (objectType == null)
            {
                throw new ArgumentException(
                    "An parameter 'objectType' cannot be null.");
            }
            if (!objectType.IsSubclassOf(typeof(System.Windows.Forms.Form)) &&
                !objectType.IsSubclassOf(typeof(System.Windows.Window)))
            {
                throw new ArgumentException(
                    "An parameter 'objectType' shold inherited from " +
                    "System.Windows.Forms.Form or System.Windows.Window");
            }
            _objectType = objectType;
        }
        /// <summary>
        /// Constructor. Initializes instance with Cleanup(post process) subroutine.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <param name="postProcess">The Cleanup process subroutine.</param>
        public TestCase(Type objectType, Action postProcess)
            : this()
        {
            Define(postProcess);
        }
        /// <summary>
        /// Constructor. Initializes instance with define 
        /// Init(pre process)/Cleanup(post process) subroutine.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <param name="preProcess">The init process subroutine.</param>
        /// <param name="postProcess">The Cleanup process subroutine.</param>
        public TestCase(Type objectType, Action preProcess, Action postProcess)
            : this()
        {
            Define(preProcess, postProcess);
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~TestCase()
        {
            Cleanup();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>Returns true if object is equals.</returns>
        public override bool Equals(object obj)
        {
            TestCase cmpObj = (obj as TestCase);
            if (null != cmpObj)
            {
                // compare only both is test case objects.
                return this.ObjectType.Equals(cmpObj.ObjectType);
            }
            else return false;
        }
        /// <summary>
        /// GetHashCode.
        /// </summary>
        /// <returns>Returns HashCode.</returns>
        public override int GetHashCode()
        {
            return this.ObjectType.GetHashCode();
        }

        #endregion

        #region Public Methods

        #region Define

        /// <summary>
        /// Define subroutine that used for Init process and Cleanup process
        /// in test case.
        /// </summary>
        /// <param name="preProcess">The init process subroutine.</param>
        /// <param name="postProcess">The cleanup process subroutine.</param>
        public void Define(Action preProcess, Action postProcess)
        {
            _preProcess = preProcess; // assign pre process subroutine
            _postProcess = postProcess; // assign post process subroutine
        }
        /// <summary>
        /// Define subroutine that used for Cleanup process in test case.
        /// </summary>
        /// <param name="postProcess">The cleanup process subroutine.</param>
        public void Define(Action postProcess)
        {
            _postProcess = postProcess; // assign post process subroutine
        }

        #endregion

        #region Init

        /// <summary>
        /// Call initialized subroutine if define.
        /// </summary>
        public void Init()
        {
            if (null != _preProcess)
            {
                _preProcess.Invoke(); // call sub routine
            }
        }

        #endregion

        #region Create Form/Window

        /// <summary>
        /// Create new instance of specificed type normally inherited from
        /// System.Windows.Forms.Form class.
        /// </summary>
        /// <returns>Return new instance of specificed type normally inherited from 
        /// System.Windows.Forms.Form class.</returns>
        public System.Windows.Forms.Form CreateForm()
        {
            if (null == _objectType ||
                !_objectType.IsSubclassOf(typeof(System.Windows.Forms.Form)))
            {
                throw new ArgumentException(
                    "The objectType cannot convert to System.Windows.Forms.Form");
            }

            if (null == _objectInst)
            {
                try
                {
                    _objectInst = NLib.Reflection.DynamicAccess.Create(_objectType);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return _objectInst as System.Windows.Forms.Form;
        }
        /// <summary>
        /// Create new instance of specificed type normally inherited from
        /// System.Windows.Window class.
        /// </summary>
        /// <returns>Return new instance of specificed type normally inherited from 
        /// System.Windows.Window class.</returns>
        public System.Windows.Window CreateWindow()
        {
            if (null == _objectType ||
                !_objectType.IsSubclassOf(typeof(System.Windows.Window)))
            {
                throw new ArgumentException(
                    "The objectType cannot convert to System.Windows.Window");
            }

            if (null == _objectInst)
            {
                try
                {
                    _objectInst = NLib.Reflection.DynamicAccess.Create(_objectType);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return _objectInst as System.Windows.Window;
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Cleanup related objects by call post sub routine and release
        /// all refenrece that refered to this test case.
        /// </summary>
        public void Cleanup()
        {
            #region Invoke Cleanup

            if (null != _postProcess)
            {
                _postProcess.Invoke(); // call sub routine
            }

            #endregion

            #region Release form/window

            if (_objectType != typeof(System.Windows.Forms.Form))
            {
                System.Windows.Forms.Form d =
                    (_objectInst as System.Windows.Forms.Form);

                if (null != d && !d.Disposing && !d.IsDisposed)
                {
                    try { d.Dispose(); }
                    catch { }
                    try { d.Close(); }
                    catch { }
                }
            }
            else if (_objectType != typeof(System.Windows.Window))
            {
                System.Windows.Window d =
                    (_objectInst as System.Windows.Window);

                if (null != d)
                {
                    try { d.Close(); }
                    catch { }
                }
            }

            #endregion

            #region Release variables

            // clean up reference vars.
            _preProcess = null;
            _postProcess = null;
            _objectInst = null;
            _objectType = null;

            #endregion
        }

        #endregion

        #endregion

        #region Public Property

        /// <summary>
        /// Get Object's type on test case.
        /// </summary>
        public Type ObjectType
        {
            get { return _objectType; }
        }

        #endregion
    }

    #endregion
}
