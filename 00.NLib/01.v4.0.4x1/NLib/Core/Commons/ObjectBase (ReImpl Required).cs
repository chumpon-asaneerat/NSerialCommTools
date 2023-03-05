#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2010-01-23
=================
- All Object Collections and related classes ported from GFA Library GFA37 tor GFA38v3
  - Changed all exception handling code to used new ExceptionManager class instread of 
	LogManager.

======================================================================================================================
Update 2008-06-12
=================
- Collection - Object Container and it's Collections Generic version
  - Has feature like ObjectContainer original version but can used with AutoImplement class.
  - Speed optimized
  - Support multithread access (thread safe event access).
  - Provide common interface IObjectContainer and IObjectContainerCollection
  - The Generic version of ObjectContainer support assign default to new object instance.

======================================================================================================================
Update 2008-06-03
=================
- Collection - Object Collections Portd from GFA27 in SysLib.Objects namespace.
  - ObjectBase and ObjectCollectionBase class added. These classes is implement PropertyXXXChanged machanism
	and support PropertyOrder Attribute.
  - ObjectElement and ObjectElementCollectionBase class added. These classes is implement NameChanged event
	used with object that has unique name in the collection.
  - CheckableObjectElement and CheckableObjectElementCollectionBase classes added. This class support checkable
	object that can used like CheckedListBox.
  - ObjectContainer and ObjectContainerCollection class added. These classes is implement for general used as
	Container that can keep object state and can undo if required.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

#endregion

#region Using For Generic Version

using System.Collections.Generic;

#endregion

#region Using Extras

using NLib.Collections;
using NLib.Design;
using NLib.Utils;
using NLib.Xml;

#endregion

#region Interfaces

namespace NLib.Collections
{
	/// <summary>
	/// The IObjectContainer interface.
	/// </summary>
	public interface IObjectContainer : IEditableObject, ICloneable
	{
		#region Begin/End Init

		/// <summary>
		/// Call when data is initialize.
		/// </summary>
		void BeginInit();
		/// <summary>
		/// Call when finished initialize.
		/// </summary>
		void EndInit();

		#endregion

		#region HasChanged/Accept/Reject

		/// <summary>
		/// Check that object has been changed or not.
		/// </summary>
		/// <returns>true if data has been changed.</returns>
		bool HasChanged();
		/// <summary>
		/// Accept Changed made on object.
		/// </summary>
		void Accept();
		/// <summary>
		/// Reject Changed made on object
		/// </summary>
		void Reject();

		#endregion

		#region Lock

		/// <summary>
		/// Check is Lock for Stop Event Raising.
		/// </summary>
		bool IsLock { get; }
		/// <summary>
		/// Lock to block Event Raising.
		/// </summary>
		void Lock();
		/// <summary>
		/// Unlock to allow Event Raising.
		/// </summary>
		void Unlock();

		#endregion

		#region Property

		/// <summary>
		/// Access Data object (Readonly).
		/// </summary>
		object Data { get; }
		/// <summary>
		/// Gets or sets No. or Object Index.
		/// </summary>
		int _No_ { get; }

		#endregion

		#region Event

		/// <summary>
		/// Raise when BeginInit() is called.
		/// </summary>
		event System.EventHandler OnBeginInit;
		/// <summary>
		/// Raise when EndInit() is called.
		/// </summary>
		event System.EventHandler OnEndInit;
		/// <summary>
		/// Raise when Object's Property is changed.
		/// </summary>
		event System.EventHandler OnPropertyChanged;

		#endregion
	}
	/// <summary>
	/// The IObjectContainer Collection interface.
	/// </summary>
	public interface IObjectContainerCollection : IList, ICollection, IEnumerable
	{
		#region Remove item managements

		/// <summary>
		/// Checks that has removed item in list.
		/// </summary>
		bool HasRemovedItem { get; }
		/// <summary>
		/// Gets Removed Items.
		/// </summary>
		/// <returns>Returns array of item that removed from collection.</returns>
		object[] GetRemoveItems();
		/// <summary>
		/// Clear all remove items.
		/// </summary>
		void ClearRemoveItems();

		#endregion

		#region Current item management

		/// <summary>
		/// Check is the collection is in pending state.
		/// </summary>
		bool IsPending { get; }
		/// <summary>
		/// Gets Current Pending object.
		/// </summary>
		/// <returns>Returns the instance of IObjectContainer.</returns>
		IObjectContainer GetPendingObject();

		#endregion

		#region Subset of IBindingList

		/// <summary>
		/// Create new object and set as pending item.
		/// </summary>
		/// <returns>Returns new object.</returns>
		IObjectContainer AddNew();
		/// <summary>
		/// Is allow edit item.
		/// </summary>
		bool AllowEdit { get; }
		/// <summary>
		/// Is allow add new.
		/// </summary>
		bool AllowNew { get; }
		/// <summary>
		/// Is allow remove.
		/// </summary>
		bool AllowRemove { get; }

		#endregion

		#region Property

		/// <summary>
		/// Gets Item base type.
		/// </summary>
		Type BaseType { get; }

		#endregion

		#region Events

		/// <summary>
		/// Collection Changed.
		/// </summary>
		event System.EventHandler CollectionChanged;
		/// <summary>
		/// Property Changed.
		/// </summary>
		event System.EventHandler PropertyChanged;

		#endregion
	}
}

#endregion

#region Object Base and Collections

namespace NLib.Collections
{
	#region ObjectBase and Collection class

	#region Object Base

	/// <summary>
	/// The Object Base (abstract class).
	/// </summary>
	[Serializable]
	public abstract class ObjectBase : ICustomTypeDescriptor
	{
		#region Intenal Variable

		private int _lockEvent = 0;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public ObjectBase() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~ObjectBase() { }

		#endregion

		#region Protected Methods

		#region Lock/Unlock Event

		/// <summary>
		/// Lock Events.
		/// </summary>
		protected internal void LockEvents() { _lockEvent++; }
		/// <summary>
		/// Unlock Events.
		/// </summary>
		protected internal void UnlockEvents()
		{
			_lockEvent--;
			if (_lockEvent < 0) _lockEvent = 0;
		}
		/// <summary>
		/// Is Lock Events.
		/// </summary>
		protected internal bool IsLockEvents { get { return (_lockEvent > 0); } }

		#endregion

		#region Raise Event

		/// <summary>
		/// Raise Event.
		/// </summary>
		/// <param name="eventHandler">The event handler.</param>
		protected void RaiseEvent(System.EventHandler eventHandler)
		{
			if (null != eventHandler && !IsLockEvents)
			{
				eventHandler.Invoke(this, System.EventArgs.Empty);
			}
		}

		#endregion

		#endregion

		#region ICustomTypeDescriptor Members

		#region Stub

