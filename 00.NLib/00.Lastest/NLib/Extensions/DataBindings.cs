#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-12-12
=================
- NLib Controls Data Binding Extension Methods.
  - Add Windows Forms's Controls Data Binding Extension Methods class.
  - Add WPF's Controls Data Binding Extension Methods class but still not implements.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

#endregion

namespace NLib
{
    #region Extra Using
    
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    #endregion

    /// <summary>
    /// The Windows  Forms's Control Data Binding Extenstion Methods.
    /// </summary>
    public static class WindowsFormsControlDataBindingExtenstionMethods
    {
        #region Bind
        
        /// <summary>
        /// Bind datasource to control.
        /// </summary>
        /// <typeparam name="T">The control type.</typeparam>
        /// <param name="ctrl">The control instance.</param>
        /// <param name="dataSource">The datasource instance.</param>
        /// <param name="dataMemberName">The Data Source Member Name to binding.</param>
        /// <param name="propertyName">The Control's Property Name to binding.</param>
        public static void Bind<T>(this T ctrl, object dataSource,
            string dataMemberName, string propertyName = "Text")
            where T : Control
        {
            ctrl.DataBindings.Clear();
            Binding binding = new Binding(propertyName, dataSource, dataMemberName);
            ctrl.DataBindings.Add(binding);
        }

        #endregion
    }
}

namespace NLib
{
    /// <summary>
    /// The WPF's Control Data Binding Extenstion Methods.
    /// </summary>
    public static class WpfControlDataBindingExtenstionMethods
    {

    }
}