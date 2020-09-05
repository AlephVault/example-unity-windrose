using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GMM
{
    namespace Utils
    {
        /// <summary>
        ///   This is an extension class with utility methods for types. Many different
        ///     but class-related utility functions are defined in this class.
        /// </summary>
        public static class Classes
        {
            /// <summary>
            ///   Checks whether another type is the same or is a subclass of a base type.
            /// </summary>
            /// <param name="derivedType">The derived type to check.</param>
            /// <param name="baseType">The base type to check against.</param>
            /// <returns>Whether is the same or subclass, or not.</returns>
            public static bool IsSameOrSubclassOf(Type derivedType, Type baseType)
            {
                return baseType == derivedType || derivedType.IsSubclassOf(baseType);
            }

            /// <summary>
            ///   Checks whether another type is the same or is a subclass of a generic type.
            ///     This is done by unwrapping the generic class on every possible inheritance
            ///     step in the chain until the top of the hierarchy is reached.
            /// </summary>
            /// <param name="genericType">The class to check against - a generic one</param>
            /// <param name="derivedType">The class to check</param>
            /// <returns>Whether that class implements, directly or indirectly, the given generic</returns>
            public static bool IsSubclassOfRawGeneric(Type derivedType, Type genericType)
            {
                while (derivedType != null && derivedType != typeof(object))
                {
                    var cur = derivedType.IsGenericType ? derivedType.GetGenericTypeDefinition() : derivedType;
                    if (genericType == cur)
                    {
                        return true;
                    }
                    derivedType = derivedType.BaseType;
                }
                return false;
            }

            /// <summary>
            ///   Enumerates all the types that are not generic and are
            ///     defined in all the currently loaded assemblies in
            ///     the current application domain.
            /// </summary>
            /// <returns>An enumerator of all those types</returns>
            public static IEnumerable<Type> GetTypes()
            {
                return GetTypes(AppDomain.CurrentDomain.GetAssemblies());
            }

            /// <summary>
            ///   Enumerates all the types that are not generic and are
            ///     defined in the given assemblies.
            /// </summary>
            /// <returns>An enumerator of all those types</returns>
            public static IEnumerable<Type> GetTypes(params Assembly[] assemblies)
            {
                return from assembly in assemblies
                       from collectedType in GetTypes(assembly)
                       select collectedType;
            }

            /// <summary>
            ///   Enumerates all the types that are not generic and are
            ///     defined in the given assembly.
            /// </summary>
            /// <returns>An enumerator of all those types</returns>
            public static IEnumerable<Type> GetTypes(Assembly assembly)
            {
                return from assemblyType in assembly.GetTypes()
                       from collectedType in CollectTypes(assemblyType)
                       select collectedType;
            }

            private static IEnumerable<Type> CollectTypes(Type assemblyType)
            {
                if (assemblyType.IsGenericType) yield break;

                yield return assemblyType;

                foreach (Type childType in assemblyType.GetNestedTypes())
                {
                    foreach (Type collectedType in CollectTypes(childType))
                    {
                        yield return collectedType;
                    }
                }
            }
        }
    }
}