		TypeConverter ICustomTypeDescriptor.GetConverter() { return null; }
		string ICustomTypeDescriptor.GetComponentName() { return null; }
		string ICustomTypeDescriptor.GetClassName() { return null; }
		AttributeCollection ICustomTypeDescriptor.GetAttributes() { return new AttributeCollection(null); }
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return null; }
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return null; }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) { return new EventDescriptorCollection(null); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { return new EventDescriptorCollection(null); }
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return null; }
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) { return this; }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(null);
		}

		#endregion

		#region GetProperties (not call recheck require. don't know why??)

		/// <summary>
		/// ICustomTypeDescriptor.GetProperties.
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this, attributes, true);

			ArrayList orderedProperties = new ArrayList();
			foreach (PropertyDescriptor pd in pdc)
			{
				Attribute attribute = pd.Attributes[typeof(PropertyOrderAttribute)];
				if (attribute != null)
				{
					PropertyOrderAttribute poa = (PropertyOrderAttribute)attribute;
					orderedProperties.Add(new PropertyOrderPair(pd.Name, poa.Order));
				}
				else orderedProperties.Add(new PropertyOrderPair(pd.Name, 0));
			}
			//
			// Perform the actual order using the value PropertyOrderPair classes
			// implementation of IComparable to sort
			//
			orderedProperties.Sort();
			//
			// Build a string list of the ordered names
			//
			List<string> propertyNames = new List<string>();
			foreach (PropertyOrderPair pop in orderedProperties) 
				propertyNames.Add(pop.Name);
			//
			// Pass in the ordered list for the PropertyDescriptorCollection to sort by
			//
			return pdc.Sort(propertyNames.ToArray());
		}

		#endregion

		#endregion
	}

	#endregion

	#region Object Collection Base

	/// <summary>
	/// The Object Collection Base.
	/// </summary>
	[ListBindable(BindableSupport.No)]
	public abstract class ObjectCollectionBase : CollectionBase, ITypedList
	{
		#region Internal Variable

		private bool __debug = false;
		private bool _debug
		{
			get
			{
				return __debug;
			}
			set { __debug = value; }
		}

		private string _defaultPrefix = "Name";
		private Dictionary<string, object> _caches = new Dictionary<string, object>();

		private bool _innerLock = false;

		private PropertyInfo info = null;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public ObjectCollectionBase() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~ObjectCollectionBase()
		{
			if (null != _caches) _caches.Clear();
			_caches = null;
		}

		#endregion

		#region Private Method

		#region Get/Set Property

		/// <summary>
		/// Gets Property Info.
		/// </summary>
		/// <param name="target">The target object.</param>
		/// <returns>The property information instance.</returns>
		private PropertyInfo GetPropertyInfo(object target)
		{
			PropertyInfo propInfo = null;

			if (null == target) return propInfo;
			if (string.IsNullOrWhiteSpace(PropertyName)) 
				return propInfo; // no property specificed

			if (null == ObjectType)
			{
				Type objType = target.GetType();
				propInfo = objType.GetProperty(PropertyName,
					BindingFlags.Public | BindingFlags.NonPublic |
					BindingFlags.CreateInstance | BindingFlags.Instance);
			}
			else
			{
				if (null == info)
				{
					info = ObjectType.GetProperty(PropertyName,
						BindingFlags.Public | BindingFlags.NonPublic |
						BindingFlags.CreateInstance | BindingFlags.Instance);
				}
				propInfo = info;
			}

			return propInfo;
		}
		/// <summary>
		/// Sets Property.
		/// </summary>
		/// <param name="target">The target object.</param>
		/// <param name="value">The value to set.</param>
		private void SetProperty(object target, string value)
		{
			if (null == target) return;
			if (string.IsNullOrWhiteSpace(PropertyName)) 
				return; // no property specificed

			PropertyInfo propInfo = GetPropertyInfo(target);
			if (null == propInfo && propInfo.PropertyType == typeof(string))
			{
				try
				{
					propInfo.SetValue(target, value, new object[0]);
				}
				catch (Exception ex)
				{
					ex.Err();
					if (_debug)
					{
						Console.WriteLine("SetProperty : Exception " + ex.Message);
					}
				}
			}
			else
			{
				if (_debug)
				{
					Console.WriteLine("SetProperty : Cannot Get Property Info");
				}
			}
		}
		/// <summary>
		/// Gets Object's Value for check.
		/// </summary>
		/// <param name="target">The object to get Name's property.</param>
		/// <returns>Returns empty string if cannot access property value otherwise
		/// return Name value of target object.</returns>
		private string GetProperty(object target)
		{
			string result = "";
			if (null == target)
			{
				if (_debug)
				{
					Console.WriteLine("GetProperty : target object is null");
				}
				return result;
			}
			if (string.IsNullOrWhiteSpace(PropertyName))
			{
				if (_debug)
				{
					Console.WriteLine("GetProperty : PropertyName is not assigned");
				}
				return result; // no property specificed
			}

			PropertyInfo propInfo = GetPropertyInfo(target);

			if (propInfo != null && propInfo.PropertyType == typeof(string))
			{
				try
				{
					object propVal = propInfo.GetValue(target, new object[0]);
					if (propVal != null) result = (string)propVal;
				}
				catch (Exception ex)
				{
					ex.Err();
					if (_debug)
					{
						Console.WriteLine("GetProperty : Exception " + ex.Message);
					}
				}
			}
			else
			{
				if (_debug)
				{
					Console.WriteLine("GetProperty : Cannot Get Property Info");
				}
			}

			return result;
		}

		#endregion

		#region Changing/Changed

		/// <summary>
		/// Name Changing Event Handler.
		/// </summary>
		/// <param name="sender">sender object.</param>
		/// <param name="e">event paramter not used.</param>
		private void obj_NameChanging(object sender, EventArgs e)
		{
			if (null != sender && null != ObjectType && 
				!string.IsNullOrWhiteSpace(PropertyName))
			{
				Type valType = sender.GetType();
				if (valType == ObjectType || valType.IsSubclassOf(ObjectType))
				{
					string name = GetProperty(sender);
					string key = FormatKey(name);
					if (_caches.ContainsKey(key))
					{
						if (_debug)
						{
							Console.WriteLine("Object Name Changing and exists in hash so remove from hash");
						}
						_caches.Remove(key); // remove from cache
					}
				}
			}
		}
		/// <summary>
		/// The Name Changed Event Handler.
		/// </summary>
		/// <param name="sender">sender object.</param>
		/// <param name="e">event paramter not used.</param>
		private void obj_NameChanged(object sender, EventArgs e)
		{
			if (_innerLock) 
				return;
			if (null != sender && null != ObjectType && 
				!string.IsNullOrWhiteSpace(PropertyName))
			{
				Type valType = sender.GetType();
				if (valType == ObjectType || valType.IsSubclassOf(ObjectType))
				{
					string name = GetProperty(sender);
					string key = FormatKey(name);
					string newName = GetUniqueKey();
					if (!_caches.ContainsKey(key))
					{
						if (_debug)
						{
							Console.WriteLine("Object Name Changed and new name not exists in hash so append to hash");
						}
						_caches.Add(key, sender); // add to cache
					}
					else
					{
						if (_debug)
						{
							Console.WriteLine("Object Name Changed and new name exists in hash so append to hash with new unique name and auto changed object's name");
						}
						key = FormatKey(newName);
						_caches.Add(key, sender); // add to cache

						// internal changed name
						_innerLock = true; // block recursive if no LockEvent Method

						//obj.LockEvents();
						MethodInfo med1 = valType.GetMethod("LockEvents",
							BindingFlags.Public | BindingFlags.NonPublic |
							BindingFlags.Instance);
						if (null != med1)
							med1.Invoke(sender, new object[0]);

						SetProperty(sender, newName);

						//obj.UnlockEvents();
						MethodInfo med2 = valType.GetMethod("UnlockEvents",
							BindingFlags.Public | BindingFlags.NonPublic |
							BindingFlags.Instance);
						if (null != med2)
							med2.Invoke(sender, new object[0]);

						_innerLock = false; // block recursive if no LockEvent Method
					}
				}
			}
		}

		#endregion

		#endregion

		#region Abstract Methods

		/// <summary>
		/// Gets Object Type that used in Collection.
		/// </summary>
		protected abstract Type ObjectType { get; }
		/// <summary>
		/// Gets Proeprty's Name that used to check.
		/// </summary>
		protected abstract string PropertyName { get; }
		/// <summary>
		/// Gets Proeprty's Changing Event's Name that used to hook.
		/// </summary>
		protected abstract string PropertyChangingEventName { get; }
		/// <summary>
		/// Gets Proeprty's Changed Event's Name that used to hook.
		/// </summary>
		protected abstract string PropertyChangedEventName { get; }

		#endregion

		#region Protected Method(s)

		/// <summary>
		/// Format Key.
		/// </summary>
		/// <param name="key">The check key.</param>
		/// <returns>Returns formating name used as key.</returns>
		protected virtual string FormatKey(string key)
		{
			return key.Trim().ToUpper();
		}
		/// <summary>
		/// Show Debug Console Message.
		/// </summary>
		[Category("Debug")]
		[Description("Show Debug Console Message")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShowDebugConsoleMessage { get { return _debug; } set { _debug = value; } }
		/// <summary>
		/// Access Internal HashTable should check null value before used it.
		/// </summary>
		protected Dictionary<string, object> InternalCaches { get { return _caches; } }
		/// <summary>
		/// Gets or sets Default Prefix Name.
		/// </summary>
		protected string DefaultPrefix { get { return _defaultPrefix; } set { _defaultPrefix = value; } }
		/// <summary>
		/// Gets Object by Name or key.
		/// </summary>
		/// <param name="key">key or name to find</param>
		/// <returns>if found return match object otherwise return null</returns>
		protected object GetByKey(string key)
		{
			string uniquekey = FormatKey(key);
			if (_caches.ContainsKey(uniquekey))
			{
				if (null != _caches[uniquekey])
					return _caches[uniquekey];
			}
			return null;
		}
		/// <summary>
		/// Gets Unique Key.
		/// </summary>
		/// <returns>Returns Unique Key Name.</returns>
		protected string GetUniqueKey()
		{
			int id = 1;
			string chkName = _defaultPrefix + id.ToString();
			string key = FormatKey(chkName);
			while (_caches.ContainsKey(key))
			{
				id++;
				chkName = _defaultPrefix + id.ToString();
				key = FormatKey(chkName);
			}
			return chkName;
		}

		#endregion

		#region Virtual Method(s)

		/// <summary>
		/// After Inserted.
		/// </summary>
		/// <param name="index">object's index in list.</param>
		/// <param name="value">value that inserted.</param>
		protected virtual void AfterInserted(int index, object value) { }
		/// <summary>
		/// After Removed.
		/// </summary>
		/// <param name="index">object's index in list.</param>
		/// <param name="value">value that removed.</param>
		protected virtual void AfterRemoved(int index, object value) { }

		#endregion

		#region Override(s)

		#region OnInsert - Handle check duplicate key for single object

		/// <summary>
		/// OnInsert.
		/// </summary>
		/// <param name="index">object's index in list.</param>
		/// <param name="value">value that inserting.</param>
		protected override void OnInsert(int index, object value)
		{
			if (null != value && null != ObjectType && 
				!string.IsNullOrWhiteSpace(PropertyName))
			{
				Type valType = value.GetType();
				if (valType == ObjectType || valType.IsSubclassOf(ObjectType))
				{
					string name = GetProperty(value);
					// check
					if (name.Length <= 0)
					{
						if (_debug)
						{
							Console.WriteLine("OnInsert object Name auto create because Name is blank");
						}
						// name not exist. auto assign name
						SetProperty(value, GetUniqueKey());
					}
					else
					{
						string key = FormatKey(name);
						if (_caches.ContainsKey(key))
						{
							if (_debug)
							{
								Console.WriteLine("OnInsert object Name auto create because Name is exists");
							}
							// name exists
							SetProperty(value, GetUniqueKey());
						}
					}
				}
				else
				{
					// Mismatch Type
					if (_debug)
					{
						Console.WriteLine("OnInsert Object Type mismatch");
					}
				}
			}
			else
			{
				// Object is null or ObjectType is not set or Property's Name not set
				if (_debug)
				{
					Console.WriteLine("OnInsert Object is null or ObjectType is not set or Property's Name not set");
				}
			}
			base.OnInsert(index, value);
		}

		#endregion

		#region OnInsertComplete - Handle Hook Event and Set Owner for single object

		/// <summary>
		/// OnInsertComplete.
		/// </summary>
		/// <param name="index">object's index in list.</param>
		/// <param name="value">value that inserted.</param>
		protected override void OnInsertComplete(int index, object value)
		{
			if (null != value && null != ObjectType &&
				!string.IsNullOrWhiteSpace(PropertyName))
			{
				Type valType = value.GetType();
				if (valType == ObjectType || valType.IsSubclassOf(ObjectType))
				{
					string name = GetProperty(value);

					// add event handler
					EventInfo evt1 = valType.GetEvent(PropertyChangingEventName,
						BindingFlags.Public | BindingFlags.NonPublic |
						BindingFlags.Instance);
					if (null != evt1)
					{
						try
						{
							evt1.AddEventHandler(value, new EventHandler(obj_NameChanging));
						}
						catch (Exception ex)
						{
							ex.Err();
							if (_debug)
							{
								Console.WriteLine("OnInsertCompleted : " + ex.Message);
							}
						}
					}
					EventInfo evt2 = valType.GetEvent(PropertyChangedEventName,
						BindingFlags.Public | BindingFlags.NonPublic |
						BindingFlags.Instance);
					if (null != evt2)
					{
						try
						{
							evt2.AddEventHandler(value, new EventHandler(obj_NameChanged));
						}
						catch (Exception ex)
						{
							ex.Err();
							if (_debug)
							{
								Console.WriteLine("OnInsertCompleted : " + ex.Message);
							}
						}
					}

					// add or replace to hash
					string key = FormatKey(name);
					if (!_caches.ContainsKey(key))
					{
						if (_debug)
						{
							Console.WriteLine("OnInsertCompleted object Name not exists so append to hash");
						}
						_caches.Add(key, value);
					}
					else
					{
						if (_debug)
						{
							Console.WriteLine("OnInsertCompleted object Name is exists so replace to hash");
						}
						_caches[key] = value;
					}
					// call virtual method
					AfterInserted(index, value);
				}
				else
				{
					// Mismatch Type
					if (_debug)
					{
						Console.WriteLine("OnInsertComplete Object Type mismatch");
					}
				}
			}
			else
			{
				// Object is null or ObjectType is not set or Property's Name not set
				if (_debug)
				{
					Console.WriteLine("OnInsertComplete Object is null or ObjectType is not set or Property's Name not set");
				}
			}
			base.OnInsertComplete(index, value);
		}

		#endregion

		#region OnRemoveComplete - Handle UnHook Event and Release Owner for single object

		/// <summary>
		/// OnRemoveComplete.
		/// </summary>
		/// <param name="index">object's index in list.</param>
		/// <param name="value">value that removed.</param>
		protected override void OnRemoveComplete(int index, object value)
		{
			if (null != value && null != ObjectType &&
				!string.IsNullOrWhiteSpace(PropertyName))
			{
				Type valType = value.GetType();
				if (valType == ObjectType || valType.IsSubclassOf(ObjectType))
				{
					string name = GetProperty(value);

					// Remove event handler
					EventInfo evt1 = valType.GetEvent(PropertyChangingEventName,
						BindingFlags.Public | BindingFlags.NonPublic |
						BindingFlags.Instance);
					if (null != evt1)
					{
						try
						{
							evt1.RemoveEventHandler(value, new EventHandler(obj_NameChanging));
						}
						catch (Exception ex)
						{
							ex.Err();
							if (_debug)
							{
								Console.WriteLine("OnRemoveComplete : " + ex.Message);
							}
						}
					}
					EventInfo evt2 = valType.GetEvent(PropertyChangedEventName,
						BindingFlags.Public | BindingFlags.NonPublic |
						BindingFlags.Instance);
					if (null != evt2)
					{
						try
						{
							evt2.RemoveEventHandler(value, new EventHandler(obj_NameChanged));
						}
						catch (Exception ex)
						{
							ex.Err();
							if (_debug)
							{
								Console.WriteLine("OnRemoveComplete : " + ex.Message);
							}
						}
					}

					// remove to hash
					string key = FormatKey(name);
					if (_caches.ContainsKey(key))
					{
						if (_debug)
						{
							Console.WriteLine("OnRemoveComplete object Name is exists so remove from hash");
						}
						_caches.Remove(key);
					}
					else
					{
						if (_debug)
						{
							Console.WriteLine("OnRemoveComplete object Name is not exists so do nothing");
						}
						_caches[key] = value;
					}
					// call virtual method
					AfterRemoved(index, value);
				}
				else
				{
					// Mismatch Type
					if (_debug)
					{
						Console.WriteLine("OnRemoveComplete Object Type mismatch");
					}
				}
			}
			else
			{
				// Object is null or ObjectType is not set or Property's Name not set
				if (_debug)
				{
					Console.WriteLine("OnRemoveComplete Object is null or ObjectType is not set or Property's Name not set");
				}
			}
			base.OnRemoveComplete(index, value);
		}

		#endregion

		#region OnClear - Handle UnHook Event and Release Owner for all object

		/// <summary>
		/// OnClear.
		/// </summary>
		protected override void OnClear()
		{
			int index = 0;
			foreach (object obj in List)
			{
				Type valType = obj.GetType();
				// Remove event handler
				EventInfo evt1 = valType.GetEvent(PropertyChangingEventName,
					BindingFlags.Public | BindingFlags.NonPublic |
					BindingFlags.Instance);
				if (null != evt1)
				{
					try
					{
						evt1.RemoveEventHandler(obj, new EventHandler(obj_NameChanging));
					}
					catch (Exception ex)
					{
						ex.Err();
						if (_debug)
						{
							Console.WriteLine("OnClear : " + ex.Message);
						}
					}
				}
				EventInfo evt2 = valType.GetEvent(PropertyChangedEventName,
					BindingFlags.Public | BindingFlags.NonPublic |
					BindingFlags.Instance);
				if (null != evt2)
				{
					try
					{
						evt2.RemoveEventHandler(obj, new EventHandler(obj_NameChanged));
					}
					catch (Exception ex)
					{
						ex.Err();
						if (_debug)
						{
							Console.WriteLine("OnClear : " + ex.Message);
						}
					}
				}
				// call virtual method
				AfterRemoved(index, obj);
				index++;
			}
			// clear hash
			if (null != _caches)
			{
				if (_debug)
				{
					Console.WriteLine("OnClear all object is clear so from hash");
				}
				_caches.Clear();
			}
			base.OnClear();
		}

		#endregion

		#endregion

		#region Public Method(s)

		/// <summary>
		/// Gets Cache Counts
		/// </summary>
		/// <returns>Returns Number of Key in Cache.</returns>
		public int GetCacheCount() 
		{
			if (null == _caches)
				return -1;
			return _caches.Count; 
		}
		/// <summary>
		/// Gets Object Keys.
		/// </summary>
		/// <returns>Returns Array of Object's Name (or key) in hash</returns>
		public string[] GetObjectKeys()
		{
			if (null == List || List.Count <= 0)
				return new string[0]; // no element

			List<string> list = new List<string>();

			string val = null;
			foreach (object obj in List)
			{
				val = GetProperty(obj);
				if (val.Length > 0 && !List.Contains(val))
					list.Add(val);
			}

			string[] results = list.ToArray();
			list.Clear();
			list = null;

			return results;
		}
		/// <summary>
		/// Checks is key is in collection.
		/// </summary>
		/// <param name="key">The key to check</param>
		/// <returns>Returns true if key is already exists.</returns>
		public bool Contains(string key)
		{
			if (null == _caches)
				return false;
			string uniquekey = FormatKey(key);
			return _caches.ContainsKey(uniquekey);
		}
		/// <summary>
		/// Gets Index of Value by object's instance.
		/// </summary>
		/// <param name="value">An object to find index.</param>
		/// <returns>Returns index of target object.</returns>
		public int IndexOf(object value)
		{
			return List.IndexOf(value);
		}
		/// <summary>
		/// Gets Index of Value by object's key.
		/// </summary>
		/// <param name="key">The key or name of object to find index.</param>
		/// <returns>Returns index of object.</returns>
		public int IndexOf(string key)
		{
			object value = GetByKey(key);
			if (null == value) return -1;
			else return List.IndexOf(value);
		}
		/// <summary>
		/// Remove By key.
		/// </summary>
		/// <param name="key">The key or name of object to remove</param>
		public void Remove(string key)
		{
			object value = GetByKey(key);
			if (null == value) return;
			else if (List.Contains(value))
			{
				List.Remove(value);
			}
		}

		#endregion

		#region ITypedList Members

		/// <summary>
		/// ITypedList.GetItemProperties.
		/// </summary>
		/// <param name="listAccessors">The PropertyDescriptor array.</param>
		/// <returns>Returns PropertyDescriptor Collection.</returns>
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this.ObjectType);
			//PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value, attributes);
			//PropertyDescriptorCollection pdc = new PropertyDescriptorCollection(listAccessors);
			ArrayList orderedProperties = new ArrayList();
			foreach (PropertyDescriptor pd in pdc)
			{
				Attribute attribute = pd.Attributes[typeof(PropertyOrderAttribute)];
				if (null != attribute)
				{
					PropertyOrderAttribute poa = (PropertyOrderAttribute)attribute;
					orderedProperties.Add(new PropertyOrderPair(pd.Name, poa.Order));
				}
				else orderedProperties.Add(new PropertyOrderPair(pd.Name, 0));
			}
			//
			// Perform the actual order using the value PropertyOrderPair classes
			// implementation of IComparable to sort
			orderedProperties.Sort();
			// Build a string list of the ordered names
			List<string> propertyNames = new List<string>();
			foreach (PropertyOrderPair pop in orderedProperties) 
				propertyNames.Add(pop.Name);
			// Pass in the ordered list for the PropertyDescriptorCollection to sort by
			return pdc.Sort(propertyNames.ToArray());
		}
		/// <summary>
		/// ITypedList.GetListName.
		/// </summary>
		/// <param name="listAccessors">The PropertyDescriptor array.</param>
		/// <returns>Returns Property's Name</returns>
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			if (null == listAccessors || listAccessors.Length == 0)
				return "JOE"; // nevermind, never queryed

			return listAccessors[listAccessors.Length - 1].Name; // works as TableName
		}

		#endregion
	}

	#endregion

	#endregion

	#region ObjectElement and Collection class

	#region ObjectElement (abstract)

	/// <summary>
	/// The ObjectElement (abstract).
	/// </summary>
	public abstract class ObjectElement : ObjectBase
	{
		#region Internal Variable

		private string _name = string.Empty;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public ObjectElement() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~ObjectElement() { }

		#endregion

		#region Protected Method

		/// <summary>
		/// Find Type.
		/// </summary>
		/// <param name="typeName">The type name in string.</param>
		/// <returns>Returns Type instance.</returns>
		protected Type FindType(string typeName)
		{
			if (!NETTypeCache.Instance.Contains(typeName))
			{
				Type type = NETTypeCache.Instance.FindType(typeName);
				if (null != type) NETTypeCache.Instance.Add(type);
			}
			return NETTypeCache.Instance[typeName];
		}

		#endregion

		#region Override Method

		#region ToString

		/// <summary>
		/// ToString.
		/// </summary>
		/// <returns>See Object.ToString().</returns>
		public override string ToString()
		{
			return _name;
		}

		#endregion

		#endregion

		#region Public Property

		/// <summary>
		/// Gets or sets Element's Name.
		/// </summary>
		[Category("Definitions")]
		[Description("Gets or sets Element's Name.")]
		[XmlIgnore]
		[RefreshProperties(RefreshProperties.Repaint)]
		[Browsable(false)]
		[NLib.Design.PropertyOrder(0)]
		public virtual string Name
		{
			get { return _name; }
			set
			{
				RaiseEvent(NameChanging);
				_name = value;
				RaiseEvent(NameChanged);
			}
		}

		#endregion

		#region Public Event

		/// <summary>
		/// The NameChanging Event occur when Name Property about to changing.
		/// </summary>
		[Category("Definitions")]
		[Description("The NameChanging Event occur when Name Property about to changing.")]
		public event System.EventHandler NameChanging;
		/// <summary>
		/// The NameChanged Event occur when Name Property is changed.
		/// </summary>
		[Category("Definitions")]
		[Description("The NameChanged Event occur when Name Property is changed.")]
		public event System.EventHandler NameChanged;

		#endregion
	}

	#endregion

	#region ObjectElement CollectionBase (abstract)

	/// <summary>
	/// The ObjectElement CollectionBase (abstract).
	/// </summary>
	public abstract class ObjectElementCollectionBase : ObjectCollectionBase
	{
		#region Override

		/// <summary>
		/// After Inserted.
		/// </summary>
		/// <param name="index">The index of object in list.</param>
		/// <param name="value">The object that inserted.</param>
		protected override void AfterInserted(int index, object value)
		{
			base.AfterInserted(index, value);
		}
		/// <summary>
		/// After Removed.
		/// </summary>
		/// <param name="index">The index of object in list.</param>
		/// <param name="value">The object that Removed.</param>
		protected override void AfterRemoved(int index, object value)
		{
			base.AfterRemoved(index, value);
		}
		/// <summary>
		/// Gets Object Type.
		/// </summary>
		protected override Type ObjectType { get { return typeof(ObjectElement); } }
		/// <summary>
		/// Gets Property Name.
		/// </summary>
		protected override string PropertyName { get { return "Name"; } }
		/// <summary>
		/// Gets Property Changing Event Name.
		/// </summary>
		protected override string PropertyChangingEventName { get { return PropertyName + "Changing"; } }
		/// <summary>
		/// Gets Property Changed Event Name.
		/// </summary>
		protected override string PropertyChangedEventName { get { return PropertyName + "Changed"; } }

		#endregion
	}

	#endregion

	#endregion

	#region CheckableObjectElement and Collection base class

	#region Checkable Object Element (abstract)

	/// <summary>
	/// The Checkable Object Element (abstract).
	/// </summary>
	public abstract class CheckableObjectElement : ObjectElement
	{
		#region Internal Variable

		private bool _checked = false;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public CheckableObjectElement() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~CheckableObjectElement() { }

		#endregion

		#region Public Property

		/// <summary>
		/// Gets or sets is Checked.
		/// </summary>
		[Category("Definitions")]
		[Description("Gets or sets is Checked.")]
		[XmlAttribute]
		[RefreshProperties(RefreshProperties.Repaint)]
		[Browsable(true)]
		[NLib.Design.PropertyOrder(50)]
		public virtual bool Checked
		{
			get { return _checked; }
			set
			{
				RaiseEvent(CheckedChanging);
				_checked = value;
				RaiseEvent(CheckedChanged);
			}
		}

		#endregion

		#region Public Event

		/// <summary>
		/// The CheckedChanging Event occur when Checked Property about to changing.
		/// </summary>
		[Category("Definitions")]
		[Description("CheckedChanging Event occur when Checked Property about to changing.")]
		public event System.EventHandler CheckedChanging;
		/// <summary>
		/// The CheckedChanged Event occur when Checked Property is changed.
		/// </summary>
		[Category("Definitions")]
		[Description("CheckedChanged Event occur when Checked Property is changed.")]
		public event System.EventHandler CheckedChanged;

		#endregion
	}

	#endregion

	#region CheckableObjectElement CollectionBase (abstract)

	/// <summary>
	/// The CheckableObjectElement CollectionBase (abstract).
	/// </summary>
	public abstract class CheckableObjectElementCollectionBase : ObjectElementCollectionBase
	{
		#region Override

		/// <summary>
		/// On Insert Complete.
		/// </summary>
		/// <param name="index">The index of object in list.</param>
		/// <param name="value">The object that inserted.</param>
		protected override void OnInsertComplete(int index, object value)
		{
			if (null != value && value is CheckableObjectElement)
			{
				CheckableObjectElement val = (value as CheckableObjectElement);
				val.CheckedChanging += new EventHandler(val_CheckedChanging);
				val.CheckedChanged += new EventHandler(val_CheckedChanged);
			}
			base.OnInsertComplete(index, value);
			if (null != CollectionAdded) 
				CollectionAdded.Invoke(this, System.EventArgs.Empty);
		}
		/// <summary>
		/// On Remove Complete.
		/// </summary>
		/// <param name="index">The index of object in list</param>
		/// <param name="value">The object that Removed</param>
		protected override void OnRemoveComplete(int index, object value)
		{
			if (null != value && value is CheckableObjectElement)
			{
				CheckableObjectElement val = (value as CheckableObjectElement);
				val.CheckedChanging -= new EventHandler(val_CheckedChanging);
				val.CheckedChanged -= new EventHandler(val_CheckedChanged);
			}
			base.OnRemoveComplete(index, value);
			if (null != CollectionRemoved)
				CollectionRemoved.Invoke(this, System.EventArgs.Empty);
		}
		/// <summary>
		/// On Clear.
		/// </summary>
		protected override void OnClear()
		{
			foreach (CheckableObjectElement val in List)
			{
				val.CheckedChanging -= new EventHandler(val_CheckedChanging);
				val.CheckedChanged -= new EventHandler(val_CheckedChanged);
			}
			base.OnClear();
		}
		/// <summary>
		/// On Clear Complete
		/// </summary>
		protected override void OnClearComplete()
		{
			base.OnClearComplete();
			if (null != CollectionCleared)
				CollectionCleared.Invoke(this, System.EventArgs.Empty);
		}
		/// <summary>
		/// Gets Object Type.
		/// </summary>
		protected override Type ObjectType { get { return typeof(CheckableObjectElement); } }

		#endregion

		#region Element check changing and changed event handler

		private void val_CheckedChanging(object sender, EventArgs e)
		{
			if (null != ItemCheckedChanging) 
				ItemCheckedChanging.Invoke(this, System.EventArgs.Empty);
		}

		private void val_CheckedChanged(object sender, EventArgs e)
		{
			if (null != ItemCheckedChanged)
				ItemCheckedChanged.Invoke(this, System.EventArgs.Empty);
		}

		#endregion

		#region Public Method(s)

		#region Checked State

		/// <summary>
		/// Unchecked All items in list.
		/// </summary>
		public void UncheckedAll()
		{
			foreach (CheckableObjectElement obj in List)
			{
				obj.Checked = false;
			}
		}
		/// <summary>
		/// Checked All items in list.
		/// </summary>
		public void CheckedAll()
		{
			foreach (CheckableObjectElement obj in List)
			{
				obj.Checked = true;
			}
		}
		/// <summary>
		/// Gets Checked State.
		/// </summary>
		/// <param name="index">The index to get checked state.</param>
		/// <returns>The checked state if object found and false if object is not found.</returns>
		public bool GetChecked(int index)
		{
			object obj = List[index];
			if (null != obj && obj is CheckableObjectElement)
			{
				return (obj as CheckableObjectElement).Checked;
			}
			else return false;
		}
		/// <summary>
		/// Sets Checked State.
		/// </summary>
		/// <param name="index">The index to set checked state.</param>
		/// <param name="checkState">The new checked state.</param>
		public void SetChecked(int index, bool checkState)
		{
			object obj = List[index];
			if (null != obj && obj is CheckableObjectElement)
			{
				(obj as CheckableObjectElement).Checked = checkState;
			}
		}
		/// <summary>
		/// Gets Checked State by name.
		/// </summary>
		/// <param name="name">The Name to gets checked state</param>
		/// <returns>Returns checked state if object found and false if object is not found.</returns>
		public bool GetChecked(string name)
		{
			object obj = GetByKey(name);
			if (obj != null && obj is CheckableObjectElement)
			{
				return (obj as CheckableObjectElement).Checked;
			}
			else return false;
		}
		/// <summary>
		/// Sets Checked State by name.
		/// </summary>
		/// <param name="name">The Name to set checked state.</param>
		/// <param name="checkState">The new checked state.</param>
		public void SetChecked(string name, bool checkState)
		{
			object obj = GetByKey(name);
			if (null != obj && obj is CheckableObjectElement)
			{
				(obj as CheckableObjectElement).Checked = checkState;
			}
		}

		#endregion

		#endregion

		#region Events

		/// <summary>
		/// The ItemCheckedChanging event. Occur when Checked status is changing.
		/// </summary>
		[Category("Collections")]
		[Description("The ItemCheckedChanging event. Occur when Checked status is changing")]
		public event System.EventHandler ItemCheckedChanging;
		/// <summary>
		/// The ItemCheckedChanged event. Occur when Checked status is changed.
		/// </summary>
		[Category("Collections")]
		[Description("The ItemCheckedChanged event. Occur when Checked status is changed.")]
		public event System.EventHandler ItemCheckedChanged;
		/// <summary>
		/// The CollectionCleared event. Occur when Clear Completed.
		/// </summary>
		[Category("Collections")]
		[Description("The CollectionCleared event. Occur when Clear Completed.")]
		public event System.EventHandler CollectionCleared;
		/// <summary>
		/// The CollectionAdded event. Occur when item is added to collection.
		/// </summary>
		[Category("Collections")]
		[Description("The CollectionAdded event. Occur when item is added to collection.")]
		public event System.EventHandler CollectionAdded;
		/// <summary>
		/// The CollectionRemoved event. Occur when item is removed from collection.
		/// </summary>
		[Category("Collections")]
		[Description("The CollectionRemoved event. Occur when item is removed from collection.")]
		public event System.EventHandler CollectionRemoved;

		#endregion
	}

	#endregion

	#endregion
}

