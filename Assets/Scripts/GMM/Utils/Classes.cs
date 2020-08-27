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
        ///   This is an extension class with utility methods for types. Please refers to its members.
        /// </summary>
        public static class Classes
        {
            /// <summary>
            ///   An add-on on <see cref="Type"/> class to check whether another type is the
            ///     same or is a subclass of a base type.
            /// </summary>
            /// <param name="derivedType">The derived type to check.</param>
            /// <param name="baseType">The base type to check against.</param>
            /// <returns>Whether is the same or subclass, or not.</returns>
            public static bool IsSameOrSubclassOf(Type derivedType, Type baseType)
            {
                return baseType == derivedType || derivedType.IsSubclassOf(baseType);
            }

            /// <summary>
            ///   Enumerates all the types that are not generic and are
            ///     defined in all the currently loaded assemblies.
            /// </summary>
            /// <returns>An enumerator of all those types</returns>
            public static IEnumerable<Type> GetTypes()
            {
                return from assembly in AppDomain.CurrentDomain.GetAssemblies()
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
                if (!assemblyType.IsGenericType) yield return assemblyType;
                foreach (Type childType in assemblyType.GetNestedTypes())
                {
                    foreach(Type collectedType in CollectTypes(childType))
                    {
                        yield return collectedType;
                    }
                }
            }
        }
    }
}
