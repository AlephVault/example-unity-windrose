namespace NetRose
{
    namespace Types
    {
        /// <summary>
        ///   A base exception class for all the NetRose package exceptions.
        /// </summary>
        public class Exception : GMM.Types.Exception
        {
            public Exception() { }
            public Exception(string message) : base(message) { }
            public Exception(string message, System.Exception inner) : base(message, inner) { }
        }
    }
}
