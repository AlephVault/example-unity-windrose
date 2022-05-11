using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth
{
    public interface IEthGetBalance
    {
        BlockParameter DefaultBlock { get; set; }

        RpcRequest BuildRequest(string address, BlockParameter block, object id = null);
        Task<HexBigInteger> SendRequestAsync(string address, object id = null);
        Task<HexBigInteger> SendRequestAsync(string address, BlockParameter block, object id = null);
    }
}