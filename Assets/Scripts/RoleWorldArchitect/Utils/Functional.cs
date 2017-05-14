using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoleWorldArchitect.Utils
{
    class Functional
    {
        public static U[] Filter<U>(U[] source, Func<U, bool> predicate)
        {
            List<U> result = new List<U>();
            for(int i=0; i<source.Length; i++)
            {
                if (predicate(source[i])) result.Add(source[i]);
            }
            return result.ToArray();
        }

        public static V[] Map<U, V>(U[] source, Func<U, V> mapper)
        {
            V[] target = new V[source.Length];
            for(int i=0; i<source.Length; i++)
            {
                target[i] = mapper(source[i]);
            }
            return target;
        }

        public static U Inject<U>(U initial, U[] source, Func<U, U, U> reducer)
        {
            if (source.Length == 0) { return default(U); }
            if (source.Length == 1) { return source[0]; }
            U result = initial;
            for(int i=0; i<source.Length; i++)
            {
                result = reducer(result, source[i]);
            }
            return result;
        }
    }
}
