namespace AlephVault.Unity.MMO
{
    namespace Types
    {
        namespace Authentication
        {
            /// <summary>
            ///   This exception is meant to be triggered when any error occurs
            ///   on account loading, after a successful login. Both the code
            ///   and message will be used as a logout response.
            /// </summary>
            public class LoginAborted : Exception
            {
                public string Code { get; private set; }
                public LoginAborted(string code, string message) : base(message) { Code = code; }
                public LoginAborted(string code, string message, System.Exception inner) : base(message, inner) { Code = code; }
            }
        }
    }
}
