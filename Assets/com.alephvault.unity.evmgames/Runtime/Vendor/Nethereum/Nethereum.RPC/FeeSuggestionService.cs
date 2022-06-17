using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Fee1559Suggestions;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC
{
    public class FeeSuggestionService : RpcClientWrapper
    {
        public FeeSuggestionService(IClient client) : base(client)
        {
        }

#if !DOTNET35

        public SimpleFeeSuggestionStrategy GetSimpleFeeSuggestionStrategy()
        {
            return new SimpleFeeSuggestionStrategy(Client);
        }

        public TimePreferenceFeeSuggestionStrategy GetTimePreferenceFeeSuggestionStrategy()
        {
            return new TimePreferenceFeeSuggestionStrategy(Client);
        }

        public MedianPriorityFeeHistorySuggestionStrategy GetMedianPriorityFeeHistorySuggestionStrategy()
        {
            return new MedianPriorityFeeHistorySuggestionStrategy(Client);
        }
#endif
    }
}