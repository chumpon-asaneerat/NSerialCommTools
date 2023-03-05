#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-10-13
=================
- Reflection Framework ported
  - DynamicAccess is ported from NLib v4.0.3.7 to NLib v4.0.4.1.

======================================================================================================================
Update 2014-05-18
=================
- Reflection Framework ported
  - DynamicAccess is ported from NLib v4.0.3.5 to NLib v4.0.3.7.

======================================================================================================================
Update 2011-11-07
=================
- Reflection Framework ported
  - DynamicAccess is ported from GFA4.0 to NLib.

======================================================================================================================
Update 2010-04-26
=================
- Reflection Framework updated
  - DynamicAccess add check null object access.

======================================================================================================================
Update 2010-01-08
=================
- Reflection Framework updated
  - DynamicAccess and related classes ported. Ports DynamicAccess classes from GFA37 to GFA38.
  - Change return Type in DynamicAccess<T>.Create (Generic version) now supports automatic 
    casting to target type.

======================================================================================================================
Update 2008-08-08
=================
- DynamicAccess. Remove BindingFlag ExactBinding

======================================================================================================================
Update 2007-11-26
=================
- Reflection Framework optimized to used relection API in high performance and high speed.
  see TypeEx, TypeUtils, TypeAccess, DynamicAccess, AssemblyAssist, TypeAssist, MethodAssist, 
  ConstructorAssist and EmitAssist.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion

namespace NLib.Reflection
{
    #region Cache

    /// <summary>
    /// Cache for Dynamic IL access
    /// </summary>
    static class Cache
    {
        #region Delegate

        public delegate object Constructor();
        public delegate object ParamterisedConstructor(object[] Params);
        public delegate object Get(object Instance);
        public delegate void Set(object Instance, object Value);
        public delegate object Method(object Instance, object[] Params);

        #endregion

        #region Imediate Language (IL) wrapper clase

        /// <summary>
        /// Imediate Language (IL) wrapper clase
        /// </summary>
        public class IL
        {
            #region Internal Variable

            private Type _type;

            private Constructor _constructor = null;
            private Dictionary<string, ParamterisedConstructor> _parameterisedConstructors = null;
            private Dictionary<string, Get> _getProperties = null;
            private Dictionary<string, Set> _setProperties = null;
            private Dictionary<string, Method> _methods = null;
            private Dictionary<string, Get> _staticGetProperties = null;
            private Dictionary<string, Set> _staticSetProperties = null;
            private Dictionary<string, Method> _staticMethods = null;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="t">Target Type</param>
            internal IL(Type t)
            {
                _type = t;
            }

            #endregion

            #region Private Method for Generate Emit

            /// <summary>
            /// EmitCastToReference
            /// </summary>
            /// <param name="il">ILGenerator instance</param>
            /// <param name="type">Type to cast</param>
            private static void EmitCastToReference(ILGenerator il, System.Type type)
            {
                if (null == il || null == type)
                    return;
                if (type.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, type);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, type);
                }
            }
            /// <summary>
            /// EmitBoxIfNeeded
            /// </summary>
            /// <param name="il">ILGenerator instance</param>
            /// <param name="type">Type to cast</param>
            private static void EmitBoxIfNeeded(ILGenerator il, System.Type type)
            {
                if (null == il || null == type)
                    return;
                if (type.IsValueType)
                {
                    il.Emit(OpCodes.Box, type);
                }
            }
            /// <summary>
            /// EmitFastInt
            /// </summary>
            /// <param name="il">ILGenerator instance</param>
            /// <param name="value">int value</param>
            private static void EmitFastInt(ILGenerator il, int value)
            {
                if (null == il)
                    return;
                // for small integers, emit the proper opcode
                switch (value)
                {
                    case -1:
                        il.Emit(OpCodes.Ldc_I4_M1);
                        return;
                    case 0:
                        il.Emit(OpCodes.Ldc_I4_0);
                        return;
                    case 1:
                        il.Emit(OpCodes.Ldc_I4_1);
                        return;
                    case 2:
                        il.Emit(OpCodes.Ldc_I4_2);
                        return;
                    case 3:
                        il.Emit(OpCodes.Ldc_I4_3);
                        return;
                    case 4:
                        il.Emit(OpCodes.Ldc_I4_4);
                        return;
                    case 5:
                        il.Emit(OpCodes.Ldc_I4_5);
                        return;
                    case 6:
                        il.Emit(OpCodes.Ldc_I4_6);
                        return;
                    case 7:
                        il.Emit(OpCodes.Ldc_I4_7);
                        return;
                    case 8:
                        il.Emit(OpCodes.Ldc_I4_8);
                        return;
                }

                // for bigger values emit the short or long opcode
                if (value > -129 && value < 128)
                {
                    il.Emit(OpCodes.Ldc_I4_S, (SByte)value);
                }
                else
                {
                    il.Emit(OpCodes.Ldc_I4, value);
                }
            }

            #endregion

            #region Internal IL generators

            #region Get Method Invoker

