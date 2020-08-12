namespace NetRose
{
    namespace Worlds
    {
        /// <summary>
        ///   Triggered when trying to move a player object to a scene that is not loaded.
        /// </summary>
        class SceneNotLoadedException : Exception
        {
            public SceneNotLoadedException() { }
            public SceneNotLoadedException(string message) : base(message) { }
            public SceneNotLoadedException(string message, System.Exception inner) : base(message, inner) { }
        }
    }
}
