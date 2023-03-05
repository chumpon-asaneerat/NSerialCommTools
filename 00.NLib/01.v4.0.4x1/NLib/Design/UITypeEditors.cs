#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2010-01-23
=================
- All DesignTime editors
  - Changed SaveFileDialog to System.Windows.Forms.SaveFileDialog.

======================================================================================================================
Update 2010-01-23
=================
- All DesignTime editors ported from GFA Library GFA37 tor GFA38v3

======================================================================================================================
Update 2008-11-29
=================
- DesignTime editors move from GFA.Lib to GFA.Lib.Core.

======================================================================================================================
Update 2008-10-16
=================
- DesignTime editor
  - CustomDropDownPropertyEditor Add DropdownWidth property (protected) for custom set width 
	of dropdown window.

======================================================================================================================
Update 2008-04-17
=================
- DesignTime editor
  - Add SaveTextFileEditor.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

#endregion

namespace NLib.Design
{
	#region Custom Dropdown Property Editor

	/// <summary>
	/// Custom DropDown Property Editor (abstract) for Dropdown list style property's editor
	/// </summary>
	public abstract class CustomDropDownPropertyEditor : System.Drawing.Design.UITypeEditor
	{
		#region Internal Variable

		private int comboWd = 300;
		private IWindowsFormsEditorService edSvc = null;

		#endregion

		#region Protected access

		/// <summary>
		/// Get/Set Dropdown Width
		/// </summary>
		protected int DropdownWidth { get { return comboWd; } set { comboWd = value; } }

		#endregion

		#region Value Change Event Handler for ListBox

		/// <summary>
		/// Track changed when user commit selection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ValueChanged(object sender, EventArgs e)
		{
			if (edSvc != null) edSvc.CloseDropDown();
		}

		#endregion

		#region Init Listbox Value

		/// <summary>
		/// SetEditorProps override to initialization the list box in popup window
		/// </summary>
		/// <param name="editingInstance">object to edit</param>
		/// <param name="editor">List box to used in Popup window</param>
		protected virtual void SetEditorProps(object editingInstance, ListBox editor)
		{
		}

		#endregion

		#region Edit Value

