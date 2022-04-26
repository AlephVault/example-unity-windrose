using System.Collections.Generic;
using Newtonsoft.Json;


namespace AlephVault.Unity.RemoteStorage
{
    namespace Samples
    {
        public class Universe
        {
            [JsonProperty("caption")]
            public string Caption;
            
            [JsonProperty("motd")]
            public string MOTD;

            [JsonProperty("version")]
            public Version Version;

            public override string ToString()
            {
                return $"[{Caption}:{Version} - {MOTD}]";
            }
        }
    }
}