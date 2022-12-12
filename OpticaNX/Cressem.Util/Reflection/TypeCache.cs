﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCache.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cressem.Util.Caching;

namespace Cressem.Util.Reflection
{
    /// <summary>
    /// Cache containing the types of an appdomain.
    /// </summary>
    public static class TypeCache
    {
        /// <summary>
        /// Cache containing all the types by assembly. This means that the first dictionary contains the assembly name
        /// and all types contained by that assembly.
        /// </summary>
        private static Dictionary<string, Dictionary<string, Type>> _typesByAssembly;

        /// <summary>
        /// Cache containing all the types based on a string. This way, it is easy to retrieve a type based on a 
        /// string containing the type name and assembly without the overhead, such as <c>Catel.TypeHelper, Catel.Core</c>.
        /// </summary>
        private static Dictionary<string, Type> _typesWithAssembly;

        /// <summary>
        /// Cache containing all the types based on a string. This way, it is easy to retrieve a type based on a 
        /// string containing the type name and assembly without the overhead, such as <c>Catel.TypeHelper, Catel.Core</c>.
        /// </summary>
        private static Dictionary<string, Type> _typesWithAssemblyLowerCase;

        /// <summary>
        /// Cache containing all the types based without an assembly. This means that a type with this format:
        /// <c>Catel.TypeHelper, Catel.Core</c> will be located as <c>Catel.TypeHelper</c>.
        /// <para />
        /// The values resolved from this dictionary can be used as key in the <see cref="_typesWithAssembly"/> dictionary.
        /// </summary>
        private static Dictionary<string, string> _typesWithoutAssembly;

        /// <summary>
        /// Cache containing all the types based without an assembly. This means that a type with this format:
        /// <c>Catel.TypeHelper, Catel.Core</c> will be located as <c>Catel.TypeHelper</c>.
        /// <para />
        /// The values resolved from this dictionary can be used as key in the <see cref="_typesWithAssembly"/> dictionary.
        /// </summary>
        private static Dictionary<string, string> _typesWithoutAssemblyLowerCase;

        /// <summary>
        /// The list of loaded assemblies which do not required additional initialization again.
        /// <para />
        /// This is required because the AppDomain.AssemblyLoad might be called several times for the same AppDomain
        /// </summary>
        private static readonly HashSet<string> _loadedAssemblies = new HashSet<string>();

        /// <summary>
        /// The lock object.
        /// </summary>
        private static readonly object _lockObject = new object();