		/// <summary>
		/// EditValue
		/// </summary>
		/// <param name="context">see ITypeDescriptorContext</param>
		/// <param name="provider">see IServiceProvider</param>
		/// <param name="value">object to edit</param>
		/// <returns>object instance after edit</returns>
		public override object EditValue(ITypeDescriptorContext context,
			IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null && context != null && context.Instance != null)
				{
					ListBox list = new ListBox();
					list.BorderStyle = BorderStyle.None;
					list.Width = comboWd;
					list.SelectedIndexChanged += new EventHandler(this.ValueChanged);

					SetEditorProps(context.Instance, list);
					list.SelectedItem = value;
					edSvc.DropDownControl(list);
					value = list.SelectedItem;

					//list.Dispose();
					//list = null;
				}
			}
			return value;
		}

		#endregion

		#region Edit Style (Default is Dropdown)

		/// <summary>
		/// GetEditStyle (Default is Dropdown)
		/// </summary>
		/// <param name="context">see ITypeDescriptorContext</param>
		/// <returns>UITypeEditorEditStyle Type (Default is Dropdown)</returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}

		#endregion
	}

	#endregion

	#region Custom Dropdown Flag Editor

	/// <summary>
	/// Custom Dropdown Flag Editor (abstract) for Dropdown list style flag's editor
	/// </summary>
	public abstract class CustomDropDownFlagEditor : System.Drawing.Design.UITypeEditor
	{
		#region Internal Class

		/// <summary>
		/// Internal class used for storing custom data in listviewitems
		/// </summary>
		internal class clbItem
		{
			#region Internal Variable

			private string str;
			private int value;
			private string tooltip;

			#endregion

			#region Constructor

			/// <summary>
			/// Creates a new instance of the <c>clbItem</c>
			/// </summary>
			/// <param name="str">The string to display in the <c>ToString</c> method. 
			/// It will contains the name of the flag</param>
			/// <param name="value">The integer value of the flag</param>
			/// <param name="tooltip">The tooltip to display in the <see cref="CheckedListBox"/></param>
			public clbItem(string str, int value, string tooltip)
			{
				this.str = str;
				this.value = value;
				this.tooltip = tooltip;
			}

			#endregion

			#region Public Property

			/// <summary>
			/// Gets the int value for this item
			/// </summary>
			public int Value
			{
				get { return value; }
			}
			/// <summary>
			/// Gets the tooltip for this item
			/// </summary>
			public string Tooltip
			{
				get { return tooltip; }
			}
			/// <summary>
			/// Gets the name of this item
			/// </summary>
			/// <returns>The name passed in the constructor</returns>
			public override string ToString()
			{
				return str;
			}

			#endregion
		}

		#endregion

		#region Internal Variable

		private IWindowsFormsEditorService edSvc = null;
		private CheckedListBox clb;
		private ToolTip tooltipControl;

		#endregion

		#region Init Listbox Value

		/// <summary>
		/// SetEditorProps override to initialization the list box in popup window
		/// </summary>
		/// <param name="editingInstance">object to edit</param>
		/// <param name="editor">Checked List box to used in Popup window</param>
		protected virtual void SetEditorProps(object editingInstance, CheckedListBox editor)
		{
		}

		#endregion

		#region Edit Value

		/// <summary>
		/// EditValue
		/// </summary>
		/// <param name="context">see ITypeDescriptorContext</param>
		/// <param name="provider">see IServiceProvider</param>
		/// <param name="value">object to edit</param>
		/// <returns>object instance after edit</returns>
		public override object EditValue(ITypeDescriptorContext context,
			IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					clb = new CheckedListBox();
					clb.BorderStyle = BorderStyle.None;
					clb.Width = 300;
					clb.CheckOnClick = true;
					clb.MouseDown += new MouseEventHandler(this.OnMouseDown);
					clb.MouseMove += new MouseEventHandler(this.OnMouseMoved);

					tooltipControl = new ToolTip();
					tooltipControl.ShowAlways = true;

					SetEditorProps(context.Instance, clb);

					#region Original code (incorrect)
					/*
					foreach(string name in Enum.GetNames(context.PropertyDescriptor.PropertyType))
					{
						// Get the enum value
						object enumVal = Enum.Parse(context.PropertyDescriptor.PropertyType, name);
						// Get the int value 
						int intVal = (int) Convert.ChangeType(enumVal, typeof(int));
						
						// Get the description attribute for this field
						System.Reflection.FieldInfo fi = context.PropertyDescriptor.PropertyType.GetField(name);
						DescriptionAttribute[] attrs = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

						// Store the the description
						string tooltip = attrs.Length > 0 ? attrs[0].Description : string.Empty;

						// Get the int value of the current enum value (the one being edited)
						int intEdited = (int) Convert.ChangeType(value, typeof(int));

						// Creates a clbItem that stores the name, the int value and the tooltip
						clbItem item = new clbItem(enumVal.ToString(), intVal, tooltip);

						// Get the checkstate from the value being edited
						bool checkedItem = (intEdited & intVal) > 0;

						// Add the item with the right check state
						clb.Items.Add(item, checkedItem);
					}
					*/
					#endregion

					int intEdited = (int)Convert.ChangeType(value, typeof(int));

					Array enums = Enum.GetValues(context.PropertyDescriptor.PropertyType);
					foreach (object enumVal in enums)
					{
						// Get the int value 
						string name = enumVal.ToString();
						int intVal = (int)Convert.ChangeType(enumVal, typeof(int));

						// Get the description attribute for this field
						System.Reflection.FieldInfo fi = context.PropertyDescriptor.PropertyType.GetField(name);
						DescriptionAttribute[] attrs = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

						// Store the the description
						string tooltip = attrs.Length > 0 ? attrs[0].Description : string.Empty;

						// Creates a clbItem that stores the name, the int value and the tooltip
						clbItem item = new clbItem(name, intVal, tooltip);

						// Get the checkstate from the value being edited
						bool checkedItem = ((intEdited & intVal) == intVal);

						// Add the item with the right check state
						clb.Items.Add(item, checkedItem);
					}

					// Show our CheckedListbox as a DropDownControl. 
					// This methods returns only when the dropdowncontrol is closed
					edSvc.DropDownControl(clb);

					// Get the sum of all checked flags
					int result = 0;
					foreach (clbItem obj in clb.CheckedItems)
					{
						result += obj.Value;
					}

					// return the right enum value corresponding to the result
					value = Enum.ToObject(context.PropertyDescriptor.PropertyType, result);
				}
			}
			return value;
		}

		#endregion

		private bool handleLostfocus = false;

		/// <summary>
		/// When got the focus, handle the lost focus event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (!handleLostfocus && clb.ClientRectangle.Contains(clb.PointToClient(new Point(e.X, e.Y))))
			{
				clb.LostFocus += new EventHandler(this.ValueChanged);
				handleLostfocus = true;
			}
		}
		/// <summary>
		/// Occurs when the mouse is moved over the checkedlistbox. 
		/// Sets the tooltip of the item under the pointer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseMoved(object sender, MouseEventArgs e)
		{
			int index = clb.IndexFromPoint(e.X, e.Y);
			if (index >= 0)
				tooltipControl.SetToolTip(clb, ((clbItem)clb.Items[index]).Tooltip);
		}
		/// <summary>
		/// Close the dropdowncontrol when the user has selected a value
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ValueChanged(object sender, EventArgs e)
		{
			if (edSvc != null)
			{
				edSvc.CloseDropDown();
			}
		}

		#region Edit Style (Default is Dropdown)

		/// <summary>
		/// GetEditStyle (Default is Dropdown)
		/// </summary>
		/// <param name="context">see ITypeDescriptorContext</param>
		/// <returns>UITypeEditorEditStyle Type (Default is Dropdown)</returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}

		#endregion
	}

	#endregion

	#region Enum Flags Editor

	/// <summary>
	/// Enum Flags Editor (General used)
	/// </summary>
	public class EnumFlagsEditor : CustomDropDownFlagEditor { }

	#endregion

	#region Custom Property Edit's Form (abstract)

	/// <summary>
	/// Custom Property Edit's Form (abstract)
	/// </summary>
	public abstract class CustomPropertyEditForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Set Edit Value
		/// </summary>
		/// <param name="obj">object's owner</param>
		/// <param name="value">property's value to edit</param>
		public abstract void SetEditValue(object obj, object value);
		/// <summary>
		/// Get Edit Value
		/// </summary>
		/// <param name="obj">object's owner</param>
		/// <returns>property's value after edit</returns>
		public abstract object GetEditValue(object obj);
	}

	#endregion

	#region Custom Modal Property Editor

	/// <summary>
	/// Custom Modal Property Editor (abstract) for Modal Dialog style property's editor
	/// </summary>
	public abstract class CustomModalPropertyEditor : System.Drawing.Design.UITypeEditor
	{
		#region Internal Variable

		private IWindowsFormsEditorService edSvc = null;

		#endregion

		#region CreateEditForm

		/// <summary>
		/// CreateEditForm
		/// </summary>
		/// <returns>CustomPropertyEditForm instance</returns>
		protected abstract CustomPropertyEditForm CreateEditForm();

		#endregion

		#region Edit Value

		/// <summary>
		/// EditValue
		/// </summary>
		/// <param name="context">see ITypeDescriptorContext</param>
		/// <param name="provider">see IServiceProvider</param>
		/// <param name="value">object to edit</param>
		/// <returns>object instance after edit</returns>
		public override object EditValue(ITypeDescriptorContext context,
			IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					CustomPropertyEditForm _editForm = CreateEditForm();
					if (_editForm == null)
						return value;
					// init edit form
					_editForm.SetEditValue(context.Instance, value);
					if (edSvc.ShowDialog(_editForm) == DialogResult.OK)
					{
						value = _editForm.GetEditValue(context.Instance);
					}
					_editForm = null;
				}
			}
			return value;
		}

		#endregion

		#region Edit Style (Default is Modal)

		/// <summary>
		/// GetEditStyle (Default is Modal)
		/// </summary>
		/// <param name="context">see ITypeDescriptorContext</param>
		/// <returns>UITypeEditorEditStyle Type (Default is Modal)</returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}

		#endregion
	}

	#endregion

	#region Generic SaveFile Editor

	/// <summary>
	/// Generic Save File Editor
	/// </summary>
	public class SaveFileEditor : System.Drawing.Design.UITypeEditor
	{
		#region Overrides

		/// <summary>
		/// Setup EditStyle (Default is UITypeEditorEditStyle.Modal)
		/// </summary>
		/// <param name="context">ITypeDescriptorContext object's instance</param>
		/// <returns>UITypeEditorEditStyle that used with PropertyGrid</returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		/// <summary>
		/// See System.Drawing.Design.UITypeEditor Document
		/// </summary>
		/// <param name="context">ITypeDescriptorContext object's instance</param>
		/// <param name="provider">IServiceProvider object's instance</param>
		/// <param name="value">object instance for edit</param>
		/// <returns>result after edit completed</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			System.Windows.Forms.SaveFileDialog saveFileDialog =
				new System.Windows.Forms.SaveFileDialog();
			saveFileDialog.Filter = "All Files (*.*)|*.*";
			InitializeDialog(saveFileDialog);
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				return saveFileDialog.FileName;
			}
			else return value;
		}

		#endregion

		#region Virtuals

		/// <summary>
		/// Initialize Dialog before process Saving
		/// </summary>
		/// <param name="saveFileDialog">Save Dialog Instance</param>
		protected virtual void InitializeDialog(System.Windows.Forms.SaveFileDialog saveFileDialog)
		{
		}

		#endregion
	}

	#endregion

	#region Generic SaveTextFile Editor

	/// <summary>
	/// Generic Save Text File Editor
	/// </summary>
	public class SaveTextFileEditor : System.Drawing.Design.UITypeEditor
	{
		#region Overrides

		/// <summary>
		/// Setup EditStyle (Default is UITypeEditorEditStyle.Modal)
		/// </summary>
		/// <param name="context">ITypeDescriptorContext object's instance</param>
		/// <returns>UITypeEditorEditStyle that used with PropertyGrid</returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		/// <summary>
		/// See System.Drawing.Design.UITypeEditor Document
		/// </summary>
		/// <param name="context">ITypeDescriptorContext object's instance</param>
		/// <param name="provider">IServiceProvider object's instance</param>
		/// <param name="value">object instance for edit</param>
		/// <returns>result after edit completed</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			System.Windows.Forms.SaveFileDialog saveFileDialog = 
				new System.Windows.Forms.SaveFileDialog();
			saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
			InitializeDialog(saveFileDialog);
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				return saveFileDialog.FileName;
			}
			else return value;
		}

		#endregion

		#region Virtuals

		/// <summary>
		/// Initialize Dialog before process Saving
		/// </summary>
		/// <param name="saveFileDialog">Save Dialog Instance</param>
		protected virtual void InitializeDialog(System.Windows.Forms.SaveFileDialog saveFileDialog)
		{
		}

		#endregion
	}

	#endregion
}

