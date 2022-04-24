using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace AlephVault.Unity.RemoteStorage
{
    namespace StandardHttp
    {
        namespace Types
        {
            /// <summary>
            ///   Data of a bad request message.
            /// </summary>
            public class BadRequest
            {
                /// <summary>
                ///   The code of the bad request. Several errors
                ///   can occur:
                ///   - Auth: Missing header ("authorization:missing-header").
                ///   - Auth: Bad scheme ("authorization:bad-scheme").
                ///   - Format: Unexpected ("format:unexpected").
                ///   - Validation errors ("schema:invalid").
                /// </summary>
                [JsonProperty("code")]
                public string Code;

                /// <summary>
                ///   For the case of validation errors, this field
                ///   contains the actual validation errors.
                /// </summary>
                [JsonProperty("errors")]
                public JObject ValidationErrors;
            }
        }
    }
}