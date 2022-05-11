using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Mining;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Services
{
    public interface IEthApiMiningService
    {
        IEthGetWork GetWork { get; }
        IEthHashrate Hashrate { get; }
        IEthMining IsMining { get; }
        IEthSubmitHashrate SubmitHashrate { get; }
        IEthSubmitWork SubmitWork { get; }
    }
}