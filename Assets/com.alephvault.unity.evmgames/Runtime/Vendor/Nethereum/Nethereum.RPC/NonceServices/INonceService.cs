using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.NonceServices
{
    public interface INonceService
    {
        IClient Client { get; set; }
        Task<HexBigInteger> GetNextNonceAsync();
        Task ResetNonceAsync();
    }
}