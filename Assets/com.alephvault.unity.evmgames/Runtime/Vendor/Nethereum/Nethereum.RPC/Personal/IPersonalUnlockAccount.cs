using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Personal
{
    public interface IPersonalUnlockAccount
    {
        RpcRequest BuildRequest(string address, string passPhrase, int? durationInSeconds, object id = null);
#if !DOTNET35
        Task<bool> SendRequestAsync(EthCoinBase coinbaseRequest, string passPhrase, object id = null);
#endif
        Task<bool> SendRequestAsync(string address, string passPhrase, HexBigInteger durationInSeconds, object id = null);
        Task<bool> SendRequestAsync(string address, string passPhrase, ulong? durationInSeconds, object id = null);
    }
}