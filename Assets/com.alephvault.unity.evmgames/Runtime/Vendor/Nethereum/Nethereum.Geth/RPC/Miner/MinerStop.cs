using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Miner
{
    /// <Summary>
    ///     Stop the CPU mining operation.
    /// </Summary>
    public class MinerStop : GenericRpcRequestResponseHandlerNoParam<bool>, IMinerStop
    {
        public MinerStop(IClient client) : base(client, ApiMethods.miner_stop.ToString())
        {
        }
    }
}