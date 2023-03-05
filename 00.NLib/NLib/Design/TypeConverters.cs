#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-03-08
=================
- Property Sorter TypeConverters classes changed.
  - All Property Sorter TypeConverters classes is change from abstract to normal.

======================================================================================================================
Update 2011-11-07
=================
- TypeConverters classes ported from GFA Library GFA40 to NLib 

======================================================================================================================
Update 2010-01-23
=================
- TypeConverters ported from GFA Library GFA37 tor GFA38v3

======================================================================================================================
Update 2008-11-29
=================
- TypeConverters move from GFA.Lib to GFA.Lib.Core.
- change PropertyOrderPair class access to public.

======================================================================================================================
Update 2007-11-26
=================
- TypeConverters ported from GFA Library v.2.27 with some fixed. Basically support custom
  property order in Property grid.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

#endregion

namespace NLib.Design
{
    #region Property Order Pair and Attribute

    #region PropertyOrderPair

    /// <summary>
    /// PropertyOrderPair used for compare property order.
    /// </summary>
    public class PropertyOrderPair : IComparable
    {
        #region Internal Variable

        private int _order = 0;
        private string _name = "";

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Property Name</param>
        /// <param name="order">Property Order</param>
        public PropertyOrderPair(string name, int order)
        {
            _order = order;
            _name = name;
        }

        #endregion

        #region IComparable implement - CompareTo

        /// <summary>
        /// IComparable CompareTo Implement
        /// </summary>
        /// <param name="obj">object to compare with current object</param>
        /// <returns>see IComparable.CompareTo Document</returns>
        public int CompareTo(object obj)
        {
            //
            // Sort the pair objects by ordering by order value
            // Equal values get the same rank
            //
            int otherOrder = ((PropertyOrderPair)obj)._order;
            if (otherOrder == _order)
            {
                //
                // If order not specified, sort by name
                //
                string otherName = ((PropertyOrderPair)obj)._name;
                return string.Compare(_name, otherName);
            }
            else if (otherOrder > _order) return -1;
            return 1;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get Property Name
        /// </summary>
        [Category("Properties")]
        [Description("Get Property Name")]
        public string Name { get { return _name; } }

        #endregion
    }

    #endregion

    #region PropertyOrder Attribute

    /// <summary>
    /// Property Order Attribute Class for Custom Sort Property in Property Grid
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyOrderAttribute : Attribute
    {
        #region Internal Variable

        private int _order = 1;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="order">Property's Order. default value is 1</param>
        public PropertyOrderAttribute(int order) { _order = order; }

        #endregion

        #region Public Property

        /// <summary>
        /// Get Property Order. default value is 1
        /// </summary>
        [Category("Properties")]
        [Description("Get Property Order. default value is 1")]
        public int Order { get { return _order; } }

        #endregion
    }

    #endregion

    #endregion
}

namespace NLib.Design
{
    #region Note: about TypeConverter and Serializable.

    // When used Serializable attribute with Type Converter windows forms will used resource
    // to stored information and not used logic in Type Converter class.
    // So when need to make custom code serialization should used TypeConverter only
    // because XmlSerializer not need Serializable attribute and still save and load correctly.
    //
    // Example.
    //
    // internal class NodeTypeConverter : NLib.Design.CoreTypeConverter
    // {
    //      protected override Type ObjectType { get { return typeof(NLib.Tests.Node); } }
    //      protected override string[] Properties { get { return new string[] { "Name", "Nodes" }; } }
    // }
    //
    // [TypeConverter(typeof(Design.NodeTypeConverter))]
    // public class Node
    // {
    //      private string _name = string.Empty;
    //      private NodeCollection _nodes = new NodeCollection();
    //
    //      public Node()
    //          : base()
    //      {
    //      }
    //      public Node(string name, Node[] nodes)
    //          : base()
    //      {
    //          _name = name;
    //          if (nodes != null && nodes.Length > 0)
    //          {
    //              _nodes.AddRange(nodes);
    //          }
    //      }
    //      [XmlAttribute]
    //      public string Name 
    //      { 
    //          get { return _name; } 
    //          set 
    //          { 
    //              _name = value;
    //          } 
    //      }
    //      [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    //      public NodeCollection Nodes { get { return _nodes; } }
    // }
    //
    // [ListBindable(BindableSupport.No)]
    // public class NodeCollection : CollectionBase
    // {
    //      public NodeCollection() : base() {}
    //      ...
    //      ...
    //      Implement list access...
    //      ...
    //      ...
    //      public Node[] ToArray() { return (Node[])InnerList.ToArray(typeof(Node)); }
    // }

    #endregion

    #region Property Sorter Support TypeConverters (abstract)

    /// <summary>
    /// Property Sorter Support Type Converter abstract Class for Custom Sort Property in Property Grid
    /// </summary>
    public class PropertySorterSupportTypeConverter : TypeConverter
    {
        #region Override Methods

