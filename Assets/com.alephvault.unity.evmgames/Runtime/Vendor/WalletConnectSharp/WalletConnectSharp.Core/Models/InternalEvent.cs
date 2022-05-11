using Newtonsoft.Json;

namespace AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Models
{
    public class InternalEvent
    {
        [JsonProperty("event")]
        public string @event;
    }
}