using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Miner
{
    public interface IMinerSetGasPrice
    {
        RpcRequest BuildRequest(HexBigInteger price, object id = null);
        Task<bool> SendRequestAsync(HexBigInteger price, object id = null);
    }
}