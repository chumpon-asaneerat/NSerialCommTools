#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-07-09
=================
- GarbageCollector changed.
  - Rename to NGC.
  - Chance to static class.
  - Change default MemoryBarrier to 512 MB.
  - All code that used DebugManager is changed to call via Debug extension methods.

======================================================================================================================
Update 2012-12-19
=================
- GarbageCollector changed.
  - The Units class is move into GarbageCollector class.
  - Fixed mispell comment for FreeGC methods.

======================================================================================================================
Update 2010-11-07
=================
- GarbageCollector code ported.
  - Ported Code to GFA40 to NLib project.

======================================================================================================================
Update 2010-08-30
=================
- GarbageCollector code ported.
  - Ported Code to GFA40 and GFA20 project.
  - Merge units class in same file.

======================================================================================================================
Update 2010-01-22
=================
- GarbageCollector code updated.
  - GarbageCollector class change Exception handle related code to used new version of 
    ExceptionManager.

======================================================================================================================
Update 2010-01-17
=================
- GarbageCollector code updated.
  - All code related to debug now used DebugManager instead.

======================================================================================================================
Update 2010-01-16
=================
- GarbageCollector code updated.
  - All code related to write error log will now used ExceptionManager instead. 

======================================================================================================================
Update 2009-12-25
=================
- GarbageCollector code ported code.
  - GarbageCollector class is ported from GFA37 to GFA38
  - GarbageCollector class log code is temporary removed.
  - New Units class added. Used to be common place to keep constant for Unit reference 
    like MB/KB/GB

======================================================================================================================
Update 2009-10-17
=================
- GarbageCollector updated
  - Add internal check last release memory time to prevent call release too fast.

======================================================================================================================
Update 2008-11-26
=================
- GarbageCollector move from GFA.Lib to GFA.Lib.Core.

======================================================================================================================
Update 2008-04-03
=================
- GarbageCollector New Property added.
  - Add new EnableDetailDebug property that used for enable show more debug detail.

======================================================================================================================
Update 2008-03-29
=================
- GarbageCollector New Feature added.
  - FreeGC method add new overloading for release variable memory allocation before set to null.
  - Support Memory Barrier.
  - Add Usage Example.
  - Add OutOfMemory Event to let application handle when Memory used is over than memory barrier.
  - Add TotalMemory property to check current memory allocation size.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.ComponentModel;
using System.Reflection;

#endregion

namespace NLib
{
    #region NGC

    /// <summary>
    /// Helper for Control Garbage Collector
    /// </summary>
    public static class NGC
    {
        #region Units

        /// <summary>
        /// Units constant class
        /// </summary>
        public sealed class Units
        {
            #region For Memory/Disk Size and Transfer rate

            /// <summary>
            /// Byte
            /// </summary>
            public const int Byte = 1;
            /// <summary>
            /// Kilobyte
            /// </summary>
            public const int KB = 1024;
            /// <summary>
            /// Megabyte
            /// </summary>
            public const int MB = 1024 * KB;
            /// <summary>
            /// Gigabyte
            /// </summary>
            public const int GB = 1024 * MB;

            #endregion
        }

        #endregion

        #region Consts

        /// <summary>
        /// Minimum Memory Barrier 512 MB
        /// </summary>
        public const long MinimumBarrier = 512 * Units.MB;

        #endregion

        #region Internal Variable (static)

        private static bool _onRelease = false;
        private static long _memoryBarrier = -1;

        private static bool _enableDetailDebug = false;

        private static DateTime _lastReleaseTime = DateTime.Now;
        private static int _releaseInMillisecond = 250; // do not change this value.

        #endregion

        #region Public Method (static)

