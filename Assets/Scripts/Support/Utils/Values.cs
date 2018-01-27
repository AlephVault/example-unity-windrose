using System.Collections.Generic;

namespace Support
{
    namespace Utils
    {
        public class Values
        {
            public delegate V DictionaryMergePicker<K, V>(K key, V leftValue, V rightValue);

            public static T Min<T>(T a, T b) where T : struct
            {
                return (Comparer<T>.Default.Compare(a, b) < 0) ? a : b;
            }

            public static T Max<T>(T a, T b) where T : struct
            {
                return (Comparer<T>.Default.Compare(a, b) > 0) ? a : b;
            }

            public static T Clamp<T>(T? min, T value, T? max) where T : struct
            {
                if (min == null && max == null)
                {
                    return value;
                }
                else if (min == null)
                {
                    return Min<T>(value, max.Value);
                }
                else if (max == null)
                {
                    return Max<T>(value, min.Value);
                }
                else if (Comparer<T>.Default.Compare(max.Value, min.Value) < 0)
                {
                    return Clamp<T>(max, value, min);
                }
                return Min<T>(Max<T>(min.Value, value), max.Value);
            }

            public static Dictionary<K, V> merge<K, V>(Dictionary<K, V> left, Dictionary<K, V> right, bool inplace = true, DictionaryMergePicker<K, V> picker = null)
            {
                if (picker == null)
                {
                    picker = delegate (K key, V leftValue, V rightValue) { return rightValue; };
                }
                Dictionary<K, V> destination;
                if (!inplace)
                {
                    destination = new Dictionary<K, V>();
                    foreach (KeyValuePair<K, V> item in left)
                    {
                        destination.Add(item.Key, item.Value);
                    }
                }
                else
                {
                    destination = left;
                }
                foreach (KeyValuePair<K, V> item in right)
                {
                    destination[item.Key] = item.Value;
                }
                return destination;
            }
        }
    }
}