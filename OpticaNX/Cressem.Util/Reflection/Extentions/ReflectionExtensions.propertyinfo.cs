// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensions.propertyinfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;

namespace Cressem.Util.Reflection
{
	/// <summary>
	/// Reflection extension class.
	/// </summary>
	public static partial class ReflectionExtensions
	{
		public static Attribute GetCustomAttributeEx(this PropertyInfo propertyInfo, Type attributeType, bool inherit)
		{
			var attributes = GetCustomAttributesEx(propertyInfo, attributeType, inherit);
			return (attributes.Length > 0) ? attributes[0] : null;
		}

		public static Attribute[] GetCustomAttributesEx(this PropertyInfo propertyInfo, bool inherit)
		{
			Argument.IsNotNull("propertyInfo", propertyInfo);

			return propertyInfo.GetCustomAttributes(inherit).ToAttributeArray();
		}

		public static Attribute[] GetCustomAttributesEx(this PropertyInfo propertyInfo, Type attributeType, bool inherit)
		{
			Argument.IsNotNull("propertyInfo", propertyInfo);
			Argument.IsNotNull("attributeType", attributeType);

			return propertyInfo.GetCustomAttributes(attributeType, inherit).ToAttributeArray();
		}
	}
}
