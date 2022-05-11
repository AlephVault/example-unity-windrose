using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Services;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Transactions;
using AlephVault.Unity.EVMGames.Nethereum.RPC.TransactionManagers;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC
{
    public interface IEthApiService
    {
        IEthChainId ChainId { get; }
        IEthAccounts Accounts { get; }
        IEthApiBlockService Blocks { get; }
        IEthCoinBase CoinBase { get; }
        IEthApiCompilerService Compile { get; }
        BlockParameter DefaultBlock { get; set; }
        IEthApiFilterService Filters { get; }
        IEthGasPrice GasPrice { get; }
        IEthGetBalance GetBalance { get; }
        IEthGetCode GetCode { get; }
        IEthGetStorageAt GetStorageAt { get; }
        IEthApiMiningService Mining { get; }
        IEthProtocolVersion ProtocolVersion { get; }
        IEthSign Sign { get; }
        IEthSyncing Syncing { get; }
        ITransactionManager TransactionManager { get; set; }
        IEthApiTransactionsService Transactions { get; }
        IEthApiUncleService Uncles { get; }
        IEthFeeHistory FeeHistory { get; }
#if !DOTNET35
        IEtherTransferService GetEtherTransferService();
#endif
    }
}