namespace GMM
{
    namespace Types
    {
        /// <summary>
        ///   Base class for exceptions in this package.
        /// </summary>
        public class Exception : System.Exception
        {
            public Exception() { }
            public Exception(string message) : base(message) { }
            public Exception(string message, System.Exception inner) : base(message, inner) { }
        }
    }
}