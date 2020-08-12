namespace NetRose
{
    namespace Worlds
    {
        /// <summary>
        ///   Triggered when trying to move a non-player across the world.
        /// </summary>
        class NoPlayerException : Exception
        {
            public NoPlayerException() { }
            public NoPlayerException(string message) : base(message) { }
            public NoPlayerException(string message, System.Exception inner) : base(message, inner) { }
        }
    }
}
