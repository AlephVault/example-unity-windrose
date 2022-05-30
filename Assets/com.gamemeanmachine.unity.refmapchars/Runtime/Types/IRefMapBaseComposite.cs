namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Types
    {
        /// <summary>
        ///   Provides a way to pick the some involved REFMAP-like
        ///   textures for some common parts (body, hair, hair tail
        ///   necklace, hat, and hand tools), and a way to compute
        ///   a hash of the current content.
        /// </summary>
        public interface IRefMapBaseComposite
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
            ///   The source to use as necklace.
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
            ///   A digest to use to cache this setting. This should
            ///   use the sha3 function or a similar one with low
            ///   collisions.
            /// </summary>
            public string Hash();
        }
    }
}
