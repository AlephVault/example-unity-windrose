using AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Miner;
using AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.TxnPool;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth
{
    public class MinerApiService : RpcClientWrapper, IMinerApiService
    {
        public MinerApiService(IClient client) : base(client)
        {
            SetGasPrice = new MinerSetGasPrice(client);
            Start = new MinerStart(client);
            Stop = new MinerStop(client);
        }

        public IMinerSetGasPrice SetGasPrice { get; }
        public IMinerStart Start { get; }
        public IMinerStop Stop { get; }
    }
}