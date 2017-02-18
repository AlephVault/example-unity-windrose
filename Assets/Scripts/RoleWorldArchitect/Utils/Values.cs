using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoleWorldArchitect
{
    namespace Utils
    {
        public class Values
        {
            public static T Min<T>(T a, T b)
            {
                return (Comparer<T>.Default.Compare(a, b) < 0) ? a : b;
            }

            public static T Max<T>(T a, T b)
            {
                return (Comparer<T>.Default.Compare(a, b) > 0) ? a : b;
            }

            public static T Clamp<T>(T min, T value, T max)
            {
                if (Comparer<T>.Default.Compare(max, min) < 0)
                {
                    return Clamp<T>(max, value, min);
                }
                return Min<T>(Max<T>(min, value), max);
            }
        }
    }
}