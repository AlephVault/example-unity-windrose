using System.Collections.Generic;

namespace GMM
{
    namespace Utils
    {
        /// <summary>
        ///   This class acts as a namespace to holds several general-purpose utility methods. Please refer to its methods.
        /// </summary>
        public static class Values
        {
            /// <summary>
            ///   A function used as callback to merge dictionary values when invoking the
            ///     <see cref="Merge{K, V}(Dictionary{K, V}, Dictionary{K, V}, bool, DictionaryMergePicker{K, V})"/>
            ///     method.
            /// </summary>
            /// <typeparam name="K">A dictionary key.</typeparam>
            /// <typeparam name="V">A dictioanry value.</typeparam>
            /// <param name="key">The key being merged.</param>
            /// <param name="leftValue">Value for the key <paramref name="key"/> in the left dictionary.</param>
            /// <param name="rightValue">Value for the key <paramref name="key"/> in the right dictionary.</param>
            /// <returns>The final -merged- value.</returns>
            public delegate V DictionaryMergePicker<K, V>(K key, V leftValue, V rightValue);

            /// <summary>
            ///   Using a default comparer, returns the minimum value between the two values.
            /// </summary>
            /// <typeparam name="T">Type of values to compare. It must be a value type and have a default comparer.</typeparam>
            /// <param name="a">Value to compare.</param>
            /// <param name="b">Value to compare.</param>
            /// <returns>The minimum value.</returns>
            public static T Min<T>(T a, T b) where T : struct
            {
                return (Comparer<T>.Default.Compare(a, b) < 0) ? a : b;
            }

            /// <summary>
            ///   Using a default comparer, returns the maximum value between the two values.
            /// </summary>
            /// <typeparam name="T">Type of values to compare. It must be a value type and have a default comparer.</typeparam>
            /// <param name="a">Value to compare.</param>
            /// <param name="b">Value to compare.</param>
            /// <returns>The maximum value.</returns>
            public static T Max<T>(T a, T b) where T : struct
            {
                return (Comparer<T>.Default.Compare(a, b) > 0) ? a : b;
            }

            /// <summary>
            ///   Using a default comparer, returns a clamped value between the two values, given also a base value.
            /// </summary>
            /// <remarks>This means: if the base value is lower than the minimum value, the minimum value is returned. Analogous behaviour goes for the maximum. Otherwise, the base value is returned.</remarks>
            /// <typeparam name="T">Type of values to compare. It must be a value type and have a default comparer.</typeparam>
            /// <param name="min">Lower bound.</param>
            /// <param name="value">Initial value.</param>
            /// <param name="max">Upper bound.</param>
            /// <returns>The clamped value.</returns>
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

            /// <summary>
            ///   Tells whether a value is clamped between a minimum and a maximum, including them.
            /// </summary>
            /// <typeparam name="T">Type of values to compare. It must be a value type and have a default comparer.</typeparam>
            /// <param name="min">Lower bound.</param>
            /// <param name="value">Initial value.</param>
            /// <param name="max">Upper bound.</param>
            /// <returns>Whether the value is clamped or not.</returns>
            public static bool In<T>(T? min, T value, T? max) where T : struct
            {
                return (min == null || Comparer<T>.Default.Compare(min.Value, value) <= 0) && (max == null || Comparer<T>.Default.Compare(value, max.Value) <= 0);
            }

            /// <summary>
            ///   Merges two dictionaries. Merging implies having a dictionary with all the keys from the left and the right dictionaries. If they have the same
            ///     key, a 
            /// </summary>
            /// <typeparam name="K">A dictionary key.</typeparam>
            /// <typeparam name="V">A dictioanry value.</typeparam>
            /// <param name="left">The left side dictionary.</param>
            /// <param name="right">The right side dictionary.</param>
            /// <param name="inPlace">If <c>true</c> the dictionary being merged is the left. Otherwise, it is a new dictionary having a merge of left and right.</param>
            /// <param name="picker">The merge function. If absent, a function taking the value from the right side will be used.</param>
            /// <returns>The merged dictionary. Will be a new one if <paramref name="inPlace"/> is false - otherwise, the left dictionary will be merged and returned</returns>
            public static Dictionary<K, V> Merge<K, V>(Dictionary<K, V> left, Dictionary<K, V> right, bool inPlace = true, DictionaryMergePicker<K, V> picker = null)
            {
                if (picker == null)
                {
                    picker = delegate (K key, V leftValue, V rightValue) { return rightValue; };
                }
                Dictionary<K, V> destination = inPlace ? left : new Dictionary<K, V>(left);
                foreach (KeyValuePair<K, V> item in right)
                {
                    if (destination.ContainsKey(item.Key))
                    {
                        destination[item.Key] = picker(item.Key, destination[item.Key], item.Value);
                    }
                    else
                    {
                        destination[item.Key] = item.Value;
                    }
                }
                return destination;
            }
        }
    }
}