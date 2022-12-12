// ------------------------------------------------------------------------------------------------- -------------------
// <copyright file="AssemblyHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cressem.Util.Reflection
{
	/// <summary>
	/// Assembly helper class.
	/// </summary>
	public static class AssemblyHelper
	{
		#region Fields

		private static readonly object _lockObject = new object();
		private static readonly Dictionary<string, string> _assemblyMappings = new Dictionary<string, string>();

		#endregion // Fields

		#region Constructors

		/// <summary>
		/// Initializes static members of the <see cref="AssemblyHelper"/> class.
		/// </summary>
		static AssemblyHelper()
		{
		}

		#endregion // Constructors

		#region Public Mehothds

		/// <summary>
		/// Gets the entry assembly.
		/// </summary>
		/// <returns>Assembly.</returns>
		public static Assembly GetEntryAssembly()
		{
			Assembly assembly = null;

			try
			{
				assembly = Assembly.GetEntryAssembly();
			}
			catch (Exception)
			{
				assembly = typeof(AssemblyHelper).GetAssemblyEx();
			}

			return assembly;
		}

		/// <summary>
		/// Gets the assembly name with version which is currently available in the <see cref="AppDomain" />.
		/// </summary>
		/// <param name="assemblyNameWithoutVersion">The assembly name without version.</param>
		/// <returns>The assembly name with version or <c>null</c> if the assembly is not found in the <see cref="AppDomain"/>.</returns>
		/// <exception cref="ArgumentException">The <paramref name="assemblyNameWithoutVersion" /> is <c>null</c> or whitespace.</exception>
		public static string GetAssemblyNameWithVersion(string assemblyNameWithoutVersion)
		{
			Argument.IsNotNullOrWhitespace("assemblyNameWithoutVersion", assemblyNameWithoutVersion);

			if (assemblyNameWithoutVersion.Contains(", Version="))
			{
				return assemblyNameWithoutVersion;
			}

			lock (_lockObject)
			{
				if (_assemblyMappings.ContainsKey(assemblyNameWithoutVersion))
				{
					return _assemblyMappings[assemblyNameWithoutVersion];
				}

				return null;
			}
		}

		/// <summary>
		/// Gets all types from the assembly safely. Sometimes, the <see cref="ReflectionTypeLoadException"/> is thrown,
		/// and no types are returned. In that case the user must manually get the successfully loaded types from the
		/// <see cref="ReflectionTypeLoadException.Types"/>.
		/// <para/>
		/// This method automatically loads the types. If the <see cref="ReflectionTypeLoadException"/> occurs, this method
		/// will return the types that were loaded successfully.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <param name="logLoaderExceptions">If set to <c>true</c>, the loader exceptions will be logged.</param>
		/// <returns>The array of successfully loaded types.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="assembly"/> is <c>null</c>.</exception>
		public static Type[] GetAllTypesSafely(Assembly assembly, bool logLoaderExceptions = true)
		{
			Argument.IsNotNull("assembly", assembly);

			Type[] foundAssemblyTypes;

			RegisterAssemblyWithVersionInfo(assembly);

			try
			{
				foundAssemblyTypes = assembly.GetTypesEx();
			}
			catch (ReflectionTypeLoadException typeLoadException)
			{
				foundAssemblyTypes = (from type in typeLoadException.Types
											 where type != null
											 select type).ToArray();

				if (logLoaderExceptions)
				{
					foreach (var error in typeLoadException.LoaderExceptions)
					{
					}
				}
			}
			catch (Exception)
			{
				foundAssemblyTypes = new Type[] { };
			}

			return foundAssemblyTypes;
		}

		/// <summary>
		/// Gets the loaded assemblies by using the right method. For Windows applications, it uses
		/// <c>AppDomain.GetAssemblies()</c>. For Silverlight, it uses the assemblies
		/// from the current application.
		/// </summary>
		/// <returns><see cref="List{Assembly}" /> of all loaded assemblies.</returns>
		public static List<Assembly> GetLoadedAssemblies()
		{
			return GetLoadedAssemblies(AppDomain.CurrentDomain);
		}

		/// <summary>
		/// Gets the loaded assemblies by using the right method. For Windows applications, it uses
		/// <c>AppDomain.GetAssemblies()</c>. For Silverlight, it uses the assemblies
		/// from the current application.
		/// </summary>
		/// <param name="appDomain">The app domain to search in.</param>
		/// <returns><see cref="List{Assembly}" /> of all loaded assemblies.</returns>
		public static List<Assembly> GetLoadedAssemblies(AppDomain appDomain)
		{
			var assemblies = new List<Assembly>();

			assemblies.AddRange(appDomain.GetAssemblies());
			// Make sure to prevent duplicates
			assemblies = assemblies.Distinct().ToList();

			// Map all assemblies
			foreach (var assembly in assemblies)
			{
				RegisterAssemblyWithVersionInfo(assembly);
			}

			return assemblies;
		}

		#endregion

		#region Private Methods

		private static void RegisterAssemblyWithVersionInfo(Assembly assembly)
		{
			Argument.IsNotNull("assembly", assembly);

			lock (_lockObject)
			{
				try
				{
					var assemblyNameWithVersion = assembly.FullName;
					var assemblyNameWithoutVersion = TypeHelper.GetAssemblyNameWithoutOverhead(assemblyNameWithVersion);
					_assemblyMappings[assemblyNameWithoutVersion] = assemblyNameWithVersion;
				}
				catch (Exception)
				{
				}
			}
		}

		#endregion
	}
}
