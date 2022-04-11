namespace AlephVault.Unity.RemoteStorage
{
    namespace StandardHttp
    {
        namespace Types
        {
            /// <summary>
            ///   An authorization header.
            /// </summary>
            public class Authorization
            {
                /// <summary>
                ///   The authorization scheme.
                /// </summary>
                public readonly string Scheme;
                
                /// <summary>
                ///   The authorization value.
                /// </summary>
                public readonly string Value;

                public Authorization(string scheme, string value)
                {
                    Scheme = scheme;
                    Value = value;
                }
            }
        }
    }
}