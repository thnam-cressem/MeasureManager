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
		public static Type[] GetTypesEx(this Assembly assembly)
		{
			Argument.IsNotNull("assembly", assembly);

			return assembly.GetTypes();
		}

		public static Attribute GetCustomAttributeEx(this Assembly assembly, Type attributeType)
		{
			var attributes = GetCustomAttributesEx(assembly, attributeType);

			return (attributes.Length > 0) ? attributes[0] : null;
		}

		public static Attribute[] GetCustomAttributesEx(this Assembly assembly, Type attributeType)
		{
			Argument.IsNotNull("assembly", assembly);
			Argument.IsNotNull("attributeType", attributeType);

			return assembly.GetCustomAttributes(attributeType, true).ToAttributeArray();
		}
	}
}
