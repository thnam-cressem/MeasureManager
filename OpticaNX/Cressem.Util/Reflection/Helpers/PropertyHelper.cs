﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Reflection;
using Cressem.Util.Caching;

namespace Cressem.Util.Reflection
{
	/// <summary>
	/// Property helper class.
	/// </summary>
	public static partial class PropertyHelper
	{
		#region Constants

		private static readonly ICacheStorage<string, PropertyInfo> _availableProperties = new CacheStorage<string, PropertyInfo>();

		#endregion

		#region Methods

		/// <summary>
		/// Determines whether the specified property is a public property on the specified object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="property">The property.</param>
		/// <returns><c>true</c> if the property is a public property on the specified object; otherwise, <c>false</c>.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
		public static bool IsPublicProperty(object obj, string property)
		{
			Argument.IsNotNull("obj", obj);
			Argument.IsNotNullOrWhitespace("property", property);

			var propertyInfo = GetPropertyInfo(obj, property);
			if (propertyInfo == null)
			{
				return false;
			}

			var getMethod = propertyInfo.GetGetMethod();
			if (getMethod == null)
			{
				return false;
			}

			return getMethod.IsPublic;
		}

		/// <summary>
		/// Determines whether the specified property is available on the object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="property">The property.</param>
		/// <returns><c>true</c> if the property exists on the object type; otherwise, <c>false</c>.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
		public static bool IsPropertyAvailable(object obj, string property)
		{
			Argument.IsNotNull("obj", obj);
			Argument.IsNotNullOrWhitespace("property", property);

			return GetPropertyInfo(obj, property) != null;
		}

		/// <summary>
		/// Tries to get the property value. If it fails, not exceptions will be thrown but the <paramref name="value" />
		/// is set to a default value and the method will return <c>false</c>.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="property">The property.</param>
		/// <param name="value">The value as output parameter.</param>
		/// <returns><c>true</c> if the method succeeds; otherwise <c>false</c>.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
		public static bool TryGetPropertyValue(object obj, string property, out object value)
		{
			Argument.IsNotNull("obj", obj);
			Argument.IsNotNullOrWhitespace("property", property);

			return TryGetPropertyValue<object>(obj, property, out value);
		}

		/// <summary>
		/// Tries to get the property value. If it fails, not exceptions will be thrown but the <paramref name="value" />
		/// is set to a default value and the method will return <c>false</c>.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="obj">The object.</param>
		/// <param name="property">The property.</param>
		/// <param name="value">The value as output parameter.</param>
		/// <returns><c>true</c> if the method succeeds; otherwise <c>false</c>.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
		public static bool TryGetPropertyValue<TValue>(object obj, string property, out TValue value)
		{
			Argument.IsNotNull("obj", obj);
			Argument.IsNotNullOrWhitespace("property", property);

			TValue defaultValue = default(TValue);

			var propertyInfo = GetPropertyInfo(obj, property);
			if (propertyInfo == null)
			{
				value = defaultValue;
				return false;
			}

			if (!propertyInfo.CanRead)
			{
				value = defaultValue;
				return false;
			}

			try
			{
				value = (TValue)propertyInfo.GetValue(obj, null);
				return true;
			}
			catch (Exception)
			{
				value = defaultValue;
				return false;
			}
		}

		/// <summary>
		/// Gets the property value of a specific object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="property">The property.</param>
		/// <returns>The property value or <c>null</c> if no property can be found.</returns>
		/// <exception cref="PropertyNotFoundException">The <paramref name="obj" /> is not found or not publicly available.</exception>
		/// <exception cref="CannotGetPropertyValueException">The property value cannot be read.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
		public static object GetPropertyValue(object obj, string property)
		{
			Argument.IsNotNull("obj", obj);
			Argument.IsNotNullOrWhitespace("property", property);

			var propertyInfo = GetPropertyInfo(obj, property);
			if (propertyInfo == null)
			{
				throw new PropertyNotFoundException(property);
			}

			// Return property value if available
			if (!propertyInfo.CanRead)
			{
				throw new CannotGetPropertyValueException(property);
			}

			try
			{
				return propertyInfo.GetValue(obj, null);
			}
			catch (MethodAccessException)
			{
				throw new CannotGetPropertyValueException(property);
			}
		}

		/// <summary>
		/// Gets the property value of a specific object.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="obj">The object.</param>
		/// <param name="property">The property.</param>
		/// <returns>The property value or <c>null</c> if no property can be found.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
		/// <exception cref="PropertyNotFoundException">The <paramref name="obj" /> is not found or not publicly available.</exception>
		/// <exception cref="CannotGetPropertyValueException">The property value cannot be read.</exception>
		public static TValue GetPropertyValue<TValue>(object obj, string property)
		{
			return (TValue)GetPropertyValue(obj, property);
		}

		/// <summary>
		/// Tries to set the property value. If it fails, no exceptions will be thrown, but <c>false</c> will
		/// be returned.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="property">The property.</param>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if the method succeeds; otherwise <c>false</c>.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
		public static bool TrySetPropertyValue(object obj, string property, object value)
		{
			Argument.IsNotNull("obj", obj);
			Argument.IsNotNullOrWhitespace("property", property);

			try
			{
				SetPropertyValue(obj, property, value);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Sets the property value of a specific object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="property">The property.</param>
		/// <param name="value">The value.</param>
		/// <exception cref="PropertyNotFoundException">The <paramref name="obj" /> is not found or not publicly available.</exception>
		/// <exception cref="CannotSetPropertyValueException">The the property value cannot be written.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
		public static void SetPropertyValue(object obj, string property, object value)
		{
			Argument.IsNotNull("obj", obj);
			Argument.IsNotNullOrWhitespace("property", property);

			var propertyInfo = GetPropertyInfo(obj, property);
			if (propertyInfo == null)
			{
				throw new PropertyNotFoundException(property);
			}

			if (!propertyInfo.CanWrite)
			{
				throw new CannotSetPropertyValueException(property);
			}

         var setMethod = propertyInfo.GetSetMethod(true);
			if (setMethod == null)
			{
				throw new CannotSetPropertyValueException(property);
			}

			setMethod.Invoke(obj, new[] { value });
		}

		/// <summary>
		/// Gets hidden property value.
		/// </summary>
		/// <typeparam name="TValue">The type of the T value.</typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="property">The property.</param>
		/// <param name="baseType">The base Type.</param>
		/// <returns>``0.</returns>
		/// <exception cref="PropertyNotFoundException"></exception>
		/// <exception cref="System.ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
		/// <exception cref="System.ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
		/// <exception cref="System.ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
		public static TValue GetHiddenPropertyValue<TValue>(object obj, string property, Type baseType)
		{
			Argument.IsNotNull("obj", obj);
			Argument.IsNotNullOrWhitespace("property", property);
			Argument.IsNotNull("baseType", baseType);
			Argument.IsOfType("obj", obj, baseType);

			const BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			PropertyInfo propertyInfo = baseType.GetPropertyEx(property, BindingFlags);
			if (propertyInfo == null)
			{
				throw new PropertyNotFoundException(property);
			}

			return (TValue)propertyInfo.GetValue(obj, BindingFlags, null, new object[] { }, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Gets the property info from the cache.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="property">The property.</param>
		/// <returns>PropertyInfo.</returns>
		private static PropertyInfo GetPropertyInfo(object obj, string property)
		{
			string cacheKey = string.Format("{0}_{1}", obj.GetType().FullName, property);
			return _availableProperties.GetFromCacheOrFetch(cacheKey, () => obj.GetType().GetPropertyEx(property));
		}

		#endregion
	}
}
