namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Types
    {
        /// <summary>
        ///   Provides a way to pick the involved REFMAP-like
        ///   textures, and a related hash of them, so the
        ///   final texture is generated or cache-returned.
        ///   This mode uses only one texture for full clothes,
        ///   and the necklace, if any, goes on top of the full
        ///   clothes.
        /// </summary>
        public interface IRefMapSimpleComposite
        {
            /// <summary>
            ///   The source to use as body.
            /// </summary>
            public RefMapSource Body { get; }
            
            /// <summary>
            ///   The source to use as hair.
            /// </summary>
            public RefMapSource Hair { get; }
            
            /// <summary>
            ///   The source to use as hair tail (few hair
            ///   styles make use of this).
            /// </summary>
            public RefMapSource HairTail { get; }

            /// <summary>
            ///   The full cloth to wear.
            /// </summary>
            public RefMapSource Cloth { get; }
            
            /// <summary>
            ///   The source to use as necklace. Always on
            ///   top of the clothes.
            /// </summary>
            public RefMapSource Necklace { get; }
            
            /// <summary>
            ///   The source to use as hat.
            /// </summary>
            public RefMapSource Hat { get; }
            
            /// <summary>
            ///   The source to use as item in the skilled hand.
            /// </summary>
            public RefMapSource SkilledHandItem { get; }
            
            /// <summary>
            ///   The source to use as item in the dumb hand.
            /// </summary>
            public RefMapSource DumbHandItem { get; }
            
            /// <summary>
            ///   A digest to use to cache this setting. This should use the
            ///   sha3 function or a similar one with low commissions.
            /// </summary>
            public string Hash();
        }
    }
}
