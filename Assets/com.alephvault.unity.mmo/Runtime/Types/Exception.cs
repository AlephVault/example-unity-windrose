namespace AlephVault.Unity.MMO
{
    namespace Types
    {
        /// <summary>
        ///   A base exception class for these network-related
        ///   exceptions (e.g. wrong execution context). Both
        ///   the code and message will be used as a logout
        ///   response.
        /// </summary>
        public class Exception : Support.Types.Exception
        {
            public Exception() { }
            public Exception(string message) : base(message) { }
            public Exception(string message, System.Exception inner) : base(message, inner) { }
        }
    }
}
