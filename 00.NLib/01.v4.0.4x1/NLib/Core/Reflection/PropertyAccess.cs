#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2010-01-25
=================
- Rewrite code for Property Access and related classes.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;

using NLib.Reflection.Emit;

#endregion

namespace NLib.Reflection
{
	#region Property Access - <<<< Faster but required to generate assembly at runtime need to optimized >>>>

	#region IPropertyAccess

	/// <summary>
	/// The IPropertyAccess interface defines a property accessor.
	/// </summary>
	public interface IPropertyAccess
	{
		/// <summary>
		/// Gets the value stored in the property for the specified target.
		/// </summary>
		/// <param name="target">Object to retrieve the property from.</param>
		/// <returns>Property value.</returns>
		object Get(object target);
		/// <summary>
		/// Sets the value for the property of the specified target.
		/// </summary>
		/// <param name="target">Object to set the property on.</param>
		/// <param name="value">Property value.</param>
		void Set(object target, object value);
	}

	#endregion

	#region IObjectFactory

	/// <summary>
	/// IObjectFactory
	/// </summary>
	public interface IObjectFactory
	{
		/// <summary>
		/// Create new object instance
		/// </summary>
		/// <returns>object instance</returns>
		object Create();
	}

	#endregion

	#region Property Access Factory

	/// <summary>
	/// Property Access Factory
	/// </summary>
	internal sealed class PropertyAccessFactory
	{
		#region Static Method

		#region Create Assembly Assist

		/// <summary>
		/// Create Assembly Assist
		/// </summary>
		/// <param name="type">specificed type to create</param>
		/// <returns>Assembly Assist instance</returns>
		public static AssemblyAssist CreateAssemblyAssist(Type type)
		{
			string assemblyName = type.FullName + ".DynamicAssembly";
			string moduleName = "Module";
			// Create Assembly and set module as same name
			AssemblyAssist assemAssist = new AssemblyAssist(assemblyName, moduleName);

			TypeEx typeEx = new TypeEx(type);

			TypeAssist typeAssist;
			string className = string.Empty;

			// Define class factory type
			className = type.FullName.Replace(".", "_").Replace("+", "_") + "Factory";
			typeAssist = assemAssist.DefineType(className);
			// Add Interface Implement
			typeAssist.TypeBuilder.AddInterfaceImplementation(typeof(IObjectFactory));
			// Add Default Constructor
			typeAssist.TypeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

			EmitCreateCode(typeAssist, type);

			typeAssist.Create(); // Create Type

			System.Reflection.PropertyInfo[] propertyInfos = typeEx.GetProperties();
			if (propertyInfos != null && propertyInfos.Length > 0)
			{
				foreach (System.Reflection.PropertyInfo info in propertyInfos)
				{
					// Define property access type
					className = type.FullName.Replace(".", "_").Replace("+", "_") + info.Name + "Property";
					typeAssist = assemAssist.DefineType(className);
					// Add Interface Implement
					typeAssist.TypeBuilder.AddInterfaceImplementation(typeof(IPropertyAccess));
					// Add Default Constructor
					typeAssist.TypeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

					EmitGetCode(typeAssist, type, info);
					EmitSetCode(typeAssist, type, info);

					typeAssist.Create(); // Create Type
				}
			}
			return assemAssist;
		}

		#endregion

		#region EmitGetCode

