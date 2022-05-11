using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Uncles;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Services
{
    public interface IEthApiUncleService
    {
        IEthGetUncleByBlockHashAndIndex GetUncleByBlockHashAndIndex { get; }
        IEthGetUncleByBlockNumberAndIndex GetUncleByBlockNumberAndIndex { get; }
        IEthGetUncleCountByBlockHash GetUncleCountByBlockHash { get; }
        IEthGetUncleCountByBlockNumber GetUncleCountByBlockNumber { get; }
    }
}