        /// <summary>
        /// see TypeConverter.GetPropertiesSupported Document
        /// </summary>
        /// <param name="context">see TypeConverter Document</param>
        /// <returns>see TypeConverter Document</returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        /// <summary>
        /// see TypeConverter.GetProperties Document
        /// </summary>
        /// <param name="context">see TypeConverter Document</param>
        /// <param name="value">see TypeConverter Document</param>
        /// <param name="attributes">see TypeConverter Document</param>
        /// <returns>see TypeConverter Document</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value, attributes);
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
            ArrayList propertyNames = new ArrayList();
            foreach (PropertyOrderPair pop in orderedProperties) propertyNames.Add(pop.Name);
            //
            // Pass in the ordered list for the PropertyDescriptorCollection to sort by
            //
            return pdc.Sort((string[])propertyNames.ToArray(typeof(string)));
        }

        #endregion
    }
    /// <summary>
    /// Property Sorter Support Expandable Type Converter abstract Class for Custom Sort Property in Property Grid
    /// </summary>
    public class PropertySorterSupportExpandableTypeConverter : ExpandableObjectConverter
    {
        #region Override Methods

        /// <summary>
        /// see ExpandableObjectConverter.GetPropertiesSupported Document
        /// </summary>
        /// <param name="context">see ExpandableObjectConverter Document</param>
        /// <returns>see ExpandableObjectConverter Document</returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        /// <summary>
        /// see ExpandableObjectConverter.GetProperties Document
        /// </summary>
        /// <param name="context">see ExpandableObjectConverter Document</param>
        /// <param name="value">see ExpandableObjectConverter Document</param>
        /// <param name="attributes">see ExpandableObjectConverter Document</param>
        /// <returns>see ExpandableObjectConverter Document</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value, attributes);
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
            ArrayList propertyNames = new ArrayList();
            foreach (PropertyOrderPair pop in orderedProperties) propertyNames.Add(pop.Name);
            //
            // Pass in the ordered list for the PropertyDescriptorCollection to sort by
            //
            return pdc.Sort((string[])propertyNames.ToArray(typeof(string)));
        }

