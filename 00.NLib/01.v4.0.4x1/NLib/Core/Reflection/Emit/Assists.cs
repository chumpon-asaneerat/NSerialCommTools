#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-10-24
=================
- Reflection Emit Framework updated.
  - Update code from BLToolkit 4.1 (snapshot).

======================================================================================================================
Update 2011-02-10
=================
- Update code in EmitAssist
  - Add new method to work with label by access label in local cache in EmitAsist
  - Add new WriteLine method in EmitAssist
  - Fixed but in TypeAssist in DefinePublicConstructor

======================================================================================================================
Update 2010-02-06
=================
- Fiexd bug in AssemblyAssist class.
  - Fixed access null object acsess in ModuleBuilder property get's code.
 
======================================================================================================================
Update 2009-12-25
=================
- Update code for Reflection Emit API
  - EmitAssist class add and changed some methods for compatibility with GFA37.

======================================================================================================================
Update 2009-12-25
=================
- Rewrite code for Reflection Emit API
  - EmitAssist class is rewrite in C# 2.0 language spec.
  - EmitAssist class is rewrite all ILGenerator Methods.
  - EmitAssist class is rewrite all Reflection.Emit commands.

======================================================================================================================
Update 2009-12-24
=================
- Rewrite code for Reflection Emit API
  - AssemblyAssist class is rewrite in C# 2.0 language spec.
  - AssemblyAssist class now support build runtime assembly with Partial Trust Security.
  - TypeAssist class is rewrite in C# 2.0 language spec.
  - TypeAssist class is rewrite to fully supports Generic in C# 2.0.
  - MethodAssistBase class is ported from GFA37.
  - MethodAssist class is rewrite in C# 2.0 language spec.
  - ConstructorAssist class is rewrite in C# 2.0 language spec.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Security;
using System.Runtime.InteropServices;
using System.Diagnostics.SymbolStore;
using System.Diagnostics.CodeAnalysis;

using NLib.Reflection.Common;

#endregion

#region Emit

namespace NLib.Reflection.Emit
{
    #region Assembly Assist

    /// <summary>
    /// Assembly Assist. A wrapper around the <see cref="AssemblyBuilder"/> and <see cref="ModuleBuilder"/> classes.
    /// </summary>
    /// <seealso cref="System.Reflection.Emit.AssemblyBuilder">AssemblyBuilder Class</seealso>
    /// <seealso cref="System.Reflection.Emit.ModuleBuilder">ModuleBuilder Class</seealso>
    public class AssemblyAssist
    {
        #region Internal Variable

