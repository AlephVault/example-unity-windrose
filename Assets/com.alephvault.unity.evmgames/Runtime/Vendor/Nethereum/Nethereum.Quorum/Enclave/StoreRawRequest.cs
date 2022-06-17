﻿using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.Enclave
{

    public class StoreRawRequest
    {
        [JsonProperty(PropertyName =  "payload")]
        public string Payload { get; set; }
        [JsonProperty(PropertyName =  "from")]
        public string From { get; set; }
    }
}