		/// <summary>
		/// EmitGetCode
		/// </summary>
		/// <param name="typeAssist">Type Assist instance</param>
		/// <param name="targetType">Target Type</param>
		/// <param name="propertyInfo">Target Property instance</param>
		private static void EmitGetCode(TypeAssist typeAssist, Type targetType, System.Reflection.PropertyInfo propertyInfo)
		{
			// Define a method for the get operation. 
			Type[] getParamTypes = new Type[] { typeof(object) };
			Type getReturnType = typeof(object);

			EmitAssist emit = typeAssist.DefineMethod("Get", MethodAttributes.Public | MethodAttributes.Virtual,
				getReturnType, getParamTypes).Emitter;

			// public int IntValue
			// {
			//		get { return _intValue; }
			//		set { ...; }
			// }
			MethodInfo targetGetMethod = targetType.GetMethod("get_" + propertyInfo.Name);
			if (targetGetMethod != null)
			{
				#region IL code
				/*
				getIL.DeclareLocal(typeof(object));
				getIL.Emit(OpCodes.Ldarg_1);								//Load the first argument 
				//(target object)
				getIL.Emit(OpCodes.Castclass, this.mTargetType);			//Cast to the source type
				getIL.EmitCall(OpCodes.Call, targetGetMethod, null);		//Get the property value

				if(targetGetMethod.ReturnType.IsValueType)
				{
					getIL.Emit(OpCodes.Box, targetGetMethod.ReturnType);	//Box if necessary
				}
				getIL.Emit(OpCodes.Stloc_0);								//Store it
			
				getIL.Emit(OpCodes.Ldloc_0);
				getIL.Emit(OpCodes.Ret);
				*/
				#endregion
				if (targetGetMethod.ReturnType.IsValueType)
				{
					emit
						.DeclareLocal(typeof(object))
						.ldarg_1  // Load the first argument 
						.castclass(targetType) // Cast to the source type
						.call(targetGetMethod) // Get the property value
						.box(targetGetMethod.ReturnType) // Box if necessary
						.stloc_0 // Store it
						.ldloc_0
						.ret();
				}
				else
				{
					emit
						.DeclareLocal(typeof(object))
						.ldarg_1  // Load the first argument 
						.castclass(targetType) // Cast to the source type
						.callvirt(targetGetMethod) // Get the property value
						.stloc_0 // Store it
						.ldloc_0
						.ret();
				}
			}
			else
			{
				#region IL code - raise exception
				/*
				getIL.ThrowException(typeof(MissingMethodException));
				getIL.Emit(OpCodes.Ret);
				*/
				#endregion
				/*
				emit
					.ThrowException(typeof(MissingMethodException))
					.ret();
				*/
				#region IL code - return null
				/*
				// Code size       6 (0x6)
				.maxstack  1
				.locals init ([0] object CS$00000003$00000000)
				IL_0000:  ldnull
				IL_0001:  stloc.0
				IL_0002:  br.s       IL_0004
				IL_0004:  ldloc.0
				IL_0005:  ret
				*/
				#endregion
				emit
					.DeclareLocal(typeof(object))
					.ldnull
					.stloc_0 // Store it
					.ldloc_0
					.ret();
			}
		}

		#endregion

		#region EmitSetCode

		/// <summary>
		/// EmitSetCode
		/// </summary>
		/// <param name="typeAssist">Type Assist instance</param>
		/// <param name="targetType">Target Type</param>
		/// <param name="propertyInfo">Target Property instance</param>
		private static void EmitSetCode(TypeAssist typeAssist, Type targetType, System.Reflection.PropertyInfo propertyInfo)
		{
			// Define a method for the set operation.
			Type[] setParamTypes = new Type[] { typeof(object), typeof(object) };
			Type setReturnType = null;

			EmitAssist emit = typeAssist.DefineMethod("Set", MethodAttributes.Public | MethodAttributes.Virtual,
				setReturnType, setParamTypes).Emitter;

			// public int IntValue
			// {
			//		get { ...; }
			//		set { _intValue = value; }
			// }
			MethodInfo targetSetMethod = targetType.GetMethod("set_" + propertyInfo.Name);
			if (targetSetMethod != null)
			{
				Type paramType = targetSetMethod.GetParameters()[0].ParameterType;
				#region IL code
				/*
				setIL.DeclareLocal(paramType);
				setIL.Emit(OpCodes.Ldarg_1);						//Load the first argument (target object)
				setIL.Emit(OpCodes.Castclass, this.mTargetType);	//Cast to the source type
				setIL.Emit(OpCodes.Ldarg_2);						//Load the second argument (value object)

				if (paramType.IsValueType)
				{
					setIL.Emit(OpCodes.Unbox, paramType);			//Unbox it 	
					if(mTypeHash[paramType]!=null)					//and load
					{
						OpCode load = (OpCode)mTypeHash[paramType];
						setIL.Emit(load);
					}
					else
					{
						setIL.Emit(OpCodes.Ldobj,paramType);
					}
				}
				else
				{
					setIL.Emit(OpCodes.Castclass, paramType);		//Cast class
				}
				setIL.EmitCall(OpCodes.Callvirt, 
					targetSetMethod, null);							//Set the property value
				getIL.Emit(OpCodes.Ret);
				*/
				#endregion
				if (paramType.IsValueType)
				{
					object load = EmitAssist.FindOpCode(paramType);
					if (null != load)
					{
						// OpCode for value type is found
						emit
							.DeclareLocal(paramType) // Load the first argument 
							.ldarg_1 // Load the first argument (target object)
							.castclass(targetType) // Cast to the source type
							.ldarg_2 // Load the second argument (value object)
							.unbox(paramType) // Unbox it 	
							.Emit((OpCode)load) // and load
							.callvirt(targetSetMethod) // Set the property value
							.ret();
					}
					else
					{
						// OpCode for value type is not found
						emit
							.DeclareLocal(paramType) // Load the first argument 
							.ldarg_1 // Load the first argument (target object)
							.castclass(targetType) // Cast to the source type
							.ldarg_2 // Load the second argument (value object)
							.unbox(paramType) // Unbox it 	
							.ldobj(paramType) // load value type object from top of evaluate stack
							.callvirt(targetSetMethod) // Set the property value
							.ret();
					}
				}
				else
				{
					// is not value type
					emit
						.DeclareLocal(paramType) // Load the first argument 
						.ldarg_1 // Load the first argument (target object)
						.castclass(targetType) // Cast to the source type
						.ldarg_2 // Load the second argument (value object)
						.castclass(paramType) // Cast class
						.callvirt(targetSetMethod) // Set the property value
						.ret();
				}
			}
			else
			{
				#region IL code - raise exception
				/*
				getIL.ThrowException(typeof(MissingMethodException));
				getIL.Emit(OpCodes.Ret);
				*/
				#endregion

				/*
				emit
					.ThrowException(typeof(MissingMethodException))
					.ret();
				*/

				#region IL code - blank set property
				/*
				IL_0000:  nop
				IL_0001:  ret
				*/
				#endregion
				emit
					.nop
					.ret();
			}
		}

