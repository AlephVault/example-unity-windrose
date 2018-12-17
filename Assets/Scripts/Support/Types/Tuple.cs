using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Support.Types
{
    public class Tuple<A, B>
    {
        public readonly A First;
        public readonly B Second;

        public Tuple(A first, B second)
        {
            First = first;
            Second = second;
        }
    }

    public class Tuple<A, B, C> : Tuple<A, B>
    {
        public readonly C Third;

        public Tuple(A first, B second, C third) : base(first, second)
        {
            Third = third;
        }
    }

    public class Tuple<A, B, C, D> : Tuple<A, B, C>
    {
        public readonly D Fourth;

        public Tuple(A first, B second, C third, D fourth) : base(first, second, third)
        {
            Fourth = fourth;
        }
    }
}
