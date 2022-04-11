using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace AlephVault.Unity.RemoteStorage
{
    namespace StandardHttp
    {
        namespace Types
        {
            /// <summary>
            ///   A cursor. For these HTTP standards, it is nothing more
            ///   than a serializable arguments list to be converted to
            ///   query string.
            /// </summary>
            public class Cursor
            {
                // The base arguments to use.
                protected readonly string baseQueryString;
                
                public Cursor(Dictionary<string, object> baseArgs = null)
                {
                    Dictionary<string, object> baseArguments = baseArgs ?? new Dictionary<string, object>();
                    baseQueryString = string.Join("&",
                        from arg in baseArguments
                        select $"{HttpUtility.UrlEncode(arg.Key)}={HttpUtility.UrlEncode(arg.Value.ToString())}"
                    );
                }

                /// <summary>
                ///   Returns the query string representation of the arguments.
                /// </summary>
                /// <returns>The query string</returns>
                public virtual string QueryString()
                {
                    return baseQueryString;
                }
            }
        }
    }
}