        #endregion
    }

    #endregion

    #region Abstract Type Converters

    /// <summary>
    /// Abstract Type Converter class
    /// </summary>
    public abstract class AbstractTypeConverter : PropertySorterSupportTypeConverter
    {
        #region Override

        #region CanConvertTo

        /// <summary>
        /// CanConvertTo. By default implementation only handle when 
        /// destinationType is System.String or 
        /// System.ComponentModel.Design.Serialization.InstanceDescriptor
        /// </summary>
        /// <param name="context">see ITypeDescriptorContext</param>
        /// <param name="destinationType">Destination Type in this case is System.String or
        /// System.ComponentModel.Design.Serialization.InstanceDescriptor</param>
        /// <returns>True if condition is match. in this case when destination type is
        /// System.String or System.ComponentModel.Design.Serialization.InstanceDescriptor
        /// this method will return true</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) ||
                destinationType == typeof(string)) return true;
            return base.CanConvertTo(context, destinationType);
        }

        #endregion

        #region ConvertTo

        /// <summary>
        /// ConvertTo
        /// 
        /// Note.
        /// For destination type is System.String
        /// ------------------------------------------
        /// Handle convert to String. this used when property grid need to
        /// display custom information about selected object that current edit
        /// for example when edit object in collection editor sometime
        /// we need to display user friendly information rather than
        /// show object type (by default if not implement this method)
        /// we need to check and format the display information here.
        /// beware when implement this method please make sure that format
        /// that used to display on convert to string is match with format
        /// that implement in ConvertFrom Method
        /// 
        /// For destination type is InstanceDescriptor
        /// ------------------------------------------
        /// </summary>
        /// <param name="context">see ITypeDescriptorContext</param>
        /// <param name="culture">culture information in some proeprty this
        /// parameter is need to check</param>
        /// <param name="value">value to convert</param>
        /// <param name="destinationType">target type to transform</param>
        /// <returns>object instance that has same type as destination type</returns>
        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            if (value != null)
            {
                if (destinationType == typeof(string))
                {
                    // Handle convert to String. this used when property grid need to
                    // display custom information about selected object that current edit
                    // for example when edit object in collection editor sometime
                    // we need to display user friendly information rather than
                    // show object type (by default if not implement this method)
                    // we need to check and format the display information here.
                    // beware when implement this method please make sure that format
                    // that used to display on convert to string is match with format
                    // that implement in ConvertFrom Method
                    if (CanConvertToDisplayString(culture, value))
                    {
                        return ConvertToDisplayString(culture, value);
                    }
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
                    // Handle convert to InstanceDescriptor
                    if (CanConvertToInstanceDescriptor(culture, value))
                    {
                        try
                        {
                            return ConvertToInstanceDescriptor(culture, value);
                        }
                        catch (Exception)
                        {
                            // used default
                            //LogManager.Error(ex);
                            return base.ConvertTo(context, culture, value, destinationType);
                        }
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion

        #region CanConvertFrom

        /// <summary>
        /// CanConvertFrom. By default this class is handle only source type is System.String
        /// </summary>
        /// <param name="context">see ITypeDescriptorContext</param>
        /// <param name="sourceType">Type of Source component</param>
        /// <returns>True if condition is match in this case if source type is System.String
        /// it's will return true otherwise return base.CanConvertFrom(...)</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            // convert from string to target or owner class
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        #endregion

        #region ConvertFrom

        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="context">see ITypeDescriptorContext</param>
        /// <param name="culture">culture information in some proeprty this
        /// parameter is need to check</param>
        /// <param name="value">value to convert</param>
        /// <returns>object instance that convert from value parameter</returns>
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value != null)
            {
                // Handle convert from String to create target instance
                if (value is string)
                {
                    if (CanConvertFormDisplayString(culture, (value as string)))
                    {
                        try
                        {
                            return ConvertFormDisplayString(culture, (value as string));
                        }
                        catch (Exception)
                        {
                            //LogManager.Error(ex);
                            return base.ConvertFrom(context, culture, value);
                        }
                    }
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        #endregion

        #endregion

        #region Virtual Method

        #region CanConvertToDisplayString

        /// <summary>
        /// CanConvertToDisplayString override when the interited class is implement
        /// ConvertToDisplayString method. see ConvertTo for more information
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">object to check</param>
        /// <returns>true if ConvertToDisplayString method is impment for target
        /// object by default return true</returns>
        protected virtual bool CanConvertToDisplayString(CultureInfo culture, object value)
        {
            return true;
        }

        #endregion

        #region ConvertToDisplayString

        /// <summary>
        /// ConvertToDisplayString  override when the interited class need to format display
        /// string for target object for user friendly information about target object.
        /// see ConvertTo for more information
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">target object to format display on proeprty grid</param>
        /// <returns>format string for target object by default will return value.ToString()</returns>
        protected virtual string ConvertToDisplayString(CultureInfo culture, object value)
        {
            if (value == null)
                return string.Empty;
            return value.ToString();
        }

        #endregion

        #region CanConvertFormDisplayString

        /// <summary>
        /// CanConvertFormDisplayString override this method when ConvertFormDisplayString
        /// is implement(override)
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">string to check</param>
        /// <returns>true if object is match and ConvertFormDisplayString is implement.
        /// by default will return false.</returns>
        protected virtual bool CanConvertFormDisplayString(CultureInfo culture, string value)
        {
            return false;
        }

        #endregion

        #region ConvertFormDisplayString

        /// <summary>
        /// ConvertFormDisplayString override to create new instance from format string
        /// note that the format in ConvertFromDisplayString and ConvertToDisplayString
        /// should match see ConvertToDisplayString for more information
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">string to convert</param>
        /// <returns>new object instance that convert from string. by default return null.</returns>
        protected virtual object ConvertFormDisplayString(CultureInfo culture, string value)
        {
            return null;
        }

        #endregion

        #region CanConvertToInstanceDescriptor

        /// <summary>
        /// CanConvertToInstanceDescriptor override to handle ConvertToInstanceDescriptor method
        /// by default this method is return false
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">value to check</param>
        /// <returns>true if ConvertToInstanceDescriptor is implement.
        /// by default this method is return false</returns>
        protected virtual bool CanConvertToInstanceDescriptor(CultureInfo culture,
            object value)
        {
            return false;
        }

        #endregion

        #region ConvertToInstanceDescriptor

        /// <summary>
        /// ConvertToInstanceDescriptor override to implement how to create new instance
        /// descriptor by specificed object instance. to create new instance descriptor
        /// we need to specificed proper constructor with types and values that match which
        /// the constructor.
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">value to convert</param>
        /// <returns>return new InstanceDescriptor instance. by default this method will
        /// return null. see CreateInstanceDescriptor for helper method that will help to
        /// create new InstanceDescriptor</returns>
        protected virtual InstanceDescriptor ConvertToInstanceDescriptor(CultureInfo culture,
            object value)
        {
            return null;
        }

        #endregion

        #region CreateInstanceDescriptor

        /// <summary>
        /// CreateInstanceDescriptor
        /// </summary>
        /// <param name="targetType">Target Object Type</param>
        /// <param name="types">Type of Parameters in Constructor that need to used for
        /// create new instance</param>
        /// <param name="properties">Value of Parameters then match types and need for
        /// create new instance</param>
        /// <returns>new InstanceDescriptor object</returns>
        protected virtual InstanceDescriptor CreateInstanceDescriptor(Type targetType,
            Type[] types, object[] properties)
        {
            if (targetType == null)
                return null; // no type specificed

            if ((types != null && properties == null) ||
                (types == null && properties != null))
                return null; // parameter not match

            if (types != null && properties != null &&
                types.Length != properties.Length)
                return null; // parameter length not match

            // get constructor that match all types
            ConstructorInfo ci = targetType.GetConstructor(types);
            // create new InstanceDescriptor by constructor information and properties
            return new InstanceDescriptor(ci, properties);
        }

        #endregion

        #endregion

        #region Protected Methods

        #region Extract Display Strings

        /// <summary>
        /// Extract Display Strings
        /// </summary>
        /// <param name="value">Display string</param>
        /// <returns>Array of string that used to be constructor parameter</returns>
        protected string[] ExtractDisplayStrings(string value)
        {
            return ExtractDisplayStrings(value, new char[] { ';', ':' });
        }
        /// <summary>
        /// Extract Display Strings
        /// </summary>
        /// <param name="value">Display string</param>
        /// <param name="delimeterChars">Delimeter Chars</param>
        /// <returns>Array of string that used to be constructor parameter</returns>
        protected string[] ExtractDisplayStrings(string value, char[] delimeterChars)
        {
            if (value.Length <= 0)
                return null;

            string[] elements = null;
            if (delimeterChars.Length <= 0)
                elements = value.Split(new char[] { ';', ':' });
            else elements = value.Split(delimeterChars);

            ArrayList list = new ArrayList();
            for (int i = 1; i < elements.Length; i += 2)
            {
                list.Add(elements[i].Trim());
            }

            string[] results = null;
            if (list.Count > 0)
            {
                results = (string[])list.ToArray(typeof(string));
            }
            return results;
        }

        #endregion

        #endregion
    }
    /// <summary>
    /// Abstract Expandable TypeConverter class
    /// </summary>
    public abstract class AbstractExpandableTypeConverter : PropertySorterSupportExpandableTypeConverter
    {
        #region Override

        #region CanConvertTo

        /// <summary>
        /// CanConvertTo. By default implementation only handle when 
        /// destinationType is System.String or 
        /// System.ComponentModel.Design.Serialization.InstanceDescriptor
        /// </summary>
        /// <param name="context">see ITypeDescriptorContext</param>
        /// <param name="destinationType">Destination Type in this case is System.String or
        /// System.ComponentModel.Design.Serialization.InstanceDescriptor</param>
        /// <returns>True if condition is match. in this case when destination type is
        /// System.String or System.ComponentModel.Design.Serialization.InstanceDescriptor
        /// this method will return true</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) ||
                destinationType == typeof(string)) return true;
            return base.CanConvertTo(context, destinationType);
        }

        #endregion

        #region ConvertTo

        /// <summary>
        /// ConvertTo
        /// Note.
        /// 
        /// For destination type is System.String
        /// ------------------------------------------
        /// Handle convert to String. this used when property grid need to
        /// display custom information about selected object that current edit
        /// for example when edit object in collection editor sometime
        /// we need to display user friendly information rather than
        /// show object type (by default if not implement this method)
        /// we need to check and format the display information here.
        /// beware when implement this method please make sure that format
        /// that used to display on convert to string is match with format
        /// that implement in ConvertFrom Method
        /// 
        /// For destination type is InstanceDescriptor
        /// ------------------------------------------
        /// </summary>
        /// <param name="context">see ITypeDescriptorContext</param>
        /// <param name="culture">culture information in some proeprty this
        /// parameter is need to check</param>
        /// <param name="value">value to convert</param>
        /// <param name="destinationType">target type to transform</param>
        /// <returns>object instance that has same type as destination type</returns>
        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            if (value != null)
            {
                if (destinationType == typeof(string))
                {
                    // Handle convert to String. this used when property grid need to
                    // display custom information about selected object that current edit
                    // for example when edit object in collection editor sometime
                    // we need to display user friendly information rather than
                    // show object type (by default if not implement this method)
                    // we need to check and format the display information here.
                    // beware when implement this method please make sure that format
                    // that used to display on convert to string is match with format
                    // that implement in ConvertFrom Method
                    if (CanConvertToDisplayString(culture, value))
                    {
                        return ConvertToDisplayString(culture, value);
                    }
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
                    // Handle convert to InstanceDescriptor
                    if (CanConvertToInstanceDescriptor(culture, value))
                    {
                        try
                        {
                            return ConvertToInstanceDescriptor(culture, value);
                        }
                        catch (Exception)
                        {
                            // used default
                            //LogManager.Error(ex);
                            return base.ConvertTo(context, culture, value, destinationType);
                        }
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion

        #region CanConvertFrom

        /// <summary>
        /// CanConvertFrom. By default this class is handle only source type is System.String
        /// </summary>
        /// <param name="context">see ITypeDescriptorContext</param>
        /// <param name="sourceType">Type of Source component</param>
        /// <returns>True if condition is match in this case if source type is System.String
        /// it's will return true otherwise return base.CanConvertFrom(...)</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            // convert from string to target or owner class
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        #endregion

        #region ConvertFrom

        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="context">see ITypeDescriptorContext</param>
        /// <param name="culture">culture information in some proeprty this
        /// parameter is need to check</param>
        /// <param name="value">value to convert</param>
        /// <returns>object instance that convert from value parameter</returns>
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value != null)
            {
                // Handle convert from String to create target instance
                if (value is string)
                {
                    if (CanConvertFormDisplayString(culture, (value as string)))
                    {
                        try
                        {
                            return ConvertFormDisplayString(culture, (value as string));
                        }
                        catch (Exception)
                        {
                            //LogManager.Error(ex);
                            return base.ConvertFrom(context, culture, value);
                        }
                    }
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        #endregion

        #region GetCreateInstanceSupported

        /// <summary>
        /// GetCreateInstanceSupported. Default this method will return true to force
        /// create new instance when sub proeprties is about to changed
        /// for example if we has class A that contain class B as property and class
        /// B has 2 properties like X and Y. So changed occur with X or Y property
        /// will cause to create new instance of class B and the property grid will
        /// assign new instance to editting object (class A's instance)
        /// </summary>
        /// <param name="context">Type Descriptor Context Interface's instance</param>
        /// <returns>true if support create instance</returns>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        #endregion

        #endregion

        #region Virtual Method

        #region CanConvertToDisplayString

        /// <summary>
        /// CanConvertToDisplayString override when the interited class is implement
        /// ConvertToDisplayString method. see ConvertTo for more information
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">object to check</param>
        /// <returns>true if ConvertToDisplayString method is impment for target
        /// object by default return true</returns>
        protected virtual bool CanConvertToDisplayString(CultureInfo culture, object value)
        {
            return true;
        }

        #endregion

        #region ConvertToDisplayString

        /// <summary>
        /// ConvertToDisplayString  override when the interited class need to format display
        /// string for target object for user friendly information about target object.
        /// see ConvertTo for more information
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">target object to format display on proeprty grid</param>
        /// <returns>format string for target object by default will return value.ToString()</returns>
        protected virtual string ConvertToDisplayString(CultureInfo culture, object value)
        {
            if (value == null)
                return string.Empty;
            return value.ToString();
        }

        #endregion

        #region CanConvertFormDisplayString

        /// <summary>
        /// CanConvertFormDisplayString override this method when ConvertFormDisplayString
        /// is implement(override)
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">string to check</param>
        /// <returns>true if object is match and ConvertFormDisplayString is implement.
        /// by default will return false.</returns>
        protected virtual bool CanConvertFormDisplayString(CultureInfo culture, string value)
        {
            return false;
        }

        #endregion

        #region ConvertFormDisplayString

        /// <summary>
        /// ConvertFormDisplayString override to create new instance from format string
        /// note that the format in ConvertFromDisplayString and ConvertToDisplayString
        /// should match see ConvertToDisplayString for more information
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">string to convert</param>
        /// <returns>new object instance that convert from string. by default return null.</returns>
        protected virtual object ConvertFormDisplayString(CultureInfo culture, string value)
        {
            return null;
        }

        #endregion

        #region CanConvertToInstanceDescriptor

        /// <summary>
        /// CanConvertToInstanceDescriptor override to handle ConvertToInstanceDescriptor method
        /// by default this method is return false
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">value to check</param>
        /// <returns>true if ConvertToInstanceDescriptor is implement.
        /// by default this method is return false</returns>
        protected virtual bool CanConvertToInstanceDescriptor(CultureInfo culture,
            object value)
        {
            return false;
        }

        #endregion

        #region ConvertToInstanceDescriptor

        /// <summary>
        /// ConvertToInstanceDescriptor override to implement how to create new instance
        /// descriptor by specificed object instance. to create new instance descriptor
        /// we need to specificed proper constructor with types and values that match which
        /// the constructor.
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">value to convert</param>
        /// <returns>return new InstanceDescriptor instance. by default this method will
        /// return null. see CreateInstanceDescriptor for helper method that will help to
        /// create new InstanceDescriptor</returns>
        protected virtual InstanceDescriptor ConvertToInstanceDescriptor(CultureInfo culture,
            object value)
        {
            return null;
        }

        #endregion

        #region CreateInstanceDescriptor

        /// <summary>
        /// CreateInstanceDescriptor
        /// </summary>
        /// <param name="targetType">Target Object Type</param>
        /// <param name="types">Type of Parameters in Constructor that need to used for
        /// create new instance</param>
        /// <param name="properties">Value of Parameters then match types and need for
        /// create new instance</param>
        /// <returns>new InstanceDescriptor object</returns>
        protected virtual InstanceDescriptor CreateInstanceDescriptor(Type targetType,
            Type[] types, object[] properties)
        {
            if (targetType == null)
                return null; // no type specificed

            if ((types != null && properties == null) ||
                (types == null && properties != null))
                return null; // parameter not match

            if (types != null && properties != null &&
                types.Length != properties.Length)
                return null; // parameter length not match

            // get constructor that match all types
            ConstructorInfo ci = targetType.GetConstructor(types);
            // create new InstanceDescriptor by constructor information and properties
            return new InstanceDescriptor(ci, properties);
        }

        #endregion

        #region CreateInstanceFromProperties

        /// <summary>
        /// CreateInstanceFromProperties
        /// </summary>
        /// <param name="propertyValues">List of property's values that need to create
        /// new object instance</param>
        /// <returns>new object instance that need to assigned to owner's object
        /// by default this method will return null</returns>
        protected virtual object CreateInstanceFromProperties(IDictionary propertyValues)
        {
            return null;
        }

        #endregion

        #endregion

        #region Protected Methods

        #region Extract Display Strings

        /// <summary>
        /// Extract Display Strings
        /// </summary>
        /// <param name="value">Display string</param>
        /// <returns>Array of string that used to be constructor parameter</returns>
        protected string[] ExtractDisplayStrings(string value)
        {
            return ExtractDisplayStrings(value, new char[] { ';', ':' });
        }
        /// <summary>
        /// Extract Display Strings
        /// </summary>
        /// <param name="value">Display string</param>
        /// <param name="delimeterChars">Delimeter Chars</param>
        /// <returns>Array of string that used to be constructor parameter</returns>
        protected string[] ExtractDisplayStrings(string value, char[] delimeterChars)
        {
            if (value.Length <= 0)
                return null;

            string[] elements = null;
            if (delimeterChars.Length <= 0)
                elements = value.Split(new char[] { ';', ':' });
            else elements = value.Split(delimeterChars);

            ArrayList list = new ArrayList();
            for (int i = 1; i < elements.Length; i += 2)
            {
                list.Add(elements[i].Trim());
            }

            string[] results = null;
            if (list.Count > 0)
            {
                results = (string[])list.ToArray(typeof(string));
            }
            return results;
        }

        #endregion

        #endregion
    }

    #endregion

    #region Core Type Converter

    /// <summary>
    /// Core Type Converter. All Typeconverter should inherited from this class
    /// </summary>
    public abstract class CoreTypeConverter : AbstractTypeConverter
    {
        #region Internal Variable

        private Type _type = null;
        private string[] _properties = new string[] { };

        private BindingFlags _bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private PropertyInfo[] _propInfos = new PropertyInfo[] { };
        private Type[] _propTypes = new Type[] { };

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor (protected)
        /// </summary>
        protected CoreTypeConverter()
            : base()
        {
            _type = ObjectType;
            _properties = Properties;

            #region Reflect Informations

            if (_type != null && _properties != null && _properties.Length > 0)
            {
                PropertyInfo info;
                ArrayList listProp = new ArrayList();
                ArrayList listType = new ArrayList();
                foreach (string property in _properties)
                {
                    try
                    {
                        info = _type.GetProperty(property, _bindFlags);
                    }
                    catch (Exception)
                    {
                        //LogManager.Error(ex);
                        info = null;
                    }
                    if (info != null)
                    {
                        listProp.Add(info);
                        listType.Add(info.PropertyType);
                    }
                }
                _propInfos = (PropertyInfo[])listProp.ToArray(typeof(PropertyInfo));
                _propTypes = (Type[])listType.ToArray(typeof(Type));
                listProp.Clear();
                listProp = null;
                listType.Clear();
                listType = null;
            }

            #endregion
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Target Type</param>
        /// <param name="properties">Properties that used for Constructor parameters</param>
        public CoreTypeConverter(Type type, string[] properties)
            : this()
        {
        }

        #endregion

        #region abstract method

        #region Protected Abstract Property

        /// <summary>
        /// Get Object Type or Class Type
        /// </summary>
        [Browsable(false)]
        protected abstract Type ObjectType { get; }
        /// <summary>
        /// Get Properties that used for Constructor Parameters
        /// </summary>
        [Browsable(false)]
        protected abstract string[] Properties { get; }

        #endregion

        #endregion

        #region Protected Access

        /// <summary>
        /// Get Constructor Parameter Types
        /// </summary>
        protected Type[] ConstructorParameterTypes { get { return _propTypes; } }

        #endregion

        #region Overrides

        #region ConvertToInstanceDescriptor

        /// <summary>
        /// CanConvertToInstanceDescriptor override to handle ConvertToInstanceDescriptor method
        /// by default this method is return false
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">value to check</param>
        /// <returns>true if ConvertToInstanceDescriptor is implement.
        /// by default this method is return false</returns>
        protected override bool CanConvertToInstanceDescriptor(CultureInfo culture, object value)
        {
            return true;
        }
        /// <summary>
        /// ConvertToInstanceDescriptor override to implement how to create new instance
        /// descriptor by specificed object instance. to create new instance descriptor
        /// we need to specificed proper constructor with types and values that match which
        /// the constructor.
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">value to convert</param>
        /// <returns>return new InstanceDescriptor instance. by default this method will
        /// return null. see CreateInstanceDescriptor for helper method that will help to
        /// create new InstanceDescriptor</returns>
        protected override InstanceDescriptor ConvertToInstanceDescriptor(CultureInfo culture, object value)
        {
            if (value != null && _type != null && value.GetType() == _type)
            {
                Type[] types = null;
                object[] values = null;
                if (_propInfos != null && _propTypes != null &&
                    _propTypes.Length == _propInfos.Length)
                {
                    types = new Type[_propTypes.Length];
                    values = new object[_propTypes.Length];
                    int i = 0;
                    foreach (PropertyInfo propInfo in _propInfos)
                    {
                        try
                        {
                            #region Fixed Constructor that used Object's Array

                            // Fixed Constructor that used Object's Array as
                            // parameter but proeprty is inhetired from 
                            // CollectionBase. Developer should implement
                            // ToArray method to make reflection code work
                            types[i] = Type.GetType(_propTypes[i].FullName);
                            if (types[i].IsSubclassOf(typeof(CollectionBase)))
                            {
                                MethodInfo method = types[i].GetMethod("ToArray",
                                    BindingFlags.Instance | BindingFlags.Public);
                                if (method != null)
                                {
                                    try
                                    {
                                        object cols = propInfo.GetValue(value, Type.EmptyTypes);
                                        if (cols != null)
                                        {
                                            values[i] = method.Invoke(cols, Type.EmptyTypes);
                                            types[i] = method.ReturnType; // change type for constructor
                                        }
                                        else values[i] = null;
                                    }
                                    catch (Exception ex)
                                    {
                                        throw (ex);
                                    }
                                }
                                else values[i] = null;
                            }
                            else
                            {
                                values[i] = propInfo.GetValue(value, Type.EmptyTypes);
                            }

                            #endregion
                        }
                        catch (Exception)
                        {
                            //LogManager.Error(ex);
                            values[i] = null;
                        }
                        i++;
                    }
                }
                else
                {
                    types = Type.EmptyTypes;
                    values = new object[] { };
                }
                return CreateInstanceDescriptor(value.GetType(), types, values);
            }
            return base.ConvertToInstanceDescriptor(culture, value); // call base
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Core Expandable Type Converter. All Expandable Typeconverter should inherited from this class
    /// </summary>
    public abstract class CoreExpandableTypeConverter : AbstractExpandableTypeConverter
    {
        #region Internal Variable

        private Type _type = null;
        private string[] _properties = new string[] { };

        private BindingFlags _bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private PropertyInfo[] _propInfos = new PropertyInfo[] { };
        private Type[] _propTypes = new Type[] { };

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor (protected)
        /// </summary>
        protected CoreExpandableTypeConverter()
            : base()
        {
            _type = ObjectType;
            _properties = Properties;

            #region Reflect Informations

            if (_type != null && _properties != null && _properties.Length > 0)
            {
                PropertyInfo info;
                ArrayList listProp = new ArrayList();
                ArrayList listType = new ArrayList();
                foreach (string property in _properties)
                {
                    try
                    {
                        info = _type.GetProperty(property, _bindFlags);
                    }
                    catch (Exception)
                    {
                        //LogManager.Error(ex);
                        info = null;
                    }
                    if (info != null)
                    {
                        listProp.Add(info);
                        listType.Add(info.PropertyType);
                    }
                }
                _propInfos = (PropertyInfo[])listProp.ToArray(typeof(PropertyInfo));
                _propTypes = (Type[])listType.ToArray(typeof(Type));
                listProp.Clear();
                listProp = null;
                listType.Clear();
                listType = null;
            }

            #endregion
        }

        #endregion

        #region abstract method

        #region Protected Abstract Property

        /// <summary>
        /// Get Object Type or Class Type
        /// </summary>
        [Browsable(false)]
        protected abstract Type ObjectType { get; }
        /// <summary>
        /// Get Properties that used for Constructor Parameters
        /// </summary>
        [Browsable(false)]
        protected abstract string[] Properties { get; }

        #endregion

        #endregion

        #region Protected Access

        /// <summary>
        /// Get Constructor Parameter Types
        /// </summary>
        protected Type[] ConstructorParameterTypes { get { return _propTypes; } }

        #endregion

        #region Overrides

        #region ConvertToInstanceDescriptor

        /// <summary>
        /// CanConvertToInstanceDescriptor override to handle ConvertToInstanceDescriptor method
        /// by default this method is return false
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">value to check</param>
        /// <returns>true if ConvertToInstanceDescriptor is implement.
        /// by default this method is return false</returns>
        protected override bool CanConvertToInstanceDescriptor(CultureInfo culture, object value)
        {
            return true;
        }
        /// <summary>
        /// ConvertToInstanceDescriptor override to implement how to create new instance
        /// descriptor by specificed object instance. to create new instance descriptor
        /// we need to specificed proper constructor with types and values that match which
        /// the constructor.
        /// </summary>
        /// <param name="culture">culture information</param>
        /// <param name="value">value to convert</param>
        /// <returns>return new InstanceDescriptor instance. by default this method will
        /// return null. see CreateInstanceDescriptor for helper method that will help to
        /// create new InstanceDescriptor</returns>
        protected override InstanceDescriptor ConvertToInstanceDescriptor(CultureInfo culture, object value)
        {
            if (value != null && _type != null && value.GetType() == _type)
            {
                Type[] types = null;
                object[] values = null;
                if (_propInfos != null && _propTypes != null &&
                    _propTypes.Length == _propInfos.Length)
                {
                    types = new Type[_propTypes.Length];
                    values = new object[_propTypes.Length];
                    int i = 0;
                    foreach (PropertyInfo propInfo in _propInfos)
                    {
                        try
                        {
                            #region Fixed Constructor that used Object's Array

                            // Fixed Constructor that used Object's Array as
                            // parameter but proeprty is inhetired from 
                            // CollectionBase. Developer should implement
                            // ToArray method to make reflection code work

                            types[i] = Type.GetType(_propTypes[i].FullName);
                            if (types[i].IsSubclassOf(typeof(CollectionBase)))
                            {
                                MethodInfo method = types[i].GetMethod("ToArray",
                                    BindingFlags.Instance | BindingFlags.Public);
                                if (method != null)
                                {
                                    try
                                    {
                                        object cols = propInfo.GetValue(value, Type.EmptyTypes);
                                        if (cols != null)
                                        {
                                            values[i] = method.Invoke(cols, Type.EmptyTypes);
                                            types[i] = method.ReturnType; // change type for constructor
                                        }
                                        else values[i] = null;
                                    }
                                    catch (Exception ex)
                                    {
                                        //LogManager.Error(ex);
                                        throw (ex);
                                    }
                                }
                                else values[i] = null;
                            }
                            else
                            {
                                values[i] = propInfo.GetValue(value, Type.EmptyTypes);
                            }

                            #endregion
                        }
                        catch (Exception)
                        {
                            //LogManager.Error(ex);
                            values[i] = null;
                        }
                        i++;
                    }
                }
                else
                {
                    types = Type.EmptyTypes;
                    values = new object[] { };
                }
                return CreateInstanceDescriptor(value.GetType(), types, values);
            }
            return base.ConvertToInstanceDescriptor(culture, value); // call base
        }

        #endregion

        #region CreateInstanceFromProperties

        /// <summary>
        /// CreateInstanceFromProperties
        /// </summary>
        /// <param name="propertyValues">List of property's values that need to create
        /// new object instance</param>
        /// <returns>new object instance that need to assigned to owner's object
        /// by default this method will return null</returns>
        protected override object CreateInstanceFromProperties(IDictionary propertyValues)
        {
            if (propertyValues != null && _propTypes != null &&
                _propTypes.Length == _properties.Length)
            {
                Type[] types = null;
                object[] values = null;

                types = new Type[_propTypes.Length];
                values = new object[_propTypes.Length];
                int i = 0;
                foreach (string propName in _properties)
                {
                    try
                    {
                        #region Fixed Constructor that used Object's Array

                        // Fixed Constructor that used Object's Array as
                        // parameter but proeprty is inhetired from 
                        // CollectionBase. Developer should implement
                        // ToArray method to make reflection code work
                        types[i] = Type.GetType(_propTypes[i].FullName);
                        if (types[i].IsSubclassOf(typeof(CollectionBase)))
                        {
                            MethodInfo method = types[i].GetMethod("ToArray",
                                BindingFlags.Instance | BindingFlags.Public);
                            if (method != null)
                            {
                                try
                                {
                                    object cols = propertyValues[propName];
                                    if (cols != null)
                                    {
                                        values[i] = method.Invoke(cols, Type.EmptyTypes);
                                        types[i] = method.ReturnType; // change type for constructor
                                    }
                                    else values[i] = null;
                                }
                                catch (Exception ex)
                                {
                                    //LogManager.Error(ex);
                                    throw (ex);
                                }
                            }
                            else values[i] = null;
                        }
                        else
                        {
                            values[i] = propertyValues[propName];
                        }

                        #endregion
                    }
                    catch
                    {
                        values[i] = null;
                    }
                    i++;
                }

                InstanceDescriptor inst = CreateInstanceDescriptor(_type, types, values);
                if (inst != null) return inst.Invoke();
            }
            return base.CreateInstanceFromProperties(propertyValues);
        }

        #endregion

        #endregion
    }

    #endregion
}
