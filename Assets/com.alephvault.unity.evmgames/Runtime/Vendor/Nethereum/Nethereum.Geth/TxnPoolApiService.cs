using AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.TxnPool;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth
{
    public class TxnPoolApiService : RpcClientWrapper, ITxnPoolApiService
    {
        public TxnPoolApiService(IClient client) : base(client)
        {
            PoolContent = new TxnPoolContent(client);
            PoolInspect = new TxnPoolInspect(client);
            PoolStatus = new TxnPoolStatus(client);
        }

        public ITxnPoolContent PoolContent { get; }
        public ITxnPoolInspect PoolInspect { get; }
        public ITxnPoolStatus PoolStatus { get; }
    }
}