		#endregion

		#region EmitCreateCode

		/// <summary>
		/// EmitCreateCode
		/// </summary>
		/// <param name="typeAssist">Type Assist instance</param>
		/// <param name="targetType">Target Type</param>
		private static void EmitCreateCode(TypeAssist typeAssist, Type targetType)
		{
			// Define a method for the get operation. 
			Type[] createParamTypes = Type.EmptyTypes;
			Type createReturnType = typeof(object);

			EmitAssist emit = typeAssist.DefineMethod("Create", MethodAttributes.Public | MethodAttributes.Virtual,
				createReturnType, createParamTypes).Emitter;

			// public object Create()
			// {
			//		return new xxx();
			// }
			ConstructorInfo targetCtorMethod = targetType.GetConstructor(createParamTypes);
			if (targetCtorMethod != null)
			{
				emit
					.DeclareLocal(typeof(object))
					.newobj(targetCtorMethod)
					.stloc_0 // Store it
					.ldloc_0
					.ret();
			}
			else
			{
				#region IL code - raise exception
				/*
				getIL.ThrowException(typeof(MissingMethodException));
				getIL.Emit(OpCodes.Ret);
				*/
				#endregion
				/*
				emit
					.ThrowException(typeof(MissingMethodException))
					.ret();
				*/
				#region IL code - return null
				/*
				// Code size       6 (0x6)
				.maxstack  1
				.locals init ([0] object CS$00000003$00000000)
				IL_0000:  ldnull
				IL_0001:  stloc.0
				IL_0002:  br.s       IL_0004
				IL_0004:  ldloc.0
				IL_0005:  ret
				*/
				#endregion
				emit
					.DeclareLocal(typeof(object))
					.ldnull
					.stloc_0 // Store it
					.ldloc_0
					.ret();
			}
		}

		#endregion

		#endregion
	}

	#endregion

	#region __PropertyAccess internal cache

	/// <summary>
	/// __PropertyAccess internal cache
	/// </summary>
	internal class __PropertyAccess
	{
		#region Singelton