#endregion

#region Object Container Original version

namespace NLib.Collections
{
	#region ContainerRTTI

	/// <summary>
	/// The Container Runtime Type Information class.
	/// </summary>
	internal class ContainerRTTI
	{
		#region Internal Variable

		private System.Reflection.PropertyInfo[] _props;
		private IComparer _comparer = null;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Hide constructor.
		/// </summary>
		private ContainerRTTI() : base() { }
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="properties">The Array of property information.</param>
		public ContainerRTTI(System.Reflection.PropertyInfo[] properties)
			: base()
		{
			_props = properties;
			InitComparer();
		}
		/// <summary>
		/// Destructor.
		/// </summary>
		~ContainerRTTI()
		{
			_props = null;
			_comparer = null;
		}

		#endregion

		#region Private Method

		private void InitComparer()
		{
			MultiPropertyComparer cmp;
			cmp = new MultiPropertyComparer();

			cmp.UsedPropertyAccess = true;

			string[] props = (string[])Array.CreateInstance(typeof(string), _props.Length);
			for (int i = 0; i < _props.Length; i++) props[i] = _props[i].Name;
			cmp.Properties = props;
			_comparer = cmp;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Access Property Information.
		/// </summary>
		public PropertyInfo[] Properties { get { return _props; } }
		/// <summary>
		/// Gets Comparer's object.
		/// </summary>
		public IComparer ObjectComparer { get { return _comparer; } }

		#endregion
	}

	#endregion

	#region Reflecion Type Manager

	/// <summary>
	/// The Reflecion Type Manager.
	/// </summary>
	internal class __ReflecionManager
	{
		#region Internal Variable

		private Dictionary<Type, ContainerRTTI> _caches = new Dictionary<Type, ContainerRTTI>();

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public __ReflecionManager() : base() { }
		/// <summary>
		/// Destructor.
		/// </summary>
		~__ReflecionManager()
		{
			if (null != _caches)
				_caches.Clear();
			_caches = null;
		}

		#endregion

		#region Caches Access

		/// <summary>
		/// Gets ContainerRTTI by Type.
		/// </summary>
		public ContainerRTTI this[Type type]
		{
			get
			{
				if (!_caches.ContainsKey(type))
				{
					System.Reflection.PropertyInfo[] infos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty);
					ContainerRTTI info = new ContainerRTTI(infos);
					_caches.Add(type, info);
				}
				return (ContainerRTTI)_caches[type];
			}
		}

		#endregion
	}

	#endregion

	#region Container Cache Type Reflecion Manager

	/// <summary>
	/// The Container Cache Type Reflecion Manager.
	/// </summary>
	internal class ContainerTypeManager
	{
		#region Static Variable

		private static __ReflecionManager rm = new __ReflecionManager();

		#endregion

		#region Static Method

		/// <summary>
		/// Gets Runtime Type Information.
		/// </summary>
		/// <param name="type">The Type instance.</param>
		/// <returns>Returns ContainerRTTI instance that match Type instance.</returns>
		public static ContainerRTTI GetRTTI(Type type)
		{
			return rm[type];
		}

		#endregion
	}

	#endregion

	#region ObjectContainer PropertyDescriptor

	/// <summary>
	/// The ObjectContainer PropertyDescriptor.
	/// </summary>
	internal class ObjectContainerPropertyDescriptor : PropertyDescriptor
	{
		#region Internal Variable

		private string _propertyName = string.Empty;
		private Type _propertyType = null;
		private PropertyInfo _propInfo = null;
		private string _category = string.Empty;
		private string _description = string.Empty;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">The Property's Name.</param>
		/// <param name="propertyType">The Property's DataType.</param>
		/// <param name="category">The Property's Category.</param>
		/// <param name="description">The Property's Description.</param>
		/// <param name="attributes">The Array of property's attribute.</param>
		public ObjectContainerPropertyDescriptor(string propertyName, Type propertyType,
			string category, string description, Attribute[] attributes)
			: base(propertyName, attributes)
		{
			_propertyName = propertyName;
			_propertyType = propertyType;
			_category = category;
			_description = description;
		}

		#endregion

		#region Stub

		/// <summary>
		/// Can Reset Value.
		/// </summary>
		/// <param name="component">The target component.</param>
		/// <returns>Returns true if component can reset value.</returns>
		public override bool CanResetValue(object component) { return false; }
		/// <summary>
		/// Reset Value.
		/// </summary>
		/// <param name="component">The target component.</param>
		public override void ResetValue(object component) { }
		/// <summary>
		/// Is Read Only.
		/// </summary>
		public override bool IsReadOnly { get { return false; } }
		/// <summary>
		/// Should Serialize Value.
		/// </summary>
		/// <param name="component">The target component.</param>
		/// <returns>Returns true if the value should serialization.</returns>
		public override bool ShouldSerializeValue(object component) { return false; }
		/// <summary>
		///  When overridden in a derived class, gets the type of the component 
		///  this property is bound to. in this case is ObjectContainer.
		/// </summary>
		public override Type ComponentType { get { return typeof(ObjectContainer); } }

		#endregion

		#region Property Type

