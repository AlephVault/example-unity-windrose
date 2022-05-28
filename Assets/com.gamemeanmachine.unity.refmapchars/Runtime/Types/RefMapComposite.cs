namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Types
    {
        /// <summary>
        ///   Provides a way to pick the involved REFMAP-like
        ///   textures, and a related hash of them, so the
        ///   final texture is generated or cache-returned.
        /// </summary>
        public interface RefMapComposite
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
            ///   The source to use as boots.
            /// </summary>
            public RefMapSource Boots { get; }
            
            /// <summary>
            ///   The source to use as pants.
            /// </summary>
            public RefMapSource Pants { get; }
            
            /// <summary>
            ///   The source to use as shirt.
            /// </summary>
            public RefMapSource Shirt { get; }
            
            /// <summary>
            ///   The source to use as light armor / vest.
            /// </summary>
            public RefMapSource Chest { get; }
            
            /// <summary>
            ///   The source to use as waist.
            /// </summary>
            public RefMapSource Waist { get; }
            
            /// <summary>
            ///   The source to use as shirt's arms.
            /// </summary>
            public RefMapSource Arms { get; }
            
            /// <summary>
            ///   The source to use as long shirt / tunic.
            /// </summary>
            public RefMapSource LongShirt { get; }
            
            /// <summary>
            ///   The source to use as necklace.
            /// </summary>
            public RefMapSource Necklace { get; }
            
            /// <summary>
            ///   The source to use as shoulders.
            /// </summary>
            public RefMapSource Shoulder { get; }
            
            /// <summary>
            ///   The source to use as cloak.
            /// </summary>
            public RefMapSource Cloak { get; }
            
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
            ///   Whether the boots should be rendered after the pants.
            /// </summary>
            public bool BootsOverPants { get; }

            /// <summary>
            ///   Whether the necklace should be rendered after the long shirt.
            /// </summary>
            public bool NecklaceOverLongShirt { get; }

            /// <summary>
            ///   A digest to use to cache this setting. This should use the
            ///   sha3 function or a similar one with low commissions.
            /// </summary>
            public string Hash();
        }
    }
}
