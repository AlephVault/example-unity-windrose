using AlephVault.Unity.EVMGames.Nethereum.Web3;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity
{
    public interface IWeb3Parity: IWeb3
    {
        IAccountsApiService Accounts { get; }
        IAdminApiService Admin { get; }
        IBlockAuthoringApiService BlockAuthoring { get; }
        INetworkApiService Network { get; }
        ITraceApiService Trace { get; }
    }
}