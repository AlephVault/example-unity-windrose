using Newtonsoft.Json;

namespace AlephVault.Unity.RemoteStorage.Input
{
    namespace Samples
    {
        public class MOTDInput
        {
            [JsonProperty("motd")]
            public string MOTD;
        }
    }
}