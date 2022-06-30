using Newtonsoft.Json;


namespace AlephVault.Unity.EVMGames.LiveCache
{
    namespace Types
    {
        /// <summary>
        ///   The ownership of a token in a given
        ///   contract.
        /// </summary>
        public class ERC721Balance
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
        }
    }
}