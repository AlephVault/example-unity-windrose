using System;
using System.Collections.Generic;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Types
    {
        /// <summary>
        ///   A multi setting is useful to set the values both for a default
        ///   "idle" state and for extra state bundles (with fallbacks).
        /// </summary>
        /// <typeparam name="T">The type to map</typeparam>
        public class MultiSettings<T> : Tuple<T, Dictionary<string, Tuple<T, string>>>
        {
            public MultiSettings(T item1, Dictionary<string, Tuple<T, string>> item2) : base(item1, item2) {}
        }
    }
}