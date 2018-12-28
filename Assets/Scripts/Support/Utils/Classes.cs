using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Support
{
    namespace Utils
    {
        public class Classes
        {
            public static bool IsSameOrSubclassOf(Type derivedType, Type baseType)
            {
                return baseType == derivedType || derivedType.IsSubclassOf(baseType);
            }
        }
    }
}
