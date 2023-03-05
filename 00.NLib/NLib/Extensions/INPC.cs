#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-12-12
=================
- NLib INotifyPropertyChanged Extension Methods.
  - Add INotifyPropertyChanged Extension Methods class.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace NLib
{
    /// <summary>
    /// The INotifyPropertyChanged Extension Methods.
    /// </summary>
    public static class INotifyPropertyChangedExtensionMethods
    {
        #region Public Methods
        
        /// <summary>
        /// Raise PropertyChanged event.
        /// Note. When used INotifyPropertyChanged in real time environment or in environment that
        /// properties is highly changed. This would cause performance penalty that when bind with UI
        /// the UI control(s) will slow down for sure.
        /// </summary>
        /// <typeparam name="TSender">The sender type.</typeparam>
        /// <typeparam name="TPropertyValue">The property value type.</typeparam>
        /// <param name="handler">The PropertyChanged delegate.</param>
        /// <param name="sender">The sender or owner of PropertyChanged delegate.</param>
        /// <param name="property">The Property instance expression.</param>
        public static void Raise<TSender, TPropertyValue>(
            this PropertyChangedEventHandler handler, TSender sender,
            Expression<Func<TSender, TPropertyValue>> property)
        {
            if (null == handler)
                return;

            RaiseImpl(handler, sender, property);
        }
        /// <summary>
        /// Raise PropertyChanged event.
        /// Note. When used INotifyPropertyChanged in real time environment or in environment that
        /// properties is highly changed. This would cause performance penalty that when bind with UI
        /// the UI control(s) will slow down for sure.
        /// </summary>
        /// <typeparam name="TPropertyValue">The property value type.</typeparam>
        /// <param name="handler">The PropertyChanged delegate.</param>
        /// <param name="sender">The sender or owner of PropertyChanged delegate.</param>
        /// <param name="property">The Property instance expression.</param>
        public static void Raise<TPropertyValue>(
            this PropertyChangedEventHandler handler, object sender,
            Expression<Func<TPropertyValue>> property)
        {
            if (null == handler)
                return;

            RaiseImpl(handler, sender, property);
        }

        #endregion

        #region Private Methods

        private static void RaiseImpl<TSender>(PropertyChangedEventHandler handler,
            TSender sender, Expression property)
        {
            if (null == sender)
                throw new ArgumentNullException("sender");

            var lamda = ThrowOnNull(property, property as LambdaExpression);
            var call = ThrowOnNull(property, lamda.Body as MemberExpression);
            CheckIsProperty(property, call);
            var nameToFire = call.Member.Name;

            // Call event by delegate invoker via ApplicationManager.
            handler.Call(sender, new PropertyChangedEventArgs(nameToFire));

            // Call event direct (original).
            // handler(sender, new PropertyChangedEventArgs(nameToFire));
        }

        private static void CheckIsProperty(Expression property, MemberExpression call)
        {
            if (MemberTypes.Property != call.Member.MemberType)
            {
                throw new ArgumentException("Require a property-access, but is :" + property);
            }
        }

        private static T ThrowOnNull<T>(Expression property, T lamda) where T : class
        {
            if (null == lamda)
            {
                throw new ArgumentException("Require a property-access, but is :" + property);
            }

            return lamda;
        }

        #endregion
    }
}
