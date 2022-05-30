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
        public interface IRefMapSimpleComposite : IRefMapBaseComposite
        {
            /// <summary>
            ///   The full cloth to wear.
            /// </summary>
            public RefMapSource Cloth { get; }
        }
    }
}
