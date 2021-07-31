namespace AlephVault.Unity.MMO
{
    namespace Types
    {
        namespace Realms
        {
            /// <summary>
            ///   A base exception class for these realm-related
            ///   exceptions (e.g. wrong id type or missing account
            ///   data). Both the code and message will be used as
            ///   a logout response.
            /// </summary>
            public class Exception : Types.Exception
            {
                public Exception() { }
                public Exception(string message) : base(message) { }
                public Exception(string message, System.Exception inner) : base(message, inner) { }
            }
        }
    }
}
