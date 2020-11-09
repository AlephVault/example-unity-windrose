namespace GMM
{
    namespace Types
    {
        /// <summary>
        ///   An exception class for singletons.
        /// </summary>
        public class SingletonException : Exception
        {
            public SingletonException() { }
            public SingletonException(string message) : base(message) { }
            public SingletonException(string message, System.Exception inner) : base(message, inner) { }
        }
    }
}