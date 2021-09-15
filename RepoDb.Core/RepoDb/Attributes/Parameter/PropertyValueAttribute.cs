﻿using System;
using System.Data;
using System.Reflection;

namespace RepoDb.Attributes.Parameter
{
    /// <summary>
    /// An attribute that is being used to set a value to any property of the <see cref="IDbDataParameter"/> object.
    /// </summary>
    public class PropertyValueAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="PropertyValueAttribute"/> class.
        /// </summary>
        /// <param name="parameterType">The type of the <see cref="IDbDataParameter"/> object.</param>
        /// <param name="propertyName">The name to be set to the parameter.</param>
        /// <param name="value">The value to be set to the parameter.</param>
        public PropertyValueAttribute(Type parameterType,
            string propertyName,
            object value)
            : this(parameterType, propertyName, value, true)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="PropertyValueAttribute"/> class.
        /// </summary>
        /// <param name="parameterType">The type of the <see cref="IDbDataParameter"/> object.</param>
        /// <param name="propertyName">The name to be set to the parameter.</param>
        /// <param name="value">The value to be set to the parameter.</param>
        /// <param name="includedInCompilation">
        /// The value that indicates whether this current attribute method invocation 
        /// will be included on the ahead-of-time (AOT) compilation.
        /// </param>
        internal PropertyValueAttribute(Type parameterType,
            string propertyName,
            object value,
            bool includedInCompilation)
        {
            // Validation
            Validate(parameterType, propertyName);

            // Set the properties
            ParameterType = parameterType;
            PropertyName = propertyName;
            Value = value;
            IncludedInCompilation = includedInCompilation;
        }

        /*
         * Properties
         */

        /// <summary>
        /// Gets the represented <see cref="Type"/> of the <see cref="IDbDataParameter"/> object.
        /// </summary>
        public Type ParameterType { get; }

        /// <summary>
        /// Gets the name of the target property to be set.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the value that is used to set in the parameter.
        /// </summary>
        protected internal object Value { get; }

        /// <summary>
        /// Gets the value that indicates whether this current attribute method invocation 
        /// will be included on the ahead-of-time (AOT) compilation.
        /// </summary>
        protected internal bool IncludedInCompilation { get; }

        /// <summary>
        /// Gets the instance of the <see cref="PropertyInfo"/> based on the target property name.
        /// </summary>
        /// <returns></returns>
        internal PropertyInfo PropertyInfo { get; private set; }

        /*
         * Methods
         */

        /// <summary>
        /// Gets the string representation of the current attribute object.
        /// </summary>
        /// <returns>The represented string.</returns>
        public override string ToString() =>
            $"{ParameterType?.FullName}.{PropertyName} = {Value}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        internal void SetValue(IDbDataParameter parameter)
        {
            ThrowIfNull(parameter, "Parameter");

            if (ParameterType.IsAssignableFrom(parameter.GetType()))
            {
                PropertyInfo.SetValue(parameter, Value);
            }
        }

        /*
         * Helpers
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterType"></param>
        /// <param name="propertyName"></param>
        private void Validate(Type parameterType,
            string propertyName)
        {
            ThrowIfNull(parameterType, "ParameterType");
            ValidateParameterType(parameterType);
            ThrowIfNull(propertyName, "PropertyName");
            EnsurePropertyInfo(parameterType, propertyName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterType"></param>
        private void ValidateParameterType(Type parameterType)
        {
            if (StaticType.IDbDataParameter.IsAssignableFrom(parameterType) == false)
            {
                throw new InvalidOperationException($"The parameter type must be deriving from the '{StaticType.IDbDataParameter.FullName}' interface. " +
                    $"The current passed parameter type is '{parameterType.FullName}'.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterType"></param>
        /// <param name="propertyName"></param>
        private void EnsurePropertyInfo(Type parameterType,
            string propertyName)
        {
            // Property
            PropertyInfo = parameterType?.GetProperty(propertyName);
            ThrowIfNull(PropertyInfo,
                $"The property '{propertyName}' is not found from type '{parameterType?.FullName}'.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        private void ThrowIfNull(object obj,
            string message)
        {
            if (obj == null)
            {
                throw new NullReferenceException(message);
            }
        }
    }
}
