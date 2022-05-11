using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs
{
    [DataContract]
    public class AccessList
    {
        [DataMember(Name = "address")]
        public string Address { get; set; }
        [DataMember(Name = "storageKeys")]
        public List<string> StorageKeys { get; set; }
    }
}