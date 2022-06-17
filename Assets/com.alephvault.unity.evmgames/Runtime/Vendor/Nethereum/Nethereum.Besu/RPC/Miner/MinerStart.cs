using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Miner
{
    /// <Summary>
    ///     Starts the CPU mining process. To start mining, a miner coinbase must have been previously specified using the
    ///     --miner-coinbase command line option.
    /// </Summary>
    public class MinerStart : GenericRpcRequestResponseHandlerNoParam<bool>, IMinerStart
    {
        public MinerStart(IClient client) : base(client, ApiMethods.miner_start.ToString())
        {
        }
    }
}