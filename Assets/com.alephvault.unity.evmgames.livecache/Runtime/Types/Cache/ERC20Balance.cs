using Newtonsoft.Json;


namespace AlephVault.Unity.EVMGames.LiveCache
{
    namespace Types
    {
        /// <summary>
        ///   A balance entry for a given contract,
        ///   and the address of the owner.
        /// </summary>
        public class ERC20Balance
        {
            /// <summary>
            ///   The contract key.
            /// </summary>
            [JsonProperty("contract-key")]
            public string ContractKey;

            /// <summary>
            ///   The owner.
            /// </summary>
            [JsonProperty("owner")]
            public string Owner;
            
            /// <summary>
            ///   The balance.
            /// </summary>
            [JsonProperty("amount")]
            public string Amount;
        }
    }
}