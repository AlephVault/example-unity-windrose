namespace NetRose
{
    namespace Worlds
    {
        /// <summary>
        ///   Triggered when trying to create another world instance.
        /// </summary>
        class SingletonException : Exception
        {
            public SingletonException() { }
            public SingletonException(string message) : base(message) { }
            public SingletonException(string message, System.Exception inner) : base(message, inner) { }
        }
    }
}