		/// <summary>
		/// Gets property type.
		/// </summary>
		public override Type PropertyType { get { return _propertyType; } }
		/// <summary>
		/// Gets Category.
		/// </summary>
		public override string Category
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_category)) 
					return base.Category;
				return _category;
			}
		}
		/// <summary>
		/// Gets Description.
		/// </summary>
		public override string Description
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_description)) 
					return base.Description;
				return _description;
			}
		}

		#endregion

		#region Get and Set Value

		/// <summary>
		/// Init Property Info.
		/// </summary>
		/// <param name="actualObject">The object to load property info.</param>
		private void InitPropertyInfo(object actualObject)
		{
			if (null == actualObject) 
				return;
			if (null == _propInfo)
			{
				BindingFlags binds = BindingFlags.Instance | BindingFlags.Public;
				_propInfo = actualObject.GetType().GetProperty(_propertyName, binds);
			}
		}
		/// <summary>
		/// Get Value.
		/// </summary>
		/// <param name="component">The target component.</param>
		/// <returns>The value to get from component's property.</returns>
		public override object GetValue(object component)
		{
			if (null == component) 
				return null;
			if (!(component is ObjectContainer)) 
				return null;
			ObjectContainer _Container = (component as ObjectContainer);
			if (null == _Container) 
				return null;

			if (_propertyName == "_No_" && _propertyType == typeof(int)) // _No_ only Property by design
			{
				// try to find property that related in ObjectContainer
				if (null == _propInfo) 
					InitPropertyInfo(_Container);
				if (null == _propInfo) 
					return null;
				// get value from container object
				try
				{
					if (_propInfo.CanRead)
					{
						return _propInfo.GetValue(_Container, Type.EmptyTypes);
					}
					else
					{
						return null;
					}
				}
				catch
				{
					return null;
				}
			}
			else
			{
				if (null == _Container.Data) 
					return null;
				if (null == _propInfo) 
					InitPropertyInfo(_Container.Data);
				if (null == _propInfo) 
					return null;
				// get value from internal object
				try
				{
					if (_propInfo.CanRead)
					{
						return _propInfo.GetValue(_Container.Data, Type.EmptyTypes);
					}
					else
					{
						return null;
					}
				}
				catch
				{
					return null;
				}
			}
		}
		/// <summary>
		/// Set Value.
		/// </summary>
		/// <param name="component">The target component.</param>
		/// <param name="value">The value to set to component's property.</param>
		public override void SetValue(object component, object value)
		{
			if (null == component) 
				return;
			if (!(component is ObjectContainer)) 
				return;
			ObjectContainer _Container = (component as ObjectContainer);
			if (null == _Container) 
				return;

			if (_propertyName == "_No_" && _propertyType == typeof(int)) // _No_ only Property by design
			{
				if (!_Container.IsLock)
					_Container.Backup(); // backup data first
				// try to find property that related in ObjectContainer
				if (null == _propInfo) 
					InitPropertyInfo(_Container);
				if (null == _propInfo) 
					return;
				// set value to container object
				try
				{
					if (_propInfo.CanWrite)
					{
						_propInfo.SetValue(_Container, value, Type.EmptyTypes);
						OnValueChanged(component, EventArgs.Empty);
					}
				}
				catch
				{
				}

			}
			else
			{
				if (null == _Container.Data) return;

				if (!_Container.IsLock)
					_Container.Backup(); // backup data first

				if (null == _propInfo) 
					InitPropertyInfo(_Container.Data);
				if (null == _propInfo) 
					return;
				// set value to internal object
				try
				{
					if (_propInfo.CanWrite)
					{
						_propInfo.SetValue(_Container.Data, value, Type.EmptyTypes);
						OnValueChanged(component, EventArgs.Empty);
					}
				}
				catch
				{
				}
			}
		}

		#endregion
	}

	#endregion

	#region ObjectContainer Class

	/// <summary>
	/// The Object Container.
	/// </summary>
	public class ObjectContainer : ICustomTypeDescriptor, IObjectContainer
	{
		#region Internal Class

		private class NotCopied
		{
			private static NotCopied _Value = new NotCopied();
			public static NotCopied Value { get { return _Value; } }
		}

		#endregion

		#region Internal Variable

		private object _temp = null;
		private object _object = null;
		private ObjectContainerCollection _Collection = null;
		private object[] _OriginalValues = null;
		private int iLock = 0;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Hide Constructor.
		/// </summary>
		private ObjectContainer() { }
		/// <summary>
		/// Default Constructor require data object.
		/// </summary>
		/// <param name="data">The data instance.</param>
		public ObjectContainer(object data)
		{
			if (data == null)
				throw new Exception("No Data Assigned.");
			_object = data;
		}
		/// <summary>
		/// Destructor.
		/// </summary>
		~ObjectContainer()
		{
			_object = null;
			_temp = null;
		}

		#endregion

		#region Helper Method(s)

		#region Internal

		/// <summary>
		/// Set Collection.
		/// not a contructor since it would fuck up derived classes.
		/// </summary>
		/// <param name="Collection"></param>
		internal void SetCollection(ObjectContainerCollection Collection)
		{
			_Collection = Collection;
		}
		/// <summary>
		/// Internal Set Data.
		/// </summary>
		/// <param name="data">The data instance.</param>
		internal void SetData(object data)
		{
			_object = data;
		}
		/// <summary>
		/// Protected to Access Collection.
		/// </summary>
		protected ObjectContainerCollection Collection
		{
			get { return _Collection; }
		}
		/// <summary>
		/// Checks Is Pending Insert.
		/// </summary>
		[Browsable(false)]
		public bool PendingInsert
		{
			get { return (_Collection._PendingInsert == this); }
		}
		/// <summary>
		/// Cheks Is Editting.
		/// </summary>
		[Browsable(false)]
		public bool IsEdit
		{
			get { return (null != _OriginalValues); }
		}

		#endregion

		#region Get Clone

		/// <summary>
		/// Gets Clone of Data.
		/// </summary>
		/// <returns>Returns clone data instance.</returns>
		protected virtual object GetClone()
		{
			if (null == _object) return null;
			object obj = System.Activator.CreateInstance(_object.GetType(), true);
			if (null != obj) 
				this.CloneData(_object, obj); // copy information
			return obj;
		}

		#endregion

		#region Clone Data

		/// <summary>
		/// Clone Source to Object.
		/// </summary>
		/// <param name="source">The Source Object.</param>
		/// <param name="dest">The Destination Object.</param>
		/// <returns>Returns true if clone success.</returns>
		protected bool CloneData(object source, object dest)
		{
			if (null == source || null == dest)
				return false;

			Type sourceType = source.GetType();
			Type destType = dest.GetType();
			if (sourceType != destType) 
				return false; // not same type

			// get all property
			PropertyInfo[] infos = ContainerTypeManager.GetRTTI(sourceType).Properties;

			int infoCount = infos.Length;
			PropertyInfo info;
			for (int i = 0; i < infoCount; i++)
			{
				try
				{
					info = infos[i];
					object val;

					val = info.GetValue(source, Type.EmptyTypes);

					if (null != val && info.PropertyType.IsEnum)
					{
						val = Enum.ToObject(info.PropertyType, val);
					}

					info.SetValue(dest, val, Type.EmptyTypes);
				}
				catch //(Exception ex)
				{
					// error ignore
					//Console.WriteLine(ex.Message);
				}
			}

			return true; // all completed
		}

		#endregion

		#region Has Changed

		/// <summary>
		/// Checks that object has been changed or not.
		/// </summary>
		/// <returns>true if data has been changed.</returns>
		public bool HasChanged()
		{
			if (null == _temp) 
				return false;
			return (ContainerTypeManager.GetRTTI(_object.GetType()).ObjectComparer.Compare(_object, _temp) != 0);
		}

		#endregion

		#region Backup

		/// <summary>
		/// Backup Data Object.
		/// </summary>
		protected internal void Backup()
		{
			if (null != _object && null == _temp)
			{
				_temp = GetClone(); // clone data
			}
		}

		#endregion

		#region RaisePropertyChanged

		/// <summary>
		/// Raise Property Changed.
		/// </summary>
		private void RaisePropertyChanged()
		{
			if (HasChanged())
			{
				if (null != OnPropertyChanged) 
					OnPropertyChanged.Invoke(this, System.EventArgs.Empty);
				if (null != _Collection)
				{
					_Collection.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Begin and End Init

		/// <summary>
		/// Call when data is initialize.
		/// </summary>
		public void BeginInit()
		{
			Lock();
			if (null != OnBeginInit) OnBeginInit.Invoke(this, System.EventArgs.Empty);
		}
		/// <summary>
		/// Call when finished initialize.
		/// </summary>
		public void EndInit()
		{
			Unlock();
			if (null != OnEndInit) OnEndInit.Invoke(this, System.EventArgs.Empty);
		}

		#endregion

		#region Accept and Reject

		/// <summary>
		/// Accept Changed made on object.
		/// </summary>
		public void Accept()
		{
			_temp = null; // clear temp
		}
		/// <summary>
		/// Reject Changed made on object.
		/// </summary>
		public void Reject()
		{
			if (null == _temp || null == _object) 
				return; // no changed occur

			this.Lock();
			CloneData(_temp, _object);
			this.Unlock();
		}

		#endregion

		#region Lock and Unlock

		/// <summary>
		/// Checks is Lock for Stop Event Raising.
		/// </summary>
		[System.ComponentModel.Bindable(false)]
		[Browsable(false)]
		public bool IsLock
		{
			get { return (iLock > 0); }
		}
		/// <summary>
		/// Lock to block Event Raising.
		/// </summary>
		public void Lock()
		{
			iLock++;
		}
		/// <summary>
		/// Unlock to allow Event Raising.
		/// </summary>
		public void Unlock()
		{
			iLock--;
			if (iLock < 0) iLock = 0;
		}

		#endregion

		#region Create Helper

		/// <summary>
		/// Create new object Container instance.
		/// </summary>
		/// <param name="data">The actual object instance.</param>
		/// <returns>Returns ObjectContainer instance that wrap around actual object(or data).</returns>
		public static ObjectContainer Create(object data)
		{
			if (null == data) 
				return null;
			return new ObjectContainer(data);
		}
		/// <summary>
		/// Create new object Container array instance.
		/// </summary>
		/// <param name="datatype">The DataType to create blank instance</param>
		/// <param name="size">The size of array</param>
		/// <returns>Returns Array of ObjectContainer with initialize instance of object by specificed type (blank instance).
		/// note that the specificed data type should at least contains default constructor</returns>
		public static ObjectContainer[] CreateArray(Type datatype, int size)
		{
			if (null == datatype) 
				return null;
			if (size <= 0) return new ObjectContainer[] { }; // empty array

			object data;

			ObjectContainer[] results = 
				(ObjectContainer[])Array.CreateInstance(typeof(ObjectContainer), size);
			for (int i = 0; i < size; i++)
			{
				data = Activator.CreateInstance(datatype, true);
				results[i] = new ObjectContainer(data);
			}

			return results;
		}

		#endregion

		#endregion

		#region Fill Method

		/// <summary>
		/// Internal Fill method for speed increment.
		/// </summary>
		internal void Fill(DataRow row, System.Reflection.PropertyInfo[] infos)
		{
			if (null == row) 
				return;
			this.Lock();
			int infoCount = infos.Length;
			System.Reflection.PropertyInfo info;

			for (int i = 0; i < infoCount; i++)
			{
				try
				{
					info = infos[i];
					object val = row[info.Name];
					if (val == DBNull.Value) // check for speed
						continue; // require null conversion here
					if (info.PropertyType.IsEnum)
					{
						val = Enum.ToObject(info.PropertyType, val);
					}
					info.SetValue(_object, val, Type.EmptyTypes);
				}
				catch
				{
					// error ignore
				}
			}
			this.Unlock();
		}
		/// <summary>
		/// Internal Fill method for speed increment.
		/// </summary>
		internal void Fill(IDataReader reader, System.Reflection.PropertyInfo[] infos, int[] flds)
		{
			if (null == reader) 
				return;
			this.Lock();
			int infoCount = infos.Length;
			System.Reflection.PropertyInfo info;
			for (int i = 0; i < infoCount; i++)
			{
				try
				{
					info = infos[i];
					object val = reader.GetValue(flds[i]); // method call little bit faster
					//object val = reader[flds[i]]; // indexer
					if (val == DBNull.Value) // check for speed
						continue; // require null conversion here
					if (info.PropertyType.IsEnum)
					{
						val = Enum.ToObject(info.PropertyType, val);
					}
					info.SetValue(_object, val, Type.EmptyTypes);
				}
				catch
				{
					// error ignore
				}
			}
			this.Unlock();
		}
		/// <summary>
		/// Fill Object with data in DataRow Object.
		/// </summary>
		/// <param name="row">The Source DataRow instance.</param>
		public virtual void Fill(DataRow row)
		{
			if (null == row) 
				return;

			this.Lock();
			Type objType = _object.GetType();
			System.Reflection.PropertyInfo[] infos = 
				ContainerTypeManager.GetRTTI(objType).Properties;
			int infoCount = infos.Length;
			System.Reflection.PropertyInfo info;
			for (int i = 0; i < infoCount; i++)
			{
				try
				{
					info = infos[i];
					object val = row[info.Name];
					if (val == DBNull.Value) // check for speed
						continue; // require null conversion here
					if (info.PropertyType.IsEnum)
					{
						val = Enum.ToObject(info.PropertyType, val);
					}
					info.SetValue(_object, val, Type.EmptyTypes);
				}
				catch
				{
					// error ignore
				}
			}
			this.Unlock();
		}
		/// <summary>
		/// Fill Object with data in DataRowView Object.
		/// </summary>
		/// <param name="row">The Source DataRowView.</param>
		public virtual void Fill(DataRowView row)
		{
			if (null == row) return;
			this.Lock();
			Type objType = _object.GetType();
			System.Reflection.PropertyInfo[] infos = ContainerTypeManager.GetRTTI(objType).Properties;
			int infoCount = infos.Length;
			System.Reflection.PropertyInfo info;
			for (int i = 0; i < infoCount; i++)
			{
				try
				{
					info = infos[i];
					object val = row[info.Name];
					if (val == DBNull.Value) // check for speed
						continue; // require null conversion here
					if (val != null && info.PropertyType.IsEnum)
					{
						val = Enum.ToObject(info.PropertyType, val);
					}
					info.SetValue(_object, val, Type.EmptyTypes);
				}
				catch
				{
					// error ignore
				}
			}
			this.Unlock();
		}

		#endregion

		#region ICustomTypeDescriptor Members

		#region Stub

		/// <summary>
		/// ICustomTypeDescriptor.GetConverter.
		/// </summary>
		/// <returns>alway return null</returns>
		TypeConverter ICustomTypeDescriptor.GetConverter() { return null; }
		/// <summary>
		/// ICustomTypeDescriptor.GetComponentName.
		/// </summary>
		/// <returns>alway return null.</returns>
		string ICustomTypeDescriptor.GetComponentName() { return null; }
		/// <summary>
		/// ICustomTypeDescriptor.GetClassName.
		/// </summary>
		/// <returns>alway return null.</returns>
		string ICustomTypeDescriptor.GetClassName() { return null; }
		/// <summary>
		/// ICustomTypeDescriptor.GetAttributes.
		/// </summary>
		/// <returns>Empty Attribute Collection.</returns>
		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return new AttributeCollection(null);
		}
		/// <summary>
		/// ICustomTypeDescriptor.GetDefaultProperty.
		/// </summary>
		/// <returns>alway return null</returns>
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return null; }
		/// <summary>
		/// ICustomTypeDescriptor.GetDefaultEvent.
		/// </summary>
		/// <returns>alway return null</returns>
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return null; }
		/// <summary>
		/// ICustomTypeDescriptor.GetEvents.
		/// </summary>
		/// <param name="attributes">Array of attribute.</param>
		/// <returns>Empty EventDescriptor's Collection.</returns>
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) { return new EventDescriptorCollection(null); }
		/// <summary>
		/// ICustomTypeDescriptor.GetEvents
		/// </summary>
		/// <returns>Empty EventDescriptor's Collection</returns>
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { return new EventDescriptorCollection(null); }
		/// <summary>
		/// ICustomTypeDescriptor.GetEditor.
		/// </summary>
		/// <param name="editorBaseType"></param>
		/// <returns>alway return null</returns>
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return null; }
		/// <summary>
		/// ICustomTypeDescriptor.GetPropertyOwner.
		/// </summary>
		/// <param name="pd">Property Descriptor instance.</param>
		/// <returns>return this instance.</returns>
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) { return this; }
		/// <summary>
		/// ICustomTypeDescriptor.GetProperties.
		/// </summary>
		/// <returns>PropertyDescriptor's Collection.</returns>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() { return ((ICustomTypeDescriptor)this).GetProperties(null); }

		#endregion

		#region GetProperties

		/// <summary>
		/// ICustomTypeDescriptor.GetProperties.
		/// </summary>
		/// <param name="attributes">Array of attribute.</param>
		/// <returns>PropertyDescriptor Collection instance.</returns>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection props;
			// Create the property collection and filter
			props = new PropertyDescriptorCollection(null);

			// for display in property grid
			props.Add(new ObjectContainerPropertyDescriptor("_No_", typeof(int), "Information", "Get Number in list", attributes));

			PropertyDescriptorCollection sourceProps = TypeDescriptor.GetProperties(_object, attributes, true);
			foreach (PropertyDescriptor prop in sourceProps)
			{
				props.Add(new ObjectContainerPropertyDescriptor(prop.Name, prop.PropertyType, prop.Category, prop.Description, attributes));
			}

			return props;
		}

		#endregion

		#endregion

		#region IEditableObject implements

		/// <summary>
		/// IEditableObject.BeginEdit().
		/// </summary>
		void IEditableObject.BeginEdit()
		{
			if (!IsEdit)
			{
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(_object, null);
				object[] vals = new object[props.Count];
				for (int i = 0; i < props.Count; ++i)
				{
					vals[i] = NotCopied.Value;
					PropertyDescriptor desc = props[i];
					if (desc.PropertyType.IsSubclassOf(typeof(ValueType)))
					{
						vals[i] = desc.GetValue(_object);
					}
					else
					{
						object val = desc.GetValue(_object);
						if (null == val) vals[i] = null;
						else if (val is IList) { /* if IList, then no copy */ }
						else if (val is ICloneable) vals[i] = ((ICloneable)val).Clone();
					}
				}
				_OriginalValues = vals;
			}
		}
		/// <summary>
		/// IEditableObject.CancelEdit().
		/// </summary>
		void IEditableObject.CancelEdit()
		{
			if (IsEdit)
			{
				if (PendingInsert) ((IList)_Collection).Remove(this);
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(_object, null);
				for (int i = 0; i < props.Count; ++i)
				{
					if (_OriginalValues[i] is NotCopied) continue;
					props[i].SetValue(_object, _OriginalValues[i]);
				}
				_OriginalValues = null;
				RaisePropertyChanged();
			}
		}
		/// <summary>
		/// IEditableObject.EndEdit().
		/// </summary>
		void IEditableObject.EndEdit()
		{
			if (IsEdit)
			{
				if (PendingInsert) _Collection._PendingInsert = null;
				_OriginalValues = null;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region ICloneable Members

		/// <summary>
		/// Clone Object.
		/// </summary>
		/// <returns>Returns new instance that clone from this object</returns>
		public object Clone()
		{
			object cloneData = GetClone();
			if (null != cloneData) 
				return new ObjectContainer(cloneData);
			else return null;
		}

		#endregion

		#region Override

		/// <summary>
		/// Equals.
		/// </summary>
		/// <param name="obj">object instance to check equal.</param>
		/// <returns>See Object.Equals.</returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
		/// <summary>
		/// GetHashCode.
		/// </summary>
		/// <returns>See Object.GetHashCode.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		/// <summary>
		/// ToString.
		/// </summary>
		/// <returns>See Object.ToString.</returns>
		public override string ToString()
		{
			if (null == _object)
				return base.ToString();
			else return _object.ToString();
		}

		#endregion

		#region Public Property

		/// <summary>
		/// Access Data object (Readonly).
		/// </summary>
		[Browsable(false)]
		public object Data { get { return _object; } }
		/// <summary>
		/// Gets or sets No. or Object Index.
		/// </summary>
		[Browsable(false)]
		public int _No_
		{
			get
			{
				if (null == _Collection) return 0;
				else return _Collection.IndexOf(this) + 1;
			}
		}

		#endregion

		#region Public Events

		/// <summary>
		/// OnBeginInit event. Raise when BeginInit() is called.
		/// </summary>
		[Category("Init")]
		public event System.EventHandler OnBeginInit;
		/// <summary>
		/// OnEndInit event. Raise when EndInit() is called.
		/// </summary>
		[Category("Init")]
		public event System.EventHandler OnEndInit;
		/// <summary>
		/// OnPropertyChanged event. Raise when Object's Property is changed
		/// </summary>
		[Category("Changed")]
		public event System.EventHandler OnPropertyChanged;

		#endregion
	}

	#endregion

	#region ObjectContainer Collection Class

	/// <summary>
	/// The Object Container Collection Binding List.
	/// </summary>
	[ListBindable(false)]
	public class ObjectContainerCollection : IObjectContainerCollection,
		IBindingList, ITypedList
	{
		#region Internal Variable

		private ArrayList _List = null;
		internal object _PendingInsert = null;
		private Type _baseType = null;
		private ArrayList _removedList = new ArrayList();

		private bool _editable = true;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		protected ObjectContainerCollection()
			: base()
		{
			GC.Collect(0);
			GC.WaitForPendingFinalizers();

			_List = new ArrayList();
			_PendingInsert = null;
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="baseType">The base element type.</param>
		public ObjectContainerCollection(Type baseType)
			: this(baseType, null, false)
		{
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="baseType">The base element type.</param>
		/// <param name="table">The DataTable instance.</param>
		public ObjectContainerCollection(Type baseType, DataTable table)
			: this(baseType, null, false)
		{
			if (null != table && table.Rows.Count > 0)
			{
				PropertyInfo[] infos = baseType.GetProperties();
				ObjectContainer[] results = ObjectContainer.CreateArray(baseType, table.Rows.Count);
				int i = 0;
				foreach (DataRow row in table.Rows)
				{
					results[i].Fill(row, infos);
					i++;
				}
				this.AddRangeInternal(results);
			}
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="baseType">The base element type.</param>
		/// <param name="reader">The IDataReader instance.</param>
		public ObjectContainerCollection(Type baseType, IDataReader reader)
			: this(baseType, null, false)
		{
			if (null != reader && null != baseType)
			{
				PropertyInfo[] infos = baseType.GetProperties();
				PropertyInfo info;
				int propCnt = infos.Length;

				int[] flds = (int[])Array.CreateInstance(typeof(int), reader.FieldCount);
				bool firstLoop = true;
				ArrayList list = new ArrayList();

				object obj;
				while (reader.Read())
				{
					// create
					obj = System.Activator.CreateInstance(baseType, Type.EmptyTypes);

					if (firstLoop)
					{
						for (int j = 0; j < propCnt; j++)
						{
							info = infos[j];
							flds[j] = reader.GetOrdinal(info.Name);
						}
					}
					firstLoop = false;
					// call for internal hacking method for speed purpose
					ObjectContainer result = ObjectContainer.Create(obj);
					result.Fill(reader, infos, flds);
					list.Add(result);
				}

				this.AddRangeInternal((ObjectContainer[])list.ToArray(typeof(ObjectContainer)));

				if (list != null) list.Clear();
				list = null;
			}
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="baseType">The base element type.</param>
		/// <param name="values">The ICollection instance.</param>
		/// <param name="force">The Reserved for future used. alway false.</param>
		public ObjectContainerCollection(Type baseType, ICollection values, bool force)
			: this()
		{
			_baseType = baseType;
			if (null == values || values.Count <= 0) return;
			else
			{
				ObjectContainer[] Containers = (ObjectContainer[])Array.CreateInstance(typeof(ObjectContainer), values.Count);
				int i = 0;
				foreach (object val in values)
				{
					Containers[i] = new ObjectContainer(val);
					Containers[i].SetCollection(this);
					i++;
				}
				_List.AddRange(Containers);
			}
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="baseType">The base element type.</param>
		/// <param name="values">The ICollection instance.</param>
		public ObjectContainerCollection(Type baseType, ICollection values)
			: this(baseType, values, false)
		{
		}
		/// <summary>
		/// Destructor.
		/// </summary>
		~ObjectContainerCollection()
		{
			if (null != _List) _List.Clear();
			_List = null;

			if (null != _removedList) _removedList.Clear();
			_removedList = null;

			_PendingInsert = null;
			_baseType = null;
		}

		#endregion

		#region Removed List related Method

		/// <summary>
		/// Add To Remove List.
		/// </summary>
		/// <param name="value">The value that remove from collection.</param>
		private void AddToRemoveList(object value)
		{
			object item = null;
			if (value is ObjectContainer)
			{
				ObjectContainer container = (value as ObjectContainer);
				if (_PendingInsert != container)
					item = container.Data; // read data from container
			}
			else if (value.GetType() == _baseType)
			{
				if (_PendingInsert != null && (_PendingInsert is ObjectContainer) &&
					(_PendingInsert as ObjectContainer).Data != value)
					item = value; // actual item
			}
			if (item != null) _removedList.Add(item);
		}
		/// <summary>
		/// Checks that has removed item in list.
		/// </summary>
		public bool HasRemovedItem
		{
			get
			{
				if (null == _removedList) 
					return false;
				return (_removedList.Count > 0);
			}
		}
		/// <summary>
		/// Gets Removed Objects.
		/// </summary>
		/// <returns>Returns array of object that removed from collection.</returns>
		public object[] GetRemovedObjects()
		{
			if (null == _removedList || _removedList.Count <= 0)
				return null;

			if (null == this._baseType)
				return _removedList.ToArray();
			else
				return (object[])_removedList.ToArray(this._baseType);
		}
		/// <summary>
		/// Clear all object from removed list.
		/// </summary>
		public void ClearRemovedList()
		{
			if (null != _removedList) _removedList.Clear();
		}

		#endregion

		#region Public Access

		/// <summary>
		/// Gets the number of elememts in list.
		/// </summary>
		public int Count
		{
			get { return _List.Count; }
		}
		/// <summary>
		/// Clear all element in list and all data in removed list.
		/// </summary>
		public void Clear()
		{
			OnClear();
			if (null != _List)
			{
				for (int i = 0; i < _List.Count; ++i) ((ObjectContainer)_List[i]).SetCollection(null);

				if (null != _List) _List.Clear();
				if (null != _removedList) _removedList.Clear();

				_PendingInsert = null;
				OnClearComplete();
				if (null != _ListChanged) 
					_ListChanged.Invoke(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
			}
		}
		/// <summary>
		/// Remove Element From List via index.
		/// </summary>
		/// <param name="index">The index to remove.</param>
		public void RemoveAt(int index)
		{
			if (index < 0 || index >= _List.Count) 
				throw new ArgumentOutOfRangeException();
			object item = _List[index];
			OnValidate(item);
			OnRemove(index, item);
			((ObjectContainer)_List[index]).SetCollection(null);
			if (_PendingInsert == item) _PendingInsert = null;
			_List.RemoveAt(index);
			OnRemoveComplete(index, item);

			// Append to Remove List
			if (null != item) AddToRemoveList(item);

			if (null != _ListChanged)
			{
				try
				{
					_ListChanged.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
				}
				catch (Exception ex)
				{
					throw (ex);
				}
			}
		}
		/// <summary>
		/// Get Enumerator.
		/// </summary>
		/// <returns>IEnumerator object.</returns>
		public IEnumerator GetEnumerator()
		{
			return _List.GetEnumerator();
		}
		/// <summary>
		/// Add Object Container.
		/// </summary>
		/// <param name="value">The object to add.</param>
		public void Add(ObjectContainer value)
		{
			if (null == value) return;
			ObjectContainer[] values = new ObjectContainer[] { value };
			AddRange(values);
		}
		/// <summary>
		/// Add Object Container Array.
		/// </summary>
		/// <param name="values">The Object Container's array.</param>
		public void AddRange(ObjectContainer[] values)
		{
			if (null == values) return;
			AddRangeInternal(values);

			if (null != _ListChanged)
			{
				try
				{
					ListChangedEventArgs evt = new ListChangedEventArgs(ListChangedType.ItemAdded, -1);
					_ListChanged.Invoke(this, evt);
				}
				catch (Exception ex)
				{
					throw (ex);
				}
			}
		}

		#endregion

		#region Protected Access

		/// <summary>
		/// helper method for add range via derived class.
		/// </summary>
		/// <param name="c">The Object Container's array</param>
		protected void AddRangeInternal(ObjectContainer[] c)
		{
			if (null == c) 
				return;
			_List.AddRange(c);
			foreach (ObjectContainer obj in c) obj.SetCollection(this);
		}
		/// <summary>
		/// helper method for add range via derived class.
		/// </summary>
		/// <param name="c">The IList instance to add.</param>
		protected void AddRangeInternal(IList c)
		{
			if (null == c) 
				return;
			ObjectContainer[] Containers = (ObjectContainer[])Array.CreateInstance(typeof(ObjectContainer), c.Count);
			int i = 0;
			foreach (object val in c)
			{
				Containers[i] = new ObjectContainer(val);
				Containers[i].SetCollection(this);
				i++;
			}
			_List.AddRange(Containers);
		}
		/// <summary>
		/// There is no InnerList since using it would cease firing events.
		/// </summary>
		protected IList List
		{
			get { return this; }
		}
		/// <summary>
		/// Gets Element Type override to get the correct type.
		/// </summary>
		protected virtual Type ElementType
		{
			get { return typeof(object); }
		}
		/// <summary>
		/// Create New Container.
		/// </summary>
		/// <param name="data">The data instance.</param>
		/// <returns>Returns instance of ObjectContainer.</returns>
		protected ObjectContainer CreateNewContainer(object data)
		{
			ObjectContainer Container = new ObjectContainer(data);
			Container.SetCollection(this);
			return Container;
		}
		/// <summary>
		/// Create Instance. Override if the default constructor is not suitable.
		/// </summary>
		/// <returns>Returns new object instance.</returns>
		protected virtual object CreateInstance()
		{
			if (this._baseType == null) return null;
			return Activator.CreateInstance(this._baseType, true);
		}

		#endregion

		#region IObjectContainerCollection Implements

		/// <summary>
		/// Gets Removed Items.
		/// </summary>
		/// <returns>Returns array of item that removed from collection.</returns>
		object[] IObjectContainerCollection.GetRemoveItems()
		{
			return GetRemovedObjects();
		}
		/// <summary>
		/// Clear all remove items.
		/// </summary>
		void IObjectContainerCollection.ClearRemoveItems()
		{
			ClearRemovedList();
		}
		/// <summary>
		/// Check is the collection is in pending state.
		/// </summary>
		bool IObjectContainerCollection.IsPending
		{
			get { return (_PendingInsert != null); }
		}
		/// <summary>
		/// Gets Current Pending object.
		/// </summary>
		/// <returns>Returns the instance of IObjectContainer.</returns>
		IObjectContainer IObjectContainerCollection.GetPendingObject()
		{
			if (null != _PendingInsert && _PendingInsert is IObjectContainer)
				return (IObjectContainer)_PendingInsert;
			else return null;
		}
		/// <summary>
		/// Create new object and set as pending item.
		/// </summary>
		/// <returns>Returns new object.</returns>
		IObjectContainer IObjectContainerCollection.AddNew()
		{
			object val = (this as IBindingList).AddNew();
			if (null != val && val is IObjectContainer)
			{
				return (val as IObjectContainer);
			}
			else return null;
		}
		/// <summary>
		/// Is allow edit item.
		/// </summary>
		bool IObjectContainerCollection.AllowEdit { get { return _editable; } }
		/// <summary>
		/// Is allow add new.
		/// </summary>
		bool IObjectContainerCollection.AllowNew { get { return _editable; } }
		/// <summary>
		/// Is allow remove.
		/// </summary>
		bool IObjectContainerCollection.AllowRemove { get { return _editable; } }

		#endregion

		#region IBindingList Implements

		/// <summary>
		/// Gets IBindingList.AllowEdit.
		/// </summary>
		bool IBindingList.AllowEdit
		{
			get { return _editable; }
		}
		/// <summary>
		/// Get IBindingList.AllowNew.
		/// </summary>
		bool IBindingList.AllowNew
		{
			get { return _editable; }
		}
		/// <summary>
		/// Get IBindingList.AllowRemove .
		/// </summary>
		bool IBindingList.AllowRemove
		{
			get { return _editable; }
		}
		/// <summary>
		/// Get IBindingList.SupportsChangeNotification.
		/// </summary>
		bool IBindingList.SupportsChangeNotification
		{
			get { return true; }
		}
		/// <summary>
		/// IBindingList.AddNew.
		/// </summary>
		/// <returns>Returns new object instance.</returns>
		object IBindingList.AddNew()
		{
			if (null != _PendingInsert) ((IEditableObject)_PendingInsert).CancelEdit();

			object item;
			object data = CreateInstance();
			if (null != data) item = CreateNewContainer(data);
			else item = null;

			if (null != item)
			{
				((IList)this).Add(item);
				_PendingInsert = item;
			}
			return item;
		}

		private ListChangedEventHandler _ListChanged;

		/// <summary>
		/// IBindingList.ListChanged event.
		/// </summary>
		event ListChangedEventHandler IBindingList.ListChanged
		{
			add { _ListChanged += value; }
			remove { _ListChanged -= value; }
		}
		/// <summary>
		/// IBindingList.SupportsSearching.
		/// </summary>
		bool IBindingList.SupportsSearching
		{
			get { return false; }
		}
		/// <summary>
		/// IBindingList.SupportsSorting.
		/// </summary>
		bool IBindingList.SupportsSorting
		{
			get { return false; }
		}
		/// <summary>
		/// IBindingList.IsSorted. 
		/// </summary>
		bool IBindingList.IsSorted
		{
			get { return false; }
		}
		/// <summary>
		/// IBindingList.SortDirection. 
		/// </summary>
		ListSortDirection IBindingList.SortDirection
		{
			get { throw new NotSupportedException(); }
		}
		/// <summary>
		/// IBindingList.SortProperty. 
		/// </summary>
		PropertyDescriptor IBindingList.SortProperty
		{
			get { throw new NotSupportedException(); }
		}
		/// <summary>
		/// IBindingList.AddIndex.
		/// </summary>
		/// <param name="property">Property Descriptor instance</param>
		void IBindingList.AddIndex(PropertyDescriptor property)
		{
			throw new NotSupportedException();
		}
		/// <summary>
		/// IBindingList.ApplySort.
		/// </summary>
		/// <param name="property">Property Descriptor instance.</param>
		/// <param name="direction">Sort direction.</param>
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}
		/// <summary>
		/// IBindingList.Find.
		/// </summary>
		/// <param name="property">Property Descriptor instance.</param>
		/// <param name="key">object that used as key.</param>
		/// <returns>index that match by specificed key.</returns>
		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}
		/// <summary>
		/// IBindingList.RemoveIndex.
		/// </summary>
		/// <param name="property">Property Descriptor instance.</param>
		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
			throw new NotSupportedException();
		}
		/// <summary>
		/// IBindingList.RemoveSort.
		/// </summary>
		void IBindingList.RemoveSort()
		{
			throw new NotSupportedException();
		}

		#endregion

		#region ICollection Implements

		/// <summary>
		/// ICollection.CopyTo.
		/// </summary>
		/// <param name="array">destination array.</param>
		/// <param name="index">index to copy.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			_List.CopyTo(array, index);
		}
		/// <summary>
		/// ICollection.IsSynchronized.
		/// </summary>
		bool ICollection.IsSynchronized
		{
			get { return _List.IsSynchronized; }
		}
		/// <summary>
		/// ICollection.SyncRoot. 
		/// </summary>
		object ICollection.SyncRoot
		{
			get { return _List.SyncRoot; }
		}

		#endregion

		#region IList Implements

		/// <summary>
		/// IList.Add.
		/// </summary>
		/// <param name="value">object to add.</param>
		/// <returns>index of added object.</returns>
		int IList.Add(object value)
		{
			OnValidate(value);
			OnInsert(_List.Count, value);
			int index = _List.Add(value);
			try
			{
				OnInsertComplete(index, value);
			}
			catch
			{
				_List.RemoveAt(index);
				throw;
			}
			if (value is ObjectContainer) ((ObjectContainer)value).SetCollection(this);
			if (_ListChanged != null)
			{
				try
				{
					ListChangedEventArgs evt = new ListChangedEventArgs(ListChangedType.ItemAdded, index);
					_ListChanged(this, evt);
				}
				catch (Exception ex)
				{
					throw (ex);
				}
			}
			return index;
		}
		/// <summary>
		/// Indexer Access.
		/// </summary>
		public object this[int index]
		{
			get
			{
				if (index < 0 || index >= _List.Count)
				{
					//throw new ArgumentOutOfRangeException();
					return null;
				}
				return _List[index];
			}
			set
			{
				if (index < 0 || index >= _List.Count)
				{
					//throw new ArgumentOutOfRangeException();
					return;
				}
				OnValidate(value);
				object item = _List[index];
				OnSet(index, item, value);
				_List[index] = value;
				try
				{
					OnSetComplete(index, item, value);
				}
				catch
				{
					_List[index] = item;
					throw;
				}
				if (_ListChanged != null)
				{
					try
					{
						_ListChanged(this, new ListChangedEventArgs(ListChangedType.ItemChanged, index));
					}
					catch (Exception ex)
					{
						throw (ex);
					}
				}
			}
		}
		/// <summary>
		/// Contains.
		/// </summary>
		/// <param name="value">object to check containment</param>
		/// <returns>true if object is already exists on collection</returns>
		public bool Contains(object value)
		{
			return _List.Contains(value);
		}
		/// <summary>
		/// IList.IsFixedSize
		/// </summary>
		bool IList.IsFixedSize
		{
			get { return _List.IsFixedSize; }
		}
		/// <summary>
		/// IList.IsReadOnly 
		/// </summary>
		bool IList.IsReadOnly
		{
			get { return !_editable; }
		}
		/// <summary>
		/// Index Of
		/// </summary>
		/// <param name="value">object to find index</param>
		/// <returns>index of specificed object</returns>
		public int IndexOf(object value)
		{
			return _List.IndexOf(value);
		}
		/// <summary>
		/// IList.Insert
		/// </summary>
		/// <param name="index">index to insert</param>
		/// <param name="value">object to inserted collection</param>
		void IList.Insert(int index, object value)
		{
			if (index < 0 || index > _List.Count) throw new ArgumentOutOfRangeException();
			OnValidate(value);
			OnInsert(index, value);
			_List.Insert(index, value);
			try
			{
				OnInsertComplete(index, value);
			}
			catch
			{
				_List.RemoveAt(index);
				throw;
			}
			if (value is ObjectContainer) ((ObjectContainer)value).SetCollection(this);
			if (_ListChanged != null)
			{
				try
				{
					_ListChanged(this, new ListChangedEventArgs(ListChangedType.ItemAdded, index));
				}
				catch (Exception ex)
				{
					throw (ex);
				}
			}
		}
		/// <summary>
		/// Remove
		/// </summary>
		/// <param name="value">object to remove from collection</param>
		public void Remove(object value)
		{
			OnValidate(value);
			int index = _List.IndexOf(value);
			if (index < 0) throw new ArgumentException();
			OnRemove(index, value);
			_List.RemoveAt(index);
			OnRemoveComplete(index, value);

			// Append to Remove List
			if (value != null) AddToRemoveList(value);

			if (_ListChanged != null)
			{
				try
				{
					_ListChanged(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
				}
				catch (Exception ex)
				{
					throw (ex);
				}
			}
		}

		#endregion

		#region ITypedList Members

		/// <summary>
		/// ITypedList.GetItemProperties
		/// </summary>
		/// <param name="listAccessors">Property Descriptor's array</param>
		/// <returns>Property Descriptor's collection</returns>
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			/*
			// sample implement for ListSource
			
			if(listAccessors == null || listAccessors.Length == 0)
				return ((ICustomTypeDescriptor)_Tables).GetProperties(); // get Tables
			Type elementtype = _Tables.GetElementType(listAccessors);
			if(elementtype == null)
				return new PropertyDescriptorCollection(null);
			return TypeDescriptor.GetProperties(elementtype); // get Table columns
			
			*/
			PropertyDescriptorCollection props;
			// Create the property collection and filter
			props = new PropertyDescriptorCollection(null);

			// for display in data grid
			props.Add(new ObjectContainerPropertyDescriptor("_No_", typeof(int), "Information", "Get Number in list", null));

			ArrayList list = new ArrayList();
			PropertyDescriptorCollection sourceProps = TypeDescriptor.GetProperties(_baseType);
			foreach (PropertyDescriptor prop in sourceProps)
			{
				list.Clear();
				if (prop.Attributes != null && prop.Attributes.Count > 0)
				{
					list.AddRange(prop.Attributes);
					props.Add(new ObjectContainerPropertyDescriptor(prop.Name,
						prop.PropertyType, prop.Category, prop.Description,
						(Attribute[])list.ToArray(typeof(Attribute))));
				}
				else
				{
					props.Add(new ObjectContainerPropertyDescriptor(prop.Name,
						prop.PropertyType, prop.Category, prop.Description, null));
				}
			}
			list.Clear();
			list = null;

			return props;
		}
		/// <summary>
		/// ITypedList.GetListName
		/// </summary>
		/// <param name="listAccessors">Property Descriptor's array</param>
		/// <returns>List name</returns>
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors == null || listAccessors.Length == 0)
				return "JOE"; // nevermind, never queryed
			if (_baseType == null)
				return "";
			return listAccessors[listAccessors.Length - 1].Name; // works as TableName
		}

		#endregion

		#region overridable notifications

		/// <summary>
		/// overridable notifications
		/// </summary>
		protected virtual void OnClear()
		{
		}
		/// <summary>
		/// overridable notifications
		/// </summary>
		protected virtual void OnClearComplete()
		{
			RaiseCollectionChanged();
		}
		/// <summary>
		/// overridable notifications
		/// </summary>
		/// <param name="index">index of inserted object</param>
		/// <param name="value">object value to inserted</param>
		protected virtual void OnInsert(int index, object value)
		{
		}
		/// <summary>
		/// overridable notifications
		/// </summary>
		/// <param name="index">index of inserted object</param>
		/// <param name="value">object value to inserted</param>
		protected virtual void OnInsertComplete(int index, object value)
		{
			RaiseCollectionChanged();
		}
		/// <summary>
		/// overridable notifications
		/// </summary>
		/// <param name="index">index of removed object</param>
		/// <param name="value">object value to removed</param>
		protected virtual void OnRemove(int index, object value)
		{
		}
		/// <summary>
		/// overridable notifications
		/// </summary>
		/// <param name="index">index of removed object</param>
		/// <param name="value">object value to removed</param>
		protected virtual void OnRemoveComplete(int index, object value)
		{
			RaiseCollectionChanged();
		}
		/// <summary>
		/// overridable notifications
		/// </summary>
		/// <param name="index">index of set object</param>
		/// <param name="oldValue">old object value</param>
		/// <param name="newValue">new object value</param>
		protected virtual void OnSet(int index, object oldValue, object newValue)
		{
		}
		/// <summary>
		/// overridable notifications
		/// </summary>
		/// <param name="index">index of set object</param>
		/// <param name="oldValue">old object value</param>
		/// <param name="newValue">new object value</param>
		protected virtual void OnSetComplete(int index, object oldValue, object newValue)
		{
		}
		/// <summary>
		/// overridable notifications
		/// </summary>
		/// <param name="value">object value to validate</param>
		protected virtual void OnValidate(object value)
		{
			if (value == null) throw new ArgumentNullException("value");
		}

		#endregion

		#region Event and EventRaiser

		/// <summary>
		/// RaiseCollectionChanged
		/// </summary>
		protected void RaiseCollectionChanged()
		{
			if (CollectionChanged != null) CollectionChanged(this, System.EventArgs.Empty);
		}
		/// <summary>
		/// RaisePropertyChanged
		/// </summary>
		protected internal void RaisePropertyChanged()
		{
			if (PropertyChanged != null) PropertyChanged(this, System.EventArgs.Empty);
		}
		/// <summary>
		/// CollectionChanged Event
		/// </summary>
		[Category("Changed")]
		public event System.EventHandler CollectionChanged;
		/// <summary>
		/// PropertyChanged Event
		/// </summary>
		[Category("Changed")]
		public event System.EventHandler PropertyChanged;

		#endregion

		#region Public Property

		/// <summary>
		/// Get/Set Is Editable collection
		/// </summary>
		[Browsable(false)]
		public bool Editable { get { return _editable; } set { _editable = value; } }
		/// <summary>
		/// Get Base Type
		/// </summary>
		[Browsable(false)]
		public Type BaseType { get { return _baseType; } }

		#endregion
	}

	#endregion
}

