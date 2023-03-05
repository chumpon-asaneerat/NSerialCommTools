#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2009-12-24
=================
- Rewrite code for Reflection common classes
  - GenericBinder class is rewrite in C# 2.0 language spec.
  - New class added -> ReflectionConsts provide common place for constants and error messages.
  - NameOrIndexParameter class is rewrite in C# 2.0 language spec.
  - New classes added -> FakeParameterInfo and FakeMethodInfo class. Used internal for build
    Dynamic Types.

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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Diagnostics.SymbolStore;
using System.Diagnostics.CodeAnalysis;

using NLib.Reflection.Common;

#endregion

#region Consts and Error Messages

namespace NLib.Reflection
{
    #region Reflection Consts class

    /// <summary>
    /// Reflection Consts class. Provide common place to keep constant and message.
    /// </summary>
    public sealed class ReflectionConsts
    {
        #region Error Messages

        /// <summary>
        /// The Error Message.
        /// </summary>
        public sealed class Errors
        {
            #region For NameOrIndexParameter class

            /// <summary>
            /// Bad Name Error Message.
            /// </summary>
            public const string InitBadName =
                "Name must be a valid string.";
            /// <summary>
            /// Bad Index Error Message.
            /// </summary>
            public const string InitBadIndex =
                "The index parameter must be greater or equal to zero.";
            /// <summary>
            /// Invalid Access By Name.
            /// </summary>
            public const string InvalidOpGetName =
                "This instance was initialized by index.";
            /// <summary>
            /// Invalid Access By Index.
            /// </summary>
            public const string InvalidOpGetIndex =
                "This instance was initialized by name.";

            #endregion

            #region For EmitAssist class

            /// <summary>
            /// Not expected type.
            /// </summary>
            public static string NotExpectedType =
                "Type '{0}' is not expected.";
            /// <summary>
            /// Failed to get type for specificed method
            /// </summary>
            public static string NoSuchMethod =
                "Failed to query type '{0}' for method '{1}'.";

            #endregion
        }

        #endregion
    }

    #endregion
}

#endregion

#region Generic Binder

namespace NLib.Reflection
{
    #region Generic Binder

    /// <Summary>
    /// Selects a member from a list of candidates, and performs type conversion
    /// from actual argument type to formal argument type.
    /// </Summary>
    [Serializable]
    public class GenericBinder : Binder
    {
        #region Internal Variables

        private readonly bool _genericMethodDefinition;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="genericMethodDefinition">Specificed is generic method definition.</param>
        public GenericBinder(bool genericMethodDefinition)
        {
            _genericMethodDefinition = genericMethodDefinition;
        }

        #endregion

        #region Private Method

        private static bool CheckGenericTypeConstraints(Type genType, Type parameterType)
        {
            Type[] constraints = genType.GetGenericParameterConstraints();

            for (int i = 0; i < constraints.Length; i++)
                if (!constraints[i].IsAssignableFrom(parameterType))
                    return false;

            return true;
        }

        private static bool CompareGenericTypesRecursive(Type genType, Type specType)
        {
            Type[] genArgs = genType.GetGenericArguments();
            Type[] specArgs = specType.GetGenericArguments();

            bool match = (genArgs.Length == specArgs.Length);

            for (int i = 0; match && i < genArgs.Length; i++)
            {
                if (genArgs[i] == specArgs[i])
                    continue;

                if (genArgs[i].IsGenericParameter)
                    match = CheckGenericTypeConstraints(genArgs[i], specArgs[i]);
                else if (genArgs[i].IsGenericType && specArgs[i].IsGenericType)
                    match = CompareGenericTypesRecursive(genArgs[i], specArgs[i]);
                else
                    match = false;
            }

            return match;
        }

        #endregion

        #region Override methods

