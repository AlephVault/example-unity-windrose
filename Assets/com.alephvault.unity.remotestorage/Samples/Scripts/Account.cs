using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;


namespace AlephVault.Unity.RemoteStorage
{
    namespace Samples
    {
        public class Account
        {
            [JsonProperty("name")]
            public string Name;
            
            [JsonProperty("address")]
            public string Address;

            [JsonProperty("inventory")]
            public Dictionary<string, string> Inventory;

            public override string ToString()
            {
                return $"[{Name} ({Address}) - {InventoryToString()}]";
            }

            public string InventoryToString()
            {
                return string.Join(";",
                    from pair in (Inventory ?? new Dictionary<string, string>())
                    select $"{pair.Key}->{pair.Value}"
                );
            }
        }
    }
}