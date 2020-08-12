namespace NetRose
{
    namespace Worlds
    {
        /// <summary>
        ///   Triggered when trying to move a player object with inactive connection.
        /// </summary>
        class InactiveConnectionException : Exception
        {
            public InactiveConnectionException() { }
            public InactiveConnectionException(string message) : base(message) { }
            public InactiveConnectionException(string message, System.Exception inner) : base(message, inner) { }
        }
    }
}
