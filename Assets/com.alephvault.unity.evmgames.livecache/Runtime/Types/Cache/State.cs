using System.Collections.Generic;
using Newtonsoft.Json;


namespace AlephVault.Unity.EVMGames.LiveCache
{
    namespace Types
    {
        namespace Cache
        {
            /// <summary>
            ///   The state has only one field: its "value".
            ///   That field is a string => string dictionary.
            /// </summary>
            public class State
            {
                [JsonProperty("value")]
                public Dictionary<string, string> Value;
            }
        }
    }
}