#endregion

#region Object Container Generic Version

namespace NLib.Generic
{
	#region Container RTTI

	/// <summary>
	/// Container RTTI.
	/// </summary>
	internal class ContainerRTTI
	{
		#region Internal Variable

		private Type _objectType = null;
		private System.Reflection.PropertyInfo[] _props;
		private IComparer _comparer = null;

		private Type[] defaultValueTypes = null;
		private string[] defaultValuePropertyNames = null;
		private object[] defaultValues = null;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor
		/// </summary>
		private ContainerRTTI() : base() { }
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="objectType">The object Type.</param>
		public ContainerRTTI(Type objectType)
			: this()
		{
			_objectType = objectType;
			if (_objectType != null)
			{
				// keep properties
				_props = objectType.GetProperties(BindingFlags.Instance |
					BindingFlags.Public | BindingFlags.NonPublic |
					BindingFlags.SetProperty); // read all property that has set property.
				InitComparer();
			}
		}
		/// <summary>
		/// Destructor
		/// </summary>
		~ContainerRTTI()
		{
			_props = null;
			_comparer = null;
			_objectType = null;
		}

		#endregion

		#region Private Method

		private void InitComparer()
		{
			EnhanceMultiPropertyComparer cmp;
			cmp = new EnhanceMultiPropertyComparer();

			cmp.UsedDynamicAccess = true;

			string[] props = (string[])Array.CreateInstance(typeof(string), _props.Length);
			for (int i = 0; i < _props.Length; i++) props[i] = _props[i].Name;
			cmp.Properties = props;
			_comparer = cmp;
		}

