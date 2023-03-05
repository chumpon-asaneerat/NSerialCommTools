#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-17
=================
- Controls Library updated.
  - PropertyGridEx change log code (used MethodBase).

======================================================================================================================
Update 2014-10-18
=================
- Controls Library updated.
  - Ported PropertyGridEx from GFA v4.0. to NLib v4.0.4.1.

======================================================================================================================
Update 2010-02-01
=================
- Controls Library updated.
  - Ported PropertyGridEx control from GFA37 to GFA38v3. (Supports only pro and 
    standard version).
  - PropertyGridEx control updated log and error code by used new debug/exception framework. 

======================================================================================================================
Update 2007-12-01
=================
- PropertyGridEx control is exteneded PropertyGrid that provides abilities to custom the
  preference of PropertyGrid and the detach class like dynamic create property at runtime
  or change how to display property's name or add custom property editor etc.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Serialization;

#endregion

namespace NLib.Controls.Design
{
    #region Attributes

    #region Browsable Label Style Attribute

    /// <summary>
    /// Browsable Label Style Attribute
    /// </summary>
    public class BrowsableLabelStyleAttribute : Attribute
    {
        #region Internal Variable

        private LabelStyle eLabelStyle = LabelStyle.Normal;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="LabelStyle">Label's Style</param>
        public BrowsableLabelStyleAttribute(LabelStyle LabelStyle)
        {
            eLabelStyle = LabelStyle;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Label's Style
        /// </summary>
        public LabelStyle LabelStyle
        {
            get
            {
                return eLabelStyle;
            }
            set
            {
                eLabelStyle = value;
            }
        }

        #endregion
    }

    #endregion

    #region Delegate Attribute

    /// <summary>
    /// Delegate Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal class DelegateAttribute : Attribute
    {
        #region Protected

        /// <summary>
        /// On Click delegate
        /// </summary>
        private UICustomEventHandler m_MethodDelegate;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="MethodDelegate">On Click delegate</param>
        public DelegateAttribute(UICustomEventHandler MethodDelegate)
        {
            this.m_MethodDelegate = MethodDelegate;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get On Click Delegate
        /// </summary>
        public UICustomEventHandler GetMethod
        {
            get
            {
                return this.m_MethodDelegate;
            }
        }

        #endregion
    }

    #endregion

    #region FileDialogFilter Attribute

    /// <summary>
    /// FileDialogFilter Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FileDialogFilterAttribute : Attribute
    {
        #region Internal Variable

        private string _filter;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filter">File filter</param>
        public FileDialogFilterAttribute(string filter)
        {
            this._filter = filter;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get File Filter
        /// </summary>
        public string Filter
        {
            get
            {
                return this._filter;
            }
        }

        #endregion
    }

    #endregion

    #region SaveFile Attribute

    /// <summary>
    /// SaveFile Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SaveFileAttribute : Attribute
    {
    }

    #endregion

    #region UI Listbox Datasource Attribute

    /// <summary>
    /// UI Listbox Datasource Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal class UIListboxDatasourceAttribute : Attribute
    {
        #region Internal Variable

        private object oDataSource;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Datasource">The datasource</param>
        public UIListboxDatasourceAttribute(ref object Datasource)
        {
            oDataSource = Datasource;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get DataSource
        /// </summary>
        public object Value
        {
            get
            {
                return oDataSource;
            }
        }

        #endregion
    }

    #endregion

    #region UI Listbox Value Member Attribute

    /// <summary>
    /// UI Listbox Value Member Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal class UIListboxValueMemberAttribute : Attribute
    {
        #region Internal Variable

        private string sValueMember;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ValueMember">The Value Member</param>
        public UIListboxValueMemberAttribute(string ValueMember)
        {
            sValueMember = ValueMember;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Value Member
        /// </summary>
        public string Value
        {
            get
            {
                return sValueMember;
            }
            set
            {
                sValueMember = value;
            }
        }

        #endregion
    }

    #endregion

    #region UI Listbox Display Member Attribute

    /// <summary>
    /// UI Listbox Display Member Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal class UIListboxDisplayMemberAttribute : Attribute
    {
        #region Internal Variable

        private string sDisplayMember;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="DisplayMember">The Display Member</param>
        public UIListboxDisplayMemberAttribute(string DisplayMember)
        {
            sDisplayMember = DisplayMember;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Display Member
        /// </summary>
        public string Value
        {
            get
            {
                return sDisplayMember;
            }
            set
            {
                sDisplayMember = value;
            }
        }

        #endregion
    }

    #endregion

    #region UI Listbox Is DropDown Resizable Attribute

    /// <summary>
    /// UI Listbox Is DropDown Resizable Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal class UIListboxIsDropDownResizableAttribute : Attribute
    {
    }

    #endregion

    #region PropertyEx Choices Attribute List Attribute

    /// <summary>
    /// PropertyEx Choices Attribute List Attribute
    /// </summary>
    internal class PropertyExChoicesAttributeListAttribute : Attribute
    {
        #region Internal Variable

        private ArrayList oList = new ArrayList();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="List"></param>
        public PropertyExChoicesAttributeListAttribute(string[] List)
        {
            oList.AddRange(List);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="List"></param>
        public PropertyExChoicesAttributeListAttribute(ArrayList List)
        {
            oList.AddRange(List);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="List"></param>
        public PropertyExChoicesAttributeListAttribute(ListBox.ObjectCollection List)
        {
            oList.AddRange(List);
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get Item
        /// </summary>
        public ArrayList Item
        {
            get
            {
                return this.oList;
            }
        }
        /// <summary>
        /// Get Values
        /// </summary>
        public TypeConverter.StandardValuesCollection Values
        {
            get
            {
                return new TypeConverter.StandardValuesCollection(this.oList);
            }
        }

        #endregion
    }

    #endregion

    #endregion

    #region Type Converters

    #region Browsable TypeConverter

    /// <summary>
    /// Browsable TypeConverter
    /// </summary>
    internal class BrowsableTypeConverter : ExpandableObjectConverter
    {
        #region Overrides

        /// <summary>
        /// CanConvertTo
        /// </summary>
        /// <param name="context">ITypeDescriptorContext context</param>
        /// <param name="destinationType">destination type to convert</param>
        /// <returns>true if can convert to destination type</returns>
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType)
        {
            return true;
        }
        /// <summary>
        /// ConvertTo
        /// </summary>
        /// <param name="context">ITypeDescriptorContext context</param>
        /// <param name="culture">CultureInfo instance</param>
        /// <param name="value">value to convert</param>
        /// <param name="destinationType">destination type to convert</param>
        /// <returns>object instance that converted</returns>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture, object value, System.Type destinationType)
        {
            BrowsableLabelStyleAttribute attribute1 = (BrowsableLabelStyleAttribute)context.PropertyDescriptor.Attributes[typeof(BrowsableLabelStyleAttribute)];
            if (attribute1 != null)
            {
                switch (attribute1.LabelStyle)
                {
                    case LabelStyle.Normal:
                        {
                            return base.ConvertTo(context, culture, RuntimeHelpers.GetObjectValue(value), destinationType);
                        }
                    case LabelStyle.TypeName:
                        {
                            return ("(" + value.GetType().Name + ")");
                        }
                    case LabelStyle.Ellipsis:
                        {
                            return "(...)";
                        }
                }
            }
            return base.ConvertTo(context, culture, RuntimeHelpers.GetObjectValue(value), destinationType);
        }

        #endregion
    }

    #endregion

    #region PropertyEx Choices TypeConverter

    /// <summary>
    /// PropertyEx Choices TypeConverter
    /// </summary>
    internal class PropertyExChoicesTypeConverter : TypeConverter
    {
        #region Internal Variable

        private PropertyExChoicesAttributeListAttribute oChoices = null;

        #endregion

        #region Overrides

        /// <summary>
        /// GetStandardValuesSupported
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            bool returnValue;
            PropertyExChoicesAttributeListAttribute Choices = (PropertyExChoicesAttributeListAttribute)context.PropertyDescriptor.Attributes[typeof(PropertyExChoicesAttributeListAttribute)];
            if (oChoices != null)
            {
                return true;
            }
            if (Choices != null)
            {
                oChoices = Choices;
                returnValue = true;
            }
            else
            {
                returnValue = false;
            }
            return returnValue;
        }
        /// <summary>
        /// GetStandardValuesExclusive
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context)
        {
            bool returnValue;
            PropertyExChoicesAttributeListAttribute Choices = (PropertyExChoicesAttributeListAttribute)context.PropertyDescriptor.Attributes[typeof(PropertyExChoicesAttributeListAttribute)];
            if (oChoices != null)
            {
                return true;
            }
            if (Choices != null)
            {
                oChoices = Choices;
                returnValue = true;
            }
            else
            {
                returnValue = false;
            }
            return returnValue;
        }
        /// <summary>
        /// GetStandardValues
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
        {
            PropertyExChoicesAttributeListAttribute Choices = (PropertyExChoicesAttributeListAttribute)context.PropertyDescriptor.Attributes[typeof(PropertyExChoicesAttributeListAttribute)];
            if (oChoices != null)
            {
                return oChoices.Values;
            }
            return base.GetStandardValues(context);
        }

        #endregion
    }

    #endregion

    #endregion

    #region UI Type Editors

    #region UI CustomEvent Editor

    /// <summary>
    /// UI CustomEvent Editor
    /// </summary>
    internal class UICustomEventEditor : System.Drawing.Design.UITypeEditor
    {
        #region Delegate and Event

        /// <summary>
        /// Delegate variable for OnClick
        /// </summary>
        private UICustomEventHandler m_MethodDelegate;
        /// <summary>
        /// Sender object
        /// </summary>
        private PropertyExDescriptor m_sender;

        #endregion

        #region Overrides

        /// <summary>
        /// GetEditStyle
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                if (!context.PropertyDescriptor.IsReadOnly)
                {
                    return UITypeEditorEditStyle.Modal;
                }
            }
            return UITypeEditorEditStyle.None;
        }
        /// <summary>
        /// EditValue
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [RefreshProperties(RefreshProperties.All)]
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            if (context == null || provider == null || context.Instance == null)
            {
                return base.EditValue(provider, value);
            }
            if (m_MethodDelegate == null)
            {
                DelegateAttribute attr = (DelegateAttribute)context.PropertyDescriptor.Attributes[typeof(DelegateAttribute)];
                m_MethodDelegate = attr.GetMethod;
            }
            if (m_sender == null)
            {
                m_sender = context.PropertyDescriptor as PropertyExDescriptor;
            }
            /*
            if (m_sender != null)
                return m_MethodDelegate.Invoke(m_sender.Property, null);
            else return m_MethodDelegate.Invoke(null, null);
            */
            return m_MethodDelegate.Invoke(m_sender, null);
        }

        #endregion
    }

    #endregion

    #region UI Filename Editor

    /// <summary>
    /// UI Filename Editor
    /// </summary>
    internal class UIFilenameEditor : System.Drawing.Design.UITypeEditor
    {
        #region Override

        /// <summary>
        /// GetEditStyle
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                if (!context.PropertyDescriptor.IsReadOnly)
                {
                    return UITypeEditorEditStyle.Modal;
                }
            }
            return UITypeEditorEditStyle.None;
        }
        /// <summary>
        /// EditValue
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [RefreshProperties(RefreshProperties.All)]
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            if (context == null || provider == null || context.Instance == null)
            {
                return base.EditValue(provider, value);
            }

            FileDialog fileDlg;
            if (context.PropertyDescriptor.Attributes[typeof(SaveFileAttribute)] == null)
            {
                fileDlg = new OpenFileDialog();
            }
            else
            {
                fileDlg = new SaveFileDialog();
            }
            fileDlg.Title = "Select " + context.PropertyDescriptor.DisplayName;
            fileDlg.FileName = (string)value;

            FileDialogFilterAttribute filterAtt = (FileDialogFilterAttribute)context.PropertyDescriptor.Attributes[typeof(FileDialogFilterAttribute)];
            if (filterAtt != null)
            {
                fileDlg.Filter = filterAtt.Filter;
            }
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                value = fileDlg.FileName;
            }
            fileDlg.Dispose();
            return value;
        }

