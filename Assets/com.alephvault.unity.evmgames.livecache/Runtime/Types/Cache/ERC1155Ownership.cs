using Newtonsoft.Json;


namespace AlephVault.Unity.EVMGames.LiveCache
{
    namespace Types
    {
        /// <summary>
        ///   A balance entry for a given contract,
        ///   an address and the token this balance
        ///   is about.
        /// </summary>
        public class ERC1155Ownership
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
            ///   The token.
            /// </summary>
            [JsonProperty("token")]
            public string Token;

            /// <summary>
            ///   The amount.
            /// </summary>
            [JsonProperty("amount")]
            public string Amount;
        }
    }
}