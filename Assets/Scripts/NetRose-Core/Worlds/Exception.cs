namespace NetRose
{
    namespace Worlds
    {
        /// <summary>
        ///   A base exception class for the World singleton.
        /// </summary>
        class Exception : GMM.Types.Exception
        {
            public Exception() { }
            public Exception(string message) : base(message) { }
            public Exception(string message, System.Exception inner) : base(message, inner) { }
        }
    }
}
