using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC.DTOs
{
    public class PrivateRawTransaction
    {
        public PrivateRawTransaction()
        {
        }

        public PrivateRawTransaction(string[] privateFor)
        {
            PrivateFor = privateFor;
        }

        [JsonProperty(PropertyName =  "privateFor")]
        public string[] PrivateFor { get; set; }
    }
}