using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Txpool;
using AlephVault.Unity.EVMGames.Nethereum.RPC;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu
{
    public class TxPoolApiService : RpcClientWrapper, ITxPoolApiService
    {
        public TxPoolApiService(IClient client) : base(client)
        {
            BesuStatistics = new TxpoolBesuStatistics(client);
            BesuTransactions = new TxpoolBesuTransactions(client);
        }

        public ITxpoolBesuStatistics BesuStatistics { get; }
        public ITxpoolBesuTransactions BesuTransactions { get; }
    }
}