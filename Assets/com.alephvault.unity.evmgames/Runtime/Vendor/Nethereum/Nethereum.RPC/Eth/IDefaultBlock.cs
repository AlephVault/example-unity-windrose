using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth
{
    public interface IDefaultBlock
    {
        BlockParameter DefaultBlock { get; set; }
    }
}