		#endregion

		#region Public Method

		/// <summary>
		/// Create new instance.
		/// </summary>
		/// <returns>The new instance for specificed type.</returns>
		public object Create()
		{
			if (_objectType == null)
				return null;

			object result = null;

			result = NLib.Reflection.DynamicAccess.Create(_objectType);

			if (result != null)
			{
				if (defaultValuePropertyNames == null ||
					defaultValues == null)
				{
					#region init list

					List<string> _names = new List<string>();
					List<Type> _types = new List<Type>();
					List<object> _values = new List<object>();
					int propCount = _props.Length;
					PropertyInfo info;
					for (int i = 0; i < propCount; i++)
					{
						info = this.Properties[i];
						object[] defAttrs = info.GetCustomAttributes(typeof(DefaultValueAttribute), true);
						if (defAttrs != null && defAttrs.Length > 0 &&
							defAttrs[0] is DefaultValueAttribute)
						{
							DefaultValueAttribute attr = defAttrs[0] as DefaultValueAttribute;
							_names.Add(info.Name); // add property name.
							_types.Add(info.PropertyType); // add property type.
							_values.Add(attr.Value); // add value.
						}
					}
					if (_names.Count > 0 && _values.Count > 0 &&
						_names.Count == _values.Count)
					{
						// assign to array
						defaultValuePropertyNames = _names.ToArray();
						defaultValueTypes = _types.ToArray();
						defaultValues = _values.ToArray();
					}
					else
					{
						// create empty array.
						defaultValuePropertyNames = new string[0];
						defaultValueTypes = new Type[0];
						defaultValues = new object[0];
					}

					#endregion
				}

				#region Assign Default Value if required.

				if (defaultValuePropertyNames.Length > 0 &&
					defaultValues.Length > 0 &&
					defaultValueTypes.Length > 0 &&
					defaultValuePropertyNames.Length == defaultValues.Length &&
					defaultValueTypes.Length == defaultValues.Length)
				{
					// assign default value to object instance
					for (int i = 0; i < defaultValues.Length; i++)
					{
						string name = defaultValuePropertyNames[i];
						object val = defaultValues[i];
						Type type = defaultValueTypes[i];
						try
						{
							NLib.Reflection.DynamicAccess.Set(result, name,
								Convert.ChangeType(val, type));
						}
						catch { } // ignore exception
					}
				}
				else
				{
					// do nothing.
				}

				#endregion

			}

			return result;
		}
		/// <summary>
		/// Clone data
		/// </summary>
		/// <param name="source">The source object.</param>
		/// <param name="dest">The target object.</param>
		/// <returns>true if clone success</returns>
		public bool CloneData(object source, object dest)
		{
			if (source == null || dest == null)
				return false;

			Type sourceType = source.GetType();
			Type destType = dest.GetType();
			if (sourceType != destType) return false; // not same type
			int propCount = this.Properties.Length;
			PropertyInfo info;
			for (int i = 0; i < propCount; i++)
			{
				try
				{
					info = this.Properties[i];
					object val;

					val = NLib.Reflection.DynamicAccess.Get(source, info.Name);

					if (val != null && info.PropertyType.IsEnum)
					{
						val = Enum.ToObject(info.PropertyType, val);
					}

					NLib.Reflection.DynamicAccess.Set(dest, info.Name, val);
				}
				catch (Exception ex)
				{
					// error ignore
					ex.Err();
				}
			}

			return true; // all completed
		}
		/// <summary>
		/// Check is objects equal by it's proeprties
		/// </summary>
		/// <param name="source">Source object</param>
		/// <param name="dest">destination object</param>
		/// <returns>true if all properties is same.</returns>
		public bool IsEquals(object source, object dest)
		{
			if (this.ObjectComparer == null)
				return source.Equals(dest);
			else return (this.ObjectComparer.Compare(source, dest) == 0);
		}

		#endregion

		#region Public Property

		/// <summary>
		/// Access Property Information
		/// </summary>
		public PropertyInfo[] Properties { get { return _props; } }
		/// <summary>
		/// Get Comparer's object
		/// </summary>
		public IComparer ObjectComparer { get { return _comparer; } }

		#endregion
	}

	#endregion

	#region Reflecion Type Manager

	/// <summary>
	/// Reflecion Type Manager.
	/// </summary>
	internal class __ReflecionManager
	{
		#region Internal Variable

		private Dictionary<Type, ContainerRTTI> _caches = new Dictionary<Type, ContainerRTTI>();

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor
		/// </summary>
		public __ReflecionManager() : base() { }
		/// <summary>
		/// Destructor
		/// </summary>
		~__ReflecionManager()
		{
			if (_caches != null)
			{
				_caches.Clear();
			}
			_caches = null;
		}

		#endregion

		#region Hash Access

		/// <summary>
		/// Get ContainerRTTI by Type
		/// </summary>
		public ContainerRTTI this[Type type]
		{
			get
			{
				if (!_caches.ContainsKey(type))
				{

					ContainerRTTI info = new ContainerRTTI(type);
					_caches.Add(type, info);
				}
				return _caches[type];
			}
		}

		#endregion
	}

	#endregion

	#region Container Cache Type Reflecion Manager

	/// <summary>
	/// Container Cache Type Reflecion Manager.
	/// </summary>
	internal class ContainerTypeManager
	{
		#region Static Variable

		private static __ReflecionManager rm = new __ReflecionManager();

		#endregion

		#region Static Method

		/// <summary>
		/// Get Runtime Type Information
		/// </summary>
		/// <param name="type">Type instance</param>
		/// <returns>ContainerRTTI instance that match Type instance</returns>
		public static ContainerRTTI GetRTTI(Type type)
		{
			return rm[type];
		}

		#endregion
	}

	#endregion

	#region ObjectContainer PropertyDescriptor

	/// <summary>
	/// ObjectContainer PropertyDescriptor
	/// </summary>
	internal class ObjectContainerPropertyDescriptor<T> : PropertyDescriptor
	{
		#region Internal Variable

		private string _propertyName = "";
		private Type _propertyType = null;
		private PropertyInfo _propInfo = null;
		private string _category = "";
		private string _description = "";

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="propertyName">Property's Name</param>
		/// <param name="propertyType">Property's DataType</param>
		/// <param name="category">Property's Category</param>
		/// <param name="description">Property's Description</param>
		/// <param name="attributes">Array of property's attribute</param>
		public ObjectContainerPropertyDescriptor(string propertyName, Type propertyType,
			string category, string description, Attribute[] attributes)
			: base(propertyName, attributes)
		{
			_propertyName = propertyName;
			_propertyType = propertyType;
			_category = category;
			_description = description;
		}

		#endregion

		#region Stub

		/// <summary>
		/// CanResetValue
		/// </summary>
		/// <param name="component">target component</param>
		/// <returns>true if component can reset value</returns>
		public override bool CanResetValue(object component) { return false; }
		/// <summary>
		/// ResetValue
		/// </summary>
		/// <param name="component">target component</param>
		public override void ResetValue(object component) { }
		/// <summary>
		/// IsReadOnly
		/// </summary>
		public override bool IsReadOnly { get { return false; } }
		/// <summary>
		/// ShouldSerializeValue
		/// </summary>
		/// <param name="component">target component</param>
		/// <returns>true if the value should serialization</returns>
		public override bool ShouldSerializeValue(object component) { return false; }
		/// <summary>
		///  When overridden in a derived class, gets the type of the component 
		///  this property is bound to. in this case is ObjectContainer
		/// </summary>
		public override Type ComponentType { get { return typeof(ObjectContainer<T>); } }

		#endregion

		#region Property Type

		/// <summary>
		/// get property type
		/// </summary>
		public override Type PropertyType { get { return _propertyType; } }
		/// <summary>
		/// Get Category
		/// </summary>
		public override string Category
		{
			get
			{
				if (_category.Length <= 0) return base.Category;
				return _category;
			}
		}
		/// <summary>
		/// Get Description
		/// </summary>
		public override string Description
		{
			get
			{
				if (_description.Length <= 0) return base.Description;
				return _description;
			}
		}

		#endregion

		#region Get and Set Value

		/// <summary>
		/// InitPropertyInfo
		/// </summary>
		/// <param name="actualObject">object to load property info</param>
		private void InitPropertyInfo(object actualObject)
		{
			if (actualObject == null) return;
			if (_propInfo == null)
			{
				BindingFlags binds = BindingFlags.Instance | BindingFlags.Public;
				_propInfo = actualObject.GetType().GetProperty(_propertyName, binds);
			}
		}
		/// <summary>
		/// GetValue
		/// </summary>
		/// <param name="component">target component</param>
		/// <returns>value to get from component's property</returns>
		public override object GetValue(object component)
		{
			if (component == null) return null;
			if (!(component is ObjectContainer<T>)) return null;
			ObjectContainer<T> _Container = (component as ObjectContainer<T>);
			if (_Container == null) return null;

			if (_propertyName == "_No_" && _propertyType == typeof(int)) // _No_ only Property by design
			{
				// try to find property that related in ObjectContainer
				if (_propInfo == null) InitPropertyInfo(_Container);
				if (_propInfo == null) return null;
				// get value from container object
				try
				{
					if (_propInfo.CanRead)
					{
						return NLib.Reflection.DynamicAccess.Get(_Container, _propInfo.Name);
					}
					else
					{
						return null;
					}
				}
				catch
				{
					return null;
				}
			}
			else
			{
				if (_Container.Data == null) return null;
				if (_propInfo == null) InitPropertyInfo(_Container.Data);
				if (_propInfo == null) return null;
				// get value from internal object
				try
				{
					if (_propInfo.CanRead)
					{
						return NLib.Reflection.DynamicAccess.Get(_Container.Data, _propInfo.Name);
					}
					else
					{
						return null;
					}
				}
				catch
				{
					return null;
				}
			}
		}
		/// <summary>
		/// SetValue
		/// </summary>
		/// <param name="component">target component</param>
		/// <param name="value">value to set to component's property</param>
		public override void SetValue(object component, object value)
		{
			if (component == null) return;
			if (!(component is ObjectContainer<T>)) return;
			ObjectContainer<T> _Container = (component as ObjectContainer<T>);
			if (_Container == null) return;

			if (_propertyName == "_No_" && _propertyType == typeof(int)) // _No_ only Property by design
			{
				if (!_Container.IsLock)
					_Container.Backup(); // backup data first

				// try to find property that related in ObjectContainer
				if (_propInfo == null) InitPropertyInfo(_Container);
				if (_propInfo == null) return;
				// set value to container object
				try
				{
					if (_propInfo.CanWrite)
					{
						NLib.Reflection.DynamicAccess.Set(_Container, _propInfo.Name, value);
						OnValueChanged(component, EventArgs.Empty);
					}
				}
				catch
				{
				}

			}
			else
			{
				if (_Container.Data == null) return;

				if (!_Container.IsLock)
					_Container.Backup(); // backup data first

				if (_propInfo == null) InitPropertyInfo(_Container.Data);
				if (_propInfo == null) return;
				// set value to internal object
				try
				{
					if (_propInfo.CanWrite)
					{
						NLib.Reflection.DynamicAccess.Set(_Container.Data, _propInfo.Name, value);
						OnValueChanged(component, EventArgs.Empty);
					}
				}
				catch
				{
				}
			}
		}

		#endregion
	}

	#endregion

	#region Object Container (Generic version)

	/// <summary>
	/// Object Container (Generic version).
	/// </summary>
	/// <typeparam name="T">The object type.</typeparam>
	public class ObjectContainer<T> : ICustomTypeDescriptor, IObjectContainer
	{
		#region Internal Class

		private class NotCopied
		{
			private static NotCopied _Value = new NotCopied();
			public static NotCopied Value { get { return _Value; } }
		}

		#endregion

		#region Internal Variable

		private T _temp;
		private T _object;
		private ObjectContainerCollection<T> _Collection = null;
		private object[] _OriginalValues = null;
		private int iLock = 0;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor
		/// </summary>
		public ObjectContainer() : base() { }
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="value"></param>
		public ObjectContainer(T value)
			: this()
		{
			_object = value;
		}
		/// <summary>
		/// Destructor.
		/// </summary>
		~ObjectContainer()
		{
			_object = default(T);
			_temp = default(T);
			_OriginalValues = null;
			_Collection = null;
		}

		#endregion

		#region Internal Access

		/// <summary>
		/// SetCollection
		/// not a contructor since it would fuck up derived classes
		/// </summary>
		/// <param name="Collection">Owner collection.</param>
		internal void SetCollection(ObjectContainerCollection<T> Collection)
		{
			_Collection = Collection;
		}
		/// <summary>
		/// Internal Set Data
		/// </summary>
		/// <param name="data">The data instance to assign direct to object container.</param>
		internal void SetData(T data)
		{
			_object = data;
		}
		/// <summary>
		/// Protected to Access Collection
		/// </summary>
		protected ObjectContainerCollection<T> Collection
		{
			get { return _Collection; }
		}

		#endregion

		#region Private Method

		private void CloneData(object source, object target)
		{
			ContainerRTTI rtti = ContainerTypeManager.GetRTTI(typeof(T));
			if (rtti != null)
			{
				rtti.CloneData(source, target); // copy information
			}
		}

		#endregion

		#region Interface Implements

		#region ICustomTypeDescriptor Members

		#region Stub

