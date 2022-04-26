using System.Collections.Generic;
using Newtonsoft.Json;


namespace AlephVault.Unity.RemoteStorage
{
    namespace Samples
    {
        public class Version
        {
            [JsonProperty("major")]
            public uint Major;
            
            [JsonProperty("minor")]
            public uint Minor;

            [JsonProperty("revision")]
            public uint Revision;

            public override string ToString()
            {
                return $"{Major}:{Minor}:{Revision}";
            }
        }
    }
}