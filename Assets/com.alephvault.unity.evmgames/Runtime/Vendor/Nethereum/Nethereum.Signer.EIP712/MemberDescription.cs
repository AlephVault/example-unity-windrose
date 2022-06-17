using AlephVault.Unity.EVMGames.Nethereum.ABI;
using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;
using AlephVault.Unity.EVMGames.Nethereum.ABI.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AlephVault.Unity.EVMGames.Nethereum.Signer.EIP712
{
    public class MemberDescription
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}