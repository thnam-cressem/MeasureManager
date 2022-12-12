// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticMemberHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Cressem.Util.Helpers;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Cressem.Util.Reflection
{
	/// <summary>
	/// Helper class for static classes and members.
	/// </summary>
	public static class StaticHelper
	{
		/// <summary>
		/// Gets the type which is calling the current method which might be static. 
		/// </summary>
		/// <returns>The type calling the method.</returns>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Type GetCallingType()
		{
			if (EnvironmentHelper.IsProcessHostedByTool)
			{
				return typeof(object);
			}

			var frame = new StackFrame(2, false);
			var type = frame.GetMethod().DeclaringType;

			return type;
		}
	}
}