		private static __PropertyAccess _instance = null;
		/// <summary>
		/// Singelton Access
		/// </summary>
		public static __PropertyAccess Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (typeof(__PropertyAccess))
					{
						_instance = new __PropertyAccess();
					}
				}
				return _instance;
			}
		}

		#endregion

		#region Internal Varaiable

		private Hashtable _types = new Hashtable();

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor
		/// </summary>
		private __PropertyAccess()
			: base()
		{
			_types = new Hashtable();
		}
		/// <summary>
		/// Destructor
		/// </summary>
		~__PropertyAccess()
		{
			if (_types != null) _types.Clear();
			_types = null;
		}

		#endregion

		#region Public Property

		/// <summary>
		/// Get Type Access via Type
		/// </summary>
		public TypeAccess this[Type type]
		{
			get
			{
				if (!_types.Contains(type))
				{
					lock (this)
					{
						_types.Add(type, TypeAccess.Create(type));
					}
				}
				if (!_types.Contains(type))
					return null;
				else return (TypeAccess)_types[type];
			}
		}

		#endregion
	}

	#endregion

	#region Type Access

	/// <summary>
	/// Type Access
	/// </summary>
	public sealed class TypeAccess
	{
		#region Internal Variable

		private Type _type = null;
		private Hashtable _propertyHash = null;
		private IObjectFactory _objectFactory = null;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="type">target type to create property access list</param>
		private TypeAccess(Type type)
			: base()
		{
			_propertyHash = new Hashtable();

			_type = type;

			AssemblyAssist assemAssist = PropertyAccessFactory.CreateAssemblyAssist(_type);
			Assembly assem = assemAssist.AssemblyBuilder as Assembly;

			TypeEx typeEx = new TypeEx(type);

			string className = string.Empty;

			className = type.FullName.Replace(".", "_").Replace("+", "_") + "Factory";
			_objectFactory = assem.CreateInstance(className) as IObjectFactory;

			PropertyInfo[] props = typeEx.GetProperties();
			if (props != null && props.Length > 0)
			{
				foreach (PropertyInfo prop in props)
				{
					className = _type.FullName.Replace(".", "_").Replace("+", "_") + prop.Name + "Property";
					IPropertyAccess propertyAccess = assem.CreateInstance(className) as IPropertyAccess;
					_propertyHash.Add(prop.Name, PropertyAccess.Create(this, prop, propertyAccess));
				}
			}
		}
		/// <summary>
		/// Destructor
		/// </summary>
		~TypeAccess()
		{
			if (_propertyHash != null) _propertyHash.Clear();
			_propertyHash = null;

			_objectFactory = null;

			_type = null;
		}

		#endregion

		#region Internal Access

		/// <summary>
		/// Object Fattory Instance
		/// </summary>
		internal IObjectFactory Factory { get { return _objectFactory; } }

		#endregion

		#region Public Property

		/// <summary>
		/// Get Target Type
		/// </summary>
		public Type TargetType { get { return _type; } }
		/// <summary>
		/// Get PropertyAccess Via Indexer
		/// </summary>
		public PropertyAccess this[string propertyName]
		{
			get
			{
				if (!_propertyHash.Contains(propertyName))
					return null;
				else return (PropertyAccess)_propertyHash[propertyName];
			}
		}

		#endregion

		#region Static Method

		/// <summary>
		/// Create new Type Access
		/// </summary>
		/// <param name="type">Type to Create</param>
		/// <returns>new Type Access instance</returns>
		internal static TypeAccess Create(Type type)
		{
			return new TypeAccess(type);
		}
		/// <summary>
		/// Create New Instance for specificed type
		/// </summary>
		/// <param name="type">specificed type</param>
		/// <returns>New Instance for specificed type</returns>
		public static object CreateInstance(Type type)
		{
			if (type == null)
				return null;
			TypeAccess typeAccess = __PropertyAccess.Instance[type];
			if (typeAccess == null || typeAccess.Factory == null)
				return null;
			else
			{
				return typeAccess.Factory.Create();
			}
		}

		#endregion
	}

	#endregion

	#region Property Access

	/// <summary>
	/// Property Access
	/// </summary>
	public sealed class PropertyAccess
	{
		#region Internal Variable

		private TypeAccess _parentType = null;
		private IPropertyAccess _propertyAccess = null;
		private PropertyInfo _propertyInfo = null;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="parentType">Parent Type Access instance</param>
		/// <param name="propertyInfo">PropertyInfo instance</param>
		/// <param name="propertyAccess">IPropertyAccess instance</param>
		private PropertyAccess(TypeAccess parentType, PropertyInfo propertyInfo, IPropertyAccess propertyAccess)
		{
			_parentType = parentType;
			_propertyInfo = propertyInfo;
			_propertyAccess = propertyAccess;
		}
		/// <summary>
		/// Destructor
		/// </summary>
		~PropertyAccess()
		{
			_propertyAccess = null;
			_propertyInfo = null;
			_parentType = null;
		}

		#endregion

		#region Public Method

		/// <summary>
		/// Get Property Value
		/// </summary>
		/// <param name="target">target object</param>
		/// <returns>value that stored in specificed property</returns>
		public object Get(object target)
		{
			if (_propertyAccess == null)
				return null;
			else return _propertyAccess.Get(target);
		}
		/// <summary>
		/// Get Property Value
		/// </summary>
		/// <param name="target">target object</param>
		/// <param name="value">value to assigned</param>
		public void Set(object target, object value)
		{
			if (_propertyAccess == null)
				return;
			_propertyAccess.Set(target, value);
		}

		#endregion

		#region Public Property

		/// <summary>
		/// Whether or not the Property supports write access.
		/// </summary>
		public bool CanWrite { get { return _propertyInfo.CanWrite; } }
		/// <summary>
		/// Whether or not the Property supports read access.
		/// </summary>
		public bool CanRead { get { return _propertyInfo.CanRead; } }
		/// <summary>
		/// The Type of object this property accessor was created for.
		/// </summary>
		public Type TargetType { get { return this._parentType.TargetType; } }
		/// <summary>
		/// The Type of the Property being accessed.
		/// </summary>
		public Type PropertyType { get { return _propertyInfo.PropertyType; } }

		#endregion

		#region Static Method

		/// <summary>
		/// Create new Property Access instance
		/// </summary>
		/// <param name="parentType">Parent Type Access instance</param>
		/// <param name="propertyInfo">PropertyInfo instance</param>
		/// <param name="propertyAccess">IPropertyAccess instance</param>
		/// <returns>new Property Access instance</returns>
		internal static PropertyAccess Create(TypeAccess parentType, PropertyInfo propertyInfo, IPropertyAccess propertyAccess)
		{
			return new PropertyAccess(parentType, propertyInfo, propertyAccess);
		}
		/// <summary>
		/// Get Type Access
		/// </summary>
		/// <param name="type">target type</param>
		/// <returns>Type Access Instance</returns>
		public static TypeAccess GetTypeAccess(Type type)
		{
			return __PropertyAccess.Instance[type];
		}
		/// <summary>
		/// Get Property Access
		/// </summary>
		/// <param name="type">Target Type</param>
		/// <param name="propertyName">Proeprty's Name</param>
		/// <returns>Property Access Instance</returns>
		public static PropertyAccess GetProperty(Type type, string propertyName)
		{
			if (type == null)
				return null;
			TypeAccess typeAccess = GetTypeAccess(type);
			if (typeAccess == null)
				return null;
			else
				return typeAccess[propertyName];
		}
		/// <summary>
		/// Can Read or Get
		/// </summary>
		/// <param name="target">Target object</param>
		/// <param name="propertyName">Proeprty's Name</param>
		/// <returns>true if target property can read</returns>
		public static bool CanGet(object target, string propertyName)
		{
			if (target == null)
				return false;
			Type type = target.GetType();
			PropertyAccess propAccess = GetProperty(type, propertyName);

			if (propAccess == null)
				return false;
			return propAccess.CanRead;
		}
		/// <summary>
		/// Can Write or Set
		/// </summary>
		/// <param name="target">Target object</param>
		/// <param name="propertyName">Proeprty's Name</param>
		/// <returns>true if target property can write</returns>
		public static bool CanSet(object target, string propertyName)
		{
			if (target == null)
				return false;
			Type type = target.GetType();
			PropertyAccess propAccess = GetProperty(type, propertyName);

			if (propAccess == null)
				return false;
			return propAccess.CanWrite;
		}
		/// <summary>
		/// Get Property Value
		/// </summary>
		/// <param name="target">Target object</param>
		/// <param name="propertyName">Proeprty's Name</param>
		/// <returns>value that stored in object</returns>
		public static object GetValue(object target, string propertyName)
		{
			if (target == null)
				return null;
			Type type = target.GetType();

			PropertyAccess propAccess = GetProperty(type, propertyName);

			if (propAccess == null)
				return null;
			return propAccess.Get(target);
		}
		/// <summary>
		/// Set Property Value
		/// </summary> 
		/// <param name="target">Target object</param>
		/// <param name="propertyName">Proeprty's Name</param>
		/// <param name="value">value to set</param>
		public static void SetValue(object target, string propertyName, object value)
		{
			if (target == null)
				return;
			Type type = target.GetType();

			PropertyAccess propAccess = GetProperty(type, propertyName);

			if (propAccess == null)
				return;
			propAccess.Set(target, value);
		}

		#endregion
	}

	#endregion

	#endregion
}