        #endregion
    }

    #endregion

    #region UI Listbox Editor

    /// <summary>
    /// UI Listbox Editor
    /// </summary>
    internal class UIListboxEditor : UITypeEditor
    {
        #region Internal Variable

        private bool bIsDropDownResizable = false;
        private ListBox oList = new ListBox();
        private object oSelectedValue = null;
        private IWindowsFormsEditorService oEditorService;

        #endregion

        #region Private Method

        private void SelectedItem(object sender, EventArgs e)
        {
            if (oEditorService != null)
            {
                if (oList.SelectedValue != null)
                {
                    oSelectedValue = oList.SelectedValue;
                }
                oEditorService.CloseDropDown();
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// GetEditStyle
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                UIListboxIsDropDownResizableAttribute attribute = (UIListboxIsDropDownResizableAttribute)context.PropertyDescriptor.Attributes[typeof(UIListboxIsDropDownResizableAttribute)];
                if (attribute != null)
                {
                    bIsDropDownResizable = true;
                }
                return UITypeEditorEditStyle.DropDown;
            }
            return UITypeEditorEditStyle.None;
        }
        /// <summary>
        /// Get Is DropDownResizable
        /// </summary>
        public override bool IsDropDownResizable
        {
            get
            {
                return bIsDropDownResizable;
            }
        }
        /// <summary>
        /// EditValue
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [RefreshProperties(RefreshProperties.All)]
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            if (context == null || provider == null || context.Instance == null)
            {
                return base.EditValue(provider, value);
            }

            oEditorService = (System.Windows.Forms.Design.IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (oEditorService != null)
            {

                // Get the Back reference to the PropertyEx
                PropertyExDescriptor oDescriptor = (PropertyExDescriptor)context.PropertyDescriptor;
                PropertyEx cp = oDescriptor.Property;

                // Declare attributes
                UIListboxDatasourceAttribute datasource;
                UIListboxValueMemberAttribute valuemember;
                UIListboxDisplayMemberAttribute displaymember;

                // Get attributes
                datasource = (UIListboxDatasourceAttribute)context.PropertyDescriptor.Attributes[typeof(UIListboxDatasourceAttribute)];
                valuemember = (UIListboxValueMemberAttribute)context.PropertyDescriptor.Attributes[typeof(UIListboxValueMemberAttribute)];
                displaymember = (UIListboxDisplayMemberAttribute)context.PropertyDescriptor.Attributes[typeof(UIListboxDisplayMemberAttribute)];

                oList.BorderStyle = BorderStyle.None;
                oList.IntegralHeight = true;

                if (datasource != null)
                {
                    oList.DataSource = datasource.Value;
                }

                if (displaymember != null)
                {
                    oList.DisplayMember = displaymember.Value;
                }

                if (valuemember != null)
                {
                    oList.ValueMember = valuemember.Value;
                }

                if (value != null)
                {
                    if (value.GetType().Name == "String")
                    {
                        oList.Text = (string)value;
                    }
                    else
                    {
                        oList.SelectedItem = value;
                    }
                }


                oList.SelectedIndexChanged += new System.EventHandler(this.SelectedItem);

                oEditorService.DropDownControl(oList);
                if (oList.SelectedIndices.Count == 1)
                {
                    cp.SelectedItem = oList.SelectedItem;
                    cp.SelectedValue = oSelectedValue;
                    value = oList.Text;
                }
                oEditorService.CloseDropDown();
            }
            else
            {
                return base.EditValue(provider, value);
            }

            return value;

        }

        #endregion
    }

    #endregion

    #endregion
}

namespace NLib.Controls
{
    #region Delegate

    /// <summary>
    /// UI Custom Event Click Delegate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public delegate object UICustomEventHandler(object sender, EventArgs e);

    #endregion

    #region Enums

    #region Label Style

    /// <summary>
    /// Label Style
    /// </summary>
    public enum LabelStyle
    {
        /// <summary>
        /// Normal
        /// </summary>
        Normal,
        /// <summary>
        /// TypeName
        /// </summary>
        TypeName,
        /// <summary>
        /// Ellipsis
        /// </summary>
        Ellipsis
    }

    #endregion

    #region File Dialog Type

    /// <summary>
    /// File Dialog Type
    /// </summary>
    public enum FileDialogType
    {
        /// <summary>
        /// Open File Dialog
        /// </summary>
        OpenFileDialog,
        /// <summary>
        /// Save File Dialog
        /// </summary>
        SaveFileDialog
    }

    #endregion

    #endregion

    #region Custom Color Scheme

    /// <summary>
    /// Custom Color Scheme
    /// </summary>
    internal class CustomColorScheme : ProfessionalColorTable
    {
        #region Overrides