        private readonly string _moduleName;
        private readonly AssemblyName _assemblyName = new AssemblyName();
        private readonly Action<int> _createAssemblyBuilder;
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private AssemblyAssist() : base() { }
        /// <summary>
        /// Constructor. Initializes a new instance of the <see cref="AssemblyAssist"/> class
        /// with the specified parameters.
        /// </summary>
        /// <param name="assemblyName">The Assembly's Name</param>
        /// <param name="moduleName">The Module's Name</param>
        public AssemblyAssist(string assemblyName, string moduleName)
            : this()
        {
            if (assemblyName.Trim() == string.Empty)
            {
                throw new ArgumentNullException("assemblyName");
            }
            _assemblyName.Name = assemblyName;
            // If need DEBUG used this line
            //_assemblyName.Flags |= AssemblyNameFlags.EnableJITcompileTracking;
            _assemblyName.Flags |= AssemblyNameFlags.EnableJITcompileOptimizer;

            if (moduleName.Trim() == string.Empty)
            {
                _moduleName = _assemblyName.Name;
            }
            else _moduleName = moduleName;

            // init delegate
            _createAssemblyBuilder = delegate(int _)
            {
                _assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(_assemblyName,
                    AssemblyBuilderAccess.Run);
                // set assembly attribute for partial trust assembly.
                _assemblyBuilder.SetCustomAttribute(
                    new CustomAttributeBuilder(
                        typeof(AllowPartiallyTrustedCallersAttribute)
                            .GetConstructor(Type.EmptyTypes),
                        new object[0]));
            };
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~AssemblyAssist()
        {
            _assemblyBuilder = null;
            _moduleBuilder = null;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets Assembly Builder.
        /// </summary>
        [Browsable(false)]
        public AssemblyBuilder AssemblyBuilder
        {
            get
            {
                if (_assemblyBuilder == null)
                    _createAssemblyBuilder(0);
                return _assemblyBuilder;
            }
        }
        /// <summary>
        /// Gets Assembly Name.
        /// </summary>
        [Browsable(false)]
        public AssemblyName AssemblyName { get { return _assemblyName; } }
        /// <summary>
        /// Get Module's Name
        /// </summary>
        [Browsable(false)]
        public string ModuleName { get { return _moduleName; } }
        /// <summary>
        /// Gets ModuleBuilder.
        /// </summary>
        [Browsable(false)]
        public ModuleBuilder ModuleBuilder
        {
            get
            {
                if (_moduleBuilder == null)
                {
                    lock (this)
                    {
                        _moduleBuilder = AssemblyBuilder.DefineDynamicModule(_moduleName);
                    }

                }
                return _moduleBuilder;
            }
        }

        #endregion

        #region Assembly/Module Builder Operator overloadings

        /// <summary>
        /// Converts the supplied <see cref="AssemblyAssist"/> instance to a <see cref="AssemblyBuilder"/>.
        /// </summary>
        /// <param name="assemblyAssist">The <see cref="AssemblyAssist"/> instance.</param>
        /// <returns>An <see cref="AssemblyBuilder"/> instance.</returns>
        public static implicit operator AssemblyBuilder(AssemblyAssist assemblyAssist)
        {
            if (assemblyAssist == null)
                throw new ArgumentNullException("assemblyAssist");

            return assemblyAssist.AssemblyBuilder;
        }
        /// <summary>
        /// Converts the supplied <see cref="AssemblyAssist"/> instance to a <see cref="ModuleBuilder"/>.
        /// </summary>
        /// <param name="assemblyAssist">The <see cref="AssemblyAssist"/> instance.</param>
        /// <returns>A <see cref="ModuleBuilder"/> instance.</returns>
        public static implicit operator ModuleBuilder(AssemblyAssist assemblyAssist)
        {
            if (assemblyAssist == null)
                throw new ArgumentNullException("assemblyAssist");

            return assemblyAssist.ModuleBuilder;
        }

        #endregion

        #region Type Builder Overloadings

        /// <summary>
        /// Constructs a <see cref="TypeAssist"/> instance for a type with the specified name.
        /// </summary>
        /// <param name="name">The full path of the type.</param>
        /// <returns>Returns the created <see cref="TypeAssist"/> instance.</returns>
        /// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string)">
        /// ModuleBuilder.DefineType Method</seealso>
        public TypeAssist DefineType(string name)
        {
            return new TypeAssist(this,
                ModuleBuilder.DefineType(name));
        }
        /// <summary>
        /// Constructs a <see cref="TypeAssist"/> instance for a type with the specified name 
        /// and base type.
        /// </summary>
        /// <param name="name">The full path of the type.</param>
        /// <param name="parent">The Type that the defined type extends.</param>
        /// <returns>Returns the created <see cref="TypeAssist"/> instance.</returns>
        /// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string,TypeAttributes,Type)">
        /// ModuleBuilder.DefineType Method</seealso>
        public TypeAssist DefineType(string name, Type parent)
        {
            return new TypeAssist(this,
                ModuleBuilder.DefineType(name, TypeAttributes.Public, parent));
        }
        /// <summary>
        /// Constructs a <see cref="TypeAssist"/> instance for a type with the specified name, 
        /// its attributes, and base type.
        /// </summary>
        /// <param name="name">The full path of the type.</param>
        /// <param name="attrs">The attribute to be associated with the type.</param>
        /// <param name="parent">The Type that the defined type extends.</param>
        /// <returns>Returns the created <see cref="TypeAssist"/> instance.</returns>
        /// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string,TypeAttributes,Type)">
        /// ModuleBuilder.DefineType Method</seealso>
        public TypeAssist DefineType(string name, TypeAttributes attrs, Type parent)
        {
            return new TypeAssist(this,
                ModuleBuilder.DefineType(name, attrs, parent));
        }
        /// <summary>
        /// Constructs a <see cref="TypeAssist"/> instance for a type with the specified name, 
        /// base type,
        /// and the interfaces that the defined type implements.
        /// </summary>
        /// <param name="name">The full path of the type.</param>
        /// <param name="parent">The Type that the defined type extends.</param>
        /// <param name="interfaces">The list of interfaces that the type implements.</param>
        /// <returns>Returns the created <see cref="TypeAssist"/> instance.</returns>
        /// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string,TypeAttributes,Type,Type[])">
        /// ModuleBuilder.DefineType Method</seealso>
        public TypeAssist DefineType(string name, Type parent,
            params Type[] interfaces)
        {
            return new TypeAssist(this,
                ModuleBuilder.DefineType(name, TypeAttributes.Public, parent, interfaces));
        }
        /// <summary>
        /// Constructs a <see cref="TypeAssist"/> instance for a type with the specified name, its attributes, base type,
        /// and the interfaces that the defined type implements.
        /// </summary>
        /// <param name="name">The full path of the type.</param>
        /// <param name="attrs">The attribute to be associated with the type.</param>
        /// <param name="parent">The Type that the defined type extends.</param>
        /// <param name="interfaces">The list of interfaces that the type implements.</param>
        /// <returns>Returns the created <see cref="TypeAssist"/> instance.</returns>
        /// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string,TypeAttributes,Type,Type[])">ModuleBuilder.DefineType Method</seealso>
        public TypeAssist DefineType(string name, TypeAttributes attrs, Type parent,
            params Type[] interfaces)
        {
            return new TypeAssist(
                this,
                ModuleBuilder.DefineType(name, attrs, parent, interfaces));
        }

        #endregion
    }

    #endregion

    #region Type Assist

    /// <summary>
    /// Type Assist. A wrapper around the <see cref="TypeBuilder"/> class.
    /// </summary>
    /// <seealso cref="System.Reflection.Emit.TypeBuilder">TypeBuilder Class</seealso>
    public class TypeAssist
    {
        #region Internal Variable

        private readonly AssemblyAssist _assembly;
        private readonly System.Reflection.Emit.TypeBuilder _typeBuilder;

        private ConstructorAssist _typeInitializer;
        private ConstructorAssist _defaultConstructor;
        //private ConstructorAssist _initConstructor;

        private Dictionary<MethodInfo, MethodBuilder> _overriddenMethods;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor. Initializes a new instance of the <see cref="TypeAssist"/> class
        /// with the specified parameters.
        /// </summary>
        /// <param name="assemblyAssist">Associated <see cref="AssemblyAssist"/> instance.</param>
        /// <param name="typeBuilder">A <see cref="TypeBuilder"/> instance.</param>
        public TypeAssist(AssemblyAssist assemblyAssist,
            System.Reflection.Emit.TypeBuilder typeBuilder)
        {
            if (assemblyAssist == null) throw new ArgumentNullException("assemblyAssist");
            if (typeBuilder == null) throw new ArgumentNullException("typeBuilder");

            _assembly = assemblyAssist;
            _typeBuilder = typeBuilder;

            //_typeBuilder.SetCustomAttribute(_assembly.BLToolkitAttribute);
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~TypeAssist()
        {
            if (_overriddenMethods != null)
            {
                try { _overriddenMethods.Clear(); }
                catch { }
            }
            _overriddenMethods = null;
            _defaultConstructor = null;
            //_initConstructor = null;
            _typeInitializer = null;

        }

        #endregion

        #region Public Method

        #region Initializer Access

        /// <summary>
        /// Gets the initializer for this type.
        /// </summary>
        [Browsable(false)]
        public ConstructorAssist TypeInitializer
        {
            get
            {
                if (_typeInitializer == null)
                {
                    lock (this)
                    {
                        _typeInitializer = new ConstructorAssist(this,
                            _typeBuilder.DefineTypeInitializer());
                    }
                }
                return _typeInitializer;
            }
        }
        /// <summary>
        /// Returns true if the initializer for this type has a body.
        /// </summary>
        [Browsable(false)]
        public bool IsTypeInitializerDefined { get { return _typeInitializer != null; } }
        /// <summary>
        /// Gets the default constructor for this type.
        /// </summary>
        [Browsable(false)]
        public ConstructorAssist DefaultConstructor
        {
            get
            {
                if (_defaultConstructor == null)
                {
                    lock (this)
                    {
                        ConstructorBuilder builder = _typeBuilder.DefineConstructor(
                            MethodAttributes.Public,
                            CallingConventions.Standard,
                            Type.EmptyTypes);

                        _defaultConstructor = new ConstructorAssist(this, builder);
                    }
                }

                return _defaultConstructor;
            }
        }
        /// <summary>
        /// Returns true if the default constructor for this type has a body.
        /// </summary>
        [Browsable(false)]
        public bool IsDefaultConstructorDefined { get { return _defaultConstructor != null; } }
        /*
        /// <summary>
        /// Gets the init context constructor for this type.
        /// </summary>
        [Browsable(false)]
        public ConstructorAssist InitConstructor
        {
            get
            {
                if (_initConstructor == null)
                {
                    lock (this)
                    {
                        ConstructorBuilder builder = _typeBuilder.DefineConstructor(
                            MethodAttributes.Public,
                            CallingConventions.Standard,
                            new Type[] { typeof(InitContext) });

                        _initConstructor = new ConstructorAssist(this, builder);
                    }
                }

                return _initConstructor;
            }
        }
        /// <summary>
        /// Returns true if a constructor with parameter of <see cref="InitContext"/> for this type has a body.
        /// </summary>
        [Browsable(false)]
        public bool IsInitConstructorDefined { get { return _initConstructor != null; } }
        */
        #endregion

        #region Define Methods

        /// <summary>
        /// Retrieves the map of base type methods overridden by this type.
        /// </summary>
        public Dictionary<MethodInfo, MethodBuilder> OverriddenMethods
        {
            get
            {
                return _overriddenMethods ?? (_overriddenMethods = new Dictionary<MethodInfo, MethodBuilder>());
            }
        }
        /// <summary>
        /// Adds a new method to the class, with the given name and method signature.
        /// </summary>
        /// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
        /// <param name="attributes">The attributes of the method. </param>
        /// <param name="returnType">The return type of the method.</param>
        /// <param name="parameterTypes">The types of the parameters of the method.</param>
        /// <returns>The defined method assist instance.</returns>
        public MethodAssist DefineMethod(string name,
            MethodAttributes attributes, Type returnType, params Type[] parameterTypes)
        {
            return new MethodAssist(this,
                _typeBuilder.DefineMethod(name, attributes, returnType, parameterTypes));
        }
        /// <summary>
        /// Adds a new method to the class, with the given name and method signature.
        /// </summary>
        /// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
        /// <param name="attributes">The attributes of the method. </param>
        /// <param name="callingConvention">The <see cref="CallingConventions">calling convention</see> of the method.</param>
        /// <param name="returnType">The return type of the method.</param>
        /// <param name="parameterTypes">The types of the parameters of the method.</param>
        /// <returns>The defined method assist instance.</returns>
        public MethodAssist DefineMethod(string name,
            MethodAttributes attributes, CallingConventions callingConvention,
            Type returnType, Type[] parameterTypes)
        {
            return new MethodAssist(this, _typeBuilder.DefineMethod(
                name, attributes, callingConvention, returnType, parameterTypes));
        }
        /// <summary>
        /// Adds a new method to the class, with the given name and method signature.
        /// </summary>
        /// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
        /// <param name="attributes">The attributes of the method. </param>
        /// <param name="returnType">The return type of the method.</param>
        /// <returns>The defined method assist instance.</returns>
        public MethodAssist DefineMethod(string name, MethodAttributes attributes, Type returnType)
        {
            return new MethodAssist(
                this,
                _typeBuilder.DefineMethod(name, attributes, returnType, Type.EmptyTypes));
        }
        /// <summary>
        /// Adds a new method to the class, with the given name and method signature.
        /// </summary>
        /// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
        /// <param name="attributes">The attributes of the method. </param>
        /// <returns>The defined method assist instance.</returns>
        public MethodAssist DefineMethod(string name, MethodAttributes attributes)
        {
            return new MethodAssist(this,
                _typeBuilder.DefineMethod(name, attributes, typeof(void), Type.EmptyTypes));
        }
        /// <summary>
        /// Adds a new method to the class, with the given name and method signature.
        /// </summary>
        /// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
        /// <param name="attributes">The attributes of the method. </param>
        /// <returns>The defined method assist instance.</returns>
        /// <param name="callingConvention">The calling convention of the method.</param>
        public MethodAssist DefineMethod(string name,
            MethodAttributes attributes, CallingConventions callingConvention)
        {
            return new MethodAssist(this,
                _typeBuilder.DefineMethod(name, attributes, callingConvention));
        }
        /// <summary>
        /// Adds a new method to the class, with the given name and method signature.
        /// </summary>
        /// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
        /// <param name="attributes">The attributes of the method. </param>
        /// <param name="callingConvention">The <see cref="CallingConventions">calling convention</see> of the method.</param>
        /// <param name="genericArguments">Generic arguments of the method.</param>
        /// <param name="returnType">The return type of the method.</param>
        /// <param name="parameterTypes">The types of the parameters of the method.</param>
        /// <returns>The defined (generic) method assist instance.</returns>
        public MethodAssist DefineGenericMethod(string name,
            MethodAttributes attributes, CallingConventions callingConvention,
            Type[] genericArguments, Type returnType, Type[] parameterTypes)
        {
            return new MethodAssist(this,
                _typeBuilder.DefineMethod(name, attributes, callingConvention),
                genericArguments, returnType, parameterTypes);
        }
        /// <summary>
        /// Adds a new method to the class, with the given name and method signature.
        /// </summary>
        /// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
        /// <param name="methodInfoDeclaration">The method whose declaration is to be used.</param>
        /// <param name="attributes">The attributes of the method. </param>
        /// <returns>The defined method assist instance.</returns>
        public MethodAssist DefineMethod(
            string name,
            MethodInfo methodInfoDeclaration,
            MethodAttributes attributes)
        {
            if (methodInfoDeclaration == null)
                throw new ArgumentNullException("methodInfoDeclaration");

            MethodAssist method;
            ParameterInfo[] pi = methodInfoDeclaration.GetParameters();
            Type[] parameters = new Type[pi.Length];

            for (int i = 0; i < pi.Length; i++)
                parameters[i] = pi[i].ParameterType;

            if (methodInfoDeclaration.ContainsGenericParameters)
            {
                method = DefineGenericMethod(
                    name,
                    attributes,
                    methodInfoDeclaration.CallingConvention,
                    methodInfoDeclaration.GetGenericArguments(),
                    methodInfoDeclaration.ReturnType,
                    parameters);
            }
            else
            {
                method = DefineMethod(
                    name,
                    attributes,
                    methodInfoDeclaration.CallingConvention,
                    methodInfoDeclaration.ReturnType,
                    parameters);
            }

            // Compiler overrides methods only for interfaces. We do the same.
            // If we wanted to override virtual methods, then methods should've had
            // MethodAttributes.VtableLayoutMask attribute
            // and the following condition should've been used below:
            // if ((methodInfoDeclaration is FakeMethodInfo) == false)
            //
            if (methodInfoDeclaration.DeclaringType.IsInterface &&
                !(methodInfoDeclaration is FakeMethodInfo))
            {
                OverriddenMethods.Add(methodInfoDeclaration, method.MethodBuilder);
                _typeBuilder.DefineMethodOverride(method.MethodBuilder, methodInfoDeclaration);
            }

            method.OverriddenMethod = methodInfoDeclaration;

            for (int i = 0; i < pi.Length; i++)
                method.MethodBuilder.DefineParameter(i + 1, pi[i].Attributes, pi[i].Name);

            return method;
        }
        /// <summary>
        /// Adds a new method to the class, with the given name and method signature.
        /// </summary>
        /// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
        /// <param name="methodInfoDeclaration">The method whose declaration is to be used.</param>
        /// <returns>The defined method assist instance.</returns>
        public MethodAssist DefineMethod(string name, MethodInfo methodInfoDeclaration)
        {
            return DefineMethod(name, methodInfoDeclaration, MethodAttributes.Virtual);
        }
        /// <summary>
        /// Adds a new private method to the class.
        /// </summary>
        /// <param name="methodInfoDeclaration">The method whose declaration is to be used.</param>
        /// <returns>The defined method assist instance.</returns>
        public MethodAssist DefineMethod(MethodInfo methodInfoDeclaration)
        {
            if (methodInfoDeclaration == null)
                throw new ArgumentNullException("methodInfoDeclaration");

            bool isInterface = methodInfoDeclaration.DeclaringType.IsInterface;
            bool isFake = methodInfoDeclaration is FakeMethodInfo;

            string name = isInterface && !isFake ?
                methodInfoDeclaration.DeclaringType.FullName + "." + methodInfoDeclaration.Name :
                methodInfoDeclaration.Name;

            MethodAttributes attributes =
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig |
                MethodAttributes.PrivateScope |
                methodInfoDeclaration.Attributes & MethodAttributes.SpecialName;

            if (isInterface && !isFake)
                attributes |= MethodAttributes.Private;
            else if ((attributes & MethodAttributes.SpecialName) != 0)
                attributes |= MethodAttributes.Public;
            else
                attributes |= methodInfoDeclaration.Attributes &
                    (MethodAttributes.Public | MethodAttributes.Private);

            return DefineMethod(name, methodInfoDeclaration, attributes);
        }

        #endregion

        #region Create

        /// <summary>
        /// Creates a Type object for the class.
        /// </summary>
        /// <returns>Returns the new Type object for this class.</returns>
        public Type Create() { return TypeBuilder.CreateType(); }

        #endregion

        #region Set Attributes

        /// <summary>
        /// Sets a custom attribute using a custom attribute type.
        /// </summary>
        /// <param name="attributeType">Attribute type.</param>
        public void SetCustomAttribute(Type attributeType)
        {
            if (attributeType == null)
                throw new ArgumentNullException("attributeType");

            ConstructorInfo ci = attributeType.GetConstructor(Type.EmptyTypes);
            CustomAttributeBuilder caBuilder = new CustomAttributeBuilder(ci, new object[0]);

            _typeBuilder.SetCustomAttribute(caBuilder);
        }
        /// <summary>
        /// Sets a custom attribute using a custom attribute type
        /// and named properties.
        /// </summary>
        /// <param name="attributeType">Attribute type.</param>
        /// <param name="properties">Named properties of the custom attribute.</param>
        /// <param name="propertyValues">Values for the named properties of the custom attribute.</param>
        public void SetCustomAttribute(
            Type attributeType,
            PropertyInfo[] properties,
            object[] propertyValues)
        {
            if (attributeType == null)
                throw new ArgumentNullException("attributeType");

            ConstructorInfo ci = attributeType.GetConstructor(Type.EmptyTypes);
            CustomAttributeBuilder caBuilder = new CustomAttributeBuilder(
                ci, new object[0], properties, propertyValues);

            _typeBuilder.SetCustomAttribute(caBuilder);
        }
        /// <summary>
        /// Sets a custom attribute using a custom attribute type
        /// and named property.
        /// </summary>
        /// <param name="attributeType">Attribute type.</param>
        /// <param name="propertyName">A named property of the custom attribute.</param>
        /// <param name="propertyValue">Value for the named property of the custom attribute.</param>
        public void SetCustomAttribute(
            Type attributeType,
            string propertyName,
            object propertyValue)
        {
            SetCustomAttribute(
                attributeType,
                new PropertyInfo[] { attributeType.GetProperty(propertyName) },
                new object[] { propertyValue });
        }

        #endregion

        #region Define Field

        /// <summary>
        /// Adds a new field to the class, with the given name, attributes and field type.
        /// </summary>
        /// <param name="fieldName">The name of the field. <paramref name="fieldName"/> cannot contain embedded nulls.</param>
        /// <param name="type">The type of the field.</param>
        /// <param name="attributes">The attributes of the field.</param>
        /// <returns>The defined field.</returns>
        public FieldBuilder DefineField(
            string fieldName,
            Type type,
            FieldAttributes attributes)
        {
            return _typeBuilder.DefineField(fieldName, type, attributes);
        }

        #endregion

        #region Define Constructor

        /// <summary>
        /// Adds a new public constructor to the class, with the given parameters.
        /// </summary>
        /// <param name="parameterTypes">The types of the parameters of the method.</param>
        /// <returns>The defined constructor assist instance.</returns>
        public ConstructorAssist DefinePublicConstructor(params Type[] parameterTypes)
        {
            /*
            return new ConstructorAssist(this,
                _typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameterTypes));
            */
            if (null != parameterTypes && parameterTypes.Length > 0)
            {
                return new ConstructorAssist(this,
                    _typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, parameterTypes));
            }
            else
            {
                return new ConstructorAssist(this,
                    _typeBuilder.DefineDefaultConstructor(MethodAttributes.Public));
            }
        }

        /// <summary>
        /// Adds a new constructor to the class, with the given attributes and parameters.
        /// </summary>
        /// <param name="attributes">The attributes of the field.</param>
        /// <param name="callingConvention">The <see cref="CallingConventions">calling convention</see> of the method.</param>
        /// <param name="parameterTypes">The types of the parameters of the method.</param>
        /// <returns>The defined constructor assist instance.</returns>
        public ConstructorAssist DefineConstructor(MethodAttributes attributes,
            CallingConventions callingConvention, params Type[] parameterTypes)
        {
            return new ConstructorAssist(
                this,
                _typeBuilder.DefineConstructor(attributes, callingConvention, parameterTypes));
        }

        #endregion

        #region Define NestedType

        /// <summary>
        /// Defines a nested type given its name..
        /// </summary>
        /// <param name="name">The short name of the type.</param>
        /// <returns>Returns the created <see cref="TypeAssist"/> instance.</returns>
        /// <seealso cref="System.Reflection.Emit.TypeBuilder.DefineNestedType(string)">
        /// TypeBuilder.DefineNestedType Method</seealso>
        public TypeAssist DefineNestedType(string name)
        {
            return new TypeAssist(_assembly, _typeBuilder.DefineNestedType(name));
        }
        /// <summary>
        /// Defines a public nested type given its name and the type that it extends.
        /// </summary>
        /// <param name="name">The short name of the type.</param>
        /// <param name="parent">The type that the nested type extends.</param>
        /// <returns>Returns the created <see cref="TypeAssist"/> instance.</returns>
        /// <seealso cref="System.Reflection.Emit.TypeBuilder.DefineNestedType(string,TypeAttributes,Type)">
        /// TypeBuilder.DefineNestedType Method</seealso>
        public TypeAssist DefineNestedType(string name, Type parent)
        {
            return new TypeAssist(_assembly,
                _typeBuilder.DefineNestedType(name, TypeAttributes.NestedPublic, parent));
        }
        /// <summary>
        /// Defines a nested type given its name, attributes, and the type that it extends.
        /// </summary>
        /// <param name="name">The short name of the type.</param>
        /// <param name="attributes">The attributes of the type.</param>
        /// <param name="parent">The type that the nested type extends.</param>
        /// <returns>Returns the created <see cref="TypeAssist"/> instance.</returns>
        /// <seealso cref="System.Reflection.Emit.TypeBuilder.DefineNestedType(string,TypeAttributes,Type)">
        /// TypeBuilder.DefineNestedType Method</seealso>
        public TypeAssist DefineNestedType(string name, TypeAttributes attributes, Type parent)
        {
            return new TypeAssist(_assembly,
                _typeBuilder.DefineNestedType(name, attributes, parent));
        }
        /// <summary>
        /// Defines a public nested type given its name, the type that it extends, and the interfaces that it implements.
        /// </summary>
        /// <param name="name">The short name of the type.</param>
        /// <param name="parent">The type that the nested type extends.</param>
        /// <param name="interfaces">The interfaces that the nested type implements.</param>
        /// <returns>Returns the created <see cref="TypeAssist"/> instance.</returns>
        /// <seealso cref="System.Reflection.Emit.TypeBuilder.DefineNestedType(string,TypeAttributes,Type,Type[])">
        /// TypeBuilder.DefineNestedType Method</seealso>
        public TypeAssist DefineNestedType(string name, Type parent, params Type[] interfaces)
        {
            return new TypeAssist(_assembly,
                _typeBuilder.DefineNestedType(name, TypeAttributes.NestedPublic, parent, interfaces));
        }
        /// <summary>
        /// Defines a nested type given its name, attributes, the type that it extends, and the interfaces that it implements.
        /// </summary>
        /// <param name="name">The short name of the type.</param>
        /// <param name="attributes">The attributes of the type.</param>
        /// <param name="parent">The type that the nested type extends.</param>
        /// <param name="interfaces">The interfaces that the nested type implements.</param>
        /// <returns>Returns the created <see cref="TypeAssist"/> instance.</returns>
        /// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string,TypeAttributes,Type,Type[])">ModuleBuilder.DefineType Method</seealso>
        public TypeAssist DefineNestedType(string name, TypeAttributes attributes,
            Type parent, params Type[] interfaces)
        {
            return new TypeAssist(_assembly,
                _typeBuilder.DefineNestedType(name, attributes, parent, interfaces));
        }

        #endregion

        #endregion

        #region Public Property

        /// <summary>
        /// Gets associated <see cref="AssemblyAssist"/> instance.
        /// </summary>
        [Browsable(false)]
        public AssemblyAssist Assembly { get { return _assembly; } }
        /// <summary>
        /// Gets <see cref="System.Reflection.Emit.TypeBuilder"/> instance.
        /// </summary>
        [Browsable(false)]
        public System.Reflection.Emit.TypeBuilder TypeBuilder { get { return _typeBuilder; } }

        #endregion

        #region Type Builder Operator overloadings

        /// <summary>
        /// Converts the supplied <see cref="TypeAssist"/> to a <see cref="TypeBuilder"/> instance.
        /// </summary>
        /// <param name="typeAssist">The <see cref="TypeAssist"/> instance.</param>
        /// <returns>A <see cref="TypeBuilder"/> instance.</returns>
        public static implicit operator System.Reflection.Emit.TypeBuilder(TypeAssist typeAssist)
        {
            if (typeAssist == null) throw new ArgumentNullException("typeAssist");

            return typeAssist.TypeBuilder;
        }

        #endregion
    }

    #endregion

    #region Method Assist Base (abstract)

    /// <summary>
    /// Base class for wrappers around methods and constructors.
    /// </summary>
    public abstract class MethodAssistBase
    {
        #region Internal Variable

        private readonly TypeAssist _type;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodAssist"/> class
        /// with the specified parameters.
        /// </summary>
        /// <param name="typeAssist">Associated <see cref="TypeAssist"/> instance.</param>
        protected MethodAssistBase(TypeAssist typeAssist)
        {
            if (typeAssist == null)
                throw new ArgumentNullException("typeAssist");
            _type = typeAssist;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets associated <see cref="TypeAssist"/>.
        /// </summary>
        [Browsable(false)]
        public TypeAssist Type
        {
            get { return _type; }
        }
        /// <summary>
        /// Gets <see cref="EmitAssist"/> instance.
        /// </summary>
        [Browsable(false)]
        public abstract EmitAssist Emitter { get; }

        #endregion
    }

    #endregion

    #region Method Assist

    /// <summary>
    /// Method Assist. A wrapper around the <see cref="MethodBuilder"/> class.
    /// </summary>
    /// <seealso cref="System.Reflection.Emit.MethodBuilder">MethodBuilder Class</seealso>
    public class MethodAssist : MethodAssistBase
    {
        #region Internal Variable

        private readonly MethodBuilder _methodBuilder;
        private EmitAssist _emitter;
        private MethodInfo _overriddenMethod;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor. Initializes a new instance of the <see cref="MethodAssist"/> class
        /// with the specified parameters.
        /// </summary>
        /// <param name="typeAssist">Associated <see cref="TypeAssist"/> instance.</param>
        /// <param name="methodBuilder">A <see cref="MethodBuilder"/></param>
        public MethodAssist(TypeAssist typeAssist, MethodBuilder methodBuilder)
            : base(typeAssist)
        {
            if (methodBuilder == null)
                throw new ArgumentNullException("methodBuilder");

            _methodBuilder = methodBuilder;

            //methodBuilder.SetCustomAttribute(Type.Assembly.BLToolkitAttribute);
        }
        /// <summary>
        /// Constructor. Initializes a new instance of the <see cref="MethodAssist"/> class
        /// with the specified parameters.
        /// </summary>
        /// <param name="typeAssist">Associated <see cref="TypeAssist"/> instance.</param>
        /// <param name="methodBuilder">A <see cref="MethodBuilder"/></param>
        /// <param name="genericArguments">Generic arguments of the method.</param>
        /// <param name="returnType">The return type of the method.</param>
        /// <param name="parameterTypes">The types of the parameters of the method.</param>
        internal MethodAssist(TypeAssist typeAssist,
            MethodBuilder methodBuilder, Type[] genericArguments,
            Type returnType, Type[] parameterTypes)
            : base(typeAssist)
        {
            if (methodBuilder == null)
                throw new ArgumentNullException("methodBuilder");
            if (genericArguments == null)
                throw new ArgumentNullException("genericArguments");

            _methodBuilder = methodBuilder;

            //string[] genArgNames = Array.ConvertAll<Type, string>(genericArguments, delegate(Type t) { return t.Name; });
            var genArgNames = genericArguments.Select(t => t.Name).ToArray();

            //GenericTypeParameterBuilder[] genParams = methodBuilder.DefineGenericParameters(genArgNames);
            var genParams = methodBuilder.DefineGenericParameters(genArgNames);

            // Copy parameter constraints.
            List<Type> interfaceConstraints = null;
            for (int i = 0; i < genParams.Length; i++)
            {
                genParams[i].SetGenericParameterAttributes(genericArguments[i].GenericParameterAttributes);

                foreach (var constraint in genericArguments[i].GetGenericParameterConstraints())
                {
                    if (constraint.IsClass)
                    {
                        genParams[i].SetBaseTypeConstraint(constraint);
                    }
                    else
                    {
                        if (interfaceConstraints == null)
                            interfaceConstraints = new List<Type>();
                        interfaceConstraints.Add(constraint);
                    }
                }

                if (interfaceConstraints != null && interfaceConstraints.Count != 0)
                {
                    genParams[i].SetInterfaceConstraints(interfaceConstraints.ToArray());
                    interfaceConstraints.Clear();
                }
            }

            // When a method contains a generic parameter we need to replace all
            // generic types from methodInfoDeclaration with local ones.
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                parameterTypes[i] =
                    TypeEx.TranslateGenericParameters(parameterTypes[i], genParams);
            }
            methodBuilder.SetParameters(parameterTypes);
            methodBuilder.SetReturnType(TypeEx.TranslateGenericParameters(returnType, genParams));

            // Once all generic stuff is done is it is safe to call SetCustomAttribute
            //methodBuilder.SetCustomAttribute(Type.Assembly.BLToolkitAttribute);
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~MethodAssist()
        {
            _overriddenMethod = null;
            _emitter = null;
        }

        #endregion

        #region Public Method

        #region Set Custom Attribute

        /// <summary>
        /// Sets a custom attribute using a custom attribute type.
        /// </summary>
        /// <param name="attributeType">Attribute type.</param>
        public void SetCustomAttribute(Type attributeType)
        {
            if (attributeType == null)
                throw new ArgumentNullException("attributeType");

            ConstructorInfo ci = attributeType.GetConstructor(System.Type.EmptyTypes);
            CustomAttributeBuilder caBuilder = new CustomAttributeBuilder(ci, new object[0]);

            _methodBuilder.SetCustomAttribute(caBuilder);
        }
        /// <summary>
        /// Sets a custom attribute using a custom attribute type
        /// and named properties.
        /// </summary>
        /// <param name="attributeType">Attribute type.</param>
        /// <param name="properties">Named properties of the custom attribute.</param>
        /// <param name="propertyValues">Values for the named properties of the custom attribute.</param>
        public void SetCustomAttribute(Type attributeType, PropertyInfo[] properties,
            object[] propertyValues)
        {
            if (attributeType == null)
                throw new ArgumentNullException("attributeType");

            ConstructorInfo ci = attributeType.GetConstructor(System.Type.EmptyTypes);
            CustomAttributeBuilder caBuilder = new CustomAttributeBuilder(
                ci, new object[0], properties, propertyValues);

            _methodBuilder.SetCustomAttribute(caBuilder);
        }
        /// <summary>
        /// Sets a custom attribute using a custom attribute type
        /// and named property.
        /// </summary>
        /// <param name="attributeType">Attribute type.</param>
        /// <param name="propertyName">A named property of the custom attribute.</param>
        /// <param name="propertyValue">Value for the named property of the custom attribute.</param>
        public void SetCustomAttribute(Type attributeType, string propertyName,
            object propertyValue)
        {
            SetCustomAttribute(attributeType,
                new PropertyInfo[] { attributeType.GetProperty(propertyName) },
                new object[] { propertyValue });
        }

        #endregion

        #endregion

        #region Overrides

        /// <summary>
        /// Gets <see cref="EmitAssist"/> instance.
        /// </summary>
        [Browsable(false)]
        public override EmitAssist Emitter
        {
            get
            {
                if (_emitter == null)
                {
                    lock (this)
                    {
                        _emitter = new EmitAssist(this, _methodBuilder.GetILGenerator());
                    }
                }
                return _emitter;
            }
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets MethodBuilder.
        /// </summary>
        [Browsable(false)]
        public MethodBuilder MethodBuilder
        {
            get { return _methodBuilder; }
        }
        /// <summary>
        /// Returns the type that declares this method.
        /// </summary>
        public Type DeclaringType
        {
            get { return _methodBuilder.DeclaringType; }
        }
        /// <summary>
        /// Gets or sets the base type method overridden by this method, if any.
        /// </summary>
        public MethodInfo OverriddenMethod
        {
            get { return _overriddenMethod; }
            set { _overriddenMethod = value; }
        }

        #endregion

        #region Method Builder Operator overloading

        /// <summary>
        /// Converts the supplied <see cref="MethodAssist"/> instance to a <see cref="MethodBuilder"/>.
        /// </summary>
        /// <param name="methodAssist">The <see cref="MethodAssist"/> instance.</param>
        /// <returns>A <see cref="MethodBuilder"/>.</returns>
        public static implicit operator MethodBuilder(MethodAssist methodAssist)
        {
            if (methodAssist == null)
                throw new ArgumentNullException("methodAssist");

            return methodAssist.MethodBuilder;
        }

        #endregion
    }

    #endregion

    #region Constructor Assist

    /// <summary>
    /// Constructor Assist. A wrapper around the <see cref="ConstructorBuilder"/> class.
    /// </summary>
    public class ConstructorAssist : MethodAssistBase
    {
        #region Internal Variable

        private readonly ConstructorBuilder _constructorBuilder;
        private EmitAssist _emitter;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor. Initializes a new instance of the <see cref="ConstructorAssist"/> class
        /// with the specified parameters.
        /// </summary>
        /// <param name="typeAssist">Associated <see cref="TypeAssist"/> instance.</param>
        /// <param name="constructorBuilder">A <see cref="ConstructorBuilder"/></param>
        public ConstructorAssist(TypeAssist typeAssist, ConstructorBuilder constructorBuilder)
            : base(typeAssist)
        {
            if (constructorBuilder == null)
                throw new ArgumentNullException("constructorBuilder");

            _constructorBuilder = constructorBuilder;
            //_constructorBuilder.SetCustomAttribute(Type.Assembly.BLToolkitAttribute);
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~ConstructorAssist()
        {
            _emitter = null;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets ConstructorBuilder.
        /// </summary>
        public ConstructorBuilder ConstructorBuilder
        {
            get { return _constructorBuilder; }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets <see cref="EmitAssist"/> instance.
        /// </summary>
        [Browsable(false)]
        public override EmitAssist Emitter
        {
            get
            {
                if (_emitter == null)
                {
                    lock (this)
                    {
                        _emitter = new EmitAssist(this, _constructorBuilder.GetILGenerator());
                    }
                }
                return _emitter;
            }
        }

        #endregion

        #region Constructor Builder operator overloading

        /// <summary>
        /// Converts the supplied <see cref="ConstructorAssist"/> instance to a <see cref="MethodBuilder"/>.
        /// </summary>
        /// <param name="constructorAssist">The <see cref="ConstructorAssist"/>.</param>
        /// <returns>A <see cref="ConstructorBuilder"/>.</returns>
        public static implicit operator ConstructorBuilder(ConstructorAssist constructorAssist)
        {
            if (constructorAssist == null)
                throw new ArgumentNullException("constructorAssist");

            return constructorAssist.ConstructorBuilder;
        }

        #endregion
    }

    #endregion

    #region EmitAssist

    /// <summary>
    /// Emit Assist
    /// </summary>
    public class EmitAssist
    {
        #region static variable

        private static Hashtable _valueTypeHash = null;

        #endregion

        #region Static constructor

        /// <summary>
        /// Constructor (static)
        /// </summary>
        static EmitAssist()
        {
            InitTypes();
        }

        #endregion

        #region Static Method

        #region InitTypes

        /// <summary>
        /// Initialize Types
        /// </summary>
        private static void InitTypes()
        {
            _valueTypeHash = new Hashtable();
            _valueTypeHash[typeof(sbyte)] = OpCodes.Ldind_I1;
            _valueTypeHash[typeof(byte)] = OpCodes.Ldind_U1;
            _valueTypeHash[typeof(char)] = OpCodes.Ldind_U2;
            _valueTypeHash[typeof(short)] = OpCodes.Ldind_I2;
            _valueTypeHash[typeof(ushort)] = OpCodes.Ldind_U2;
            _valueTypeHash[typeof(int)] = OpCodes.Ldind_I4;
            _valueTypeHash[typeof(uint)] = OpCodes.Ldind_U4;
            _valueTypeHash[typeof(long)] = OpCodes.Ldind_I8;
            _valueTypeHash[typeof(ulong)] = OpCodes.Ldind_I8;
            _valueTypeHash[typeof(bool)] = OpCodes.Ldind_I1;
            _valueTypeHash[typeof(double)] = OpCodes.Ldind_R8;
            _valueTypeHash[typeof(float)] = OpCodes.Ldind_R4;
        }

        #endregion

        #region Find OpCode

        /// <summary>
        /// Find OpCode
        /// </summary>
        /// <param name="type">Type to find OpCode</param>
        /// <returns>OpCode that match specificed type (Value Type)</returns>
        public static object FindOpCode(Type type)
        {
            if (type == null)
                return null;
            return _valueTypeHash[type];
        }

        #endregion

        #endregion

        #region Internal Variable

        private readonly MethodAssistBase _method;
        private readonly ILGenerator _ilGenerator;

        private Dictionary<string, Label> _labels = new Dictionary<string, Label>();

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EmitAssist"/> class
        /// with the specified <see cref="System.Reflection.Emit.ILGenerator"/>.
        /// </summary>
        /// <param name="ilGenerator">The <see cref="System.Reflection.Emit.ILGenerator"/> to use.</param>
        public EmitAssist(ILGenerator ilGenerator)
        {
            if (ilGenerator == null)
                throw new ArgumentNullException("ilGenerator");
            _ilGenerator = ilGenerator;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EmitAssist"/> class
        /// with the specified <see cref="System.Reflection.Emit.ILGenerator"/>.
        /// </summary>
        /// <param name="methodAssist">Associated <see cref="MethodAssistBase"/>.</param>
        /// <param name="ilGenerator">The <see cref="System.Reflection.Emit.ILGenerator"/> to use.</param>
        public EmitAssist(MethodAssistBase methodAssist, ILGenerator ilGenerator)
        {
            if (methodAssist == null)
                throw new ArgumentNullException("methodAssist");
            if (ilGenerator == null)
                throw new ArgumentNullException("ilGenerator");

            _method = methodAssist;
            _ilGenerator = ilGenerator;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~EmitAssist()
        {
            ClearLabels();
            _labels = null;
        }

        #endregion

        #region Create Exceptions (for internal error)

        private static Exception CreateNoSuchMethodException(Type type, string methodName)
        {
            return new InvalidOperationException(
                string.Format(ReflectionConsts.Errors.NoSuchMethod, type.FullName, methodName));
        }

        private static Exception CreateNotExpectedTypeException(Type type)
        {
            return new ArgumentException(
                string.Format(ReflectionConsts.Errors.NotExpectedType, type.FullName));
        }

        #endregion

        #region Public Method

        #region ILGenerator Methods - Extended by joe

        /// <summary>
        /// Call Console.WriteLine
        /// </summary>
        /// <param name="value">The text to display on console.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist WriteLine(string value)
        {
            _ilGenerator.EmitWriteLine(value);
            return this;
        }
        /// <summary>
        /// Clear all labels.
        /// </summary>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ClearLabels()
        {
            if (null != _labels) _labels.Clear();
            return this;
        }
        /// <summary>
        /// Define Label.
        /// </summary>
        /// <param name="labelName">The label name to add into local cache.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist DefineLabel(string labelName)
        {
            if (null == _labels) _labels = new Dictionary<string, Label>();
            Label newLabel = _ilGenerator.DefineLabel();
            if (!_labels.ContainsKey(labelName))
                _labels.Add(labelName, newLabel);
            return this;
        }
        /// <summary>
        /// Mark Label.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist MarkLabel(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                Label existLabel = _labels[labelName];
                if (null != existLabel)
                {
                    _ilGenerator.MarkLabel(existLabel);
                }
            }
            return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Beq"/>, label) that
        /// transfers control to a target instruction if two values are equal.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Beq">OpCodes.Beq</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist beq(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return beq(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Beq_S"/>, label) that
        /// transfers control to a target instruction (short form) if two values are equal.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Beq_S">OpCodes.Beq_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist beq_s(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return beq_s(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bge"/>, label) that
        /// transfers control to a target instruction if the first value is greater than or equal to the second value.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Bge">OpCodes.Bge</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bge(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return bge(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bge_S"/>, label) that
        /// transfers control to a target instruction (short form) 
        /// if the first value is greater than or equal to the second value.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Bge_S">OpCodes.Bge_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bge_s(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return bge_s(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bge_Un"/>, label) that
        /// transfers control to a target instruction if the the first value is greather than the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Bge_Un">OpCodes.Bge_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bge_un(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return bge_un(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bge_Un_S"/>, label) that
        /// transfers control to a target instruction (short form) if if the the first value is greather than the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Bge_Un_S">OpCodes.Bge_Un_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bge_un_s(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return bge_un_s(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bgt"/>, label) that
        /// transfers control to a target instruction if the first value is greater than the second value.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Bgt">OpCodes.Bgt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bgt(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return bgt(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bgt_S"/>, label) that
        /// transfers control to a target instruction (short form) if the first value is greater than the second value.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Bgt_S">OpCodes.Bgt_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bgt_s(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return bgt_s(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bgt_Un"/>, label) that
        /// transfers control to a target instruction if the first value is greater than the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Bgt_Un">OpCodes.Bgt_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bgt_un(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return bgt_un(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bgt_Un_S"/>, label) that
        /// transfers control to a target instruction (short form) if the first value is greater than the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Bgt_Un_S">OpCodes.Bgt_Un_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bgt_un_s(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return bgt_un_s(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ble"/>, label) that
        /// transfers control to a target instruction if the first value is less than or equal to the second value.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Ble">OpCodes.Ble</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ble(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return ble(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ble_S"/>, label) that
        /// transfers control to a target instruction (short form) if the first value is less than or equal to the second value.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Ble_S">OpCodes.Ble_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ble_s(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return ble_s(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ble_Un"/>, label) that
        /// transfers control to a target instruction if the first value is less than or equal to the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Ble_Un">OpCodes.Ble_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ble_un(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return ble_un(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ble_Un_S"/>, label) that
        /// transfers control to a target instruction (short form) if the first value is less than or equal to the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Ble_Un_S">OpCodes.Ble_Un_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ble_un_s(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return ble_un_s(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Blt"/>, label) that
        /// transfers control to a target instruction if the first value is less than the second value.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Blt">OpCodes.Blt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist blt(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return blt(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Blt_S"/>, label) that
        /// transfers control to a target instruction (short form) if the first value is less than the second value.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Blt_S">OpCodes.Blt_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist blt_s(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return blt_s(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Blt_Un"/>, label) that
        /// transfers control to a target instruction if the first value is less than the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Blt_Un">OpCodes.Blt_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist blt_un(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return blt_un(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Blt_Un_S"/>, label) that
        /// transfers control to a target instruction (short form) if the first value is less than the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Blt_Un_S">OpCodes.Blt_Un_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist blt_un_s(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return blt_un_s(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bne_Un"/>, label) that
        /// transfers control to a target instruction when two unsigned integer values or unordered float values are not equal.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Bne_Un">OpCodes.Bne_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bne_un(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return bne_un(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bne_Un_S"/>, label) that
        /// transfers control to a target instruction (short form) 
        /// when two unsigned integer values or unordered float values are not equal.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Bne_Un_S">OpCodes.Bne_Un_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bne_un_s(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return bne_un_s(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Br"/>, label) that
        /// unconditionally transfers control to a target instruction. 
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Br">OpCodes.Br</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist br(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return br(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Brfalse"/>, label) that
        /// transfers control to a target instruction if value is false, a null reference (Nothing in Visual Basic), or zero.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Brfalse">OpCodes.Brfalse</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist brfalse(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return brfalse(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Brfalse_S"/>, label) that
        /// transfers control to a target instruction if value is false, a null reference, or zero. 
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Brfalse_S">OpCodes.Brfalse_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist brfalse_s(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return brfalse_s(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Brtrue"/>, label) that
        /// transfers control to a target instruction if value is true, not null, or non-zero.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Brtrue">OpCodes.Brtrue</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist brtrue(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return brtrue(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Brtrue_S"/>, label) that
        /// transfers control to a target instruction (short form) if value is true, not null, or non-zero.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Brtrue_S">OpCodes.Brtrue_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist brtrue_s(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return brtrue_s(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Br_S"/>, label) that
        /// unconditionally transfers control to a target instruction (short form).
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Br_S">OpCodes.Br_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist br_s(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return br_s(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Leave"/>, label) that
        /// exits a protected region of code, unconditionally tranferring control to a specific target instruction.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Leave">OpCodes.Leave</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist leave(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return leave(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Leave_S"/>, label) that
        /// exits a protected region of code, unconditionally transferring control to a target instruction (short form).
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Leave_S">OpCodes.Leave_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist leave_s(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return leave_s(_labels[labelName]);
            }
            else return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Switch"/>, label[]) that
        /// implements a jump table.
        /// </summary>
        /// <param name="labelNames">The array of label names in local cache.</param>
        /// <seealso cref="OpCodes.Switch">OpCodes.Switch</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label[])">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist @switch(string[] labelNames)
        {
            if (null == _labels || _labels.Count <= 0 ||
                null == labelNames || labelNames.Length <= 0)
                return this;
            List<Label> labels = new List<Label>();
            foreach (string labelName in labelNames)
            {
                if (!_labels.ContainsKey(labelName) || null == _labels[labelName])
                    continue;
                labels.Add(_labels[labelName]);
            }
            return @switch(labels.ToArray());
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Unaligned"/>, label) that
        /// indicates that an address currently atop the evaluation stack might not be aligned 
        /// to the natural size of the immediately following ldind, stind, ldfld, stfld, ldobj, stobj, 
        /// initblk, or cpblk instruction.
        /// </summary>
        /// <param name="labelName">The label name in local cache.</param>
        /// <seealso cref="OpCodes.Unaligned">OpCodes.Unaligned</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist unaligned(string labelName)
        {
            if (null != _labels && _labels.ContainsKey(labelName))
            {
                return unaligned(_labels[labelName]);
            }
            else return this;
        }

        #endregion

        #region ILGenerator Methods

        /// <summary>
        /// Begins a catch block.
        /// </summary>
        /// <param name="exceptionType">The Type object that represents the exception.</param>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.BeginCatchBlock(Type)">ILGenerator.BeginCatchBlock Method</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist BeginCatchBlock(Type exceptionType)
        {
            _ilGenerator.BeginCatchBlock(exceptionType);
            return this;
        }
        /// <summary>
        /// Begins an exception block for a filtered exception.
        /// </summary>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.BeginExceptFilterBlock">ILGenerator.BeginCatchBlock Method</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist BeginExceptFilterBlock()
        {
            _ilGenerator.BeginExceptFilterBlock();
            return this;
        }
        /// <summary>
        /// Begins an exception block for a non-filtered exception.
        /// </summary>
        /// <returns>The label for the end of the block.</returns>
        public Label BeginExceptionBlock()
        {
            return _ilGenerator.BeginExceptionBlock();
        }
        /// <summary>
        /// Begins an exception fault block in the Microsoft intermediate language (MSIL) stream.
        /// </summary>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist BeginFaultBlock()
        {
            _ilGenerator.BeginFaultBlock();
            return this;
        }
        /// <summary>
        /// Begins a finally block in the Microsoft intermediate language (MSIL) instruction stream.
        /// </summary>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist BeginFinallyBlock()
        {
            _ilGenerator.BeginFinallyBlock();
            return this;
        }
        /// <summary>
        /// Begins a lexical scope.
        /// </summary>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist BeginScope()
        {
            _ilGenerator.BeginScope();
            return this;
        }
        /// <summary>
        /// Declares a local variable.
        /// </summary>
        /// <param name="localType">The Type of the local variable.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        /*/// <returns>The declared local variable.</returns>*/
        public EmitAssist DeclareLocal(Type localType)
        //public LocalBuilder DeclareLocal(Type localType)
        {
            //return _ilGenerator.DeclareLocal(localType);
            _ilGenerator.DeclareLocal(localType);
            return this;
        }
        /// <summary>
        /// Declares a local variable, optionally pinning the object referred to by the variable.
        /// </summary>
        /// <param name="localType">The Type of the local variable.</param>
        /// <param name="pinned"><b>true</b> to pin the object in memory; otherwise, <b>false</b>.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        /*/// <returns>The declared local variable.</returns>*/
        public EmitAssist DeclareLocal(Type localType, bool pinned)
        //public LocalBuilder DeclareLocal(Type localType, bool pinned)
        {
            //return _ilGenerator.DeclareLocal(localType, pinned);
            _ilGenerator.DeclareLocal(localType, pinned);
            return this;
        }
        /// <summary>
        /// Declares a new label.
        /// </summary>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        /*/// <returns>Returns a new label that can be used as a token for branching.</returns>*/
        public EmitAssist DefineLabel()
        //public Label DefineLabel()
        {
            //return _ilGenerator.DefineLabel();
            _ilGenerator.DefineLabel();
            return this;
        }
        /// <summary>
        /// Ends an exception block.
        /// </summary>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist EndExceptionBlock()
        {
            _ilGenerator.EndExceptionBlock();
            return this;
        }
        /// <summary>
        /// Ends a lexical scope.
        /// </summary>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist EndScope()
        {
            _ilGenerator.EndScope();
            return this;
        }
        /// <summary>
        /// Marks the Microsoft intermediate language (MSIL) stream's current position 
        /// with the given label.
        /// </summary>
        /// <param name="loc">The label for which to set an index.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist MarkLabel(Label loc)
        {
            _ilGenerator.MarkLabel(loc);
            return this;
        }
        /// <summary>
        /// Marks a sequence point in the Microsoft intermediate language (MSIL) stream.
        /// </summary>
        /// <param name="document">The document for which the sequence point is being defined.</param>
        /// <param name="startLine">The line where the sequence point begins.</param>
        /// <param name="startColumn">The column in the line where the sequence point begins.</param>
        /// <param name="endLine">The line where the sequence point ends.</param>
        /// <param name="endColumn">The column in the line where the sequence point ends.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist MarkSequencePoint(ISymbolDocumentWriter document,
            int startLine, int startColumn,
            int endLine, int endColumn)
        {
            _ilGenerator.MarkSequencePoint(document, startLine, startColumn, endLine, endColumn);
            return this;
        }
        /// <summary>
        /// Emits an instruction to throw an exception.
        /// </summary>
        /// <param name="exceptionType">The class of the type of exception to throw.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ThrowException(Type exceptionType)
        {
            _ilGenerator.ThrowException(exceptionType);
            return this;
        }
        /// <summary>
        /// Specifies the namespace to be used in evaluating locals and watches for 
        /// the current active lexical scope.
        /// </summary>
        /// <param name="namespaceName">The namespace to be used in evaluating locals and watches for the current active lexical scope.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist UsingNamespace(string namespaceName)
        {
            _ilGenerator.UsingNamespace(namespaceName);
            return this;
        }

        #endregion

        #region Emit Wrappers

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Add"/>) that
        /// adds two values and pushes the result onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Add">OpCodes.Add</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist add
        {
            get { _ilGenerator.Emit(OpCodes.Add); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Add_Ovf"/>) that
        /// adds two integers, performs an overflow check, and pushes the result onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Add_Ovf">OpCodes.Add_Ovf</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist add_ovf
        {
            get { _ilGenerator.Emit(OpCodes.Add_Ovf); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Add_Ovf_Un"/>) that
        /// adds two unsigned integer values, performs an overflow check, and pushes the result onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Add_Ovf_Un">OpCodes.Add_Ovf_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist add_ovf_un
        {
            get { _ilGenerator.Emit(OpCodes.Add_Ovf_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.And"/>) that
        /// computes the bitwise AND of two values and pushes the result onto the evalution stack.
        /// </summary>
        /// <seealso cref="OpCodes.And">OpCodes.And</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist and
        {
            get { _ilGenerator.Emit(OpCodes.And); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Arglist"/>) that
        /// returns an unmanaged pointer to the argument list of the current method.
        /// </summary>
        /// <seealso cref="OpCodes.Arglist">OpCodes.Arglist</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist arglist
        {
            get { _ilGenerator.Emit(OpCodes.Arglist); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Beq"/>, label) that
        /// transfers control to a target instruction if two values are equal.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Beq">OpCodes.Beq</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist beq(Label label)
        {
            _ilGenerator.Emit(OpCodes.Beq, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Beq_S"/>, label) that
        /// transfers control to a target instruction (short form) if two values are equal.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Beq_S">OpCodes.Beq_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist beq_s(Label label)
        {
            _ilGenerator.Emit(OpCodes.Beq_S, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bge"/>, label) that
        /// transfers control to a target instruction if the first value is greater than or equal to the second value.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bge">OpCodes.Bge</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bge(Label label)
        {
            _ilGenerator.Emit(OpCodes.Bge, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bge_S"/>, label) that
        /// transfers control to a target instruction (short form) 
        /// if the first value is greater than or equal to the second value.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bge_S">OpCodes.Bge_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bge_s(Label label)
        {
            _ilGenerator.Emit(OpCodes.Bge_S, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bge_Un"/>, label) that
        /// transfers control to a target instruction if the the first value is greather than the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bge_Un">OpCodes.Bge_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bge_un(Label label)
        {
            _ilGenerator.Emit(OpCodes.Bge_Un, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bge_Un_S"/>, label) that
        /// transfers control to a target instruction (short form) if if the the first value is greather than the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bge_Un_S">OpCodes.Bge_Un_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bge_un_s(Label label)
        {
            _ilGenerator.Emit(OpCodes.Bge_Un_S, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bgt"/>, label) that
        /// transfers control to a target instruction if the first value is greater than the second value.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bgt">OpCodes.Bgt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bgt(Label label)
        {
            _ilGenerator.Emit(OpCodes.Bgt, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bgt_S"/>, label) that
        /// transfers control to a target instruction (short form) if the first value is greater than the second value.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bgt_S">OpCodes.Bgt_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bgt_s(Label label)
        {
            _ilGenerator.Emit(OpCodes.Bgt_S, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bgt_Un"/>, label) that
        /// transfers control to a target instruction if the first value is greater than the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bgt_Un">OpCodes.Bgt_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bgt_un(Label label)
        {
            _ilGenerator.Emit(OpCodes.Bgt_Un, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bgt_Un_S"/>, label) that
        /// transfers control to a target instruction (short form) if the first value is greater than the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bgt_Un_S">OpCodes.Bgt_Un_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bgt_un_s(Label label)
        {
            _ilGenerator.Emit(OpCodes.Bgt_Un_S, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ble"/>, label) that
        /// transfers control to a target instruction if the first value is less than or equal to the second value.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Ble">OpCodes.Ble</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ble(Label label)
        {
            _ilGenerator.Emit(OpCodes.Ble, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ble_S"/>, label) that
        /// transfers control to a target instruction (short form) if the first value is less than or equal to the second value.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Ble_S">OpCodes.Ble_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ble_s(Label label)
        {
            _ilGenerator.Emit(OpCodes.Ble_S, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ble_Un"/>, label) that
        /// transfers control to a target instruction if the first value is less than or equal to the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Ble_Un">OpCodes.Ble_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ble_un(Label label)
        {
            _ilGenerator.Emit(OpCodes.Ble_Un, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ble_Un_S"/>, label) that
        /// transfers control to a target instruction (short form) if the first value is less than or equal to the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Ble_Un_S">OpCodes.Ble_Un_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ble_un_s(Label label)
        {
            _ilGenerator.Emit(OpCodes.Ble_Un_S, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Blt"/>, label) that
        /// transfers control to a target instruction if the first value is less than the second value.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Blt">OpCodes.Blt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist blt(Label label)
        {
            _ilGenerator.Emit(OpCodes.Blt, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Blt_S"/>, label) that
        /// transfers control to a target instruction (short form) if the first value is less than the second value.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Blt_S">OpCodes.Blt_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist blt_s(Label label)
        {
            _ilGenerator.Emit(OpCodes.Blt_S, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Blt_Un"/>, label) that
        /// transfers control to a target instruction if the first value is less than the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Blt_Un">OpCodes.Blt_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist blt_un(Label label)
        {
            _ilGenerator.Emit(OpCodes.Blt_Un, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Blt_Un_S"/>, label) that
        /// transfers control to a target instruction (short form) if the first value is less than the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Blt_Un_S">OpCodes.Blt_Un_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist blt_un_s(Label label)
        {
            _ilGenerator.Emit(OpCodes.Blt_Un_S, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bne_Un"/>, label) that
        /// transfers control to a target instruction when two unsigned integer values or unordered float values are not equal.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bne_Un">OpCodes.Bne_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bne_un(Label label)
        {
            _ilGenerator.Emit(OpCodes.Bne_Un, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bne_Un_S"/>, label) that
        /// transfers control to a target instruction (short form) 
        /// when two unsigned integer values or unordered float values are not equal.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bne_Un_S">OpCodes.Bne_Un_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist bne_un_s(Label label)
        {
            _ilGenerator.Emit(OpCodes.Bne_Un_S, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Box"/>, type) that
        /// converts a value type to an object reference.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Box">OpCodes.Box</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist box(Type type)
        {
            _ilGenerator.Emit(OpCodes.Box, type); return this;
        }
        /// <summary>
        /// Converts a value type to an object reference if the value is a value type.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Box">OpCodes.Box</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist boxIfValueType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return type.IsValueType ? box(type) : this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Br"/>, label) that
        /// unconditionally transfers control to a target instruction. 
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Br">OpCodes.Br</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist br(Label label)
        {
            _ilGenerator.Emit(OpCodes.Br, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Break"/>) that
        /// signals the Common Language Infrastructure (CLI) to inform the debugger that a break point has been tripped.
        /// </summary>
        /// <seealso cref="OpCodes.Break">OpCodes.Break</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist @break
        {
            get { _ilGenerator.Emit(OpCodes.Break); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Brfalse"/>, label) that
        /// transfers control to a target instruction if value is false, a null reference (Nothing in Visual Basic), or zero.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Brfalse">OpCodes.Brfalse</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist brfalse(Label label)
        {
            _ilGenerator.Emit(OpCodes.Brfalse, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Brfalse_S"/>, label) that
        /// transfers control to a target instruction if value is false, a null reference, or zero. 
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Brfalse_S">OpCodes.Brfalse_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist brfalse_s(Label label)
        {
            _ilGenerator.Emit(OpCodes.Brfalse_S, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Brtrue"/>, label) that
        /// transfers control to a target instruction if value is true, not null, or non-zero.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Brtrue">OpCodes.Brtrue</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist brtrue(Label label)
        {
            _ilGenerator.Emit(OpCodes.Brtrue, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Brtrue_S"/>, label) that
        /// transfers control to a target instruction (short form) if value is true, not null, or non-zero.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Brtrue_S">OpCodes.Brtrue_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist brtrue_s(Label label)
        {
            _ilGenerator.Emit(OpCodes.Brtrue_S, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Br_S"/>, label) that
        /// unconditionally transfers control to a target instruction (short form).
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Br_S">OpCodes.Br_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist br_s(Label label)
        {
            _ilGenerator.Emit(OpCodes.Br_S, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Call"/>, methodInfo) that
        /// calls the method indicated by the passed method descriptor.
        /// </summary>
        /// <param name="methodInfo">The method to be called.</param>
        /// <seealso cref="OpCodes.Call">OpCodes.Call</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,MethodInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist call(MethodInfo methodInfo)
        {
            _ilGenerator.Emit(OpCodes.Call, methodInfo); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Call"/>, constructorInfo) that
        /// calls the method indicated by the passed method descriptor.
        /// </summary>
        /// <param name="constructorInfo">The constructor to be called.</param>
        /// <seealso cref="OpCodes.Call">OpCodes.Call</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,MethodInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist call(ConstructorInfo constructorInfo)
        {
            _ilGenerator.Emit(OpCodes.Call, constructorInfo); return this;
        }
        /// <summary>
        /// Calls ILGenerator.EmitCall(<see cref="OpCodes.Call"/>, methodInfo, optionalParameterTypes) that
        /// calls the method indicated by the passed method descriptor.
        /// </summary>
        /// <param name="methodInfo">The method to be called.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method.</param>
        /// <seealso cref="OpCodes.Call">OpCodes.Call</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist call(MethodInfo methodInfo, Type[] optionalParameterTypes)
        {
            _ilGenerator.EmitCall(OpCodes.Call, methodInfo, optionalParameterTypes); return this;
        }
        /// <summary>
        /// Calls ILGenerator.EmitCall(<see cref="OpCodes.Call"/>, methodInfo, optionalParameterTypes) that
        /// calls the method indicated by the passed method descriptor.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <param name="methodName">The name of the method to be called.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method.</param>
        /// <seealso cref="OpCodes.Call">OpCodes.Call</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist call(Type type, string methodName, params Type[] optionalParameterTypes)
        {
            if (type == null) throw new ArgumentNullException("type");

            MethodInfo methodInfo = type.GetMethod(methodName, optionalParameterTypes);

            if (methodInfo == null)
                throw CreateNoSuchMethodException(type, methodName);

            return call(methodInfo);
        }
        /// <summary>
        /// Calls ILGenerator.EmitCall(<see cref="OpCodes.Call"/>, methodInfo, optionalParameterTypes) that
        /// calls the method indicated by the passed method descriptor.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <param name="methodName">The name of the method to be called.</param>
        /// <param name="flags">A bitmask comprised of one or more <see cref="BindingFlags"/> 
        /// that specify how the search is conducted.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method.</param>
        /// <seealso cref="OpCodes.Call">OpCodes.Call</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist call(Type type, string methodName, BindingFlags flags, params Type[] optionalParameterTypes)
        {
            if (type == null) throw new ArgumentNullException("type");

            MethodInfo methodInfo = type.GetMethod(methodName, flags, null, optionalParameterTypes, null);

            if (methodInfo == null)
                throw CreateNoSuchMethodException(type, methodName);

            return call(methodInfo);
        }
        /// <summary>
        /// Calls ILGenerator.EmitCalli(<see cref="OpCodes.Calli"/>, <see cref="CallingConvention"/>, Type, Type[]) that
        /// calls the method indicated on the evaluation stack (as a pointer to an entry point) 
        /// with arguments described by a calling convention using an unmanaged calling convention.
        /// </summary>
        /// <param name="unmanagedCallConv">The unmanaged calling convention to be used.</param>
        /// <param name="returnType">The Type of the result.</param>
        /// <param name="parameterTypes">The types of the required arguments to the instruction.</param>
        /// <seealso cref="OpCodes.Calli">OpCodes.Calli</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCalli(OpCode,CallingConvention,Type,Type[])">ILGenerator.EmitCalli</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist calli(CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes)
        {
            _ilGenerator.EmitCalli(OpCodes.Calli, unmanagedCallConv, returnType, parameterTypes); return this;
        }
        /// <summary>
        /// Calls ILGenerator.EmitCalli(<see cref="OpCodes.Calli"/>, <see cref="CallingConvention"/>, Type, Type[], Type[]) that
        /// calls the method indicated on the evaluation stack (as a pointer to an entry point)
        /// with arguments described by a calling convention using a managed calling convention.
        /// </summary>
        /// <param name="callingConvention">The managed calling convention to be used.</param>
        /// <param name="returnType">The Type of the result.</param>
        /// <param name="parameterTypes">The types of the required arguments to the instruction.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments for vararg calls.</param>
        /// <seealso cref="OpCodes.Calli">OpCodes.Calli</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCalli(OpCode,CallingConventions,Type,Type[],Type[])">ILGenerator.EmitCalli</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist calli(CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
        {
            _ilGenerator.EmitCalli(OpCodes.Calli, callingConvention, returnType, parameterTypes, optionalParameterTypes);
            return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Callvirt"/>, methodInfo) that
        /// calls a late-bound method on an object, pushing the return value onto the evaluation stack.
        /// </summary>
        /// <param name="methodInfo">The method to be called.</param>
        /// <seealso cref="OpCodes.Callvirt">OpCodes.Callvirt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,MethodInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist callvirt(MethodInfo methodInfo)
        {
            _ilGenerator.Emit(OpCodes.Callvirt, methodInfo); return this;
        }
        /// <summary>
        /// Calls ILGenerator.EmitCall(<see cref="OpCodes.Callvirt"/>, methodInfo, optionalParameterTypes) that
        /// calls a late-bound method on an object, pushing the return value onto the evaluation stack.
        /// </summary>
        /// <param name="methodInfo">The method to be called.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method.</param>
        /// <seealso cref="OpCodes.Callvirt">OpCodes.Callvirt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist callvirt(MethodInfo methodInfo, Type[] optionalParameterTypes)
        {
            _ilGenerator.EmitCall(OpCodes.Callvirt, methodInfo, optionalParameterTypes); return this;
        }
        /// <summary>
        /// Calls ILGenerator.EmitCall(<see cref="OpCodes.Callvirt"/>, methodInfo, optionalParameterTypes) that
        /// calls a late-bound method on an object, pushing the return value onto the evaluation stack.
        /// </summary>
        /// <param name="methodName">The method to be called.</param>
        /// <param name="type">The declaring type of the method.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method.</param>
        /// <seealso cref="OpCodes.Callvirt">OpCodes.Callvirt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist callvirt(Type type, string methodName, params Type[] optionalParameterTypes)
        {
            if (type == null) throw new ArgumentNullException("type");

            MethodInfo methodInfo = type.GetMethod(methodName, optionalParameterTypes);

            if (methodInfo == null)
                throw CreateNoSuchMethodException(type, methodName);

            return callvirt(methodInfo);
        }
        /// <summary>
        /// Calls ILGenerator.EmitCall(<see cref="OpCodes.Callvirt"/>, methodInfo, optionalParameterTypes) that
        /// calls a late-bound method on an object, pushing the return value onto the evaluation stack.
        /// </summary>
        /// <param name="methodName">The method to be called.</param>
        /// <param name="type">The declaring type of the method.</param>
        /// <param name="flags">A bitmask comprised of one or more <see cref="BindingFlags"/> 
        /// that specify how the search is conducted.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method.</param>
        /// <seealso cref="OpCodes.Callvirt">OpCodes.Callvirt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist callvirt(Type type, string methodName, BindingFlags flags, params Type[] optionalParameterTypes)
        {
            MethodInfo methodInfo =
                optionalParameterTypes == null ?
                    type.GetMethod(methodName, flags) :
                    type.GetMethod(methodName, flags, null, optionalParameterTypes, null);

            if (methodInfo == null)
                throw CreateNoSuchMethodException(type, methodName);

            return callvirt(methodInfo, null);
        }
        /// <summary>
        /// Calls ILGenerator.EmitCall(<see cref="OpCodes.Callvirt"/>, methodInfo, optionalParameterTypes) that
        /// calls a late-bound method on an object, pushing the return value onto the evaluation stack.
        /// </summary>
        /// <param name="methodName">The method to be called.</param>
        /// <param name="type">The declaring type of the method.</param>
        /// <param name="flags">A bitmask comprised of one or more <see cref="BindingFlags"/> 
        /// that specify how the search is conducted.</param>
        /// <seealso cref="OpCodes.Callvirt">OpCodes.Callvirt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist callvirt(Type type, string methodName, BindingFlags flags)
        {
            return callvirt(type, methodName, flags, null);
        }
        /// <summary>
        /// Calls ILGenerator.EmitCall(<see cref="OpCodes.Callvirt"/>, methodInfo, optionalParameterTypes) that
        /// calls a late-bound method on an object, pushing the return value onto the evaluation stack.
        /// </summary>
        /// <param name="methodName">The non-generic method to be called.</param>
        /// <param name="type">The declaring type of the method.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method.</param>
        /// <seealso cref="OpCodes.Callvirt">OpCodes.Callvirt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist callvirtNoGenerics(Type type, string methodName, params Type[] optionalParameterTypes)
        {
            MethodInfo methodInfo = type.GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public,
                GenericBinder.NonGeneric,
                optionalParameterTypes, null);

            if (methodInfo == null)
                throw CreateNoSuchMethodException(type, methodName);

            return callvirt(methodInfo);
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Castclass"/>, type) that
        /// attempts to cast an object passed by reference to the specified class.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Castclass">OpCodes.Castclass</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist castclass(Type type)
        {
            _ilGenerator.Emit(OpCodes.Castclass, type); return this;
        }
        /// <summary>
        /// Attempts to cast an object passed by reference to the specified class 
        /// or to unbox if the type is a value type.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist castType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return type.IsValueType ? unbox_any(type) : castclass(type);
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ceq"/>) that
        /// compares two values. If they are equal, the integer value 1 (int32) is pushed onto the evaluation stack;
        /// otherwise 0 (int32) is pushed onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Ceq">OpCodes.Ceq</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ceq
        {
            get { _ilGenerator.Emit(OpCodes.Ceq); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Cgt"/>) that
        /// compares two values. If the first value is greater than the second,
        /// the integer value 1 (int32) is pushed onto the evaluation stack;
        /// otherwise 0 (int32) is pushed onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Cgt">OpCodes.Cgt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist cgt
        {
            get { _ilGenerator.Emit(OpCodes.Cgt); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Cgt_Un"/>) that
        /// compares two unsigned or unordered values.
        /// If the first value is greater than the second, the integer value 1 (int32) is pushed onto the evaluation stack;
        /// otherwise 0 (int32) is pushed onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Cgt_Un">OpCodes.Cgt_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist cgt_un
        {
            get { _ilGenerator.Emit(OpCodes.Cgt_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Constrained"/>) that
        /// constrains the type on which a virtual method call is made.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Cgt_Un">OpCodes.Constrained</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist constrained(Type type)
        {
            _ilGenerator.Emit(OpCodes.Constrained, type); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ckfinite"/>) that
        /// throws <see cref="ArithmeticException"/> if value is not a finite number.
        /// </summary>
        /// <seealso cref="OpCodes.Ckfinite">OpCodes.Ckfinite</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ckfinite
        {
            get { _ilGenerator.Emit(OpCodes.Ckfinite); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Clt"/>) that
        /// compares two values. If the first value is less than the second,
        /// the integer value 1 (int32) is pushed onto the evaluation stack;
        /// otherwise 0 (int32) is pushed onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Clt">OpCodes.Clt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist clt
        {
            get { _ilGenerator.Emit(OpCodes.Clt); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Clt_Un"/>) that
        /// compares the unsigned or unordered values value1 and value2.
        /// If value1 is less than value2, then the integer value 1 (int32) is pushed onto the evaluation stack;
        /// otherwise 0 (int32) is pushed onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Clt_Un">OpCodes.Clt_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist clt_un
        {
            get { _ilGenerator.Emit(OpCodes.Clt_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_I"/>) that
        /// converts the value on top of the evaluation stack to natural int.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_I">OpCodes.Conv_I</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_i
        {
            get { _ilGenerator.Emit(OpCodes.Conv_I); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_I1"/>) that
        /// converts the value on top of the evaluation stack to int8, then extends (pads) it to int32.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_I1">OpCodes.Conv_I1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_i1
        {
            get { _ilGenerator.Emit(OpCodes.Conv_I1); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_I2"/>) that
        /// converts the value on top of the evaluation stack to int16, then extends (pads) it to int32.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_I2">OpCodes.Conv_I2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_i2
        {
            get { _ilGenerator.Emit(OpCodes.Conv_I2); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_I4"/>) that
        /// converts the value on top of the evaluation stack to int32.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_I4">OpCodes.Conv_I4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_i4
        {
            get { _ilGenerator.Emit(OpCodes.Conv_I4); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_I8"/>) that
        /// converts the value on top of the evaluation stack to int64.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_I8">OpCodes.Conv_I8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_i8
        {
            get { _ilGenerator.Emit(OpCodes.Conv_I8); return this; }
        }
        /// <summary>
        /// Converts the value on top of the evaluation stack to the specified type.
        /// </summary>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte: conv_i1.end(); break;
                case TypeCode.Int16: conv_i2.end(); break;
                case TypeCode.Int32: conv_i4.end(); break;
                case TypeCode.Int64: conv_i8.end(); break;

                case TypeCode.Byte: conv_u1.end(); break;
                case TypeCode.Char:
                case TypeCode.UInt16: conv_u2.end(); break;
                case TypeCode.UInt32: conv_u4.end(); break;
                case TypeCode.UInt64: conv_u8.end(); break;

                case TypeCode.Single: conv_r4.end(); break;
                case TypeCode.Double: conv_r8.end(); break;

                default:
                    {
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            ConstructorInfo ci = type.GetConstructor(type.GetGenericArguments());
                            if (ci != null)
                            {
                                newobj(ci);
                                break;
                            }
                        }

                        throw CreateNotExpectedTypeException(type);
                    }
            }

            return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I"/>) that
        /// converts the signed value on top of the evaluation stack to signed natural int,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_I">OpCodes.Conv_Ovf_I</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_i
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_I); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I1"/>) that
        /// converts the signed value on top of the evaluation stack to signed int8 and extends it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_I1">OpCodes.Conv_Ovf_I1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_i1
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_I1); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I1_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to signed int8 and extends it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_I1_Un">OpCodes.Conv_Ovf_I1_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_i1_un
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_I1_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I2"/>) that
        /// converts the signed value on top of the evaluation stack to signed int16 and extending it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_I2">OpCodes.Conv_Ovf_I2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_i2
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_I2); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I2_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to signed int16 and extends it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_I2_Un">OpCodes.Conv_Ovf_I2_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_i2_un
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_I2_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I4"/>) that
        /// converts the signed value on top of the evaluation tack to signed int32, throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_I4">OpCodes.Conv_Ovf_I4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_i4
        {
            get
            {
                //_ilGenerator.Emit(OpCodes.Conv_Ovf_I2_Un); return this; // <- maybe bug in original code
                _ilGenerator.Emit(OpCodes.Conv_Ovf_I4); return this;
            }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I4_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to signed int32, throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_I4_Un">OpCodes.Conv_Ovf_I4_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_i4_un
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_I4_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I8"/>) that
        /// converts the signed value on top of the evaluation stack to signed int64,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_I8">OpCodes.Conv_Ovf_I8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_i8
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_I8); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I8_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to signed int64, throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_I8_Un">OpCodes.Conv_Ovf_I8_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_i8_un
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_I8_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to signed natural int,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_I_Un">OpCodes.Conv_Ovf_I_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_i_un
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_I_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U"/>) that
        /// converts the signed value on top of the evaluation stack to unsigned natural int,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_U">OpCodes.Conv_Ovf_U</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_u
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_U); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U1"/>) that
        /// converts the signed value on top of the evaluation stack to unsigned int8 and extends it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_U1">OpCodes.Conv_Ovf_U1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_u1
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_U1); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U1_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to unsigned int8 and extends it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_U1_Un">OpCodes.Conv_Ovf_U1_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_u1_un
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_U1_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U2"/>) that
        /// converts the signed value on top of the evaluation stack to unsigned int16 and extends it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_U2">OpCodes.Conv_Ovf_U2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_u2
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_U2); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U2_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to unsigned int16 and extends it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_U2_Un">OpCodes.Conv_Ovf_U2_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_u2_un
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_U2_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U4"/>) that
        /// Converts the signed value on top of the evaluation stack to unsigned int32, throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_U4">OpCodes.Conv_Ovf_U4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_u4
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_U4); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U4_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to unsigned int32, throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_U4_Un">OpCodes.Conv_Ovf_U4_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_u4_un
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_U4_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U8"/>) that
        /// converts the signed value on top of the evaluation stack to unsigned int64, throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_U8">OpCodes.Conv_Ovf_U8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_u8
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_U8); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U8_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to unsigned int64, throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_U8_Un">OpCodes.Conv_Ovf_U8_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_u8_un
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_U8_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to unsigned natural int,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_Ovf_U_Un">OpCodes.Conv_Ovf_U_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_ovf_u_un
        {
            get { _ilGenerator.Emit(OpCodes.Conv_Ovf_U_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_R4"/>) that
        /// converts the value on top of the evaluation stack to float32.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_R4">OpCodes.Conv_R4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_r4
        {
            get { _ilGenerator.Emit(OpCodes.Conv_R4); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_R8"/>) that
        /// converts the value on top of the evaluation stack to float64.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_R8">OpCodes.Conv_R8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_r8
        {
            get { _ilGenerator.Emit(OpCodes.Conv_R8); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_R_Un"/>) that
        /// converts the unsigned integer value on top of the evaluation stack to float32.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_R_Un">OpCodes.Conv_R_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_r_un
        {
            get { _ilGenerator.Emit(OpCodes.Conv_R_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_U"/>) that
        /// converts the value on top of the evaluation stack to unsigned natural int, and extends it to natural int.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_U">OpCodes.Conv_U</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_u
        {
            get { _ilGenerator.Emit(OpCodes.Conv_U); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_U1"/>) that
        /// converts the value on top of the evaluation stack to unsigned int8, and extends it to int32.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_U1">OpCodes.Conv_U1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_u1
        {
            get { _ilGenerator.Emit(OpCodes.Conv_U1); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_U2"/>) that
        /// converts the value on top of the evaluation stack to unsigned int16, and extends it to int32.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_U2">OpCodes.Conv_U2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_u2
        {
            get { _ilGenerator.Emit(OpCodes.Conv_U2); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_U4"/>) that
        /// converts the value on top of the evaluation stack to unsigned int32, and extends it to int32.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_U4">OpCodes.Conv_U4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_u4
        {
            get { _ilGenerator.Emit(OpCodes.Conv_U4); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_U8"/>) that
        /// converts the value on top of the evaluation stack to unsigned int64, and extends it to int64.
        /// </summary>
        /// <seealso cref="OpCodes.Conv_U8">OpCodes.Conv_U8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist conv_u8
        {
            get { _ilGenerator.Emit(OpCodes.Conv_U8); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Cpblk"/>) that
        /// copies a specified number bytes from a source address to a destination address.
        /// </summary>
        /// <seealso cref="OpCodes.Cpblk">OpCodes.Cpblk</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist cpblk
        {
            get { _ilGenerator.Emit(OpCodes.Cpblk); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Cpobj"/>, type) that
        /// copies the value type located at the address of an object (type &amp;, * or natural int) 
        /// to the address of the destination object (type &amp;, * or natural int).
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Cpobj">OpCodes.Cpobj</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist cpobj(Type type)
        {
            _ilGenerator.Emit(OpCodes.Cpobj, type); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Div"/>) that
        /// divides two values and pushes the result as a floating-point (type F) or
        /// quotient (type int32) onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Div">OpCodes.Div</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist div
        {
            get { _ilGenerator.Emit(OpCodes.Div); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Div_Un"/>) that
        /// divides two unsigned integer values and pushes the result (int32) onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Div_Un">OpCodes.Div_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist div_un
        {
            get { _ilGenerator.Emit(OpCodes.Div_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Dup"/>) that
        /// copies the current topmost value on the evaluation stack, and then pushes the copy onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Dup">OpCodes.Dup</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist dup
        {
            get { _ilGenerator.Emit(OpCodes.Dup); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Endfilter"/>) that
        /// transfers control from the filter clause of an exception back to
        /// the Common Language Infrastructure (CLI) exception handler.
        /// </summary>
        /// <seealso cref="OpCodes.Endfilter">OpCodes.Endfilter</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist endfilter
        {
            get { _ilGenerator.Emit(OpCodes.Endfilter); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Endfinally"/>) that
        /// transfers control from the fault or finally clause of an exception block back to
        /// the Common Language Infrastructure (CLI) exception handler.
        /// </summary>
        /// <seealso cref="OpCodes.Endfinally">OpCodes.Endfinally</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist endfinally
        {
            get { _ilGenerator.Emit(OpCodes.Endfinally); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Initblk"/>) that
        /// initializes a specified block of memory at a specific address to a given size and initial value.
        /// </summary>
        /// <seealso cref="OpCodes.Initblk">OpCodes.Initblk</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist initblk
        {
            get { _ilGenerator.Emit(OpCodes.Initblk); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Initobj"/>, type) that
        /// initializes all the fields of the object at a specific address to a null reference or 
        /// a 0 of the appropriate primitive type.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Initobj">OpCodes.Initobj</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist initobj(Type type)
        {
            _ilGenerator.Emit(OpCodes.Initobj, type); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Isinst"/>, type) that
        /// tests whether an object reference (type O) is an instance of a particular class.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Isinst">OpCodes.Isinst</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist isinst(Type type)
        {
            _ilGenerator.Emit(OpCodes.Isinst, type); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Jmp"/>, methodInfo) that
        /// exits current method and jumps to specified method.
        /// </summary>
        /// <param name="methodInfo">The method to be jumped.</param>
        /// <seealso cref="OpCodes.Jmp">OpCodes.Jmp</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,MethodInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist jmp(MethodInfo methodInfo)
        {
            _ilGenerator.Emit(OpCodes.Jmp, methodInfo); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarg"/>, short) that
        /// loads an argument (referenced by a specified index value) onto the stack.
        /// </summary>
        /// <param name="index">Index of the argument that is pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldarg">OpCodes.Ldarg</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldarg(short index)
        {
            _ilGenerator.Emit(OpCodes.Ldarg, index); return this;
        }
        /// <summary>
        /// Loads an argument onto the stack.
        /// </summary>
        /// <param name="parameterInfo">A <see cref="ParameterInfo"/> representing a parameter.</param>
        /// <param name="box">True, if parameter must be converted to a reference.</param>
        /// <seealso cref="ldarg(ParameterInfo)"/>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldargEx(ParameterInfo parameterInfo, bool box)
        {
            ldarg(parameterInfo);

            Type type = parameterInfo.ParameterType;

            if (parameterInfo.ParameterType.IsByRef)
                type = parameterInfo.ParameterType.GetElementType();

            if (parameterInfo.ParameterType.IsByRef)
            {
                if (type.IsValueType && type.IsPrimitive == false)
                    ldobj(type);
                else
                    ldind(type);
            }

            if (box)
                boxIfValueType(type);

            return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarg"/>, short) or 
        /// ILGenerator.Emit(<see cref="OpCodes.Ldarg_S"/>, byte) that
        /// loads an argument (referenced by a specified index value) onto the stack.
        /// </summary>
        /// <param name="index">Index of the argument that is pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldarg">OpCodes.Ldarg</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldarg(int index)
        {
            switch (index)
            {
                case 0: ldarg_0.end(); break;
                case 1: ldarg_1.end(); break;
                case 2: ldarg_2.end(); break;
                case 3: ldarg_3.end(); break;
                default:
                    if (index <= byte.MaxValue) ldarg_s((byte)index);
                    else if (index <= short.MaxValue) ldarg((short)index);
                    else
                        throw new ArgumentOutOfRangeException("index");

                    break;
            }

            return this;
        }
        /// <summary>
        /// Loads an argument onto the stack.
        /// </summary>
        /// <param name="parameterInfo">A <see cref="ParameterInfo"/> representing a parameter.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldarg(ParameterInfo parameterInfo)
        {
            if (parameterInfo == null) throw new ArgumentNullException("parameterInfo");

            bool isStatic =
                Method is MethodAssist && ((MethodAssist)Method).MethodBuilder.IsStatic;

            return ldarg(parameterInfo.Position + (isStatic ? 0 : 1));
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarga"/>, short) that
        /// load an argument address onto the evaluation stack.
        /// </summary>
        /// <param name="index">Index of the address addr of the argument that is pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldarga">OpCodes.Ldarga</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldarga(short index)
        {
            _ilGenerator.Emit(OpCodes.Ldarga, index); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarga_S"/>, byte) that
        /// load an argument address, in short form, onto the evaluation stack.
        /// </summary>
        /// <param name="index">Index of the address addr of the argument that is pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldarga_S">OpCodes.Ldarga_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,byte)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldarga_s(byte index)
        {
            _ilGenerator.Emit(OpCodes.Ldarga_S, index); return this;
        }
        /// <summary>
        /// Load an argument address onto the evaluation stack.
        /// </summary>
        /// <param name="index">Index of the address addr of the argument that is pushed onto the stack.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldarga(int index)
        {
            if (index <= byte.MaxValue) ldarga_s((byte)index);
            else if (index <= short.MaxValue) ldarga((short)index);
            else
                throw new ArgumentOutOfRangeException("index");

            return this;
        }
        /// <summary>
        /// Loads an argument address onto the stack.
        /// </summary>
        /// <param name="parameterInfo">A <see cref="ParameterInfo"/> representing a parameter.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldarga(ParameterInfo parameterInfo)
        {
            if (parameterInfo == null)
                throw new ArgumentNullException("parameterInfo");

            bool isStatic =
                Method is MethodAssist && ((MethodAssist)Method).MethodBuilder.IsStatic;

            return ldarga(parameterInfo.Position + (isStatic ? 0 : 1));
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarg_0"/>) that
        /// loads the argument at index 0 onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Ldarg_0">OpCodes.Ldarg_0</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldarg_0
        {
            get { _ilGenerator.Emit(OpCodes.Ldarg_0); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarg_1"/>) that
        /// loads the argument at index 1 onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Ldarg_1">OpCodes.Ldarg_1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldarg_1
        {
            get { _ilGenerator.Emit(OpCodes.Ldarg_1); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarg_2"/>) that
        /// loads the argument at index 2 onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Ldarg_2">OpCodes.Ldarg_2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldarg_2
        {
            get { _ilGenerator.Emit(OpCodes.Ldarg_2); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarg_3"/>) that
        /// loads the argument at index 3 onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Ldarg_3">OpCodes.Ldarg_3</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldarg_3
        {
            get { _ilGenerator.Emit(OpCodes.Ldarg_3); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarg_S"/>, byte) that
        /// loads the argument (referenced by a specified short form index) onto the evaluation stack.
        /// </summary>
        /// <param name="index">Index of the argument value that is pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldarg_S">OpCodes.Ldarg_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,byte)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldarg_s(byte index)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_S, index); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_0"/> or <see cref="OpCodes.Ldc_I4_1"/>) that
        /// pushes a supplied value of type int32 onto the evaluation stack as an int32.
        /// </summary>
        /// <param name="b">The value pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldc_I4">OpCodes.Ldc_I4_0</seealso>
        /// <seealso cref="OpCodes.Ldc_I4">OpCodes.Ldc_I4_1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,int)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_bool(bool b)
        {
            _ilGenerator.Emit(b ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_I4"/>, int) that
        /// pushes a supplied value of type int32 onto the evaluation stack as an int32.
        /// </summary>
        /// <param name="num">The value pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldc_I4">OpCodes.Ldc_I4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,int)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_i4(int num)
        {
            _ilGenerator.Emit(OpCodes.Ldc_I4, num); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_0"/>) that
        /// pushes the integer value of 0 onto the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldc_I4_0">OpCodes.Ldc_I4_0</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_i4_0
        {
            get { _ilGenerator.Emit(OpCodes.Ldc_I4_0); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_1"/>) that
        /// pushes the integer value of 1 onto the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldc_I4_1">OpCodes.Ldc_I4_1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_i4_1
        {
            get { _ilGenerator.Emit(OpCodes.Ldc_I4_1); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_2"/>) that
        /// pushes the integer value of 2 onto the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldc_I4_2">OpCodes.Ldc_I4_2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_i4_2
        {
            get { _ilGenerator.Emit(OpCodes.Ldc_I4_2); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_3"/>) that
        /// pushes the integer value of 3 onto the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldc_I4_3">OpCodes.Ldc_I4_3</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_i4_3
        {
            get { _ilGenerator.Emit(OpCodes.Ldc_I4_3); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_4"/>) that
        /// pushes the integer value of 4 onto the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldc_I4_4">OpCodes.Ldc_I4_4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_i4_4
        {
            get { _ilGenerator.Emit(OpCodes.Ldc_I4_4); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_5"/>) that
        /// pushes the integer value of 5 onto the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldc_I4_5">OpCodes.Ldc_I4_0</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_i4_5
        {
            get { _ilGenerator.Emit(OpCodes.Ldc_I4_5); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_6"/>) that
        /// pushes the integer value of 6 onto the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldc_I4_6">OpCodes.Ldc_I4_6</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_i4_6
        {
            get { _ilGenerator.Emit(OpCodes.Ldc_I4_6); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_7"/>) that
        /// pushes the integer value of 7 onto the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldc_I4_7">OpCodes.Ldc_I4_7</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_i4_7
        {
            get { _ilGenerator.Emit(OpCodes.Ldc_I4_7); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_8"/>) that
        /// pushes the integer value of 8 onto the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldc_I4_8">OpCodes.Ldc_I4_8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_i4_8
        {
            get { _ilGenerator.Emit(OpCodes.Ldc_I4_8); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_M1"/>) that
        /// pushes the integer value of -1 onto the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldc_I4_M1">OpCodes.Ldc_I4_M1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_i4_m1
        {
            get { _ilGenerator.Emit(OpCodes.Ldc_I4_M1); return this; }
        }
        /// <summary>
        /// Calls the best form of ILGenerator.Emit(Ldc_I4_X) that
        /// pushes the integer value of -1 onto the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="ldc_i4"/>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_i4_(int num)
        {
            switch (num)
            {
                case -1: ldc_i4_m1.end(); break;
                case 0: ldc_i4_0.end(); break;
                case 1: ldc_i4_1.end(); break;
                case 2: ldc_i4_2.end(); break;
                case 3: ldc_i4_3.end(); break;
                case 4: ldc_i4_4.end(); break;
                case 5: ldc_i4_5.end(); break;
                case 6: ldc_i4_6.end(); break;
                case 7: ldc_i4_7.end(); break;
                case 8: ldc_i4_8.end(); break;
                default:
                    if (num >= sbyte.MinValue && num <= sbyte.MaxValue)
                        ldc_i4_s((sbyte)num);
                    else
                        ldc_i4(num);

                    break;
            }

            return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_S"/>, byte) that
        /// pushes the supplied int8 value onto the evaluation stack as an int32, short form.
        /// </summary>
        /// <param name="num">The value pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldc_I4_S">OpCodes.Ldc_I4_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,byte)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        //[CLSCompliant(false)]
        public EmitAssist ldc_i4_s(sbyte num)
        {
            _ilGenerator.Emit(OpCodes.Ldc_I4_S, num); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_I8"/>, long) that
        /// pushes a supplied value of type int64 onto the evaluation stack as an int64.
        /// </summary>
        /// <param name="num">The value pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldc_I8">OpCodes.Ldc_I8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,long)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_i8(long num)
        {
            _ilGenerator.Emit(OpCodes.Ldc_I8, num); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_R4"/>, float) that
        /// pushes a supplied value of type float32 onto the evaluation stack as type F (float).
        /// </summary>
        /// <param name="num">The value pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldc_R4">OpCodes.Ldc_R4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,float)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_r4(float num)
        {
            _ilGenerator.Emit(OpCodes.Ldc_R4, num); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldc_R8"/>, double) that
        /// pushes a supplied value of type float64 onto the evaluation stack as type F (float).
        /// </summary>
        /// <param name="num">The value pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldc_R8">OpCodes.Ldc_R8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,double)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldc_r8(double num)
        {
            _ilGenerator.Emit(OpCodes.Ldc_R8, num); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldelema"/>, type) that
        /// loads the address of the array element at a specified array index onto the top of the evaluation stack 
        /// as type &amp; (managed pointer).
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Ldelema">OpCodes.Ldelema</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldelema(Type type)
        {
            _ilGenerator.Emit(OpCodes.Ldelema, type); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldelem_I"/>) that
        /// loads the element with type natural int at a specified array index onto the top of the evaluation stack 
        /// as a natural int.
        /// </summary>
        /// <seealso cref="OpCodes.Ldelem_I">OpCodes.Ldelem_I</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldelem_i
        {
            get { _ilGenerator.Emit(OpCodes.Ldelem_I); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldelem_I1"/>) that
        /// loads the element with type int8 at a specified array index onto the top of the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldelem_I1">OpCodes.Ldelem_I1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldelem_i1
        {
            get { _ilGenerator.Emit(OpCodes.Ldelem_I1); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldelem_I2"/>) that
        /// loads the element with type int16 at a specified array index onto the top of the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldelem_I2">OpCodes.Ldelem_I2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldelem_i2
        {
            get { _ilGenerator.Emit(OpCodes.Ldelem_I2); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldelem_I4"/>) that
        /// loads the element with type int32 at a specified array index onto the top of the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldelem_I4">OpCodes.Ldelem_I4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldelem_i4
        {
            get { _ilGenerator.Emit(OpCodes.Ldelem_I4); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldelem_I8"/>) that
        /// loads the element with type int64 at a specified array index onto the top of the evaluation stack as an int64.
        /// </summary>
        /// <seealso cref="OpCodes.Ldelem_I8">OpCodes.Ldelem_I8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldelem_i8
        {
            get { _ilGenerator.Emit(OpCodes.Ldelem_I8); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldelem_R4"/>) that
        /// loads the element with type float32 at a specified array index onto the top of the evaluation stack as type F (float).
        /// </summary>
        /// <seealso cref="OpCodes.Ldelem_R4">OpCodes.Ldelem_R4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldelem_r4
        {
            get { _ilGenerator.Emit(OpCodes.Ldelem_R4); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldelem_R8"/>) that
        /// loads the element with type float64 at a specified array index onto the top of the evaluation stack as type F (float).
        /// </summary>
        /// <seealso cref="OpCodes.Ldelem_R8">OpCodes.Ldelem_R8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldelem_r8
        {
            get { _ilGenerator.Emit(OpCodes.Ldelem_R8); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldelem_Ref"/>) that
        /// loads the element containing an object reference at a specified array index 
        /// onto the top of the evaluation stack as type O (object reference).
        /// </summary>
        /// <seealso cref="OpCodes.Ldelem_Ref">OpCodes.Ldelem_Ref</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldelem_ref
        {
            get { _ilGenerator.Emit(OpCodes.Ldelem_Ref); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldelem_U1"/>) that
        /// loads the element with type unsigned int8 at a specified array index onto the top of the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldelem_U1">OpCodes.Ldelem_U1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldelem_u1
        {
            get { _ilGenerator.Emit(OpCodes.Ldelem_U1); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldelem_U2"/>) that
        /// loads the element with type unsigned int16 at a specified array index 
        /// onto the top of the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldelem_U2">OpCodes.Ldelem_U2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldelem_u2
        {
            get { _ilGenerator.Emit(OpCodes.Ldelem_U2); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldelem_U4"/>) that
        /// loads the element with type unsigned int32 at a specified array index 
        /// onto the top of the evaluation stack as an int32.
        /// </summary>
        /// <seealso cref="OpCodes.Ldelem_U4">OpCodes.Ldelem_U4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldelem_u4
        {
            get { _ilGenerator.Emit(OpCodes.Ldelem_U4); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldfld"/>, fieldInfo) that
        /// finds the value of a field in the object whose reference is currently on the evaluation stack.
        /// </summary>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <seealso cref="OpCodes.Ldfld">OpCodes.Ldfld</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldfld(FieldInfo fieldInfo)
        {
            _ilGenerator.Emit(OpCodes.Ldfld, fieldInfo); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldflda"/>, fieldInfo) that
        /// finds the address of a field in the object whose reference is currently on the evaluation stack.
        /// </summary>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <seealso cref="OpCodes.Ldflda">OpCodes.Ldflda</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldflda(FieldInfo fieldInfo)
        {
            _ilGenerator.Emit(OpCodes.Ldflda, fieldInfo); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldftn"/>, methodInfo) that
        /// pushes an unmanaged pointer (type natural int) to the native code implementing a specific method 
        /// onto the evaluation stack.
        /// </summary>
        /// <param name="methodInfo">The method to be called.</param>
        /// <seealso cref="OpCodes.Ldftn">OpCodes.Ldftn</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,MethodInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldftn(MethodInfo methodInfo)
        {
            _ilGenerator.Emit(OpCodes.Ldftn, methodInfo); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldind_I"/>) that
        /// loads a value of type natural int as a natural int onto the evaluation stack indirectly.
        /// </summary>
        /// <seealso cref="OpCodes.Ldind_I">OpCodes.Ldind_I</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldind_i
        {
            get { _ilGenerator.Emit(OpCodes.Ldind_I); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldind_I1"/>) that
        /// loads a value of type int8 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        /// <seealso cref="OpCodes.Ldind_I1">OpCodes.Ldind_I1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldind_i1
        {
            get { _ilGenerator.Emit(OpCodes.Ldind_I1); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldind_I2"/>) that
        /// loads a value of type int16 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        /// <seealso cref="OpCodes.Ldind_I2">OpCodes.Ldind_I2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldind_i2
        {
            get { _ilGenerator.Emit(OpCodes.Ldind_I2); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldind_I4"/>) that
        /// loads a value of type int32 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        /// <seealso cref="OpCodes.Ldind_I4">OpCodes.Ldind_I4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldind_i4
        {
            get { _ilGenerator.Emit(OpCodes.Ldind_I4); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldind_I8"/>) that
        /// loads a value of type int64 as an int64 onto the evaluation stack indirectly.
        /// </summary>
        /// <seealso cref="OpCodes.Ldind_I8">OpCodes.Ldind_I8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldind_i8
        {
            get { _ilGenerator.Emit(OpCodes.Ldind_I8); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldind_R4"/>) that
        /// loads a value of type float32 as a type F (float) onto the evaluation stack indirectly.
        /// </summary>
        /// <seealso cref="OpCodes.Ldind_R4">OpCodes.Ldind_R4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldind_r4
        {
            get { _ilGenerator.Emit(OpCodes.Ldind_R4); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldind_R8"/>) that
        /// loads a value of type float64 as a type F (float) onto the evaluation stack indirectly.
        /// </summary>
        /// <seealso cref="OpCodes.Ldind_R8">OpCodes.Ldind_R8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldind_r8
        {
            get { _ilGenerator.Emit(OpCodes.Ldind_R8); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldind_Ref"/>) that
        /// loads an object reference as a type O (object reference) onto the evaluation stack indirectly.
        /// </summary>
        /// <seealso cref="OpCodes.Ldind_Ref">OpCodes.Ldind_Ref</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldind_ref
        {
            get { _ilGenerator.Emit(OpCodes.Ldind_Ref); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldind_U1"/>) that
        /// loads a value of type unsigned int8 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        /// <seealso cref="OpCodes.Ldind_U1">OpCodes.Ldind_U1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldind_u1
        {
            get { _ilGenerator.Emit(OpCodes.Ldind_U1); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldind_U2"/>) that
        /// loads a value of type unsigned int16 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        /// <seealso cref="OpCodes.Ldind_U2">OpCodes.Ldind_U2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldind_u2
        {
            get { _ilGenerator.Emit(OpCodes.Ldind_U2); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldind_U4"/>) that
        /// loads a value of type unsigned int32 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        /// <seealso cref="OpCodes.Ldind_U4">OpCodes.Ldind_U4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldind_u4
        {
            get { _ilGenerator.Emit(OpCodes.Ldind_U4); return this; }
        }
        /// <summary>
        /// Loads a value of the type from a supplied address.
        /// </summary>
        /// <param name="type">A Type.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldind(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.SByte: ldind_i1.end(); break;

                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16: ldind_i2.end(); break;

                case TypeCode.Int32:
                case TypeCode.UInt32: ldind_i4.end(); break;

                case TypeCode.Int64:
                case TypeCode.UInt64: ldind_i8.end(); break;

                case TypeCode.Single: ldind_r4.end(); break;
                case TypeCode.Double: ldind_r8.end(); break;

                default:
                    if (type.IsClass)
                        ldind_ref.end();
                    else if (type.IsValueType)
                        stobj(type);
                    else
                        throw CreateNotExpectedTypeException(type);
                    break;
            }

            return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldlen"/>) that
        /// pushes the number of elements of a zero-based, one-dimensional array onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Ldlen">OpCodes.Ldlen</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldlen
        {
            get { _ilGenerator.Emit(OpCodes.Ldlen); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldloc"/>, short) that
        /// load an argument address onto the evaluation stack.
        /// </summary>
        /// <param name="index">Index of the local variable value pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldloc">OpCodes.Ldloc</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldloc(short index)
        {
            _ilGenerator.Emit(OpCodes.Ldloc, index); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldloc"/>, <see cref="LocalBuilder"/>) that
        /// load an argument address onto the evaluation stack.
        /// </summary>
        /// <param name="localBuilder">Local variable builder.</param>
        /// <seealso cref="OpCodes.Ldloc">OpCodes.Ldloc</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldloc(LocalBuilder localBuilder)
        {
            _ilGenerator.Emit(OpCodes.Ldloc, localBuilder); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldloca"/>, short) that
        /// loads the address of the local variable at a specific index onto the evaluation stack.
        /// </summary>
        /// <param name="index">Index of the local variable.</param>
        /// <seealso cref="OpCodes.Ldloca">OpCodes.Ldloca</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldloca(short index)
        {
            _ilGenerator.Emit(OpCodes.Ldloca, index); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldloca_S"/>, byte) that
        /// loads the address of the local variable at a specific index onto the evaluation stack, short form.
        /// </summary>
        /// <param name="index">Index of the local variable.</param>
        /// <seealso cref="OpCodes.Ldloca_S">OpCodes.Ldloca_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,byte)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldloca_s(byte index)
        {
            _ilGenerator.Emit(OpCodes.Ldloca_S, index); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldloca"/>, <see cref="LocalBuilder"/>) that
        /// loads the address of the local variable at a specific index onto the evaluation stack.
        /// </summary>
        /// <param name="local">A <see cref="LocalBuilder"/> representing the local variable.</param>
        /// <seealso cref="OpCodes.Ldloca">OpCodes.Ldloca</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldloca(LocalBuilder local)
        {
            _ilGenerator.Emit(OpCodes.Ldloca, local); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldloc_0"/>) that
        /// loads the local variable at index 0 onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Ldloc_0">OpCodes.Ldloc_0</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldloc_0
        {
            get { _ilGenerator.Emit(OpCodes.Ldloc_0); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldloc_1"/>) that
        /// loads the local variable at index 1 onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Ldloc_1">OpCodes.Ldloc_1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldloc_1
        {
            get { _ilGenerator.Emit(OpCodes.Ldloc_1); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldloc_2"/>) that
        /// loads the local variable at index 2 onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Ldloc_2">OpCodes.Ldloc_2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldloc_2
        {
            get { _ilGenerator.Emit(OpCodes.Ldloc_2); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldloc_3"/>) that
        /// loads the local variable at index 3 onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Ldloc_3">OpCodes.Ldloc_3</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldloc_3
        {
            get { _ilGenerator.Emit(OpCodes.Ldloc_3); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldloc_S"/>, byte) that
        /// loads the local variable at a specific index onto the evaluation stack, short form.
        /// </summary>
        /// <param name="index">Index of the local variable.</param>
        /// <seealso cref="OpCodes.Ldloc_S">OpCodes.Ldloc_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,byte)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldloc_s(byte index)
        {
            _ilGenerator.Emit(OpCodes.Ldloca_S, index); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldnull"/>) that
        /// pushes a null reference (type O) onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Ldnull">OpCodes.Ldnull</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldnull
        {
            get { _ilGenerator.Emit(OpCodes.Ldnull); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldobj"/>, type) that
        /// copies the value type object pointed to by an address to the top of the evaluation stack.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Ldobj">OpCodes.Ldobj</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldobj(Type type)
        {
            _ilGenerator.Emit(OpCodes.Ldobj, type); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldsfld"/>, fieldInfo) that
        /// pushes the value of a static field onto the evaluation stack.
        /// </summary>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <seealso cref="OpCodes.Ldsfld">OpCodes.Ldsfld</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldsfld(FieldInfo fieldInfo)
        {
            _ilGenerator.Emit(OpCodes.Ldsfld, fieldInfo); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldsflda"/>, fieldInfo) that
        /// pushes the address of a static field onto the evaluation stack.
        /// </summary>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <seealso cref="OpCodes.Ldsflda">OpCodes.Ldsflda</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldsflda(FieldInfo fieldInfo)
        {
            _ilGenerator.Emit(OpCodes.Ldsflda, fieldInfo); return this;
        }
        /// <summary>
        /// Calls <see cref="ldstr"/> -or- <see cref="ldnull"/>,
        /// if given string is a null reference.
        /// </summary>
        /// <param name="str">The String to be emitted.</param>
        /// <seealso cref="ldstr"/>
        /// <seealso cref="ldnull"/>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldstrEx(string str)
        {
            return str == null ? ldnull : ldstr(str);
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldstr"/>, string) that
        /// pushes a new object reference to a string literal stored in the metadata.
        /// </summary>
        /// <param name="str">The String to be emitted.</param>
        /// <seealso cref="OpCodes.Ldstr">OpCodes.Ldstr</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldstr(string str)
        {
            _ilGenerator.Emit(OpCodes.Ldstr, str); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldstr"/>, string) that
        /// pushes a new object reference to a string literal stored in the metadata.
        /// </summary>
        /// <param name="nameOrIndex">The <see cref="NameOrIndexParameter"/> to be emitted.</param>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldNameOrIndex(NameOrIndexParameter nameOrIndex)
        {
            return nameOrIndex.ByName ?
                ldstr(nameOrIndex.Name).call(typeof(NameOrIndexParameter), "op_Implicit", typeof(string)) :
                ldc_i4_(nameOrIndex.Index).call(typeof(NameOrIndexParameter), "op_Implicit", typeof(int));
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldtoken"/>, methodInfo) that
        /// converts a metadata token to its runtime representation, pushing it onto the evaluation stack.
        /// </summary>
        /// <param name="methodInfo">The method to be called.</param>
        /// <seealso cref="OpCodes.Ldtoken">OpCodes.Ldtoken</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,MethodInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldtoken(MethodInfo methodInfo)
        {
            _ilGenerator.Emit(OpCodes.Ldtoken, methodInfo); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldtoken"/>, fieldInfo) that
        /// converts a metadata token to its runtime representation, 
        /// pushing it onto the evaluation stack.
        /// </summary>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <seealso cref="OpCodes.Ldtoken">OpCodes.Ldtoken</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldtoken(FieldInfo fieldInfo)
        {
            _ilGenerator.Emit(OpCodes.Ldtoken, fieldInfo); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldtoken"/>, type) that
        /// converts a metadata token to its runtime representation, pushing it onto the evaluation stack.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Ldtoken">OpCodes.Ldtoken</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldtoken(Type type)
        {
            _ilGenerator.Emit(OpCodes.Ldtoken, type); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldvirtftn"/>, methodInfo) that
        /// pushes an unmanaged pointer (type natural int) to the native code implementing a particular virtual method 
        /// associated with a specified object onto the evaluation stack.
        /// </summary>
        /// <param name="methodInfo">The method to be called.</param>
        /// <seealso cref="OpCodes.Ldvirtftn">OpCodes.Ldvirtftn</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,MethodInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ldvirtftn(MethodInfo methodInfo)
        {
            _ilGenerator.Emit(OpCodes.Ldvirtftn, methodInfo); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Leave"/>, label) that
        /// exits a protected region of code, unconditionally tranferring control to a specific target instruction.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <seealso cref="OpCodes.Leave">OpCodes.Leave</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist leave(Label label)
        {
            _ilGenerator.Emit(OpCodes.Leave, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Leave_S"/>, label) that
        /// exits a protected region of code, unconditionally transferring control to a target instruction (short form).
        /// </summary>
        /// <param name="label">The label.</param>
        /// <seealso cref="OpCodes.Leave_S">OpCodes.Leave_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist leave_s(Label label)
        {
            _ilGenerator.Emit(OpCodes.Leave_S, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Localloc"/>) that
        /// allocates a certain number of bytes from the local dynamic memory pool and pushes the address 
        /// (a transient pointer, type *) of the first allocated byte onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Localloc">OpCodes.Localloc</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist localloc
        {
            get { _ilGenerator.Emit(OpCodes.Localloc); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Mkrefany"/>, type) that
        /// pushes a typed reference to an instance of a specific type onto the evaluation stack.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Mkrefany">OpCodes.Mkrefany</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist mkrefany(Type type)
        {
            _ilGenerator.Emit(OpCodes.Mkrefany, type); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Mul"/>) that
        /// multiplies two values and pushes the result on the evaluation stack.
        /// (a transient pointer, type *) of the first allocated byte onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Mul">OpCodes.Mul</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist mul
        {
            get { _ilGenerator.Emit(OpCodes.Mul); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Mul_Ovf"/>) that
        /// multiplies two integer values, performs an overflow check, 
        /// and pushes the result onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Mul_Ovf">OpCodes.Mul_Ovf</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist mul_ovf
        {
            get { _ilGenerator.Emit(OpCodes.Mul_Ovf); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Mul_Ovf_Un"/>) that
        /// multiplies two unsigned integer values, performs an overflow check, 
        /// and pushes the result onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Mul_Ovf_Un">OpCodes.Mul_Ovf_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist mul_ovf_un
        {
            get { _ilGenerator.Emit(OpCodes.Mul_Ovf_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Neg"/>) that
        /// negates a value and pushes the result onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Neg">OpCodes.Neg</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist neg
        {
            get { _ilGenerator.Emit(OpCodes.Neg); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Newarr"/>, type) that
        /// pushes an object reference to a new zero-based, one-dimensional array whose elements 
        /// are of a specific type onto the evaluation stack.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Newarr">OpCodes.Newarr</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist newarr(Type type)
        {
            _ilGenerator.Emit(OpCodes.Newarr, type); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Newobj"/>, <see cref="ConstructorInfo"/>) that
        /// creates a new object or a new instance of a value type,
        /// pushing an object reference (type O) onto the evaluation stack.
        /// </summary>
        /// <param name="constructorInfo">A <see cref="ConstructorInfo"/> representing a constructor.</param>
        /// <seealso cref="OpCodes.Newobj">OpCodes.Newobj</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,ConstructorInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist newobj(ConstructorInfo constructorInfo)
        {
            _ilGenerator.Emit(OpCodes.Newobj, constructorInfo); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Newobj"/>, ConstructorInfo) that
        /// creates a new object or a new instance of a value type,
        /// pushing an object reference (type O) onto the evaluation stack.
        /// </summary>
        /// <param name="type">A type.</param>
        /// <param name="parameters">An array of System.Type objects representing
        /// the number, order, and type of the parameters for the desired constructor.
        /// -or- An empty array of System.Type objects, to get a constructor that takes
        /// no parameters. Such an empty array is provided by the static field System.Type.EmptyTypes.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist newobj(Type type, params Type[] parameters)
        {
            if (type == null) throw new ArgumentNullException("type");

            ConstructorInfo ci = type.GetConstructor(parameters);

            return newobj(ci);
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Nop"/>) that
        /// fills space if opcodes are patched. No meaningful operation is performed although 
        /// a processing cycle can be consumed.
        /// </summary>
        /// <seealso cref="OpCodes.Nop">OpCodes.Nop</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist nop
        {
            get { _ilGenerator.Emit(OpCodes.Nop); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Not"/>) that
        /// computes the bitwise complement of the integer value on top of the stack 
        /// and pushes the result onto the evaluation stack as the same type.
        /// </summary>
        /// <seealso cref="OpCodes.Not">OpCodes.Not</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist not
        {
            get { _ilGenerator.Emit(OpCodes.Not); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Or"/>) that
        /// compute the bitwise complement of the two integer values on top of the stack and 
        /// pushes the result onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Or">OpCodes.Or</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist or
        {
            get { _ilGenerator.Emit(OpCodes.Or); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Pop"/>) that
        /// removes the value currently on top of the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Pop">OpCodes.Pop</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist pop
        {
            get { _ilGenerator.Emit(OpCodes.Pop); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Refanytype"/>) that
        /// specifies that the subsequent array address operation performs
        /// no type check at run time, and that it returns a managed pointer
        /// whose mutability is restricted.
        /// </summary>
        /// <seealso cref="OpCodes.Refanytype">OpCodes.Refanytype</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist @readonly
        {
            get { _ilGenerator.Emit(OpCodes.Readonly); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Refanytype"/>) that
        /// retrieves the type token embedded in a typed reference.
        /// </summary>
        /// <seealso cref="OpCodes.Refanytype">OpCodes.Refanytype</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public EmitAssist refanytype
        {
            get { _ilGenerator.Emit(OpCodes.Refanytype); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Refanyval"/>, type) that
        /// retrieves the address (type &amp;) embedded in a typed reference.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Refanyval">OpCodes.Refanyval</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist refanyval(Type type)
        {
            _ilGenerator.Emit(OpCodes.Refanyval, type); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Rem"/>) that
        /// divides two values and pushes the remainder onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Rem">OpCodes.Rem</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist rem
        {
            get { _ilGenerator.Emit(OpCodes.Rem); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Rem_Un"/>) that
        /// divides two unsigned values and pushes the remainder onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Rem_Un">OpCodes.Rem_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist rem_un
        {
            get { _ilGenerator.Emit(OpCodes.Rem_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ret"/>) that
        /// returns from the current method, pushing a return value (if present) 
        /// from the caller's evaluation stack onto the callee's evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Ret">OpCodes.Ret</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist ret()
        {
            _ilGenerator.Emit(OpCodes.Ret); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Rethrow"/>) that
        /// rethrows the current exception.
        /// </summary>
        /// <seealso cref="OpCodes.Rethrow">OpCodes.Rethrow</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist rethrow
        {
            get { _ilGenerator.Emit(OpCodes.Rethrow); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Shl"/>) that
        /// shifts an integer value to the left (in zeroes) by a specified number of bits,
        /// pushing the result onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Shl">OpCodes.Shl</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist shl
        {
            get { _ilGenerator.Emit(OpCodes.Shl); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Shr"/>) that
        /// shifts an integer value (in sign) to the right by a specified number of bits,
        /// pushing the result onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Shr">OpCodes.Shr</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist shr
        {
            get { _ilGenerator.Emit(OpCodes.Shr); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Shr_Un"/>) that
        /// shifts an unsigned integer value (in zeroes) to the right by a specified number of bits,
        /// pushing the result onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Shr_Un">OpCodes.Shr_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist shr_un
        {
            get { _ilGenerator.Emit(OpCodes.Shr_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Sizeof"/>, type) that
        /// pushes the size, in bytes, of a supplied value type onto the evaluation stack.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Sizeof">OpCodes.Sizeof</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist @sizeof(Type type)
        {
            _ilGenerator.Emit(OpCodes.Sizeof, type); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Starg"/>, short) that
        /// stores the value on top of the evaluation stack in the argument slot at a specified index.
        /// </summary>
        /// <param name="index">Slot index.</param>
        /// <seealso cref="OpCodes.Starg">OpCodes.Starg</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist starg(short index)
        {
            _ilGenerator.Emit(OpCodes.Starg, index); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Starg_S"/>, byte) that
        /// stores the value on top of the evaluation stack in the argument slot at a specified index,
        /// short form.
        /// </summary>
        /// <param name="index">Slot index.</param>
        /// <seealso cref="OpCodes.Starg_S">OpCodes.Starg_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,byte)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist starg_s(byte index)
        {
            _ilGenerator.Emit(OpCodes.Starg_S, index); return this;
        }
        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at a specified index.
        /// </summary>
        /// <param name="index">Slot index.</param>
        /// <seealso cref="OpCodes.Starg">OpCodes.Starg</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist starg(int index)
        {
            if (index < byte.MaxValue) starg_s((byte)index);
            else if (index < short.MaxValue) starg((short)index);
            else
                throw new ArgumentOutOfRangeException("index");

            return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stelem_I"/>) that
        /// replaces the array element at a given index with the natural int value 
        /// on the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Stelem_I">OpCodes.Stelem_I</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stelem_i
        {
            get { _ilGenerator.Emit(OpCodes.Stelem_I); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stelem_I1"/>) that
        /// replaces the array element at a given index with the int8 value on the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Stelem_I1">OpCodes.Stelem_I1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stelem_i1
        {
            get { _ilGenerator.Emit(OpCodes.Stelem_I1); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stelem_I2"/>) that
        /// replaces the array element at a given index with the int16 value on the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Stelem_I2">OpCodes.Stelem_I2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stelem_i2
        {
            get { _ilGenerator.Emit(OpCodes.Stelem_I2); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stelem_I4"/>) that
        /// replaces the array element at a given index with the int32 value on the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Stelem_I4">OpCodes.Stelem_I4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public EmitAssist stelem_i4
        {
            get { _ilGenerator.Emit(OpCodes.Stelem_I4); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stelem_I8"/>) that
        /// replaces the array element at a given index with the int64 value on the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Stelem_I8">OpCodes.Stelem_I8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stelem_i8
        {
            get { _ilGenerator.Emit(OpCodes.Stelem_I8); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stelem_R4"/>) that
        /// replaces the array element at a given index with the float32 value on the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Stelem_R4">OpCodes.Stelem_R4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stelem_r4
        {
            get { _ilGenerator.Emit(OpCodes.Stelem_R4); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stelem_R8"/>) that
        /// replaces the array element at a given index with the float64 value on the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Stelem_R8">OpCodes.Stelem_R8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stelem_r8
        {
            get { _ilGenerator.Emit(OpCodes.Stelem_R8); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stelem_Ref"/>) that
        /// replaces the array element at a given index with the object ref value (type O)
        /// on the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Stelem_Ref">OpCodes.Stelem_Ref</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stelem_ref
        {
            get { _ilGenerator.Emit(OpCodes.Stelem_Ref); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stfld"/>, <see cref="FieldInfo"/>) that
        /// replaces the value stored in the field of an object reference or pointer with a new value.
        /// </summary>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <seealso cref="OpCodes.Stfld">OpCodes.Stfld</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stfld(FieldInfo fieldInfo)
        {
            _ilGenerator.Emit(OpCodes.Stfld, fieldInfo); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stind_I"/>) that
        /// stores a value of type natural int at a supplied address.
        /// </summary>
        /// <seealso cref="OpCodes.Stind_I">OpCodes.Stind_I</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stind_i
        {
            get { _ilGenerator.Emit(OpCodes.Stind_I); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stind_I1"/>) that
        /// stores a value of type int8 at a supplied address.
        /// </summary>
        /// <seealso cref="OpCodes.Stind_I1">OpCodes.Stind_I1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stind_i1
        {
            get { _ilGenerator.Emit(OpCodes.Stind_I1); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stind_I2"/>) that
        /// stores a value of type int16 at a supplied address.
        /// </summary>
        /// <seealso cref="OpCodes.Stind_I2">OpCodes.Stind_I2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stind_i2
        {
            get { _ilGenerator.Emit(OpCodes.Stind_I2); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stind_I4"/>) that
        /// stores a value of type int32 at a supplied address.
        /// </summary>
        /// <seealso cref="OpCodes.Stind_I4">OpCodes.Stind_I4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stind_i4
        {
            get { _ilGenerator.Emit(OpCodes.Stind_I4); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stind_I8"/>) that
        /// stores a value of type int64 at a supplied address.
        /// </summary>
        /// <seealso cref="OpCodes.Stind_I8">OpCodes.Stind_I8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stind_i8
        {
            get { _ilGenerator.Emit(OpCodes.Stind_I8); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stind_R4"/>) that
        /// stores a value of type float32 at a supplied address.
        /// </summary>
        /// <seealso cref="OpCodes.Stind_R4">OpCodes.Stind_R4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stind_r4
        {
            get { _ilGenerator.Emit(OpCodes.Stind_R4); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stind_R8"/>) that
        /// stores a value of type float64 at a supplied address.
        /// </summary>
        /// <seealso cref="OpCodes.Stind_R8">OpCodes.Stind_R8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stind_r8
        {
            get { _ilGenerator.Emit(OpCodes.Stind_R8); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stind_Ref"/>) that
        /// stores an object reference value at a supplied address.
        /// </summary>
        /// <seealso cref="OpCodes.Stind_Ref">OpCodes.Stind_Ref</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stind_ref
        {
            get { _ilGenerator.Emit(OpCodes.Stind_Ref); return this; }
        }
        /// <summary>
        /// Stores a value of the type at a supplied address.
        /// </summary>
        /// <param name="type">A Type.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stind(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.SByte: stind_i1.end(); break;

                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16: stind_i2.end(); break;

                case TypeCode.Int32:
                case TypeCode.UInt32: stind_i4.end(); break;

                case TypeCode.Int64:
                case TypeCode.UInt64: stind_i8.end(); break;

                case TypeCode.Single: stind_r4.end(); break;
                case TypeCode.Double: stind_r8.end(); break;

                default:
                    if (type.IsClass)
                        stind_ref.end();
                    else if (type.IsValueType)
                        stobj(type);
                    else
                        throw CreateNotExpectedTypeException(type);
                    break;
            }

            return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stloc"/>, <see cref="LocalBuilder"/>) that
        /// pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at a specified index.
        /// </summary>
        /// <param name="local">A local variable.</param>
        /// <seealso cref="OpCodes.Stloc">OpCodes.Stloc</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,LocalBuilder)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stloc(LocalBuilder local)
        {
            _ilGenerator.Emit(OpCodes.Stloc, local); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stloc"/>, short) that
        /// pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at a specified index.
        /// </summary>
        /// <param name="index">A local variable index.</param>
        /// <seealso cref="OpCodes.Stloc">OpCodes.Stloc</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stloc(short index)
        {
            if (index >= byte.MinValue && index <= byte.MaxValue)
                return stloc_s((byte)index);

            _ilGenerator.Emit(OpCodes.Stloc, index);
            return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stloc_0"/>) that
        /// pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at index 0.
        /// </summary>
        /// <seealso cref="OpCodes.Stloc_0">OpCodes.Stloc_0</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stloc_0
        {
            get { _ilGenerator.Emit(OpCodes.Stloc_0); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stloc_1"/>) that
        /// pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at index 1.
        /// </summary>
        /// <seealso cref="OpCodes.Stloc_1">OpCodes.Stloc_1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stloc_1
        {
            get { _ilGenerator.Emit(OpCodes.Stloc_1); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stloc_2"/>) that
        /// pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at index 2.
        /// </summary>
        /// <seealso cref="OpCodes.Stloc_2">OpCodes.Stloc_2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stloc_2
        {
            get { _ilGenerator.Emit(OpCodes.Stloc_2); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stloc_3"/>) that
        /// pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at index 3.
        /// </summary>
        /// <seealso cref="OpCodes.Stloc_3">OpCodes.Stloc_3</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stloc_3
        {
            get { _ilGenerator.Emit(OpCodes.Stloc_3); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stloc_S"/>, <see cref="LocalBuilder"/>) that
        /// pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at index (short form).
        /// </summary>
        /// <param name="local">A local variable.</param>
        /// <seealso cref="OpCodes.Stloc_S">OpCodes.Stloc_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,LocalBuilder)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stloc_s(LocalBuilder local)
        {
            _ilGenerator.Emit(OpCodes.Stloc_S, local); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stloc_S"/>, byte) that
        /// pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at index (short form).
        /// </summary>
        /// <param name="index">A local variable index.</param>
        /// <seealso cref="OpCodes.Stloc_S">OpCodes.Stloc_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stloc_s(byte index)
        {
            switch (index)
            {
                case 0: stloc_0.end(); break;
                case 1: stloc_1.end(); break;
                case 2: stloc_2.end(); break;
                case 3: stloc_3.end(); break;

                default:
                    _ilGenerator.Emit(OpCodes.Stloc_S, index); break;
            }

            return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stobj"/>, type) that
        /// copies a value of a specified type from the evaluation stack into a supplied memory address.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Stobj">OpCodes.Stobj</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stobj(Type type)
        {
            _ilGenerator.Emit(OpCodes.Stobj, type); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Stsfld"/>, fieldInfo) that
        /// replaces the value of a static field with a value from the evaluation stack.
        /// </summary>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <seealso cref="OpCodes.Stsfld">OpCodes.Stsfld</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist stsfld(FieldInfo fieldInfo)
        {
            _ilGenerator.Emit(OpCodes.Stsfld, fieldInfo); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Sub"/>) that
        /// subtracts one value from another and pushes the result onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Sub">OpCodes.Sub</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist sub
        {
            get { _ilGenerator.Emit(OpCodes.Sub); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Sub_Ovf"/>) that
        /// subtracts one integer value from another, performs an overflow check,
        /// and pushes the result onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Sub_Ovf">OpCodes.Sub_Ovf</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist sub_ovf
        {
            get { _ilGenerator.Emit(OpCodes.Sub_Ovf); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Sub_Ovf_Un"/>) that
        /// subtracts one unsigned integer value from another, performs an overflow check,
        /// and pushes the result onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Sub_Ovf_Un">OpCodes.Sub_Ovf_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist sub_ovf_un
        {
            get { _ilGenerator.Emit(OpCodes.Sub_Ovf_Un); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Switch"/>, label[]) that
        /// implements a jump table.
        /// </summary>
        /// <param name="labels">The array of label objects to which to branch from this location.</param>
        /// <seealso cref="OpCodes.Switch">OpCodes.Switch</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label[])">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist @switch(Label[] labels)
        {
            _ilGenerator.Emit(OpCodes.Switch, labels); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Tailcall"/>) that
        /// performs a postfixed method call instruction such that the current method's stack frame 
        /// is removed before the actual call instruction is executed.
        /// </summary>
        /// <seealso cref="OpCodes.Tailcall">OpCodes.Tailcall</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist tailcall
        {
            get { _ilGenerator.Emit(OpCodes.Tailcall); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Throw"/>) that
        /// throws the exception object currently on the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Throw">OpCodes.Throw</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist @throw
        {
            get { _ilGenerator.Emit(OpCodes.Throw); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Unaligned"/>, label) that
        /// indicates that an address currently atop the evaluation stack might not be aligned 
        /// to the natural size of the immediately following ldind, stind, ldfld, stfld, ldobj, stobj, 
        /// initblk, or cpblk instruction.
        /// </summary>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Unaligned">OpCodes.Unaligned</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist unaligned(Label label)
        {
            _ilGenerator.Emit(OpCodes.Unaligned, label); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Unaligned"/>, long) that
        /// indicates that an address currently atop the evaluation stack might not be aligned 
        /// to the natural size of the immediately following ldind, stind, ldfld, stfld, ldobj, stobj, 
        /// initblk, or cpblk instruction.
        /// </summary>
        /// <param name="addr">An address is pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Unaligned">OpCodes.Unaligned</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,long)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist unaligned(long addr)
        {
            _ilGenerator.Emit(OpCodes.Unaligned, addr); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Unbox"/>, type) that
        /// converts the boxed representation of a value type to its unboxed form.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Unbox">OpCodes.Unbox</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist unbox(Type type)
        {
            _ilGenerator.Emit(OpCodes.Unbox, type); return this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Unbox_Any"/>, type) that
        /// converts the boxed representation of a value type to its unboxed form.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Unbox_Any">OpCodes.Unbox_Any</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist unbox_any(Type type)
        {
            _ilGenerator.Emit(OpCodes.Unbox_Any, type);
            return this;
        }
        /// <summary>
        /// Calls <see cref="unbox_any(Type)"/> if given type is a value type.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Unbox_Any">OpCodes.Unbox</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist unboxIfValueType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return type.IsValueType ? unbox_any(type) : this;
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Volatile"/>) that
        /// specifies that an address currently atop the evaluation stack might be volatile, 
        /// and the results of reading that location cannot be cached or that multiple stores 
        /// to that location cannot be suppressed.
        /// </summary>
        /// <seealso cref="OpCodes.Volatile">OpCodes.Volatile</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist @volatile
        {
            get { _ilGenerator.Emit(OpCodes.Volatile); return this; }
        }
        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Xor"/>) that
        /// computes the bitwise XOR of the top two values on the evaluation stack, 
        /// pushing the result onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Xor">OpCodes.Xor</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist xor
        {
            get { _ilGenerator.Emit(OpCodes.Xor); return this; }
        }
        /// <summary>
        /// Ends sequence of property calls.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void end()
        {
        }

        #endregion

        #region Load/Init/Cast/AddMaxStackSize

        /// <summary>
        /// Loads default value of given type onto the evaluation stack.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist LoadInitValue(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                    ldc_i4_0.end();
                    break;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    ldc_i4_0.conv_i8.end();
                    break;

                case TypeCode.Single:
                case TypeCode.Double:
                    ldc_r4(0).end();
                    break;

                case TypeCode.String:
                    ldsfld(typeof(string).GetField("Empty"));
                    break;

                default:
                    if (type.IsClass || type.IsInterface)
                        ldnull.end();
                    else
                        throw CreateNotExpectedTypeException(type);
                    break;
            }

            return this;
        }
        /// <summary>
        /// Loads supplied object value (if possible) onto the evaluation stack.
        /// </summary>
        /// <param name="o">Any object instance or null reference.</param>
        /// <returns>True is a value was loaded, otherwise false.</returns>
        public bool LoadWellKnownValue(object o)
        {
            if (o == null)
                ldnull.end();
            else
                switch (Type.GetTypeCode(o.GetType()))
                {
                    case TypeCode.Boolean: ldc_bool((Boolean)o); break;
                    case TypeCode.Char: ldc_i4_((Char)o); break;
                    case TypeCode.Single: ldc_r4((Single)o); break;
                    case TypeCode.Double: ldc_r8((Double)o); break;
                    case TypeCode.String: ldstr((String)o); break;
                    case TypeCode.SByte: ldc_i4_((SByte)o); break;
                    case TypeCode.Int16: ldc_i4_((Int16)o); break;
                    case TypeCode.Int32: ldc_i4_((Int32)o); break;
                    case TypeCode.Int64: ldc_i8((Int64)o); break;
                    case TypeCode.Byte: ldc_i4_((Byte)o); break;
                    case TypeCode.UInt16: ldc_i4_((UInt16)o); break;
                    case TypeCode.UInt32: ldc_i4_(unchecked((Int32)(UInt32)o)); break;
                    case TypeCode.UInt64: ldc_i8(unchecked((Int64)(UInt64)o)); break;
                    default:
                        return false;
                }

            return true;
        }
        /// <summary>
        /// Initialize parameter with some default value.
        /// </summary>
        /// <param name="parameterInfo">A method parameter.</param>
        /// <param name="index">The parameter index.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist Init(ParameterInfo parameterInfo, int index)
        {
            if (parameterInfo == null)
                throw new ArgumentNullException("parameterInfo");

            Type type = TypeEx.GetUnderlyingType(parameterInfo.ParameterType);

            if (parameterInfo.ParameterType.IsByRef)
            {
                type = type.GetElementType();

                return type.IsValueType && type.IsPrimitive == false ?
                    ldarg(index).initobj(type) :
                    ldarg(index).LoadInitValue(type).stind(type);
            }
            else
            {
                return type.IsValueType && type.IsPrimitive == false ?
                    ldarga(index).initobj(type) :
                    LoadInitValue(type).starg(index);
            }
        }
        /// <summary>
        /// Initialize all output parameters with some default value.
        /// </summary>
        /// <param name="parameters">A method parameters array.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist InitOutParameters(ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo pi = parameters[i];

                if (pi.IsOut)
                    Init(pi, i + 1);
            }

            return this;
        }
        /// <summary>
        /// Initialize local variable with some default value.
        /// </summary>
        /// <param name="localBuilder">A method local variable.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist Init(LocalBuilder localBuilder)
        {
            if (localBuilder == null)
                throw new ArgumentNullException("localBuilder");

            Type type = localBuilder.LocalType;

            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);

            return type.IsValueType && type.IsPrimitive == false ?
                ldloca(localBuilder).initobj(type) :
                LoadInitValue(type).stloc(localBuilder);
        }
        /// <summary>
        /// Loads a type instance at runtime.
        /// </summary>
        /// <param name="type">A type</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist LoadType(Type type)
        {
            return type == null ?
                ldnull :
                ldtoken(type).call(typeof(Type), "GetTypeFromHandle", typeof(RuntimeTypeHandle));
        }
        /// <summary>
        /// Loads a field instance at runtime.
        /// </summary>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist LoadField(FieldInfo fieldInfo)
        {
            return fieldInfo.IsStatic ? ldsfld(fieldInfo) : ldarg_0.ldfld(fieldInfo);
        }
        /// <summary>
        /// Cast an object passed by reference to the specified type
        /// or unbox a boxed value type.
        /// </summary>
        /// <param name="type">A type</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist CastFromObject(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return
                type == typeof(object) ? this :
                (type.IsValueType ? unbox_any(type) :
                    castclass(type));
        }
        /// <summary>
        /// Cast an object passed by reference to the specified type
        /// or unbox a boxed value type unless <paramref name="expectedType"/>
        /// is a parent of <paramref name="actualType"/>.
        /// </summary>
        /// <param name="expectedType">A type required.</param>
        /// <param name="actualType">A type available.</param>
        /// <returns>Current instance of the <see cref="EmitAssist"/>.</returns>
        public EmitAssist CastIfNecessary(Type expectedType, Type actualType)
        {
            if (expectedType == null) throw new ArgumentNullException("expectedType");
            if (actualType == null) throw new ArgumentNullException("actualType");

            return
                TypeEx.IsSameOrParent(expectedType, actualType) ? this :
                actualType.IsValueType ? unbox_any(expectedType) :
                    castclass(expectedType);
        }
        /// <summary>
        /// Increase max stack size by specified delta.
        /// </summary>
        /// <param name="size">Number of bytes to enlarge max stack size.</param>
        public void AddMaxStackSize(int size)
        {
            // m_maxStackSize isn't public so we need some hacking here.
            FieldInfo fi = _ilGenerator.GetType().GetField(
                "m_maxStackSize", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi != null)
            {
                size += (int)fi.GetValue(_ilGenerator);
                fi.SetValue(_ilGenerator, size);
            }
        }
        /// <summary>
        /// Custom Emit OpCode
        /// </summary>
        /// <param name="code">OpCode to emit</param>
        /// <returns>EmitAssist instance</returns>
        public EmitAssist Emit(OpCode code)
        {
            _ilGenerator.Emit(code);

            return this;
        }

        #endregion

        #endregion

        #region Public Property

        /// <summary>
        /// Gets Method Assist instance (MethodAssist or ConstructorAssist).
        /// </summary>
        public MethodAssistBase Method { get { return _method; } }
        /// <summary>
        /// Gets MSIL generator.
        /// </summary>
        public ILGenerator ILGenerator { get { return _ilGenerator; } }

        #endregion

        #region ILGenerator Operator overrloading

        /// <summary>
        /// Converts the supplied <see cref="EmitAssist"/> to a <see cref="ILGenerator"/>.
        /// </summary>
        /// <param name="emitAssist">The Emit Assist instance.</param>
        /// <returns>An ILGenerator.</returns>
        public static implicit operator ILGenerator(EmitAssist emitAssist)
        {
            if (emitAssist == null)
                throw new ArgumentNullException("emitAssist");
            return emitAssist.ILGenerator;
        }

        #endregion
    }

    #endregion
}

#endregion
