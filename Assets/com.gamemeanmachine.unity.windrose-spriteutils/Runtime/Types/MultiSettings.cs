using System;
using System.Collections.Generic;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Types
    {
        /// <summary>
        ///   A multi setting is useful to set the values both for a default
        ///   "idle" state and for extra state bundles.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class MultiSettings<T> : Tuple<T, Dictionary<Type, T>>
        {
            public MultiSettings(T item1, Dictionary<Type, T> item2) : base(item1, item2) {}
        }
    }
}