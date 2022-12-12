// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensions.type.cs" company="Catel development team">
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
		public static Attribute GetCustomAttributeEx(this MethodInfo methodInfo, Type attributeType, bool inherit)
		{
			var attributes = GetCustomAttributesEx(methodInfo, attributeType, inherit);

			return (attributes.Length > 0) ? attributes[0] : null;
		}

		public static Attribute[] GetCustomAttributesEx(this MethodInfo methodInfo, bool inherit)
		{
			Argument.IsNotNull("methodInfo", methodInfo);

			return methodInfo.GetCustomAttributes(inherit).ToAttributeArray();
		}

		public static Attribute[] GetCustomAttributesEx(this MethodInfo methodInfo, Type attributeType, bool inherit)
		{
			Argument.IsNotNull("methodInfo", methodInfo);
			Argument.IsNotNull("attributeType", attributeType);

			return methodInfo.GetCustomAttributes(attributeType, inherit).ToAttributeArray();
		}
	}
}
