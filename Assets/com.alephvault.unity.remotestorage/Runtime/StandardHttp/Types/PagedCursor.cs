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
            ///   A paged cursor. Aside from a base arguments, it uses a named
            ///   argument: "page".
            /// </summary>
            public class PagedCursor : Cursor
            {
                /// <summary>
                ///   The page to use for this cursor.
                /// </summary>
                public uint Page;
                
                public PagedCursor(Dictionary<string, object> baseArgs = null, uint page = 0) : base(baseArgs)
                {
                    Page = page;
                }

                /// <summary>
                ///   Returns the query string representation of the arguments.
                /// </summary>
                /// <returns>The query string</returns>
                public override string QueryString()
                {
                    if (baseQueryString == "")
                    {
                        return $"page={Page}";
                    }
                    else
                    {
                        return $"{baseQueryString}&page={Page}";
                    }
                }
            }
        }
    }
}