		/// <summary>
		/// ICustomTypeDescriptor.GetConverter
		/// </summary>
		/// <returns>alway return null</returns>
		TypeConverter ICustomTypeDescriptor.GetConverter() { return null; }
		/// <summary>
		/// ICustomTypeDescriptor.GetComponentName
		/// </summary>
		/// <returns>alway return null</returns>
		string ICustomTypeDescriptor.GetComponentName() { return null; }
		/// <summary>
		/// ICustomTypeDescriptor.GetClassName
		/// </summary>
		/// <returns>alway return null</returns>
		string ICustomTypeDescriptor.GetClassName() { return null; }
		/// <summary>
		/// ICustomTypeDescriptor.GetAttributes
		/// </summary>
		/// <returns>Empty Attribute Collection</returns>
		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return new AttributeCollection(null);
		}
		/// <summary>
		/// ICustomTypeDescriptor.GetDefaultProperty
		/// </summary>
		/// <returns>alway return null</returns>
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return null; }
		/// <summary>
		/// ICustomTypeDescriptor.GetDefaultEvent
		/// </summary>
		/// <returns>alway return null</returns>
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return null; }
		/// <summary>
		/// ICustomTypeDescriptor.GetEvents
		/// </summary>
		/// <param name="attributes">Array of attribute</param>
		/// <returns>Empty EventDescriptor's Collection</returns>
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) { return new EventDescriptorCollection(null); }
		/// <summary>
		/// ICustomTypeDescriptor.GetEvents
		/// </summary>
		/// <returns>Empty EventDescriptor's Collection</returns>
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { return new EventDescriptorCollection(null); }
		/// <summary>
		/// ICustomTypeDescriptor.GetEditor
		/// </summary>
		/// <param name="editorBaseType"></param>
		/// <returns>alway return null</returns>
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return null; }
		/// <summary>
		/// ICustomTypeDescriptor.GetPropertyOwner
		/// </summary>
		/// <param name="pd">Property Descriptor instance</param>
		/// <returns>return this instance</returns>
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) { return this; }
		/// <summary>
		/// ICustomTypeDescriptor.GetProperties
		/// </summary>
		/// <returns>PropertyDescriptor's Collection</returns>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() { return ((ICustomTypeDescriptor)this).GetProperties(null); }

		#endregion

		#region GetProperties

		/// <summary>
		/// ICustomTypeDescriptor.GetProperties
		/// </summary>
		/// <param name="attributes">Array of attribute</param>
		/// <returns>PropertyDescriptor Collection instance</returns>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection props;
			// Create the property collection and filter
			props = new PropertyDescriptorCollection(null);

			// for display in property grid
			props.Add(new ObjectContainerPropertyDescriptor("_No_", typeof(int), "Information", "Get Number in list", attributes));

			PropertyDescriptorCollection sourceProps = TypeDescriptor.GetProperties(_object, attributes, true);
			foreach (PropertyDescriptor prop in sourceProps)
			{
				props.Add(new ObjectContainerPropertyDescriptor(prop.Name, prop.PropertyType, prop.Category, prop.Description, attributes));
			}

			return props;
		}

		#endregion

		#endregion

		#region IEditableObject implements

		/// <summary>
		/// IEditableObject.BeginEdit()
		/// </summary>
		void IEditableObject.BeginEdit()
		{
			if (!IsEdit)
			{
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(_object, null);
				object[] vals = new object[props.Count];
				for (int i = 0; i < props.Count; ++i)
				{
					vals[i] = NotCopied.Value;
					PropertyDescriptor desc = props[i];
					if (desc.PropertyType.IsSubclassOf(typeof(ValueType)))
					{
						vals[i] = desc.GetValue(_object);
					}
					else
					{
						object val = desc.GetValue(_object);
						if (val == null) vals[i] = null;
						else if (val is IList) { /* if IList, then no copy */ }
						else if (val is ICloneable) vals[i] = ((ICloneable)val).Clone();
					}
				}
				_OriginalValues = vals;
			}
		}
		/// <summary>
		/// IEditableObject.CancelEdit() 
		/// </summary>
		void IEditableObject.CancelEdit()
		{
			if (IsEdit)
			{
				if (PendingInsert) ((IList)_Collection).Remove(this);
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(_object, null);
				for (int i = 0; i < props.Count; ++i)
				{
					if (_OriginalValues[i] is NotCopied) continue;
					props[i].SetValue(_object, _OriginalValues[i]);
				}
				_OriginalValues = null;
				RaisePropertyChanged();
			}
		}
		/// <summary>
		/// IEditableObject.EndEdit
		/// </summary>
		void IEditableObject.EndEdit()
		{
			if (IsEdit)
			{
				if (PendingInsert) _Collection._PendingInsert = null;
				_OriginalValues = null;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region ICloneable Members

		/// <summary>
		/// Clone Object
		/// </summary>
		/// <returns>new instance that clone from this object</returns>
		public object Clone()
		{
			object cloneData = GetClone();
			if (cloneData != null)
				return new ObjectContainer<T>((T)cloneData);
			else return null;
		}

		#endregion

		#endregion

		#region Override

		/// <summary>
		/// Equals
		/// </summary>
		/// <param name="obj">object instance to check equal</param>
		/// <returns>see Object.Equals</returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
		/// <summary>
		/// GetHashCode
		/// </summary>
		/// <returns>see Object.GetHashCode</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		/// <summary>
		/// ToString
		/// </summary>
		/// <returns>see Object.ToString</returns>
		public override string ToString()
		{
			if (_object == null)
				return base.ToString();
			else return _object.ToString();
		}

		#endregion

		#region Protected Method

		#region GetClone

		/// <summary>
		/// Gets Clone.
		/// </summary>
		/// <returns>The clone of internal object.</returns>
		protected virtual T GetClone()
		{
			if (_object == null)
				return default(T);
			ContainerRTTI rtti = ContainerTypeManager.GetRTTI(typeof(T));
			if (rtti != null)
			{
				object obj = rtti.Create();
				if (obj != null)
					rtti.CloneData(_object, obj); // copy information
				return (T)obj;
			}
			else return default(T);
		}

		#endregion

		#region Backup

		/// <summary>
		/// Backup Data Object.
		/// </summary>
		protected internal void Backup()
		{
			if (_object != null && _temp == null)
			{
				_temp = GetClone(); // clone data
			}
		}

		#endregion

		#region Event Raisers

		/// <summary>
		/// Raise event. Thread safe.
		/// </summary>
		/// <param name="del">The delegate.</param>
		/// <param name="args">The arguments.</param>
		protected void RaiseEvent(Delegate del, params object[] args)
		{
			ApplicationManager.Instance.Invoke(del, args);
		}
		/// <summary>
		/// Raise Begin Init event.
		/// </summary>
		protected void RaiseBeginInit()
		{
			if (OnBeginInit != null) RaiseEvent(OnBeginInit, this, System.EventArgs.Empty);
		}
		/// <summary>
		/// Raise End Init event.
		/// </summary>
		protected void RaiseEndInit()
		{
			if (OnEndInit != null) RaiseEvent(OnEndInit, this, System.EventArgs.Empty);
		}
		/// <summary>
		/// Raise Property Change event.
		/// </summary>
		protected void RaisePropertyChanged()
		{
			if (HasChanged())
			{
				if (OnPropertyChanged != null) RaiseEvent(OnPropertyChanged, this, System.EventArgs.Empty);
				if (_Collection != null)
				{
					_Collection.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#endregion

		#region Public Method

		#region Begin and End Init

		/// <summary>
		/// Call when data is initialize.
		/// </summary>
		public void BeginInit()
		{
			Lock();
			RaiseBeginInit();
		}

		/// <summary>
		/// Call when finished initialize.
		/// </summary>
		public void EndInit()
		{
			Unlock();
			RaiseEndInit();
		}

		#endregion

		#region Accept and Reject

		/// <summary>
		/// Accept Changed made on object.
		/// </summary>
		public void Accept()
		{
			_temp = default(T); // clear temp
		}
		/// <summary>
		/// Reject Changed made on object.
		/// </summary>
		public void Reject()
		{
			if (_temp == null || _object == null) return; // no changed occur
			this.Lock();
			CloneData(_temp, _object);
			this.Unlock();
		}

		#endregion

		#region Lock and Unlock

		/// <summary>
		/// Check is Lock for Stop Event Raising.
		/// </summary>
		[System.ComponentModel.Bindable(false)]
		[Browsable(false)]
		public bool IsLock
		{
			get { return (iLock > 0); }
		}
		/// <summary>
		/// Lock to block Event Raising.
		/// </summary>
		public void Lock()
		{
			iLock++;
		}
		/// <summary>
		/// Unlock to allow Event Raising.
		/// </summary>
		public void Unlock()
		{
			iLock--;
			if (iLock < 0) iLock = 0;
		}

		#endregion

		#region Has Changed

		/// <summary>
		/// Check that object has been changed or not.
		/// </summary>
		/// <returns>true if data has been changed.</returns>
		public bool HasChanged()
		{
			if (_temp == null)
				return false;
			return !ContainerTypeManager.GetRTTI(typeof(T)).IsEquals(_object, _temp);
		}

		#endregion

		#endregion

		#region Public Property

		/// <summary>
		/// Is Pending Insert.
		/// </summary>
		[Browsable(false)]
		public bool PendingInsert
		{
			get { return (_Collection._PendingInsert == this); }
		}
		/// <summary>
		/// Is Editting.
		/// </summary>
		[Browsable(false)]
		public bool IsEdit
		{
			get { return (_OriginalValues != null); }
		}
		/// <summary>
		/// Access Data object (Readonly).
		/// </summary>
		[Browsable(false)]
		public T Data { get { return _object; } }
		/// <summary>
		/// Access Data object (Readonly).
		/// </summary>
		object IObjectContainer.Data { get { return _object; } }
		/// <summary>
		/// Gets or sets No. or Object Index.
		/// </summary>
		[Browsable(false)]
		public int _No_
		{
			get
			{
				if (_Collection == null) return 0;
				else return _Collection.IndexOf(this) + 1;
			}
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Raise when BeginInit() is called.
		/// </summary>
		[Category("Init")]
		public event System.EventHandler OnBeginInit;
		/// <summary>
		/// Raise when EndInit() is called.
		/// </summary>
		[Category("Init")]
		public event System.EventHandler OnEndInit;
		/// <summary>
		/// Raise when Object's Property is changed.
		/// </summary>
		[Category("Changed")]
		public event System.EventHandler OnPropertyChanged;

		#endregion

		#region Static Method

		#region Create Helper

		/// <summary>
		/// Create new object Container instance.
		/// </summary>
		/// <param name="data">actual object instance.</param>
		/// <returns>ObjectContainer instance that wrap around actual object(or data).</returns>
		public static ObjectContainer<T> Create(T data)
		{
			if (data == null)
				return null;
			return new ObjectContainer<T>(data);
		}
		/// <summary>
		/// Create new object Container array instance.
		/// </summary>
		/// <param name="size">size of array.</param>
		/// <returns>Array of ObjectContainer with initialize instance of object.</returns>
		public static ObjectContainer<T>[] CreateArray(int size)
		{
			ContainerRTTI rtti = ContainerTypeManager.GetRTTI(typeof(T));
			if (rtti != null)
			{
				List<ObjectContainer<T>> _results = new List<ObjectContainer<T>>();
				for (int i = 0; i < size; i++)
				{
					_results.Add(new ObjectContainer<T>((T)rtti.Create()));
				}
				return _results.ToArray();
			}
			else return null;
		}

		#endregion

		#endregion
	}

	#endregion

	#region Object Container Collection (Generic version)

	/// <summary>
	/// Object Container Collection (Generic version).
	/// </summary>
	/// <typeparam name="T">The object type.</typeparam>
	public class ObjectContainerCollection<T> : IObjectContainerCollection,
		IBindingList, ITypedList
	{
		#region Internal Variable

		private ArrayList _List = null;
		internal ObjectContainer<T> _PendingInsert = null;
		private Type _baseType = null;
		private List<T> _removedList = new List<T>();

		private bool _editable = true;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public ObjectContainerCollection()
			: base()
		{
			_baseType = typeof(T);
			_List = new ArrayList();
			_PendingInsert = null;
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="values">The array of object.</param>
		public ObjectContainerCollection(T[] values)
			: this()
		{
			if (values == null || values.Length <= 0)
				return;
			else
			{
				int i = 0;
				ObjectContainer<T>[] Containers = (ObjectContainer<T>[])Array.CreateInstance(typeof(ObjectContainer<T>), values.Length);
				foreach (T val in values)
				{
					Containers[i] = new ObjectContainer<T>(val);
					Containers[i].SetCollection(this);
					i++;
				}
				_List.AddRange(Containers);
			}
		}
		/// <summary>
		/// Destructor.
		/// </summary>
		~ObjectContainerCollection()
		{
			if (_List != null) _List.Clear();
			_List = null;

			if (_removedList != null) _removedList.Clear();
			_removedList = null;

			_PendingInsert = null;
			_baseType = null;

			NGC.FreeGC(this);
		}

		#endregion

		#region Internal Access

		/// <summary>
		/// Gets Current pending object.
		/// </summary>
		protected internal ObjectContainer<T> PendingInsert { get { return _PendingInsert; } }

		#endregion

		#region Removed List related Method

		/// <summary>
		/// Add To Remove List.
		/// </summary>
		/// <param name="value">The value that remove from collection.</param>
		private void AddToRemoveList(ObjectContainer<T> value)
		{
			ObjectContainer<T> container = (value as ObjectContainer<T>);
			if (_PendingInsert != container)
			{
				T item = container.Data; // read data from container
				_removedList.Add(item);
			}
		}
		/// <summary>
		/// Add To Remove List.
		/// </summary>
		/// <param name="value">The value that remove from collection.</param>
		private void AddToRemoveList(T value)
		{
			if (_PendingInsert != null)
			{
				// has pending insert. try to compare
				if (!_PendingInsert.Data.Equals(value))
				{
					if (!_removedList.Contains(value)) _removedList.Add(value);
				}
			}
			else
			{
				// no pending insert
				if (!_removedList.Contains(value)) _removedList.Add(value);
			}
		}
		/// <summary>
		/// Checks that has removed item in list.
		/// </summary>
		public bool HasRemovedItem
		{
			get
			{
				if (_removedList == null) return false;
				return (_removedList.Count > 0);
			}
		}
		/// <summary>
		/// Gets Removed Objects.
		/// </summary>
		/// <returns>array of object that removed from collection.</returns>
		public T[] GetRemovedObjects()
		{
			if (_removedList == null || _removedList.Count <= 0)
				return null;
			return _removedList.ToArray();
		}
		/// <summary>
		/// Clear all object from removed list.
		/// </summary>
		public void ClearRemovedList()
		{
			if (_removedList != null) _removedList.Clear();
		}

		#endregion

		#region Public Access

		/// <summary>
		/// Gets the number of elememts in list.
		/// </summary>
		public int Count { get { return _List.Count; } }
		/// <summary>
		/// Clear all element in list and all data in removed list.
		/// </summary>
		public void Clear()
		{
			OnClear();
			if (_List != null)
			{
				for (int i = 0; i < _List.Count; ++i)
					((ObjectContainer<T>)_List[i]).SetCollection(null);

				if (_List != null) _List.Clear();
				if (_removedList != null) _removedList.Clear();

				_PendingInsert = null;
				OnClearComplete();
				if (_ListChanged != null)
				{
					RaiseEvent(_ListChanged,
						this, new ListChangedEventArgs(ListChangedType.Reset, 0));
				}
			}
		}
		/// <summary>
		/// Remove Element From List via index.
		/// </summary>
		/// <param name="index">index to remove.</param>
		public void RemoveAt(int index)
		{
			if (index < 0 || index >= _List.Count)
				throw new ArgumentOutOfRangeException();
			ObjectContainer<T> item = (ObjectContainer<T>)_List[index];
			OnValidate(item);
			OnRemove(index, item);
			((ObjectContainer<T>)_List[index]).SetCollection(null);
			if (_PendingInsert == item) _PendingInsert = null;
			_List.RemoveAt(index);
			OnRemoveComplete(index, item);

			// Append to Remove List
			if (item != null) AddToRemoveList(item);

			if (_ListChanged != null)
			{
				try
				{
					RaiseEvent(_ListChanged,
						this, new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
				}
				catch (Exception ex)
				{
					throw (ex);
				}
			}
		}
		/// <summary>
		/// Get Enumerator.
		/// </summary>
		/// <returns>Returns IEnumerator object.</returns>
		public IEnumerator GetEnumerator() { return _List.GetEnumerator(); }
		/// <summary>
		/// Add Object Container.
		/// </summary>
		/// <param name="value">The object to add.</param>
		public void Add(ObjectContainer<T> value)
		{
			if (value == null) return;
			ObjectContainer<T>[] values = new ObjectContainer<T>[] { value };
			AddRange(values);
		}
		/// <summary>
		/// Add Data array.
		/// </summary>
		/// <param name="values">The data array.</param>
		public void AddRange(T[] values)
		{
			if (values == null || values.Length <= 0)
				return;
			else
			{
				int i = 0;
				ObjectContainer<T>[] Containers = (ObjectContainer<T>[])Array.CreateInstance(typeof(ObjectContainer<T>), values.Length);
				foreach (T val in values)
				{
					Containers[i] = new ObjectContainer<T>(val);
					Containers[i].SetCollection(this);
					i++;
				}
				_List.AddRange(Containers);
			}
		}
		/// <summary>
		/// Add Object Container Array.
		/// </summary>
		/// <param name="values">The Object Container's array.</param>
		public void AddRange(ObjectContainer<T>[] values)
		{
			if (values == null) return;
			AddRangeInternal(values);

			if (_ListChanged != null)
			{
				try
				{
					ListChangedEventArgs evt = new ListChangedEventArgs(ListChangedType.ItemAdded, -1);
					RaiseEvent(_ListChanged, this, evt);
				}
				catch (Exception ex)
				{
					throw (ex);
				}
			}
		}

		#endregion

		#region Protected Access

		/// <summary>
		/// helper method for add range via derived class.
		/// </summary>
		/// <param name="c">Object Container's array.</param>
		protected void AddRangeInternal(ObjectContainer<T>[] c)
		{
			if (c == null || c.Length <= 0) return;
			_List.AddRange(c);
			foreach (ObjectContainer<T> obj in c) obj.SetCollection(this);
		}
		/// <summary>
		/// helper method for add range via derived class.
		/// </summary>
		/// <param name="c">object array instance to add.</param>
		protected void AddRangeInternal(T[] c)
		{
			if (c == null || c.Length <= 0)
				return;

			ObjectContainer<T>[] Containers = (ObjectContainer<T>[])Array.CreateInstance(typeof(ObjectContainer<T>), c.Length);
			int i = 0;
			foreach (T val in c)
			{
				Containers[i] = new ObjectContainer<T>(val);
				Containers[i].SetCollection(this);
				i++;
			}
			_List.AddRange(Containers);
		}
		/// <summary>
		/// There is no InnerList since using it would cease firing events.
		/// </summary>
		protected IList List { get { return this; } }
		/// <summary>
		/// Get Element Type override to get the correct type.
		/// </summary>
		protected virtual Type ElementType { get { return _baseType; } }
		/// <summary>
		/// Create New Container.
		/// </summary>
		/// <param name="data">The data instance.</param>
		/// <returns>Returns instance of ObjectContainer.</returns>
		protected ObjectContainer<T> CreateNewContainer(T data)
		{
			ObjectContainer<T> Container = new ObjectContainer<T>(data);
			Container.SetCollection(this);
			return Container;
		}
		/// <summary>
		/// Create Instance override if the default constructor is not suitable.
		/// </summary>
		/// <returns>Returns new object instance.</returns>
		protected virtual T CreateInstance()
		{
			return (T)ContainerTypeManager.GetRTTI(this.BaseType).Create();
		}

		#endregion

		#region IObjectContainerCollection Implements

		/// <summary>
		/// Get Removed Items.
		/// </summary>
		/// <returns>array of item that removed from collection.</returns>
		object[] IObjectContainerCollection.GetRemoveItems()
		{
			ArrayList results = new ArrayList(GetRemovedObjects());
			return results.ToArray();
		}
		/// <summary>
		/// Clear all remove items.
		/// </summary>
		void IObjectContainerCollection.ClearRemoveItems()
		{
			ClearRemovedList();
		}
		/// <summary>
		/// Checks is the collection is in pending state.
		/// </summary>
		bool IObjectContainerCollection.IsPending
		{
			get { return (_PendingInsert != null); }
		}
		/// <summary>
		/// Gets Current Pending object.
		/// </summary>
		/// <returns>The instance of IObjectContainer.</returns>
		IObjectContainer IObjectContainerCollection.GetPendingObject()
		{
			if (_PendingInsert != null && _PendingInsert is IObjectContainer)
				return (IObjectContainer)_PendingInsert;
			else return null;
		}
		/// <summary>
		/// Create new object and set as pending item.
		/// </summary>
		/// <returns>return new object.</returns>
		IObjectContainer IObjectContainerCollection.AddNew()
		{
			object val = (this as IBindingList).AddNew();
			if (val != null && val is IObjectContainer)
			{
				return (val as IObjectContainer);
			}
			else return null;
		}
		/// <summary>
		/// Is allow edit item.
		/// </summary>
		bool IObjectContainerCollection.AllowEdit { get { return _editable; } }
		/// <summary>
		/// Is allow add new.
		/// </summary>
		bool IObjectContainerCollection.AllowNew { get { return _editable; } }
		/// <summary>
		/// Is allow remove.
		/// </summary>
		bool IObjectContainerCollection.AllowRemove { get { return _editable; } }

		#endregion

		#region IBindingList Implements

		/// <summary>
		/// Get IBindingList.AllowEdit. 
		/// </summary>
		bool IBindingList.AllowEdit
		{
			get { return _editable; }
		}
		/// <summary>
		/// Get IBindingList.AllowNew.
		/// </summary>
		bool IBindingList.AllowNew
		{
			get { return _editable; }
		}
		/// <summary>
		/// Get IBindingList.AllowRemove. 
		/// </summary>
		bool IBindingList.AllowRemove
		{
			get { return _editable; }
		}
		/// <summary>
		/// Get IBindingList.SupportsChangeNotification. 
		/// </summary>
		bool IBindingList.SupportsChangeNotification
		{
			get { return true; }
		}
		/// <summary>
		/// IBindingList.AddNew.
		/// </summary>
		/// <returns>new object instance.</returns>
		object IBindingList.AddNew()
		{
			if (_PendingInsert != null) ((IEditableObject)_PendingInsert).CancelEdit();

			ObjectContainer<T> item;
			T data = CreateInstance();
			if (data != null) item = CreateNewContainer(data);
			else item = null;

			if (item != null)
			{
				((IList)this).Add(item);
				_PendingInsert = item;
			}
			return item;
		}

		private ListChangedEventHandler _ListChanged;

		/// <summary>
		/// IBindingList.ListChanged event.
		/// </summary>
		event ListChangedEventHandler IBindingList.ListChanged
		{
			add { _ListChanged += value; }
			remove { _ListChanged -= value; }
		}
		/// <summary>
		/// IBindingList.SupportsSearching. 
		/// </summary>
		bool IBindingList.SupportsSearching
		{
			get { return false; }
		}
		/// <summary>
		/// IBindingList.SupportsSorting. 
		/// </summary>
		bool IBindingList.SupportsSorting
		{
			get { return false; }
		}
		/// <summary>
		/// IBindingList.IsSorted. 
		/// </summary>
		bool IBindingList.IsSorted
		{
			get { return false; }
		}
		/// <summary>
		/// IBindingList.SortDirection. 
		/// </summary>
		ListSortDirection IBindingList.SortDirection
		{
			get { throw new NotSupportedException(); }
		}
		/// <summary>
		/// IBindingList.SortProperty. 
		/// </summary>
		PropertyDescriptor IBindingList.SortProperty
		{
			get { throw new NotSupportedException(); }
		}
		/// <summary>
		/// IBindingList.AddIndex.
		/// </summary>
		/// <param name="property">Property Descriptor instance.</param>
		void IBindingList.AddIndex(PropertyDescriptor property)
		{
			throw new NotSupportedException();
		}
		/// <summary>
		/// IBindingList.ApplySort.
		/// </summary>
		/// <param name="property">Property Descriptor instance.</param>
		/// <param name="direction">Sort direction.</param>
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}
		/// <summary>
		/// IBindingList.Find.
		/// </summary>
		/// <param name="property">Property Descriptor instance.</param>
		/// <param name="key">object that used as key.</param>
		/// <returns>index that match by specificed key.</returns>
		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}
		/// <summary>
		/// IBindingList.RemoveIndex.
		/// </summary>
		/// <param name="property">Property Descriptor instance.</param>
		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
			throw new NotSupportedException();
		}
		/// <summary>
		/// IBindingList.RemoveSort.
		/// </summary>
		void IBindingList.RemoveSort()
		{
			throw new NotSupportedException();
		}

		#endregion

		#region ICollection Implements

		/// <summary>
		/// ICollection.CopyTo.
		/// </summary>
		/// <param name="array">destination array.</param>
		/// <param name="index">index to copy.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			_List.CopyTo(array, index);
		}
		/// <summary>
		/// ICollection.IsSynchronized. 
		/// </summary>
		bool ICollection.IsSynchronized
		{
			get { return _List.IsSynchronized; }
		}
		/// <summary>
		/// ICollection.SyncRoot.
		/// </summary>
		object ICollection.SyncRoot
		{
			get { return _List.SyncRoot; }
		}

		#endregion

		#region IList Implements

		/// <summary>
		/// IList.Add.
		/// </summary>
		/// <param name="value">object to add.</param>
		/// <returns>index of added object.</returns>
		int IList.Add(object value)
		{
			if (value == null || !(value is ObjectContainer<T>))
			{
				return -1;
			}
			ObjectContainer<T> item = (ObjectContainer<T>)value;
			OnValidate(item);
			OnInsert(_List.Count, item);
			int index = _List.Add(item);
			try
			{
				OnInsertComplete(index, item);
			}
			catch
			{
				_List.RemoveAt(index);
				throw;
			}
			item.SetCollection(this); // set collection.
			if (_ListChanged != null)
			{
				try
				{
					ListChangedEventArgs evt = new ListChangedEventArgs(ListChangedType.ItemAdded, index);
					_ListChanged(this, evt);
				}
				catch (Exception ex)
				{
					throw (ex);
				}
			}
			return index;
		}
		/// <summary>
		/// Indexer Access
		/// </summary>
		public object this[int index]
		{
			get
			{
				if (index < 0 || index >= _List.Count)
				{
					//throw new ArgumentOutOfRangeException();
					return null;
				}
				return _List[index];
			}
			set
			{
				if (index < 0 || index >= _List.Count)
				{
					//throw new ArgumentOutOfRangeException();
					return;
				}
				if (value == null || !(value is ObjectContainer<T>))
				{
					// invalid type.
					return;
				}

				ObjectContainer<T> newItem = (ObjectContainer<T>)value;

				OnValidate(newItem);
				ObjectContainer<T> oldItem = (ObjectContainer<T>)_List[index];
				OnSet(index, oldItem, newItem);
				_List[index] = newItem;
				try
				{
					OnSetComplete(index, oldItem, newItem);
				}
				catch
				{
					_List[index] = oldItem;
					throw;
				}
				if (_ListChanged != null)
				{
					try
					{
						RaiseEvent(_ListChanged,
							this, new ListChangedEventArgs(ListChangedType.ItemChanged, index));
					}
					catch (Exception ex)
					{
						throw (ex);
					}
				}
			}
		}
		/// <summary>
		/// Contains.
		/// </summary>
		/// <param name="value">object to check containment.</param>
		/// <returns>true if object is already exists on collection.</returns>
		public bool Contains(object value)
		{
			return _List.Contains(value);
		}
		/// <summary>
		/// IList.IsFixedSize
		/// </summary>
		bool IList.IsFixedSize
		{
			get { return _List.IsFixedSize; }
		}
		/// <summary>
		/// IList.IsReadOnly.
		/// </summary>
		bool IList.IsReadOnly
		{
			get { return !_editable; }
		}
		/// <summary>
		/// Index Of.
		/// </summary>
		/// <param name="value">object to find index.</param>
		/// <returns>index of specificed object.</returns>
		public int IndexOf(object value)
		{
			return _List.IndexOf(value);
		}
		/// <summary>
		/// IList.Insert.
		/// </summary>
		/// <param name="index">index to insert.</param>
		/// <param name="value">object to inserted collection.</param>
		void IList.Insert(int index, object value)
		{
			if (index < 0 || index > _List.Count)
				throw new ArgumentOutOfRangeException();

			if (value == null || !(value is ObjectContainer<T>))
			{
				return;
			}
			ObjectContainer<T> item = (ObjectContainer<T>)value;

			OnValidate(item);
			OnInsert(index, item);
			_List.Insert(index, item);
			try
			{
				OnInsertComplete(index, item);
			}
			catch
			{
				_List.RemoveAt(index);
				throw;
			}
			item.SetCollection(this); // set collection
			if (_ListChanged != null)
			{
				try
				{
					RaiseEvent(_ListChanged,
						this, new ListChangedEventArgs(ListChangedType.ItemAdded, index));
				}
				catch (Exception ex)
				{
					throw (ex);
				}
			}
		}
		/// <summary>
		/// Remove.
		/// </summary>
		/// <param name="value">object to remove from collection.</param>
		public void Remove(object value)
		{
			if (value == null || !(value is ObjectContainer<T>))
			{
				return;
			}
			ObjectContainer<T> item = (ObjectContainer<T>)value;

			OnValidate(item);
			int index = _List.IndexOf(item);
			if (index < 0)
				throw new ArgumentException();
			OnRemove(index, item);
			_List.RemoveAt(index);
			OnRemoveComplete(index, item);

			// Append to Remove List
			if (value != null)
				AddToRemoveList(item);

			if (_ListChanged != null)
			{
				try
				{
					RaiseEvent(_ListChanged,
						this, new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
				}
				catch (Exception ex)
				{
					throw (ex);
				}
			}
		}

		#endregion

		#region ITypedList Members

		/// <summary>
		/// ITypedList.GetItemProperties.
		/// </summary>
		/// <param name="listAccessors">Property Descriptor's array.</param>
		/// <returns>Property Descriptor's collection.</returns>
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			PropertyDescriptorCollection props;
			// Create the property collection and filter
			props = new PropertyDescriptorCollection(null);

			// for display in data grid
			props.Add(new ObjectContainerPropertyDescriptor<T>("_No_",
				typeof(int), "Information", "Get Number in list", null));

			ArrayList list = new ArrayList();
			PropertyDescriptorCollection sourceProps = TypeDescriptor.GetProperties(_baseType);
			foreach (PropertyDescriptor prop in sourceProps)
			{
				list.Clear();
				if (prop.Attributes != null && prop.Attributes.Count > 0)
				{
					list.AddRange(prop.Attributes);
					props.Add(new ObjectContainerPropertyDescriptor<T>(prop.Name,
						prop.PropertyType, prop.Category, prop.Description,
						(Attribute[])list.ToArray(typeof(Attribute))));
				}
				else
				{
					props.Add(new ObjectContainerPropertyDescriptor<T>(prop.Name,
						prop.PropertyType, prop.Category, prop.Description, null));
				}
			}
			list.Clear();
			list = null;

			return props;
		}
		/// <summary>
		/// ITypedList.GetListName.
		/// </summary>
		/// <param name="listAccessors">Property Descriptor's array.</param>
		/// <returns>List name.</returns>
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors == null || listAccessors.Length == 0)
				return "JOE"; // nevermind, never queryed
			if (_baseType == null)
				return "";
			return listAccessors[listAccessors.Length - 1].Name; // works as TableName
		}

		#endregion

		#region overridable notifications

		/// <summary>
		/// overridable notifications.
		/// </summary>
		protected virtual void OnClear() { }
		/// <summary>
		/// overridable notifications.
		/// </summary>
		protected virtual void OnClearComplete()
		{
			RaiseCollectionChanged();
		}
		/// <summary>
		/// overridable notifications.
		/// </summary>
		/// <param name="index">index of inserted object.</param>
		/// <param name="value">object value to inserted.</param>
		protected virtual void OnInsert(int index, ObjectContainer<T> value)
		{
		}
		/// <summary>
		/// overridable notifications
		/// </summary>
		/// <param name="index">index of inserted object</param>
		/// <param name="value">object value to inserted</param>
		protected virtual void OnInsertComplete(int index, ObjectContainer<T> value)
		{
			RaiseCollectionChanged();
		}
		/// <summary>
		/// overridable notifications.
		/// </summary>
		/// <param name="index">index of removed object.</param>
		/// <param name="value">object value to removed.</param>
		protected virtual void OnRemove(int index, ObjectContainer<T> value)
		{
		}
		/// <summary>
		/// overridable notifications.
		/// </summary>
		/// <param name="index">index of removed object.</param>
		/// <param name="value">object value to removed.</param>
		protected virtual void OnRemoveComplete(int index, ObjectContainer<T> value)
		{
			RaiseCollectionChanged();
		}
		/// <summary>
		/// overridable notifications.
		/// </summary>
		/// <param name="index">index of set object.</param>
		/// <param name="oldValue">old object value.</param>
		/// <param name="newValue">new object value.</param>
		protected virtual void OnSet(int index, ObjectContainer<T> oldValue, ObjectContainer<T> newValue)
		{
		}
		/// <summary>
		/// overridable notifications.
		/// </summary>
		/// <param name="index">index of set object.</param>
		/// <param name="oldValue">old object value.</param>
		/// <param name="newValue">new object value.</param>
		protected virtual void OnSetComplete(int index, ObjectContainer<T> oldValue, ObjectContainer<T> newValue)
		{
		}
		/// <summary>
		/// overridable notifications.
		/// </summary>
		/// <param name="value">object value to validate</param>
		protected virtual void OnValidate(ObjectContainer<T> value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
		}

		#endregion

		#region Event and Event Raiser

		/// <summary>
		/// Raise event. Thread safe.
		/// </summary>
		/// <param name="del">The delegate.</param>
		/// <param name="args">The arguments</param>
		protected void RaiseEvent(Delegate del, params object[] args)
		{
			ApplicationManager.Instance.Invoke(del, args);
		}
		/// <summary>
		/// RaiseCollectionChanged
		/// </summary>
		protected void RaiseCollectionChanged()
		{
			if (CollectionChanged != null)
			{
				RaiseEvent(CollectionChanged, this, System.EventArgs.Empty);
			}
		}
		/// <summary>
		/// RaisePropertyChanged.
		/// </summary>
		protected internal void RaisePropertyChanged()
		{
			if (PropertyChanged != null)
			{
				RaiseEvent(PropertyChanged, this, System.EventArgs.Empty);
			}
		}
		/// <summary>
		/// CollectionChanged Event.
		/// </summary>
		[Category("Changed")]
		public event System.EventHandler CollectionChanged;
		/// <summary>
		/// PropertyChanged Event.
		/// </summary>
		[Category("Changed")]
		public event System.EventHandler PropertyChanged;

		#endregion

		#region Public Property

		/// <summary>
		/// Gets or sets Is Editable collection.
		/// </summary>
		[Browsable(false)]
		public bool Editable { get { return _editable; } set { _editable = value; } }
		/// <summary>
		/// Gets Base Type.
		/// </summary>
		[Browsable(false)]
		public Type BaseType { get { return _baseType; } }

		#endregion
	}

	#endregion
}

#endregion
