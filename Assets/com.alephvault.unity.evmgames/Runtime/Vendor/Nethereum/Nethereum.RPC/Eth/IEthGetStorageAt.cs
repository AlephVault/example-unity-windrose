using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth
{
    public interface IEthGetStorageAt
    {
        BlockParameter DefaultBlock { get; set; }

        RpcRequest BuildRequest(string address, HexBigInteger position, BlockParameter block, object id = null);
        Task<string> SendRequestAsync(string address, HexBigInteger position, object id = null);
        Task<string> SendRequestAsync(string address, HexBigInteger position, BlockParameter block, object id = null);
    }
}