        /// <summary>
        /// Bind To Method. For more inforamtion <see cref="Binder"/>.
        /// </summary>
        /// <param name="bindingAttr">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="match">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="args">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="modifiers">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="culture">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="names">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="state">For more inforamtion <see cref="Binder"/>.</param>
        /// <returns>For more inforamtion <see cref="Binder"/>.</returns>
        public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args,
            ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state)
        {
            throw new NotImplementedException("GenericBinder.BindToMethod");
        }
        /// <summary>
        /// Bind To Field. For more inforamtion <see cref="Binder"/>.
        /// </summary>
        /// <param name="bindingAttr">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="match">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="value">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="culture">For more inforamtion <see cref="Binder"/>.</param>
        /// <returns>For more inforamtion <see cref="Binder"/>.</returns>
        public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture)
        {
            throw new NotImplementedException("GenericBinder.BindToField");
        }
        /// <summary>
        /// Select Method. For more inforamtion <see cref="Binder"/>.
        /// </summary>
        /// <param name="bindingAttr">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="matchMethods">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="parameterTypes">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="modifiers">For more inforamtion <see cref="Binder"/>.</param>
        /// <returns>For more inforamtion <see cref="Binder"/>.</returns>
        public override MethodBase SelectMethod(
            BindingFlags bindingAttr,
            MethodBase[] matchMethods,
            Type[] parameterTypes,
            ParameterModifier[] modifiers)
        {
            for (int i = 0; i < matchMethods.Length; ++i)
            {
                if (matchMethods[i].IsGenericMethodDefinition != _genericMethodDefinition)
                    continue;

                ParameterInfo[] pis = matchMethods[i].GetParameters();
                bool match = (pis.Length == parameterTypes.Length);

                for (int j = 0; match && j < pis.Length; ++j)
                {
                    match = TypeEx.CompareParameterTypes(pis[j].ParameterType, parameterTypes[j]);
                }

                if (match)
                    return matchMethods[i];
            }

            return null;
        }
        /// <summary>
        /// Select Property. For more inforamtion <see cref="Binder"/>.
        /// </summary>
        /// <param name="bindingAttr">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="match">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="returnType">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="indexes">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="modifiers">For more inforamtion <see cref="Binder"/>.</param>
        /// <returns>For more inforamtion <see cref="Binder"/>.</returns>
        public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType,
            Type[] indexes, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException("GenericBinder.SelectProperty");
        }
        /// <summary>
        /// Change Type. For more inforamtion <see cref="Binder"/>.
        /// </summary>
        /// <param name="value">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="type">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="culture">For more inforamtion <see cref="Binder"/>.</param>
        /// <returns>For more inforamtion <see cref="Binder"/>.</returns>
        public override object ChangeType(object value, Type type, CultureInfo culture)
        {
            throw new NotImplementedException("GenericBinder.ChangeType");
        }
        /// <summary>
        /// Reorder Argument Array.
        /// </summary>
        /// <param name="args">For more inforamtion <see cref="Binder"/>.</param>
        /// <param name="state">For more inforamtion <see cref="Binder"/>.</param>
        public override void ReorderArgumentArray(ref object[] args, object state)
        {
            throw new NotImplementedException("GenericBinder.ReorderArgumentArray");
        }

        #endregion

        #region Static Access

        private static GenericBinder _generic;
        private static GenericBinder _nonGeneric;

        /// <summary>
        /// Get Generic Binder instance.
        /// </summary>
        public static GenericBinder Generic
        {
            get { return _generic ?? (_generic = new GenericBinder(true)); }
        }
        /// <summary>
        /// Get Non Generic Binder instance.
        /// </summary>
        public static GenericBinder NonGeneric
        {
            get { return _nonGeneric ?? (_nonGeneric = new GenericBinder(false)); }
        }

        #endregion
    }

    #endregion
}

#endregion

#region Name Or Index Parameter

namespace NLib.Reflection.Common
{
    #region Name Or Index Parameter

