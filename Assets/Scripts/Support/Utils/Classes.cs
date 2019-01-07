using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Support
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
        }
    }
}