        /// <summary>
        /// Get ButtonCheckedGradientBegin <see cref="ProfessionalColorTable.ButtonCheckedGradientBegin"/>
        /// </summary>
        public override Color ButtonCheckedGradientBegin
        {
            get
            {
                return Color.FromArgb(0xC1, 210, 0xEE);
            }
        }
        /// <summary>
        /// Get ButtonCheckedGradientEnd <see cref="ProfessionalColorTable.ButtonCheckedGradientEnd"/>
        /// </summary>
        public override Color ButtonCheckedGradientEnd
        {
            get
            {
                return Color.FromArgb(0xC1, 210, 0xEE);
            }
        }
        /// <summary>
        /// Get ButtonCheckedGradientMiddle <see cref="ProfessionalColorTable.ButtonCheckedGradientMiddle"/>
        /// </summary>
        public override Color ButtonCheckedGradientMiddle
        {
            get
            {
                return Color.FromArgb(0xC1, 210, 0xEE);
            }
        }
        /// <summary>
        /// Get ButtonPressedBorder <see cref="ProfessionalColorTable.ButtonPressedBorder"/>
        /// </summary>
        public override Color ButtonPressedBorder
        {
            get
            {
                return Color.FromArgb(0x31, 0x6A, 0xC5);
            }
        }
        /// <summary>
        /// Get ButtonPressedGradientBegin <see cref="ProfessionalColorTable.ButtonPressedGradientBegin"/>
        /// </summary>
        public override Color ButtonPressedGradientBegin
        {
            get
            {
                return Color.FromArgb(0x98, 0xB5, 0xE2);
            }
        }
        /// <summary>
        /// Get ButtonPressedGradientEnd <see cref="ProfessionalColorTable.ButtonPressedGradientEnd"/>
        /// </summary>
        public override Color ButtonPressedGradientEnd
        {
            get
            {
                return Color.FromArgb(0x98, 0xB5, 0xE2);
            }
        }
        /// <summary>
        /// Get ButtonPressedGradientMiddle <see cref="ProfessionalColorTable.ButtonPressedGradientMiddle"/>
        /// </summary>
        public override Color ButtonPressedGradientMiddle
        {
            get
            {
                return Color.FromArgb(0x98, 0xB5, 0xE2);
            }
        }
        /// <summary>
        /// Get ButtonSelectedBorder <see cref="ProfessionalColorTable.ButtonSelectedBorder"/>
        /// </summary>
        public override Color ButtonSelectedBorder
        {
            get
            {
                return base.ButtonSelectedBorder;
            }
        }
        /// <summary>
        /// Get ButtonSelectedGradientBegin <see cref="ProfessionalColorTable.ButtonSelectedGradientBegin"/>
        /// </summary>
        public override Color ButtonSelectedGradientBegin
        {
            get
            {
                return Color.FromArgb(0xC1, 210, 0xEE);
            }
        }
        /// <summary>
        /// Get ButtonSelectedGradientEnd <see cref="ProfessionalColorTable.ButtonSelectedGradientEnd"/>
        /// </summary>
        public override Color ButtonSelectedGradientEnd
        {
            get
            {
                return Color.FromArgb(0xC1, 210, 0xEE);
            }
        }
        /// <summary>
        /// Get ButtonSelectedGradientMiddle <see cref="ProfessionalColorTable.ButtonSelectedGradientMiddle"/>
        /// </summary>
        public override Color ButtonSelectedGradientMiddle
        {
            get
            {
                return Color.FromArgb(0xC1, 210, 0xEE);
            }
        }
        /// <summary>
        /// Get CheckBackground <see cref="ProfessionalColorTable.CheckBackground"/>
        /// </summary>
        public override Color CheckBackground
        {
            get
            {
                return Color.FromArgb(0xE1, 230, 0xE8);
            }
        }
        /// <summary>
        /// Get CheckPressedBackground <see cref="ProfessionalColorTable.CheckPressedBackground"/>
        /// </summary>
        public override Color CheckPressedBackground
        {
            get
            {
                return Color.FromArgb(0x31, 0x6A, 0xC5);
            }
        }
        /// <summary>
        /// Get CheckSelectedBackground <see cref="ProfessionalColorTable.CheckSelectedBackground"/>
        /// </summary>
        public override Color CheckSelectedBackground
        {
            get
            {
                return Color.FromArgb(0x31, 0x6A, 0xC5);
            }
        }
        /// <summary>
        /// Get GripDark <see cref="ProfessionalColorTable.GripDark"/>
        /// </summary>
        public override Color GripDark
        {
            get
            {
                return Color.FromArgb(0xC1, 190, 0xB3);
            }
        }
        /// <summary>
        /// Get GripLight <see cref="ProfessionalColorTable.GripLight"/>
        /// </summary>
        public override Color GripLight
        {
            get
            {
                return Color.FromArgb(0xFF, 0xFF, 0xFF);
            }
        }
        /// <summary>
        /// Get ImageMarginGradientBegin <see cref="ProfessionalColorTable.ImageMarginGradientBegin"/>
        /// </summary>
        public override Color ImageMarginGradientBegin
        {
            get
            {
                return Color.FromArgb(251, 250, 247);
            }
        }
        /// <summary>
        /// Get ImageMarginGradientEnd <see cref="ProfessionalColorTable.ImageMarginGradientEnd"/>
        /// </summary>
        public override Color ImageMarginGradientEnd
        {
            get
            {
                return Color.FromArgb(0xBD, 0xBD, 0xA3);
            }
        }
        /// <summary>
        /// Get ImageMarginGradientMiddle <see cref="ProfessionalColorTable.ImageMarginGradientMiddle"/>
        /// </summary>
        public override Color ImageMarginGradientMiddle
        {
            get
            {
                return Color.FromArgb(0xEC, 0xE7, 0xE0);
            }
        }
        /// <summary>
        /// Get ImageMarginRevealedGradientBegin <see cref="ProfessionalColorTable.ImageMarginRevealedGradientBegin"/>
        /// </summary>
        public override Color ImageMarginRevealedGradientBegin
        {
            get
            {
                return Color.FromArgb(0xF7, 0xF6, 0xEF);
            }
        }
        /// <summary>
        /// Get ImageMarginRevealedGradientEnd <see cref="ProfessionalColorTable.ImageMarginRevealedGradientEnd"/>
        /// </summary>
        public override Color ImageMarginRevealedGradientEnd
        {
            get
            {
                return Color.FromArgb(230, 0xE3, 210);
            }
        }
        /// <summary>
        /// Get ImageMarginRevealedGradientMiddle <see cref="ProfessionalColorTable.ImageMarginRevealedGradientMiddle"/>
        /// </summary>
        public override Color ImageMarginRevealedGradientMiddle
        {
            get
            {
                return Color.FromArgb(0xF2, 240, 0xE4);
            }
        }
        /// <summary>
        /// Get MenuBorder <see cref="ProfessionalColorTable.MenuBorder"/>
        /// </summary>
        public override Color MenuBorder
        {
            get
            {
                return Color.FromArgb(0x8A, 0x86, 0x7A);
            }
        }
        /// <summary>
        /// Get MenuItemBorder <see cref="ProfessionalColorTable.MenuItemBorder"/>
        /// </summary>
        public override Color MenuItemBorder
        {
            get
            {
                return Color.FromArgb(0x31, 0x6A, 0xC5);
            }
        }
        /// <summary>
        /// Get MenuItemPressedGradientBegin <see cref="ProfessionalColorTable.MenuItemPressedGradientBegin"/>
        /// </summary>
        public override Color MenuItemPressedGradientBegin
        {
            get
            {
                return base.MenuItemPressedGradientBegin;
            }
        }
        /// <summary>
        /// Get MenuItemPressedGradientEnd <see cref="ProfessionalColorTable.MenuItemPressedGradientEnd"/>
        /// </summary>
        public override Color MenuItemPressedGradientEnd
        {
            get
            {
                return base.MenuItemPressedGradientEnd;
            }
        }
        /// <summary>
        /// Get MenuItemPressedGradientMiddle <see cref="ProfessionalColorTable.MenuItemPressedGradientMiddle"/>
        /// </summary>
        public override Color MenuItemPressedGradientMiddle
        {
            get
            {
                return base.MenuItemPressedGradientMiddle;
            }
        }
        /// <summary>
        /// Get MenuItemSelected <see cref="ProfessionalColorTable.MenuItemSelected"/>
        /// </summary>
        public override Color MenuItemSelected
        {
            get
            {
                return Color.FromArgb(0xC1, 210, 0xEE);
            }
        }
        /// <summary>
        /// Get MenuItemSelectedGradientBegin <see cref="ProfessionalColorTable.MenuItemSelectedGradientBegin"/>
        /// </summary>
        public override Color MenuItemSelectedGradientBegin
        {
            get
            {
                return Color.FromArgb(0xC1, 210, 0xEE);
            }
        }
        /// <summary>
        /// Get MenuItemSelectedGradientEnd <see cref="ProfessionalColorTable.MenuItemSelectedGradientEnd"/>
        /// </summary>
        public override Color MenuItemSelectedGradientEnd
        {
            get
            {
                return Color.FromArgb(0xC1, 210, 0xEE);
            }
        }
        /// <summary>
        /// Get MenuStripGradientBegin <see cref="ProfessionalColorTable.MenuStripGradientBegin"/>
        /// </summary>
        public override Color MenuStripGradientBegin
        {
            get
            {
                return Color.FromArgb(0xE5, 0xE5, 0xD7);
            }
        }
        /// <summary>
        /// Get MenuStripGradientEnd <see cref="ProfessionalColorTable.MenuStripGradientEnd"/>
        /// </summary>
        public override Color MenuStripGradientEnd
        {
            get
            {
                return Color.FromArgb(0xF4, 0xF2, 0xE8);
            }
        }
        /// <summary>
        /// Get OverflowButtonGradientBegin <see cref="ProfessionalColorTable.OverflowButtonGradientBegin"/>
        /// </summary>
        public override Color OverflowButtonGradientBegin
        {
            get
            {
                return Color.FromArgb(0xF3, 0xF2, 240);
            }
        }
        /// <summary>
        /// Get OverflowButtonGradientEnd <see cref="ProfessionalColorTable.OverflowButtonGradientEnd"/>
        /// </summary>
        public override Color OverflowButtonGradientEnd
        {
            get
            {
                return Color.FromArgb(0x92, 0x92, 0x76);
            }
        }
        /// <summary>
        /// Get OverflowButtonGradientMiddle <see cref="ProfessionalColorTable.OverflowButtonGradientMiddle"/>
        /// </summary>
        public override Color OverflowButtonGradientMiddle
        {
            get
            {
                return Color.FromArgb(0xE2, 0xE1, 0xDB);
            }
        }
        /// <summary>
        /// Get RaftingContainerGradientBegin <see cref="ProfessionalColorTable.RaftingContainerGradientBegin"/>
        /// </summary>
        public override Color RaftingContainerGradientBegin
        {
            get
            {
                return Color.FromArgb(0xE5, 0xE5, 0xD7);
            }
        }
        /// <summary>
        /// Get RaftingContainerGradientEnd <see cref="ProfessionalColorTable.RaftingContainerGradientEnd"/>
        /// </summary>
        public override Color RaftingContainerGradientEnd
        {
            get
            {
                return Color.FromArgb(0xF4, 0xF2, 0xE8);
            }
        }
        /// <summary>
        /// Get SeparatorDark <see cref="ProfessionalColorTable.SeparatorDark"/>
        /// </summary>
        public override Color SeparatorDark
        {
            get
            {
                return Color.FromArgb(0xC5, 0xC2, 0xB8);
            }
        }
        /// <summary>
        /// Get SeparatorLight <see cref="ProfessionalColorTable.SeparatorLight"/>
        /// </summary>
        public override Color SeparatorLight
        {
            get
            {
                return Color.FromArgb(0xFF, 0xFF, 0xFF);
            }
        }
        /// <summary>
        /// Get ToolStripBorder <see cref="ProfessionalColorTable.ToolStripBorder"/>
        /// </summary>
        public override Color ToolStripBorder
        {
            get
            {
                return Color.FromArgb(0xA3, 0xA3, 0x7C);
            }
        }
        /// <summary>
        /// Get ToolStripDropDownBackground <see cref="ProfessionalColorTable.ToolStripDropDownBackground"/>
        /// </summary>
        public override Color ToolStripDropDownBackground
        {
            get
            {
                return Color.FromArgb(0xFC, 0xFC, 0xF9);
            }
        }
        /// <summary>
        /// Get ToolStripGradientBegin <see cref="ProfessionalColorTable.ToolStripGradientBegin"/>
        /// </summary>
        public override Color ToolStripGradientBegin
        {
            get
            {
                return Color.FromArgb(0xF7, 0xF6, 0xEF);
            }
        }
        /// <summary>
        /// Get ToolStripGradientEnd <see cref="ProfessionalColorTable.ToolStripGradientEnd"/>
        /// </summary>
        public override Color ToolStripGradientEnd
        {
            get
            {
                return Color.FromArgb(192, 192, 168);
            }
        }
        /// <summary>
        /// Get ToolStripGradientMiddle <see cref="ProfessionalColorTable.ToolStripGradientMiddle"/>
        /// </summary>
        public override Color ToolStripGradientMiddle
        {
            get
            {
                return Color.FromArgb(0xF2, 240, 0xE4);
            }
        }
        /// <summary>
        /// Get ToolStripPanelGradientBegin <see cref="ProfessionalColorTable.ToolStripPanelGradientBegin"/>
        /// </summary>
        public override System.Drawing.Color ToolStripPanelGradientBegin
        {
            get
            {
                return Color.FromArgb(0xE5, 0xE5, 0xD7);
            }
        }
        /// <summary>
        /// Get ToolStripPanelGradientEnd <see cref="ProfessionalColorTable.ToolStripPanelGradientEnd"/>
        /// </summary>
        public override System.Drawing.Color ToolStripPanelGradientEnd
        {
            get
            {
                return Color.FromArgb(0xF4, 0xF2, 0xE8);
            }
        }

