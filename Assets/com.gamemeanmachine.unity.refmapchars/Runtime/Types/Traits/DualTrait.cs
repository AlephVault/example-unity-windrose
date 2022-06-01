using System;


namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Types
    {
        namespace Traits
        {
            /// <summary>
            ///   A dual trait has a hash and 2 textures: One for front, and
            ///   one for back.
            /// </summary>
            public class DualTrait : Tuple<string, RefMapSource, RefMapSource>
            {
                public DualTrait(string item1, RefMapSource item2, RefMapSource item3) : base(item1, item2, item3) {}

                /// <summary>
                ///   The assigned hash.
                /// </summary>
                public string Hash => Item1;
                
                /// <summary>
                ///   The front texture.
                /// </summary>
                public RefMapSource Front => Item2;

                /// <summary>
                ///   The back texture.
                /// </summary>
                public RefMapSource Back => Item3;
            }
        }
    }
}