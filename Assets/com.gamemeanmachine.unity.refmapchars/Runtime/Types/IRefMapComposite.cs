namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Types
    {
        /// <summary>
        ///   Provides a way to pick the involved REFMAP-like
        ///   textures, and a related hash of them, so the
        ///   final texture is generated or cache-returned.
        /// </summary>
        public interface IRefMapComposite : IRefMapBaseComposite
        {
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
            ///   The source to use as [long]shirt's arms.
            /// </summary>
            public RefMapSource Arms { get; }
            
            /// <summary>
            ///   The source to use as long shirt / tunic.
            /// </summary>
            public RefMapSource LongShirt { get; }
            
            /// <summary>
            ///   The source to use as shoulders.
            /// </summary>
            public RefMapSource Shoulder { get; }
            
            /// <summary>
            ///   The source to use as cloak.
            /// </summary>
            public RefMapSource Cloak { get; }
            
            /// <summary>
            ///   Whether the boots should be rendered after the pants.
            /// </summary>
            public bool BootsOverPants { get; }

            /// <summary>
            ///   Whether the necklace should be rendered after the long shirt.
            /// </summary>
            public bool NecklaceOverLongShirt { get; }
        }
    }
}