        static TypeCache()
        {
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;

            // Initialize the types of early loaded assemblies.
            lock (_lockObject)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    InitializeTypes(false, assembly);
                }
            }
        }

        /// <summary>
        /// Called when an assembly is loaded in the current <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="AssemblyLoadEventArgs" /> instance containing the event data.</param>
        private static void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            var assembly = args.LoadedAssembly;
            if (assembly.ReflectionOnly)
            {
                return;
            }

            lock (_lockObject)
            {
                var assemblyName = assembly.FullName;
                if (_loadedAssemblies.Contains(assemblyName))
                {
                    return;
                }

                InitializeTypes(false, assembly);

                var handler = AssemblyLoaded;
                if (handler != null)
                {
                    var types = GetTypesOfAssembly(assembly);
                    var eventArgs = new AssemblyLoadedEventArgs(assembly, types);

                    handler(null, eventArgs);
                }

                _loadedAssemblies.Add(assemblyName);
            }
        }

        #region Events

        /// <summary>
        /// Occurs when an assembly is loaded into the currently <see cref="AppDomain"/>.
        /// </summary>
        public static event EventHandler<AssemblyLoadedEventArgs> AssemblyLoaded;
        
		  #endregion

        /// <summary>
        /// Gets the specified type from the loaded assemblies. This is a great way to load types without having
        /// to know the exact version in Silverlight.
        /// </summary>
        /// <param name="typeName">The name of the type including namespace.</param>
        /// <param name="assemblyName">The name of the type including namespace.</param>
        /// <param name="ignoreCase">A value indicating whether the case should be ignored.</param>
        /// <returns>The <see cref="Type"/> or <c>null</c> if the type cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="typeName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="assemblyName"/> is <c>null</c> or whitespace.</exception>
        public static Type GetTypeWithAssembly(string typeName, string assemblyName, bool ignoreCase = false)
        {
            Argument.IsNotNullOrWhitespace("typeName", typeName);
            Argument.IsNotNullOrWhitespace("assemblyName", assemblyName);

            return GetType(typeName, assemblyName, ignoreCase);
        }

        /// <summary>
        /// Gets the type without assembly. For example, when the value <c>Catel.TypeHelper</c> is used as parameter, the type for
        /// <c>Catel.TypeHelper, Catel.Core</c> will be returned.
        /// </summary>
        /// <param name="typeNameWithoutAssembly">The type name without assembly.</param>
        /// <param name="ignoreCase">A value indicating whether the case should be ignored.</param>
        /// <returns>The <see cref="Type"/> or <c>null</c> if the type cannot be found.</returns>
        /// <remarks>
        /// Note that this method can only support one type of "simple type name" resolving. For example, if "Catel.TypeHelper" is located in
        /// multiple assemblies, it will always use the latest known type for resolving the type.
        /// </remarks>
        /// <exception cref="ArgumentException">The <paramref name="typeNameWithoutAssembly"/> is <c>null</c> or whitespace.</exception>
        public static Type GetTypeWithoutAssembly(string typeNameWithoutAssembly, bool ignoreCase = false)
        {
            Argument.IsNotNullOrWhitespace("typeNameWithoutAssembly", typeNameWithoutAssembly);

            return GetType(typeNameWithoutAssembly, null, ignoreCase);
        }

        /// <summary>
        /// Gets the specified type from the loaded assemblies. This is a great way to load types without having
        /// to know the exact version in Silverlight.
        /// </summary>
        /// <param name="typeNameWithAssembly">The name of the type including namespace and assembly, formatted with the <see cref="TypeHelper.FormatType"/> method.</param>
        /// <param name="ignoreCase">A value indicating whether the case should be ignored.</param>
        /// <returns>The <see cref="Type"/> or <c>null</c> if the type cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="typeNameWithAssembly"/> is <c>null</c> or whitespace.</exception>
        public static Type GetType(string typeNameWithAssembly, bool ignoreCase = false)
        {
            Argument.IsNotNullOrWhitespace("typeNameWithAssembly", typeNameWithAssembly);

            var typeName = TypeHelper.GetTypeName(typeNameWithAssembly);
            var assemblyName = TypeHelper.GetAssemblyName(typeNameWithAssembly);

            return GetType(typeName, assemblyName, ignoreCase);
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="assemblyName">Name of the assembly. Can be <c>null</c> if no assembly is known.</param>
        /// <param name="ignoreCase">A value indicating whether the case should be ignored.</param>
        /// <returns>The <see cref="Type"/> or <c>null</c> if the type cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="typeName"/> is <c>null</c> or whitespace.</exception>
        private static Type GetType(string typeName, string assemblyName, bool ignoreCase)
        {
            Argument.IsNotNullOrWhitespace("typeName", typeName);

            InitializeTypes(false);

            lock (_lockObject)
            {
                var typesWithoutAssembly = ignoreCase ? _typesWithoutAssemblyLowerCase : _typesWithoutAssembly;
                var typesWithAssembly = ignoreCase ? _typesWithAssemblyLowerCase : _typesWithAssembly;

                if (ignoreCase)
                {
                    typeName = typeName.ToLowerInvariant();
                }

                var typeNameWithAssembly = string.IsNullOrEmpty(assemblyName) ? null : TypeHelper.FormatType(assemblyName, typeName);
                if (typeNameWithAssembly == null)
                {
                    if (typesWithoutAssembly.ContainsKey(typeName))
                    {
                        return typesWithAssembly[typesWithoutAssembly[typeName]];
                    }

                    // Note that lazy-loaded types (a few lines below) are added to the types *with* assemblies so we have
                    // a direct access cache
                    if (typesWithAssembly.ContainsKey(typeName))
                    {
                        return typesWithAssembly[typeName];
                    }

                    var fallbackType = Type.GetType(typeName);
                    if (fallbackType != null)
                    {
                        // Though it was not initially found, we still have found a new type, register it
                        typesWithAssembly[typeName] = fallbackType;
                    }

                    return fallbackType;
                }

                if (typesWithAssembly.ContainsKey(typeNameWithAssembly))
                {
                    return typesWithAssembly[typeNameWithAssembly];
                }

                // Try to remove version info from assembly info
                var assemblyNameWithoutOverhead = TypeHelper.GetAssemblyNameWithoutOverhead(assemblyName);
                var typeNameWithoutAssemblyOverhead = TypeHelper.FormatType(assemblyNameWithoutOverhead, typeName);
                if (typesWithAssembly.ContainsKey(typeNameWithoutAssemblyOverhead))
                {
                    return typesWithAssembly[typeNameWithoutAssemblyOverhead];
                }

                // Fallback to GetType
                try
                {
                    var type = Type.GetType(typeNameWithAssembly, false, ignoreCase);
                    if (type != null)
                    {
                        typesWithAssembly.Add(typeNameWithAssembly, type);
                        return type;
                    }
                }
                catch (System.IO.FileLoadException)
                {
                }
                catch (Exception)
                {
                }

                // Fallback for this assembly only
                InitializeTypes(false, assemblyName);

                if (typesWithAssembly.ContainsKey(typeNameWithAssembly))
                {
                    return typesWithAssembly[typeNameWithAssembly];
                }

                if (typesWithAssembly.ContainsKey(typeNameWithoutAssemblyOverhead))
                {
                    return typesWithAssembly[typeNameWithoutAssemblyOverhead];
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the types of the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="predicate">The predicate to use on the types.</param>
        /// <returns>All types of the specified assembly.</returns>
        public static Type[] GetTypesOfAssembly(Assembly assembly, Func<Type, bool> predicate = null)
        {
            Argument.IsNotNull("assembly", assembly);

            var assemblyName = TypeHelper.GetAssemblyNameWithoutOverhead(assembly.FullName);
            return GetTypesPrefilteredByAssembly(assemblyName, predicate);
        }

        /// <summary>
        /// Gets all the types from the current <see cref="AppDomain"/> where the <paramref name="predicate"/> returns true.
        /// </summary>
        /// <param name="predicate">The predicate where the type should apply to.</param>
        /// <returns>An array containing all the <see cref="Type"/> that match the predicate.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is <c>null</c>.</exception>
        public static Type[] GetTypes(Func<Type, bool> predicate = null)
        {
            return GetTypesPrefilteredByAssembly(null, predicate);
        }

        /// <summary>
        /// Gets the types prefiltered by assembly. If types must be retrieved from a single assembly only, this method is very fast.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>System.Type[].</returns>
        private static Type[] GetTypesPrefilteredByAssembly(string assemblyName, Func<Type, bool> predicate)
        {
            InitializeTypes(false);

            lock (_lockObject)
            {
                Dictionary<string, Type> typeSource = null;

                if (!string.IsNullOrWhiteSpace(assemblyName))
                {
                    if (_typesByAssembly.ContainsKey(assemblyName))
                    {
                        typeSource = _typesByAssembly[assemblyName];
                    }
                }
                else
                {
                    typeSource = _typesWithAssembly;
                }

                if (typeSource == null)
                {
                    return new Type[] { };
                }

                int retryCount = 3;
                while (retryCount > 0)
                {
                    retryCount--;

                    try
                    {
                        if (predicate != null)
                        {
                            return typeSource.Values.Where(predicate).ToArray();
                        }

                        return typeSource.Values.ToArray();
                    }
                    catch (Exception)
                    {
                    }
                }

                return new Type[] { };
            }
        }

        /// <summary>
        /// Initializes the types in Silverlight. It does this by looping through all loaded assemblies and
        /// registering the type by type name and assembly name.
        /// <para/>
        /// The types initialized by this method are used by <see cref="object.GetType"/>.
        /// </summary>
        /// <param name="forceFullInitialization">If <c>true</c>, the types are initialized, even when the types are already initialized.</param>
        /// <param name="assemblyName">Name of the assembly. If <c>null</c>, all assemblies will be checked.</param>
        /// <exception cref="ArgumentException">The <paramref name="assemblyName"/> is <c>null</c> or whitespace.</exception>
        public static void InitializeTypes(bool forceFullInitialization, string assemblyName)
        {
            Argument.IsNotNullOrWhitespace("assemblyName", assemblyName);

            lock (_lockObject)
            {
                foreach (var assembly in AssemblyHelper.GetLoadedAssemblies())
                {
                    try
                    {
                        if (assembly.FullName.Contains(assemblyName))
                        {
                            InitializeTypes(forceFullInitialization, assembly);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the types in the specified assembly. It does this by looping through all loaded assemblies and
        /// registering the type by type name and assembly name.
        /// <para/>
        /// The types initialized by this method are used by <see cref="object.GetType"/>.
        /// </summary>
        /// <param name="forceFullInitialization">If <c>true</c>, the types are initialized, even when the types are already initialized.</param>
        /// <param name="assembly">The assembly to initialize the types from. If <c>null</c>, all assemblies will be checked.</param>
        public static void InitializeTypes(bool forceFullInitialization, Assembly assembly = null)
        {
            bool checkSingleAssemblyOnly = assembly != null;

            if (!forceFullInitialization && !checkSingleAssemblyOnly && (_typesWithAssembly != null))
            {
                return;
            }

            lock (_lockObject)
            {
                if (forceFullInitialization)
                {
                    _loadedAssemblies.Clear();

                    if (_typesByAssembly != null)
                    {
                        _typesByAssembly.Clear();
                        _typesByAssembly = null;
                    }

                    if (_typesWithAssembly != null)
                    {
                        _typesWithAssembly.Clear();
                        _typesWithAssembly = null;
                    }

                    if (_typesWithAssemblyLowerCase != null)
                    {
                        _typesWithAssemblyLowerCase.Clear();
                        _typesWithAssemblyLowerCase = null;
                    }

                    if (_typesWithoutAssembly != null)
                    {
                        _typesWithoutAssembly.Clear();
                        _typesWithoutAssembly = null;
                    }

                    if (_typesWithoutAssemblyLowerCase != null)
                    {
                        _typesWithoutAssemblyLowerCase.Clear();
                        _typesWithoutAssemblyLowerCase = null;
                    }
                }

                if (_typesByAssembly == null)
                {
                    _typesByAssembly = new Dictionary<string, Dictionary<string, Type>>();
                }

                if (_typesWithAssembly == null)
                {
                    _typesWithAssembly = new Dictionary<string, Type>();
                }

                if (_typesWithAssemblyLowerCase == null)
                {
                    _typesWithAssemblyLowerCase = new Dictionary<string, Type>();
                }

                if (_typesWithoutAssembly == null)
                {
                    _typesWithoutAssembly = new Dictionary<string, string>();
                }

                if (_typesWithoutAssemblyLowerCase == null)
                {
                    _typesWithoutAssemblyLowerCase = new Dictionary<string, string>();
                }

                var typesToAdd = new Dictionary<Assembly, HashSet<Type>>();

                var assembliesToLoad = new List<Assembly>();
                if (assembly != null)
                {
                    assembliesToLoad.Add(assembly);
                }
                else
                {
                    assembliesToLoad.AddRange(AssemblyHelper.GetLoadedAssemblies());
                }

                foreach (var loadedAssembly in assembliesToLoad)
                {
                    var loadedAssemblyFullName = loadedAssembly.FullName;

                    try
                    {
                        if (_loadedAssemblies.Contains(loadedAssemblyFullName))
                        {
                            continue;
                        }

                        typesToAdd[loadedAssembly] = new HashSet<Type>();

                        foreach (var type in AssemblyHelper.GetAllTypesSafely(loadedAssembly))
                        {
                            typesToAdd[loadedAssembly].Add(type);
                        }

                        _loadedAssemblies.Add(loadedAssemblyFullName);
                    }
                    catch (Exception)
                    {
                    }
                }

                foreach (var assemblyWithTypes in typesToAdd)
                {
                    foreach (var type in assemblyWithTypes.Value)
                    {
                        if (ShouldIgnoreType(assemblyWithTypes.Key, type))
                        {
                            continue;
                        }

                        var currentAssembly = assemblyWithTypes.Key;

                        var newAssemblyName = TypeHelper.GetAssemblyNameWithoutOverhead(currentAssembly.FullName);
                        string newFullType = TypeHelper.FormatType(newAssemblyName, type.FullName);

                        if (!_typesByAssembly.ContainsKey(newAssemblyName))
                        {
                            _typesByAssembly[newAssemblyName] = new Dictionary<string, Type>();
                        }

                        var typesByAssembly = _typesByAssembly[newAssemblyName];
                        if (!typesByAssembly.ContainsKey(newFullType))
                        {
                            typesByAssembly[newFullType] = type;

                            _typesWithAssembly[newFullType] = type;
                            _typesWithAssemblyLowerCase[newFullType.ToLowerInvariant()] = type;

                            var typeNameWithoutAssembly = TypeHelper.GetTypeName(newFullType);
                            _typesWithoutAssembly[typeNameWithoutAssembly] = newFullType;
                            _typesWithoutAssemblyLowerCase[typeNameWithoutAssembly.ToLowerInvariant()] = newFullType.ToLowerInvariant();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the specified type must be ignored by the type cache.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the type should be ignored, <c>false</c> otherwise.</returns>
        private static bool ShouldIgnoreType(Assembly assembly, Type type)
        {
            if (type == null)
            {
                return true;
            }

            var typeName = type.FullName;

            // Ignore useless types
            if (typeName.Contains("<PrivateImplementationDetails>") ||
                typeName.Contains("c__DisplayClass") ||
                typeName.Contains("d__") ||
                typeName.Contains("f__AnonymousType") ||
                typeName.Contains("o__") ||
                typeName.Contains("__DynamicallyInvokableAttribute") ||
                typeName.Contains("ProcessedByFody") ||
                typeName.Contains("FXAssembly") ||
                typeName.Contains("ThisAssembly") ||
                typeName.Contains("AssemblyRef") ||
                typeName.Contains("MS.Internal") ||
                typeName.Contains("::") ||
                typeName.Contains("\\*") ||
                typeName.Contains("_extraBytes_") ||
                typeName.Contains("CppImplementationDetails"))
            {
                return true;
            }

            // Ignore types that might cause a security exception
            if (typeName.Contains("System.Data.Metadata.Edm.") ||
                typeName.Contains("System.Data.EntityModel.SchemaObjectModel."))
            {
                return true;
            }

            return false;
        }
    }
}