        /// <summary>
        /// Force Free Garbage Collector
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// private void button1_Click(object sender, EventArgs e)
        /// {
        ///    ...
        ///    // Call for Get Data From database.
        ///    DataTable table = GetDataTable();
        /// 
        ///     ... do some operation
        /// 
        ///     .. after finished.
        ///     if (table != null)
        ///     {
        ///         table.Dispose(); // Dispose table to free resource.
        ///     }
        ///     table = null;
        /// 
        ///    // Force Garbage Collector to free resource.
        ///    NGC.FreeGC();
        ///    ...
        /// }
        /// </code>
        /// </example>
        public static void FreeGC()
        {
            FreeGC(null);
        }
        /// <summary>
        /// Force Free Garbage Collector
        /// </summary>
        /// <param name="value">object reference to release memory</param>
        /// <example>
        /// <code lang="C#">
        /// private void button1_Click(object sender, EventArgs e)
        /// {
        ///    ...
        ///    // Call for Get Data From database.
        ///    DataTable table = GetDataTable();
        /// 
        ///     ... do some operation
        /// 
        ///     .. after finished.
        ///     if (table != null)
        ///     {
        ///         table.Dispose(); // Dispose table to free resource.
        ///     }
        /// 
        ///    // Force Garbage Collector to free resource.
        ///    NGC.FreeGC(table);
        ///    table = null;
        ///    ...
        /// }
        /// </code>
        /// </example>
        public static void FreeGC(object value)
        {
            if (_onRelease)
                return;

            if (ApplicationManager.Instance.IsExit)
                return;

            _onRelease = true; // mark flag
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                #region Verify is need to release

                bool needToRelease = false;
                if (_memoryBarrier < 0)
                {
                    needToRelease = true;
                }
                else
                {
                    long usedMemory = GC.GetTotalMemory(false);
                    if (usedMemory > _memoryBarrier)
                    {
                        needToRelease = true;
                    }
                    else
                    {
                        string msg = "Allocated memory is not over than barrier no need to Free GC";
                        msg.Info(med);

                        needToRelease = false;
                        // update last release time to current date time.
                        _lastReleaseTime = DateTime.Now;
                    }
                }

                #endregion

                #region Release

                if (value != null)
                {
                    try
                    {
                        GC.SuppressFinalize(value);
                    }
                    catch (Exception exx)
                    {
                        #region Write Exception

                        "FreeGC Detected Exception when SuppressFinalize an object instance".Err(med);
                        exx.Err(med);

                        #endregion
                    }
                }

                #endregion

                #region Check need to release

                if (needToRelease)
                {
                    TimeSpan ts = DateTime.Now - _lastReleaseTime;
                    if (ts.TotalMilliseconds > _releaseInMillisecond)
                    {
                        if (_enableDetailDebug)
                        {
                            "Force GC to Free Unused Memory.".Info(med);                            
                        }

                        // Force version will release memory immediately
                        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

                        // The below line is document as
                        // no garantee thread is terminated
                        // and may cause deadlock.
                        {
                            //GC.WaitForPendingFinalizers();
                        }

                        // Check again.
                        if (_memoryBarrier >= MinimumBarrier) // if memory barrier is set
                        {
                            long allocatedMem = GC.GetTotalMemory(true);
                            if (allocatedMem >= MinimumBarrier)
                            {
                                // memory still over than barrier. Need to do something.
                                if (OnOutOfMemory != null)
                                {
                                    OnOutOfMemory.Call(null, System.EventArgs.Empty);
                                }
                            }
                        }
                        // update last release time to current date time.
                        _lastReleaseTime = DateTime.Now;
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Write Exception

                "FreeGC Detected Exception when Collect memory and WaitForPendingFinalizers.".Err(med);
                ex.Err(med);

                #endregion
            }
            finally
            {
                _onRelease = false;
            }
        }

        #endregion

        #region Public Property (static)

        /// <summary>
        /// Get/Set Memory Barrier (-1) for not check memory barrier.
        /// </summary>
        [Category("GC")]
        [Description("Get/Set Memory Barrier (-1) for not check memory barrier.")]
        public static long MemoryBarrier
        {
            get { return _memoryBarrier; }
            set
            {
                if (value < 0)
                {
                    _memoryBarrier = value;
                }
                if (value < MinimumBarrier)
                {
                    _memoryBarrier = MinimumBarrier;
                }
                else
                {
                    _memoryBarrier = value;
                }
            }
        }
        /// <summary>
        /// Get Total Memory allocation in bytes.
        /// </summary>
        [Category("GC")]
        [Description("Get Total Memory allocation in bytes.")]
        public static long TotalMemory
        {
            get
            {
                // get memory when force release memory
                return GC.GetTotalMemory(true);
            }
        }
        /// <summary>
        /// Get/Set Show Detail Debug.
        /// </summary>
        [Category("GC")]
        [Description("Get/Set Show Detail Debug.")]
        public static bool EnableDetailDebug
        {
            get { return _enableDetailDebug; }
            set { _enableDetailDebug = value; }
        }

        #endregion

        #region Public Events (static)

        /// <summary>
        /// OnOutOfMemory event. Occur when FreeGC Method is force to free memory and the allocate memory still
        /// over than limit memory barrier.
        /// </summary>
        [Category("GC")]
        [Description("OnOutOfMemory event. Occur when FreeGC Method is force to free memory and the allocate memory still over than limit memory barrier.")]
        public static event System.EventHandler OnOutOfMemory;

        #endregion
    }

    #endregion
}
