using AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.GethEth;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth
{
    public class GethEthApiService : RpcClientWrapper, IGethEthApiService
    {
        public GethEthApiService(IClient client) : base(client)
        {
            PendingTransactions = new EthPendingTransactions(client);
            Call = new EthCall(client);
        }

        public IEthPendingTransactions PendingTransactions { get; }
        public IEthCall Call { get; }
    }
}