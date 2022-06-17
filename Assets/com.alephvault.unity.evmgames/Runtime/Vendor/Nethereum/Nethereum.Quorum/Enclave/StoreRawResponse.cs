using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.Enclave
{

    public class StoreRawResponse
    {
        [JsonProperty(PropertyName =  "key")]
        public string Key { get; set; }
    }
}