        #endregion
    }

    #endregion

    #region PropertyEx Choices

    /// <summary>
    /// PropertyEx Choices
    /// </summary>
    [Serializable()]
    public class PropertyExChoices : ArrayList
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="array"></param>
        /// <param name="IsSorted"></param>
        public PropertyExChoices(ArrayList array, bool IsSorted)
        {
            this.AddRange(array);
            if (IsSorted)
            {
                this.Sort();
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="array"></param>
        public PropertyExChoices(ArrayList array)
        {
            this.AddRange(array);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="array"></param>
        /// <param name="IsSorted"></param>
        public PropertyExChoices(string[] array, bool IsSorted)
        {
            this.AddRange(array);
            if (IsSorted)
            {
                this.Sort();
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="array"></param>
        public PropertyExChoices(string[] array)
        {
            this.AddRange(array);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="array"></param>
        /// <param name="IsSorted"></param>
        public PropertyExChoices(int[] array, bool IsSorted)
        {
            this.AddRange(array);
            if (IsSorted)
            {
                this.Sort();
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="array"></param>
        public PropertyExChoices(int[] array)
        {
            this.AddRange(array);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="array"></param>
        /// <param name="IsSorted"></param>
        public PropertyExChoices(double[] array, bool IsSorted)
        {
            this.AddRange(array);
            if (IsSorted)
            {
                this.Sort();
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="array"></param>
        public PropertyExChoices(double[] array)
        {
            this.AddRange(array);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="array"></param>
        /// <param name="IsSorted"></param>
        public PropertyExChoices(object[] array, bool IsSorted)
        {
            this.AddRange(array);
            if (IsSorted)
            {
                this.Sort();
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="array"></param>
        public PropertyExChoices(object[] array)
        {
            this.AddRange(array);
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get Item List
        /// </summary>
        public ArrayList Items
        {
            get { return this; }
        }

        #endregion
    }

    #endregion

    #region PropertyEx Collection Set

    /// <summary>
    /// PropertyEx Collection Set
    /// </summary>
    public class PropertyExCollectionSet : System.Collections.CollectionBase
    {
        #region List Access

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual int Add(PropertyExCollection value)
        {
            return base.List.Add(value);
        }
        /// <summary>
        /// Add
        /// </summary>
        /// <returns></returns>
        public virtual int Add()
        {
            return base.List.Add(new PropertyExCollection());
        }
        /// <summary>
        /// Indexer Access
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual PropertyExCollection this[int index]
        {
            get
            {
                return ((PropertyExCollection)base.List[index]);
            }
            set
            {
                base.List[index] = value;
            }
        }
        /// <summary>
        /// ToArray
        /// </summary>
        /// <returns></returns>
        public virtual PropertyExCollection[] ToArray()
        {
            return (PropertyExCollection[])InnerList.ToArray(typeof(PropertyExCollection));
        }

        #endregion
    }

    #endregion

    #region PropertyEx Collection

    /// <summary>
    /// PropertyEx Collection
    /// </summary>
    [Serializable()]
    public class PropertyExCollection : System.Collections.CollectionBase, ICustomTypeDescriptor
    {
        #region List Access

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual int Add(PropertyEx value)
        {
            return base.List.Add(value);
        }
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="objValue"></param>
        /// <param name="boolIsReadOnly"></param>
        /// <param name="strCategory"></param>
        /// <param name="strDescription"></param>
        /// <param name="boolVisible"></param>
        /// <returns></returns>
        public virtual int Add(string strName, object objValue, bool boolIsReadOnly, string strCategory, string strDescription, bool boolVisible)
        {
            return base.List.Add(new PropertyEx(strName, objValue, boolIsReadOnly, strCategory, strDescription, boolVisible));
        }
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="objRef"></param>
        /// <param name="strProp"></param>
        /// <param name="boolIsReadOnly"></param>
        /// <param name="strCategory"></param>
        /// <param name="strDescription"></param>
        /// <param name="boolVisible"></param>
        /// <returns></returns>
        public virtual int Add(string strName, ref object objRef, string strProp, bool boolIsReadOnly, string strCategory, string strDescription, bool boolVisible)
        {
            return base.List.Add(new PropertyEx(strName, ref objRef, strProp, boolIsReadOnly, strCategory, strDescription, boolVisible));
        }
        /// <summary>
        /// Get/Set PropertyEx via indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual PropertyEx this[int index]
        {
            get
            {
                return ((PropertyEx)base.List[index]);
            }
            set
            {
                base.List[index] = value;
            }
        }
        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="Name"></param>
        public virtual void Remove(string Name)
        {
            PropertyEx CustomProp;
            foreach (PropertyEx tempLoopVar_CustomProp in base.List)
            {
                CustomProp = tempLoopVar_CustomProp;
                if (CustomProp.Name == Name)
                {
                    base.List.Remove(CustomProp);
                    return;
                }
            }
        }

        #endregion

        #region ICustomTypeDescriptor Implements

        /// <summary>
        /// GetAttributes
        /// </summary>
        /// <returns></returns>
        public System.ComponentModel.AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }
        /// <summary>
        /// GetClassName
        /// </summary>
        /// <returns></returns>
        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }
        /// <summary>
        /// GetComponentName
        /// </summary>
        /// <returns></returns>
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }
        /// <summary>
        /// GetConverter
        /// </summary>
        /// <returns></returns>
        public System.ComponentModel.TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }
        /// <summary>
        /// GetDefaultEvent
        /// </summary>
        /// <returns></returns>
        public System.ComponentModel.EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }
        /// <summary>
        /// GetDefaultProperty
        /// </summary>
        /// <returns></returns>
        public System.ComponentModel.PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }
        /// <summary>
        /// GetEditor
        /// </summary>
        /// <param name="editorBaseType"></param>
        /// <returns></returns>
        public object GetEditor(System.Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }
        /// <summary>
        /// GetEvents
        /// </summary>
        /// <returns></returns>
        public System.ComponentModel.EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }
        /// <summary>
        /// GetEvents
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public System.ComponentModel.EventDescriptorCollection GetEvents(System.Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }
        /// <summary>
        /// GetProperties
        /// </summary>
        /// <returns></returns>
        public System.ComponentModel.PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }
        /// <summary>
        /// GetProperties
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public System.ComponentModel.PropertyDescriptorCollection GetProperties(System.Attribute[] attributes)
        {
            PropertyDescriptorCollection Properties = new PropertyDescriptorCollection(null);
            PropertyEx CustomProp;
            foreach (PropertyEx tempLoopVar_CustomProp in base.List)
            {
                CustomProp = tempLoopVar_CustomProp;
                if (CustomProp.Visible)
                {
                    ArrayList attrs = new ArrayList();

                    // Expandable Object Converter
                    if (CustomProp.IsBrowsable)
                    {
                        attrs.Add(new TypeConverterAttribute(typeof(Design.BrowsableTypeConverter)));
                    }

                    // The Filename Editor
                    if (CustomProp.UseFileNameEditor == true)
                    {
                        attrs.Add(new EditorAttribute(typeof(Design.UIFilenameEditor), typeof(UITypeEditor)));
                    }

                    // Custom Choices Type Converter
                    if (CustomProp.Choices != null)
                    {
                        attrs.Add(new TypeConverterAttribute(typeof(Design.PropertyExChoicesTypeConverter)));
                    }

                    // Password Property
                    if (CustomProp.IsPassword)
                    {
                        attrs.Add(new PasswordPropertyTextAttribute(true));
                    }

                    // Parenthesize Property
                    if (CustomProp.Parenthesize)
                    {
                        attrs.Add(new ParenthesizePropertyNameAttribute(true));
                    }

                    // Datasource
                    if (CustomProp.Datasource != null)
                    {
                        attrs.Add(new EditorAttribute(typeof(Design.UIListboxEditor), typeof(UITypeEditor)));
                    }

                    // Custom Editor
                    if (CustomProp.CustomEditor != null)
                    {
                        attrs.Add(new EditorAttribute(CustomProp.CustomEditor.GetType(), typeof(UITypeEditor)));
                    }

                    // Custom Type Converter
                    if (CustomProp.CustomTypeConverter != null)
                    {
                        attrs.Add(new TypeConverterAttribute(CustomProp.CustomTypeConverter.GetType()));
                    }

                    // Is Percentage
                    if (CustomProp.IsPercentage)
                    {
                        attrs.Add(new TypeConverterAttribute(typeof(OpacityConverter)));
                    }

                    // 3-dots button event delegate
                    if (CustomProp.OnClick != null)
                    {
                        attrs.Add(new EditorAttribute(typeof(Design.UICustomEventEditor), typeof(UITypeEditor)));
                    }

                    // Default value attribute
                    if (CustomProp.DefaultValue != null)
                    {
                        attrs.Add(new DefaultValueAttribute(CustomProp.Type, CustomProp.Value.ToString()));
                    }
                    else
                    {
                        // Default type attribute
                        if (CustomProp.DefaultType != null)
                        {
                            attrs.Add(new DefaultValueAttribute(CustomProp.DefaultType, null));
                        }
                    }

                    // Extra Attributes
                    if (CustomProp.Attributes != null)
                    {
                        attrs.AddRange(CustomProp.Attributes);
                    }

                    // Add my own attributes
                    Attribute[] attrArray = (System.Attribute[])attrs.ToArray(typeof(Attribute));
                    Properties.Add(new PropertyExDescriptor(CustomProp, attrArray));
                }
            }
            return Properties;
        }
        /// <summary>
        /// GetPropertyOwner
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd)
        {
            return this;
        }

        #endregion

        #region Serialize & Deserialize related methods

        /// <summary>
        /// SaveXml
        /// </summary>
        /// <param name="filename"></param>
        public void SaveXml(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PropertyExCollection));
            FileStream writer = new FileStream(filename, FileMode.Create);
            try
            {
                serializer.Serialize(writer, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }
            writer.Close();
        }
        /// <summary>
        /// LoadXml
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool LoadXml(string filename)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PropertyExCollection));
                FileStream reader = new FileStream(filename, FileMode.Open);

                PropertyExCollection cpc = (PropertyExCollection)serializer.Deserialize(reader);
                foreach (PropertyEx customprop in cpc)
                {
                    customprop.RebuildAttributes();
                    this.Add(customprop);
                }
                cpc = null;
                reader.Close();
                return true;

            }
            catch (Exception)
            {
                return false;
            }

        }
        /// <summary>
        /// SaveBinary
        /// </summary>
        /// <param name="filename"></param>
        public void SaveBinary(string filename)
        {
            Stream stream = File.Create(filename);
            BinaryFormatter serializer = new BinaryFormatter();
            try
            {
                serializer.Serialize(stream, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }
            stream.Close();
        }
        /// <summary>
        /// LoadBinary
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool LoadBinary(string filename)
        {
            try
            {
                Stream stream = File.Open(filename, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                if (stream.Length > 0)
                {
                    PropertyExCollection cpc = (PropertyExCollection)formatter.Deserialize(stream);
                    foreach (PropertyEx customprop in cpc)
                    {
                        customprop.RebuildAttributes();
                        this.Add(customprop);
                    }
                    cpc = null;
                    stream.Close();
                    return true;
                }
                else
                {
                    stream.Close();
                    return false;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }

    #endregion

    #region PropertyEx Descriptor

    /// <summary>
    /// PropertyEx Descriptor
    /// </summary>
    public class PropertyExDescriptor : PropertyDescriptor
    {
        private PropertyEx oCustomProperty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="myProperty"></param>
        /// <param name="attrs"></param>
        public PropertyExDescriptor(PropertyEx myProperty, Attribute[] attrs)
            : base(myProperty.Name, attrs)
        {
            if (myProperty == null)
            {
                oCustomProperty = null;
            }
            else
            {

                oCustomProperty = myProperty;
            }
        }
        /// <summary>
        /// CanResetValue
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool CanResetValue(object component)
        {
            if ((oCustomProperty.DefaultValue != null) || (oCustomProperty.DefaultType != null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Get Component Type
        /// </summary>
        public override System.Type ComponentType
        {
            get
            {
                return this.GetType();
            }
        }
        /// <summary>
        /// GetValue
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override object GetValue(object component)
        {
            return oCustomProperty.Value;
        }
        /// <summary>
        /// Get Is ReadOnly
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return oCustomProperty.IsReadOnly;
            }
        }
        /// <summary>
        /// Get Property Type
        /// </summary>
        public override System.Type PropertyType
        {
            get
            {
                return oCustomProperty.Type;
            }
        }
        /// <summary>
        /// ResetValue
        /// </summary>
        /// <param name="component"></param>
        public override void ResetValue(object component)
        {
            oCustomProperty.Value = oCustomProperty.DefaultValue;
            this.OnValueChanged(component, EventArgs.Empty);
        }
        /// <summary>
        /// SetValue
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        public override void SetValue(object component, object value)
        {
            oCustomProperty.Value = value;
            this.OnValueChanged(component, EventArgs.Empty);
        }
        /// <summary>
        /// ShouldSerializeValue
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool ShouldSerializeValue(object component)
        {
            object oValue = oCustomProperty.Value;
            if ((oCustomProperty.DefaultValue != null) && (oValue != null))
            {
                return !oValue.Equals(oCustomProperty.DefaultValue);
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Get Description
        /// </summary>
        public override string Description
        {
            get
            {
                return oCustomProperty.Description;
            }
        }
        /// <summary>
        /// Get Category
        /// </summary>
        public override string Category
        {
            get
            {
                return oCustomProperty.Category;
            }
        }
        /// <summary>
        /// Get DisplayName
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return oCustomProperty.Name;
            }
        }
        /// <summary>
        /// Get IsBrowsable
        /// </summary>
        public override bool IsBrowsable
        {
            get
            {
                return oCustomProperty.IsBrowsable;
            }
        }
        /// <summary>
        /// Get Property
        /// </summary>
        public PropertyEx Property
        {
            get
            {
                return oCustomProperty;
            }
        }
    }

    #endregion

    #region PropertyEx

    /// <summary>
    /// PropertyEx
    /// </summary>
    [Serializable(), XmlRootAttribute("PropertyEx")]
    public class PropertyEx
    {
        #region Internal Variable

        // Common properties
        private string sName = "";
        private object oValue = null;
        private bool bIsReadOnly = false;
        private bool bVisible = true;
        private string sDescription = "";
        private string sCategory = "";
        private bool bIsPassword = false;
        private bool bIsPercentage = false;
        private bool bParenthesize = false;

        // Filename editor properties
        private string sFilter = null;
        private FileDialogType eDialogType = FileDialogType.OpenFileDialog;
        private bool bUseFileNameEditor = false;

        // Custom choices properties
        private PropertyExChoices oChoices = null;

        // Browsable properties
        private bool bIsBrowsable = false;
        private LabelStyle eBrowsablePropertyLabel = LabelStyle.Ellipsis;

        // Dynamic properties
        private bool bRef = false;
        private object oRef = null;
        private string sProp = "";

        // Databinding properties
        private object oDatasource = null;
        private string sDisplayMember = null;
        private string sValueMember = null;
        private object oSelectedValue = null;
        private object oSelectedItem = null;
        private bool bIsDropdownResizable = false;

        /// <summary>
        /// 3-dots button event handler
        /// </summary>
        private UICustomEventHandler MethodDelegate;

        // Extended Attributes
        [NonSerialized()]
        private AttributeCollection oCustomAttributes = null;
        private object oTag = null;
        private object oDefaultValue = null;
        private Type oDefaultType = null;

        // Custom Editor and Custom Type Converter
        [NonSerialized()]
        private UITypeEditor oCustomEditor = null;
        [NonSerialized()]
        private TypeConverter oCustomTypeConverter = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public PropertyEx()
        {
            sName = "New Property";
            oValue = new string(' ', 0);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="objValue"></param>
        /// <param name="boolIsReadOnly"></param>
        /// <param name="strCategory"></param>
        /// <param name="strDescription"></param>
        /// <param name="boolVisible"></param>
        public PropertyEx(string strName, object objValue, bool boolIsReadOnly, string strCategory, string strDescription, bool boolVisible)
        {
            sName = strName;
            oValue = objValue;
            bIsReadOnly = boolIsReadOnly;
            sDescription = strDescription;
            sCategory = strCategory;
            bVisible = boolVisible;
            if (oValue != null)
            {
                oDefaultValue = oValue;
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="objRef"></param>
        /// <param name="strProp"></param>
        /// <param name="boolIsReadOnly"></param>
        /// <param name="strCategory"></param>
        /// <param name="strDescription"></param>
        /// <param name="boolVisible"></param>
        public PropertyEx(string strName, ref object objRef, string strProp, bool boolIsReadOnly, string strCategory, string strDescription, bool boolVisible)
        {
            sName = strName;
            bIsReadOnly = boolIsReadOnly;
            sDescription = strDescription;
            sCategory = strCategory;
            bVisible = boolVisible;
            bRef = true;
            oRef = objRef;
            sProp = strProp;
            if (Value != null)
            {
                oDefaultValue = Value;
            }
        }

        #endregion

        #region Private methods

        private void BuildAttributes_FilenameEditor()
        {
            ArrayList attrs = new ArrayList();
            Design.FileDialogFilterAttribute FilterAttribute = new Design.FileDialogFilterAttribute(sFilter);
            Design.SaveFileAttribute SaveDialogAttribute = new Design.SaveFileAttribute();
            Attribute[] attrArray;
            attrs.Add(FilterAttribute);
            if (eDialogType == FileDialogType.SaveFileDialog)
            {
                attrs.Add(SaveDialogAttribute);
            }
            attrArray = (System.Attribute[])attrs.ToArray(typeof(Attribute));
            oCustomAttributes = new AttributeCollection(attrArray);
        }

        private void BuildAttributes_CustomChoices()
        {
            if (oChoices != null)
            {
                Design.PropertyExChoicesAttributeListAttribute list = new Design.PropertyExChoicesAttributeListAttribute(oChoices.Items);
                ArrayList attrs = new ArrayList();
                Attribute[] attrArray;
                attrs.Add(list);
                attrArray = (System.Attribute[])attrs.ToArray(typeof(Attribute));
                oCustomAttributes = new AttributeCollection(attrArray);
            }
        }

        private void BuildAttributes_ListboxEditor()
        {
            if (oDatasource != null)
            {
                Design.UIListboxDatasourceAttribute ds = new Design.UIListboxDatasourceAttribute(ref oDatasource);
                Design.UIListboxValueMemberAttribute vm = new Design.UIListboxValueMemberAttribute(sValueMember);
                Design.UIListboxDisplayMemberAttribute dm = new Design.UIListboxDisplayMemberAttribute(sDisplayMember);
                Design.UIListboxIsDropDownResizableAttribute ddr = null;
                ArrayList attrs = new ArrayList();
                attrs.Add(ds);
                attrs.Add(vm);
                attrs.Add(dm);
                if (bIsDropdownResizable)
                {
                    ddr = new Design.UIListboxIsDropDownResizableAttribute();
                    attrs.Add(ddr);
                }
                Attribute[] attrArray;
                attrArray = (System.Attribute[])attrs.ToArray(typeof(Attribute));
                oCustomAttributes = new AttributeCollection(attrArray);
            }
        }

        private void BuildAttributes_BrowsableProperty()
        {
            Design.BrowsableLabelStyleAttribute style = new Design.BrowsableLabelStyleAttribute(eBrowsablePropertyLabel);
            oCustomAttributes = new AttributeCollection(new Attribute[] { style });
        }

        private void BuildAttributes_CustomEventProperty()
        {
            Design.DelegateAttribute attr = new Design.DelegateAttribute(MethodDelegate);
            oCustomAttributes = new AttributeCollection(new Attribute[] { attr });
        }
        /// <summary>
        /// Get/Set DataColumn
        /// </summary>
        private object DataColumn
        {
            get
            {
                DataRow oRow = (System.Data.DataRow)oRef;
                if (oRow.RowState != DataRowState.Deleted)
                {
                    if (oDatasource == null)
                    {
                        return oRow[sProp];
                    }
                    else
                    {
                        DataTable oLookupTable = oDatasource as DataTable;
                        if (oLookupTable != null)
                        {
                            return oLookupTable.Select(sValueMember + "=" + oRow[sProp])[0][sDisplayMember];
                        }
                        else
                        {
                            MethodBase med = MethodBase.GetCurrentMethod();
                            string msg =
                                "Bind of DataRow with a DataSource that is not a DataTable is not possible";
                            msg.Err(med);
                            return null;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                DataRow oRow = (System.Data.DataRow)oRef;
                if (oRow.RowState != DataRowState.Deleted)
                {
                    if (oDatasource == null)
                    {
                        oRow[sProp] = value;
                    }
                    else
                    {
                        DataTable oLookupTable = oDatasource as DataTable;
                        if (oLookupTable != null)
                        {
                            if (oLookupTable.Columns[sDisplayMember].DataType.Equals(System.Type.GetType("System.String")))
                            {

                                oRow[sProp] = oLookupTable.Select(oLookupTable.Columns[sDisplayMember].ColumnName + " = \'" + value + "\'")[0][sValueMember];
                            }
                            else
                            {
                                oRow[sProp] = oLookupTable.Select(oLookupTable.Columns[sDisplayMember].ColumnName + " = " + value)[0][sValueMember];
                            }
                        }
                        else
                        {
                            MethodBase med = MethodBase.GetCurrentMethod();
                            string msg = "Bind of DataRow with a DataSource that is not a DataTable is impossible";
                            msg.Err(med);
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// RebuildAttributes
        /// </summary>
        public void RebuildAttributes()
        {
            if (bUseFileNameEditor)
            {
                BuildAttributes_FilenameEditor();
            }
            else if (oChoices != null)
            {
                BuildAttributes_CustomChoices();
            }
            else if (oDatasource != null)
            {
                BuildAttributes_ListboxEditor();
            }
            else if (bIsBrowsable)
            {
                BuildAttributes_BrowsableProperty();
            }
        }

        #endregion

        #region Public property

        /// <summary>
        /// Get/Set Display Name of the PropertyEx.
        /// </summary>
        [Category("Appearance"), DisplayName("Name"), DescriptionAttribute("Display Name of the PropertyEx."), ParenthesizePropertyName(true), XmlElementAttribute("Name")]
        public string Name
        {
            get
            {
                return sName;
            }
            set
            {
                sName = value;
            }
        }
        /// <summary>
        /// Get/Set read only attribute of the PropertyEx.
        /// </summary>
        [Category("Appearance"), DisplayName("ReadOnly"), DescriptionAttribute("Set read only attribute of the PropertyEx."), XmlElementAttribute("ReadOnly")]
        public bool IsReadOnly
        {
            get
            {
                return bIsReadOnly;
            }
            set
            {
                bIsReadOnly = value;
            }
        }
        /// <summary>
        /// Get/Set visibility attribute of the PropertyEx.
        /// </summary>
        [Category("Appearance"), DescriptionAttribute("Set visibility attribute of the PropertyEx.")]
        public bool Visible
        {
            get
            {
                return bVisible;
            }
            set
            {
                bVisible = value;
            }
        }
        /// <summary>
        /// Get/Set Value that Represent the Value of the PropertyEx.
        /// </summary>
        [Category("Appearance"), DescriptionAttribute("Represent the Value of the PropertyEx.")]
        public object Value
        {
            get
            {
                if (bRef)
                {
                    if (oRef.GetType() == typeof(DataRow) || oRef.GetType().IsSubclassOf(typeof(DataRow)))
                        return this.DataColumn;
                    else
                    {
                        return Microsoft.VisualBasic.Interaction.CallByName(oRef, sProp,
                            Microsoft.VisualBasic.CallType.Get, null);
                        /*
                        PropertyInfo prop = oRef.GetType().GetProperty(sProp, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
                        if (prop != null) return prop.GetValue(oRef, Type.EmptyTypes);
                        */
                        /*
                        try
                        {
                            if (oRef == null)
                                return null;
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(oRef);                            
                            if (props != null && props.Count > 0 && props[sProp] != null)
                            {
                                return props[sProp].GetValue(oRef);
                            }
                            else return null;
                        }
                        catch (Exception ex)
                        {
                            Logs.LogManager.Error("Error detected ", "ProperyEx");
                            Logs.LogManager.Error("  Get Property : " + sProp, "ProperyEx");
                            Logs.LogManager.Error(ex, "ProperyEx");
                            return null;
                        }
                        */
                    }
                }
                else
                {
                    return oValue;
                }
            }
            set
            {
                if (bRef)
                {
                    if (oRef.GetType() == typeof(DataRow) || oRef.GetType().IsSubclassOf(typeof(DataRow)))
                        this.DataColumn = value;
                    else
                    {
                        Microsoft.VisualBasic.Interaction.CallByName(oRef, sProp,
                            Microsoft.VisualBasic.CallType.Set, value);
                        /*
                        PropertyInfo prop = oRef.GetType().GetProperty(sProp, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
                        if (prop != null) prop.SetValue(oRef, value, Type.EmptyTypes);
                        */
                        /*
                        try
                        {
                            if (oRef == null)
                                return;
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(oRef);
                            if (props != null && props.Count > 0 && props[sProp] != null)
                            {
                                props[sProp].SetValue(oRef, value);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logs.LogManager.Error("Error detected ", "ProperyEx");
                            Logs.LogManager.Error("  Set Property : " + sProp, "ProperyEx");
                            Logs.LogManager.Error(ex, "ProperyEx");
                            return;
                        }
                        */
                    }
                }
                else
                {
                    oValue = value;
                }
            }
        }
        /// <summary>
        /// Get/Set description associated with the PropertyEx.
        /// </summary>
        [Category("Appearance"), DescriptionAttribute("Set description associated with the PropertyEx.")]
        public string Description
        {
            get
            {
                return sDescription;
            }
            set
            {
                sDescription = value;
            }
        }
        /// <summary>
        /// Get/Set category associated with the PropertyEx.
        /// </summary>
        [Category("Appearance"), DescriptionAttribute("Set category associated with the PropertyEx.")]
        public string Category
        {
            get
            {
                return sCategory;
            }
            set
            {
                sCategory = value;
            }
        }
        /// <summary>
        /// Get/Set Target Type
        /// </summary>
        [XmlIgnore()]
        public System.Type Type
        {
            get
            {
                if (Value != null)
                {
                    return Value.GetType();
                }
                else
                {
                    if (oDefaultValue != null)
                    {
                        return oDefaultValue.GetType();
                    }
                    else
                    {
                        return oDefaultType;
                    }
                }
            }
        }
        /// <summary>
        /// Get/Set Property Attributes
        /// </summary>
        [XmlIgnore()]
        public AttributeCollection Attributes
        {
            get
            {
                return oCustomAttributes;
            }
            set
            {
                oCustomAttributes = value;
            }
        }
        /// <summary>
        /// Indicates if the property is browsable or not.
        /// </summary>
        [Category("Behavior"), DescriptionAttribute("Indicates if the property is browsable or not."), XmlElementAttribute(IsNullable = false)]
        public bool IsBrowsable
        {
            get
            {
                return bIsBrowsable;
            }
            set
            {
                bIsBrowsable = value;
                if (value == true)
                {
                    BuildAttributes_BrowsableProperty();
                }
            }
        }
        /// <summary>
        /// Indicates whether the name of the associated property is displayed with parentheses in the 
        /// Properties window.
        /// </summary>
        [Category("Appearance"), DisplayName("Parenthesize"), DescriptionAttribute("Indicates whether the name of the associated property is displayed with parentheses in the Properties window."), DefaultValue(false), XmlElementAttribute("Parenthesize")]
        public bool Parenthesize
        {
            get
            {
                return bParenthesize;
            }
            set
            {
                bParenthesize = value;
            }
        }
        /// <summary>
        /// Indicates the style of the label when a property is browsable.
        /// </summary>
        [Category("Behavior"),
        DescriptionAttribute("Indicates the style of the label when a property is browsable."),
        XmlElementAttribute(IsNullable = false)]
        public LabelStyle BrowsableLabelStyle
        {
            get
            {
                return eBrowsablePropertyLabel;
            }
            set
            {
                bool Update = false;
                if (value != eBrowsablePropertyLabel)
                {
                    Update = true;
                }
                eBrowsablePropertyLabel = value;
                if (Update)
                {
                    Design.BrowsableLabelStyleAttribute style = new Design.BrowsableLabelStyleAttribute(value);
                    oCustomAttributes = new AttributeCollection(new Attribute[] { style });
                }
            }
        }
        /// <summary>
        /// Indicates if the property is masked or not.
        /// </summary>
        [Category("Behavior"), DescriptionAttribute("Indicates if the property is masked or not."), XmlElementAttribute(IsNullable = false)]
        public bool IsPassword
        {
            get
            {
                return bIsPassword;
            }
            set
            {
                bIsPassword = value;
            }
        }
        /// <summary>
        /// Indicates if the property represents a value in percentage.
        /// </summary>
        [Category("Behavior"), DescriptionAttribute("Indicates if the property represents a value in percentage."), XmlElementAttribute(IsNullable = false)]
        public bool IsPercentage
        {
            get
            {
                return bIsPercentage;
            }
            set
            {
                bIsPercentage = value;
            }
        }
        /// <summary>
        /// Indicates if the property uses a FileNameEditor converter.
        /// </summary>
        [Category("Behavior"), DescriptionAttribute("Indicates if the property uses a FileNameEditor converter."), XmlElementAttribute(IsNullable = false)]
        public bool UseFileNameEditor
        {
            get
            {
                return bUseFileNameEditor;
            }
            set
            {
                bUseFileNameEditor = value;
            }
        }
        /// <summary>
        /// Apply a filter to FileNameEditor converter.
        /// </summary>
        [Category("Behavior"), DescriptionAttribute("Apply a filter to FileNameEditor converter."), XmlElementAttribute(IsNullable = false)]
        public string FileNameFilter
        {
            get
            {
                return sFilter;
            }
            set
            {
                bool UpdateAttributes = false;
                if (value != sFilter)
                {
                    UpdateAttributes = true;
                }
                sFilter = value;
                if (UpdateAttributes)
                {
                    BuildAttributes_FilenameEditor();
                }
            }
        }
        /// <summary>
        /// DialogType of the FileNameEditor.
        /// </summary>
        [Category("Behavior"), DescriptionAttribute("DialogType of the FileNameEditor."), XmlElementAttribute(IsNullable = false)]
        public FileDialogType FileNameDialogType
        {
            get
            {
                return eDialogType;
            }
            set
            {
                bool UpdateAttributes = false;
                if (value != eDialogType)
                {
                    UpdateAttributes = true;
                }
                eDialogType = value;
                if (UpdateAttributes)
                {
                    BuildAttributes_FilenameEditor();
                }
            }
        }
        /// <summary>
        /// Get/Set PropertyEx Choices list.
        /// </summary>
        [Category("Behavior"), DescriptionAttribute("Get/Set PropertyEx Choices list."), XmlIgnore()]
        public PropertyExChoices Choices
        {
            get
            {
                return oChoices;
            }
            set
            {
                oChoices = value;
                BuildAttributes_CustomChoices();
            }
        }
        /// <summary>
        /// Get/Set DataSource
        /// </summary>
        [Category("Databinding"), XmlIgnore()]
        public object Datasource
        {
            get
            {
                return oDatasource;
            }
            set
            {
                oDatasource = value;
                BuildAttributes_ListboxEditor();
            }
        }
        /// <summary>
        /// Get/Set Value Member
        /// </summary>
        [Category("Databinding"), XmlElementAttribute(IsNullable = false)]
        public string ValueMember
        {
            get
            {
                return sValueMember;
            }
            set
            {
                sValueMember = value;
                BuildAttributes_ListboxEditor();
            }
        }
        /// <summary>
        /// Get/Set Display Member
        /// </summary>
        [Category("Databinding"), XmlElementAttribute(IsNullable = false)]
        public string DisplayMember
        {
            get
            {
                return sDisplayMember;
            }
            set
            {
                sDisplayMember = value;
                BuildAttributes_ListboxEditor();
            }
        }
        /// <summary>
        /// Get/Set SelectedValue
        /// </summary>
        [Category("Databinding"), XmlElementAttribute(IsNullable = false)]
        public object SelectedValue
        {
            get
            {
                return oSelectedValue;
            }
            set
            {
                oSelectedValue = value;
            }
        }
        /// <summary>
        /// Get/Set SelectedItem
        /// </summary>
        [Category("Databinding"), XmlElementAttribute(IsNullable = false)]
        public object SelectedItem
        {
            get
            {
                return oSelectedItem;
            }
            set
            {
                oSelectedItem = value;
            }
        }
        /// <summary>
        /// Get/Set Is Dropdown Resizable
        /// </summary>
        [Category("Databinding"), XmlElementAttribute(IsNullable = false)]
        public bool IsDropdownResizable
        {
            get
            {
                return bIsDropdownResizable;
            }
            set
            {
                bIsDropdownResizable = value;
                BuildAttributes_ListboxEditor();
            }
        }
        /// <summary>
        /// Get/Set Custom Editor
        /// </summary>
        [XmlIgnore()]
        public UITypeEditor CustomEditor
        {
            get
            {
                return oCustomEditor;
            }
            set
            {
                oCustomEditor = value;
            }
        }
        /// <summary>
        /// Get/Set Custom TypeConverter
        /// </summary>
        [XmlIgnore()]
        public TypeConverter CustomTypeConverter
        {
            get
            {
                return oCustomTypeConverter;
            }
            set
            {
                oCustomTypeConverter = value;
            }
        }
        /// <summary>
        /// Get/Set Tag
        /// </summary>
        [XmlIgnore()]
        public object Tag
        {
            get
            {
                return oTag;
            }
            set
            {
                oTag = value;
            }
        }
        /// <summary>
        /// Get/Set Default Value
        /// </summary>
        [XmlIgnore()]
        public object DefaultValue
        {
            get
            {
                return oDefaultValue;
            }
            set
            {
                oDefaultValue = value;
            }
        }
        /// <summary>
        /// Get/Set Default Type
        /// </summary>
        [XmlIgnore()]
        public Type DefaultType
        {
            get
            {
                return oDefaultType;
            }
            set
            {
                oDefaultType = value;
            }
        }
        /// <summary>
        /// Get/Set OnClick Event
        /// </summary>
        [XmlIgnore()]
        public UICustomEventHandler OnClick
        {
            get
            {
                return MethodDelegate;
            }
            set
            {
                MethodDelegate = value;
                BuildAttributes_CustomEventProperty();
            }
        }

        #endregion
    }

    #endregion

    #region PropertyGridEx

    /// <summary>
    /// PropertyGridEx Control
    /// </summary>
    [global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()] // For Visual basic
    public class PropertyGridEx : PropertyGrid
    {
        #region Internal Variable

        // CustomPropertyCollection assigned to SelectedObject
        private PropertyExCollection oCustomPropertyCollection;
        private bool bShowCustomProperties;

        // CustomPropertyCollectionSet assigned to SelectedObjects
        private PropertyExCollectionSet oCustomPropertyCollectionSet;
        private bool bShowCustomPropertiesSet;

        // Internal PropertyGrid Controls
        private object oPropertyGridView;
        private object oHotCommands;
        private object oDocComment;
        private ToolStrip oToolStrip;

        // Internal PropertyGrid Fields
        private Label oDocCommentTitle;
        private Label oDocCommentDescription;
        private FieldInfo oPropertyGridEntries;

        // Properties variables
        private bool bAutoSizeProperties;
        private bool bDrawFlatToolbar;
        //Required by the Windows Form Designer
        private System.ComponentModel.Container components = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public PropertyGridEx()
            : base()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            // Initialize collections
            oCustomPropertyCollection = new PropertyExCollection();
            oCustomPropertyCollectionSet = new PropertyExCollectionSet();

            // Attach internal controls
            oPropertyGridView = base.GetType().BaseType.InvokeMember("gridView", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, this, null);
            oHotCommands = base.GetType().BaseType.InvokeMember("hotcommands", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, this, null);
            oToolStrip = (ToolStrip)base.GetType().BaseType.InvokeMember("toolStrip", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, this, null);
            oDocComment = base.GetType().BaseType.InvokeMember("doccomment", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, this, null);

            // Attach DocComment internal fields
            if (oDocComment != null)
            {
                oDocCommentTitle = (Label)oDocComment.GetType().InvokeMember("m_labelTitle", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, oDocComment, null);
                oDocCommentDescription = (Label)oDocComment.GetType().InvokeMember("m_labelDesc", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, oDocComment, null);
            }

            // Attach PropertyGridView internal fields
            if (oPropertyGridView != null)
            {
                oPropertyGridEntries = oPropertyGridView.GetType().GetField("allGridEntries", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            }

            // Apply Toolstrip style
            if (oToolStrip != null)
            {
                ApplyToolStripRenderMode(bDrawFlatToolbar);
            }
        }

        #endregion

        #region Private Method

        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.
        //Do not modify it using the code editor.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        }

        #endregion

        #region Override

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        [System.Diagnostics.DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// OnResize
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            if (bAutoSizeProperties)
            {
                AutoSizeSplitter(32);
            }
        }
        /// <summary>
        /// Refresh
        /// </summary>
        public override void Refresh()
        {
            if (bShowCustomPropertiesSet)
            {
                base.SelectedObjects = (object[])oCustomPropertyCollectionSet.ToArray();
            }
            base.Refresh();
            if (bAutoSizeProperties)
            {
                AutoSizeSplitter(32);
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// MoveSplitterTo
        /// </summary>
        /// <param name="x"></param>
        public void MoveSplitterTo(int x)
        {
            oPropertyGridView.GetType().InvokeMember("MoveSplitterTo", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, oPropertyGridView, new object[] { x });
        }
        /// <summary>
        /// SetComment
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        public void SetComment(string title, string description)
        {
            MethodInfo method = oDocComment.GetType().GetMethod("SetComment");
            method.Invoke(oDocComment, new object[] { title, description });
            //oDocComment.SetComment(title, description);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// AutoSizeSplitter
        /// </summary>
        /// <param name="RightMargin"></param>
        protected void AutoSizeSplitter(int RightMargin)
        {
            GridItemCollection oItemCollection = (System.Windows.Forms.GridItemCollection)oPropertyGridEntries.GetValue(oPropertyGridView);
            if (oItemCollection == null)
            {
                return;
            }
            System.Drawing.Graphics oGraphics = System.Drawing.Graphics.FromHwnd(this.Handle);
            int CurWidth = 0;
            int MaxWidth = 0;

            foreach (GridItem oItem in oItemCollection)
            {
                if (oItem.GridItemType == GridItemType.Property)
                {
                    CurWidth = (int)oGraphics.MeasureString(oItem.Label, this.Font).Width + RightMargin;
                    if (CurWidth > MaxWidth)
                    {
                        MaxWidth = CurWidth;
                    }
                }
            }

            MoveSplitterTo(MaxWidth);
        }
        /// <summary>
        /// ApplyToolStripRenderMode
        /// </summary>
        /// <param name="value"></param>
        protected void ApplyToolStripRenderMode(bool value)
        {
            if (value)
            {
                oToolStrip.Renderer = new ToolStripSystemRenderer();
            }
            else
            {
                ToolStripProfessionalRenderer renderer = new ToolStripProfessionalRenderer(new CustomColorScheme());
                renderer.RoundedEdges = false;
                oToolStrip.Renderer = renderer;
            }
        }
        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set the collection of the PropertyEx. Set ShowCustomProperties to True to enable it.
        /// </summary>
        [Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DescriptionAttribute("Set the collection of the PropertyEx. Set ShowCustomProperties to True to enable it."), RefreshProperties(RefreshProperties.Repaint)]
        public PropertyExCollection Items
        {
            get
            {
                return oCustomPropertyCollection;
            }
        }
        /// <summary>
        /// Set the PropertyEx Collection Set. Set ShowPropertiesSet to True to enable it.
        /// </summary>
        [Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DescriptionAttribute("Set the CustomPropertyCollectionSet. Set ShowCustomPropertiesSet to True to enable it."), RefreshProperties(RefreshProperties.Repaint)]
        public PropertyExCollectionSet PropertiesSet
        {
            get
            {
                return oCustomPropertyCollectionSet;
            }
        }
        /// <summary>
        /// Get/Set is PropertyGrid Move automatically the splitter to better fit all the properties shown.
        /// </summary>
        [Category("Behavior"), DefaultValue(false), DescriptionAttribute("Move automatically the splitter to better fit all the properties shown.")]
        public bool AutoSizeProperties
        {
            get
            {
                return bAutoSizeProperties;
            }
            set
            {
                bAutoSizeProperties = value;
                if (value)
                {
                    AutoSizeSplitter(32);
                }
            }
        }
        /// <summary>
        /// Use the custom properties collection as SelectedObject.
        /// </summary>
        [Category("Behavior"), DefaultValue(false), DescriptionAttribute("Use the custom properties collection as SelectedObject."), RefreshProperties(RefreshProperties.All)]
        public bool ShowCustomProperties
        {
            get
            {
                return bShowCustomProperties;
            }
            set
            {
                if (value == true)
                {
                    bShowCustomPropertiesSet = false;
                    base.SelectedObject = oCustomPropertyCollection;
                }
                bShowCustomProperties = value;
            }
        }
        /// <summary>
        /// Use the custom properties collections as SelectedObjects.
        /// </summary>
        [Category("Behavior"), DefaultValue(false), DescriptionAttribute("Use the custom properties collections as SelectedObjects."), RefreshProperties(RefreshProperties.All)]
        public bool ShowPropertiesSet
        {
            get
            {
                return bShowCustomPropertiesSet;
            }
            set
            {
                if (value == true)
                {
                    bShowCustomProperties = false;
                    base.SelectedObjects = (object[])oCustomPropertyCollectionSet.ToArray();
                }
                bShowCustomPropertiesSet = value;
            }
        }
        /// <summary>
        /// Get/Set Draw a flat toolbar
        /// </summary>
        [Category("Appearance"), DefaultValue(false), DescriptionAttribute("Draw a flat toolbar")]
        public new bool DrawFlatToolbar
        {
            get
            {
                return bDrawFlatToolbar;
            }
            set
            {
                bDrawFlatToolbar = value;
                ApplyToolStripRenderMode(bDrawFlatToolbar);
            }
        }
        /// <summary>
        /// Access Toolbar object
        /// </summary>
        [Category("Appearance"), DisplayName("Toolstrip"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DescriptionAttribute("Toolbar object"), Browsable(true)]
        public ToolStrip ToolStrip
        {
            get
            {
                return oToolStrip;
            }
        }
        /// <summary>
        /// Represent the comments area of the PropertyGrid.
        /// </summary>
        [Category("Appearance"), DisplayName("Help"), DescriptionAttribute("DocComment object. Represent the comments area of the PropertyGrid."), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Control DocComment
        {
            get
            {
                return (System.Windows.Forms.Control)oDocComment;
            }
        }
        /// <summary>
        /// Access Help Title Label Control.
        /// </summary>
        [Category("Appearance"), DisplayName("HelpTitle"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DescriptionAttribute("Help Title Label."), Browsable(true)]
        public Label DocCommentTitle
        {
            get
            {
                return oDocCommentTitle;
            }
        }
        /// <summary>
        /// Access Help Description Label Control.
        /// </summary>
        [Category("Appearance"), DisplayName("HelpDescription"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DescriptionAttribute("Help Description Label."), Browsable(true)]
        public Label DocCommentDescription
        {
            get
            {
                return oDocCommentDescription;
            }
        }
        /// <summary>
        /// Get/Set Help Image Background.
        /// </summary>
        [Category("Appearance"), DisplayName("HelpImageBackground"), DescriptionAttribute("Help Image Background.")]
        public Image DocCommentImage
        {
            get
            {
                return ((Control)oDocComment).BackgroundImage;
            }
            set
            {
                ((Control)oDocComment).BackgroundImage = value;
            }
        }

        #endregion

    }

    #endregion
}

