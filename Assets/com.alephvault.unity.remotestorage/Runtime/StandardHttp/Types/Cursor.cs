namespace AlephVault.Unity.RemoteStorage
{
    namespace StandardHttp
    {
        namespace Types
        {
            /// <summary>
            ///   A cursor. It implies an offset and a limit.
            /// </summary>
            public class Cursor
            {
                /// <summary>
                ///   The offset to apply.
                /// </summary>
                public ulong Offset;
                
                /// <summary>
                ///   The limit to apply.
                /// </summary>
                public ulong Limit;
                
                public Cursor(ulong offset, ulong limit)
                {
                    Offset = offset;
                    Limit = limit;
                }

                /// <summary>
                ///   Returns the query string representation of the arguments.
                /// </summary>
                /// <returns>The query string</returns>
                public string QueryString()
                {
                    return $"offset={Offset}&limit={Limit}";
                }
            }
        }
    }
}