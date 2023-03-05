#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-08-05
=================
- NLib Common Interfaces updated.
  - The interface IApplicationController is removed unused base interfaces.

======================================================================================================================
Update 2013-01-01
=================
- NLib Common Interfaces updated.
  - The interface IApplicationController is split to new file IApplicationController.cs.
  - All log related methods is removed. Used Log extenstions instead.

======================================================================================================================
Update 2012-12-21
=================
- NLib Application Manager updated.
  - Add new Extenstion methods for Logs (Supports MethodBase parameter).
  - Add property IsLogEnable for enable or disable log.

======================================================================================================================
Update 2012-12-19
=================
- NLib Application Manager updated.
  - The interface IDelegateInvoker and IApplicationController is merged into
    ApplicationManager.cs file.
  - The Wait static method is changed to non static method.
- NLib Application Manager Extension Methods added.
  - Supports Sleep/Wait/DoEvents for multithread purpose.
  - Supports Invoke for event raiser.
  - Supports FreeGC for memory management.
  - Supports Logs methods.

======================================================================================================================
Update 2012-01-04
=================
- NLib Application Manager updated.
  - Add supports application single instance.
  - Add Share Manager.
  - Add exitCode optional parameter in Shutdown methods.

======================================================================================================================
Update 2011-12-15
=================
- NLib Application Manager. Redesign based on GFA40.
  - The Current version is still not supports all GFA40 functionals.
  - Add Static method Wait.
  - Redesign Application Controller function in IoC styles for used with multiple
    type of application like Windows Forms, WPF and Windows sercices.
  - Add new properties to work with application environments with on each company
    and each products will supports windows security to grant permission for users
    group to access the files and folders.
  - Supports custom Delegate Invoker depend on each type of applications in the
    Application's Controller.
  - Supports custom Application.DoEvents for Windows Forms and WPF.

======================================================================================================================
Update 2011-01-31
=================
- ApplicationManager class update.
  - Fixed Update Message Methods. The tag parameter is never used in last version.

======================================================================================================================
Update 2010-09-07
=================
- ApplicationManager class update.
  - Add Update Message Methods.
  - Add ApplicationMessageEventHandler and ApplicationMessageEventArgs.

======================================================================================================================
Update 2010-09-01
=================
- ApplicationManager class redesign.
  - Used code based on GFA37.
  - ShareManager and related classes added.
  - ArgsUtils class added.

======================================================================================================================
Update 2010-02-02
=================
- ApplicationManager class updated.
  - Update code to create interanl object like system tray and timer on same thread as
    MainForm thread.

======================================================================================================================
Update 2010-02-06
=================
- ApplicationManager class updated.
  - Add Start method.
  - Add Shutdown method.
  - Add IsVisible<T> method.
  - Update code to reise event by synchronized context to internal form
    if not assigned and used the MainForm if assigned.
- IApplicationService interface updated.
  - Add Init Method.
  - Add Release Method.

======================================================================================================================
Update 2010-02-03
=================
- Application Framework - Application Manager added.
  - Add new implementation of ApplicationManager class.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;

#endregion

namespace NLib
{
    #region IApplicationController

    /// <summary>
    /// The IApplicationManager.
    /// </summary>
    public interface IApplicationController
    {
        /// <summary>
        /// Shutdown. No auto kill process.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        void Shutdown(int exitCode = 0);
        /// <summary>
        /// Shutdown with auto kill process.
        /// </summary>
        /// <param name="autokill">True for autokill</param>
        /// <param name="exitCode">The exit code.</param>
        void Shutdown(bool autokill, int exitCode = 0);
        /// <summary>
        /// Shutdown application manager.
        /// </summary>
        /// <param name="autokill">True for autokill</param>
        /// <param name="autoKillInMs">The time to force process auto 
        /// kill in millisecond. if this parameter is less than 100 ms. 
        /// so no auto kill process running.</param>
        /// <param name="exitCode">The exit code.</param>
        void Shutdown(bool autokill, uint autoKillInMs, int exitCode = 0);
        /// <summary>
        /// Checks is the application is in exit state.
        /// </summary>
        bool IsExit { get; }
        /// <summary>
        /// Checks is application has more instance than one.
        /// </summary>
        bool HasMoreInstance { get; }
    }

    #endregion
}
