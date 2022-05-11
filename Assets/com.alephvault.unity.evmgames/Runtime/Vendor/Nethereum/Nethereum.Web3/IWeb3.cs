using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.Services;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.Services;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC;
using AlephVault.Unity.EVMGames.Nethereum.RPC.TransactionManagers;

namespace AlephVault.Unity.EVMGames.Nethereum.Web3
{
    public interface IWeb3
    {
        IClient Client { get; }
        IEthApiContractService Eth { get; }
        IBlockchainProcessingService Processing { get; }
        INetApiService Net { get; }
        IPersonalApiService Personal { get; }
        IShhApiService Shh { get; }
        ITransactionManager TransactionManager { get; set; }
    }
}