using System;


namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Types
    {
        namespace Traits
        {
            /// <summary>
            ///   A simple trait has a hash and one front texture.
            /// </summary>
            public class SimpleTrait : Tuple<string, RefMapSource>
            {
                public SimpleTrait(string hash, RefMapSource front) : base(hash, front) {}
                
                /// <summary>
                ///   The assigned hash.
                /// </summary>
                public string Hash => Item1;
                
                /// <summary>
                ///   The front (and only) texture.
                /// </summary>
                public RefMapSource Front => Item2;
            }
        }
    }
}