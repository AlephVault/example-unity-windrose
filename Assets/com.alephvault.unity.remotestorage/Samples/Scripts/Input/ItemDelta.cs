using Newtonsoft.Json;

namespace AlephVault.Unity.RemoteStorage.Input
{
    namespace Samples
    {
        public class ItemDelta
        {
            [JsonProperty("item")]
            public string Item;

            [JsonProperty("by")]
            public string By;
        }
    }
}