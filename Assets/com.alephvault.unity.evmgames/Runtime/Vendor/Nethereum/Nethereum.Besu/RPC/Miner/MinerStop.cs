using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Miner
{
    /// <Summary>
    ///     Stops the CPU mining process on the client.
    /// </Summary>
    public class MinerStop : GenericRpcRequestResponseHandlerNoParam<bool>, IMinerStop
    {
        public MinerStop(IClient client) : base(client, ApiMethods.miner_stop.ToString())
        {
        }
    }
}