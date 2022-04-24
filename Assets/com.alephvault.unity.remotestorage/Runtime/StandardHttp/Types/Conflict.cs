namespace AlephVault.Unity.RemoteStorage
{
    namespace StandardHttp
    {
        namespace Types
        {
            /// <summary>
            ///   Data of a conflict message.
            /// </summary>
            public class Conflict
            {
                /// <summary>
                ///   The code of the conflict. Two types of conflict errors
                ///   can occur:
                ///   - "Duplicate Key" ("duplicate-key").
                ///   - "Already Exists" ("already-exists").
                ///   - "Still in Use" ("in-use").
                /// </summary>
                public string Code;
            }
        }
    }
}