    /// <summary>
    /// This argument adapter class allows either names (strings) or
    /// indices (ints) to be passed to a function.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public struct NameOrIndexParameter
    {
        #region Internal Variable

        private readonly string _name;
        private readonly int _index;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The Name.</param>
        public NameOrIndexParameter(string name)
        {
            if (null == name)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException(ReflectionConsts.Errors.InitBadName, "name");

            _name = name;
            _index = 0;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">The index.</param>
        public NameOrIndexParameter(int index)
        {
            if (index < 0)
                throw new ArgumentException(ReflectionConsts.Errors.InitBadIndex, "index");

            _name = null;
            _index = index;
        }

        #endregion

        #region Override methods

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj">object to compare</param>
        /// <returns>true if object is equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is NameOrIndexParameter)
            {
                NameOrIndexParameter nip = (NameOrIndexParameter)obj;

                if (null != _name && null != nip._name && _name == nip._name)
                    return true; // Same name

                if (null == _name && null == nip._name && _index == nip._index)
                    return true; // Same index

                return false;
            }

            if (obj is string)
            {
                string name = (string)obj;
                return (null != _name && _name == name);
            }

            if (obj is int)
            {
                int index = (int)obj;
                return (null == _name && _index == index);
            }

            return false;
        }
        /// <summary>
        /// Get Hash code
        /// </summary>
        /// <returns>Return hash code value.</returns>
        public override int GetHashCode()
        {
            return (null != _name) ? _name.GetHashCode() : _index.GetHashCode();
        }
        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>string that represent object instamce</returns>
        public override string ToString()
        {
            //return (null != _name) ? _name : "#" + _index;
            return _name ?? "#" + _index; // used new ?? operator in C# 2.0
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Is Parameter is initialized by Name
        /// </summary>
        [Browsable(false)]
        public bool ByName { get { return null != _name; } }
        /// <summary>
        /// Get Name
        /// </summary>
        [Browsable(false)]
        public string Name
        {
            get
            {
                if (null == _name)
                    throw new InvalidOperationException(ReflectionConsts.Errors.InvalidOpGetName);
                return _name;
            }
        }
        /// <summary>
        /// Get Index
        /// </summary>
        public int Index
        {
            get
            {
                if (null != _name)
                    throw new InvalidOperationException(ReflectionConsts.Errors.InvalidOpGetIndex);
                return _index;
            }
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Assign From String Array
        /// </summary>
        /// <param name="names">The Name array.</param>
        /// <returns>Return NameOrIndexParameter array.</returns>
        public static NameOrIndexParameter[] FromStringArray(string[] names)
        {
            return names.Select(name => new NameOrIndexParameter(name)).ToArray();
        }
        /// <summary>
        /// Assign From Index Array
        /// </summary>
        /// <param name="indices">The Index array.</param>
        /// <returns>Return NameOrIndexParameter array.</returns>
        public static NameOrIndexParameter[] FromIndexArray(int[] indices)
        {
            return indices.Select(index => new NameOrIndexParameter(index)).ToArray();
        }

        #endregion

        #region Operator overloadings

        /// <summary>
        /// Convert string to NameOrIndexParameter
        /// </summary>
        /// <param name="name">Name to convert</param>
        /// <returns>new instance of NameOrIndexParameter</returns>
        public static implicit operator NameOrIndexParameter(string name)
        {
            return new NameOrIndexParameter(name);
        }
        /// <summary>
        /// Convert int to NameOrIndexParameter
        /// </summary>
        /// <param name="index">index to convert</param>
        /// <returns>new instance of NameOrIndexParameter</returns>
        public static implicit operator NameOrIndexParameter(int index)
        {
            return new NameOrIndexParameter(index);
        }

        #endregion
    }

    #endregion
}

#endregion

#region Fake Method Info

namespace NLib.Reflection.Common
{
    #region FakeMethodInfo

    /// <summary>
    /// Fake Method Info Abstract class.
    /// </summary>
    abstract class FakeMethodInfo : MethodInfo
    {
        #region Internal Class

        /// <summary>
        /// Custom Attribute Provider
        /// </summary>
        class CustomAttributeProvider : ICustomAttributeProvider
        {
            static readonly object[] _object = new object[0];

            /// <summary>
            /// Get Custom Attributes
            /// </summary>
            /// <param name="inherit">True for search in chain.</param>
            /// <returns></returns>
            public object[] GetCustomAttributes(bool inherit)
            {
                return _object;
            }
            /// <summary>
            /// Get Custom Attributes
            /// </summary>
            /// <param name="attributeType"></param>
            /// <param name="inherit">True for search in chain.</param>
            /// <returns></returns>
            public object[] GetCustomAttributes(Type attributeType, bool inherit)
            {
                return _object;
            }
            /// <summary>
            /// IsDefined.
            /// </summary>
            /// <param name="attributeType">The attribute.</param>
            /// <param name="inherit">True for search in chain.</param>
            /// <returns>Return in this class always return false.</returns>
            public bool IsDefined(Type attributeType, bool inherit)
            {
                return false;
            }
        }

        #endregion

        #region Protected and Static Readonly Variables

        /// <summary>
        /// Access associated MethodInfo.
        /// </summary>
        protected MethodInfo _pair;
        /// <summary>
        /// Access associated PropertyInfo.
        /// </summary>
        protected PropertyInfo _property;
        /// <summary>
        /// The custom attribute provider.
        /// </summary>
        static readonly CustomAttributeProvider _customAttributeProvider = new CustomAttributeProvider();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="propertyInfo">The property information instance.</param>
        /// <param name="pair">The Method Info instance.</param>
        protected FakeMethodInfo(PropertyInfo propertyInfo, MethodInfo pair)
        {
            _property = propertyInfo;
            _pair = pair;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Get Method Attributes.
        /// </summary>
        public override MethodAttributes Attributes
        {
            get { return _pair.Attributes; }
        }
        /// <summary>
        /// Get Calling Conventions.
        /// </summary>
        public override CallingConventions CallingConvention
        {
            get { return _pair.CallingConvention; }
        }
        /// <summary>
        /// Get Declaring Type.
        /// </summary>
        public override Type DeclaringType
        {
            get { return _property.DeclaringType; }
        }
        /// <summary>
        /// Get Base Definition.
        /// </summary>
        /// <returns>Return method information instance.</returns>
        public override MethodInfo GetBaseDefinition()
        {
            return _pair.GetBaseDefinition();
        }
        /// <summary>
        /// Get Custom Attributes.
        /// </summary>
        /// <param name="inherit">True for chain search.</param>
        /// <returns>Return custom attributes for specificed property.</returns>
        public override object[] GetCustomAttributes(bool inherit)
        {
            return _property.GetCustomAttributes(inherit);
        }
        /// <summary>
        /// Get Custom Attributes.
        /// </summary>
        /// <param name="attributeType">The attribute type to search for.</param>
        /// <param name="inherit">True for chain search.</param>
        /// <returns>Return custom attributes for specificed property.</returns>
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _property.GetCustomAttributes(attributeType, inherit);
        }
        /// <summary>
        /// Get Method Implementation Flags
        /// </summary>
        /// <returns>Return MethodImplAttributes instance.</returns>
        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return _pair.GetMethodImplementationFlags();
        }
        /// <summary>
        /// Invoke.
        /// </summary>
        /// <param name="obj">The target object.</param>
        /// <param name="invokeAttr">The Binding Flags.</param>
        /// <param name="binder">The Binder instance.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="culture">The CultureInfo instance.</param>
        /// <returns>Return object that return from invocation.</returns>
        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder,
            object[] parameters, CultureInfo culture)
        {
            return null;
        }
        /// <summary>
        /// Check Is Defined speficifed attribute.
        /// </summary>
        /// <param name="attributeType">The speficifed attribute.</param>
        /// <param name="inherit">True for chain search.</param>
        /// <returns>Return In this class this method always return false.</returns>
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return false;
        }
        /// <summary>
        /// Get Member Type.
        /// </summary>
        public override MemberTypes MemberType
        {
            get { return _property.MemberType; }
        }
        /// <summary>
        /// Get Method Handle.
        /// </summary>
        public override RuntimeMethodHandle MethodHandle
        {
            get { return new RuntimeMethodHandle(); }
        }
        /// <summary>
        /// Get Reflected Type.
        /// </summary>
        public override Type ReflectedType
        {
            get { return _property.ReflectedType; }
        }
        /// <summary>
        /// Get Custom Attribute for Return Type.
        /// </summary>
        public override ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get { return _customAttributeProvider; }
        }
        /// <summary>
        /// Get Return parameter.
        /// </summary>
        public override ParameterInfo ReturnParameter
        {
            get { return new FakeParameterInfo("ret", ReturnType, this, null); }
        }

        #endregion
    }

    #endregion

    #region FakeParameterInfo

    /// <summary>
    /// FakeParameterInfo class.
    /// </summary>
    class FakeParameterInfo : ParameterInfo
    {
        #region Internal Variable

        private readonly string _name;
        private readonly Type _type;
        private readonly MemberInfo _memberInfo;

        private readonly object[] _attributes;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The parameter's name.</param>
        /// <param name="type">The parameter's type.</param>
        /// <param name="memberInfo">The Member info instance.</param>
        /// <param name="attributes">The attributes.</param>
        public FakeParameterInfo(string name, Type type, MemberInfo memberInfo,
            object[] attributes)
        {
            _name = name;
            _type = type;
            _memberInfo = memberInfo;
            _attributes = attributes ?? new object[0];
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="method">The method info instance.</param>
        public FakeParameterInfo(MethodInfo method)
            : this("ret", method.ReturnType, method,
            method.ReturnTypeCustomAttributes.GetCustomAttributes(true))
        {
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Get Parameter's Attributes.
        /// </summary>
        public override ParameterAttributes Attributes
        {
            get { return ParameterAttributes.Retval; }
        }
        /// <summary>
        /// Get Default Value. Alway return DBNull.Value.
        /// </summary>
        public override object DefaultValue { get { return DBNull.Value; } }
        /// <summary>
        /// Get Custom Attributes.
        /// </summary>
        /// <param name="inherit">True for search chain.</param>
        /// <returns>Return Custom Attributes.</returns>
        public override object[] GetCustomAttributes(bool inherit)
        {
            return _attributes;
        }
        /// <summary>
        /// Get Custom Attributes.
        /// </summary>
        /// <param name="attributeType">The attribute type to serach for.</param>
        /// <param name="inherit">True for search chain.</param>
        /// <returns>Return Custom Attributes.</returns>
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            if (attributeType == null)
                throw new ArgumentNullException("attributeType");

            if (_attributes.Length == 0)
                return (object[])Array.CreateInstance(attributeType, 0);

            ArrayList list = new ArrayList();

            foreach (object o in _attributes)
                if (o.GetType() == attributeType || attributeType.IsInstanceOfType(o))
                    list.Add(o);

            return (object[])list.ToArray(attributeType);
        }
        /// <summary>
        /// Is Defined.
        /// </summary>
        /// <param name="attributeType">The attribute type to serach for.</param>
        /// <param name="inherit">True for search chain.</param>
        /// <returns>Return true if match.</returns>
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            if (attributeType == null) throw new ArgumentNullException("attributeType");

            foreach (object o in _attributes)
                if (o.GetType() == attributeType || attributeType.IsInstanceOfType(o))
                    return true;

            return false;
        }
        /// <summary>
        /// Get MemberInfo instance.
        /// </summary>
        public override MemberInfo Member { get { return _memberInfo; } }
        /// <summary>
        /// Get Parameter's Name.
        /// </summary>
        public override string Name { get { return _name; } }
        /// <summary>
        /// Get Parameter's Type.
        /// </summary>
        public override Type ParameterType { get { return _type; } }
        /// <summary>
        /// Get Position. Alway return 0.
        /// </summary>
        public override int Position { get { return 0; } }

        #endregion
    }

    #endregion
}

#endregion