            /// <summary>
            /// Get Method Invoker
            /// </summary>
            /// <param name="methodInfo">Method info instance</param>
            /// <param name="paramTypes">Type of parameters</param>
            /// <returns>Method delegate</returns>
            public Method MethodInvoker(MethodInfo methodInfo, Type[] paramTypes)
            {
                if (methodInfo == null)
                    return null;

                try
                {
                    // Determine the key
                    String signature = methodInfo.Name;
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    //Type[] paramTypes = new Type[parameters.Length];

                    if (paramTypes != null && paramTypes.Length > 0)
                    {
                        String[] sParams = new String[paramTypes.Length];
                        for (int i = 0; i < parameters.Length; ++i)
                        {
                            sParams[i] = paramTypes[i].Name;
                        }
                        signature += String.Format("({0})", String.Join(",", sParams));
                    }
                    Dictionary<string, Method> cache;
                    // The the IL is already cached, then return it
                    if (methodInfo.IsStatic)
                    {
                        if (_staticMethods == null)
                            _staticMethods = new Dictionary<string, Method>();
                        else if (_staticMethods.ContainsKey(signature))
                            return _staticMethods[signature];
                        cache = _staticMethods;
                    }
                    else
                    {
                        if (_methods == null)
                            _methods = new Dictionary<string, Method>();
                        else if (_methods.ContainsKey(signature))
                            return _methods[signature];
                        cache = _methods;
                    }
                    // Generate the IL for the method
                    DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType.Module);
                    ILGenerator il = dynamicMethod.GetILGenerator();

                    int len = (paramTypes == null) ? 0 : paramTypes.Length;
                    LocalBuilder[] locals = new LocalBuilder[len];
                    if (paramTypes != null)
                    {
                        // generates a local variable for each parameter
                        for (int i = 0; i < paramTypes.Length; i++)
                        {
                            locals[i] = il.DeclareLocal(paramTypes[i], true);
                        }
                        // creates code to copy the parameters to the local variables
                        for (int i = 0; i < paramTypes.Length; i++)
                        {
                            il.Emit(OpCodes.Ldarg_1);
                            EmitFastInt(il, i);
                            il.Emit(OpCodes.Ldelem_Ref);
                            EmitCastToReference(il, paramTypes[i]);
                            il.Emit(OpCodes.Stloc, locals[i]);
                        }
                    }
                    if (!methodInfo.IsStatic)
                    {
                        // loads the object into the stack
                        il.Emit(OpCodes.Ldarg_0);
                    }

                    if (paramTypes != null)
                    {
                        // loads the parameters copied to the local variables into the stack
                        for (int i = 0; i < paramTypes.Length; i++)
                        {
                            if (parameters[i].ParameterType.IsByRef)
                                il.Emit(OpCodes.Ldloca_S, locals[i]);
                            else
                                il.Emit(OpCodes.Ldloc, locals[i]);
                        }
                    }

                    // calls the method
                    if (!methodInfo.IsStatic)
                    {
                        il.EmitCall(OpCodes.Callvirt, methodInfo, null);
                    }
                    else
                    {
                        il.EmitCall(OpCodes.Call, methodInfo, null);
                    }

                    // creates code for handling the return value
                    if (methodInfo.ReturnType == typeof(void))
                    {
                        il.Emit(OpCodes.Ldnull);
                    }
                    else
                    {
                        EmitBoxIfNeeded(il, methodInfo.ReturnType);
                    }

                    if (paramTypes != null)
                    {
                        // iterates through the parameters updating the parameters passed by ref
                        for (int i = 0; i < paramTypes.Length; i++)
                        {
                            if (parameters[i].ParameterType.IsByRef)
                            {
                                il.Emit(OpCodes.Ldarg_1);
                                EmitFastInt(il, i);
                                il.Emit(OpCodes.Ldloc, locals[i]);
                                if (locals[i].LocalType.IsValueType)
                                    il.Emit(OpCodes.Box, locals[i].LocalType);
                                il.Emit(OpCodes.Stelem_Ref);
                            }
                        }
                    }

                    // returns the value to the caller
                    il.Emit(OpCodes.Ret);

                    // converts the DynamicMethod to a FastInvokeHandler delegate to call to the method
                    Method invoker = (Method)dynamicMethod.CreateDelegate(typeof(Method));
                    cache.Add(signature, invoker);
                    return (Method)invoker;
                }
                catch (Exception)
                {
                    //throw (ex);
                    return null;
                }
            }

            #endregion

            #region Get Property Get/Set Invoker

            /// <summary>
            /// Get Property Get Invoker
            /// </summary>
            /// <param name="propInfo">PropertyInfo instance</param>
            /// <returns>Get Invoker delegate</returns>
            public Get PropertyGetInvoker(PropertyInfo propInfo)
            {
                if (propInfo == null)
                    return null;
                try
                {
                    MethodInfo methodInfo = propInfo.GetGetMethod();
                    Dictionary<string, Get> cache;
                    if (null == methodInfo)
                        return null;

                    if (methodInfo.IsStatic)
                    {
                        if (_staticGetProperties == null)
                            _staticGetProperties = new Dictionary<string, Get>();
                        else if (_staticGetProperties.ContainsKey(propInfo.Name))
                            return _staticGetProperties[propInfo.Name];
                        cache = _staticGetProperties;
                    }
                    else
                    {
                        if (_getProperties == null)
                            _getProperties = new Dictionary<string, Get>();
                        else if (_getProperties.ContainsKey(propInfo.Name))
                            return _getProperties[propInfo.Name];
                        cache = _getProperties;
                    }
                    DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object) }, propInfo.DeclaringType.Module);
                    ILGenerator il = dynamicMethod.GetILGenerator();
                    if (!methodInfo.IsStatic)
                    {
                        il.Emit(OpCodes.Ldarg_0);
                        il.EmitCall(OpCodes.Callvirt, methodInfo, null);
                    }
                    else
                        il.EmitCall(OpCodes.Call, methodInfo, null);
                    EmitBoxIfNeeded(il, propInfo.PropertyType);
                    il.Emit(OpCodes.Ret);
                    Get invoker = (Get)dynamicMethod.CreateDelegate(typeof(Get));
                    cache.Add(propInfo.Name, invoker);
                    return invoker;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            /// <summary>
            /// Get Property Set Invoker
            /// </summary>
            /// <param name="propInfo">PropertyInfo instance</param>
            /// <returns>Set Invoker delegate</returns>
            public Set PropertySetInvoker(PropertyInfo propInfo)
            {
                if (propInfo == null)
                    return null;

                try
                {
                    MethodInfo methodInfo = propInfo.GetSetMethod();
                    Dictionary<string, Set> cache;
                    if (null == methodInfo)
                        return null;

                    if (methodInfo.IsStatic)
                    {
                        if (_staticSetProperties == null)
                            _staticSetProperties = new Dictionary<string, Set>();
                        else if (_staticSetProperties.ContainsKey(propInfo.Name))
                            return _staticSetProperties[propInfo.Name];
                        cache = _staticSetProperties;
                    }
                    else
                    {
                        if (_setProperties == null)
                            _setProperties = new Dictionary<string, Set>();
                        else if (_setProperties.ContainsKey(propInfo.Name))
                            return _setProperties[propInfo.Name];
                        cache = _setProperties;
                    }

                    DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, null,
                        new Type[] { typeof(object), typeof(object) }, propInfo.DeclaringType.Module);

                    ILGenerator il = dynamicMethod.GetILGenerator();

                    if (!methodInfo.IsStatic)
                    {
                        il.Emit(OpCodes.Ldarg_0);
                    }

                    il.Emit(OpCodes.Ldarg_1);

                    EmitCastToReference(il, propInfo.PropertyType);
                    if (methodInfo.IsStatic)
                        il.EmitCall(OpCodes.Call, methodInfo, null);
                    else
                        il.EmitCall(OpCodes.Callvirt, methodInfo, null);

                    il.Emit(OpCodes.Ret);

                    Set invoker = (Set)dynamicMethod.CreateDelegate(typeof(Set));
                    cache.Add(propInfo.Name, invoker);
                    return invoker;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            #endregion

            #region Get Constructor

            /// <summary>
            /// Get Constructor Invoker
            /// </summary>
            /// <returns>Constructor delegate</returns>
            public Constructor GetConstructorInvoker()
            {
                try
                {
                    if (_constructor == null)
                    {
                        ConstructorInfo info = _type.GetConstructor(Type.EmptyTypes);
                        if (null == info)
                            return null;
                        DynamicMethod dynamicMethod =
                            new DynamicMethod(string.Empty, _type, new Type[0], info.DeclaringType.Module);
                        ILGenerator il = dynamicMethod.GetILGenerator();
                        il.Emit(OpCodes.Newobj, info);
                        il.Emit(OpCodes.Ret);
                        _constructor = (Constructor)dynamicMethod.CreateDelegate(typeof(Constructor));
                    }
                    return _constructor;
                }
                catch (Exception)
                {
                    // Except occur
                    return null;
                }
            }
            /// <summary>
            /// Get Constructor Invoker
            /// </summary>
            /// <param name="Params">Constructor's parameters</param>
            /// <returns>Paramterised Constructor delegate</returns>
            public ParamterisedConstructor GetConstructorInvoker(object[] Params)
            {
                if (Params == null)
                    return null; // invalid call with null parameters

                try
                {
                    Type[] paramTypes = new Type[Params.Length];
                    string[] sParamTypes = new string[Params.Length];

                    for (int i = 0; i < Params.Length; ++i)
                    {
                        paramTypes[i] = Params[i].GetType();
                        sParamTypes[i] = paramTypes[i].ToString();
                    }

                    string signature = _type.Name + "(" + string.Join(",", sParamTypes) + ")";
                    if (_parameterisedConstructors == null)
                    {
                        _parameterisedConstructors =
                            new Dictionary<string, ParamterisedConstructor>();
                    }
                    else if (_parameterisedConstructors.ContainsKey(signature))
                    {
                        return _parameterisedConstructors[signature];
                    }

                    ConstructorInfo info = _type.GetConstructor(paramTypes);
                    if (null == info)
                        return null;
                    DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, _type,
                        new Type[] { typeof(object), typeof(object) }, info.DeclaringType.Module);

                    ILGenerator il = dynamicMethod.GetILGenerator();

                    // Copy the paramete4rs into local variables
                    LocalBuilder[] locals = new LocalBuilder[paramTypes.Length];

                    // generates a local variable for each parameter
                    for (int i = 0; i < paramTypes.Length; i++)
                    {
                        locals[i] = il.DeclareLocal(paramTypes[i], true);
                    }


                    // creates code to copy the parameters to the local variables
                    for (int i = 0; i < paramTypes.Length; i++)
                    {
                        il.Emit(OpCodes.Ldarg_1);
                        EmitFastInt(il, i);
                        il.Emit(OpCodes.Ldelem_Ref);
                        EmitCastToReference(il, paramTypes[i]);
                        il.Emit(OpCodes.Stloc, locals[i]);
                    }
                    for (int i = 0; i < paramTypes.Length; i++)
                    {
                        il.Emit(OpCodes.Ldloc, locals[i]);
                    }
                    // generates code to create a new object of the specified type using the specified constructor
                    il.Emit(OpCodes.Newobj, info);
                    // returns the value to the caller
                    il.Emit(OpCodes.Ret);
                    // converts the DynamicMethod to a FastCreateInstanceHandler delegate to create the object
                    ParamterisedConstructor invoker =
                        (ParamterisedConstructor)dynamicMethod.CreateDelegate(typeof(ParamterisedConstructor));
                    _parameterisedConstructors.Add(signature, invoker);

                    return invoker;
                }
                catch (Exception)
                {
                    // Except occur
                    return null;
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region Static Method

        private static Dictionary<Type, IL> _cache = new Dictionary<Type, IL>();

        #region Create

        /// <summary>
        /// Create new instance of Type
        /// </summary>
        /// <param name="t">Target Type</param>
        /// <returns>new instance of target type</returns>
        public static object Create(Type t)
        {
            if (t == null)
                return null;
            IL code;
            if (!_cache.ContainsKey(t))
                _cache.Add(t, code = new IL(t));
            else
                code = _cache[t];
            Constructor obj = code.GetConstructorInvoker();
            if (obj == null)
                return null;
            else return obj();
        }
        /// <summary>
        /// Create new instance of Type
        /// </summary>
        /// <param name="t">Target Type</param>
        /// <param name="Params"></param>
        /// <returns>new instance of target type</returns>
        public static object Create(Type t, object[] Params)
        {
            if (t == null)
                return null;
            IL code;
            if (!_cache.ContainsKey(t))
                _cache.Add(t, code = new IL(t));
            else
                code = _cache[t];

            ParamterisedConstructor obj = code.GetConstructorInvoker(Params);
            if (obj == null)
                return null;
            else return obj(Params);
        }

        #endregion

        #region Get/Set Property

        /// <summary>
        /// Get Property
        /// </summary>
        /// <param name="t">Target Type</param>
        /// <param name="instance">Object instance</param>
        /// <param name="info">PropertyInfo instance</param>
        /// <returns>value that get from specificed property</returns>
        public static object GetProperty(Type t, object instance, PropertyInfo info)
        {
            IL code;
            if (_cache.ContainsKey(t))
                code = _cache[t];
            else
                _cache.Add(t, code = new IL(t));
            Get obj = code.PropertyGetInvoker(info);
            if (obj == null)
                return null;
            else return obj(instance);
        }
        /// <summary>
        /// Set Property
        /// </summary>
        /// <param name="t">Target Type</param>
        /// <param name="instance">Object instance</param>
        /// <param name="info">PropertyInfo instance</param>
        /// <param name="Value">value that set to specificed property</param>
        public static void SetProperty(Type t, object instance, PropertyInfo info, object Value)
        {
            IL code;
            if (_cache.ContainsKey(t))
                code = _cache[t];
            else
                _cache.Add(t, code = new IL(t));
            Set obj = code.PropertySetInvoker(info);

            //if (obj != null) obj(instance, Value); // old code.

            if (info.PropertyType.IsValueType && null == Value)
            {
                try
                {
                    object tVal = Activator.CreateInstance(info.PropertyType);
                    if (obj != null) obj(instance, tVal);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                object tVal;
                try
                {
                    tVal = Convert.ChangeType(Value, info.PropertyType);
                }
                catch (Exception ex1)
                {
                    Console.WriteLine(ex1);
                    tVal = Activator.CreateInstance(info.PropertyType);
                }

                try
                {
                    if (obj != null) obj(instance, tVal);
                }
                catch (Exception ex2)
                {
                    Console.WriteLine(ex2);
                }
            }
        }

        #endregion

        #region Call Method

        /// <summary>
        /// Call Method
        /// </summary>
        /// <param name="t">Target Type</param>
        /// <param name="instance">Object instance</param>
        /// <param name="info">MethodInfo instance</param>
        /// <param name="ParamTypes">Type of Parameters</param>
        /// <param name="Params">Value of Parameters</param>
        /// <returns>result that return from specificed method.</returns>
        public static object CallMethod(Type t, object instance, MethodInfo info, Type[] ParamTypes, object[] Params)
        {
            IL code;
            if (_cache.ContainsKey(t))
                code = _cache[t];
            else
                _cache.Add(t, code = new IL(t));

            Method obj = code.MethodInvoker(info, ParamTypes);
            if (obj == null)
                return null;
            else return obj(instance, Params);
        }

        #endregion

        #endregion
    }

    #endregion
}

namespace NLib.Reflection
{
    // Note: by Joe.
    // The MethodInfo and PropertyInfo that get from GetProperty or GetMethod will not
    // raise exception evenif the method or proeprty's name is empty string so do not need to 
    // add check code for the name.

    #region DynamicAccess

    /// <summary>
    /// Encapsulates a dynamic access instance of an object
    /// </summary>
    /// <remarks>
    /// This class is used as an alternative to the System.Reflection classes and methods
    /// to access a Type's properties and Methods at runtime, based on data retrieved from
    /// runtime configuration. It generates and cache's they Type's MSIL code at runtime, 
    /// providing much more efficient access to the object's members than Reflection can.
    /// <para>
    /// An instance of this class can be used to construct an instance of the dynamic access class,
    /// and/or to call both either instance or static methods on the class. This is particularly 
    /// useful when all you have is an interface returned from some unknown plug-in, but the plug-in
    /// requires other properties in order to operate correctly.
    /// </para>
    /// </remarks>
    public sealed class DynamicAccess
    {
        #region Internal Variable

        private object _instance;
        private Type _type;

        #endregion

        #region Constructor

        /// <summary>
        /// Dynamic Access object constructor, when only the type of the object required is known.
        /// </summary>
        /// <param name="t">The type of object the caller requires an new instance of.</param>
        /// <remarks>
        /// This constructor will generate and call the IL required to create an instance of an
        /// object of Type t. This call is considerably faster than using reflection to perform
        /// the same job, particularly as the generated IL is cached to reuse whenever another
        /// instance of the object is required.
        /// <example>
        /// <code>
        /// Type t;
        /// 
        /// t = SomeType;
        /// DynamicAccess someType = new DynamicAccess(t);
        /// </code>
        /// </example>
        /// </remarks>
        public DynamicAccess(Type t)
        {
            _type = t;
            _instance = Cache.Create(t);
        }
        /// <summary>
        /// DynamicAccess object parameterised constructor, where the unknown object's constructor
        /// requires parameters.
        /// </summary>
        /// <param name="t">The type of object the caller requires an instance of</param>
        /// <param name="Params">Parameters reuired for type t's constructor</param>
        /// <remarks>
        /// This constructor will generate and call the IL required to create an instance of an
        /// object of Type t, passing the supplied parameters to the appropriate constructor. 
        /// This call is considerably faster than using reflection to perform the same job,
        /// particularly as the generated IL is cached to reuse whenever another
        /// instance of the object is required.
        /// <para>
        /// Note that the types of the parameters passed into this constructor must match exactly
        /// the signature of required object's constructor parameters, otherwise the call will fail.
        /// </para>
        /// <para>
        /// This type of constructor is particularly useful when it is known that a type that 
        /// implements a known interface takes a parameterised constructor. The follwing example 
        /// constructs an object that implements IMyPlugin where the Type has been retrieved from
        /// an attribute on the class. It has been agreed that the plugins will be able to accept
        /// a configuration file's path as a constructor parameter, in order to initialise its 
        /// custom properties.
        /// <example>
        /// <code>
        /// Attribute attr = Attribute.GetCustomAttribute(MyPluginAssembly, typeof(PluginImplementationAttribute));
        /// Type t = attr.PluginImplentationType;
        /// String path = MySettingsPath;
        /// 
        /// DynamicAccess MyPluginInstance = new DynamicAccess(t, path);
        /// IMyPlugin iPlugin = (IMyPlugin)MyPluginInstance.Instance;
        /// </code>
        /// </example>
        /// </para>
        /// </remarks>
        public DynamicAccess(Type t, params object[] Params)
        {
            _type = t;
            _instance = Cache.Create(t, Params);
        }
        /// <summary>
        /// Creates a DynamicAccess object from an existing instance of the type
        /// </summary>
        /// <param name="Instance">The object you want to call Latebound methods and properties on</param>
        /// <remarks>
        /// This type of latebound constructor is usefule when a plugin returns an interface pointer
        /// to a constructed object. The resulting latebound object will then allow the caller to
        /// call other properties and methods on the object that are not included in the interface.
        /// <example>
        /// <code>
        /// IMyPlugin myPlugin = PluginManager.CreateInstance("MyPluginName");
        /// DynamicAccess myLateboundInstance = new DynamicAccess(myPlugin);
        /// 
        /// object myPluginProperty = myLateboundInstance["ThePropertyName"];
        /// </code>
        /// </example>
        /// </remarks>
        public DynamicAccess(object Instance)
        {
            _instance = Instance;
            _type = Instance.GetType();
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Get/Set Property's value by specificed Property Name
        /// </summary>
        /// <param name="PropertyName">specificed Property Name to get/set value</param>
        /// <returns>value that stored in specificed property's name</returns>
        public object this[string PropertyName]
        {
            get
            {
                return Cache.GetProperty(_type, _instance, _type.GetProperty(PropertyName));
            }
            set
            {
                Cache.SetProperty(_type, _instance, _type.GetProperty(PropertyName), value);
            }
        }
        /// <summary>
        /// Call Method
        /// </summary>
        /// <param name="MethodName">target method's name</param>
        /// <param name="Params">parameter for specificed method</param>
        /// <returns>value that return from specificed method.</returns>
        public object Call(string MethodName, params object[] Params)
        {
            MethodInfo info;
            Type[] types = null;
            if (Params != null && Params.Length > 0)
            {
                types = new Type[Params.Length];
                for (int i = 0; i < Params.Length; ++i)
                    types[i] = Params[i].GetType();
                info = _type.GetMethod(MethodName,
                    //BindingFlags.NonPublic | BindingFlags.ExactBinding | BindingFlags.Public | BindingFlags.Instance,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                    null, types, null);
            }
            else
            {
                info = _type.GetMethod(MethodName,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            }
            return Cache.CallMethod(_type, _instance, info, types, Params);
        }
        /// <summary>
        /// Get Object instance
        /// </summary>
        public object Instance { get { return _instance; } }

        #endregion

        #region Static Method

        /// <summary>
        /// Get Value from Property
        /// </summary>
        /// <param name="instance">Object Instance</param>
        /// <param name="PropertyName">Property's Name</param>
        /// <returns>value that contain in specificed property</returns>
        public static object Get(object instance, string PropertyName)
        {
            if (instance == null)
                return null;
            Type t = instance.GetType();
            return Cache.GetProperty(t, instance, t.GetProperty(PropertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
        }
        /// <summary>
        /// Set Value to Property
        /// </summary>
        /// <param name="instance">Object Instance</param>
        /// <param name="PropertyName">Property's Name</param>
        /// <param name="Value">value to assigned to property</param>
        public static void Set(object instance, string PropertyName, object Value)
        {
            if (instance == null)
                return;
            Type t = instance.GetType();
            Cache.SetProperty(t, instance, t.GetProperty(PropertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic), Value);
        }
        /// <summary>
        /// Call Method
        /// </summary>
        /// <param name="instance">Object Instance</param>
        /// <param name="MethodName">target method's name</param>
        /// <param name="Params">parameter for specificed method</param>
        /// <returns>value that return from specificed method.</returns>
        public static object Call(object instance, string MethodName, params object[] Params)
        {
            if (instance == null)
                return null;

            Type t = instance.GetType();

            MethodInfo info;
            Type[] types = null;
            if (Params != null && Params.Length > 0)
            {
                types = new Type[Params.Length];
                for (int i = 0; i < Params.Length; ++i)
                    types[i] = Params[i].GetType();
                info = t.GetMethod(MethodName,
                    //BindingFlags.NonPublic | BindingFlags.ExactBinding | BindingFlags.Public | BindingFlags.Instance,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                    null, types, null);
            }
            else
            {
                info = t.GetMethod(MethodName,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            }
            return Cache.CallMethod(t, instance, info, types, Params);
        }
        /// <summary>
        /// Call Static Method
        /// </summary>
        /// <param name="t">Target Type</param>
        /// <param name="MethodName">Static Method's Name</param>
        /// <param name="Params">Method Parameters</param>
        /// <returns>value that return from method</returns>
        public static object CallStatic(Type t, string MethodName, params object[] Params)
        {
            MethodInfo info;
            Type[] types = null;
            if (Params != null && Params.Length > 0)
            {
                types = new Type[Params.Length];
                for (int i = 0; i < Params.Length; ++i)
                    types[i] = Params[i].GetType();
                info = t.GetMethod(MethodName,
                    //BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.ExactBinding,
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                    null, types, null);
            }
            else
            {
                info = t.GetMethod(MethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }
            return Cache.CallMethod(t, null, info, types, Params);
        }
        /// <summary>
        /// Get Value from Static Property
        /// </summary>
        /// <param name="t">Target Type</param>
        /// <param name="PropertyName">Property's Name</param>
        /// <returns>value that contain in specificed property</returns>
        public static object GetStatic(Type t, string PropertyName)
        {
            return Cache.GetProperty(t, null, t.GetProperty(PropertyName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
        }
        /// <summary>
        /// Set Value to Static Property
        /// </summary>
        /// <param name="t">Target Type</param>
        /// <param name="PropertyName">Property's Name</param>
        /// <param name="Value">value to assigned to property</param>
        public static void SetStatic(Type t, string PropertyName, object Value)
        {
            Cache.SetProperty(t, null, t.GetProperty(PropertyName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), Value);
        }
        /// <summary>
        /// Create New instance
        /// </summary>
        /// <param name="t">Type of object</param>
        /// <returns>return new object instance</returns>
        public static object Create(Type t)
        {
            return Cache.Create(t);
        }
        /// <summary>
        /// Create New instance
        /// </summary>
        /// <param name="t">Type of object</param>
        /// <param name="Params">constructor parameters</param>
        /// <returns>return new object instance</returns>
        public static object Create(Type t, params object[] Params)
        {
            return Cache.Create(t, Params);
        }

        #endregion
    }

    #endregion

    #region DynamicAccess - Generic Version

    /// <summary>
    /// Encapsulates a dynamic access instance of an object. Generic Version
    /// </summary>
    /// <typeparam name="T">Target Type</typeparam>
    /// <remarks>
    /// This class is used as an alternative to the System.Reflection classes and methods
    /// to access a Type's properties and Methods at runtime, based on data retrieved from
    /// runtime configuration. It generates and cache's they Type's MSIL code at runtime, 
    /// providing much more efficient access to the object's members than Reflection can.
    /// <para>
    /// An instance of this class can be used to construct an instance of the dynamic access class,
    /// and/or to call both either instance or static methods on the class. This is particularly 
    /// useful when all you have is an interface returned from some unknown plug-in, but the plug-in
    /// requires other properties in order to operate correctly.
    /// </para>
    /// </remarks>
    public sealed class DynamicAccess<T> where T : class
    {
        #region Internal Variable

        private T _instance;

        #endregion

        #region Constructor

        /// <summary>
        /// Dynamic Access object constructor, when only the type of the object required is known.
        /// </summary>
        /// <remarks>
        /// This constructor will generate and call the IL required to create an instance of an
        /// object of Type t. This call is considerably faster than using reflection to perform
        /// the same job, particularly as the generated IL is cached to reuse whenever another
        /// instance of the object is required.
        /// <example>
        /// <code>
        /// Type t;
        /// 
        /// t = SomeType;
        /// DynamicAccess someType = new DynamicAccess(t);
        /// </code>
        /// </example>
        /// </remarks>
        public DynamicAccess()
        {
            _instance = (T)Cache.Create(typeof(T));
        }
        /// <summary>
        /// DynamicAccess object parameterised constructor, where the unknown object's constructor
        /// requires parameters.
        /// </summary>
        /// <param name="Params">Parameters reuired for type t's constructor</param>
        /// <remarks>
        /// This constructor will generate and call the IL required to create an instance of an
        /// object of Type t, passing the supplied parameters to the appropriate constructor. 
        /// This call is considerably faster than using reflection to perform the same job,
        /// particularly as the generated IL is cached to reuse whenever another
        /// instance of the object is required.
        /// <para>
        /// Note that the types of the parameters passed into this constructor must match exactly
        /// the signature of required object's constructor parameters, otherwise the call will fail.
        /// </para>
        /// <para>
        /// This type of constructor is particularly useful when it is known that a type that 
        /// implements a known interface takes a parameterised constructor. The follwing example 
        /// constructs an object that implements IMyPlugin where the Type has been retrieved from
        /// an attribute on the class. It has been agreed that the plugins will be able to accept
        /// a configuration file's path as a constructor parameter, in order to initialise its 
        /// custom properties.
        /// <example>
        /// <code>
        /// Attribute attr = Attribute.GetCustomAttribute(MyPluginAssembly, typeof(PluginImplementationAttribute));
        /// Type t = attr.PluginImplentationType;
        /// String path = MySettingsPath;
        /// 
        /// DynamicAccess MyPluginInstance = new DynamicAccess(t, path);
        /// IMyPlugin iPlugin = (IMyPlugin)MyPluginInstance.Instance;
        /// </code>
        /// </example>
        /// </para>
        /// </remarks>
        public DynamicAccess(params object[] Params)
        {
            _instance = (T)Cache.Create(typeof(T), Params);
        }
        /// <summary>
        /// Creates a DynamicAccess object from an existing instance of the type
        /// </summary>
        /// <param name="Instance">The object you want to call Latebound methods and properties on</param>
        /// <remarks>
        /// This type of latebound constructor is usefule when a plugin returns an interface pointer
        /// to a constructed object. The resulting latebound object will then allow the caller to
        /// call other properties and methods on the object that are not included in the interface.
        /// <example>
        /// <code>
        /// IMyPlugin myPlugin = PluginManager.CreateInstance("MyPluginName");
        /// DynamicAccess myLateboundInstance = new DynamicAccess(myPlugin);
        /// 
        /// object myPluginProperty = myLateboundInstance["ThePropertyName"];
        /// </code>
        /// </example>
        /// </remarks>
        public DynamicAccess(object Instance)
        {
            if (!typeof(T).IsInstanceOfType(Instance))
                throw new ArgumentException(String.Format("Parameter is not an instance of type {0}", typeof(T).FullName), "Instance");
            _instance = (T)Instance;
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Get/Set Property's value by specificed Property Name
        /// </summary>
        /// <param name="PropertyName">specificed Property Name to get/set value</param>
        /// <returns>value that stored in specificed property's name</returns>
        public object this[string PropertyName]
        {
            get
            {
                return Cache.GetProperty(typeof(T), _instance, typeof(T).GetProperty(PropertyName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            }
            set
            {
                Cache.SetProperty(typeof(T), _instance, typeof(T).GetProperty(PropertyName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic), value);
            }
        }
        /// <summary>
        /// Call Method
        /// </summary>
        /// <param name="MethodName">target method's name</param>
        /// <param name="Params">parameter for specificed method</param>
        /// <returns>value that return from specificed method.</returns>
        public object Call(string MethodName, params object[] Params)
        {
            MethodInfo info;
            Type _type = typeof(T);
            Type[] types = null;
            if (Params != null && Params.Length > 0)
            {
                types = new Type[Params.Length];
                for (int i = 0; i < Params.Length; ++i)
                    types[i] = Params[i].GetType();
                info = _type.GetMethod(MethodName,
                    //BindingFlags.NonPublic | BindingFlags.ExactBinding | BindingFlags.Public | BindingFlags.Instance,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                    null, types, null);
            }
            else
            {
                info = _type.GetMethod(MethodName,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            }
            return Cache.CallMethod(_type, _instance, info, types, Params);
        }
        /// <summary>
        /// Get Object instance
        /// </summary>
        public T Instance { get { return _instance; } }

        #endregion

        #region Static Method

        /// <summary>
        /// Get Value from Property
        /// </summary>
        /// <param name="instance">Object Instance</param>
        /// <param name="PropertyName">Property's Name</param>
        /// <returns>value that contain in specificed property</returns>
        public static object Get(object instance, string PropertyName)
        {
            if (instance == null)
                return (T)null;
            Type t = instance.GetType();
            return Cache.GetProperty(t, instance, t.GetProperty(PropertyName));
        }
        /// <summary>
        /// Set Value to Property
        /// </summary>
        /// <param name="instance">Object Instance</param>
        /// <param name="PropertyName">Property's Name</param>
        /// <param name="Value">value to assigned to property</param>
        public static void Set(object instance, string PropertyName, object Value)
        {
            if (instance == null)
                return;
            Type t = instance.GetType();
            Cache.SetProperty(t, instance, t.GetProperty(PropertyName), Value);
        }
        /// <summary>
        /// Call Method
        /// </summary>
        /// <param name="instance">Object Instance</param>
        /// <param name="MethodName">target method's name</param>
        /// <param name="Params">parameter for specificed method</param>
        /// <returns>value that return from specificed method.</returns>
        public static object Call(object instance, string MethodName, params object[] Params)
        {
            if (instance == null)
                return null;

            Type t = instance.GetType();

            MethodInfo info;
            Type[] types = null;
            if (Params != null && Params.Length > 0)
            {
                types = new Type[Params.Length];
                for (int i = 0; i < Params.Length; ++i)
                    types[i] = Params[i].GetType();
                info = t.GetMethod(MethodName,
                    //BindingFlags.NonPublic | BindingFlags.ExactBinding | BindingFlags.Public | BindingFlags.Instance,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                    null, types, null);
            }
            else
            {
                info = t.GetMethod(MethodName,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            }
            return Cache.CallMethod(t, instance, info, types, Params);
        }
        /// <summary>
        /// Call Static Method
        /// </summary>
        /// <param name="MethodName">Static Method's Name</param>
        /// <param name="Params">Method Parameters</param>
        /// <returns>value that return from method</returns>
        public static object CallStatic(string MethodName, params object[] Params)
        {
            MethodInfo info;
            Type t = typeof(T);
            Type[] types = null;
            if (Params != null && Params.Length > 0)
            {
                types = new Type[Params.Length];
                for (int i = 0; i < Params.Length; ++i)
                    types[i] = Params[i].GetType();
                info = t.GetMethod(MethodName,
                    //BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.ExactBinding,
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                    null, types, null);
            }
            else
            {
                info = t.GetMethod(MethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }
            return Cache.CallMethod(t, null, info, types, Params);
        }
        /// <summary>
        /// Get Value from Static Property
        /// </summary>
        /// <param name="PropertyName">Property's Name</param>
        /// <returns>value that contain in specificed property</returns>
        public static object GetStatic(string PropertyName)
        {
            Type t = typeof(T);
            return Cache.GetProperty(t, null, t.GetProperty(PropertyName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
        }
        /// <summary>
        /// Set Value to Static Property
        /// </summary>
        /// <param name="PropertyName">Property's Name</param>
        /// <param name="Value">value to assigned to property</param>
        public static void SetStatic(string PropertyName, object Value)
        {
            Type t = typeof(T);
            Cache.SetProperty(t, null, t.GetProperty(PropertyName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), Value);
        }
        /// <summary>
        /// Create New instance
        /// </summary>
        /// <returns>return new object instance</returns>
        public static T Create()
        {
            object inst = Cache.Create(typeof(T));
            if (null == inst)
                return default(T);
            return (T)inst;
        }
        /// <summary>
        /// Create New instance
        /// </summary>
        /// <param name="Params">constructor parameters</param>
        /// <returns>return new object instance</returns>
        public static T Create(params object[] Params)
        {
            object inst = Cache.Create(typeof(T), Params);
            if (null == inst)
                return default(T);
            return (T)inst;
        }

        #endregion
    }

    #endregion
}
