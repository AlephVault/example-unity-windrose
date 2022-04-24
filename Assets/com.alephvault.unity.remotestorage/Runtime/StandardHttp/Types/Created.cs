using Newtonsoft.Json;


namespace AlephVault.Unity.RemoteStorage
{
    namespace StandardHttp
    {
        namespace Types
        {
            /// <summary>
            ///   Data of a conflict message.
            /// </summary>
            public class Created
            {
                /// <summary>
                ///   The code of the conflict. Two types of conflict errors
                ///   can occur:
                ///   - "Duplicate Key" ("duplicate-key").
                ///   - "Already Exists" ("already-exists").
                ///   - "Still in Use" ("in-use").
                /// </summary>
                [JsonProperty("id")]
                public string Id;
            }
        }
    }
}