using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.EEA;
using AlephVault.Unity.EVMGames.Nethereum.RPC;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu
{
    public class EeaApiService : RpcClientWrapper, IEeaApiService
    {
        public EeaApiService(IClient client) : base(client)
        {
            GetTransactionReceipt = new EeaGetTransactionReceipt(client);
            SendRawTransaction = new EeaSendRawTransaction(client);
        }

        public IEeaGetTransactionReceipt GetTransactionReceipt { get; }
        public IEeaSendRawTransaction SendRawTransaction { get; }
    }
}