namespace AlephVault.Unity.EVMGames.LiveCache
{
    namespace Types
    {
        /// <summary>
        ///   A LiveCache exception. It accounts for the
        ///   context where it occurred and for the http
        ///   code it is related to, if any.
        /// </summary>
        public class Exception : System.Exception
        {
            /// <summary>
            ///   The underlying http code, if any.
            /// </summary>
            public readonly int HttpCode;

            /// <summary>
            ///   The context where this error occurred.
            ///   This is the method (e.g. "grab") where
            ///   this error occurred.
            /// </summary>
            public readonly string Context;

            public Exception(int httpCode, string context) : base()
            {
                HttpCode = httpCode;
                Context = context;
            }

            public Exception(int httpCode, string context, string message) : base(message)
            {
                HttpCode = httpCode;
                Context = context;
            }

            public Exception(int httpCode, string context, string message, System.Exception cause) : base(message,
                cause)
            {
                HttpCode = httpCode;
                Context = context;
            }
        }
    }
}
