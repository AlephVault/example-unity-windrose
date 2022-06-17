using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Clique
{
    /// <Summary>
    ///     Returns current proposals.
    /// </Summary>
    public class CliqueProposals : GenericRpcRequestResponseHandlerNoParam<JObject>, ICliqueProposals
    {
        public CliqueProposals(IClient client) : base(client, ApiMethods.clique_proposals.ToString())
        {
        }
    }
}