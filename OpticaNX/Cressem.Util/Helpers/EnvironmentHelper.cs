// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnvironmentHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Cressem.Util.Helpers
{
	/// <summary>
	/// Helper class for environment information.
	/// </summary>
	public static class EnvironmentHelper
	{
		private static readonly Lazy<bool> _hostedByVisualStudio = new Lazy<bool>(IsProcessCurrentlyHostedByVisualStudio);
		private static readonly Lazy<bool> _hostedByExpressionBlend = new Lazy<bool>(IsProcessCurrentlyHostedByExpressionBlend);

		/// <summary>
		/// Determines whether the process is hosted by visual studio.
		/// </summary>
		/// <returns><c>true</c> if the process is hosted by visual studio; otherwise, <c>false</c>.</returns>
		public static bool IsProcessHostedByVisualStudio
		{
			get
			{
				// This is required because the logging checks for this when creating the Lazy class
				if (_hostedByVisualStudio == null)
				{
					return false;
				}

				return _hostedByVisualStudio.Value;
			}
		}

		/// <summary>
		/// Determines whether the process is hosted by expression blend.
		/// </summary>
		/// <returns><c>true</c> if the process is hosted by expression blend; otherwise, <c>false</c>.</returns>
		public static bool IsProcessHostedByExpressionBlend
		{
			get
			{
				// This is required because the logging checks for this when creating the Lazy class
				if (_hostedByExpressionBlend == null)
				{
					return false;
				}

				return _hostedByExpressionBlend.Value;
			}
		}

		/// <summary>
		/// Determines whether the process is hosted by any tool, such as visual studio or blend.
		/// </summary>
		/// <returns><c>true</c> if the process is hosted by any tool, such as visual studio or blend; otherwise, <c>false</c>.</returns>
		public static bool IsProcessHostedByTool
		{
			get { return IsProcessHostedByVisualStudio || IsProcessHostedByExpressionBlend; }
		}

		/// <summary>
		/// Determines whether the process is hosted by visual studio.
		/// <para />
		/// This methods executes the logic every time it is called. To get a cached value, use the 
		/// <see cref="IsProcessHostedByVisualStudio"/> instead.
		/// </summary>
		/// <returns><c>true</c> if the process is hosted by visual studio; otherwise, <c>false</c>.</returns>
		public static bool IsProcessCurrentlyHostedByVisualStudio()
		{
			return Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Determines whether the process is hosted by expression blend.
		/// <para />
		/// This methods executes the logic every time it is called. To get a cached value, use the 
		/// <see cref="IsProcessHostedByExpressionBlend"/> instead.
		/// </summary>
		/// <returns><c>true</c> if the process is hosted by expression blend; otherwise, <c>false</c>.</returns>
		public static bool IsProcessCurrentlyHostedByExpressionBlend()
		{
			return Process.GetCurrentProcess().ProcessName.StartsWith("blend", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Determines whether the process is hosted by any tool, such as visual studio or blend.
		/// <para />
		/// This methods executes the logic every time it is called. To get a cached value, use the 
		/// <see cref="IsProcessHostedByTool"/> instead.
		/// </summary>
		/// <returns><c>true</c> if the current process is hosted by any tool; otherwise, <c>false</c>.</returns>
		public static bool IsProcessCurrentlyHostedByTool()
		{
			if (IsProcessCurrentlyHostedByVisualStudio())
			{
				return true;
			}

			if (IsProcessCurrentlyHostedByExpressionBlend())
			{
				return true;
			}

			return false;
		}
	}
}
